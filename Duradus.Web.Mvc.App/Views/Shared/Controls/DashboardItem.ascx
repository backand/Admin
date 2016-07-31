<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Link>" %>

<div class="portlet" d_type="<%=string.IsNullOrEmpty(Model.Description) ? "grid" : "desc" %>">
	<div class="portlet-header"><%= Model.Title %></div>
	<div class="portlet-content">
	<% Durados.Link link = Model; %>
	    
	<% if (string.IsNullOrEmpty(Model.Description))
    { %>
        <input type="hidden" name="guid" value="<%= link.ViewName + "_" + Durados.Web.Mvc.Infrastructure.ShortGuid.Next() + "_"%>" />
        <input type="hidden" name="ViewName" value="<%= link.ViewName %>" />
        <div class="portlet-grid">
            <img  src='<%= Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/progress-wheel.gif"%>'  />
        </div>
	<%}
    else
    { %>
	    <div><%= Model.Description%></div>
	    <%} %>
	    <div class="portlet-link">
	    <% string target = string.IsNullOrEmpty(link.Target) ? string.Empty : "target='" + link.Target + "'"; %>
        <% var targetAtt = string.IsNullOrEmpty(link.Target) ? null : new { target = link.Target }; %>
        <% Durados.Workspace linkWorkspace = link.GetWorkspace(Map.Database); %>
        <% string linkText = Map.Database.Localizer.Translate("View..."); %>
        
        <% if (!string.IsNullOrEmpty(link.ViewName) && Map.Database.Views.ContainsKey(link.ViewName)){ %>
        <% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Map.Database.Views[link.ViewName]; %>
        <% if (view != null && view.IsAllow())
           { %>
           <% object routeParams = null;
              if (!string.IsNullOrEmpty(link.Url))
                  routeParams = new { viewName = view.Name, string.Empty, parameters = link.Url };
              else
                  routeParams = new { viewName = view.Name, path = string.Empty};
              
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
            <%= Html.ActionLink(linkText, view.IndexAction, view.Controller, routeParams, targetAtt)%>
                
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
            <a href='<%=link.Url %>' <%=target %> ><%=linkText%></a>
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
                <%= Html.ActionLink(linkText, "RdlcReport", "Durados", new { reportName = link.ReportServicePath, reportDisplayName = link.Title, path = string.Empty }, targetAtt)%>
                <%} else {
                 // string action = Url.Action("RdlcReport", "Durados", new { reportName = link.ReportServicePath, reportDisplayName = link.Title, path = Model.GetPath() });
                  //string action = "../../Reports/RdlcWebForm.aspx?ReportName=" + link.ReportServicePath + "&ReportDisplayName=" + link.Title + "&NewWindow=True";
                      string action = Url.Action("RdlcReport", "Durados", new { reportName = link.ReportServicePath, reportDisplayName = link.Title, path = string.Empty });
                    %>
                        <a href='#'>
                        <span onclick='var win = window.open("<%=action %>"); win.moveTo(screen.availLeft, screen.availTop);
    win.resizeTo(screen.availWidth, screen.availHeight);'><%=title %></span>
                        </a>
            <%} %>
        <%} %>
	    
	    </div>
	</div>
</div>