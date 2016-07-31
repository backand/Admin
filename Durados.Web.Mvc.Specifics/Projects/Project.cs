using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Durados.Web.Mvc.Specifics.Projects
{
    //public abstract class Project : Durados.Web.Mvc.Config.IProject
    //{
    //    public Map Map
    //    {
    //        get
    //        {
    //            return Maps.Instance.GetMap();
    //        }
    //    }

    //    public virtual DataSet GetDataSet()
    //    {
    //        return new DataSet();
    //    }


    //    public abstract string ConnectionStringKey
    //    {
    //        get;
    //    }

    //    public abstract string ConfigFileNameKey
    //    {
    //        get;
    //    }

    //    public virtual void ConfigConfig(Database configDatabase)
    //    {
    //        try
    //        {
    //            //configDatabase.Menu.Name = "Admin";
    //            //configDatabase.DenyConfigConfigRoles = "Manager,User";
    //            configDatabase.DisplayName = "Configuration";
    //            configDatabase.AllowConfigConfigRoles = "Developer,Admin";

    //            configDatabase.Views["Database"].DisplayColumn = "DisplayName";
    //            configDatabase.Views["Database"].AllowCreate = false;
    //            configDatabase.Views["Database"].AllowDelete = false;
    //            //configDatabase.Views["Database"].DenyCreateRoles = configDatabase.DenyConfigConfigRoles;
    //            //configDatabase.Views["Database"].DenyDeleteRoles = configDatabase.DenyConfigConfigRoles;
    //            //configDatabase.Views["Database"].DenyEditRoles = configDatabase.DenyConfigConfigRoles;
    //            //configDatabase.Views["Database"].DenySelectRoles = configDatabase.DenyConfigConfigRoles;
    //            /* TODO check if need to implement
    //            configDatabase.Views["Database"].AllowCreateRoles = configDatabase.DefaultAllowCreateRoles;
    //            configDatabase.Views["Database"].AllowEditRoles = configDatabase.DefaultAllowEditRoles;
    //            configDatabase.Views["Database"].AllowSelectRoles = configDatabase.DefaultAllowSelectRoles;
    //            */
    //            foreach (View view in configDatabase.Views.Values)
    //            {
    //                SetViewRoles(view, "Developer");
    //            }

    //            SetViewRoles(configDatabase.Views["View"], "Developer,Admin");
    //            SetViewRoles(configDatabase.Views["Field"], "Developer,Admin");
    //            SetViewRoles(configDatabase.Views["Workspace"], "Developer,Admin");
    //            SetViewRoles(configDatabase.Views["Category"], "Developer,Admin");

    //            configDatabase.Views["Database"].Fields["ID"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["ID"].DisplayName = "Id";
    //            configDatabase.Views["Database"].Fields["DefaultController"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultController"].DisplayName = "Default Controller";
    //            configDatabase.Views["Database"].Fields["DefaultIndexAction"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultIndexAction"].DisplayName = "Default Index Action";
    //            configDatabase.Views["Database"].Fields["DefaultCreateAction"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultCreateAction"].DisplayName = "Default Create Action";
    //            configDatabase.Views["Database"].Fields["DefaultEditAction"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultEditAction"].DisplayName = "Default Edit Action";
    //            configDatabase.Views["Database"].Fields["DefaultDeleteAction"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultDeleteAction"].DisplayName = "Default Delete Action";
    //            configDatabase.Views["Database"].Fields["DefaultFilterAction"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultFilterAction"].DisplayName = "Default Filter Action";
    //            configDatabase.Views["Database"].Fields["DefaultUploadAction"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultUploadAction"].DisplayName = "Default Upload Action";
    //            configDatabase.Views["Database"].Fields["DefaultGetJsonViewAction"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultGetJsonViewAction"].DisplayName = "Default GetJsonView Action";
    //            configDatabase.Views["Database"].Fields["DefaultAutoCompleteAction"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultAutoCompleteAction"].DisplayName = "Default AutoComplete Action";
    //            configDatabase.Views["Database"].Fields["DefaultAutoCompleteController"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultAutoCompleteController"].DisplayName = "Default AutoComplete Controller";
    //            configDatabase.Views["Database"].Fields["DefaultExportToCsvAction"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultExportToCsvAction"].DisplayName = "Default ExportToCsv Action";
    //            configDatabase.Views["Database"].Fields["DefaultPrintAction"].HideInTable = true;
    //            configDatabase.Views["Database"].Fields["DefaultPrintAction"].DisplayName = "Default Print Action";
    //            configDatabase.Views["Database"].Fields["DisplayName"].DisplayName = "Display Name";
    //            configDatabase.Views["Database"].Fields["ConnectionString"].DisplayName = "Connection String";
    //            configDatabase.Views["Database"].Fields["FirstViewName"].DisplayName = "First View Name";
    //            configDatabase.Views["Database"].Fields["DefaultPageSize"].DisplayName = "Default Page Size";
    //            configDatabase.Views["Database"].Fields["DateFormat"].DisplayName = "Date Format";
    //            //configDatabase.Views["Database"].Fields["MicrosoftDateFormat"].DisplayName = "Microsoft Date Format";

    //            if (configDatabase.Views.ContainsKey("SiteInfo") && configDatabase.Views["SiteInfo"].Fields.ContainsKey("Logo"))
    //                ((Durados.Web.Mvc.ColumnField)configDatabase.Views["SiteInfo"].Fields["Logo"]).Upload = new Upload() { Override = true, Title = "Logo", UploadFileType = UploadFileType.Other, UploadStorageType = UploadStorageType.File, UploadVirtualPath = "/Uploads/" };

    //            if (configDatabase.Views["Database"].Fields.ContainsKey("Localization_Parent"))
    //                configDatabase.Views["Database"].Fields["Localization_Parent"].DisplayName = "Localization";
    //            configDatabase.Views["Database"].Fields["Views_Children"].DisplayName = "Views";

    //            configDatabase.Views["Database"].Fields["DisplayName"].Order = 0;
    //            configDatabase.Views["Database"].Fields["ConnectionString"].Order = 1;
    //            configDatabase.Views["Database"].Fields["FirstViewName"].Order = 2;
    //            configDatabase.Views["Database"].Fields["DefaultPageSize"].Order = 3;
    //            configDatabase.Views["Database"].Fields["DateFormat"].Order = 4;
    //            //configDatabase.Views["Database"].Fields["MicrosoftDateFormat"].Order = 5;
    //            if (configDatabase.Views["Database"].Fields.ContainsKey("Localization_Parent"))
    //                configDatabase.Views["Database"].Fields["Localization_Parent"].Order = 6;
    //            configDatabase.Views["Database"].Fields["Views_Children"].Order = 7;

    //            if (configDatabase.Views.ContainsKey("Localization"))
    //            {
    //                configDatabase.Views["Localization"].Fields["ID"].HideInTable = true;
    //                configDatabase.Views["Localization"].Fields["ID"].DisplayName = "Id";
    //                if (configDatabase.Views["Localization"].Fields.ContainsKey("AddKeyIfMissing"))
    //                    configDatabase.Views["Localization"].Fields["AddKeyIfMissing"].DisplayName = "Add Key If Missing";
    //                if (configDatabase.Views["Localization"].Fields.ContainsKey("ReturnKeyIfMissing"))
    //                    configDatabase.Views["Localization"].Fields["ReturnKeyIfMissing"].DisplayName = "Return Key If Missing";
    //                if (configDatabase.Views["Localization"].Fields.ContainsKey("Prefix"))
    //                    configDatabase.Views["Localization"].Fields["Prefix"].DisplayName = "Prefix";
    //                if (configDatabase.Views["Localization"].Fields.ContainsKey("Postfix"))
    //                    configDatabase.Views["Localization"].Fields["Postfix"].DisplayName = "Postfix";
    //                if (configDatabase.Views["Localization"].Fields.ContainsKey("Title"))
    //                    configDatabase.Views["Localization"].Fields["Title"].DisplayName = "Title";
    //                if (configDatabase.Views["Localization"].Fields.ContainsKey("Localization_Children"))
    //                    configDatabase.Views["Localization"].Fields["Localization_Children"].DisplayName = "Database";
    //            }

    //            //View
    //            (configDatabase.Views["View"] as Durados.Web.Mvc.View).Popup = true;
    //            configDatabase.Views["View"].DisplayName = "Views";
    //            configDatabase.Views["View"].DisplayColumn = "Name";
    //            configDatabase.Views["View"].DefaultSort = "Name asc";
    //            configDatabase.Views["View"].AllowCreate = true;
    //            configDatabase.Views["View"].AllowDelete = true;
    //            configDatabase.Views["View"].AllowDuplicate = false;
    //            configDatabase.Views["View"].UseOrderForEdit = true;
    //            configDatabase.Views["View"].AllowSelectRoles = configDatabase.DefaultAllowSelectRoles;
    //            configDatabase.Views["View"].AllowCreateRoles = configDatabase.DefaultAllowCreateRoles;
    //            configDatabase.Views["View"].AllowEditRoles = configDatabase.DefaultAllowEditRoles;
    //            configDatabase.Views["View"].AllowDeleteRoles = configDatabase.DefaultAllowDeleteRoles;
    //            ((View)configDatabase.Views["View"]).MultiSelect = true;
    //            configDatabase.Views["View"].GridEditable = true;
    //            configDatabase.Views["View"].ColumnsInDialogPerCategory = "Display;2;Behaviour;2;Description;1;Email;1;Advanced;3;Permissions;1;Developers;3;System;3";
    //            configDatabase.Views["View"].AddItemsFieldName = "Menu_Parent";
    //            configDatabase.Views["View"].PermanentFilter = "SystemView = 0";
            
    //            configDatabase.Views["View"].OrdinalColumnName = "DisplayName";
    //            configDatabase.Views["View"].Fields["ID"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ID"].DisplayName = "Id";
    //            configDatabase.Views["View"].Fields["Controller"].DenyEditRoles = "User";
    //            configDatabase.Views["View"].Fields["Controller"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["Controller"].DisplayName = "Controller";
    //            configDatabase.Views["View"].Fields["IndexAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["IndexAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["IndexAction"].DisplayName = "Index Action";
    //            configDatabase.Views["View"].Fields["SetLanguageAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["SetLanguageAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["SetLanguageAction"].DisplayName = "Set Language Action";
    //            configDatabase.Views["View"].Fields["CreateAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["CreateAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["CreateAction"].DisplayName = "Create Action";
    //            configDatabase.Views["View"].Fields["EditAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["EditAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["EditAction"].DisplayName = "Edit Action";
    //            configDatabase.Views["View"].Fields["GetJsonViewAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["GetJsonViewAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["GetJsonViewAction"].DisplayName = "Get JsonView Action";
    //            configDatabase.Views["View"].Fields["DeleteAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["DeleteAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DeleteAction"].DisplayName = "Delete Action";
    //            configDatabase.Views["View"].Fields["FilterAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["FilterAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["FilterAction"].DisplayName = "Filter Action";
    //            configDatabase.Views["View"].Fields["UploadAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["UploadAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["UploadAction"].DisplayName = "Upload Action";
    //            configDatabase.Views["View"].Fields["ExportToCsvAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["ExportToCsvAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ExportToCsvAction"].DisplayName = "ExportToCsv Action";
    //            configDatabase.Views["View"].Fields["PrintAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["PrintAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["PrintAction"].DisplayName = "Print Action";
    //            configDatabase.Views["View"].Fields["AutoCompleteAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["AutoCompleteAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AutoCompleteAction"].DisplayName = "Auto Complete Action";
    //            configDatabase.Views["View"].Fields["AutoCompleteController"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["AutoCompleteController"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AutoCompleteController"].DisplayName = "Auto Complete Controller";
    //            configDatabase.Views["View"].Fields["InlineAddingDialogAction"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["InlineAddingDialogAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["InlineAddingDialogAction"].DisplayName = "Inline Adding Dialog Action";

    //            configDatabase.Views["View"].Fields["InlineEditingDialogAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["InlineEditingDialogAction"].DisplayName = "Inline Editing Dialog Action";
    //            configDatabase.Views["View"].Fields["InlineEditingEditAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["InlineEditingEditAction"].DisplayName = "Inline Editing Edit Action";
    //            configDatabase.Views["View"].Fields["ContainerGraphicProperties"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ContainerGraphicProperties"].DisplayName = "Container Graphic";

    //            configDatabase.Views["View"].Fields["Description"].ColSpanInDialog = 2;
    //            configDatabase.Views["View"].Fields["Description"].DisplayName = "Description / Tooltip";
    //            (configDatabase.Views["View"].Fields["Description"] as ColumnField).TextHtmlControlType = TextHtmlControlType.TextArea;
    //            (configDatabase.Views["View"].Fields["Description"] as ColumnField).CssClass = "exwtextareawide";
    //            (configDatabase.Views["View"].Fields["Description"] as ColumnField).Rich = true;
    //            (configDatabase.Views["View"].Fields["Description"] as ColumnField).PartialLength = 20;

    //            configDatabase.Views["View"].Fields["PromoteButtonDescription"].ColSpanInDialog = 2;
    //            ((ColumnField)configDatabase.Views["View"].Fields["PromoteButtonDescription"]).TextHtmlControlType = TextHtmlControlType.TextArea;
    //            ((ColumnField)configDatabase.Views["View"].Fields["PromoteButtonDescription"]).CssClass = "exwtextareawide";
    //            configDatabase.Views["View"].Fields["NewButtonDescription"].ColSpanInDialog = 2;
    //            ((ColumnField)configDatabase.Views["View"].Fields["NewButtonDescription"]).TextHtmlControlType = TextHtmlControlType.TextArea;
    //            ((ColumnField)configDatabase.Views["View"].Fields["NewButtonDescription"]).CssClass = "exwtextareawide";
    //            configDatabase.Views["View"].Fields["EditButtonDescription"].ColSpanInDialog = 2;
    //            ((ColumnField)configDatabase.Views["View"].Fields["EditButtonDescription"]).TextHtmlControlType = TextHtmlControlType.TextArea;
    //            ((ColumnField)configDatabase.Views["View"].Fields["EditButtonDescription"]).CssClass = "exwtextareawide";
    //            configDatabase.Views["View"].Fields["DuplicateButtonDescription"].ColSpanInDialog = 2;
    //            ((ColumnField)configDatabase.Views["View"].Fields["DuplicateButtonDescription"]).TextHtmlControlType = TextHtmlControlType.TextArea;
    //            ((ColumnField)configDatabase.Views["View"].Fields["DuplicateButtonDescription"]).CssClass = "exwtextareawide";


    //            configDatabase.Views["View"].Fields["DuplicateAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DuplicateAction"].DisplayName = "Duplicate Action";
    //            configDatabase.Views["View"].Fields["InlineDuplicateDialogAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["InlineDuplicateDialogAction"].DisplayName = "Inline Duplicate Dialog Action";
    //            configDatabase.Views["View"].Fields["InlineDuplicateAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["InlineDuplicateAction"].DisplayName = "Inline Duplicate Action";
    //            configDatabase.Views["View"].Fields["InlineSearchDialogAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["InlineSearchDialogAction"].DisplayName = "Inline Search Dialog Action";
    //            configDatabase.Views["View"].Fields["EditOnlyAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["EditOnlyAction"].DisplayName = "Edit Only Action";
    //            configDatabase.Views["View"].Fields["InlineAddingCreateAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["InlineAddingCreateAction"].DisplayName = "Inline Adding Create Action";

    //            configDatabase.Views["View"].Fields["CreateOnlyAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["CreateOnlyAction"].DisplayName = "Create Only Action";
    //            configDatabase.Views["View"].Fields["EditRichAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["EditRichAction"].DisplayName = "Edit Rich Action";
    //            configDatabase.Views["View"].Fields["GetRichAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["GetRichAction"].DisplayName = "Get Rich Action";
    //            configDatabase.Views["View"].Fields["GetSelectListAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["GetSelectListAction"].DisplayName = "Get Select List Action";
    //            configDatabase.Views["View"].Fields["EditSelectionAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["EditSelectionAction"].DisplayName = "Edit Selection Action";
    //            configDatabase.Views["View"].Fields["ExportToCsv"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ExportToCsv"].DisplayName = "Export To Excel";
    //            configDatabase.Views["View"].Fields["Print"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["Print"].DisplayName = "Print";
    //            configDatabase.Views["View"].Fields["PageSize"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["PageSize"].DisplayName = "Page Size";

    //            configDatabase.Views["View"].Fields["ColumnsInDialog"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ColumnsInDialog"].DisplayName = "Columns In Dialog";
    //            configDatabase.Views["View"].Fields["AllowCreate"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AllowCreate"].DisplayName = "Allow Create";
    //            configDatabase.Views["View"].Fields["AllowEdit"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AllowEdit"].DisplayName = "Allow Edit";
    //            configDatabase.Views["View"].Fields["AllowDelete"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AllowDelete"].DisplayName = "Allow Delete";
    //            configDatabase.Views["View"].Fields["DenyCreateRoles"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DenyCreateRoles"].DisplayName = "Deny Create (Roles)";
    //            configDatabase.Views["View"].Fields["DenyEditRoles"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DenyEditRoles"].DisplayName = "Deny Edit (Roles)";
    //            configDatabase.Views["View"].Fields["DenyDeleteRoles"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DenyDeleteRoles"].DisplayName = "Deny Delete (Roles)";
    //            configDatabase.Views["View"].Fields["DenySelectRoles"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DenySelectRoles"].DisplayName = "Deny Read (Roles)";
    //            configDatabase.Views["View"].Fields["AllowCreateRoles"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AllowCreateRoles"].DisplayName = "Allow Create (Roles)";
    //            configDatabase.Views["View"].Fields["AllowEditRoles"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AllowEditRoles"].DisplayName = "Allow Edit (Roles)";
    //            configDatabase.Views["View"].Fields["AllowSelectRoles"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AllowSelectRoles"].DisplayName = "Allow Read (Roles)";
    //            configDatabase.Views["View"].Fields["AllowDeleteRoles"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AllowDeleteRoles"].DisplayName = "Allow Delete (Roles)";

    //            //configDatabase.Views["View"].Fields["Name"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["Name"].DisplayName = "Name";
    //            configDatabase.Views["View"].Fields["Name"].DisableInEdit = true;
    //            //configDatabase.Views["View"].Fields["DisplayName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DisplayName"].DisplayName = "Display Name";
    //            //configDatabase.Views["View"].Fields["Order"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["Order"].DisplayName = "Order";
    //            //configDatabase.Views["View"].Fields["DisplayColumn"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DisplayColumn"].DisplayName = "Display Column";
    //            configDatabase.Views["View"].Fields["Views_Parent"].DisplayName = "Database";
    //            configDatabase.Views["View"].Fields["Views_Parent"].DenyEditRoles = "Admin, User";
    //            configDatabase.Views["View"].Fields["Views_Parent"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["Views_Parent"].DisableInEdit = true;
    //            configDatabase.Views["View"].Fields["Fields_Children"].DisplayName = "Fields";
    //            configDatabase.Views["View"].Fields["Fields_Children"].HideInTable = false;

    //            configDatabase.Views["View"].Fields["HideInMenu"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["HideInMenu"].DisplayName = "Hide Menu?";

    //            configDatabase.Views["View"].Fields["DeleteSelectionAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DeleteSelectionAction"].DisplayName = "Delete Selection Action";
    //            configDatabase.Views["View"].Fields["MultiSelect"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["MultiSelect"].DisplayName = "Multi Select?";
    //            configDatabase.Views["View"].Fields["HasChildrenRow"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["HasChildrenRow"].DisplayName = "Has Children Row?";
    //            configDatabase.Views["View"].Fields["UseOrderForCreate"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["UseOrderForCreate"].DisplayName = "Use Order For Create?";
    //            configDatabase.Views["View"].Fields["UseOrderForEdit"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["UseOrderForEdit"].DisplayName = "Use Order For Edit?";
    //            configDatabase.Views["View"].Fields["BaseViewName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["BaseViewName"].DisplayName = "Base View Name";
    //            configDatabase.Views["View"].Fields["Menu_Parent"].DisplayName = "Menu";
    //            //configDatabase.Views["View"].Fields["Menu_Parent"].HideInTable = true;

    //            configDatabase.Views["View"].Fields["NotifyMessageKey"].DisplayName = "Notification Message Key";
    //            configDatabase.Views["View"].Fields["NotifyMessageKey"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["NotifySubjectKey"].DisplayName = "Notification Subject Key";
    //            configDatabase.Views["View"].Fields["NotifySubjectKey"].HideInTable = true;

    //            configDatabase.Views["View"].Fields["DuplicateMessage"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DuplicateMessage"].DisplayName = "Duplicate Message";
    //            configDatabase.Views["View"].Fields["BaseTableName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["BaseTableName"].DisplayName = "Counter Base Table Name";
    //            configDatabase.Views["View"].Fields["Derivation_Parent"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["Derivation_Parent"].DisplayName = "Derivation Parent";
    //            configDatabase.Views["View"].Fields["ChartInfo_Parent"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ChartInfo_Parent"].DisplayName = "ChartInfo Parent";

    //            configDatabase.Views["View"].Fields["AnotherRowLinkText"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AnotherRowLinkText"].DisplayName = "Another Row Link Text";
    //            configDatabase.Views["View"].Fields["AnotherRowLinkFunction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AnotherRowLinkFunction"].DisplayName = "Another Row Link Function";
    //            configDatabase.Views["View"].Fields["JavaScripts"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["JavaScripts"].DisplayName = "JavaScripts";
    //            configDatabase.Views["View"].Fields["IsImageTable"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["IsImageTable"].DisplayName = "Is Image Table?";

    //            configDatabase.Views["View"].Fields["RefreshAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["RefreshAction"].DisplayName = "Refresh Action";
    //            configDatabase.Views["View"].Fields["Popup"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["Popup"].DisplayName = "Open as Popup?";
    //            configDatabase.Views["View"].Fields["TabCache"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["TabCache"].DisplayName = "Cache the Tab?";
    //            configDatabase.Views["View"].Fields["RefreshOnClose"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["RefreshOnClose"].DisplayName = "Refresh On Close?";
    //            configDatabase.Views["View"].Fields["HideFilter"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["HideFilter"].DisplayName = "Hide Filter?";
    //            configDatabase.Views["View"].Fields["HidePager"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["HidePager"].DisplayName = "Hide Pager?";
    //            configDatabase.Views["View"].Fields["CheckListAction"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["CheckListAction"].DisplayName = "CheckList Action";

    //            configDatabase.Views["View"].Fields["StyleSheets"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ImportFromExcel"].HideInTable = true;

    //            configDatabase.Views["View"].Fields["PromoteButtonName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["NewButtonName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["EditButtonName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DuplicateButtonName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["InsertButtonName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DeleteButtonName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["HideToolbar"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ShowUpDown"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["HistoryNotifyList"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ColumnsInDialogPerCategory"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AllowDuplicate"].HideInTable = true;
    //            //configDatabase.Views["View"].Fields["PermanentFilter"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["HideSearch"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["RowColorColumnName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["GroupingFields"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["EditableTableName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["SaveHistory"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["CreateDateColumnName"].HideInTable = true;

    //            configDatabase.Views["View"].Fields["ModifiedDateColumnName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["CreatedByColumnName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ModifiedByColumnName"].HideInTable = true;

    //            configDatabase.Views["View"].Fields["OrdinalColumnName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["OrdinalColumnName"].DisplayName = "Ordinal Column Name";
    //            configDatabase.Views["View"].Fields["ImageSrcColumnName"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["ImageSrcColumnName"].DisplayName = "Image Src Column Name";
    //            //configDatabase.Views["View"].Fields["DefaultSort"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DefaultSort"].DisplayName = "Default Sort Column";
    //            configDatabase.Views["View"].Fields["DataRowView"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DataRowView"].DisplayName = "Row View";
    //            ((ColumnField)configDatabase.Views["View"].Fields["DataRowView"]).EnumType = typeof(DataRowView).AssemblyQualifiedName;
    //            configDatabase.Views["View"].Fields["DisplayType"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DisplayType"].DisplayName = "Display Type";
    //            ((ColumnField)configDatabase.Views["View"].Fields["DisplayType"]).EnumType = typeof(DisplayType).AssemblyQualifiedName;
    //            configDatabase.Views["View"].Fields["DuplicationMethod"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DuplicationMethod"].DisplayName = "Duplication Method";
    //            ((ColumnField)configDatabase.Views["View"].Fields["DuplicationMethod"]).EnumType = typeof(DuplicationMethod).AssemblyQualifiedName;
    //            configDatabase.Views["View"].Fields["PromoteButtonDescription"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["NewButtonDescription"].HideInTable = true;
    //            if (configDatabase.Views["View"].Fields.ContainsKey("ShowDisabledSteps"))
    //                configDatabase.Views["View"].Fields["ShowDisabledSteps"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["EditButtonDescription"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["DuplicateButtonDescription"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["WorkspaceID"].EditInTableView = true;
    //            if (configDatabase.Views["View"].Fields.ContainsKey("Cached"))
    //            {
    //                configDatabase.Views["View"].Fields["Cached"].HideInTable = false;
    //                configDatabase.Views["View"].Fields["Cached"].DisplayName = "Cache in Memory";
    //            }
    //            configDatabase.Views["View"].Fields["MaxSubGridHeight"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["AddItemsFieldName"].HideInTable = true;


    //            configDatabase.Views["View"].Fields["Send"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["Send"].DisplayName = "Send Email By user?";
    //            configDatabase.Views["View"].Fields["SendTo"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["SendTo"].DisplayName = "To";
    //            configDatabase.Views["View"].Fields["SendCc"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["SendCc"].DisplayName = "CC";
    //            configDatabase.Views["View"].Fields["SendSubject"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["SendSubject"].DisplayName = "Subject";
    //            configDatabase.Views["View"].Fields["SendTemplate"].HideInTable = true;
    //            configDatabase.Views["View"].Fields["SendTemplate"].DisplayName = "Template";
                //configDatabase.Views["View"].Fields["OrdinalColumnName"].HideInTable = true;
                //configDatabase.Views["View"].Fields["OrdinalColumnName"].DisplayName = "Ordinal Column Name";
                //configDatabase.Views["View"].Fields["ImageSrcColumnName"].HideInTable = true;
                //configDatabase.Views["View"].Fields["ImageSrcColumnName"].DisplayName = "Image Src Column Name";
                ////configDatabase.Views["View"].Fields["DefaultSort"].HideInTable = true;
                //configDatabase.Views["View"].Fields["DefaultSort"].DisplayName = "Default Sort Column";
                //configDatabase.Views["View"].Fields["DataRowView"].HideInTable = true;
                //configDatabase.Views["View"].Fields["DataRowView"].DisplayName = "Row View";
                //((ColumnField)configDatabase.Views["View"].Fields["DataRowView"]).EnumType = typeof(DataRowView).AssemblyQualifiedName;
                //configDatabase.Views["View"].Fields["DisplayType"].HideInTable = true;
                //configDatabase.Views["View"].Fields["DisplayType"].DisplayName = "Display Type";
                //((ColumnField)configDatabase.Views["View"].Fields["DisplayType"]).EnumType = typeof(DisplayType).AssemblyQualifiedName;
                //configDatabase.Views["View"].Fields["DuplicationMethod"].HideInTable = true;
                //configDatabase.Views["View"].Fields["DuplicationMethod"].DisplayName = "Duplication Method";
                //((ColumnField)configDatabase.Views["View"].Fields["DuplicationMethod"]).EnumType = typeof(DuplicationMethod).AssemblyQualifiedName;
                //((ParentField)configDatabase.Views["Field"].Fields["Fields_Parent"]).EditInTableView = true;
                //configDatabase.Views["View"].Fields["PromoteButtonDescription"].HideInTable = true;
                //configDatabase.Views["View"].Fields["NewButtonDescription"].HideInTable = true;
                //if (configDatabase.Views["View"].Fields.ContainsKey("ShowDisabledSteps"))
                //    configDatabase.Views["View"].Fields["ShowDisabledSteps"].HideInTable = true;
                //configDatabase.Views["View"].Fields["EditButtonDescription"].HideInTable = true;
                //configDatabase.Views["View"].Fields["DuplicateButtonDescription"].HideInTable = true;
                //configDatabase.Views["View"].Fields["WorkspaceID"].EditInTableView = true;
                //if (configDatabase.Views["View"].Fields.ContainsKey("Cached"))
                //{
                //    configDatabase.Views["View"].Fields["Cached"].HideInTable = false;
                //    configDatabase.Views["View"].Fields["Cached"].DisplayName = "Cache in Memory";
                //}
                //configDatabase.Views["View"].Fields["MaxSubGridHeight"].HideInTable = true;
                //configDatabase.Views["View"].Fields["AddItemsFieldName"].HideInTable = true;

                //configDatabase.Views["View"].Fields["NotifyMessageKey"].HideInTable = true;
                //configDatabase.Views["View"].Fields["NotifySubjectKey"].HideInTable = true;
               
            
                //configDatabase.Views["View"].Fields["Send"].HideInTable = true;    
                //configDatabase.Views["View"].Fields["Send"].DisplayName = "Send Email By user?";
                //configDatabase.Views["View"].Fields["SendTo"].HideInTable = true;
                //configDatabase.Views["View"].Fields["SendTo"].DisplayName = "To";
                //configDatabase.Views["View"].Fields["SendCc"].HideInTable = true;
                //configDatabase.Views["View"].Fields["SendCc"].DisplayName = "CC";
                //configDatabase.Views["View"].Fields["SendSubject"].HideInTable = true;
                //configDatabase.Views["View"].Fields["SendSubject"].DisplayName = "Subject";
                //configDatabase.Views["View"].Fields["SendTemplate"].HideInTable = true;
                //configDatabase.Views["View"].Fields["SendTemplate"].DisplayName = "Template";
                
    //            configDatabase.Views["View"].Fields["Fields_Children"].Order = -10;
    //            configDatabase.Views["View"].Fields["Name"].Order = 10;
    //            configDatabase.Views["View"].Fields["Name"].OrderForEdit = 10;
    //            configDatabase.Views["View"].Fields["DisplayName"].Order = 20;
    //            configDatabase.Views["View"].Fields["DisplayName"].OrderForEdit = 10;
    //            configDatabase.Views["View"].Fields["DisplayColumn"].Order = 30;
    //            configDatabase.Views["View"].Fields["BaseName"].Order = 35;
    //            configDatabase.Views["View"].Fields["WorkspaceID"].Order = 5;
    //            configDatabase.Views["View"].Fields["WorkspaceID"].OrderForEdit = 30;
    //            configDatabase.Views["View"].Fields["Description"].Order = 40;
    //            configDatabase.Views["View"].Fields["Description"].OrderForEdit = 40;
    //            configDatabase.Views["View"].Fields["Order"].Order = 70;
    //            configDatabase.Views["View"].Fields["DefaultSort"].Order = 80;
    //            configDatabase.Views["View"].Fields["PermanentFilter"].Order = 90;
    //            configDatabase.Views["View"].Fields["Cached"].Order = 100;

    //            if (configDatabase.Views["Derivation"].Fields.ContainsKey("Deriveds"))
    //                ((ColumnField)configDatabase.Views["Derivation"].Fields["Deriveds"]).TextHtmlControlType = TextHtmlControlType.TextArea;

    //            //Fields

    //            //hide all the properties of the field in create
    //            foreach (Field field in configDatabase.Views["Field"].Fields.Values)
    //            {
    //                field.HideInCreate = true;
    //            }

    //            (configDatabase.Views["Field"] as Durados.Web.Mvc.View).Popup = true;
    //            configDatabase.Views["Field"].GridEditable = true;
    //            configDatabase.Views["Field"].DisplayName = "Fields";

    //            configDatabase.Views["Field"].DisplayColumn = "Name";
    //            configDatabase.Views["Field"].AllowCreate = true;
    //            configDatabase.Views["Field"].AllowDelete = false;
    //            configDatabase.Views["Field"].AllowDuplicate = false;
    //            configDatabase.Views["Field"].UseOrderForEdit = true;
    //            configDatabase.Views["Field"].UseOrderForCreate = true;
    //            configDatabase.Views["Field"].PageSize = 200;
    //            ((View)configDatabase.Views["Field"]).MultiSelect = true;
    //            configDatabase.Views["Field"].ColumnsInDialogPerCategory = "Display;2;Behaviour;2;Advanced;3;Permissions;1;Developers;3;System;3";


    //            configDatabase.Views["Field"].Fields["ID"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["ID"].DisplayName = "Id";
    //            configDatabase.Views["Field"].Fields["ParentHtmlControlType"].HideInTable = true;
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("ParentHtmlControlType"))
    //                configDatabase.Views["Field"].Fields["ParentHtmlControlType"].DisplayName = "Select Type";
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("ChildrenHtmlControlType"))
    //            {
    //                configDatabase.Views["Field"].Fields["ChildrenHtmlControlType"].DisplayName = "Children View";
    //                configDatabase.Views["Field"].Fields["ChildrenHtmlControlType"].HideInTable = true;
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("AutocompleteMathing"))
    //            {
    //                configDatabase.Views["Field"].Fields["AutocompleteMathing"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["AutocompleteMathing"].DisplayName = "Must Match Autocomplete";
    //                ((ColumnField)configDatabase.Views["Field"].Fields["AutocompleteMathing"]).EnumType = typeof(AutocompleteMathing).AssemblyQualifiedName;
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("SpecialColumn"))
    //            {
    //                configDatabase.Views["Field"].Fields["SpecialColumn"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["SpecialColumn"].DisplayName = "Special Column";
    //                ((ColumnField)configDatabase.Views["Field"].Fields["SpecialColumn"]).EnumType = typeof(SpecialColumn).AssemblyQualifiedName;
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("LimitToStartAutocomplete"))
    //            {
    //                configDatabase.Views["Field"].Fields["LimitToStartAutocomplete"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["LimitToStartAutocomplete"].DisplayName = "Limit To Start Autocomplete";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("Localize"))
    //            {
    //                configDatabase.Views["Field"].Fields["Localize"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["Localize"].DisplayName = "Localize";
    //            }

    //            //configDatabase.Views["Field"].Fields["InlineEditing"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["InlineEditing"].DisplayName = "Inline Editing";
    //            //configDatabase.Views["Field"].Fields["Name"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Name"].DisplayName = "Name";
    //            configDatabase.Views["Field"].Fields["Name"].DisableInEdit = true;
    //            configDatabase.Views["Field"].Fields["FieldType"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["FieldType"].DisplayName = "Field Type";
    //            //configDatabase.Views["Field"].Fields["DisplayName"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["DisplayName"].DisplayName = "Display Name";
    //            configDatabase.Views["Field"].Fields["DisplayName"].HideInCreate = false;
    //            configDatabase.Views["Field"].Fields["NullString"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["NullString"].DisplayName = "Null String";
    //            //configDatabase.Views["Field"].Fields["HideInTable"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["HideInTable"].DisplayName = "Hide In View";
    //            //configDatabase.Views["Field"].Fields["HideInEdit"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["HideInEdit"].DisplayName = "Hide In Edit";
    //            //configDatabase.Views["Field"].Fields["HideInCreate"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["HideInCreate"].DisplayName = "Hide In Create";
    //            //configDatabase.Views["Field"].Fields["DisableInEdit"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["DisableInEdit"].DisplayName = "Disable In Edit";
    //            //configDatabase.Views["Field"].Fields["DisableInCreate"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["DisableInCreate"].DisplayName = "Disable In Create";
    //            configDatabase.Views["Field"].Fields["HideInFilter"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["HideInFilter"].DisplayName = "Hide In Filter";
    //            //configDatabase.Views["Field"].Fields["Order"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Order"].DisplayName = "Order";
    //            configDatabase.Views["Field"].Fields["OrderForCreate"].DisplayName = "Order For Create";
    //            configDatabase.Views["Field"].Fields["OrderForEdit"].DisplayName = "Order For Edit";
    //            configDatabase.Views["Field"].Fields["Sortable"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Sortable"].DisplayName = "Sortable";
    //            //configDatabase.Views["Field"].Fields["DefaultValue"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["DefaultValue"].DisplayName = "Default Value";
    //            configDatabase.Views["Field"].Fields["ColSpanInDialog"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["ColSpanInDialog"].DisplayName = "Column Span In Dialog";
    //            configDatabase.Views["Field"].Fields["DenyCreateRoles"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["DenyCreateRoles"].DisplayName = "Deny Create (Roles)";
    //            configDatabase.Views["Field"].Fields["DenyEditRoles"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["DenyEditRoles"].DisplayName = "Deny Edit (Roles)";
    //            configDatabase.Views["Field"].Fields["DenySelectRoles"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["DenySelectRoles"].DisplayName = "Deny Read (Roles)";

    //            if (configDatabase.Views["View"].Fields.ContainsKey("AllowCreateRoles"))
    //            {
    //                configDatabase.Views["Field"].Fields["AllowCreateRoles"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["AllowCreateRoles"].DisplayName = "Allow Create (Roles)";
    //            }
    //            if (configDatabase.Views["View"].Fields.ContainsKey("AllowEditRoles"))
    //            {
    //                configDatabase.Views["Field"].Fields["AllowEditRoles"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["AllowEditRoles"].DisplayName = "Allow Edit (Roles)";
    //            }
    //            if (configDatabase.Views["View"].Fields.ContainsKey("AllowSelectRoles"))
    //            {
    //                configDatabase.Views["Field"].Fields["AllowSelectRoles"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["AllowSelectRoles"].DisplayName = "Allow Read (Roles)";
    //            }


    //            //configDatabase.Views["Field"].Fields["Fields_Parent"].HideInTable = true;
    //            //((ParentField)configDatabase.Views["Field"].Fields["Fields_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
    //            configDatabase.Views["Field"].Fields["Fields_Parent"].DisplayName = "View";
    //            configDatabase.Views["Field"].Fields["Fields_Parent"].DisableInEdit = true;
    //            //configDatabase.Views["Field"].Fields["Fields_Parent"].NoHyperlink = false;
    //            configDatabase.Views["Field"].Fields["Fields_Parent"].HideInCreate = false;
    //            //configDatabase.Views["Field"].Fields["Fields_Parent"].DisableInCreate = true;
    //            //((ParentField)configDatabase.Views["Field"].Fields["Fields_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
    //            //configDatabase.Views["Field"].Fields["GraphicProperties"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["GraphicProperties"].DisplayName = "CSS Class";
    //            //configDatabase.Views["Field"].Fields["ContainerGraphicProperties"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["ContainerGraphicProperties"].DisplayName = "Parent CSS Class";
    //            configDatabase.Views["Field"].Fields["Upload_Parent"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Upload_Parent"].DisplayName = "Upload";
    //            configDatabase.Views["Field"].Fields["EnumType"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["EnumType"].DisplayName = "Enum Type";
    //            //configDatabase.Views["Field"].Fields["StringConversionFormat"].HideInTable = true;
    //            //configDatabase.Views["Field"].Fields["StringConversionFormat"].DisplayName = "Conversion Format";

    //            if (configDatabase.Views["Field"].Fields.ContainsKey("NoHyperlink"))
    //            {
    //                configDatabase.Views["Field"].Fields["NoHyperlink"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["NoHyperlink"].DisplayName = "No Hyperlink?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("RadioColumns"))
    //            {
    //                configDatabase.Views["Field"].Fields["RadioColumns"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["RadioColumns"].DisplayName = "Radio Columns";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("InlineAdding"))
    //            {
    //                //configDatabase.Views["Field"].Fields["InlineAdding"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["InlineAdding"].DisplayName = "Inline Adding?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("BaseFieldName"))
    //            {
    //                configDatabase.Views["Field"].Fields["BaseFieldName"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["BaseFieldName"].DisplayName = "Base Field Name";
    //            }
    //            //configDatabase.Views["Field"].Fields["ExcludeInUpdate"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["ExcludeInUpdate"].DisplayName = "Exclude In Update?";
    //            //configDatabase.Views["Field"].Fields["ExcludeInInsert"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["ExcludeInInsert"].DisplayName = "Exclude In Insert?";
    //            configDatabase.Views["Field"].Fields["DisableInFilter"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["DisableInFilter"].DisplayName = "Disable In Filter?";
    //            configDatabase.Views["Field"].Fields["BooleanHtmlControlType"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["BooleanHtmlControlType"].DisplayName = "Boolean Html Control Type";
    //            ((ColumnField)configDatabase.Views["Field"].Fields["BooleanHtmlControlType"]).EnumType = typeof(BooleanHtmlControlType).AssemblyQualifiedName;
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("LabelContentLayout"))
    //                ((ColumnField)configDatabase.Views["Field"].Fields["LabelContentLayout"]).EnumType = typeof(Orientation).AssemblyQualifiedName;
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("UpdateParent"))
    //                ((ColumnField)configDatabase.Views["Field"].Fields["UpdateParent"]).EnumType = typeof(UpdateParent).AssemblyQualifiedName;
    //            configDatabase.Views["Field"].Fields["RadioOrientation"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["RadioOrientation"].DisplayName = "Radio Orientation";
    //            ((ColumnField)configDatabase.Views["Field"].Fields["RadioOrientation"]).EnumType = typeof(Orientation).AssemblyQualifiedName;
    //            configDatabase.Views["Field"].Fields["Rich"].HideInTable = true;
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("DependencyFieldName"))
    //            {
    //                configDatabase.Views["Field"].Fields["DependencyFieldName"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["DependencyFieldName"].DisplayName = "Dependency Field Name";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("InsideTriggerFieldName"))
    //            {
    //                configDatabase.Views["Field"].Fields["InsideTriggerFieldName"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["InsideTriggerFieldName"].DisplayName = "Inside Trigger Field Name";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("Category_Parent"))
    //            {
    //                configDatabase.Views["Field"].Fields["Category_Parent"].DisplayName = "Category";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("Dialog"))
    //            {
    //                configDatabase.Views["Field"].Fields["Dialog"].HideInTable = true;
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("PartialLength"))
    //            {
    //                configDatabase.Views["Field"].Fields["PartialLength"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["PartialLength"].DisplayName = "Partial Length";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("ShowDependencyInTable"))
    //            {
    //                configDatabase.Views["Field"].Fields["ShowDependencyInTable"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["ShowDependencyInTable"].DisplayName = "Show Dependency In Table?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("Custom"))
    //            {
    //                configDatabase.Views["Field"].Fields["Custom"].HideInTable = true;
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("MultiFilter"))
    //            {
    //                configDatabase.Views["Field"].Fields["MultiFilter"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["MultiFilter"].DisplayName = "Multi Filter?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("InlineDuplicate"))
    //            {
    //                configDatabase.Views["Field"].Fields["InlineDuplicate"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["InlineDuplicate"].DisplayName = "Inline Duplicate?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("InlineSearch"))
    //            {
    //                configDatabase.Views["Field"].Fields["InlineSearch"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["InlineSearch"].DisplayName = "Inline Search?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("InlineSearchView"))
    //            {
    //                configDatabase.Views["Field"].Fields["InlineSearchView"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["InlineSearchView"].DisplayName = "Inline Search View";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("SelectionSortColumn"))
    //            {
    //                configDatabase.Views["Field"].Fields["SelectionSortColumn"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["SelectionSortColumn"].DisplayName = "Dropdown Sort Column";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("DefaultFilter"))
    //            {
    //                configDatabase.Views["Field"].Fields["DefaultFilter"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["DefaultFilter"].DisplayName = "Default Filter";
    //            }


    //            /*
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("Validation_Parent"))
    //            {
    //                configDatabase.Views["Field"].Fields["Validation_Parent"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["Validation_Parent"].DisplayName = "Validation Rules";
    //                configDatabase.Views["Field"].Fields["Validation_Parent"].HideInEdit = false;
    //                ((ParentField)configDatabase.Views["Field"].Fields["Validation_Parent"]).InlineEditing = true;

    //                configDatabase.Views["Field"].Fields["Validation_Parent"].HideInCreate = true;
    //                //((ParentField)configDatabase.Views["Field"].Fields["Validation_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Autocomplete;
    //            }*/
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("Required"))
    //            {
    //                //configDatabase.Views["Field"].Fields["Required"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["Required"].DisplayName = "Required?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("Unique"))
    //            {
    //                configDatabase.Views["Field"].Fields["Unique"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["Unique"].DisplayName = "Unique for Duplicate?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("PartFromUniqueIndex"))
    //            {
    //                configDatabase.Views["Field"].Fields["PartFromUniqueIndex"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["PartFromUniqueIndex"].DisplayName = "Part From Unique Index?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("BrowserAutocomplete"))
    //            {
    //                configDatabase.Views["Field"].Fields["BrowserAutocomplete"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["BrowserAutocomplete"].DisplayName = "BrowserAutocomplete?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("AdvancedFilter"))
    //            {
    //                configDatabase.Views["Field"].Fields["AdvancedFilter"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["AdvancedFilter"].DisplayName = "Advanced Filter?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("AllowDuplication"))
    //            {
    //                configDatabase.Views["Field"].Fields["AllowDuplication"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["AllowDuplication"].DisplayName = "Allow Duplication?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("Integral"))
    //            {
    //                configDatabase.Views["Field"].Fields["Integral"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["Integral"].DisplayName = "Integral?";
    //            }
                //configDatabase.Views["Field"].Fields["AutocompleteColumn"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["AutocompleteTable"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["AutocompleteSql"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["ContainerGraphicField"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["TableCellMinWidth"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["Refresh"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["SaveHistory"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["CheckListInTableLimit"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["Import"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["MinWidth"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["EditInTableView"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["EditInTableView"].DisplayName= "Open link in dialog";
                //configDatabase.Views["Field"].Fields["Seperator"].HideInTable = true;
                ////configDatabase.Views["Field"].Fields["DisableInDuplicate"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["GridEditable"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["Seperator"].HideInTable = true;
                //configDatabase.Views["Field"].Fields["Seperator"].HideInTable = true;

    //            if (configDatabase.Views["Field"].Fields.ContainsKey("TextHtmlControlType"))
    //            {
    //                configDatabase.Views["Field"].Fields["TextHtmlControlType"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["TextHtmlControlType"].DisplayName = "Text Html Control Type";
    //                ((ColumnField)configDatabase.Views["Field"].Fields["TextHtmlControlType"]).EnumType = typeof(TextHtmlControlType).AssemblyQualifiedName;
    //            }

    //            if (configDatabase.Views["Field"].Fields.ContainsKey("TrimSpaces"))
    //            {
    //                configDatabase.Views["Field"].Fields["TrimSpaces"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["TrimSpaces"].DisplayName = "Trim Spaces?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("LabelContentLayout"))
    //            {
    //                configDatabase.Views["Field"].Fields["LabelContentLayout"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["LabelContentLayout"].DisplayName = "Label Content Layout";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("Searchable"))
    //            {
    //                configDatabase.Views["Field"].Fields["Searchable"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["Searchable"].DisplayName = "Searchable?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("NoCache"))
    //            {
    //                configDatabase.Views["Field"].Fields["NoCache"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["NoCache"].DisplayName = "Prevent Cache on Tab?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("CounterInitiated"))
    //            {
    //                configDatabase.Views["Field"].Fields["CounterInitiated"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["CounterInitiated"].DisplayName = "Counter In itiated?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("Counter"))
    //            {
    //                configDatabase.Views["Field"].Fields["Counter"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["Counter"].DisplayName = "Counter?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("LoadForBlockTemplate"))
    //            {
    //                configDatabase.Views["Field"].Fields["LoadForBlockTemplate"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["LoadForBlockTemplate"].DisplayName = "Include in Word template?";
    //            }
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("HideInTableMobile"))
    //            {
    //                configDatabase.Views["Field"].Fields["HideInTableMobile"].HideInTable = true;
    //                configDatabase.Views["Field"].Fields["HideInTableMobile"].DisplayName = "Hide In Table Mobile?";
    //            }

    //            configDatabase.Views["Field"].Fields["AutocompleteColumn"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["AutocompleteTable"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["AutocompleteSql"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["ContainerGraphicField"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["TableCellMinWidth"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Refresh"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["SaveHistory"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["CheckListInTableLimit"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Import"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["MinWidth"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["EditInTableView"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Seperator"].HideInTable = true;
    //            //configDatabase.Views["Field"].Fields["DisableInDuplicate"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["GridEditable"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Seperator"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Seperator"].HideInTable = true;

    //            configDatabase.Views["Field"].Fields["MultiValueParentViewName"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["MultiValueAdditionals"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["SeperatorTitle"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["SubGridExport"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["SubGridPlacement"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["DropDownValueField"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["DropDownDisplayField"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["AutocompleteFilter"].HideInTable = true;
    //            if (configDatabase.Views["View"].Fields.ContainsKey("WorkFlowStepsFieldName"))
    //                configDatabase.Views["View"].Fields["WorkFlowStepsFieldName"].HideInTable = true;


    //            configDatabase.Views["Field"].Fields["CloneChildrenViewName"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["CloneChildrenViewName"].DisplayName = "Clone View Name";

    //            configDatabase.Views["Field"].Fields["MultiValueParentViewName"].DisplayName = "Drop Down Parent View Name";
    //            configDatabase.Views["Field"].Fields["DropDownValueField"].DisplayName = "Drop Down Value Field";
    //            configDatabase.Views["Field"].Fields["DropDownDisplayField"].DisplayName = "Drop Down Display Field";

    //            configDatabase.Views["Field"].Fields["Description"].ColSpanInDialog = 2;
    //            configDatabase.Views["Field"].Fields["Description"].DisplayName = "Description / Tooltip";
    //            configDatabase.Views["Field"].Fields["Description"].HideInCreate = false;
    //            (configDatabase.Views["Field"].Fields["Description"] as ColumnField).TextHtmlControlType = TextHtmlControlType.Text;
    //            (configDatabase.Views["Field"].Fields["Description"] as ColumnField).CssClass = "textboxmid";

    //            configDatabase.Views["Field"].Fields["DataType"].DisplayName = "Type";
    //            configDatabase.Views["Field"].Fields["DataType"].HideInTable = false;
    //            configDatabase.Views["Field"].Fields["DataType"].HideInCreate = false;
    //            ((ColumnField)configDatabase.Views["Field"].Fields["DataType"]).EnumType = typeof(DataType).AssemblyQualifiedName;
    //            configDatabase.Views["Field"].Fields["DataType"].DefaultValue = "ShortText";

    //            configDatabase.Views["Field"].Fields["DatabaseNames"].HideInTable = false;
    //            configDatabase.Views["Field"].Fields["DatabaseNames"].DisableInEdit = true;
    //            configDatabase.Views["Field"].Fields["DatabaseNames"].DisableInCreate = true;
    //            configDatabase.Views["Field"].Fields["DatabaseNames"].DisplayName = "Database Name";

    //            configDatabase.Views["Field"].Fields["IsUnique"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Min"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["Max"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["UpdateParent"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["UpdateParentInGrid"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["SearchFilter"].HideInTable = true;
    //            configDatabase.Views["Field"].Fields["HideInDerivation"].HideInTable = true;

    //            configDatabase.Views["Field"].Fields["Fields_Parent"].Order = 5;
    //            configDatabase.Views["Field"].Fields["Name"].Order = 10;
    //            configDatabase.Views["Field"].Fields["RelatedViewName"].Order = 15;
    //            configDatabase.Views["Field"].Fields["DisplayName"].Order = 20;
    //            configDatabase.Views["Field"].Fields["DatabaseNames"].Order = 21;
    //            configDatabase.Views["Field"].Fields["DataType"].Order = 22;
    //            configDatabase.Views["Field"].Fields["Description"].Order = 24;
    //            configDatabase.Views["Field"].Fields["Category_Parent"].Order = 27;
    //            configDatabase.Views["Field"].Fields["Order"].Order = 30;
    //            configDatabase.Views["Field"].Fields["OrderForCreate"].Order = 32;
    //            configDatabase.Views["Field"].Fields["OrderForEdit"].Order = 34;
    //            configDatabase.Views["Field"].Fields["Excluded"].Order = 40;
    //            configDatabase.Views["Field"].Fields["HideInTable"].Order = 50;
    //            configDatabase.Views["Field"].Fields["HideInEdit"].Order = 60;
    //            configDatabase.Views["Field"].Fields["HideInCreate"].Order = 70;
    //            configDatabase.Views["Field"].Fields["DisableInEdit"].Order = 75;
    //            configDatabase.Views["Field"].Fields["DisableInCreate"].Order = 80;
    //            configDatabase.Views["Field"].Fields["DisableInDuplicate"].Order = 85;
    //            configDatabase.Views["Field"].Fields["DefaultValue"].Order = 90;
    //            configDatabase.Views["Field"].Fields["Required"].Order = 100;
    //            configDatabase.Views["Field"].Fields["ExcludeInUpdate"].Order = 110;
    //            configDatabase.Views["Field"].Fields["ExcludeInInsert"].Order = 120;
    //            configDatabase.Views["Field"].Fields["InlineEditing"].Order = 130;
    //            configDatabase.Views["Field"].Fields["InlineAdding"].Order = 140;
    //            configDatabase.Views["Field"].Fields["Precedent"].Order = 150;
    //            configDatabase.Views["Field"].Fields["ContainerGraphicProperties"].Order = 160;
    //            configDatabase.Views["Field"].Fields["GraphicProperties"].Order = 170;

    //            configDatabase.Views["Field"].Fields["DisplayName"].OrderForEdit = 10;
    //            configDatabase.Views["Field"].Fields["Name"].OrderForEdit = 20;
    //            configDatabase.Views["Field"].Fields["DataType"].OrderForEdit = 30;
    //            configDatabase.Views["Field"].Fields["DatabaseNames"].OrderForEdit = 40;
    //            configDatabase.Views["Field"].Fields["RelatedViewName"].OrderForEdit = 50;
    //            configDatabase.Views["Field"].Fields["Description"].OrderForEdit = 60;
    //            configDatabase.Views["Field"].Fields["Fields_Parent"].OrderForEdit = 55;

    //            configDatabase.Views["Field"].Fields["DisplayName"].OrderForCreate = 10;
    //            configDatabase.Views["Field"].Fields["Name"].OrderForCreate = 20;
    //            configDatabase.Views["Field"].Fields["DataType"].OrderForCreate = 30;
    //            configDatabase.Views["Field"].Fields["DatabaseNames"].OrderForCreate = 40;
    //            configDatabase.Views["Field"].Fields["RelatedViewName"].OrderForCreate = 50;
    //            configDatabase.Views["Field"].Fields["Description"].OrderForCreate = 60;
    //            configDatabase.Views["Field"].Fields["Fields_Parent"].OrderForCreate = 55;

    //            // switch reorder
    //            ((Durados.Web.Mvc.View)configDatabase.Views["Field"]).OrdinalColumnName = "Order";
    //            ((Durados.Web.Mvc.View)configDatabase.Views["Field"]).DefaultSort = "Order";


    //            if (configDatabase.Views.ContainsKey("Upload"))
    //            {
    //                configDatabase.Views["Upload"].DisplayColumn = "Title";
    //                //configDatabase.Views["Upload"].DenyCreateRoles = configDatabase.DenyConfigConfigRoles;
    //                //configDatabase.Views["Upload"].DenyDeleteRoles = configDatabase.DenyConfigConfigRoles;
    //                //configDatabase.Views["Upload"].DenyEditRoles = configDatabase.DenyConfigConfigRoles;
    //                //configDatabase.Views["Upload"].DenySelectRoles = configDatabase.DenyConfigConfigRoles;
    //                /* todo - check
    //                configDatabase.Views["Upload"].AllowCreateRoles = configDatabase.AllowConfigConfigRoles;
    //                configDatabase.Views["Upload"].AllowDeleteRoles = configDatabase.AllowConfigConfigRoles;
    //                configDatabase.Views["Upload"].AllowEditRoles = configDatabase.AllowConfigConfigRoles;
    //                configDatabase.Views["Upload"].AllowSelectRoles = configDatabase.AllowConfigConfigRoles;
    //                */


    //                configDatabase.Views["Upload"].Fields["ID"].DisplayName = "Id";
    //                configDatabase.Views["Upload"].Fields["UploadFileType"].DisplayName = "Upload File Type";
    //                configDatabase.Views["Upload"].Fields["UploadStorageType"].DisplayName = "Upload Storage Type";
    //                if (configDatabase.Views["Upload"].Fields.ContainsKey("UploadVirtualPath"))
    //                    configDatabase.Views["Upload"].Fields["UploadVirtualPath"].DisplayName = "Virtual Path";
    //                if (configDatabase.Views["Upload"].Fields.ContainsKey("UploadPhysicalPath"))
    //                    configDatabase.Views["Upload"].Fields["UploadPhysicalPath"].DisplayName = "Physical Path";
    //                configDatabase.Views["Upload"].Fields["Title"].DisplayName = "Title";
    //                configDatabase.Views["Upload"].Fields["Upload_Children"].DisplayName = "Attachment";

    //                ((ColumnField)configDatabase.Views["Upload"].Fields["UploadFileType"]).EnumType = typeof(UploadFileType).AssemblyQualifiedName;
    //                ((ColumnField)configDatabase.Views["Upload"].Fields["UploadStorageType"]).EnumType = typeof(UploadStorageType).AssemblyQualifiedName;

    //            }

    //            if (configDatabase.Views.ContainsKey("Milestone"))
    //            {
    //                configDatabase.Views["Milestone"].DisplayColumn = "Name";

    //            }

    //            if (configDatabase.Views.ContainsKey("Localization"))
    //            {
    //                configDatabase.Views["Localization"].DisplayColumn = "Title";
    //                //configDatabase.Views["Localization"].DenyCreateRoles = configDatabase.DenyConfigConfigRoles;
    //                //configDatabase.Views["Localization"].DenyDeleteRoles = configDatabase.DenyConfigConfigRoles;
    //                //configDatabase.Views["Localization"].DenyEditRoles = configDatabase.DenyConfigConfigRoles;
    //                //configDatabase.Views["Localization"].DenySelectRoles = configDatabase.DenyConfigConfigRoles;
    //            }

    //            if (configDatabase.Views["Field"].Fields.ContainsKey("ParentHtmlControlType"))
    //                ((ColumnField)configDatabase.Views["Field"].Fields["ParentHtmlControlType"]).EnumType = typeof(ParentHtmlControlType).AssemblyQualifiedName;
    //            ((ColumnField)configDatabase.Views["Field"].Fields["FieldType"]).EnumType = typeof(FieldType).AssemblyQualifiedName;
    //            if (configDatabase.Views["Field"].Fields.ContainsKey("ChildrenHtmlControlType"))
    //                ((ColumnField)configDatabase.Views["Field"].Fields["ChildrenHtmlControlType"]).EnumType = typeof(ChildrenHtmlControlType).AssemblyQualifiedName;


    //            //configDatabase.Views["Category"].DenyCreateRoles = configDatabase.DenyConfigConfigRoles;
    //            //configDatabase.Views["Category"].DenyDeleteRoles = configDatabase.DenyConfigConfigRoles;
    //            //configDatabase.Views["Category"].DenyEditRoles = configDatabase.DenyConfigConfigRoles;
    //            //configDatabase.Views["Category"].DenySelectRoles = configDatabase.DenyConfigConfigRoles;

    //            foreach (Field field in configDatabase.Views["Database"].Fields.Values)
    //            {
    //                field.HideInTable = true;
    //            }

    //            if (configDatabase.Views["Database"].Fields.ContainsKey("SpecialMenus_Children"))
    //            {
    //                configDatabase.Views["Database"].Fields["SpecialMenus_Children"].DisplayName = "Menus";
    //                configDatabase.Views["Database"].Fields["SpecialMenus_Children"].HideInTable = false;
    //                configDatabase.Views["Database"].Fields["SpecialMenus_Children"].Order = -1;
    //            }

    //            if (configDatabase.Views["Database"].Fields.ContainsKey("Tooltips_Children"))
    //            {
    //                configDatabase.Views["Database"].Fields["Tooltips_Children"].DisplayName = "Tooltips";
    //                configDatabase.Views["Database"].Fields["Tooltips_Children"].HideInTable = false;
    //                configDatabase.Views["Database"].Fields["Tooltips_Children"].Order = 0;
    //            }

    //            if (configDatabase.Views["Database"].Fields.ContainsKey("Crons_Children"))
    //            {
    //                configDatabase.Views["Database"].Fields["Crons_Children"].DisplayName = "Crons";
    //                configDatabase.Views["Database"].Fields["Crons_Children"].HideInTable = false;
    //                configDatabase.Views["Database"].Fields["Crons_Children"].Order = 1;
    //            }

    //            if (configDatabase.Views.ContainsKey("Tooltip"))
    //            {
    //                configDatabase.Views["Tooltip"].ColumnsInDialog = 1;
    //                configDatabase.Views["Tooltip"].HideInMenu = true;
    //                configDatabase.Views["Tooltip"].AllowCreate = false;
    //                configDatabase.Views["Tooltip"].AllowDuplicate = false;
    //                configDatabase.Views["Tooltip"].AllowDelete = false;

    //                if (configDatabase.Views["Tooltip"].Fields.ContainsKey("Tooltips_Parent"))
    //                {
    //                    configDatabase.Views["Tooltip"].Fields["Tooltips_Parent"].HideInTable = true;
    //                    ((ParentField)configDatabase.Views["Tooltip"].Fields["Tooltips_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
    //                    configDatabase.Views["Tooltip"].Fields["Tooltips_Parent"].Order = 100;
    //                    configDatabase.Views["Tooltip"].Fields["Tooltips_Parent"].HideInEdit = true;
    //                    configDatabase.Views["Tooltip"].Fields["Tooltips_Parent"].HideInCreate = true;
    //                }
    //                if (configDatabase.Views["Tooltip"].Fields.ContainsKey("ID"))
    //                {
    //                    configDatabase.Views["Tooltip"].Fields["ID"].HideInTable = true;
    //                }
    //                if (configDatabase.Views["Tooltip"].Fields.ContainsKey("Name"))
    //                {
    //                    configDatabase.Views["Tooltip"].Fields["Name"].HideInTable = true;
    //                    configDatabase.Views["Tooltip"].Fields["Name"].HideInEdit = true;
    //                    configDatabase.Views["Tooltip"].Fields["Name"].HideInCreate = true;
    //                }

    //                if (configDatabase.Views["Tooltip"].Fields.ContainsKey("Description"))
    //                {
    //                    ((ColumnField)configDatabase.Views["Tooltip"].Fields["Description"]).TextHtmlControlType = TextHtmlControlType.TextArea;
    //                    ((ColumnField)configDatabase.Views["Tooltip"].Fields["Description"]).Rich = false;
    //                }
    //            }

    //            if (configDatabase.Views.ContainsKey("SpecialMenu"))
    //            {
    //                configDatabase.Views["SpecialMenu"].HideInMenu = true;
    //                if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("Menus_Children"))
    //                {
    //                    configDatabase.Views["SpecialMenu"].Fields["Menus_Children"].HideInTable = false;
    //                    configDatabase.Views["SpecialMenu"].Fields["Menus_Children"].DisplayName = "Menus";
    //                }
    //                if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("Links_Children"))
    //                {
    //                    configDatabase.Views["SpecialMenu"].Fields["Links_Children"].HideInTable = false;
    //                }
    //                if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("Root"))
    //                {
    //                    configDatabase.Views["SpecialMenu"].Fields["Root"].HideInTable = true;
    //                    configDatabase.Views["SpecialMenu"].Fields["Root"].HideInEdit = true;
    //                    configDatabase.Views["SpecialMenu"].Fields["Root"].HideInCreate = true;
    //                }
    //                if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("Menus_Parent"))
    //                {
    //                    configDatabase.Views["SpecialMenu"].Fields["Menus_Parent"].HideInTable = true;
    //                    ((ParentField)configDatabase.Views["SpecialMenu"].Fields["Menus_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
    //                    configDatabase.Views["SpecialMenu"].Fields["Menus_Parent"].Order = 100;
    //                }
    //                if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("SpecialMenus_Parent"))
    //                {
    //                    configDatabase.Views["SpecialMenu"].Fields["SpecialMenus_Parent"].HideInTable = true;
    //                    ((ParentField)configDatabase.Views["SpecialMenu"].Fields["SpecialMenus_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
    //                    configDatabase.Views["SpecialMenu"].Fields["SpecialMenus_Parent"].Order = 100;

    //                }
    //                if (configDatabase.Views["SpecialMenu"].Fields.ContainsKey("ID"))
    //                {
    //                    configDatabase.Views["SpecialMenu"].Fields["SpecialMenus_Parent"].HideInTable = true;
    //                }
    //            }

    //            if (configDatabase.Views.ContainsKey("Cron"))
    //            {
    //                configDatabase.Views["Cron"].HideInMenu = true;

    //                ColumnField template = (ColumnField)configDatabase.Views["Cron"].Fields["Template"];
    //                template.TextHtmlControlType = TextHtmlControlType.DropDown;
    //                template.MultiValueParentViewName = "durados_Html";

    //                if (configDatabase.Views["Cron"].Fields.ContainsKey("Crons_Parent"))
    //                {
    //                    configDatabase.Views["Cron"].Fields["Crons_Parent"].HideInTable = true;
    //                    ((ParentField)configDatabase.Views["Cron"].Fields["Crons_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
    //                    configDatabase.Views["Cron"].Fields["Crons_Parent"].Order = 100;
    //                    configDatabase.Views["Cron"].Fields["Crons_Parent"].DefaultValue = "0";

    //                }

    //            }


    //            foreach (View view in configDatabase.Views.Values)
    //            {
    //                view.GridEditable = true;
    //                view.GridEditableEnabled = true;
    //            }

    //            //reset security to all fields
    //            foreach (Field field in configDatabase.Views["View"].Fields.Values)
    //            {
    //                field.Precedent = true;
    //                field.AllowSelectRoles = "Developer,Admin";
    //                field.AllowCreateRoles = "Developer,Admin";
    //                field.AllowEditRoles = "Developer,Admin";
    //                field.DenyCreateRoles = "";
    //                field.DenyEditRoles = "";
    //                field.DenySelectRoles = "";

    //            }

    //            foreach (Field field in configDatabase.Views["Field"].Fields.Values)
    //            {
    //                field.Precedent = true;
    //                field.AllowSelectRoles = "Developer,Admin";
    //                field.AllowCreateRoles = "Developer,Admin";
    //                field.AllowEditRoles = "Developer,Admin";
    //                field.DenyCreateRoles = "";
    //                field.DenyEditRoles = "";
    //                field.DenySelectRoles = "";

    //            }

    //            if (configDatabase.Views["View"].Fields.ContainsKey("Name"))
    //            {
    //                ColumnField databaseTableNameField = ((ColumnField)configDatabase.Views["View"].Fields["Name"]);
    //                databaseTableNameField.TextHtmlControlType = TextHtmlControlType.Autocomplete;
    //                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;

    //                databaseTableNameField.AutocompleteConnectionString = connectionString;
    //                databaseTableNameField.AutocompleteSql = (new SqlSchema()).GetTableAndViewsNamesSelectStatement();
    //                databaseTableNameField.AutocompleteColumn = "Name";
    //                databaseTableNameField.DisableInEdit = true;
    //                databaseTableNameField.ExcludeInUpdate = true;
    //                databaseTableNameField.DisableInCreate = false;
    //                databaseTableNameField.ExcludeInInsert = false;
    //                databaseTableNameField.AutocompleteFilter = false;
    //            }

    //            if (configDatabase.Views["View"].Fields.ContainsKey("EditableTableName"))
    //            {
    //                ColumnField editableTableNameField = ((ColumnField)configDatabase.Views["View"].Fields["EditableTableName"]);
    //                editableTableNameField.TextHtmlControlType = TextHtmlControlType.Autocomplete;
    //                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;

    //                editableTableNameField.AutocompleteConnectionString = connectionString;
    //                editableTableNameField.AutocompleteSql = (new SqlSchema()).GetTableNamesSelectStatement();
    //                editableTableNameField.AutocompleteColumn = "Name";
    //            }

    //            if (configDatabase.Views["Field"].Fields.ContainsKey("RelatedViewName"))
    //            {
    //                ColumnField relatedViewNameField = ((ColumnField)configDatabase.Views["Field"].Fields["RelatedViewName"]);
    //                //relatedViewNameField.TextHtmlControlType = TextHtmlControlType.Autocomplete;
    //                //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;

    //                //relatedViewNameField.AutocompleteConnectionString = connectionString;
    //                //relatedViewNameField.AutocompleteSql = (new SqlSchema()).GetTableAndViewsNamesSelectStatement();
    //                //relatedViewNameField.AutocompleteColumn = "Name";
    //                relatedViewNameField.HideInCreate = false;
    //            }

    //            ConfigCategories(configDatabase);

    //            foreach (Field field in configDatabase.Views["View"].Fields.Values.Where(f => f.Category != null && (f.Category.Name == "Developers" || f.Category.Name == "Advanced" || f.Category.Name == "System")))
    //            {
    //                field.AllowSelectRoles = "Developer";
    //            }

    //            foreach (Field field in configDatabase.Views["Field"].Fields.Values.Where(f => f.Category != null && (f.Category.Name == "Developers" || f.Category.Name == "Advanced" || f.Category.Name == "System" || f.Category.Name == "Xml")))
    //            {
    //                SetRoles(field, "Developer");
    //            }

    //            ConfigureSecurity(configDatabase);

    //            SetNewViewDefaults(configDatabase);

    //        }
    //        catch { }
    //    }

    //    private void SetNewViewDefaults(Database configDatabase)
    //    {
    //        Database db = Map.GetDefaultDatabase();

    //        View view = (View)db.Views["Table"];
    //        Type type = view.GetType();

    //        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


    //        foreach (PropertyInfo propertyInfo in properties)
    //        {
    //            object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(Durados.Config.Attributes.PropertyAttribute), true);
    //            Durados.Config.Attributes.PropertyAttribute propertyAttribute = null;
    //            if (propertyAttributes.Length == 1)
    //            {
    //                propertyAttribute = (Durados.Config.Attributes.PropertyAttribute)propertyAttributes[0];

    //            }

    //            if (propertyAttribute != null)
    //            {
    //                //PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];

    //                switch (propertyAttribute.PropertyType)
    //                {
    //                    case Durados.Config.Attributes.PropertyType.Column:
    //                        try
    //                        {
    //                            if (propertyInfo.Name != "DisplayName" && propertyInfo.Name != "DisplayColumn" && configDatabase.Views["View"].Fields.ContainsKey(propertyInfo.Name))
    //                                configDatabase.Views["View"].Fields[propertyInfo.Name].DefaultValue = propertyInfo.GetValue(view, null);
    //                        }
    //                        catch { }
    //                        break;

    //                    default:
    //                        break;
    //                }
    //            }
    //        }
    //    }



    //    private void SetRoles(Field field, string roles)
    //    {
    //        field.AllowSelectRoles = roles;
    //        field.AllowCreateRoles = roles;
    //        field.AllowEditRoles = roles;
    //    }

    //    private void SetViewRoles(Durados.View view, string roles)
    //    {
    //        view.Precedent = true;
    //        view.AllowCreateRoles = roles;
    //        view.AllowEditRoles = roles;
    //        view.AllowSelectRoles = roles;
    //        view.AllowDeleteRoles = roles;

    //    }

    //    private void ConfigureSecurity(Database configDatabase)
    //    {
    //        if (configDatabase.Views["View"].Fields.ContainsKey("WorkspaceID"))
    //        {
    //            ColumnField workspaceID = (ColumnField)configDatabase.Views["View"].Fields["WorkspaceID"];
    //            workspaceID.TextHtmlControlType = TextHtmlControlType.DropDown;
    //            workspaceID.MultiValueParentViewName = "Workspace";
    //            workspaceID.DisplayName = "Workspace";
    //        }

    //        if (configDatabase.Views["Link"].Fields.ContainsKey("WorkspaceID"))
    //        {
    //            ColumnField workspaceID = (ColumnField)configDatabase.Views["Link"].Fields["WorkspaceID"];
    //            workspaceID.TextHtmlControlType = TextHtmlControlType.DropDown;
    //            workspaceID.MultiValueParentViewName = "Workspace";
    //            workspaceID.DisplayName = "Workspace";
    //            workspaceID.ExcludeInInsert = false;
    //            workspaceID.ExcludeInUpdate = false;
    //            workspaceID.HideInCreate = false;
    //            workspaceID.HideInEdit = false;
    //        }

    //        //Workspace
    //        configDatabase.Views["Workspace"].ColumnsInDialog = 1;
    //        configDatabase.Views["Workspace"].DisplayName = "Workspace Permission";
    //        configDatabase.Views["Workspace"].Description = "Grant and restrict permission to Workspace.<br/> Select roles for Allow to grant permission for functioanlity and select deny to restrict role for functionality";
    //        configDatabase.Views["Workspace"].GridEditable = false;
    //        configDatabase.Views["Workspace"].AllowDelete = false;
    //        ((View)configDatabase.Views["Workspace"]).Derivation = new Derivation() { DerivationField = "Precedent", Deriveds = "0;AllowCreateRoles,AllowEditRoles,AllowDeleteRoles,AllowSelectRoles,DenyCreateRoles,DenyEditRoles,DenyDeleteRoles,DenySelectRoles" };

    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["ID"]).HideInTable = true;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["Name"]).DisplayName = "Workspace Name";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["Name"]).Description = "The title of the view";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["Name"]).Order = 0;
    //        if (configDatabase.Views["Workspace"].Fields.ContainsKey("Workspaces_Parent"))
    //        {
    //            ((ParentField)configDatabase.Views["Workspace"].Fields["Workspaces_Parent"]).DefaultValue = 0;
    //            ((ParentField)configDatabase.Views["Workspace"].Fields["Workspaces_Parent"]).ParentHtmlControlType = ParentHtmlControlType.Hidden;
    //            ((ParentField)configDatabase.Views["Workspace"].Fields["Workspaces_Parent"]).HideInTable = true;
    //        }

    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["Precedent"]).DisplayName = "Override permissions";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["Precedent"]).Description = "By checking this option you override the System default permissions";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["Precedent"]).Order = 10;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["Precedent"]).GridEditable = false;

    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).Seperator = true;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).SeperatorTitle = "Please select the Roles for Allow (Grant) permissions:";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).Order = 20;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).DisplayName = "Allow Create";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).Description = "Can create, duplicate records";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowEditRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowEditRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowEditRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowEditRoles"]).Order = 30;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowEditRoles"]).DisplayName = "Allow Edit";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowEditRoles"]).Description = "Can edit records";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowDeleteRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowDeleteRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowDeleteRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowDeleteRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowDeleteRoles"]).Order = 40;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowDeleteRoles"]).DisplayName = "Allow Delete";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowDeleteRoles"]).Description = "Can delete records";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowSelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowSelectRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowSelectRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowSelectRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowSelectRoles"]).Order = 50;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowSelectRoles"]).DisplayName = "Allow Read";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowSelectRoles"]).Description = "Can view information and history";

    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowSelectRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowEditRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowDeleteRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyEditRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyDeleteRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenySelectRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";



    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).Seperator = true;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).SeperatorTitle = "Please select Roles for Deny (Restrict) permissions:";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).Order = 100;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).DisplayName = "Deny Create";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).Description = "Can&#39t create or duplicate records";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyEditRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyEditRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyEditRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyEditRoles"]).Order = 110;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyEditRoles"]).DisplayName = "Deny Edit";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyEditRoles"]).Description = "Can&#39t edit records";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyDeleteRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyDeleteRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyDeleteRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyDeleteRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyDeleteRoles"]).Order = 120;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyDeleteRoles"]).DisplayName = "Deny Delete";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyDeleteRoles"]).Description = "Can&#39t delete records";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenySelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenySelectRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenySelectRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenySelectRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenySelectRoles"]).Order = 130;
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenySelectRoles"]).DisplayName = "Deny Read";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenySelectRoles"]).Description = "Can&#39t view information or select view";

    //        //Views
    //        ((View)configDatabase.Views["View"]).Derivation = new Derivation() { DerivationField = "Precedent", Deriveds = "0;AllowCreateRoles,AllowEditRoles,AllowDeleteRoles,AllowSelectRoles,DenyCreateRoles,DenyEditRoles,DenyDeleteRoles,DenySelectRoles" };

    //        ((ColumnField)configDatabase.Views["View"].Fields["Precedent"]).DisplayName = "Override inheritable";
    //        ((ColumnField)configDatabase.Views["View"].Fields["Precedent"]).Description = "By checking this option you override the Workspace permissions";
    //        ((ColumnField)configDatabase.Views["View"].Fields["Precedent"]).OrderForEdit = 10;

    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowCreateRoles"]).Seperator = true;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowCreateRoles"]).SeperatorTitle = "Please select the Roles for Allow (Grant) permissions:";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowCreateRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowCreateRoles"]).OrderForEdit = 20;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowCreateRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowCreateRoles"]).DisplayName = "Allow Create";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowCreateRoles"]).Description = "Can create, duplicate records";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowEditRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowEditRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowEditRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowEditRoles"]).OrderForEdit = 30;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowEditRoles"]).DisplayName = "Allow Edit";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowEditRoles"]).Description = "Can edit records";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowDeleteRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowDeleteRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowDeleteRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowDeleteRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowDeleteRoles"]).OrderForEdit = 40;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowDeleteRoles"]).DisplayName = "Allow Delete";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowDeleteRoles"]).Description = "Can delete records";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowSelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowSelectRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowSelectRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowSelectRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowSelectRoles"]).OrderForEdit = 50;
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowSelectRoles"]).DisplayName = "Allow Read";
    //        ((ColumnField)configDatabase.Views["View"].Fields["AllowSelectRoles"]).Description = "Can view information and history";

    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowSelectRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowEditRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowCreateRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["AllowDeleteRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyCreateRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyEditRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenyDeleteRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";
    //        ((ColumnField)configDatabase.Views["Workspace"].Fields["DenySelectRoles"]).ContainerGraphicProperties = "d_fieldContainerWrap";

    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyCreateRoles"]).Seperator = true;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyCreateRoles"]).SeperatorTitle = "Please select Roles for Deny (Restrict) permissions:";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyCreateRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyCreateRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyCreateRoles"]).OrderForEdit = 100;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyCreateRoles"]).DisplayName = "Deny Create";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyCreateRoles"]).Description = "Can&#39t create or duplicate records";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyEditRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyEditRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyEditRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyEditRoles"]).OrderForEdit = 110;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyEditRoles"]).DisplayName = "Deny Edit";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyEditRoles"]).Description = "Can&#39t edit records";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyDeleteRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyDeleteRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyDeleteRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyDeleteRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyDeleteRoles"]).OrderForEdit = 120;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyDeleteRoles"]).DisplayName = "Deny Delete";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenyDeleteRoles"]).Description = "Can&#39t delete records";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenySelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenySelectRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenySelectRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenySelectRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenySelectRoles"]).OrderForEdit = 130;
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenySelectRoles"]).DisplayName = "Deny Read";
    //        ((ColumnField)configDatabase.Views["View"].Fields["DenySelectRoles"]).Description = "Can&#39t view information or select view";

    //        //Fields
    //        ((View)configDatabase.Views["Field"]).Derivation = new Derivation() { DerivationField = "Precedent", Deriveds = "0;AllowCreateRoles,AllowEditRoles,AllowSelectRoles,DenyCreateRoles,DenyEditRoles,DenySelectRoles" };

    //        ((ColumnField)configDatabase.Views["Field"].Fields["Precedent"]).DisplayName = "Override inheritable";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["Precedent"]).Description = "By checking this option you override the View permissions";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["Precedent"]).OrderForEdit = 10;

    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowCreateRoles"]).Seperator = true;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowCreateRoles"]).SeperatorTitle = "Please select the Roles for Allow (Grant) permissions:";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowCreateRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowCreateRoles"]).OrderForEdit = 20;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowCreateRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowCreateRoles"]).DisplayName = "Allow Create";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowCreateRoles"]).Description = "Field displays in New or Duplicare dialog";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowEditRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowEditRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowEditRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowEditRoles"]).OrderForEdit = 30;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowEditRoles"]).DisplayName = "Allow Edit";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowEditRoles"]).Description = "Field displays in Edit dialog";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowSelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowSelectRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowSelectRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowSelectRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowSelectRoles"]).OrderForEdit = 50;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowSelectRoles"]).DisplayName = "Allow Read";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["AllowSelectRoles"]).Description = "Field displays in View (Grid) window";

    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyCreateRoles"]).Seperator = true;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyCreateRoles"]).SeperatorTitle = "Please select Roles for Deny (Restrict) permissions:";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyCreateRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyCreateRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyCreateRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyCreateRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyCreateRoles"]).OrderForEdit = 100;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyCreateRoles"]).DisplayName = "Deny Create";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyCreateRoles"]).Description = "Field doesn&#39t display in New or Duplicare dialog";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyEditRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyEditRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyEditRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyEditRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyEditRoles"]).OrderForEdit = 110;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyEditRoles"]).DisplayName = "Deny Edit";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenyEditRoles"]).Description = "Field doesn&#39t display in Edit dialog";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenySelectRoles"]).TextHtmlControlType = TextHtmlControlType.CheckList;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenySelectRoles"]).MultiValueAdditionals = "everyone,everyone";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenySelectRoles"]).MultiValueParentViewName = "UserRole";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenySelectRoles"]).MinWidth = 350;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenySelectRoles"]).OrderForEdit = 130;
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenySelectRoles"]).DisplayName = "Deny Read";
    //        ((ColumnField)configDatabase.Views["Field"].Fields["DenySelectRoles"]).Description = "Field doesn&#39t display in View (Grid) window";

    //    }


    //    protected virtual void ConfigCategories(Database configDatabase)
    //    {
    //        Category displayCategory = new Category() { Name = "Display", Ordinal = 10 };
    //        configDatabase.Views["View"].Fields["Order"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["Order"], "Developer");
    //        configDatabase.Views["View"].Fields["ColumnsInDialog"].Category = displayCategory;
    //        configDatabase.Views["View"].Fields["PageSize"].Category = displayCategory;
    //        configDatabase.Views["View"].Fields["DisplayColumn"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["DisplayColumn"], "Developer");
    //        configDatabase.Views["View"].Fields["Menu_Parent"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["Menu_Parent"], "Developer");

    //        configDatabase.Views["View"].Fields["HideFilter"].Category = displayCategory;
    //        configDatabase.Views["View"].Fields["HidePager"].Category = displayCategory;
    //        configDatabase.Views["View"].Fields["DataRowView"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["DataRowView"], "Developer");
    //        configDatabase.Views["View"].Fields["Popup"].Category = displayCategory;
    //        configDatabase.Views["View"].Fields["DisplayType"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["DisplayType"], "Developer");
    //        configDatabase.Views["View"].Fields["DuplicateMessage"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["DuplicateMessage"], "Developer");
    //        configDatabase.Views["View"].Fields["HideSearch"].Category = displayCategory;
    //        configDatabase.Views["View"].Fields["HideToolbar"].Category = displayCategory;
    //        configDatabase.Views["View"].Fields["CollapseFilter"].Category = displayCategory;


    //        //configDatabase.Views["Field"].Fields["DisplayName"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["HideInTable"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["HideInEdit"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["HideInCreate"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["DisableInEdit"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["DisableInCreate"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["HideInFilter"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["Order"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["OrderForCreate"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["OrderForCreate"], "Developer");
    //        configDatabase.Views["Field"].Fields["OrderForEdit"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["OrderForEdit"], "Developer");
    //        configDatabase.Views["Field"].Fields["Sortable"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["DefaultValue"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["DefaultValue"], "Developer");
    //        configDatabase.Views["Field"].Fields["ColSpanInDialog"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["ColSpanInDialog"], "Developer");
    //        configDatabase.Views["Field"].Fields["HideInTableMobile"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["HideInTableMobile"], "Developer");
    //        configDatabase.Views["Field"].Fields["ChildrenHtmlControlType"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["ChildrenHtmlControlType"], "Developer");
    //        configDatabase.Views["Field"].Fields["MultiFilter"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["SelectionSortColumn"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["SelectionSortColumn"], "Developer");
    //        configDatabase.Views["Field"].Fields["DefaultFilter"].Category = displayCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["DefaultFilter"], "Developer");
    //        configDatabase.Views["Field"].Fields["Required"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["Seperator"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["DisableInDuplicate"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["MinWidth"].Category = displayCategory;
    //        configDatabase.Views["Field"].Fields["DisplayField"].Category = displayCategory;

    //        Category bhCategory = new Category() { Name = "Behaviour", Ordinal = 20 };
    //        configDatabase.Views["View"].Fields["HideInMenu"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["HideInMenu"], "Developer");
    //        configDatabase.Views["View"].Fields["ExportToCsv"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["Print"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["AllowCreate"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["AllowEdit"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["AllowDuplicate"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["AllowDelete"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["MultiSelect"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["HasChildrenRow"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["HasChildrenRow"], "Developer");
    //        configDatabase.Views["View"].Fields["UseOrderForCreate"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["UseOrderForCreate"], "Developer");
    //        configDatabase.Views["View"].Fields["UseOrderForEdit"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["UseOrderForEdit"], "Developer");
    //        configDatabase.Views["View"].Fields["DefaultSort"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["DefaultSort"], "Developer");
    //        configDatabase.Views["View"].Fields["TabCache"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["TabCache"], "Developer");
    //        configDatabase.Views["View"].Fields["RefreshOnClose"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["RefreshOnClose"], "Developer");
    //        configDatabase.Views["View"].Fields["DuplicationMethod"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["View"].Fields["DuplicationMethod"], "Developer");
    //        configDatabase.Views["View"].Fields["ImportFromExcel"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["HistoryNotifyList"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["SaveHistory"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["GridEditable"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["GridEditableEnabled"].Category = bhCategory;
    //        configDatabase.Views["View"].Fields["GridEditable"].DefaultValue = true;
    //        configDatabase.Views["View"].Fields["GridEditableEnabled"].DefaultValue = true;
    //        configDatabase.Views["View"].Fields["MaxSubGridHeight"].Category = bhCategory;

    //        ((View)configDatabase.Views["View"]).ImportFromExcel = true;

    //        configDatabase.Views["Field"].Fields["GraphicProperties"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["ContainerGraphicProperties"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["ContainerGraphicProperties"], "Developer");
    //        configDatabase.Views["Field"].Fields["NoHyperlink"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["NoHyperlink"], "Developer");
    //        configDatabase.Views["Field"].Fields["InlineAdding"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["ExcludeInUpdate"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["ExcludeInInsert"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["IncludeInDuplicate"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["HideInDerivation"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["HideInDerivation"], "Developer");
    //        configDatabase.Views["Field"].Fields["Excluded"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["DisableInFilter"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["Rich"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["TrimSpaces"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["TrimSpaces"], "Developer");
    //        configDatabase.Views["Field"].Fields["Custom"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["Custom"], "Developer");
    //        configDatabase.Views["Field"].Fields["Category_Parent"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["Dialog"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["PartialLength"].Category = bhCategory;
    //        //configDatabase.Views["Field"].Fields["StringConversionFormat"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["ParentHtmlControlType"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["ParentHtmlControlType"], "Developer");

    //        configDatabase.Views["Field"].Fields["InlineEditing"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["Searchable"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["NoCache"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["NoCache"], "Developer");
    //        configDatabase.Views["Field"].Fields["LabelContentLayout"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["LabelContentLayout"], "Developer");
    //        configDatabase.Views["Field"].Fields["InlineDuplicate"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["InlineDuplicate"], "Developer");
    //        configDatabase.Views["Field"].Fields["InlineSearch"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["InlineSearch"], "Developer");
    //        configDatabase.Views["Field"].Fields["InlineSearchView"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["InlineSearchView"], "Developer");
    //        configDatabase.Views["Field"].Fields["AdvancedFilter"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["AllowDuplication"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["AllowDuplication"], "Developer");
    //        configDatabase.Views["Field"].Fields["EditInTableView"].Category = bhCategory;
    //        SetRoles(configDatabase.Views["Field"].Fields["EditInTableView"], "Developer");
    //        configDatabase.Views["Field"].Fields["GridEditable"].Category = bhCategory;
    //        configDatabase.Views["Field"].Fields["TextHtmlControlType"].Category = bhCategory;

    //        Category descCategory = new Category() { Name = "Description", Ordinal = 22 };
    //        configDatabase.Views["View"].Fields["NewButtonDescription"].Category = descCategory;
    //        configDatabase.Views["View"].Fields["EditButtonDescription"].Category = descCategory;
    //        configDatabase.Views["View"].Fields["DuplicateButtonDescription"].Category = descCategory;
    //        configDatabase.Views["View"].Fields["PromoteButtonDescription"].Category = descCategory;
            //if (configDatabase.Views["Field"].Fields.ContainsKey("OriginalParentRelatedFieldName"))
            //{
            //    configDatabase.Views["Field"].Fields["OriginalFieldName"].Category = devCategory;
            //    configDatabase.Views["Field"].Fields["OriginalParentRelatedFieldName"].Category = devCategory;

            //}

            //Category sysCategory = new Category() { Name = "System", Ordinal = 50 };
            //configDatabase.Views["View"].Fields["Views_Parent"].Category = sysCategory;

    //        Category emailCategory = new Category() { Name = "Email", Ordinal = 24 };
    //        configDatabase.Views["View"].Fields["Send"].Category = emailCategory;
    //        configDatabase.Views["View"].Fields["SendTo"].Category = emailCategory;
    //        configDatabase.Views["View"].Fields["SendTo"].DisplayName = "To";
    //        configDatabase.Views["View"].Fields["SendCc"].Category = emailCategory;
    //        configDatabase.Views["View"].Fields["SendCc"].DisplayName = "CC";
    //        configDatabase.Views["View"].Fields["SendSubject"].Category = emailCategory;
    //        configDatabase.Views["View"].Fields["SendSubject"].DisplayName = "Subject";
    //        configDatabase.Views["View"].Fields["SendTemplate"].Category = emailCategory;
    //        configDatabase.Views["View"].Fields["SendTemplate"].DisplayName = "Template";
    //        configDatabase.Views["View"].Fields["NotifyMessageKey"].Category = emailCategory;
    //        configDatabase.Views["View"].Fields["NotifySubjectKey"].Category = emailCategory;

    //        Category advCategory = new Category() { Name = "Advanced", Ordinal = 25 };
    //        configDatabase.Views["Field"].Fields["ShowDependencyInTable"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["RadioColumns"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["BooleanHtmlControlType"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["RadioOrientation"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["LoadForBlockTemplate"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["Unique"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["AutocompleteColumn"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["AutocompleteTable"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["AutocompleteSql"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["ContainerGraphicField"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["TableCellMinWidth"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["Refresh"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["SaveHistory"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["CheckListInTableLimit"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["Import"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["MultiValueParentViewName"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["MultiValueAdditionals"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["SeperatorTitle"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["SubGridExport"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["SubGridPlacement"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["DropDownValueField"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["DropDownDisplayField"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["CloneChildrenViewName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["Cached"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["UpdateParent"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["UpdateParentInGrid"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["SearchFilter"].Category = advCategory;
    //        configDatabase.Views["Field"].Fields["IsUnique"].Category = advCategory;
    //        //configDatabase.Views["Field"].Fields["DataType"].Category = advCategory;

    //        configDatabase.Views["View"].Fields["ShowUpDown"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["ColumnsInDialogPerCategory"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["PermanentFilter"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["BaseName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["ContainerGraphicProperties"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["RowColorColumnName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["GroupingFields"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["EditableTableName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["CreateDateColumnName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["ModifiedDateColumnName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["CreatedByColumnName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["ModifiedByColumnName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["PromoteButtonName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["NewButtonName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["EditButtonName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["DuplicateButtonName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["InsertButtonName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["DeleteButtonName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["ShowDisabledSteps"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["WorkFlowStepsFieldName"].Category = advCategory;
    //        configDatabase.Views["View"].Fields["AddItemsFieldName"].Category = advCategory;

    //        ColumnField baseName = (ColumnField)configDatabase.Views["View"].Fields["BaseName"];
    //        baseName.TextHtmlControlType = TextHtmlControlType.DropDown;
    //        baseName.MultiValueParentViewName = "View";
    //        baseName.DropDownValueField = "Name";
    //        baseName.DropDownDisplayField = "Name";
    //        baseName.DisplayName = "Base View";
    //        baseName.NoHyperlink = false;

    //        Category rolesCategory = new Category() { Name = "Permissions", Ordinal = 30 };
    //        //if (configDatabase.Views["View"].Fields.ContainsKey("WorkspaceID"))
    //        //    configDatabase.Views["View"].Fields["WorkspaceID"].Category = rolesCategory;
    //        configDatabase.Views["View"].Fields["DenyCreateRoles"].Category = rolesCategory;
    //        configDatabase.Views["View"].Fields["DenyEditRoles"].Category = rolesCategory;
    //        configDatabase.Views["View"].Fields["DenyDeleteRoles"].Category = rolesCategory;
    //        configDatabase.Views["View"].Fields["DenySelectRoles"].Category = rolesCategory;
    //        configDatabase.Views["View"].Fields["Precedent"].Category = rolesCategory;
    //        configDatabase.Views["View"].Fields["AllowCreateRoles"].Category = rolesCategory;
    //        configDatabase.Views["View"].Fields["AllowEditRoles"].Category = rolesCategory;
    //        configDatabase.Views["View"].Fields["AllowDeleteRoles"].Category = rolesCategory;
    //        configDatabase.Views["View"].Fields["AllowSelectRoles"].Category = rolesCategory;

    //        configDatabase.Views["Field"].Fields["AllowCreateRoles"].Category = rolesCategory;
    //        configDatabase.Views["Field"].Fields["AllowEditRoles"].Category = rolesCategory;
    //        configDatabase.Views["Field"].Fields["AllowSelectRoles"].Category = rolesCategory;
    //        configDatabase.Views["Field"].Fields["Precedent"].Category = rolesCategory;
    //        configDatabase.Views["Field"].Fields["DenyCreateRoles"].Category = rolesCategory;
    //        configDatabase.Views["Field"].Fields["DenyEditRoles"].Category = rolesCategory;
    //        configDatabase.Views["Field"].Fields["DenySelectRoles"].Category = rolesCategory;

    //        Category xmlCategory = new Category() { Name = "Xml", Ordinal = 35 };
    //        configDatabase.Views["View"].Fields["XmlElement"].Category = xmlCategory;
    //        configDatabase.Views["View"].Fields["XmlElement"].HideInTable = true;


    //        Category devCategory = new Category() { Name = "Developers", Ordinal = 40 };
    //        configDatabase.Views["View"].Fields["BaseViewName"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["Controller"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["IndexAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["SetLanguageAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["CreateAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["EditAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["GetJsonViewAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["DeleteAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["FilterAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["UploadAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["ExportToCsvAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["PrintAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["AutoCompleteAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["AutoCompleteController"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["InlineAddingDialogAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["EditOnlyAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["InlineAddingCreateAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["DeleteSelectionAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["CheckListAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["RefreshAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["InlineEditingDialogAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["InlineEditingEditAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["DuplicateAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["InlineDuplicateDialogAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["InlineDuplicateAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["InlineSearchDialogAction"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["BaseTableName"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["Derivation_Parent"].Category = devCategory;
    //        configDatabase.Views["View"].Fields["ChartInfo_Parent"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("DatabaseTableName"))
    //            configDatabase.Views["View"].Fields["DatabaseTableName"].Category = devCategory;

    //        configDatabase.Views["View"].Fields["CreateOnlyAction"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("EditRichAction"))
    //            configDatabase.Views["View"].Fields["EditRichAction"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("GetRichAction"))
    //            configDatabase.Views["View"].Fields["GetRichAction"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("GetSelectListAction"))
    //            configDatabase.Views["View"].Fields["GetSelectListAction"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("EditSelectionAction"))
    //            configDatabase.Views["View"].Fields["EditSelectionAction"].Category = devCategory;

    //        if (configDatabase.Views["View"].Fields.ContainsKey("AnotherRowLinkText"))
    //            configDatabase.Views["View"].Fields["AnotherRowLinkText"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("AnotherRowLinkFunction"))
    //            configDatabase.Views["View"].Fields["AnotherRowLinkFunction"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("JavaScripts"))
    //            configDatabase.Views["View"].Fields["JavaScripts"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("StyleSheets"))
    //            configDatabase.Views["View"].Fields["StyleSheets"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("IsImageTable"))
    //            configDatabase.Views["View"].Fields["IsImageTable"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("OrdinalColumnName"))
    //            configDatabase.Views["View"].Fields["OrdinalColumnName"].Category = devCategory;
    //        if (configDatabase.Views["View"].Fields.ContainsKey("ImageSrcColumnName"))
    //            configDatabase.Views["View"].Fields["ImageSrcColumnName"].Category = devCategory;


    //        configDatabase.Views["Field"].Fields["AttributesNames"].Category = xmlCategory;
    //        configDatabase.Views["Field"].Fields["AttributesNames"].HideInTable = true;
    //        configDatabase.Views["Field"].Fields["ExportToXml"].Category = xmlCategory;
    //        configDatabase.Views["Field"].Fields["ExportToXml"].HideInTable = true;
    //        configDatabase.Views["Field"].Fields["XmlElement"].Category = xmlCategory;
    //        configDatabase.Views["Field"].Fields["XmlElement"].HideInTable = true;
    //        configDatabase.Views["Field"].Fields["XmlFields"].Category = xmlCategory;
    //        configDatabase.Views["Field"].Fields["XmlFields"].HideInTable = true;

    //        configDatabase.Views["Field"].Fields["ID"].Category = devCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("AutocompleteMathing"))
    //            configDatabase.Views["Field"].Fields["AutocompleteMathing"].Category = devCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("AutocompleteFilter"))
    //            configDatabase.Views["Field"].Fields["AutocompleteFilter"].Category = devCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("LimitToStartAutocomplete"))
    //            configDatabase.Views["Field"].Fields["LimitToStartAutocomplete"].Category = devCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("Localize"))
    //            configDatabase.Views["Field"].Fields["Localize"].Category = devCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("BaseFieldName"))
    //            configDatabase.Views["Field"].Fields["BaseFieldName"].Category = devCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("EnumType"))
    //            configDatabase.Views["Field"].Fields["EnumType"].Category = devCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("NullString"))
    //            configDatabase.Views["Field"].Fields["NullString"].Category = devCategory;

    //        if (configDatabase.Views["Field"].Fields.ContainsKey("Upload_Parent"))
    //            configDatabase.Views["Field"].Fields["Upload_Parent"].Category = devCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("Milestone_Parent"))
    //        {
    //            configDatabase.Views["Field"].Fields["Milestone_Parent"].Category = advCategory;
    //            configDatabase.Views["Field"].Fields["Milestone_Parent"].HideInTable = true;
    //        }
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("DependencyFieldName"))
    //            configDatabase.Views["Field"].Fields["DependencyFieldName"].Category = devCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("InsideTriggerFieldName"))
    //            configDatabase.Views["Field"].Fields["InsideTriggerFieldName"].Category = devCategory;

    //        if (configDatabase.Views["Field"].Fields.ContainsKey("CounterInitiated"))
    //            configDatabase.Views["Field"].Fields["CounterInitiated"].Category = devCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("Counter"))
    //            configDatabase.Views["Field"].Fields["Counter"].Category = devCategory;

    //        //if (configDatabase.Views["Field"].Fields.ContainsKey("RelatedViewName"))
    //        //    configDatabase.Views["Field"].Fields["RelatedViewName"].Category = devCategory;

    //        configDatabase.Views["Field"].Fields["SpecialColumn"].Category = devCategory;

    //        if (configDatabase.Views["Field"].Fields.ContainsKey("Format"))
    //            configDatabase.Views["Field"].Fields["Format"].Category = devCategory;

    //        if (configDatabase.Views["Field"].Fields.ContainsKey("Min"))
    //            configDatabase.Views["Field"].Fields["Min"].Category = devCategory;

    //        if (configDatabase.Views["Field"].Fields.ContainsKey("Max"))
    //            configDatabase.Views["Field"].Fields["Max"].Category = devCategory;

    //        configDatabase.Views["Field"].Fields["PartFromUniqueIndex"].Category = devCategory;
    //        configDatabase.Views["Field"].Fields["BrowserAutocomplete"].Category = devCategory;
    //        configDatabase.Views["Field"].Fields["Integral"].Category = devCategory;
    //        //configDatabase.Views["Field"].Fields["DatabaseNames"].Category = devCategory;

    //        Category sysCategory = new Category() { Name = "System", Ordinal = 50 };
    //        configDatabase.Views["View"].Fields["Views_Parent"].Category = sysCategory;

    //        //if (configDatabase.Views["Field"].Fields.ContainsKey("Fields_Parent"))
    //        //    configDatabase.Views["Field"].Fields["Fields_Parent"].Category = sysCategory;
    //        if (configDatabase.Views["Field"].Fields.ContainsKey("FieldType"))
    //            configDatabase.Views["Field"].Fields["FieldType"].Category = sysCategory;

    //        if (configDatabase.Views["Rule"].Fields.ContainsKey("Rules_Parent"))
    //            configDatabase.Views["Rule"].Fields["Rules_Parent"].DisplayName = "View";
    //        if (configDatabase.Views["Rule"].Fields.ContainsKey("DataAction"))
    //            ((ColumnField)configDatabase.Views["Rule"].Fields["DataAction"]).EnumType = typeof(Durados.TriggerDataAction).AssemblyQualifiedName;
    //        if (configDatabase.Views["Rule"].Fields.ContainsKey("WorkflowAction"))
    //            ((ColumnField)configDatabase.Views["Rule"].Fields["WorkflowAction"]).EnumType = typeof(Durados.WorkflowAction).AssemblyQualifiedName;
    //        if (configDatabase.Views["Rule"].Fields.ContainsKey("WhereCondition"))
    //            ((ColumnField)configDatabase.Views["Rule"].Fields["WhereCondition"]).TextHtmlControlType = TextHtmlControlType.TextArea;
    //        if (configDatabase.Views["Rule"].Fields.ContainsKey("Parameters_Children"))
    //        {
    //            configDatabase.Views["Rule"].Fields["Parameters_Children"].HideInTable = false;
    //            configDatabase.Views["Rule"].Fields["Parameters_Children"].Order = -10;
    //        }
    //        configDatabase.Views["Rule"].DisplayName = "Workflow Rules";

    //        if (configDatabase.Views["Rule"].Fields.ContainsKey("Views"))
    //        {
    //            ColumnField additionalViews = (ColumnField)configDatabase.Views["Rule"].Fields["Views"];
    //            additionalViews.MultiValueParentViewName = "View";
    //            additionalViews.DisplayName = "Additional View";
    //            additionalViews.DropDownValueField = "Name";
    //            additionalViews.TextHtmlControlType = TextHtmlControlType.CheckList;
    //        }

    //        configDatabase.Views["Rule"].UseOrderForCreate = true;
    //        configDatabase.Views["Rule"].UseOrderForEdit = true;
    //        configDatabase.Views["Rule"].Fields["Name"].OrderForCreate = 10;
    //        configDatabase.Views["Rule"].Fields["Name"].OrderForEdit = 10;
    //        configDatabase.Views["Rule"].Fields["Name"].ColSpanInDialog = 2;
    //        configDatabase.Views["Rule"].Fields["Name"].GraphicProperties = "LongText";
    //        configDatabase.Views["Rule"].Fields["Rules_Parent"].OrderForCreate = 20;
    //        configDatabase.Views["Rule"].Fields["Rules_Parent"].OrderForEdit = 20;
    //        configDatabase.Views["Rule"].Fields["DataAction"].OrderForCreate = 30;
    //        configDatabase.Views["Rule"].Fields["DataAction"].OrderForEdit = 30;
    //        if (configDatabase.Views["Rule"].Fields.ContainsKey("Views"))
    //        {
    //            configDatabase.Views["Rule"].Fields["Views"].OrderForCreate = 40;
    //            configDatabase.Views["Rule"].Fields["Views"].OrderForEdit = 40;
    //        }
    //        configDatabase.Views["Rule"].Fields["WorkflowAction"].OrderForCreate = 50;
    //        configDatabase.Views["Rule"].Fields["WorkflowAction"].OrderForEdit = 50;
    //        configDatabase.Views["Rule"].Fields["DatabaseViewName"].OrderForCreate = 60;
    //        configDatabase.Views["Rule"].Fields["DatabaseViewName"].OrderForEdit = 60;
    //        configDatabase.Views["Rule"].Fields["UseSqlParser"].OrderForCreate = 70;
    //        configDatabase.Views["Rule"].Fields["UseSqlParser"].OrderForEdit = 70;
    //        configDatabase.Views["Rule"].Fields["WhereCondition"].OrderForCreate = 80;
    //        configDatabase.Views["Rule"].Fields["WhereCondition"].OrderForEdit = 80;
    //        configDatabase.Views["Rule"].Fields["WhereCondition"].ColSpanInDialog = 2;

    //        configDatabase.Views["Parameter"].ColumnsInDialog = 1;
    //        ((ColumnField)configDatabase.Views["Parameter"].Fields["Name"]).TextHtmlControlType = TextHtmlControlType.TextArea;
    //        ((ColumnField)configDatabase.Views["Parameter"].Fields["Name"]).CssClass = "exwtextareawide";
    //        ((ColumnField)configDatabase.Views["Parameter"].Fields["Value"]).TextHtmlControlType = TextHtmlControlType.TextArea;
    //        ((ColumnField)configDatabase.Views["Parameter"].Fields["Value"]).CssClass = "exwtextareawide";

    //        //Category hideCategory = new Category() { Name = "Hide", Ordinal = 30 };
    //        //configDatabase.Views["Field"].Fields["HideInEdit"].Category = hideCategory;
    //        //configDatabase.Views["Field"].Fields["HideInCreate"].Category = hideCategory;
    //        //configDatabase.Views["Field"].Fields["HideInTable"].Category = hideCategory;
    //        //configDatabase.Views["Field"].Fields["HideInFilter"].Category = hideCategory;

    //        foreach (View view in configDatabase.Views.Values)
    //        {
    //            foreach (Field field in view.Fields.Values)
    //            {
    //                System.Reflection.PropertyInfo property = field.GetType().GetProperty(field.Name);
    //                if (property != null)
    //                {
    //                    object[] attributes = property.GetCustomAttributes(typeof(Durados.Config.Attributes.PropertyAttribute), true);
    //                    if (attributes.Length == 1)
    //                    {
    //                        string description = ((Durados.Config.Attributes.PropertyAttribute)attributes[0]).Description;
    //                        field.Description = description;
    //                    }
    //                }
    //            }
    //        }

    //        foreach (View view in configDatabase.Views.Values)
    //            view.HideInMenu = false;

    //        //configDatabase.Views["Cron"].HideInMenu = true;

    //    }

    //    public virtual void ConfigLocalization(Database localizationDatabase)
    //    {
    //        //localizationDatabase.Menu.Name = "Localization";
    //        localizationDatabase.DenyLocalizationConfigRoles = "Manager,User";

    //        localizationDatabase.Views["Durados_Language"].DisplayColumn = "Name";
    //        localizationDatabase.Views["Durados_Language"].DenyCreateRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_Language"].DenyDeleteRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_Language"].DenyEditRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_Language"].DenySelectRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_Language"].DisplayName = "Language";
    //        localizationDatabase.Views["Durados_Language"].HideInMenu = false;

    //        ((Durados.Web.Mvc.View)localizationDatabase.Views["Durados_Language"]).Controller = "Localization";

    //        localizationDatabase.Views["Durados_Language"].Fields["FK_Durados_Translation_Durados_Language_Children"].DisplayName = "Dictionary Keys";
    //        localizationDatabase.Views["Durados_Language"].Fields["NativeName"].DisplayName = "Native Name";
    //        ((ColumnField)localizationDatabase.Views["Durados_Language"].Fields["Direction"]).EnumType = typeof(Durados.Localization.LocalizationConfig.DirectionType).AssemblyQualifiedName;


    //        localizationDatabase.Views["Durados_TranslationKey"].DisplayColumn = "Key";
    //        localizationDatabase.Views["Durados_TranslationKey"].DenyCreateRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_TranslationKey"].DenyDeleteRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_TranslationKey"].DenyEditRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_TranslationKey"].DenySelectRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_TranslationKey"].DisplayName = "Keys";
    //        localizationDatabase.Views["Durados_TranslationKey"].HideInMenu = false;
    //        ((Durados.Web.Mvc.View)localizationDatabase.Views["Durados_TranslationKey"]).Controller = "Localization";

    //        localizationDatabase.Views["Durados_Translation"].DisplayColumn = "Translation";
    //        localizationDatabase.Views["Durados_Translation"].DenyCreateRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_Translation"].DenyDeleteRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_Translation"].DenyEditRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_Translation"].DenySelectRoles = localizationDatabase.DenyLocalizationConfigRoles;
    //        localizationDatabase.Views["Durados_Translation"].DisplayName = "Translation";
    //        localizationDatabase.Views["Durados_Translation"].HideInMenu = false;
    //        ((Durados.Web.Mvc.View)localizationDatabase.Views["Durados_Translation"]).Controller = "Localization";


    //        localizationDatabase.Views["Durados_Translation"].Fields["FK_Durados_Translation_Durados_TranslationKey_Parent"].DisplayName = "Key";
    //        localizationDatabase.Views["Durados_Translation"].Fields["FK_Durados_Translation_Durados_Language_Parent"].DisplayName = "Language";

    //        localizationDatabase.Views["Durados_Translation"].Fields["ID"].Order = 0;
    //        localizationDatabase.Views["Durados_Translation"].Fields["FK_Durados_Translation_Durados_TranslationKey_Parent"].Order = 1;
    //        localizationDatabase.Views["Durados_Translation"].Fields["Translation"].Order = 2;

    //    }
    //}

    public class Project : Durados.Web.Mvc.Config.Project
    {
        
    }
}
