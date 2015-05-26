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


namespace BackAnd.Web.Api.Controllers
{


    [RoutePrefix("api/Account")]
    public class AccountController : apiController
    {

        public AccountController()
            : this(Startup.OAuthOptions, Startup.CookieOptions)
        {
        }


        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }


        public AccountController(OAuthAuthorizationServerOptions oAuthOptions,
            CookieAuthenticationOptions cookieOptions)
        {
            OAuthOptions = oAuthOptions;
            CookieOptions = cookieOptions;
        }

        public OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public CookieAuthenticationOptions CookieOptions { get; private set; }


        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        [Route("profile")]
        [HttpGet]
        public IHttpActionResult profile()
        {
            var currentUserProfile = Durados.Web.Mvc.UI.Helpers.RestHelper.GetUser(Map.Database);
            return Ok(currentUserProfile);
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult unlock()
        {
            if (!IsAdmin())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));

            Durados.Web.Mvc.Controllers.AccountMembershipService accountMembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();

            string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
            if (string.IsNullOrEmpty(json))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.FieldNameIsMissing));
            }
            Dictionary<string, object> data = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

            if (!data.ContainsKey("username"))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.FieldNameIsMissing));
            }

            string username = data["username"].ToString();

            bool success = accountMembershipService.UnlockUser(username);

            return Ok(new { success = success });
        }

        [HttpGet]

        [AllowAnonymous]
        public async Task<IHttpActionResult> Github(string code, string state)
        {
           var context = new GitHubAuthenticationHandler().ChallengeResult(code, state);

            if(context == null)
            {
                return null;
            }

            var query = HttpUtility.ParseQueryString(context.Properties.RedirectUri);
            
            string email = ExtractEmailAddress(context.Identity);
            var appName = query["appName"];
            var returnAddress = query["returnAddress"];
            var provider = "github";
            
            if (email != null &&
                !string.IsNullOrWhiteSpace(appName) &&
                !string.IsNullOrWhiteSpace(returnAddress) &&
                SimpleAuthorizationServerProvider.IsAppExists(appName))
            {
                var identity = new ClaimsIdentity("Bearer");
                identity.AddClaim(new Claim("username", email));
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, provider));
                identity.AddClaim(new Claim("appname", appName));


                // create token
                string AccessToken = CreateToken(identity);

                // add cookie
                return Redirect(returnAddress + "?token=" + AccessToken);

            }
            else //validation problem
            {
                return BadRequest("Email and appname must be valid");
            };


        }

        protected virtual Dictionary<string, object> changePassword(string newPassword, string confirmPassword, string userSysGuid)
        {
            string data = string.Format("newPassword={0}&confirmPassword={1}&userSysGuid={2}", newPassword, confirmPassword, userSysGuid);
            string url = Durados.Web.Mvc.UI.Helpers.RestHelper.GetAppUrl(Durados.Web.Mvc.Maps.DuradosAppName, Durados.Web.Mvc.Maps.OldAdminHttp) + "/Account/ForgotPassword?" + data;

            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("Account", "changePassword", "Post", "changePassword call", url, 3, null, DateTime.Now);
            string response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(url, data);
            Dictionary<string, object> json = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
            return json;
        }

        [Route("changePassword")]
        [HttpPost]
        public virtual IHttpActionResult changePassword()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);


                if (!values.ContainsKey("password"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The new password is missing"));

                }

                string password = values["password"].ToString();

                if (!values.ContainsKey("confirmPassword"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The confirm password is missing"));

                }

                string confirmPassword = values["confirmPassword"].ToString();

                if (!values.ContainsKey("token"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The token is missing"));

                }

                string token = values["token"].ToString();


                Dictionary<string, object> jsonResponse = changePassword(password, confirmPassword, token);

                if (jsonResponse.ContainsKey("success") && jsonResponse["success"].Equals(true))
                    return Ok();
                else
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, jsonResponse));

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        protected virtual Dictionary<string, object> SendChangePasswordLink(string username)
        {
            string data = string.Format("userName={0}", username);
            string url = Durados.Web.Mvc.UI.Helpers.RestHelper.GetAppUrl(Durados.Web.Mvc.Maps.DuradosAppName, Durados.Web.Mvc.Maps.OldAdminHttp) + "/Account/PasswordReset?" + data;

            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("Account", "SendChangePasswordLink", "Post", "SendChangePasswordLink call", url, 3, null, DateTime.Now);
            string response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(url, data);
            Dictionary<string, object> json = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
            return json;
        }

        [Route("sendChangePasswordLink")]
        [HttpPost]
        public virtual IHttpActionResult SendChangePasswordLink()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);


                if (!values.ContainsKey("username"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "username is missing"));

                }

                string username = values["username"].ToString();


                Dictionary<string, object> jsonResponse = SendChangePasswordLink(username);

                if (jsonResponse.ContainsKey("error") && jsonResponse["error"].Equals("success"))
                    return Ok();
                else
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, jsonResponse));

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [Route("approve")]
        [HttpGet]
        public virtual HttpResponseMessage approve(string parameters)
        {
            try
            {
                Dictionary<string, string> approvalParameters = DecryptApprovalParameters(parameters);
                return Approve(approvalParameters["userToken"], approvalParameters["appName"], approvalParameters["redirect"]);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private Dictionary<string, string> DecryptApprovalParameters(string parameters)
        {
            string text = Durados.Security.CipherUtility.Decrypt<System.Security.Cryptography.AesManaged>(parameters, Durados.Web.Mvc.Maps.Instance.DuradosMap.Database.DefaultMasterKeyPassword, Durados.Web.Mvc.Maps.Instance.DuradosMap.Database.Salt);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (string parameter in text.Split('&'))
            {
                string[] keyValue = parameter.Split('=');
                string key = keyValue[0];
                string val = keyValue[1];
                dictionary.Add(key, val);
            }

            return dictionary;
        }

        protected virtual HttpResponseMessage Approve(string userToken, string appName, string redirect)
        {
            try
            {
                if (string.IsNullOrEmpty(appName))
                {
                    string message = "Could not find app";
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, message);
                }

                Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appName);
                if (map == null)
                {
                    string message = "Could not find app";
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, message);
                }

                bool allowRegistration = IsRegisreationAllowed(map);

                if (!allowRegistration)
                {
                    string message = "To enable Sign Up, please Enable User Registration";
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, message);
                }

                string userGuid = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetUserGuidFromTmpGuid(userToken);

                string username = map.Database.GetUsernameByGuid(userGuid);
                ApproveUser(appName, username);

                var response = Request.CreateResponse(HttpStatusCode.Moved);
                response.Headers.Location = new Uri(redirect);
                return response;
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private void ApproveUser(string appName, string username)
        {

            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appName);
            string mapId = map.Id;
            if (string.IsNullOrEmpty(mapId))
            {
                System.Data.DataRow appRow = Durados.Web.Mvc.Maps.Instance.GetAppRow(appName);
                mapId = appRow != null ? appRow["Id"].ToString() : string.Empty;
            }
            string userId = map.Database.GetUserID(username).ToString();
                
            Dictionary<string, object> parameters2 = new Dictionary<string, object>();
            parameters2.Add("@UserId", userId);
            parameters2.Add("@AppId", mapId);
            Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();
            if (string.IsNullOrEmpty(sql.ExecuteScalar(Durados.Web.Mvc.Maps.Instance.DuradosMap.connectionString, "SELECT TOP 1 [ID] FROM [durados_UserApp] WHERE [UserId]=@UserId AND [AppId]=@AppId", parameters2)))
            {
                parameters2.Add("@newUser", username);
                parameters2.Add("@appName", appName);
                sql.ExecuteNonQuery(Durados.Web.Mvc.Maps.Instance.DuradosMap.Database.ConnectionString, "durados_AssignPendingApps @newUser,@appName", parameters2, AssignPendingAppsCallback);
            }
        }

        protected virtual string AssignPendingAppsCallback(Dictionary<string, object> paraemeters)
        {
            string username = paraemeters["@newUser"].ToString();
            System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, true);
            if (user != null)
            {
                if (!user.IsApproved && Durados.Web.Mvc.Maps.MultiTenancy)
                {
                    user.IsApproved = true;
                    System.Web.Security.Membership.UpdateUser(user);

                }


            }
            return "success";
        }

        private bool IsRegisreationAllowed(Durados.Web.Mvc.Map map)
        {
            return (map.Database.EnableUserRegistration);
        }

        //private string GetApp(string appToken)
        //{
        //    Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

        //    return sqlAccess.ExecuteScalar(Durados.Web.Mvc.Maps.Instance.DuradosMap.connectionString, "select name from durados_app where Guid = @guid", new Dictionary<string, object>() { { "@guid", appToken } });
        //}

        protected virtual Dictionary<string, object> Register(string appName, string firstName, string lastName, string email, string password, string confirmPassword, string defaultRole, bool isApproved)
        {
            string data = string.Format("First_Name={0}&Last_Name={1}&Email={2}&Password={3}&ConfirmPassword={4}&DefaultRole={5}&IsApproves={6}", firstName, lastName, email, password, confirmPassword, defaultRole, isApproved);
            string url = Durados.Web.Mvc.UI.Helpers.RestHelper.GetAppUrl(appName, Durados.Web.Mvc.Maps.OldAdminHttp) + "/DuradosAccount/RegistrationRequest?" + data;
            //string data = string.Format("First_Name={0}&Last_Name={1}&Email={2}&Password={3}&ConfirmPassword={4}&appName={5}", firstName, lastName, email, password, confirmPassword, appName);
            //string url = Durados.Web.Mvc.UI.Helpers.RestHelper.GetAppUrl(Durados.Web.Mvc.Maps.DuradosAppName, Durados.Web.Mvc.Maps.OldAdminHttp) + "/DuradosAccount/RegistrationRequest?" + data;

            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("Account", "Register", "Post", "register call", url, 3, null, DateTime.Now);
            string response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(url, data);
            try
            {
                Dictionary<string, object> json = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
                return json;
            }
            catch
            {
                return new Dictionary<string, object>() { { "error", response } };
            }
        }

        

        //[Route("register")]
        //[HttpPost]
        //[BackAnd.Web.Api.Controllers.Filters.TokenAuthorize(BackAnd.Web.Api.Controllers.Filters.HeaderToken.SignUpToken)]
        //public virtual IHttpActionResult Register()
        //{
        //    try
        //    {
        //        string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
        //        Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

        //        if (!values.ContainsKey("firstName"))
        //        {
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "First name is missing"));

        //        }

        //        string firstName = values["firstName"].ToString();

        //        if (!values.ContainsKey("lastName"))
        //        {
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Last name is missing"));

        //        }
        //        string lastName = values["lastName"].ToString();
        //        if (!values.ContainsKey("email"))
        //        {
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Email is missing"));

        //        }
        //        string email = values["email"].ToString();
               

        //        if (!values.ContainsKey("password"))
        //        {
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Password is missing"));

        //        }
        //        string password = values["password"].ToString();
                
        //        if (!values.ContainsKey("confirmPassword"))
        //        {
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Confirm password is missing"));

        //        }
        //        string confirmPassword = values["confirmPassword"].ToString();
                
        //        if (!password.Equals(confirmPassword))
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The password is is not confirmed"));

        //        string appName = Durados.Web.Mvc.Maps.DuradosAppName;
        //        if (values.ContainsKey("appName"))
        //        {
        //            appName = values["appName"].ToString();

        //        }

        //        if (!appName.Equals(System.Web.HttpContext.Current.Items[Durados.Web.Mvc.Database.AppName]))
        //        {
        //            string message = "The SignUpToken in the header does not match with the appName in the request";
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, message));
        //        }


        //        string redirect = null;
        //        if (values.ContainsKey("redirect"))
        //        {
        //            redirect = values["redirect"].ToString();
        //            if (!(redirect.Equals(Durados.Web.Mvc.Maps.Instance.GetMap(appName).Database.RegistrationRedirectUrl)))
        //            {
        //                string message = "The redirect parameter does not match with the Registration Redirect Url in the configuration";
        //                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, message));

        //            }
        //        }
        //        else
        //        {
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "redirect is missing"));
        //        }

        //        bool allowRegistration = IsRegisreationAllowed(Durados.Web.Mvc.Maps.Instance.GetMap(appName));

        //        if (!allowRegistration)
        //        {
        //            string message = "To enable Sign Up, please Enable User Registration";
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, message));
        //        }

        //        string defaultRole = GetDefaultRole(appName);
        //        bool isApproved = GetIsApproved(appName);

        //        Dictionary<string, object> jsonResponse = Register(appName, firstName, lastName, email, password, confirmPassword, defaultRole, isApproved);

        //        if (jsonResponse.ContainsKey("error") && jsonResponse["error"].Equals("success"))
        //        {
        //            SendRegistrationApproval(firstName, lastName, email, email, appName, redirect);
        //            return Ok();
        //        }
        //        else
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, jsonResponse));

        //    }
        //    catch (Exception exception)
        //    {
        //        throw new BackAndApiUnexpectedResponseException(exception, this);
        //    }
        //}

        private bool GetIsApproved(string appName)
        {
            return !Durados.Web.Mvc.Maps.Instance.GetMap(appName).Database.ApproveNewUsersManually;
            
        }

        private string GetDefaultRole(string appName)
        {
            string role = Durados.Web.Mvc.Maps.Instance.GetMap(appName).Database.NewUserDefaultRole;
            if (string.IsNullOrEmpty(role))
            {
                throw new Durados.DuradosException("Please provide a New User Default Role.");
            }

            if (!IsRoleExists(role))
            {
                throw new Durados.DuradosException("The User Default Role does not match any of the roles in the system.");
            }

            return role;
        }

        private bool IsRoleExists(string role)
        {
            string sql = "SELECT [Name] FROM [durados_UserRole] WITH(NOLOCK)  WHERE [Name] = @role";
            using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(Durados.Web.Mvc.Maps.Instance.DuradosMap.Database.ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, cnn))
                {

                    command.Parameters.AddWithValue("role", role);
                    cnn.Open();
                    object scalar = command.ExecuteScalar();
                    if (scalar == null || scalar == DBNull.Value)
                        return false;
                    return true;
                }
            }
        }

        private void SendRegistrationApproval(string firstName, string lastName, string username, string email, string appName, string redirect)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string smtpUsername = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string smtpPassword = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            string subject = Map.Database.Localizer.Translate(System.Web.Mvc.CmsHelper.GetHtml("registrationConfirmationSubject"));
            string message = Map.Database.Localizer.Translate(System.Web.Mvc.CmsHelper.GetHtml("registrationConfirmationMessage"));

            string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;

            string token = System.Web.HttpContext.Current.Server.UrlEncode(GetApprovalParametersToken(appName, username, redirect));
            message = message.Replace("[FirstName]", firstName);
            message = message.Replace("[LastName]", lastName);
            message = message.Replace("[token]", token);
            message = message.Replace("[Url]", siteWithoutQueryString);
            message = message.Replace("[Username]", username ?? email);
            message = message.Replace("[Product]", Map.Database.SiteInfo.GetTitle());
            
            string to = email;



            Durados.Cms.DataAccess.Email.Send(host, Map.Database.UseSmtpDefaultCredentials, port, smtpUsername, smtpPassword, false, to.Split(';'), new string[0], new string[1] { from }, subject, message, from, null, null, false, null, Map.Database.Logger, true);

        }

        private string GetApprovalParametersToken(string appName, string username, string redirect)
        {
            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appName);
            string userToken = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetTmpUserGuidFromGuid(map.Database.GetGuidByUsername(username));
            return EncryptApprovalParameters(appName, userToken, redirect);
        }
        private string EncryptApprovalParameters(string appName, string userToken, string redirect)
        {
            string parameters = string.Format("appName={0}&userToken={1}&redirect={2}", appName, userToken, redirect);
            return Durados.Security.CipherUtility.Encrypt<System.Security.Cryptography.AesManaged>(parameters, Durados.Web.Mvc.Maps.Instance.DuradosMap.Database.DefaultMasterKeyPassword, Durados.Web.Mvc.Maps.Instance.DuradosMap.Database.Salt);
        }

        protected virtual Dictionary<string, object> SignUp(string fullName, string email, string password)
        {
            string data = string.Format("fullname={0}&username={1}&Password={2}&send=true&phone=&dbtype=100&dbother=", fullName, email, password);
            string url = Durados.Web.Mvc.UI.Helpers.RestHelper.GetAppUrl(Durados.Web.Mvc.Maps.DuradosAppName, Durados.Web.Mvc.Maps.OldAdminHttp) + "/WebsiteAccount/SignUp?" + data;

            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("Account", "SignUp", "Post", "SignUp call", url, 3, null, DateTime.Now);
            string response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(url, data);
            Dictionary<string, object> json = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
            return json;
        }

        [Route("signUp")]
        [HttpPost]
        public virtual IHttpActionResult SignUp()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (!values.ContainsKey("fullName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "fullName is missing"));

                }

                string fullName = values["fullName"].ToString();

                if (!values.ContainsKey("email"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "email is missing"));

                }
                string email = values["email"].ToString();


                if (!values.ContainsKey("password"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "password is missing"));

                }
                string password = values["password"].ToString();

                if (!values.ContainsKey("confirmPassword"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "confirmPassword is missing"));

                }
                string confirmPassword = values["confirmPassword"].ToString();

                if (!password.Equals(confirmPassword))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The password is not confirmed"));


                Dictionary<string, object> jsonResponse = SignUp(fullName, email, password);

                if (jsonResponse.ContainsKey("Message") && jsonResponse["Message"].Equals("Success"))
                {
                    Durados.Web.Mvc.UI.Helpers.Account account = new Durados.Web.Mvc.UI.Helpers.Account(this);
                    account.InviteAdminAfterSignUp(email);
                    return Ok();
                }
                else
                {
                    object message = jsonResponse;
                    if (jsonResponse.ContainsKey("Message"))
                    {
                        message = jsonResponse["Message"].ToString();
                    }
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, message));
                }

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(Startup.ExternalCookieAuthenticationType)]
        [AllowAnonymous]
        [Route("ExternalLogin")]
        [HttpGet]
        public async Task<IHttpActionResult> ExternalLogin(string provider, string appName, string returnAddress)
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
                new DuradosAuthorizationHelper().IsAppExists(appName))
            {
                var identity = new ClaimsIdentity("Bearer");
                identity.AddClaim(new Claim("username", email));
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, provider));
                identity.AddClaim(new Claim("appname", appName));

                // create token
                string AccessToken = CreateToken(identity);

                // return token
                if(returnAddress.Contains('?')) // already have query string
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



        private static string ExtractEmailAddress(ClaimsIdentity result)
        {
            var claims = result.Claims.ToList().FirstOrDefault(a => a.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
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
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));
            string AccessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            return AccessToken;
        }


        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        [HttpGet]
        public IEnumerable<ExternalLoginViewModel> ExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();


            string state;

            if (generateState)
            {
                state = GenerateAntiForgeryState();
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                var nvc = CreateExternalLoginQueryString(state, description);
                var url = "/api/Account/ExternalLogin?" + nvc.ToString();

                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = url,
                };
                logins.Add(login);
            }

            return logins;
        }

        private static System.Collections.Specialized.NameValueCollection CreateExternalLoginQueryString(string state, AuthenticationDescription description)
        {
            var nvc = System.Web.HttpUtility.ParseQueryString(string.Empty);

            nvc["provider"] = description.AuthenticationType;
            nvc["response_type"] = "token";
            nvc["client_id"] = Startup.PublicClientId;
            nvc["redirect_uri"] = "www.google.com";
            nvc["state"] = state;
            return nvc;
        }

        private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();
        private string GenerateAntiForgeryState()
        {
            const int strengthInBits = 256;
            const int strengthInBytes = strengthInBits / 8;
            byte[] data = new byte[strengthInBytes];
            _random.GetBytes(data);
            return Convert.ToBase64String(data);
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }
            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.Claims.FirstOrDefault().Value//FindFirstValue(ClaimTypes.Name)
                };
            }
        }
    }
}
     
        /*[BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
=======
        public AccountController(OAuthAuthorizationServerOptions oAuthOptions,
            CookieAuthenticationOptions cookieOptions)
        {
            OAuthOptions = oAuthOptions;
            CookieOptions = cookieOptions;
        }

        public OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public CookieAuthenticationOptions CookieOptions { get; private set; }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(Startup.ExternalCookieAuthenticationType)]
        [AllowAnonymous]
        [Route("ExternalLogin")]
        [HttpGet]
        public async Task<IHttpActionResult> ExternalLogin(string provider, string appName, string returnAddress)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            var result = Request.GetOwinContext().Authentication.AuthenticateAsync(Startup.ExternalCookieAuthenticationType).Result;
            //            Authentication.SignOut(Startup.ExternalCookieAuthenticationType);

            string email = ExtractEmailAddress(result);

            if (email != null &&
                !string.IsNullOrWhiteSpace(appName) &&
                !string.IsNullOrWhiteSpace(returnAddress) &&
                new DuradosAuthorizationHelper().IsAppExists(appName))
            {
                var identity = new ClaimsIdentity("Bearer");
                identity.AddClaim(new Claim("username", email));
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, provider));
                identity.AddClaim(new Claim("appname", appName));

                // create token
                string AccessToken = CreateToken(identity);

                // add cookie
                return Redirect(returnAddress + "?token=" + AccessToken);
            }
            else //validation problem
            {
                return BadRequest("Email and appname must be valid");
            };
        }



>>>>>>> .r3849
        [System.Web.Http.HttpPost]
        public IHttpActionResult unlock()
        {
            if (!IsAdmin())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));

            Durados.Web.Mvc.Controllers.AccountMembershipService accountMembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();

            string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
            if (string.IsNullOrEmpty(json))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.FieldNameIsMissing));
            }
            Dictionary<string, object> data = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

            if (!data.ContainsKey("username"))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.FieldNameIsMissing));
            }

            string username = data["username"].ToString();

            bool success = accountMembershipService.UnlockUser(username);

            return Ok(new { success = success });
        }
<<<<<<< .mine


     
        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();
        private string GenerateAntiForgeryState()
        {
            const int strengthInBits = 256;
            const int strengthInBytes = strengthInBits / 8;
            byte[] data = new byte[strengthInBytes];
            _random.GetBytes(data);
            return Convert.ToBase64String(data);
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.Claims.FirstOrDefault().Value//FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        #endregion
=======

        private static string ExtractEmailAddress(AuthenticateResult result)
        {
            var claims = result.Identity.Claims.ToList().FirstOrDefault(a => a.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
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
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));
            string AccessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            return AccessToken;
        }


        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [OverrideAuthentication]
        [AllowAnonymous]
        [Route("ExternalLogins")]
        [HttpGet]
        public IEnumerable<ExternalLoginViewModel> ExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();


            string state;

            if (generateState)
            {
                state = GenerateAntiForgeryState();
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {

                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    name = description.Caption,
                    url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = "www.google.com",
                        state = state
                    }),
                    state = state
                };
                logins.Add(login);
            }

            return logins;
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();
        private string GenerateAntiForgeryState()
        {
            const int strengthInBits = 256;
            const int strengthInBytes = strengthInBits / 8;
            byte[] data = new byte[strengthInBytes];
            _random.GetBytes(data);
            return Convert.ToBase64String(data);
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.Claims.FirstOrDefault().Value//FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        #endregion
>>>>>>> .r3849
    }
}

*/