using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Net;
using Durados.Web.Mvc;
using Durados.Web.Mvc.Farm;
using Durados.Data;

namespace BackAnd.Web.Api.Controllers.Filters
{
    public class BackAndAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {

        public const string UserRoleNotSufficient = "UserRoleNotSufficient";
        string _allowedRoles = null;
        public BackAndAuthorizeAttribute()
            : base()
        {

        }
        public BackAndAuthorizeAttribute(string allowedRoles)
            : base()
        {
            _allowedRoles = allowedRoles;
        }
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (IsBasicAuthorized(actionContext))
            {
                ((apiController)actionContext.ControllerContext.Controller).Init();
                return;
            }

            if (IsServerAuthorized(actionContext))
                return;

            if (IsAnonymous(actionContext))
                return;

            base.OnAuthorization(actionContext);

            ClaimsPrincipal principal = actionContext.Request.GetRequestContext().Principal as ClaimsPrincipal;

            var ci = principal.Identity as ClaimsIdentity;
            if (!ci.IsAuthenticated)
            {
                AuthorizationTokenExpiredException authorizationTokenExpiredException = new AuthorizationTokenExpiredException();
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        authorizationTokenExpiredException.Message,
                        authorizationTokenExpiredException);
                return;
            }

            try
            {
                var usernameObj = principal.Claims.Where(c => c.Type == Database.Username).Single();


                var appnameObj = principal.Claims.Where(c => c.Type == Database.AppName).Single();

                if (usernameObj == null)
                {
                    HandleUnauthorized(actionContext, null, null);
                    return;
                }

                if (appnameObj == null)
                {
                    HandleUnauthorized(actionContext, null, null);
                    return;
                }

                string username = usernameObj.Value;
                string appname = appnameObj.Value;
                string appNameFromToken = appnameObj.Value;
                if (actionContext.Request.Headers.Contains("AppName"))
                {
                    appname = actionContext.Request.Headers.GetValues("AppName").FirstOrDefault();
                }

                if (!System.Web.HttpContext.Current.Items.Contains(Database.Username))
                    System.Web.HttpContext.Current.Items.Add(Database.Username, username);

                if (!System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                    System.Web.HttpContext.Current.Items.Add(Database.AppName, appname);

                try
                {
                    if (SharedMemorySingeltone.Instance.Contains(appname, SharedMemoryKey.DebugMode))
                    {
                        System.Web.HttpContext.Current.Items[Durados.Workflow.JavaScript.Debug] = true;
                    }

                }
                catch { }

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
                    if (!accountMembershipService.ValidateUser(username) || !accountMembershipService.IsApproved(username) || Revoked(appname, actionContext.Request.Headers.Authorization.Parameter))
                    {
                        HandleUnauthorized(actionContext, appname, username);
                        return;
                    }

                    if (!(Maps.IsDevUser(username) || System.Web.HttpContext.Current.Request.Url.Segments.Contains("general/")) && (new Durados.Web.Mvc.UI.Helpers.DuradosAuthorizationHelper().IsAppLocked(appname) || IsAdminLocked(appname, appNameFromToken, username)))
                    {
                        actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    string.Format(Durados.Web.Mvc.UI.Helpers.UserValidationErrorMessages.AppLocked, appname));
                        return;
                    }
                }
                catch (Durados.Web.Mvc.UI.Helpers.AppNotReadyException exception)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(
            HttpStatusCode.NoContent,
            exception.Message);
                    return;
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
                    if (!(userRole == "Admin" || userRole == "Developer"))
                    {
                        actionContext.ActionArguments.Add(Database.backand_serverAuthorizationAttempt, true);
                        actionContext.ActionArguments.Add(UserRoleNotSufficient, true);

                        HandleUnauthorized(actionContext, appname, username);
                        return;
                    }
                }
                if (_allowedRoles == "Developer")
                {
                    string userRole = Maps.Instance.GetMap(appname).Database.GetUserRole();
                    if (userRole != "Developer")
                    {
                        actionContext.ActionArguments.Add(Database.backand_serverAuthorizationAttempt, true);
                        actionContext.ActionArguments.Add(UserRoleNotSufficient, true);

                        HandleUnauthorized(actionContext, appname, username);
                        return;
                    }
                }
                ((apiController)actionContext.ControllerContext.Controller).Init();
            }
            catch
            {
                HandleUnauthorized(actionContext, null, null);
                return;
            }
        }

        private bool Revoked(string appname, string token)
        {
            if (Maps.Instance.GetMap(appname).Database.EnableTokenRevokation)
            {
                return Durados.Web.Mvc.Farm.SharedMemorySingeltone.Instance.Contains(token);
            }
            return false;
        }

        private bool IsAdminLocked(string appName, string appNameFromToken, string username)
        {
            if (appNameFromToken != Maps.DuradosAppName)
                return false;

            return Maps.Instance.PaymentStatus(appName) == Durados.Web.Mvc.Billing.PaymentStatus.Locked;
            
        }

        public class BasicAuthenticationIdentity
        {
            public string AppGuid { get; private set; }
            public string UserGuid { get; private set; }
            public BasicAuthenticationIdentity(string appGuid, string userGuid)
            {
                AppGuid = appGuid;
                UserGuid = userGuid;
            }
        }

        private bool IsBasicAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!IsBasicAuthentication(actionContext))
                return false;
            string username = null;
            string appName = null;

            try
            {
                BasicAuthenticationIdentity basicAuthenticationIdentity = GetBasicAuthenticationIdentity(actionContext);
                if (!IsBasicAuthorized(basicAuthenticationIdentity, out username, out appName))
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                            HttpStatusCode.Unauthorized,
                            new BasicAuthorizationException());
                    return true;
                }
            }
            catch (Exception exception)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                            HttpStatusCode.InternalServerError,
                            exception.Message);
                return true;
            }

            if (!Durados.Web.Mvc.Maps.IsDevUser(username) && new Durados.Web.Mvc.UI.Helpers.DuradosAuthorizationHelper().IsAppLocked(appName))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    string.Format(Durados.Web.Mvc.UI.Helpers.UserValidationErrorMessages.AppLocked, appName));
                return true;
            }

            if (!Maps.Instance.GetMap(appName).Database.EnableSecretKeyAccess)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        new BasicAuthorizationDisabledException());
                return true;
            }


            if (!System.Web.HttpContext.Current.Items.Contains(Database.Username))
                System.Web.HttpContext.Current.Items.Add(Database.Username, username);

            if (!System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                System.Web.HttpContext.Current.Items.Add(Database.AppName, appName);

            try
            {
                if (SharedMemorySingeltone.Instance.Contains(appName, SharedMemoryKey.DebugMode))
                {
                    System.Web.HttpContext.Current.Items[Durados.Workflow.JavaScript.Debug] = true;
                }

            }
            catch { }

            if (!System.Web.HttpContext.Current.Items.Contains(Database.RequestId))
                System.Web.HttpContext.Current.Items.Add(Database.RequestId, Guid.NewGuid().ToString());

            return true;
        }

        private bool IsBasicAuthorized(BasicAuthenticationIdentity basicAuthenticationIdentity, out string username, out string appName)
        {
            appName = null;
            username = null;
            if (basicAuthenticationIdentity == null)
            {
                return false;
            }
            try
            {
                appName = Maps.Instance.GetAppNameByGuid(basicAuthenticationIdentity.AppGuid);
            }
            catch (ArgumentException)
            {
                return false;
            }
            Map map = Maps.Instance.GetMap(appName);
            if (map == null || map.IsMainMap)
            {
                return false;
            }

            Guid parsedGuid;
            if (!Guid.TryParse(basicAuthenticationIdentity.UserGuid, out parsedGuid))
            {
                return false;
            }

            username = map.Database.GetUsernameByGuid(basicAuthenticationIdentity.UserGuid);

            if (string.IsNullOrEmpty(username))
            {
                username = Maps.Instance.DuradosMap.Database.GetUsernameByGuid(basicAuthenticationIdentity.UserGuid);
                if (string.IsNullOrEmpty(username))
                {
                    return false;
                }
                if (map.Database.GetUserRow(username) == null)
                {
                    return false;
                }
            }

            const string AppId = "appId";
            if (Durados.Web.Mvc.Maps.IsDevUser(username) && System.Web.HttpContext.Current.Request.Headers[AppId] != null)
            {
                int id = -1;
                if (int.TryParse(System.Web.HttpContext.Current.Request.Headers[AppId], out id))
                {
                    appName = Maps.Instance.GetAppNameById(id);
                    if (appName == null)
                    {
                        throw new Durados.DuradosException("App not found with id " + id);
                    }
                }
                else
                {
                    throw new Durados.DuradosException("AppId must be a number instead of " + System.Web.HttpContext.Current.Request.Headers[AppId]);
                }
            }

            return true;
        }

        const string BASIC = "basic";
        const string AUTHORIZATION = "Authorization";
        
            
        private bool IsBasicAuthentication(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            return (actionContext.Request.Headers.Authorization != null && actionContext.Request.Headers.Authorization.Scheme.ToLower() == BASIC) || System.Web.HttpContext.Current.Request.QueryString[BASIC] != null ||
                (System.Web.HttpContext.Current.Request.QueryString[AUTHORIZATION] != null &&
                System.Web.HttpContext.Current.Request.QueryString[AUTHORIZATION].ToLower().Contains(BASIC));
        }

        private BasicAuthenticationIdentity GetBasicAuthenticationIdentity(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string authHeaderValue = null;
            var authRequest = actionContext.Request.Headers.Authorization;
            if (authRequest != null && !String.IsNullOrEmpty(authRequest.Scheme) && authRequest.Scheme.ToLower() == BASIC)
                authHeaderValue = authRequest.Parameter;
            if (string.IsNullOrEmpty(authHeaderValue))
                return GetBasicAuthenticationIdentityFromQueryString();
            authHeaderValue = System.Text.Encoding.Default.GetString(Convert.FromBase64String(authHeaderValue));
            var credentials = authHeaderValue.Split(':');
            return credentials.Length < 2 ? null : new BasicAuthenticationIdentity(credentials[0], credentials[1]);
        }

        private BasicAuthenticationIdentity GetBasicAuthenticationIdentityFromQueryString()
        {
            if (System.Web.HttpContext.Current.Request.QueryString[BASIC] != null)
            {
                var credentials = System.Web.HttpContext.Current.Request.QueryString[BASIC].Split(':');
                return credentials.Length < 2 ? null : new BasicAuthenticationIdentity(credentials[0], credentials[1]);
            }
            else if (System.Web.HttpContext.Current.Request.QueryString[AUTHORIZATION] != null &&
                System.Web.HttpContext.Current.Request.QueryString[AUTHORIZATION].ToLower().Contains(BASIC))
            {
                var credentials1 = System.Web.HttpContext.Current.Request.QueryString[AUTHORIZATION].Split(' ');
                if (credentials1.Length != 2)
                    return null;
                var credentials = credentials1[1].Split(':');
                return credentials.Length < 2 ? null : new BasicAuthenticationIdentity(credentials[0], credentials[1]);
            }
            return null;
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

            if (new Durados.Web.Mvc.UI.Helpers.DuradosAuthorizationHelper().IsAppLocked(appName))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    string.Format(Durados.Web.Mvc.UI.Helpers.UserValidationErrorMessages.AppLocked, appName));
                return true;
            }

            string username = Durados.Database.GuestUsername;

            if (!System.Web.HttpContext.Current.Items.Contains(Database.Username))
                System.Web.HttpContext.Current.Items.Add(Database.Username, username);

            if (!System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                System.Web.HttpContext.Current.Items.Add(Database.AppName, appName);

            try
            {
                if (SharedMemorySingeltone.Instance.Contains(appName, SharedMemoryKey.DebugMode))
                {
                    System.Web.HttpContext.Current.Items[Durados.Workflow.JavaScript.Debug] = true;
                }

            }
            catch { }

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
            else if (actionContext.Request.Headers.Contains("Authorization") && (actionContext.Request.Headers.Authorization.ToString().Contains("anonymous")))
            {
                return actionContext.Request.Headers.Authorization.ToString().Replace("anonymous-", "");
            }
            return null;
        }

        private bool HasAccessToken(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            return (actionContext.Request.Headers.Contains("Authorization") && !(actionContext.Request.Headers.Authorization.ToString().Contains("anonymous")));

        }

        private bool IsServerAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (IsAction(3, "refresh"))
                return IsServerAuthorizedForRefresh(actionContext);

            if (IsAction(3, "rdsResponse"))
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

            if (!dbAppGuid.Equals(appGuid, StringComparison.CurrentCultureIgnoreCase))
            {
                actionContext.ActionArguments.Add(Database.backand_appGuidNotMatch, true);
                return false;
            }

            return true;
        }

        private string GetAppGuid(string appName)
        {

            string sql = "SELECT [Guid] FROM [durados_app] WITH(NOLOCK)  WHERE [Name] =@appName";
            using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(Maps.Instance.DuradosMap.Database.ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, cnn))
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

        private bool IsAction(int actionIndex, string actionName)
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


        private void HandleUnauthorized(System.Web.Http.Controllers.HttpActionContext actionContext, string appName, string username)
        {
            if (appName == null)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        string.Format("The user is unauthorized. Please try to login again or contact the system administrator."));
            }
            else if (username == null)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        string.Format("The user is unauthorized. Please try to login again or contact the system administrator."));
            }
            else if (actionContext.ActionArguments.ContainsKey(Database.backand_serverAuthorizationAttempt))
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
            else if (!string.IsNullOrEmpty(appName) && Maps.Instance.AppExists(appName).HasValue && Maps.Instance.GetOnBoardingStatus(Maps.Instance.AppExists(appName).Value.ToString()) != OnBoardingStatus.Ready)
            {
                string message = new Durados.Web.Mvc.UI.Helpers.AppNotReadyException(appName).Message;
                actionContext.Response = actionContext.Request.CreateResponse(
                        HttpStatusCode.NoContent,
                        message);
            }
            else
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        string.Format("The user {0} is unauthorized for {1}. Please try to login again or contact the system administrator.", username, appName));
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

    public class AuthorizationTokenExpiredException : ServerAuthorizationFailureException
    {
        public AuthorizationTokenExpiredException() : base("invalid or expired token") { }
    }

    public class BasicAuthorizationException : ServerAuthorizationFailureException
    {
        public BasicAuthorizationException() : base("invalid credentials") { }
    }

    public class BasicAuthorizationDisabledException : ServerAuthorizationFailureException
    {
        public BasicAuthorizationDisabledException() : base("Basic auth is disabled") { }
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