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

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

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

        //private bool UserBelongToApp(string username, string appname)
        //{
        //    return true;
        //}
    }

    

    
}

   // Refresh token keep it
    ///*    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    //    {
    //        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
    //        {

    //            string clientId = string.Empty;
    //            string clientSecret = string.Empty;
    //            Client client = null;

    //            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
    //            {
    //                context.TryGetFormCredentials(out clientId, out clientSecret);
    //            }

    //            if (context.ClientId == null)
    //            {
    //                //Remove the comments from the below line context.SetError, and invalidate context 
    //                //if you want to force sending clientId/secrects once obtain access tokens. 
    //                context.Validated();
    //                //context.SetError("invalid_clientId", "ClientId should be sent.");
    //                return Task.FromResult<object>(null);
    //            }

    //            /*using (AuthRepository _repo = new AuthRepository())
    //            {
    //                client = _repo.FindClient(context.ClientId);
    //            }*/

    //            if (client == null)
    //            {
    //                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
    //                return Task.FromResult<object>(null);
    //            }

    //            if (client.ApplicationType == Models.ApplicationTypes.NativeConfidential)
    //            {
    //                if (string.IsNullOrWhiteSpace(clientSecret))
    //                {
    //                    context.SetError("invalid_clientId", "Client secret should be sent.");
    //                    return Task.FromResult<object>(null);
    //                }
    //                else
    //                {
    //                    if (client.Secret != ""/*Helper.GetHash(clientSecret)*/)
    //                    {
    //                        context.SetError("invalid_clientId", "Client secret is invalid.");
    //                        return Task.FromResult<object>(null);
    //                    }
    //                }
    //            }

    //            if (!client.Active)
    //            {
    //                context.SetError("invalid_clientId", "Client is inactive.");
    //                return Task.FromResult<object>(null);
    //            }

    //            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
    //            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

    //            context.Validated();
    //            return Task.FromResult<object>(null);
    //        }

    //        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
    //        {

    //            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

    //            if (allowedOrigin == null) allowedOrigin = "*";

    //            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

    //           /* using (AuthRepository _repo = new AuthRepository())
    //            {
    //                IdentityUser user = await _repo.FindUser(context.UserName, context.Password);

    //                if (user == null)
    //                {
    //                    context.SetError("invalid_grant", "The user name or password is incorrect.");
    //                    return;
    //                }
    //            }*/

    //            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
    //            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
    //            identity.AddClaim(new Claim("sub", context.UserName));
    //            identity.AddClaim(new Claim("role", "user"));

    //            var props = new AuthenticationProperties(new Dictionary<string, string>
    //                {
    //                    { 
    //                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
    //                    },
    //                    { 
    //                        "userName", context.UserName
    //                    }
    //                });

    //            var ticket = new AuthenticationTicket(identity, props);
    //            context.Validated(ticket);

    //        }

    //        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
    //        {
    //            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
    //            var currentClient = context.ClientId;

    //            if (originalClient != currentClient)
    //            {
    //                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
    //                return Task.FromResult<object>(null);
    //            }

    //            // Change auth ticket for refresh token requests
    //            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
    //            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

    //            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
    //            context.Validated(newTicket);

    //            return Task.FromResult<object>(null);
    //        }

    //        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
    //        {
    //            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
    //            {
    //                context.AdditionalResponseParameters.Add(property.Key, property.Value);
    //            }

    //            return Task.FromResult<object>(null);
    //        }

    //    }
    //}*/
  
//}