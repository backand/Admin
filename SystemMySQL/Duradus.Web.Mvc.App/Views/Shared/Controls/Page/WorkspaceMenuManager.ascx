<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Workspace>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<% string viewName = (ViewData["viewName"] ?? string.Empty).ToString(); %>
<%
    Workspace workspace = Model ?? MenuHelper.GetCurrentWorkspace(viewName);
%>
<% if (workspace != null)
   { %>
<%--<div class="cf nestable-lists">--%>



    <div class="pages-manager dd">
        <div class="pages-manager-scroll">
            <ol class="dd-list">
                <% foreach (SpecialMenu menu in workspace.SpecialMenus.Values.OrderBy(m=>m.Ordinal))
                   { %>
                <% if (!menu.IsEmpty(Map.Database) || !string.IsNullOrEmpty(menu.Url))
                   {
                       string menuName = menu.Name;
                       string menuUrl = string.IsNullOrEmpty(menu.Url) ? "#" : menu.Url;
                       string configViewName = menu.GetConfigViewName();
                       string pk = configViewName == "View" ? new Durados.DataAccess.ConfigAccess().GetViewPK(menu.ViewName, Map.GetConfigDatabase().ConnectionString) : menu.ViewName;
                       string pageTypeClass = "page-type-" + menu.LinkType;
                       string hideFromMenu = menu.HideFromMenu ? "yes" : "no";
                       string homepage = menu.ID.Equals(workspace.HomePage) ? "yes" : "no";
                       string homepageShow = homepage == "yes" ? "home-page-show" : string.Empty;
                       string hideFromMenuClass = menu.HideFromMenu ? "hide-from-menu" : "";
                       string title = Map.Database.Localizer.Translate("Settings");
                       if (Map.Database.IsMultiLanguages)
                       {
                           menuName = Map.Database.Localizer.Translate(menu.Name);
                       }%>
                <li class="dd-item dd3-item" data-id="<%=menu.ID %>">
                    <div class="dd-handle dd3-handle">
                    </div>
                    <div class="dd3-content <%=hideFromMenuClass %>" url="<%=menuUrl%>">
                        <span class="page-type <%=pageTypeClass%>"></span><span class="page-name" title="<%=menuName%>">
                            <%=menuName%></span><span class="home-page <%=homepageShow %>"></span><span viewname="<%=configViewName%>"
                                pk="<%=pk%>" hidefrommenu="<%=hideFromMenu %>" homepage="<%=homepage %>" class="dd3-settings" title="<%=title %>"></span></div>
                    <% if (menu.Menus.Count > 0)
                       {%>
                    <ol>
                        <%  Html.RenderPartial("~/Views/Shared/Controls/Page/Pages.ascx", menu); %>
                    </ol>
                    <%} %>
                </li>
                <%} %>
                <%} %>
            </ol>
        </div>
        <div class='page-button-container'>
            <a onclick="openAddPageDialog()"><span class='add-page-button-icon'></span><span class='add-page-button'><%=Map.Database.Localizer.Translate("Add Page") %></span></a>
        </div>
        <div class="page-button-container page-settings-toolbar">
            <a onclick="window.location = '/Admin/Index/Workspace'"><span class='manage-workspaces-icon'></span><span class='manage-workspaces-button'><%=Map.Database.Localizer.Translate("Manage Workspaces")%></span></a>
        </div>
        
     </div>
     
<%} %>