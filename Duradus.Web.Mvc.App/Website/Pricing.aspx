<%@ Page Title="Back& Pricing" Language="C#" MasterPageFile="~/Website/Main.master" AutoEventWireup="true" %>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">

    <My:MenuControl runat="server" ID="MenuControl1" MenuSelected="pricing"/>
    <div id="main-contain">
        <div class="wrapper-mid">
            <div class="info-heading">
                Pricing: 14 Day Free Trial
            </div>

            <div class="pricing-table">
                <div class="grid_3 pricing-cell1">
                    <h5>
                        Starter<br/>
                        <font style="font-size:16px">$49</font><span>/month</span>
                    </h5>
                    <div>4 Users</div>
                    <div>1 Production Console</div>
                    <div>3 Test Consoles</div>
                    <div>Support by Email + Phone</div>
                    <div>All Standard Features Included</div>
                    <div>Reports & Dashboard</div>
                </div>
                <div class="grid_3 pricing-cell2">
                    <h5>
                        Plus<br/>
                        <font style="font-size:16px">$149</font><span>/month</span>
                    </h5>
                    <div>15 Users</div>
                    <div>1 Production Console</div>
                    <div>10 Test Consoles</div>
                    <div>Support by Email + Phone</div>
                    <div>All Standard Features Included</div>
                    <div>Reports & Dashboard</div>
                    <div>Workflow Rules</div>
                    <div>Web Services Calls</div>

                </div>
                <div class="grid_3 pricing-cell3">
                    <h5>
                        Pro<br/>
                        <font style="font-size:16px">$249</font><span>/month</span>
                    </h5>
                    <div>50 Users+</div>
                    <div>2 Production Console</div>
                    <div>Unlimited Test Consoles</div>
                    <div>Support by Email + Phone</div>
                    <div>All Standard Features Included</div>
                    <div>Reports & Dashboard</div>
                    <div>Workflow Rules</div>
                    <div>Web Services Calls</div>
                    <div>Look & Feel Customization</div>
                    <div>Your Own Domain</div>
                    <div>Integration with MailChimp</div>
                </div>
                <div class="grid_3 pricing-cell4">
                    <div class="inner">
                        <h5><a href="javascrpt:;" class="a_signup">Try For</a><br/>
                            <span class="bold"><a href="javascrpt:;" class="a_signup">FREE >></a></span><br/><span>Choose a plan later</span>
                        </h5>
                    </div>
                </div>
                <div class="clear"></div>
            </div>
            <div class="freeplan">
                <div class="left">
                    <h5>FOREVER <b>FREE</b> PLAN</h5>
                    <div class="slogun">FFL, as in Free For Life, no credit card required</div><br/>
                    <ul>
                        <li>1 Console</li>
                        <li>1 User</li>
                        <li>Email Support</li>
                        <li>All Standard Features</li>
                    </ul>
                </div>
                <div class="right">
                    <a class="a_signup" href="javascript:;">signup</a>
                </div>
                <div class="clear"></div>
            </div>
            <div id="tips">
                <div class="box">
                    <h5>HOW LONG IS THE TRIAL PERIOD?</h5>
                    <p>We offer a 14-day free trial, enabling you to explore Back&’s feature rich functionality without any restrictions.</p>
                </div>
                <div class="box">
                    <h5>IS THERE A SETUP FEE?</h5>
                    <p>None whatsoever! Back& initial setup takes only 2 minutes.</p>
                </div>
                <div class="box last">
                    <h5>CAN I CHANGE MY PLAN?</h5>
                    <p>Absolutely! You can upgrade, downgrade or cancel your subscription at any time.</p>
                </div>
                <div class="clear"></div>
            </div>
            <div id="contact-qoute">
                <a href="/contact?o=5">contact us</a>
            </div>
        </div>
    </div>

</asp:Content>
