﻿using System;
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
    public class BackAndAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        
        public const string UserRoleNotSufficient = "UserRoleNotSufficient";
        string _allowedRoles = null;
        public BackAndAuthorizeAttribute():base()
        {

        }
        public BackAndAuthorizeAttribute(string allowedRoles):base()
        {
            _allowedRoles = allowedRoles;
        }
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (IsServerAuthorized(actionContext))
                return;

            if (IsAnonymous(actionContext))
                return;

            base.OnAuthorization(actionContext);

            ClaimsPrincipal principal = actionContext.Request.GetRequestContext().Principal as ClaimsPrincipal;

            try
            {
                var usernameObj = principal.Claims.Where(c => c.Type == Database.Username).Single();


                var appnameObj = principal.Claims.Where(c => c.Type == Database.AppName).Single();

                if (usernameObj == null)
                {
                    HandleUnauthorized(actionContext);
                    return;
                }

                if (appnameObj == null)
                {
                    HandleUnauthorized(actionContext);
                    return;
                }

                string username = usernameObj.Value;
                string appname = appnameObj.Value;
                if (actionContext.Request.Headers.Contains("AppName"))
                {
                    appname = actionContext.Request.Headers.GetValues("AppName").FirstOrDefault();
                }

                if (!System.Web.HttpContext.Current.Items.Contains(Database.Username))
                    System.Web.HttpContext.Current.Items.Add(Database.Username, username);

                if (!System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                    System.Web.HttpContext.Current.Items.Add(Database.AppName, appname);

                if (!System.Web.HttpContext.Current.Items.Contains(Database.RequestId))
                    System.Web.HttpContext.Current.Items.Add(Database.RequestId, Guid.NewGuid().ToString());

                //if (actionContext.Request.Headers.Contains("AppName"))
                //{
                    Durados.Web.Mvc.Controllers.AccountMembershipService accountMembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();
                    try
                    {
                        //if (!IsAppReady()) 
                        //{
                        //    throw new Durados.DuradosException("App is not ready yet");
                        //}
                        if (!accountMembershipService.ValidateUser(username) || !accountMembershipService.IsApproved(username))
                        {
                            HandleUnauthorized(actionContext);
                            return;
                        }
                    }
                    catch (Exception exception)
                    {
                        actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                string.Format(Messages.Unexpected, exception.Message));

                        try
                        {
                            Maps.Instance.DuradosMap.Logger.Log(actionContext.ControllerContext.Controller.GetType().Name, actionContext.Request.RequestUri.ToString(), "BackAndAuthorizeAttribute", exception, 1, "authorization failure");
                        }
                        catch { }

                        try
                        {
                            if (Maps.Instance.AppInCach(appname))
                            {
                                Durados.Web.Mvc.UI.Helpers.RestHelper.Refresh(appname);
                            }



                        }
                        catch { }
                        return;
                    }
                //}
                if (_allowedRoles == "Admin")
                {
                    string userRole = Maps.Instance.GetMap(appname).Database.GetUserRole();
                    if (!(userRole == "Admin" || userRole == "Developer") )
                    {
                        actionContext.ActionArguments.Add(Database.backand_serverAuthorizationAttempt, true);
                        actionContext.ActionArguments.Add(UserRoleNotSufficient, true);
                     
                        HandleUnauthorized(actionContext);
                        return;
                    }
                }
                ((apiController)actionContext.ControllerContext.Controller).Init();
            }
            catch
            {
                HandleUnauthorized(actionContext);
                return;
            }
        }

        private bool IsAppReady()
        {
            string appName = Maps.GetCurrentAppName();

            if (appName == Maps.DuradosAppName)
                return true;

            if (!Maps.Instance.AppInCach(appName))
            {
                string appId = Maps.Instance.GetCurrentAppId();

                OnBoardingStatus status = Maps.Instance.GetOnBoardingStatus(appId);

                if (status != OnBoardingStatus.Ready)
                    return false;
            }

            return true;
        }

        private bool IsAnonymous(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (HasAccessToken(actionContext))
                return false;
            string anonymousToken = GetAnonymousToken(actionContext);
            if (string.IsNullOrEmpty(anonymousToken))
                return false;
            string appName = GetAppByToken(anonymousToken);
            if (string.IsNullOrEmpty(appName))
                return false;
            if (!IsAllowAnonymousAccess(appName))
                return false;

            string username = Durados.Database.GuestUsername;

            if (!System.Web.HttpContext.Current.Items.Contains(Database.Username))
                System.Web.HttpContext.Current.Items.Add(Database.Username, username);

            if (!System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                System.Web.HttpContext.Current.Items.Add(Database.AppName, appName);

            if (!System.Web.HttpContext.Current.Items.Contains(Database.RequestId))
                System.Web.HttpContext.Current.Items.Add(Database.RequestId, Guid.NewGuid().ToString());

            return true;
        }

        private bool IsAllowAnonymousAccess(string appName)
        {
            Map map = Maps.Instance.GetMap(appName);
            if (map == null || map.IsMainMap)
                return false;

            return map.Database.SecureLevel == Durados.SecureLevel.AllUsers;
        }

        private string GetAppByToken(string anonymousToken)
        {
            string sql = "SELECT [Name] FROM [durados_app] WITH(NOLOCK)  WHERE [AnonymousToken] =@AnonymousToken";
            using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(Maps.Instance.DuradosMap.Database.ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, cnn))
                {

                    command.Parameters.AddWithValue(Database.AnonymousToken, anonymousToken);
                    cnn.Open();
                    object scalar = command.ExecuteScalar();
                    if (scalar == null || scalar == DBNull.Value)
                        return null;
                    return scalar.ToString();
                }
            }
        }

        private string GetAnonymousToken(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Contains(Database.AnonymousToken))
            {
                return actionContext.Request.Headers.GetValues(Database.AnonymousToken).FirstOrDefault();
            }
            return null;
        }

        private bool HasAccessToken(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            return (actionContext.Request.Headers.Contains("Authorization"));
            
        }

        private bool IsServerAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (IsAction(3,"refresh"))
                return IsServerAuthorizedForRefresh(actionContext);

            if (IsAction(3,"rdsResponse"))
                return IsServerAuthorizedForRDScallback(actionContext);
            
            return false;
        }

        private bool IsServerAuthorizedForRDScallback(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string appGuid = null;
            if (!IsServerAuthorizationAttempt(out appGuid))
            {
                return false;
            }
            actionContext.ActionArguments.Add(Database.backand_serverAuthorizationAttempt, true);

            string appName = System.Web.HttpContext.Current.Request.QueryString[Database.AppName];

            if (string.IsNullOrEmpty(appName))
            {
                actionContext.ActionArguments.Add(Database.backand_appNameEmpty, true);
                return false;
            }

            string dbAppGuid = GetAppGuid(appName);
            
            if (!dbAppGuid.Equals(appGuid,StringComparison.CurrentCultureIgnoreCase))
            {
                actionContext.ActionArguments.Add(Database.backand_appGuidNotMatch, true);
                return false;
            }

            return true;
        }

        private string GetAppGuid(string appName)
        {
            
            string sql = "SELECT [Guid] FROM [durados_app] WITH(NOLOCK)  WHERE [Name] =@appName";
            using(System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(Maps.Instance.DuradosMap.Database.ConnectionString))
            {
                using(System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql,cnn))
                {

                    command.Parameters.AddWithValue("appName", appName);
                    cnn.Open();
                    object scalar = command.ExecuteScalar();
                    if (scalar == null || scalar == DBNull.Value)
                        return null;
                    return scalar.ToString();
                }
            }
        }

        private bool IsAction(int actionIndex,string actionName)
        {
            return System.Web.HttpContext.Current.Request.Url.ToString().Contains(actionName);
        }

       
    
        private bool IsServerAuthorizedForRefresh(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string appGuid = null;
            if (!IsServerAuthorizationAttempt(out appGuid))
            {
                return false;
            }
            actionContext.ActionArguments.Add(Database.backand_serverAuthorizationAttempt, true);

            string appName = System.Web.HttpContext.Current.Request.Url.Segments.Last();

            if (string.IsNullOrEmpty(appName))
            {
                actionContext.ActionArguments.Add(Database.backand_appNameEmpty, true);
                return false;
            }

            if (!Maps.Instance.AppInCach(appName))
            {
                actionContext.ActionArguments.Add(Database.backand_appNotInCache, true);
                return false;
            }

            Map map = Maps.Instance.GetMap(appName);
            if (map == null)
            {
                actionContext.ActionArguments.Add(Database.backand_appNotInCache, true);
                return false;
            }

            if (map.Guid.ToString() != appGuid)
            {
                actionContext.ActionArguments.Add(Database.backand_appGuidNotMatch, true);
                return false;
            }

            return true;
        }
        private bool IsServerAuthorizationAttempt(out string appGuid)
        {
            appGuid = System.Web.HttpContext.Current.Request.QueryString[Database.AppGuid];
            return (appGuid != null);
        }
       

        private void HandleUnauthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.ActionArguments.ContainsKey(Database.backand_serverAuthorizationAttempt))
            {
                if (actionContext.ActionArguments.ContainsKey(Database.backand_appNameEmpty))
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        new AppnameIsEmptyServerAuthorizationFailureException());
                }
                else if (actionContext.ActionArguments.ContainsKey(Database.backand_appNotInCache))
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        new AppNotInCacheServerAuthorizationFailureException());
                }
                else if (actionContext.ActionArguments.ContainsKey(Database.backand_appGuidNotMatch))
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        new AppGuidNotMatcheServerAuthorizationFailureException());
                    
                }
                else if (actionContext.ActionArguments.ContainsKey(UserRoleNotSufficient))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(
                        HttpStatusCode.Unauthorized,
                        "user role not sufficient for this operation");

                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        new UnknownReasonServerAuthorizationFailureException());
                }
            }
            else
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        "The user is unauthorized. Please try to login again or contact the system administrator.");
            }
        }
    }

    public class ServerAuthorizationFailureException : Durados.DuradosException
    {
        public ServerAuthorizationFailureException(string message) : base(message) { }
    }

    public class AppnameIsEmptyServerAuthorizationFailureException : ServerAuthorizationFailureException
    {
        public AppnameIsEmptyServerAuthorizationFailureException() : base("appname is empty") { }
    }

    public class AppNotInCacheServerAuthorizationFailureException : ServerAuthorizationFailureException
    {
        public AppNotInCacheServerAuthorizationFailureException() : base("app not in cache") { }
    }

    public class AppGuidNotMatcheServerAuthorizationFailureException : ServerAuthorizationFailureException
    {
        public AppGuidNotMatcheServerAuthorizationFailureException() : base("app guid not matche") { }
    }

    public class UnknownReasonServerAuthorizationFailureException : ServerAuthorizationFailureException
    {
        public UnknownReasonServerAuthorizationFailureException() : base("unknown reason") { }
    }

    public class MissingOrIncorrectSignUpToken : ServerAuthorizationFailureException
    {
        public MissingOrIncorrectSignUpToken() : base("Missing or incorrect SignUpToken") { }
    }

    public class MissingOrIncorrectMasterToken : ServerAuthorizationFailureException
    {
        public MissingOrIncorrectMasterToken() : base("Missing or incorrect master key") { }
    }
}