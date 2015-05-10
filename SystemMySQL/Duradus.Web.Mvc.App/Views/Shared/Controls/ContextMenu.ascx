<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.ClientParams>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Durados.DataAccess" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Json" %>
<% string guid = Model.Guid; %>
<% Durados.Web.Mvc.View view = Model.View; %>

<!-- Context Menu -->
<% bool displayAdminContextMenu = (bool)(!MenuHelper.HideSettings()) && view.IsAdmin() && !view.Database.IsConfig; %>

<ul  id="myMenu<%=guid %>" class="contextMenu">
    <%
        string editCaption = view.IsEditable(guid) ? view.EditButtonName : "View";
        string editClass = view.IsEditable(guid) ? "edit" : "view";
    %>
    <% if (view.DisplayType == Durados.DisplayType.Table && view.IsEditable(guid))
    {  %>
        <li class="<%=editClass %>"><a d_action="edit" href="#edit"><span class="icon"></span><%=Map.Database.Localizer.Translate(editCaption)%></a></li>
    <%} %>
    <% if (view.IsDuplicatable())
    {  %>
        <li class="duplicate"><a d_action="duplicate" href="#duplicate"><span class="icon"></span><%=Map.Database.Localizer.Translate(view.DuplicateButtonName)%></a></li>
    <%} %>
    <% if (view.IsCreatable() && view.IsOrdered)
    {  %>
        <li class="insert"><a d_action="insert" href="#insert"><span class="icon"></span><%=Map.Database.Localizer.Translate(view.InsertButtonName)%></a></li>
    <%} %>
    <% if (view.HasWorkFlowSteps && view.DisplayType == Durados.DisplayType.Table && view.IsEditable(guid))
    {  %>
        <li class="complete"><a d_action="complete" href="#complete" title='<%=view.PromoteButtonDescription %>' ><span class="icon"></span><%=Map.Database.Localizer.Translate(view.PromoteButtonName)%></a></li>
    <%} %>
	<% if (view.IsDeletable())
       { %>
        <li class="delete"><a d_action="delete" href="#delete"><span class="icon"></span><%=Map.Database.Localizer.Translate(view.DeleteButtonName)%></a></li>
    <%} %>
    <% if ((view.GridEditable && view.IsEditable(guid)))
       { %>
        <li class="copy separator"><a d_action="copy" class="copyButton" href="#copy"><span class="icon"></span><%=Map.Database.Localizer.Translate("Copy")%></a></li>
        <li class="paste"><a d_action="paste" class="pasteButton" href="#paste"><span class="icon"></span><%=Map.Database.Localizer.Translate("Paste")%></a></li>
    
    <%} %>
	<% if (view.IsCreatable())
    {  %>
        <li class="add separator"><a d_action="add" href="#add"><span class="icon"></span><%=Map.Database.Localizer.Translate(view.NewButtonName)%></a></li>
    <%} %>
    <% if (!string.IsNullOrEmpty(view.AnotherRowLinkText))
       { %>
        <li class="another separator"><a d_action="another" href="#another"><span class="icon"></span><%=Map.Database.Localizer.Translate(view.AnotherRowLinkText)%></a></li>
    <%} %>
    <% if (view.SaveHistory && view.DisplayType == Durados.DisplayType.Table)
    {  %>
        <li class="history"><a d_action="history" href="#history"><span class="icon"></span><%=Map.Database.Localizer.Translate("History")%></a></li>
    <%} %>
    <% if (view.Send && view.DisplayType == Durados.DisplayType.Table)
    {  %>
        <li class="send"><a d_action="send" href="#send"><span class="icon"></span><%=Map.Database.Localizer.Translate("Send")%></a></li>
    <%} %>
    
    <% if (view is Durados.Config.IConfigView && (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin")) && view.Name == "View")
    { %>    
        <li class="open"><a d_action="open" href="#open"><span class="icon"></span><%=Map.Database.Localizer.Translate("Open")%></a></li>
    <%} %>
    
    <%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="hide"><a d_action="hide" href="#hide"><%=Map.Database.Localizer.Translate("Hide")%></a></li>
    <%} %>
    
    <%--<%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="unhide"><a d_action="unhide" href="#unhide"><%=Map.Database.Localizer.Translate("Unhide")%></a></li>
    <%} %>--%>
    
    <%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="left separator"><a d_action="left" href="#left"><%=Map.Database.Localizer.Translate("Left")%></a></li>
    <%} %>
    
    <%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="right"><a d_action="right" href="#right"><%=Map.Database.Localizer.Translate("Right")%></a></li>
    <%} %>
    
    <%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="before"><a d_action="before" href="#before"><%=Map.Database.Localizer.Translate("Before")%></a></li>
    <%} %>

    <%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="after"><a d_action="after" href="#after"><%=Map.Database.Localizer.Translate("After")%></a></li>
    <%} %>
    
    <%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="rename separator"><a d_action="rename" href="#rename"><%=Map.Database.Localizer.Translate("Rename")%></a></li>
    <%} %>

<%--    <%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="widths separator"><a d_action="saveAsDefaults" href="#saveAsDefaults"><%=Map.Database.Localizer.Translate("Set As Default")%></a></li>
    <%} %>

    <%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="widths"><a d_action="restoreDefaults" href="#restoreDefaults"><%=Map.Database.Localizer.Translate("Restore Default")%></a></li>
    <%} %>
    
   
    <%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="config separator"><a d_action="config" href="#config"><%=Map.Database.Localizer.Translate("Config")%></a></li>
    <%} %>
--%>     
    <%if (displayAdminContextMenu && (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin"))){ %>
        <li d_admin="d_admin" class="addColumn"><a d_action="addColumn" href="#addColumn"><%=Map.Database.Localizer.Translate("Add Column")%></a></li>
    <%} %>
    
    <%if (displayAdminContextMenu){ %>
        <li d_admin="d_admin" class="properties"><a d_action="properties" href="#properties"><%=Map.Database.Localizer.Translate("Properties")%></a></li>
    <%} %>
    
    <%if (displayAdminContextMenu && !Map.Database.AutoCommit)  { %>
        <li d_admin="d_admin" class="commit separator"><a d_action="commit" href="#commit"><%=Map.Database.Localizer.Translate("Commit")%></a></li>
    <%} %>
</ul>


