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
    public class MasterKeyAuthorize : System.Web.Http.AuthorizeAttribute
    {
        public MasterKeyAuthorize(string key)
            : base()
        {
            Key = key;
        }
        public string Key { get; private set; }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization.Scheme.Equals(GetAuthorization()))
            {
                return;
            }

            actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        new MissingOrIncorrectMasterToken());

        }

        public string GetAuthorization()
        {
            return (System.Configuration.ConfigurationManager.AppSettings[Key] ?? "69F77115-495F-4C83-A9EC-0AA46714482E").ToString();
        }

        
    }


    
}