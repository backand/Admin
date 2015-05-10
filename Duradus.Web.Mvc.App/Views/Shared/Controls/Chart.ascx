<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Durados.Chart>" %>
<style>
div.chart-Table-container{overflow:hidden;margin:5px;rgba(0, 0, 0, 0.180392);}
</style>
<%
    string height = "340";
    if (Model.Height != null && Model.Height != 0)
        height = Model.Height.ToString();
%>
<div>
    <div class="dialog" title="<%=Model.Name %>">
   
        <div id="<%= "Chart" + Model.ID %>" dashboard="<%=ViewData["DashboardId"] %>" class="chart" style="height: <%= height%>px; margin: 0 auto; "></div>
        
        <div id="<%= "Chart" + Model.ID +"Table" %>" dashboard="<%=ViewData["DashboardId"] %>" class="chart-Table-container" style="width:99%;"></div>
    </div>
</div>



