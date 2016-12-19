using BackAnd.Web.Api.Controllers.Filters;
using Durados.Data;
using Durados.Web.Mvc;
using Durados.Web.Mvc.Farm;
using Durados.Web.Mvc.SocialLogin;
using Durados.Web.Mvc.UI.Helpers;
//﻿using Backand.Web.Api;
//using BackAnd.Web.Api.Providers;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace BackAnd.Web.Api.Controllers
{


    [RoutePrefix("1/user")]
    public class userController : wfController
    {
        [HttpDelete]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        [Route("backand")]
        public IHttpActionResult Delete(string username = null)
        {
            try
            {
                if (Maps.Instance.DuradosMap.Database.GetUserRole() != "Developer" && username != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, new { Messsage = Messages.ActionIsUnauthorized }));
                }

                if (username == null)
                {
                    username = Map.Database.GetCurrentUsername();
                }

                AccountService account = new AccountService(this);
                string appName = null;
                if (map != Maps.Instance.DuradosMap)
                {
                    appName = map.AppName;
                }
                account.DeleteUser(username, appName);
                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        [HttpGet]
        [Route("")]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        public IHttpActionResult Get()
        {
            DataRow row = Map.Database.GetUserRow();
            if (row == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "User not found"));
            }
            try
            {
                return Ok(new Dictionary<string, object>() { { "firstName", row.IsNull("firstName") ? null : row["FirstName"].ToString() }, { "lastName", row.IsNull("lastName") ? null : row["lastName"].ToString() }, { "email", row.IsNull("Email") ? null : row["Email"].ToString() }, { "role", row.IsNull("Role") ? null : row["Role"].ToString() } });
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        [HttpGet]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        [Route("key")]
        public IHttpActionResult key(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "username is missing"));
            }

            try
            {
                return Ok(RestHelper.GetUserKey(username));


            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        protected override void BeforeEditInDatabase(Durados.EditEventArgs e)
        {
            if (e.Values.ContainsKey("Guid"))
            {
                if (!e.ColumnNames.Contains("Guid"))
                {
                    e.ColumnNames.Add("Guid");
                }
            }
        }

        [Route("key/reset")]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        [HttpPut]
        public IHttpActionResult reset(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "username is missing"));
            }

            string role = map.Database.GetUserRole(map.Database.GetCurrentUsername());
            if (!(role == "Admin" || role == "Developer"))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, "Only admin can get keys"));

            try
            {
                return Ok(RestHelper.ResetUserKey(username, view_BeforeEditInDatabase));
            }
            catch (Durados.UserNotFoundException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, exception.Message));

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
        [Route("")]
        public virtual IHttpActionResult Post()
        {
            if (!IsAdmin())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));

            string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("+", "%2B"));
            Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

            string role = null;
            if (values.ContainsKey("role"))
            {
                role = values["role"].ToString();
            }

            string appName = System.Web.HttpContext.Current.Items[Durados.Web.Mvc.Database.AppName].ToString();

            if (!AccountService.IsValidRole(appName, role))
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

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected virtual IHttpActionResult AddNewUser(bool? isSignupEmailVerification, string role = null, bool byAdmin = false)
        {
            try
            {
                string json = Request.Content.ReadAsStringAsync().Result;
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
                if (!IsValidEmail(email))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The email is not valid"));
                }

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
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The password is not confirmed"));


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
                AccountService account = new AccountService(this);

                try
                {
                    if (SharedMemorySingeltone.Instance.Contains(appName, SharedMemoryKey.DebugMode))
                    {
                        System.Web.HttpContext.Current.Items[Durados.Workflow.JavaScript.Debug] = true;
                    }

                }
                catch { }

                try
                {
                    Durados.Web.Mvc.UI.Helpers.AccountService.SignUpResults signUpResults = account.SignUp(appName, firstName, lastName, email, role, byAdmin, password, confirmPassword, isSignupEmailVerification, parameters, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                    GetMap(appName).Logger.Log(GetControllerNameForLog(ControllerContext), string.Empty, string.Empty, null, -37, signUpResults.Username + ": " + account.GetSignUpStatusMessage(signUpResults.Status));

                    if (signUpResults.Status == AccountService.SignUpStatus.Ready)
                    {
                        string accessToken = CreateToken(email, appName);

                        var response = new { username = signUpResults.Username, currentStatus = (int)signUpResults.Status, message = account.GetSignUpStatusMessage(signUpResults.Status), listOfPossibleStatus = account.GetListOfPossibleStatus(), token = accessToken };
                        return Ok(response);
                    }
                    else
                    {
                        var response = new { username = signUpResults.Username, currentStatus = (int)signUpResults.Status, message = account.GetSignUpStatusMessage(signUpResults.Status), listOfPossibleStatus = account.GetListOfPossibleStatus() };
                        return Ok(response);
                    }
                }
                catch (Durados.Web.Mvc.UI.Helpers.AccountService.SignUpException exception)
                {
                    Log(appName, exception, 2);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, exception.GetJsonError("signup_error")));
                }
            }
            catch (Exception exception)
            {
                //if (exception.InnerException != null && (exception.InnerException).Message.Contains("users' doesn't exist"))
                //{
                //    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Your objects do not contain a users object. Please set the where condition to false in the following Security & Auth actions: Create My App User, Update My App User and Delete My App User."));
                //}
                //if (exception.InnerException != null && exception.InnerException is Durados.Workflow.WorkflowEngineException)
                //{
                //    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Please check the following Security & Auth actions: Create My App User, Update My App User and Delete My App User."));
                //}
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

            if (string.IsNullOrEmpty(map.Database.SignInRedirectUrl))
                return Request.CreateResponse(HttpStatusCode.NotFound, "SignIn redirect url was not supplied in configuration");
            if (string.IsNullOrEmpty(map.Database.RegistrationRedirectUrl))
                return Request.CreateResponse(HttpStatusCode.NotFound, "SignUp redirect url was not supplied in configuration");

            try
            {
                AccountService account = new AccountService(this);

                try
                {
                    Durados.Web.Mvc.UI.Helpers.AccountService.SignUpResults signUpResults = account.Verify(appName, parameters, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                    var response = Request.CreateResponse(HttpStatusCode.Moved);
                    response.Headers.Location = new Uri(signUpResults.Redirect);
                    return response;
                }
                catch (Durados.Web.Mvc.UI.Helpers.AccountService.SignUpException exception)
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

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("+", "%2B"));
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (!values.ContainsKey("username"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.PreconditionFailed, "The username is missing"));

                }

                if (!values.ContainsKey("password"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.PreconditionFailed, "The password is missing"));

                }
                string username = values["username"].ToString();
                string password = values["password"].ToString();

                if (IsAdmin(username))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.PreconditionFailed, "You cannot change other admins password."));
                }

                if (map is DuradosMap)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Please provide AppName in the header"));
                }

                AccountService account = new AccountService(this);

                if (account.UserBelongToMoreThanOneApp(username, Map.AppName))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.PreconditionFailed, "You cannot change the password to a user that belongs to additional Backand apps, other than yours. In this case, user must reset password using your app UI."));
                }

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
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("+", "%2B"));
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


                AccountService account = new AccountService(this);
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
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("+", "%2B"));
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

                Map map2 = Maps.Instance.GetMap(appName);
                if (map2 == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.AppNotFound));
                }


                if (map.Database.GetUserRow(username) == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "The username is not correct or does not belong to this app."));
                }


                if (string.IsNullOrEmpty(map2.Database.ForgotPasswordUrl))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Reset password url was not supplied in configuration"));


                AccountService account = new AccountService(this);

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
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("+", "%2B"));
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

                AccountService account = new AccountService(this);

                string userName = account.ChangePassword(token, password);

                try
                {
                    int userId = Maps.Instance.DuradosMap.Database.GetUserID(userName);

                    Dictionary<string, string> apps = ((DuradosMap)Maps.Instance.DuradosMap).GetUserApps(userId);
                    foreach (string appName in apps.Values)
                    {
                        RestHelper.ResetUserKey(map.Database.GetCurrentUsername(), null, Maps.Instance.GetMap(appName));
                    }
                }
                catch
                {

                }
                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        [Route("migrate")]
        [HttpPost]
        public IHttpActionResult MigrateUsers(int? pageSize = null)
        {
            if (!IsAdmin())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));

            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("+", "%2B"));
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (values == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "please send the following parameters\n{\"usersObjectName\":\"<object name>\", \"emailFieldName\" : \"<email field name>\", \"firstNameFieldName\" : \"<first name field name>\", \"lastNameFieldName\" : \"<last name field name>\", \"passwordFieldName\" : \"<password field name>\"}"));
                }
                string usersObjectName = null;
                if (!values.ContainsKey("usersObjectName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The usersObjectName is missing"));

                }
                usersObjectName = values["usersObjectName"].ToString();

                string emailFieldName = null;
                if (!values.ContainsKey("emailFieldName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The emailFieldName is missing"));

                }
                emailFieldName = values["emailFieldName"].ToString();

                string firstNameFieldName = null;
                if (!values.ContainsKey("firstNameFieldName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The firstNameFieldName is missing"));

                }
                firstNameFieldName = values["firstNameFieldName"].ToString();

                string lastNameFieldName = null;
                if (!values.ContainsKey("lastNameFieldName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The lastNameFieldName is missing"));

                }
                lastNameFieldName = values["lastNameFieldName"].ToString();

                string passwordFieldName = null;
                if (!values.ContainsKey("passwordFieldName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The passwordFieldName is missing"));

                }
                passwordFieldName = values["passwordFieldName"].ToString();

                UserMigrator userMigrator = new UserMigrator();

                return Ok(userMigrator.Migrate(Map, usersObjectName, emailFieldName, emailFieldName, firstNameFieldName, lastNameFieldName, passwordFieldName, pageSize ?? 1000, GetCurrentBaseUrl()));
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }


        }

        [Route("signout")]
        [HttpGet]
        [HttpPost]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        public IHttpActionResult signout()
        {
            try
            {
                if (!map.Database.EnableTokenRevokation)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Please set Enable Signout to true"));

                }
                revokeToken();

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [Route("socialSignout")]
        [HttpGet]
        [HttpPost]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        public IHttpActionResult socialSignout(string provider, string returnAddress = null)
        {
            try
            {
                if (!map.Database.EnableTokenRevokation)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Please set Enable Signout to true"));

                }
                revokeToken();

                AbstractSocialProvider social = SocialProviderFactory.GetSocialProvider(provider);
                return Redirect(social.GetLogOutRedirectUrl(Maps.Instance.GetMap().AppName, returnAddress));
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private void revokeToken()
        {
            string token = Request.Headers.Authorization.Parameter;
            int day = 1000 * 60 * 60 * 24;
            Durados.Web.Mvc.Farm.SharedMemorySingeltone.Instance.Set(token, "true", day);
        }

        [AllowAnonymous]
        [Route("socialSignin")]
        [HttpGet]
        public async Task<IHttpActionResult> socialSignin(string provider, string appName, string returnAddress = null, string code = null, string email = null, bool signupIfNotSignedIn = false, bool useHashRouting = true)
        {

            AbstractSocialProvider social = SocialProviderFactory.GetSocialProvider(provider);

            string authUrl = null;
            if (code != null)
            {
                SocialProfile profile = social.Authenticate(appName, code, returnAddress);
                SocialSigninInner(profile);
                return Ok(GetAccessToken(profile.email, appName, provider));
            }
            try
            {
                // return Redirect("http://wwww.backand.aaaaaaaaa?message={'hello' : 'world'}");
                authUrl = social.GetAuthUrl(appName, returnAddress ?? GetCurrentAddress(), null, "signin", email, signupIfNotSignedIn, useHashRouting);
                return Redirect(authUrl);
            }
            catch (Exception exception)
            {
                Map.Logger.Log("user", "socialSignin", exception.Source, exception, 1, authUrl);
                return BadRequest(exception.Message);
            }

        }

        private string GetCurrentAddress()
        {
            if (System.Web.HttpContext.Current.Request.UrlReferrer == null)
                return string.Empty;
            return System.Web.HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
        }

        [AllowAnonymous]
        [Route("socialSignup")]
        [HttpGet]
        public async Task<IHttpActionResult> socialSignup(string provider, string appName, string returnAddress = null, string parameters = null, string email = null, bool useHashRouting = true)
        {
            try
            {
                AbstractSocialProvider social = SocialProviderFactory.GetSocialProvider(provider);
                return Redirect(social.GetAuthUrl(appName, returnAddress ?? GetCurrentAddress(), parameters, "signup", email, false, useHashRouting));
            }
            catch (Exception exception)
            {
                Map.Logger.Log("user", "socialSignin", exception.Source, exception, 1, null);
                return BadRequest(exception.Message);
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
                providersUrls.Add(new
                {
                    name = provider,
                    signin = "1/user/socialSignin?provider=" + provider + "&appname=" + appName + (string.IsNullOrEmpty(returnAddress) ? "" : "&returnAddress=" + System.Web.HttpContext.Current.Server.UrlEncode(returnAddress)),
                    signup = "1/user/socialSignup?provider=" + provider + "&appname=" + appName + (string.IsNullOrEmpty(returnAddress) ? "" : "&returnAddress=" + System.Web.HttpContext.Current.Server.UrlEncode(returnAddress))
                });

            }

            return Ok(providersUrls);
        }

        [AllowAnonymous]
        [Route("{provider}/token")]
        [HttpGet]
        public async Task<IHttpActionResult> socialToken(string provider, string appName, string accessToken, bool signupIfNotSignedIn = false)
        {
            try
            {
                string returnAddress = null;
                AbstractSocialProvider social = SocialProviderFactory.GetSocialProvider(provider);
                SocialProfile profile = social.GetProfile(appName, accessToken);

                if (profile == null)
                {
                    return BadRequest("can't validate social token of " + provider);
                }

                returnAddress = profile.returnAddress;

                try
                {
                    SocialSigninInner(profile);
                }
                catch (UserNotSignedUpSocialException userNotSignedUpSocialException)
                {
                    if (signupIfNotSignedIn)
                    {
                        SocialSignupInner(profile);
                        SocialSigninInner(profile);
                    }
                    else
                    {
                        return BadRequest(userNotSignedUpSocialException.Message);
                    }
                }

                string email = profile.email;

                // create token
                string AccessToken = CreateToken(email, appName);

                string role, userId;
                int expiration;
                GetNeededParamsForToken(appName, email, out role, out userId, out expiration);

                //var data = "data={\"access_token\":\"" + accessToken + "\",\"token_type\":\"bearer\",\"expires_in\":" + expiration + ",\"appName\":\""
                //+ appName + "\",\"username\":\"" + email + "\",\"role\":\"" + role + "\",\"userId\":\"" + userId + "\"}";

                return Ok(new
                {
                    access_token = AccessToken,
                    token_type = "bearer",
                    expires_in = expiration,
                    appName = appName,
                    username = email,
                    role = role,
                    userId = userId
                });

            }
            catch (Exception e)
            {
                Map.Logger.Log("user", "socialToken", e.Source, e, 1, null);
                return BadRequest(e.Message);
            }

        }

        [Route("{provider}/link")]
        [BackAndAuthorize]
        [HttpPost]
        public async Task<IHttpActionResult> LinkSocialProviderToExistingUser(string provider, [FromBody]SopcialLoginUserData userLoginData)
        {


            if (string.IsNullOrEmpty(userLoginData.code) || userLoginData.code == "undefined") // js is greart...
            {
                return BadRequest("code can't be null or undefiend");
            }

            var username = RestHelper.GetCurrentUsername();

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("user must be logged in to link a social provider");
            }

            try
            {
                AbstractSocialProvider social = SocialProviderFactory.GetSocialProvider(provider);
                SocialProfile profile = social.Authenticate(userLoginData);

                // link
                int appId = Convert.ToInt32(Maps.Instance.GetMap(profile.appName).Id);
                new AccountService(null).SetEmailBySocialId(provider, profile.id, username, appId);

                if (profile == null)
                {
                    return BadRequest("can't validate social code of " + provider);
                }


                return Ok();
            }
            catch (Exception ex)
            {
                // todo: stringBuilder + log!
                throw new SocialException("can't linq user " + username + " to external provider " + provider + " " + ex);
            }
        }

        [AllowAnonymous]
        [Route("{provider}/code")]
        [HttpPost]
        public async Task<IHttpActionResult> socialLoginCode(string provider, [FromBody]SopcialLoginUserData userLoginData)
        {


            if (string.IsNullOrEmpty(userLoginData.code) || userLoginData.code == "undefined") // js is greart...
            {
                return BadRequest("code can't be null or undefiend");
            }

            try
            {
                string returnAddress = null;
                AbstractSocialProvider social = SocialProviderFactory.GetSocialProvider(provider);
                SocialProfile profile = social.Authenticate(userLoginData);

                if (profile == null)
                {
                    return BadRequest("can't validate social code of " + provider);
                }

                string appName = userLoginData.appName;
                returnAddress = profile.returnAddress;

                SocialSigninInner(profile);

                string email = profile.email;

                // create token
                string AccessToken = CreateToken(email, appName);

                string role, userId;
                int expiration;
                GetNeededParamsForToken(appName, email, out role, out userId, out expiration);

                return Ok(new
                {
                    access_token = AccessToken,
                    token_type = "bearer",
                    expires_in = expiration,
                    appName = appName,
                    username = email,
                    role = role,
                    userId = userId
                });

            }
            catch (Exception e)
            {
                Map.Logger.Log("user", "socialToken", e.Source, e, 1, null);
                return BadRequest(e.Message);
            }

        }

        [AllowAnonymous]
        [Route("{provider}/signupCode")]
        [HttpPost]
        public async Task<IHttpActionResult> socialSignupCode(string provider, [FromBody]SocialSignupUserData userLoginData)
        {

            if (string.IsNullOrEmpty(userLoginData.code) || userLoginData.code == "undefined") // js is greart...
            {
                return BadRequest("code can't be null or undefiend");
            }

            try
            {
                var social = SocialProviderFactory.GetSocialProvider(provider);
                SocialProfile profile = social.Authenticate(userLoginData);

                if (profile == null)
                {
                    return BadRequest("can't validate social code of " + provider);
                }


                // change from facebook data to user sent data
                profile.email = userLoginData.userName;
                profile.firstName = string.IsNullOrEmpty(userLoginData.firstName) ? profile.firstName : userLoginData.firstName;
                profile.lastName = string.IsNullOrEmpty(userLoginData.lastName) ? profile.lastName : userLoginData.lastName;

                SocialSignupInner(profile);

                string appName = userLoginData.appName;

                SocialSigninInner(profile);

                string email = profile.email;

                // create token
                string AccessToken = CreateToken(email, userLoginData.appName);

                string role, userId;
                int expiration;
                GetNeededParamsForToken(appName, email, out role, out userId, out expiration);

                return Ok(new
                {
                    access_token = AccessToken,
                    token_type = "bearer",
                    expires_in = expiration,
                    appName = appName,
                    username = email,
                    role = role,
                    userId = userId
                });

            }
            catch (Exception e)
            {
                Map.Logger.Log("user", "socialToken", e.Source, e, 1, null);
                return BadRequest(e.Message);
            }

        }

        [AllowAnonymous]
        [Route("{provider}/auth")]
        [HttpGet]
        public async Task<IHttpActionResult> auth(string provider)
        {
            return authInner(provider);
        }

        private IHttpActionResult authInner(string provider)
        {
            string returnAddress = null;
            try
            {
                AbstractSocialProvider social = SocialProviderFactory.GetSocialProvider(provider);
                SocialProfile profile = social.Authenticate();
                returnAddress = profile.returnAddress;

                try
                {
                    if (SharedMemorySingeltone.Instance.Contains(profile.appName, SharedMemoryKey.DebugMode))
                    {
                        System.Web.HttpContext.Current.Items[Durados.Workflow.JavaScript.Debug] = true;
                    }

                }
                catch { }

                if (profile.activity != "signin" && profile.activity != "signup")
                {
                    return Redirect(GetErrorUrl(returnAddress, "missing activity", provider));
                }

                if (profile.activity == "signin")
                {
                    try
                    {
                        SocialSigninInner(profile);
                    }
                    catch (UserNotSignedUpSocialException userNotSignedUpSocialException)
                    {
                        if (!profile.signupIfNotSignedIn)
                        {
                            return BadRequest(userNotSignedUpSocialException.Message);
                        }
                        else
                        {
                            SocialSignupInner(profile);
                            SocialSigninInner(profile);
                        }

                    }
                }
                else if (profile.activity == "signup")
                {
                    SocialSignupInner(profile);
                }


                //login the user use email
                ClaimsIdentity identity = CreateIdentity(profile.email, profile.appName, provider);

                // create token
                string AccessToken = SecurityHelper.GetTmpUserGuidFromGuid(AccountService.GetUserGuid(profile.email));//CreateToken(identity);

                if (Maps.Instance.GetMap(profile.appName).Database.UseRefreshToken && (provider == SocialProviders.AzureAd.ToString().ToLower() || provider == SocialProviders.Adfs.ToString().ToLower()))
                {
                    int tenMinutes = 1000 * 60 * 10;
                    Durados.Web.Mvc.Farm.SharedMemorySingeltone.Instance.Set(AccessToken, profile.additionalValues["refreshToken"].ToString(), tenMinutes);

                }

                // return token

                string url = GetSuccessUrl(returnAddress, AccessToken, profile.appName, profile.email, profile.useHashRouting);
                return Redirect(url);

            }
            catch (Exception exception)
            {
                try
                {
                    try
                    {
                        Map.Logger.Log("user", "signin", exception.Source, exception, 1, null);
                    }
                    catch { }
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
                catch (Exception e)
                {
                    return BadRequest(e.Message + ": " + e.StackTrace + (e.InnerException != null ? "||| message: " + e.InnerException.Message : string.Empty));

                }
            }
        }

        private static bool GetCanLoginWithProfile(SocialProfile profile)
        {
            return
                            !string.IsNullOrWhiteSpace(profile.appName) &&
                            !string.IsNullOrWhiteSpace(profile.returnAddress) &&
                            (new DuradosAuthorizationHelper().IsAppExists(profile.appName) || profile.appName == Maps.DuradosAppName);
        }

        private static void EnsureUserNotAlreadySingedUp(SocialProfile profile)
        {
            DataRow userRow = null;
            if (profile.appName == Maps.DuradosAppName)
            {
                userRow = Maps.Instance.DuradosMap.Database.GetUserRow(profile.email);
            }
            else
            {
                userRow = Maps.Instance.GetMap(profile.appName).Database.GetUserRow(profile.email);
            }

            if (userRow != null)
            {
                throw new SocialException("The user already signed up to " + profile.appName);
            }
        }

        private Dictionary<string, object> CreateLoginValuesForAction(SocialProfile profile, string appName, string parameters)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(parameters) && parameters != "parameters")
            {
                try
                {
                    values = Durados.Web.Mvc.Controllers.Api.JsonConverter.Deserialize(parameters);
                }
                catch (Exception exception)
                {
                    Log(profile.appName, exception, 1);
                    throw new SocialException("Failed to get parameters");
                }
            }

            if (!values.ContainsKey("socialProfile"))
            {
                values.Add("socialProfile", profile);
            }

            return values;
        }

        private void RegisterUserToMainApp(string email, string appName, string firstName, string lastName)
        {
            Durados.Web.Mvc.UI.Helpers.AccountService account = new Durados.Web.Mvc.UI.Helpers.AccountService(this);
            account.InviteAdminAfterSignUp(email);
            
            AccountService.SendRegistrationRequest(firstName, lastName, email, string.Empty, email, string.Empty, Maps.Instance.DuradosMap, DontSend);
            try
            {
                AccountService.UpdateWebsiteUsers(email, Convert.ToInt32(Maps.Instance.GetMap(appName).Database.GetUserID()));
            }
            catch (Exception ex)
            {
                Maps.Instance.DuradosMap.Logger.Log("user", "SignUp", "SignUp", ex, 1, "failed to update websiteusercookie with userid");

            }

            //Insert into website users
            try
            {
                AccountService.InsertContactUsUsers(email, firstName + " " + lastName, null, string.Empty, 10, 100, null); //10=welcome email
            }
            catch (Exception ex)
            {
                Maps.Instance.DuradosMap.Logger.Log("user", "SignUp", "SignUp", ex, 1, "failed to update websiteuser in ContactUs");

            }
                    
        }

        private string CreateToken(string username, string appName)
        {
            return GetAccessToken(username, appName, "Backand");
        }

        private string GetAccessToken(string username, string appName, string provider)
        {
            ClaimsIdentity identity = CreateIdentity(username, appName, provider);

            // create token
            return CreateToken(identity);
        }

        private static ClaimsIdentity CreateIdentity(string username, string appName, string provider)
        {
            var identity = new ClaimsIdentity("Bearer");
            identity.AddClaim(new Claim("username", username));
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, provider));
            identity.AddClaim(new Claim("appname", appName));
            return identity;
        }

        private static string CreateToken(ClaimsIdentity identity)
        {
            AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());

            var appName = identity.Claims.FirstOrDefault(a => a.Type == "appname").ValueType;

            Durados.Web.Mvc.Map map = Maps.Instance.GetMap();
            //Map map = Maps.Instance.GetMap();

            var currentUtc = new SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            int expiration = map.Database.TokenExpiration;
            if (expiration == 0 || expiration == 8640)
                expiration = 86400;

            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromSeconds(expiration));
            string AccessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            return AccessToken;
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

        private string GetSuccessUrl(string url, string accessToken, string appName, string email, bool useHashRouting)
        {
            string role, userId;
            int expiration;
            GetNeededParamsForToken(appName, email, out role, out userId, out expiration);

            if (!url.Contains("#/") && useHashRouting)
                url += "#/";
            if (url.Contains('?')) // already have query string
            {
                url += "&";
            }
            else
            {
                url += "?";
            }

            var data = "data={\"access_token\":\"" + accessToken + "\",\"token_type\":\"bearer\",\"expires_in\":" + expiration + ",\"appName\":\""
                + appName + "\",\"username\":\"" + email + "\",\"role\":\"" + role + "\",\"userId\":\"" + userId + "\"}";
            return url + data;
        }

        private static void GetNeededParamsForToken(string appName, string email, out string role, out string userId, out int expiration)
        {
            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appName);
            role = map.Database.GetUserRole(email);
            userId = map.Database.GetUserID(email).ToString();
            expiration = map.Database.TokenExpiration;
            if (expiration == 0 || expiration == 8640)
                expiration = 86400;
        }

        private string GetErrorUrl(string url, string message, string provider)
        {
            url = System.Web.HttpUtility.UrlDecode(url);
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

        private void CallActionBeforeSignup(string appName, string username, SocialProfile profile, Dictionary<string, object> values)
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
                wfe.PerformActions(this, view, Durados.TriggerDataAction.OnDemand, values, null, null, map.Database.ConnectionString, -1, null, null, null, "beforeSocialSignup");
            }
        }

        private void SocialSignupInner(SocialProfile profile)
        {

            System.Web.HttpContext.Current.Items.Add(Durados.Database.AppName, profile.appName);

            if (!System.Web.HttpContext.Current.Items.Contains(Durados.Database.RequestId))
                System.Web.HttpContext.Current.Items.Add(Durados.Database.RequestId, Guid.NewGuid().ToString());

            NewRelic.Api.Agent.NewRelic.AddCustomParameter(Durados.Database.RequestId, System.Web.HttpContext.Current.Items[Durados.Database.RequestId].ToString());
            var canLoginWithProfile = GetCanLoginWithProfile(profile);

            if (profile.email == null)
            {
                throw new SocialException("can't signup without email. NO_EMAIL_SOCIAL");
            }

            if (canLoginWithProfile)
            {
                // check if user belongs to app
                EnsureUserNotAlreadySingedUp(profile);
                Dictionary<string, object> values = CreateLoginValuesForAction(profile, profile.appName, profile.parameters);

                try
                {
                    CallActionBeforeSignup(profile.appName, profile.email, profile, values);
                }
                catch (Exception exception)
                {
                    Log(profile.appName, exception, 1);
                    throw new SocialException("Failed to run beforeSocialSignup action");
                }

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

                if (string.IsNullOrEmpty(lastName))
                {
                    lastName = firstName;
                }

                var password = DuradosAuthorizationHelper.GeneratePassword(4, 4, 4);


                // todo: start transaction

                SignUpCommand(profile, values, firstName, lastName, password);

                // todo: stop transaction

                // handle case of bko application
                if (profile.appName == Maps.DuradosAppName)
                {
                    RegisterUserToMainApp(profile.email, profile.appName, firstName, lastName);
                }
            }
            else
            {
                throw new SocialException("The user does not belong to " + profile.appName);
            }

        }

        private void SignUpCommand(SocialProfile profile, Dictionary<string, object> values, string firstName, string lastName, string password)
        {
            AccountService accountService = new AccountService(this);
            accountService.SignUp(profile.appName, firstName, lastName, profile.email, null, false, password,
                password, false, values,
                view_BeforeCreate, view_BeforeCreateInDatabase,
                view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit,
                view_BeforeEdit, view_BeforeEditInDatabase,
                view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

            int appId = Convert.ToInt32(Maps.Instance.GetMap(profile.appName).Id);

            var currentData = accountService.GetEmailBySocialId(profile.Provider, profile.id, appId);

            if (string.IsNullOrEmpty(currentData))
            {
                accountService.SetEmailBySocialId(profile.Provider, profile.id, profile.email, appId);
            }

        }

        public virtual void SocialSigninInner(SocialProfile profile)
        {
            int appId = Convert.ToInt32(Maps.Instance.GetMap(profile.appName).Id);
            var accountService = new AccountService(null);
            var emailFromService = accountService
                                    .GetEmailBySocialId(profile.Provider, profile.id, appId);

            profile.email = emailFromService ?? profile.email;

            if (profile.email == null)
            {
                throw new SocialException("The user is not signed up to " + profile.appName);
            }

            if (GetCanLoginWithProfile(profile))
            {
                // check if user belongs to app
                DataRow userRow = null;
                if (profile.appName == Maps.DuradosAppName)
                {
                    userRow = Maps.Instance.DuradosMap.Database.GetUserRow(profile.email);

                    if (userRow == null)
                    {
                        throw new SocialException("The user is not signed up to " + profile.appName);
                    }
                }
                else
                {
                    userRow = Maps.Instance.GetMap(profile.appName).Database.GetUserRow(profile.email, true);

                    if (userRow == null)
                    {
                        throw new UserNotSignedUpSocialException(profile.appName);
                    }

                    if (!userRow.IsNull("IsApproved"))
                    {
                        object isApproved = userRow["IsApproved"];
                        if (isApproved.Equals(false) || isApproved.Equals(0))
                        {
                            throw new SocialException("The user did not finish signing up to " + profile.appName);
                        }
                    }
                }

                // here we are with a valid user
                // user that already exist in backand, and signup before we add external provider link 
                if (emailFromService == null && profile.email != null)
                {
                    accountService.SetEmailBySocialId(profile.Provider, profile.id, profile.email, appId);
                }
            }
            else
            {
                throw new SocialException("Email, appname and returnAddress must be valid");
            }
        }
    }
}