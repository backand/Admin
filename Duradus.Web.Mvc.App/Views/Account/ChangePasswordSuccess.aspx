<%@ Page Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" MasterPageFile="~/Views/Account/AccountMaster.Master" %>
<%@ Import Namespace="Durados.Localization" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
Change Password
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="signInHeader">
    <%=Map.Database.Localizer.Translate("Change Password")%>
</div>
            
             <%    
                bool isForgotPassword=false;
                string guid = ViewData["guid"]==null?string.Empty:ViewData["guid"].ToString();
                string isReset = ViewData["reset"]==null?string.Empty : ViewData["reset"].ToString(); 
                if (!string.IsNullOrEmpty(guid) && !string.IsNullOrEmpty(isReset))
                {
                      isForgotPassword=true;
                 }
               %>
           <div class="username"> <%=Map.Database.Localizer.Translate("Your password has been changed successfully.")%></div>

           <div class="btn-green" style="margin-left:274px;margin-top:25px;width:80px;"><span name="submit" class="inner"><%=Map.Database.Localizer.Translate("Home")%></span></div>

 <script type="text/javascript">
    var submitElement = $('[name="submit"]');
    submitElement.click(function () {
        window.location.href = "/";
    });
 </script>
</asp:Content>


