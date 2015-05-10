<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataView>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%    
    Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()];
    bool isIE = Request.Browser.Browser == "IE";
%>
<%------------------------Background----------------------%>
<%if (!string.IsNullOrEmpty(view.Background))
  {
      string color = view.Background;
%>
<style type="text/css">
    table.gridview thead, table.gridviewhead tbody, .gridviewhead th { background: <% =color %>; }
    .filter-buttons { background-color: <% =color %>; }
    .ui-state-active, .ui-widget-content .ui-state-active, .ui-widget-header .ui-state-active { background-color: <% =color %>; }
    .groupFilter{ background-color: <% =color %>;}
    .tree_filter_selectedValues, .group-sorting {background-color: <% =color %>;}
    .gridPager-div {background-image: none;}
    .gridPager-div {background-color: <% =color %>;}
    td.ddlpageSize select, td.curPageInput input {background-color: <% =color %>;}
    tr.rowfilter td { background-color: <% =color %>; }
    
    .filter-buttons .button {background-image: none;}
     .filter-buttons .button {background-image:linear-gradient(<% =color %> 0%, <%=Durados.Web.Mvc.Infrastructure.ColorUtility.GetGradient(color, 0.75f, false) %> 100%);}
     .filter-buttons .button:hover {background-image:linear-gradient( <%=Durados.Web.Mvc.Infrastructure.ColorUtility.GetGradient(color, 0.75f, false) %> 0%, <% =color %> 100%);}
     
     .table-menu .group-l a.filterButton,  .table-menu .group-r a.filterButton {background-image: none;}
    .table-menu .group-l a.filterButton, .table-menu .group-r a.filterButton {background-image:linear-gradient(<%=Durados.Web.Mvc.Infrastructure.ColorUtility.GetGradient(color, 0.75f, false) %> 0%, <% =color %> 100% );}
    
</style>
<%} %>
<%------------------------RowBackground------------------------%>
<%if (!string.IsNullOrEmpty(view.RowBackground))
  {
      string color = view.RowBackground;
      bool defaultSkin = view.Skin == Durados.Web.Mvc.SkinType.skin5 || view.Skin == Durados.Web.Mvc.SkinType.DefaultSkin;
      string bg = defaultSkin ? "rgb(255, 182, 38)" : "transparent";
%>
<style>
    .d_fix_row td { background-color: <% =color %>; }
    
    
    tr.data-row {background-color: rgba(255, 255, 255, 0) !important;border-bottom: 1px rgba(255, 255, 255, 0) !important;}
    
    .selected td, .selected.hovered td {background-color: <% =bg %>;}
</style>
<%} %>
<%------------------------AlternateRowBackground------------------------%>
<%if (!string.IsNullOrEmpty(view.AlternateRowBackground))
  {
      string color = view.AlternateRowBackground;
      bool defaultSkin = view.Skin == Durados.Web.Mvc.SkinType.skin5 || view.Skin == Durados.Web.Mvc.SkinType.DefaultSkin;
      string bg = defaultSkin ? "rgb(255, 182, 38)" : "transparent";
%>
<style>
    .d_alt_row td { background-color: <% =color %>; }
    tr.data-row.d_alt_row {background-color: rgba(255, 255, 255, 0) !important;border-bottom: 1px rgba(255, 255, 255, 0) !important;}
    .selected td, .selected.hovered td {background-color: <% =bg %>;}
</style>
<%} %>

<%------------------------ToolBoxBackground------------------------%>
<%if (!string.IsNullOrEmpty(view.ToolBoxBackground))
  {
      string color = view.ToolBoxBackground;
%>
<style>
   .table-menu .group-l a, .table-menu .group-r a {background-image:none;}
    .table-menu { background-image: none; }
    .table-menu { background-color: <% =color %>; }
     .table-menu { background-image: linear-gradient(<% =color %> 0%, <%=Durados.Web.Mvc.Infrastructure.ColorUtility.GetGradient(color, 0.75f, false) %> 100%); }
    
     
</style>
<%} %>
<%------------------------FontColor------------------------%>
<%if (!string.IsNullOrEmpty(view.FontColor))
  {
      string color = view.FontColor;
%>
<style>
    th A.Sortable, th SPAN.sortable { color: <% =color %> !Important; }
    .table-menu a.filterButton.filterClicked { color: <% =color %>; }
    .filter-buttons .button { color: <% =color %>; }
    .gridPager td { color: <% =color %>; }
    button.ui-button { color: <% =color %> !important; }
    .gridPager-div .totalRows { color: <% =color %>; }
    td.curPageInput input { color: <% =color %>; }
    td.ddlpageSize select { color: <% =color %>; }
</style>
<%} %>
<%------------------------TextFontColor------------------------%>
<%if (!string.IsNullOrEmpty(view.TextFontColor))
  {
      string color = view.TextFontColor;
%>
<style>
    .d_alt_row td div { color: <% =color %> !Important; }
    .d_alt_row .disabledCell { color: <% =color %> !Important; }
     .watermarkThis { color: <% =color %> !important; }
    
</style>
<%} %>
<%------------------------Alternate Row text color------------------------%>

<%if (!string.IsNullOrEmpty(view.AlterTextColor))
  {
      string color = view.AlterTextColor;
%>
<style>
    .d_fix_row td div { color: <% =color %> !Important; }
    
    
</style>
<%} %>
<%------------------------ToolBoxColor------------------------%>
<%if (!string.IsNullOrEmpty(view.ToolBoxTextColor))
  {
      string color = view.ToolBoxTextColor;
%>
<style>
.table-menu .group-l a, .table-menu .group-r a { color: <% =color %>; }
</style>

<%} %>


<%------------------------BorderColor------------------------%>
<%if (!string.IsNullOrEmpty(view.BorderColor))
  {
      string color = view.BorderColor;
%>
<style>
    table.gridview td, table.gridviewhead td, .gridview th, .gridviewhead th { border-left-color: <% =color %> !Important; }
    .data-row td { border-bottom: 1px <% =color %> solid !important;}
</style>
<%} %>
<%--table.gridview td, table.gridviewhead td, .gridview th, .gridviewhead th--%>


<%------------------------HoverTextColor------------------------%>
<%if (!string.IsNullOrEmpty(view.HoverTextColor))
  {
      string color = view.HoverTextColor;
%>
<style>
tr.hovered td div {color: <% =color %> !important;}
</style>

<%} %>

<%------------------------HoverBeckground------------------------%>
<%if (!string.IsNullOrEmpty(view.HoverBackground))
  {
      string color = view.HoverBackground;
%>
<style>
tr.hovered td {background: <% =color %>;}
</style>

<%} %>





