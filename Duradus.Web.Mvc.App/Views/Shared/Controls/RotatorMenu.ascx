<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

        <% string viewName = (ViewData["viewName"] ?? string.Empty).ToString(); %>
        <%
            Workspace workspace = MenuHelper.GetCurrentWorkspace(viewName);
        %> 
        <%
            int selected = 1;
            List<Durados.Workspace> workspaces = Map.Database.Workspaces.Values.Where(w=>!SecurityHelper.IsDenied(w.GetDenySelectRoles(Database), w.GetAllowSelectRoles(Database))).OrderBy(w => w.Ordinal).ToList();

            string role = Map.Database.GetUserRole();
            if (!(role == "Admin" || role == "Developer"))
            {
                for (int i = workspaces.Count - 1; i >= 0; i--)
                {
                    Durados.Workspace w = workspaces[i];
                    if (w.Name == "Admin")
                        workspaces.Remove(w);
                }
            }
            for (int i = 0; i < workspaces.Count; i++)
            {
                if (workspaces[i].Name == workspace.Name)
                    selected = i + 1;
            }
            
            string undefined = "&nbsp;";//Map.Database.Localizer.Translate("&nbsp;");
            string url = Url.Action("Default", "Home");
        
        %>
          

        <div class="zone">
          <div id="menuheadbutton" class="item<%=selected %>" >
            <div class="rotator"></div>
          </div>
          <div class="submenu-container">
            <div class="icon-container">
              <div class="rowcontainer"> 
                <% if (workspaces.Count > 0){ %>
                <a href="<%=url + "?workspaceId=" + workspaces[0].ID %>" class="submenu-item item<%="1" %>"><%=Map.Database.Localizer.Translate(workspaces[0].Name)%></a> 
                <%} else { %>
                <span class="submenu-item"><%=undefined %></span>
                <%} %>
                <% if (workspaces.Count > 1){ %>
                <a href="<%=url + "?workspaceId=" + workspaces[1].ID %>" class="submenu-item item<%="2" %>"><%=Map.Database.Localizer.Translate(workspaces[1].Name)%></a> 
                <%} else { %>
                <span class="submenu-item"><%=undefined %></span>
                <%} %>
                <div class="clearfix"></div>
              </div>
              <div class="rowcontainer"> 
                <% if (workspaces.Count > 2){ %>
                <a href="<%=url + "?workspaceId=" + workspaces[2].ID %>" class="submenu-item item<%="3" %>"><%=Map.Database.Localizer.Translate(workspaces[2].Name)%></a> 
                <%} else { %>
                <span class="submenu-item"><%=undefined %></span>
                <%} %>
                <% if (workspaces.Count > 3){ %>
                <a href="<%=url + "?workspaceId=" + workspaces[3].ID %>" class="submenu-item item<%="4" %>"><%=Map.Database.Localizer.Translate(workspaces[3].Name)%></a> 
                <%} else { %>
                <span class="submenu-item"><%=undefined %></span>
                <%} %>
                <% if (workspaces.Count > 4){ %>
                <a href="<%=url + "?workspaceId=" + workspaces[4].ID %>" class="submenu-item item<%="5" %>"><%=Map.Database.Localizer.Translate(workspaces[4].Name)%></a> 
                <%} else { %>
                <span class="submenu-item"><%=undefined %></span>
                <%} %>
                <div class="clearfix"></div>
              </div>
              <div class="rowcontainer"> 
                <% if (workspaces.Count > 5){ %>
                <a href="<%=url + "?workspaceId=" + workspaces[5].ID %>" class="submenu-item item<%="6" %>"><%=Map.Database.Localizer.Translate(workspaces[5].Name)%></a> 
                <%} else { %>
                <span class="submenu-item"><%=undefined %></span>
                <%} %>
                <% if (workspaces.Count > 6){ %>
                <a href="<%=url + "?workspaceId=" + workspaces[6].ID %>" class="submenu-item item<%="7" %>"><%=Map.Database.Localizer.Translate(workspaces[6].Name)%></a> 
                <%} else { %>
                <span class="submenu-item"><%=undefined %></span>
                <%} %>
                <% if (workspaces.Count > 7){ %>
                <a href="<%=url + "?workspaceId=" + workspaces[7].ID %>" class="submenu-item item<%="8" %>"><%=Map.Database.Localizer.Translate(workspaces[7].Name)%></a> 
                <%} else { %>
                <span class="submenu-item"><%=undefined %></span>
                <%} %>
                <div class="clearfix"></div>
              </div>
              <div class="rowcontainer"> 
                <% if (workspaces.Count > 8){ %>
                <a href="<%=url + "?workspaceId=" + workspaces[8].ID %>" class="submenu-item item<%="9" %>"><%=Map.Database.Localizer.Translate(workspaces[8].Name)%></a> 
                <%} else { %>
                <span class="submenu-item"><%=undefined %></span>
                <%} %>
                <% if (workspaces.Count > 9){ %>
                <a href="<%=url + "?workspaceId=" + workspaces[9].ID %>" class="submenu-item item<%="10" %>"><%=Map.Database.Localizer.Translate(workspaces[9].Name)%></a> 
                <%} else { %>
                <span class="submenu-item"><%=undefined %></span>
                <%} %>
                <div class="clearfix"></div>
              </div>
            </div>
          </div>
        </div>
