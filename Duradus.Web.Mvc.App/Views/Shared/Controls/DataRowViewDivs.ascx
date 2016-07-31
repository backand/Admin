<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.DataActionFields>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>

<% 
    string guid = Model.Guid;
    Durados.Web.Mvc.View view = Model.View;

    string prefix = Model.DataAction.ToString();
%>

 
            
<form id='<%=guid + prefix + view.Name.ReplaceNonAlphaNumeric() %>DataRowForm' enctype="multipart/form-data" action="" onsubmit="return false;">
<% if (view.HasCategories)
   { %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTab.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Model.DataAction, view.FieldsWithNoCategory.Fields), DataAction = Model.DataAction, Guid = guid, View = view }); %>
         
            <% foreach (Durados.Category category in view.Categories.Values.OrderBy(c => c.Ordinal))
               { %>
            <% if (category.Fields.Count > 0)
               { %>
               <fieldset>
                    <legend><%= category.Name%></legend>
                    <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTab.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Model.DataAction, category.Fields), DataAction = Model.DataAction, Guid = guid, View = view }); %>
                </fieldset>
                <br />
            <%} %>
            <%} %>
    <%}
   else
   { %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTab.ascx", Model); %>
<%} %>
</form>
