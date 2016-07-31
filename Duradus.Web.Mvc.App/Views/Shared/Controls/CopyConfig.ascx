<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>

<label><%=Map.Database.Localizer.Translate("Source")%>:</label>
<select name='sourceView' id='sourceView'>
<option value='' ></option>
<% foreach (Durados.View sourceView in Map.Database.Views.Values.OrderBy(v=>v.DisplayName))
   { %>
<option value='<%=sourceView.Name %>' ><%=sourceView.DisplayName%></option>
<%} %>
</select>&nbsp;

<label><%=Map.Database.Localizer.Translate("Destination")%>:</label>
<select name='destView' id='destView'>
<option value='' ></option>
<% foreach (Durados.View destView in Map.Database.Views.Values.OrderBy(v => v.DisplayName))
   { %>
<option value='<%=destView.Name %>' ><%=destView.DisplayName%></option>
<%} %>
</select>&nbsp;