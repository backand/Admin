<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
	.column { width: 50%; float: left; padding-bottom: 100px; }
	.portlet { margin: 0 1em 1em 0; }
	.portlet-header { margin: 0.3em; padding-bottom: 4px; padding-left: 0.2em; }
	.portlet-header .ui-icon { float: right; }
	.portlet-content{ padding: 0.4em; }
	.portlet-link A{ color: Blue !important; }
	.ui-sortable-placeholder { border: 1px dotted black; visibility: visible !important; height: 50px !important; }
	.ui-sortable-placeholder * { visibility: hidden; }
	</style>
    <script>
        $(function() {
            Dashboard.run();
        });
	</script>
	<%  if (!Durados.Web.Infrastructure.General.IsMobile())
            Html.RenderPartial("~/Views/Shared/Controls/MainMenu.ascx"); %>
    
    <table cellspacing="0" cellpadding="0" width="100%">
        <tr id='rowtabletitle'>
            <td colspan="*">
               
                &nbsp;
                <span class="tabletitle" ><%=Map.Database.Localizer.Translate("My Dashboard")%></span>
              
            </td>
        </tr>
    </table>
    <br />
    <%  Html.RenderPartial("~/Views/Shared/Controls/Dashboard.ascx"); %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headCSS" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
</asp:Content>
