<%@ Page Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Workflow.Graph>" %>
<%@ Import Namespace="Durados" %>
<%
    string CssPath = ResolveUrl("~/Content/");
    string JsPath = ResolveUrl("~/Scripts/");
    string GraphState = ViewData["GraphState"].ToString();
    string guid = Map.Version;
        
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>WorkFlow</title>  
    <script type="text/javascript">
        var gVD = '<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath()%>';
    </script>  
    <link type="text/css" href='<%= CssPath + "jsPlumb.css" + "?id=" + guid %>?rnd=1' rel="stylesheet" />
    <script src='<%=ResolveUrl("~/Scripts/general-min-1.0.0.js") + "?id=" + guid%>' type="text/javascript"></script>   
    <script type="text/javascript" src='<%= JsPath + "jquery.jsPlumb-1.3.3-all-min.js" + "?id=" + guid %>'></script>
<script type="text/javascript">
    //TODO - Add try catch!!!
    <%= "var graphData = " + Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize<Durados.Workflow.Graph>(Model) + ";var graphStates=[" + GraphState + "];var graphState;var always = '" + Map.Database.Localizer.Translate("Always Available") + "';" %>
    
    
    if (graphStates && graphStates.length) {graphState = graphStates[0];} else {
        graphState={};
    }
</script>		
<script type="text/javascript" src='<%= JsPath + "json.js" + "?id=" + guid %>'></script>
     
</head>
<body onunload="jsPlumb.unload();">
<% if (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin"))
   { %>
<button class="graphSaveButton" onclick="saveGraphState()"><%=Map.Database.Localizer.Translate("Save")%></button>
<% } %>
<div id="graphDiv"></div>
</body>
</html>
