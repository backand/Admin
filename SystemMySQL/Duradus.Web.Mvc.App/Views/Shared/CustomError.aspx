<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Web.Mvc.Infrastructure.CustomError>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class='error-page'>
    <center>
    <br />
    <h1><%=Model.Title %></h1>
    <br />
    <br />
        <h2>
            <%=Model.Message%>
        </h2>
        <% 
            try
            {
                string userRole = Map.Database.GetUserRole();
                if (userRole == "Admin" || userRole == "Developer")
                {
                    if (Model.Log.Trace != null && (Model.Log.Trace.Contains("SqlException") || Model.Log.Trace.Contains("MySql")) && Model.ViewName != null)
                    { %>
                    <h4>A possible change in schema. <a href="#" onclick="syncError('<%=Request.Url.PathAndQuery%>', '<%= Model.ViewName%>')">Sync</a> is recommended.</h4>
         <%         }
                }
            } catch{}%>
    <br />

    <%--<a href="#" onclick="history.go(-1);">back</a>
    <%--
    <a href="/Home/Default" >Home</a>&nbsp;&nbsp;&nbsp;&nbsp;
    <a href="/Home/Default?workspaceId=1&menuId=10001" >Admin</a>
    --%>
    <br/>
    <img class="bg_404" style="padding-left: 85px;" src="/Content/images/Backand-error.png" alt="error image"/>
    <br/>
    </center>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headCSS" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
</asp:Content>
