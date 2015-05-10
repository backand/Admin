using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PlugInDynamicAuthorizeAttribute : AuthorizeAttribute
    {
        private Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        private bool authorize;
        private bool guidValidation;

        public PlugInDynamicAuthorizeAttribute()
            : this(true, false)
        {
        }

        public PlugInDynamicAuthorizeAttribute(bool Authorize, bool guidValidation)
            : base()
        {
            this.authorize = Authorize;
            this.guidValidation = guidValidation;

            string roles = string.Empty;
            foreach (string r in System.Web.Security.Roles.GetAllRoles())
            {
                roles += r + ",";
            }

            roles = roles.TrimEnd(',');

            this.Roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string controllerName = ((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["controller"].ToString();
            Maps.Instance.DuradosMap.Logger.Log("Plugin Authorization Filter " + ((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["controller"].ToString(), ((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + System.Web.HttpContext.Current.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString());
            return Maps.Instance.DuradosMap.Database.GetRegisteredUserId(PlugInHelper.GetPlugInUserId(GetPlugInType(controllerName), httpContext.Request)).HasValue;
            //return GetPlugInSecurity(controllerName, httpContext).GetRegisteredUserId().HasValue;
        }

        protected virtual PlugInType GetPlugInType(string controllerName)
        {
            switch (controllerName)
            {
                case "WixPlugIn":
                    return PlugInType.Wix;

                default:
                    throw new NotImplementedException();
            }
        }


    }

    //public class PlugInSecurity
    //{
    //    protected HttpContextBase httpContext;
    //    public PlugInSecurity(HttpContextBase httpContext)
    //    {
    //        this.httpContext = httpContext;
    //    }

        
    //    public virtual int? GetRegisteredUserId()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class WixPlugInSecurity : PlugInSecurity
    //{
    //    public WixPlugInSecurity(HttpContextBase httpContext)
    //        : base(httpContext)
    //    {
    //    }

    //    protected string GetIdFromParameters()
    //    {
    //        WixInstance wixInstance = WixPlugInHelper.GetInstance(httpContext.Request.QueryString["instance"]);

    //        string compId = httpContext.Request.QueryString["origCompId"] ?? httpContext.Request.QueryString["compId"];

    //        return wixInstance.instanceId + compId;

    //    }

        

    //    public override int? GetRegisteredUserId()
    //    {
    //        return Maps.Instance.DuradosMap.Database.GetRegisteredUserId(WixPlugInHelper.GetInstance(httpContext.Request.QueryString["instance"]).uid.ToString());
    //    }
    //}
}
