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

namespace BackAnd.Web.Api.Controllers.Filters
{
    public class ConfigBackupFilterAttribute : ActionFilterAttribute  
    {
        /// <summary>
        /// Backup app configuration <bold>sync</bold>
        /// Note: Function don't stop execution on failure
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                BackupConfig(actionContext);
            }
            catch (Exception exception)
            {
                Log(actionContext, exception);
            }
            base.OnActionExecuting(actionContext);
        }

        protected virtual void BackupConfig(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.ControllerContext.Controller is IHasMap)
               ((IHasMap)actionContext.ControllerContext.Controller).Map.BackupConfig();
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

                map.Logger.Log(apiController.GetControllerNameForLog(apiController.ControllerContext), verb, appName + ": " + exceptionSource, exception, logType, actionContext.Request.RequestUri.ToString());
            }
            catch (Exception e)
            {
                Durados.Diagnostics.EventViewer.WriteEvent(e);
            }
        }
    }
}