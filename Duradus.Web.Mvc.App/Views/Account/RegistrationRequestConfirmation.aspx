<%@ Page Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" MasterPageFile="~/Views/Account/AccountMaster.Master" %>
<%@ Import Namespace="Durados.Localization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
Registration
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="signInHeader">
            <h1><%=Map.Database.Localizer.Translate("Registration")%></h1>
            </div>
            
            <div class="centerParent leftFloat" style="width:100px">
                <div class="center" style="width: 1px; height: 328px; background-color: rgb(221, 221, 221); "></div>
            </div>
            
            <div class="leftFloat" style="width:320px">
    <div class="registration">
        <%if ((bool)(ViewData["confirmed"] ?? false))
            { %>
            <%= Server.HtmlDecode(Html.GetHtml("registration confirmation"))%>
        <%} else {%>
            <%= Server.HtmlDecode(Html.GetHtml("registration confirmation failed"))%>
        <%} %>
    </div>
    </div>
</asp:Content>

