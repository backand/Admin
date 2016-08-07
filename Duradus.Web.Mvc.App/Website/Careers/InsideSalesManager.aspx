<%@ Page Title="" Language="C#" MasterPageFile="~/Website/Main.master" AutoEventWireup="true"%>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="../Menu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <section>

<My:MenuControl runat="server" ID="MenuControl1" MenuSelected=""/>
        <div class="rowcontent page404">
            <div class="container">
                <div class="row">
                    <div class="col-md-9 text-justify">
                        <h3>Inside sales Manager</h3>
                        <h4>Job Description: </h4>
                        <div>The Sales manager is responsible for working with leads from an international marketplace. This is an Inside Sales position.</div>
                        <h4>Main Responsibilities:</h4>
                        <ul>      
                            <li> Qualification and follow up for inbound leads.</li>
                            <li>  Generation of online leads.</li>
                            <li> Pre-sales online presentations and the actual closing of the sale.</li>
                            <li> The position requires individuals with experience in international digital/web sales.</li>
                        </ul>
                        <h4>Skills/Qualifications:</h4>
                        <ul>
                            <li>Proven technical background in DB or software development</li>
                            <li>Experience and understanding of the online space and web based technology</li>
                            <li>Minimum of 1 years experience in digital/web sales</li>
                            <li>Proven track record of consistently meeting or exceeding targets</li>
                            <li>Experience in SaaS product sales - an advantage</li>
                            <li>Experience with international sales</li>
                            <li>Self -motivated, creative, able to work both autonomously as well as in a group dynamic</li>
                            <li>Education: BA or BS required</li>
                            <li>Fluent phone and email skills in English (mother tongue) - a MUST</li>
                        </ul>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
