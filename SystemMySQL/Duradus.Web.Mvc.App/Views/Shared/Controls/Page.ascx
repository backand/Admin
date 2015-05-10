<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Page>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<% if (Model.PageType == Durados.PageType.Content)
       { %>
        <%
           string content = Model.Content;    
        %>
        <%=content%>
    <%}
       else
       { %>

       <%
           string url = string.Empty;
           string scrolling = Model.Scroll ? "yes" : "no";
           string width = Model.Width == 0 ? "100%" : Model.Width + "px";
           string height = Model.Height == 0 ? "500px" : Model.Height + "px";
           string[] urlVal = new string[0];
           if (Model.PageType == PageType.IFrame)
           {
               if (Model.ExternalPage != null)
               {
                   urlVal = Model.ExternalPage.Split('|');
               }
           }
           else if (Model.PageType == PageType.External)
           {
               if (Model.ExternalNewPage != null)
               {
                   urlVal = Model.ExternalNewPage.Split('|');
               }
           }
           
            if (urlVal.Length == 3)
            {
                url = urlVal[2];

            }
            else if (Model.PageType == PageType.ReportingServices)
            {
                url = Url.Action("RdlcReport", "Durados", new { reportName = Model.ReportName, reportDisplayName = Model.ReportDisplayName, menu = "off" });
            }
               
           
           url = Map.Database.GetEncryptedUrl(url);
           
        %>
        <iframe src="<%=url %>" scrolling="<%=scrolling %>" width="<%=width %>" height="<%=height %>">
        
        </iframe>
    <%} %>


        <br />
    <% string currentRole = Map.Database.GetUserRole(); %>
    <% if (currentRole == "Admin" || currentRole == "Developer")
       {%>
       
       <br />
       <a href="#" onclick="Page.edit(<%=Model.ID %>); return false;"><%= Map.Database.Localizer.Translate("Click here to edit the content of this page")%></a>


       <%  Html.RenderPartial("~/Views/Shared/Controls/Url.ascx"); %>
    

    <%} %>