using Durados.DataAccess;
using Durados.Web.Mvc.Logging;
using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using Durados.Web.Mvc.UI.Json;
using System.Configuration;

namespace Durados.Web.Mvc.Controllers
{
 
    [HandleError]
    //[Durados.Web.Mvc.Controllers.Attributes.DynamicAuthorizeAttribute(false, false)]
    public class DuradosAccountController : AccountController//: Durados.Web.Mvc.Controllers.DuradosController
    {
      
       
        public override JsonResult RegistrationRequest(FormCollection collection)
        {
            try
            {
                string appName = this.Request.QueryString["appName"];
                string id = this.Request.QueryString["id"];

                
                Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                
                string email = string.Empty;
                if (string.IsNullOrEmpty(collection["Email"]))
                    throw new DuradosException(Map.Database.Localizer.Translate("Missing 'Email' in the registration form"));
                else
                    email = collection["Email"];
                parameters.Add("@Email", email);

                string username = string.Empty;
                if (string.IsNullOrEmpty(collection["Username"]))
                    username = email;
                else
                    username = collection["Username"];
                parameters.Add("@Username", username);

                //if (!string.IsNullOrEmpty(appName))
                //{
                //    if (string.IsNullOrEmpty(id))
                //    {
                //        return Json("error");
                //    }
                //    else
                //    {
                //        if (Maps.Instance.GetMap(appName).Database.GetUsernameByGuid(id) != username)
                //        {
                //            return Json("error");
                //        }
                //    }
                //}



                if (sql.ExecuteScalar(Map.Database.GetUserView().ConnectionString, "SELECT TOP 1 [Username] FROM [durados_User] WHERE [Username]=@Username", parameters) != string.Empty)
                {
                    throw new DuradosException(Map.Database.Localizer.Translate(username + " already exists"));
                }

                string firstName = string.Empty;
                if (string.IsNullOrEmpty(collection["First_Name"]))
                    throw new DuradosException(Map.Database.Localizer.Translate("Missing 'First Name' in the registration form"));
                else
                    firstName = collection["First_Name"];
                parameters.Add("@FirstName", firstName);

                string lastName = string.Empty;
                if (string.IsNullOrEmpty(collection["Last_Name"]))
                    throw new DuradosException(Map.Database.Localizer.Translate("Missing 'Last Name' in the registration form"));
                else
                    lastName = collection["Last_Name"];
                parameters.Add("@LastName", lastName);

                
                string password = string.Empty;
                if (string.IsNullOrEmpty(collection["Password"]))
                    throw new DuradosException(Map.Database.Localizer.Translate("Missing 'Password' in the registration form"));
                else
                    password = collection["Password"];

                string confirmPassword = string.Empty;
                if (string.IsNullOrEmpty(collection["ConfirmPassword"]))
                    throw new DuradosException(Map.Database.Localizer.Translate("Missing 'Confirm Password' in the registration form"));
                else
                    confirmPassword = collection["ConfirmPassword"];
               
                if (!password.Equals(confirmPassword))
                    throw new DuradosException(Map.Database.Localizer.Translate("'Password' does not equals 'Confirm Password' in the registration form"));

                parameters.Add("@Password", password);

                string role = string.Empty;
                if (collection["DefaultRole"] == null)
                    role = "User";
                else
                    role = collection["DefaultRole"];
                parameters.Add("@Role", role);

                Guid guid = Guid.NewGuid();
                parameters.Add("@Guid", guid);

                bool isApproved = false;
                if (collection["isApproved"] == null)
                    isApproved = false;
                else
                    isApproved = Convert.ToBoolean(collection["isApproved"]);
                parameters.Add("@IsApproved", isApproved);

                collection.Add("Guid", guid.ToString());
                string userId = sql.ExecuteScalar(Maps.Instance.DuradosMap.Database.GetUserView().ConnectionString, "SELECT TOP 1 [ID] FROM [durados_User] WHERE [Username]=@Username", parameters);
                ExecuteNonQueryRollbackCallback executeNonQueryRollbackCallback = CreateMembershipCallback;
                if (!string.IsNullOrEmpty(userId))
                    executeNonQueryRollbackCallback = null;

                sql.ExecuteNonQuery(Map.Database.GetUserView().ConnectionString, "INSERT INTO [" + Map.Database.GetUserView().GetTableName() + "] ([Username],[FirstName],[LastName],[Email],[Role],[Guid],[IsApproved]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid,@IsApproved)", parameters, executeNonQueryRollbackCallback);
                if (string.IsNullOrEmpty(appName) && !Map.IsMainMap)
                {
                    //appName = Map.AppName;
                    
                    if (string.IsNullOrEmpty(userId))
                    {
                        sql.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.GetUserView().ConnectionString, "INSERT INTO [" + Maps.Instance.DuradosMap.Database.GetUserView().GetTableName() + "] ([Username],[FirstName],[LastName],[Email],[Role],[Guid]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid)", parameters, null);
                    }
                    Dictionary<string, object> parameters2 = new Dictionary<string, object>();
                    parameters2.Add("@Username", username);
                    parameters2.Add("@AppId", Map.Id);
                    parameters2.Add("@Role", role);
                    sql.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.ConnectionString, "INSERT INTO [durados_UserApp_Pending] ([Username],[AppId],[Role]) VALUES (@Username,@AppId,@Role)", parameters2, null);
                    
                }

                if (!string.IsNullOrEmpty(appName))
                {

                    string mapId = Map.Id;
                    if (string.IsNullOrEmpty(mapId))
                    {
                        DataRow appRow = Maps.Instance.GetAppRow(appName);
                        mapId = appRow != null ? appRow["Id"].ToString() : string.Empty;
                    }
                    Dictionary<string, object> parameters2 = new Dictionary<string, object>();
                    parameters2.Add("@UserId", userId);
                    parameters2.Add("@AppId", mapId);
                    if (string.IsNullOrEmpty(sql.ExecuteScalar(Maps.Instance.DuradosMap.connectionString, "SELECT TOP 1 [ID] FROM [durados_UserApp] WHERE [UserId]=@UserId AND [AppId]=@AppId", parameters2)))
                    {
                        parameters2.Add("@newUser", username);
                        parameters2.Add("@appName", appName);
                        sql.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.ConnectionString, "durados_AssignPendingApps @newUser,@appName", parameters2, AssignPendingAppsCallback);
                    }
                }

                HandlePlugInInfo(username);

                if (!string.IsNullOrEmpty(appName))
                {
                    System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, true);
                    if (user != null)
                    {
                        if (!user.IsApproved && Maps.MultiTenancy)
                        {
                            user.IsApproved = true;
                            RenewUserGuid(username, Maps.Instance.GetMap(appName), sql);
                            System.Web.Security.Membership.UpdateUser(user);

                        }

                        FormsAuth.SignIn(username, true);

                    }
                }
                string returnUrl = string.Empty;
                if (!string.IsNullOrEmpty(collection["returnUrl"]))
                    returnUrl = collection["returnUrl"];

                if (string.IsNullOrEmpty(appName))
                {
                    SendRegistrationRequest(collection);
                    return Json(new { url = string.IsNullOrEmpty(returnUrl) ? Maps.GetMainAppUrl() + "/apps" : Server.UrlEncode(returnUrl + (returnUrl.Contains("?") ? "&" : "?") + "username=" + username + "&id=" + Maps.Instance.DuradosMap.Database.GetGuidByUsername(username)), error = "success" });
                }
                else
                {
                    return Json(new { url = Maps.GetAppUrl(appName) + "?id=" + id, error = "success" });
                }

            }
            catch (DuradosException exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                return Json(exception.Message);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                //ViewData["confirmed"] = false;
                return Json("error");
            }

            //return Json("success");
        }

        private void RenewUserGuid(string username, Map map, SqlAccess sqlAccess)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Guid", Guid.NewGuid());
            parameters.Add("Username", username);
            string sql = "UPDATE [{0}] SET [Guid]=@Guid WHERE [Username]=@Username";

            sqlAccess.ExecuteNonQuery(map.Database.GetUserView().ConnectionString, string.Format(sql, map.Database.GetUserView().GetTableName()), parameters, null);
          //  sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.ConnectionString, string.Format(sql, Maps.Instance.DuradosMap.Database.GetUserView().GetTableName()), parameters, null);

        }

        protected virtual void HandlePlugInInfo(string username)
        {
            string plugInUserId = string.Empty;

            try
            {
                string key = "instance";
                if (string.IsNullOrEmpty(Request.QueryString[key]))
                    return;

                string instance = Request.QueryString[key];

                WixInstance wixInstance = WixPlugInHelper.GetInstance(instance);

                plugInUserId = wixInstance.uid.ToString();

                int userId = Maps.Instance.DuradosMap.Database.GetUserID(username);

                Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@PlugInUserId", plugInUserId);
                parameters.Add("@PlugInId", (int)PlugInType.Wix);
                parameters.Add("@RegisteredUserId", userId);
                parameters.Add("@SelectionDate", DateTime.Now);


                sql.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.ConnectionString, "insert into durados_PlugInRegisteredUser (PlugInUserId ,PlugInId, RegisteredUserId, SelectionDate) values (@PlugInUserId ,@PlugInId, @RegisteredUserId, @SelectionDate)", parameters, null);
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "could not register username: " + username + " with wix user: " + plugInUserId);
            }


        }

        protected virtual string AssignPendingAppsCallback(Dictionary<string, object> paraemeters)
        {
            return "success";
        }

        protected override void SendRegistrationRequest(FormCollection collection)
        {
            AccountService.SendRegistrationRequest(collection["First_Name"], collection["Last_Name"], collection["Email"], collection["Guid"], collection["Username"], collection["Password"], Map, DontSend);
        }


        

        protected override void SendPasswordResetEmail(FormCollection collection)
        {
            string host,username,password,from, subject,message,siteWithoutQueryString;
            
            int port;

            ConfigureDuradosEmail(out host, out port, out username, out password, out from, out subject, out message, out siteWithoutQueryString, "passwordResetConfirmationSubject", "passwordResetConfirmationMessage");     
           
            message = message.Replace("[username]", collection["username"]);
            message = message.Replace("[Product]", Map.Database.SiteInfo.GetTitle());
            message = message.Replace("[Url]", siteWithoutQueryString);
            message = message.Replace("[Guid]", collection["Guid"]);
            
            string to = collection["to"];

            Durados.Cms.DataAccess.Email.Send(host, Map.Database.UseSmtpDefaultCredentials, port, username, password, false, to.Split(';'), new string[0], null, subject, message, from, null, null, DontSend, null, Map.Database.Logger, true);

        }

        private void ConfigureDuradosEmail(out string host, out int port, out string username, out string password, out string from, out string subject, out string message, out string siteWithoutQueryString,string subjectKey,string contentKey)
        {
            host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            subject = Map.Database.Localizer.Translate(CmsHelper.GetHtml(subjectKey));
            message = Map.Database.Localizer.Translate(CmsHelper.GetHtml(contentKey));

            siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;

        }

        protected override string CreateMembershipCallback(Dictionary<string, object> paraemeters)
        {
            string username = paraemeters["@Username"].ToString();
            string password = paraemeters["@Password"].ToString();
            string email = paraemeters["@Email"].ToString();
            string role = paraemeters["@Role"].ToString();

            if (String.IsNullOrEmpty(role))
            {
                //System.Web.Security.Membership.Provider.DeleteUser(username, true);
                return Map.Database.Localizer.Translate("Failed to create user, Role is missing");
            }

            System.Web.Security.MembershipUser existingUser = System.Web.Security.Membership.Provider.GetUser(username, true);
            if (existingUser != null)
            {
                if (System.Web.Security.Membership.Provider.ValidateUser(username, password))
                {
                    return "success";
                }
                else
                {
                    return ErrorCodeToString(MembershipCreateStatus.InvalidPassword);
                }
            }

            System.Web.Security.MembershipCreateStatus status = CreateUser(username, password, email);

            if (status == MembershipCreateStatus.Success)
            {

                System.Web.Security.Roles.AddUserToRole(username, role);

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

    }

    [HandleError]
    //[Durados.Web.Mvc.Controllers.Attributes.DynamicAuthorizeAttribute(false, false)]
    public class AccountController :  Controller // BaseController//: Durados.Web.Mvc.Controllers.DuradosController
    {
        protected Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        public bool DontSend
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DontSend"] ?? "false");
            }
        }

        protected string   PWResetNotAllowedMessage{
            get { return Map.Database.Localizer.Translate("Password reset is not allowed\r\n"); }
        }
        protected string UsernameNotExistsMessage
        {
            get { return Map.Database.Localizer.Translate("Username dose not exists."); }
        }
        protected string UsernameNotfinishRegister
        {
            get { return Map.Database.Localizer.Translate("User didn't finish registration. Please follow the email instruction in order to finish registration."); }
        }                
        // This constructor is used by the MVC framework to instantiate the controller using
        // the default forms authentication and membership providers.

        public AccountController()
            : this(null, null)
        {
        }

        private Dictionary<string,string> userData;
        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AccountController(IFormsAuthentication formsAuth, IMembershipService service)
        {
            FormsAuth = formsAuth ?? new FormsAuthenticationService();
            MembershipService = service ?? new AccountMembershipService();
        }

        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public IMembershipService MembershipService
        {
            get;
            private set;
        }

        protected virtual string GetLogOnPath()
        {
            string plugIn = Request.QueryString["plugInType"];
            if (!string.IsNullOrEmpty(plugIn))
            {
                return PlugInHelper.GetLogOnPath();
                
            }
            return Map.GetLogOnPath();
        }

        protected virtual bool GetLogMvc()
        {
            string plugIn = Request.QueryString["plugInType"];
            if (!string.IsNullOrEmpty(plugIn))
            {
                return true;
                
            }
            return Map.GetLogMvc();
        }

        protected virtual void HandlePlugIn(string username, string returnUrl)
        {
            if (Request.QueryString["instance"] != null)
            {
                PlugInType plugInType = PlugInType.Wix;
                string plugInUserId = PlugInHelper.GetPlugInUserId(plugInType, Request);
                int userId = (int)Maps.Instance.DuradosMap.Database.GetUserRow(username)["ID"];
                Maps.Instance.DuradosMap.Database.SetCurrentPlugInRegisteredUser(plugInType, plugInUserId, userId, false);
                Maps.Instance.DuradosMap.Database.AddSamplesToRegisteredUsers(plugInType, Request);
            }
        }
        /// <summary>
        /// Generate the URL for google oauth
        /// </summary>
        /// <returns></returns>
        public ActionResult GoogleLogin(SignMode t)
        {
    //        var parameterValue = MyUrls.Url1;
    //if (!string.IsNullOrEmpty(parameter) && !Enum.TryParse(parameter, out parameterValue))
     
            SignMode smode = t;
            string clientId = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
            string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
            string scope = "https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile";

            if (smode == SignMode.SignIn)
                redirectUri += "SigninGoogle";
            else if (smode == SignMode.SignInApp)
                redirectUri += "SigninGoogleApp";
            else if (smode == SignMode.SignUp)
                redirectUri += "SignupGoogle";
            else
                return RedirectPermanent("/");

            string url = string.Format("https://accounts.google.com/o/oauth2/auth?scope={0}&client_id={1}&redirect_uri={2}&response_type=code&access_type=offline&state={3}",scope,clientId,redirectUri,Map.AppName);

            return RedirectPermanent(url);
        }
        public ActionResult SignupGoogle()
        {

            return GoogleOauth(SignMode.SignUp, GetWebsiteUrl() + "/sign-up");
        }

        private static string GetWebsiteUrl()
        {
            return string.Empty;
        }
        public ActionResult SigninGoogle()
        {
            //return GoogleOauth(SignMode,"/ws/sign-in.html");
            return GoogleOauth(SignMode.SignIn, GetWebsiteUrl() + "/sign-in");
        }

        public ActionResult SigninGoogleApp()
        {
            string url = Map.Database.DefaultController;
            string state = Request.QueryString["state"];
            if (state != null)
                url = Maps.GetAppUrl(state);// "http://" + state + ".backand.loc:4008";
            return GoogleOauth(SignMode.SignInApp,url);
        } 
        /// <summary>
        /// Redirect response from Google with code that enabling getting the acceess_token
        /// </summary>
        /// <returns></returns>
        public ActionResult GoogleOauth(SignMode mode,string returnUrl) 
        {


            //get the code from Google and request from access token
            string code = Request.QueryString["code"];
            string error = Request.QueryString["error"];

            if(code==null || error != null)
            {
                string json = ErrorMessageToJson(error,"nocode");
                return Redirect(returnUrl+"?" + JsonToBase64(json));
            }
            try { 
                string urlAccessToken = "https://accounts.google.com/o/oauth2/token";
                //build the URL to send to Google
                string clientId = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
                string clientSecret = System.Configuration.ConfigurationManager.AppSettings["GoogleclientSecret"];
                string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
                if (mode == SignMode.SignIn)
                    redirectUri += "SigninGoogle";
                else if (mode == SignMode.SignInApp)
                    redirectUri += "SigninGoogleApp";
                else if (mode == SignMode.SignUp)
                    redirectUri += "SignupGoogle";
                string accessTokenData = string.Format("scope=&code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code", code, clientId, clientSecret, redirectUri);
                string response = Infrastructure.Http.PostWebRequest(urlAccessToken, accessTokenData);

                //get the access token from the return JSON
                //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                //AuthResponse validateResponse = (AuthResponse)jsonSerializer.Deserialize<AuthResponse>(response);

                Dictionary<string, object> validateResponse = JsonSerializer.Deserialize(response);

                //get the Google user profile using the access token
                string profileUrl = "https://www.googleapis.com/plus/v1/people/me";
                string profileHeader = "Authorization: Bearer " + validateResponse["access_token"].ToString();
                string profiel = Infrastructure.Http.GetWebRequest(profileUrl, profileHeader);

                //get the user email out of goolge profile
                //GoogleProfile googleProfile = (GoogleProfile)jsonSerializer.Deserialize<GoogleProfile>(profiel); ;
                Dictionary<string, object> googleProfile = JsonSerializer.Deserialize(profiel);
                string email = ((Dictionary<string, object>)((object[])googleProfile["emails"])[0])["value"].ToString();
                string name = googleProfile["displayName"].ToString();
            
                //login the user use email
                returnUrl = LoginOrRegister(mode,email,name, "Google",returnUrl);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                returnUrl += "?" +JsonToBase64( ErrorMessageToJson(null, exception.Message));
            }
                
            return Redirect(returnUrl);


        }

        private static string ErrorMessageToJson(string error,string msg)
        {
            string message = (error != null) ? error : msg;
            SuccessMessageResponse res = new SuccessMessageResponse { Success = false, Message = message };
            string json = JsonSerializer.Serialize(res);
            return json;
        }

        /// <summary>
        /// Perform login or registartion when user use oauth2
        /// </summary>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private string LoginOrRegister(SignMode mode, string email, string name, string source,string appUrl)
        {
            string returnUrl = string.Empty;

            Session.Remove("OauthRequestType"); //clear session
            if (mode == null)
                return returnUrl;

            if (mode == SignMode.SignIn || mode == SignMode.SignInApp) //sign-in
            {
                //login the user use email
                if (MembershipService.ValidateUserExists(email))
                {
                    //log and sign in
                    Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "logon=" + source, null, 3, "username=" + email.Replace("'", "\""));
                    FormsAuth.SignIn(email, true);
                    if (mode == SignMode.SignIn)
                        returnUrl = "/apps";
                    else
                        returnUrl = appUrl + "/Home/Default";
                }
                else
                    returnUrl = string.Format("{0}/sign-up?{1}", Maps.Instance.DuradosMap.Url, JsonToBase64(ErrorMessageToJson(null, "no_user")));
            }
            else if (mode == SignMode.SignUp) //sign-up
            {
                //register the user using POST like the website
                string url = Maps.Instance.DuradosMap.Url + "/WebsiteAccount/SignUp";
                string data = string.Format("username={0}&password=&send=true&phone=&fullname={1}&dbtype=100&dbother=",email,name);
                string response = Infrastructure.Http.PostWebRequest(url, data);
                Dictionary<string, object> json = JsonSerializer.Deserialize(response);
                bool success = Convert.ToBoolean(json["Success"]);
                string qstring;
                if (!success)
                {
                     qstring = JsonToBase64(response);
                    
                }
                else
                {
                    DataRow userRow = Map.Database.GetUserRow(email);
                    string guid = userRow["Guid"].ToString();
                    string id = SecurityHelper.GetTmpUserGuidFromGuid(guid);
                    //move to the next page
                    qstring ="gid=" + id;
                }
                returnUrl = string.Format("{0}/sign-up?{1}", Maps.Instance.DuradosMap.Url, qstring);
            }

            return returnUrl;

        }

        private static string JsonToBase64(string response)
        {
            byte[] ret = System.Text.Encoding.UTF8.GetBytes(response);
            string jsonResponseToClient = Convert.ToBase64String(ret);
            return jsonResponseToClient;
        }

        /// <summary>
        /// Generate the URL for Github oauth 
        /// </summary>
        /// <returns></returns>
        public ActionResult GithubLogin(SignMode t)
        {

            string clientId = System.Configuration.ConfigurationManager.AppSettings["GithubClientId"];
            string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];

            if (t == SignMode.SignIn)
                redirectUri += "SigninGithub";
            else if (t == SignMode.SignUp)
                redirectUri += "SignupGithub";
            else if (t == SignMode.SignInApp)
                redirectUri += "SigninGithubApp";
            else
                return RedirectPermanent("/");

            string url = string.Format("https://github.com/login/oauth/authorize?scope=user:email&client_id={0}&redirect_uri={1}?appName={3}&state={2}", clientId, redirectUri, Session.SessionID, Map.AppName);
            return RedirectPermanent(url);
        }

        public ActionResult SignupGithub()
        {
            return GithubOauth(SignMode.SignUp,"/sign-up");
        }

        public ActionResult SigninGithub()
        {
            return GithubOauth(SignMode.SignIn,"/sign-in");
        }

        /// <summary>
        /// Redirect from Github with code and state in order to get the access token and register/login
        /// </summary>
        /// <returns></returns>     
        /// 
        public ActionResult SigninGithubApp(string appName)
        {
            string url = Map.Database.DefaultController;
            
            if (appName != null)
                url = Maps.GetAppUrl(appName);
            return GithubOauth(SignMode.SignInApp, url);
        } 
        public ActionResult GithubOauth(SignMode mode,string returnUrl)
        {
           
            //if(mode!= SignMode.SignInApp)
            //string returnUrl =);

            //get the code from Google and request from access token
            string code = Request.QueryString["code"];
            string state = Request.QueryString["code"];

            if(code==null)
                return Redirect(returnUrl + "?" + JsonToBase64(JsonToBase64(ErrorMessageToJson(null, "nocode"))));
            if (state != null && state != Session.SessionID && state != code)
                return Redirect(returnUrl + "?" + JsonToBase64(ErrorMessageToJson(null, "nostate")));
            try
            {
                string urlAccessToken = "https://github.com/login/oauth/access_token";
                //build the URL to send to Github
                string clientId = System.Configuration.ConfigurationManager.AppSettings["GithubClientId"];
                string clientSecret = System.Configuration.ConfigurationManager.AppSettings["GithubclientSecret"];
                string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
                string accessTokenData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}", code, clientId, clientSecret, redirectUri);
                string response = Infrastructure.Http.PostWebRequest(urlAccessToken, accessTokenData, "", "application/json");

                //get the access token from the return JSON
                Dictionary<string, object> accessObject = JsonSerializer.Deserialize(response);
                string accessToken = accessObject["access_token"].ToString();

                //get the Google user profile using the access token
                string profileUrl = "https://api.github.com/user/emails";
                string profileHeader = "Authorization: Bearer " + accessToken;
                string profiel = Infrastructure.Http.GetWebRequest(profileUrl, profileHeader, "https://api.github.com/meta");

                //get the user email out of goolge profile
                //need to make the JSON more statndrd for us
                profiel = "{\"data\":" + profiel + "}";
                Dictionary<string, object> githubObject = JsonSerializer.Deserialize(profiel);
                string email = ((Dictionary<string, object>)((object[])githubObject["data"])[0])["email"].ToString();

                //login the user use email
                returnUrl = LoginOrRegister(mode,email, "", "Github",returnUrl);

            }
            catch (Exception exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                returnUrl += "?" + JsonToBase64(ErrorMessageToJson(null, exception.Message));
            }

            return Redirect(returnUrl);


        }

        public ActionResult SigninGithubBack()
        {
            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];

            if(string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            {
                //todo return error message
                return null;
            }
            

            var apiAdress = ConfigurationManager.AppSettings["webApiAddress"];
            return Redirect(apiAdress + "/api/account/github?code=" + Uri.EscapeDataString(code) + "&state=" + Uri.EscapeDataString(state));

        }

        [Attributes.RequiresSSL]
        public ActionResult LogOn(string returnUrl)
        {
            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "logon", null, 2, null);

            if (returnUrl != null && returnUrl.Equals("~/"))
                return Redirect(returnUrl);

            Maps.Instance.DuradosMap.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name, null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", returnUrl: " + returnUrl);
            
            //check AD
            //bool x = LDAPAuth("mobixsrv1:389", "MWS\\itay", "shira123");
            string logon = GetLogOnPath(); //Map.GetLogOnPath();// "~/Views/Account/LogOn.aspx";

            //if (Map.Database.SiteInfo != null)
            //    logon = Map.Database.SiteInfo.LogOnPath;

            string id = this.Request.QueryString["id"];
            Guid guid = new Guid();
            string userId = this.Request.QueryString["userId"];

            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out guid) || !string.IsNullOrEmpty(userId))
            {
               
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "id", null, 2, id);
                string username =null;
                if (!string.IsNullOrEmpty(id))
                {
                    string tmpid = SecurityHelper.GetTmpUserGuidFromGuid(id);
                    if (!string.IsNullOrEmpty(tmpid))
                        username = Map.Database.GetUsernameByGuid(tmpid);

                    if (username == null)
                        username = Maps.Instance.DuradosMap.Database.GetUsernameByGuid(tmpid);
                   
                    if (username == null)
                        username = Map.Database.GetUsernameByGuid(SecurityHelper.GetUserGuidFromTmpGuid(id));
                }
                if (username == null && !string.IsNullOrEmpty(userId))
                    username = Map.Database.GetUsernameById(userId);
                //if (id == System.Web.Configuration.WebConfigurationManager.AppSettings["code"])
                #region username != null
                if (username != null)
                {


                    Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username", null, 2, username);

                    System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, true);
                    #region  user == null
                    if (user == null)
                    {
                        if (Maps.MultiTenancy)
                        {
                            string firstName = Map.Database.GetUserFieldValue("FirstName", username);
                            string lastName = Map.Database.GetUserFieldValue("LastName", username);
                            string regUrl = Maps.Instance.DuradosMap.Url + string.Format("/Account/RegistrationRequest?appName={0}&username={1}&firstName={2}&lastName={3}&id={4}", Maps.GetCurrentAppName(), Server.UrlEncode(username), Server.UrlEncode(firstName), Server.UrlEncode(lastName), id);
                            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "regUrl", null, 2, regUrl);

                            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && System.Web.HttpContext.Current.User.Identity.Name != null)
                            {
                                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "SignOut", null, 2, System.Web.HttpContext.Current.User.Identity.Name);
                                FormsAuth.SignOut();
                                System.Web.Security.Roles.DeleteCookie();
                                Session.Clear();

                            }

                            return Redirect(regUrl);
                        }
                        else
                        {
                            //return View(logon);
                            if (Map.GetLogMvc())
                                return View(logon);
                            else
                                return Redirect(logon);
                        }
                    }
                    #endregion

                    #region !user.IsApproved
                    if (!user.IsApproved)
                    {
                        user.IsApproved = true;
                        System.Web.Security.Membership.UpdateUser(user);
                        if (!Maps.MultiTenancy)
                            //return View(logon);
                            if (Map.GetLogMvc())
                                return View(logon);
                            else
                                return Redirect(logon);
                    }
                    #endregion

                    #region User is not null or empty
                    else if (!string.IsNullOrEmpty(userId))
                    {
                        //return View(logon);
                        if (Map.GetLogMvc())
                            return View(logon);
                        else
                            return Redirect(logon);
                    }
                    #endregion

                    //FormsAuth.SignIn(System.Web.Configuration.WebConfigurationManager.AppSettings["codeuser"], true);
                    Maps.Instance.DuradosMap.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"] + ", after authentication: " + username, null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString() + ", returnUrl: " + returnUrl);

                    FormsAuth.SignIn(username, true);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), null, null, 3, "username=" + username.Replace("'", "\""));
                            return Redirect(returnUrl);
                    }
                    else
                    {
                        //Durados.Web.Mvc.View view = Map.Database.FirstView;
                        //return RedirectToAction(view.IndexAction, view.Controller, new { viewName = view.Name });
                        return RedirectToAction("FirstTime", Map.Database.DefaultController);

                    }

                } 
                #endregion
            }

            if (Request.IsAjaxRequest())
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return Content(returnUrl);
            }


            if (Maps.MultiTenancy && !(Map is DuradosMap))
            {
                if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && System.Web.HttpContext.Current.User.Identity.Name != null)
                {
                    string username = System.Web.HttpContext.Current.User.Identity.Name;

                    System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, true);
                    if (user != null)
                    {
                        if (user.IsApproved)
                        {
                            System.Data.DataRow userRow = Map.Database.GetUserRow(username);
                            bool registerd = (userRow != null);
                            bool approved = registerd && !userRow["IsApproved"].Equals(false);
                            if (approved)
                            {
                                return RedirectToAction("FirstTime", Map.Database.DefaultController);
                            }
                        }
                    }
                }
            }

            LogOff1();
            
            //return View(logon);
            if (GetLogMvc())
                return View(logon);
            else
                return Redirect(logon);
        }

        
        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public JsonResult ValidateAuthentication(string userName, string password)
        {
            return Json(ValidateLogOn(userName, password));
        }

        //[Attributes.SecureConnection]
        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult LogOn(string userName, string password, bool rememberMe, string returnUrl,string token)
        {

            if (!ValidateLogOn(userName, password) ||( !string.IsNullOrEmpty(Map.Database.LogOnUrlAuth) && !ValidateLogOnAuthUrl(Request.Form)))
            {
                //string logon = "~/Views/Account/LogOn.aspx";

                //if (Map.Database.SiteInfo != null)
                //    logon = Map.Database.SiteInfo.LogOnPath;
                string logon = GetLogOnPath();
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "logon=" + logon, null, 3, "username=" + userName.Replace("'", "\""));
                if (GetLogMvc())
                    return View(logon);
                else
                    return Redirect(logon);
            }

           
            FormsAuth.SignIn(userName, rememberMe);
            HandlePlugIn(userName, returnUrl);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                //HttpCookie cookie = Request.Cookies["d_lv"];
                //string viewName = cookie.Value;
                //Durados.Web.Mvc.View view = null;
                //if (!Map.Database.Views.ContainsKey(viewName))
                //{
                //    view = (View)Map.Database.Views[viewName];
                //}
                //else
                //{
                //    view = Map.Database.GetUserFirstView(userName);
                //}
                //try
                //{
                //    return RedirectToAction(view.IndexAction, view.Controller, new { viewName = view.Name });
                //}
                //catch (AccessViolationException)
                //{
                //    view = Map.Database.GetUserFirstView(userName);
                //    return RedirectToAction(view.IndexAction, view.Controller, new { viewName = view.Name });
                //}
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "first time", null, 150, "username=" + userName.Replace("'", "\""));
                //if (Maps.Skin)
                //    return RedirectToAction("FirstTime", Map.Database.DefaultController);
                //else
                //{
                //    return Redirect("~/apps");
                //}
                return RedirectToAction("FirstTime", Map.Database.DefaultController);
            }
        }

        private string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }

        private bool ValidateLogOnAuthUrl(NameValueCollection formCollecion)
        {
            try
            {
                NameValueCollection nameValueCollection = new NameValueCollection();

                foreach (string nv in Map.Database.LogOnUrlAuthToken)
                {
                    nameValueCollection.Add(nv, formCollecion[nv]);
                }
                nameValueCollection.Add("username", formCollecion["username"]);

                string externalValidetionIdentificationKey = System.Configuration.ConfigurationManager.AppSettings["ExternalValidetionIdentificationKey"] ?? "auth";
                string externalValidetionIdentificationValue = System.Configuration.ConfigurationManager.AppSettings["ExternalValidetionIdentificationValue"] ?? true.ToString();

                nameValueCollection.Add(externalValidetionIdentificationKey, externalValidetionIdentificationValue);


                string url = Map.Database.LogOnUrlAuthBase + ToQueryString(nameValueCollection);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = HttpVerb.GET.ToString();
                
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string responseContent = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                    
                    SuccessMessageResponse validateResponse = (SuccessMessageResponse)jsonSerializer.Deserialize<SuccessMessageResponse>(responseContent) ;;

                    if (validateResponse.Success)
                        return true;
                    else
                    {
                        ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate(validateResponse.Message));
                        return false;
                        };
                }
                // }
            }
            catch (Exception ex)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "ValidateLogOnAuthUrl", "Fail to validate through LogOnAuthUrl", ex.StackTrace, 3, "", DateTime.Now);
                ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate(ex.Message));
                return false;
            }

        }

        [System.Runtime.Serialization.DataContract]
        private class SuccessMessageResponse
        {
            [System.Runtime.Serialization.DataMember]
            public bool Success { get; set; }
            [System.Runtime.Serialization.DataMember]
            public string Message { get; set; }
        }

        /* Google Profile
        {
          "kind": "plus#person", 
          "displayName": "Itay Herskovits", 
          "name": {
            "givenName": "Itay", 
            "familyName": "Herskovits"
          }, 
          "language": "en", 
          "isPlusUser": true, 
          "url": "https://plus.google.com/109864257821637722214", 
          "gender": "male", 
          "image": {
            "url": "https://lh3.googleusercontent.com/-L2nt9bPZgik/AAAAAAAAAAI/AAAAAAAAAn4/M5A-KYLyVVQ/photo.jpg?sz=50", 
            "isDefault": false
          }, 
          "domain": "backand.com", 
          "emails": [
            {
              "type": "account", 
              "value": "itay@backand.com"
            }
          ], 
          "etag": "\"goGJzLGpDAdTIjyZUs7et8jwqfg/BrhpysDrlUUvrievcBIhbvSp4xo\"", 
          "verified": false, 
          "circledByCount": 32, 
          "id": "109864257821637722214", 
          "objectType": "person"
        }*/

        private void LogOff1()
        {
            Session.Clear();
            System.Web.Security.Roles.DeleteCookie();
            FormsAuth.SignOut();
            Session.Abandon();
            //Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));

            // Clear authentication cookie
            HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            rFormsCookie.Expires = DateTime.Now.AddYears(-1);
            rFormsCookie.Domain = "." + Maps.Host;
            Response.Cookies.Add(rFormsCookie);

            // Clear session cookie 
            HttpCookie rSessionCookie = new HttpCookie("ASP.NET_SessionId", "");
            rSessionCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(rSessionCookie);

            this.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.Cache.SetNoStore();

        }

        public ActionResult LogOff()
        {
            LogOff1();

            string logon = Map.GetLogOnPath();// "~/Views/Account/LogOn.aspx";
            if (Map.GetLogMvc())
                return View(logon);
            else
                return Redirect(logon);
            //return RedirectToAction("LogOn", "Account");
        }

        public virtual ActionResult RegistrationRequest(string returnUrl)
        {
            LogOff1();
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        public ActionResult RegistrationRequestConfirmation(bool confirmed)
        {
            ViewData["confirmed"] = confirmed;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult RegistrationRequest(FormCollection collection)
        {
            try
            {
                Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                string email = string.Empty;
                if (collection["Email"] == null)
                    throw new DuradosException("Missing 'Email or Username' in the registration form");
                else
                    email = collection["Email"];
                parameters.Add("@Email", email);

                string username = string.Empty;
                if (string.IsNullOrEmpty(collection["Username"]))
                    username = email; //throw new DuradosException("Missing 'Username' in the registration form");
                else
                    username = collection["Username"];
                parameters.Add("@Username", username);


                if (sql.ExecuteScalar(Map.Database.ConnectionString, "SELECT TOP 1 [Username] FROM [User] WHERE [Username]=@Username", parameters) != string.Empty)
                {
                    throw new DuradosException("'Username' already exist [" + collection["Username"] + "]");
                }

                string firstName = string.Empty;
                if (collection["First_Name"] == null)
                    throw new DuradosException("Missing 'First Name' in the registration form");
                else
                    firstName = collection["First_Name"];
                parameters.Add("@FirstName", firstName);

                string lastName = string.Empty;
                if (collection["Last_Name"] == null)
                    throw new DuradosException("Missing 'Last Name' in the registration form");
                else
                    lastName = collection["Last_Name"];
                parameters.Add("@LastName", lastName);

                string password = string.Empty;
                if (Map.Database.Views.ContainsKey(Map.Database.UserViewName) && Map.Database.Views[Map.Database.UserViewName].Fields.ContainsKey("Password") && Map.Database.Views[Map.Database.UserViewName].Fields["Password"].DefaultValue != null)
                    password = Map.Database.Views[Map.Database.UserViewName].Fields["Password"].DefaultValue.ToString();
                else
                    password = "dd7gdd8asc";
                parameters.Add("@Password", password);

                string role = string.Empty;
                if (collection["DefaultRole"] == null)
                    throw new DuradosException("Missing hidden field 'DefaultRole' in the registration form");
                else
                    role = collection["DefaultRole"];
                parameters.Add("@Role", role);

                parameters.Add("@NewUser", true);

                string comments = string.Empty;
                if (collection["Reason"] == null)
                    throw new DuradosException("Missing 'Reason' in the registration form");
                else
                    comments = collection["Reason"];
                parameters.Add("@Comments", comments);



                sql.ExecuteNonQuery(Map.Database.ConnectionString, "INSERT INTO [User] ([Username],[FirstName],[LastName],[Email],[Password],[Role],[NewUser],[Comments]) VALUES (@Username,@FirstName,@LastName,@Email,@Password,@Role,@NewUser,@Comments)", parameters, CreateMembershipCallback);
                SendRegistrationRequest(collection);
          

            }
            catch (DuradosException exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                return Json(exception.Message);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                //ViewData["confirmed"] = false;
                return Json("error");
            }

            return Json("success");
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
                return Map.Database.Localizer.Translate("Failed to create user, Role is missing");
            }

            System.Web.Security.MembershipCreateStatus status = CreateUser(username, password, email);

            if (status == MembershipCreateStatus.Success)
            {

                System.Web.Security.Roles.AddUserToRole(username, role);

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

        //public new bool DontSend
        //{
        //    get
        //    {
        //        return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DontSend"] ?? "false");
        //    }
        //}

        protected virtual void SendRegistrationRequest(FormCollection collection)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            string subject = collection["Subject"];
            string message = string.Empty;
            foreach (string key in collection.AllKeys)
            {
                if (!(key == "Subject" || key == "Comments" || key == "to" || key == "cc"))
                    message += key.Replace("_", " ") + ":" + " " + collection[key] + "<br>";
            }
            message += collection["Comments"];
            string to = collection["to"];
            string cc = collection["cc"];



            View userView = (View)Map.Database.GetUserView();
            if (userView != null)
            {
                message += "<br><br>";
                string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;
                message += "<a href=\"" + siteWithoutQueryString + "/" + userView.Controller + "/" + userView.IndexAction + "/" + userView.Name + "\">Go to " + Map.Database.SiteInfo.Product + " users</a>";
            }

            Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, to.Split(';'), cc.Split(';'), null, subject, message, from, null, null, DontSend, null);

        }

        protected virtual MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            System.Web.Security.Membership.Provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult RegistrationRequest(FormCollection collection)
        //{
        //    try
        //    {


        //        string host = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["host"]);
        //        int port = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["port"]);
        //        string username = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["username"]);
        //        string password = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["password"]);

        //        string from = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["fromAlert"]);
        //        string subject = collection["Subject"];
        //        string message = string.Empty;
        //        foreach (string key in collection.AllKeys)
        //        {
        //            if (!(key == "Subject" ||key == "Comments" ||key == "to" ||key == "cc"))
        //                message += key.Replace("_", " ") + ":" + " " + collection[key] + "<br>";
        //        }
        //        message += collection["Comments"];
        //        string to = collection["to"];
        //        string cc = collection["cc"];

        //        Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, to.Split(';'), cc.Split(';'), null, subject, message, from, null, null, null);

        //        ViewData["confirmed"] = true;
        //    }
        //    catch(Exception exception)
        //    {
        //        Durados.Web.Mvc.Logging.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
        //        ViewData["confirmed"] = false;
        //    }

        //    return View("RegistrationRequestConfirmation");
        //}

        public ActionResult Register()
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(string userName, string email, string password, string confirmPassword)
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (ValidateRegistration(userName, email, password, confirmPassword))
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(userName, password, email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuth.SignIn(userName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        //[Authorize]
        public ActionResult ChangePassword()
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            ViewData["guid"] = Request.QueryString["id"];
             
            return View();
        }


        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            
            
            string errors;
            if (!ValidateChangePassword2(currentPassword, newPassword, confirmPassword,out errors) )
            {
                return Json(new { success = false, message = errors });
                //return View();
            }

            try
            {
                string username = User.Identity.Name;
                if (MembershipService.ChangePassword(username, currentPassword, newPassword, true))
                {
                   
                    FormsAuth.SignIn(username, true);
                    MembershipService.UnlockUser(username);

                    return Json(new { success = true, message = Map.Database.Localizer.Translate("Password have been change successfuly") });
                }
                else
                {
                    return Json(new { success = false, message = Map.Database.Localizer.Translate("The current password is incorrect or the new password is invalid.") });
                    
                }
            }
            catch
            {
                return Json(new { success = false, message = Map.Database.Localizer.Translate("The current password is incorrect or the new password is invalid.") });
                
            }
        }
        
        
    
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ForgotPassword(string newPassword, string confirmPassword, string userSysGuid)
        {
            string usernameForgot = null;
            string currentPassword = null;
            if (string.IsNullOrEmpty(userSysGuid))
            {
                return Json(new { success = false, message = "missing user identification" });

            }
            if (string.IsNullOrEmpty(confirmPassword))
            {
                return Json(new
                {
                    success = false,
                    message = String.Format(CultureInfo.CurrentCulture,
                        "You must specify a new password of {0} or more characters.",
                        MembershipService.MinPasswordLength)
                });

            }

            if (!ValidateNewPassword(newPassword, confirmPassword))

                return Json(new { success = false, message = "Passwords do not match." });

            userSysGuid = SecurityHelper.GetUserGuidFromTmpGuid(userSysGuid);
            if (string.IsNullOrEmpty(userSysGuid))
            {
                return Json(new { success = false, message = "User identification is invalid." });
            }
            string guid = GetUserDetail(userSysGuid, Map.Database.UserGuidFieldName);// GetUserDetailsFromGuid(userSysGuid, "[" + Map.Database.UserViewName + "].[" + Map.Database.UserGuidFieldName + "]");
            if (string.IsNullOrEmpty(guid))// &&  guid.Equals(userSysGuid)
            {
                return Json(new { success = false, message = "User data is invalid." });

            }

           

            string errorMessage = Map.Database.Localizer.Translate("The current password is incorrect or the new password is invalid.");
            currentPassword = ChangePasswordAfterForgot(userSysGuid, out usernameForgot);
            try
            {
                string username = usernameForgot ?? User.Identity.Name;
                if (MembershipService.ChangePassword(username, currentPassword, newPassword, true))
                {

                    MembershipService.UnlockUser(username);
                    return Json(new { success = true, message = "Your password has been changed successfully." });
                }
                else
                {

                    ModelState.AddModelError("_FORM", errorMessage);
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch
            {
                ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate("The current password is incorrect or the new password is invalid."));
                return Json(new { success = false, message = errorMessage });
            }

        }

    
        private string  GetFullName(string userSysGuid)
        {
            //return Map.Database.GetUserFullName();
            //return GetUserDetailsFromGuid(userSysGuid,"["+Map.Database.UserViewName+"].[FirstName]+' '+["+Map.Database.UserViewName+"].[LastName]");
            return GetUserDetail(userSysGuid, "FirstName") + " " + GetUserDetail(userSysGuid, "LastName");
        }

      
        private  string  ChangePasswordAfterForgot(string guid,out string username)
        {
            username = GetUserDetail(guid, Map.Database.UsernameFieldName);// Map.Database.GetUsernameByGuid(guid);//GetUserDetailsFromGuid(guid, "[" + Map.Database.UserViewName + "].[" + Map.Database. + "]");
                return MembershipService.ResetPassword(username);

        }
        private string GetUserDetail(string guid, string userField)
        {
            if (userData == null)
                LoadUserData(guid);
            //return GetUserDetailsFromGuid(guid, userField);
            if (userData == null)
                throw new DuradosException("User does not exist.");
            if (userData.ContainsKey(userField))
                return userData[userField];
            else
                return string.Empty;


        }

        private void LoadUserData(string guid)
        {
            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@guid", guid);

            string sqlDuradosSys = string.Format("SELECT TOP 1 username FROM durados_user WITH(NOLOCK)  WHERE guid=@guid");

            object duradosSysUser = sqlAccess.ExecuteScalar(Maps.Instance.ConnectionString, sqlDuradosSys, parameters);

            if (duradosSysUser == null || duradosSysUser == DBNull.Value)
                throw new DuradosException(Map.Database.Localizer.Translate("Username has no uniqe guid ,password canot be reset."));

            parameters.Clear();

            parameters.Add("@username", duradosSysUser.ToString());

            string sql = string.Format("SELECT TOP 1 {0} FROM {1} WITH(NOLOCK)  WHERE {2}=@username", GetUserFieldsForSelect(), Map.Database.UserViewName, Map.Database.UsernameFieldName);

            object dataTable = sqlAccess.ExecuteTable(Map.Database.GetUserView().ConnectionString, sql, parameters, CommandType.Text);

            if (dataTable == null || dataTable == DBNull.Value)
                throw new DuradosException(Map.Database.Localizer.Translate("Username has no uniqe guid ,password canot be reset."));
            if (((DataTable)dataTable).Rows.Count <= 0)
                throw new DuradosException(UsernameNotExistsMessage);
            userData = new Dictionary<string, string>();
            DataRow row = ((DataTable)dataTable).Rows[0];
           
            foreach (DataColumn col in row.Table.Columns)
            {
                userData.Add(col.ColumnName, row[col.ColumnName].ToString());
            }
        }

        private string GetUserFieldsForSelect()
        {
            string select;
            select = string.Format("[{0}],[{1}],[{2}],[{3}],[{4}]", Map.Database.UserGuidFieldName, Map.Database.UsernameFieldName,"FirstName","LastName","Email");
            
            return select;
        }
     
        //private string GetUserDetailsFromGuid(string duradosSysUserGuid, string userField)
        //{
        //    string userDetail;
        //    Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

        //    Dictionary<string, object> parameters = new Dictionary<string, object>();

        //    parameters.Add("@guid", duradosSysUserGuid);

        //    string sqlDuradosSys = string.Format("SELECT TOP 1 username FROM durados_user WITH(NOLOCK)  WHERE guid=@guid");

        //    object duradosSysUser = sqlAccess.ExecuteScalar(Maps.Instance.ConnectionString, sqlDuradosSys, parameters);

        //    if (duradosSysUser == null || duradosSysUser == DBNull.Value)
        //        throw new DuradosException(Map.Database.Localizer.Translate("Username has no uniqe guid ,password canot be reset."));

        //    parameters.Clear();

        //    parameters.Add("@username", duradosSysUser.ToString());

        //    string sql = string.Format("SELECT TOP 1 {0} FROM {1} WITH(NOLOCK)  WHERE "+Map.Database.UsernameFieldName+"=@username", userField,Map.Database.UserViewName);

        //    object user = sqlAccess.ExecuteScalar(Map.Database.GetUserView().ConnectionString, sql, parameters);

        //    if (user == null || user == DBNull.Value)
        //        throw new DuradosException(Map.Database.Localizer.Translate("Username has no uniqe guid ,password canot be reset."));

        //    userDetail = user.ToString();

        //    return userDetail;
        //}

        
             [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
   
        protected virtual void SaveUserPassword(string newPassword)
        {
            //Durados.Web.Mvc.View view = GetView(((Durados.Web.Mvc.Database)Database).UserViewName);

            //Dictionary<string, object> values = new Dictionary<string,object>();
            //values.Add(GetPasswordFieldName(), newPassword);
            //view.Edit(values, view.GetPkValue(GetUserRow()), null, null, null);
            Map.Database.SaveUserPassword(newPassword);
        }

        public ActionResult ChangePasswordSuccess()
        {

            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string action = filterContext.RouteData.Values["action"].ToString();
            if (Request.Browser.Browser == "IE")
                System.Web.HttpContext.Current.Response.AddHeader("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");

            if (filterContext.HttpContext.User.Identity is WindowsIdentity && Maps.Skin == false)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }

            if ((new Attributes.RequiresSSL()).ForceHttps(filterContext))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            if (Map.Logger.Events.BeforeAction || Map.Logger.IsAllEventsAction(action))
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                Map.Logger.Log(controller, action, "BeforeAction", null, 3, "action event");
            }

            base.OnActionExecuting(filterContext);
        }

        #region Validation Methods
        private bool ValidateChangePassword2(string currentPassword, string newPassword, string confirmPassword, out string errors)
        {
            errors = string.Empty;
            string userName = null;
            try
            {
                userName = System.Web.HttpContext.Current.User.Identity.Name;
            }
            catch { }
            if (string.IsNullOrEmpty(userName))
            {
                errors += string.Format("<br>{0}", Map.Database.Localizer.Translate("User is not logged."));
                return false;
            }
            if (String.IsNullOrEmpty(currentPassword))
            {
                errors += string.Format("<br>{0}", Map.Database.Localizer.Translate("You must specify a current password."));
                return false;
            }
            if (!MembershipService.ValidateUserExists(userName))
            {
                errors += string.Format("<br>{0}", Map.Database.Localizer.Translate("username not found."));
                return false;
            }
            else if (!MembershipService.ValidateUser(userName, currentPassword))
            {
                errors += string.Format("<br>{0}", Map.Database.Localizer.Translate("The username or password provided is incorrect."));
                return false;
            }

            if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            {
                errors += string.Format("<br>{0}",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
                return false;
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                errors += string.Format("<br>{0}", Map.Database.Localizer.Translate("The new password and confirmation password do not match."));
                return false;
            }


            return true;
        
        }
        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            string userName = null;
            try
            {
                userName = System.Web.HttpContext.Current.User.Identity.Name;
            }
            catch { }
            if (string.IsNullOrEmpty(userName) || !ValidateLogOn(userName, currentPassword))
            { 
                ModelState.AddModelError("currentPassword", Map.Database.Localizer.Translate("User is not logged in or current password is in correct.")); 
            }
            if (String.IsNullOrEmpty(currentPassword))
            {

                ModelState.AddModelError("currentPassword", Map.Database.Localizer.Translate("You must specify a current password."));

            }

            return ValidateNewPassword(newPassword, confirmPassword);
        }

        private bool ValidateNewPassword(string newPassword, string confirmPassword)
        {
            if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate("The new password and confirmation password do not match."));
            }

            return ModelState.IsValid;
        }

        private bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", Map.Database.Localizer.Translate("You must specify a username."));
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", Map.Database.Localizer.Translate("You must specify a password."));
            }
            if (!MembershipService.ValidateUserExists(userName))
            {
                ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate("username not found."));
            }
            else if (!MembershipService.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate("The username or password provided is incorrect."));
            }

            return ModelState.IsValid;
        }

        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", Map.Database.Localizer.Translate("You must specify a username."));
            }
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", Map.Database.Localizer.Translate("You must specify an email address."));
            }
            if (password == null || password.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate("The new password and confirmation password do not match."));
            }
            return ModelState.IsValid;
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
        #endregion

#region
       
        public ActionResult PasswordReset()
        {
            if (!MembershipService.PasswordResetEnabled) throw new DuradosException(PWResetNotAllowedMessage);
          //  string url = "~/Views/Account/PasswordReset.aspx";
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            ViewData["username"] = Request.QueryString["username"];
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PasswordReset(string userName)
        {
              
            try
            {
               
                if (!MembershipService.PasswordResetEnabled) throw new DuradosException(PWResetNotAllowedMessage);

                if (!MembershipService.ValidateUserExists(userName))
                    if(string.IsNullOrEmpty(Map.Database.GetGuidByUsername(userName)))
                        return Json(new { error = "error", message = UsernameNotExistsMessage });
                    else
                        return Json(new { error = "error", message = UsernameNotfinishRegister });

                string  guid = GetUserGuid(userName);

                //string guid = Map.Database.GetGuidByUsername(userName);
               
                FormCollection collection = new FormCollection();
               
                collection.Add("username", userName);

                string to = GetUserDetail(guid, "Email");//GetUserDetailsFromGuid(guid, "["+Map.Database.UserViewName+"].[email]");
                //string to = Map.Database.GetEmailByUsername(userName);

                if (string.IsNullOrEmpty(to))
                    throw new DuradosException("Account missing email, reset password email can not be sent.");

                collection.Add("to", to );

                collection.Add("Guid", SecurityHelper.GetTmpUserGuidFromGuid(guid.ToString()));

                string subject = CmsHelper.GetHtml("passwordResetConfirmationSubject");
                string messageKey =string.Empty;
                if (Maps.Instance.GetMap() is DuradosMap)
                    messageKey = "passwordResetConfirmationMessageInSite";
                else 
                    messageKey = "passwordResetConfirmationMessage";
                string message = CmsHelper.GetHtml(messageKey);
               
                if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
                    throw new DuradosException("Missing email Content, make sure your database contains content table with required keys.");

                collection.Add("Subject",Map.Database.Localizer.Translate(subject) );

                collection.Add("Message", Map.Database.Localizer.Translate(message));

               SendPasswordResetEmail(collection);

               return Json(new { error = "success", message = Map.Database.Localizer.Translate("Please check your mailbox - we've sent you an email with a link to reset your password.") });
            }

            catch (DuradosException exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                return Json(new { error = "error", message = exception.Message });
            }
          
        }

        private string GetUserGuid(string userName)
        {
            try
            {
                Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("@username", userName);
                string userViewName=Map.Database.UserViewName;
                object guid = sql.ExecuteScalar(Maps.Instance.DuradosMap.connectionString, "SELECT TOP 1 [durados_user].[guid] FROM durados_user WITH(NOLOCK)  WHERE [durados_user].[username]=@username", parameters);

                if (guid == null || guid == DBNull.Value)
                    throw new DuradosException(Map.Database.Localizer.Translate("Username has no uniqe guid ,password canot be reset."));

                return guid.ToString();
            }
            catch (Exception ex)
            {
                throw new DuradosException("User guid was not found.", ex);
            }

        }

       
        private string GetLoginUrl()
        {
            string thisUrl = Request.Url.AbsoluteUri;
            string baseUrl = thisUrl.Substring(0, thisUrl.LastIndexOf('/'));
            return baseUrl + "/LogOn";
        }

     
        protected virtual void SendPasswordResetEmail(FormCollection collection)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            string subject = collection["Subject"]??string.Empty;
            string message = collection["Message"]??string.Empty;
            
            message += collection["Comments"];
            string to = collection["to"]??string.Empty;
            string cc = collection["cc"] ?? string.Empty;

            string siteWithoutQueryString = Maps.GetAppUrl(Maps.GetCurrentAppName()); //System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;  

            View userView = (View)Map.Database.GetUserView();
       
            message = message.Replace("[username]", collection["username"]);
            message = message.Replace("[fullname]", GetFullName(collection["Guid"]),false);
            message = message.Replace("[Product]", Map.Database.SiteInfo.GetTitle());
            message = message.Replace("[Url]", siteWithoutQueryString);
            message = message.Replace("[Guid]", collection["Guid"]);
           
            Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, to.Split(';'), cc.Split(';'), null, subject, message, from, null, null, DontSend, Map.Logger);

        }


#endregion
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    string action = filterContext.RouteData.Values["action"].ToString();
        //    if (Logger.Events.BeforeAction || Logger.IsAllEventsAction(action))
        //    {
        //        string controller = filterContext.RouteData.Values["controller"].ToString();
        //        Logger.Log(controller, action, "BeforeAction", null, 3, "action event");
        //    }

        //    base.OnActionExecuting(filterContext);
        //}

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string action = filterContext.RouteData.Values["action"].ToString();

            if (Map.Logger.Events.AfterAction || Map.Logger.IsAllEventsAction(action))
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                Map.Logger.Log(controller, action, "AfterAction", null, 3, "action event");
            }

            base.OnActionExecuted(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            string action = filterContext.RouteData.Values["action"].ToString();

            if (Map.Logger.Events.BeforeResult || Map.Logger.IsAllEventsAction(action))
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                Map.Logger.Log(controller, action, "BeforeResult", null, 3, "action event");
            }
            base.OnResultExecuting(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            string action = filterContext.RouteData.Values["action"].ToString();

            if (Map.Logger.Events.AfterResult || Map.Logger.IsAllEventsAction(action))
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                Map.Logger.Log(controller, action, "AfterResult", null, 3, "action event");
            }
            base.OnResultExecuted(filterContext);
        }


        
    }

    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.
    public enum SignMode
    { SignIn,SignUp,SignInApp}

    public interface IFormsAuthentication
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthentication
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            //createPersistentCookie = false;

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
            
            //SSO
            if (Maps.MultiTenancy && createPersistentCookie)
            {
                System.Web.HttpCookie MyCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(userName, false);
                MyCookie.Domain = "." + Maps.Host;
                //the second level domain name
                System.Web.HttpContext.Current.Response.AppendCookie(MyCookie);
            }
        }
        public void SignOut()
        {
            if (Maps.MultiTenancy)
            {
                string userName = null;
                try
                {
                    userName = System.Web.HttpContext.Current.User.Identity.Name;
                }
                catch{}
                if (!string.IsNullOrEmpty(userName))
                {
                    System.Web.HttpCookie MyCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(userName, false);
                    MyCookie.Domain = Maps.Host;
                    //the second level domain name
                    System.Web.HttpContext.Current.Response.Cookies.Remove(MyCookie.Name);
                }
            }
            FormsAuthentication.SignOut();
        }
    }

    public interface IMembershipService
    {
 
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword, bool userIsOnline);

        bool PasswordResetEnabled { get; }
        bool RequiresQuestionAndAnswer { get; }
        string ResetPassword(string userName);
        bool ValidateUserExists(string userName);
        string GetUserQuestion(string userName);
        string GetUserEmail(string userName);

        bool UnlockUser(string username);
    }

    public class AccountMembershipService : IMembershipService
    {
        private Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        private static MembershipProvider _provider;

        public AccountMembershipService()
            : this(GetProvider())
        {
        }

        private static  MembershipProvider GetProvider()
        {
            MembershipProvider provider = null;
            //if (!string.IsNullOrEmpty(Maps.Instance.GetMap().securityConnectionString))
            //{
            // provider=   new MapMembershipProvider();
            // provider.Initialize(null,null);
            //}
            return provider;
            
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            
                _provider =provider?? System.Web.Security.Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool AuthenticateUser(Map map, string userName, string password)
        {
            //if (map.Database.BackandSSO)
            if (map is DuradosMap)
            {
                return _provider.ValidateUser(userName, password);
            }
            else
            {
                return MultiSignOnValidation(map, userName, password);
            }
        }

        public bool IsRegisterd(string userName)
        {
            System.Data.DataRow userRow = Map.Database.GetUserRow(userName);
            return (userRow != null);
                    
        }

        public bool IsApproved(string userName, string appName = null)
        {
            Map map;
            if (appName == null)
                map = Map;
            else
                map = Maps.Instance.GetMap(appName);

            System.Data.DataRow userRow = map.Database.GetUserRow(userName);
            bool registerd = (userRow != null);
            return registerd && !userRow["IsApproved"].Equals(false);
        }


        public bool ValidateUser(string userName, string password)
        {
            AuthenticatedUserInfo userInfo = null;
            bool authenticated = ValidateUserSingleTenant(userName, password, out userInfo);
            bool registerd = false;
            bool approved = false;
            bool validated = false;
            if (Maps.MultiTenancy)
            {
                if (authenticated)
                {
                    System.Data.DataRow userRow = Map.Database.GetUserRow(userName);
                    registerd = (userRow != null);
                    approved = registerd && !userRow["IsApproved"].Equals(false);
                }
                if (Map.Database.SecureLevel == SecureLevel.RegisteredUsers)
                {
                    validated = approved;
                }
                else if (Map.Database.SecureLevel == SecureLevel.AuthenticatedUsers)
                {
                    validated = authenticated;
                    if (authenticated && !registerd)
                    {
                        RegisterGuest(userInfo, userName);
                    }
                }
                else if (Map.Database.SecureLevel == SecureLevel.AllUsers)
                {
                    validated = true;
                    if (authenticated && !registerd)
                    {
                        RegisterGuest();
                    }
                    else if (!authenticated)
                    {
                        //string role = Map.Database.GetUserRole(userName);
                        //validated = string.IsNullOrEmpty(role) || role.Equals(Map.Database.DefaultGuestRole);
                        validated = false;
                    }
                }
                else
                {
                    validated = false;
                }
            }
            else
            {
                validated = authenticated;
            }

            return validated;
        }

        public string RegisterGuest()
        {
            return RegisterGuest(new AuthenticatedUserInfo() { FirstName = Database.GuestUsername, LastName = "", Email = "guest@durados.com" }, Database.GuestUsername);
        }

        public virtual string RegisterGuest(AuthenticatedUserInfo userInfo, string username)
        {
            DataRow row = Map.Database.GetUserRow(username);
            if (row != null)
                return row["ID"].ToString();

            View userView = (View)Map.Database.GetUserView();
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Username", username);
            values.Add("Email", userInfo.Email);
            values.Add("FirstName", userInfo.FirstName);
            values.Add("LastName", userInfo.LastName);
            if (!userView.Fields["IsApproved"].ExcludeInInsert)
                values.Add("IsApproved", true);
            string role = Map.Database.DefaultGuestRole;
            if (string.IsNullOrEmpty(role))
            {
                role = "User";
            }
            values.Add(userView.GetFieldByColumnNames("Role").Name, role);

            foreach (Field field in userView.Fields.Values.Where(f => !f.ExcludeInInsert && f.Name != "ID" && f.FieldType != FieldType.Children))
            {
                if (!values.ContainsKey(field.Name))
                {
                    object defaultValue = field.DefaultValue ?? string.Empty;

                    if (field.Required)
                    {
                        if (defaultValue.Equals(string.Empty))
                            throw new DuradosException("Please enter a default value for '" + field.DisplayName + "' set it for not required.");
                    }
                    else
                    {
                        values.Add(field.Name, defaultValue);
                    }
                }
            }

            return userView.Create(values);
        }

        public bool ValidateUserSingleTenant(string userName, string password, out AuthenticatedUserInfo userInfo)
        {
            bool ignoreAD = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IgnoreAD"]);
            bool valid = false;
            //if active directory enable use it otherwise provider
            string path = Map.Database.ActiveDirectoryServer;
            string ldapConnectionString = Map.Database.ActiveDirectoryConnectionString;
            userInfo = null;
            if (!string.IsNullOrEmpty(path) || !ignoreAD)
            {
                MembershipUser user = _provider.GetUser(userName, true);

                if (user != null && user.IsApproved)
                {
                    string domainUser = userName;
                    if (!string.IsNullOrEmpty(Map.Database.ActiveDirectoryDomain))
                    {
                        domainUser = Map.Database.ActiveDirectoryDomain + "\\" + userName;
                    }

                    if (!LDAPAuth(path, domainUser, ldapConnectionString, password, out userInfo))
                    {
                        if (ignoreAD)
                        {

                            valid = AuthenticateUser(Map, userName, password); //try users that are not in Active Directory
                            
                        }
                        else
                            valid = false;
                    }
                    else
                        valid = true; //pass the Active directory


                }
                else
                    valid = false;
            }
            else
            {
                if (Map is DuradosMap)
                    valid = AuthenticateUser(Map, userName, password) && ValidateUser(userName);
                else
                    valid = AuthenticateUser(Map, userName, password);
            }

            if (valid && userInfo == null)
            {
                try
                {
                    DataRow row = Maps.Instance.DuradosMap.Database.GetUserRow(userName);
                    if (row != null)
                    {
                        userInfo = new AuthenticatedUserInfo() { FirstName = row["FirstName"].ToString(), LastName = row["LastName"].ToString(), Email = row["Email"].ToString() };
                    }
                }
                catch (Exception exception)
                {
                    Map.Logger.Log("AccountMembershipService", "ValidateUserSingleTenant-get user info", exception.Source, exception, 1, exception.Message);
                }
            }

            return valid;

        }

        private bool MultiSignOnValidation(Map map, string userName, string password)
        {
            return AccountService.MultiSignOnValidation(map, userName, password);
        }

        public bool ValidateUser(string userName)
        {
            if (Map is DuradosMap || Maps.IsDevUser())
                return true;
            DataRow row = Maps.Instance.DuradosMap.Database.GetUserRow(userName);
            if (row == null)
                return false;
            else
            {
                if (Map == null)
                {
                    return false;
                }
                return ValidateUser(Convert.ToInt32(Map.Id), Convert.ToInt32(row["ID"]));
            }
        }

        private bool ValidateUser(int appID, int userId)
        {
            string sql = string.Format("select Cast( case when exists(select 1 from durados_App where durados_app.[ToDelete]=0 AND  Id = {0} and Creator = {1}) or exists(select 1 from dbo.durados_UserApp where  AppId = {0} and UserId = {1}) then 1 else 0 end as bit)", appID, userId);
            string scalar = (new SqlAccess()).ExecuteScalar(Maps.Instance.DuradosMap.Database.ConnectionString, sql);

            if (string.IsNullOrEmpty(scalar))
                return false;
            else
                return Convert.ToBoolean(scalar);
                   
        }


        public class AuthenticatedUserInfo
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        private bool LDAPAuth(string path, string domainUser, string ldapConnectionString, string pwd, out AuthenticatedUserInfo userInfo)
        {
            userInfo = null;
            try
            {

                LdapDirectoryIdentifier ldapDir = new LdapDirectoryIdentifier(path, false, false);
                LdapConnection _ldapConn = new LdapConnection(ldapDir);
                _ldapConn.AuthType = AuthType.Basic;
                _ldapConn.Credential = new NetworkCredential(domainUser, pwd);
                _ldapConn.SessionOptions.SecureSocketLayer = false;
                _ldapConn.Timeout = new TimeSpan(0, 0, 30);
                _ldapConn.Bind();

                try
                {
                    DirectoryEntry objOU = new DirectoryEntry(ldapConnectionString);
                    string firstName = null;
                    try
                    {
                        firstName = objOU.Properties[GetLDFirstName()].Value.ToString();
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log("AccountMembershipService", "LDAPAuth-get firstName using: " + GetLDFirstName(), exception.Source, exception, 1, exception.Message);
                    }
                    string lastName = null;
                    try
                    {
                        lastName = objOU.Properties[GetLDLastName()].Value.ToString();
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log("AccountMembershipService", "LDAPAuth-get lastName using: " + GetLDLastName(), exception.Source, exception, 1, exception.Message);
                    }
                    
                    string email = null;
                    try
                    {
                        email = objOU.Properties[GetLDEmail()].Value.ToString();
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log("AccountMembershipService", "LDAPAuth-get email using: " + GetLDEmail(), exception.Source, exception, 1, exception.Message);
                    }
                    userInfo = new AuthenticatedUserInfo() { FirstName = firstName, LastName = lastName, Email = email };
                }
                catch (Exception exception)
                {
                    Map.Logger.Log("AccountMembershipService", "LDAPAuth-get user info", exception.Source, exception, 1, exception.Message);
                }
            }
            catch (Exception exception)
            {
                Map.Logger.Log("AccountMembershipService", "LDAPAuth", exception.Source, exception, 1, exception.Message);
                return false;
            }

            return true;
        }

        private string GetLDFirstName()
        {
            return System.Configuration.ConfigurationManager.AppSettings["LDFirstName"] ?? "givenName";
        }

        private string GetLDLastName()
        {
            return System.Configuration.ConfigurationManager.AppSettings["LDLastName"] ?? "sn";
        }

        private string GetLDEmail()
        {
            return System.Configuration.ConfigurationManager.AppSettings["LDEmail"] ?? "mail";
        }


        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword, bool userIsOnline)
        {
            MembershipUser currentUser = _provider.GetUser(userName, userIsOnline);
            
            return currentUser.ChangePassword(oldPassword, newPassword);
        }

        public string GetRandomPassword(int chars)
        {
            return AccountService.GetRandomPassword(chars);
        }
        
        #region forgot password
        
        public bool PasswordResetEnabled
        {
            get
            {
                return _provider.EnablePasswordReset;
            }
        }

        public bool RequiresQuestionAndAnswer
        {
            get
            {
                return _provider.RequiresQuestionAndAnswer;
            }
        }

        public string GetUserQuestion(string userName)
        {
            MembershipUser user = _provider.GetUser(userName, false);
            if (user == null)
            {
                throw new DuradosException("User name not found");
            }
            else
            {
                return user.PasswordQuestion;
            }
        }
        public string GetUserEmail(string userName)
        {
            MembershipUser user = _provider.GetUser(userName, false);
            if (user == null)
            {
                throw new DuradosException("User name not found");
            }
            else
            {
                return user.Email;
            }
        }

        public string ResetPassword(string userName)
        {
            MembershipUser user = _provider.GetUser(userName, false);
            if (user == null)
            {
                throw new DuradosException(Map.Database.Localizer.Translate("User name not found"));
            }
            else
            {
                // Reset the password.
                _provider.UnlockUser(userName);
                return _provider.ResetPassword(userName, null);

             
            }
        }

        public bool ValidateUserExists(string userName)
        {
            bool success = false;
            MembershipUser user = _provider.GetUser(userName, false);
            if (user != null)
                success = true;
            return success;
        }
       
        
        
        

        #endregion


        #region IMembershipService Members


        public bool UnlockUser(string username)
        {
           return  _provider.UnlockUser(username);
        }

        public bool IsLockedOut(string username)
        {
            MembershipUser user = _provider.GetUser(username, false);
            if (user == null)
                return false;

            return user.IsLockedOut;
        }

        #endregion
    }

    

}
