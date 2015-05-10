<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"%>
<%@ Register TagPrefix="My" TagName="ContactControl" Src="Contact.ascx" %>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <My:MenuControl runat="server" ID="MenuControl1" MenuSelected="contact"/>

        <div class="rowcontent page404">
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <h1 class="text-left">Contact Us</h1>
                        <My:ContactControl runat="server" ID="ContactControl" />
                        <div class="clearfix"></div>
                        <br/>
                    </div>
                </div> 
            </div>
        </div>
</asp:Content>
