<%@ Page Title="" Language="C#" MasterPageFile="~/Views/PlugIn/PlugIn.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Web.Mvc.UI.Json.ViewStyle>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>



<%-- R E A D    T H I S:  Use the MainContent placeholder for the content. Do not put any elements outside this placeholder! --%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <% Html.RenderPartial("~/Views/PlugIn/Controls/ViewStyle.ascx"); %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" src="//sslstatic.wix.com/services/js-sdk/1.16.0/js/Wix.js"></script>
<script src='<%=ResolveUrl("~/Scripts/jquery-1.4.2.min.js")%>' type="text/javascript"></script>
<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/PlugIn/plugin.js")%>"></script>

<script>

var jsonModel = <%=Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(Model)%>;
var rootPath = '<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath()%>';
</script>

</asp:Content>