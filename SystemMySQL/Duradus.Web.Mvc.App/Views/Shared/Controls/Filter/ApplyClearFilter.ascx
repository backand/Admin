<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.View>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<!------------------General variables -------------------->
<%
    string guid = ViewData["guid"] as string;
    string viewFilterVisibility = Model.Name + "_filterVisibility";
    bool collapseFilter = Model.CollapseFilter || (Map.Session[viewFilterVisibility] != null && !(bool)Map.Session[viewFilterVisibility]);
    if (Durados.Web.Infrastructure.General.IsMobile())
        collapseFilter = true;
   
    string mode = collapseFilter ? "show" : "hide";
    string filterClass = ViewData["filterClass"] as string;
    Durados.Web.Mvc.View view = Model;
    
%>
<a id="<%=guid %>filterButton" href="#" onclick="FilterForm.Toggle(this, '<%=guid %>','hide'); return false;"
    title="<%=Map.Database.Localizer.Translate("Hide")%>" class="filterButton <%=filterClass%> <%= collapseFilter? "" : " filterClicked" %>" mode=<%=mode %>>
    <span class="icon filter"></span><%=Map.Database.Localizer.Translate("Filter")%><%--<span class="icon trigger"></span>--%>
    
    <div class="filter-border-triangle">
</div>
    </a>

    <a href="#" class="filter-a apply-filter-a <%= collapseFilter? "" : " filterClicked" %>" onclick="FilterForm.Apply(false, '<%=guid %>')" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.APPLY_FILTER)%>">
                <span class="icon apply-filter"></span>
                &nbsp;</a>
    
    <a href="#" class="filter-a clear-filter-a <%= collapseFilter? "" : " filterClicked" %>" onclick="FilterForm.Apply(true, '<%=guid %>')" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.CLEAR)%>">
                <span class="icon clear-filter"></span>
                &nbsp;</a>
    


    