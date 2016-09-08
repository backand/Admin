<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Durados.Web.Mvc.UI.Json.UploadInfo>" %>
<%
    string s = Model.Path + "|" + Model.FileName + "|";
   //string s =  ResolveUrl(Model.Path) + "?" + Model.FileName;
   //string json = "'{path:" + ResolveUrl(Model.Path) + ",filename:" + Model.FileName+"}'";
 %>
<%= s %>

<head id="Head1" runat="server">
    <script type="text/javascript">
        document.domain = '<%=Durados.Web.Mvc.Maps.Host %>';
        //alert(document.domain);
    </script>
</head>