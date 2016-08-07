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
                        <h3>Front End / Node.js Developer</h3>
                        <h4>Job Description: </h4>
                        <div>We're seeking a developer with a passion for UI/UX, great HTML/CSS/JavaScript skills and that can own the entire Node.js development. The ideal profile combines deep experience of MEAN Stack with a strong computer science background.<br />
                         You must be able to demonstrate prior experience on web sites you’re proud to show off for their polished UI, appropriate use of Javascript, and optimized backends.</div>
                        <h4>You should be passionate about:</h4>
                        <ul>      
                            <li>User experience and goal-directed design</li>
                            <li>Current Node.js/jQuery/AngularJS best practices</li>
                            <li>Style, presentation, and design</li>
                        </ul>
                        <h4>You should be very strong on all of the following:</h4>
                        <ul>
                            <li>Node.js</li>
                            <li>HTML/CSS – not a web designer, but extremely good at implementing markup</li>
                            <li>JavaScript – including jQuery/AngularJS and cross-browser compatibility</li>
                            <li>Database RDMS and NoSQL</li>
                            <li>Performance optimization, SQL tuning, caching strategies</li>
                            <li>Ability to own projects end-to-end</li>
                        </ul>
                        <h4>Bonus skills:</h4>
                        <ul>
                            <li>Ruby on Rails</li>
                            <li>MongoDB</li>
                            <li>HTML5 fanciness (e.g. websockets, canvas)</li>
                            <li>Amazon Web Services (AWS e.g. S3, EC2)</li>
                            <li>Having founded your own startup</li>
                        </ul>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
