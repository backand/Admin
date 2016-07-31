<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.DataActionFields>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>
<%@ Import Namespace="Durados.Web.Mvc.Infrastructure"%>


<div class="page-settings-header">
    <span><%= Map.Database.Localizer.Translate("Settings")%></span>
    <a href='#' class="page-settings-done" title="<%= Map.Database.Localizer.Translate("Close")%>">
        <span class="pages-close">close</span>
    </a>
</div>

<div class="page-button-container page-settings-toolbar">
<a href="#" class="page-settings-action page-settings-delete"><span class='button-icon'></span><span class="button-text"><%=Map.Database.Localizer.Translate("Delete Page")%></span></a></div>

<%--<a class="page-settings-action page-settings-duplicate"><%=Map.Database.Localizer.Translate("Duplicate") %></a>--%>

<div class="dialogTitle page-settings">
    
<div class="settings-group-header">Main Settings</div>
<table class="settings-group">
<tr>
<td>Page Name:</td>
</tr>
<tr>
<td><input name="PageName" type="text"/></td>
</tr>
<tr>
<td><input name="HideFromMenu" type="checkbox"/> Hide from menu</td>
</tr>
<tr>
<td><input name="Homepage" type="checkbox"/> Set as homepage</td>
</tr>
</table>
            
<%  Html.RenderPartial("~/Views/Shared/Controls/Page/ViewSettings.ascx", Model); %>
</div>