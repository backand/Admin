<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        document.domain = '<%=Durados.Web.Mvc.Maps.Host %>';
    </script>
    
    <script type="text/javascript">
        $(function () {
            Charts.startDashboard(<%=ViewData["DashboardId"] %>);

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
    <br /> 
   
    <div id="mainAppDiv">
    <%  string qstr = ChartHelper.GetAddChartQueryString();
       // else
        //    qstr = "&menuId=";   
            %>
     <% if (userRole == "Admin" || userRole == "Developer")
        {%>
     <div class="charts-top-bar">
<div class="charts-top-bar-left"><a class="add-new-chart-btn" href="/Admin/AddChart?dashboardId=<%=ViewData["dashboardId"] %><%=qstr %>","><span class="icon add-chart"></span>Add Chart</a></div><%--onclick="AddNewChartToDashboard(<%=ViewData["dashboardId"] %>)";--%>
 <div class="charts-top-bar-right"><label for ="chartsColumns"><%=Map.Database.Localizer.Translate("Number of Columns")%>:</label>
 <select id="chartColumns">
 <%for (int i = 1; i <= 4; i++)
   { %>

<option value='<%=i %>' <%=(Map.Database.Dashboards[Convert.ToInt32(ViewData["DashboardId"])].Columns==i)?"selected=selected":string.Empty %> ><%=i%></option><%} %>

</select></div></div> <%} %>

    <%  Html.RenderPartial("~/Views/Shared/Controls/Charts.ascx", Map.Database.Dashboards[Convert.ToInt32(ViewData["DashboardId"])]); %>
    </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headCSS" runat="server">
<style type="text/css">
        .add-new-chart-btn{padding-left:26px;position:relative;top:3px;color: rgb(74,74,74);font-family: arial;font-weight:bold;vertical-align:middle;}
	    .charts-top-bar{display: inline-block;position: relative;width: 100%;height:26px;clear:both;background:rgb(241,241,241);margin-bottom:10px;color:#4c4c4c;}
	    .charts-top-bar-right{float:right;margin-right: 1.5em; font-size:13px;}
	    .charts-top-bar-left{float:left; margin-left: 0.3em; padding-bottom: 4px; padding-left: 0.2em; font:13px bold;}
	    .icon.add-chart{background: url(/Content/Images/ui-sprite.png) no-repeat 0px 0px;height:20px;width:25px;display:block;position:absolute;}
	    .add-new-chart-btn:hover .icon.add-chart{background-position: 0px -99px;}
	    .add-new-chart-btn:hover {color:#f26522;}
	   #Charts .ui-widget-header, .ui-dialog-titlebar{background:#f3b858;color: rgb(74,74,74);}
	     #Charts .ui-chart {color: rgb(74,74,74) !important;}
	     .edit-chart .button-icon{visibility: visible;margin-left: -24px;background-position: -50px 0px;}
	     .portlet-header:hover .edit-chart .button-icon{visibility: visible;margin-left: -24px;background-position: -50px 0px;}
	     
	     .ui-dialog-buttonset > button.ui-button {
background: rgba(27, 160, 133, 1);
color: white;
font-weight: bold;
border: none;
}
        </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
</asp:Content>