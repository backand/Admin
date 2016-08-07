<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"%>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="Menu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <My:MenuControl runat="server" ID="MenuControl1" MenuSelected=""/>
        <div class="rowcontent page404">
            <div class="container">
                <div class="row">
                    <div class="col-md-9 text-justify">
                      <h1>About Back&</h1>
                      <h3>Back& is the premier online Back-office Service for Web and Mobile applications</h3>
                      <p>A Back& HTML 5 Back Office is not only compatible with all major browsers and mobile devices; it can be easily configured for additional requirements without any further coding. In addition to a flexible UI Back& features powerful data entry and workflow capabilities, backed by a sophisticated analytics dashboard, document generation module and an email campaign toolkit. With quick integration optionality with third party services like Microsoft Reporting Services, Salesforce.com and Wix Web Builder the potential is limitless. Back& is the complete Back Office solution you deserve.</p>
                      <h1>Management Team</h1>
                      <p><b>Gal Frenkel</b>, Co-founder and CEO: Gal comes from a combined business and technology background. Prior to joining Back& for 3 Years Gal was the CEO of Intoatv, a startup in the home entertainment industry. He has several years of experience leading a variety of businesses as partner and CEO. His technical background includes establishing and managing the Orange Israel DWH & BI marketing department.</p>
                      <p><b>Itay Herskovits</b>, Co-founder and CTO: With 17 years of software & product development experience, Itay is responsible for the technical delivery, operation and development of enterprise applications and products. He managed IT P&L Centers with budgets of up to $5M and served as VP Product Management at Mobix.com, Director of Engineering at Ask.com (US / California), and Dev Manager at Esurance.com (US / California).</p>
                      <p><b>Relly Rivlin</b>, Co-founder and VP R&D: Relly has 20 years experience in development. Prior to joining Back& he was the VP R&D for 7 years in a startup call Notal Vision and before that as development manager in enterprise companies. Back& core technology emerged by Relly’s M.Sc studies in database visualization and ORM.</p>
                    </div>
            </div>
        </div>
    </div>
</asp:Content>


