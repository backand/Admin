<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PlugIn.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Web.Mvc.UI.Json.Settings>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

     <fieldset>
    <p>
    <span>Application:</span>
    <select name='apps' ></select>
    </p>

    <p>
    <span>View:</span>
    <select name='views' ></select>
    </p>

    <input type="button" name="close" value="Update and Close"/>
        
    </fieldset>
    
    <fieldset>
    
    <span>Preview:</span>
    <center>
        <div id="IframeWrapper" style="position: relative;"> 
            <div id="iframeBlocker" style="position: absolute; top: 0; left: 0; width: 100%; height: 300px"></div>
            <iframe src=''></iframe>
        </div>
    </center>
    
    </fieldset>
   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">


<%

    Durados.Web.Mvc.UI.Helpers.PlugInType plugIn = (Durados.Web.Mvc.UI.Helpers.PlugInType)ViewData["PlugIn"];    
    
%>


<script src='<%=ResolveUrl("~/Scripts/jquery.min.js")%>' type="text/javascript"></script>
<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/PlugIn/select.js")%>"></script>
<% if (plugIn == Durados.Web.Mvc.UI.Helpers.PlugInType.Wix)
   { %>
   <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/PlugIn/wix.js")%>"></script>
    <script type="text/javascript" src="//sslstatic.wix.com/services/js-sdk/1.17.0/js/Wix.js"></script>
<%} %>
<script>

    var rootPath = '<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath()%>';
    var jsonModel = <%=Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(Model)%>;
    
</script>
<style>
    fieldset p span {width:100px;float:left;}
    fieldset p select {width:200px;}
</style>
</asp:Content>
