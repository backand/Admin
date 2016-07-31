<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Durados.Web.Mvc.View>>" %>

<table>
    <% foreach (Durados.Web.Mvc.View view in Model){ %>
        <tr>
            <td class="tablemenu">
                <span style="white-space:nowrap">
                <%= Html.ActionLink(view.GetLocalizedDisplayName(), view.IndexAction, view.Controller, new { viewName = view.Name }, null)%>
                </span>
            </td>
        </tr>
    <%} %>
</table>
