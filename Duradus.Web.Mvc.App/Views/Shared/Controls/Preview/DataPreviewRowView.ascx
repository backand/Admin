<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataRowView>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%
    Durados.Web.Mvc.View view = ViewData["View"] as Durados.Web.Mvc.View;
    string guid = ViewData["Guid"].ToString();
    string pkValue = view.GetPkValue(Model.Row);
    string editClick = string.Empty;
    string url = Url.Action("Item", view.Controller, new { viewName = view.Name, pk = pkValue, guid = view.Name + "_Item_" });
    string displayValue = Html.Encode(view.GetDisplayValue(Model.Row));
    int rowIndex = (int)ViewData["RowIndex"];
    int rowsCount = (int)ViewData["RowsCount"];
    Durados.Web.Mvc.UI.Styler styler = ViewData["Styler"] as Durados.Web.Mvc.UI.Styler;
    Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];
    Dictionary<string, Durados.Field> excludedColumn = ViewData["ExcludedColumn"] as Dictionary<string, Durados.Field>;
    string unselectable = ViewData["Unselectable"].ToString();

    bool isPkField = view.IsPartOfPk(Model.Row);

    string pkAttr = isPkField ? " partofpk='partofpk' " : string.Empty;
    
    editClick += "d_edit('" + pkValue + "', " + view.Popup.ToString().ToLower() + ", " + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + view.Name + "', '" + guid + "', this)";
    rowIndex++;
%>
<!------------------Div container for one dataRow-------------------->
<div style="" class='boardrow2 portlet' id='d_row_<%=guid + pkValue %>' d_row='d_row' <%=pkAttr %>
    pk="<%=pkValue %>" d_displayvalue="<%= displayValue%>" onclick="Multi.BoardClicked(event, $(this).children('.boardtitle'), '<%=guid %>', true);<%=editClick%>;"
    <%= tableViewer.ShowRowHover(view, Model.Row) ? "onMouseOver=\"$(this).addClass('hovered')\" onMouseOut=\"$(this).removeClass('hovered')\"" : "" %>
    <%=unselectable %>>
    <!------------------Div title for one dataRow-------------------->
    <div class="boardtitle portlet-header" guid='<%=guid %>' onclick="Multi.BoardClicked(event, this, '<%=guid %>', true)"
        <%=unselectable %> d_pk='<%=pkValue %>'>
        <%= HttpUtility.HtmlDecode(displayValue)%> 
    </div>
    <!------------------Fields content for one dataRow-------------------->
    <table class="portlet-content">
        <!------------------For each field in dataRow-------------------->
        <%                                            
            foreach (Durados.Field field in view.VisibleFieldsForPreview)
            //foreach (Durados.Field field in view.Fields.Values.Take(5))
            {
                string value = Server.HtmlEncode(tableViewer.GetFieldValue(field, Model.Row));
                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Replace("\0", "");
                }

                string alt = styler.GetAlt(field, Model.Row, guid);

                alt = (string.IsNullOrEmpty(alt) ? string.Empty : " title='" + alt + "'");

                string tableViewElement = tableViewer.GetElementForTableView(field, Model.Row, guid);
                if (!string.IsNullOrEmpty(tableViewElement))
                {
                    tableViewElement = tableViewElement.Replace("\0", "");
                }
                if (tableViewer.IsVisible(field, excludedColumn, guid))
                {
        %><tr>
            <td <%=alt %><%=unselectable %>>
                <b>
                    <%=tableViewer.GetDisplayName(field, null, guid)%></b>
            </td>
            <td>
                <%=tableViewElement%>
            </td>
        </tr>
        <%}
            }

            if (tableViewer.IsEditable(view))
            {
                string[] pkArray = ViewData["pkArray"] == null ? null : (string[])ViewData["pkArray"];
                bool isFirst = rowIndex == 1;
                bool isLast = rowIndex == rowsCount;
                string nextPK = string.Empty;
                if (!isLast)
                {
                    nextPK = view.GetPkValue(Model.Row);
                }
                string prevPK = "";
                if (!isFirst)
                {
                    prevPK = view.GetPkValue(Model.Row);
                }
                string isFirstAttr = (isFirst) ? "isFirst='isFirst'" : "";
                string isLastAttr = (isLast) ? "isLast='isLast'" : "";
        %>
        <tr guid='<%=guid %>' mode='board' style="display: none">
            <%
                if (tableViewer.SelectorCheckbox(view))
                { %>
            <td colspan="2">
                <input type="checkbox" title="<%= Map.Database.Localizer.Translate("Select current record")%>"
                    class="Multi" pk='<%=pkValue %>' prevpk='<%=prevPK %>' nextpk='<%=nextPK %>'
                    <%=isFirstAttr %> <%=isLastAttr %> <%=pkArray != null && pkArray.Contains(pkValue) ? "checked='checked'" : "" %> />
            </td>
            <% } %>
        </tr>
        <% }  %>
    </table>
</div>
