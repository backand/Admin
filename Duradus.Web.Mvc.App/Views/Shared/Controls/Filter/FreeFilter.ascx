<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.View>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%
    string guid = ViewData["guid"] as string;
    string textSearch = Map.Database.Localizer.Translate("Search on text...");
    string search = ViewData["search"] as string;
%>
<a class="search">
    <input dvalue="<%=textSearch %>" class="search_text serch-field watermark" type="text"
        value="<%=search %>" onkeypress="handleEnterFilter(this, event,'<%=guid %>',this);"
        title="<%=Map.Database.Localizer.Translate("Search on all text fields")%>" />
    <%--<input type="text" class="serch-field watermark" value="Search Table..." />--%>
    <span class="icon" onclick="FilterForm.ApplySearch(false, '<%=guid %>', this, '<%=textSearch %>')"
        title="<%=Model.GetActionTooltipDescription(Durados.ToolbarActionNames.SEARCH)%>">
    </span></a><a onclick="FilterForm.ApplyClear(false, '<%=guid %>', this)" title="<%=Model.GetActionTooltipDescription(Durados.ToolbarActionNames.CLEAR)%>">
        <%=Map.Database.Localizer.Translate("Clear")%></a>
<!------------------Free Search-------------------->
<%--<input class="search_text serch-field" type="text" value="<%=search %>" onkeypress="handleEnterFilter(this, event,'<%=guid %>',this);"
    onblur="if(this.value==''){this.value='<%=textSearch %>';}" onclick="if(this.value=='<%=textSearch %>') this.value = '';"
    title="<%=Map.Database.Localizer.Translate("Search on all text fields")%>" />--%>
<!------------------Apply Free Search-------------------->
<%--<img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/img/search.png" %>"
    width="16px" height="16px" align="absmiddle" onclick="FilterForm.ApplySearch(false, '<%=guid %>', this, '<%=textSearch %>')"
    title="<%=Model.GetActionTooltipDescription(Durados.ToolbarActionNames.SEARCH)%>" />--%>
<!------------------Clear Free Search-------------------->
<%--<img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/cancelsearch.gif" %>"
    width="16px" height="16px" align="absmiddle" onclick="FilterForm.ApplyClear(false, '<%=guid %>', this)"
    title="<%=Model.GetActionTooltipDescription(Durados.ToolbarActionNames.CLEAR)%>" />--%>
