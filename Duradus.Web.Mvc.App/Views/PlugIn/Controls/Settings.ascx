<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    bool isRegisteredUser = (bool)ViewData["isRegisteredUser"];
    bool isFree = (bool)ViewData["isFree"];
    bool isSampleApp = (bool)ViewData["isSampleApp"];
    //string disabled = isFree ? "" : "disabled='disabled'";
    bool isDebug = System.Configuration.ConfigurationManager.AppSettings["Debug"].ToLower() == "true";
%>

<div class="pg_header pg_top_section pg_inline_block">
    <div style="display:inline-block;">
    <div style="display:inline-block;">
        <img src="../../Content/Images/logoMBWix.png" alt="BackAnd Logo" class="pg_logo" />
    </div>
    <div style="display:inline-block;">
<% if (isFree && isSampleApp){ %>
        <div class="btn-green"><span name="upgrade" class="inner">Upgrade to Premium</span></div>
<%} %>
    </div>
    <div style="display:inline-block;">
    <% if (isRegisteredUser)
   { %>
    <div class="btn-gray" style="margin-left: 32px;"><span name="myAccount" class="inner">Connect to BackAnd</span></div>
    <%if(isDebug){ %>
        <input type="button" value="Logoff" name="logoff" />
    <%} %>
<%}
   else
   { %>
    <div class="btn-gray" style="left: 182px;"><span name="myAccount" class="inner">Connect to BackAnd</span></div>
<%} %>

    </div>
    </div>

    <div style="display:inline-block;padding-bottom:10px;">With Back&'s List Creator app you can create lists with search, filter and sort. Import lists and data from excel and make them available on your Wix site. <br />You can even connect your existing database to List Creator and display dynamic content on your Wix site without duplicating data or import/export harassments.</br><a href="javascript:void(0);" onclick="window.open('https://www.BackAnd.com/contact', 'BackAnd', 'width=1450,height=820');">Feedback and suggestions are welcome</a></div>

</div>

<div class="pg_header pg_top_section pg_inline_block">
    <div style="padding-top:10px;">Select your Grid:</div>
    <div style="padding-bottom:5px;"><select <%--<%=disabled %>--%> name='appsAndViews' style="width:360px;"></select></div>
    <img class='excelImportImage'  name="excelImport" alt="" src="../../../Content/Images/Excel-icon-disabled.png"  />
    <div class="btn-green" style="position: absolute; left: 443px; top: 20px;height:10px;"><span name="editData" class="inner">Edit Data</span></div>
    <%--<button type="button" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" style="left:0px; onclick='alert(5)'><span onclick='alert(6)' class="ui-button-text">bbbb</span></button>--%>
<iframe id="settingIframe" onload="enableImport();">
</iframe>
</div>
