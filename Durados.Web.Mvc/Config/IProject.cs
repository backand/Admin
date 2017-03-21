using System;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados;

namespace Durados.Web.Mvc.Config
{
    public interface IProject
    {
        DataSet GetDataSet();

        Map Map { get; set; }

        string ConnectionStringKey { get; }

        string SystemConnectionStringKey { get; }

        string ConfigFileNameKey { get; }

        void ConfigConfig(Durados.Web.Mvc.Database configDatabase, Durados.Web.Mvc.Database database, Durados.Web.Mvc.Database localizationDatabase);

        void ConfigLocalization(Durados.Web.Mvc.Database localizationDatabase);
    }


    public class Project : Durados.Web.Mvc.Config.IProject
    {
        private Map map;

        public Map Map
        {
            get
            {
                return map;
            }
            set
            {
                this.map = value;
            }
        }

        public virtual DataSet GetDataSet()
        {
            return new DataSet();
        }

        public virtual string ConfigFileNameKey
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string ConnectionStringKey
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string SystemConnectionStringKey
        {
            get { return ConnectionStringKey; }
        }

        public static string ViewOwenrRole { get { return "m_ViewOwnerRole"; } }// 

        //public abstract string ConnectionStringKey
        //{
        //    get;
        //}

        //public abstract string ConfigFileNameKey
        //{
        //    get;
        //}

        //private string GetDelimitedFields(IList<Field> fields)
        //{
        //    string s = string.Empty;

        //    foreach (Field field in fields)
        //    {
        //        s += field.Name + ",";
        //        s += field.DisplayName + ",";
        //    }

        //    return s.TrimEnd(',');
        //}

        private void CloneView(string name, Durados.Web.Mvc.Database configDatabase)
        {
            if (!configDatabase.Views.ContainsKey(name))
            {
                View baseView = (View)configDatabase.Views["View"];
                Durados.Web.Mvc.Config.View cloneView = new Durados.Web.Mvc.Config.View(baseView.DataTable, (Durados.Web.Mvc.Config.Database)baseView.Database, name);
                configDatabase.Views.Add(name, cloneView);
            }
        }

        public virtual void ConfigConfig(Durados.Web.Mvc.Database configDatabase, Durados.Web.Mvc.Database database, Durados.Web.Mvc.Database localizationDatabase)
        {
            try
            {
                CloneView("MenuOrganizerView", configDatabase);

                ConfigAdminMenu(configDatabase, database, localizationDatabase);

                View configView = (View)configDatabase.Views["View"];
                //configDatabase.Menu.Name = "Admin";
                //configDatabase.DenyConfigConfigRoles = "Manager,User";
                configDatabase.DisplayName = "Configuration";
                configDatabase.AllowConfigConfigRoles = "Developer,Admin";

                configDatabase.Views["Database"].DisplayColumn = "DisplayName";
                configDatabase.Views["Database"].AllowCreate = false;
                configDatabase.Views["Database"].AllowDelete = false;
                (configDatabase.Views["Database"] as View).DataDisplayType = DataDisplayType.Preview;
                (configDatabase.Views["Database"] as View).HideFilter = true;
                (configDatabase.Views["Database"] as View).HideToolbar = true;
                (configDatabase.Views["Database"] as View).HidePager = true;
                (configDatabase.Views["Database"] as View).DashboardWidth = "0";

                foreach (View view in configDatabase.Views.Values)
                {
                    SetViewRoles(view, "Developer");
                    view.Skin = SkinType.Skin6;
                    view.GridDisplayType = GridDisplayType.FitToWindowWidth;
                }

                foreach (Durados.Web.Mvc.View view in database.Views.Values.Where(v => v.SystemView))
                {
                    view.Skin = SkinType.Skin6;
                    view.GridDisplayType = GridDisplayType.FitToWindowWidth;
                }

                SetViewRoles(configDatabase.Views["Rule"], "Developer,Admin");
                SetViewRoles(configDatabase.Views["Workspace"], "Developer,Admin");
                SetViewRoles(configDatabase.Views["Category"], "Developer,Admin," + ViewOwenrRole);
                SetViewRoles(configDatabase.Views["Menu"], "Developer,Admin");
                if (configDatabase.Views.ContainsKey("UrlLink"))
                    SetViewRoles(configDatabase.Views["UrlLink"], "Developer,Admin");
                SetViewRoles(configDatabase.Views["Chart"], "Developer,Admin");
                configDatabase.Views["Chart"].AllowCreateRoles = "Developer";

                ((Durados.Web.Mvc.View)configDatabase.Views["Menu"]).OrdinalColumnName = "Ordinal";
                ((Durados.Web.Mvc.View)configDatabase.Views["Menu"]).DefaultSort = "Ordinal";
                configDatabase.Views["Menu"].Fields["Ordinal"].HideInCreate = true;
                configDatabase.Views["Menu"].Fields["Ordinal"].HideInEdit = false;
                configDatabase.Views["Menu"].Fields["Ordinal"].HideInTable = false;

                ((Durados.Web.Mvc.View)configDatabase.Views["Category"]).OrdinalColumnName = "Ordinal";
                ((Durados.Web.Mvc.View)configDatabase.Views["Category"]).DefaultSort = "Ordinal";
                configDatabase.Views["Category"].Fields["Ordinal"].HideInCreate = true;
                configDatabase.Views["Category"].Fields["Ordinal"].HideInEdit = false;
                configDatabase.Views["Category"].Fields["Ordinal"].HideInTable = false;
                configDatabase.Views["Category"].ColumnsInDialog = 1;

                ConfigDatabaseView(configDatabase, database);

                if (configDatabase.Views.ContainsKey("SiteInfo") && configDatabase.Views["SiteInfo"].Fields.ContainsKey("Logo"))
                    ((Durados.Web.Mvc.ColumnField)configDatabase.Views["SiteInfo"].Fields["Logo"]).Upload = new Upload() { Override = true, Title = "Logo", UploadFileType = UploadFileType.Other, UploadStorageType = UploadStorageType.File, UploadVirtualPath = "/Uploads/" };


                if (configDatabase.Views.ContainsKey("Localization"))
                {
                    configDatabase.Views["Localization"].Fields["ID"].HideInTable = true;
                    configDatabase.Views["Localization"].Fields["ID"].DisplayName = "Id";
                    if (configDatabase.Views["Localization"].Fields.ContainsKey("AddKeyIfMissing"))
                        configDatabase.Views["Localization"].Fields["AddKeyIfMissing"].DisplayName = "Add Key If Missing";
                    if (configDatabase.Views["Localization"].Fields.ContainsKey("ReturnKeyIfMissing"))
                        configDatabase.Views["Localization"].Fields["ReturnKeyIfMissing"].DisplayName = "Return Key If Missing";
                    if (configDatabase.Views["Localization"].Fields.ContainsKey("Prefix"))
                        configDatabase.Views["Localization"].Fields["Prefix"].DisplayName = "Prefix";
                    if (configDatabase.Views["Localization"].Fields.ContainsKey("Postfix"))
                        configDatabase.Views["Localization"].Fields["Postfix"].DisplayName = "Postfix";
                    if (configDatabase.Views["Localization"].Fields.ContainsKey("Title"))
                        configDatabase.Views["Localization"].Fields["Title"].DisplayName = "Title";
                    if (configDatabase.Views["Localization"].Fields.ContainsKey("Localization_Children"))
                        configDatabase.Views["Localization"].Fields["Localization_Children"].DisplayName = "Database";
                }

                if (configDatabase.Views.ContainsKey("FtpUpload"))
                {
                    View ftpUploadView = (View)configDatabase.Views["FtpUpload"];

                    ftpUploadView.DisplayName = "Upload Storage";
                    SetViewRoles(configDatabase.Views["FtpUpload"], "Developer,Admin," + ViewOwenrRole);
                    ftpUploadView.ColumnsInDialog = 1;
                    ftpUploadView.DataRowView = DataRowView.Tabs;

                    if (ftpUploadView.Fields.ContainsKey("FileMaxSize"))
                    {
                        ftpUploadView.Fields["FileMaxSize"].DisplayName = "File Max Size In MB";
                        ftpUploadView.Fields["FileMaxSize"].DefaultValue = System.Configuration.ConfigurationManager.AppSettings["FtpDefaultFileSize"];
                        ftpUploadView.Fields["FileMaxSize"].HideInTable = true;
                        configDatabase.Views["FtpUpload"].Fields["FileMaxSize"].Min = 0;
                        configDatabase.Views["FtpUpload"].Fields["FileMaxSize"].Max = int.Parse(System.Configuration.ConfigurationManager.AppSettings["FtpMaxFileSize"]);
                    }
                    if (ftpUploadView.Fields.ContainsKey("FtpPassword"))
                    {
                        ftpUploadView.Fields["FtpPassword"].SpecialColumn = SpecialColumn.Password;
                        ftpUploadView.Fields["FtpPassword"].HideInTable = true;
                        ftpUploadView.Fields["FtpPassword"].Required = true;
                        ftpUploadView.Fields["FtpPassword"].GridEditable = false;
                        ((ColumnField)ftpUploadView.Fields["FtpPassword"]).Encrypted = true;
                    }

                    ftpUploadView.Fields["FtpUserName"].GridEditable = false;
                    ftpUploadView.Fields["FtpHost"].GridEditable = false;
                    ftpUploadView.Fields["FtpPort"].GridEditable = false;
                    if (ftpUploadView.Fields.ContainsKey("StorageType"))
                    {
                        ftpUploadView.Fields["StorageType"].GridEditable = false;
                        ftpUploadView.Fields["AzureAccountName"].GridEditable = false;
                        ftpUploadView.Fields["AzureAccountKey"].GridEditable = false;
                    }
                    if (ftpUploadView.Fields.ContainsKey("AzureAccountKey"))
                    {
                        ftpUploadView.Fields["AzureAccountKey"].SpecialColumn = SpecialColumn.Password;
                        ftpUploadView.Fields["AzureAccountKey"].HideInTable = true;
                        ftpUploadView.Fields["AzureAccountKey"].GridEditable = false;
                        ((ColumnField)ftpUploadView.Fields["AzureAccountKey"]).Encrypted = true;
                    }

                    if (ftpUploadView.Fields.ContainsKey("AwsAccessKeyId"))
                    {
                        ftpUploadView.Fields["AwsAccessKeyId"].GridEditable = false;
                        ftpUploadView.Fields["AwsAccessKeyId"].HideInTable = true;
                    }
                    if (ftpUploadView.Fields.ContainsKey("AwsSecretAccessKey"))
                    {
                        ftpUploadView.Fields["AwsSecretAccessKey"].SpecialColumn = SpecialColumn.Password;
                        ftpUploadView.Fields["AwsSecretAccessKey"].HideInTable = true;
                        ftpUploadView.Fields["AwsSecretAccessKey"].GridEditable = false;
                        ((ColumnField)ftpUploadView.Fields["AwsSecretAccessKey"]).Encrypted = true;
                    }

                    ftpUploadView.Fields["FtpUserName"].Required = true;
                    ftpUploadView.Fields["FtpHost"].Required = true;
                    ftpUploadView.Fields["Title"].Required = true;
                    if (ftpUploadView.Fields.ContainsKey("StorageType"))
                    {
                        ftpUploadView.Fields["StorageType"].Required = true;
                        ftpUploadView.Fields["AzureAccountName"].Required = true;
                        ftpUploadView.Fields["AzureAccountKey"].Required = true;
                        if (ftpUploadView.Fields.ContainsKey("AwsAccessKeyId"))
                        {
                            ftpUploadView.Fields["AwsAccessKeyId"].Required = true;
                        }
                        if (ftpUploadView.Fields.ContainsKey("AwsSecretAccessKey"))
                        {
                            ftpUploadView.Fields["AwsSecretAccessKey"].Required = true;
                        }
                    }

                    ftpUploadView.Fields["DirectoryBasePath"].DefaultValue = "/";
                    ftpUploadView.Fields["DirectoryBasePath"].DisplayName = "Folder / Container";
                    //ftpUploadView.Fields["DirectoryBasePath"].Description = "The physical folder in the FTP server destination or the Azure Container";
                    ftpUploadView.Fields["DirectoryBasePath"].HideInTable = true;
                    ftpUploadView.Fields["DirectoryVirtualPath"].DisplayName = "URL prefix";
                    ftpUploadView.Fields["DirectoryVirtualPath"].Description = "Only use when saving in the database the image/document name and not the full path";
                    ftpUploadView.Fields["DirectoryVirtualPath"].HideInTable = true;

                    ftpUploadView.Fields["Override"].DisplayName = "Override File on Save";
                    ftpUploadView.Fields["Override"].HideInTable = true;

                    ftpUploadView.Fields["Title"].Description = "Internal name of the storage";
                    ftpUploadView.Fields["Title"].DisplayName = "Storage Title";
                    ftpUploadView.Fields["FtpHost"].DisplayName = "Server";
                    ftpUploadView.Fields["FtpHost"].HideInTable = true;
                    ftpUploadView.Fields["FtpUserName"].DisplayName = "User";
                    ftpUploadView.Fields["FtpUserName"].HideInTable = true;
                    ftpUploadView.Fields["FtpPassword"].DisplayName = "Password";
                    ftpUploadView.Fields["FtpPort"].DisplayName = "Ftp Port";
                    ftpUploadView.Fields["FtpPort"].DefaultValue = 21;
                    ftpUploadView.Fields["FtpPort"].HideInTable = true;
                    ftpUploadView.Fields["UsePassive"].DisplayName = "Use Passive";
                    ftpUploadView.Fields["UsePassive"].DefaultValue = true;
                    ftpUploadView.Fields["UsePassive"].HideInTable = true;
                    ftpUploadView.Fields["UploadFileType"].DisplayName = "File Type";
                    ((ColumnField)ftpUploadView.Fields["UploadFileType"]).EnumType = typeof(UploadFileType).AssemblyQualifiedName;
                    ftpUploadView.Fields["UploadFileType"].DefaultValue = UploadFileType.Image;
                    ftpUploadView.Fields["Width"].DisplayName = "Width In Pixels";
                    ftpUploadView.Fields["Height"].DisplayName = "Height In Pixels";
                    //ftpUploadView.Fields["UploadFileType"].DependencyData = "0|Other;Width,Height";
                    ftpUploadView.Fields["Width"].Required = false;
                    ftpUploadView.Fields["Width"].HideInTable = true;
                    ftpUploadView.Fields["Height"].Required = false;
                    ftpUploadView.Fields["Height"].DefaultValue = 30;
                    ftpUploadView.Fields["Height"].HideInTable = true;
                    if (ftpUploadView.Fields.ContainsKey("StorageType"))
                    {
                        ftpUploadView.Fields["StorageType"].DisplayName = "Storage Type";
                        ftpUploadView.Fields["AzureAccountName"].DisplayName = "Account Name";
                        ftpUploadView.Fields["AzureAccountName"].HideInTable = true;
                        ftpUploadView.Fields["AzureAccountKey"].DisplayName = "Account Key";
                        if (ftpUploadView.Fields.ContainsKey("AwsAccessKeyId"))
                        {
                            ftpUploadView.Fields["AwsAccessKeyId"].DisplayName = "Access Key ID";
                        }
                        if (ftpUploadView.Fields.ContainsKey("AwsSecretAccessKey"))
                        {
                            ftpUploadView.Fields["AwsSecretAccessKey"].DisplayName = "Secret Access Key";
                        }
                    }

                    ftpUploadView.Fields["FileAllowedTypes"].DefaultValue = "jpg,jpeg,gif,png,JPEG,JPG,GIF,PNG";
                    ftpUploadView.Fields["FileAllowedTypes"].Description = "Type of file to upload with comma separate (e.g. jpg,png,gif). Empty cell for all types.";
                    ftpUploadView.Fields["FileAllowedTypes"].HideInTable = true;

                    //SetRoles(ftpUploadView.Fields["TemplatePath"], "Developer");
                    ftpUploadView.Fields["TemplatePath"].Excluded = false;
                    ftpUploadView.Fields["TemplatePath"].HideInTable = true;
                    ftpUploadView.Fields["ID"].HideInTable = true;
                    if (ftpUploadView.Fields.ContainsKey("StorageType"))
                    {
                        ftpUploadView.Fields["StorageType"].HideInTable = false;
                        //ftpUploadView.Fields["AzureAccountName"].HideInTable = true;
                        ftpUploadView.Fields["AzureAccountKey"].HideInTable = true;
                    }

                    ftpUploadView.Fields["Title"].Order = 10;
                    ftpUploadView.Fields["DirectoryBasePath"].Order = 20;
                    ftpUploadView.Fields["DirectoryVirtualPath"].Order = 30;
                    ftpUploadView.Fields["UploadFileType"].Order = 50;
                    ftpUploadView.Fields["FileAllowedTypes"].Order = 60;
                    ftpUploadView.Fields["FileMaxSize"].Order = 65;
                    ftpUploadView.Fields["Override"].Order = 70;
                    ftpUploadView.Fields["Width"].Order = 80;
                    ftpUploadView.Fields["Height"].Order = 90;

                    ftpUploadView.Fields["FtpHost"].Order = 100;
                    ftpUploadView.Fields["FtpUserName"].Order = 110;
                    ftpUploadView.Fields["FtpPassword"].Order = 120;
                    ftpUploadView.Fields["FtpPort"].Order = 130;
                    ftpUploadView.Fields["UsePassive"].Order = 140;
                    if (ftpUploadView.Fields.ContainsKey("StorageType"))
                    {
                        ftpUploadView.Fields["StorageType"].Order = 5;
                        ftpUploadView.Fields["AzureAccountName"].Order = 150;
                        ftpUploadView.Fields["AzureAccountKey"].Order = 160;
                        ftpUploadView.Fields["StorageType"].DefaultValue = "Aws";


                        ((ColumnField)ftpUploadView.Fields["StorageType"]).EnumType = typeof(StorageType).AssemblyQualifiedName;
                    }

                    Category ftpCategory = new Category() { Name = "FTP", Ordinal = 10 };
                    ftpUploadView.Fields["FtpHost"].Category = ftpCategory;
                    ftpUploadView.Fields["FtpUserName"].Category = ftpCategory;
                    ftpUploadView.Fields["FtpPassword"].Category = ftpCategory;
                    ftpUploadView.Fields["FtpPort"].Category = ftpCategory;
                    ftpUploadView.Fields["UsePassive"].Category = ftpCategory;

                    ftpUploadView.Fields["Override"].Category = ftpCategory;
                    ftpUploadView.Fields["TemplatePath"].Category = ftpCategory;
                    ftpUploadView.Fields["Width"].Category = ftpCategory;
                    ftpUploadView.Fields["Height"].Category = ftpCategory;

                    if (ftpUploadView.Fields.ContainsKey("StorageType"))
                    {
                        Category azureCategory = new Category() { Name = "Azure", Ordinal = 20 };
                        ftpUploadView.Fields["AzureAccountKey"].Category = azureCategory;
                        ftpUploadView.Fields["AzureAccountName"].Category = azureCategory;

                        if (ftpUploadView.Fields.ContainsKey("AwsAccessKeyId"))
                        {
                            Category awsCategory = new Category() { Name = "Amazon S3", Ordinal = 30 };
                            ftpUploadView.Fields["AwsAccessKeyId"].Category = awsCategory;
                            ftpUploadView.Fields["AwsSecretAccessKey"].Category = awsCategory;
                        }

                        ftpUploadView.Derivation = new Derivation() { DerivationField = "StorageType", Deriveds = "Ftp;AzureAccountName,AzureAccountKey,AwsAccessKeyId,AwsSecretAccessKey|Azure;Override,Width,Height,TemplatePath,FtpHost,FtpUserName,FtpPassword,FtpPort,UsePassive,AwsAccessKeyId,AwsSecretAccessKey|Aws;Override,Width,Height,TemplatePath,FtpHost,FtpUserName,FtpPassword,FtpPort,UsePassive,AzureAccountName,AzureAccountKey" };

                    }

                }



                if (configDatabase.Views.ContainsKey("Upload"))
                {
                    configDatabase.Views["Upload"].DisplayColumn = "Title";
                    configDatabase.Views["Upload"].Fields["ID"].DisplayName = "Id";
                    configDatabase.Views["Upload"].Fields["UploadFileType"].DisplayName = "Upload File Type";
                    configDatabase.Views["Upload"].Fields["UploadStorageType"].DisplayName = "Upload Storage Type";
                    if (configDatabase.Views["Upload"].Fields.ContainsKey("UploadVirtualPath"))
                        configDatabase.Views["Upload"].Fields["UploadVirtualPath"].DisplayName = "Virtual Path";
                    if (configDatabase.Views["Upload"].Fields.ContainsKey("UploadPhysicalPath"))
                        configDatabase.Views["Upload"].Fields["UploadPhysicalPath"].DisplayName = "Physical Path";
                    configDatabase.Views["Upload"].Fields["Title"].DisplayName = "Title";
                    configDatabase.Views["Upload"].Fields["Upload_Children"].DisplayName = "Attachment";

                    ((ColumnField)configDatabase.Views["Upload"].Fields["UploadFileType"]).EnumType = typeof(UploadFileType).AssemblyQualifiedName;
                    ((ColumnField)configDatabase.Views["Upload"].Fields["UploadStorageType"]).EnumType = typeof(UploadStorageType).AssemblyQualifiedName;

                }

                if (configDatabase.Views.ContainsKey("Milestone"))
                {
                    configDatabase.Views["Milestone"].DisplayColumn = "Name";

                }

                if (configDatabase.Views.ContainsKey("Localization"))
                {
                    configDatabase.Views["Localization"].DisplayColumn = "Title";
                }

                if (configDatabase.Views.ContainsKey("Tooltip"))
                {
                    configDatabase.Views["Tooltip"].ColumnsInDialog = 1;
                    configDatabase.Views["Tooltip"].HideInMenu = true;
                    configDatabase.Views["Tooltip"].AllowCreate = false;
                    configDatabase.Views["Tooltip"].AllowDuplicate = false;
                    configDatabase.Views["Tooltip"].AllowDelete = false;

                    if (configDatabase.Views["Tooltip"].Fields.ContainsKey("Tooltips_Parent"))
                    {
                        configDatabase.Views["Tooltip"].Fields["Tooltips_Parent"].HideInTable = true;
                        ((ParentField)configDatabase.Views["Tooltip"].Fields["Tooltips_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
                        configDatabase.Views["Tooltip"].Fields["Tooltips_Parent"].Order = 100;
                        configDatabase.Views["Tooltip"].Fields["Tooltips_Parent"].HideInEdit = true;
                        configDatabase.Views["Tooltip"].Fields["Tooltips_Parent"].HideInCreate = true;
                    }
                    if (configDatabase.Views["Tooltip"].Fields.ContainsKey("ID"))
                    {
                        configDatabase.Views["Tooltip"].Fields["ID"].HideInTable = true;
                    }
                    if (configDatabase.Views["Tooltip"].Fields.ContainsKey("Name"))
                    {
                        configDatabase.Views["Tooltip"].Fields["Name"].HideInTable = true;
                        configDatabase.Views["Tooltip"].Fields["Name"].HideInEdit = true;
                        configDatabase.Views["Tooltip"].Fields["Name"].HideInCreate = true;
                    }

                    if (configDatabase.Views["Tooltip"].Fields.ContainsKey("Description"))
                    {
                        ((ColumnField)configDatabase.Views["Tooltip"].Fields["Description"]).TextHtmlControlType = TextHtmlControlType.TextArea;
                        ((ColumnField)configDatabase.Views["Tooltip"].Fields["Description"]).Rich = false;
                    }
                }

                if (configDatabase.Views.ContainsKey("SpecialMenu"))
                {
                    configDatabase.Views["SpecialMenu"].HideInMenu = true;
                    if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("Menus_Children"))
                    {
                        configDatabase.Views["SpecialMenu"].Fields["Menus_Children"].HideInTable = false;
                        configDatabase.Views["SpecialMenu"].Fields["Menus_Children"].DisplayName = "Menus";
                    }
                    if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("Links_Children"))
                    {
                        configDatabase.Views["SpecialMenu"].Fields["Links_Children"].HideInTable = false;
                    }
                    if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("Root"))
                    {
                        configDatabase.Views["SpecialMenu"].Fields["Root"].HideInTable = true;
                        configDatabase.Views["SpecialMenu"].Fields["Root"].HideInEdit = true;
                        configDatabase.Views["SpecialMenu"].Fields["Root"].HideInCreate = true;
                    }
                    if (configDatabase.Views["Menu"].Fields.ContainsKey("Root"))
                    {
                        configDatabase.Views["Menu"].Fields["Root"].HideInTable = true;
                        configDatabase.Views["Menu"].Fields["Root"].HideInEdit = true;
                        configDatabase.Views["Menu"].Fields["Root"].HideInCreate = true;
                        configDatabase.Views["Menu"].Fields["Root"].ExcludeInInsert = true;
                        configDatabase.Views["Menu"].Fields["Root"].ExcludeInUpdate = true;

                    }
                    if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("Menus_Parent"))
                    {
                        configDatabase.Views["SpecialMenu"].Fields["Menus_Parent"].HideInTable = true;
                        ((ParentField)configDatabase.Views["SpecialMenu"].Fields["Menus_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
                        configDatabase.Views["SpecialMenu"].Fields["Menus_Parent"].Order = 100;
                    }
                    if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("SpecialMenus_Parent"))
                    {
                        configDatabase.Views["SpecialMenu"].Fields["SpecialMenus_Parent"].HideInTable = true;
                        ((ParentField)configDatabase.Views["SpecialMenu"].Fields["SpecialMenus_Parent"]).ParentHtmlControlType = ParentHtmlControlType.DropDown;
                        configDatabase.Views["SpecialMenu"].Fields["SpecialMenus_Parent"].Order = 100;

                    }
                    if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("SpecialMenus_Parent"))
                    {
                        configDatabase.Views["SpecialMenu"].Fields["SpecialMenus_Parent"].HideInTable = true;
                    }
                }

                if (configDatabase.Views["Menu"].Fields.ContainsKey("UrlLinks_Children"))
                {
                    configDatabase.Views["Menu"].Fields["UrlLinks_Children"].HideInTable = false;
                    configDatabase.Views["Menu"].Fields["UrlLinks_Children"].DisplayName = "Links";
                    ((ChildrenField)configDatabase.Views["Menu"].Fields["UrlLinks_Children"]).ChildrenHtmlControlType = ChildrenHtmlControlType.Grid;
                    //SetRoles(configDatabase.Views["Menu"].Fields["UrlLinks_Children"], "Developer,Admin");

                }

                if (configDatabase.Views.ContainsKey("UrlLink"))
                {
                    configDatabase.Views["UrlLink"].Fields["Target"].Excluded = true;
                    configDatabase.Views["UrlLink"].Fields["ID"].HideInTable = true;
                    configDatabase.Views["UrlLink"].Fields["Url"].DefaultValue = "http://";
                }

                if (configDatabase.Views.ContainsKey("Cron"))
                {
                    configDatabase.Views["Cron"].HideInMenu = true;

                    ColumnField template = (ColumnField)configDatabase.Views["Cron"].Fields["Template"];
                    template.TextHtmlControlType = TextHtmlControlType.DropDown;
                    template.MultiValueParentViewName = "durados_Html";

                    if (configDatabase.Views["Cron"].Fields.ContainsKey("Crons_Parent"))
                    {
                        configDatabase.Views["Cron"].Fields["Crons_Parent"].HideInTable = true;
                        ((ParentField)configDatabase.Views["Cron"].Fields["Crons_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
                        configDatabase.Views["Cron"].Fields["Crons_Parent"].Order = 100;
                        configDatabase.Views["Cron"].Fields["Crons_Parent"].DefaultValue = "0";

                    }
                    if (configDatabase.Views["Cron"].Fields.ContainsKey("Cycle"))
                    {
                        ColumnField cycle = (ColumnField)configDatabase.Views["Cron"].Fields["Cycle"];
                        cycle.TextHtmlControlType = TextHtmlControlType.DropDown;
                        cycle.EnumType = typeof(CycleEnum).AssemblyQualifiedName;
                        // cycle.MultiValueParentViewName = "durados_Html";
                    }

                }
                if (configDatabase.Views["Derivation"].Fields.ContainsKey("Deriveds"))
                    ((ColumnField)configDatabase.Views["Derivation"].Fields["Deriveds"]).TextHtmlControlType = TextHtmlControlType.TextArea;



                foreach (View view in configDatabase.Views.Values)
                {
                    view.GridEditable = true;
                    view.GridEditableEnabled = true;
                }


                ConfigCategories(configDatabase);

                //View
                ConfigViewView(configDatabase);

                ConfigViewMenuOrganizerView(configDatabase);

                //Field
                ConfigFieldView(configDatabase);//, configView

                //Chart
                ConfigChartView(configDatabase);

                //Dashboard
                ConfigDashboardView(configDatabase);

                foreach (Field field in configView.Fields.Values.Where(f => f.Category != null && (f.Category.Name == "Developers" || f.Category.Name == "Advanced_Layout" || f.Category.Name == "Advanced" || f.Category.Name == "System" || f.Category.Name == "Xml" || f.Category.Name == "Tree")))
                {
                    SetRoles(field, "Developer");
                }

                foreach (Field field in configDatabase.Views["Field"].Fields.Values.Where(f => f.Category != null && (f.Category.Name == "Developers" || f.Category.Name == "Advanced" || f.Category.Name == "Encryption" || f.Category.Name == "System" || f.Category.Name == "Xml" || f.Category.Name == "Tree" || f.Category.Name == "Email" || f.Category.Name == "Behaviour")))
                {
                    SetRoles(field, "Developer");
                }

                ConfigureSecurity(configDatabase, database);

                SetNewViewDefaults(configDatabase);

                //Overrride security
                SetRoles(configDatabase.Views["View"].Fields["SystemView"], "Developer");
                configDatabase.Views["View"].Fields["SystemView"].AllowSelectRoles = "Developer,Admin";

                ConfigPage(configDatabase);

                SetPlans(database, configDatabase);
            }
            catch { }
        }

        protected virtual void SetPlans(Mvc.Database database, Mvc.Database configDatabase)
        {
            //Plans:
            //1 - Enterprise - Full access to all
            //2 - Wix Premium
            //3 - Wix Free

            View viewView = (View)configDatabase.Views["View"];

            foreach (View view in configDatabase.Views.Values)
            {
                foreach (Field field in view.Fields.Values)
                {
                    field.Plan = "1";
                }
            }

            //3 - Wix Free
            if (viewView.Fields.ContainsKey("Layout"))
                viewView.Fields["Layout"].Plan = "1,2,3";
            if (viewView.Fields.ContainsKey("Skin"))
                viewView.Fields["Skin"].Plan = "1,2,3";
            if (viewView.Fields.ContainsKey("Theme"))
                viewView.Fields["Theme"].Plan = "1,2,3";
            if (viewView.Fields.ContainsKey("RowHeight"))
                viewView.Fields["RowHeight"].Plan = "1,2,3";
            viewView.Fields["DisplayName"].Plan = "1,2,3";
            if (viewView.Fields.ContainsKey("JsonName"))
                viewView.Fields["JsonName"].Plan = "1,2,3";
            viewView.Fields["PageSize"].Plan = "1,2,3";

            viewView.Fields["HideFilter"].Plan = "1,2,3";
            viewView.Fields["HidePager"].Plan = "1,2,3";
            viewView.Fields["HideSearch"].Plan = "1,2,3";
            viewView.Fields["HideToolbar"].Plan = "1,2,3";
            viewView.Fields["CollapseFilter"].Plan = "1,2,3";
            viewView.Fields["ExportToCsv"].Plan = "1,2,3";
            viewView.Fields["ImportFromExcel"].Plan = "1,2";
            viewView.Fields["GridEditable"].Plan = "1,2,3";

            viewView.Fields["ColumnsInDialog"].Plan = "1,2,3";
            viewView.Fields["AllowCreate"].Plan = "1,2,3";
            viewView.Fields["AllowEdit"].Plan = "1,2,3";
            viewView.Fields["AllowDuplicate"].Plan = "1,2,3";
            viewView.Fields["AllowDelete"].Plan = "1,2,3";
            viewView.Fields["MultiSelect"].Plan = "1,2,3";
            viewView.Fields["Send"].Plan = "1,2,3";

            viewView.Fields["Description"].Plan = "1,2,3";
            viewView.Fields["NewButtonName"].Plan = "1,2,3";
            viewView.Fields["EditButtonName"].Plan = "1,2,3";
            viewView.Fields["DuplicateButtonName"].Plan = "1,2,3";
            viewView.Fields["DeleteButtonName"].Plan = "1,2,3";
            viewView.Fields["GridDisplayType"].Plan = "1,2,3";

            if (viewView.Fields.ContainsKey("Background"))
            {
                viewView.Fields["Background"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("RowBackground"))
            {
                viewView.Fields["RowBackground"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("HoverBackground"))
            {
                viewView.Fields["HoverBackground"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("AlternateRowBackground"))
            {
                viewView.Fields["AlternateRowBackground"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("ToolBoxBackground"))
            {
                viewView.Fields["ToolBoxBackground"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("FontColor"))
            {
                viewView.Fields["FontColor"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("ToolBoxTextColor"))
            {
                viewView.Fields["ToolBoxTextColor"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("AlterTextColor"))
            {
                viewView.Fields["AlterTextColor"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("HoverTextColor"))
            {
                viewView.Fields["HoverTextColor"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("TextFontColor"))
            {
                viewView.Fields["TextFontColor"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("BorderColor"))
            {
                viewView.Fields["BorderColor"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("ApplyColorDesignToAllViews"))
            {
                viewView.Fields["ApplyColorDesignToAllViews"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("ApplySkinToAllViews"))
            {
                viewView.Fields["ApplySkinToAllViews"].Plan = "1,2,3";
            }
            if (viewView.Fields.ContainsKey("CustomThemePath"))
            {
                viewView.Fields["CustomThemePath"].Plan = "1,2,3";
            }

            //2 - Wix Premium
            viewView.Fields["SaveHistory"].Plan = "1,2";

            View fieldView = (View)configDatabase.Views["Field"];

            fieldView.Fields["DisplayName"].Plan = "1,2,3";
            if (fieldView.Fields.ContainsKey("JsonName"))
                fieldView.Fields["JsonName"].Plan = "1,2,3";
            fieldView.Fields["DisplayFormat"].Plan = "1,2,3";
            if (fieldView.Fields.ContainsKey("ShowColumnHeader"))
                fieldView.Fields["ShowColumnHeader"].Plan = "1,2,3";
            fieldView.Fields["DataType"].Plan = "1,2,3";
            fieldView.Fields["Excluded"].Plan = "1,2,3";
            fieldView.Fields["Required"].Plan = "1,2,3";
            fieldView.Fields["Unique"].Plan = "1,2,3";
            fieldView.Fields["DefaultValue"].Plan = "1,2,3";
            fieldView.Fields["Description"].Plan = "1,2,3";
            if (fieldView.Fields.ContainsKey("TextAlignment"))
                fieldView.Fields["TextAlignment"].Plan = "1,2,3";

            fieldView.Fields["Min"].Plan = "1,2";
            fieldView.Fields["Max"].Plan = "1,2";

            fieldView.Fields["HideInTable"].Plan = "1,2,3";
            fieldView.Fields["Sortable"].Plan = "1,2,3";
            if (fieldView.Fields.ContainsKey("Preview"))
                fieldView.Fields["Preview"].Plan = "1,2,3";
            fieldView.Fields["HideInFilter"].Plan = "1,2,3";
            //fieldView.Fields["GroupFilterWidth"].Plan = "1,2,3";
            fieldView.Fields["HideInFilter"].Plan = "1,2,3";
            fieldView.Fields["HideInEdit"].Plan = "1,2,3";
            fieldView.Fields["HideInCreate"].Plan = "1,2,3";

            fieldView.Fields["ColSpanInDialog"].Plan = "1,2,3";
            fieldView.Fields["Width"].Plan = "1,2,3";
            fieldView.Fields["Seperator"].Plan = "1,2,3";

        }

        protected virtual void ConfigViewMenuOrganizerView(Durados.Web.Mvc.Database configDatabase)
        {
            View viewView = (View)configDatabase.Views["MenuOrganizerView"];
            ConfigViewView(configDatabase, viewView);
            viewView.Fields["DisplayName"].Excluded = false;
            viewView.Fields["Order"].Excluded = false;
            viewView.Fields["Menu_Parent"].Excluded = false;
            viewView.Fields["HideInMenu"].Excluded = false;

            viewView.Fields["Order"].HideInTable = true;
            viewView.Fields["Menu_Parent"].HideInTable = true;
            viewView.Fields["HideInMenu"].HideInTable = true;

            viewView.Fields["Order"].HiddenAttribute = true;
            viewView.Fields["Menu_Parent"].HiddenAttribute = true;
            viewView.Fields["HideInMenu"].HiddenAttribute = true;
            viewView.OrdinalColumnName = "Order";
            foreach (Field field in viewView.Fields.Values)
            {
                field.Sortable = false;
            }
            viewView.DefaultSort = "HideInMenu desc, Menu asc, Order asc";
            viewView.PermanentFilter = "SystemView = 0";
            viewView.HideFilter = true;
            viewView.HidePager = true;
            viewView.HideToolbar = true;
            viewView.HideSearch = true;
            viewView.MultiSelect = false;
        }

        protected virtual void ConfigViewView(Durados.Web.Mvc.Database configDatabase)
        {
            View viewView = (View)configDatabase.Views["View"];

            ConfigViewView(configDatabase, viewView);
        }

        protected virtual void SetViewSettingsFields(Durados.Web.Mvc.Config.View viewView)
        {
            viewView.SettingsFields.Add(viewView.Fields["DisplayName"]);

            viewView.SettingsFields.Add(viewView.Fields["AllowCreate"]);
            viewView.SettingsFields.Add(viewView.Fields["AllowDuplicate"]);
            viewView.SettingsFields.Add(viewView.Fields["AllowEdit"]);
            viewView.SettingsFields.Add(viewView.Fields["AllowDelete"]);

            viewView.SettingsFields.Add(viewView.Fields["ExportToCsv"]);
            viewView.SettingsFields.Add(viewView.Fields["ImportFromExcel"]);
            viewView.SettingsFields.Add(viewView.Fields["SaveHistory"]);

            viewView.SettingsFields.Add(viewView.Fields["GridDisplayType"]);

        }

        protected virtual void SetPageSettingsFields(Durados.Web.Mvc.Config.View pageView)
        {
            foreach (Field field in pageView.Fields.Values)
            {
                field.IsAdminPreview = true;
            }

        }

        protected virtual void ConfigViewView(Durados.Web.Mvc.Database configDatabase, View viewView)
        {
            SetViewRoles(viewView, "Developer,Admin");
            SetViewSettingsFields(viewView);
            SetPageSettingsFields((View)configDatabase.Views["Page"]);


            #region init View Properties
            viewView.Layout = LayoutType.Custom;
            viewView.DataDisplayType = DataDisplayType.Table;
            viewView.EnableTableDisplay = true;
            viewView.EnableDashboardDisplay = true;
            viewView.EnablePreviewDisplay = true;
            viewView.FilterType = FilterType.Group;
            viewView.SortingType = SortingType.Group;

            viewView.PageSize = 100;
            viewView.Popup = true;
            viewView.DisplayName = "Views";

            foreach (Field field in viewView.Fields.Values)
            {
                field.HideInFilter = true;
                field.Sortable = false;
            }

            //configView.TreeType = TreeType.Adjacency;
            //configView.TreeViewName = "SpecialMenu";
            //configView.TreeRelatedFieldName = "SpecialMenu_Parent";
            //configDatabase.Views["SpecialMenu"].TreeRelatedFieldName = "Menus_Parent";
            viewView.DisplayColumn = "Name";
            viewView.DefaultSort = "DisplayName asc";
            viewView.AllowCreate = true;
            viewView.AllowDelete = true;
            viewView.AllowDuplicate = false;
            viewView.UseOrderForEdit = true;
            viewView.UseOrderForCreate = true;
            viewView.DataRowView = DataRowView.Accordion;
            viewView.GridDisplayType = GridDisplayType.BasedOnColumnWidth;
            //viewView.AllowSelectRoles = configDatabase.DefaultAllowSelectRoles;
            //viewView.AllowCreateRoles = configDatabase.DefaultAllowCreateRoles;
            //viewView.AllowEditRoles = configDatabase.DefaultAllowEditRoles;
            //viewView.AllowDeleteRoles = configDatabase.DefaultAllowDeleteRoles;
            ((View)viewView).MultiSelect = true;
            viewView.GridEditable = true;
            viewView.ColumnsInDialogPerCategory = "General;2;Design;1;Advanced_Design;2;Behaviour;2;Description;2;Email;1;Advanced;3;Permissions;1;Developers;3;System;3";
            viewView.AddItemsFieldName = "Menu_Parent";
            //configView.PermanentFilter = "SystemView = 0";
            viewView.Fields["SystemView"].DefaultFilter = "False";
            viewView.Fields["SystemView"].DisplayName = "System View";
            viewView.Fields["SystemView"].HideInFilter = false;
            viewView.Fields["SystemView"].GroupFilterWidth = 40;

            //viewView.OrdinalColumnName = "DisplayName";
            viewView.Fields["ID"].HideInTable = true;
            viewView.Fields["ID"].DisplayName = "Id";
            viewView.Fields["Controller"].DenyEditRoles = "User";
            viewView.Fields["Controller"].HideInTable = true;
            viewView.Fields["Controller"].DisplayName = "Controller";
            viewView.Fields["IndexAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["IndexAction"].HideInTable = true;
            viewView.Fields["IndexAction"].DisplayName = "Index Action";
            viewView.Fields["SetLanguageAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["SetLanguageAction"].HideInTable = true;
            viewView.Fields["SetLanguageAction"].DisplayName = "Set Language Action";
            viewView.Fields["CreateAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["CreateAction"].HideInTable = true;
            viewView.Fields["CreateAction"].DisplayName = "Create Action";
            viewView.Fields["EditAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["EditAction"].HideInTable = true;
            viewView.Fields["EditAction"].DisplayName = "Edit Action";
            viewView.Fields["GetJsonViewAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["GetJsonViewAction"].HideInTable = true;
            viewView.Fields["GetJsonViewAction"].DisplayName = "Get JsonView Action";
            viewView.Fields["DeleteAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["DeleteAction"].HideInTable = true;
            viewView.Fields["DeleteAction"].DisplayName = "Delete Action";
            viewView.Fields["FilterAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["FilterAction"].HideInTable = true;
            viewView.Fields["FilterAction"].DisplayName = "Filter Action";
            viewView.Fields["UploadAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["UploadAction"].HideInTable = true;
            viewView.Fields["UploadAction"].DisplayName = "Upload Action";
            viewView.Fields["ExportToCsvAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["ExportToCsvAction"].HideInTable = true;
            viewView.Fields["ExportToCsvAction"].DisplayName = "ExportToCsv Action";
            viewView.Fields["PrintAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["PrintAction"].HideInTable = true;
            viewView.Fields["PrintAction"].DisplayName = "Print Action";
            viewView.Fields["AutoCompleteAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["AutoCompleteAction"].HideInTable = true;
            viewView.Fields["AutoCompleteAction"].DisplayName = "Auto Complete Action";
            viewView.Fields["AutoCompleteController"].DenyEditRoles = "Admin, User";
            viewView.Fields["AutoCompleteController"].HideInTable = true;
            viewView.Fields["AutoCompleteController"].DisplayName = "Auto Complete Controller";
            viewView.Fields["InlineAddingDialogAction"].DenyEditRoles = "Admin, User";
            viewView.Fields["InlineAddingDialogAction"].HideInTable = true;
            viewView.Fields["InlineAddingDialogAction"].DisplayName = "Inline Adding Dialog Action";
            ((View)viewView).AddItemsButtonName = "Add Views";
            ((View)viewView).AddItemsButtonDescription = "Add tables/views from the Database";
            ((View)viewView).NewButtonName = "New View";
            ((View)viewView).NewButtonDescription = "Add existing or new table/view from the Database";
            ((View)viewView).ImportFromExcel = true;

            viewView.Fields["InlineEditingDialogAction"].HideInTable = true;
            viewView.Fields["InlineEditingDialogAction"].DisplayName = "Inline Editing Dialog Action";
            viewView.Fields["InlineEditingEditAction"].HideInTable = true;
            viewView.Fields["InlineEditingEditAction"].DisplayName = "Inline Editing Edit Action";
            viewView.Fields["ContainerGraphicProperties"].HideInTable = true;
            viewView.Fields["ContainerGraphicProperties"].DisplayName = "Container Graphic";

            viewView.Fields["Description"].ColSpanInDialog = 2;
            viewView.Fields["Description"].DisplayName = "Tooltip";
            viewView.Fields["Description"].Excluded = true;
            (viewView.Fields["Description"] as ColumnField).TextHtmlControlType = TextHtmlControlType.TextArea;
            (viewView.Fields["Description"] as ColumnField).CssClass = "wtextareashort40";

            viewView.Fields["PromoteButtonDescription"].ColSpanInDialog = 2;
            ((ColumnField)viewView.Fields["PromoteButtonDescription"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            ((ColumnField)viewView.Fields["PromoteButtonDescription"]).CssClass = "wtextareashort40";
            viewView.Fields["NewButtonDescription"].ColSpanInDialog = 2;
            ((ColumnField)viewView.Fields["NewButtonDescription"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            ((ColumnField)viewView.Fields["NewButtonDescription"]).CssClass = "wtextareashort40";
            viewView.Fields["EditButtonDescription"].ColSpanInDialog = 2;
            ((ColumnField)viewView.Fields["EditButtonDescription"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            ((ColumnField)viewView.Fields["EditButtonDescription"]).CssClass = "wtextareashort40";
            viewView.Fields["DuplicateButtonDescription"].ColSpanInDialog = 2;
            ((ColumnField)viewView.Fields["DuplicateButtonDescription"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            ((ColumnField)viewView.Fields["DuplicateButtonDescription"]).CssClass = "wtextareashort40";
            viewView.Fields["AddItemsButtonDescription"].ColSpanInDialog = 2;
            ((ColumnField)viewView.Fields["AddItemsButtonDescription"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            ((ColumnField)viewView.Fields["AddItemsButtonDescription"]).CssClass = "wtextareashort40";


            viewView.Fields["DuplicateAction"].HideInTable = true;
            viewView.Fields["DuplicateAction"].DisplayName = "Duplicate Action";
            viewView.Fields["InlineDuplicateDialogAction"].HideInTable = true;
            viewView.Fields["InlineDuplicateDialogAction"].DisplayName = "Inline Duplicate Dialog Action";
            viewView.Fields["InlineDuplicateAction"].HideInTable = true;
            viewView.Fields["InlineDuplicateAction"].DisplayName = "Inline Duplicate Action";
            viewView.Fields["InlineSearchDialogAction"].HideInTable = true;
            viewView.Fields["InlineSearchDialogAction"].DisplayName = "Inline Search Dialog Action";
            viewView.Fields["EditOnlyAction"].HideInTable = true;
            viewView.Fields["EditOnlyAction"].DisplayName = "Edit Only Action";
            viewView.Fields["InlineAddingCreateAction"].HideInTable = true;
            viewView.Fields["InlineAddingCreateAction"].DisplayName = "Inline Adding Create Action";

            viewView.Fields["CreateOnlyAction"].HideInTable = true;
            viewView.Fields["CreateOnlyAction"].DisplayName = "Create Only Action";
            viewView.Fields["EditRichAction"].HideInTable = true;
            viewView.Fields["EditRichAction"].DisplayName = "Edit Rich Action";
            viewView.Fields["GetRichAction"].HideInTable = true;
            viewView.Fields["GetRichAction"].DisplayName = "Get Rich Action";
            viewView.Fields["GetSelectListAction"].HideInTable = true;
            viewView.Fields["GetSelectListAction"].DisplayName = "Get Select List Action";
            viewView.Fields["EditSelectionAction"].HideInTable = true;
            viewView.Fields["EditSelectionAction"].DisplayName = "Edit Selection Action";
            viewView.Fields["ExportToCsv"].HideInTable = true;
            viewView.Fields["ExportToCsv"].IsAdminPreview = true;
            viewView.Fields["ExportToCsv"].DisplayName = "Export To Excel";
            viewView.Fields["ExportToCsv"].HideInFilter = false;
            viewView.Fields["ExportToCsv"].GroupFilterWidth = 40;
            viewView.Fields["Print"].HideInTable = true;
            viewView.Fields["Print"].DisplayName = "Show Print";
            ColumnField pageZizeColumnField = (ColumnField)viewView.Fields["PageSize"];
            pageZizeColumnField.HideInTable = false;
            pageZizeColumnField.DisplayName = "Rows per Page";
            pageZizeColumnField.TextHtmlControlType = TextHtmlControlType.DropDown;
            pageZizeColumnField.MultiValueAdditionals = "5,5,10,10,15,15,20,20,30,30,50,50,100,100,200,200,500,500,1000,1000";

            //viewView.Fields["PageSize"].Width = 50;
            viewView.Fields["PageSize"].IsAdminPreview = true;
            if (viewView.Fields.ContainsKey("WorkFlowStepsFieldName"))
                viewView.Fields["WorkFlowStepsFieldName"].HideInTable = true;
            if (viewView.Fields.ContainsKey("OpenSingleRow"))
            {
                Field openSingleRowField = viewView.Fields["OpenSingleRow"];
                openSingleRowField.HideInTable = true;
            }

            ColumnField columnsInDialogField = (ColumnField)viewView.Fields["ColumnsInDialog"];
            columnsInDialogField.HideInTable = true;
            columnsInDialogField.DisplayName = "Columns In Dialog";
            columnsInDialogField.TextHtmlControlType = TextHtmlControlType.DropDown;
            columnsInDialogField.MultiValueAdditionals = "1,1,2,2,3,3,4,4";
            columnsInDialogField.Width = 120;

            viewView.Fields["AllowCreate"].HideInTable = true;
            viewView.Fields["AllowCreate"].IsAdminPreview = true;
            viewView.Fields["AllowCreate"].DisplayName = "Allow Add";
            viewView.Fields["AllowCreate"].HideInFilter = false;
            viewView.Fields["AllowCreate"].GroupFilterWidth = 40;
            viewView.Fields["AllowEdit"].HideInTable = true;
            viewView.Fields["AllowEdit"].DisplayName = "Allow Edit";
            viewView.Fields["AllowEdit"].IsAdminPreview = true;
            viewView.Fields["AllowEdit"].HideInFilter = false;
            viewView.Fields["AllowEdit"].GroupFilterWidth = 40;
            viewView.Fields["AllowDelete"].HideInTable = true;
            viewView.Fields["AllowDelete"].DisplayName = "Allow Delete";
            viewView.Fields["AllowDelete"].IsAdminPreview = true;
            viewView.Fields["AllowDelete"].HideInFilter = false;
            viewView.Fields["AllowDelete"].GroupFilterWidth = 40;
            viewView.Fields["DenyCreateRoles"].HideInTable = true;
            viewView.Fields["DenyCreateRoles"].DisplayName = "Deny Create Roles";
            viewView.Fields["DenyEditRoles"].HideInTable = true;
            viewView.Fields["DenyEditRoles"].DisplayName = "Deny Edit Roles";
            viewView.Fields["DenyDeleteRoles"].HideInTable = true;
            viewView.Fields["DenyDeleteRoles"].DisplayName = "Deny Delete Roles";
            viewView.Fields["DenySelectRoles"].HideInTable = true;
            viewView.Fields["DenySelectRoles"].DisplayName = "Deny Read Roles";
            viewView.Fields["AllowCreateRoles"].HideInTable = true;
            viewView.Fields["AllowCreateRoles"].DisplayName = "Allow Create Roles";
            viewView.Fields["AllowEditRoles"].HideInTable = true;
            viewView.Fields["AllowEditRoles"].DisplayName = "Allow Edit Roles";
            viewView.Fields["AllowSelectRoles"].HideInTable = true;
            viewView.Fields["AllowSelectRoles"].DisplayName = "Allow Read Roles";
            viewView.Fields["AllowDeleteRoles"].HideInTable = true;
            viewView.Fields["AllowDeleteRoles"].DisplayName = "Allow Delete Roles";
            if (viewView.Fields.ContainsKey("ViewOwnerRoles"))
            {
                viewView.Fields["ViewOwnerRoles"].HideInTable = true;
                viewView.Fields["ViewOwnerRoles"].DisplayName = "View Owner Roles";
            }
            viewView.Fields["GridEditable"].DefaultValue = true;
            viewView.Fields["GridEditableEnabled"].DefaultValue = true;
            viewView.Fields["GridEditableEnabled"].HideInTable = true;

            //configView.Fields["Name"].HideInTable = true;
            viewView.Fields["Name"].DisplayName = "Database Name";
            viewView.Fields["Name"].Preview = true;
            viewView.Fields["Name"].DisableInEdit = true;
            viewView.Fields["Name"].HideInFilter = false;
            viewView.Fields["Name"].Sortable = true;
            viewView.Fields["Name"].GroupFilterWidth = 160;
            viewView.Fields["Name"].TableCellMinWidth = 120;
            viewView.Fields["DisplayName"].HideInCreate = true; ;
            viewView.Fields["DisplayName"].HideInTable = false;
            viewView.Fields["DisplayName"].DisplayName = "Caption Text";
            viewView.Fields["DisplayName"].IsAdminPreview = true;
            viewView.Fields["DisplayName"].Preview = true;
            viewView.Fields["DisplayName"].Sortable = true;
            viewView.Fields["DisplayName"].HideInFilter = false;
            viewView.Fields["DisplayName"].GroupFilterWidth = 160;

            if (viewView.Fields.ContainsKey("JsonName"))
            {
                viewView.Fields["JsonName"].HideInCreate = true; ;
                viewView.Fields["JsonName"].HideInTable = false;
                viewView.Fields["JsonName"].DisplayName = "Name";
                viewView.Fields["JsonName"].IsAdminPreview = false;
                viewView.Fields["JsonName"].Preview = true;
                viewView.Fields["JsonName"].Sortable = true;
                viewView.Fields["JsonName"].HideInFilter = false;
                viewView.Fields["JsonName"].GroupFilterWidth = 160;
            }

            viewView.Fields["Name"].TableCellMinWidth = 120;

            //viewView.Fields["EditableTableName"].DisplayName = "";

            //viewView.Fields["Order"].HideInTable = true;
            //viewView.Fields["Order"].HideInEdit = true;
            viewView.Fields["Order"].HideInCreate = true;
            viewView.Fields["Order"].DisplayName = "order";
            //configView.Fields["DisplayColumn"].HideInTable = true;
            ColumnField displayColumnField = (ColumnField)viewView.Fields["DisplayColumn"];
            displayColumnField.DisplayName = "Column Display in Title";
            displayColumnField.TextHtmlControlType = TextHtmlControlType.DependencyCustom;
            displayColumnField.HideInTable = false;
            displayColumnField.HideInFilter = true;
            displayColumnField.GridEditable = false;
            displayColumnField.IsAdminPreview = true;

            //displayColumnField.MultiValueParentViewName = "Field";
            //displayColumnField.DropDownValueField = "Name";
            //displayColumnField.DropDownDisplayField = "DisplayName";
            //displayColumnField.MultiValueAdditionals = GetDelimitedFields(configView.Fields.Values.Where(f => f.FieldType != FieldType.Children).ToList());

            viewView.Fields["Views_Parent"].DisplayName = "Database";
            viewView.Fields["Views_Parent"].DenyEditRoles = "Admin, User";
            viewView.Fields["Views_Parent"].HideInTable = true;
            viewView.Fields["Views_Parent"].DisableInEdit = true;
            viewView.Fields["Fields_Children"].DisplayName = "Fields";
            viewView.Fields["Fields_Children"].HideInTable = false;
            ((ChildrenField)viewView.Fields["Fields_Children"]).ChildrenHtmlControlType = ChildrenHtmlControlType.Grid;

            viewView.Fields["HideInMenu"].HideInTable = true;
            viewView.Fields["HideInMenu"].HideInCreate = true;
            viewView.Fields["HideInMenu"].HideInEdit = true;
            viewView.Fields["HideInMenu"].DisplayName = "Hide Menu";
            //viewView.Fields["HideInMenu"].IsAdminPreview = true;
            viewView.Fields["HideInMenu"].HideInTable = true;
            viewView.Fields["HideInMenu"].HideInEdit = true;
            viewView.Fields["HideInMenu"].HideInCreate = true;

            viewView.Fields["DeleteSelectionAction"].HideInTable = true;
            viewView.Fields["DeleteSelectionAction"].DisplayName = "Delete Selection Action";
            viewView.Fields["MultiSelect"].HideInTable = false;
            viewView.Fields["MultiSelect"].IsAdminPreview = true;
            viewView.Fields["MultiSelect"].DisplayName = "Multi Select";
            viewView.Fields["HasChildrenRow"].HideInTable = true;
            viewView.Fields["HasChildrenRow"].DisplayName = "Has Children Row";
            viewView.Fields["UseOrderForCreate"].HideInTable = true;
            viewView.Fields["UseOrderForCreate"].DisplayName = "Use Order For Create";
            viewView.Fields["UseOrderForEdit"].HideInTable = true;
            viewView.Fields["UseOrderForEdit"].DisplayName = "Use Order For Edit";
            viewView.Fields["BaseViewName"].HideInTable = false;
            viewView.Fields["BaseViewName"].DisableInCreate = true;
            viewView.Fields["BaseViewName"].DisableInDuplicate = true;
            viewView.Fields["BaseViewName"].DisableInEdit = true;
            viewView.Fields["BaseViewName"].DisplayName = "Base View Name";

            viewView.Fields["Menu_Parent"].DisplayName = "Menu";
            viewView.Fields["Menu_Parent"].HideInTable = true;
            viewView.Fields["Menu_Parent"].HideInEdit = true;
            viewView.Fields["Menu_Parent"].HideInCreate = true;
            //viewView.Fields["Menu_Parent"].GridEditable = false;
            //configView.Fields["Menu_Parent"].HideInTable = true;

            viewView.Fields["NotifyMessageKey"].DisplayName = "Notification Message Key";
            viewView.Fields["NotifyMessageKey"].HideInTable = true;
            viewView.Fields["NotifySubjectKey"].DisplayName = "Notification Subject Key";
            viewView.Fields["NotifySubjectKey"].HideInTable = true;

            viewView.Fields["DuplicateMessage"].HideInTable = true;
            viewView.Fields["DuplicateMessage"].DisplayName = "Duplicate Message";
            viewView.Fields["BaseTableName"].HideInTable = true;
            viewView.Fields["BaseTableName"].DisplayName = "Counter Base Table Name";
            viewView.Fields["Derivation_Parent"].HideInTable = true;
            viewView.Fields["Derivation_Parent"].DisplayName = "Derivation Parent";
            viewView.Fields["ChartInfo_Parent"].HideInTable = true;
            viewView.Fields["ChartInfo_Parent"].DisplayName = "ChartInfo Parent";

            viewView.Fields["AnotherRowLinkText"].HideInTable = true;
            viewView.Fields["AnotherRowLinkText"].DisplayName = "Another Row Link Text";
            viewView.Fields["AnotherRowLinkFunction"].HideInTable = true;
            viewView.Fields["AnotherRowLinkFunction"].DisplayName = "Another Row Link Function";
            viewView.Fields["JavaScripts"].HideInTable = true;
            viewView.Fields["JavaScripts"].DisplayName = "JavaScripts";
            viewView.Fields["IsImageTable"].HideInTable = true;
            viewView.Fields["IsImageTable"].DisplayName = "Is Image Table";

            viewView.Fields["RefreshAction"].HideInTable = true;
            viewView.Fields["RefreshAction"].DisplayName = "Refresh Action";

            viewView.Fields["Popup"].HideInTable = true;
            viewView.Fields["Popup"].DisplayName = "Open as Popup";

            viewView.Fields["TabCache"].HideInTable = true;
            viewView.Fields["TabCache"].DisplayName = "Cache the Tab";
            viewView.Fields["RefreshOnClose"].HideInTable = true;
            viewView.Fields["RefreshOnClose"].DisplayName = "Refresh On Close";

            viewView.Fields["HideFilter"].ColSpanInDialog = 2;
            viewView.Fields["HideFilter"].HideInTable = false;
            viewView.Fields["HideFilter"].IsAdminPreview = true;
            viewView.Fields["HideFilter"].DisplayName = "Hide Filter";
            viewView.Fields["HidePager"].HideInTable = false;
            viewView.Fields["HidePager"].DisplayName = "Hide Footer";
            viewView.Fields["HidePager"].IsAdminPreview = true;

            #region HideInTable
            viewView.Fields["CheckListAction"].HideInTable = true;
            viewView.Fields["CheckListAction"].DisplayName = "CheckList Action";

            viewView.Fields["StyleSheets"].HideInTable = true;
            viewView.Fields["ImportFromExcel"].HideInTable = false;
            viewView.Fields["ImportFromExcel"].IsAdminPreview = true;
            viewView.Fields["ImportFromExcel"].HideInFilter = false;
            viewView.Fields["ImportFromExcel"].GroupFilterWidth = 40;

            viewView.Fields["PromoteButtonName"].HideInTable = true;
            viewView.Fields["NewButtonName"].HideInTable = true;
            viewView.Fields["NewButtonName"].IsAdminPreview = true;
            viewView.Fields["EditButtonName"].HideInTable = true;
            viewView.Fields["EditButtonName"].IsAdminPreview = true;
            viewView.Fields["DuplicateButtonName"].HideInTable = true;
            viewView.Fields["DuplicateButtonName"].IsAdminPreview = true;
            viewView.Fields["AddItemsButtonName"].HideInTable = true;
            viewView.Fields["InsertButtonName"].HideInTable = true;
            viewView.Fields["DeleteButtonName"].HideInTable = true;
            viewView.Fields["DeleteButtonName"].IsAdminPreview = true;

            viewView.Fields["HideToolbar"].HideInTable = true;
            viewView.Fields["HideToolbar"].IsAdminPreview = true;
            viewView.Fields["ShowUpDown"].HideInTable = true;
            viewView.Fields["HistoryNotifyList"].HideInTable = true;
            viewView.Fields["ColumnsInDialogPerCategory"].HideInTable = true;
            viewView.Fields["AllowDuplicate"].HideInTable = true;
            viewView.Fields["AllowDuplicate"].IsAdminPreview = true;
            //configView.Fields["PermanentFilter"].HideInTable = true;
            viewView.Fields["HideSearch"].HideInTable = true;
            viewView.Fields["HideSearch"].DisplayName = "Hide Search Box";
            viewView.Fields["HideSearch"].IsAdminPreview = true;
            viewView.Fields["RowColorColumnName"].HideInTable = true;
            viewView.Fields["GroupingFields"].HideInTable = true;
            viewView.Fields["EditableTableName"].HideInTable = false;
            viewView.Fields["SaveHistory"].HideInTable = true;
            viewView.Fields["SaveHistory"].IsAdminPreview = true;
            viewView.Fields["CreateDateColumnName"].HideInTable = true;

            viewView.Fields["ModifiedDateColumnName"].HideInTable = true;
            viewView.Fields["CreatedByColumnName"].HideInTable = true;
            viewView.Fields["ModifiedByColumnName"].HideInTable = true;

            viewView.Fields["OrdinalColumnName"].HideInTable = true;
            viewView.Fields["OrdinalColumnName"].DisplayName = "Ordinal Column Name";
            viewView.Fields["ImageSrcColumnName"].HideInTable = true;
            #endregion
            viewView.Fields["ImageSrcColumnName"].DisplayName = "Image Src Column Name";
            viewView.Fields["DefaultSort"].HideInTable = false;
            viewView.Fields["DefaultSort"].DisplayName = "Default Sort";
            viewView.Fields["DefaultSort"].PostLabel = "(add to ORDER BY clause)";
            viewView.Fields["DefaultSort"].Width = 300;
            viewView.Fields["DefaultSort"].IsAdminPreview = true;
            viewView.Fields["DataRowView"].HideInTable = true;
            viewView.Fields["DataRowView"].DisplayName = "Row View";
            viewView.Fields["DataRowView"].Width = 120;
            ((ColumnField)viewView.Fields["DataRowView"]).EnumType = typeof(DataRowView).AssemblyQualifiedName;
            SetRoles(viewView.Fields["DataRowView"], "Developer");
            viewView.Fields["DisplayType"].HideInTable = true;
            viewView.Fields["DisplayType"].DisplayName = "Display Type";
            ((ColumnField)viewView.Fields["DisplayType"]).EnumType = typeof(DisplayType).AssemblyQualifiedName;
            viewView.Fields["DuplicationMethod"].HideInTable = true;
            viewView.Fields["DuplicationMethod"].DisplayName = "Duplication Method";
            ((ColumnField)viewView.Fields["DuplicationMethod"]).EnumType = typeof(DuplicationMethod).AssemblyQualifiedName;
            viewView.Fields["PromoteButtonDescription"].HideInTable = true;
            viewView.Fields["NewButtonDescription"].HideInTable = true;
            if (viewView.Fields.ContainsKey("ShowDisabledSteps"))
                viewView.Fields["ShowDisabledSteps"].HideInTable = true;
            viewView.Fields["EditButtonDescription"].HideInTable = true;
            viewView.Fields["DuplicateButtonDescription"].HideInTable = true;
            viewView.Fields["AddItemsButtonDescription"].HideInTable = true;
            viewView.Fields["CollapseFilter"].HideInTable = false;
            viewView.Fields["CollapseFilter"].DisplayName = "Open Filter as Collapsed";
            viewView.Fields["CollapseFilter"].IsAdminPreview = true;
            viewView.Fields["WorkspaceID"].EditInTableView = true;
            viewView.Fields["WorkspaceID"].IsAdminPreview = true;
            //viewView.Fields["WorkspaceID"].DefaultValue = Durados.DataAccess.ConfigAccess.GetWorkspaceId("Public", configDatabase.ConnectionString);
            viewView.Fields["WorkspaceID"].Required = true;
            if (viewView.Fields.ContainsKey("ReloadPage"))
            {
                ColumnField reloadPageField = ((ColumnField)viewView.Fields["ReloadPage"]);
                reloadPageField.EnumType = typeof(ReloadPage).AssemblyQualifiedName;
                reloadPageField.HideInTable = true;
            }

            if (viewView.Fields.ContainsKey("Cached"))
            {
                viewView.Fields["Cached"].HideInTable = false;
                viewView.Fields["Cached"].DisplayName = "Cache in Memory";
            }
            viewView.Fields["MaxSubGridHeight"].HideInTable = true;
            viewView.Fields["AddItemsFieldName"].HideInTable = true;
            ((ParentField)viewView.Fields["Menu_Parent"]).InlineAdding = true;
            ((ParentField)viewView.Fields["Menu_Parent"]).InlineEditing = true;

            viewView.Fields["DashboardHeight"].DisplayName = "Cards Item Height (px)";
            viewView.Fields["DashboardHeight"].Description = "Height of Cards, leave blank for auto height";
            viewView.Fields["DashboardHeight"].OrderForCreate = 32;
            viewView.Fields["DashboardHeight"].OrderForEdit = 32;
            viewView.Fields["DashboardWidth"].DisplayName = "Cards Item Width (px)";
            viewView.Fields["DashboardWidth"].Description = "Width of Cards, leave blank for default width";
            viewView.Fields["DashboardWidth"].OrderForCreate = 34;
            viewView.Fields["DashboardWidth"].OrderForEdit = 34;

            if (viewView.Fields.ContainsKey("DataDisplayType"))
            {
                viewView.Fields["DataDisplayType"].HideInTable = true;
                viewView.Fields["DataDisplayType"].DisplayName = "Data Display Type";
                ((ColumnField)viewView.Fields["DataDisplayType"]).EnumType = typeof(DataDisplayType).AssemblyQualifiedName;
                viewView.Fields["DataDisplayType"].OrderForCreate = 10;
                viewView.Fields["DataDisplayType"].OrderForEdit = 10;
                viewView.Fields["DataDisplayType"].IsAdminPreview = true;
            }

            if (viewView.Fields.ContainsKey("FilterType"))
            {
                viewView.Fields["FilterType"].HideInTable = true;
                viewView.Fields["FilterType"].DisplayName = "Filter Type";
                ((ColumnField)viewView.Fields["FilterType"]).EnumType = typeof(FilterType).AssemblyQualifiedName;
                viewView.Fields["FilterType"].OrderForCreate = 20;
                viewView.Fields["FilterType"].OrderForEdit = 20;
                viewView.Fields["FilterType"].IsAdminPreview = true;
            }

            if (viewView.Fields.ContainsKey("SortingType"))
            {
                viewView.Fields["SortingType"].HideInTable = true;
                viewView.Fields["SortingType"].DisplayName = "Sorting Type";
                ((ColumnField)viewView.Fields["SortingType"]).EnumType = typeof(SortingType).AssemblyQualifiedName;
                viewView.Fields["SortingType"].OrderForCreate = 30;
                viewView.Fields["SortingType"].OrderForEdit = 30;
                viewView.Fields["SortingType"].IsAdminPreview = true;
            }

            if (viewView.Fields.ContainsKey("EnableTableDisplay"))
            {
                viewView.Fields["EnableTableDisplay"].HideInTable = true;
                viewView.Fields["EnableTableDisplay"].DisplayName = "Enable Table Display";
                viewView.Fields["EnableTableDisplay"].DefaultValue = true;
                viewView.Fields["EnableTableDisplay"].OrderForCreate = 40;
                viewView.Fields["EnableTableDisplay"].OrderForEdit = 40;
                viewView.Fields["EnableTableDisplay"].IsAdminPreview = true;
            }

            if (viewView.Fields.ContainsKey("EnableDashboardDisplay"))
            {
                viewView.Fields["EnableDashboardDisplay"].HideInTable = true;
                viewView.Fields["EnableDashboardDisplay"].DisplayName = "Enable Cards Display";
                viewView.Fields["EnableDashboardDisplay"].DefaultValue = true;
                viewView.Fields["EnableDashboardDisplay"].OrderForCreate = 50;
                viewView.Fields["EnableDashboardDisplay"].OrderForEdit = 50;
                viewView.Fields["EnableDashboardDisplay"].IsAdminPreview = true;
            }

            if (viewView.Fields.ContainsKey("EnablePreviewDisplay"))
            {
                viewView.Fields["EnablePreviewDisplay"].HideInTable = true;
                viewView.Fields["EnablePreviewDisplay"].DisplayName = "Enable Preview Display";
                viewView.Fields["EnablePreviewDisplay"].DefaultValue = true;
                viewView.Fields["EnablePreviewDisplay"].OrderForCreate = 60;
                viewView.Fields["EnablePreviewDisplay"].OrderForEdit = 60;
                viewView.Fields["EnablePreviewDisplay"].IsAdminPreview = true;
            }
            if (viewView.Fields.ContainsKey("GroupFilterDisplayLabel"))
            {
                viewView.Fields["GroupFilterDisplayLabel"].HideInTable = true;
                viewView.Fields["GroupFilterDisplayLabel"].HideInCreate = true;
                viewView.Fields["GroupFilterDisplayLabel"].DisplayName = "Group Filter Display Label";
                viewView.Fields["GroupFilterDisplayLabel"].DefaultValue = true;
                viewView.Fields["GroupFilterDisplayLabel"].OrderForCreate = 70;
                viewView.Fields["GroupFilterDisplayLabel"].OrderForEdit = 70;
                viewView.Fields["GroupFilterDisplayLabel"].IsAdminPreview = true;
            }

            if (viewView.Fields.ContainsKey("GridDisplayType"))
            {
               // viewView.Fields["GridDisplayType"].DisplayName = "Grid Display Mode";
                viewView.Fields["GridDisplayType"].HideInTable = true;
                ((ColumnField)viewView.Fields["GridDisplayType"]).EnumType = typeof(GridDisplayType).AssemblyQualifiedName;
                viewView.Fields["GridDisplayType"].DisplayName = "Columns Width";
                viewView.Fields["GridDisplayType"].DefaultValue = GridDisplayType.BasedOnColumnWidth;
                viewView.Fields["GridDisplayType"].OrderForCreate = 35;
                viewView.Fields["GridDisplayType"].OrderForEdit = 35;
                viewView.Fields["GridDisplayType"].IsAdminPreview = true;
            }

            #region Colors Properties

            if (viewView.Fields.ContainsKey("Background"))
            {
                viewView.Fields["Background"].HideInTable = true;
                viewView.Fields["Background"].DisplayName = "Background";
                viewView.Fields["Background"].OrderForCreate = 140;
                viewView.Fields["Background"].OrderForEdit = 140;
                viewView.Fields["Background"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["Background"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;

                viewView.Fields["Background"].Seperator = true;
                viewView.Fields["Background"].SeperatorTitle = "Header/Footer";
            }

            if (viewView.Fields.ContainsKey("FontColor"))
            {
                viewView.Fields["FontColor"].HideInTable = true;
                viewView.Fields["FontColor"].DisplayName = "Text";
                viewView.Fields["FontColor"].OrderForCreate = 150;
                viewView.Fields["FontColor"].OrderForEdit = 150;
                viewView.Fields["FontColor"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["FontColor"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;
            }

            if (viewView.Fields.ContainsKey("BorderColor"))
            {
                viewView.Fields["BorderColor"].HideInTable = true;
                viewView.Fields["BorderColor"].DisplayName = "Border";
                viewView.Fields["BorderColor"].OrderForCreate = 130;
                viewView.Fields["BorderColor"].OrderForEdit = 130;
                viewView.Fields["BorderColor"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["BorderColor"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;

                viewView.Fields["BorderColor"].ColSpanInDialog = 2;
            }

            if (viewView.Fields.ContainsKey("RowBackground"))
            {
                viewView.Fields["RowBackground"].HideInTable = true;
                viewView.Fields["RowBackground"].DisplayName = "Background 2";
                viewView.Fields["RowBackground"].OrderForCreate = 110;
                viewView.Fields["RowBackground"].OrderForEdit = 110;
                viewView.Fields["RowBackground"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["RowBackground"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;


            }

            if (viewView.Fields.ContainsKey("AlterTextColor"))
            {
                viewView.Fields["AlterTextColor"].HideInTable = true;
                viewView.Fields["AlterTextColor"].DisplayName = "Text 2";
                viewView.Fields["AlterTextColor"].OrderForCreate = 120;
                viewView.Fields["AlterTextColor"].OrderForEdit = 120;
                viewView.Fields["AlterTextColor"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["AlterTextColor"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;
            }

            if (viewView.Fields.ContainsKey("HoverBackground"))
            {
                viewView.Fields["HoverBackground"].HideInTable = true;
                viewView.Fields["HoverBackground"].DisplayName = "Hover Background";
                viewView.Fields["HoverBackground"].OrderForCreate = 122;
                viewView.Fields["HoverBackground"].OrderForEdit = 122;
                viewView.Fields["HoverBackground"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["HoverBackground"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;


            }

            if (viewView.Fields.ContainsKey("HoverTextColor"))
            {
                viewView.Fields["HoverTextColor"].HideInTable = true;
                viewView.Fields["HoverTextColor"].DisplayName = "Hover Text";
                viewView.Fields["HoverTextColor"].OrderForCreate = 127;
                viewView.Fields["HoverTextColor"].OrderForEdit = 127;
                viewView.Fields["HoverTextColor"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["HoverTextColor"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;
            }

            if (viewView.Fields.ContainsKey("AlternateRowBackground"))
            {
                viewView.Fields["AlternateRowBackground"].HideInTable = true;
                viewView.Fields["AlternateRowBackground"].DisplayName = "Background 1";
                viewView.Fields["AlternateRowBackground"].OrderForCreate = 90;
                viewView.Fields["AlternateRowBackground"].OrderForEdit = 90;
                viewView.Fields["AlternateRowBackground"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["AlternateRowBackground"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;

                viewView.Fields["AlternateRowBackground"].Seperator = true;
                viewView.Fields["AlternateRowBackground"].SeperatorTitle = "Body";
            }

            if (viewView.Fields.ContainsKey("TextFontColor"))
            {
                viewView.Fields["TextFontColor"].HideInTable = true;
                viewView.Fields["TextFontColor"].DisplayName = "Text 1";
                viewView.Fields["TextFontColor"].OrderForCreate = 100;
                viewView.Fields["TextFontColor"].OrderForEdit = 100;
                viewView.Fields["TextFontColor"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["TextFontColor"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;
            }

            if (viewView.Fields.ContainsKey("ToolBoxBackground"))
            {
                viewView.Fields["ToolBoxBackground"].HideInTable = true;
                viewView.Fields["ToolBoxBackground"].DisplayName = "Background";
                viewView.Fields["ToolBoxBackground"].OrderForCreate = 170;
                viewView.Fields["ToolBoxBackground"].OrderForEdit = 170;
                viewView.Fields["ToolBoxBackground"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["ToolBoxBackground"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;

                viewView.Fields["ToolBoxBackground"].Seperator = true;
                viewView.Fields["ToolBoxBackground"].SeperatorTitle = "Toolbar";

            }


            if (viewView.Fields.ContainsKey("ToolBoxTextColor"))
            {
                viewView.Fields["ToolBoxTextColor"].HideInTable = true;
                viewView.Fields["ToolBoxTextColor"].DisplayName = "Text";
                viewView.Fields["ToolBoxTextColor"].OrderForCreate = 180;
                viewView.Fields["ToolBoxTextColor"].OrderForEdit = 180;
                viewView.Fields["ToolBoxTextColor"].IsAdminPreview = true;
                ((ColumnField)viewView.Fields["ToolBoxTextColor"]).TextHtmlControlType = TextHtmlControlType.ColorPicker;
            }

            if (viewView.Fields.ContainsKey("ApplyColorDesignToAllViews"))
            {
                viewView.Fields["ApplyColorDesignToAllViews"].HideInTable = true;
                viewView.Fields["ApplyColorDesignToAllViews"].ColSpanInDialog = 2;
                viewView.Fields["ApplyColorDesignToAllViews"].DisplayName = "Apply the color design to all the views";
                viewView.Fields["ApplyColorDesignToAllViews"].OrderForCreate = 200;
                viewView.Fields["ApplyColorDesignToAllViews"].OrderForEdit = 200;
                viewView.Fields["ApplyColorDesignToAllViews"].Seperator = true;
                //viewView.Fields["ApplyColorDesignToAllViews"].IsAdminPreview = true;
            }

            if (viewView.Fields.ContainsKey("ApplySkinToAllViews"))
            {
                viewView.Fields["ApplySkinToAllViews"].HideInTable = true;
                viewView.Fields["ApplySkinToAllViews"].DisplayName = "Apply the skin to all the views";
                viewView.Fields["ApplySkinToAllViews"].OrderForCreate = 200;
                viewView.Fields["ApplySkinToAllViews"].OrderForEdit = 200;
                //viewView.Fields["ApplyColorDesignToAllViews"].Seperator = true;
                //viewView.Fields["ApplyColorDesignToAllViews"].IsAdminPreview = true;
            }
            if (viewView.Fields.ContainsKey("CustomThemePath"))
            {
                viewView.Fields["CustomThemePath"].HideInTable = true;
                viewView.Fields["CustomThemePath"].DisplayName = "User Preview Custom URL";
                viewView.Fields["CustomThemePath"].OrderForCreate = 33;
                viewView.Fields["CustomThemePath"].OrderForEdit = 33;
                //viewView.Fields["ApplyColorDesignToAllViews"].Seperator = true;
                //viewView.Fields["ApplyColorDesignToAllViews"].IsAdminPreview = true;
            }

            #endregion

            //viewView.Fields["HasDashboardView"].DefaultValue = true;
            //viewView.HasDashboardView = true;
            ((View)viewView).DataDashboardView = "~/Views/Shared/Controls/DatabaseDesignerView.ascx";

            if (viewView.Fields.ContainsKey("Layout"))
            {
                ((ColumnField)viewView.Fields["Layout"]).EnumType = typeof(LayoutType).AssemblyQualifiedName;
                viewView.Fields["Layout"].ColSpanInDialog = 2;
                viewView.Fields["Layout"].DefaultValue = LayoutType.BasicGrid;
                viewView.Fields["Layout"].HideInTable = true;
                viewView.Fields["Layout"].DisplayName = "Admin layout";
                viewView.Fields["Layout"].DisplayFormat = DisplayFormat.Slider;
                viewView.Fields["Layout"].IsAdminPreview = true;
            }
            if (viewView.Fields.ContainsKey("Skin"))
            {
                ((ColumnField)viewView.Fields["Skin"]).EnumType = typeof(SkinType).AssemblyQualifiedName;
                viewView.Fields["Skin"].ColSpanInDialog = 2;
                viewView.Fields["Skin"].DefaultValue = SkinType.Skin6;
                viewView.Fields["Skin"].HideInTable = true;
                viewView.Fields["Skin"].DisplayName = "Admin Skin";
                viewView.Fields["Skin"].DisplayFormat = DisplayFormat.Slider;
                viewView.Fields["Skin"].IsAdminPreview = true;
            }
            if (viewView.Fields.ContainsKey("Theme"))
            {
                ((ColumnField)viewView.Fields["Theme"]).EnumType = typeof(ThemeType).AssemblyQualifiedName;
                viewView.Fields["Theme"].ColSpanInDialog = 2;
                viewView.Fields["Theme"].DefaultValue = ThemeType.AdminLTE;
                viewView.Fields["Theme"].HideInTable = true;
                viewView.Fields["Theme"].DisplayName = "User Preview Theme";
                viewView.Fields["Theme"].DisplayFormat = DisplayFormat.Slider;
                viewView.Fields["Theme"].IsAdminPreview = false;
            }
            if (viewView.Fields.ContainsKey("RowHeight"))
            {
                viewView.Fields["RowHeight"].HideInTable = true;
                viewView.Fields["RowHeight"].DisplayName = "Row Height [Pixels]";
                viewView.Fields["RowHeight"].IsAdminPreview = true;
                viewView.Fields["RowHeight"].DisplayFormat = DisplayFormat.GeneralNumeric;
                viewView.Fields["RowHeight"].Min = 20;
                viewView.Fields["RowHeight"].Max = 800;
            }

            if (viewView.Fields.ContainsKey("InAddItemsaddAllItems"))
            {
                viewView.Fields["InAddItemsaddAllItems"].HideInTable = true;
                viewView.Fields["InAddItemsaddAllItems"].DisplayName = "In Add Items add All Items";
                viewView.Fields["InAddItemsaddAllItems"].IsAdminPreview = false;
                viewView.Fields["InAddItemsaddAllItems"].DefaultValue = false;

            }

            #endregion

            #region Send Properties
            viewView.Fields["Send"].HideInTable = true;
            viewView.Fields["Send"].DisplayName = "Send Email";
            viewView.Fields["Send"].IsAdminPreview = true;
            viewView.Fields["SendTo"].HideInTable = true;
            viewView.Fields["SendTo"].DisplayName = "To";
            viewView.Fields["SendCc"].HideInTable = true;
            viewView.Fields["SendCc"].DisplayName = "CC";
            viewView.Fields["SendSubject"].HideInTable = true;
            viewView.Fields["SendSubject"].DisplayName = "Subject";
            viewView.Fields["SendSubject"].IsAdminPreview = true;
            viewView.Fields["SendTemplate"].HideInTable = true;
            viewView.Fields["SendTemplate"].DisplayName = "Message";

            viewView.Fields["SendTemplate"].ColSpanInDialog = 2;
            viewView.Fields["SendTemplate"].GraphicProperties = "wtextareashort";
            (viewView.Fields["SendTemplate"] as ColumnField).TextHtmlControlType = TextHtmlControlType.TextArea;


            viewView.Fields["SendTemplate"].IsAdminPreview = true;
            #endregion

            #region TreeProperties
            viewView.Fields["TreeType"].HideInTable = true;
            viewView.Fields["TreeViewName"].HideInTable = true;
            viewView.Fields["TreeRelatedFieldName"].HideInTable = true;
            viewView.Fields["TreeRoot"].HideInTable = true;
            viewView.Fields["XmlElement"].HideInTable = true;
            #endregion

            //#region Description Properties

            //viewView.Fields["NewButtonDescription"].GraphicProperties = "exwtextareawide60";
            //viewView.Fields["NewButtonDescription"].ColSpanInDialog = 2;
            //viewView.Fields["EditButtonDescription"].GraphicProperties = "exwtextareawide60";
            //viewView.Fields["EditButtonDescription"].ColSpanInDialog = 2;
            //viewView.Fields["DuplicateButtonDescription"].GraphicProperties = "exwtextareawide60";
            //viewView.Fields["DuplicateButtonDescription"].ColSpanInDialog = 2;
            //viewView.Fields["PromoteButtonDescription"].GraphicProperties = "exwtextareawide60";
            //viewView.Fields["PromoteButtonDescription"].ColSpanInDialog = 2;

            //#endregion

            #region security
            SetRoles(viewView.Fields["DuplicateMessage"], "Developer");
            SetRoles(viewView.Fields["DisplayType"], "Developer");
            SetRoles(viewView.Fields["Print"], "Developer");
            //SetRoles(viewView.Fields["Popup"], "Developer");

            SetRoles(viewView.Fields["Popup"], "Developer");
            SetRoles(viewView.Fields["RefreshOnClose"], "Developer");
            SetRoles(viewView.Fields["TabCache"], "Developer");
            SetRoles(viewView.Fields["HasChildrenRow"], "Developer");
            //SetRoles(viewView.Fields["DefaultSort"], "Developer");
            SetRoles(viewView.Fields["UseOrderForCreate"], "Developer");
            SetRoles(viewView.Fields["UseOrderForEdit"], "Developer");

            SetRoles(viewView.Fields["PromoteButtonName"], "Developer");
            SetRoles(viewView.Fields["PromoteButtonDescription"], "Developer");
            SetRoles(viewView.Fields["AddItemsButtonName"], "Developer");
            SetRoles(viewView.Fields["AddItemsButtonDescription"], "Developer");
            SetRoles(viewView.Fields["InsertButtonName"], "Developer");

            //SetRoles(viewView.Fields["HideInMenu"], "Developer");

            SetRoles(viewView.Fields["DuplicationMethod"], "Developer");

            SetRoles(viewView.Fields["HistoryNotifyList"], "Developer");
            SetRoles(viewView.Fields["MaxSubGridHeight"], "Developer");

            if (viewView.Fields.ContainsKey("InAddItemsaddAllItems"))
                SetRoles(viewView.Fields["InAddItemsaddAllItems"], "Developer");
            //SetRoles(configView.Fields["Order"], "Developer");
            //SetRoles(configView.Fields["DataRowView"], "Developer");
            #endregion

            #region Order

            #region GridOrder
            viewView.Fields["Fields_Children"].Order = -10;
            viewView.Fields["DisplayName"].Order = 5;
            if (viewView.Fields.ContainsKey("JsonName"))
                viewView.Fields["JsonName"].Order = 6;
            viewView.Fields["Name"].Order = 10;
            viewView.Fields["WorkspaceID"].Order = 15;
            //viewView.Fields["Menu_Parent"].Order = 20;
            //viewView.Fields["HideInMenu"].Order = 25;
            //viewView.Fields["Order"].Order = 30;
            viewView.Fields["DisplayColumn"].Order = 40;
            viewView.Fields["PermanentFilter"].Order = 45;
            if (viewView.Fields.ContainsKey("NosqlPermanentFilter"))
                viewView.Fields["NosqlPermanentFilter"].Order = 46;
            viewView.Fields["BaseName"].Order = 50;
            viewView.Fields["Description"].Order = 60;
            viewView.Fields["DefaultSort"].Order = 70;
            viewView.Fields["Cached"].Order = 100;
            viewView.Fields["GridEditable"].Order = 110;
            //viewView.Fields["GridEditableEnabled"].Order = 120;
            viewView.Fields["Precedent"].Order = 130;
            //viewView.Fields["CollapseFilter"].Order = 140;
            viewView.Fields["SystemView"].Order = 150;

            #endregion

            #region DialogOrder

            viewView.UseOrderForCreate = true;
            viewView.UseOrderForEdit = true;
            #region GeneralTab

            //general
            viewView.Fields["DisplayName"].OrderForCreate = 5;
            viewView.Fields["DisplayName"].OrderForEdit = 5;
            if (viewView.Fields.ContainsKey("JsonName"))
            {
                viewView.Fields["JsonName"].OrderForCreate = 6;
                viewView.Fields["JsonName"].OrderForEdit = 6;
            }
            viewView.Fields["Name"].OrderForCreate = 10;
            viewView.Fields["Name"].OrderForEdit = 10;
            viewView.Fields["EditableTableName"].OrderForCreate = 20;
            viewView.Fields["EditableTableName"].OrderForEdit = 20;
            if (viewView.Fields.ContainsKey("Layout"))
            {
                viewView.Fields["Layout"].OrderForCreate = 30;
                viewView.Fields["Layout"].OrderForEdit = 30;
            }
            if (viewView.Fields.ContainsKey("Skin"))
            {
                viewView.Fields["Skin"].OrderForCreate = 35;
                viewView.Fields["Skin"].OrderForEdit = 35;
            }
            if (viewView.Fields.ContainsKey("Theme"))
            {
                viewView.Fields["Theme"].OrderForCreate = 32;
                viewView.Fields["Theme"].OrderForEdit = 32;
            }
            if (viewView.Fields.ContainsKey("ApplySkinToAllViews"))
            {
                viewView.Fields["ApplySkinToAllViews"].OrderForCreate = 36;
                viewView.Fields["ApplySkinToAllViews"].OrderForEdit = 36;
            }
            if (viewView.Fields.ContainsKey("CustomThemePath"))
            {
                viewView.Fields["CustomThemePath"].OrderForCreate = 33;
                viewView.Fields["CustomThemePath"].OrderForEdit = 33;
            }
            if (viewView.Fields.ContainsKey("RowHeight"))
            {
                viewView.Fields["RowHeight"].OrderForCreate = 37;
                viewView.Fields["RowHeight"].OrderForEdit = 37;
            }
            viewView.Fields["DisplayColumn"].OrderForEdit = 40;
            viewView.Fields["DisplayColumn"].OrderForCreate = 40;
            viewView.Fields["PageSize"].OrderForEdit = 50;
            viewView.Fields["PageSize"].OrderForCreate = 50;

            viewView.Fields["HideFilter"].OrderForEdit = 60;
            viewView.Fields["HideFilter"].OrderForCreate = 60;
            viewView.Fields["HideSearch"].OrderForEdit = 70;
            viewView.Fields["HideSearch"].OrderForCreate = 70;

            //menu
            //viewView.Fields["WorkspaceID"].OrderForCreate = 50;
            //viewView.Fields["WorkspaceID"].OrderForEdit = 50;
            //viewView.Fields["Menu_Parent"].OrderForEdit = 60;
            //viewView.Fields["Menu_Parent"].OrderForCreate = 60;
            //viewView.Fields["HideInMenu"].OrderForEdit = 70;
            //viewView.Fields["HideInMenu"].OrderForCreate = 70;
            //viewView.Fields["Order"].OrderForEdit = 80;
            //viewView.Fields["Order"].OrderForCreate = 80;

            //toolbar
            viewView.Fields["Popup"].OrderForEdit = 75;
            viewView.Fields["Popup"].OrderForCreate = 75;
            viewView.Fields["HideToolbar"].SeperatorTitle = "Toolbar Hide Options";
            viewView.Fields["HideToolbar"].Seperator = true;
            viewView.Fields["HideToolbar"].OrderForEdit = 80;
            viewView.Fields["HideToolbar"].OrderForCreate = 80;
            viewView.Fields["CollapseFilter"].OrderForEdit = 110;
            viewView.Fields["CollapseFilter"].OrderForCreate = 110;
            viewView.Fields["HidePager"].OrderForEdit = 95;
            viewView.Fields["HidePager"].OrderForCreate = 95;
            viewView.Fields["Send"].OrderForEdit = 100;
            viewView.Fields["Send"].OrderForCreate = 100;
            viewView.Fields["Send"].ColSpanInDialog = 2;


            viewView.Fields["ExportToCsv"].SeperatorTitle = "Toolbar Allow Options";
            viewView.Fields["ExportToCsv"].Seperator = true;
            viewView.Fields["ExportToCsv"].OrderForEdit = 130;
            viewView.Fields["ExportToCsv"].OrderForCreate = 130;
            viewView.Fields["ImportFromExcel"].OrderForEdit = 140;
            viewView.Fields["ImportFromExcel"].OrderForCreate = 140;
            viewView.Fields["SaveHistory"].OrderForEdit = 150;
            viewView.Fields["SaveHistory"].OrderForCreate = 150;

            viewView.Fields["Print"].OrderForEdit = 160;
            viewView.Fields["Print"].OrderForCreate = 160;
            viewView.Fields["GridEditable"].OrderForEdit = 170;
            viewView.Fields["GridEditable"].OrderForCreate = 170;
            viewView.Fields["GridEditableEnabled"].OrderForEdit = 180;
            viewView.Fields["GridEditableEnabled"].OrderForEdit = 180;
            #endregion

            #region BehaviorTab

            viewView.Fields["ColumnsInDialog"].OrderForCreate = 3;
            viewView.Fields["ColumnsInDialog"].OrderForEdit = 3;
            viewView.Fields["DataRowView"].OrderForCreate = 5;
            viewView.Fields["DataRowView"].OrderForEdit = 5;
            if (viewView.Fields.ContainsKey("OpenDialogMax"))
            {
                viewView.Fields["OpenDialogMax"].OrderForEdit = 7;
                viewView.Fields["OpenDialogMax"].OrderForCreate = 7;
            }

            viewView.Fields["AllowCreate"].Seperator = true;
            viewView.Fields["AllowCreate"].SeperatorTitle = "Main Functionality";
            viewView.Fields["AllowCreate"].OrderForEdit = 10;
            viewView.Fields["AllowCreate"].OrderForCreate = 10;
            viewView.Fields["AllowEdit"].OrderForEdit = 20;
            viewView.Fields["AllowEdit"].OrderForCreate = 20;
            viewView.Fields["AllowDuplicate"].OrderForEdit = 30;
            viewView.Fields["AllowDuplicate"].OrderForCreate = 30;
            viewView.Fields["AllowDelete"].OrderForEdit = 40;
            viewView.Fields["AllowDelete"].OrderForCreate = 40;
            viewView.Fields["MultiSelect"].OrderForEdit = 50;
            viewView.Fields["MultiSelect"].OrderForCreate = 50;
            //viewView.Fields["MultiSelect"].ColSpanInDialog = 2;

            viewView.Fields["PermanentFilter"].Seperator = true;
            viewView.Fields["PermanentFilter"].SeperatorTitle = "Display Behavior";
            viewView.Fields["PermanentFilter"].ColSpanInDialog = 2;
            viewView.Fields["PermanentFilter"].GraphicProperties = "wtextareashort";
            (viewView.Fields["PermanentFilter"] as ColumnField).TextHtmlControlType = TextHtmlControlType.TextArea;
            viewView.Fields["PermanentFilter"].OrderForEdit = 60;
            viewView.Fields["PermanentFilter"].OrderForCreate = 60;
            viewView.Fields["PermanentFilter"].DisplayName = "Permanent Filter";
            (viewView.Fields["PermanentFilter"] as ColumnField).CssClass += " field-dic";
            (viewView.Fields["PermanentFilter"] as ColumnField).DictionaryType = DictionaryType.PlaceHolders;
            //(viewView.Fields["PermanentFilter"] as ColumnField).DictionaryViewFieldName = "Name";
            viewView.Fields["PermanentFilter"].PostLabel = "(add to WHERE Clause)";

            if (viewView.Fields.ContainsKey("NosqlPermanentFilter"))
            {
                viewView.Fields["NosqlPermanentFilter"].Seperator = true;
                viewView.Fields["NosqlPermanentFilter"].ColSpanInDialog = 2;
                viewView.Fields["NosqlPermanentFilter"].GraphicProperties = "wtextareashort";
                (viewView.Fields["NosqlPermanentFilter"] as ColumnField).TextHtmlControlType = TextHtmlControlType.TextArea;
                viewView.Fields["NosqlPermanentFilter"].OrderForEdit = 61;
                viewView.Fields["NosqlPermanentFilter"].OrderForCreate = 61;
                viewView.Fields["NosqlPermanentFilter"].DisplayName = "Nosql Permanent Filter";
                (viewView.Fields["NosqlPermanentFilter"] as ColumnField).CssClass += " field-dic";
                (viewView.Fields["NosqlPermanentFilter"] as ColumnField).DictionaryType = DictionaryType.PlaceHolders;
                //(viewView.Fields["PermanentFilter"] as ColumnField).DictionaryViewFieldName = "Name";
                
            }
            viewView.Fields["DefaultSort"].OrderForEdit = 70;
            viewView.Fields["DefaultSort"].OrderForCreate = 70;

            viewView.Fields["RefreshOnClose"].OrderForEdit = 75;
            viewView.Fields["RefreshOnClose"].OrderForCreate = 75;
            viewView.Fields["TabCache"].OrderForEdit = 80;
            viewView.Fields["TabCache"].OrderForCreate = 80;
            viewView.Fields["HasChildrenRow"].OrderForEdit = 90;
            viewView.Fields["HasChildrenRow"].OrderForCreate = 90;
            viewView.Fields["UseOrderForCreate"].OrderForEdit = 110;
            viewView.Fields["UseOrderForCreate"].OrderForCreate = 110;
            viewView.Fields["UseOrderForEdit"].OrderForEdit = 120;
            viewView.Fields["UseOrderForEdit"].OrderForCreate = 120;
            if (viewView.Fields.ContainsKey("OpenSingleRow"))
            {
                Field openSingleRowField = viewView.Fields["OpenSingleRow"];
                openSingleRowField.OrderForEdit = 55;
                openSingleRowField.OrderForCreate = 55;
            }

            if (viewView.Fields.ContainsKey("OpenDialogMax"))
            {
                Field openDialogMax = viewView.Fields["OpenDialogMax"];
                openDialogMax.HideInTable = false;
                openDialogMax.DisplayName = "Open Dialog in Maximaize";

            }
            #endregion

            #region DescriptionTab

            viewView.Fields["Description"].OrderForEdit = 5;
            viewView.Fields["Description"].OrderForCreate = 5;
            viewView.Fields["NewButtonName"].OrderForEdit = 10;
            viewView.Fields["NewButtonName"].OrderForCreate = 10;
            viewView.Fields["NewButtonDescription"].OrderForEdit = 20;
            viewView.Fields["NewButtonDescription"].OrderForCreate = 20;
            viewView.Fields["EditButtonName"].OrderForEdit = 30;
            viewView.Fields["EditButtonName"].OrderForCreate = 30;
            viewView.Fields["EditButtonDescription"].OrderForEdit = 40;
            viewView.Fields["EditButtonDescription"].OrderForCreate = 40;
            viewView.Fields["DuplicateButtonName"].OrderForEdit = 50;
            viewView.Fields["DuplicateButtonName"].OrderForCreate = 50;
            viewView.Fields["DuplicateButtonDescription"].OrderForEdit = 60;
            viewView.Fields["DuplicateButtonDescription"].OrderForCreate = 60;
            viewView.Fields["PromoteButtonName"].OrderForEdit = 70;
            viewView.Fields["PromoteButtonName"].OrderForCreate = 70;
            viewView.Fields["PromoteButtonDescription"].OrderForEdit = 80;
            viewView.Fields["PromoteButtonDescription"].OrderForCreate = 80;
            viewView.Fields["AddItemsButtonName"].OrderForEdit = 90;
            viewView.Fields["AddItemsButtonName"].OrderForCreate = 90;
            viewView.Fields["AddItemsButtonDescription"].OrderForEdit = 100;
            viewView.Fields["AddItemsButtonDescription"].OrderForCreate = 100;
            viewView.Fields["DeleteButtonName"].OrderForEdit = 110;
            viewView.Fields["DeleteButtonName"].OrderForCreate = 110;
            viewView.Fields["DeleteButtonName"].ColSpanInDialog = 2;
            viewView.Fields["InsertButtonName"].OrderForEdit = 120;
            viewView.Fields["InsertButtonName"].OrderForCreate = 120;

            #endregion.

            #region PermissionTab
            viewView.Fields["WorkspaceID"].OrderForEdit = 5;
            viewView.Fields["WorkspaceID"].OrderForCreate = 5;
            viewView.Fields["Precedent"].OrderForEdit = 10;
            viewView.Fields["Precedent"].OrderForCreate = 10;
            viewView.Fields["AllowCreateRoles"].OrderForCreate = 10;
            viewView.Fields["AllowCreateRoles"].OrderForEdit = 20;
            viewView.Fields["AllowEditRoles"].OrderForCreate = 20;
            viewView.Fields["AllowEditRoles"].OrderForEdit = 30;
            viewView.Fields["AllowDeleteRoles"].OrderForCreate = 30;
            viewView.Fields["AllowDeleteRoles"].OrderForEdit = 40;
            viewView.Fields["AllowSelectRoles"].OrderForCreate = 40;
            viewView.Fields["AllowSelectRoles"].OrderForEdit = 50;
            if (viewView.Fields.ContainsKey("ViewOwnerRoles"))
            {
                viewView.Fields["ViewOwnerRoles"].OrderForCreate = 50;
                viewView.Fields["ViewOwnerRoles"].OrderForEdit = 60;
            }
            viewView.Fields["DenyCreateRoles"].OrderForCreate = 50;
            viewView.Fields["DenyCreateRoles"].OrderForEdit = 60;
            viewView.Fields["DenyEditRoles"].OrderForCreate = 60;
            viewView.Fields["DenyEditRoles"].OrderForEdit = 70;
            viewView.Fields["DenyDeleteRoles"].OrderForCreate = 70;
            viewView.Fields["DenyDeleteRoles"].OrderForEdit = 80;
            viewView.Fields["DenySelectRoles"].OrderForCreate = 80;
            viewView.Fields["DenySelectRoles"].OrderForEdit = 90;

            #endregion

            #endregion

            #endregion

        }

        protected virtual void ConfigDashboardView(Durados.Web.Mvc.Database configDatabase)//, View configView
        {
            View dashboardView = (View)configDatabase.Views["MyCharts"];

            ((ChildrenField)dashboardView.Fields["Charts_Children"]).ChildrenHtmlControlType = ChildrenHtmlControlType.Grid;
            ((ChildrenField)dashboardView.Fields["Charts_Children"]).DisplayName = "Widgets";
            ParentField Dashboards_ParentField = (ParentField)dashboardView.Fields["Dashboards_Parent"];

            // Dashboards_ParentField.ParentHtmlControlType = ParentHtmlControlType.Hidden;
            Dashboards_ParentField.DefaultValue = 0;
            Dashboards_ParentField.HideInTable = true;
            dashboardView.Fields["ID"].DisplayName = "Id";
        }
        protected virtual void ConfigChartView(Durados.Web.Mvc.Database configDatabase)//, View configView
        {
            View chartView = (View)configDatabase.Views["Chart"];
            chartView.AllowCreate = true;
            chartView.AllowDuplicate = true;
            chartView.AllowDelete = true;
            chartView.AllowCreateRoles = "Developer";

            chartView.Fields["ID"].DisplayName = "Id";

            ParentField charts_ParentField = (ParentField)chartView.Fields["Charts_Parent"];
            charts_ParentField.HideInEdit = true;
            charts_ParentField.ExcludeInUpdate = true;
            charts_ParentField.HideInTable = true;

            ColumnField nameColumn = (ColumnField)chartView.Fields["Name"];
            nameColumn.DisplayName = "Title";

            ColumnField subTitleColumn = (ColumnField)chartView.Fields["SubTitle"];
            subTitleColumn.DisplayName = "Subtitle";

            ColumnField chartTypeColumn = (ColumnField)chartView.Fields["ChartType"];
            chartTypeColumn.EnumType = typeof(ChartType).AssemblyQualifiedName;
            chartTypeColumn.DisplayName = "Type";
            //chartTypeColumn.ColSpanInDialog = 2;
            //chartTypeColumn.MultiValueExclude = "Gauge";

            ColumnField sqlColumn = (ColumnField)chartView.Fields["SQL"];
            sqlColumn.TextHtmlControlType = TextHtmlControlType.TextArea;
            sqlColumn.CssClass = "querychart";
            sqlColumn.ColSpanInDialog = 2;
            sqlColumn.Seperator = true;
            sqlColumn.SeperatorTitle = "SQL Details";
            sqlColumn.DisplayName = "Query";

            if (chartView.Fields.ContainsKey("ShowTable"))
            {
                ColumnField showTableColumn = ((ColumnField)chartView.Fields["ShowTable"]);
                showTableColumn.DisplayName = "Show Data";
                showTableColumn.Description = "By checking this option a table will be shown under the chart";
                showTableColumn.Order = 52;
                showTableColumn.ColSpanInDialog = 2;
                showTableColumn.HideInTable = true;
            }
            
            //  Category chartMainParamsCategory = new CssClass(){ Name = "Parameters", Ordinal = 10 }; 

            ColumnField xFieldColumn = (ColumnField)chartView.Fields["XField"];
            xFieldColumn.DisplayName = "X-Field";
            xFieldColumn.CssClass = "ChartParameters";
            xFieldColumn.SeperatorTitle = "Chart Parameters";
            xFieldColumn.Seperator = true;

            ColumnField yFieldColumn = (ColumnField)chartView.Fields["YField"];
            yFieldColumn.DisplayName = "Y-Field";
            yFieldColumn.CssClass = "ChartParameters";

            ColumnField xTitleColumn = (ColumnField)chartView.Fields["XTitle"];
            xTitleColumn.DisplayName = "X-Title";
            xTitleColumn.CssClass = "ChartParameters";

            ColumnField yTitleColumn = (ColumnField)chartView.Fields["YTitle"];
            yTitleColumn.DisplayName = "Y-Title";
            yTitleColumn.CssClass = "ChartParameters";

            ColumnField heightColumn = (ColumnField)chartView.Fields["Height"];
            heightColumn.DataType = DataType.Numeric;
            heightColumn.DisplayName = "Height (Pixels)";
            heightColumn.Min = 200;
            heightColumn.Max = 1200;
            heightColumn.DefaultValue = 340;
            //heightColumn.ColSpanInDialog = 2;

            ColumnField columnColumn = (ColumnField)chartView.Fields["Column"];
            columnColumn.HideInCreate = true;
            columnColumn.HideInEdit = true;

            ((ColumnField)chartView.Fields["Align"]).HideInEdit = true;
            ((ColumnField)chartView.Fields["Ordinal"]).HideInEdit = true;

            List<Field> gaugeFields = new List<Field>();
            // Category chartGaugeParamsCategory = new Category() { Name = "Gauge", Ordinal = 20 }; ;
            if (chartView.Fields.ContainsKey("GaugeGreen"))
            {

                ColumnField GaugeGreenColumn = (ColumnField)chartView.Fields["GaugeGreen"];
                GaugeGreenColumn.DisplayName = "Green Bands (eg.: 0,80 )";
                GaugeGreenColumn.Width = 100;
                GaugeGreenColumn.Order = 111;
                gaugeFields.Add(GaugeGreenColumn);
            }

            if (chartView.Fields.ContainsKey("GaugeYellow"))
            {
                ColumnField GaugeYellowColumn = (ColumnField)chartView.Fields["GaugeYellow"];
                GaugeYellowColumn.DisplayName = "Yellow Bands (eg.: 80,120 )";
                GaugeYellowColumn.Width = 100;
                GaugeYellowColumn.Order = 113;
                gaugeFields.Add(GaugeYellowColumn);
            }

            if (chartView.Fields.ContainsKey("GaugeRed"))
            {

                ColumnField GaugeRedColumn = (ColumnField)chartView.Fields["GaugeRed"];
                GaugeRedColumn.DisplayName = "Red Bands (eg.: 120,200 )";
                GaugeRedColumn.Width = 100;
                GaugeRedColumn.Order = 115;
                gaugeFields.Add(GaugeRedColumn);

            }
            if (chartView.Fields.ContainsKey("RefreshInterval"))
            {

                ColumnField RefreshIntervalColumn = (ColumnField)chartView.Fields["RefreshInterval"];
                RefreshIntervalColumn.DisplayName = "Refresh Interval (min 3 sec)";
                RefreshIntervalColumn.Width = 100;
                RefreshIntervalColumn.Order = 114;
                RefreshIntervalColumn.DefaultValue = 0;
                RefreshIntervalColumn.Precedent = true;
                RefreshIntervalColumn.AllowEditRoles = "Developer";
                RefreshIntervalColumn.AllowCreateRoles = "Developer";
                RefreshIntervalColumn.AllowSelectRoles = "Developer,Admin";
                gaugeFields.Add(RefreshIntervalColumn);
            }

            if (chartView.Fields.ContainsKey("GaugeMinValue"))
            {

                ColumnField GaugeMinValueColumn = (ColumnField)chartView.Fields["GaugeMinValue"];
                GaugeMinValueColumn.DisplayName = "Gauge Min Value";
                GaugeMinValueColumn.Width = 100;
                GaugeMinValueColumn.DefaultValue = 0;
                GaugeMinValueColumn.Order = 110;
                GaugeMinValueColumn.SeperatorTitle = "Gauge Parameters";
                GaugeMinValueColumn.Seperator = true;
                gaugeFields.Add(GaugeMinValueColumn);
            }

            if (chartView.Fields.ContainsKey("GaugeMaxValue"))
            {

                ColumnField GaugeMaxValueColumn = (ColumnField)chartView.Fields["GaugeMaxValue"];
                GaugeMaxValueColumn.DisplayName = "Gauge Max Value";
                GaugeMaxValueColumn.Width = 100;
                GaugeMaxValueColumn.DefaultValue = 100;
                GaugeMaxValueColumn.Order = 112;
                gaugeFields.Add(GaugeMaxValueColumn);
            }

            //used to control the show and hide of the gauge fields
            foreach (Field gaugeField in gaugeFields)
            {
                ((ColumnField)gaugeField).CssClass = "GaugeParameters";
            }


            chartView.Derivation = new Derivation() { DerivationField = "Precedent", Deriveds = "0;AllowSelectRoles" };
            if (chartView.Fields.ContainsKey("Precedent"))
            {
                ColumnField precedent = ((ColumnField)chartView.Fields["Precedent"]);
                precedent.DisplayName = "Override inheritable";
                precedent.Description = "By checking this option you override the Page permissions";
                precedent.Order = 125;
                precedent.HideInTable = true;
            }
            if (chartView.Fields.ContainsKey("AllowSelectRoles"))
            {
                ColumnField allowSelectRolesPage = ((ColumnField)chartView.Fields["AllowSelectRoles"]);
                allowSelectRolesPage.TextHtmlControlType = TextHtmlControlType.CheckList;
                allowSelectRolesPage.MultiValueAdditionals = "everyone,everyone";
                allowSelectRolesPage.MultiValueParentViewName = string.IsNullOrEmpty(configDatabase.RoleViewName) ? "durados_UserRole" : configDatabase.RoleViewName;
                allowSelectRolesPage.MinWidth = 200;
                allowSelectRolesPage.Order = 130;
                allowSelectRolesPage.DisplayName = "Allow Read";
                allowSelectRolesPage.Description = "Can view information and history";
                allowSelectRolesPage.DefaultValue = "Developer,Admin,User";
                allowSelectRolesPage.HideInTable = true;
                allowSelectRolesPage.ColSpanInDialog = 2;

            }
            if (chartView.Fields.ContainsKey("WorkspaceID"))
            {
                ColumnField workspaceID = (ColumnField)chartView.Fields["WorkspaceID"];
                workspaceID.TextHtmlControlType = TextHtmlControlType.DropDown;
                workspaceID.MultiValueParentViewName = "Workspace";
                workspaceID.DisplayName = "Security Workspace";
                workspaceID.MultiValueExclude = "Admin";
                workspaceID.GridEditable = false;
                workspaceID.Required = true;
                workspaceID.ColSpanInDialog = 2;
                workspaceID.Order = 120;
                workspaceID.DefaultValue = 0;
                workspaceID.Seperator = true;
                workspaceID.SeperatorTitle = "Security";
            }

            nameColumn.Order = 10;
            subTitleColumn.Order = 20;
            chartTypeColumn.Order = 30;
            heightColumn.Order = 40;
            sqlColumn.Order = 50;
            xFieldColumn.Order = 60;
            xTitleColumn.Order = 70;
            yFieldColumn.Order = 80;
            yTitleColumn.Order = 90;

        }

        protected virtual void ConfigFieldView(Durados.Web.Mvc.Database configDatabase)//, View configView
        {
            View fieldView = (View)configDatabase.Views["Field"];
            fieldView.FilterType = FilterType.Group;

            string roles = "Developer,Admin," + ViewOwenrRole;
            SetViewRoles(fieldView, roles);

            //foreach (Field field in fieldView.Fields.Values)
            //{
            //    SetRoles(field, "Developer,Admin");
            //}

            fieldView.DisplayColumn = "DisplayName";

            fieldView.DataDisplayType = DataDisplayType.Preview;
            fieldView.DataRowView = DataRowView.Accordion;
            fieldView.DashboardWidth = "180";
            fieldView.HidePager = false;
            fieldView.NewButtonName = "Add Column";
            fieldView.FilterType = FilterType.Group;
            fieldView.SortingType = SortingType.Group;

            fieldView.Fields["Excluded"].DefaultFilter = "False";
            //Fields
            //hide all the properties of the field in create
            foreach (Field field in fieldView.Fields.Values)
            {
                field.HideInCreate = true;
                field.HideInFilter = true;
                field.Sortable = false;
            }

            fieldView.Fields["DataType"].Sortable = true;

            fieldView.Fields["Category_Parent"].HideInFilter = false;
            fieldView.Fields["Fields_Parent"].HideInFilter = false;

            #region InitFieldViewProperties

            if (fieldView.Fields.ContainsKey("TextAlignment"))
            {
                ColumnField textAlignmentColumnField = (ColumnField)fieldView.Fields["TextAlignment"];
                textAlignmentColumnField.EnumType = typeof(TextAlignment).AssemblyQualifiedName;

            }

            if (fieldView.Fields.ContainsKey("ParentHtmlControlType"))
                ((ColumnField)fieldView.Fields["ParentHtmlControlType"]).EnumType = typeof(ParentHtmlControlType).AssemblyQualifiedName;
            ((ColumnField)fieldView.Fields["FieldType"]).EnumType = typeof(FieldType).AssemblyQualifiedName;
            if (fieldView.Fields.ContainsKey("ChildrenHtmlControlType"))
                ((ColumnField)fieldView.Fields["ChildrenHtmlControlType"]).EnumType = typeof(ChildrenHtmlControlType).AssemblyQualifiedName;

            if (fieldView.Fields.ContainsKey("DependencyData"))
            {
                fieldView.Fields["DependencyData"].DisplayName = "Dependencies Data";
                //fieldView.Fields["DependencyData"].Category = devCategory;

                fieldView.Fields["DataType"].DependencyData = @"0|ShortText,LongText,Numeric,Boolean,DateTime,Email,Image,Url,Html;RelatedViewName";
                fieldView.Fields["DataType"].DependencyData += "|LongText,Numeric,Boolean,DateTime,SingleSelect,MultiSelect,ImageList;FtpUpload_Parent,SpecialColumn,Encrypt,Encrypted,CertificateName,SymmetricKeyName,SymmetricKeyAlgorithm";
                fieldView.Fields["DataType"].DependencyData += "|ShortText,LongText,Numeric,Boolean,DateTime,MultiSelect;SelectionSortColumn,EditInTableView,NoHyperlink,InlineSearch,ParentHtmlControlType";
                fieldView.Fields["DataType"].DependencyData += "|ShortText,LongText,Numeric,Boolean,DateTime,SingleSelect,ImageList;ChildrenHtmlControlType";
                fieldView.Fields["DataType"].DependencyData += "|SingleSelect,MultiSelect;AdvancedFilter";


                fieldView.Fields["Encrypt"].DependencyData = "0|'true';Encrypt";
                fieldView.Fields["Encrypted"].DependencyData = "0|'true';Encrypt";


            }
            (fieldView as Durados.Web.Mvc.View).Popup = true;
            fieldView.GridEditable = true;
            fieldView.DisplayName = "Fields";
            fieldView.EnableTableDisplay = true;
            fieldView.EnablePreviewDisplay = true;
            fieldView.GridDisplayType = GridDisplayType.BasedOnColumnWidth;

            //fieldView.DisplayColumn = "Name";
            fieldView.AllowCreate = true;
            fieldView.AllowDelete = false;
            fieldView.AllowDuplicate = false;
            fieldView.UseOrderForEdit = true;
            fieldView.UseOrderForCreate = true;
            fieldView.PageSize = 50;
            ((View)fieldView).MultiSelect = true;
            fieldView.ColumnsInDialogPerCategory = "Display;2;Behaviour;2;Advanced;3;Permissions;1;Developers;3;System;3";


            fieldView.Fields["ID"].HideInTable = true;
            fieldView.Fields["ID"].DisplayName = "Id";
            fieldView.Fields["Excluded"].DisplayName = "Delete";
            fieldView.Fields["Excluded"].HideInFilter = false;

            fieldView.Fields["ParentHtmlControlType"].HideInTable = true;
            if (fieldView.Fields.ContainsKey("ParentHtmlControlType"))
                fieldView.Fields["ParentHtmlControlType"].DisplayName = "Select Type";
            if (fieldView.Fields.ContainsKey("ChildrenHtmlControlType"))
            {
                fieldView.Fields["ChildrenHtmlControlType"].SeperatorTitle = "Drop Down Configuration";
                fieldView.Fields["ChildrenHtmlControlType"].Seperator = true;
                fieldView.Fields["ChildrenHtmlControlType"].DisplayName = "Children View";
                fieldView.Fields["ChildrenHtmlControlType"].HideInTable = true;
                fieldView.Fields["ChildrenHtmlControlType"].HideInCreate = true;
                fieldView.Fields["ChildrenHtmlControlType"].HideInEdit = true;
            }
            if (fieldView.Fields.ContainsKey("AutocompleteMathing"))
            {
                fieldView.Fields["AutocompleteMathing"].HideInTable = true;
                fieldView.Fields["AutocompleteMathing"].DisplayName = "Must Match Autocomplete";
                ((ColumnField)fieldView.Fields["AutocompleteMathing"]).EnumType = typeof(AutocompleteMathing).AssemblyQualifiedName;
            }
            if (fieldView.Fields.ContainsKey("SpecialColumn"))
            {
                fieldView.Fields["SpecialColumn"].HideInTable = true;
                fieldView.Fields["SpecialColumn"].DisplayName = "Special Column";
                ((ColumnField)fieldView.Fields["SpecialColumn"]).EnumType = typeof(SpecialColumn).AssemblyQualifiedName;
            }
            if (fieldView.Fields.ContainsKey("LimitToStartAutocomplete"))
            {
                fieldView.Fields["LimitToStartAutocomplete"].HideInTable = true;
                fieldView.Fields["LimitToStartAutocomplete"].DisplayName = "Limit To Start Autocomplete";
            }
            if (fieldView.Fields.ContainsKey("Localize"))
            {
                fieldView.Fields["Localize"].HideInTable = true;
                fieldView.Fields["Localize"].DisplayName = "Localize";
            }

            if (fieldView.Fields.ContainsKey("Formula"))
            {
                fieldView.Fields["Formula"].HideInCreate = false;
                (fieldView.Fields["Formula"] as ColumnField).TextHtmlControlType = TextHtmlControlType.TextArea;
            }


            if (fieldView.Fields.ContainsKey("IsCalculated"))
            {
                fieldView.Fields["IsCalculated"].HideInEdit = true;
                fieldView.Fields["IsCalculated"].HideInFilter = false;
                fieldView.Fields["IsCalculated"].GroupFilterWidth = 40;
            }

            //fieldView.Fields["InlineEditing"].HideInTable = true;
            fieldView.Fields["InlineEditing"].DisplayName = "Inline Editing";
            //fieldView.Fields["Name"].HideInTable = true;
            fieldView.Fields["Name"].DisplayName = "Internal Name";
            fieldView.Fields["Name"].DisableInEdit = true;
            fieldView.Fields["Name"].HideInCreate = true;
            fieldView.Fields["Name"].DisableInCreate = true;
            fieldView.Fields["Name"].Sortable = true;
            fieldView.Fields["Name"].HideInFilter = false;
            fieldView.Fields["Name"].GroupFilterWidth = 160;

            fieldView.Fields["FieldType"].HideInTable = true;
            fieldView.Fields["FieldType"].DisplayName = "Field Type";
            //fieldView.Fields["DisplayName"].HideInTable = true;
            fieldView.Fields["DisplayName"].DisplayName = "Display Name";
            fieldView.Fields["DisplayName"].HideInCreate = false;
            fieldView.Fields["DisplayName"].IsAdminPreview = true;
            fieldView.Fields["DisplayName"].HideInFilter = false;
            fieldView.Fields["DisplayName"].GroupFilterWidth = 160;
            fieldView.Fields["DisplayName"].Sortable = true;
            fieldView.Fields["DisplayName"].ColSpanInDialog = 2;
            if (fieldView.Fields.ContainsKey("TextAlignment"))
                fieldView.Fields["TextAlignment"].IsAdminPreview = true;

            if (fieldView.Fields.ContainsKey("JsonName"))
            {
                fieldView.Fields["JsonName"].DisplayName = "Name";
                fieldView.Fields["JsonName"].HideInCreate = true;
                fieldView.Fields["JsonName"].IsAdminPreview = false;
                fieldView.Fields["JsonName"].HideInFilter = false;
                fieldView.Fields["JsonName"].GroupFilterWidth = 160;
                fieldView.Fields["JsonName"].Sortable = true;
            }


            fieldView.Fields["NullString"].HideInTable = true;
            fieldView.Fields["NullString"].DisplayName = "Null String";
            //fieldView.Fields["HideInTable"].HideInTable = true;
            fieldView.Fields["HideInTable"].DisplayName = "Do not Display in Grid";
            fieldView.Fields["HideInTable"].HideInFilter = false;
            fieldView.Fields["HideInTable"].GroupFilterWidth = 40;
            fieldView.Fields["HideInTable"].IsAdminPreview = true;
            //fieldView.Fields["HideInEdit"].HideInTable = true;
            fieldView.Fields["HideInEdit"].DisplayName = "Hide In Edit";
            fieldView.Fields["HideInEdit"].HideInFilter = false;
            fieldView.Fields["HideInEdit"].GroupFilterWidth = 40;
            //fieldView.Fields["HideInCreate"].HideInTable = true;
            fieldView.Fields["HideInCreate"].DisplayName = "Hide In Create";
            fieldView.Fields["HideInCreate"].GroupFilterWidth = 40;
            fieldView.Fields["HideInCreate"].HideInFilter = true;
            //fieldView.Fields["DisableInEdit"].HideInTable = true;
            fieldView.Fields["DisableInEdit"].DisplayName = "Disable In Edit";
            //fieldView.Fields["DisableInCreate"].HideInTable = true;
            fieldView.Fields["DisableInCreate"].DisplayName = "Disable In Create";

            fieldView.Fields["HideInFilter"].Seperator = true;
            fieldView.Fields["HideInFilter"].SeperatorTitle = "Filter & Sorting";
            fieldView.Fields["HideInFilter"].HideInTable = true;
            fieldView.Fields["HideInFilter"].DisplayName = "Hide Filter";
            fieldView.Fields["HideInFilter"].IsAdminPreview = true;
            //fieldView.Fields["Order"].HideInTable = true;
            fieldView.Fields["Order"].DisplayName = "Order";
            fieldView.Fields["Order"].IsAdminPreview = true;
            fieldView.Fields["Excluded"].IsAdminPreview = true;

            fieldView.Fields["OrderForCreate"].DisplayName = "Order For Create";
            fieldView.Fields["OrderForEdit"].DisplayName = "Order For Edit";
            fieldView.Fields["Sortable"].HideInTable = true;
            fieldView.Fields["Sortable"].DisplayName = "Enable Sort";
            fieldView.Fields["Sortable"].IsAdminPreview = true;
            if (fieldView.Fields.ContainsKey("ShowColumnHeader"))
            {
                fieldView.Fields["ShowColumnHeader"].HideInTable = true;
                fieldView.Fields["ShowColumnHeader"].DisplayName = "Display Column Title";
                fieldView.Fields["ShowColumnHeader"].IsAdminPreview = true;
                fieldView.Fields["ShowColumnHeader"].DefaultValue = true;
            }

            //fieldView.Fields["DefaultValue"].HideInTable = true;
            fieldView.Fields["DefaultValue"].DisplayName = "Default Value";
            fieldView.Fields["ColSpanInDialog"].HideInTable = true;
            fieldView.Fields["ColSpanInDialog"].DisplayName = "Column Span In Dialog";
            //((ColumnField)fieldView.Fields["ColSpanInDialog"]).EnumType = typeof(ColSpanInDialog).AssemblyQualifiedName;
            fieldView.Fields["DenyCreateRoles"].HideInTable = true;
            fieldView.Fields["DenyCreateRoles"].DisplayName = "Deny Create Roles";
            fieldView.Fields["DenyEditRoles"].HideInTable = true;
            fieldView.Fields["DenyEditRoles"].DisplayName = "Deny Edit Roles";
            fieldView.Fields["DenySelectRoles"].HideInTable = true;
            fieldView.Fields["DenySelectRoles"].DisplayName = "Deny Read Roles";

            if (fieldView.Fields.ContainsKey("AllowCreateRoles"))
            {
                fieldView.Fields["AllowCreateRoles"].HideInTable = true;
                fieldView.Fields["AllowCreateRoles"].DisplayName = "Allow Create Roles";
            }
            if (fieldView.Fields.ContainsKey("AllowEditRoles"))
            {
                fieldView.Fields["AllowEditRoles"].HideInTable = true;
                fieldView.Fields["AllowEditRoles"].DisplayName = "Allow Edit Roles";
            }
            if (fieldView.Fields.ContainsKey("AllowSelectRoles"))
            {
                fieldView.Fields["AllowSelectRoles"].HideInTable = true;
                fieldView.Fields["AllowSelectRoles"].DisplayName = "Allow Read Roles";
            }


            //fieldView.Fields["Fields_Parent"].HideInTable = true;
            //((ParentField)fieldView.Fields["Fields_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
            fieldView.Fields["Fields_Parent"].DisplayName = "View";
            fieldView.Fields["Fields_Parent"].DisableInEdit = true;
            fieldView.Fields["Fields_Parent"].NoHyperlink = false;
            fieldView.Fields["Fields_Parent"].HideInCreate = false;
            fieldView.Fields["Fields_Parent"].EditInTableView = true;
            fieldView.Fields["Fields_Parent"].GroupFilterWidth = 160;
            //fieldView.Fields["Fields_Parent"].DisableInCreate = true;
            //fieldView.Fields["GraphicProperties"].HideInTable = true;
            fieldView.Fields["GraphicProperties"].DisplayName = "Form CSS Class";
            ((ColumnField)fieldView.Fields["GraphicProperties"]).TextHtmlControlType = TextHtmlControlType.Autocomplete;
            ColumnField graphicsProp = ((ColumnField)fieldView.Fields["GraphicProperties"]);
            graphicsProp.MultiValueAdditionals = @"exwtextareawide,wtextareasmall,wtextareashort,exwtextareawide60,wtextarealarge1100";
            graphicsProp.MultiValueAdditionals += ",exwtextareawide,wtextareawide,d_MRD,textboxmid,d_fieldContainer600,exwtextareawide60,exwtextareawide60,autocompletelong";
            graphicsProp.AutocompleteMathing = AutocompleteMathing.Contains;
            //fieldView.Fields["ContainerGraphicProperties"].HideInTable = true;
            fieldView.Fields["ContainerGraphicProperties"].DisplayName = "Grid CSS Class";
            fieldView.Fields["ContainerGraphicProperties"].HideInFilter = false;
            fieldView.Fields["ContainerGraphicProperties"].GroupFilterWidth = 160;
            fieldView.Fields["Upload_Parent"].HideInTable = true;
            fieldView.Fields["Upload_Parent"].DisplayName = "Upload";
            ((ParentField)fieldView.Fields["Upload_Parent"]).InlineAdding = true;
            ((ParentField)fieldView.Fields["Upload_Parent"]).InlineEditing = true;


            fieldView.Fields["EnumType"].HideInTable = true;
            fieldView.Fields["EnumType"].DisplayName = "Enum Type";
            //fieldView.Fields["StringConversionFormat"].HideInTable = true;
            //fieldView.Fields["StringConversionFormat"].DisplayName = "Conversion Format";

            if (fieldView.Fields.ContainsKey("NoHyperlink"))
            {
                fieldView.Fields["NoHyperlink"].HideInTable = true;
                fieldView.Fields["NoHyperlink"].DisplayName = "Hide Hyperlink";
            }
            if (fieldView.Fields.ContainsKey("RadioColumns"))
            {
                fieldView.Fields["RadioColumns"].HideInTable = true;
                fieldView.Fields["RadioColumns"].DisplayName = "Radio Columns";
            }
            if (fieldView.Fields.ContainsKey("InlineAdding"))
            {
                //fieldView.Fields["InlineAdding"].HideInTable = true;
                fieldView.Fields["InlineAdding"].DisplayName = "Inline Adding";
            }
            if (fieldView.Fields.ContainsKey("BaseFieldName"))
            {
                fieldView.Fields["BaseFieldName"].HideInTable = true;
                fieldView.Fields["BaseFieldName"].DisplayName = "Base Field Name";
            }
            //fieldView.Fields["ExcludeInUpdate"].HideInTable = true;
            fieldView.Fields["ExcludeInUpdate"].DisplayName = "Exclude In Update";
            //fieldView.Fields["ExcludeInInsert"].HideInTable = true;
            fieldView.Fields["ExcludeInInsert"].DisplayName = "Exclude In Insert";
            fieldView.Fields["DisableInFilter"].HideInTable = true;
            fieldView.Fields["DisableInFilter"].DisplayName = "Disable In Filter";
            fieldView.Fields["BooleanHtmlControlType"].HideInTable = true;
            fieldView.Fields["BooleanHtmlControlType"].DisplayName = "Boolean Html Control Type";
            ((ColumnField)fieldView.Fields["BooleanHtmlControlType"]).EnumType = typeof(BooleanHtmlControlType).AssemblyQualifiedName;
            if (fieldView.Fields.ContainsKey("LabelContentLayout"))
                ((ColumnField)fieldView.Fields["LabelContentLayout"]).EnumType = typeof(Orientation).AssemblyQualifiedName;
            if (fieldView.Fields.ContainsKey("UpdateParent"))
                ((ColumnField)fieldView.Fields["UpdateParent"]).EnumType = typeof(UpdateParent).AssemblyQualifiedName;
            fieldView.Fields["RadioOrientation"].HideInTable = true;
            fieldView.Fields["RadioOrientation"].DisplayName = "Radio Orientation";
            ((ColumnField)fieldView.Fields["RadioOrientation"]).EnumType = typeof(Orientation).AssemblyQualifiedName;
            fieldView.Fields["Rich"].HideInTable = true;
            if (fieldView.Fields.ContainsKey("DependencyFieldName"))
            {
                fieldView.Fields["DependencyFieldName"].HideInTable = true;
                fieldView.Fields["DependencyFieldName"].DisplayName = "Dependency Field Name";
            }
            if (fieldView.Fields.ContainsKey("InsideTriggerFieldName"))
            {
                fieldView.Fields["InsideTriggerFieldName"].HideInTable = true;
                fieldView.Fields["InsideTriggerFieldName"].DisplayName = "Inside Trigger Field Name";
            }
            if (fieldView.Fields.ContainsKey("Category_Parent"))
            {
                fieldView.Fields["Category_Parent"].DisplayName = "Category";
                fieldView.Fields["Category_Parent"].GroupFilterWidth = 160;
                ((ParentField)fieldView.Fields["Category_Parent"]).InlineAdding = true;
                ((ParentField)fieldView.Fields["Category_Parent"]).InlineEditing = true;
                ((ParentField)fieldView.Fields["Category_Parent"]).ColSpanInDialog = 2;
            }

            if (fieldView.Fields.ContainsKey("Dialog"))
            {
                fieldView.Fields["Dialog"].HideInTable = true;
                fieldView.Fields["Dialog"].DisplayName = "Rich edit on grid";
            }
            if (fieldView.Fields.ContainsKey("PartialLength"))
            {
                fieldView.Fields["PartialLength"].HideInTable = true;
                fieldView.Fields["PartialLength"].DisplayName = "Long text length (letters)";
            }
            if (fieldView.Fields.ContainsKey("ShowDependencyInTable"))
            {
                fieldView.Fields["ShowDependencyInTable"].HideInTable = true;
                fieldView.Fields["ShowDependencyInTable"].DisplayName = "Show Dependency In Table";
            }
            if (fieldView.Fields.ContainsKey("Custom"))
            {
                fieldView.Fields["Custom"].HideInTable = true;
            }
            if (fieldView.Fields.ContainsKey("MultiFilter"))
            {
                fieldView.Fields["MultiFilter"].HideInTable = true;
                fieldView.Fields["MultiFilter"].DisplayName = "Multi Filter";
            }
            if (fieldView.Fields.ContainsKey("InlineDuplicate"))
            {
                fieldView.Fields["InlineDuplicate"].HideInTable = true;
                fieldView.Fields["InlineDuplicate"].DisplayName = "Inline Duplicate";
            }
            if (fieldView.Fields.ContainsKey("InlineSearch"))
            {
                fieldView.Fields["InlineSearch"].HideInTable = true;
                fieldView.Fields["InlineSearch"].DisplayName = "Inline Search";
            }
            if (fieldView.Fields.ContainsKey("InlineSearchView"))
            {
                fieldView.Fields["InlineSearchView"].HideInTable = true;
                fieldView.Fields["InlineSearchView"].DisplayName = "Inline Search View";
            }
            if (fieldView.Fields.ContainsKey("SelectionSortColumn"))
            {
                fieldView.Fields["SelectionSortColumn"].HideInTable = true;
                fieldView.Fields["SelectionSortColumn"].DisplayName = "Dropdown Sort Column";
            }
            if (fieldView.Fields.ContainsKey("DefaultFilter"))
            {
                fieldView.Fields["DefaultFilter"].HideInTable = true;
                fieldView.Fields["DefaultFilter"].DisplayName = "Default Filter";
            }

            if (fieldView.Fields.ContainsKey("GroupFilterDisplayLabel"))
            {
                fieldView.Fields["GroupFilterDisplayLabel"].HideInTable = true;
                fieldView.Fields["GroupFilterDisplayLabel"].DisplayName = "Group Filter Display Label";
                fieldView.Fields["GroupFilterDisplayLabel"].DefaultValue = GroupFilterDisplayLabel.Inherit;
                ((ColumnField)fieldView.Fields["GroupFilterDisplayLabel"]).EnumType = typeof(GroupFilterDisplayLabel).AssemblyQualifiedName;
                fieldView.Fields["GroupFilterDisplayLabel"].HideInCreate = true;
                fieldView.Fields["GroupFilterDisplayLabel"].OrderForEdit = 120;
                fieldView.Fields["GroupFilterDisplayLabel"].OrderForCreate = 120;
            }

            /*
            if (fieldView.Fields.ContainsKey("Validation_Parent"))
            {
                fieldView.Fields["Validation_Parent"].HideInTable = true;
                fieldView.Fields["Validation_Parent"].DisplayName = "Validation Rules";
                fieldView.Fields["Validation_Parent"].HideInEdit = false;
                ((ParentField)fieldView.Fields["Validation_Parent"]).InlineEditing = true;

                fieldView.Fields["Validation_Parent"].HideInCreate = true;
                //((ParentField)fieldView.Fields["Validation_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Autocomplete;
            }*/
            if (fieldView.Fields.ContainsKey("Required"))
            {
                //fieldView.Fields["Required"].HideInTable = true;
                fieldView.Fields["Required"].DisplayName = "Required";
                fieldView.Fields["Required"].HideInFilter = true;
                fieldView.Fields["Required"].GroupFilterWidth = 40;
            }
            if (fieldView.Fields.ContainsKey("Unique"))
            {
                //fieldView.Fields["Unique"].HideInTable = true;
                fieldView.Fields["Unique"].DisplayName = "Unique";
            }
            if (fieldView.Fields.ContainsKey("PartFromUniqueIndex"))
            {
                fieldView.Fields["PartFromUniqueIndex"].HideInTable = true;
                fieldView.Fields["PartFromUniqueIndex"].DisplayName = "Part From Unique Index";
            }
            if (fieldView.Fields.ContainsKey("BrowserAutocomplete"))
            {
                fieldView.Fields["BrowserAutocomplete"].HideInTable = true;
                fieldView.Fields["BrowserAutocomplete"].DisplayName = "BrowserAutocomplete";
            }
            if (fieldView.Fields.ContainsKey("AdvancedFilter"))
            {
                fieldView.Fields["AdvancedFilter"].HideInTable = true;
                fieldView.Fields["AdvancedFilter"].DisplayName = "Advanced Filter";
            }
            if (fieldView.Fields.ContainsKey("AllowDuplication"))
            {
                fieldView.Fields["AllowDuplication"].HideInTable = true;
                fieldView.Fields["AllowDuplication"].DisplayName = "Allow Duplication";
            }
            if (fieldView.Fields.ContainsKey("Integral"))
            {
                fieldView.Fields["Integral"].HideInTable = true;
                fieldView.Fields["Integral"].DisplayName = "Integral";
            }

            if (fieldView.Fields.ContainsKey("TextHtmlControlType"))
            {
                fieldView.Fields["TextHtmlControlType"].HideInTable = true;
                fieldView.Fields["TextHtmlControlType"].DisplayName = "Text Html Control Type";
                ((ColumnField)fieldView.Fields["TextHtmlControlType"]).EnumType = typeof(TextHtmlControlType).AssemblyQualifiedName;
            }

            if (fieldView.Fields.ContainsKey("TrimSpaces"))
            {
                fieldView.Fields["TrimSpaces"].HideInTable = true;
                fieldView.Fields["TrimSpaces"].DisplayName = "Trim Spaces";
            }
            if (fieldView.Fields.ContainsKey("LabelContentLayout"))
            {
                fieldView.Fields["LabelContentLayout"].HideInTable = true;
                fieldView.Fields["LabelContentLayout"].DisplayName = "Label Content Layout";
            }
            if (fieldView.Fields.ContainsKey("Searchable"))
            {
                fieldView.Fields["Searchable"].HideInTable = true;
                fieldView.Fields["Searchable"].DisplayName = "Searchable";
            }
            if (fieldView.Fields.ContainsKey("NoCache"))
            {
                fieldView.Fields["NoCache"].HideInTable = true;
                fieldView.Fields["NoCache"].DisplayName = "Prevent Cache on Tab";
            }
            if (fieldView.Fields.ContainsKey("CounterInitiated"))
            {
                fieldView.Fields["CounterInitiated"].HideInTable = true;
                fieldView.Fields["CounterInitiated"].DisplayName = "Counter In itiated";
            }
            if (fieldView.Fields.ContainsKey("Counter"))
            {
                fieldView.Fields["Counter"].HideInTable = true;
                fieldView.Fields["Counter"].DisplayName = "Counter";
            }
            if (fieldView.Fields.ContainsKey("LoadForBlockTemplate"))
            {
                fieldView.Fields["LoadForBlockTemplate"].HideInTable = true;
                fieldView.Fields["LoadForBlockTemplate"].DisplayName = "Include in Word template";
            }
            if (fieldView.Fields.ContainsKey("HideInTableMobile"))
            {
                fieldView.Fields["HideInTableMobile"].HideInTable = true;
                fieldView.Fields["HideInTableMobile"].DisplayName = "Hide In Table Mobile";
            }

            if (fieldView.Fields.ContainsKey("FtpUpload_Parent"))
            {
                ((ParentField)fieldView.Fields["FtpUpload_Parent"]).InlineAdding = true;
                ((ParentField)fieldView.Fields["FtpUpload_Parent"]).InlineEditing = true;
            }
            if (fieldView.Fields.ContainsKey("RelatedViewName"))
            {
                ColumnField relatedViewNameField = ((ColumnField)fieldView.Fields["RelatedViewName"]);
                //relatedViewNameField.TextHtmlControlType = TextHtmlControlType.Autocomplete;
                //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;

                //relatedViewNameField.AutocompleteConnectionString = connectionString;
                //relatedViewNameField.AutocompleteSql = (new SqlSchema()).GetTableAndViewsNamesSelectStatement();
                //relatedViewNameField.AutocompleteColumn = "Name";
                relatedViewNameField.TextHtmlControlType = TextHtmlControlType.Autocomplete;
                relatedViewNameField.AutocompleteTable = "View";
                relatedViewNameField.AutocompleteColumn = "Name";
                //relatedViewNameField.AutocompleteDisplayColumn = "DisplayName";

                relatedViewNameField.AutocompleteMathing = AutocompleteMathing.Contains;

                relatedViewNameField.HideInCreate = false;

            }

            #endregion

            #region HideInTable
            fieldView.Fields["AutocompleteColumn"].HideInTable = true;
            fieldView.Fields["AutocompleteTable"].HideInTable = true;
            fieldView.Fields["AutocompleteSql"].HideInTable = true;
            fieldView.Fields["ContainerGraphicField"].HideInTable = true;
            fieldView.Fields["TableCellMinWidth"].HideInTable = true;
            fieldView.Fields["Refresh"].HideInTable = true;
            fieldView.Fields["SaveHistory"].HideInTable = true;
            fieldView.Fields["CheckListInTableLimit"].HideInTable = true;
            fieldView.Fields["Import"].HideInTable = true;
            fieldView.Fields["Import"].DefaultValue = true;
            fieldView.Fields["MinWidth"].HideInTable = true;
            fieldView.Fields["EditInTableView"].HideInTable = true;
            fieldView.Fields["Seperator"].HideInTable = true;
            fieldView.Fields["Seperator"].DisplayName = "Add horizontal line abouve the field";
            fieldView.Fields["Width"].HideInTable = false;
            //fieldView.Fields["DisableInDuplicate"].HideInTable = true;
            fieldView.Fields["GridEditable"].HideInTable = true;
            fieldView.Fields["SeperatorTitle"].HideInTable = true;

            fieldView.Fields["MultiValueParentViewName"].HideInTable = true;
            fieldView.Fields["MultiValueAdditionals"].HideInTable = true;
            fieldView.Fields["SeperatorTitle"].HideInTable = true;
            fieldView.Fields["PreLabel"].HideInTable = true;
            fieldView.Fields["PostLabel"].HideInTable = true;
            fieldView.Fields["SubGridExport"].HideInTable = true;
            fieldView.Fields["SubGridPlacement"].HideInTable = true;
            fieldView.Fields["DropDownValueField"].HideInTable = true;
            fieldView.Fields["DropDownDisplayField"].HideInTable = true;
            fieldView.Fields["AutocompleteFilter"].HideInTable = true;


            #endregion

            //fieldView.Fields["MultiValueExclude"].HideInTable = true;
            //fieldView.Fields["MultiValueExclude"].HideInCreate = true;
            //fieldView.Fields["MultiValueExclude"].HideInEdit = true;
            //fieldView.Fields["MultiValueExclude"].HideInFilter = true;


            fieldView.Fields["CloneChildrenViewName"].HideInTable = true;
            fieldView.Fields["CloneChildrenViewName"].DisplayName = "Clone View Name";

            fieldView.Fields["MultiValueParentViewName"].DisplayName = "Drop Down Parent View Name";
            fieldView.Fields["DropDownValueField"].DisplayName = "Drop Down Value Field";
            fieldView.Fields["DropDownDisplayField"].DisplayName = "Drop Down Display Field";

            fieldView.Fields["ColSpanInDialog"].ColSpanInDialog = 1;
            fieldView.Fields["Sortable"].ColSpanInDialog = 2;
            fieldView.Fields["Description"].ColSpanInDialog = 2;
            fieldView.Fields["Description"].DisplayName = "Tooltip";
            fieldView.Fields["Description"].HideInCreate = true;
            fieldView.Fields["Description"].HideInTable = true;
            (fieldView.Fields["Description"] as ColumnField).TextHtmlControlType = TextHtmlControlType.TextArea;
            (fieldView.Fields["Description"] as ColumnField).CssClass = "wtextare-w200-h40";
            ((ColumnField)fieldView.Fields["Description"]).Rich = false;
            fieldView.Fields["Description"].IsAdminPreview = true;

            fieldView.Fields["DataType"].DisplayName = "Type";
            fieldView.Fields["DataType"].HideInTable = false;
            fieldView.Fields["DataType"].HideInCreate = false;
            fieldView.Fields["DataType"].GridEditable = false;
            fieldView.Fields["DataType"].HideInFilter = false;
            ((ColumnField)fieldView.Fields["DataType"]).EnumType = typeof(DataType).AssemblyQualifiedName;
            fieldView.Fields["DataType"].DefaultValue = "ShortText";
            fieldView.Fields["DataType"].IsAdminPreview = false;

            fieldView.Fields["DisplayFormat"].DisplayName = "Display Format";
            fieldView.Fields["DisplayFormat"].IsAdminPreview = true;
            fieldView.Fields["DisplayFormat"].HideInTable = false;
            fieldView.Fields["DisplayFormat"].HideInCreate = false;
            fieldView.Fields["DisplayFormat"].GridEditable = false;
            ((ColumnField)fieldView.Fields["DisplayFormat"]).EnumType = typeof(DisplayFormat).AssemblyQualifiedName;
            fieldView.Fields["DisplayFormat"].DefaultValue = "";
            #region HideInTable
            fieldView.Fields["DatabaseNames"].HideInTable = false;
            fieldView.Fields["DatabaseNames"].DisableInEdit = true;
            fieldView.Fields["DatabaseNames"].DisableInCreate = true;
            fieldView.Fields["DatabaseNames"].HideInCreate = true;
            fieldView.Fields["DatabaseNames"].DisableInCreate = true;
            fieldView.Fields["DatabaseNames"].DisplayName = "Database Name";
            fieldView.Fields["DatabaseNames"].ColSpanInDialog = 2;
            fieldView.Fields["DatabaseNames"].HideInFilter = false;
            fieldView.Fields["DatabaseNames"].Sortable = true;
            fieldView.Fields["DatabaseNames"].GroupFilterWidth = 160;

            fieldView.Fields["IsUnique"].HideInTable = true;
            fieldView.Fields["Min"].HideInTable = true;
            fieldView.Fields["Min"].DisplayName = "Minimum Value";
            fieldView.Fields["Max"].HideInTable = true;
            fieldView.Fields["Max"].DisplayName = "Maximum Value";
            fieldView.Fields["UpdateParent"].HideInTable = true;
            fieldView.Fields["UpdateParentInGrid"].HideInTable = true;
            fieldView.Fields["SearchFilter"].HideInTable = true;
            fieldView.Fields["HideInDerivation"].HideInTable = true;

            fieldView.Fields["Encrypt"].HideInTable = true;
            fieldView.Fields["Encrypted"].HideInTable = true;
            fieldView.Fields["CertificateName"].HideInTable = true;
            fieldView.Fields["SymmetricKeyName"].HideInTable = true;
            fieldView.Fields["SymmetricKeyAlgorithm"].HideInTable = true;

            fieldView.Fields["DisplayField"].HideInTable = true;
            fieldView.Fields["IncludeInDuplicate"].HideInTable = true;
            fieldView.Fields["OriginalFieldName"].HideInTable = true;
            fieldView.Fields["OriginalParentRelatedFieldName"].HideInTable = true;
            fieldView.Fields["DisplayField"].HideInTable = true;

            if (fieldView.Fields.ContainsKey("TextAlignment"))
            {
                ColumnField textAlignmentColumnField = (ColumnField)fieldView.Fields["TextAlignment"];
                textAlignmentColumnField.HideInTable = true;

            }
            #endregion
            //fieldView.Fields["Dashboard"].HideInTable = true;
            fieldView.Fields["Dashboard"].DisplayName = "Display in Cards";
            fieldView.Fields["Formula"].ColSpanInDialog = 2;
            fieldView.Fields["Formula"].HideInCreate = false;
            (fieldView.Fields["Formula"] as ColumnField).TextHtmlControlType = TextHtmlControlType.TextArea;
            (fieldView.Fields["Formula"] as ColumnField).CssClass = "exwtextareawide60";
            ((ColumnField)fieldView.Fields["Formula"]).Rich = false;

            fieldView.Fields["Excluded"].HideInCreate = false;
            fieldView.Fields["Required"].HideInCreate = false;
            fieldView.Fields["DefaultValue"].HideInCreate = false;
            fieldView.Fields["DefaultFilter"].HideInCreate = false;

            if (fieldView.Fields.ContainsKey("SubgridInstructions"))
            {
                ColumnField subgridInstructionsField = (ColumnField)fieldView.Fields["SubgridInstructions"];
                subgridInstructionsField.DisplayName = "Subgrid Instructions";
                subgridInstructionsField.HideInTable = true;
                subgridInstructionsField.ColSpanInDialog = 2;
                subgridInstructionsField.TextHtmlControlType = TextHtmlControlType.TextArea;
                subgridInstructionsField.CssClass = "exwtextareawide60";
                subgridInstructionsField.Rich = false;
                subgridInstructionsField.DefaultValue = "To add new {0}, Please fill the required fields and save it first";
                subgridInstructionsField.Order = 500;
                subgridInstructionsField.PostLabel = "{0} will be replaced by the Display Name";
            }

            if (fieldView.Fields.ContainsKey("AutoIncrement"))
            {
                ColumnField autoIncrementField = (ColumnField)fieldView.Fields["AutoIncrement"];
                autoIncrementField.HideInTable = false;
                autoIncrementField.Order = 610;
            }
            if (fieldView.Fields.ContainsKey("AutoIncrementSequanceName"))
            {
                ColumnField AutoIncrementSequanceNameField = (ColumnField)fieldView.Fields["AutoIncrementSequanceName"];
                AutoIncrementSequanceNameField.HideInTable = false;
                AutoIncrementSequanceNameField.Order = 620;
            }
            #region Order
            #region GridOrder
            fieldView.Fields["Fields_Parent"].Order = 5;
            fieldView.Fields["DisplayName"].Order = 7;
            if (fieldView.Fields.ContainsKey("JsonName"))
                fieldView.Fields["JsonName"].Order = 8;
            fieldView.Fields["Name"].Order = 10;
            fieldView.Fields["DatabaseNames"].Order = 12;
            fieldView.Fields["DataType"].Order = 15;
            fieldView.Fields["DisplayFormat"].Order = 20;
            fieldView.Fields["RelatedViewName"].Order = 22;
            fieldView.Fields["Category_Parent"].Order = 27;
            fieldView.Fields["Order"].Order = 30;
            fieldView.Fields["OrderForCreate"].Order = 32;
            fieldView.Fields["OrderForEdit"].Order = 34;
            fieldView.Fields["Excluded"].Order = 40;
            if (fieldView.Fields.ContainsKey("ShowColumnHeader"))
                fieldView.Fields["ShowColumnHeader"].Order = 45;
            fieldView.Fields["HideInTable"].Order = 50;
            fieldView.Fields["HideInEdit"].Order = 60;
            fieldView.Fields["HideInCreate"].Order = 70;
            fieldView.Fields["DisableInEdit"].Order = 75;
            fieldView.Fields["DisableInCreate"].Order = 80;
            fieldView.Fields["DisableInDuplicate"].Order = 85;
            fieldView.Fields["DefaultValue"].Order = 90;
            fieldView.Fields["Required"].Order = 100;
            fieldView.Fields["ExcludeInUpdate"].Order = 110;
            fieldView.Fields["ExcludeInInsert"].Order = 120;
            fieldView.Fields["InlineEditing"].Order = 130;
            fieldView.Fields["InlineAdding"].Order = 140;
            fieldView.Fields["Precedent"].Order = 150;
            fieldView.Fields["ContainerGraphicProperties"].Order = 160;
            fieldView.Fields["GraphicProperties"].Order = 170;
            fieldView.Fields["Upload_Parent"].Order = 180;

            #endregion
            #region DialogOrder
            #region GeneralTab
            fieldView.Fields["DisplayName"].OrderForCreate = 10;
            fieldView.Fields["DisplayName"].OrderForEdit = 10;
            if (fieldView.Fields.ContainsKey("JsonName"))
            {
                fieldView.Fields["JsonName"].OrderForCreate = 11;
                fieldView.Fields["JsonName"].OrderForEdit = 11;
            }
            fieldView.Fields["Name"].OrderForCreate = 20;
            fieldView.Fields["Name"].OrderForEdit = 20;
            fieldView.Fields["DatabaseNames"].OrderForEdit = 30;
            fieldView.Fields["DatabaseNames"].OrderForCreate = 30;
            fieldView.Fields["DataType"].OrderForEdit = 40;
            fieldView.Fields["DataType"].OrderForCreate = 40;
            fieldView.Fields["DisplayFormat"].OrderForEdit = 50;
            fieldView.Fields["DisplayFormat"].OrderForCreate = 50;
            fieldView.Fields["RelatedViewName"].OrderForEdit = 60;
            fieldView.Fields["RelatedViewName"].OrderForCreate = 60;
            fieldView.Fields["Formula"].OrderForEdit = 70;
            fieldView.Fields["Formula"].OrderForCreate = 70;
            fieldView.Fields["Formula"].ColSpanInDialog = 2;
            fieldView.Fields["HideInTable"].OrderForEdit = 80;
            fieldView.Fields["HideInTable"].OrderForCreate = 80;
            fieldView.Fields["Dashboard"].OrderForEdit = 84;
            fieldView.Fields["Dashboard"].OrderForCreate = 84;
            fieldView.Fields["Preview"].OrderForEdit = 86;
            fieldView.Fields["Preview"].OrderForCreate = 86;
            fieldView.Fields["Description"].OrderForEdit = 100;
            fieldView.Fields["Description"].OrderForCreate = 100;

            #endregion

            #region BehavoiurTab

            fieldView.Fields["Required"].OrderForEdit = 30;
            fieldView.Fields["Required"].OrderForCreate = 30;
            fieldView.Fields["ExcludeInInsert"].OrderForEdit = 40;
            fieldView.Fields["ExcludeInInsert"].OrderForCreate = 40;
            fieldView.Fields["ExcludeInUpdate"].OrderForEdit = 50;
            fieldView.Fields["ExcludeInUpdate"].OrderForCreate = 50;
            fieldView.Fields["Excluded"].OrderForEdit = 55;
            fieldView.Fields["Excluded"].OrderForCreate = 55;
            fieldView.Fields["DefaultValue"].OrderForEdit = 60;
            fieldView.Fields["DefaultValue"].OrderForCreate = 60;
            fieldView.Fields["DefaultFilter"].OrderForEdit = 70;
            fieldView.Fields["DefaultFilter"].OrderForCreate = 70;
            fieldView.Fields["Min"].OrderForEdit = 80;
            fieldView.Fields["Min"].OrderForCreate = 80;
            fieldView.Fields["Max"].OrderForEdit = 90;
            fieldView.Fields["Max"].OrderForCreate = 90;
            fieldView.Fields["FtpUpload_Parent"].OrderForEdit = 100;
            fieldView.Fields["FtpUpload_Parent"].OrderForCreate = 100;
            fieldView.Fields["Fields_Parent"].OrderForEdit = 110;
            fieldView.Fields["Fields_Parent"].OrderForCreate = 110;

            #endregion

            #region GridViewTab

            fieldView.Fields["Order"].OrderForEdit = 20;
            fieldView.Fields["Order"].OrderForCreate = 20;
            if (fieldView.Fields.ContainsKey("ShowColumnHeader"))
            {
                fieldView.Fields["ShowColumnHeader"].OrderForEdit = 25;
                fieldView.Fields["ShowColumnHeader"].OrderForCreate = 25;
            }
            fieldView.Fields["HideInFilter"].OrderForEdit = 30;
            fieldView.Fields["HideInFilter"].OrderForCreate = 30;
            fieldView.Fields["DisableInFilter"].OrderForEdit = 40;
            fieldView.Fields["DisableInFilter"].OrderForCreate = 40;
            fieldView.Fields["MultiFilter"].OrderForEdit = 50;
            fieldView.Fields["MultiFilter"].OrderForCreate = 50;
            fieldView.Fields["AdvancedFilter"].OrderForEdit = 60;
            fieldView.Fields["AdvancedFilter"].OrderForCreate = 60;
            fieldView.Fields["Sortable"].OrderForEdit = 70;
            fieldView.Fields["Sortable"].OrderForCreate = 70;
            fieldView.Fields["NoHyperlink"].OrderForEdit = 80;
            fieldView.Fields["NoHyperlink"].OrderForCreate = 80;
            fieldView.Fields["GridEditable"].OrderForEdit = 90;
            fieldView.Fields["GridEditable"].OrderForCreate = 90;
            fieldView.Fields["Searchable"].OrderForEdit = 100;
            fieldView.Fields["Searchable"].OrderForCreate = 100;
            fieldView.Fields["Dashboard"].OrderForEdit = 110;
            fieldView.Fields["Dashboard"].OrderForCreate = 110;
            fieldView.Fields["Dialog"].OrderForEdit = 120;
            fieldView.Fields["Dialog"].OrderForCreate = 120;
            fieldView.Fields["PartialLength"].OrderForEdit = 130;
            fieldView.Fields["PartialLength"].OrderForCreate = 130;
            fieldView.Fields["EditInTableView"].OrderForEdit = 140;
            fieldView.Fields["EditInTableView"].OrderForCreate = 140;
            fieldView.Fields["ContainerGraphicProperties"].OrderForEdit = 150;
            fieldView.Fields["ContainerGraphicProperties"].OrderForCreate = 150;

            #endregion
            #region FormDialog

            fieldView.Fields["Category_Parent"].OrderForEdit = 10;
            fieldView.Fields["Category_Parent"].OrderForCreate = 10;
            fieldView.Fields["GraphicProperties"].OrderForEdit = 20;
            fieldView.Fields["GraphicProperties"].OrderForCreate = 20;
            fieldView.Fields["OrderForCreate"].OrderForEdit = 30;
            fieldView.Fields["OrderForCreate"].OrderForCreate = 30;
            fieldView.Fields["OrderForEdit"].OrderForEdit = 40;
            fieldView.Fields["OrderForEdit"].OrderForCreate = 40;
            fieldView.Fields["HideInCreate"].OrderForEdit = 50;
            fieldView.Fields["HideInCreate"].OrderForCreate = 50;
            fieldView.Fields["HideInEdit"].OrderForEdit = 60;
            fieldView.Fields["HideInEdit"].OrderForCreate = 60;
            fieldView.Fields["DisableInEdit"].OrderForEdit = 70;
            fieldView.Fields["DisableInEdit"].OrderForCreate = 70;
            fieldView.Fields["DisableInCreate"].OrderForCreate = 80;
            fieldView.Fields["DisableInCreate"].OrderForEdit = 80;
            fieldView.Fields["IncludeInDuplicate"].OrderForCreate = 90;
            fieldView.Fields["IncludeInDuplicate"].OrderForEdit = 90;
            fieldView.Fields["DisableInDuplicate"].OrderForCreate = 100;
            fieldView.Fields["DisableInDuplicate"].OrderForEdit = 100;
            fieldView.Fields["ColSpanInDialog"].OrderForCreate = 0;
            fieldView.Fields["ColSpanInDialog"].OrderForEdit = 0;
            fieldView.Fields["Width"].OrderForEdit = 115;
            fieldView.Fields["Width"].OrderForCreate = 115;
            fieldView.Fields["Seperator"].OrderForCreate = 120;
            fieldView.Fields["Seperator"].OrderForEdit = 120;
            fieldView.Fields["SeperatorTitle"].OrderForCreate = 130;
            fieldView.Fields["SeperatorTitle"].OrderForEdit = 130;
            fieldView.Fields["PreLabel"].OrderForCreate = 132;
            fieldView.Fields["PreLabel"].OrderForEdit = 132;
            fieldView.Fields["PostLabel"].OrderForCreate = 132;
            fieldView.Fields["PostLabel"].OrderForEdit = 132;
            ((ColumnField)fieldView.Fields["PreLabel"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            ((ColumnField)fieldView.Fields["PreLabel"]).Rich = true;
            ((ColumnField)fieldView.Fields["PreLabel"]).ColSpanInDialog = 2;
            ((ColumnField)fieldView.Fields["PreLabel"]).CssClass = "wtextareawide";

            ((ColumnField)fieldView.Fields["PostLabel"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            ((ColumnField)fieldView.Fields["PostLabel"]).Rich = true;
            ((ColumnField)fieldView.Fields["PostLabel"]).ColSpanInDialog = 2;
            ((ColumnField)fieldView.Fields["PostLabel"]).CssClass = "wtextareawide";
            fieldView.Fields["PreLabel"].OrderForCreate = 132;
            fieldView.Fields["PreLabel"].OrderForEdit = 132;
            fieldView.Fields["PostLabel"].OrderForCreate = 133;
            fieldView.Fields["PostLabel"].OrderForEdit = 133;

            fieldView.Fields["Width"].DisplayName = "Width (pixels)";
            fieldView.Fields["Width"].Min = 10;
            fieldView.Fields["Width"].Max = 1200;



            fieldView.Fields["ChildrenHtmlControlType"].OrderForCreate = 140;
            fieldView.Fields["ChildrenHtmlControlType"].OrderForEdit = 140;
            fieldView.Fields["InlineSearch"].OrderForCreate = 150;
            fieldView.Fields["InlineSearch"].OrderForEdit = 150;
            fieldView.Fields["InlineAdding"].OrderForCreate = 160;
            fieldView.Fields["InlineAdding"].OrderForEdit = 160;
            fieldView.Fields["InlineEditing"].OrderForCreate = 170;
            fieldView.Fields["InlineEditing"].OrderForEdit = 170;
            fieldView.Fields["SelectionSortColumn"].OrderForCreate = 180;
            fieldView.Fields["SelectionSortColumn"].OrderForEdit = 180;
            fieldView.Fields["InlineSearchView"].OrderForCreate = 190;
            fieldView.Fields["InlineSearchView"].OrderForEdit = 190;
            fieldView.Fields["AllowDuplication"].OrderForCreate = 200;
            fieldView.Fields["AllowDuplication"].OrderForEdit = 200;
            fieldView.Fields["InlineDuplicate"].OrderForCreate = 210;
            fieldView.Fields["InlineDuplicate"].OrderForEdit = 210;
            fieldView.Fields["NoCache"].OrderForCreate = 220;
            fieldView.Fields["NoCache"].OrderForEdit = 220;
            fieldView.Fields["HideInDerivation"].OrderForCreate = 230;
            fieldView.Fields["HideInDerivation"].OrderForEdit = 230;
            fieldView.Fields["TrimSpaces"].OrderForCreate = 240;
            fieldView.Fields["TrimSpaces"].OrderForEdit = 240;
            fieldView.Fields["Custom"].OrderForCreate = 250;
            fieldView.Fields["Custom"].OrderForEdit = 250;
            fieldView.Fields["DisplayField"].OrderForCreate = 260;
            fieldView.Fields["DisplayField"].OrderForEdit = 260;

            #endregion

            #endregion
            #endregion

            // switch reorder
            ((Durados.Web.Mvc.View)fieldView).OrdinalColumnName = "Order";
            ((Durados.Web.Mvc.View)fieldView).DefaultSort = "Order";

            #region Security
            foreach (Field field in fieldView.Fields.Values)
            {
                field.Precedent = true;
                field.AllowSelectRoles = "everyone";
                field.AllowCreateRoles = "Developer,Admin";
                field.AllowEditRoles = "Developer,Admin";
                field.DenyCreateRoles = "";
                field.DenyEditRoles = "";
                field.DenySelectRoles = "";

            }
            SetRoles(fieldView.Fields["EditInTableView"], "Developer");
            SetRoles(fieldView.Fields["ContainerGraphicProperties"], "Developer");
            SetRoles(fieldView.Fields["OrderForCreate"], "Developer");
            SetRoles(fieldView.Fields["OrderForEdit"], "Developer");
            SetRoles(fieldView.Fields["SelectionSortColumn"], "Developer");
            SetRoles(fieldView.Fields["InlineSearchView"], "Developer");
            SetRoles(fieldView.Fields["AllowDuplication"], "Developer");
            SetRoles(fieldView.Fields["InlineDuplicate"], "Developer");
            SetRoles(fieldView.Fields["NoCache"], "Developer");
            SetRoles(fieldView.Fields["HideInDerivation"], "Developer");
            SetRoles(fieldView.Fields["TrimSpaces"], "Developer");
            SetRoles(fieldView.Fields["Custom"], "Developer");
            SetRoles(fieldView.Fields["DisplayField"], "Developer");
            //SetRoles(fieldView.Fields["Order"], "Developer");
            SetRoles(fieldView.Fields["NoHyperlink"], "Developer");
            SetRoles(fieldView.Fields["GridEditable"], "Developer");
            SetRoles(fieldView.Fields["Searchable"], "Developer");
            SetRoles(fieldView.Fields["GroupFilterDisplayLabel"], "Developer");
            SetRoles(fieldView.Fields["GroupFilterWidth"], "Developer");
            SetRoles(fieldView.Fields["Dialog"], "Developer");
            SetRoles(fieldView.Fields["PartialLength"], "Developer");
            SetRoles(fieldView.Fields["SubgridInstructions"], "Developer");

            #endregion

        }

        private void ConfigDatabaseView(Durados.Web.Mvc.Database configDatabase, Durados.Web.Mvc.Database database)
        {
            View databaseView = (View)configDatabase.Views["Database"];
            databaseView.Fields["ID"].HideInTable = true;
            databaseView.Fields["ID"].DisplayName = "Id";
            //databaseView.Fields["DefaultController"].HideInTable = true;
            databaseView.Fields["DefaultController"].DisplayName = "Default Controller";
            //databaseView.Fields["DefaultIndexAction"].HideInTable = true;
            databaseView.Fields["DefaultIndexAction"].DisplayName = "Default Index Action";
            //databaseView.Fields["DefaultCreateAction"].HideInTable = true;
            databaseView.Fields["DefaultCreateAction"].DisplayName = "Default Create Action";
            //databaseView.Fields["DefaultEditAction"].HideInTable = true;
            databaseView.Fields["DefaultEditAction"].DisplayName = "Default Edit Action";
            //databaseView.Fields["DefaultDeleteAction"].HideInTable = true;
            databaseView.Fields["DefaultDeleteAction"].DisplayName = "Default Delete Action";
            //databaseView.Fields["DefaultFilterAction"].HideInTable = true;
            databaseView.Fields["DefaultFilterAction"].DisplayName = "Default Filter Action";
            //databaseView.Fields["DefaultUploadAction"].HideInTable = true;
            databaseView.Fields["DefaultUploadAction"].DisplayName = "Default Upload Action";
            //databaseView.Fields["DefaultGetJsonViewAction"].HideInTable = true;
            databaseView.Fields["DefaultGetJsonViewAction"].DisplayName = "Default GetJsonView Action";
            //databaseView.Fields["DefaultAutoCompleteAction"].HideInTable = true;
            databaseView.Fields["DefaultAutoCompleteAction"].DisplayName = "Default AutoComplete Action";
            //databaseView.Fields["DefaultAutoCompleteController"].HideInTable = true;
            databaseView.Fields["DefaultAutoCompleteController"].DisplayName = "Default AutoComplete Controller";
            //databaseView.Fields["DefaultExportToCsvAction"].HideInTable = true;
            databaseView.Fields["DefaultExportToCsvAction"].DisplayName = "Default ExportToCsv Action";
            //databaseView.Fields["DefaultPrintAction"].HideInTable = true;
            databaseView.Fields["DefaultPrintAction"].DisplayName = "Default Print Action";
            databaseView.Fields["DisplayName"].DisplayName = "Display Name";
            //databaseView.Fields["ConnectionString"].DisplayName = "Connection String";
            ColumnField firstViewNameField = (ColumnField)databaseView.Fields["FirstViewName"];
            firstViewNameField.DisplayName = "First View Name";
            firstViewNameField.TextHtmlControlType = TextHtmlControlType.DropDown;
            firstViewNameField.MultiValueParentViewName = "View";
            firstViewNameField.DropDownValueField = "Name";
            firstViewNameField.DropDownDisplayField = "DisplayName";
            firstViewNameField.MultiValueExclude = database.Views.Values.Where(v => v.SystemView).Names().Delimited();
            firstViewNameField.Excluded = true;

            databaseView.Fields["DefaultPageSize"].DisplayName = "Default Page Size";
            databaseView.Fields["DateFormat"].DisplayName = "Default Date Format";
            databaseView.Fields["RequiresSSL"].DisplayName = "SSL Required (https)";
            //databaseView.Fields["MicrosoftDateFormat"].DisplayName = "Microsoft Date Format";
            if (databaseView.Fields.ContainsKey("PoweredByLogo"))
            {
                ColumnField poweredByLogoField = ((ColumnField)databaseView.Fields["PoweredByLogo"]);
                poweredByLogoField.Upload = new Upload() { FileAllowedTypes = "jpg,png,gif", FileMaxSize = 100, Override = false, Title = "UploadPoweredBy", UploadFileType = Mvc.UploadFileType.Image, UploadStorageType = Mvc.UploadStorageType.File, UploadVirtualPath = "/Uploads/" + Map.Id ?? string.Empty + "/" };
            }
            if (databaseView.Fields.ContainsKey("PoweredByUrl"))
            {
                ((ColumnField)databaseView.Fields["PoweredByUrl"]).TextHtmlControlType = TextHtmlControlType.Url;
            }

            Category displayCategory = new Category() { Name = "Display", Ordinal = 10 };
            databaseView.Fields["InsideTextSearch"].DisplayName = "Default Text Filter - Contains";

            databaseView.Fields["RequiresSSL"].Category = displayCategory;
            databaseView.Fields["DefaultPageSize"].Category = displayCategory;

            if (databaseView.Fields.ContainsKey("Config"))
                databaseView.Fields["Config"].Category = displayCategory;
            if (databaseView.Fields.ContainsKey("DateFormat"))
            {
                ColumnField dateFormatColumnField = (ColumnField)databaseView.Fields["DateFormat"];
                dateFormatColumnField.Category = displayCategory;
                dateFormatColumnField.TextHtmlControlType = TextHtmlControlType.Autocomplete;
                dateFormatColumnField.MultiValueAdditionals = @"MM/dd/yyyy,dd/MM/yyyy";
                dateFormatColumnField.AutocompleteMathing = AutocompleteMathing.Contains;
            }
            databaseView.Fields["HideMyStuff"].Category = displayCategory;
            databaseView.Fields["DefaultLast"].Category = displayCategory;
            databaseView.Fields["InsideTextSearch"].Category = displayCategory;
            databaseView.Fields["UserViewName"].Category = displayCategory;
            databaseView.Fields["RoleViewName"].Category = displayCategory;
            databaseView.Fields["UsernameFieldName"].Category = displayCategory;
            databaseView.Fields["UserGuidFieldName"].Category = displayCategory;
            ((ColumnField)databaseView.Fields["DefaultWorkspaceId"]).Category = displayCategory;

            ((ColumnField)databaseView.Fields["DefaultWorkspaceId"]).Order = 1;
            databaseView.Fields["DefaultPageSize"].Order = 2;
            databaseView.Fields["DateFormat"].Order = 3;
            databaseView.Fields["HideMyStuff"].Order = 4;
            databaseView.Fields["DefaultLast"].Order = 5;
            databaseView.Fields["RequiresSSL"].Order = 6;
            databaseView.Fields["InsideTextSearch"].Order = 16;
            databaseView.Fields["UserViewName"].Order = 20;
            databaseView.Fields["RoleViewName"].Order = 30;
            databaseView.Fields["UsernameFieldName"].Order = 40;
            databaseView.Fields["UserGuidFieldName"].Order = 50;

            if (databaseView.Fields.ContainsKey("HeaderContent"))
            {
                ColumnField headerContent = (ColumnField)databaseView.Fields["HeaderContent"];
                headerContent.Order = 60;
                headerContent.Category = displayCategory;
                headerContent.TextHtmlControlType = TextHtmlControlType.TextArea;
                headerContent.Rich = true;
                headerContent.ColSpanInDialog = 2;
                headerContent.CssClass = "exwtextareawide";
                headerContent.PostLabel = "<p class='configuration-backup' style=\"white-space: nowrap; vertical-align:middle; padding-left:20px;color:#f26522;\">Configuration:<br><a href=\"/Home/DownloadConfig\" target=\"_blank\">- Download &amp; backup</a><br/><a onclick='UploadConfig.Open(\"Database\", getMainPageGuid());' href=\"#\">- Upload &amp; restore</a><br/><a href=\"/Admin/Restart\" target=\"_blank\">- Restart</a></p>";
            }
            if (databaseView.Fields.ContainsKey("AdminEmail"))
            {
                ColumnField columnAdminEmail = (ColumnField)databaseView.Fields["AdminEmail"];
                columnAdminEmail.Order = 55;
                columnAdminEmail.ColSpanInDialog = 2;
                columnAdminEmail.Category = displayCategory;
                SetRoles(databaseView.Fields["AdminEmail"], "Developer");
            }
            if (databaseView.Fields.ContainsKey("UserPreviewUrl"))
            {
                ColumnField columnUserPreviewUrl = (ColumnField)databaseView.Fields["UserPreviewUrl"];
                columnUserPreviewUrl.Order = 0;
                columnUserPreviewUrl.ColSpanInDialog = 2;
                columnUserPreviewUrl.Category = displayCategory;
                SetRoles(columnUserPreviewUrl, "Admin,Developer");
            }
                


            Category layCategory = new Category() { Name = "Layout", Ordinal = 15 };
            ((ColumnField)databaseView.Fields["FirstViewName"]).Category = layCategory;
            ((ColumnField)databaseView.Fields["FirstViewName"]).Order = 20;
            ((ColumnField)databaseView.Fields["IsMultiLanguages"]).Category = layCategory;
            ((ColumnField)databaseView.Fields["IsMultiLanguages"]).Order = 30;
            ((ColumnField)databaseView.Fields["DoLocalizeAdmin"]).Category = layCategory;
            ((ColumnField)databaseView.Fields["DoLocalizeAdmin"]).Order = 40;
            if (databaseView.Fields.ContainsKey("PoweredByUrl"))
            {
                ((ColumnField)databaseView.Fields["PoweredByUrl"]).Category = layCategory;
                ((ColumnField)databaseView.Fields["PoweredByUrl"]).Order = 50;
            }
            if (databaseView.Fields.ContainsKey("PoweredByLogo"))
            {
                ((ColumnField)databaseView.Fields["PoweredByLogo"]).Category = layCategory;
                ((ColumnField)databaseView.Fields["PoweredByLogo"]).Order = 60;
            }

            if (databaseView.Fields.ContainsKey("DefaultExportImportFormat"))
            {
                ColumnField DefaultExportImportFormat = (ColumnField)databaseView.Fields["DefaultExportImportFormat"];
                DefaultExportImportFormat.EnumType = typeof(ExportFileType).AssemblyQualifiedName;
                DefaultExportImportFormat.Category = layCategory;
                DefaultExportImportFormat.Order = 110;
                DefaultExportImportFormat.Required = true;
                DefaultExportImportFormat.DisplayName = "Export File Format";
            }
            if (databaseView.Fields.ContainsKey("StyleSheets"))
            {
                ColumnField generalCss = (ColumnField)databaseView.Fields["StyleSheets"];
                generalCss.Category = layCategory;
            }

            Category rolesCategory = new Category() { Name = "Permissions", Ordinal = 20 };
            databaseView.Fields["DefaultAllowCreateRoles"].Category = rolesCategory;
            databaseView.Fields["DefaultAllowEditRoles"].Category = rolesCategory;
            databaseView.Fields["DefaultAllowDeleteRoles"].Category = rolesCategory;
            databaseView.Fields["DefaultAllowSelectRoles"].Category = rolesCategory;
            if (databaseView.Fields.ContainsKey("DefaultViewOwnerRoles"))
                databaseView.Fields["DefaultViewOwnerRoles"].Category = rolesCategory;

            databaseView.Fields["DefaultDenyCreateRoles"].Category = rolesCategory;
            databaseView.Fields["DefaultDenyEditRoles"].Category = rolesCategory;
            databaseView.Fields["DefaultDenyDeleteRoles"].Category = rolesCategory;
            databaseView.Fields["DefaultDenySelectRoles"].Category = rolesCategory;

            ColumnField secureLevel = (ColumnField)databaseView.Fields["SecureLevel"];
            secureLevel.Order = 390;
            secureLevel.Category = rolesCategory;
            secureLevel.EnumType = typeof(Durados.SecureLevel).AssemblyQualifiedName;
            secureLevel.HideInTable = true;
            secureLevel.DependencyData = "0|RegisteredUsers;DefaultGuestRole";

            ColumnField defaultGuestRole = (ColumnField)databaseView.Fields["DefaultGuestRole"];
            defaultGuestRole.Order = 391;
            defaultGuestRole.Category = rolesCategory;
            defaultGuestRole.Required = true;
            defaultGuestRole.TextHtmlControlType = TextHtmlControlType.DropDown;
            defaultGuestRole.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            defaultGuestRole.MultiValueExclude = "Developer,Admin";
            defaultGuestRole.HideInTable = true;

            if (databaseView.Fields.ContainsKey("EnableUserRegistration"))
            {
                ColumnField enableUserRegistration = (ColumnField)databaseView.Fields["EnableUserRegistration"];
                enableUserRegistration.Order = 500;
                enableUserRegistration.Category = rolesCategory;
                enableUserRegistration.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("NewUserDefaultRole"))
            {
                ColumnField newUserDefaultRole = (ColumnField)databaseView.Fields["NewUserDefaultRole"];
                newUserDefaultRole.Order = 510;
                newUserDefaultRole.Category = rolesCategory;
                newUserDefaultRole.Required = true;
                newUserDefaultRole.TextHtmlControlType = TextHtmlControlType.DropDown;
                newUserDefaultRole.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
                newUserDefaultRole.MultiValueExclude = "Developer,Admin";
                newUserDefaultRole.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("ApproveNewUsersManually"))
            {
                ColumnField approveNewUsersManually = (ColumnField)databaseView.Fields["ApproveNewUsersManually"];
                approveNewUsersManually.Order = 520;
                approveNewUsersManually.Category = rolesCategory;
                approveNewUsersManually.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("RegistrationRedirectUrl"))
            {
                ColumnField registrationRedirectUrl = (ColumnField)databaseView.Fields["RegistrationRedirectUrl"];
                registrationRedirectUrl.Order = 530;
                registrationRedirectUrl.Category = rolesCategory;
                registrationRedirectUrl.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("SignInRedirectUrl"))
            {
                ColumnField signInRedirectUrl = (ColumnField)databaseView.Fields["SignInRedirectUrl"];
                signInRedirectUrl.Order = 540;
                signInRedirectUrl.Category = rolesCategory;
                signInRedirectUrl.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("ForgotPasswordUrl"))
            {
                ColumnField signInRedirectUrl = (ColumnField)databaseView.Fields["ForgotPasswordUrl"];
                signInRedirectUrl.Order = 550;
                signInRedirectUrl.Category = rolesCategory;
                signInRedirectUrl.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("SignupEmailVerification"))
            {
                ColumnField signInRedirectUrl = (ColumnField)databaseView.Fields["SignupEmailVerification"];
                signInRedirectUrl.Order = 560;
                signInRedirectUrl.Category = rolesCategory;
                signInRedirectUrl.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("GoogleClientId"))
            {
                ColumnField GoogleClientId = (ColumnField)databaseView.Fields["GoogleClientId"];
                GoogleClientId.Order = 600;
                GoogleClientId.Category = rolesCategory;
                GoogleClientId.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("GoogleClientSecret"))
            {
                ColumnField GoogleClientSecret = (ColumnField)databaseView.Fields["GoogleClientSecret"];
                GoogleClientSecret.Order = 600;
                GoogleClientSecret.Category = rolesCategory;
                GoogleClientSecret.HideInTable = true;
            }


            if (databaseView.Fields.ContainsKey("TokenExpiration"))
            {
                ColumnField TokenExpiration = (ColumnField)databaseView.Fields["TokenExpiration"];
                TokenExpiration.Order = 600;
                TokenExpiration.Category = rolesCategory;
                TokenExpiration.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("UseRefreshToken"))
            {
                ColumnField UseRefreshToken = (ColumnField)databaseView.Fields["UseRefreshToken"];
                UseRefreshToken.Order = 600;
                UseRefreshToken.Category = rolesCategory;
                UseRefreshToken.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("GithubClientId"))
            {
                ColumnField GithubClientId = (ColumnField)databaseView.Fields["GithubClientId"];
                GithubClientId.Order = 600;
                GithubClientId.Category = rolesCategory;
                GithubClientId.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("GithubClientSecret"))
            {
                ColumnField GithubClientSecret = (ColumnField)databaseView.Fields["GithubClientSecret"];
                GithubClientSecret.Order = 600;
                GithubClientSecret.Category = rolesCategory;
                GithubClientSecret.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("FacebookClientId"))
            {
                ColumnField FacebookClientId = (ColumnField)databaseView.Fields["FacebookClientId"];
                FacebookClientId.Order = 600;
                FacebookClientId.Category = rolesCategory;
                FacebookClientId.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("FacebookClientSecret"))
            {
                ColumnField FacebookClientSecret = (ColumnField)databaseView.Fields["FacebookClientSecret"];
                FacebookClientSecret.Order = 600;
                FacebookClientSecret.Category = rolesCategory;
                FacebookClientSecret.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("EnableFacebook"))
            {
                ColumnField EnableFacebook = (ColumnField)databaseView.Fields["EnableFacebook"];
                EnableFacebook.Order = 600;
                EnableFacebook.Category = rolesCategory;
                EnableFacebook.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("FacebookScope"))
            {
                ColumnField FacebookScope = (ColumnField)databaseView.Fields["FacebookScope"];
                FacebookScope.Order = 600;
                FacebookScope.Category = rolesCategory;
                FacebookScope.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("AdfsClientId"))
            {
                ColumnField AdfsClientId = (ColumnField)databaseView.Fields["AdfsClientId"];
                AdfsClientId.Order = 600;
                AdfsClientId.Category = rolesCategory;
                AdfsClientId.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("AdfsResource"))
            {
                ColumnField AdfsResource = (ColumnField)databaseView.Fields["AdfsResource"];
                AdfsResource.Order = 600;
                AdfsResource.Category = rolesCategory;
                AdfsResource.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("AdfsHost"))
            {
                ColumnField AdfsHost = (ColumnField)databaseView.Fields["AdfsHost"];
                AdfsHost.Order = 600;
                AdfsHost.Category = rolesCategory;
                AdfsHost.HideInTable = true;
                AdfsHost.DisplayName = "adfsOauth2EndPoint";
            }
            if (databaseView.Fields.ContainsKey("EnableAdfs"))
            {
                ColumnField EnableAdfs = (ColumnField)databaseView.Fields["EnableAdfs"];
                EnableAdfs.Order = 600;
                EnableAdfs.Category = rolesCategory;
                EnableAdfs.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("ReturnAddressURIs"))
            {
                ColumnField ReturnAddressURIs = (ColumnField)databaseView.Fields["ReturnAddressURIs"];
                ReturnAddressURIs.Order = 600;
                ReturnAddressURIs.Category = rolesCategory;
                ReturnAddressURIs.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("MaxInvalidPasswordAttempts"))
            {
                ColumnField MaxInvalidPasswordAttempts = (ColumnField)databaseView.Fields["MaxInvalidPasswordAttempts"];
                MaxInvalidPasswordAttempts.Order = 600;
                MaxInvalidPasswordAttempts.Category = rolesCategory;
                MaxInvalidPasswordAttempts.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("MinRequiredPasswordLength"))
            {
                ColumnField MinRequiredPasswordLength = (ColumnField)databaseView.Fields["MinRequiredPasswordLength"];
                MinRequiredPasswordLength.Order = 600;
                MinRequiredPasswordLength.Category = rolesCategory;
                MinRequiredPasswordLength.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("PasswordAttemptWindow"))
            {
                ColumnField PasswordAttemptWindow = (ColumnField)databaseView.Fields["PasswordAttemptWindow"];
                PasswordAttemptWindow.Order = 600;
                PasswordAttemptWindow.Category = rolesCategory;
                PasswordAttemptWindow.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("PasswordStrengthRegularExpression"))
            {
                ColumnField PasswordStrengthRegularExpression = (ColumnField)databaseView.Fields["PasswordStrengthRegularExpression"];
                PasswordStrengthRegularExpression.Order = 600;
                PasswordStrengthRegularExpression.Category = rolesCategory;
                PasswordStrengthRegularExpression.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("AuthAppId"))
            {
                ColumnField AuthAppId = (ColumnField)databaseView.Fields["AuthAppId"];
                AuthAppId.Order = 600;
                AuthAppId.Category = rolesCategory;
                AuthAppId.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("EnableTokenRevokation"))
            {
                ColumnField EnableTokenRevokation = (ColumnField)databaseView.Fields["EnableTokenRevokation"];
                EnableTokenRevokation.Order = 600;
                EnableTokenRevokation.Category = rolesCategory;
                EnableTokenRevokation.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("AzureAdClientId"))
            {
                ColumnField AzureAdClientId = (ColumnField)databaseView.Fields["AzureAdClientId"];
                AzureAdClientId.Order = 600;
                AzureAdClientId.Category = rolesCategory;
                AzureAdClientId.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("AzureAdResource"))
            {
                ColumnField AzureAdResource = (ColumnField)databaseView.Fields["AzureAdResource"];
                AzureAdResource.Order = 600;
                AzureAdResource.Category = rolesCategory;
                AzureAdResource.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("AzureAdHost"))
            {
                ColumnField AzureAdHost = (ColumnField)databaseView.Fields["AzureAdHost"];
                AzureAdHost.Order = 600;
                AzureAdHost.Category = rolesCategory;
                AzureAdHost.HideInTable = true;
                AzureAdHost.DisplayName = "azureAdOauth2EndPoint";
            }
            if (databaseView.Fields.ContainsKey("EnableAzureAd"))
            {
                ColumnField EnableAzureAd = (ColumnField)databaseView.Fields["EnableAzureAd"];
                EnableAzureAd.Order = 600;
                EnableAzureAd.Category = rolesCategory;
                EnableAzureAd.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("TwitterClientId"))
            {
                ColumnField TwitterClientId = (ColumnField)databaseView.Fields["TwitterClientId"];
                TwitterClientId.Order = 600;
                TwitterClientId.Category = rolesCategory;
                TwitterClientId.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("TwitterClientSecret"))
            {
                ColumnField TwitterClientSecret = (ColumnField)databaseView.Fields["TwitterClientSecret"];
                TwitterClientSecret.Order = 600;
                TwitterClientSecret.Category = rolesCategory;
                TwitterClientSecret.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("EnableTwitter"))
            {
                ColumnField EnableTwitter = (ColumnField)databaseView.Fields["EnableTwitter"];
                EnableTwitter.Order = 600;
                EnableTwitter.Category = rolesCategory;
                EnableTwitter.HideInTable = true;
            }

            //if (databaseView.Fields.ContainsKey("BackandSSO"))
            //{
            //    ColumnField BackandSSO = (ColumnField)databaseView.Fields["BackandSSO"];
            //    BackandSSO.Order = 600;
            //    BackandSSO.Category = rolesCategory;
            //    BackandSSO.HideInTable = true;
            //}
            
            
            if (databaseView.Fields.ContainsKey("PkType"))
            {
                ColumnField PkType = (ColumnField)databaseView.Fields["PkType"];
                PkType.Order = 600;
                PkType.Category = rolesCategory;
                PkType.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("EnableGithub"))
            {
                ColumnField EnableGithub = (ColumnField)databaseView.Fields["EnableGithub"];
                EnableGithub.Order = 600;
                EnableGithub.Category = rolesCategory;
                EnableGithub.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("EnableGoogle"))
            {
                ColumnField EnableGoogle = (ColumnField)databaseView.Fields["EnableGoogle"];
                EnableGoogle.Order = 600;
                EnableGoogle.Category = rolesCategory;
                EnableGoogle.HideInTable = true;
            }
            if (databaseView.Fields.ContainsKey("EnableSecretKeyAccess"))
            {
                ColumnField EnableSecretKeyAccess = (ColumnField)databaseView.Fields["EnableSecretKeyAccess"];
                EnableSecretKeyAccess.Order = 600;
                EnableSecretKeyAccess.Category = rolesCategory;
                EnableSecretKeyAccess.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("DefaultLevelOfDept"))
            {
                ColumnField DefaultLevelOfDept = (ColumnField)databaseView.Fields["DefaultLevelOfDept"];
                DefaultLevelOfDept.Order = 600;
                DefaultLevelOfDept.Category = displayCategory;
                DefaultLevelOfDept.HideInTable = true;
            }
            

            Category generalCategory = new Category() { Name = "General", Ordinal = 30 };
            //View databaseView = (View)databaseView;
            Category ssrsCategory = new Category() { Name = "3rd Party", Ordinal = 50 };
            if (databaseView.Fields.ContainsKey("SsrsUsername"))
            {
                ColumnField ssrsUsernameColumnField = (ColumnField)databaseView.Fields["SsrsUsername"];
                ssrsUsernameColumnField.Category = ssrsCategory;
                ssrsUsernameColumnField.Order = 10;
                ssrsUsernameColumnField.DisplayName = "Username";
                ssrsUsernameColumnField.HideInTable = true;
                ssrsUsernameColumnField.SeperatorTitle = "SSRS";
                ssrsUsernameColumnField.Seperator = true;
            }

            if (databaseView.Fields.ContainsKey("SsrsPassword"))
            {
                ColumnField ssrsPasswordColumnField = (ColumnField)databaseView.Fields["SsrsPassword"];
                ssrsPasswordColumnField.Category = ssrsCategory;
                ssrsPasswordColumnField.Order = 20;
                ssrsPasswordColumnField.DisplayName = "Password";
                ssrsPasswordColumnField.SpecialColumn = SpecialColumn.Password;
                ssrsPasswordColumnField.HideInTable = true;
                ssrsPasswordColumnField.GridEditable = false;
                ssrsPasswordColumnField.Encrypted = true;
            }

            if (databaseView.Fields.ContainsKey("SsrsDomain"))
            {
                ColumnField ssrsDomainColumnField = (ColumnField)databaseView.Fields["SsrsDomain"];
                ssrsDomainColumnField.Category = ssrsCategory;
                ssrsDomainColumnField.Order = 30;
                ssrsDomainColumnField.DisplayName = "Domain";
                ssrsDomainColumnField.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("SsrsReportServerUrl"))
            {
                ColumnField ssrsReportServerUrlColumnField = (ColumnField)databaseView.Fields["SsrsReportServerUrl"];
                ssrsReportServerUrlColumnField.Category = ssrsCategory;
                ssrsReportServerUrlColumnField.Order = 40;
                ssrsReportServerUrlColumnField.DisplayName = "Server URL";
                ssrsReportServerUrlColumnField.TextHtmlControlType = TextHtmlControlType.Url;
                ssrsReportServerUrlColumnField.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("SsrsPath"))
            {
                ColumnField ssrsPathColumnField = (ColumnField)databaseView.Fields["SsrsPath"];
                ssrsPathColumnField.Category = ssrsCategory;
                ssrsPathColumnField.Order = 40;
                ssrsPathColumnField.DisplayName = "Path";
                ssrsPathColumnField.HideInTable = true;
            }

            if (databaseView.Fields.ContainsKey("PlanContent"))
            {
                ColumnField planContentColumnField = (ColumnField)databaseView.Fields["PlanContent"];
                planContentColumnField.DisplayName = "Change Plan Content";
                planContentColumnField.HideInTable = true;
                planContentColumnField.Rich = true;
                planContentColumnField.ColSpanInDialog = 2;
                planContentColumnField.TextHtmlControlType = TextHtmlControlType.TextArea;
                planContentColumnField.CssClass = "hwtextareawide";

            }

            //set security for the database fields
            SetViewRoles(databaseView, "Developer,Admin");

            foreach (Field field in databaseView.Fields.Values.Where(f => f.Category == null))
            {
                field.Category = generalCategory;
                field.Precedent = true;
                field.AllowSelectRoles = "Developer";
                field.AllowCreateRoles = "Developer";
                field.AllowEditRoles = "Developer";

                field.DenyCreateRoles = "";
                field.DenyEditRoles = "";
                field.DenySelectRoles = "";
                field.HideInTable = false;
            }

            SetRoles(databaseView.Fields["StyleSheets"], "Developer");
            SetRoles(databaseView.Fields["IsMultiLanguages"], "Developer");
            SetRoles(databaseView.Fields["RequiresSSL"], "Developer");
            SetRoles(databaseView.Fields["PoweredByUrl"], "Developer");
            SetRoles(databaseView.Fields["PoweredByLogo"], "Developer");
            SetRoles(databaseView.Fields["DefaultExportImportFormat"], "Developer");
            SetRoles(databaseView.Fields["DoLocalizeAdmin"], "Developer");
            SetRoles(databaseView.Fields["UserViewName"], "Developer");
            SetRoles(databaseView.Fields["RoleViewName"], "Developer");
            SetRoles(databaseView.Fields["UsernameFieldName"], "Developer");
            SetRoles(databaseView.Fields["UserGuidFieldName"], "Developer");
            SetRoles(databaseView.Fields["DefaultExportImportFormat"], "Developer");

            if (databaseView.Fields.ContainsKey("SpecialMenus_Children"))
            {
                databaseView.Fields["SpecialMenus_Children"].DisplayName = "Menus";
                databaseView.Fields["SpecialMenus_Children"].HideInTable = true;
                databaseView.Fields["SpecialMenus_Children"].Order = -1;
            }

            if (databaseView.Fields.ContainsKey("Tooltips_Children"))
            {
                databaseView.Fields["Tooltips_Children"].DisplayName = "Tooltips";
                databaseView.Fields["Tooltips_Children"].HideInTable = true;
                databaseView.Fields["Tooltips_Children"].Order = 0;
            }

            if (databaseView.Fields.ContainsKey("Crons_Children"))
            {
                databaseView.Fields["Crons_Children"].DisplayName = "Crons";
                databaseView.Fields["Crons_Children"].HideInTable = true;
                databaseView.Fields["Crons_Children"].Order = 1;
            }

            if (databaseView.Fields.ContainsKey("Dashboards_Children"))
            {
                databaseView.Fields["Dashboards_Children"].DisplayName = "Dashboards";
                databaseView.Fields["Dashboards_Children"].HideInTable = true;
                databaseView.Fields["Dashboards_Children"].Order = 2;
            }

            if (databaseView.Fields.ContainsKey("Workspaces_Children"))
            {
                databaseView.Fields["Workspaces_Children"].DisplayName = "Workspace";
                databaseView.Fields["Workspaces_Children"].HideInTable = true;
                databaseView.Fields["Workspaces_Children"].Order = 2;
            }
            if (databaseView.Fields.ContainsKey("Queries_Children"))
            {
                databaseView.Fields["Queries_Children"].DisplayName = "Queries";
                databaseView.Fields["Queries_Children"].HideInTable = true;
                databaseView.Fields["Queries_Children"].Order = 2;
            }
            if (databaseView.Fields.ContainsKey("Views_Children"))
            {
                databaseView.Fields["Views_Children"].DisplayName = "View";
                databaseView.Fields["Views_Children"].HideInTable = true;
                databaseView.Fields["Views_Children"].Order = 3;
            }

            if (databaseView.Fields.ContainsKey("Localization_Parent"))
                databaseView.Fields["Localization_Parent"].DisplayName = "Localization";
            databaseView.Fields["Views_Children"].DisplayName = "Views";

            databaseView.Fields["DisplayName"].Order = 0;
            //databaseView.Fields["ConnectionString"].Order = 1;
            databaseView.Fields["FirstViewName"].Order = 2;

            //databaseView.Fields["MicrosoftDateFormat"].Order = 5;
            if (databaseView.Fields.ContainsKey("Localization_Parent"))
                databaseView.Fields["Localization_Parent"].Order = 6;
            databaseView.Fields["Views_Children"].Order = 7;

            databaseView.ColumnsInDialogPerCategory = "Display;1;General;2;Permissions;1;Layout;1"; ;
            if (databaseView.Fields.ContainsKey("StyleSheets"))
            {
                ColumnField generalCss = (ColumnField)databaseView.Fields["StyleSheets"];
                generalCss.DisplayName = "General Css";
            }
            if (databaseView.Fields.ContainsKey("JavaScripts"))
            {
                ColumnField generalJS = (ColumnField)databaseView.Fields["JavaScripts"];
                generalJS.DisplayName = "General Java Script";
                generalJS.Category = generalCategory;
                generalJS.Excluded = true;
            }
            if (databaseView.Fields.ContainsKey("DashboardColumns"))
            {
                ColumnField dashboardColumns = (ColumnField)databaseView.Fields["DashboardColumns"];
                dashboardColumns.DisplayName = "General Java Script";
                dashboardColumns.Category = generalCategory;
                dashboardColumns.Excluded = true;
            }

            if (databaseView.Fields.ContainsKey("GrobootNotificationAccessKey"))
            {
                ColumnField GrobootNotificationAccessKeyColumn = (ColumnField)databaseView.Fields["GrobootNotificationAccessKey"];
                GrobootNotificationAccessKeyColumn.DisplayName = "PushApps API Secret Token";
                GrobootNotificationAccessKeyColumn.Category = ssrsCategory;

                GrobootNotificationAccessKeyColumn.SpecialColumn = SpecialColumn.Password;
                GrobootNotificationAccessKeyColumn.HideInTable = true;
                GrobootNotificationAccessKeyColumn.GridEditable = false;
                GrobootNotificationAccessKeyColumn.Encrypted = true;
                GrobootNotificationAccessKeyColumn.ColSpanInDialog = 2;
                GrobootNotificationAccessKeyColumn.SeperatorTitle = "Push Notification";
                GrobootNotificationAccessKeyColumn.Seperator = true;
                SetRoles(GrobootNotificationAccessKeyColumn, "Developer,Admin");
            }
            if (databaseView.Fields.ContainsKey("LogOnUrlAuth"))
            {
                ColumnField column = (ColumnField)databaseView.Fields["LogOnUrlAuth"];
                column.DisplayName = "Log On Authentication Url ";
                column.Category = ssrsCategory;
                column.Width = 500;
                column.ColSpanInDialog = 2;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.HideInTable = true;
                column.GridEditable = false;
                column.Order = 100;
                column.Seperator = true;
                column.SeperatorTitle = "External Authentication";
            }

        }


        private void ConfigAdminMenu(Durados.Web.Mvc.Database configDatabase, Durados.Web.Mvc.Database database, Durados.Web.Mvc.Database localizationDatabase)
        {
            //configDatabase.Menus.Clear();

            var workspace = database.Workspaces.Values.Where(w => w.Name == "Admin").FirstOrDefault();
            int workspaceId = -1000;

            if (workspace == null)
                return;

            workspaceId = workspace.ID;


            //values.Add("Name", view.DisplayName);
            //values.Add("Menus_Parent", menuPk);
            //values.Add("Ordinal", view.Order);
            //values.Add("ViewName", view.Name);
            //values.Add("Url", "/" + view.Controller + "/" + view.IndexAction + "/" + view.Name);

            //values.Add("WorkspaceID", workspaceIdString);
            //values.Add("LinkType", LinkType.View.ToString());

            if (workspace.SpecialMenus == null)
                return;

            workspace.SpecialMenus.Clear();

            int ordinal = 10000;
            SpecialMenu link = null;

            SpecialMenu dashboardMenu = new SpecialMenu() { ID = ordinal++, Name = "Dashboard", WorkspaceID = workspaceId };
            workspace.SpecialMenus.Add(dashboardMenu.Name, dashboardMenu);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Home/Default?workspaceId=1", Name = "Admin Home", WorkspaceID = workspaceId, LinkType = LinkType.Page, Ordinal = ordinal, ViewName = "" };
            dashboardMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Database", Name = "Default Settings", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Database" };
            dashboardMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/View", Name = "Tables & Views", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "View" };
            dashboardMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Workspace", Name = "Workspaces", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Workspace" };
            dashboardMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Home/Index/" + database.UserViewName, Name = "Users", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = database.UserViewName };
            dashboardMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Rule", Name = "Business Rules", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Rule" };
            dashboardMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Home/Index/Durados_Log", Name = "Monitor", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Durados_Log" };
            dashboardMenu.Menus.Add(link.Name, link);


            SpecialMenu designMenu = new SpecialMenu() { ID = ordinal++, Name = "Design", WorkspaceID = workspaceId };
            workspace.SpecialMenus.Add(designMenu.Name, designMenu);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Database", Name = "Default Settings", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Database" };
            designMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/View", Name = "Tables & Views", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "View" };
            designMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Field", Name = "Fields", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Field" };
            designMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Rule", Name = "Business Rules", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Rule" };
            designMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Page", Name = "Content Pages", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Page" };
            designMenu.Menus.Add(link.Name, link);

            //link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Category", Name = "Categories", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Category" };
            //designMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/FtpUpload", Name = "Upload Storage", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "FtpUpload" };
            designMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Home/Index/durados_Html", Name = "Content", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "durados_Html" };
            designMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Chart", Name = "Charts", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Chart" };
            designMenu.Menus.Add(link.Name, link);


            SpecialMenu usersAndRolesMenu = new SpecialMenu() { ID = ordinal++, Name = "Users & Roles", WorkspaceID = workspaceId };
            workspace.SpecialMenus.Add(usersAndRolesMenu.Name, usersAndRolesMenu);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Home/Index/" + database.UserViewName, Name = "Users", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = database.UserViewName };
            usersAndRolesMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Home/Index/" + (string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName), Name = "Roles", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName };
            usersAndRolesMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Workspace", Name = "Workspaces", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Workspace" };
            usersAndRolesMenu.Menus.Add(link.Name, link);


            SpecialMenu traceMenu = new SpecialMenu() { ID = ordinal++, Name = "Trace", WorkspaceID = workspaceId };
            workspace.SpecialMenus.Add(traceMenu.Name, traceMenu);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Home/Index/Durados_Log", Name = "Monitor", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Durados_Log" };
            traceMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Home/Index/durados_v_ChangeHistory", Name = "History", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "durados_v_ChangeHistory" };
            traceMenu.Menus.Add(link.Name, link);


            SpecialMenu developerMenu = new SpecialMenu() { ID = ordinal++, Name = "Developer", WorkspaceID = workspaceId };
            workspace.SpecialMenus.Add(developerMenu.Name, developerMenu);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Localization", Name = "Localization", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Localization" };
            developerMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/SiteInfo", Name = "App Info", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "SiteInfo" };
            developerMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Milestone", Name = "Milestone", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Milestone" };
            developerMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Parameter", Name = "Parameters", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Parameter" };
            developerMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Derivation", Name = "Derivation", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Derivation" };
            developerMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/ChartInfo", Name = "Chart Info", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "ChartInfo" };
            developerMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Tooltip", Name = "Tooltip", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Tooltip" };
            developerMenu.Menus.Add(link.Name, link);

            link = new SpecialMenu() { ID = ordinal++, Url = "/Admin/Index/Cron", Name = "Cron", WorkspaceID = workspaceId, LinkType = LinkType.View, Ordinal = ordinal, ViewName = "Cron" };
            developerMenu.Menus.Add(link.Name, link);

        }


        private void ConfigAdminMenuOld(Durados.Web.Mvc.Database configDatabase, Durados.Web.Mvc.Database database, Durados.Web.Mvc.Database localizationDatabase)
        {
            configDatabase.Menus.Clear();

            var workspace = database.Workspaces.Values.Where(w => w.Name == "Admin").FirstOrDefault();
            int workspaceId = -1000;
            if (workspace != null)
                workspaceId = workspace.ID;

            // I M P O R T A N T !!!!
            // If you change this category name, you must change the specific isDock code in the DataRowViewTab
            SpecialMenu designMenu = new SpecialMenu() { Name = "Design", WorkspaceID = workspaceId };
            configDatabase.Menus.Add(designMenu.Name, designMenu);

            Link generalLink = new Link() { Title = "System Defaults", ViewName = "Database" };
            designMenu.Links.Add(generalLink);

            Link viewLink = new Link() { Title = "Tables & Views", ViewName = "View" };
            designMenu.Links.Add(viewLink);

            Link fieldLink = new Link() { Title = "Fields", ViewName = "Field" };
            designMenu.Links.Add(fieldLink);

            Link pageLink = new Link() { Title = "Pages", ViewName = "Page" };
            designMenu.Links.Add(pageLink);

            Link menuLink = new Link() { Title = "Menus", ViewName = "Menu" };
            designMenu.Links.Add(menuLink);

            Link categoryLink = new Link() { Title = "Categories", ViewName = "Category" };
            designMenu.Links.Add(categoryLink);

            Link ftpUploadLink = new Link() { Title = "Upload", ViewName = "FtpUpload" };
            designMenu.Links.Add(ftpUploadLink);

            Link contentLink = new Link() { Title = "Content", ViewName = "durados_Html" };
            designMenu.Links.Add(contentLink);

            Link chartsLink = new Link() { Title = "Charts", ViewName = "Chart" };
            designMenu.Links.Add(chartsLink);

            SpecialMenu usersAndRolesMenu = new SpecialMenu() { Name = "Users & Roles", WorkspaceID = workspaceId };
            configDatabase.Menus.Add(usersAndRolesMenu.Name, usersAndRolesMenu);

            Link usersLink = new Link() { Title = "Users", ViewName = database.UserViewName };
            usersAndRolesMenu.Links.Add(usersLink);

            Link roleLink = new Link() { Title = "Roles", ViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName };
            usersAndRolesMenu.Links.Add(roleLink);

            Link workspaceLink = new Link() { Title = "Workspaces", ViewName = "Workspace" };
            usersAndRolesMenu.Links.Add(workspaceLink);

            SpecialMenu traceMenu = new SpecialMenu() { Name = "Trace", WorkspaceID = workspaceId };
            configDatabase.Menus.Add(traceMenu.Name, traceMenu);

            Link monitorLink = new Link() { Title = "Monitor", ViewName = "Durados_Log" };
            traceMenu.Links.Add(monitorLink);

            Link historyLink = new Link() { Title = "History", ViewName = "durados_v_ChangeHistory" };
            traceMenu.Links.Add(historyLink);

            SpecialMenu developerMenu = new SpecialMenu() { Name = "Developer", WorkspaceID = workspaceId };
            configDatabase.Menus.Add(developerMenu.Name, developerMenu);

            Link localizationLink = new Link() { Title = "Localization", ViewName = "Localization" };
            developerMenu.Links.Add(localizationLink);

            //if (localizationDatabase != null && !Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsDenied(localizationDatabase.DenyLocalizationConfigRoles, "everyone") && database.Localization != null && localizationDatabase.HasViewsWithNoMenu && localizationDatabase.ViewsWithNoMenu != null)
            //{
            //    SpecialMenu translationMenu = new SpecialMenu() { Name = "Translation", WorkspaceID = workspaceId };
            //    developerMenu.Menus.Add(translationMenu.Name, translationMenu);

            //    foreach (Durados.View view in localizationDatabase.ViewsWithNoMenu.Views)
            //    {
            //        Link translationLink = new Link() { Title = view.DisplayName, ViewName = view.Name };
            //        translationMenu.Links.Add(translationLink);
            //    }
            //}

            Link siteInfoLink = new Link() { Title = "App Info", ViewName = "SiteInfo" };
            developerMenu.Links.Add(siteInfoLink);

            Link milestoneLink = new Link() { Title = "Milestone", ViewName = "Milestone" };
            developerMenu.Links.Add(milestoneLink);

            Link ruleLink = new Link() { Title = "Workflow Rules", ViewName = "Rule" };
            developerMenu.Links.Add(ruleLink);

            Link parameteresLink = new Link() { Title = "Parameters", ViewName = "Parameter" };
            developerMenu.Links.Add(parameteresLink);

            Link derivationLink = new Link() { Title = "Derivation", ViewName = "Derivation" };
            developerMenu.Links.Add(derivationLink);

            Link chartInfoLink = new Link() { Title = "Chart Info", ViewName = "ChartInfo" };
            developerMenu.Links.Add(chartInfoLink);

            Link tooltipLink = new Link() { Title = "Tooltip", ViewName = "Tooltip" };
            developerMenu.Links.Add(tooltipLink);

            Link cronLink = new Link() { Title = "Cron", ViewName = "Cron" };
            developerMenu.Links.Add(cronLink);
        }

        private void SetNewViewDefaults(Durados.Web.Mvc.Database configDatabase)
        {
            Durados.Web.Mvc.Database db = Map.GetDefaultDatabase();

            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)db.Views["Table"];
            Type type = view.GetType();

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


            foreach (PropertyInfo propertyInfo in properties)
            {
                object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(Durados.Config.Attributes.PropertyAttribute), true);
                Durados.Config.Attributes.PropertyAttribute propertyAttribute = null;
                if (propertyAttributes.Length == 1)
                {
                    propertyAttribute = (Durados.Config.Attributes.PropertyAttribute)propertyAttributes[0];

                }

                if (propertyAttribute != null)
                {
                    //PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];

                    View configView = (View)configDatabase.Views["View"];
                    switch (propertyAttribute.PropertyType)
                    {
                        case Durados.Config.Attributes.PropertyType.Column:
                            try
                            {
                                if (propertyInfo.Name != "DisplayName" && propertyInfo.Name != "DisplayColumn" && configView.Fields.ContainsKey(propertyInfo.Name) && propertyInfo.Name != "WorkspaceID")
                                    configView.Fields[propertyInfo.Name].DefaultValue = propertyInfo.GetValue(view, null);
                            }
                            catch { }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void SetRoles(Field field, string roles)
        {
            field.Precedent = true;
            //field.AllowSelectRoles = roles;
            if (roles.Contains("Admin"))
                field.AllowSelectRoles = "everyone";
            else
                field.AllowSelectRoles = roles;
            field.AllowCreateRoles = roles;
            field.AllowEditRoles = roles;
        }

        private void SetViewRoles(Durados.View view, string roles)
        {
            view.Precedent = true;
            view.AllowCreateRoles = roles;
            view.AllowEditRoles = roles;
            //view.AllowSelectRoles = roles;
            if (roles.Contains("Admin") || (view.Name != "Field" && view.Name != "View"))
                view.AllowSelectRoles = "everyone";
            else
                view.AllowSelectRoles = roles;
            view.AllowDeleteRoles = roles;

        }

        private void ConfigureSecurity(Durados.Web.Mvc.Database configDatabase, Durados.Web.Mvc.Database database)
        {
            View configView = (View)configDatabase.Views["View"];
            if (configView.Fields.ContainsKey("WorkspaceID"))
            {
                ColumnField workspaceID = (ColumnField)configView.Fields["WorkspaceID"];
                workspaceID.TextHtmlControlType = TextHtmlControlType.DropDown;
                workspaceID.MultiValueParentViewName = "Workspace";
                workspaceID.DisplayName = "Security Workspace";
                workspaceID.Preview = true;
                workspaceID.Sortable = true;
                workspaceID.HideInFilter = false;
                workspaceID.GroupFilterWidth = 160;
                //workspaceID.MultiValueExclude = "Admin";
                workspaceID.GridEditable = false;
                workspaceID.Required = true;
            }

            View configMenu = (View)configDatabase.Views["Menu"];
            configMenu.ColumnsInDialog = 1;

            if (configMenu.Fields.ContainsKey("WorkspaceID"))
            {
                ColumnField workspaceID = (ColumnField)configMenu.Fields["WorkspaceID"];
                workspaceID.TextHtmlControlType = TextHtmlControlType.DropDown;
                workspaceID.MultiValueParentViewName = "Workspace";
                workspaceID.DisplayName = "Security Workspace";
                //workspaceID.MultiValueExclude = "Admin";
                workspaceID.Required = true;
                workspaceID.DisableInCreate = true;
                workspaceID.DisableInEdit = true;
            }
            ColumnField menuName = (ColumnField)configMenu.Fields["Name"];
            menuName.Required = true;

            if (configDatabase.Views.ContainsKey("Link") && configDatabase.Views["Link"].Fields.ContainsKey("WorkspaceID"))
            {
                ColumnField workspaceID = (ColumnField)configDatabase.Views["Link"].Fields["WorkspaceID"];
                workspaceID.TextHtmlControlType = TextHtmlControlType.DropDown;
                workspaceID.MultiValueParentViewName = "Workspace";
                workspaceID.DisplayName = "Workspace";
                workspaceID.ExcludeInInsert = false;
                workspaceID.ExcludeInUpdate = false;
                workspaceID.HideInCreate = false;
                workspaceID.HideInEdit = false;
            }

            //Workspace
            Category rolesCategory = new Category() { Name = "Permissions", Ordinal = -20 };
            Category detailsCategory = new Category() { Name = "Details", Ordinal = 10 };

            View workspaceView = (View)configDatabase.Views["Workspace"];
            foreach (Field field in workspaceView.Fields.Values)
            {
                field.HideInFilter = true;
                field.Sortable = false;
            }

            workspaceView.ColumnsInDialog = 1;
            workspaceView.DisplayName = "Workspace Permission";
            workspaceView.Description = "Grant and restrict permission to Workspace.<br/> Select roles for Allow to grant permission for functioanlity and select deny to restrict role for functionality";
            workspaceView.GridEditable = false;
            workspaceView.AllowDelete = false;
            workspaceView.OrdinalColumnName = "Ordinal";
            workspaceView.DataDisplayType = DataDisplayType.Preview;
            workspaceView.FilterType = FilterType.Group;
            workspaceView.SortingType = SortingType.Group;
            workspaceView.DashboardWidth = "160";

            workspaceView.Derivation = new Derivation() { DerivationField = "Precedent", Deriveds = "0;AllowCreateRoles,AllowEditRoles,AllowDeleteRoles,AllowSelectRoles,ViewOwnerRoles,DenyCreateRoles,DenyEditRoles,DenyDeleteRoles,DenySelectRoles" };

            ((ColumnField)workspaceView.Fields["ID"]).HideInTable = true;
            ColumnField nameField = (ColumnField)workspaceView.Fields["Name"];
            nameField.DisplayName = "Workspace Name";
            nameField.Description = "The title of the view";
            nameField.Order = 0;
            nameField.HideInFilter = false;
            nameField.GroupFilterWidth = 160;
            nameField.Sortable = true;
            nameField.Category = detailsCategory;
            if (workspaceView.Fields.ContainsKey("Workspaces_Parent"))
            {
                ((ParentField)workspaceView.Fields["Workspaces_Parent"]).DefaultValue = 0;
                ((ParentField)workspaceView.Fields["Workspaces_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
                ((ParentField)workspaceView.Fields["Workspaces_Parent"]).HideInTable = true;
            }

            ColumnField descriptionField = (ColumnField)workspaceView.Fields["Description"];

            descriptionField.TextHtmlControlType = TextHtmlControlType.TextArea;
            descriptionField.Rich = true;
            descriptionField.Upload = new Upload() { FileAllowedTypes = "jpg,png,gif", FileMaxSize = 100, Override = false, Title = "UploadContent", UploadFileType = Mvc.UploadFileType.Image, UploadStorageType = Mvc.UploadStorageType.File, UploadVirtualPath = "/Uploads/" + Map.Id ?? string.Empty + "/" };
            descriptionField.Category = detailsCategory;
            descriptionField.Order = 300;
            descriptionField.CssClass = "wtextarealarge";
            descriptionField.HideInTable = true;


            ((ColumnField)workspaceView.Fields["Ordinal"]).HideInCreate = true;
            ((ColumnField)workspaceView.Fields["Ordinal"]).HideInEdit = true;
            ((ColumnField)workspaceView.Fields["Ordinal"]).HideInTable = true;

            if (workspaceView.Fields.ContainsKey("HomePage"))
            {
                ((ColumnField)workspaceView.Fields["HomePage"]).HideInCreate = true;
                ((ColumnField)workspaceView.Fields["HomePage"]).HideInEdit = true;
            }

            ColumnField precedentField = (ColumnField)workspaceView.Fields["Precedent"];
            precedentField.DisplayName = "Override permissions";
            precedentField.Description = "By checking this option you override the System default permissions";
            precedentField.Order = 10;
            precedentField.GridEditable = false;
            precedentField.HideInFilter = true;
            precedentField.GroupFilterWidth = 40;
            precedentField.Category = rolesCategory;

            ColumnField allowCreateRolesField = (ColumnField)workspaceView.Fields["AllowCreateRoles"];
            allowCreateRolesField.Seperator = true;
            allowCreateRolesField.SeperatorTitle = "Select the Roles for Allow (Grant) Permissions:";
            allowCreateRolesField.TextHtmlControlType = TextHtmlControlType.CheckList;
            allowCreateRolesField.MultiValueAdditionals = "everyone,everyone";
            allowCreateRolesField.MinWidth = 350;
            allowCreateRolesField.Order = 20;
            allowCreateRolesField.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            allowCreateRolesField.DisplayName = "Allow Create";
            allowCreateRolesField.Description = "Can create, duplicate records";
            allowCreateRolesField.Category = rolesCategory;

            ColumnField allowEditRolesField = (ColumnField)workspaceView.Fields["AllowEditRoles"];
            allowEditRolesField.TextHtmlControlType = TextHtmlControlType.CheckList;
            allowEditRolesField.MultiValueAdditionals = "everyone,everyone";
            allowEditRolesField.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            allowEditRolesField.MinWidth = 350;
            allowEditRolesField.Order = 30;
            allowEditRolesField.DisplayName = "Allow Edit";
            allowEditRolesField.Description = "Can edit records";
            allowEditRolesField.Category = rolesCategory;

            ColumnField allowDeleteRolesField = (ColumnField)workspaceView.Fields["AllowDeleteRoles"];
            allowDeleteRolesField.TextHtmlControlType = TextHtmlControlType.CheckList;
            allowDeleteRolesField.MultiValueAdditionals = "everyone,everyone";
            allowDeleteRolesField.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            allowDeleteRolesField.MinWidth = 350;
            allowDeleteRolesField.Order = 40;
            allowDeleteRolesField.DisplayName = "Allow Delete";
            allowDeleteRolesField.Description = "Can delete records";
            allowDeleteRolesField.Category = rolesCategory;

            ColumnField allowSelectRolesField = (ColumnField)workspaceView.Fields["AllowSelectRoles"];
            allowSelectRolesField.TextHtmlControlType = TextHtmlControlType.CheckList;
            allowSelectRolesField.MultiValueAdditionals = "everyone,everyone";
            allowSelectRolesField.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            allowSelectRolesField.MinWidth = 350;
            allowSelectRolesField.Order = 50;
            allowSelectRolesField.DisplayName = "Allow Read";
            allowSelectRolesField.Description = "Can view information and history";
            allowSelectRolesField.Category = rolesCategory;

            if (workspaceView.Fields.ContainsKey("ViewOwnerRoles"))
            {
                ColumnField viewOwnerRolesField = (ColumnField)workspaceView.Fields["ViewOwnerRoles"];
                viewOwnerRolesField.TextHtmlControlType = TextHtmlControlType.CheckList;
                // viewOwnerRolesField.MultiValueAdditionals = "everyone,everyone";
                viewOwnerRolesField.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
                viewOwnerRolesField.MinWidth = 350;
                viewOwnerRolesField.Order = 60;
                viewOwnerRolesField.DisplayName = "Views Owner";
                viewOwnerRolesField.Description = "Can configure views look and behavior";
                viewOwnerRolesField.Category = rolesCategory;
                viewOwnerRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            }
            allowSelectRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            allowEditRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            allowCreateRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            allowDeleteRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";




            ColumnField denyCreateRolesField = (ColumnField)workspaceView.Fields["DenyCreateRoles"];
            denyCreateRolesField.Seperator = true;
            denyCreateRolesField.SeperatorTitle = "Select Roles for Deny (Restrict) Permissions:";
            denyCreateRolesField.TextHtmlControlType = TextHtmlControlType.CheckList;
            denyCreateRolesField.MultiValueAdditionals = "everyone,everyone";
            denyCreateRolesField.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            denyCreateRolesField.MinWidth = 350;
            denyCreateRolesField.Order = 100;
            denyCreateRolesField.DisplayName = "Deny Create";
            denyCreateRolesField.Description = "Can&#39t create or duplicate records";
            denyCreateRolesField.Category = rolesCategory;

            ColumnField denyEditRolesField = (ColumnField)workspaceView.Fields["DenyEditRoles"];
            denyEditRolesField.TextHtmlControlType = TextHtmlControlType.CheckList;
            denyEditRolesField.MultiValueAdditionals = "everyone,everyone";
            denyEditRolesField.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            denyEditRolesField.MinWidth = 350;
            denyEditRolesField.Order = 110;
            denyEditRolesField.DisplayName = "Deny Edit";
            denyEditRolesField.Description = "Can&#39t edit records";
            denyEditRolesField.Category = rolesCategory;

            ColumnField denyDeleteRolesField = (ColumnField)workspaceView.Fields["DenyDeleteRoles"];
            denyDeleteRolesField.TextHtmlControlType = TextHtmlControlType.CheckList;
            denyDeleteRolesField.MultiValueAdditionals = "everyone,everyone";
            denyDeleteRolesField.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            denyDeleteRolesField.MinWidth = 350;
            denyDeleteRolesField.Order = 120;
            denyDeleteRolesField.DisplayName = "Deny Delete";
            denyDeleteRolesField.Description = "Can&#39t delete records";
            denyDeleteRolesField.Category = rolesCategory;

            ColumnField denySelectRolesField = (ColumnField)workspaceView.Fields["DenySelectRoles"];
            denySelectRolesField.TextHtmlControlType = TextHtmlControlType.CheckList;
            denySelectRolesField.MultiValueAdditionals = "everyone,everyone";
            denySelectRolesField.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            denySelectRolesField.MinWidth = 350;
            denySelectRolesField.Order = 130;
            denySelectRolesField.DisplayName = "Deny Read";
            denySelectRolesField.Description = "Can&#39t view information or select view";
            denySelectRolesField.Category = rolesCategory;



            denyCreateRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            denyEditRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            denyDeleteRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            denySelectRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";


            //Views


            ((View)configView).Derivation = new Derivation() { DerivationField = "Precedent", Deriveds = "0;AllowCreateRoles,AllowEditRoles,AllowDeleteRoles,AllowSelectRoles,ViewOwnerRoles,DenyCreateRoles,DenyEditRoles,DenyDeleteRoles,DenySelectRoles" };

            ((ColumnField)configView.Fields["Precedent"]).DisplayName = "Override inheritable";
            ((ColumnField)configView.Fields["Precedent"]).Description = "By checking this option you override the Workspace permissions";
            ((ColumnField)configView.Fields["Precedent"]).OrderForEdit = 10;

            ((ColumnField)configView.Fields["AllowCreateRoles"]).Seperator = true;
            ((ColumnField)configView.Fields["AllowCreateRoles"]).SeperatorTitle = "Select the Roles for Allow (Grant) Permissions:";
            ((ColumnField)configView.Fields["AllowCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)configView.Fields["AllowCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)configView.Fields["AllowCreateRoles"]).MinWidth = 350;
            ((ColumnField)configView.Fields["AllowCreateRoles"]).OrderForEdit = 20;
            ((ColumnField)configView.Fields["AllowCreateRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            //((ColumnField)configView.Fields["AllowCreateRoles"]).DisplayName = "Allow Create";
            ((ColumnField)configView.Fields["AllowCreateRoles"]).Description = "Can create, duplicate records";
            ((ColumnField)configView.Fields["AllowEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)configView.Fields["AllowEditRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)configView.Fields["AllowEditRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)configView.Fields["AllowEditRoles"]).MinWidth = 350;
            ((ColumnField)configView.Fields["AllowEditRoles"]).OrderForEdit = 30;
            //((ColumnField)configView.Fields["AllowEditRoles"]).DisplayName = "Allow Edit";
            ((ColumnField)configView.Fields["AllowEditRoles"]).Description = "Can edit records";
            ((ColumnField)configView.Fields["AllowDeleteRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)configView.Fields["AllowDeleteRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)configView.Fields["AllowDeleteRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)configView.Fields["AllowDeleteRoles"]).MinWidth = 350;
            ((ColumnField)configView.Fields["AllowDeleteRoles"]).OrderForEdit = 40;
            //((ColumnField)configView.Fields["AllowDeleteRoles"]).DisplayName = "Allow Delete";
            ((ColumnField)configView.Fields["AllowDeleteRoles"]).Description = "Can delete records";
            ((ColumnField)configView.Fields["AllowSelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)configView.Fields["AllowSelectRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)configView.Fields["AllowSelectRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)configView.Fields["AllowSelectRoles"]).MinWidth = 350;
            ((ColumnField)configView.Fields["AllowSelectRoles"]).OrderForEdit = 50;
            //((ColumnField)configView.Fields["AllowSelectRoles"]).DisplayName = "Allow Read";
            ((ColumnField)configView.Fields["AllowSelectRoles"]).Description = "Can view information and history";

            if (configView.Fields.ContainsKey("ViewOwnerRoles"))
            {
                ((ColumnField)configView.Fields["ViewOwnerRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
                // ((ColumnField)configView.Fields["ViewOwnerRoles"]).MultiValueAdditionals = "everyone,everyone";
                ((ColumnField)configView.Fields["ViewOwnerRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
                ((ColumnField)configView.Fields["ViewOwnerRoles"]).MinWidth = 350;
                ((ColumnField)configView.Fields["ViewOwnerRoles"]).OrderForEdit = 55;
                ((ColumnField)configView.Fields["ViewOwnerRoles"]).DisplayName = "View Owner";
                ((ColumnField)configView.Fields["ViewOwnerRoles"]).Description = "Can Configure view look and behavior.";
            }
            allowSelectRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            allowEditRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            allowCreateRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            allowDeleteRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";

            denyCreateRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            denyEditRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            denyDeleteRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";
            denySelectRolesField.ContainerGraphicProperties = "d_fieldContainerWrap";

            ((ColumnField)configView.Fields["DenyCreateRoles"]).Seperator = true;
            ((ColumnField)configView.Fields["DenyCreateRoles"]).SeperatorTitle = "Select Roles for Deny (Restrict) Permissions:";
            ((ColumnField)configView.Fields["DenyCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)configView.Fields["DenyCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)configView.Fields["DenyCreateRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)configView.Fields["DenyCreateRoles"]).MinWidth = 350;
            ((ColumnField)configView.Fields["DenyCreateRoles"]).OrderForEdit = 100;
            ((ColumnField)configView.Fields["DenyCreateRoles"]).DisplayName = "Deny Create";
            ((ColumnField)configView.Fields["DenyCreateRoles"]).Description = "Can&#39t create or duplicate records";
            ((ColumnField)configView.Fields["DenyEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)configView.Fields["DenyEditRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)configView.Fields["DenyEditRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)configView.Fields["DenyEditRoles"]).MinWidth = 350;
            ((ColumnField)configView.Fields["DenyEditRoles"]).OrderForEdit = 110;
            ((ColumnField)configView.Fields["DenyEditRoles"]).DisplayName = "Deny Edit";
            ((ColumnField)configView.Fields["DenyEditRoles"]).Description = "Can&#39t edit records";
            ((ColumnField)configView.Fields["DenyDeleteRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)configView.Fields["DenyDeleteRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)configView.Fields["DenyDeleteRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)configView.Fields["DenyDeleteRoles"]).MinWidth = 350;
            ((ColumnField)configView.Fields["DenyDeleteRoles"]).OrderForEdit = 120;
            ((ColumnField)configView.Fields["DenyDeleteRoles"]).DisplayName = "Deny Delete";
            ((ColumnField)configView.Fields["DenyDeleteRoles"]).Description = "Can&#39t delete records";
            ((ColumnField)configView.Fields["DenySelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)configView.Fields["DenySelectRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)configView.Fields["DenySelectRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)configView.Fields["DenySelectRoles"]).MinWidth = 350;
            ((ColumnField)configView.Fields["DenySelectRoles"]).OrderForEdit = 130;
            ((ColumnField)configView.Fields["DenySelectRoles"]).DisplayName = "Deny Read";
            ((ColumnField)configView.Fields["DenySelectRoles"]).Description = "Can&#39t view information or select view";


            //Fields
            View fieldView = (View)configDatabase.Views["Field"];
            ((View)fieldView).Derivation = new Derivation() { DerivationField = "Precedent", Deriveds = "0;AllowCreateRoles,AllowEditRoles,AllowSelectRoles,DenyCreateRoles,DenyEditRoles,DenySelectRoles" };

            ((ColumnField)fieldView.Fields["Precedent"]).DisplayName = "Override inheritable";
            ((ColumnField)fieldView.Fields["Precedent"]).Description = "By checking this option you override the View permissions";
            ((ColumnField)fieldView.Fields["Precedent"]).OrderForEdit = 10;

            ((ColumnField)fieldView.Fields["AllowCreateRoles"]).Seperator = true;
            ((ColumnField)fieldView.Fields["AllowCreateRoles"]).SeperatorTitle = "Select the Roles for Allow (Grant) Permissions:";
            ((ColumnField)fieldView.Fields["AllowCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)fieldView.Fields["AllowCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)fieldView.Fields["AllowCreateRoles"]).MinWidth = 350;
            ((ColumnField)fieldView.Fields["AllowCreateRoles"]).OrderForEdit = 20;
            ((ColumnField)fieldView.Fields["AllowCreateRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)fieldView.Fields["AllowCreateRoles"]).DisplayName = "Allow Create";
            ((ColumnField)fieldView.Fields["AllowCreateRoles"]).Description = "Field displays in New or Duplicare dialog";
            ((ColumnField)fieldView.Fields["AllowEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)fieldView.Fields["AllowEditRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)fieldView.Fields["AllowEditRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)fieldView.Fields["AllowEditRoles"]).MinWidth = 350;
            ((ColumnField)fieldView.Fields["AllowEditRoles"]).OrderForEdit = 30;
            ((ColumnField)fieldView.Fields["AllowEditRoles"]).DisplayName = "Allow Edit";
            ((ColumnField)fieldView.Fields["AllowEditRoles"]).Description = "Field displays in Edit dialog";
            ((ColumnField)fieldView.Fields["AllowSelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)fieldView.Fields["AllowSelectRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)fieldView.Fields["AllowSelectRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)fieldView.Fields["AllowSelectRoles"]).MinWidth = 350;
            ((ColumnField)fieldView.Fields["AllowSelectRoles"]).OrderForEdit = 50;
            ((ColumnField)fieldView.Fields["AllowSelectRoles"]).DisplayName = "Allow Read";
            ((ColumnField)fieldView.Fields["AllowSelectRoles"]).Description = "Field displays in View (Grid) window";

            ((ColumnField)fieldView.Fields["DenyCreateRoles"]).Seperator = true;
            ((ColumnField)fieldView.Fields["DenyCreateRoles"]).SeperatorTitle = "Select Roles for Deny (Restrict) Permissions:";
            ((ColumnField)fieldView.Fields["DenyCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)fieldView.Fields["DenyCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)fieldView.Fields["DenyCreateRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)fieldView.Fields["DenyCreateRoles"]).MinWidth = 350;
            ((ColumnField)fieldView.Fields["DenyCreateRoles"]).OrderForEdit = 100;
            ((ColumnField)fieldView.Fields["DenyCreateRoles"]).DisplayName = "Deny Create";
            ((ColumnField)fieldView.Fields["DenyCreateRoles"]).Description = "Field doesn&#39t display in New or Duplicare dialog";
            ((ColumnField)fieldView.Fields["DenyEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)fieldView.Fields["DenyEditRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)fieldView.Fields["DenyEditRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)fieldView.Fields["DenyEditRoles"]).MinWidth = 350;
            ((ColumnField)fieldView.Fields["DenyEditRoles"]).OrderForEdit = 110;
            ((ColumnField)fieldView.Fields["DenyEditRoles"]).DisplayName = "Deny Edit";
            ((ColumnField)fieldView.Fields["DenyEditRoles"]).Description = "Field doesn&#39t display in Edit dialog";
            ((ColumnField)fieldView.Fields["DenySelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)fieldView.Fields["DenySelectRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)fieldView.Fields["DenySelectRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)fieldView.Fields["DenySelectRoles"]).MinWidth = 350;
            ((ColumnField)fieldView.Fields["DenySelectRoles"]).OrderForEdit = 130;
            ((ColumnField)fieldView.Fields["DenySelectRoles"]).DisplayName = "Deny Read";
            ((ColumnField)fieldView.Fields["DenySelectRoles"]).Description = "Field doesn&#39t display in View (Grid) window";

            SetViewOwnerConfig();
            //Database
            View databaseConfigView = (View)configDatabase.Views["Database"];
            ((ColumnField)databaseConfigView.Fields["DefaultAllowCreateRoles"]).Seperator = true;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowCreateRoles"]).SeperatorTitle = "Select the Roles for Allow (Grant) Permissions:";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowCreateRoles"]).MinWidth = 350;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowCreateRoles"]).Order = 20;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowCreateRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowCreateRoles"]).DisplayName = "Allow Create";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowCreateRoles"]).Description = "Can create, duplicate records";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowEditRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowEditRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowEditRoles"]).MinWidth = 350;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowEditRoles"]).Order = 30;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowEditRoles"]).DisplayName = "Allow Edit";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowEditRoles"]).Description = "Can edit records";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowDeleteRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowDeleteRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowDeleteRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowDeleteRoles"]).MinWidth = 350;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowDeleteRoles"]).Order = 40;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowDeleteRoles"]).DisplayName = "Allow Delete";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowDeleteRoles"]).Description = "Can delete records";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowSelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowSelectRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowSelectRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowSelectRoles"]).MinWidth = 350;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowSelectRoles"]).Order = 50;
            ((ColumnField)databaseConfigView.Fields["DefaultAllowSelectRoles"]).DisplayName = "Allow Read";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowSelectRoles"]).Description = "Can view information and history";

            if (databaseConfigView.Fields.ContainsKey("DefaultViewOwnerRoles"))
            {
                ((ColumnField)databaseConfigView.Fields["DefaultViewOwnerRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
                //((ColumnField)databaseConfigView.Fields["DefaultViewOwnerRoles"]).MultiValueAdditionals = "everyone,everyone";
                ((ColumnField)databaseConfigView.Fields["DefaultViewOwnerRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
                ((ColumnField)databaseConfigView.Fields["DefaultViewOwnerRoles"]).MinWidth = 350;
                ((ColumnField)databaseConfigView.Fields["DefaultViewOwnerRoles"]).Order = 70;
                ((ColumnField)databaseConfigView.Fields["DefaultViewOwnerRoles"]).DisplayName = "Views Owner";
                ((ColumnField)databaseConfigView.Fields["DefaultViewOwnerRoles"]).Description = "Can configure view look and behavior.";
            }
            ((ColumnField)databaseConfigView.Fields["DefaultAllowSelectRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowEditRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowCreateRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
            ((ColumnField)databaseConfigView.Fields["DefaultAllowDeleteRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
            if (databaseConfigView.Fields.ContainsKey("DefaultViewOwnerRoles"))
                ((ColumnField)databaseConfigView.Fields["DefaultViewOwnerRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyCreateRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyEditRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyDeleteRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
            ((ColumnField)databaseConfigView.Fields["DefaultDenySelectRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";



            ((ColumnField)databaseConfigView.Fields["DefaultDenyCreateRoles"]).Seperator = true;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyCreateRoles"]).SeperatorTitle = "Select Roles for Deny (Restrict) Permissions:";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyCreateRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyCreateRoles"]).MinWidth = 350;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyCreateRoles"]).Order = 100;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyCreateRoles"]).DisplayName = "Deny Create";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyCreateRoles"]).Description = "Can&#39t create or duplicate records";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyEditRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyEditRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyEditRoles"]).MinWidth = 350;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyEditRoles"]).Order = 110;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyEditRoles"]).DisplayName = "Deny Edit";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyEditRoles"]).Description = "Can&#39t edit records";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyDeleteRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyDeleteRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyDeleteRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyDeleteRoles"]).MinWidth = 350;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyDeleteRoles"]).Order = 120;
            ((ColumnField)databaseConfigView.Fields["DefaultDenyDeleteRoles"]).DisplayName = "Deny Delete";
            ((ColumnField)databaseConfigView.Fields["DefaultDenyDeleteRoles"]).Description = "Can&#39t delete records";
            ((ColumnField)databaseConfigView.Fields["DefaultDenySelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
            ((ColumnField)databaseConfigView.Fields["DefaultDenySelectRoles"]).MultiValueAdditionals = "everyone,everyone";
            ((ColumnField)databaseConfigView.Fields["DefaultDenySelectRoles"]).MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
            ((ColumnField)databaseConfigView.Fields["DefaultDenySelectRoles"]).MinWidth = 350;
            ((ColumnField)databaseConfigView.Fields["DefaultDenySelectRoles"]).Order = 130;
            ((ColumnField)databaseConfigView.Fields["DefaultDenySelectRoles"]).DisplayName = "Deny Read";
            ((ColumnField)databaseConfigView.Fields["DefaultDenySelectRoles"]).Description = "Can&#39t view information or select view";
            ((ColumnField)databaseConfigView.Fields["DefaultWorkspaceId"]).DisplayName = "Default Workspace";
            ((ColumnField)databaseConfigView.Fields["DefaultWorkspaceId"]).TextHtmlControlType = TextHtmlControlType.DropDown;
            ((ColumnField)databaseConfigView.Fields["DefaultWorkspaceId"]).MultiValueParentViewName = "Workspace";
            ((ColumnField)databaseConfigView.Fields["DefaultWorkspaceId"]).MultiValueExclude = "Admin";

            //Page
            View pageView = (View)configDatabase.Views["Page"];
            ((View)pageView).Derivation = new Derivation() { DerivationField = "Precedent", Deriveds = "0;AllowSelectRoles" };
            if (pageView.Fields.ContainsKey("Precedent"))
            {
                ((ColumnField)pageView.Fields["Precedent"]).DisplayName = "Override inheritable";
                ((ColumnField)pageView.Fields["Precedent"]).Description = "By checking this option you override the Page permissions";
                ((ColumnField)pageView.Fields["Precedent"]).Order = -120;
            }
            if (pageView.Fields.ContainsKey("AllowSelectRoles"))
            {
                ColumnField allowSelectRolesPage = ((ColumnField)pageView.Fields["AllowSelectRoles"]);
                allowSelectRolesPage.TextHtmlControlType = TextHtmlControlType.CheckList;
                allowSelectRolesPage.MultiValueAdditionals = "everyone,everyone";
                allowSelectRolesPage.MultiValueParentViewName = string.IsNullOrEmpty(database.RoleViewName) ? "durados_UserRole" : database.RoleViewName;
                allowSelectRolesPage.MinWidth = 200;
                allowSelectRolesPage.Order = -110;
                allowSelectRolesPage.DisplayName = "Allow Read";
                allowSelectRolesPage.Description = "Can view information and history";
                //allowSelectRolesPage.DefaultValue = "Developer,Admin,User";
                allowSelectRolesPage.HideInTable = true;
            }

        }

        private void SetViewOwnerConfig()
        {
            Database db = Map.GetConfigDatabase();
            if (db.Views["View"].Fields.ContainsKey("ViewOwnerRoles"))
            {
                string roles = "Developer,Admin," + ViewOwenrRole;
                View configView = (View)db.Views["View"];
                View fieldView = (View)db.Views["Field"];
                configView.ViewOwnerRoles = roles;
                fieldView.ViewOwnerRoles = roles;

                if (configView.Fields.ContainsKey("GridDisplayType"))
                    SetRoles(((ColumnField)configView.Fields["GridDisplayType"]), roles);

                //Views
                //SetRoles(((ColumnField)configView.Fields["Layout"]), roles);
                if (configView.Fields.ContainsKey("Skin"))
                    SetRoles(((ColumnField)configView.Fields["Skin"]), roles);
                if (configView.Fields.ContainsKey("Theme"))
                    SetRoles(((ColumnField)configView.Fields["Theme"]), roles);
                if (configView.Fields.ContainsKey("RowHeight"))
                    SetRoles(((ColumnField)configView.Fields["RowHeight"]), roles);
                //SetRoles(((ColumnField)configView.Fields["DisplayName"]), roles);
                SetRoles(((ColumnField)configView.Fields["PageSize"]), roles);
                //AddRolesToEdit(((ColumnField)configView.Fields["DisplayColumn"]), roles);
                SetRoles(((ChildrenField)configView.Fields["Fields_Children"]), roles);

                SetRoles(((ColumnField)configView.Fields["HideFilter"]), roles);
                SetRoles(((ColumnField)configView.Fields["HidePager"]), roles);
                SetRoles(((ColumnField)configView.Fields["HideSearch"]), roles);
                SetRoles(((ColumnField)configView.Fields["HideToolbar"]), roles);
                SetRoles(((ColumnField)configView.Fields["CollapseFilter"]), roles);
                SetRoles(((ColumnField)configView.Fields["Send"]), roles);
                //SetRoles(((ColumnField)configView.Fields["ExportToCsv"]), roles);
                //SetRoles(((ColumnField)configView.Fields["ImportFromExcel"]), roles);
                //SetRoles(((ColumnField)configView.Fields["SaveHistory"]), roles);
                //SetRoles(((ColumnField)configView.Fields["GridEditable"]), roles);

                //SetRoles(((ColumnField)configView.Fields["ColumnsInDialog"]), roles);
                //SetRoles(((ColumnField)configView.Fields["AllowCreate"]), roles);
                //SetRoles(((ColumnField)configView.Fields["AllowEdit"]), roles);
                //SetRoles(((ColumnField)configView.Fields["AllowDuplicate"]), roles);
                //SetRoles(((ColumnField)configView.Fields["AllowDelete"]), roles);
                //SetRoles(((ColumnField)configView.Fields["MultiSelect"]), roles);

                //SetRoles(((ColumnField)configView.Fields["Description"]), roles);
                //SetRoles(((ColumnField)configView.Fields["NewButtonName"]), roles);
                //SetRoles(((ColumnField)configView.Fields["EditButtonName"]), roles);
                //SetRoles(((ColumnField)configView.Fields["DuplicateButtonName"]), roles);
                //SetRoles(((ColumnField)configView.Fields["DeleteButtonName"]), roles);
                if (configView.Fields.ContainsKey("Background"))
                {
                    SetRoles(((ColumnField)configView.Fields["Background"]), roles);
                }
                if (configView.Fields.ContainsKey("RowBackground"))
                {
                    SetRoles(((ColumnField)configView.Fields["RowBackground"]), roles);
                }
                if (configView.Fields.ContainsKey("HoverBackground"))
                {
                    SetRoles(((ColumnField)configView.Fields["HoverBackground"]), roles);
                }
                if (configView.Fields.ContainsKey("AlternateRowBackground"))
                {
                    SetRoles(((ColumnField)configView.Fields["AlternateRowBackground"]), roles);
                }
                if (configView.Fields.ContainsKey("ToolBoxBackground"))
                {
                    SetRoles(((ColumnField)configView.Fields["ToolBoxBackground"]), roles);
                }
                if (configView.Fields.ContainsKey("FontColor"))
                {
                    SetRoles(((ColumnField)configView.Fields["FontColor"]), roles);
                }
                if (configView.Fields.ContainsKey("TextFontColor"))
                {
                    SetRoles(((ColumnField)configView.Fields["TextFontColor"]), roles);
                }
                if (configView.Fields.ContainsKey("ToolBoxTextColor"))
                {
                    SetRoles(((ColumnField)configView.Fields["ToolBoxTextColor"]), roles);
                }
                if (configView.Fields.ContainsKey("AlterTextColor"))
                {
                    SetRoles(((ColumnField)configView.Fields["AlterTextColor"]), roles);
                }
                if (configView.Fields.ContainsKey("HoverTextColor"))
                {
                    SetRoles(((ColumnField)configView.Fields["HoverTextColor"]), roles);
                }
                if (configView.Fields.ContainsKey("BorderColor"))
                {
                    SetRoles(((ColumnField)configView.Fields["BorderColor"]), roles);
                }
                //Fields
                SetRoles(((ColumnField)fieldView.Fields["DisplayName"]), roles);
                if (fieldView.Fields.ContainsKey("JsonName"))
                    SetRoles(((ColumnField)fieldView.Fields["JsonName"]), roles);
                SetRoles(((ColumnField)fieldView.Fields["DisplayFormat"]), roles);
                if (fieldView.Fields.ContainsKey("ShowColumnHeader"))
                    SetRoles(((ColumnField)fieldView.Fields["ShowColumnHeader"]), roles);
                SetRoles(((ColumnField)fieldView.Fields["DataType"]), roles);
                SetRoles(((ColumnField)fieldView.Fields["Excluded"]), roles);
                if (fieldView.Fields.ContainsKey("TextAlignment"))
                    SetRoles(((ColumnField)fieldView.Fields["TextAlignment"]), roles);
                //SetRoles(((ColumnField)fieldView.Fields["DefaultValue"]), roles);
                SetRoles(((ColumnField)fieldView.Fields["Description"]), roles);

                //SetRoles(((ColumnField)fieldView.Fields["Min"]), roles);
                //SetRoles(((ColumnField)fieldView.Fields["Max"]), roles);

                SetRoles(((ColumnField)fieldView.Fields["HideInTable"]), roles);
                SetRoles(((ColumnField)fieldView.Fields["Sortable"]), roles);
                //SetRoles(((ColumnField)fieldView.Fields["Preview"]), roles);
                SetRoles(((ColumnField)fieldView.Fields["HideInFilter"]), roles);
                //SetRoles(((ColumnField)fieldView.Fields["GroupFilterWidth"]), roles);

                //SetRoles(((ParentField)fieldView.Fields["Category_Parent"]), roles);
                //SetRoles(((ColumnField)fieldView.Fields["HideInFilter"]), roles);
                //SetRoles(((ColumnField)fieldView.Fields["HideInEdit"]), roles);
                //SetRoles(((ColumnField)fieldView.Fields["HideInCreate"]), roles);

                //SetRoles(((ColumnField)fieldView.Fields["ColSpanInDialog"]), roles);
                //SetRoles(((ColumnField)fieldView.Fields["Width"]), roles);
                //SetRoles(((ColumnField)fieldView.Fields["Seperator"]), roles);


            }

        }

        protected virtual void ConfigPage(Durados.Web.Mvc.Database configDatabase)
        {
            if (configDatabase.Views.ContainsKey("Page"))
            {
                View pageView = (View)configDatabase.Views["Page"];
                pageView.DisplayName = "Content Page";

                SetViewRoles(pageView, "Developer,Admin");

                pageView.ColumnsInDialog = 1;
                pageView.DataDisplayType = DataDisplayType.Preview;
                pageView.FilterType = FilterType.Group;
                pageView.SortingType = SortingType.Group;
                foreach (Field field in pageView.Fields.Values)
                {
                    field.HideInFilter = true;
                    field.Sortable = false;
                }
                // pageView.Derivation = new Derivation() { DerivationField = "PageType", Deriveds = "Content;ExternalPage,Width,Height,Scroll,ExternalNewPage,NewTab,Target,ReportName,ReportDisplayName|IFrame;Content,ExternalNewPage,NewTab,Target,ReportName,ReportDisplayName|External;Content,ExternalPage,Width,Height,Scroll,ReportName,ReportDisplayName|ReportingServices;Content,ExternalPage,Width,Height,Scroll,ExternalNewPage,NewTab,Target" };

                if (pageView.Fields.ContainsKey("Order"))
                {
                    pageView.OrdinalColumnName = "Order";

                    Field orderField = pageView.Fields["Order"];

                    pageView.DefaultSort = "Order";
                    orderField.HideInCreate = false;
                    orderField.HideInEdit = false;
                    orderField.HideInTable = false;
                    //orderField.ColSpanInDialog = 2;
                    orderField.HideInEdit = true;

                }

                if (pageView.Fields.ContainsKey("Pages_Parent"))
                {
                    ParentField pagesParentField = (ParentField)pageView.Fields["Pages_Parent"];

                    pagesParentField.ParentHtmlControlType = ParentHtmlControlType.Hidden;
                    pagesParentField.DefaultValue = 0;
                    pagesParentField.HideInTable = true;
                }

                if (pageView.Fields.ContainsKey("ID"))
                {
                    ColumnField idField = (ColumnField)pageView.Fields["ID"];
                    idField.DisplayName = "Id";
                    idField.HideInTable = false;
                }

                if (pageView.Fields.ContainsKey("Title"))
                {
                    ColumnField titleField = (ColumnField)pageView.Fields["Title"];

                    titleField.Order = -500;
                    titleField.ColSpanInDialog = 2;
                    titleField.Required = false;
                    titleField.HideInEdit = true;
                    titleField.HideInFilter = false;
                    titleField.Sortable = true;
                    titleField.GroupFilterWidth = 160;
                }

                //if (pageView.Fields.ContainsKey("PageWorkspace_Parent"))
                //{
                //    ParentField pageWorkspace_ParentField = (ParentField)pageView.Fields["PageWorkspace_Parent"];

                //    pageWorkspace_ParentField.Order = 20;
                //    pageWorkspace_ParentField.GridEditable = false;
                //    pageWorkspace_ParentField.ColSpanInDialog = 1;
                //    pageWorkspace_ParentField.Required = true;
                //}

                if (pageView.Fields.ContainsKey("WorkspaceID"))
                {
                    ColumnField workspaceID = (ColumnField)pageView.Fields["WorkspaceID"];
                    workspaceID.TextHtmlControlType = TextHtmlControlType.DropDown;
                    workspaceID.MultiValueParentViewName = "Workspace";
                    workspaceID.DisplayName = "Security Workspace";
                    workspaceID.MultiValueExclude = "Admin";
                    workspaceID.GridEditable = false;
                    workspaceID.Required = true;
                    workspaceID.ColSpanInDialog = 1;
                    workspaceID.Order = -400;
                    workspaceID.Seperator = true;
                    workspaceID.SeperatorTitle = "Security";
                    workspaceID.HideInFilter = false;
                    workspaceID.Sortable = true;
                    workspaceID.GroupFilterWidth = 160;
                    workspaceID.Preview = true;
                }

                if (pageView.Fields.ContainsKey("PageMenu_Parent"))
                {
                    ParentField pageMenu_ParentField = (ParentField)pageView.Fields["PageMenu_Parent"];

                    pageMenu_ParentField.HideInCreate = true;
                    pageMenu_ParentField.HideInEdit = true;
                    pageMenu_ParentField.HideInTable = true;
                    //pageMenu_ParentField.Order = 30;
                    //pageMenu_ParentField.GridEditable = false;
                    //pageMenu_ParentField.ColSpanInDialog = 1;
                    //pageMenu_ParentField.Required = false;
                    //pageMenu_ParentField.InlineAdding = true;

                }

                if (pageView.Fields.ContainsKey("PageType"))
                {
                    ColumnField pageTypeField = (ColumnField)pageView.Fields["PageType"];

                    pageTypeField.Order = 35;
                    pageTypeField.GridEditable = false;
                    pageTypeField.ColSpanInDialog = 1;
                    pageTypeField.Required = true;
                    pageTypeField.EnumType = typeof(Durados.PageType).AssemblyQualifiedName;
                    pageTypeField.DefaultValue = "Content";
                    pageTypeField.SeperatorTitle = "Page Content";
                    pageTypeField.Seperator = true;
                    pageTypeField.HideInFilter = false;
                    pageTypeField.Sortable = true;
                    pageTypeField.GroupFilterWidth = 160;
                    pageTypeField.Preview = true;

                }

                Category contentCategory = new Category() { Name = "Content", Ordinal = 10 };

                if (pageView.Fields.ContainsKey("Content"))
                {
                    ColumnField contentField = (ColumnField)pageView.Fields["Content"];

                    contentField.Order = 40;
                    contentField.ColSpanInDialog = 2;
                    contentField.TextHtmlControlType = TextHtmlControlType.TextArea;
                    contentField.Rich = true;
                    contentField.Category = contentCategory;
                    contentField.CssClass = "hwtextareawide";
                    contentField.HideInTable = true;
                    contentField.Upload = new Upload() { FileAllowedTypes = "jpg,png,gif", FileMaxSize = 100, Override = false, Title = "UploadContent", UploadFileType = Mvc.UploadFileType.Image, UploadStorageType = Mvc.UploadStorageType.File, UploadVirtualPath = "/Uploads/" + Map.Id ?? string.Empty + "/" };
                }

                Category externalCategory = new Category() { Name = "External_IFrame", Ordinal = 20 };

                if (pageView.Fields.ContainsKey("ExternalPage"))
                {
                    ColumnField externalPageField = (ColumnField)pageView.Fields["ExternalPage"];

                    externalPageField.Order = 50;
                    externalPageField.DisplayName = "IFrame URL";
                    externalPageField.Category = externalCategory;
                    externalPageField.TextHtmlControlType = TextHtmlControlType.Url;
                    externalPageField.HideInTable = true;
                }

                if (pageView.Fields.ContainsKey("Width"))
                {
                    ColumnField widthField = (ColumnField)pageView.Fields["Width"];

                    widthField.Order = 70;
                    widthField.Category = externalCategory;
                    widthField.Max = 1200;
                    widthField.Min = 0;
                    widthField.HideInTable = true;
                    widthField.Width = 50;
                }

                if (pageView.Fields.ContainsKey("Height"))
                {
                    ColumnField heightField = (ColumnField)pageView.Fields["Height"];

                    heightField.Order = 80;
                    heightField.Category = externalCategory;
                    heightField.Max = 1200;
                    heightField.Min = 0;
                    heightField.HideInTable = true;
                    heightField.Width = 50;
                }

                if (pageView.Fields.ContainsKey("Scroll"))
                {
                    ColumnField scrollField = (ColumnField)pageView.Fields["Scroll"];

                    scrollField.Order = 60;
                    scrollField.Category = externalCategory;
                    scrollField.HideInTable = true;
                }

                Category externalLinkCategory = new Category() { Name = "External_Link", Ordinal = 40 };

                if (pageView.Fields.ContainsKey("ExternalNewPage"))
                {
                    ColumnField externalNewPageField = (ColumnField)pageView.Fields["ExternalNewPage"];

                    externalNewPageField.Order = 90;
                    externalNewPageField.DisplayName = "External Link";
                    externalNewPageField.Category = externalLinkCategory;
                    externalNewPageField.TextHtmlControlType = TextHtmlControlType.Url;
                    externalNewPageField.HideInTable = true;
                }
                if (pageView.Fields.ContainsKey("NewTab"))
                {
                    ColumnField newTabPageField = (ColumnField)pageView.Fields["NewTab"];

                    newTabPageField.Order = 100;
                    newTabPageField.DisplayName = "Open in a New Tab";
                    newTabPageField.Category = externalLinkCategory;
                    newTabPageField.HideInTable = true;
                }

                if (pageView.Fields.ContainsKey("Target"))
                {
                    ColumnField targetField = (ColumnField)pageView.Fields["Target"];

                    targetField.Order = 110;
                    targetField.DisplayName = "Target";
                    targetField.Category = externalLinkCategory;
                    targetField.HideInTable = true;
                }

                Category ssrsCategory = new Category() { Name = "Reporting_Services", Ordinal = 40 };

                if (pageView.Fields.ContainsKey("ReportName"))
                {
                    ColumnField reportNameField = (ColumnField)pageView.Fields["ReportName"];

                    reportNameField.Order = 120;
                    reportNameField.DisplayName = "Report Name";
                    reportNameField.Category = ssrsCategory;
                    reportNameField.HideInTable = true;
                }

                if (pageView.Fields.ContainsKey("ReportDisplayName"))
                {
                    ColumnField reportDisplayNameField = (ColumnField)pageView.Fields["ReportDisplayName"];

                    reportDisplayNameField.Order = 130;
                    reportDisplayNameField.DisplayName = "Report Title";
                    reportDisplayNameField.Category = ssrsCategory;
                    reportDisplayNameField.HideInTable = true;
                }

            }
        }

        protected virtual void ConfigCategories(Durados.Web.Mvc.Database configDatabase)
        {
            View viewView = (View)configDatabase.Views["View"];

            Category generalCategory = new Category() { Name = "General", Ordinal = 10 };

            #region Views

            viewView.Fields["Name"].Category = generalCategory;
            viewView.Fields["DisplayName"].Category = generalCategory;
            if (viewView.Fields.ContainsKey("JsonName"))
                viewView.Fields["JsonName"].Category = generalCategory;
            viewView.Fields["EditableTableName"].Category = generalCategory;

            viewView.Fields["DisplayColumn"].Category = generalCategory;
            if (viewView.Fields.ContainsKey("Layout"))
                viewView.Fields["Layout"].Category = generalCategory;

            //Category menuCategory = new Category() { Name = "Menu", Ordinal = 15 };
            //viewView.Fields["WorkspaceID"].Category = menuCategory;
            //viewView.Fields["Menu_Parent"].Category = menuCategory;
            //viewView.Fields["Order"].Category = menuCategory;
            //viewView.Fields["HideInMenu"].Category = menuCategory;

            Category designCategory = new Category() { Name = "Design", Ordinal = 15 };
            if (viewView.Fields.ContainsKey("Skin"))
                viewView.Fields["Skin"].Category = designCategory;
            if (viewView.Fields.ContainsKey("Theme"))
                viewView.Fields["Theme"].Category = designCategory;
            if (viewView.Fields.ContainsKey("RowHeight"))
                viewView.Fields["RowHeight"].Category = designCategory;
            if (viewView.Fields.ContainsKey("ApplySkinToAllViews"))
                viewView.Fields["ApplySkinToAllViews"].Category = designCategory;
            if (viewView.Fields.ContainsKey("CustomThemePath"))
                viewView.Fields["CustomThemePath"].Category = designCategory;
            viewView.Fields["PageSize"].Category = designCategory;
            viewView.Fields["HideFilter"].Category = designCategory;
            viewView.Fields["HideSearch"].Category = designCategory;


            Category toolbarCategory = new Category() { Name = "Toolbar_Settings", Ordinal = 18 };

            viewView.Fields["HidePager"].Category = toolbarCategory;
            viewView.Fields["Popup"].Category = toolbarCategory;
            viewView.Fields["HideToolbar"].Category = toolbarCategory;
            viewView.Fields["CollapseFilter"].Category = toolbarCategory;
            viewView.Fields["ExportToCsv"].Category = toolbarCategory;
            viewView.Fields["ImportFromExcel"].Category = toolbarCategory;
            viewView.Fields["Print"].Category = toolbarCategory;
            viewView.Fields["SaveHistory"].Category = toolbarCategory;
            viewView.Fields["GridEditable"].Category = toolbarCategory;
            viewView.Fields["GridEditableEnabled"].Category = toolbarCategory;
            viewView.Fields["Send"].Category = toolbarCategory;

            Category dashCategory = new Category() { Name = "Advanced_Layout", Ordinal = 24 };
            if (viewView.Fields.ContainsKey("FilterType"))
            {
                viewView.Fields["FilterType"].Category = dashCategory;
            }
            if (viewView.Fields.ContainsKey("DataDisplayType"))
            {
                viewView.Fields["DataDisplayType"].Category = dashCategory;
            }
            if (viewView.Fields.ContainsKey("SortingType"))
            {
                viewView.Fields["SortingType"].Category = dashCategory;
            }
            if (viewView.Fields.ContainsKey("GroupFilterDisplayLabel"))
            {
                viewView.Fields["GroupFilterDisplayLabel"].Category = dashCategory;
            }
            if (viewView.Fields.ContainsKey("EnableTableDisplay"))
            {
                viewView.Fields["EnableTableDisplay"].Category = dashCategory;
            }
            if (viewView.Fields.ContainsKey("EnableDashboardDisplay"))
            {
                viewView.Fields["EnableDashboardDisplay"].Category = dashCategory;
            }
            if (viewView.Fields.ContainsKey("EnablePreviewDisplay"))
            {
                viewView.Fields["EnablePreviewDisplay"].Category = dashCategory;
            }
            if (viewView.Fields.ContainsKey("GridDisplayType"))
            {
                viewView.Fields["GridDisplayType"].Category = generalCategory;
            }

            viewView.Fields["DashboardHeight"].Category = dashCategory;
            viewView.Fields["DashboardWidth"].Category = dashCategory;
            viewView.Fields["DashboardHeight"].HideInTable = true;
            viewView.Fields["DashboardWidth"].HideInTable = true;
            viewView.Fields["DashboardHeight"].IsAdminPreview = true;
            viewView.Fields["DashboardWidth"].IsAdminPreview = true;
            
            foreach (Field field in dashCategory.Fields)
            {
                SetRoles(field, "Developer");
            }

            Category deCategory = new Category() { Name = "Data_Editing", Ordinal = 20 };


            viewView.Fields["ColumnsInDialog"].Category = deCategory;
            viewView.Fields["DataRowView"].Category = deCategory;

            viewView.Fields["AllowCreate"].Category = deCategory;
            viewView.Fields["AllowEdit"].Category = deCategory;
            viewView.Fields["AllowDuplicate"].Category = deCategory;
            viewView.Fields["AllowDelete"].Category = deCategory;
            viewView.Fields["MultiSelect"].Category = deCategory;
            viewView.Fields["HasChildrenRow"].Category = deCategory;
            viewView.Fields["UseOrderForCreate"].Category = deCategory;
            viewView.Fields["UseOrderForEdit"].Category = deCategory;
            viewView.Fields["DefaultSort"].Category = deCategory;
            viewView.Fields["TabCache"].Category = deCategory;
            viewView.Fields["RefreshOnClose"].Category = deCategory;
            viewView.Fields["PermanentFilter"].Category = deCategory;
            if (viewView.Fields.ContainsKey("NosqlPermanentFilter"))
            {
                viewView.Fields["NosqlPermanentFilter"].Category = deCategory;
            }
            if (viewView.Fields.ContainsKey("OpenSingleRow"))
            {
                viewView.Fields["OpenSingleRow"].Category = deCategory;
            }
            if (viewView.Fields.ContainsKey("OpenDialogMax"))
            {
                viewView.Fields["OpenDialogMax"].Category = toolbarCategory;
            }

            Category fCategory = new Category() { Name = "Organize_Columns", Ordinal = 35 };
            viewView.Fields["Fields_Children"].Category = fCategory;

            Category descCategory = new Category() { Name = "Description", Ordinal = 22 };
            viewView.Fields["Description"].Category = descCategory;
            viewView.Fields["NewButtonDescription"].Category = descCategory;
            viewView.Fields["EditButtonDescription"].Category = descCategory;
            viewView.Fields["DuplicateButtonDescription"].Category = descCategory;
            viewView.Fields["AddItemsButtonDescription"].Category = descCategory;
            viewView.Fields["PromoteButtonDescription"].Category = descCategory;
            viewView.Fields["PromoteButtonName"].Category = descCategory;
            viewView.Fields["NewButtonName"].Category = descCategory;
            viewView.Fields["EditButtonName"].Category = descCategory;
            viewView.Fields["DuplicateButtonName"].Category = descCategory;
            viewView.Fields["AddItemsButtonName"].Category = descCategory;
            viewView.Fields["InsertButtonName"].Category = descCategory;
            viewView.Fields["DeleteButtonName"].Category = descCategory;

            Category emailCategory = new Category() { Name = "Email", Ordinal = 24 };

            viewView.Fields["SendTo"].Category = emailCategory;
            viewView.Fields["SendTo"].DisplayName = "To";
            viewView.Fields["SendCc"].Category = emailCategory;
            viewView.Fields["SendCc"].DisplayName = "CC";
            viewView.Fields["SendSubject"].Category = emailCategory;
            viewView.Fields["SendSubject"].DisplayName = "Subject";
            viewView.Fields["SendSubject"].DictionaryType = DictionaryType.DisplayNames;
            viewView.Fields["SendSubject"].DictionaryViewFieldName = "Name";
            viewView.Fields["SendTemplate"].Category = emailCategory;
            viewView.Fields["SendTemplate"].DisplayName = "Message";
            viewView.Fields["SendTemplate"].DictionaryType = DictionaryType.DisplayNames;
            viewView.Fields["SendTemplate"].DictionaryViewFieldName = "Name";


            Category advCategory = new Category() { Name = "Advanced", Ordinal = 25 };
            viewView.Fields["ShowUpDown"].Category = advCategory;
            viewView.Fields["ColumnsInDialogPerCategory"].Category = advCategory;
            viewView.Fields["BaseName"].Category = advCategory;
            viewView.Fields["ContainerGraphicProperties"].Category = advCategory;
            viewView.Fields["RowColorColumnName"].Category = advCategory;
            viewView.Fields["GroupingFields"].Category = advCategory;
            //configView.Fields["EditableTableName"].Category = advCategory;
            viewView.Fields["CreateDateColumnName"].Category = advCategory;
            viewView.Fields["ModifiedDateColumnName"].Category = advCategory;
            viewView.Fields["CreatedByColumnName"].Category = advCategory;
            viewView.Fields["ModifiedByColumnName"].Category = advCategory;
            viewView.Fields["ShowDisabledSteps"].Category = advCategory;
            viewView.Fields["WorkFlowStepsFieldName"].Category = advCategory;
            viewView.Fields["AddItemsFieldName"].Category = advCategory;
            viewView.Fields["DuplicateMessage"].Category = advCategory;
            viewView.Fields["DisplayType"].Category = advCategory;

            viewView.Fields["MaxSubGridHeight"].Category = advCategory;
            viewView.Fields["HistoryNotifyList"].Category = advCategory;
            viewView.Fields["DuplicationMethod"].Category = advCategory;


            Category rolesCategory = new Category() { Name = "Permissions", Ordinal = 30 };
            viewView.Fields["WorkspaceID"].Category = rolesCategory;
            viewView.Fields["DenyCreateRoles"].Category = rolesCategory;
            viewView.Fields["DenyEditRoles"].Category = rolesCategory;
            viewView.Fields["DenyDeleteRoles"].Category = rolesCategory;
            viewView.Fields["DenySelectRoles"].Category = rolesCategory;
            viewView.Fields["Precedent"].Category = rolesCategory;
            viewView.Fields["AllowCreateRoles"].Category = rolesCategory;
            viewView.Fields["AllowEditRoles"].Category = rolesCategory;
            viewView.Fields["AllowDeleteRoles"].Category = rolesCategory;
            viewView.Fields["AllowSelectRoles"].Category = rolesCategory;
            if (viewView.Fields.ContainsKey("ViewOwnerRoles"))
                viewView.Fields["ViewOwnerRoles"].Category = rolesCategory;

            Category xmlCategory = new Category() { Name = "Xml", Ordinal = 35 };
            viewView.Fields["XmlElement"].Category = xmlCategory;

            Category treeCategory = new Category() { Name = "Tree", Ordinal = 35 };
            viewView.Fields["TreeType"].Category = treeCategory;
            viewView.Fields["TreeViewName"].Category = treeCategory;
            viewView.Fields["TreeRelatedFieldName"].Category = treeCategory;
            viewView.Fields["TreeRoot"].Category = treeCategory;


            Category devCategory = new Category() { Name = "Developers", Ordinal = 40 };
            viewView.Fields["SystemView"].Category = devCategory;
            viewView.Fields["BaseViewName"].Category = devCategory;
            viewView.Fields["Controller"].Category = devCategory;
            viewView.Fields["IndexAction"].Category = devCategory;
            viewView.Fields["SetLanguageAction"].Category = devCategory;
            viewView.Fields["CreateAction"].Category = devCategory;
            viewView.Fields["EditAction"].Category = devCategory;
            viewView.Fields["GetJsonViewAction"].Category = devCategory;
            viewView.Fields["DeleteAction"].Category = devCategory;
            viewView.Fields["FilterAction"].Category = devCategory;
            viewView.Fields["UploadAction"].Category = devCategory;
            viewView.Fields["ExportToCsvAction"].Category = devCategory;
            viewView.Fields["PrintAction"].Category = devCategory;
            viewView.Fields["AutoCompleteAction"].Category = devCategory;
            viewView.Fields["AutoCompleteController"].Category = devCategory;
            viewView.Fields["InlineAddingDialogAction"].Category = devCategory;
            viewView.Fields["EditOnlyAction"].Category = devCategory;
            viewView.Fields["InlineAddingCreateAction"].Category = devCategory;
            viewView.Fields["DeleteSelectionAction"].Category = devCategory;
            viewView.Fields["CheckListAction"].Category = devCategory;
            viewView.Fields["RefreshAction"].Category = devCategory;
            viewView.Fields["InlineEditingDialogAction"].Category = devCategory;
            viewView.Fields["InlineEditingEditAction"].Category = devCategory;
            viewView.Fields["DuplicateAction"].Category = devCategory;
            viewView.Fields["InlineDuplicateDialogAction"].Category = devCategory;
            viewView.Fields["InlineDuplicateAction"].Category = devCategory;
            viewView.Fields["InlineSearchDialogAction"].Category = devCategory;
            viewView.Fields["BaseTableName"].Category = devCategory;
            viewView.Fields["Derivation_Parent"].Category = devCategory;
            viewView.Fields["ChartInfo_Parent"].Category = devCategory;
            viewView.Fields["Cached"].Category = advCategory;
            viewView.Fields["NotifyMessageKey"].Category = devCategory;
            viewView.Fields["NotifySubjectKey"].Category = devCategory;
            if (viewView.Fields.ContainsKey("ReloadPage"))
                viewView.Fields["ReloadPage"].Category = devCategory;

            if (viewView.Fields.ContainsKey("SystemView"))
            {
                viewView.Fields["SystemView"].Category = devCategory;
            }
            if (viewView.Fields.ContainsKey("DatabaseTableName"))
                viewView.Fields["DatabaseTableName"].Category = devCategory;

            viewView.Fields["CreateOnlyAction"].Category = devCategory;
            if (viewView.Fields.ContainsKey("EditRichAction"))
                viewView.Fields["EditRichAction"].Category = devCategory;
            if (viewView.Fields.ContainsKey("GetRichAction"))
                viewView.Fields["GetRichAction"].Category = devCategory;
            if (viewView.Fields.ContainsKey("GetSelectListAction"))
                viewView.Fields["GetSelectListAction"].Category = devCategory;
            if (viewView.Fields.ContainsKey("EditSelectionAction"))
                viewView.Fields["EditSelectionAction"].Category = devCategory;

            if (viewView.Fields.ContainsKey("AnotherRowLinkText"))
                viewView.Fields["AnotherRowLinkText"].Category = devCategory;
            if (viewView.Fields.ContainsKey("AnotherRowLinkFunction"))
                viewView.Fields["AnotherRowLinkFunction"].Category = devCategory;
            if (viewView.Fields.ContainsKey("JavaScripts"))
                viewView.Fields["JavaScripts"].Category = devCategory;
            if (viewView.Fields.ContainsKey("StyleSheets"))
                viewView.Fields["StyleSheets"].Category = devCategory;
            if (viewView.Fields.ContainsKey("IsImageTable"))
                viewView.Fields["IsImageTable"].Category = devCategory;
            if (viewView.Fields.ContainsKey("OrdinalColumnName"))
                viewView.Fields["OrdinalColumnName"].Category = devCategory;
            if (viewView.Fields.ContainsKey("ImageSrcColumnName"))
                viewView.Fields["ImageSrcColumnName"].Category = devCategory;
            if (viewView.Fields.ContainsKey("InAddItemsaddAllItems"))
                viewView.Fields["InAddItemsaddAllItems"].Category = devCategory;

            Category sysCategory = new Category() { Name = "System", Ordinal = 50 };
            viewView.Fields["Views_Parent"].Category = sysCategory;

            if (viewView.Fields.ContainsKey("AllFilterValuesAction"))
            {
                viewView.Fields["AllFilterValuesAction"].HideInTable = true;
                viewView.Fields["AllFilterValuesAction"].DisplayName = "AllFilterValues Action";
                viewView.Fields["AllFilterValuesAction"].Category = devCategory;
            }

            Category styleCategory = new Category() { Name = "Advanced_Design", Ordinal = 17 };
            if (viewView.Fields.ContainsKey("Background"))
            {
                viewView.Fields["Background"].Category = styleCategory;
            }
            if (viewView.Fields.ContainsKey("RowBackground"))
            {
                viewView.Fields["RowBackground"].Category = styleCategory;
            }
            if (viewView.Fields.ContainsKey("HoverBackground"))
            {
                viewView.Fields["HoverBackground"].Category = styleCategory;
            }
            if (viewView.Fields.ContainsKey("AlternateRowBackground"))
            {
                viewView.Fields["AlternateRowBackground"].Category = styleCategory;
            }
            if (viewView.Fields.ContainsKey("ToolBoxBackground"))
            {
                viewView.Fields["ToolBoxBackground"].Category = styleCategory;
            }
            if (viewView.Fields.ContainsKey("FontColor"))
            {
                viewView.Fields["FontColor"].Category = styleCategory;
            }
            if (viewView.Fields.ContainsKey("TextFontColor"))
            {
                viewView.Fields["TextFontColor"].Category = styleCategory;
            }
            if (viewView.Fields.ContainsKey("ToolBoxTextColor"))
            {
                viewView.Fields["ToolBoxTextColor"].Category = styleCategory;
            }
            if (viewView.Fields.ContainsKey("AlterTextColor"))
            {
                viewView.Fields["AlterTextColor"].Category = styleCategory;
            }
            if (viewView.Fields.ContainsKey("HoverTextColor"))
            {
                viewView.Fields["HoverTextColor"].Category = styleCategory;
            }

            if (viewView.Fields.ContainsKey("BorderColor"))
            {
                viewView.Fields["BorderColor"].Category = styleCategory;
            }
            if (viewView.Fields.ContainsKey("ApplyColorDesignToAllViews"))
            {
                viewView.Fields["ApplyColorDesignToAllViews"].Category = styleCategory;
            }

            #region CreateOverride

            if (viewView.Fields.ContainsKey("CreateOverride"))
            {
                viewView.Fields["CreateOverride"].HideInTable = true;
                viewView.Fields["CreateOverride"].DisplayName = "Create Override By Workflow";
                viewView.Fields["CreateOverride"].Category = devCategory;
                viewView.Fields["CreateOverride"].DefaultValue = false;
            }

            #endregion

            #region DeleteOverride

            if (viewView.Fields.ContainsKey("DeleteOverride"))
            {
                viewView.Fields["DeleteOverride"].HideInTable = true;
                viewView.Fields["DeleteOverride"].DisplayName = "Delete Override By Workflow";
                viewView.Fields["DeleteOverride"].Category = devCategory;
                viewView.Fields["DeleteOverride"].DefaultValue = false;
            }

            #endregion

            #endregion

            #region Fields
            View fieldView = (View)configDatabase.Views["Field"];

            #region General
            //fieldView.Fields["DisplayName"].Category = displayCategory;
            fieldView.Fields["DisplayName"].Category = generalCategory;
            if (fieldView.Fields.ContainsKey("JsonName"))
                fieldView.Fields["JsonName"].Category = generalCategory;
            fieldView.Fields["Name"].Category = generalCategory;
            fieldView.Fields["DatabaseNames"].Category = generalCategory;

            fieldView.Fields["RelatedViewName"].Category = generalCategory;
            fieldView.Fields["FieldType"].Category = generalCategory;
            fieldView.Fields["DataType"].Category = generalCategory;
            fieldView.Fields["DisplayFormat"].Category = generalCategory;
            fieldView.Fields["Formula"].Category = generalCategory;
            fieldView.Fields["HideInTable"].Category = generalCategory;
            fieldView.Fields["Dashboard"].Category = generalCategory;

            if (fieldView.Fields.ContainsKey("Preview"))
            {
                fieldView.Fields["Preview"].DisplayName = "Display in Preview";
                fieldView.Fields["Preview"].Category = generalCategory;
            }

            Category alCategory = new Category() { Name = "Advanced_Layout", Ordinal = 11 };

            fieldView.Fields["Excluded"].Category = alCategory;
            fieldView.Fields["Required"].Category = alCategory;
            fieldView.Fields["Unique"].Category = alCategory;
            fieldView.Fields["ExcludeInUpdate"].Category = alCategory;
            fieldView.Fields["ExcludeInInsert"].Category = alCategory;

            fieldView.Fields["DefaultValue"].Category = alCategory;
            fieldView.Fields["DefaultFilter"].Category = alCategory;
            if (fieldView.Fields.ContainsKey("Min"))
                fieldView.Fields["Min"].Category = alCategory;
            if (fieldView.Fields.ContainsKey("Max"))
                fieldView.Fields["Max"].Category = alCategory;
            if (fieldView.Fields.ContainsKey("FtpUpload_Parent"))
            {
                fieldView.Fields["FtpUpload_Parent"].Category = alCategory;
                fieldView.Fields["FtpUpload_Parent"].HideInTable = true;
                ((ParentField)fieldView.Fields["FtpUpload_Parent"]).InlineAdding = true;
                ((ParentField)fieldView.Fields["FtpUpload_Parent"]).InlineEditing = true;
            }

            if (fieldView.Fields.ContainsKey("AutoIncrement"))
            {
                fieldView.Fields["AutoIncrement"].Category = alCategory;
            }
            if (fieldView.Fields.ContainsKey("AutoIncrementSequanceName"))
            {
                fieldView.Fields["AutoIncrementSequanceName"].Category = alCategory;
            }
            
            fieldView.Fields["Fields_Parent"].Category = alCategory;
            #endregion

            #region Layout
            Category layoutCategory = new Category() { Name = "Form_Layout", Ordinal = 21 };
            fieldView.Fields["GraphicProperties"].Category = layoutCategory;
            fieldView.Fields["ColSpanInDialog"].Category = layoutCategory;
            fieldView.Fields["Seperator"].Category = layoutCategory;
            fieldView.Fields["Width"].Category = layoutCategory;
            fieldView.Fields["SeperatorTitle"].Category = layoutCategory;
            fieldView.Fields["PreLabel"].Category = layoutCategory;
            fieldView.Fields["PostLabel"].Category = layoutCategory;
            #endregion


            #region Grid View
            Category gridViewCategory = new Category() { Name = "Grid", Ordinal = 15 };
            fieldView.Fields["Description"].Category = gridViewCategory;
            fieldView.Fields["Order"].Category = gridViewCategory;
            fieldView.Fields["HideInFilter"].Category = gridViewCategory;
            fieldView.Fields["DisableInFilter"].Category = gridViewCategory;
            fieldView.Fields["MultiFilter"].Category = gridViewCategory;
            fieldView.Fields["AdvancedFilter"].Category = gridViewCategory;
            fieldView.Fields["Sortable"].Category = gridViewCategory;
            fieldView.Fields["NoHyperlink"].Category = gridViewCategory;
            fieldView.Fields["GridEditable"].Category = gridViewCategory;
            fieldView.Fields["Searchable"].Category = gridViewCategory;


            if (fieldView.Fields.ContainsKey("TextAlignment"))
            {
                ColumnField textAlignmentColumnField = (ColumnField)fieldView.Fields["TextAlignment"];
                textAlignmentColumnField.Category = gridViewCategory;
                textAlignmentColumnField.OrderForCreate = 10;
                textAlignmentColumnField.OrderForEdit = 10;

            }
            if (fieldView.Fields.ContainsKey("ShowColumnHeader"))
            {
                fieldView.Fields["ShowColumnHeader"].Category = gridViewCategory;

            }
            #region Preview


            #endregion

            if (fieldView.Fields.ContainsKey("GroupFilterWidth"))
            {
                fieldView.Fields["GroupFilterWidth"].HideInTable = true;
                fieldView.Fields["GroupFilterWidth"].DisplayName = "Group Filter Width";
                fieldView.Fields["GroupFilterWidth"].DefaultValue = 80;
                fieldView.Fields["GroupFilterWidth"].HideInCreate = false;
                fieldView.Fields["GroupFilterWidth"].OrderForEdit = 160;
                fieldView.Fields["GroupFilterWidth"].OrderForCreate = 160;
                fieldView.Fields["GroupFilterWidth"].Category = gridViewCategory;
            }
            if (fieldView.Fields.ContainsKey("GroupFilterDisplayLabel"))
                fieldView.Fields["GroupFilterDisplayLabel"].Category = gridViewCategory;

            fieldView.Fields["Dialog"].Category = gridViewCategory;
            fieldView.Fields["PartialLength"].Category = gridViewCategory;
            fieldView.Fields["EditInTableView"].Category = gridViewCategory;
            fieldView.Fields["ContainerGraphicProperties"].Category = gridViewCategory;
            #endregion

            #region Form View
            Category formViewCategory = new Category() { Name = "Form", Ordinal = 20 };

            fieldView.Fields["Category_Parent"].Category = formViewCategory;
            fieldView.Fields["OrderForCreate"].Category = formViewCategory;
            fieldView.Fields["OrderForEdit"].Category = formViewCategory;
            fieldView.Fields["HideInEdit"].Category = formViewCategory;
            fieldView.Fields["HideInCreate"].Category = formViewCategory;
            fieldView.Fields["DisableInEdit"].Category = formViewCategory;
            fieldView.Fields["DisableInCreate"].Category = formViewCategory;
            fieldView.Fields["IncludeInDuplicate"].Category = formViewCategory;
            fieldView.Fields["DisableInDuplicate"].Category = formViewCategory;
            fieldView.Fields["ChildrenHtmlControlType"].Category = formViewCategory;
            fieldView.Fields["InlineSearch"].Category = formViewCategory;
            fieldView.Fields["InlineAdding"].Category = formViewCategory;
            fieldView.Fields["InlineEditing"].Category = formViewCategory;
            fieldView.Fields["SelectionSortColumn"].Category = formViewCategory;
            fieldView.Fields["InlineSearchView"].Category = formViewCategory;
            fieldView.Fields["AllowDuplication"].Category = formViewCategory;
            fieldView.Fields["InlineDuplicate"].Category = formViewCategory;
            fieldView.Fields["NoCache"].Category = formViewCategory;
            fieldView.Fields["HideInDerivation"].Category = formViewCategory;
            fieldView.Fields["TrimSpaces"].Category = formViewCategory;
            fieldView.Fields["Custom"].Category = formViewCategory;
            fieldView.Fields["DisplayField"].Category = formViewCategory;
            if (fieldView.Fields.ContainsKey("SubgridInstructions"))
            {
                fieldView.Fields["SubgridInstructions"].Category = formViewCategory;
            }

            #endregion

            #region behaviorCategory
            Category bhCategory = new Category() { Name = "Behaviour", Ordinal = 20 };

            fieldView.Fields["Rich"].Category = bhCategory;
            //fieldView.Fields["StringConversionFormat"].Category = bhCategory;
            fieldView.Fields["ParentHtmlControlType"].Category = bhCategory;
            fieldView.Fields["TextHtmlControlType"].Category = bhCategory;
            bool debug = System.Configuration.ConfigurationManager.AppSettings["Debug"].ToLower() == "true";
            fieldView.Fields["SpecialColumn"].Category = bhCategory;
            #endregion

            #region Advance
            fieldView.Fields["HideInTableMobile"].Category = advCategory;
            fieldView.Fields["MinWidth"].Category = advCategory;
            fieldView.Fields["ShowDependencyInTable"].Category = advCategory;
            fieldView.Fields["RadioColumns"].Category = advCategory;
            fieldView.Fields["BooleanHtmlControlType"].Category = advCategory;
            fieldView.Fields["RadioOrientation"].Category = advCategory;
            fieldView.Fields["LoadForBlockTemplate"].Category = advCategory;
            //fieldView.Fields["Unique"].Category = advCategory;
            fieldView.Fields["AutocompleteColumn"].Category = advCategory;
            fieldView.Fields["AutocompleteTable"].Category = advCategory;
            fieldView.Fields["AutocompleteSql"].Category = advCategory;
            fieldView.Fields["ContainerGraphicField"].Category = advCategory;
            fieldView.Fields["TableCellMinWidth"].Category = advCategory;
            fieldView.Fields["Refresh"].Category = advCategory;
            fieldView.Fields["SaveHistory"].Category = advCategory;
            fieldView.Fields["CheckListInTableLimit"].Category = advCategory;
            fieldView.Fields["Import"].Category = advCategory;
            fieldView.Fields["MultiValueParentViewName"].Category = advCategory;
            fieldView.Fields["MultiValueAdditionals"].Category = advCategory;
            fieldView.Fields["SubGridExport"].Category = advCategory;
            fieldView.Fields["SubGridPlacement"].Category = advCategory;
            fieldView.Fields["DropDownValueField"].Category = advCategory;
            fieldView.Fields["DropDownDisplayField"].Category = advCategory;
            fieldView.Fields["CloneChildrenViewName"].Category = advCategory;
            fieldView.Fields["UpdateParent"].Category = advCategory;
            fieldView.Fields["UpdateParentInGrid"].Category = advCategory;
            fieldView.Fields["SearchFilter"].Category = advCategory;
            fieldView.Fields["IsUnique"].Category = advCategory;

            //fieldView.Fields["DataType"].Category = advCategory;
            #endregion

            ColumnField baseName = (ColumnField)viewView.Fields["BaseName"];
            baseName.TextHtmlControlType = TextHtmlControlType.DropDown;
            baseName.MultiValueParentViewName = "View";
            baseName.DropDownValueField = "Name";
            baseName.DropDownDisplayField = "Name";
            baseName.DisplayName = "Base View";
            baseName.NoHyperlink = false;

            #region Roles #endregion
            fieldView.Fields["AllowCreateRoles"].Category = rolesCategory;
            fieldView.Fields["AllowEditRoles"].Category = rolesCategory;
            fieldView.Fields["AllowSelectRoles"].Category = rolesCategory;
            fieldView.Fields["Precedent"].Category = rolesCategory;
            fieldView.Fields["DenyCreateRoles"].Category = rolesCategory;
            fieldView.Fields["DenyEditRoles"].Category = rolesCategory;
            fieldView.Fields["DenySelectRoles"].Category = rolesCategory;
            #endregion

            #region Encryption
            Category encryptionCategory = new Category() { Name = "Encryption", Ordinal = 28 };
            try
            {
                fieldView.Fields["Encrypt"].Category = encryptionCategory;
                fieldView.Fields["Encrypted"].Category = encryptionCategory;
                fieldView.Fields["CertificateName"].Category = encryptionCategory;
                fieldView.Fields["SymmetricKeyName"].Category = encryptionCategory;
                fieldView.Fields["SymmetricKeyAlgorithm"].Category = encryptionCategory;
            }
            catch { }

            #endregion

            #region XML Properties
            fieldView.Fields["AttributesNames"].Category = xmlCategory;
            fieldView.Fields["AttributesNames"].HideInTable = true;
            fieldView.Fields["ExportToXml"].Category = xmlCategory;
            fieldView.Fields["ExportToXml"].HideInTable = true;
            fieldView.Fields["XmlElement"].Category = xmlCategory;
            fieldView.Fields["XmlElement"].HideInTable = true;
            fieldView.Fields["XmlFields"].Category = xmlCategory;
            fieldView.Fields["XmlFields"].HideInTable = true;
            #endregion

            #region Developer
            if (fieldView.Fields.ContainsKey("OriginalParentRelatedFieldName"))
            {
                fieldView.Fields["OriginalFieldName"].Category = devCategory;
                fieldView.Fields["OriginalParentRelatedFieldName"].Category = devCategory;

            }

            fieldView.Fields["ID"].Category = devCategory;
            if (fieldView.Fields.ContainsKey("AutocompleteMathing"))
                fieldView.Fields["AutocompleteMathing"].Category = devCategory;
            if (fieldView.Fields.ContainsKey("AutocompleteFilter"))
                fieldView.Fields["AutocompleteFilter"].Category = devCategory;
            if (fieldView.Fields.ContainsKey("LimitToStartAutocomplete"))
                fieldView.Fields["LimitToStartAutocomplete"].Category = devCategory;
            if (fieldView.Fields.ContainsKey("Localize"))
                fieldView.Fields["Localize"].Category = devCategory;
            if (fieldView.Fields.ContainsKey("BaseFieldName"))
                fieldView.Fields["BaseFieldName"].Category = devCategory;
            if (fieldView.Fields.ContainsKey("EnumType"))
                fieldView.Fields["EnumType"].Category = devCategory;
            if (fieldView.Fields.ContainsKey("NullString"))
                fieldView.Fields["NullString"].Category = devCategory;

            if (fieldView.Fields.ContainsKey("Upload_Parent"))
                fieldView.Fields["Upload_Parent"].Category = devCategory;
            if (fieldView.Fields.ContainsKey("Milestone_Parent"))
            {
                fieldView.Fields["Milestone_Parent"].Category = advCategory;
                fieldView.Fields["Milestone_Parent"].HideInTable = true;
                (fieldView.Fields["Milestone_Parent"] as ParentField).InlineAdding = true;
                (fieldView.Fields["Milestone_Parent"] as ParentField).InlineEditing = true;
                (fieldView.Fields["Milestone_Parent"] as ParentField).InlineDuplicate = true;
            }
            if (fieldView.Fields.ContainsKey("DependencyFieldName"))
                fieldView.Fields["DependencyFieldName"].Category = devCategory;
            if (fieldView.Fields.ContainsKey("InsideTriggerFieldName"))
                fieldView.Fields["InsideTriggerFieldName"].Category = devCategory;

            if (fieldView.Fields.ContainsKey("CounterInitiated"))
                fieldView.Fields["CounterInitiated"].Category = devCategory;
            if (fieldView.Fields.ContainsKey("Counter"))
                fieldView.Fields["Counter"].Category = devCategory;
            fieldView.Fields["DependencyData"].Category = devCategory;

            //if (fieldView.Fields.ContainsKey("RelatedViewName"))
            //    fieldView.Fields["RelatedViewName"].Category = devCategory;


            if (fieldView.Fields.ContainsKey("Format"))
                fieldView.Fields["Format"].Category = alCategory;

            fieldView.Fields["LabelContentLayout"].Category = devCategory;
            fieldView.Fields["PartFromUniqueIndex"].Category = devCategory;
            fieldView.Fields["BrowserAutocomplete"].Category = devCategory;
            fieldView.Fields["Integral"].Category = devCategory;
            //fieldView.Fields["DatabaseNames"].Category = devCategory;

            #endregion

            //if (fieldView.Fields.ContainsKey("Fields_Parent"))
            //    fieldView.Fields["Fields_Parent"].Category = sysCategory;
            if (fieldView.Fields.ContainsKey("FieldType"))
            {
                fieldView.Fields["FieldType"].Category = sysCategory;
            }


            #endregion

            #region Rules
            configDatabase.Views["Rule"].Fields["Name"].Required = true;
            if (configDatabase.Views["Rule"].Fields.ContainsKey("Rules_Parent"))
            {
                configDatabase.Views["Rule"].Fields["Rules_Parent"].DisplayName = "View/Table";
                configDatabase.Views["Rule"].Fields["Rules_Parent"].Required = true;
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("DataAction"))
            {
                ((ColumnField)configDatabase.Views["Rule"].Fields["DataAction"]).EnumType = typeof(Durados.TriggerDataAction).AssemblyQualifiedName;
                ((ColumnField)configDatabase.Views["Rule"].Fields["DataAction"]).Required = true;
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("WorkflowAction"))
            {
                ((ColumnField)configDatabase.Views["Rule"].Fields["WorkflowAction"]).EnumType = typeof(Durados.WorkflowAction).AssemblyQualifiedName;
                ((ColumnField)configDatabase.Views["Rule"].Fields["WorkflowAction"]).Required = true;
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("WhereCondition"))
            {
                ((ColumnField)configDatabase.Views["Rule"].Fields["WhereCondition"]).TextHtmlControlType = TextHtmlControlType.TextArea;
                ((ColumnField)configDatabase.Views["Rule"].Fields["WhereCondition"]).Required = true;
                ((ColumnField)configDatabase.Views["Rule"].Fields["WhereCondition"]).DictionaryType = DictionaryType.InternalNamesPlaceHolders;
                ((ColumnField)configDatabase.Views["Rule"].Fields["WhereCondition"]).DictionaryViewFieldName = "Rules_Parent";

            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("Parameters_Children"))
            {
                configDatabase.Views["Rule"].Fields["Parameters_Children"].HideInTable = false;
                configDatabase.Views["Rule"].Fields["Parameters_Children"].Order = -10;
                ((ChildrenField)configDatabase.Views["Rule"].Fields["Parameters_Children"]).ChildrenHtmlControlType = ChildrenHtmlControlType.Grid;
            }
            configDatabase.Views["Rule"].DisplayName = "Business Rules";

            if (configDatabase.Views["Rule"].Fields.ContainsKey("Views"))
            {
                ColumnField additionalViews = (ColumnField)configDatabase.Views["Rule"].Fields["Views"];
                additionalViews.MultiValueParentViewName = "View";
                additionalViews.DisplayName = "Additional View";
                additionalViews.DropDownValueField = "Name";
                additionalViews.TextHtmlControlType = TextHtmlControlType.CheckList;
            }

            configDatabase.Views["Rule"].UseOrderForCreate = true;
            configDatabase.Views["Rule"].UseOrderForEdit = true;
            configDatabase.Views["Rule"].Fields["Name"].OrderForCreate = 10;
            configDatabase.Views["Rule"].Fields["Name"].OrderForEdit = 10;
            configDatabase.Views["Rule"].Fields["Name"].ColSpanInDialog = 2;
            configDatabase.Views["Rule"].Fields["Name"].GraphicProperties = "LongText";
            configDatabase.Views["Rule"].Fields["Rules_Parent"].OrderForCreate = 20;
            configDatabase.Views["Rule"].Fields["Rules_Parent"].OrderForEdit = 20;
            configDatabase.Views["Rule"].Fields["DataAction"].OrderForCreate = 30;
            configDatabase.Views["Rule"].Fields["DataAction"].OrderForEdit = 30;
            if (configDatabase.Views["Rule"].Fields.ContainsKey("Views"))
            {
                configDatabase.Views["Rule"].Fields["Views"].OrderForCreate = 40;
                configDatabase.Views["Rule"].Fields["Views"].OrderForEdit = 40;
            }

            if (configDatabase.Views["Rule"].Fields.ContainsKey("NotifyTo"))
            {
                ColumnField notifyToColumn = (ColumnField)configDatabase.Views["Rule"].Fields["NotifyTo"];
                notifyToColumn.HideInDerivation = true;
                notifyToColumn.DisplayName = "To";
                notifyToColumn.HideInTable = true;
                notifyToColumn.SeperatorTitle = "Parameters";
                notifyToColumn.Seperator = true;

            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("NotifyCC"))
            {
                ColumnField notifyCCColumn = (ColumnField)configDatabase.Views["Rule"].Fields["NotifyCC"];
                notifyCCColumn.HideInDerivation = true;
                notifyCCColumn.DisplayName = "cc";
                notifyCCColumn.HideInTable = true;
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("NotifyBCC"))
            {
                ColumnField notifyBCCColumn = (ColumnField)configDatabase.Views["Rule"].Fields["NotifyBCC"];
                notifyBCCColumn.HideInDerivation = true;
                notifyBCCColumn.DisplayName = "bcc";
                notifyBCCColumn.HideInTable = true;
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("NotifyFrom"))
            {
                ColumnField notifyFromColumn = (ColumnField)configDatabase.Views["Rule"].Fields["NotifyFrom"];
                notifyFromColumn.HideInDerivation = true;
                notifyFromColumn.DisplayName = "from";
                notifyFromColumn.HideInTable = true;
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("NotifySubject"))
            {
                ColumnField notifySubjectColumn = (ColumnField)configDatabase.Views["Rule"].Fields["NotifySubject"];
                notifySubjectColumn.ColSpanInDialog = 2;
                notifySubjectColumn.HideInDerivation = true;
                notifySubjectColumn.DisplayName = "Subject";
                notifySubjectColumn.HideInTable = true;
                notifySubjectColumn.Width = 604;
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("NotifyMessage"))
            {
                ColumnField notifyMessageColumn = (ColumnField)configDatabase.Views["Rule"].Fields["NotifyMessage"];
                notifyMessageColumn.TextHtmlControlType = TextHtmlControlType.TextArea;
                notifyMessageColumn.ColSpanInDialog = 2;
                notifyMessageColumn.HideInDerivation = true;
                notifyMessageColumn.HideInTable = true;
                notifyMessageColumn.DisplayName = "Message";
                notifyMessageColumn.DictionaryViewFieldName = "Rules_Parent";
                //  (notifyMessageColumn as ColumnField).CssClass += " field-dic";
                (notifyMessageColumn as ColumnField).DictionaryType = DictionaryType.DisplayNames;

            }


            if (configDatabase.Views["Rule"].Fields.ContainsKey("ExecuteCommand"))
            {
                ColumnField executeCommandColumn = (ColumnField)configDatabase.Views["Rule"].Fields["ExecuteCommand"];
                executeCommandColumn.HideInDerivation = true;
                executeCommandColumn.DisplayName = "Command";
                executeCommandColumn.HideInTable = true;
                executeCommandColumn.TextHtmlControlType = TextHtmlControlType.TextArea;
                executeCommandColumn.ColSpanInDialog = 2;
                executeCommandColumn.DictionaryType = DictionaryType.InternalNamesPlaceHolders;
                executeCommandColumn.DictionaryViewFieldName = "Rules_Parent";
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("ExecuteMessage"))
            {
                ColumnField executeMessageColumn = (ColumnField)configDatabase.Views["Rule"].Fields["ExecuteMessage"];
                executeMessageColumn.HideInDerivation = true;
                executeMessageColumn.DisplayName = "Message";
                executeMessageColumn.HideInTable = true;
                executeMessageColumn.TextHtmlControlType = TextHtmlControlType.TextArea;
                executeMessageColumn.ColSpanInDialog = 2;
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("ValidateCommand1"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["ValidateCommand1"];
                column.HideInDerivation = true;
                column.DisplayName = "Expression";
                column.HideInTable = true;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.CssClass = "textarea2linev";
                column.LabelContentLayout = Orientation.Vertical;
                column.DictionaryType = DictionaryType.InternalNamesPlaceHolders;
                column.DictionaryViewFieldName = "Rules_Parent";
                // column.PreLabel = "To validate string columns souraund column name with Apostrophe";
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("ValidateMessage1"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["ValidateMessage1"];
                column.HideInDerivation = true;
                column.DisplayName = "Non Valid Message";
                column.HideInTable = true;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.CssClass = "textarea2linev";
                column.LabelContentLayout = Orientation.Vertical;
            }

            if (configDatabase.Views["Rule"].Fields.ContainsKey("ValidateCommand2"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["ValidateCommand2"];
                column.HideInDerivation = true;
                column.DisplayName = "Expression";
                column.HideInTable = true;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.CssClass = "textarea2linev";
                column.LabelContentLayout = Orientation.Vertical;
                column.DictionaryType = DictionaryType.InternalNamesPlaceHolders;
                column.DictionaryViewFieldName = "Rules_Parent";
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("ValidateMessage2"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["ValidateMessage2"];
                column.HideInDerivation = true;
                column.DisplayName = "Non Valid Message";
                column.HideInTable = true;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.CssClass = "textarea2linev";
                column.LabelContentLayout = Orientation.Vertical;

            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("ValidateCommand3"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["ValidateCommand3"];
                column.HideInDerivation = true;
                column.DisplayName = "Expression";
                column.HideInTable = true;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.CssClass = "textarea2linev";
                column.LabelContentLayout = Orientation.Vertical;
                column.DictionaryType = DictionaryType.InternalNamesPlaceHolders;
                column.DictionaryViewFieldName = "Rules_Parent";
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("ValidateMessage3"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["ValidateMessage3"];
                column.HideInDerivation = true;
                column.DisplayName = "Non Valid Message";
                column.HideInTable = true;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.CssClass = "textarea2linev";
                column.LabelContentLayout = Orientation.Vertical;
            }

            if (configDatabase.Views["Rule"].Fields.ContainsKey("ValidateCommand4"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["ValidateCommand4"];
                column.HideInDerivation = true;
                column.DisplayName = "Expression";
                column.HideInTable = true;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.CssClass = "textarea2linev";
                column.LabelContentLayout = Orientation.Vertical;
                column.DictionaryType = DictionaryType.InternalNamesPlaceHolders;
                column.DictionaryViewFieldName = "Rules_Parent";
            }
            if (configDatabase.Views["Rule"].Fields.ContainsKey("ValidateMessage4"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["ValidateMessage4"];
                column.HideInDerivation = true;
                column.DisplayName = "Non Valid Message";
                column.HideInTable = true;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.CssClass = "textarea2linev";
                column.LabelContentLayout = Orientation.Vertical;

            }

            if (configDatabase.Views["Rule"].Fields.ContainsKey("WebService1"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["WebService1"];
                column.HideInDerivation = true;
                column.DisplayName = "Web Service Url";
                column.HideInTable = true;
                column.Width = 500;
                column.ColSpanInDialog = 2;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.DictionaryType = DictionaryType.InternalNamesPlaceHolders;
                column.DictionaryViewFieldName = "Rules_Parent";
                //column.CssClass = "textboxmid";
            }

            if (configDatabase.Views["Rule"].Fields.ContainsKey("ExecuteInParameters"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["ExecuteInParameters"];
                column.HideInDerivation = true;
                column.DisplayName = "In parameters";
                column.HideInTable = true;

            }

            if (configDatabase.Views["Rule"].Fields.ContainsKey("ExecuteOutParameters"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["ExecuteOutParameters"];
                column.HideInDerivation = true;
                column.DisplayName = "Out Parameters";
                column.HideInTable = true;
            }

            if (configDatabase.Views["Rule"].Fields.ContainsKey("Code"))
            {
                ColumnField column = (ColumnField)configDatabase.Views["Rule"].Fields["Code"];
                column.HideInDerivation = true;
                column.TextHtmlControlType = TextHtmlControlType.TextArea;
                column.ColSpanInDialog = 2;
                column.HideInTable = true;
            }
            configDatabase.Views["Rule"].Fields["WorkflowAction"].OrderForCreate = 50;
            configDatabase.Views["Rule"].Fields["WorkflowAction"].OrderForEdit = 50;
            configDatabase.Views["Rule"].Fields["DatabaseViewName"].OrderForCreate = 60;
            configDatabase.Views["Rule"].Fields["DatabaseViewName"].OrderForEdit = 60;
            configDatabase.Views["Rule"].Fields["UseSqlParser"].OrderForCreate = 70;
            configDatabase.Views["Rule"].Fields["UseSqlParser"].OrderForEdit = 70;
            configDatabase.Views["Rule"].Fields["WhereCondition"].OrderForCreate = 80;
            configDatabase.Views["Rule"].Fields["WhereCondition"].OrderForEdit = 80;
            configDatabase.Views["Rule"].Fields["WhereCondition"].ColSpanInDialog = 2;

            Category ruleCategory = new Category() { Name = "General", Ordinal = 10 };
            foreach (Field field in configDatabase.Views["Rule"].Fields.Values)
                field.Category = ruleCategory;
            //configDatabase.Views["Rule"].Fields["Name"].Category = ruleCategory;
            //configDatabase.Views["Rule"].Fields["Rules_Parent"].Category = ruleCategory;
            //configDatabase.Views["Rule"].Fields["Views"].Category = ruleCategory;
            //configDatabase.Views["Rule"].Fields["WorkflowAction"].Category = ruleCategory;
            //configDatabase.Views["Rule"].Fields["DatabaseViewName"].Category = ruleCategory;
            //configDatabase.Views["Rule"].Fields["UseSqlParser"].Category = ruleCategory;
            //configDatabase.Views["Rule"].Fields["WhereCondition"].Category = ruleCategory;
            //configDatabase.Views["Rule"].Fields["DataAction"].Category = ruleCategory;

            Category ruleCategoryParam = new Category() { Name = "Parameters", Ordinal = 20 };
            configDatabase.Views["Rule"].Fields["Parameters_Children"].Category = ruleCategoryParam;
            configDatabase.Views["Rule"].Fields["Parameters_Children"].AllowSelectRoles = "Admin,Developer";
            configDatabase.Views["Rule"].Fields["Parameters_Children"].AllowEditRoles = "Admin,Developer";
            configDatabase.Views["Rule"].Fields["Parameters_Children"].AllowCreateRoles = "Admin,Developer";

            configDatabase.Views["Rule"].Derivation = new Derivation()
            {
                DerivationField = "WorkflowAction",
                Deriveds = @"CompleteStep~Approval~Document~Xml~Custom~;ExecuteCommand,ExecuteMessage,NotifyMessage,NotifySubject,NotifyFrom,NotifyBCC,NotifyCC,NotifyTo,ValidateCommand1,ValidateMessage1,ValidateCommand2,ValidateMessage2,ValidateCommand3,ValidateMessage3,ValidateCommand4,ValidateMessage4,WebService1,ExecuteInParameters,ExecuteOutParameters
                    |Notify;ExecuteCommand,ExecuteMessage,ValidateCommand1,ValidateMessage1,ValidateCommand2,ValidateMessage2,ValidateCommand3,ValidateMessage3,ValidateCommand4,ValidateMessage4,WebService1,ExecuteInParameters,ExecuteOutParameters
                    |Execute;NotifyMessage,NotifySubject,NotifyFrom,NotifyBCC,NotifyCC,NotifyTo,ValidateCommand1,ValidateMessage1,ValidateCommand2,ValidateMessage2,ValidateCommand3,ValidateMessage3,ValidateCommand4,ValidateMessage4,WebService1
                    |Validate;ExecuteCommand,ExecuteMessage,NotifyMessage,NotifySubject,NotifyFrom,NotifyBCC,NotifyCC,NotifyTo,WebService1,ExecuteInParameters,ExecuteOutParameters
                    |WebService;ExecuteCommand,ExecuteMessage,NotifyMessage,NotifySubject,NotifyFrom,NotifyBCC,NotifyCC,NotifyTo,ValidateCommand1,ValidateMessage1,ValidateCommand2,ValidateMessage2,ValidateCommand3,ValidateMessage3,ValidateCommand4,ValidateMessage4,ExecuteInParameters,ExecuteOutParameters"
            };
            #endregion

            #region Parameter
            configDatabase.Views["Parameter"].ColumnsInDialog = 1;
            ((ColumnField)configDatabase.Views["Parameter"].Fields["Name"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            ((ColumnField)configDatabase.Views["Parameter"].Fields["Name"]).CssClass = "exwtextareawide";
            ((ColumnField)configDatabase.Views["Parameter"].Fields["Value"]).TextHtmlControlType = TextHtmlControlType.TextArea;
            ((ColumnField)configDatabase.Views["Parameter"].Fields["Value"]).CssClass = "exwtextareawide";
            ((ColumnField)configDatabase.Views["Parameter"].Fields["ID"]).HideInTable = true;

            ((ParentField)configDatabase.Views["Parameter"].Fields["Parameters_Parent"]).HideInTable = true;
            ((ParentField)configDatabase.Views["Parameter"].Fields["Parameters_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;

            Category paramCategory = new Category() { Name = "General", Ordinal = 10 };
            configDatabase.Views["Parameter"].Fields["Name"].Category = paramCategory;
            configDatabase.Views["Parameter"].Fields["Value"].Category = paramCategory;
            configDatabase.Views["Parameter"].Fields["UseSqlParser"].Category = paramCategory;
            SetViewRoles(configDatabase.Views["Parameter"], "Admin,Developer");

            #endregion

            //Category hideCategory = new Category() { Name = "Hide", Ordinal = 30 };
            //fieldView.Fields["HideInEdit"].Category = hideCategory;
            //fieldView.Fields["HideInCreate"].Category = hideCategory;
            //fieldView.Fields["HideInTable"].Category = hideCategory;
            //fieldView.Fields["HideInFilter"].Category = hideCategory;

            #region FieldDescription
            foreach (View view in configDatabase.Views.Values)
            {
                foreach (Field field in view.Fields.Values)
                {
                    System.Reflection.PropertyInfo property = field.GetType().GetProperty(field.Name);
                    if (property != null)
                    {
                        object[] attributes = property.GetCustomAttributes(typeof(Durados.Config.Attributes.PropertyAttribute), true);
                        if (attributes.Length == 1)
                        {
                            string description = ((Durados.Config.Attributes.PropertyAttribute)attributes[0]).Description;
                            field.Description = description;
                        }
                    }
                }
            }
            #endregion

            foreach (View view in configDatabase.Views.Values)
                view.HideInMenu = false;

            //configDatabase.Views["Cron"].HideInMenu = true;


            View menuView = (View)configDatabase.Views["Menu"];

            menuView.Fields["Name"].Category = generalCategory;
            menuView.Fields["Ordinal"].Category = generalCategory;
            menuView.Fields["WorkspaceID"].Category = generalCategory;
            Category urlCategory = new Category() { Name = "Urls", Ordinal = 30 };
            menuView.Fields["UrlLinks_Children"].Category = urlCategory;
            menuView.PermanentFilter = "WorkspaceID <> 1";

        }

        public virtual void ConfigLocalization(Durados.Web.Mvc.Database localizationDatabase)
        {
            //localizationDatabase.Menu.Name = "Localization";
            localizationDatabase.DenyLocalizationConfigRoles = "Admin,User";

            localizationDatabase.Views["Durados_Language"].DisplayColumn = "Name";
            localizationDatabase.Views["Durados_Language"].DenyCreateRoles = localizationDatabase.DenyLocalizationConfigRoles;
            localizationDatabase.Views["Durados_Language"].DenyDeleteRoles = localizationDatabase.DenyLocalizationConfigRoles;
            localizationDatabase.Views["Durados_Language"].DenyEditRoles = localizationDatabase.DenyLocalizationConfigRoles;
            localizationDatabase.Views["Durados_Language"].DenySelectRoles = localizationDatabase.DenyLocalizationConfigRoles;
            localizationDatabase.Views["Durados_Language"].DisplayName = "Language";
            localizationDatabase.Views["Durados_Language"].HideInMenu = false;
            localizationDatabase.Views["Durados_Language"].GridEditable = false;
            ((Durados.Web.Mvc.View)localizationDatabase.Views["Durados_Language"]).ReloadPage = ReloadPage.Always;

            foreach (ChildrenField childrenField in localizationDatabase.Views["Durados_Language"].Fields.Values.Where(f => f.FieldType == FieldType.Children))
            {
                childrenField.ChildrenHtmlControlType = ChildrenHtmlControlType.Hide;
            }


            ((Durados.Web.Mvc.View)localizationDatabase.Views["Durados_Language"]).Controller = "Localization";

            localizationDatabase.Views["Durados_Language"].Fields["FK_Durados_Translation_Durados_Language_Children"].DisplayName = "Dictionary Keys";
            localizationDatabase.Views["Durados_Language"].Fields["NativeName"].DisplayName = "Native Name";
            ((ColumnField)localizationDatabase.Views["Durados_Language"].Fields["Direction"]).EnumType = typeof(Durados.Localization.LocalizationConfig.DirectionType).AssemblyQualifiedName;


            localizationDatabase.Views["Durados_TranslationKey"].DisplayColumn = "Key";
            localizationDatabase.Views["Durados_TranslationKey"].DenyCreateRoles = localizationDatabase.DenyLocalizationConfigRoles;
            localizationDatabase.Views["Durados_TranslationKey"].DenyDeleteRoles = localizationDatabase.DenyLocalizationConfigRoles;
            localizationDatabase.Views["Durados_TranslationKey"].DenyEditRoles = localizationDatabase.DenyLocalizationConfigRoles;
            localizationDatabase.Views["Durados_TranslationKey"].DenySelectRoles = localizationDatabase.DenyLocalizationConfigRoles;
            localizationDatabase.Views["Durados_TranslationKey"].DisplayName = "Keys";
            localizationDatabase.Views["Durados_TranslationKey"].HideInMenu = false;
            ((Durados.Web.Mvc.View)localizationDatabase.Views["Durados_TranslationKey"]).Controller = "Localization";

            foreach (ChildrenField childrenField in localizationDatabase.Views["Durados_TranslationKey"].Fields.Values.Where(f => f.FieldType == FieldType.Children))
            {
                childrenField.ChildrenHtmlControlType = ChildrenHtmlControlType.Hide;
            }

            Durados.Web.Mvc.View translationView = (Durados.Web.Mvc.View)localizationDatabase.Views["Durados_Translation"];
            translationView.DisplayColumn = "Translation";
            translationView.DenyCreateRoles = localizationDatabase.DenyLocalizationConfigRoles;
            translationView.DenyDeleteRoles = localizationDatabase.DenyLocalizationConfigRoles;
            translationView.DenyEditRoles = localizationDatabase.DenyLocalizationConfigRoles;
            translationView.DenySelectRoles = localizationDatabase.DenyLocalizationConfigRoles;
            translationView.DisplayName = "Translation";
            translationView.HideInMenu = false;
            translationView.Controller = "Localization";
            foreach (ChildrenField childrenField in localizationDatabase.Views["Durados_Translation"].Fields.Values.Where(f => f.FieldType == FieldType.Children))
            {
                childrenField.ChildrenHtmlControlType = ChildrenHtmlControlType.Hide;
            }

            ParentField translationKeyParent = (ParentField)translationView.Fields["FK_Durados_Translation_Durados_TranslationKey_Parent"];
            translationKeyParent.DisplayName = "Key";
            translationKeyParent.ParentHtmlControlType = ParentHtmlControlType.Autocomplete;
            translationKeyParent.AutocompleteFilter = true;

            translationView.Fields["FK_Durados_Translation_Durados_Language_Parent"].DisplayName = "Language";

            translationView.Fields["ID"].Order = 0;
            translationKeyParent.Order = 1;
            translationView.Fields["Translation"].Order = 2;

        }
    }

}
