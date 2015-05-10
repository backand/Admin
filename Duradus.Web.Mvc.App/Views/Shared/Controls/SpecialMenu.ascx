<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.SpecialMenu>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<% if (Model!=null && Model.Menus!= null){ %>


            <%
                string viewName = (ViewData["viewName"] ?? string.Empty).ToString();
                Workspace workspace = MenuHelper.GetCurrentWorkspace(viewName);
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
<% foreach (Durados.SpecialMenu menu in Model.Menus.Values.Where(m=>!m.HideFromMenu).OrderBy(m=>m.Ordinal).ToList()) {%>
    <% menu.Parent = Model; %>
    <%--<% bool isAllowedView = menu.LinkType.Equals(LinkType.View) && ( string.IsNullOrEmpty(menu.ViewName) || (Map.Database.Views.ContainsKey(menu.ViewName) && Map.Database.Views[menu.ViewName].IsAllow()) || (Map.GetConfigDatabase().Views.ContainsKey(menu.ViewName) && Map.GetConfigDatabase().Views[menu.ViewName].IsAllow())); 
       bool isAllowedPage =  menu.LinkType.Equals(LinkType.Page) && (Map.Database.Pages.ContainsKey(Convert.ToInt32(menu.ViewName)) && Map.Database.Pages[Convert.ToInt32(menu.ViewName)].IsAllow() );
       bool isNotViewOrPage = !menu.LinkType.Equals(LinkType.View) && !menu.LinkType.Equals(LinkType.Page);
                     
                   %>--%>
    <% if (Map.Database.IsAllowMenu(menu))// || !string.IsNullOrEmpty(menu.Url))!menu.LinkType.Equals(LinkType.View) || string.IsNullOrEmpty(menu.ViewName) || (Map.Database.Views.ContainsKey(menu.ViewName) && Map.Database.Views[menu.ViewName].IsAllow()) || (Map.GetConfigDatabase().Views.ContainsKey(menu.ViewName) && Map.GetConfigDatabase().Views[menu.ViewName].IsAllow())
        { 
                string menuName = menu.Name;
               //string menuUrl = string.IsNullOrEmpty(menu.Url) ? "#" : menu.Url;
               string menuUrl = menu.GetMenuUrl();
               string target = string.Empty;

               string selected3 = menu.Equals(selectedMenu) ? "class=\"selected3\"" : string.Empty;
           
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
               if (Map.Database.IsMultiLanguages)
               {
                   menuName = Map.Database.Localizer.Translate(menu.Name);
               }%>
        <li <%=selected3 %>><a <%=target %> href="<%=menuUrl%>"><%=menuName%></a>
        <% if (menu.Menus.Count>0){%>
           <ul>
            <%  Html.RenderPartial("~/Views/Shared/Controls/SpecialMenu.ascx", menu); %>
            </ul>
        </li>
    <%} %>
<%} %>
<%} %>
<% if (Model!=null && Model.Links!= null){ %>
<% foreach (Durados.Link link in Model.Links.OrderBy(l=> (int)(l==null? 0 : l.Order))) {%>
    <% string target = string.IsNullOrEmpty(link.Target) ? string.Empty : "target='" + link.Target + "'"; %>
    <% var targetAtt = string.IsNullOrEmpty(link.Target) ? null : new { target = link.Target }; %>
    
    <%  bool hasViewName = !string.IsNullOrEmpty(link.ViewName); %>
    <%  Durados.Web.Mvc.View view = null; %>
    <% if (hasViewName &&  Map.Database.Views.ContainsKey(link.ViewName)){ %>
    <% view = (Durados.Web.Mvc.View)Map.Database.Views[link.ViewName]; %>
    <%}
       else if (hasViewName && Map.GetConfigDatabase().Views.ContainsKey(link.ViewName))
       { %>
    <% view = (Durados.Web.Mvc.View)Map.GetConfigDatabase().Views[link.ViewName]; %>
    <%} %>

    <% Durados.Workspace linkWorkspace = null; %>
    
    <% if (hasViewName && view != null ){ %>
    
    <% linkWorkspace = link.GetWorkspace(view.Database); %>
    <%} %>

    <% if (hasViewName ){ %>
    <% if (view != null && view.IsAllow())
       { %>
       <% object routeParams = null;
          if (!string.IsNullOrEmpty(link.Url))
              routeParams = new { viewName = view.Name, path = Model.GetPath(), parameters = link.Url };
          else
              routeParams = new { viewName = view.Name, path = Model.GetPath()};
          
           string title = string.Empty;
           
           if (string.IsNullOrEmpty(link.Title)) {
               title = view.GetLocalizedDisplayName();
           } 
           else 
           {
               if (Map.Database.IsMultiLanguages)
               {
                   title = Map.Database.Localizer.Translate(link.Title);
               }
               else
               {
                   title = link.Title;
               }
           }
           %>
        <li><%= Html.ActionLink(title, view.IndexAction, view.Controller, routeParams, targetAtt)%>
        </li>
    <%} %>
    <%}
       else if (!string.IsNullOrEmpty(link.Url) && !string.IsNullOrEmpty(link.Title) && (linkWorkspace == null || !Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsDenied(linkWorkspace.DenySelectRoles, linkWorkspace.AllowSelectRoles)))
       {
           string title = string.Empty;

           if (Map.Database.IsMultiLanguages)
           {
               title = Map.Database.Localizer.Translate(link.Title);
           }
           else
           {
               title = link.Title;
           }

           %>
        <li><a href='<%=link.Url %>' <%=target %> ><%=title%></a></li>
    <%} %>
    <% else if (!string.IsNullOrEmpty(link.ReportServicePath) && !string.IsNullOrEmpty(link.Title) && (linkWorkspace == null || !Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsDenied(linkWorkspace.DenySelectRoles, linkWorkspace.AllowSelectRoles)))
        {
            string title = string.Empty;

            if (Map.Database.IsMultiLanguages)
            {
                title = Map.Database.Localizer.Translate(link.Title);
            }
            else
            {
                title = link.Title;
            }
           if (link.Target == null || link.Target.ToLower() != "blank"){
           %>
            <li><%= Html.ActionLink(title, "RdlcReport", "Durados", new { reportName = link.ReportServicePath, reportDisplayName = link.Title, path = Model.GetPath() }, targetAtt)%>
            </li>
            <%} else {
             // string action = Url.Action("RdlcReport", "Durados", new { reportName = link.ReportServicePath, reportDisplayName = link.Title, path = Model.GetPath() });
              string action = "../../Reports/RdlcWebForm.aspx?ReportName=" + link.ReportServicePath + "&ReportDisplayName=" + link.Title + "&NewWindow=True";
                %>
                <li>
                    <a href='#'>
                    <span onclick='var win = window.open("<%=action %>"); win.moveTo(screen.availLeft, screen.availTop);
win.resizeTo(screen.availWidth, screen.availHeight);'><%=title %></span>
                    </a>
                </li>
        <%} %>
    <%} %>
<%} %>
<%} %>
<%} %>