<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Durados.Cms.Model.Email>" %>
<table>
    <tr>
        <td>
            <label>
                To:</label>
        </td>
        <td>
            <%=Html.TextBox("to", Model.To, new { style = "width:630px;"})%>
            
        </td>
    </tr>
    <tr>
        <td>
            <label>
                CC:</label>
        </td>
        <td>
            <%=Html.TextBox("cc", Model.Cc, new { style = "width:630px;" })%>
        </td>
    </tr>
    <tr>
        <td>
            <label>
                BCC:</label>
        </td>
        <td>
            <%=Html.TextBox("bcc", Model.BCc, new { style = "width:630px;" })%>
        </td>
    </tr>
    <tr>
        <td>
            <label>
                Subject:</label>
        </td>
        <td>
            <%=Html.TextBox("subject", Model.Subject, new { style = "width:630px;" })%>
        </td>
    </tr>
    <tr>
        <td colspan="2" width="100%">
            <%=Html.TextArea("body", Model.Body, new { style = "width:700px;height:400px;" })%>
        </td>
    </tr>
</table>
 