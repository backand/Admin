<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.CheckList>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json"%>

<%=Html.DropDownList(Model.Name, Model.SelectList, new {id=Model.ID, multiple="multiple", viewName=Model.ChildrenField.View.Name, @class="checklist"}) %>

