<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>

<div class="dashboard">

<%
    Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];

    List<Durados.Link> links = Map.Database.GetDashboardLinks();    
    
    int columns = Map.Database.DashboardColumns;
    if (columns == 0)
        columns = 3;

    int rows = links.Count / columns;
    int remain = links.Count - rows * columns;
    if (remain > 0)
        rows+=1;
    
    int i = 0;
    int col = 1;
%>
<div class="column">

<% foreach (Durados.Link link in links)
   {  %>
    
    

    <% 
        string item = tableViewer.GetDashboardItem(link);
    %>
    
    <% if (item == null)
        {%>
      <% Html.RenderPartial("~/Views/Shared/Controls/DashboardItem.ascx", link);%>
       <% } else { %>
         <%= item %>
   <%} %>
         <% i++; %>
        <% int row = i % rows; %>
        <%
            if (row == 0)
                row = rows;
        %>
   <% 
   int x = 0;
   if (remain > 0 && col > remain)
           x =1;
   %>
       
    <% if (row == rows - x)
       { %>    
    </div>
    <div class="column">
        <% col++; %>
    <%} %>
    
<%} %>


</div>
    

<%--<div class="column">

	<div class="portlet">
		<div class="portlet-header">Feeds</div>
		<div class="portlet-content">Lorem ipsum dolor sit amet, consectetuer adipiscing elit</div>
	</div>
	
	<div class="portlet">
		<div class="portlet-header">News</div>
		<div class="portlet-content">Lorem ipsum dolor sit amet, consectetuer adipiscing elit</div>
	</div>

</div>

<div class="column">

	<div class="portlet">
		<div class="portlet-header">Shopping</div>
		<div class="portlet-content">Lorem ipsum dolor sit amet, consectetuer adipiscing elit</div>
	</div>

</div>

<div class="column">

	<div class="portlet">
		<div class="portlet-header">Links</div>
		<div class="portlet-content">Lorem ipsum dolor sit amet, consectetuer adipiscing elit</div>
	</div>
	
	<div class="portlet">
		<div class="portlet-header">Images</div>
		<div class="portlet-content">Lorem ipsum dolor sit amet, consectetuer adipiscing elit</div>
	</div>--%>

</div>
