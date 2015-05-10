<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.DataActionFields>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>

<%  
    switch (Model.View.DataRowView)
    {
        //case Durados.Web.Mvc.DataRowView.Columns:
        //    Html.RenderPartial("~/Views/Shared/Controls/DataRowViewColumns.ascx", Model);
        //    break;
        case Durados.Web.Mvc.DataRowView.Tabs:
            if(!Durados.Web.Infrastructure.General.IsMobile())
                Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTabs.ascx", Model);
            else
                Html.RenderPartial("~/Views/Shared/Controls/Accordion/DataRowViewAccordion.ascx", Model);
            break;
        case Durados.Web.Mvc.DataRowView.Groups:
            Html.RenderPartial("~/Views/Shared/Controls/DataRowViewGroups.ascx", Model);
            break;
        case Durados.Web.Mvc.DataRowView.Divs:
            Html.RenderPartial("~/Views/Shared/Controls/DataRowViewDivs.ascx", Model);
            break;
        case Durados.Web.Mvc.DataRowView.Accordion:
            if (Model.View.Database.IsConfig && (Model.View.Name == "View" || Model.View.Name == "Field") && (ViewContext.RouteData.Values["action"].ToString() == "Item" || (Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains("IndexWithButtons"))))
            {
                Html.RenderPartial("~/Views/Shared/Controls/Accordion/DataRowViewAccordion.ascx", Model);
            }
            else
            {
                Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTabs.ascx", Model); 
            }
            break;
        default:
            break;
    }
%>
