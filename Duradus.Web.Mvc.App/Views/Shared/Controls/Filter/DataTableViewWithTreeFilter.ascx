<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataView>" %>
<!------------------General variables -------------------->
<%
    Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()];
    ViewData["guid"] = Model.Table.ExtendedProperties["guid"].ToString();
%>
<!------------------Render DataTableView and TreeFilter for ajax call -------------------->
<div>
    <div id="DataTableView">
        <%  Html.RenderPartial("~/Views/Shared/Controls/DataTableView.ascx", Model); %></div>
    <div id="TreeFilter">
        <%  Html.RenderPartial("~/Views/Shared/Controls/Filter/TreeFilter.ascx", view); %>
    </div>
</div>
