<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.SpecialMenu>" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<% if (Model != null && Model.Menus != null)
   { %>
    <% foreach (Durados.SpecialMenu menu in Model.Menus.Values.OrderBy(m => m.Ordinal).ToList())
       {%>
        <% menu.Parent = Model; %>
        <% if (!menu.IsHidden())
        {
            string menuName = menu.Name;
            string menuUrl = string.IsNullOrEmpty(menu.Url) ? "#" : menu.Url;

            if (Map.Database.IsMultiLanguages)
            {
                menuName = Map.Database.Localizer.Translate(menu.Name);
            }%>
            <li class="dd-item dd3-item" data-id="<%=menu.ID %>">
            <div class="dd-handle dd3-handle"></div>
            <div class="dd3-content" url="<%=menuUrl%>"><span><%=menuName%></span><span class="dd3-settings"></span></div>
            <% if (menu.Menus.Count > 0)
                { %>
                <ol class="dd-list">
                    <%  Html.RenderPartial("~/Views/Shared/Controls/Pages.ascx", menu); %>
                </ol>
            <%} %>
             </li>
        <%} %>
    <%} %>
<%} %>