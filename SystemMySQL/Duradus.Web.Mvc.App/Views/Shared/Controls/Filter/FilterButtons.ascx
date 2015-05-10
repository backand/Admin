<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataView>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json" %>
<%
    string guid = Model.Table.ExtendedProperties["guid"].ToString();
    bool collapseFilter = (ViewData["collapseFilter"] == null) ? false : (bool)ViewData["collapseFilter"];
    if (Durados.Web.Infrastructure.General.IsMobile())
        collapseFilter = true;
   
    bool enableCollapseFilter = (ViewData["enableCollapseFilter"] == null) ? true : (bool)ViewData["enableCollapseFilter"];
    Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()];
%>
<div id="<%=guid %>filterButtons" class="filter-buttons" <%= collapseFilter? " style='display:none'" : "" %>>
    <a onclick="FilterForm.Apply(false, '<%=guid %>')" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.APPLY_FILTER)%>"
        class="button">
        <%=Map.Database.Localizer.Translate("Apply")%></a>
    <%--
//In future unmark this- for tree filer (br)
        <a onclick="FilterForm.Apply(true, '<%=guid %>'<%if(view.FilterType==Durados.FilterType.Tree){ %>, null, true<%}%>)"
            title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.CLEAR)%>"
            class="button">--%>
    <a onclick="FilterForm.Apply(true, '<%=guid %>')" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.CLEAR)%>"
        class="button">
        <%=Map.Database.Localizer.Translate("Clear")%></a>
    <% 
        if (enableCollapseFilter)
        {
    %>
    <a onclick="$('#<%=guid %>filterButton').click(); return false;" class="button"><%=Map.Database.Localizer.Translate("Close")%></a>
    <% 
            }
    %>
    <div class="clearfix">
    </div>
</div>
