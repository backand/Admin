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
               <%-- <% if (!Map.Database.HideMyStuff)
                   { %>
                    <%  Html.RenderPartial("~/Views/Shared/Controls/UserStuffMenu.ascx"); %>          
                <%} %>--%>

               <%if (workspace.Name == "Admin")
               { %>

                   <%
                    SpecialMenu selectedMenu = null;
                    if (!string.IsNullOrEmpty(Request.QueryString["menuId"]))
                    {
                        selectedMenu = workspace.GetSpecialMenu(Convert.ToInt32(Request.QueryString["menuId"].TrimEnd('#')));

                    }
                    else
                    {
                        if (ViewData.ContainsKey("ViewName"))
                        {
                            selectedMenu = workspace.GetSpecialMenu(viewName);

                        }
                    }
                    %>
                    <% foreach (Durados.SpecialMenu menu in Map.Database.GetAdminWorkspace().SpecialMenus.Values.OrderBy(m => m.Ordinal))
                    {%>
                        <%if (menu.Special){%>
                                <% if (!((SpecialMenu)menu).IsHidden() && (menu.Name != "Developer" || (menu.Name == "Developer" && Database.GetUserRole() == "Developer"))) //&& (!((SpecialMenu)menu).IsEmpty(Map.GetConfigDatabase()) || !((SpecialMenu)menu).IsEmpty(Map.Database)))
                                {
                                   string menuName = menu.Name;
                                   if (Map.Database.IsMultiLanguages)
                                   {
                                       menuName = Map.Database.Localizer.Translate(menu.Name);
                                   }

                                   string selected2 = string.Empty;
                                   if (selectedMenu != null && (selectedMenu.Equals(menu) || selectedMenu.Parent != null && selectedMenu.Parent.Equals(menu)))
                                   {
                                       selected2 = "class='selected2'";
                                   }
                                    %>
                                <li <%=selected2 %>><a href="#"><%=menuName%></a>
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
                SpecialMenu selectedMenu = null;
                if (!string.IsNullOrEmpty(Request.QueryString["menuId"]))
                {
                    selectedMenu = workspace.GetSpecialMenu(Convert.ToInt32(Request.QueryString["menuId"].TrimEnd('#')));

                }
                else
                {
                    if (ViewData.ContainsKey("ViewName"))
                    {
                        selectedMenu = workspace.GetSpecialMenu(viewName);

                    }
                }
            %>
            <% foreach (SpecialMenu menu in workspace.SpecialMenus.Values.Where(m => !m.HideFromMenu).OrderBy(m=>m.Ordinal))
               { %>
               <%--<% bool isAllowedView =;// menu.LinkType.Equals(LinkType.View) && ( string.IsNullOrEmpty(menu.ViewName) || (Map.Database.Views.ContainsKey(menu.ViewName) && Map.Database.Views[menu.ViewName].IsAllow()) || (Map.GetConfigDatabase().Views.ContainsKey(menu.ViewName) && Map.GetConfigDatabase().Views[menu.ViewName].IsAllow())); 
                  bool isAllowedPage =  menu.LinkType.Equals(LinkType.Page) && (Map.Database.Pages.ContainsKey(Convert.ToInt32(menu.ViewName)) && Map.Database.Pages[Convert.ToInt32(menu.ViewName)].IsAllow() );
                  bool isNotViewOrPage = !menu.LinkType.Equals(LinkType.View) && !menu.LinkType.Equals(LinkType.Page);
                     
                   %>--%>
                   <% menu.Parent = null; %>
        
                <% if (Map.Database.IsAllowMenu(menu))// || !string.IsNullOrEmpty(menu.Url))
                   {
               string menuName = menu.Name;
               //string menuUrl = string.IsNullOrEmpty(menu.Url) ? "#" : menu.Url;
               string menuUrl = menu.GetMenuUrl();
               string target = string.Empty;
               if ((menu.LinkType.Equals(LinkType.Page) || menu.LinkType.Equals(LinkType.Report)) && !string.IsNullOrEmpty(menu.ViewName) && Map.Database.Pages.ContainsKey(Convert.ToInt32(menu.ViewName)))
               {
                   Durados.Page page = Map.Database.Pages[Convert.ToInt32(menu.ViewName)];
                   if (page.PageType.Equals(PageType.External) && !string.IsNullOrEmpty(page.ExternalNewPage))
                   {
                       string[] urlVal = page.ExternalNewPage.Split('|');
                       if (urlVal.Length == 3)
                       {
                           menuUrl = urlVal[2];

                       }
                       
                   }
                   else if (page.PageType == Durados.PageType.ReportingServices)
                   {
                       //action = "/Reports/RdlcWebForm.aspx?ReportName=" + page.ReportName + "&ReportDisplayName=" + page.ReportDisplayName;
                       menuUrl = Url.Action("RdlcReport", "Durados", new { reportName = page.ReportName, reportDisplayName = page.ReportDisplayName, menuId = menu.ID });
                   }
           
                   if (page.NewTab)
                   {
                       target = "target='_blank'";
                   }
               }
                       
               string selected2 = string.Empty;

                if (selectedMenu != null && (selectedMenu.Equals(menu) || selectedMenu.Parent != null && selectedMenu.Parent.Equals(menu)))
                {
                    selected2 = "class='selected2'";
                }
               
                       
               if (Map.Database.IsMultiLanguages)
               {
                   menuName = Map.Database.Localizer.Translate(menu.Name);
               }%>
        <li <%=selected2 %>><a <%=target %> href="<%=menuUrl%>"><%=menuName%></a>
        
        <% if (menu.Menus.Values.Where(m => Map.Database.IsAllowMenu(m) && !m.HideFromMenu).Count() > 0 && Map.Database.MenuType == MenuType.PullDown)
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