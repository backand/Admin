<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"%>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
            //if (!Durados.Web.Mvc.Maps.Instance.GetMap().IsMainMap)
        //if (Durados.Web.Mvc.Maps.Instance.GetAppName() != Durados.Web.Mvc.Maps.DuradosAppName)
                //Response.Redirect("/Home/e404");
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
        <My:MenuControl runat="server" ID="MenuControl1" MenuSelected=""/>
        
        <div class="rowcontent page404">
            <div class="container">
                <div class="row">
                    <div class="col-md-12 text-center">
                        <h1 class="text-left">404 - Page Not Found</h1>
                        <br/>
                        <img class="bg_404" src="/website/assets/images/bg/bg_404.gif"/>
                        <br/>
                        <p class="desc_404">If you have any questions or concerns please contacts us at: <a href="mailto:support@backand.com">support@backand.com</a></p>
                        <a href="javascript:;" class="btn-yellow btn-yellow-short">TRY FOR <strong>FREE</strong> >></a>
                    </div>
                </div> 
            </div>
        </div>
</asp:Content>