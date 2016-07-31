<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.DataActionFields>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>

<% int col = 0; %>

<form id='<%=Model.DataAction.ToString() %>DataRowForm' enctype="multipart/form-data" action="">
<%
    string prefix = string.Empty;
    if (Model.DataAction == Durados.DataAction.Create)
        prefix = "c_";
    else
        prefix = "e_";
 %>

    <div id="<%=prefix %>tabs-1">
    <table cellpadding="0" cellspacing="5" >
        <%  int i = 0;
            foreach (Durados.Field field in Model.Fields)
          {
              i++;
              if (i > 3)
                  continue;
                %>
            <% if (field.IsVisibleForRow(Model.DataAction)){ %>
            <%if (col % field.View.ColumnsInDialog == 0)
              {%>
                <% if (col > 0)
                   { %>
                    </tr>
                <%} %>
                <tr>
           <% }%>
            <td valign="top" style="white-space:nowrap; vertical-align:middle; padding-left:20px;">
                <%=Durados.Web.Localization.Localizer.Translate(field.DisplayName)%>:
            </td>
            <td id='the<%=field.GetDataActionPrefix(Model.DataAction) + field.Name %>' colspan='<%=(field.ColSpanInDialog * 2 - 1).ToString() %>' style="vertical-align:middle;">
                <%=field.GetElementForRowView(Model.DataAction) %>
                <%=field.GetValidationElements() %>
            </td>
            <%  col += field.ColSpanInDialog;%>
        <%} %>
        <%} %>
            </tr> 
        </table>
    </div>
    <div id="<%=prefix %>tabs-2">
    <table cellpadding="0" cellspacing="5" >
        <%  i = 0;
            foreach (Durados.Field field in Model.Fields)
          {
              i++;
              if (i < 3)
                  continue;
                %>
            <% if (field.IsVisibleForRow(Model.DataAction)){ %>
            <%if (col % field.View.ColumnsInDialog == 0)
              {%>
                <% if (col > 0)
                   { %>
                    </tr>
                <%} %>
                <tr>
           <% }%>
            <td valign="top" style="white-space:nowrap; vertical-align:middle; padding-left:20px;">
                <%=Durados.Web.Localization.Localizer.Translate(field.DisplayName)%>:
            </td>
            <td id='Td1' colspan='<%=(field.ColSpanInDialog * 2 - 1).ToString() %>' style="vertical-align:middle;">
                <%=field.GetElementForRowView(Model.DataAction) %>
                <%=field.GetValidationElements() %>
            </td>
            <%  col += field.ColSpanInDialog;%>
        <%} %>
        <%} %>
            </tr> 
        </table>
    </div>
    
</form>
