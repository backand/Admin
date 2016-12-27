using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using System.Runtime.Caching;

namespace Durados.Web.Mvc
{
    public partial class View : Durados.View, Durados.Services.IStyleable
    {
        private Map map = null;
        public Map Map
        {
            get
            {
                if (map == null)
                    map = Maps.Instance.GetMap();
                return map;
            }
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true)]
        public string Controller { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string IndexAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string CheckListAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string SetLanguageAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string CreateAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string RefreshAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string CreateOnlyAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string EditAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DuplicateAction { get; set; }

        public string ViewDataSetID { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string EditRichAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string GetRichAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string EditOnlyAction { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string GetJsonViewAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string GetSelectListAction { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string InlineAddingDialogAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string InlineEditingDialogAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string InlineAddingCreateAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string InlineEditingEditAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string InlineDuplicateDialogAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string InlineDuplicateAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string InlineSearchDialogAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DeleteAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DeleteSelectionAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string EditSelectionAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string FilterAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AllFilterValuesAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string UploadAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ExportToCsvAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string PrintAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AutoCompleteAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AutoCompleteController { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AnotherRowLinkText { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AnotherRowLinkFunction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string JavaScripts { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string StyleSheets { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool ExportToCsv { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool ImportFromExcel { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Print { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool MultiSelect { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Default = DataRowView.Tabs)]
        public DataRowView DataRowView { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Popup { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool TabCache { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool RefreshOnClose { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "The name of the promote buttun in the dialog and the context menu")]
        public string PromoteButtonName { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "The tooltip of the promote buttun in the context menu")]
        public string PromoteButtonDescription { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string NewButtonName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string NewButtonDescription { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public string EditButtonName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string EditButtonDescription { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DuplicateButtonName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DuplicateButtonDescription { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AddItemsButtonName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AddItemsButtonDescription { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string InsertButtonName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DeleteButtonName { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true)]
        public bool HideToolbar { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool ShowUpDown { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "History notification distribution list")]
        public string HistoryNotifyList { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Max sub-grid height without scroll")]
        public int MaxSubGridHeight { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Ajax - peform partial refresh, Add - reload the entire page after Add, Edit - reload the entire page after Edit, Delete - reload the entire page after Delete, always- after Add, Edit or Delete")]
        public ReloadPage ReloadPage { get; set; }

        public string DataDashboardView { get; set; }


        public new Database Database
        {
            get
            {
                return (Database)base.Database;
            }
        }

        public View(DataTable dataTable, Database database)
            : this(dataTable, database, null)
        {
        }

        public View(DataTable dataTable, Durados.Web.Mvc.Database database, string name) :
            base(dataTable, database, name)
        {

            Controller = Database.DefaultController;
            IndexAction = Database.DefaultIndexAction;
            CheckListAction = Database.DefaultCheckListAction;
            CreateAction = Database.DefaultCreateAction;
            RefreshAction = Database.DefaultRefreshAction;
            CreateOnlyAction = Database.DefaultCreateOnlyAction;
            EditAction = Database.DefaultEditAction;
            DuplicateAction = Database.DefaultDuplicateAction;

            EditRichAction = Database.DefaultEditRichAction;
            GetRichAction = Database.DefaultGetRichAction;
            EditOnlyAction = Database.DefaultEditOnlyAction;
            GetJsonViewAction = Database.DefaultGetJsonViewAction;
            GetSelectListAction = Database.DefaultGetSelectListAction;

            DeleteAction = Database.DefaultDeleteAction;
            DeleteSelectionAction = Database.DefaultDeleteSelectionAction;
            EditSelectionAction = Database.DefaultEditSelectionAction;
            FilterAction = Database.DefaultFilterAction;
            UploadAction = Database.DefaultUploadAction;
            ExportToCsvAction = Database.DefaultExportToCsvAction;
            PrintAction = Database.DefaultPrintAction;
            SetLanguageAction = Database.DefaultSetLanguageAction;
            AutoCompleteAction = Database.DefaultAutoCompleteAction;
            AutoCompleteController = Database.DefaultAutoCompleteController;
            InlineAddingDialogAction = Database.DefaultInlineAddingDialogAction;
            InlineAddingCreateAction = Database.DefaultInlineAddingCreateAction;
            InlineEditingDialogAction = Database.DefaultInlineEditingDialogAction;
            InlineEditingEditAction = Database.DefaultInlineEditingEditAction;
            InlineDuplicateDialogAction = Database.DefaultInlineDuplicateDialogAction;
            InlineDuplicateAction = Database.DefaultInlineDuplicateAction;
            InlineSearchDialogAction = Database.DefaultInlineSearchDialogAction;
            AllFilterValuesAction = Database.DefaultAllFilterValuesAction;

            ExportToCsv = true;
            Print = false;
            DataRowView = DataRowView.Tabs;
            Popup = true;
            TabCache = true;
            RefreshOnClose = false;
            ImportFromExcel = true;
            DashboardHeight = "200";
            DashboardWidth = "400";
            PromoteButtonName = "Promote";
            NewButtonName = "New";
            EditButtonName = "Edit";
            DuplicateButtonName = "Duplicate";
            InsertButtonName = "Insert";
            DeleteButtonName = "Delete";

            HideToolbar = false;
            MultiSelect = true;
            MaxSubGridHeight = 400;

            DataDashboardView = "~/Views/Shared/Controls/DataDashboardView.ascx";

            OpenDialogMax = false;

        }

        protected override Durados.ColumnField CreateColumnField(DataColumn dataColumn)
        {
            return new Durados.Web.Mvc.ColumnField(this, dataColumn);
        }

        protected override Durados.ParentField CreateParentField(DataRelation dataRelation)
        {
            return new Durados.Web.Mvc.ParentField(this, dataRelation);
        }

        protected override Durados.ChildrenField CreateChildrenField(DataRelation dataRelation)
        {
            return new Durados.Web.Mvc.ChildrenField(this, dataRelation);
        }

        public object GetAutoCompleteValues(string fieldName, int limit, string q, bool api = false)
        {
            return Fields[fieldName].GetAutoCompleteValues(q, limit, api);
        }

        public override bool IsAllow()
        {
            return !UI.Helpers.SecurityHelper.IsDenied(DenySelectRoles, AllowSelectRoles) || UI.Helpers.SecurityHelper.IsConfigViewForViewOwner(this);
        }

        public override bool IsVisible()
        {
            return !this.HideInMenu && !UI.Helpers.SecurityHelper.IsDenied(DenySelectRoles, AllowSelectRoles);
        }

        public bool IsCreatable()
        {
            if (AllowCreate)
                return !UI.Helpers.SecurityHelper.IsDenied(DenyCreateRoles, AllowCreateRoles) || UI.Helpers.SecurityHelper.IsConfigViewForViewOwner(this);
            else
                return false;
        }

        public bool IsDuplicatable()
        {
            if (AllowDuplicate)
                return !UI.Helpers.SecurityHelper.IsDenied(DenyCreateRoles, AllowCreateRoles);
            else
                return false;
        }

        public bool IsDisabled(string guid)
        {
            return IsDisabled(guid, null);
        }

        public bool HasOpenRules
        {
            get
            {
                var openRules = Rules.Values.Where(r => r.DataAction == TriggerDataAction.Open);
                return openRules != null && openRules.Count() > 0;
            }
        }

        public virtual bool Check(Durados.TriggerDataAction dataAction, DataRow row)
        {
            if (row == null)
                return false;

            List<Rule> rules = Rules.Values.Where(r => r.DataAction == dataAction).ToList();
            int currentUserId = Convert.ToInt32(Database.GetUserID());
            foreach (Durados.Rule rule in rules)
            {
                if ((new Durados.Workflow.LogicalParser()).Check(rule.WhereCondition.Replace(currentUserId).Replace(row)))
                {
                    return true;
                }
            }

            return false;

        }

        public override string GetPermanentFilter()
        {
            string permanentFilter = base.GetPermanentFilter();
            if (string.IsNullOrEmpty(permanentFilter))
                return permanentFilter;
            if (permanentFilter.Contains(Database.UserPlaceHolder) || permanentFilter.ToLower().Contains(Database.UserPlaceHolder.ToLower()))
                permanentFilter = permanentFilter.Replace(Database.UserPlaceHolder, Database.GetUserID() ?? Database.NullInt, false);
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && !string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name) && (permanentFilter.Contains(Database.UsernamePlaceHolder) || permanentFilter.ToLower().Contains(Database.UsernamePlaceHolder.ToLower())))
                permanentFilter = permanentFilter.Replace(Database.UsernamePlaceHolder, System.Web.HttpContext.Current.User.Identity.Name ?? Database.NullInt, false);
            if (permanentFilter.Contains(Database.RolePlaceHolder) || permanentFilter.ToLower().Contains(Database.RolePlaceHolder.ToLower()))
                permanentFilter = permanentFilter.Replace(Database.RolePlaceHolder, Database.GetUserRole(), false);

            if (permanentFilter.Contains(Database.SysUserPlaceHolder.AsToken()) || permanentFilter.ToLower().Contains(Database.SysUserPlaceHolder.ToLower().AsToken()))
                permanentFilter = permanentFilter.Replace(Database.SysUserPlaceHolder.AsToken(), Database.GetUserID() ?? Database.NullInt, false);
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && !string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name) && (permanentFilter.Contains(Database.SysUsernamePlaceHolder.AsToken()) || permanentFilter.ToLower().Contains(Database.SysUsernamePlaceHolder.ToLower().AsToken())))
                permanentFilter = permanentFilter.Replace(Database.SysUsernamePlaceHolder.AsToken(), System.Web.HttpContext.Current.User.Identity.Name ?? Database.NullInt, false);

            if (permanentFilter.ToLower().Contains(Database.SysUsernamePlaceHolder.ToLower().AsToken()))
                permanentFilter = permanentFilter.Replace(Database.SysUsernamePlaceHolder.AsToken(), this.Database.GetCurrentUsername(), false);



            if (permanentFilter.Contains(Database.SysRolePlaceHolder.AsToken()) || permanentFilter.ToLower().Contains(Database.SysRolePlaceHolder.ToLower().AsToken()))
                permanentFilter = permanentFilter.Replace(Database.SysRolePlaceHolder.AsToken(), Database.GetUserRole(), false);

            //permanentFilter = permanentFilter.ReplaceGlobals(Database);
            permanentFilter = permanentFilter.ReplaceConfig(Database);

            permanentFilter = ReplaceQueryString(permanentFilter);
            permanentFilter = ReplaceQueryStringParameters(permanentFilter);


            return permanentFilter;
        }

        private string ReplaceQueryString(string permanentFilter)
        {
            foreach (string key in System.Web.HttpContext.Current.Request.QueryString.Keys)
            {
                if (!string.IsNullOrEmpty(key))
                    permanentFilter = permanentFilter.Replace(key.AsToken(), System.Web.HttpContext.Current.Request.QueryString[key], false);
            }

            return permanentFilter;
        }

        private string ReplaceQueryStringParameters(string permanentFilter)
        {
            if (System.Web.HttpContext.Current.Request.QueryString["parameters"] == null)
                return permanentFilter;

            string parameters = System.Web.HttpContext.Current.Request.QueryString["parameters"];
            Dictionary<string, object> values = new Dictionary<string,object>();
            try
            {
                //Dictionary<string, object> rulesParameters = RestHelper.Deserialize(this, System.Web.HttpContext.Current.Server.UrlDecode(parameters));
                values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(parameters);
            }
            catch(Exception ex)
            {
                string text;
                try
                {
                     text = Map.Database.Decrypt(parameters);
                }
                catch(Exception e)
                {
                    throw ex;
                }
                values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(text);
            }

            if (values != null)
            {
                foreach (string key in values.Keys)
                {
                    permanentFilter = permanentFilter.Replace(key.AsToken(), values[key].ToString(), false);
                }
            }

            return permanentFilter;
        }

        public bool IsDisabled(string guid, DataRow row)
        {
            if (Map.Session["disabled" + guid] == null)
            {
                bool isDisabled;
                if (row == null)
                {
                    isDisabled = false;
                }
                else
                {
                    isDisabled = Check(TriggerDataAction.Open, row);
                }
                Map.Session["disabled" + guid] = isDisabled;
                return isDisabled;
            }
            else
            {
                return (bool)Map.Session["disabled" + guid] == true;
            }
        }

        public bool IsEditable()
        {
            if (AllowEdit)
                return (!UI.Helpers.SecurityHelper.IsDenied(DenyEditRoles, AllowEditRoles) || IsViewOwner());
            else
                return false;
        }
        public bool IsEditable(string guid)
        {
            if (AllowEdit)
                return (!UI.Helpers.SecurityHelper.IsDenied(DenyEditRoles, AllowEditRoles) || IsViewOwner()) && !IsDisabled(guid);
            else
                return false;
        }

        public bool IsDeletable(string guid)
        {
            if (AllowDelete)
                return !UI.Helpers.SecurityHelper.IsDenied(DenyDeleteRoles, AllowDeleteRoles) && !IsDisabled(guid);
            else
                return false;
        }

        public bool IsDeletable()
        {
            if (AllowDelete)
                return !UI.Helpers.SecurityHelper.IsDenied(DenyDeleteRoles, AllowDeleteRoles);
            else
                return false;
        }

        public List<Field> VisibleFieldsForTable
        {
            get
            {
                return Fields.Values.Where(f => f.IsVisibleForTable()).OrderBy(f => f.Order).ToList();
            }
        }

        public List<Field> VisibleFieldsForTableFirstRow
        {
            get
            {
                return Fields.Values.Where(f => f.IsVisibleForTable()).Where(f => f.FieldType != FieldType.Children || !this.HasChildrenRow).OrderBy(f => f.Order).ToList();
            }
        }

        public List<Field> VisibleFieldsForTableChildrenRow
        {
            get
            {
                return Fields.Values.Where(f => f.IsVisibleForTable()).Where(f => f.FieldType == FieldType.Children && this.HasChildrenRow).OrderBy(f => f.Order).ToList();
            }
        }

        public List<Field> VisibleFieldsForFilter
        {
            get
            {
                return Fields.Values.Where(field => field.IsVisibleForFilter()).OrderBy(field => field.Order).ToList();
            }
        }

        /// <summary>
        /// Get visible fields for sorting- fields where Sortable=true
        /// </summary>
        public List<Field> VisibleFieldsForSorting
        {
            get
            {
                return Fields.Values.Where(field => field.IsVisibleForSort()).OrderBy(field => field.Order).ToList();
            }
        }

        /// <summary>
        /// Get visible fields for sorting- fields where Sortable=true
        /// </summary>
        public List<Field> VisibleFieldsForPreview
        {
            get
            {
                return VisibleFieldsForTableFirstRow.Where(field => field.Preview).OrderBy(field => field.Order).ToList();
            }
        }

        /// <summary>
        /// Get visible fields for tree filter: fields of type children or parent
        /// </summary>
        public List<Field> VisibleFieldsForTreeFilter
        {
            get
            {
                return VisibleFieldsForFilter.Where(field => field.FieldType != FieldType.Column).OrderBy(f => f.Order).ToList();
            }
        }

        public List<Field> VisibleFieldsForCreate
        {
            get
            {
                return Fields.Values.Where(f => f.IsVisibleForCreate()).OrderBy(f => f.GetOrder(DataAction.Create)).ToList();
            }
        }

        public List<Field> VisibleFieldsForReport
        {
            get
            {
                return Fields.Values.Where(f => !f.HideInCreate).OrderBy(f => f.GetOrder(DataAction.Create)).ToList();
            }
        }

        public List<Field> VisibleFieldsForEdit
        {
            get
            {
                return Fields.Values.Where(f => f.IsVisibleForEdit()).OrderBy(f => f.GetOrder(DataAction.Edit)).ToList();
            }
        }


        public override bool DoLocalize()
        {
            return ((Database)this.Database).Localization != null && ((Database)this.Database).IsMultiLanguages;
        }

        public string GetLocalizedDisplayName()
        {
            if (this.DoLocalize())
            {
                return Map.Database.Localizer.Translate(this.DisplayName);
            }
            else
            {
                return this.DisplayName;
            }
        }

        private MemoryCache fieldTypes = null;
        private MemoryCache modelFieldTypes = null;
        
        public Dictionary<string, object> GetFieldsTypes()
        {
            if (fieldTypes == null)
            {
                fieldTypes = GetFieldsTypesInner(false);
            }

            return (Dictionary<string, object>)fieldTypes[Name];
        }

        public Dictionary<string, object> GetModelFieldsTypes()
        {
            if (modelFieldTypes == null)
            {
                modelFieldTypes = GetFieldsTypesInner(true);
            }

            return (Dictionary<string, object>)modelFieldTypes[Name];
        }
        private MemoryCache GetFieldsTypesInner(bool model)
        {
            MemoryCache typesCache = new MemoryCache("fieldTypes_" + (model ? "model_" : "").ToString() + Database.GetCurrentAppName() + "_" + Name);

            Dictionary<string, object> types = new Dictionary<string, object>();

            foreach (Field field in GetVisibleFieldsForRow(DataAction.Edit).OrderBy(f => f.Order))
            {
                string type = field.GetRestType();
                Dictionary<string, object> values = new Dictionary<string, object>();

                if (model)
                {
                    if (field.Name == "id")
                    {
                        continue;
                    }
                    else if (field.Name == CreateDateColumnName || field.Name == ModifiedDateColumnName)
                    {
                        continue;
                    }
                }
                
                if (type == "object")
                {
                    string relatedObject = ((ParentField)field).ParentView.JsonName;
                    values.Add(type, relatedObject);
                    types.Add(field.JsonName, values);
                }
                else if (type == "collection")
                {
                    string relatedObject = ((ChildrenField)field).ChildrenView.JsonName;
                    string via = ((ChildrenField)field).GetRelatedParentField().JsonName;
                    values.Add(type, relatedObject);
                    values.Add("via", via);
                    if (!types.ContainsKey(field.JsonName))
                        types.Add(field.JsonName, values);
                }
                else
                {
                    if (field.IsAutoIdentity)
                    {
                        type = "int";
                    }
                    if (((ColumnField)field).IsAutoGuid)
                    {
                        type = "guid";
                    }
                    values.Add("type", type);
                    if (field.Required && !field.IsAutoIdentity)
                    {
                        values.Add("required", true);
                    }
                    if (field.Unique)
                    {
                        values.Add("unique", true);
                    }
                    if (model)
                    {
                        if (field.DefaultValue != null && field.DefaultValue != string.Empty && field.DefaultValue != System.DBNull.Value)
                        {
                            values.Add("defaultValue", field.DefaultValue);
                        }
                    }

                    if (types.ContainsKey(field.JsonName))
                    {
                        throw new DuplicateFieldException(field.JsonName,field.View.Name);
                    }

                    types.Add(field.JsonName, values);
                }
            }

            typesCache[Name] = types;
            return typesCache;
        }

        public List<Field> GetVisibleFieldsForRow(DataAction dataAction)
        {
            switch (dataAction)
            {
                case DataAction.Create:
                    return VisibleFieldsForCreate;
                case DataAction.Report:
                    return VisibleFieldsForReport;
                case DataAction.Edit:
                    return VisibleFieldsForEdit;
                case DataAction.InlineAdding:
                    return VisibleFieldsForCreate;
                case DataAction.InlineEditing:
                    return VisibleFieldsForEdit;
                default:
                    throw new NotSupportedException();
            }
        }

        public List<Field> GetVisibleFieldsForRow(DataAction dataAction, IEnumerable<Field> fields)
        {
            if (dataAction == DataAction.Create || dataAction == DataAction.InlineAdding)
            {
                return fields.Where(f => f.IsVisibleForCreate()).OrderBy(f => f.GetOrder(dataAction)).ToList();
            }
            else
            {
                return fields.Where(f => f.IsVisibleForEdit()).OrderBy(f => f.GetOrder(dataAction)).ToList();
            }
        }

        public bool IsInAdminMode()
        {
            return ((Database)Database).IgnoreInAdminMode && (((Database)Database).IsInRole("Developer") || ((Database)Database).IsInRole("Admin") || IsViewOwner());
        }

        public bool IsAdmin()
        {
            return (((Database)Database).IsInRole("Developer") || ((Database)Database).IsInRole("Admin") || IsViewOwner());
        }

        public bool IsViewOwner()
        {
            string userRole = Database.GetUserRole();
            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(ViewOwnerRoles))
                return false;

            string[] viewOwners = ViewOwnerRoles.Split(','); ;

            if (viewOwners.Length > 0 && viewOwners.Contains(Durados.Web.Mvc.Config.Project.ViewOwenrRole) && SecurityHelper.IsConfigViewForViewOwner(this))
            {
                return true;
            }



            foreach (string role in viewOwners.Where(r => !string.IsNullOrEmpty(r)))
                if (role.Equals(userRole))
                    return true;

            return false;
        }

        [Durados.Config.Attributes.ColumnProperty(Default = LayoutType.BasicDashboard)]
        public LayoutType Layout { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Default = SkinType.Skin6, Groups = "Skin")]
        public SkinType Skin { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Default = ThemeType.AdminLTE, Groups = "Theme")]
        public ThemeType Theme { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string CustomThemePath { get; set; }

        private bool applySkinToAllViews = false;
        [Durados.Config.Attributes.ColumnProperty(Description = "Apply the skin to all the views")]
        public bool ApplySkinToAllViews { get { return applySkinToAllViews; } set { applySkinToAllViews = false; } }

        public override IDbCommand GetCommand()
        {
            return this.GetNewCommand();
        }


        public string GetHiddenAttributes(DataRow row)
        {
            return GetHiddenAttributes(row, "d_");
        }

        public string GetHiddenAttributes(DataRow row, string prefix)
        {
            string s = string.Empty;

            foreach (Field field in Fields.Values.Where(f => f.HiddenAttribute))
            {
                string name = field.Name;
                string value = string.Empty;
                if (field.FieldType == FieldType.Parent)
                    value = System.Web.HttpContext.Current.Server.HtmlEncode(((ParentField)field).ConvertToString(row));
                else
                    value = System.Web.HttpContext.Current.Server.HtmlEncode(field.GetValue(row));

                s += prefix + name + "='" + value + "' ";
            }

            return s;
        }

        public override void SendRealTimeEvent(string pk, Crud crud)
        {
            Backand.socket socket = new Backand.socket();
            string eventName = crud.ToString() + "d";
            string appName = (System.Web.HttpContext.Current.Items[Durados.Database.AppName] ?? string.Empty).ToString();
            if (SendRealTimeEvents || Database.IsConfig)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(delegate
                    {
                        try
                        {
                            string data = string.Format("{{\"id\":\"{0}\", \"objectName\":\"{1}\", \"event\":\"{2}\"}}", pk, JsonName, eventName);
                            if (Database.IsConfig)
                            {
                                socket.emitRole(JsonName + "." + eventName, data, "Admin", appName);
                            }
                            else
                            {
                                socket.emitAll(JsonName + "." + eventName, data, appName);
                            }

                        }
                        catch (Exception exception)
                        {
                            Database.Logger.Log("view", "socket", "emit", exception, 1, crud.ToString());
                        }
                    });
            }
        }


        public override Dictionary<string, object> RowToShallowDictionary(DataRow row, string pk)
        {
            return (new DictionaryConverter()).RowToShallowDictionary(this, row, pk, false, true);
        }
    }


    public enum DataRowView
    {
        //Columns,
        Tabs,
        Groups,
        Divs,
        Accordion
    }

    public enum ReloadPage
    {
        Ajax,
        Add,
        Edit,
        Delete,
        Always
    }
    [Durados.Config.Attributes.EnumDisplay(EnumNames = new string[6] { "BasicGrid", "BasicDashboard", "BasicPreview", "GridGroupFilter", "DashboardVerticalShort","Custom" }, EnumDisplayNames = new string[6] { "<div title='Simple Grid' class='LayoutSelectorImages BasicGridImg'></div>", 
        "<div title='Cards' class='LayoutSelectorImages BasicDashboardImg'></div>","<div title='Preview' class='LayoutSelectorImages BasicPreviewImg'></div>","<div title='Grid and Filter' class='LayoutSelectorImages GridGroupFilterImg'></div>","<div title='Cards long Items' class='LayoutSelectorImages DashboardVerticalShortImg'></div>","<div title='Custom' class='LayoutSelectorImages CustomPage'></div>"})]
    public enum LayoutType
    {
        BasicGrid,//table,excel,excel thre checkbox false
        BasicDashboard,//Dashboard,group,group,200,400,false false false true
        BasicPreview,//Preview ,group,group,200,400,false false false true
        GridGroupFilter,//table ,group,group,200,400,false false false true
        DashboardVerticalShort,//Dashboard,group,group,400,200,false false false true
        Custom, //no settings
    }

    [Durados.Config.Attributes.EnumDisplay(EnumNames = new string[8] { "DefaultSkin", "Skin1", "Skin2", "Skin3", "Skin4", "skin5", "Skin6", "Transparent" }, EnumDisplayNames = new string[8] { "<div title='Gray Skin' class='SkinSelectorImages DefaultSkin'></div>", 
        "<div title='Red Skin' class='SkinSelectorImages Skin1'></div>","<div title='Green Skin' class='SkinSelectorImages Skin2'></div>","<div title='Blue Skin' class='SkinSelectorImages Skin3'></div>","<div title='Yellow Skin' class='SkinSelectorImages Skin4'></div>","<div title='Corporate Skin' class='SkinSelectorImages Skin5'></div>","<div title='Default Skin' class='SkinSelectorImages Skin6'></div>", "<div title='Transparent Skin' class='SkinSelectorImages Transparent'></div>"})]
    public enum SkinType
    {
        Skin6,
        DefaultSkin,
        Skin1, //Gray
        Skin2, //Blue+White
        Skin3, //red
        Skin4,
        skin5,
        Transparent,
    }

    [Durados.Config.Attributes.EnumDisplay(EnumNames = new string[4] { "AdminLTE", "DashGum", "DevOOPS", "Custom", }, EnumDisplayNames = new string[4] { "<div title='Admin LTE' class='ThemeSelectorImages AdminLTE'></div>", "<div title='DashGum' class='ThemeSelectorImages DashGum'></div>", "<div title='DevOOPS' class='ThemeSelectorImages DevOOPS'></div>", "<div title='Custom' class='ThemeSelectorImages Custom'></div>" })]
    public enum ThemeType
    {
        Custom = 1,
        AdminLTE = 2,
        DashGum = 3,
        DevOOPS = 4,
    }
}
