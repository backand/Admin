<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage"%>
<%@ Register TagPrefix="My" TagName="ContactControl" Src="Contact.ascx" %>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="/website/assets/js/rhinoslider-1.05.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#slider').rhinoslider({
                effect: 'fade',
                showTime: 5000,
                effectTime: 950,
                controlsMousewheel: false,
                controlsKeyboard: false,
                controlsPrevNext: false,
                controlsPlayPause: false,
                autoPlay: true,
                showBullets: 'never',
                showControls: 'never'
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
        
        <My:MenuControl runat="server" ID="MenuControl1" MenuSelected=""/>
        <div class="rowshowcase fill">
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <div class="col-md-6">
                            <h2>The Online<br/>Back-office Generator</h2>
                            <p class="text">Connect your database and automatically<br/>generate a fully customized back-office interface</p>
                            <a href="javascript:;" class="btn-yellow">TRY IT FOR <strong>FREE</strong> >></a>
                            <p class="desc">Ready in less than 2 minutes<br/>No credit card required</p>
                        </div>
                        <div class="col-md-6" style="width:auto;">
                            <a class="a_video" href="javascript:;"><span class="openvideo"></span></a>
                        </div>
                    </div>                    
                </div> 
            </div>            
        </div>

        <div class="rowusercounter">
            <div class="container">
                <div class="row">
                    <div class="text-center wrappercounter">
                        <div class="counter">
                            <%=Durados.Web.Mvc.Maps.Instance.GetAppAcount().ToString("00000") %>&nbsp;&nbsp;
                        </div>
                        <div class="desc-users">
                            Consoles and counting...
                        </div>
                        <hr/>
                    </div>
                </div> 
            </div>
        </div>

        <div class="rowfeatured">
            <div class="container">
                <div class="row">
                    <div class="col-md-12 text-center">
                        <ul id="slider" class="nospace list-inline" style="height:129px;">
                            <li>
                                <ul  class="nav-justified list-inline nospace">
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/WagerZone.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/moblin.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/Cocacola.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/hp.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/Fashioholic.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/Doritos.png"/></a></li>
                                </ul>
                            </li>
                            <li>
                                <ul  class="nav-justified list-inline nospace">
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/DragonPlay.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/HaPoalim.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/fibi.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/sandisk.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/Gameffective2.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/aroma.png"/></a></li>
                                </ul>
                            </li>
                            <li>
                                <ul  class="nav-justified list-inline nospace">
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/Diamin.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/GoGetMe.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/PushApps.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/TeximonDriver.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/Zemingo.png"/></a></li>
                                    <li><a href="javascript:;"><img src="/website/assets/images/logo/Shmaim.png"/></a></li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>

        <div class="rowcontent rowcontent-homepage">
            <div class="container">
                <div class="row">
                    <div class="col-md-12 text-center">
                        <ul class="list-inline nospace contentlist">
                            <li><a href="/product#linkdata"><img id="content1" src="/website/assets/images/contents/content-1.jpg" alt=""/></a>
                                <a href="/product#linkdata"><p>sql database c.r.u.d</p></a></li>
                            <li><a href="/product#nocoding"><img id="content2" src="/website/assets/images/contents/content-2.jpg" alt=""/></a>
                                <a href="/product#nocoding"><p>no coding</p></a></li>
                            <li><a href="/product#file"><img id="content3" src="/website/assets/images/contents/content-3.jpg" alt=""/></a>
                                <a href="/product#file"><p>file manage & upload</p></a></li>
                            <li><a href="/product#role"><img id="content4" src="/website/assets/images/contents/content-4.jpg" alt=""/></a>
                                <a href="/product#role"><p>role based security</p></a></li>
                            <li><a href="/product#data"><img id="content5" src="/website/assets/images/contents/content-5.jpg" alt=""/></a>
                                <a href="/product#data"><p>data history</p></a></li>
                            <li><a href="/product#report"><img id="content6" src="/website/assets/images/contents/content-6.jpg" alt=""/></a>
                                <a href="/product#report"><p>reports & charts</p></a></li>
                            <li><a href="/product#personal"><img id="content7" src="/website/assets/images/contents/content-7.jpg" alt=""/></a>
                                <a href="/product#personal"><p>personal interface</p></a></li>
                            <li><a href="/product#linksecured"><img id="content8" src="/website/assets/images/contents/content-8.jpg" alt=""/></a>
                                <a href="/product#linksecured"><p>secured environment</p></a></li>
                        </ul>
                        <br/>
                        <a id="a_tryforfree" href="javascript:;" class="btn-yellow btn-yellow-short">TRY FOR <strong>FREE</strong> >></a>
                    </div>
                </div> 
            </div>
        </div>

        <div id="contactdlg">
            <div class="dialog-header">
                <div class="title">
                    Contact Us
                </div>
                <div class="sub-title">
                    Submit your request
                </div>
            </div>
            <div class="trynow-main">
                <My:ContactControl runat="server" ID="ContactControl" />
            </div>
        </div>
</asp:Content>