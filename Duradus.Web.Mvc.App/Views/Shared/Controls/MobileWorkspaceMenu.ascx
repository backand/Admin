<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Workspace>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<% string viewName = (ViewData["viewName"] ?? string.Empty).ToString(); %>
<%
    Durados.Workspace workspace = Model;
%>
<% if (false){ %>
        <% foreach (Durados.Web.Mvc.View view in Map.Database.GetWorkspaceViewsWithoutMenu(workspace).OrderBy(v => (int)(v == null ? 0 : v.Order)))
           {%>
        <% if (view != null && view.IsVisible() && !view.HideInMenu)
           { %>
        <% 
            string url = Url.Action(view.IndexAction, view.Controller, new { viewName = view.Name });
               
        %>
        <li><a href="<%=url %>"><span>
            <%=view.GetLocalizedDisplayName() %></span></a> </li>
        <%} %>
        <%} %>
        <% foreach (Durados.Page page in Map.Database.GetWorkspacePagesWithoutMenu(workspace).OrderBy(p => (int)(p == null ? 0 : p.Order)))
           {%>
        <% if (page != null)
           { %>
        <% 
                           string url = Url.Action("Page", "Home", new { pageId = page.ID });
                           string target = string.Empty;

                           if (page.PageType == PageType.External)
                           {
                               if (page.ExternalNewPage != null)
                               {
                                   string[] urlVal = page.ExternalNewPage.Split('|');

                                   if (urlVal.Length == 3)
                                   {
                                       url = urlVal[2];

                                   }
                               }
                               if (!string.IsNullOrEmpty(page.Target))
                                   target = "target='" + page.Target + "'";
                               else if (page.NewTab)
                                   target = "target='_blank'";
                           }
                           else if (page.PageType == PageType.ReportingServices)
                           {
                               url = Url.Action("RdlcReport", "Durados", new { reportName = page.ReportName, reportDisplayName = page.ReportDisplayName });
                           }
        %>
        <li><a <%=target %> href="<%=url %>"><span>
            <%=page.Title %></span></a> </li>
        <%} %>
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
              
              string selected = string.Empty;
                
        %>
        <li><a href="#">
            <%=menuName%></a>
            <ul <%=selected %> >
                <%  Html.RenderPartial("~/Views/Shared/Controls/DatabaseMenu.ascx", menu); %>
            </ul>
        </li>
        <%}
          %>
        
        <%   } %>
        
        <%} else {%>

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

                
               
                       
               if (Map.Database.IsMultiLanguages)
               {
                   menuName = Map.Database.Localizer.Translate(menu.Name);
               }%>
        
        <% if (menu.Menus.Values.Where(m => Map.Database.IsAllowMenu(m)).Count() > 0 && Map.Database.MenuType == MenuType.PullDown)
           {%>
            <li ><a><%=menuName%></a>
                <ul>
            <li ><a <%=target %> href="<%=menuUrl%>"><%=menuName%></a></li>
            <%  Html.RenderPartial("~/Views/Shared/Controls/SpecialMenu.ascx", menu); %>
            </ul>
            <%} else { %>
                <li><a <%=target %> href="<%=menuUrl%>"><%=menuName%></a>
        
            <%} %>
        </li>
    <%} %>


        <%} %>
        <%} %>