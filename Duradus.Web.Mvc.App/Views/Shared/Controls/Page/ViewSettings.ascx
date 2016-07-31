<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.DataActionFields>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>

<% string guid = Model.Guid; %>
<% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Model.View; %>
<% if(view != null){ %>
<% bool toTranslate = view.Database.Map.Database.IsMultiLanguages && (view.Database.Map.Database.TranslateAllViews || view.Database.IsConfig);%>
<% Durados.Localization.ILocalizer local = view.Database.Localizer ?? view.Database.Map.Database.Localizer;%>

<%  
    Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];
    string prefix = Model.DataAction.ToString();

%>
<% try
   { %>
   <form d_prefix='<%=prefix %>' id='<%=guid + prefix + view.Name.ReplaceNonAlphaNumeric() %>DataRowForm' viewName='<%=view.Name %>' enctype="multipart/form-data" action="" onsubmit="return false;">

    <table cellpadding="0" cellspacing="5" >
    <%foreach (Durados.Field field in Model.Fields)
      {%>
      <% try
   { %>
        <tr class="settings-row">
                
       <% 
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
           <td>
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
        
            </tr>
            
         
        
    
    <%}
          catch (Exception exception)
          { %>
        <% Map.Logger.Log(view.Name + ";" + field.Name, "DataRowViewTab", exception.Source, exception, 1, null); %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/ErrorMessage.ascx", exception.Message); %>
        <% throw exception; %>
    <%} %>
    
    <%} %>
 
    </table>
    </form>

    <% if (view.Name == "View"){ %>
    <div class="page-button-container">
   
    <div class="page-settings-advanced-container"><a href="#" class="page-settings page-settings-advanced"><span class='button-icon'></span><span class="button-text"><%=Map.Database.Localizer.Translate("Advanced Settings")%></span></a></div>
    <div><a href="#" class="page-settings page-settings-fields"><span class='button-icon'></span><span class="button-text"><%=Map.Database.Localizer.Translate("Organize Columns")%></span></a></div>

    </div>
    <%} %>
    <%}
   catch (System.Exception exception)
   { %>
        <% Map.Logger.Log("ascx", "DataRowViewTab", exception.Source, exception, 1, null); %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/ErrorMessage.ascx", exception.Message); %>
        <% throw exception; %>
    <%} %>
<%} %>