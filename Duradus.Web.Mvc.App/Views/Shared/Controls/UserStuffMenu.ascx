<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<%
    string basePath = Html.GetAppBasePath();
    string role = Map.Database.GetUserRole();
    bool isAdmin = role == "Admin" || role == "Developer";
%>
<li><a href="#"><%=Map.Database.Localizer.Translate("My Stuff")%></a>
    <ul>
    <%
        
    Durados.Web.Mvc.View durados_v_MessageBoardView = (Durados.Web.Mvc.View)Map.Database.Views["durados_v_MessageBoard"];
            
     %>
     <% if (Durados.Web.Mvc.Maps.MultiTenancy)
        { %>
        <%
            string url = Durados.Web.Mvc.Maps.Instance.DuradosMap.Url + "/apps";
            
            string chartsUrl = Url.Action("Charts", "Home");
             %>
             <%--<% if (!Durados.Web.Mvc.Infrastructure.Http.IsAlias())
                { %>
        <li><a href='<%= url%>'><span><%= Map.Database.Localizer.Translate("My Consoles")%></span></a></li>
        <%} %>--%>
        <%--<li><a href='<%= chartsUrl%>'><span><%= Map.Database.Localizer.Translate("My Charts")%></span></a></li>--%>
        
    <%} %>
<%--        <li><%= Html.ActionLink(durados_v_MessageBoardView.GetLocalizedDisplayName(), durados_v_MessageBoardView.IndexAction, durados_v_MessageBoardView.Controller, new {viewName = durados_v_MessageBoardView.Name}, null)%></li>
--%>  
    <li><a href="#"><%=Map.Database.Localizer.Translate("Bookmarks")%></a>
    <ul id="bookmarksMenu">
        
    <li><a href="#" onclick="BookmarkPage('<%= ViewData["guid"] %>');return false;" title=""><%=Map.Database.Localizer.Translate("Add Bookmark")%></a></li>
   
    <li><a href="#" onclick="openBookmarksManager()"><%=Map.Database.Localizer.Translate("Bookmark Manager")%></a></li>

    <% foreach (Durados.Bookmark bm in BookmarksHelper.GetCachedBookmarks(0))
       { %>
       <li><a href="<%= basePath %>Durados/LoadBookmark/?id=<%= bm.Id %>" title="<%= bm.Description%>"><%= bm.Name %></a></li>
    <% } %>
    </ul>
</li>
<%--<li><a href="#"><%=Map.Database.Localizer.Translate("Recents")%></a>
     <ul id="recentsMenu">
         <% foreach (Durados.Bookmark recent in BookmarksHelper.GetCachedBookmarks(1))
           { %>
           <li><a href="<%= basePath %>Durados/LoadBookmark/?id=<%= recent.Id %>" title="<%= recent.Description%>"><%= recent.Name%></a></li>
        <% } %>
    </ul>
</li>
--%>
<% if (isAdmin && false) {%>
        <% 
               string menuOrganizerUrl = Url.Action("MenuOrganizer", "Admin");
               string title = Map.Database.Localizer.Translate("Menu Organizer");
        %>
            <li><a href="#" onclick="menuOrganizer('<%= menuOrganizerUrl%>','<%= title%>');return false;"><span><%= title%></span></a></li>
        <%} %>
    </ul>
</li>  