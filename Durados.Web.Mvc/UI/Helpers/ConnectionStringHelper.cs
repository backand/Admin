using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class ConnectionStringHelper
    {
        public class ConnectionParameter
        {
            public string serverName { get; set; }
            public string catalog { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string port { get; set; }
            public bool integratedSecurity { get; set; }
            public string connectionString { get; set; }
            public string formatConnectionString { get; set; }

        } ;

        public static void ValidateConnectionString(string serverName, string catalog, string username, string password, bool? integratedSecurity, string connectionStringExample, SqlProduct sqlProduct, int localPort, bool usesSsh)
        {
            string connectionString = GetConnection(serverName, catalog, integratedSecurity, username, password, connectionStringExample, sqlProduct, localPort, usesSsh);
            ValidateConnectionString(connectionString);
        }

        public static void ConnectionParams(string connectionString)
        {

            SqlConnectionStringBuilder decoder = new SqlConnectionStringBuilder(connectionString);

            string UserID = decoder.UserID;
            string Password = decoder.Password;

            ConnectionParameter oConnectionParameter = new ConnectionParameter();
            oConnectionParameter.username = decoder.UserID;
            oConnectionParameter.password = decoder.Password;
            oConnectionParameter.catalog = decoder.InitialCatalog;
            oConnectionParameter.integratedSecurity = decoder.IntegratedSecurity;

        }
        public static void ValidateConnectionString(string connectionString)
        {
            ValidateConnectionString(new SqlConnection(connectionString));
        }

        public static void ValidateConnectionString(System.Data.IDbConnection connection)
        {
            try
            {
                connection.Open();
            }
            catch (InvalidOperationException ex)
            {
                throw new DuradosException("Connection to Database Faild. Please check connection fields.", ex);
            }
            catch (SqlException ex)
            {

                //string message = string.Empty;
                //switch (ex.Class)
                //{
                //    case 20: message += "Error Locating Server/Instance Specified.<br>"; //"Server name is missing or does not exist.<br>";
                //        break;
                //    case 11: message += "Cannot open database.<br>" ;
                //        break;
                //    case 14: message += "Loging Failed.<br>";
                //        break;
                //    default: message += "Connection string test failed.<br>";
                //        break;
                //}
                throw new DuradosException(ex.Message, ex);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public static string GetConnection(string serverName, string catalog, bool? integratedSecurity, string username, string password, string connectionStringExample, SqlProduct sqlProduct, int localPort, bool usesSsh)
        {

            string connectionString = null;
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = connectionStringExample;

            bool hasServer = !string.IsNullOrEmpty(serverName);
            bool hasCatalog = !string.IsNullOrEmpty(catalog);


            if (!hasCatalog)
                throw new DuradosException("Catalog Name is missing");


            if (integratedSecurity.HasValue && integratedSecurity.Value)
            {
                if (!hasServer)
                    serverName = builder.DataSource;

                connectionString = "Data Source={0};Initial Catalog={1};Integrated Security=True;";

                return string.Format(connectionString, serverName, catalog);
            }
            else
            {

                connectionString = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;";
                if (sqlProduct == SqlProduct.MySql)
                {
                    if (usesSsh)
                        serverName = "localhost";
                    connectionString = "server={0};database={1};User Id={2};password={3};port={4};convert zero datetime=True";
                }

                bool hasUsername = !string.IsNullOrEmpty(username);
                bool hasPassword = !string.IsNullOrEmpty(password);

                if (!hasServer)
                    serverName = builder.DataSource;

                if (!hasUsername)
                    username = builder.UserID;

                if (!hasPassword)
                    password = builder.Password;

                return string.Format(connectionString, serverName, catalog, username, password, localPort);

            }
        }

        public static TroubleshootInfo GetTroubleshootInfo(Exception exception, string serverName, string catalog, string username, string password, bool usesSsh, SqlProduct sqlProduct, string sshRemoteHost, string sshUser, string sshPassword, int sshPort, int productPort)
        {
            string ourIp = "137.117.97.68";

            TroubleshootInfo troubleshootInfo = new TroubleshootInfo();
            string exceptionMessage = exception.Message;
            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                exceptionMessage += "\n\r" + innerException.Message;
                innerException = innerException.InnerException;
            }

            troubleshootInfo.Exception = exceptionMessage;
            troubleshootInfo.StackTrace = exception.StackTrace;
            troubleshootInfo.Id = 0;

            if (serverName.StartsWith("192.168"))
            {
                troubleshootInfo.Field = "Server";
                troubleshootInfo.Cause = "The server ip address you provided is a private network address";
                troubleshootInfo.Fix = "Make sure your server has a remote access and provide the public ip address of the server.";
                troubleshootInfo.Id = 3001;
            }
            else if (serverName.Equals(ourIp))
            {
                troubleshootInfo.Field = "Server";
                troubleshootInfo.Cause = "The server ip address you provided is ours";
                troubleshootInfo.Fix = "Please provide your server ip address";
                troubleshootInfo.Id = 3002;
            }

            else if (sqlProduct == SqlProduct.MySql)
            {
                if (exceptionMessage.Contains("11004") || exceptionMessage.Contains("11001"))
                {
                    troubleshootInfo.Field = "Server";
                    troubleshootInfo.Cause = "The server was not found or was not accessible";
                    troubleshootInfo.Fix = "Make sure the server name is spelled correct, up and running and allow remote access";
                    troubleshootInfo.Id = 1001;
                    troubleshootInfo.Internal = exceptionMessage.Contains("11004") ? 11004 : 11001;
                }
                else if (exceptionMessage.Contains("Unknown database"))
                {
                    troubleshootInfo.Field = "Database";
                    troubleshootInfo.Cause = "The database does not exist";
                    troubleshootInfo.Fix = "If it is, grant permissions on this database for user " + username + "<br>" + "GRANT ALL PRIVILEGES ON " + catalog + ".* to '" + username + "'@'" + serverName + "' identified by '[password]'<br>Replace the [password] value with its appropriate replacement.";
                    troubleshootInfo.Id = 1003;
                }
                else if (exceptionMessage.Contains("Access denied for user"))
                {
                    troubleshootInfo.Field = "Database,Username,Password";
                    troubleshootInfo.Cause = "The database is not accessible for user " + username;
                    troubleshootInfo.Fix = "If it is, grant permissions on this database for user " + username + "<br>" + "GRANT ALL PRIVILEGES ON " + catalog + ".* to '" + username + "'@'" + serverName + "' identified by '[password]'<br>Replace the [password] value with its appropriate replacement.";
                    troubleshootInfo.Id = 1004;
                }
                else if (exceptionMessage.Contains("Unable to connect to any of the specified MySQL hosts"))
                {
                    troubleshootInfo.Cause = "The server " + serverName + " is not reachable.";
                    troubleshootInfo.Fix = "Please enable firewall exception on port " + productPort + " for ip: " + ourIp;
                    troubleshootInfo.Id = 1002;
                }
                else
                {
                    troubleshootInfo.Cause = "Unknown";
                    troubleshootInfo.Id = 0;
                }
            }
            else
            {
                if (exceptionMessage.Contains("error: 40 "))
                {
                    troubleshootInfo.Field = "Server";
                    troubleshootInfo.Cause = "The server was not found or was not accessible";
                    troubleshootInfo.Fix = "Make sure the server name is spelled correct, up and running and allow remote access";
                    troubleshootInfo.Id = 2001;
                    troubleshootInfo.Internal = 40;
                }
                else if (exceptionMessage.Contains("error: 26 "))
                {
                    troubleshootInfo.Cause = "There is no remote access to this server";
                    troubleshootInfo.Fix = "Please enable firewall exception on port 1433" + " for ip: " + ourIp;
                    troubleshootInfo.Id = 2002;
                    troubleshootInfo.Internal = 26;
                }
                else
                {
                    troubleshootInfo.Cause = "Unknown";
                    troubleshootInfo.Id = 0;
                }
            }

            return troubleshootInfo;
        }


        public static System.Data.IDbConnection GetConnection(string connectionString)
        {

            if (Durados.DataAccess.OracleAccess.IsOracleConnectionString(connectionString))
                return new Durados.DataAccess.OracleSchema().GetConnection(connectionString); 
            else if (Durados.DataAccess.PostgreAccess.IsPostgreConnectionString(connectionString))
                return new Durados.DataAccess.PostgreSchema().GetConnection(connectionString);
            else if (Durados.DataAccess.MySqlAccess.IsMySqlConnectionString(connectionString))
                return new Durados.DataAccess.MySqlSchema().GetConnection(connectionString);
            return new SqlSchema().GetConnection(connectionString);
        
        }
        public static string GetConnectionStringSchema(MapDataSet.durados_SqlConnectionRow sqlConnectionRow)
        {

            bool usesSsh = !sqlConnectionRow.IsSshUsesNull() && sqlConnectionRow.SshUses;
            bool usesSsl = !sqlConnectionRow.IsSslUsesNull() && sqlConnectionRow.SslUses;
            string localPort = sqlConnectionRow.ProductPort;
            SqlProduct sqlProductId = (SqlProduct)sqlConnectionRow.SqlProductId;
           
            switch ((SqlProduct)sqlProductId)
            {
                case SqlProduct.MySql:
                    return Durados.DataAccess.MySqlAccess.GetConnectionStringSchema(usesSsh);
                    
                case SqlProduct.Postgre:
                    return Durados.DataAccess.PostgreAccess.GetConnectionStringSchema(usesSsl);
                    
                case SqlProduct.Oracle:
                    return Durados.DataAccess.OracleAccess.GetConnectionStringSchema();
                
                default:
                    return "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;";
                    
            }
        }
    }

    public class TroubleshootInfo
    {
        public string StackTrace { get; set; }
        public string Exception { get; set; }
        public string Cause { get; set; }
        public string Fix { get; set; }
        public string Field { get; set; }
        public int Id { get; set; }
        public int Internal { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
