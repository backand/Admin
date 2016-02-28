using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Data.SqlClient;

using Durados.DataAccess;
using Durados.Web.Mvc;
namespace Durados.Web.Mvc.App.Controllers
{
    [HandleError]
    public class ConnectionStringController : Durados.Web.Mvc.Controllers.DuradosController
    {
        string ServernameFieldName = "ServerName";
        string CatalogFieldName = "Catalog";
        string UsernameFieldName = "Username";
        string PasswordFieldName = "Password";
        string IntegratedSecurityFieldName = "IntegratedSecurity";
        string DuradosUserFieldName = "DuradosUser";
        string ProductPortFieldName = "ProductPort";

        string SshRemoteHost = "SshRemoteHost";
        string SshPort = "SshPort";
        string SshUser = "SshUser";
        string SshPassword = "SshPassword";
        string SshPrivateKey = "SshPrivateKey";
        string SshUses = "SshUses";
        string SslUses = "SslUses";
        string ProductPort = "ProductPort";

        protected override void AfterCreateBeforeCommit(CreateEventArgs e)
        {
            base.AfterCreateBeforeCommit(e);

            ValidateConnectionString(e);
        }
        protected override void AfterEditBeforeCommit(EditEventArgs e)
        {
            base.AfterEditBeforeCommit(e);

            ValidateConnectionString(e);

            try
            {
                UpdateProductCache(e);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "UpdateProductCache exception");

            }
        }

        private void UpdateProductCache(EditEventArgs e)
        {
            int id = Convert.ToInt32(e.PrimaryKey);
            string sqlProductColumnName = "SqlProductId";
            string sqlProductFieldName = e.View.GetFieldByColumnNames(sqlProductColumnName).Name;
            SqlProduct sqlProduct = (SqlProduct)Enum.Parse(typeof(SqlProduct), e.Values.ContainsKey(sqlProductFieldName) ? e.Values[sqlProductFieldName].ToString() : SqlProduct.SqlServer.ToString());
            SqlProduct prevSqlProduct = (SqlProduct)Enum.Parse(typeof(SqlProduct), e.PrevRow.Table.Columns.Contains(sqlProductColumnName) ? e.PrevRow[sqlProductColumnName].ToString() : SqlProduct.SqlServer.ToString());

            if (prevSqlProduct != sqlProduct)
            {
                UpdateProductCache(id, sqlProduct, e.Command);
            }
        }

        private void UpdateProductCache(int id, SqlProduct sqlProduct, IDbCommand command)
        {
            string[] apps = GetAppsName(id, command);

            foreach (string appName in apps)
            {
                Maps.UpdateSqlProduct(appName, sqlProduct);
            }
        }

        private string[] GetAppsName(int id, IDbCommand command)
        {
            command.CommandText = "SELECT dbo.durados_App.Name FROM dbo.durados_App with(nolock) INNER JOIN dbo.durados_SqlConnection with(nolock) ON dbo.durados_App.SqlConnectionId = dbo.durados_SqlConnection.Id WHERE (dbo.durados_SqlConnection.Id = 1)";

            List<string> apps = new List<string>();

            IDataReader reader = command.ExecuteReader();
            int ord = reader.GetOrdinal("Name");

            while (reader.Read())
            {
                string appName = reader.GetString(ord);
                if (!string.IsNullOrEmpty(appName))
                    apps.Add(appName);
            }

            reader.Close(); 
            
            return apps.ToArray();
        }

        private void ValidateConnectionString(DataActionEventArgs e)
        {
            OpenSshSessionIfNecessary(e);

            bool? integratedSecurity = null;
            bool integratedSecurityTmp;
            string connectionString = null;
            string serverName = e.Values.ContainsKey(ServernameFieldName) ? e.Values[ServernameFieldName].ToString() : string.Empty;
            string catalog = e.Values.ContainsKey(CatalogFieldName) ? e.Values[CatalogFieldName].ToString() : string.Empty;
            string username = e.Values.ContainsKey(UsernameFieldName) ? e.Values[UsernameFieldName].ToString() : string.Empty;
            string password = e.Values.ContainsKey(PasswordFieldName) ? e.Values[PasswordFieldName].ToString() : string.Empty;
            bool usesSsh = e.Values.ContainsKey(SshUses) ? Convert.ToBoolean(e.Values[SshUses].ToString()) : false;
            bool usesSsl = e.Values.ContainsKey(SslUses) ? Convert.ToBoolean(e.Values[SslUses].ToString()) : false;

            string sqlProductFieldName = e.View.GetFieldByColumnNames("SqlProductId").Name;
            SqlProduct sqlProduct = (SqlProduct)Enum.Parse(typeof(SqlProduct), e.Values.ContainsKey(sqlProductFieldName) ? e.Values[sqlProductFieldName].ToString() : SqlProduct.SqlServer.ToString());

            if (sqlProduct == SqlProduct.MySql && !usesSsh)
                localPort = e.Values.ContainsKey(ProductPortFieldName) ? Convert.ToInt32(e.Values[ProductPortFieldName]) : localPort;

            if (sqlProduct == SqlProduct.Postgre && !usesSsh)
                localPort = e.Values.ContainsKey(ProductPortFieldName) ? Convert.ToInt32(e.Values[ProductPortFieldName]) : localPort;

            if (sqlProduct == SqlProduct.Oracle)
                localPort = e.Values.ContainsKey(ProductPortFieldName) ? Convert.ToInt32(e.Values[ProductPortFieldName]) : localPort;

            if (e.Values.ContainsKey(IntegratedSecurityFieldName))
                if (bool.TryParse(e.Values[IntegratedSecurityFieldName].ToString(), out integratedSecurityTmp))
                    integratedSecurity = integratedSecurityTmp;
            string duradosUserId = e.Values.ContainsKey(DuradosUserFieldName) ? e.Values[DuradosUserFieldName].ToString() : string.Empty;

            //string sqlProductFieldName = e.View.GetFieldByColumnNames("SqlProductId").Name;
            //SqlProduct sqlProduct = (SqlProduct)Enum.Parse(typeof(SqlProduct), e.Values.ContainsKey(sqlProductFieldName) ? e.Values[sqlProductFieldName].ToString() : SqlProduct.SqlServer.ToString());


            //connectionString = GetConnection(serverName, catalog, integratedSecurity, username, password, duradosUserId);
            //SqlConnection connection = new SqlConnection(connectionString);

            connectionString = GetConnection(serverName, catalog, integratedSecurity, username, password, duradosUserId, sqlProduct, localPort, usesSsh, usesSsl);
            IDbConnection connection = GetNewConnection(sqlProduct, connectionString);

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
            catch (MySql.Data.MySqlClient.MySqlException ex)
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
            catch (Npgsql.NpgsqlException ex)
            {
                throw new DuradosException(ex.Message, ex);
            }

            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                throw new DuradosException(ex.Message, ex);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                CloseSshSessionIfNecessary();
            }
        }

         private void CloseSshSessionIfNecessary()
        {
            if (session != null)
                session.Close();
        }

        Durados.Security.Ssh.ISession session = null;
        int localPort = 0;
        private void OpenSshSessionIfNecessary(DataActionEventArgs e)
        {
            bool usingSsh = e.Values.ContainsKey(SshUses) && !e.Values[SshUses].Equals(string.Empty) ? Convert.ToBoolean(e.Values[SshUses]) : false;
            if (usingSsh)
            {
                Durados.Security.Ssh.ITunnel tunnel = new Durados.DataAccess.Ssh.Tunnel();

                tunnel.RemoteHost = e.Values.ContainsKey(SshRemoteHost) ? e.Values[SshRemoteHost].ToString() : string.Empty;
                tunnel.User = e.Values.ContainsKey(SshUser) ? e.Values[SshUser].ToString() : string.Empty;
                tunnel.Password = e.Values.ContainsKey(SshPassword) ? e.Values[SshPassword].ToString() : string.Empty;
                tunnel.PrivateKey = e.Values.ContainsKey(SshPrivateKey) ? e.Values[SshPrivateKey].ToString() : string.Empty;
                tunnel.Port = e.Values.ContainsKey(SshPort) && !e.Values[SshPort].Equals(string.Empty) ? Convert.ToInt32(e.Values[SshPort]) : 22;
                int remotePort = e.Values.ContainsKey(ProductPort) && !e.Values[ProductPort].Equals(string.Empty) ? Convert.ToInt32(e.Values[ProductPort]) : 3306;
                localPort = Maps.Instance.AssignLocalPort();
                
                //session = new Durados.DataAccess.Ssh.ChilkatSession(tunnel, remotePort, localPort);
                //session.Open();
            }
        }

        
    }

}

