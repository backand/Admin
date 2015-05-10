<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.MyCharts>" %>
    <style type="text/css">
	    .column { width: <%=100/Model.Columns%>%; float: left; padding-bottom: 0; min-height:100px; }
	    .portlet { margin: 0 1em 1em 0; }
	    .portlet-header { margin: 0.3em; padding-bottom: 4px; padding-left: 0.2em; }
	    .portlet-header .ui-icon { float: right; }
	    .portlet-content{ padding: 0.4em; }
	    .portlet-link A{ color: Blue !important; }
	    .portlet-header-right { margin: 0.3em; padding-bottom: 0px; padding-left: 0.2em; }
	    .ui-sortable-placeholder { border: 1px dotted black; visibility: visible !important; height: 50px !important; }
	    .ui-sortable-placeholder * { visibility: hidden; }
	   
	    body{overflow:auto;}
	</style>
<%
    string role = Database.GetUserRole();
    bool cancelDrag = role != "Admin" && role != "Developer";
    string cancelDragAtt = cancelDrag ? "cancelDrag='cancelDrag'" : string.Empty;
%>

<%  if (!Durados.Web.Infrastructure.General.IsMobile()) {%>

<%for(int i=1;i<=Model.Columns;i++){%>
<div class="column <%=(i==1)?"charts":string.Empty %>" <%=cancelDragAtt %>>
<% foreach (Durados.Chart chart in Model.Charts.Values.Where(c => (c.GetColumn() == i) && c.IsVisible(role)).OrderBy(c => c.Ordinal))
   { %>
    <div class="portlet" chartId="<%= "Chart" + chart.ID %>" >
		<div class="portlet-header"></div>
		<div class="portlet-content"><% Html.RenderPartial("~/Views/Shared/Controls/Chart.ascx", chart);%></div>
	</div>
	<%} %>

</div>
<%} %>
<%--
<div class="column">
<% foreach (Durados.Chart chart in Model.Charts.Values.Where(c => c.Align == Durados.ChartAlignment.Right && c.IsVisible(role)).OrderBy(c => c.Ordinal))
   { %>
    <div class="portlet" chartId="<%= "Chart" + chart.ID %>" >
		<div class="portlet-header"></div>
		<div class="portlet-content"><% Html.RenderPartial("~/Views/Shared/Controls/Chart.ascx", chart);%></div>
	</div>
	
<%} %>
</div>--%>
<%} else {%>
<div class="column" style="width: 100%;">
<% foreach (Durados.Chart chart in Model.Charts.Values.Where(c => c.IsVisible(role) && c.IsAllow()).OrderBy(c => c.Ordinal))
   { %>
    <div class="portlet" chartId="<%= "Chart" + chart.ID %>" >
		<div class="portlet-header"></div>
		<div class="portlet-content"><% Html.RenderPartial("~/Views/Shared/Controls/Chart.ascx", chart);%></div>
	</div>
	
<%} %>
</div>

<%} %>

`