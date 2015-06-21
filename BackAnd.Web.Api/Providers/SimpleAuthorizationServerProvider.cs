using Durados;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;

using System.Linq;
using System.Collections.Generic;
using Durados.Web.Mvc.UI.Helpers;

namespace BackAnd.Web.Api.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            var usernameObj = context.Identity.Claims.Where(c => c.Type == Database.Username).Single();

            var appnameObj = context.Identity.Claims.Where(c => c.Type == Database.AppName).Single();

            if (usernameObj != null && appnameObj != null)
            {
                string username = usernameObj.Value;
                string appname = appnameObj.Value;

                context.AdditionalResponseParameters.Add("appName", appname);
                context.AdditionalResponseParameters.Add("username", username);

                try
                {
                    Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appname);
                    string role = map.Database.GetUserRole(username);
                    string userId = map.Database.GetUserID(username).ToString();

                    context.AdditionalResponseParameters.Add("role", role);
                    context.AdditionalResponseParameters.Add("userId", userId);
                }
                catch { }
            }

            return Task.FromResult<object>(null);
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            var value = context.Request.Query.Get("token");

            if (!string.IsNullOrEmpty(value))
            {
             //   context.Token = value;
            }

            context.Validated();
        }

        private void ValidateById(OAuthGrantResourceOwnerCredentialsContext context)
        {
            if (System.Web.HttpContext.Current.Request.Form["appid"] == null || System.Web.HttpContext.Current.Request.Form["userid"] == null)
            {
                context.SetError(UserValidationErrorMessages.MissingUserIdOrAppId, UserValidationErrorMessages.MissingUserIdOrAppId);
                return;
            }
            string appName = GetAppName(System.Web.HttpContext.Current.Request.Form["appid"]);
            if (appName == null)
            {
                context.SetError(UserValidationErrorMessages.WrongUserIdOrAppId, UserValidationErrorMessages.WrongUserIdOrAppId);
                return;
            }
            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appName);
            if (map == Durados.Web.Mvc.Maps.Instance.DuradosMap)
            {
                context.SetError(UserValidationErrorMessages.WrongUserIdOrAppId, UserValidationErrorMessages.WrongUserIdOrAppId);
                return;
            }
            if (!map.Database.EnableSecretKeyAccess)
            {
                context.SetError(UserValidationErrorMessages.EnableKeysAccess, UserValidationErrorMessages.EnableKeysAccess);
                return;
            }
            string username = GetUserName(System.Web.HttpContext.Current.Request.Form["userid"], appName);
            if (username == null)
            {
                context.SetError(UserValidationErrorMessages.WrongUserIdOrAppId, UserValidationErrorMessages.WrongUserIdOrAppId);
                return;
            }

            
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(Database.Username, username));
            identity.AddClaim(new Claim(Database.AppName, appName));

            context.Validated(identity);

            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-end", appName, username, null, 3, string.Empty);
        }

        private void ValidateByOneTimeAccessToken(OAuthGrantResourceOwnerCredentialsContext context)
        {
            if (System.Web.HttpContext.Current.Request.Form["accessToken"] == null)
            {
                context.SetError(UserValidationErrorMessages.MissingAccessToken, UserValidationErrorMessages.MissingAccessToken);
                return;
            }
            string accessToken = System.Web.HttpContext.Current.Request.Form["accessToken"];

            string appName = System.Web.HttpContext.Current.Request.Form["appName"];
            if (appName == null)
            {
                context.SetError(UserValidationErrorMessages.AppNameNotSupplied, UserValidationErrorMessages.AppNameNotSupplied);
                return;
            }
            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appName);
            if (map == null || (appName != Durados.Web.Mvc.Maps.DuradosAppName && map == Durados.Web.Mvc.Maps.Instance.DuradosMap))
            {
                context.SetError(UserValidationErrorMessages.WrongAppName, UserValidationErrorMessages.WrongAppName);
                return;
            }

            string userGuid = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetUserGuidFromTmpGuid(accessToken);

            string username = Durados.Web.Mvc.Maps.Instance.DuradosMap.Database.GetUsernameByGuid(userGuid);
            if (username == null)
            {
                context.SetError(UserValidationErrorMessages.InvalidAccessToken, UserValidationErrorMessages.InvalidAccessToken);
                return;
            }

            Durados.Web.Mvc.Controllers.AccountMembershipService accountMembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();
            if (map.Database.GetUserRow(username) == null)
            {
                context.SetError(UserValidationErrorMessages.UserDoesNotBelongToApp, UserValidationErrorMessages.UserDoesNotBelongToApp);
                return;
            }


            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(Database.Username, username));
            identity.AddClaim(new Claim(Database.AppName, appName));

            context.Validated(identity);

            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-end", appName, username, null, 3, string.Empty);
        }

        private string GetAppName(string appId)
        {
            return Durados.Web.Mvc.Maps.Instance.GetAppNameByGuid(appId);
        }

        private string GetUserName(string userId, string appName)
        {
            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appName);
            return map.Database.GetUsernameByGuid(userId);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            
            
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            if (System.Web.HttpContext.Current.Request.Form["username"] == null && System.Web.HttpContext.Current.Request.Form["userid"] != null)
            {
                ValidateById(context);
                return;
            }
            else if (System.Web.HttpContext.Current.Request.Form["username"] == null && System.Web.HttpContext.Current.Request.Form["accessToken"] != null)
            {
                ValidateByOneTimeAccessToken(context);
                return;
            }
            string appname = null;

            appname = System.Web.HttpContext.Current.Request.Form[Database.AppName];
            if (string.IsNullOrEmpty(appname))
            {
                context.SetError(UserValidationErrorMessages.InvalidGrant, UserValidationErrorMessages.AppNameNotSupplied);
                return;
            }

            if (!IsAppExists(appname))
            {
                context.SetError(UserValidationErrorMessages.InvalidGrant, string.Format(UserValidationErrorMessages.AppNameNotExists, appname));
                return;
            }

            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-start", appname, context.UserName, null, 3, string.Empty);


            if (!System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                System.Web.HttpContext.Current.Items.Add(Database.AppName, appname);

            UserValidationError userValidationError = UserValidationError.Valid;
            if (!IsValid(context.UserName, context.Password, out userValidationError))
            {
                
                
                Durados.Web.Mvc.Controllers.AccountMembershipService accountMembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();

                string message = UserValidationErrorMessages.Unknown;

                switch (userValidationError)
                {
                    case UserValidationError.IncorrectUsernameOrPassword:
                        message = UserValidationErrorMessages.IncorrectUsernameOrPassword;
                        break;
                    case UserValidationError.LockedOut:
                        message = UserValidationErrorMessages.LockedOut;
                        break;
                    case UserValidationError.NotApproved:
                        message = UserValidationErrorMessages.NotApproved;
                        break;
                    case UserValidationError.NotRegistered:
                        message = UserValidationErrorMessages.NotRegistered;
                        break;
                    case UserValidationError.UserDoesNotBelongToApp:
                        message = UserValidationErrorMessages.UserDoesNotBelongToApp;
                        break;

                    default:
                        message = UserValidationErrorMessages.Unknown;
                        break;

                }

                context.SetError(UserValidationErrorMessages.InvalidGrant, message);

                Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-end-failure", appname, context.UserName, null, 3, message);

                return;
            }

            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appname);
            
            if (!string.IsNullOrEmpty(map.Database.LogOnUrlAuth) && !new DuradosAuthorizationHelper().ValidateLogOnAuthUrl(map, System.Web.HttpContext.Current.Request.Form))
            {
                string message="External authentication failer";
                context.SetError(UserValidationErrorMessages.InvalidGrant, message);

                Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-end-failure", appname, context.UserName, null, 3, message);

                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(Database.Username, context.UserName));
            identity.AddClaim(new Claim(Database.AppName, appname));

            context.Validated(identity);

            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-end", appname, context.UserName, null, 3, string.Empty);
        }

        public static bool IsAppExists(string appname)
        {
            if (appname.Equals(Durados.Web.Mvc.Maps.DuradosAppName))
                return true;
            return new DuradosAuthorizationHelper().IsAppExists(appname);
        }

        public bool IsValid(string username, string password, out UserValidationError userValidationError)
        {

            return new DuradosAuthorizationHelper().IsValid(username, password, out userValidationError);
        }

        public override Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            return base.ValidateAuthorizeRequest(context);
        }

       
    }

    

    
}

   