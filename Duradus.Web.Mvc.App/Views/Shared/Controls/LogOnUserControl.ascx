<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>
<% bool isMobile = Durados.Web.Infrastructure.General.IsMobile(); %>
<%if (((System.Web.Configuration.AuthenticationSection)System.Web.HttpContext.Current.GetSection("system.web/authentication")).Mode != System.Web.Configuration.AuthenticationMode.None)
  { %>
<% Durados.Localization.ILocalizer localizer = Map.Database.Localizer; %>
<%
      if (Request.IsAuthenticated)
      {

          Durados.Workspace currentWorkspace = Durados.Web.Mvc.UI.Helpers.MenuHelper.GetCurrentWorkspace(ViewData["ViewName"] as string);
          int adminWorkspaceId = Map.Database.GetAdminWorkspaceId();
%>
<div>
    <ul class="sf-menu sf-js-enabled" id="mymenu">
     <% if (!isMobile)
        {%>
       
        <%  if (!Map.IsMainMap && (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin")))
            { %>
        <% string adminButtonUrl = currentWorkspace.ID == adminWorkspaceId ? Durados.Web.Mvc.Maps.GetPublicButtonUrl(Map) : Durados.Web.Mvc.Maps.GetAdminButtonUrl(Map); %>
        <% string adminButtonText = currentWorkspace.ID == adminWorkspaceId ? Map.Database.GetPublicWorkspace().Name : Map.Database.GetAdminWorkspace().Name; %>
        <% 
            var builder = new UriBuilder(Request.Url);

            string previewUrl = Map.GetPreviewPath();
            string userPreviewUrl = Map.Database.UserPreviewUrl;
        
        %>
       
        <li style="margin-right: 5px;display:none">
             <a target="angularbknd" href="<%=(!string.IsNullOrEmpty(userPreviewUrl))?userPreviewUrl:"javascript:void(0);"%>">
       
            <div class="preview-notice" style="display: none;"></div>
            <img style="vertical-align: middle;" src="<%= Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/angular-icon-1.png"%>" />
            <%= Map.Database.Localizer.Translate("User Preview")%></a>
            <%if (string.IsNullOrEmpty(userPreviewUrl )){%>
            <ul  id="userPreview" style="line-height: 20px;width:250px;height:80px;display:none;background:#f3b858;border:1px solid #1ba085;padding: 8px;">
                    <li style="line-height: 20px;display:inline;"><a style="line-height: 20px;display:inline;border-left:none #f3b858" target="angularbknd"  href="/Admin/IndexPage/Database">Click here</a> to setup your ng-back url.</li>
                     <li style="line-height: 20px;display:inline;">--Or<a  style="line-height: 20px;display:inline;border-left:none #f3b858" target="angularbknd" href='<%=previewUrl%>'>Click here</a> for your User Preview branched from the Backand master code.</li>
                
            </ul>
            <%} %>
        </li>
        
        <li style="margin-right: 5px;"><a href='<%=adminButtonUrl%>'>
            <%=adminButtonText%></a>
            <%if (currentWorkspace.ID != Map.Database.GetAdminWorkspaceId())
              { %>
            <ul>
                <li><a href="/Admin/Index/Database">Default Settings</a></li>
                <li><a href="/Admin/Index/View">Tables & Views</a></li>
                <li><a href="/Admin/Index/Workspace">Workspaces</a></li>
                <li><a href="/Home/IndexPage/v_durados_User">Users</a></li>
                <li><a href="/Admin/Index/Rule">Business Rules</a></li>
                <li><a href="/Home/Index/Durados_Log">Trace</a></li>
            </ul>
            <%} %>
        </li>
        <%} %>
        <% if (Map.Database.Localization != null && (Map.Database.IsMultiLanguages || Map.Database.Localizer.Languages.Where(l => l.Active).Count() > 1))
           { %>
        <li>
            <%=Html.DropDownList("languageDropDown", Html.GetLanguages(), new { id = "languageDropDown", onchange = "SetLanguage('" + ViewData["guid"] + "')" })%></li>
        <%} %>
        <%} %>
        <li><a href="#">
            <%= Html.Encode(Map.Database.GetCurrentUsername()) %></a>
             
            <ul>
                <li>
                    <%= Html.ActionLink(localizer.Translate("Log Out"), "LogOff", "Account")%>
                </li>
                <% if (Map.Database.HasChangePassword && !isMobile)
                   { %>
                <li>
                    <% =Html.ActionLink(Map.Database.Localizer.Translate("Change Password"), "ChangePassword", "Account")%>
                </li>
                <% if (!Durados.Web.Infrastructure.General.IsMobile())
                   { %>
                <% if (!Durados.Web.Mvc.Infrastructure.Http.IsAlias())
                   { %>
                <li><a href='<%= Durados.Web.Mvc.Maps.Instance.DuradosMap.Url + "/apps"%>'>
                    <%= Map.Database.Localizer.Translate("My Consoles")%></a></li>
                <%} %>
                <li></li>
                <li><a href="http://blog.backand.com/questions" target="_help">
                    <%= Map.Database.Localizer.Translate("Support")%></a> </li>
                <%} %>
            </ul>
            
        </li>
        <%} %>
        <%
           }
           else
           {
        %>
        <a href="/Account/LogOn"><%=localizer.Translate("Log On")%></a>
        <%
           }
        %>
        <%} %>
    </ul>
</div>
