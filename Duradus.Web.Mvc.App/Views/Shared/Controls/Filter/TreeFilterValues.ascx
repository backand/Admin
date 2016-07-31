<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Field>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<!------------------General variables -------------------->
<%       
    bool loadAll = (ViewData["loadAll"] as bool?).HasValue ? (ViewData["loadAll"] as bool?).Value : false;
    int top = 5;
    int? topForLoading = loadAll ? (int?)top + 1 : null;
    Dictionary<string, string> filterValues = Model.GetSelectOptions(string.Empty, topForLoading, true);
    
    if (filterValues != null && filterValues.Count > 0)
    {
        string guid = ViewData["guid"] as string;
        string id = guid + "filter_" + Model.Name.ReplaceNonAlphaNumeric();
        IEnumerable<KeyValuePair<string, string>> lessFilterValues = filterValues.Take(top);       
%>
<!------------------Container div for filter field -------------------->
<div class="tree-filter-values" data-rel="<%=id %>" data-guid="<%=guid %>" data-loadAll="<%=loadAll %>">
<!------------------Field name-------------------->
    <div class="field-name">
        <%=Model.DisplayName%></div>
<!------------------List of less values -------------------->
    <ul>
        <% foreach (KeyValuePair<string, string> _filterValue in lessFilterValues)
           {
        %>
        <li data-val="<%=_filterValue.Key %>">
            <%=_filterValue.Value%></li>
        <%         
           }
        
        %>
    </ul>
<!------------------If exist more values -------------------->
    <%
        if (filterValues.Count > lessFilterValues.Count())
        {
            if (loadAll)
            {%>
<!------------------If loadAll=true: Display list of more values -------------------->
    <ul id="<%=id %>_more_items">
        <% foreach (KeyValuePair<string, string> _filterValue in filterValues.Except(lessFilterValues))
           {
        %>
        <li data-val="<%=_filterValue.Key %>">
            <%=_filterValue.Value%></li>
        <%         
           }
        
        %>
    </ul>
    
    <% }
            %>
<!------------------If exist more values and loadAll=true: Display "Less..." link -------------------->
            <div id="<%=id %>_less" class="more_less_filter_values" onclick="FilterForm.LessFilterValues('<%=id %>')" <%if (!loadAll) {%> style="display: none;"<%} %>>
        Less...</div>
<!------------------If exist more values and loadAll=false: Display "More..." link -------------------->
    <div id="<%=id %>_more" class="more_less_filter_values" onclick="FilterForm.MoreFilterValues('<%=Model.View.Name %>', '<%=Model.Name %>', '<%=guid %>', '<%=id %>')" <%if (loadAll) {%> style="display: none;"<%} %>>
        More...</div>
    <%
        }
    %>
    <hr />
</div>
<%
    }
%>