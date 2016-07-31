<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>

<label><%=Map.Database.Localizer.Translate("Base") %>:</label>
<select name='baseViewName' id='baseViewName'>
<option value='' ></option>
<% foreach (Durados.View sourceView in Map.Database.Views.Values.OrderBy(v=>v.Name))
   { %>
<option value='<%=sourceView.Name %>' ><%=sourceView.Name%></option>
<%} %>
</select>&nbsp;

<label><%=Map.Database.Localizer.Translate("Cloned")%>:</label>
<input type="text" id="clonedViewName"/>
