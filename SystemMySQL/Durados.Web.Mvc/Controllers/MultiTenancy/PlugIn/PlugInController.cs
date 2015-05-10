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
using System.Net;
using Durados.Web.Mvc.UI.Json;

namespace Durados.Web.Mvc.Controllers
{
    public class PlugInController : BaseController
    {
        
        const string EmptyWidgetActionName = "EmptyWidget";

        public abstract class ParameterNames
        {
            public abstract List<string> GetNames();

            public string GetParameters(HttpRequestBase request)
            {
                string parameters = string.Empty;
                foreach (string name in GetNames())
                {
                    //if (!string.IsNullOrEmpty(request.QueryString["viewName"]))
                        parameters += name + "=" + request.QueryString[name] + "&";
                }

                parameters = parameters.TrimEnd('&');

                return parameters;
            }
        }

        public string CleanDeletedApps(int? limit)
        {
            if (!Map.IsMainMap)
                return "Access denied if not main app";

            if (!SecurityHelper.IsInRole("Developer"))
                return "Access denied for non developers";

            int l = limit.HasValue ? limit.Value : 100;

            try
            {
                int successes = (new Cleaner()).CleanDeletedApps(l);
                return successes + " Cleaned";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public ActionResult Settings()
        {
            string url = Url.Action("Settings2") + "?" + Request.QueryString.ToString();
            ViewData["url"] = url;
            ViewData["signOutUrl"] = Url.Action("SignOut") + "?" + GetParameters(); //map.Url + "/Account/LogOff";
            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", redirect: " + url);
            return View("~/Views/PlugIn/Open.aspx");
        }

        public ActionResult Settings2()
        {
            var jsonModel = new Settings() { parameters = GetParameters() };
            ViewData["isRegisteredUser"] = GetRegisteredUserId().HasValue; 
            ViewData["PlugIn"] = PlugInType;
            string id = GetIdFromParameters();
            DataRow instanceRow = Maps.Instance.DuradosMap.Database.GetSelectedInstanceRow(id, PlugInType);
            int? sampleAppId = null;
            if (instanceRow != null)
            {
                string SampleAppIdColumnName = "SampleAppId";
                sampleAppId = instanceRow.IsNull(SampleAppIdColumnName) ? null : (int?)instanceRow[SampleAppIdColumnName];
            }

            ViewData["isFree"] = IsFreePlan();
            ViewData["isSampleApp"] = sampleAppId.HasValue;
            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", sampleAppId: " + sampleAppId);
            return View("~/Views/Shared/Settings.aspx", jsonModel);
        }

        public string GetViewName()
        {
            var jsonModel = new Settings() { parameters = GetParameters() };
            string id = GetIdFromParameters();
            DataRow instanceRow = Maps.Instance.DuradosMap.Database.GetSelectedInstanceRow(id, PlugInType);
            string viewName = instanceRow["ViewName"].ToString(); ;
            return viewName;
        }
        protected void UpdatePlan(DataRow instanceRow)
        {
            if (instanceRow != null)
            {
                string SampleAppIdColumnName = "SampleAppId";
                int? sampleAppId = instanceRow.IsNull(SampleAppIdColumnName) ? null : (int?)instanceRow[SampleAppIdColumnName];
                int appId = (int)instanceRow["AppId"];
                int planId = GetPlanId();
                if (sampleAppId.HasValue)
                {
                    UpdatePlan(appId, planId);
                }
            }
        }

        protected Option[] GetApps(int userId)
        {
            List<Option> apps = new List<Option>();
            Dictionary<string,string> appsDictionary = ((DuradosMap)Maps.Instance.DuradosMap).GetUserApps(userId);
            foreach (string key in appsDictionary.Keys)
            {
                apps.Add(new Option() { Value = key, Text = appsDictionary[key] });
            }

            return apps.ToArray();
        }

        protected virtual string GetPlugInUserId()
        {
            throw new NotSupportedException();
        }

        protected virtual bool IsOwner()
        {
            return false;
        }

        protected virtual int GetPlanId()
        {
            return PlugInHelper.FreePlan;
        }

        protected virtual bool IsFreePlan()
        {
            return GetPlanId().Equals(PlugInHelper.FreePlan);
        }

        protected virtual void UpdatePlan(int appId, int planId)
        {
            Maps.Instance.DuradosMap.Database.UpdatePlan(appId, planId, GetSiteIdFromParameters());
        }

        protected string GetUserIdParameterForLogin(DataRow instanceRow, int appId)
        {
            //bool isSampleApp = !instanceRow.IsNull("SampleAppId");

            string paremeters = string.Empty;

            if (IsOwner())
            {
                string creatorGuid = GetCreatorGuid(appId);
                paremeters = "&id=" + creatorGuid;
            }

            return paremeters;
        }

        const string SampleAppIdInQueryString = "sid";
        public ActionResult Widget()
        {
            string id = GetIdFromParameters();

            PlugInType plugInType = PlugInType;

            //string userId = GetUserID();

            //if (string.IsNullOrEmpty(userId))
            //{
            //    userId = Maps.Instance.DuradosMap.Database.CreatePlugInUser(id, plugInType, GetPlugInUserGuid());
            //}
            
            //if (string.IsNullOrEmpty(userId))
            //    return RedirectToAction(EmptyWidgetActionName);

            DataRow instanceRow = Maps.Instance.DuradosMap.Database.GetSelectedInstanceRow(id, plugInType);
            if (instanceRow == null)
            {
                int? sampleAppId = null;
                if (string.IsNullOrEmpty( Request.QueryString[SampleAppIdInQueryString]))
                {
                    sampleAppId = Maps.Instance.DuradosMap.Database.GetFirstSampleId(plugInType);
                }
                else
                {
                    sampleAppId = (int?)Convert.ToInt32(Request.QueryString[SampleAppIdInQueryString]);
                }
                instanceRow = Maps.Instance.DuradosMap.Database.CreateSampleInstanceRow(id, plugInType, sampleAppId, GetPlugInUserId());
                if (instanceRow == null)
                {
                    new AppsGenerator().Generate(sampleAppId.Value, Maps.PlugInSampleGenerationCount);
                    instanceRow = Maps.Instance.DuradosMap.Database.CreateSampleInstanceRow(id, plugInType, sampleAppId, GetPlugInUserId());
                }
                if (instanceRow == null)
                {
                    Exception exception = new DuradosException("Could not create sample instance");
                    Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], exception, 78, "url: " + System.Web.HttpContext.Current.Request.Url.ToString());
                    return RedirectToAction(EmptyWidgetActionName);
                    
                }
            }

            //int appId = Maps.Instance.DuradosMap.Database.GetAppId(id, plugInType, instanceRow, userId); //(int)instanceRow["AppId"];
            int appId = (int)instanceRow["AppId"];

            string viewName = (string)instanceRow["ViewName"];

            string appName = Maps.Instance.GetAppRow(appId).Name;

            if (string.IsNullOrEmpty(appName))
            {
                Exception exception = new DuradosException("No app name for id:" + appId);
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], exception, 78, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", appName: " + appName + ", viewName: " + viewName);

                return RedirectToAction(EmptyWidgetActionName);
            }
            
            //bool isSampleApp = !instanceRow.IsNull("SampleAppId");

            //string paremeters = string.Empty;

            //if (isSampleApp)
            //{
            //    string creatorGuid = GetCreatorGuid(appId);
            //    paremeters = "&id=" + creatorGuid;
            //}
           
            Map map = Maps.Instance.GetMap(appName);
            if (!IsAuthorized(map))
            {
                Exception exception = new DuradosException("Not authorized for app " + appName);
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], exception, 78, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", appName: " + appName + ", viewName: " + viewName);
                return RedirectToAction(EmptyWidgetActionName);
            }

            Durados.View view = map.Database.Views.ContainsKey(viewName) ? map.Database.Views[viewName] : null;

            if (view == null || !IsAuthorized(view))
            {
                Exception exception = new DuradosException("Could not find view " + viewName + " check that configuration files exist");
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], exception, 78, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", appName: " + appName + ", viewName: " + viewName);
                return RedirectToAction(EmptyWidgetActionName);
            }

            string url = GetViewUrl(map, viewName) + GetPublicParameter();
            //string url = GetViewUrl(map, viewName) + GetUserIdParameterForLogin(instanceRow, appId) + GetPublicParameter();
            //if (!IsSignedIn(Maps.Instance.DuradosMap.Database.GetCreatorUsername(appId)))
            //{
            //    SignOut();
            //    SignIn(Maps.Instance.DuradosMap.Database.GetCreatorUsername(appId));
            //    return Redirect(url);
            //}

            UpdatePlan(instanceRow);

            ViewData["url"] = url;
            ViewData["signOutUrl"] = Url.Action("SignOut") + "?" + GetParameters(); //map.Url + "/Account/LogOff";
            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", redirect: " + url + ", appName: " + appName + ", viewName: " + viewName);
            return View();
            //return Redirect(url);
        }

        protected virtual string GetPublicParameter()
        {
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["public"]) && System.Web.HttpContext.Current.Request.QueryString["public"].Equals("true"))
                return "&public=true";
            else
                return string.Empty;
        }

        public virtual void Logoff()
        {
            Maps.Instance.DuradosMap.Database.Logoff(PlugInType, GetPlugInUserId());
        }

        public virtual void SignOut()
        {
            string id = string.Empty;
            try
            {
                id = GetIdFromParameters();
            }
            catch
            {
                return;
            }
            PlugInType plugInType = PlugInType;

            DataRow instanceRow = Maps.Instance.DuradosMap.Database.GetSelectedInstanceRow(id, plugInType);
            int appId = (int)instanceRow["AppId"];

            if (GetRegisteredUserId().HasValue && !IsSignedIn(GetRegisteredUsername()))
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["plugInSignOut"] ?? "true"))
                {
                    Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", appId: " + appId);
                    PlugInHelper.SignOut();
                }
            }
        }

        public virtual void RemoveWidget()
        {

        }

        //protected virtual void SignIn(string username)
        //{
        //    PlugInHelper.SignIn(username);
        //}

        protected virtual bool IsSignedIn(string username)
        {
            return System.Web.HttpContext.Current.User == null || System.Web.HttpContext.Current.User == null || System.Web.HttpContext.Current.User.Identity == null || string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name) || System.Web.HttpContext.Current.User.Identity.Name.Equals(username);
        }

        protected virtual string GetCreatorGuid(int appId)
        {
            return Maps.Instance.DuradosMap.Database.GetCreatorGuid(appId);
        }

        protected virtual int? GetRegisteredUserId()
        {
            //if (Maps.Instance.DuradosMap.Database.IsPlugInUser())
            //{
            //    int? userId = Maps.Instance.DuradosMap.Database.GetRegisteredUserId(GetPlugInUserId());
            //    if (userId.HasValue)
            //    {
            //        return userId;
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            //else
            //{
            //    return Convert.ToInt32(Maps.Instance.DuradosMap.Database.GetUserID());
            //}

            return Maps.Instance.DuradosMap.Database.GetRegisteredUserId(GetPlugInUserId());
        }

        protected virtual string GetRegisteredUsername()
        {
            int? userId = Maps.Instance.DuradosMap.Database.GetRegisteredUserId(GetPlugInUserId());
            if (userId.HasValue)
            {
                return Maps.Instance.DuradosMap.Database.GetUserRow(userId.Value)["Username"].ToString();
            }
            return null;
        }

        protected virtual string GetRegisteredUserGuid()
        {
            //if (Maps.Instance.DuradosMap.Database.IsPlugInUser())
            //{
            //    int? userId = Maps.Instance.DuradosMap.Database.GetRegisteredUserId(GetPlugInUserId());
            //    if (userId.HasValue)
            //    {
            //        return Maps.Instance.DuradosMap.Database.GetUserRow(userId.Value)["Guid"].ToString();
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            //else
            //{
            //    return Maps.Instance.DuradosMap.Database.GetUserGuid();
            //}

            int? userId = Maps.Instance.DuradosMap.Database.GetRegisteredUserId(GetPlugInUserId());
            if (userId.HasValue)
            {
                return Maps.Instance.DuradosMap.Database.GetUserRow(userId.Value)["Guid"].ToString();
            }
            return null;
        }

        protected virtual string GetIdFromParameters()
        {
            throw new NotImplementedException();
        }

        protected virtual string GetSiteIdFromParameters()
        {
            throw new NotImplementedException();
        }

        protected virtual string GetSiteUrlFromParameters()
        {
            throw new NotImplementedException();
        }

        protected virtual string GetSiteInfoFromParameters()
        {
            throw new NotImplementedException();
        }

        protected virtual bool IsAuthorized()
        {
            return true;
        }

        public JsonResult UpdateSiteInfo()
        {
            string siteId = GetSiteIdFromParameters();
            string siteInfo = GetSiteInfoFromParameters();
            string url = GetSiteUrlFromParameters();

            Maps.Instance.DuradosMap.Database.UpdateSiteInfo(siteId, siteInfo, url);

            return Json(true);
        }

        public JsonResult GetViews(int appId)
        {
            if (!IsAuthorized())
                return Json(string.Empty);

            string appName = Maps.Instance.GetAppRow(appId).Name;
            
            Map map = Maps.Instance.GetMap(appName);
            if (!IsAuthorized(map))
                return Json(string.Empty);

            return Json(map.Database.GetViewNameDisplayList());
        }


        public virtual PlugInType PlugInType
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public JsonResult GetSampleViews()
        {
            if (!IsOwner())
                return Json(string.Empty);

            string id = GetIdFromParameters();

            return Json(Maps.Instance.DuradosMap.Database.GetInstanceAndSamples(PlugInType, id, Request));
        }


        protected virtual int GetCurrentUserID(int appId)
        {
            int? registered = GetRegisteredUserId();
            if (registered.HasValue)
                return registered.Value;

            return Maps.Instance.DuradosMap.Database.GetCreator(appId).Value;
        }

        public void SaveNewInstance(int? appId, string viewName)
        {
             if (!IsOwner())
                throw new DuradosException("No authorization");

             string id = GetIdFromParameters();

            if (!appId.HasValue && !string.IsNullOrEmpty(Request.QueryString["appId"]))
                appId = (int?) Convert.ToInt32(Request.QueryString["appId"]);

            if (string.IsNullOrEmpty(viewName))
                viewName = Request.QueryString["viewName"];

            if (!string.IsNullOrEmpty(id) && appId.HasValue && !string.IsNullOrEmpty(viewName))
                Maps.Instance.DuradosMap.Database.SaveSelectedInstance(id, null, PlugInType, appId.Value, viewName, GetCurrentUserID(appId.Value), GetPlugInUserId());
        }

        public void SaveChangeInstance(int? sampleAppId)
        {
            if (!IsOwner())
                throw new DuradosException("No authorization");

            string id = GetIdFromParameters();

            if (!sampleAppId.HasValue && !string.IsNullOrEmpty(Request.QueryString["sampleAppId"]) && Request.QueryString["sampleAppId"] != "null")
            {
                try
                {
                    sampleAppId = (int?)Convert.ToInt32(Request.QueryString["sampleAppId"]);
                }
                catch { }
            }

            DataRow instanceRow = Maps.Instance.DuradosMap.Database.GetInstanceRow(id, sampleAppId);

            if (instanceRow == null )
            {
                instanceRow = Maps.Instance.DuradosMap.Database.CreateSampleInstanceRow(id, PlugInType, sampleAppId, GetPlugInUserId());
                
            }
            if (instanceRow == null)
            {
                new AppsGenerator().Generate(sampleAppId.Value, Maps.PlugInSampleGenerationCount);
                instanceRow = Maps.Instance.DuradosMap.Database.CreateSampleInstanceRow(id, PlugInType, sampleAppId, GetPlugInUserId());
            }
            if (instanceRow != null)
            {
                int appId = (int)instanceRow["AppId"];
                string viewName = (string)instanceRow["ViewName"];

                Maps.Instance.DuradosMap.Database.SaveSelectedInstance(id, sampleAppId, PlugInType, appId, viewName, GetCurrentUserID(appId), GetPlugInUserId());
            }
        }

        private string GetParameters()
        {
            ParameterNames parameterNames = GetParameterNames();

            return parameterNames.GetParameters(Request) + "&plugInType=" + ((int)PlugInType).ToString();
        }

        private string GetViewUrl(Map map, string viewName)
        {
            return GetViewUrl(map, viewName, "/Home/IndexPage/{0}?guid=guid_&mainPage=True&menu=off&plugIn=" + PlugInType.ToString());
        }

        private string GetViewUrl(Map map, string viewName, string templateUrl)
        {
            string parameters = GetParameters();

            string url = map.Url + string.Format(templateUrl, viewName);
            if (parameters != string.Empty)
                url += "&" + parameters;
            return url;// map.Url + "?instance=jmcRFbFGuM4-I5s9b53Pk83nKA__xDbsa7VW7udf1gY.eyJpbnN0YW5jZUlkIjoiMTJkOTczMDEtNjhiYy1mM2Y4LTJhYjgtMDBkMDY3MjRmYmU3Iiwic2lnbkRhdGUiOiIyMDEzLTAxLTE1VDAzOjQ2OjUyLjUxOC0wNjowMCIsInVpZCI6Ijk2YmJhZmE0LTA3NGUtNDJjMS05MjY3LWM3YmZjOTAzNDEyMCIsInBlcm1pc3Npb25zIjoiT1dORVIiLCJpcEFuZFBvcnQiOiIxOTIuMTE2Ljk1LjUyLzMwMDY3IiwidmVuZG9yUHJvZHVjdElkIjpudWxsLCJkZW1vTW9kZSI6ZmFsc2V9&width=618&locale=en&origCompId=TPWdgt2&compId=TPSttngsg";
        }

        private string GetAdminViewUrl(Map map, string appName, string viewName)
        {
            return GetAdminViewUrl(map, appName, viewName, "/Admin/Item/View?pk={0}&guid=guid&appName={1}&viewName={2}&plugIn={3}&menu=off");
        }

        private string GetViewPK(Map map, string viewName)
        {
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

            return configAccess.GetViewPK(viewName, map.GetConfigDatabase().ConnectionString);
        }

        private string GetAdminViewUrl(Map map, string appName, string viewName, string templateUrl)
        {
            string parameters = GetParameters();

            string url = map.Url + string.Format(templateUrl, GetViewPK(map, viewName), appName, viewName, PlugInType);
            if (parameters != string.Empty)
                url += "&" + parameters;
            return url;// map.Url + "?instance=jmcRFbFGuM4-I5s9b53Pk83nKA__xDbsa7VW7udf1gY.eyJpbnN0YW5jZUlkIjoiMTJkOTczMDEtNjhiYy1mM2Y4LTJhYjgtMDBkMDY3MjRmYmU3Iiwic2lnbkRhdGUiOiIyMDEzLTAxLTE1VDAzOjQ2OjUyLjUxOC0wNjowMCIsInVpZCI6Ijk2YmJhZmE0LTA3NGUtNDJjMS05MjY3LWM3YmZjOTAzNDEyMCIsInBlcm1pc3Npb25zIjoiT1dORVIiLCJpcEFuZFBvcnQiOiIxOTIuMTE2Ljk1LjUyLzMwMDY3IiwidmVuZG9yUHJvZHVjdElkIjpudWxsLCJkZW1vTW9kZSI6ZmFsc2V9&width=618&locale=en&origCompId=TPWdgt2&compId=TPSttngsg";
        }

        
        protected virtual ParameterNames GetParameterNames()
        {
            throw new NotSupportedException();
        }
        

        public ActionResult EmptyWidget()
        {
            return View();
        }

        protected virtual bool IsAuthorized(Durados.View view)
        {
            return true;
        }

        protected virtual bool IsAuthorized(Map map)
        {
            return true;
        }

        //protected virtual bool IsLoggedInToMain()
        //{
        //    return Maps.Instance.DuradosMap.Database.IsLoggedIn();
        //}

        public ActionResult Update()
        {
            string url = Url.Action("Update2") + "?" + Request.QueryString.ToString();
            ViewData["url"] = url;
            ViewData["signOutUrl"] = Url.Action("SignOut") + "?" + GetParameters(); //map.Url + "/Account/LogOff";
            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", redirect: " + url);
            return View("~/Views/PlugIn/Open.aspx");
        }

        public ActionResult Update2()
        {
            string id = GetIdFromParameters();

            if (!IsOwner())
                return RedirectToAction(EmptyWidgetActionName);

            DataRow instanceRow = Maps.Instance.DuradosMap.Database.GetSelectedInstanceRow(id, PlugInType);
            if (instanceRow == null)
                return RedirectToAction(EmptyWidgetActionName);

            int appId = (int)instanceRow["AppId"];

            
            string viewName = (string)instanceRow["ViewName"];

            string appName = Maps.Instance.GetAppRow(appId).Name;

            if (string.IsNullOrEmpty(appName))
                return RedirectToAction(EmptyWidgetActionName);

            Map map = Maps.Instance.GetMap(appName);
            if (!IsAuthorized(map))
                return RedirectToAction(EmptyWidgetActionName);


            ViewData["map"] = map;
            ViewData["viewName"] = viewName;
            ViewData["PlugIn"] = PlugInType;
            ViewData["id"] = GetCreatorGuid(appId);
            var jsonModel = new Settings() { parameters = GetParameters() + GetUserIdParameterForLogin(instanceRow, appId) };
            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", appName: " + appName + ", viewName: " + viewName);
            return View("~/Views/Shared/Update.aspx", jsonModel);
        }

        public ActionResult Connect()
        {
            string url = Url.Action("Select") + "?" + Request.QueryString.ToString();
            ViewData["url"] = url;
            ViewData["signOutUrl"] = Url.Action("SignOut") + "?" + GetParameters(); //map.Url + "/Account/LogOff";
            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", redirect: " + url);
            return View("~/Views/PlugIn/Open.aspx");
        }

        [Durados.Web.Mvc.Controllers.Attributes.PlugInDynamicAuthorizeAttribute()]
        public ActionResult Select()
        {
            string registeredUserGuid = GetRegisteredUserGuid();
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                if (string.IsNullOrEmpty(registeredUserGuid))
                {
                    return RedirectToAction("LogOn", "Account");
                }
                return Redirect(Request.Url.ToString() + "&id=" + registeredUserGuid);
            }
            string id = GetIdFromParameters();

            if (!IsOwner())
                return RedirectToAction(EmptyWidgetActionName);

            DataRow instanceRow = Maps.Instance.DuradosMap.Database.GetSelectedInstanceRow(id, PlugInType);
            if (instanceRow == null)
                return RedirectToAction(EmptyWidgetActionName);

            ViewData["PlugIn"] = PlugInType;
            var jsonModel = new Settings() { apps = GetApps(GetRegisteredUserId().Value), parameters = GetParameters() + "&id=" + id };
            int appId = (int)instanceRow["AppId"];
            //if (!IsSignedIn(Maps.Instance.DuradosMap.Database.GetCreatorUsername(appId)))
            //{
            //    SignOut();
            //    SignIn(Maps.Instance.DuradosMap.Database.GetCreatorUsername(appId));
            //    return Redirect(Request.Url.ToString() + "&id=" + registeredUserGuid);
            //}
            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", appId: " + appId);
            return View(jsonModel);
        }

        public ActionResult Design()
        {
            if (!IsOwner())
                return RedirectToAction(EmptyWidgetActionName);

            string id = GetIdFromParameters();

            DataRow instanceRow = Maps.Instance.DuradosMap.Database.GetSelectedInstanceRow(id, PlugInType);
            if (instanceRow == null)
                return RedirectToAction(EmptyWidgetActionName);

            int appId = (int)instanceRow["AppId"];

            string viewName = (string)instanceRow["ViewName"];

            string appName = Maps.Instance.GetAppRow(appId).Name;

            if (string.IsNullOrEmpty(appName))
                return RedirectToAction(EmptyWidgetActionName);

            Map map = Maps.Instance.GetMap(appName);
            if (!IsAuthorized(map))
                return RedirectToAction(EmptyWidgetActionName);

            string url = GetAdminViewUrl(map, appName, viewName) + GetUserIdParameterForLogin(instanceRow, appId);
            ViewData["url"] = url;
            ViewData["signOutUrl"] = Url.Action("SignOut") + "?" + GetParameters(); //map.Url + "/Account/LogOff";
            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", redirect: " + url + ", appName: " + appName + ", viewName: " + viewName);
            
            return View();
        }

        public virtual JsonResult GenerateDB(int sampleAppId, int count)
        {
            string id = GetIdFromParameters();
            new AppsGenerator().Generate(sampleAppId, count);
            return null;
        }

        public virtual ActionResult AfterRegistration(string username)
        {
            string id = this.Request.QueryString["id"];

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(id) && Maps.Instance.DuradosMap.Database.GetGuidByUsername(username) == id)
            {
                    
                System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, true);
                user.IsApproved = true;
                System.Web.Security.Membership.UpdateUser(user);
                PlugInHelper.SignIn(username);
            }
            return Redirect("/index.aspx");
        }
    }
}



