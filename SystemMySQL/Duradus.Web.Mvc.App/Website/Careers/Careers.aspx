<%@ Page Title="" Language="C#" MasterPageFile="~/Website/Main.master" AutoEventWireup="true"%>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="../Menu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
<My:MenuControl runat="server" ID="MenuControl1" MenuSelected=""/>
        <div class="rowcontent page404">
            <div class="container">
                <div class="row">
                    <div class="col-md-9 text-justify">
                    <h3>Business & Sales</h3>
                    <ul>      
                        <li><div><a href="/website/careers/VPMarketing.aspx" >VP Marketing</a></div></li>
                        <li><div><a href="/website/careers/InsideSalesManager.aspx" >Inside sales Manager</a></div></li>
                    </ul>
                    <h3>Engineering</h3>
                    <ul>      
                        <li><div><a href="/website/careers/nodejs120.aspx" >Front End / Node.js Developer</a></div></li>
                        <li><div><a href="/website/careers/seniorSE121.aspx" >Senior Software Engineer</a></div></li>
                        <li><div><a href="/website/careers/tam125.aspx" >Technical Account Manager</a></div></li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
