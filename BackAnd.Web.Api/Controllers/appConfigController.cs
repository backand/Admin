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

using Durados.Web.Mvc;
using System.Net.Http.Headers;
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
    [RoutePrefix("1/app")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class appConfigController : apiController
    {
        
        const string AppView = "Database";

        #region config

        public IHttpActionResult Get(int? workspaceId = null)
        {
            try
            {
                View view = GetView(AppView);

                Durados.Workspace workspace = null;
                if (workspaceId.HasValue)
                {
                    workspace = Map.Database.GetWorkspace(workspaceId.Value);
                    if (workspace == null)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.WorkspaceNotFound));
                    }
                }
                else
                {
                    workspace = Map.Database.GetDefaultWorkspace();
                    if (workspace == null)
                        workspace = Map.Database.GetPublicWorkspace();
                }

                var item = RestHelper.GetApp(view, workspace, Map.Database, false);

                return Ok(item);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
                
            }
        }

        #endregion config

        [System.Web.Http.HttpGet]
        public HttpResponseMessage refresh(string appname, string appguid = null)
        {
            try
            {
                HttpResponseMessage response = null;
                if (!Maps.Instance.AppInCach(appname))
                {
                    response = Request.CreateResponse(System.Net.HttpStatusCode.OK, "App was not in cache", new TextPlainFormatter());
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-javascript");

                    RestHelper.Refresh(appname);

                    if (!string.IsNullOrEmpty(appname) && appname != Maps.DuradosAppName)
                    {
                        FarmCachingSingeltone.Instance.ClearMachinesCache(appname);
                    }
                    return response;
                }

                RestHelper.Refresh(appname);

                if (!string.IsNullOrEmpty(appname) && appname != Maps.DuradosAppName)
                {
                    FarmCachingSingeltone.Instance.ClearMachinesCache(appname);
                }

                response = Request.CreateResponse(System.Net.HttpStatusCode.OK, "App refreshed", new TextPlainFormatter());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
                return response; 
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        [System.Web.Http.HttpGet]
        [Route("reload")]
        public IHttpActionResult reload()
        {
            if (!IsAdmin())
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ActionIsUnauthorized));
            } 
            
            try
            {
                //RefreshConfigCache();
                string appName = System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString();
                RestHelper.Refresh(appName);

                if (!string.IsNullOrEmpty(appName) && appName != Maps.DuradosAppName)
                {
                    FarmCachingSingeltone.Instance.ClearMachinesCache(appName);
                }
                
                try
                {
                    new Sync().Initiate(Maps.Instance.GetMap(appName));
                    new Sync().Initiate(Maps.Instance.GetMap(appName));
                }
                catch { }
                
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "Reload " + appName, "", 3, null, DateTime.Now);

                return Ok();
            }
            catch (Exception exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message));
            }
        }

        [System.Web.Http.HttpGet]
        [Route("restore")]
        public IHttpActionResult restore(string version = null)
        {
            if (!IsAdmin())
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ActionIsUnauthorized));
            }

            try
            {
                //RefreshConfigCache();
                string appName = System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString();
                Maps.Instance.Restore(appName, version);
                RestHelper.Refresh(appName);
                try
                {
                    new Sync().Initiate(Maps.Instance.GetMap(appName));
                    new Sync().Initiate(Maps.Instance.GetMap(appName));
                }
                catch { }
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "Reload " + appName, "", 3, null, DateTime.Now);

                return Ok();
            }
            catch (Exception exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message));
            }
        }


        [System.Web.Http.HttpGet]
        [Route("versions")]
        public IHttpActionResult versions()
        {
            if (!IsAdmin())
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ActionIsUnauthorized));
            }

            string currentVersion = string.Empty;
            try
            {
                string appName = System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString();
                try
                {
                    Map map = Maps.Instance.GetMap(appName);
                    currentVersion = map.Database.ConfigVersion;
                }
                catch { }
                return Ok(new { versions = Maps.Instance.GetVersions(appName), current = currentVersion });
            }
            catch (Exception exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message));
            }
        }

        [System.Web.Http.HttpGet]
        [Route("dbStat")]
        public IHttpActionResult dbStat()
        {
            if (!IsAdmin())
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ActionIsUnauthorized));
            }

            return Ok(RestHelper.GetDbStat(Map.Database));
        }

        [System.Web.Http.HttpGet]
        [Route("sync")]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public IHttpActionResult sync()
        {
            if (!IsAdmin())
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ActionIsUnauthorized));
            }
            try
            {
                Dictionary<string,object> result = (new Sync()).AddNewViewsAndSyncAll(Map);

                RefreshConfigCache(Map);

                try
                {
                    RefreshOldAdmin(Map.AppName);
                }
                catch (Exception exception)
                {
                    Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, exception, 2, Map.AppName, DateTime.Now);
                }
                return Ok(result);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        [System.Web.Http.HttpPost]
        [Route("upload")]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize("Admin,Developer")]
        public IHttpActionResult upload()
        {
            if (!IsAdmin())
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ActionIsUnauthorized));
            }

            string appname = string.Empty;
            try
            {
                appname = System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString();
                if (appname == Maps.DuradosAppName)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, Messages.AppNameCannotBeNull));
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, Messages.AppNameCannotBeNull));
            }
            try
            {
                string jsonPost = Request.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> jsonPostDict = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(jsonPost);


                if (!jsonPostDict.ContainsKey("filename"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "You must send the filename parameter"));
                String filename = System.Web.HttpContext.Current.Server.UrlDecode((String)jsonPostDict["filename"]);
                if (!jsonPostDict.ContainsKey("filedata"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "You must send the filedata parameter"));

                XmlConfigHelper configHelper = new XmlConfigHelper();

                string version = configHelper.UploadConfigFromFile(appname, (String)jsonPostDict["filedata"]);

                return Ok(new { success = true, message = String.Format("Config Version {0} was succesfuly loaded to {1}", version, appname) });
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [System.Web.Http.HttpGet]
        [Route("download")]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize("Admin,Developer")]
        public HttpResponseMessage download(string version = null)
        {
            XmlConfigHelper configHelper = new XmlConfigHelper();

            string appname;
            try
            {
                appname = System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString();
                if (appname == Maps.DuradosAppName)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, Messages.AppNameCannotBeNull);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Messages.AppNameCannotBeNull);
            }



            System.IO.Stream s = configHelper.GetZipConfig(appname, version);
            HttpResponseMessage response;
            if (s == null)
                response = new HttpResponseMessage(HttpStatusCode.NotFound);

            response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(s);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = Map.AppName + ".zip";

            return response;

        }
        
    }

}
