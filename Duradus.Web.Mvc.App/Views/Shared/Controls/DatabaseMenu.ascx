<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Durados.Menu>" %>
<% if (Model!=null && Model.Views!= null){ %>
<% List<Durados.UrlLink> links = new List<Durados.UrlLink>(); %>
<% foreach (Durados.Web.Mvc.View view in Model.Views) {%>
    <% if (view != null && view.IsVisible() && !view.HideInMenu)
       { %>
        <%
           string action = Url.Action(view.IndexAction, view.Controller, new { viewName = view.Name });
           string title = view.GetLocalizedDisplayName();
           int order = view.Order;

           links.Add(new Durados.UrlLink() { Order = order, Title = title, Url = action });
        %>

        
    <%} %>
<%} %>

<% foreach (Durados.Page page in Model.Pages) {%>
    <% if (page != null)
       { %>
        <%
           string action = Url.Action("Page", "Home", new { pageId = page.ID });

           string target = string.Empty;

           if (page.PageType == Durados.PageType.External)
           {
               string[] urlVal = page.ExternalNewPage.Split('|');
               if (urlVal.Length == 3)
               {
                   action = urlVal[2];

               }
               if (!string.IsNullOrEmpty(page.Target))
                   target = "target='" + page.Target + "'";
               else if (page.NewTab)
                   target = "target='_blank'";
           }
           else if (page.PageType == Durados.PageType.ReportingServices)
           {
               //action = "/Reports/RdlcWebForm.aspx?ReportName=" + page.ReportName + "&ReportDisplayName=" + page.ReportDisplayName;
               action = Url.Action("RdlcReport", "Durados", new { reportName = page.ReportName, reportDisplayName = page.ReportDisplayName });
           }
           
           string title = page.Title;
           int order = page.Order;

           links.Add(new Durados.UrlLink() { Order = order, Title = title, Url = action, Target = target });
        %>

        
    <%} %>
<%} %>

<% foreach (Durados.UrlLink link in Model.UrlLinks.Values) {%>
    <% links.Add(link); %>
<%} %>


<% foreach (Durados.UrlLink link in links.OrderBy(l=>l.Order)) {%>
        <li><a <%=link.Target %> href='<%=link.Url %>'><span><%= link.Title%></span></a>
        </li>
<%} %>
<%} %>