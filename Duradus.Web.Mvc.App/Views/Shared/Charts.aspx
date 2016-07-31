<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <script type="text/javascript">
        $(function () {
            Charts.run();
        });
	</script>
    <div id="Charts" dir="ltr" >
    <%
    string userRole = Map.Database.GetUserRole();
    string editChartCaption = "";
    if (userRole == "Admin" || userRole == "Developer")
        editChartCaption = "Edit";
     %>

    <%=Html.Hidden("a", Durados.Web.Mvc.Infrastructure.General.GetRootPath(), new { id = "GetRootPath" })%>
    <%=Html.Hidden("a", Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(new Durados.Web.Mvc.UI.Json.Translator(Map.Database)), new { id = "translator" })%>
    <%=Html.Hidden("a", Map.Database.Localizer.Translate(editChartCaption), new { id = "editChartCaption", fieldType = "isEnabled" })%>
    
    <%  if (!Durados.Web.Infrastructure.General.IsMobile() && Durados.Web.Mvc.Maps.Skin)
            Html.RenderPartial("~/Views/Shared/Controls/MainMenu.ascx"); %>
    
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
    <br /> <br />
    <div id="mainAppDiv">
    <%  Html.RenderPartial("~/Views/Shared/Controls/Charts.ascx", Map.Database.MyCharts); %>
    </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headCSS" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
</asp:Content>
