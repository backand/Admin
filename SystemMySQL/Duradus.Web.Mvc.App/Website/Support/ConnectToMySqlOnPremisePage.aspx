<%@ Page Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"
    CodeBehind="ConnectToMySqlOnPremisePage.aspx.cs" Inherits="Durados.Web.Mvc.App.Website.Support.ConnectToMySqlOnPremisePage" %>

<%@ Register TagPrefix="My" TagName="MenuControl" Src="../Menu.ascx" %>
<%@ Register Src="~/Website/Support/ConnectToMySqlOnPremise.ascx" TagPrefix="ucsupport"
    TagName="ConnectToMySqlOnPremise" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <%--<div class="site-width">--%>
    <My:MenuControl runat="server" ID="MenuControl1" MenuSelected="" />
    <div class="rowcontent page404">
        <div class="container">
            <div class="row">
                <div class="col-md-9 text-justify">
                    <ucsupport:ConnectToMySqlOnPremise ID="ConnectToMySqlOnPremise1" runat="server" />
                </div>
            </div>
        </div>
    </div>
    <%--   <div class="side-item-container">
      <div class="side-item tutorial">Video Demo</div>
      <div class="side-item demo">Working Demo</div>
      <div class="side-item try">TRY</div>
    </div>
    --%>
</asp:Content>
