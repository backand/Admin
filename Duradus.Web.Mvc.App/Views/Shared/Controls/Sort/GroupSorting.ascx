<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.View>" %>
<!------------------General variables-------------------->
<%  int iFields = 0;
    Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];
    string guid = ViewData["guid"].ToString();
    bool displaySeperator = false;%>
<div class="group-sorting">
    <div class="header">
        <%=Map.Database.Localizer.Translate("Sort By:")%>
    </div>
    <div>
    <!------------------For each visible field of sorting -------------------->
    <%    foreach (Durados.Field field in Model.VisibleFieldsForSorting)
          {
              iFields++;
              string headerTitle = tableViewer.GetDisplayName(field, null, guid);
              string description = string.IsNullOrEmpty(field.Description) ? headerTitle : field.Description;
              string sortId = "sort_" + guid + iFields.ToString();
        
    %>
        <!------------------Container div for sort field -------------------->
        <div sortcolumn='<%=field.Name%>'>
            <!------------------Seperator -------------------->
            <%if (displaySeperator)
              { %>
            <span>|</span>
            <%} %>
            <!------------------Link to change field sort state -------------------->
            <% bool sorted = (ViewData["SortColumn"] != null && ViewData["SortColumn"].ToString() == field.Name); %>
            <% string directionClass = sorted ? "icon_sort_" + ViewData["direction"].ToString() : string.Empty; %>
            <% string sortClass = sorted ? "sort_command" : string.Empty; %>
            <a id='<%=sortId %>' href="#" class="Sortable <%=sortClass %>" sortcolumn='<%=field.Name%>'
                onclick="FilterForm.Sort('<%=sortId%>', '<%=guid %>')" title='<%=description %>'>
                <%=headerTitle%></a>
            <!------------------Image of sort state- in case the field has sort -------------------->
            <%--<%= (ViewData["SortColumn"] != null && ViewData["SortColumn"].ToString() == field.Name) ? "&nbsp;<img alt='' title='" + ViewData["direction"].ToString() + "ending' src='" + Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/icon_sort_" + ViewData["direction"].ToString() + ".GIF' class='sortIcon' />" : ""%>--%>
            <%= sorted ? "&nbsp;<span class='" + directionClass + " sortIcon'></span>" : ""%>
        </div>
        <%
               displaySeperator = true;

          }
        %>
    </div>
</div>
