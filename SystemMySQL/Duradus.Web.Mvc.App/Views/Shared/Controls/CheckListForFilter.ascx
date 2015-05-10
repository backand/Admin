<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Durados.Web.Mvc.ParentField>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>

<% string guid = ViewData["guid"].ToString(); %>
<%= Model.GetCheckListForFilter(guid) %>