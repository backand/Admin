<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="Durados.Web.Mvc.App.Website.Menu" %>
<script language="C#" runat="server">
    public String MenuSelected;
</script>

<div class="rowmenu">
    <div class="container">
        <div class="row">
            <div class="col-md-12 ">
                <div class="logo pull-left">
                    <a href="/">
                        <img src="/website/assets/images/logo.png" alt="Homepage" />
                    </a>
                </div>
                <div class="wrappermenu pull-left">
                    <div id="asaaslickheader"></div>
                    <ul class="list-unstyled list-inline mainmenu nospace" id="asaheadermenu">
                        <li<%if(MenuSelected=="product"){%> class="current"<%} %>><a href="/product">product</a></li>
                        <li<%if(MenuSelected=="pricing"){%> class="current"<%} %>><a href="/pricing">pricing</a></li>
                        <li<%if(MenuSelected=="support"){%> class="current"<%} %>><a href="http://blog.backand.com/questions">support</a></li>
                        <li<%if(MenuSelected=="contact"){%> class="current"<%} %>><a href="/contact">contact</a></li>
                        <li<%if(MenuSelected=="blog"){%> class="current"<%} %>><a href="http://blog.backand.com">blog</a></li>
                    </ul>
                </div>   
                <a href="javascript:;" class="btn-yellow btn-yellow-short pull-left">TRY FOR <strong>FREE</strong> >></a>
            </div>
        </div> 
    </div>
</div>