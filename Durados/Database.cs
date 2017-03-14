using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;

namespace Durados
{
    [Durados.Config.Attributes.ClassConfig()]
    public partial class Database
    {

        private const string DATE_FORMAT = "MM/dd/yyyy";

        public static readonly string UserPlaceHolder = "[m_User]";
        public static readonly string UsernamePlaceHolder = "[m_Username]";
        public static readonly string NullInt = (-10000000).ToString();
        public static readonly DateTime NullDate = DateTime.MinValue;
        public static readonly string RolePlaceHolder = "[m_Role]";
        public static readonly string CurrentDatePlaceHolder = "[m_CurrentDate]";
        public static readonly DateTime NaDate = new DateTime(1930, 1, 1);
        public static readonly string GuestUsername = "Guest";
        public static readonly string IPAddressPlaceHolder = "[m_CurrentIP]";
        public static readonly string LongProductName = "backand";
        public static readonly string ProductShorthandName = "b";
        public static readonly string ShortProductName = "Back&";
        public static readonly string SystemRelatedViewPrefix = "enum_";

        public static readonly string DictionaryPrefix = "{{";
        public static readonly string DictionaryPostfix = "}}";
        public static readonly string SysPlaceHolder = "sys::";
        public static readonly string ConfigPlaceHolder = "config::";
        public static readonly string SysUserPlaceHolder = SysPlaceHolder + "user";
        public static readonly string SysUsernamePlaceHolder = SysPlaceHolder + "username";
        public static readonly string SysRolePlaceHolder = SysPlaceHolder + "role";
        public static readonly string SysPrevPlaceHolder = "prev:";

        public static readonly string __metadata = "__metadata";

        public static readonly string backand_serverAuthorizationAttempt = "backand_serverAuthorizationAttempt";
        public static readonly string backand_appNameEmpty = "backand_appNameEmpty";
        public static readonly string backand_appNotInCache = "backand_appNotInCache";
        public static readonly string backand_appGuidNotMatch = "backand_appGuidNotMatch";
        public static readonly string AppName = "appname";
        public static readonly string TokenInfo = "info";
        public static readonly string TokenInfoPrefixForPredefindFilter = "tokenInfo.";
        public static readonly string MainAppFromToken = "MainAppFromToken";
        
        public static readonly string AppId = "appid";
        public static readonly string CurAppName = "curappname";
        public static readonly string AppGuid = "appguid";
        public static readonly string Username = "username";
        public static readonly string UserRole = "role";
        public static readonly string FullName = "fullName";
        public static readonly string CurrentUserId = "CurrentUserId";

        public static readonly string RequestId = "requestId";
        public static readonly string AnonymousToken = "AnonymousToken";
        public static readonly string CreateSchema = "CreateSchema";
        public static readonly string CustomValidationActionName = "backandAuthOverride";
        public static readonly string CustomSocialValidationActionName = "socialAuthOverride";
        public static readonly string CustomSocialValidationActionFileName = "socialAuthOverride.js";
        public static readonly string CustomAccessFilterActionName = "accessFilter";
        public static readonly string CustomAccessFilterActionFileName = "accessFilter.js";

        public static readonly string ChangePasswordOverride = "ChangePasswordOverride";
        public static readonly string CustomTokenAttrKey = "CustomTokenAttr";
        public static readonly string AutoGuidPkType = "char(36)";

        public static readonly string LogMessage = "LogMessage";
        public static readonly string EntityType = "EntityType";
        public static readonly string ObjectName = "ObjectName";
        public static readonly string QueryName = "QueryName";
        public static readonly string ActionName = "ActionName";
        public static readonly string SignupInProcess = "SignupInProcess";
        
        public static readonly string MainLogger = "MainLogger";
        public static readonly string EnableDecryptionKey = "EnableDecryption";
        
        public virtual Guid Guid
        {
            get
            {
                return Guid.NewGuid();
            }
        }

        public Durados.Localization.ILocalizer Localizer { get; set; }

        public virtual Dictionary<string, Dictionary<string, string>> ForeignKeys { get; protected set; }

        public string DbConnectionString { get; set; }
        public virtual string SysDbConnectionString { get { return null; } }
        public virtual string SecurityDbConnectionString { get { return null; } }
        //[Durados.Config.Attributes.ChildrenProperty(TableName = "SpecialMenu", Type = typeof(SpecialMenu), DictionaryKeyColumnName = "Name")]
        //public Dictionary<string, SpecialMenu> SpecialMenus { get; private set; }

        [Durados.Config.Attributes.ChildrenProperty(TableName = "View", Type = typeof(View), DictionaryKeyColumnName = "Name")]
        public Dictionary<string, View> Views { get; private set; }
        private DataSet dataSet;

        [Durados.Config.Attributes.ChildrenProperty(TableName = "Tooltip", Type = typeof(Tooltip), DictionaryKeyColumnName = "Name")]
        public Dictionary<string, Tooltip> Tooltips { get; private set; }

        public virtual Database GetAuthDatabase()
        {
            throw new NotImplementedException();
        }

        private bool _signupEmailVerification;
        [Durados.Config.Attributes.ColumnProperty()]
        public bool SignupEmailVerification
        {
            get
            {
                if (HasAuthApp)
                {
                    return GetAuthDatabase().SignupEmailVerification;
                }
                else
                {
                    return _signupEmailVerification;
                }
            }
            set
            {
                _signupEmailVerification = value;
            }
        }


        [Durados.Config.Attributes.ColumnProperty()]
        public bool AutoCommit { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool DefaultLast { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DisplayName { get; set; }
        //[Durados.Config.Attributes.ColumnProperty()]
        public string ConnectionString { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        public string SystemConnectionString { get; set; }

        public bool NoSysIndex = false;

        //public View FirstView { get; private set; }

        public virtual string EncryptedPlaceHolder { get; protected set; }
        public virtual string Salt { get; protected set; }

        public string DefaultMasterKeyPassword { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultCertificateName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultSymmetricKeyName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public SymmetricKeyAlgorithm DefaultSymmetricKeyAlgorithm { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string FirstViewName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string True { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string False { get; set; }

        public string SwVersion { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "You can enter either '*', '*.*' or '*.*.*'. It will be displayed as '*.*.*'.")]
        public string ConfigVersion { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "This role will be the role for non registered users.")]
        public string DefaultGuestRole { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Registered Users - only the users that registered into system can login, Authenticated Users - users that registered to Durados or other customized authentication tools such as LDAP, can login, All Users - everyone can login.  The default is Registered Users (recommended).")]
        public SecureLevel SecureLevel { get; set; }

        private int defaultLevelOfDept;
        [Durados.Config.Attributes.ColumnProperty(Description = "The default level of dept 3.")]
        public int DefaultLevelOfDept
        {
            get
            {
                return GetDefaultLevelOfDept();
            }
            set
            {
                if (value <= 0)
                    throw new DuradosException();
                defaultLevelOfDept = value;
            }
        }


        //[Durados.Config.Attributes.ColumnProperty(Description = "Backand Single Sign On")]
        //public bool BackandSSO { get; set; }

        public virtual Durados.Data.IDataAccess GetDataAccess(string connectionString) { return null; }
        public void SetNextMinorConfigVersion()
        {
            Version version = new Version(ConfigVersion);

            version.NextMinor();

            ConfigVersion = version.ToString();
        }

        public string GetConfigVersion()
        {
            Version version = new Version(ConfigVersion);

            return version.ToString();
        }


        public virtual bool IdenticalSystemConnection
        {
            get
            {
                return false;
            }
        }

        public virtual string DefaultStepMessage { get; set; }
        public virtual string DefaultWorkFlowCompletedMessage { get; set; }

        public virtual int Plan { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "The number of maximum options to select from dropdowns and checklists")]
        public virtual int SelectionLimit { get; set; }


        //[Durados.Config.Attributes.ColumnProperty()]
        //public string FirstViewName
        //{
        //    set
        //    {
        //        if (Views.ContainsKey(value))
        //        {
        //            FirstView = Views[value];
        //        }
        //        else
        //        {
        //            FirstView = Views["durados_v_MessageBoard"];
        //        }
        //    }
        //    get
        //    {
        //        if (FirstView == null)
        //            return "durados_v_MessageBoard";
        //        else
        //            return FirstView.Name;
        //    }
        //}

        public Diagnostics.ILogger Logger { get; set; }
        public Diagnostics.Report DiagnosticsReport { get; set; }
        public bool DiagnosticsReportInProgress
        {
            get
            {
                return DiagnosticsReport != null;
            }
        }

        public Dictionary<string, Menu> menus;
        public Menu ViewsWithNoMenu { get; private set; }

        public Dictionary<string, Menu> Menus
        {
            get
            {
                if (menus == null)
                    LoadMenus();

                return menus;
            }

        }

        public List<Link> GetDashboardLinks()
        {
            List<Link> links = new List<Link>();

            foreach (SpecialMenu menu in Menus.Values.OrderBy(m => m.Ordinal))
            {
                links.AddRange(menu.Links.Where(l => l.Dashboard));
            }

            return links;
        }

        //public Dictionary<string, Workspace> workspaces;
        //public Dictionary<string, Workspace> Workspaces
        //{
        //    get
        //    {
        //        if (workspaces == null)
        //            LoadWorkspaces();

        //        return workspaces;
        //    }

        //}

        public readonly int MinColumnWidth = 52;

        [Durados.Config.Attributes.ChildrenProperty(TableName = "Workspace", Type = typeof(Workspace), DictionaryKeyColumnName = "ID")]
        public Dictionary<int, Workspace> Workspaces { get; private set; }

        //[Durados.Config.Attributes.ChildrenProperty(TableName = "Workspace", Type = typeof(Workspace))]
        //public List<Workspace> Workspaces { get; private set; }


        public bool HasMenus
        {
            get
            {
                return Menus.Count > 0;
            }
        }

        public bool HasViewsWithNoMenu
        {
            get
            {
                if (ViewsWithNoMenu == null)
                    return false;
                if (ViewsWithNoMenu.Views == null)
                    return false;
                if (ViewsWithNoMenu == null || ViewsWithNoMenu.Views == null)
                    return false;
                return ViewsWithNoMenu.Views.Count > 0;
            }
        }

        public virtual void RefreshMenus()
        {
            menus = null;
        }

        //public virtual void RefreshWorkspaces()
        //{
        //    workspaces = null;
        //}
        //bool isPagesInitialized =false;
        private Dictionary<int, Page> pages;
        [Durados.Config.Attributes.ChildrenProperty(TableName = "Page", Type = typeof(Page), DictionaryKeyColumnName = "ID")]
        public Dictionary<int, Page> Pages
        {
            get
            {
                //if (!isPagesInitialized)
                //{
                foreach (Page page in pages.Values)
                    page.Database = this;
                //}
                //isPagesInitialized = true;
                return pages;

            }
            private set { pages = value; }
        }


        protected virtual void LoadMenus()
        {
            menus = new Dictionary<string, Menu>();

            ViewsWithNoMenu = new Menu() { Name = "___", Root = true };

            foreach (View view in Views.Values)
            {
                if (view.Menu != null)
                {
                    Menu menu;

                    if (menus.ContainsKey(view.Menu.Name))
                    {
                        menu = menus[view.Menu.Name];

                    }
                    else
                    {
                        menu = new Menu() { Name = view.Menu.Name, Ordinal = view.Menu.Ordinal, Root = view.Menu.Root, WorkspaceID = view.Menu.WorkspaceID };

                        foreach (UrlLink urllink in view.Menu.UrlLinks.Values)
                            menu.UrlLinks.Add(urllink.Title, urllink);

                        menus.Add(menu.Name, menu);
                    }

                    view.SetMenu(menu);

                    menu.Views.Add(view);

                }
                else
                {
                    ViewsWithNoMenu.Views.Add(view);
                }
            }


            foreach (Menu specialMenu in SpecialMenus.Values)
            {
                menus.Add(specialMenu.Name, specialMenu);
            }

            foreach (Page page in Pages.Values)
            {
                if (page.Menu != null)
                {
                    Menu menu;

                    if (menus.ContainsKey(page.Menu.Name))
                    {
                        menu = menus[page.Menu.Name];

                    }
                    else
                    {
                        menu = new Menu() { Name = page.Menu.Name, Ordinal = page.Menu.Ordinal, Root = page.Menu.Root, WorkspaceID = page.Menu.WorkspaceID };

                        foreach (UrlLink urllink in page.Menu.UrlLinks.Values)
                            menu.UrlLinks.Add(urllink.Title, urllink);

                        menus.Add(menu.Name, menu);
                    }

                    page.SetMenu(menu);

                    menu.Pages.Add(page);

                }
                else
                {
                    ViewsWithNoMenu.Pages.Add(page);
                }
            }
        }

        //protected virtual void LoadWorkspaces()
        //{
        //    workspaces = new Dictionary<string, Workspace>();

        //    foreach (View view in Views.Values)
        //    {
        //        if (view.Workspace != null)
        //        {
        //            Workspace workspace;

        //            if (workspaces.ContainsKey(view.Workspace.Name))
        //            {
        //                workspace = workspaces[view.Workspace.Name];

        //            }
        //            else
        //            {
        //                workspace = new Workspace() { Name = view.Workspace.Name, 
        //                                            AllowCreateRoles = view.Workspace.AllowCreateRoles, 
        //                                            AllowDeleteRoles = view.Workspace.AllowDeleteRoles, 
        //                                            AllowEditRoles = view.Workspace.AllowEditRoles, 
        //                                            AllowSelectRoles = view.Workspace.AllowSelectRoles,
        //                                              DenyCreateRoles = view.Workspace.DenyCreateRoles,
        //                                              DenyDeleteRoles = view.Workspace.DenyDeleteRoles,
        //                                              DenyEditRoles = view.Workspace.DenyEditRoles,
        //                                              DenySelectRoles = view.Workspace.DenySelectRoles
        //                };

        //                workspaces.Add(workspace.Name, workspace);
        //            }

        //            view.SetWorkspace(workspace);

        //            workspace.Views.Add(view);

        //        }
        //    }
        //}

        //[Durados.Config.Attributes.ChildrenProperty(TableName = "SpecialMenu", Type = typeof(SpecialMenu), DictionaryKeyColumnName = "Name")]
        public Dictionary<string, SpecialMenu> SpecialMenus { get; private set; }

        public SpecialMenu GetMenuById(int id)
        {
            foreach (SpecialMenu menu in Menus.Values)
            {
                if (menu.ID == id)
                    return menu;
                else
                {
                    menu.GetMenuById(id);
                }
            }

            return null;
        }

        public void SetSpecialMenusParent()
        {
            foreach (SpecialMenu specialMenu in SpecialMenus.Values)
            {
                specialMenu.Parent = null;
                specialMenu.SetSpecialMenusParent(specialMenu);
            }
        }

        private Dictionary<int, Cron> crons;
        [Durados.Config.Attributes.ChildrenProperty(TableName = "Cron", Type = typeof(Cron), DictionaryKeyColumnName = "ID")]
        public Dictionary<int, Cron> Crons
        {
            get
            {
                foreach (Cron cron in crons.Values)
                    cron.Database = this;
                //}
                //isPagesInitialized = true;
                return crons;


            }
            private set { crons = value; }
        }

        [Durados.Config.Attributes.ParentProperty(TableName = "MyCharts")]
        public MyCharts MyCharts { get; set; }

        private Dictionary<int, MyCharts> dashboards;
        [Durados.Config.Attributes.ChildrenProperty(TableName = "MyCharts", Type = typeof(MyCharts), DictionaryKeyColumnName = "Id")]
        public Dictionary<int, MyCharts> Dashboards
        {
            get
            {
                foreach (MyCharts dashboard in dashboards.Values)
                    dashboard.Database = this;
                //}
                //isPagesInitialized = true;
                return dashboards;


            }
            private set { dashboards = value; }
        }



        private Dictionary<int, Query> queries;
        [Durados.Config.Attributes.ChildrenProperty(TableName = "Query", Type = typeof(Query), DictionaryKeyColumnName = "ID")]
        public Dictionary<int, Query> Queries
        {
            get
            {
                foreach (Query query in queries.Values)
                    query.Database = this;
                //}
                //isPagesInitialized = true;
                return queries;


            }
            private set { queries = value; }
        }

        //private Dictionary<int, NameValue> globals;
        //[Durados.Config.Attributes.ChildrenProperty(TableName = "NameValue", Type = typeof(NameValue), DictionaryKeyColumnName = "ID")]
        //public Dictionary<int, NameValue> Globals
        //{
        //    get
        //    {
        //        foreach (NameValue global in globals.Values)
        //            global.Database = this;
        //        //}
        //        //isPagesInitialized = true;
        //        return globals;


        //    }
        //    private set { globals = value; }
        //}


        //[Durados.Config.Attributes.ParentProperty(TableName = "Menu", RelationName="RootMenu")]
        //public Menu Menu { get; private set; }

        public int? GetDashboardPK(int id)
        {
            foreach (KeyValuePair<int, MyCharts> dashboard in Dashboards)
            {
                if (dashboard.Value.Charts.ContainsKey(id))
                    return dashboard.Key;
            }
            return null;
        }


        public Database(DataSet dataSet)
        {
            //Menu = new Menu() { Name = "Views", Order = 0 };

            this.dataSet = dataSet;

            DefaultPageSize = 20;

            RecentsCount = 10;

            Init();

            Views = new Dictionary<string, View>();

            foreach (DataTable dataTable in dataSet.Tables)
            {
                //if (dataTable.PrimaryKey == null || dataTable.PrimaryKey.Count() == 0)
                //{
                //    throw new NoPrimaryKeyException(dataTable.TableName);
                //}
                this.Views.Add(dataTable.TableName, CreateView(dataTable));
            }


            LoadTooltip();

            //FirstView = Views.Values.Where(v=>v.SystemView == false).FirstOrDefault();

            DisplayName = dataSet.DataSetName;


            True = "Yes";
            False = "No";

            DefaultTimeZoneKey = "US Eastern Standard Time";

            SpecialMenus = new Dictionary<string, SpecialMenu>();

            Crons = new Dictionary<int, Durados.Cron>();
            Dashboards = new Dictionary<int, Durados.MyCharts>();
            Queries = new Dictionary<int, Durados.Query>();
            //Globals = new Dictionary<int, Durados.NameValue>();

            Workspaces = new Dictionary<int, Workspace>();
            DefaultStepMessage = "Please select the next step";
            DefaultWorkFlowCompletedMessage = "Workflow Completed";

            DiagnosticsReport = null;

            UseSpecificDateFormats = true;

            AutoCommit = true;

            DefaultLast = true;

            EncryptedPlaceHolder = "      ";

            MyCharts = new MyCharts(this);

            SecureLevel = Durados.SecureLevel.AllUsers; //RegisteredUsers;
            DefaultGuestRole = "User";

            DefaultLevelOfDept = 3;

            Pages = new Dictionary<int, Page>();

            GeneralErrorMessage = "Oops it seems something went wrong, please contact your system admin";

            SqlProduct = SqlProduct.SqlServer;

            SelectionLimit = 2000;

            MenuType = Durados.MenuType.PullDown;


            FilterParameterOptions = GetFilterParameterOptions();

            SignupEmailVerification = false;

            //BackandSSO = false;
        }

        public object this[string fieldName]
        {
            get
            {
                PropertyInfo info = this.GetType().GetProperty(fieldName);
                if (info == null)
                    return null;
                return info.GetValue(this, null);
            }
            //set
            //{
            //    PropertyInfo info = this.GetType().GetProperty(fieldName);
            //    if (info != null)
            //        info.SetValue(this, value, null);
            //}
        }

        private int GetDefaultLevelOfDept()
        {
            if (defaultLevelOfDept <= 0)
                return 3;
            return defaultLevelOfDept;
        }

        private FilterParameterOption[] GetFilterParameterOptions()
        {
            List<FilterParameterOption> filterParameterOptions = new List<FilterParameterOption>();

            //numeric
            FilterParameterOption filterParameterOption = new FilterParameterOption() { FieldType = FilterFieldType.numericOrDate };
            List<FilterOperand> filterOperands = new List<FilterOperand>();
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.equals, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.notEquals, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.greaterThan, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.greaterThanOrEqualsTo, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.lessThan, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.lessThanOrEqualsTo, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.empty, ValueType = FilterValueType.noValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.notEmpty, ValueType = FilterValueType.noValue });
            filterParameterOption.Operands = filterOperands.ToArray();

            filterParameterOptions.Add(filterParameterOption);

            // text
            filterParameterOption = new FilterParameterOption() { FieldType = FilterFieldType.text };
            filterOperands = new List<FilterOperand>();
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.equals, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.notEquals, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.startsWith, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.endsWith, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.contains, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.notContains, ValueType = FilterValueType.singleValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.empty, ValueType = FilterValueType.noValue });
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.notEmpty, ValueType = FilterValueType.noValue });
            filterParameterOption.Operands = filterOperands.ToArray();

            filterParameterOptions.Add(filterParameterOption);

            // relation
            filterParameterOption = new FilterParameterOption() { FieldType = FilterFieldType.relation };
            filterOperands = new List<FilterOperand>();
            filterOperands.Add(new FilterOperand() { OperandType = FilterOperandType.@in, ValueType = FilterValueType.array });

            filterParameterOptions.Add(filterParameterOption);

            return filterParameterOptions.ToArray();
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Either side menu or pull-down menu")]
        public MenuType MenuType { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Description = "General error meassage, the default is \"Oops it seems something went wrong, please contact your system admin\"")]
        public string GeneralErrorMessage { get; set; }


        int? publicWorkspaceId = null;
        public int GetPublicWorkspaceId()
        {
            if (GetWorkspace(0) != null)
                publicWorkspaceId = 0;

            else if (publicWorkspaceId == null)
            {
                Workspace workspace = GetWorkspace("Public");
                if (workspace == null)
                    return -1000;

                publicWorkspaceId = workspace.ID;
            }

            return publicWorkspaceId.Value;
        }

        private Workspace GetWorkspace(string name)
        {
            return Workspaces.Values.Where(w => w.Name == name).FirstOrDefault();
        }

        public Workspace GetWorkspace(int id)
        {
            return Workspaces.Values.Where(w => w.ID == id).FirstOrDefault();
        }

        public Workspace GetWorkspaceByMenu(int menuId)
        {
            foreach (Workspace workspace in Workspaces.Values)
            {
                SpecialMenu menu = workspace.GetSpecialMenu(menuId);
                if (menu != null)
                    return workspace;
            }

            return null;
        }

        public Workspace GetAdminWorkspace()
        {
            return GetWorkspace(GetAdminWorkspaceId());
        }

        public Workspace GetPublicWorkspace()
        {
            return GetWorkspace(GetPublicWorkspaceId());
        }


        int? adminWorkspaceId = null;
        public int GetAdminWorkspaceId()
        {
            if (GetWorkspace(1) != null)
                adminWorkspaceId = 1;

            else if (adminWorkspaceId == null)
            {
                Workspace workspace = GetWorkspace("Admin");
                if (workspace == null)
                    return -1000;

                adminWorkspaceId = workspace.ID;
            }

            return adminWorkspaceId.Value;
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public int DefaultWorkspaceId { get; set; }

        public Workspace GetDefaultWorkspace()
        {
            if (Workspaces.ContainsKey(DefaultWorkspaceId))
                return Workspaces[DefaultWorkspaceId];

            Workspace workspace = GetPublicWorkspace();
            if (workspace == null)
            {
                return Workspaces.Values.Where(w => w.Name != "Admin").FirstOrDefault();
            }

            return workspace;
        }

        public IEnumerable<Menu> GetWorkspaceMenus(Workspace workspace)
        {
            if (workspace.Menus == null)
            {
                workspace.Menus = new List<Menu>();
                foreach (Menu menu in Menus.Values)
                {
                    if (menu.WorkspaceID == workspace.ID)
                    {
                        workspace.Menus.Add(menu);
                    }
                }
            }

            return workspace.Menus;
        }

        public IEnumerable<View> GetWorkspaceViews(Workspace workspace)
        {
            if (workspace.Views == null)
            {
                workspace.Views = new List<View>();
                foreach (View view in Views.Values)
                {
                    if (view.WorkspaceID == workspace.ID)
                    {
                        workspace.Views.Add(view);
                    }
                }
            }

            return workspace.Views;
        }

        public IEnumerable<View> GetWorkspaceViewsWithoutMenu(Workspace workspace)
        {
            if (workspace.ViewsWithoutMenu == null)
            {
                workspace.ViewsWithoutMenu = new List<View>();
                foreach (View view in GetWorkspaceViews(workspace))
                {
                    if (view.Menu == null)
                    {
                        workspace.ViewsWithoutMenu.Add(view);
                    }
                }
            }

            return workspace.ViewsWithoutMenu;
        }

        public IEnumerable<Page> GetWorkspacePagesWithoutMenu(Workspace workspace)
        {
            return Pages.Values.Where(p => p.WorkspaceID == workspace.ID && p.Menu == null);
        }

        protected virtual void LoadTooltip()
        {
            Tooltips = new Dictionary<string, Tooltip>();

            Tooltips.Add(ToolbarActionNames.NEW.ToString(), new Tooltip(ToolbarActionNames.NEW.ToString()) { Description = "Add new row", Title = "New" });
            Tooltips.Add(ToolbarActionNames.DUPLICATE.ToString(), new Tooltip(ToolbarActionNames.DUPLICATE.ToString()) { Description = "Duplicate selected row", Title = "Duplicate" });
            Tooltips.Add(ToolbarActionNames.ADD_ITEMS.ToString(), new Tooltip(ToolbarActionNames.ADD_ITEMS.ToString()) { Description = "Add Items", Title = "Add Items" });
            Tooltips.Add(ToolbarActionNames.UP.ToString(), new Tooltip(ToolbarActionNames.UP.ToString()) { Description = "Move selected row up", Title = "Up" });
            Tooltips.Add(ToolbarActionNames.DOWN.ToString(), new Tooltip(ToolbarActionNames.DOWN.ToString()) { Description = "Move selected row down", Title = "Down" });
            Tooltips.Add(ToolbarActionNames.SELECT_ALL.ToString(), new Tooltip(ToolbarActionNames.SELECT_ALL.ToString()) { Description = "Select all rows", Title = "Select All" });
            Tooltips.Add(ToolbarActionNames.CLEAR.ToString(), new Tooltip(ToolbarActionNames.CLEAR.ToString()) { Description = "Clears all selection", Title = "Clear Selection" });
            Tooltips.Add(ToolbarActionNames.COPY.ToString(), new Tooltip(ToolbarActionNames.COPY.ToString()) { Description = "Copy selected cells", Title = "Copy" });
            Tooltips.Add(ToolbarActionNames.PASTE.ToString(), new Tooltip(ToolbarActionNames.PASTE.ToString()) { Description = "Paste copy cells into selected cells", Title = "Paste" });
            Tooltips.Add(ToolbarActionNames.EDIT.ToString(), new Tooltip(ToolbarActionNames.EDIT.ToString()) { Description = "Edit selected row", Title = "Edit" });
            Tooltips.Add(ToolbarActionNames.INSERT.ToString(), new Tooltip(ToolbarActionNames.INSERT.ToString()) { Description = "Add new row after selected row", Title = "Insert" });
            Tooltips.Add(ToolbarActionNames.DELETE.ToString(), new Tooltip(ToolbarActionNames.DELETE.ToString()) { Description = "Delete selected row", Title = "Delete" });
            Tooltips.Add(ToolbarActionNames.PAINT.ToString(), new Tooltip(ToolbarActionNames.PAINT.ToString()) { Description = "Highlight selected row with background color", Title = "Highlight" });
            Tooltips.Add(ToolbarActionNames.HISTORY.ToString(), new Tooltip(ToolbarActionNames.HISTORY.ToString()) { Description = "View the changes of the selected row", Title = "History" });
            Tooltips.Add(ToolbarActionNames.EXPORT.ToString(), new Tooltip(ToolbarActionNames.EXPORT.ToString()) { Description = "Export all pages into a CSV file", Title = "Export" });
            Tooltips.Add(ToolbarActionNames.IMPORT.ToString(), new Tooltip(ToolbarActionNames.IMPORT.ToString()) { Description = "Import data from Excel", Title = "Import" });
            Tooltips.Add(ToolbarActionNames.PRINT.ToString(), new Tooltip(ToolbarActionNames.PRINT.ToString()) { Description = "Print current page", Title = "Print" });
            Tooltips.Add(ToolbarActionNames.SEND.ToString(), new Tooltip(ToolbarActionNames.SEND.ToString()) { Description = "Send a link to the selected row", Title = "Send" });
            Tooltips.Add(ToolbarActionNames.APPLY_FILTER.ToString(), new Tooltip(ToolbarActionNames.APPLY_FILTER.ToString()) { Description = "Apply filter", Title = "Apply" });
            Tooltips.Add(ToolbarActionNames.CLEAR_FILTER.ToString(), new Tooltip(ToolbarActionNames.CLEAR_FILTER.ToString()) { Description = "Clear the filter", Title = "Clear" });
            Tooltips.Add(ToolbarActionNames.SEARCH.ToString(), new Tooltip(ToolbarActionNames.SEARCH.ToString()) { Description = "Search on all text fields", Title = "Search" });
            Tooltips.Add(ToolbarActionNames.GRID_EDIT_MODE.ToString(), new Tooltip(ToolbarActionNames.GRID_EDIT_MODE.ToString()) { Description = "Enable/Disable editing on the grid", Title = "Edit Mode" });
            Tooltips.Add(ToolbarActionNames.REFRESH.ToString(), new Tooltip(ToolbarActionNames.REFRESH.ToString()) { Description = "Refresh the view content", Title = "Refresh" });
            Tooltips.Add(ToolbarActionNames.MESSAGE_BOARD.ToString(), new Tooltip(ToolbarActionNames.MESSAGE_BOARD.ToString()) { Description = "View the Message Board messages for the current view", Title = "Messages" });
            Tooltips.Add(ToolbarActionNames.BACK.ToString(), new Tooltip(ToolbarActionNames.BACK.ToString()) { Description = "Browser Go Back", Title = "Back" });
            Tooltips.Add(ToolbarActionNames.COPY_CONFIG.ToString(), new Tooltip(ToolbarActionNames.COPY_CONFIG.ToString()) { Description = "Copy view configuration", Title = "Copy" });
            Tooltips.Add(ToolbarActionNames.CLONE_CONFIG.ToString(), new Tooltip(ToolbarActionNames.CLONE_CONFIG.ToString()) { Description = "Clone view configuration", Title = "Clone" });
            Tooltips.Add(ToolbarActionNames.SEND_CONFIG.ToString(), new Tooltip(ToolbarActionNames.SEND_CONFIG.ToString()) { Description = "Send configuration", Title = "Send" });
            Tooltips.Add(ToolbarActionNames.DOWNLOAD_CONFIG.ToString(), new Tooltip(ToolbarActionNames.DOWNLOAD_CONFIG.ToString()) { Description = "Download configuration", Title = "Download" });
            Tooltips.Add(ToolbarActionNames.UPLOAD_CONFIG.ToString(), new Tooltip(ToolbarActionNames.UPLOAD_CONFIG.ToString()) { Description = "Upload configuration", Title = "Upload" });
            Tooltips.Add(ToolbarActionNames.SYNC_ALL.ToString(), new Tooltip(ToolbarActionNames.SYNC_ALL.ToString()) { Description = "Sync All", Title = "Sync All" });
            Tooltips.Add(ToolbarActionNames.DIAGNOSE.ToString(), new Tooltip(ToolbarActionNames.DIAGNOSE.ToString()) { Description = "Diagnose", Title = "Diagnose" });
            Tooltips.Add(ToolbarActionNames.DICTIONARY.ToString(), new Tooltip(ToolbarActionNames.DICTIONARY.ToString()) { Description = "Dictionary", Title = "Dictionary" });

        }

        protected virtual void Init()
        {
            DefaultAllowCreateRoles = "Developer,Admin,User";
            DefaultAllowEditRoles = "Developer,Admin,User";
            DefaultAllowSelectRoles = "Developer,Admin,User";
            DefaultAllowDeleteRoles = "Developer,Admin,User";
            DefaultCulture = "en-US";
        }

        public View GetView(string displayName)
        {
            foreach (View view in Views.Values)
            {
                if (view.DisplayName.Equals(displayName))
                    return view;
            }

            return null;
        }

        public virtual bool UseSmtpDefaultCredentials
        {
            get
            {
                return false;
            }
        }

        public View GetViewByJsonName(string jsonName)
        {
            var views = Views.Values.Where(v => v.JsonName.Equals(jsonName, StringComparison.InvariantCultureIgnoreCase));
            if (views == null)
                return null;

            return views.FirstOrDefault();
        }

        public View GetViewByEditableTableName(string tableName)
        {
            foreach (View view in Views.Values)
            {
                if (view.Name.Equals(tableName) || (!string.IsNullOrEmpty(view.EditableTableName) && view.EditableTableName.Equals(tableName)))
                    return view;
            }

            return null;
        }


        public virtual View CreateView(DataTable dataTable)
        {
            return new View(dataTable, this);
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public int DefaultPageSize { get; set; }



        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultAllowCreateRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultAllowEditRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultAllowSelectRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultAllowDeleteRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultDenyCreateRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultDenyEditRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultDenySelectRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultDenyDeleteRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultViewOwnerRoles { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int RecentsCount { get; set; }

        protected string defaultCulture = "en-US";

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultCulture
        {
            get
            {
                return defaultCulture;
            }
            set
            {
                defaultCulture = value;
            }
        }

        public const string EmptyString = "[null]";

        public virtual string EmptyDisplay
        {
            get
            {
                return "(Empty)";
            }
        }

        protected Dictionary<string, string> lowerCaseViewNames = null;

        public string GetCorrectCaseViewName(string viewName)
        {
            if (lowerCaseViewNames == null)
            {
                lowerCaseViewNames = new Dictionary<string, string>();
                foreach (View view in Views.Values)
                {
                    lowerCaseViewNames.Add(view.Name.ToLower(), view.Name);
                }
            }

            string lowerCaseViewName = viewName.ToLower();
            if (lowerCaseViewNames.ContainsKey(lowerCaseViewName))
                return lowerCaseViewNames[lowerCaseViewName];
            else
                return viewName;
        }

        protected bool isMultiLanguages = false;

        [Durados.Config.Attributes.ColumnProperty()]
        public bool IsMultiLanguages
        {
            get
            {
                return GetIsMultiLanguages();
            }
            set
            {
                isMultiLanguages = value;
            }
        }

        protected virtual bool GetIsMultiLanguages()
        {
            return isMultiLanguages;
        }

        protected bool doLocalizeAdmin = false;

        [Durados.Config.Attributes.ColumnProperty()]
        public bool DoLocalizeAdmin
        {
            get
            {
                return doLocalizeAdmin;
            }
            set
            {
                doLocalizeAdmin = value;
            }
        }


        [Durados.Config.Attributes.ColumnProperty()]
        public bool UseSpecificDateFormats { get; set; }

        protected string dateFormat = DATE_FORMAT;

        [Durados.Config.Attributes.ColumnProperty()]
        public string DateFormat
        {
            get
            {
                return dateFormat;
            }
            set
            {
                foreach (DatesFormatStrings formats in DateFormatsMapper.getLegalDateFormats())
                {
                    if (formats.Csharp == value)
                    {
                        dateFormat = value;
                        return;
                    }

                }
                //TODO ...
                dateFormat = "MMM dd, yyyy";
                //throw new IlegalDateFormatException();
            }
        }

        public List<View> GetVisibleViews(string role)
        {
            return Views.Values.Where(v => v.DenySelectRoles.Contains(role)).ToList();
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultTimeZoneKey { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Default false. When true the options in the Toolbar menu should always be available for Admin but changed for user ")]
        public bool IgnoreInAdminMode { get; set; }

        public virtual bool IsConfig
        {
            get
            {
                return false;
            }
        }

        //[Durados.Config.Attributes.ColumnProperty()]
        //public int DashboardColumns { get; set; }

        public virtual View GetUserView()
        {
            return null;
        }

        public SqlProduct SqlProduct { get; set; }
        public SqlProduct SystemSqlProduct { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Report Services Username for credentials")]
        public string SsrsUsername { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Report Services Password for credentials")]
        public string SsrsPassword { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Report Services Domain for credentials")]
        public string SsrsDomain { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Report Services web address")]
        public string SsrsReportServerUrl { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "The root path for the reports ie. /Root/Path/")]
        public string SsrsPath { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Description = "Default false. if set to true, it is a containing search, it performs a like '%something%' where statement, otherwise it is a start with search, it performs a like 'something%' where statement")]
        public bool InsideTextSearch { get; set; }

        private ExportFileType defaultExportImportFormat = ExportFileType.Excel;
        [Durados.Config.Attributes.ColumnProperty(Description = "Default Excel. Export File Format")]
        public ExportFileType DefaultExportImportFormat
        {
            get
            {
                return defaultExportImportFormat;
            }
            set
            {
                this.defaultExportImportFormat = value;
            }
        }

        private bool translateAllViews = false;
        [Durados.Config.Attributes.ColumnProperty(Description = "If true than Translate user table display names and lables. The default is false.")]
        public virtual bool TranslateAllViews
        {
            get
            {
                return translateAllViews;
            }
            set
            {
                translateAllViews = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "The content in the top center of all pages.")]
        public string HeaderContent { get; set; }


        public virtual string GetCurrentUsername()
        {
            return string.Empty;
        }

        public virtual string GetCurrentAppName()
        {
            return string.Empty;
        }

        public virtual Database GetMainDatabase()
        {
            return null;
        }

        public virtual object GetCurrentUserId()
        {
            return null;
        }

        public virtual bool IsAllow(Durados.Services.ISecurable securable)
        {
            return true;
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Groboot Push Notification Access Key")]
        public string GrobootNotificationAccessKey { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Primary Key type")]
        public string PkType { get; set; }

        public FilterParameterOption[] FilterParameterOptions { get; private set; }
        public virtual string GetUserID()
        {
            return null;
        }

        public virtual bool IsMain()
        {
            return false;
        }

        public virtual bool IsApi()
        {
            throw new NotImplementedException();
        }

        public virtual string GetConnectionSource()
        {
            throw new NotImplementedException();
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Configuration JSON")]
        public string Config { get; set; }

        public virtual Dictionary<string, object> GetConfigDictionary()
        {
            return null;
        }

        //public Dictionary<string, object> GetGlobalsDictionary()
        //{
        //    Dictionary<string, object> dic = new Dictionary<string, object>();

        //    foreach (int key in Globals.Keys)
        //    {
        //        string name = Globals[key].Name;
        //        string value = Globals[key].Value;

        //        dic.Add(name, value);
        //    }

        //    return dic;
        //}

        public virtual Guid GetAnonymousToken()
        {
            return Guid.NewGuid();
        }

        public virtual string GetAuthorization()
        {
            return null;
        }

        public virtual object GetLimit(Limits limit)
        {
            throw new NotImplementedException();
        }

        public virtual bool EnableDecryption
        {
            get
            {
                return true;
            }
        }

        public virtual string GetCacheValue(string key)
        {
            throw new NotImplementedException();
        }

        public virtual void SetCacheValue(string key, string value, int milliseconds)
        {
            throw new NotImplementedException();
        }

        public virtual NameValueCollection GetSecuritySettings(string appId)
        {
            throw new NotImplementedException();
        }


        [Durados.Config.Attributes.ColumnProperty()]
        public string AuthAppId { get; set; }

        public virtual bool HasAuthApp
        {
            get
            {
                return !string.IsNullOrEmpty(AuthAppId);
            }
        }

        public bool IsAuthApp {
            get { return GetIsAuthApp(); }
            set { }
        }

        public virtual bool IsDomainController
        {
            get
            {
                return false;
            }
        }

        protected virtual bool GetIsAuthApp()
        {
            throw new NotImplementedException();
        }

        public virtual string GetDomainControllerProvider()
        {
            return null;
        }
        

    }

    public class IlegalDateFormatException : DuradosException
    {
        public IlegalDateFormatException()
            : base("Ilegal date format exception.")
        {
        }
    }

    public class NoPrimaryKeyException : DuradosException
    {
        public NoPrimaryKeyException(string tableName)
            : base("The table " + tableName + " does not have a primary key.")
        {
        }
    }


    public enum SqlProduct
    {
        SqlServer = 1,
        SqlAzure = 2,
        MySql = 3,
        Postgre = 4,
        Oracle = 5
    }

    public enum ToolbarActionNames
    {
        NEW,
        DUPLICATE,
        ADD_ITEMS,
        UP,
        DOWN,
        SELECT_ALL,
        CLEAR,
        COPY,
        PASTE,
        EDIT,
        INSERT,
        DELETE,
        PAINT,
        HISTORY,
        EXPORT,
        IMPORT,
        PRINT,
        SEND,
        APPLY_FILTER,
        CLEAR_FILTER,
        SEARCH,
        GRID_EDIT_MODE,
        REFRESH,
        BOOKMARK,
        MESSAGE_BOARD,
        BACK,
        COPY_CONFIG,
        CLONE_CONFIG,
        SEND_CONFIG,
        DOWNLOAD_CONFIG,
        UPLOAD_CONFIG,
        DIAGNOSE,
        DICTIONARY,
        SYNC_ALL
    }


    public enum ImportModes : byte
    {
        Insert = 1,
        Update,
        UpdateOrInsert,
        InsertIgnoreUnique
    }

    public enum SecureLevel : byte
    {
        RegisteredUsers,
        AuthenticatedUsers,
        AllUsers
    }

    public enum SymmetricKeyAlgorithm : byte
    {
        DES, TRIPLE_DES, RC2, RC4, RC4_128, DESX, AES_128, AES_192, AES_256
    }

    public enum Style
    {
        Turquoise,
        DesertGlow,
        ClassicGray
    }

    public enum ExportFileType
     {
        Excel,
        Csv
    }

    public enum MenuType
    {
        PullDown,
        Side,
        
    }

    public enum FilterOperandType
    {
        equals,
        notEquals,
        lessThan,
        lessThanOrEqualsTo,
        greaterThan,
        greaterThanOrEqualsTo,
        empty,
        notEmpty,
        
        startsWith,
        endsWith,
        contains,
        notContains,

        @in,
    }

    public enum FilterValueType
    {
        noValue,
        singleValue,
        array,
    }

    public enum FilterFieldType
    {
        text,
        numericOrDate,
        relation,

    }

    public class FilterParameterOption
    {
        public FilterFieldType FieldType { get; set; }
        public FilterOperand[] Operands { get; set; }
        
    }

    public class FilterOperand
    {
        public FilterOperandType OperandType { get; set; }
        public FilterValueType ValueType { get; set; }

    }

    public enum EntityType
    {
        Object,
        Action,
        Query,
    }

    public enum Limits
    {
        Cron,
        ActionParametersKbSize,
        ActionTimeMSec,
        UploadTimeMSec
    }
}
