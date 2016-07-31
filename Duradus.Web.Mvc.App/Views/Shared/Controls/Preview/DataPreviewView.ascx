<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataView>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json" %>
<!------------------General variables -------------------->
<% try
   {
       string guid = Model.Table.ExtendedProperties["guid"].ToString();
       Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()];
       string viewSafety = view.Name + "_safety";
       bool safetyMode = Map.Session[viewSafety] != null ? (bool)Map.Session[viewSafety] : false;
       string pk = (ViewData["pk"] == null) ? string.Empty : ViewData["pk"].ToString();
       int rowCount = Convert.ToInt32(ViewData["rowCount"]); string rootPath = Durados.Web.Mvc.Infrastructure.General.GetRootPath();
       Durados.Web.Mvc.UI.Styler styler = ViewData["Styler"] == null ? new Durados.Web.Mvc.UI.Styler(view, Model) : (Durados.Web.Mvc.UI.Styler)ViewData["Styler"];
       Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];
       Durados.Web.Mvc.UI.ColumnsExcluder columnsExcluder = ViewData["ColumnsExcluder"] == null ? null : (Durados.Web.Mvc.UI.ColumnsExcluder)ViewData["ColumnsExcluder"];
       Dictionary<string, Durados.Field> excludedColumn = columnsExcluder.ExcludedColumns;
       string previewWidth = string.IsNullOrEmpty(view.DashboardWidth) ? "250px" : view.DashboardWidth + "px";
       string marginStyle = "margin-" + (Map.Database.Localizer.Direction == "RTL" ? "right" : "left") + ":" + previewWidth + ";";
       bool hidePager = view.HidePager && !view.IsInAdminMode();
       bool hideFilter = view.HideFilter && !view.IsInAdminMode();
    
       tableViewer.DataView = Model;%>
<%Html.RenderPartial("~/Views/Shared/Controls/Toolbar/Toolbar.ascx", Model);%>
<%     
  //Handle Filter
       if (!hideFilter)
  {
      if (view.FilterType == Durados.FilterType.Group)
      {
          Html.RenderPartial("~/Views/Shared/Controls/Filter/GroupFilter.ascx", view);
      }
      //else if (view.FilterType == Durados.FilterType.Tree)
      //{
      //    Html.RenderPartial("~/Views/Shared/Controls/Filter/TreeFilterSelectedValues.ascx", view);
      //}
  }
  //Handle Sort
  if (view.SortingType == Durados.SortingType.Group)
  {
      if (!(view.Database.IsConfig && (view.Name == "Field") && (ViewContext.RouteData.Values["action"].ToString() == "Item" || (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri.Contains("menu=off")) || (Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains("IndexWithButtons")))))
      {
          ViewData["guid"] = guid;
          Html.RenderPartial("~/Views/Shared/Controls/Sort/GroupSorting.ascx", view);
      }
  }
  
  string unselectable = safetyMode ? " unselectable='on'" : "";      
%>
<%
       bool enableOrder = view.IsOrdered && tableViewer.IsEditable(view) && !view.IsDisabled(guid);
%>
<%=Html.Hidden("pk", pk, new { id = guid + "pk" })%>
<!------------------fixedViewPort-------------------->
<%
       string viewClass = string.Empty;
       if (ViewName == "Field")
       {
           viewClass = ViewName;
       }     
     %>
<div class="fixedViewPort  <%=viewClass %>" d_fix="<%=guid %>" view="preview">
    <table class="gridview" cellspacing="0" rowcount='<%=rowCount %>' pagesize='<%=view.PageSize %>'
        <%=unselectable %>>
        <tbody>
            <tr class="dashboardTr">
                <td style="width: 100% !important; padding: 0; margin: 0">
                    <div class="gridboard">
                        <div id="<%=guid %>previewContainer" class="preview-container" style="width: <% =previewWidth %>;">
                            <%=Html.Hidden("a","enabledField", new { isEnabled = enableOrder.ToString() })%>
                            <div class="column">
                                <!------------------Preview all data-------------------->
                                <% int rowIndex = 0;

                                   System.Data.DataView dataView = tableViewer.GetDataView(Model, view, guid);
                                   if (!(view is Durados.Web.Mvc.Config.View))
                                   {
                                       Model.RowStateFilter = DataViewRowState.ModifiedOriginal;
                                       if (Model.Count == 0)
                                       {
                                           Model.RowStateFilter = System.Data.DataViewRowState.Unchanged | System.Data.DataViewRowState.Added | System.Data.DataViewRowState.ModifiedCurrent;
                                       }
                                   }

                                   ViewData["RowsCount"] = Model.Count;
                                   ViewData["View"] = view;
                                   ViewData["Guid"] = guid;
                                   ViewData["ExcludedColumn"] = excludedColumn;
                                   ViewData["Unselectable"] = unselectable;

                                   //<!------------------For each dataRow in view-------------------->
                                   foreach (System.Data.DataRowView row in Model)
                                   {
                                       if (!(view.IsPartOfPk(row.Row) && Map.Database.GetUserRole() == "View Owner"))
                                       {
                                           rowIndex++;
                                           ViewData["RowIndex"] = rowIndex;
                                           Html.RenderPartial("~/Views/Shared/Controls/Preview/DataPreviewRowView.ascx", row);
                                       }
                                %>
                                <%   }
                                %>
                            </div>
                        </div>
                        <div id="<%=guid %>_editContainer" class="preview-edit-container" style="<% =marginStyle %>">
                        </div>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
</div>
<% 
       if (view.Database.IsConfig && (view.Name == "Field") && (ViewContext.RouteData.Values["action"].ToString() == "Item" || (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri.Contains("menu=off")) || (Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains("IndexWithButtons"))))
       {
       }
       else if (!hidePager)
   {
       Html.RenderPartial("~/Views/Shared/Controls/Pager.ascx", Model.Table);
   }
%>
<%}
   catch (Exception exception)
   { %>
<span>
    <%= "$$error start$$ " + exception.Message + " $$error end$$"%></span>
<%} %>