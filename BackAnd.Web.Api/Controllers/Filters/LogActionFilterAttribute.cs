using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Net;
using System.Web.Http.Filters;
using Durados.Web.Mvc;
using Durados.Web.Mvc.Webhook;
using Durados.Web.Mvc.Controllers.Api;
using System.Threading.Tasks;

namespace BackAnd.Web.Api.Controllers.Filters
{
    public class LogActionFilterAttribute : ActionFilterAttribute  
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Log(actionExecutedContext.ActionContext, actionExecutedContext.Exception);
            base.OnActionExecuted(actionExecutedContext);
            try
            {
                if (!Maps.Instance.GetMap().IsMainMap)
                {
                    HandleHooks(actionExecutedContext);
                }
            }
            catch { }
        }

        async Task<string> GetResponseString(HttpActionExecutedContext actionExecutedContext)
        {
            var contents = await actionExecutedContext.Response.Content.ReadAsStringAsync();

            return contents;
        }

        object GetResponseObject(HttpActionExecutedContext actionExecutedContext)
        {
            Task<string> result = GetResponseString(actionExecutedContext);

            object responseObject = null;

            string json = result.Result;
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    responseObject = JsonConverter.Deserialize(json);
                }
                catch { }
            }

            return responseObject;
        }
        private void HandleHooks(HttpActionExecutedContext actionExecutedContext)
        {
            string webhookName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName + "." + actionExecutedContext.ActionContext.ActionDescriptor.ActionName;

            
            Webhook webhook = new Webhook();
            webhook.Send(webhookName, null, null, null, null, true, null, GetResponseObject, actionExecutedContext);
            //actionExecutedContext.Response.Content.
        }

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            Log(actionContext, null);
            base.OnActionExecuting(actionContext);
        }

        protected virtual void Log(System.Web.Http.Controllers.HttpActionContext actionContext, Exception exception)
        {
            try
            {
                apiController apiController = (apiController)actionContext.ControllerContext.Controller;

                Map map = apiController.Map;

                string exceptionSource = exception == null ? null : exception.Source;

                int logType = exception == null ? 3 : 1;

                string verb = actionContext.Request.Method.Method;

                string appName = string.Empty;
                try
                {
                    if (System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                        appName = System.Web.HttpContext.Current.Items[Database.AppName].ToString();
                }
                catch { }

                int? reqMilli = null;
                if (!apiController.started.HasValue)
                {
                    apiController.started = DateTime.Now;
                }
                else
                {
                    reqMilli = Convert.ToInt32(DateTime.Now.Subtract(apiController.started.Value).TotalMilliseconds);
                }

                UpdateRequestInfo(actionContext);
                map.Logger.Log(apiController.GetControllerNameForLog(apiController.ControllerContext), verb, appName + ": " + exceptionSource, exception , logType, actionContext.Request.RequestUri.ToString(), DateTime.Now, reqMilli);
            }
            catch (Exception e)
            {
                Durados.Diagnostics.EventViewer.WriteEvent(e);
            }
        }

        private void UpdateRequestInfo(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                const string ObjectRoute = "controller";
                const string NAME = "name";
                const string OBJECT = "Object";
                const string QUERY = "Query";
                const string ACTION = "Action";
                const string ObjectController = "viewData";
                const string QueryRoute = "1/query/data/{name}";
                const string ActionRoute = "1/objects/action/{viewName}/{id}";
                const string ViewName = "viewName";
                
                if (actionContext.RequestContext.RouteData.Values.ContainsKey(ObjectRoute))
                {
                    string controller = actionContext.RequestContext.RouteData.Values[ObjectRoute].ToString();
                    if (controller == ObjectController)
                    {
                        if (actionContext.RequestContext.RouteData.Values.ContainsKey(NAME))
                        {
                            string objectName = actionContext.RequestContext.RouteData.Values[NAME].ToString();
                            System.Web.HttpContext.Current.Items[Database.EntityType] = OBJECT;
                            System.Web.HttpContext.Current.Items[Database.ObjectName] = objectName;
                        }
                    }
                }
                else if (actionContext.RequestContext.RouteData.Route.RouteTemplate == QueryRoute)
                {
                    if (actionContext.RequestContext.RouteData.Values.ContainsKey(NAME))
                    {
                        string queryName = actionContext.RequestContext.RouteData.Values[NAME].ToString();
                        System.Web.HttpContext.Current.Items[Database.EntityType] = QUERY;
                        System.Web.HttpContext.Current.Items[Database.QueryName] = queryName;
                    }
                }
                else if (actionContext.RequestContext.RouteData.Route.RouteTemplate == ActionRoute)
                {
                    if (actionContext.ActionArguments.ContainsKey(NAME) && actionContext.ActionArguments.ContainsKey(ViewName))
                    {
                        string actionName = actionContext.ActionArguments[NAME].ToString();
                        string objectName = actionContext.ActionArguments[ViewName].ToString();
                        System.Web.HttpContext.Current.Items[Database.EntityType] = ACTION;
                        System.Web.HttpContext.Current.Items[Database.ActionName] = actionName;
                        System.Web.HttpContext.Current.Items[Database.ObjectName] = objectName;
                    }
                }
            }
            catch { }
        }
    }
}