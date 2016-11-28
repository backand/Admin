<%@ Page Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" MasterPageFile="~/Views/Account/AccountMaster.Master" %>
<%@ Import Namespace="Durados.Localization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
Sign In
</asp:Content>

<asp:Content ContentPlaceHolderID="script" runat="server">
<script type="text/javascript">

    function resetPassword() {
        var username = document.getElementById('username').value;
        window.location = '/Account/PasswordReset?username=' + username;

    }
   
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">



<% 
    string returnUrl = Request.QueryString["returnUrl"];
    string parameters = string.Empty;
    if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("plugInType"))
    {
        string[] qs = returnUrl.Split("?".ToCharArray());
        if (qs.Length > 1)
        {
            parameters = qs[1];
        }
        
    }
    
    string forgotpwLinkText = Map.Database.Localizer.Translate("Forgot your password?");
    string dontHaveAccount = Map.Database.Localizer.Translate("Don't have a BackAnd account?");

    bool isPlugIn = Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains("PlugIn");
    string urlReferrer = string.Empty;
    if (isPlugIn)
        urlReferrer = Request.UrlReferrer.ToString();
        
%>
    
     <div class="dragonicon"></div>
     <div class="bchandimage">

    <div class="signInHeader">
        <%=Map.Database.Localizer.Translate("Sign in to")%>
    </div>
    
    <div class="errors">
        <font style="color:Red;"><%= Html.ValidationSummary(Map.Database.Localizer.Translate("Username and password don’t match"))%></font>
    </div>

           <%--<form id="loginform" action="<%=Url.Action("LogOn", "Account") + "?" + parameters %>" method="post">--%>
           <form id="loginform" action="<%="/Account/LogOn" + "?" + parameters %>" method="post">
                <% = Html.Hidden("returnUrl", Request.QueryString["returnUrl"]) %>
                <div class="username">
                    <label for="username"><%=Map.Database.Localizer.Translate("Username")%>&nbsp;</label>
                    <%= Html.TextBox("username", string.Empty, new { id = "username", type = "email", placeholder="Username"})%>
                </div>
                <div class="password">
                    <label for="password"><%=Map.Database.Localizer.Translate("Password")%>&nbsp;</label>
                    <%= Html.Password("password", string.Empty, new { id="Logon_password", type = "password", placeholder="Password" })%>
                </div>
                <% if (Map.Database.LogOnUrlAuthToken != null && Map.Database.LogOnUrlAuthToken.Length > 0 )  %>
                    <%{ %>
                        <%foreach (string authToken in Map.Database.LogOnUrlAuthToken) %>
                        <% { %>
                          <div class="url-auth password">
                            <label for="url-auth-<%=authToken %>"><%=Map.Database.Localizer.Translate(authToken)%>&nbsp;</label>
                            <%= Html.TextBox(authToken, string.Empty, new { id = "Logon_"+authToken })%><%--, type = "password"--%>
                        </div>
                        <%} %>
                <%}%>
                <div class="forgotPassword">
                    <a href='JavaScript:void(0);' id="A1" name="resetPassword" onclick="resetPassword()"><%=forgotpwLinkText%></a>
                </div>
                        
                <div class="remember">
                    <label class="remember">&nbsp;</label>
                    <%= Html.CheckBox("rememberMe") %> <label class="rememberMe" for="rememberMe"><%=Map.Database.Localizer.Translate("Remember Me")%>?</label>
                

                    <div class="btn-green" id="btn_login" style=""><span name="submit" class="inner"><%=Map.Database.Localizer.Translate("Log In")%></span></div>
                    <br />
                   <%-- <div class="btn-green" id="signin-goolge" style="width:200px;"><span  class="inner">log on with Google</span></div>
                    <br />
                    <div class="btn-green" id="signin-github" style="width:200px;"><span  class="inner">log on with Github</span></div>--%>
                </div>
                <input type="submit" id="submit" style="display:none;" value="" />
                <div><%= Server.HtmlDecode(Html.GetHtml("login")) %></div>

            </form>
            </div>


<script type="text/javascript">
    var submitElement = $('[name="submit"]');
    var createElement = $('[name="createaccount"]');

    $('#signin-goolge').click(function () {
        window.location.replace("/Account/GoogleLogin?t=SignInApp");
    });
    $('#signin-github').click(function () {
        window.location.replace("/Account/GithubLogin?t=SignInApp");
    });

    submitElement.click(function () {
        $('#submit').click();
    });

    createElement.click(function () {
        window.location.href = "/Account/RegistrationRequest";
    });

    $(function () {
        $("#loginform").keypress(function (e) {
            if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                $('#submit').click();
                return false;
            } else {
                return true;
            }
        });
    });
</script>
<% if (isPlugIn)
   { %>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/PlugIn/wix.js")%>"></script>
    <script type="text/javascript" src="//sslstatic.wix.com/services/js-sdk/1.17.0/js/Wix.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            document.location = '<%=urlReferrer %>';
        }
    </script>
<%} %>
</asp:Content>



