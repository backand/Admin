<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.View>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json" %>
<!------------------General variables -------------------->
<%  string guid = ViewData["guid"] as string;
%>
<!------------------Tree filter container -------------------->
<div class="tree_filter" id="tree_filter_<%=guid %>">
    <!------------------Header -------------------->
    <div class="refer-bar">
        <%=Map.Database.Localizer.Translate("Tree Filter")%>
    </div>
    <!------------------For each field -------------------->
    <% foreach (Durados.Field field in Model.VisibleFieldsForTreeFilter)
       {
           object filterValue = (ViewData["filter"] == null) ? null : (((Dictionary<string, Field>)ViewData["filter"]).ContainsKey(field.Name)) ? ((Dictionary<string, Field>)ViewData["filter"])[field.Name].Value : null;
           if (filterValue == null || string.IsNullOrEmpty(filterValue.ToString()))
           {
    %>
    <!------------------Container for filter values -------------------->
    <div class="filterValuesContainer">
        <%
               Html.RenderPartial("~/Views/Shared/Controls/Filter/TreeFilterValues.ascx", field); 
        %>
    </div>
    <%
           }
       }
    %>
</div>
