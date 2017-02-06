using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;

namespace Durados.Web.Mvc
{
    public partial class Database : Durados.Database, Durados.Services.IStyleable
    {
        public Map Map;

        public const string DefaultPageContentKey = "DefaultPageContent";

        public override Dictionary<string, Dictionary<string, string>> ForeignKeys
        {
            get
            {
                return Map.ForeignKeys;
            }

        }

        public View FirstView
        {
            get
            {
                View userFirstView = GetUserFirstView();
                if (userFirstView == null)
                    return null;
                //return (View)base.FirstView;
                else
                    return userFirstView;
            }
        }

        public override string DefaultStepMessage
        {
            get
            {
                return Localizer.Translate(base.DefaultStepMessage);
            }
            set
            {
                base.DefaultStepMessage = value;
            }
        }

        public override string DefaultWorkFlowCompletedMessage
        {
            get
            {
                return Localizer.Translate(base.DefaultWorkFlowCompletedMessage);
            }
            set
            {
                base.DefaultWorkFlowCompletedMessage = value;
            }
        }

        public Database(DataSet dataSet, Map map) :
            base(dataSet)
        {
            Map = map;
            Salt = GetFtpSalt();

            DefaultMasterKeyPassword = GetDefaultMasterKeyPassword();
            DefaultSymmetricKeyName = GetDefaultSymmetricKeyName();
            DefaultCertificateName = GetDefaultCertificateName();
            DefaultSymmetricKeyAlgorithm = SymmetricKeyAlgorithm.TRIPLE_DES;
            MaxInvalidPasswordAttempts = 5;
            MinRequiredPasswordLength = 6;
            MinRequiredNonalphanumericCharacters = 0;
            PasswordAttemptWindow = 10;
            PasswordStrengthRegularExpression = string.Empty;
            RequiresQuestionAndAnswer = false;

            EnableUserRegistration = true;
        }

        public int GetWorkspaceIndex(Workspace workspace)
        {
            int i = 0;
            foreach (Workspace w in Map.Database.Workspaces.Values.Where(w => w.ID != Map.Database.GetAdminWorkspaceId()).OrderBy(w => w.Ordinal))
            {
                i++;
                if (w.Equals(workspace))
                    return i;
            }

            return 0;
        }

        private string GetFtpSalt()
        {
            return System.Configuration.ConfigurationManager.AppSettings["FtpSalt"] + "_durados2008ftp";
        }

        private string GetDefaultMasterKeyPassword()
        {
            return System.Configuration.ConfigurationManager.AppSettings["MasterKeyPassword"] ?? "dura2008";
        }

        private string GetDefaultSymmetricKeyName()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SymmetricKeyName"] ?? "DuradosSymmetricKey";
        }

        private string GetDefaultCertificateName()
        {
            return System.Configuration.ConfigurationManager.AppSettings["CertificateName"] ?? "DuradosCertificate";
        }

        public override bool UseSmtpDefaultCredentials
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseSmtpDefaultCredentials"] ?? "false");

            }
        }

        protected override void Init()
        {
            base.Init();

            MvcInit();
        }

        protected virtual void MvcInit()
        {
            UploadFolder = "/Uploads/";
            DefaultController = "Home";
            DefaultIndexAction = "Index";
            DefaultCheckListAction = "CheckList";
            DefaultCreateAction = "Create";
            DefaultRefreshAction = "Refresh";
            DefaultCreateOnlyAction = "CreateOnly";
            DefaultEditAction = "Edit";
            DefaultDuplicateAction = "Duplicate";
            DefaultEditRichAction = "EditRich";
            DefaultGetRichAction = "GetRich";
            DefaultEditOnlyAction = "EditOnly";
            DefaultGetJsonViewAction = "GetJsonView";
            DefaultGetSelectListAction = "GetSelectList";
            DefaultDeleteAction = "Delete";
            DefaultDeleteSelectionAction = "DeleteSelection";
            DefaultEditSelectionAction = "EditSelection";
            DefaultFilterAction = "Filter";
            DefaultUploadAction = "Upload";
            DefaultExportToCsvAction = "ExportToCsv";
            DefaultPrintAction = "Print";
            DefaultAutoCompleteController = "Home";
            DefaultAutoCompleteAction = "AutoComplete";
            DefaultSetLanguageAction = "SetLanguage";
            DefaultInlineAddingDialogAction = "InlineAddingDialog";
            DefaultInlineAddingCreateAction = "InlineAddingCreate";
            DefaultInlineEditingDialogAction = "InlineEditingDialog";
            DefaultInlineEditingEditAction = "InlineEditingEdit";
            DefaultInlineDuplicateDialogAction = "InlineDuplicateDialog";
            DefaultInlineDuplicateAction = "InlineDuplicate";
            DefaultInlineSearchDialogAction = "InlineSearchDialog";
            DefaultAllFilterValuesAction = "GetAllFilterValues";

            HasChangePassword = true;
            UserViewName = "v_durados_User";
            UsernameFieldName = "Username";
            UserGuidFieldName = "Guid";
            UserIdFieldName = "ID";
            RoleViewName = "durados_UserRole";

            RequiresSSL = true;

            EnableGoogle = true;
            EnableGithub = true;
            EnableFacebook = true;
            EnableTwitter = true;
            EnableAdfs = true;
            EnableAzureAd = true;
            EnableSecretKeyAccess = true;
            TokenExpiration = 86400;
            UseRefreshToken = false;
            EnableTokenRevokation = false;
            FacebookScope = "email";
        }



        public override bool IsMain()
        {
            return Map.IsMainMap;
        }

        public override string EmptyDisplay
        {
            get
            {
                if (Localization != null)
                {
                    return Localizer.Translate(base.EmptyDisplay);
                }
                return base.EmptyDisplay;
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool RequiresSSL { get; set; }


        public override Durados.View CreateView(DataTable dataTable)
        {
            return new Durados.Web.Mvc.View(dataTable, this);
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool EnableTokenRevokation { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public int TokenExpiration { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool UseRefreshToken { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string GoogleClientId { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string GoogleClientSecret { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string TwitterClientId { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string TwitterClientSecret { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string GithubClientId { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string GithubClientSecret { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool EnableGithub { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool EnableGoogle { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool EnableTwitter { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool EnableAdfs { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public bool EnableAzureAd { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string FacebookClientId { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string FacebookClientSecret { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string FacebookScope { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AdfsClientId { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AdfsResource { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AdfsHost { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AzureAdClientId { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AzureAdResource { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AzureAdHost { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public bool EnableFacebook { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public bool EnableSecretKeyAccess { get; set; }

        public HashSet<string> GetSocialProviders()
        {
            HashSet<string> providers = new HashSet<string>();

            if (EnableGoogle)
            {
                providers.Add("google");
            }
            if (EnableGithub)
            {
                providers.Add("github");
            }
            if (EnableFacebook)
            {
                providers.Add("facebook");
            }
            if (EnableTwitter)
            {
                providers.Add("twitter");
            }
            if (EnableAdfs)
            {
                providers.Add("adfs");
            }
            if (EnableAzureAd)
            {
                providers.Add("azuread");
            }

            return providers;
        }


        [Durados.Config.Attributes.ColumnProperty()]
        public string RegistrationRedirectUrl { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string SignInRedirectUrl { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ForgotPasswordUrl { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string NewUserDefaultRole { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool ApproveNewUsersManually { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool EnableUserRegistration { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string UploadFolder { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ActiveDirectoryServer { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ActiveDirectoryDomain { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ActiveDirectoryConnectionString { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultController { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultIndexAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultAllFilterValuesAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultCheckListAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultCreateAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultRefreshAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultCreateOnlyAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultEditAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultDuplicateAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultEditRichAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultGetRichAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultEditOnlyAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultDeleteAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultDeleteSelectionAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultEditSelectionAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultFilterAction { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultUploadAction { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultGetJsonViewAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultGetSelectListAction { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultAutoCompleteAction { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultAutoCompleteController { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultInlineAddingDialogAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultInlineAddingCreateAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultInlineEditingDialogAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultInlineEditingEditAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultInlineDuplicateDialogAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultInlineDuplicateAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultInlineSearchDialogAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultExportToCsvAction { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultPrintAction { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultSetLanguageAction { get; set; }


        private Durados.Localization.LocalizationConfig localization = null;

        [Durados.Config.Attributes.ParentProperty(TableName = "Localization")]
        public Durados.Localization.LocalizationConfig Localization
        {
            get
            {
                return GetLocalization();
            }
            set
            {
                localization = value;
            }
        }

        protected virtual Durados.Localization.LocalizationConfig GetLocalization()
        {
            return localization;
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DenyLocalizationConfigRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AllowConfigConfigRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool HasChangePassword { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool HideMyStuff { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string UserViewName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string RoleViewName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string RefreshUrl { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string UsernameFieldName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string UserGuidFieldName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string UserIdFieldName { get; set; }

        [Durados.Config.Attributes.ParentProperty(TableName = "SiteInfo")]
        public SiteInfo SiteInfo { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string NewButtonDescription { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string EditButtonDescription { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DuplicateButtonDescription { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Enables to replace the default Powered by logo")]
        public string PoweredByLogo { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Enables to replace the default Powered by logo")]
        public string PoweredByUrl { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        //public bool AutoCommit { get; set; }


        //[Durados.Config.Attributes.ParentProperty(TableName = "Upload")]
        //public Upload Upload { get; set; }

        public List<View> GetVisibleViews()
        {
            List<View> views = new List<View>();

            foreach (View view in Views.Values.OrderBy(f => f.Order).ToList())
            {
                if (view.IsVisible())
                {
                    views.Add(view);
                }
            }

            return views;
        }

        public virtual View GetRoleView()
        {
            if (Views.ContainsKey(RoleViewName))
            {
                return (View)Views[RoleViewName];
            }
            else
            {
                return null;
            }
        }

        public Durados.View GetDevUserView()
        {
            return Maps.Instance.DuradosMap.Database.Views[UserViewName];
            
        }

        public override Durados.View GetUserView()
        {
            if (Views.ContainsKey(UserViewName))
            {
                return (View)Views[UserViewName];
            }
            else
            {
                return null;
            }
        }

        public virtual Field GetUsernameField()
        {
            View userView = (View)GetUserView();
            if (userView == null)
                return null;

            if (userView.Fields.ContainsKey(UsernameFieldName))
            {
                return userView.Fields[UsernameFieldName];
            }
            else
            {
                return null;
            }
        }

        public virtual DataRow GetUserRow()
        {
            //View userView = GetUserView();
            //if (userView == null)
            //    return null;

            //Field usernameField = GetUsernameField();
            //if (usernameField == null)
            //{
            //    return null;
            //}
            string username = GetCurrentUsername();
            if (username == null)
                return null;
            return GetUserRow(username);

        }

        public override string SysDbConnectionString
        {
            get
            {
                return Map.systemConnectionString;
            }
        }
        public override string SecurityDbConnectionString
        {
            get
            {
                return Map.securityConnectionString;
            }
        }

        private bool? identicalSystemConnection = null;
        public override bool IdenticalSystemConnection
        {
            get
            {
                if (!identicalSystemConnection.HasValue)
                {
                    identicalSystemConnection = Map.systemConnectionString.Equals(Map.connectionString);
                }
                return identicalSystemConnection.Value;
            }
        }

        private object GetValueFromRequestCache(string key)
        {
            if (System.Web.HttpContext.Current.Items.Contains(key))
                return System.Web.HttpContext.Current.Items[key];

            return null;

        }

        private void SetValueFromRequestCache(string key, object value)
        {
            if (System.Web.HttpContext.Current.Items.Contains(key))
                System.Web.HttpContext.Current.Items[key] = value;
            else
                System.Web.HttpContext.Current.Items.Add(key, value);

        }
        public virtual DataRow GetUserRow(string username, bool withoutCache = false)
        {
            string key = Map.AppName + "_userRow_" + username;
            object o = (GetValueFromRequestCache(key));
            if (!withoutCache && o != null)
                return (DataRow)o;

            if (string.IsNullOrEmpty(username) && SecureLevel == Durados.SecureLevel.AllUsers)
            {
                username = Database.GuestUsername;
            }

            View userView = (View)GetUserView();
            if (userView == null)
                return null;

            Field usernameField = GetUsernameField();
            if (usernameField == null)
            {
                return null;
            }
            DataRow userRow = userView.GetDataRow(usernameField, username, false);
            if (userRow == null)
                userRow = GetDevUserRow(username);

            SetValueFromRequestCache(key, userRow);

            return userRow;
        }

        public virtual DataRow GetDevUserRow(string username)
        {
            if (!IsDevUser(username))
                return null;
            View userView = (View)GetDevUserView();
            Field usernameField = userView.Fields[UsernameFieldName];
            DataRow userRow = userView.GetDataRow(usernameField, username, false);

            
            return userRow;
        }

        private bool IsDevUser(string username)
        {
            return Maps.IsDevUser(username);
        }

        public virtual DataRow GetUserRow(int userId)
        {
            View userView = (View)GetUserView();
            if (userView == null)
                return null;

            return userView.GetDataRow(userId.ToString());
        }

        public virtual DataRow GetRoleRow(string role)
        {
            View roleView = GetRoleView();
            if (roleView == null)
                return null;

            return roleView.GetDataRow(role);


        }

        public virtual int GetUserID(string userName)
        {
            DataRow userRow = Maps.Instance.DuradosMap.Database.GetUserRow(userName);
            int userId = -1;

            if (userRow != null)
            {
                userId = Convert.ToInt32(userRow["ID"]);
            }

            return userId;
        }

        public override string GetUserID()
        {
            string sessionKey = GetSessionKey("-userID");
            object userID = Map.Session[sessionKey];

            if (!Maps.Skin)
            {
                if (userID != null)
                    return userID.ToString();
            }

            View userView = (View)GetUserView();
            if (userView == null)
                return null;

            DataRow userRow = GetUserRow();

            if (userRow == null)
                return null;

            userID = userView.GetPkValue(userRow);

            Map.Session[sessionKey] = userID;

            return userID.ToString();
        }

        public virtual string GetUserGuid()
        {
            View userView = (View)GetUserView();
            if (userView == null)
                return null;

            DataRow userRow = GetUserRow();

            if (userRow == null)
                return null;

            if (userRow.IsNull(UserGuidFieldName))
                return null;

            return userRow[UserGuidFieldName].ToString();
        }

        public virtual string GetUserFullName2(string username)
        {
            if (System.Web.HttpContext.Current == null)
            {
                return GetUserFullName(username);
            }
            if (!System.Web.HttpContext.Current.Items.Contains(Database.FullName))
            {
                System.Web.HttpContext.Current.Items.Add(Database.FullName, GetUserFullName(username));
            }
            return (System.Web.HttpContext.Current.Items[Database.FullName] ?? string.Empty).ToString();
        }

        public virtual string GetUserFullName(string username)
        {
            string fullNameFieldName = "FullName";
            View userView = (View)GetUserView();
            if (userView == null)
                return null;

            DataRow userRow = GetUserRow(username);

            if (userRow == null)
                return null;

            if (!userRow.Table.Columns.Contains(fullNameFieldName))
                fullNameFieldName = UsernameFieldName;

            if (userRow.IsNull(fullNameFieldName))
                return null;

            return userRow[fullNameFieldName].ToString();
        }

        public virtual string GetUserFieldValue(string fieldName, string username)
        {
            object fieldValue = null;

            View userView = (View)GetUserView();
            if (userView == null)
                return string.Empty;

            DataRow userRow = GetUserRow(username);

            if (userRow == null)
                return string.Empty;

            if (!userRow.Table.Columns.Contains(fieldName))
                fieldName = UsernameFieldName;

            if (userRow.IsNull(fieldName))
                return string.Empty;

            fieldValue = userRow[fieldName];

            return fieldValue.ToString();
        }

        public virtual string GetUserFirstName()
        {
            string sessionKey = GetSessionKey("-firstName");
            string firstNameFieldName = "FirstName";
            object firstName = Map.Session[sessionKey];

            if (!Maps.Skin)
            {
                if (firstName != null)
                    return firstName.ToString();
            }

            View userView = (View)GetUserView();
            if (userView == null)
                return string.Empty;

            DataRow userRow = GetUserRow();

            if (userRow == null)
                return string.Empty;

            if (!userRow.Table.Columns.Contains(firstNameFieldName))
                firstNameFieldName = UsernameFieldName;

            if (userRow.IsNull(firstNameFieldName))
                return string.Empty;

            firstName = userRow[firstNameFieldName];

            Map.Session[sessionKey] = firstName;

            return firstName.ToString();
        }

        public virtual string GetUserLastName()
        {
            string sessionKey = GetSessionKey("-lastName");
            string lastNameFieldName = "LastName";
            object lastName = Map.Session[sessionKey];

            if (!Maps.Skin)
            {
                if (lastName != null)
                    return lastName.ToString();
            }

            View userView = (View)GetUserView();
            if (userView == null)
                return string.Empty;

            DataRow userRow = GetUserRow();

            if (userRow == null)
                return string.Empty;

            if (!userRow.Table.Columns.Contains(lastNameFieldName))
                lastNameFieldName = UsernameFieldName;

            if (userRow.IsNull(lastNameFieldName))
                return string.Empty;

            lastName = userRow[lastNameFieldName];

            Map.Session[sessionKey] = lastName;

            return lastName.ToString();
        }

        public virtual string GetUserFullName()
        {
            const string FullNameColumnName = "FullName";
            const string FirstNameColumnName = "FirstName";
            const string LastNameColumnName = "LastName";
            string sessionKey = GetSessionKey("-" + FullNameColumnName);
            object fullName = Map.Session[sessionKey];

            if (!Maps.Skin)
            {
                if (fullName != null)
                    return fullName.ToString();
            }

            View userView = (View)GetUserView();
            if (userView == null)
                return string.Empty;

            DataRow userRow = GetUserRow();

            if (userRow == null)
                return string.Empty;
            if (!userRow.Table.Columns.Contains(FullNameColumnName))
            {
                if (userRow.Table.Columns.Contains(FirstNameColumnName))
                {
                    if (userRow.IsNull(FirstNameColumnName))
                    {
                        fullName = string.Empty;
                    }
                    else
                    {
                        fullName = userRow[FirstNameColumnName];
                    }
                    if (userRow.IsNull(LastNameColumnName))
                    {
                    }
                    else
                    {
                        if (userRow.IsNull(LastNameColumnName))
                        {
                        }
                        else
                        {
                            if (fullName != string.Empty)
                            {
                                fullName += " ";
                            }
                            fullName += userRow[LastNameColumnName].ToString();
                        }
                    }
                }
            }
            else
            {
                if (userRow.IsNull(FullNameColumnName))
                {
                    fullName = string.Empty;
                }
                else
                {
                    fullName = userRow[FullNameColumnName];
                }
            }

            Map.Session[sessionKey] = fullName;

            return fullName.ToString();
        }

        public bool HasRoles
        {
            get
            {
                //return Views.ContainsKey("UserRole");
                return Views.ContainsKey(RoleViewName);
            }
        }

        public virtual string GetUserRole()
        {
            return GetUserRole(null);
        }

        public string GetSessionKey(string suffix)
        {
            string currentUser = GuestUsername;
            //if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && System.Web.HttpContext.Current.User.Identity.Name != null)
            //    currentUser = System.Web.HttpContext.Current.User.Identity.Name;
            string username = GetCurrentUsername();
            if (!string.IsNullOrEmpty(username))
                currentUser = username;

            string sessionKey = Map.AppName + "-" + currentUser + suffix;

            return sessionKey;
        }

        public virtual string GetUserRole2(string username)
        {
            if (System.Web.HttpContext.Current == null)
            {
                return GetUserRole(username);
            }
            if (!System.Web.HttpContext.Current.Items.Contains(Database.UserRole))
            {
                System.Web.HttpContext.Current.Items.Add(Database.UserRole, GetUserRole(username));
            }
            return System.Web.HttpContext.Current.Items[Database.UserRole].ToString();
        }

        public virtual string GetUserRole(string username)
        {
            if (Maps.IsSuperDeveloper(username))
                return "Developer";

            object role = null;
            string sessionKey = GetSessionKey("-role");
            if (UserIsAuthorizedOrAnonymous())
            {
                if (SecureLevel != Durados.SecureLevel.AllUsers)
                {
                    throw new DuradosException("Please enable Anonymous Access.");
                }
                if (string.IsNullOrEmpty(DefaultGuestRole))
                {
                    throw new DuradosException("Please set a default role for anonymous users.");
                }
                return DefaultGuestRole;
            }

            if (((!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["public"]) &&
                  System.Web.HttpContext.Current.Request.QueryString["public"].Equals("true")) ||
                  (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["public2"]) && System.Web.HttpContext.Current.Request.QueryString["public2"].Equals("true"))) &&
                  SecureLevel == Durados.SecureLevel.AllUsers && !string.IsNullOrEmpty(DefaultGuestRole))
            {
                return DefaultGuestRole;
            }

            string key = null;
            if (System.Web.HttpContext.Current.Session != null && username == null)
            {
                role = System.Web.HttpContext.Current.Session[sessionKey];
            }
            else if (System.Web.HttpContext.Current.Session == null && username == null)
            {
                string currentUsername = GetCurrentUsername();
                if (!string.IsNullOrEmpty(currentUsername))
                {
                    key = currentUsername + "_role";
                    if (System.Web.HttpContext.Current.Items.Contains(key))
                    {
                        role = System.Web.HttpContext.Current.Items[key];
                    }
                }
            }

            if (role != null)
                return role.ToString();

            View userView = (View)GetUserView();
            if (userView == null)
                return string.Empty;

            DataRow userRow = null;

            if (username == null)
            {
                userRow = GetUserRow();
            }
            else
            {
                userRow = GetUserRow(username);
            }

            if (userRow == null)
                return string.Empty;

            if (!userRow.Table.Columns.Contains("Role"))
                return string.Empty;

            if (userRow.IsNull("Role"))
                return string.Empty;


            role = userRow["Role"];

            //role = (new SqlAccess()).GetScalar((ParentField) userView.GetFieldByColumnNames("Role")

            if (System.Web.HttpContext.Current.Session != null && username == null)
                System.Web.HttpContext.Current.Session[sessionKey] = role;
            else if (System.Web.HttpContext.Current.Session == null && username == null)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    System.Web.HttpContext.Current.Items[key] = role;
                }
            }
            return role.ToString();
        }

        private static bool UserIsAuthorizedOrAnonymous()
        {
            return (string.IsNullOrEmpty(HttpContext.Current.Request.Headers["Authorization"]) &&
                            !string.IsNullOrEmpty(HttpContext.Current.Request.Headers[Database.AnonymousToken])) ||
                            (!string.IsNullOrEmpty(HttpContext.Current.Request.Headers["Authorization"]) &&
                            HttpContext.Current.Request.Headers["Authorization"].Contains("anonymous"));
        }

        public virtual View GetUserFirstView()
        {
            if (!DefaultLast)
            {
                return null;
            }

            View firstView = GetUserFirstView(null);

            if (firstView == null)
            {
                if (string.IsNullOrEmpty(FirstViewName))
                {
                    return null;
                }
                else if (Views.ContainsKey(FirstViewName))
                {
                    return (View)Views[FirstViewName];
                }
                else
                {
                    return null;
                }
                //firstView = (View)Views["durados_v_MessageBoard"];
            }
            else
                return firstView;
        }

        public virtual View GetUserFirstView(string username)
        {
            string role = GetUserRole(username);

            if (role == null)
                return null;
            //return (View)base.FirstView;

            DataRow roleRow = GetRoleRow(role);

            if (roleRow == null)
                return null;
            //return (View)base.FirstView;

            if (!roleRow.Table.Columns.Contains("FirstView"))
                return null;
            //return (View)base.FirstView;

            if (roleRow.IsNull("FirstView"))
                return null;
            //return (View)base.FirstView;

            string firstViewName = roleRow["FirstView"].ToString();

            if (Views.ContainsKey(firstViewName))
                return (View)Views[firstViewName];
            else
                return null;
            //return (View)base.FirstView;
        }


        public bool IsInRole(string role)
        {
            string userRole = GetUserRole();
            if (string.IsNullOrEmpty(userRole))
                return false;
            return role.Equals(userRole);
        }

        public virtual string GetUsernameByGuid(string guid)
        {
            View userView = (View)GetUserView();
            if (userView == null)
                return null;

            DataRow userRow = userView.GetDataRow(userView.Fields[UserGuidFieldName], guid, false);

            if (userRow == null)
                return null;

            return userRow[UsernameFieldName].ToString();

        }

        public virtual string GetUsernameById(string id)
        {
            View userView = (View)GetUserView();
            if (userView == null)
                return null;

            DataRow userRow = userView.GetDataRow(userView.Fields[UserIdFieldName], id, false);

            if (userRow == null)
                return null;

            return userRow[UsernameFieldName].ToString();

        }

        public virtual string GetGuidByUsername(string username)
        {
            View userView = (View)GetUserView();
            if (userView == null)
                return null;

            DataRow userRow = GetUserRow(username);

            if (userRow == null)
                return null;

            if (userRow.IsNull(UserGuidFieldName))
                return null;

            return userRow[UserGuidFieldName].ToString();
        }

        public virtual string GetEmailByUsername(string username)
        {
            View userView = (View)GetUserView();
            if (userView == null)
                return null;

            DataRow userRow = GetUserRow(username);

            if (userRow == null)
                return null;

            if (userRow.IsNull("Email"))
                return null;

            return userRow["Email"].ToString();
        }

        protected virtual string GetPasswordFieldName()
        {
            return "Password";
        }

        public virtual void SaveUserPassword(string newPassword)
        {
            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Views[UserViewName];

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(GetPasswordFieldName(), newPassword);
            view.Edit(values, view.GetPkValue(GetUserRow()), null, null, null, null);
        }

        public string GetTitle()
        {
            if (Map.IsMainMap)
                return Durados.Database.ShortProductName;

            string title = SiteInfo.Product;
            if (Maps.MultiTenancy && title == string.Empty)
                title = Maps.GetCurrentAppName();
            return title;
        }

        public override int Plan
        {
            get
            {
                if (Map.Plan == 0)
                {
                    return 1;
                }
                return Map.Plan;
            }
            set
            {
            }
        }

        public override Guid Guid
        {
            get
            {
                return Map.Guid;
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string PlanContent { get; set; }

        public virtual string GetPlanContent()
        {
            if (string.IsNullOrEmpty(PlanContent))
            {
                return ((DuradosMap)Maps.Instance.DuradosMap).GetPlanContent();
            }


            return PlanContent;
        }

        public override string GetCurrentUsername()
        {
            if (System.Web.HttpContext.Current == null)
                return string.Empty;

            if (System.Web.HttpContext.Current.Items.Contains(Database.Username) && System.Web.HttpContext.Current.Items[Database.Username] != null)
            {
                return System.Web.HttpContext.Current.Items[Database.Username].ToString();
            }

            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && System.Web.HttpContext.Current.User.Identity.Name != null)
                return System.Web.HttpContext.Current.User.Identity.Name;
            else
                return string.Empty;
        }

        public override object GetCurrentUserId()
        {
            try
            {
                if (!System.Web.HttpContext.Current.Items.Contains(Database.CurrentUserId))
                {
                    if (this.Views.ContainsKey("users"))
                    {
                        View usersView = (View)this.Views["users"];
                        Field emailField = usersView.GetFieldByColumnNames("email");
                        if (emailField != null)
                        {
                            DataRow row = usersView.GetDataRow(emailField, GetCurrentUsername());
                            if (row != null)
                            {
                                string pk = usersView.GetPkValue(row);
                                if (!string.IsNullOrEmpty(pk))
                                {
                                    System.Web.HttpContext.Current.Items[Database.CurrentUserId] = pk;
                                }
                            }
                        }
                    }
                }

                if (System.Web.HttpContext.Current.Items[Database.CurrentUserId] == null)
                    return null;

                return System.Web.HttpContext.Current.Items[Database.CurrentUserId];
            }
            catch
            {
                return null;
            }
        }

        public override Guid GetAnonymousToken()
        {
            return Map.AnonymousToken;
        }

        public override string GetCurrentAppName()
        {
            if (Map != null && !string.IsNullOrEmpty(Map.AppName))
                return Map.AppName;

            if (System.Web.HttpContext.Current.Items[Database.AppName] == null)
                return null;

            return System.Web.HttpContext.Current.Items[Database.AppName].ToString();
        }

        public override Durados.Database GetMainDatabase()
        {
            return Maps.Instance.DuradosMap.Database;
        }
        public override bool IsAllow(Durados.Services.ISecurable securable)
        {
            return !UI.Helpers.SecurityHelper.IsDenied(null, securable.AllowSelectRoles);// || UI.Helpers.SecurityHelper.IsConfigViewForViewOwner(securable);
        }
        public bool IsAllowMenu(SpecialMenu menu)
        {
            bool isAllowView = menu.LinkType.Equals(LinkType.View) && (string.IsNullOrEmpty(menu.ViewName) || (Map.Database.Views.ContainsKey(menu.ViewName) && Map.Database.Views[menu.ViewName].IsAllow()) || (Map.GetConfigDatabase().Views.ContainsKey(menu.ViewName) && Map.GetConfigDatabase().Views[menu.ViewName].IsAllow()));
            bool isAllowedPage = menu.LinkType.Equals(LinkType.Page) && !string.IsNullOrEmpty(menu.ViewName) && (Map.Database.Pages.ContainsKey(Convert.ToInt32(menu.ViewName)) && Map.Database.Pages[Convert.ToInt32(menu.ViewName)].IsAllow());
            bool isNotViewOrPage = !menu.LinkType.Equals(LinkType.View) && !menu.LinkType.Equals(LinkType.Page);
            return isAllowView || isAllowedPage || isNotViewOrPage;
        }

        internal string GetDashboardUrl()
        {
            return "/Home/ChartsDashboard?Id=";
        }


        [Durados.Config.Attributes.ColumnProperty(Description = "General CSS file")]
        public virtual string StyleSheets { get; set; }
        [Durados.Config.Attributes.ColumnProperty(Description = "General JS file")]
        public virtual string JavaScripts { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Log On Authentication Url")]
        public string LogOnUrlAuth { get; set; }
        public string[] LogOnUrlAuthToken
        {
            get
            {
                if (string.IsNullOrEmpty(LogOnUrlAuth))
                    return null;
                else
                {
                    // string qs = string.Join(string.Empty, LogOnUrlAuth.Split('|').Skip(2).ToArray());
                    string queryString = string.Join(string.Empty, LogOnUrlAuth.Split('?').Skip(1).ToArray());
                    System.Collections.Specialized.NameValueCollection nv = System.Web.HttpUtility.ParseQueryString(queryString);
                    return nv.AllKeys;
                }
            }
        }
        public string LogOnUrlAuthBase
        {
            get
            {
                if (string.IsNullOrEmpty(LogOnUrlAuth))
                    return null;
                else
                {
                    // string qs = string.Join(string.Empty, LogOnUrlAuth.Split('|').Skip(2).ToArray());
                    return LogOnUrlAuth.Split('?')[0];

                }
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Console Administror Email")]
        public string AdminEmail { get; set; }
        [Durados.Config.Attributes.ColumnProperty(Description = "Console Administror Email")]
        public string UserPreviewUrl { get; set; }


        public override Durados.Data.IDataAccess GetDataAccess(string connectionString)
        {
            return DataAccessFactory.GetDataAccess(connectionString);
        }

        bool? isApi = null;
        public override bool IsApi()
        {
            if (!isApi.HasValue)
            {
                string s = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
                isApi = !string.IsNullOrEmpty(s);
            }
            return isApi.Value;
        }

        public override string GetConnectionSource()
        {
            return RestHelper.GetConnectionSource(Map.AppName);
        }

        public override Dictionary<string, object> GetConfigDictionary()
        {
            if (string.IsNullOrEmpty(Config))
                return new Dictionary<string, object>();
            try
            {
                return Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(Config);
            }
            catch (Exception exception)
            {
                throw new DuradosException("Failed to parse the configuration JSON: " + exception.Message, exception);
            }
        }

        public System.Collections.ArrayList GetModel()
        {
            ArrayList list = new ArrayList();

            foreach (View view in Views.Values.Where(v => !v.SystemView).OrderBy(v => v.Order))
            {
                list.Add(new Dictionary<string, object>() { { "name", view.JsonName }, { "fields", view.GetModelFieldsTypes() } });
            }

            return list;
        }
        public string Decrypt(string token)
        {
            string text = null;
            Map duradosMap = Maps.Instance.DuradosMap;
            try
            {
                text = Durados.Security.CipherUtility.Decrypt<System.Security.Cryptography.AesManaged>(token, duradosMap.Database.DefaultMasterKeyPassword, duradosMap.Database.Salt);
            }
            catch
            {
                text = Durados.Security.CipherUtility.Decrypt<System.Security.Cryptography.AesManaged>(System.Web.HttpContext.Current.Server.UrlDecode(token), duradosMap.Database.DefaultMasterKeyPassword, duradosMap.Database.Salt);
            }
            return text;
        }

        public override string GetAuthorization()
        {
            return Map.GetAuthorization();
        }

        public override object GetLimit(Limits limit)
        {
            return Map.GetLimit(limit);
        }

        public override bool EnableDecryption
        {
            get
            {
                return System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items[Database.EnableDecryptionKey] != null && System.Web.HttpContext.Current.Items[Database.EnableDecryptionKey].Equals(true);
            }
        }

        public override string GetCacheValue(string key)
        {
            return Map.GetCacheValue(key);
        }

        public override void SetCacheValue(string key, string value, int milliseconds)
        {
            Map.SetCacheValue(key, value, milliseconds);
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "max invalid password attempts")]
        public int MaxInvalidPasswordAttempts { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "min required password length")]
        public int MinRequiredPasswordLength { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "min required non alphanumeric characters")]
        public int MinRequiredNonalphanumericCharacters { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "password attempt window")]
        public int PasswordAttemptWindow { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "password strength regular expression")]
        public string PasswordStrengthRegularExpression { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "requires question and answer")]
        public bool RequiresQuestionAndAnswer { get; set; }
        
        
        

        public override NameValueCollection GetSecuritySettings(string appId)
        {
    
            NameValueCollection _config = new NameValueCollection();
            _config.Add("connectionStringName", "SecurityConnectionString");
            _config.Add("enablePasswordRetrieval", "false");
            _config.Add("enablePasswordReset", "true");
            _config.Add("requiresQuestionAndAnswer", RequiresQuestionAndAnswer ? "true" : "false");
            _config.Add("requiresUniqueEmail", "false");
            _config.Add("passwordFormat", "Hashed");
            _config.Add("maxInvalidPasswordAttempts", MaxInvalidPasswordAttempts.ToString());
            _config.Add("minRequiredPasswordLength", MinRequiredPasswordLength.ToString());
            _config.Add("minRequiredNonalphanumericCharacters", MinRequiredNonalphanumericCharacters.ToString());
            _config.Add("passwordAttemptWindow", PasswordAttemptWindow.ToString());
            _config.Add("passwordStrengthRegularExpression", PasswordStrengthRegularExpression);
            _config.Add("applicationName", appId);
            return _config;
        }

    }
}
