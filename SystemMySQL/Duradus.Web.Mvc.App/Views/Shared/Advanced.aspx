<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PlugIn.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Web.Mvc.UI.Json.Settings>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%
        bool select = Convert.ToBoolean(ViewData["select"]);
        string url = "/PlugIn/EmptyWidget";
        if (ViewData["map"] != null && ViewData["viewName"] != null)
        {
            Durados.Web.Mvc.Map map = (Durados.Web.Mvc.Map)ViewData["map"];
            string viewName = ViewData["viewName"].ToString();
            url = map.Url + "/Home/IndexPage/" + viewName + "?guid=guid_&mainPage=True&menu=off";
        }
     %>

     <%if (select)
       { %>
    <fieldset>
    <p>
    <span>Application:</span>
    <select name='apps' ></select>
    </p>

    <p>
    <span>View:</span>
    <select name='views' ></select>
    </p>
    </fieldset>
    <%} %>

    <%if (select)
      { %>
    <fieldset>
    
    <span>Preview:</span>
    <%} %>
    <center>
    <iframe src='<%=url %>'></iframe>
    </center>

    <center>
    <input type="button" value="Save & Close"/>
    </center>
    <%if (select)
       { %>
    </fieldset>
    <%} %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    document.domain = '<%=Durados.Web.Mvc.Maps.Host %>';
</script>

<%

    Durados.Web.Mvc.UI.Helpers.PlugInType plugIn = (Durados.Web.Mvc.UI.Helpers.PlugInType)ViewData["PlugIn"];    
    
%>


<script src='<%=ResolveUrl("~/Scripts/jquery-1.4.2.min.js")%>' type="text/javascript"></script>
<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/PlugIn/advanced.js")%>"></script>
<% if (plugIn == Durados.Web.Mvc.UI.Helpers.PlugInType.Wix)
   { %>
   <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/PlugIn/wix.js")%>"></script>
    <script type="text/javascript" src="//sslstatic.wix.com/services/js-sdk/1.16.0/js/Wix.js"></script>
<%} %>
<script>

    var rootPath = '<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath()%>';
    var jsonModel = <%=Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(Model)%>;
    var appName = '';
    var viewName = '';
    
</script>
<style>
    fieldset p span {width:100px;float:left;}
    fieldset p select {width:200px;}
</style>
</asp:Content>
