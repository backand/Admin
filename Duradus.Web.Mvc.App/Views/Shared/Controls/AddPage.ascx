<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Durados.AddPage>" %>



<div class="add-page">

<div class="add-page-content">
<%=Model.Content %>
</div>
<label>Page Layout:</label>
<select class="add-page-type" size="20">
    <%foreach (Durados.LinkType linkType in Model.Types.Keys)
      { %>
      <optgroup label="<%=Model.GetLinkTypeDisplayName(linkType) %>">
        <%foreach (Durados.LinkSubType linkSubType in Model.Types[linkType].Keys)
          { %>
            <option value="<%=linkSubType.ToString() %>"><%=Model.Types[linkType][linkSubType] %></option>
        <%} %>
      </optgroup>
    <%} %>
</select>

<img src=""/>

<div class="add-page-details">
<label>Name your page:</label>
<input type="text" class='add-page-name'/>

<label>Select Table:</label>
<select class='add-page-entity'>
<option value="0">New Table</option>
<option value="1">From Excel</option>
<option disabled="disabled" value="">-----------</option>
<% foreach (System.Data.DataRowView row in Model.EntityTable){ %>
<option value="<%= Model.GetValue(row.Row)%>"><%= Model.GetText(row.Row)%></option>

<%} %>
</select>


<label for="add-page-new-name">Table Name:</label>
<input name="add-page-new-name" type="text"/>

<input name="add-page-excel" type="text" class="upload" style="display: none"/>

</div>
</div>

