<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.SpecialMenu>" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<% string viewName = (ViewData["viewName"] ?? string.Empty).ToString(); %>
<%
    Durados.Workspace workspace = MenuHelper.GetCurrentWorkspace(viewName);
%>
<% if (Model != null && Model.Menus != null)
   { %>
    <% foreach (Durados.SpecialMenu menu in Model.Menus.Values.OrderBy(m => m.Ordinal).ToList())
       {%>
        <% menu.Parent = Model; %>
        <% 
        string menuName = menu.Name;
        string menuUrl = string.IsNullOrEmpty(menu.Url) ? "#" : menu.Url;
        string configViewName = menu.GetConfigViewName();
        string pk = configViewName == "View" ? new Durados.DataAccess.ConfigAccess().GetViewPK(menu.ViewName, Map.GetConfigDatabase().ConnectionString) : menu.ViewName;
        string pageTypeClass = "page-type-" + menu.LinkType;
        string hideFromMenu = menu.HideFromMenu ? "yes" : "no";
        string homepage = menu.ID.Equals(workspace.HomePage) ? "yes" : "no";
        string homepageShow = homepage == "yes" ? "home-page-show" : string.Empty;
        string hideFromMenuClass = menu.HideFromMenu ? "hide-from-menu" : "";
        int maxWidth = 120 - 30 * menu.Generation;
        string title = Map.Database.Localizer.Translate("Settings");
                       
        if (Map.Database.IsMultiLanguages)
        {
            menuName = Map.Database.Localizer.Translate(menu.Name);
        }%>
        <li class="dd-item dd3-item" data-id="<%=menu.ID %>">
        <div class="dd-handle dd3-handle"></div>
        <div class="dd3-content <%=hideFromMenuClass %>" url="<%=menuUrl%>"><span class="page-type <%=pageTypeClass%>"></span><span style="max-width:<%=maxWidth%>px" class="page-name" title="<%=menuName%>"><%=menuName%></span><span class="home-page <%=homepageShow %>"></span><span viewName="<%=configViewName%>" pk="<%=pk%>" hideFromMenu="<%=hideFromMenu %>" homepage="<%=homepage %>" class="dd3-settings"  title="<%=title %>"></span></div>
        <% if (menu.Menus.Count > 0)
            { %>
            <ol class="dd-list">
                <%  Html.RenderPartial("~/Views/Shared/Controls/Page/Pages.ascx", menu); %>
            </ol>
        <%} %>
        </li>
        
    <%} %>
<%} %>