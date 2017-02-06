using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Durados.Web.Mvc.UI.Helpers;

using Durados.DataAccess;
using Durados.Web.Mvc;
using System.Net.Http.Headers;
using Durados.Web.Mvc.Controllers.Api;
using System.Text.RegularExpressions;
using BackAnd.Web.Api.Controllers.Admin;
using Durados.Web.Mvc.Webhook;
using Durados.Web.Mvc.UI.Helpers.Cloning;
using System.Runtime.Caching;
/*
 HTTP Verb	|Entire Collection (e.g. /customers)	                                                        |Specific Item (e.g. /customers/{id})
-----------------------------------------------------------------------------------------------------------------------------------------------
GET	        |200 (OK), list data items. Use pagination, sorting and filtering to navigate big lists.	    |200 (OK), single data item. 404 (Not Found), if ID not found or invalid.
PUT	        |404 (Not Found), unless you want to update/replace every resource in the entire collection.	|200 (OK) or 204 (No Content). 404 (Not Found), if ID not found or invalid.
POST	    |201 (Created), 'Location' header with link to /customers/{id} containing new ID.	            |404 (Not Found).
DELETE	    |404 (Not Found), unless you want to delete the whole collection—not often desirable.	        |200 (OK). 404 (Not Found), if ID not found or invalid.
 
 */

namespace BackAnd.Web.Api.Controllers
{
    [RoutePrefix("admin/myAppConnection")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class myAppConnectionController : apiController
    {
        const string AppViewName = "durados_App";
        const string ConnectionViewName = "durados_SqlConnection";
        const string ProductPort = "productPort";
        const string Product = "product";

        protected internal override View GetView(string viewName)
        {
            return (View)Maps.Instance.DuradosMap.Database.Views[viewName];
        }


        private void CreateAppForNewDatabase(string id, string template, string name, string title, out string server, out string catalog, out string username, out string password, out int productPort, string sampleApp)
        {

            Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "start create new app in api", HttpStatusCode.OK.ToString(), 3, null, DateTime.Now);

            Durados.SqlProduct? sqlProduct = Durados.Web.Mvc.UI.Helpers.RDSNewDatabaseFactory.GetSqlProductfromTemplate(template);
            if (!sqlProduct.HasValue)
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, null, 1, "Faild to retrive Sql Product");
                throw new Exception("Failed to retrive new App parameters");
            }

            int port;

            AppFactory appFactory = new AppFactory();
            Durados.Web.Mvc.UI.Helpers.NewDatabaseParameters newDbParameters = null;
            try
            {
                newDbParameters = appFactory.GetNewExternalDBParameters(sqlProduct.Value, id, out  server, out port, sampleApp);//, out  catalog
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "continue create new app in api", HttpStatusCode.OK.ToString(), 3, null, DateTime.Now);

            }
            catch (Exception ex)
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, ex, 1, "Faild to retrive new app parameters");
                throw new Exception("Failed to retrive new App parameters", ex);
            }
            if (newDbParameters == null)
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, null, 1, "Faild to retrive new app parameters");
                throw new Exception("Failed to retrive new App parameters 2");
            }


            catalog = newDbParameters.DbName;
            username = newDbParameters.Username;
            password = newDbParameters.Password;
            productPort = port;

            //string data = string.Format(@"&template={0}&name={1}&title={2}&server={3}&catalog={4}&username={5}&password={6}&usingSsh={7}&usingSsl={8}&sshRemoteHost={9}&sshUser={10}&sshPassword={11}&sshPrivateKey={12}&sshPort={13}&productPort={14}&zone={15}&characterSetName={16}&engine={17}&engineVersion={18}&themeId={19}"
            //    , GetSqlProductFromTemplate(template)//0
            //    , name//1
            //    , title//2
            //    , server//3
            //    , catalog//4
            //    , newDbParameters.Username//5
            //    , newDbParameters.Password//6
            //    , false //usingSsh //7
            //    , false//usingSsl//8
            //    , string.Empty //sshRemoteHost //9
            //    , string.Empty //sshUser //10
            //    , string.Empty //sshPassword //11
            //    , string.Empty //sshPrivateKey //12
            //    , "22" //sshPort //13
            //    , port //14

            //    , newDbParameters.Zone//15
            //    , newDbParameters.CharacterSetName//16
            //    , newDbParameters.Engine//17
            //    , newDbParameters.EngineVersion//18

            //    , themeId);//19
            //id = GetTempGuid();
            ////move to the next page
            //string qstring = "id=" + id;
            //string url = RestHelper.GetAppUrl(Maps.DuradosAppName, Maps.OldAdminHttp) + "/Website/CreateAppGet?" + qstring + data;

            //Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "API Create App async call", url, 3, null, DateTime.Now);

            //string response = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(url);
            //json = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
            //return json;
        }

        private static string GetSqlProductFromTemplate(string template)
        {
            if (template == "10")
                return 4.ToString();
            else if (template == "11")
                return 8.ToString();
            else if (template == "12")
                return 1.ToString();
            else if (template == "13")
                return 7.ToString();
            return template;


        }


        protected virtual Dictionary<string, object> CreateApp(string template, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort, int? themeId, int? templateId)
        {
            return CreateApp2(template, name, title, server, catalog, username, password, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, themeId, templateId);
        }

        #region create app
        private void RunDummyCall(string callbackUrl)
        {
            string response = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(callbackUrl + "&success=true");
            Dictionary<string, object> json = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
        }
        private static int GetStorageSize(Durados.SqlProduct product)
        {
            return 5;
        }

        private static string GetInstanceDBClass()
        {
            return "db.t1.micro";
        }
        private string GetPostDataForCreateNewRdsDatabase(string callbackUrl)
        {

            //  string ipRange = System.Configuration.ConfigurationManager.AppSettings["rdsIpRange"] ?? "[0.0.0.0/32]";
            string authToken = System.Configuration.ConfigurationManager.AppSettings["nodeServicesAuth"] ?? GetMasterGuid();
            string securityGroup = System.Configuration.ConfigurationManager.AppSettings["AWSsecurityGroup"] ?? "bknd-Allcustomer";

            string dbClass = GetInstanceDBClass();
            int storageSize = GetStorageSize(newDbParameters.SqlProduct);
            //List<string> IPRange = new List<string>();
            //IPRange.Add(ipRange);

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("instanceName", newDbParameters.InstanceName);
            dict.Add("dbName", newDbParameters.DbName);
            dict.Add("instanceClass", dbClass);
            dict.Add("storageSize", storageSize.ToString());
            //dict.Add("IPRange", ipRange);
            dict.Add("engine", newDbParameters.Engine);
            dict.Add("engineVersion", newDbParameters.EngineVersion);// need to change
            dict.Add("username", newDbParameters.Username);
            dict.Add("password", newDbParameters.Password);
            dict.Add("region", newDbParameters.Zone);
            dict.Add("characterSetName", newDbParameters.CharacterSetName);
            dict.Add("callbackUrl", "callbackUrlValue");
            dict.Add("authToken", authToken);
            dict.Add("securityGroup", securityGroup);

            //string postData = Newtonsoft.Json.JsonConvert.SerializeObject(dict);
            System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            string postData = jsSerializer.Serialize(dict);
            postData = postData.Replace("callbackUrlValue", callbackUrl);
            return postData; ;
        }

        private string GetNewAppGuid(Durados.CreateEventArgs e)
        {
            Guid guid;
            string sql = "Select [Guid] from durados_app with(nolock) where id=" + e.PrimaryKey;
            e.Command.CommandText = sql;

            object scalar = e.Command.ExecuteScalar();
            if (scalar == null || scalar == DBNull.Value || !Guid.TryParse(scalar.ToString(), out guid))
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", "GetNewAppGuid", null, 1, "Failed to retrive guid for app id=" + e.PrimaryKey);
                throw new Durados.DuradosException("Failed to retrive guid for app id=" + e.PrimaryKey);
            }


            return guid.ToString();
        }

        NewDatabaseParameters newDbParameters;
        private bool IsTempAppBelongToCreator(Durados.CreateEventArgs e, out int? appId)
        {
            string appName = e.Values["Name"].ToString();
            int creator = Convert.ToInt32(GetUserID());
            appId = Maps.Instance.AppExists(appName, creator);
            return appId.HasValue;
        }
        int? tempAppId = null;

        //private object locker1 = new object();

        protected override void BeforeCreate(Durados.CreateEventArgs e)
        {
            if (e.View.Name == "durados_App")
            {
                if (IsTempAppBelongToCreator(e, out tempAppId))
                {
                    string sqlDeleteTempApp = "delete durados_App with (rowlock) where Id = " + tempAppId.Value;
                    e.Command.CommandText = sqlDeleteTempApp;

                    //lock (locker1)
                   // {
                    Durados.SmartRun.RunWithRetry.Run<System.Data.SqlClient.SqlException>(() =>
                    {
                        e.Command.ExecuteNonQuery();
                    }, 8, 2000);
                   // }
                    
                }
            }
            const string DatabaseStatus = "DatabaseStatus";
            e.Values.Add(DatabaseStatus, (int)OnBoardingStatus.Processing);

            BeforeCreate2(e);
        }

        string nameFieldName = "Name";
        string imageFieldName = "Image";
        string dataSourceTypeFieldName = "FK_durados_App_durados_DataSourceType_Parent";

        private string GetUrl(string name)
        {
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
            string host = Maps.Host;

            if (System.Web.HttpContext.Current.Request.Url.ToString().Contains(port))
                host += ":" + port;

            return string.Format("{0}.{1}|_blank|" + System.Web.HttpContext.Current.Request.Url.Scheme + "://{0}.{1}?appName={0}", name, host);

        }
        protected void BeforeCreate2(Durados.CreateEventArgs e)
        {
            if (Map.Database.GetCurrentUsername().ToLower().StartsWith("wix"))
            {
                throw new Durados.Web.Mvc.Controllers.PlugInUserException("You cannot make this operation as a Wix user. Please <a href='/Account/LogOff'>logout</a> and register.");
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
                Durados.Field sqlConnectionField = e.View.GetFieldByColumnNames("SqlConnectionId");
                object sqlConnectionId = e.Values[sqlConnectionField.Name];
                if (sqlConnectionId == null || sqlConnectionId.Equals(string.Empty))
                    throw new Durados.DuradosException(Map.Database.Localizer.Translate("Please create or select a ") + sqlConnectionField.DisplayName);
            }

            if (dataSourceTypeId == "4") // template
            {
                //sqlAccess.ExecuteNoneQueryStoredProcedure(Maps.Instance.ConnectionString, "durados_CreateNewDatabase", values);
                Durados.Field templateField = e.View.GetFieldByColumnNames("TemplateId");
                object templateId = e.Values[templateField.Name];
                if (templateId == null || templateId.Equals(string.Empty))
                    throw new Durados.DuradosException(Map.Database.Localizer.Translate("Please select an ") + templateField.DisplayName);
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

            //if (Session["UserApps"] != null)
            //    Session["UserApps"] = null;
            base.BeforeCreate(e);
        }

        protected override void AfterCreateBeforeCommit(Durados.CreateEventArgs e)//dateConnectionStatus(connectionId.Value, json["status"]== "Ok" ? "Pending" : "Faild");
        {
            if (e.View.Name != "durados_App")
                return;
            if (newDbParameters != null)
            {
                if (string.IsNullOrEmpty(e.PrimaryKey))
                    throw new Durados.DuradosException("Failed to save new app.");
                string callbackUrl = string.Format("{0}/admin/myAppConnection/rdsResponse?appguid={1}&appname={2}", Maps.ApiUrls[0], GetNewAppGuid(e), e.Values["Name"].ToString());


                string url = System.Configuration.ConfigurationManager.AppSettings["nodeServicesUrl"] + "/createRDSInstance";
                ///{"instanceName":"yrvtest23","dbName":"yrvtest23","instanceClass":"db.t1.micro","storageSize":"5","IPRange":["0.0.0.0/32"],"engine":"MySQL","engineVersion":"5.6.21","username":"yariv","password":"123456789","region":"us-east-1","characterSetName":"ASCII","callbackUrl":"http://backand-dev3.cloudapp.net:4109/admin/myAppConnection/rdsResponse?appguid=86bec9ad-3319-423d-8125-9860ccd535c4&appname=test1&success=true","authToken":"123456789","securityGroup":"bknd-Allcustomers"}

                string postData = GetPostDataForCreateNewRdsDatabase(callbackUrl);
                Dictionary<string, object> json = new Dictionary<string, object>();

                try
                {
                    if (newDbParameters.Engine == RDSEngin.sqlserver_ee.ToString().Replace('_', '-'))
                    {
                        System.Threading.Tasks.Task.Run(() => RunDummyCall(callbackUrl));
                        json.Add("status", null);
                    }
                    else
                    {
                        string response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(url, postData);
                        json = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
                    }
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "Failed to create Amazon RDS database. username=" + Map.Database.GetUsernameById(Map.Database.GetUserID()));
                    throw new Durados.DuradosException("Server is busy, Please try again later.");
                }

            }

            base.AfterCreateBeforeCommit(e);
        }
        private string GetServerName(string connectionString)
        {
            return new MySqlSchema().GetServerName(connectionString);
        }

        private Durados.SqlProduct GetSystemSqlProduct()
        {
            if (MySqlAccess.IsMySqlConnectionString(Maps.Instance.SystemConnectionString))
                return Durados.SqlProduct.MySql;
            return Durados.SqlProduct.SqlServer;
        }

        private string GetTempalteUploadFolder(string templateConfigFileName)
        {

            System.Data.DataSet ds = new System.Data.DataSet();
            if (!Maps.Cloud)
                ds.ReadXml(templateConfigFileName);
            else
                Map.ReadConfigFromCloud(ds, templateConfigFileName);
            return ds.Tables["Database"].Rows[0]["UploadFolder"].ToString();
        }

        private void DirectoryCopy(string sourcePath, string targetPath, bool copySubDirs)
        {

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(System.Web.HttpContext.Current.Server.MapPath(sourcePath));
            if (!dir.Exists)
            {
                Map.Logger.Log(this.ToString(), "AfterCreateAfterCommit", "CopyUploadDirectory", null, 140, "The upload directory of the template app doesn't exists.");
                return;
            }

            System.IO.DirectoryInfo[] dirs = dir.GetDirectories();



            if (!System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(targetPath)))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(targetPath));
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 2, null);
                    return;
                }
            }

            System.IO.FileInfo[] files = dir.GetFiles();
            foreach (System.IO.FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(targetPath, file.Name);

                try
                {
                    file.CopyTo(System.Web.HttpContext.Current.Server.MapPath(temppath), true);
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 2, null);
                }
            }

            if (copySubDirs)
            {
                foreach (System.IO.DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(targetPath, subdir.Name);
                    string subdirpath = System.IO.Path.Combine(sourcePath, subdir.Name);
                    DirectoryCopy(subdirpath, temppath, copySubDirs);
                }
            }
        }

        private void CopyContainer(string sourcePath, string targetPath, bool p)
        {
            //throw new NotImplementedException();
        }

        protected string[] ChangeAppIdInConfigFiels(string[] fileNames, string oldConsoleId, string to)
        {
            string baseFolder = System.Configuration.ConfigurationManager.AppSettings["ChangeAppIdInConfigFielsFolder"] ?? "C:\\temp";

            List<string> newFileNames = new List<string>();
            foreach (string oldfilename in fileNames)
            {

                System.IO.FileInfo info = new System.IO.FileInfo(oldfilename);

                string newFileName = baseFolder + "\\" + info.Name.Replace(oldConsoleId, to);

                System.IO.File.Copy(oldfilename, newFileName, true);

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(newFileName);
                System.Xml.XmlNodeList uploadNodes = doc.SelectNodes("/NewDataSet/Upload/UploadVirtualPath/text()[contains(.,'" + oldConsoleId + "')]");

                foreach (System.Xml.XmlNode node in uploadNodes)
                {
                    node.InnerText = node.InnerText.Replace(oldConsoleId, to);
                }

                uploadNodes = doc.SelectNodes("/NewDataSet/Database/UploadFolder/text()[contains(.,'" + oldConsoleId + "')]");
                foreach (System.Xml.XmlNode node in uploadNodes)
                {
                    node.InnerText = node.InnerText.Replace(oldConsoleId, to);
                }

                doc.Save(newFileName);
                newFileNames.Add(newFileName);
            }
            return newFileNames.ToArray();
        }
        private void SetAndWirteNewConfigFile(string templateConfigFileName, string newConfigFileName, string targetPath, string templateId, string newConsoleId, string appName)
        {

            System.Data.DataSet ds = new System.Data.DataSet();
            if ((!Maps.Cloud))
            {
                if (System.IO.File.Exists(templateConfigFileName))
                {
                    ds.ReadXml(templateConfigFileName);
                    ChangeAppIdInConfigFiels(new string[] { newConfigFileName }, templateId, newConsoleId);
                    // ds.Tables["Database"].Rows[0]["UploadFolder"] = targetPath;
                    ds.WriteXml(newConfigFileName, System.Data.XmlWriteMode.WriteSchema);

                    if (System.IO.File.Exists(templateConfigFileName + ".xml"))
                        System.IO.File.Copy(templateConfigFileName + ".xml", newConfigFileName + ".xml");
                }
            }
            else
            {
                Map.ReadConfigFromCloud(ds, templateConfigFileName);
                ds.WriteXml(newConfigFileName, System.Data.XmlWriteMode.WriteSchema);
                string[] newConfigTempFileNames = ChangeAppIdInConfigFiels(new string[] { newConfigFileName }, templateId, newConsoleId);
                foreach (string tempConfigFileName in newConfigTempFileNames)
                {
                    System.Data.DataSet ds2 = new System.Data.DataSet();
                    ds2.ReadXml(newConfigFileName);
                    Map.WriteConfigToCloud2(ds2, newConfigFileName, false, Map);
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(newConfigFileName);
                    fileInfo.Delete();
                }
                System.Data.DataSet schemads = new System.Data.DataSet();
                Map.ReadConfigFromCloud(schemads, templateConfigFileName + ".xml");
                Map.WriteConfigToCloud2(schemads, newConfigFileName + ".xml", false, Map);
            }


        }

        protected override void AfterCreateAfterCommit(Durados.CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);

            string dataSourceTypeId = e.Values[dataSourceTypeFieldName].ToString();
            string name = e.Values[nameFieldName].ToString();
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = Map.connectionString;
            string pk = e.PrimaryKey;

            string cleanName = GetCleanName(name);

            Durados.SqlProduct sqlProduct = Durados.SqlProduct.SqlServer;

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
                catch (System.Data.SqlClient.SqlException exception)
                {
                    throw new Durados.DuradosException(exception.Message, exception);
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
                string newPassword = new Durados.Web.Mvc.Controllers.AccountMembershipService().GetRandomPassword(12);
                string newUsername = appCatalog + "User";
                try
                {
                    CreateDatabaseUser(builder.DataSource, appCatalog, builder.UserID, builder.Password, builder.IntegratedSecurity, newUsername, newPassword, false);
                    sqlAccess.CreateDatabaseUserWithoutLogin(Map.connectionString, sysCatalog, newUsername, newPassword, "db_owner");
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "Failed to create database user. username=" + newUsername);
                    throw new Durados.DuradosException("Failed to create database user");
                }
                int? appConnId = SaveConnection(builder.DataSource, appCatalog, newUsername, newPassword, duradosUser, Durados.SqlProduct.SqlServer);

                int? sysConnId = SaveConnection(builder.DataSource, sysCatalog, newUsername, newPassword, duradosUser, Durados.SqlProduct.SqlServer);

                //values = new Dictionary<string, object>();
                //values.Add("FK_durados_App_durados_SqlConnection_Parent", appConnId);
                //values.Add("FK_durados_App_durados_SqlConnection_System_Parent", sysConnId);

                //e.View.Edit(values, e.PrimaryKey, null, null, null, null);

                string sql = "update durados_App set SqlConnectionId = " + appConnId + ", SystemSqlConnectionId = " + sysConnId + " where id = " + e.PrimaryKey;

                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Maps.Instance.ConnectionString))
                {
                    connection.Open();
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, connection))
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
                    string newPassword = new Durados.Web.Mvc.Controllers.AccountMembershipService().GetRandomPassword(12);
                    string newUsername = Durados.Database.ProductShorthandName + pk;
                    string serverName = GetServerName(Maps.Instance.SystemConnectionString);
                    //Dictionary<string, object> values = new Dictionary<string, object>();
                    //values.Add("AppId", Convert.ToInt32(pk));
                    //values.Add("Catalog", Maps.DuradosAppPrefix + pk);
                    //values.Add("SysCatalog", Maps.DuradosAppSysPrefix + pk);
                    //values.Add("DuradosUser", Map.Database.GetUserID());
                    //sqlAccess.ExecuteNoneQueryStoredProcedure(Maps.Instance.ConnectionString, "durados_SetSysConnection", values);
                    Durados.SqlProduct systemSqlProduct = GetSystemSqlProduct();
                    int? sysConnId = null;
                    if (systemSqlProduct == Durados.SqlProduct.SqlServer)
                    {
                        try
                        {
                            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Maps.Instance.ConnectionString))
                            {
                                connection.Open();
                                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand())
                                {
                                    command.Connection = connection;
                                    sqlAccess.ExecuteNonQuery(e.View, command, "create database " + sysCatalog, null);
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "Failed to create system database. catalog=" + sysCatalog);
                            throw new Durados.DuradosException("Failed to create database");
                        }

                        try
                        {
                            CreateDatabaseUser(builder.DataSource, sysCatalog, builder.UserID, builder.Password, builder.IntegratedSecurity, newUsername, newPassword, false);
                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "Failed to create database user. username=" + newUsername);
                            throw new Durados.DuradosException("Failed to create database user");
                        }
                        sysConnId = SaveConnection(builder.DataSource, sysCatalog, newUsername, newPassword, duradosUser, Durados.SqlProduct.SqlServer);

                    }
                    else if (systemSqlProduct == Durados.SqlProduct.MySql)
                    {
                        try
                        {
                            NewDatabaseParameters sysDbParameters = new NewDatabaseParameters { DbName = sysCatalog, Username = newUsername, Password = newPassword };
                            AppFactory appFactory = new AppFactory();
                            appFactory.CreateNewSystemSchemaAndUser(Maps.Instance.SystemConnectionString, sysDbParameters);
                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "Failed to create database user. username=" + newUsername);
                            throw new Durados.DuradosException("Failed to create database user");
                        }

                        int port = Convert.ToInt32(new MySqlSchema().GetPort(Maps.Instance.SystemConnectionString));
                        sysConnId = SaveConnection(serverName, sysCatalog, newUsername, newPassword, duradosUser, Durados.SqlProduct.MySql, port);
                    }
                    //Dictionary<string, object> values = new Dictionary<string, object>();

                    //values = new Dictionary<string, object>();
                    //values.Add("FK_durados_App_durados_SqlConnection_System_Parent", sysConnId);

                    //e.View.Edit(values, e.PrimaryKey, null, null, null, null);
                    string sql = "update durados_App set SystemSqlConnectionId = " + sysConnId + " where id = " + e.PrimaryKey;

                    using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Maps.Instance.ConnectionString))
                    {
                        connection.Open();
                        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, connection))
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
                        using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Maps.Instance.ConnectionString))
                        {
                            connection.Open();
                            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("exec('create database " + sysCatalog + "')", connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (System.Data.SqlClient.SqlException exception)
                {
                    throw new Durados.DuradosException("Could not create system database: " + sysCatalog + "; Additional info: " + exception.Message, exception);
                }

                try
                {
                    if (!appExists)
                    {
                        using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Maps.Instance.ConnectionString))
                        {
                            connection.Open();
                            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("exec('create database " + appCatalog + "')", connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (System.Data.SqlClient.SqlException exception)
                {
                    throw new Durados.DuradosException("Could not create app database: " + appCatalog + "; Additional info: " + exception.Message, exception);
                }


                string duradosUser = Map.Database.GetUserID();
                string newPassword = new Durados.Web.Mvc.Controllers.AccountMembershipService().GetRandomPassword(12);
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
                        catch (System.Data.SqlClient.SqlException)
                        {
                            sqlAccess.CreateDatabaseUserWithoutLogin(Map.connectionString, sysCatalog, newUsername, newPassword, "db_owner");
                        }
                    }
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "Failed to create database user. username=" + newUsername);
                    throw new Durados.DuradosException("Failed to create database user");
                }

                int? appConnId = null;

                if (!appExists)
                    appConnId = SaveConnection(builder.DataSource, appCatalog, newUsername, newPassword, duradosUser, Durados.SqlProduct.SqlServer);
                else
                    appConnId = Convert.ToInt32(e.Values[appconnstrFieldName]);

                int? sysConnId = null;
                if (!sysExists)
                    sysConnId = SaveConnection(builder.DataSource, sysCatalog, newUsername, newPassword, duradosUser, Durados.SqlProduct.SqlServer);
                else
                    sysConnId = Convert.ToInt32(e.Values[sysconnstrFieldName]);

                //values = new Dictionary<string, object>();
                //values.Add("FK_durados_App_durados_SqlConnection_Parent", appConnId);
                //values.Add("FK_durados_App_durados_SqlConnection_System_Parent", sysConnId);

                //e.View.Edit(values, e.PrimaryKey, null, null, null, null);

                string sql2 = "update durados_App set SqlConnectionId = " + appConnId + ", SystemSqlConnectionId = " + sysConnId + " where id = " + e.PrimaryKey;

                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Maps.Instance.ConnectionString))
                {
                    connection.Open();
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql2, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }




                try
                {
                    string templateId = e.Values[e.View.GetFieldByColumnNames("TemplateId").Name].ToString();
                    System.Data.DataRow templateAppRow = e.View.GetDataRow(templateId);

                    if (templateAppRow != null)
                    {
                        string image = string.Format("{0}/{1}", e.PrimaryKey, Regex.Replace(templateAppRow["Image"].ToString(), @"^[\d]+/", string.Empty));

                        string connectionId = e.Values[e.View.GetFieldByColumnNames("SqlConnectionId").Name].ToString();
                        if (string.IsNullOrEmpty(connectionId))
                        {
                            connectionId = templateAppRow["SqlConnectionId"].ToString();
                        }
                        View sqlConnectionView = (View)e.View.Database.Views["durados_SqlConnection"];
                        System.Data.DataRow sqlConnectionRow = sqlConnectionView.GetDataRow(connectionId);
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
                        sqlProduct = sqlConnectionRow.IsNull("SqlProductId") ? Durados.SqlProduct.SqlServer : (Durados.SqlProduct)sqlConnectionRow["SqlProductId"];
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
                    string sql = "delete durados_App with (rowlock) where Id = " + e.PrimaryKey;
                    sqlAccess.ExecuteNonQuery(e.View.ConnectionString, sql);
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, null);
                    throw new Durados.DuradosException("Failed to create app, please try again later", exception);
                }
            }

            if (e.View.Fields.ContainsKey(imageFieldName) && e.View.Fields[imageFieldName].DefaultValue != null)
            {
                string defaultImage = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/" + e.View.Fields[imageFieldName].DefaultValue);
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
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 2, "Could not copy the app default icon from: " + defaultImage + " to: " + destination);

                    }
            }

        }

        

        private bool IsValidConnectionData(Durados.SqlProduct product, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort, out string errors)
        {
            errors = string.Empty;


            if (string.IsNullOrEmpty(name))
                errors += "<br>Appication name is required";

            if (string.IsNullOrEmpty(server))
                errors += "<br>End Point/Server name is required";

            if (string.IsNullOrEmpty(catalog))
                errors += "<br>Db name is required";

            if (string.IsNullOrEmpty(username))
                errors += "<br>Username is required";

            if (string.IsNullOrEmpty(password))
                errors += "<br>Password is required";

            if (productPort == 0)
                errors += "<br>Port is required";
            if (product == Durados.SqlProduct.MySql && usingSsh)
            {
                if (string.IsNullOrEmpty(sshRemoteHost))
                    errors += "<br>SSH Remote Host is required";

                if (sshPort == 0)
                    errors += "<br>SSH port is 0";

                if (string.IsNullOrEmpty(sshPrivateKey))
                {
                    if (string.IsNullOrEmpty(sshUser))
                        errors += "<br>SSH User is required";

                    if (string.IsNullOrEmpty(sshPassword))
                        errors += "<br>SSH password is required";
                }

            }

            if (!string.IsNullOrEmpty(errors))
                return false;
            else
                return true;

        }

        private string GetCleanName(string name)
        {
            name = name.Trim();

            if (name.ToLower().Equals("www"))
                throw new Durados.DuradosException(Map.Database.Localizer.Translate("This name already exists."));

            Regex regex = new Regex("^[A-Za-z0-9\\-]+$");/**\\- support - inside url*/
            if (!regex.IsMatch(name))
            {
                throw new Durados.DuradosException(Map.Database.Localizer.Translate("The 'Name' is the first part of the URL of the Console.\n\rOnly alphanumeric characters are allowed."));
            }

            if (name.Length > Maps.AppNameMax)
                throw new Durados.DuradosException(Map.Database.Localizer.Translate("The 'Name' is the first part of the URL of the Console.\n\rMaximum 25 characters are allowed."));


            return name;
        }

        private void ValidateConnection(string server, string catalog, string username, string password)
        {
            ValidateConnection(server, catalog, username, password, Durados.SqlProduct.SqlServer, 0, false, false, null, null, null, null, null, 0, 0);
        }

        SqlAccess sqlAccess = new SqlAccess();

        private string CreateDatabase(string server, string catalog, string username, string password, string source, string template)
        {
            if (template == "1")
                return sqlAccess.CopyAzureDatabase(server, catalog, username, password, source);
            else
                return sqlAccess.CopySqlServerDatabase(server, catalog, username, password, source, Maps.DemoOnPremiseSourcePath);
        }

        private string RenamePendingDatabase(string server, string catalog, string username, string password, string source)
        {
            return sqlAccess.RenameAzureDatabase(server, catalog, username, password, source);
        }

        private void CreateDatabaseUser(string server, string catalog, string username, string password, bool integrated, string newUser, string newPassword, bool notify)
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

            System.Data.DataRow row = Map.Database.GetUserRow();

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

        public bool DontSend
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DontSend"] ?? "false");
            }
        }

        protected virtual void SendError(int logType, Exception exception, string controller, string action, Durados.Web.Mvc.Logging.Logger logger)
        {
            SendError(logType, exception, controller, action, logger, string.Empty);
        }

        protected virtual void SendError(int logType, Exception exception, string controller, string action, Durados.Web.Mvc.Logging.Logger logger, string moreInfo)
        {
            try
            {
                bool sendError = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["sendError"]) && logType == 1;
                if (sendError)
                {
                    Durados.Cms.Services.EmailProvider provider = new Durados.Cms.Services.EmailProvider();
                    Durados.Cms.Services.SMTPServiceDetails smtp = provider.GetSMTPServiceDetails(provider.GetSMTPProvider());

                    string applicationName = System.Web.HttpContext.Current.Request.Url.Host;

                    string defaultTo = smtp.to;
                    string[] to = !string.IsNullOrEmpty(Map.Database.AdminEmail) ? Map.Database.AdminEmail.Split(';') : null;
                    string[] cc = new string[1] { defaultTo };
                    if (to == null || to.Length == 0)
                    {
                        to = cc;
                        cc = null;
                    }



                    string message = "The following error occurred:\n\r" + exception.ToString();
                    if (!string.IsNullOrEmpty(moreInfo))
                    {
                        message += "\n\r\n\r\n\rMore info:\n\r" + moreInfo;
                    }

                    Durados.Cms.DataAccess.Email.Send(smtp.host, smtp.useDefaultCredentials, smtp.port, smtp.username, smtp.password, false, to, cc, null, applicationName + " error", message, smtp.from, null, null, DontSend, logger);
                }
            }
            catch (Exception ex)
            {
                logger.Log(controller, action, exception.Source, ex, 1, "Error sending email when logging an exception");
            }
        }
        private int? SaveConnection(string server, string catalog, string username, string password, string userId, Durados.SqlProduct sqlProduct, int port)
        {
            return SaveConnection(server, catalog, username, password, userId, sqlProduct, false, false, string.Empty, string.Empty, string.Empty, string.Empty, 0, port);
        }
        protected int? SaveConnection(string server, string catalog, string username, string password, string userId, Durados.SqlProduct sqlProduct)
        {
            return SaveConnection(server, catalog, username, password, userId, sqlProduct, false, false, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0);
        }

        protected int? SaveConnection(string server, string catalog, string username, string password, string userId, Durados.SqlProduct sqlProduct, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort)
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
                throw new Durados.DuradosException("Failed to get connection id");

            int id = -1;

            if (Int32.TryParse(pk, out id))
                return id;
            else
                throw new Durados.DuradosException("Failed to get connection id");
        }

        private void HandleTemplate(System.Data.DataRow row, string connectionString)
        {
            HandleTemplate(row["Name"].ToString(), row["Id"].ToString(), (MapDataSet.durados_AppRow)row, connectionString);
        }

        private void HandleTemplate(string name, string id, MapDataSet.durados_AppRow appRow, string connectionString)
        {
            string oldConfigFileName = Maps.GetConfigPath(string.Format("durados_AppSys_{0}.xml", id));
            string newConfigFileName = Maps.GetConfigPath(string.Format("{0}.xml", Maps.DemoConfigFilename));
            string newSchemaFileName = newConfigFileName + ".xml";
            string oldSchemaFileName = oldConfigFileName + ".xml";


            System.IO.File.Copy(newConfigFileName, oldConfigFileName);
            System.IO.File.Copy(newSchemaFileName, oldSchemaFileName);

            string directory = Maps.DemoFtpPhysicalPath + name;
            System.IO.Directory.CreateDirectory(directory);

            string employeePicture = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/pic.jpg");
            System.IO.File.Copy(employeePicture, directory + @"\" + "pic.jpg");

            try
            {
                AzureHelper.CopyAzureContainer(Convert.ToInt32(id), oldConfigFileName);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "Could not copy uploads");
            }

            //Maps.Instance.Restart(name);

        }
        private string GetThemePath(int? themeId)
        {
            return Maps.Instance.GetTheme(themeId).RelativePath;
        }

        //public Dictionary<string, object> CreateAppGet2(string template, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort, string zone, string characterSetName, string engine, string engineVersion, int? themeId)
        //{
        //    Durados.SqlProduct? product = Durados.Web.Mvc.UI.Helpers.RDSNewDatabaseFactory.GetSqlProductfromTemplate(template);
        //    newDbParameters = new NewDatabaseParameters();
        //    newDbParameters.Zone = zone;
        //    newDbParameters.Engine = engine;
        //    newDbParameters.EngineVersion = engineVersion;
        //    newDbParameters.CharacterSetName = characterSetName;
        //    newDbParameters.InstanceName = server;
        //    newDbParameters.DbName = catalog;
        //    newDbParameters.Username = username;
        //    newDbParameters.Password = password;
        //    newDbParameters.SqlProduct = product.HasValue ? product.Value : Durados.SqlProduct.SqlServer;
        //    server = System.Configuration.ConfigurationManager.AppSettings["rdsCreatDbPendingMessage"] + "Not available yet";
        //    return CreateApp2(template, name, title, server, catalog, username, password, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, themeId);
        //}

        private Dictionary<string, object> CreateApp2(string template, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort, int? themeId, int? templateId)
        {
            Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", "CreateApp", "started", "", 3, "server: " + server + ", database: " + catalog, DateTime.Now);

            Durados.SqlProduct sqlProduct = Durados.SqlProduct.SqlServer;

            if (template == "1" || template == "3")
                sqlProduct = Durados.SqlProduct.SqlAzure;
            else if (template == "4")
                sqlProduct = Durados.SqlProduct.MySql;
            else if (template == "8")
                sqlProduct = Durados.SqlProduct.Postgre;
            else if (template == "7")
                sqlProduct = Durados.SqlProduct.Oracle;

            if (sshPrivateKey != null)
                sshPrivateKey = sshPrivateKey.Replace(System.Environment.NewLine, string.Empty);
            string errors;
            if (!IsValidConnectionData(sqlProduct, name, title, server, catalog, username, password, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, out errors))
                return new Dictionary<string, object>() { { "Success", false }, { "Message", errors } };
            bool isBlankDababase = template.ToLower() == "5";
            bool isAzureDemo = template.ToLower() == "1";
            bool isOnPremiseDemo = template.ToLower() == "0";

            bool isRdsBlank = IsNewDatabase(template);
            if (isRdsBlank)
                sqlProduct = Durados.Web.Mvc.UI.Helpers.RDSNewDatabaseFactory.GetSqlProductfromTemplate(template).Value;
            bool connectionExists = false;
            string userId = GetUserID();
            string templateConnectionString = null;
            string demoUsername = Maps.DemoSqlUsername;
            string demoPassword = Maps.DemoSqlPassword;
            bool isDemo = isAzureDemo || isOnPremiseDemo;

            bool basic = !isDemo;

            if (isAzureDemo)
            {
                demoUsername = Maps.DemoAzureUsername;
                demoPassword = Maps.DemoAzurePassword;
            }

            string newPassword = password;
            if (isAzureDemo || isOnPremiseDemo)
                newPassword = new Durados.Web.Mvc.Controllers.AccountMembershipService().GetRandomPassword(12);
            string newUsername = username;

            if (string.IsNullOrEmpty(userId))
                return new Dictionary<string, object>() { { "Success", false }, { "Message", "Please login or sign-up." } };

            View view = GetView("durados_App");

            try
            {
                name = GetCleanName(name);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "app name exception");

                return new Dictionary<string, object>() { { "Success", false }, { "Message", exception.Message } };
            }

            int? connectionId = null;
            bool pendingExists = false;

            if (isAzureDemo || isOnPremiseDemo)
            {
                string source = Maps.DemoDatabaseName + Map.SourceSuffix;
                bool templateExists = Maps.Instance.AppExists(name).HasValue;
                if (templateExists)
                {
                    return new Dictionary<string, object>() { { "Success", true }, { "Message", Maps.GetAppUrl(name) } };
                }
                else
                {
                    connectionId = Maps.Instance.GetConnection(server, catalog, newUsername, userId);
                    connectionExists = connectionId.HasValue;
                    if (connectionExists)
                    {
                        try
                        {
                            ValidateConnection(server, catalog, newUsername, newPassword);
                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 3, null);

                            connectionExists = false;
                        }
                    }

                    if (!connectionExists)
                    {
                        if (isAzureDemo)
                        {
                            string pending = Maps.GetPendingDatabase(template);

                            //try
                            //{
                            try
                            {
                                for (int i = 0; i < Maps.DemoPendingNext; i++)
                                {
                                    try
                                    {
                                        pendingExists = false;
                                        ValidateConnection(server, pending, demoUsername, demoPassword);
                                        templateConnectionString = RenamePendingDatabase(server, catalog, demoUsername, demoPassword, pending);
                                        pendingExists = true;
                                        break;
                                    }
                                    catch (Exception exception)
                                    {
                                        pending = Maps.GetPendingDatabase(template);
                                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 3, "pending=" + pending + ";i=" + i);
                                    }
                                }
                                if (pendingExists)
                                {
                                    CreateDatabase(server, pending, demoUsername, demoPassword, source, template);
                                }
                            }
                            catch (Exception exception)
                            {
                                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 3, null);

                                pendingExists = false;
                            }

                            //}
                            //catch (Exception exception)
                            //{
                            //    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 3, null);

                            //    pendingExists = false;
                            //}



                            if (!pendingExists)
                            {
                                try
                                {
                                    templateConnectionString = CreateDatabase(server, catalog, demoUsername, demoPassword, source, template);

                                }
                                catch (Exception exception)
                                {
                                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "Failed to create Azure database. username=" + newUsername);
                                    return new Dictionary<string, object>() { { "Success", false }, { "Message", "Server is busy, Please try again later." } };
                                }
                            }
                            else
                            {
                                try
                                {
                                    CreateDatabaseUser(server, catalog, demoUsername, demoPassword, false, newUsername, newPassword, false);

                                }
                                catch (Exception exception)
                                {
                                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "Failed to create Azure database. username=" + newUsername);
                                    return new Dictionary<string, object>() { { "Success", false }, { "Message", "Server is busy, Please try again later." } };
                                }
                            }
                        }
                        else
                        {
                            templateConnectionString = CreateDatabase(server, catalog, demoUsername, demoPassword, source, template);
                            CreateDatabaseUser(server, catalog, demoUsername, demoPassword, false, newUsername, newPassword, false);
                        }
                    }
                }
            }

            if (!isAzureDemo && !isBlankDababase && !isRdsBlank)
            {
                try
                {
                    ValidateConnection(server, catalog, newUsername, newPassword, sqlProduct, 3306, usingSsh, usingSsl, userId, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort);
                }
                catch (Exception exception)
                {
                    string cnnstr = GetConnection(server, catalog, null, username, "*****", null, sqlProduct, productPort, usingSsh, usingSsl);
                    TroubleshootInfo troubleshootInfo = ConnectionStringHelper.GetTroubleshootInfo(exception, server, catalog, username, password, usingSsh, sqlProduct, sshRemoteHost, sshUser, sshPassword, sshPort, productPort);
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, cnnstr + "\n\r" + "Troubleshoot Info Id = " + troubleshootInfo.Id);
                    SendError(1, exception, GetControllerNameForLog(this.ControllerContext), "CreateApp", Map.Logger, cnnstr + "\n\r" + "Troubleshoot Info Id = " + troubleshootInfo.Id);
                    //if(exception.InnerException is MySql.Data.MySqlClient.MySqlException)
                    //    return Json(new { Success = false, Message = "Could not connect. "+exception.InnerException.Message });
                    //return Json(new { Success = false, Message = "Could not connect. Please check the connection parameters and make sure the server is up and running." });
                    string message = exception.InnerException == null ? exception.Message : exception.InnerException.Message;
                    return new Dictionary<string, object>() { { "Success", false }, { "Message", message }, { "CnnString", cnnstr }, { "port", productPort }, { "TroubleshootInfo", troubleshootInfo } };
                }
            }

            if (!connectionId.HasValue && !isBlankDababase)
            {
                try
                {
                    connectionId = SaveConnection(server, catalog, newUsername, newPassword, userId, sqlProduct, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort);
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "fail to save connection string");

                    return new Dictionary<string, object>() { { "Success", false }, { "Message", exception.Message } };
                }
            }

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(view.GetFieldByColumnNames("Creator").Name, Map.Database.GetUserID().ToString());
            values.Add("Name", name);
            values.Add("Title", title);
            values.Add("Image", Durados.Database.LongProductName + ".png");

            if (!isBlankDababase)
            {
                values.Add("FK_durados_App_durados_DataSourceType_Parent", "2");
                values.Add("FK_durados_App_durados_SqlConnection_Parent", connectionId.Value.ToString());
            }
            else
            {
                values.Add("FK_durados_App_durados_DataSourceType_Parent", "1");
                values.Add("FK_durados_App_durados_SqlConnection_Parent", string.Empty);
            }
            values.Add("FK_durados_App_durados_SqlConnection_System_Parent", string.Empty);
            values.Add("FK_durados_App_durados_Template_Parent", string.Empty);
            values.Add("SpecificDOTNET", string.Empty);
            values.Add("SpecificJS", string.Empty);
            values.Add("SpecificCss", string.Empty);
            values.Add("Description", string.Empty);
            values.Add("TemplateFile", string.Empty);
            values.Add("FK_durados_App_durados_SqlConnection_Security_Parent", string.Empty);
            values.Add("Basic", basic);
            values.Add("FK_durados_App_durados_Theme_Parent", (themeId ?? Maps.DefaultThemeId).ToString());
            if (templateId.HasValue)
            {
                string templateIdFieldName = view.GetFieldByColumnNames("TemplateId").Name;
                if (values.ContainsKey(templateIdFieldName))
                    values[templateIdFieldName] = templateId.Value;
                else
                    values.Add(templateIdFieldName, templateId.Value);
            
            }

            System.Data.DataRow row = null;
            try
            {
                row = view.Create(values, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);
                if (isAzureDemo || isOnPremiseDemo)
                {
                    //string sourcePath = Maps.DemoUploadSourcePath;
                    //string targetPath = "/Uploads/" + row["Id"] + "/";
                    //try
                    //{
                    //    DirectoryCopy(sourcePath, targetPath, true);
                    //}
                    //catch (Exception exception) 
                    //{
                    //    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 3, "Could not copy uploads");

                    //}
                }

            }
            catch (System.Data.SqlClient.SqlException exception)
            {
                if (exception.Number == 2601)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 6, "App name already exists");
                    return new Dictionary<string, object>() { { "Success", false }, { "Message", "Application name already exists, please enter a different name." } };
                }
                else
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "failed to create app row");
                    SendError(1, exception, GetControllerNameForLog(this.ControllerContext), "CreateApp", Map.Logger);
                    return new Dictionary<string, object>() { { "Success", false }, { "Message", "Server is busy, please try again later" } };
                }
            }
            catch (Durados.Web.Mvc.Controllers.PlugInUserException exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 2, "failed to create app row");
                //SendError(1, exception, GetControllerNameForLog(this.ControllerContext), "CreateApp", Map.Logger);
                return new Dictionary<string, object>() { { "Success", false }, { "Message", exception.Message } };
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "failed to create app row");
                SendError(1, exception, GetControllerNameForLog(this.ControllerContext), "CreateApp", Map.Logger);
                return new Dictionary<string, object>() { { "Success", false }, { "Message", "Server is busy, please try again later" } };
            }

            if (isAzureDemo || isOnPremiseDemo)
            {
                try
                {
                    HandleTemplate(row, templateConnectionString);
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "failed to create northwind template");

                    return new Dictionary<string, object>() { { "Success", false }, { "Message", "Server is busy, please try again later" } };
                }

                if (isAzureDemo)
                {
                    if (!pendingExists)
                    {

                        bool inProcess = true;
                        int counter = 0;
                        while (inProcess && counter < 10)
                        {
                            try
                            {
                                counter++;
                                ValidateConnection(server, catalog, newUsername, newPassword);
                                System.Threading.Thread.Sleep(500);
                                inProcess = false;
                            }
                            catch
                            {
                                inProcess = true;
                            }
                        }

                        if (inProcess)
                        {
                            try
                            {
                                ValidateConnection(server, catalog, newUsername, newPassword);
                            }
                            catch (Exception exception)
                            {
                                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, null);

                            }
                        }

                        try
                        {
                            CreateDatabaseUser(server, catalog, demoUsername, demoPassword, false, newUsername, newPassword, false);

                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "Failed to create Azure database. username=" + newUsername);
                            return new Dictionary<string, object>() { { "Success", false }, { "Message", Database.GeneralErrorMessage } };
                        }
                    }
                }

            }
            var builder = new UriBuilder(System.Web.HttpContext.Current.Request.Url);
            string previewUrl = "http://" + name + Durados.Web.Mvc.Maps.UserPreviewUrl + GetThemePath(themeId);

            return new Dictionary<string, object>() { { "Success", true }, { "Url", Maps.GetAppUrl(name) }, { "previewUrl", previewUrl } };
        }
        #endregion create app

        [HttpPost]
        public virtual IHttpActionResult Test()
        {
            Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "API test connection started", string.Empty, 3, null, DateTime.Now);

            View view = GetView(AppViewName);
            if (view == null)
            {
                string message = string.Format(Messages.ViewNameNotFound, AppViewName);
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.NotFound.ToString(), 3, null, DateTime.Now);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, message));
            }



            string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

            Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "API Create App get params", json, 3, null, DateTime.Now);


            Dictionary<string, object> values = RestHelper.Deserialize(view, json);

            if (!values.ContainsKey(Product))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Test connection requires " + Product)));

            string template = values[Product].ToString();

            if (!values.ContainsKey("database"))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Test connection requires database")));

            string catalog = values["database"].ToString();

            if (!values.ContainsKey("username"))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Test connection requires username")));

            string username = values["username"].ToString();

            if (!values.ContainsKey("password"))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Test connection requires password")));

            string password = values["password"].ToString();
            bool usingSsh = false;
            if (values.ContainsKey("usingSsh"))
                usingSsh = Convert.ToBoolean(values["usingSsh"]);

            bool usingSsl = false;
            if (values.ContainsKey("usingSsl"))
                usingSsl = Convert.ToBoolean(values["usingSsl"]);

            string sshRemoteHost = null;
            if (values.ContainsKey("sshRemoteHost"))
                sshRemoteHost = values["sshRemoteHost"].ToString();

            string sshUser = null;
            if (values.ContainsKey("sshUser"))
                sshUser = values["sshUser"].ToString();

            string sshPassword = null;
            if (values.ContainsKey("sshPassword"))
                sshPassword = values["sshPassword"].ToString();

            string sshPrivateKey = null;
            if (values.ContainsKey("sshPrivateKey"))
                sshPrivateKey = values["sshPrivateKey"].ToString();

            int sshPort = 0;
            if (values.ContainsKey("sshPort"))
                sshPort = Convert.ToInt32(values["sshPort"]);


            object[] serverAndPort = GetProductPortAndServer(values);
            string server = (string)serverAndPort[0];
            int productPort = (int)serverAndPort[1];

            Durados.SqlProduct sqlProduct = Durados.SqlProduct.SqlServer;

            if (template == "1" || template == "3")
                sqlProduct = Durados.SqlProduct.SqlAzure;
            else if (template == "4")
                sqlProduct = Durados.SqlProduct.MySql;
            else if (template == "8")
                sqlProduct = Durados.SqlProduct.Postgre;
            else if (template == "7")
                sqlProduct = Durados.SqlProduct.Oracle;
            try
            {
                ValidateConnection(server, catalog, username, password, sqlProduct, 3306, usingSsh, usingSsl, null, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort);
            }
            catch (Exception exception)
            {
                string cnnstr = GetConnection(server, catalog, null, username, "*****", null, sqlProduct, productPort, usingSsh, usingSsl);
                TroubleshootInfo troubleshootInfo = ConnectionStringHelper.GetTroubleshootInfo(exception, server, catalog, username, password, usingSsh, sqlProduct, sshRemoteHost, sshUser, sshPassword, sshPort, productPort);
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, cnnstr + "\n\r" + "Troubleshoot Info Id = " + troubleshootInfo.Id);
                //if(exception.InnerException is MySql.Data.MySqlClient.MySqlException)
                //    return Json(new { Success = false, Message = "Could not connect. "+exception.InnerException.Message });
                //return Json(new { Success = false, Message = "Could not connect. Please check the connection parameters and make sure the server is up and running." });
                string message = exception.InnerException == null ? exception.Message : exception.InnerException.Message;
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, message));
            }

            return Ok();
        }

        protected virtual bool IsAllowed(View view)
        {
            return view.IsAllow();
        }

        
        public virtual IHttpActionResult Post(string id, bool force = false)
        {
            try
            {
                System.Web.HttpContext.Current.Items[Durados.Database.EnableDecryptionKey] = true;

                id = id.ToLower();
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "API Create App started", string.Empty, 3, null, DateTime.Now);

                View view = GetView(AppViewName);
                if (view == null)
                {
                    string message = string.Format(Messages.ViewNameNotFound, AppViewName);
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.NotFound.ToString(), 3, null, DateTime.Now);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, message));
                }

                if (string.IsNullOrEmpty(id))
                {
                    string message = Messages.IdIsMissing;
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.NotFound.ToString(), 3, null, DateTime.Now);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, message));
                }

                if (!IsAllowed(view))
                {
                    string message = Messages.ActionIsUnauthorized;
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.NotFound.ToString(), 3, null, DateTime.Now);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, message));
                }

                int? appId = Maps.Instance.AppExists(id, Convert.ToInt32(Maps.Instance.DuradosMap.Database.GetUserID()));
                if (!appId.HasValue)
                {
                    string message = string.Format(Messages.ItemWithIdNotFound, id, AppViewName);
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.NotFound.ToString(), 3, null, DateTime.Now);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, message));
                }

                var item = RestHelper.Get(view, appId.Value.ToString(), false, view_BeforeSelect, view_AfterSelect);

                if (item == null)
                {
                    string message = string.Format(Messages.ItemWithIdNotFound, id, AppViewName);
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.NotFound.ToString(), 3, null, DateTime.Now);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, message));
                }

                if (!item.ContainsKey("Name") || !item.ContainsKey("Title"))
                {
                    string message = string.Format(Messages.ItemWithIdNotFound, id, AppViewName);
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.NotFound.ToString(), 3, null, DateTime.Now);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, message));
                }

                string name = item["Name"].ToString();
                string title = item["Title"].ToString();


                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "API Create App get params", json, 3, null, DateTime.Now);


                Dictionary<string, object> values = RestHelper.Deserialize(view, json);

                string product = values[Product].ToString();
                string server = null;

                string catalog = null;
                if (values.ContainsKey("database"))
                    catalog = values["database"].ToString();

                string username = null;
                if (values.ContainsKey("username"))
                    username = values["username"].ToString();

                string password = null;
                if (values.ContainsKey("password"))
                    password = values["password"].ToString();

                bool usingSsh = false;
                if (values.ContainsKey("usingSsh"))
                    usingSsh = Convert.ToBoolean(values["usingSsh"]);

                bool usingSsl = false;
                if (values.ContainsKey("usingSsl"))
                    usingSsl = Convert.ToBoolean(values["usingSsl"]);

                string sshRemoteHost = null;
                if (values.ContainsKey("sshRemoteHost"))
                    sshRemoteHost = values["sshRemoteHost"].ToString();

                string sshUser = null;
                if (values.ContainsKey("sshUser"))
                    sshUser = values["sshUser"].ToString();

                string sshPassword = null;
                if (values.ContainsKey("sshPassword"))
                    sshPassword = values["sshPassword"].ToString();

                string sshPrivateKey = null;
                if (values.ContainsKey("sshPrivateKey"))
                    sshPrivateKey = values["sshPrivateKey"].ToString();

                int sshPort = 0;
                if (values.ContainsKey("sshPort"))
                    sshPort = Convert.ToInt32(values["sshPort"]);

                int? templateId = null;
                if (values.ContainsKey("templateId"))
                    templateId = Convert.ToInt32(values["templateId"]);

                object copyOptions = null;
                if (values.ContainsKey("copyOptions"))
                    copyOptions = values["copyOptions"];


                object[] serverAndPort = GetProductPortAndServer(values);
                server = (string)serverAndPort[0];
                int productPort = (int)serverAndPort[1];
                //10 mysql,postgre,dummy
                //10 mysql 11 postgre 12 dummy
                Dictionary<string, object> result;
                bool success = false;
                bool isNewDatabase = IsNewDatabase(product);
                bool? poolSuccess = false;
                var responseMessage = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK)); 
                if (isNewDatabase)
                {
                    int? appIdFromPool = null;
                    if (!IsSampleApp(values))
                        poolSuccess = new AppsPool().Pop(id, title, Maps.Instance.DuradosMap.Database.GetCurrentUsername(), out appIdFromPool, product, templateId, force);

                    for (int i = 0; i < 3; i++)
                    {
                        if (!poolSuccess.HasValue)
                        {
                            poolSuccess = new AppsPool().Pop(id, title, Maps.Instance.DuradosMap.Database.GetCurrentUsername(), out appIdFromPool, product, templateId, force);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (poolSuccess.Value)
                    {
                        responseMessage.Response.Headers.Add("pool", "true");
                        result = new Dictionary<string, object>() { { "Success", true } };
                    }
                    else
                    {
                        responseMessage.Response.Headers.Add("pool", "false");
                        SetTemplateToCache(values, id);
                        
                        string sampleApp = null;
                        if (values.ContainsKey("sampleApp"))
                            sampleApp = values["sampleApp"].ToString();

                        //string id, string product, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort, int? themeId)
                        // result =
                        CreateAppForNewDatabase(id, product, name, title, out server, out catalog, out username, out password, out productPort, sampleApp);
                        Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "return from CreateApp in console", HttpStatusCode.OK.ToString(), 3, null, DateTime.Now);
                        product = GetSqlProductFromTemplate(product);

                        result = CreateApp(product, name, title, server, catalog, username, password, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, null, templateId);
                    }
                }
                else
                {
                    result = CreateApp(product, name, title, server, catalog, username, password, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, null, templateId);
                }

                success = Convert.ToBoolean(result["Success"]);
                if (success)
                {
                    ProcessDatabase(appId.Value, id);
                    string message = "App " + id + " id: " + appId.Value + " connected";
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.OK.ToString(), 3, null, DateTime.Now);

                    //if (string.IsNullOrEmpty(username))
                    //{
                        username = Maps.Instance.DuradosMap.Database.GetCurrentUsername();
                    //}

                    if (Maps.Instance.DuradosMap.Database.GetUsernameById(Maps.PoolCreator.ToString()) != username)
                    {
                        Webhook webhook = new Webhook();
                        try
                        {
                            webhook.Send(WebhookType.AppCreated, GetBody(name, username));
                            Maps.Instance.DuradosMap.Logger.Log("webhook", "AppCreated", this.Request.Method.Method, null, 3, null, DateTime.Now);
                        }
                        catch (Exception exception)
                        {
                            webhook.HandleException(WebhookType.AppCreated, exception);
                        }
                    }
                    return responseMessage;
                }
                else
                {
                    string message = result["Message"].ToString();
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.ExpectationFailed.ToString(), 1, null, DateTime.Now);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, message));
                }
            }
            catch (InvalidSchemaException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, exception.Message));
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private void SetTemplateToCache(Dictionary<string, object> values, string id)
        {
            if (!map.AllKindOfCache.Contains(Durados.Database.CreateSchema))
            {
                map.AllKindOfCache[Durados.Database.CreateSchema] = new MemoryCache(Durados.Database.CreateSchema);
            }

            var CreateSchemaCache = (MemoryCache)map.AllKindOfCache[Durados.Database.CreateSchema];
            if (CreateSchemaCache.Contains(id))
            {
                CreateSchemaCache.Remove(id);
            }
                
            if (values.ContainsKey("templateId"))
            {
                CreateSchemaCache[id] = new Template() { TemplateType = TemplateType.TemplateId, Value = values["templateId"], CopyOptions = new CopyOptions(true) };
            }
            else if (values.ContainsKey("appId"))
            {
                object copyOptions = null;
                if (values.ContainsKey("copyOptions"))
                    copyOptions = values["copyOptions"];

                CreateSchemaCache[id] = new Template() { TemplateType = TemplateType.AppId, Value = values["appId"], CopyOptions = new CopyOptions(copyOptions) };
            }
            else if (values.ContainsKey("schema"))
            {
                object schema = values["schema"];
                //SchemaGenerator sg = new SchemaGenerator();

                //ValidateSchema(schema, id);
                Dictionary<string, object> transformResult = Transform("{newSchema: " + new JavaScriptSerializer().Serialize(schema) + ", severity: 0}", false);

                if (!transformResult.ContainsKey("alter"))
                {
                    //return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, Messages.InvalidSchema + ": " + GetWarnings(transformResult)));

                    throw new InvalidSchemaException(Messages.InvalidSchema + ": " + GetWarnings(transformResult));

                }
                //sg.Validate(Map, (IEnumerable<object>)schema);

                string schemaJson = new JavaScriptSerializer().Serialize(schema);

                CreateSchemaCache[id] = new Template() { TemplateType = TemplateType.Schema, Value = schemaJson };
            }

        }

        private bool IsSampleApp(Dictionary<string, object> values)
        {
            if (values.ContainsKey("sampleApp"))
            {
                string sampleApp = values["sampleApp"].ToString();
                return !(string.IsNullOrEmpty(sampleApp));
            }
            return false;
        }

        
        private static bool IsNewDatabase(string template)
        {
            int templateId;
            return int.TryParse(template, out templateId) && templateId >= 10;
        }


        public class SendAsyncErrorHandler : Durados.Web.Mvc.Infrastructure.ISendAsyncErrorHandler
        {
            int appId;
            public SendAsyncErrorHandler(int appId)
            {
                this.appId = appId;
            }
            public void HandleError(Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "CreateApp", exception.Source, exception, 1, null);

                string sql = "Update durados_App set DatabaseStatus = " + (int)OnBoardingStatus.Error + " where id = " + appId;
                Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

                sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql);

            }
        }
        private void ProcessDatabase(int appId, string appName)
        {
            //if (!System.Web.HttpContext.Current.Items.Contains("xxxzzzzzzzzz"))
            //    System.Web.HttpContext.Current.Items.Add("xxxzzzzzzzzz", appName);
            //Map map = Maps.Instance.GetMap(appName);
            try
            {
                HandleCreateSchemaIfExist(appName);
                //appId = Maps.Instance.AppExists(appName).Value;
                //UpdateDatabaseStatus(appId, OnBoardingStatus.Ready);
                //new Backand.socket().emitUsers("applicationReady", new { name = appName }, new string[] { "myUsername" });
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, exception.Message, exception.StackTrace, 1, null, DateTime.Now);

                UpdateDatabaseStatus(appId, OnBoardingStatus.Error);

            }
        }

        private void RespCallbackNew2(IAsyncResult ar)
        {
            // Get the RequestState object from the async result.
            Durados.Web.Mvc.Infrastructure.RequestState rs = (Durados.Web.Mvc.Infrastructure.RequestState)ar.AsyncState;

            // Get the WebRequest from RequestState.
            WebRequest req = rs.Request;

            string appName = Request.RequestUri.Segments.LastOrDefault().ToLower(); //req.RequestUri.Authority.Split('.')[0];
            int? appId = Maps.Instance.AppExists(appName);
            try
            {
                Template template = GetTemplateFromCache(appName);
                if (template.TemplateType == TemplateType.Schema)
                {
                    CallHttpRequestToCreateTheSchema(appName, template.Value.ToString());
                }
                else if (template.TemplateType == TemplateType.TemplateId)
                {
                    int templateId = (int)template.Value;
                    int sourceAppId = GetAppId(templateId);
                    CopyApp(sourceAppId, appId.Value, template.CopyOptions);
                }
                else if (template.TemplateType == TemplateType.AppId)
                {
                    int sourceAppId = (int)template.Value;
                    CopyApp(sourceAppId, appId.Value, template.CopyOptions);
                }
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, exception.Message, exception.StackTrace, 1, null, DateTime.Now);

                UpdateDatabaseStatus(appId.Value, OnBoardingStatus.Error);

            }

        }

        private void CopyApp(int sourceAppId, int targetAppId, CopyOptions copyOptions)
        {
            Cloner cloner = new Cloner();
            cloner.Clone(sourceAppId, targetAppId, copyOptions);
        }

        private int GetAppId(int templateId)
        {
            string sql =
                "select AppId from durados_Template with(NOLOCK) where id = " + templateId;
            string scalar = new SqlAccess().ExecuteScalar(Maps.Instance.DuradosMap.connectionString, sql);
            if (!string.IsNullOrEmpty(scalar))
            {
                return Convert.ToInt32(scalar);
            }
            throw new Durados.DuradosException("Could not find app for template: " + templateId);
        }

        private void RespCallbackNew(IAsyncResult ar)
        {
            // Get the RequestState object from the async result.
            Durados.Web.Mvc.Infrastructure.RequestState rs = (Durados.Web.Mvc.Infrastructure.RequestState)ar.AsyncState;

            // Get the WebRequest from RequestState.
            WebRequest req = rs.Request;

            string appName = Request.RequestUri.Segments.LastOrDefault().ToLower(); //req.RequestUri.Authority.Split('.')[0];
            int? appId = Maps.Instance.AppExists(appName);
            try
            {
                UpdateDatabaseStatus(appId.Value, OnBoardingStatus.Ready);
                //new Backand.socket().emitUsers("applicationReady", new { name = appName }, new string[] { "myUsername" });
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, exception.Message, exception.StackTrace, 1, null, DateTime.Now);

                UpdateDatabaseStatus(appId.Value, OnBoardingStatus.Error);

            }

        }

        //private void RespCallback(IAsyncResult ar)
        //{
        //    // Get the RequestState object from the async result.
        //    Durados.Web.Mvc.Infrastructure.RequestState rs = (Durados.Web.Mvc.Infrastructure.RequestState)ar.AsyncState;

        //    // Get the WebRequest from RequestState.
        //    WebRequest req = rs.Request;

        //    string appName = req.RequestUri.Authority.Split('.')[0].ToLower();
        //    int? appId = Maps.Instance.AppExists(appName);
        //    try
        //    {
        //        HandleCreateSchemaIfExist(appName);
        //        UpdateDatabaseStatus(appId.Value, OnBoardingStatus.Ready);
        //        //new Backand.socket().emitUsers("applicationReady", new { name = appName }, new string[] { "myUsername" });
        //    }
        //    catch (Exception exception)
        //    {
        //        Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, exception.Message, exception.StackTrace, 1, null, DateTime.Now);

        //        UpdateDatabaseStatus(appId.Value, OnBoardingStatus.Error);

        //    }

        //}

        private void HandleCreateSchemaIfExist(string appName)
        {
            Template template = GetTemplateFromCache(appName);
            if (template == null)
            {
                CallHttpRequestToConnectExisting(appName);
                return;
            }
            if (!string.IsNullOrEmpty(appName))
            {
                //CallHttpRequestToCreateTheSchema(appName, json);
                CallHttpRequestToCreateTheSchema2(appName);
            }
        }

        private void CallHttpRequestToCreateTheSchema2(string appName)
        {
            string url = GetUrlToConnect(appName);

            int appId = Maps.Instance.AppExists(appName).Value;
            Dictionary<string, string> headers = GetHeaders(appName);
            AsyncCallback asyncCallback = new AsyncCallback(RespCallbackNew2);
            try
            {
                Durados.Web.Mvc.Infrastructure.Http.AsyncWebRequest(url, new SendAsyncErrorHandler(appId), asyncCallback, headers);
            }
            catch(Exception exception)
            {
                try
                {
                    UpdateDatabaseStatus(appId, OnBoardingStatus.Error);
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, null);
                }
                catch { }
            }
        }

        private void CallHttpRequestToConnectExisting(string appName)
        {
            string url = GetUrlToConnect(appName);

            int appId = Maps.Instance.AppExists(appName).Value;
            Dictionary<string, string> headers = GetHeaders(appName);
            AsyncCallback asyncCallback = new AsyncCallback(RespCallbackNew);
            try
            {
                Durados.Web.Mvc.Infrastructure.Http.AsyncWebRequest(url, new SendAsyncErrorHandler(appId), asyncCallback, headers);
            }
            catch (Exception exception)
            {
                try
                {
                    UpdateDatabaseStatus(appId, OnBoardingStatus.Error);
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, null);
                }
                catch { }
            }
        }

        private string GetUrlToConnect(string appName)
        {
            return Maps.LocalAddress + "/admin/myAppConnection/status/" + appName;
        }

        private void CallHttpRequestToCreateTheSchema(string appName, string json)
        {
            json = "{newSchema: " + json + ", severity: 0}";


            string url = GetUrl();
            Dictionary<string, string> headers = GetHeaders(appName);
            //string response = Durados.Web.Mvc.Infrastructure.Http.WebRequestingJson(url, json, headers);
            //Dictionary<string, object> ret = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
            //return ret;

            int appId = Maps.Instance.AppExists(appName).Value;
            AsyncCallback asyncCallback = new AsyncCallback(RespCallbackNew);
            try
            {
                Durados.Web.Mvc.Infrastructure.Http.AsyncPostWebRequest(url, json, headers, new SendAsyncErrorHandler(appId), asyncCallback);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CallHttpRequestToCreateTheSchema", exception.Source, exception, 1, url);

            }
        }

        private Dictionary<string, string> GetHeaders(string appName)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("Authorization", Request.Headers.GetValues("Authorization").FirstOrDefault());
            headers.Add("AppName", appName);

            return headers;
        }

        private string GetUrl()
        {
            return Maps.LocalAddress + "/1/model?firstTime=true";
        }


        private Template GetTemplateFromCache(string appName)
        {
            
            if (!Maps.Instance.DuradosMap.AllKindOfCache.Contains(Durados.Database.CreateSchema))
                return null;

            var CreateSchemaCache = (MemoryCache)Maps.Instance.DuradosMap.AllKindOfCache[Durados.Database.CreateSchema];

            if (CreateSchemaCache.Contains(appName))
            {
                return (Template)CreateSchemaCache[appName];
            }

            return null;
        }

        private void UpdateDatabaseStatus(int appId, OnBoardingStatus onBoardingStatus)
        {
            string sql = "Update durados_App set DatabaseStatus = " + (int)onBoardingStatus + " where id = " + appId;
            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();
            try
            {
                sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, null);

            }
        }


        private object[] GetProductPortAndServer(Dictionary<string, object> values)
        {
            int port = 0;
            string server = null;
            if (values.ContainsKey("server"))
            {
                string[] serverAndPort = values["server"].ToString().Split(GetDefaultProductPortSpliter(values).ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (serverAndPort.Length > 1)
                {
                    string portString = serverAndPort[serverAndPort.Length - 1];
                    if (Int32.TryParse(portString, out port))
                    {
                        server = string.Join(GetDefaultProductPortSpliter(values), serverAndPort, 0, serverAndPort.Length - 1);
                    }
                    else
                    {
                        server = values["server"].ToString();
                    }
                }
                else
                {
                    server = values["server"].ToString();
                }
            }


            if (port == 0)
                port = GetDefaultProductPort(values);

            return new object[2] { server, port };
        }

        private string GetDefaultProductPortSpliter(Dictionary<string, object> values)
        {
            int product = Convert.ToInt32(values[Product]);
            string spliter = ":";

            switch (product)
            {
                case 4:
                    spliter = ":";
                    break;
                case 1:
                case 2:
                    spliter = ";";
                    break;
                case 8:
                    spliter = ":";
                    break;
                case 7:
                    spliter = ":";
                    break;
                default:
                    spliter = ":";
                    break;
            }
            return spliter;
        }

        private int GetDefaultProductPort(Dictionary<string, object> values)
        {
            int product = Convert.ToInt32(values[Product]);
            Durados.SqlProduct sqlProduct = Durados.SqlProduct.MySql;
            switch (product)
            {
                case 4:
                    sqlProduct = Durados.SqlProduct.MySql;
                    break;
                case 1:
                case 2:
                    sqlProduct = Durados.SqlProduct.SqlServer;
                    break;
                case 8:
                    sqlProduct = Durados.SqlProduct.Postgre;
                    break;
                case 7:
                    sqlProduct = Durados.SqlProduct.Oracle;
                    break;
                default:
                    sqlProduct = Durados.SqlProduct.MySql;
                    break;
            }
            return GetDefaultProductPort(sqlProduct);
        }

        private int GetDefaultProductPort(Durados.SqlProduct sqlProduct)
        {
            switch (sqlProduct)
            {
                case Durados.SqlProduct.MySql:
                    return 3306;
                case Durados.SqlProduct.Postgre:
                    return 5432;
                case Durados.SqlProduct.Oracle:
                    return 1542;

                default:
                    return 1433;
            }
        }

        public virtual IHttpActionResult Put(string id)
        {
            try
            {
                System.Web.HttpContext.Current.Items[Durados.Database.EnableDecryptionKey] = true;

                id = id.ToLower();

                View view = GetView(AppViewName);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, AppViewName)));
                }

                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                if (!IsAllowed(view))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                int? appId = Maps.Instance.AppExists(id, Convert.ToInt32(Maps.Instance.DuradosMap.Database.GetUserID()));
                if (!appId.HasValue)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, AppViewName)));
                }

                var item = RestHelper.Get(view, appId.Value.ToString(), false, view_BeforeSelect, view_AfterSelect);

                if (item == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, AppViewName)));
                }

                object connectionId = item[view.GetFieldByColumnNames("SqlConnectionId").JsonName];

                if (connectionId == null || connectionId.Equals(string.Empty))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, connectionId, ConnectionViewName)));
                }

                view = GetView(ConnectionViewName);

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                Dictionary<string, object> values = RestHelper.Deserialize(view, json);

                string server = null;

                string catalog = null;
                if (values.ContainsKey("database"))
                    catalog = values["database"].ToString();


                if (values.ContainsKey(SshUses) && values[SshUses].Equals(string.Empty))
                {
                    values.Remove(SshUses);
                }

                if (values.ContainsKey(SslUses) && values[SslUses].Equals(string.Empty))
                {
                    values.Remove(SslUses);
                }

                string sqlProductFieldName = view.GetFieldByColumnNames("SqlProductId").JsonName;
                if (!values.ContainsKey(sqlProductFieldName))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Missing " + sqlProductFieldName)));

                if (values.ContainsKey(Product))
                    values[Product] = values[sqlProductFieldName];
                else
                    values.Add(Product, values[sqlProductFieldName]);


                object[] serverAndPort = GetProductPortAndServer(values);
                server = (string)serverAndPort[0];
                int productPort = (int)serverAndPort[1];

                if (!values.ContainsKey("Catalog"))
                    values.Add("Catalog", catalog);

                if (values.ContainsKey("server") && !values.ContainsKey("ServerName"))
                    values.Add("ServerName", server);

                if (!values.ContainsKey("ProductPort"))
                    values.Add("ProductPort", productPort);

                view.Update(values, connectionId.ToString(), false, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                try
                {
                    Maps.Instance.Delete(id);
                }
                catch { }

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        protected override void AfterEditBeforeCommit(Durados.EditEventArgs e)
        {
            base.AfterEditBeforeCommit(e);

            ValidateConnectionString(e);

            try
            {
                UpdateProductCache(e);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "CreateApp", exception.Source, exception, 1, "UpdateProductCache exception");

            }
        }

        private void UpdateProductCache(Durados.EditEventArgs e)
        {
            int id = Convert.ToInt32(e.PrimaryKey);
            string sqlProductColumnName = "SqlProductId";
            string sqlProductFieldName = e.View.GetFieldByColumnNames(sqlProductColumnName).Name;
            Durados.SqlProduct sqlProduct = (Durados.SqlProduct)Enum.Parse(typeof(Durados.SqlProduct), e.Values.ContainsKey(sqlProductFieldName) ? e.Values[sqlProductFieldName].ToString() : Durados.SqlProduct.SqlServer.ToString());
            Durados.SqlProduct prevSqlProduct = (Durados.SqlProduct)Enum.Parse(typeof(Durados.SqlProduct), e.PrevRow.Table.Columns.Contains(sqlProductColumnName) ? e.PrevRow[sqlProductColumnName].ToString() : Durados.SqlProduct.SqlServer.ToString());

            if (prevSqlProduct != sqlProduct)
            {
                UpdateProductCache(id, sqlProduct, e.Command);
            }
        }

        private void UpdateProductCache(int id, Durados.SqlProduct sqlProduct, System.Data.IDbCommand command)
        {
            string[] apps = GetAppsName(id, command);

            foreach (string appName in apps)
            {
                Maps.UpdateSqlProduct(appName, sqlProduct);
            }
        }

        private string[] GetAppsName(int id, System.Data.IDbCommand command)
        {
            command.CommandText = "SELECT dbo.durados_App.Name FROM dbo.durados_App with(nolock) INNER JOIN dbo.durados_SqlConnection with(nolock) ON dbo.durados_App.SqlConnectionId = dbo.durados_SqlConnection.Id WHERE (dbo.durados_SqlConnection.Id = 1)";

            List<string> apps = new List<string>();

            System.Data.IDataReader reader = command.ExecuteReader();
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

        string SslUses = "SslUses";
        string SshPrivateKey = "SshPrivateKey";

        private void ValidateConnectionString(Durados.DataActionEventArgs e)
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
            Durados.SqlProduct sqlProduct = (Durados.SqlProduct)Enum.Parse(typeof(Durados.SqlProduct), e.Values.ContainsKey(sqlProductFieldName) ? e.Values[sqlProductFieldName].ToString() : Durados.SqlProduct.SqlServer.ToString());

            if (sqlProduct == Durados.SqlProduct.MySql && !usesSsh)
                localPort = e.Values.ContainsKey(ProductPortFieldName) ? Convert.ToInt32(e.Values[ProductPortFieldName]) : localPort;

            if (sqlProduct == Durados.SqlProduct.Postgre && !usesSsh)
                localPort = e.Values.ContainsKey(ProductPortFieldName) ? Convert.ToInt32(e.Values[ProductPortFieldName]) : localPort;

            if (sqlProduct == Durados.SqlProduct.Oracle)
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
            System.Data.IDbConnection connection = GetNewConnection(sqlProduct, connectionString);

            try
            {
                connection.Open();

            }
            catch (InvalidOperationException ex)
            {
                throw new Durados.DuradosException("Connection to Database Faild. Please check connection fields.", ex);
            }
            catch (System.Data.SqlClient.SqlException ex)
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
                throw new Durados.DuradosException(ex.Message, ex);
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
                throw new Durados.DuradosException(ex.Message, ex);
            }
            catch (Npgsql.NpgsqlException ex)
            {
                throw new Durados.DuradosException(ex.Message, ex);
            }

            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                throw new Durados.DuradosException(ex.Message, ex);
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

        private void OpenSshSessionIfNecessary(Durados.DataActionEventArgs e)
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

                session = new Durados.DataAccess.Ssh.ChilkatSession(tunnel, remotePort, localPort);
                session.Open();
            }
        }

        public IHttpActionResult Get(string id = null)
        {
            try
            {
                View view = GetView(AppViewName);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, AppViewName)));
                }

                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                if (!IsAllowed(view))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ViewIsUnauthorized));
                }

                int? appId = Maps.Instance.AppExists(id, Convert.ToInt32(Maps.Instance.DuradosMap.Database.GetUserID()));
                if (!appId.HasValue)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, AppViewName)));
                }

                var item = RestHelper.Get(view, appId.Value.ToString(), false, view_BeforeSelect, view_AfterSelect);

                if (item == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, AppViewName)));
                }

                object connectionId = item[view.GetFieldByColumnNames("SqlConnectionId").JsonName];

                if (connectionId == null || connectionId.Equals(string.Empty))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, connectionId, ConnectionViewName)));
                }

                view = GetView(ConnectionViewName);

                item = RestHelper.Get(view, connectionId.ToString(), false, view_BeforeSelect, view_AfterSelect);
                if (item == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, connectionId, ConnectionViewName)));
                }

                string sqlProductFieldName = view.GetFieldByColumnNames("SqlProductId").JsonName;
                if (!item.ContainsKey(sqlProductFieldName))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Missing " + sqlProductFieldName)));

                string server = item[ServernameFieldName].ToString();
                string productPort = item["ProductPort"].ToString();

                if (!(server.Contains(':') || server.Contains(',')) && !string.IsNullOrEmpty(productPort))
                {
                    item[ServernameFieldName] = server + ":" + productPort;
                }



                return Ok(item);


            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        [Route("status/{id}")]
        [HttpGet]
        public IHttpActionResult status(string id = null)
        {
            int? appId = Maps.Instance.AppExists(id, Convert.ToInt32(Maps.Instance.DuradosMap.Database.GetUserID()));
            if (!appId.HasValue)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.AppNotFound, id)));
            }
            return Ok(new { status = Maps.Instance.GetOnBoardingStatus(appId.Value.ToString()).ToString() });
        }





        [Route("getPassword/{id}")]
        [HttpGet]
        public IHttpActionResult getPassword(string id = null)
        {
            try
            {
                System.Web.HttpContext.Current.Items[Durados.Database.EnableDecryptionKey] = true;
                
                View view = GetView(AppViewName);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, AppViewName)));
                }

                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                if (!IsAllowed(view))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ViewIsUnauthorized));
                }

                int? appId = Maps.Instance.AppExists(id, Convert.ToInt32(Maps.Instance.DuradosMap.Database.GetUserID()));
                if (!appId.HasValue)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, AppViewName)));
                }

                var item = RestHelper.Get(view, appId.Value.ToString(), false, view_BeforeSelect, view_AfterSelect);

                if (item == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, AppViewName)));
                }

                object connectionId = item[view.GetFieldByColumnNames("SqlConnectionId").JsonName];

                if (connectionId == null || connectionId.Equals(string.Empty))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, ConnectionViewName)));
                }

                view = GetView(ConnectionViewName);
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("Id", "&&%&=&&%& " + connectionId.ToString());
                int rowCount = 0;
                System.Data.DataView dataView = view.FillPage(1, 2, values, false, null, out rowCount, view_BeforeSelect, view_AfterSelect);

                if (dataView == null || rowCount != 1)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, ConnectionViewName)));
                }


                return Ok(dataView[0]["Password"]);


            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private void ValidateConnection(string server, string catalog, string username, string password, Durados.SqlProduct sqlProduct, int localPort, bool usingSsh, bool usingSsl, string duradosUserId, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort)
        {
            ValidateConnectionString(false, server, catalog, username, password, usingSsh, usingSsl, duradosUserId, sqlProduct, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort);
        }


        public string GetConnection(string serverName, string catalog, bool? integratedSecurity, string username, string password, string duradosuserId, Durados.SqlProduct sqlProduct, int localPort, bool usesSsh, bool usesSsl)
        {

            string connectionString = null;
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = Map.connectionString;

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
                    connectionString = OracleAccess.GetConnectionStringSchema();


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

        protected virtual System.Data.IDbConnection GetNewConnection(Durados.SqlProduct sqlProduct, string connectionString)
        {
            return Durados.DataAccess.DataAccessObject.GetNewConnection(sqlProduct, connectionString);
        }

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
        string SshUses = "SshUses";
        //string ProductPort = "ProductPort";

        protected virtual void ValidateConnectionString(bool integratedSecurity, string serverName, string catalog, string username, string password, bool usesSsh, bool usesSsl, string duradosUserId, Durados.SqlProduct sqlProduct, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort)
        {
            OpenSshSessionIfNecessary(usesSsh, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, sqlProduct);

            int port = productPort;
            if (usesSsh)
                port = localPort;
            string connectionString = GetConnection(serverName, catalog, integratedSecurity, username, password, duradosUserId, sqlProduct, port, usesSsh, usesSsl);
            System.Data.IDbConnection connection = GetNewConnection(sqlProduct, connectionString);

            try
            {
                connection.Open();

            }
            catch (InvalidOperationException ex)
            {
                throw new Durados.DuradosException("Connection to Database Faild. Please check connection fields.", ex);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new Durados.DuradosException(ex.Message, ex);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Durados.DuradosException(ex.Message, ex);
            }
            catch (ExceedLengthException ex)
            {
                throw new Durados.DuradosException(ex.Message, ex);
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
        private void OpenSshSessionIfNecessary(bool usingSsh, string sshRemoteHost, string sshUser, string sshPassword, string privateKey, int sshPort, int productPort, Durados.SqlProduct product)
        {
            if (product == Durados.SqlProduct.MySql && usingSsh)
            {
                Durados.Security.Ssh.ITunnel tunnel = new Durados.DataAccess.Ssh.Tunnel();

                tunnel.RemoteHost = sshRemoteHost;
                tunnel.User = sshUser;
                tunnel.Password = sshPassword;
                tunnel.PrivateKey = privateKey;
                tunnel.Port = sshPort;
                int remotePort = productPort;
                localPort = Maps.Instance.AssignLocalPort();

                session = new Durados.DataAccess.Ssh.ChilkatSession(tunnel, remotePort, localPort);
                session.Open(15);
            }
        }

        protected void NotifyNewDatabase(string server, string catalog, string newUser, string newPassword, int creator, string previewPath)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            string subject = Map.Database.Localizer.Translate(CmsHelper.GetHtml("newRdsDatabaseSubject"));
            string message = Map.Database.Localizer.Translate(CmsHelper.GetHtml("newRdsDatabaseMessage"));

            string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;

            System.Data.DataRow row = Map.Database.GetUserRow(creator);

            message = message.Replace("[FirstName]", row["FirstName"].ToString());
            message = message.Replace("[LastName]", row["LastName"].ToString());
            message = message.Replace("[Url]", siteWithoutQueryString);
            message = message.Replace("[server]", server);
            message = message.Replace("[catalog]", catalog);
            message = message.Replace("[username]", newUser);
            message = message.Replace("[password]", newPassword);
            message = message.Replace("[Product]", Maps.Instance.DuradosMap.Database.SiteInfo.GetTitle());
            message = message.Replace("[UserPreviewUrl]", previewPath);

            string to = row["Email"].ToString();

            Durados.Cms.DataAccess.Email.Send(host, Map.Database.UseSmtpDefaultCredentials, port, username, password, false, to.Split(';'), new string[0], new string[1] { from }, subject, message, from, null, null, false, null, Map.Database.Logger);

        }
    }

    public class InvalidSchemaException : Durados.DuradosException
    {
        public InvalidSchemaException(string message)
            : base(message)
        {

        }
    }

    public class Template
    {
        public TemplateType TemplateType { get; set; }
        public object Value { get; set; }
        public CopyOptions CopyOptions { get; set; }

        
    }

    
    public enum TemplateType
    {
        Schema,
        TemplateId,
        AppId
    }
}
