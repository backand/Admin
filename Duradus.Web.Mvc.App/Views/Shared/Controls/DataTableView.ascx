<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataView>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json"%>
<% try {

    string guid = Model.Table.ExtendedProperties["guid"].ToString();
    ViewData["guid"] = guid;
    bool mainPage = Model.Table.ExtendedProperties.ContainsKey("mainPage") ? (bool)Model.Table.ExtendedProperties["mainPage"] : false;
    bool isViewHaveGantt = false;
    Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()];
    bool strechViewPort = view.GridDisplayType == Durados.GridDisplayType.FitToWindowWidth; ;
    string viewAsDisplayType = view.Name + "_" + guid + "_dataDisplayType";

    Durados.DataDisplayType displayType = Map.Session[viewAsDisplayType] != null ? (Durados.DataDisplayType)Map.Session[viewAsDisplayType] : view.DataDisplayType;
    //if ((Request.QueryString["subGrid2"] != null && Request.QueryString["subGrid2"].Equals("yes")) || (ViewData["subGrid2"] != null && ViewData["subGrid2"].Equals("yes")))
       
    //{
    //    displayType = Durados.DataDisplayType.Table;
    //}
    ViewData["displayType"] = displayType;

    //by br4--
    if (displayType == Durados.DataDisplayType.Dashboard)
        //if (Map.Session[viewAsDisplayType] != null && (bool)Map.Session[view.Name + "_" + guid + "_dashboard"])
    {
        Html.RenderPartial(view.DataDashboardView, Model);
        return;
    }
    else if(displayType == Durados.DataDisplayType.Preview)
    {
        Html.RenderPartial("~/Views/Shared/Controls/Preview/DataPreviewView.ascx", Model);
        return;
    }
       
    string viewSafety = view.Name + "_safety";
    bool safetyMode = false;
    if (Map.Session[viewSafety] != null){
        safetyMode = (bool)Map.Session[viewSafety];
    }
    string pk = (ViewData["pk"] == null) ? string.Empty : ViewData["pk"].ToString();

    string textSearch = Map.Database.Localizer.Translate("Search on text...");
    string search = ViewData["search"] as string; if (string.IsNullOrEmpty(search)) search = textSearch;
    string viewFilterVisibility = view.Name + "_filterVisibility";
    bool hideFilter = view.HideFilter && !view.IsInAdminMode();
    bool hidePager = view.HidePager && !view.IsInAdminMode();
    bool collapseFilter = view.HideToolbar ? false : view.CollapseFilter || (Map.Session[viewFilterVisibility] != null && !(bool)Map.Session[viewFilterVisibility]);
    if (Durados.Web.Infrastructure.General.IsMobile())
        collapseFilter = true;
    //bool collapseFilter = view.CollapseFilter || (Map.Session[viewFilterVisibility] != null && !(bool)Map.Session[viewFilterVisibility]);
      
    int rowCount = Convert.ToInt32(ViewData["rowCount"]); %>
<%=Html.Hidden("pk", pk, new { id = guid + "pk" })%>
<%    Html.RenderPartial("~/Views/Shared/Controls/Style.ascx", Model);%>
<%  string rootPath = Durados.Web.Mvc.Infrastructure.General.GetRootPath();

    Durados.Web.Mvc.UI.Styler styler = ViewData["Styler"] == null ? new Durados.Web.Mvc.UI.Styler(view, Model) : (Durados.Web.Mvc.UI.Styler)ViewData["Styler"];    
  
    Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];
    tableViewer.DataView = Model;
  
    Durados.Web.Mvc.UI.ColumnsExcluder columnsExcluder = ViewData["ColumnsExcluder"] == null ? null : (Durados.Web.Mvc.UI.ColumnsExcluder)ViewData["ColumnsExcluder"];    
    Dictionary<string,Durados.Field> excludedColumn = columnsExcluder.ExcludedColumns;

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
    
    Durados.Web.Mvc.UI.Json.CustomView cv = Durados.Web.Mvc.UI.Helpers.ViewHelper.GetDesrializedCustomView(view.Name);

    //Init variables in viewData for inline partial views
    ViewData["filterClass"] = filterClass;

//<!-- TOOLBAR -->
    Html.RenderPartial("~/Views/Shared/Controls/Toolbar/Toolbar.ascx", Model);
  
string gridEditableEnabledAttr = view.GridEditable && safetyMode ? "d_editMode='on'" : "d_editMode='off'";
string unselectable = safetyMode ? " unselectable='on'" : "";

bool showCheckboxes = Request.QueryString["checkbox"] == "true" || Session["checkbox" + guid] == "true";
if (Request.QueryString["checkbox"] == "true")
{
    Session["checkbox" + guid] = "true";
}

string selectionCheckboxClass = tableViewer.IsEditable(view, guid) || showCheckboxes ? "" : "hideCheckboxes";

   
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

%>

<% bool displayAdminContextMenu = (bool)(!MenuHelper.HideSettings()) && view.IsAdmin() && !view.Database.IsConfig; %>
<% string contextMenuAttr = displayAdminContextMenu ? "contextMenu=\"1\"" : ""; %>
<%
       bool free = Map.Database.Plan == 3 && !view.IsEditable(guid);
       bool multiSelect = tableViewer.SelectorCheckbox(view) || showCheckboxes;
       int rowCountInPage = Model.Count;
    %>
    
<div class="fixedviewportWrapper">
<div class="fixedViewPort<%= strechViewPort? " strechViewPort":"" %>" d_fix="<%=guid %>">
<table class="gridview" cellspacing="0" rowCount='<%=rowCount %>' rowCountInPage='<%=rowCountInPage %>' pageSize='<%=view.PageSize %>' <%=gridEditableEnabledAttr+unselectable %> openSingleRow='<%=view.OpenSingleRow %>' multiSelect='<%=multiSelect %>' hasCustomView='<%=cv!=null %>'>
<thead>
            <!-- FILTER -->
            <% if (!hideFilter && view.FilterType == Durados.FilterType.Excel)
            {%>        
            <tr class='rowfilter' name='rowfilter'<%= collapseFilter? " style='display:none'" : "" %>>
               <% if (tableViewer.IsEditable(view)){%>
                    <td colspan="2" class="tablecmdhead edittd <%=selectionCheckboxClass %>"><div><span class='<%=filterClass%>'>
        <%=Map.Database.Localizer.Translate("Filter:")%></span></div></td>
               <%}
                  List<Durados.Field> visibleFieldsForTableFirstRow = view.VisibleFieldsForTableFirstRow;
                   foreach (Durados.Field field in visibleFieldsForTableFirstRow)
                   { 
                       if (tableViewer.IsVisible(field, excludedColumn,guid))
                       { 
                           string colStyle= "style=\"" + tableViewer.GetCellWidthStyle(field, cv, false, "header") + "\"";
                            
                   
                       if (field.IsVisibleForFilter())
                       {
                       
                        object filterValue = (ViewData["filter"] == null) ? null : (((Dictionary<string, Field>)ViewData["filter"]).ContainsKey(field.Name)) ? ((Dictionary<string, Field>)ViewData["filter"])[field.Name].Value : null; 
                        
                           if (field.FieldType == Durados.FieldType.Column && ((Durados.Web.Mvc.ColumnField)field).IsMilestonesField ) 
                           {
                               isViewHaveGantt = true;
                               tableViewer.Gantt.Init((Durados.Web.Mvc.ColumnField)field, filterValue);
                    %>
                    <td><div class="GridFilterDiv ganttFilter"><%= field.GetElementForFilter(tableViewer.Gantt.FilterText, guid)%></div></td>
                    <%                               
                           } else { 
                               if (filterValue != null && filterValue.ToString() == search) {
                                   filterValue = "";
                               }
                    %>
                    <td><div class="GridFilterDiv"><%= field.GetElementForFilter(filterValue, guid)%></div></td>
                    <%                           
                           }
                       }
                       else
                       {%>
                    <td><div></div></td>
                    <%}
                    } 
                  } %>
                  <%if (!strechViewPort) 
                    {%>
                <td class="lastTD"><div class="lastTD">&nbsp;</div></td>
                <%} %>
            </tr>
            <%}
            if (view.DisplayType != Durados.DisplayType.Chart) { %>            
                <!-- COLUMNS HEADER -->
                <tr class="gridviewhead" >
                    <% if (tableViewer.IsEditable(view)){%>
                    <% if (multiSelect)
                           { %>
                     <th class="tablecmdhead thCheck <%=selectionCheckboxClass %>"><div class="thdiv">
                        <!-- SELECT ALL -->
                        <input type="checkbox" safari='1' onchange="Multi.Toggle(this,'<%=guid %>');return false"
                            title="<%=view.GetActionTooltipDescription(Durados.ToolbarActionNames.SELECT_ALL)%>"/>
                           
                       
                    </div></th>
                    <th  class="tablecmdhead thAll <%=selectionCheckboxClass %>" ><div class="thdiv"><span class="sortable col-header"><%=Map.Database.Localizer.Translate("All")%></span></div></th>
                   <%} else{%>
                   <th  class="tablecmdhead <%=selectionCheckboxClass %>" style="border-right-width:0;"><div class="thdiv"></div></th>
                   <th  class="tablecmdhead thAll2 <%=selectionCheckboxClass %>"><div class="thdiv"><span class="filler-icon"></span></div></th>
                    
                    <%} %>
                   <%--<th class="tablecmdhead"><div class="thdiv"></div></th>--%>
                    <%}
                       int i = 0; foreach (Durados.Field field in view.VisibleFieldsForTableFirstRow)
                       { 
                         if (tableViewer.IsVisible(field, excludedColumn,guid))
                         {
                             string colStyle = "style=\"" + tableViewer.GetCellWidthStyle(field, cv, false, "header") + tableViewer.GetCellAlignmentStyle(field, cv, false, "data") + "\""; 
                            string headerTitle =  tableViewer.GetDisplayName(field, null, guid);
                            string dataTypeClass = field.DataType.ToString() + "FieldType";
                            
                             if (field.FieldType == Durados.FieldType.Column && ((Durados.Web.Mvc.ColumnField)field).IsMilestonesField) 
                             {
                                   //object filterValue = (ViewData["filter"] == null) ? null : (((Dictionary<string, Field>)ViewData["filter"]).ContainsKey(field.Name)) ? ((Dictionary<string, Field>)ViewData["filter"])[field.Name].Value : null;
                                    %>                                    
                                    <th class="ganttHeaders <%=dataTypeClass%>" SortColumn="<%=field.Name%>" unselectable="on"><%=tableViewer.Gantt.GetHeadersRow()%></th>
                                    <%
                             }
                             else if (!field.ShowColumnHeader)
                             {%>
                                 <th  class="NotSortable <%=dataTypeClass%>" SortColumn='<%=field.Name%>'></th>
                             <%}
                             else if (tableViewer.IsSortable(field, guid))
                             {
                                 i++;
                                 string description = string.IsNullOrEmpty(field.Description) ? headerTitle : field.Description;
                                 string sortId = "sort_" + guid + i.ToString();
                                   
                                  %>
                                    <th <%=contextMenuAttr %> class="<%=dataTypeClass%>" SortColumn='<%=field.Name%>'><div <%=colStyle %> class="thdiv"><a id='<%=sortId %>' href="#" class="Sortable col-header" SortColumn='<%=field.Name%>' onclick="FilterForm.Sort('<%=sortId%>', '<%=guid %>')" title='<%=description %>'><%=headerTitle%></a> 
                                        <%if(!string.IsNullOrEmpty(description) && String.Compare(description,headerTitle,true)>0){ %>
                                        <i class="icon-info_outline" title="<%=description %>"></i>
                                        <%} %>
                                        <%= (ViewData["SortColumn"] != null && ViewData["SortColumn"].ToString() == field.Name) ? "<div class='sortIcon-con'><img alt='' title='" + ViewData["direction"].ToString() + "ending' src='" + Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/icon_sort_" + ViewData["direction"].ToString() + ".GIF' class='sortIcon grid' /></div>" : ""%></div>
                                        <%= displayAdminContextMenu && !view.SystemView ? "<span class=\"desc-icon\"></span>" : string.Empty%> 
                                       
                                    </th>
                            <%}
                             else
                             { %>
                                <th <%=contextMenuAttr %>  class="NotSortable <%=dataTypeClass%>" SortColumn='<%=field.Name%>'><div  <%=colStyle %> class="thdiv">
                                    <% if (field.FieldType == Durados.FieldType.Children && ((Durados.Web.Mvc.ChildrenField)field).ChildrenHtmlControlType != Durados.Web.Mvc.ChildrenHtmlControlType.CheckList)
                                       { }
                                       else
                                       { %>
                                    <span class="col-header" ><%=headerTitle%></span>
                                    <%} %>
                                </div><%= displayAdminContextMenu && !view.SystemView ? "<span class=\"desc-icon\"></span>" : string.Empty%></span></th>
                            <%}
                          }
                       } %>
                    <%if (!strechViewPort) 
                    {%>
                    <th class="lastTD"><div class="lastTD">&nbsp;</div></th>
                    <%} %>
                </tr>
            <% } %>
</thead>                
<tbody>                
                <!-- DATA -->
                <% int rowIndex = 0;                
                   Dictionary<string, object> groupingValues = null;
                   Durados.Field[] groupingFields = view.GetGroupingFields();
                   bool showGroup = view.DisplayType != Durados.DisplayType.Table && groupingFields != null && groupingFields.Count() > 0 && ViewData["SortColumn"] != null && ViewData["SortColumn"].ToString() == groupingFields[0].Name;
                   if (showGroup)
                   {
                       groupingValues = new Dictionary<string, object>();
                       
                       foreach (Durados.Field field in groupingFields)
                       {
                           groupingValues.Add(field.Name, null);
                       }
                   }
                   
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
                    string pkHtmlValue = Server.HtmlEncode(pkValue);
                    string pkUrlValue = Server.UrlEncode(pkValue);
                       
                    rowIndex++;
                    
                    string editClick = string.Empty;
                    string url = Url.Action("Item", view.Controller, new { viewName = view.Name, pk = pkUrlValue, guid = view.Name + "_Item_" });
                    editClick += "d_edit('" + pkHtmlValue + "', " + view.Popup.ToString().ToLower() + ", " + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + view.Name + "', '" + guid + "', this)";

                    string displayValue = Html.Encode(view.GetDisplayValue(row.Row));

                    string hiddenAttributes = view.GetHiddenAttributes(row.Row); 

                    string rowCss = styler.GetRowCss(view, row.Row, rowIndex);
                    if (showGroup){
                    bool equalGroupValues = view.CompareGroupingFields(groupingValues, row.Row);
                    if (!equalGroupValues)
                    { %>
                        <tr class='groupingRow' d_group='d_group' <%=unselectable %>>
                        <% foreach (Durados.Field field in view.VisibleFieldsForTableFirstRow)
                           {
                                if (tableViewer.IsVisible(field, excludedColumn,guid))
                                { 
                                string colStyle= "style=\"" + tableViewer.GetCellWidthStyle(field, cv, false, "grouping") + "\"";
                                if (groupingValues.ContainsKey(field.Name)) {%>
                                <td class='groupingTd' d_group='d_group'><div <%=colStyle %>><%=field.GetElementForTableView(row.Row, guid)%></div></td>
                                <%} else {%>
                                <td class='groupingTd' d_group='d_group'><div></div></td>
                                <%}
                              }
                          } %>                        
                        </tr>
                    <%}
                    } %>
                    <tr class='data-row <%= rowCss%>' style='<%=tableViewer.GetRowHeightStyle(view) %>' id='d_row_<%=guid + pkHtmlValue %>' guid='<%=guid %>' d_row='d_row' d_pk='<%=pkHtmlValue %>' d_displayValue="<%= displayValue%>" <%=hiddenAttributes %>  <%= tableViewer.IsEditOnDblClick(view) ? "ondblclick=\"" + editClick + "\"" : "" %>  <%= tableViewer.ShowRowHover(view, row.Row) ? "onMouseOver=\"$(this).addClass('hovered')\" onMouseOut=\"$(this).removeClass('hovered')\"" : "" %> <%=unselectable %>>
                    <% if (tableViewer.IsEditable(view))
                       { 
                           string[] pkArray = ViewData["pkArray"] == null ? null : (string[])ViewData["pkArray"];
                          
                           bool isFirst = rowIndex == 1;
                           bool isLast = rowIndex == Model.Count;
                           string nextPK = string.Empty;
                           if (!isLast)
                           {
                               nextPK = Server.HtmlEncode(view.GetPkValue(Model[rowIndex].Row));
                           }
                            string prevPK = "";
                            if (!isFirst)
                            {
                                prevPK = Server.HtmlEncode(view.GetPkValue(Model[rowIndex - 2].Row));
                            }
                            string isFirstAttr = (isFirst) ? "isFirst='isFirst'" : "";
                            string isLastAttr = (isLast) ? "isLast='isLast'" : "";
                        %>
                        <td class="tablecmd <%=selectionCheckboxClass %>">
                        <%--<% string checkboxStyle = string.Empty;
                            if (!tableViewer.SelectorCheckbox(view))
                            {
                                checkboxStyle = @"style=""display:none;""";
                            } %>
                            <div <%=checkboxStyle %>>--%>
                            <div>
                            <input type="checkbox" safari='1' title="<%= Map.Database.Localizer.Translate("Select current record")%>" class="Multi" pk='<%=pkHtmlValue %>' prevPK='<%=prevPK %>' nextPK='<%=nextPK %>' <%=isFirstAttr %> <%=isLastAttr %> <%=pkArray != null && pkArray.Contains(pkValue) ? "checked='checked'" : "" %> />
                        </div></td>
                        <td class="tablecmd <%=selectionCheckboxClass %>"><div class="line-edit">
                        <%
                            string editCaption = tableViewer.GetEditCaption(view, row.Row, guid);
                            string editTitle = tableViewer.GetEditTitle(view, row.Row);
                        %><a href="#" onclick="<%=editClick %>" title="<%= Map.Database.Localizer.Translate(editTitle)%>"><span class="<%=editCaption%>-icon"></span></a></div></td>
                    <%}
                    
                       foreach (Durados.Field field in view.VisibleFieldsForTableFirstRow)
                       {
                           try
                           {
                               string value = null;
                               try
                               {
                                   value = Server.HtmlEncode(tableViewer.GetFieldValue(field, row.Row));
                               }
                               catch (Exception exception)
                               {
                                   if (field.FieldType == Durados.FieldType.Children && exception is NullReferenceException)
                                   {
                                       ((ChildrenField)field).ChildrenHtmlControlType = ChildrenHtmlControlType.Grid;
                                       try
                                       {
                                           value = Server.HtmlEncode(tableViewer.GetFieldValue(field, row.Row));
                                       }
                                       catch
                                       {
                                           value = "Err!";
                                       }
                                   }
                                   else
                                   {
                                       value = "Err!";
                                   }
                               }
                               if (!string.IsNullOrEmpty(value))
                               {
                                   value = value.Replace("\0", "");
                               }

                               bool checklist = field.FieldType == Durados.FieldType.Children && ((Durados.Web.Mvc.ChildrenField)field).ChildrenHtmlControlType == Durados.Web.Mvc.ChildrenHtmlControlType.CheckList;

                               bool hasFormat = field.FieldType == Durados.FieldType.Column && !string.IsNullOrEmpty(((Durados.Web.Mvc.ColumnField)field).Format);
                               bool hasCss = !string.IsNullOrEmpty(field.ContainerGraphicField) && field.Name == field.ContainerGraphicField;

                               bool refreshCell = hasFormat;
                               string d_refresh = refreshCell ? " d_refresh=\"d_refresh\" " : string.Empty;
                               string d_hasCss = hasCss ? " d_hasCss=\"d_hasCss\" " : string.Empty;

                               bool editable = tableViewer.IsEditable(field, row.Row, guid) && !(field.FieldType == Durados.FieldType.Parent && ((Durados.Web.Mvc.ParentField)field).ParentHtmlControlType == Durados.Web.Mvc.ParentHtmlControlType.Hidden);

                               string disabledCell = string.Empty;
                               if (tableViewer.IsEditable(view, guid) && view.GridEditable && safetyMode)
                               {
                                   disabledCell = editable ? "enabledCell" : "disabledCell";
                               }
                               bool derivationEditable = tableViewer.IsDerivationEditable(field, row.Row, guid);
                               bool textArea = field.FieldType == Durados.FieldType.Column && ((Durados.Web.Mvc.ColumnField)field).TextHtmlControlType == Durados.Web.Mvc.TextHtmlControlType.TextArea;
                               string updateParent = field.UpdateParentInGrid ? " updateParent='updateParent' " : string.Empty;
                               bool isHoverEditIconEnabled = tableViewer.IsHoverEditIconEnabled(field, row.Row, guid);
                               
                               string gridEditorAttributes = editable ? updateParent + "d_role=\"cell\" d_val=\"" + value + "\" d_guid=\"" + guid + "\" d_name=\"" + field.Name + "\" d_pk=\"" + pkHtmlValue + "\" textArea=\"" + (textArea == true ? "textArea" : "").ToString() + "\" " + (isHoverEditIconEnabled? "hoverEdit=\"1\" ": "").ToString() + (checklist ? "limit=\"" + ((Durados.Web.Mvc.ChildrenField)field).CheckListInTableLimit + "\"" : "").ToString() + " d_derivation=\"" + (derivationEditable ? "" : "disabled") + "\" " + d_refresh + d_hasCss : " d_val=\"" + value + "\" d_guid=\"" + guid + "\" d_name=\"" + field.Name + "\" ";
                               string alt = styler.GetAlt(field, row.Row, guid);
                               alt = (string.IsNullOrEmpty(alt) ? string.Empty : "alt='" + alt + "' title='" + alt + "'").ToString();
                               string tableViewElement = tableViewer.GetElementForTableView(field, row.Row, guid);
                               string dataTypeClass = field.DataType.ToString() + "FieldType";
                               if (!string.IsNullOrEmpty(tableViewElement))
                               {
                                   tableViewElement = tableViewElement.Replace("\0", "");
                               }
                               if (tableViewer.IsVisible(field, excludedColumn, guid))
                               {
                                   if (showGroup && groupingValues.ContainsKey(field.Name))
                                   { %>
                                <td class='<%=styler.GetCellCss(field, row.Row, guid) %>' <%=unselectable %>><div></div></td>
                           <%}
                                   else
                                   {
                                       
                                       string colStyle = "style=\"" + tableViewer.GetCellWidthStyle(field, cv, editable, "data") + tableViewer.GetCellAlignmentStyle(field, cv, editable, "data") + tableViewer.GetCellHeightStyle(field, cv, editable, "data") + "\"";
                                     
                                       if (field.FieldType == Durados.FieldType.Column && ((Durados.Web.Mvc.ColumnField)field).IsMilestonesField)
                                       {
                                           //object filterValue = (ViewData["filter"] == null) ? null : (((Dictionary<string, Field>)ViewData["filter"]).ContainsKey(field.Name)) ? ((Dictionary<string, Field>)ViewData["filter"])[field.Name].Value : null;
                                 %>
                                 <td class='ganttDataRow' unselectable="on"><%=tableViewer.Gantt.GetMilestonesRow(row.Row)%></td>
                                 <% }
                                       else
                                       { %>
                                <td <%=gridEditorAttributes %> onmousedown="Durados.FieldEditor.Show(this, event);" class='<%=styler.GetCellCss(field, row.Row, guid) %> <%=disabledCell %> <%=dataTypeClass %>' <%=alt %><%=unselectable %>><div <%=colStyle %> onmouseover="try{Durados.GridHandler.showContentAsTooltip(this);}catch(ex){}"><%=tableViewElement%></div></td>
                            <%}
                                   }
                               }
                           }
                           catch (Exception fieldException)
                           {
                               %>
                               <td>Error</td>
                               <%
                               Map.Logger.Log("DataTableView", "DataTableView", "Field", fieldException, 1, "Field: " + field.Name);
                               
                           }
                       }%>
                    <%if (!strechViewPort) 
                    {%>
                    <td class="lastTD"><div class="lastTD">&nbsp;</div></td>
                    <%} %>
                    </tr>                
                <%} %>

</tbody>
</table>

<% if (free)
   { %>
<img alt="BackAnd logo" class="freeBg" src="/Content/Images/wm.png" onclick="" style="position:absolute;right:12px;bottom:2px;z-index:10000;display:none;overflow:hidden;cursor:pointer;"/>
    <script type="text/javascript">
        $(document).ready(function () {
            //add onclick event to the watermark
            $('img.freeBg').click(function () {
                window.open('https://www.BackAnd.com/index.aspx?utm_source=wix&utm_medium=banner&utm_campaign=wm', 'BackAnd', 'width=1450,height=820');
            });
        });
    </script>
<%} %>
</div>
</div>
        <% 
            if (!hidePager && view.DisplayType != Durados.DisplayType.Chart)// && !hidePager)
           {
            Html.RenderPartial("~/Views/Shared/Controls/Pager.ascx", Model.Table);
           }
        %><%
       if (isViewHaveGantt) {
       %>   
           <script type="text/javascript">
               $(document).ready(function () {
                   $('th.ganttHeaders').each(function () {
                       var h = $(this).height();
                       $(this).children('div.ganttRowContainer').css("height", h + "px").css("line-height", h + "px");
                   });
               });
           </script>
       <% } 
           if (view.DisplayType != Durados.DisplayType.Table) { %>
<div id='<%=guid + "Chart" %>' class="chartPlaceHolder"></div>
<%} 
} catch(Exception exception){ %>
    <% Map.Logger.Log("DataTableView", "DataTableView", "DataTableView", exception, 1, ""); %>
    <span><%= "$$error start$$ " + exception.Message + " $$error end$$"%></span>
    <% throw exception; %>
<%} %>