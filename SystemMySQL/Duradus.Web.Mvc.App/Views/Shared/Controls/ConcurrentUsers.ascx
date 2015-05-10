<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Collections.Generic.List<Durados.Concurrency+Reader>>" %>
    <li><select>
    <option value=''><%=Map.Database.Localizer.Translate("Also present...")%></option>
    <% foreach (var item in Model) { %>
        <option disabled='disabled' value='<%= Html.Encode(item.Username)%>'><%=Html.Encode(item.Fullname)%></option>
    <% } %>
    </select></li>
    