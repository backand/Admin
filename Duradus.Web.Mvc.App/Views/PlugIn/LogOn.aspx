<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PlugIn.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div id="signIn" title="">
  <div class="sign-up-form">
    <form method="post" action="">
    <div class="inner">
      <div class="title">Login</div>
      <div class="sub-title">Login to BackAnd</div>
      <div><span class="error-message general">Incorrect username or password</span>&nbsp;</div>
      <fieldset>
        <label>Your email address </label>
        <div class="field-holder ui-corner-all">
          <input type="text" class="textfield" tabindex="1" name="userName"/>
          <span class="error-message">Data required</span></div>
      </fieldset>
      <fieldset>
        <label>Password</label>
        <div class="field-holder ui-corner-all">
          <input type="password" class="textfield" name="password" tabindex="2"/>
          <span class="error-message">Data required</span></div>
      </fieldset>
           
      <fieldset>
                        <p class="forgotPassword">
                            <label class="forgotPassword">&nbsp;</label>
                           <a href='JavaScript:void;' name="resetPassword" onclick="Forgot.resetPassword()">Forgot your password?</a>
                        </p>
      </fieldset>
             
      <div class="login-item login closeMe" >Login</div>
    </div>
    </form>
  </div>
</div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    document.domain = '<%=Durados.Web.Mvc.Maps.Host %>';
</script>
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
</script>
</asp:Content>
