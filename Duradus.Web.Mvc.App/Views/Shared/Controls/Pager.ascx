<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataTable>" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<%--<% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.TableName]; %>
--%><% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.ExtendedProperties["viewName"].ToString()]; %>
<% int rowCount = Convert.ToInt32(ViewData["rowCount"]); %>
<% string guid = Model.ExtendedProperties["guid"].ToString(); %>
<% string pageCount = PagerHelper.GetPageCount(view, rowCount, guid).ToString(); %>
    <div class='gridPager-div'>
    <div class="totalRows">
                <span><%=Map.Database.Localizer.Translate("Total Rows:")%></span>&nbsp;<span style="font-weight:bold;" id='<%=guid %>rowCount'><%=rowCount.ToString()%></span>
            </div>
            <table d_role='pager' class='gridPager' cellpadding="0" cellspacing="0">
                <tr>
                    <% if (!Durados.Web.Infrastructure.General.IsMobile())
                    { %>
                    <td style="width:60px;">&nbsp;</td>
                    <%} %>
                    
                    <% if (!Durados.Web.Infrastructure.General.IsMobile())
                    { %>
                        <td><span><%=Map.Database.Localizer.Translate("Show:")%>&nbsp;</span></td>
                        <%string rowstitle = (!Durados.Web.Infrastructure.General.IsMobile()) ? Map.Database.Localizer.Translate("rows per page") : ""; %>
                        <td class="ddlpageSize"><%=Html.DropDownList("pageSize", PagerHelper.GetPageSizeSelectList(view, guid), new { onchange = "ChangePageSize('" + guid + "', this, '" + view.Name + "', '" + view.GetChangePageSizeUrl() + "')", id = guid + "pageSize" })%>&nbsp;&nbsp;<span><%=rowstitle%></span></td>
                    <%} %>
                    <% if (!Durados.Web.Infrastructure.General.IsMobile())
                    { %>
                    <td width="30%">&nbsp;&nbsp;&nbsp;</td>
                    <%} %>
                    <td>
                        <%=Ajax.GoToFirstPage(view, rowCount, guid)%>
                    </td>
                    <td>
                        <%=Ajax.GoToPrevPage(view, rowCount, guid)%>
                    </td>
                    <td id='<%=guid %>currentPage'>
                        <span><%=Map.Database.Localizer.Translate("Page")%>&nbsp;</span>
                    </td>
                    <td class="curPageInput">
                     <% if (!Durados.Web.Infrastructure.General.IsMobile())
                        { %>
                        <input id='<%=guid %>currentPageVal' type="text" name="currentPage" value='<%=PagerHelper.GetCurrentPage(view, guid).ToString() %>' onkeydown="if (event.keyCode == $.ui.keyCode.ENTER || event.keyCode == $.ui.keyCode.TAB) MovePage('<%=guid %>', this, <%=PagerHelper.GetCurrentPage(view, guid).ToString() %>, <%=pageCount %>, '<%=view.Name %>', '<%= view.GetIndexUrl()%>')" onblur="MovePage('<%=guid %>', this, <%=PagerHelper.GetCurrentPage(view, guid).ToString() %>, <%=pageCount %>, '<%=view.Name %>', '<%= view.GetIndexUrl()%>')"/>
                    <%}
                        else
                        { %>
                        <span><%=PagerHelper.GetCurrentPage(view, guid).ToString() %></span>
                    <%} %>
                    </td>
                    <td id='<%=guid %>outof' d_last='<%=pageCount %>'>&nbsp;<span><%=Map.Database.Localizer.Translate("of")%>&nbsp;<%=pageCount %>&nbsp;</span>
                    </td>
                    <td>
                        <%=Ajax.GoToNextPage(view, rowCount, guid)%>
                    </td>
                    <td>
                        <%=Ajax.GoToLastPage(view, rowCount, guid)%>
                    </td>
                    <% if (!Durados.Web.Infrastructure.General.IsMobile())
                    { %>
                    <td>
                        &nbsp;&nbsp;&nbsp;
                    </td>
                    <%} %>
                </tr>
            </table>
            
     </div>