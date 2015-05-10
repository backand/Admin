<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataView>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json" %>
<% 
    string guid = Model.Table.ExtendedProperties["guid"].ToString();
    Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()];
    Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];
    string textSearch = Map.Database.Localizer.Translate("Search on text...");
    string search = ViewData["search"] as string; if (string.IsNullOrEmpty(search)) search = textSearch;
    
    if (!view.HideToolbar)
   { %>
<% if (!Durados.Web.Mvc.Maps.Skin)
   { %>
<%bool displayHeader = (this.Request.QueryString["menu"] != "off"); %>
<% string style = displayHeader ? string.Empty : "style='margin-left:2px'"; %>
<div <%=style %> class="table-menu">
    <div class="group">
        <% if (view.IsCreatable() && tableViewer.IsEditable(view) && !view.IsDisabled(guid))
           {
               string addClick = string.Empty;
               string url = Url.Action("Item", view.Controller, new { viewName = view.Name, add = "yes", guid = view.Name + "_Item_" });
               addClick += "d_addNew(null, " + view.Popup.ToString().ToLower() + ", " + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + view.Name + "', '" + guid + "', this, " + view.IsEditable(guid).ToString().ToLower() + ", '')";
        %>
        <!-- NEW -->
        <a href="#" onclick="<%=addClick %>" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.NEW)%>">
            <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.NEW)%></a> |
        <%} %>
        <% if (view.HasAddItemsField && tableViewer.IsEditable(view) && !view.IsDisabled(guid) && (view.AddItemsField.ParentView.IsAllow() || view.Name == "View"))
           {
               string addItemsClick = string.Empty;
               string url = Url.Action("AddItems", view.Controller, new { viewName = view.Name });
               Durados.Web.Mvc.View searchView = (Durados.Web.Mvc.View)view.AddItemsField.ParentView;
               string searchUrl = searchView.GetIndexUrl() + "?firstTime=true&disabled=true";
               addItemsClick += "d_addItems(" + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + searchUrl + "', '" + view.Name + "', '" + searchView.DisplayName + "', '" + guid + "', this)";
        %>
        <!-- ADD ITEMS -->
        <a href="#" onclick="<%=addItemsClick %>" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.ADD_ITEMS)%>">
            <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.ADD_ITEMS)%></a> |
        <%} %>
        <% if (view.IsEditable(guid) && tableViewer.IsEditable(view) && view.DisplayType == Durados.DisplayType.Table)
           {  %>
        <!-- EDIT -->
        <a href="#" onclick="d_doAction('<%=guid %>', 'edit');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.EDIT)%>">
            <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.EDIT)%></a> |
        <%} %>
        <% if (view.IsCreatable() && view.IsOrdered && tableViewer.IsEditable(view) && view.IsEditable(guid))
           {  %>
        <!-- INSERT -->
        <a href="#" onclick="d_doAction('<%=guid %>', 'insert');return false;" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.INSERT)%>">
            <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.INSERT)%></a> |
        <%} %>
        <% if (view.IsDeletable() && tableViewer.IsEditable(view) && view.IsDeletable(guid))
           { %>
        <!-- DELETE -->
        <a href="#" onclick="d_doAction('<%=guid %>', 'delete');return false;" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.DELETE)%>">
            <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.DELETE)%></a> |
        <% if (!string.IsNullOrEmpty(view.TreeViewName))
           { %>
        <!-- ASSIGN TO TREE -->
        <a class="assignTree" href="#" onclick="Durados.TreeView.assignLeavesToTree('<%=guid %>', this);return false"
            title="">
            <%=Map.Database.Localizer.Translate("Assign to Tree")%></a> |
        <%} %>
        <%} %>
        <% if (view.Send && !view.IsDisabled(guid))
           { %>
        <!-- SEND -->
        <a href="#" onclick="Send.Show('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.SEND)%>">
            <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.SEND)%></a>
        <%} %>
    </div>
    <div class="group">
        <% if (view.MultiSelect)
           { %>
        <span style="font-weight: bold" class="label">
            <%=Map.Database.Localizer.Translate("Select:")%></span>
        <!-- SELECT ALL -->
        <a href="#" onclick="Multi.BoardAll('<%=guid %>', true);return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.SELECT_ALL)%>">
            <%=Map.Database.Localizer.Translate("All")%></a> |
        <!-- CLEAR -->
        <a href="#" onclick="Multi.BoardAll('<%=guid %>', false);return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.CLEAR)%>">
            <%=Map.Database.Localizer.Translate("Clear")%></a>
        <%} %>
    </div>
    <div class="group">
        <!-- REFRESH -->
        <a href="#" onclick="Refresh('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.REFRESH)%>">
            <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.REFRESH)%></a>
    </div>
    <% if (!view.HideFilter && !view.HideSearch)
       { %>
    <div class="group">
        <!-- SEARCH -->
        <input class="search_text serch-field" type="text" value="<%=search %>" onkeypress="handleEnterFilter(this, event,'<%=guid %>',this);"
            onblur="if(this.value==''){this.value='<%=textSearch %>';}" onclick="if(this.value=='<%=textSearch %>') this.value = '';"
            title="<%=Map.Database.Localizer.Translate("Search on all text fields")%>" />
        <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/img/search.png" %>"
            width="16px" height="16px" align="absmiddle" onclick="FilterForm.ApplySearch(false, '<%=guid %>', this, '<%=textSearch %>')"
            title="<%=view.GetActionTooltipDescription(Durados.ToolbarActionNames.SEARCH)%>" />
        <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/cancelsearch.gif" %>"
            width="16px" height="16px" align="absmiddle" onclick="FilterForm.ApplyClear(false, '<%=guid %>', this)"
            title="<%=view.GetActionTooltipDescription(Durados.ToolbarActionNames.CLEAR)%>" />
    </div>
    <%   } %>
    <!-- View Mode -->
    <%
       ViewData["guid"] = guid;
       ViewData["displayType"] = Durados.DataDisplayType.Preview;
           Html.RenderPartial("~/Views/Shared/Controls/Toolbar/DisplayTypeToolbar.ascx", view);
    %>
    <div class="clearfix">
    </div>
</div>
<%}
   else
   { %>
<table cellspacing="0" cellpadding="0" width="100%" class="toolbar">
    <tr class='rowcommands'>
        <td colspan="*" class="tablecommand" valign="top">
            <span class="tablecommands float">
                <% if (view.IsCreatable() && tableViewer.IsEditable(view) && !view.IsDisabled(guid))
                   {
                       string addClick = string.Empty;
                       string url = Url.Action("Item", view.Controller, new { viewName = view.Name, add = "yes", guid = view.Name + "_Item_" });
                       addClick += "d_addNew(null, " + view.Popup.ToString().ToLower() + ", " + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + view.Name + "', '" + guid + "', this, " + view.IsEditable(guid).ToString().ToLower() + ", '')";
                %>
                <!-- NEW -->
                <a style="font-weight: bold;" href="#" onclick="<%=addClick %>" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.NEW)%>">
                    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.NEW)%></a><span>&nbsp;|&nbsp;</span>
                <%} %>
                <% if (view.HasAddItemsField && tableViewer.IsEditable(view) && !view.IsDisabled(guid) && view.AddItemsField.ParentView.IsAllow())
                   {
                       string addItemsClick = string.Empty;
                       string url = Url.Action("AddItems", view.Controller, new { viewName = view.Name });
                       Durados.Web.Mvc.View searchView = (Durados.Web.Mvc.View)view.AddItemsField.ParentView;
                       string searchUrl = searchView.GetIndexUrl() + "?firstTime=true&disabled=true";
                       addItemsClick += "d_addItems(" + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + searchUrl + "', '" + view.Name + "', '" + searchView.DisplayName + "', '" + guid + "', this)";
                %>
                <!-- ADD ITEMS -->
                <a style="font-weight: bold;" href="#" onclick="<%=addItemsClick %>" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.ADD_ITEMS)%>">
                    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.ADD_ITEMS)%></a><span>&nbsp;|&nbsp;</span>
                <%} %>
                <%  if (view.MultiSelect)
                    { %>
                <span style="font-weight: bold;">
                    <%=Map.Database.Localizer.Translate("Select:")%></span>
                <!-- SELECT ALL -->
                <a href="#" onclick="Multi.BoardAll('<%=guid %>', true);return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.SELECT_ALL)%>">
                    <%=Map.Database.Localizer.Translate("All")%></a>
                <!-- CLEAR -->
                <a href="#" onclick="Multi.BoardAll('<%=guid %>', false);return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.CLEAR)%>">
                    <%=Map.Database.Localizer.Translate("Clear")%></a> <span>&nbsp;|&nbsp;</span>
                <%} %>
                <% if (view.IsEditable(guid) && tableViewer.IsEditable(view) && view.DisplayType == Durados.DisplayType.Table)
                   {  %>
                <!-- EDIT -->
                <a href="#" onclick="d_doAction('<%=guid %>', 'edit');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.EDIT)%>">
                    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.EDIT)%></a><span>&nbsp;|&nbsp;</span>
                <%} %>
                <% if (view.IsCreatable() && view.IsOrdered && tableViewer.IsEditable(view) && view.IsEditable(guid))
                   {  %>
                <!-- INSERT -->
                <a href="#" onclick="d_doAction('<%=guid %>', 'insert');return false;" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.INSERT)%>">
                    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.INSERT)%></a><span>&nbsp;|&nbsp;</span>
                <%} %>
                <% if (view.IsDeletable() && tableViewer.IsEditable(view) && view.IsDeletable(guid))
                   { %>
                <!-- DELETE -->
                <a href="#" onclick="d_doAction('<%=guid %>', 'delete');return false;" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.DELETE)%>">
                    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.DELETE)%></a><span>&nbsp;|&nbsp;</span>
                <% if (!string.IsNullOrEmpty(view.TreeViewName))
                   { %>
                <!-- ASSIGN TO TREE -->
                <a class="assignTree" href="#" onclick="Durados.TreeView.assignLeavesToTree('<%=guid %>', this);return false"
                    title="">
                    <%=Map.Database.Localizer.Translate("Assign to Tree")%></a><span>&nbsp;|&nbsp;</span>
                <%} %>
                <%} %>
                <% if (view.SaveHistory && tableViewer.IsEditable(view))
                   { %>
                <% string historyGuid = "History_" + Durados.Web.Mvc.Infrastructure.ShortGuid.Next() + "_";  %>
                <%string historyUrl = Url.Action("HistoryFilter", "History", new { viewName = "durados_v_ChangeHistory" });
                  string historyView = view.Name;
                  if (view.Base.Name != null) { historyView = view.Base.Name; }
                %>
                <!-- HISTORY -->
                <a href="#" onclick="History('<%=historyUrl %>', '<%=guid %>', '<%=historyGuid %>', '<%=historyView %>');return false;"
                    title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.HISTORY)%>">
                    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.HISTORY)%></a><span>&nbsp;|&nbsp;</span>
                <%} %>
                <% if (view.Send && !view.IsDisabled(guid))
                   { %>
                <!-- SEND -->
                <a href="#" onclick="Send.Show('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.SEND)%>">
                    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.SEND)%></a><span>&nbsp;|&nbsp;</span>
                <%} %>
                <% if (view.HasMessages())
                   { %>
                <!-- Messages -->
                <a href="#" onclick="Messages.Show('<%=view.Name %>', '<%=guid %>')" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.MESSAGE_BOARD)%>">
                    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.MESSAGE_BOARD)%></a><span>&nbsp;|&nbsp;</span>
                <% } %>
                <!-- REFRESH -->
                <a href="#" onclick="Refresh('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.REFRESH)%>">
                    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.REFRESH)%></a>&nbsp;
            </span>
            <% if (!view.HideFilter && !view.HideSearch)
               { %>
            <!-- SEARCH -->
            <span class="float"><span class="float">&nbsp;|&nbsp;</span>
                <input class="search_text float" type="text" value="<%=search %>" onkeypress="handleEnterFilter(this, event,'<%=guid %>',this);"
                    onblur="if(this.value==''){this.value='<%=textSearch %>';}" onclick="if(this.value=='<%=textSearch %>') this.value = '';"
                    title="<%=Map.Database.Localizer.Translate("Search on all text fields")%>" />
                <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/Magnify.png" %>"
                    class="search_img float" onclick="FilterForm.ApplySearch(false, '<%=guid %>', this, '<%=textSearch %>')"
                    title="<%=view.GetActionTooltipDescription(Durados.ToolbarActionNames.SEARCH)%>" />
                <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/cancelsearch.gif" %>"
                    class="search_img float" onclick="FilterForm.ApplyClear(false, '<%=guid %>', this)"
                    title="<%=view.GetActionTooltipDescription(Durados.ToolbarActionNames.CLEAR)%>" />
            </span>
            <% if (view.GridEditable && view.IsEditable(guid))
               { %>
            <span class="float">&nbsp;|&nbsp;</span>
            <% }
               } %>
        </td>
        <!-- View Mode -->
        <%--<% if (view.HasDashboardView)
           { %>--%>
        <td class="tablecommand" style="width: 80px; padding-right: 10px; border-left: 0">
            <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/grid.png" %>"
                class="dashboard_img float" onclick="Durados.DisplayType.changeDisplayType('<%=guid %>', 'Table');return false"
                title="<%=Map.Database.Localizer.Translate("Grid View")%>" />
            <span class="float">&nbsp;|&nbsp;</span>
            <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/dashboard.png" %>"
                class="dashboard_img float disabled" onclick="Durados.DisplayType.changeDisplayType('<%=guid %>', 'Dashboard');return false"
                title="<%=Map.Database.Localizer.Translate("Summary")%>" />
            <span class="float">&nbsp;|&nbsp;</span>
            <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/preview.gif" %>"
                class="dashboard_img float disabled" onclick="return false" title="Preview" />
        </td>
        <%--<% } %>--%>
    </tr>
</table>
<%} %>
<%}%>
