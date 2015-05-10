<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PlugIn.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
html,body{
width:100%;
height:100%;
margin:0px;
padding:0px;
background-color:transparent;
}
</style>
<%
    
    string url = ViewData["url"].ToString();
    string signOutUrl = ViewData["signOutUrl"].ToString();
 %>
 <script src='<%=ResolveUrl("~/Scripts/jquery.min.js")%>' type="text/javascript"></script>

<script>
//$(document).ready(function () {

    var url = '<%=url %>';
    $.ajax({
        url: "<%=signOutUrl %>",
        contentType: 'application/json; charset=utf-8',
        data: {},
        async: false,
        dataType: 'json',
        cache: false,
        success: function () {
            
        }
    });
//    $('iframe.aaa').attr('src', url);
    //    $('h1').text(url);
    //setTimeout(function () {
        window.location = url;
    //}, 10000);
//    if (document.layers) {
//        document.write('<Layer src="' + url + '" visibility="hide"></Layer>');
//    }
//    else if (document.all || document.getElementById) {
//        document.write('<iframe src="' + url + '" style="visibility:hidden;"></iframe>');
//    }
//    else {
//        location.href = url;
//    }
//});


</script>
</asp:Content>
