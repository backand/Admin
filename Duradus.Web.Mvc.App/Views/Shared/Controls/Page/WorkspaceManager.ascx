<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Workspace>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<% string viewName = (ViewData["viewName"] ?? string.Empty).ToString(); %>
<%
    Workspace workspace = Model ?? MenuHelper.GetCurrentWorkspace(viewName);
    string selected = string.Empty;
%>
<% if (workspace != null)
   { %>
   <div class="slider-container workspace-slider-container">
        <div class="anythingSlider anythingSlider-default activeSlider" style="width: 200px;
            height: 68px;">
            <div class="anythingWindow">
                <ul class="ui-slider anythingBase horizontal" style="left: -200px; width: 1100px;"
                    name="Workspace" viewname="Workspace" pk="">

                    <% 
                        string activeClass = string.Empty;
                        int i = 1;
                    %>
                    <%foreach (Workspace workspace2 in Map.Database.Workspaces.Values.Where(w=>w.ID != Map.Database.GetAdminWorkspaceId()).OrderBy(w=>w.Ordinal))
                      { %>
                      <% 
                          string workspaceName = workspace2.Name;
                          string workspaceId = "item" + i;
                          string pk = workspace2.ID.ToString();
                          i++;
                          if (workspace.Name == workspace2.Name)
                          {
                              activeClass = "workspace-slider-active";
                              selected = pk;
                          }
                          else
                          {
                              activeClass = string.Empty;
                          }
                    %>
                    <li>
                        <div class="ui-slider-item ui-slider-item-workspace <%= activeClass%>" value="<%=pk %>" index='<%=(i - 1) %>' onclick="slider.UpdateSelectedImage(this);">
                            <div title="<%=workspaceName%>" class="workspace-selector <%=workspaceId %>"><span class="w-text"><%=workspaceName%></span>
                            </div>
                        </div>
                    </li>
                    <%} %>
                </ul>
            </div>
            <%--<span class="arrow back"><a href="#"><span>«</span></a></span><span class="arrow forward"><a
                href="#"><span>»</span></a></span>--%>
            </div>
        <input type="hidden" id="xxPageXx_inlineEditing_Workspace" name="Workspace" value="<%=selected %>" viewname="Workspace"
            pk="">
    </div>

<%} %>