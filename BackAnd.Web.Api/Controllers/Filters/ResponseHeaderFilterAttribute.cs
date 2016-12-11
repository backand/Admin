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
    public class ResponseHeaderFilterAttribute : ActionFilterAttribute  
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                UpdateDebugResponseHeader(actionExecutedContext.ActionContext);
            }
            catch { }
            base.OnActionExecuted(actionExecutedContext);
        }

        protected virtual void UpdateDebugResponseHeader(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (System.Web.HttpContext.Current.Items.Contains(apiController.GuidKey))
            {
                string actionHeaderGuidValue = System.Web.HttpContext.Current.Items[apiController.GuidKey].ToString();
                if (!actionContext.Response.Headers.Contains(apiController.actionHeaderGuidName))
                    actionContext.Response.Headers.Add(apiController.actionHeaderGuidName, actionHeaderGuidValue);
            }
        }
    }
}