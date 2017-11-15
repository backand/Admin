using Durados.DataAccess;
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
                    stringBuilder.AppendFormat("<br>Completely remove app number {0}", dr["Id"].ToString());
                    Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApps", "RemoveApps", null, 3, string.Format("Completely remove app number {0}.", dr["Id"].ToString()));

                }
                catch (Exception ex)
                {
                    Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApps", "RemoveApps", ex, 2, string.Format("Could not completely remove app number {0}.", dr["Id"].ToString()));
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
                SqlServerManager manager = Connections.GetServerManager(app.Server);
                SqlServerManager sysManager = Connections.GetServerManager(app.SystemServer);

                switch (app.AppType)
                {
                    case 0:

                        break;

                    case 2: //free

                    case 3: //northwind
                        if (manager == null)
                            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", null, "RemoveApp", null, 1, string.Format("Could not drop main db from app {0}, the database server was not found, the app type is {1}", app.Name, app.AppType.ToString()));
                        else
                            manager.DropDB(app.Server, app.Catalog);
                        goto case 1;
                    case 1:// console
                        if (sysManager == null)
                            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", null, "RemoveApp", null, 1, string.Format("Could not drop system db from app {0}, the database server was not found the app type is {1}", app.Name, app.AppType.ToString()));
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
            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApp", "RemoveApp", null, 1, "configuration files " + configFileName + " was remove from storage");
            UpdateDeletedApp(app.AppId);
            success = true;
            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApp", "RemoveApp", null, 1, "App name: " + app.Name + " Number: " + app.AppId + " was marked deleted");
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
            using (IDbConnection connection = Maps.MainAppSchema.GetNewConnection(Maps.Instance.ConnectionString))
            {

                using (IDbCommand command = connection.CreateCommand())
                {
                    //command.Connection = schema.GetConnection(Maps.Instance.DuradosMap.connectionString);
                    command.CommandText = Maps.MainAppSchema.GetUpdateAppToBeDeleted();
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "Id";
                    parameter.Value = appId;
                    command.Parameters.Add(parameter);
                    
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


        public App GetAppById(string appId)
        {
            int id;
            if(int.TryParse(appId,out id))
            {
                return GetAppById(id);
            }
            return null;
        }


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
            ISqlMainSchema schema = Maps.MainAppSchema;;
            IDbCommand command = schema.GetNewCommand();
            command.Connection = schema.GetNewConnection(Maps.Instance.DuradosMap.connectionString);
            //ValidateSelectFunctionExists(command);
            command.CommandText = sql;
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterVal;
            command.Parameters.Add(parameter);
            
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
            string sql = Maps.MainAppSchema.GetValidateSelectFunctionExistsSql();
            if(string.IsNullOrEmpty(sql)) return;
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
            using (IDbConnection connection = sqlSchema.GetConnection(Maps.Instance.DuradosMap.connectionString))
            {
                command.Connection = connection;
                dt = sqlAccess.ExecuteTable(((DuradosCommand)command).Command, command.CommandText, null);
            }

            return dt;



        }
        private static string GetAppSql()
        {
            return Maps.MainAppSchema.GetAppSql();
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
            System.Configuration.ConnectionStringSettings systemConnection = System.Configuration.ConfigurationManager.ConnectionStrings["SystemMapsConnectionString"] ?? null;
                System.Data.Common.DbConnectionStringBuilder csb;
                if (MySqlAccess.IsMySqlConnectionString(systemConnection.ConnectionString))
                    csb = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(systemConnection.ConnectionString);
                else
                    csb = new System.Data.SqlClient.SqlConnectionStringBuilder(systemConnection.ConnectionString);

                if (!List.ContainsKey(csb["data source"].ToString()))
                    List.Add(csb["data source"].ToString(), systemConnection.ConnectionString);
                //else
                //    List[cnnParameter.serverName]=new SqlServersManager(connectionString) ;
            
            AddOnPremissDemoDb();
            AddExternalInstances();


            isLoaded = true;
        }

        private static void AddOnPremissDemoDb()
        {
            string demoServer = System.Configuration.ConfigurationManager.AppSettings["demoOnPremiseServer"];
            string demoUsername = System.Configuration.ConfigurationManager.AppSettings["demoSqlUsername"];
            string demoPassword = System.Configuration.ConfigurationManager.AppSettings["demoSqlPassword"];
            if (!List.ContainsKey(demoServer))
                List.Add(demoServer, GetConnectionString(demoServer, demoUsername, demoPassword));
        }

        private static void AddExternalInstances()
        {
            Durados.Web.Mvc.UI.Helpers.AppFactory externalAppFactory = new UI.Helpers.AppFactory();

            Dictionary<string, string> conns = externalAppFactory.GetExternalInstanceConnection();
            foreach (string endpoint in conns.Keys)
                if (!List.ContainsKey(endpoint))
                    List.Add(endpoint, conns[endpoint]);
        }

        public static string GetConnectionString(string server, string username, string password, bool isMaster = false, Durados.SqlProduct product = Durados.SqlProduct.SqlServer)
        {
            System.Data.Common.DbConnectionStringBuilder builder = GetConnectionStringBuilder(product);

            builder["data source"] = server;
            builder["User ID"] = username;
            builder["Password"] = password;
            if (isMaster) builder["initial catalog"] = "master";
            return builder.ConnectionString;
        }

        private static System.Data.Common.DbConnectionStringBuilder GetConnectionStringBuilder(Durados.SqlProduct product)
        {
            if (product == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            return new System.Data.SqlClient.SqlConnectionStringBuilder();

        }
        public static SqlServerManager GetServerManager(string server)
        {
            if (!IsLoaded)
                LoadConnections();
            if (List == null || List.Count == 0 || !List.ContainsKey(server))
                return null;

            if (MySqlAccess.IsMySqlConnectionString(List[server]))
                return new MysqlManager(List[server]);

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
            System.Data.Common.DbConnectionStringBuilder builder = GetConnectionStringBuilder();
            builder.ConnectionString = connectionString;
            this.Server = builder["DataSource"].ToString();
            this.ServerUserName = builder["UserID"].ToString();
            this.ServerPassword = builder["Password"].ToString();

        }

        protected virtual System.Data.Common.DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new System.Data.SqlClient.SqlConnectionStringBuilder();
        }
        public SqlServerManager(string server, string username, string password)
        {
            this.Server = server;
            this.ServerUserName = username;
            this.ServerPassword = password;
        }

        public virtual void DropDB(string server, string catalog)
        {
            SqlAccess sqlAccess = new SqlAccess();
            try
            {
                sqlAccess.DeleteDatabase(Connections.GetConnectionString(this.Server, this.ServerUserName, this.ServerPassword, true, GetSqlProduct()), catalog);
            }
            catch (System.Data.SqlClient.SqlException exception)
            {
                if ((exception.Number == 3702))
                    throw new DuradosException(catalog + " is currently busy");
                else
                    throw new DuradosException("Error occured when deleting app " + catalog + " Error message: " + exception.Message);
            }
            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApp", "DropDB", null, 3, "Catalog:" + catalog + " was droped");

        }

        protected virtual SqlProduct GetSqlProduct()
        {
            return SqlProduct.SqlServer;
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


    public class MysqlManager : SqlServerManager
    {
        public MysqlManager(string connectionString)
            : base(connectionString)
        {

        }
        protected override System.Data.Common.DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();

        }
        protected override SqlProduct GetSqlProduct()
        {
            return SqlProduct.MySql;
        }

        public override void DropDB(string server, string catalog)
        {

            MySqlAccess sqlAccess = new MySqlAccess();
            try
            {
                sqlAccess.DeleteDatabase(Connections.GetConnectionString(this.Server, this.ServerUserName, this.ServerPassword, false, GetSqlProduct()), catalog);
            }
            catch (MySql.Data.MySqlClient.MySqlException exception)
            {
                throw new DuradosException("Error occured when deleting app " + catalog + " Error message: " + exception.Message);
            }
            Maps.Instance.DuradosMap.Logger.Log("ProductMaintenance", "RemoveApp", "DropDB", null, 3, "Catalog:" + catalog + " was droped");

        }
    }
}
