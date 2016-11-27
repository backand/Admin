<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.DataActionFields>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>

<% string guid = Model.Guid; %>
<% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Model.View; %>
<% int col = 0; %>
<% bool isAccordion = Durados.Web.Infrastructure.General.IsMobile() || view.DataRowView == Durados.Web.Mvc.DataRowView.Accordion; %>
<% bool isDock =  view.Database.IsConfig && (view.Name == "View" || view.Name == "Field") && (ViewContext.RouteData.Values["action"].ToString() == "Item" || (Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains("IndexWithButtons"))); %>
<% bool toTranslate = view.Database.Map.Database.IsMultiLanguages && (view.Database.Map.Database.TranslateAllViews || view.Database.IsConfig);%>
<% Durados.Localization.ILocalizer local = view.Database.Localizer ?? view.Database.Map.Database.Localizer;%>

<%  
    Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];
    System.Random rand = new Random();

    if (Model != null && Model.Category != null && Model.Category.Name == "Advanced_Design")
    {
        isDock = false;
    }
%>
<%bool showLable;%>
<% try
   {
       int viewColDialog = Durados.Web.Infrastructure.General.IsMobile() || (isAccordion && isDock) ? 1 : view.GetColumnsInDialog(Model.Category);
       %>
    <table cellpadding="0" cellspacing="5" class="editForm tableCol<%=viewColDialog %>">
    <%foreach (Durados.Field field in Model.Fields)
      {%>
      <%showLable = false; %>
       <% try
          { %>
      <%
              int fieldColSpan = Durados.Web.Infrastructure.General.IsMobile() ? 1 : field.ColSpanInDialog;
          %>
        <% if (field.IsVisibleForRow(Model.DataAction))
           { %>
        <% int cosInRow = col % viewColDialog; %>
        <%if (cosInRow == 0 || cosInRow + fieldColSpan > viewColDialog)
          {%>
            <%showLable = true; %>
            <% if (col >= 0)
               {
                   col = 0;%>
                </tr>
                 <%showLable = true; %>
                <% if (field.Seperator)
                   { %>
                    <tr>
                        <td colspan="<%=viewColDialog*2 %>"><hr /><% if (!string.IsNullOrEmpty(field.SeperatorTitle))
                                                                     { %><span class="SeperatorTitle">
                                                                     <%=toTranslate ? local.Translate(field.SeperatorTitle) : field.SeperatorTitle%>
                                                                     </span><%} %></td>
                    </tr>
                <%}
                   else if (!string.IsNullOrEmpty(field.PreLabel))
                   {%>
                    <tr>
                        <td colspan="<%=viewColDialog*2 %>"><span class="fieldLabel">
                        <% string preLableText = field.PreLabel.Replace(Durados.Database.IPAddressPlaceHolder, "137.117.97.68"); %>
                        <%=toTranslate ? local.Translate(preLableText) : preLableText%>
                        
                        </span></td>
                       
                    </tr>
                <%} %>
            <%} %>
            <tr>
       <% }%>
       <% 
          int span = fieldColSpan * 2 - 1;
          if (field.LabelContentLayout == Durados.Orientation.Vertical)
              span = span + 1;
          string tdClass = "rowViewCell";
          if (field.FieldType == Durados.FieldType.Children && ((Durados.Web.Mvc.ChildrenField)field).ChildrenHtmlControlType != Durados.Web.Mvc.ChildrenHtmlControlType.CheckList)
          {
              tdClass = "";
          }
          string hideInDerivation = string.Empty;
          if (field.HideInDerivation)
          {
              hideInDerivation = "hideInDerivation='hideInDerivation'";
          }

          string description = Map.Database.Localizer.Translate(field.Description);
          string upgradePlanContent = field.IsInPlan ? string.Empty : Database.GetPlanContent();
        
        %>
        <% if (field.LabelContentLayout == Durados.Orientation.Horizontal && (!isAccordion))
           { %>
           <% if (!(Model.Fields.Count == 1 && field.FieldType == Durados.FieldType.Children && !field.IsCheckList())) {%>
            <td class="rowViewLable" title='<%= description %>' <%=hideInDerivation %>>
                <% if (!field.IsHidden())
                   { %>
                    <%=tableViewer.GetDisplayName(field, null, guid)%><%if(!string.IsNullOrEmpty(description)){ %>
                                        <i class="icon-info_outline" title="<%=description %>"></i>
                                        <%} %>:
                <%} %>
                <%=upgradePlanContent%>
            </td>
            <%} %>
            <% string nowrap = field.FieldType == Durados.FieldType.Column && ((Durados.ColumnField)field).DataColumn.DataType == typeof(DateTime) ? "white-space: nowrap" : ""; %>
            <td id='the<%=guid + field.GetDataActionPrefix(Model.DataAction) + view.Name + "_" + field.Name %>' colspan='<%=span.ToString() %>' style="<%=nowrap%>" class="<%=tdClass%>" <%=hideInDerivation %>>
                <%=field.GetElementForRowView(Model.DataAction, guid)%>
                <%=field.GetValidationElements(Model.DataAction, guid)%>
            </td>
        <%}
           else
           { %>
           <td colspan='<%=span.ToString() %>' <%=hideInDerivation %>>
           <table>
            <tr>
                <%
               string elm = field.GetElementForRowView(Model.DataAction, guid);
               bool chk = elm.Contains("type='check") && !elm.Contains("color='1'");
                 %>
                 <% if (!chk)
                    { %>
                    <span aa="aa" style="margin-left:3px"><%=tableViewer.GetDisplayName(field, null, guid)%>:</span>
                    <%=upgradePlanContent%>
                <%} %>
                <td id='the<%=guid + field.GetDataActionPrefix(Model.DataAction) + view.Name + "_" + field.Name %>' class="<%=tdClass%>">
                <%=elm%>
                <% if (chk)
                   { %>
                   <%= tableViewer.GetDisplayName(field, null, guid) %>
                    <%=upgradePlanContent%>
                <%} %>
                <%=field.GetValidationElements(Model.DataAction, guid)%>
            </td>
            </tr>
            
           </table>
           </td>
        <%} %>
         <%if (showLable && !string.IsNullOrEmpty(field.PostLabel))
           {%>
                    <tr>
                        <td colspan="<%=viewColDialog*2 %>"><span class="fieldLabel">
                        <%=toTranslate?local.Translate(field.PostLabel):field.PostLabel%>
                        
                        </span></td>
                    </tr>
                <%} %>
        <%  col += fieldColSpan;%>
    <%} %>
    
    <%}
          catch (Exception exception)
          { %>
        <% Map.Logger.Log(view.Name + ";" + field.Name, "DataRowViewTab", exception.Source, exception, 1, null); %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/ErrorMessage.ascx", exception.Message); %>
        <% throw exception; %>
    <%} %>
    
    <%} %>
        </tr>    
    </table>

    <%}
   catch (Exception exception)
   { %>
        <% Map.Logger.Log("ascx", "DataRowViewTab", exception.Source, exception, 1, null); %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/ErrorMessage.ascx", exception.Message); %>
        <% throw exception; %>
    <%} %>