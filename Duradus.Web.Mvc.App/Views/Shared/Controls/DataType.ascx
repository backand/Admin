<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl" %>
<%

    int l = 15;
    string dbName = Map.DatabaseName ?? "";
    string shortDbName = dbName.Length > l ? dbName.Substring(0, l) + "..." : dbName;
    
%>
<label class="relation-settings"><%= Map.Database.Localizer.Translate("Relation Settings") %></label>
<div class="your-or-backand-database">
<label class="select-database"><%= Map.Database.Localizer.Translate("Related Table Location:") %></label>
<div class="your-database">
<input id="InYourDatabase" type="radio" class="InYourDatabase" name="Database" value="1" />
<label for="InYourDatabase" title="<%=dbName %>"><%= shortDbName%></label>
</div>

<div class="system-database">
<input id="InSystemDatabase" type="radio" class="InSystemDatabase" name="Database" value="2" />
<label for="InSystemDatabase"><%= Map.Database.Localizer.Translate("Back&")%></label>
</div>
</div>

<div class="new-or-existing-related-table">
<label class="new-or-existing"><%= Map.Database.Localizer.Translate("New or Existing Table:") %></label>
<div class="related-table existing-related-table">
<input id="ExistingRelatedTable" type="radio" class="ExistingRelatedTable" name="RelatedTable" value="1" />
<label for="ExistingRelatedTable"><%= Map.Database.Localizer.Translate("Select from Existing")%></label>



</div>

<div class="related-table new-related-table">
<input id="NewRelatedTable" type="radio" class="NewRelatedTable" name="RelatedTable" value="2" />
<label for="NewRelatedTable"><%= Map.Database.Localizer.Translate("Create New Table")%></label>

</div>

<div class="ExistingRelatedTableName">
<select class="existing-tables">
<option value=""></option>
<%foreach (Durados.View view in Map.Database.Views.Values.Where(v => !v.SystemView).OrderBy(v=>v.DisplayName))
  { %>
  <option value="<%=view.Name %>"><%=view.DisplayName %></option>
<%} %>
</select>
<select class="existing-system-tables">
<option value=""></option>
<%foreach (Durados.View view in Map.Database.Views.Values.Where(v => v.SystemView && !v.Name.ToLower().StartsWith("durados") && !v.Name.ToLower().StartsWith("block") || (v.Name.ToLower().Contains("user"))).OrderBy(v => v.DisplayName))
  { %>
  <option value="<%=view.Name %>"><%=view.DisplayName %></option>
<%} %>
</select>
</div>
<div class="NewRelatedTableName">
<input type="text" class="new-related-table-name" />
</div>

</div>


