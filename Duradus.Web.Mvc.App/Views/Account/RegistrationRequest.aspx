<%@ Page Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" MasterPageFile="~/Views/Account/AccountMaster.Master" %>
<%@ Import Namespace="Durados.Localization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
Registration
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="style" runat="server">

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="script" runat="server">
<script type="text/javascript">
    function queryString(key) {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars[key];
    }

    var returnUrl = '<%= ViewData["returnUrl"]%>';

    $(document).ready(function () {

        var firstName = queryString("firstName");
        if (firstName) {
            $('[name="First_Name"]').val(decodeURIComponent(firstName).replace('+', ' '));
            $('[name="First_Name"]').attr('disabled', true);
        }
        else {
            $('[name="First_Name"]').attr('disabled', false);
        }
        var lastName = queryString("lastName");
        if (lastName) {
            $('[name="Last_Name"]').val(decodeURIComponent(lastName).replace('+', ' '));
            $('[name="Last_Name"]').attr('disabled', true);
        }
        else {
            $('[name="Last_Name"]').attr('disabled', false);
        }
        //           if (lastName) $('[name="Last_Name"]').val(decodeURIComponent(lastName).replace('+',' '));
        var username = queryString("username");
        if (username) {
            $('[name="Email"]').val(decodeURIComponent(username));
            $('[name="Email"]').attr('disabled', true);
        }
        else {
            $('[name="Email"]').attr('disabled', false);
        }

        $('input[type="submit"]').click(function () {
            var form = $('form');
            if (Spry.Widget.Form.validate(form[0]))
                register($(this));
        });
    });

    function register(button) {
        $("body").css("cursor", "wait");
        button.attr('disabled', true);
        $.post($('form').attr('action'),
            {
                Username: $('[name="Username"]').val(),
                First_Name: $('[name="First_Name"]').val(),
                Last_Name: $('[name="Last_Name"]').val(),
                Email: $('[name="Email"]').val(),
                DefaultRole: $('[name="DefaultRole"]').val(),
                Reason: $('[name="Reason"]').val(),
                Type_of_Access: $('[name="Type_of_Access"]').val(),
                Group: $('[name="Group"]').val(),
                to: $('[name="to"]').val(),
                cc: $('[name="cc"]').val(),
                Password: $('[name="Password"]').val(),
                ConfirmPassword: $('[name="ConfirmPassword"]').val(),
                Subject: $('[name="Subject"]').val(),
                returnUrl: returnUrl
            },
            function (error) {
                button.attr('disabled', false);
                if (error == 'success') {
                    $('div.error').hide();
                    window.location = $('#RegistrationRequestConfirmation').val() + "?confirmed=true";
                }
                else if (error == 'error') {
                    $('div.error').hide();
                    window.location = $('#RegistrationRequestConfirmation').val() + "?confirmed=false";
                }
                else if (error.error == 'success') {
                    $('div.error').hide();
                    window.location = decodeURIComponent(error.url);
                }
                else {
                    $('div.error').html(error);
                    $('div.error').show();
                }
                button.attr('disabled', false);
                $("body").css("cursor", "default");
            });
    }
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="registerdrogon"></div>
    <div class="signInHeader">
        <%=Map.Database.Localizer.Translate("Sign in to ")%>
    </div>
            
            
 <!--<div class="error"></div>-->
        <% 
            string controller = "DuradosAccount";
            //if (Map.Database.SiteInfo.Product.ToLower() == Durados.Web.Mvc.Maps.GetMainAppName().ToLower())
            //    controller = "DuradosAccount";    
        %>
        <form  id="loginform" action="<%=Url.Action("RegistrationRequest", controller, new {appName=Request.QueryString["appName"], id=Request.QueryString["id"], instance=Request.QueryString["instance"]})%>" method="post">
            <div class="registration">
                <% string content = string.Empty;
                    if (!Durados.Web.Infrastructure.General.IsMobile()) {                
                        content= Server.HtmlDecode(Html.GetHtml("registration"));
                   } else {
                       content = Server.HtmlDecode(Html.GetHtml("registrationMobile"));
                   } %>       
                   <%= content%>
            </div>

             <div class="username">
                    <label for="username"><%=Map.Database.Localizer.Translate("Username")%>&nbsp;</label>
                    <%= Html.TextBox("username", string.Empty, new { id = "fullname", type = "email", placeholder="Full Name"})%>
                </div>

                <div class="emailaddress">
                    
                    <%= Html.TextBox("emailaddress", string.Empty, new { id = "emailaddress", type = "email", placeholder="Email Address (username)"})%>
                </div>

                <div class="logonpassword">
                    
                    <%= Html.Password("password", string.Empty, new { id="password", type = "password", placeholder="Password" })%>
                </div>
                 <div class="password">
                    <label for="password"><%=Map.Database.Localizer.Translate("Password")%>&nbsp;</label>
                    <%= Html.Password("password", string.Empty, new { id="reset_password", type = "password", placeholder="Re-type Password" })%>
                </div>
                <div class="signup_button" id="btn_signup" style=""><span name="submit" class="inner"><%=Map.Database.Localizer.Translate("Sign Up")%></span></div>
         

        </form>
</asp:Content>

