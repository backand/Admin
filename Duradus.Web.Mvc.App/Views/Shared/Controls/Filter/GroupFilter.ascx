<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.View>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json" %>
<!------------------General variables -------------------->
<%  string guid = ViewData["guid"] as string;
    string textSearch = Map.Database.Localizer.Translate("Search on text...");
    string search = ViewData["search"] as string;
    Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];
    string viewFilterVisibility = Model.Name + "_filterVisibility";
    bool hideFilter = Model.HideFilter && !Model.IsInAdminMode();

    bool collapseFilter = Model.CollapseFilter || (Map.Session[viewFilterVisibility] != null && !(bool)Map.Session[viewFilterVisibility]);
    if (Durados.Web.Infrastructure.General.IsMobile())
        collapseFilter = true;
   
    if (string.IsNullOrEmpty(search)) search = textSearch;
%>
<div class="groupFilter ui-widget" guid='<%=guid %>' name='rowfilter' <%= collapseFilter? " style='display:none'" : "" %>
    id=" ">
    <!------------------For Each Filter Type-------------------->
    <% foreach (Durados.Field field in Model.VisibleFieldsForFilter)
       {
           object filterValue = (ViewData["filter"] == null) ? null : (((Dictionary<string, Field>)ViewData["filter"]).ContainsKey(field.Name)) ? ((Dictionary<string, Field>)ViewData["filter"])[field.Name].Value : null;
           string elementForFilter = string.Empty;

           //Build filter element for milestones field
           if (field.FieldType == Durados.FieldType.Column && ((Durados.Web.Mvc.ColumnField)field).IsMilestonesField)
           {
               tableViewer.Gantt.Init((Durados.Web.Mvc.ColumnField)field, filterValue);
               elementForFilter = field.GetElementForFilter(tableViewer.Gantt.FilterText, guid);
           }
           //Build filter element for regular field
           else
           {
               if (filterValue != null && filterValue.ToString() == search)
               {
                   filterValue = "";
               }
               elementForFilter = field.GetElementForFilter(filterValue, guid);
           }

           //If there is a filter element to display for current field
           if (!string.IsNullOrEmpty(elementForFilter))
           {
    %>
    <div class="groupFilter_item">
        <!------------------Label For Filter Field-------------------->
        <%if (field.NeedDisplayFilterLabel())
          { %>
        <span class="groupFilter_label">
            <%= field.DisplayName%>:</span>
        <%} %>
        <span class="groupFilter_element">
            <%= elementForFilter%>
            </span>
    </div>
    <%     }
       } %>
    <div class="groupFilter_searchActions">
        <!------------------Free Search-------------------->
        <div class="group groupFilter_freeSearch">
            <%Html.RenderPartial("~/Views/Shared/Controls/Filter/FreeFilter.ascx", Model);%>
        </div>
        <!------------------Show/Hide/Apply/Clear Filter-------------------->
        <div class="group groupFilter_applyClear">
            <%ViewData["displayShowHideFilter"] = false;
              Html.RenderPartial("~/Views/Shared/Controls/Filter/ApplyClearFilter.ascx", Model);
            %>
        </div>
    </div>
</div>
