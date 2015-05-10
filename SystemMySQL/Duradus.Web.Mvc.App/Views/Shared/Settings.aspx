<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PlugIn.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Web.Mvc.UI.Json.Settings>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

      <% Html.RenderPartial("~/Views/PlugIn/Controls/Settings.ascx"); %>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

<%

    Durados.Web.Mvc.UI.Helpers.PlugInType plugIn = (Durados.Web.Mvc.UI.Helpers.PlugInType)ViewData["PlugIn"];    
    
%>

<script src='<%=ResolveUrl("~/Scripts/jquery.min.js")%>' type="text/javascript"></script>
<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/PlugIn/plugin.js")%>"></script>
<% if (plugIn == Durados.Web.Mvc.UI.Helpers.PlugInType.Wix)
   { %>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/PlugIn/wix.js")%>"></script>
    <script type="text/javascript" src="//sslstatic.wix.com/services/js-sdk/1.17.0/js/Wix.js"></script>
<%} %>
<script type="text/javascript">

    var rootPath = '<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath()%>';
    var jsonModel = <%=Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(Model)%>;
    var appName = '';
    var viewName = '';
</script>
</asp:Content>
