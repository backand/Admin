<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<string>" %>

<%if (string.IsNullOrEmpty(Model)){ %>
<input class="x1" type="hidden" value='<%=Map.Database.Localizer.Translate("Are you sure that you want to delete {0}?")%>' />
<input class="x2" type="hidden" value='<%=Map.Database.Localizer.Translate("the selected row")%>' />
<%}else{ %>
<%=Model %>
<%} %>