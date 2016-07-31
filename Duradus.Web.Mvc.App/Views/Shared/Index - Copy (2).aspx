<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<System.Data.DataView>" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%=((Durados.Web.Mvc.View)Database.Views[Model.Table.TableName]).JavaScripts %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.Table.TableName]; %>
    
    <%  Html.RenderPartial("~/Views/Shared/Controls/MainMenu.ascx"); %>
    <div id="ajaxDiv"><%  Html.RenderPartial("~/Views/Shared/Controls/DataTableView.ascx", Model); %></div>
    
    <%--for paging, sorting and filtering--%>
    <div style="display:none" id="DataRowCreate" >
         <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowView.ascx", new Durados.Web.Mvc.UI.DataActionFields(){Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Create), DataAction = Durados.DataAction.Create}); %>
    </div>
    
    
    <div style="display:none" id="DataRowEdit" >
        <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowView.ascx", new Durados.Web.Mvc.UI.DataActionFields(){Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Edit), DataAction = Durados.DataAction.Edit}); %>
    </div>
    
    <div style="display:none" id="DeleteMessage" >
        <%  Html.RenderPartial("~/Views/Shared/Controls/DeleteMessage.ascx", String.Empty); %>
    </div>
    
     <div style="display:none" id="DeleteSelectionMessage" >
        <%  Html.RenderPartial("~/Views/Shared/Controls/DeleteMessage.ascx", "Are you sure that you want to delete the selected rows?"); %>
    </div>
    
    <div style="display:none" id="rich" >
        <textarea id='richTextArea' class='wtextarea'></textarea>
    </div>
    
    <div style="display:none" id="DataRowInlineAdding" >
    </div>
    
    <div style="display:none" id="DataRowDialog" >
    </div>
    
    <div style="display:none" id="inlineAdding" >
    </div>

    <div id="ProgressionDiv">
        <img  src='<%= Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/progress-wheel.gif"%>' class="Progress" />
    </div>
    
    <div style="display:none" id="Div1" >
    </div>


    <script type="text/javascript">
    
    // global from server
    var gColCount = <%=view.ColumnsInDialog %>;
    var gCreateRowCount = <%=view.VisibleFieldsForCreate.Count %>;
    var gEditRowCount = <%=view.VisibleFieldsForEdit.Count %>;
    var gCreateUrl = '<%=view.GetCreateUrl() %>';
    var gEditUrl = '<%=view.GetEditUrl() %>';
    var gEditSelectionUrl = '<%=view.GetEditSelectionUrl() %>';
    var gGetJsonViewUrl = '<%=view.GetJsonViewUrl() %>';
    var gGetRichUrl = '<%=view.GetRichUrl() %>';
    var gEditRichUrl = '<%=view.GetEditRichUrl() %>';
    var gGetSelectListUrl = '<%=view.GetSelectListUrl() %>';
    
    var gGetJsonViewInlineAddingUrl = '<%=view.GetJsonViewInlineAddingUrl() %>';
    var gInlineAddingDialogUrl = '<%=view.GetInlineAddingDialogUrl() %>';
    var gInlineAddingCreateUrl = '';
    var gDeleteUrl = '<%=view.GetDeleteUrl() %>';
    var gDeleteSelectionUrl = '<%=view.GetDeleteSelectionUrl() %>';
    var gFilterUrl = '<%=view.GetFilterUrl() %>';
    var gSetLanguageUrl = '<%=view.GetSetLanguageUrl() %>';
    var gIndexUrl = '<%=view.GetIndexUrl() %>';
    var gExportToCsvUrl = '<%=view.GetExportToCsvUrl() %>';
    var gPrintUrl = '<%=view.GetPrintUrl() %>';
    var gAutoCompleteUrl = '<%=view.GetAutoCompleteUrl() %>';
    var gViewDisplayName = '<%=Durados.Web.Localization.Localizer.Translate(view.DisplayName) %>';
    var gDateFormat = '<%=view.GetDateFormat() %>';
    var gViewName = '<%=view.Name %>';

    var createPrefix = 'create_';
    var editPrefix = 'edit_';
    var filterPrefix = 'filter_';
    var inlineAddingPrefix = 'inlineAdding_';
 
   
    // upload 
    var gUploadUrl = '<%=view.GetUploadUrl() %>';
    function UploadFile(id, prefix) {
        new Ajax_upload(prefix + 'upload_' + id, {
		    action: gUploadUrl,
		    data: {viewName: gViewName, fieldName: id},
		    onSubmit : function(file , ext){
			    $('#' + prefix + 'upload_span_' + id).text('Uploading ' + file);
			    this.disable();	
		    },
		    onComplete : function(file, response){
		        showUpload(id, prefix, file, response);
			    
		    }		
	    });	
    }
    
    function showUpload(id, prefix, file, src){
        $('#' + prefix + 'upload_span_' + id).text('Uploaded ');
	    $('#' + prefix + id).val(file);
	    $('#' + prefix + 'upload_img_' + id).attr('src', src);
	    $('#' + prefix + 'upload_img_' + id).css('visibility','visible');
    }
    
    function resetUpload(){
        $('.uploadDiv span').text('Upload');
        $('.uploadDiv img').attr('src', '');
        $('.uploadDiv img').css('visibility','hidden');
        $('.uploadDiv input').val('');
    }
    
    // validation 
    <%=view.GetValidation(Durados.DataAction.Create) %>
    <%=view.GetValidation(Durados.DataAction.Edit) %>

    //todo: remove and get the JSON when needed
    function GetJsonViewForCreate(){
        //var jsonView = <%=view.GetJsonView(Durados.DataAction.Create) %>;
        var jsonView = <%=ViewData["jsonView"].ToString() %>;
        
        return jsonView;
    }
    
    function GetJsonViewForInlineAdding(viewName){
        var syncJsonView = null;
        $.ajax({
            url: gGetJsonViewInlineAddingUrl + viewName,
            contentType: 'application/json; charset=utf-8',
            data: { viewName: viewName, dataAction: 'Create' },
            async: false,
            dataType: 'json',
            cache: false,
            error: function(XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
            },
            success: function(jsonView) {
                if (jsonView == null || jsonView == '') {
                    alert("error");
                }
                else {
                    syncJsonView = Sys.Serialization.JavaScriptSerializer.deserialize(jsonView);
                }

            }

        });
        
        return syncJsonView;
    }
    
    function GetJsonViewForEdit(){
        var jsonView = <%=view.GetJsonView(Durados.DataAction.Edit) %>;
        return jsonView;
    }
    
    function GetJsonFilter(){
        var jsonFilter = <%=view.GetJsonFilter() %>;
        return jsonFilter;
    }
    
    function FillJson(json, prefix){
        for (var index = 0, len = json.Fields.length; index < len; ++index) {
            
            var field = json.Fields[index].Value;
            field.Value = $('#' + prefix + field.Name).val();
            //
            var htmlField = $('#' + prefix + field.Name);
            if (htmlField[0] != null){
                if (htmlField[0].type == "checkbox")
                    field.Value = htmlField.attr('checked');
                else if ($(htmlField[0]).attr('radioButtons') == 'radioButtons')
                    field.Value = $('[name=' + prefix + field.Name + ']:checked').val();
                else if ($(htmlField[0]).attr('radioButtons') == 'radioButtons')
                    field.Value = $('[name=' + prefix + field.Name + ']:checked').val();
                else if(htmlField.attr('valueId') != undefined)
                    field.Value = htmlField.attr('valueId');
                else
                    field.Value = htmlField.val();
            }
        }
        
        return json;
    }
    
    
        
    $(document).ready(function() {
        
        Autocomplete.Init();
        
    });
    
//    $(document).ready(function() {
//        $("#tabs").tabs();
//    });
//    
//    function SetSelected(tab){
//       
//        $(tab).siblings('li').attr('class','ui-state-default ui-corner-top');
//        $(tab).attr('class','ui-state-default ui-corner-top ui-tabs-selected ui-state-active');
//        
//        //alert($(tab).parents('div:first').attr('id'));
//        $(tab).parents('div:first').children('div').css('z-index','100000');
//        var id = $(tab).children('a:first').attr('href');
//        //alert($(id).css('visibility'));
//        $(id).css('z-index','100001');
//        //alert($(id).css('visibility'));
//    }
    
//    $(document).ready(function() {
//         $("#DataRowCreate").bind('dialogopen', function() { 
//        /* init tabs, when dialog is opened */ 
//            $(".tabs").tabs(); 
//        }); 
//        $("#DataRowCreate").bind('dialogclose', function() { 
//            /* destroy tabs to avoid problems on re-open */ 
//            $(".tabs").tabs('destroy'); 
//        }); 
//        
//    });
    

    var Autocomplete; if (!Autocomplete) Autocomplete = {};

    Autocomplete.Init = function() {
        Autocomplete.InitCreateFields();
        Autocomplete.InitEditFields();
        Autocomplete.InitFilterFields();
    }
    
    Autocomplete.InitCreateFields = function() {
        var json = GetJsonViewForCreate();
        Autocomplete.InitFields(json, createPrefix);
    }
    
    Autocomplete.InitEditFields = function() {
        var json = GetJsonViewForEdit();
        Autocomplete.InitFields(json, editPrefix);
    }
    
    Autocomplete.InitFilterFields = function() {
        var json = GetJsonFilter();
        Autocomplete.InitFields(json, filterPrefix);
    }
    
    Autocomplete.InitFields = function(json, prefix) {
        for (var index = 0, len = json.Fields.length; index < len; ++index) {
            var field = json.Fields[index].Value;
            if(field.Type == "Autocomplete")
            {
                Autocomplete.InitField(field.Name, prefix);    
            }
        }
    }
    
    Autocomplete.InitField = function(fieldName, prefix) {
     
        $("#" + prefix + fieldName).autocomplete(gAutoCompleteUrl +'?field=' + fieldName,
        {
            dataType: 'json',
            parse: AutoCompleteParse,
            formatItem: function(row) {
                return row.Name;
            },
            width: 160,
            highlight: false,
            multiple: false,
            multipleSeparator: ";"
        }).result(AutoCompleteResult);
    }
    
  
    //initiate dialog for add & edit
    $(document).ready(function() {
        var winWidth = $(window).width();
        var winHeight = $(window).height();
        var tabWidth = winWidth * .75 - 55;
          
        var rec = Rectangle.Load(gViewName);
        if(rec != null)
        {
            $("#DataRowCreate").dialog({
                bgiframe: true,
                autoOpen: false,
                modal: true,
                position: [rec.left,rec.top],
                width: rec.width,
                height: rec.height,
                resizeStop: SaveDialogOnResize,
                dragStop: SaveDialogOnDrag
            });
        
            $("#DataRowEdit").dialog({
                bgiframe: true,
                autoOpen: false,
                modal: true,
                position: [rec.left,rec.top],
                width: rec.width,
                height: rec.height,
                resizeStop: SaveDialogOnResize,
                dragStop: SaveDialogOnDrag
            });
            
            tabWidth = rec.width - 55;
        }
        else
        {                      
    
            $("#DataRowCreate").dialog({
                bgiframe: true,
                autoOpen: false,
                modal: true,
                position: 'center',
                width: (winWidth *.75),
                resizeStop: SaveDialogOnResize,
                dragStop: SaveDialogOnDrag
            });
        
            $("#DataRowEdit").dialog({
                bgiframe: true,
                autoOpen: false,
                modal: true,
                position: 'center',
                width: (winWidth *.75),
                resizeStop: SaveDialogOnResize,
                dragStop: SaveDialogOnDrag
            });
        }
        
        $("#rich").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            position: 'center',
            height: (winHeight *.75),
            width: (winWidth *.75)
        });
        
        $("#DataRowInlineAdding").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            close: InlineAdding.CloseEvent,
            position: 'center',
            height: (winHeight *.75),
            width: (winWidth *.75)
        });
        
        //init tabs
        $('#CreateTabs').css("width", tabWidth  + 'px');
        $('#EditTabs').css("width", tabWidth  + 'px');
        $('#CreateTabs').tabs({ fx: { opacity: 'toggle', duration: 'fast' } }); // first tab selected
        $('#EditTabs').tabs({ fx: { opacity: 'toggle', duration: 'fast' } }); // first tab selected
        
    }); 
    
    
    
    function getDialogWidth(colCount, rowWidth){
        return colCount * rowWidth * 2;
    }
    
    function getDialogHeight(colCount, rowCount, rowWidth){
        (rowCount / colCount + 1) * rowWidth + 120;
    }
    
    //initiate dialog for delete
    $(document).ready(function() {
        $("#DeleteMessage").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            width: 500,
            height: 200,
            position: 'center'
        });
    }); 
    
     //initiate dialog for delete
    $(document).ready(function() {
        $("#DeleteSelectionMessage").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            width: 500,
            height: 200,
            position: 'center'
        });
    }); 
    
    // Rectangle
    var Rectangle; if (!Rectangle) Rectangle = {};
    
    Rectangle.New = function(top, left, width, height) {
        var rect = new Object();
		
		rect.top = top;
		rect.left = left;
        rect.width = width;
        rect.height = height;
         
        return rect;
    }
    
    Rectangle.Save = function(rect, name) {
        if(rect != null)
        {
            var src = Sys.Serialization.JavaScriptSerializer.serialize(rect);
		    $.cookie(name, src);
		}
    }

    Rectangle.Load = function(name) {
        var src = $.cookie(name);
        
        if(src != null)
        {
		    var rect = Sys.Serialization.JavaScriptSerializer.deserialize(src);
		    return rect;
		}
		else return null;
    }
    
//    $(document).ready(function() {
//        var rect = Rectangle.New(0, 0, 200, 100);
//        
//        var name = "stam";
//        
//        Rectangle.Save(rect, name);
//        
//        var r = Rectangle.Load(name);
//        
//        alert(r.Width);
//        alert(r.Height);
//    });
    
    $(document).ready(function() {
            ddsmoothmenu.init({
            mainmenuid: "smoothmenu", //menu DIV id
            orientation: 'h', //Horizontal or vertical menu: Set to "h" or "v"
            classname: 'ddsmoothmenu', //class added to menu's outer DIV
            contentsource: "markup" //"markup" or ["container_id", "path_to_menu_file"]
        })
    });
    
    
    //Dialog
    function SaveDialogOnResize(event, ui)
    {
        var rect = Rectangle.New(ui.position.top, ui.position.left, ui.size.width, ui.size.height);
        Rectangle.Save(rect, gViewName);
        $('#CreateTabs').css("width", (ui.size.width - 55) + 'px');
        $('#EditTabs').css("width", (ui.size.width - 55)  + 'px');

    }
    
    function SaveDialogOnDrag(event, ui)
    {
        var rect = Rectangle.Load(gViewName)
        if(rect != null)
        {
            rect.top = ui.position.top;
            rect.left = ui.position.left;
            Rectangle.Save(rect, gViewName);
        }
    }
    
    function refreshView(){
        showProgress();
        
        
        $.post(gIndexUrl,
        {
            
        },
        function(html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                FilterForm.HandleSuccess(html, false);
            }   
            else {
                FilterForm.ShowFailureMessage(html.replace("$$error$$",""));
            }
        });
    }
    
    
    // add
    var AddDialog; if (!AddDialog) AddDialog = {};

    AddDialog.Open = function(pk) {
        $('#DataRowCreate').data('title.dialog', '<%= Durados.Web.Localization.Localizer.Translate("Add Row to")%> ' + gViewDisplayName); 
        $("#DataRowCreate").dialog("option", "buttons", {
          '<%= Durados.Web.Localization.Localizer.Translate("Add")%>': function() { AddDialog.Add(false); },
          '<%= Durados.Web.Localization.Localizer.Translate("Add Another")%>': function() { AddDialog.Add(true); },
          '<%= Durados.Web.Localization.Localizer.Translate("Cancel")%>': function() { $(this).dialog("close"); }
        });
        
        AddDialog.Reset();
        
        //jquery doesn't remember the position just the size
        var rec = Rectangle.Load(gViewName);
        if(rec != null)
        {
            $('#DataRowCreate').dialog("option", "position", [rec.left,rec.top]);
        
        }
        $('#DataRowCreate').dialog('open');

        //Create wysiwyg for all textareas
        AddDialog.CreateWysiwyg();

        var jsonFilter = FilterForm.JsonFilter;
        if (jsonFilter==null){
            jsonFilter=GetJsonFilter();
        }
        if (pk==null){
            //AddDialog.SetDefaultsFromFilter(jsonFilter, createPrefix);
        }
        else{
            EditDialog.Load(pk, createPrefix);
        }
        
        complete();
    }
    
    AddDialog.onafterSetDefaultsFromFilter = function(){}
    
       
    AddDialog.SetDefaultsFromFilter = function(json, prefix) {
        if (json!=null){
            var firstField = null;
            for (var index = 0, len = json.Fields.length; index < len; ++index) {
                var field = json.Fields[index].Value;
                var htmlField = $('#' + prefix + field.Name);
                if (index==0)
                    firstField=htmlField;
                 
                if (field.Value != null && field.Value != ''){
                    
                    if ($(htmlField[0]).attr('upload') == 'upload'){
                        showUpload(field.Name, prefix, field.Value, $('#' + prefix + 'upload_img_' + field.Name).attr('UploadPath') + field.Value);
                    }        
                    if ($(htmlField[0]).attr('radioButtons') == 'radioButtons'){
                        $('[name=' + prefix + field.Name + '][value=' + field.Value + '' + ']').attr('checked', 'checked');
                    }        
                    else if(htmlField[0] != undefined && htmlField[0].type == "textarea")
                    {
                        htmlField.text(field.Value);
                        //if (htmlField.attr('rich')=='true')
                        //$('#' + prefix + field.Name).wysiwyg(); //create the wysiwyg
                    }
                    else if(htmlField[0] != undefined && htmlField[0].type == "checkbox")
            	        htmlField.attr('checked', field.Value);
        	        else if(field.Type == 'Autocomplete'){
                        htmlField.attr('valueId', field.Value);
                        htmlField.val(field.Default);
                    }
			        else
                        htmlField.val(field.Value);
                } 
            }
            
            $(AddDialog).trigger('onafterSetDefaultsFromFilter', json);
            firstField.focus();
        }
    }
    
    
    AddDialog.Show = function() {
        $('#DataRowCreate').data('title.dialog', '<%= Durados.Web.Localization.Localizer.Translate("Add Row to")%> ' + gViewDisplayName); 
        $("#DataRowCreate").dialog("option", "buttons", {
          '<%= Durados.Web.Localization.Localizer.Translate("Add")%>': function() { AddDialog.Add(false); },
          '<%= Durados.Web.Localization.Localizer.Translate("Add Another")%>': function() { AddDialog.Add(true); },
          '<%= Durados.Web.Localization.Localizer.Translate("Cancel")%>': function() { $(this).dialog("close"); }
        });

        //jquery doesn't remember the position just the size
        var rec = Rectangle.Load(gViewName);
        if(rec != null)
        {
            $('#DataRowCreate').dialog("option", "position", [rec.left,rec.top]);
        
        }
        $('#DataRowCreate').dialog('open');

        //Create wysiwyg for all textareas
        AddDialog.CreateWysiwyg();

        complete();
    } 

    

    AddDialog.Reset = function() {
        $('#CreateDataRowForm')[0].reset();
        resetUpload();
        AddDialog.SetDefaults();
        
        var jsonFilter = FilterForm.JsonFilter;
        if (jsonFilter==null){
            jsonFilter=GetJsonFilter();
        }
        AddDialog.SetDefaultsFromFilter(jsonFilter, createPrefix);
    }
    
    AddDialog.SetDefaults = function(){
        var json = GetJsonViewForCreate();
        
        if (json!=null){
            for (var index = 0, len = json.Fields.length; index < len; ++index) {
                var field = json.Fields[index].Value;
                var htmlField = $('#' + createPrefix + field.Name);
                if (field.Default != null && field.Default != ''){
                    
                    if ($(htmlField[0]).attr('upload') == 'upload'){
                        showUpload(field.Name, createPrefix, field.Default, $('#' + createPrefix + 'upload_img_' + field.Name).attr('UploadPath') + field.Default);
                    }        
                    if ($(htmlField[0]).attr('radioButtons') == 'radioButtons'){
                        $('[name=' + createPrefix + field.Name + '][value=' + field.Value + '' + ']').attr('checked', 'checked');
                    }        
                    else if(htmlField[0].type == "textarea")
                    {
                        htmlField.text(field.Default);
                        $('#' + createPrefix + field.Name + ' [rich="true"]').htmlarea(); //create the wysiwyg
                    }
                    else if(htmlField[0].type == "checkbox")
            	        htmlField.attr('checked', field.Default);
        	        else if(field.Type == 'Autocomplete'){
                        htmlField.attr('valueId', field.Default);
                        htmlField.val(field.Default);
                    }
			        else
                        htmlField.val(field.Default);
                } 
            }
        }
    }
    
    AddDialog.CreateWysiwyg = function() {
        $('#DataRowCreate textarea[rich="true"]').each(function() {$(this).htmlarea("dispose")});
        $('#DataRowCreate textarea[rich="true"]').htmlarea();
        //$(".wysiwyg").remove();
        //$('#DataRowCreate textarea[rich="true"]').wysiwyg();
    } 
    
    AddDialog.Close = function() {
        $('#DataRowCreate').dialog('close');
    } 
    
    AddDialog.Add = function(another) {
        if (AddDialog.isValid() == false) {
            return;
        }
        showProgress();
        $.post(gCreateUrl,
        {
            jsonView : Sys.Serialization.JavaScriptSerializer.serialize(FillJson(GetJsonViewForCreate(), createPrefix))
        },
        function(html) {
            hideProgress();
            var indexError = html.indexOf("$$error$$", 0);
            var indexMessage = html.indexOf("$$message$$", 0);
            var hasError = indexError > 0 && indexError < 1000;
            var hasMessage = indexMessage > 0 && indexMessage < 1000;
            if (hasError){
                AddDialog.ShowFailureMessage(html.replace("$$error$$",""));
            }
            else if (hasMessage){
                AddDialog.ShowFailureMessage(html.replace("$$message$$",""));
                showProgress();
                refreshView();
                if (!another)
                    AddDialog.Close();
            }
            else{
                AddDialog.HandleSuccess(html, another);
            }
//            if (indexError < 0 || indexError > 1000) {
//                AddDialog.HandleSuccess(html, another);
//            }
//            else {
//                AddDialog.ShowFailureMessage(html.replace("$$error$$",""));
//            }
        });
    }
    
    
    AddDialog.HandleSuccess = function(html, another) {
        if (another){
                AddDialog.Reset();
        }
        else{
            AddDialog.Close();
            
        }
        $('#ajaxDiv').html(html);
        success();
    }
    
    AddDialog.ShowFailureMessage = function(message) {
        alert(message);
        complete();
    }
    
    AddDialog.isValid = function() {
        
        var form = $('#CreateDataRowForm')[0];

        if (Spry.Widget.Form.validate(form) == false) {
            return false;
        }
        return true;
    }
    
    // rich
    var RichDialog; if (!RichDialog) RichDialog = {};

    RichDialog.Open = function(pk, fieldName) {
        begin();
        
        $('#rich').data('title.dialog', '<%= Durados.Web.Localization.Localizer.Translate("Edit Html on")%> ' + gViewDisplayName); 
        $("#rich").dialog("option", "buttons", {
          '<%= Durados.Web.Localization.Localizer.Translate("Update")%>': function() { RichDialog.Update(pk, fieldName); },
          '<%= Durados.Web.Localization.Localizer.Translate("Cancel")%>': function() { $(this).dialog("close"); }
        });
        
        $('#rich').dialog('open');
        
        RichDialog.Load(pk, fieldName);
         
        complete();
    } 
    
    RichDialog.Load = function(pk, fieldName) {
        
        var html = RichDialog.GetValue(pk, fieldName);

        //delete all class=wysiwyg to clear the textarea
        //$(".wysiwyg").remove();
        var textarea = $('#richTextArea');
        
        textarea.htmlarea("dispose");
        
        textarea.text(html);
        textarea.htmlarea(); //create the wysiwyg

        
    } 
    
    RichDialog.GetValue = function(pk, fieldName) {
        var syncJsonView = null;
        $.ajax({
            url: gGetRichUrl,
            contentType: 'application/json; charset=utf-8',
            data: { viewName: gViewName, pk: pk, fieldName: fieldName },
            async: false,
            dataType: 'json',
            cache: false,
            error: function(XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
            },
            success: function(html) {
                syncJsonView = html;

            }

        });
        
        return syncJsonView;
    }
    
    RichDialog.Update = function(pk, fieldName) {
        
        begin();
        
        var textarea = $('#richTextArea');
        
        var html = textarea.text();
        
       
        $.post(gEditRichUrl,
        {
            pk: pk,
            fieldName: fieldName,
            html : html
        },
        function(h) {
            var index = h.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                RichDialog.HandleSuccess(h);
            }
            else {
                RichDialog.ShowFailureMessage(h.replace("$$error$$",""));
            }
        });
    }
    
    RichDialog.HandleSuccess = function(html) {
        RichDialog.Close();
        $('#ajaxDiv').html(html);
        success();
    }
    
    RichDialog.ShowFailureMessage = function(message) {
        alert(message);
        complete();
    }
    
    RichDialog.Close = function() {
        $('#rich').dialog('close');
    } 
    
    // Dependency
    var Dependency; if (!Dependency) Dependency = {};
    
    Dependency.Load = function(viewName, fieldName, fk, select) {

        
        $.ajax({
            url: gGetSelectListUrl,
            contentType: 'application/json; charset=utf-8',
            data: { 
                viewName2: viewName,
                fieldName : fieldName,
                fk: fk 
            },
            async: false,
            dataType: 'json',
            cache: false,
            error: function(XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
            },
            success: function(selectList) {
                Dependency.LoadSelectList(select, selectList.Options);

                $(Dependency).trigger('onafterLoadSelectList', select);
           
            }

        });
        
    }
    
    Dependency.onafterLoadSelectList = function(){}
    
    Dependency.LoadSelectList = function(select, options) {
        Dependency.Clear(select);
        
        for (var index = 0, len = options.length; index < len; ++index) {
            select.append('<option value="' + options[index].Value + '">' + options[index].Text + '</option>');
        }
    }
    
    Dependency.Clear = function(select) {
        var first = select.children(":first").text();
        
        select.empty();
        select.append('<option value="">' + first + '</option>');
    }
    
    Dependency.ClearAll = function(select) {
        var prefix = GetPrefix(select);
            
        while (select != null){
            Dependency.Clear(select);
            if (select.attr('outsideDependency') == 'outsideDependency'){
                var dependentFieldName = select.attr('dependentFieldName');
                var dependentFieldViewName = select.attr('dependentFieldViewName');
                select = $('#' + prefix + dependentFieldName);
            }
            else{
                select = null;
            }
        }
    }
    
    $(document).ready(function() {
       $("select[outsideDependency='outsideDependency']").change(function() {
            var htmlField = $(this);
            
            var prefix = GetPrefix(htmlField);
            
            var dependentFieldName = htmlField.attr('dependentFieldName');
            var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
            var select = $('#' + prefix + dependentFieldName);
            Dependency.ClearAll(select);
            
            Dependency.Load(dependentFieldViewName, dependentFieldName, htmlField.val(), select);
            
            
        });
    }); 
    
    
    function GetPrefix(element){
        var prefix = null;
        if (element.parents().index($('#DataRowCreate')) >= 0) {
            prefix = createPrefix;
        }
        else if (element.parents().index($('#DataRowEdit')) >= 0) {
            prefix = editPrefix;
        }
        
        return prefix;
    }

    // edit
    var EditDialog; if (!EditDialog) EditDialog = {};

    EditDialog.Open = function(pk) {
        begin();
        
        $('#DataRowEdit').data('title.dialog', '<%= Durados.Web.Localization.Localizer.Translate("Edit Row on")%> ' + gViewDisplayName); 
        $("#DataRowEdit").dialog("option", "buttons", {
          '<%= Durados.Web.Localization.Localizer.Translate("Update")%>': function() { EditDialog.Update(pk); },
          '<%= Durados.Web.Localization.Localizer.Translate("Cancel")%>': function() { $(this).dialog("close"); }
        });
        
        EditDialog.Reset();

        //jquery doesn't remember the position just the size
        var rec = Rectangle.Load(gViewName);
        if(rec != null)
        {
            $('#DataRowEdit').dialog("option", "position", [rec.left,rec.top]);
        
        }
        $('#DataRowEdit').dialog('open');
        
        EditDialog.Load(pk, editPrefix);
         
        complete();
    } 
    
    EditDialog.OpenSelection = function() {
        begin();
        
        $('#DataRowEdit').data('title.dialog', '<%= Durados.Web.Localization.Localizer.Translate("Edit Row on")%> ' + gViewDisplayName); 
        $("#DataRowEdit").dialog("option", "buttons", {
          '<%= Durados.Web.Localization.Localizer.Translate("Update")%>': function() { EditDialog.UpdateSelection(); },
          '<%= Durados.Web.Localization.Localizer.Translate("Cancel")%>': function() { $(this).dialog("close"); }
        });
        
        EditDialog.Reset();
        //jquery doesn't remember the position just the size
        var rec = Rectangle.Load(gViewName);
        if(rec != null)
        {
            $('#DataRowEdit').dialog("option", "position", [rec.left,rec.top]);
        
        }
        $('#DataRowEdit').dialog('open');
        
        complete();
    } 
    
    EditDialog.UpdateSelection = function() {
        
        begin();
        
       
        $.post(gEditSelectionUrl,
        {
            pks: Sys.Serialization.JavaScriptSerializer.serialize(Multi.GetSelection()),
            jsonView : Sys.Serialization.JavaScriptSerializer.serialize(FillJson(GetJsonViewForEdit(), editPrefix))
        },
        function(html) {
            
            
            
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                EditDialog.HandleSuccess(html);
            }
            else {
                EditDialog.ShowFailureMessage(html.replace("$$error$$",""));
            }
        });
    }
    
    EditDialog.Reset = function() {
        $('#EditDataRowForm')[0].reset();
        resetUpload();
    }
    
    EditDialog.Load = function(pk, prefix) {
        
        var jsonView = EditDialog.GetJsonViewValue(pk);

        //delete all class=wysiwyg to clear the textarea
        //$(".wysiwyg").remove();
        $('#EditDataRowForm textarea[rich="true"]').each(function() {$(this).htmlarea("dispose")});
        
        var firstField = null;
        
        for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
            var field = jsonView.Fields[index].Value;
            var htmlField = $('#' + prefix + field.Name);
            if (index==0)
                firstField=htmlField;
                
            if ($(htmlField[0]).attr('upload') == 'upload'){
                if (field.Value != ''){
                    showUpload(field.Name, prefix, field.Value, $('#' + prefix + 'upload_img_' + field.Name).attr('UploadPath') + field.Value);
                }
            }   
            else if ($(htmlField[0]).attr('radioButtons') == 'radioButtons'){
                $('[name=' + prefix + field.Name + '][value=' + field.Value + '' + ']').attr('checked', 'checked');
            }
            else if (htmlField.attr('outsideDependency') == 'outsideDependency'){
                htmlField.val(field.Value);
                var dependentFieldName = htmlField.attr('dependentFieldName');
                var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
                var select = $('#' + prefix + dependentFieldName);
                Dependency.Load(dependentFieldViewName, dependentFieldName, field.Value, select);
            }        
                    
            else if(htmlField[0].type == "textarea")
            {
                htmlField.text(field.Value);
                if (htmlField.attr('rich') == 'true')
                    $('#' + prefix + field.Name).htmlarea(); //create the wysiwyg
            }
            else if(htmlField[0].type == "checkbox")
            	htmlField.attr('checked', field.Value);
        	else if(field.Type == 'Autocomplete'){
                htmlField.attr('valueId', field.Value);
                htmlField.val(field.Default);
            }
			else
                htmlField.val(field.Value);
                
        }
        
        firstField.focus();
    } 
    
    EditDialog.GetJsonViewValue = function(pk) {
        var syncJsonView = null;
        $.ajax({
            url: gGetJsonViewUrl,
            contentType: 'application/json; charset=utf-8',
            data: { viewName: gViewName, dataAction: 'Edit', pk: pk },
            async: false,
            dataType: 'json',
            cache: false,
            error: function(XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
            },
            success: function(jsonView) {
                if (jsonView == null || jsonView == '') {
                    alert("error");
                }
                else {
                    syncJsonView = Sys.Serialization.JavaScriptSerializer.deserialize(jsonView);
                }

            }

        });
        
        return syncJsonView;
    }
    
    EditDialog.Close = function() {
        $('#DataRowEdit').dialog('close');
    } 
    
    EditDialog.Update = function(pk) {
        if (EditDialog.isValid() == false) {
            return;
        }
        
        begin();
        
       
        $.post(gEditUrl,
        {
            pk: pk,
            jsonView : Sys.Serialization.JavaScriptSerializer.serialize(FillJson(GetJsonViewForEdit(), editPrefix))
        },
        function(html) {
            var indexError = html.indexOf("$$error$$", 0);
            var indexMessage = html.indexOf("$$message$$", 0);
            var hasError = indexError > 0 && indexError < 1000;
            var hasMessage = indexMessage > 0 && indexMessage < 1000;
            if (hasError){
                EditDialog.ShowFailureMessage(html.replace("$$error$$",""));
            }
            else if (hasMessage){
                EditDialog.ShowFailureMessage(html.replace("$$message$$",""));
                showProgress();
                refreshView();
                EditDialog.Close();
            }
            else{
                EditDialog.HandleSuccess(html);
            }
            
//            var index = html.indexOf("$$error$$", 0)
//            if (index < 0 || index > 1000) {
//                EditDialog.HandleSuccess(html);
//            }
//            else {
//                EditDialog.ShowFailureMessage(html.replace("$$error$$",""));
//            }
        });
    }
    
    EditDialog.HandleSuccess = function(html) {
        EditDialog.Close();
        $('#ajaxDiv').html(html);
        success();
    }
    
    EditDialog.ShowFailureMessage = function(message) {
        alert(message);
        complete();
    }
    
    EditDialog.isValid = function() {
        
        var form = $('#EditDataRowForm')[0];

        if (Spry.Widget.Form.validate(form) == false) {
            return false;
        }
        return true;
    }
    
    // delete
    var DeleteDialog; if (!DeleteDialog) DeleteDialog = {};

    DeleteDialog.Open = function(pk) {
        $('#DeleteMessage').data('title.dialog', '<%= Durados.Web.Localization.Localizer.Translate("Delete Row from")%> ' + gViewDisplayName); 
        $("#DeleteMessage").dialog("option", "buttons", {
          '<%= Durados.Web.Localization.Localizer.Translate("Delete")%>': function() { DeleteDialog.Delete(pk); },
          '<%= Durados.Web.Localization.Localizer.Translate("Cancel")%>': function() { $(this).dialog("close"); }
        });
        
        $('#DeleteMessage').dialog('open');
    } 
    
    DeleteDialog.OpenSelection = function() {
        $('#DeleteSelectionMessage').data('title.dialog', '<%= Durados.Web.Localization.Localizer.Translate("Delete Selected Rows")%>'); 
        $("#DeleteSelectionMessage").dialog("option", "buttons", {
          '<%= Durados.Web.Localization.Localizer.Translate("Delete")%>': function() { DeleteDialog.DeleteSelection(); },
          '<%= Durados.Web.Localization.Localizer.Translate("Cancel")%>': function() { $(this).dialog("close"); }
        });
        
        $('#DeleteSelectionMessage').dialog('open');
    } 
    
    DeleteDialog.Close = function() {
        $('#DeleteMessage').dialog('close');
    } 
    
    DeleteDialog.Delete = function(pk) {
    
        begin();
        
        $.post(gDeleteUrl,
        {
            pk: pk
        },
        function(html) {
            var index = html.indexOf("$$error$$", 0)
            if (index < 0) {
                DeleteDialog.HandleSuccess(html);
            }
            else {
                DeleteDialog.ShowFailureMessage(html.replace("$$error$$",""));
            }
        });
    }
    
    DeleteDialog.DeleteSelection = function() {
    
        begin();
        
        $.post(gDeleteSelectionUrl,
        {
            pks: Sys.Serialization.JavaScriptSerializer.serialize(Multi.GetSelection())
        },
        function(html) {
            var index = html.indexOf("$$error$$", 0)
            if (index < 0) {
                DeleteDialog.HandleSelectionSuccess(html);
            }
            else {
                DeleteDialog.ShowFailureMessage(html.replace("$$error$$",""));
            }
        });
    }
    
    DeleteDialog.HandleSuccess = function(html) {
        DeleteDialog.Close();
        $('#ajaxDiv').html(html);
        success();
    }
    
    DeleteDialog.HandleSelectionSuccess = function(html) {
        $('#DeleteSelectionMessage').dialog('close');
        $('#ajaxDiv').html(html);
        success();
    }
    
    DeleteDialog.ShowFailureMessage = function(message) {
        alert(message);
        complete();
    }


    function AutoCompleteParse(data)
    {
        var rows = new Array();
        for (var i = 0; i < data.length; i++) {
            rows[i] = { data: data[i].Tag, value: data[i].Tag.PK, result: data[i].Tag.Name };
        }
        return rows;
    }
    
    function AutoCompleteResult(event, item)
    {
        $(event.currentTarget).attr('valueId',item.PK);
    }
   
    
    //filter
    var FilterForm; if (!FilterForm) FilterForm = function(){this.JsonFilter = GetJsonFilter();};
    
    FilterForm.Apply = function(clear) {
        showProgress();
        
        if(!clear)
            this.JsonFilter = FillJson(GetJsonFilter(), 'filter_');
        else
            this.JsonFilter = GetJsonFilter();
        
        $.post(gFilterUrl,
        {
            jsonFilter : Sys.Serialization.JavaScriptSerializer.serialize(this.JsonFilter)
        },
        function(html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                FilterForm.HandleSuccess(html, clear);
            }
            else {
                FilterForm.ShowFailureMessage(html.replace("$$error$$",""));
            }
        });
    }
    
    FilterForm.HandleSuccess = function(html, clear) {
        $("#ajaxDiv").html(html);
  
        if(!clear)
        {
            success();
        }
        else
            complete();
    }
    
    FilterForm.ShowFailureMessage = function(message) {
        complete();
        alert(message);
    }
    
    
    var prevSortedColumn = '';
    var prevSortedDirection = '';
    
    FilterForm.Sort = function(id){
        showProgress();
        
        var SortColumn, direction;
        
        var a = $('#'+id);
        SortColumn = a.attr('SortColumn');
        direction = SortColumn == prevSortedColumn && prevSortedDirection == 'Asc' ? "Desc" : "Asc";
        
        $.post(gIndexUrl,
        {
            SortColumn : SortColumn, direction: direction
        },
        function(html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                FilterForm.HandleSuccess(html, false);
                prevSortedColumn = SortColumn;
                prevSortedDirection = direction;
            }   
            else {
                FilterForm.ShowFailureMessage(html.replace("$$error$$",""));
            }
        });
    }

//Inline Adding
    var currentDialog = null;
    var CreateDialog; if (!CreateDialog) CreateDialog = function(){};
    
    CreateDialog.AddNewCurrentDialog =  function(initiator, div) {
        var dialog = { Initiator: initiator, Div: div };
        return dialog;
    }
    
    CreateDialog.CreateAndOpen = function(viewName, viewDisplay, type, id, createUrl){
        showProgress();
        var html = CreateDialog.Create(viewName);
        
//        var div = $("<div>");
        var div = $('#inlineAdding');
        div.html(html);
//        $("body").prepend(div);
        $('#InlineAddingTabs').tabs({ fx: { opacity: 'toggle', duration: 'fast' } }); // first tab selected
        

        currentDialog = CreateDialog.AddNewCurrentDialog(currentDialog, div);
        CreateDialog.Open(viewName, viewDisplay, type, id, createUrl);
        
        //Create wysiwyg for all textareas
        CreateDialog.CreateWysiwyg();

        complete();
    }
    
    CreateDialog.Create = function(viewName) {
        var syncHtml = '';
        
        $.ajax({
            url: gInlineAddingDialogUrl + viewName,
            contentType: 'application/json; charset=utf-8',
            data: { viewName: viewName },
            async: false,
            cache: false,
            error: function(XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
            },
            success: function(html) {
                hideProgress();
                var index = html.indexOf("$$error$$", 0)
                if (index < 0 || index > 1000) {
                    syncHtml = html;
                }
                else {
                    CreateDialog.ShowFailureMessage(html.replace("$$error$$",""));
                }
            }

        });
        
        
        return syncHtml;
    }
    
    CreateDialog.Open = function(viewName, viewDisplay, type, id, createUrl) {
    
        var winWidth = $(window).width();
        var winHeight = $(window).height();
          
        var rec = Rectangle.Load(gViewName);
        var dialog = currentDialog.Div;
        
        if(rec != null)
        {
             dialog.dialog({
                bgiframe: true,
                autoOpen: false,
                position: [rec.left,rec.top],
                width: rec.width,
                height: rec.height,
                modal: true,
                close: CreateDialog.CloseEvent,
                position: 'center',
                resizeStop: SaveDialogOnResize,
                dragStop: SaveDialogOnDrag

            });
        
        }
        else{
            dialog.dialog({
                bgiframe: true,
                autoOpen: false,
                position: 'center',
                width: (winWidth *.75),
                modal: true,
                close: CreateDialog.CloseEvent,
                position: 'center',
                resizeStop: SaveDialogOnResize,
                dragStop: SaveDialogOnDrag
            });
        }
        
        dialog.data('title.dialog', 'Add Row to ' + viewDisplay); 
        dialog.dialog("option", "buttons", {
          "Add": function() { CreateDialog.Add(viewName, false, type, id, createUrl); },
          "Add Another": function() { CreateDialog.Add(viewName, true, type, id, createUrl); },
          "Cancel": function() { $(this).dialog("close"); }
        });
        
        showProgress();
        AddDialog.Close();
        dialog.dialog('open');

//        if(currentDialog.Initiator.Div == null){
//            CreateDialog.SetDefaultsFromFilter(jsonFilter, createPrefix);
//        }
        //Create wysiwyg for all textareas
        CreateDialog.CreateWysiwyg(viewName);
        hideProgress();
    } 
    
    CreateDialog.SetDefaultsFromFilter = function(json, prefix) {
        if (json!=null){
            for (var index = 0, len = json.Fields.length; index < len; ++index) {
                var field = json.Fields[index].Value;
                var htmlField = $('#' + prefix + field.Name);
                if (field.Value != null && field.Value != ''){
                    
                    if ($(htmlField[0]).attr('upload') == 'upload'){
                        showUpload(field.Name, prefix, field.Value, $('#' + prefix + 'upload_img_' + field.Name).attr('UploadPath') + field.Value);
                    }        
                    if ($(htmlField[0]).attr('radioButtons') == 'radioButtons'){
                        $('[name=' + prefix + field.Name + '][value=' + field.Value + '' + ']').attr('checked', 'checked');
                    }        
                    else if(htmlField[0] != undefined && htmlField[0].type == "textarea")
                    {
                        htmlField.text(field.Value);
                        if (htmlField.attr('rich') == 'true')
                            $('#' + prefix + field.Name).htmlarea(); //create the wysiwyg

                    }
                    else if(htmlField[0] != undefined && htmlField[0].type == "checkbox")
            	        htmlField.attr('checked', field.Value);
        	        else if(field.Type == 'Autocomplete'){
                        htmlField.attr('valueId', field.Value);
                        htmlField.val(field.Default);
                    }
			        else
                        htmlField.val(field.Value);
                } 
            }
        }
    }
    
    CreateDialog.CreateWysiwyg = function() {
        $('#DataRowInlineAddingForm textarea[rich="true"]').each(function() {$(this).htmlarea("dispose")});
        $('#DataRowInlineAddingForm textarea[rich="true"]').htmlarea();
    } 
    
    CreateDialog.Reset = function() {
        $('#DataRowInlineAddingForm')[0].reset();
        resetUpload();
    }
    
    
    
    CreateDialog.Close = function() {
        //$('#DataRowInlineAdding').dialog('close');
        currentDialog.Div.dialog('close');
    } 
    
    CreateDialog.CloseEvent = function() {
        setTimeout('AddDialog.Show()',1);

    }
    
    CreateDialog.Add = function(viewName, another, type, id, createUrl) {
        if (CreateDialog.isValid() == false) {
            return;
        }
        showProgress();
        
        $.post(createUrl+viewName,
        {
            jsonView : Sys.Serialization.JavaScriptSerializer.serialize(FillJson(GetJsonViewForInlineAdding(viewName), inlineAddingPrefix))
        },
        function(jsonDisplayValue) {
            hideProgress();
            var index = jsonDisplayValue.indexOf("$$error$$", 0);
            if (index < 0 || index > 1000) {
                var displayValue = Sys.Serialization.JavaScriptSerializer.deserialize(jsonDisplayValue);
                displayValue = Sys.Serialization.JavaScriptSerializer.deserialize(displayValue);

                CreateDialog.HandleSuccess(another, type, id, displayValue);
            }
            else {
                CreateDialog.ShowFailureMessage(jsonDisplayValue.replace("$$error$$",""));
            }
        });
    }
    
    CreateDialog.onafterInlineAdding = function(){}

    
    CreateDialog.HandleSuccess = function(another, type, id, displayValue) {
        if (type=='DropDown'){
            var field = displayValue.Fields[0].Value;
            var value = field.Value;
            var text = field.Default;
            var select = $('#'+id);
            select.append('<option value="' + value + '">' + text + '</option>');
            var selectEdit = $('#'+id.replace(createPrefix, editPrefix));
            selectEdit.append('<option value="' + value + '">' + text + '</option>');
            $(CreateDialog).trigger('onafterInlineAdding', select);

        }
        
        if (another){
           CreateDialog.Reset();
        }
        else{
            CreateDialog.Close();
//            AddDialog.Show();
        }
        
        if (type=='DropDown'){
            var select = $('#'+id);
            select.val(value);
        }
    }
    
    CreateDialog.isValid = function() {
        
        var form = $('#DataRowInlineAddingForm')[0];

        if (Spry.Widget.Form.validate(form) == false) {
            return false;
        }
        return true;
    }
    
    
    
    
    CreateDialog.ShowFailureMessage = function(message) {
        complete();
        alert(message);
    }


    //Inline Adding
    var InlineAdding; if (!InlineAdding) InlineAdding = function(){};
    
    InlineAdding.CreateAndOpen = function(viewName, viewDisplay, type, id){
        showProgress();
        var html = InlineAdding.Create(viewName);
        $("#DataRowInlineAdding").html(html);
        InlineAdding.Open(viewName, viewDisplay, type, id);
        hideProgress();
    }
    
    InlineAdding.Create = function(viewName) {
        var syncHtml = '';
        
        $.ajax({
            url: gInlineAddingDialogUrl + viewName,
            contentType: 'application/json; charset=utf-8',
            data: { viewName: viewName },
            async: false,
            cache: false,
            error: function(XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
            },
            success: function(html) {
                hideProgress();
                var index = html.indexOf("$$error$$", 0)
                if (index < 0 || index > 1000) {
                    syncHtml = html;
                }
                else {
                    InlineAdding.ShowFailureMessage(html.replace("$$error$$",""));
                }
            }

        });
        
        
        return syncHtml;
    }
    
    InlineAdding.Open = function(viewName, viewDisplay, type, id) {
        var dialog = $('#DataRowInlineAdding');
        dialog.data('title.dialog', 'Add Row to ' + viewDisplay); 
        dialog.dialog("option", "buttons", {
          "Add": function() { InlineAdding.Add(viewName, false, type, id); },
          "Add Another": function() { InlineAdding.Add(viewName, true, type, id); },
          "Cancel": function() { $(this).dialog("close"); }
        });
        
        AddDialog.Close();
        dialog.dialog('open');

        //Create wysiwyg for all textareas
        InlineAdding.CreateWysiwyg(viewName);
    } 

    InlineAdding.CreateWysiwyg = function(viewName) {
    
        $('#DataRowInlineAddingForm textarea[rich="true"]').each(function() {$(this).htmlarea("dispose")});

        //loops on all textarea and enable wysiwyg
        var json = GetJsonViewForInlineAdding(viewName);
        for (var index = 0, len = json.Fields.length; index < len; ++index) {
            var field = json.Fields[index].Value;
            if($('#' + inlineAddingPrefix + field.Name)[0].type == "textarea" && $('#' + inlineAddingPrefix + field.Name).attr('rich')=='true')
            {
                $('#' + inlineAddingPrefix + field.Name).htmlarea();    
            }
        }
        
        gInlineAddingCreateUrl = json.InlineAddingCreateUrl;
    } 
    
    InlineAdding.Reset = function() {
        $('#DataRowInlineAddingForm')[0].reset();
        resetUpload();
    }
    
    InlineAdding.Close = function() {
        $('#DataRowInlineAdding').dialog('close');

    } 
    
    InlineAdding.CloseEvent = function() {
        setTimeout('AddDialog.Show()',1);

    }
    
    InlineAdding.Add = function(viewName, another, type, id) {
        if (InlineAdding.isValid() == false) {
            return;
        }
        showProgress();
        
        $.post(gInlineAddingCreateUrl+viewName,
        {
            jsonView : Sys.Serialization.JavaScriptSerializer.serialize(FillJson(GetJsonViewForInlineAdding(viewName), inlineAddingPrefix))
        },
        function(jsonDisplayValue) {
            hideProgress();
            var index = jsonDisplayValue.indexOf("$$error$$", 0);
            if (index < 0 || index > 1000) {
                var displayValue = Sys.Serialization.JavaScriptSerializer.deserialize(jsonDisplayValue);
                displayValue = Sys.Serialization.JavaScriptSerializer.deserialize(displayValue);

                InlineAdding.HandleSuccess(another, type, id, displayValue);
            }
            else {
                InlineAdding.ShowFailureMessage(jsonDisplayValue.replace("$$error$$",""));
            }
        });
    }
    
    
    InlineAdding.HandleSuccess = function(another, type, id, displayValue) {
        if (type=='DropDown'){
            var field = displayValue.Fields[0].Value;
            var value = field.Value;
            var text = field.Default;
            var select = $('#'+id);
            select.append('<option value="' + value + '">' + text + '</option>');
        }
        
        if (another){
            $('#DataRowInlineAddingForm')[0].reset();
        }
        else{
            InlineAdding.Close();
//            AddDialog.Show();
        }
        
        if (type=='DropDown'){
            var select = $('#'+id);
            select.val(value);
        }
    }
    
    InlineAdding.isValid = function() {
        
        var form = $('#DataRowInlineAddingForm')[0];

        if (Spry.Widget.Form.validate(form) == false) {
            return false;
        }
        return true;
    }
    
    
    
    
    InlineAdding.ShowFailureMessage = function(message) {
        complete();
        alert(message);
    }
    
    
    /// multi
    var Multi; if (!Multi) Multi = function(){};
   
    Multi.All = function() {
        $('.Multi').attr('checked','checked');
    } 
    
    Multi.Clear = function() {
        $('.Multi').attr('checked','');
    } 
    
    Multi.GetSelection = function(){
        var selection = new Array();
        
        $(".Multi:checked").each(function() {
            var pk = $(this).attr("pk");
            selection.push(pk);
        });

        
        return selection;
    } 
    
    function isChecked(checkbox){
        return checkbox.attr('checked') == 'checked';
    }
    
    function getCheckedQueryStringValue(checkbox){
        if (isChecked(checkbox))
            return '1';
        else
            return '0';
    }
    
    function Print(){
        $("#ajaxDiv").printElement();
    }
    
    function rightTrim(sString) 
    {
        while (sString.substring(sString.length-1, sString.length) == '&')
        {
            sString = sString.substring(0,sString.length-1);
        }
        return sString;
    }
   
    $(document).ready(function() {   
        $(function() {
            success();
	    });
	});
	
	
	function success(){
	    complete();
	    //FilterForm.Update();
    }
    
    function begin(){
        showProgress();
    }
    
    function complete(){
    	$(".date").datepicker({ showOn: 'button', buttonImage: '<%= Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/smoothness/images/calendar.jpg"%>', buttonImageOnly: true, dateFormat: gDateFormat });
    	Autocomplete.Init();
        hideProgress();
    }

    function showProgress(){
        setTimeout('$("#ProgressionDiv").css("display", "block"); $("body").css("cursor","wait");', 50);
    }
    
    function hideProgress(){
        setTimeout('$("#ProgressionDiv").css("display", "none"); $("body").css("cursor","default");', 100);
    }

    //Language Functions
    function SetLanguage() {
        var language = $('#languageDropDown').val();
        SetSelectedLanguage(language);
    }

    function SetSelectedLanguage(language) {
        var url = gSetLanguageUrl+'?languageCode='+language;
        top.location.href = url;
    }

    //Menu Functions
    function displayMenu(id, imgID) {
        if (isDisplayed(id))
            HideMenu(id, imgID);
        else
            ShowMenu(id, imgID);
    }
    function isDisplayed(id) {
        return $('#' + id).css('display') == 'block';
    }
    function ShowMenu(id, imgID) {
        $('#' + id).css('display', 'block');
        $('#' + imgID).attr('src', '<%= Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/Minus.JPG"%>');
    }

    function HideMenu(id, imgID) {
        $('#' + id).css('display', 'none');
        $('#' + imgID).attr('src', '<%= Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/Plus.JPG"%>');
    }
</script>

</asp:Content>

