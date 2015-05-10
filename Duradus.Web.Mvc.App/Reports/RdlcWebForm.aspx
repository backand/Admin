<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RdlcWebForm.aspx.cs" Inherits="Durados.Web.Mvc.App.Reports.RdlcWebForm" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Report - <%=Request.QueryString["ReportDisplayName"]%></title>
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE8" />
<style type="text/css">
html,body,form {height:100%;}
body   {margin:0;overflow: auto}
</style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" enablepagemethods="true" enablescriptglobalization="true" scriptmode="release"></asp:ScriptManager>   
        <rsweb:ReportViewer  Width="100%" Height="100%" ID="ReportViewer1"  runat="server" ProcessingMode="Remote" ShowCredentialPrompts="true" AsyncRendering="True" SizeToReportContent="false" ></rsweb:ReportViewer>
    </form>
</body>

</html>
