using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Security.Claims;
//using System.Web.Http;
using System.Net;
using System.Web.Http.Filters;
using Durados.Web.Mvc;

using Durados.Web.Mvc.UI.Helpers;

namespace BackAnd.Web.Api.Controllers.Filters
{
    public class LogExceptionFilterAttribute : ExceptionFilterAttribute, IExceptionFilter  
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Log(actionExecutedContext);
            base.OnException(actionExecutedContext);
        }

        public override System.Threading.Tasks.Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, System.Threading.CancellationToken cancellationToken)
        {
            Log(actionExecutedContext);
            return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
        }

        protected virtual void Log(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                Exception exception = actionExecutedContext.Exception;

                apiController apiController = (apiController)actionExecutedContext.ActionContext.ControllerContext.Controller;

                Map map = apiController.Map;
                string appName = string.Empty;
                try
                {
                    if (System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                        appName = System.Web.HttpContext.Current.Items[Database.AppName].ToString();
                }
                catch { }

                HandleConnectionFailure(map, exception);

                map.Logger.Log(apiController.GetControllerNameForLog(apiController.ControllerContext), apiController.ControllerContext.RouteData.Values["action"].ToString(), appName + ": " + exception.Source, exception, 1, actionExecutedContext.Request.RequestUri.ToString());
            }
            catch (Exception e)
            {
                Durados.Diagnostics.EventViewer.WriteEvent(e);
            }
        }

        private void HandleConnectionFailure(Map map, Exception exception)
        {
            if (IsLoginFailureException(map, exception))
            {
                Maps.Instance.UpdateOnBoardingStatus(OnBoardingStatus.Error, map.Id);
            }
        }

        private bool IsLoginFailureException(Map map, Exception exception)
        {
            return ((Database) map.Database).IsLoginFailureException(exception);
        }
    }
}