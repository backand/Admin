<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Web.Mvc.UI.RdlcReport>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<asp:Content ID="Content3" ContentPlaceHolderID="headCSS" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    

    
    <%  if (!Durados.Web.Infrastructure.General.IsMobile() && (Durados.Web.Mvc.Maps.Skin))
            Html.RenderPartial("~/Views/Shared/Controls/MainMenu.ascx"); %>
    <%--<table cellspacing="0" cellpadding="0" width="100%">
        <tr id='rowtabletitle'>
            <td colspan="*">
                <% string description = Map.Database.Localizer.Translate(view.Description); %>
                <span class="tabletitle" alt='<%= description %>' title='<%= description %>'><%=Map.Database.Localizer.Translate(view.DisplayName)%></span>&nbsp;&nbsp;&nbsp;<span class="tabletitlefilter"><%=view.GetPageFilter(guid).ToFriendlyString()%></span>
            </td>
        </tr>
    </table>
    
    <%  Html.RenderPartial("~/Views/Shared/Controls/ReportView.ascx", Model); %>
    
    <div style="display:none" id="Div1" ></div>
 
     

    --%>
    <%--<iframe width=1000 height=600 src="../../Reports/RdlcWebForm.aspx?ReportName=<%=Model.ReportName%>&ReportDisplayName=<%=Model.ReportDisplayName %>">
        
    </iframe>--%>
    <%--<iframe id="reportsIframe" src="externalpage.htm" scrolling="no" marginwidth="0" marginheight="0" frameborder="0" vspace="0" hspace="0" style="overflow:visible; width:100%; display:none; width: 500px; height: 200px"></iframe>--%>
    <div id="reportsDiv">
    <iframe id="reportsIframe" name="reportsIframe" frameborder="0" scrolling="auto" height="96%" width="100%" src="../../Reports/RdlcWebForm.aspx?ReportName=<%=Model.ReportName%>&ReportDisplayName=<%=Model.ReportDisplayName %>" marginwidth="0" marginheight="0" onload="iframeLoaded(this)"></iframe></div>
   
    <%--<iframe id="reportsIframe" frameborder="0" scrolling="auto" width="100%"  onload="adjustMyFrameHeight();" src="../../Reports/RdlcWebForm.aspx?ReportName=<%=Model.ReportName%>&ReportDisplayName=<%=Model.ReportDisplayName %>" marginwidth="0" marginheight="0" frameborder="0" vspace="0" hspace="0"></iframe>MemRequestReport--%>
    
 	<style type="text/css"> 
        /*.ui-resizable-helper { border: 75px solid #EFEFEF; margin: -75px; } */
	</style>    
    <script type="text/javascript">

        $(document).ready(function () {
            //{ handles: 'ne, se' }
            showProgress();
            //            $('#ProgressionDiv').show();
            var offsetPos = $('#reportsDiv').position();
            var h = $(window).height() - offsetPos.top - 10;
            $('#reportsIframe').height(h);
            //$("#reportsDiv").css('display','inline');

        });

        function resizeIframe() {
            var offsetPos = $('#reportsIframe').position();
            var h = $(window).height() - offsetPos.top - 20;
            $('#reportsIframe').height(h);
        }

        function iframeLoaded(el) {
            hideProgress();
//            $('#ProgressionDiv').hide();
        }
    
    
        document.title = '<%= Map.Database.Localizer.Translate(Map.Database.SiteInfo == null ? Map.Database.DisplayName : (string.IsNullOrEmpty(Map.Database.SiteInfo.Product) ? Map.Database.DisplayName : Map.Database.SiteInfo.Product)) + " - " + Model.ReportDisplayName %>';
        
        
        
    </script>
</asp:Content>

