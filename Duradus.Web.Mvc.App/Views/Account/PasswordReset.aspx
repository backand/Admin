<%@ Page Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" MasterPageFile="~/Views/Account/AccountMaster.Master" %>
<%@ Import Namespace="Durados.Localization" %>


<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    Reset Password
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<div class="bgdragonimage"></div>
    <div class="signInHeader">
        <%=Map.Database.Localizer.Translate("Reset  Password")%>
    </div>

    <div class="error" style="border: none;text-align: left;margin-left: 60px;display: none;margin-top: 15px;">
        <font style="color:Red"></font><%-- <%= Html.ValidationSummary(Map.Database.Localizer.Translate("reset was unsuccessful. Please correct the errors and try again."))%>--%>
    </div>
    <% 
        string controller = "Account";
        if (Map.IsMainMap)
            controller = "DuradosAccount"; 
                   
    %>
    <form onsubmit="return false;" action="<%=Url.Action("PasswordReset", controller, new {appName=Request.QueryString["appName"], id=Request.QueryString["id"]})%>" method="post">
        <div class="pwResetForm">
            <div class="instructions"><%=Map.Database.Localizer.Translate("To reset your password, enter your Back& username")%></div>
            <div class="resetpassworduser">
               
                <%= Html.TextBox("username", ViewData["username"], new { id = "username", type = "email", placeholder="Username" })%>
            </div>
            <div class="btn-green" id="btn_reset"><span name="submit" class="inner"><%=Map.Database.Localizer.Translate("Reset Password")%></span></div>
            <input type="submit" id="submit" style="display:none;" />
        </div>               
   </form>
   <div class="pwResetConfirm username"></div>

<script type="text/javascript">

    var submitElement = $('[name="submit"]');
    submitElement.click(function () {
        $('#submit').click();
    });

    $(document).ready(function () {

        $('input[type="submit"]').click(function () {
            var form = $('form');
            if (Spry.Widget.Form.validate(form[0]))
                resetPassword($(this));
        });
    });
    $(function () {
        $("form").keypress(function (e) {
            if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                $('#submit').click();
                return false;
            } else {
                return true;
            }
        });
    });
    function resetPassword(button) {
        $("body").css("cursor", "wait");
        button.attr('disabled', true);
        var user = "userName=" + $('[name="username"]').val();
        $.post($('form').attr('action'), user,
            function (error) {
                button.attr('disabled', false);
                var err = error;
                if (error == 'success') {
                    $('div.error').hide();
                }
                else if (error == 'error') {
                    $('div.error').hide();
                }
                else if (error.error == 'success') {//
                    $('div.error').hide();
                    $('div.pwResetForm').hide();
                    $('div.pwResetConfirm').show();
                    $('div.pwResetConfirm').html(''+error.message+'');
                    //window.location = error.url;
                }
                else {
                    $('div.error').html(error.message);
                    $('div.error').show();
                }
                button.attr('disabled', false);
                $("body").css("cursor", "default");

            });
    }
</script>

</asp:Content>