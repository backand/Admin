<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.DataActionFields>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>
<%@ Import Namespace="Durados.Web.Mvc.Infrastructure"%>

<% 
    string guid = Model.Guid;
    Durados.Web.Mvc.View view = Model.View;

    string prefix = Model.DataAction.ToString();

    bool? clientValidation = (bool?)ViewData["clientValidation"]; 
    
    DateTime now = DateTime.Now;
    string secInDay = (now.Hour * 60 * 60 + now.Minute * 60 + now.Second).ToString() + "_" + now.Millisecond.ToString();
%>

<div style="display:none" class="dialogTitle"></div>
    
            
<form d_prefix='<%=prefix %>' id='<%=guid + prefix + view.Name.ReplaceNonAlphaNumeric() %>DataRowForm' viewName='<%=view.Name %>' enctype="multipart/form-data" action="" onsubmit="return false;">
<% if (view.HasCategories)
   { %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTab.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Model.DataAction, view.FieldsWithNoCategory.Fields), DataAction = Model.DataAction, Guid = guid, View = view }); %>

        <% 
            var categories = view.Categories.Values.OrderBy(c => c.Ordinal);
            Durados.Category firstCategory = categories.FirstOrDefault();
        %>



        <div id="<%=guid + prefix%>Tabs">
            <ul>
                <% int index = 0; %>
                <% foreach (Durados.Category category in categories)
                   { %>
                    <% string categoryName = category.Name.ReplaceNonAlphaNumeric(); %>
                    <% string tabID = guid + prefix + categoryName + secInDay; %>
                    <% if (category.Fields.Where(f=>f.IsVisibleForRow(Model.DataAction)).Count() > 0)
                       {
                           if (category.Fields.Count == 1 && category.Fields[0].FieldType == Durados.FieldType.Children && category != firstCategory) 
                           {
                               Durados.Web.Mvc.ChildrenField f = (Durados.Web.Mvc.ChildrenField)category.Fields[0];
                               if (((Durados.Web.Mvc.View)f.ChildrenView).IsAllow())
                               {
                                   string url = f.GetElementForTableView(null, guid);
                                   string nocache = f.NoCache ? "nocache='nocache'" : "";
                                   string tabTitle = categoryName.Replace('_', ' ');
                                   tabTitle = Map.Database.Localizer.Translate(tabTitle);//Map.Database.Localizer.Translate(
                                   //if (f.HasCounter && Model.DataAction == Durados.DataAction.Edit)
                                   //    tabTitle += " (" + "xxxx" + ")";
                               %>
                                <li index="<%=index %>" haschildren="haschildren" url="<%=url %>" childrenFieldName='<%=f.Name %>'>
                                    <a href="<%=url %>" <%=nocache%> title="">
                                        <span><%= tabTitle%>
                                            <% if (f.HasCounter && Model.DataAction == Durados.DataAction.Edit){ %>
                                            (<span hasCounter='hasCounter'></span>)
                                            <%} %>
                                        </span>
                                    </a>
                                </li>
                               <%} %>
                        <%} else { %>
                                <li><a href="#<%=tabID %>"><span><%=categoryName.Replace('_', ' ')%></span></a></li>
                        <%} %>
                    <% index++; %>
                    <%} %>
                <%} %>
            </ul>
            <% foreach (Durados.Category category in categories)
               { %>
            <% if (category.Fields.Count > 0 && (category.Fields.Count > 1 || category.Fields[0].FieldType != Durados.FieldType.Children || category == firstCategory))
               { %>
                <% string categoryName = category.Name.ReplaceNonAlphaNumeric(); %>
                <% string tabID = guid + prefix + categoryName + secInDay; %>
                <div id="<%=tabID %>">
                    <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTab.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Model.DataAction, category.Fields), DataAction = Model.DataAction, Guid = guid, Category = category, View = view }); %>
                </div>
            <%} %>
            <%} %>
        </div>
            
        
    <%}
   else
   { %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTab.ascx", Model); %>
<%} %>
</form>

