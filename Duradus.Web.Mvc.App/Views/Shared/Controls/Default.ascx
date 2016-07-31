<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<%
        string content = Convert.ToString(ViewData[Durados.Web.Mvc.Database.DefaultPageContentKey] ?? string.Empty);    
    %>
    <%=string.IsNullOrEmpty(content) ? "<h1>This is the first page</h1><h2>Place your content here</h2>" : content %>

    <br />
    <% string currentRole = Map.Database.GetUserRole(); %>
    <% if (currentRole == "Admin" || currentRole == "Developer")
       {%>
       <% Durados.Workspace workspace = Durados.Web.Mvc.UI.Helpers.MenuHelper.GetCurrentWorkspace(); %>
       <% int? workspaceId = null; %>
       <% if (workspace != null)
          { %>
          <% workspaceId = Durados.DataAccess.ConfigAccess.GetWorkspaceId(workspace.Name, Map.GetConfigDatabase().ConnectionString); %>
       <%} %>
       <% if (workspaceId.HasValue)
          { %>
          <a class="edit-workspace-content" href="#" onclick="EditView.open(<%=workspaceId.Value %>, 'Workspace', 'Workspace Content'); return false;"><%= Map.Database.Localizer.Translate("Click here to edit the content of this page")%></a>
      <%-- <%= Html.ActionLink(Map.Database.Localizer.Translate("Click here to edit the content of this page"), "Index", "Admin", new { viewName = "Workspace", pk = workspaceId.Value }, null)%>--%>
       <%}
          else
          { %>
          <%
              if (string.IsNullOrEmpty(CmsHelper.GetHtml(Durados.Web.Mvc.Database.DefaultPageContentKey)))
              {
                  CmsHelper.SetHtml(Durados.Web.Mvc.Database.DefaultPageContentKey, "<h1>This is the first page</h1><br><h2>Place your content here</h2>");
              }

              
               %>
          <a class="edit-workspace-content" href="#" onclick="EditView.open(<%=Durados.Web.Mvc.Database.DefaultPageContentKey %>, 'durados_Html', 'Default Content'); return false;"><%= Map.Database.Localizer.Translate("Click here to edit the content of this page")%></a>
         <%-- <%= Html.ActionLink(Map.Database.Localizer.Translate("Click here to edit the content of this page"), "EditDefaultContent", "Admin")%>--%>
       <%} %>
    <%} %>