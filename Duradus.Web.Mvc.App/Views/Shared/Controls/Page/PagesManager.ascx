<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Workspace>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<% string viewName = (ViewData["viewName"] ?? string.Empty).ToString(); %>
<%
    Workspace workspace = Model ?? MenuHelper.GetCurrentWorkspace(viewName);
    if (workspace.ID == Map.Database.GetAdminWorkspaceId())
    {
        Map.Session["workspaceId"] = Map.Database.GetPublicWorkspaceId();
        workspace = Map.Database.GetPublicWorkspace();
    }
%>

<div class="page-settings-header pages-header">
    <span><%= Map.Database.Localizer.Translate("Pages")%></span>
    <%--<a onclick="openAddPageDialog()"><span class='add-page-button-icon' title="<%=Map.Database.Localizer.Translate("Add Page") %>"></span><span class='add-page-button' ></span></a>--%>
    <a href='#' class="pages-done" title="<%= Map.Database.Localizer.Translate("Close")%>">
        <span class="pages-close">close</span>
    </a>
</div>

<div class="page-button-container page-settings-toolbar page-settings-toolbar-add-page">
<a onclick="openAddPageDialog()" class="page-settings-action page-settings-add"><span class='add-page-button-icon'></span><span class="button-text"><%=Map.Database.Localizer.Translate("Add Page")%></span></a></div>


<div class="pages-instructions">
<%= Map.Database.Localizer.Translate("Navigate through your pages and change the page order. To rename a page, click the wheel icon.")%>
</div>

<% if (workspace != null)
   { %>
<%  Html.RenderPartial("~/Views/Shared/Controls/Page/WorkspaceManager.ascx", workspace); %>

<div class="pages-manager-wrapper">
    <%  Html.RenderPartial("~/Views/Shared/Controls/Page/WorkspaceMenuManager.ascx", workspace); %>
    
</div>
 
<%} %>