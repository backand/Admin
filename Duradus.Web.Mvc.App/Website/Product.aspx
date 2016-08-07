<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"%>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
        <My:MenuControl runat="server" ID="MenuControl1" MenuSelected="product"/>
        <div class="rowcontent page404">
            <div class="container">
                <div class="row">                    
                    <div class="col-md-12">
                        <h1 class="text-left">The Product</h1>
                        <br/>
                        <div class="pull-left col-product">
                            <div id="linkdata" class="box-product">
                                <div class="col-product-icon pull-left"><img src="/website/assets/images/product/db.jpg"></div>
                                <div class="col-product-content pull-left">
                                    <h3  class="nospace">DATABASE EXPLORER & C.R.U.D</h3>
                                    <p>Back& quickly and automatically generates a feature rich Back Office admin interface, without the need to change your App.  By providing your connection string, the Back& interface presents your application's database tables or views and their relationships. Each View can be easily configured to display as a single page, tab or a dialog.</p>
                                </div>
                                <div class="clearfix"></div>
                            </div>  
                            <div class="clearfix"></div>
                            <br>
                            <img src="/website/assets/images/product/row1-1.jpg" style="margin-left: 50px; margin-top: 80px;">
                            <div class="clearfix"></div>
                            <div id="nocoding" class="box-product">
                                <div class="col-product-icon pull-left">
                                    <img alt="ZERO CODING" src="/website/assets/images/product/nocoding.jpg" /></div>
                                <div class="col-product-content pull-left">
                                    <h3 class="nospace">ZERO CODING</h3>
                                    <p>Back&'s no coding approach empowers both developers and non-developers alike to customize their Back Office with outstanding results. Our powerful Admin functionality enables View and Field behavior control through an extremely user friendly dialog interface.</p>
                                </div>
                                <div class="clearfix"></div>
                            </div>  
                            <div class="clearfix"></div>
                            <img src="/website/assets/images/product/row1-2.jpg" style="float: right;">
                            <div class="clearfix"></div>
                            <div id="role" class="box-product">
                                <div class="col-product-icon pull-left">
                                    <img src="/website/assets/images/product/role.jpg" alt="role" /></div>
                                <div class="col-product-content pull-left">
                                    <h3 class="nospace">ROLE BASED SECURITY</h3>
                                    <p>Back& employs strong role-based security, where roles are defined by the security settings applied to each object such as the Workspace, View, Fields and Menu items. Users are each assigned a role that define the data and operations that are available to them within the interface.</p>
                                </div>
                                <div class="clearfix"></div>
                            </div>  
                            <div class="clearfix"></div>
                            <img src="/website/assets/images/product/row1-3.jpg" style="margin-left:140px; margin-top: 40px;"/>
                            <div class="clearfix"></div>
                            <div id="personal" class="box-product">
                                <div class="col-product-icon pull-left">
                                    <img alt="PERSONAL INTERFACE" src="/website/assets/images/product/personal.jpg" /></div>
                                <div class="col-product-content pull-left">
                                    <h3 class="nospace">PERSONAL INTERFACE</h3>
                                    <p>Your Back& Back Office application can be easily customized to your needs. From uploading your Logo to a complete customization through a .CSS file, the look and feel of your interface is entirely your choice.</p>
                                </div>
                                <div class="clearfix"></div>
                            </div>  
                            <div class="clearfix"></div>
                        </div>
                        <div class="pull-left col-product">
                            <img src="/website/assets/images/product/row2-1.jpg" style="margin-top:-20px;"/>
                            <div class="clearfix"></div>
                            <div id="report" class="box-product">
                                <div class="col-product-icon pull-left"><img alt="Reports" src="/website/assets/images/product/reports.jpg"/></div>
                                <div class="col-product-content pull-left">
                                    <h3 class="nospace">REPORTS & CHARTS</h3>
                                    <p>Back& Report View enables browsing by table data, displaying selected sets of columns, applying rules to filter selected records and using sorting and grouping options. Chart functionality presents data graphically, enabling horizontal or vertical bar diagrams, linear and pie charts.</p>
                                </div>
                                <div class="clearfix"></div>
                            </div> 
                            <div class="clearfix"></div>
                            <img src="/website/assets/images/product/row2-2.jpg" style="margin-left:350px; margin-bottom:100px;"/>
                            <div id="file" class="box-product">
                                <div class="col-product-icon pull-left"><img alt="file upload" src="/website/assets/images/product/file.jpg"/></div>
                                <div class="col-product-content pull-left">
                                    <h3 class="nospace">MEDIA MANAGEMENT & UPLOAD</h3>
                                    <p>Back& allows users to also manage images and documents that are located on Amazon S3, Azure Storage or the App website. Our Gateway management system enables images, media and documents to be uploaded and displayed on each View.</p>
                                </div>
                                <div class="clearfix"></div>
                            </div> 
                            <div class="clearfix"></div>
                            <img src="/website/assets/images/product/row2-3.jpg" style="margin-left:100px;"/>
                            <div class="clearfix"></div>
                            <div id="data" class="box-product">
                                <div class="col-product-icon pull-left"><img alt="DATA HISTORY" src="/website/assets/images/product/data.jpg"/></div>
                                <div class="col-product-content pull-left">
                                    <h3 class="nospace">DATA HISTORY</h3>
                                    <p>All data changes are logged in the Back& history table with the following values: User, Previous Value, New Value, Date and Time of the change. In the event of an accident or disaster, Back& Recovery quickly enables data history support. Furthermore, Administrators can easily enable email notifications for any changes in database data as well.</p>
                                </div>
                                <div class="clearfix"></div>
                            </div> 
                            <div class="clearfix"></div>
                            <img src="/website/assets/images/product/row2-4.jpg" style=""/>
                            <div class="clearfix"></div>
                            <div id="linksecured" class="box-product">
                                <div class="col-product-icon pull-left"><img src="/website/assets/images/product/secured.jpg"/></div>
                                <div id="linksecured" class="col-product-content pull-left">
                                    <h3  class="nospace">SECURED ENVIRONMENT</h3>
                                    <p>VeriSign approved, SSL protected and full passwords encryption, Back& is completely secured for your data protection.</p>
                                </div>
                                <div class="clearfix"></div>
                            </div> 
                        </div>
                    </div>
                    <div class="clearfix"></div>
                    <p class="text-center"><br/>
                    <br/>
                    <br/>
                    <br/><a href="javascript:;" class="btn-yellow btn-yellow-short">TRY FOR <strong>FREE</strong> >></a></p>
                </div> 
            </div>
        </div>
</asp:Content>