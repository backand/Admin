using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc.Azure;
using Newtonsoft.Json;
using System.Diagnostics;
using Durados.Data;
using Durados.SmartRun;
using Durados.Web.Mvc.Farm;

namespace Durados.Web.Mvc
{
    public class SqlPersistency : IPersistency
    {
        public string ConnectionString { get; set; }
        public string SystemConnectionString { get; set; }

        public object GetConnection(MapDataSet.durados_AppRow appRow, object builder)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, builder);
        }

        public object GetSystemConnection(MapDataSet.durados_AppRow appRow, object builder)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            if (appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System == null)
                return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, builder);
            else
                return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System, dataSourceTypeId, builder);

        }
        public object GetSecurityConnection(MapDataSet.durados_AppRow appRow, object builder)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            if (appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_Security == null)
                return null;//System.Configuration.ConfigurationManager.ConnectionStrings["SecurityConnectionString"];
            else
                return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_Security, dataSourceTypeId, builder);

        }

        public object GetLogConnection(MapDataSet.durados_AppRow appRow, object builder)
        {
            return GetSystemConnection(appRow, builder);// +";MultipleActiveResultSets=True;Asynchronous Processing=true;";

        }

        public object GetConnection(MapDataSet.durados_SqlConnectionRow sqlConnectionRow, int dataSourceTypeId, object builder)
        {
            return GetConnection(sqlConnectionRow, dataSourceTypeId, (System.Data.SqlClient.SqlConnectionStringBuilder)builder);
        }

        public object GetConnection(MapDataSet.durados_SqlConnectionRow sqlConnectionRow, int dataSourceTypeId, System.Data.SqlClient.SqlConnectionStringBuilder builder)
        {
            return GetConnection(sqlConnectionRow, dataSourceTypeId, builder, GetConnectionStringTemplate(sqlConnectionRow));
        }

        private string GetConnectionStringTemplate(MapDataSet.durados_SqlConnectionRow sqlConnectionRow)
        {
            int sqlProductId = sqlConnectionRow.SqlProductId;
            if (((SqlProduct)sqlProductId) == SqlProduct.MySql)
                return ConnectionStringHelper.GetConnectionStringSchema(sqlConnectionRow);
            return "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;";
        }

        public object GetSqlServerConnection(MapDataSet.durados_AppRow appRow, object builder)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, (System.Data.SqlClient.SqlConnectionStringBuilder)builder, "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;");
        }

        public object GetMySqlConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            bool usesSsh = !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshUsesNull() && appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshUses;
            return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, (System.Data.SqlClient.SqlConnectionStringBuilder)builder, usesSsh ? "server=localhost;database={1};User Id={2};password={3};Allow User Variables=True;CharSet=utf8;UseProcedureBodies=true;" : "server={0};database={1};User Id={2};password={3}") + ";port=" + localPort.ToString() + ";convert zero datetime=True;default command timeout=90;Connection Timeout=60;Allow User Variables=True;CharSet=utf8;UseProcedureBodies=true;";
        }

        public object GetPostgreConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            bool usesSsl = !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSslUsesNull() && appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SslUses;
            return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, (System.Data.SqlClient.SqlConnectionStringBuilder)builder, usesSsl ? "server={0};database={1};User Id={2};password={3};SSL=true;SslMode=Require;" : "server={0};database={1};User Id={2};password={3}") + ";port=" + localPort.ToString() + ";Encoding=UNICODE;CommandTimeout=90;Timeout=60;";
        }

        public object GetOracleConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            bool usesSsh = !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshUsesNull() && appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshUses;
            return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, (System.Data.SqlClient.SqlConnectionStringBuilder)builder, OracleAccess.GetConnectionStringSchema());
        }
        public object GetConnection(MapDataSet.durados_SqlConnectionRow sqlConnectionRow, int dataSourceTypeId, System.Data.SqlClient.SqlConnectionStringBuilder builder, string template)
        {
            string connectionString = null;
            string serverName = null;
            bool? integratedSecurity = null;

            if (dataSourceTypeId == 2 || dataSourceTypeId == 4)
            {
                if (sqlConnectionRow.IsServerNameNull())
                    serverName = builder.DataSource;
                else
                    serverName = sqlConnectionRow.ServerName;

                if (sqlConnectionRow.IsIntegratedSecurityNull())
                    integratedSecurity = builder.IntegratedSecurity;
                else
                    integratedSecurity = sqlConnectionRow.IntegratedSecurity;
            }
            else
            {
                integratedSecurity = builder.IntegratedSecurity;
                serverName = builder.DataSource;
            }

            if (integratedSecurity.HasValue && integratedSecurity.Value)
            {
                connectionString = "Data Source={0};Initial Catalog={1};Integrated Security=True;";
                return string.Format(connectionString, serverName, sqlConnectionRow.Catalog);
            }
            else
            {
                //connectionString = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;";
                connectionString = template;
                string username = null;
                string password = null;
                if (dataSourceTypeId == 2 || dataSourceTypeId == 4)
                {

                    if (sqlConnectionRow.IsUsernameNull())
                        username = builder.UserID;
                    else
                        username = sqlConnectionRow.Username;
                    if (sqlConnectionRow.IsPasswordNull())
                        password = builder.Password;
                    else
                        password = sqlConnectionRow.Password;

                }
                else
                {
                    username = builder.UserID;
                    password = builder.Password;
                }
                if (!sqlConnectionRow.IsProductPortNull())
                    return string.Format(connectionString, serverName, sqlConnectionRow.Catalog, username, password, sqlConnectionRow.ProductPort);
                else
                    return string.Format(connectionString, serverName, sqlConnectionRow.Catalog, username, password);
            }
        }

    }
}
