<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.Item>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json" %>
<% string guid = Model.Guid; %>
<% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.ViewName]; %>
<% bool mainPage = true; %>
<%  
    Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];
%>

<% if (tableViewer.IsEditable(view)){ %>
<div style="display: none" id="<%=guid %>DataRowEdit" hasguid='hasguid' guid='<%=guid %>'>
    <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowView.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Edit), DataAction = Durados.DataAction.Edit, Guid = guid, View = view }); %>
</div>
<%} %>
<%--<div style="display: block" id="<%=guid %>DataRowFilter" hasguid='hasguid' guid='<%=guid %>'>
    <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowView.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Create), DataAction = Durados.DataAction.Create, Guid = guid }); %>
</div>--%>
<table>
<tr>
<td>
<%  Html.RenderPartial("~/Views/Shared/Controls/DataRowView.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Report).Where(f=>f.FieldType != Durados.FieldType.Children).ToList(), DataAction = Durados.DataAction.Report, Guid = guid, View = view }); %>
</td>
<td>
<input type="button" value="Run" class="runReport gbutton" d_viewName="<%=view.Name %>" d_guid="<%=guid %>" onclick="runReport('<%=guid %>', '<%=view.Name %>')"/>
</td>
</tr>
</table>


<div id="<%=guid %>ajaxDiv" ajaxdiv="ajaxDiv" guid="<%=guid %>" d_MainPage="<%=mainPage %>">
    <%--<%  Html.RenderPartial("~/Views/Shared/Controls/DataTableView.ascx", Model); %>--%>
</div>


<%  Html.RenderPartial("~/Views/Shared/Controls/ClientParams.ascx", new Durados.Web.Mvc.UI.ClientParams() { View = view, Guid = guid, MainPage = mainPage }); %>

