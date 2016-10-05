<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<%-- R E A D    T H I S:  Use the MainContent placeholder for the content. Do not put any elements outside this placeholder! --%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        body{overflow:auto;}
    </style>
        <%=Html.Hidden("a", Durados.Web.Mvc.Infrastructure.General.GetRootPath(), new { id = "GetRootPath" })%>
        <%=Html.Hidden("a", Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(new Durados.Web.Mvc.UI.Json.Translator(Map.Database)), new { id = "translator" })%>
    <script type="text/javascript">
        document.domain = '<%=Durados.Web.Mvc.Maps.Host %>';
    </script>
    <script  type="text/javascript">
        var rootPath = $('#GetRootPath').val();
        var d_MinColumnWidth = <%=Map.Database.MinColumnWidth %>;
        var translator = Sys.Serialization.JavaScriptSerializer.deserialize($('#translator').val());
    </script>

    <% if (Durados.Web.Mvc.Maps.Skin)
       { %>

    <%-- Menu start --%>
    <%  if (!Durados.Web.Infrastructure.General.IsMobile())
            Html.RenderPartial("~/Views/Shared/Controls/MainMenu.ascx"); %>
    
    <%-- Menu end --%>
    <%} %>

    <%if (Durados.Web.Infrastructure.General.IsMobile())
      { %>
        <%
          string onclick = ""; 
            Workspace workspace = MenuHelper.GetCurrentWorkspace();
            string workspaceName = workspace.Name;
            workspaceName += " " + Map.Database.Localizer.Translate("Workspace");
        %>
        <% onclick = "mobileMenu('Workspaces')"; %> 
            
      <div class="refer-bar">
      <a href="#" class="mobile-menu-icon" onclick="<%=onclick %>" >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</a><%=workspaceName %>
    <%if(string.IsNullOrEmpty(Map.Database.GetCurrentUsername() ) || Map.Database.GetCurrentUsername() == Durados.Database.GuestUsername) {%>
                 <nav id="topRight">
				<% Html.RenderPartial("~/Views/Shared/Controls/LogOnUserControl.ascx", null); %>
				
			</nav>
       
    <%} %>
      <div class="mobileMenuContent" style="display:none;">
        <% Html.RenderPartial("~/Views/Shared/Controls/MobileMenu.ascx"); %>
        </div>
        </div>
    <%} %>
    <br /><br />
    
    <div id="mainAppDiv">
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
    </div>
</asp:Content>

 <%--Placeholder for stylesheets (css) --%>
<asp:Content ID="Content2" ContentPlaceHolderID="headCSS" runat="server">
</asp:Content>

<%--Placeholder for javascript files --%>
<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
</asp:Content>
