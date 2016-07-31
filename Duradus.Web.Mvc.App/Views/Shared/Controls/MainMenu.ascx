<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<%
    Durados.Web.Mvc.Database database = Map.Database;

    Durados.Web.Mvc.Database configDatabase = Map.GetConfigDatabase();

    Durados.Web.Mvc.Database localizationDatabase = Map.GetLocalizationDatabase();

    database.RefreshMenus();
        
%>

<% bool displaySettings = !MenuHelper.HideSettings(); // (bool)(Map.Session["MenuDisplayState"] ?? false); %>
<div id="smoothmenu" class="ddsmoothmenu">
<ul>
    <% if (database.Menus.Values.Count > 0 && database.Menus.Values.Where(m => m.Root).FirstOrDefault() != null) 
        {%>
        <li><a class="newmenu" href="#">NEW</a>
        <ul>
            <%foreach (Durados.Web.Mvc.View view in database.Menus.Values.Where(m => m.Root).Single().Views.OrderBy(v => (int)(v == null ? 0 : v.Order)))
            {%>
            <% if (view != null && view.IsVisible())
               { %>
                <li><%= Html.ActionLink(Map.Database.Localizer.Translate("New") + " " + view.GetLocalizedDisplayName(), "Item", view.Controller, new { viewName = view.Name, add = "yes", guid = view.Name + "_Item_" }, null)%>
                </li>
            <%} %>
        <%}%>
        </ul>
        </li>
        <li><a class="newmenugap" href="#">&nbsp;</a>
    <%}%>
    </li>
    
    <% if (!Map.Database.HideMyStuff)
       { %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/UserStuffMenu.ascx"); %>          
    <%} %>

    <% foreach (Durados.Menu menu in database.Menus.Values.OrderBy(m => m.Ordinal))
       {%>
       <%if (menu.Root)
         { %>
              <%  Html.RenderPartial("~/Views/Shared/Controls/DatabaseMenu.ascx", menu); %>
           
       <%}
         else if (menu.HasVisibleViews() || menu.Pages.Count > 0)
         {
             string menuName = menu.Name;
             if (database.IsMultiLanguages)
             {
                 menuName = Map.Database.Localizer.Translate(menu.Name);
             }
             %>
           <li><a href="#"><%=menuName%></a>
                <ul>
                <%  Html.RenderPartial("~/Views/Shared/Controls/DatabaseMenu.ascx", menu); %>
                </ul>
            </li>
        <%} else if (menu.Special){%>
            <% if (!((SpecialMenu)menu).IsHidden() && !((SpecialMenu)menu).IsEmpty(Map.Database))
               {
                   string menuName = menu.Name;
                   if (database.IsMultiLanguages)
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
    <%   } %>
            
                      
       <%if (database.HasViewsWithNoMenu) 
         { %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/DatabaseMenu.ascx", database.ViewsWithNoMenu); %>
    <%} %>
    
    <% if (displaySettings && !SecurityHelper.IsDenied("", configDatabase.AllowConfigConfigRoles) && configDatabase.HasViewsWithNoMenu)
       {%>
        <li><a href="#"><%=Map.Database.Localizer.Translate("Admin")%></a>
            <ul>

            <%--start--%>
            <%
           

             %>
            <% foreach (Durados.Menu menu in configDatabase.Menus.Values.OrderBy(m => m.Ordinal))
       {%>
       <%if (menu.Special){%>
            <% if (!((SpecialMenu)menu).IsHidden() && (!((SpecialMenu)menu).IsEmpty(Map.GetConfigDatabase()) || !((SpecialMenu)menu).IsEmpty(Map.Database)))
               {
                   string menuName = menu.Name;
                   if (database.IsMultiLanguages)
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
    <%   } %>
            
                      
       



            <%--end--%>

            <%--<%  Html.RenderPartial("~/Views/Shared/Controls/DatabaseMenu.ascx", configDatabase.ViewsWithNoMenu); %>--%>
            <%--<% if (database.Views.ContainsKey(database.UserViewName))
               { %>

                <% Durados.Web.Mvc.View userView = (Durados.Web.Mvc.View)database.Views[database.UserViewName]; %>
                <li><%= Html.ActionLink(userView.GetLocalizedDisplayName(), userView.IndexAction, userView.Controller, new { viewName = userView.Name }, null)%></li>
            <%} %>
            <% if (database.Views.ContainsKey("Durados_Log"))
               { %>

                <% Durados.Web.Mvc.View logView = (Durados.Web.Mvc.View)database.Views["Durados_Log"]; %>
                <li><%= Html.ActionLink(logView.GetLocalizedDisplayName(), logView.IndexAction, logView.Controller, new { viewName = logView.Name }, null)%></li>
            <%} %>--%>
            
            <% if (!string.IsNullOrEmpty(database.RefreshUrl))
               { %>
                <li>
                    <a href='<%=database.RefreshUrl %>'><%=Map.Database.Localizer.Translate("Refresh")%></a>
                </li>
            <%} %>
            </ul>
            
        </li>
    <%} %>

    <%
        
        string role = Map.Database.GetUserRole();
        bool isAdmin = role == "Admin" || role == "Developer";
     %>
    <% if (localizationDatabase != null && displaySettings && isAdmin && !SecurityHelper.IsDenied(localizationDatabase.DenyLocalizationConfigRoles, "everyone") && database.Localization != null) // && localizationDatabase.HasViewsWithNoMenu && localizationDatabase.ViewsWithNoMenu != null)
       {%>
        <%--<li><a href="#"><%=Map.Database.Localizer.Translate("Localization")%></a>
            <ul>
            <%  Html.RenderPartial("~/Views/Shared/Controls/DatabaseMenu.ascx", localizationDatabase.ViewsWithNoMenu); %>
            </ul>
        </li>--%>
        <li><a href="#"><%= Map.Database.Localizer.Translate("Translation") %></a>
        <ul>
            <%foreach (Durados.Web.Mvc.View view in localizationDatabase.Views.Values)
            {%>
            <% if (view != null && view.IsVisible())
               { %>
                <li><%= Html.ActionLink(Map.Database.Localizer.Translate(view.DisplayName), view.IndexAction, view.Controller, new { viewName = view.Name }, null)%>
                </li>
            <%} %>
        <%}%>
        </ul>
        </li>
    <%} %>

</ul>
<%--<% if (ViewData["viewName"] != null)
   {%>
<a class="settings" <%=Map.Database.Localizer.Direction=="RTL" ? " style='float: left'":" style='float: right'" %> href='<%=Url.Action("MenuDisplayState", "Durados", new {viewName = ViewData["viewName"], guid = ViewData["guid"], display = !displaySettings}) %>'><%=Map.Database.Localizer.Translate(displaySettings ? "Hide Settings" : "Show Settings")%></a>
<%} %>--%>

<br /><br style="clear: both" />

</div>

