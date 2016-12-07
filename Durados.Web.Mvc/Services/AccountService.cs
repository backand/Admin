using Durados.DataAccess;
using Durados.Web.Mvc.SocialLogin;
using Durados.Web.Mvc.UI.Helpers;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class AccountService
    {
        object controller = null;

        public AccountService(object controller)
        {
            this.controller = controller;
        }

        private Map GetMap(string appName)
        {
            Map map = Maps.Instance.GetMap(appName);

            if (map == null || map is DuradosMap)
                throw new AppDoesNotExistException(appName);

            return map;
        }

        private Map GetDuradosMap()
        {
            return Maps.Instance.DuradosMap;
        }

        protected virtual bool VerifyPassword(string username, string password)
        {
            Durados.Web.Mvc.Controllers.AccountMembershipService accountMembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();
            return accountMembershipService.AuthenticateUser(username, password);
        }

        public string GetEmailBySocialId(string provider, string socialId, int appId)
        {
            View view = GetUserSocialView();


            //int rowCount = -1;
            //DataView dataView = view.FillPage(1, 1, new Dictionary<string, object>() { { "Provider", provider }, { "SocialId", socialId } }, null, null, out rowCount, null, null);

            SqlAccess sa = new SqlAccess();

            string sql = "select UserId from durados_UserSocial where Provider = @Provider and SocialId = @SocialId and AppId = @AppId";

            object scalar = sa.ExecuteScalar(view.ConnectionString, sql, new Dictionary<string, object>() { { "Provider", provider }, { "SocialId", socialId }, { "AppId", appId } });

            if (scalar == null)
            {
                sql = "select UserId from durados_UserSocial where Provider = @Provider and SocialId = @SocialId and AppId is null";

                scalar = sa.ExecuteScalar(view.ConnectionString, sql, new Dictionary<string, object>() { { "Provider", provider }, { "SocialId", socialId } });
            }

            if (scalar == null || scalar.Equals(string.Empty))
            {
                return null;
            }

            string userId = scalar.ToString();

            //if (dataView.Count == 0)
            //{
            //    return null;
            //}

            //if (dataView.Count > 1)
            //{
            //    throw new DuradosException("More than one found!");
            //}

            //string userId = dataView[0]["UserId"].ToString();
            return GetDuradosMap().Database.GetUsernameById(userId);
        }

        private View GetUserSocialView()
        {
            string UserSocialViewName = "durados_UserSocial";
            if (!GetDuradosMap().Database.Views.ContainsKey(UserSocialViewName))
            {
                throw new DuradosException("Missing user social table");
            }
            View view = (View)GetDuradosMap().Database.Views[UserSocialViewName];
            return view;
        }

        public void SetEmailBySocialId(string provider, string socialId, string email, int appId)
        {
            View view = GetUserSocialView();
            int userId = GetDuradosMap().Database.GetUserID(email);
            try
            {
                Dictionary<string, object> values = new Dictionary<string, object>() { { "Provider", provider }, { view.GetFieldByColumnNames("UserId").Name, userId },
                { "SocialId", socialId }, { view.GetFieldByColumnNames("AppId").Name, appId }};
                view.Create(values);
            }
            catch (Exception exception)
            {
                if (IsProviderSocialAndAppAlreadyExists(exception))
                {
                    UpdateSocialByProviderUserAndApp(provider, socialId, userId, appId);
                }
                else
                {
                    throw new DuradosException("Failed to Set email by social id", exception);
                }
            }
        }

        private bool IsProviderSocialAndAppAlreadyExists(Exception exception)
        {
            return exception.Message.Contains("IX_durados_UserSocial_UserId_Provider");
        }

        private void UpdateSocialByProviderUserAndApp(string provider, string socialId, int userId, int appId)
        {
            string pk = GetUserSocialId(provider, userId, appId);
            UpdateUserSocialEmailById(pk, socialId);
        }

        private void UpdateUserSocialEmailById(string pk, string socialId)
        {
            GetUserSocialView().Edit(new Dictionary<string, object>() { { "SocialId", socialId } }, pk, null, null, null, null);
        }

        private string GetUserSocialId(string provider, int userId, int appId)
        {
            View view = GetUserSocialView();
            Dictionary<string, object> values = new Dictionary<string, object>() {{ view.GetFieldByColumnNames("UserId").Name, userId },
                 { "Provider", provider },  { view.GetFieldByColumnNames("AppId").Name, appId }};
            int count;
            DataView dataView = view.FillPage(1, 2, values, null, null, out count, null, null);

            if (count != 1)
            {
                return null;
            }

            return view.GetPkValue(dataView[0].Row);
        }

        private static MembershipProvider _provider = System.Web.Security.Membership.Provider;

        public virtual void ChangePassword(string username, string password)
        {
            _provider.UnlockUser(username);
            string oldPassword = _provider.ResetPassword(username, null);
            _provider.ChangePassword(username, oldPassword, password);
        }

        public virtual void ChangePassword(string username, string newPassword, string oldPassword)
        {
            if (!VerifyPassword(username, oldPassword))
                throw new DuradosException("The old password is incorrect");

            ChangePassword(username, newPassword);
        }

        public static bool IsValidRole(string appName, string role)
        {
            if (role == null)
                return true;

            Map map = Maps.Instance.GetMap(appName);

            if (map == null)
                return false;

            return map.Database.GetRoleRow(role) != null;
        }

        public Dictionary<string, object> SignUpToBackand(string username, string password, string send, string phone, string fullname, string dbtype, string dbother)
        {
            int identity = -1;
            bool DontSend = false;
            try
            {
                Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();
                Dictionary<string, object> parameters = new Dictionary<string, object>();

                string email = username.Trim();

                parameters.Add("@Email", email);
                parameters.Add("@Username", username);

                if (sql.ExecuteScalar(Maps.Instance.DuradosMap.Database.ConnectionString, "SELECT TOP 1 [Username] FROM [durados_User] WHERE [Username]=@Username", parameters) != string.Empty)
                {
                    return new Dictionary<string, object>() { { "Success", false }, { "Message", string.Format("{0} is already signed up.", username) } };
                }

                string email1 = email;
                try
                {
                    email1 = email.Split('@')[0];
                }
                catch { }

                email1 = email1.ReplaceNonAlphaNumeric();
                if (string.IsNullOrEmpty(email1))
                    email1 = email;

                string[] email1arr = email1.Split('_');
                string firstName = string.Empty;
                if (email1arr.Length > 0)
                    firstName = email1arr[0];
                else
                    firstName = email;

                parameters.Add("@FirstName", firstName);
                string lastName = string.Empty;
                if (email1arr.Length > 0)
                    lastName = email1arr[email1arr.Length - 1];
                else
                    lastName = email;
                parameters.Add("@LastName", lastName);

                //Create random Password
                if (string.IsNullOrEmpty(password))
                {
                    password = new Durados.Web.Mvc.Controllers.AccountMembershipService().GetRandomPassword(10);
                }
                parameters.Add("@Password", password);
                string role = "User";
                parameters.Add("@Role", role);

                Guid guid = Guid.NewGuid();
                parameters.Add("@Guid", guid);

                sql.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.ConnectionString, "INSERT INTO [durados_User] ([Username],[FirstName],[LastName],[Email],[Role],[Guid]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid)", parameters, CreateMembershipCallback);

                System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, true);
                if (user != null)
                {
                    if (!user.IsApproved && Maps.MultiTenancy)
                    {
                        user.IsApproved = true;
                        System.Web.Security.Membership.UpdateUser(user);

                    }
                }

                //FormsAuth.SignIn(username, true);

                //identity = Convert.ToInt32(Maps.Instance.DuradosMap.Database.GetUserRow(username)["Id"]);
                //CreatePendingDatabase(identity);

                bool sendEmail = false;
                sendEmail = send != null && send == "true";

                if (sendEmail)
                    SendRegistrationRequest(fullname, lastName, email, guid.ToString(), username, password, Maps.Instance.DuradosMap, DontSend);

                try
                {
                    Durados.Web.Mvc.UI.Helpers.AccountService.UpdateWebsiteUsers(username, identity);
                }
                catch (Exception ex)
                {
                    Maps.Instance.DuradosMap.Logger.Log("account", "SignUpToBackand", "SignUp", ex, 1, "failed to update websiteusercookie with userid");

                }

                //Insert into website users
                try
                {
                    Durados.Web.Mvc.UI.Helpers.AccountService.InsertContactUsUsers(username, fullname, null, phone, 10, int.Parse(dbtype), dbother); //10=welcome email
                }
                catch (Exception ex)
                {
                    Maps.Instance.DuradosMap.Logger.Log("account", "SignUpToBackand", "SignUp", ex, 1, "failed to update websiteuser in ContactUs");

                }

            }
            catch (DuradosException exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("account", "SignUpToBackand", exception.Source, exception, 3, null);
                return new Dictionary<string, object>() { { "Success", false }, { "Message", "The server is busy, please try again later." } };

            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("account", "SignUpToBackand", exception.Source, exception, 1, null);
                //ViewData["confirmed"] = false;
                return new Dictionary<string, object>() { { "Success", false }, { "Message", "The server is busy, please try again later." } };
            }

            return new Dictionary<string, object>() { { "Success", true }, { "Message", "Success" } };
            //return Json(new { Success = true, Message = "Success", identity = identity, DemoDefaults = GetDefaultDemo(identity) });
        }

        protected virtual string CreateMembershipCallback(Dictionary<string, object> paraemeters)
        {
            string username = paraemeters["@Username"].ToString();
            string password = paraemeters["@Password"].ToString();
            string email = paraemeters["@Email"].ToString();
            string role = paraemeters["@Role"].ToString();

            if (String.IsNullOrEmpty(role))
            {
                //System.Web.Security.Membership.Provider.DeleteUser(username, true);
                return Maps.Instance.DuradosMap.Database.Localizer.Translate("Failed to create user, Role is missing");
            }

            System.Web.Security.MembershipCreateStatus status = CreateUser(username, password, email);

            if (status == MembershipCreateStatus.Success)
            {

                //System.Web.Security.Roles.AddUserToRole(username, role);

                System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, true);
                user.IsApproved = false;
                System.Web.Security.Membership.UpdateUser(user);

                return "success";
            }
            else if (status == MembershipCreateStatus.DuplicateUserName)
            {
                return "success";
            }
            else
            {
                return ErrorCodeToString(status);
            }
        }

        protected virtual string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        public virtual SignUpResults SignUp(string appName, string firstName, string lastName, string username, string role, bool byAdmin, string password, string confirmPassword, bool? isSignupEmailVerification, Dictionary<string, object> parameters, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            try
            {
                if (System.Web.HttpContext.Current.Items[Database.Username] == null)
                    System.Web.HttpContext.Current.Items[Database.Username] = username;

                if (!password.Equals(confirmPassword))
                {
                    throw new PasswordConfirmationException();
                }

                if (!IsAppExists(appName) && appName != Maps.DuradosAppName)
                {
                    throw new AppDoesNotExistException(appName);
                }

                bool isPrivate = IsPrivate(appName);

                bool isInvited = IsInvited(username, appName);

                if (isPrivate && !byAdmin)
                {
                    if (!isInvited)
                    {
                        throw new OnlyInvitedUsersAreAllowedException();
                    }
                }

                bool isActive = IsActive(username, appName);

                if (isActive)
                {
                    bool isApproved = IsApproved(username, appName);
                    if (isApproved)
                    {
                        throw new AlreadySignedUpToAppException();
                    }
                    else
                    {
                        SendSignupToAnalytics(appName, username, parameters);
                        return new SignUpResults() { AppName = appName, Username = username, Status = SignUpStatus.PendingAdminApproval };
                    }
                }

                bool isAuthenticated = IsAuthenticated(username, appName);

                bool isPending = true;
                if (!isAuthenticated)
                {
                    AddToAuthenticatedUsers(appName, firstName, lastName, username, password, !isPending, GetDefaultUserRole());
                }
                else
                {
                    isPending = IsPending(username);
                }

                if (!isInvited)
                {
                    Invite(appName, firstName, lastName, username, role, parameters, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
                }

                SignUpResults signUpResults = new SignUpResults() { AppName = appName, Username = username };
                SendSignupToAnalytics(appName, username, parameters);
                if (isPending)
                {
                    if (IsSignupEmailVerification(appName, isSignupEmailVerification))
                    {
                        SendVerificationEmail(username, appName, firstName, lastName,parameters);
                        signUpResults.Status = SignUpStatus.PengingVerification;
                        return signUpResults;
                    }
                    else
                    {
                        return Verified(appName, signUpResults, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback);
                    }
                }
                else
                {
                    Activate(username, appName);

                    bool isManualApprove = IsManualApprove(appName);
                    signUpResults.Status = SignUpStatus.PendingAdminApproval;

                    if (!isManualApprove)
                    {
                        Approve(username, appName, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback);
                        signUpResults.Status = SignUpStatus.Ready;
                    }
                    signUpResults.Redirect = GetRedirectToSignIn(signUpResults.AppName);
                }

                return signUpResults;

            }
            catch (SignUpException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new DuradosException("An unexpected signup exception occurred: " + exception.Message, exception);
            }
        }

        private void SendSignupToAnalytics(string appName, string username, Dictionary<string, object> parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || appName != Maps.DuradosAppName)
                    return;
                string provider = (parameters == null || !parameters.ContainsKey("socialProfile")) ? "self" : (parameters["socialProfile"] as SocialProfile).Provider;

                Analytics.Log(Durados.Web.Mvc.Logging.ExternalAnalyticsAction.SocialSignedUp, username, new Dictionary<string, object>() {
                             { Durados.Web.Mvc.Logging.ExternalAnalyticsTraitsKey.name.ToString(), username }
                            , { Durados.Web.Mvc.Logging.ExternalAnalyticsTraitsKey.provider.ToString(), provider }
                            ,{ Durados.Database.AppName, appName}});
            }
            catch (Exception exception)
            {
                try
                {
                    Maps.Instance.DuradosMap.Logger.Log("Anaytics", "Log-Signup", "SendSignupToAnalytics", exception, 1, null, DateTime.Now);
                }
                catch { }
            }
        }

        private bool IsSignupEmailVerification(string appName, bool? isSignupEmailVerification)
        {
            if (appName == Maps.DuradosAppName)
                return false;
            if (isSignupEmailVerification.HasValue)
                return isSignupEmailVerification.Value;
            Database database = GetMap(appName).Database;
            return database.SignupEmailVerification;
        }

        private bool IsApproved(string username, string appName)
        {
            Database database = GetMap(appName).Database;
            DataRow userRow = database.GetUserView().GetDataRow(database.GetUsernameField(), username, false);
            if (userRow == null)
                return false;
            if (userRow.IsNull("IsApproved"))
                return false;
            return (bool)userRow["IsApproved"];
        }

        protected virtual Durados.Web.Mvc.Workflow.Engine CreateWorkflowEngine()
        {
            return new Durados.Web.Mvc.Workflow.Engine();
        }

        private void SendVerificationEmail(string username, string appName, string firstName, string lastName, Dictionary<string,object> parameters)
        {
            string encrypted = GetVerificationParametersToken(appName, username);
            string token = System.Web.HttpContext.Current.Server.UrlEncode(encrypted);
            Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();

            Map map = GetMap(appName);
            if (map == null || map is DuradosMap)
            {
                throw new AppDoesNotExistException(appName);
            }

            Durados.View view = map.Database.GetUserView();
            if (view == null)
                throw new ViewNotFoundExeption("user");

            DataRow row = map.Database.GetUserRow(username);
            if (row == null)
                throw new UserDoesNotExistException(username);
            Dictionary<string, object> values = new Dictionary<string, object>();
            string id = row["ID"].ToString();
            values.Add("token".AsToken(), token);
            string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + (System.Web.HttpContext.Current.Request.Url.ToString().Contains("backapi") ? "/backapi" : "");
            values.Add("apiPath".AsToken(), siteWithoutQueryString);
            values.Add("appName".AsToken(), appName);
            values.Add("firstName".AsToken(), row.IsNull("FirstName") ? username : row["FirstName"].ToString());
            
            if (parameters != null && parameters.Count > 0)
            {
                foreach (string key in parameters.Keys)
                    if (!values.ContainsKey(key))
                        values.Add(key.AsToken(), parameters[key]);
            }

            wfe.PerformActions(this.controller, view, Durados.TriggerDataAction.OnDemand, values, id, row, map.Database.ConnectionString, Convert.ToInt32(id), row["Role"].ToString(), null, null, "newUserVerification");


            Debug.WriteLine(encrypted);
            Debug.WriteLine(token);
        }

        private void SendApprovalEmail(string username, string appName)
        {
            string token = System.Web.HttpContext.Current.Server.UrlEncode(GetVerificationParametersToken(appName, username));
            Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();

            Map map = GetMap(appName);
            if (map == null || map is DuradosMap)
            {
                throw new AppDoesNotExistException(appName);
            }

            Durados.View view = map.Database.GetUserView();
            if (view == null)
                throw new ViewNotFoundExeption("user");

            DataRow row = map.Database.GetUserRow(username);
            if (row == null)
                throw new UserDoesNotExistException(username);
            Dictionary<string, object> values = new Dictionary<string, object>();
            string id = row["ID"].ToString();
            string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + (System.Web.HttpContext.Current.Request.Url.ToString().Contains("backapi") ? "/backapi" : "");
            values.Add("signInUrl".AsToken(), map.Database.SignInRedirectUrl);
            values.Add("appName".AsToken(), appName);
            values.Add("firstName".AsToken(), row.IsNull("FirstName") ? username : row["FirstName"].ToString());


            wfe.PerformActions(this.controller, view, Durados.TriggerDataAction.OnDemand, values, id, row, map.Database.ConnectionString, Convert.ToInt32(id), row["Role"].ToString(), null, null, "userApproval");


            Debug.WriteLine(token);
        }

        private string GetVerificationParametersToken(string appName, string username)
        {
            Map map = GetMap(appName);
            string userToken = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetTmpUserGuidFromGuid(map.Database.GetGuidByUsername(username));
            return Encrypt(appName, userToken);
        }

        private string Encrypt(string appName, string userToken)
        {
            Map duradosMap = GetDuradosMap();
            VerificationToken token = new VerificationToken
            {
                appName = appName,
                userToken = userToken
            };

            string parameters = Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(token);
            return Durados.Security.CipherUtility.Encrypt<System.Security.Cryptography.AesManaged>(parameters, duradosMap.Database.DefaultMasterKeyPassword, duradosMap.Database.Salt);
        }

        private bool IsManualApprove(string appName)
        {
            Map map = GetMap(appName);
            return map.Database.ApproveNewUsersManually;
        }

        private void Approve(string username, string appName, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            Map map = GetMap(appName);

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("IsApproved", true);

            DataRow row = map.Database.GetUserRow(username);
            if (row == null)
                throw new DuradosException("In RestHelper.Approve could not find user row, GetUserRow, for username: " + username);

            string userId = row["ID"].ToString();

            map.Database.GetUserView().Edit(values, userId, null, null, null, null);//, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);

            //SendApprovalEmail(username, appName);
        }

        protected virtual void Activate(string username, string appName)
        {
            Activate(username, appName, GetDefaultUserRole());
        }

        protected virtual void Activate(string username, string appName, string role)
        {
            Map map = GetMap(appName);
            Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", map.Database.GetUserID(username));
            parameters.Add("@AppId", map.Id);
            parameters.Add("@Role", role);
            sql.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.ConnectionString, "INSERT INTO [durados_UserApp] ([UserId],[AppId],[Role]) VALUES (@UserId,@AppId,@Role)", parameters, null);
        }

        public virtual void ActivateAdmin(string username, string appName)
        {
            bool isAuthenticated = IsAuthenticated(username, appName);

            if (isAuthenticated)
            {
                Activate(username, appName, "Admin");
            }
            else
            {
                InviteAdminBeforeAignUp(username, appName);
            }

        }

        private void InviteAdminBeforeAignUp(string username, string appName)
        {
            SqlAccess sqlAccess = new SqlAccess();

            try
            {
                Map map = GetDuradosMap();
                string appId = GetMap(appName).Id;
                sqlAccess.ExecuteNonQuery(map.connectionString, string.Format("insert into durados_Invite (username, appId) values ('{0}', {1})", username, appId));
            }
            catch (Exception exception)
            {
                throw new DuradosException("Failed to invite admin before signup", exception);
            }
        }

        public static void SendRegistrationRequest(string firstName, string lastName, string email, string guid, string username, string password, Map Map, bool DontSend)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string smtpUsername = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string smtpPassword = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            string subject = Map.Database.Localizer.Translate(CmsHelper.GetHtml("registrationConfirmationSubject"));
            string message = Map.Database.Localizer.Translate(CmsHelper.GetHtml("registrationConfirmationMessage"));

            string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;

            message = message.Replace("[FirstName]", firstName);
            message = message.Replace("[LastName]", lastName);
            message = message.Replace("[Guid]", guid);
            message = message.Replace("[Url]", siteWithoutQueryString);
            message = message.Replace("[Username]", username ?? email);
            message = message.Replace("[Password]", password);
            if (Maps.Skin)
            {
                message = message.Replace("[Product]", Map.Database.SiteInfo.GetTitle());
            }
            else
            {
                message = message.Replace("[Product]", Maps.Instance.DuradosMap.Database.SiteInfo.GetTitle());
            }

            string to = email;



            Durados.Cms.DataAccess.Email.Send(host, Map.Database.UseSmtpDefaultCredentials, port, smtpUsername, smtpPassword, false, to.Split(';'), new string[0], new string[1] { from }, subject, message, from, null, null, DontSend, null, Map.Database.Logger, true);

        }

        /// <summary>
        /// Update the cookie guid for each new user registration
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userId"></param>
        public static void UpdateWebsiteUsers(string username, int userId)
        {
            SqlAccess sqlAccess = new SqlAccess();
            string sql = @"INSERT INTO [website_UsersCookie]([UserId],[CookieGuid],[CreateDate]) 
                            VALUES(@UserId,@CookieGuid,@CreateDate)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            object orgGuid = GetTrackingCookieGuid();
            if (orgGuid != null)
                parameters.Add("@CookieGuid", orgGuid);
            else
                parameters.Add("@CookieGuid", DBNull.Value);
            parameters.Add("@CreateDate", DateTime.Now);
            sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql, parameters, null);

        }

        /// <summary>
        /// Save users that sent email by contact us and asscociate the cookie guid
        /// </summary>
        /// <param name="email"></param>
        /// <param name="fullname"></param>
        /// <param name="comments"></param>
        public static void InsertContactUsUsers(string email, string fullname, string comments, string phone, int requestSubjectId, int? dbType, string dbOther)
        {
            SqlAccess sqlAccess = new SqlAccess();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Email", email);
            if (fullname == null)
                parameters.Add("@FullName", DBNull.Value);
            else
                parameters.Add("@FullName", fullname);
            if (comments == null)
                parameters.Add("@Comments", DBNull.Value);
            else
                parameters.Add("@Comments", comments);
            object orgGuid = GetTrackingCookieGuid();
            if (orgGuid != null)
                parameters.Add("@CookieGuid", orgGuid);
            else
                parameters.Add("@CookieGuid", DBNull.Value);
            if (phone != null)
                parameters.Add("@Phone", phone);
            else
                parameters.Add("@Phone", DBNull.Value);


            parameters.Add("@RequestSubject", requestSubjectId);
            if (dbType == null)
                parameters.Add("@DBtype", DBNull.Value);
            else
                parameters.Add("@DBtype", dbType);
            if (dbOther == null)
                parameters.Add("@DBother", DBNull.Value);
            else
                parameters.Add("@DBother", dbOther);

            sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, "dbo.website_AddEditUser @Email,@FullName,@Comments,@CookieGuid,@Phone,@RequestSubject,@DBtype,@DBother", parameters, null);

        }

        /// <summary>
        /// Retrun the GUID stored in the tracking cookie
        /// </summary>
        /// <returns></returns>
        public static object GetTrackingCookieGuid()
        {
            string cookieTrackingName = "ModuBizTracking";
            System.Web.HttpCookie trackingCookie = System.Web.HttpContext.Current.Request.Cookies[cookieTrackingName];
            if (trackingCookie == null)
                return null;
            return trackingCookie.Values["guid"];

        }

        public virtual void InviteAdminAfterSignUp(string username)
        {
            try
            {
                Map map = GetDuradosMap();
                int userId = GetDuradosMap().Database.GetUserID(username);

                using (SqlConnection connection = new SqlConnection(map.connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        using (SqlCommand command = new SqlCommand())
                        {

                            command.Connection = connection;
                            command.Transaction = transaction;

                            command.CommandText = string.Format("select appId from durados_Invite where username = '{0}'", username);
                            List<string> apps = new List<string>();
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string appId = reader["appId"].ToString();

                                    apps.Add(appId);
                                }
                            }

                            foreach (string appId in apps)
                            {
                                command.CommandText = string.Format("insert into durados_UserApp (UserId, AppId, Role) values ({0},{1},'{2}')", userId, appId, "Admin");
                                command.ExecuteNonQuery();
                            }
                            command.CommandText = string.Format("delete durados_Invite where username = '{0}'", username);
                            command.ExecuteNonQuery();
                            transaction.Commit();

                        }
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { }
                    }

                }

            }
            catch (Exception exception)
            {
                throw new DuradosException("Failed to invite admin after signup", exception);
            }
        }

        protected virtual void Invite(string appName, string firstName, string lastName, string username, string role, Dictionary<string, object> parameters, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            Map map = GetMap(appName);
            if (map is DuradosMap)
                return;

            View userView = (View)map.Database.GetUserView();

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Username", username);
            values.Add("FirstName", firstName);
            values.Add("LastName", lastName);
            values.Add("Email", username);
            values.Add("IsApproved", false);
            values.Add(userView.GetFieldByColumnNames("Role").Name, role ?? GetDefaultUserRole(appName));

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    if (!values.ContainsKey(key))
                        values.Add(key.AsToken(), parameters[key]);
                }
            }

            userView.Create(values, null, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
        }

        protected virtual bool IsPending(string username)
        {
            System.Web.Security.MembershipUser existingUser = System.Web.Security.Membership.Provider.GetUser(username, false);

            if (existingUser == null)
            {
                throw new NotSignedUpToBackandException();
            }

            return !existingUser.IsApproved;
        }

        //protected virtual void AddToAuthenticatedUsers(string appName, string firstName, string lastName, string username, string password, bool isPending)
        //{
        //    string defaultUserRole = role ?? GetDefaultUserRole();
        //    AddToAuthenticatedUsers(appName, firstName, lastName, username, password, !isPending, defaultUserRole);
        //}

        protected virtual string GetDefaultUserRole()
        {
            return "User";
        }

        protected virtual string GetDefaultUserRole(string appName)
        {
            Map map = GetMap(appName);

            string role = map.Database.NewUserDefaultRole;

            if (string.IsNullOrEmpty(role))
            {
                throw new MissingDefaultSignupRoleBackandException();
            }

            return role;
        }

        protected virtual void AddToAuthenticatedUsers(string appName, string firstName, string lastName, string username, string password, bool isApproved, string role)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Username", username);
            parameters.Add("Password", password);
            parameters.Add("FirstName", firstName);
            parameters.Add("LastName", lastName);
            parameters.Add("Email", username);
            parameters.Add("Role", role);
            parameters.Add("Guid", Guid.NewGuid());
            Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();
            sql.ExecuteNonQuery(GetDuradosMap().Database.GetUserView().ConnectionString, "INSERT INTO [" + GetDuradosMap().Database.GetUserView().GetTableName() + "] ([Username],[FirstName],[LastName],[Email],[Role],[Guid]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid)", parameters, AddToAuthenticatedUsersCallback);

        }

        protected virtual string AddToAuthenticatedUsersCallback(Dictionary<string, object> paraemeters)
        {
            CreateMembership(paraemeters["Username"].ToString(), paraemeters["Password"].ToString(), paraemeters["Role"].ToString());
            return "success";
        }
        protected virtual void CreateMembership(string username, string password, string role)
        {

            System.Web.Security.MembershipUser existingUser = System.Web.Security.Membership.Provider.GetUser(username, false);
            if (existingUser != null)
            {
                if (!System.Web.Security.Membership.Provider.ValidateUser(username, password))
                {
                    throw new AlreadySignedUpToBackandException();
                }
            }

            System.Web.Security.MembershipCreateStatus status = CreateUser(username, password, username);

            if (status == MembershipCreateStatus.Success)
            {

                //System.Web.Security.Roles.AddUserToRole(username, role);

                System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, false);
                user.IsApproved = false;
                System.Web.Security.Membership.UpdateUser(user);
            }
            else if (status == MembershipCreateStatus.DuplicateUserName)
            {
            }
            else
            {
                throw new MembershipException(status);
            }
        }

        protected virtual MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            System.Web.Security.Membership.Provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        protected virtual bool IsAuthenticated(string username, string appName)
        {
            return GetDuradosMap().Database.GetUserRow(username) != null;
        }

        protected virtual bool IsActive(string username, string appName)
        {
            if (appName == Maps.DuradosAppName)
                return false;

            Map map = GetMap(appName);

            return Maps.Instance.AppExists(appName, map.Database.GetUserID(username), true).HasValue;
        }

        protected virtual bool IsInvited(string username, string appName)
        {
            if (appName == Maps.DuradosAppName)
            {
                return true;
            }

            Map map = GetMap(appName);
            return map.Database.GetUserRow(username) != null;
        }

        public SignUpResults Verify(string appName, string token, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            SignUpResults signUpResults = null;

            try
            {
                signUpResults = Decrypt(token);
            }
            catch (Exception exception)
            {
                try
                {
                    GetMap(appName).Logger.Log("signup", "verify", string.Empty, exception, 2, string.Empty);
                }
                catch { };
                signUpResults = new SignUpResults() { AppName = appName };
                signUpResults.Redirect = GetRedirectToSignUpWithError(appName) + "&message=" + exception.Message;
                return signUpResults;
            }

            return Verified(appName, signUpResults, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback);
        }

        private SignUpResults Verified(string appName, SignUpResults signUpResults, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            FinishPendingUser(signUpResults.Username);

            if (appName == Maps.DuradosAppName)
            {
                signUpResults.Status = SignUpStatus.Ready;
                return signUpResults;
            }

            Activate(signUpResults.Username, signUpResults.AppName);

            bool isManualApprove = IsManualApprove(signUpResults.AppName);
            signUpResults.Status = SignUpStatus.PendingAdminApproval;

            if (!isManualApprove)
            {
                Approve(signUpResults.Username, signUpResults.AppName, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback);
                signUpResults.Status = SignUpStatus.Ready;
            }

            signUpResults.Redirect = GetRedirectToSignIn(signUpResults.AppName);

            return signUpResults;
        }

        public virtual string GetSignUpStatusMessage(SignUpStatus status)
        {
            if (status == SignUpStatus.PendingAdminApproval)
                return "The user signed up and is now waiting for an administrator approval.";
            else if (status == SignUpStatus.PengingVerification)
                return "The system is now waiting for the user to responed a verification email.";
            else
                return "The user is ready to sign in";
        }

        private string GetRedirectToSignIn(string appName)
        {
            Map map = GetMap(appName);
            return map.Database.SignInRedirectUrl;
        }

        private string GetRedirectToSignUpWithError(string appName)
        {
            Map map = GetMap(appName);
            string url = map.Database.RegistrationRedirectUrl;

            string appendSign = "?";
            if (url.Contains(appendSign))
            {
                appendSign = "&";
            }
            return url + appendSign + "verificationError=true";
        }

        private void FinishPendingUser(string username)
        {
            System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, false);
            user.IsApproved = true;
            System.Web.Security.Membership.UpdateUser(user);
        }

        private SignUpResults Decrypt(string token)
        {
            Map duradosMap = GetDuradosMap();
            string text = null;
            text = duradosMap.Database.Decrypt(token);

            string appName = null;
            string userToken = null;

            Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(text);
          
            appName = (values.ContainsKey("appName") ? values["appName"].ToString() : null);
            userToken = (values.ContainsKey("userToken") ? values["userToken"].ToString() : null);

            if (appName == null)
            {
                throw new TokenDecryptionException("Missing app name");
            }
            if (userToken == null)
            {
                throw new TokenDecryptionException("user token decryption");
            }

            Map map = GetMap(appName);

            string userGuid = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetUserGuidFromTmpGuid(userToken);

            string username = map.Database.GetUsernameByGuid(userGuid);
            if (username == null)
            {
                throw new TokenDecryptionException("temp guid failure");
            }

            return new SignUpResults() { Username = username, AppName = appName };

        }



        protected virtual bool IsPrivate(string appName)
        {
            if (appName == Maps.DuradosAppName)
                return true;
            Map map = GetMap(appName);
            return !map.Database.EnableUserRegistration;
        }

        protected virtual bool IsAppExists(string appName)
        {
            return Maps.Instance.AppInCach(appName) || Maps.Instance.AppExists(appName).HasValue;
        }

        public class SignUpException : RestException
        {
            public SignUpException(string message, Exception innerException) : base(message, innerException) { }
            public SignUpException(Exception exception)
                : base("An unexpected error has occured during the signup.", exception)
            {

            }

            public SignUpException(string message)
                : base(message)
            {

            }
        }

        public class PasswordConfirmationException : SignUpException
        {
            public PasswordConfirmationException()
                : base("The password does not match with the confirmation.")
            {

            }
        }

        public class AppDoesNotExistException : SignUpException
        {
            public AppDoesNotExistException(string appName)
                : base(string.Format("The app {0} does not exist.", appName))
            {

            }
        }

        public class UserDoesNotExistException : SignUpException
        {
            public UserDoesNotExistException(string username)
                : base(string.Format("The user {0} does not exist.", username))
            {

            }
        }

        public class OnlyInvitedUsersAreAllowedException : SignUpException
        {
            public OnlyInvitedUsersAreAllowedException()
                : base("Only invited users are allowed.")
            {

            }
        }

        public class TokenDecryptionException : SignUpException
        {
            public TokenDecryptionException(string message)
                : base("Verification token decryption failure: " + message)
            {

            }
        }


        public class AlreadySignedUpToAppException : SignUpException
        {
            public AlreadySignedUpToAppException()
                : base("The user is already signed up to this app")
            {

            }
        }

        public class AlreadySignedUpToBackandException : SignUpException
        {
            public AlreadySignedUpToBackandException()
                : base("The user is already signed up to this app")
            {

            }
        }

        public class NotSignedUpToBackandException : SignUpException
        {
            public NotSignedUpToBackandException()
                : base("The user is not signed up to backand")
            {

            }
        }

        public class MissingDefaultSignupRoleBackandException : SignUpException
        {
            public MissingDefaultSignupRoleBackandException()
                : base("Missing default signup role")
            {

            }
        }

        public class MembershipException : SignUpException
        {
            public MembershipException(MembershipCreateStatus status)
                : base("Membership failure:" + status.ToString())
            {

            }
        }


        public class SignUpResults
        {
            public string Username { get; set; }
            public string AppName { get; set; }
            public string Redirect { get; set; }

            public SignUpStatus Status { get; set; }
        }

        public enum SignUpStatus
        {
            Ready = 1,
            PengingVerification = 2,
            PendingAdminApproval = 3
        }

        public object[] GetListOfPossibleStatus()
        {
            List<object> statuses = new List<object>();

            foreach (SignUpStatus status in Enum.GetValues(typeof(SignUpStatus)))
            {
                statuses.Add(new { status = status, message = GetSignUpStatusMessage(status) });
            }

            return statuses.ToArray();
        }

        public void SendForgotPasswordToken(string appName, string username)
        {
            Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();
            string guid = GetUserGuid(username);

            string token = SecurityHelper.GetTmpUserGuidFromGuid(guid);

            Map map = Maps.Instance.GetMap(appName);

            Durados.View view = map.Database.GetUserView();
            DataRow row = map.Database.GetUserRow(username);
            if (row == null)
                throw new UserDoesNotExistException(username);

            string mainAppUrl = System.Configuration.ConfigurationManager.AppSettings["forgotPasswordUrl"] ?? Maps.GetAppUrl("www") + "/Account/ChangePassword?id=" + token + "&isReset=true";
            string currentAppUrl = map.Database.ForgotPasswordUrl;
            if (!string.IsNullOrEmpty(currentAppUrl))
                currentAppUrl += "?token=" + token;

            Dictionary<string, object> values = new Dictionary<string, object>();
            string id = row["ID"].ToString();
            values.Add("token".AsToken(), token);
            values.Add("ForgotPasswordUrl".AsToken(), string.IsNullOrEmpty(currentAppUrl) ? mainAppUrl : currentAppUrl);
            values.Add("AppName".AsToken(), appName);
            values.Add("FirstName".AsToken(), row.IsNull("FirstName") ? username : row["FirstName"].ToString());


            wfe.PerformActions(this.controller, view, Durados.TriggerDataAction.OnDemand, values, id, row, map.Database.ConnectionString, Convert.ToInt32(id), row["Role"].ToString(), null, null, "requestResetPassword");

            Debug.WriteLine(token);
        }

        public static string GetUserGuid(string userName)
        {
            try
            {
                Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("@username", userName);
                object guid = sql.ExecuteScalar(Maps.Instance.DuradosMap.connectionString, "SELECT TOP 1 [durados_user].[guid] FROM durados_user WITH(NOLOCK)  WHERE [durados_user].[username]=@username", parameters);

                if (guid == null || guid == DBNull.Value)
                    throw new DuradosException("Username has no unique guid, password canot be reset.");

                return guid.ToString();
            }
            catch (Exception ex)
            {
                throw new DuradosException("User guid was not found.", ex);
            }

        }



        public string ChangePassword(Guid token, string password)
        {
            string userSysGuid = SecurityHelper.GetUserGuidFromTmpGuid(token.ToString());
            if (string.IsNullOrEmpty(userSysGuid))
            {
                throw new DuradosException("User identification is invalid.");
            }
            string username = GetUsername(userSysGuid);
            if (string.IsNullOrEmpty(username))// &&  guid.Equals(userSysGuid)
            {
                throw new DuradosException("User data is invalid.");

            }

            ChangePassword(username, password);

            return username;
        }

        private string GetUsername(string guid)
        {
            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@guid", guid);

            string sqlDuradosSys = string.Format("SELECT TOP 1 username FROM durados_user WITH(NOLOCK)  WHERE guid=@guid");

            return sqlAccess.ExecuteScalar(Maps.Instance.ConnectionString, sqlDuradosSys, parameters);
        }

        public void DeleteUser(string username, string appName)
        {
            if (UserHasApps(username))
                throw new DuradosException("user has apps");

            if (UserBelongToMoreThanOneApp(username, appName))
                throw new DuradosException("user belong to more than one app");



            if (Maps.Instance.DuradosMap.Database.GetUserRow() == null)
                throw new DuradosException("user does not exist");

            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@username", username);


            string sql = string.Format("delete FROM durados_user WHERE [username]=@username");

            if (appName != null && appName != Maps.DuradosAppName)
            {
                Map map = Maps.Instance.GetMap(appName);
                if (map == null || map == Maps.Instance.DuradosMap)
                    throw new DuradosException("app not found");

                sqlAccess.ExecuteNonQuery(map.Database.GetUserView().ConnectionString, sql, parameters, null);
            }
            sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql, parameters, null);

            System.Web.Security.Membership.Provider.DeleteUser(username, true);

        }

        public bool UserBelongToMoreThanOneApp(string username, string appName)
        {
            int id = Maps.Instance.DuradosMap.Database.GetUserID(username);

            int appId = 0;
            if (appName != null && appName != Maps.DuradosAppName)
            {
                Map map = Maps.Instance.GetMap(appName);
                appId = Convert.ToInt32(map.Id);
            }

            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@appid", appId);

            parameters.Add("@userid", id);

            string sql = string.Format("select id FROM durados_userapp WHERE [userid]=@userid and appid<>@appid");

            return !sqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.connectionString, sql, parameters).Equals(string.Empty);



        }

        private bool UserHasApps(string username)
        {
            int id = Maps.Instance.DuradosMap.Database.GetUserID(username);

            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@id", id);

            string sql = string.Format("SELECT TOP 1 id FROM durados_app WITH(NOLOCK)  WHERE creator=@id");

            return !sqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.connectionString, sql, parameters).Equals(string.Empty);
        }

        public class DefaultUsersTable
        {
            public static void HandleFirstTime(Map map)
            {
                HandleCreator(map);
                HandleBackandUsersActions(map);
                HandleModelUsersActions(map);
            }
            public static void HandleCreator(Map map)
            {
                if (IsExist(map))
                {
                    if (!IsContainCreator(map))
                    {
                        AddCreator(map);
                    }
                }
            }

            private static void AddCreator(Map map)
            {
                DataRow creatorRow = map.Database.GetUserRow();
                View usersView = (View)map.Database.Views["users"];
                usersView.Create(new Dictionary<string, object>() { { "email", creatorRow["Username"].ToString() }, { "firstName", creatorRow["FirstName"].ToString() }, { "lastName", creatorRow["LastName"].ToString() }, { "role", creatorRow["Role"].ToString() } });
            }

            private static bool IsContainCreator(Map map)
            {
                View usersView = (View)map.Database.Views["users"];
                int rowCount = 0;
                DataView dataView = usersView.FillPage(1, 1, null, false, null, out rowCount, null, null);
                return rowCount > 0;
            }

            private static bool IsExist(Map map)
            {
                return map.Database.Views.ContainsKey("users");
            }

            public static void HandleBackandUsersActions(Map map)
            {
                if (!IsExist(map))
                {
                    DisableAction(map, "Create My App User");
                    DisableAction(map, "Update My App User");
                    DisableAction(map, "Delete My App User");
                }
            }

            private static string signupBeforeActionFileName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\deployment\signupBeforeAction.js";
            private static string signupAfterActionFileName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\deployment\signupAfterAction.js";

            private static string signupBeforeActionCode = null;
            private static string signupAfterActionCode = null;
            private static string GetSignupActionCode(string signupActionFileName, string signupActionCode)
            {
                if (signupActionCode == null)
                {
                    if (File.Exists(signupActionFileName))
                    {
                        signupActionCode = File.ReadAllText(signupActionFileName);
                    }
                    else
                    {
                        throw new System.IO.FileNotFoundException("The js infrastructure file was not found", signupActionFileName);
                    }
                }
                return signupActionCode;
            }

            public static void HandleModelUsersActions(Map map)
            {
                if (IsExist(map))
                {
                    const string USERS = "users";

                    string code = GetSignupActionCode(signupBeforeActionFileName, signupBeforeActionCode);

                    string whereCondition = "true";

                    Database configDatabase = map.GetConfigDatabase();
                    ConfigAccess configAccess = new DataAccess.ConfigAccess();
                    string userViewPK = configAccess.GetViewPK(USERS, configDatabase.ConnectionString);
                    View ruleView = (View)configDatabase.Views["Rule"];
                    Dictionary<string, object> values = new Dictionary<string, object>();
                    values.Add("Name", "Validate Backand Register User");
                    values.Add("Rules_Parent", userViewPK);
                    values.Add("DataAction", Durados.TriggerDataAction.BeforeCreate.ToString());
                    values.Add("WorkflowAction", Durados.WorkflowAction.JavaScript.ToString());
                    values.Add("WhereCondition", whereCondition);
                    values.Add("Code", code);
                    ruleView.Create(values, null, null, null, null, null);


                    code = GetSignupActionCode(signupAfterActionFileName, signupAfterActionCode);
                    values = new Dictionary<string, object>();
                    values.Add("Name", "Create Backand Register User");
                    values.Add("Rules_Parent", userViewPK);
                    values.Add("DataAction", Durados.TriggerDataAction.AfterCreate.ToString());
                    values.Add("WorkflowAction", Durados.WorkflowAction.JavaScript.ToString());
                    values.Add("WhereCondition", whereCondition);
                    values.Add("Code", code);
                    ruleView.Create(values, null, null, null, null, null);

                }
            }

            private static void DisableAction(Map map, string actionName)
            {
                View ruleView = (View)map.GetConfigDatabase().Views["Rule"];
                View backandUsersView = (View)map.Database.Views["v_durados_User"];
                Durados.Rule rule = backandUsersView.GetRules().Where(r => r.Name.Equals(actionName)).FirstOrDefault();
                if (rule != null)
                {
                    if (rule.WhereCondition == "true")
                    {
                        string id = rule.ID.ToString();

                        ruleView.Edit(new Dictionary<string, object>() { { "WhereCondition", "false" } }, id, null, null, null, null);

                    }
                }
            }
        }
    }
    public class VerificationToken
    {
        public string appName;
        public string userToken;
    }
}
