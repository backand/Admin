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
        
        protected override View GetView(string viewName)
        {
            return (View)Maps.Instance.DuradosMap.Database.Views[viewName];
        }

        
        private void CreateAppForNewDatabase(string id,  string template, string name, string title, out string server, out string catalog, out string username, out string password, out int productPort, string sampleApp)
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
                newDbParameters = appFactory.GetNewExternalDBParameters(sqlProduct.Value, id, out  server, out port,sampleApp);//, out  catalog
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "start create new app in api", HttpStatusCode.OK.ToString(), 3, null, DateTime.Now);
           
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
       

       
        protected virtual Dictionary<string, object> CreateApp(string template, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort, int? themeId)
        {
            string id = GetTempGuid();
            //move to the next page
            string qstring = "id=" + id;

            string data = string.Format("&template={0}&name={1}&title={2}&server={3}&catalog={4}&username={5}&password={6}&usingSsh={7}&usingSsl={8}&sshRemoteHost={9}&sshUser={10}&sshPassword={11}&sshPrivateKey={12}&sshPort={13}&productPort={14}&themeId={11}", template, name, title, HttpUtility.UrlEncode(server), HttpUtility.UrlEncode(catalog), HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(password), usingSsh, usingSsl, HttpUtility.UrlEncode(sshRemoteHost), HttpUtility.UrlEncode(sshUser), HttpUtility.UrlEncode(sshPassword), sshPrivateKey, sshPort, productPort, themeId);
            string url = RestHelper.GetAppUrl(Maps.DuradosAppName, Maps.OldAdminHttp) + "/Website/CreateAppGet?" + qstring + data;

            Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "API Create App async call", url, 3, null, DateTime.Now);
                
            string response = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(url);
            Dictionary<string, object> json = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
            return json;
        }

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

            string template = values[Product].ToString();
            string catalog = values["database"].ToString();
            string username = values["username"].ToString();
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
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, cnnstr + "\n\r" + "Troubleshoot Info Id = " + troubleshootInfo.Id);
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

        public virtual IHttpActionResult Post(string id)
        {
            try
            {
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

                string template = values[Product].ToString();
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

                object[] serverAndPort = GetProductPortAndServer(values);
                server = (string)serverAndPort[0];
                int productPort = (int)serverAndPort[1];
                //10 mysql,postgre,dummy
                //10 mysql 11 postgre 12 dummy
                 Dictionary<string, object> result;
                 bool success = false;
                 bool isNewDatabase = IsNewDatabase(template);
                 if (isNewDatabase)
                 {
                     if (values.ContainsKey("schema"))
                     {
                         object schema = values["schema"];
                         SchemaGenerator sg = new SchemaGenerator();
                         sg.Validate(Map, (IEnumerable<object>)schema);

                         if (!map.AllKindOfCache.ContainsKey(Durados.Database.CreateSchema))
                         {
                             map.AllKindOfCache.Add(Durados.Database.CreateSchema, new Dictionary<string, object>());
                         }

                         string schemaJson = new JavaScriptSerializer().Serialize(schema);

                         map.AllKindOfCache[Durados.Database.CreateSchema].Add(id, schemaJson);
                     }

                     string sampleApp = null;
                     if (values.ContainsKey("sampleApp"))
                         sampleApp = values["sampleApp"].ToString();

                     //string id, string template, string name, string title, string server, string catalog, string username, string password, bool usingSsh, bool usingSsl, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort, int? themeId)
                     // result =
                     CreateAppForNewDatabase(id, template, name, title, out server, out catalog, out username, out password, out productPort,sampleApp);
                     Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "return from CreateApp in console", HttpStatusCode.OK.ToString(), 3, null, DateTime.Now);
                     template = GetSqlProductFromTemplate(template);

                     
                     
                 }
                 
                     result = CreateApp(template, name, title, server, catalog, username, password, usingSsh, usingSsl, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, null);
                 
                
                success = Convert.ToBoolean(result["Success"]);
                if (success)
                {
                    ProcessDatabase(appId.Value, id);
                    string message = "App " + id + " id: " + appId.Value + " connected";
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.OK.ToString(), 3, null, DateTime.Now);
                    return Ok();
                }
                else
                {
                    string message = result["Message"].ToString();
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, message, HttpStatusCode.ExpectationFailed.ToString(), 1, null, DateTime.Now);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, message));
            }
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
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
            //string id = GetTempGuid();
            string url = RestHelper.GetAppUrl(appName, Maps.OldAdminHttp);
            url += "?appName=" + appName;// +"&id=" + id;
            AsyncCallback asyncCallback = new AsyncCallback(RespCallback);

            Durados.Web.Mvc.Infrastructure.Http.AsyncWebRequest(url, new SendAsyncErrorHandler(appId), asyncCallback);
        }

        private void RespCallback(IAsyncResult ar)
        {
            // Get the RequestState object from the async result.
            Durados.Web.Mvc.Infrastructure.RequestState rs = (Durados.Web.Mvc.Infrastructure.RequestState)ar.AsyncState;

            // Get the WebRequest from RequestState.
            WebRequest req = rs.Request;

            string appName = req.RequestUri.Authority.Split('.')[0];
            int? appId = Maps.Instance.AppExists(appName);
            try
            {
                HandleCreateSchemaIfExist(appName);
                UpdateDatabaseStatus(appId.Value, OnBoardingStatus.Ready);
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, exception.Message, exception.StackTrace, 1, null, DateTime.Now);
                    
                UpdateDatabaseStatus(appId.Value, OnBoardingStatus.Error);

            }

        }

        private void HandleCreateSchemaIfExist(string appName)
        {
            string json = GetSchemaFromCache(appName);
            if (string.IsNullOrEmpty(json))
                return;

            if (!string.IsNullOrEmpty(appName))
            {
                CallHttpRequestToCreateTheSchema(appName, json);
            }
        }

        private Dictionary<string, object> CallHttpRequestToCreateTheSchema(string appName, string json)
        {
            string url = GetUrl();
            Dictionary<string, string> headers = GetHeaders(appName);
            string response = Durados.Web.Mvc.Infrastructure.Http.WebRequestingJson(url, json, headers);
            Dictionary<string, object> ret = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
            return ret;
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
            return this.Request.RequestUri.OriginalString.Split(new string[1] { "admin" }, StringSplitOptions.RemoveEmptyEntries)[0] + "1/table/config/template";
        }


        private string GetSchemaFromCache(string appName)
        {
            if (!Maps.Instance.DuradosMap.AllKindOfCache.ContainsKey(Durados.Database.CreateSchema))
                return null;

            if (Maps.Instance.DuradosMap.AllKindOfCache[Durados.Database.CreateSchema].ContainsKey(appName))
            {
                return Maps.Instance.DuradosMap.AllKindOfCache[Durados.Database.CreateSchema][appName].ToString();
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
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                
            }
        }

        
        private object[] GetProductPortAndServer(Dictionary<string, object> values)
        {
            int port = 0;
            string server = null;
            if (values.ContainsKey("server"))
            {
                string[] serverAndPort = values["server"].ToString().Split(new char[1]{':'}, StringSplitOptions.RemoveEmptyEntries);
                if (serverAndPort.Length > 1)
                {
                    string portString = serverAndPort[serverAndPort.Length - 1];
                    if (Int32.TryParse(portString, out port))
                    {
                        server = string.Join(":", serverAndPort, 0, serverAndPort.Length - 1);
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
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "UpdateProductCache exception");

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

                if (!server.Contains(':') && !string.IsNullOrEmpty(productPort))
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

        [Route("rdsResponse")]
        [HttpGet]
        public IHttpActionResult rdsResponse(string appguid, string appname, string endpoint, bool? success = null)
        {
            Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "rdsResponse",null,"start", null, 3, null,DateTime.Now);
            Guid guid;
            int id=0;
            int creator;
            if(string.IsNullOrEmpty(appguid) || !Guid.TryParse(appguid,out guid) || string.IsNullOrEmpty(appname)  )
            {
                Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "rdsResponse",null,"appGuid or app id  null or unformmatted",null, 3, null,DateTime.Now);
                return NotFound() ;
            }
            try
            {
                System.Data.DataRow row = Maps.Instance.GetAppRow(appname);
                id = Convert.ToInt32(row["Id"].ToString());
                if (id == 0)
                {
                    Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "rdsResponse", null, null, 1, "can not find row for appName");
                    return NotFound();
                }
                creator = Convert.ToInt32(row["Creator"]);
            }
            catch (Exception ex)
            {
                Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "rdsResponse", null, ex, 1, null);
                return NotFound();
            }
            try
            {
                if (success.HasValue && success.Value)
                {
                    int? connectionId;
                    int? port;
                    string serverName,catalog,dbUsername,dbPassword;
                    if (!IsValidConnection(endpoint, id, out connectionId, out serverName, out port, out catalog,out dbUsername,out  dbPassword))
                    {
                        Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "rdsResponse", null, null, 1, "Endpointvalidation failed");
                        return NotFound();
                    }
                    UpdateAppEndpoint(serverName,port.Value, connectionId);
                    //UpdateDatabaseStatus(id,OnBoardingStatus.Ready);
                    ProcessDatabase(id, appname);
                    Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "rdsResponse",null,"succes - call ProccesDatabse",null, 3, null,DateTime.Now);
                    if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["NotifyUserOnConsoleReady"] ?? "false"))
                    {
                        Map map = Maps.Instance.GetMap(appname);
                        NotifyNewDatabase(endpoint, catalog, dbUsername, dbPassword, creator, map.GetPreviewPath());
                    }
                    return Ok();
                }
                else
                {
                    UpdateDatabaseStatus(id, OnBoardingStatus.Error);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "rdsResponse",null,ex,1,null);

                return NotFound();
            }

           
        }

        private void UpdateAppEndpoint(string serverName,int port, int? connectioId)
        {
            if (connectioId.HasValue)
            {
                
                string sql = string.Format("UPDATE [dbo].[durados_SqlConnection]  SET [ServerName] = '{0}',[ProductPort]='{1}' WHERE Id={2}", serverName, port, connectioId);
                Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();
                try
                {
                    sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql);
                }
                catch(Exception ex)
                {
                    Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "UpdateAppEndpoint", null, ex, 1, null);
                    
                }
            }
        }

        private bool IsValidConnection(string endpoint, int id, out int? connectionId,out string serverName,out int? port,out string catalog,out string username, out string password)
        {
            connectionId = null;
            serverName = string.Empty;
            username = string.Empty;
            password = string.Empty;
            catalog = string.Empty;
            port = null;
            if (string.IsNullOrEmpty(endpoint))
                return false;
            MapDataSet.durados_AppRow appRow = Maps.Instance.GetAppRow(id);
            if(appRow==null)
            {
                Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "rdsResponse", "IsValidConnection", "no appRow for appId="+id.ToString(), null, 3, null, DateTime.Now);
                return false;
            }
            MapDataSet.durados_SqlConnectionRow sqlConnectioRow = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection;
            if (sqlConnectioRow == null)
            {
                Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "rdsResponse", "IsValidConnection", "no connectioRow for appId=" + id.ToString(), null, 3, null, DateTime.Now);
                return false;
            }
            connectionId = (int)sqlConnectioRow.Id;
            Durados.SqlProduct sqlProduct;
            
            
            bool isValid = true;
            try
            {
                GetConnectionParameters(sqlConnectioRow, out sqlProduct, out username, out password, out catalog);
                ExtractServerNameAndPort(endpoint, sqlProduct, out serverName, out port);
                ValidateConnectionString(false, serverName, catalog, username, password, false, false, appRow.Creator.ToString(), sqlProduct, null, null, null, null, 0, port.Value);
            }
            catch (Exception ex)
            {
                Maps.Instance.DuradosMap.Logger.Log("myAppConnection", "rdsResponse", "IsValidConnection", ex, 1, null);
                isValid = false;
            }
                return isValid;
        }

        private void GetConnectionParameters(MapDataSet.durados_SqlConnectionRow sqlConnectioRow, out Durados.SqlProduct sqlProduct, out string userName, out string password, out string catalog)
        {
            if (!sqlConnectioRow.IsSqlProductIdNull())
                sqlProduct = (Durados.SqlProduct)sqlConnectioRow.SqlProductId;
            else
                sqlProduct = Durados.SqlProduct.MySql;

            userName = sqlConnectioRow.Username;
            password = sqlConnectioRow.Password;
            catalog = sqlConnectioRow.Catalog;
        }

        private void ExtractServerNameAndPort(string endpoint,Durados.SqlProduct sqlProduct,out string serverName,out int? port)
        {
            port = null;
            int sIndex = endpoint.IndexOf(":");// serverUrl.substring( + 1);
            if (sIndex == -1)
            {
                serverName = endpoint;
                port = GetDefaultProductPort(sqlProduct);
            }

            else
            {
                serverName = endpoint.Substring(0, sIndex);
                int p;
                if (!int.TryParse(endpoint.Substring(sIndex + 1), out p))
                    p = GetDefaultProductPort(sqlProduct);
                else
                    port = p;
            }
        }


        [Route("getPassword/{id}")]
        [HttpGet]
        public IHttpActionResult getPassword(string id = null)
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
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, ConnectionViewName)));
                }

                view = GetView(ConnectionViewName);
                Dictionary<string,object> values = new Dictionary<string,object>();
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

        protected void NotifyNewDatabase(string server, string catalog, string newUser, string newPassword,int creator,string previewPath)
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

}
