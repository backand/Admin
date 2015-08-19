﻿using Durados;
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
                    var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;

                    context.Properties.IssuedUtc = currentUtc;
                    context.Properties.ExpiresUtc = currentUtc.Add(System.TimeSpan.FromSeconds(map.Database.TokenExpiration));

                    string role = map.Database.GetUserRole(username);
                    string userId = map.Database.GetUserID(username).ToString();
                    
                    context.AdditionalResponseParameters.Add("role", role);
                    try
                    {
                        string fullName = map.Database.GetUserFullName(username).ToString();
                        context.AdditionalResponseParameters.Add("fullName", fullName);
                    }
                    catch { }
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
            string id, secret;
            if (context.TryGetBasicCredentials(out id, out secret))
            {
                if (secret == "secret")
                {
                    // need to make the client_id available for later security checks
                    context.OwinContext.Set<string>("as:client_id", id);
                    context.Validated();
                }
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

        private System.Collections.Specialized.NameValueCollection GetFixedForm()
        {
            string temp1 = "_$temp1$_";
            string temp2 = "_$temp2$_";

            string eq = "=";
            string amp = "&";

            string originalForm = System.Web.HttpContext.Current.Request.Form.ToString();

            string tempFrom = originalForm;
            foreach (string key in System.Web.HttpContext.Current.Request.Form.Keys)
            {
                tempFrom = tempFrom.Replace(amp + key + eq, temp1 + key);
            }
            foreach (string key in System.Web.HttpContext.Current.Request.Form.Keys)
            {
                tempFrom = tempFrom.Replace(key + eq, temp2 + key);
            }

            tempFrom = tempFrom.Replace("&", "%26").Replace("=", "%3D").Replace("+", "%2B");

            foreach (string key in System.Web.HttpContext.Current.Request.Form.Keys)
            {
                tempFrom = tempFrom.Replace(temp1 + key, amp + key + eq);
            }

            foreach (string key in System.Web.HttpContext.Current.Request.Form.Keys)
            {
                tempFrom = tempFrom.Replace(temp2 + key, key + eq);
            }

            System.Collections.Specialized.NameValueCollection form = System.Web.HttpUtility.ParseQueryString(tempFrom);

            return form;

        }

  //      public override async Task GrantRefreshToken(
  //OAuthGrantRefreshTokenContext context)
  //      {
  //          var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
  //          var currentClient = context.OwinContext.Get<string>("as:client_id");

  //          // enforce client binding of refresh token
  //          if (originalClient != currentClient)
  //          {
  //              context.Rejected();
  //              return;
  //          }

  //          // chance to change authentication ticket for refresh token requests
  //          var newId = new ClaimsIdentity(context.Ticket.Identity);
  //          newId.AddClaim(new Claim("newClaim", "refreshToken"));

  //          var newTicket = new Microsoft.Owin.Security.AuthenticationTicket(newId, context.Ticket.Properties);
  //          context.Validated(newTicket);
  //      }

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
            else if (System.Web.HttpContext.Current.Request.Form["username"] != null && System.Web.HttpContext.Current.Request.Form["refreshToken"] != null)
            {
                ValidateByRefreshToken(context, System.Web.HttpContext.Current.Request.Form["username"], System.Web.HttpContext.Current.Request.Form[Database.AppName], System.Web.HttpContext.Current.Request.Form["refreshToken"]);
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

            System.Collections.Specialized.NameValueCollection form = GetFixedForm();

            string username = context.UserName;

            try
            {
                username = form["username"];
            }
            catch { }

            string password = context.Password;

            try
            {
                password = form["password"];
            }
            catch { }


            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-start", appname, username, null, 3, string.Empty);


            if (!System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                System.Web.HttpContext.Current.Items.Add(Database.AppName, appname);

            if (!System.Web.HttpContext.Current.Items.Contains(Database.Username))
                System.Web.HttpContext.Current.Items.Add(Database.Username, username);

            UserValidationError userValidationError = UserValidationError.Valid;
            if (!IsValid(username, password, out userValidationError))
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

                Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-end-failure", appname, username, null, 3, message);

                return;
            }

            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appname);

            if (map.Database.SecureLevel == SecureLevel.AllUsers)
            {
                if (!(new Durados.Web.Mvc.Controllers.AccountMembershipService().AuthenticateUser(username, password)))
                {
                    context.SetError(UserValidationErrorMessages.InvalidGrant, UserValidationErrorMessages.IncorrectUsernameOrPassword);

                    Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-end-failure", appname, username, null, 3, UserValidationErrorMessages.IncorrectUsernameOrPassword);

                    return;
                }
            }

            if (!string.IsNullOrEmpty(map.Database.LogOnUrlAuth) && !new DuradosAuthorizationHelper().ValidateLogOnAuthUrl(map, System.Web.HttpContext.Current.Request.Form))
            {
                string message = "External authentication failer";
                context.SetError(UserValidationErrorMessages.InvalidGrant, message);

                Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-end-failure", appname, username, null, 3, message);

                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(Database.Username, username));
            identity.AddClaim(new Claim(Database.AppName, appname));
            // create metadata to pass on to refresh token provider
            //var props = new Microsoft.Owin.Security.AuthenticationProperties(new Dictionary<string, string>
            //{
            //    { "as:client_id", context.ClientId }
            //});
            //var ticket = new Microsoft.Owin.Security.AuthenticationTicket(identity, props);

            //context.Validated(ticket);
            context.Validated(identity);

            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("auth-end", appname, username, null, 3, string.Empty);
        }

        private void ValidateByRefreshToken(OAuthGrantResourceOwnerCredentialsContext context, string username, string appName, string refreshToken)
        {
            try
            {
                if (!RefreshToken.Validate(appName, refreshToken, username))
                {
                    context.SetError(UserValidationErrorMessages.InvalidRefreshToken, "Either invalid refresh or wrong username or app name");
                    Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("refresh token", appName, username, null, 3, UserValidationErrorMessages.InvalidRefreshToken);
                    return;
                }
            }
            catch (AppNotFoundException exception)
            {
                context.SetError(exception.Message, exception.Message);
                Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("refresh token", appName, username, exception, 2, string.Empty);
                return;
            }
            catch (System.Exception exception)
            {
                context.SetError(UserValidationErrorMessages.Unknown, UserValidationErrorMessages.Unknown);
                Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("refresh token", appName, username, exception, 1, string.Empty);
                return;
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(Database.Username, username));
            identity.AddClaim(new Claim(Database.AppName, appName));

            context.Validated(identity);

            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("refresh token", appName, username, null, 3, string.Empty);
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

   