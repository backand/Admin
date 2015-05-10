<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>The View <%= ViewData["viewName"] %> was deleted </h2>
    <p>
    Either remove this page or click <a href="/Admin/Index/View">here</a> to add this view again
    </p>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headCSS" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
</asp:Content>
