using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using System.Net;
using Durados.Web.Mvc.UI.Json;
using System.Globalization;
using System.Configuration;

namespace Durados.Web.Mvc.Controllers
{
    public class WixPlugInController : PlugInController
    {

        public class WixParameterNames : ParameterNames
        {
            public string instance { get { return "instance"; } }
            public string width { get { return "width"; } }
            public string locale { get { return "locale"; } }
            public string origCompId { get { return "origCompId"; } }
            public string compId { get { return "compId"; } }

            public override List<string> GetNames()
            {
                return new List<string>() { instance, width, locale, origCompId, compId };
            }

        }

        private static SqlAccess sqlAccess = null;
        private static SqlAccess SqlAccess
        {
            get
            {
                if (sqlAccess == null)
                    sqlAccess = new SqlAccess();

                return sqlAccess;
            }
        }

        private static AccountMembershipService membershipService = null;
        private static AccountMembershipService MembershipService
        {
            get
            {
                if (membershipService == null)
                    membershipService = new AccountMembershipService();

                return membershipService;
            }
        }

        protected override ParameterNames GetParameterNames()
        {
            return new WixParameterNames();
        }

        public override PlugInType PlugInType
        {
            get
            {
                return PlugInType.Wix;
            }
        }

        protected override string GetPlugInUserId()
        {
            return WixPlugInHelper.GetInstance(Request.QueryString["instance"]).uid.ToString().ToUpper();
        }

        protected override bool IsOwner()
        {
            return WixPlugInHelper.GetInstance(Request.QueryString["instance"]).permissions == "OWNER";
        }

        protected override int GetPlanId()
        {
            return WixPlugInHelper.GetPlanId(Request.QueryString["instance"]);
        }

        protected override string GetIdFromParameters()
        {
            WixInstance wixInstance = WixPlugInHelper.GetInstance(Request.QueryString["instance"]);

            string compId = Request.QueryString["origCompId"] ?? Request.QueryString["compId"];

            return wixInstance.instanceId + compId;

        }

        protected override string GetSiteIdFromParameters()
        {
            WixInstance wixInstance = WixPlugInHelper.GetInstance(Request.QueryString["instance"]);

            return wixInstance.instanceId.ToString();

        }

        /// <summary>
        /// Get details of current web master access settings- By check if master user is exists
        /// </summary>
        /// <param name="viewPk"></param>
        /// <returns></returns>
        public JsonResult GetWebMasterAccessDetails(string viewPk)
        {
            string instance = Request.QueryString["instance"];
            ConfigAccess configAccess = new ConfigAccess();
            string viewName = configAccess.GetViewNameByPK(viewPk, Map.GetConfigDatabase().ConnectionString);
            WebMasterAccess webMasterAccess = WixPlugInHelper.GetWebMasterAccessDetails(Map, instance, viewName);
            
            return Json(webMasterAccess);
        }

        /// <summary>
        /// Set web master access for a specific view, and protect it by password
        /// </summary>
        /// <param name="viewPk"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public JsonResult SetWebMasterAccess(string viewPk, string password)
        {
            //Validate before set
            validateSetWebMasterAccess(password);

            //Get new user name
            DataRow userRow = Maps.Instance.DuradosMap.Database.GetUserRow();
            if (userRow == null)
            {
                throw new DuradosException("User not registered");
            }
            string userId = Convert.ToString(userRow["ID"]);
            string userNameFormat = ConfigurationManager.AppSettings["MasterUserNameFormat"] ?? "master{0}@modubiz.com";
            string userName = string.Format(userNameFormat, userId);

            //Create new master user
            bool existInUserAppTable = isUserExistInUserAppTable(userName);
            bool existInUsersTable = Maps.Instance.DuradosMap.Database.GetUserRow(userName) != null;
            bool existInCurrentAppUsersTable = Map.Database.GetUserRow(userName) != null;
            bool existInMembership = MembershipService.ValidateUserExists(userName);

            CreateNewMasterUser(userName, password, !existInUsersTable, !existInMembership, !existInCurrentAppUsersTable, !existInUserAppTable);

            //Change web master access password if neccessary
            changeWebMasterPassword(existInMembership, existInUsersTable, userName, password);

            //Get WebMasterUrl;
            string viewName = new ConfigAccess().GetViewNameByPK(viewPk, Map.GetConfigDatabase().ConnectionString);
            string WebMasterUrl = WixPlugInHelper.GetWebAccessUrl(Map.Url, viewName, userName);
            return Json(WebMasterUrl);
        }

        /// <summary>
        /// Check some validations before set web master access
        /// </summary>
        /// <param name="password"></param>
        private void validateSetWebMasterAccess(string password)
        {
            if (!WixPlugInHelper.ValidateSignedRequest(Request.QueryString["instance"]))
            {
                throw new DuradosException("Wix instance not registered");
            }

            if (GetPlanId() == (int)PlugInHelper.FreePlan)
            {
                throw new DuradosException("Cannot set web master access for free plugin plan");
            }

            if (password == null || password.Length < MembershipService.MinPasswordLength)
            {
                throw new DuradosException(String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }
        }

        /// <summary>
        /// Check if user exist in UserAppTable
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private bool isUserExistInUserAppTable(string userName)
        {
            bool isUserExistInUserAppTable = false;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            int userId = Map.Database.GetUserID(userName);

            if (userId > -1)
            {
                parameters = new Dictionary<string, object>();
                parameters.Add("UserId", userId);
                parameters.Add("AppId", Map.Id);

                string userAppId = sqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.Database.ConnectionString, "select [Id] from durados_UserApp where [UserId] = @UserId and [AppId] = @AppId", parameters);
                if (!string.IsNullOrEmpty(userAppId))
                {
                    isUserExistInUserAppTable = true;
                }
            }

            return isUserExistInUserAppTable;
        }

        /// <summary>
        /// Change web master access password if neccessary
        /// </summary>
        /// <param name="changeMembershipPassword"></param>
        /// <param name="changeUserPasswordInDB"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        private void changeWebMasterPassword(bool changeMembershipPassword, bool changeUserPasswordInDB, string userName, string password)
        {

            //Get newUser id
            DataRow newUserRow = Maps.Instance.DuradosMap.Database.GetUserRow(userName);
            if (newUserRow == null)
            {
                throw new DuradosException("Error in create new web master user");
            }

            int newUserId = Convert.ToInt32(newUserRow["ID"]);

            //change password in Membership
            if (changeMembershipPassword)
            {
                string currentEncriptedPassword = Convert.ToString(newUserRow["Password"]);
                string currentPassword = CryptorHelper.Decrypt(currentEncriptedPassword, true);

                if (!changeUserPasswordInDB || !MembershipService.ChangePassword(userName, currentPassword, password, false))
                {
                    currentPassword = MembershipService.ResetPassword(userName);
                    MembershipService.ChangePassword(userName, currentPassword, password, false);
                }
            }

            //change password in DB
            if (changeUserPasswordInDB)
            {
                string encryptedPassword = CryptorHelper.Encrypt(password, true);
                Dictionary<string, object> updatedValues = new Dictionary<string, object>();

                updatedValues.Add("Password", encryptedPassword);

                SqlGeneralAccess.Update(updatedValues, "durados_User", "ID=" + newUserId.ToString(), Maps.Instance.DuradosMap.Database.ConnectionString);
            }

            MembershipService.UnlockUser(userName);
        }

        /// <summary>
        /// Create new master user in membership service, in modubiz db, in curent app db
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="createInUsersTable"></param>
        /// <param name="CreateInMembership"></param>
        /// <param name="createInCurrentAppUsersTable"></param>
        /// <param name="createInUserAppTable"></param>
        private void CreateNewMasterUser(string userName, string password, bool createInUsersTable, bool CreateInMembership, bool createInCurrentAppUsersTable, bool createInUserAppTable)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string email = "wix@modubiz.com";
            string role = "User";
            string encryptedPassword = CryptorHelper.Encrypt(password, true);

            if (createInUsersTable)
            {
                string firstName = "wix";
                string lastName = "wix";
                Guid guid = Guid.NewGuid();
                string sql = "INSERT INTO [durados_User] ([Username],[FirstName],[LastName],[Email],[Role],[Guid], [Password]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid, @Password); SELECT IDENT_CURRENT(N'[durados_User]') AS ID ";

                parameters.Add("@Email", email);
                parameters.Add("@Username", userName);
                parameters.Add("@Password", encryptedPassword);
                parameters.Add("@FirstName", firstName);
                parameters.Add("@LastName", lastName);
                parameters.Add("@Role", role);
                parameters.Add("@Guid", guid);

                object scalar = SqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.Database.ConnectionString, sql, parameters);
            }

            if (createInUserAppTable)
            {
                parameters = new Dictionary<string, object>();
                parameters.Add("newUser", userName);
                parameters.Add("appName", Map.AppName);
                parameters.Add("role", role);
                sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, "durados_NewAppAsignment @newUser, @appName, @role", parameters, null);
            }

            if (createInCurrentAppUsersTable)
            {
                int userId = Map.Database.GetUserID(userName);

                if (userId == -1)
                {
                    throw new DuradosException("Problem with get user detalis");
                }

                PlugInHelper.AddUserToApp(Convert.ToInt32(Map.Id), userId, role);
            }

            if (CreateInMembership)
            {

                System.Web.Security.MembershipCreateStatus createStatus = (new Durados.Web.Mvc.Controllers.AccountMembershipService()).CreateUser(userName, password, email);
                if (createStatus == System.Web.Security.MembershipCreateStatus.Success)
                {
                    System.Web.Security.Roles.AddUserToRole(userName, role);

                }
            }
        }
    }
}



