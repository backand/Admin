<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"%>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%
      string func = "var g_videoid = ''";
      if(!string.IsNullOrEmpty(Request.QueryString["vid"]))
      {
          func = "var g_videoid = '" + Request.QueryString["vid"] + "';";
      }
       %>
    <script type="text/javascript">
        <%=func %>
        $(document).ready(function () {
            $(".v_box_wrapper").hover(function () {
                $(this).children(".v_video_desc").slideToggle();
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
<%

   string divText = 
    "<div class=\"col-md-4 howtovideo\" id=\"xxxxxxxxxxxxx\">\n"+
    "   <div class=\"v_box_wrapper\">\n"+
    "       <div class=\"v_video_thumb\">\n"+
    "           <img src=\"\" alt=\"Video\" class=\"v_video_thumb_img\"/>\n"+
    "           <img src=\"/website/assets/images/video/v_video_play.png\" alt=\"Play\" class=\"v_video_play\"/>\n"+
    "       </div>\n"+
    "       <div class=\"v_video_title\"></div>\n"+
    "       <div class=\"v_video_desc\">\n"+
    "           <p class=\"v_desc_video_name\"></p>\n"+
    "           <p class=\"v_desc_video_desc\"></p>\n"+
    "           <hr/>\n"+
    "           <p class=\"\">\n"+
    "           <span class=\"v_desc_length\"></span> | <span class=\"v_desc_play\">\n"+
    "               <a href=\"javascript:;\" class=\"\">Play>></a>\n"+
    "           </span>\n"+
    "           </p>\n"+
    "       </div>\n"+
    "   </div>\n"+
    "</div>\n";
    /*
                    <div class="col-md-4 howtovideo" id="WOJkbcEFdKk">
                        <div class="v_box_wrapper">
                            <div class="v_video_thumb">
                                <img src="" alt="Video" class="v_video_thumb_img"/>
                                <img src="/website/assets/images/video/v_video_play.png" alt="Play" class="v_video_play"/>
                            </div>
                            <div class="v_video_title"></div>
                            <div class="v_video_desc">
                                <p class="v_desc_video_name"></p>
                                <p class="v_desc_video_desc"></p>
                                <hr/>
                                <p class="">
                                    <span class="v_desc_length"></span> | <span class="v_desc_play">
                                        <a href="javascript:;" class="">Play>></a>
                                    </span>
                                </p>
                            </div>
                        </div> 
                    </div>
     */
   
     %>
    <My:MenuControl runat="server" ID="MenuControl1" MenuSelected="videos"/>
         <div class="rowcontent pagevideos">
            <div class="container">
                <div class="row">
                    <div class="col-md-12 ">
                        <h1 class="">How To Videos</h1>
                        <br/>
                        <!-- MASTER PAGE CONTENT -->                        
                    </div>                    
                </div>                
                <div class="row">
                    <div class="col-md-12">
                        <p class="v_cat_header">Initial Setup</p>
                    </div>
                </div>
                <div class="row">
<%
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "eSBqt4FbGbc"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "WOJkbcEFdKk"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "mG5uhX72cM4"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "cu49opckIME"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "DjZhipDPbiw"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "2kSC-Cl_8nU"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "0_wy8gTCifw"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "Fx-yzTKI_ws"));
    
%>
                    <div class="col-md-12">
                        <p class="v_cat_header">General Functionality</p>
                    </div>
<%
    
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "8KT1ecbGPeA"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "48p-UjiWytw"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "OgAYlcCMOt4"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "RM2GvPA1DrI"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "_BeDn7tb3MU"));

%>
                    <div class="col-md-12">
                        <p class="v_cat_header">Tables & Columns Configuration</p>
                    </div>
<%
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "bFWqXVQrGJw"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "Re6oxgWNM08"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "ItQAi1JLFKs"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "mS5SZjQTUCk"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "hItivCB4gCA"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "dRmXk6PxYcI"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "rjpPvmGmst8"));
    Response.Write(divText.Replace("xxxxxxxxxxxxx", "Mkm49lse55s"));
    
%>
                    
                </div>
            </div>
        </div>

</asp:Content>
