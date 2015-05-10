<%@ Page Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=8"/>
    <title><%=Durados.Web.Mvc.Maps.GetMainAppName() %> 2.0 - Login</title>
    <style type="text/css">
    body
    {
        font-family: Calibri;
        font-size: 14px;
        background-image: url(/Uploads/Specifics/SanDisk/Excel/sd-login-bg2.jpg);
        background-position:center;
        background-repeat:no-repeat;
    }
    .logo
    {
        position:absolute;
    }
    .outerFrame
    {
        border: solid 1px black;
        padding:20px;
    }
    .welcome
    {
        background-color:rgb(83,141,213);
        font-size: 46px;
        color:White;
        width:430px;
        height:170px;
        font-weight:bold;
        font-style:italic;
        padding-top: 30px;
        border: solid 1px black;
        margin-top:25px;
        margin-bottom:18px;
    }
    .version
    {
        font-size: 14px;
        font-weight:normal;
    }
    .quote
    {
        font-style:italic;
        font-size: 16px;
        margin-bottom:40px;
    }
    .instructions
    {
        font-style:normal;
    }
    p.username label, p.password label
    {
        text-align:left;
        width: 100px;
        float:left;
        font-style:normal;
    }
    label.remember 
    {
        text-align:left;
        width: 100px;
        float:left;
        font-style:normal;
    }
    label.rememberMe
    {
        text-align:right;
        width: 150px;
        float:none;
        font-style:normal;
    }
    input
    {
    }
    p.username input, p.password input
    {
        width:150px;
        border-style: inset;
        margin-left:3px;
   }
    .login
    {
        width:260px;
        margin-left: auto ;
        margin-right: auto ;
    }
    .durados a:link img,
    .durados a:visited img,
    .durados a:hover img
    {
        text-decoration: none;
        border:solid 0 black;
    }
    .durados td span
    {
        font-size:14px;
    }
    </style>
</head>
<body dir='<%=Map.Database.Localizer.Direction %>'>
    <div class="outerFrame">
        <span class="logo">
            <% if (!string.IsNullOrEmpty(Map.Database.SiteInfo.LogoHref))
               {%>
            <a href='<%= Map.Database.SiteInfo.LogoHref%>'>
            <%} %>
            <img src='<%= ResolveUrl("~" + Map.Database.UploadFolder + "logo-sandisk-login.png") %>' alt='<%= Map.Database.SiteInfo.Company %>' />
            <% if (!string.IsNullOrEmpty(Map.Database.SiteInfo.LogoHref))
               {%>
            </a>
            <%} %>
        </span>
        <center>
            <div class="welcome">
                <span>
                        Welcome to
                </span>
                <br />
                <span><%= Map.Database.SiteInfo.Product%></span>
                <% if (Map.Database.SiteInfo.ShowVersion){ %>
                <span class="version"><br /><br />version&nbsp;<%=Map.Version%>&nbsp;<%= Map.Database.SiteInfo.Version%></span>
                <%} %>
            </div>
        </center>
        <center>
            <div class="quote">
                 <span>Empowered by the CTO Organization</span>
           </div>
        </center>
        
        <center>
            <div class="instructions">
                 <span>Please enter your windows username and password</span>
           </div>
            
            <font style="color:Red;"><%= Html.ValidationSummary(Map.Database.Localizer.Translate("Login was unsuccessful. Please correct the errors and try again.")) %></font>
        </center>    

           <% using (Html.BeginForm()) { %>
                <div class="login">
                        <p class="username">
                            <label for="username"><%=Map.Database.Localizer.Translate("Username")%>&nbsp;</label>
                            <font style="color:Red;">
                            <%= Html.TextBox("username") %>
                            </font>
                        </p>
                        <p class="password">
                            <label for="password"><%=Map.Database.Localizer.Translate("Password")%>&nbsp;</label>
                            <font style="color:Red;">
                            <%= Html.Password("password") %>
                            </font>
                        </p>
                        <p class="remember">
                            <label class="remember">&nbsp;</label>
                            <%= Html.CheckBox("rememberMe") %> <label class="rememberMe" for="rememberMe"><%=Map.Database.Localizer.Translate("Remember me")%>?</label>
                        </p>
                        <p>
                            <label class="remember">&nbsp;</label>
                            <input type="submit" value="<%=Map.Database.Localizer.Translate("Log On")%>" />
                        </p>
                </div>
                <center>
                    <%= Server.HtmlDecode(Html.GetHtml("login")) %>
                </center>
                <br />
            <% } %>
            <center>
                <div  class="durados">
                <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                <td valign="middle">
                <span>Powered by&nbsp;&nbsp;</span>
                </td>
                <td valign="middle">
                    <a href="<%=Durados.Web.Mvc.Maps.GetMainAppUrl() %>"> <img alt="<%=Durados.Web.Mvc.Maps.GetMainAppName() %>" title="<%=Durados.Web.Mvc.Maps.GetMainAppName() %>" src="/Content/Images/<%=Durados.Web.Mvc.Maps.GetMainAppName() %>)_logo.png" /></a>
                </td>
                </tr>
                </table>
                </div>
            </center>
    </div>
    
</body>
</html>
