<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.AddPage>" %>



<div class="add-page">

<div class="add-page-content">
<%=Model.Content %>
</div>
<label>Page Name:</label>
<input type="text" class='add-page-name' maxlength='25'/>


<div class="add-page-left-content">

<label>Page Layout:</label><br />
<select class="add-page-type" size="20">
    <%foreach (Durados.LinkType linkType in Model.Types.Keys)
      { %>
      <optgroup label="<%=Model.GetLinkTypeDisplayName(linkType) %>">
        <%foreach (Durados.LinkSubType linkSubType in Model.Types[linkType].Keys)
          {
              bool isDisabled = linkType == Durados.LinkType.Report;
              bool isSelected = linkSubType == Durados.LinkSubType.Grid1;
              if (linkSubType == Durados.LinkSubType.ReportingServices)
                  isDisabled = false;
              
              string disabled = isDisabled ? "disabled='disabled'" : string.Empty;
              string selected = isSelected ? "selected='selected'" : string.Empty; 
             %>
            <option <%=disabled %> <%=selected %> value="<%=linkSubType.ToString() %>"><%=Model.Types[linkType][linkSubType] %></option>
        <%} %>
      </optgroup>
    <%} %>
</select>
</div>

<div class="add-page-right-content">
<div class="page-type-image"></div>

<div class="add-page-details">

<div class="add-page-create add-page-create-view">


<div>
<input id="NewTable" type="radio" class="radio"  name="createView" value="0" />
<label for="NewTable"><%= Map.Database.Localizer.Translate("Create New Table") %></label>

<div>
<input id="Excel" type="radio" class="radio" name="createView" value="1" />
<label for="Excel"><%= Map.Database.Localizer.Translate("Create Table From Excel")%></label>
</div>

<div>
<input id="Select" type="radio" class="radio" name="createView" value="2" />
<label for="Select"><%= Map.Database.Localizer.Translate("Select Table or View")%></label>


<%--<label for='add-page-entity' style="display: none">Select Table:</label><br />--%>
<select class='add-page-entity' name="add-page-entity" style="display: none">
<option value="-1"><%= Map.Database.Localizer.Translate("[Select Table or View]")%></option>
<%--<option disabled="disabled" value="">-----------</option>
<option value="0">Create New Table</option>
<option value="1">Import From Excel</option>
<option disabled="disabled" value="">-----------</option>--%>
<% foreach (System.Data.DataRowView row in Model.EntityTable){ %>
<option value="<%= Model.GetValue(row.Row)%>" <%=Model.IsNewView(row.Row) ? "isNewView='yes'" : "" %> displayName="<%= Model.GetDisplayName(row.Row)%>"><%= Model.GetText(row.Row)%></option>

<%} %>
</select>



<%--<input name="add-page-excel" type="text" class="upload" style="display: none"/>--%>

<label for='add-page-editable-table' style="display: none">Editable Table:</label><br />
<select class='add-page-editable-table' name="add-page-editable-table" style="display: none">
<option value="-1">[Select Table]</option>
<% foreach (string tableName in Model.Tables.OrderBy(t=>t)){ %>
<option value="<%= tableName%>"><%= tableName%></option>
<%} %>
</select>
</div>

</div>

</div>

<div class="add-page-create add-page-create-dashboard">

<div>
<input id="NewDashboard" type="radio" class="radio"  name="createDashboard" value="3" />
<label for="NewDashboard"><%= Map.Database.Localizer.Translate("Create New Dashboard") %></label>


<div>
<input id="ExistingDashboard" type="radio" class="radio" name="createDashboard" value="4" />
<label for="ExistingDashboard"><%= Map.Database.Localizer.Translate("Select Dashboard")%></label>



<select class='add-page-dashboard' name="add-page-dashboard" style="display: none">
<option value="-1"><%= Map.Database.Localizer.Translate("[Select Dashboard]")%></option>

<% foreach (KeyValuePair<int,string> dashboard in Model.Dashboards){ %>
<option value="<%= dashboard.Key%>"  ><%= dashboard.Value%></option>

<%} %>
</select>

</div>

</div>

</div>
</div>
</div>
</div>

