<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.DataActionFields>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>

<form id='<%=guid + Model.DataAction.ToString() + Model.View.Name.ReplaceNonAlphaNumeric() %>DataRowForm' enctype="multipart/form-data" action="" onsubmit="return false;">
    <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowViewTab.ascx", Model); %>
</form>
