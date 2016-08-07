<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"%>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="Menu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script type="text/javascript">
            $(function () {
                $("#accordion").accordion();
            });
        </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
        <My:MenuControl runat="server" ID="MenuControl1" MenuSelected="support"/>
        <div class="rowcontent page404">
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <h1>FAQ</h1>
                        <br/>
                        <div class="exp-faq">
                            <div id="accordion">
                                <h3><span class="q">Q</span> How do I contact Back&?</h3>
                                <div>
                                    <p>
                                    <span class="a">A&nbsp;</span>
                                    To contact us please:<br />
                                        <ul>
                                        <li>Call toll free: U.S. 1-1888-259-9242 | E.U. 44-800-048-8730</li>
                                        <li>Email <a href="mailto:support@backand.com">support@backand.com</a></li>
                                        <li>Complete the form <a href="/contact">here</a></li>
                                        </ul>
                                    </p>
                                </div>
                                <h3><span class="q">Q</span> Will my database be secure?</h3>
                                <div>
                                    <p>
                                        <span class="a">A&nbsp;</span>Absolutely! Our entire platform is SSL encrypted. Not only are all database connections secured by default (if server supported), passwords and user information is encrypted as well. We do recommend when you initially use Back& you connect to a non-production environment.
                                    </p>
                                </div>
                                <h3><span class="q">Q</span> How do I access my database?</h3>
                                <div>
                                    <p>
                                        <span class="a">A&nbsp;</span>Simply register with Back& and follow our easy to use wizard. For more information specific to your type of database, please go <a href="./Website/Support/ConnectToRemoteDatabase.aspx">here</a>
                                    </p>
                                    
                                </div>
                                <h3><span class="q">Q</span> Do you store any of my database data?</h3>
                                <div>
                                    <p>
                                        <span class="a">A&nbsp;</span>We do not. Back& is an advanced, back-end to your database. We do not store any of your data locally. However, if you wish to enable the History feature to any View, data will then be stored on our secure database server. This functionality is only enabled however if you decide to do so.
                                    </p>
                                </div>
                                <h3><span class="q">Q</span> What’s next for Back&?</h3>
                                <div>
                                    <p>
                                        <span class="a">A&nbsp;</span>We have some amazing features in the pipeline for release in the near future- Cementing Back&’s position as the best on line back-office platform.
                                    </p>
                                   
                                </div>
                                <h3><span class="q">Q</span> How much does Back& cost?</h3>
                                <div>
                                    <p>
                                        <span class="a">A&nbsp;</span>Our free trial runs for 14 days, and our Free For Life plan covers 1 console for 1 user. For a complete breakdown on our amazing price plans go <a href="/pricing">here</a>.
                                    </p>
                                </div>
                                <h3><span class="q">Q</span> What database types do you support?</h3>
                                <div>
                                    <p>
                                        <span class="a">A&nbsp;</span>We currently support: MySQL, PostgreSQL, Oracle, SQL Server and SQL Server Azure. In the near future we will be enabling MongoDB as well.
                                    </p>
                                </div>
                                <h3><span class="q">Q</span> How does Back& work?</h3>
                                <div>
                                    <p>
                                        <span class="a">A&nbsp;</span>Back& connects to your database and performs a complex analysis of the tables, views and relationships. It then generates a complete, user friendly interface. It's easier experienced than described so sign up for your free trial!
                                    </p>
                                </div>
                            </div>
                        </div>
                        <p class="text-center"><br/><br/><br/><br/><br/><a href="/contact" class="btn-yellow btn-yellow-short btn-text">Contact us for<br/> more info >></a></p>
                    </div>
                </div> 
            </div>
        </div>
</asp:Content>