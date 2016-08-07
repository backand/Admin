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
                        <h3>VP Marketing</h3>
                        <h4>Job Description: </h4>
                        <div>We are seeking a vibrant, creative and analytical marketing professional to make it easy for technical decision makers to understand what we uniquely provide and motivate them to take action. This is both a hands-on and strategic role.</div>
                        <h4>Main Responsibilities:</h4>
                        <ul>      
                            <li> Develop company and product positioning that clearly explains and differentiates Back& and communicates the product’s value proposition</li>
                            <li> Identify marketing funnel gaps and develop process solutions</li>
                            <li> Develop and execute B2B marketing campaigns to improve engagement, conversion, and revenue goals</li>
                            <li> Identify and manage conferences and trade show attendance</li>
                            <li> Build marketing database and optimize CRM</li>
                            <li> Source and implement appropriate automated marketing tools</li>
                            <li> Ensure proper data sharing and reporting throughout the company</li>
                            <li> Create and execute email marketing campaigns to general new quality leads and nurture existing leads </li>
                            <li> Create high-impact high impact marketing materials such as website content, datasheets, blog posts, whitepapers, case studies, sales presentations, ROI calculators, videos, etc…</li>
                            <li> Provide thought leadership and content placement that drives viral sharing in the desired markets and generates high quality traffic back to Back&</li>
                            <li> Proactively stay ahead of next generation B2B marketing best practices, and target market trends and changes</li>
                        </ul>
                        <h4>Skills/Qualifications:</h4>
                        <ul>
                            <li> Minimum 7-10 years of B2B SaaS experience</li>
                            <li> Excellent written and spoken English – other languages helpful</li> 
                            <li> Ability to thrive in a highly entrepreneurial, fast-changing, and collaborative environment</li>
                            <li> Expert knowledge of marketing operations best practices</li>
                            <li> Understanding of Cloud. Mobile App, and Database technologies</li>
                            <li> Ability to collect, correlate, and analyze data with accurate results generating recommended actions</li>
                            <li> Excellent communication skills</li>
                        </ul>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
