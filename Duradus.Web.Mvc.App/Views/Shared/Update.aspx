<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PlugIn.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Web.Mvc.UI.Json.Settings>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%
        string url = "/PlugIn/EmptyWidget";
        if (ViewData["map"] != null && ViewData["viewName"] != null)
        {
            Durados.Web.Mvc.Map map = (Durados.Web.Mvc.Map)ViewData["map"];
            string viewName = ViewData["viewName"].ToString();
            string id = ViewData["id"].ToString();
            url = map.Url + "/Home/IndexPage/" + viewName + "?guid=guid_&mainPage=True&menu=off&settings=fields&id=" + id;
        }
     %>

     
    <center>
        <iframe src='<%=url %>'></iframe>
    </center>
     
    <%--<div class="btn-green"><span name="close" class="inner">Update & Close</span></div>
    <div class="btn-green"><span name="Update" class="inner">Update</span></div>--%>
     
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

<%

    Durados.Web.Mvc.UI.Helpers.PlugInType plugIn = (Durados.Web.Mvc.UI.Helpers.PlugInType)ViewData["PlugIn"];    
    
%>


<script src='<%=ResolveUrl("~/Scripts/jquery.min.js")%>' type="text/javascript"></script>
<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/PlugIn/update.js")%>"></script>
<% if (plugIn == Durados.Web.Mvc.UI.Helpers.PlugInType.Wix)
   { %>
   <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/PlugIn/wix.js")%>"></script>
    <script type="text/javascript" src="//sslstatic.wix.com/services/js-sdk/1.17.0/js/Wix.js"></script>
<%} %>
<script type="text/javascript">
    document.domain = '<%=Durados.Web.Mvc.Maps.Domain %>';
</script>
    
<script type="text/javascript">
    var rootPath = '<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath()%>';
    var jsonModel = <%=Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(Model)%>;
</script>
<style type="text/css">
    fieldset p span {width:100px;float:left;}
    fieldset p select {width:200px;}
</style>
</asp:Content>
