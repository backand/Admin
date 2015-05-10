<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<% string viewName = (ViewData["viewName"] ?? string.Empty).ToString(); %>
<%
    int adminId = Map.Database.GetAdminWorkspaceId();
    List<Durados.Workspace> workspaces = Map.Database.Workspaces.Values.Where(w=>w.ID != adminId).ToList();
    Workspace currentWorkspace = MenuHelper.GetCurrentWorkspace(viewName);
    
%>
<div class="mobile-settings-menu">
    <ul>
        <% foreach (Workspace workspace in workspaces)
           {
               string selected = string.Empty;
               if (workspace.Equals(currentWorkspace))
                   selected = " selected='selected' ";
               %>
           <li><a href="#">
            <%=workspace.Name%></a>
            <ul <%=selected %> >
                <%  Html.RenderPartial("~/Views/Shared/Controls/MobileWorkspaceMenu.ascx", workspace); %>
            </ul>
        </li>

        <%} %>
    </ul>
</div>
