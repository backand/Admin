<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PlugIn.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Design</h2>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<%
    
    string url = ViewData["url"].ToString();
    string signOutUrl = ViewData["signOutUrl"].ToString();
 %>
 <script src='<%=ResolveUrl("~/Scripts/jquery.min.js")%>' type="text/javascript"></script>

<script>
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

    window.location = '<%=url %>';
</script>
</asp:Content>
