using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc.Azure;
using Newtonsoft.Json;
using System.Diagnostics;
using Durados.Data;

namespace Durados.Web.Mvc
{
    [Serializable()]
    public class Map : Durados.DataAccess.Storage.IStorage, ILargeObjectCachingStamp
    {
        public virtual bool IsMainMap
        {
            get
            {
                return false;
            }
        }

        public Dictionary<string, Dictionary<string, string>> ForeignKeys = new Dictionary<string, Dictionary<string, string>>();

        public DateTime TimeStamp { get; set; }
        public DateTime LastCheck { get; private set; }
        public void UpdateLastCheck()
        {
            LastCheck = DateTime.Now;
        }

        private PersistentSession session;

        private string version;
        private static string systemVersion;

        public Durados.Concurrency Concurrency { get; private set; }

        public Logging.Logger Logger { get; private set; }

        public string AppName { get; set; }

        public bool IsConfigChanged { get; set; }

        public readonly string SourceSuffix = "Source";

        public HashSet<string> Aliases { get; private set; }

        public int LocalPort { get; set; }

        public int PlugInId { get; set; }
        public string DefaultPagePath
        {
            get
            {
                if (!Database.UploadFolder.EndsWith("/"))
                    Database.UploadFolder += "/";
                return Database.UploadFolder + "/Specifics/";
            }
        }

        public virtual string Url { get; set; }

        private bool? hostedByUs = null;
        public bool HostedByUs
        {
            get
            {
                if (!hostedByUs.HasValue)
                    hostedByUs = (new Durados.Web.Mvc.Infrastructure.AppFactory().GetAppById(Id)).AppType == 2;

                return hostedByUs.Value;
            }
        }
        public virtual string DownloadActionName { get { return Maps.DownloadActionName; } }

        public DataAccess.AutoGeneration.Dynamic.Mapper DynamicMapper { get; private set; }
        public DataAccess.AutoGeneration.Dynamic.Mapper SystemDynamicMapper { get; private set; }

        //public string CreatorUserName { get; set; }

        public Map()
        {
            Logger = new Durados.Web.Mvc.Logging.Logger();

            Aliases = new HashSet<string>();

            configuration = new Configuration();

            //if (Maps.Cloud)
            //{
            //    version = GetVersion();
            //}
            //else
            //{
            //    version = GetVersion();
            //}
            version = GetVersion();
            //  systemVersion = General.Version(Durados.Web.Mvc.App.Helpers);
            Concurrency = new Concurrency();


        }

        public bool UsingSsh { get; set; }

        public Durados.Security.Ssh.ISession SshSession { get; set; }

        public void OpenSshSession()
        {
            if (UsingSsh && SshSession != null)
                SshSession.Open();
        }

        public void SshStop()
        {
            if (UsingSsh && SshSession != null)
                SshSession.AllStop();
        }

        List<Bookmark> publicBoolmarks = null;

        public void ResetPublicBoolmarks()
        {
            publicBoolmarks = null;
        }

        public List<Bookmark> GetPublicBoolmarks()
        {
            if (publicBoolmarks == null)
            {
                LoadPublicBookmarks();
            }

            return publicBoolmarks;
        }

        private void LoadPublicBookmarks()
        {
            publicBoolmarks = BookmarksHelper.GetPublicBookmarks();
        }


        private string GetVersion()
        {
            if (string.IsNullOrEmpty(Maps.Version))
            {
                string versionFileName = System.Web.HttpContext.Current.Server.MapPath("~/deployment/version.txt");
                if (!File.Exists(versionFileName))
                    throw new DuradosException("Version file " + versionFileName + " does not exists");
                TextReader tr = new StreamReader(versionFileName);

                // read a line of text
                version = tr.ReadLine();



                // close the stream
                tr.Close();

                Maps.Version = version;
            }
            return Maps.Version;
        }

        public virtual string GetLogOnPath()
        {
            string logon = System.Configuration.ConfigurationManager.AppSettings["LogOnPath"] ?? "~/Views/Account/LogOn.aspx";

            if (Database.SiteInfo != null && !string.IsNullOrEmpty(Database.SiteInfo.LogOnPath))
                logon = Database.SiteInfo.LogOnPath;


            return logon;
        }

        public virtual bool GetLogMvc()
        {
            return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogOnMvc"] ?? "true");

        }

        public string CreateView(System.Data.DataTable table)
        {
            Database db = GetDefaultDatabase();
            Field field = db.Views["Table"].Fields["Column"];
            ParentField parentField = (ParentField)db.Views["Table"].Fields["Relation1_Parent"];

            Dictionary<string, string> viewDefaults = GetViewDefaults();

            return DynamicMapper.CreateView(table, Database, configDatabase.Views["View"], configDatabase.Views["Field"], dataset, field, parentField, viewDefaults, false);


        }

        private Dictionary<string, string> GetViewDefaults()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("Controller", Database.DefaultController);
            values.Add("IndexAction", Database.DefaultIndexAction);
            values.Add("CheckListAction", Database.DefaultCheckListAction);
            values.Add("CreateAction", Database.DefaultCreateAction);
            values.Add("RefreshAction", Database.DefaultRefreshAction);
            values.Add("CreateOnlyAction", Database.DefaultCreateOnlyAction);
            values.Add("EditAction", Database.DefaultEditAction);
            values.Add("DuplicateAction", Database.DefaultDuplicateAction);

            values.Add("EditRichAction", Database.DefaultEditRichAction);
            values.Add("GetRichAction", Database.DefaultGetRichAction);
            values.Add("EditOnlyAction", Database.DefaultEditOnlyAction);
            values.Add("GetJsonViewAction", Database.DefaultGetJsonViewAction);
            values.Add("GetSelectListAction", Database.DefaultGetSelectListAction);

            values.Add("DeleteAction", Database.DefaultDeleteAction);
            values.Add("DeleteSelectionAction", Database.DefaultDeleteSelectionAction);
            values.Add("EditSelectionAction", Database.DefaultEditSelectionAction);
            values.Add("FilterAction", Database.DefaultFilterAction);
            values.Add("UploadAction", Database.DefaultUploadAction);
            values.Add("ExportToCsvAction", Database.DefaultExportToCsvAction);
            values.Add("PrintAction", Database.DefaultPrintAction);
            values.Add("SetLanguageAction", Database.DefaultSetLanguageAction);
            values.Add("AutoCompleteAction", Database.DefaultAutoCompleteAction);
            values.Add("AutoCompleteController", Database.DefaultAutoCompleteController);
            values.Add("InlineAddingDialogAction", Database.DefaultInlineAddingDialogAction);
            values.Add("InlineAddingCreateAction", Database.DefaultInlineAddingCreateAction);
            values.Add("InlineEditingDialogAction", Database.DefaultInlineEditingDialogAction);
            values.Add("InlineEditingEditAction", Database.DefaultInlineEditingEditAction);
            values.Add("InlineDuplicateDialogAction", Database.DefaultInlineDuplicateDialogAction);
            values.Add("InlineDuplicateAction", Database.DefaultInlineDuplicateAction);
            values.Add("InlineSearchDialogAction", Database.DefaultInlineSearchDialogAction);
            values.Add("AllFilterValuesAction", Database.DefaultAllFilterValuesAction);

            return values;
        }

        public string DynamicMapperExceptions
        {
            get
            {
                return DynamicMapper.Exceptions;
            }
        }

        public void ClearDynamicMapperExceptions()
        {
            DynamicMapper.Exceptions = string.Empty;
        }

        public void EncryptColumn(ColumnField field)
        {
            DynamicMapper.Encrypt(field);
        }

        public void CreateView(string name, string configViewPk, string editableTableName, View configView)
        {
            Database db = GetDefaultDatabase();
            Field field = db.Views["Table"].Fields["Column"];
            ParentField parentField = (ParentField)db.Views["Table"].Fields["Relation1_Parent"];

            DynamicMapper.CreateView(Database, name, configViewPk, configDatabase.Views["Field"], dataset, field, parentField, editableTableName, configView, null);
        }

        public void CreateField(string viewName, string fieldName, string type, string parentViewName)
        {
            DynamicMapper.CreateField(viewName, fieldName, type, parentViewName, dataset);
        }

        public bool IsDebug()
        {
            return System.Configuration.ConfigurationManager.AppSettings["Debug"].ToLower() == "true";
        }

        public string Version
        {
            get
            {
                return version;
            }
        }

        public static string SystemVersion
        {
            get
            {
                return systemVersion;
            }
            set { systemVersion = value; }
        }

        public Workflow.Engine WorkflowEngine { get; set; }

        public PersistentSession Session
        {
            get
            {
                return session;
            }
        }

        private Dictionary<string, string> userEmails = new Dictionary<string, string>();
        public string GetUserEmail(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (!userEmails.ContainsKey(key))
            {
                View userView = (View)Database.GetUserView();
                if (userView == null)
                    return null;

                DataRow userRow = userView.GetDataRow(key);

                if (userRow == null)
                    return null;

                if (userRow.IsNull("Email"))
                    return null;

                userEmails.Add(key, userRow["Email"].ToString());
            }
            return userEmails[key];
        }

        private Database db;
        private Configuration configuration;
        public string connectionString;
        public string systemConnectionString;
        public string securityConnectionString { get; set; }
        //public string ConnectionString
        //{
        //    get
        //    {
        //        return connectionString;
        //    }
        //    set
        //    {
        //        connectionString = value;
        //        db.ConnectionString = connectionString;
        //    }
        //}

        public Database Database
        {
            get
            {
                return db;
            }
        }



        public string ConfigFileName { get; set; }

        private DataSet dataset;

        private Durados.Web.Mvc.Config.IProject project;

        public string selectedProject;

        private Durados.DataAccess.AutoGeneration.Generator systemGenerator = null;

        protected virtual Durados.Web.Mvc.Config.IProject GetProject()
        {
            if (Maps.MultiTenancy)
            {
                if (string.IsNullOrEmpty(selectedProject))
                {
                    return new Durados.Web.Mvc.Config.Project();
                }
                else
                {
                    if (System.IO.File.Exists(selectedProject))
                    {
                        //Durados.Web.Mvc.Config.IProject dotNetProject = null;
                        Assembly assembly = Assembly.LoadFrom(selectedProject);

                        Type[] types = assembly.GetTypes();

                        foreach (Type type in types)
                        {
                            if (type.GetInterface("IProject", true) != null)
                            {
                                return (Durados.Web.Mvc.Config.IProject)assembly.CreateInstance(type.FullName);

                            }
                        }


                    }
                    //return (Durados.Web.Mvc.Config.IProject)Activator.CreateInstance(Type.GetType(selectedProject));

                }
            }

            bool useAppPath = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseAppPath"]);
            string selectedProjectKey = string.Empty;
            if (useAppPath)
            {
                selectedProjectKey = Durados.Web.Infrastructure.General.GetRootName() + "Project";
            }
            else
            {
                selectedProjectKey = System.Configuration.ConfigurationManager.AppSettings["SelectedProject"];
            }

            selectedProject = System.Configuration.ConfigurationManager.AppSettings[selectedProjectKey];
            Durados.Web.Mvc.Config.IProject project = null;
            if (System.IO.File.Exists(selectedProject))
            {
                Assembly assembly = Assembly.LoadFrom(selectedProject);

                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (type.GetInterface("IProject", true) != null)
                    {
                        project = (Durados.Web.Mvc.Config.IProject)assembly.CreateInstance(type.FullName);

                    }
                }


            }
            else
            {

                project = (Durados.Web.Mvc.Config.IProject)Activator.CreateInstance(Type.GetType(selectedProject));
            }
            return project;
        }

        private bool initiated = false;
        public bool Initiated
        {
            get { return initiated; }
        }

        public void Initiate()
        {
            Initiate(true);
        }

        private string  GetCaller(object message)
        {
            // frame 1, true for source info
            StackFrame frame = new StackFrame(1, true);
            var method = frame.GetMethod();
            var fileName = frame.GetFileName();
            var lineNumber = frame.GetFileLineNumber();

            // we'll just use a simple Console write for now    
            return string.Format("{0}({1}):{2} - {3}", fileName, lineNumber, method.Name, message);
        }

        private string GetRequest()
        {
            try
            {
                return string.Format("url: {0}; method: {1}", System.Web.HttpContext.Current.Request.Url.ToString(), System.Web.HttpContext.Current.Request.HttpMethod);
            }
            catch
            {
                return "unknown";
            }
        }

        public bool Initiate(bool save)
        {
            try
            {
                Durados.Web.Mvc.Config.IProject project = GetProject();
                project.Map = this;
                if (JsonConfigCache != null)
                    JsonConfigCache.Clear();

                bool firstTime = Initiate(project, save);
                initiated = true;
                return firstTime;
            }
            catch (Exception exception)
            {
                string appName = Maps.Instance.GetAppName();
                Maps.Instance.DuradosMap.Logger.Log("Map", "Initiate", "Initiate", exception, 1, GetCaller("caller") + "; " + GetRequest());
                throw new AppNotReadyException(appName);
                //throw new DuradosException("Failed to initiate app", exception);
            }
        }

        protected virtual Durados.DataAccess.AutoGeneration.Dynamic.Mapper GetNewMapper()
        {
            if (SqlProduct == Durados.SqlProduct.Postgre)
                return new Durados.DataAccess.AutoGeneration.Dynamic.PostgreMapper();
            if (SqlProduct == Durados.SqlProduct.MySql)
                return new Durados.DataAccess.AutoGeneration.Dynamic.MySqlMapper();
            if (SqlProduct == Durados.SqlProduct.Oracle)
                return new Durados.DataAccess.AutoGeneration.Dynamic.OracleMapper();

            else
                return new Durados.DataAccess.AutoGeneration.Dynamic.Mapper();

        }
        protected virtual Durados.DataAccess.AutoGeneration.Dynamic.Mapper GetNewSystemMapper(string connectionString)
        {
            if (MySqlAccess.IsMySqlConnectionString(connectionString))
                return new Durados.DataAccess.AutoGeneration.Dynamic.MySqlMapper();
            else
                return new Durados.DataAccess.AutoGeneration.Dynamic.Mapper();
        }


        private bool Initiate(Durados.Web.Mvc.Config.IProject project, bool save)
        {

            this.project = project;
            DataSet ds = project.GetDataSet();

            if (this.connectionString == null)
                this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[project.ConnectionStringKey].ConnectionString;
            if (this.systemConnectionString == null)
                this.systemConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[project.SystemConnectionStringKey].ConnectionString;
            if (this.ConfigFileName == null)
                ConfigFileName = System.Web.Configuration.WebConfigurationManager.AppSettings[project.ConfigFileNameKey];

            //DynamicMapper = new Durados.DataAccess.AutoGeneration.Dynamic.Mapper();
            DynamicMapper = GetNewMapper();
            SystemDynamicMapper = GetNewSystemMapper(this.systemConnectionString);
            systemGenerator = SystemDynamicMapper.GetNewGenerator();
            DynamicMapper.Storage = this;

            DynamicMapper.Logger = Logger;
            DynamicMapper.ConnectionString = connectionString;
            DynamicMapper.FileName = this.ConfigFileName + ".xml";

            string historySchemaGeneratorFileName = SystemDynamicMapper.GetGenerateScriptFileName(Maps.GetDeploymentPath("Sql/History.sql"));
            Durados.DataAccess.AutoGeneration.History history =
                SystemDynamicMapper.GetHistoryGenerator(systemConnectionString, historySchemaGeneratorFileName);

            //string messageBoardSchemaGeneratorFileName = Maps.GetDeploymentPath("Sql/MessageBoard.sql");
            //Durados.DataAccess.AutoGeneration.MessageBoard persistentMessageBoard =
            //    new Durados.DataAccess.AutoGeneration.MessageBoard(systemConnectionString, messageBoardSchemaGeneratorFileName);


            string sessionSchemaGeneratorFileName = SystemDynamicMapper.GetGenerateScriptFileName(Maps.GetDeploymentPath("Sql/Session.sql"));
            session = GetPersistentSessionGenerator(SystemDynamicMapper, systemConnectionString, sessionSchemaGeneratorFileName);

            string contentSchemaGeneratorFileName = SystemDynamicMapper.GetGenerateScriptFileName(Maps.GetDeploymentPath("Sql/Content.sql"));
            Durados.DataAccess.AutoGeneration.Content persistentContent = SystemDynamicMapper.GetPersistentContentGenerator(systemConnectionString, contentSchemaGeneratorFileName);


            string linkSchemaGeneratorFileName = SystemDynamicMapper.GetGenerateScriptFileName(Maps.GetDeploymentPath("Sql/Link.sql"));
            Durados.DataAccess.AutoGeneration.Link persistentLink = SystemDynamicMapper.GetLinkGenerator(systemConnectionString, linkSchemaGeneratorFileName);


            //string mailingServiceSchemaGeneratorFileName = Maps.GetDeploymentPath("Sql/MailingService.sql");
            //Durados.DataAccess.AutoGeneration.MailingService mailingServiceLink =
            //    new Durados.DataAccess.AutoGeneration.MailingService(systemConnectionString, mailingServiceSchemaGeneratorFileName);

            //string workFlowSchemaGeneratorFileName = Maps.GetDeploymentPath("Sql/WorkFlowGraph.sql");

            //Durados.DataAccess.AutoGeneration.WorkFlowGraph wf = new Durados.DataAccess.AutoGeneration.WorkFlowGraph(systemConnectionString, workFlowSchemaGeneratorFileName);

            string customViewsFileName = SystemDynamicMapper.GetGenerateScriptFileName(Maps.GetDeploymentPath("Sql/CustomViews.sql"));

            Durados.DataAccess.AutoGeneration.CustomViews cv = SystemDynamicMapper.GetCustomViewsGenerator(systemConnectionString, customViewsFileName);

            string userFileName = SystemDynamicMapper.GetGenerateScriptFileName(Maps.GetDeploymentPath("Sql/DuradosUsers.sql"));

            bool firstTime = false;
            Durados.DataAccess.AutoGeneration.User u = SystemDynamicMapper.GetUserGenerator(systemConnectionString, userFileName);
            if (!u.Exists())
            {
                u.Buid();
                firstTime = true;

            }

            if (this is DuradosMap && !Maps.PrivateCloud)
            {
                string plugInFileName = Maps.GetDeploymentPath("Sql/PlugIn.sql");

                Durados.DataAccess.AutoGeneration.PlugIn plugIn = new Durados.DataAccess.AutoGeneration.PlugIn(systemConnectionString, plugInFileName);

            }


            AddSystemTables(ds);
            if (ds.Tables.Contains("User") && ds.Tables["User"].Columns.Contains("Password"))
                ds.Tables["User"].Columns["Password"].AllowDBNull = true;

            DynamicMapper.AddSchema(ds);



            Initiate(ds, connectionString, this.ConfigFileName, save); //"~/bugit2.xml");

            if (firstTime && Maps.MultiTenancy)
            {
                InitiateFirstTime();



            }
            else
            {
                if (Maps.MultiTenancy)// && !(this is DuradosMap))
                {
                    var workspace = db.GetPublicWorkspace();
                    if (workspace == null)
                    {
                        ConfigWorkspace();
                    }


                    if (!(this is DuradosMap) && !HasRule("newUserVerification"))
                    {
                        AddNewUserVerification();
                        Commit();
                    }
                    if (!(this is DuradosMap) && !HasRule("beforeSocialSignup"))
                    {
                        AddBeforeSocialSignup();
                        Commit();
                    }
                    if (!(this is DuradosMap) && !HasRule("requestResetPassword"))
                    {
                        AddSendForgotPassword();
                        Commit();
                    }

                    if (!(this is DuradosMap) && !HasRule(Database.CustomValidationActionName))
                    {
                        AddBackandAuthenticationOverride();
                        Commit();
                    }

                    if (!(this is DuradosMap) && !HasRule("userApproval"))
                    {
                        AddUserApproval();
                        Commit();
                    }
                    if (!(this is DuradosMap) && !HasRule("User Invitation Email"))
                    {
                        AddNewUserInvitation();
                        Commit();
                    }
                    if (!(this is DuradosMap) && !HasRule("Admin Invitation Email"))
                    {
                        AddNewAdminInvitation();
                        Commit();
                    }


                    //if (!ConfigAccess.ContainsCategoryName("General", configDatabase.ConnectionString))
                    //{
                    //    ConfigCategory();
                    //}
                }
            }

            if (this is DuradosMap && Maps.MultiTenancy)
            {
                View userView = (View)Database.GetUserView();
                userView.Precedent = true;
                userView.AllowSelectRoles = "Developer";

                string dnsAliasFileName = Maps.GetDeploymentPath("Sql/DnsAlias.sql");

                Durados.DataAccess.AutoGeneration.DnsAlias dnsAlias = new Durados.DataAccess.AutoGeneration.DnsAlias(systemConnectionString, dnsAliasFileName);


                View appView = (View)Database.Views["durados_App"];

                Field sysConnectionField = appView.GetFieldByColumnNames("SystemSqlConnectionId");
                Field secConnectionField = appView.GetFieldByColumnNames("SecuritySqlConnectionId");

                sysConnectionField.Precedent = true;
                sysConnectionField.AllowSelectRoles = "Developer";
                sysConnectionField.AllowEditRoles = "Developer";
                sysConnectionField.AllowCreateRoles = "Developer";
                //sysConnectionField.HideInCreate = true;

                secConnectionField.Precedent = true;
                secConnectionField.AllowSelectRoles = "Developer";
                secConnectionField.AllowEditRoles = "Developer";
                secConnectionField.AllowCreateRoles = "Developer";
                //secConnectionField.HideInCreate = true;

                if (!Maps.PrivateCloud)
                {
                    View connView = (View)Database.Views["durados_SqlConnection"];
                    Field integratedSecurityField = connView.GetFieldByColumnNames("IntegratedSecurity");
                    integratedSecurityField.Excluded = true;
                }
            }

            if (firstTime && Maps.MultiTenancy)
            {
                string basicFieldName = "Basic";
                Durados.Web.Mvc.MapDataSet.durados_AppRow dr = Durados.Web.Mvc.Maps.Instance.GetAppRow();
                if (dr != null && dr.Table.Columns.Contains(basicFieldName) && !dr.IsNull(basicFieldName) && ((bool)dr[basicFieldName]))
                {
                    AddViews(dr);
                }

            }
            return firstTime;
        }

        private PersistentSession GetPersistentSessionGenerator(DataAccess.AutoGeneration.Dynamic.Mapper mapper, string systemConnectionString, string sessionSchemaGeneratorFileName)
        {
            if (mapper is Durados.DataAccess.AutoGeneration.Dynamic.MySqlMapper)
                return new MySqlPersistentSession(systemConnectionString, sessionSchemaGeneratorFileName);

            return new PersistentSession(systemConnectionString, sessionSchemaGeneratorFileName);
        }


        public bool HasRule(string ruleName)
        {
            return Database.GetUserView().GetRules().Where(r => r.Name.Equals(ruleName)).Count() > 0;
        }

        public Rule GetRule(string ruleName)
        {
            return Database.GetUserView().GetRules().Where(r => r.Name.Equals(ruleName)).FirstOrDefault();
        }

        public void NotifyUser(string subjectKey, string messageKey)
        {
            try
            {
                string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
                int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
                string smtpUsername = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
                string smtpPassword = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

                string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
                string subject = Database.Localizer.Translate(System.Web.Mvc.CmsHelper.GetContent(subjectKey));
                subject = string.IsNullOrEmpty(subject) ? subjectKey : subject;
                string message = Database.Localizer.Translate(System.Web.Mvc.CmsHelper.GetContent(messageKey));
                message = string.IsNullOrEmpty(message) ? messageKey : message;
                string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;
                message = message.Replace("[Url]", siteWithoutQueryString).Replace("[UserPreviewUrl]", this.GetPreviewPath());
                int appId = Convert.ToInt32(Maps.Instance.GetCurrentAppId());
                string to = Maps.Instance.DuradosMap.Database.GetCreatorUsername(appId);

                Durados.Cms.DataAccess.Email.Send(host, Database.UseSmtpDefaultCredentials, port, smtpUsername, smtpPassword, false, new string[1] { to }, null, null, subject, message, from, null, null, false, null, Logger);

            }
            catch (Exception exception)
            {
                Logger.Log("Map", "FirstTime", "NotifyUser", exception, 1, "Failed to Notify user on console ready.");
            }

        }

        public bool SaveChangesInConfigStructure()
        {
            if (IsChangesInConfigStructure && !(this is DuradosMap))
            {

                isChangesInConfigStructure = false;
                SetSaveChangesIndicationFromDb();

                //HandleWorkspaceContent();

                Commit();

                return true;
            }

            return false;
        }

        private bool? isChangesInConfigStructure = null;

        public bool IsChangesInConfigStructure
        {
            get
            {
                if (string.IsNullOrEmpty(Id))
                    return false;

                if (!isChangesInConfigStructure.HasValue)
                {
                    int config = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["isChangesInConfigStructure"] ?? "0");

                    int db = GetSaveChangesIndicationFromDb();

                    isChangesInConfigStructure = config >= db;
                }


                return isChangesInConfigStructure.Value;
            }
        }

        private int GetSaveChangesIndicationFromDb()
        {
            try
            {
                return GetSaveChangesIndicationFromDb2();
            }
            catch (SqlException)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(Maps.Instance.DuradosMap.connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            new SqlSchema().AddNewColumnToTable("durados_App", "ConfigChangesIndication", DataType.SingleSelect, command);
                        }
                    }
                    return GetSaveChangesIndicationFromDb2();
                }
                catch (Exception)
                {
                    return 1;
                }
            }
            catch (Exception)
            {
                return 1;
            }
        }

        private int GetSaveChangesIndicationFromDb2()
        {
            string sql = "select ConfigChangesIndication from durados_App where id = " + Id;
            string scalar = new SqlAccess().ExecuteScalar(Maps.Instance.DuradosMap.connectionString, sql);
            return Convert.ToInt32(string.IsNullOrEmpty(scalar) ? "0" : scalar);

        }

        private void SetSaveChangesIndicationFromDb()
        {
            int config = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["isChangesInConfigStructure"] ?? "0") + 1;

            string sql = "Update durados_App set ConfigChangesIndication = " + config + " where id = " + Id;
            SqlAccess sqlAccess = new SqlAccess();
            try
            {
                sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql);
            }
            catch (SqlException)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(Maps.Instance.DuradosMap.connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            new SqlSchema().AddNewColumnToTable("durados_App", "ConfigChangesIndication", DataType.SingleSelect, command);
                        }
                    }
                    sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql);
                }
                catch (Exception)
                {

                }
            }
            catch (Exception)
            {

            }

        }

        public void ChartsBackwardCompatebility()
        {
            bool config = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["chartsBackwardCompatebility"] ?? "true");
            bool isNecessary = config && !Database.Map.IsMainMap && Database.Dashboards.Count() == 0 && Database.MyCharts.Charts.Count > 0;

            if (!isNecessary)
            {
                return;
            }

            Commit();

            //View dashboardView = (View)configDatabase.Views["MyCharts"];
            //View chartView = (View)configDatabase.Views["Chart"];

            //Dictionary<string, object> values = new Dictionary<string, object>();

            //values.Add("Name", Database.MyCharts.Name);
            //values.Add("Columns", 2);
            //values.Add("Columns", 2);
            ////...
            //DataRow dashboardRow = dashboardView.Create(values, null, null, null, null, null);

            string dashboardId = CreateDashboard(Database.MyCharts.Name);

            int index = 1;

            foreach (Chart chart in Database.MyCharts.Charts.Values)
            {
                Dictionary<string, object> chartValues = new Dictionary<string, object>();
                chartValues.Add("Name", chart.Name);
                chartValues.Add("SubTitle", chart.SubTitle);
                chartValues.Add("SQL", chart.SQL);
                chartValues.Add("XField", chart.XField);
                chartValues.Add("YField", chart.YField);
                chartValues.Add("XTitle", chart.XTitle);
                chartValues.Add("YTitle", chart.YTitle);
                chartValues.Add("Height", chart.Height);
                chartValues.Add("WorkspaceID", chart.WorkspaceID);
                chartValues.Add("ChartType", chart.ChartType);
                chartValues.Add("Precedent", chart.Precedent);
                chartValues.Add("AllowSelectRoles", chart.AllowSelectRoles);
                chartValues.Add("RefreshInterval", chart.RefreshInterval);
                chartValues.Add("GaugeGreen", chart.GaugeGreen);
                chartValues.Add("GaugeRed", chart.GaugeRed);
                chartValues.Add("GaugeYellow", chart.GaugeYellow);
                chartValues.Add("GaugeMaxValue", chart.GaugeMaxValue);
                chartValues.Add("GaugeMinValue", chart.GaugeMinValue);

                AddNewChartToDashboard(dashboardId, index, chart.Align == ChartAlignment.Left ? 1 : 2, chartValues, false);

            }

            Commit();


        }
        public string AddNewChartToDashboard(string dashboardId, int ordinal, int column, Dictionary<string, object> values, bool save)
        {
            values = values ?? new Dictionary<string, object>();
            values.Add("Charts_Parent", dashboardId);
            values.Add("Ordinal", ordinal);
            values.Add("Column", column);

            Durados.Database configDatabase = GetConfigDatabase();
            View view = (View)configDatabase.Views["Chart"];
            DataRow row = view.Create(values, null, null, null, null, null);
            string chartId = view.GetPkValue(row);
            if (save)
                ConfigAccess.SaveConfigDataset(configDatabase.ConnectionString, Logger);

            int id = Convert.ToInt32(dashboardId);
            if (Database.Dashboards.ContainsKey(id))
            {
                Database.Dashboards[id].Charts.Add(Convert.ToInt32(chartId), new Chart() { Ordinal = ordinal, Column = column, WorkspaceID = 0, Precedent = false, Database = Database, ID = Convert.ToInt32(chartId) });
            }
            return chartId;
        }
        public string CreateDashboard(string dashboardName)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", dashboardName);
            values.Add("Dashboards_Parent", 0);
            values.Add("Columns", 2);

            Durados.Database configDatabase = GetConfigDatabase();
            View view = (View)configDatabase.Views["MyCharts"];
            DataRow row = view.Create(values, null, null, null, null, null);
            return view.GetPkValue(row);

        }

        public void MenusBackwardCompatebility()
        {
            bool config = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["menusBackwardCompatebility"] ?? "true");
            bool isNecessary = config && !Database.Map.IsMainMap && Database.GetPublicWorkspace().SpecialMenus.Count() == 0 && Database.Menus.Count > 1 && Database.Views.Values.Where(v => v.WorkspaceID != Database.GetAdminWorkspaceId() && !v.HideInMenu).Count() > 0;
            foreach (Workspace workspace in Database.Workspaces.Values.Where(w => w.Name != "Admin"))
            {
                if (workspace.SpecialMenus.Count > 0)
                {
                    isNecessary = false;
                    break;
                }
            }

            //isNecessary = false;
            if (isNecessary)
            {
                Commit();
                //HandleWorkspaceContent();

                View specialMenuView = (View)configDatabase.Views["SpecialMenu"];
                string databasePK = ConfigAccess.GetDatabasePK(configDatabase.ConnectionString);
                //Database databasex = new Database(dataset, this);

                foreach (Workspace workspace in Database.Workspaces.Values)
                {
                    int? workspaceId = ConfigAccess.GetWorkspaceId(workspace.Name, configDatabase.ConnectionString);
                    string workspaceIdString = workspaceId.HasValue ? workspaceId.Value.ToString() : string.Empty;
                    if (workspace.Name != "Admin")
                    {
                        foreach (Durados.Web.Mvc.View view in Database.GetWorkspaceViewsWithoutMenu(workspace).Where(c => !c.HideInMenu).OrderBy(v => v.Order))
                        {
                            //if (!databasex.SpecialMenus.ContainsKey(view.DisplayName))
                            //{
                            Dictionary<string, object> values = new Dictionary<string, object>();

                            values.Add("Name", view.DisplayName);
                            values.Add("SpecialMenus_Parent", workspaceIdString);


                            values.Add("Ordinal", view.Order);
                            values.Add("Url", "/" + view.Controller + "/" + view.IndexAction + "/" + view.Name);
                            values.Add("WorkspaceID", workspaceIdString);


                            values.Add("ViewName", view.Name);
                            DataRow menuRow = specialMenuView.Create(values, null, null, null, null, null);

                            menuRow["Url"] = values["Url"];
                            menuRow["ViewName"] = values["ViewName"];
                            menuRow["LinkType"] = LinkType.View.ToString();

                            //    string menuPk = specialMenuView.GetPkValue(menuRow);
                            //    databasex.SpecialMenus.Add(view.DisplayName, new SpecialMenu() { ID = Convert.ToInt32(menuPk), Name = view.DisplayName });

                            //}
                        }

                        foreach (Durados.Page page in Database.GetWorkspacePagesWithoutMenu(workspace).OrderBy(v => v.Order))
                        {
                            //if (!databasex.SpecialMenus.ContainsKey(page.Title))
                            //{
                            Dictionary<string, object> values = new Dictionary<string, object>();

                            values.Add("Name", page.Title);
                            values.Add("SpecialMenus_Parent", workspaceIdString);


                            values.Add("Ordinal", page.Order);
                            values.Add("Url", "/Home/Page?pageId=" + page.ID);
                            values.Add("WorkspaceID", workspaceIdString);


                            values.Add("ViewName", page.ID);
                            DataRow menuRow = specialMenuView.Create(values, null, null, null, null, null);
                            menuRow["Url"] = values["Url"];
                            menuRow["ViewName"] = values["ViewName"];
                            if (page.PageType == Durados.PageType.ReportingServices)
                            {
                                menuRow["LinkType"] = LinkType.Report.ToString();
                                menuRow["ReportServicePath"] = page.ReportName;
                            }
                            else
                            {
                                menuRow["LinkType"] = LinkType.Page.ToString();
                            }

                            //    string menuPk = specialMenuView.GetPkValue(menuRow);
                            //    databasex.SpecialMenus.Add(page.Title, new SpecialMenu() { ID = Convert.ToInt32(menuPk), Name = page.Title });
                            //}
                        }

                        foreach (Durados.Menu menu in Database.GetWorkspaceMenus(workspace).OrderBy(m => m.Ordinal))
                        {
                            //if (!databasex.SpecialMenus.ContainsKey(menu.Name))
                            //{
                            Dictionary<string, object> values = new Dictionary<string, object>();

                            values.Add("Name", menu.Name);
                            values.Add("SpecialMenus_Parent", workspaceIdString);
                            values.Add("Ordinal", menu.Ordinal + 1000000);
                            values.Add("WorkspaceID", workspaceIdString);


                            DataRow menuRow = specialMenuView.Create(values, null, null, null, null, null);
                            string menuPk = specialMenuView.GetPkValue(menuRow);


                            if (menu.Views.Count > 0)
                            {
                                View view = (View)menu.Views[0];
                                menuRow["Url"] = "/" + view.Controller + "/" + view.IndexAction + "/" + view.Name;
                                menuRow["ViewName"] = view.Name;
                                menuRow["LinkType"] = LinkType.View.ToString();

                            }
                            else if (menu.Pages.Count > 0)
                            {
                                Durados.Page page = menu.Pages[0];
                                menuRow["Url"] = "/Home/Page?pageId=" + page.ID;
                                menuRow["ViewName"] = page.ID;
                                if (page.PageType == Durados.PageType.ReportingServices)
                                {
                                    menuRow["LinkType"] = LinkType.Report.ToString();
                                    menuRow["ReportServicePath"] = page.ReportName;
                                }
                                else
                                {
                                    menuRow["LinkType"] = LinkType.Page.ToString();
                                }
                            }

                            //databasex.SpecialMenus.Add(menu.Name, new SpecialMenu() { ID = Convert.ToInt32(menuPk), Name = menu.Name });
                            //SpecialMenu parentSpecialMenu = databasex.GetMenuById(Convert.ToInt32(menuPk));
                            int i = 0;
                            foreach (Durados.Web.Mvc.View view in menu.Views.Where(c => !c.HideInMenu))
                            {
                                //if (!parentSpecialMenu.Menus.ContainsKey(view.DisplayName))
                                //{
                                values = new Dictionary<string, object>();

                                values.Add("Name", view.DisplayName);
                                values.Add("Menus_Parent", menuPk);
                                values.Add("Ordinal", i++);
                                values.Add("ViewName", view.Name);
                                values.Add("Url", "/" + view.Controller + "/" + view.IndexAction + "/" + view.Name);

                                values.Add("WorkspaceID", workspaceIdString);

                                DataRow menuRow2 = specialMenuView.Create(values, null, null, null, null, null);
                                menuRow2["Url"] = values["Url"];
                                menuRow2["ViewName"] = values["ViewName"];
                                menuRow2["Menus"] = Convert.ToInt32(menuPk);
                                menuRow2["LinkType"] = LinkType.View.ToString();

                                //string menuPk2 = specialMenuView.GetPkValue(menuRow);
                                //parentSpecialMenu.Menus.Add(view.DisplayName, new SpecialMenu() { ID = Convert.ToInt32(menuPk2), Name = view.DisplayName });
                                //}
                            }

                            foreach (Durados.Page page in menu.Pages)
                            {
                                //if (!parentSpecialMenu.Menus.ContainsKey(page.Title))
                                //{
                                values = new Dictionary<string, object>();

                                values.Add("Name", page.Title);
                                values.Add("Menus_Parent", menuPk);
                                values.Add("Ordinal", i++);
                                values.Add("ViewName", page.ID);
                                values.Add("Url", "/Home/Page?pageId=" + page.ID);

                                values.Add("WorkspaceID", workspaceIdString);

                                DataRow menuRow2 = specialMenuView.Create(values, null, null, null, null, null);
                                menuRow2["Url"] = values["Url"];
                                menuRow2["ViewName"] = values["ViewName"];
                                menuRow2["Menus"] = Convert.ToInt32(menuPk);
                                if (page.PageType == Durados.PageType.ReportingServices)
                                {
                                    menuRow2["LinkType"] = LinkType.Report.ToString();
                                    menuRow2["ReportServicePath"] = page.ReportName;
                                }
                                else
                                {
                                    menuRow2["LinkType"] = LinkType.Page.ToString();
                                }
                                //string menuPk2 = specialMenuView.GetPkValue(menuRow);
                                //    parentSpecialMenu.Menus.Add(page.Title, new SpecialMenu() { ID = Convert.ToInt32(menuPk2), Name = page.Title });
                                //}
                            }
                            //}
                        }
                    }
                }
                Commit();
            }

        }

        private void HandleWorkspaceContent()
        {
            HandleAdminWorkspaceContent();
            HandlePublicWorkspaceContent();

        }

        private void HandleAdminWorkspaceContent()
        {
            View workspaceView = (View)configDatabase.Views["Workspace"];
            string adminWorkspaceId = Database.GetAdminWorkspaceId().ToString();

            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add("Description", GetWorkspaceAdminDescription());
            workspaceView.Edit(values, adminWorkspaceId, null, null, null, null);

        }

        private void HandlePublicWorkspaceContent()
        {
            string description = GetWorkspaceDefaultDescription();

            string config = System.Configuration.ConfigurationManager.AppSettings["oldDescriptionText"] ?? "Organize the menus";
            string oldDescriptionText = Database.GetPublicWorkspace().Description;

            if (!oldDescriptionText.Contains(config))
                return;

            View workspaceView = (View)configDatabase.Views["Workspace"];
            string publicWorkspaceId = Database.GetPublicWorkspaceId().ToString();

            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add("Description", description);
            workspaceView.Edit(values, publicWorkspaceId, null, null, null, null);

        }

        private void AddViews(Durados.Web.Mvc.MapDataSet.durados_AppRow dr)
        {
            string errorMessage = string.Empty;
            DataView dataView = GetSchemaEntities();
            List<string> pkList = new List<string>();
            string tableEntity = DynamicMapper.GetTableEntity();
            foreach (System.Data.DataRowView row in dataView)
            {
                if (row["EntityType"].Equals(tableEntity))
                {
                    pkList.Add(row["Name"].ToString());
                }
            }
            string pks = pkList.ToArray().Delimited();
            int added = Database.AddViews(pks, dataView, true, out errorMessage);
            if (added == -100)
            {
                for (int i = 0; i < 5; i++)
                {
                    added = Database.AddViews(pks, dataView, true, out errorMessage);
                    if (added != -100)
                        return;
                }
            }
        }

        private void InitiateFirstTime()
        {
            SetSaveChangesIndicationFromDb();
            AddCreator();
            AddDemo();
            AddNewUserVerification();
            AddBeforeSocialSignup();
            AddSendForgotPassword();
            AddBackandAuthenticationOverride();
            AddUserApproval();
            AddSyncUserRules();
            AddNewUserInvitation();
            AddNewAdminInvitation();
            ConfigWorkspace();
            SetDefaultNewUserDefaultRole();
            //ConfigCategory();
            Commit();
            //CopyDefaultPage();
            ConfigLocalization();
            Commit();
            try
            {
                //Maps.Instance.UpdateOnBoardingStatus(OnBoardingStatus.Ready, Id);
            }
            catch (Exception exception)
            {
                Logger.Log("Map", "Initiate", "UpdateOnBoardingStatus", exception, 1, "Failed to Update OnBoarding Status");
            }
        }

        private void SetDefaultNewUserDefaultRole()
        {
            View databaseView = (View)configDatabase.Views["Database"];

            databaseView.Edit(new Dictionary<string, object>() { { "NewUserDefaultRole", "User" } }, "0", null, null, null, null);
        }

        ////public string GetLocalDatabaseHost()
        ////{
        ////    return (System.Configuration.ConfigurationManager.AppSettings["localDatabaseHost"] ?? "yrv-dev.czvbzzd4kpof.eu-central-1.rds.amazonaws.com").ToString();
        ////}
        public string GetConnectionSource()
        {
            try
            {
                //string localDatabaseHost = GetLocalDatabaseHost();
                if(HostedByUs)// (connectionString.Contains(localDatabaseHost))
                {
                    return "local";
                }
                else
                {
                    return "external";
                }

            }
            catch
            {
                return "unknown";
            }
        }

        private void AddSyncUserRulesOld()
        {
            const string USERS = "users";

            string whereCondition = GetConnectionSource() == "local" ? "true" : "false";

            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder();
            ConfigAccess configAccess = new DataAccess.ConfigAccess();
            string userViewPK = configAccess.GetViewPK(Database.UserViewName, configDatabase.ConnectionString);
            View ruleView = (View)configDatabase.Views["Rule"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", "Create My App User");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.AfterCreateBeforeCommit.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.Execute.ToString());
            values.Add("WhereCondition", whereCondition);
            values.Add("ExecuteMessage", "Your objects do not contain a users object. Please set the where condition to false in the Security & Auth action \"Create My App User\".");
            values.Add("ExecuteCommand", "insert into " + sqlTextBuilder.EscapeDbObject(USERS) + " (" + sqlTextBuilder.EscapeDbObject("email") + "," + sqlTextBuilder.EscapeDbObject("firstName") + "," + sqlTextBuilder.EscapeDbObject("lastName") + ") " + sqlTextBuilder.Top("select '{{Username}}','{{FirstName}}','{{LastName}}' " + sqlTextBuilder.FromDual() + " WHERE NOT EXISTS (SELECT * FROM " + sqlTextBuilder.EscapeDbObject(USERS) + " WHERE " + sqlTextBuilder.EscapeDbObject("email") + "='{{Username}}' ) ", 1));
            ruleView.Create(values, null, null, null, null, null);

            values = new Dictionary<string, object>();
            values.Add("Name", "Update My App User");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.AfterEditBeforeCommit.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.Execute.ToString());
            values.Add("WhereCondition", whereCondition);
            values.Add("ExecuteMessage", "Your objects do not contain a users object. Please set the where condition to false in the Security & Auth action \"Update My App User\".");
            values.Add("ExecuteCommand", "update " + sqlTextBuilder.EscapeDbObject(USERS) + " set " + sqlTextBuilder.EscapeDbObject("firstName") + " = '{{FirstName}}', " + sqlTextBuilder.EscapeDbObject("lastName") + " = '{{lastName}}' where " + sqlTextBuilder.EscapeDbObject("email") + " = '{{Username}}'");
            ruleView.Create(values, null, null, null, null, null);

            values = new Dictionary<string, object>();
            values.Add("Name", "Delete My App User");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.AfterDeleteBeforeCommit.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.Execute.ToString());
            values.Add("WhereCondition", whereCondition);
            values.Add("ExecuteMessage", "Your objects do not contain a users object. Please set the where condition to false in the Security & Auth action \"Delete My App User\".");
            values.Add("ExecuteCommand", "delete from " + sqlTextBuilder.EscapeDbObject(USERS) + " where " + sqlTextBuilder.EscapeDbObject("email") + " = '{{Username}}'");
            ruleView.Create(values, null, null, null, null, null);
        }


        private string GetJsCode(string internalCode)
        {
            string code = "/* globals\n" +
            "$http - Service for AJAX calls \n" +
            "CONSTS - CONSTS.apiUrl for Backands API URL\n" +
            "Config - Global Configuration\n" +
            "*/\n" +
            "'use strict';\n" +
            "function backandCallback(userInput, dbRow, parameters, userProfile) {\n" +
            "   \n" +
            internalCode + "\n" +
            "   return {};\n" +
            "}";

            return code;
        }


        string indentSpaces = "   ";

        private string GetPostCode(string varName, string objectName, Dictionary<string, object> parameters, Dictionary<string, object> data, int indent = 1)
        {
            return GetPostCode(varName, objectName, JsonConvert.SerializeObject(parameters ?? new Dictionary<string, object>()), JsonConvert.SerializeObject(data ?? new Dictionary<string, object>()), indent);
        }

        private string GetPostCode(string varName, string objectName, string parameters, string data, int indent = 1)
        {
            return String.Concat(Enumerable.Repeat(indentSpaces, indent)) + "var " + varName + " = $http({\n" +
         String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "method: \"POST\",\n" +
         String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "url:CONSTS.apiUrl + \"/1/objects/" + objectName + "\",\n" +
         String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "params: " + (parameters ?? "{}") + ",\n" +
         String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "data: " + data + ",\n" +
         String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "headers: {\"Authorization\": userProfile.token}\n" +
         String.Concat(Enumerable.Repeat(indentSpaces, indent)) + "});";
        }

        public string GetPutCode(string varName, string objectName, string id, Dictionary<string, object> parameters, Dictionary<string, object> data, int indent = 1)
        {
            return GetPutCode(varName, objectName, id, JsonConvert.SerializeObject(parameters ?? new Dictionary<string, object>()), JsonConvert.SerializeObject(data ?? new Dictionary<string, object>()), indent);
        }

        public string GetPutCode(string varName, string objectName, string id, string parameters, string data, int indent = 1)
        {
            string code = String.Concat(Enumerable.Repeat(indentSpaces, indent)) + "var " + varName + " = $http({\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "method: \"PUT\",\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "url:CONSTS.apiUrl + \"/1/objects/" + objectName + "/" + id + "\",\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "params: " + (parameters ?? "{}") + ",\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "data: " + data + ",\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "headers: {\"Authorization\": userProfile.token}\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent)) + "});";
            return code;
        }

        public string GetDeleteCode(string varName, string objectName, string id, Dictionary<string, object> parameters, int indent = 1)
        {
            return GetDeleteCode(varName, objectName, id, JsonConvert.SerializeObject(parameters ?? new Dictionary<string, object>()), indent);
        }

        public string GetDeleteCode(string varName, string objectName, string id, string parameters, int indent = 1)
        {
            string code = String.Concat(Enumerable.Repeat(indentSpaces, indent)) + "var " + varName + " = $http({\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "method: \"DELETE\",\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "url:CONSTS.apiUrl + \"/1/objects/" + objectName + "/" + id + "\",\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "params: " + (parameters ?? "{}") + ",\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "headers: {\"Authorization\": userProfile.token}\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent)) + "});";
            return code;
        }

        public string GetAllCode(string varName, string objectName, Dictionary<string, object> parameters = null, int indent = 1)
        {
            return GetAllCode(varName, objectName, JsonConvert.SerializeObject(parameters ?? new Dictionary<string, object>()), indent);
        }

        public string GetAllCode(string varName, string objectName, string parameters = null, int indent = 1)
        {
            string code = String.Concat(Enumerable.Repeat(indentSpaces, indent)) + "var " + varName + " = $http({\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "method: \"GET\",\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "url:CONSTS.apiUrl + \"/1/objects/" + objectName + "\",\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "params: " + (parameters ?? "{}") + ",\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent + 1)) + "headers: {\"Authorization\": userProfile.token}\n" +
        String.Concat(Enumerable.Repeat(indentSpaces, indent)) + "});";
            return code;
        }

        private void AddSyncUserRules()
        {
            const string USERS = "users";

            string whereCondition = GetConnectionSource() == "local" ? "true" : "false";

            string createCode = "   // When a new user registers, add her to the users object. \n" +
                "   // If you are using a different object for your users then change this action accordingly. \n" +
                "   if (parameters.sync)\n" +
                "      return {};\n" +
                "   if (!parameters)\n" +
                "      parameters = {};\n" +
                "   parameters.email = userInput.Username;\n" +
                "   parameters.firstName = userInput.FirstName;\n" +
                "   parameters.lastName = userInput.LastName;\n" +
                "   try{\n" +
            GetPostCode("response", USERS, "{parameters: {\"sync\": true}}", "parameters", 1) + "\n" +
                "}\n" +
                "catch(err) {\n" +
                "   // register user even if there is an error or no users object \n" +
                "}";

            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder();
            ConfigAccess configAccess = new DataAccess.ConfigAccess();
            string userViewPK = configAccess.GetViewPK(Database.UserViewName, configDatabase.ConnectionString);
            View ruleView = (View)configDatabase.Views["Rule"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", "Create My App User");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.AfterCreateBeforeCommit.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.JavaScript.ToString());
            values.Add("WhereCondition", whereCondition);
            values.Add("Code", GetJsCode(createCode));
            ruleView.Create(values, null, null, null, null, null);

            string updateCode = "   // When a registered user is changed, change your users object as well. \n" +
                "   // If you are using a different object for your users then change this action accordingly. \n" +
                "\n" +
                "   // Get the user id by the user's email\n" +
                GetAllCode("currentUser", USERS, "{filter:[{\"fieldName\":\"email\", \"operator\":\"equals\", \"value\": userInput.Username }]}") + "\n" +
                "   if (currentUser.data.length == 1) { \n" +
                "      var currentUserId = currentUser.data[0].__metadata.id; \n" +
                GetPutCode("response", USERS, "\" + currentUserId + \"", null, "{\"firstName\": userInput.FirstName, \"lastName\": userInput.LastName }", 2) + "\n" +
                "   } \n";

            values = new Dictionary<string, object>();
            values.Add("Name", "Update My App User");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.AfterEditBeforeCommit.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.JavaScript.ToString());
            values.Add("WhereCondition", whereCondition);
            values.Add("Code", GetJsCode(updateCode));
            ruleView.Create(values, null, null, null, null, null);


            string deleteCode = "   // When a registered user name is deleted, delete her from your users object as well. \n" +
                "   // If you are using a different object for your users then change this action accordingly. \n" +
                "\n" +
                "   // Get the user id by the user's email\n" +
                GetAllCode("currentUser", USERS, "{filter:[{\"fieldName\":\"email\", \"operator\":\"equals\", \"value\": dbRow.Username }]}") + "\n" +
                "   if (currentUser.data.length == 1) { \n" +
                "      var currentUserId = currentUser.data[0].__metadata.id; \n" +
                GetDeleteCode("response", USERS, "\" + currentUserId + \"", "{}", 2) + "\n" +
                "   } \n";


            values = new Dictionary<string, object>();
            values.Add("Name", "Delete My App User");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.AfterDeleteBeforeCommit.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.JavaScript.ToString());
            values.Add("WhereCondition", whereCondition);
            values.Add("Code", GetJsCode(deleteCode));
            ruleView.Create(values, null, null, null, null, null);
        }

        private ISqlTextBuilder GetSqlTextBuilder()
        {
            ISqlTextBuilder sqlTextBuilder = null;

            switch (SqlProduct)
            {
                case SqlProduct.MySql:
                    sqlTextBuilder = new MySqlTextBuilder();
                    break;
                case SqlProduct.Postgre:
                    sqlTextBuilder = new PostgreTextBuilder();
                    break;
                case SqlProduct.Oracle:
                    sqlTextBuilder = new OracleTextBuilder();
                    break;

                default:
                    sqlTextBuilder = new SqlTextBuilder();
                    break;
            }

            return sqlTextBuilder;
        }

        private void ConfigLocalization()
        {
            View localizationView = (View)configDatabase.Views["Localization"];
            string databasePK = ConfigAccess.GetDatabasePK(configDatabase.ConnectionString);

            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add("Name", "Public");
            values.Add("AddKeyIfMissing", true);
            values.Add("ReturnKeyIfMissing", true);
            values.Add("Prefix", string.Empty);
            values.Add("Postfix", string.Empty);
            values.Add("Title", "Translation Settings");
            values.Add("TranslateKeyIfMissing", false);
            values.Add("TranslateKeyOrDefaultLanguage", false);
            values.Add("DefaultLanguage", "en-us");
            values.Add("LocalizationSchemaGeneratorFileName", string.Empty);
            values.Add("LocalizationConnectionStringKey", string.Empty);

            DataRow localizationRow = localizationView.Create(values, null, null, null, null, null);

            View databaseView = (View)configDatabase.Views["Database"];

            values = new Dictionary<string, object>();

            values.Add("Localization_Parent", localizationView.GetPkValue(localizationRow));

            databaseView.Edit(values, databasePK, null, null, null, null);
        }

        //public void ConfigCategory()
        //{
        //    Dictionary<string, object> values = new Dictionary<string, object>();

        //    View categoryView = (View)configDatabase.Views["Category"];
        //    if (!ConfigAccess.ContainsCategoryName("General", configDatabase.ConnectionString))
        //    {
        //        values.Add("Name", "General");
        //        values.Add("Ordinal", 0);

        //        categoryView.Create(values, null, null, null, null, null);
        //    }

        //}

        public void ConfigWorkspace()
        {
            string adminRoles = "Developer,Admin";
            string OwnerRoles = "Developer,Admin,View Owner";
            string publicRoles = "Developer,Admin,View Owner,User";
            string readOnlyRoles = "ReadOnly";
            View workspaceView = (View)configDatabase.Views["Workspace"];
            string databasePK = ConfigAccess.GetDatabasePK(configDatabase.ConnectionString);

            Dictionary<string, object> values = new Dictionary<string, object>();

            if (!ConfigAccess.ContainsWorkspaceName("Public", configDatabase.ConnectionString))
            {
                values.Add("Name", "Public");
                values.Add("Workspaces_Parent", databasePK);
                //values.Add("Description", "<h1>Public workspace page</h1><h2>The following text should be replaced by your content...</h2><h3>Welcome to ModuBiz, you can start by browse to the Admin area by clicking on the <img src=\"/Admin/Download/Workspace?fieldName=Description&amp;filename=/moduBiz.png&amp;pk=\" height=\"30\">&nbsp;icon.</h3><h3>Any user can manage the tables content clicking on the menu and promptly start to add, edit, and delete records.</h3><h3>Admin user can do the following:<ul><li>* Explore all Admin functionality by selecting the <a href=\"/Home/Default?workspaceId=1\">Admin </a>work-space<br></li></ul><ul><li>* Update the column's behavior by right clicking on the column's header</li></ul><ul><li>* Add a new column (= new field to the table in the database) by right clicking on the header and selecting \"Add Column\"</li></ul><ul><li>* Update a view's behavior by&nbsp;clicking&nbsp;on the view name (title)<br></li></ul><ul><li>* Add new tables or views from the database in the Admin work-space menu <a href=\"/Admin/IndexPage/View?guid=View_g3_&amp;mainPage=True&amp;path=Design%20-%20\">Design - Views</a></li></ul></h3>");
                //values.Add("Description", "<div class=\"workspace-content\"><h1>Welcome to Your: Application Management Console</h1><h2>From here you can start managing your application.</h2><h3>Use the menus above to navigate between the tables and views in your database.<br>Let other users view or edit data<br>Use role based security to determine what other users can see and do<br>Explore all Admin functionality by selecting the <a href=\"/Home/Default?workspaceId=1\">Admin </a>work-space<br>To learn more go to <a href=\"http://www.modubiz.com/support\">www.modubiz.com/support</a><br><br>This is the Public workspace page – All The text should be replaced by your content…</h3></div>");
                values.Add("Description", GetWorkspaceDefaultDescription());
                values.Add("AllowCreateRoles", publicRoles);
                values.Add("AllowEditRoles", publicRoles);
                values.Add("AllowDeleteRoles", publicRoles);
                values.Add("AllowSelectRoles", string.Format("{0},{1}", publicRoles, readOnlyRoles));
                values.Add("ViewOwnerRoles", OwnerRoles);
                values.Add("Precedent", true);

                workspaceView.Create(values, null, null, null, null, null);
            }

            if (!ConfigAccess.ContainsWorkspaceName("Admin", configDatabase.ConnectionString))
            {
                values = new Dictionary<string, object>();
                values.Add("Name", "Admin");
                values.Add("Workspaces_Parent", databasePK);
                //values.Add("Description", "<h1>Admin workspace landing page</h1><h2>The main functionality of this page:</h2><div><ul><li><font size=\"4\"><b>Design</b></font></li><ul><li style=\"font-size: 18px;\"><b>- Defaults</b>: Control the application's default&nbsp;behavior</li></ul><ul><li style=\"font-size: 18px;\"><b>- Views</b>: A list of all the tables and views that selected from the database. You can add new tables by selecting \"<b>Add Views</b>\". To view the <b>ERD </b>of the&nbsp;database&nbsp;click on the summary icon</li><li style=\"font-size: 18px;\">- <b>Fields</b>: A list of all the fields in the entire database</li><li style=\"font-size: 18px;\">- <b>Menus</b>: Manage the menu in the work-space</li><li style=\"font-size: 18px;\">- <b>Categories</b>: A category is used to create Tab style in form view</li><li style=\"font-size: 18px;\">- <b>FTP Upload</b>: Manage all the FTP locations that host files</li><li style=\"font-size: 18px;\">- <b>Content</b>: A content management page for emails, user notifications and more</li><li style=\"font-size: 18px;\">- <b>Charts</b>: Manage the details in the chart dashboard</li></ul></ul><ul><li style=\"font-size: 18px;\"><b>Users &amp; Roles</b></li><ul><li style=\"font-size: 18px;\"><b>Users:&nbsp;</b>&nbsp;Manage the list of users and their roles</li><li style=\"font-size: 18px;\"><b>Roles</b>:&nbsp;Manage the&nbsp;roles</li><li style=\"font-size: 18px;\"><b>Workspaces</b>: Manage the works-paces content page and security access list</li></ul></ul><ul><li><font size=\"4\"><b>Trace</b></font></li><ul><li><font size=\"4\"><b>Monitor</b>: Log all the activity and exceptions</font></li><li><font size=\"4\"><b>History</b>: Display all the recorded changes made by user. In order to record the changes, enable the History flag for each views</font></li></ul></ul><ul><li><font size=\"4\"><b>New Views</b></font></li><ul><li><font size=\"4\">Shows all the new views that have recently been added to the application</font></li></ul></ul></div>");
                values.Add("Description", GetWorkspaceAdminDescription());
                values.Add("AllowCreateRoles", adminRoles);
                values.Add("AllowEditRoles", adminRoles);
                values.Add("AllowDeleteRoles", adminRoles);
                values.Add("AllowSelectRoles", adminRoles);
                values.Add("ViewOwnerRoles", adminRoles);
                values.Add("Precedent", true);
                workspaceView.Create(values, null, null, null, null, null);
            }
            View databaseView = (View)configDatabase.Views["Database"];
            values = new Dictionary<string, object>();
            values.Add("DefaultWorkspaceId", ConfigAccess.GetWorkspaceId("Public", configDatabase.ConnectionString));
            databaseView.Edit(values, databasePK, null, null, null, null);

        }

        public static string GetWorkspaceAdminDescription()
        {
            return "<h1>Admin workspace landing page</h1><h2>The main functionality of Admin area:</h2><div><ul><li><font size=\"4\"><b>Design</b></font></li><ul><li style=\"font-size: 18px;\"><b>- Default Settings</b>: Manage the console default &amp; general settings</li></ul><ul><li style=\"font-size: 18px;\"><b>- Tables &amp; Views</b>: A list of all the tables and views that selected from the database. To view the <b>ERD </b>of the&nbsp;database&nbsp;click on the summary icon</li><li style=\"font-size: 18px;\">- <b>Fields</b>: A list of all the fields in the entire database</li><li style=\"font-size: 18px;\">-&nbsp;<b>Business Rules</b>: Based on Add, Edit and Delete events, add calls to stored procedures, Web services, advanced validations, automatic emails and more.</li><li style=\"font-size: 18px;\">- <b>Upload Storage</b>: Manage the Storage that host files</li><li style=\"font-size: 18px;\">- <b>Content</b>: Content management for emails, user notifications and more</li></ul></ul><ul><li style=\"font-size: 18px;\"><b>Users &amp; Roles</b></li><ul><li style=\"font-size: 18px;\"><b>Users:&nbsp;</b>&nbsp;Manage the list of users and their roles</li><li style=\"font-size: 18px;\"><b>Roles</b>:&nbsp;Manage the&nbsp;roles</li><li style=\"font-size: 18px;\"><b>Workspaces</b>: Manage the security workspaces and Menu workspaces</li></ul></ul><ul><li><font size=\"4\"><b>Trace</b></font></li><ul><li><font size=\"4\"><b>Monitor</b>: Log all the activity and exceptions</font></li><li><font size=\"4\"><b>History</b>: Display all the recorded changes made by user. In order to record the changes, enable the History flag for each views</font></li></ul></ul><ul><li><font size=\"4\"><b>New Views</b></font></li><ul><li><font size=\"4\">Shows all the new views that have recently been added to the application</font></li></ul></ul></div>";
        }


        public static string GetWorkspaceDefaultDescription()
        {
            string workspaceDefaultDescriptionFileName = Maps.GetDeploymentPath("Sql/WorkspaceDefaultDescription.txt");
            FileInfo file = new FileInfo(workspaceDefaultDescriptionFileName);
            string desc = file.OpenText().ReadToEnd();
            return desc;
        }

        public static string GetUserVerificationMessage()
        {
            return "<div>Hello {{firstName}},<br><br>Thank you for joining {{appName}}<br><br>For security reasons, Please click <a href=\"{{apiPath}}/1/user/verify?appName={{appName}}&parameters={{token}}\" >here</a> to verify your email.<br><br>Sincerely,<br><br>{{appName}} Team</Div>";
        }
        public static string GetUserApprovalMessage()
        {
            return "<div>Hello {{firstName}},<br><br>Thank you for joining {{appName}}<br><br>Your participants in {{appName}} has been approved, Please click <a href=\"{{signInUrl}}\" >here</a> to sign in.<br><br>Sincerely,<br><br>{{appName}} Team</Div>";
        }

        public static string GetAdminInvitationMessage()
        {
            return "<div>Hi {{FirstName}},<br><br>You have been invited to {{AppName}} management console.<br><br>Please click <a href=\"https://www.backand.com/apps/#/sign_up\" >here</a> to start.<br>If you are not a registered Backand user, you will have to register first.<br><br>Have fun,<br><br>{{AppName}} Team</div>";
        }

        public static string GetUserInvitationMessage()
        {
            return "<div>Hi {{FirstName}},<br><br>You have been invited to use {{AppName}}.<br><br>Please click <a href=\"{{RegistrationRedirectUrl}}\">here</a> to start the registration process.<br><br>Sincerely,<br><br>{{AppName}} Team</div>";
            //string userInvitationMessageFileName = Maps.GetDeploymentPath("Sql/UserInvitationMessage.txt");
            //FileInfo file = new FileInfo(userInvitationMessageFileName);
            //string desc = file.OpenText().ReadToEnd();
            //return desc;
        }
        private void CopyDefaultPage()
        {
            string destPath = System.Web.HttpContext.Current.Server.MapPath(DefaultPagePath);
            System.IO.DirectoryInfo destDirectoryInfo = new DirectoryInfo(destPath);
            string sourcePath = System.Web.HttpContext.Current.Server.MapPath("/Views/Shared/DefaultPage/");
            System.IO.DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(sourcePath);

            Durados.DataAccess.DataAccessHelper.CopyAll(sourceDirectoryInfo, destDirectoryInfo);


        }

        public void Commit()
        {
            Durados.DataAccess.ConfigAccess.SaveConfigDataset(configDatabase.ConnectionString, Logger);
            Initiate();
            ConfigAccess.Refresh(configDatabase.ConnectionString);
        }


        private void AddNewUserVerification()
        {
            ConfigAccess configAccess = new DataAccess.ConfigAccess();
            string userViewPK = configAccess.GetViewPK(Database.UserViewName, configDatabase.ConnectionString);
            View ruleView = (View)configDatabase.Views["Rule"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", "newUserVerification");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.OnDemand.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.Notify.ToString());
            values.Add("WhereCondition", "true");

            values.Add("NotifySubject", "Wellcome to {{AppName}}");
            values.Add("NotifyMessage", GetUserVerificationMessage());

            values.Add("NotifyTo", "$Username");

            DataRow row = ruleView.Create(values, null, null, null, null, null);
            string rulePK = ruleView.GetPkValue(row);

        }

        public const string EmptyCode = "/* globals\n" +
    "$http - Service for AJAX calls \n" +
    "CONSTS - CONSTS.apiUrl for Backands API URL\n" +
    "Config - Global Configuration\n" +
    "*/\n" +
    "'use strict';\n" +
    "function backandCallback(userInput, dbRow, parameters, userProfile) {\n" +
    "   \n" +
    "   //Example for SSO in OAuth 2.0 standard\n" +
    "   //$http({\"method\":\"POST\",\"url\":\"http://www.mydomain.com/api/token\", \"data\":\"grant_type=password&username=\" + userInput.username + \"&password=\" + userInput.password, \"headers\":{\"Content-Type\":\"application/x-www-form-urlencoded\"}});\n" +
    "   \n" +
    "   //Return results of \"allow\" or \"deny\" to override the Backand auth and provide a denied message\n" +
    "   //Return ignore to ignore this fucntion and use Backand default authentication\n" +
    "   //Return additionalTokenInfo that will be added to backand auth result.\n" +
    "   //You may access this later by using the getUserDetails function of the Backand SDK.\n" +
    "   return {\"result\": \"ignore\", \"message\":\"\", \"additionalTokenInfo\":{}};\n" +
    "}";
        private void AddBackandAuthenticationOverride()
        {
            ConfigAccess configAccess = new DataAccess.ConfigAccess();
            string userViewPK = configAccess.GetViewPK(Database.UserViewName, configDatabase.ConnectionString);
            View ruleView = (View)configDatabase.Views["Rule"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", Database.CustomValidationActionName);
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.OnDemand.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.JavaScript.ToString());
            values.Add("WhereCondition", "true");

            values.Add("Code", EmptyCode);

            DataRow row = ruleView.Create(values, null, null, null, null, null);
            string rulePK = ruleView.GetPkValue(row);

        }
        private void AddSendForgotPassword()
        {
            ConfigAccess configAccess = new DataAccess.ConfigAccess();
            string userViewPK = configAccess.GetViewPK(Database.UserViewName, configDatabase.ConnectionString);
            View ruleView = (View)configDatabase.Views["Rule"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", "requestResetPassword");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.OnDemand.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.Notify.ToString());
            values.Add("WhereCondition", "true");

            values.Add("NotifySubject", "Your password was reset.");
            values.Add("NotifyMessage", "<div>Hi {{FirstName}},<br><br><br>We received a request to reset the password for your account. If you made this request, click <a href='{{ForgotPasswordUrl}}' >here to reset your password.</a><br>If you didn't make this request, please ignore this email.<br><br>Cheers,<br><br>The {{AppName}} Team</Div>");

            values.Add("NotifyTo", "{{Email}}");

            DataRow row = ruleView.Create(values, null, null, null, null, null);
            string rulePK = ruleView.GetPkValue(row);

        }

        private void AddBeforeSocialSignup()
        {
            ConfigAccess configAccess = new DataAccess.ConfigAccess();
            string userViewPK = configAccess.GetViewPK(Database.UserViewName, configDatabase.ConnectionString);
            View ruleView = (View)configDatabase.Views["Rule"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", "beforeSocialSignup");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.OnDemand.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.JavaScript.ToString());
            values.Add("WhereCondition", "false");
            string code = "/* globals\\n\\  $http - service for AJAX calls - $http({method:'GET',url:CONSTS.apiUrl + '/1/objects/yourObject' , headers: {'Authorization':userProfile.token}});\n" +
      "  CONSTS - CONSTS.apiUrl for Backands API URL\n" +
      "*/\n" +
      "\'use strict\';\n" +
      "function backandCallback(userInput, dbRow, parameters, userProfile) {\n" +
      "}";
            values.Add("Code", code);

            DataRow row = ruleView.Create(values, null, null, null, null, null);
            string rulePK = ruleView.GetPkValue(row);

        }



        private void AddUserApproval()
        {
            ConfigAccess configAccess = new DataAccess.ConfigAccess();
            string userViewPK = configAccess.GetViewPK(Database.UserViewName, configDatabase.ConnectionString);
            View ruleView = (View)configDatabase.Views["Rule"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", "userApproval");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.OnDemand.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.Notify.ToString());
            values.Add("WhereCondition", "true");

            values.Add("NotifySubject", "Wellcome to [Product]");
            values.Add("NotifyMessage", GetUserApprovalMessage());

            values.Add("NotifyTo", "$Username");

            DataRow row = ruleView.Create(values, null, null, null, null, null);
            string rulePK = ruleView.GetPkValue(row);

        }

        private void AddNewUserInvitation()
        {
            ConfigAccess configAccess = new DataAccess.ConfigAccess();
            string userViewPK = configAccess.GetViewPK(Database.UserViewName, configDatabase.ConnectionString);
            View ruleView = (View)configDatabase.Views["Rule"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", "User Invitation Email");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.AfterCreateBeforeCommit.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.Notify.ToString());
            values.Add("WhereCondition", "'{{UserRole_Parent}}' <> 'Admin' and '{{sys::role}}' = 'Admin'");
            values.Add("NotifySubject", "Welcome to {{AppName}}");
            values.Add("NotifyMessage", GetUserInvitationMessage());
            values.Add("NotifyTo", "$Username");

            DataRow row = ruleView.Create(values, null, null, null, null, null);
            string rulePK = ruleView.GetPkValue(row);

            View databaseView = (View)configDatabase.Views["Database"];
            values = new Dictionary<string, object>();
            values.Add("AutoCommit", true);
            values.Add("UploadFolder", "/Uploads/" + Maps.Instance.GetCurrentAppId() + "/");
            databaseView.Edit(values, null, null, null, null, null);
        }

        private void AddNewAdminInvitation()
        {
            ConfigAccess configAccess = new DataAccess.ConfigAccess();
            string userViewPK = configAccess.GetViewPK(Database.UserViewName, configDatabase.ConnectionString);
            View ruleView = (View)configDatabase.Views["Rule"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", "Admin Invitation Email");
            values.Add("Rules_Parent", userViewPK);
            values.Add("DataAction", Durados.TriggerDataAction.AfterCreateBeforeCommit.ToString());
            values.Add("WorkflowAction", Durados.WorkflowAction.Notify.ToString());
            values.Add("WhereCondition", "'{{UserRole_Parent}}' = 'Admin' and '{{sys::role}}' = 'Admin'");
            values.Add("NotifySubject", "Welcome to {{AppName}}");
            values.Add("NotifyMessage", GetAdminInvitationMessage());
            values.Add("NotifyTo", "$Username");

            DataRow row = ruleView.Create(values, null, null, null, null, null);
            string rulePK = ruleView.GetPkValue(row);

            View databaseView = (View)configDatabase.Views["Database"];
            values = new Dictionary<string, object>();
            values.Add("AutoCommit", true);
            values.Add("UploadFolder", "/Uploads/" + Maps.Instance.GetCurrentAppId() + "/");
            databaseView.Edit(values, null, null, null, null, null);
        }

        private void AddDemo()
        {
            if (Maps.GetCurrentAppName().StartsWith(Maps.DemoDatabaseName))
            {
                DataAccess.SqlAccess sqlAccess = new SqlAccess();

                string scriptFilename = Maps.GetDeploymentPath("Sql/NorthwindSysAdditional.sql");

                try
                {
                    sqlAccess.RunScriptFile(scriptFilename, systemConnectionString);
                }
                catch (Exception exception)
                {
                    Logger.Log("Map", "Initiate", "AddDemo", exception, 1, "Failed to add to demo sys");
                }
                scriptFilename = Maps.GetDeploymentPath("Sql/NorthwindAdditional.sql");

                try
                {
                    sqlAccess.RunScriptFile(scriptFilename, connectionString);
                }
                catch (Exception exception)
                {
                    Logger.Log("Map", "Initiate", "AddDemo", exception, 1, "Failed to add to demo");
                }
            }
        }

        private void AddCreator()
        {

            View userView = (View)Database.GetUserView();
            Dictionary<string, object> values = new Dictionary<string, object>();
            MapDataSet.v_durados_UserRow userRow = Maps.Instance.GetCreatorUserRow();

            if (Database.GetUserRow(userRow.Username, true) != null)
                return;


            values.Add(Database.UsernameFieldName, userRow.Username);
            if (userView.Fields.ContainsKey("FirstName"))
                values.Add("FirstName", userRow.FirstName);
            if (userView.Fields.ContainsKey("LastName"))
                values.Add("LastName", userRow.LastName);
            if (userView.Fields.ContainsKey("Email"))
                values.Add("Email", userRow.Email);
            if (userView.Fields.ContainsKey("IsApproved"))
                values.Add("IsApproved", true);

            string roleName = userView.GetFieldByColumnNames("Role").Name;
            values.Add(roleName, "Admin");

            foreach (Field field in userView.Fields.Values.Where(f => !f.ExcludeInInsert && f.Name != roleName && f.Name != "ID" && f.FieldType != FieldType.Children))
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

            userView.Create(values);
        }

        private void AddSystemTables(DataSet ds)
        {
            AddSystemBlockTable(ds);
            AddSystemImportTable(ds);
            AddSystemUploadConfigTable(ds);
            AddSystemContentTable(ds);
            AddSystemUserTables(ds);
            //
            AddSystemHistoryTables(ds);
            AddSystemLinkTable(ds);
            //
            AddSystemLogTable(ds);
            AddSystemSchemaTable(ds);

            if (this is DuradosMap && !Maps.PrivateCloud)
            {
                AddSystemPlugInTable(ds);
                AddSystemMailingServiceTable(ds);
                AddSystemMessageBoardTable(ds);
            }
        }

        private void ConfigSystemTables()
        {
            ConfigSystemBlockTable();
            ConfigSystemImportTable();
            ConfigSystemUploadConfigTable();
            ConfigSystemContentTable();
            ConfigSystemUserTable();
            //
            ConfigSystemHistoryTables();
            ConfigSystemLinkTable();
            //
            ConfigSystemSchemaTable();
            ConfigSystemLogTable();
            ConfigSystemRoleTable();
            ConfigSystemPlugInTable();
            if (this is DuradosMap && !Maps.PrivateCloud)
            {
                ConfigSystemMailingServiceTable();
                ConfigSystemMessageBoardTable();
            }
            try
            {
                foreach (View systemView in db.Views.Values.Where(v => v.SystemView))
                {
                    int? workspaceID = ConfigAccess.GetWorkspaceId("Admin", configDatabase.ConnectionString);
                    if (workspaceID.HasValue)
                        systemView.WorkspaceID = workspaceID.Value;
                }
            }
            catch (Exception exception)
            {
                try
                {
                    Logger.Log("Map", "Initiate", "ConfigSystemTables", exception, 1, "");
                }
                catch { }
            }
        }

        private void AddSystemMailingServiceTable(DataSet ds)
        {
            string tableName = "durados_MailingServiceSubscribers";

            if (ds.Tables.Contains(tableName))
                return;

            DataTable table = systemGenerator.CreateTable(tableName, systemConnectionString);

            ds.Tables.Add(table);


            table.ExtendedProperties.Add("system", true);
        }



        private void AddSystemBlockTable(DataSet ds)
        {
            string tableName = "Block";

            if (ds.Tables.Contains(tableName))
                return;

            DataTable table = ds.Tables.Add(tableName);

            table.ExtendedProperties.Add("system", true);

            DataColumn pk = table.Columns.Add("Tag", typeof(string));

            table.Columns.Add("Token", typeof(string));

            table.Columns.Add("DataType", typeof(Durados.DataType));

            table.PrimaryKey = new DataColumn[1] { pk };
        }

        private void AddSystemContentTable(DataSet ds)
        {
            string tableName = "durados_Html";

            if (ds.Tables.Contains(tableName))
                return;

            DataTable table = ds.Tables.Add(tableName);
            table.ExtendedProperties.Add("system", true);

            DataColumn pk = table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Text", typeof(string)).MaxLength = 1073741823;

            table.PrimaryKey = new DataColumn[1] { pk };
        }


        private void AddSystemUserTables(DataSet ds)
        {
            bool multiTenancy = Maps.MultiTenancy;

            if (!multiTenancy)
                return;

            if (this is DuradosMap)
                return;


            string v_durados_User = "v_durados_User";
            string durados_User = "durados_User";
            string durados_UserRole = "durados_UserRole";

            if (ds.Tables.Contains(v_durados_User))
                return;

            //DataTable v_durados_UserTable = Durados.DataAccess.AutoGeneration.Generator.CreateTable(v_durados_User, systemConnectionString);
            DataTable v_durados_UserTable = systemGenerator.CreateTable(v_durados_User, durados_User, systemConnectionString);

            ds.Tables.Add(v_durados_UserTable);
            v_durados_UserTable.PrimaryKey = new DataColumn[1] { v_durados_UserTable.Columns["ID"] };


            //DataTable durados_UserRoleTable = Durados.DataAccess.AutoGeneration.Generator.CreateTable(durados_UserRole, systemConnectionString);
            DataTable durados_UserRoleTable = systemGenerator.CreateTable(durados_UserRole, systemConnectionString);

            ds.Tables.Add(durados_UserRoleTable);

            ds.Relations.Add("UserRole", durados_UserRoleTable.Columns["Name"], v_durados_UserTable.Columns["Role"]);

            durados_UserRoleTable.ExtendedProperties.Add("system", true);
            v_durados_UserTable.ExtendedProperties.Add("system", true);

        }

        private void AddSystemPlugInTable(DataSet ds)
        {
            bool multiTenancy = Maps.MultiTenancy;

            if (!multiTenancy)
                return;

            DataTable durados_PlugInTable = systemGenerator.CreateTable("durados_PlugIn", systemConnectionString);
            ds.Tables.Add(durados_PlugInTable);
            durados_PlugInTable.PrimaryKey = new DataColumn[1] { durados_PlugInTable.Columns["Id"] };

            DataTable durados_SampleAppTable = systemGenerator.CreateTable("durados_SampleApp", systemConnectionString);
            ds.Tables.Add(durados_SampleAppTable);
            durados_SampleAppTable.PrimaryKey = new DataColumn[1] { durados_SampleAppTable.Columns["Id"] };

            DataTable durados_PlugInInstanceTable = systemGenerator.CreateTable("durados_PlugInInstance", systemConnectionString);
            ds.Tables.Add(durados_PlugInInstanceTable);
            durados_PlugInInstanceTable.PrimaryKey = new DataColumn[1] { durados_PlugInInstanceTable.Columns["Id"] };

            DataColumn plugInIdColumn = durados_PlugInTable.PrimaryKey[0];
            ds.Relations.Add("SampleAppPlugIn", plugInIdColumn, durados_SampleAppTable.Columns["PlugInId"]);
            ds.Relations.Add("InstancePlugIn", plugInIdColumn, durados_PlugInInstanceTable.Columns["PlugInId"]);

            if (ds.Tables.Contains("durados_App"))
            {
                DataTable durados_AppTable = ds.Tables["durados_App"];

                if (durados_AppTable.PrimaryKey.Length == 1)
                {
                    DataColumn appIdColumn = durados_AppTable.PrimaryKey[0];

                    ds.Relations.Add("SampleAppApp", appIdColumn, durados_SampleAppTable.Columns["AppId"]);
                    ds.Relations.Add("InstanceApp", appIdColumn, durados_PlugInInstanceTable.Columns["AppId"]);

                }

            }


            durados_PlugInTable.ExtendedProperties.Add("system", true);
            durados_SampleAppTable.ExtendedProperties.Add("system", true);
            durados_PlugInInstanceTable.ExtendedProperties.Add("system", true);


        }

        private void AddSystemHistoryTables(DataSet ds)
        {
            //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[project.ConnectionStringKey].ConnectionString;
            string durados_ChangeHistory = "durados_ChangeHistory";
            string durados_ChangeHistoryField = "durados_ChangeHistoryField";
            string durados_Action = "durados_Action";
            string durados_v_ChangeHistory = "durados_v_ChangeHistory";

            if (ds.Tables.Contains(durados_ChangeHistory))
                return;

            DataTable durados_ChangeHistoryTable = systemGenerator.CreateTable(durados_ChangeHistory, systemConnectionString);
            ds.Tables.Add(durados_ChangeHistoryTable);

            DataTable durados_ChangeHistoryFieldTable = systemGenerator.CreateTable(durados_ChangeHistoryField, systemConnectionString);
            ds.Tables.Add(durados_ChangeHistoryFieldTable);

            DataTable durados_ActionTable = systemGenerator.CreateTable(durados_Action, systemConnectionString);
            ds.Tables.Add(durados_ActionTable);

            DataTable durados_v_ChangeHistoryTable = systemGenerator.CreateTable(durados_v_ChangeHistory, systemConnectionString);
            ds.Tables.Add(durados_v_ChangeHistoryTable);
            durados_v_ChangeHistoryTable.PrimaryKey = new DataColumn[1] { durados_v_ChangeHistoryTable.Columns["AutoId"] };

            ds.Relations.Add("ChangedFields", durados_ChangeHistoryTable.Columns["Id"], durados_ChangeHistoryFieldTable.Columns["ChangeHistoryId"]);
            ds.Relations.Add("Action", durados_ActionTable.Columns["Id"], durados_ChangeHistoryTable.Columns["ActionId"]);
            ds.Relations.Add("ActionHistory", durados_ActionTable.Columns["Id"], durados_v_ChangeHistoryTable.Columns["ActionId"]);

            DataTable userTable = null;
            if (ds.Tables.Contains("v_User"))
            {
                userTable = ds.Tables["v_User"];
            }
            else if (ds.Tables.Contains("User"))
            {
                userTable = ds.Tables["User"];
            }
            else if (ds.Tables.Contains("v_durados_User"))
            {
                userTable = ds.Tables["v_durados_User"];
            }
            if (userTable != null)
            {
                if (userTable.Columns.Contains("ID"))
                {
                    ds.Relations.Add(userTable.TableName, userTable.Columns["ID"], durados_ChangeHistoryTable.Columns["UpdateUserId"]);
                    ds.Relations.Add("HistoryUsers", userTable.Columns["ID"], durados_v_ChangeHistoryTable.Columns["UpdateUserId"]);

                }
            }

            durados_v_ChangeHistoryTable.ExtendedProperties.Add("system", true);
            durados_ChangeHistoryTable.ExtendedProperties.Add("system", true);
            durados_ActionTable.ExtendedProperties.Add("system", true);
            durados_ChangeHistoryFieldTable.ExtendedProperties.Add("system", true);
        }

        private void AddSystemImportTable(DataSet ds)
        {
            string tableName = "durados_Import";

            if (ds.Tables.Contains(tableName))
                return;

            DataTable table = ds.Tables.Add(tableName);

            DataColumn pk = table.Columns.Add("ID", typeof(int));
            table.Columns.Add("SourceType", typeof(int));
            table.Columns.Add("FileName", typeof(string));
            table.Columns.Add("SheetName", typeof(string));
            table.Columns.Add("WriteErrors", typeof(bool));
            table.Columns.Add("RollBackOnError", typeof(bool));

            table.Columns.Add("ImportMode", typeof(int));

            table.PrimaryKey = new DataColumn[1] { pk };
            table.ExtendedProperties.Add("system", true);

        }

        private void AddSystemUploadConfigTable(DataSet ds)
        {
            string tableName = "durados_UploadConfig";

            if (ds.Tables.Contains(tableName))
                return;

            DataTable table = ds.Tables.Add(tableName);

            DataColumn pk = table.Columns.Add("ID", typeof(int));
            table.Columns.Add("FileName", typeof(string));

            table.PrimaryKey = new DataColumn[1] { pk };
            table.ExtendedProperties.Add("system", true);

        }

        private void AddSystemLogTable(DataSet ds)
        {
            string tableName = "Durados_Log";
            bool multiTenancy = Maps.MultiTenancy;

            if (ds.Tables.Contains(tableName))
                return;

            if (!multiTenancy)
                return;


            DataTable table = systemGenerator.CreateTable(tableName, systemConnectionString);
            ds.Tables.Add(table);
            table.PrimaryKey = new DataColumn[1] { table.Columns["ID"] };
            table.ExtendedProperties.Add("system", true);

        }

        private void AddSystemMessageBoardTable(DataSet ds)
        {
            string tableName = "durados_v_MessageBoard";

            if (ds.Tables.Contains(tableName))
                return;

            //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[project.ConnectionStringKey].ConnectionString;


            DataTable durados_v_MessageBoard = systemGenerator.CreateTable(tableName, systemConnectionString);

            ds.Tables.Add(durados_v_MessageBoard);


            tableName = "durados_MessageStatus";

            DataTable durados_MessageStatus = systemGenerator.CreateTable(tableName, systemConnectionString);

            ds.Tables.Add(durados_MessageStatus);



            durados_v_MessageBoard.PrimaryKey = new DataColumn[1] { durados_v_MessageBoard.Columns["Id"] };
            durados_v_MessageBoard.PrimaryKey[0].AutoIncrement = true;

            ds.Relations.Add("fk_MessageBoard_MessageStatus", durados_v_MessageBoard.Columns["Id"], durados_MessageStatus.Columns["MessageId"]);

            DataTable userTable = null;


            if (ds.Tables.Contains("v_durados_User"))
            {
                userTable = ds.Tables["v_durados_User"];
            }
            else if (ds.Tables.Contains("v_User"))
            {
                userTable = ds.Tables["v_User"];
            }
            else if (ds.Tables.Contains("User"))
            {
                userTable = ds.Tables["User"];
            }
            if (userTable != null)
            {
                if (userTable.Columns.Contains("ID"))
                {
                    ds.Relations.Add("fk_user_MessageBoard_OriginatedUser", userTable.Columns["ID"], durados_v_MessageBoard.Columns["OriginatedUserId"]);
                    ds.Relations.Add("fk_user_MessageStatus", userTable.Columns["ID"], durados_MessageStatus.Columns["UserId"]);

                }
            }

            durados_v_MessageBoard.ExtendedProperties.Add("system", true);
            durados_MessageStatus.ExtendedProperties.Add("system", true);

        }

        private void AddSystemSchemaTable(DataSet ds)
        {
            string tableName = "durados_Schema";

            if (ds.Tables.Contains(tableName))
                return;

            DataTable table = ds.Tables.Add(tableName);

            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("EntityType", typeof(string));
            table.Columns.Add("Schema", typeof(string));
            table.Columns.Add("EditableTableName", typeof(string));

            table.PrimaryKey = new DataColumn[1] { table.Columns[0] };
            table.ExtendedProperties.Add("system", true);
        }

        private void AddSystemLinkTable(DataSet ds)
        {
            string tableName = "durados_Link";

            if (ds.Tables.Contains(tableName))
                return;

            //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[project.ConnectionStringKey].ConnectionString;

            //DataTable table = ds.Tables.Add(tableName);

            //table.PrimaryKey = new DataColumn[1] { table.Columns["Id"] };

            DataTable durados_LinkTable = systemGenerator.CreateTable(tableName, systemConnectionString);

            ds.Tables.Add(durados_LinkTable);
            /*
            DataTable userTable = null;
            if (ds.Tables.Contains("v_User"))
            {
                userTable = ds.Tables["v_User"];
            }
            else if (ds.Tables.Contains("User"))
            {
                userTable = ds.Tables["User"];
            }
            if (userTable != null)
            {
                if (userTable.Columns.Contains("ID"))
                {
                    //ds.Relations.Add("UsersLinks", userTable.Columns["ID"], durados_LinkTable.Columns["UserId"]);

                }
            }
             */
            DataColumn messagesColumn = new DataColumn("GrobootMessages");
            durados_LinkTable.Columns.Add(messagesColumn);

            durados_LinkTable.ExtendedProperties.Add("system", true);


        }

        private void ConfigSystemImportTable()
        {
            View importView = (View)db.Views["durados_Import"];
            foreach (Field field in importView.Fields.Values)
            {
                field.Precedent = true;
                field.AllowSelectRoles = "everyone";
                field.AllowCreateRoles = "everyone";
                field.AllowEditRoles = "everyone";
                field.DenyCreateRoles = "";
                field.DenyEditRoles = "";
                field.DenySelectRoles = "";

            }
            db.Views["durados_Import"].SystemView = true;
            db.Views["durados_Import"].HideInMenu = true;
            db.Views["durados_Import"].Fields["ID"].HideInCreate = true; ;
            //((Durados.Web.Mvc.ColumnField)db.Views["durados_Import"].Fields["FileName"]).Required = true;
            //((Durados.Web.Mvc.ColumnField)db.Views["durados_Import"].Fields["FileName"]).Validation.RequiredMessage = "Please enter the sheet name";

            Durados.Web.Mvc.ColumnField fileNameField = ((Durados.Web.Mvc.ColumnField)db.Views["durados_Import"].Fields["FileName"]);
            fileNameField.Upload = new Upload() { Override = true, Title = "Import Excel", UploadFileType = UploadFileType.Other, UploadStorageType = UploadStorageType.File, UploadVirtualPath = Database.UploadFolder };
            fileNameField.PreLabel = GetImportDialogPreLableHtml();
            //To view the correct format of the Excel file please export the table.";

            Durados.Web.Mvc.ColumnField rollBackOnErrorField = ((Durados.Web.Mvc.ColumnField)db.Views["durados_Import"].Fields["RollBackOnError"]);
            rollBackOnErrorField.DefaultValue = true;
            Durados.Web.Mvc.ColumnField writeErrorsField = ((Durados.Web.Mvc.ColumnField)db.Views["durados_Import"].Fields["WriteErrors"]);
            writeErrorsField.DefaultValue = true;
            Durados.Web.Mvc.ColumnField sheetNameField = ((Durados.Web.Mvc.ColumnField)db.Views["durados_Import"].Fields["SheetName"]);

            Durados.Web.Mvc.ColumnField importMode = ((Durados.Web.Mvc.ColumnField)db.Views["durados_Import"].Fields["ImportMode"]);
            importMode.MultiValueAdditionals = "1,Add new lines,2,Update existing lines,3,Both,4,Add and Ignore Duplicates";
            importMode.TextHtmlControlType = TextHtmlControlType.DropDown;
            importMode.Description = "Insert: Only&nbsp;insert&nbsp;new&nbsp;rows. Aborts&nbsp;the&nbsp;Import&nbsp;if&nbsp;a&nbsp;row&nbsp;already&nbsp;exists.&#13;";
            importMode.Description += "Update: Only&nbsp;Update&nbsp;an&nbsp;existing&nbsp;row. Aborts&nbsp;the&nbsp;Import&nbsp;if&nbsp;a&nbsp;row&nbsp;was&nbsp;not&nbsp;found.&#13;";
            importMode.Description += "Insert&nbsp;and&nbsp;Update: If&nbsp;a&nbsp;row&nbsp;was&nbsp;found&nbsp;then&nbsp;Update&nbsp;Mode, otherwise&nbsp;Insert&nbsp;Mode.";
            importMode.DefaultValue = 4;

            Durados.Web.Mvc.ColumnField sourceType = ((Durados.Web.Mvc.ColumnField)db.Views["durados_Import"].Fields["SourceType"]);
            sourceType.MultiValueAdditionals = "1,Excel"; // "1,Excel,2,Google Docs";
            sourceType.Excluded = true;
            sourceType.TextHtmlControlType = TextHtmlControlType.DropDown;
            sourceType.Precedent = true;
            sourceType.AllowCreateRoles = "Developer";
            //sourceType.DenyCreateRoles = "View Owner,Admin";
            importMode.Precedent = true;
            importMode.AllowCreateRoles = "Developer";
            //importMode.DenyCreateRoles = "View Owner,Admin";
            writeErrorsField.Precedent = true;
            writeErrorsField.AllowCreateRoles = "Developer";
            //writeErrorsField.DenyCreateRoles = "View Owner,Admin";
            rollBackOnErrorField.Precedent = true;
            rollBackOnErrorField.AllowCreateRoles = "Developer";
            //rollBackOnErrorField.DenyCreateRoles = "View Owner,Admin";
            sheetNameField.Precedent = true;
            sheetNameField.AllowCreateRoles = "Developer";
            //sheetNameField.DenyCreateRoles = "View Owner,Admin";

            sourceType.DefaultValue = 1;
        }

        private string GetImportDialogPreLableHtml()
        {
            Durados.Localization.ILocalizer local = this.Database.Localizer;
            StringBuilder sb = new StringBuilder();
            sb.Append(local.Translate("Please upload an Excel file with the data that you wish to add to the table."));
            sb.Append("<br><br><div  name='ImportAddTemplate'>");
            sb.AppendFormat("{0} <a format='None' class='url blueUrl' href=''><span>{1}</span></a> {2}.</div>", local.Translate("Use this"), local.Translate("Template"), local.Translate("to add new lines"));
            sb.Append("<div name='ImportEditTemplate'><br>");
            sb.AppendFormat("{0} <a format='None' class='url blueUrl' href=''><span>{1}</span></a> {2}.</div>", local.Translate("Use this"), local.Translate("Template"), local.Translate("to edit existing lines"));
            sb.AppendFormat("<div name='importTypeDiv'>{0}<br>", local.Translate("Choose import method:"));
            sb.AppendFormat("<input type='radio' name='importType' class='radioClass' value='Replace'>{0}<br>", local.Translate("Replace"));
            sb.AppendFormat("<input type='radio' name='importType' class='radioClass' value='Merg' checked='checked'>{0}<br>", local.Translate("Merg"));

            return sb.ToString();
        }

        private void ConfigSystemUploadConfigTable()
        {
            View uploadConfigView = (View)db.Views["durados_UploadConfig"];
            uploadConfigView.SystemView = true;
            uploadConfigView.HideInMenu = true;
            uploadConfigView.Fields["ID"].HideInCreate = true; ;
            Durados.Web.Mvc.ColumnField uploadField = (Durados.Web.Mvc.ColumnField)uploadConfigView.Fields["FileName"];

            uploadField.Upload = new Upload() { Override = true, Title = "Upload Configuration", UploadFileType = UploadFileType.Other, UploadStorageType = UploadStorageType.File, UploadVirtualPath = "/Uploads/" + Id ?? string.Empty + "/" };
            uploadField.Required = true;

            string downloadConfigUrl = "/Admin/DownloadConfig";
            uploadField.PreLabel = "<div class='upload-config-w'><ul><li><span class='upload-config-w1'>This option allows you to update the configuration of you production app from your test app.<br>The database schema of both apps must be identical.</span></li><li><span class='upload-config-w2'>Please upload the zip file you downloaded from your test app.</span></li><li><span class='upload-config-w3'>Please make a backup of the current configuration. You may download it <a href='" + downloadConfigUrl + "'>here</a></span></li></ul></div>";

        }


        private void ConfigSystemBlockTable()
        {
            //((Durados.Web.Mvc.View)db.Views["Block"]).Controller = "Block";
            db.Views["Block"].SystemView = true;
        }

        private void ConfigSystemContentTable()
        {
            //db.Views["durados_Import"].HideInMenu = true;
            View contentView = (View)db.Views["durados_Html"];
            //contentView.AllowCreate = false;
            //contentView.AllowDelete = false;
            //contentView.AllowDuplicate = false;

            contentView.Precedent = true;
            contentView.AllowCreateRoles = "Developer";
            contentView.AllowDeleteRoles = "Developer";
            contentView.AllowEditRoles = "Developer,Admin";

            contentView.DataDisplayType = DataDisplayType.Preview;
            contentView.FilterType = FilterType.Group;
            contentView.SortingType = SortingType.Group;
            contentView.DashboardWidth = "300";

            contentView.DisplayName = "Content";
            ColumnField nameField = contentView.Fields["Name"] as ColumnField;
            nameField.ExcludeInUpdate = true;
            nameField.HideInEdit = true;

            //((Durados.Web.Mvc.ColumnField)db.Views["durados_Import"].Fields["FileName"]).Upload = new Upload();
            ColumnField textField = contentView.Fields["Text"] as ColumnField;
            textField.Rich = true;
            textField.HideInFilter = true;
            textField.Sortable = false;
            textField.Upload = new Upload() { FileAllowedTypes = "jpg,png,gif", FileMaxSize = 100, Override = false, Title = "UploadContent", UploadFileType = Mvc.UploadFileType.Image, UploadStorageType = Mvc.UploadStorageType.File, UploadVirtualPath = "/Uploads/" + Id ?? string.Empty + "/" };
            textField.CssClass = "wtextarealarge";
            db.Views["durados_Html"].SystemView = true;
        }

        private void ConfigSystemMailingServiceTable()
        {
            Durados.View mailServiceView = db.Views["durados_MailingServiceSubscribers"];
            mailServiceView.SystemView = true;
            mailServiceView.DisplayName = "Mailing Service Subscribers";

            mailServiceView.DefaultSort = "";
            ////db.Views["durados_Link"].PermanentFilter = "LinkType=0";

            //mailServiceView.Fields["Id"].HideInTable = true;

            //if (mailServiceView.Fields.ContainsKey("Url"))
            //{
            //    mailServiceView.Fields["Url"].DisableInEdit = true;
            //    //db.Views["durados_Link"].Fields["Url"].HideInEdit = true;                
            //}
            //if (mailServiceView.Fields.ContainsKey("CreationDate"))
            //    mailServiceView.Fields["CreationDate"].HideInEdit = true;



        }

        private void ConfigSystemHistoryTables()
        {
            try
            {
                db.Views["durados_ChangeHistory"].DisplayName = "History";

                db.Views["durados_ChangeHistoryField"].DisplayName = "Changed Fields";
                db.Views["durados_Action"].DisplayName = "Database Actions";
                db.Views["durados_v_ChangeHistory"].DisplayName = "History Log";

                db.Views["durados_ChangeHistory"].SystemView = true;
                db.Views["durados_ChangeHistoryField"].SystemView = true;
                db.Views["durados_Action"].SystemView = true;
                db.Views["durados_v_ChangeHistory"].SystemView = true;


                db.Views["durados_ChangeHistory"].Send = false;
                db.Views["durados_v_ChangeHistory"].Send = false;

                db.Views["durados_ChangeHistory"].Precedent = true;
                db.Views["durados_ChangeHistoryField"].Precedent = true;
                db.Views["durados_v_ChangeHistory"].Precedent = true;
                db.Views["durados_ChangeHistory"].AllowSelectRoles = "everyone";
                db.Views["durados_ChangeHistoryField"].AllowSelectRoles = "everyone";
                db.Views["durados_v_ChangeHistory"].AllowSelectRoles = "everyone";

                //db.Views["durados_ChangeHistory"].DisplayType = DisplayType.Report;
                db.Views["durados_v_ChangeHistory"].DisplayType = DisplayType.Table;
                db.Views["durados_v_ChangeHistory"].AllowCreate = false;
                db.Views["durados_v_ChangeHistory"].AllowDelete = false;
                db.Views["durados_v_ChangeHistory"].AllowDuplicate = false;
                db.Views["durados_v_ChangeHistory"].AllowEdit = false;
                db.Views["durados_v_ChangeHistory"].HideSearch = true;

                ((Durados.Web.Mvc.View)db.Views["durados_v_ChangeHistory"]).Controller = "History";
                db.Views["durados_v_ChangeHistory"].DefaultSort = "UpdateDate Desc";

                db.Views["durados_ChangeHistory"].Fields["ChangedFields_Children"].HideInTable = false;
                db.Views["durados_ChangeHistory"].Fields["id"].HideInTable = true;
                db.Views["durados_v_ChangeHistory"].Fields["Id"].HideInTable = true;
                db.Views["durados_v_ChangeHistory"].Fields["ViewName"].HideInTable = false;
                db.Views["durados_v_ChangeHistory"].Fields["AutoId"].HideInTable = true;
                db.Views["durados_v_ChangeHistory"].Fields["PK"].HideInTable = false;
                db.Views["durados_v_ChangeHistory"].Fields["ChangeHistoryId"].HideInTable = true;
                db.Views["durados_v_ChangeHistory"].Fields["ColumnNames"].HideInTable = true;
                db.Views["durados_v_ChangeHistory"].Fields["Comments"].HideInTable = false;
                db.Views["durados_v_ChangeHistory"].Fields["PK"].DisplayName = "Name";
                db.Views["durados_v_ChangeHistory"].Fields["FieldName"].DisplayName = "Field";
                db.Views["durados_v_ChangeHistory"].Fields["UpdateDate"].DisplayName = "Date";
                db.Views["durados_v_ChangeHistory"].Fields["OldValue"].DisplayName = "Old";
                db.Views["durados_v_ChangeHistory"].Fields["NewValue"].DisplayName = "New";

                ((ColumnField)db.Views["durados_v_ChangeHistory"].Fields["Admin"]).DataColumn.DataType = typeof(Int32);
                (db.Views["durados_v_ChangeHistory"].Fields["Admin"] as ColumnField).DataType = DataType.Numeric;
                db.Views["durados_v_ChangeHistory"].Fields["ActionHistory_Parent"].DisplayName = "Action";
                ((ParentField)db.Views["durados_v_ChangeHistory"].Fields["ActionHistory_Parent"]).NoHyperlink = true;
                ((ParentField)db.Views["durados_v_ChangeHistory"].Fields["ActionHistory_Parent"]).JsonName = "Action";

                ((ParentField)db.Views["durados_v_ChangeHistory"].Fields["ActionHistory_Parent"]).ParentHtmlControlType = ParentHtmlControlType.DropDown;
                if (db.Views["durados_v_ChangeHistory"].Fields.ContainsKey("HistoryUsers_Parent"))
                {
                    ((ParentField)db.Views["durados_v_ChangeHistory"].Fields["HistoryUsers_Parent"]).NoHyperlink = true;
                    ((ParentField)db.Views["durados_v_ChangeHistory"].Fields["HistoryUsers_Parent"]).JsonName = "Username";
                }
                db.Views["durados_v_ChangeHistory"].Fields["FieldName"].HideInFilter = true;

                ((ColumnField)db.Views["durados_v_ChangeHistory"].Fields["UpdateDate"]).Format = "MMM dd, yyyy hh:mm:ss";
                ((ColumnField)db.Views["durados_v_ChangeHistory"].Fields["FieldName"]).TextHtmlControlType = TextHtmlControlType.Text;
                ((ColumnField)db.Views["durados_v_ChangeHistory"].Fields["FieldName"]).Dialog = false;
                db.Views["durados_v_ChangeHistory"].UseLikeInFilter = false;

                Field userField = db.Views["durados_v_ChangeHistory"].GetFieldByColumnNames("UpdateUserId");
                if (userField != null)
                    userField.DisplayName = "Username";

            }
            catch { }

        }

        private void ConfigSystemMessageBoardTable()
        {
            db.Views["durados_v_MessageBoard"].SystemView = true;
            View durados_v_MessageBoardView = (View)db.Views["durados_v_MessageBoard"];
            durados_v_MessageBoardView.Controller = "MessageBoard";
            durados_v_MessageBoardView.DisplayName = "My Inbox";
            durados_v_MessageBoardView.DefaultSort = "CreatedDate Desc";
            durados_v_MessageBoardView.PermanentFilter = "  Deleted <> 1 and (UserId is null or UserId = " + Database.UserPlaceHolder + ") ";
            durados_v_MessageBoardView.EditableTableName = "durados_MessageStatus";
            durados_v_MessageBoardView.AllowCreate = false;
            durados_v_MessageBoardView.AllowDuplicate = false;
            durados_v_MessageBoardView.SystemView = true;
            durados_v_MessageBoardView.AllowDelete = true;
            durados_v_MessageBoardView.RowColorColumnName = "";
            durados_v_MessageBoardView.GridEditable = true;
            durados_v_MessageBoardView.GridEditableEnabled = true;
            durados_v_MessageBoardView.Precedent = true;
            durados_v_MessageBoardView.AllowEditRoles = "everyone";

            durados_v_MessageBoardView.Fields["Id"].HideInTable = true;
            durados_v_MessageBoardView.Fields["PK"].HideInTable = true;
            durados_v_MessageBoardView.Fields["Message"].HideInTable = false;
            durados_v_MessageBoardView.Fields["Message"].ExcludeInInsert = true;
            durados_v_MessageBoardView.Fields["Message"].ExcludeInUpdate = true;
            durados_v_MessageBoardView.Fields["Subject"].ExcludeInInsert = true;
            durados_v_MessageBoardView.Fields["Subject"].ExcludeInUpdate = true;

            durados_v_MessageBoardView.Fields["ViewLink"].ExcludeInUpdate = true;
            durados_v_MessageBoardView.Fields["CreatedDate"].ExcludeInUpdate = true;


            durados_v_MessageBoardView.Fields["RowDisplayName"].HideInTable = true;
            durados_v_MessageBoardView.Fields["ViewName"].HideInTable = true;
            durados_v_MessageBoardView.Fields["ViewDisplayName"].HideInTable = true;
            durados_v_MessageBoardView.Fields["Css"].HideInTable = true;
            durados_v_MessageBoardView.Fields["Id"].HideInEdit = true;
            durados_v_MessageBoardView.Fields["PK"].HideInEdit = true;
            durados_v_MessageBoardView.Fields["Message"].HideInEdit = false;
            durados_v_MessageBoardView.Fields["RowDisplayName"].HideInEdit = true;
            durados_v_MessageBoardView.Fields["RowLink"].HideInFilter = true;
            durados_v_MessageBoardView.Fields["RowLink"].HideInTable = true;
            durados_v_MessageBoardView.Fields["RowLink"].HideInEdit = true;
            durados_v_MessageBoardView.Fields["ViewName"].HideInEdit = true;
            durados_v_MessageBoardView.Fields["ViewLink"].HideInFilter = true;
            durados_v_MessageBoardView.Fields["ViewDisplayName"].HideInEdit = true;
            durados_v_MessageBoardView.Fields["Css"].HideInEdit = true;
            ((ColumnField)durados_v_MessageBoardView.Fields["Message"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            //((ColumnField)durados_v_MessageBoardView.Fields["RowLink"]).DisplayName = "Message";
            //((ColumnField)durados_v_MessageBoardView.Fields["RowLink"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            ((ColumnField)durados_v_MessageBoardView.Fields["RowLink"]).DisableInEdit = false;
            ((ColumnField)durados_v_MessageBoardView.Fields["ViewLink"]).TextHtmlControlType = TextHtmlControlType.Url;
            ((ColumnField)durados_v_MessageBoardView.Fields["ViewLink"]).DisplayName = "Hyperlink";
            durados_v_MessageBoardView.Fields["UserId"].ExcludeInInsert = true;
            durados_v_MessageBoardView.Fields["UserId"].HideInTable = true;
            durados_v_MessageBoardView.Fields["UserId"].HideInEdit = true;
            durados_v_MessageBoardView.Fields["CreatedDate"].ExcludeInInsert = true;
            if (durados_v_MessageBoardView.Fields.ContainsKey("fk_user_MessageBoard_OriginatedUser_Parent"))
            {
                durados_v_MessageBoardView.Fields["fk_user_MessageBoard_OriginatedUser_Parent"].DisplayName = "Originator";
                durados_v_MessageBoardView.Fields["fk_user_MessageBoard_OriginatedUser_Parent"].ExcludeInUpdate = true;
                durados_v_MessageBoardView.Fields["fk_user_MessageBoard_OriginatedUser_Parent"].Order = 990;
            }

            foreach (Field field in durados_v_MessageBoardView.Fields.Values)
            {
                field.DisableInEdit = true;
            }

            durados_v_MessageBoardView.Fields["Deleted"].HideInTable = true;
            durados_v_MessageBoardView.Fields["Deleted"].HideInEdit = true;
            durados_v_MessageBoardView.Fields["Reviewed"].HideInTable = true;
            durados_v_MessageBoardView.Fields["Reviewed"].DisableInEdit = false;
            durados_v_MessageBoardView.Fields["Important"].DisableInEdit = false;
            if (durados_v_MessageBoardView.Fields.ContainsKey("ActionRequired"))
            {
                durados_v_MessageBoardView.Fields["ActionRequired"].DisableInEdit = false;
                durados_v_MessageBoardView.Fields["ActionRequired"].ExcludeInInsert = true;
            }

            durados_v_MessageBoardView.Fields["Deleted"].ExcludeInInsert = true;
            durados_v_MessageBoardView.Fields["Reviewed"].ExcludeInInsert = true;
            durados_v_MessageBoardView.Fields["Important"].ExcludeInInsert = true;


        }

        private void ConfigSystemSchemaTable()
        {

            db.Views["durados_Schema"].SystemView = true;
            db.Views["durados_Schema"].DisplayName = "Database Entities";
            db.Views["durados_Schema"].DefaultSort = "EntityType Asc, Name Asc";
            db.Views["durados_Schema"].AllowCreate = false;
            db.Views["durados_Schema"].AllowEdit = true;
            db.Views["durados_Schema"].AllowDelete = false;
            db.Views["durados_Schema"].AllowDuplicate = false;
            ((View)db.Views["durados_Schema"]).ExportToCsv = false;
            db.Views["durados_Schema"].GridEditable = true;
            db.Views["durados_Schema"].HidePager = true;
            db.Views["durados_Schema"].GridEditableEnabled = true;
            ((View)db.Views["durados_Schema"]).Controller = "Admin";
            ((View)db.Views["durados_Schema"]).SaveHistory = false;
            ((View)db.Views["durados_Schema"]).Print = false;
            ((View)db.Views["durados_Schema"]).Send = false;
            ((View)db.Views["durados_Schema"]).MultiSelect = true;
            ((View)db.Views["durados_Schema"]).HideSearch = true;

            db.Views["durados_Schema"].Fields["EntityType"].DisableInEdit = true;
            db.Views["durados_Schema"].Fields["Name"].DisableInEdit = true;
            db.Views["durados_Schema"].Fields["Schema"].HideInEdit = true;
            db.Views["durados_Schema"].Fields["Schema"].ExcludeInUpdate = true;
            db.Views["durados_Schema"].Fields["Schema"].HideInTable = true;
            db.Views["durados_Schema"].Fields["EntityType"].Sortable = false;
            db.Views["durados_Schema"].Fields["Name"].Sortable = false;
            db.Views["durados_Schema"].Fields["Schema"].Sortable = false;


            ColumnField editableTableName = (ColumnField)db.Views["durados_Schema"].Fields["EditableTableName"];

            editableTableName.Sortable = false;

            editableTableName.TextHtmlControlType = TextHtmlControlType.Autocomplete;

            editableTableName.AutocompleteConnectionString = connectionString;
            editableTableName.AutocompleteSql = GetSqlSchema().GetTableNamesSelectStatementWithFilter();
            editableTableName.AutocompleteColumn = "Name";
            editableTableName.DisableInEdit = false;
            editableTableName.ExcludeInUpdate = false;
            editableTableName.DisableInCreate = true;
            editableTableName.ExcludeInInsert = true;
            editableTableName.AutocompleteFilter = false;
        }

        private SqlSchema GetSqlSchema()
        {
            if (SqlProduct == SqlProduct.MySql)
                return new MySqlSchema();
            return new SqlSchema();
        }

        private void ConfigSystemLinkTable()
        {

            db.Views["durados_Link"].SystemView = true;
            db.Views["durados_Link"].DisplayName = "My Stuff";

            db.Views["durados_Link"].DefaultSort = "Ordinal Asc, Id Desc";
            //db.Views["durados_Link"].PermanentFilter = "LinkType=0";

            db.Views["durados_Link"].Fields["Id"].HideInTable = true;
            db.Views["durados_Link"].Fields["UserId"].HideInTable = true;
            db.Views["durados_Link"].Fields["LinkType"].HideInTable = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("Guid"))
                db.Views["durados_Link"].Fields["Guid"].HideInTable = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("Filter"))
                db.Views["durados_Link"].Fields["Filter"].HideInTable = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("SortColumn"))
                db.Views["durados_Link"].Fields["SortColumn"].HideInTable = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("SortDirection"))
                db.Views["durados_Link"].Fields["SortDirection"].HideInTable = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("PageNo"))
                db.Views["durados_Link"].Fields["PageNo"].HideInTable = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("PageSize"))
                db.Views["durados_Link"].Fields["PageSize"].HideInTable = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("FolderId"))
                db.Views["durados_Link"].Fields["FolderId"].HideInTable = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("CreationDate"))
                db.Views["durados_Link"].Fields["CreationDate"].ExcludeInInsert = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("ControllerName"))
                db.Views["durados_Link"].Fields["ControllerName"].HideInTable = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("ControllerName"))
                db.Views["durados_Link"].Fields["ControllerName"].HideInEdit = true;

            db.Views["durados_Link"].Fields["Id"].HideInEdit = true;
            db.Views["durados_Link"].Fields["UserId"].HideInEdit = true;
            db.Views["durados_Link"].Fields["LinkType"].HideInEdit = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("Guid"))
                db.Views["durados_Link"].Fields["Guid"].HideInEdit = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("Filter"))
                db.Views["durados_Link"].Fields["Filter"].HideInEdit = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("SortColumn"))
                db.Views["durados_Link"].Fields["SortColumn"].HideInEdit = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("SortDirection"))
                db.Views["durados_Link"].Fields["SortDirection"].HideInEdit = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("PageNo"))
                db.Views["durados_Link"].Fields["PageNo"].HideInEdit = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("PageSize"))
                db.Views["durados_Link"].Fields["PageSize"].HideInEdit = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("FolderId"))
                db.Views["durados_Link"].Fields["FolderId"].HideInEdit = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("CreationDate"))
                db.Views["durados_Link"].Fields["CreationDate"].HideInEdit = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("ViewName"))
                db.Views["durados_Link"].Fields["ViewName"].HideInEdit = true;
            if (db.Views["durados_Link"].Fields.ContainsKey("Url"))
            {
                db.Views["durados_Link"].Fields["Url"].DisableInEdit = true;
                //db.Views["durados_Link"].Fields["Url"].HideInEdit = true;                
            }
            if (db.Views["durados_Link"].Fields.ContainsKey("CreationDate"))
                db.Views["durados_Link"].Fields["CreationDate"].HideInEdit = true;


            if (db.Views["durados_Link"].Fields.ContainsKey("GrobootMessages"))
            {

                ColumnField grobootMessagesColumn = (ColumnField)db.Views["durados_Link"].Fields["GrobootMessages"];
                grobootMessagesColumn.IsCalculated = true;
                grobootMessagesColumn.Formula = "'GrobootMessages'";
                grobootMessagesColumn.AllowEditRoles = "Developer,Admin";
                grobootMessagesColumn.AllowCreateRoles = "Developer,Admin";
                grobootMessagesColumn.AllowSelectRoles = "Developer,Admin";
                grobootMessagesColumn.TextHtmlControlType = TextHtmlControlType.CheckList;
                grobootMessagesColumn.HideInTable = true;
                grobootMessagesColumn.ExcludeInInsert = true;
                grobootMessagesColumn.ExcludeInUpdate = true;
                grobootMessagesColumn.Precedent = true;
                grobootMessagesColumn.DisplayName = "PushApps Notifications";

                ((ColumnField)db.Views["durados_Link"].Fields["GrobootMessages"]).Excluded = true;
                if (!string.IsNullOrEmpty(db.GrobootNotificationAccessKey))
                {
                    ((ColumnField)db.Views["durados_Link"].Fields["GrobootMessages"]).Excluded = false;

                }

            }
        }

        private void ConfigSystemLogTable()
        {
            View logView = (View)db.Views["Durados_Log"];
            logView.SystemView = true;
            logView.DisplayName = "Monitor Log";

            logView.DefaultSort = "ID Desc";
            logView.AllowCreate = false;
            logView.AllowEdit = false;
            logView.AllowDelete = false;
            logView.AllowDuplicate = false;
            logView.GridDisplayType = GridDisplayType.BasedOnColumnWidth;

            if (logView.Fields.ContainsKey("LogType"))
            {
                ((ColumnField)logView.Fields["LogType"]).DefaultFilter = "1";
            }

            if (logView.Fields.ContainsKey("ExceptionMessage"))
            {
                ((ColumnField)logView.Fields["ExceptionMessage"]).TextHtmlControlType = TextHtmlControlType.TextArea;
                logView.Fields["ExceptionMessage"].ColSpanInDialog = 2;
            }
            if (logView.Fields.ContainsKey("Trace"))
            {
                ((ColumnField)logView.Fields["Trace"]).TextHtmlControlType = TextHtmlControlType.TextArea;
                logView.Fields["Trace"].ColSpanInDialog = 2;
            }

            if (logView.Fields.ContainsKey("FreeText"))
            {
                ((ColumnField)logView.Fields["FreeText"]).TextHtmlControlType = TextHtmlControlType.TextArea;
                logView.Fields["FreeText"].ColSpanInDialog = 2;
            }

            if (logView.Fields.ContainsKey("Time"))
            {
                ((ColumnField)logView.Fields["Time"]).Format = "MMM dd, yyyy hh:mm:ss";
            }

        }

        private void ConfigSystemUserTable()
        {
            View userView = (View)db.Views["v_durados_User"];
            userView.SystemView = true;
            userView.DisplayName = "User";
            userView.JsonName = "backandUsers";
            userView.Controller = "DuradosUser";

            userView.EditableTableName = "durados_User";
            if (userView.Fields.ContainsKey("Username"))
            {
                ColumnField usernameField = (ColumnField)userView.Fields["Username"];
                usernameField.DisplayName = "Username";
                usernameField.SpecialColumn = SpecialColumn.None;
                if (!Maps.PrivateCloud)
                {
                    usernameField.DisplayName += " (Email)";
                    usernameField.SpecialColumn = SpecialColumn.Email;
                }

                usernameField.DisableInEdit = true;
            }
            if (userView.Fields.ContainsKey("Password"))
            {
                ColumnField passwordField = (ColumnField)userView.Fields["Password"];
                passwordField.HideInEdit = true;
                passwordField.HideInCreate = true;
                passwordField.ExcludeInInsert = true;
                passwordField.ExcludeInUpdate = true;
                passwordField.HideInTable = true;
            }

            if (userView.Fields.ContainsKey("Guid"))
            {
                ColumnField guidField = (ColumnField)userView.Fields["Guid"];
                guidField.HideInEdit = false;
                guidField.DisableInEdit = true;
                guidField.HideInCreate = true;
                guidField.ExcludeInInsert = true;
                guidField.ExcludeInUpdate = true;
                guidField.HideInTable = true;
                guidField.AllowSelectRoles = "Developer";

            }


            if (userView.Fields.ContainsKey("Signature"))
            {
                ColumnField signatureField = (ColumnField)userView.Fields["Signature"];
                signatureField.HideInEdit = true;
                signatureField.HideInCreate = true;
                signatureField.ExcludeInInsert = true;
                signatureField.ExcludeInUpdate = true;
                signatureField.HideInTable = true;
            }
            if (userView.Fields.ContainsKey("SignatureHTML"))
            {
                ColumnField signatureHTMLField = (ColumnField)userView.Fields["SignatureHTML"];
                signatureHTMLField.HideInEdit = true;
                signatureHTMLField.HideInCreate = true;
                signatureHTMLField.ExcludeInInsert = true;
                signatureHTMLField.ExcludeInUpdate = true;
                signatureHTMLField.HideInTable = true;
            }

            if (userView.Fields.ContainsKey("IsApproved"))
            {
                ColumnField isApprovedField = (ColumnField)userView.Fields["IsApproved"];
                isApprovedField.HideInEdit = false;
                isApprovedField.HideInCreate = false;
                if (this is DuradosMap)
                {
                    isApprovedField.ExcludeInInsert = true;
                    isApprovedField.ExcludeInUpdate = true;
                }
                else
                {
                    isApprovedField.ExcludeInInsert = false;
                    isApprovedField.ExcludeInUpdate = false;
                }
                isApprovedField.ExcludeInUpdate = false;
                isApprovedField.HideInTable = false;
                isApprovedField.DefaultValue = true;
                isApprovedField.Required = false;
            }

            if (userView.Fields.ContainsKey("FullName"))
            {
                ColumnField fullNameField = (ColumnField)userView.Fields["FullName"];
                fullNameField.HideInEdit = true;
                fullNameField.HideInCreate = true;
                fullNameField.ExcludeInInsert = true;
                fullNameField.ExcludeInUpdate = true;
                fullNameField.HideInTable = true;
            }
            if (userView.Fields.ContainsKey("NewUser"))
            {
                ColumnField newUserField = (ColumnField)userView.Fields["NewUser"];
                newUserField.HideInEdit = true;
                newUserField.HideInCreate = true;
                newUserField.ExcludeInInsert = true;
                newUserField.ExcludeInUpdate = true;
                newUserField.HideInTable = true;
            }

            if (userView.Fields.ContainsKey("Comments"))
            {
                ColumnField commentsField = (ColumnField)userView.Fields["Comments"];
                commentsField.HideInEdit = false;
                commentsField.HideInCreate = false;
                commentsField.ExcludeInInsert = false;
                commentsField.ExcludeInUpdate = false;
                commentsField.HideInTable = false;
                commentsField.Required = false;
                commentsField.TextHtmlControlType = TextHtmlControlType.TextArea;
                commentsField.Rich = false;
                commentsField.ColSpanInDialog = 2;
            }
            if (userView.Fields.ContainsKey("Email"))
            {
                ColumnField emailField = (ColumnField)userView.Fields["Email"];
                emailField.SpecialColumn = SpecialColumn.Email;
                //emailField.HideInCreate = true;
                //emailField.HideInEdit = true;
                //emailField.ExcludeInInsert = true;
                //emailField.ExcludeInUpdate = true;

            }

            Field roleField = userView.GetFieldByColumnNames("Role");

            if (roleField != null)
            {
                roleField.DisplayName = "Role";
                roleField.JsonName = "Role";
            }

        }

        private void ConfigSystemPlugInTable()
        {
            if (db.Views.ContainsKey("durados_PlugInInstance"))
            {
                View instanceView = (View)db.Views["durados_PlugInInstance"];

                instanceView.Fields["Id"].ExcludeInInsert = false;
            }
        }

        private void ConfigSystemRoleTable()
        {
            if (db.Views.ContainsKey("durados_UserRole"))
            {

                View userView = (View)db.Views["durados_UserRole"];
                userView.DisplayName = "User Role";
                userView.JsonName = "backandRoles";
                userView.SystemView = true;
                userView.GridDisplayType = GridDisplayType.BasedOnColumnWidth;

                if (userView.Fields.ContainsKey("FirstView"))
                {

                    ColumnField firstViewField = (ColumnField)userView.Fields["FirstView"];
                    //SetRoles(firstViewField, "Developer");
                    firstViewField.Excluded = true;
                }
                userView.Fields["Name"].TableCellMinWidth = 200;
                userView.Fields["Description"].TableCellMinWidth = 200;
            }
        }
        private void SetRoles(ColumnField field, string roles)
        {
            field.Precedent = true;
            field.AllowSelectRoles = roles;
            field.AllowCreateRoles = roles;
            field.AllowEditRoles = roles;
        }


        private void Initiate(DataSet dataSetParam, string connectionString, string configFileName, bool save)
        {
            ConfigFileName = configFileName;
            dataset = dataSetParam;
            
            Refresh();

            if (save)
            {
                SaveConfigForTheFirstTimeInCaseOfChangeInStructure();
            }


            if (db.Localization != null)
                localizationDatabase = CreateLocalizationDatabase();
        }

        private void InitiateLocalization()
        {
            string localizationSchemaGeneratorFileName = Maps.GetDeploymentPath("Sql/Localization.sql");

            string cs = db.SysDbConnectionString;

            if (db.Localization != null)
            {
                if (!string.IsNullOrEmpty(db.Localization.LocalizationConnectionStringKey))
                    cs = System.Configuration.ConfigurationManager.ConnectionStrings[db.Localization.LocalizationConnectionStringKey].ConnectionString;
            }
            Durados.Localization.ILocalizer localizer = GetLocalizer();

            if (db.Localization != null)
            {
                localizer.InitLocalizer(db.Localization, cs, localizationSchemaGeneratorFileName);
            }

            Database.Localizer = localizer;
        }

        private Durados.Localization.ILocalizer GetLocalizer()
        {
            if (SystemDynamicMapper is Durados.DataAccess.AutoGeneration.Dynamic.MySqlMapper)
                return new Durados.Web.Localization.MySqlLocalizer();
            return new Durados.Web.Localization.Localizer();
        }

        public SiteInfo SiteInfo { get; set; }

        public void BackupConfig()
        {
            string filename = configDatabase.ConnectionString;
            BackupConfig(filename);
            BackupConfig(filename + ".xml");
        }

        private void BackupConfig(string filename)
        {
            string containerName = Maps.GetStorageBlobName(filename);

            CloudBlobContainer container = GetContainer(containerName);

            (new Durados.Web.Mvc.Azure.BlobBackup()).BackupSync(container, containerName, Database.ConfigVersion);
        }

        public void Refresh()
        {
            IsConfigChanged = true;
            DataSet ds1 = null;
            try
            {
                ds1 = dataset.Clone();
            }
            catch (Exception exception)
            {
                if (exception.Message == "MaxLength applies to string data type only. You cannot set Column 'id' property MaxLength to be non-negative number.")
                {
                    foreach (DataTable table in dataset.Tables)
                    {
                        foreach (DataColumn column in table.Columns)
                        {
                            if (column.DataType != typeof(string) && column.MaxLength >= 0)
                            {
                                Type datatype = column.DataType;
                                column.DataType = typeof(string);
                                column.MaxLength = -1;
                                column.DataType = datatype;
                            }
                        }
                    }
                    ds1 = dataset.Clone();
                }
                else throw exception;
            }
            Database tempDb = new Database(ds1, this);
            tempDb.ConnectionString = connectionString;
            tempDb.SystemConnectionString = systemConnectionString;


            configDatabase = CreateConfigDatabase(tempDb);
            configDatabase.DbConnectionString = connectionString;

            db = tempDb;
            db.Logger = Logger;
            configDatabase.Logger = Logger;
            db.SwVersion = Version;
            configDatabase.SwVersion = Version;

            foreach (View configView in configDatabase.Views.Values)
            {
                configView.SaveHistory = true;
            }

            UpdateAppInfo();

            InitiateLocalization();

            if (db.Localization != null)
            {
                //InitiateLocalization();
            }
            else
            {
                //Database.Localizer.UnsetLocalizationConfig();
            }
            db.SqlProduct = SqlProduct;
            db.SystemSqlProduct = SystemSqlProduct;
            configDatabase.SystemSqlProduct = SystemSqlProduct;
            ConfigSystemTables();

            List<ChildrenField> fields = CreateCounters();
            if (fields.Count > 0)
            {
                SaveConfigForTheFirstTimeInCaseOfChangeInStructure();
                //db = new Database(dataset);
                //db.ConnectionString = connectionString;

                //if (db.Localization != null)
                //    InitiateLocalization();

                foreach (ChildrenField field in fields)
                {
                    string counterFieldName = ChildrenField.FkCounterPrefix + field.Name;
                    if (dataset.Tables[field.View.Name].Columns.Contains(counterFieldName))
                    {
                        DataColumn dataColumn = dataset.Tables[field.View.Name].Columns[counterFieldName];
                        if (!db.Views[field.View.Name].Fields.ContainsKey(counterFieldName))
                        {
                            Field counterField = new ColumnField((View)field.View, dataColumn);
                            db.Views[field.View.Name].Fields.Add(counterFieldName, counterField);
                            db.Views[field.View.Name].Fields[counterFieldName].DisplayName = field.DisplayName + " Counter";
                            db.Views[field.View.Name].Fields[counterFieldName].ExcludeInInsert = true;
                            db.Views[field.View.Name].Fields[counterFieldName].ExcludeInUpdate = true;
                            db.Views[field.View.Name].Fields[counterFieldName].HideInTable = true;
                            db.Views[field.View.Name].Fields[counterFieldName].HideInFilter = true;
                        }
                    }
                }


                Durados.Config.Configurator configurator = new Durados.Config.Configurator();
                DataSet ds = configurator.ConvertToDataSet(db);
                string filename = Maps.GetConfigPath(ConfigFileName);
                //ds.WriteXml(filename, XmlWriteMode.WriteSchema);
                SaveConfig(ds, filename);
            }
            if (db.Localization != null)
            {
                Durados.Localization.ILocalizer localizer = db.Localizer;
                if (localizer.IsInitiated)
                    localizer.Refresh();
                else
                {
                    string localizationSchemaGeneratorFileName = Maps.GetDeploymentPath("Sql/Localization.sql");
                    string cs = db.SysDbConnectionString;
                    if (!string.IsNullOrEmpty(db.Localization.LocalizationConnectionStringKey))
                        cs = System.Configuration.ConfigurationManager.ConnectionStrings[db.Localization.LocalizationConnectionStringKey].ConnectionString;

                    localizer.InitLocalizer(db.Localization, cs, localizationSchemaGeneratorFileName);

                    //db.Localizer.Initiate(connectionString, localizationSchemaGeneratorFileName);
                }
            }
        }

        public void UpdateAppInfo()
        {
            if (SiteInfo != null)
            {
                if (Database.SiteInfo == null)
                    Database.SiteInfo = new SiteInfo();

                if (Database.SiteInfo.Company == null && SiteInfo.Company != null)
                {
                    Database.SiteInfo.Company = SiteInfo.Company;
                    Database.SiteInfo.ShowCompany = true;
                }

                //if (Database.SiteInfo.Logo == null && SiteInfo.Logo != null)
                //{
                if (SiteInfo.Logo != null)
                {
                    string[] segments = SiteInfo.Logo.Split('/');
                    if (segments.Length > 0)
                    {
                        Database.SiteInfo.Logo = segments[segments.Length - 1];
                        Database.SiteInfo.ShowLogo = true;
                    }
                }
                else
                {
                    Database.SiteInfo.ShowLogo = false;
                }
                //}

                if (Database.SiteInfo.LogoHref == null)
                {
                    if (SiteInfo.LogoHref != null)
                        Database.SiteInfo.LogoHref = SiteInfo.LogoHref;
                    else
                        Database.SiteInfo.LogoHref = "/Home/Default";
                }

                //if (SiteInfo.Product != null) || )
                //{
                Database.SiteInfo.Product = SiteInfo.Product;
                Database.SiteInfo.ShowProduct = true;
                if (string.IsNullOrEmpty(Database.SiteInfo.Version))
                    Database.SiteInfo.ShowVersion = false;
                //Database.SiteInfo.ShowProduct =Database.SiteInfo.ShowProduct?? SiteInfo.ShowProduct;
                //}
                //else 
                //if (Database.SiteInfo.Product == null && SiteInfo.Product == null)
                //{
                //    Database.SiteInfo.ShowProduct = false;
                //}
            }
        }

        private List<ChildrenField> CreateCounters()
        {
            bool counterCreated = false;
            List<ChildrenField> fields = new List<ChildrenField>();

            foreach (View view in Database.Views.Values)
            {
                foreach (ChildrenField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Children))
                {
                    if (field.Counter && !field.CounterInitiated)
                    {
                        if (CreateCounter(field))
                        {
                            counterCreated = true;
                            fields.Add(field);
                        }
                    }
                }
            }

            if (counterCreated)
            {
                SaveConfigForTheFirstTimeInCaseOfChangeInStructure();
            }

            return fields;
        }

        private bool CreateCounter(ChildrenField field)
        {
            string counterFieldName = ChildrenField.FkCounterPrefix + field.Name;
            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();
            try
            {
                sqlAccess.CreateCounter(field);
                field.CounterInitiated = true;
                if (!dataset.Tables[field.View.Name].Columns.Contains(counterFieldName))
                {
                    dataset.Tables[field.View.Name].Columns.Add(counterFieldName, typeof(int));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SaveConfigForTheFirstTimeInCaseOfChangeInStructure()
        {
            SetIds();
            string filename = ConfigFileName;
            Durados.Config.Configurator configurator = new Durados.Config.Configurator();
            DataSet ds = configurator.ConvertToDataSet(db);

            ds.SchemaSerializationMode = SchemaSerializationMode.IncludeSchema;
            try
            {
                SaveConfig(ds, filename);
            }
            catch (Exception exception)
            {
                Logger.Log("Map", "Initiation", "SaveConfigForTheFirstTimeInCaseOfChangeInStructure", exception, 3, "");
            }
        }

        private void SetIds()
        {
            List<Field> fields = new List<Field>();
            foreach (View view in db.Views.Values)
            {
                fields.AddRange(view.Fields.Values);
            }

            foreach (View view in db.Views.Values)
            {
                if (view.ID == -1)
                {
                    int maxID = db.Views.Values.Max(v => v.ID);
                    view.ID = maxID + 1;
                }
                foreach (Field field in view.Fields.Values)
                {
                    if (field.ID == -1)
                    {
                        int maxID = fields.Max(f => f.ID) + 1;
                        field.ID = maxID;
                    }
                }
            }
        }

        private void SaveConfig(DataSet ds, string filename)
        {
            if (Maps.Cloud && !(this is DuradosMap))
            {
                try
                {
                    WriteConfig(ds, filename);
                }
                catch (Exception exception)
                {
                    Logger.Log("Map", "SaveConfig", "WriteConfig", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                }

                return;
            }
            if (!File.Exists(filename))
            {
                try
                {
                    WriteConfig(ds, filename);
                }
                catch (Exception exception)
                {
                    Logger.Log("Map", "SaveConfig", "WriteConfig", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                }
            }


            string tempFileName = filename + ".temp_" + Guid.NewGuid().ToString() + ".xml";

            try
            {
                WriteConfig(ds, tempFileName);
            }
            catch (Exception exception)
            {
                Logger.Log("Map", "SaveConfig", "WriteConfig", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                throw new DuradosException("Failed to write temp config file. " + "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename, exception);

            }

            string copyFileName = filename + ".copy_" + Guid.NewGuid().ToString() + ".xml";

            try
            {
                File.Copy(filename, copyFileName, true);
            }
            catch (Exception exception)
            {
                Logger.Log("Map", "SaveConfig", "copy copy", exception, 2, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);

            }

            if (!FileHelper.Compare(tempFileName, filename))
            {
                if (System.IO.File.Exists(filename) && !Maps.Cloud)
                {
                    try
                    {
                        Backup(filename);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log("Map", "SaveConfig", "Backup", exception, 2, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);

                    }
                }
            }
            //WriteConfig(ds, filename);
            try
            {
                if (!FileHelper.ValidateConfig(tempFileName, filename))
                    return;
            }
            catch (Exception exception)
            {
                Logger.Log("Map", "SaveConfig", "FileHelper.ValidateConfig", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);

                return;
            }

            try
            {
                File.Copy(tempFileName, filename, true);
            }
            catch (Exception exception)
            {
                Logger.Log("Map", "SaveConfig", "File.Copy", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                //lock (this)
                //{
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        File.Copy(tempFileName, filename, true);
                        break;
                    }
                    catch (Exception exception2)
                    {
                        Logger.Log("Map", "SaveConfig", "File.Copy" + i, exception2, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                    }
                    System.Threading.Thread.Sleep(500);

                }
                if (!System.IO.File.Exists(filename))
                {
                    try
                    {
                        Logger.Log("Map", "SaveConfig", "File not exists, trying to copy", null, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                        File.Copy(copyFileName, filename, true);
                    }
                    catch (Exception exception3)
                    {
                        Logger.Log("Map", "SaveConfig", "File not exists, failed to copy", exception3, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                        throw new DuradosException("Config file does not exists. " + "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename, exception3);
                    }
                }
                //}
            }
            finally
            {
                try
                {
                    File.Delete(tempFileName);
                }
                catch (Exception exception)
                {
                    Logger.Log("Map", "SaveConfig", "Delete temp", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                }

                try
                {
                    File.Delete(copyFileName);
                }
                catch (Exception exception)
                {
                    Logger.Log("Map", "SaveConfig", "Delete copy", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                }
            }
        }

        private int backupFiles = 10;
        private string backupFolder = "Backup";

        private void Backup(string filename)
        {
            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("backupFiles"))
                backupFiles = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["backupFiles"]);
            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("backupFolder"))
                backupFolder = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["backupFolder"]);

            string backupPath = Path.Combine(Path.GetDirectoryName(filename), backupFolder);
            string filenameWithoutPath = Path.GetFileNameWithoutExtension(filename);

            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }

            for (int i = backupFiles; i >= 1; i--)
            {
                string backupFilename = Path.Combine(backupPath, filenameWithoutPath + "_" + i + ".xml");
                string newBackupFilename = Path.Combine(backupPath, filenameWithoutPath + "_" + (i + 1) + ".xml");
                if (File.Exists(backupFilename))
                {
                    if (i == backupFiles)
                    {
                        File.Delete(backupFilename);
                    }
                    else
                    {
                        File.Move(backupFilename, newBackupFilename);
                    }
                }
            }

            File.Move(filename, Path.Combine(backupPath, filenameWithoutPath + "_1.xml"));
        }

        private Durados.Web.Mvc.Config.Database configDatabase;
        private Durados.Web.Mvc.Database localizationDatabase;

        public Durados.Web.Mvc.Config.Database GetConfigDatabase()
        {
            return configDatabase;
        }

        public Durados.Web.Mvc.Database GetLocalizationDatabase()
        {
            if (db.Localization == null)
                return null;
            return localizationDatabase;
        }

        private Durados.Web.Mvc.Config.Database CreateConfigDatabase(Database tempDb)
        {
            string filename = ConfigFileName;

            Durados.Config.Configurator configurator = new Durados.Config.Configurator();

            DataSet ds = new DataSet();
            //if (storage.Exists(filename))
            //{
            //    ReadConfig(ds, filename);
            //}
            if ((Maps.Cloud && !(this is DuradosMap) && storage.Exists(filename)) || (System.IO.File.Exists(filename)))
            {
                ReadConfig(ds, filename);
            }
            else
            {
                ds = configurator.ConvertToDataSet(tempDb);
                ds.SchemaSerializationMode = SchemaSerializationMode.IncludeSchema;
                WriteConfig(ds, filename);
            }

            HashDerivedViewDataRows(ds);
            LoadDerivedViews(ds, tempDb);
            configurator.Load(ds, tempDb);
            CopyToDerivedViews(ds, tempDb);
            tempDb.ConnectionString = connectionString;
            tempDb.SystemConnectionString = systemConnectionString;
            configDatabase = new Durados.Web.Mvc.Config.Database(ds, this);
            configDatabase.DbConnectionString = connectionString;
            configDatabase.SwVersion = Version;
            configDatabase.ConnectionString = filename;

            foreach (DataRow row in ds.Tables["View"].Rows)
            {
                string viewName = row["Name"].ToString();
                if (tempDb.Views.ContainsKey(viewName))
                {
                    ((View)tempDb.Views[viewName]).ViewDataSetID = row["ID"].ToString();
                }
            }

            //LoadRulesAdditionalViews(tempDb);
            project.ConfigConfig(configDatabase, tempDb, localizationDatabase);

            return configDatabase;
        }

        public void WriteToStorage(DataSet ds, string filename)
        {
            WriteConfig(ds, filename);
        }

        public void ReadFromStorage(DataSet ds, string filename)
        {
            ReadConfig(ds, filename);
        }
        private void WriteConfig(DataSet ds, string filename)
        {
            if (Maps.Cloud && !(this is DuradosMap))
            {
                WriteConfigToCloud(ds, filename);

            }
            else
            {
                ds.WriteXml(filename, XmlWriteMode.WriteSchema);
            }
        }

        private void ReadConfig(DataSet ds, string filename)
        {
            if (Maps.Cloud && !(this is DuradosMap))
            {
                ReadConfigFromCloud(ds, filename);

            }
            else
            {
                try
                {
                    ds.ReadXml(filename, XmlReadMode.ReadSchema);
                }
                catch (ConstraintException ex)
                {
                    string errMsg = string.Empty;
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.HasErrors)
                        {
                            string rowErr = string.Empty;
                            foreach (DataRow dr in dt.GetErrors())
                            {
                                rowErr += dr.RowError + "\n";
                            }
                            errMsg += string.Format("Errors occurred in Table {0} :\n{1}\n", dt.TableName, rowErr.TrimEnd('\n'));

                        }
                    }
                    throw new ConstraintException("Error occurred while loading xml file\n" + errMsg, ex);
                }

            }
        }

        public bool IsMainApp(string filename)
        {
            System.IO.FileInfo fileInfo = new FileInfo(filename);
            string filenameOnly = fileInfo.Name.TrimEnd(fileInfo.Extension.ToCharArray());
            return filenameOnly.Equals(Maps.GetmainAppConfigName());
        }

        public bool Exists(string filename)
        {
            if (!Maps.Cloud)
                return System.IO.File.Exists(filename);

            string containerName = Maps.GetStorageBlobName(filename);

            return storage.Exists(containerName);
        }

        public void ReadConfigFromCloud(DataSet ds, string filename)
        {
            
            string blobName = Maps.GetStorageBlobName(filename);
            DataSet cachedDs = Maps.Instance.StorageCache.Get(blobName);
            
            // check exist in cache
            if (cachedDs != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    try
                    {
                        cachedDs.WriteXml(stream, XmlWriteMode.WriteSchema);
                        stream.Seek(0, SeekOrigin.Begin);
                        ds.ReadXml(stream);
                    }
                    catch(ConstraintException e)
                    {
                        string errMsg = string.Empty;
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.HasErrors)
                            {
                                string rowErr = string.Empty;
                                foreach (DataRow dr in dt.GetErrors())
                                {
                                    rowErr += dr.RowError + "\n";
                                }
                                errMsg += string.Format("Errors occurred in Table {0} :\n{1}\n", dt.TableName, rowErr.TrimEnd('\n'));

                            }
                        }

                        Maps.Instance.DuradosMap.Logger.Log("ReadConfigFromCloud", "ReadConfigFromCloud", "ReadConfigFromCloud", e, 1, errMsg);
                    }
                }
                return;
            }

            // fetch from storage
            ReadConfigFromCloudStorage(ds, filename);

            // add to cache for next read
            Maps.Instance.StorageCache.Add(blobName, ds);
        }
        
        public void ReadConfigFromCloudStorage(DataSet ds, string filename)
        {

            string containerName = Maps.GetStorageBlobName(filename);
            CloudBlobContainer container = GetContainer(containerName);

            CloudBlob blob = container.GetBlobReference(containerName);
            try
            {
                //BlobStream stream = blob.OpenRead();
                //string tempFileName = fileInfo.DirectoryName + "\\temp" + filenameOnly + "." + fileInfo.Extension;

                using (MemoryStream stream = new MemoryStream())
                {
                    blob.DownloadToStream(stream);

                    stream.Seek(0, SeekOrigin.Begin);

                    ds.ReadXml(stream);
                }
                //System.IO.File.Delete(tempFileName);

            }
            catch { }
        }

        public void WriteConfigToCloud(DataSet ds, string filename)
        {
            if (Maps.IsInMemoryMode())
                return;

            WriteConfigToCloud(ds, filename, true);
        }
        public void WriteConfigToCloud(DataSet ds, string filename, bool async)
        {
            Map map = null;
            if (Maps.Instance.AppInCach(Maps.GetCurrentAppName()))
                map = Maps.Instance.GetMap();

            WriteConfigToCloud2(ds, filename, async, map);

            if (map == null)
            {
                //map = Maps.Instance.GetMap();
                try
                {
                    map = Maps.Instance.GetMap();
                }
                catch
                {
                    return;
                }
            }
            if (map is DuradosMap)
                return;

            map.TimeStamp = DateTime.Now;
            Maps.Instance.UpdateCache(map.AppName, map);

            //SyncCache();
            //using (MemoryStream msNew = new MemoryStream())
            //{
            //    ds.WriteXml(stream);
            //    msNew.Seek(0, SeekOrigin.Begin);
            //    blob.UploadFromStream(msNew);
            //}  

        }

        public void WriteConfigToCloud2(DataSet ds, string filename, bool async, Map map)
        {
            string containerName = Maps.GetStorageBlobName(filename);
            Maps.Instance.StorageCache.Add(containerName, ds);

            CloudBlobContainer container = GetContainer(containerName);

            CloudBlob blob = container.GetBlobReference(containerName);
            blob.Properties.ContentType = "application/xml";

            if (!Maps.Instance.StorageCache.ContainsKey(containerName) || !async)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    ds.WriteXml(stream, XmlWriteMode.WriteSchema);
                    stream.Seek(0, SeekOrigin.Begin);

                    blob.UploadFromStream(stream);

                    RefreshApis(map);

                    Maps.Instance.Backup.BackupAsync(container, containerName);

                }
            }
            else
            {
                MemoryStream stream = new MemoryStream();
                ds.WriteXml(stream, XmlWriteMode.WriteSchema);
                stream.Seek(0, SeekOrigin.Begin);

                DateTime started = DateTime.Now;

                blob.BeginUploadFromStream(stream, BlobTransferCompletedCallback, new BlobTransferAsyncState(blob, stream, started, container, containerName, map));

                try
                {
                    if (map != null)
                    {
                        Maps.Instance.DuradosMap.Logger.Log("Map", "WriteConfigToCloud", map.AppName ?? string.Empty, string.Empty, string.Empty, -8, containerName + " started", started);
                    }
                }
                catch { }
            }
        }

        private void BlobTransferCompletedCallback(IAsyncResult result)
        {
            BlobTransferAsyncState state = (BlobTransferAsyncState)result.AsyncState;
            if (state == null || state.Map == null)
                return;
                
            try
            {
                Maps.Instance.DuradosMap.Logger.Log("Map", "WriteConfigToCloud", state.Map.AppName ?? string.Empty, DateTime.Now.Subtract(state.Started).TotalMilliseconds.ToString(), string.Empty, -8, state.BlobName + " ended", DateTime.Now);
            }
            catch { }

            RefreshApis(state.Map);

            try
            {
                state.Blob.EndUploadFromStream(result);
                if (!Maps.IsApi2())
                {
                    Maps.Instance.Backup.BackupAsync(state.Container, state.BlobName);
                }
            }
            catch (Exception exception)
            {
                Map map = Maps.Instance.GetMap();
                map.Logger.Log("Map", "BlobTransferCompletedCallback", map.AppName ?? string.Empty, exception, 1, string.Empty);
            }
        }

        public void RefreshApis(Map map)
        {
            try
            {
                if (map == null)
                    return;
                if (map is DuradosMap)
                    return;

                foreach (string apiUrl in Maps.ApiUrls)
                {
                    if (!string.IsNullOrEmpty(apiUrl))
                        RefreshApi(apiUrl, map.AppName, map.Guid.ToString());
                }
            }
            catch { }
        }

        private void RefreshApi(string apiUrl, string appname, string appguid)
        {
            try
            {
                string refreshApiUrl = apiUrl + string.Format("/1/config/refresh/{0}?appguid={1}", appname, appguid);
                Durados.Web.Mvc.Infrastructure.Http.CallWebRequest(refreshApiUrl);
            }
            catch { }
        }


        Azure.DuradosStorage storage = new Azure.DuradosStorage();

        private CloudBlobContainer GetContainer(string filename)
        {
            // Get a handle on account, create a blob service client and get container proxy
            //var account = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("ConfigAzureStorage"));
            //var client = account.CreateCloudBlobClient();
            //return client.GetContainerReference(RoleEnvironment.GetConfigurationSettingValue("configContainer"));
            return storage.GetContainer(filename);
        }


        //private void EnsureContainerExists()
        //{
        //    var container = GetContainer();
        //    container.CreateIfNotExist();
        //    var permissions = container.GetPermissions();
        //    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
        //    container.SetPermissions(permissions);
        //}


        private void LoadRulesAdditionalViews(Database database)
        {
            //we stop support the old token in addtion there are customers that has [User] table so it doesn't work

            //string oldUserPlaceHolder = "[User]";
            //foreach (View view in database.Views.Values)
            //{
            //    if (!string.IsNullOrEmpty(view.PermanentFilter) && view.PermanentFilter.Contains(oldUserPlaceHolder))
            //    {
            //        view.PermanentFilter = view.PermanentFilter.Replace(oldUserPlaceHolder, Database.UserPlaceHolder);
            //    }
            //    if (!string.IsNullOrEmpty(view.PermanentFilter) && view.PermanentFilter.Contains(oldUserPlaceHolder.ToLower()))
            //    {
            //        view.PermanentFilter = view.PermanentFilter.Replace(oldUserPlaceHolder.ToLower(), Database.UserPlaceHolder);
            //    }
            //    foreach (Field field in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            //    {
            //        if (field.DefaultValue != null && field.DefaultValue.ToString().Equals(oldUserPlaceHolder))
            //        {
            //            field.DefaultValue = Database.UserPlaceHolder;
            //        }
            //        if (field.DefaultValue != null && field.DefaultValue.ToString().Equals(oldUserPlaceHolder.ToLower()))
            //        {
            //            field.DefaultValue = Database.UserPlaceHolder;
            //        }
            //    }

            //    foreach (Rule rule in view.Rules.Values)
            //    {
            //        if (!string.IsNullOrEmpty(rule.WhereCondition) && rule.WhereCondition.Contains(oldUserPlaceHolder))
            //        {
            //            rule.WhereCondition = rule.WhereCondition.Replace(oldUserPlaceHolder, Database.UserPlaceHolder);
            //        }
            //        LoadRuleAdditionalViews(database, rule);
            //    }
            //}
        }

        private void LoadRuleAdditionalViews(Database database, Rule rule)
        {
            string[] viewNames = rule.GetAdditionalViewNames();

            if (viewNames == null)
                return;

            foreach (string viewName in rule.GetAdditionalViewNames())
            {
                if (database.Views.ContainsKey(viewName))
                {
                    View view = (View)database.Views[viewName];

                    if (!view.Rules.ContainsKey(rule.Name) && !view.AdditionalRules.ContainsKey(rule.Name))
                    {
                        view.AdditionalRules.Add(rule.Name, rule);
                    }
                }
            }
        }

        private void CopyToDerivedViews(DataSet ds, Database db)
        {
            foreach (string baseViewName in derivedViewDictionary.Keys)
            {
                foreach (DataRow row in derivedViewDictionary[baseViewName])
                {
                    CopyFromBaseView((View)db.Views[baseViewName], (View)db.Views[row["Name"].ToString()]);
                }
            }
        }

        private void LoadDerivedViews(DataSet ds, Database db)
        {
            foreach (string baseViewName in derivedViewDictionary.Keys)
            {
                foreach (DataRow row in derivedViewDictionary[baseViewName])
                {
                    LoadDerivedView(row, (View)db.Views[baseViewName]);
                }
            }
        }

        Dictionary<string, List<DataRow>> derivedViewDictionary;

        void HashDerivedViewDataRows(DataSet ds)
        {
            derivedViewDictionary = new Dictionary<string, List<DataRow>>();

            if (ds.Tables.Contains("View") && ds.Tables["View"].Columns.Contains(baseName))
            {
                foreach (DataRow row in ds.Tables["View"].Rows)
                {
                    if (row[baseName] != null && row[baseName].ToString() != string.Empty)
                    {
                        string baseViewName = row[baseName].ToString();

                        if (!derivedViewDictionary.ContainsKey(baseViewName))
                        {
                            derivedViewDictionary.Add(baseViewName, new List<DataRow>());
                        }
                        derivedViewDictionary[baseViewName].Add(row);
                    }
                }
            }

        }


        private Durados.Web.Mvc.Database CreateLocalizationDatabase()
        {
            Durados.Localization.Model.LocalizationDataSet ds = (Durados.Localization.Model.LocalizationDataSet)Database.Localizer.LocalizationDataSet.Clone();
            Durados.Web.Mvc.Database localizationDatabase = new Durados.Web.Mvc.Database(ds, this);
            localizationDatabase.ConnectionString = systemConnectionString;
            localizationDatabase.SystemConnectionString = systemConnectionString;
            localizationDatabase.DefaultController = "Localization";

            project.ConfigLocalization(localizationDatabase);
            return localizationDatabase;
        }

        string baseName = "baseName";


        private void LoadDerivedView(DataRow row, DataRow baseRow, View baseView, Dictionary<DataRow, object> loadedObjects, Durados.Config.Configurator configurator)
        {
            string name = row["Name"].ToString();
            if (!baseView.Database.Views.ContainsKey(name))
            {
                View view = new View(baseView.DataTable, baseView.Database, name);
                baseView.Database.Views.Add(name, view);

            }

        }

        private void LoadDerivedView(DataRow row, View baseView)
        {
            string name = row["Name"].ToString();
            LoadDerivedView(name, baseView);
        }

        private void LoadDerivedView(string name, View baseView)
        {
            if (!baseView.Database.Views.ContainsKey(name))
            {
                View view = new View(baseView.DataTable, baseView.Database, name);
                baseView.Database.Views.Add(name, view);
            }

        }

        private void CopyFromBaseView(View baseView, View view)
        {
            Copy(baseView, view, true);
            view.BaseName = baseView.Name;
        }

        public String Copy(View template, View view)
        {
            return Copy(template, view, false);

        }

        Durados.Config.Configurator configuratorCopy = new Durados.Config.Configurator();

        public String Copy(View template, View view, bool skipDoNotCopy)
        {
            string notFound = string.Empty;
            string failed = string.Empty;

            try
            {
                configuratorCopy.Copy(template, view, skipDoNotCopy);
            }
            catch { }

            foreach (Field field in template.Fields.Values)
            {
                Field destField = null;

                switch (field.FieldType)
                {
                    case FieldType.Column:
                        if (view.Fields.ContainsKey(field.Name))
                        {
                            destField = view.Fields[field.Name];

                            //configurator.Copy(field, f);
                        }
                        else
                        {
                            notFound += field.DisplayName + ";";

                        }
                        break;

                    case FieldType.Parent:
                        destField = view.GetTwin((ParentField)field);
                        if (destField == null)
                        {
                            notFound += field.DisplayName + ";";
                        }
                        break;

                    case FieldType.Children:
                        destField = view.GetTwin((ChildrenField)field);
                        if (destField == null)
                        {
                            notFound += field.DisplayName + ";";
                        }
                        break;

                    default:
                        break;
                }

                if (destField != null)
                {
                    try
                    {
                        configuratorCopy.Copy(field, destField, skipDoNotCopy);
                    }
                    catch
                    {
                        failed += field.DisplayName + ";";
                    }
                }

            }

            string message = "Not found in source: " + notFound + "\n\n Failed to copy: " + failed;

            return message;
        }

        public View GetView(string pk)
        {
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

            DataRow row = configAccess.GetDataRow("View", configDatabase.ConnectionString, pk);

            if (row.Table.Columns.Contains("Name"))
            {
                string name = row["Name"].ToString();
                if (this.Database.Views.ContainsKey(name))
                {
                    return (View)this.Database.Views[name];
                }
            }

            return null;
        }

        public Workspace GetWorkspace(string pk)
        {
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

            DataRow row = configAccess.GetDataRow("Workspace", configDatabase.ConnectionString, pk);

            int id = -1;
            if (int.TryParse(pk, out id))
            {
                if (this.Database.Workspaces.ContainsKey(id))
                {
                    return (Workspace)this.Database.Workspaces[id];
                }
            }

            return null;
        }

        public Field GetField(string pk)
        {
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

            DataRow row = configAccess.GetDataRow("Field", configDatabase.ConnectionString, pk);

            if (row.Table.Columns.Contains("Name"))
            {
                string fieldName = row["Name"].ToString();

                DataRow viewRow = row.GetParentRow("Fields");

                if (viewRow != null)
                {

                    string viewName = viewRow["Name"].ToString();
                    if (this.Database.Views.ContainsKey(viewName))
                    {
                        View view = (View)this.Database.Views[viewName];

                        if (view.Fields.ContainsKey(fieldName))
                        {
                            return view.Fields[fieldName];
                        }
                    }
                }
            }

            return null;
        }

        public bool IsViewAlreadyExists(string name)
        {
            return DynamicMapper.IsViewAlreadyExists(name, dataset);
        }

        public void SaveDynamicMapping()
        {
            DynamicMapper.Save();
        }

        private Database defualtDatabase = null;

        public Database GetDefaultDatabase()
        {
            //if (defualtDatabase == null)
            //{

            DataSet ds = GetDefaultDataSet();
            defualtDatabase = new Database(ds, this);
            //}

            return defualtDatabase;
        }

        public DataSet GetDefaultDataSet()
        {
            DataSet ds = new DataSet();

            DataTable parentTable = ds.Tables.Add("ParentTable");
            DataColumn pk = parentTable.Columns.Add("Id", typeof(int));
            parentTable.PrimaryKey = new DataColumn[1] { pk };

            parentTable.Columns.Add("Column", typeof(string));

            DataTable table = ds.Tables.Add("Table");

            DataColumn pk2 = table.Columns.Add("Id", typeof(int));
            table.PrimaryKey = new DataColumn[1] { pk2 };

            table.Columns.Add("Column", typeof(string));
            DataColumn fk = table.Columns.Add("Parent", typeof(int));

            ds.Relations.Add(pk, fk);

            return ds;
        }

        public void Sync(string viewName, string configViewPk)
        {
            Database db = GetDefaultDatabase();
            Field field = db.Views["Table"].Fields["Column"];
            ParentField parentField = (ParentField)db.Views["Table"].Fields["Relation1_Parent"];


            DynamicMapper.Sync(Database.Views[viewName], configViewPk, configDatabase.Views["Field"], field, dataset, Database, parentField);
        }

        public Dictionary<string, int> RemovedDeletedViews(Database database, out string message)
        {
            message = string.Empty;
            string[] deletedViews = DynamicMapper.GetDeletedViews(database);
            int i = 0;
            foreach (string viewName in deletedViews)
            {
                if (database.Views.ContainsKey(viewName))
                {
                    View view = (View)database.Views[viewName];
                    try
                    {
                        DynamicMapper.DeleteView2(view, dataset);
                        i++;
                    }
                    catch (Exception exception)
                    {
                        message += exception.Message + ";\n";
                    }
                }
            }

            return new Dictionary<string, int>() { { "deletedTables", deletedViews.Length }, { "removed", i } };
        }

        public string CreateField(string viewName, string configViewPk, Dictionary<string, object> values, out string manyToManyViewName)
        {
            Database db = GetDefaultDatabase();
            Field field = db.Views["Table"].Fields["Column"];
            ParentField parentField = (ParentField)db.Views["Table"].Fields["Relation1_Parent"];
            Dictionary<string, string> viewDefaults = GetViewDefaults();
            field.JsonName = values["JsonName"].ToString();
            parentField.JsonName = values["JsonName"].ToString();

            return DynamicMapper.CreateField(Database.Views[viewName], configViewPk, configDatabase.Views["Field"], field, parentField, values, db, viewDefaults, dataset, out manyToManyViewName);
        }


        public void ChangeFieldRelation(View view, Durados.DataAccess.AutoGeneration.Dynamic.RelationChange relationChange, string oldRelatedViewName, string newRelatedViewName, string configFieldPk, View configFieldView, Dictionary<string, object> values)
        {
            Database db = GetDefaultDatabase();
            DynamicMapper.ChangeFieldRelation(view, relationChange, oldRelatedViewName, newRelatedViewName, configFieldPk, configFieldView, db, values, dataset);
        }

        public void DeleteView(string viewName)
        {
            View view = (View)this.Database.Views[viewName];

            DynamicMapper.DeleteView(view, dataset);
        }

        public void ChangeSimpleDataType(View view, Durados.ColumnField columnField, string oldDataType, string newDataType, string configFieldPk, View configView, string configViewPk, View configFieldView, Dictionary<string, object> values)
        {

            Database db = GetDefaultDatabase();
            Field field = db.Views["Table"].Fields["Column"];
            ParentField parentField = (ParentField)db.Views["Table"].Fields["Relation1_Parent"];
            Dictionary<string, string> viewDefaults = GetViewDefaults();
            DynamicMapper.ChangeSimpleDataType(view, columnField, oldDataType, newDataType, configFieldPk, configView, configViewPk, configFieldView, db, field, parentField, values, viewDefaults, dataset);
        }

        public string ChangeDataType(View view, string oldDataType, string newDataType, string configFieldPk, View configView, string configViewPk, View configFieldView, Dictionary<string, object> values, out string manyToManyViewName)
        {
            Database db = GetDefaultDatabase();
            Field field = db.Views["Table"].Fields["Column"];
            ParentField parentField = (ParentField)db.Views["Table"].Fields["Relation1_Parent"];
            Dictionary<string, string> viewDefaults = GetViewDefaults();
            return DynamicMapper.ChangeDataType(view, oldDataType, newDataType, configFieldPk, configView, configViewPk, configFieldView, db, field, parentField, values, viewDefaults, dataset, out manyToManyViewName);
        }

        public string GetNewConfigFieldPk(View configView, string configViewPk, string newFieldName)
        {
            return DynamicMapper.GetNewConfigFieldPk(new ConfigAccess(), configView, configViewPk, newFieldName);
        }

        public DataView GetSchemaEntities()
        {
            return DynamicMapper.GetSchemaEntities(Database.Views["durados_Schema"]);
        }
        public virtual string GetLogoSrc(string fileName)
        {
            Durados.Web.Mvc.MapDataSet.durados_AppRow dr = Durados.Web.Mvc.Maps.Instance.GetAppRow();
            if (dr != null)
            {
                ///Admin/Download/Workspace?fieldName=Description&amp;filename=/moduBiz.png&amp;pk=\"
                if (string.IsNullOrEmpty(fileName))
                    fileName = dr.Image;

                return string.Format("/Home/{0}/{1}?fieldName={2}&amp;fileName={3}&amp;pk={4}", DownloadActionName, dr.Table.TableName, "Image", fileName, dr.Id.ToString());
            }
            return null;
        }
        public virtual string GetLogoSrc()
        {
            return GetLogoSrc(string.Empty);
        }

        public string Id { get; set; }

        public string DatabaseName { get; set; }

        public int Plan { get; set; }

        public virtual SqlProduct SqlProduct { get; set; }
        public virtual SqlProduct SystemSqlProduct { get; set; }

        public virtual Guid Guid { get; set; }

        public bool AsyncOperationRuning { get; set; }

        JsonConfigCache jsonConfigCache = new JsonConfigCache();
        public JsonConfigCache JsonConfigCache
        {
            get
            {
                return jsonConfigCache;
            }
        }

        Durados.Data.ICache<DataSet> configCache = CacheFactory.CreateCache<DataSet>("configCache");

        public Durados.Data.ICache<DataSet> ConfigCache
        {
            get
            {
                return configCache;
            }
        }

        Dictionary<string, Dictionary<string, object>> allKindOfCache = new Dictionary<string, Dictionary<string, object>>();

        public Dictionary<string, Dictionary<string, object>> AllKindOfCache
        {
            get
            {
                return allKindOfCache;
            }
        }

        MapSimpleCache mapSimpleCache = new MapSimpleCache();
        public MapSimpleCache MapSimpleCache
        {
            get
            {
                return mapSimpleCache;
            }
        }

        public Theme Theme { get; set; }

        public void UpdateTheme(Theme theme)
        {
            try
            {
                if (theme.Id == Maps.CustomTheme)
                {
                    string sql = @"update durados_App set ThemeId = @ThemeId, CustomThemePath = @CustomThemePath where ID = @AppId";
                    (new SqlAccess()).ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql, new Dictionary<string, object>() { { "ThemeId", theme.Id }, { "CustomThemePath", theme.Path }, { "AppId", Id } }, null);
                }
                else
                {
                    string sql = @"update durados_App set ThemeId = @ThemeId where ID = @AppId";
                    (new SqlAccess()).ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql, new Dictionary<string, object>() { { "ThemeId", theme.Id }, { "AppId", Id } }, null);
                }
                Theme = theme;
            }
            catch (Exception exception)
            {
                Logger.Log("Map", "UpdateTheme", "ExecuteNonQuery", exception, 1, "theme id: " + theme.Id.ToString());
            }
        }

        public string GetPreviewPath()
        {
            if (Theme.Id == Maps.CustomTheme)
            {
                return Theme.Path;
            }
            else
            {
                string baseUrl = string.IsNullOrEmpty(Database.UserPreviewUrl) ? "http://" + AppName + Durados.Web.Mvc.Maps.UserPreviewUrl : Database.UserPreviewUrl;

                return baseUrl + Theme.Path;
            }

        }


        private ICache<object> lockerCache = CacheFactory.CreateCache<object>("lockerCache");

        /// <summary>
        ///  Should not be used if you are not configAccess!!
        /// </summary>
        public Data.ICache<object> LockerCache
        {
            get 
            {
                return lockerCache;
            }
        }
    }

    public class Theme
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

    }

    public class MapSimpleCache
    {
        public Dictionary<string, object> cache = new Dictionary<string, object>();

        public object Get(string name)
        {
            if (cache.ContainsKey(name))
                return cache[name];

            return null;
        }

        public bool ContainsKey(string name)
        {
            return cache.ContainsKey(name);
        }

        public void Set(string name, object value)
        {
            if (cache.ContainsKey(name))
                cache[name] = value;
            else
                cache.Add(name, value);
        }
    }
    public class JsonConfigCache
    {
        public Dictionary<string, Dictionary<string, object>> jsonConfigCache = new Dictionary<string, Dictionary<string, object>>();

        public Dictionary<string, object> Get(string name)
        {
            if (jsonConfigCache.ContainsKey(name))
                return jsonConfigCache[name];

            return null;
        }

        public bool ContainsKey(string name)
        {
            return jsonConfigCache.ContainsKey(name);
        }

        public void Set(string name, Dictionary<string, object> value)
        {
            if (jsonConfigCache.ContainsKey(name))
                jsonConfigCache[name] = value;
            else
                jsonConfigCache.Add(name, value);
        }
        public void Clear()
        {
            jsonConfigCache.Clear();
        }
    }

    //public class AppStack
    //{
    //    int size;
    //    int first;
    //    int from;
    //    int to;
    //    IDbCommand command;

    //    public AppStack(int size, IDbCommand command) :
    //        this(size, 1, command)
    //    {
    //    }

    //    public AppStack(int size, int first, IDbCommand command)
    //    {
    //        this.size = size;
    //        this.first = first;
    //        this.command = command;
    //        LoadRange();
    //    }

    //    private void LoadRange()
    //    {
    //        LoadFrom();
    //        LoadTo();
    //    }

    //    private void LoadFrom()
    //    {
    //    }

    //    private void LoadTo()
    //    {
    //    }
    //}

    public class PendingPool
    {
        int size;
        int prev;
        int first;

        public PendingPool(int size) :
            this(size, 1)
        {
        }

        public PendingPool(int size, int first)
        {
            this.size = size;
            this.first = first;
            this.prev = first - 1;
        }

        int last
        {
            get
            {
                return size + first - 1;
            }
        }


        public int Next()
        {
            if (prev >= last)
            {
                return first;
            }
            else
            {
                prev++;
                return prev;
            }
        }
    }

    public class Maps
    {
        private static Maps instance;

        public static string GetInMemoryKey()
        {
            return System.Web.HttpContext.Current.Request.Headers["TestKey"];
        }

        public static bool IsInMemoryMode()
        {
            return GetInMemoryKey() != null;
        }

        public string[] GetVersions(string name)
        {
            int? id = AppExists(name);
            if (!id.HasValue)
                return null;

            string containerName = Maps.DuradosAppPrefix.Replace("_", "").Replace(".", "").ToLower() + id.ToString();
            CloudBlobContainer container = GetContainer(containerName);

            return (new Durados.Web.Mvc.Azure.BlobBackup()).GetVersions(container);
        }

        public void Restore(string name, string version = null)
        {
            int? id = AppExists(name);
            if (!id.HasValue)
                return;

            string containerName = Maps.DuradosAppPrefix.Replace("_", "").Replace(".", "").ToLower() + id.ToString();
            CloudBlobContainer container = GetContainer(containerName);

            if (version == null)
            {
                (new Durados.Web.Mvc.Azure.BlobBackup()).RestoreSync(container, containerName);
            }
            else
            {
                (new Durados.Web.Mvc.Azure.BlobBackup()).CopyBack(container, version, containerName);
            }

            containerName += "xml";
            container = GetContainer(containerName);
            if (version == null)
            {
                (new Durados.Web.Mvc.Azure.BlobBackup()).RestoreSync(container, containerName);
            }
            else
            {
                (new Durados.Web.Mvc.Azure.BlobBackup()).CopyBack(container, version, containerName);
            }
        }

        Azure.DuradosStorage storage = new Azure.DuradosStorage();

        private CloudBlobContainer GetContainer(string filename)
        {
            // Get a handle on account, create a blob service client and get container proxy
            //var account = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("ConfigAzureStorage"));
            //var client = account.CreateCloudBlobClient();
            //return client.GetContainerReference(RoleEnvironment.GetConfigurationSettingValue("configContainer"));
            return storage.GetContainer(filename);
        }

        private Maps()
        {
            InitPersistency();

            if (multiTenancy)
            {
                duradosMap = new DuradosMap();
                duradosMap.connectionString = persistency.ConnectionString;
                duradosMap.systemConnectionString = persistency.ConnectionString;
                duradosMap.ConfigFileName = Maps.GetConfigPath(Maps.GetmainAppConfigName() + ".xml");
                duradosMap.Url = GetAppUrl(duradosAppName);
                duradosMap.Initiate(false);

                View appView = (View)duradosMap.Database.Views["durados_App"];
                appView.PermanentFilter = "(durados_App.toDelete =0 AND (durados_App.Creator = [m_User] or durados_App.id in (select durados_UserApp.AppId from durados_UserApp where durados_UserApp.UserId = [m_User] and (durados_UserApp.[Role] = 'Admin' or durados_UserApp.[Role] = 'Developer'))))";
                appView.Controller = "MultiTenancy";

                View connectionView = (View)duradosMap.Database.Views["durados_SqlConnection"];
                connectionView.PermanentFilter = "";// "DuradosUser = [Durados_User] ";//OR durados_SqlConnection.id  in 
                //((select SqlConnectionId from durados_app inner join durados_userApp on durados_app.id =durados_userApp.appId where durados_UserApp.UserId = [Durados_User])
                //union
                //(select SystemSqlConnectionId from durados_app inner join durados_userApp on durados_app.id =durados_userApp.appId where durados_UserApp.UserId = [Durados_User]))";

                //maps = new Dictionary<string, Map>();
                mapsCache = CacheFactory.CreateCache<Map>("maps");

                LoadDnsAliases();

                PluginsCache = new Dictionary<PlugInType, PluginCache>();

                foreach (PlugInType plugInType in Enum.GetValues(typeof(PlugInType)))
                {
                    PluginsCache.Add(plugInType, new PluginCache());
                }
            }
        }



        public static Maps Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Maps();
                }
                return instance;
            }
        }

        public void WakeupCalltoApps()
        {
            using (SqlConnection connection = new SqlConnection(duradosMap.connectionString))
            {
                connection.Open();

                string sql = "select [Id],[Url] from dbo.durados_App with (NOLOCK) where [Creator] is null";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int urlOrdinal = reader.GetOrdinal("Url");

                        while (reader.Read())
                        {
                            string url = reader.GetString(urlOrdinal).ToLower();
                            //Infrastructure.ISendAsyncErrorHandler SendAsyncErrorHandler = null;
                            //ignore errors
                            Infrastructure.Http.CallWebRequest(url.Split('|')[2]);
                        }
                    }
                }
            }

        }

        private void LoadDnsAliases()
        {
            DnsAliases = new Dictionary<string, string>();

            using (SqlConnection connection = new SqlConnection(duradosMap.connectionString))
            {
                connection.Open();

                string sql = "SELECT dbo.durados_DnsAlias.Alias, dbo.durados_App.Name FROM dbo.durados_App with (NOLOCK) INNER JOIN dbo.durados_DnsAlias with (NOLOCK) ON dbo.durados_App.Id = dbo.durados_DnsAlias.AppId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int aliasOrdinal = reader.GetOrdinal("Alias");
                        int nameOrdinal = reader.GetOrdinal("Name");
                        while (reader.Read())
                        {
                            DnsAliases.Add(reader.GetString(aliasOrdinal).ToLower(), reader.GetString(nameOrdinal).ToLower());
                        }
                    }
                }
            }

        }

        static Maps()
        {
            host = System.Configuration.ConfigurationManager.AppSettings["durados_host"] ?? "durados.com";
            poolCreator = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["poolCreator"] ?? "55555");
            poolShouldBeUsed = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["poolShouldBeUsed"] ?? "false");

            redisConnectionString = System.Configuration.ConfigurationManager.AppSettings["redisConnectionString"] ?? "pub-redis-10938.us-east-1-4.3.ec2.garantiadata.com:10938,password=bell1234"; 
        

            mainAppConfigName = System.Configuration.ConfigurationManager.AppSettings["mainAppConfigName"] ?? "backand";
            hostByUs = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["hostByUs"] ?? "false");
            duradosAppName = System.Configuration.ConfigurationManager.AppSettings["durados_appName"] ?? "www";
            demoAzureUsername = System.Configuration.ConfigurationManager.AppSettings["demoAzureUsername"] ?? "itayher";
            demoAzurePassword = System.Configuration.ConfigurationManager.AppSettings["demoAzurePassword"] ?? "Durados2012";
            demoSqlUsername = System.Configuration.ConfigurationManager.AppSettings["demoSqlUsername"] ?? "durados";
            demoSqlPassword = System.Configuration.ConfigurationManager.AppSettings["demoSqlPassword"] ?? "durados";
            demoDatabaseName = System.Configuration.ConfigurationManager.AppSettings["demoDatabaseName"] ?? "Northwind";
            demoConfigFilename = System.Configuration.ConfigurationManager.AppSettings["demoConfigFilename"] ?? "Northwind";
            demoAzureServer = System.Configuration.ConfigurationManager.AppSettings["demoAzureServer"] ?? "tcp:d9gwdrhh5n.database.windows.net,1433";
            demoOnPremiseServer = System.Configuration.ConfigurationManager.AppSettings["demoOnPremiseServer"] ?? @"durados.info\sqlexpress";
            demoCreatePending = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["demoCreatePending"] ?? "true");
            demoPendingNext = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["demoPendingNext"] ?? "5");
            demoFtpTempHost = System.Configuration.ConfigurationManager.AppSettings["demoFtpTempHost"] ?? "temp";
            demoFtpHost = System.Configuration.ConfigurationManager.AppSettings["demoFtpHost"] ?? "durados.info";
            demoFtpPort = System.Configuration.ConfigurationManager.AppSettings["demoFtpPort"] ?? "21";
            demoFtpFileSizeLimitKb = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["demoFtpFileSizeLimitKb"] ?? "1024");
            demoFtpFolderSizeLimitKb = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["demoFtpFolderSizeLimitKb"] ?? "1024");
            demoFtpUser = System.Configuration.ConfigurationManager.AppSettings["demoFtpUser"] ?? "itay";
            demoFtpPassword = System.Configuration.ConfigurationManager.AppSettings["demoFtpPassword"] ?? "dio2008";
            demoFtpPhysicalPath = System.Configuration.ConfigurationManager.AppSettings["demoFtpPhysicalPath"] ?? @"C:\FTP\";
            demoUploadSourcePath = System.Configuration.ConfigurationManager.AppSettings["demoUploadSourcePath"] ?? "/Uploads/220/";
            demoOnPremiseSourcePath = System.Configuration.ConfigurationManager.AppSettings["demoOnPremiseSourcePath"] ?? @"C:\Dev\Databases\";
            allowLocalConnection = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["allowLocalConnection"] ?? "false");
            cloud = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["cloud"] ?? "false"); // false;// RoleEnvironment.IsAvailable;//
            multiTenancy = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["multiTenancy"] ?? "false");
            useSecureConnection = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseSecureConnection"] ?? "false");
            skin = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["skin"] ?? "false");
            duradosAppPrefix = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["duradosAppSysPrefix"] ?? "durados_AppSys_");
            duradosAppSysPrefix = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["duradosAppPrefix"] ?? "durados_App_");
            debug = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["debug"] ?? "false");
            appNameMax = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["appNameMax"] ?? "32");
            dropAppDatabase = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["dropAppDatabase"] ?? "true");
            privateCloud = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["privateCloud"] ?? "false");
            ConfigPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["configPath"] ?? "~/Config/");
            plugInSampleGenerationCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["plugInSampleGenerationCount"] ?? "5");

            superDeveloper = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["superDeveloper"] ?? "dev@devitout.com").ToLower();

            DownloadDenyPolicy = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DownloadDenyPolicy"] ?? "true");
            OldAdminHttp = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["OldAdminHttp"] ?? "false");
            AllowedDownloadFileTypes = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["AllowedDownloadFileTypes"] ?? allowedDownloadFileTypesDefault).Split(',').ToArray();
            DenyDownloadFileTypes = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DenyDownloadFileTypes"] ?? denyDownloadFileTypesDefault).Split(',').ToArray();

            ReservedAppNames = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ReservedAppNames"] ?? reservedAppNames).Split(',').Select(k => k).ToHashSet<string>();

            azureDatabasePendingPool = new PendingPool(demoPendingNext);
            onPremiseDatabasePendingPool = new PendingPool(demoPendingNext);

            AzureStorageAccountName = System.Configuration.ConfigurationManager.AppSettings["AzureStorageAccountName"];
            if (AzureStorageAccountName == null)
            {
                throw new DuradosException("Please add the AzureStorageAccountName to the web.config.");
            }

            ConfigAzureStorageAccountName = System.Configuration.ConfigurationManager.AppSettings["ConfigAzureStorageAccountName"];
            if (ConfigAzureStorageAccountName == null)
            {
                throw new DuradosException("Please add the ConfigAzureStorageAccountName to the web.config.");
            }

            AzureStorageAccountKey = System.Configuration.ConfigurationManager.AppSettings["AzureStorageAccountKey"];
            if (AzureStorageAccountKey == null)
            {
                throw new DuradosException("Please add the AzureStorageAccountKey to the web.config.");
            }

            ConfigAzureStorageAccountKey = System.Configuration.ConfigurationManager.AppSettings["ConfigAzureStorageAccountKey"];
            if (ConfigAzureStorageAccountKey == null)
            {
                throw new DuradosException("Please add the ConfigAzureStorageAccountKey to the web.config.");
            }


            AzureStorageUrl = System.Configuration.ConfigurationManager.AppSettings["AzureStorageUrl"] ?? "http://{0}.blob.core.windows.net/{1}";


            AzureCacheAccountKey = System.Configuration.ConfigurationManager.AppSettings["AzureCacheAccountKey"];
            if (AzureCacheAccountKey == null)
            {
                throw new DuradosException("Please add the AzureCacheAccountKey to the web.config.");
            }

            AzureCacheAccountName = System.Configuration.ConfigurationManager.AppSettings["AzureCacheAccountName"];
            if (AzureCacheAccountName == null)
            {
                throw new DuradosException("Please add the AzureCacheAccountName to the web.config.");
            }

            AzureCacheUrl = System.Configuration.ConfigurationManager.AppSettings["AzureCacheUrl"];
            if (AzureCacheUrl == null)
            {
                throw new DuradosException("Please add the AzureCacheUrl to the web.config.");
            }

         

            AzureCachePort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AzureCachePort"] ?? "22233");

            AzureCacheUpdateInterval = new TimeSpan(0, 0, Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AzureCacheUpdateInterval"] ?? "60"));

            DefaultUploadName = System.Configuration.ConfigurationManager.AppSettings["DefaultUploadName"] ?? "DefaultUpload";

            DefaultImageHeight = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DefaultImageHeight"] ?? "80");

            SplitProducts = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["SplitProducts"] ?? "true");

            ProductsPort = new Dictionary<SqlProduct, int>();

            foreach (SqlProduct sqlProduct in Enum.GetValues(typeof(SqlProduct)))
            {
                ProductsPort.Add(sqlProduct, Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings[sqlProduct.ToString() + "Port"] ?? "80"));
            }

            ApiUrls = (System.Configuration.ConfigurationManager.AppSettings["apiUrls"] ?? string.Empty).Split(';');

            UserPreviewUrl = System.Configuration.ConfigurationManager.AppSettings["UserPreviewUrl"] ?? ".backand.loc:4012/";

            S3Bucket = System.Configuration.ConfigurationManager.AppSettings["S3Bucket"] ?? "hosting.backand.net";

        }

        public static string GetConfigPath(string filename)
        {
            //if (ConfigPath.StartsWith("~"))
            //    return System.Web.HttpContext.Current.Server.MapPath(ConfigPath + filename);
            //else 
            //    return ConfigPath + filename.Replace('/','\\');

            return GetConfigPath(filename, ConfigPath);
        }

        public static string GetConfigPath(string filename, string configPath)
        {
            if (configPath.StartsWith("~"))
                return System.Web.HttpContext.Current.Server.MapPath(configPath + filename);
            else
                return configPath + filename.Replace('/', '\\');
        }

        public static string GetConfigPath(string filename, SqlProduct sqlProduct)
        {
            string key = sqlProduct.ToString() + "ConfigPath";
            string configPath = System.Configuration.ConfigurationManager.AppSettings[key] ?? ConfigPath;

            return GetConfigPath(filename, configPath);
        }

        public static string GetUploadPath(SqlProduct sqlProduct)
        {
            string key = sqlProduct.ToString() + "UploadPath";
            string uploadPath = System.Configuration.ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(uploadPath))
                return System.Web.HttpContext.Current.Server.MapPath("/Uploads/");
            else if (uploadPath.StartsWith("~"))
                return System.Web.HttpContext.Current.Server.MapPath(uploadPath);
            else
                return uploadPath;
        }

        public static string GetDeploymentPath(string filename)
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/Deployment/") + filename;

        }

        public static string Version = null;

        public static Dictionary<SqlProduct, int> ProductsPort { get; private set; }
        public static bool SplitProducts { get; private set; }
        public static string AzureStorageUrl { get; private set; }
        public static string AzureStorageAccountName { get; private set; }
        public static string AzureStorageAccountKey { get; private set; }
        public static string DefaultUploadName { get; private set; }
        public static int DefaultImageHeight { get; private set; }
        public static string ConfigAzureStorageAccountName { get; private set; }
        public static string ConfigAzureStorageAccountKey { get; private set; }
        public static string[] ApiUrls { get; private set; }

        public static string AzureCacheAccountKey { get; private set; }
        public static string AzureCacheAccountName { get; private set; }
        public static string AzureCacheUrl { get; private set; }
        public static int AzureCachePort { get; private set; }
        public static string UserPreviewUrl { get; private set; }
        public static string S3Bucket { get; private set; }

        public static TimeSpan AzureCacheUpdateInterval { get; private set; }

        public static string ConfigPath { get; private set; }

        public static bool DownloadDenyPolicy { get; private set; }
        public static string[] AllowedDownloadFileTypes { get; private set; }
        public static string[] DenyDownloadFileTypes { get; private set; }

        private Durados.Data.ICache<Map> mapsCache = null;
        public static Dictionary<string, string> DnsAliases = null;
        private IPersistency persistency = null;
        private static bool multiTenancy = false;
        private static string duradosAppPrefix;
        private static string duradosAppSysPrefix;
        private static bool cloud = false;
        private static bool skin = false;
        private static bool useSecureConnection = false;
        private static bool debug = false;
        private static bool dropAppDatabase = true;
        private static int appNameMax = 32;
        private static string host = "durados.com";
        private static int poolCreator = 5555;
        private static bool poolShouldBeUsed = false;

        private static string redisConnectionString = "";

        private static string mainAppConfigName = "backand";
        private static bool hostByUs = false;
        private static string duradosAppName = "www";
        private static string demoAzureUsername = "itayher";
        private static string demoAzurePassword = "Durados2012";
        private static string demoSqlUsername = "durados";
        private static string demoSqlPassword = "durados";
        private static string demoDatabaseName = "Northwind";
        private static string demoConfigFilename = "Northwind";
        private static string demoAzureServer = "tcp:d9gwdrhh5n.database.windows.net,1433";
        private static string demoOnPremiseServer = @"durados.info\sqlexpress";
        private static bool demoCreatePending = true;
        private static int demoPendingNext = 5;
        private static string demoFtpTempHost = "temp";
        private static string demoFtpHost = "durados.info";
        private static string demoFtpPort = "21";
        private static long demoFtpFileSizeLimitKb = 1024;
        private static long demoFtpFolderSizeLimitKb = 1024;
        private static string demoFtpPhysicalPath = @"C:\FTP\";
        private static string demoUploadSourcePath = @"C:\Dev\Demo\";
        private static string demoOnPremiseSourcePath = @"C:\Dev\Databases\";
        private static string demoFtpUser = "itay";
        private static string demoFtpPassword = "dio2008";
        private static bool allowLocalConnection = false;
        private static PendingPool azureDatabasePendingPool;
        private static PendingPool onPremiseDatabasePendingPool;
        private static string PandingDatabaseSuffix = "Pending";
        private static bool privateCloud = false;

        private static bool downloadDenyPolicy = true;
        private static string allowedDownloadFileTypesDefault = "jpg,png,gif,pdf,docx,doc,xls,xlsx,pptx,ppt";
        private static string denyDownloadFileTypesDefault = "ade,adp,app,bas,bat,chm,cmd,cpl,crt,csh,exe,fxp,hlp,hta,inf,ins,isp,ksh,Lnk,mda,mdb,mde,mdt,mdt,mdw,mdz,msc,msi,msp,mst,ops,pcd,pif,prf,prg,pst,reg,scf,scr,sct,shb,shs,url,vb,vbe,vbs,wsc,wsf,wsh,config,dll";
        private static string downloadActionName = "Download";
        private static string azureAppPrefix = "app";

        private static int plugInSampleGenerationCount = 5;
        private static string superDeveloper = "dev@devitout.com";
        private static string adminButtonText = "Admin";
        private static string publicButtonText = "Public";
        public static bool OldAdminHttp = false;

        private static string reservedAppNames = "api";

        private Map duradosMap = null;
        System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();


        public static string GetAdminButtonText()
        {
            return adminButtonText;
        }

        public static string GetPublicButtonText()
        {
            return publicButtonText;
        }

        public static string GetAdminButtonUrl(Map map)
        {
            return "/Home/Default?workspaceId=" + map.Database.GetAdminWorkspaceId() + "&menuId=10001";
        }

        public static string GetPublicButtonUrl(Map map)
        {
            return "/Home/Default?workspaceId=" + map.Database.GetPublicWorkspaceId();
        }

        public static bool IsSuperDeveloper(string userName)
        {
            if (string.IsNullOrEmpty(userName) && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.Name != null)
                userName = HttpContext.Current.User.Identity.Name;
            return !string.IsNullOrEmpty(userName) && userName.Equals(SuperDeveloper);
        }

        protected virtual void InitPersistency()
        {
            if (multiTenancy)
            {


                IPersistency sqlPersistency = GetNewPersistency();
                sqlPersistency.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MapsConnectionString"].ConnectionString;
                if (System.Configuration.ConfigurationManager.ConnectionStrings["SystemMapsConnectionString"] == null)
                    throw new DuradosException("Please add SystemMapsConnectionString to the web.config connection strings");
                sqlPersistency.SystemConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SystemMapsConnectionString"].ConnectionString;

                persistency = sqlPersistency;
                builder.ConnectionString = sqlPersistency.ConnectionString;
                
                Durados.DataAccess.ConfigAccess.storage = new Map();
                Durados.DataAccess.ConfigAccess.multiTenancy = multiTenancy;
                Durados.DataAccess.ConfigAccess.cloud = cloud;
                

            }
        }



        public static string GetPendingDatabase(string template)
        {
            string next;
            if (template == "1")
                next = azureDatabasePendingPool.Next().ToString();
            else
                next = onPremiseDatabasePendingPool.Next().ToString();
            return demoDatabaseName + PandingDatabaseSuffix + next;
        }



        public virtual IPersistency GetNewPersistency()
        {
            return new SqlPersistency();
        }

        public static bool PrivateCloud
        {
            get
            {
                return privateCloud;
            }
        }

        public static bool DropAppDatabase
        {
            get
            {
                return dropAppDatabase;
            }
        }

        public static bool AllowLocalConnection
        {
            get
            {
                return allowLocalConnection;
            }
        }

        public static string DemoFtpPhysicalPath
        {
            get
            {
                return demoFtpPhysicalPath;
            }
        }

        public static string DemoUploadSourcePath
        {
            get
            {
                return demoUploadSourcePath;
            }
        }


        public static string DemoOnPremiseSourcePath
        {
            get
            {
                return demoOnPremiseSourcePath;
            }
        }

        public static string DemoFtpPassword
        {
            get
            {
                return demoFtpPassword;
            }
        }

        public static string DemoFtpUser
        {
            get
            {
                return demoFtpUser;
            }
        }

        public static string DemoFtpHost
        {
            get
            {
                return demoFtpHost;
            }
        }
        public static int PoolCreator
        {
            get
            {
                return poolCreator;
            }
        }

        public static string RedisConnectionString
        {
            get
            {
                return redisConnectionString;
            }
        }

        
        public static bool PoolShouldBeUsed
        {
            get
            {
                return poolShouldBeUsed;
            }
        }




        public static string DemoFtpPort
        {
            get
            {
                return demoFtpPort;
            }
        }

        public static long DemoFtpFileSizeLimitKb
        {
            get
            {
                return demoFtpFileSizeLimitKb;
            }
        }

        public static long DemoFtpFolderSizeLimitKb
        {
            get
            {
                return demoFtpFolderSizeLimitKb;
            }
        }

        public static string DemoFtpTempHost
        {
            get
            {
                return demoFtpTempHost;
            }
        }

        public static int DemoPendingNext
        {
            get
            {
                return demoPendingNext;
            }
        }

        public static bool DemoCreatePending
        {
            get
            {
                return demoCreatePending;
            }
        }

        public static string DemoConfigFilename
        {
            get
            {
                return demoConfigFilename;
            }
        }

        public static string DemoDatabaseName
        {
            get
            {
                return demoDatabaseName;
            }
        }

        public static string DemoAzureUsername
        {
            get
            {
                return demoAzureUsername;
            }
        }

        public static string DemoAzurePassword
        {
            get
            {
                return demoAzurePassword;
            }
        }

        public static string DemoSqlUsername
        {
            get
            {
                return demoSqlUsername;
            }
        }

        public static string DemoSqlPassword
        {
            get
            {
                return demoSqlPassword;
            }
        }

        public static string DemoAzureServer
        {
            get
            {
                return demoAzureServer;
            }
        }

        public static string DemoOnPremiseServer
        {
            get
            {
                return demoOnPremiseServer;
            }
        }

        public static string Host
        {
            get
            {
                return host;
            }
        }

        public static bool HostByUs
        {
            get
            {
                return hostByUs;
            }
        }

        public static string DuradosAppName
        {
            get
            {
                return duradosAppName;
            }
        }

        public static bool Debug
        {
            get
            {
                return debug;
            }
        }

        public static bool UseSecureConnection
        {
            get
            {
                return useSecureConnection;
            }
        }

        public static int AppNameMax
        {
            get
            {
                return appNameMax;
            }
        }

        public static string SuperDeveloper
        {
            get
            {
                return superDeveloper;
            }
        }


        public static int PlugInSampleGenerationCount
        {
            get
            {
                return plugInSampleGenerationCount;
            }
        }

        public string ConnectionString
        {
            get
            {
                return persistency.ConnectionString;
            }
        }
        public string SystemConnectionString
        {
            get
            {
                return persistency.SystemConnectionString;
            }
        }

        public static bool MultiTenancy
        {
            get
            {
                return multiTenancy;
            }
        }

        public static bool Skin
        {
            get
            {
                return skin;
            }
        }

        public static string DuradosAppSysPrefix
        {
            get
            {
                return duradosAppSysPrefix;
            }
        }

        public static string DuradosAppPrefix
        {
            get
            {
                return duradosAppPrefix;
            }
        }

        public static bool Cloud
        {
            get
            {
                return cloud;
            }
        }

        public static string DownloadActionName
        {
            get
            {
                return downloadActionName;
            }
        }
        public static string AzureAppPrefix
        {
            get
            {
                return azureAppPrefix;
            }
        }

        public static string GetCurrentAppName()
        {
            if (System.Web.HttpContext.Current == null)
            {
                return null;
            }
            if (System.Web.HttpContext.Current.Items.Contains(Database.AppName))
            {
                return System.Web.HttpContext.Current.Items[Database.AppName].ToString();
            }

            if (System.Web.HttpContext.Current == null)
                throw new DuradosException("System.Web.HttpContext.Current is null");
            if (System.Web.HttpContext.Current.Request == null)
                throw new DuradosException("System.Web.HttpContext.Current.Request is null");
            if (System.Web.HttpContext.Current.Request.Headers == null)
                throw new DuradosException("System.Web.HttpContext.Current.Request.Headers is null");
            if (System.Web.HttpContext.Current.Request.Headers["Host"] == null)
                throw new DuradosException("System.Web.HttpContext.Current.Request.Headers[\"Host\"] is null");

            string headersHost = System.Web.HttpContext.Current.Request.Headers["Host"];
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();

            if (headersHost.ToLower().Contains(host.ToLower()))
            {

                return headersHost.Replace("." + host, string.Empty).Replace(":" + port, string.Empty);
            }
            else if (DnsAliases.ContainsKey(headersHost.ToLower().Replace(":" + port, string.Empty)))
            {
                return DnsAliases[headersHost.ToLower().Replace(":" + port, string.Empty)];
            }
            else
                return null;
        }

        public Map DuradosMap
        {
            get
            {
                return duradosMap;
            }
        }

        public Map GetMap()
        {
            if (!multiTenancy)
            {
                if (this.map == null)
                {
                    this.map = new Map();
                    this.map.Initiate(false);
                }

                return this.map;
            }

            return GetMap(GetAppName());
        }

        string prevAppName = null;
        public string GetAppName()
        {
            try
            {
                if (System.Web.HttpContext.Current == null)
                {
                    return null;
                }

                //int l = System.Web.HttpContext.Current.Request.Url.Segments.Length;

                //if (l > 3)
                //{

                //    if (System.Web.HttpContext.Current.Items.Contains("xxxzzzzzzzzz") && System.Web.HttpContext.Current.Request.Url.Segments[l - 2] == "myAppConnection/" && System.Web.HttpContext.Current.Request.HttpMethod == "POST")
                //    {
                //        return System.Web.HttpContext.Current.Request.Url.Segments[l - 1];
                //    }
                //}

                if (System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                {
                    return System.Web.HttpContext.Current.Items[Database.AppName].ToString();
                }

                string logText = "OriginalString: " + System.Web.HttpContext.Current.Request.Url.OriginalString + "; host:" + System.Web.HttpContext.Current.Request.Headers["Host"] + "; Referer: " + System.Web.HttpContext.Current.Request.Headers["Referer"];
                string appName = GetCurrentAppName();
                if (appName == null || prevAppName == null || !appName.Equals(prevAppName))
                    DuradosMap.Logger.Log("Maps", "GetAppName", appName ?? string.Empty, null, 170, logText);
                prevAppName = appName;
                return appName;
            }
            catch (Exception exception)
            {
                DuradosMap.Logger.Log("Maps", "GetMap", "GetAppName", exception, 5, "");
                return null;
            }
        }

        private Map map = null;

        private View GetAppView()
        {
            return (View)duradosMap.Database.Views["durados_App"];
        }

        /***Return - Plugin Type (Id) or 0 if value wasn't set or exist*/
        private int GetPluginType(int appId)
        {
            SqlAccess sql = new SqlAccess();

            string sSqlCommand = "SELECT     dbo.durados_PlugInSite.PlugInId ";
            sSqlCommand += "from  dbo.durados_PlugInSite with(nolock), dbo.durados_PlugInSiteApp with(nolock) ";
            sSqlCommand += "where dbo.durados_PlugInSite.Id = dbo.durados_PlugInSiteApp.PlugInSiteId ";
            sSqlCommand += " and  dbo.durados_PlugInSiteApp.AppId = " + appId + " ";

            object scalar = sql.ExecuteScalar(duradosMap.connectionString, sSqlCommand);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(scalar);
        }

        public int GetPluginSiteId(int appId)
        {
            SqlAccess sql = new SqlAccess();

            string sSqlCommand = "SELECT     dbo.durados_PlugInSiteApp.PlugInSiteId ";
            sSqlCommand += "from  dbo.durados_PlugInSiteApp with(nolock) ";
            sSqlCommand += " where  dbo.durados_PlugInSiteApp.AppId = " + appId + " ";

            object scalar = sql.ExecuteScalar(duradosMap.connectionString, sSqlCommand);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(scalar);
        }
        private View GetDnsAliasView()
        {
            return (View)duradosMap.Database.Views["durados_DnsAlias"];
        }


        public void Rename(string oldAppName, string newAppName)
        {
            Map map = null;
            if (mapsCache.ContainsKey(oldAppName))
            {
                map = mapsCache[oldAppName];
                RemoveMap(oldAppName);
            }
            else if (mapsCache.ContainsKey(oldAppName.ToLower()))
            {
                map = mapsCache[oldAppName.ToLower()];
                RemoveMap(oldAppName.ToLower());
            }
            if (map != null)
            {
                map.AppName = newAppName;
                AddMap(newAppName, map);
            }
        }

        public void Restart(string pk)
        {
            //try
            //{

            //    //string filename = GetMap().ConfigFileName;
            //    string key = "duradosappsys";//GetStorageBlobName(filename);
            //    int? id = AppExists(pk);
            //    if (id.HasValue)
            //        key += id.Value;
            //    else
            //    {
            //        id = AppExists(pk.ToLower());
            //        if (id.HasValue)
            //            key += id.Value;
            //    }
            //    if (id.HasValue)
            //    {
            //        StorageCache.Remove(key);
            //        key = key + "xml";
            //        StorageCache.Remove(key);
            //    }
            //}
            //catch { }

            mapsCache.Remove(pk);
            mapsCache.Remove(pk.ToLower());

            try
            {
                RemoveSqlProduct(pk);
            }
            catch { }
            //GetMap(pk);
        }

        public void Delete(string pk)
        {
            if (mapsCache.ContainsKey(pk))
            {
                RemoveMap(pk);
            }
        }

        public Map GetMap(string appName)
        {
            if (appName == null || ReservedAppNames.Contains(appName))
            {
                return duradosMap;
            }

            if (appName == DuradosAppName)
            {
                return duradosMap;
            }

            Map map = null;

            if (IsInMemoryMode())
            {
                if (mapsCache.ContainsKey(appName + GetInMemoryKey()))
                {
                    map = mapsCache[appName + GetInMemoryKey()];
                }
            }
            else if (mapsCache.ContainsKey(appName))
            {
                map = mapsCache[appName];
            }


            //else if (maps.ContainsKey(appName.ToLower()))
            //{
            //    map = maps[appName.ToLower()];
            //}
            

            if (map == null)
            {
                bool newStructure = false;
                try
                {
                    map = CreateMap(appName, out newStructure);
                }
                catch (Exception)
                {
                    throw new AppNotReadyException(appName);
                }

                // app not exist
                if(map == null)
                {
                    return null;
                }

                //todo: check null return

                if (!newStructure)
                {
                    if (IsInMemoryMode())
                    {
                        AddMap(appName + GetInMemoryKey(), map);
                    }
                    else
                    {
                        AddMap(appName, map);
                    }
                }
            }

            return map;
        }

        public static bool IsApi2()
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
            return !string.IsNullOrEmpty(s);
        }

        public bool IsApi()
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
            return !string.IsNullOrEmpty(s);
        }

        private bool BlobExists(string appName)
        {
            int? appId = AppExists(appName);
            if (!appId.HasValue)
                return false;

            if (Maps.Instance.AppInCach(appName))
                return true;

            bool blobExists = (new Durados.Web.Mvc.Azure.DuradosStorage()).Exists(Maps.GetConfigPath(Maps.DuradosAppPrefix + appId.ToString() + ".xml"));

            if (!blobExists)
                return false;

            return true;
        }

        private Map GetMapFromSession(Map map)
        {

            //if (IsPreviewModeOff)
            //{
            //    SetPreviewModeOff();
            //}
            /*
            else
            {
                if (IsPreviewModeOn && HttpContext.Current.Session["AdminPreviewMap"] == null)
                {
                    Map mapCopy = null;

                    DateTime started = DateTime.Now;
                    mapCopy = Maps.Instance.CreateMap(map.AppName);
                    TimeSpan span = DateTime.Now.Subtract(started);
                    double ms = span.TotalMilliseconds;

                    //started = DateTime.Now;
                    //mapCopy = GenericCopier<Map>.DeepCopy(map);
                    //span = DateTime.Now.Subtract(started);
                    //ms = span.TotalMilliseconds;

                    HttpContext.Current.Session["AdminPreviewMap"] = mapCopy;
                }
                if (HttpContext.Current.Session["AdminPreviewMap"] != null)
                {
                    map = (Map)HttpContext.Current.Session["AdminPreviewMap"];

                }
            }
            */
            return map;
        }

        //private void SetPreviewModeOff()
        //{
        //    //if (HttpContext.Current.Session == null) return;

        //    //if (HttpContext.Current.Session["AdminPreviewMap"] != null)// out of preview mode - remove Map from session
        //    //{
        //    //    HttpContext.Current.Session["AdminPreviewMap"] = null;
        //    //}
        //}

        //public bool IsPreviewModeOff
        //{
        //    get
        //    {
        //        string actionName = GetActionName();
        //        return string.IsNullOrEmpty(actionName) || actionName == "IndexPage" || actionName == "InlineEditingEdit" || actionName == "PreviewModeOff" || actionName == "Edit";

        //    }
        //}

        //public bool IsPreviewModeOn
        //{
        //    get
        //    {
        //        string actionName = GetActionName();
        //        return !string.IsNullOrEmpty(actionName) && actionName == "PreviewEdit";

        //    }
        //}


        public HttpContextBase httpContext
        {
            get
            {
                HttpContextWrapper context =
                    new HttpContextWrapper(System.Web.HttpContext.Current);
                return (HttpContextBase)context;
            }
        }

        public string GetActionName()
        {
            string url = HttpContext.Current.Request.RawUrl;
            System.Web.Routing.RouteData route = System.Web.Routing.RouteTable.Routes.GetRouteData(httpContext);
            if (route == null)
                return string.Empty;
            System.Web.Mvc.UrlHelper urlHelper = new System.Web.Mvc.UrlHelper(new System.Web.Routing.RequestContext(httpContext, route));

            var routeValueDictionary = urlHelper.RequestContext.RouteData.Values;

            if (!routeValueDictionary.ContainsKey("action"))
                return string.Empty;

            string actionName = routeValueDictionary["action"].ToString();
            return actionName;
        }

        PortManager portManager = new PortManager(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["startSshTunnelPort"] ?? "10000"), Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["endSshTunnelPort"] ?? "63000"));

        private Map CreateMap(string appName, out bool newStructure)
        {
            //Durados.Diagnostics.EventViewer.WriteEvent("Start CreateMap for: " + appName);

            newStructure = false;
            View appView = GetAppView();
            Field idField = appView.Fields["Id"];

            int? id = AppExists(appName, null);
            
            if (!id.HasValue)
            {
                Durados.Diagnostics.EventViewer.WriteEvent("CreateMap: could not find is for: " + appName);
                return null;
            }
            
            // if you are here, your application exist but don't have already created map
            MapDataSet.durados_AppRow appRow = null;
            try
            {
                appRow = (MapDataSet.durados_AppRow)appView.GetDataRow(idField, id.Value.ToString(), false);
            }
            catch (Exception exception)
            {
                Durados.Diagnostics.EventViewer.WriteEvent("failed to GetDataRow for id: " + id.Value, exception);
                try
                {
                    ((DuradosMap)Maps.Instance.DuradosMap).AddSslAndAahKeyColumn();
                    appRow = (MapDataSet.durados_AppRow)appView.GetDataRow(idField, id.Value.ToString(), false);
                }
                catch (Exception exception2)
                {
                    //Durados.Diagnostics.EventViewer.WriteEvent(exception2);
                }
            }

            if (appRow == null)
            {
                return null;
            }

            //Durados.Diagnostics.EventViewer.WriteEvent("appRow found for: " + appName + " id: " + id, System.Diagnostics.EventLogEntryType.SuccessAudit, 500);

            Map map = new Map();

            map.PlugInId = GetPluginType((int)id);/***Return - Plugin Type (Id) or 0 if value wasn't set or exist*/

            //map.connectionString = persistency.GetConnection(appRow, builder).ToString();

            int sqlProduct = 1;
            int systemSqlProduct = 1;
            if (appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection == null)
                throw new DuradosException("The app " + appName + " is not connected. Please connect your app.");

            if (!appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSqlProductIdNull())
                sqlProduct = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SqlProductId;

            if (!appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System.IsSqlProductIdNull())
                systemSqlProduct = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System.SqlProductId;

            map.SqlProduct = (SqlProduct)sqlProduct;
            map.SystemSqlProduct = (SqlProduct)systemSqlProduct;
            if (appsSqlProducts.ContainsKey(appName))
            {
                appsSqlProducts.Remove(appName);
            }
            try
            {
                appsSqlProducts.Add(appName, (SqlProduct)sqlProduct);
            }
            catch { }

            int localPort = 0;
            if (sqlProduct == (int)SqlProduct.MySql)
            {
                if (appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshUsesNull() || !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshUses)
                    localPort = 3306;
                else
                    localPort = AssignLocalPort();
            }
            else if (sqlProduct == (int)SqlProduct.Postgre)
            {
                localPort = Convert.ToInt32(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.ProductPort ?? "5432");
            }
            else if (sqlProduct == (int)SqlProduct.Oracle)
            {
                localPort = Convert.ToInt32(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.ProductPort ?? "1521");
            }

            map.LocalPort = localPort;

            if (sqlProduct == 3)
            {
                map.connectionString = persistency.GetMySqlConnection(appRow, builder, localPort).ToString();
            }
            else if (sqlProduct == 4)
            {
                map.connectionString = persistency.GetPostgreConnection(appRow, builder, localPort).ToString();
            }
            else if (sqlProduct == 5)
            {
                map.connectionString = persistency.GetOracleConnection(appRow, builder, localPort).ToString();
            }
            else
            {
                map.connectionString = persistency.GetSqlServerConnection(appRow, builder).ToString();
            }
            map.systemConnectionString = persistency.GetSystemConnection(appRow, builder).ToString();
            map.securityConnectionString = Convert.ToString(persistency.GetSecurityConnection(appRow, builder));
            map.Logger.ConnectionString = persistency.GetLogConnection(appRow, builder).ToString();
            string pk = appRow.Id.ToString();
            map.Id = pk;
            map.DatabaseName = appRow != null && appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection != null && !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsCatalogNull() ? appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.Catalog : "Yours";

            map.ConfigFileName = Maps.GetConfigPath(Maps.DuradosAppPrefix + pk + ".xml");
            if (!appRow.IsUsesSpecificBinaryNull() && appRow.UsesSpecificBinary)
                // map.selectedProject = string.Format("Durados.Web.Mvc.Specifics.{0}.{0}Project, Durados.Web.Mvc.Specifics.{0}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Maps.DuradosAppPrefix + pk);
                map.selectedProject = HttpContext.Current.Server.MapPath("/" + appRow.SpecificDOTNET);

            //Durados.Diagnostics.EventViewer.WriteEvent("connections set for: " + appName + " id: " + id);

            map.SiteInfo = new SiteInfo();
            if (appRow.IsTitleNull())
                map.SiteInfo.Product = string.Empty;
            else
                map.SiteInfo.Product = appRow.Title;
            map.SiteInfo.Logo = appRow.Image;


            map.UsingSsh = !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshUsesNull() && appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshUses;
            if (sqlProduct == (int)SqlProduct.MySql && map.UsingSsh)
            {
                Durados.Security.Ssh.ITunnel tunnel = new Durados.DataAccess.Ssh.Tunnel();
                tunnel.RemoteHost = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshRemoteHost;
                tunnel.User = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshUser;
                tunnel.Password = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshPasswordNull() ? null : appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshPassword;
                tunnel.Port = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshPort;
                tunnel.PrivateKey = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshPrivateKeyNull() ? null : appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshPrivateKey;

                map.SshSession = new Durados.DataAccess.Ssh.ChilkatSession(tunnel, Convert.ToInt32(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.ProductPort), localPort);
                //map.SshSession = new Durados.DataAccess.Ssh.TamirSession(tunnel, 3306);
                map.OpenSshSession();
            }

            bool firstTime = map.Initiate(false);
            ConfigAccess.Refresh(map.GetConfigDatabase().ConnectionString);

            map.AppName = appName;
            map.Url = GetAppUrl(appName);
            map.SiteInfo.LogoHref = map.Url;
            map.Guid = appRow.Guid;
            
            int themeId = 0;
            string themeName = "";
            string themePath = "";

            MapDataSet.durados_ThemeRow themeRow = appRow.durados_ThemeRow ?? GetDefaultTheme();

            themeId = themeRow.Id;
            themeName = themeRow.Name;
            if (themeRow.Id == CustomTheme)
            {
                themePath = appRow.IsCustomThemePathNull() ? string.Empty : appRow.CustomThemePath;
            }
            else
            {
                themePath = themeRow.RelativePath;
            }

            map.Theme = new Theme() { Id = themeId, Name = themeName, Path = themePath };
            if (firstTime && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["NotifyUserOnConsoleReady"] ?? "true"))
                map.NotifyUser("consoleFirstTimeSubject", "consoleFirstTimeMessage");
            
            RefreshMapDnsAlias(map);
            UpdatePlan(appRow.Id, map);

            newStructure = map.SaveChangesInConfigStructure();

            try
            {
                map.MenusBackwardCompatebility();
            }
            catch (Exception exception)
            {
                DuradosMap.Logger.Log("Map", "MenusBackwardCompatebility", appName ?? string.Empty, exception, 1, "Backward Compatebility Failed");
            }

            try
            {
                map.ChartsBackwardCompatebility();
            }
            catch (Exception exception)
            {
                DuradosMap.Logger.Log("Map", "ChartsBackwardCompatebility", appName ?? string.Empty, exception, 1, "Charts Backward Compatebility Failed");
            }

            return map;
        }

        private MapDataSet.durados_ThemeRow defaultThemeRow = null;

        public const int DefaultThemeId = 2;
        public const int CustomTheme = 1;

        private MapDataSet.durados_ThemeRow GetDefaultTheme()
        {

            if (defaultThemeRow == null)
                defaultThemeRow = GetTheme(DefaultThemeId);

            return defaultThemeRow;
        }

        public string GetAppThemePath(string appName)
        {
            if (string.IsNullOrEmpty(appName))
                return GetTheme().RelativePath;

            if (AppInCach(appName))
            {
                return this.GetMap(appName).Theme.Path;
            }

            int? themeId = null;
            string sql = "select ThemeId from durados_App where [Name]=@appName";
            try
            {

                string scalar = (new SqlAccess()).ExecuteScalar(duradosMap.connectionString, sql, new Dictionary<string, object>() { { "appName", appName } });
                if (!string.IsNullOrEmpty(scalar))
                    themeId = Convert.ToInt32(scalar);
            }
            catch
            {

            }
            if (themeId.Equals(CustomTheme))
            {
                sql = "select CustomThemePath from durados_App where [Name]=@appName";
                try
                {

                    string scalar = (new SqlAccess()).ExecuteScalar(duradosMap.connectionString, sql, new Dictionary<string, object>() { { "appName", appName } });
                    if (!string.IsNullOrEmpty(scalar))
                        return scalar;
                    else
                        return GetTheme(themeId).RelativePath;
                }
                catch
                {
                    return GetTheme(themeId).RelativePath;
                }
            }
            else
            {
                return GetTheme(themeId).RelativePath;
            }
        }

        public MapDataSet.durados_ThemeRow GetTheme(int? themeId = DefaultThemeId)
        {
            if (!themeId.HasValue)
                themeId = DefaultThemeId;
            return (MapDataSet.durados_ThemeRow)duradosMap.Database.Views["durados_Theme"].GetDataRow(themeId.ToString());
        }

        public int AssignLocalPort()
        {
            return portManager.Assign();
        }

        public void UpdatePlan(int appId, Map map)
        {
            string sql = "select top(1) PlanId from durados_AppPlan where AppId=" + appId + " order by PurchaseDate desc";
            try
            {
                string scalar = (new SqlAccess()).ExecuteScalar(duradosMap.connectionString, sql);
                if (string.IsNullOrEmpty(scalar))
                    map.Plan = 0;
                else
                    map.Plan = Convert.ToInt32(scalar);
            }
            catch
            {
                map.Plan = 0;
            }

        }

        private void RefreshMapDnsAlias(Map map)
        {
            int rowCount = 0;
            Dictionary<string, object> values = new Dictionary<string, object>();

            View dnsAliasView = GetDnsAliasView();

            DataView dnsAliasDataView = dnsAliasView.FillPage(1, 10000, values, null, null, out rowCount, null, null);
            map.Aliases.Clear();
            foreach (System.Data.DataRowView dnsAliasRow in dnsAliasDataView)
            {
                map.Aliases.Add(dnsAliasRow["Alias"].ToString());
            }
        }

        public bool AppInCach(string appName)
        {
            return mapsCache.ContainsKey(appName);
        }

        public string GetAppNameByGuid(string guid)
        {
            SqlAccess sql = new SqlAccess();
            string sSqlCommand = "";

            sSqlCommand = "select [name] from durados_App with(nolock) where [Guid] = '" + guid + "'";

            object scalar = sql.ExecuteScalar(duradosMap.connectionString, sSqlCommand);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return null;
            else
                return scalar.ToString();
        }

        public int? AppExists(string appName, int? userId = null)
        {
            SqlAccess sql = new SqlAccess();
            string sSqlCommand = "";

            if (!userId.HasValue)
            {
                sSqlCommand = "select Id from durados_App with(nolock) where Name = N'" + appName + "'";
            }
            else
            {
                sSqlCommand = "SELECT dbo.durados_App.Id FROM dbo.durados_App with(nolock), dbo.durados_UserApp with(nolock) where (dbo.durados_App.Name = N'" + appName + "' and ((dbo.durados_UserApp.UserId=" + userId + " and dbo.durados_UserApp.AppId = dbo.durados_App.Id) or dbo.durados_App.Creator=" + userId + ") ) group by(dbo.durados_App.Id)";
                /*"SELECT dbo.durados_App.Id FROM dbo.durados_App with(nolock) INNER JOIN dbo.durados_UserApp with(nolock) ON dbo.durados_App.Id = dbo.durados_UserApp.AppId WHERE (dbo.durados_App.Name = N'" + appName + "' and dbo.durados_UserApp.UserId = "+userId+")";*/
            }

            object scalar = sql.ExecuteScalar(duradosMap.connectionString, sSqlCommand);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return null;
            else
                return Convert.ToInt32(scalar);
        }

        public MapDataSet.durados_AppRow GetAppRow(int id)
        {
            View appView = GetAppView();
            Field idField = appView.Fields["Id"];
            return (MapDataSet.durados_AppRow)appView.GetDataRow(idField, id.ToString(), false);

        }

        public static string GetAppUrl(string appName, bool? fullUrl = null)
        {
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
            string appHost = host;
            if (System.Web.HttpContext.Current.Request.Url.ToString().Contains(port))
                appHost += ":" + port;

            //Prepare url format
            string urlFormat = System.Web.HttpContext.Current.Request.Url.Scheme + "://{0}.{1}";
            if (fullUrl == true)
            {
                urlFormat = "{0}.{1}|_blank|" + urlFormat;
            }

            string url = string.Format(urlFormat, appName, appHost);
            return url;
        }

        public static string GetMainAppUrl()
        {
            return GetAppUrl(duradosAppName);
        }

        public static string GetmainAppConfigName()
        {
            return mainAppConfigName;
        }


        public MapDataSet.v_durados_UserRow GetCreatorUserRow()
        {
            return GetCreatorUserRow(GetAppName());
        }

        public MapDataSet.durados_AppRow GetAppRow()
        {
            return GetAppRow(GetAppName());

        }

        public MapDataSet.durados_AppRow GetAppRow(string appName)
        {
            View appView = GetAppView();
            Field nameField = appView.Fields["Name"];

            return (MapDataSet.durados_AppRow)appView.GetDataRow(nameField, appName, false);
        }

        public MapDataSet.v_durados_UserRow GetCreatorUserRow(string appName)
        {
            MapDataSet.durados_AppRow appRow = GetAppRow(appName);
            return appRow.v_durados_UserRow;
        }


        public void AddMap(string appName, Map map)
        {
            if (!mapsCache.ContainsKey(appName))
                mapsCache.Add(appName, map);

        }

        public void UpdateCache(string appName, Map map)
        {
            mapsCache.Add(appName, map);

        }

        public void RemoveMap(string appName)
        {
            mapsCache.Remove(appName);
        }
        private void RemoveConfig(string filename)//DataSet ds,
        {
            if (Maps.Cloud)
            {
                try
                {
                    DeleteConfig(filename);
                }
                catch (Exception exception)
                {
                    Maps.Instance.duradosMap.Logger.Log("Map", "RemoveApp", "RemoveConfig", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                }

                return;
            }
            //if (!File.Exists(filename))
            //{
            //    try
            //    {
            //        DeleteConfig( filename);
            //    }
            //    catch (Exception exception)
            //    {
            //        Maps.Instance.duradosMap.Logger.Log("Map", "RemoveApp", "RemoveConfig", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
            //    }
            //}
        }

        private void DeleteConfig(string filename)
        {
            if (Maps.Cloud)
            {
                RemoveConfigFromCloud(filename);

            }
            else
            {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }
        public void RemoveConfigFromCloud(string filename)
        {
            string containerName = Maps.GetStorageBlobName(filename);

            CloudBlobContainer container = new Azure.DuradosStorage().GetContainer(containerName);
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["deleteFullContainerOndelete"] ?? "true"))
                container.Delete();
            else
            {
                //if (container.GetBlockBlobReference(filename).Exists())
                //    container.GetBlobReference(filename).DeleteIfExists();
                if (container.GetBlockBlobReference(containerName).Exists())
                    container.GetBlobReference(containerName).DeleteIfExists();
            }
            if (Maps.Instance.StorageCache.ContainsKey(containerName))
            {
                Maps.Instance.StorageCache.Remove(containerName);
            }
        }


        internal void ChangeName(string oldName, string newName)
        {
            if (mapsCache.ContainsKey(oldName))
            {
                Map map = mapsCache[oldName];
                if (mapsCache.ContainsKey(newName))
                    throw new DuradosException("The " + newName + " already exists in the dictionary.");
                AddMap(newName, map);
                RemoveMap(oldName);
            }

        }

        public int GetAppAcount()
        {
            int appCount = 4010;
            if (HttpContext.Current.Session["AppAcount"] != null)
                return Convert.ToInt16(HttpContext.Current.Session["AppAcount"]);
            try
            {
                using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MapsConnectionString"].ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT count(*) FROM dbo.durados_App a with(nolock) INNER JOIN dbo.durados_PlugInInstance p with(nolock) ON a.id = p.Appid WHERE Deleted =0 and p.selected=1";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        object scalar = command.ExecuteScalar();
                        appCount = Convert.ToInt16(scalar);
                        HttpContext.Current.Session["AppAcount"] = appCount;
                    }

                }
            }
            catch { }

            return appCount;

        }
        public string GetCurrentAppId()
        {
            return AppExists(GetCurrentAppName()).ToString();
        }

        public int? GetConnection(string server, string catalog, string username, string userId)
        {
            SqlAccess sql = new SqlAccess();

            object scalar = sql.ExecuteScalar(duradosMap.connectionString, string.Format("select Id from durados_SqlConnection where ServerName=N'{0}' and Catalog=N'{1}' and Username=N'{2}' and DuradosUser={3}", server, catalog, username, userId));

            if (string.Empty.Equals(scalar) || scalar == null || scalar == DBNull.Value)
                return null;
            else
                return Convert.ToInt32(scalar);
        }
        public static bool IsAlloweDownload(string virtualPath)
        {
            string extension = VirtualPathUtility.GetExtension(virtualPath).TrimStart('.');
            if (DownloadDenyPolicy)
            {
                return !DenyDownloadFileTypes.Contains(extension, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                return AllowedDownloadFileTypes.Contains(extension, StringComparer.OrdinalIgnoreCase);
            }

        }
        public Dictionary<PlugInType, PluginCache> PluginsCache { get; private set; }

        public static SqlProduct GetCurrentSqlProduct()
        {
            return GetSqlProduct(GetCurrentAppName());
        }

        private static Dictionary<string, SqlProduct> appsSqlProducts = new Dictionary<string, SqlProduct>();

        public static void RemoveSqlProduct(string appName)
        {
            if (appsSqlProducts.ContainsKey(appName))
                appsSqlProducts.Remove(appName);
        }

        public static void UpdateSqlProduct(string appName, SqlProduct sqlProduct)
        {
            RemoveSqlProduct(appName);
            appsSqlProducts.Add(appName, sqlProduct);
        }

        public static SqlProduct GetSqlProduct(string appName)
        {
            if (string.IsNullOrEmpty(appName) || appName == duradosAppName)
                return SqlProduct.SqlServer;

            if (!appsSqlProducts.ContainsKey(appName))
            {
                using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MapsConnectionString"].ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT dbo.durados_SqlConnection.SqlProductId FROM dbo.durados_App with(nolock) INNER JOIN dbo.durados_SqlConnection with(nolock) ON dbo.durados_App.SqlConnectionId = dbo.durados_SqlConnection.Id WHERE (dbo.durados_App.Name = @AppName)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AppName", appName);
                        object scalar = command.ExecuteScalar();
                        if (scalar == null || scalar == DBNull.Value)
                        {
                            return SqlProduct.SqlServer;
                        }

                        appsSqlProducts.Add(appName, (SqlProduct)scalar);
                    }
                }
            }

            return appsSqlProducts[appName];
        }



        public static string GetStorageBlobName(string filename)
        {
            System.IO.FileInfo fileInfo = new FileInfo(filename);
            string filenameOnly = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);

            return filenameOnly.Replace("_", "").Replace(".", "").ToLower();

        }

        Durados.Data.ICache<DataSet> storageCache = CacheFactory.CreateCache<DataSet>("storageCache");

        public Durados.Data.ICache<DataSet> StorageCache
        {
            get
            {
                return storageCache;
            }
        }

        BlobBackup backup = new BlobBackup();
        public BlobBackup Backup
        {
            get
            {
                return backup;
            }
        }

        FieldProperty fieldProperty = new FieldProperty();
        public FieldProperty FieldProperty
        {
            get
            {
                return fieldProperty;
            }
        }

        public static HashSet<string> ReservedAppNames { get; set; }

        public void UpdateOnBoardingStatus(OnBoardingStatus onBoardingStatus, string appId)
        {
            string sql = "Update durados_App set DatabaseStatus = " + (int)onBoardingStatus + " where id = " + appId;
            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

            sqlAccess.ExecuteNonQuery(DuradosMap.connectionString, sql);
        }

        public OnBoardingStatus GetOnBoardingStatus(string appId)
        {
            string sql = "select DatabaseStatus from dbo.durados_App with (NOLOCK) where id = " + appId;
            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

            object scalar = sqlAccess.ExecuteScalar(DuradosMap.connectionString, sql);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return OnBoardingStatus.NotStarted;
            else
                return (OnBoardingStatus)Convert.ToInt32(scalar);
        }
    }

    public class FieldProperty
    {
        private Type columnType = typeof(Durados.Web.Mvc.ColumnField);
        private Type parentType = typeof(Durados.Web.Mvc.ParentField);
        private Type childrenType = typeof(Durados.Web.Mvc.ChildrenField);

        //private Dictionary<string, Dictionary<string, bool>> properties = new Dictionary<string, Dictionary<string, bool>>();
        private Durados.Data.ICache<Durados.Data.ICache<bool>> properties = CacheFactory.CreateCache<Durados.Data.ICache<bool>>("properties");

        public bool IsInType(string fieldName, string fieldType)
        {
            Type type = GetFieldType(fieldType);

            return Has(type, fieldType, fieldName);
        }


        private bool Has(Type type, string fieldType, string fieldName)
        {
            if (!properties.ContainsKey(fieldType))
                properties.Add(fieldType, CacheFactory.CreateCache<bool>(fieldType));

            if (!properties[fieldType].ContainsKey(fieldName))
                properties[fieldType].Add(fieldName, type != null && type.GetProperty(fieldName) != null);

            return properties[fieldType][fieldName];
        }

        private Type GetFieldType(string fieldType)
        {
            if (fieldType == FieldType.Children.ToString())
                return childrenType;
            else if (fieldType == FieldType.Column.ToString())
                return columnType;
            else if (fieldType == FieldType.Parent.ToString())
                return parentType;
            else
                return null;
        }
    }

    public class PortManager
    {
        public HashSet<int> OfficialPorts { get; private set; }
        public int StartFromPort { get; private set; }
        public int LastAssigned { get; private set; }
        public int MaxPort { get; private set; }

        public PortManager(int startFromPort, int maxPort)
            : this(new HashSet<int>() { 10008,
10010,
10050,
10051,
10110,
10113,
10114,
10115,
10116,
11371,
12222,
12223,
13075,
13720,
13721,
13724,
13782,
13783,
13785,
13786,
15000,
15000,
15345,
17500,
17500,
18104,
19283,
19315,
19999,
20000,
22347,
22350,
24465,
24554,
25000,
25003,
25005,
25007,
25010,
26000,
26000,
31457,
31620,
33434,
34567,
40000,
43047,
43048,
45824,
47001,
47808,
49151
}, startFromPort, maxPort)
        {
        }

        public PortManager(HashSet<int> officialPorts, int startFromPort, int maxPort)
        {
            this.OfficialPorts = officialPorts;
            this.StartFromPort = startFromPort;
            this.LastAssigned = startFromPort;
            this.MaxPort = maxPort;
        }

        public int Assign()
        {
            if (LastAssigned >= MaxPort)
                LastAssigned = StartFromPort;
            int p = LastAssigned + 1;
            while (IsUsed(p) || IsOfficial(p))
            {
                if (LastAssigned >= MaxPort)
                    throw new NoMorePortsAvailableException();
                p++;
            }

            LastAssigned = p;

            return p;
        }

        private bool IsOfficial(int port)
        {
            return OfficialPorts.Contains(port);
        }


        private bool IsUsed(int port)
        {

            bool isUsed = false;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            System.Net.NetworkInformation.IPGlobalProperties ipGlobalProperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            System.Net.NetworkInformation.TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (System.Net.NetworkInformation.TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    isUsed = true;
                    break;
                }
            }

            return isUsed;
        }
    }


    public class NoMorePortsAvailableException : DuradosException
    {
        public NoMorePortsAvailableException()
            : base("There are no more ports available.")
        {
        }
    }

    public class PluginCache
    {
        private Dictionary<int, int> counters = null;
        private int maxCount;
        private int batch;
        private int remains;

        public PluginCache()
        {
            counters = new Dictionary<int, int>();
            maxCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["plugInMaxCount"] ?? "2");
            batch = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["plugInBatch"] ?? "4");
            remains = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["plugInRemains"] ?? "1");

        }

        private int GetCount(int sampleId)
        {
            if (!counters.ContainsKey(sampleId))
            {
                counters.Add(sampleId, 0);
            }

            counters[sampleId] += 1;

            return counters[sampleId];
        }

        public void Initiate(int sampleId)
        {
            if (!counters.ContainsKey(sampleId))
            {
                counters.Add(sampleId, 0);
            }

            counters[sampleId] = 0;

        }

        public bool IsPassMaxCount(int sampleId)
        {
            return GetCount(sampleId) >= maxCount;
        }

        public int Batch
        {
            get
            {
                return batch;
            }
        }

        public int Remains
        {
            get
            {
                return remains;
            }
        }
    }

    public interface IPersistency
    {
        string ConnectionString { get; set; }
        string SystemConnectionString { get; set; }
        object GetConnection(MapDataSet.durados_AppRow appRow, object builder);
        object GetSqlServerConnection(MapDataSet.durados_AppRow appRow, object builder);
        object GetMySqlConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort);
        object GetPostgreConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort);
        object GetOracleConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort);
        object GetSystemConnection(MapDataSet.durados_AppRow appRow, object builder);
        object GetSecurityConnection(MapDataSet.durados_AppRow appRow, object builder);
        object GetLogConnection(MapDataSet.durados_AppRow appRow, object builder);
    }

    public class SqlPersistency : IPersistency
    {
        public string ConnectionString { get; set; }
        public string SystemConnectionString { get; set; }


        public object GetConnection(MapDataSet.durados_AppRow appRow, object builder)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, builder);
        }

        public object GetSystemConnection(MapDataSet.durados_AppRow appRow, object builder)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            if (appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System == null)
                return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, builder);
            else
                return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System, dataSourceTypeId, builder);

        }
        public object GetSecurityConnection(MapDataSet.durados_AppRow appRow, object builder)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            if (appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_Security == null)
                return null;//System.Configuration.ConfigurationManager.ConnectionStrings["SecurityConnectionString"];
            else
                return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_Security, dataSourceTypeId, builder);

        }

        public object GetLogConnection(MapDataSet.durados_AppRow appRow, object builder)
        {
            return GetSystemConnection(appRow, builder);// +";MultipleActiveResultSets=True;Asynchronous Processing=true;";

        }

        public object GetConnection(MapDataSet.durados_SqlConnectionRow sqlConnectionRow, int dataSourceTypeId, object builder)
        {
            return GetConnection(sqlConnectionRow, dataSourceTypeId, (System.Data.SqlClient.SqlConnectionStringBuilder)builder);
        }



        public object GetConnection(MapDataSet.durados_SqlConnectionRow sqlConnectionRow, int dataSourceTypeId, System.Data.SqlClient.SqlConnectionStringBuilder builder)
        {
            return GetConnection(sqlConnectionRow, dataSourceTypeId, builder, GetConnectionStringTemplate(sqlConnectionRow));
        }

        private string GetConnectionStringTemplate(MapDataSet.durados_SqlConnectionRow sqlConnectionRow)
        {
            int sqlProductId = sqlConnectionRow.SqlProductId;
            if (((SqlProduct)sqlProductId) == SqlProduct.MySql)
                return ConnectionStringHelper.GetConnectionStringSchema(sqlConnectionRow);
            return "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;";
        }

        public object GetSqlServerConnection(MapDataSet.durados_AppRow appRow, object builder)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, (System.Data.SqlClient.SqlConnectionStringBuilder)builder, "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;");
        }

        public object GetMySqlConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            bool usesSsh = !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshUsesNull() && appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshUses;
            return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, (System.Data.SqlClient.SqlConnectionStringBuilder)builder, usesSsh ? "server=localhost;database={1};User Id={2};password={3};Allow User Variables=True;" : "server={0};database={1};User Id={2};password={3}") + ";port=" + localPort.ToString() + ";convert zero datetime=True;default command timeout=90;Connection Timeout=60;Allow User Variables=True;";
        }

        public object GetPostgreConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            bool usesSsl = !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSslUsesNull() && appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SslUses;
            return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, (System.Data.SqlClient.SqlConnectionStringBuilder)builder, usesSsl ? "server={0};database={1};User Id={2};password={3};SSL=true;SslMode=Require;" : "server={0};database={1};User Id={2};password={3}") + ";port=" + localPort.ToString() + ";Encoding=UNICODE;CommandTimeout=90;Timeout=60;";
        }

        public object GetOracleConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort)
        {
            int dataSourceTypeId = appRow.DataSourceTypeId;
            bool usesSsh = !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshUsesNull() && appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshUses;
            return GetConnection(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection, dataSourceTypeId, (System.Data.SqlClient.SqlConnectionStringBuilder)builder, OracleAccess.GetConnectionStringSchema());
        }
        public object GetConnection(MapDataSet.durados_SqlConnectionRow sqlConnectionRow, int dataSourceTypeId, System.Data.SqlClient.SqlConnectionStringBuilder builder, string template)
        {
            string connectionString = null;
            string serverName = null;
            bool? integratedSecurity = null;

            if (dataSourceTypeId == 2 || dataSourceTypeId == 4)
            {
                if (sqlConnectionRow.IsServerNameNull())
                    serverName = builder.DataSource;
                else
                    serverName = sqlConnectionRow.ServerName;

                if (sqlConnectionRow.IsIntegratedSecurityNull())
                    integratedSecurity = builder.IntegratedSecurity;
                else
                    integratedSecurity = sqlConnectionRow.IntegratedSecurity;
            }
            else
            {
                integratedSecurity = builder.IntegratedSecurity;
                serverName = builder.DataSource;
            }

            if (integratedSecurity.HasValue && integratedSecurity.Value)
            {
                connectionString = "Data Source={0};Initial Catalog={1};Integrated Security=True;";
                return string.Format(connectionString, serverName, sqlConnectionRow.Catalog);
            }
            else
            {
                //connectionString = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;";
                connectionString = template;
                string username = null;
                string password = null;
                if (dataSourceTypeId == 2 || dataSourceTypeId == 4)
                {

                    if (sqlConnectionRow.IsUsernameNull())
                        username = builder.UserID;
                    else
                        username = sqlConnectionRow.Username;
                    if (sqlConnectionRow.IsPasswordNull())
                        password = builder.Password;
                    else
                        password = sqlConnectionRow.Password;

                }
                else
                {
                    username = builder.UserID;
                    password = builder.Password;
                }
                if (!sqlConnectionRow.IsProductPortNull())
                    return string.Format(connectionString, serverName, sqlConnectionRow.Catalog, username, password, sqlConnectionRow.ProductPort);
                else
                    return string.Format(connectionString, serverName, sqlConnectionRow.Catalog, username, password);
            }
        }

    }

    public class DuradosProject : Durados.Web.Mvc.Config.Project
    {
        public override DataSet GetDataSet()
        {
            return new MapDataSet();
        }
    }

    public class DuradosMap : Map
    {
        public override bool IsMainMap
        {
            get
            {
                return true;
            }
        }

        protected override Durados.Web.Mvc.Config.IProject GetProject()
        {
            return new DuradosProject();
        }

        public override string GetLogOnPath()
        {
            string logon = System.Configuration.ConfigurationManager.AppSettings["MainLogOnPath"] ?? "~/Views/Account/LogOn.aspx";

            return logon;
        }

        public override bool GetLogMvc()
        {
            return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["MainLogOnMvc"] ?? "true");

        }

        public Dictionary<string, string> GetUserApps()
        {
            int rowCount = 0;
            Dictionary<string, object> values = new Dictionary<string, object>();

            Durados.View appsView = Database.Views["durados_App"];

            DataView dataView = null;
            try
            {
                dataView = appsView.FillPage(1, 10000, values, null, null, out rowCount, null, null);
            }
            catch (Exception exception)
            {
                AddSslAndAahKeyColumn();
                dataView = appsView.FillPage(1, 10000, values, null, null, out rowCount, null, null);
            }

            AddSslAndAahKeyColumnConfig();

            Dictionary<string, string> apps = new Dictionary<string, string>();
            foreach (System.Data.DataRowView row in dataView)
            {
                apps.Add(row["Id"].ToString(), row["Title"].ToString());
            }

            return apps;
        }

        public void AddSslAndAahKeyColumn()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    new SqlSchema().AddNewColumnToTable("durados_SqlConnection", "SslUses", DataType.Boolean, command);
                    new SqlSchema().AddNewColumnToTable("durados_SqlConnection", "SshPrivateKey", DataType.LongText, command);
                }
            }
        }

        public void AddSslAndAahKeyColumnConfig()
        {
            View connectionsView = (View)Database.Views["durados_SqlConnection"];

            if (!connectionsView.Fields.ContainsKey("SslUses"))
            {
                connectionsView.Fields.Add("SslUses", new ColumnField(connectionsView, connectionsView.DataTable.Columns["SslUses"]));
            }
            if (!connectionsView.Fields.ContainsKey("SshPrivateKey"))
            {
                connectionsView.Fields.Add("SshPrivateKey", new ColumnField(connectionsView, connectionsView.DataTable.Columns["SshPrivateKey"]));
            }
        }

        public Dictionary<string, string> GetUserApps(int userId)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            SqlAccess sqlAccess = new SqlAccess();

            DataTable table = sqlAccess.ExecuteTable(Maps.Instance.DuradosMap.Database.ConnectionString, "select * from durados_App where durados_app.[ToDelete]=0 AND  Creator = " + userId + " or id in (select durados_UserApp.AppId from durados_UserApp where durados_UserApp.UserId = " + userId + ") ", null, CommandType.Text);

            Dictionary<string, string> apps = new Dictionary<string, string>();
            foreach (System.Data.DataRow row in table.Rows)
            {
                apps.Add(row["Id"].ToString(), row["Name"].ToString());
            }

            return apps;
        }

        public virtual string GetPlanContent()
        {
            if (string.IsNullOrEmpty(Database.PlanContent) || Database.PlanContent == "<a title=\"Change plan\"><img class=\"plan\" onclick=\"setPlan(this)\" src=\"/Content/Images/Plan.png\"></a>")
            {
                return "<span class='plan' onclick='setPlan(this)'>Upgrade to Premium</span>";//"<img class='plan' onclick='setPlan(this)' src='/Content/Images/Plan.png'/>";
            }

            return Database.PlanContent;
        }

        public override string GetLogoSrc()
        {
            return string.Format("/Content/Images/{0}", this.Database.SiteInfo.Logo);
            //return string.Format("/Home/{0}/{1}?fieldName={2}&amp;fileName={3}&amp;pk='\'", DownloadActionName, "durados_App", "Image", this.Database.SiteInfo.Logo);
        }

        public override SqlProduct SqlProduct
        {
            get
            {
                return SqlProduct.SqlServer;
            }
            set
            {
            }
        }

        public override Guid Guid
        {
            get
            {
                return Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["masterGuid"] ?? Guid.Empty.ToString());
            }
            set
            {
                base.Guid = value;
            }
        }
    }

    public static class IEnumerableExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }
    public class BlobTransferAsyncState
    {
        public CloudBlob Blob;
        public Stream Stream;
        public DateTime Started;
        public bool Cancelled;
        public CloudBlobContainer Container;
        public string BlobName;
        public Map Map;

        public BlobTransferAsyncState(CloudBlob blob, Stream stream, CloudBlobContainer container, string blobName, Map map)
            : this(blob, stream, DateTime.Now, container, blobName, map)
        { }

        public BlobTransferAsyncState(CloudBlob blob, Stream stream, DateTime started, CloudBlobContainer container, string blobName, Map map)
        {
            Blob = blob;
            Stream = stream;
            Started = started;
            Cancelled = false;
            Container = container;
            BlobName = blobName;
            Map = map;
        }
    }

    public enum OnBoardingStatus
    {
        NotStarted = 0,
        Ready = 1,
        Processing = 2,
        Error = 3
    }
}
