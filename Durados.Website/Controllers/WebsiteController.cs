using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.Web.Mvc;
using Durados.Web.Mvc.Controllers;
using Durados;
using Durados.DataAccess;
using System.Data;
using Durados.Web.Mvc.UI.Helpers;
using System.Data.SqlClient;
using System.IO;
using Durados.website.Helpers;

namespace Durados.Website.Controllers
{
    public class WebsiteController : MultiTenancyController
    {
        //Rds free database parameters
        
       
        NewDatabaseParameters newDbParameters;
        private bool IsTempAppBelongToCreator(CreateEventArgs e, out int? appId)
        {
            string appName = e.Values["Name"].ToString();
            int creator = Convert.ToInt32(GetUserID());
            appId = Maps.Instance.AppExists(appName, creator);
            return appId.HasValue;
        }

        int? tempAppId = null;
            
        protected override void BeforeCreate(CreateEventArgs e)
        {
            if (e.View.Name == "durados_App")
            {
                if (IsTempAppBelongToCreator(e, out tempAppId))
                {
                    string sqlDeleteTempApp = "delete durados_App where Id = " + tempAppId.Value;
                    e.Command.CommandText = sqlDeleteTempApp;
                    e.Command.ExecuteNonQuery();
                }
            }
            const string DatabaseStatus = "DatabaseStatus";
            e.Values.Add(DatabaseStatus, (int)OnBoardingStatus.Processing);

            base.BeforeCreate(e);
        }

        protected override void AfterCreateBeforeCommit(CreateEventArgs e)//dateConnectionStatus(connectionId.Value, json["status"]== "Ok" ? "Pending" : "Faild");
        { 
            if (e.View.Name != "durados_App" )
                return;
            if (newDbParameters != null)
            {
                if (string.IsNullOrEmpty(e.PrimaryKey))
                    throw new DuradosException("Failed to save new app.");
                string callbackUrl = string.Format("{0}/admin/myAppConnection/rdsResponse?appguid={1}&appname={2}", Maps.ApiUrls[0], GetNewAppGuid(e), e.Values["Name"].ToString());


                string url = System.Configuration.ConfigurationManager.AppSettings["nodeServicesUrl"] + "/createRDSInstance";
                ///{"instanceName":"yrvtest23","dbName":"yrvtest23","instanceClass":"db.t1.micro","storageSize":"5","IPRange":["0.0.0.0/32"],"engine":"MySQL","engineVersion":"5.6.21","username":"yariv","password":"123456789","region":"us-east-1","characterSetName":"ASCII","callbackUrl":"http://backand-dev3.cloudapp.net:4109/admin/myAppConnection/rdsResponse?appguid=86bec9ad-3319-423d-8125-9860ccd535c4&appname=test1&success=true","authToken":"123456789","securityGroup":"bknd-Allcustomers"}

                string postData = GetPostDataForCreateNewRdsDatabase(callbackUrl);
                Dictionary<string, object> json = new Dictionary<string, object>();

                try
                {
                    if (newDbParameters.Engine == RDSEngin.sqlserver_ee.ToString().Replace('_', '-'))
                    {
                         System.Threading.Tasks.Task.Run(() =>RunDummyCall(callbackUrl));
                        json.Add("status", null);
                    }
                    else
                    {
                        string response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(url,postData);
                        json = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
                    }
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Failed to create Amazon RDS database. username=" + Map.Database.GetUsernameById(Map.Database.GetUserID()));
                    throw new DuradosException("Server is busy, Please try again later.");
                }
                
            }

            base.AfterCreateBeforeCommit(e);
        }

        private string GetPostDataForCreateNewRdsDatabase( string callbackUrl)
        {
            
          //  string ipRange = System.Configuration.ConfigurationManager.AppSettings["rdsIpRange"] ?? "[0.0.0.0/32]";
            string authToken = System.Configuration.ConfigurationManager.AppSettings["nodeServicesAuth"] ?? GetMasterGuid();
            string securityGroup = System.Configuration.ConfigurationManager.AppSettings["AWSsecurityGroup"] ??"bknd-Allcustomer";
                
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
            System.Web.Script.Serialization.JavaScriptSerializer jsSerializer= new  System.Web.Script.Serialization.JavaScriptSerializer();
            
            string postData = jsSerializer.Serialize(dict);
            postData = postData.Replace("callbackUrlValue", callbackUrl);
            return postData; ;
        }

        private static int GetStorageSize(SqlProduct product)
        {
            return 5;
        }

        private static string GetInstanceDBClass()
        {
            return "db.t1.micro";
        }

       
        protected  string GetMasterGuid()
        {
            string currentUser = Maps.SuperDeveloper;
            System.Data.DataRow userRow = Maps.Instance.DuradosMap.Database.GetUserRow(currentUser);
            string guid = userRow["Guid"].ToString();
            return SecurityHelper.GetTmpUserGuidFromGuid(guid);

        }
        private void RunDummyCall(string callbackUrl)
        {
                string response = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(callbackUrl + "&success=true");
                Dictionary<string, object> json = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
        }

        private string GetNewAppGuid(CreateEventArgs e)
        {
            Guid guid;
            string sql = "Select [Guid] from durados_app with(nolock) where id=" + e.PrimaryKey;
            e.Command.CommandText = sql;

            object scalar = e.Command.ExecuteScalar();
            if (scalar == null || scalar == DBNull.Value || !Guid.TryParse(scalar.ToString(), out guid))
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "GetNewAppGuid", null, 1, "Failed to retrive guid for app id=" + e.PrimaryKey);
                throw new DuradosException("Failed to retrive guid for app id=" + e.PrimaryKey);
            }


            return guid.ToString();
        }


        private static bool IsNewDatabase(string template)
        {
            int templateId;
            return int.TryParse(template, out templateId) && templateId >= 10;
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult CreateAppGet2(string template, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort,string zone,string characterSetName ,string engine ,string engineVersion, int? themeId)
         {
            SqlProduct? product = Durados.Web.Mvc.UI.Helpers.RDSNewDatabaseFactory.GetSqlProductfromTemplate(template);
            newDbParameters = new NewDatabaseParameters();
            newDbParameters.Zone = zone;
            newDbParameters.Engine= engine;
            newDbParameters.EngineVersion = engineVersion;
            newDbParameters.CharacterSetName= characterSetName;
            newDbParameters.InstanceName = server;
            newDbParameters.DbName = catalog;
            newDbParameters.Username = username;
            newDbParameters.Password = password;
            newDbParameters.SqlProduct = product.HasValue?product.Value:SqlProduct.SqlServer;
            server = System.Configuration.ConfigurationManager.AppSettings["rdsCreatDbPendingMessage"] + "Not available yet";
            return CreateApp2(template, name, title, server, catalog, username, password, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, themeId);
        }
       
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult CreateAppGet(string template, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort, int? themeId)
        {
            return CreateApp2(template, name, title, server, catalog, username, password, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, themeId);
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CreateApp(string template, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort, int? themeId)
        {
            return CreateApp2(template, name, title, server, catalog, username, password, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, themeId);
        }

        private JsonResult CreateApp2(string template, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort, int? themeId)
        {
            Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "CreateApp", "started", "", 3, "server: " + server + ", database: " + catalog, DateTime.Now);
            
            SqlProduct sqlProduct = SqlProduct.SqlServer;

            if (template == "1" || template == "3")
                sqlProduct = SqlProduct.SqlAzure;
            else if (template == "4")
                sqlProduct = SqlProduct.MySql;
            else if (template == "8")
                sqlProduct = SqlProduct.Postgre;
            else if (template == "7")
                sqlProduct = SqlProduct.Oracle;

            if (sshPrivateKey != null)
                sshPrivateKey = sshPrivateKey.Replace(System.Environment.NewLine, string.Empty);
            string errors;
            if (!IsValidConnectionData(sqlProduct, name, title, server, catalog, username, password, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, out errors))
                return Json(new { Success = false, Message = errors });
            bool isBlankDababase = template.ToLower() == "5";
            bool isAzureDemo = template.ToLower() == "1";
            bool isOnPremiseDemo = template.ToLower() == "0";

            bool isRdsBlank = IsNewDatabase(template);
            if(isRdsBlank)
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
                newPassword = new AccountMembershipService().GetRandomPassword(12);
            string newUsername = username;

            if (string.IsNullOrEmpty(userId))
                return Json(new { Success = false, Message = "Please login or sign-up." });

            View view = GetView("durados_App");

            try
            {
                name = GetCleanName(name);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "app name exception");

                return Json(new { Success = false, Message = exception.Message });
            }

            int? connectionId = null;
            bool pendingExists = false;

            if (isAzureDemo || isOnPremiseDemo)
            {
                string source = Maps.DemoDatabaseName + Map.SourceSuffix;
                bool templateExists = Maps.Instance.AppExists(name).HasValue;
                if (templateExists)
                {
                    return Json(new { Success = true, Url = Maps.GetAppUrl(name) });
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
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);

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
                                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, "pending=" + pending + ";i=" + i);
                                    }
                                }
                                if (pendingExists)
                                {
                                    CreateDatabase(server, pending, demoUsername, demoPassword, source, template);
                                }
                            }
                            catch (Exception exception)
                            {
                                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);

                                pendingExists = false;
                            }

                            //}
                            //catch (Exception exception)
                            //{
                            //    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);

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
                                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Failed to create Azure database. username=" + newUsername);
                                    return Json(new { Success = false, Message = "Server is busy, Please try again later." });
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
                                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Failed to create Azure database. username=" + newUsername);
                                    return Json(new { Success = false, Message = "Server is busy, Please try again later." });
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
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, cnnstr + "\n\r" + "Troubleshoot Info Id = " + troubleshootInfo.Id);
                    SendError(1, exception, GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), Map.Logger, cnnstr + "\n\r" + "Troubleshoot Info Id = " + troubleshootInfo.Id);
                    //if(exception.InnerException is MySql.Data.MySqlClient.MySqlException)
                    //    return Json(new { Success = false, Message = "Could not connect. "+exception.InnerException.Message });
                    //return Json(new { Success = false, Message = "Could not connect. Please check the connection parameters and make sure the server is up and running." });
                    string message = exception.InnerException == null ? exception.Message : exception.InnerException.Message;
                    return Json(new { Success = false, Message = message, CnnString = cnnstr, port = productPort, TroubleshootInfo = troubleshootInfo });
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
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "fail to save connection string");

                    return Json(new { Success = false, Message = exception.Message });
                }
            }

            Dictionary<string, object> values = new Dictionary<string, object>();
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

            DataRow row = null;
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
                    //    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, "Could not copy uploads");

                    //}
                }

            }
            catch (SqlException exception)
            {
                if (exception.Number == 2601)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 6, "App name already exists");
                    return Json(new { Success = false, Message = "Application name already exists, please enter a different name." });
                }
                else
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "failed to create app row");
                    SendError(1, exception, GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), Map.Logger);
                    return Json(new { Success = false, Message = "Server is busy, please try again later" });
                }
            }
            catch (PlugInUserException exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 2, "failed to create app row");
                //SendError(1, exception, GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), Map.Logger);
                return Json(new { Success = false, Message = exception.Message });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "failed to create app row");
                SendError(1, exception, GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), Map.Logger);
                return Json(new { Success = false, Message = "Server is busy, please try again later" });
            }

            if (isAzureDemo || isOnPremiseDemo)
            {
                try
                {
                    HandleTemplate(row, templateConnectionString);
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "failed to create northwind template");

                    return Json(new { Success = false, Message = "Server is busy, please try again later" });
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
                                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                            }
                        }

                        try
                        {
                            CreateDatabaseUser(server, catalog, demoUsername, demoPassword, false, newUsername, newPassword, false);

                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Failed to create Azure database. username=" + newUsername);
                            return Json(new { Success = false, Message = Database.GeneralErrorMessage });
                        }
                    }
                }

            }
            var builder = new UriBuilder(Request.Url);
            string previewUrl = "http://" + name + Durados.Web.Mvc.Maps.UserPreviewUrl + GetThemePath(themeId);

            return Json(new { Success = true, Url = Maps.GetAppUrl(name), previewUrl = previewUrl });
        }

        //private void SetNewExternalDBParameters(out SqlProduct sqlProduct,  string template, out string server, out string catalog, out string username, out string password, out bool usingSsh, out bool usingSsl, out int productPort, out int? themeId)
        //{
        //   // set all the database parameters for the createapp
        //    SqlProduct product = GetProductfromTemplate(template);
        //     freeDbParameters = new RDSFreeDatabaseFactory().GetNewParameters(product);
        //    server = System.Configuration.ConfigurationManager.AppSettings["AWSServer"];//aws database server
        //    catalog = product == SqlProduct.SqlServer ? freeDbParameters.InstanceName : freeDbParameters.DbName;
        //    username = freeDbParameters.Username;
        //    password = freeDbParameters.Password;
        //    usingSsh =  false ;
        //    usingSsl = false;
        //    productPort = freeDbParameters.Port;
        //    sqlProduct = product;
        //    themeId = null;
                          
           
                    
        //}
        
        private string GetThemePath(int? themeId)
        {
            return Maps.Instance.GetTheme(themeId).RelativePath;
        }

        private bool IsValidConnectionData(SqlProduct product, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort,out string errors)
        {
            errors=string.Empty;
         

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

            if (productPort==0)
                errors += "<br>Port is required";
            if(product == SqlProduct.MySql &&  usingSsh)
            {
                if (string.IsNullOrEmpty(sshRemoteHost))
                    errors += "<br>SSH Remote Host is required";

                if(sshPort==0)
                    errors += "<br>SSH port is 0";
               
                if(string.IsNullOrEmpty(sshPrivateKey))
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

        private void ValidateConnection(string server, string catalog, string username, string password)
        {
            ValidateConnection(server, catalog, username, password, SqlProduct.SqlServer, 0, false, false, null, null, null, null, null, 0, 0);
        }

        private void ValidateConnection(string server, string catalog, string username, string password, SqlProduct sqlProduct, int localPort, bool usingSsh, bool usingSsl, string duradosUserId, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort)
        {
            //ConnectionStringHelper.ValidateConnectionString(server, catalog, username, password, false, Map.connectionString, Map.SqlProduct, Map.LocalPort, Map.UsingSsh);
            //ConnectionStringHelper.ValidateConnectionString(GetNewConnection(sqlProduct, GetConnection(server, catalog, false, username, password, null, sqlProduct, localPort, usingSsh)));
            ValidateConnectionString(false, server, catalog, username, password, usingSsh, usingSsl, duradosUserId, sqlProduct, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort);
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

        private void HandleTemplate(DataRow row, string connectionString)
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

            string employeePicture = Server.MapPath("~/Content/Images/pic.jpg");
            System.IO.File.Copy(employeePicture, directory + @"\" + "pic.jpg");

            try
            {
                CopyAzureContainer(Convert.ToInt32(id), oldConfigFileName);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Could not copy uploads");
            }

            //Maps.Instance.Restart(name);

        }

        private void CopyAzureContainer(int appId, string filePath)
        {
            string containerName = Maps.AzureAppPrefix + Maps.DemoUploadSourcePath;
            string newContainerName = Maps.AzureAppPrefix + appId;
            if (AzureHelper.DoesDefaultContainerExist(containerName))
                AzureHelper.GetDefaultContainerReference(Convert.ToInt32(Maps.DemoUploadSourcePath)).Duplicate(newContainerName);
            else if (AzureHelper.DoesContainerExist(AzureHelper.GetDefaultBlobClient(), "general"))
                AzureHelper.GetContainerReference(AzureHelper.GetDefaultBlobClient(), "general").Duplicate(newContainerName);


            FileInfo file = new FileInfo(filePath);
            string content = System.IO.File.ReadAllText(filePath);
            string oldAzureContainer = containerName + "</DirectoryVirtualPath>";
            string oldAzurePath = "<DirectoryBasePath>" + containerName + "</DirectoryBasePath>";
            string newAzurePath = "<DirectoryBasePath>" + newContainerName + "</DirectoryBasePath>";
            string oldGeneralAzureContainer = "general</DirectoryVirtualPath>";
            string newAzureContainer = newContainerName + "</DirectoryVirtualPath>";

            //Replace Azure Container
            content = content.Replace(oldAzureContainer, newAzureContainer);
            content = content.Replace(oldGeneralAzureContainer, newAzureContainer);
            content = content.Replace(oldAzurePath, newAzurePath);


            System.IO.File.WriteAllText(filePath, content);

        }


    }
}
