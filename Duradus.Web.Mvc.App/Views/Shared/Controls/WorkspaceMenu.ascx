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
            <%--<% if (workspace.Name == "My Stuff")
               { %>
                    <%  Html.RenderPartial("~/Views/Shared/Controls/UserStuffMenu.ascx"); %>          
            <%}
               else --%>
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

                <%
                
                      List<Link> links = new List<Link>();
                %>
                <% foreach (Durados.Web.Mvc.View view in Map.Database.GetWorkspaceViewsWithoutMenu(workspace)) {%>
                    <% if (view != null && view.IsVisible() && !view.HideInMenu)
                       { %>
                       <% 
                            string url = Url.Action(view.IndexAction, view.Controller, new { viewName = view.Name });
                            string title = view.GetLocalizedDisplayName();

                            links.Add(new Link() { Order = view.Order, Title = title, Url = url, Target = null });    
                       %>
                        
                    <%} %>
                <%} %>


                 
                <% foreach (Durados.Page page in Map.Database.GetWorkspacePagesWithoutMenu(workspace)) {%>
                    <% if (page != null)
                       { %>
                       <% 
                           string url = Url.Action("Page", "Home", new { pageId = page.ID });
                           string target = page.Target;
                           string title = page.Title;

                           
                           if (page.PageType == PageType.External)
                           {
                               if (page.ExternalNewPage != null)
                               {
                                   string[] urlVal = page.ExternalNewPage.Split('|');

                                   if (urlVal.Length == 3)
                                   {
                                       url = urlVal[2];
                                       url = Map.Database.GetEncryptedUrl(url);

                                   }
                               }
                               if (page.NewTab)
                                   target = "_blank";
                           }
                           else if (page.PageType == PageType.ReportingServices)
                           {
                               //url = "/Reports/RdlcWebForm.aspx?ReportName=" + page.ReportName + "&ReportDisplayName=" + page.ReportDisplayName;
                               url = Url.Action("RdlcReport", "Durados", new { reportName = page.ReportName, reportDisplayName = page.ReportDisplayName });
                           }

                           links.Add(new Link() { Order = page.Order, Title = title, Url = url, Target = null });    
                        %>
                        
                    <%} %>
                <%} %>

                <% foreach (Link link in links.OrderBy(l => (int)(l == null ? 0 : l.Order)))
                    {%>

                    <%
                        string target = string.Empty;
                           
                        if (!string.IsNullOrEmpty(link.Target))
                            target = "target='" + link.Target + "'";
                            
                    %>
                    <li>
                        <a <%=target %> href="<%=link.Url %>"><span><%=link.Title %></span></a>
                    </li>
                    
                <%} %>

            <% foreach (Durados.Menu menu in Map.Database.GetWorkspaceMenus(workspace).OrderBy(m => m.Ordinal))
               {%>
               <%if (menu.HasVisibleViews() || menu.Pages.Count > 0)
                 {
                     string menuName = menu.Name;
                     if (Map.Database.IsMultiLanguages)
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
            <%   } %>


            <%} %>

            <%--<li class="i-level-menu"><a class="dd" onclick="return false;" href="javascript:void;"><span>My Stuff</span></a>
              <ul style="display: none; visibility: hidden;">
                <li><a href="javascript:void;"><span>SubCat1</span></a></li>
                <li class="i-level-menu"> <a href="javascript:void;"><span>SubCat2</span></a></li>
                <li class="i-level-menu"> <a href="javascript:void;"><span>SubCat3</span></a></li>
              </ul>
            </li>
            <li class="i-level-menu"> <a href="index.html"><span>Admin</span></a></li>
            <li class="i-level-menu"> <a href="index.html"><span>Names</span></a></li>
            <li class="i-level-menu"> <a href="index.html"><span>Users</span></a></li>--%>
          </ul>
        </div>

        <%} %>