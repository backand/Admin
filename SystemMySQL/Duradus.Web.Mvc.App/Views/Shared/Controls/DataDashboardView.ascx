<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataView>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json"%>

<% try {

    string guid = Model.Table.ExtendedProperties["guid"].ToString();

    bool mainPage = Model.Table.ExtendedProperties.ContainsKey("mainPage")? (bool)Model.Table.ExtendedProperties["mainPage"] : false;
       
    bool isViewHaveGantt = false;
    

    Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()];
    string viewSafety = view.Name + "_safety";
    bool safetyMode = false;
    if (Map.Session[viewSafety] != null){
        safetyMode = (bool)Map.Session[viewSafety];
    }
    //Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.TableName];
    string pk = (ViewData["pk"] == null) ? string.Empty : ViewData["pk"].ToString();

    string textSearch = Map.Database.Localizer.Translate("Search on text...");
    string search = ViewData["search"] as string; if (string.IsNullOrEmpty(search)) search = textSearch;
    int rowCount = Convert.ToInt32(ViewData["rowCount"]); %>
<%=Html.Hidden("pk", pk, new { id = guid + "pk" })%>
<%  string rootPath = Durados.Web.Mvc.Infrastructure.General.GetRootPath();

    Durados.Web.Mvc.UI.Styler styler = ViewData["Styler"] == null ? new Durados.Web.Mvc.UI.Styler(view, Model) : (Durados.Web.Mvc.UI.Styler)ViewData["Styler"];    
  
    Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];
    tableViewer.DataView = Model;
  
    Durados.Web.Mvc.UI.ColumnsExcluder columnsExcluder = ViewData["ColumnsExcluder"] == null ? null : (Durados.Web.Mvc.UI.ColumnsExcluder)ViewData["ColumnsExcluder"];    
    Dictionary<string,Durados.Field> excludedColumn = columnsExcluder.ExcludedColumns;
    bool hidePager = view.HidePager && !view.IsInAdminMode();
    bool hideFilter = view.HideFilter && !view.IsInAdminMode();
    
    bool hasSearch =!string.IsNullOrEmpty(search) && search != textSearch;
    bool hasFilter = false;
    if (ViewData["filter"] != null)
    {
        foreach (Field jsonFilter in ((Dictionary<string, Field>)ViewData["filter"]).Values)
        {
            if (jsonFilter.Value != null && jsonFilter.Value.ToString() != string.Empty && !jsonFilter.Permanent)
            {
                hasFilter = true;
                break;
            }            
        }
    }
    string filterClass = (hasFilter || hasSearch) ? "filterOn" : "filterOff";
%>
<style type="text/css">
div.gridboard div.boardrow
{
    <% if (!string.IsNullOrEmpty(view.DashboardWidth)){ %>
    width: <%=view.DashboardWidth %>px;
    <%} %>
    <% if (!string.IsNullOrEmpty(view.DashboardHeight)){ %>
    height: <%=view.DashboardHeight %>px;
    <%} %>
}
<% if (!string.IsNullOrEmpty(view.DashboardHeight)){ %>
div.gridboard td
{
    white-space: nowrap !important;
}
<%} %>
</style>
<!-- TOOLBAR -->
    <%Html.RenderPartial("~/Views/Shared/Controls/Toolbar/Toolbar.ascx", Model);

 

//Handle Filter
      if (!hideFilter)
{
    if (view.FilterType == Durados.FilterType.Group)
    {
        Html.RenderPartial("~/Views/Shared/Controls/Filter/GroupFilter.ascx", view);
    }
    //else if (view.FilterType == Durados.FilterType.Tree)
    //{
    //    Html.RenderPartial("~/Views/Shared/Controls/Filter/TreeFilterSelectedValues.ascx", view);
    //}
}
//Handle Sort
if (view.SortingType == Durados.SortingType.Group)
{
    ViewData["guid"] = guid;
    Html.RenderPartial("~/Views/Shared/Controls/Sort/GroupSorting.ascx", view);
}
string unselectable = safetyMode ? " unselectable='on'" : "";      
%>
<div class="fixedViewPort" d_fix="<%=guid %>" view="dashboard">
<table class="gridview" cellspacing="0" rowCount='<%=rowCount %>' pageSize='<%=view.PageSize %>' <%=unselectable %>>              
<tbody>
<tr class="dashboardTr">
<td style="width:100% !important; padding:0; margin:0">
<div class="gridboard">              
                <!-- DATA -->
                <% int rowIndex = 0;                
                   
                   System.Data.DataView dataView = tableViewer.GetDataView(Model, view, guid);
                   if (!(view is Durados.Web.Mvc.Config.View))
                   {
                       Model.RowStateFilter = DataViewRowState.ModifiedOriginal;
                       if (Model.Count == 0)
                       {
                           Model.RowStateFilter = System.Data.DataViewRowState.Unchanged | System.Data.DataViewRowState.Added | System.Data.DataViewRowState.ModifiedCurrent;
                       }
                   }
                   foreach (System.Data.DataRowView row in Model)
                   {
                    string pkValue = view.GetPkValue(row.Row);
                    rowIndex++;
                    
                    string editClick = string.Empty;
                    string url = Url.Action("Item", view.Controller, new { viewName = view.Name, pk = pkValue, guid = view.Name + "_Item_" });
                    editClick += "d_edit('" + pkValue + "', " + view.Popup.ToString().ToLower() + ", " + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + view.Name + "', '" + guid + "', this)";

                    string displayValue = Html.Encode(view.GetDisplayValue(row.Row));

                    //string rowCss = styler.GetRowCss(view, row.Row, rowIndex);
                    %>
                    <div class='boardrow' id='d_row_<%=guid + pkValue %>' d_row='d_row' d_displayValue="<%= displayValue%>"  ondblclick="<%=editClick%>" <%= tableViewer.ShowRowHover(view, row.Row) ? "onMouseOver=\"$(this).addClass('hovered')\" onMouseOut=\"$(this).removeClass('hovered')\"" : "" %> <%=unselectable %>>
                    <div class="boardtitle" guid='<%=guid %>' onclick="Multi.BoardClicked(event, this, '<%=guid %>')" <%=unselectable %>  d_pk='<%=pkValue %>'><%= displayValue %>
                    
                    <%    
                        //if (tableViewer.SelectorCheckbox(view, row.Row))
                           //{
                    string editCaption = tableViewer.GetEditCaption(view, row.Row, guid);
                    string editTitle = tableViewer.GetEditTitle(view, row.Row);
                            %>
                            <span><a href="#" onclick="<%=editClick %>;return false" title="<%= Map.Database.Localizer.Translate(editTitle)%>"><span class="edit-chart-icon"></span><%=Map.Database.Localizer.Translate(editCaption)%></a></span>
                     <%     
                         //}
                               %>
                    
                    </div>
                    <table>
                    <%                                            
                       foreach (Durados.Field field in view.VisibleFieldsForTableFirstRow)
                       {
                           if (!field.Dashboard) continue;
                       
                           string value = Server.HtmlEncode(tableViewer.GetFieldValue(field, row.Row));
                           if (!string.IsNullOrEmpty(value))
                           {
                               value = value.Replace("\0", "");
                           }
                        
                            string alt = styler.GetAlt(field, row.Row, guid);
                       
                            alt = (string.IsNullOrEmpty(alt) ? string.Empty : " title='" + alt + "'");
                       
                            string tableViewElement = tableViewer.GetElementForTableView(field, row.Row, guid);
                            if (!string.IsNullOrEmpty(tableViewElement))
                            {
                                tableViewElement = tableViewElement.Replace("\0", "");
                            }
                            if (tableViewer.IsVisible(field, excludedColumn,guid))
                            {
                                 %><tr>
                                   <td <%=alt %><%=unselectable %>><b><%=tableViewer.GetDisplayName(field, null, guid)%></b></td><td><%=tableViewElement%></td></tr>
                          <%} 
                       }
                   
                       if (tableViewer.IsEditable(view))
                       { 
                           string[] pkArray = ViewData["pkArray"] == null ? null : (string[])ViewData["pkArray"];                          
                           bool isFirst = rowIndex == 1;
                           bool isLast = rowIndex == Model.Count;
                           string nextPK = string.Empty;
                           if (!isLast)
                           {
                               nextPK = view.GetPkValue(Model[rowIndex].Row);
                           }
                            string prevPK = "";
                            if (!isFirst)
                            {
                                prevPK = view.GetPkValue(Model[rowIndex - 2].Row);
                            }
                            string isFirstAttr = (isFirst) ? "isFirst='isFirst'" : "";
                            string isLastAttr = (isLast) ? "isLast='isLast'" : "";
                           %> <tr guid='<%=guid %>' mode='board' style="display: none"> <%
                           if (tableViewer.SelectorCheckbox(view))
                           { %>
                            <td colspan="2"><input type="checkbox" title="<%= Map.Database.Localizer.Translate("Select current record")%>" class="Multi" pk='<%=pkValue %>' prevPK='<%=prevPK %>' nextPK='<%=nextPK %>' <%=isFirstAttr %> <%=isLastAttr %> <%=pkArray != null && pkArray.Contains(pkValue) ? "checked='checked'" : "" %> /></td>
                           <% } 
                            //string editCaption = tableViewer.GetEditCaption(view, row.Row, guid);
                            //string editTitle = tableViewer.GetEditTitle(view, row.Row);
                            
                        %>
                    </tr>
                <% }  %> 
                     </table>
                    </div>          
            <% } %>
</div>
</td> 
</tr>
</tbody>
</table>
</div>
        <% if (!hidePager)
           {
            Html.RenderPartial("~/Views/Shared/Controls/Pager.ascx", Model.Table);
           }
       %> 
<%   
} catch(Exception exception){ %>
    <span><%= "$$error start$$ " + exception.Message + " $$error end$$"%></span>
<%} %>