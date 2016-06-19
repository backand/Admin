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
                HandleHooks(actionExecutedContext);
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

                map.Logger.Log(apiController.GetControllerNameForLog(apiController.ControllerContext), verb, appName + ": " + exceptionSource, exception , logType, actionContext.Request.RequestUri.ToString(), DateTime.Now, reqMilli);
            }
            catch (Exception e)
            {
                Durados.Diagnostics.EventViewer.WriteEvent(e);
            }
        }
    }
}