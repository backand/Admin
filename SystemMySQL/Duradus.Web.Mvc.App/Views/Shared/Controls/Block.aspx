<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<System.Data.DataView>" %>
<%@ Import Namespace="System.Data"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <% if (!Durados.Web.Mvc.Maps.Skin)
       { %>

       
    <%bool displayHeader = (this.Request.QueryString["menu"] != "off"); %>
    <% string classMargin = displayHeader ? string.Empty : " refer-bar-nomargin"; %>
    
    <div class="refer-bar<%=classMargin %>">
    
                
        <a id="rowtabletitleSpan"  title="Dictionary" d_Dn='Dictionary' colname="Fields_Parent" >Dictionary</a>
        <a  href="#" onClick="" class="sec"></a>
                 
    </div>
<%}
       else
       { %>
    
    <%  Html.RenderPartial("~/Views/Shared/Controls/MainMenu.ascx"); %>
    <table cellspacing="0" cellpadding="0" width="100%" class="dictionary-table">
        <tr id='rowtabletitle'>
            <td colspan="*">
                <span class="tabletitle"><%=Map.Database.Localizer.Translate("Blocks")%></span>
            </td>
        </tr>
    </table>
    <%} %>
     <%  Html.RenderPartial("~/Views/Shared/Controls/Block.ascx",Model); %>
  <%--  <table cellspacing="0" cellpadding="0" width="100%" class="dictionary-table">
        <!-- COLUMNS HEADER -->
        <tr id="rowtitle">
            <td style="font-weight:bold">Tags:</td>
        </tr>
        <!-- DATA -->
        <% foreach (DataRowView row in Model)
           { %>
            <tr>
                <td>
                    <%=row.Row["Tag"].ToString()%> 
                </td>
            </tr>
            <%} %>
    </table>--%>
    
    <table cellspacing="0" cellpadding="0" width="100%">
        <tr>
            <td colspan="*" align="center">
                <b><%=Map.Database.Localizer.Translate("Copyright 2010 by ")%><a href="http://wwww.devitout.com"><%=Map.Database.Localizer.Translate("Dev.IT.Out")%></a>-&nbsp;<a href="<%=Durados.Web.Mvc.Maps.GetMainAppUrl() %>"><%=Map.Database.Localizer.Translate("Durados 2.0")%></a></b>
            </td>
        </tr>
    </table>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
