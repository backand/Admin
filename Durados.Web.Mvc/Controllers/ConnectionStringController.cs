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

        protected override void AfterCreateBeforeCommit(CreateEventArgs e)
        {
            base.AfterCreateBeforeCommit(e);

            ValidateConnectionString(e);
        }
        protected override void AfterEditBeforeCommit(EditEventArgs e)
        {
            base.AfterEditBeforeCommit(e);

            ValidateConnectionString(e);
        }


        private void ValidateConnectionString(DataActionEventArgs e)
        {
            bool? integratedSecurity = null;
            bool integratedSecurityTmp;
            string connectionString = null;
            string serverName = e.Values.ContainsKey(ServernameFieldName) ? e.Values[ServernameFieldName].ToString() : string.Empty;
            string catalog = e.Values.ContainsKey(CatalogFieldName) ? e.Values[CatalogFieldName].ToString() : string.Empty;
            string username = e.Values.ContainsKey(UsernameFieldName) ? e.Values[UsernameFieldName].ToString() : string.Empty;
            string password = e.Values.ContainsKey(PasswordFieldName) ? e.Values[PasswordFieldName].ToString() : string.Empty;
            if (e.Values.ContainsKey(IntegratedSecurityFieldName))
                if (bool.TryParse(e.Values[IntegratedSecurityFieldName].ToString(), out integratedSecurityTmp))
                    integratedSecurity = integratedSecurityTmp;
            string duradosUserId = e.Values.ContainsKey(DuradosUserFieldName) ? e.Values[DuradosUserFieldName].ToString() : string.Empty;


            connectionString = GetConnection(serverName, catalog, integratedSecurity, username, password, duradosUserId);
            SqlConnection connection = new SqlConnection(connectionString);

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

                string message = string.Empty;
                switch (ex.Class)
                {
                    case 20: message += "Error Locating Server/Instance Specified.<br>"; //"Server name is missing or does not exist.<br>";
                        break;
                    case 11: message += "Cannot open database.<br>" ;
                        break;
                    case 14: message += "Loging Failed.<br>";
                        break;
                    default: message += "Connection string test failed.<br>";
                        break;
                }
                throw new DuradosException(message, ex);
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

        public string GetConnection(string serverName, string catalog, bool? integratedSecurity, string username,string password,string duradosuserId)
        {
            
            string connectionString = null;
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = Map.connectionString;

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

                bool hasUsername=!string.IsNullOrEmpty(username);
                bool hasPassword=!string.IsNullOrEmpty(password);
               
                if(!hasServer)
                    serverName = builder.DataSource;
                
                if(!hasUsername)
                     username = builder.UserID;
                
                if (!hasPassword)
                    password = builder.Password;
                
                return string.Format(connectionString, serverName, catalog, username, password);  
                
            }
        }
    }

}

