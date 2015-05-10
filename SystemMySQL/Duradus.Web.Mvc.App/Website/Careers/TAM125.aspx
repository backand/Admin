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
                        <h3>Technical Account Manager</h3>
                        <h4>Job Description: </h4>
                        <div>Back&'s platform is becoming increasingly popular for web & mobile developers. We need to guide them in getting the most out of the platform across their entire organization. We're looking to hire a Technical Account Manager to foster the technical success and overall happiness of our customers.</div>
                        <h4>Main Responsibilities:</h4>
                        <ul>      
                            <li>Getting new customers quickly up to speed on Back&, ensuring that they get the most out of the platform</li>
                            <li>Maintaining a high retention rate of customers you manage</li>
                            <li>Building strong relationships with developers and business contacts</li>
                            <li>Showing customers how Back&'s deployment model can save time, money & support innovation</li>
                            <li>Championing technical & business side issues for customers</li>
                        </ul>
                        <h4>Skills/Qualifications:</h4>
                        <ul>
                            <li>4+ years experience with database development & designing preferably SQL Server and MySQL</li>
                            <li>Customer facing experience</li>
                            <li>Engaging with customers to make them successful</li>
                            <li>Exposure to coding languages like C#, JavaScript and Node.js</li>
                            <li>BS/MS in computer science or equivalent work experience</li>
                        </ul>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
