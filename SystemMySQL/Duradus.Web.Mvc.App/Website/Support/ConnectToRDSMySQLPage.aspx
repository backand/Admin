﻿<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"
    CodeBehind="~/Website/Support/ConnectToRDSMySQLPage.aspx.cs" Inherits="Durados.Web.Mvc.App.Website.Support.ConnectToRDSMySQLPage" %>

<%@ Register TagPrefix="My" TagName="MenuControl" Src="../Menu.ascx" %>
<%@ Register Src="~/Website/Support/ConnectToRDSMySQL.ascx" TagName="ConnectToRDSMySQL"
    TagPrefix="ucsupport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%-- <style type="text/css">
      img{margin:10px 0 10px 0}
    </style>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <%--<div class="site-width">--%>
    <My:MenuControl runat="server" ID="MenuControl1" MenuSelected="" />
    <div class="rowcontent page404">
        <div class="container">
            <div class="row">
                <div class="col-md-9 text-justify">
                    <ucsupport:ConnectToRDSMySQL runat="server" />
                </div>
            </div>
        </div>
    </div>
    <%-- <div class="side-item-container">
      <div class="side-item tutorial">Video Demo</div>
      <div class="side-item demo">Working Demo</div>
      <div class="side-item try">TRY</div>
    </div>--%>
</asp:Content>