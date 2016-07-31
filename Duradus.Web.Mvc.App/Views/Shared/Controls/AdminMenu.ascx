<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Durados.Web.Mvc.App.Views.Shared.Controls.AdminMenuModel>" %>
<%@ Import Namespace="Durados.DataAccess" %>

<div class="menu" style="display:<%=Model.Views.Count()==0?"none":"block" %>">
    <img id='adminMenuImg<%=Model.MenuName %>' border='0' src='<%=Model.Expand?Durados.Web.Mvc.Infrastructure.General.GetRootPath()+"Content/Images/Minus.JPG":Durados.Web.Mvc.Infrastructure.General.GetRootPath()+"Content/Images/Plus.JPG"%>' onclick="displayMenu('adminMenu<%=Model.MenuName %>', 'adminMenuImg<%=Model.MenuName %>')"/>
    <span ><%=Model.MenuName%></span>
    <div id='adminMenu<%=Model.MenuName %>' style='display:<%=Model.Expand?"block":"none"%>; margin-left:18px'>
        <%  Html.RenderPartial("~/Views/Shared/Controls/Menu.ascx", Model.Views); %>
        
    </div>
</div>


