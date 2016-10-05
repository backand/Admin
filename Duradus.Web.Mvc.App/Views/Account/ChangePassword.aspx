<%@ Page Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" MasterPageFile="~/Views/Account/AccountMaster.Master" %>
<%@ Import Namespace="Durados.Localization" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
Change Password
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="changedragonimg"></div>

<div class="signInHeader">
    <%=Map.Database.Localizer.Translate("Change Password")%>
</div>
            
            <div class="instructions">
               <%= Map.Database.Localizer.Translate("The password is required to be a minimum of ") + Html.Encode(ViewData["PasswordLength"])+Map.Database.Localizer.Translate(" characters in length.")%> 
               <br />
                <span class="error-desc" style="display:none;"><font  style="color:Red;">
                    <%= Html.ValidationSummary(Map.Database.Localizer.Translate("Password change was unsuccessful. Please correct the errors and try again."))%>
                </font>
                    </span>
            </div>
             <%    
                
                string guid = ViewData["guid"] == null ? string.Empty : ViewData["guid"].ToString();
                bool isForgotPassword = !string.IsNullOrEmpty(guid) ? true : false;
                            
               %>
    
    <form >

        <%if (!isForgotPassword)
          { %>
        <div class="password">
            <label for="currentPassword"><%= Map.Database.Localizer.Translate("Current password")%></label>
            <%= Html.Password("currentPassword", string.Empty, new { id = "currentPassword", type = "password" , placeholder="Current Password" })%>
            <font style="color: Red;">
                <%= Html.ValidationMessage("currentPassword")%>
                <%= Html.ValidationMessage("newPassword")%>
            </font>
        </div>
        <%}
          else
          { %>
        <div class="password"><%=Html.Hidden("userSysGuid", guid)%></div>
        <%} %>
        <div class="password">
            <label for="newPassword"><%= Map.Database.Localizer.Translate("New password")%>:</label>
            <%= Html.Password("newPassword", string.Empty, new { id = "newPassword", type = "password" , placeholder="Password" })%>
        </div>
        <div class="password">
            <label for="confirmPassword"><%= Map.Database.Localizer.Translate("Confirm new password")%>:</label>
            <%= Html.Password("confirmPassword", string.Empty, new { id = "Change_confirmPassword", type = "password" , placeholder="Password" })%>
        </div>
        <div class="btn-green" id="btn_changepassword"><span name="submit" class="inner"><%=Map.Database.Localizer.Translate("Reset Password")%></span></div>
        <input style="display: none;" id="submit" type="submit" value="" />
    </form>

 <script type="text/javascript">
    var submitElement = $('[name="submit"]');
    submitElement.click(function () {
        var url = "<%=isForgotPassword?"/Account/ForgotPassword":"/Account/ChangePassword"%>";
        $('span.error-desc').hide();
        var data = { confirmPassword: $('#Change_confirmPassword').val(), newPassword: $('#newPassword').val(),currentPassword: $('#currentPassword').val(),userSysGuid: '<%=guid%>'}
        $.ajax({
            url: url,
            data: data,
            type: "Post",
            async: false,
            dataType: 'json',
            cache: false,
            error: function () {$('.error-desc').html('The server is busy, please try again later.');  },
            success: function (response) {
                if (response.success) {
                    $('form').html('<h3 class="white-text">' + response.message + '<br>You are being redirected to the app...</h3>');
                    $('span.error-desc, .instructions').hide();
                    window.location.replace('/Home/Default');
                }
                else {
                    var message = (!response.message && response.message == '') ? 'The server is busy, please try again later.' : response.message;
                    $('span.error-desc>font').html(message);
                    $('span.error-desc').show();
                }
            }

        });
    });
 </script>
</asp:Content>

