using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Durados.Web.Mvc.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DynamicAuthorizeAttribute : AuthorizeAttribute
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

        public DynamicAuthorizeAttribute()
            : this(true, false)
        {
        }

        public DynamicAuthorizeAttribute(bool Authorize, bool guidValidation)
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

        private string GetUsernameFromQueryString(HttpContextBase httpContext)
        {
            string username = null;
            string id = httpContext.Request.QueryString["id"];
            string tmpid = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetTmpUserGuidFromGuid(id);
            if (!string.IsNullOrEmpty(tmpid))
                username = Map.Database.GetUsernameByGuid(tmpid);

            if (username == null)
                username = Maps.Instance.DuradosMap.Database.GetUsernameByGuid(tmpid);

            if (username == null)
                username = Map.Database.GetUsernameByGuid(Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetUserGuidFromTmpGuid(id));

            System.Web.HttpContext.Current.Items[Database.Username] = username;

            return username;
        }
        private bool IsConnectingDatabase(HttpContextBase httpContext)
        {
            string action =  ((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["action"].ToString();
            if (!(action == "CreateAppGet" || action == "CreateAppGet2" || action == "Restart" || action == "DeleteApp"))
                return false;

            return GetUsernameFromQueryString(httpContext) != null;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (IsConnectingDatabase(httpContext))
                return true;

            if (Map.Database.SecureLevel == SecureLevel.AllUsers && string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["id"]))
            {
                if (string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name))
                {
                    System.Data.DataRow userRow = Map.Database.GetUserRow();
                    if (userRow == null)
                    {
                        AccountMembershipService accountMembershipService = new AccountMembershipService();
                        string userId = accountMembershipService.RegisterGuest();

                      
                    }

                   
                 }
                return true;
            }

            if (!authorize)
                return true;

            if (httpContext.User.Identity is System.Security.Principal.WindowsIdentity && Maps.Skin == true)
            {
                bool userRegistered = Map.Database.GetUserRow() != null;
                if (Map.Database.SecureLevel == SecureLevel.AuthenticatedUsers)
                {
                    if (!userRegistered)
                    {
                        string email = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromError"]);

                        AccountMembershipService accountMembershipService = new AccountMembershipService();
                        accountMembershipService.RegisterGuest(new Durados.Web.Mvc.Controllers.AccountMembershipService.AuthenticatedUserInfo() { FirstName = httpContext.User.Identity.Name, LastName = httpContext.User.Identity.Name, Email = email }, httpContext.User.Identity.Name);
                    }
                    return true;
                }
                else if (Map.Database.SecureLevel == SecureLevel.RegisteredUsers)
                {
                    if (!userRegistered)
                    {
                        try
                        {
                            Map.Logger.Log(httpContext.Request.Url.PathAndQuery, "AuthorizeCore", httpContext.User.Identity.Name, null, 1, null);
                        }
                        catch { }
                    }
                    return userRegistered;
                }
            }

            if (guidValidation)// || httpContext.Request.Browser.Browser.ToLower() == "ie")
            {
                string id = httpContext.Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                {
                    string username = Map.Database.GetUsernameByGuid(id);
                    //if (id == System.Web.Configuration.WebConfigurationManager.AppSettings["code"])
                    if (username != null)
                    {
                        return true;
                    }
                }
            }

            bool b = base.AuthorizeCore(httpContext);

            if (!b)
            {
                try
                {
                    Map.Logger.Log(httpContext.Request.Url.PathAndQuery, "AuthorizeCore", httpContext.User.Identity.Name, null, 77, "Not authorized user: " + httpContext.User.Identity.Name + ". probably session endded");
                }
                catch { }
            }

            //if (!(Map is DuradosMap) && !string.IsNullOrEmpty(Map.securityConnectionString))
            //{
            //    //MapMembershipProvider provider = new MapMembershipProvider();
            //    System.Web.Security.MembershipUser user =(System.Web.Security.Membership.Provider.GetUser(System.Web.HttpContext.Current.User.Identity.Name, System.Web.HttpContext.Current.User.Identity.IsAuthenticated));
            //    if(user!=null)
            //        b=user.IsApproved && !user.IsLockedOut;

            //}
            Maps.Instance.DuradosMap.Logger.Log(((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["controller"].ToString() + " Authorization Filter", ((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + System.Web.HttpContext.Current.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString());
            
            return b;
        }
    }

    public class RequiresSSL : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ForceHttps(filterContext);
            base.OnActionExecuting(filterContext);
        }

        public bool ForceHttps(ActionExecutingContext filterContext)
        {
            HttpRequestBase req = filterContext.HttpContext.Request;
            HttpResponseBase res = filterContext.HttpContext.Response;

            const int DefaultPort = 80;
            const int SecuredPort = 443;

            //Check if we're secure or not and if we're on the local box
            //if (!req.IsSecureConnection && !req.IsLocal && Maps.UseSecureConnection)

            var builder = new UriBuilder(req.Url);
            bool redirect = false;

            if (!req.IsSecureConnection && Maps.UseSecureConnection)
            {
                if (builder.Scheme != Uri.UriSchemeHttps || builder.Port != SecuredPort)
                {
                    builder.Scheme = Uri.UriSchemeHttps;
                    builder.Port = SecuredPort;

                    redirect = true;
                }
            }

            SqlProduct sqlProduct = Maps.GetCurrentSqlProduct();
            if (Maps.SplitProducts && Maps.ProductsPort.ContainsKey(sqlProduct))
            {
                int port = Maps.ProductsPort[sqlProduct];
                if (builder.Port != port)
                {
                    builder.Port = port;
                    redirect = true;
                }
            }

            if (redirect)
            {
                string url = builder.Uri.ToString();
                if (req.IsLocal)
                {
                    string appName = Maps.GetCurrentAppName();
                    url = url.Replace("localhost", appName + "." + Maps.Host, false);
                }
                res.Redirect(url);
                return true;
            }
            return false;
        }
    }
    //public class RequiresAuthorizationAttribute : ActionFilterAttribute 
    //{     
    //    private readonly bool _authorize;      
    //    public RequiresAuthorizationAttribute()     
    //    {     	
    //        _authorize = true;     
    //    }      
    //    public RequiresAuthorizationAttribute(bool authorize)     
    //    {     	
    //        _authorize = authorize;     
    //    }      
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)     
    //    {     	
    //        var overridingAttributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof (RequiresAuthorizationAttribute), false);  
    //        if (overridingAttributes.Length > 0 && overridingAttributes[0] as RequiresAuthorizationAttribute != null && !((RequiresAuthorizationAttribute)overridingAttributes[0])._authorize)     		
    //            return;      	
    //        if (_authorize)     	
    //        {     		
    //            //redirect if not authenticated     		
    //            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)     		
    //            {     			//use the current url for the redirect     			
    //                var redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;
    //                //send them off to the login page     			
    //                //var redirectUrl = string.Format("?RedirectUrl={0}", redirectOnSuccess); 
    //                var loginUrl = LinkBuilder.BuildUrlFromExpression(filterContext.RequestContext, RouteTable.Routes, x => x.Login(redirectOnSuccess));     			
    //                filterContext.HttpContext.Response.Redirect(loginUrl, true);     		
    //            }     	
    //        }     
    //    } 
    //}
}
