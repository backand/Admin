<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.DataActionFields>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%@ Import Namespace="Durados.Web.Mvc.Infrastructure" %>
<% 
    string guid = Model.Guid;
    Durados.Web.Mvc.View view = Model.View;

    string prefix = Model.DataAction.ToString();

    bool? clientValidation = (bool?)ViewData["clientValidation"];

    DateTime now = DateTime.Now;
    string secInDay = (now.Hour * 60 * 60 + now.Minute * 60 + now.Second).ToString() + "_" + now.Millisecond.ToString();
%>
<div style="display: none" class="dialogTitle">
</div>
<form d_prefix='<%=prefix %>' id='<%=guid + prefix + view.Name.ReplaceNonAlphaNumeric() %>DataRowForm'
viewname='<%=view.Name %>' enctype="multipart/form-data" action="" onsubmit="return false;">
<% if (view.HasCategories)
   { %>
<%  Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTab.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Model.DataAction, view.FieldsWithNoCategory.Fields), DataAction = Model.DataAction, Guid = guid, View = view }); %>
<div id="<%=guid + prefix%>Accordion">
    <% int index = 0; %>
    <% foreach (Durados.Category category in view.Categories.Values.OrderBy(c => c.Ordinal))
       { %>
    <% string categoryName = category.Name.ReplaceNonAlphaNumeric(); %>
    <% string tabID = guid + prefix + categoryName + secInDay; %>
    <% if (category.Fields.Count(f => f.IsVisibleForRow(Model.DataAction)) > 0)
       {
           if (category.Fields.Count == 1 && category.Fields[0].FieldType == Durados.FieldType.Children)
           {
               Durados.Web.Mvc.ChildrenField field = (Durados.Web.Mvc.ChildrenField)category.Fields[0];
               if (((Durados.Web.Mvc.View)field.ChildrenView).IsAllow())
               {
                   string url = field.GetElementForTableView(null, guid);
                   string nocache = field.NoCache ? "nocache='nocache'" : "";
                   string tabTitle = Map.Database.Localizer.Translate(categoryName.Replace('_', ' '));
    %>
    <h3 index="<%=index %>" <%=nocache%> haschildren="haschildren" url="<%=url %>" childrenfieldname='<%=field.Name %>'>
        <span>
            <%= tabTitle%>
            <% if (field.HasCounter && Model.DataAction == Durados.DataAction.Edit)
               { %>
            (<span hascounter='hasCounter'></span>)
            <%} %>
        </span>
    </h3>
    <div>
    </div>
    <%} %>
    <%}
           else
           { %>
    <h3>
        <span>
            <%=Map.Database.Localizer.Translate(categoryName.Replace('_', ' '))%></span></h3>
    <div>
        <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTab.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Model.DataAction, category.Fields), DataAction = Model.DataAction, Guid = guid, Category = category, View = view }); %>
    </div>
    <%} %>
    <% index++; %>
    <%} %>
    <%} %>
</div>
<%}
   else
   { %>
<%  Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTab.ascx", Model); %>
<%} %>
</form>
