﻿using Durados.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Infrastructure
{
    public class ProductMaintenance
    {
        public static string toDeleteViewName = "v_toDeleteApps";
       

        
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
                    stringBuilder.AppendFormat("<br>Completely remove app number {0}" , dr["Id"].ToString() );
                    Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApps", "RemoveApps", null, 3, string.Format("Completely remove app number {0}.", dr["Id"].ToString()));
                   
                }
                catch(Exception ex)
                {
                    Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApps", "RemoveApps",ex, 2,     string.Format("Could not completely remove app number {0}.",dr["Id"].ToString() ));
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
            App app =  new AppFactory().GetAppByName(appName);
            if (app == null || app.AppId <= 0)
                throw new DuradosException("App was not found");
            return RemoveApp(app);
        }
        public bool RemoveApp(int appId)
        {
            if(appId <= 0) 
                    throw new DuradosException("App could not be 0");

            App app = new AppFactory().GetAppById(appId);
            if (app == null || app.AppId <= 0)
                throw new DuradosException("App was not found");
            return  RemoveApp(app);
        }
        private bool RemoveApp(App app)
        {
            //if (connections == null)
            //{
            //   connections= new Connections();
            //   connections.Init();
            //}
            bool success = false;
            Map map = null;
            string configFileName = string.Empty;
            try
            {
                map = Maps.Instance.GetMap(app.Name);
                configFileName = map.ConfigFileName;
            }
            catch
            {
                configFileName = Maps.DuradosAppPrefix + app.AppId + ".xml";
            }
            try
            {
                SqlServerManager manager = Connections.GetSqlServerManager(app.Server);
                SqlServerManager sysManager = Connections.GetSqlServerManager(app.SystemServer);
                switch (app.AppType)
                {
                    case 0:

                        break;

                    case 2: //free

                    case 3: //northwind
                        if (manager == null)
                            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", null, "RemoveApp", null, 2, string.Format("Could not drop main db from app {0}, the database server was not found, the app type is {1}", app.Name, app.AppType.ToString()));
                        else
                            manager.DropDB(app.Server, app.Catalog);
                        goto case 1;
                    case 1:// console
                        if (sysManager == null)
                            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", null, "RemoveApp", null, 2, string.Format("Could not drop system db from app {0}, the database server was not found the app type is {1}", app.Name, app.AppType.ToString()));
                        else
                        {

                            sysManager.DropDB(app.SystemServer, app.SystemCatalog);

                        }

                        break;
                    case 4://wix
                        if (map != null)
                        {
                            foreach (View view in map.Database.Views.Values)
                                if (!view.SystemView)
                                    sysManager.DropTable(view);
                        }
                        break;
                    case 5:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", null, "RemoveApp", exception, 1, app.Name);
            }
            DeleteConfiguration(configFileName);
            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApp", "RemoveApp", null, 3, "configuration files " + configFileName + " was remove from storage");
            UpdateDeletedApp(app.AppId);
            success = true;
            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApp", "RemoveApp", null, 3, "App name: " + app.Name + " Number: " + app.AppId + " was completly removed");
            return success;

            // Maps.Instance.RemoveMap(app.Name);
        }
        private void DeleteConfiguration(string configFileName)
        {
            
            Maps.Instance.RemoveConfigFromCloud(configFileName);
            Maps.Instance.RemoveConfigFromCloud(configFileName + ".xml");
        }
        private void UpdateDeletedApp(int appId)
        {
            SqlSchema schema = new SqlSchema();
            using (IDbCommand command = schema.GetCommand())
            {
                command.Connection = schema.GetConnection(Maps.Instance.DuradosMap.connectionString);
                command.CommandText = "UPDATE durados_App SET [ToDelete]=1,[deleteddate] =getdate() WHERE Id=@Id";
                command.Parameters.Add(new System.Data.SqlClient.SqlParameter("Id", appId));
                try
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                { throw new DuradosException("<br>Failed to update app " +appId.ToString()); }
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
            SystemServer = Convert.ToString(dataRow["sysServerName"]);
            SystemCatalog = Convert.ToString(dataRow["sysCatalog"]);
            AppType = Convert.ToInt32(dataRow["AppType"]);
        }
        public App(DataRow dr)
        {
            this.Load(dr);
        }



        public object Table { get; set; }
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
            return GetApp(sql , "appId", appId.ToString());
        }

        public App GetAppByName(string name)
        {
            string sql = GetAppSql();
            sql = sql + " AND  a.Name=@Name ";
            return GetApp(sql, "Name", name);
        }
        private App GetApp(string sql,string parameterName,string parameterVal)
        {
           
            App app = new App();
            SqlSchema schema = new SqlSchema();
            IDbCommand command = schema.GetCommand();
            command.Connection = schema.GetConnection(Maps.Instance.DuradosMap.connectionString);
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

            command.CommandText = "Select * from ["+ProductMaintenance.toDeleteViewName +"]";
            using (IDbConnection connection = sqlSchema.GetConnection(Maps.Instance.DuradosMap.connectionString))
            {
                command.Connection = connection;
                dt = sqlAccess.ExecuteTable(((DuradosCommand)command).Command, command.CommandText, null);
            }

            return dt;


           
        }
        private static string GetAppSql()
        {
            return @"SELECT a.Id, a.Name,  dbo.f_report_connection_type(a.SqlConnectionId) AS AppType, 
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
        private static void LoadConnections()
        {
            foreach (System.Configuration.ConnectionStringSettings connectionString in System.Configuration.ConfigurationManager.ConnectionStrings)
            {
                System.Data.SqlClient.SqlConnectionStringBuilder cnnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString.ConnectionString);

                if (!List.ContainsKey(cnnBuilder.DataSource))
                    List.Add(cnnBuilder.DataSource, connectionString.ConnectionString);
                //else
                //    List[cnnParameter.serverName]=new SqlServersManager(connectionString) ;
            }
            string demoServer = System.Configuration.ConfigurationManager.AppSettings["demoOnPremiseServer"];
            string demoUsername = System.Configuration.ConfigurationManager.AppSettings["demoSqlUsername"];
            string demoPassword = System.Configuration.ConfigurationManager.AppSettings["demoSqlPassword"];
            if (!List.ContainsKey(demoServer))
                List.Add(demoServer, GetConnectionString(demoServer, demoUsername, demoPassword));

            isLoaded = true;
        }
        public static string GetConnectionString(string server, string username, string password,bool isMaster=false)
        {
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();

            builder.DataSource = server;
            builder.UserID = username;
            builder.Password = password;
            if (isMaster) builder.InitialCatalog = "master";
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


        public string Server { get; set; }
        public string ServerUserName { get; set; }
        public string ServerPassword { get; set; }
        public SqlServerManager(string connectionString)
        {
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = connectionString;
            this.Server = builder.DataSource;
            this.ServerUserName = builder.UserID;
            this.ServerPassword = builder.Password;


        }
        public SqlServerManager(string server, string username, string password)
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
                sqlAccess.DeleteDatabase(Connections.GetConnectionString(this.Server, this.ServerUserName, this.ServerPassword,true), catalog);
            }
            catch(System.Data.SqlClient.SqlException exception)
            {
                if ((exception.Number == 3702))
                    throw new DuradosException(catalog + " is currently busy");
                else
                    throw new DuradosException("Error occured when deleting app " + catalog + " Error message: " + exception.Message);
            }
            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApp", "DropDB", null, 3, "Catalog:" + catalog + " was droped");

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
                Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApp", "DropTable", null, 3, "In Catalog " + command.Connection.Database + " view:" + view.Name + " and his editable table was droped ");
            }
                

        }
      
    }
}