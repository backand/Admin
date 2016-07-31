using Durados.DataAccess;
using Durados.Web.Mvc;
using Durados.Windows.Utilities.AzureUploader;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Windows.Utilities
{
    public class ProductMaintenance
    {
        static Durados.Web.Mvc.Logging.Logger logger = new Web.Mvc.Logging.Logger();
        public ProductMaintenance()
        {
            logger.ConnectionString = Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.LocalConnection;
        }

        public ProductMaintenance(System.Windows.Forms.RichTextBox resultOutput)
            : base()
        {
            // TODO: Complete member initialization
            this.resultOutput = resultOutput;

        }
        public static string toDeleteViewName = "v_toDeleteApps";
        private System.Windows.Forms.RichTextBox resultOutput = null;



        public string RemoveApps(DataTable dt, IDbCommand command)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (dt == null)
            {
                dt = AppFactory.GetToDeleteAppsTable(command);
            }
            if (dt == null)
                return "No apps to delete";
            //Connections connection = new Connections();
            //connection.Init();

            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    App app = new App(dr);
                    RemoveApp(app);
                    string msg = string.Format("Completely remove app number {0}", dr["Id"].ToString());
                    if(resultOutput!= null)resultOutput.Text += "\n\r" + msg;
                    stringBuilder.AppendFormat("<br>{0}" , msg);
                    logger.Log("ProductMaintenance", "RemoveApps", "RemoveApps", null, 3, string.Format("Completely remove app number {0}.", dr["Id"].ToString()));

                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not completely remove app number {0}.", dr["Id"].ToString());
                    if (resultOutput != null) resultOutput.Text += "\n\r" + msg;
                    logger.Log("ProductMaintenance", "RemoveApps", "RemoveApps", ex, 2,msg );
                    stringBuilder.AppendFormat("<br>Failed to completely remove app number" + dr["Id"].ToString() + ". Cause: " + ex.Message);
                }
            }
            return stringBuilder.ToString();

        }
        //public void RemoveApp(int appId,string appName)
        //{
        //    App app = (appId > 0) ? new AppFactory().GetAppById(appId) : new AppFactory().GetAppByName(appName);
        //   if(app== null || app.AppId<=0)
        //       throw new DuradosException("App was not found");
        //    RemoveApp(app);
        //}
        public bool RemoveApp(string appName)
        {
            if (string.IsNullOrEmpty(appName))
                throw new DuradosException("App name is empty");
            App app = new AppFactory().GetAppByName(appName);
            if (app == null || app.AppId <= 0)
                throw new DuradosException("App was not found");
            return RemoveApp(app);
        }
        public bool RemoveApp(int appId)
        {
            if (appId <= 0)
                throw new DuradosException("App could not be 0");

            App app = new AppFactory().GetAppById(appId);
            if (app == null || app.AppId <= 0)
                throw new DuradosException("App was not found");
            return RemoveApp(app);
        }
        private bool RemoveApp(App app)
        {
            //if (connections == null)
            //{
            //   connections= new Connections();
            //   connections.Init();
            //}
            bool success = false;
            //Map map = null;
            string configFileName = string.Empty;
            //try
            //{
            //    map = Maps.Instance.GetMap(app.Name);
            //    configFileName = map.ConfigFileName;
            //}
            //catch
            //{
            string configFileprefix = !string.IsNullOrEmpty(Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.DuradosAppSysPrefix )? Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.DuradosAppSysPrefix :"durados_AppSys_";
            configFileName = configFileprefix + app.AppId + ".xml";
            //}
            SqlServerManager manager = Connections.GetSqlServerManager(app.Server);
            SqlServerManager sysManager = Connections.GetSqlServerManager(app.SystemServer);
            switch (app.AppType)
            {
                case 0:

                    break;

                case 2: //free

                case 3: //northwind
                    RemoveAppDb(app, manager);
                    goto case 1;
                case 1:// console
                    RemoveSystemDB(app, sysManager);
                    DeleteConfigurationFromCloud(configFileName);
                    break;
                case 4://wix
                    RemoveWixViews(app, manager);
                    DeleteLocalConfiguration(configFileName);
                    break;
                case 5:
                    break;
                default:
                    break;
            }


            logger.Log("ProductMaintenance", "RemoveApp", "RemoveApp", null, 3, "configuration files " + configFileName + " was remove from storage");
            UpdateDeletedApp(app.AppId);
            success = true;
            logger.Log("ProductMaintenance", "RemoveApp", "RemoveApp", null, 3, "App name: " + app.Name + " Number: " + app.AppId + " was completly removed");
            return success;

            // Maps.Instance.RemoveMap(app.Name);
        }

        private void DeleteLocalConfiguration(string configFileName)
        {
            string fullConfigFileName = Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.ConfigPath + configFileName;
            if (System.IO.File.Exists(fullConfigFileName))
                System.IO.File.Delete(fullConfigFileName);
            if (System.IO.File.Exists(fullConfigFileName + ".xml"))
                System.IO.File.Delete(fullConfigFileName + ".xml");

        }

        //private static void RemoveWixViews(Map map, SqlServerManager sysManager)
        //{
        //    if (map != null)
        //    {
        //        foreach (View view in map.Database.Views.Values)
        //            if (!view.SystemView)
        //                sysManager.DropTable(view);
        //    }
        //}
        private static void RemoveWixViews(App app, SqlServerManager manager)
        {
            SqlSchema schema = new SqlSchema();
            string  connectionString = Connections.GetConnectionString(app.Server,app.Catalog);
            HashSet<string> referencedTables= GetReferencedTables(app.TableName, connectionString);
            manager.DropTable(app.TableName, connectionString);
            foreach (string relatedTable in referencedTables)
                        manager.DropTable(relatedTable,connectionString);
            

        }

        private static HashSet<string> GetReferencedTables(string tableName, string connectionString)
        {
            HashSet<string> referenceTables = new HashSet<string>();
            string sql = @"SELECT KCU2.TABLE_NAME AS REFERENCED_TABLE_NAME 
	                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC 
	                    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU1 ON KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME 
	                    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU2 ON KCU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME WHERE KCU1.TABLE_NAME=N'" + tableName+"'";
            SqlSchema schema= new SqlSchema();
           using (IDbCommand command = schema.GetCommand())
            {
                command.Connection = schema.GetConnection(connectionString);
                command.CommandText = sql;
                
                try
                {
                    command.Connection.Open();
                    IDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        referenceTables.Add( reader["REFERENCED_TABLE_NAME"].ToString());
                    }
                    return referenceTables;

                }
                catch (Exception ex)
                { 
                    throw new DuradosException("<br>Failed to get referenced table for " + tableName);
                }

            }
            
            
        }

        private static void RemoveAppDb(App app, SqlServerManager manager)
        {
            if (manager == null)
                logger.Log("ProductMaintenance", null, "RemoveApp", null, 2, string.Format("Could not drop main db from app {0}, the database server was not found, the app type is {1}", app.Name, app.AppType.ToString()));
            else
                manager.DropDB(app.Server, app.Catalog);
        }

        private void RemoveSystemDB(App app, SqlServerManager sysManager)
        {
            if (sysManager == null)
                logger.Log("ProductMaintenance", null, "RemoveApp", null, 2, string.Format("Could not drop system db from app {0}, the database server was not found the app type is {1}", app.Name, app.AppType.ToString()));
            else
                sysManager.DropDB(app.SystemServer, app.SystemCatalog);

        }
        private void DeleteConfigurationFromCloud(string configFileName)
        {
            //TODO: use http web web call to remove config from cloud INCLUDING CACHE
            //Maps.Instance.RemoveConfigFromCloud(configFileName);
            //Maps.Instance.RemoveConfigFromCloud(configFileName + ".xml");
            RemoveConfigFromCloud(configFileName);
            RemoveConfigFromCloud(configFileName + ".xml");

        }
        private void RemoveConfigFromCloud(string fileName)
        {
            Storage storage = new Storage();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            string filenameOnly = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);
            string containerName = filenameOnly.Replace("_", "").Replace(".", "").ToLower();

            CloudBlobContainer container = storage.GetContainer(containerName);
            if(Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.DeleteFullContainerOndelete)
                container.Delete();
            else
            {
                    container.GetBlobReference(containerName).DeleteIfExists();
            }
            //Durados.Web.Mvc.Azure.LocalCache storageCache = new LocalCache();
            //if (Maps.Instance.StorageCache.ContainsKey(containerName))
            //{
            //    Maps.Instance.StorageCache.Remove(containerName);
            //}
        }
        private void UpdateDeletedApp(int appId)
        {
            SqlSchema schema = new SqlSchema();
            using (IDbCommand command = schema.GetCommand())
            {
                command.Connection = schema.GetConnection(Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.LocalConnection);
                command.CommandText = "UPDATE durados_App SET [ToDelete]=1,[deleteddate] =getdate() WHERE Id=@Id";
                command.Parameters.Add(new System.Data.SqlClient.SqlParameter("Id", appId));
                try
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                { throw new DuradosException("<br>Failed to update app " + appId.ToString()); }
                command.CommandText = "Delete From  durados_UserApp  WHERE appId=@Id";

                try
                {
                    if (command.Connection.State == ConnectionState.Closed)
                        command.Connection.Open();
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                { throw new DuradosException("<br>Failed to update user app " + appId.ToString()); }

            }
        }


    }
    public class App
    {
        public int AppId { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }
        public string Catalog { get; set; }
        public string TableName { get; set; }
        public string SystemServer { get; set; }
        public string SystemCatalog { get; set; }
        public int AppType { get; set; }

        public App()
        {

        }
        private void Load(DataRow dataRow)
        {
            AppId = Convert.ToInt32(dataRow["Id"]);
            Name = Convert.ToString(dataRow["Name"]);
            Server = Convert.ToString(dataRow["ServerName"]);
            Catalog = Convert.ToString(dataRow["catalog"]);
            TableName = Convert.ToString(dataRow["TableName"]);
            SystemServer = Convert.ToString(dataRow["sysServerName"]);
            SystemCatalog = Convert.ToString(dataRow["sysCatalog"]);
            AppType = Convert.ToInt32(dataRow["AppType"]);
        }
        public App(DataRow dr)
        {
            this.Load(dr);
        }



        //public object Table { get; set; }
    }
    public class AppFactory
    {


        //public App(int appId)
        //{
        //    DataTable dt = GetAppDataRowById(appId);
        //    this.Load(dt.Rows[0]);
        //}

        public App GetAppById(int appId)
        {
            string sql = GetAppSql();
            sql = sql + " AND  a.Id=@appId ";
            return GetApp(sql, "appId", appId.ToString());
        }

        public App GetAppByName(string name)
        {
            string sql = GetAppSql();
            sql = sql + " AND  a.Name=@Name ";
            return GetApp(sql, "Name", name);
        }
        private App GetApp(string sql, string parameterName, string parameterVal)
        {

            App app = new App();
            SqlSchema schema = new SqlSchema();
            IDbCommand command = schema.GetCommand();
            command.Connection = schema.GetConnection(Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.DuradosMapConnectionString);
            ValidateSelectFunctionExists(command);
            command.CommandText = sql;
            command.Parameters.Add(new System.Data.SqlClient.SqlParameter(parameterName, parameterVal));
            try
            {
                command.Connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        app.Server = reader.GetString(reader.GetOrdinal("ServerName"));
                        app.Catalog = reader.GetString(reader.GetOrdinal("catalog"));
                        app.TableName = reader.GetString(reader.GetOrdinal("TableName"));
                        app.SystemServer = reader.GetString(reader.GetOrdinal("sysServerName"));
                        app.SystemCatalog = reader.GetString(reader.GetOrdinal("sysCatalog"));
                        app.AppType = reader.GetInt32(reader.GetOrdinal("AppType"));
                        app.Name = reader.GetString(reader.GetOrdinal("Name"));
                        app.AppId = reader.GetInt32(reader.GetOrdinal("Id"));
                    }

                    reader.Close();
                }
            }
            finally
            {
                command.Connection.Close();
            }
            return app;
        }

        private void ValidateSelectFunctionExists(IDbCommand command)
        {
            string sql = @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[f_report_connection_type]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
                BEGIN
                execute dbo.sp_executesql @statement = N'
                -- =============================================
                -- Author:		<Author,,Name>
                -- Create date: <Create Date, ,>
                -- Description:	<Description, ,>
                -- =============================================
                Create FUNCTION [dbo].[f_report_connection_type] 
                (
	                -- Add the parameters for the function here
	                @id int
                )
                RETURNS int
                AS
                BEGIN
	                -- Declare the return variable here
	                DECLARE @ResultVar int
	
	                --1	console
	                --2	free
	                --3	nw
	                --4	wix
	                --5	system

	                -- Add the T-SQL statements to compute the return value here
	                select @ResultVar = CASE WHEN SUBSTRING([Catalog],1,14) = ''durados_AppSys'' THEN 2 ELSE 
	                CASE WHEN (SUBSTRING([Catalog],1,6) in (select SUBSTRING(DatabaseName,1,6) from  dbo.durados_SampleApp with (NOLOCK))) THEN 4
	                WHEN ServerName = ''137.117.97.62'' or ServerName=''tcp:d9gwdrhh5n.database.windows.net,1433'' THEN 3  
	                WHEN ServerName <> ''137.117.97.62\prod2'' THEN 1
	                ELSE 5 END END
	                from dbo.durados_SqlConnection c with (NOLOCK)
	                where id=@id

	                -- Return the result of the function
	                RETURN @ResultVar

                END

                ' 
                END
                ";
            command.CommandText = sql;

            try
            {
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception) { }
            finally
            {
                if (command.Connection.State != ConnectionState.Closed) command.Connection.Close();
            }
        }

        public static DataTable GetToDeleteAppsTable(IDbCommand command)
        {
            DataTable dt = null;
            SqlSchema sqlSchema = new SqlSchema();
            SqlAccess sqlAccess = new SqlAccess();
            if (command == null)
            {
                command = sqlSchema.GetCommand();
            }

            command.CommandText = "Select * from [" + ProductMaintenance.toDeleteViewName + "]";
            using (IDbConnection connection = sqlSchema.GetConnection(Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.LocalConnection))
            {
                command.Connection = connection;
                dt = sqlAccess.ExecuteTable(((DuradosCommand)command).Command, command.CommandText, null);
            }

            return dt;



        }
        private static string GetAppSql()
        {
            return @"SELECT a.Id, a.Name,  dbo.f_report_connection_type(a.SqlConnectionId) AS AppType, (select s.ViewName from [dbo].[durados_SampleApp] s WITH (NOLOCK) where a.templateId= s.appId )+cast(a.Id as varchar(20)) as TableName ,
                             a.Creator,cnn.ServerName, cnn.catalog ,syscnn.ServerName sysServerName,syscnn.catalog sysCatalog
                            FROM dbo.durados_App AS a WITH (NOLOCK) INNER JOIN dbo.durados_SqlConnection AS cnn WITH (NOLOCK) ON a.SqlConnectionId = cnn.Id INNER JOIN dbo.durados_SqlConnection AS syscnn WITH (NOLOCK) ON a.SystemSqlConnectionId = syscnn.Id
                            WHERE   [ToDelete]<>1";
            //dbo.f_report_is_user_from_wix(a.Creator, NULL) AS inwix,   
        }
    }
    public static class Connections
    {
        private static bool isLoaded = false;
        public static bool IsLoaded { get { return isLoaded; } set { isLoaded = value; } }
        public static Dictionary<string, string> List = new Dictionary<string, string>();
        //private static void LoadConnections()
        //{
        //    foreach (System.Configuration.ConnectionStringSettings connectionString in System.Configuration.ConfigurationManager.ConnectionStrings)
        //    {
        //        System.Data.SqlClient.SqlConnectionStringBuilder cnnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString.ConnectionString);

        //        if (!List.ContainsKey(cnnBuilder.DataSource))
        //            List.Add(cnnBuilder.DataSource, connectionString.ConnectionString);
        //        //else
        //        //    List[cnnParameter.serverName]=new SqlServersManager(connectionString) ;
        //    }
        //    string demoServer = System.Configuration.ConfigurationManager.AppSettings["demoOnPremiseServer"];
        //    string demoUsername = System.Configuration.ConfigurationManager.AppSettings["demoSqlUsername"];
        //    string demoPassword = System.Configuration.ConfigurationManager.AppSettings["demoSqlPassword"];
        //    if (!List.ContainsKey(demoServer))
        //        List.Add(demoServer, GetConnectionString(demoServer, demoUsername, demoPassword));

        //    isLoaded = true;
        //}
        private static void LoadConnections()
        {
                string cnnStrLocal= Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.LocalConnection;
                System.Data.SqlClient.SqlConnectionStringBuilder cnnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(cnnStrLocal);

                if (!List.ContainsKey(cnnBuilder.DataSource))
                    List.Add(cnnBuilder.DataSource, cnnStrLocal);
                
            string cnnStrProd = Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.ProductionConnection;
            cnnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(cnnStrProd);

                if (!List.ContainsKey(cnnBuilder.DataSource))
                    List.Add(cnnBuilder.DataSource, cnnStrProd);

                string cnnStrQA = Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.QAConnection;
                cnnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(cnnStrQA);

                if (!List.ContainsKey(cnnBuilder.DataSource))
                    List.Add(cnnBuilder.DataSource, cnnStrQA);

            isLoaded = true;
        }
        public static string GetConnectionString(string server, string username, string password, bool isMaster = false)
        {
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();

            builder.DataSource = server;
            builder.UserID = username;
            builder.Password = password;
            if (isMaster) builder.InitialCatalog = "master";
            return builder.ConnectionString;
        }
        public static string GetConnectionString(string server, string catalog)
        {

            if (!isLoaded) LoadConnections();
            string connection = List[server];
            if (string.IsNullOrEmpty(connection)) return null;
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connection);
            builder.InitialCatalog = catalog;
            return builder.ConnectionString;
        }

        public static SqlServerManager GetSqlServerManager(string server)
        {
            if (!IsLoaded)
                LoadConnections();
            if (List == null || List.Count == 0 || !List.ContainsKey(server))
                return null;
            return new SqlServerManager(List[server]);
        }
    }
    public class SqlServerManager
    {
        // "" value="137.117.97.62"/>
        //<add key="" value="modubizqa"/>
        //<add key="" value="qazWSX123"/>
        Durados.Web.Mvc.Logging.Logger logger = new Web.Mvc.Logging.Logger();

        public string Server { get; set; }
        public string ServerUserName { get; set; }
        public string ServerPassword { get; set; }
        public SqlServerManager()
        {
            logger.ConnectionString = Durados.Windows.Utilities.AzureUploader.Properties.Settings.Default.LocalConnection;
        }
        public SqlServerManager(string connectionString)
            : this()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = connectionString;
            this.Server = builder.DataSource;
            this.ServerUserName = builder.UserID;
            this.ServerPassword = builder.Password;


        }
        public SqlServerManager(string server, string username, string password)
            : this()
        {
            this.Server = server;
            this.ServerUserName = username;
            this.ServerPassword = password;
        }

        internal void DropDB(string server, string catalog)
        {
            SqlAccess sqlAccess = new SqlAccess();
            try
            {
                sqlAccess.DeleteDatabase(Connections.GetConnectionString(this.Server, this.ServerUserName, this.ServerPassword, true), catalog);
            }
            catch (System.Data.SqlClient.SqlException exception)
            {
                if ((exception.Number == 3702))
                    throw new DuradosException(catalog + " is currently busy");
                else
                    throw new DuradosException("Error occured when deleting app " + catalog + " Error message: " + exception.Message);
            }
            logger.Log("ProductMaintenance", "RemoveApp", "DropDB", null, 3, "Catalog:" + catalog + " was droped");

        }
        internal void DropTable(View view)
        {
            SqlSchema sqlSchema = new SqlSchema();
            IDbCommand command = sqlSchema.GetCommand();
            using (command.Connection = sqlSchema.GetConnection(view.Database.ConnectionString))
            {
                command.Connection.Open();

                if (sqlSchema.IsViewExists(view.Name, command))
                {
                    sqlSchema.DropView(view.Name, command);
                    if (!string.IsNullOrEmpty(view.EditableTableName))
                    {
                        if (sqlSchema.IsTableExists(view.EditableTableName, command))
                        {
                            sqlSchema.DropTable(view.EditableTableName, command);

                        }
                    }
                }
                else
                {
                    if (sqlSchema.IsTableExists(view.Name, command))
                    {
                        sqlSchema.DropTable(view.Name, command);


                    }
                }
                logger.Log("ProductMaintenance", "RemoveApp", "DropTable", null, 3, "In Catalog " + command.Connection.Database + " view:" + view.Name + " and his editable table was droped ");
            }


        }
        internal void DropTable(string tableName, string connectionString)
        {
            SqlSchema sqlSchema = new SqlSchema();
            // SqlAccess sqlAccess= new SqlAccess();
            IDbCommand command = sqlSchema.GetCommand();
            try
            {
                using (command.Connection = sqlSchema.GetConnection(connectionString))
                {
                    command.Connection.Open();
                    if (sqlSchema.IsTableExists(tableName, command))
                    {
                        sqlSchema.DropTable(tableName, command);

                    }
                }

            }
            catch (System.Data.SqlClient.SqlException exception)
            {
                logger.Log("ProductMaintenance", "RemoveApp", "DropTable", null, 3, "In Catalog " + command.Connection.Database + " view:" + tableName + " Fail!!!! was droped ");
            }
            logger.Log("ProductMaintenance", "RemoveApp", "DropTable", null, 3, "In Catalog " + command.Connection.Database + " view:" +tableName + " and his editable table was droped ");
        }




    }
}
