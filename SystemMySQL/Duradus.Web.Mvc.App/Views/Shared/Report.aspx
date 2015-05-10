<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Web.Mvc.UI.Item>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<asp:Content ID="Content3" ContentPlaceHolderID="headCSS" runat="server">
    <%=((Durados.Web.Mvc.View)Database.Views[Model.ViewName]).GetStyleSheets() %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%=((Durados.Web.Mvc.View)Database.Views[Model.ViewName]).GetScripts() %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% try { %>
    <% string guid = Model.Guid; %>
    
    <% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.ViewName]; %>
    
    <%=Html.Hidden("a", Durados.Web.Mvc.Infrastructure.General.GetRootPath(), new { id = "GetRootPath" })%>
    <%=Html.Hidden("a", Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(new Durados.Web.Mvc.UI.Json.Translator(Map.Database)), new { id = "translator" })%>
    
    <% ViewData["cameFromIndex"] = true; %>
    <script type="text/javascript">

        var rootPath = $('#GetRootPath').val();
        var translator = Sys.Serialization.JavaScriptSerializer.deserialize($('#translator').val());
//        initDataTableView('<%=guid %>');
        $(document).ready(function() {
            initDataTableView('<%=guid %>');
        });

        document.title = '<%= Map.Database.Localizer.Translate(Map.Database.SiteInfo == null ? Durados.Web.Mvc.Map.Database.DisplayName : (string.IsNullOrEmpty(Durados.Web.Mvc.Map.Database.SiteInfo.Product) ? Durados.Web.Mvc.Map.Database.DisplayName : Durados.Web.Mvc.Map.Database.SiteInfo.Product)) + " - " + Map.Database.Localizer.Translate(view.DisplayName) %>';
    
    </script>

    
    <%  if(!Durados.Web.Infrastructure.General.IsMobile())
            Html.RenderPartial("~/Views/Shared/Controls/MainMenu.ascx"); %>
    <%--<table cellspacing="0" cellpadding="0" width="100%">
        <tr id='rowtabletitle'>
            <td colspan="*">
                <% string description = Map.Database.Localizer.Translate(view.Description); %>
                <span class="tabletitle" alt='<%= description %>' title='<%= description %>'><%=Map.Database.Localizer.Translate(view.DisplayName)%></span>&nbsp;&nbsp;&nbsp;<span class="tabletitlefilter"><%=view.GetPageFilter(guid).ToFriendlyString()%></span>
            </td>
        </tr>
    </table>--%>
    <table cellspacing="0" cellpadding="0" width="100%">
        <tr id='rowtabletitle'>
            <td colspan="*">
                <%  
        Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];    
                %>
                <% string description = view.GetTooltip(); %>
                <% 
        string displayName = tableViewer.GetDisplayName(view, guid);
        string path = (ViewData["path"] ?? string.Empty).ToString();                   
                %>
                &nbsp;
                <%if (!(view.Database is Durados.Web.Mvc.Config.Database) && (Map.Database.IsInRole("Developer") || Map.Database.IsInRole("Admin")))
                  { %>
                  <% string href = Url.Action("Index", "Admin", new { viewName = "View", isMainPage = true, ID = view.ViewDataSetID, __Fields_Parent__ = view.Description }); %>
                <a href="<%=href %>" colname="Fields_Parent" pk="<%= view.ViewDataSetID%>"><span class="tabletitle" ><%=path %></span>
                <span id="rowtabletitleSpan" class="tabletitle" title='<%= description %>' d_Dn='<%=displayName%>'><%=displayName%></span>&nbsp;&nbsp;&nbsp;<span class="tabletitlefilter"><%=view.GetPageFilter(guid).ToFriendlyString()%></span>
                </a>
                <%}
                  else
                  { %>
                 <span class="tabletitle" ><%=path %></span><span id="rowtabletitleSpan" class="tabletitle" title='<%= description %>' d_Dn='<%=displayName%>'><%=displayName%></span>&nbsp;&nbsp;&nbsp;<span class="tabletitlefilter"><%=view.GetPageFilter(guid).ToFriendlyString()%></span>
              
                <%} %>
            </td>
        </tr>
    </table>
    <div id="mainAppDiv">
    <%  Html.RenderPartial("~/Views/Shared/Controls/ReportView.ascx", Model); %>
    </div>
    <!-- Context Menu -->
    <%  Html.RenderPartial("~/Views/Shared/Controls/ContextMenu.ascx", new Durados.Web.Mvc.UI.ClientParams() { View = view, Guid = guid, MainPage = false }); %>

    <div style="display:none" id="Div1" ></div>

    <%} catch(Exception exception){ %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/ErrorMessage.ascx", exception.Message); %>
    <%} %>
</asp:Content>

