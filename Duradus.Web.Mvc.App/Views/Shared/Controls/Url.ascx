<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>

<div style="display:none" id="UrlDialog" >
    <table>
        <tr>
            <td>
                <span><%=Map.Database.Localizer.Translate("Display Name") %>:</span>
            </td>
            <td>
                <input type="text" name="urlDisplayName" />
            </td>
        </tr>
        <tr>
            <td>
                <span><%=Map.Database.Localizer.Translate("Internet Address") %>:</span>
            </td>
            <td>
                <input type="text" name="urlAddress" value="http://" />
            </td>
        </tr>
        <tr>
            <td>
                <span><%=Map.Database.Localizer.Translate("Open in new window") %>:</span>
            </td>
            <td>
                <input type="checkbox" safari="1" name="urlNewWindow" checked="checked" />
            </td>
        </tr>
    </table>
</div>