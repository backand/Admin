using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Controllers
{
    public class MultiTenancyController : CrmController
    {
        string nameFieldName = "Name";
        string imageFieldName = "Image";
        string dataSourceTypeFieldName = "FK_durados_App_durados_DataSourceType_Parent";

        protected override UI.TableViewer GetNewTableViewer()
        {
            return new UI.MultiTenancyTableViewer();
        }
        protected string GetCleanName(string name)
        {
            name = name.Trim();

            if (name.ToLower().Equals("www"))
                throw new DuradosException(Map.Database.Localizer.Translate("This name already exists."));

            Regex regex = new Regex("^[A-Za-z0-9\\-]+$");/**\\- support - inside url*/
            if (!regex.IsMatch(name))
            {
                throw new DuradosException(Map.Database.Localizer.Translate("The 'Name' is the first part of the URL of the Console.\n\rOnly alphanumeric characters are allowed."));
            }

            if (name.Length > Maps.AppNameMax)
                throw new DuradosException(Map.Database.Localizer.Translate("The 'Name' is the first part of the URL of the Console.\n\rMaximum 25 characters are allowed."));


            return name;
        }

        protected override string GetDeleteConfirmationMessage()
        {
            return Map.Database.Localizer.Translate("Deleting the Console is final.<br>Are you sure that you want to delete?");
        }

        protected override void AfterDeleteBeforeCommit(DeleteEventArgs e)
        {
            base.AfterDeleteBeforeCommit(e);
            string name = e.PrevRow["Name"].ToString();

            Maps.Instance.Delete(name);

            try
            {
                int id = Convert.ToInt32(e.PrimaryKey);

                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(Maps.Instance.ConnectionString);
                string mapServer = scsb.DataSource;
                MapDataSet.durados_SqlConnectionRow systemConnectionRow = ((MapDataSet.durados_AppRow)e.PrevRow).durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System;
                if (systemConnectionRow != null)
                {
                    string systemServer = systemConnectionRow.IsServerNameNull() ? null : systemConnectionRow.ServerName;
                    string systemDatabase = systemConnectionRow.IsCatalogNull() ? null : systemConnectionRow.Catalog;

                    if (string.IsNullOrEmpty(systemServer) || systemServer.Equals(mapServer))
                    {
                        DropDatabase(systemDatabase);
                    }
                }

                MapDataSet.durados_SqlConnectionRow appConnectionRow = ((MapDataSet.durados_AppRow)e.PrevRow).durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection;
                if (appConnectionRow != null)
                {
                    string appServer = appConnectionRow.IsServerNameNull() ? null : appConnectionRow.ServerName;
                    string appDatabase = appConnectionRow.IsCatalogNull() ? null : appConnectionRow.Catalog;

                    if (string.IsNullOrEmpty(appServer) || appServer.Equals(mapServer))
                    {
                        if (Maps.DropAppDatabase)
                        {
                            if (!HasOtherConnectios(appDatabase))
                            {
                                DropDatabase(appDatabase);
                            }
                        }
                    }
                }

                DeleteConfig(id);
                DeleteUploads(id);
            }

            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 2, "delete databases and config");
            }

        }

        private bool HasOtherConnectios(string appDatabase)
        {
            using (SqlConnection connection = new SqlConnection(Maps.Instance.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("select count(*) from dbo.durados_SqlConnection where [Catalog] = N'" + appDatabase + "'", connection))
                {
                    connection.Open();
                    object scalar = command.ExecuteScalar();

                    if (scalar == null || scalar == DBNull.Value || scalar.Equals(0) || scalar.Equals(1))
                        return false;
                    return true;
                }
            }
        }



        private void DeleteUploads(int id)
        {
            string path = Server.MapPath("~/Uploads/" + id.ToString());
            if (System.IO.Directory.Exists(path))
                System.IO.Directory.Delete(path, true);
        }

        private void DropDatabase(string name)
        {
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(Maps.Instance.ConnectionString);
            //scsb.InitialCatalog = null;

            using (SqlConnection connection = new SqlConnection(scsb.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("drop database " + name, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteConfig(int id)
        {
            //string configFileName = Server.MapPath(string.Format(Maps.ConfigPath + "durados_AppSys_{0}.xml", id));
            string configFileName = Maps.GetConfigPath(string.Format("durados_AppSys_{0}.xml", id));
            if (System.IO.File.Exists(configFileName))
                System.IO.File.Delete(configFileName);
        }


        protected override void BeforeEdit(EditEventArgs e)
        {
            if (e.Values.ContainsKey("Name"))
            {
                MapDataSet.durados_AppRow appRow = (MapDataSet.durados_AppRow)e.PrevRow;
                string oldName = appRow.Name;
                string newName = e.Values["Name"].ToString();
                if (!oldName.Equals(newName))
                {
                    //string port = Request.Url.Port.ToString();
                    //string host = Maps.Host;

                    //if (Request.Url.ToString().Contains(port))
                    //    host += ":" + port;

                    string urlFieldName = "Url";
                    e.Values[urlFieldName] = GetUrl(newName); //string.Format("{0}.{1}|_blank|" + System.Web.HttpContext.Current.Request.Url.Scheme + "://{0}.{1}?appName={0}", newName, host);
                }
            }

            base.BeforeEdit(e);
        }

        //protected override void DropDownFilter(ParentField parentField, ref string sql)
        //{
        //    base.DropDownFilter(parentField, ref sql);
        //    sql = HandleSqlConnectionFilter(parentField, sql);
        //}

      

        protected override void BeforeCreate(CreateEventArgs e)
        {
            if (Map.Database.GetCurrentUsername().ToLower().StartsWith("wix"))
            {
                throw new PlugInUserException("You cannot make this operation as a Wix user. Please <a href='/Account/LogOff'>logout</a> and register.");
            }
            string urlFieldName = "Url";

            string name = e.Values[nameFieldName].ToString();

            string dataSourceTypeId = e.Values[dataSourceTypeFieldName].ToString();

            if (e.View.Fields.ContainsKey(imageFieldName) && e.View.Fields[imageFieldName].DefaultValue != null)
            {
                if (!e.Values.ContainsKey(imageFieldName))
                    e.Values.Add(imageFieldName, e.View.Fields[imageFieldName].DefaultValue);
            }
            string cleanName = GetCleanName(name);
            if (dataSourceTypeId == "2" || dataSourceTypeId == "4")
            {
                Field sqlConnectionField = e.View.GetFieldByColumnNames("SqlConnectionId");
                object sqlConnectionId = e.Values[sqlConnectionField.Name];
                if (sqlConnectionId == null || sqlConnectionId.Equals(string.Empty))
                    throw new DuradosException(Map.Database.Localizer.Translate("Please create or select a ") + sqlConnectionField.DisplayName);
            }
            
            if (dataSourceTypeId == "4") // template
            {
                //sqlAccess.ExecuteNoneQueryStoredProcedure(Maps.Instance.ConnectionString, "durados_CreateNewDatabase", values);
                Field templateField = e.View.GetFieldByColumnNames("TemplateId");
                object templateId = e.Values[templateField.Name];
                if (templateId == null || templateId.Equals(string.Empty))
                    throw new DuradosException(Map.Database.Localizer.Translate("Please select an ") + templateField.DisplayName);
            }
            

            if (dataSourceTypeId == "1" || dataSourceTypeId == "4") // blank or template
            {

            }
            else if (dataSourceTypeId == "2" && !Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer"))
            {
                string secFieldName = "FK_durados_App_durados_SqlConnection_Security_Parent";
                if (!e.Values.ContainsKey(secFieldName))
                {
                    e.Values.Add(secFieldName, string.Empty);
                }
                e.Values[secFieldName] = string.Empty;
                string sysFieldName = "FK_durados_App_durados_SqlConnection_System_Parent";
                if (!e.Values.ContainsKey(secFieldName))
                {
                    e.Values.Add(sysFieldName, string.Empty);
                }
                e.Values[sysFieldName] = string.Empty;
            }

            if (!e.Values.ContainsKey(urlFieldName))
            {
                e.Values.Add(urlFieldName, string.Empty);
            }

            //string port = Request.Url.Port.ToString();
            //string host = Maps.Host;

            //if (Request.Url.ToString().Contains(port))
            //    host += ":" + port;

            e.Values[urlFieldName] = GetUrl(cleanName); //string.Format("{0}.{1}|_blank|" + System.Web.HttpContext.Current.Request.Url.Scheme + "://{0}.{1}?appName={0}", cleanName, host);

            //string uploadPath = ((ColumnField)e.View.Fields["Image"]).GetUploadPath();
            //string image = e.Values["Image"].ToString();
            //string path = uploadPath + image;
            //string newPath = uploadPath + cleanName + "\\" + image;
            //System.IO.FileInfo fileInfo = new System.IO.FileInfo(newPath);
            //fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.
            
            
            //try
            //{
            //    System.IO.File.Copy(path, newPath);
            //}
            //catch { }

            if (Session["UserApps"] != null)
                Session["UserApps"] = null;
            base.BeforeCreate(e);
        }

        protected override void BeforeDelete(DeleteEventArgs e)
        {
            if (Session["UserApps"] != null)
                Session["UserApps"] = null;
            base.BeforeDelete(e);
        }
        private string GetUrl(string name)
        {
            string port = Request.Url.Port.ToString();
            string host = Maps.Host;

            if (Request.Url.ToString().Contains(port))
                host += ":" + port;

            return string.Format("{0}.{1}|_blank|" + System.Web.HttpContext.Current.Request.Url.Scheme + "://{0}.{1}?appName={0}", name, host);

        }

        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);

            string dataSourceTypeId = e.Values[dataSourceTypeFieldName].ToString();
            string name = e.Values[nameFieldName].ToString();
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = Map.connectionString;
            string pk = e.PrimaryKey;

            string cleanName = GetCleanName(name);

            SqlProduct sqlProduct = SqlProduct.SqlServer;

            if (dataSourceTypeId == "1")// || dataSourceTypeId == "4") // blank or template
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                string appCatalog = Maps.DuradosAppPrefix + pk;
                string sysCatalog = Maps.DuradosAppSysPrefix + pk;

                values.Add("newdb", appCatalog);
                values.Add("newSysDb", sysCatalog);

                try
                {
                    sqlAccess.ExecuteNoneQueryStoredProcedure(Maps.Instance.ConnectionString, "durados_CreateNewDatabase", values);
                }
                catch (SqlException exception)
                {
                    throw new DuradosException(exception.Message, exception);
                }


                //values = new Dictionary<string, object>();
                //values.Add("AppId", Convert.ToInt32(pk));
                //values.Add("Catalog", appCatalog);
                //values.Add("SysCatalog", sysCatalog);
                ////values.Add("ServerName", builder.DataSource);
                ////values.Add("Username", builder.UserID);
                ////values.Add("Password", builder.Password);
                ////values.Add("IntegratedSecurity", builder.IntegratedSecurity);
                //values.Add("DuradosUser", Map.Database.GetUserID());
                //sqlAccess.ExecuteNoneQueryStoredProcedure(Maps.Instance.ConnectionString, "durados_SetConnection", values);

                string duradosUser = Map.Database.GetUserID();
                string newPassword = new AccountMembershipService().GetRandomPassword(12);
                string newUsername = appCatalog + "User";
                try
                {
                    CreateDatabaseUser(builder.DataSource, appCatalog, builder.UserID, builder.Password, builder.IntegratedSecurity, newUsername, newPassword, false);
                    sqlAccess.CreateDatabaseUserWithoutLogin(Map.connectionString, sysCatalog, newUsername, newPassword, "db_owner");
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Failed to create database user. username=" + newUsername);
                    throw new DuradosException("Failed to create database user");
                }
                int? appConnId = SaveConnection(builder.DataSource, appCatalog, newUsername, newPassword, duradosUser, SqlProduct.SqlServer);

                int? sysConnId = SaveConnection(builder.DataSource, sysCatalog, newUsername, newPassword, duradosUser, SqlProduct.SqlServer);

                //values = new Dictionary<string, object>();
                //values.Add("FK_durados_App_durados_SqlConnection_Parent", appConnId);
                //values.Add("FK_durados_App_durados_SqlConnection_System_Parent", sysConnId);

                //e.View.Edit(values, e.PrimaryKey, null, null, null, null);

                string sql = "update durados_App set SqlConnectionId = " + appConnId + ", SystemSqlConnectionId = " + sysConnId + " where id = " + e.PrimaryKey;

                using (SqlConnection connection = new SqlConnection(Maps.Instance.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            if (dataSourceTypeId == "2") // existing
            {

                string sysConnection = e.Values["FK_durados_App_durados_SqlConnection_System_Parent"].ToString();
                if (string.IsNullOrEmpty(sysConnection))
                {
                    string sysCatalog = Maps.DuradosAppSysPrefix + pk;

                    string duradosUser = Map.Database.GetUserID();
                    string newPassword = new AccountMembershipService().GetRandomPassword(12);
                    string newUsername = Durados.Database.ProductShorthandName + pk ;
                    string serverName   = GetServerName(Maps.Instance.SystemConnectionString);
                    //Dictionary<string, object> values = new Dictionary<string, object>();
                    //values.Add("AppId", Convert.ToInt32(pk));
                    //values.Add("Catalog", Maps.DuradosAppPrefix + pk);
                    //values.Add("SysCatalog", Maps.DuradosAppSysPrefix + pk);
                    //values.Add("DuradosUser", Map.Database.GetUserID());
                    //sqlAccess.ExecuteNoneQueryStoredProcedure(Maps.Instance.ConnectionString, "durados_SetSysConnection", values);
                    SqlProduct systemSqlProduct = GetSystemSqlProduct() ;
                    int? sysConnId=null;
                    if (systemSqlProduct == SqlProduct.SqlServer)
                    {
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(Maps.Instance.ConnectionString))
                            {
                                connection.Open();
                                using (SqlCommand command = new SqlCommand())
                                {
                                    command.Connection = connection;
                                    sqlAccess.ExecuteNonQuery(e.View, command, "create database " + sysCatalog, null);
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Failed to create system database. catalog=" + sysCatalog);
                            throw new DuradosException("Failed to create database");
                        }

                        try
                        {
                            CreateDatabaseUser(builder.DataSource, sysCatalog, builder.UserID, builder.Password, builder.IntegratedSecurity, newUsername, newPassword, false);
                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Failed to create database user. username=" + newUsername);
                            throw new DuradosException("Failed to create database user");
                        }
                         sysConnId = SaveConnection(builder.DataSource, sysCatalog, newUsername, newPassword, duradosUser, SqlProduct.SqlServer);

                    }
                    else if (systemSqlProduct == SqlProduct.MySql)
                    {
                        try
                        {
                            NewDatabaseParameters sysDbParameters = new NewDatabaseParameters { DbName = sysCatalog, Username = newUsername, Password = newPassword };
                            AppFactory appFactory = new AppFactory();
                            appFactory.CreateNewSystemSchemaAndUser(Maps.Instance.SystemConnectionString, sysDbParameters);
                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Failed to create database user. username=" + newUsername);
                            throw new DuradosException("Failed to create database user");
                        }

                        int port = Convert.ToInt32(new MySqlSchema().GetPort(Maps.Instance.SystemConnectionString));
                         sysConnId = SaveConnection(serverName, sysCatalog, newUsername, newPassword, duradosUser, SqlProduct.MySql, port);
                    }
                    //Dictionary<string, object> values = new Dictionary<string, object>();

                    //values = new Dictionary<string, object>();
                    //values.Add("FK_durados_App_durados_SqlConnection_System_Parent", sysConnId);

                    //e.View.Edit(values, e.PrimaryKey, null, null, null, null);
                    string sql = "update durados_App set SystemSqlConnectionId = " + sysConnId + " where id = " + e.PrimaryKey;

                    using (SqlConnection connection = new SqlConnection(Maps.Instance.ConnectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                }

                
            }

            if (dataSourceTypeId == "4") // template
            {
                //IPersistency persistency = Maps.Instance.GetNewPersistency();
                //MapDataSet.durados_AppRow appRow = (MapDataSet.durados_AppRow)((View)e.View).GetDataRow(e.View.Fields[nameFieldName], name);

                //if (!appRow.IsTemplateFileNull() && !string.IsNullOrEmpty(appRow.TemplateFile))
                //{
                //    TemplateGenerator TemplateGenerator = new TemplateGenerator(persistency.GetConnection(appRow, builder).ToString(), appRow.TemplateFile);
                //}



                Dictionary<string, object> values2 = new Dictionary<string, object>();
                string appCatalog = Maps.DuradosAppPrefix + pk;
                string sysCatalog = Maps.DuradosAppSysPrefix + pk;

                string sysconnstrFieldName = e.View.GetFieldByColumnNames("SystemSqlConnectionId").Name;
                bool sysExists = e.Values.ContainsKey(sysconnstrFieldName) && e.Values[sysconnstrFieldName] != null && e.Values[sysconnstrFieldName].ToString() != string.Empty;


                string appconnstrFieldName = e.View.GetFieldByColumnNames("SqlConnectionId").Name;
                bool appExists = e.Values.ContainsKey(appconnstrFieldName) && e.Values[appconnstrFieldName] != null && e.Values[appconnstrFieldName].ToString() != string.Empty;

                try
                {
                    if (!sysExists)
                    {
                        using (SqlConnection connection = new SqlConnection(Maps.Instance.ConnectionString))
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand("exec('create database " + sysCatalog + "')", connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (SqlException exception)
                {
                    throw new DuradosException("Could not create system database: " + sysCatalog + "; Additional info: " + exception.Message, exception);
                }

                try
                {
                    if (!appExists)
                    {
                        using (SqlConnection connection = new SqlConnection(Maps.Instance.ConnectionString))
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand("exec('create database " + appCatalog + "')", connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (SqlException exception)
                {
                    throw new DuradosException("Could not create app database: " + appCatalog + "; Additional info: " + exception.Message, exception);
                }


                string duradosUser = Map.Database.GetUserID();
                string newPassword = new AccountMembershipService().GetRandomPassword(12);
                string newUsername = appCatalog + "User";
                try
                {
                    if (!appExists)
                    {
                        CreateDatabaseUser(builder.DataSource, appCatalog, builder.UserID, builder.Password, builder.IntegratedSecurity, newUsername, newPassword, false);
                        if (!sysExists)
                        {
                            sqlAccess.CreateDatabaseUserWithoutLogin(Map.connectionString, sysCatalog, newUsername, newPassword, "db_owner");
                        }
                    }
                    else if (!sysExists)
                    {
                        try
                        {
                            sqlAccess.CreateDatabaseUser(Map.connectionString, sysCatalog, newUsername, newPassword, "db_owner");
                        }
                        catch (SqlException)
                        {
                            sqlAccess.CreateDatabaseUserWithoutLogin(Map.connectionString, sysCatalog, newUsername, newPassword, "db_owner");
                        }
                    }
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Failed to create database user. username=" + newUsername);
                    throw new DuradosException("Failed to create database user");
                }

                int? appConnId = null;

                if (!appExists)
                    appConnId = SaveConnection(builder.DataSource, appCatalog, newUsername, newPassword, duradosUser, SqlProduct.SqlServer);
                else
                    appConnId = Convert.ToInt32(e.Values[appconnstrFieldName]);

                int? sysConnId = null;
                if (!sysExists)
                    sysConnId = SaveConnection(builder.DataSource, sysCatalog, newUsername, newPassword, duradosUser, SqlProduct.SqlServer);
                else
                    sysConnId = Convert.ToInt32(e.Values[sysconnstrFieldName]);

                //values = new Dictionary<string, object>();
                //values.Add("FK_durados_App_durados_SqlConnection_Parent", appConnId);
                //values.Add("FK_durados_App_durados_SqlConnection_System_Parent", sysConnId);

                //e.View.Edit(values, e.PrimaryKey, null, null, null, null);

                string sql2 = "update durados_App set SqlConnectionId = " + appConnId + ", SystemSqlConnectionId = " + sysConnId + " where id = " + e.PrimaryKey;

                using (SqlConnection connection = new SqlConnection(Maps.Instance.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }




                try
                {
                    string templateId = e.Values[e.View.GetFieldByColumnNames("TemplateId").Name].ToString();
                    DataRow templateAppRow = e.View.GetDataRow(templateId);

                    if (templateAppRow != null)
                    {
                        string image = string.Format("{0}/{1}", e.PrimaryKey, Regex.Replace(templateAppRow["Image"].ToString(), @"^[\d]+/", string.Empty));

                        string connectionId = e.Values[e.View.GetFieldByColumnNames("SqlConnectionId").Name].ToString();
                        if (string.IsNullOrEmpty(connectionId))
                        {
                            connectionId = templateAppRow["SqlConnectionId"].ToString();
                        }
                        View sqlConnectionView = (View)e.View.Database.Views["durados_SqlConnection"];
                        DataRow sqlConnectionRow = sqlConnectionView.GetDataRow(connectionId);
                        if (sqlConnectionRow != null)
                        {

                            Dictionary<string, object> values = new Dictionary<string, object>();
                            values.Add("ServerName", sqlConnectionRow["ServerName"].ToString());
                            values.Add("Catalog", sqlConnectionRow["Catalog"].ToString());
                            values.Add("Username", sqlConnectionRow["Username"].ToString());
                            values.Add("Password", sqlConnectionRow["Password"].ToString());
                            values.Add("IntegratedSecurity", sqlConnectionRow["IntegratedSecurity"]);
                            values.Add(sqlConnectionView.GetFieldByColumnNames("DuradosUser").Name, Map.Database.GetUserID());
                            values.Add(sqlConnectionView.GetFieldByColumnNames("SqlProductId").Name, sqlConnectionRow["SqlProductId"]);
                            values.Add("ProductPort", sqlConnectionRow.IsNull("ProductPort") ? string.Empty : sqlConnectionRow["ProductPort"].ToString());
                            values.Add("SshRemoteHost", sqlConnectionRow.IsNull("SshRemoteHost") ? string.Empty : sqlConnectionRow["SshRemoteHost"].ToString());
                            values.Add("SshPort", sqlConnectionRow.IsNull("SshPort") ? string.Empty : sqlConnectionRow["SshPort"].ToString());
                            values.Add("SshUser", sqlConnectionRow.IsNull("SshUser") ? string.Empty : sqlConnectionRow["SshUser"].ToString());
                            values.Add("SshUses", sqlConnectionRow.IsNull("SshUses") ? string.Empty : sqlConnectionRow["SshUses"].ToString());
                            values.Add("SshPassword", sqlConnectionRow.IsNull("SshPassword") ? string.Empty : sqlConnectionRow["SshPassword"].ToString());
                            

                            string newConnectionId = sqlConnectionView.Create(values);
                            //values = new Dictionary<string, object>();
                            //values.Add(e.View.GetFieldByColumnNames("SqlConnectionId").Name, newConnectionId);
                            //e.View.Edit(values, e.PrimaryKey, null, null, null, null);
                            string sql = "update durados_App set SqlConnectionId = " + newConnectionId + ",Image= '" + image + "', DataSourceTypeId=2 where Id = " + e.PrimaryKey;
                            sqlAccess.ExecuteNonQuery(e.View.ConnectionString, sql);
                        }
                        sqlProduct = sqlConnectionRow.IsNull("SqlProductId") ? SqlProduct.SqlServer : (SqlProduct)sqlConnectionRow["SqlProductId"];
                        //string templateConfigFileName = Server.MapPath(string.Format(Maps.ConfigPath + "durados_AppSys_{0}.xml", templateId));
                        string templateConfigFileName = Maps.GetConfigPath(string.Format("durados_AppSys_{0}.xml", templateId), sqlProduct);
                        string newConfigFileName = Maps.GetConfigPath(string.Format("durados_AppSys_{0}.xml", e.PrimaryKey), sqlProduct);
                        //string newSchemaFileName = newConfigFileName + ".xml";
                        //string templateSchemaFileName = templateConfigFileName + ".xml";

                        string sourcePath = GetTempalteUploadFolder(templateConfigFileName);
                        string targetPath = "/Uploads/" + e.PrimaryKey + "/";
                        try
                        {
                            if (!Maps.Cloud)
                                DirectoryCopy(sourcePath, targetPath, true);
                            else
                            {
                                CopyContainer(sourcePath, targetPath, true);
                                
                            }
                        }
                        catch { }
                        SetAndWirteNewConfigFile(templateConfigFileName, newConfigFileName, targetPath, templateId, e.PrimaryKey, cleanName);
                        //Map.WriteConfigToCloud2(ds2, newConfigFileName, false);
                        //if (System.IO.File.Exists(templateConfigFileName))
                        //    System.IO.File.Copy(templateConfigFileName, newConfigFileName);
                        //if (System.IO.File.Exists(templateSchemaFileName))
                        //    System.IO.File.Copy(templateSchemaFileName, newSchemaFileName);

                    }
                }
                catch (Exception exception)
                {
                    string sql = "delete durados_App where Id = " + e.PrimaryKey;
                    sqlAccess.ExecuteNonQuery(e.View.ConnectionString, sql);
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                    throw new DuradosException("Failed to create app, please try again later", exception);
                }
            }

            if (e.View.Fields.ContainsKey(imageFieldName) && e.View.Fields[imageFieldName].DefaultValue != null)
            {
                string defaultImage = Server.MapPath("~/Content/Images/" + e.View.Fields[imageFieldName].DefaultValue);
                //string destination = Server.MapPath("/Uploads/" + e.PrimaryKey + "/" + e.View.Fields[imageFieldName].DefaultValue);
                string destination = Maps.GetUploadPath(sqlProduct) + "\\" + e.PrimaryKey + "\\" + e.View.Fields[imageFieldName].DefaultValue;
                if (System.IO.File.Exists(defaultImage))
                    try
                    {
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(destination);
                        fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.
            
                        System.IO.File.Copy(defaultImage, destination);
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 2, "Could not copy the app default icon from: " + defaultImage + " to: " + destination);
                   
                    }
            }

            //UpdateCache(name);
            CreateDns(cleanName);
        }

        private SqlProduct GetSystemSqlProduct()
        {
            if (MySqlAccess.IsMySqlConnectionString(Maps.Instance.SystemConnectionString))
                return SqlProduct.MySql;
            return SqlProduct.SqlServer;
        }

        private string GetServerName(string connectionString)
        {
            return new MySqlSchema().GetServerName(connectionString);
        }

        private void CopyContainer(string sourcePath, string targetPath, bool p)
        {
            //throw new NotImplementedException();
        }

        protected void CreateDatabaseUser(string server, string catalog, string username, string password, bool integrated, string newUser, string newPassword, bool notify)
        {
            sqlAccess.CreateDatabaseOwnerUser(server, catalog, username, password, integrated, newUser, newPassword);
            if (notify)
                NotifyNewDatabaseUser(server, catalog, newUser, newPassword);
        }

        protected void NotifyNewDatabaseUser(string server, string catalog, string newUser, string newPassword)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            string subject = Map.Database.Localizer.Translate(CmsHelper.GetHtml("newDatabaseUserSubject"));
            string message = Map.Database.Localizer.Translate(CmsHelper.GetHtml("newDatabaseUserMessage"));

            string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;

            DataRow row = Map.Database.GetUserRow();

            message = message.Replace("[FirstName]", row["FirstName"].ToString());
            message = message.Replace("[LastName]", row["LastName"].ToString());
            message = message.Replace("[Url]", siteWithoutQueryString);
            message = message.Replace("[server]", server);
            message = message.Replace("[catalog]", catalog);
            message = message.Replace("[username]", newUser);
            message = message.Replace("[password]", newPassword);
            message = message.Replace("[Product]", Maps.Instance.DuradosMap.Database.SiteInfo.GetTitle());

            string to = row["Email"].ToString();

            Durados.Cms.DataAccess.Email.Send(host, Map.Database.UseSmtpDefaultCredentials, port, username, password, false, to.Split(';'), new string[0], new string[1] { from }, subject, message, from, null, null, DontSend, null, Map.Database.Logger, true);

        }
        private void SetAndWirteNewConfigFile(string templateConfigFileName, string newConfigFileName, string targetPath, string templateId, string newConsoleId, string appName)
        {

            DataSet ds = new DataSet();
            if ((!Maps.Cloud))
            {
                if (System.IO.File.Exists(templateConfigFileName))
                {
                    ds.ReadXml(templateConfigFileName);
                    ChangeAppIdInConfigFiels(new string[] { newConfigFileName }, templateId, newConsoleId);
                    // ds.Tables["Database"].Rows[0]["UploadFolder"] = targetPath;
                    ds.WriteXml(newConfigFileName, XmlWriteMode.WriteSchema);

                    if (System.IO.File.Exists(templateConfigFileName + ".xml"))
                        System.IO.File.Copy(templateConfigFileName + ".xml", newConfigFileName + ".xml");
                }
            }
            else
            {
                Map.ReadConfigFromCloud(ds, templateConfigFileName);
                ds.WriteXml(newConfigFileName, XmlWriteMode.WriteSchema);
                string[] newConfigTempFileNames = ChangeAppIdInConfigFiels(new string[] { newConfigFileName }, templateId, newConsoleId);
                foreach (string tempConfigFileName in newConfigTempFileNames)
                {
                    DataSet ds2 = new DataSet();
                    ds2.ReadXml(newConfigFileName);
                    Map.WriteConfigToCloud2(ds2, newConfigFileName, false, Map);
                    FileInfo fileInfo = new FileInfo(newConfigFileName);
                    fileInfo.Delete();
                }
                DataSet schemads = new DataSet();
                Map.ReadConfigFromCloud(schemads, templateConfigFileName + ".xml");
                Map.WriteConfigToCloud2(schemads, newConfigFileName + ".xml", false, Map);
            }


        }
        

        private string GetTempalteUploadFolder(string templateConfigFileName)
        {

            DataSet ds = new DataSet();
            if (!Maps.Cloud)
                ds.ReadXml(templateConfigFileName);
            else
                Map.ReadConfigFromCloud(ds, templateConfigFileName);
            return ds.Tables["Database"].Rows[0]["UploadFolder"].ToString();
        }


        //         private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        private void DirectoryCopy(string sourcePath, string targetPath, bool copySubDirs)
        {

            DirectoryInfo dir = new DirectoryInfo(Server.MapPath(sourcePath));
            if (!dir.Exists)
            {
                Map.Logger.Log(this.ToString(), "AfterCreateAfterCommit", "CopyUploadDirectory", null, 140, "The upload directory of the template app doesn't exists.");
                return;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();



            if (!Directory.Exists(Server.MapPath(targetPath)))
            {
                try
                {
                    Directory.CreateDirectory(Server.MapPath(targetPath));
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 2, null);
                    return;
                }
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(targetPath, file.Name);

                try
                {
                    file.CopyTo(Server.MapPath(temppath), true);
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 2, null);
                }
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(targetPath, subdir.Name);
                    string subdirpath = Path.Combine(sourcePath, subdir.Name);
                    DirectoryCopy(subdirpath, temppath, copySubDirs);
                }
            }
        }



        protected override void MoveUploadedFile(IUpload upload, string oldPath, string newPhisicalPath)
        {
            string newPath = upload.GetBaseUploadPath(newPhisicalPath);
            System.IO.File.Copy(oldPath, newPath);
        }

        //protected virtual void UpdateCache(string name)
        //{
        //    Maps.Instance.AddMap(name);
        //}

        protected virtual void CreateDns(string name)
        {
            if (Maps.Debug)
            {
                try
                {
                    string windowsPath = System.Environment.GetEnvironmentVariable("windir");

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(windowsPath + @"\system32\drivers\etc\hosts", true))
                    {

                        sw.WriteLine(string.Format("127.0.0.1   {0}", name + "." + Maps.Host));

                        sw.Close();

                    }
                }
                catch { }
            }


        }

        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);
            if (e.Values.ContainsKey("Name"))
            {
                MapDataSet.durados_AppRow appRow = (MapDataSet.durados_AppRow)e.PrevRow;
                string oldName = appRow.Name;
                string newName = e.Values["Name"].ToString();
                if (!oldName.Equals(newName))
                {
                    //Maps.Instance.ChangeName(oldName, newName);
                    CreateDns(newName);
                }

                SqlProduct product = Maps.GetSqlProduct(newName);

                if (product == SqlProduct.MySql)
                {
                    string url = Maps.GetAppUrl(newName);
                    string[] split = url.Split(':');
                    url = split[0] + ":" + split[1] + ":" + Maps.ProductsPort[product] + "/Admin/Restart?id=" + Map.Database.GetUserGuid();

                    Infrastructure.Http.CallWebRequest(url);

                }
                else
                {
                    Maps.Instance.Restart(oldName);
                }
            }
        }

        SqlAccess sqlAccess = new SqlAccess();
        private int? SaveConnection(string server, string catalog, string username, string password, string userId, SqlProduct sqlProduct,int port)
        {
            return SaveConnection(server, catalog, username, password, userId, sqlProduct, false, false, string.Empty, string.Empty, string.Empty, string.Empty, 0,port);
        }
        protected int? SaveConnection(string server, string catalog, string username, string password, string userId, SqlProduct sqlProduct)
        {
            return SaveConnection(server, catalog, username, password, userId, sqlProduct, false, false, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0);
        }

        protected int? SaveConnection(string server, string catalog, string username, string password, string userId, SqlProduct sqlProduct, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort)
        {
            View view = GetView("durados_SqlConnection");

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("ServerName", server);
            values.Add("Catalog", catalog);
            values.Add("Username", username);
            values.Add("IntegratedSecurity", false);
            values.Add("Password", password);
            values.Add(view.GetFieldByColumnNames("SqlProductId").Name, ((int)sqlProduct).ToString());
            values.Add(view.GetFieldByColumnNames("DuradosUser").Name, userId);

            values.Add("ProductPort", productPort.ToString());
            values.Add("SshRemoteHost", sshRemoteHost);
            values.Add("SshPort", sshPort);
            values.Add("SshUser", sshUser);
            values.Add("SshPassword", sshPassword);
            values.Add("SshPrivateKey", sshPrivateKey);
            values.Add("SshUses", usingSsh);
            values.Add("SslUses", usingSsl);
           
            string pk = view.Create(values);

            if (string.IsNullOrEmpty(pk))
                throw new DuradosException("Failed to get connection id");

            int id = -1;

            if (Int32.TryParse(pk, out id))
                return id;
            else
                throw new DuradosException("Failed to get connection id");
        }

        protected override void HandleMultiTenancyDownLoadAuthorization(string virtualPath)
        {
            string[] virtualPathDirs = VirtualPathUtility.GetDirectory(virtualPath.ToLower()).Trim('/').Split('/');

            if ((virtualPathDirs.Length > 1) && Session["UserApps"] != null && !((Dictionary<string, string>)Session["UserApps"]).ContainsKey(virtualPathDirs[1]))
            {
                throw new AccessViolationException();
            }

        }

        public override System.Web.Mvc.ActionResult IndexControl(string viewName, int? page, string SortColumn, string direction, string pks, string guid, string newPk, bool? firstTime, bool mainPage, bool? safety, bool? disabled)
        {
            if (Session["CurrentEditAppId"] != null) Session["CurrentEditAppId"] = null;
            if (Session["UserApps"] == null) Session["UserApps"] = ((DuradosMap)Map).GetUserApps();
            return base.IndexControl(viewName, page, SortColumn, direction, pks, guid, newPk, firstTime, mainPage, safety, disabled);
        }

        public override ActionResult IndexPage(string viewName, int? page, string SortColumn, string direction, string pk, string guid, bool? mainPage, bool? safety, string path)
        {
            if (Session["CurrentEditAppId"] != null) Session["CurrentEditAppId"] = null;
            if (Session["UserApps"] == null) Session["UserApps"] = ((DuradosMap)Map).GetUserApps();
            return base.IndexPage(viewName, page, SortColumn, direction, pk, guid, mainPage, safety, path);
        }

        public override JsonResult GetJsonView(string viewName, DataAction dataAction, string pk, string guid)
        {
            Session["CurrentEditAppId"] = pk ?? string.Empty;
            return base.GetJsonView(viewName, dataAction, pk, guid);
        }

        protected override void HandleMultiTenancyUploadPath(ref string uploadPath)
        {
            if (Map is DuradosMap)
            {
                if (Session["CurrentEditAppId"] != null && Session["CurrentEditAppId"].ToString() != string.Empty)
                    uploadPath = string.Format("{0}{1}\\", uploadPath, Session["CurrentEditAppId"]);
            }
            else
            {
                uploadPath = string.Format("{0}{1}\\", uploadPath, Map.Id);
            }
        }

        protected override void HandleUploadsSpecialPaths(Durados.View view, DataAction dataAction, Dictionary<string, object> values, string pk, DataActionEventArgs e)
        {
                if(dataAction.CompareTo(DataAction.Create)==0 )
                    base.HandleUploadsSpecialPaths(view, dataAction, values, pk, e);
        }
        protected override void HandleMultiTenancyAppFileName(ref string filename, string pk, bool isMultiTenancy)
        {
            string appId = pk;

            if (Session["CurrentEditAppId"] != null && Session["CurrentEditAppId"].ToString() != string.Empty)
                appId = Session["CurrentEditAppId"].ToString();

            if ((!string.IsNullOrEmpty(appId) && !filename.StartsWith(appId + "/")))
                filename = string.Format("{0}/{1}", appId, filename);

        }

        public JsonResult UpdateLogoTitle(string pk, string logo, string title)
        {
            if (string.IsNullOrEmpty(pk))
                return Json(new { success = false, message = "Cannot update. pk is missing" });

            if (string.IsNullOrEmpty(logo))
                return Json(new { success = false, message = "Missing logo" });

            if (string.IsNullOrEmpty(pk))
                return Json(new { success = false, message = "Missing title" });

            
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Image", logo);
            values.Add("Title", title);

            View view = (View)Maps.Instance.DuradosMap.Database.Views["durados_App"];

            view.Edit(values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

            Map.SiteInfo.Product = title;
            Map.Database.SiteInfo.Product = title;
            Map.SiteInfo.Logo = logo;


            return Json(new { success = true });
        }

        public class TemplateGenerator : Durados.DataAccess.AutoGeneration.Generator
        {
            public TemplateGenerator(string connectionString, string schemaGeneratorFileName)
                : base(connectionString, schemaGeneratorFileName)
            {
            }

            protected override string RootObjectName
            {
                get { return string.Empty; }
            }

            protected override bool SchemaExists(string connectionString)
            {
                return false;
            }
        }

    }

    public class PlugInUserException : DuradosException
    {
        public PlugInUserException(string message) : base(message) { }
    }
}



