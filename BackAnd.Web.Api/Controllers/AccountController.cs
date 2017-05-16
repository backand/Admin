//﻿using Backand.Web.Api;
using BackAnd.Web.Api.Models;
using Durados.Web.Mvc.Logging;
//using BackAnd.Web.Api.Providers;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;


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


        protected virtual Dictionary<string, object> changePassword(string newPassword, string confirmPassword, string userSysGuid)
        {
            return ForgotPassword(newPassword, confirmPassword, userSysGuid);
        }

        Durados.Web.Mvc.Controllers.AccountMembershipService MembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();

        private bool ValidateNewPassword(string newPassword, string confirmPassword)
        {
            if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(System.Globalization.CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate("The new password and confirmation password do not match."));
            }

            return ModelState.IsValid;
        }

        private Dictionary<string, string> userData;
        private string GetUserDetail(string guid, string userField)
        {
            if (userData == null)
                LoadUserData(guid);
            //return GetUserDetailsFromGuid(guid, userField);
            if (userData == null)
                throw new Durados.DuradosException("User does not exist.");
            if (userData.ContainsKey(userField))
                return userData[userField];
            else
                return string.Empty;


        }

        private string GetUserFieldsForSelect()
        {
            string select;
            select = string.Format("[{0}],[{1}],[{2}],[{3}],[{4}]", Map.Database.UserGuidFieldName, Map.Database.UsernameFieldName, "FirstName", "LastName", "Email");

            return select;
        }
        protected string UsernameNotfinishRegister
        {
            get { return Map.Database.Localizer.Translate("User didn't finish registration. Please follow the email instruction in order to finish registration."); }
        }

        protected string UsernameNotExistsMessage
        {
            get { return Map.Database.Localizer.Translate("Username dose not exists."); }
        }
        protected string PWResetNotAllowedMessage
        {
            get { return Map.Database.Localizer.Translate("Password reset is not allowed\r\n"); }
        }
        private void LoadUserData(string guid)
        {
            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@guid", guid);

            string sqlDuradosSys = string.Format("SELECT TOP 1 username FROM durados_user WITH(NOLOCK)  WHERE guid=@guid");

            object duradosSysUser = sqlAccess.ExecuteScalar(Durados.Web.Mvc.Maps.Instance.ConnectionString, sqlDuradosSys, parameters);

            if (duradosSysUser == null || duradosSysUser == DBNull.Value)
                throw new Durados.DuradosException(Map.Database.Localizer.Translate("Username has no uniqe guid ,password canot be reset."));

            parameters.Clear();

            parameters.Add("@username", duradosSysUser.ToString());

            string sql = string.Format("SELECT TOP 1 {0} FROM {1} WITH(NOLOCK)  WHERE {2}=@username", GetUserFieldsForSelect(), Map.Database.UserViewName, Map.Database.UsernameFieldName);

            object dataTable = sqlAccess.ExecuteTable(Map.Database.GetUserView().ConnectionString, sql, parameters, System.Data.CommandType.Text);

            if (dataTable == null || dataTable == DBNull.Value)
                throw new Durados.DuradosException(Map.Database.Localizer.Translate("Username has no uniqe guid ,password canot be reset."));
            if (((System.Data.DataTable)dataTable).Rows.Count <= 0)
                throw new Durados.DuradosException(UsernameNotExistsMessage);
            userData = new Dictionary<string, string>();
            System.Data.DataRow row = ((System.Data.DataTable)dataTable).Rows[0];

            foreach (System.Data.DataColumn col in row.Table.Columns)
            {
                userData.Add(col.ColumnName, row[col.ColumnName].ToString());
            }
        }

        private string ChangePasswordAfterForgot(string guid, out string username)
        {
            username = GetUserDetail(guid, Map.Database.UsernameFieldName);// Map.Database.GetUsernameByGuid(guid);//GetUserDetailsFromGuid(guid, "[" + Map.Database.UserViewName + "].[" + Map.Database. + "]");
            return MembershipService.ResetPassword(username);

        }

        protected virtual Dictionary<string, object> ForgotPassword(string newPassword, string confirmPassword, string userSysGuid)
        {
            string usernameForgot = null;
            string currentPassword = null;

            if (string.IsNullOrEmpty(userSysGuid))
            {
                return new Dictionary<string, object>() { { "success", false }, { "message", "missing user identification" } };

            }
            if (string.IsNullOrEmpty(confirmPassword))
            {
                return new Dictionary<string, object>() { { "success", false }, { "message", String.Format(System.Globalization.CultureInfo.CurrentCulture,
                        "You must specify a new password of {0} or more characters.",
                        MembershipService.MinPasswordLength) } };

            }

            if (!ValidateNewPassword(newPassword, confirmPassword))
                return new Dictionary<string, object>() { { "success", false }, { "message", "Passwords do not match." } };

            userSysGuid = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetUserGuidFromTmpGuid(userSysGuid);
            if (string.IsNullOrEmpty(userSysGuid))
            {
                return new Dictionary<string, object>() { { "success", false }, { "message", "User identification is invalid." } };
            }
            string guid = GetUserDetail(userSysGuid, Map.Database.UserGuidFieldName);// GetUserDetailsFromGuid(userSysGuid, "[" + Map.Database.UserViewName + "].[" + Map.Database.UserGuidFieldName + "]");
            if (string.IsNullOrEmpty(guid))// &&  guid.Equals(userSysGuid)
            {
                return new Dictionary<string, object>() { { "success", false }, { "message", "User data is invalid." } };

            }



            string errorMessage = Map.Database.Localizer.Translate("The current password is incorrect or the new password is invalid.");
            currentPassword = ChangePasswordAfterForgot(userSysGuid, out usernameForgot);
            try
            {
                string username = usernameForgot ?? User.Identity.Name;
                if (MembershipService.ChangePassword(username, currentPassword, newPassword, true))
                {

                    MembershipService.UnlockUser(username);
                    return new Dictionary<string, object>() { { "success", true }, { "message", "Your password has been changed successfully." } };
                }
                else
                {

                    ModelState.AddModelError("_FORM", errorMessage);
                    return new Dictionary<string, object>() { { "success", false }, { "message", errorMessage } };
                }
            }
            catch
            {
                ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate("The current password is incorrect or the new password is invalid."));
                return new Dictionary<string, object>() { { "success", false }, { "message", errorMessage } };
            }

        }

        [Route("changePassword")]
        [HttpPost]
        public virtual IHttpActionResult changePassword()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("+", "%2B"));
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
            return PasswordReset(username);
        }

        private string GetUserGuid(string userName)
        {
            try
            {
                Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("@username", userName);
                string userViewName = Map.Database.UserViewName;
                object guid = sql.ExecuteScalar(Durados.Web.Mvc.Maps.Instance.DuradosMap.connectionString, "SELECT TOP 1 [durados_user].[guid] FROM durados_user WITH(NOLOCK)  WHERE [durados_user].[username]=@username", parameters);

                if (guid == null || guid == DBNull.Value)
                    throw new Durados.DuradosException(Map.Database.Localizer.Translate("Username has no uniqe guid ,password canot be reset."));

                return guid.ToString();
            }
            catch (Exception ex)
            {
                throw new Durados.DuradosException("User guid was not found.", ex);
            }

        }

        private string GetFullName(string userSysGuid)
        {
            //return Map.Database.GetUserFullName();
            //return GetUserDetailsFromGuid(userSysGuid,"["+Map.Database.UserViewName+"].[FirstName]+' '+["+Map.Database.UserViewName+"].[LastName]");
            return GetUserDetail(userSysGuid, "FirstName") + " " + GetUserDetail(userSysGuid, "LastName");
        }


        protected virtual void SendPasswordResetEmail(Dictionary<string, string> collection)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            string subject = collection["Subject"] ?? string.Empty;
            string message = collection["Message"] ?? string.Empty;

            //message += collection["Comments"];
            string to = collection["to"] ?? string.Empty;
            //string cc = collection["cc"] ?? string.Empty;

            //string siteWithoutQueryString = GetCloudServiceUrl();//Durados.Web.Mvc.Maps.GetAppUrl(Durados.Web.Mvc.Maps.GetCurrentAppName()); //System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;  

            Durados.Web.Mvc.View userView = (Durados.Web.Mvc.View)Map.Database.GetUserView();

            message = message.Replace("[username]", collection["username"]);
            message = message.Replace("[fullname]", GetFullName(collection["Guid"]), false);
            message = message.Replace("[Product]", Map.Database.SiteInfo.GetTitle());
            //message = message.Replace("[Url]", siteWithoutQueryString);
            message = message.Replace("[Guid]", collection["Guid"]);

            Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, to.Split(';'), null, null, subject, message, from, null, null, false, Map.Logger);

        }

        protected virtual Dictionary<string, object> PasswordReset(string userName)
        {

            try
            {

                if (!MembershipService.PasswordResetEnabled) throw new Durados.DuradosException(PWResetNotAllowedMessage);

                if (!MembershipService.ValidateUserExists(userName))
                    return new Dictionary<string, object>() { { "error", "error" }, { "message", UsernameNotExistsMessage } };
                    //if (string.IsNullOrEmpty(Map.Database.GetGuidByUsername(userName)))
                    //    return new Dictionary<string, object>() { { "error", "error" }, { "message", UsernameNotExistsMessage } };
                    //else
                    //    return new Dictionary<string, object>() { { "error", "error" }, { "message", UsernameNotfinishRegister } };
                    
                string guid = GetUserGuid(userName);

                //string guid = Map.Database.GetGuidByUsername(userName);

                Dictionary<string, string> collection = new Dictionary<string, string>();

                collection.Add("username", userName);

                string to = GetUserDetail(guid, "Email");//GetUserDetailsFromGuid(guid, "["+Map.Database.UserViewName+"].[email]");
                //string to = Map.Database.GetEmailByUsername(userName);

                if (string.IsNullOrEmpty(to))
                    throw new Durados.DuradosException("Account missing email, reset password email can not be sent.");

                collection.Add("to", to);

                collection.Add("Guid", Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetTmpUserGuidFromGuid(guid.ToString()));

                string subject = System.Web.Mvc.CmsHelper.GetHtml("passwordResetConfirmationSubject");
                string messageKey = string.Empty;
                if (Durados.Web.Mvc.Maps.Instance.GetMap() is Durados.Web.Mvc.DuradosMap)
                    messageKey = "passwordResetConfirmationMessageInSite";
                else
                    messageKey = "passwordResetConfirmationMessage";
                string message = System.Web.Mvc.CmsHelper.GetHtml(messageKey);

                if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
                    throw new Durados.DuradosException("Missing email Content, make sure your database contains content table with required keys.");

                collection.Add("Subject", Map.Database.Localizer.Translate(subject));

                collection.Add("Message", Map.Database.Localizer.Translate(message));

                SendPasswordResetEmail(collection);

                return new Dictionary<string, object>() { { "error", "success" }, { "message", Map.Database.Localizer.Translate("Please check your mailbox - we've sent you an email with a link to reset your password.") } };
            }

            catch (Durados.DuradosException exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), "PasswordReset", exception.Source, exception, 3, null);
                return new Dictionary<string, object>() { { "error", "error" }, { "message", exception.Message } };
            }

        }

        [Route("sendChangePasswordLink")]
        [HttpPost]
        public virtual IHttpActionResult SendChangePasswordLink()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("+", "%2B"));
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
            string appName = paraemeters["@appName"].ToString();
            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appName);
            
            string username = paraemeters["@newUser"].ToString();
            System.Web.Security.MembershipUser user = map.GetMembershipProvider().GetUser(username, true);
            if (user != null)
            {
                if (!user.IsApproved && Durados.Web.Mvc.Maps.MultiTenancy)
                {
                    user.IsApproved = true;
                    System.Web.Security.Membership.UpdateUser(user);

                }


            }

            //string appName = paraemeters["@appName"].ToString();
            //Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap(appName);
            //if (!map.Database.BackandSSO)
            //{
            //    Durados.Web.Mvc.UI.Helpers.AccountService.UpdateIsApproved(username, true, map);
            //}

            return "success";
        }

        private bool IsRegisreationAllowed(Durados.Web.Mvc.Map map)
        {
            return (map.Database.EnableUserRegistration);
        }


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
            Dictionary<string, object> json = new Durados.Web.Mvc.UI.Helpers.AccountService(this).SignUpToBackand(email, password, Durados.Web.Mvc.Maps.SendWelcomeEmail, null, fullName, "100", null);
            Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("Account", "SignUp", "Post", "SignUp call", email, 3, null, DateTime.Now);
            return json;
        }

        [Route("signUp")]
        [HttpPost]
        public virtual IHttpActionResult SignUp()
        {
            try
            {
                string json = Request.Content.ReadAsStringAsync().Result;
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
                    Durados.Web.Mvc.UI.Helpers.AccountService account = new Durados.Web.Mvc.UI.Helpers.AccountService(this);
                    account.InviteAdminAfterSignUp(email);
                    Durados.Web.Mvc.UI.Helpers.Analytics.Log(Durados.Web.Mvc.Logging.ExternalAnalyticsAction.SignedUp, email, new Dictionary<string, object>() {
                        { Durados.Database.AppName, Durados.Web.Mvc.Maps.DuradosAppName }
                        , { Durados.Web.Mvc.Logging.ExternalAnalyticsTraitsKey.name.ToString(), fullName } },
                      SegmentAnalytics.GetPage(), SegmentAnalytics.GetCampaign(), SegmentAnalytics.GetUserAgent());
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
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(1440));
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
