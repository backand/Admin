<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<System.Data.DataView>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json"%>

<%  try{
        string guid = Model.Table.ExtendedProperties["guid"].ToString();
        Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()];
        string viewSafety = view.Name + "_safety";
        bool safetyMode = false;
        if (Map.Session[viewSafety] != null){
        safetyMode = (bool)Map.Session[viewSafety];
        }
        Random rnd = new Random(DateTime.Now.Millisecond);
        
        string url = Url.Action("ERD", view.Controller, new { appName = Maps.GetCurrentAppName(), rnd=rnd.Next() });
        bool configChanged=Map.IsConfigChanged;
        if (configChanged)
        {
            ERDHelper erdHElper = new ERDHelper();
            string s;
            erdHElper.CreateERDFile(Map.Database, true, out s);
        }   
            
                
    %>
   <table cellspacing="0" cellpadding="0" width="100%" class="toolbar">
    <tr class='rowcommands'>
        <td colspan="*" class="tablecommand" valign="top">
        <span class="tablecommands float">
        <a href="#" onclick="Refresh('<%=guid %>');return false" title="<%= view.GetActionTooltipDescription(Durados.ToolbarActionNames.REFRESH)%>"><%=view.GetActionTooltipTitle(Durados.ToolbarActionNames.REFRESH)%></a>&nbsp;
       </span>
        </td>
         <!-- View Mode -->  
         <%
         string onclick = "Durados.DisplayType.changeDisplayType('" + guid + "', '{0}');return false;";
        %>
       <% if (view.EnableDashboardDisplay) { %>
    <td class="tablecommand" style="width:80px;padding-right:10px;border-left:0">
     <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/grid.png" %>" class="dashboard_img float" onclick="<%=string.Format(onclick, "Table") %>";return false" title="<%=Map.Database.Localizer.Translate("Grid View")%>" />
     <span class="float">&nbsp;|&nbsp;</span>
     <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/dashboard.png" %>" class="dashboard_img float disabled" onclick="return false" title="<%=Map.Database.Localizer.Translate("Summary")%>" />
    </td>
       <% } %> </tr></table>
   
  
 <iframe id="ERDframe" name="ERDframe" frameborder="0" scrolling="auto" height="50px" width="100%" src="<%=url %>"   marginwidth="0" marginheight="0" onload="iframeLoaded(this)"></iframe>
    
    <%   } catch(Exception exception){ %>
    <span><%= "$$error start$$ " + exception.Message + " $$error end$$"%></span>
<%} %>

<script type="text/javascript" >
    iframeLoaded = function () {
        var offset = $("#ERDframe").offset();
        var winHeight = $(window).height();
        if (offset)
        $("#ERDframe").height(winHeight - offset.top - 10);
    }
    $(window).resize(iframeLoaded);
</script>