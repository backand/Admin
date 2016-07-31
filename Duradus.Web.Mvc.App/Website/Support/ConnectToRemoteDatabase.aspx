<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"
    CodeBehind="~/Website/Support/ConnectToDatabase.aspx.cs" Inherits="Durados.Web.Mvc.App.Website.Support.ConnectToDatabase" %>

<%@ Register TagPrefix="My" TagName="MenuControl" Src="../Menu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        img
        {
            margin: 10px 0 10px 0;
        }
        ul.supportContext a:link
        {
            color: #6a6a6d;
            font-family: Open Sans, Arial, Helvetica, sans-serif;
            font-weight: 400;
            font-size: 16px;
            line-height: 24px;
            margin-bottom: 30px;
        }
        ul.supportContext
        {
            padding-left: 20px;
            font-weight: bold;
        }
        ul.supportContext li
        {
            display: list-item;
            list-style-type: square;
        }
    </style>
    <script type="text/javascript">

        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }
        $(document).ready(function () {
            var urlHash = getUrlVars();
            if (urlHash != null && urlHash["Master"] != null && urlHash["Master"] == "Simple") {
                $('ul.supportContext a').each(function () {
                    var href = $(this).attr('href');
                    href += "?Master=Simple";
                    $(this).attr('href', href);
                });
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <%--<section>--%>
    <My:MenuControl runat="server" ID="MenuControl1" MenuSelected="" />
    <div class="rowcontent page404">
        <div class="container">
            <div class="row">
                <div class="col-md-9 text-justify">
                    <ul class="supportContext">
                        <li><a href="ConnectToMSSQLPage.aspx">Connect to SQL Server instance.</a></li>
                        <li><a href="ConnectToAzurePage.aspx">Connect to Azure database</a></li>
                        <li><a href="ConnectToMySqlOnPremisePage.aspx">Connect to MySQL database on Windows</a></li>
                        <li><a href="ConnectToRDSMySQLPage.aspx">Connect to MySQL on Amazon RDS</a></li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
    <%-- <div class="site-width">
    <div class="info-container">
      <div class="info-heading">Connect to Database</div>
        <div class="info-text"> 
        <ul class="supportContext">
         <li><a href="ConnectToMSSQLPage.aspx">Connect to SQL Server instance.</a></li>
         <li><a href="ConnectToAzurePage.aspx">Connect to Azure database</a></li>
         <li><a href="ConnectToMySqlOnPremisePage.aspx">Connect to MySQL database on Windows</a></li>
         <li><a href="ConnectToRDSMySQLPage.aspx">Connect to MySQL on Amazon RDS</a></li>
        </ul>
        </div>
    </div>
    <div class="side-item-container">
      <div class="side-item tutorial">Video Demo</div>
      <div class="side-item demo">Working Demo</div>
      <div class="side-item try">TRY</div>
    </div>
  </div>--%>
    </section>
</asp:Content>
