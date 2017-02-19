using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Net;
using Durados.Web.Mvc;

namespace BackAnd.Web.Api.Controllers.Filters
{
    public class SocialAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private string queryStringProviderKey;
        private string queryStringAppNameKey;
        public SocialAuthorizeAttribute(string queryStringProviderKey = "provider", string queryStringAppNameKey = "appName")
            : base()
        {
            this.queryStringProviderKey = queryStringProviderKey;
            this.queryStringAppNameKey = queryStringAppNameKey;
        }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string appName = null;
            string provider = null;
                try
            {
                provider = GetProvider(actionContext.Request);
                appName = GetAppName(actionContext.Request);
                
                Map map = Maps.Instance.GetMap(appName);
                var providers = map.Database.GetSocialProviders();
                if (provider != null && providers.Contains(provider.ToLower()))
                {
                    return;
                }

                actionContext.Response = actionContext.Request.CreateErrorResponse(
                            HttpStatusCode.Unauthorized,
                            "The " + provider + " provider is unauthorized.");
            }
            catch (Exception exception)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                            HttpStatusCode.Unauthorized,
                            provider + " provider failed to authenticate in " + appName + ": " + exception.Message);
            }
        }

        protected virtual string GetAppName(HttpRequestMessage request)
        {
            return request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value,
                StringComparer.OrdinalIgnoreCase)[queryStringAppNameKey];
 

        }
        
        protected virtual string GetProvider(HttpRequestMessage request)
        {
            if (queryStringProviderKey != null)
                return GetProviderFromQueryString(request);
            else
                return null;

        }

        //private string GetProviderFromRoute(HttpRequestMessage request)
        //{
        //    var s = request.GetRouteData().Values["provider"];
        //    var segments = request.RequestUri.AbsolutePath.Split('/');

        //    for (int i = 0; i < segments.Length -1; i++)
        //    {
        //        if (segments[i + 1].ToLower().Equals(routeBeforeKey.ToLower()))
        //            return segments[i];
        //    }

        //    return null;
        //}

        private string GetProviderFromQueryString(HttpRequestMessage request)
        {
            return request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv=> kv.Value,
                            StringComparer.OrdinalIgnoreCase)[queryStringProviderKey];
        }
    }


    
}