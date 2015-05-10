<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

    <% string viewName = (ViewData["viewName"] ?? string.Empty).ToString(); %>
    <%
        Workspace workspace = MenuHelper.GetCurrentWorkspace(viewName);
    %>    

    <% if (workspace != null){ %>
    <div class="navigation-left">
        
          <ul class="sf-menu sf-js-enabled" id="mainmenu">
                <% if (!Map.Database.HideMyStuff)
                   { %>
                    <%  Html.RenderPartial("~/Views/Shared/Controls/UserStuffMenu.ascx"); %>          
                <%} %>

               <%if (workspace.Name == "Admin")
               { %>

                    <% foreach (Durados.Menu menu in Map.GetConfigDatabase().Menus.Values.OrderBy(m => m.Ordinal))
                    {%>
                        <%if (menu.Special){%>
                                <% if (!((SpecialMenu)menu).IsHidden() && (!((SpecialMenu)menu).IsEmpty(Map.GetConfigDatabase()) || !((SpecialMenu)menu).IsEmpty(Map.Database)))
                                {
                                   string menuName = menu.Name;
                                   if (Map.Database.IsMultiLanguages)
                                   {
                                       menuName = Map.Database.Localizer.Translate(menu.Name);
                                   }
                                    %>
                                <li><a href="#"><%=menuName%></a>
                                    <ul>
                                        <%  Html.RenderPartial("~/Views/Shared/Controls/SpecialMenu.ascx", menu); %>
                                    </ul>
                                </li>
                                <%} %> 
                        <%} %>
                    <%} %>

                    <% if (Map.Database.Menus.ContainsKey("New Views") && Map.Database.Menus["New Views"].HasVisibleViews()){ %>
                    <% Durados.Menu newViewMenu = Map.Database.Menus["New Views"]; %>
                    <li><a href="#"><%=newViewMenu.Name%></a>
                        <ul>
                        <%  Html.RenderPartial("~/Views/Shared/Controls/DatabaseMenu.ascx", newViewMenu); %>
                        </ul>
                    </li>
                    <%} %>
                    <%
                   Durados.Web.Mvc.Database localizationDatabase = Map.GetLocalizationDatabase();
                   Durados.Web.Mvc.Database database = Map.Database;

                         %>
                    <% if (localizationDatabase != null && !SecurityHelper.IsDenied(localizationDatabase.DenyLocalizationConfigRoles, "everyone") && database.Localization != null)
       {%>
         <li><a href="#"><%= Map.Database.Localizer.Translate("Translation") %></a>
        <ul>
            <%foreach (Durados.Web.Mvc.View view in localizationDatabase.Views.Values)
            {%>
            <% if (view != null && view.IsVisible())
               { %>
                <li><%= Html.ActionLink(view.DisplayName, view.IndexAction, view.Controller, new { viewName = view.Name }, null)%>
                </li>
            <%} %>
        <%}%>
        </ul>
        </li>
    <%} %>

                <%} else { %>

                
            <% foreach (SpecialMenu menu in Map.Database.SpecialMenus.Values){ %>
                <% if (!menu.IsEmpty(Map.Database) || !string.IsNullOrEmpty(menu.Url))
                   {
               string menuName = menu.Name;
               string menuUrl = string.IsNullOrEmpty(menu.Url) ? "#" : menu.Url;
               
               if (Map.Database.IsMultiLanguages)
               {
                   menuName = Map.Database.Localizer.Translate(menu.Name);
               }%>
        <li><a href="<%=menuUrl%>"><%=menuName%></a>
        <% if (menu.Menus.Count > 0)
           {%>
            <ul>
            <%  Html.RenderPartial("~/Views/Shared/Controls/SpecialMenu.ascx", menu); %>
            </ul>
            <%} %>
        </li>
    <%} %>
            <%} %>
            <%} %>

            
          </ul>
        </div>

        <%} %>