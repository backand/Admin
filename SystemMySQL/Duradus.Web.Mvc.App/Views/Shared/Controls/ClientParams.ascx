<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.ClientParams>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json"%>

<% Durados.Web.Mvc.View view = Model.View; %>
<% string guid = Model.Guid; %>
<% string addItemUrl = Url.Action("Item", view.Controller, new { viewName = view.Name, add = "yes", guid = view.Name + "_Item_" }); %>
<% string editItemUrl = Url.Action("Item", view.Controller, new { viewName = view.Name, guid = view.Name + "_Item_" }); %>
<% string duplicateItemUrl = Url.Action("Item", view.Controller, new { viewName = view.Name, add = "yes", guid = view.Name + "_Item_" }); %>
<% string insertItemUrl = Url.Action("Item", view.Controller, new { viewName = view.Name, add = "yes", guid = view.Name + "_Item_" }); %>
<% string editOnlyUrl = Url.Action("EditOnly", view.Controller, new { viewName = view.Name }); %>
<%=Html.Hidden("a", view.Name, new { id = guid + "ViewName" })%>
<%=Html.Hidden("a", view.GetCreateUrl(), new { id = guid + "GetCreateUrl" })%>
<%=Html.Hidden("a", view.GetEditUrl(), new { id = guid + "GetEditUrl" })%>
<%=Html.Hidden("a", view.GetEditSelectionUrl(), new { id = guid + "GetEditSelectionUrl" })%>
<%=Html.Hidden("a", view.GetDuplicateUrl(), new { id = guid + "GetDuplicateUrl" })%>
<%=Html.Hidden("a", view.GetJsonViewUrl(), new { id = guid + "GetJsonViewUrl" })%>
<%=Html.Hidden("a", view.GetRichUrl(), new { id = guid + "GetRichUrl" })%>
<%=Html.Hidden("a", view.GetEditRichUrl(), new { id = guid + "GetEditRichUrl" })%>
<%=Html.Hidden("a", view.GetSelectListUrl(), new { id = guid + "GetSelectListUrl" })%>
<%=Html.Hidden("a", view.GetJsonViewInlineAddingUrl(), new { id = guid + "GetJsonViewInlineAddingUrl" })%>
<%=Html.Hidden("a", view.GetJsonViewInlineEditingUrl(), new { id = guid + "GetJsonViewInlineEditingUrl" })%>
<%=Html.Hidden("a", view.GetInlineAddingDialogUrl(), new { id = guid + "GetInlineAddingDialogUrl" })%>
<%=Html.Hidden("a", view.GetInlineEditingDialogUrl(), new { id = guid + "GetInlineEditingDialogUrl" })%>
<%=Html.Hidden("a", view.GetInlineDuplicateDialogUrl(), new { id = guid + "GetInlineDuplicateDialogUrl" })%>
<%=Html.Hidden("a", view.GetInlineSearchDialogUrl(), new { id = guid + "GetInlineSearchDialogUrl" })%>
<%=Html.Hidden("a", view.GetDeleteUrl(), new { id = guid + "GetDeleteUrl" })%>
<%=Html.Hidden("a", view.GetDeleteSelectionUrl(), new { id = guid + "GetDeleteSelectionUrl" })%>
<%=Html.Hidden("a", view.GetFilterUrl(), new { id = guid + "GetFilterUrl" })%>
<%=Html.Hidden("a", view.GetSetLanguageUrl(), new { id = guid + "GetSetLanguageUrl" })%>
<%=Html.Hidden("a", view.GetIndexUrl(), new { id = guid + "GetIndexUrl" })%>
<%=Html.Hidden("a", view.GetExportToCsvUrl(), new { id = guid + "GetExportToCsvUrl" })%>
<%=Html.Hidden("a", view.GetPrintUrl(), new { id = guid + "GetPrintUrl" })%>
<%=Html.Hidden("a", view.GetAutoCompleteUrl(), new { id = guid + "GetAutoCompleteUrl" })%>
<%=Html.Hidden("a", view.GetLocalizedDisplayName(), new { id = guid + "DisplayName" })%>
<%=Html.Hidden("a", view.Name, new { id = guid + "viewName" })%>
<%=Html.Hidden("a", ViewData["jsonView"].ToString(), new { id = guid + "jsonViewForCreate" })%>
<%=Html.Hidden("a", view.GetUploadUrl(), new { id = guid + "GetUploadUrl" })%>
<%=Html.Hidden("a", view.GetJsonView(Durados.DataAction.Edit, guid), new { id = guid + "jsonViewForEdit" })%>
<%=Html.Hidden("a", view.GetJsonFilter(guid), new { id = guid + "GetJsonFilter" })%>
<%=Html.Hidden("a", view.Controller, new { id = guid + "Controller" })%>
<%=Html.Hidden("a", view.TabCache, new { id = guid + "TabCache" })%>
<%=Html.Hidden("a", view.TabCache, new { id = guid + "RefreshOnClose" })%>
<%=Html.Hidden("a", view.GetRefreshUrl(), new { id = guid + "GetRefreshUrl" })%>
<%=Html.Hidden("a", Model.MainPage, new { id = guid + "mainPage" })%>
<%=Html.Hidden("a", view.MultiSelect, new { id = guid + "MultiSelect" })%>
<%=Html.Hidden("a", view.Popup, new { id = guid + "Popup" })%>
<%=Html.Hidden("a", view.IsEditable(guid), new { id = guid + "AllowEdit" })%>
<%=Html.Hidden("a", Durados.Web.Infrastructure.General.IsMobile(), new { id = guid + "Mobile" })%>
<%=Html.Hidden("a", addItemUrl, new { id = guid + "AddItemUrl" })%>
<%=Html.Hidden("a", editItemUrl, new { id = guid + "EditItemUrl" })%>
<%=Html.Hidden("a", duplicateItemUrl, new { id = guid + "DuplicateItemUrl" })%>
<%=Html.Hidden("a", insertItemUrl, new { id = guid + "InsertItemUrl" })%>
<%=Html.Hidden("a", view.DuplicationMethod, new { id = guid + "DuplicationMethod" })%>
<%=Html.Hidden("a", view.DuplicateMessage, new { id = guid + "DuplicateMessage" })%>
<%=Html.Hidden("a", view.PageSize, new { id = guid + "PageSize" })%>
<%=Html.Hidden("a", view.DisplayType, new { id = guid + "DisplayType" })%>
<%=Html.Hidden("a", Map.Database.Localizer.Translate(view.PromoteButtonName), new { id = guid + "PromoteButton" })%>
<%=Html.Hidden("a", Map.Database.Localizer.Translate(view.NewButtonName), new { id = guid + "addTitle" })%>
<%=Html.Hidden("a", Map.Database.Localizer.Translate(view.DuplicateButtonName), new { id = guid + "dupTitle" })%>
<%=Html.Hidden("a", view.ShowUpDown, new { id = guid + "ShowUpDown" })%>
<%=Html.Hidden("a", editOnlyUrl, new { id = guid + "EditOnlyUrl" })%>
<%=Html.Hidden("a", view.RowColorColumnName, new { id = guid + "RowColorColumnName" })%>
<%=Html.Hidden("a", view.HasOpenRules, new { id = guid + "HasOpenRules" })%>
<%=Html.Hidden("a", view.MaxSubGridHeight, new { id = guid + "MaxSubGridHeight" })%>
<%=Html.Hidden("a", view.AllowCreate, new { id = guid + "AllowCreate" })%>
<%=Html.Hidden("a", view.WorkFlowStepsFieldName, new { id = guid + "WorkFlowStepsFieldName" })%>
<%=Html.Hidden("a", view.ShowDisabledSteps, new { id = guid + "showDisabledSteps" })%>
<%=Html.Hidden("a", view.ReloadPage, new { id = guid + "ReloadPage" })%>
<%=Html.Hidden("a", view.GetAllFilterValuesUrl(), new { id = guid + "GetAllFilterValuesUrl" })%>
<%=Html.Hidden("a", Database.GetUserRole(), new { id = guid + "Role" })%>
<%=Html.Hidden("a", view.OpenDialogMax, new { id = guid + "OpenDialogMax" })%>
<%=Html.Hidden("a", Map.Database.SqlProduct, new { id = guid + "SqlProduct" })%>
<%string viewAsDisplayType = view.Name + "_" + guid + "_dataDisplayType";
  Durados.DataDisplayType displayType = Map.Session[viewAsDisplayType] != null ? (Durados.DataDisplayType)Map.Session[viewAsDisplayType] : Durados.DataDisplayType.Table;%>

<%=Html.Hidden("a", displayType, new { id = guid + "DataDisplayType" })%>
<%=Html.Hidden("a", view.GridDisplayType, new { id = guid + "GridDisplayType" })%>
<%=Html.Hidden("a", view.Database.InsideTextSearch, new { id = guid + "InsideTextSearch" })%>
