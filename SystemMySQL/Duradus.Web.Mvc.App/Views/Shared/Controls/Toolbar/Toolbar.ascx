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
    tableViewer.DataView = Model;
    string viewSafety = view.Name + "_safety";
    bool safetyMode = false;
    Durados.DataDisplayType displayType = ViewData["displayType"] == null ? Durados.DataDisplayType.Table : (Durados.DataDisplayType)ViewData["displayType"];

    bool hideSearch = (view.HideSearch && !view.IsInAdminMode()) || Durados.Web.Infrastructure.General.IsMobile();
    bool hideFilter = view.HideFilter && !view.IsInAdminMode();
    bool exportToCsv = view.ExportToCsv && !view.IsInAdminMode();
    bool importFromExcel = view.ImportFromExcel && !view.IsInAdminMode();
    bool gridEditable = view.GridEditable || view.IsInAdminMode();

    if (Map.Session[viewSafety] != null)
    {
        safetyMode = (bool)Map.Session[viewSafety];
    }
    safetyMode = (safetyMode && !Durados.Web.Infrastructure.General.IsMobile());
    string pk = (ViewData["pk"] == null) ? string.Empty : ViewData["pk"].ToString();

    string textSearch = Map.Database.Localizer.Translate("Search on text...");
    string search = ViewData["search"] as string; if (string.IsNullOrEmpty(search)) search = textSearch;

    bool hasSearch = !string.IsNullOrEmpty(search) && search != textSearch;
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
    string viewFilterVisibility = view.Name + "_filterVisibility";
    bool collapseFilter = view.CollapseFilter || (Map.Session[viewFilterVisibility] != null && !(bool)Map.Session[viewFilterVisibility]);
    if (Durados.Web.Infrastructure.General.IsMobile())
        collapseFilter = true;
   
    bool hideToolbar = view.HideToolbar && !view.IsInAdminMode();

    string addFieldClass = view.Name == "Field" ? "class=\"btn-green\"" : string.Empty;
    if (!hideToolbar)
    { %>
<% if (!Durados.Web.Mvc.Maps.Skin)
   { %>
<%bool displayHeader = (this.Request.QueryString["menu"] != "off"); %>
<% string classMargin = displayHeader ? string.Empty : " table-menu-nomargin"; %>
<div class="table-menu<%=classMargin %>" id='<%=guid%>toolbar'>
    <div class="group-l" role='crud'>
        <% if (view.IsCreatable() && tableViewer.IsEditable(view) && !view.IsDisabled(guid))
           {
               string addClick = string.Empty;
               string url = Url.Action("Item", view.Controller, new { viewName = view.Name, add = "yes", guid = view.Name + "_Item_" });
               addClick += "d_addNew(null, " + view.Popup.ToString().ToLower() + ", " + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + view.Name + "', '" + guid + "', this, " + view.IsEditable(guid).ToString().ToLower() + ", '')";
        %>
        <!-- NEW -->
        <a <%--<%=addFieldClass %>--%> name="new" href="#" onclick="<%=addClick %>" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.NEW)%>">
            <span class="icon new"></span>
            <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.NEW)%></a>
        <%} %>
        <%  if (view.Database.IsConfig && (view.Name == "Field") && (ViewContext.RouteData.Values["action"].ToString() == "Item" || (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri.Contains("menu=off")) || (Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains("IndexWithButtons"))))
            {%>
        <!-- REFRESH -->
        <a href="#" onclick="Refresh('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.REFRESH)%>">
            <span class="icon refresh"></span>
            <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.REFRESH)%></a>
    </div>
    <div class="clearfix">
    </div>
</div>
<% return; %>
<%} %>
<% if (view.HasAddItemsField && tableViewer.IsEditable(view) && !view.IsDisabled(guid) && (view.AddItemsField.ParentView.IsAllow() || view.Name == "View"))
   {
       string addItemsClick = string.Empty;
       string url = Url.Action("AddItems", view.Controller, new { viewName = view.Name });
       Durados.Web.Mvc.View searchView = (Durados.Web.Mvc.View)view.AddItemsField.ParentView;
       string searchUrl = searchView.GetIndexUrl() + "?firstTime=true&disabled=true&checkbox=true";
       
       addItemsClick += "d_addItems(" + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + searchUrl + "', '" + view.Name + "', '" + searchView.DisplayName + "', '" + guid + "', this)";
%>
<!-- ADD ITEMS -->
<a href="#" onclick="<%=addItemsClick %>" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.ADD_ITEMS)%>">
    <span class="icon new"></span>
    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.ADD_ITEMS)%></a>
<%} %>
<%if (displayType == Durados.DataDisplayType.Table || displayType == Durados.DataDisplayType.Preview)
  {
      if (view is Durados.Config.IConfigView && (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin")) && view.Name == "View")
      { %>

      <!-- OPEN VIEW -->
<a href="#" onclick="d_doAction('<%=guid %>', 'open');" title="<%= Map.Database.Localizer.Translate("Open View")%>">
    <span class="icon new"></span>
    <%=Map.Database.Localizer.Translate("Open")%></a>

<!-- SYNC VIEW WITH SERVER -->
<% string syncUrl = Url.Action("Sync", "Admin"); %>
<a href="#" onclick="sync('<%=syncUrl %>', '<%=guid %>');" title="<%= Map.Database.Localizer.Translate("Sync with server")%>">
    <span class="icon new"></span>
    <%=Map.Database.Localizer.Translate("Sync")%></a>
<%} %>
<% if (view.IsDuplicatable() && tableViewer.IsEditable(view) && !view.IsDisabled(guid) && !Durados.Web.Infrastructure.General.IsMobile())
   {  %>
<!-- DUPLICATE -->
<a class="duplicate-a" href="#" onclick="d_doAction('<%=guid %>', 'duplicate');" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.DUPLICATE)%>">
    <span class="icon duplicate"></span>
    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.DUPLICATE)%></a>
<%} %>
<% if (view is Durados.Config.IConfigView && view.Name == "View" && Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer"))
   { %>
<% string repairOrderUrl = Url.Action("RepairOrder", "Admin"); %>
<a href="#" onclick="repairOrder('<%=repairOrderUrl %>', '<%=guid %>');return false"
    title="<%= Map.Database.Localizer.Translate("Reorder the numbers according to the current order")%>">
    <span class="icon new"></span>
    <%=Map.Database.Localizer.Translate("Reorder")%></a>
<%} %>
<% if (view is Durados.Config.IConfigView && (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin")) && !Map.Database.AutoCommit)
   { %>
<% string refreshConfigUrl = Url.Action("RefreshConfig", "Admin"); %>
<!-- CACHE -->
<a href="#" onclick="refreshConfig('<%=refreshConfigUrl %>', '<%=guid %>');return false"
    title="<%= Map.Database.Localizer.Translate("Commit administration changes")%>">
    <span class="icon new"></span>
    <%=Map.Database.Localizer.Translate("Commit")%></a>
<%}
  } %>
<% if (view.IsEditable(guid) && tableViewer.IsEditable(view) && view.DisplayType == Durados.DisplayType.Table && !Durados.Web.Infrastructure.General.IsMobile())
   {  %>
<!-- EDIT -->
<a href="#" onclick="d_doAction('<%=guid %>', 'edit');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.EDIT)%>">
    <span class="icon edit-batch"></span>
    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.EDIT)%></a>
<%} %>
<% if (view.IsCreatable() && view.IsOrdered && tableViewer.IsEditable(view) && view.IsEditable(guid) && !Durados.Web.Infrastructure.General.IsMobile())
   {  %>
<!-- INSERT -->
<a href="#" onclick="d_doAction('<%=guid %>', 'insert');return false;" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.INSERT)%>">
    <span class="icon new"></span>
    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.INSERT)%></a>
<%} %>
<% if (view.IsDeletable() && tableViewer.IsEditable(view) && view.IsDeletable(guid))
   { %>
<!-- DELETE -->
<a href="#" onclick="d_doAction('<%=guid %>', 'delete');return false;" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.DELETE)%>">
    <span class="icon delete"></span>
    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.DELETE)%></a>
<% if (!string.IsNullOrEmpty(view.TreeViewName))
   { %>
<!-- ASSIGN TO TREE -->
<a class="assignTree" href="#" onclick="Durados.TreeView.assignLeavesToTree('<%=guid %>', this);return false"
    title=""><span class="icon new"></span>
    <%=Map.Database.Localizer.Translate("Assign to Tree")%></a>
<%} %>
<%} %>
<%if (displayType == Durados.DataDisplayType.Table)
  {
      if (view.IsEditable(guid) && view.HasRowColor && tableViewer.IsEditable(view))
      { %>
<span class="crayon_sample" title="<%=view.GetActionTooltipDescription(Durados.ToolbarActionNames.PAINT)%>"
    id="color_choozer">&nbsp;</span>
<!-- PAINT -->
<a href="#" onclick="d_doAction('<%=guid %>', 'paint');return false;"><span class="icon new">
</span>
    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.PAINT)%></a><input type="text"
        id="color_input" style="display: none" />
<%} %>
<% if (!string.IsNullOrEmpty(view.AnotherRowLinkText))
   { %>
<!-- ANOTHER -->
<a href="#" onclick="d_doAction('<%=guid %>', '<%= view.AnotherRowLinkText %>');return false"
    title="<%= Map.Database.Localizer.Translate(view.AnotherRowLinkText)%>"><span class="icon new">
    </span>
    <%=Map.Database.Localizer.Translate(view.AnotherRowLinkText)%></a>
<%}
  }%>
<% if (view.Send && !view.IsDisabled(guid) && !Durados.Web.Infrastructure.General.IsMobile())
   { %>
<!-- SEND -->
<a href="#" onclick="Send.Show('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.SEND)%>">
    <span class="icon send"></span>
    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.SEND)%></a>
<%} %>
<% if (displayType == Durados.DataDisplayType.Table)
   {
       if (view.IsOrdered && tableViewer.IsEditable(view) && !view.IsDisabled(guid))
       { %>
<%--<span style="font-weight:bold" class="label"><%=Map.Database.Localizer.Translate("Move:")%></span>--%>
<%string urlSwitch = Url.Action("Switch", view.Controller, new { viewName = view.Name }); %>
<!-- UP -->
<a href="#" ondblclick="return false;" onclick="return reorder('<%=guid %>', 'up', '<%=view.Name %>', '<%=urlSwitch %>');"
    title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.UP)%>"><span
        class="icon new"></span>
    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.UP)%></a><%--\--%>
<%--<a href="#" style='background-image:url("img/arrow-right.png")' ondblclick="return false;" onclick="return reorder('<%=guid %>', 'up', '<%=view.Name %>', '<%=urlSwitch %>');" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.UP)%>"><span class="icon new"></span><%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.UP)%></a>--%><%--\--%>
<!-- DOWN -->
<a href="#" ondblclick="return false;" onclick="return reorder('<%=guid %>', 'down', '<%=view.Name %>', '<%=urlSwitch %>');"
    title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.DOWN)%>">
    <span class="icon new"></span>
    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.DOWN)%></a>
<%}
   } %>
<% if (!hideFilter)
   { 
%>
<!-- SHOW/HIDE/APPLY/CLEAR FILTER -->
<%
       ViewData["filterClass"] = filterClass;
    Html.RenderPartial("~/Views/Shared/Controls/Filter/ApplyClearFilter.ascx", view);%>
<%--</div>--%>
<%} %>
<% if (displayType == Durados.DataDisplayType.Table)
   {
       if (gridEditable && view.IsEditable(guid) && !Durados.Web.Infrastructure.General.IsMobile() && !view.GridEditableEnabled)
       { %>
<% string gridEditableEnabled = safetyMode ? "checked='checked'" : ""; %>
<!-- GRID EDIT MODE -->
<a><span class="icon">
    <input type="checkbox" safari="1" onchange="SafetyModeChanged('<%=guid %>')" class="cbSafetyMode float"
        title="<%=view.GetActionTooltipDescription(Durados.ToolbarActionNames.GRID_EDIT_MODE)%>"
        id='<%=guid %>Safety' <%= gridEditableEnabled %> />
    <%--<input name="checkbox1.1" type="checkbox" safari="1" />--%>
</span>
    <%=Map.Database.Localizer.Translate("Edit Mode")%></a>
<%--<input onclick="SafetyModeChanged('<%=guid %>')" class="cbSafetyMode float" title="<%=view.GetActionTooltipDescription(Durados.ToolbarActionNames.GRID_EDIT_MODE)%>" id='<%=guid %>Safety' type="checkbox" <%= gridEditableEnabled %> /> 
            <span style="font-weight:bold"><%=Map.Database.Localizer.Translate("Edit Mode")%></span> |--%>
<%}
   }%>
<!-- REFRESH -->
<%if (!Durados.Web.Infrastructure.General.IsMobile())
  {%>
<a href="#" onclick="Refresh('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.REFRESH)%>">
    <span class="icon refresh"></span>
    <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.REFRESH)%></a>
<%} %>
<% if (displayType == Durados.DataDisplayType.Table || displayType == Durados.DataDisplayType.Preview)
   { %>
<% IDictionary<Durados.ToolbarActionNames, string> moreActionsMenu = view.GetMoreActionsMenu(guid, tableViewer, Url);

   int actionsCount = moreActionsMenu.Count;
   bool hasMoreActions = actionsCount > 0;
   bool isImportable = true;
%>
<% if (hasMoreActions && !Durados.Web.Infrastructure.General.IsMobile())
   {
       if (actionsCount == 1)
       {
%>
<% =moreActionsMenu.First().Value %>
<%
       }
       else
       {
%>
<span class="more-actions-container"><a class="menu-trigger slide-down">
    <%=Map.Database.Localizer.Translate("More Actions")%><span class="icon trigger"></span></a>
    <div class="more-actions">
        <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.HISTORY))
           { %>
        <ul>
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.HISTORY]%>
            </li>
        </ul>
        <%} %>
        <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.EXPORT) || moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.IMPORT))
           { %>
        <ul>
            <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.EXPORT))
               { %>
            <!-- EXPORT -->
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.EXPORT]%>
            </li>
            <%} %>
            <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.IMPORT))
               { %>
            <!-- IMPORT -->
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.IMPORT]%>
            </li>
            <%} %>
        </ul>
        <%} %>
        <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.PRINT))
           { %>
        <!-- PRINT -->
        <ul>
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.PRINT]%>
            </li>
        </ul>
        <%} %>
        <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.MESSAGE_BOARD))
           { %>
        <!-- Messages -->
        <ul>
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.MESSAGE_BOARD]%>
            </li>
        </ul>
        <%} %>
        <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.COPY_CONFIG) || moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.CLONE_CONFIG))
           { %>
        <ul>
            <!-- COPY VIEW CONFIGURATION -->
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.COPY_CONFIG]%>
            </li>
            <!-- CLONE VIEW CONFIGURATION -->
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.CLONE_CONFIG]%>
            </li>
        </ul>
        <%} %>
        <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.SEND_CONFIG) || moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.DOWNLOAD_CONFIG) || moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.UPLOAD_CONFIG))
           { %>
        <ul>
            <!-- SEND CONFIGURATION -->
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.SEND_CONFIG]%>
            </li>
            <!-- DOWNLOAD CONFIGURATION -->
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.DOWNLOAD_CONFIG]%>
            </li>
            <!-- UPLOAD CONFIGURATION -->
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.UPLOAD_CONFIG]%>
            </li>
        </ul>
        <%} %>
        <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.DIAGNOSE) || moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.DICTIONARY) || moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.SYNC_ALL))
           { %>
        <ul>
            <!-- SYNC_ALL -->
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.SYNC_ALL]%>
            </li>
            <!-- DIAGNOSE -->
            <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.DIAGNOSE)) {%>
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.DIAGNOSE]%>
            </li>
            <%}%>
            <!-- DICTIONARY -->
            <% if (moreActionsMenu.ContainsKey(Durados.ToolbarActionNames.DIAGNOSE)) {%>
            <li>
                <%=moreActionsMenu[Durados.ToolbarActionNames.DICTIONARY]%>
            </li>
            <%}%>
        </ul>
        <%} %>
        <!-- JSON -->
        <%if (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer")){ %>
        <ul>
            <li>
                <a href="#" onclick="d_doAction('<%=guid %>', 'json');return false;">
                    JSON</a>
            </li>
        </ul>
        <%} %>
    </div>
</span>
<%}
   }
   }%>
<!-- View Mode -->
<%
   ViewData["guid"] = guid;
   Html.RenderPartial("~/Views/Shared/Controls/Toolbar/DisplayTypeToolbar.ascx", view);
%>
</div>
<div class="group-r">
    <% 
        if (!hideFilter && !hideSearch)
       { %>
    <!-- SEARCH -->
    <%--<a href="#" onclick="Refresh('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.REFRESH)%>"><span class="icon new"></span><%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.REFRESH)%></a>--%>
    <%Html.RenderPartial("~/Views/Shared/Controls/Filter/FreeFilter.ascx", view);%>
    <%   } %>
</div>
<div class="clearfix">
</div>
</div>
<% 
       if (!hideFilter)
       {
           ViewData["collapseFilter"] = collapseFilter;
           //Html.RenderPartial("~/Views/Shared/Controls/Filter/FilterButtons.ascx", Model);
       }
   }
   else
   { %>
<table cellspacing="0" cellpadding="0" width="100%" class="toolbar">
    <tr class='rowcommands'>
        <td colspan="*" class="tablecommand" valign="top">
            <% if (view.IsCreatable() && tableViewer.IsEditable(view) && !view.IsDisabled(guid))
               {
                   string addClick = string.Empty;
                   string url = Url.Action("Item", view.Controller, new { viewName = view.Name, add = "yes", guid = view.Name + "_Item_" });
                   addClick += "d_addNew(null, " + view.Popup.ToString().ToLower() + ", " + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + view.Name + "', '" + guid + "', this, " + view.IsEditable(guid).ToString().ToLower() + ", '')";
            %>
            <!-- NEW -->
            <a style="font-weight: bold;" href="#" onclick="<%=addClick %>" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.NEW)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.NEW)%></a>
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
            <a style="font-weight: bold;" href="#" onclick="<%=addItemsClick %>" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.ADD_ITEMS)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.ADD_ITEMS)%></a><span>&nbsp;|&nbsp;</span>
            <%} %>
            <% if (displayType == Durados.DataDisplayType.Table)
               {
                   if (view is Durados.Config.IConfigView && (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin")) && view.Name == "View")
                   { %>
            <!-- SYNC VIEW WITH SERVER -->
            <% string syncUrl = Url.Action("Sync", "Admin"); %>
            <a href="#" onclick="sync('<%=syncUrl %>', '<%=guid %>');" title="<%= Map.Database.Localizer.Translate("Sync with server")%>">
                <span class="icon new"></span>
                <%=Map.Database.Localizer.Translate("Sync")%></a>
            <%} %>
            <% if (view.IsDuplicatable() && tableViewer.IsEditable(view) && !view.IsDisabled(guid))
               {  %>
            <!-- DUPLICATE -->
            <a style="font-weight: bold;" href="#" onclick="d_doAction('<%=guid %>', 'duplicate');"
                title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.DUPLICATE)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.DUPLICATE)%></a>
            <%} %>
            <% if (view.IsOrdered && tableViewer.IsEditable(view) && !view.IsDisabled(guid))
               { %>
            <%string urlSwitch = Url.Action("Switch", view.Controller, new { viewName = view.Name }); %>
            <!-- UP -->
            <a href="#" ondblclick="return false;" onclick="return reorder('<%=guid %>', 'up', '<%=view.Name %>', '<%=urlSwitch %>');"
                title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.UP)%>"><span
                    class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.UP)%></a>
            <!-- DOWN -->
            <a href="#" ondblclick="return false;" onclick="return reorder('<%=guid %>', 'down', '<%=view.Name %>', '<%=urlSwitch %>');"
                title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.DOWN)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.DOWN)%></a>
            <%-- <% if (view is Durados.Config.IConfigView && view.Name == "Field" && Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer"))
           { %>
            
                <% string ordinalColumnName = ((Durados.Web.Mvc.View)Map.GetConfigDatabase().Views["Field"]).OrdinalColumnName;  %>
              <select name='d_fieldOrder'>
              <option <%= ordinalColumnName == "Order" ? "selected='selected'" : "" %> value='Order' >Table</option>
              <option <%= ordinalColumnName == "OrderForCreate" ? "selected='selected'" : "" %> value='OrderForCreate' >Create</option>
              <option <%= ordinalColumnName == "OrderForEdit" ? "selected='selected'" : "" %> value='OrderForEdit' >Edit</option>
              </select>&nbsp;
              
              <span>&nbsp;|&nbsp;</span>
              
              <% string copyOrderUrl = Url.Action("CopyOrder", "Admin"); %>
                <a href="#" onclick="copyOrder('<%=copyOrderUrl %>', '<%=guid %>');return false" title="<%= Map.Database.Localizer.Translate("Reorder Edit and Create fields based on the table order")%>"><%=Map.Database.Localizer.Translate("Reorder")%></a><span>&nbsp;|&nbsp;</span>
              <%} %>--%>
            <%} %>
            <% if (view is Durados.Config.IConfigView && view.Name == "View" && Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer"))
               { %>
            <% string repairOrderUrl = Url.Action("RepairOrder", "Admin"); %>
            <a href="#" onclick="repairOrder('<%=repairOrderUrl %>', '<%=guid %>');return false"
                title="<%= Map.Database.Localizer.Translate("Reorder the numbers according to the current order")%>">
                <span class="icon new"></span>
                <%=Map.Database.Localizer.Translate("Reorder")%></a>
            <%} %>
            <% if (view is Durados.Config.IConfigView && (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin")) && !Map.Database.AutoCommit)
               { %>
            <% string refreshConfigUrl = Url.Action("RefreshConfig", "Admin"); %>
            <!-- CACHE -->
            <a href="#" onclick="refreshConfig('<%=refreshConfigUrl %>', '<%=guid %>');return false"
                title="<%= Map.Database.Localizer.Translate("Commit administration changes")%>">
                <span class="icon new"></span>
                <%=Map.Database.Localizer.Translate("Commit")%></a>
            <%}
               } %>
            <% if (view.MultiSelect && !Durados.Web.Infrastructure.General.IsMobile())
               { %>
            <span style="font-weight: bold;">
                <%=Map.Database.Localizer.Translate("Select:")%></span>
            <!-- SELECT ALL -->
            <a href="#" onclick="Multi.All('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.SELECT_ALL)%>">
                <span class="icon new"></span>
                <%=Map.Database.Localizer.Translate("All")%></a>
            <!-- CLEAR -->
            <a href="#" onclick="Multi.Clear('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.CLEAR)%>">
                <span class="icon new"></span>
                <%=Map.Database.Localizer.Translate("Clear")%></a>
            <%} %>
            <% if (view.IsEditable(guid) && tableViewer.IsEditable(view) && view.DisplayType == Durados.DisplayType.Table)
               {  %>
            <!-- EDIT -->
            <a href="#" onclick="d_doAction('<%=guid %>', 'edit');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.EDIT)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.EDIT)%></a>
            <%} %>
            <% if (view.IsCreatable() && view.IsOrdered && tableViewer.IsEditable(view) && view.IsEditable(guid))
               {  %>
            <!-- INSERT -->
            <a href="#" onclick="d_doAction('<%=guid %>', 'insert');return false;" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.INSERT)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.INSERT)%></a>
            <%} %>
            <% if (view.IsDeletable() && tableViewer.IsEditable(view) && view.IsDeletable(guid))
               { %>
            <!-- DELETE -->
            <a href="#" onclick="d_doAction('<%=guid %>', 'delete');return false;" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.DELETE)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.DELETE)%></a>
            <% if (!string.IsNullOrEmpty(view.TreeViewName))
               { %>
            <!-- ASSIGN TO TREE -->
            <a class="assignTree" href="#" onclick="Durados.TreeView.assignLeavesToTree('<%=guid %>', this);return false"
                title=""><span class="icon new"></span>
                <%=Map.Database.Localizer.Translate("Assign to Tree")%></a>
            <%} %>
            <%} %>
            <%if (displayType == Durados.DataDisplayType.Table)
              {
                  if (view.IsEditable(guid) && view.HasRowColor && tableViewer.IsEditable(view))
                  { %>
            <span class="crayon_sample" title="<%=view.GetActionTooltipDescription(Durados.ToolbarActionNames.PAINT)%>"
                id="Span1">&nbsp;</span>
            <!-- PAINT -->
            <a href="#" onclick="d_doAction('<%=guid %>', 'paint');return false;">
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.PAINT)%></a><input type="text"
                    id="Text1" style="display: none" />
            <%} %>
            <% if (!string.IsNullOrEmpty(view.AnotherRowLinkText))
               { %>
            <!-- ANOTHER -->
            <a href="#" onclick="d_doAction('<%=guid %>', '<%= view.AnotherRowLinkText %>');return false"
                title="<%= Map.Database.Localizer.Translate(view.AnotherRowLinkText)%>"><span class="icon new">
                </span>
                <%=Map.Database.Localizer.Translate(view.AnotherRowLinkText)%></a>
            <%}
              } %>
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
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.HISTORY)%></a>
            <%} %>
            <% if (displayType == Durados.DataDisplayType.Table)
               {
                   bool isImportable = view.ImportFromExcel && !Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsDenied(view.DenyEditRoles, view.AllowEditRoles) && !Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsDenied(view.DenyCreateRoles, view.AllowCreateRoles); %>
            <% if ((view.ExportToCsv || isImportable) && !view.IsDisabled(guid))
               { %>
            <span style="font-weight: bold;">
                <%=Map.Database.Localizer.Translate("Excel:")%></span>
            <%} %>
            <% if (view.ExportToCsv && !view.IsDisabled(guid))
               { %>
            <!-- EXPORT -->
            <a href="<%=view.GetExportToCsvUrl() + "?guid=" + guid %>" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.EXPORT)%>">
                <span class="icon export"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.EXPORT)%></a>
            <%} %>
            <% if (isImportable && !view.IsDisabled(guid))
               { %>
            <!-- IMPORT -->
            <a href='#' onclick="Excel.Import('<%=view.Name %>','<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.IMPORT)%>">
                <span class="icon import"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.IMPORT)%></a>
            <%} %>
            <% if ((view.ExportToCsv || view.ImportFromExcel) && !view.IsDisabled(guid))
               { %>
            <%} %>
            <% if (view.Print && !view.IsDisabled(guid))
               { %>
            <!-- PRINT -->
            <a href="#" onclick="Print('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.PRINT)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.PRINT)%></a><span>&nbsp;|&nbsp;</span>
            <%}
               }%>
            <% if (view.Send && !view.IsDisabled(guid))
               { %>
            <!-- SEND -->
            <a href="#" onclick="Send.Show('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.SEND)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.SEND)%></a><span>&nbsp;|&nbsp;</span>
            <%} %>
            <% if (displayType == Durados.DataDisplayType.Table)
               {
                   if (!!hideFilter)
                   { 
             
            %>
            <!-- SHOW/HIDE/APPLY/CLEAR FILTER -->
            <%
                ViewData["filterClass"] = filterClass;
                Html.RenderPartial("~/Views/Shared/Controls/Filter/ApplyClearFilter.ascx", Model);%>
            <%}
               } %>
            <% if (view.HasMessages())
               { %>
            <!-- Messages -->
            <a href="#" onclick="Messages.Show('<%=view.Name %>', '<%=guid %>')" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.MESSAGE_BOARD)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.MESSAGE_BOARD)%></a><span>&nbsp;|&nbsp;</span>
            <%} %>
            <% if (displayType == Durados.DataDisplayType.Table)
               {
                   if (view is Durados.Config.IConfigView && Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") && view.Name == "View")
                   { %>
            <!-- COPY VIEW CONFIGURATION -->
            <a href="#" onclick="showCopyDialog('<%=guid %>', this);" title="<%= Map.Database.Localizer.Translate("Copy view configuration")%>">
                <span class="icon new"></span>
                <%=Map.Database.Localizer.Translate("Copy")%></a>
            <!-- CLONE VIEW CONFIGURATION -->
            <a href="#" onclick="showCloneDialog('<%=guid %>');" title="<%= Map.Database.Localizer.Translate("Clone view configuration")%>">
                <span class="icon new"></span>
                <%=Map.Database.Localizer.Translate("Clone")%></a>
            <%} %>
            <% if (view is Durados.Config.IConfigView && (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin")) && view.Name == "Database")
               { %>
            <% string title = Map.Database.Localizer.Translate("Send configuration"); %>
            <% string sendConfigUrl = Url.Action("SendConfig", "Admin"); %>
            <!-- SEND CONFIGURATION -->
            <a href="#" onclick="sendConfig('<%= sendConfigUrl%>');" title="<%= title%>"><span
                class="icon new"></span>
                <%=Map.Database.Localizer.Translate("Send")%></a>
            <% title = Map.Database.Localizer.Translate("Download configuration"); %>
            <% string downloadConfigUrl = Url.Action("DownloadConfig", "Admin"); %>
            <!-- DOWNLOAD CONFIGURATION -->
            <a target="_blank" href="<%= downloadConfigUrl%>" title="<%= title%>"><span class="icon new">
            </span>
                <%=Map.Database.Localizer.Translate("Download")%></a>
            <!-- UPLOAD CONFIGURATION -->
            <% title = Map.Database.Localizer.Translate("Upload configuration"); %>
            <a href="#" onclick="UploadConfig.Open('Database', '<%=guid %>')" title="<%= title%>">
                <span class="icon new"></span>
                <%=Map.Database.Localizer.Translate("Upload")%></a>
            <%--<% title = Map.Database.Localizer.Translate("Upload configuration"); %>
                <% string uploadConfigUrl = Url.Action("UploadConfig", "Admin"); %>
                <a href="#" onclick="uploadConfig('<%= uploadConfigUrl%>');" title="<%= title%>" alt="<%= title%>"><%=Map.Database.Localizer.Translate("Upload")%></a>
            --%>
            <%} %>
            <% if (view is Durados.Config.IConfigView && Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") && (view.Name == "Database" || view.Name == "View"))
               { %>
            <% string title = Map.Database.Localizer.Translate("Diagnose"); %>
            <a href="#" onclick="Diagnostics.Diagnose('<%=guid %>');" title="<%= title%>"><span
                class="icon new"></span>
                <%=title%></a>
            <% title = Map.Database.Localizer.Translate("Dictionary"); %>
            <% string dictionaryUrl = Url.Action("Index", "Block", new { parameters = "xxx" }); %>
            <a href="#" onclick="Dictionary('<%=dictionaryUrl %>', '<%=guid %>');" title="<%= title%>">
                <span class="icon new"></span>
                <%=title%></a> <span>&nbsp;|&nbsp;</span>
            <%}
               } %>
            <!-- REFRESH -->
            <a href="#" onclick="Refresh('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.REFRESH)%>">
                <span class="icon new"></span>
                <%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.REFRESH)%>&nbsp;</a>
            <% if (!!hideFilter && !hideSearch)
               { %>
            <!-- SEARCH -->
            <%Html.RenderPartial("~/Views/Shared/Controls/Filter/FreeFilter.ascx", view);%>
            <% } %>
            <%if (displayType == Durados.DataDisplayType.Table)
              {
                  if (gridEditable && view.IsEditable(guid) && !Durados.Web.Infrastructure.General.IsMobile())
                  { %>
            <span>
                <%=Map.Database.Localizer.Translate("Edit Mode:")%>
            </span>
            <% string gridEditableEnabled = safetyMode ? "checked='checked'" : ""; %>
            <!-- GRID EDIT MODE -->
            <input onchange="SafetyModeChanged('<%=guid %>')" class="cbSafetyMode" title="<%=view.GetActionTooltipDescription(Durados.ToolbarActionNames.GRID_EDIT_MODE)%>"
                id='<%=guid %>Safety' type="checkbox" <%= gridEditableEnabled %> />
            <%}
              } %>
            <br style="clear: both" />
        </td>
        <!-- View Mode -->
        <td class="tablecommand" style="width: 80px; padding-right: 10px; border-left: 0">
            <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/grid.png" %>"
                class="dashboard_img disabled" onclick="return false" title="<%=Map.Database.Localizer.Translate("Grid View")%>" />
            <span>&nbsp;|&nbsp;</span>
            <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/dashboard.png" %>"
                class="dashboard_img" onclick="Durados.DisplayType.changeDisplayType('<%=guid %>', 'Dashboard');return false"
                title="<%=Map.Database.Localizer.Translate("Summary")%>" />
            <span class="float">&nbsp;|&nbsp;</span>
            <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/preview.gif" %>"
                class="dashboard_img float disabled" onclick="Durados.DisplayType.changeDisplayType('<%=guid %>', 'Preview');return false"
                title="Preview" />
        </td>
    </tr>
</table>
<%} %>
<%}
    else
    {
        if (!hideFilter)
        {
            ViewData["enableCollapseFilter"] = false;
            //Html.RenderPartial("~/Views/Shared/Controls/Filter/FilterButtons.ascx", Model);
        }
    }
%>
