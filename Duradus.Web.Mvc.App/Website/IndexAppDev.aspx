<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <section>
  <div class="site-width">
    <div class="info-container">
      <div class="info-heading">App Developers</div>
      <div class="info-subheader"><%=Durados.Database.ShortProductName %> is a Content Management Service for web and mobile applications</div>
      <div class="info-subheader-thin"><%=Durados.Database.ShortProductName %> lets mobile and web app companies focus on their main objective by eliminating the need to code a CMS</div>
      <div class="info-text"><%=Durados.Database.ShortProductName %>’s automatically analyzes the database scheme and content characteristics of any application. A scalable, customized CMS is generated, equipped with rich functionality within a secure environment. The generated CMS can be configured further to adopt additional requirements, without the need of coding.</div>
      <div class="info-text">The <%=Durados.Database.ShortProductName %> CMS presents an HTML5 user interface compatible with all major browsers and mobile devices. It also features powerful data entry and workflow capabilities, as well as a rich UI. The business layer of the CMS offers a sophisticated analytics dashboard, email campaign toolkit and document generation module. The <%=Durados.Database.ShortProductName %> CMS assures limitless capabilities by providing quick integration with third party services like Microsoft Reporting Services, Salesforce.com and WIX web builder.</div>
    </div>
    <div class="side-item-container">
      <div class="side-item tutorial">Video Demo</div>
      <div class="side-item demo">Working Demo</div>
      <div class="side-item try">TRY</div>
    </div>
  </div>
</section>
</asp:Content>


