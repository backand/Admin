using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Durados.DataAccess;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class AppFactory
    {
        const string AppViewName = "durados_App";
        const string ConnectionViewName = "durados_SqlConnection";
      
        
        public  NewDatabaseParameters GetNewExternalDBParameters(Durados.SqlProduct sqlProduct, string id, out string server,out int port,string sampleApp)//, out string catalog
        {
            // set all the database parameters for the createapp

                Durados.Web.Mvc.UI.Helpers.NewDatabaseParameters newDbParameters = new Durados.Web.Mvc.UI.Helpers.RDSNewDatabaseFactory().GetNewParameters(sqlProduct, id);
            //catalog = newDbParameters.DbName;
            CreateNewSchemaAndUser(sqlProduct, out server,out port, newDbParameters,sampleApp);
            //server =  newDbParameters.InstanceName; //System.Configuration.ConfigurationManager.AppSettings["AWSServer"]??".";//aws database server
            //catalog = sqlProduct == Durados.SqlProduct.SqlServer ? newDbParameters.InstanceName : newDbParameters.DbName;
            return newDbParameters;

        }
        private void CreateNewSchemaAndUser(SqlProduct sqlProduct,out string server,out int port, NewDatabaseParameters newDbParameters,string sampleApp)
        {
            string sql =string.Format( @"CREATE  DATABASE IF NOT EXISTS `{0}`;
                            CREATE USER '{1}'@'%'  IDENTIFIED BY  '{2}';
                            GRANT ALL ON `{0}`.* TO '{1}'@'%';", newDbParameters.DbName, newDbParameters.Username, newDbParameters.Password);
            if (!string.IsNullOrEmpty(sampleApp))
            {
                string scriptFileName = Maps.GetDeploymentPath(string.Format("Sql/SampleApp/{0}.sql",sampleApp));
                
                System.IO.FileInfo file = new System.IO.FileInfo(scriptFileName);
                if (!file.Exists)
                {
                    Maps.Instance.DuradosMap.Logger.Log("myAppConnectionController", "Post","CreateNewSchemaAndUser", "Sample app file do not exists",null,1,"Sample app file do not exists",DateTime.Now);
                    throw new Exception("Sample app file do not exists"); 
                }
                string script = file.OpenText().ReadToEnd();
                sql += script.Replace("__DB__Name__", newDbParameters.DbName);
            }
            using (System.Data.IDbConnection connection = GetExternalAvailableInstanceConnection(sqlProduct, out server,out port))
            {
                if (connection == null)
                    throw new Exception("Failed to set a connection to external available instance");
                if (connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();
                using (System.Data.IDbTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                   
                    using (System.Data.IDbCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = sql;
                        try
                        {
                            //if (command.Connection.State == System.Data.ConnectionState.Closed)
                            //    command.Connection.Open();
                            command.ExecuteScalar();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                             if (connection != null && connection.State != System.Data.ConnectionState.Closed)
                               transaction.Rollback();
                            
                            Maps.Instance.DuradosMap.Logger.Log("AppFactory", null, "CreateNewSchemaAndUser", ex, 1, "Faild to create new schema for new rds app");
                            throw new Exception("Faild to create new schema for new rds app", ex);
                        }
                    }
                }

            }

        }
        private System.Data.IDbConnection GetExternalAvailableInstanceConnection(SqlProduct product,out string server,out int port)
        {
            
            string catalog =null;
            string username = null;
            string password = null;
            server = null;
            port = 0;
            string spName = "durados_GetExternalAvailableInstance";
            int? connectionId= null;
            using (System.Data.IDbConnection cnn = Durados.DataAccess.DataAccessObject.GetNewConnection(SqlProduct.SqlServer, Maps.Instance.ConnectionString))
            {
                using (DuradosCommand command = new DuradosCommand(GetSystemProduct()))
                {
                    command.Connection = cnn;
                    command.CommandText = spName;
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    if (command.Connection.State == System.Data.ConnectionState.Closed)
                    {
                        try
                        {
                            command.Connection.Open();
                        }
                        catch(Exception ex)
                        {
                            Maps.Instance.DuradosMap.Logger.Log("AppFactory", null, "GetExternalAvailableInstanceConnection", null, 1, "No connection to main database");
                            throw new Exception("No connection to main database", ex);
                        }
                    }
                    System.Data.IDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                        connectionId = reader.GetInt32(reader.GetOrdinal("SqlConnectionId"));
                }
            }

            Durados.Web.Mvc.View view = GetView(ConnectionViewName);
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Id", "&&%&=&&%& " + connectionId.Value.ToString());
            int rowCount = 0;
            System.Data.DataView dataView = view.FillPage(1, 2, values, false, null, out rowCount, null, null);
            if (dataView == null || rowCount != 1)
            {
                Maps.Instance.DuradosMap.Logger.Log("AppFactory", null, "GetExternalAvailableInstanceConnection", null, 1, "Failed to retrive available external instance = no data");
                throw new Exception("Failed to retrive available external instance = no data");
            }

            try
            {
                password = Convert.ToString(dataView[0]["Password"]);
                username = Convert.ToString(dataView[0]["Username"]);
                server = Convert.ToString(dataView[0]["ServerName"]);
                catalog = Convert.ToString(dataView[0]["Catalog"]);
                port = Convert.ToInt32(dataView[0]["ProductPort"]);

            }
            catch(Exception ex)
            {
                Maps.Instance.DuradosMap.Logger.Log("AppFactory", null, "GetExternalAvailableInstanceConnection", ex, 1, "Missing external instance parameters or converion errors");
                throw new Exception("Missing external instance parameters or converion errors");
            }
            string connectionString = GetConnectionString(server, catalog, false, username, password, null, product,port, false, false);

            return GetConnection(product, connectionString);           

         
        }
        protected virtual View GetView(string viewName)
        {
            return (View)Maps.Instance.DuradosMap.Database.Views[viewName];
        }
        private SqlProduct GetSystemProduct()
        {
            return SqlProduct.SqlServer;
        }
        public static string GetConnectionString(string serverName, string catalog, bool? integratedSecurity, string username, string password, string duradosuserId, Durados.SqlProduct sqlProduct, int localPort, bool usesSsh, bool usesSsl)
        {

            string connectionString = null;
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = Maps.Instance.ConnectionString;

            bool hasServer = !string.IsNullOrEmpty(serverName);
            bool hasCatalog = !string.IsNullOrEmpty(catalog);


            if (!hasCatalog)
                throw new Durados.DuradosException("Catalog Name is missing");


            if (integratedSecurity.HasValue && integratedSecurity.Value)
            {
                if (!hasServer)
                {
                    serverName = builder.DataSource;

                }
                connectionString = "Data Source={0};Initial Catalog={1};Integrated Security=True;";
                return string.Format(connectionString, serverName, catalog);
            }
            else
            {

                connectionString = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;";
                if (sqlProduct == Durados.SqlProduct.MySql)
                {
                    if (usesSsh)
                        connectionString = "server=localhost;database={1};User Id={2};password={3};port={4};convert zero datetime=True";
                    else
                        connectionString = "server={0};database={1};User Id={2};password={3};port={4};convert zero datetime=True";
                }
                if (sqlProduct == Durados.SqlProduct.Postgre)
                {
                    if (usesSsl)
                        if (usesSsh)
                            connectionString = "server=localhost;database={1};User Id={2};password={3};port={4};SSL=true;SslMode=Require;";
                        else
                            connectionString = "server={0};database={1};User Id={2};password={3};port={4};SSL=true;SslMode=Require;";
                    else
                        if (usesSsh)
                            connectionString = "server=localhost;database={1};User Id={2};password={3};port={4};Encoding=UNICODE;";
                        else
                            connectionString = "server={0};database={1};User Id={2};password={3};port={4};Encoding=UNICODE;";
                }
                if (sqlProduct == Durados.SqlProduct.Oracle)
                {
                    connectionString = Durados.DataAccess.OracleAccess.GetConnectionStringSchema();


                }

                bool hasUsername = !string.IsNullOrEmpty(username);
                bool hasPassword = !string.IsNullOrEmpty(password);


                if (!hasServer)
                {
                    if (Maps.AllowLocalConnection)
                        serverName = builder.DataSource;
                    else
                        throw new Durados.DuradosException("Server Name is missing");
                }

                if (!hasUsername)
                {
                    if (Maps.AllowLocalConnection)
                        username = builder.UserID;
                    else
                        throw new Durados.DuradosException("Username Name is missing");
                }

                if (!hasPassword)
                {
                    if (Maps.AllowLocalConnection)
                        password = builder.Password;
                    else
                        throw new Durados.DuradosException("Password Name is missing");
                }

                return string.Format(connectionString, serverName, catalog, username, password, localPort);

            }
        }

        public static  System.Data.IDbConnection GetConnection(Durados.SqlProduct sqlProduct, string connectionString)
        {
            return Durados.DataAccess.DataAccessObject.GetNewConnection(sqlProduct, connectionString);
        }



    }
}
