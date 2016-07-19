<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Page>" %>
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
        document.title = '<%=Model.Title %>';
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
        <div class="mobileMenuContent" style="display:none;">
        <% Html.RenderPartial("~/Views/Shared/Controls/MobileMenu.ascx"); %>
        </div>
        <% Html.RenderPartial("~/Views/Shared/Controls/LogOnUserControl.ascx", null); %>
      </div>
    <%} %>
    <div id="mainAppDiv">
    <%  Html.RenderPartial("~/Views/Shared/Controls/Page.ascx", Model); %>
    
    </div>
</asp:Content>

 <%--Placeholder for stylesheets (css) --%>
<asp:Content ID="Content2" ContentPlaceHolderID="headCSS" runat="server">
</asp:Content>

<%--Placeholder for javascript files --%>
<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
</asp:Content>
