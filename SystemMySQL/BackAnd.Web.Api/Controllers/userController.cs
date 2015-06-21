using System;
//﻿using Backand.Web.Api;
using BackAnd.Web.Api.Models;
//using BackAnd.Web.Api.Providers;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Durados.Web.Mvc.Controllers;
using System.Web;
using Microsoft.Owin.Security.Infrastructure;
using Owin.Security.Providers.GitHub;
using Backand.Web.Api;
using BackAnd.Web.Api.Providers;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc;
using System.Data;
using System.Web.Script.Serialization;
using System.Text;
using System.Globalization;
using Microsoft.Owin;
using Newtonsoft.Json;
using Microsoft.Owin.Helpers;
using Newtonsoft.Json.Linq;


namespace BackAnd.Web.Api.Controllers
{


    [RoutePrefix("1/user")]
    public class userController : wfController
    {
        [HttpDelete]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        [Route("backand")]
        public IHttpActionResult Delete(string username)
        {
            if (Maps.Instance.DuradosMap.Database.GetUserRole() != "Developer")
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));
            }

            Account account = new Account(this);
            account.DeleteUser(username, map.AppName);
            return Ok();
        }

        [HttpGet]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        [Route("key/{id}")]
        public IHttpActionResult key(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
            }

            try
            {
                return Ok(RestHelper.GetUserKey(id));


            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        protected override void BeforeEditInDatabase(Durados.EditEventArgs e)
        {
            e.ColumnNames.Add("Guid");
        }

        [Route("key/reset/{id}")]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        [HttpGet]
        public IHttpActionResult reset(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
            }

            try
            {
                return Ok(RestHelper.ResetUserKey(id, view_BeforeEditInDatabase));


            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }



        [HttpGet]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        [Route("exists")]
        public virtual IHttpActionResult exists(string username)
        {
            if (!IsAdmin())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));

            System.Web.Security.MembershipUser existingUser = System.Web.Security.Membership.Provider.GetUser(username, false);

            bool exist = existingUser != null;
            return Ok(new { exists = exist });
        }

        [HttpPost]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        public virtual IHttpActionResult Post()
        {
            if (!IsAdmin())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));

            string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
            Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

            string role = null;
            if (values.ContainsKey("role"))
            {
                role = values["role"].ToString();
            }

            string appName = System.Web.HttpContext.Current.Items[Durados.Web.Mvc.Database.AppName].ToString();
                
            if (!Account.IsValidRole(appName, role))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The user role does not match any of the app roles."));


            return AddNewUser(false, role, true);
        }

        [Route("signup")]
        [HttpPost]
        [BackAnd.Web.Api.Controllers.Filters.TokenAuthorize(BackAnd.Web.Api.Controllers.Filters.HeaderToken.SignUpToken)]
        public virtual IHttpActionResult SignUp()
        {
            return AddNewUser(null);
        }

        protected virtual IHttpActionResult AddNewUser(bool? isSignupEmailVerification, string role = null, bool byAdmin = false)
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (!values.ContainsKey("firstName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "First name is missing"));

                }

                string firstName = values["firstName"].ToString();


                if (!values.ContainsKey("email"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Email is missing"));
                }

                string email = values["email"].ToString();

                if (!values.ContainsKey("lastName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Last name is missing"));

                }
                string lastName = values["lastName"].ToString();
                

                if (!values.ContainsKey("password"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Password is missing"));

                }
                string password = values["password"].ToString();

                if (!values.ContainsKey("confirmPassword"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Confirm password is missing"));

                }
                string confirmPassword = values["confirmPassword"].ToString();

                if (!password.Equals(confirmPassword))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The password is is not confirmed"));


                Dictionary<string, object> parameters = null;
                if (values.ContainsKey("parameters"))
                {
                    if (!(values["parameters"] is Dictionary<string, object>))
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Could not convert parameters to object"));
                    }
                    parameters = (Dictionary<string, object>)values["parameters"];
                }

                if (!System.Web.HttpContext.Current.Items.Contains(Durados.Web.Mvc.Database.AppName))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "App not found"));
                }
                string appName = System.Web.HttpContext.Current.Items[Durados.Web.Mvc.Database.AppName].ToString();
                Account account = new Account(this);

                try
                {
                    Durados.Web.Mvc.UI.Helpers.Account.SignUpResults signUpResults = account.SignUp(appName, firstName, lastName, email, role, byAdmin, password, confirmPassword, isSignupEmailVerification, parameters, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                    if (signUpResults.Status == Account.SignUpStatus.Ready)
                    {
                        var identity = new ClaimsIdentity("Bearer");
                        identity.AddClaim(new Claim("username", email));
                        identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, "Backand"));
                        identity.AddClaim(new Claim("appname", appName));


                        // create token
                        string accessToken = CreateToken(identity);

                        var response = new { username = signUpResults.Username, currentStatus = (int)signUpResults.Status, message = account.GetSignUpStatusMessage(signUpResults.Status), listOfPossibleStatus = account.GetListOfPossibleStatus(), token = accessToken };
                        return Ok(response);
                    }
                    else
                    {
                        var response = new { username = signUpResults.Username, currentStatus = (int)signUpResults.Status, message = account.GetSignUpStatusMessage(signUpResults.Status), listOfPossibleStatus = account.GetListOfPossibleStatus() };
                        return Ok(response);
                    }
                }
                catch (Durados.Web.Mvc.UI.Helpers.Account.SignUpException exception)
                {
                    Log(appName, exception, 3);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, exception.Message));
                }
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private Map GetMap(string appName)
        {
            Map map = Maps.Instance.GetMap(appName);

            return map;
        }

        protected virtual void Log(string appName, Exception exception, int logType)
        {
            GetMap(appName).Logger.Log(GetControllerNameForLog(ControllerContext), string.Empty, string.Empty, exception, logType, null);

        }

        protected virtual void Log(Exception exception, int logType)
        {
            Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), string.Empty, string.Empty, exception, logType, null);

        }

        [Route("verify")]
        [HttpGet]
        public virtual HttpResponseMessage Verify(string appName, string parameters)
        {
            if (string.IsNullOrEmpty(appName))
                return Request.CreateResponse(HttpStatusCode.NotFound, "appName cannot be empty");
            Map map = GetMap(appName);
            if (map == null || map is DuradosMap)
                return Request.CreateResponse(HttpStatusCode.NotFound, "app was not found");

            if (string.IsNullOrEmpty( map.Database.SignInRedirectUrl))
                return Request.CreateResponse(HttpStatusCode.NotFound, "SignIn redirect url was not supplied in configuration");
            if (string.IsNullOrEmpty( map.Database.RegistrationRedirectUrl))
                return Request.CreateResponse(HttpStatusCode.NotFound, "SignUp redirect url was not supplied in configuration");

            try
            {
                Account account = new Account(this);

                try
                {
                    Durados.Web.Mvc.UI.Helpers.Account.SignUpResults signUpResults = account.Verify(appName, parameters, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                    var response = Request.CreateResponse(HttpStatusCode.Moved);
                    response.Headers.Location = new Uri(signUpResults.Redirect);
                    return response;
                }
                catch (Durados.Web.Mvc.UI.Helpers.Account.SignUpException exception)
                {
                    Log(exception, 3);
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, exception.Message);
                }
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [Route("password")]
        [HttpPut]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        public virtual IHttpActionResult password()
        {
            try
            {
                if (!IsAdmin())
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (!values.ContainsKey("username"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The username is missing"));

                }

                if (!values.ContainsKey("password"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The password is missing"));

                }
                string username = values["username"].ToString();
                string password = values["password"].ToString();

                Account account = new Account(this);
                account.ChangePassword(username, password);


                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [Route("changePassword")]
        [HttpPost]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        public virtual IHttpActionResult changePassword()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (!values.ContainsKey("oldPassword"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The old password is missing"));

                }

                if (!values.ContainsKey("newPassword"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The new password is missing"));

                }

                string newPassword = values["newPassword"].ToString();
                string oldPassword = values["oldPassword"].ToString();
                string username = Map.Database.GetCurrentUsername();


                Account account = new Account(this);
                account.ChangePassword(username, newPassword, oldPassword);


                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [Route("requestResetPassword")]
        [HttpPost]
        public virtual IHttpActionResult requestResetPassword()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (!values.ContainsKey("appName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The appName is missing"));

                }

                if (!values.ContainsKey("username"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The username is missing"));

                }

                string appName = values["appName"].ToString();
                string username = values["username"].ToString();

                Account account = new Account(this);

                account.SendForgotPasswordToken(appName, username);

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [Route("resetPassword")]
        [HttpPost]
        public virtual IHttpActionResult resetPassword()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (!values.ContainsKey("newPassword"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The newPassword is missing"));

                }

                if (!values.ContainsKey("resetToken"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The resetToken is missing"));

                }
                Guid token = Guid.Parse(values["resetToken"].ToString());
                string password = values["newPassword"].ToString();

                Account account = new Account(this);

                account.ChangePassword(token, password);

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [AllowAnonymous]
        [Route("signInFacebook")]
        [HttpGet]
        public async Task<IHttpActionResult> facebookSignin(string code, string state)
        {
             var _httpClient = new HttpClient();
           // _httpClient.Timeout = Options.BackchannelTimeout;
            _httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB

            var appId = "1106885725994585";
            var appSecret = "2c0a87bf88bffacd8d9c80b6bd567bc1";
            var properties = JsonConvert.DeserializeObject<AuthenticationProperties>(state);
            
            if (properties == null)
            {
                return null;
            }

                // OAuth2 10.12 CSRF
            /*    if (!ValidateCorrelationId(properties, _logger))
                {
                    return new AuthenticationTicket(null, properties);
                }

                if (code == null)
                {
                    // Null if the remote server returns an error.
                    return new AuthenticationTicket(null, properties);
                }
            */
                
                
                string redirectUri = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/1/user/signInFacebook";
                string tokenRequest = "grant_type=authorization_code" +
                    "&code=" + Uri.EscapeDataString(code) +
                    "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                    "&client_id=" + Uri.EscapeDataString(appId) +
                    "&client_secret=" + Uri.EscapeDataString(appSecret);

                HttpResponseMessage tokenResponse = await _httpClient.GetAsync(TokenEndpoint + "?" + tokenRequest, HttpCompletionOption.ResponseContentRead);
                tokenResponse.EnsureSuccessStatusCode();
                string text = await tokenResponse.Content.ReadAsStringAsync();
                IFormCollection form = WebHelpers.ParseForm(text);

                string accessToken = form["access_token"];
                string expires = form["expires"];
                string graphAddress = GraphApiEndpoint + "?access_token=" + Uri.EscapeDataString(accessToken);
                
               /* if (Options.SendAppSecretProof)
                {
                    graphAddress += "&appsecret_proof=" + GenerateAppSecretProof(accessToken);
                }*/

                HttpResponseMessage graphResponse = await _httpClient.GetAsync(graphAddress,HttpCompletionOption.ResponseContentRead);
                graphResponse.EnsureSuccessStatusCode();
                text = await graphResponse.Content.ReadAsStringAsync();
                JObject user = JObject.Parse(text);

                var email = user["email"].Value<string>();
                var redirectUrl = properties.RedirectUri;
                var appName = properties.Dictionary["appname"];
                
   
            // Dear Relly,
            // You have here the 3 parameters you need to finsih authentification.
            // So i don't block you for tomorrow.
            // My todo's are:
            // 1. Handle unsucessful login
            // 2. Securize State data


                return null;
                /*var context = new FacebookAuthenticatedContext(Context, user, accessToken, expires);
                context.Identity = new ClaimsIdentity(
                    Options.AuthenticationType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                if (!string.IsNullOrEmpty(context.Id))
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, context.Id, XmlSchemaString, Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.UserName))
                {
                    context.Identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, context.UserName, XmlSchemaString, Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.Email))
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.Email, context.Email, XmlSchemaString, Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.Name))
                {
                    context.Identity.AddClaim(new Claim("urn:facebook:name", context.Name, XmlSchemaString, Options.AuthenticationType));

                    // Many Facebook accounts do not set the UserName field.  Fall back to the Name field instead.
                    if (string.IsNullOrEmpty(context.UserName))
                    {
                        context.Identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, context.Name, XmlSchemaString, Options.AuthenticationType));
                    }
                }
                if (!string.IsNullOrEmpty(context.Link))
                {
                    context.Identity.AddClaim(new Claim("urn:facebook:link", context.Link, XmlSchemaString, Options.AuthenticationType));
                }
                context.Properties = properties;

                await Options.Provider.Authenticated(context);

                return new AuthenticationTicket(context.Identity, context.Properties);
            */
        }

        private const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
        private const string TokenEndpoint = "https://graph.facebook.com/oauth/access_token";
        private const string GraphApiEndpoint = "https://graph.facebook.com/me";
        [AllowAnonymous]
        [Route("socialSignin")]
        [HttpGet]
        public async Task<IHttpActionResult> socialSignin(string provider, string appName, string returnAddress = null, string code = null)
        {

            if (code != null)
            {
                Social social = Social.GetSocialProvider(provider);
                Social.Profile profile = social.Authenticate(appName, code);
                social.Signin(profile);
                return Ok(GetAccessToken(profile.email, appName, provider));
            }
            try
            {
                Social social = Social.GetSocialProvider(provider);
                return Redirect(social.GetAuthUrl(appName, returnAddress ?? GetCurrentAddress(), null, "signin"));
            }
            catch (Exception exception)
            {
                Map.Logger.Log("user", "socialSignin", exception.Source, exception, 1, null);
                return BadRequest(exception.Message);
            }
           
        }
        

        private string GetCurrentAddress()
        {
            return System.Web.HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
        }

        [AllowAnonymous]
        [Route("socialSignup")]
        [HttpGet]
        public async Task<IHttpActionResult> socialSignup(string provider, string appName, string returnAddress = null, string parameters = null)
        {
            try
            {
                //if (provider == "facebook")
                //{
                //    string appId = "1106885725994585";
                //    string redirectUri = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/1/user/signInFacebook";

                //    AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = returnAddress, };
                //    properties.Dictionary.Add("appname", appName);
                //    // OAuth2 10.12 CSRF
                //    //GenerateCorrelationId(properties);

                //    // comma separated
                //    string scope = "email";

                //    string state = JsonConvert.SerializeObject(properties);

                //    string authorizationEndpoint =
                //        "https://www.facebook.com/dialog/oauth" +
                //            "?response_type=code" +
                //            "&client_id=" + Uri.EscapeDataString(appId) +
                //            "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                //            "&scope=" + Uri.EscapeDataString(scope) +
                //            "&state=" + Uri.EscapeDataString(state);
                //    return Redirect(authorizationEndpoint);
                //}

                Social social = Social.GetSocialProvider(provider);
                return Redirect(social.GetAuthUrl(appName, returnAddress ?? GetCurrentAddress(), parameters, "signup"));
            }
            catch (Exception exception)
            {
                Map.Logger.Log("user", "socialSignin", exception.Source, exception, 1, null);
                return BadRequest(exception.Message);
            }
        }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static string GenerateTimeStamp()
        {
            TimeSpan secondsSinceUnixEpocStart = DateTime.UtcNow - Epoch;
            return Convert.ToInt64(secondsSinceUnixEpocStart.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        private static string ComputeSignature(string consumerSecret, string tokenSecret, string signatureData)
        {
            using (var algorithm = new HMACSHA1())
            {
                algorithm.Key = Encoding.ASCII.GetBytes(
                    string.Format(CultureInfo.InvariantCulture,
                        "{0}&{1}",
                        Uri.EscapeDataString(consumerSecret),
                        string.IsNullOrEmpty(tokenSecret) ? string.Empty : Uri.EscapeDataString(tokenSecret)));
                byte[] hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(signatureData));
                return Convert.ToBase64String(hash);
            }
        }

        [AllowAnonymous]
        [Route("socialProviders")]
        [HttpGet]
        public IHttpActionResult socialProviders(string appName, string returnAddress = null)
        {
            Map map = Maps.Instance.GetMap(appName);

            HashSet<string> providers = map.Database.GetSocialProviders();

            List<object> providersUrls = new List<object>();

            foreach (string provider in providers)
            {
                providersUrls.Add(new { name = provider, 
                    signin = "1/user/socialSignin?provider=" + provider + "&appname=" + appName + (string.IsNullOrEmpty(returnAddress) ? "" : "&returnAddress=" + System.Web.HttpContext.Current.Server.UrlEncode(returnAddress)), 
                    signup = "1/user/socialSignup?provider=" + provider + "&appname=" + appName + (string.IsNullOrEmpty(returnAddress) ? "" : "&returnAddress=" + System.Web.HttpContext.Current.Server.UrlEncode(returnAddress)) 
                });
           
            }

            return Ok(providersUrls);
        }


        [AllowAnonymous]
        [Route("{provider}/auth")]
        [HttpGet]
        public async Task<IHttpActionResult> auth(string provider)
        {
            string returnAddress = null;
            try
            {
                Social social = Social.GetSocialProvider(provider);

                Social.Profile profile = social.Authenticate();
                returnAddress = profile.returnAddress;
                    
                if (profile.activity == "signin")
                {
                    social.Signin(profile);
                }
                else if (profile.activity == "signup")
                {
                    string email = profile.email;
                    string appName = profile.appName;
                    string parameters = profile.parameters;

                    System.Web.HttpContext.Current.Items.Add(Durados.Database.AppName, appName);
                    if (!System.Web.HttpContext.Current.Items.Contains(Durados.Database.RequestId))
                        System.Web.HttpContext.Current.Items.Add(Durados.Database.RequestId, Guid.NewGuid().ToString());


                    if (email != null &&
                        !string.IsNullOrWhiteSpace(appName) &&
                        !string.IsNullOrWhiteSpace(returnAddress) &&
                        (new DuradosAuthorizationHelper().IsAppExists(appName) || appName == Maps.DuradosAppName))
                    {
                        // check if user belongs to app
                        DataRow userRow = null;
                        if (appName == Maps.DuradosAppName)
                        {
                            userRow = Maps.Instance.DuradosMap.Database.GetUserRow(email);
                        }
                        else
                        {
                            userRow = Maps.Instance.GetMap(appName).Database.GetUserRow(email);
                        }
                        if (userRow != null)
                        {
                            return Redirect(GetErrorUrl(returnAddress, "The user already signed up to " + appName, provider));
                        }


                        Dictionary<string, object> values = new Dictionary<string, object>();

                        if (!string.IsNullOrEmpty(parameters) && parameters != "parameters")
                        {
                            try
                            {
                                values = Durados.Web.Mvc.Controllers.Api.JsonConverter.Deserialize(parameters);
                            }
                            catch (Exception exception)
                            {
                                Log(appName, exception, 1);
                                return Redirect(GetErrorUrl(returnAddress, "Failed to get parameters", provider));
                            }
                        }
                        if (!values.ContainsKey("socialProfile"))
                        {
                            values.Add("socialProfile", profile);
                        }

                        try
                        {
                            CallActionBeforeSignup(appName, email, profile, values);
                        }
                        catch (Exception exception)
                        {
                            Log(appName, exception, 1);
                            return Redirect(GetErrorUrl(returnAddress, "Failed to run beforeSocialSignup action", provider));
                        }

                        Account account = new Account(this);

                        string firstName = profile.firstName;
                        if (values.ContainsKey("firstName") && values["firstName"] != null)
                        {
                            firstName = values["firstName"].ToString();
                        }

                        string lastName = profile.lastName;
                        if (values.ContainsKey("lastName") && values["lastName"] != null)
                        {
                            lastName = values["lastName"].ToString();
                        }
                        
                        var password = GeneratePassword(4, 4, 4);

                        account.SignUp(appName, firstName, lastName, email, null, false, password, password, false, values, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                    }
                    else
                    {
                        return Redirect(GetErrorUrl(returnAddress, "The user does not belong to " + appName, provider));
                    }
                }
                else
                {
                    return Redirect(GetErrorUrl(returnAddress, "missing activity", provider));
                }
                {
                    string email = profile.email;
                    string appName = profile.appName;
                    
                    //login the user use email
                    //string returnUrl = LoginOrRegister(mode, email, name, "Google", returnUrl);
                    var identity = new ClaimsIdentity("Bearer");
                    identity.AddClaim(new Claim("username", email));
                    identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, provider));
                    identity.AddClaim(new Claim("appname", appName));


                    // create token
                    string AccessToken = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetTmpUserGuidFromGuid(Account.GetUserGuid(email));//CreateToken(identity);

                    // return token

                    string url = GetSuccessUrl(returnAddress, AccessToken, appName, email);
                    return Redirect(url);
                }
            }
            catch (Exception exception)
            {
                Map.Logger.Log("user", "signin", exception.Source, exception, 1, null);
                if (string.IsNullOrEmpty(returnAddress))
                {
                    returnAddress = GetReturnAddress();
                }

                if (!string.IsNullOrEmpty(returnAddress))
                {
                    return Redirect(GetErrorUrl(returnAddress, exception.Message, provider));
                }
                else
                {
                    return BadRequest(exception.Message);
                }
            }
        }

        private string GetAccessToken(string username, string appName, string provider)
        {
            var identity = new ClaimsIdentity("Bearer");
            identity.AddClaim(new Claim("username", username));
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, provider));
            identity.AddClaim(new Claim("appname", appName));


            // create token
            return CreateToken(identity);
        }

        private string GetReturnAddress()
        {
            string state = System.Web.HttpContext.Current.Request.QueryString["state"];
            if (state == null)
                return null;

            var jss = new JavaScriptSerializer();
            try
            {
                Dictionary<string, object> stateObject = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(state);
                return stateObject["returnAddress"].ToString().Split('?').FirstOrDefault().TrimEnd('/');
            }
            catch
            {
                return null;
            }
        }

        private string GetSuccessUrl(string url, string accessToken, string appName, string email)
        {
            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appName);
            string role = map.Database.GetUserRole(email);
            string userId = map.Database.GetUserID(email).ToString();
            if (!url.Contains("#/"))
                url += "#/";
            if (url.Contains('?')) // already have query string
            {
                url += "&";
            }
            else
            {
                url += "?";
            }
            return url + "data={\"access_token\":\"" + accessToken + "\",\"token_type\":\"bearer\",\"expires_in\":86399,\"appName\":\"" + appName + "\",\"username\":\"" + email + "\",\"role\":\"" + role + "\",\"userId\":\"" + userId + "\"}";
        }

        private string GetErrorUrl(string url, string message, string provider)
        {
            if (url.Contains('?')) // already have query string
            {
                url += "&";
            }
            else
            {
                url += "?";
            }
            return url + "error={\"message\":\"" + message + "\",\"provider\":\"" + provider + "\"}";
        }

        /*
        // GET api/Account/socialSignin
        [OverrideAuthentication]
        [HostAuthentication(Startup.ExternalCookieAuthenticationType)]
        [AllowAnonymous]
        [Route("socialSigninOwin")]
        [HttpGet]
        public async Task<IHttpActionResult> socialSigninOwin(string provider, string appName, string returnAddress)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            Social.ExternalLoginData externalLogin = Social.ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            var result = Request.GetOwinContext().Authentication.AuthenticateAsync(Startup.ExternalCookieAuthenticationType).Result;
            //            Authentication.SignOut(Startup.ExternalCookieAuthenticationType);

            string email = ExtractEmailAddress(result.Identity);

            if (email != null &&
                !string.IsNullOrWhiteSpace(appName) &&
                !string.IsNullOrWhiteSpace(returnAddress) &&
                (new DuradosAuthorizationHelper().IsAppExists(appName) || appName == Maps.DuradosAppName))
            {
                // check if user belongs to app
                DataRow userRow = null;
                if (appName == Maps.DuradosAppName)
                {
                    userRow = Maps.Instance.DuradosMap.Database.GetUserRow(email);
                    if (userRow == null)
                    {
                        return BadRequest("The user is not signed up to " + appName);
                    }
                }
                else
                {
                    userRow = Maps.Instance.GetMap(appName).Database.GetUserRow(email);
                    if (userRow == null || (!userRow.IsNull("IsApproved") && !(bool)userRow["IsApproved"]))
                    {
                        return BadRequest("The user is not signed up to " + appName);
                    }
                }
                

                var identity = new ClaimsIdentity("Bearer");
                identity.AddClaim(new Claim("username", email));
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, provider));
                identity.AddClaim(new Claim("appname", appName));

                
                // create token
                string AccessToken = CreateToken(identity);

                // return token
                if (returnAddress.Contains('?')) // already have query string
                {
                    returnAddress += "&";
                }
                else
                {
                    returnAddress += "?";
                }

                return Redirect(returnAddress + "token=" + AccessToken);
            }
            else //validation problem
            {
                return BadRequest("Email and appname must be valid");
            };
        }

        // GET api/Account/socialSignup
        [OverrideAuthentication]
        [HostAuthentication(Startup.ExternalCookieAuthenticationType)]
        [AllowAnonymous]
        [Route("socialSignupOwin")]
        [HttpGet]
        public async Task<IHttpActionResult> socialSignupOwin(string provider, string appName, string returnAddress, string parameters = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            System.Web.HttpContext.Current.Items.Add(Durados.Database.AppName, appName);
            if (!System.Web.HttpContext.Current.Items.Contains(Durados.Database.RequestId))
                System.Web.HttpContext.Current.Items.Add(Durados.Database.RequestId, Guid.NewGuid().ToString());

            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            Social.ExternalLoginData externalLogin = Social.ExternalLoginData.FromIdentity(claimsIdentity);
            var result = Request.GetOwinContext().Authentication.AuthenticateAsync(Startup.ExternalCookieAuthenticationType).Result;
            //            Authentication.SignOut(Startup.ExternalCookieAuthenticationType);

            string email = ExtractEmailAddress(result.Identity);

            if (email != null &&
                !string.IsNullOrWhiteSpace(appName) &&
                !string.IsNullOrWhiteSpace(returnAddress) &&
                (new DuradosAuthorizationHelper().IsAppExists(appName) || appName == Maps.DuradosAppName))
            {
                // check if user belongs to app
                DataRow userRow = null;
                if (appName == Maps.DuradosAppName)
                {
                    userRow = Maps.Instance.DuradosMap.Database.GetUserRow(email);
                }
                else
                {
                    userRow = Maps.Instance.GetMap(appName).Database.GetUserRow(email);
                }
                if (userRow != null)
                {
                    return BadRequest("The user already signed up to " + appName);
                }
                

                Dictionary<string, object> values = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(parameters) && parameters != "parameters")
                {
                    try
                    {
                        values = Durados.Web.Mvc.Controllers.Api.JsonConverter.Deserialize(parameters);
                    }
                    catch (Exception exception)
                    {
                        Log(appName, exception, 1);
                        return BadRequest("Failed to get parameters");
                    }
                }
                if (!values.ContainsKey("socialProfile"))
                {
                    values.Add("socialProfile", claimsIdentity.Claims.ToList());
                }

                try
                {
                    CallActionBeforeSignup(appName, email, claimsIdentity, values);
                }
                catch (Exception exception)
                {
                    Log(appName, exception, 1);
                    return BadRequest("Failed to run beforeSocialSignup action");
                }

                Account account = new Account(this);

                string firstName = ExtractFirstName(claimsIdentity);
                if (values.ContainsKey("firstName") && values["firstName"] != null)
                {
                    firstName = values["firstName"].ToString(); 
                }
                if (firstName == null)
                {
                    firstName = email.Split('@').FirstOrDefault();
                }

                string lastName = ExtractLastName(claimsIdentity);
                if (values.ContainsKey("lastName") && values["lastName"] != null)
                {
                    lastName = values["lastName"].ToString();
                }
                if (lastName == null)
                {
                    lastName = string.Empty;
                }

                var password = GeneratePassword(4, 4, 4);

                Durados.Web.Mvc.UI.Helpers.Account.SignUpResults signUpResults = account.SignUp(appName, firstName, lastName, email, null, false, password, password, false, values, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                var response = new { username = signUpResults.Username, currentStatus = (int)signUpResults.Status, message = account.GetSignUpStatusMessage(signUpResults.Status), listOfPossibleStatus = account.GetListOfPossibleStatus() };
                


                var identity = new ClaimsIdentity("Bearer");
                identity.AddClaim(new Claim("username", email));
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, provider));
                identity.AddClaim(new Claim("appname", appName));

                // create token
                string AccessToken = CreateToken(identity);

                // return token
                if (returnAddress.Contains('?')) // already have query string
                {
                    returnAddress += "&";
                }
                else
                {
                    returnAddress += "?";
                }

                return Redirect(returnAddress + "token=" + AccessToken);
            }
            else //validation problem
            {
                return BadRequest("Email and appname must be valid");
            };
        }
        */
        private string GeneratePassword(int lowercase, int uppercase, int numerics)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);

        }

        private void CallActionBeforeSignup(string appName, string username, Social.Profile profile, Dictionary<string, object> values)
        {
            Map map = Maps.Instance.GetMap(appName);
            Durados.View view = map.Database.GetUserView();
            if (view == null)
                throw new Durados.DuradosException("user view not found");



            values.Add("firstName".AsToken(), profile.firstName);
            values.Add("lastName".AsToken(), profile.lastName);
            values.Add("email".AsToken(), username);

            if (map.HasRule("beforeSocialSignup"))
            {
                Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();
                wfe.PerformActions(this, view, Durados.TriggerDataAction.OnDemand, values, null, null, map.Database.ConnectionString, -1, null, null, "beforeSocialSignup");
            }
        }

        /*
        private void CallActionBeforeSignup(string appName, string username, ClaimsIdentity claimsIdentity, Dictionary<string, object> values)
        {
            Map map = Maps.Instance.GetMap(appName);
            Durados.View view = map.Database.GetUserView();
            if (view == null)
                throw new Durados.DuradosException("user view not found");



            values.Add("firstName".AsToken(), ExtractFirstName(claimsIdentity));
            values.Add("lastName".AsToken(), ExtractLastName(claimsIdentity));
            values.Add("email".AsToken(), username);

            if (map.HasRule("beforeSocialSignup"))
            {
                Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();
                wfe.PerformActions(this, view, Durados.TriggerDataAction.OnDemand, values, null, null, map.Database.ConnectionString, -1, null, null, "beforeSocialSignup");
            }
        }

        private static string ExtractEmailAddress(ClaimsIdentity result)
        {
            var claims = result.Claims.ToList().FirstOrDefault(a => a.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            if (claims != null)
            {
                return claims.Value;
            }

            return null;
        }

        private static string ExtractFirstName(ClaimsIdentity result)
        {
            var claims = result.Claims.ToList().FirstOrDefault(a => a.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            if (claims != null)
            {
                return claims.Value;
            }

            return null;
        }

        private static string ExtractLastName(ClaimsIdentity result)
        {
            var claims = result.Claims.ToList().FirstOrDefault(a => a.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");
            if (claims != null)
            {
                return claims.Value;
            }

            return null;
        }
        */

        private static string CreateToken(ClaimsIdentity identity)
        {
            AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            var currentUtc = new SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(1440));
            string AccessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            return AccessToken;
        }
    }
}