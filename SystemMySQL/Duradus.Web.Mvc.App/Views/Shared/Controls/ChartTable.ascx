<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<style type="text/css">
    table.chartTbl {table-layout:fixed;border-collapse:collapse;width:100%;}
    table.chartTbl th  {background: #1ba085;color: white; } 
    table.chartTbl th, table.chartTbl td{ border: 1px solid rgb(204,204,204);overflow:hidden;text-align:center;}
	
	</style>

    <%
        tblObject tblObject = (tblObject)Model;
     %>

     <% if (tblObject.Type == "table") { %>
     <center class="chart-table-title">
     <% = tblObject.Title %>
     </center>
 <center class="chart-table-subtitle">
     <% = tblObject.SubTitle %>
     </center>

     <%} %>
<table class="chartTbl">
    <tr class="ui-dialog-titlebar" style="table-layout: fixed">
        <% var cellWidth = 100 / tblObject.xAxis.Count; %>
        <% int MaxRows = 100;%>
        <% int MaxCols = 100;%>
        <% int index = 0;%>
        <th style="width:<%=cellWidth-2%>%;"></th>
        
        <%foreach (object colHeader in tblObject.xAxis)
          { %>
        <th style="width: <%=cellWidth%>%;">
            <%= colHeader.ToString()%>
        </th>
        <%index++; if (index > MaxCols) break;  %>
        <%} %>
    </tr>
    <%  int indexRow = 0;%>
    <%foreach (Durados.Web.Mvc.UI.Helpers.ChartSeries chartSeries in (List<Durados.Web.Mvc.UI.Helpers.ChartSeries>)tblObject.series)
      { %>
    <tr>
        <td class="chart-table-series-name">
            <%=chartSeries.name %>
        </td>
        <% int indexCol = 0;%>
        <%foreach (object data in chartSeries.data)
          {%>
        <td>
            <%=data.ToString()%>
        </td>
        <%indexCol++; if (indexCol > MaxRows) break;  %>
        <%}%>
    </tr>
    <%indexRow++; if (indexRow > MaxRows) break;  %>
    <%} %>
</table>
