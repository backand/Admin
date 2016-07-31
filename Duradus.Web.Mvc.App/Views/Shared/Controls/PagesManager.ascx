<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<% string viewName = (ViewData["viewName"] ?? string.Empty).ToString(); %>
<%
    Workspace workspace = MenuHelper.GetCurrentWorkspace(viewName);
%>
<% if (workspace != null)
   { %>
   <%--<div class="cf nestable-lists">--%>
<div class="pages-manager-wrapper">
<div class="pages-manager dd">
    <ol class="dd-list">
        <% foreach (SpecialMenu menu in Map.Database.SpecialMenus.Values)
           { %>
        <% if (!menu.IsEmpty(Map.Database) || !string.IsNullOrEmpty(menu.Url))
           {
               string menuName = menu.Name;
               string menuUrl = string.IsNullOrEmpty(menu.Url) ? "#" : menu.Url;

               if (Map.Database.IsMultiLanguages)
               {
                   menuName = Map.Database.Localizer.Translate(menu.Name);
               }%>
        <li class="dd-item dd3-item" data-id="<%=menu.ID %>"><div class="dd-handle dd3-handle"></div><div class="dd3-content" url="<%=menuUrl%>"><span><%=menuName%></span><span class="dd3-settings"></span></div>
        <% if (menu.Menus.Count > 0)
           {%>
            <ol>
                <%  Html.RenderPartial("~/Views/Shared/Controls/Pages.ascx", menu); %>
            </ol>
            <%} %>
        </li>
        <%} %>
        <%} %>
    </ol>

    <a href="#" onclick="openAddPageDialog()">Add Page</a>
</div>
</div>
<%} %>