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


namespace BackAnd.Web.Api.Controllers
{


    [RoutePrefix("1/user")]
    public class userController : wfController
    {
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
        [Route("socialSignin")]
        [HttpGet]
        public async Task<IHttpActionResult> socialSignin(string provider, string appName, string returnAddress)
        {
            Dictionary<string, object> keys = GetGoogleKeys(appName);
            string clientId = keys["GoogleClientId"].ToString();
            string redirectUri = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/1/user/signin/google"; // System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
            string scope = "https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile";

            //var state = Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(new Dictionary<string, object>() { { "appName", appName }, { "returnAddress", System.Web.HttpContext.Current.Server.UrlEncode(returnAddress) } });
            var state = new { appName = appName, returnAddress = System.Web.HttpContext.Current.Server.UrlEncode(returnAddress) };
            var jss = new JavaScriptSerializer();
            string url = string.Format("https://accounts.google.com/o/oauth2/auth?scope={0}&client_id={1}&redirect_uri={2}&response_type=code&access_type=offline&state={3}", scope, clientId, redirectUri, jss.Serialize(state));

            return Redirect(url);
           
        }

        private Dictionary<string, object> GetKeys(string provider, string appName)
        {
            switch (provider.ToLower())
            {
                case "google":
                    return GetGoogleKeys(appName);

                default:
                    return null;
            }
        }

        private Dictionary<string, object> GetGoogleKeys(string appName)
        {
            string GoogleClientId = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
            string GoogleClientSecret = System.Configuration.ConfigurationManager.AppSettings["GoogleClientSecret"];
            Dictionary<string, object> keys = new Dictionary<string, object>();
            keys.Add("GoogleClientId", GoogleClientId);
            keys.Add("GoogleClientSecret", GoogleClientSecret);

            if (appName == Maps.DuradosAppName)
                return keys;

            Map map = Maps.Instance.GetMap(appName);
            if (map == null)
                return keys;

            if (!string.IsNullOrEmpty(map.Database.GoogleClientId))
            {
                keys["GoogleClientId"] = map.Database.GoogleClientId;
            }
            if (!string.IsNullOrEmpty(map.Database.GoogleClientSecret))
            {
                keys["GoogleClientSecret"] = map.Database.GoogleClientSecret;
            }

            return keys;
        }

        [AllowAnonymous]
        [Route("signin/{provider}")]
        [HttpGet]
        public async Task<IHttpActionResult> signin(string provider)
        {
             
            //get the code from Google and request from access token
            string code = System.Web.HttpContext.Current.Request.QueryString["code"];
            string error = System.Web.HttpContext.Current.Request.QueryString["error"];

            if (code == null || error != null)
            {
                return BadRequest(error);
            }
            try
            {
                string urlAccessToken = "https://accounts.google.com/o/oauth2/token";
                string state = System.Web.HttpContext.Current.Request.QueryString["state"];
                var jss = new JavaScriptSerializer();
                Dictionary<string, object> stateObject = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(state);
                if (!stateObject.ContainsKey("appName"))
                {
                    return BadRequest("Could not find the app name");
                }
                string appName = stateObject["appName"].ToString();
                if (!stateObject.ContainsKey("returnAddress"))
                {
                    return BadRequest("Could not find the return address");
                }
                string returnAddress = stateObject["returnAddress"].ToString();

                //build the URL to send to Google
                Dictionary<string, object> keys = GetGoogleKeys(appName);
                string clientId = keys["GoogleClientId"].ToString();
                string clientSecret = keys["GoogleClientSecret"].ToString();

                string redirectUri = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/1/user/signin/google"; // System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
                
                string accessTokenData = string.Format("scope=&code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code", code, clientId, clientSecret, redirectUri);
                string response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(urlAccessToken, accessTokenData);

                //get the access token from the return JSON
                //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                //AuthResponse validateResponse = (AuthResponse)jsonSerializer.Deserialize<AuthResponse>(response);

                Dictionary<string, object> validateResponse = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);

                //get the Google user profile using the access token
                string profileUrl = "https://www.googleapis.com/plus/v1/people/me";
                string profileHeader = "Authorization: Bearer " + validateResponse["access_token"].ToString();
                string profiel = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(profileUrl, profileHeader);

                //get the user email out of goolge profile
                //GoogleProfile googleProfile = (GoogleProfile)jsonSerializer.Deserialize<GoogleProfile>(profiel); ;
                Dictionary<string, object> googleProfile = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(profiel);
                string email = ((Dictionary<string, object>)((object[])googleProfile["emails"])[0])["value"].ToString();
                string name = googleProfile["displayName"].ToString();

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
                    //login the user use email
                    //string returnUrl = LoginOrRegister(mode, email, name, "Google", returnUrl);
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
            catch (Exception exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return BadRequest(exception.Message);
            }

            


        }

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

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
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
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(claimsIdentity);
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