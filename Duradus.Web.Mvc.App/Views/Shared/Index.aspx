<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<System.Data.DataView>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

  
<asp:Content ID="Content3" ContentPlaceHolderID="headCSS" runat="server">
<% if (this.Request.QueryString["menu"] == "off")
   { %>
   <style type="text/css">
   .refer-bar{margin-left:0 !important; display:none}
   </style>
<%} %>
    
   
    <%
        Durados.Web.Mvc.SkinType skin = ((Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()]).Skin;

        //bool useSkin = false;
        string virtualPath = "~/Content/Themes/" + skin.ToString() + ".css";
            
        //if (skin != Durados.Web.Mvc.SkinType.DefaultSkin)
        //{
        //    if (System.IO.File.Exists(Server.MapPath(virtualPath)))
        //    {
        //        useSkin = true;
        //    }
        //}
     %>

     <%--<%if (useSkin){ %>--%>

        <%="<link role=\"skin\" rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(virtualPath) + "\" />"%>
         <%=((Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()]).GetStyleSheets()%>
     <%--<%} %>--%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        document.domain = '<%=Durados.Web.Mvc.Maps.Domain %>';
    </script>
    <%=((Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()]).GetScripts()%>
    <%if (System.Configuration.ConfigurationManager.AppSettings["profile"].ToLower() == "true")
      {%>
    <!--Debug Profile-->
    <script type="text/javascript" src="http://yui.yahooapis.com/combo?2.9.0/build/yahoo/yahoo-min.js&2.9.0/build/profiler/profiler-min.js"></script>
    <%} %>    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% try
       { %>
    <%  bool profile = (System.Configuration.ConfigurationManager.AppSettings["profile"].ToLower() == "true");
        int viewSettingsWidth = 500;
        string guid = Model.Table.ExtendedProperties["guid"].ToString(); %>
    
    <% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.ExtendedProperties["viewName"].ToString()];%>
    
    <%=Html.Hidden("a", Durados.Web.Mvc.Infrastructure.General.GetRootPath(), new { id = "GetRootPath" })%>
    <%=Html.Hidden("a", Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(new Durados.Web.Mvc.UI.Json.Translator(Map.Database)), new { id = "translator" })%>
    
    <% ViewData["cameFromIndex"] = true; ViewData["guid"] = guid; %>

    <% bool displaySettings = !MenuHelper.HideSettings(); // (bool)(Map.Session["MenuDisplayState"] ?? false); %>

    <%  
        Durados.Web.Mvc.UI.TableViewer tableViewer = ViewData["TableViewer"] == null ? new Durados.Web.Mvc.UI.TableViewer() : (Durados.Web.Mvc.UI.TableViewer)ViewData["TableViewer"];    
                %>
                <% string description = view.GetTooltip(); %>
                <% 
                    HttpCookie cookie = new HttpCookie("d_lv");
                    cookie.Value = view.Name;
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    Response.Cookies.Add(cookie);

                    string displayName = tableViewer.GetDisplayName(view, guid) + "  " + (view.Database.IsConfig ? string.Empty : view.GetPageFilter(guid).ToFriendlyString());
                    string path = (ViewData["path"] ?? string.Empty).ToString();                   
                %>
                
    <%
           bool settingsAble = !(view.Database is Durados.Web.Mvc.Config.Database) && view.IsAdmin() && !Durados.Web.Infrastructure.General.IsMobile();
           string viewPk = string.Empty;
           string viewSettingsUrl = "''";
           int settingsAbleJS = settingsAble && !Map.IsMainMap && !view.SystemView  ? 1 : 0;
    %>
  
    <% if (!Durados.Web.Mvc.Maps.Skin){ %>

    <%bool displayHeader = (this.Request.QueryString["menu"] != "off"); %>
    <% string classMargin = displayHeader ? string.Empty : " refer-bar-nomargin"; %>
    <% string style = Durados.Web.Infrastructure.General.IsMobile() ? "" : "style='display:none;'"; %>
    <div <%=style %> class="refer-bar<%=classMargin %>">
    <% string href = "#";%>
    <% string onclick = "";%>

       <%if (Durados.Web.Infrastructure.General.IsMobile())
      { %>
   
        <%
            viewPk = (new Durados.DataAccess.ConfigAccess()).GetViewPK(view.Name, Map.GetConfigDatabase().ConnectionString);
            viewSettingsUrl = "'/Admin/Item/View?pk=" + viewPk + "&menu=off'";
            Workspace workspace = MenuHelper.GetCurrentWorkspace(view.Name);
        %>
        <% onclick = "mobileMenu('Workspaces')"; %> 
            
      <a class="mobile-menu-icon" onclick="<%=onclick %>" title="<%= description %>" d_Dn='<%=displayName%>' href="<%=href %>" colname="Fields_Parent" pk="<%= view.ViewDataSetID%>">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</a>

      <div class="mobileMenuContent" style="display:none;">
        <% Html.RenderPartial("~/Views/Shared/Controls/MobileMenu.ascx"); %>
        </div>
        <%} %>
    

        <%if (settingsAble)
            { %>
            <%
                viewPk = (new Durados.DataAccess.ConfigAccess()).GetViewPK(view.Name, Map.GetConfigDatabase().ConnectionString);
                viewSettingsUrl = "'/Admin/Item/View?pk=" + viewPk + "&menu=off'";
                if (Request.QueryString["settings"] == "fields")
                    viewSettingsUrl = "'/Home/IndexWithButtons?url=" + Uri.EscapeUriString(Map.Url) + "%2FAdmin%2FIndex%2FField%3FFields%3D" + viewPk + "%26__Fields_Children__%3Dxx%26BackTo%3DView%26children%3Dtrue%26menu%3Doff'";    
                
            %>

            <% onclick = "toggleViewProperties('" + guid + "', isViewPropertiesFloat(), " + viewSettingsWidth + ", " + viewSettingsUrl + ",'" + view.DisplayName + "')"; %> 
            <%--<%  href = Url.Action("Index", "Admin", new { viewName = "View", isMainPage = true, ID = view.ViewDataSetID, __Fields_Parent__ = view.Description }); %>
            --%>
            <span class="desc-icon grid" onclick="<%=onclick %>"></span>
        <%} %>
                <%--<a href="<%=href %>" colname="Fields_Parent" pk="<%= view.ViewDataSetID%>"><span class="tabletitle" ><%=path %></span>
                <span id="rowtabletitleSpan" class="tabletitle" title="<%= description %>" d_Dn='<%=displayName%>'><%=displayName%></span>&nbsp;&nbsp;&nbsp;<span class="tabletitlefilter"><%=view.GetPageFilter(guid).ToFriendlyString()%></span>
                </a>--%>
                <%--<% string style = Durados.Web.Infrastructure.General.IsMobile() ? "" : "style='display:none;'"; %>--%>
        <a id="rowtabletitleSpan" onclick="<%=onclick %>" title="<%= description %>" d_Dn='<%=displayName%>' href="<%=href %>" colname="Fields_Parent" pk="<%= view.ViewDataSetID%>"><%=displayName%></a>
        <a  href="#" onClick="" class="sec"></a>
        <%--<% if (ViewData["viewName"] != null)
            {%>
            <a class="settings" <%=Map.Database.Localizer.Direction=="RTL" ? " style='float: left'":" style='float: right'" %> href='<%=Url.Action("MenuDisplayState", "Durados", new {viewName = ViewData["viewName"], guid = ViewData["guid"], display = !displaySettings}) %>'><%=Map.Database.Localizer.Translate(displaySettings ? "Hide Settings" : "Show Settings")%></a>
        <%} %>--%>
        <%if (Durados.Web.Infrastructure.General.IsMobile())
      { %>
                <% Html.RenderPartial("~/Views/Shared/Controls/LogOnUserControl.ascx", null); %>
                <%} %>
    </div>
    <%--<div class="refer-bar"> <a onClick="dosomething">Calendar</a><a onClick="dosomething" class="sec">3rd quarter Glowner's Video Conference</a><a onClick="dosomething"  class="settings">Hide Settings</a></div>--%>
    
    <%}else { %>
   
    <div class="mainMenu">
    <%  if (!Durados.Web.Infrastructure.General.IsMobile())
            Html.RenderPartial("~/Views/Shared/Controls/MainMenu.ascx"); %>
    </div>

     <table cellspacing="0" cellpadding="0" width="100%">
        <tr id='rowtabletitle'>
            <td colspan="*">
                &nbsp;
                <%--<%if (!(view.Database is Durados.Web.Mvc.Config.Database) && (Map.Database.IsInRole("Developer") || Map.Database.IsInRole("Admin")))
                  { %>
                  <% string href = Url.Action("Index", "Admin", new { viewName = "View", isMainPage = true, ID = view.ViewDataSetID, __Fields_Parent__ = view.Description }); %>
                <a href="<%=href %>" colname="Fields_Parent" pk="<%= view.ViewDataSetID%>"><span class="tabletitle" ><%=path %></span>
                <span id="rowtabletitleSpan" class="tabletitle" title="<%= description %>" d_Dn='<%=displayName%>'><%=displayName%></span>&nbsp;&nbsp;&nbsp;<span class="tabletitlefilter"><%=view.GetPageFilter(guid).ToFriendlyString()%></span>
                </a>
                <%}
                  else
                  { %>
                 <span class="tabletitle" ><%=path %></span><span id="rowtabletitleSpan" class="tabletitle" title='<%= description %>' d_Dn='<%=displayName%>'><%=displayName%></span>&nbsp;&nbsp;&nbsp;<span class="tabletitlefilter"><%=view.GetPageFilter(guid).ToFriendlyString()%></span>
              
                <%} %>
                --%>
                <%if (profile)
                  {%><span><input type="button" value="Profile" onclick="runYahoo();" /></span><%} %>

                  <% string href = "#";%>
    <% string onclick = "";%>
        <%if (!(view.Database is Durados.Web.Mvc.Config.Database) && (Map.Database.IsInRole("Developer") || Map.Database.IsInRole("Admin") || view.IsViewOwner()))
            { %>
            <% onclick = "toggleViewProperties('" + guid + "')"; %> 
            <%--<%  href = Url.Action("Index", "Admin", new { viewName = "View", isMainPage = true, ID = view.ViewDataSetID, __Fields_Parent__ = view.Description }); %>
            --%>
        <%} %>
                <%--<a href="<%=href %>" colname="Fields_Parent" pk="<%= view.ViewDataSetID%>"><span class="tabletitle" ><%=path %></span>
                <span id="rowtabletitleSpan" class="tabletitle" title="<%= description %>" d_Dn='<%=displayName%>'><%=displayName%></span>&nbsp;&nbsp;&nbsp;<span class="tabletitlefilter"><%=view.GetPageFilter(guid).ToFriendlyString()%></span>
                </a>--%>
        <a id="rowtabletitleSpan" onclick="<%=onclick %>" title="<%= description %>" d_Dn='<%=displayName%>' href="<%=href %>" colname="Fields_Parent" pk="<%= view.ViewDataSetID%>"><%=displayName%></a>
        <a  href="#" onClick="" class="sec"></a>
        <%--<% if (ViewData["viewName"] != null)
            {%>
            <a class="settings" <%=Map.Database.Localizer.Direction=="RTL" ? " style='float: left'":" style='float: right'" %> href='<%=Url.Action("MenuDisplayState", "Durados", new {viewName = ViewData["viewName"], guid = ViewData["guid"], display = !displaySettings}) %>'><%=Map.Database.Localizer.Translate(displaySettings ? "Hide Settings" : "Show Settings")%></a>
        <%} %>--%>
            </td>
        </tr>
    </table>

    <%} %>
        <%if (profile)
                  {%><span><input type="button" value="Profile" onclick="runYahoo();" /></span><%} %>
    <%--<table cellspacing="0" cellpadding="0" width="100%" style="display: none">
        <tr id='rowtabletitle'>
            <td colspan="*">
                
                &nbsp;
                
                
                <%if (profile)
                  {%><span><input type="button" value="Profile" onclick="runYahoo();" /></span><%} %>
            </td>
        </tr>
    </table>--%>

    <script type="text/javascript">
        <% if (profile){ %>
        //FOR profile USING YAHOO
        try{
            var yFunc = new Array();
            //yFunc.push("EditDialog.Open2,null");
            //yFunc.push("initDataTableView,window");
            
            //yFunc.push("initDropdownchecklistFilter,window");
            //yFunc.push("success,window");
            //yFunc.push("complete,window");
            //yFunc.push("editopen,loop"); //monitor a section/block in the method
//yFunc.push("prgress,loop");
//yFunc.push("selectRow,loop");
yFunc.push("settings,loop");
            for (var i=0, len = yFunc.length; i < len; ++i) {       
                var vals = yFunc[i].split(',');
                if(vals[1]=="null")
                    YAHOO.tool.Profiler.registerFunction(vals[0],null);
                else if(vals[1]=="window")
                    YAHOO.tool.Profiler.registerFunction(vals[0],window);
            }
            
            function runYahoo() {
                var msg = "";
                for (var i=0, len = yFunc.length; i < len; ++i) {       
                    msg += getYMessage(yFunc[i]) + '\n';
                }
                alert(msg);
            }

            function getYMessage(func) {
                
                var vals = func.split(',');
                var obj = null;
                if(vals[1]=="window")
                    obj = window;
                    
                var calls = YAHOO.tool.Profiler.getCallCount(vals[0],obj);
                var max = YAHOO.tool.Profiler.getMax(vals[0],obj);
                var min = YAHOO.tool.Profiler.getMin(vals[0],obj);
                var avg = YAHOO.tool.Profiler.getAverage(vals[0],obj);
                
                if(vals[1]!="loop")
                {
                    YAHOO.tool.Profiler.unregisterFunction(vals[0],obj);
                    YAHOO.tool.Profiler.registerFunction(vals[0],obj);
                }

                var msg = vals[0] + " was run " + calls + " times.\n"
                msg += "The average time was " + avg + "ms.\n";
                msg += "The max time was " + max + " ms.\n";
                msg += "The min time was " + min + " ms.\n";
                YAHOO.tool.Profiler.clear(vals[0]);
                return msg;
            }
        }
        catch(err){
            
        }
       //END OF profile
        <%} %>
        var erdguid = '<%=guid %>';
        var d_host = '<%=Durados.Web.Mvc.Maps.Host %>';
        
        var d_autoCommit = '<%= Map.Database.AutoCommit %>' == 'True';
        var rootPath = $('#GetRootPath').val();
        var d_MinColumnWidth = <%=Map.Database.MinColumnWidth %>;
        var translator = Sys.Serialization.JavaScriptSerializer.deserialize($('#translator').val());
            
        $(document).ready(function() {
            initDataTableView('<%=guid %>');
//            var settingsAbleJS = '<%=settingsAbleJS %>';
//            
//            if (settingsAbleJS == '1' && !$.cookie("viewPropertiesFirstTime")){
//                toggleViewProperties('<%=guid %>', false, '<%=viewSettingsWidth %>' , <%=viewSettingsUrl %>,'<%=view.DisplayName %>');
//                
//                
//            }

            
            <%if(view.IsEditable(guid) || view.IsCreatable() || Map.Database.IsInRole("Developer") || Map.Database.IsInRole("Admin")){ %>
            Durados.ColumnResizer.initHandler('<%=guid %>');
            <%} %>
        });
        //document.title = '<%= Map.Database.Localizer.Translate(Map.Database.SiteInfo == null ? Map.Database.DisplayName : (string.IsNullOrEmpty(Map.Database.SiteInfo.Product) ? Map.Database.DisplayName : Map.Database.SiteInfo.Product)) + " - " + displayName %>';
    </script>
    <% 
    bool debug = System.Configuration.ConfigurationManager.AppSettings["Debug"].ToLower() == "true";
    string mainAppDivClass = string.Empty;
    %>    
    <% if (!string.IsNullOrEmpty(view.TreeViewName)) {
           mainAppDivClass = "main_app_1_tree";%>
    <% if (debug)
       { %>
        <script src="<%=ResolveUrl("~/Scripts/jquery.jstree.js") %>" type="text/javascript"></script>
    <%} %>
   <%-- <%} else { %>
        <script src="<%=ResolveUrl("~/Scripts/jquery.jstree.min.js") %>" type="text/javascript"></script>
    
    <%} %>--%>

       

       <div id="AppTreeDiv">
    
       <div id="TreeInnerDiv"></div>

       <div id="TreeSplitterBar" class="vsplitbar"></div>

    </div>
    
   <%-- <% } 
       
      if(view.FilterType==Durados.FilterType.Tree && !view.HideFilter)
      {
          bool isExistAdditionalTree = !string.IsNullOrEmpty(view.TreeViewName);
          mainAppDivClass = isExistAdditionalTree ? "main_app_2_tree" : "main_app_1_tree";%>
      <div id="AppFilterTreeDiv" <% if (isExistAdditionalTree){%> class="filter_tree_with_margin" <% } %>>    
       <div id="FilterTreeInnerDiv">
    <%  Html.RenderPartial("~/Views/Shared/Controls/Filter/TreeFilter.ascx", view); %>
       </div>
       <div data-guid="<%=guid %>" id="FilterTreeSplitterBar" class="vsplitbar"></div>
    </div>
--%>
    <% } %>

    

    <div id="mainAppDiv" <% if (!string.IsNullOrEmpty(mainAppDivClass)) { %> class="<%=mainAppDivClass %>" <% } %>>
    <%  Html.RenderPartial("~/Views/Shared/Controls/GridView.ascx", Model); %>
    </div>
          <% if (!string.IsNullOrEmpty(view.TreeViewName)) { 
                 Durados.Web.Mvc.View treeView = (Durados.Web.Mvc.View)Database.Views[view.TreeViewName];
          %>
          <script type="text/javascript">
              $(document).ready(function () {
                  Durados.TreeView.initTreeView('<%= treeView.Name %>', '', '<%= guid %>', '<%= view.TreeRelatedFieldName %>');
              });
          </script>
        <% } %>  
    
     <%--for paging, sorting and filtering--%>
    <div id="Div1" style="display:none"></div>
    <div id='sendEmailDiv' style='Display:none'></div>
    <div style="display:none" id="DeleteMessage"  hasguid='hasguid' guid='<%=guid %>'>
        <%  Html.RenderPartial("~/Views/Shared/Controls/DeleteMessage.ascx", String.Empty); %>
    </div>
    
    
     <div style="display:none" id="DeleteSelectionMessage" >
        <%  Html.RenderPartial("~/Views/Shared/Controls/DeleteMessage.ascx", Map.Database.Localizer.Translate("Are you sure that you want to delete the selected rows?")); %>
    </div>
    
    <div style="display:none" id="rich" >
    <span id="richSpan">
        <textarea id='richTextArea' class='wtextarea' hasguid='hasguid' guid='<%=guid %>'></textarea>
        <span class="textareaMaxCharsMsg textareaRequiredMsg"></span>
        </span>
    </div>
    
    <div style="display:none" id="notRich" > 
     <span id="notRichSpan">       
        <textarea id='notRichTextArea' style="width:100%;height:100%" hasguid='hasguid' guid='<%=guid %>'></textarea>
        <span class="textareaMaxCharsMsg textareaRequiredMsg"></span>
        </span>
    </div>
    <div style="position: absolute;top:0;left:0" id="SpryFormatsTooltip">
    <table>
    <thead align="left">
      <tr>
        <th> <p>Value</p></th>
        <th> <p>Description</p></th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>"0"</td>
        <td>Whole numbers between 0 and 9</td>
      </tr>
      <tr>
        <td>"A" </td>
        <td>Uppercase alphabetic characters</td>
      </tr>
      <tr>
        <td>"a"</td>
        <td>Lowercase alphabetic characters</td>
      </tr>
      <tr>
        <td>"B"; "b"</td>
        <td>Case-insensitive alphabetic characters</td>
      </tr>
      <tr>
        <td>"X"</td>
        <td>Uppercase alphanumeric characters</td>
      </tr>
      <tr>
        <td>"x"</td>
        <td>Lowercase alphanumeric characters</td>
      </tr>
      <tr>
        <td>"Y"; "y"</td>
        <td>Case-insensitive alphanumeric characters</td>
      </tr>
      <tr>
        <td>"?"</td>
        <td>Any character</td>
      </tr>
    </tbody>
  </table>
    
    </div>
    
    <div style="display:none" id="richDisabled" >
        <div id='richDiv' style="border:none" class='wtextarea' hasguid='hasguid' guid='<%=guid %>'></div>
    </div>
    
    <%  Html.RenderPartial("~/Views/Shared/Controls/Url.ascx"); %>
    <%} catch(Exception exception){ %>
        <% Map.Logger.Log("aspx", "index", exception.Source, exception, 1, null); %>
        <%  Html.RenderPartial("~/Views/Shared/Controls/ErrorMessage.ascx", exception.Message); %>
    <%} %>
</asp:Content>

