<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataView>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json" %>
<% string guid = Model.Table.ExtendedProperties["guid"].ToString(); %>
<%--<% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.TableName]; %>
--%><% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()]; %>
<% bool mainPage = (bool)Model.Table.ExtendedProperties["mainPage"]; %>
<% string viewSafety = view.Name + "_safety";%>
<% bool safetyMode = (bool)Map.Session[viewSafety]; %>


<div id="<%=guid %>ajaxDiv" class="ajaxDiv" ajaxdiv="ajaxDiv" guid="<%=guid %>" d_MainPage="<%=mainPage %>" d_domain="<%=Durados.Web.Mvc.Maps.Domain %>"  onmousedown="Durados.FieldEditor.MouseDown(event, this)">
    <%  Html.RenderPartial("~/Views/Shared/Controls/DataTableView.ascx", Model); %>
</div>
<%
    if (ViewData["cameFromIndex"] == null || view.Popup)
    {%>
        <% if (view.IsEditable(guid) && view.IsCreatable()){ %>
        <div style="display: none" id="<%=guid %>DataRowCreate" hasguid='hasguid' guid='<%=guid %>'>
            <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowView.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Create), DataAction = Durados.DataAction.Create, Guid = guid, View = view }); %>
        </div>
        <%} %>
        <div style="display: none" id="<%=guid %>DataRowEdit" hasguid='hasguid' guid='<%=guid %>'>
            <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowView.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Edit), DataAction = Durados.DataAction.Edit, Guid = guid, View = view }); %>
        </div>
    <%} else if (view.MultiSelect) {
    %>
       <div style="display: none" id="<%=guid %>DataRowEdit" hasguid='hasguid' guid='<%=guid %>'>
            <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowView.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Edit), DataAction = Durados.DataAction.Edit, Guid = guid, View = view }); %>
        </div>
     <%} %>


<%  Html.RenderPartial("~/Views/Shared/Controls/ClientParams.ascx", new Durados.Web.Mvc.UI.ClientParams() { View = view, Guid = guid, MainPage = mainPage }); %>

<% if (view is Durados.Config.IConfigView && Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") && view.Name == "View")
    { %>    
    <div id='copyConfigDialog' style="display:none">
    <%  Html.RenderPartial("~/Views/Shared/Controls/CopyConfig.ascx"); %>
    </div>
<%} %>

<% if (view is Durados.Config.IConfigView && Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") && view.Name == "View")
    { %>    
    <div id='cloneViewDialog' style="display:none">
    <%  Html.RenderPartial("~/Views/Shared/Controls/CloneView.ascx"); %>
    </div>
<%} %>

<!-- Context Menu -->
<%  Html.RenderPartial("~/Views/Shared/Controls/ContextMenu.ascx", new Durados.Web.Mvc.UI.ClientParams() { View = view, Guid = guid, MainPage = mainPage }); %>
