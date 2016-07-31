<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.View>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json" %>
<%@ Import Namespace="Durados.Web.Mvc" %>
<%@ Import Namespace="Durados.DataAccess" %>
<!------------------General variables-------------------->
<%  string guid = ViewData["guid"] as string;
    string viewFilterVisibility = Model.Name + "_filterVisibility";
    bool collapseFilter = Model.CollapseFilter || (Map.Session[viewFilterVisibility] != null && !(bool)Map.Session[viewFilterVisibility]);
    if (Durados.Web.Infrastructure.General.IsMobile())
        collapseFilter = true;
   
    bool isFilterExist = false;
%>
<div class="tree_filter_selectedValues" name='rowfilter' <%= collapseFilter? " style='display:none'" : "" %>>
    <div class="header">
        <%=Map.Database.Localizer.Translate("Filter:")%>
    </div>
    <!------------------For each visible field of tree filter -------------------->
    <%    foreach (Durados.Field field in Model.VisibleFieldsForTreeFilter)
          {
              string id = guid + "filter_" + field.Name.ReplaceNonAlphaNumeric();
              string filterValue = string.Empty;
              string filterDisplayValue = string.Empty;
              bool displaySeperator = isFilterExist;

              if (ViewData["filter"] != null && ((Dictionary<string, Field>)ViewData["filter"]).ContainsKey(field.Name))
              {
                  Field filterField = ((Dictionary<string, Field>)ViewData["filter"])[field.Name];
                  ParentField parentField = field.GetParentField();

                  filterValue = Convert.ToString(filterField.Value);

                  if (parentField != null)
                  {
                      filterDisplayValue = parentField.GetDisplayValue(filterValue);
                  }

                  if (!string.IsNullOrEmpty(filterValue))
                  {
                      isFilterExist = true;
                  }
              }
    %>
    <!------------------Container div for filter field -------------------->
    <div id="<%=id %>_container" <% if (string.IsNullOrEmpty(filterValue)) { %> style="display: none;"
        <%}%>>
        <!------------------If this filter field is not first- need display a seperator -------------------->
        <%if (displaySeperator)
          { %>
        <span>|</span>
        <%} %>
        <!------------------Field name-------------------->
        <span style="color: #718abe;">
            <%= field.DisplayName%>:</span>
        <!------------------Filter value-------------------->
        <span id="<%=id %>" data-val="<%=filterValue %>">
            <%= filterDisplayValue%></span>
        <!------------------Close image-------------------->
        <img class="close" alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/close.gif" %>"
            align="absmiddle" title="Click to remove this filter" data-guid="<%=guid %>" />
    </div>
    <%
          }
          if (!isFilterExist)
          {
    %>
    <!------------------Display "All displayed" if there is no a filter value-------------------->
    <div class="all-displayed">
        All displayed</div>
    <%   
          }
    %>
</div>
