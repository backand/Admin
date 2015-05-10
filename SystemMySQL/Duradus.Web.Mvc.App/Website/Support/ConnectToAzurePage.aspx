<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"
    CodeBehind="~/Website/Support/ConnectToAzurePage.aspx.cs" Inherits="Durados.Web.Mvc.App.Website.Support.ConnectToAzurePage" %>

<%@ Register TagPrefix="My" TagName="MenuControl" Src="../Menu.ascx" %>
<%@ Register Src="~/Website/Support/ConnectToAzure.ascx" TagName="ConnectToAzure"
    TagPrefix="ucsupport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <My:MenuControl runat="server" ID="MenuControl1" MenuSelected="" />
    <div class="rowcontent page404">
        <div class="container">
            <div class="row">
                <div class="col-md-9 text-justify">
                    <ucsupport:ConnectToAzure runat="server" />
                </div>
            </div>
        </div>
    </div>
    <%--  <div class="side-item-container">
      <div class="side-item tutorial">Video Demo</div>
      <div class="side-item demo">Working Demo</div>
      <div class="side-item try">TRY</div>
    </div>--%>
</asp:Content>
