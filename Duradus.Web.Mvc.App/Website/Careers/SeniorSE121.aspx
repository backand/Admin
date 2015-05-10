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
                        <h3>Senior Software Engineer</h3>
                        <h4>Job Description: </h4>
                        <div>The Senior Software Developer is experienced in all aspects of the full software development life cycle as well as design and development of client/server and/or web applications. 
                        This individual must be well rounded and have experience developing software in OO technologies, preferably C# and MySQL, and knowledge of NoSQL databases, preferably MongoDB.</div>
                        <h4>Main Responsibilities:</h4>
                        <ul>      
                            <li>Gathering functional requirements, developing technical specifications, and project & test planning</li>
                            <li>Designing/developing web, software, mobile apps, prototypes, or proofs of concepts (POC’s).</li>
                            <li>Contribute to the design and architecture of the project.</li>
                            <li>Experience with Agile Development, SCRUM, or Extreme Programming methodologies.</li>
                        </ul>
                        <h4>Skills/Qualifications:</h4>
                        <ul>
                            <li>6+ years experience developing web, software, or mobile applications.</li>
                            <li>BS/MS in computer science or equivalent work experience.</li>
                            <li>Strong experience with any of the following Object Oriented Languages (OOD): Java/J2EE, C#, VB.NET, Python, or sometimes C++.</li>
                            <li>Strong experience with database application development preferably MySQL and MongoDB</li>
                            <li>Experience with web services (consuming or creating) with REST or SOAP.</li>
                            <li>Strong understanding of the Software design/architecture process.</li>
                            <li>Experience developing, maintaining, and innovating large scale, consumer facing web or mobile applications.</li>
                            <li>Familiar with the development challenges inherent with highly scalable and available web applications.</li>
                        </ul>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
