$.expr[":"].containsExact = function (obj, index, meta, stack) {
    return (obj.textContent || obj.innerText || $(obj).text() || "") == meta[3];
};

function GoBack() {
    window.history.back();
}

String.prototype.capitalize = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
}

if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (str) {
        return !this.indexOf(str);
    }
}

function getBrowser() {
    var browser = '';

    jQuery.each(jQuery.browser, function (i, val) {
        browser = browser + i + " " + val + " ";
    });
    return browser;
}

function getUserRole() {
    return $('span#user-role').attr('val');
}
function isUserAdmin()
{
    var userrole = getUserRole()
    return userrole == "Developer" || userrole == "Admin"
}
function isIE() {
    return $.browser.msie;
}

function queryString(key) {
    return queryString2(window.location.href, key);
}

function queryString2(url, key) {
    var vars = [], hash;
    var hashes = url.slice(url.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars[key];
}


var createPrefix = 'create_';
var editPrefix = 'edit_';
var filterPrefix = 'filter_';
var inlineAddingPrefix = 'inlineAdding_';
var inlineEditingPrefix = 'inlineEditing_';
var currentViewName = '';
var TOKEN = '&&%&';

var Durados;
if (!Durados) Durados = {
    DuringdLoadIframe: false
};

if (!Durados.View) Durados.View = function (
        guid,
        ViewName,
        gCreateUrl,
        gEditUrl,
        gEditSelectionUrl,
        gDuplicateUrl,
        gGetJsonViewUrl,
        gGetRichUrl,
        gEditRichUrl,
        gGetSelectListUrl,
        gGetJsonViewInlineAddingUrl,
        gGetJsonViewInlineEditingUrl,
        gInlineAddingDialogUrl,
        gInlineEditingDialogUrl,
        gInlineDuplicateDialogUrl,
        gInlineSearchDialogUrl,
        gDeleteUrl,
        gDeleteSelectionUrl,
        gFilterUrl,
        gSetLanguageUrl,
        gIndexUrl,
        gExportToCsvUrl,
        gPrintUrl,
        gAutoCompleteUrl,
        gViewDisplayName,
        gViewName,
        jsonViewForCreate,
        gUploadUrl,
        JsonViewForEdit,
        JsonFilter,
        Controller,
        TabCache,
        RefreshOnClose,
        gRefreshUrl,
        mainPage,
        MultiSelect,
        Popup,
        Mobile,
        AllowEdit,
        AddItemUrl,
        EditItemUrl,
        DuplicateItemUrl,
        InsertItemUrl,
        DuplicationMethod,
        DuplicateMessage,
        PageSize,
        DisplayType,
        PromoteButton,
        addTitle,
        dupTitle,
        ShowUpDown,
        EditOnlyUrl,
        RowColorColumnName,
        HasOpenRules,
        MaxSubGridHeight,
        AllowCreate,
        WorkFlowStepsFieldName,
        showDisabledSteps,
        ReloadPage,
        allFilterValuesUrl,
        DataDisplayType,
        GridDisplayType,
        InsideTextSearch,
        Role,
        OpenDialogMax,
        SqlProduct) {
    // global from server
    this.guid = guid;
    this.ViewName = ViewName;
    this.gCreateUrl = gCreateUrl;
    this.gEditUrl = gEditUrl;
    this.gEditSelectionUrl = gEditSelectionUrl;
    this.gDuplicateUrl = gDuplicateUrl;
    this.gGetJsonViewUrl = gGetJsonViewUrl;
    this.gGetRichUrl = gGetRichUrl;
    this.gEditRichUrl = gEditRichUrl;
    this.gGetSelectListUrl = gGetSelectListUrl;

    this.gGetJsonViewInlineAddingUrl = gGetJsonViewInlineAddingUrl;
    this.gGetJsonViewInlineEditingUrl = gGetJsonViewInlineEditingUrl;
    this.gInlineAddingDialogUrl = gInlineAddingDialogUrl;
    this.gInlineEditingDialogUrl = gInlineEditingDialogUrl;
    this.gInlineDuplicateDialogUrl = gInlineDuplicateDialogUrl;
    this.gInlineSearchDialogUrl = gInlineSearchDialogUrl;
    this.gDeleteUrl = gDeleteUrl;
    this.gDeleteSelectionUrl = gDeleteSelectionUrl;
    this.gFilterUrl = gFilterUrl;
    this.gSetLanguageUrl = gSetLanguageUrl;
    this.gIndexUrl = gIndexUrl;
    this.gExportToCsvUrl = gExportToCsvUrl;
    this.gPrintUrl = gPrintUrl;
    this.gAutoCompleteUrl = gAutoCompleteUrl;
    this.gViewDisplayName = gViewDisplayName;
    this.gViewName = gViewName;
    this.allFilterValuesUrl = allFilterValuesUrl;

    this.jsonViewForCreate = jsonViewForCreate;

    // upload 
    this.gUploadUrl = gUploadUrl;


    this.jsonViewForEdit = JsonViewForEdit;
    this.jsonFilter = JsonFilter;
    this.Controller = Controller;
    this.TabCache = TabCache;
    this.RefreshOnClose = RefreshOnClose;
    this.gRefreshUrl = gRefreshUrl;
    this.mainPage = mainPage;
    this.MultiSelect = MultiSelect == "True";
    this.Popup = Popup == "True";
    this.Mobile = Mobile == "True";
    this.AllowEdit = AllowEdit == "True";
    this.AddItemUrl = AddItemUrl;
    this.EditItemUrl = EditItemUrl;
    this.DuplicateItemUrl = DuplicateItemUrl;
    this.InsertItemUrl = InsertItemUrl;
    this.DuplicationMethod = DuplicationMethod;
    this.DuplicateMessage = DuplicateMessage;
    this.PageSize = PageSize;
    this.DisplayType = DisplayType;
    this.PromoteButton = PromoteButton;
    this.addTitle = addTitle;
    this.dupTitle = dupTitle;
    this.ShowUpDown = ShowUpDown == "True";
    this.EditOnlyUrl = EditOnlyUrl;
    this.RowColorColumnName = RowColorColumnName;
    this.HasOpenRules = HasOpenRules == "True";
    this.MaxSubGridHeight = MaxSubGridHeight;
    this.AllowCreate = AllowCreate == "True";
    this.WorkFlowStepsFieldName = WorkFlowStepsFieldName;
    this.showDisabledSteps = showDisabledSteps == "True";
    this.ReloadPage = ReloadPage;
    this.DataDisplayType = DataDisplayType;
    this.GridDisplayType = GridDisplayType;
    this.InsideTextSearch = InsideTextSearch == "True";
    this.Role = Role;
    this.OpenDialogMax = OpenDialogMax;
    this.SqlProduct = SqlProduct;

    this.validations = [];
    this.tab = null;
    this.disabledRows = [];
    this.sortDashboard = [];
}

Durados.View.prototype.constructor = Durados.View;

Durados.View.prototype.guid = function () {
    return this.guid;
}

Durados.View.prototype.ViewName = function () {
    return this.ViewName;
}

Durados.View.prototype.gCreateUrl = function () {
    return this.gCreateUrl;
}

Durados.View.prototype.gEditUrl = function () {
    return this.gEditUrl;
}

Durados.View.prototype.gEditSelectionUrl = function () {
    return this.gEditSelectionUrl;
}

Durados.View.prototype.gDuplicateUrl = function () {
    return this.gDuplicateUrl;
}

Durados.View.prototype.gGetJsonViewUrl = function () {
    return this.gGetJsonViewUrl;
}

Durados.View.prototype.gGetRichUrl = function () {
    return this.gGetRichUrl;
}

Durados.View.prototype.gEditRichUrl = function () {
    return this.gEditRichUrl;
}

Durados.View.prototype.gGetSelectListUrl = function () {
    return this.gGetSelectListUrl;
}

Durados.View.prototype.gGetJsonViewInlineAddingUrl = function () {
    return this.gGetJsonViewInlineAddingUrl;
}

Durados.View.prototype.gGetJsonViewInlineEditingUrl = function () {
    return this.gGetJsonViewInlineEditingUrl;
}

Durados.View.prototype.gInlineAddingDialogUrl = function () {
    return this.gInlineAddingDialogUrl;
}

Durados.View.prototype.gInlineEditingDialogUrl = function () {
    return this.gInlineEditingDialogUrl;
}

Durados.View.prototype.gInlineDuplicateDialogUrl = function () {
    return this.gInlineDuplicateDialogUrl;
}

Durados.View.prototype.gInlineSearchDialogUrl = function () {
    return this.gInlineSearchDialogUrl;
}

Durados.View.prototype.gDeleteUrl = function () {
    return this.gDeleteUrl;
}

Durados.View.prototype.gDeleteSelectionUrl = function () {
    return this.gDeleteSelectionUrl;
}

Durados.View.prototype.gFilterUrl = function () {
    return this.gFilterUrl;
}

Durados.View.prototype.gSetLanguageUrl = function () {
    return this.gSetLanguageUrl;
}

Durados.View.prototype.gIndexUrl = function () {
    return this.gIndexUrl;
}

Durados.View.prototype.gExportToCsvUrl = function () {
    return this.gExportToCsvUrl;
}

Durados.View.prototype.gPrintUrl = function () {
    return this.gPrintUrl;
}

Durados.View.prototype.gAutoCompleteUrl = function () {
    return this.gAutoCompleteUrl;
}

Durados.View.prototype.gViewDisplayName = function () {
    return this.gViewDisplayName;
}

Durados.View.prototype.gViewName = function () {
    return this.gViewName;
}

Durados.View.prototype.jsonViewForCreate = function () {
    return this.jsonViewForCreate;
}

Durados.View.prototype.gUploadUrl = function () {
    return this.gUploadUrl;
}

Durados.View.prototype.GetJsonViewForEdit = function () {
    return this.jsonViewForEdit;
}

Durados.View.prototype.GetJsonFilter = function () {
    return this.jsonFilter;
}

Durados.View.prototype.Controller = function () {
    return this.Controller;
}

Durados.View.prototype.TabCache = function () {
    return this.TabCache;
}

Durados.View.prototype.RefreshOnClose = function () {
    return this.RefreshOnClose;
}

Durados.View.prototype.gRefreshUrl = function () {
    return this.gRefreshUrl;
}

Durados.View.prototype.mainPage = function () {
    return this.mainPage;
}

Durados.View.prototype.MultiSelect = function () {
    return this.MultiSelect;
}

Durados.View.prototype.Popup = function () {
    return this.Popup;
}

Durados.View.prototype.Mobile = function () {
    return this.Mobile;
}

Durados.View.prototype.AddItemUrl = function () {
    return this.AddItemUrl;
}

Durados.View.prototype.EditItemUrl = function () {
    return this.EditItemUrl;
}

Durados.View.prototype.DuplicateItemUrl = function () {
    return this.DuplicateItemUrl;
}

Durados.View.prototype.InsertItemUrl = function () {
    return this.InsertItemUrl;
}

Durados.View.prototype.DuplicationMethod = function () {
    return this.DuplicationMethod;
}

Durados.View.prototype.DuplicateMessage = function () {
    return this.DuplicateMessage;
}


Durados.View.prototype.PageSize = function () {
    return this.PageSize;
}

Durados.View.prototype.DisplayType = function () {
    return this.DisplayType;
}

Durados.View.prototype.addTitle = function () {
    return this.addTitle;
}

Durados.View.prototype.PromoteButton = function () {
    return this.PromoteButton;
}

Durados.View.prototype.dupTitle = function () {
    return this.dupTitle;
}

Durados.View.prototype.ShowUpDown = function () {
    return this.ShowUpDown;
}

Durados.View.prototype.EditOnlyUrl = function () {
    return this.EditOnlyUrl;
}

Durados.View.prototype.RowColorColumnName = function () {
    return this.RowColorColumnName;
}

Durados.View.prototype.HasOpenRules = function () {
    return this.HasOpenRules;
}

Durados.View.prototype.MaxSubGridHeight = function () {
    return this.MaxSubGridHeight;
}
Durados.View.prototype.AllowCreate = function () {
    return this.AllowCreate;
}
Durados.View.prototype.WorkFlowStepsFieldName = function () {
    return this.WorkFlowStepsFieldName;
}

Durados.View.prototype.showDisabledSteps = function () {
    return this.showDisabledSteps;
}

Durados.View.prototype.Role = function () {
    return this.Role;
}

Durados.View.prototype.ReloadPage = function () {
    return this.ReloadPage;
}

Durados.View.prototype.HasWorkFlowSteps = function () {
    return this.WorkFlowStepsFieldName != "";
}

Durados.View.prototype.validations = function () {
    return this.validations;
}

Durados.View.prototype.getTab = function () {
    return this.tab;
}

Durados.View.prototype.setTab = function (tab) {
    return this.tab = tab;
}

Durados.View.prototype.setTab = function (tab) {
    return this.tab = tab;
}

Durados.View.prototype.updateCounter = function () {
    if (this.tab != null) {
        var tabRowCounter = this.tab.find('span[hasCounter="hasCounter"]');
        if (tabRowCounter.length == 1) {
            // alegro
            //var tableRowCounter = $('#' + this.guid + 'rowCount').text();

            var container = $('#' + this.guid + 'ajaxDiv').find('table.gridview');

            //if (container.length == 0) container = $('#' + this.guid + 'ajaxDiv div.gridboard').first();

            var tableRowCounter = container.attr('rowCount');

            tabRowCounter.text(tableRowCounter);
        }
    }
}



var views = [];

var jsonViewsForInlineDialog = {};



function UploadFile(id, prefix, guid, viewName) {
    if (!viewName) viewName = $('#' + guid + prefix + ReplaceNonAlphaNumeric(id)).attr('viewName');
    if ($('#' + guid + prefix + 'upload_' + ReplaceNonAlphaNumeric(id)).length == 0)
        return;

    var txt = $('#' + guid + prefix + 'upload_span_' + ReplaceNonAlphaNumeric(id)).text();

    var url = '';
    if (guid == null || guid == 'null' || views[guid] == null) {
        if (viewName == "durados_App")
            url = '/MultiTenancy/Upload/';
        else
            url = '/Admin/Upload/';
    }
    else {
        url = views[guid].gUploadUrl;
    }

    var uploader = new Ajax_upload(guid + prefix + 'upload_' + id, {
        action: url,
        data: { viewName: viewName, fieldName: id },
        onSubmit: function (file, ext) {
            showProgress();
            $('#' + guid + prefix + 'upload_span_' + ReplaceNonAlphaNumeric(id)).text('Uploading ' + file);
            //this.disable();
        },
        onComplete: function (file, response) {
            hideProgress();
            if (response.indexOf('Upload failed: ') > -1) {
                $('#' + guid + prefix + 'upload_span_' + ReplaceNonAlphaNumeric(id)).text(txt);
                modalErrorMsg(response);
            } else {
                var deresponse = decodeURIComponent(response);
                var arr = decodeURIComponent(deresponse).split('|');
                if (decodeURIComponent(response).indexOf("__filename__") > -1 && arr[0].split('//')[1]) {
                    showUpload(id, prefix, arr[2], '/' + arr[0].split('//')[1] + '?' + arr[1], guid);
                    if (richUploadDiv) {
                        richUploadDiv.find("#" + guid + prefix + id).val(arr[2]);

                    }
                }
                else {
                    showUpload(id, prefix, arr[1], arr[0], guid);
                    if (richUploadDiv) {
                        var fullUrl = $('#' + guid + prefix + id).attr('fullUrl') == 'yes';
                        if (fullUrl)
                            richUploadDiv.find("#" + guid + prefix + id).val(arr[0]);
                        else
                            richUploadDiv.find("#" + guid + prefix + id).val(arr[1]);

                    }
                }
            }
        }
    });


    //    var uploader = new qq.FileUploader({
    //        // pass the dom node (ex. $(selector)[0] for jQuery users)
    //        element: document.getElementById(guid + prefix + 'upload_' + id),
    //        // path to server-side upload script
    //        sizeLimit: 0, // max size   
    //        minSizeLimit: 0, // min size
    //        action: views[guid].gUploadUrl,
    //        params: { viewName: views[guid].gViewName, fieldName: id },
    //        onSubmit: function(id2, fileName) {
    //            $('#' + guid + prefix + 'upload_span_' + id).text('Uploading ' + fileName);
    //            //this.disable();
    //        },
    //        onProgress: function(id2, fileName, loaded, total){},
    //        onComplete: function(id2, fileName, responseJSON) {
    //            var arr = responseJSON.split('?');
    //            showUpload(id, prefix, arr[1], arr[0], guid);
    //        },
    //        onCancel: function(id2, fileName){}

    //    });

    return uploader;
}

function showUpload(id, prefix, file, src, guid) {
    if (file == null)
        file = '';

    var fullUrl = $('#' + guid + prefix + id).attr('fullUrl') == 'yes';

    id = ReplaceNonAlphaNumeric(id);
    $('#' + guid + prefix + 'upload_span_' + id).text($('#' + guid + prefix + 'upload_span_' + id).parent().children('input').attr('upload'));
    $('#' + guid + prefix + id).val(file);

    //by br
    //    $('#' + guid + prefix + id).text(file);

    $('#' + guid + prefix + id).attr('d_file', file);
    if (src && src.indexOf("__filename__") > -1) {
        if (file) {
            src = src.replace('__filename__', encodeURI(file));
            $('#' + guid + prefix + 'upload_img_' + id).attr('src', src);
            $('#' + guid + prefix + 'upload_img_' + id).css('visibility', 'visible');
        }
        else {
            $('#' + guid + prefix + 'upload_img_' + id).attr('src', '');
            $('#' + guid + prefix + 'upload_img_' + id).css('visibility', 'hidden');
        }
    }
    else if (src && src.indexOf(".") > -1) { //src.substring(src.length - 1) != "/"
        src = src.replace('\\', '/'); //.replace('//', '/');
        src = src.replace('\\', '/'); //.replace('//', '/');
        $('#' + guid + prefix + 'upload_img_' + id).attr('src', src);
        $('#' + guid + prefix + 'upload_img_' + id).css('visibility', 'visible');
    }
    else {
        $('#' + guid + prefix + 'upload_img_' + id).css('visibility', 'hidden');
    }

    if (fullUrl && file != '') {
        $('#' + guid + prefix + id).val(src);
        $('#' + guid + prefix + id).attr('d_file', src);
    }

    var downloadIcon = $('#' + guid + prefix + 'DownloadIcon_' + id);

    if (downloadIcon.length > 0) {
        if (!file) {
            downloadIcon.hide();
        }
        else {
            downloadIcon.show();
            var href = downloadIcon.attr('d_href');
            downloadIcon.attr('href', href.replace('__filename__', encodeURI(file)));
        }
    }

    var deleteIcon = $('#' + guid + prefix + 'DeleteIcon_' + id);
    if (deleteIcon.length > 0) {
        if (!file) {
            deleteIcon.hide();
        }
        else {
            deleteIcon.css('visibility', 'visible');
            deleteIcon.show();

        }
    }
}

function resetUpload() {
    var d = $('div.uploadDiv');
    d.find('span').text('Upload');
    d.find('img').attr('src', '').css('visibility', 'hidden');
    d.find('input').val('');
}

function isUploadDialog(id) {
    var input = $('#' + id.replace('_create_', '_inlineEditing__'));
    if (input.length == 0)
        return false;
    return input.parent().parent().hasClass('ui-dialog-content');
}

function performOk(id) {
    var input = $('#' + id.replace('_create_', '_inlineEditing__'));
    if (input.length == 0)
        return;
    input.parent().parent().parent().find('button:contains("' + translator.ok + '")').click();
}

function DeleteFile(url, id, name, guid, prefix) {
    Durados.Dialogs.Confirm(translator.DeleteFileTitle, translator.DeleteFileMessage, function () { DeleteFile2(url, id, name, guid, prefix); if (isUploadDialog(id)) { performOk(id); } }, null);
}

function DeleteFile2(url, id, name, guid, prefix) {
    name = ReplaceNonAlphaNumeric(name);
    var filename = $('#' + id).val();
    if (filename == null || filename == '')
        filename = $('#' + id.replace('_create_', '_inlineEditing__')).val();
    //guid_inlineEditing__az1
    url = url + filename;
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            if (json == 'Success') {
                $('#' + id).val('');
                $('#' + guid + prefix + 'DownloadIcon_' + name).hide();
                $('#' + guid + prefix + 'DeleteIcon_' + name).hide();
                $('#' + guid + prefix + "upload_" + name).find('img:first').css('visibility', 'hidden');

                $('#' + id.replace('_create_', '_inlineEditing__')).val('');
                $('#' + guid + 'inlineEditing__' + 'DownloadIcon_' + name).hide();
                $('#' + guid + 'inlineEditing__' + 'DeleteIcon_' + name).hide();
                $('#' + guid + 'inlineEditing__' + "upload_" + name).find('img:first').css('visibility', 'hidden');
            }
            else {
                ajaxNotSuccessMsg(json);
            }

        }

    });
}

function GetJsonViewForInlineAdding(viewName, guid, addingUrl) {
    var syncJsonView = null;
    var url = '';
    if (!guid || guid == 'null') {
        if (addingUrl == null)
            addingUrl = '/Home/GetJsonView/';
        url = addingUrl;
    }
    else {
        url = views[guid].gGetJsonViewInlineAddingUrl;
    }
    $.ajax({
        url: url + viewName,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: viewName, dataAction: 'Create', guid: guid },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (jsonView) {
            if (jsonView == null || jsonView == '') {
                ajaxNotSuccessMsg('');
            }
            else {
                syncJsonView = Sys.Serialization.JavaScriptSerializer.deserialize(jsonView);
            }

        }

    });

    return syncJsonView;
}

function GetJsonViewForInlineEditing(viewName, guid, pk) {
    var syncJsonView = null;
    var url = guid ? views[guid].gGetJsonViewInlineEditingUrl + viewName : '/Home/GetJsonView/' + viewName;

    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: viewName, dataAction: 'Edit', pk: pk, guid: guid },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (jsonView) {
            if (jsonView == null || jsonView == '') {
                ajaxNotSuccessMsg('');
            }
            else {
                syncJsonView = Sys.Serialization.JavaScriptSerializer.deserialize(jsonView);
            }

        }

    });

    return syncJsonView;
}

function GetJsonViewForEdit(guid) {
    return views[guid].GetJsonViewForEdit();
}

function GetJsonFilter(guid) {
    return views[guid].GetJsonFilter();
}

function FillJson(json, prefix, guid, dialog, isMultiEdit, forFilter, postfix) {
    for (var index = 0, len = json.Fields.length; index < len; ++index) {

        var field = json.Fields[index].Value;
        var fieldName = ReplaceNonAlphaNumeric(field.Name);
        var htmlField;
        if (dialog == null) {
            htmlField = $('#' + guid + prefix + fieldName);
            if (postfix)
                htmlField = $('#' + guid + prefix + fieldName + postfix);
            
        }
        else {
            htmlField = dialog.find('#' + guid + prefix + fieldName);
            if (postfix)
                htmlField = $('#' + guid + prefix + fieldName + postfix);
            
        }

        //field.Value = htmlField.val();
        //
        if (htmlField.length == 1) {

            if (isMultiEdit) {
                if (htmlField.attr('ignore') == 'no') {
                    field.Refresh = true;
                } else {
                    field.Refresh = false;
                    field.Value = '';
                    continue;
                }
            }


            if (htmlField.attr('disSource')) {
                field.Value = field.Default;
                continue;
            }

            if (htmlField[0].nodeName == "SPAN")
                field.Value = htmlField.attr('data-val');
            else if (htmlField[0].type == "checkbox")
            //field.Value = htmlField.attr('checked') == 'checked' || htmlField.attr('checked') == true;
                field.Value = Durados.CheckBox.IsChecked(htmlField);
            else if ($(htmlField[0]).attr('d_type') == 'Url')
                field.Value = $(htmlField[0]).attr('value');
            else if ($(htmlField[0]).attr('radioButtons') == 'radioButtons')
                field.Value = $('[name=' + prefix + field.Name + ']:checked').val();
            else if ($(htmlField[0]).attr('radioButtons') == 'radioButtons')
                field.Value = $('[name=' + prefix + field.Name + ']:checked').val();
            else if ($(htmlField[0]).attr('color') == '1') {
                var checkBox = htmlField.siblings('input[type=checkbox]');
                var isChecked = Durados.CheckBox.IsChecked(checkBox);
                var value = isChecked ? htmlField.spectrum("get").toRgbString() : '';

                field.Value = value;
            }
            else if (field.Type == "Autocomplete" && htmlField.attr('valueId') != undefined)
                field.Value = GetAutoCompleteValueId(htmlField);
            else if (field.Type == "TextArea" && htmlField[0].nodeName == "DIV")
                field.Value = htmlField.val();

            //            else if (htmlField.isChecklist())
            //                field.Value = getCheckListValues(htmlField);
            else {
                var obj = htmlField.val();
                if (forFilter && htmlField.hasClass('advancedFilter')) {
                    Durados.dropdowndiv.handleFilterValue(htmlField);
                    obj = htmlField.attr('d_val');
                }

                if ($.isArray(obj))
                    field.Value = obj.toString();
                else
                    field.Value = obj;
            }

            if (forFilter) { //var isFilter = htmlField.attr('filter') == 'filter';

                if (htmlField.is('select')) {
                    field.Format = getCheckListDisplayValues(htmlField);
                }
                else if (htmlField[0].nodeName == "SPAN") {
                    field.Format = htmlField.text();
                }
                else {
                    field.Format = field.Value;
                }
            } else {
                field.Format = '';
            }



        }

    }

    return json;
}


function getFriendlyFilter(guid) {

    var filter = '';

    var json = views[guid].GetJsonFilter();

    var d = $('#' + guid + 'ajaxDiv div.fixedViewPort').first();

    var isViewInDialog = d.parents("div.ui-dialog:first");

    for (var index = 0, len = json.Fields.length; index < len; ++index) {

        var field = json.Fields[index].Value;
        var htmlField;
        if (!isViewInDialog.length) {
            htmlField = $('#' + guid + 'filter_' + field.Name);
        }
        else {
            htmlField = isViewInDialog.find('#' + guid + 'filter_' + field.Name);
        }

        if (htmlField.length == 1) {

            if (htmlField.is('select')) {
                val = getCheckListDisplayValues(htmlField);
            } else {
                val = htmlField.val();
            }

            if (val) {
                if (filter) filter += " | ";
                filter += field.Name + "=" + val;
            }
        }
    }
    return filter;
}

//function MergeFields(json1, json2) {
//    var contains = false;
//    for (var index1 = 0, len1 = json1.Fields.length; index1 < len1; ++index1) {

//        var field1 = json1.Fields[index1].Value;
//        var name1 = field1.Name;
//        var val1 = field1.Value;
//        
//        var field2 = null;
//        for (var index2 = 0, len2 = json2.Fields.length; index2 < len2; ++index2) {

//            field2 = json2.Fields[index2].Value;
//            var name2 = field2.Name;
//            var val2 = field2.Value;

//            if (name1 == name2) {
//                contains = true;
//                break;
//            }

//        }

//        if (!contains) {
//            json1.Fields.push(field2);
//        }
//    }
//}

function FillJsonForSearch(json, prefix, guid, search) {
    for (var index = 0, len = json.Fields.length; index < len; ++index) {
        var field = json.Fields[index].Value, val;
        if (field.Searchable) {
            val = getFilterValueForSearch($('#' + guid + prefix + ReplaceNonAlphaNumeric(field.Name)))
            if (!val) field.Value = search;
        }
    }
    return json;
}

function getFilterValueForSearch($field) {
    if ($field.length && ($field[0].type == 'text' || $field[0].nodeName == 'SELECT')) {
        return $field.val()
    }

}

function getCheckListValues(checkList) {
    var val = '';
    checkList.find(':checked').each(function () {
        val = val + $(this).val() + ',';
    });

    if (val != '')
        val = val.slice(0, -1)

    return val;
}


function getCheckListDisplayValues(checkList) {
    var val = '';
    checkList.find('option:selected').each(function () {
        if (val != '') val += ',';
        val = val + $(this).text();
    });

    return val;
}

function ClearJson(json, viewName) {
    for (var index = 0, len = json.Fields.length; index < len; ++index) {
        var k = json.Fields[index].Key;
        if (viewName == "durados_v_ChangeHistory" && (k == 'PK' || k == 'ViewName')) continue;

        var field = json.Fields[index].Value;
        if (field.Permanent == false) field.Value = "";
    }

    return json;
}


Durados.round = function (number, digits) {
    var pow = Math.pow(10, digits);
    return Math.round(number * pow) / pow;
}

//call filter if press enter
function handleEnterFilter(inField, e, guid, control) {

    if (LoadingApplicationData) return;

    var charCode;

    if (e && e.which) {
        charCode = e.which;
    } else if (window.event) {
        e = window.event;
        charCode = e.keyCode;
    }

    if (charCode == 13) {
        if (!(e.srcElement.className.indexOf("search_text") !== -1)) {
            //            if ($('.advancedFilterDialog.ui-widget:visible').length) {
            //                Durados.dropdowndiv.hide();
            //            }
            //            else {
            Durados.dropdowndiv.hide();
            FilterForm.Apply(false, guid, control);
            //            }
        }
        else {
            var searchBtn = $(e.srcElement).siblings().filter(":first");
            FilterForm.ApplySearch(true, guid, searchBtn, "Search on text...")
        }
    }
    else if (charCode == 27) {
        Durados.dropdowndiv.hide();
    }
}


var Autocomplete; if (!Autocomplete) Autocomplete = {};

Autocomplete.Init = function (guid) {
    $('input.Autocomplete').each(function () {
        Autocomplete.InitElement($(this), guid);
        //        var ac = $(this).autocomplete(views[guid].gAutoCompleteUrl + $(this).attr('viewName') + '?field=' + $(this).attr('name'),
        //        {
        //            dataType: 'json',
        //            parse: AutoCompleteParse,
        //            mustMatch: false,
        //            max: 20,
        //            formatItem: function(row) {
        //                return row.Name;
        //            },
        //            formatResult: function(data, position, total) {
        //                alert(data);
        //                return data;
        //            },
        //            width: 0,
        //            highlight: false,
        //            multiple: false,
        //            multipleSeparator: ";"
        //        }).result(AutoCompleteResult);
        //        $(this).bind('keypress', AutoCompleteSearch);
    });

    //    var graphicProperties = $('input[name="GraphicProperties"]');
    //    if (graphicProperties.length == 1) {
    //        alert("graphicProperties");
    //        graphicProperties.autocomplete({
    //            source: ["c++", "java", "php", "coldfusion", "javascript", "asp", "ruby"]
    //        });
    //    }
}

Autocomplete.InitElement = function (element, guid) {
    var ac = element.autocomplete(views[guid].gAutoCompleteUrl + element.attr('viewName') + '?field=' + element.attr('name'),
    {
        dataType: 'json',
        parse: AutoCompleteParse,
        mustMatch: false,
        max: 20,
        formatItem: function (row) {
            return row.Name;
        },
        formatResult: function (data, position, total) {
            ajaxNotSuccessMsg(data);
            return data;
        },
        width: 0,
        highlight: false,
        multiple: false,
        multipleSeparator: ";"
    }).result(AutoCompleteResult);
    element.bind('keypress', AutoCompleteSearch);
}

//Autocomplete.InitCreateFields = function(guid) {
//    var json = views[guid].jsonViewForCreate;
//    Autocomplete.InitFields(json, createPrefix, guid);
//}

//Autocomplete.InitEditFields = function(guid) {
//    var json = GetJsonViewForEdit(guid);
//    Autocomplete.InitFields(json, editPrefix, guid);
//}

//Autocomplete.InitFilterFields = function(guid) {
//    var json = GetJsonFilter(guid);
//    Autocomplete.InitFields(json, filterPrefix, guid);
//}

//Autocomplete.InitFields = function(json, prefix, guid) {
//    for (var index = 0, len = json.Fields.length; index < len; ++index) {
//        var field = json.Fields[index].Value;
//        if (field.Type == "Autocomplete") {
//            Autocomplete.InitField(field.Name, prefix, guid);
//        }
//    }
//}

//Autocomplete.InitField = function(fieldName, prefix, guid) {

//    $("#" + guid + prefix + fieldName).autocomplete(views[guid].gAutoCompleteUrl + '?field=' + fieldName,
//        {
//            dataType: 'json',
//            parse: AutoCompleteParse,
//            mustMatch: true,
//            max: 20,
//            formatItem: function(row) {
//                return row.Name;
//            },
//            width: 160,
//            highlight: false,
//            multiple: false,
//            multipleSeparator: ";"
//        }).result(AutoCompleteResult);
//}

function getDialogWidth(colCount, rowWidth) {
    return colCount * rowWidth * 2;
}

function getDialogHeight(colCount, rowCount, rowWidth) {
    (rowCount / colCount + 1) * rowWidth + 120;
}

// Rectangle
var Rectangle; if (!Rectangle) Rectangle = {};

Rectangle.New = function (top, left, width, height) {
    var rect = new Object();

    rect.top = top;
    rect.left = left;
    rect.width = width;
    rect.height = height;

    return rect;
}

Rectangle.Save = function (rect, name) {
    if (rect != null) {
        var src = Sys.Serialization.JavaScriptSerializer.serialize(rect);
        $.cookie(name, src, { expires: 1000 });
    }
}

Rectangle.Load = function (name) {
    var src = $.cookie(name);

    if (src != null) {
        var rect = Sys.Serialization.JavaScriptSerializer.deserialize(src);
        return rect;
    }
    else return null;
}

//menu
$(document).ready(function () 
{
    InitTopMenu();
    $.ajaxSetup({
        error: function (x, e) {
            ajaxErrorsHandler(x, null, e);
        }
    });

    if($.browser.mozilla) 
    {
        $('.imgLoadSelector').each(function () 
        {   /**Goes over images tags and set image to display*/
            var sSrc = $(this).attr('src');
            if (sSrc) 
            {
                /**imgLoadSelector (jQuery+css onload+display image selector solution) solution for image onload, onload is used here for displaying image when finishing loading, it doesn't work for Firefox*/
                Durados.Image.SetSize(this);
            }
        });
    }
        //////////////////
});

function InitTopMenu() {

    
    if ($("#smoothmenu").length == 0)
        return;

    ddsmoothmenu.init({
        mainmenuid: "smoothmenu", //menu DIV id
        orientation: 'h', //Horizontal or vertical menu: Set to "h" or "v"
        classname: 'ddsmoothmenu', //class added to menu's outer DIV
        contentsource: "markup", //"markup" or ["container_id", "path_to_menu_file"]
        classdownarrow: 'downarrowclass',
        classrightarrow: 'rightarrowclass',
        shadow: true
    })

}

function InitCommandMenu(guid) {

    //    if ($("#" + guid + "smoothcommand").length == 0)
    //        return;

    //    ddsmoothmenu.init({
    //        mainmenuid: guid + "smoothcommand", //menu DIV id
    //        orientation: 'h', //Horizontal or vertical menu: Set to "h" or "v"
    //        classname: 'ddsmoothcommand', //class added to menu's outer DIV
    //        contentsource: "markup", //"markup" or ["container_id", "path_to_menu_file"]
    //        classdownarrow: 'downarrowcommandclass',
    //        classrightarrow: 'rightarrowclass',
    //        shadow: false
    //    })
    //    $('.contextmenudownarrow').hover(function() {
    //        $(this).
    //    });
}

//Dialog
function SaveDialogOnResize(event, ui) {
    SaveDialogSizeAndPosition(event, ui);
}

function SaveDialogOnDrag(event, ui) {
    SaveDialogSizeAndPosition(event, ui);
}

function SaveDialogSizeAndPosition(event, ui) {
    //Changed by br- Fix bug in save view settings dialog position
    //    var guid = $(event.srcElement).attr('guid');
    //    if (guid == null)
    //        guid = $(event.srcElement).siblings().closest('div [hasguid="hasguid"]').attr('guid');

    //    if (guid == null)
    //        return;

    try {
        var dialog = $(event.target).parent();
        if (dialogExt.getState(dialog) == "max")
            return;
    }
    catch (eee) {
        return;
    }

    try {
        var top = dialog.position().top;
        var left = dialog.position().left;

        var width = dialog.width();
        var height = dialog.height();

        var rect = Rectangle.New(top, left, width, height);
        Rectangle.Save(rect, currentViewName);

        //        if (guid != null) {
        //$('#' + guid + 'CreateTabs').css("width", (width - 55) + 'px');
        //$('#' + guid + 'EditTabs').css("width", (width - 55) + 'px');
        //        }

    }
    catch (eee) {
        return;
    }

}

function SaveFloatDialogOnResize(event, ui) {
    var dialog = $(event.target).parent();
    SaveFloatDialogSizeAndPosition(dialog);
}

function SaveFloatDialogOnDrag(event, ui) {
    SaveFloatDialogSizeAndPosition(event, ui);
}

function SaveFloatDialogSizeAndPosition(dialog) {

    try {
        var guid = dialog.find('div[ajaxdiv]').first().attr('guid');
        if (guid) {
            var displayType = views[guid].DataDisplayType;
            if (displayType == "Preview") {
                return;
            }

            Durados.GridHandler.adjustDataTableHeight(guid);
        }
    } catch (ex) { }


    try {
        var top = dialog.position().top;
        var left = dialog.position().left;

        var width = dialog.width();
        var height = dialog.height();

        var rect = Rectangle.New(top, left, width, height);
        Rectangle.Save(rect, currentViewName);

    }
    catch (eee) {
        return;
    }

}

function getErrorsList(dialog) {
    var errorList = '<p>Please correct the validation warnings:</p><ul>';

    try {
        dialog.find(".textfieldRequiredMsg, .selectRequiredMsg, .textareaRequiredMsg").each(function () {
            if ($(this).css('display') != 'none')
                errorList += '<li>' + $(this).attr('d_label') + " is required.</li>";
        });

        dialog.find(".textfieldInvalidFormatMsg, .textfieldMinValueMsg, .textfieldMaxValueMsg, .textfieldMinCharsMsg, .textfieldMaxCharsMsg").each(function () {
            if ($(this).css('display') != 'none')
                errorList += '<li>' + $(this).attr('d_label') + " is invalid.</li>";
        });
    }
    catch (err) { }
    errorList += '</ul>';

    var data = { dialog: dialog, message: errorList, cancel: false }
    $(Durados.View).trigger('getErrorsList', data);

    if (data.cancel)
        errorList = data.message;

    return errorList;
}

function refreshView(guid, qs, data) {

    if (LoadingApplicationData) return;

    saveElementScrollsPosition(guid);

    showProgress();

    if (!data) data = {}

    var url = views[guid].gIndexUrl;

    //url += '?guid='+guid; //__FK_TREE_METERS_Children__  

    if (qs) url += '?' + qs + '&guid=' + guid;
    else url += '?guid=' + guid;

    var pks = Multi.GetSelection(guid);

    $.post(url, data,
        function (html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                FilterForm.HandleSuccess(html, false, guid, pks);
                resetElementScrollsPosition(guid);
                if (pks != null) {
                    Multi.SelectByPKs(guid, pks);
                }
            }
            else {
                //FilterForm.ShowFailureMessage(html.replace("$$error$$", ""));
                ajaxNotSuccessMsg(html);
            }
        });
}

function shortcutKey(element, f, key, ctrl) {
    element.keydown(function (event) {
        if (event.keyCode == key && (!ctrl || (ctrl && event.ctrlKey))) {
            event.preventDefault();
            f();
        }
    });
}

function isAdmin(guid) {
    return views[guid].Controller == "Admin";
}

// add
var AddDialog; if (!AddDialog) AddDialog = {};

AddDialog.GetButtons = function (pk, guid, hideClose, allowEdit, insertAbovePK, duplicateRecursive) {
    if (allowEdit == null)
        allowEdit = true;

    var buttons = {};  //initialize the object to hold my buttons
    var newButtonEventArgs = { pk: pk, guid: guid, hide: false };

    if (!hideClose) {
        buttons[translator.saveAndClose] = function () { AddDialog.Add(false, guid, true, allowEdit, insertAbovePK, duplicateRecursive, pk, false); }  //the function that does the save
    }
    buttons[translator.save] = function () { AddDialog.Add(false, guid, false, allowEdit, insertAbovePK, duplicateRecursive, pk, false); }  //the function that does the save

    $(AddDialog).trigger('newButton', newButtonEventArgs);
    if (!newButtonEventArgs.hide) {
        buttons[translator.New] = function () { AddDialog.Reset(guid, false); }  //the function that does the save
    }

    if (views[guid].WorkFlowStepsFieldName != '') {
        var promoteButtonEventArgs = { pk: pk, guid: guid, hide: false };
        $(AddDialog).trigger('promoteButton', promoteButtonEventArgs);
        if (!promoteButtonEventArgs.hide) {
            buttons[translator.saveAnd + views[guid].PromoteButton] = function () { AddDialog.Add(false, guid, true, allowEdit, insertAbovePK, duplicateRecursive, pk, true); }  //the function that does the save & complete
        }
    }

    if (isAdmin(guid) && !d_autoCommit) {
        buttons[translator.saveAndCommit] = function () {
            AddDialog.Add(false, guid, true, allowEdit, insertAbovePK, duplicateRecursive, pk, false);
            var url = rootPath + 'Admin/RefreshConfig/';
            refreshConfig(url, guid);
        }  //the function that does the save
    }
    if (!hideClose) {
        buttons[translator.close] = function () { AddDialog.Close(guid); }  //the function that does the save
    }


    return buttons;
}

AddDialog.HandleDockField = function (guid) {
    var jsonFilter = FilterForm.JsonFilter;
    if (jsonFilter == null) {
        jsonFilter = GetJsonFilter(guid);
    }
    AddDialog.SetDefaultsFromFilter(jsonFilter, createPrefix, guid);

    AddDialog.SetDefaults(views[guid].jsonViewForCreate, createPrefix, guid);

    $("#" + guid + "DataRowCreate").find('[name="DisplayName"]').val('NewField$$');
    $("#" + guid + "DataRowCreate").attr('firstTime', 'yes');
    AddDialog.Add(false, guid, true);

}


AddDialog.Open = function (pk, guid, hideClose, allowEdit, insertAbovePK, duplicateRecursive) {
    if (isDockFields(guid)) {
        //        $("#overlay", window.parent.document).show();
        var isViewOwner = views[guid] == null ? false : views[guid].Role == "View Owner";

        if (IsFieldsSettings() && isViewOwner) {
            AddDialog.HandleDockField(guid);
            return;
        }
        else {
            parent.parent.AddColumn(function (newColumnName) {
                AddDialog.Refresh(guid, null, newColumnName);
                parent.parent.Refresh(parent.parent.getMainPageGuid());
            });
            return;
        }
    }
    try {
        showProgress();
    }
    catch (err) {
    }
    var dialog = $('#' + guid + 'DataRowCreate');

    var dup = pk != null;
    var title;
    try {
        if (dup) {
            title = views[guid].dupTitle == null || views[guid].dupTitle == '' || views[guid].dupTitle == translator.addRowTo || views[guid].dupTitle.length == 3 ? translator.addRowTo + ' ' + views[guid].gViewDisplayName : views[guid].dupTitle;
        }
        else {
            title = views[guid].addTitle == null || views[guid].addTitle == '' || views[guid].addTitle == translator.addRowTo || views[guid].addTitle.length == 3 ? translator.addRowTo + ' ' + views[guid].gViewDisplayName : views[guid].addTitle;
        }
    }
    catch (err) {
    }

    dialog.dialog("option", "title", title);
    var buttons = AddDialog.GetButtons(pk, guid, hideClose, allowEdit, insertAbovePK, duplicateRecursive);
    dialog.dialog("option", "buttons", buttons);
    
    var s = 83; // key code
    shortcutKey(dialog, buttons[translator.save], s, true);
    var e = 69; // key code
    shortcutKey(dialog, buttons[translator.saveAndClose], e, true);
    var n = 78; // key code
    shortcutKey(dialog, buttons[translator.New], n, true);

    //  sandisk alegro
    //    dialog.bind("dialogopen", function(event, ui) {
    //        dialog.find('.inlineAddingImg').each(function() {
    //            expand(this, true);
    //        });
    //    });


    AddDialog.Reset(guid, dup);
    // var qMax = queryString('max');
    var isMaximize = isMaximizeDialog(guid);

    var state = isMaximize == true ? 'max' : $.cookie("state_" + views[guid].gViewName);

    if (itemCreateDialog == null || itemCreateDialog.attr('id') != dialog.attr('id')) {
        currentViewName = views[guid].gViewName;
        //jquery doesn't remember the position just the size
        if (state != "max") {
            var rec = Rectangle.Load(currentViewName);
            if (rec != null) {
                dialog.dialog("option", "position", [rec.left, rec.top]);
            }
        }
        else {
            dialogExt.max(dialog, null, guid);
        }
    }

    if (state != "max") {
        dialog.dialog("option", "height", 'auto');
    }
    else {
        dialogExt.max(dialog, null, guid);
    }

    //    dialog.dialog("option", "open", function(event, ui) {
    //        window.setTimeout(function() {
    //            jQuery(document).unbind('mousedown.dialog-overlay').unbind('mouseup.dialog-overlay');
    //        }, 1);
    //    });

    dialog.dialog({
        show: { effect: 'fold', duration: 500 },
        hide: { effect: 'fold', duration: 500 }
    });

    dialog.dialog('open');
    //Create wysiwyg for all textareas
    AddDialog.CreateWysiwyg(guid);

    Durados.Tabs.select($('#' + guid + 'CreateTabs'), 0);
    //    $('#' + guid + 'CreateTabs').tabs("select", 0)

    var jsonFilter = FilterForm.JsonFilter;
    if (jsonFilter == null) {
        jsonFilter = GetJsonFilter(guid);
    }
    if (pk == null) {
        Durados.Dependencies.cleanUp();
        for (var i = 0; i < views[guid].jsonViewForCreate.Fields.length; i++) {
            if (views[guid].jsonViewForCreate.Fields[i].Value.DependencyData)
                Durados.Dependencies.createHandlers($('#' + guid + createPrefix + ReplaceNonAlphaNumeric(views[guid].jsonViewForCreate.Fields[i].Key)), guid, views[guid].jsonViewForCreate.Fields, views[guid].jsonViewForCreate.ViewName, createPrefix);
        }

    }
    else {
        var jsonView = EditDialog.GetJsonViewValue(pk, guid, 'Create');
        EditDialog.Load(pk, createPrefix, guid, jsonView, 'DataRowEdit');
    }
    subgridsHashes[guid] = new Array();
    $(Durados.View).trigger('addBeforeDerivation', { pk: pk, guid: guid, dialog: dialog, viewName: views[guid].ViewName });
    initDerivationOnShow(views[guid].jsonViewForCreate.Derivation, dialog, createPrefix, guid, views[guid].jsonViewForCreate.ViewName);

    dialog.attr('firstTime', 'yes');

    initDropdownChecklistsCreate(guid);

    dialog.attr('changed', 'no');

    //Bind dialog inputs to dialog change event
    Durados.Dialogs.BindToChangeEvent('input', dialog);
    Durados.Dialogs.BindToChangeEvent('select', dialog);
    Durados.Dialogs.BindToChangeEvent('textarea', dialog);

    if (views[guid].jsonViewForCreate.ViewName == "Field") {
        var dataType = dialog.find('[name="DataType"]');
        dialog.find('[name=RelatedViewName]').parents('tr:first').parents('tr:first').hide();
        if (dataType.length == 1) {
            dataType.change(function () {
                setDataType(dialog, dataType)

            });
        }
    }

    $(Durados.View).trigger('add', { pk: pk, guid: guid, dialog: dialog, viewName: views[guid].jsonViewForCreate.ViewName, prefix: createPrefix });

    setFocusInDialog(dialog);

    try {
        hideProgress();
    }
    catch (err) {
    }


}

function initDerivation(derivation, dialog, prefix, guid, viewName) {
    if (derivation == null)
        return;

    var disabledFields = dialog.find('[disabled]');

    derivation.DisabledFields[prefix] = disabledFields;

    var derivationField = $('#' + guid + prefix + ReplaceNonAlphaNumeric(derivation.DerivationField));

    derivationField.change(function () {
        showProgress();

        setTimeout(function () {
            var value;
            if (derivationField.attr('type') == 'checkbox')
            //value = derivationField.attr('checked') || derivationField.attr('checked') == 'checked';
                value = Durados.CheckBox.IsChecked(derivationField);
            else
                value = derivationField.val();
            var eventArgs = { derivationField: derivationField, value: value, guid: guid, viewName: viewName, prefix: prefix, cancel: false };
            var cancel = $(Durados.View).trigger('derivationFieldChanged', eventArgs);
            if (!eventArgs.cancel) {
                initDerivation2(derivation, value, guid, prefix, dialog, viewName)
            }

            hideProgress();

        }, 1);
    });

}

function enableField(f, guid, prefix, viewName) {
    f.removeAttr('disabled');
    var td = $('#the' + guid + prefix + viewName + '_' + f.attr('name'));
    td.find('img').show();
    var hide = getHideInDerivation(f);
    if (hide) {
        showInDerivation(f);
    }
    $(Durados.View).trigger('fieldEnabledInDerivation', { field: f, guid: guid, prefix: prefix, viewName: viewName, type: 'field' });
}

function enableChildren(li, guid, prefix, viewName) {
    var url = li.attr('newUrl');
    //    if (url.indexOf("&disabled=") != -1) {
    ////        url = url + "&disabled=False";
    ////        li.attr('url', url);
    //    }
    //    else {
    if (li.attr('childrenDisabled') == 'childrenDisabled') {
        url = url + "&disabled=False";
        var index = parseInt(li.attr('index'));

        li.parents("div:first").tabs("url", index, url);

        li.find("a").attr('nocache', 'nocache');
        li.removeAttr('childrenDisabled')
    }
    else {
        li.find("a").removeAttr('nocache');
    }
    $(Durados.View).trigger('fieldEnabledInDerivation', { field: li, guid: guid, prefix: prefix, viewName: viewName, type: 'tabgrid' });

    //    }
}

function disableField(f, guid, prefix, viewName) {
    f.attr('disabled', 'disabled');
    var td = $('#the' + guid + prefix + viewName + '_' + f.attr('name'));
    td.find('img:not(.jquery-safari-checkbox img, .plan)').hide();
    var hide = getHideInDerivation(f);
    if (hide) {
        hideInDerivation(f);
    }
    $(Durados.View).trigger('fieldDisabledInDerivation', { field: f, guid: guid, prefix: prefix, viewName: viewName, type: 'field' });
}

function disableChildren(li, guid, prefix, viewName) {
    var url = li.attr('newUrl');
    if (li.attr('childrenDisabled') == 'childrenDisabled') {
        //        url = url + "&disabled=True";
        //        li.attr('url', url);
        //        var index = parseInt(li.attr('index'));
        //        li.parents("div:first").tabs("url", index, url);

        li.find("a").attr('nocache', 'nocache');
    }
    else {
        url = url + "&disabled=True";
        var index = parseInt(li.attr('index'));

        li.parents("div:first").tabs("url", index, url);

        li.find("a").attr('nocache', 'nocache');
        li.attr('childrenDisabled', 'childrenDisabled')

    }

    $(Durados.View).trigger('fieldDisabledInDerivation', { field: li, guid: guid, prefix: prefix, viewName: viewName, type: 'tabgrid' });

}

function getSubGridHash(dialog) {
    var grids = dialog.find('span.inlineAddingImg');

    var hash = new Array();

    grids.each(function () {
        hash[$(this).attr('id')] = { toDisable: false, disabled: false };

    });

    return hash;
}

var subgridsHashes = new Array();

function initDerivation2(derivation, value, guid, prefix, dialog, viewName) {
    //dialog.find('[disabled]').removeAttr('disabled');
    dialog.find('[disabled]').each(function () {
        //$(this).removeAttr('disabled');
        enableField($(this), guid, prefix, viewName);
    });
    var subgridsHash = null;
    if (subgridsHashes[guid][dialog] == null) {
        subgridsHashes[guid][dialog] = getSubGridHash(dialog);
    }
    else {
        var grids = dialog.find('span.inlineAddingImg');
        var hash = subgridsHashes[guid][dialog];

        grids.each(function () {
            hash[$(this).attr('id')].toDisable = false;

        });
    }

    subgridsHash = subgridsHashes[guid][dialog];

    dialog.find("li[childrenFieldName]").each(function () {
        enableChildren($(this), guid, prefix, viewName);
    });

    dialog.find('.dropdownchecklist').each(function () {
        initDropdownchecklist($(this), guid);
    });

    //derivation.DisabledFields[prefix].attr('disabled', 'disabled');
    derivation.DisabledFields[prefix].each(function () {
        //$(this).attr('disabled', 'disabled');
        disableField($(this), guid, prefix, viewName);
    });
    InitRequiredFieds(derivation, guid, prefix, viewName);

    for (var i = 0, lenI = derivation.Deriveds.length; i < lenI; ++i) {
        var derived = derivation.Deriveds[i];
        var derivedValue = derived.Value;
        if (derivedValue == value) {
            for (var j = 0, lenJ = derived.Fields.length; j < lenJ; ++j) {
                var name = derived.Fields[j].Key;
                var disabledField = $('#' + guid + prefix + ReplaceNonAlphaNumeric(name));
                if (disabledField.length > 0) {
                    if (disabledField.hasClass('inlineAddingImg')) { // sub grid
                        subgridsHash[disabledField.attr('id')].toDisable = true;
                    }
                    else {
                        disableField(disabledField, guid, prefix, viewName);
                        if (disabledField.attr('class') == "dropdownchecklist") {
                            initDropdownchecklist(disabledField, guid);
                        }
                        try {
                            var v = views[guid].validations[prefix + "," + viewName + "," + name];
                            if (v != null) {
                                v.isRequired = false;
                                v.Required = false;
                                v.reset();
                            }
                        }
                        catch (err) { }
                    }
                }
                else {
                    var li = dialog.find("li[childrenFieldName='" + name + "']");
                    if (li.length == 1) { // tab grid
                        disableChildren(li, guid, prefix, viewName);
                    }

                }
            }

            var grids = dialog.find('span.inlineAddingImg');

            grids.each(function () {
                var disabledField = $(this);
                showInDerivation(disabledField);
                var id = disabledField.attr('id');
                if (subgridsHash[id].disabled == false && subgridsHash[id].toDisable == true) {
                    var hide = getHideInDerivation(disabledField);
                    if (!hide) {
                        expand(disabledField, true, null, true, guid);
                        subgridsHash[id].disabled = true;
                        $(Durados.View).trigger('fieldDisabledInDerivation', { field: disabledField, guid: guid, prefix: prefix, viewName: viewName, type: 'subgrid' });
                    }
                    else {
                        hideInDerivation(disabledField);
                    }
                }
                else if (subgridsHash[id].disabled == true && subgridsHash[id].toDisable == false) {
                    showInDerivation(disabledField);
                    expand(disabledField, true, null, false, guid);
                    subgridsHash[id].disabled = false;
                    $(Durados.View).trigger('fieldEnabledInDerivation', { field: disabledField, guid: guid, prefix: prefix, viewName: viewName, type: 'subgrid' });
                }

            });
            return;
        }
    }

    //$(Durados.View).trigger('afterDerivationChanged', { derivation: derivation, value: value, guid: guid, prefix: prefix, dialog: dialog, viewName: viewName });

}

function getHideInDerivation(field) {
    return field.parents('td:first[hideInDerivation="hideInDerivation"]').length > 0;
}

function derivationVisibility(field, hide) {
    var visibility = hide ? 'hidden' : 'visible';
    var td = field.parents('td:first[hideInDerivation="hideInDerivation"]');
    td.css('visibility', visibility);
     hide ?td.hide() : td.show();// tr.hide() : tr.show();
    var prevTd = td.prev();
    if (prevTd.attr('hideInDerivation') == 'hideInDerivation') {
        prevTd.css('visibility', visibility);
        hide ? prevTd.hide() : prevTd.show();
    }
    var tr = td.closest('tr');

    for (var i =  tr.children('td').length-1 ; i >=0 ; i--) {
        if ($(tr.children('td')[i]).css('visibility') == "visible") break;
    }
    (i == -1) ? tr.hide() : tr.show();
//    var lableTr = $(tr.prev());
//    if (i == -1) {
//        tr.hide();
//        if (lableTr.find('span.fieldLabel').length > 1) lableTr.hide();
//    }
//    else {
//        tr.show();
//        if (lableTr.find('span.fieldLabel').length > 1) lableTr.show();
//    }
}

function hideInDerivation(field) {
    derivationVisibility(field, true)

}

function showInDerivation(field) {
    derivationVisibility(field, false)
}


function InitRequiredFieds(derivation, guid, prefix, viewName) {
    for (var i = 0, lenI = derivation.Deriveds.length; i < lenI; ++i) {
        var derived = derivation.Deriveds[i];
        for (var j = 0, lenJ = derived.Fields.length; j < lenJ; ++j) {
            var required = derived.Fields[j].Value;
            if (required) {
                var name = derived.Fields[j].Key;
                var v = views[guid].validations[prefix + "," + viewName + "," + name];
                if (v != null) {
                    v.isRequired = true;
                    v.Required = true;
                }
            }
        }
    }
}

function initDerivationOnShow(derivation, dialog, prefix, guid, viewName) {
    if (derivation == null)
        return;

    var disabledFields = derivation.DisabledFields[prefix];

    var derivationField = $('#' + guid + prefix + ReplaceNonAlphaNumeric(derivation.DerivationField));

    var value;
    if (derivationField.attr('type') == 'checkbox')
    //value = derivationField.attr('checked') || derivationField.attr('checked') == 'checked';
        value = Durados.CheckBox.IsChecked(derivationField);
    else
        value = derivationField.val();

    initDerivation2(derivation, value, guid, prefix, dialog, viewName)

}

AddDialog.onafterSetDefaultsFromFilter = function () { }

//var isUploaded = false;

AddDialog.SetDefaultsFromFilter = function (json, prefix, guid) {
    if (json != null) {
        for (var index = 0, len = json.Fields.length; index < len; ++index) {
            var field = json.Fields[index].Value;
            var fieldName = ReplaceNonAlphaNumeric(field.Name);
            var htmlField = $('#' + guid + prefix + fieldName);

            if (htmlField.length > 0 && field.Value != null && field.Value != '' || $(htmlField[0]).attr('upload') == 'upload') {

                if ($(htmlField[0]).attr('upload') == 'upload') {
//                    if (!isUploaded) {
                        UploadFile(fieldName, prefix, guid);
                        showUpload(fieldName, prefix, field.Value, $('#' + guid + prefix + 'upload_img_' + fieldName).attr('UploadPath') + field.Value, guid);
//                        isUploaded = true;
//                    }
                }
                else if ($(htmlField[0]).attr('radioButtons') == 'radioButtons') {
                    //$('[name=' + guid + prefix + fieldName + '][value=' + field.Value + '' + ']').attr('checked', 'checked');
                    Durados.CheckBox.SetChecked($('[name=' + guid + prefix + fieldName + '][value=' + field.Value + '' + ']'), 'checked');
                }
                else if (htmlField[0] != undefined && htmlField[0].type == "textarea") {
                    htmlField.text(field.Value);
                }
                else if (htmlField[0] != undefined && htmlField[0].type == "checkbox")
                // htmlField.attr('checked', field.Value);
                    Durados.CheckBox.SetChecked(htmlField, field.Value);
                else if (field.Type == 'Autocomplete') {
                    htmlField.attr('valueId', field.Value);
                    htmlField.val(field.Default);
                }
                else {
                    if (htmlField[0].tagName == "SELECT") {
                        if (htmlField.find('option[value="' + field.Value + '"]').length == 0) {
                            var filter = $('#' + guid + filterPrefix + fieldName);
                            var text = '';
                            if (filter.length > 0) {
                                text = filter.find('option[value="' + field.Value + '"]').text();
                            }
                            else {
                                text = field.Default;
                            }
                            if (text && field.Value) {
                                htmlField.append($("<option value='" + field.Value + "'>" + text + "</option>"));
                            }
                        }
                    }
                    if (field.Value.indexOf(TOKEN) == -1) // check is not any token in value
                    {
                        htmlField.val(field.Value);
                    }

                    Dependency.Init(htmlField, prefix, guid);
                }
            }
        }

        $(AddDialog).trigger('onafterSetDefaultsFromFilter', { guid: guid, json: json });
    }
}


//AddDialog.Show = function(guid) {
//    var dialog = $('#' + guid + 'DataRowCreate');
//    dialog.dialog("option", "title", translator.addRowTo + views[guid].gViewDisplayName);
//    dialog.dialog("option", "buttons", AddDialog.GetButtons(guid));

//    //jquery doesn't remember the position just the size
//    if (itemCreateDialog.attr('id') != dialog.attr('id')) {
//        currentViewName = views[guid].gViewName;
//        var rec = Rectangle.Load(currentViewName);
//        if (rec != null) {
//            dialog.dialog("option", "position", [rec.left, rec.top]);

//        }
//    }
//    dialog.dialog('open');

//    //Create wysiwyg for all textareas
//    AddDialog.CreateWysiwyg(guid);

//    $('#' + guid + 'CreateTabs').tabs("select", 0)

//    complete(guid);


//}

AddDialog.Reset = function (guid, dup) {
    var dialog = $('#' + guid + 'DataRowCreate');
    var changed = dialog.attr('changed') == 'yes';
    if (changed) {
        if (!confirm("Discard changes?")) {
            return;
        }
        else {
        }
    }
    else {

    }

    dialog.parent().find('.ui-dialog-buttonpane button').each(function () {
        if ($(this).text() == translator.saveAndClose || $(this).text() == translator.save) {
            $(this).removeAttr('disabled');
            $(this).removeClass('ui-state-disabled');
        }
    });

    var form = $('#' + guid + 'Create' + ReplaceNonAlphaNumeric(views[guid].jsonViewForCreate.ViewName) + 'DataRowForm');
    if (form.length) form[0].reset();

    dialog.find('select').each(function () {
        if (this.multiple) {
            var options = $(this).find('option');
            options.each(function () {
                $(this).removeAttr('selected');
            });
            $(this).dropdownchecklist("refresh");
        }
    });

    AddDialog.ResetDependencies(form, createPrefix, guid);
    dialog.attr('firstTime', 'yes');

    resetUpload();
    AddDialog.DisableTab('CreateTabs', guid);

    AddDialog.SetDisDup(views[guid].jsonViewForCreate, createPrefix, guid, dup);

    var jsonFilter = FilterForm.JsonFilter;
    if (jsonFilter == null) {
        jsonFilter = GetJsonFilter(guid);
    }
    AddDialog.SetDefaultsFromFilter(jsonFilter, createPrefix, guid);

    AddDialog.SetDefaults(views[guid].jsonViewForCreate, createPrefix, guid);


    // sandisk alegro
    dialog.find('.childrenViewer').each(function () {
        $(this).html('');
    });


    //Clear wysiwyg for all textareas
    clearWysiwyg(dialog);
}

function clearWysiwyg(dialog) {
    try {
        dialog.find('textarea[rich = "true"]').each(function () {
            $(this).parent().find('iframe').contents().find("body").text('');
        });

        if (!isIE()) {
            dialog.find('textarea[rich = "true"]').each(function () {
                $(this).text('');
            });
        }
    }
    catch (err) { }

}

AddDialog.ResetDependencies = function (form, prefix, guid) {
    form.find("select[insideDependency='insideDependency']").each(function () {
        var triggerFieldName = $(this).attr('name');
        var dependentFieldNames = $(this).attr('dependentFieldNames');
        var dependentFieldViewName = $(this).attr('dependentFieldViewName');
        var arr = dependentFieldNames.split(';');
        $(arr).each(function () {
            var dependentFieldName = this;
            try {
                var select = $('#' + guid + prefix + ReplaceNonAlphaNumeric(dependentFieldName));
            }
            catch (err) {
                alert("Could not find the dependent field " + dependentFieldName + " of the field " + triggerFieldName, guid);
                return;
            }
            if (select.length != 1) {
                alert("Could not find the dependent field " + dependentFieldName + " of the field " + triggerFieldName, guid);
                return;
            }
            Dependency.ClearAll(select, guid);
        });

    });
}

AddDialog.SetDefaults = function (json, prefix, guid) {
    if (json != null) {
        for (var index = 0, len = json.Fields.length; index < len; ++index) {
            var field = json.Fields[index].Value;
            var fieldName = ReplaceNonAlphaNumeric(field.Name);

            var htmlField = $('#' + guid + prefix + fieldName);
            if ((field.Default != null && field.Default != '') || $(htmlField[0]).attr('upload') == 'upload' || htmlField.attr('hasInsideDefault') == 'hasInsideDefault') {

                if ($(htmlField[0]).attr('upload') == 'upload') {
//                    if (!isUploaded) {
                        UploadFile(field.Name, prefix, guid);
                        showUpload(field.Name, prefix, field.Default, $('#' + guid + prefix + 'upload_img_' + fieldName).attr('UploadPath') + field.Default, guid);
//                        isUploaded = true;
//                    }
                }
                else if ($(htmlField[0]).attr('radioButtons') == 'radioButtons') {
                    //$('[name=' + guid + prefix + field.Name + '][value=' + field.Value + '' + ']').attr('checked', 'checked');
                    Durados.CheckBox.SetChecked($('[name=' + guid + prefix + field.Name + '][value=' + field.Value + '' + ']'), 'checked');
                }
                else if (htmlField[0].type == "textarea") {
                    htmlField.text(field.Default);
                    $('#' + guid + prefix + fieldName + ' [rich="true"]').htmlarea(); //create the wysiwyg
                }
                else if (htmlField[0].type == "checkbox") {
                    if (field.Default != 'No') {
                        // htmlField.attr('checked', field.Default);
                        Durados.CheckBox.SetChecked(htmlField, field.Default);
                    }
                }
                else if (field.Type == 'Autocomplete') {
                    htmlField.attr('valueId', field.Default);
                    htmlField.val(field.Value);
                }
                else {
                    if (htmlField.attr('hasInsideDefault') == 'hasInsideDefault') {
                        var dependencyValue = getInsideDeafult(htmlField, guid, prefix);
                        if (dependencyValue != null && dependencyValue != '') {
                            htmlField.val(dependencyValue);

                            if (htmlField.attr('insideDependency') == 'insideDependency') {
                                var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
                                var dependentFieldNames = htmlField.attr('dependentFieldNames').split(';');
                                Dependency.LoadInside(dependentFieldViewName, dependentFieldNames, prefix, htmlField[0], guid);
                            }
                        }
                        else {
                            LoadSelect(views[guid].ViewName, htmlField.attr('name'), htmlField, guid);
                        }
                    }
                    else {
                        htmlField.val(field.Default);
                    }
                }
            }
        }
    }
}

LoadSelect = function (viewName, fieldName, select, guid) {

    var data = {
        viewName2: viewName,
        fieldName: fieldName
    };

    var url = views[guid].gGetSelectListUrl;
    url = url.replace('GetSelectList', 'GetSelectList2');

    var isChecklist = select.length > 0 && select[0].multiple;


    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: data,
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (selectList) {
            var val = select.val();

            Dependency.LoadSelectList(select, selectList.Options);

            if (!isChecklist || select.attr('hasInsideTrigger') != 'hasInsideTrigger')
                select.val(val);

            $(Dependency).trigger('onafterLoadSelectList', { select: select, guid: guid });

        }

    });

}

getInsideDeafult = function (htmlField, guid, prefix) {
    var triggerFieldName = htmlField.attr('triggerName');
    if (triggerFieldName == '' || triggerFieldName == null)
        return null;
    var triggerField = $('#' + guid + prefix + ReplaceNonAlphaNumeric(triggerFieldName));
    if (triggerField.length == 0) {
        modalErrorMsg('The trigger field ' + triggerFieldName + " does not exists in the dialog please confirm that it is not hidden in create.");
        return null;
    }
    var triggerValue = triggerField.val();
    var dependencyViewName = htmlField.attr('dependencyViewName');
    var dependencyFieldName = htmlField.attr('dependencyFieldName');
    if (dependencyFieldName == null || dependencyFieldName == '' || dependencyViewName == null || dependencyViewName == '') {
        modalErrorMsg('The trigger field ' + triggerFieldName + " does not contain the dependency information. Please make sure it is a drop down element.");
        return null;
    }

    var dependencyValue = Data.GetScalar(dependencyViewName, triggerValue, dependencyFieldName, guid);

    return dependencyValue;
}

AddDialog.SetDisDup = function (json, prefix, guid, dup) {
    if (json != null) {
        for (var index = 0, len = json.Fields.length; index < len; ++index) {
            var field = json.Fields[index].Value;
            var htmlField = $('#' + guid + prefix + ReplaceNonAlphaNumeric(field.Name));
            var disDup = field.DisDup;
            var disabled = field.Disabled;
            var name = htmlField.attr('name');
            var viewName = views[guid].ViewName;
            var container = htmlField.parents('#the' + guid + prefix + viewName + '_' + name);
            if (container.length > 0)
                container = htmlField.parents('#the' + guid + prefix + viewName + '_' + ReplaceNonAlphaNumeric(name));
            var inlines = null;
            if (container.length > 0) {
                inlines = container.find('img');
            }
            if (dup) {
                if (disDup && !disabled) {
                    htmlField.attr("disabled", "disabled");
                    if (inlines != null) {
                        inlines.hide();
                    }
                }
            }
            else {
                if (disDup && !disabled) {
                    htmlField.removeAttr("disabled");
                    if (inlines != null) {
                        inlines.show();
                    }
                }
            }
        }
    }
}

AddDialog.CreateWysiwyg = function (guid) {
    $('#' + guid + 'DataRowCreate textarea[rich="true"]').each(function () { $(this).htmlarea("dispose") });
    $('#' + guid + 'DataRowCreate textarea[rich="true"]').htmlarea();
}

AddDialog.Close = function (guid) {
    if (views[guid].ReloadPage == "Add" || views[guid].ReloadPage == "Always") {
        reloadPage();
    }

    var dialog = $('#' + guid + 'DataRowCreate');
    var changed = dialog.attr('changed') == 'yes';
    if (changed) {
        if (confirm("Exit without save?")) {
            dialog.attr('changed', 'no');
            if (views[guid].RefreshOnClose == 'True') {
                AddDialog.Refresh(guid, null);
                //                $("div[itemDataRowView='itemDataRowViewCreate']").dialog("option", "zIndex", 900); 
            }
            dialog.dialog('close');
        }
        else {
        }
    }
    else {
        dialog.attr('changed', 'no');
        if (views[guid].RefreshOnClose == 'True') {
            AddDialog.Refresh(guid, null);
            //            $("div[itemDataRowView='itemDataRowViewCreate']").dialog("option", "zIndex", 900);
        }
        dialog.dialog('close');
    }
}

AddDialog.Refresh = function (guid, pk, displayName) {
    saveElementScrollsPosition(guid);
    $.post(views[guid].gRefreshUrl,
        {
            guid: guid
        },
        function (html) {
            hideProgress();
            var indexError = html.indexOf("$$error$$", 0);
            var indexMessage = html.indexOf("$$message$$", 0);
            var hasError = indexError > 0 && indexError < 1000;
            var hasMessage = indexMessage > 0 && indexMessage < 1000;
            if (hasError) {
                ajaxNotSuccessMsg(html, guid);
            }
            else {
                $('#' + guid + 'ajaxDiv').html(html);
                success(guid);
                resetElementScrollsPosition(guid);
                if (pk != null) {
                    Multi.SelectByPK(guid, pk);
                }
                else if (displayName != null) {
                    EditDialog.selectRow(pk, guid, displayName);
                }
            }
        });
}

var blockOperation = false;
AddDialog.Add = function (another, guid, close, allowEdit, insertAbovePK, duplicateRecursive, pk, complete) {
    //    YAHOO.tool.Profiler.start("Save");
    //    YAHOO.tool.Profiler.start("clientBefore");


    if (blockOperation)
        return;

    var dialog = $('#' + guid + 'DataRowCreate');

    var onbeforeAdd = { guid: guid, dialog: dialog, pk: pk, close: close, cancel: false };
    $(AddDialog).trigger('onbeforeAdd', onbeforeAdd);

    if (onbeforeAdd.cancel)
        return;


    if (AddDialog.isValid(guid, views[guid].gViewName) == false) {
        modalSuccessMsg(getErrorsList(dialog));
        blockOperation = false;
        return;
    }

    showProgress();

    blockOperation = true;



    if (dialog.attr('firstTime') == 'no') {
        var pk = dialog.attr('pk');
        //var pk = $('#' + guid + 'ajaxDiv').find('input[name="pk"]').val();
        blockOperation = false;
        AddDialog.Update(pk, guid, another, close, complete);
        return;
    }

    var url = views[guid].gCreateUrl;
    if (complete) {
        url = rootPath + views[guid].Controller + '/CreateStep/' + views[guid].gViewName;

    }
    if (views[guid].gViewName == "Field") {
        var json = GetJsonFilter(guid);
        var mainViewName = '';
        for (var i = 0; i < json.Fields.length; i++) {
            var field = json.Fields[i];
            if (field.Key == "Fields_Parent") {
                mainViewName = field.Value.Default;
                break;
            }
        }
        url = url + '?mainViewName=' + mainViewName;
    }
    //    YAHOO.tool.Profiler.stop("clientBefore");
    //    YAHOO.tool.Profiler.start("server");

    if (views[guid].gViewName == "Field") {
        var dataType = dialog.find('[name="DataType"]');
        //dialog.find('[name=RelatedViewName]').parents('tr:first').parents('tr:first').hide();
        if (dataType.length == 1) {
            if (!isValidDataType(dialog, dataType)) {
                blockOperation = false;
                hideProgress();
                return;
            }
            if (isCreatingNewTableInYourDb(dialog) && !confirm("The following change is going to create new table, continue?")) {
                blockOperation = false;
                hideProgress();
                return;
            }

        }
    }

    var json = FillJson(views[guid].jsonViewForCreate, createPrefix, guid, dialog);

    if (views[guid].gViewName == "Field" && dialog.find('.InSystemDatabase').prop('checked')) {
        var field = jQuery.extend(true, {}, json.Fields[0]);
        field.Key = "sysdb";
        field.Value.Value = true;
        field.Value.Name = "sysdb";
        json.Fields.push(field);
    }


    $.post(url,
        {
            jsonView: Sys.Serialization.JavaScriptSerializer.serialize(json), guid: guid, insertAbovePK: insertAbovePK, duplicateRecursive: duplicateRecursive, duplicatePK: pk
        },
        function (html) {
            //            YAHOO.tool.Profiler.stop("server");
            //            YAHOO.tool.Profiler.start("clientAfter");

            if (!allowEdit) {
                dialog.parent().find('.ui-dialog-buttonpane button').each(function () {
                    if ($(this).text() == translator.saveAndClose || $(this).text() == translator.save) {
                        $(this).attr('disabled', 'disabled');
                        $(this).addClass('ui-state-disabled');
                    }
                });
            }

            if (complete && html.IsResult) {
                AddDialog.SaveWithoutClose(dialog, html.PK, guid);
                dialog.attr('firstTime', 'no');
                WorkFlow.CompleteStep(html.PK, guid, views[guid].WorkFlowStepsFieldName, html, dialog);
            }
            else {

                hideProgress();
                var indexError = html.indexOf("$$error$$", 0);
                var indexMessage = html.indexOf("$$message$$", 0);
                var hasError = indexError > 0 && indexError < 1000;
                var hasMessage = indexMessage > 0 && indexMessage < 1000;
                if (hasError) {
                    //AddDialog.ShowFailureMessage(html.replace("$$error$$", ""), guid);
                    ajaxNotSuccessMsg(html, guid);
                }
                else if (hasMessage) {
                    //AddDialog.ShowFailureMessage(html.replace("$$message$$", ""), guid);
                    ajaxNotSuccessMsg(html, guid);
                    showProgress();
                    refreshView(guid);
                    if (!another)
                        AddDialog.Close(guid);
                }
                else {
                    AddDialog.HandleSuccess(html, another, guid);
                    if (views[guid].updateParent) {
                        updateParent(guid);
                    }
                    else if (views[guid].updateParentGrid) {
                        updateParentGrid(guid);
                    }
                    var pk = $('#' + guid + 'ajaxDiv').find('input[name="pk"]').val();
                    PreviewDisplay.selectedPk = pk;
                    //                    AddDialog.HandleTab(pk, 'CreateTabs', guid);
                    //                    dialog.attr('pk', pk);
                    //                    dialog.attr('changed', 'no');

                    //                    dialog.find('img.expand').each(function() {
                    //                        $(this).removeAttr('disabled');
                    //                        $(this).attr('title', 'Expand');
                    //                        var src = $(this).attr('src');
                    //                        $(this).attr('src', src.replace('Disabled', ''));
                    //                    });

                    //                    // sandisk alegro
                    //                    dialog.find('input.inlineAddingImg').each(function() {
                    //                        $(this).parent('div').siblings('div').attr('pk', pk);
                    //                        expand(this, true, null, false, guid);
                    //                    });
                    AddDialog.SaveWithoutClose(dialog, pk, guid);

                    if (close) {
                        AddDialog.Close(guid);
                    }

                    if (isDockFields(guid)) {
                        handleToolbar(guid);
                        $("#d_row_" + guid + pk).click();
                        //                        setTimeout(function () {
                        //                            EditDialog.Open2(pk, guid, false);
                        //                        }, 100);
                        setTimeout(function () {
                            EditDialog.Open2(pk, guid, false);
                        }, 1);

                        try {
                            showProgress();
                            var newColumnName = $('#' + guid + 'ajaxDiv .boardtitle[d_pk=' + pk + ']').text().trim();
                            newColumnName = ReplaceNonAlphaNumeric(newColumnName);
                            try {
                                window.parent.parent.Durados.GridHandler.setNewColumnWidth(window.parent.parent.getMainPageGuid(), newColumnName);
                            }
                            catch (err) {
                            }
                            setTimeout(function () {
                                window.parent.parent.Refresh(window.parent.parent.getMainPageGuid(), true);
                            }, 50);
                        }
                        catch (err) {
                            hideProgress();
                        }
                        //$('div[d_pk="' + pk + '"]').click();
                    }
                    else if (views[guid].ViewName == 'Field' && views[guid].DataDisplayType == "Preview") {
                        $("#d_row_" + guid + pk).click();
                        setTimeout(function () {
                            EditDialog.Open2(pk, guid, false);
                        }, 1);
                    }

                    plugInRefresh();

                }


                blockOperation = false;
            }
            if (!hasError) {
                var pk2 = $('#' + guid + 'ajaxDiv').find('input[name="pk"]').val();
                $(AddDialog).trigger('onafterAdd', { guid: guid, dialog: dialog, pk: pk2, close: close });
            }
            //            if (isDockFields(guid)) {
            //                $("#overlay", window.parent.document).hide();
            //            }
        });
}

AddDialog.SaveWithoutClose = function (dialog, pk, guid) {
    AddDialog.HandleTab(pk, 'CreateTabs', guid);
    dialog.attr('pk', pk);
    dialog.attr('changed', 'no');

    dialog.find('img.expand').each(function () {
        $(this).removeAttr('disabled');
        $(this).attr('title', 'Expand');
        var src = $(this).attr('src');
        $(this).attr('src', src.replace('Disabled', ''));
    });

    // sandisk alegro
    dialog.find('span.inlineAddingImg').each(function () {
        $(this).parent('div').siblings('div').attr('pk', pk);
        expand(this, true, null, false, guid);
    });
}

AddDialog.Update = function (pk, guid, another, close, complete) {

    var dialog = $('#' + guid + 'DataRowCreate');

    if (blockOperation)
        return;

    if (AddDialog.isValid(guid, views[guid].gViewName) == false) {
        modalSuccessMsg(getErrorsList(dialog));
        return;
    }

    begin();

    blockOperation = true;

    var url = views[guid].gEditUrl;
    if (complete) {
        url = rootPath + views[guid].Controller + '/EditStep/' + views[guid].gViewName;

    }

    $.post(url,
        {
            pk: pk,
            jsonView: Sys.Serialization.JavaScriptSerializer.serialize(FillJson(GetJsonViewForEdit(guid), createPrefix, guid, dialog)), guid: guid
        },
        function (html) {
            if (complete && html.IsResult) {
                WorkFlow.CompleteStep(pk, guid, views[guid].WorkFlowStepsFieldName, html, dialog);
            }
            else {
                var indexError = html.indexOf("$$error$$", 0);
                var indexMessage = html.indexOf("$$message$$", 0);
                var hasError = indexError > 0 && indexError < 1000;
                var hasMessage = indexMessage > 0 && indexMessage < 1000;
                if (hasError) {
                    //AddDialog.ShowFailureMessage(html.replace("$$error$$", ""), guid);
                    ajaxNotSuccessMsg(html, guid);
                }
                else if (hasMessage) {
                    //AddDialog.ShowFailureMessage(html.replace("$$message$$", ""), guid);
                    ajaxNotSuccessMsg(html, guid);
                    showProgress();
                    refreshView(guid);
                    AddDialog.Close(guid);
                }
                else {
                    AddDialog.HandleSuccess(html, another, guid);
                    dialog.attr('changed', 'no');
                    if (close) {
                        AddDialog.Close(guid);
                    }
                }
                blockOperation = false;
                hideProgress();
            }
        });
}

AddDialog.DisableTab = function (tasPostfix, guid) {
    var createTab = $('#' + guid + tasPostfix);
    Durados.Tabs.select(createTab, 0);
    //    createTab.tabs("select", 0)
    var tabsWithChildren = new Array();
    createTab.find('li[index][haschildren]').each(function () {
        var index = parseInt($(this).attr('index'));
        tabsWithChildren.push(index);
    });
    createTab.tabs("option", "disabled", tabsWithChildren);
}

AddDialog.HandleTab = function (pk, tasPostfix, guid) {
    var createTab = $('#' + guid + tasPostfix);
    createTab.tabs("option", "disabled", false);
    if (pk[pk.length - 1] == '#')
        pk = pk.slice(0, -1);

    var pkSplit = pk.split(',');
    createTab.find('li[haschildren="haschildren"]').each(function () {
        //var a = $(this).children('a');
        var index = parseInt($(this).attr('index'));
        var url = unescape($(this).attr('url'));
        var urlSplit = url.split('$$');

        var newUrl = '';

        for (var i = 0, len = pkSplit.length; i < len; ++i) {
            newUrl = newUrl + urlSplit[i] + pkSplit[i];
        }

        newUrl = newUrl + urlSplit[urlSplit.length - 1] + "&children=true";

        createTab.tabs("url", index, newUrl);

        $(this).attr('newUrl', newUrl);

        // a.attr('href', newHref);
    });
}

AddDialog.HandleSuccess = function (html, another, guid) {
    var dialog = $('#' + guid + 'DataRowCreate');
    if (another) {
        AddDialog.Reset(guid, false);
        dialog.attr('firstTime', 'yes');

    }
    else {
        //AddDialog.Close(guid);
        dialog.attr('firstTime', 'no');

    }
    $('#' + guid + 'ajaxDiv').html(html);
    success(guid);
    views[guid].updateCounter();
}

//AddDialog.ShowFailureMessage = function(message, guid) {
//    alert(message);
//    complete(guid);
//}

AddDialog.isValid = function (guid, viewName) {

    var form = $('#' + guid + 'Create' + ReplaceNonAlphaNumeric(viewName) + 'DataRowForm');

    var arr = GetIncompleteDependencies(form);

    if (!isIE()) {
        //form.find('iframe:first').contents().find("body").focus();

        form.find('iframe').each(function () {
            if ($(this).parent().parent().attr('class') == 'jHtmlArea') {
                var text = $(this).contents().find("body").html();
                if (text != '') {
                    var textarea = $(this).parent().parent().find('textarea');
                    if (textarea.text() == '') {
                        textarea.text(text);
                    }
                }
            }
        });
        //form.find('iframe').contents().find("body")
    }

    var isSpryValid = Spry.Widget.Form.validate(form[0]);
    var isValidUpload = IsValidUpload(form[0]);
    if (isSpryValid == false || isValidUpload == false) {
        SetBackIncompleteDependencies(form, arr);
        return false;
    }
    return true;
}

function GetIncompleteDependencies(form) {
    var arr = new Array();

    var tds = form.find('td');

    tds.each(function (index, element) {
        var selects = $(element).find('select');
        if (selects.length > 1) {
            var firstSelect = $(selects[0]);
            var lastSelect = $(selects[selects.length - 1]);
            if (firstSelect.val() != '' && lastSelect.val() == '') {
                arr.push({ select: firstSelect, val: firstSelect.val() });
                firstSelect.val('');
            }
        }
    });

    return arr;
}

function SetBackIncompleteDependencies(form, arr) {
    $.each(arr, function (index, value) {
        value.select.val(value.val);
    });
}

// rich
var RichDialog; if (!RichDialog) RichDialog = {};

var richUploader = null;
var richUploadDiv = null;

RichDialog.OpenUploadDialog = function (viewName, fieldName, guid, callback, file, src) {

    //var dialog = $('<div><DIV class=uploadDiv><SPAN class=gbutton>Upload</SPAN><IMG style="DISPLAY: none; VISIBILITY: visible"  title=Icon alt=Icon jQuery1342700698105="266" UploadPath="/uploads/"><INPUT disabled name=Image type=text jQuery1342700698105="256" autocomplete="on" upload="upload"></DIV></div>');

    if (richUploadDiv)
        richUploadDiv.remove();

    var dialog = $('<div>' + RichDialog.GetUploadElement(viewName, fieldName, guid) + '</div>');
    richUploadDiv = dialog;

    var title = "Upload Image";


    FloatingDialog.Open(dialog, title, true, guid);

    dialog.dialog("option", "height", 'auto');
    dialog.dialog("option", "width", 'auto');
    dialog.dialog("option", "resizable", false);
    try {
        if (richUploader)
            richUploader.destroy();
    }
    catch (e) {
    }
    if (richUploader)
    {
        richUploader = null;
    }
    var fixedFieldName = ReplaceNonAlphaNumeric(fieldName);
    var oldUploadId = '#' + guid + createPrefix + 'upload_' + fixedFieldName;
    var newUploadId = guid + inlineEditingPrefix + '_' + 'upload_' + fixedFieldName;
    dialog.find(oldUploadId).attr('id', newUploadId);

    oldUploadId = '#' + guid + createPrefix + 'upload_span_' + fixedFieldName;
    newUploadId = guid + inlineEditingPrefix + '_' + 'upload_span_' + fixedFieldName;
    dialog.find(oldUploadId).attr('id', newUploadId);

    oldUploadId = '#' + guid + createPrefix + fixedFieldName;
    newUploadId = guid + inlineEditingPrefix + '_' + fixedFieldName;
    dialog.find(oldUploadId).attr('id', newUploadId);

    oldUploadId = '#' + guid + createPrefix + 'upload_img_' + fixedFieldName;
    newUploadId = guid + inlineEditingPrefix + '_' + 'upload_img_' + fixedFieldName;
    dialog.find(oldUploadId).attr('id', newUploadId);
    //    'DownloadIcon_' 'DeleteIcon_'

    richUploader = UploadFile(fieldName, inlineEditingPrefix + '_', guid);
    showUpload(fieldName, inlineEditingPrefix + '_', file, src, guid);
    //    setTimeout(function () {
    //        dialog.find('input[type="text"]:first').focus();
    //    }, 100);

    hideProgress();

    var buttons = {};
    buttons['OK'] = function () {
        blockOperation = false;
        var url = dialog.find('img[UploadPath]').attr('UploadPath');
        var file = dialog.find('input[type="text"]').val();
        if (url && url.indexOf("__filename__") > -1) {
            if (file) {
                url = url.replace('__filename__', encodeURI(file));
            }
        }
        else {
            if (url) {
                if (file) {
                    url += file;
                }
            }
            else {
                url = file;
            }
        }
        dialog.dialog('close');
        callback(url, file);
    };
    buttons['Cancel'] = function () {
        dialog.dialog('close');
    };
    dialog.dialog("option", "buttons", buttons);

    dialog.bind("dialogclose", function (event, ui) {
        dialog.dialog("destroy");
    });

    return dialog;
}

RichDialog.GetButtons = function (dialog, fieldName, disabled, rich, guid, callback) {
    var buttons = {};  //initialize the object to hold my buttons
    if (!disabled) {
        buttons[translator.saveAndClose] = function () {
            RichDialog.Update(dialog, fieldName, rich, guid, callback); //the function that does the save
            $(this).dialog("close");
        }
        buttons[translator.close] = function () { $(this).dialog("close"); }
    }
    else {
        buttons[translator.close] = function () { $(this).dialog("close"); }
    }
    return buttons;
}

RichDialog.Open = function (fieldName, disabled, rich, guid, value, callback) {
    //RichDialog.Open = function (pk, fieldName, disabled, rich, guid, elm, value, callback) {
    begin();

    var dialog;
    if (rich) {
        if (!disabled)
            dialog = $('#rich');
        else
            dialog = $('#richDisabled');
    }
    else {
        dialog = $('#notRich');
        if (!disabled) {
            dialog.find('textarea').removeAttr("disabled");
        }
        else {
            dialog.find('textarea').attr("disabled", "disabled");
        }
    }

    dialog.dialog("option", "title", translator.editHtmlOn + views[guid].gViewDisplayName);
    dialog.dialog("option", "buttons", RichDialog.GetButtons(dialog, fieldName, disabled, rich, guid, callback));

    //by br
    dialog.dialog('option', 'height', 'auto');
    dialog.dialog('option', 'width', 'auto');

    dialog.dialog('open');

    RichDialog.Load(dialog, fieldName, disabled, rich, guid, value);

    var textarea = dialog.find('textarea');
    var iframe = dialog.find('iframe');
    var dialogWidth = dialog.width();
    var dialogHeight = dialog.height();
    var textareaWidth = textarea.width();
    var textareaHeight = textarea.height();
    var iframeWidth = iframe.width();
    var iframeHeight = iframe.height();

    dialog.on("dialogresizestop", function (event, ui) {
        var diffWidth = dialog.width() - dialogWidth;
        var diffHeight = dialog.height() - dialogHeight;
        textarea.width(textareaWidth + diffWidth);
        textarea.height(textareaHeight + diffHeight);
        iframe.width(iframeWidth + diffWidth);
        iframe.height(iframeHeight + diffHeight);
    });

    complete(guid);

    if (!disabled) {
        try {
            iframe.contents().find('body').focus();
        } catch (err) { }
    }
    //setFocusInDialog(dialog);
}

RichDialog.spryValidation = null;

RichDialog.Load = function (dialog, fieldName, disabled, rich, guid, value) {

    var html = value;
    //    var html = RichDialog.GetValue(pk, fieldName, guid);

    if (!disabled) {

        var jsonView = GetJsonViewForEdit(guid);

        var field;
        try {
            field = getFieldJson(fieldName, jsonView).Fields[0].Value;
        } catch (ex) { }


        //delete all class=wysiwyg to clear the textarea
        //$(".wysiwyg").remove();


        var extraParams = {};
        var containerId;
        if (rich) {
            containerId = 'richSpan';
        } else {
            containerId = 'notRichSpan';
        }

        if (RichDialog.spryValidation) RichDialog.spryValidation.reset();

        var textarea = $('#' + containerId).find('textarea');

        if (rich) { textarea.htmlarea("dispose"); }

        textarea.attr('name', fieldName);
        textarea.attr('viewName', views[guid].gViewName);

        if (field) {
            extraParams.isRequired = field.Required;

            if (field.Max && field.Max > 0) {
                extraParams.maxChars = field.Max;
                //extraParams.validateOn = 'change';
                extraParams.useCharacterMasking = true;
            }
        }
        if (!rich)
            RichDialog.spryValidation = new Spry.Widget.ValidationTextarea(containerId, extraParams);

        textarea.val(html);

        if (rich) {
            textarea.htmlarea(); //{"maxLength" : 20 }create the wysiwyg
        }

    }
    else {
        if (rich) {
            $('#richDiv').html(html);
        } else {
            $('#notRichTextArea').val(html);
        }
    }

}

RichDialog.GetUploadElement = function (viewName, fieldName, guid) {
    var syncJsonView = null;
    var controller = views[guid].Controller;
    var url = rootPath + controller + "/GetUploadForRich/" + viewName;
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: viewName, fieldName: fieldName, guid: guid },
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (html) {
            syncJsonView = html;

        }

    });

    return syncJsonView;
}


RichDialog.GetValue = function (pk, fieldName, guid) {
    var syncJsonView = null;
    $.ajax({
        url: views[guid].gGetRichUrl,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: views[guid].gViewName, pk: pk, fieldName: fieldName },
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (html) {
            syncJsonView = html;

        }

    });

    return syncJsonView;
}

RichDialog.Update = function (dialog, fieldName, rich, guid, callback) {
    //if (RichDialog.spryValidation && !RichDialog.spryValidation.validate()) return;

    var textarea;
    if (rich) {
        textarea = $('#richTextArea');
    }
    else {
        textarea = dialog.find("textarea");
    }

    var html = textarea.val();


    if (rich) {

        var jsonView = GetJsonViewForEdit(guid);

        var field;
        try {
            field = getFieldJson(fieldName, jsonView).Fields[0].Value;
            if (field && field.Max && html.length > field.Max) {
                ajaxNotSuccessMsg("Html limited to " + field.Max + " characters!"); return;
            }
        } catch (ex) { }


    }

    if (callback) {
        callback(html);
    }
    //    saveElementScrollsPosition(guid);
    //    begin();


    //    $.post(views[guid].gEditRichUrl,
    //        {
    //            pk: pk,
    //            fieldName: fieldName,
    //            html: html,
    //            guid: guid
    //        },
    //        function (html) {
    //            var index = html.indexOf("$$error$$", 0)
    //            if (index < 0 || index > 1000) {
    //                RichDialog.HandleSuccess(html, rich, guid, elm);
    //            }
    //            else {
    //                //RichDialog.ShowFailureMessage(html.replace("$$error$$", ""), guid);
    //                ajaxNotSuccessMsg(html, guid);
    //            }
    //        });
}

RichDialog.HandleSuccess = function (html, rich, guid, elm) {
    RichDialog.Close(rich);
    //$('#' + guid + 'ajaxDiv').html(html);
    //success(guid);
    //resetElementScrollsPosition(guid);
    $(elm).parent('div').html(html);
    hideProgress();
}

//RichDialog.ShowFailureMessage = function(message, guid) {
//    alert(message);
//    complete(guid);
//}

RichDialog.Close = function (rich) {
    if (rich)
        $('#rich').dialog('close');
    else
        $('#notRich').dialog('close');
}

// Dependency
var Dependency; if (!Dependency) Dependency = {};

Dependency.Init = function (htmlField, prefix, guid) {
    if (htmlField.attr('insideDependency') == 'insideDependency') {
        var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
        var dependentFieldNames = htmlField.attr('dependentFieldNames').split(';');
        Dependency.LoadInside(dependentFieldViewName, dependentFieldNames, prefix, htmlField[0], guid);
        insideDependencyChange(htmlField, guid);
    }
}

Dependency.LoadInside = function (viewName, fieldNames, prefix, trigger, guid) {
    var fk = $(trigger).val();
    var pk = $(trigger).attr('dpk');
    $(fieldNames).each(function () {
        var select = $('#' + guid + prefix + ReplaceNonAlphaNumeric(this));
        if (!fk && !($(trigger).attr('name') == fieldNames[0]))
            Dependency.Clear(select);
        else
            Dependency.Load(viewName, this + '', fk, select, guid, pk);

        if (select.attr('role') == 'childrenCheckList') {
            initDropdownchecklist(select);
        }
    });
}

$(Dependency).bind('onbeforeLoadSelectList', function (e, data) {
    if (data.data.viewName2 == "View" && data.data.fieldName == "DisplayColumn") {
        data.data.viewName2 = "Field";
        data.data.fk = data.pk;
    }
});

Dependency.Load = function (viewName, fieldName, fk, select, guid, pk, url) {

    var data = {
        viewName2: viewName,
        fieldName: fieldName,
        fk: fk
    };

    if (!url)
        url = views[guid].gGetSelectListUrl;

    if (viewName == "View" && fieldName == "DisplayColumn") {
        url = "/Admin/GetSelectList";
    }

    var isFilter = select.attr('filter') == 'filter';
    var isChecklist = select.length > 0 && select[0].multiple;

    if (!isFilter && isChecklist && select.attr('hasInsideTrigger') == 'hasInsideTrigger') {
        data.pk = pk || '';
        url = url.replace('GetSelectList', 'GetDependencyCheckList');
    }

    var eventData = { select: select, guid: guid, url: url, data: data, pk: pk };
    $(Dependency).trigger('onbeforeLoadSelectList', eventData);

    if (!data.viewName2 || !data.fieldName)
        return;

    $.ajax({
        url: eventData.url,
        contentType: 'application/json; charset=utf-8',
        data: data,
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (selectList) {
            var val = select.val();

            Dependency.LoadSelectList(select, selectList.Options);

            if (!isChecklist || select.attr('hasInsideTrigger') != 'hasInsideTrigger')
                select.val(val);

            $(Dependency).trigger('onafterLoadSelectList', { select: select, guid: guid });

        }

    });

}

Dependency.onafterLoadSelectList = function () { }

Dependency.LoadSelectList = function (select, options) {
    Dependency.Clear(select);
    var selected;
    for (var index = 0, len = options.length; index < len; ++index) {
        if (options[index].Selected) {
            selected = ' selected="selected"';
        } else {
            selected = '';
        }
        select.append('<option value="' + options[index].Value + '"' + selected + '>' + options[index].Text + '</option>');
    }
}

Dependency.Clear = function (select) {
    var first = select.children(":first").text();

    select.empty();
    select.append('<option value="">' + first + '</option>');
}


//Dependency.Clear = function(select) {
//    var first = select.children(":first").text();
//    if (first == '')
//        first = translator.all;
//        
//    select.empty();

//    select.append('<option value="">' + first + '</option>');
//}

Dependency.ClearAll = function (select, guid) {
    var prefix = GetPrefix(select, guid);

    while (select != null) {
        Dependency.Clear(select);
        if (select.attr('outsideDependency') == 'outsideDependency') {
            var dependentFieldName = select.attr('dependentFieldName');
            var dependentFieldViewName = select.attr('dependentFieldViewName');
            select = $('#' + guid + prefix + ReplaceNonAlphaNumeric(dependentFieldName));
        }
        else {
            select = null;
        }
    }
}




function GetPrefix(element, guid) {
    var prefix = null;

    if (element.parents().index($('#' + guid + 'DataRowCreate')) >= 0) {
        prefix = createPrefix;
    }
    else if (element.parents().index($('#' + guid + 'DataRowEdit')) >= 0) {
        prefix = editPrefix;
    }
    else if (element.parents('form:first').attr('d_prefix') == 'InlineAdding') {
        prefix = inlineAddingPrefix;
    }
    else if (element.parents('form:first').attr('d_prefix') == 'InlineEditing') {
        prefix = inlineEditingPrefix;
    } else {
        prefix = createPrefix
    }
    return prefix;
}



var WorkFlow; if (!WorkFlow) WorkFlow = {};

WorkFlow.CompleteStep = function (pk, guid, WorkFlowStepsFieldName, result, prevDialog) {
    var allowEdit = EditDialog.AllowEdit(guid, pk);
    if (allowEdit.disabled) {
        ajaxNotSuccessMsg(allowEdit.message, guid);
        return;
    }

    if (prevDialog != null)
        prevDialog.attr('changed', 'no');

    if (typeof (result) == 'string') {
        var index = result.indexOf("$$error$$", 0)
        var error = !(index < 0 || index > 1000);

        if (error) {
            blockOperation = false;
            ajaxNotSuccessMsg(result, guid);
            return;
        }
    }

    if (result.IsResult) {
        if (result.MessageOnly) {
            modalSuccessMsg(result.WorkflowCompletedMessage);
            blockOperation = false;
            if (prevDialog != null)
                prevDialog.dialog('close');
            AddDialog.Refresh(guid, pk);
        }
        else {
            WorkFlow.OpenStepDialog(pk, guid, WorkFlowStepsFieldName, result, prevDialog);
            hideProgress();
        }
    }
    else {
        EditDialog.HandleSuccess(result, guid);
    }
    blockOperation = false;

}

WorkFlow.OpenStepDialog = function (pk, guid, WorkFlowStepsFieldName, result, prevDialog) {
    var dialog = WorkFlow.CreateStepDialog(pk, guid, WorkFlowStepsFieldName, result, prevDialog);
    var title = views[guid].gViewDisplayName + ' ' + translator.WorflowSteps;

    FloatingDialog.Open(dialog, title, true, guid);

    $("#stepsTabs").tabs({
        select: function (event, ui) {
            if (ui.index == 1) {
                var dialogIframe = $('#stepsGraphTab iframe');
                if (dialogIframe.attr("src") == "about:blank") {
                    var guid = FloatingDialog.TheDialog.attr("guid");

                    dialogIframe.attr("src", rootPath + views[guid].Controller + "/WorkFlowGraph/" + views[guid].gViewName + "?viewName=" + views[guid].gViewName);
                }
            }
            return true;
        }
    });

    $("#stepsTabs").tabs('select', '#stepsVisualTab');

    dialog.bind('dialogresize', function (event) {
        setDialogIframeSize('stepsGraphTabIframe');
    });

    dialog.bind('dialogclose', function (event) {
        $('#stepsGraphTabIframe').attr("src", "about:blank");
    });

    var buttons = {};
    buttons[translator.close] = function () {
        if (prevDialog != null) {
            prevDialog.attr('changed', 'no');
            prevDialog.dialog('close');
        }
        blockOperation = false;
        dialog.dialog('close');
        AddDialog.Refresh(guid, pk);
    };
    buttons[translator.cancel] = function () {
        dialog.dialog('close');
    };
    dialog.dialog("option", "buttons", buttons);

}

WorkFlow.CompleteStepUpdate = function (pk, guid, WorkFlowStepsFieldName, result, EnabledStep, dialog, prevDialog) {
    var url = rootPath + views[guid].Controller + '/CompleteStep/' + views[guid].gViewName;

    $.post(url,
    {
        viewName: views[guid].gViewName, pk: pk, guid: guid, stepId: EnabledStep.Id, prevStepId: result.CurrentStepId
    },
    function (html) {
        var index = html.indexOf("$$error$$", 0)
        if (index < 0 || index > 1000) {
            EditDialog.HandleSuccess(html, guid);
            if (prevDialog != null)
                prevDialog.dialog('close');
            dialog.dialog('close');
        }
        else {
            ajaxNotSuccessMsg(html, guid);
        }
        hideProgress();
    });

}

WorkFlow.CreateStepDialog = function (pk, guid, WorkFlowStepsFieldName, result, prevDialog) {
    var dialog = $('<div />');

    var showDisabledSteps = views[guid].showDisabledSteps;

    $('#stepsTabs').remove();

    var tabsDiv = $('<div id="stepsTabs"></div>');

    var div = $('<div id="stepsVisualTab" class="WfStep"></div>');

    var center = $('<center></center>');

    div.append(center);


    if (result.Message != null && result.Message != '') {
        var p = $('<p></p>');
        var messageSpen = $('<span></span>');
        var moreInfoSpen = $('<span class="moreInfo"></span>');
        messageSpen.text(result.Message);
        p.append(messageSpen);
        if (result.Description != null && result.Description != '') {
            moreInfoSpen.text(' ' + translator.MoreInfo);
            moreInfoSpen.attr('title', result.Description);
            p.append(moreInfoSpen);
            initTooltip(moreInfoSpen);
        }
        center.append(p);
    }

    if (result.EnabledSteps != null) {
        var table = $('<table></table>');
        center.append(table);


        var showAllSteps = translator.ShowAllSteps == null ? "Show All Steps:" : translator.ShowAllSteps;
        var checkbox = $('<input type="checkbox" />');
        //checkbox.attr('checked', showDisabledSteps);
        Durados.CheckBox.SetChecked(checkbox, showDisabledSteps);

        var trHeader = $('<tr></tr>');
        table.append(trHeader);
        trHeader.attr('enabled', true);

        var thHeader = $('<td colspan="2" align="right">' + showAllSteps + '</td>');
        trHeader.append(thHeader);

        var thFilter = $('<td></td>');
        trHeader.append(thFilter);
        thFilter.append(checkbox);

        $(result.EnabledSteps).each(function () {
            var EnabledStep = this;
            var tr = $('<tr></tr>');
            var urHereTd = $('<td class="urHere" valign="middle" align="right"></td>');
            if (result.CurrentStepId == EnabledStep.Id) {
                var span = $('<span style="vertical-align:middle"></span>');
                span.text(translator.UrHere);
                var arrowImg = $('<img  height="20px"  width="20px" ALIGN="middle"/>');
                arrowImg.attr('src', '/Content/Images/forward.png');
                urHereTd.append(span);
                urHereTd.append(arrowImg);
            }

            var nameTd = $('<td class="wfStepName" align="left"></td>');
            var nameSpen = $('<span></span>');
            nameSpen.text(EnabledStep.Name);
            nameTd.append(nameSpen);
            var src = EnabledStep.Enable ? '/Content/Images/GreenCheck.png' : '/Content/Images/RedCross.png';
            var img = $('<img />');
            img.attr('src', src);
            img.attr('alt', EnabledStep.Name);
            img.attr('title', EnabledStep.Description);
            nameSpen.attr('title', EnabledStep.Description);

            var buttonTd = $('<td align="center"></td>');

            buttonTd.append(img);
            if (EnabledStep.Enable) {
                tr.attr('class', 'enabledStep');
                tr.click(function () {
                    showProgress();
                    setTimeout(function () {
                        WorkFlow.CompleteStepUpdate(pk, guid, WorkFlowStepsFieldName, result, EnabledStep, dialog, prevDialog);
                    }, 1);
                });
            }
            else {
                tr.attr('class', 'disabledStep');

            }

            tr.append(urHereTd);
            tr.append(nameTd);
            tr.append(buttonTd);
            tr.attr('enabled', EnabledStep.Enable);

            table.append(tr);
            if (EnabledStep.Description != null && EnabledStep.Description != '') {
                initTooltip(nameSpen);
                initTooltip(img);
            }

        });

        table.find('tr').each(function () {
            if ($(this).attr('enabled') == 'false') {
                if (!showDisabledSteps) {
                    $(this).hide();
                }
            }
        });
        checkbox.click(function () {
            if (!checkbox.is(':checked')) {
                table.find('tr').each(function () {
                    if ($(this).attr('enabled') == 'false') {
                        $(this).hide();
                    }
                });
            }
            else {
                table.find('tr').each(function () {
                    $(this).show();
                });
            }
        });
    }


    tabsDiv.prepend('<div id="stepsGraphTab"><iframe id="stepsGraphTabIframe" class="dialogIframe" src="about:blank" frameborder="0" onload="setDialogIframeSize(this.id)" scrolling="auto" height="100%" width="100%"></iframe></div>');

    tabsDiv.prepend(div);

    tabsDiv.prepend('<ul><li><a href="#stepsVisualTab">Steps</a></li><li><a href="#stepsGraphTab">Workflow</a></li></ul>');

    return dialog.append(tabsDiv);
}

function setDialogIframeSize(id) {
    var h = $('#' + id).closest('div.ui-dialog-content').height() - 60;
    $('#' + id).height(h);
}

// edit
var EditDialog; if (!EditDialog) EditDialog = {};

EditDialog.GetButtons = function (pk, guid, hideClose, allowEdit) {
    var buttons = {};  //initialize the object to hold my buttons

    //   
    //var allowEdit = !EditDialog.AllowEdit(guid, pk).disabled;
    if (!hideClose) {
        if (allowEdit) {
            buttons[translator.saveAndClose] = function () { EditDialog.Update(pk, guid, true, false, false); }  //the function that does the save
        }
    }
    else if (isDock(guid)) {
        var cancelButtonName = translator.cancel;
        if (isDockFields(guid) && window.parent.parent.queryString("settings") != "fields") {
            cancelButtonName = translator.Settings;
        }
        buttons[cancelButtonName] = function () {
            if (isDockFields(guid)) {
                if (window.parent.parent.queryString("settings") == "fields") {
                    var f = null;
                    var isViewOwner = views[guid] == null ? false : views[guid].Role == "View Owner";
                    if (isViewOwner) {
                        f = window.parent.parent.reloadPage;
                    }
                    else {
                        f = function () {
                           window.parent.parent.location = window.parent.parent.location.href.replace('settings=fields', '');
                        }
                    }
                    Durados.Dialogs.Confirm(translator.CancelChangesTitle, translator.CancelChangesMessage,
                    f, null);
                }
                else {
                    CancelPreview(views[guid].ViewName);
                    window.parent.parent.Refresh(window.parent.parent.getMainPageGuid());
                    window.parent.parent.viewProperties(window.parent.parent.getMainPageGuid(), false, window.parent.parent.dock.width, window.parent.parent.dock.url, window.parent.parent.dock.viewDisplayName);
                }


            }
            else {
                EditDialog.Close(guid, pk, false, true);
            }
        }
    }
    else {
        if (views[guid].ViewName == 'Field') {
            if (window.parent) {
                var backUrl = decodeURIComponent(queryString2(window.parent.location.href, 'backUrl'));
                buttons[translator.cancel] = function () { window.parent.location = backUrl; }

            }
        }
        //$(EditDialog).trigger('closeButton', { buttons: buttons, translator: translator, guid: guid});
    }

    if (allowEdit) {
        buttons[translator.save] = function () { EditDialog.Update(pk, guid, false, false, false); }  //the function that does the save
    }

    if (allowEdit && views[guid].WorkFlowStepsFieldName != '') {
        var promoteButtonEventArgs = { pk: pk, guid: guid, hide: false };
        $(EditDialog).trigger('promoteButton', promoteButtonEventArgs);
        if (!promoteButtonEventArgs.hide) {
            buttons[translator.saveAnd + views[guid].PromoteButton] = function () { EditDialog.Update(pk, guid, false, false, true); }  //the function that does the save & complete
        }
    }

       if (allowEdit && views[guid].ShowUpDown) {
           buttons['->'] = function() { EditDialog.Next(pk, guid); }
           buttons['<-'] = function() { EditDialog.Prev(pk, guid); }
       }

    if (allowEdit && isAdmin(guid) && !d_autoCommit) {
        buttons[translator.saveAndCommit] = function () {
            EditDialog.Update(pk, guid, false, true, false);

        }  //the function that does the save
    }

    if (!hideClose) {
        buttons[translator.close] = function () { EditDialog.Close(guid, pk, false, true); }  //the function that does the save
    }

    return buttons;
}

EditDialog.GetButtonsForSelection = function (guid, dialog) {
    var buttons = {};  //initialize the object to hold my buttons
    buttons[translator.saveAndClose] = function () { EditDialog.UpdateSelection(guid, true, dialog); }  //the function that does the save
    buttons[translator.save] = function () { EditDialog.UpdateSelection(guid, false, dialog); }  //the function that does the save
    buttons[translator.cancel] = function () {
        dialog.dialog("close"); 
    }  //the function that does the save 
    return buttons;
}

EditDialog.Open = function (pk, guid, hideClose, callback) {
    //added by br
    var displayType = views[guid].DataDisplayType;
    //    if (displayType == "Preview" && PreviewDisplay.selectedPk == pk) {
    if (displayType == "Preview" && $("#" + guid + "DataRowEdit").attr("pk") == pk) {
        return;
    }
    begin();

    setTimeout(function () {
        EditDialog.Open2(pk, guid, hideClose, callback);
    }, 1);
}

EditDialog.AllowEdit = function (guid, pk) {
    if (views[guid].disabledRows[pk] == null || views[guid].WorkFlowStepsFieldName != '') {
        views[guid].disabledRows[pk] = EditDialog.IsDisabled(guid, pk);
    }
    return views[guid].disabledRows[pk];
}

EditDialog.IsDisabled = function (guid, pk) {
    var isDisabled = { disabled: true, message: '' };
    if (views[guid].HasOpenRules) {
        $.ajax({
            url: rootPath + views[guid].Controller + '/IsDisabled/' + views[guid].gViewName,
            contentType: 'application/json; charset=utf-8',
            data: { viewName: views[guid].gViewName, pk: pk, guid: guid },
            async: false,
            dataType: 'json',
            cache: false,
            error: ajaxErrorsHandler,
            success: function (json) {
                isDisabled = json;
            }
        });
        return isDisabled;
    }
    else {
        return { disabled: !views[guid].AllowEdit, message: '' };
    }
}

function addMessage(dialog, message) {
    var div = dialog.find("div.dialogTitle");
    if (div.length == 1) {
        div.html(message);
        div.show();
    }
}


function removeMessage(dialog) {
    var div = dialog.find("div.dialogTitle");
    if (div.length == 1) {
        div.html('');
        div.hide();
    }
}


EditDialog.Open2 = function (pk, guid, hideClose, callback) {

    if (views[guid] == null)
        return;

    var displayType = views[guid].DataDisplayType;

    var dialog = $('#' + guid + 'DataRowEdit');
    var allowEdit = EditDialog.AllowEdit(guid, pk);

    //changed by br
    //If its preview display- try save previous changes
    if (displayType == "Preview") {
        PreviewDisplay.trySaveChanges(guid);
        PreviewDisplay.saveElementState(guid);
    }

    dialog.dialog("option", "dialogClass", '');
    //EditDialog.OpenSelectionClose(dialog, guid);

    EditDialog.Reset(guid, views[guid].gViewName);

    //changed by br
    if (displayType == "Preview") {
        //        EditDialog.pk = pk;
        PreviewDisplay.initDialogSettings(guid, pk);
    }
    else {
        if (!allowEdit.disabled) {
            dialog.dialog("option", "title", translator.editRowOn + views[guid].gViewDisplayName);
            removeMessage(dialog);
        }
        else {
            dialog.dialog("option", "title", translator.viewRowOn + views[guid].gViewDisplayName);
            addMessage(dialog, allowEdit.message);
        }

        if (!dialog.prop('item')) {
            dialog.siblings(".ui-dialog-titlebar").show();
            Durados.Dialogs.zIndex(dialog, 100001)
        }

        var buttons = EditDialog.GetButtons(pk, guid, hideClose, !allowEdit.disabled);
        dialog.dialog("option", "buttons", buttons);

        var s = 83; // key code
        shortcutKey(dialog, buttons[translator.save], s, true);

        //element, key, callback, args
        //$.ctrl(dialog, 'S', buttons[translator.save], null);
        //$.ctrl(dialog, 'E', buttons[translator.saveAndClose], null);

        var e = 69; // key code
        shortcutKey(dialog, buttons[translator.saveAndClose], e, true);

        var isMaximize = isMaximizeDialog(guid);

        var state = isMaximize == true ? 'max' : $.cookie("state_" + views[guid].gViewName);

        if (itemEditDialog == null || itemEditDialog.attr('id') != dialog.attr('id')) {

            currentViewName = views[guid].gViewName;

            //jquery doesn't remember the position just the size
            if (state != "max") {
                var rec = Rectangle.Load(currentViewName);
                if (rec != null) {
                    dialog.dialog("option", "position", [rec.left, rec.top]);
                }
            }
            else {
                dialogExt.max(dialog, null, guid);
            }
        }
//        if (state != "max") {
//            dialog.dialog("option", "height", 'auto');
//        }
        //        dialog.dialog("option", "zIndex", 100001);
    }

    dialog.dialog("option", "stack", false);

    var jsonView = EditDialog.GetJsonViewValue(pk, guid, 'Edit');
    if (jsonView == null) {
        ajaxNotSuccessMsg("The record that you request to edit has been deleted.", guid);
        //alert("The record the you request to edit has been deleted.");
        //complete(guid);
        return;
    }
    if (isViewItem(guid)) {
        dialog.dialog("option", "title", EditDialog.getViewDisplayName(jsonView, guid, editPrefix));
    }
    //    dialog.dialog("option", "open", function(event, ui) {
    //        var state = $.cookie("state_" + views[guid].gViewName);
    //        if (state == 'max') {
    //            setTimeout(function() {
    //                dialogExt.max(dialog, null, guid);
    //            }, 1);
    //        }
    //        window.setTimeout(function() {
    //            jQuery(document).unbind('mousedown.dialog-overlay').unbind('mouseup.dialog-overlay');
    //        }, 1);
    //    });

    if ((IsItem() && (views[guid].ViewName == 'View' || views[guid].ViewName == 'Field'))) {
        dialog.dialog({
            hide: { effect: 'fold', duration: 500 }
        });
    }
    else {
        dialog.dialog({
            show: { effect: 'fold', duration: 500 },
            hide: { effect: 'fold', duration: 500 }
        });
    }
    dialog.dialog('open');

    //1 sec
		if(typeof(allowedit) != "undefined"){
		   if (!allowedit.disabled && views[guid].showupdown) {
			   dialog.parent().find('.ui-dialog-buttonpane button').each(function() {
				   if ($(this).text() == '->') {
					   $(this).attr('title', 'next');
					   $(this).attr('alt', 'next');
				   }
				   else if ($(this).text() == '<-') {
					   $(this).attr('title', 'previous');
					   $(this).attr('alt', 'previous');
				   }
			   });
		   }
		}

		if(typeof(editdialog) != "undefined"){
		   if (!editdialog.allowedit(guid, pk)) {
			   editdialog.disabledialog(editprefix, guid, jsonview);
		   }
		}

    if (views[guid].ViewName == "Field") {
        var displayFormatElms = dialog.find("select[name='DisplayFormat']");
        if (displayFormatOriginalOptions == null) {
            displayFormatOriginalOptions = displayFormatElms.html();
        }
        else {
            displayFormatElms.html(displayFormatOriginalOptions);
        }
    }

    EditDialog.Load(pk, editPrefix, guid, jsonView, 'DataRowEdit', allowEdit.disabled);

    AddDialog.HandleTab(pk, 'EditTabs', guid);
    var disabledTabs = $('#' + guid + 'EditTabs').tabs("option", "disabled");
    if (disabledTabs.length > 0 && disabledTabs[0] != 0)
        Durados.Tabs.select($('#' + guid + 'EditTabs'), 0);

    if (displayType == "Preview") {
        Durados.Tabs.select($('#' + guid + 'EditTabs'), 0);
    }
    //    $('#' + guid + 'EditTabs').tabs("select", 0);

    EditDialog.HandleCounter(jsonView, 'EditTabs', guid);
    //1.2 sec
    hideProgress();
    //complete(guid);
    initDropdownChecklistsEdit(guid);
    //1.5 sec

    //changed by br- to prevent a bug
    if (displayType != "Preview") {
        setFocusInDialog(dialog);
    }

    dialog.attr('changed', 'no');

    Durados.Dialogs.BindToChangeEvent('input', dialog);
    Durados.Dialogs.BindToChangeEvent('select', dialog);
    Durados.Dialogs.BindToChangeEvent('textarea', dialog);

    //changed by br- add keydown event to prevent a bug
    if (displayType == "Preview") {
        dialog.find('input').keydown(function () {
            dialog.attr('changed', 'yes');
        });
    }

    subgridsHashes[guid] = new Array();
    $(Durados.View).trigger('editBeforeDerivation', { pk: pk, guid: guid, dialog: dialog, viewName: views[guid].ViewName });
    initDerivationOnShow(views[guid].jsonViewForEdit.Derivation, dialog, editPrefix, guid, views[guid].jsonViewForEdit.ViewName);
    EditDialog.DisableCloneView(editPrefix, guid, jsonView);

    //    $(Durados.View).unbind("afterDerivationChanged");
    //    $(Durados.View).one("afterDerivationChanged", function(event, data) {
    //        EditDialog.DisableCloneView(editPrefix, guid, jsonView);
    //    });

    PreviewDisplayModeViewJson = jsonView;
    if (window.location.href.indexOf('Admin/Item/View?') > -1 || window.location.href.indexOf('Admin/IndexPage/Field?') > -1) {
        //    if (isViewItem(guid)) {
        InlineEditingDialog.SetAdminPreviewEvents(dialog, views[guid].ViewName, guid);
    }

    if (views[guid].ViewName == "Field") {
        var dataType = dialog.find('[name="DataType"]');
        dialog.find('[name=RelatedViewName]').parents('tr:first').parents('tr:first').hide();
        if (dataType.length == 1) {
            dataType.change(function () {
                setDataType(dialog, dataType)

            });
            if (dataType.val() == 'SingleSelect') {
                setDataType(dialog, dataType)

            }
        }
    }

    $(Durados.View).trigger('edit', { pk: pk, guid: guid, dialog: dialog, viewName: views[guid].ViewName, prefix: editPrefix });

    try {
        if (views[guid].ViewName == 'Field' && plugIn()) {
            dialog.parent().find('button.ui-button').each(function () {
                if ($(this).text() == translator.save) {
                    $(this).css('display', 'none');

                }
                if ($(this).text() == translator.cancel) {
                    $(this).find('span').text('Back');

                }
            });
        }
    }
    catch (err) {
    }

    Multi.SelectByPK(guid, pk);

    if (callback) {
        callback();
    }

    //    setFocusInDialog(dialog);
    //1.8 sec
}

function plugIn() {
    try {
        return ((parent && parent.refreshWidget2) || (parent && parent.parent && parent.parent.refreshWidget2));
    }
    catch (err) {
        return false;
    }
}

function plugInRefresh() {
    try {
        if (parent && parent.refreshWidget2) {
            parent.refreshWidget2();
        }
        else if (parent && parent.parent && parent.parent.refreshWidget2) {
            parent.parent.refreshWidget2();
        }
    }
    catch (err) { }
}

function isViewItem(guid) {
    return window.parent && window.location.href.indexOf('Admin/Item/View?') > -1 && views[guid] && views[guid].ViewName == 'View';
}

function resizeWindow() {
    clearTimeout(toResize); clearTimeout(dialogResize);
    if (adjustDataTableHeightDisabled) return;
    toResize = setTimeout(
    function () {
        adjustDataTableHeight();
        $('.fixedViewPort table.gridview').each(function () {
            var guid = $(this).closest('.ajaxDiv').attr('guid');
            Durados.GridHandler.setColumnsWidth(guid);
        });
    }, 600);
    if (!resizing) dialogResize = setTimeout(dialogExt.resizeCurrent, 600);
}


//function isDock(guid) {
//    if (window.parent && window.parent.Refresh && window.location.href.indexOf('Admin/Item/View?') > -1) {
//        return true;
//    }
//    else if (window.parent && window.parent.parent && window.parent.parent.Refresh && window.location.href.indexOf('Admin/IndexPage/Field?') > -1) {
//        return true;
//    }

//    return false;

//}

function isDock(guid) {
    try {
        if (window.parent && window.parent.Refresh && window.location.href.indexOf('Admin/Item/View?') > -1) {
            return true;
        }
        else if (window.parent && window.parent.parent && window.parent.parent.Refresh && window.location.href.indexOf('Admin/IndexPage/Field?') > -1) {
            return true;
        }
    }
    catch (err) { }

    return false;

}

function isDockFields(guid) {
    try {
        if ((window.parent && window.parent.parent && window.parent.parent.Refresh && window.location.href.indexOf('Admin/IndexPage/Field?') > -1) || (guid && views[guid].ViewName == 'Field' && views[guid].DataDisplayType == "Preview")) {
            return true;
        }
    }
    catch (err) { }
    return false;

}

function setFocusInDialog(dialog) {
    try {
        dialog.find('iframe').contents().find('body').focus();
    }
    catch (err) { }

    var input = dialog.find('input:visible:enabled:first');
    var select = dialog.find('select:visible:enabled:first');
    if (select.length > 0 && input.length > 0) {
        if (select.offset().top - 1 < input.offset().top) {
            setTimeout(function () {
                select.focus();
            }, 1);
        }
        else if (select.offset().top - 1 == input.offset().top) {
            if (select.offset().left < input.offset().left) {
                setTimeout(function () {
                    select.focus();
                }, 1);
            }
            else {
                setTimeout(function () {
                    input.focus();
                }, 1);
            }
        }
        else {
            setTimeout(function () {
                input.focus();
            }, 1);
        }
    }
    else {
        if (input.length > 0) {
            setTimeout(function () {
                input.focus();
            }, 10);
        }
        else if (select.length > 0) {
            setTimeout(function () {
                select.focus();
            }, 1);
        }
        else {
            $('input:visible:enabled:first').focus();
        }
    }


    //    var firstInput = dialog.find('input:visible:enabled:first');
    //    if (firstInput.length == 1) {
    //        setTimeout(function() {
    //            firstInput.focus();
    //        }, 10);
    //    }
    //    else {
    //        $('input:visible:enabled:first').focus();
    //    } 
}

EditDialog.HandleCounter = function (jsonView, tasPostfix, guid) {
    var tab = $('#' + guid + tasPostfix);

    for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
        var field = jsonView.Fields[index].Value;
        var counterPrefix = "FkCounter_";

        if (field.Name.startsWith(counterPrefix)) {
            var fieldName = field.Name.split(counterPrefix)[1];
            var val = field.Value;
            if (val == null || val == '')
                val = 0;

            var span = tab.find('li[childrenFieldName="' + fieldName + '"]').find('span[hasCounter="hasCounter"]');
            //            var text = span.text();
            //            text = text.replace("xxxx", val);
            span.text(val);
        }
    }
}

function toggleDeleteValues(img, dependencies) {

    var element = $(img).parents('td.rowViewCell').first().find('input,select,textarea').first();

    if (dependencies && element.attr('hasinsidetrigger') == 'hasInsideTrigger') {
        var triggername = element.attr('triggername');
        EditDialog.multiDialog.find('select[name="' + triggername + '"]').parents('td.rowViewCell').first().find('img.inlineDelImg').first().click();
        return;
    }

    if (img.src.indexOf('plus.gif') > 0) {
        img.src = '/Content/Images/minus.gif';
        img.alt = 'Exclude';
        img.title = 'Exclude field from multi edit';

        element.attr('ignore', 'no').removeAttr('disabled').filter('select.dropdownchecklist').dropdownchecklist("enable");

    } else {
        img.src = '/Content/Images/plus.gif';
        //$(img).prev().attr('ignore', '');
        img.alt = 'Include';
        img.title = 'Include field for multi edit';

        element.attr('ignore', '').attr('disabled', 'disabled').filter('select.dropdownchecklist').dropdownchecklist("disable");

        var k = editPrefix + "," + views[EditDialog.guid].gViewName + "," + element.attr('name');
        if (k in views[EditDialog.guid].validations && views[EditDialog.guid].validations[k]) {
            views[EditDialog.guid].validations[k].reset();
            //views[EditDialog.guid].validations[k].destroy();
        }

    }

    if (dependencies) {
        EditDialog.toggleDependencies(element);
    }

}


EditDialog.toggleDependencies = function (element) {

    if (element.attr('insidedependency') == 'insideDependency') {
        var dependentFieldNames = element.attr('dependentFieldNames').split(';');
        element.attr('dpk', '');
        for (var i = 0; i < dependentFieldNames.length; i++) {

            EditDialog.multiDialog.find('select[name="' + dependentFieldNames[i] + '"]').each(function () {
                var img = $(this).parents('td.rowViewCell').first().find('img.inlineDelImg').first();
                if (img.length > 0) {
                    toggleDeleteValues(img[0], false);
                }
            });

        }

    }
}


EditDialog.OpenSelectionInitDone = false;

EditDialog.OpenSelectionInit = function (dialog, guid) {

    EditDialog.multiDialog = dialog;
    EditDialog.guid = guid;

    var JsonView = GetJsonViewForEdit(guid);

    $('<img class="inlineDelImg" title="Include field for multi edit" alt="Include" src="/Content/Images/plus.gif" onclick="toggleDeleteValues(this, true)" />').appendTo(dialog.find('input,select,textarea').filter(function () {
        if ($(this).attr('type') == 'hidden' || $(this).attr('filter') == 'filter') { return false; }
        var k = $(this).attr('name');
        for (var index = 0, len = JsonView.Fields.length; index < len; ++index) {
            if (JsonView.Fields[index].Key == k) {
                if (JsonView.Fields[index].Value.Disabled) {
                    return false;
                }
                else {
                    $(this).attr('disabled', 'disabled');

                    if ($(this).attr('hasInsideTrigger') == 'hasInsideTrigger') {
                        var t = $(this).attr('triggerName');
                        for (var ii = 0; ii < len; ++ii) {
                            if (t == JsonView.Fields[ii].Key) {
                                if (JsonView.Fields[ii].Value.Disabled) {
                                    return false;
                                } else {
                                    break;
                                }
                            }
                        }

                    }
                    return true;
                }
            }
        }
        return false;

    }).parents('td.rowViewCell'));

}


EditDialog.OpenSelectionReset = function (dialog, guid) {
    dialog.find('img.inlineDelImg').each(function () {
        //if (this.src.indexOf('plus.gif') < 0) {
        this.src = '/Content/Images/plus.gif';
        //$(this).prev().attr('ignore', '');
        this.alt = 'Include';
        this.title = 'Include field for multi edit';
        $(this).parents('td.rowViewCell').first().find('input,select,textarea').first().attr('ignore', '').attr('disabled', 'disabled').filter('select.dropdownchecklist').dropdownchecklist("disable");
        //}
    });
}

EditDialog.OpenSelectionClose = function (dialog, guid) {
    dialog.find('img.inlineDelImg').each(function () {
        if (this.src.indexOf('plus.gif') > 0) {
            $(this).parents('td.rowViewCell').first().find('input,select,textarea').first().removeAttr('disabled').filter('select.dropdownchecklist').dropdownchecklist("enable");
        }
    });
}

EditDialog.getDialog = function (guid) {
    if (views[guid].DataDisplayType == "Preview") {
        return $('#' + guid + 'DataRowEdit_clone'); 
    }
    else {
        return $('#' + guid + 'DataRowEdit'); 
    }
}


EditDialog.OpenSelection = function (guid, hideClose) {
    begin();

    var dialog = EditDialog.getDialog(guid);
    dialog.dialog("option", "title", translator.editRowsOn + views[guid].gViewDisplayName);

    var buttons = EditDialog.GetButtonsForSelection(guid, dialog);

    dialog.dialog("option", "buttons", buttons);

    var s = 83; // key code
    shortcutKey(dialog, buttons[translator.save], s, true);

    var e = 69; // key code
    shortcutKey(dialog, buttons[translator.saveAndClose], e, true);

    EditDialog.Reset(guid, views[guid].gViewName);

    if (!EditDialog.OpenSelectionInitDone) {
        EditDialog.OpenSelectionInit(dialog, guid);
    } else {
        EditDialog.OpenSelectionReset(dialog, guid);
    }

    EditDialog.OpenSelectionInitDone = true;

    currentViewName = views[guid].gViewName;
    //jquery doesn't remember the position just the size
    var rec = Rectangle.Load(currentViewName);
    if (rec != null) {
        dialog.dialog("option", "position", [rec.left, rec.top]);
    }
    dialog.dialog("option", "dialogClass", 'multiupdates');

    dialog.bind("dialogclose", EditDialog.OpenSelectionClosed);

    dialog.dialog('open');

    complete(guid);

    initDropdownChecklistsEdit(guid, dialog);

    $(Durados.View).trigger('multiEdit', { pks: Multi.GetSelection(guid), guid: guid, dialog: dialog, viewName: views[guid].ViewName, prefix: editPrefix });

}

EditDialog.OpenSelectionClosed = function (event, ui) {
    var dialog = $(this); //$(event.target).parent();//
    var guid = dialog.find('div[ajaxdiv]').first().attr('guid');
    EditDialog.OpenSelectionClose(dialog, guid);
    dialog.unbind("dialogclose", EditDialog.OpenSelectionClosed);
}
function ReplaceNonAlphaNumeric(s) {
    return s.replace(/ /g, '_').replace(/-/g, '_').replace(/\+/g, '_').replace(/\//g, '_').replace(/\*/g, '_').replace(/\./g, '_').replace(/\=/g, '_').replace(/\!/g, '_').replace(/\@/g, '_').replace(/\#/g, '_').replace(/\$/g, '_').replace(/\%/g, '_').replace(/\^/g, '_').replace(/\&/g, '_');

}

EditDialog.UpdateSelection = function (guid, close, dialog) {

    //var dialog = $('#' + guid + 'DataRowEdit');

    var jsonViewClone = jQuery.extend(true, {}, GetJsonViewForEdit(guid));

    //validate enabled fields
    //var form = $('#' + guid + 'Edit' + ReplaceNonAlphaNumeric(jsonViewClone.ViewName) + 'DataRowForm')[0];
    var form = dialog.find('form[d_prefix]');
    var isValid = true;
    $(jsonViewClone.Fields).each(function () {
        var field = this.Value;
        var fieldName = ReplaceNonAlphaNumeric(field.Name);
        var element = $('#' + guid + editPrefix + fieldName);
        if (views[guid].DataDisplayType == "Preview") {
            element = $('#' + guid + editPrefix + fieldName + '_clone');
        }
        var k = editPrefix + "," + jsonViewClone.ViewName + "," + fieldName;
        if (element.attr('ignore') == 'no') {
            var spanId = 'the' + guid + editPrefix + jsonViewClone.ViewName + "_" + field.Name;
            if (views[guid].DataDisplayType == "Preview") {
                spanId = 'the' + guid + editPrefix + jsonViewClone.ViewName + "_" + field.Name + '_clone';
            }
            if (views[guid].validations && k in views[guid].validations && views[guid].validations[k]) {

            } else {
                views[guid].validations[k] = getFieldValidation(field, spanId, element);
            }

            if (views[guid].validations[k]) {
                if (!views[guid].validations[k].validate()) {
                    isValid = false;
                }
            }
        }
    });

    if (!isValid) {
        modalSuccessMsg(getErrorsList(dialog));
        return;
    }

    begin();

    var pks = Multi.GetSelection(guid);

    var clonePrefix = null;
    if (views[guid].DataDisplayType == "Preview") {
        clonePrefix = '_clone';
    }

    $.post(views[guid].gEditSelectionUrl,
        {
            pks: Sys.Serialization.JavaScriptSerializer.serialize(pks),
            jsonView: Sys.Serialization.JavaScriptSerializer.serialize(FillJson(jsonViewClone, editPrefix, guid, dialog, true, null, clonePrefix)), guid: guid
        },
        function (html) {
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                EditDialog.HandleSuccess(html, guid);

                dialog.attr('changed', 'no');
                Multi.SelectByPKs(guid, pks);

                if (close) {
                    EditDialog.Close(guid, null, true, true);
                }

            }
            else {
                //EditDialog.ShowFailureMessage(html.replace("$$error$$", ""), guid);
                ajaxNotSuccessMsg(html, guid);
            }
        });
}

EditDialog.Reset = function (guid, viewName) {
    if (views[guid].DataDisplayType == "Preview") {
        var dialog = EditDialog.getDialog(guid);
        dialog.find('form')[0].reset();
        resetUpload();
        clearWysiwyg(dialog);
    }
    else {
        $('#' + guid + 'Edit' + ReplaceNonAlphaNumeric(viewName) + 'DataRowForm')[0].reset();
        resetUpload();
        var dialog = $('#' + guid + 'DataRowEdit');
        clearWysiwyg(dialog);
    }
}

EditDialog.GetFields = function (jsonView, guid, prefix) {
    var arr = [];
    var hash = [];

    for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
        var field = jsonView.Fields[index].Value;
        hash[field.Name] = { field: field, loaded: false };
    }

    for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
        var field = jsonView.Fields[index].Value;

        EditDialog.PushField(arr, hash, field, guid, prefix);
    }

    if (arr.length != jsonView.Fields.length) {

        alert("Error in loading dependency. Please contact the administrator.");
    }

    return arr;
}

EditDialog.PushField = function (arr, hash, field, guid, prefix) {
    if (!hash[field.Name].loaded) {
        var id = guid + prefix + ReplaceNonAlphaNumeric(field.Name);
        var htmlField = $('#' + id);

        if (htmlField.attr('hasInsideTrigger') == 'hasInsideTrigger') {
            var triggerName = htmlField.attr('triggerName');
            if (hash[triggerName] == null) {
                ajaxNotSuccessMsg("Please verify that the Trigger Field '" + triggerName + "' is not Hidden and exists. This trigger belong to '" + field.Name, guid);
                return;
            }

            if (hash[triggerName].loaded) {
                EditDialog.PushField2(arr, hash, field);
            }
            else {
                EditDialog.PushField(arr, hash, hash[triggerName].field, guid, prefix);
                EditDialog.PushField2(arr, hash, field);
            }
        }
        else {
            EditDialog.PushField2(arr, hash, field);
        }
    }
}

EditDialog.PushField2 = function (arr, hash, field) {
    arr.push(field);
    hash[field.Name].loaded = true;

}

EditDialog.getViewDisplayName = function (jsonView, guid, prefix) {
    for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
        //var field = fields[index].Value;
        var field = jsonView.Fields[index].Value;

        if (field.Name == 'DisplayName') {
            return field.Value + ' ' + translator.Settings;
        }
    }

    return translator.Settings;

}

EditDialog.Load = function (pk, prefix, guid, jsonView, dialogPostfix, disabled) {

    if (disabled == null)
        disabled = true;

    //delete all class=wysiwyg to clear the textarea
    //$(".wysiwyg").remove();
    $('#' + guid + prefix.slice(0, -1).capitalize() + ReplaceNonAlphaNumeric(jsonView.ViewName) + 'DataRowForm textarea[rich="true"]').each(function () { $(this).htmlarea("dispose") });

    var fields = EditDialog.GetFields(jsonView, guid, prefix);

    //for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
    for (var index = 0, len = fields.length; index < len; ++index) {
        //var field = fields[index].Value;
        var field = fields[index];

        EditDialog.LoadField(pk, prefix, guid, field);
    }

    if (dialogPostfix == null)
        dialogPostfix = 'DataRowEdit';

    var dialog = $('#' + guid + dialogPostfix);


    Durados.Dependencies.cleanUp();

    for (var index = 0, len = fields.length; index < len; ++index) {

        var field = fields[index];

        if (field.DependencyData) {
            Durados.Dependencies.Apply(field.Name, field.Value, guid, dialog, jsonView.Fields);

            Durados.Dependencies.createHandlers($('#' + guid + prefix + ReplaceNonAlphaNumeric(field.Name)), guid, jsonView.Fields, jsonView.ViewName, prefix);
        }
    }


    dialog.find('.childrenViewer').attr('pk', pk);
    dialog.attr('pk', pk);

    //dialog.find('#' + guid + 'EditTabs').attr('pk', pk);

    dialog.find('div.childrenViewer').each(function () {
        $(this).html('');
    });

    dialog.find('span.inlineAddingImg').each(function () {
        expand(this, true, null, disabled, guid);
    });

    if (jsonView.ViewName == "Field") {
        var dataType = dialog.find('[name="DataType"]');
        var table = dataType.parents('table:first').parents('table:first');
        table.find('tr.data-type-settings-row').remove()
    }

    $(EditDialog).trigger('onafterEditLoad', { guid: guid, dialog: dialog, json: jsonView, pk: pk, viewName: jsonView.ViewName });

}

EditDialog.DisableCloneView = function (prefix, guid, jsonView, isAdmin) {
    if (isAdmin || views[guid].ViewName == "View" || views[guid].ViewName == "Field") {
        for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
            var field = jsonView.Fields[index].Value;
            var fieldName = ReplaceNonAlphaNumeric(field.Name);
            var id = guid + prefix + fieldName;
            var htmlField = $('#' + id);

            htmlField.attr("disabled", field.Disabled);
        }
    }
}

EditDialog.DisableDialog = function (prefix, guid, jsonView) {
    for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
        var field = jsonView.Fields[index].Value;
        var fieldName = ReplaceNonAlphaNumeric(field.Name);
        var disabledField = $('#' + guid + prefix + fieldName);
        disableField(disabledField, guid, prefix, views[guid].ViewName);
    }
}

EditDialog.LoadField = function (pk, prefix, guid, field) {
    var fieldName = ReplaceNonAlphaNumeric(field.Name);
    var id = guid + prefix + fieldName;
    var htmlField = $('#' + id);
    //var htmlField = $("[id='" + id + "']");

    if (htmlField.length == 1) {

        //if (field.DependencyData)
        //    Durados.Dependencies.createHandlers(htmlField, guid);
        if (htmlField.attr('htmlType') == 'htmlType') {
            htmlField.val(field.Value);
            var div = htmlField = $('#' + id + 'htmlType');
            div.html(field.Value);
        }
        else if ($(htmlField[0]).attr('upload') == 'upload' && !htmlField.is("textarea")) {
            //if (field.Value != '') {
            //setTimeout('UploadFile("' + field.Name + '","' + prefix + '","' + guid + '")', 1);

            if (field.Disabled == true) {
                htmlField.val(field.Value);
            }
            else {
                UploadFile(field.Name, prefix, guid);
                var src = $('#' + guid + prefix + 'upload_img_' + ReplaceNonAlphaNumeric(field.Name)).attr('UploadPath');
                if (!src)
                    src = '';
                showUpload(field.Name, prefix, field.Value, src + field.Value, guid);
            }
            //}
        }
        else if (field.Type == "CheckList") {
            //htmlField.attr('dpk', pk);
            loadChecklist(htmlField, field.Value);
            //            var vals = field.Value.split(',');
            //            var options = htmlField.find('option');
            //            $(vals).each(function() {
            //                var val = this + '';
            //                options.each(function() {
            //                    if ($(this).attr('value') == val) {
            //                        $(this).attr('selected', 'selected');
            //                    }
            //                });
            //            });
        }
        else if ($(htmlField[0]).attr('radioButtons') == 'radioButtons') {
            // $('[name=' + guid + prefix + field.Name + '][value=' + field.Value + '' + ']').attr('checked', 'checked');
            Durados.CheckBox.SetChecked($('[name=' + guid + prefix + field.Name + '][value=' + field.Value + '' + ']'), 'checked');
        }
        else if (htmlField.attr('outsideDependency') == 'outsideDependency') {
            htmlField.val(field.Value);
            var dependentFieldName = htmlField.attr('dependentFieldName');
            var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
            var select = $('#' + guid + prefix + ReplaceNonAlphaNumeric(dependentFieldName));
            Dependency.Load(dependentFieldViewName, dependentFieldName, field.Value, select, guid);
        }

        else if (htmlField.attr('insideDependency') == 'insideDependency') {
            if (htmlField.find('option[value="' + field.Value + '"]').length == 0) {
                htmlField.append('<option value="' + field.Value + '">' + field.Default + '</option>');
            }
            htmlField.val(field.Value);
            htmlField.attr('dpk', pk);
            var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
            var dependentFieldNames = htmlField.attr('dependentFieldNames').split(';');
            Dependency.LoadInside(dependentFieldViewName, dependentFieldNames, prefix, htmlField[0], guid);
        }
        else if (htmlField.attr('color') == "1") {
            var value = field.Value;
            var hasValue = value != null && value != '';
            var disabledAttr = hasValue ? '' : 'disabled';
            var checkbox = htmlField.siblings('input[type=checkbox]');
            var spectrumAction = hasValue ? 'enable' : 'disable';

            //Init override checkBox
            Durados.CheckBox.SetChecked(checkbox, hasValue);

            //Init colorPicker value && disabled
            if (!hasValue) {
                value = '#ffffff';
            }
            var elm = $('<input type="text">');
            initColorPicker(elm);
            elm.spectrum("set", value);
            if (elm.spectrum("get").alpha == 1) {
                value = elm.spectrum("get").toHexString();
            }
            htmlField.spectrum("set", value);
            htmlField.spectrum(spectrumAction);
        }
        else if (htmlField[0].type == "textarea") {

            htmlField.val(field.Value);

            //alert(htmlField.text());
            if (htmlField.attr('rich') == 'true')
                $('#' + guid + prefix + fieldName).htmlarea(); //create the wysiwyg
        }
        else if (htmlField[0].nodeName == "DIV") {
            htmlField.html(field.Value);
        }
        else if (htmlField[0].type == "checkbox")
        //htmlField.attr('checked', field.Value);
            Durados.CheckBox.SetChecked(htmlField, field.Value);
        else if (htmlField.attr('d_type') == "Url") {
            htmlField.attr('value', field.Value);
            var values = field.Value.split('|');
            var displayText = '';
            var href = '';
            var target = '_blank';

            if (values.length == 3) {
                displayText = values[0];
                href = values[2];
                target = values[1];
            }
            else if (values.length == 1) {
                href = values[0];
            }
            if (displayText == '' && href != '#') {
                displayText = href;
            }

            var format = htmlField.attr('format');
            if (format == 'ButtonLink') {
                htmlField.find('button span').text(displayText);
            }
            else {
                htmlField.find('span').text(displayText);
            }

            htmlField.attr('href', href);
            htmlField.attr('target', target);
        }
        else if (field.Type == 'Autocomplete') {
            htmlField.attr('valueId', field.Value);
            if (field.Default != null)
                htmlField.val(field.Default);
        }
        else {
            // if duplicate and disable and has default value
            if (prefix == createPrefix && htmlField.attr('disabled') && htmlField.val() != '') {
            }
            else {
                if (htmlField[0].nodeName == "SELECT") {
                    if (htmlField.find('option[value="' + field.Value + '"]').length == 0 && field.Default != null) {
                        htmlField.append('<option value="' + field.Value + '">' + field.Default + '</option>');

                    }
                }
                htmlField.val(field.Value);
                if (htmlField.prev().find("UL").length == 1) {
                    slider.SetLoadSelectedImage(htmlField)
                }
            }

        }
        if (htmlField.hasClass('date')) { triggerDateChanged(htmlField); };
    }
    //    else
    //        alert(id);

    //    if (field.Type == "CheckList") {
    //        
    //    }
}



function clearAndLoadChecklist(htmlField, value) {
    htmlField.find("option:selected").removeAttr("selected");
    loadChecklist(htmlField, value);
}

function loadChecklist(htmlField, value) {
    var options = htmlField.find('option');
    options.each(function () {
        $(this).removeAttr('selected');
    });

    if (value == null || value == '')
        return;
    var vals = value.split(',');
    $(vals).each(function () {
        var val = this + '';
        options.each(function () {
            if ($(this).attr('value') == val) {
                $(this).attr('selected', 'selected');
            }
        });
    });
}

EditDialog.GetJsonViewValue = function (pk, guid, dataAction) {
    var syncJsonView = null;
    $.ajax({
        url: views[guid].gGetJsonViewUrl,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: views[guid].gViewName, dataAction: dataAction, pk: pk, guid: guid },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (jsonView) {
            if (jsonView == null || jsonView == '') {
                ajaxNotSuccessMsg('');
            }
            else {
                syncJsonView = Sys.Serialization.JavaScriptSerializer.deserialize(jsonView);
            }

        }

    });

    return syncJsonView;
}

EditDialog.Close = function (guid, pk, afterUpdate, close) {
    var needClose = true;
    var dialog = EditDialog.getDialog(guid); //$('#' + guid + 'DataRowEdit');

    var changed = dialog.attr('changed') == 'yes';
    if (changed) {
        if (confirm("Exit without save?")) {
            dialog.attr('changed', 'no');
            if ((views[guid].ViewName == "View" || views[guid].ViewName == "Field") && isAdmin(guid)) {
                CancelPreview(views[guid].ViewName);
                if (window.parent && window.parent.reloadPage && window.location.href.indexOf('Admin/Item/View?') > -1) {
                    window.parent.reloadPage('&settings=false');
                }
            }
            if (views[guid].RefreshOnClose == 'True' && !afterUpdate) {
                AddDialog.Refresh(guid, pk);
                //$("div[itemDataRowView='itemDataRowViewEdit']").dialog("option", "zIndex", 900);
            }
            else {
                Multi.SelectByPK(guid, pk);

            }

        }
        else {
            needClose = false;
        }
    }
    else {
        dialog.attr('changed', 'no');
        if (views[guid].RefreshOnClose == 'True' && !afterUpdate) {
            AddDialog.Refresh(guid, pk);
        }
        else {
            Multi.SelectByPK(guid, pk);
        }
        //        $("div[itemDataRowView='itemDataRowViewEdit']").dialog("option", "zIndex", 900);
    }

    if (needClose) {
        if (views[guid].ReloadPage == "Edit" || views[guid].ReloadPage == "Always") {
            reloadPage();
        }
        if (close) {
            dialog.dialog('close');
        }
    }

    return needClose;
}

EditDialog.Update = function (pk, guid, close, commitConfig, complete, doNotSelectRow) {
    //    YAHOO.tool.Profiler.start("Save");
    //    YAHOO.tool.Profiler.start("clientBefore");

    var dialog = $('#' + guid + 'DataRowEdit');
    var json = GetJsonViewForEdit(guid);

    if (EditDialog.isValid(guid, json.ViewName) == false) {

        modalSuccessMsg(getErrorsList(dialog));
        //alert(getErrorsList(dialog));
        return;
    }

    saveElementScrollsPosition(guid);

    begin();

    var url = views[guid].gEditUrl;
    if (complete) {
        url = rootPath + views[guid].Controller + '/EditStep/' + views[guid].gViewName;

    }

    if (views[guid].ViewName == "Field") {
        var dataType = dialog.find('[name="DataType"]');
        //dialog.find('[name=RelatedViewName]').parents('tr:first').parents('tr:first').hide();
        if (dataType.length == 1) {
            if (!isValidDataType(dialog, dataType)) {
                blockOperation = false;
                hideProgress();
                return;
            }
            if (isCreatingNewTableInYourDb(dialog) && !confirm("The following change is going to create new table, continue?")) {
                blockOperation = false;
                hideProgress();
                return;
            }

        }
    }
    if (views[guid].ViewName == "durados_Link")
        if (!Bookmarks.validateGrobootViews(pk, dialog))
            return;

    var json = FillJson(json, editPrefix, guid, dialog);

    if (views[guid].ViewName == "Field" && dialog.find('.InSystemDatabase').prop('checked')) {
        var field = jQuery.extend(true, {}, json.Fields[0]);
        field.Key = "sysdb";
        field.Value.Value = true;
        field.Value.Name = "sysdb";
        json.Fields.push(field);
    }




    var jsonView = Sys.Serialization.JavaScriptSerializer.serialize(json);
    //    YAHOO.tool.Profiler.stop("clientBefore");
    //    YAHOO.tool.Profiler.start("server");

    //    PreviewDisplay.loadElementStateAfterRefresh = true;

    $.post(url,
        {
            pk: pk,
            jsonView: jsonView, guid: guid
        },
        function (html) {
            //            YAHOO.tool.Profiler.stop("server");
            //            YAHOO.tool.Profiler.start("clientAfter");
            if (views[guid].ViewName == "View" || views[guid].ViewName == "Field") {
                adminPreviewChanged = false;
            }

            if (window.parent && window.parent.reloadPage && window.location.href.indexOf('Admin/Item/View?') > -1) {
                //window.parent.Refresh(window.parent.getMainPageGuid());
                var isViewOwner = views[guid] == null ? false : views[guid].Role == "View Owner";
                if (isViewOwner)
                    window.parent.reloadPage('&settings=true');
            }
            else if (window.parent && window.parent.parent && window.location.href.indexOf('Admin/IndexPage/Field?') > -1) {
                dialog.attr('changed', 'no');
                //                YAHOO.tool.Profiler.stop("prgress");
                //                YAHOO.tool.Profiler.start("selectRow");

                var indexError = html.indexOf("$$error$$", 0);
                var indexMessage = html.indexOf("$$message$$", 0);
                var hasError = indexError > 0 && indexError < 1000;
                var hasMessage = indexMessage > 0 && indexMessage < 1000;
                var displayName = dialog.find('[name="DisplayName"]').val();
                if (hasError) {
                    //EditDialog.ShowFailureMessage(html.replace("$$error$$", ""), guid);
                    ajaxNotSuccessMsg(html, guid);
                    EditDialog.selectRow(pk, guid, displayName);
                    return;
                }

                if (!plugIn()) {
                    try {
                        if (window.parent.parent.Refresh) {
                            window.parent.parent.Refresh(window.parent.parent.getMainPageGuid(), true);
                        }
                    }
                    catch (err) {
                    }
                }
                //var row = $('.boardselected');
                //row.text(displayName);
                EditDialog.HandleSuccess(html, guid);
                if (!doNotSelectRow) {
                    EditDialog.selectRow(pk, guid, displayName);
                }
                //                YAHOO.tool.Profiler.stop("selectRow");

                hideProgress();

                return;
            }
            if (views[guid].ViewName == "View" && parent && parent.refreshWidget2) {
                hideProgress();
            }

            if (commitConfig) {
                var url = rootPath + 'Admin/RefreshConfig/';
                refreshConfig(url, guid);
            }

            if (complete && html.IsResult) {
                WorkFlow.CompleteStep(pk, guid, views[guid].WorkFlowStepsFieldName, html, dialog);
            }
            else {
                var indexError = html.indexOf("$$error$$", 0);
                var indexMessage = html.indexOf("$$message$$", 0);
                var hasError = indexError > 0 && indexError < 1000;
                var hasMessage = indexMessage > 0 && indexMessage < 1000;
                if (hasError) {
                    //EditDialog.ShowFailureMessage(html.replace("$$error$$", ""), guid);
                    ajaxNotSuccessMsg(html, guid);
                }
                else if (hasMessage) {
                    //EditDialog.ShowFailureMessage(html.replace("$$message$$", ""), guid);
                    ajaxNotSuccessMsg(html, guid);
                    showProgress();
                    refreshView(guid);
                    dialog.attr('changed', 'no');
                    EditDialog.Close(guid, pk, true, true);
                }
                else {
                    EditDialog.HandleSuccess(html, guid);
                    if (views[guid].updateParent) {
                        updateParent(guid);
                    }
                    if (views[guid].updateParentGrid) {
                        updateParentGrid(guid);
                    }
                    dialog.attr('changed', 'no');
                    if (close) {
                        EditDialog.Close(guid, pk, true, true);
                    }

                }
            }
            $(EditDialog).trigger('onafterEditUpdate', { guid: guid, dialog: dialog, json: json, pk: pk, viewName: json.ViewName });

            //            YAHOO.tool.Profiler.stop("clientAfter");
            //            YAHOO.tool.Profiler.stop("Save");
        });

    try {
        if (views[guid].ViewName == 'Field' && plugIn()) {
            //            YAHOO.tool.Profiler.start("prgress");
            showProgress();
            setTimeout(function () {
                plugInRefresh();
            }, 200);
        }
    }
    catch (err) { }
    try {
        if (views[guid].ViewName == "View" && parent && parent.refreshWidget2) {
            showProgress();
            setTimeout(function () {
                parent.refreshWidget2();
            }, 1);
        }
    }
    catch (err) { }
}

EditDialog.selectRow = function (pk, guid, displayName) {
    var row = $("#d_row_" + guid + pk);
    if (row.length != 1) {
        $('#' + guid + 'ajaxDiv').find('div.boardrow2').each(function () {
            var row2 = $(this);
            if (row2.text().trim() == displayName) {
                pk = row2.attr('pk');
                row = row2;
            }
        });
    }
    if (row.length == 1) {
        row.click();
        setTimeout(function () {
            EditDialog.Open2(pk, guid, false);
        }, 1);
    }
}

$(EditDialog).bind("onafterEditUpdate", function (event, data) {
    Multi.SelectByPK(data.guid, data.pk);
    if (data.viewName == "durados_Link")
        Bookmarks.validateAndSaveMessages(event, data);
    if (data.viewName == "View") {
        if (customThemeChanged) {
            $.ajax({
                url: gVD + 'Admin/GetThemPath/',
                contentType: 'application/json; charset=utf-8',
                async: false,
                dataType: 'json',
                cache: false,
                error: ajaxErrorsHandler,
                success: function (json) {
                    var previewButtons = window.parent.parent.getPreviewButton();

                    $(previewButtons[0]).attr('href', $(previewButtons[0]).attr('href') + json);
                    $(previewButtons[1]).attr('href', $(previewButtons[1]).attr('href') + json);
                    previewButton.addClass('preview-notice');
                }

            });
        }
    }
    
});

function getPreviewButton() {
    return $('#mymenu').find('a[target="angularbknd"]');
}


EditDialog.HandleSuccess = function (html, guid) {
    //EditDialog.Close(guid);
    var pks = Multi.GetSelection(guid);
    saveElementScrollsPosition(guid);
    $('#' + guid + 'ajaxDiv').html(html);
    success(guid);
    resetElementScrollsPosition(guid);
    if (pks != null) {
        Multi.SelectByPKs(guid, pks);
    }
    views[guid].updateCounter();

}

//EditDialog.ShowFailureMessage = function(message, guid) {
//    alert(message);
//    complete(guid);
//}

EditDialog.isValid = function (guid, viewName) {

    var form = $('#' + guid + 'Edit' + viewName + 'DataRowForm')[0];
    var isSpryValid = Spry.Widget.Form.validate(form);
    var isValidUpload = IsValidUpload(form);
    if (isSpryValid == false || isValidUpload == false) {
        return false;
    }
    return true;
}

function IsValidUpload(form) {
    var valid = true;
    $(form).find("div.uploadDiv").each(function () {
        var uploadDiv = $(this);
        //var id = uploadDiv.attr("id");
        var container = uploadDiv.parents("td:first").parents("td:first");
        var invalidSpan = container.find("span.textfieldRequiredMsg");
        if (invalidSpan.length > 0) {
            var input = uploadDiv.find("input");
            if (input.val() == '') {
                valid = false;
                container.addClass("textfieldRequiredState");

            }
            else {
                container.removeClass("textfieldRequiredState");
            }
        }
    });

    return valid;
}

EditDialog.Prev = function (pk, guid) {
    var prevPK = Multi.GetPrevPK(pk, guid);
    if (prevPK != '') {
        EditDialog.Open(prevPK, guid, false);
    }
}

EditDialog.Next = function (pk, guid) {
    var nextPK = Multi.GetNextPK(pk, guid);
    if (nextPK != '') {
        EditDialog.Open(nextPK, guid, false);
    }
}

EditDialog.GetRefreshFields = function (json, panel) {
    var fields = [];
    $(json.Fields).each(function () {
        var field = this.Value;
        if (field.Refresh && panel.find("[name=" + field.Name + "]").length == 1) {
            fields.push(field);
        }
    });
    return fields;
}

EditDialog.RefreshFields = function (ui, guid) {
    var pks = Multi.GetSelection(guid);
    var pk;
    if (pks && pks.length == 1) {
        pk = pks[0];
    }
    else {
        return;
    }

    var refreshFields = EditDialog.GetRefreshFields(views[guid].jsonViewForCreate, $(ui.panel));
    if (refreshFields.length > 0) {
        var jsonView = EditDialog.GetJsonViewValue(pk, guid, 'Edit');

        for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
            var field = jsonView.Fields[index].Value;
            if (field.Refresh) {

                EditDialog.LoadField(pk, editPrefix, guid, field);
            }
        }
    }

}

// delete
var DeleteDialog; if (!DeleteDialog) DeleteDialog = {};

DeleteDialog.GetButtons = function (pk, guid) {
    var buttons = {};  //initialize the object to hold my buttons
    buttons[translator.Delete] = function () { DeleteDialog.Delete(pk, guid); }  //the function that does the save
    buttons[translator.cancel] = function () { $(this).dialog("close"); }  //the function that does the save

    return buttons;
}

DeleteDialog.GetButtonsForSelection = function (guid) {
    var buttons = {};  //initialize the object to hold my buttons
    buttons[translator.Delete] = function () { DeleteDialog.DeleteSelection(guid); }  //the function that does the save
    buttons[translator.cancel] = function () { $(this).dialog("close"); }  //the function that does the save
    return buttons;
}

DeleteDialog.Open = function (pk, guid) {
    $('#DeleteMessage').dialog("option", "title", translator.DeleteRowFrom + views[guid].gViewDisplayName);
    $("#DeleteMessage").dialog("option", "buttons", DeleteDialog.GetButtons(pk, guid));
    var displayValue = null;
    var selectedRow = $("#" + guid + 'ajaxDiv').find('tr.selected:first');
    if (selectedRow.length == 1) {
        displayValue = "'" + selectedRow.attr('d_displayvalue') + "'";
    }
    else {
        selectedRow = $("#" + guid + 'ajaxDiv').find('.boardselected');
        displayValue = "'" + selectedRow.clone().children().remove().end().text().trim() + "'";
    }
    if (!displayValue) {
        displayValue = $("#DeleteMessage input.x2").val();
    }

    var message = $("#DeleteMessage input.x1").val();
    if (message) {
        message = message.replace('{0}', displayValue);
        $("#DeleteMessage").text(message);
    }
    else {
        $("#DeleteMessage").text("Are you sure that you want to delete the selected row?");
    }
    $("#DeleteMessage").dialog("option", "position", 'center');
    $('#DeleteMessage').dialog('open');

}

DeleteDialog.OpenSelection = function (guid) {
    $('#DeleteSelectionMessage').dialog("option", "title", translator.DeleteSelectedRows);
    $("#DeleteSelectionMessage").dialog("option", "buttons", DeleteDialog.GetButtonsForSelection(guid));
    $("#DeleteSelectionMessage").dialog("option", "position", 'center');

    $('#DeleteSelectionMessage').dialog('open');
}

DeleteDialog.Close = function (guid) {
    if (views[guid].ReloadPage == "Delete" || views[guid].ReloadPage == "Always") {
        reloadPage();
    }

    $('#DeleteMessage').dialog('close');
}

DeleteDialog.Delete = function (pk, guid) {

    begin();

    $.post(views[guid].gDeleteUrl,
        {
            pk: pk, guid: guid
        },
        function (html) {
            var index = html.indexOf("$$error$$", 0)
            if (index < 0) {
                DeleteDialog.HandleSuccess(html, guid);
            }
            else {
                //DeleteDialog.ShowFailureMessage(html.replace("$$error$$", ""), guid);
                ajaxNotSuccessMsg(html, guid);
            }
        });
}

DeleteDialog.DeleteSelection = function (guid) {
    saveElementScrollsPosition(guid);
    begin();

    $.post(views[guid].gDeleteSelectionUrl,
        {
            pks: Sys.Serialization.JavaScriptSerializer.serialize(Multi.GetSelection(guid)), guid: guid
        },
        function (html) {
            var index = html.indexOf("$$error$$", 0)
            if (index < 0) {
                DeleteDialog.HandleSelectionSuccess(html, guid);
            }
            else {
                //DeleteDialog.ShowFailureMessage(html.replace("$$error$$", ""), guid);
                ajaxNotSuccessMsg(html, guid);
            }
        });
}

DeleteDialog.HandleSuccess = function (html, guid) {
    DeleteDialog.Close(guid);
    $('#' + guid + 'ajaxDiv').html(html);
    success(guid);
    resetElementScrollsPosition(guid);
    views[guid].updateCounter();
}

DeleteDialog.HandleSelectionSuccess = function (html, guid) {
    $('#DeleteSelectionMessage').dialog('close');
    $('#' + guid + 'ajaxDiv').html(html);
    success(guid);
    resetElementScrollsPosition(guid);
}

//DeleteDialog.ShowFailureMessage = function(message, guid) {
//    alert(message);
//    complete(guid);
//}


function AutoCompleteParse(data) {
    var rows = new Array();
    for (var i = 0; i < data.length; i++) {
        rows[i] = { data: data[i].Tag, value: data[i].Tag.PK, result: data[i].Tag.Name };
    }
    return rows;
}

function GetAutoCompleteValueId(element) {
    if (element.val() == '')
        return '';
    else
        return element.attr('valueId');
}

function SetAutoCompleteValueId(element, text, pk) {
    element.val(text);
    element.attr('valueId', pk);
}

function AutoCompleteResult(event, item) {
    //if ($(event.currentTarget).attr("valueId").length > 0)
    $(event.currentTarget).attr('valueId', item.PK);
    $(Autocomplete).trigger('result', { event: event, element: { Name: item.Name, PK: item.PK, Element: $(event.currentTarget)} });

}

function AutoCompleteSearch(event, item) {
    $(event.currentTarget).attr('valueId', '');
}

var GroupFilter; if (!GroupFilter) GroupFilter = function () { };

GroupFilter.resize = function (container) {
    var width = container.width();
    var table = GroupFilter.resizeToTable(container, 10, width);

    if (table && table.find('tr').length > 1)
        table.attr('width', '100%');

    if (table && table.find('span.ui-dropdownchecklist:first').length == 1) {
        var w = table.find('span.ui-dropdownchecklist:first').width() + 4;
        table.find('span.ui-dropdownchecklist').css('width', w + 'px');
    }
    
}

GroupFilter.getTable = function (cols, items) {
    var table = $('<table></table>');
    var tr;
    for (var itemIndex = 0; itemIndex < items.length; itemIndex++) {

        if (itemIndex % cols == 0) {
            tr = $('<tr></tr>');
            table.append(tr);
        }

        var item = $(items[itemIndex]);
        var label = item.find('.groupFilter_label').clone();
        var labelTd = $('<td></td>')
        tr.append(labelTd);
        labelTd.append(label);
        var filterTd = $('<td></td>')
        tr.append(filterTd);
        var filter = item.find('.groupFilter_element').clone();
        filterTd.append(filter);

    }

    return table;
}

var groupFilterContainerClone;

GroupFilter.resizeToTable = function (container, colCount, width) {
    var items = groupFilterContainerClone.find('div.groupFilter_item');

    for (var cols = colCount; cols > 0; cols = cols - 2) {
        var table = GroupFilter.getTable(cols, items);
        container.html(table);
        var tableWidth = table.width();
        if (tableWidth < width) {
            return table;
        }

    }

    
    return null;
}


//filter
var FilterForm; if (!FilterForm) FilterForm = function () { };

FilterForm.hide = {};

FilterForm.Toggle = function (handler, guid, mode) {
    var handler = $(handler);
    var hideFilter;

    var mode = handler.attr('mode');
    if (mode == 'hide') {
        //    In future unmark this- for tree filer (br)
        //        Durados.SplitLayout.hideSplitContent('#AppFilterTreeDiv', '#FilterTreeSplitterBar', null);
        handler.attr('mode', 'show');
        hideFilter = true;
        handler.parent().find('.filter-a').removeClass('filterClicked');
    }
    else {
        //    In future unmark this- for tree filer (br)
        //        Durados.SplitLayout.showSplitContent('#AppFilterTreeDiv', '#FilterTreeSplitterBar', null);
        handler.attr('mode', 'hide');
        hideFilter = false;
        handler.parent().find('.filter-a').addClass('filterClicked');
    }

    handler.toggleClass("filterClicked");

    $("#" + guid + "filterButtons").slideToggle("fast", function () {
        Durados.GridHandler.adjustDataTableHeight(guid);
    });
    $("#" + guid + "ajaxDiv [name=rowfilter]").first().slideToggle("fast",
    function () {
        Durados.GridHandler.adjustDataTableHeight(guid);
    });
    FilterForm.hide[guid] = hideFilter;

    //Change filter visibilty session

    //Get url
    var viewName = views[guid].ViewName;
    var controller = views[guid].Controller;
    var url = rootPath + controller + "/ChangeFilterVisibilty/" + viewName;

    $.post(url,
        {
            filterVisibilty: mode == 'show'
        });
}

//Toggle filter by FilterForm.hide[guid] 
//(To prevent a bug when view.collapseFilter=true, but we made apply so filter need to be visible)
FilterForm.HandleGridLoad = function (guid) {
    if (FilterForm.hide[guid] == null) { return; }

    var btnToggleFilter = $("#" + guid + "filterButton");
    if (!btnToggleFilter.length) { return; }

    var isFilterVisible = btnToggleFilter.hasClass("filterClicked");

    if (FilterForm.hide[guid] === false) {
        if (!isFilterVisible) {
            btnToggleFilter.click();
        }
    }
    else {
        if (isFilterVisible) {
            btnToggleFilter.click();
        }
    }
}

FilterForm.ApplySearch = function (clear, guid, searchButton, searchText) {
    var search = $(searchButton).siblings('input.search_text').val();
    if (!search || search == searchText) { return; }

    FilterForm.Apply(clear, guid, searchButton);
}

FilterForm.ApplyClear = function (clear, guid, searchButton) {
    var search = $(searchButton).siblings('input.search_text');
    //var val = search.val();
    //if (!val || val==searchText) { return; }
    search.val('');
    FilterForm.Apply(clear, guid, searchButton);
}

/************************************************************************************/
/*		FilterForm.Init (by br)			
/*		Init filter issues
/************************************************************************************/
FilterForm.Init = function (guid) {
    //    var applyFilter = function () {
    //        FilterForm.Apply(false, guid, null);
    //    }

    //    $('#' + guid + 'ajaxDiv').find('.rowfilter .GridFilterDiv select')
    //    .unbind('change', applyFilter)
    //    .bind('change', applyFilter);
    //    ;

}

FilterForm.Apply = function (clear, guid, searchButton) {
    var qparam = '';
    if (queryString('public') == 'true' || queryString('public') == 'true#') {
        var c = views[guid].gFilterUrl.indexOf('?') > -1 ? '&' : '?';
        qparam = c + 'public=true';
    }

    Durados.BeforeUnload(guid);
    showProgress();

    var search = '';

    var JsonFilter;
    var dialog;

    //var data = { JsonFilter: JsonFilter, replace: false };
    //$(FilterForm).trigger('JsonFilter', data);

    //if (data.replace) {
    //    JsonFilter = data.JsonFilter;
    //}
    if (views[guid].ViewName == "durados_v_ChangeHistory" && currentHistoryJson != null) {
        JsonFilter = currentHistoryJson;
        dialog = FloatingDialog.TheDialog;
    } else {
        JsonFilter = GetJsonFilter(guid);
        dialog = null;
    }

    if (searchButton == null) {
        if (!clear)
            JsonFilter = FillJson(JsonFilter, 'filter_', guid, dialog, false, true);
        else
            JsonFilter = ClearJson(JsonFilter, views[guid].ViewName);
    }
    else {
        search = $(searchButton).siblings('input.search_text').val();
        JsonFilter = FillJsonForSearch(GetJsonFilter(guid), 'filter_', guid, search);
    }

    if (views[guid].DisplayType != 'Table') {
        //        var reportJsonFilter = FillJson(GetJsonFilter(guid), createPrefix, guid);
        //        MergeFields(reportJsonFilter, JsonFilter);
        //        JsonFilter = reportJsonFilter;
        FillJson(JsonFilter, createPrefix, guid);
    }
    saveElementScrollsPosition(guid);

    var pks = Multi.GetSelection(guid);
    var hasTreeFilter = $('#tree_filter_' + guid).length != 0;

    var subGrid2 = guid == getMainPageGuid() ? "no" : "yes";
    
    $.post(views[guid].gFilterUrl + qparam,
        {
            jsonFilter: Sys.Serialization.JavaScriptSerializer.serialize(JsonFilter), guid: guid, search: search, mainPage: views[guid].mainPage, loadTreeFilter: hasTreeFilter, subGrid2: subGrid2
        },
        function (html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                FilterForm.HandleSuccess(html, clear, guid, pks, hasTreeFilter);
            }
            else {
                //FilterForm.ShowFailureMessage(html.replace("$$error$$", ""), guid);
                ajaxNotSuccessMsg(html, guid);
            }
        });
}

FilterForm.HandleSuccess = function (html, clear, guid, pks, needLoadTreeFilter) {
    if (needLoadTreeFilter) {
        var dataTableContent = $(html);
        var dataTableHtml = dataTableContent.find('#DataTableView').html();
        var treeFilterHtml = dataTableContent.find('#TreeFilter').html();

        $("#" + guid + "ajaxDiv").html(dataTableHtml);
        $('#tree_filter_' + guid).html(treeFilterHtml);
        //        $("#" + guid + "ajaxDiv").html($(html)ind);
    }
    else {
        $("#" + guid + "ajaxDiv").html(html);
    }
    success(guid);
    resetElementScrollsPosition(guid);
    Multi.SelectByPKs(guid, pks);
}

//FilterForm.ShowFailureMessage = function(message, guid) {
//    complete(guid);
//    alert(message);
//}


var prevSortedColumn = '';
var prevSortedDirection = '';

FilterForm.Sort = function (id, guid) {
    var qparam = '';
    if (queryString('public') == 'true' || queryString('public') == 'true#') {
        var c = views[guid].gIndexUrl.indexOf('?') > -1 ? '&' : '?';
        qparam = c + 'public=true';
    }

    if (LoadingApplicationData || movedColumnInfo[guid]) return;

    Durados.BeforeUnload(guid);
    showProgress();

    var SortColumn, direction;

    var a = $('#' + id);
    SortColumn = a.attr('SortColumn');
    direction = SortColumn == prevSortedColumn && prevSortedDirection == 'Asc' ? "Desc" : "Asc";
    saveElementScrollsPosition(guid);
    var pks = Multi.GetSelection(guid);
    $.post(views[guid].gIndexUrl + qparam,
        {
            SortColumn: SortColumn, direction: direction, guid: guid
        },
        function (html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                FilterForm.HandleSuccess(html, false, guid, pks);
                prevSortedColumn = SortColumn;
                prevSortedDirection = direction;
            }
            else {
                //FilterForm.ShowFailureMessage(html.replace("$$error$$", ""), guid);
                ajaxNotSuccessMsg(html, guid);
            }
        });
}

//Inline Adding
//var currentDialog = null;
var CreateDialog; if (!CreateDialog) CreateDialog = function () { };

//CreateDialog.AddNewCurrentDialog = function(initiator, div) {
//    var dialog = { Initiator: initiator, Div: div };
//    return dialog;
//}

CreateDialog.CreateAndOpen = function (viewName, viewDisplay, type, id, createUrl, duplicate, guid, callback) {
    var pk = null;
    if (duplicate) {
        if (type == "Autocomplete") {
            var input = $("#" + id);
            pk = GetAutoCompleteValueId(input);
        }
        else if (type == "DropDown") {
            var selectedOption = $("#" + id + " option:selected");
            pk = selectedOption.val();
        }

        if (pk == null || pk == '') {
            ajaxNotSuccessMsg('Please select a value.');
            //alert('Please select a value.');
            return;
        }
    }
    showProgress();

    var html = CreateDialog.Create(viewName, guid, createUrl.replace("Create/", "Dialog/"));

    var div;

    var dialogId = guid + viewName + 'inlineAdding';
    div = $('#' + dialogId);

    if (div.length == 0) {
        div = $("<div>");
        div.attr('id', dialogId);
        $("body").prepend(div);
    }
    div.html(html);

    if ((guid && guid != 'null') || viewName == "Menu") {
        var cache = views[guid].TabCache == 'True';

        var createTab = div.find('#' + guid + 'InlineAddingTabs');

        createTab.tabs({ fx: { opacity: 'toggle', duration: 'fast' }, cache: cache, ajaxOptions: { cache: false, async: false} }); // first tab selected
        disableChildrenTabs(createTab);
    }


    //currentDialog = CreateDialog.AddNewCurrentDialog(currentDialog, div);
    CreateDialog.Open(viewName, viewDisplay, type, id, createUrl, guid, pk, duplicate, div, callback);

    if (jsonViewsForInlineDialog[guid] && jsonViewsForInlineDialog[guid].Fields) {

        Durados.Dependencies.cleanUp();
        for (var i = 0; i < jsonViewsForInlineDialog[guid].Fields.length; i++) {
            if (jsonViewsForInlineDialog[guid].Fields[i].Value.DependencyData) {
                var n = jsonViewsForInlineDialog[guid].Fields[i].Key;
                var f = $('#' + guid + inlineAddingPrefix + ReplaceNonAlphaNumeric(n));
                var v = f.val();

                Durados.Dependencies.Apply(n, v, guid, div, jsonViewsForInlineDialog[guid].Fields);

                Durados.Dependencies.createHandlers($('#' + guid + inlineAddingPrefix + ReplaceNonAlphaNumeric(jsonViewsForInlineDialog[guid].Fields[i].Key)), guid, jsonViewsForInlineDialog[guid].Fields, viewName, inlineAddingPrefix);
            }
        }

    }

    //Create wysiwyg for all textareas
    CreateDialog.CreateWysiwyg(guid, viewName);


    complete(guid);
    initDropdownChecklistsCreate(guid);

    $(Durados.View).trigger('add', { pk: pk, guid: guid, dialog: div, viewName: viewName, prefix: inlineAddingPrefix, originElementId: id });

    return div;
}

CreateDialog.Create = function (viewName, guid, addingUrl) {
    var syncHtml = '';
    var url = '';
    if (!guid || guid == 'null') {
        if (!addingUrl)
            addingUrl = '/Home/InlineAddingDialog/';
        url = addingUrl + viewName;
    }
    else {
        url = views[guid].gInlineAddingDialogUrl + viewName;
    }
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: viewName, guid: guid },
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                syncHtml = html;
            }
            else {
                ajaxNotSuccessMsg(html);
            }
        }

    });


    return syncHtml;
}

CreateDialog.Open = function (viewName, viewDisplay, type, id, createUrl, guid, pk, duplicate, dialog, callback) {

    var winWidth = $(window).width();
    var winHeight = $(window).height();

    currentViewName = viewName;

    var rec = Rectangle.Load(currentViewName);
    //var dialog = currentDialog.Div;

    if (rec != null) {
        dialog.dialog({
            bgiframe: true,
            autoOpen: false,
            position: [rec.left, rec.top],
            width: rec.width,
            height: rec.height,
            modal: true,
            close: CreateDialog.CloseEvent,
            resizeStop: SaveDialogOnResize,
            zIndex: 999999,
            dragStop: SaveDialogOnDrag,
            close: function (event, ui) {
                Durados.Tabs.select($('#' + guid + 'InlineAddingTabs'), 0);
                //                $('#' + guid + 'InlineAddingTabs').tabs("select", 0)
            },
            open: function (event, ui) {
                var state = $.cookie("state_" + viewName);
                if (state == 'max' || IE7) {
                    dialogExt.max(dialog, null, guid);
                }
                currentDialog = dialog;
                window.setTimeout(function () {
                    jQuery(document).unbind('mousedown.dialog-overlay').unbind('mouseup.dialog-overlay');
                }, 1);
            }

        });

    }
    else {
        dialog.dialog({
            bgiframe: true,
            autoOpen: false,
            position: 'center',
            width: (winWidth * .75),
            modal: true,
            close: CreateDialog.CloseEvent,
            position: 'center',
            resizeStop: SaveDialogOnResize,
            zIndex: 999999,
            dragStop: SaveDialogOnDrag,
            close: function (event, ui) {
                if ($('#' + guid + 'InlineAddingTabs').length > 0) {
                    try {
                        Durados.Tabs.select($('#' + guid + 'InlineAddingTabs'), 0);
                        //                        $('#' + guid + 'InlineAddingTabs').tabs("select", 0);
                    }
                    catch (err) {
                    }
                }
            },
            open: function (event, ui) {

                var state = $.cookie("state_" + viewName);
                if (state == 'max' || IE7) {
                    dialogExt.max(dialog, null, guid);
                }
                currentDialog = dialog;
                window.setTimeout(function () {
                    jQuery(document).unbind('mousedown.dialog-overlay').unbind('mouseup.dialog-overlay');
                }, 1);
            }
        });
    }

    dialog.dialog("option", "title", 'Add ' + viewDisplay);
    dialog.dialog("option", "buttons", {
        "Save and Close": function () { CreateDialog.Add(viewName, false, type, id, createUrl, guid, dialog, callback); },
        "Close": function () { CreateDialog.Close(dialog, guid); }
    });

    dialogExt.initMaxButton(dialog, guid);

    showProgress();

    var json = null;
    if (duplicate) {
        json = GetJsonViewForInlineEditing(viewName, guid, pk);
        if (json == null) {
            ajaxNotSuccessMsg("The record that you request to edit has been deleted.", guid);
            //alert("The record the you request to edit has been deleted.");
            //complete(guid);
            return;
        }
        EditDialog.Load(pk, inlineAddingPrefix, guid, json, viewName + 'inlineAdding')

    }
    else {
        json = GetJsonViewForInlineAdding(viewName, guid, createUrl.replace('InlineAddingCreate/', 'GetJsonView/'));
        AddDialog.SetDefaults(json, inlineAddingPrefix, guid);
    }

    var state = $.cookie("state_" + viewName);
    if (state != "max") {
        if (rec != null) {
            dialog.dialog("option", "position", [rec.left, rec.top]);

        }
    }

    jsonViewsForInlineDialog[guid] = json;

    InitValidation(guid, inlineAddingPrefix, json);
    subgridsHashes[guid] = new Array();

    initDerivation(json.Derivation, dialog, inlineAddingPrefix, guid, json.ViewName);
    initDerivationOnShow(json.Derivation, dialog, inlineAddingPrefix, guid, json.ViewName);

    dialog.dialog({
        show: { effect: 'fold', duration: 500 },
        hide: { effect: 'fold', duration: 500 }
    });
    dialog.dialog('open');

    CreateDialog.CreateWysiwyg(guid, viewName);
    hideProgress();


    setFocusInDialog(dialog);

    if (viewName == "Field") {
        var dataType = dialog.find('[name="DataType"]');
        dialog.find('[name=RelatedViewName]').parents('tr:first').parents('tr:first').hide();// hide related view
        if (dataType.length == 1) {
            dataType.change(function () {
                setDataType(dialog, dataType)

            });
            
        }
    }

}

var dataTypeDiv = null;
function setDataType(dialog, dataType) {
    if (dataType.val() == 'MultiSelect')
        dialog.find('[name=RelatedViewName]').parents('tr:first').parents('tr:first').show(); // hide related view
    else
        dialog.find('[name=RelatedViewName]').parents('tr:first').parents('tr:first').hide(); // hide related view
       
    if (dataType.val() == 'SingleSelect') {
        var table = dataType.parents('table:first').parents('table:first');
        if (table.find('tr.data-type-settings-row').length == 0) {
            dataTypeDiv = $('<div class="relation-settings"></div>').load('/Admin/GetDataTypeDialog', function () {
                var tr = dataType.parents('tr:first').parents('tr:first');
                var newTr = $('<tr class="data-type-settings-row"></tr>');
                newTr.insertAfter(tr);
                var newTd = $('<td colspan="2"></td>');
                newTr.append(newTd);
                newTd.append(dataTypeDiv);

                var database = dialog.find('[name=Database]');

                var relatedViewName = dialog.find('[name=RelatedViewName]');
                var relatedViewNameVal = relatedViewName.val();

                var existingTable = dialog.find('.existing-tables');
                var existingSystemTable = dialog.find('.existing-system-tables');

                existingTable.val(relatedViewNameVal);
                existingSystemTable.val(relatedViewNameVal);

                database.change(function () {
                    var v = $(this).val();

                    if (v == "1") {
                        existingTable.show();
                        existingSystemTable.hide();
                    }
                    else {
                        existingTable.hide();
                        existingSystemTable.show();

                    }
                });

                var relatedTable = dialog.find('[name=RelatedTable]');

                relatedTable.change(function () {
                    var v = $(this).val();

                    if (v == "1") {
                        dialog.find('.ExistingRelatedTableName').css('display', 'block');
                        dialog.find('.NewRelatedTableName').hide();
                    }
                    else {
                        dialog.find('.ExistingRelatedTableName').hide();
                        dialog.find('.NewRelatedTableName').css('display', 'block');

                    }
                });


                //dialog.find('.InYourDatabase').click();
                //dialog.find('.ExistingRelatedTable').click();
                if (existingSystemTable.val()) {
                    dialog.find('.InSystemDatabase').prop('checked', true);
                    dialog.find('.InSystemDatabase').change();
                }
                else {
                    dialog.find('.InYourDatabase').prop('checked', true);
                    dialog.find('.InYourDatabase').change();
                }
                dialog.find('.ExistingRelatedTable').prop('checked', true);
                dialog.find('.ExistingRelatedTable').change();

            });
            
        }
        dataTypeDiv.show();
    }
    else {
        if (dataTypeDiv) {
            dataTypeDiv.hide();
        }
    }
}

function isCreatingNewTableInYourDb(dialog) {
    var your = dialog.find('.InYourDatabase').prop('checked');
    var existing = dialog.find('.ExistingRelatedTable').prop('checked');
    return your && !existing;
}

function isValidDataType(dialog, dataType) {
    if (dataType.val() != 'SingleSelect') {
        return true;
    }

    var your = dialog.find('.InYourDatabase').prop('checked');
    var existing = dialog.find('.ExistingRelatedTable').prop('checked');
    var existingTables = dialog.find('.existing-tables');
    var existingSystemTables = dialog.find('.existing-system-tables');
    var newRelatedTableName = dialog.find('.new-related-table-name');

    if (your) {
        if (existing) {
            var v = existingTables.val();

            if (!v) {
                Durados.Dialogs.Alert('', 'Please select existing table or view.', function () { existingTables.focus(); });
                return false;
            }
            else {
                dialog.find('[name=RelatedViewName]').val(v);
            } 
        }
        else {
            var v = newRelatedTableName.val();
            dialog.find('[name=RelatedViewName]').val(v);
//            if (!v) {
//                Durados.Dialogs.Alert('', 'Please enter new table name.', function () { $('#NewRelatedTableName').focus(); });
//                return false;
//            }
//            else {
//                dialog.find('[name=RelatedViewName]').val(v);
//            } 
        }
    }
    else {
        if (existing) {
            var v = existingSystemTables.val();

            if (!v) {
                Durados.Dialogs.Alert('', 'Please select existing table or view.', function () { existingSystemTables.focus(); });
                return false;
            }
            else {
                dialog.find('[name=RelatedViewName]').val(v);
            } 
        }
        else {
            var v = newRelatedTableName.val();
            dialog.find('[name=RelatedViewName]').val(v);

//            if (!v) {
//                Durados.Dialogs.Alert('', 'Please enter new table name.', function () { $('#NewRelatedTableName').focus(); });
//                return false;
//            }
//            else {
//                dialog.find('[name=RelatedViewName]').val('_d_new_.' + v);
//            } 
        }
    }

    return true;

//    $('#ExistingTables').change(function () {
//        dialog.find('[name=RelatedViewName]').val($(this).val());
//    });

//    $('#ExistingSystemTables').change(function () {
//        dialog.find('[name=RelatedViewName]').val($(this).val());
//    });

//    $('#NewRelatedTableName').change(function () {
//        if ($('#InYourDatabase').prop('checked')) {
//            dialog.find('[name=RelatedViewName]').val($(this).val());
//        }
//        else {
//            dialog.find('[name=RelatedViewName]').val('_d_new_.' + $(this).val());
//        }
//    });
}


CreateDialog.CreateWysiwyg = function (guid, viewName) {


    $('#' + guid + 'InlineAdding' + ReplaceNonAlphaNumeric(viewName) + 'DataRowForm textarea[rich="true"]').each(function () { $(this).htmlarea("dispose") });
    $('#' + guid + 'InlineAdding' + ReplaceNonAlphaNumeric(viewName) + 'DataRowForm textarea[rich="true"]').htmlarea();


    /*   
    try {
    $(this).htmlarea("dispose");
    $(this).htmlarea();
    }
    catch (err) {
    }
    createTab.unbind("tabsshow").bind("tabsshow", function(event, ui) {
    ui.panel.find('textarea[rich="true"]').each(function() { 
    $(this).htmlarea("dispose");
    $(this).htmlarea();
    });
    }); */

    //$('#' + guid + 'InlineAdding' + viewName + 'DataRowForm textarea[rich="true"]').htmlarea();
}

CreateDialog.Reset = function (viewName) {
    $('#' + guid + 'InlineAdding' + ReplaceNonAlphaNumeric(viewName) + 'DataRowForm')[0].reset();
    resetUpload();
}



CreateDialog.Close = function (dialog, guid) {
    //dialog.html('');
    dialog.dialog('close');
}

CreateDialog.CloseEvent = function () {


}

CreateDialog.Add = function (viewName, another, type, id, createUrl, guid, dialog, callback) {
    if (CreateDialog.isValid(guid, viewName) == false) {
        modalSuccessMsg(getErrorsList(dialog));
        //alert(getErrorsList(dialog));
        return;
    }

    if (viewName == "Field") {
        var dataType = dialog.find('[name="DataType"]');
        //dialog.find('[name=RelatedViewName]').parents('tr:first').parents('tr:first').hide();
        if (dataType.length == 1) {
            if (!isValidDataType(dialog, dataType)) {
                blockOperation = false;
                hideProgress();
                return;
            }
            if (isCreatingNewTableInYourDb(dialog) && !confirm("The following change is going to create new table, continue?")) {
                blockOperation = false;
                hideProgress();
                return;
            }

        }
    }

    var json = FillJson(GetJsonViewForInlineAdding(viewName, guid, createUrl.replace('InlineAddingCreate/', 'GetJsonView/')), inlineAddingPrefix, guid, dialog);

    if (viewName == "Field" && dialog.find('.InSystemDatabase').prop('checked')) {
        var field = jQuery.extend(true, {}, json.Fields[0]);
        field.Key = "sysdb";
        field.Value.Value = true;
        field.Value.Name = "sysdb";
        json.Fields.push(field);
    }

    showProgress();

    $.post(createUrl + viewName,
        {
            jsonView: Sys.Serialization.JavaScriptSerializer.serialize(json)
        },
        function (jsonDisplayValue) {
            hideProgress();
            var index = jsonDisplayValue.indexOf("$$error$$", 0);
            if (index < 0 || index > 1000) {
                if (jsonDisplayValue != '') {

                    var displayValue = Sys.Serialization.JavaScriptSerializer.deserialize(jsonDisplayValue);
                    //                displayValue = Sys.Serialization.JavaScriptSerializer.deserialize(displayValue);
                    CreateDialog.HandleSuccess(another, type, id, displayValue, guid, dialog, viewName);
                }
                else {
                    if (callback)
                        callback(another, type, id, displayValue, guid, dialog, viewName);

                    InlineEditingDialog.Close(dialog, guid, false, true);
                }
            }
            else {
                //CreateDialog.ShowFailureMessage(jsonDisplayValue.replace("$$error$$", ""), guid);
                ajaxNotSuccessMsg(jsonDisplayValue.replace("$$error$$", ""), guid, viewName == "Field");
            }
        });
}

CreateDialog.onafterInlineAdding = function () { }


CreateDialog.HandleSuccess = function (another, type, id, displayValue, guid, dialog, viewName) {
    if (type == 'DropDown') {
        var field = displayValue.Fields[0].Value;
        var value = field.Value;
        var text = field.Default;
        var editID = id.replace(createPrefix, editPrefix);
        var createID = id.replace(editPrefix, createPrefix);

        var selectCreate = $('#' + createID);
        selectCreate.append('<option value="' + value + '">' + text + '</option>');
        if (editID != createID) {
            var selectEdit = $('#' + editID);
            selectEdit.append('<option value="' + value + '">' + text + '</option>');
        }
        var select = $('#' + id);
        $(CreateDialog).trigger('onafterInlineAdding', { element: select, displayValue: displayValue, id: id, type: type, guid: guid, dialog: dialog, viewName: viewName });
    }

    if (another) {
        CreateDialog.Reset(viewName);
    }
    else {
        CreateDialog.Close(dialog, guid);
        //            AddDialog.Show();
    }

    if (type == 'DropDown') {
        var select = $('#' + id);
        select.val(value);
        select.change();
        select.focus();
    }
    else if (type == 'Autocomplete') {
        var field = displayValue.Fields[0].Value;
        var value = field.Value;
        var text = field.Default;
        var input = $('#' + id);
        input.val(text);
        input.attr('valueId', value);
        input.focus();
    }
}

CreateDialog.isValid = function (guid, viewName) {

    var form = $('#' + guid + 'InlineAdding' + ReplaceNonAlphaNumeric(viewName) + 'DataRowForm')[0];

    if (Spry.Widget.Form.validate(form) == false) {
        return false;
    }
    return true;
}




//CreateDialog.ShowFailureMessage = function(message, guid) {
//    complete(guid);
//    alert(message);
//}



//Inline Editing
var InlineEditingDialog; if (!InlineEditingDialog) InlineEditingDialog = function () { };

//InlineEditingDialog.AddNewCurrentDialog = function(initiator, div) {
//    var dialog = { Initiator: initiator, Div: div };
//    return dialog;
//}

InlineEditingDialog.CreateAndOpen = function (viewName, viewDisplay, type, id, editUrl, guid, e) {
    var pk = null;
    if (type == "Autocomplete") {
        var input = $("#" + id);
        pk = GetAutoCompleteValueId(input);
    }
    else if (type == "DropDown") {
        var selectedOption = $("#" + id + " option:selected");
        pk = selectedOption.val();
    }
    else if (type == "tableView") {
        var gridEditing = $(id).parent('TD.d_fieldContainer').attr('d_role') == 'cell';
        if (gridEditing && !e.ctrlKey)
            return;
        pk = $(id).attr('pk');
    }

    if (pk == null || pk == '') {
        ajaxNotSuccessMsg('Please select a value.');
        //alert('Please select a value.');
        return;
    }

    InlineEditingDialog.CreateAndOpen2(viewName, viewDisplay, type, id, editUrl, guid, e, pk);
}

InlineEditingDialog.CreateAndOpen2 = function (viewName, viewDisplay, type, id, editUrl, guid, e, pk, isAdmin, callback, close, previewCallback, closeCallback) {

    showProgress();
    var html = InlineEditingDialog.Create(viewName, guid, pk);

    var div;

    div = $('#' + guid + ReplaceNonAlphaNumeric(viewName) + 'inlineEditing');

    if (div.length == 0) {
        div = $("<div>");
        div.attr('id', guid + ReplaceNonAlphaNumeric(viewName) + 'inlineEditing');
        $("body").prepend(div);
    }
    div.html(html);
    var cache = false;
    if (guid)
        cache = views[guid].TabCache == 'True';

    var isAccordion = false; //InitAccordion(guid, 1000);

    if (guid) {
        var editAccordion = div.find('#' + guid + 'InlineEditingAccordion');

        if (editAccordion.length > 0) {
            editAccordion.accordion({ collapsible: true, autoHeight: false, changestart: function (event, ui) { accordionChanged(ui, guid, pk); } }); // first tab selected

            isAccordion = true;
        }

    }


    if ((!isAccordion && guid) || viewName == "Menu") {
        var editTab = div.find('#' + guid + 'InlineEditingTabs');

        editTab.tabs({ fx: { opacity: 'toggle', duration: 'fast' }, cache: cache, ajaxOptions: { cache: false, async: false} }); // first tab selected

        disableChildrenTabs(editTab);

    }

    var editSlider = div.find('.slider-container');
    slider.InitSlider(null); //editSlider
    //editSlider.find('.ui-slider').anythingSlider({ expand: true, showMultiple: 4, buildNavigation: false, buildStartStop: false }); 



    var jsonView = GetJsonViewForInlineEditing(viewName, guid, pk);
    if (jsonView == null) {
        ajaxNotSuccessMsg("The record that you request to edit has been deleted.", guid);
        //alert("The record the you request to edit has been deleted.");
        //complete(guid);
        return div;
    }
    if (viewName == "View" || viewName == "Field" || (viewName == "Page" && guid == pageGuid)) PreviewDisplayModeViewJson = jsonView;
    EditDialog.Load(pk, inlineEditingPrefix, guid, jsonView, viewName + 'inlineEditing');

    if ((!isAccordion && guid) || viewName == "Menu") {
        AddDialog.HandleTab(pk, 'InlineEditingTabs', guid);
        var tab = $('#' + guid + 'InlineEditingTabs');
        Durados.Tabs.select(tab, 0);
        //        tab.tabs("select", 0)


        //            tab.tabs({
        //                load: function (event, ui) {
        //                    tabLoaded(ui);
        //                },
        //                show: function (event, ui) {
        //                    hideProgress();
        //                },
        //                select: function (event, ui) {
        //                    //                    setTimeout(function () {
        ////                    tabSelected(ui, "create", guid);

        //                    showProgress();
        //                    //                    });
        //                }
        //            });
        tab.bind("tabsshow", function (event, ui) {
            tabLoaded(ui);
        });


    }
    //currentDialog = InlineEditingDialog.AddNewCurrentDialog(currentDialog, div);
    InlineEditingDialog.Open(viewName, viewDisplay, type, id, editUrl, guid, div, pk, callback, close, previewCallback, closeCallback);

    if (!guid && viewName != "Menu") {
        try {
            InitValidation(guid, inlineEditingPrefix, jsonView);
        }
        catch (err) {
        }

        try {
            var guid2 = 'null';
            subgridsHashes[guid2] = new Array();

            initDerivation(jsonView.Derivation, div, inlineEditingPrefix, guid2, jsonView.ViewName);
            initDerivationOnShow(jsonView.Derivation, div, inlineEditingPrefix, guid2, jsonView.ViewName);
        }
        catch (err) {
        }

        return div;
    }
   
    InitValidation(guid, inlineEditingPrefix, jsonView);

    subgridsHashes[guid] = new Array();

    initDerivation(jsonView.Derivation, div, inlineEditingPrefix, guid, jsonView.ViewName);
    initDerivationOnShow(jsonView.Derivation, div, inlineEditingPrefix, guid, jsonView.ViewName);

    EditDialog.DisableCloneView(inlineEditingPrefix, guid, jsonView, isAdmin);

    for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
        var field = jsonView.Fields[index].Value;
        if (field.DependencyData) {
            Durados.Dependencies.Apply(field.Name, field.Value, guid, div, jsonView.Fields);
        }
    }

    //Create wysiwyg for all textareas
    InlineEditingDialog.CreateWysiwyg(guid, viewName);

    $(Durados.View).trigger('edit', { pk: pk, guid: guid, dialog: div, viewName: viewName, prefix: inlineEditingPrefix, originElementId: id });

    complete(guid);
    initDropdownChecklistsEdit(guid);

    return div;
}

var InlineEditingViews = [];

InlineEditingDialog.Create = function (viewName, guid, pk) {
    if (InlineEditingViews[viewName] && !(guid == pageGuid && viewName == 'Page')) {
        return InlineEditingViews[viewName];
    }

    var syncHtml = '';
    var url = guid ? views[guid].gInlineEditingDialogUrl + viewName : '/Home/InlineEditingDialog/' + viewName;

    if (viewName == "View" || viewName == "Field" || (viewName == "Page" && guid == pageGuid)) {
        url = url + '?Pk=' + pk;
    }

    var zIndex = 999999;
    if (guid == pageGuid)
        zIndex = 10;

    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: viewName, guid: guid },
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        zIndex: zIndex,
        success: function (html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                syncHtml = html;
            }
            else {
                //InlineEditingDialog.ShowFailureMessage(html.replace("$$error$$", ""));
                ajaxNotSuccessMsg(html);
            }
        }

    });

    InlineEditingViews[viewName] = syncHtml;

    return syncHtml;
}

InlineEditingDialog.Open = function (viewName, viewDisplay, type, id, editUrl, guid, dialog, pk, callback, close, previewCallback, closeCallback) {

    var winWidth = $(window).width();
    var winHeight = $(window).height();

    currentViewName = viewName;
    var dialogClose = function (close) {
        var askSaveChanges = guid != pageGuid;

        var dialogClosed = InlineEditingDialog.Close(dialog, guid, askSaveChanges, close);
        if (dialogClosed) {
            if (viewName == 'Field' || viewName == 'View') {
                InlineEditingDialog.resetConfig(guid);
            }
        }
        return dialogClosed;
    }

    var rec = Rectangle.Load(currentViewName);
    if (guid == pageGuid) {
        if (!rec)
            rec = new Object();
        rec.left = 250;
        rec.top = 110;
    }
    //var dialog = currentDialog.Div;
    var modal = true;
    if (guid == pageGuid) {
        modal = false;
    }

    //if (viewName == "View" || viewName == "Field") modal = false;
    if (rec != null) {
        dialog.dialog({
            bgiframe: true,
            autoOpen: false,
            position: [rec.left, rec.top],
            width: rec.width,
            height: rec.height,
            modal: modal,
            close: InlineEditingDialog.CloseEvent,
            resizeStop: SaveDialogOnResize,
            zIndex: 999999,
            dragStop: SaveDialogOnDrag,
            beforeClose: function (event, ui) {
                return dialogClose(false);
            },
            close: function (event, ui) {
                Durados.Tabs.select($('#' + guid + 'InlineEditingTabs'), 0);
                //                $('#' + guid + 'InlineEditingTabs').tabs("select", 0)
                if (closeCallback)
                    closeCallback(event, ui);
            },
            open: function (event, ui) {
                var state = $.cookie("state_" + viewName);
                if (state == 'max' || IE7) {
                    dialogExt.max(dialog, null, guid);
                }
                currentDialog = dialog;
                window.setTimeout(function () {
                    jQuery(document).unbind('mousedown.dialog-overlay').unbind('mouseup.dialog-overlay');
                }, 1);
                if (guid == pageGuid) {
                    dialog.dialog("widget").find('.ui-dialog-titlebar').hide();
                    dialog.dialog("widget").css('border', 'none');
                    dialog.dialog("widget").css('border-left', '1px #ccc solid');
                }

                if (pk == '') {
                    dialog.find('form').hide();
                    dialog.find('.page-settings .page-button-container').hide();
                }
                else {
                    dialog.find('form').show();
                    dialog.find('.page-settings .page-button-container').show();
                }
            }


        });

    }
    else {
        dialog.dialog({
            bgiframe: true,
            autoOpen: false,
            width: (winWidth * .75),
            modal: modal,
            close: InlineEditingDialog.CloseEvent,
            position: 'center',
            resizeStop: SaveDialogOnResize,
            zIndex: 999999,
            dragStop: SaveDialogOnDrag,
            beforeClose: function (event, ui) {
                return dialogClose(false);
                //                if (InlineEditingDialog.Close(dialog, guid, false)) {
                //                    if (viewName == 'Field' || viewName == 'View') {
                //                        InlineEditingDialog.resetConfig(guid);
                //                    }
                //                }
                //                else {
                //                    return false;
                //                }
            },
            close: function (event, ui) {
                //                dialog.parent().find(".ui-dialog-buttonpane .buttton-close").click();
                Durados.Tabs.select($('#' + guid + 'InlineEditingTabs'), 0);
                //                $('#' + guid + 'InlineEditingTabs').tabs("select", 0)
            },
            open: function (event, ui) {
                var state = $.cookie("state_" + viewName);
                if (state == 'max' || IE7) {
                    dialogExt.max(dialog, null, guid);
                }
                currentDialog = dialog;
                window.setTimeout(function () {
                    jQuery(document).unbind('mousedown.dialog-overlay').unbind('mouseup.dialog-overlay');
                }, 1);

            }
        });
    }

    if (guid != pageGuid) {
        dialogExt.initMaxButton(dialog, guid);
        dialogExt.iniDockButton(dialog, guid);
    }

    if (viewDisplay.indexOf(translator.Settings) > -1) {
        dialog.dialog("option", "title", viewDisplay);
    }
    else {
        dialog.dialog("option", "title", 'Edit ' + viewDisplay);
    }

    if (guid != pageGuid) {
        dialog.dialog("option", "buttons", {
            "Save and Close": function () { InlineEditingDialog.Edit(viewName, type, id, editUrl, guid, dialog, pk, callback, close); },
            "Close": {
                text: "Close",
                click: function () {
                    dialogClose(true);
                }
            }
        });
    }

    showProgress();

    var state = $.cookie("state_" + viewName);
    if (state != "max") {
        if (rec != null) {
            dialog.dialog("option", "position", [rec.left, rec.top]);

        }
    }

    dialog.dialog({
        show: { effect: 'fold', duration: 500 },
        hide: { effect: 'fold', duration: 500 }
    });
    dialog.dialog('open');

    InlineEditingDialog.CreateWysiwyg(guid, viewName);
    hideProgress();

    //Bind dialog inputs to dialog change event
    Durados.Dialogs.BindToChangeEvent('input', dialog);
    Durados.Dialogs.BindToChangeEvent('select', dialog);
    Durados.Dialogs.BindToChangeEvent('textarea', dialog);


    if (viewName == "Field") {
        var dataType = dialog.find('[name="DataType"]');
        dialog.find('[name=RelatedViewName]').parents('tr:first').parents('tr:first').hide();
        if (dataType.length == 1) {
            dataType.change(function () {
                setDataType(dialog, dataType)

            });
            if (dataType.val() == 'SingleSelect') {
                setDataType(dialog, dataType)

            }
        }
    }


    setFocusInDialog(dialog);

    if (viewName == 'Field' || viewName == 'View' || viewName == 'Page') {
        InlineEditingDialog.SetAdminPreviewEvents(dialog, viewName, guid, previewCallback);
    }

}
InlineEditingDialog.resetConfig = function (guid) {
    PreviewDisplayModeViewJson = null;
    $.post('/Home/PreviewModeOff/', {},
            function (json) {
                if (json != "success") {
                    return;
                }

                if (isViewItem(guid)) {
                    window.parent.Refresh(window.parent.getMainPageGuid(), true);
                    //window.parent.reloadPage('&settings=true');
                }
                else {
                    Refresh(guid);
                }

            });
}

var customThemeChanged = false;

InlineEditingDialog.SetAdminPreviewEvents = function (dialog, configViewName, guid, previewCallback) {
    var editor = new Durados.FieldEditor();
    var fieldpk = $(dialog).attr('pk');
    //editor.init();
    dialog.find('[adminPreview="true"]').each(function () {
        var element = $(this);
        //var guid = element.attr('id').substr(0, (element.attr('id').indexOf("inlineEditing_")));
        var name = element.attr('name'); //name of field property
        var url = '/Admin/PreviewEdit/' + configViewName;
        var elementType = editor.getElementType(element);
        if (elementType == "Text" || elementType == "Autocomplete") {// && !element.isBound('blur', InlineEditingDialog.SetUpdatePreview)
            element.bind('change', { elementType: "Text", pk: fieldpk, name: name, guid: guid, url: url, dialog: dialog, viewName: configViewName, element: element, previewCallback: previewCallback }, InlineEditingDialog.SetUpdatePreview);
        }

        else if (elementType == "Check" || elementType == "Color") {//&& !element.isBound('change', InlineEditingDialog.SetUpdatePreview)
            element.bind('change', { elementType: elementType, pk: fieldpk, name: name, guid: guid, url: url, dialog: dialog, viewName: configViewName, element: element, previewCallback: previewCallback }, InlineEditingDialog.SetUpdatePreview);
        }

        else if (elementType == "DropDown" || elementType == "Hidden") {// && !element.isBound('change', InlineEditingDialog.SetUpdatePreview)
            element.bind('change', { elementType: "DropDown", pk: fieldpk, name: name, guid: guid, url: url, dialog: dialog, viewName: configViewName, element: element, previewCallback: previewCallback }, InlineEditingDialog.SetUpdatePreview);
        }


    });

    dialog.find('textarea[adminPreview="true"]').each(function () {
        var element = $(this);
        var guid = element.attr('id').substr(0, (element.attr('id').indexOf("inlineEditing_")));
        var name = element.attr('name'); //name of field property
        var url = '/Admin/PreviewEdit/' + configViewName;
        var elementType = editor.getElementType(element);
        if (elementType == "TextArea") {// && !element.isBound('change', InlineEditingDialog.SetUpdatePreview)
            element.bind('change', { elementType: "DropDown", pk: fieldpk, name: name, guid: guid, url: url, dialog: dialog, viewName: configViewName, previewCallback: previewCallback }, InlineEditingDialog.SetUpdatePreview);
        }


    });

    dialog.find('input[name="ApplyColorDesignToAllViews"]').each(function () {
        var applyCheckbox = $(this);
        applyCheckbox.bind('change', { elm: applyCheckbox }, function () {
            if (Durados.CheckBox.IsChecked(applyCheckbox)) {
                Durados.Dialogs.Confirm(translator.ApplyColorDesignTitle, translator.ApplyColorDesignMessage, function () { Durados.CheckBox.SetChecked(applyCheckbox, true); }, function () { Durados.CheckBox.SetChecked(applyCheckbox, false); });
            }
        });
    });

    dialog.find('input[name="ApplySkinToAllViews"]').each(function () {
        var applyCheckbox = $(this);
        applyCheckbox.bind('change', { elm: applyCheckbox }, function () {
            if (Durados.CheckBox.IsChecked(applyCheckbox)) {
                Durados.Dialogs.Confirm(translator.ApplySkinTitle, translator.ApplySkinMessage, function () { Durados.CheckBox.SetChecked(applyCheckbox, true); }, function () { Durados.CheckBox.SetChecked(applyCheckbox, false); });
            }
        });
    });

    var theme = dialog.find('input[name="Theme"]');
    var customThemePath = dialog.find('input[name="CustomThemePath"]');

    var disableCustomTheme = function () {
        var themeId = theme.val();
        customThemePath.attr('disabled', themeId != "Custom");
    }
    disableCustomTheme();
    customThemeChanged = false
    theme.change(function () {
        customThemeChanged = true;
        disableCustomTheme();
    });
    
}

PreviewDisplayModeViewJson = null;

//var confirmations = [];
InlineEditingDialog.SetUpdatePreview = function (e) {
    var guid = e.data.guid;
    var viewJson = PreviewDisplayModeViewJson;
    var viewName = e.data.viewName;
    var fieldName = e.data.name;
    var element = e.data.element;
    var cur = element.val();
    var isViewOwner = views[guid] == null ? false : views[guid].Role == "View Owner";
    var fieldId = e.data.pk;

    if (viewName == "Field" && fieldName == "DataType" && isViewOwner) {
        var pre = null;
        for (var index = 0, len = viewJson.Fields.length; index < len; ++index) {
            if (viewJson.Fields[index].Key == fieldName) {
                pre = viewJson.Fields[index].Value.Value;
                break;
            }
        }

        $.ajax({
            url: '/Admin/HasData',
            contentType: 'application/json; charset=utf-8',
            data: { fieldId: fieldId },
            async: false,
            cache: false,
            error: ajaxErrorsHandler,
            success: function (json) {
                if (json) {
                    Durados.Dialogs.Confirm(translator.ChangeTypeTitle, translator.ChangeTypeMessage, function () { InlineEditingDialog.SetUpdatePreview2(e); }, function () {
                        element.val(pre);
                    });
                }
                else {
                    InlineEditingDialog.SetUpdatePreview2(e);
                }
            }

        });

        return false;
    }
    else {
        if (confirmChangeType) {
            Durados.Dialogs.Confirm(translator.ChangeTypeTitle, "This change will affect the database schema.<br/>The change can not be rolled back.<br/>A database backup is recommended.", function () { InlineEditingDialog.SetUpdatePreview2(e); }, function () {
                element.val(confirmChangeTypePrevVal);
            });
        }
        else {
            InlineEditingDialog.SetUpdatePreview2(e);
        }
    }
}

InlineEditingDialog.SetUpdatePreview2 = function (e) {
    showProgress();
    setTimeout(function () {
        $(InlineEditingDialog).trigger('beforeUpdatePreview', { guid: e.guid, dialog: e.dialog, pk: e.pk, viewName: e.viewName });
        var value = null;
        if (e.data == null || e.data.elementType == null) {
            hideProgress();
            return;
        }
        switch (e.data.elementType) {
            case "Check": value = $(getEventSrcElement(e)).is(':checked');
                break;
            case "Text": value = $(getEventSrcElement(e)).val();
                break;
            case "Color":
                var element = $(getEventSrcElement(e));
                var checkBox = element.siblings('input[type=checkbox]');
                var isChecked = Durados.CheckBox.IsChecked(checkBox);
                value = isChecked ? element.val() : '';
                break;
            case "DropDown": value = $(getEventSrcElement(e)).val();
                break;
        }

        if (PreviewDisplayModeViewJson == null) {
            hideProgress();
            return;
        }
        var viewJson = PreviewDisplayModeViewJson;
        for (var index = 0, len = viewJson.Fields.length; index < len; ++index) {
            if (viewJson.Fields[index].Key == e.data.name) {
                if (viewJson.Fields[index].Value.Value != value) {
                    viewJson.Fields[index].Value.Value = value;

                    var guid = e.data.guid;
                    if (window.parent && window.parent.Refresh && window.location.href.indexOf('Admin/Item/View?') > -1) {
                        guid = window.parent.getMainPageGuid();
                    }

                    InlineEditingDialog.UpdatePreview(e, e.data.pk, e.data.name, value, guid, e.data.url, e.data.viewName, e.data.dialog, e.data.previewCallback);
                    if (plugIn()) {
                        break;
                    }

                    if (window.parent && window.parent.Refresh && window.location.href.indexOf('Admin/Item/View?') > -1) {
                        window.parent.Refresh(window.parent.getMainPageGuid(), true);
                    }
                    else if (window.parent && window.parent.parent && window.parent.parent.Refresh && window.location.href.indexOf('Admin/IndexPage/Field?') > -1) {
                        window.parent.parent.Refresh(window.parent.parent.getMainPageGuid(), true);
                    }

                    break;
                }

            }
        }
    }, 1);
    hideProgress();
}

function getEventSrcElement(e) {
    var targ;
    if (!e) var e = window.event;
    if (e.target) targ = e.target;
    else if (e.srcElement) targ = e.srcElement;
    if (targ.nodeType == 3) // defeat Safari bug
        targ = targ.parentNode;
    return targ;
}


InlineEditingDialog.UpdatePreview = function (e, fieldpk, name, value, guid, url, viewName, dialog, previewCallback) {

    try {
        if (viewName == 'Field' && (plugIn() || name == "DataType" || name == "Excluded" || name == "DisplayFormat")) {
            dialog.parent().find('button.ui-button').each(function () {
                if ($(this).text() == translator.save) {
                    if (!(value == false && name == "Excluded")) {
                        $(this).click();
                    }
                    return;
                }
            });
            //parent.parent.refreshWidget2();
        }
    }
    catch (err) { }
    try {
        if (viewName == 'View' && parent && parent.refreshWidget2) {
            dialog.parent().find('button.ui-button').each(function () {
                if ($(this).text() == translator.save) {
                    $(this).click();
                    return;
                }
            });
            //parent.parent.refreshWidget2();
        }
    }
    catch (err) { }

    if (!plugIn() && name != "DataType" && name != "Excluded") {
        $.post(url, { pk: fieldpk, property: name, value: value, guid: guid }
            ,
            function (json) {
                if (json != "success") {
                    return;
                }

                adminPreviewChanged = true;

                if (e && isViewItem(e.data.guid)) {
                    var mainGuid = window.parent.getMainPageGuid();

                    if (name == 'Skin') {
                        window.parent.InlineEditingDialog.HandleSkin(value, mainGuid);
                    }
                    else if (name == "DisplayName" && viewName == "View") {
                        window.parent.InlineEditingDialog.HandleViewDisplayName(value);
                    }
                    window.parent.Refresh(mainGuid, true);
                }
                else if (name == "DisplayName") {
                    var row = $('.boardselected');
                    row.text(value);

                }

                if (previewCallback) {
                    previewCallback(fieldpk, name, value, guid, url, viewName, dialog);
                }
            });

    }
}

var adminPreviewChanged = false;

InlineEditingDialog.HandleViewDisplayName = function (value) {
    $('#rowtabletitleSpan:first').text(value);
}

var loadCSS = function (url, callback) {
    var link = document.createElement('link');
    link.type = 'text/css';
    link.rel = 'stylesheet';
    link.href = url;

    document.getElementsByTagName('head')[0].appendChild(link);

    var img = document.createElement('img');
    img.onerror = function () {
        if (callback) callback(link);
    }
    img.src = url;
}

InlineEditingDialog.HandleSkin = function (value, guid) {
    var link = $('link[role="skin"]');
    var href = link.attr('href');
    href = href.substring(0, href.lastIndexOf('/') + 1) + value + ".css";
    link.attr('href', href);

    //The next rows are trick to register to event occures when finish load  new css.
    //Register to link load event does not work well!
    var img = document.createElement('img');
    img.onerror = function () {
        Durados.GridHandler.setColumnsWidth(guid);
    }
    img.src = href;
}

InlineEditingDialog.CreateWysiwyg = function (guid, viewName) {
    $('#' + guid + 'InlineEditing' + ReplaceNonAlphaNumeric(viewName) + 'DataRowForm textarea[rich="true"]').each(function () { $(this).htmlarea("dispose") });
    $('#' + guid + 'InlineEditing' + ReplaceNonAlphaNumeric(viewName) + 'DataRowForm textarea[rich="true"]').htmlarea();
}

InlineEditingDialog.Reset = function (viewName) {
    $('#' + guid + 'InlineEditing' + ReplaceNonAlphaNumeric(viewName) + 'DataRowForm')[0].reset();
    resetUpload();
}



InlineEditingDialog.Close = function (dialog, guid, askSaveChanges, close) {

    var needClose = true;

    if (askSaveChanges) {
        var changed = dialog.attr('changed') == 'yes';

        if (changed) {
            if (confirm("Exit without save?")) {
                dialog.attr('changed', 'no');
            }
            else {
                needClose = false;
            }
        }
        else {
            dialog.attr('changed', 'no');
        }
    }
    //    else {
    //        dialog.attr('changed', 'no');
    //    }

    if (needClose && close) {
        dialog.dialog('close');
    }

    return needClose;
}

InlineEditingDialog.CloseEvent = function () {


}

InlineEditingDialog.Edit = function (viewName, type, id, editUrl, guid, dialog, pk, callback, close) {
    if (InlineEditingDialog.isValid(guid, viewName) == false) {
        modalSuccessMsg(getErrorsList(dialog));
        //alert(getErrorsList(dialog));
        return;
    }

    showProgress();

    if (id != null) {
        if (type == "Autocomplete") {
            var input = $("#" + id);
            pk = GetAutoCompleteValueId(input);
        }
        else if (type == "DropDown") {
            var selectedOption = $("#" + id + " option:selected");
            pk = selectedOption.val();
        }
        else if (type == "tableView") {
            pk = $(id).attr('pk');
        }
    }
    var url = editUrl + viewName;
    if (viewName == "View" || viewName == "Field" || viewName == "Page") {
        url = url + '?Pk=' + pk;
    }

    if (viewName == "Field") {
        var dataType = dialog.find('[name="DataType"]');
        //dialog.find('[name=RelatedViewName]').parents('tr:first').parents('tr:first').hide();
        if (dataType.length == 1) {
            if (!isValidDataType(dialog, dataType)) {
                blockOperation = false;
                hideProgress();
                return;
            }
            if (isCreatingNewTableInYourDb(dialog) && !confirm("The following change is going to create new table, continue?")) {
                blockOperation = false;
                hideProgress();
                return;
            }

        }
    }

    var json = FillJson(GetJsonViewForInlineEditing(viewName, guid, pk), inlineEditingPrefix, guid, dialog);

    if (viewName == "Field" && dialog.find('.InSystemDatabase').prop('checked')) {
        var field = jQuery.extend(true, {}, json.Fields[0]);
        field.Key = "sysdb";
        field.Value.Value = true;
        field.Value.Name = "sysdb";
        json.Fields.push(field);
    }

    $.post(url,
        {
            jsonView: Sys.Serialization.JavaScriptSerializer.serialize(json), pk: pk, guid: guid
        },
        function (jsonDisplayValue) {
            hideProgress();
            var index = jsonDisplayValue.indexOf("$$error$$", 0);
            if (index < 0 || index > 1000) {
                var displayValue = Sys.Serialization.JavaScriptSerializer.deserialize(jsonDisplayValue);
                //                displayValue = Sys.Serialization.JavaScriptSerializer.deserialize(displayValue);

                InlineEditingDialog.HandleSuccess(type, id, displayValue, guid, dialog, close);

                if (callback)
                    callback(viewName, type, id, editUrl, guid, dialog, pk);
            }
            else {
                //InlineEditingDialog.ShowFailureMessage(jsonDisplayValue.replace("$$error$$", ""), guid);
                ajaxNotSuccessMsg(jsonDisplayValue.replace("$$error$$", ""), guid, viewName == "Field");
            }
        });
}



InlineEditingDialog.onafterInlineEditing = function () { }


InlineEditingDialog.HandleSuccess = function (type, id, displayValue, guid, dialog, close) {
    if (id != null) {

        if (type == 'DropDown') {
            var select = $('#' + id);
            var field = displayValue.Fields[0].Value;
            var value = field.Value;
            var text = field.Default;
            var selectedOption = $("#" + id + " option:selected");
            selectedOption.text(text);
            selectedOption.val(value);

            $(InlineEditingDialog).trigger('onafterInlineEditing', select);

        }
    }

    if (close == null || close) {
        dialog.attr('changed', 'no');
        InlineEditingDialog.Close(dialog, guid, false, true);
    }

    if (id != null) {

        if (type == 'DropDown') {
            var select = $('#' + id);
            select.val(value);
            select.change();
        }
        else if (type == 'Autocomplete') {
            var field = displayValue.Fields[0].Value;
            var value = field.Value;
            var text = field.Default;
            var input = $('#' + id);
            input.val(text);
            input.attr('valueId', value);
            input.focus();
        }
        else if (type == 'tableView') {
            var field = displayValue.Fields[0].Value;
            var value = field.Value;
            var text = field.Default;
            $(id).attr('pk', value);
            $(id).text(text);
        }
    }
}

InlineEditingDialog.isValid = function (guid, viewName) {

    var form = $('#' + guid + 'InlineEditing' + ReplaceNonAlphaNumeric(viewName) + 'DataRowForm')[0];

    if (Spry.Widget.Form.validate(form) == false) {
        return false;
    }
    return true;
}




//InlineEditingDialog.ShowFailureMessage = function(message, guid) {
//    complete(guid);
//    alert(message);
//}




/// multi
var Multi; if (!Multi) Multi = function () { };

Multi.Toggle = function (handler, guid) {
    if (Durados.CheckBox.IsChecked($(handler))) {
        Multi.All(guid);
    }
    else {
        Multi.Clear(guid);
    }
}

Multi.All = function (guid) {
    $("#" + guid + 'ajaxDiv').find('input.Multi').each(function () {
        if ($(this).parents('tr').first().attr('guid') == guid)
        //$(this).attr('checked', 'checked');
            Durados.CheckBox.SetChecked($(this), 'checked');
    });
    Multi.MarkSelection(guid);
}

Multi.Clear = function (guid) {
    $("#" + guid + 'ajaxDiv').find('input.Multi').each(function () {
        if ($(this).parents('tr').first().attr('guid') == guid)
        //$(this).attr('checked', false);
            Durados.CheckBox.SetChecked($(this), false);
    });
    Multi.MarkSelection(guid);

    cellsSelection.clear();

    copyPaste = new Durados.CopyPaste(guid);
}

//Multi.Clear = function(guid) {
//    $("#" + guid + 'ajaxDiv').find('.Multi:not("")').attr('checked', '');
//}

Multi.GetSelection = function (guid) {
    var selection = new Array();

    if (guid == null || views[guid] == null) {
        return;
    }

    var displayType = views[guid].DataDisplayType;

    if (displayType == "Table") {
        $("#" + guid + 'ajaxDiv').find("input.Multi:checked").each(function () {
            if ($(this).parents('tr').first().attr('guid') == guid) {
                var pk = $(this).attr("pk");
                selection.push(pk);
            }
        });
    }
    else {
        $("#" + guid + 'ajaxDiv').find("div.boardtitle.boardselected[guid=" + guid + "]").each(function () {
            var pk = $(this).attr("d_pk");
            selection.push(pk);
        });
    }

    return selection;
}

Multi.GetSingleSelection = function (guid) {
    var pks = Multi.GetSelection(guid);
    if (pks.length == 1) {
        return pks[0];
    }
    else if (pks.length > 1) {
        modalErrorMsg('Please select only one row');
    }
    else {
        modalErrorMsg('Please select a row');
    }

    return null;
}

Multi.GetNextPK = function (pk, guid) {
    var elm = Multi.GetElementByPK(guid, pk);
    return $(elm).attr("nextPK");
}

Multi.GetPrevPK = function (pk, guid) {
    var elm = Multi.GetElementByPK(guid, pk);
    return $(elm).attr("prevPK");
}


Multi.Init = function (guid) {
    $("#" + guid + 'ajaxDiv').find('input.Multi').each(function () {
        if ($(this).parents('tr').first().attr('guid') == guid) {
            var checkbox = $(this);

            checkbox.change(function (e) {
                //var needCheck = isChecked(checkbox);
                var needCheck = Durados.CheckBox.IsChecked(checkbox);

                Multi.Select(guid, views[guid].MultiSelect, needCheck, e.ctrlKey, checkbox[0]);
            });

        }
    });
}

Multi.Select = function (guid, multi, isChecked, ctrl, elm) {
    ctrl = true;
    if (multi && ctrl) {

    }
    else {
        if (isChecked) {

        }
        else {
            Multi.Clear(guid);
            //$(elm).attr('checked', 'checked');
            Durados.CheckBox.SetChecked($(elm), 'checked');
        }
    }
    Multi.MarkSelection(guid);

}


Multi.BoardAll = function (guid, select) {
    $("#" + guid + 'ajaxDiv div.boardtitle').each(function () {
        $board = $(this);
        if ($board.attr('guid') == guid) {
            if (select) {
                $board.addClass('boardselected');
                //$board.parent().find('input.Multi').attr('checked', 'checked');
                Durados.CheckBox.SetChecked($board.parent().find('input.Multi'), 'checked');
            } else {
                $(this).removeClass('boardselected');
                //$(this).parent().find('input.Multi').attr('checked', '');
                Durados.CheckBox.SetChecked($(this).parent().find('input.Multi'), '');
            }
        }

    });
}


Multi.BoardClicked = function (e, board, guid, selectAnyway) {

    var $board = $(board);

    if (e != null && !e.ctrlKey || !views[guid].MultiSelect) {
        $("#" + guid + 'ajaxDiv div.boardselected').each(function () {
            if ($(this).attr("d_pk") != $board.attr("d_pk")) {
                $(this).removeClass('boardselected');
                //$(this).parent().find('input.Multi').attr('checked', '');
                Durados.CheckBox.SetChecked($(this).parent().find('input.Multi'), '');
            }
        });
    }

    Multi.toggleBoardSelection($(board), selectAnyway);
}

Multi.toggleBoardSelection = function ($board, selectAnyway) {
    if ($board.hasClass('boardselected')) {
        if (!selectAnyway) {
            $board.removeClass('boardselected');
            //$board.parent().find('input.Multi').attr('checked', '');
            Durados.CheckBox.SetChecked($board.parent().find('input.Multi'), 'checked');
        }
    }
    else {
        $board.addClass('boardselected');
        //$board.parent().find('input.Multi').attr('checked', 'checked');
        Durados.CheckBox.SetChecked($board.parent().find('input.Multi'), 'checked');
    }
}

Multi.getFirstRowPK = function (guid) {
    var grid = $("#" + guid + 'ajaxDiv').find('table.gridview:first');
    if (!grid.length) return;
    return grid.find('tr[guid="' + guid + '"]:first').attr('d_pk');
}

Multi.MarkSelection = function (guid) {
    var grid = $("#" + guid + 'ajaxDiv').find('table.gridview:first');
    if (!grid.length) return;

    if (grid.find('.hideCheckboxes').length != 0) return;

    var rows = grid.find('tr[guid="' + guid + '"]');
    var c = 0;
    var firstRow = null;
    rows.each(function () {
        var row = $(this);
        var board = $(this).attr('mode') == 'board';
        if (board)
            $board = row.parents('div.boardrow').first().find('div.boardtitle').first();
        var checkbox = row.find("input.Multi:first");
        if (checkbox.length == 1) {
            //if (isChecked(checkbox)) {
            if (Durados.CheckBox.IsChecked(checkbox)) {
                if (!board) {
                    row.addClass('selected');
                    row.removeClass('hovered');
                } else {
                    $board.addClass('boardselected');
                }
                if (firstRow == null) {
                    firstRow = row;
                }
                c++;
            }
            else {
                if (!board)
                    row.removeClass('selected');
                else
                    $board.removeClass('boardselected');
            }
        }
    });

    if (c != 1) {
        firstRow = null;
    }

    Multi.UpdateGridTitle(firstRow, grid, guid);
}

var displayValues = new Array();

Multi.UpdateGridTitle = function (firstRow, grid, guid) {
    var isMainGrid = grid.parents("table.gridview").length == 0;

    if (isMainGrid) {
        var menu = $('#rowtabletitleSpan').attr("d_Dn");
        if (firstRow) {
            var rowmenu = firstRow.attr('d_displayValue');
            if (rowmenu) {
                displayValues[guid] = rowmenu;
                menu += ' - ' + rowmenu;
            }
        } else {
            displayValues[guid] = menu;
        }
        if (menu) {
            $('#rowtabletitleSpan').next().text(rowmenu);
        }
    }
}

Multi.GetElementByPK = function (guid, pk) {
    var el = $("#" + guid + 'ajaxDiv').find('input.Multi[pk="' + pk + '"]').first();
    if (el.length == 1)
        return el[0];
    else
        return null;
}

Multi.SelectByPK = function (guid, pk) {
    var el = Multi.GetElementByPK(guid, pk);
    if (el != null)
        Multi.Select(guid, false, false, false, el);
}

Multi.SelectByPKs = function (guid, pks) {

    Multi.Clear(guid);

    var els = [];

    if (pks) {
        $(pks).each(function () {
            var el = Multi.GetElementByPK(guid, this);
            if (el) { els.push(el); }
        });

        $(els).each(function () {
            //$(this).attr('checked', 'checked');
            Durados.CheckBox.SetChecked($(this), 'checked');
        });

        Multi.MarkSelection(guid);
    }
}

//function isChecked(checkbox) {
//    return checkbox.attr('checked') == 'checked' || checkbox.attr('checked') || checkbox.is('[CHECKED]');
//}

function getCheckedQueryStringValue(checkbox) {
    //if (isChecked(checkbox))
    if (Durados.CheckBox.IsChecked(checkbox))
        return '1';
    else
        return '0';
}

function Print(guid) {
    window.print();
    //$("#" + guid + "ajaxDiv").printElement({ leaveOpen: true, printMode: 'popup', overrideElementCSS: [
    // '/Content/general.min.1.0.0.css',
    //	{ href: '/Content/PrintGrid.css', media: 'all'}]
    //});
}

function rightTrim(sString) {
    while (sString.substring(sString.length - 1, sString.length) == '&') {
        sString = sString.substring(0, sString.length - 1);
    }
    return sString;
}



function success(guid) {

    complete(guid);

    initDropdownchecklistFilters(guid);
    //FilterForm.Update();
}

function initDropdownchecklistFilters(guid) {
    $("#" + guid + 'ajaxDiv').find("select.dropdownchecklist").each(function () {
        if ($(this).attr('filter') == 'filter')
            initDropdownchecklistFilter($(this), guid);
    });
}

function begin() {
    showProgress();
}

var cellsSelection;
var copyPaste;

function initTooltip(elm) {

    var title = elm.attr('title');
    if (title == '')
        return;
    if (title.indexOf('|') == -1) {
        elm.attr('title', '|' + title)
    }
    elm.cluetip({
        splitTitle: '|',
        clickThrough: true,
        cluetipClass: 'jtip',
        arrows: true,
        dropShadow: false,
        hoverIntent: false,
        sticky: title.indexOf('href') != -1,
        mouseOutClose: true,
        closePosition: 'title',
        closeText: '<img src="/content/images/cross.png" alt="close" />'
    });
}

function initTooltips() {
    $('#rowtabletitleSpan').each(function () {
        initTooltip($(this));
    });
    Durados.Preview.init();
}

function triggerEditorBlur() {
    if (adjustDataTableHeightDisabled) return;
    Durados.FieldEditor.setFocus();
    $(document).click();
    Durados.FieldEditor.CloseAllEditors();
}

function complete(guid) {

    //InitCommandMenu(guid);   
    //initDropdownchecklists(guid);
    //InitContextMenu(guid);
    //InitTopMenu();
    var container = $('.groupFilter[guid="' + guid + '"]');
    if (container.length == 1) {
        groupFilterContainerClone = container.clone();
        GroupFilter.resize(container);
        $(window).resize(function () {
            GroupFilter.resize(container);
            //success(guid);
        });
    }

    // hide filter for fields in settings
    if ($('#' + guid + 'filterButton').length == 0) {
        container.hide();
    }

    Multi.Init(guid);

    FilterForm.HandleGridLoad(guid);

    adjustSubGridUI(guid);

    Durados.GridHandler.adjustDataTable(guid);

    //if (isChecked($('#' + guid + "Safety")) && !$('body').attr('unselectable'))
    if (Durados.CheckBox.IsChecked($('#' + guid + "Safety")) && !$('body').attr('unselectable'))
        $('body').attr('unselectable', 'on').addClass('unselectable');

    $('input.date').each(function () {

        var dateType = num(DateFormats.getValidDateType($(this).attr("df")));
        //        var dateType = num(DateFormats.getValidDateType($(this).attr("dt")));
        var df = duradosGetJQueryDateFormat($(this).attr("df"), dateType);
        var dateFormat = df.dateFormat;
        switch (dateType) {
            case DateFormats.dateType.dateOnly:
                $(this).datepicker({ showButtonPanel: true, showOn: 'button', buttonImage: rootPath + "Content/smoothness/images/calendar.jpg", beforeShow: datePickerBeforeShow, onChangeMonthYear: datePickerOnChangeMonthYear, buttonImageOnly: true, dateFormat: dateFormat, onSelect: function (dateText, inst) { if (inst.input) setTimeout("triggerDateChanged('#" + $(inst.input).attr('id') + "');", 500); } });
                break;
            case DateFormats.dateType.dateAndTime:
                $(this).datetimepicker({ showButtonPanel: true, showOn: 'button', buttonImage: rootPath + "Content/smoothness/images/calendar.jpg", beforeShow: datePickerBeforeShow, onChangeMonthYear: datePickerOnChangeMonthYear, buttonImageOnly: true, dateFormat: df.dateFormat, timeFormat: df.timeFormat, onSelect: function (dateText, inst) { if (inst.input) setTimeout("triggerDateChanged('#" + $(inst.input).attr('id') + "');", 500); } });
                break;
            case DateFormats.dateType.timeOnly:
                $(this).timepicker({ showButtonPanel: true, showOn: 'button', buttonImage: rootPath + "Content/smoothness/images/calendar.jpg", beforeShow: datePickerBeforeShow, onChangeMonthYear: datePickerOnChangeMonthYear, buttonImageOnly: true, timeFormat: df.timeFormat, onSelect: function (dateText, inst) { if (inst.input) setTimeout("triggerDateChanged('#" + $(inst.input).attr('id') + "');", 500); } });
                break;
            default:
                break;
        }
    });

    
    
    Autocomplete.Init(guid);

    //initAdvancedFiters(guid);

    hideProgress();

    //YAHOO.tool.Profiler.start("editopen");

    $('#color_input').crayonbox({
        onSelection: function () {
            var c = $(this).find('.coloring').attr('title');
            if (!c) { c = "white"; }
            $('#color_choozer').css("background-color", c);
            $(this).hide();
        },
        choozerId: "color_choozer",

        onShowClicked: function () { $(this).toggle(); }
    });
    //YAHOO.tool.Profiler.stop("editopen");


    cellsSelection = new Durados.CellsSelection();

    copyPaste = new Durados.CopyPaste(guid);

    if (Durados.ColumnResizer.initHandler != null)
        Durados.ColumnResizer.init(guid);

    initTooltips();

    var pks1 = Multi.GetSelection(guid);
    if (pks1 && pks1.length > 1) {
    }
    else {
        Durados.DisplayType.InitByDisplayType(guid);
    }

    try {
        FilterForm.Init(guid);
        initMoreActions(guid);
        initWatermark();
        initCheckboxes();
        initImages();
        initButtons();
        initEditors();
        initColorPickers();
    }
    catch (err) { }
    //Multi.MarkSelection(guid);

    Durados.GridHandler.setColumnsWidth(guid);

    initTableDnd(guid);

    $(Durados.View).trigger('refreshGrid', { guid: guid });
   
}

savePositionTableDnd = function (draggedElementPk, targetElenentPk, guid) {
    if (guid == null) {
        return;
    }

    //Get url
    //trySaveChanges(guid);
    var viewName = views[guid].ViewName;
    var controller = views[guid].Controller;
    var url = rootPath + controller + "/ChangeOrdinal/" + viewName;

    showProgress();
    $.post(url, { o_pk: draggedElementPk, d_pk: targetElenentPk, guid: guid }, function () {
        try {
            hideProgress();
        }
        catch (err) {
            hideProgress();
        }
    });
}


function initTableDnd(guid) {
    
    var ajaxDiv = $('#' + guid + 'ajaxDiv');
    if (!ajaxDiv.length) { return; }

    var viewPort = ajaxDiv.find('div.fixedViewPort').first();

    var table = viewPort.find('table:first');

    if (!table.tableDnD)
        return;

    var prevIndex = null;


    table.tableDnD({
        onDrop: function (table, draggedRow) {
            if (draggedRow) {
                var currIndex = draggedRow.rowIndex;
                if (prevIndex != currIndex) {
                    var direction = prevIndex < currIndex ? "down" : "up";

                    //Get draggedElementPk, targetElenentPk
                    var draggedElementPk = $(draggedRow).attr("d_pk");
                    var targetElenentPk = direction == "up" ? $(draggedRow).next().attr("d_pk") : $(draggedRow).prev().attr("d_pk");
                    savePositionTableDnd(draggedElementPk, targetElenentPk, guid);
                }
            }
        },
        onDragStart: function (table, row) {
            prevIndex = row.rowIndex;
        },
        dragHandle: ".dragHandle"
    });

    table.find('tr').each(function () {
        $(this).find('td:first').attr('style','padding-left: 24px !important;');
    });

    table.find('tr').hover(function () {
        $(this.cells[0]).addClass('showDragHandle');
    }, function () {
        $(this.cells[0]).removeClass('showDragHandle');
    });
}

function triggerDateChanged(id, cell) {
    var el = $(id);
    if (!el.length) return;

    var dateType = num(DateFormats.getValidDateType(el.attr("dt")));
    var dateFormat = duradosGetJQueryDateFormat(el.attr("df"), dateType);

    if (el.val() == $.datepicker.formatDate(dateFormat.dateFormat, DuradosNADate)) {
        el.addClass('nadate');
        if (cell) {
            $(cell).addClass('nadateTD');
        }
    } else {
        el.removeClass('nadate');
        if (cell) {
            $(cell).removeClass('nadateTD');
        }
    }


    //}
}


//function initAdvancedFiters(guid) {}

var LoadingApplicationData = false;
var IE7 = (navigator.appVersion.indexOf("MSIE 7.") == -1) ? false : true;

function getDocumentScope(_window) {
    try {
        _window = _window != null ? _window : window;
        //        var scope = _window.document;
        var scope = _window.document;

        if (_window.parent && ((_window.isDock && _window.isDock()) || (_window.location.pathname.indexOf('/IndexPage/') == -1))) {
            scope = getDocumentScope(_window.parent);
        }
    }
    catch (err) {
        return scope;
    }
    return scope;
}

function isDuringLoadIframe() {
    var _isDuringLoadIframe = false;

    try {

        _isDuringLoadIframe = Durados.DuringdLoadIframe || (isDockFields() && window.parent.parent.$("#loadSettings").is(":visible"));
    } catch (err) {

    }
    return _isDuringLoadIframe;
}

function showProgress() {
    try {
        if (!isDuringLoadIframe()) {

            LoadingApplicationData = true;

            if (plugIn()) {
                $('body').css("cursor", "wait");
                $("#pluginProgress", window.parent.document).show();
                $("#pluginProgress").show();
            }
            else {
                var scope = getDocumentScope();
                $('body', scope).css("cursor", "wait");
                $('#progress', scope).show();
            }
        }
    }
    catch (err) {
    }
    //$.blockUI({ message: '<h1><img src="' + rootPath + 'Content/Images/progress-wheel.gif" /> Please wait...</h1>' });
}

function hideProgress() {
    try {
        //    $('#body', scope).css("cursor", "default");
        LoadingApplicationData = false;

        if (plugIn()) {
            $('body').css("cursor", "default");
            $("#pluginProgress", window.parent.document).hide();
            $("#pluginProgress").hide();
        }
        else {
            var scope = getDocumentScope();
            $('body', scope).css("cursor", "default");

            var laoding = $('#progress', scope);
            if (laoding.length == 0)
                laoding = $('#progress');
            laoding.hide();
        }
    }
    catch (err) {
    }
}

//Language Functions
function SetLanguage(guid) {
    var language = $('#languageDropDown').val();
    SetSelectedLanguage(language, guid);
}

function SetSelectedLanguage(language, guid) {
    var url = '';
    if (guid && views[guid].gSetLanguageUrl)
        url = views[guid].gSetLanguageUrl;
    else {
        url = top.location.href;
        $.ajax({
            url: '/Admin/SetLanguage2',
            contentType: 'application/html; charset=utf-8',
            data: { languageCode: language },
            async: false,
            dataType: 'html',
            cache: false,
            error: ajaxErrorsHandler,
            success: function (html) {
                top.location.href = url;
            }
        });
    }
    if (url.indexOf("?") > -1) {
        url = url + '&languageCode=' + language;
    }
    else {
        url = url + '?languageCode=' + language;
    }
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
    $('#' + imgID).attr('src', rootPath + "Content/Images/Minus.JPG");
}

function HideMenu(id, imgID) {
    $('#' + id).css('display', 'none');
    $('#' + imgID).attr('src', rootPath + "Content/Images/Plus.JPG");
}

function ChangePageSize(guid, select, viewName, url) {
    begin();
    setTimeout(function () {
        ChangePageSize2(guid, select, viewName, url);
    });
}

////// pager
function ChangePageSize2(guid, select, viewName, url) {
    var qparam = '';
    if (queryString('public') == 'true' || queryString('public') == 'true#') {
        var c = url.indexOf('?') > -1 ? '&' : '?';
        qparam = c + 'public=true';
    }

    url = url + qparam;

    var pageSize = $(select).val();

    Durados.BeforeUnload(guid);
    saveElementScrollsPosition(guid);
    $.ajax({
        url: url,
        contentType: 'application/html; charset=utf-8',
        data: { viewName: viewName, pageSize: pageSize, guid: guid },
        async: false,
        dataType: 'html',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (html) {
            $('#' + guid + 'ajaxDiv').html(html);
            success(guid);
        }
    });

}

function SafetyModeChanged(guid) {
    Durados.BeforeUnload(guid);
    saveElementScrollsPosition(guid);
    showProgress();
    var safety = Durados.CheckBox.IsChecked($('#' + guid + "Safety"));
    setTimeout(function () {
        $.ajax({
            url: views[guid].gIndexUrl,
            contentType: 'application/html; charset=utf-8',
            //data: { viewName: views[guid].gViewName, guid: guid, mainPage: views[guid].mainPage == "True", safety: isChecked($('#' + guid + "Safety")) },
            data: { viewName: views[guid].gViewName, guid: guid, mainPage: views[guid].mainPage == "True", safety: safety },
            async: false,
            dataType: 'html',
            cache: false,
            error: ajaxErrorsHandler,
            success: function (html) {
                $('#' + guid + 'ajaxDiv').html(html);
                success(guid);
                resetElementScrollsPosition(guid);
            }
        });
    }, 1);
}

function Refresh(guid, dialogs) {
    if (!views[guid])
        return;

    var url = views[guid].gIndexUrl;

    if (!url)
        return;

    var qparam = '';
    if (queryString('public') == 'true' || queryString('public') == 'true#') {
        var c = url.indexOf('?') > -1 ? '&' : '?';
        qparam = c + 'public=true';
    }

    url = url + qparam;
    //    url = '/Home/IndexPage/List';
    if (url == 'durados_v_ChangeHistory') {
        FilterForm.Apply(false, guid, null);
        return;
    }
    Durados.BeforeUnload(guid);
    saveElementScrollsPosition(guid);
    var pks = Multi.GetSelection(guid);
    showProgress();

    var firstTime = "False";
    if (dialogs)
        firstTime = "True";

    setTimeout(function () {
        $.ajax({
            url: url,
            contentType: 'application/html; charset=utf-8',
            data: { viewName: views[guid].gViewName, guid: guid, mainPage: views[guid].mainPage == "True", firstTime: firstTime },
            async: false,
            dataType: 'html',
            cache: false,
            error: ajaxErrorsHandler,
            success: function (html) {
                if (dialogs) {
                    destroyDialogs(guid);
                    var grid = $(html);
                    if (grid.length == 0)
                        return;
                    var newGuid = grid.attr('guid');
                    $('#mainAppDiv').bind('load', function () {
                        alert('111');
                    });
                    $('tr').bind('load', function () {
                        alert('111');
                    });
                    $('#mainAppDiv').html(html);

                    initDataTableView(newGuid);
                }
                else {
                    $('#' + guid + 'ajaxDiv').html(html);
                    success(guid);
                    resetElementScrollsPosition(guid);
                    Multi.SelectByPKs(guid, pks);
                }
            }
        });
    }, 1);
}

function destroyDialogs(guid) {
    $("#" + guid + "DataRowEdit").dialog("destroy");
    $("#" + guid + "DataRowCreate").dialog("destroy");
    $("#" + guid + "DataRowEdit").remove();
    $("#" + guid + "DataRowCreate").remove();
}

function MovePage(guid, inputText, currentPage, lastPage, viewName, url) {
    var qparam = '';
    if (queryString('public') == 'true' || queryString('public') == 'true#') {
        var c = url.indexOf('?') > -1 ? '&' : '?';
        qparam = c + 'public=true';
    }

    url = url + qparam;
    
    var page = parseInt($(inputText).val());

    if (page >= 1 && page <= lastPage) {
        $.ajax({
            url: url,
            data: { viewName: viewName, page: page, guid: guid },
            async: false,
            cache: false,
            error: ajaxErrorsHandler,
            success: function (html) {
                $('#' + guid + 'ajaxDiv').html(html);
                success(guid);
                //Durados.GridHandler.adjustDataTableHeight(guid);
            }

        });
    }
    else {
        $(inputText).val(currentPage);
    }
}

function tabSelected(ui, action, guid) {
    if (action == "edit") {
        EditDialog.RefreshFields(ui, guid);
    }
    $(EditDialog).trigger('tabSelected', { guid: guid, ui: ui, action: action });
}

function tabLoaded(ui) {
    var grid = $(ui.panel).find('div[ajaxDiv="ajaxDiv"]');
    if (grid.length == 0)
        return;

    if (grid.attr("isLoaded") == "isLoaded")
        return;

    var newGuid = grid.attr('guid');
    showProgress();
    initDataTableView(newGuid);
    views[newGuid].setTab($(ui.tab));
    views[newGuid].updateCounter();
    grid.attr("isLoaded", "isLoaded");
}

Durados.View.initDataTableView = function () { }

function LoadView(guid) {

    var jsonViewForCreate = $('#' + guid + 'jsonViewForCreate').val();
    if (jsonViewForCreate != null)
        jsonViewForCreate = Sys.Serialization.JavaScriptSerializer.deserialize(jsonViewForCreate);

    var jsonViewForEdit = $('#' + guid + 'jsonViewForEdit').val();
    if (jsonViewForEdit != null)
        jsonViewForEdit = Sys.Serialization.JavaScriptSerializer.deserialize(jsonViewForEdit);

    var GetJsonFilter = $('#' + guid + 'GetJsonFilter').val();
    if (GetJsonFilter != null)
        GetJsonFilter = Sys.Serialization.JavaScriptSerializer.deserialize(GetJsonFilter);


    views[guid] = new Durados.View(
        guid,
        $('#' + guid + 'ViewName').val(),
        $('#' + guid + 'GetCreateUrl').val(),
        $('#' + guid + 'GetEditUrl').val(),
        $('#' + guid + 'GetEditSelectionUrl').val(),
        $('#' + guid + 'GetDuplicateUrl').val(),
        $('#' + guid + 'GetJsonViewUrl').val(),
        $('#' + guid + 'GetRichUrl').val(),
        $('#' + guid + 'GetEditRichUrl').val(),
        $('#' + guid + 'GetSelectListUrl').val(),
        $('#' + guid + 'GetJsonViewInlineAddingUrl').val(),
        $('#' + guid + 'GetJsonViewInlineEditingUrl').val(),
        $('#' + guid + 'GetInlineAddingDialogUrl').val(),
        $('#' + guid + 'GetInlineEditingDialogUrl').val(),
        $('#' + guid + 'GetInlineDuplicateDialogUrl').val(),
        $('#' + guid + 'GetInlineSearchDialogUrl').val(),
        $('#' + guid + 'GetDeleteUrl').val(),
        $('#' + guid + 'GetDeleteSelectionUrl').val(),
        $('#' + guid + 'GetFilterUrl').val(),
        $('#' + guid + 'GetSetLanguageUrl').val(),
        $('#' + guid + 'GetIndexUrl').val(),
        $('#' + guid + 'GetExportToCsvUrl').val(),
        $('#' + guid + 'GetPrintUrl').val(),
        $('#' + guid + 'GetAutoCompleteUrl').val(),
        $('#' + guid + 'DisplayName').val(),
        $('#' + guid + 'viewName').val(),
        jsonViewForCreate,
        $('#' + guid + 'GetUploadUrl').val(),
        jsonViewForEdit,
        GetJsonFilter,
        $('#' + guid + 'Controller').val(),
        $('#' + guid + 'TabCache').val(),
        $('#' + guid + 'RefreshOnClose').val(),
        $('#' + guid + 'GetRefreshUrl').val(),
        $('#' + guid + 'mainPage').val(),
        $('#' + guid + 'MultiSelect').val(),
        $('#' + guid + 'Popup').val(),
        $('#' + guid + 'Mobile').val(),
        $('#' + guid + 'AllowEdit').val(),
        $('#' + guid + 'AddItemUrl').val(),
        $('#' + guid + 'EditItemUrl').val(),
        $('#' + guid + 'DuplicateItemUrl').val(),
        $('#' + guid + 'InsertItemUrl').val(),
        $('#' + guid + 'DuplicationMethod').val(),
        $('#' + guid + 'DuplicateMessage').val(),
        $('#' + guid + 'PageSize').val(),
        $('#' + guid + 'DisplayType').val(),
        $('#' + guid + 'PromoteButton').val(),
        $('#' + guid + 'addTitle').val(),
        $('#' + guid + 'dupTitle').val(),
        $('#' + guid + 'ShowUpDown').val(),
        $('#' + guid + 'EditOnlyUrl').val(),
        $('#' + guid + 'RowColorColumnName').val(),
        $('#' + guid + 'HasOpenRules').val(),
        $('#' + guid + 'MaxSubGridHeight').val(),
        $('#' + guid + 'AllowCreate').val(),
        $('#' + guid + 'WorkFlowStepsFieldName').val(),
        $('#' + guid + 'showDisabledSteps').val(),
        $('#' + guid + 'ReloadPage').val(),
        $('#' + guid + 'GetAllFilterValuesUrl').val(),
        $('#' + guid + 'DataDisplayType').val(),
        $('#' + guid + 'GridDisplayType').val(),
        $('#' + guid + 'InsideTextSearch').val(),
        $('#' + guid + 'Role').val(),
         $('#' + guid + 'OpenDialogMax').val(),
         $('#' + guid + 'SqlProduct').val()
        );
}

function disableChildrenTabs(tab) {
    var arr = new Array();

    tab.find('li[haschildren="haschildren"]').each(function () {
        arr.push(parseInt($(this).attr("index")));
    });
    tab.tabs("option", "disabled", arr);
}

var accordionPk = null;
function accordionChanged(ui, guid, pk) {
    var header = $(ui.newHeader[0]);
    var content = $(ui.newContent[0]);

    if (pk == null)
        return;

    if (pk[pk.length - 1] == '#')
        pk = pk.slice(0, -1);

    accordionPk = pk;
    var pkSplit = pk.split(',');
    if (header.attr('haschildren') == 'haschildren') {
        var index = parseInt(header.attr('index'));
        var url = unescape(header.attr('url'));
        var urlSplit = url.split('$$');

        var newUrl = '';

        for (var i = 0, len = pkSplit.length; i < len; ++i) {
            newUrl = newUrl + urlSplit[i] + pkSplit[i];
        }

        newUrl = newUrl + urlSplit[urlSplit.length - 1] + "&children=true";

        //        Durados.Tabs.url($(this), newUrl);
        if (true) {
            var backUrl = window.location.href;
            var newUrl2 = "/Home/IndexWithButtons?url=" + encodeURIComponent(newUrl + "&menu=off") + "&backUrl=" + encodeURIComponent(backUrl);

            //
            var scope = getDocumentScope();
            $('iframe[name=viewProp]', scope).hide();
            $('#loadSettings div:first', scope).text('Loading Columns Settings...');
            $('#loadSettings', scope).show();

            window.location = newUrl2;


        }
        else {
            content.html(FloatingDialog.Create(newUrl));

            var grid = $(ui.newContent[0]).find('div[ajaxDiv="ajaxDiv"]')
            if (grid.length == 0)
                return;

            if (grid.attr("isLoaded") == "isLoaded")
                return;

            var newGuid = grid.attr('guid');
            showProgress();
            initDataTableView(newGuid);
            //views[newGuid].setTab($(ui.tab));
            views[newGuid].updateCounter();
            grid.attr("isLoaded", "isLoaded");
        }
    }


}

function InitAccordion(guid, tabWidth, pk) {
    var createAccordion = $('#' + guid + 'CreateAccordion');
    var editAccordion = $('#' + guid + 'EditAccordion');

    if (createAccordion.length == 0)
        createAccordion = $('#nullInlineAddingAccordion');

    if (editAccordion.length == 0)
        editAccordion = $('#nullInlineEditingAccordion');

    //    if (createAccordion.length == 0)
    //        createAccordion = $('#' + guid + 'InlineAddingAccordion');

    //    if (editAccordion.length == 0)
    //        editAccordion = $('#' + guid + 'InlineEditingAccordion');

    var cache = true;
    if (views[guid])
        cache = views[guid].TabCache == 'True';

    if (plugIn()) {
        hideFields(editAccordion);
    }

    createAccordion.accordion({ collapsible: true, autoHeight: false, changestart: function (event, ui) { accordionChanged(ui, guid, null); } });  // first tab selected
    editAccordion.accordion({ collapsible: true, autoHeight: false, changestart: function (event, ui) { accordionChanged(ui, guid, pk != null ? pk : Multi.GetSingleSelection(guid)); } }); // first tab selected
    return createAccordion.length > 0 || editAccordion.length > 0;
}

function hideFields(editAccordion) {
    var h3 = editAccordion.find('h3[haschildren="haschildren"]');
    var div = h3.next();
    div.remove();
    div.remove(); h3.remove();
}


function InitTabs(guid, tabWidth) {
    var createTab = $('#' + guid + 'CreateTabs');
    var editTab = $('#' + guid + 'EditTabs');
    var editTabClone = $('#' + guid + 'EditTabs_clone');

    if (createTab.length == 0)
        createTab = $('#nullInlineAddingTabs');

    if (editTab.length == 0)
        editTab = $('#nullInlineEditingTabs');

//    if (editTabClone.length == 0)
//        editTabClone = $('#nullInlineEditingTabs_clone');

    //createTab.css("width", tabWidth + 'px');
    //editTab.css("width", tabWidth + 'px');
    var cache = 'True';

    if (views[guid])
        cache = views[guid].TabCache == 'True';

    createTab.tabs({ fx: { opacity: 'toggle', duration: 'fast' }, cache: cache, ajaxOptions: { cache: false, async: false} }); // first tab selected
    editTab.tabs({ fx: { opacity: 'toggle', duration: 'fast' }, cache: cache, ajaxOptions: { cache: false, async: false} }); // first tab selected
    if (editTabClone.length > 0)
        editTabClone.tabs({ fx: { opacity: 'toggle', duration: 'fast' }, cache: cache, ajaxOptions: { cache: false, async: false} }); // first tab selected
    //    createTab.tabs(); // first tab selected
    //    editTab.tabs(); // first tab selected
    if (createTab.length > 0) {
        createTab.bind("tabsselect", function (event, ui) {
            if ($(ui.tab).attr('nocache') == 'nocache') {
                createTab.tabs("load", ui.index);
            }
            else {
                createTab.find('input[upload="upload"]').each(function () {
                    //showUpload($(this).attr('name'), editPrefix, field.Value, $('#' + guid + prefix + 'upload_img_' + field.Name).attr('UploadPath') + field.Value, guid);
                    $(this).val($(this).attr('d_file'));
                });
            }
            tabSelected(ui, "create", guid);
        });
        disableChildrenTabs(createTab);
    }

    if (editTab.length > 0) {
        editTab.bind("tabsselect", function (event, ui) {
            if ($(ui.tab).attr('nocache') == 'nocache') {
                editTab.tabs("load", ui.index);
            }
            else {
                editTab.find('input[upload="upload"]').each(function () {
                    //showUpload($(this).attr('name'), editPrefix, field.Value, $('#' + guid + prefix + 'upload_img_' + field.Name).attr('UploadPath') + field.Value, guid);
                    $(this).val($(this).attr('d_file'));
                });
            }
            tabSelected(ui, "edit", guid);

        });
    }

    if (editTabClone.length > 0) {
        editTabClone.bind("tabsselect", function (event, ui) {
            if ($(ui.tab).attr('nocache') == 'nocache') {
                editTabClone.tabs("load", ui.index);
            }
            else {
                editTabClone.find('input[upload="upload"]').each(function () {
                    //showUpload($(this).attr('name'), editPrefix, field.Value, $('#' + guid + prefix + 'upload_img_' + field.Name).attr('UploadPath') + field.Value, guid);
                    $(this).val($(this).attr('d_file'));
                });
            }
            tabSelected(ui, "edit", guid);

        });
    }

    //    createTab.bind("tabsload", function(event, ui) {
    //        tabLoaded(ui);
    //    });
    //    editTab.bind("tabsload", function(event, ui) {
    //        tabLoaded(ui);
    //    });

    var r = false;
    if (createTab.length > 0) {
        createTab.bind("tabsshow", function (event, ui) {
            tabLoaded(ui);
        });
        r = true;
    }
    if (editTab.length > 0) {
        editTab.bind("tabsshow", function (event, ui) {
            tabLoaded(ui);
        });
        r = true;
    }

    if (editTabClone.length > 0) {
        editTabClone.bind("tabsshow", function (event, ui) {
            tabLoaded(ui);
        });
        r = true;
    }

    return r;
}

var dialogExt; if (!dialogExt) dialogExt = {};

dialogExt.changeState = function (dialog, button, guid) {
    var state = button.attr('d_state');
    if (state == 'max' || IE7) {
        dialogExt.resize(dialog, button, guid);
        state = "dialog";
    }
    else {
        dialogExt.max(dialog, button, guid);
        state = "max";
    }

    $.cookie("state_" + currentViewName, state, { expires: 1000 });

}

dialogExt.resizeCurrent = function () {
    resizing = true;
    if (currentDialog == null)
        return;

    var isItemDialog = currentDialog.prop('item');
    if (dialogExt.getState(currentDialog) != 'max' && !isItemDialog)
        return;
    var guid = currentDialog.attr('guid');
    if (guid == null)
        guid = currentDialog.siblings().closest('div [hasguid="hasguid"]').attr('guid');
    if (guid == null)
        return;
    var winWidth = $(window).width();
    var winHeight = $(window).height();
    var topPosition = 0;

    if (isItemDialog) {
        try {
            if (!isMobile()) {
                topPosition = $(".menu-container + div").position().top + 13;
            }
        }
        catch (err) {
        }
        winHeight -= topPosition;
    }
    else {
        winHeight -= 22;
        winWidth -= 6;
    }

    currentDialog.dialog("option", "height", winHeight);
    currentDialog.dialog("option", "width", winWidth);
    currentDialog.dialog("option", "position", [0, topPosition]);

    dialogExt.resizeTabs(currentDialog, guid);
    resizing = false;

}

var resizing = false;

dialogExt.max = function (dialog, button, guid) {
    resizing = true;
    if (button == null) {
        button = dialog.siblings(".ui-dialog-titlebar").find("a.ui-dialog-titlebar-max");
    }

    button.attr('d_state', 'max');

    var position = dialog.dialog("option", "position");
    try {
        if (position.length == 2) {
            if (position[0] == 0 && position[1] == 0) {
                position = "center";
            }
            else {
                position2 = new Array();
                position2[0] = position[0];
                position2[1] = position[1];
                position = position2;
            }
        }
        else {
            position = "center";
        }
    }
    catch (err) {
        position = "center";
    }
    var height = dialog.dialog("option", "height");
    var width = dialog.dialog("option", "width");

    var winWidth = $(window).width();
    var winHeight = $(window).height();

    var rec = Rectangle.Load(currentViewName);

    if (width > winWidth * 0.75) {
        if (rec == null || rec.width > winWidth * 0.75) {
            width = winWidth * 0.75;
        }
        else {
            width = rec.width;
        }
    }
    if (height > winHeight * 0.75) {
        if (rec == null || rec.height > winHeight * 0.75) {
            height = winHeight * 0.75;
        }
        else {
            height = rec.height;
        }
    }
    var rect = { position: position, height: height, width: width };

    var src = Sys.Serialization.JavaScriptSerializer.serialize(rect);

    button.attr('d_rect', src);


    $('body').css('overflow', 'hidden');

    //setTimeout(function() {
    var winWidth = $(window).width();
    var winHeight = $(window).height();

    dialog.dialog("option", "height", winHeight - 22);
    dialog.dialog("option", "width", winWidth - 6);
    dialog.dialog("option", "position", [0, 0]);
    try {
        dialog.dialog("option", "draggable", false);
        dialog.dialog("option", "resizable", false);
    }
    catch (err) {
    }
    button.children('span').removeClass('ui-icon-maxthick');
    button.children('span').addClass('ui-icon-resizethick');
    button.attr('title', translator.restore);

    dialogExt.resizeTabs(dialog, guid);

    dialog.trigger('dialogresize');

    resizing = false;


    //}, 1);
}

dialogExt.resizeTabs = function (dialog, guid) {
    var width = dialog.width();
    //var height = dialog.height();

    if (guid != null) {
        //$('#' + guid + 'CreateTabs').css("min-width", (width - 15) + 'px');
        //$('#' + guid + 'EditTabs').css("min-width", (width - 15) + 'px');
    }

}

dialogExt.getState = function (dialog) {
    var button = dialog.siblings(".ui-dialog-titlebar").find("a.ui-dialog-titlebar-max");
    if (button == null)
        return "dialog";
    return button.attr('d_state');
}

dialogExt.resize = function (dialog, button, guid) {

    button.attr('d_state', 'dialog');

    var src = button.attr('d_rect');

    var winWidth = $(window).width();
    var winHeight = $(window).height();
    //$('body').css('overflow', 'auto');

    if (src != null && src != '') {

        var rect = Sys.Serialization.JavaScriptSerializer.deserialize(src);
        var width = rect.width;
        var height = rect.height;

        if (width > winWidth * 0.75) {
            width = winWidth * 0.75;
        }
        if (height > winHeight * 0.75) {
            height = winHeight * 0.75;
        }

        dialog.dialog("option", "height", height);
        dialog.dialog("option", "width", width);
        dialog.dialog("option", "position", rect.position);
    }
    else {
        dialog.dialog("option", "height", winHeight * 0.75);
        dialog.dialog("option", "width", winWidth * 0.75);
        dialog.dialog("option", "position", 'center');
    }
    dialog.dialog("option", "draggable", true);
    dialog.dialog("option", "resizable", true);

    dialogExt.resizeTabs(dialog, guid);

    button.children('span').removeClass('ui-icon-resizethick');
    button.children('span').addClass('ui-icon-maxthick');
    button.attr('title', translator.maximize);

    dialog.trigger('dialogresize');

    Durados.GridHandler.adjustDataTableHeight(guid);
}

dialogExt.initMaxButton = function (dialog, guid) {
    var titlebar = dialog.siblings(".ui-dialog-titlebar");
    var closeButton = titlebar.find("a.ui-dialog-titlebar-close");
    var maxButton = closeButton.clone(false);
    closeButton.attr('title', translator.close);
    maxButton.attr('title', translator.maximize);
    var div = closeButton.parent("div:first");
    maxButton.removeClass('ui-dialog-titlebar-close');
    maxButton.addClass('ui-dialog-titlebar-max');
    maxButton.children('span').removeClass('ui-icon-closethick');
    maxButton.children('span').addClass('ui-icon-maxthick');
    if (!isDock(guid)) {
        titlebar.dblclick(function () {
            dialogExt.changeState(dialog, maxButton, guid);

        });
    }

    maxButton.click(function () {

        dialogExt.changeState(dialog, maxButton, guid);

        if (window.parent && window.location.href.indexOf('Admin/Item/View?') > -1) {
            window.parent.viewProperties(window.parent.getMainPageGuid(), true, window.parent.dock.width, window.parent.dock.url, window.parent.dock.viewDisplayName);
            window.parent.closeViewProperties();
        }
    })
    .hover(function () {
        maxButton.addClass('ui-state-hover');
    },
	    function () {
	        maxButton.removeClass('ui-state-hover');
	    })
    .focus(function () {
        maxButton.addClass('ui-state-focus');
    })
	.blur(function () {
	    maxButton.removeClass('ui-state-focus');
	});

    div.append(maxButton);
}

function getMainViewName() {
    return views[getMainPageGuid()].ViewName;
}

dialogExt.iniDockButton = function (dialog, guid) {
    dialogExt.initDialogTitleButton(dialog, guid, 'dock', function () {
        viewProperties(getMainPageGuid(), false, dock.width, dock.url, dock.viewDisplayName);
        dialog.dialog('close');

    }, 'ui-dialog-titlebar-dock', 'ui-icon-dockthick');
}

dialogExt.initDialogTitleButton = function (dialog, guid, title, callback, buttonClass, imageClass) {
    var titlebar = dialog.siblings(".ui-dialog-titlebar");
    var closeButton = titlebar.find("a.ui-dialog-titlebar-close");
    var newButton = closeButton.clone(false);
    newButton.attr('title', title);
    var div = closeButton.parent("div:first");
    newButton.removeClass('ui-dialog-titlebar-close');
    newButton.addClass(buttonClass);
    newButton.children('span').removeClass('ui-icon-closethick');
    newButton.children('span').addClass(imageClass);
    newButton.click(callback)
    .hover(function () {
        newButton.addClass('ui-state-hover');
    },
	    function () {
	        newButton.removeClass('ui-state-hover');
	    })
    .focus(function () {
        newButton.addClass('ui-state-focus');
    })
	.blur(function () {
	    newButton.removeClass('ui-state-focus');
	});

    div.append(newButton);
}

function handleToolbar(guid) {
    if (!isDockFields(guid)) {
        return;
    }

    var toolbar = $('#' + guid + "toolbar");
    toolbar.children('div').each(function () {
        if ($(this).attr('role') != 'crud') {
            $(this).remove();
        }
        else {
            $(this).children().each(function () {
                if ($(this).attr('name') != 'new') {
                    $(this).remove();
                }
            });
        }
    });

    $('.filter-buttons').remove();
    $('.gridPager-div').remove();
}

var dialogAutoOpened = false;

var currentDialog = null;
function initDataTableView(guid) {
    var ajaxDiv = $('div.ajaxDiv[guid="' + guid + '"]');
    if (!document.domain)
        document.domain = ajaxDiv.attr('d_domain');

    LoadView(guid);

    //handleToolbar(guid);

    Autocomplete.Init(guid);
    //InitCommandMenu();

    //initiate dialog for add & edit
    var winWidth = $(window).width();
    var winHeight = $(window).height();
    var tabWidth = winWidth * .75 - 55;

    var viewName = views[guid].gViewName;
    if (viewName == null) {
        ajaxDiv.html("");
        modalErrorMsg("View was not loaded properly.");
        return;
    }
    var rec = Rectangle.Load(viewName);

    if (views[guid].DataDisplayType == "Preview"){
        var clone = $("#" + guid + "DataRowEdit").clone(true, true);
        clone.attr('id', guid + "DataRowEdit_clone");

        clone.find('[id]').each(function () {
            $(this).attr('id', $(this).attr('id') + '_clone');
        });

        clone.find('ul>li>a').each(function () {
            $(this).attr('href', $(this).attr('href') + '_clone');
        });

        $('body').append(clone);

        clone.dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            position: 'center',
            width: (winWidth * .75),
            resizeStop: SaveDialogOnResize,
            zIndex: 10000,
            open: function (event, ui) {
                currentDialog = $("#" + guid + "DataRowEdit_clone");
                    
            },
            dragStop: SaveDialogOnDrag
        });
    }

    try {
        if (rec != null) {
            $("#" + guid + "DataRowCreate").dialog({
                bgiframe: true,
                autoOpen: false,
                modal: true,
                position: [rec.left, rec.top],
                width: rec.width,
                height: rec.height,
                resizeStop: SaveDialogOnResize,
                zIndex: 10000,
                close: function (event, ui) {
                    Durados.Tabs.select($('#' + guid + 'CreateTabs'), 0);
                    //                    $('#' + guid + 'CreateTabs').tabs("select", 0)
                },
                open: function (event, ui) {
                    var state = $.cookie("state_" + views[guid].gViewName);
                    var displayType = views[guid].DataDisplayType;

                    if (displayType != "Preview" && (state == 'max' || IE7)) {
                        dialogExt.max($("#" + guid + "DataRowCreate"), null, guid);
                    }
                    currentDialog = $("#" + guid + "DataRowCreate");
                    window.setTimeout(function () {
                        jQuery(document).unbind('mousedown.dialog-overlay').unbind('mouseup.dialog-overlay');
                    }, 1);

                },
                dragStop: SaveDialogOnDrag
            });

            $("#" + guid + "DataRowEdit").dialog({
                bgiframe: true,
                autoOpen: false,
                modal: true,
                position: [rec.left, rec.top],
                width: rec.width,
                height: rec.height,
                resizeStop: SaveDialogOnResize,
                zIndex: 10000,
                beforeClose: function (event, ui) {
                    return EditDialog.Close(guid, pk, false, false);
                },
                close: function (event, ui) {
                    Durados.Tabs.select($('#' + guid + 'EditTabs'), 0);
                },
                open: function (event, ui) {
                    var state = $.cookie("state_" + views[guid].gViewName);
                    var displayType = views[guid].DataDisplayType;

                    if (displayType != "Preview" && (state == 'max' || IE7)) {
                        dialogExt.max($("#" + guid + "DataRowEdit"), null, guid);
                    }
                    currentDialog = $("#" + guid + "DataRowEdit");
                    window.setTimeout(function () {
                        jQuery(document).unbind('mousedown.dialog-overlay').unbind('mouseup.dialog-overlay');
                    }, 1);

                },
                dragStop: SaveDialogOnDrag
            });

            tabWidth = rec.width - 55;
        }
        else {

            $("#" + guid + "DataRowCreate").dialog({
                bgiframe: true,
                autoOpen: false,
                modal: true,
                position: 'center',
                width: (winWidth * .75),
                resizeStop: SaveDialogOnResize,
                zIndex: 10000,
                beforeClose: function (event, ui) {
                    return EditDialog.Close(guid, pk, false, false);
                },
                close: function (event, ui) {
                    Durados.Tabs.select($('#' + guid + 'CreateTabs'), 0);
                },
                open: function (event, ui) {
                    var state = $.cookie("state_" + views[guid].gViewName);
                    if (state == 'max' || IE7) {
                        dialogExt.max($("#" + guid + "DataRowCreate"), null, guid);
                    }
                    currentDialog = $("#" + guid + "DataRowCreate");
                    window.setTimeout(function () {
                        jQuery(document).unbind('mousedown.dialog-overlay').unbind('mouseup.dialog-overlay');
                    }, 1);
                },
                dragStop: SaveDialogOnDrag
            });

            $("#" + guid + "DataRowEdit").dialog({
                bgiframe: true,
                autoOpen: false,
                modal: true,
                position: 'center',
                width: (winWidth * .75),
                resizeStop: SaveDialogOnResize,
                zIndex: 10000,
                close: function (event, ui) {
                    Durados.Tabs.select($('#' + guid + 'EditTabs'), 0);
                    //                    $('#' + guid + 'EditTabs').tabs("select", 0)
                },
                open: function (event, ui) {
                    var state = $.cookie("state_" + views[guid].gViewName);
                    if (state == 'max' || IE7) {
                        dialogExt.max($("#" + guid + "DataRowEdit"), null, guid);
                    }
                    currentDialog = $("#" + guid + "DataRowEdit");
                    window.setTimeout(function () {
                        jQuery(document).unbind('mousedown.dialog-overlay').unbind('mouseup.dialog-overlay');
                    }, 1);
                },
                dragStop: SaveDialogOnDrag
            });
        }

        //    var state = $.cookie("state_" + views[guid].gViewName);
        //    if (state != "max")
        //        state = "dialog";
        dialogExt.initMaxButton($("#" + guid + "DataRowEdit"), guid);
        dialogExt.initMaxButton($("#" + guid + "DataRowCreate"), guid);

        $("#rich").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            zIndex: 10000,
            position: 'center',
            height: (winHeight * .75),
            width: (winWidth * .75)
        });

        $("#richDisabled").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            zIndex: 10000,
            position: 'center',
            height: (winHeight * .75),
            width: (winWidth * .75)
        });

        $("#notRich").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            zIndex: 10000,
            position: 'center',
            height: (winHeight * .75),
            width: (winWidth * .75)
        });

        $("#UrlDialog").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            zIndex: 10000,
            position: 'center',
            height: (winHeight * .5),
            width: (winWidth * .5)
        });
    }
    catch (err) {
    }
    var isAccordion = InitAccordion(guid, tabWidth);

    if (!isAccordion) {
        //init tabs
        InitTabs(guid, tabWidth);
    }

    slider.InitSlider(null);

    var createForm = $('#' + guid + 'Create' + ReplaceNonAlphaNumeric(views[guid].jsonViewForCreate.ViewName) + 'DataRowForm');
    if (createForm.length == 1) {
        //ajaxNotSuccessMsg("<pre><code>" + htmlEntities($("#" + guid + "DataRowCreate").html()) + "</code></pre>");

        InitValidation(guid, createPrefix, views[guid].jsonViewForCreate);
    }
    else {
        //ajaxNotSuccessMsg( "<pre><code>"+htmlEntities($("#" + guid + "DataRowCreate").html())+"</code></pre>");
    }
    var editForm = $('#' + guid + 'Edit' + ReplaceNonAlphaNumeric(views[guid].jsonViewForEdit.ViewName) + 'DataRowForm');
    if (editForm.length == 1) {
        InitValidation(guid, editPrefix, views[guid].jsonViewForEdit);
    }
    initDerivation(views[guid].jsonViewForCreate.Derivation, $("#" + guid + "DataRowCreate"), createPrefix, guid, views[guid].jsonViewForCreate.ViewName);
    initDerivation(views[guid].jsonViewForEdit.Derivation, $("#" + guid + "DataRowEdit"), editPrefix, guid, views[guid].jsonViewForEdit.ViewName);

    try {
        //initiate dialog for delete
        $("#DeleteMessage").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            width: 500,
            height: 200,
            zIndex: 10000,
            position: 'center'
        });

        //initiate dialog for delete
        $("#DeleteSelectionMessage").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            width: 500,
            height: 200,
            zIndex: 10000,
            position: 'center'
        });
    }
    catch (err) {
    }

    $("select[outsideDependency='outsideDependency']").change(function () {

        var htmlField = $(this);

        var prefix = GetPrefix(htmlField, guid);

        var dependentFieldName = htmlField.attr('dependentFieldName');
        var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
        var select = $('#' + guid + prefix + ReplaceNonAlphaNumeric(dependentFieldName));
        Dependency.ClearAll(select, guid);
        var fk = htmlField.val();
        if (fk == '') {
            Dependency.Clear(select);
        }
        else {
            Dependency.Load(dependentFieldViewName, dependentFieldName, fk, select, guid);
        }

    });

    //    $("select[insideDependency='insideDependency']").change(function() {

    //        var htmlField = $(this);

    //        var prefix = GetPrefix(htmlField, guid);
    //        var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
    //        var dependentFieldNames = htmlField.attr('dependentFieldNames').split(';');

    //        Dependency.LoadInside(dependentFieldViewName, dependentFieldNames, prefix, this, guid);
    //    });

    InitContextMenu(guid);

    InitChangeFieldOrder(guid);

    if ($.browser.msie)
        $("input[type='checkbox']").click(function () { $(this).trigger('change'); }); //$("input[type='checkbox']").trigger('click').trigger('change');

    //    if ($.browser.msie)
    //        $("input[type='checkbox']").mousedown(function () { $(this).trigger('change'); });

    success(guid);

    $(Durados.View).trigger('initDataTableView', { guid: guid });

    var popup = views[guid].Popup;
    var pk = queryString('pk');

    if (!dialogAutoOpened && popup && (pk && hasRows(guid) || (openSingleRow(guid)))) {
        if (!pk) {
            pk = Multi.getFirstRowPK(guid);
        }
        $.cookie("state_" + views[guid].ViewName, 'max', { expires: 1000 });
        EditDialog.Open(pk, guid, false);
        dialogAutoOpened = true;
    }

    var AddNewdialog = queryString('Add');
    AddNewdialog = AddNewdialog && (AddNewdialog == 'newdialog' || AddNewdialog == 'newdialog#');

    if (!dialogAutoOpened && popup && AddNewdialog) {
        var autoOpenAdd = { guid: guid, cancel: false };
        $(AddDialog).trigger('autoOpenAdd', autoOpenAdd);
        if (!autoOpenAdd.cancel) {
            AddDialog.Open(null, guid, false);
        }
        dialogAutoOpened = true;
    }

    if (queryString('settings') == 'true')
        rowtabletitleSpan.click();

}
function hasRows(guid) {
    return parseInt($('#' + guid + 'ajaxDiv').find('table.gridview:first').attr('rowCount')) > 0;
}
function htmlEntities(str) {
    return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

var insideDependencyChangeWorking = false;
function insideDependencyChange(select, guid) {
    if (insideDependencyChangeWorking) return false;
    insideDependencyChangeWorking = true;
    var htmlField = $(select);
    //var val = htmlField.val();

    var prefix = GetPrefix(htmlField, guid);
    var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
    var dependentFieldNames = htmlField.attr('dependentFieldNames').split(';');

    Dependency.LoadInside(dependentFieldViewName, dependentFieldNames, prefix, select, guid);

    insideDependencyChangeWorking = false;

    return true;
}

function InitChangeFieldOrder(guid) {
    $("#" + guid + 'ajaxDiv').find('select[name="d_fieldOrder"]').change(function () {
        var fieldOrder = $(this).val();
        $.ajax({
            url: gVD + 'Admin/ChangeConfigFieldOrder/',
            contentType: 'application/json; charset=utf-8',
            data: { fieldOrder: fieldOrder },
            async: false,
            dataType: 'json',
            cache: false,
            error: ajaxErrorsHandler,
            success: function (json) {
                if (json == '') {
                    $.post(views[guid].gIndexUrl,
                    {
                        SortColumn: fieldOrder, direction: 'Asc', guid: guid
                    },
                    function (html) {
                        hideProgress();
                        var index = html.indexOf("$$error$$", 0)
                        if (index < 0 || index > 1000) {
                            FilterForm.HandleSuccess(html, false, guid, null);
                            prevSortedColumn = fieldOrder;
                            prevSortedDirection = 'Asc';
                            InitChangeFieldOrder(guid);
                        }
                        else {
                            //FilterForm.ShowFailureMessage(html.replace("$$error$$", ""), guid);
                            ajaxNotSuccessMsg(html, guid);
                        }
                    });
                }
                else {
                    ajaxNotSuccessMsg(json);
                }
            }

        });
    });
}

var contextInfo = {};

var movedColumnInfo = {};

var renamedFieldInfo = {};

function InitContextMenu(guid) {
    //conext menu
    //$("#" + guid + 'ajaxDiv').destroyContextMenu();
    //debugit("#" + guid + 'ajaxDiv ');

    $("#" + guid + 'ajaxDiv').mousedown(function (e) {
        //if (isRightClick(e)) {     
        //alert('' + e.clientX + ', ' + e.clientY);
        var context = document.elementFromPoint(e.clientX, e.clientY);
        var targetElement = $(e.target);
        var isCheckClicked = targetElement.attr('class') == 'Multi';
        var isExpandClicked = targetElement.attr('class') == "inlineAddingImg expand";

        var pk = $(context).parents('tr').attr('d_pk');

        //        if (e.button != 2) {
        //            $("#" + guid + 'ajaxDiv').disableContextMenu();
        //            e.stopPropagation();
        //            return;
        //        }

        if (isCheckClicked || isExpandClicked) {
            $("#" + guid + 'ajaxDiv').disableContextMenu();
            cellsSelection.clear();
        }
        else {
            if (pk) {
                var el = Multi.GetElementByPK(guid, pk);
                if (el != null) {
                    if ((!e.ctrlKey || !views[guid].MultiSelect) && !e.shiftKey && !Durados.CheckBox.IsChecked($(el))) {
                        //if (!(e.which == 2 && $(context).parents('td').first().hasClass('selectedCell'))) {
                        //alert($(context).parents('td').first()[0].className);
                        Multi.Clear(guid);
                        //}
                    }
                    //$(el).attr('checked', 'checked');
                    Durados.CheckBox.SetChecked($(el), 'checked');
                    //debugit($(el).attr('checked'));
                    Multi.Select(guid, views[guid].MultiSelect, true, e.ctrlKey, el);
                }
                if (isRightClick(e)) {
                    $('#myMenu' + guid).find('li').show();
                    $('#myMenu' + guid).find('li[d_admin="d_admin"]').hide();
                    $("#" + guid + 'ajaxDiv').enableContextMenu();
                }
            } else {
                var fieldName;
                if (!targetElement.length || targetElement[0].tagName != 'TH') {
                    var th = targetElement.parents('TH').first();
                    if (th.length == 1) {
                        targetElement = th;
                        fieldName = targetElement.attr('SortColumn');
                    }
                } else {
                    fieldName = targetElement.attr('SortColumn');
                }

                //var viewName = views[guid].ViewName;
                if (targetElement.length && fieldName) {
                    contextInfo[guid] = { fieldName: fieldName, targetElement: targetElement };
                    if ($(targetElement.context).attr('class') == 'desc-icon') {
                        //if (isRightClick(e) || $(targetElement.context).attr('class') == 'desc-icon') {
                        $('#myMenu' + guid).find('li').hide();
                        var list = $('#myMenu' + guid).find('li[d_admin="d_admin"]');
                        if (list.length > 0 && isLeftClick(e)) {
                            list.show();
                            $("#" + guid + 'ajaxDiv').enableContextMenu();
                            $("#" + guid + 'ajaxDiv').enableContextMenuLeftClick();
                        } else {
                            $("#" + guid + 'ajaxDiv').disableContextMenu();
                        }
                    } else {
                        $("#" + guid + 'ajaxDiv').disableContextMenu();
                        if (movedColumnInfo[guid]) {
                            var before = movedColumnInfo[guid].placement == 'before';
                            var fnFrom = movedColumnInfo[guid].elementToMove.attr('SortColumn');
                            moveFieldTo(views[guid].ViewName, fnFrom, before, guid, fieldName, rootPath + "Admin/MoveField2/" + views[guid].ViewName);
                            Durados.GridHandler.moveColumn(guid, contextInfo[guid].targetElement);
                            Durados.GridHandler.resetHeadersMoveToClass();
                        }
                    }
                } else {
                    $("#" + guid + 'ajaxDiv').disableContextMenu();
                }
            }
        }
        //}
    });


    $("#" + guid + 'ajaxDiv').contextMenu({
        menu: 'myMenu' + guid
    },
	function (action, el, pos) {
	    var elpos = $(el).offset(); //position();
	    var context = document.elementFromPoint(elpos.left, elpos.top); //(elpos.left + pos.x, elpos.top + pos.y)
	    d_doAction(guid, action);

	    //	    	    alert(
	    //	    		'Action: ' + action + '\n\n' +
	    //	    		'Element ID: ' + $(el).attr('id') + '\n\n' +
	    //	    		'X: ' + pos.x + '  Y: ' + pos.y + ' (relative to element)\n\n' +
	    //	    		'X: ' + pos.docX + '  Y: ' + pos.docY + ' (relative to document)' +
	    //	    		' ' + $(context).parents('tr').attr('d_pk')
	    //	    		);
	});

}
function LoadCheckListFilter(select, guid) {
    showProgress();
    select.attr('d_loaded', 'd_loaded');
    Dependency.Load(views[guid].gViewName, select.attr('name'), '', select, guid);

    var td = select.parents('td:first');

    //Init width
    //Changed by br
    var isGroupFilter = select.attr("isGroupFilter");
    if (isGroupFilter == 'True') {
        width = select.width();
    }
    else {
        width = td.width() - 4;
    }

    //changed by br//
    //    td.find("span[d_ph='ph']").remove();
    select.siblings('.ui-dropdownchecklist').remove();

    initDropdownchecklist(select, guid, width);

    setTimeout(function () {
        select.dropdownchecklist("drop");
    }, 1);
    hideProgress();
}

function initDropdownchecklistFilter(select, guid) {
    if (select.find('option').length > 1 || select.attr('d_loaded') == 'd_loaded') {
        initDropdownchecklist(select, guid);
        return;
    }
    //var td = select.parents('td:first');
    //var width = td.width() - 6;
    //select.siblings('span').first().find('span.ui-dropdownchecklist-text').css("width", width+"px");    

    /*
    var wrapper = $("<span/>");
    wrapper.addClass("ui-dropdownchecklist ui-dropdownchecklist-selector-wrapper ui-widget");
    wrapper.css({ display: "inline-block", cursor: "default", overflow: "hidden" });
    wrapper.attr("d_ph","ph");

    var control = $("<span/>");
    control.addClass("ui-dropdownchecklist-selector ui-state-default");
    control.css({ display: "inline-block", overflow: "hidden", 'white-space': 'nowrap' });
    // Setting a tab index means we are interested in the tab sequence
    var tabIndex = select.attr("tabIndex");
    if (tabIndex == null) {
    tabIndex = 0;
    } else {
    tabIndex = parseInt(tabIndex);
    if (tabIndex < 0) {
    tabIndex = 0;
    }
    }
    control.attr("tabIndex", tabIndex);


    var iconPlacement = "left";
    var anIcon = $("<div/>");
    anIcon.addClass("ui-icon");
    anIcon.addClass("ui-icon-triangle-1-e");
    anIcon.css({ 'float': iconPlacement });
    control.append(anIcon);
    anIcon.click(function() {
    LoadCheckListFilter(select, guid);
    });

    wrapper.append(control);


    // the text container keeps the control text that is built from the selected (checked) items
    // inline-block needed to prevent long text from wrapping to next line when icon is active
    var textContainer = $("<span></span>");
    textContainer.addClass("ui-dropdownchecklist-text");
    textContainer.css({ display: "inline-block", 'white-space': "nowrap", overflow: "hidden", width: width });
    
    control.append(textContainer);

    // add the hover styles to the control
    wrapper.hover(
    function() {
    if (!self.disabled) {
    control.addClass("ui-state-hover");
    }
    }
    , function() {
    if (!self.disabled) {
    control.removeClass("ui-state-hover");
    }
    }
    );
    // clicking on the control toggles the drop container
    wrapper.click(function(event) {
    if (!self.disabled) {
    LoadCheckListFilter(select, guid);
    }
    });

    wrapper.insertAfter(select);*/

}

function showEmptyDropdownchecklist(select) {
    try {
        select.dropdownchecklist("destroy");
    }
    catch (err) { }

    select.dropdownchecklist({ icon: {}, minWidth: 80 });
}

function initDropdownchecklist(select, guid, width) {
    var wrapper = null;
    try {
        select.dropdownchecklist("destroy");
    }
    catch (err) { }

    if (select.attr('role') != 'childrenCheckList' && select.attr('id').indexOf("filter") != -1) {
        select.parents('td:first').keyup(function (e) {
            if (e.keyCode == $.ui.keyCode.ENTER) {
                FilterForm.Apply(false, guid, null);
                return false;
            }
        });
    }

    if (((select.closest('form').length == 0 || select.closest('form').attr('viewName') != select.attr('viewName')) && select.siblings('.ui-dropdownchecklist').length == 0) || select.attr('role') == 'childrenCheckList' || select.attr('role') == 'childrenCheckListFilter') {

        var minWidth = 52;
        var d_minWidth = select.attr("d_minWidth");
        if (d_minWidth != '') {
            d_minWidth = parseInt(d_minWidth);
            if (d_minWidth > 0) {
                minWidth = d_minWidth;
            }
            else {
                if (isInDialog(select)) {
                    minWidth = 80;
                }
                else {
                    minWidth = 52;
                }
            }
        }

        var filter = select.attr('filter') == 'filter';
        if (filter) {
            if (!width) {
                width = select.parents('td:first').width();
            }
            minWidth = 10;

            select.bind("drop", function (e, data) {
                if (select.find('option').length <= 1 && select.attr('d_loaded') != 'd_loaded') {
                    LoadCheckListFilter(select, guid);
                    select.removeAttr('d_loaded');
                }
            });
        }
        else if (!width) {
            width = select.width();
            if (minWidth > width)
                width = minWidth;
        }

        var ddplacement = {}
        if ($('body').attr('dir') == 'rtl') {
            ddplacement.placement = 'right';
            ddplacement.toOpen = 'ui-icon-triangle-1-w';
        }

        var selectId = select.attr('id');
        //var l = select.find('option[selected=selected]').length;
        if (select != null && select.length > 0 && select.val() != null)
            lastSelection[selectId] = select.val().join(',');
        else
            lastSelection[selectId] = '';

        wrapper = select.dropdownchecklist({ icon: ddplacement,
            toOpen: 'ui-icon ui-icon-triangle-1-e',
            toClose: 'ui-icon ui-icon-triangle-1-s',
            width: width, maxDropHeight: 450, minWidth: minWidth, firstItemChecksAll: true, forceMultiple: true, onComplete: function (selector) {
                //var values = "";

                //                var l = 0;
                var selectionCurrValues = '';
                var selectionCurrValuesArray = [];

                $(this).find('option').removeAttr("selected");
                for (i = 0; i < selector.options.length; i++) {
                    if (selector.options[i].value == "") {
                    }
                    if (selector.options[i].selected && (selector.options[i].value != "")) {
                        //                if (values != "") values += ",";
                        //                    values += selector.options[i].value;
                    }
                    $(this).find('option[value="' + selector.options[i].value + '"').attr('selected', 'selected');
                    if (i > 0 && selector.options[i].selected) {
                        selectionCurrValuesArray.push(selector.options[i].value);
                        //l++;
                    }
                }

                selectionCurrValues = selectionCurrValuesArray.join(',');

                this.sourceSelect.trigger("blur", { select: this }); //selector

                if (lastSelection[selectId] != selectionCurrValues) {
                    if (select.attr('role') != 'childrenCheckList' && select.attr('id').indexOf("filter") != -1) {
                        FilterForm.Apply(false, guid, null);
                    }
                }
                lastSelection[selectId] = selectionCurrValues;

                //alert(values);
            }
            , textFormatFunction: function (options) {
                //                var selectedOptions = options.filter(":selected");
                //                var countOfSelected = selectedOptions.size();
                //                var size = options.size();
                //                switch (countOfSelected) {
                //                    case 0:
                //                        if (select.attr('role') == 'childrenCheckList')
                //                            return "";
                //                        else
                //                            return "(All)";
                //                    case size:
                //                        if (select.attr('role') != 'childrenCheckList')
                //                            select.find('option').removeAttr('selected');
                //                        return "(All)";
                //                    default: return getDeimetedOptions(options); 
                //}
                return getDeimetedOptions(options);
            }
        });

        if (select.is(':disabled')) {
            select.dropdownchecklist("disable");
        }
    }
    else {
        select.removeAttr('multiple');
        select.css('display', 'block');
    }

    return wrapper;
}

var lastSelection = [];

function initDropdownChecklistsCreate(guid) {
    $("#" + guid + "DataRowCreate").find("select.dropdownchecklist").each(function () {
        if ($(this).attr('filter') == 'filter') {
            //initDropdownchecklistFilter($(this), guid);
        } else if ($(this).attr('hasInsideTrigger') == 'hasInsideTrigger') {
            showEmptyDropdownchecklist($(this));
        } else {
            initDropdownchecklist($(this), guid);
        }
    });
}
function initDropdownChecklistsEdit(guid, dialog) {
    if (!dialog)
        dialog = $("#" + guid + "DataRowEdit");
    dialog.find("select.dropdownchecklist").each(function () {
        if ($(this).attr('filter') == 'filter') {
            //initDropdownchecklistFilter($(this), guid);
        } else if ($(this).attr('hasInsideTrigger') == 'hasInsideTrigger') {
            showEmptyDropdownchecklist($(this));
        } else {
            initDropdownchecklist($(this), guid);
        }
    });
}

function getDeimetedOptions(options) {
    var values = "";
    for (i = 0; i < options.length; i++) {
        if (options[i].selected && (options[i].value != "")) {
            if (values != "") values += ",";
            values += options[i].text;
        }
    }
    return values;
}


var slider; if (!slider) slider = {};
slider.InitSlider = function ($div, items) {
    if ($div == null)
        $div = $(document);

    if (!items)
        items = 3;

    var playRtl = $('body').attr('dir') == 'rtl';

    $div.find('.ui-slider').anythingSlider({
        //        resizeContents: false,      // If true, solitary images/objects in the panel will expand to fit the viewport
        //        showMultiple: 3,
        //        buildArrows: true,      // If true, builds the forwards and backwards buttons
        //        buildNavigation: false,      // If true, builds a list of anchor links to link to each panel
        //        buildStartStop: false,      // If true, builds the start/stop button and adds slideshow functionality
        //        autoPlay: false
        theme: "default", // Theme name
        mode: "horiz",   // Set mode to "horizontal", "vertical" or "fade" (only first letter needed); replaces vertical option 
        expand: false,     // If true, the entire slider will expand to fit the parent element
        resizeContents: false,      // If true, solitary images/objects in the panel will expand to fit the viewport
        showMultiple: items,     // Set this value to a number and it will show that many slides at once
        easing: "swing",   // Anything other than "linear" or "swing" requires the easing plugin or jQuery UI

        buildArrows: true,      // If true, builds the forwards and backwards buttons
        buildNavigation: false,      // If true, builds a list of anchor links to link to each panel
        buildStartStop: false,      // If true, builds the start/stop button and adds slideshow functionality

        appendForwardTo: null,      // Append forward arrow to a HTML element (jQuery Object, selector or HTMLNode), if not null
        appendBackTo: null,      // Append back arrow to a HTML element (jQuery Object, selector or HTMLNode), if not null
        appendControlsTo: null,      // Append controls (navigation + start-stop) to a HTML element (jQuery Object, selector or HTMLNode), if not null
        appendNavigationTo: null,      // Append navigation buttons to a HTML element (jQuery Object, selector or HTMLNode), if not null
        appendStartStopTo: null,      // Append start-stop button to a HTML element (jQuery Object, selector or HTMLNode), if not null

        toggleArrows: false,     // If true, side navigation arrows will slide out on hovering & hide @ other times
        toggleControls: false,     // if true, slide in controls (navigation + play/stop button) on hover and slide change, hide @ other times

        startText: "Start",   // Start button text
        stopText: "Stop",    // Stop button text
        forwardText: "&raquo;", // Link text used to move the slider forward (hidden by CSS, replaced with arrow image)
        backText: "&laquo;", // Link text used to move the slider back (hidden by CSS, replace with arrow image)
        tooltipClass: "tooltip", // Class added to navigation & start/stop button (text copied to title if it is hidden by a negative text indent)

        // Function
        enableArrows: true,      // if false, arrows will be visible, but not clickable.
        enableNavigation: true,      // if false, navigation links will still be visible, but not clickable.
        enableStartStop: true,      // if false, the play/stop button will still be visible, but not clickable. Previously "enablePlay"
        enableKeyboard: true,      // if false, keyboard arrow keys will not work for this slider.

        // Navigation
        startPanel: 1,         // This sets the initial panel
        changeBy: 1,         // Amount to go forward or back when changing panels.
        hashTags: true,      // Should links change the hashtag in the URL?
        infiniteSlides: true,      // if false, the slider will not wrap & not clone any panels
        navigationFormatter: null,      // Details at the top of the file on this use (advanced use)
        navigationSize: false,     // Set this to the maximum number of visible navigation tabs; false to disable

        // Slideshow options
        autoPlay: false,     // If true, the slideshow will start running; replaces "startStopped" option
        autoPlayLocked: false,     // If true, user changing slides will not stop the slideshow
        autoPlayDelayed: false,     // If true, starting a slideshow will delay advancing slides; if false, the slider will immediately advance to the next slide when slideshow starts
        pauseOnHover: false,      // If true & the slideshow is active, the slideshow will pause on hover
        stopAtEnd: false,     // If true & the slideshow is active, the slideshow will stop on the last page. This also stops the rewind effect when infiniteSlides is false.
        playRtl: playRtl,     // If true, the slideshow will move right-to-left

        // Times
        delay: 2000,      // How long between slideshow transitions in AutoPlay mode (in milliseconds)
        resumeDelay: 2000,     // Resume slideshow after user interaction, only if autoplayLocked is true (in milliseconds).
        animationTime: 400,       // How long the slideshow transition takes (in milliseconds)
        delayBeforeAnimate: 0,         // How long to pause slide animation before going to the desired slide (used if you want your "out" FX to show).

        isVideoPlaying: function (base) { return false; } // return true if video is playing or false if not - used by video extension

    });

}

var itemCreateDialog = null;
var itemEditDialog = null;

function InitItemDialog(guid) {

    //showProgress();
    LoadView(guid);

    Autocomplete.Init(guid);

            
    //initiate dialog for add & edit
    var dialogWidth;
    var dialogHeight;
    var position = null;
    var topPosition = 13;
    var menuDiv = null;
    var tabWidth = $(window).width() - 75;

    if (isDock(guid) || plugIn()) {
        position = [8, 118];
        dialogWidth = $(window).width() - 25;
        dialogHeight = 'auto';
    }
    else {
        
        menuDiv = $(".menu-container + div");
        var menuDivPos = [0, 0];
        if (menuDiv != null && menuDiv.length > 0) {
            try {
                menuDivPos = menuDiv.position();
            }
            catch (err) {

            }
        }

        if (menuDivPos != null && menuDivPos.top != null)
            topPosition = menuDivPos.top + 13;
        else
            topPosition = 13;

        if (isMobile())
            topPosition = 30;

        position = [0, topPosition];
        dialogWidth = $(window).width();
        dialogHeight = $(window).height() - topPosition;
    }
        
    itemCreateDialog = $("#" + guid + "DataRowCreate");
    //alert(position[0] + ',' + position[1] + ',' + dialogWidth + ',' + dialogHeight);

    


           
    if (views[guid].Mobile) {
        //alert(itemCreateDialog.length);
        var d = $('<div></div>');
        try {
            d.dialog({ autoOpen: false });
        }
        catch(err){
        }

        try {
            d.dialog('destroy');
        }
        catch (err) {
        }

        try {
            itemCreateDialog.dialog({
                //            bgiframe: true,
                autoOpen: false,
                modal: false,
                //            closeOnEscape: false,
                position: [0,65],
                width: dialogWidth,
                height: dialogHeight,
                open: function (event, ui) {
                    dialogExt.max(itemCreateDialog, null, guid);
                    currentDialog = itemCreateDialog;
                },
                stack: false,
                resizable: false
            });
        }
        catch (err) {
            
        }
        
        //itemCreateDialog.dialog('close');
    }
    else {
        itemCreateDialog.dialog({
            bgiframe: true,
            autoOpen: false,
            modal: false,
            closeOnEscape: false,
            position: position,
            width: dialogWidth,
            height: dialogHeight,
            open: function (event, ui) {
                dialogExt.max(itemCreateDialog, null, guid);
                currentDialog = itemCreateDialog;
            },
            stack: false,
            resizable: false,
            dragStop: SaveDialogOnDrag
        });
    }
    
    itemEditDialog = $("#" + guid + "DataRowEdit");

    try {
        itemEditDialog.dialog({
            bgiframe: true,
            autoOpen: false,
            modal: false,
            closeOnEscape: false,
            position: position,
            width: dialogWidth,
            height: dialogHeight,
            stack: false,
            resizable: false,
            dragStop: SaveDialogOnDrag,
            beforeClose: function (event, ui) {
                return EditDialog.Close(guid, null, false, false);
            },
            close: function (event, ui) {
                if (window.parent) {
                    setTimeout(function () {
                        window.parent.closeViewProperties();
                    }, 1);
                }
            },
            open: function (event, ui) {
                if (isDock(guid) || plugIn()) {
                    dialogExt.max(itemEditDialog, null, guid);
                }

                currentDialog = itemEditDialog;
            }
        });
    }
    catch (err) {

    }
    //alert(8);
    
    if (isDock(guid) || plugIn()) {
        dialogExt.initMaxButton(itemEditDialog, guid);
    }
    else {
        itemCreateDialog.prop('item', true);
        itemEditDialog.prop('item', true);

        Durados.Dialogs.zIndex(itemCreateDialog, 0)
        Durados.Dialogs.zIndex(itemEditDialog, 0)

        $("#" + guid + "DataRowCreate").siblings(".ui-dialog-titlebar").hide();
        $("#" + guid + "DataRowEdit").siblings(".ui-dialog-titlebar").hide();
    }

    var createTab = $('#' + guid + 'CreateTabs');
    var editTab = $('#' + guid + 'EditTabs');

    var cache = views[guid].TabCache == 'True';
    createTab.tabs({ fx: { opacity: 'toggle', duration: 'fast' }, cache: cache, ajaxOptions: { cache: false, async: false} }); // first tab selected
    editTab.tabs({ fx: { opacity: 'toggle', duration: 'fast' }, cache: cache, ajaxOptions: { cache: false, async: false} }); // first tab selected
    createTab.bind("tabsselect", function (event, ui) {
        if ($(ui.tab).attr('nocache') == 'nocache') {
            createTab.tabs("load", ui.index);
        }
    });
    //    $("#" + guid + "DataRowEdit").dialog({
    //        bgiframe: true,
    //        autoOpen: false,
    //        modal: false,
    //        position: [8, 118],
    //        width: winWidth - 25,
    //        height: winHeight - 180,
    //        resizable: false,
    //        dragStop: SaveDialogOnDrag
    //    });
    $("#" + guid + "DataRowCreate").siblings(".ui-dialog-titlebar").hide();
    $("#" + guid + "DataRowEdit").siblings(".ui-dialog-titlebar").hide();
    
    var isAccordion = InitAccordion(guid, tabWidth, queryString("pk"));
    $('#' + guid + 'EditAccordion').find('h3:not([haschildren])').append('<span class="ui-icon ui-icon-resizethick ui-accordion-header-open"></span>');
    $('#' + guid + 'EditAccordion').find('h3:[haschildren]').append('<span class="ui-icon ui-icon-triangle-1-e ui-accordion-header-open"></span>');
    
    if (!isAccordion) {
        //init tabs
        InitTabs(guid, tabWidth);
    }

    slider.InitSlider(null);

    //initiate dialog for delete
    $("#DeleteMessage").dialog({
        bgiframe: true,
        autoOpen: false,
        modal: true,
        width: 500,
        height: 200,
        //        zIndex: 10000,
        position: 'center'
    });
    
    //initiate dialog for delete
    $("#DeleteSelectionMessage").dialog({
        bgiframe: true,
        autoOpen: false,
        modal: true,
        width: 500,
        height: 200,
        //        zIndex: 10000,
        position: 'center'
    });
    
    $("select[outsideDependency='outsideDependency']").change(function () {

        var htmlField = $(this);

        var prefix = GetPrefix(htmlField, guid);

        var dependentFieldName = htmlField.attr('dependentFieldName');
        var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
        var select = $('#' + guid + prefix + ReplaceNonAlphaNumeric(dependentFieldName));
        Dependency.ClearAll(select, guid);
        var fk = htmlField.val();
        if (fk == '') {
            Dependency.Clear(select);
        }
        else {
            Dependency.Load(dependentFieldViewName, dependentFieldName, fk, select, guid);
        }

    });
    
    $("select[insideDependency='insideDependency']").change(function () {

        var htmlField = $(this);

        var prefix = GetPrefix(htmlField, guid);
        var dependentFieldViewName = htmlField.attr('dependentFieldViewName');
        var dependentFieldNames = htmlField.attr('dependentFieldNames').split(';');

        Dependency.LoadInside(dependentFieldViewName, dependentFieldNames, prefix, this, guid);
    });
    
    InitValidation(guid, createPrefix, views[guid].jsonViewForCreate);
    InitValidation(guid, editPrefix, views[guid].jsonViewForEdit);

    initDerivation(views[guid].jsonViewForCreate.Derivation, $("#" + guid + "DataRowCreate"), createPrefix, guid, views[guid].jsonViewForCreate.ViewName);
    initDerivation(views[guid].jsonViewForEdit.Derivation, $("#" + guid + "DataRowEdit"), editPrefix, guid, views[guid].jsonViewForEdit.ViewName);
    
    if ($.browser.msie)
        $("input[type='checkbox']").click(function () { $(this).trigger('change'); }); //$("input[type='checkbox']").trigger('click').trigger('change');

    if (!isMobile()) {
        success(guid);
    }

    $(Durados.View).trigger('initItemDialog', { guid: guid });
   
}

function Grid(guid) {
    this.guid = guid;
    this.parent = null;
    this.root = true;
    this.childrenList = new Array();

    this.add = function (guid, tr, button, placement, disabled) {
        var g = new Grid(guid);
        this.childrenList.push(g);
        g.tr = tr;
        g.pk = tr.attr('d_pk');
        g.parent = this;
        g.button = button;
        g.placement = placement;
        g.disabled = disabled;
        this.root = false;
        gridHash[guid] = g;
    }

}

var gridHash = new Array();

var parentGrid = null;

function addToParentGrid(currentGuid, newGuid, tr, button, placement, disabled) {
    var parent = getGrid(currentGuid);

    if (parent != null) {
        parent.add(newGuid, tr, button, placement, disabled);
    }
}

function getGrid(guid) {
    if (parentGrid == null) {
        parentGrid = new Grid(guid);
        gridHash[guid] = parentGrid;
        return parentGrid;
    }
    else {
        return gridHash[guid];
    }
}

function updateParentGrid(childGuid) {
    var child = getGrid(childGuid);

    if (child == null)
        return;

    var parent = child.parent;
    if (parent == null)
        return;

    Refresh(parent.guid);
}

function updateParent(childGuid) {
    var child = getGrid(childGuid);
    if (child == null)
        return;

    var parent = child.parent;
    if (parent == null)
        return;

    var parentGuid = parent.guid;
    if (!views[childGuid].updateParent)
        updateParent(parentGuid);


    var pk = child.pk;
    var tr = child.tr;
    if (pk == null || pk == '')
        return;

    var viewName = views[parentGuid].gViewName;
    if (viewName == null || viewName == '')
        return;


    var url = rootPath + views[childGuid].Controller + '/UpdateParent/' + viewName;

    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { pk: pk, guid: parentGuid },
        async: true,
        dataType: 'json',
        cache: false,
        error: function () { return; },
        success: function (json) {
            if (typeof (result) == 'string') {
                var index = result.indexOf("$$error$$", 0)
                var error = !(index < 0 || index > 1000);
                if (error)
                    return;
            }
            var fields = $(json);
            fields.each(function () {
                var name = this.Name;
                var value = this.Value;
                var displayValue = this.DisplayValue;
                var td = getTd(tr, name);
                updateTd(td, value, displayValue);
            });
            updateParent(parentGuid);
        }

    });
}

function getTd(tr, name) {
    return tr.find('td[d_name="' + name + '"]:first');
}

function updateTd(td, value, displayValue) {
    var val = td.attr('d_val');
    if (val != value) {
        td.attr('d_val', value);
        td.html(displayValue);
    }
}

function expand(button, exp, placement, disabled, currentGuid) {
    var state = $(button).attr('state');
    if (state == null)
        return;


    var src = $(button).attr('src');
    var innerDiv = $(button).parent('div').siblings('div');

    var pk = innerDiv.attr('pk');
    var inGrid = pk == '';

    var subGridPlacement = placement;

    var nocache = $(button).attr("nocache") == "nocache";

    if (state == 'collapsedEmpty' || exp || (state == 'collapsed' && nocache)) {
        var url = unescape(innerDiv.attr('url'));

        if (disabled == null)
            disabled = false;

        url = url.replace('$$', pk);
        showProgress();

        $.ajax({
            url: url,
            contentType: 'application/json; charset=utf-8',
            data: { children: true, disabled: disabled },
            async: true,
            dataType: 'html',
            cache: false,
            error: ajaxErrorsHandler,
            success: function (html) {

                var index = html.indexOf("$$error$$", 0)
                if (index < 0 || index > 1000) {
                    var updateParent = $(button).attr('updateParent') == 'Row';
                    var updateParentGrid = $(button).attr('updateParent') == 'Grid';

                    if (inGrid) {
                        var newGuid = expandInGrid(button, html, subGridPlacement);
                        if (newGuid != null) {
                            try {
                                initDataTableView(newGuid);
                                addToParentGrid(currentGuid, newGuid, $(button).parents('tr:first'), button, placement, disabled);
                                views[newGuid].updateParent = updateParent;
                                views[newGuid].updateParentGrid = updateParentGrid;
                            }
                            catch (err) { }
                        }
                    }
                    else {
                        innerDiv.html(html);
                    }
                    $(button).attr('state', 'expand');
                    if ($(button)[0].tagName == "IMG") {
                        $(button).attr('src', src.replace('Plus', 'Minus'));
                    }
                    else {
                        $(button).text('');
                    }
                    var ajaxDiv = innerDiv.find('div[ajaxDiv="ajaxDiv"]');
                    if (ajaxDiv.length == 1) {
                        var newGuid = ajaxDiv.attr('guid');
                        if (newGuid != null) {
                            initDataTableView(newGuid);
                            addToParentGrid(currentGuid, newGuid, $(button).parents('tr:first'), button, placement, disabled);
                            views[newGuid].updateParent = updateParent;
                            views[newGuid].updateParentGrid = updateParentGrid;
                        }
                    }
                    hideProgress();
                }
                else {
                    ajaxNotSuccessMsg(html);
                }
                hideProgress();

            }
        });
    }
    else if (state == 'collapsed') {
        innerDiv.css('display', 'block');
        $(button).attr('state', 'expand');
        $(button).attr('src', src.replace('Plus', 'Minus'));
        if (inGrid) {
            changeStateInGrid(button, state);
        }

    }
    else if (state == 'expand') {
        innerDiv.css('display', 'none');
        $(button).attr('state', 'collapsed');
        $(button).attr('src', src.replace('Minus', 'Plus'));
        if (inGrid) {
            changeStateInGrid(button, state);
        }

    }


}

function changeStateInGrid(button, state) {
    var tr = $(button).parents('tr:first');
    var nextTr = tr.next();
    var isNextTrSubGrid = nextTr.attr('d_role') == 'subGrid';
    if (isNextTrSubGrid) {
        if (state == 'expand') {
            nextTr.hide();
            tr.removeClass("expanded");
        }
        else {
            nextTr.show();
            tr.addClass("expanded");
        }
    }
    switchSelectedClass(button);
    Durados.GridHandler.adjustParentHeight(tr);
}

function switchSelectedClass(button) {
    var td = $(button).parents('td:first');
    var selectedClass = $(button).attr('selectedClass');

    if (td.hasClass(selectedClass)) {
        td.removeClass(selectedClass);
    }
    else {
        td.addClass(selectedClass);
    }
}

function expandInGrid(button, html, subGridPlacement) {
    var tr = $(button).parents('tr:first');
    resetExpandButtons(tr, button);
    var nextTr = tr.next();
    var isNextTrSubGrid = nextTr.attr('d_role') == 'subGrid';
    var table = tr.parents('table:first');
    if (isNextTrSubGrid)
        nextTr.remove();

    if (!subGridPlacement) {
        subGridPlacement = 2;
    } else if (subGridPlacement == -1) {
        var cell = $(button).parents('td').first();
        subGridPlacement = tr.children().index(cell);
        if (!subGridPlacement) subGridPlacement = 2;
    } else {
        subGridPlacement += 1;
    }

    var colCount = tr[0].cells.length; //alert(colCount);

    if (subGridPlacement > colCount) subGridPlacement = colCount - 1;

    var colSpan = colCount - subGridPlacement;

    var trHTML = $("<tr d_role='subGrid' class='trsubgrid'></tr>");

    var tablecmd = "<td class='tablecmdsubgrid' colspan='" + subGridPlacement + "'></td>";

    var cmd = $(tablecmd);
    var subgridControl = $('<div class="subgridControl" />');
    var spanClose = $('<span class="ui-icon ui-icon-closethick" />').click(function () {
        $(button).click();
    });
    var spanWin = $('<span class="ui-icon ui-icon-maxthick" />').click(function () {
        location.href = $(button).siblings('a').attr('href');
    });
    subgridControl.append(spanClose).append(spanWin);

    cmd.append(subgridControl);

    trHTML.append(cmd);

    //    for (i = 0; i < subGridPlacement; i++) {
    //        trHTML += "<td class='tablecmdsubgrid'></td>";
    //    }

    var subGridData = $("<td class='subGridData' colspan='" + colSpan + "'></td>");

    trHTML.append(subGridData);

    //trHTML.append('<td class="lastTD"><div class="lastTD">&nbsp;</div></td>');

    var subGridTr = trHTML;

    subGridTr.insertAfter(tr);

    //var subGridData = subGridTr.find('td.subGridData');

    var w = subGridData.width();

    subGridData.html(html);

    var vp = subGridTr.find('div.fixedViewPort').first();

    var ajaxDiv = subGridTr.find('div.ajaxDiv:first');

    var guid = ajaxDiv.attr('guid');

    if (vp.width() > w) {
        vp.width(w);
    } else {
        //$('#' + guid + 'ajaxDiv').css("width", (w - 64) + "px");
    }

    switchSelectedClass(button);

    tr.addClass("expanded");

    //var ajaxDiv = subGridTr.find('div[ajaxDiv="ajaxDiv"]:first');
    if (ajaxDiv.length && guid) {
        adjustSubGridUI(guid);
        return guid;
    }

    return null;

}

function getRowCount(guid) {
    return $('#' + guid + 'ajaxDiv').find('table.gridview:first').attr('rowCount');
}

function openSingleRow(guid) {
    var elm = $('#' + guid + 'ajaxDiv').find('table.gridview:first');
    var b = elm.attr('openSingleRow') == 'True';
    if (!b)
        return false;

    var rows = getRowCount(guid);
    return (rows == 1);
}


function adjustSubGridUI(guid) {
    var ajaxDiv = $('#' + guid + 'ajaxDiv');
    if (!ajaxDiv.length) return;
    var td = ajaxDiv.parents('td:first');
    var subGridTr = td.parents('tr:first');
    if (subGridTr.attr('d_role') == 'subGrid') {
        td.css({ paddingTop: 0, paddingBottom: 0, paddingRight: 0 });
        td.find('table.gridview').parent().css({ paddingBottom: 0 });
        var rowCount = $('#' + guid + 'ajaxDiv').find('table.gridview:first').attr('rowCount');
        var pageSize = $('#' + guid + 'ajaxDiv').find('table.gridview:first').attr('pageSize'); ;
        if (parseInt(rowCount) < parseInt(pageSize))
            ajaxDiv.find('.gridPager-div').hide();
    }
}

function resetExpandButtons(tr, button) {
    tr.find('img.expand').each(function () {
        var src = $(this).attr('src');
        $(this).attr('state', 'collapsedEmpty');
        $(this).attr('src', src.replace('Minus', 'Plus'));

        var selectedClass = $(this).attr('selectedClass');

        if ($(this).parents('td:first').hasClass(selectedClass))
            switchSelectedClass(this);
    });
}

var inlineValidations = [];
function InitValidation(guid, prefix, json) {
    $(json.Fields).each(function () {
        var field = this.Value;
        var element = $('#' + guid + prefix + ReplaceNonAlphaNumeric(field.Name));
        var spanId = 'the' + guid + prefix + json.ViewName + "_" + field.Name;
        if (!guid || guid == 'null') {
            inlineValidations[prefix + "," + json.ViewName + "," + field.Name] = getFieldValidation(field, spanId, element);
        }
        else {
            views[guid].validations[prefix + "," + json.ViewName + "," + field.Name] = getFieldValidation(field, spanId, element);
        }
    });

    if ($('#' + guid + 'pageSize').length > 0) {
        var pageSizeLen = $('#' + guid + 'pageSize').val().length;
        var pattern = '';
        for (var index = 0, len = pageSizeLen; index < len; ++index) {
            pattern = pattern + '0';
        }

        new Spry.Widget.ValidationTextField(guid + "currentPage", 'integer', { isRequired: true, useCharacterMasking: true });
    }
}

function isValidDate(id, spryFormat) {

    var el = $(id);

    var val = el.val();

    if (!val) {
        return true;
    }

    var format = spryFormat;
    var date = Date.parseExact(val, format);

    return date != null;
}

function checkIsValidDate(dd, mm, yy) {

    if (dd < 1 || dd > 32 || mm < 0 || mm > 11 || yy > 2900 || yy < 1900) {
        return false;
    }


    var b = new Date(yy, mm, dd);
    if ((b.getMonth() !== mm)
                || (b.getDate() !== dd)
                || (b.getFullYear() !== yy)
            ) {
        //alert(b.getMonth() + "+" + mm + "--" + b.getDate() + "+" + dd);
        return false;
    }

    return true;
}

var valid_m_names = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];


function getFieldValidation(field, containerId, element) {
    if (field.Disabled)
        return null;

    var extraParams = { isRequired: field.Required };

    var vType = field.ValidationType;

    if (!vType) { vType = 'none'; }

    //if (vType != 'none') {
    extraParams.useCharacterMasking = true;
    //}

    if (field.Type == 'Text' || field.Type == 'AutocompleteColumn') {

        if (field.ValidationType == 'date') {
            if (isMobile()) {
                extraParams.validation = function () { return true; }
            }
            else {
                var spryFormat = duradosGetSpryDateFormat(field.Format);

                extraParams.validation = function () { return isValidDate(element, spryFormat); }
            }
            vType = 'custom';
        } else if (field.ValidationType == 'custom') {
            if (!field.Format) {
                vType = 'none';
                extraParams.useCharacterMasking = false;
            } else {
                extraParams.pattern = field.Format;
                extraParams.validateOn = ['change'];
            }
        }

        if (field.ValidationType == 'integer' || field.ValidationType == 'real') {
            if (field.Min) extraParams.minValue = field.Min;
        } else if (field.Min) {
            extraParams.minChars = field.Min;
        }

        if (field.Max) {
            if (field.ValidationType == 'integer' || field.ValidationType == 'real') {
                extraParams.maxValue = field.Max;
            } else {
                extraParams.maxChars = field.Max;
            }
        }
        //extraParams.validateOn = ['change'];

        return new Spry.Widget.ValidationTextField(containerId, vType, extraParams);


    }
    else if (field.Type == 'Autocomplete') {
        return new Spry.Widget.ValidationTextField(containerId, 'custom', { validation: function () { return GetAutoCompleteValueId(element) != ''; }, isRequired: field.Required });

    }
    else if (field.Type == 'DropDown' || field.Type == 'OutsideDependency' || field.Type == "InsideDependency" || field.Type == 'InsideDependencyUniqueNames' || field.Type == 'Groups') {
        return new Spry.Widget.ValidationSelect(containerId, extraParams);
    }
    else if (field.Type == 'TextArea') {

        var rich = element.attr('rich') == 'true';

        if (!rich) {

            if (field.Min) {
                extraParams.minValue = field.Min;
            }
            if (field.Max && field.Max > 0) {
                extraParams.maxChars = field.Max;
            }
        }
        extraParams.validateOn = [];

        try {
            return new Spry.Widget.ValidationTextarea(containerId, extraParams);
        }
        catch (err) {
            return null;
        }

    } else if (field.Type == 'CheckList' && field.Required) {
        return new Spry.Widget.ValidationSelect(containerId, { isRequired: field.Required, invalidValue: "" });
    }
    return null;
}

var AddItemsDialog; if (!AddItemsDialog) AddItemsDialog = function () { };

AddItemsDialog.GetButtons = function (guid, addItemsUrl, searchGuid, dialog) {
    var buttons = {};  //initialize the object to hold my buttons

    buttons[translator.add] = function () { AddItemsDialog.Add(guid, addItemsUrl, searchGuid, dialog); }  //the function that does the save

    buttons[translator.cancel] = function () {
        dialog.dialog('close');
    }  //the function that does the save
    buttons[translator.add] = function () { AddItemsDialog.Add(guid, addItemsUrl, searchGuid, dialog); }  //the function that does the save

    return buttons;
}

AddItemsDialog.CreateAndOpen = function (viewName, viewDisplay, addItemsUrl, searchUrl, guid, anchor) {


    searchGuidPostfix++;
    searchGuid = guid + searchGuidPostfix;

    var qs = '';
    var dfVal = '';

    if (anchor) {

        var itemsFilter = $(anchor).parents('div.childrenViewer').first().attr("itemsFilter");
        if (itemsFilter && itemsFilter.indexOf(",") > 0) {

            var names = itemsFilter.split(",");
            var element = $(anchor).parents('div.ui-tabs').find('[name="' + names[0] + '"]');
            if (element.is("input")) { // autocomplete
                dfVal = GetAutoCompleteValueId(element);
            }
            else { // drop down
                dfVal = element.val();
            }
            qs = "&df=" + names[1] + "&dfval=" + dfVal;

        }
    }

    var beforeAddItemsArgs = { guid: guid, viewName: viewName, searchUrl: searchUrl, filterVal: dfVal, cancel: false };
    $(AddItemsDialog).trigger('beforeAddItems', beforeAddItemsArgs);

    if (beforeAddItemsArgs.cancel)
        return;

    var dialog = FloatingDialog.Show(beforeAddItemsArgs.searchUrl + "&guid=" + searchGuid + qs, "Add Items from " + viewDisplay, null, true);
    var buttons = AddItemsDialog.GetButtons(guid, addItemsUrl, searchGuid, dialog);
    dialog.dialog("option", "buttons", buttons);

    initDataTableView(searchGuid);

    var multiSelect = true;
    views[searchGuid].MultiSelect = multiSelect;
}

AddItemsDialog.Add = function (guid, addItemsUrl, searchGuid, dialog) {
    var pks = Multi.GetSelection(searchGuid);
    if (!pks.length) {
        ajaxNotSuccessMsg('Please select a row(s)');
        return;
    }
    showProgress();
    var parentGuid = AddItemsDialog.getParentGuid(guid);
    var parentViewName = '';
    if (parentGuid != null)
        parentViewName = views[parentGuid].ViewName;

    var fk = AddItemsDialog.getParentKey(guid);
    if (fk == null)
        fk = '';
    var viewName = views[guid].ViewName;

    var isSelectAll = AddItemsDialog.getSelectAll(searchGuid);
    if (isSelectAll == null)
        isSelectAll = false;
   
   
    var afterAddItemsArgs = { guid: guid, viewName: viewName, parentViewName: parentViewName, addItemsUrl: addItemsUrl, searchGuid: searchGuid, parentGuid: parentGuid, fk: fk, isSelectAll: isSelectAll };
    $(AddItemsDialog).trigger('afterAddItems', afterAddItemsArgs);


    $.post(afterAddItemsArgs.addItemsUrl,
    {
        viewName: afterAddItemsArgs.viewName, guid: afterAddItemsArgs.guid, pks: String(pks), parentViewName: afterAddItemsArgs.parentViewName, fk: afterAddItemsArgs.fk, isSelectAll: afterAddItemsArgs.isSelectAll, searchGuid: afterAddItemsArgs.searchGuid
    },
    function (html) {

        hideProgress();
        var index = html.indexOf("$$error$$", 0)
        if (index < 0 || index > 1000) {
            FilterForm.HandleSuccess(html, false, guid, pks);
            var afterAddItemsArgs = { guid: guid, viewName: viewName, pks: pks, parentViewName: parentViewName, fk: fk };
            $(AddItemsDialog).trigger('afterAddItems', afterAddItemsArgs);
            dialog.dialog('close');

        }
        else {
            //FilterForm.ShowFailureMessage(html.replace("$$error$$", ""), guid);
            dialog.dialog('close');
            Refresh(guid);
            setTimeout(function () {
                ajaxNotSuccessMsg(html, guid);
            }, 1);

        }


        setTimeout(function () {
            loadMainMenu(guid);
        }, 1);
    });

}

function loadMainMenu(guid) {
    $.ajax({
        url: rootPath + views[guid].Controller + '/GetMainMenu/',
        contentType: 'application/html; charset=utf-8',
        data: {},
        async: false,
        cache: false,
        success: function (html) {
            $('div.workspaceMenu').html(html);
            InitTopMenu();
        }
    });
}

function GetWorkspaceMenu() {
    $.ajax({
        url: '/Admin/GetWorkspaceMenu/',
        contentType: 'application/html; charset=utf-8',
        data: {},
        async: false,
        cache: false,
        success: function (html) {
            $('div.workspaceMenu').html(html);
            $('#mainmenu').superfish();
    
        }
    });
}


$(AddItemsDialog).bind('beforeAddItems', function (e, data) {
    if (data.viewName == 'View') {

        data.searchUrl = '/Admin/Index/durados_Schema?firstTime=true';


    }
});


AddItemsDialog.getParentGuid = function (childGuid) {
    var childGrid = $('#' + childGuid + 'ajaxDiv');
    if (childGrid.length > 0) {
        var parentGrid = childGrid.parents("div[ajaxdiv='ajaxDiv']:first");
        if (parentGrid.length > 0) { // sub grid
            return parentGrid.attr("guid");
        }
        else {
            var g = childGrid.parents('form:first').parents('div[hasguid="hasguid"]:first').attr("guid");
            if (g == null) { // link to children
                return null;
            }
            else { // edit
                return g;
            }
        }
    }
    return null;
}

AddItemsDialog.getParentKey = function (childGuid) {
    var childGrid = $('#' + childGuid + 'ajaxDiv');
    var tr = childGrid.parents("tr:first");
    var trPrev = tr.prev();
    var pk = trPrev.attr("d_pk");
    if (pk == null) {
        pk = childGrid.parent().parent().find('div.childrenViewer:first').attr('pk');
        if (pk == null) {
            pk = childGrid.parents("div[hasguid='hasguid']:first").attr('pk');
            if (pk == null)
                return null; // link to children
            else
                return pk; // sub grid tab
        }
        else {
            return pk; // edit
        }
    }
    else { // sub grid
        return pk;
    }
}

AddItemsDialog.getSelectAll = function (searchGuid) {
    var searchGrid = $('#' + searchGuid + 'ajaxDiv');
    return isChecked = Durados.CheckBox.IsChecked($(searchGrid).find('table.gridview tr.gridviewhead th.thCheck input:first'));
    
  
}

function d_addItems(mobile, url, searchUrl, viewName, viewDisplay, guid, anchor) {
    showProgress();
    setTimeout(function () {
        AddItemsDialog.CreateAndOpen(viewName, viewDisplay, url, searchUrl, guid, anchor);
    }, 1);
}

function d_addNew(pk, popup, mobile, url, viewName, guid, element, allowEdit, insertAbovePK, duplicateRecursive, parentGuid) {
    var topView = $(element).closest('form').length == 0;
    if ((popup || !topView) && !mobile) {
        AddDialog.Open(pk, guid, false, allowEdit, insertAbovePK, duplicateRecursive, parentGuid);
    }
    else {
        window.location = url + "&allowEdit=" + allowEdit + "&insertAbovePK=" + insertAbovePK + "&duplicateRecursive=" + duplicateRecursive;
    }
}

function d_edit(pk, popup, mobile, url, viewName, guid, element, e) {
    if (pk == null && $(element).attr('pk') != null) {
        var gridEditing = $(element).parent('TD.d_fieldContainer').attr('d_role') == 'cell';
        if (gridEditing && !e.ctrlKey)
            return;
        pk = $(element).attr('pk');
    }

    var topView = $(element).closest('form').length == 0;
    var isPreviewMode = guid != null && views[guid] != null && views[guid].DataDisplayType == "Preview";

    if (((popup || !topView) && !mobile) || isPreviewMode) {
        EditDialog.Open(pk, guid, false);
    }
    else {
        url = url.replace("pk=____", "pk=" + pk);
        window.location.href = url;
    }
}

function d_Duplicate(pk, popup, mobile, urlAdd, urlEdit, viewName, guid, element, duplicationMethd, message) {
    if (duplicationMethd == "Shallow") {
        d_addNew(pk, popup, mobile, urlAdd, viewName, guid, element, true, pk, false);
    }
    else if (duplicationMethd == "Recursive") {
        //d_DuplicateRecursive(pk, popup, mobile, urlEdit, viewName, guid, element);
        d_addNew(pk, popup, mobile, urlAdd, viewName, guid, element, true, pk, true);
    }
    else if (duplicationMethd == "User") {
        if (confirm(message)) {
            d_addNew(pk, popup, mobile, urlAdd, viewName, guid, element, true, pk, true);
            //d_DuplicateRecursive(pk, popup, mobile, urlEdit, viewName, guid, element);
        }
        else {
            d_addNew(pk, popup, mobile, urlAdd, viewName, guid, element, true, pk, false);
        }
    }
}

function d_DuplicateRecursive(pk, popup, mobile, url, viewName, guid, element) {
    showProgress();
    $.post(views[guid].gDuplicateUrl,
    {
        pk: pk, guid: guid
    },
    function (duplicatedPk) {
        hideProgress();
        var index = duplicatedPk.indexOf("$$error$$", 0)
        if (index < 0 || index > 1000) {

            if (duplicatedPk == null || duplicatedPk == '') {
                ajaxNotSuccessMsg("error");
                //alert("error");
            }
            else {
                d_edit(duplicatedPk, popup, mobile, url.replace('xxxx', duplicatedPk), viewName, guid, element);
            }
        }
        else {
            ajaxNotSuccessMsg("error");
            //alert("error");
        }
    });
}

function getFieldJson(name, viewJson) {

    var fieldJson = jQuery.extend(true, {}, viewJson);
    fieldJson.Fields = [];

    for (var index = 0, len = viewJson.Fields.length; index < len; ++index) {
        var field = viewJson.Fields[index];

        if (field.Key == name) {
            var f = jQuery.extend(true, {}, field);

            fieldJson.Fields[0] = f;
            break;
        }
    };

    return fieldJson;
}

var fieldsDialog = null;

function AddColumn(callback) {
    d_doAction(getMainPageGuid(), 'addColumn', callback);
    //    var inlineCreateUrl = '/Admin/InlineAddingCreate/';

    //    var createDialog = CreateDialog.CreateAndOpen("Field", "Field", null, null, inlineCreateUrl, false, guid,
    //             function (another, type, id, displayValue, guid, dialog, viewName) {

    //                 if (Durados.Indications.fitToWindowWidth(guid)) {
    //                     //Calculate newColumnName
    //                     var newColumnName = dialog.find('#' + guid + 'inlineAdding_DisplayName').val();
    //                     newColumnName = ReplaceNonAlphaNumeric(newColumnName);

    //                     Durados.GridHandler.setNewColumnWidth(guid, newColumnName);
    //                 }

    //                 //Reload page
    //                 callback();
    //             });
}

function GetJson(viewName) {
    var j = null;
    $.ajax({
        url: rootPath + "Admin/GetJson/" + viewName,
        data: { viewName: viewName },
        contentType: 'application/html; charset=utf-8',
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            j = json;
        }
    });

    return j; //JSON.stringify(j, null, 2);;
}

function ShowJson(guid, viewName) {

    //var div1 = ('<div>' + Sys.Serialization.JavaScriptSerializer.serialize(GetJsonViewForEdit(guid)) + '</br>');

    //var div1 = '<div class="container">Json ' + viewName + '<div class="line number1 index0 alt2"><code class="javascript plain">{</code></div><div class="line number2 index1 alt1"><code class="javascript spaces">&nbsp;&nbsp;</code><code class="javascript string">"score"</code><code class="javascript plain">: 1337,</code></div><div class="line number3 index2 alt2"><code class="javascript spaces">&nbsp;&nbsp;</code><code class="javascript string">"playerName"</code><code class="javascript plain">: </code><code class="javascript string">"Sean Plott"</code><code class="javascript plain">,</code></div><div class="line number4 index3 alt1"><code class="javascript spaces">&nbsp;&nbsp;</code><code class="javascript string">"cheatMode"</code><code class="javascript plain">: </code><code class="javascript keyword bold">false</code><code class="javascript plain">,</code></div><div class="line number5 index4 alt2"><code class="javascript spaces">&nbsp;&nbsp;</code><code class="javascript string">"skills"</code><code class="javascript plain">: [</code></div><div class="line number6 index5 alt1"><code class="javascript spaces">&nbsp;&nbsp;&nbsp;&nbsp;</code><code class="javascript string">"pwnage"</code><code class="javascript plain">,</code></div><div class="line number7 index6 alt2"><code class="javascript spaces">&nbsp;&nbsp;&nbsp;&nbsp;</code><code class="javascript string">"flying"</code></div><div class="line number8 index7 alt1"><code class="javascript spaces">&nbsp;&nbsp;</code><code class="javascript plain">],</code></div><div class="line number9 index8 alt2"><code class="javascript spaces">&nbsp;&nbsp;</code><code class="javascript string">"createdAt"</code><code class="javascript plain">: </code><code class="javascript string">"2011-08-20T02:06:57.931Z"</code><code class="javascript plain">,</code></div><div class="line number10 index9 alt1"><code class="javascript spaces">&nbsp;&nbsp;</code><code class="javascript string">"updatedAt"</code><code class="javascript plain">: </code><code class="javascript string">"2011-08-20T02:06:57.931Z"</code><code class="javascript plain">,</code></div><div class="line number11 index10 alt2"><code class="javascript spaces">&nbsp;&nbsp;</code><code class="javascript string">"objectId"</code><code class="javascript plain">: </code><code class="javascript string">"Ed1nuqPvcm"</code></div><div class="line number12 index11 alt1"><code class="javascript plain">}</code></div>';
    var div1 = '<div>' + GetJson(viewName);
    div1 += '<br/><br/>Pyton Code:</br><div id="highlighter_590399" class="syntaxhighlighter nogutter syntaxhighlighter python"><table border="0" cellpadding="0" cellspacing="0"><tbody><tr><td class="code"><div class="container"><div class="line number1 index0 alt2"><code class="python keyword">import</code> <code class="python plain">json,httplib</code></div><div class="line number2 index1 alt1"><code class="python plain">connection </code><code class="python keyword">=</code> <code class="python plain">httplib.HTTPSConnection(</code><code class="python string">\'api.backand.com\'</code><code class="python plain">, </code><code class="python value">443</code><code class="python plain">)</code></div><div class="line number3 index2 alt2"><code class="python plain">connection.connect()</code></div><div class="line number4 index3 alt1"><code class="python plain">connection.request(</code><code class="python string">\'PUT\'</code><code class="python plain">, </code><code class="python string">\'/1/classes/GameScore/Ed1nuqPvcm\'</code><code class="python plain">, json.dumps({</code></div><div class="line number5 index4 alt2"><code class="python spaces">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</code><code class="python string">"score"</code><code class="python plain">: </code><code class="python value">73453</code></div><div class="line number6 index5 alt1"><code class="python spaces">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</code><code class="python plain">}), {</code></div><div class="line number7 index6 alt2"><code class="python spaces">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</code><code class="python string">"X-backand-Application-Id"</code><code class="python plain">: </code><code class="python string">"<span class="application_id">${APPLICATION_ID}</span>"</code><code class="python plain">,</code></div><div class="line number8 index7 alt1"><code class="python spaces">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</code><code class="python string">"X-backand-REST-API-Key"</code><code class="python plain">: </code><code class="python string">"<span class="rest_api_key">${REST_API_KEY}</span>"</code><code class="python plain">,</code></div><div class="line number9 index8 alt2"><code class="python spaces">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</code><code class="python string">"Content-Type"</code><code class="python plain">: </code><code class="python string">"application/json"</code></div><div class="line number10 index9 alt1"><code class="python spaces">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</code><code class="python plain">})</code></div><div class="line number11 index10 alt2"><code class="python plain">result </code><code class="python keyword">=</code> <code class="python plain">json.loads(connection.getresponse().read())</code></div><div class="line number12 index11 alt1"><code class="python functions">print</code> <code class="python plain">result</code></div></div></td></tr></tbody></table></div></div>';

    var dialog = $(div1)
    var title = "Json Code";


    FloatingDialog.Open(dialog, title, true, guid);

    dialog.dialog("option", "height", 'auto');
    dialog.dialog("option", "width", '700');
    dialog.dialog("option", "resizable", true);

    hideProgress();

}

function d_doAction(guid, action, callback, parameters) {

    if (!guid)
        guid = getMainPageGuid();

    if (LoadingApplicationData) return;

    var element = $("#" + guid + 'ajaxDiv');

    var allowEdit = EditDialog.AllowEdit(guid, pk);

    var viewName = views[guid].ViewName;

    switch (action) {
        case 'editValue':
            if (!parameters.guid)
                parameters.guid = getMainPageGuid();
            if (!parameters.pk)
                parameters.pk = Multi.GetSingleSelection(guid);
            if (!parameters.viewName)
                parameters.viewName = getMainViewName();

            editValue(parameters.viewName, parameters.fieldName, parameters.value, parameters.pk, parameters.guid);

            if (parameters.refresh)
                refreshView(parameters.guid);

            if (callback)
                callback(parameters);

            break;

        case 'json':
            ShowJson(guid, viewName);
            break;

        case 'edit':
            var pks = Multi.GetSelection(guid);
            if (pks.length == 1) {
                var pk = pks[0];
                var popup = views[guid].Popup;
                var mobile = views[guid].Mobile;
                var url = views[guid].EditItemUrl + '&pk=' + (pk + '');
                d_edit(pk, popup, mobile, url, viewName, guid, element);
            }
            else if (pks.length > 1) {
                EditDialog.OpenSelection(guid, false);
            }
            else {
                modalErrorMsg('Please select a row');
                //alert('Please select a row');
            }
            break;

        case 'paint':

            var pks = Multi.GetSelection(guid);
            if (pks.length == 0) { modalErrorMsg('Please select a row'); return; }

            begin();

            try {

                var jsonView = GetJsonViewForEdit(guid);

                var fieldJson = getFieldJson(views[guid].RowColorColumnName, jsonView);

                var url = views[guid].EditOnlyUrl;

                var new_color = $('#color_input').val().toLowerCase();


                if (!new_color) new_color = "";

                var response;

                for (var i = 0; i < pks.length; i++) {

                    pk = pks[i];

                    fieldJson.Fields[0].Value.Value = new_color;

                    response = $.ajax({
                        async: false,
                        type: "POST",
                        url: url,
                        data: { guid: guid, pk: pk, jsonView: Sys.Serialization.JavaScriptSerializer.serialize(fieldJson), guid: guid }
                    }).responseText;
                    if (response != '"success"') {
                        ajaxNotSuccessMsg('Changes did not saved: ' + response);
                        //return;
                    } else {
                        $("#d_row_" + guid + pk).attr('class', new_color);
                    }
                }
                hideProgress();

            } catch (ex) { hideProgress(); modalErrorMsg(ex.description); }

            break;

        case 'add':

            var popup = views[guid].Popup;
            var mobile = views[guid].Mobile;
            var url = views[guid].AddItemUrl;
            d_addNew(null, popup, mobile, url, viewName, guid, element, !allowEdit.disabled, '', false);
            break;

        case 'duplicate':
            var pks = Multi.GetSelection(guid);
            if (pks.length == 1) {
                var pk = pks[0];
                var popup = views[guid].Popup;
                var mobile = views[guid].Mobile;
                var urlAdd = views[guid].DuplicateItemUrl + '&pk=' + (pk + '');
                var urlEdit = views[guid].EditItemUrl + '&pk=' + (pk + '');
                var duplicationMethd = views[guid].DuplicationMethod;
                var message = views[guid].DuplicateMessage;
                d_Duplicate(pk, popup, mobile, urlAdd, urlEdit, viewName, guid, element, duplicationMethd, message);
            }
            else if (pks.length > 1) {
                ajaxNotSuccessMsg('Please select only one row');
                //alert('Please select only one row');
            }
            else {
                ajaxNotSuccessMsg('Please select a row');
            }
            break;

        case 'insert':
            var pks = Multi.GetSelection(guid);
            if (pks.length == 1) {
                var pk = pks[0];
                var popup = views[guid].Popup;
                var mobile = views[guid].Mobile;
                var url = views[guid].InsertItemUrl;
                //var allowEdit = EditDialog.AllowEdit(guid, pk);
                d_addNew(null, popup, mobile, url, viewName, guid, element, !allowEdit.disabled, (pk + ''), false);
            }
            else if (pks.length > 1) {
                ajaxNotSuccessMsg('Please select only one row');
            }
            else {
                ajaxNotSuccessMsg('Please select a row');
            }
            break;
        case 'delete':
            var pks = Multi.GetSelection(guid);
            if (pks.length == 1) {
                var pk = pks[0];
                DeleteDialog.Open(pk, guid);
            }
            else if (pks.length > 1) {
                DeleteDialog.OpenSelection(guid);
            }
            else {
                ajaxNotSuccessMsg('Please select a row');
            }
            break;

        case 'history':
            var pks = Multi.GetSelection(guid);
            if (pks.length == 1) {

                var historyUrl = rootPath + "History/HistoryFilter/durados_v_ChangeHistory";
                historyGuidID++;
                History(historyUrl, guid, "history" + historyGuidID, viewName);
            }
            else if (pks.length > 1) {
                ajaxNotSuccessMsg('Please select only one row');
            }
            else {
                ajaxNotSuccessMsg('Please select a row');
            }
            break;

        case 'copy':
            copyPaste.copy();
            break;

        case 'paste':
            copyPaste.paste();
            break;

        case 'send':
            Send.Show(guid);
            break;

        case 'complete':
            var pks = Multi.GetSelection(guid);
            if (pks.length == 1) {
                var pk = pks[0];
                var result = null;

                var stepUrl = rootPath + views[guid].Controller + '/Step/' + views[guid].gViewName;

                $.ajax({
                    url: stepUrl,
                    data: { viewName: viewName, pk: pk, guid: guid, noPrevDialog: true },
                    contentType: 'application/html; charset=utf-8',
                    async: false,
                    cache: false,
                    error: ajaxErrorsHandler,
                    success: function (json) {
                        result = json;
                    }

                });

                WorkFlow.CompleteStep(pk, guid, views[guid].WorkFlowStepsFieldName, result, null);
            }
            else {
                ajaxNotSuccessMsg('Please select a row');
            }
            break;

        case 'config':
            window.open(rootPath + "Admin/ConfigField/" + viewName + "?fieldName=" + contextInfo[guid].fieldName, contextInfo[guid].fieldName);
            break;

        case 'hide':

            var hideColumn = function (hideInDialogs) {
                showProgress();
                var hideFieldUrl = rootPath + "Admin/HideField/" + viewName + "?fieldName=" + contextInfo[guid].fieldName + "&hideInDialogs=" + (hideInDialogs ? "true" : "false");

                $.ajax({
                    url: hideFieldUrl,
                    contentType: 'application/html; charset=utf-8',
                    async: false,
                    cache: false,
                    error: ajaxErrorsHandler,
                    success: function (json) {
                        hideProgress();

                        /*
                        try {
                        var newColumnName = contextInfo[guid].fieldName;
                        newColumnName = ReplaceNonAlphaNumeric(newColumnName);
                        var newColumnWidth = 80;

                        if (Durados.Indications.fitToWindowWidth(guid)) {
                            
                        }
                        else {
                        var newColumnState = { "Key": newColumnName, "Value": { "width": newColumnWidth} };
                        Durados.ColumnResizer.saveState(guid, false, newColumnState, false);
                        }
                        }
                        catch (err) {
                        }
                        */
                        Durados.GridHandler.hideColumn(contextInfo[guid].targetElement, guid);
                    }

                });
            }

            Durados.Dialogs.Confirm("Hide Column", "Do you to hide it in the Add and Edit dialogs as well?", function () { hideColumn(true); }, function () { hideColumn(false); });

            break;
        case 'unhide':
            var data = { viewName: viewName, fieldName: contextInfo[guid].fieldName };
            selectField(viewName, contextInfo[guid].fieldName, guid, unhideFieldCallback, data, 'HideInTable;True');
            break;

        case 'open':
            var pks = Multi.GetSelection(guid);
            if (pks.length == 1) {
                var pk = pks[0];

                var openUrl = rootPath + "Admin/Open/" + viewName + "?pk=" + pk;
                window.open(openUrl);
            }
            break;

        case 'commit':
            showProgress();
            var url = rootPath + 'Admin/RefreshConfig/';
            refreshConfig(url, guid);
            fieldsDialog = null;
            hideProgress();
            break;

        case 'before':
            //contextInfo[guid].before = true;
            //moveField(viewName, contextInfo[guid].fieldName, true, guid);
            movedColumnInfo[guid] = { elementToMove: contextInfo[guid].targetElement, placement: 'before' };
            Durados.GridHandler.setHeadersMoveToClass(guid, 'Left');
            break;

        case 'after':
            //contextInfo[guid].before = false;
            //moveField(viewName, contextInfo[guid].fieldName, false, guid);
            movedColumnInfo[guid] = { elementToMove: contextInfo[guid].targetElement, placement: 'after' };
            Durados.GridHandler.setHeadersMoveToClass(guid, 'Right');
            break;

        case 'left':
            var to = $(contextInfo[guid].targetElement).prev();
            var fn = to.attr('SortColumn')
            if (fn) {
                movedColumnInfo[guid] = { elementToMove: contextInfo[guid].targetElement, placement: 'before' };
                moveFieldTo(viewName, contextInfo[guid].fieldName, true, guid, fn, rootPath + "Admin/MoveField2/" + viewName);
                Durados.GridHandler.moveColumn(guid, to);
                movedColumnInfo = {};
            }
            else
                ajaxNotSuccessMsg("There is no left column.");
            break;

        case 'right':
            var to = $(contextInfo[guid].targetElement).next();
            var fn = to.attr('SortColumn')
            if (fn) {
                movedColumnInfo[guid] = { elementToMove: contextInfo[guid].targetElement, placement: 'after' };
                moveFieldTo(viewName, contextInfo[guid].fieldName, false, guid, fn, rootPath + "Admin/MoveField2/" + viewName);
                Durados.GridHandler.moveColumn(guid, to);
                movedColumnInfo = {};
            }
            else
                ajaxNotSuccessMsg("There is no right column.");

            break;

        case 'rename':
            //renameField(guid, viewName, contextInfo[guid].fieldName, rootPath + "Admin/RenameField/" + viewName);
            renamedFieldInfo = contextInfo[guid];
            renamedFieldInfo.guid = guid;

            var th = renamedFieldInfo.targetElement;

            var nameWrapper = th.find('A');
            if (!nameWrapper.length) {
                nameWrapper = th.find('div');
            }
            nameWrapper.css('visibility', 'hidden');

            var location = th.offset();
            var reNameForm = $('#reNameForm');
            if (!reNameForm.length) {
                var reNameForm = $('<input id="reNameForm" type="text" />');
                $('body').append(reNameForm);
                reNameForm.blur(renameFieldCallBack);
                reNameForm.keydown(function (event) { if (event.keyCode == $.ui.keyCode.ENTER) renameFieldCallBack(); });
            }

            reNameForm.val(nameWrapper.text().trim());
            if ($('body').attr('dir') != 'rtl') {
                reNameForm.css({ top: location.top + 3, left: location.left + 10, display: 'inline' });
            } else {
                var w = th.width() + 10;
                reNameForm.css({ top: location.top + 3, left: location.left - 100 + w, display: 'inline' });
            }
            reNameForm.focus();
            reNameForm[0].select();
            break;

        case 'saveAsDefaults':
            Durados.ColumnResizer.saveState(guid, true);
            break;

        case 'restoreDefaults':
            Durados.ColumnResizer.restoreState(guid);
            break;

        case 'addColumn':
            var inlineCreateUrl = '/Admin/InlineAddingCreate/';

            var createDialog = CreateDialog.CreateAndOpen("Field", "Field", null, null, inlineCreateUrl, false, guid,
             function (another, type, id, displayValue, guid, dialog, viewName) {

                 var newColumnName = dialog.find('#' + guid + 'inlineAdding_DisplayName').val();
                 newColumnName = ReplaceNonAlphaNumeric(newColumnName);

                 if (Durados.Indications.fitToWindowWidth(guid)) {
                     //Calculate newColumnName

                     Durados.GridHandler.setNewColumnWidth(guid, newColumnName);
                 }

                 //Reload page
                 if (callback)
                     callback(newColumnName);
                 else
                     commitAndReload();
             });

            var viewSelect = createDialog.find('select[name="Fields_Parent"]');
            if (viewSelect.length > 0) {
                viewSelect.find("option").each(function () {
                    var opt = $(this);
                    if (opt.text() == viewName) {
                        viewSelect.val(opt.attr('value'));
                        return false;
                    }
                });
            }
            var td = viewSelect.parent("td:first");
            td.hide();
            td.prev().hide();

            break;

        case 'properties':
            var inlineEditUrl = '/Admin/InlineEditingEdit/';
            var fieldPK = GetFieldPK(viewName, contextInfo[guid].fieldName);
            if (fieldPK == null || fieldPK == '')
                ajaxNotSuccessMsg("Could not find the column.");

            InlineEditingDialog.CreateAndOpen2("Field", "Field", null, null, inlineEditUrl, guid, null, fieldPK, true, propertiesCallback, false);

            break;

        default:
            var pks = Multi.GetSelection(guid);
            $(Durados.View).trigger('anotherRowFunction', { guid: guid, pks: pks });


    }
}

var dock = null;

function isViewPropertiesFloat() {
    return $.cookie("viewProperties") == 'float';
}

/************************************************************************************/
/*		toggleViewProperties (by br)						
/*		Display or hide view properties.
/************************************************************************************/
function toggleViewProperties(guid, vfloat, width, url, viewDisplayName) {
    var viewPropIframe = $('#d_viewProp');

    if (viewPropIframe.length) {
        try {
            var viewPropWindow = viewPropIframe[0].contentWindow;

            if (viewPropWindow == null || viewPropWindow.Durados == null || viewPropWindow.Durados.DuringdLoadIframe) {
                return;
            }

            viewPropWindow.$('#DataRowEdit').dialog('close');
        }
        catch (ex) {
            return;
        }
    }
    else {
        viewProperties(guid, vfloat, width, url, viewDisplayName);
    }
}

function mobileMenu(topTitle) {
    var mainDiv = $('#mainAppDiv');
    var mobileMenuDiv = $('#mobileMenuDiv');
    if (mobileMenuDiv.length > 0) {
    }
    else {
        mobileMenuDiv = $('<div id="mobileMenuDiv" style="width:' + ($(window).width() * 0.8) + 'px; height:' + ($(window).height() - mainDiv.offset().top) + 'px; position:absolute; top:37px; z-index:1000; border:1px solid black;" class="mobile-settings-menu"></div>');
        $('body').prepend(mobileMenuDiv);
        var mobileMenuContent = $('.mobileMenuContent')
        mobileMenuDiv.append(mobileMenuContent);
        mobileMenuContent.show(400);

        mobileMenuContent.find('.mobile-settings-menu').mobileSettings({ topTitle: topTitle, beforeNavigationCallback: function (li) { showProgress(); } });
        
        return;
    }

    if (mobileMenuDiv.is(':visible')) {
        mobileMenuDiv.hide(400);
    }
    else {
        mobileMenuDiv.show(400);
    }
}

var nestableState = null;
function pagesManager(width, url) {
    var mainDiv = $('#mainAppDiv');
    $('#AppFilterTreeDiv').remove();
    mainDiv.attr('class', 'main_app_1_tree');
    var viewDiv = $('<div id="AppFilterTreeDiv" style="width:' + width + 'px; background-color:White !important; height:' + ($(window).height() - mainDiv.offset().top - 16) + 'px; " class="filter_tree dark-bg"></div>');
    viewDiv.append('<div id="loadSettings" class="loading"><div>' + translator.LoadingSettingsMessage + '</div><img src="/Content/Images/wait.gif" alt="' + translator.LoadingSettingsMessage + '" /></div>');
    mainDiv.before(viewDiv);

    if ($('body').attr('dir') != 'rtl') {
        mainDiv.css('margin-left', width + 'px');
    }
    else {
        mainDiv.css('margin-right', width + 'px');
    }

    $.ajax({
        url: url,
        contentType: 'application/html; charset=utf-8',
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (html) {
            viewDiv.html(html);
            var pageManager = $('.pages-manager');
            pageManager.nestable({ maxDepth: 2 }).on('change', function () {
                var s = window.JSON.stringify($('.pages-manager').nestable('serialize'));
                if (s != nestableState) {
                    savePages(s);
                    
                }
            });
            slider.InitSlider(null, 2);

            $('.workspace-slider-container .anythingSlider-default').css('width', '240px').css('height', '50px');

            var h = $('#AppFilterTreeDiv').height() - 260;

            $('.pages-manager-scroll').css('height', h +'px');

            viewDiv.find('input[name="Workspace"]').change(function () {
                changeWorkspace($(this).val(), viewDiv.find('.slider-active:first').attr('index'));
            });

            pageManager.find('.dd3-content').mouseup(function () {
                selectPage($(this));
            });

            pageManager.find('.dd3-settings').click(function () {
                pageSettings($(this));
            });

            selectFirstPage(pageManager);

            $('#AppFilterTreeDiv .pages-done').click(function () {
                var selectedPage = getSelectedPage();
                var url = selectedPage.attr('url');
                var c = url.indexOf('?') > -1 ? '&' : '?';
                var menuId = selectedPage.parent('li:first').attr('data-id');

                window.location = url + c + 'menuId=' + menuId;
            });


            if (document.location.href.indexOf('/Admin/Pages') > -1) {
                $('body').addClass("page-page");
                if (queryString('p') == 'on') {
                    openAddPageDialog();
                }
                else
                    $('body').removeClass("page-page");
            }
        }
    });

    $(window).resize();
}

function getDefaultPageName() {
    var pagesNames = $('.pages-manager .page-name');
    var prefixU = 'Page';
    var prefixL = 'page';
    for (var i = 1; i < 1000; i++) {
        var f = false;
        pagesNames.each(function () {
            var t = $(this).text().trim();
            if (t == prefixL + i || t == prefixU + i) {
                f = true;
                return false;
            }
        });
        if (!f) return prefixU + i;
    }
}

function getSelectedPage() {
    var pageManager = $('.pages-manager');
    return pageManager.find('.dd3-selected');
}

function selectFirstPage(pageManager) {
    var homepage = pageManager.find('.home-page-show:first');
    page = null;
    var menuId = queryString('menuId');
    if (menuId) {
        page = pageManager.find('li[data-id="' + menuId.replace('#','') + '"]:first').find('.dd3-content');
    }
    if (page && page.length == 1) {
        selectPage(page);
    }
    else {
        if (homepage.length == 1) {
            page = homepage.parent();
            selectPage(page);
        }
        else {
            var firstPage = pageManager.find('.dd3-content:first');
            if (firstPage.length == 1) {
                page = firstPage;
                selectPage(page);
            }
            else {
                selectPage2(null, '/Home/Default?workspaceId=' + getWorkspaceId(), true, '', { workspaceId: getWorkspaceId() });
            }
        }
    }
}

function changeWorkspace(workspaceId, index) {
    showProgress();

    
    var div = $('#AppFilterTreeDiv');
    closePageSettings(div);
    if (pageSettingsDialog)
        pageSettingsDialog.dialog('destroy');

    $.ajax({
        url: '/Admin/ChangeWorkspace/',
        data: { workspaceId: workspaceId },
        contentType: 'application/json; charset=utf-8',
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            try {
                if (!json.Failure) {
                    var pageManager = $('.pages-manager-wrapper');
                    pageManager.html(json);
                    var pageManager2 = $('.pages-manager');
                    pageManager2.nestable({ maxDepth: 2 }).on('change', function () {
                        var s = window.JSON.stringify($('.pages-manager').nestable('serialize'));
                        if (s != nestableState) {
                            savePages(s);
                            
                        }
                    });

                    var h = $('#AppFilterTreeDiv').height() - 260;

                    $('.pages-manager-scroll').css('height', h + 'px');

                    pageManager.find('.dd3-content').mouseup(function () {
                        selectPage($(this));
                    });

                    pageManager.find('.dd3-settings').click(function () {
                        pageSettings($(this));
                    });

                    GetWorkspaceMenu();

                    if (index) {
                        $('#topMenu div.workspace').attr('class', 'workspace space-selected' + index);
                        
                        $('.workspace-slider-container .ui-slider-item-workspace').removeClass('workspace-slider-active');
                        $('.workspace-slider-container .slider-active').removeClass('slider-active');
                        $('.workspace-slider-container .workspace-selector.item' + index + ':first').parent().addClass('workspace-slider-active');

                        $('#topMenu div.workspace .w-text').text($('.workspace-slider-container .workspace-slider-active .w-text').text());
                        
                    }

                    selectFirstPage(pageManager);

                }
                else {
                    reloadPage();
                }
            }
            catch (err) {
            }
            finally {
                hideProgress();
            }
        }
    });
}

var selectedPage = null;

function selectPage(page, refresh) {
    if (page == selectedPage && !refresh)
        return;
        
    var b = !refresh && pageSettingsDialogOpen && pageSettingsDialog != null;
    
    var url = page.attr('url');

    var menuId = page.parents('li:first').attr('data-id');

    $('#settings a').attr('href', GetUrlWithSettings(url, 'menuId=' + menuId +'&settings=true'));
    $('#columns a').attr('href', GetUrlWithSettings(url, 'menuId=' + menuId + '&settings=fields'));

    
    selectPage2(page, url, page.find('.dd3-settings').attr('viewName') == 'View', page.text(), { firstTime: true, guid: pageGuid });

    if (b) {
        pageSettings(page.parents('li:first').find('.dd3-settings'));
    }

}


function selectPage2(page, url, isView, text, data) {
    var selectedClass = 'dd3-selected';
    var selectedLiClass = 'dd3-item-selected';

    if (page && page.length == 1) {
        $('.pages-manager').find('.dd3-content').removeClass(selectedClass);
        $('.pages-manager').find('li').removeClass(selectedLiClass);
        page.addClass(selectedClass);
        page.parent('li:first').addClass(selectedLiClass);

        selectedPage = page;
    }
    else {
        selectedPage = null;
    }

    var iframe = $('#mainAppFrame');
  
    var c = url.indexOf('?') > -1 ? '&' : '?';
    iframe.attr('src', url + c + 'menu=off');
    
    /*
    showProgress();
    $.ajax({
        url: url,
        data: data,
        contentType: 'application/html; charset=utf-8',
        async: true,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (html) {
            try {
                $('#mainAppDiv').html(html);
                isView = $('#mainAppDiv').find('.gridview').length > 0;
                isCharts = $('#mainAppDiv').find('.charts').length > 0;
                if (isView) {
                    initDataTableView(pageGuid);
                    $('.refer-bar').find('a,span').attr('onclick', '');
                    $('#rowtabletitleSpan').text(text + '  ');
                }
                else if (isCharts) {
                    Charts.run();
                }
            }
            catch (err) {
            }
            finally {
                hideProgress();
            }
        }
    });
    */
}

function getWorkspaceId() {
    return $('#xxPageXx_inlineEditing_Workspace').val();
}

function savePages(pages) {
    var workspaceId = getWorkspaceId();
    var data = { workspaceId: workspaceId, json: pages };
    $.post("/Admin/SavePages",
        data,
        function (json) {
            if (json.Success) {
                nestableState = pages;
                GetWorkspaceMenu();
            }
            else {
                changeWorkspace(workspaceId);
                Durados.Dialogs.Alert("", json.Exception);
            }
        });
    }

function toggleAddPageView(dialog, pageType) {
    if (pageType == 'Grid1' || pageType == 'Grid2' || pageType == 'Cards' || pageType == 'Preview') {
        dialog.find('.add-page-create-view').show();
    }
    else {
        dialog.find('.add-page-create-view').hide();
    }
   
}

function toggleAddPageDasboard(dialog, pageType) {
    if (pageType == 'Charts') {
        dialog.find('.add-page-create-dashboard').show();
    }
    else {
        dialog.find('.add-page-create-dashboard').hide();
    }
}

function openAddPageDialog() {
    showProgress();
    //$('.page-settings-done').click();

    $.ajax({
        url: "/Admin/AddPage",
        contentType: 'application/html; charset=utf-8',
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (html) {
            try {
                $('<div></div>').appendTo('body')
          .append(html)
          .dialog({
              modal: true, title: "Add Page", zIndex: 10000, autoOpen: true,
              width: '400px', height: 'auto', position: ['center', 'center'], resizable: false, close: function () {
                  var dialog = $(this);
                  dialog.remove();
                  dialog.dialog('destroy');
              },
              open: function () {
                  var dialog = $(this);
                  var pageType = dialog.find('select.add-page-type').val();
                  toggleAddPageView(dialog, pageType);
                  toggleAddPageDasboard(dialog, pageType);
                  dialog.find('select.add-page-type').change(function () {
                      pageType = $(this).val();
                      toggleAddPageView(dialog, pageType);
                      toggleAddPageDasboard(dialog, pageType);
                      var i = $(this)[0].selectedIndex;
                      if (i == -1) {
                          dialog.find('.page-type-image').css('background-position', '0 -70px');
                      }
                      else {
                          i = i - 1;
                          if (i == -1) i = 0;
                          dialog.find('.page-type-image').css('background-position', '-' + (170 + i * 110) + 'px 0');
                      }

                  });

                  var addPageEntity = dialog.find('select.add-page-entity');
                  addPageEntity.change(function () {
                      var v = $(this).val();
                      try {
                          var pageName = dialog.find('.add-page-name');

                          if (!pageName.val()) {
                              var displayName = addPageEntity.find('option[value="' + v + '"]').attr('displayName');
                              pageName.val(displayName);
                          }
                      }
                      catch (err) { }
                      var isNewView = addPageEntity.find('option[value="' + v + '"]').attr('isNewView') == 'yes';
                      if (isNewView) {
                          dialog.find('[name="add-page-editable-table"],[for="add-page-editable-table"]').show();
                      }
                  });

                  var createView = dialog.find('[name=createView]');

                  createView.change(function () {
                      var v = $(this).val();

                      dialog.find('[name="add-page-editable-table"],[for="add-page-editable-table"]').hide();
                      if (v == "0") {
                          addPageEntity.hide();
                      }
                      else if (v == "1") {
                          addPageEntity.hide();
                      }
                      else {
                          addPageEntity.show();

                      }
                  });

                  var createDasboard = dialog.find('[name=createDashboard]');

                  createDasboard.change(function () {
                      var v = $(this).val();
                      var addExistingDashbord = dialog.find('select.add-page-dashboard')
                      addExistingDashbord.hide();
                      if (v == "3") {
                          addExistingDashbord.hide();
                      }

                      else {
                          addExistingDashbord.show();

                      }
                  });
                  var qv = queryString("v");
                  if (!qv) qv = "2";

                  //alert(qv);

                  if (qv != "2") {
                      var pageName = dialog.find('.add-page-name');
                      if (!pageName.val()) pageName.val(getDefaultPageName());
                  }

                  if (qv.charAt(qv.length - 1) == '#') {
                      qv = qv.substr(0, qv.length - 1);
                  }

                  dialog.find('[value=' + qv + ']').prop("checked", true).change();
                  dialog.find('#NewDashboard').prop("checked", true).change();
                  //                  alert(dialog.find('[value=' + qv + ']').attr('id'));
                  //                  createView.change();
                  //                  dialog.find('select.add-page-entity').change(function () {
                  //                      var v = $(this).val();
                  //                      //var excel = dialog.find('[name="add-page-excel"]');
                  //                      dialog.find('[name="add-page-editable-table"],[for="add-page-editable-table"]').hide();
                  //                      if (v == "0") {
                  //                          //excel.hide();
                  //                      }
                  //                      else if (v == "1") {
                  //                          //excel.show();
                  //                      }
                  //                      else {
                  //                          //excel.hide();
                  //                          try {
                  //                              var pageName = dialog.find('.add-page-name');

                  //                              if (!pageName.val()) {
                  //                                  var displayName = $(this).find('option[value="' + v + '"]').attr('displayName');
                  //                                  pageName.val(displayName);
                  //                              }
                  //                          }
                  //                          catch (err) { }
                  //                          var isNewView = $(this).find('option[value="' + v + '"]').attr('isNewView') == 'yes';
                  //                          if (isNewView) {
                  //                              dialog.find('[name="add-page-editable-table"],[for="add-page-editable-table"]').show();
                  //                          }
                  //                      }
                  //                  });

                  var buttonset = $(this).parent().find('.ui-dialog-buttonset').css('float', 'none');
                  buttonset.find('button:first').css('margin-left', '10px');
                  buttonset.find('button:last').css('float', 'right');

              },
              buttons: {
                  "Cancel": function () { $(this).dialog('close'); },
                  "Next": function () { addPage($(this)); }
              }

          });
            }
            catch (err) {
            }
            finally {
                hideProgress();
            }
        }
    });

}

function addPage(dialog) {
    var pagesManager = $('.pages-manager');
    var entity = dialog.find('select.add-page-entity').val();
    var createView = dialog.find('[name=createView]').val();
    

    var menuName = dialog.find('input.add-page-name').val();
    var pageType = dialog.find('select.add-page-type').val();
    
    var newName = menuName;
    //var excel = dialog.find('input[name="add-page-excel"]').val();
    var isNew = $('#NewTable').prop('checked') && (pageType == 'Grid1' || pageType == 'Grid2' || pageType == 'Cards' || pageType == 'Preview');
    var isExcel = $('#Excel').prop('checked') && (pageType == 'Grid1' || pageType == 'Grid2' || pageType == 'Cards' || pageType == 'Preview');
    var isNewView = dialog.find('select.add-page-entity').find('option[value="' + entity + '"]').attr('isNewView') == 'yes';
    var editable = dialog.find('select.add-page-editable-table').val();
    if (editable == -1)
        editable = '';

    if (!pageType) {
        Durados.Dialogs.Alert("Missing data", "Please select Page Layout");
        return;
    }

    if (!menuName) {
        Durados.Dialogs.Alert("Missing data", "Please Name your page");
        hideProgress();
        return;
    }

    if (pageType == 'Grid1' || pageType == 'Grid2' || pageType == 'Cards' || pageType == 'Preview') {


        if ($('#Select').prop('checked')) {
            if (!entity || entity == -1) {
                Durados.Dialogs.Alert("Missing data", "Please select table");
                hideProgress();
                return;
            }
        }
        else if (!isNew && !isExcel) {
            Durados.Dialogs.Alert("Missing data", "Please Select one of the radio options");
            hideProgress();
            return;
        } 
    }

    if (isNew && !newName) {
        Durados.Dialogs.Alert("Missing data", "Please enter Table Name");
        hideProgress();
        return;
    }

    if (isExcel) {
        dialog.dialog('close');
        hideProgress();
        duradosImport("View", AddPageImportFromExcel, dialog, menuName);
           
        return;
    }
        
    if (isNewView && (!editable || editable==-1 || editable == '')) {
        Durados.Dialogs.Alert("Missing data", "Please select Editable Table");
        hideProgress();
        return;
    }

    if ( pageType == 'Charts' ) {

        if (!IsAddDashboardDataOK(dialog)) {
            hideProgress();
            return;
        }
        else {
            isNew = dialog.find('input[name=createDashboard]:checked').val() == 3;
            entity = dialog.find('select[name="add-page-dashboard"]').val();
         }
     }
    showProgress();
    CreatePage(selectedPage, pagesManager, pageType, entity, isNew, isExcel, menuName, editable, dialog);
}
function IsAddDashboardDataOK(dialog) {
   
    var createDashboard = dialog.find('input[name=createDashboard]:checked').val()
    var selectedDashboard = dialog.find('select[name="add-page-dashboard"]').val();
    if (createDashboard != 3 && createDashboard != 4) {
        Durados.Dialogs.Alert("Missing data", "Please select option");
        return false;
    }
    if (createDashboard == 4 && selectedDashboard == -1) {
        Durados.Dialogs.Alert("Missing data", "Please select Dashboard");
        return false;
    }

    return true;

}
function AddNewChartToDashboard(dashboardId) {
    showProgress();
    var data = { dashboardId: dashboardId };
    $.post("/Admin/AddChart",
        data,
        function (json) {
            try {
                if (json.Success) { }
            }
            catch (err) {
            }
            finally {
                hideProgress();
            }
        });
}

function CreatePage(selectedPage, pagesManager, pageType, entity, isNew, isExcel, menuName, editable, dialog) {

    var data = { viewName: entity, workspace: getWorkspaceId(), isNew: isNew, isExcel: isExcel, menuName: menuName, type: pageType, editableTable: editable };

    $.post("/Admin/AddPage",
        data,
        function (json) {
            try {
                if (json.Success) {
                    var pageTypeClass = "page-type-" + json.LinkType;

                    var li = $('<li class="dd-item dd3-item" data-id="' + json.id + '"><div class="dd-handle dd3-handle"></div><div class="dd3-content" url="' + json.Url + '"><span class="page-type ' + pageTypeClass + '"></span><span class="page-name">' + menuName + '</span><span class="home-page"></span><span viewName="' + json.configViewName + '" pk="' + json.pk + '"  hideFromMenu="no" homepage="no" class="dd3-settings"></span></div></li>');

                    if (selectedPage) {
                        selectedPage.parents('li:first').after(li);
                    }
                    else {
                        pagesManager.find('ol.dd-list').prepend(li);
                    }
                    pagesManager.nestable({
                        expandBtnHTML: '<button data-action="expand" type="button">Expand</button>',
                        collapseBtnHTML: '<button data-action="collapse" type="button">Collapse</button>',
                        maxDepth: 2
                    });



                    var s = window.JSON.stringify($('.pages-manager').nestable('serialize'));
                    savePages(s);

                    //GetWorkspaceMenu();

                    var content = li.find('.dd3-content');
                    content.mouseup(function () {
                        selectPage($(this));
                    });

                    var settings = li.find('.dd3-settings')
                    settings.click(function () {
                        pageSettings($(this));
                    });

                    if (dialog)
                        dialog.dialog('close');

                    content.mouseup();
                    if (isNew || isExcel) {
                        selectPage2(content, GetUrlWithSettings(json.Url, 'settings=fields'), true, content.text());
                        setTimeout(function () {
                            newTebleIndication();
                        }, 2000);
                    }
                    else
                        settings.click();
                }
                else {
                    if (json.Exception) {
                        Durados.Dialogs.Alert('', json.Exception);
                    }
                }
            }
            catch (err) {
            }
            finally {
                hideProgress();
            }
        });
}

function newTebleIndication() {
    indication2('New Table', 'New Table', 'Add more column to the table by clicking the Add Column button', 300, 160, [160, 180])
}

function indication2(cookieName, title, msg, width, height, position) {
    indication1(cookieName, title, $('<div style="position:relative"><table width="100%"><tr><td align="middle" width="50%"><img src="/Content/Images/arrow-up.png"></td><td align="middle" width="50%"><img src="/Content/Images/arrow-up.png"></td></tr><tr><td colspan=2><span class="indication-msg" style="font-size: 18px;color: green;">' + msg + '</span></td></tr><tr><td colspan=2><input type="checkbox" /><span>do not show me that again</span></td></tr></table></div>'), width, height, position);
}

function indication1(cookieName, title, div, width, height, position) {
    if (!$.cookie(cookieName)) {
        div.dialog({
            title: title,
            modal: false,
            open: function () {
                var dialog = $(this);
                dialog.parent().css('opacity', 0.9);
                dialog.parent().css('background-color', '#C5C5C5');
                dialog.css('background-color', '#C5C5C5');
                dialog.find('input').click(function () { $.cookie(cookieName, true, 10000); });
                $('body').click(function () { dialog.dialog('close'); })
            },
            width: width,
            height: height,
            position: position,
            create: function (event, ui) {
                $(".ui-widget-header").hide();
            },
            resizable: false,
            draggable: false
        });
    }
}

function designTable(url) {
    $('<div><iframe width="100%" height="100%" style="border:none" src="' + url + '"></iframe></div>').appendTo('body')
          .append(html)
          .dialog({
              modal: true, title: "Design Table", zIndex: 10000, autoOpen: true,
              width: 'auto', height: 'auto', position: ['center', 'center'], resizable: false, open: function () {
                  var dialog = $(this);
                  
              },
              buttons: {
                  "Finish": function () { $(this).dialog('close'); }
              }

          });
}

function AddPageImportFromExcel(dialog,viewName) {
    var pagesManager = $('.pages-manager');
    var entity = viewName;//  dialog.find('select.add-page-entity').val();
    var menuName = dialog.find('input.add-page-name').val();
    var pageType = dialog.find('select.add-page-type').val();
    var newName = menuName;
    //var excel = dialog.find('input[name="add-page-excel"]').val();
    var isNew = entity == "0";
    var isExcel = true;
    
    CreatePage(selectedPage, pagesManager, pageType, entity, isNew, isExcel, menuName);
   
}
var pageGuid = 'xxPageXx_';

function closePageSettings(div) {
    div.css('width', 250);
    $('#mainAppDiv').css('margin-left', 250);
    $(window).resize();
    pageSettingsDialogOpen = false;
    pageWasOpen = false;
}

var prevMenuName = '';
var pageSettingsDialog = null;
var pageSettingsDialogOpen = false;

function pageSettings(page) {
    var div = $('#AppFilterTreeDiv');
    if (!pageSettingsDialogOpen) {
        div.css('width', 500);
        $('#mainAppDiv').css('margin-left', 500);
        $(window).resize();
        pageSettingsDialogOpen = true;
    }

    if (pageSettingsDialog) {
        pageSettingsDialog.dialog('destroy');
    }

    if (!views[pageGuid])
        views[pageGuid] = new Durados.View(pageGuid);
    views[pageGuid].gInlineEditingDialogUrl = '/Admin/GetPageSettings/';
    views[pageGuid].gGetJsonViewInlineEditingUrl = '/Home/GetJsonView/';
    //views[guid].Role = "View Owner";
    var viewName = page.attr('viewName');
    if (!viewName)
        viewName = 'MyCharts';
    var pk = page.attr('pk');
    
    var inlineEditUrl = '/Admin/InlineEditingEdit/';

    pageSettingsDialog = InlineEditingDialog.CreateAndOpen2(viewName, 'Page ' + translator.Settings, null, null, inlineEditUrl, pageGuid, null, pk, true, null, false, function () {
        selectPage(page.parent(), true);
       
    }, function () {
        closePageSettings(div);
    }

    );

    pageSettingsDialog.dialog("option", "height", $('#AppFilterTreeDiv').height() - 15);
    pageSettingsDialog.dialog("option", "width", 250);
    pageSettingsDialog.dialog("option", "draggable", false);
    pageSettingsDialog.dialog("option", "resizable", false);

    var pageName = pageSettingsDialog.find('[name="PageName"]');
    
    var hideFromMenu = pageSettingsDialog.find('[name="HideFromMenu"]');
    var homepage = pageSettingsDialog.find('[name="Homepage"]');

    pageSettingsDialog.find('.page-settings-done').click(function () { pageSettingsDialog.dialog('close'); });
    pageName.val($.trim(selectedPage.text()));

    Durados.CheckBox.SetChecked(hideFromMenu, selectedPage.find('.dd3-settings').attr('hideFromMenu') == 'yes');
    Durados.CheckBox.SetChecked(homepage, selectedPage.find('.dd3-settings').attr('homepage') == 'yes');

    prevMenuName = pageName.val();
    pageName.blur(function () {
        var menuName = $(this).val();
        if (menuName != prevMenuName) {
            data = { workspaceId: getWorkspaceId(), menuId: selectedPage.parents('li:first').attr('data-id'), name: menuName };
            $.post("/Admin/ChangePageName",
            data,
            function (json) {
                if (json.Success) {
                    selectedPage.find('.page-name').text(menuName);
                    pageName.val(menuName);
                    prevMenuName = menuName;
                    GetWorkspaceMenu();
                }
                else {
                    pageName.val(prevMenuName);
                    Durados.Dialogs.Alert("", json.Exception);
                }
            });
        }
    });

    hideFromMenu.change(function () {
        var isHide = Durados.CheckBox.IsChecked($(this));
        var isHome = Durados.CheckBox.IsChecked(homepage);
        if (isHome && isHide) {
            Durados.Dialogs.Alert("", "Cannot hide Homepage from menu");
            Durados.CheckBox.SetChecked($(this), false);
            return false;
        }
        
        data = { workspaceId: getWorkspaceId(), menuId: selectedPage.parents('li:first').attr('data-id'), hideFromMenu: isHide };
        $.post("/Admin/ChangeHideFromMenu",
        data,
        function (json) {
            if (json.Success) {
                if (isHide) {
                    selectedPage.addClass('hide-from-menu');
                }
                else {
                    selectedPage.removeClass('hide-from-menu');
                }
                GetWorkspaceMenu();
            }
            else {
                Durados.Dialogs.Alert("", json.Exception);
            }
        });
    });

    homepage.change(function () {
        var isHome = Durados.CheckBox.IsChecked($(this));
        var isHide = Durados.CheckBox.IsChecked(hideFromMenu);
        if (isHome && isHide) {
            Durados.Dialogs.Alert("", "Cannot hide Homepage from menu");
            Durados.CheckBox.SetChecked($(this), false);
            return false;
        }
        data = { workspaceId: getWorkspaceId(), menuId: selectedPage.parents('li:first').attr('data-id'), homepage: isHome };
        $.post("/Admin/ChangeHomepage",
        data,
        function (json) {
            if (json.Success) {
                var pagesManager = $('.pages-manager');
                pagesManager.find('.home-page').removeClass('home-page-show');
                pagesManager.find('.dd3-settings').attr('homepage', 'no');
                if (isHome) {
                    selectedPage.find('.home-page').addClass('home-page-show');
                    selectedPage.find('.dd3-settings').attr('homepage', 'yes');
                }
                else {
                    selectedPage.find('.home-page').removeClass('home-page-show');
                }
            }
            else {
                Durados.Dialogs.Alert("", json.Exception);
            }
        });
    });

    pageSettingsDialog.find('.page-settings-delete').click(function () {
        Durados.Dialogs.Confirm("Delete Page", "Are you sure that you want to delete this page?", function () {
            var menuId = selectedPage.parents('li:first').attr('data-id');
            data = { workspaceId: getWorkspaceId(), menuId: menuId };
            $.post("/Admin/DeletePage",
            data,
            function (json) {
                if (json.Success) {
                    var nextPage = getPrevPage(selectedPage);
                    if (!nextPage || nextPage.length == 0)
                        nextPage = getNextPage(selectedPage)
                    if (!nextPage || nextPage.length == 0) {
                        var iframe = $('#mainAppFrame');
                        iframe.attr('src', '');
                        pageSettingsDialog.dialog('close');
                        closePageSettings(div);
                        selectedPage = null;
                    }
                    else {
                        selectPage(nextPage);
                    }

                    page.parents('li:first').remove();
                    //                    var pageManager = $('.pages-manager');
                    //                    pageManager.nestable({ maxDepth: 2 }).on('change', function () {
                    //                        var s = window.JSON.stringify($('.pages-manager').nestable('serialize'));
                    //                        if (s != nestableState) {
                    //                            savePages(s);
                    //                            nestableState = s;
                    //                        }
                    //                    });
                    GetWorkspaceMenu();

                }
                else {
                    Durados.Dialogs.Alert("", json.Exception);
                    changeWorkspace(getWorkspaceId());
                }
            });

        });
    });

    var configViewName = page.attr('viewName');

    var menuId = page.parents('li:first').attr('data-id');
    
    if (viewName == "View") {
        var url = page.parent('div').attr('url');
        pageSettingsDialog.find('.page-settings-advanced').click(function () {
            reloadWithSettings(url, 'menuId=' + menuId + '&settings=true');
        });
        
        pageSettingsDialog.find('.page-settings-fields').click(function () {
            reloadWithSettings(url, 'menuId=' + menuId + '&settings=fields');
        });
        
    }
    else if (viewName == "Page") {
    pageSettingsDialog.find('.url').blur(function () {
        var a = $(this);
        var v = a.attr('value');
        InlineEditingDialog.UpdatePreview(null, pk, a.attr("name"), v, pageGuid, '/Admin/PreviewEdit/Page/', 'Page', pageSettingsDialog, function () {
            selectPage(page.parent(), true);
        });
    });
//        dialog.find('tr.settings-row').hide();
//        var pageType = page.parent('.dd3-content:first').find('.page-type');
//        if (pageType.is('page-type-')) {
//        }
    }
    //previewCallback(fieldpk, name, value, guid, url, viewName, dialog);
}

function getNextPage(page){
    var item = page.parents('li.dd-item:first');
    if (item.length == 0)
        return null;
    var nextItem = item.next();
    if (nextItem.length > 0)
        return nextItem.find('.dd3-content:first');

    return null;
}

function getPrevPage(page){
    var item = page.parents('li.dd-item:first');
    if (item.length == 0)
        return null;

    var prevItem = item.prev();
    if (prevItem.length > 0)
        return prevItem.find('.dd3-content:first');

    var parentItem = item.parents('li.dd-item:first');
    if (parentItem.length == 0)
        return null;

    return parentItem.find('.dd3-content:first');
}


function reloadWithSettings(url, qs) {
    url = url.replace('/Index/', '/IndexPage/');
    var c = url.indexOf('?') > -1 ? '&' : '?';
    window.location.href = url + c + qs;
    
}

function GetUrlWithSettings(url, qs) {
    url = url.replace('/Index/', '/IndexPage/');
    var c = url.indexOf('?') > -1 ? '&' : '?';
    return url + c + qs;

}
function viewProperties(guid, vfloat, width, url, viewDisplayName) {
    if (dock == null) {
        dock = { width: width, url: url, viewDisplayName: viewDisplayName };
    }
    if (vfloat) {
        showProgress();
        setTimeout(function () {
            var inlineEditUrl = '/Admin/InlineEditingEdit/';
            var viewName = views[guid].ViewName;
            var viewPK = GetViewPK(viewName);
            if (viewPK == null || viewPK == '')
                ajaxNotSuccessMsg("Could not find the column.");
            InlineEditingDialog.CreateAndOpen2("View", viewDisplayName + ' ' + translator.Settings, null, null, inlineEditUrl, guid, null, viewPK, true, propertiesCallback, false);
            hideProgress();
            $.cookie("viewProperties", 'float', { expires: 1000 });

        }, 10);
    }
    else {
        //YAHOO.tool.Profiler.start("settings");
        var mainDiv = $('#mainAppDiv');
        $('#AppFilterTreeDiv').remove();
        mainDiv.attr('class', 'main_app_1_tree');
        var viewDiv = $('<div id="AppFilterTreeDiv" style="width:' + width + 'px; height:' + ($(window).height() - mainDiv.offset().top - 16) + 'px; " class="filter_tree dark-bg"></div>');
        viewDiv.append('<div id="loadSettings" class="loading"><div>' + translator.LoadingSettingsMessage + '</div><img src="/Content/Images/wait.gif" alt="' + translator.LoadingSettingsMessage + '" /></div>');
        mainDiv.before(viewDiv);

        if ($('body').attr('dir') != 'rtl') {
            mainDiv.css('margin-left', width + 'px');
        }
        else {
            mainDiv.css('margin-right', width + 'px');
        }

        var frame = $('<iframe id="d_viewProp" name="viewProp" width="' + (width - 2) + '" frameborder="0" style="margin-top:3px; display: none;"></iframe>');
        viewDiv.append(frame);
        frame.height((viewDiv.height() - 8));
        //aaa;
        try {
            frame.attr('src', url);
        }
        catch (err) {
            bbb;
        }
        //        if ($.browser.msie) {
        //            frame[0].onload = function () { frame.height((frame.height() - 1)); frame.height((frame.height() + 1)); };

        //        }
        //        else {
        //            frame[0].onload = function () {
        //                setTimeout(function () {
        //                    frame.height((frame.height() - 1)); frame.height((frame.height() + 1));
        //                }, 1);
        //            };
        //        }
        $(window).resize(function () { viewDivResize(viewDiv, mainDiv, frame); })
        $(window).resize();
        $.cookie("viewProperties", '', { expires: 1000 });
        //YAHOO.tool.Profiler.stop("settings");

    }
}

function viewDivResize(viewDiv, mainDiv, frame) {
    viewDiv.height(($(window).height() - mainDiv.offset().top - 16));
    frame.height((viewDiv.height() - 8));
    $('.main-content').addClass("admin-open");
    $('body').addClass("page-settings");
    $('#AppFilterTreeDiv').addClass("admin-open");
}

function closeViewProperties() {
    //$.cookie("viewPropertiesFirstTime", true, { expires: 100000 });
                
    var mainDiv = $('#mainAppDiv');
    var viewDiv = $('#AppFilterTreeDiv');
    var guid = mainDiv.children().first().attr('guid');

    viewDiv.remove();
    mainDiv.removeAttr('class');
    if ($('body').attr('dir') != 'rtl') {
        mainDiv.css('margin-left', 0);
    }
    else {
        mainDiv.css('margin-right', 0);
    }

    $(window).resize();

    $('.main-content').removeClass("admin-open");
    $('#AppFilterTreeDiv').removeClass("admin-open");
    $('body').removeClass("page-settings");
    //    reloadPage();
}


function propertiesCallback() {
    showProgress();
    commitAndReload();
}

function commitAndReload() {
    var url = rootPath + 'Admin/RefreshConfig/';
    refreshConfig(url, null, true);
    reloadPage();
}

function reloadPage(qs, b) {
    if (qs) {
        var href = window.location.href;
        if (href.charAt(href.length - 1) == '#') {
            href = href.substr(0, href.length - 1);
        }
        if (b) {
            href = href.replace('&settings=true', '');
            href = href.replace('&settings=fields', '');
        }
        href = href + qs;
        window.location.href = href;
        if (b) return;
    }
    location.reload();
}

function GetFieldPK(viewName, fieldName) {
    var pk = null;
    $.ajax({
        url: rootPath + "Admin/GetFieldPK/" + viewName,
        data: { viewName: viewName, fieldName: fieldName },
        contentType: 'application/html; charset=utf-8',
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            pk = json;
        }
    });

    return pk;
}

function GetViewPK(viewName) {
    var pk = null;
    $.ajax({
        url: rootPath + "Admin/GetViewPK/" + viewName,
        data: { viewName: viewName },
        contentType: 'application/html; charset=utf-8',
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            pk = json;
        }
    });

    return pk;
}


function renameFieldCallBack() {
    if (!renamedFieldInfo) return;
    var guid, th, fieldName;
    //$.each(contextInfo, function(key, value) {
    guid = renamedFieldInfo.guid;
    th = renamedFieldInfo.targetElement;
    fieldName = renamedFieldInfo.fieldName;
    //});
    if (!guid || !th || !fieldName) return;
    var reNameForm = $('#reNameForm');
    var nameWrapper = th.find('A');
    if (!nameWrapper.length) {
        nameWrapper = th.find('div');
    }
    var prevVal = nameWrapper.text().trim();
    var curVal = reNameForm.val().trim();
    if (prevVal != curVal) {
        showProgress();
        var viewName = views[guid].ViewName;
        $.ajax({
            url: rootPath + "Admin/RenameField/" + viewName,
            data: { viewName: viewName, fieldName: fieldName, NewDisplayName: curVal },
            contentType: 'application/html; charset=utf-8',
            async: false,
            cache: false,
            error: ajaxErrorsHandler
        });

        nameWrapper.text(curVal);
        //Durados.GridHandler.setHeaderWidth(th);
        hideProgress();
    }
    reNameForm.hide();
    nameWrapper.css('visibility', 'visible');
    renamedFieldInfo = {};
}

function resetRenamedField() {
    if (!renamedFieldInfo) return;
    var reNameForm = $('#reNameForm');
    if (reNameForm.css('display') != 'inline') return;
    //$.each(contextInfo, function(guid, value) {
    var th = renamedFieldInfo.targetElement;
    var nameWrapper = th.find('A');
    if (!nameWrapper.length) {
        nameWrapper = th.find('div');
    }
    reNameForm.hide();
    nameWrapper.css('visibility', 'visible');
    //});
    renamedFieldInfo = {};
}

function selectField(viewName, fieldName, guid, selectFieldCallback, data, filter) {
    if (fieldsDialog) {
        showProgress();
        setTimeout(function () {
            fieldsDialog.dialog('open');
            hideProgress();
        }, 1);
    }
    else {
        showProgress();
        setTimeout(function () {
            var url = rootPath + 'Admin/Fields/' + viewName + '?viewName=' + viewName + '&filter=' + filter;
            fieldsDialog = SearchDialog.CreateAndOpen(viewName, "Select Column", '', '', url, guid, null, null, selectFieldCallback, data);
            hideProgress();
        }, 10);
    }
}

function unhideFieldCallback(data, pks, guid, searchGuid, type, id, dialog) {
    if (pks.length == 1)
        unhideField(views[guid].ViewName, contextInfo[guid].fieldName, guid, pks[0], rootPath + "Admin/UnhideField/" + views[guid].ViewName);
}

//function selectFieldCallback(data, pks, guid, searchGuid, type, id, dialog) {
//    if (pks.length == 1)
//        moveFieldTo(views[guid].ViewName, contextInfo[guid].fieldName, contextInfo[guid].before, guid, pks[0], rootPath + "Admin/MoveField/" + views[guid].ViewName);
//}

function unhideField(viewName, fieldName, guid, pk, unhideFieldUrl) {
    showProgress();

    $.ajax({
        url: unhideFieldUrl,
        data: { viewName: viewName, fieldName: fieldName, guid: guid, pk: pk },
        contentType: 'application/html; charset=utf-8',
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            var url = rootPath + 'Admin/RefreshConfig/';
            refreshConfig(url, guid);
            fieldsDialog = null;

            hideProgress();
        }
    });

}

function moveFieldTo(viewName, fieldName, before, guid, to, moveFieldUrl) {
    showProgress();

    $.ajax({
        url: moveFieldUrl,
        data: { viewName: viewName, fieldName: fieldName, before: before, guid: guid, to: to },
        contentType: 'application/html; charset=utf-8',
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            hideProgress();
        }
    });

}

var FloatingDialog; if (!FloatingDialog) FloatingDialog = function () { };

FloatingDialog.Show = function (url, title, data, modal, guid, post, callback, width, height, position) {
    div = $("<div class='FloatingDialogDiv'>");
    $("body").prepend(div);
    if (url != null && url != '') {
        if (post) {
            FloatingDialog.CreatePost(url, data, guid, div, callback, width, height);
        }
        else {
            div.html(FloatingDialog.Create(url, data));
            if (modal == null)
                modal = false;
        }
    }
    FloatingDialog.Open(div, title, modal, guid, width, height, position);

    return div;
}


FloatingDialog.Create = function (url, data) {
    var syncHtml = '';
    if (data == null)
        data = {};
    $.ajax({
        url: url,
        contentType: 'application/html; charset=utf-8',
        data: data,
        async: false,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                syncHtml = html;
            }
            else {
                ajaxNotSuccessMsg(html);
            }
        }

    });

    return syncHtml;
}

FloatingDialog.CreatePost = function (url, data, guid, div, callback) {
    if (data == null)
        data = {};

    $.post(url,
        data,
        function (html) {
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                div.html(html);
                if (div.attr('adding') == 'hide') { //td.tablecommand
                    div.find('a').slice(0, 2).each(function () {
                        $(this).hide(); $(this).next('span').hide();
                    });
                }
                initDataTableView(guid);
                if (callback) callback(guid, div);
                hideProgress();
            }
            else {
                hideProgress();
                ajaxNotSuccessMsg(html);
            }
        });


}


FloatingDialog.Open = function (dialog, title, modal, guid, width, height, position) {
    showProgress();

    var winWidth = $(window).width();
    var winHeight = $(window).height();

    if (!width)
        width = winWidth * .75;

    if (!height)
        height = winHeight * .75;

    if (!position)
        position = 'center';

    dialog.attr("guid", guid);

    currentViewName = "float";
    var rec = Rectangle.Load("float");
    //var dialog = currentDialog.Div;

    if (rec != null) {
        dialog.dialog({
            bgiframe: true,
            autoOpen: false,
            position: [rec.left, rec.top],
            width: rec.width,
            height: rec.height,
            title: title,
            modal: modal,
            resizeStop: SaveFloatDialogOnResize,
            zIndex: 100000,
            dragStop: SaveFloatDialogOnDrag,
            open: function (event, ui) {
                var state = $.cookie("state_" + currentViewName);
                if (state == 'max' || IE7) {
                    dialogExt.max(dialog, null, guid);
                }
                currentDialog = dialog;

            }

        });

    }
    else {
        dialog.dialog({
            bgiframe: true,
            autoOpen: false,
            width: width,
            height: height,
            title: title,
            modal: modal,
            position: position,
            resizeStop: SaveFloatDialogOnResize,
            zIndex: 100000,
            dragStop: SaveFloatDialogOnDrag,
            open: function (event, ui) {
                var state = $.cookie("state_" + currentViewName);
                if (state == 'max' || IE7) {
                    dialogExt.max(dialog, null, guid);
                }
                currentDialog = dialog;
            }
        });
    }

    dialogExt.initMaxButton(dialog, guid);

    var state = $.cookie("state_" + currentViewName);
    if (state != "max") {
        if (rec != null) {
            dialog.dialog("option", "position", [rec.left, rec.top]);
        }
    }
    dialog.dialog({
        show: { effect: 'fold', duration: 500 },
        hide: { effect: 'fold', duration: 500 }
    });
    dialog.dialog('open');

    FloatingDialog.TheDialog = dialog;

    //hideProgress();
}

function loadFilterParameters(elm, filterParameters, guid) {
    var dialog = $(elm).parents('form:first');
    var result = filterParameters;

    dialog.find('input').each(function () {
        var input = $(this);
        var name = input.attr('name');
        var val = input.val();
        if (input.attr('class') == 'Autocomplete') {
            val = GetAutoCompleteValueId(input);
        }
        if (name != null && name != '')
            result = result.replace('$' + name, val);
    });

    dialog.find('select').each(function () {
        var select = $(this);
        var name = select.attr('name');
        var val = select.val();
        if (name != null && name != '')
            result = result.replace('$' + name, val);
    });

    return result;
}

var SearchDialog; if (!SearchDialog) SearchDialog = function () { };

SearchDialog.GetButtons = function (guid, searchGuid, type, id, dialog, callback, data) {
    var buttons = {};  //initialize the object to hold my buttons

    buttons[translator.select] = function () { SearchDialog.Select(guid, searchGuid, type, id, dialog, callback, data); }  //the function that does the save
    buttons[translator.cancel] = function () {
        dialog.dialog('close');
    }  //the function that does the save
    buttons[translator.select] = function () { SearchDialog.Select(guid, searchGuid, type, id, dialog, callback, data); }  //the function that does the save

    return buttons;
}

var searchGuidPostfix = 0;

SearchDialog.CreateAndOpen = function (viewName, viewDisplay, type, id, searchUrl, guid, elm, filterParameters, callback, data) {
    begin();
    setTimeout(function () { SearchDialog.CreateAndOpen2(viewName, viewDisplay, type, id, searchUrl, guid, elm, filterParameters, callback, data); }
, 10);
}


SearchDialog.CreateAndOpen2 = function (viewName, viewDisplay, type, id, searchUrl, guid, elm, filterParameters, callback, data) {
    searchGuidPostfix++;
    searchGuid = guid + searchGuidPostfix;
    //    var select = $('#' + id);
    //    var isDependent = select.attr('hasInsideTrigger') == 'hasInsideTrigger';
    //    if (isDependent) {
    //        var prevDialog = select.parents('form:first');
    //        var triggerName = select.attr('triggerName');
    //        var triggerField = prevDialog.find('select[name="' + triggerName + '"]');
    //        var triggerVal = triggerField.val();
    //        searchUrl = searchUrl.replace('$$', triggerVal);
    //    }
    var d_filter = '';
    if (filterParameters && filterParameters != '')
        d_filter = '&d_filter=' + loadFilterParameters(elm, filterParameters, guid);
    var dialog = FloatingDialog.Show(searchUrl + "&guid=" + searchGuid + d_filter + "&checkbox=true" , "Search " + viewDisplay, null, true);
    var buttons = SearchDialog.GetButtons(guid, searchGuid, type, id, dialog, callback, data);
    dialog.dialog("option", "buttons", buttons);

    initDataTableView(searchGuid);

    var multiSelect = type == "CheckList";
    views[searchGuid].MultiSelect = multiSelect;

    return dialog;
}

SearchDialog.Select = function (guid, searchGuid, type, id, dialog, callback, data) {
    var pks = Multi.GetSelection(searchGuid);

    if (!pks.length) {
        ajaxNotSuccessMsg('Please select a row(s)');
        return;
    }

    if (type == "DropDown") {
        var select = $('#' + id);
        select.val(pks[0]);
        select.change();
        var afterSearchSelectedArgs = { guid: guid, pk: pks[0], fieldName: select.attr('name') };
        $(SearchDialog).trigger('afterSearchSelected', afterSearchSelectedArgs);
    }
    else if (type == "Autocomplete") {
        var value = pks[0];
        var text = displayValues[searchGuid];
        if (text != null) {
            var input = $('#' + id);
            input.val(text);
            input.attr('valueId', value);
            var afterSearchSelectedArgs = { guid: guid, pk: value, fieldName: input.attr('name') };
            $(SearchDialog).trigger('afterSearchSelected', afterSearchSelectedArgs);
            input.focus();

        }
    }
    else if (type == "CheckList") {
        var select = $('#' + id);
        var values = select.val();

        if (values == null) {
            values = [];
        }
        $(pks).each(function () {
            values.push(String(this));
        });
        select.val(values);
        select.dropdownchecklist("refresh");
    }
    else {
        if (callback)
            callback(data, pks, guid, searchGuid, type, id, dialog);
    }


    dialog.dialog('close');
}

function reorder(guid, dir, vieweName, url) {
    var selection = Multi.GetSelection(guid);
    if (selection.length == 0) {
        ajaxNotSuccessMsg('Please select a row');
        return;
    }
    if (selection.length > 1) {
        ajaxNotSuccessMsg('Please select only one row');
        return;
    }
    var o_pk = selection[0];
    var o_tr = $('#d_row_' + guid + o_pk);
    var d_tr = null
    if (dir == 'up') {
        d_tr = o_tr.prev();
        if (d_tr.length == 1 && d_tr.attr('d_row') == 'd_row') {
            o_tr.insertBefore(d_tr);
            var d_pk = d_tr.attr('d_pk');
            reorderOnServer(guid, o_pk, d_pk, vieweName, url);
        }
    }
    else if (dir == 'down') {
        d_tr = o_tr.next();
        if (d_tr.length == 1 && d_tr.attr('d_row') == 'd_row') {
            o_tr.insertAfter(d_tr);
            var d_pk = d_tr.attr('d_pk');
            reorderOnServer(guid, o_pk, d_pk, vieweName, url);
        }
    }
    return false;
}

function reorderOnServer(guid, o_pk, d_pk, viewName, url) {
    $.ajax({
        url: url,
        contentType: 'application/html; charset=utf-8',
        data: { o_pk: o_pk, d_pk: d_pk, guid: guid },
        async: true,
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            var displayType = views[guid].DataDisplayType;

            if (displayType == "Table") {
                if (json == 'success') {
                    refreshView(guid);
                    loadMainMenu(guid);
                }
                else {
                    ajaxNotSuccessMsg(json);
                }
            }
        }

    });
}

var Excel; if (!Excel) Excel = function () { };

Excel.Import = function (viewName, guid, isMergReplaceImport, afterImportCallback, addPageDialog, pageName) {
    var d = d_Dialog.CreateAndOpen(viewName, guid, "durados_Import", "Import");
   /* if ($.browser.mozilla) {
        if (d.find('.ff-warning').length == 0) {
            d.prepend($("<div class='ff-warning'><div>Not supported in Firefox.</div><div>Import your Excel file using Chrome or Internet Explorer.</div></div>"));
        }
    }*/
    var selectImportMode = $(d_Dialog).find('select[name="ImportMode"]');
    var removedOptions = 0;

    try {
        if (guid != null && views[guid] != null) {
            selectImportMode.find('option').each(function () {
                switch ($(this).val()) {
                    case "1":
                        if (!views[guid].AllowCreate) {
                            $(this).remove();
                            removedOptions++;
                        }
                        break;
                    case "2":
                        if (!views[guid].AllowEdit) {
                            $(this).remove();
                            removedOptions++;
                        }
                        break;
                    case "3":
                        if (!views[guid].AllowCreate || !views[guid].AllowEdit) {
                            $(this).remove();
                            removedOptions++;
                        }
                        break;

                }
            });
        }
    }
    catch (err) {
    }

    if (removedOptions == 3) {
        selectImportMode.parent('td').hide();
        selectImportMode.parent('td').prev().hide();
    }
    else {
        selectImportMode.parent('td').show();
        selectImportMode.parent('td').prev().show();
    }

    if ($(d_Dialog).data('events').PerformMethod) {
        $(d_Dialog).unbind('PerformMethod');
    }
    $(d_Dialog).bind('PerformMethod', function (e, data) {

        var fileName = data.dialog.find('#' + guid + inlineAddingPrefix + "FileName").val();
        var sheetName = '';
        var sheetNameElm = data.dialog.find('#' + guid + inlineAddingPrefix + "SheetName");
        if (sheetNameElm.length == 1) {
            sheetName = sheetNameElm.val();
        }
        var writeErrors = true;
        var writeErrorsElm = data.dialog.find('#' + guid + inlineAddingPrefix + "WriteErrors");
        if (writeErrorsElm.length == 1) {
            writeErrors = Durados.CheckBox.IsChecked(writeErrorsElm);
        }

        var RollBackOnError = true;
        var RollBackOnErrorElm = data.dialog.find('#' + guid + inlineAddingPrefix + "RollBackOnError");
        if (RollBackOnErrorElm.length == 1) {
            RollBackOnError = Durados.CheckBox.IsChecked(RollBackOnErrorElm);
        }

        var ImportMode = 4;
        var ImportModeElm = data.dialog.find('#' + guid + inlineAddingPrefix + "ImportMode");
        if (ImportModeElm.length == 1) {
            ImportMode = parseInt(ImportModeElm.val());
        }

        if (fileName == "") {
            ajaxNotSuccessMsg("Please upload excel file");
            return;
        }

        if (!ImportMode && removedOptions < 3) {
            ajaxNotSuccessMsg("Please select import mode");
            return;
        }

        if (!ImportMode)
            ImportMode = 1; //insert
        var importType = data.dialog.find("input:radio[name=importType]:checked").val();

        showProgress();

        var url = rootPath + (views[guid] == null ? 'Admin' : views[guid].Controller) + '/Import/' + viewName;

        $.post(url,
         {
             fileName: fileName, sheetName: sheetName, writeErrors: writeErrors, rollBackOnError: RollBackOnError, ImportModeIndex: ImportMode, importType: importType, pageName: pageName
         },
        function (json) {
            hideProgress();
            if (!json.success) {
                //ajaxNotSuccessMsg(json.message);
                Durados.Dialogs.Alert("Import Finished", json.message);

            } else {
                if (afterImportCallback) {
                    afterImportCallback(addPageDialog, json.viewName);
                }
                //modalSuccessMsg(json.message);
                Durados.Dialogs.Alert("Import Finished", json.message, function () {
                    if (!json.hasErrors) {
                        data.dialog.dialog("close");
                    }
                });
                refreshMainContent();
            }
        });

    });

    $(d_Dialog).find("div[name=ImportAddTemplate] a").attr("href", "/Home/ExportToCsv/" + viewName + "?guid=" + guid + "&NoData=true");
    $(d_Dialog).find("div[name=ImportEditTemplate] a").attr("href", "/Home/ExportToCsv/" + viewName + "?guid=" + guid + "&NoData=false");

    var isViewOwner = guid == null || views[guid] == null ? true : views[guid].Role == "View Owner";

    isMergReplaceImport = isMergReplaceImport == null ? false : isMergReplaceImport;

    if (isViewOwner)
        $(d_Dialog).find("div[name=ImportEditTemplate]").hide();

    if (isViewOwner && !isMergReplaceImport)
        $(d_Dialog).find("div[name=importTypeDiv]").hide();

    hideProgress();
}


var d_Dialog; if (!d_Dialog) d_Dialog = function () { };

d_Dialog.CreateAndOpen = function (viewName, guid, method, title) {
    showProgress();

    var html = CreateDialog.Create(method, guid);

    var div;

    var dialogId = guid + viewName + method;
    div = $('#' + dialogId);

    if (div.length == 0) {
        div = $("<div>");
        div.attr('id', dialogId);
        $("body").prepend(div);
    }
    div.html(html);

    var cache = false;
    if (guid)
        cache = views[guid].TabCache == 'True';

    var createTab = div.find('#' + guid + 'InlineAddingTabs');

    createTab.tabs({ fx: { opacity: 'toggle', duration: 'fast' }, cache: cache, ajaxOptions: { cache: false, async: false} }); // first tab selected
    disableChildrenTabs(createTab);


    d_Dialog.Open(viewName, guid, method, div, title);

    if (guid) {
        complete(guid);
        initDropdownChecklistsCreate(guid);
    }

    return div;
}

d_Dialog.Open = function (viewName, guid, method, dialog, title) {

    var winWidth = $(window).width();
    var winHeight = $(window).height();

    currentViewName = method;

    var rec = Rectangle.Load(currentViewName);
    //var dialog = currentDialog.Div;

    if (rec != null) {
        dialog.dialog({
            bgiframe: true,
            autoOpen: false,
            position: [rec.left, rec.top],
            width: rec.width,
            height: rec.height,
            modal: true,
            close: CreateDialog.CloseEvent,
            resizeStop: SaveDialogOnResize,
            zIndex: 10000,
            dragStop: SaveDialogOnDrag

        });

    }
    else {
        dialog.dialog({
            bgiframe: true,
            autoOpen: false,
            width: (winWidth * .75),
            modal: true,
            close: CreateDialog.CloseEvent,
            position: 'center',
            resizeStop: SaveDialogOnResize,
            zIndex: 10000,
            dragStop: SaveDialogOnDrag
        });
    }

    dialog.dialog("option", "title", title);
    dialog.dialog("option", "buttons", {
        "Run": function () { d_Dialog.PerformMethod(viewName, guid, dialog); },
        "Close": function () { CreateDialog.Close(dialog, guid); }
    });

    showProgress();

    json = GetJsonViewForInlineAdding(method, guid);
    if (guid) {
        var temp = views[guid].gViewName;
        views[guid].gViewName = method;
        AddDialog.SetDefaults(json, inlineAddingPrefix, guid);
        views[guid].gViewName = temp;
    }
    else {
        AddDialog.SetDefaults(json, inlineAddingPrefix, 'null');
    }
    if (rec != null) {
        dialog.dialog("option", "position", [rec.left, rec.top]);

    }

    if (guid) {
        InitValidation(guid, inlineAddingPrefix, json);
    }

    //    initDerivation(json.Derivation, dialog, inlineAddingPrefix, guid, json.ViewName);
    //    initDerivationOnShow(json.Derivation, dialog, inlineAddingPrefix, guid, json.ViewName);

    dialog.dialog({
        show: { effect: 'fold', duration: 500 },
        hide: { effect: 'fold', duration: 500 }
    });
    dialog.dialog('open');

    //    CreateDialog.CreateWysiwyg(guid, viewName);
    hideProgress();


    //    $('input:visible:enabled:first').focus();

}

d_Dialog.PerformMethod = function (viewName, guid, dialog) {
    $(d_Dialog).trigger('PerformMethod', { viewName: viewName, guid: guid, dialog: dialog });
}

function showCopyDialog(guid, a) {

    var winWidth = $(window).width();
    var winHeight = $(window).height();

    var dialog = $('#copyConfigDialog');
    dialog.dialog({
        bgiframe: true,
        autoOpen: false,
        position: 'center',
        width: (winWidth * .75),
        modal: true,
        close: CreateDialog.CloseEvent,
        position: 'center',
        resizeStop: SaveDialogOnResize,
        zIndex: 10000,
        dragStop: SaveDialogOnDrag
    });

    dialog.dialog("option", "title", "Copy Cnfiguration");
    dialog.dialog("option", "buttons", {
        "OK": function () { copyConfig(guid, a, dialog); },
        "Cancel": function () { dialog.dialog('close'); }
    });

    dialog.dialog('open');

}

function copyConfig(guid, a, dialog) {
    var sourceView = $('#sourceView').val();
    var destView = $('#destView').val();

    if (sourceView == null || sourceView == '') {
        ajaxNotSuccessMsg("Please enter source view.");
        return;
    }

    if (destView == null || destView == '') {
        ajaxNotSuccessMsg("Please enter destination view.");
        return;
    }

    $.ajax({
        url: gVD + 'Durados/Copy/' + destView,
        contentType: 'application/json; charset=utf-8',
        data: { templateView: sourceView },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            modalSuccessMsg(json);
            dialog.dialog('close');
        }

    });
}

function showCloneDialog(guid) {

    var winWidth = $(window).width();
    var winHeight = $(window).height();

    var dialog = $('#cloneViewDialog');
    dialog.dialog({
        bgiframe: true,
        autoOpen: false,
        position: 'center',
        width: (winWidth * .75),
        modal: true,
        close: CreateDialog.CloseEvent,
        position: 'center',
        resizeStop: SaveDialogOnResize,
        zIndex: 10000,
        dragStop: SaveDialogOnDrag
    });

    dialog.dialog("option", "title", "Clone View");
    dialog.dialog("option", "buttons", {
        "OK": function () { cloneView(guid, dialog); },
        "Cancel": function () { dialog.dialog('close'); }
    });

    dialog.dialog('open');

}

function cloneView(guid, dialog) {
    var baseViewName = $('#baseViewName').val();
    var clonedViewName = $('#clonedViewName').val();

    if (baseViewName == null || baseViewName == '') {
        modalErrorMsg("Please enter base view.");
        return;
    }

    if (clonedViewName == null || clonedViewName == '') {
        modalErrorMsg("Please enter the cloned view name.");
        return;
    }

    $.ajax({
        url: gVD + 'Durados/Clone/' + baseViewName,
        contentType: 'application/json; charset=utf-8',
        data: { clonedViewName: clonedViewName },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            alert(json);
            dialog.dialog('close');
        }

    });
}

function refreshConfig(url, guid, hideMessage) {
    var fieldOrder = $("#" + guid + 'ajaxDiv').find('select[name="d_fieldOrder"]').val();

    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { fieldOrder: fieldOrder },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            if (hideMessage)
                return;
            refreshConfigSuccess(guid, fieldOrder);
            modalSuccessMsg(json);
        }

    });
}


function refreshConfigSuccess(guid, fieldOrder) {

    $.post(views[guid].gIndexUrl,
    {
        SortColumn: fieldOrder, direction: 'Asc', guid: guid
    },
    function (html) {
        hideProgress();
        var index = html.indexOf("$$error$$", 0)
        if (index < 0 || index > 1000) {
            FilterForm.HandleSuccess(html, false, guid, null);
            prevSortedColumn = fieldOrder;
            prevSortedDirection = 'Asc';
            InitChangeFieldOrder(guid);
        }
        else {
            //FilterForm.ShowFailureMessage(html.replace("$$error$$", ""), guid);
            ajaxNotSuccessMsg(html, guid);
        }
    });

}

function copyOrder(url, guid) {
    var selection = Multi.GetSelection(guid);
    if (selection.length == 0) {
        ajaxNotSuccessMsg('Please select a row');
        return;
    }

    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { fieldPK: selection[0], guid: guid },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            modalSuccessMsg(json);
        }

    });
}

function repairOrder(url, guid) {
    var selection = Multi.GetSelection(guid);
    if (selection.length == 0) {
        ajaxNotSuccessMsg('Please select a row');
        return;
    }

    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: 'View', pk: selection[0], guid: guid },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            modalSuccessMsg(json);
        }

    });
}


function runReport(guid, viewName) {
    if (!AddDialog.isValid(guid, viewName))
        return;

    showProgress();

    var JsonFilter;

    JsonFilter = FillJson(GetJsonFilter(guid), createPrefix, guid);
    //    var mainPage = views[guid].mainPage == 'True';
    //    
    //    $.ajax({
    //      type: "POST",
    //      contentType: "application/json; charset=utf-8",
    //      dataType: "html",
    //      async: false,
    //      url: views[guid].gFilterUrl,
    //      data : 
    //      {
    //        jsonFilter: Sys.Serialization.JavaScriptSerializer.serialize(JsonFilter), guid: guid, search: '', mainPage: mainPage
    //      },
    //      success: function(html) {
    //          hideProgress();
    //          var index = html.indexOf("$$error$$", 0)
    //          if (index < 0 || index > 1000) {
    //              FilterForm.HandleSuccess(html, false, guid);
    //          }
    //          else {
    //              FilterForm.ShowFailureMessage(html.replace("$$error$$", ""), guid);
    //          }
    //      },
    //      error: function(xhr, status, err) {
    //        // There has to be a better way to do this!!
    //        //var title = xhr.responseText.split("&lt;title&gt;")[1].split("&lt;/title&gt;")[0];
    //        alert("aaa");
    //      }
    //    });


    $.post(views[guid].gFilterUrl,
    {
        jsonFilter: Sys.Serialization.JavaScriptSerializer.serialize(JsonFilter), guid: guid, search: '', mainPage: views[guid].mainPage
    },
    function (html) {
        hideProgress();
        var index = html.indexOf("$$error start$$", 0)
        if (index < 0 || index > 100000) {
            FilterForm.HandleSuccess(html, false, guid, null);
        }
        else {
            //FilterForm.ShowFailureMessage(html.replace("$$error$$", ""), guid);
            var index2 = html.indexOf("$$error end$$", 0)
            ajaxNotSuccessMsg(html.substring(index + 15, index2), guid);
        }
    });
}

var historyGuidID = 0;
var currentHistoryJson = null;

function History(url, guid, historyGuid, viewName) {
    var pks = Multi.GetSelection(guid);
    if (pks.length > 1) {
        modalErrorMsg('Please select only one row');
        return;
    }
    historyGuidID++;
    historyGuid = historyGuid + historyGuidID;
    var json = getHistoryFilter(url, guid, viewName);
    currentHistoryJson = json;

    //$(FilterForm).bind("JsonFilter", function(event, data) {
    //    data.replace = true;
    //    data.JsonFilter = json;
    //});


    FloatingDialog.Show(url, "History Log", { jsonFilter: Sys.Serialization.JavaScriptSerializer.serialize(json), guid: historyGuid }, true, historyGuid, true);
    //initDataTableView(historyGuid);
}

function menuOrganizer(url, title) {
    try {
        FloatingDialog.Show(url, title + ' - Assign Views', null, true, 'menuOrganizer_guid_', true, function (guid, dialog) {
            var viewPort = dialog.find('div.fixedViewPort').first();
            
            var toolbar = $('<div></div>');

            var menuButton = $('<a href="#"><span class="menu-popup">Organize Menus</span></a>');

            menuButton.click(function () {
                var url = '/Admin/IndexPage/Menu?guid=Menu_c20_&mainPage=True&menu=off';
                FloatingDialog.Show(url, title + ' - Organize Menus', null, true, 'menu_guid_', true, null, 800, 'auto');
                return false;
            });

            toolbar.append(menuButton);

            viewPort.prepend(toolbar);

            var table = viewPort.find('table:first');

            var firstRow = table.find('tr[d_row="d_row"]:first');

            if (firstRow.length == 0)
                return;

            var colspan = firstRow.children('td').length;
            var height = 24; //firstRow.css('height')

            var getMenuRow = function (name) {
                var tr = $('<tr class="menu-organizer-header" d_row="d_row"></tr>');
                var td = $('<td colspan="' + colspan + '"></td>');
                tr.append(td);
                tr.css('height', height);
                td.text(name);
                return tr;
            }

            var hideInMenu = 'Not in Menu';
            var rootMenu = 'Top Menu';

            var curMenu = hideInMenu;
            table.prepend(getMenuRow(curMenu));

            table.find('tr').each(function () {
                var tr = $(this);
                var hidden = tr.attr('d_hideinmenu');
                var menu = tr.attr('d_menu_parent');
                if (hidden == "No" && menu == '' && curMenu == hideInMenu) {
                    curMenu = rootMenu;
                    tr.before(getMenuRow(curMenu));
                }
                else if (hidden == "No" && menu != '' && menu != curMenu && (curMenu == hideInMenu || curMenu == rootMenu)) {
                    curMenu = menu;
                    tr.before(getMenuRow(curMenu));
                }
            });

            //dialog.dialog({ width: 600 });


        }, 600, 'auto');
//        setTimeout(function () {
//          $('div.FloatingDialogDiv[guid="menuOrganizer_guid_"]').parent().css("zIndex",100000);
//        }, 30000);
//    
    }
    catch (err) {
    }
    hideProgress();
}

function getHistoryFilter(url, guid, viewName) {
    var json = { "Fields": [{ "Key": "ActionHistory_Parent", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "ActionHistory_Parent", "Permanent": false, "Required": false, "Searchable": false, "Type": "DropDown", "ValidationType": null, "Value": null} }, { "Key": "HistoryUsers_Parent", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "HistoryUsers_Parent", "Permanent": false, "Required": false, "Searchable": false, "Type": "DropDown", "ValidationType": null, "Value": null} }, { "Key": "ViewName", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "ViewName", "Permanent": false, "Required": false, "Searchable": true, "Type": "Text", "ValidationType": null, "Value": null} }, { "Key": "PK", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "PK", "Permanent": false, "Required": false, "Searchable": true, "Type": "TextArea", "ValidationType": null, "Value": null} }, { "Key": "UpdateDate", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "UpdateDate", "Permanent": false, "Required": false, "Searchable": false, "Type": "Text", "ValidationType": null, "Value": null} }, { "Key": "FieldName", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "FieldName", "Permanent": false, "Required": false, "Searchable": true, "Type": "TextArea", "ValidationType": null, "Value": null} }, { "Key": "ColumnNames", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "ColumnNames", "Permanent": false, "Required": false, "Searchable": false, "Type": "TextArea", "ValidationType": null, "Value": null} }, { "Key": "OldValue", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "OldValue", "Permanent": false, "Required": false, "Searchable": true, "Type": "Text", "ValidationType": null, "Value": null} }, { "Key": "NewValue", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "NewValue", "Permanent": false, "Required": false, "Searchable": true, "Type": "Text", "ValidationType": null, "Value": null} }, { "Key": "Id", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "Id", "Permanent": false, "Required": false, "Searchable": false, "Type": "Text", "ValidationType": null, "Value": null} }, { "Key": "ChangeHistoryId", "Value": { "Default": null, "DependencyChildren": null, "Format": null, "Min": 0, "Max": 0, "Name": "ChangeHistoryId", "Permanent": false, "Required": false, "Searchable": false, "Type": "Text", "ValidationType": null, "Value": null}}] };

    json.Fields[2].Value.Value = viewName;
    var pks = Multi.GetSelection(guid);
    if (pks.length == 1) {
        var pk = pks[0];
        json.Fields[3].Value.Value = pk;
    }

    return json;
}

function ctrlNavigate(element, e) {
    var url = $(element).attr('d_url');
    var gridEditing = $(element).parent('TD.d_fieldContainer').attr('d_role') == 'cell';
    if (gridEditing && !e.ctrlKey)
        return false;

    if (e.ctrlKey)
        window.open(url, "_blank");
    else
        window.location.href = url;
    return false;
}


function sendConfig(url) {
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: modalSuccessMsg
    });
}

//added by yossi for debugs
function debugit(msg) {
    $('#y-debug').html($('#y-debug').html() + msg + ' / ');
}

var hideAjaxErrorMsg = false;

function ajaxErrorsHandler(XMLHttpRequest, textStatus, errorThrown) {

    hideProgress();

    //TODO set errors messages by status
    //http://msdn.microsoft.com/en-us/library/ms767625(v=vs.85).aspx
    //http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html

    var msg = "";

    try {

        if (XMLHttpRequest.readyState != 4) {
            msg = "Request failed!";
            return;
        } else if (XMLHttpRequest.status == 501) {
            msg = "Internal Server Error!";
        } else if (XMLHttpRequest.status == 404) {
            msg = "Page Not Found - The server is down!";
        } else if (XMLHttpRequest.status == 403) {
            msg = "Forbidden - You are not allowed to do this action (or session timeout)!";
        } else if (XMLHttpRequest.status == 401) {
            msg = "Session expired!";
            var url_to = "/Account/LogOn?ReturnUrl=";
            if (XMLHttpRequest.responseText && XMLHttpRequest.responseText.length < 200) {
                url_to += XMLHttpRequest.responseText;
            }
            top.location.href = url_to;

        } else {
            msg = translator.GeneralErrorMessage; //"Temporary busy server.<BR>Please try again or contact the system administrator";
        }

    } catch (ex) {
        msg = translator.GeneralErrorMessage; //"Temporary busy server.<BR>Please try again or contact the system administrator";
    }

    //for debug:
    //msg = "Status: " + textStatus + " <br /><br />----------<br /><br /> Error: " + errorThrown;

    if (!hideAjaxErrorMsg)
        modalErrorMsg(msg);
}

function ajaxNotSuccessMsg(msg, guid, notComplete) {

    if (!msg) { msg = "Data didn't loaded successfully!" };

    msg = msg.replace("$$error$$", "");

    //alert("New ---- "+msg);

    if (guid) {
        if (!notComplete)
            complete(guid);
        //initDropdownchecklists(guid);

    }

    msg = msg.replace("/n", "<br /><br />");
    //msg = msg + "Please contact the system administrator";

    var winHeight = $(window).height();

    $('#modal_errors').dialog("option", "title", translator.SystemInfoMsg);

    modalErrorMsg(msg);
    var mdlErrDivHeight = $('#modal_errors').height();

    if (mdlErrDivHeight > winHeight - 10) {
        $('#modal_errors').dialog("option", "height", winHeight - 10);
    }


}

function modalSuccessMsg(msg) {

    $('#modal_errors').dialog("option", "title", translator.SystemInfoMsg);
    $('#modal_errors').dialog("option", "height", "auto");
    showMsgInModalDialog(msg);

}

function modalErrorMsg(msg) {

    $('#modal_errors').dialog("option", "title", translator.SystemInfoMsg);
    $('#modal_errors').dialog("option", "height", "auto");
    if (msg.replace(/\s/g, '') != "m_CancelError") {
        showMsgInModalDialog(msg);
    }

}


function showMsgInModalDialog(msg) {
    hideProgress();

    var dialog = $('#modal_errors');
    dialog.html(msg);
    handleMoreInfo(dialog, msg);
    dialog.dialog('open');
}

function handleMoreInfo(dialog, msg) {
    var link = dialog.find('.additional-info-link');
    var icon = dialog.find('.additional-info-icon');
    if (link.length != 1)
        return;

    var div = dialog.find('.additional-info');
    div.hide();
    icon.removeClass('additional-info-icon-col');
    icon.addClass('additional-info-icon-exp');

    dialog.find('.additional-info-link,.additional-info-icon').click(function () {
        if (div.is(':visible')) {
            div.hide();
            icon.removeClass('additional-info-icon-exp');
            icon.addClass('additional-info-icon-col');
        }
        else {
            div.show();
            icon.removeClass('additional-info-icon-col');
            icon.addClass('additional-info-icon-exp');
        }
    });
}

function adjustFreeBg(guid) {
    var viewPort = $('#' + guid + 'ajaxDiv div.fixedViewPort').first();

    if (viewPort.length == 0) { return; }

    var data = $('#' + guid + 'ajaxDiv table.gridview').first(); //viewPort Child?

    if (data.length == 0) { return; }

    var img = viewPort.find(".freeBg:last"); //viewPort Child?

    if (img.length == 0) { return; }
    img.show();
    var lastTD = data.find('td:last');
    if (lastTD.length != 1) {
        return;
    }
    var ih = img.height();
    if (ih == 0)
        ih = 45;
    var h = $('.fixedViewPort').position().top + $('.fixedViewPort').height() - ih - 2;
    var hpx = h + "px";
    img.css('top', hpx);

    return data.height() + ', ' + img.height();
}

var toResize;
var dialogResize;

$(document).ready(function () {

    resizeWindow();

    mainMenu();

    try {
        $('<div id="modal_errors"></div>')
		.dialog({
		    autoOpen: false,
		    position: 'center',
		    width: ($(window).width() * 0.5),
		    modal: true,
		    position: 'center',
		    zIndex: 99999999,
		    title: 'System error message',
		    buttons: {
		        "Close": function () { $('#modal_errors').dialog('close'); hideProgress(); }
		    }
		});
    }
    catch (err) {
    }

    initSplitters();

    FilterForm.InitTreeFilter();

    $(window).resize(function () {
        resizeWindow();
    });

    $(document).bind('keyup', function (event) {
        if (event.keyCode == $.ui.keyCode.ESCAPE) {
            Durados.GridHandler.resetHeadersMoveToClass();
            resetRenamedField();
        }
    });

    //$.browser.mozilla
    if ($.browser.webkit) {
        $('body').addClass('webkit');
    }

    if (inIframe()) {
        $('.main-content').addClass('in-iframe');
    }
    logTitle.initiate();

    //Checkboxs	
    //$('input:checkbox:not([safari])').checkbox();
    //$('input[safari]:checkbox').checkbox({ cls: 'jquery-safari-checkbox' });
    //$('input:radio').checkbox();

    if (!isMobile()) {
        try {
            initRotateMenu();
        }
        catch (err) { }
    }

    if (queryString("settings") == "fields" || queryString("settings") == "view") {
        //setTimeout(function () {
        $('#rowtabletitleSpan').click();
        //}, 1000);
    }

    var b = false;
    var p = IsFieldsSettings();
    window.onbeforeunload = function () {
        if (adminPreviewChanged) {
            b = true;
        }
        try {
            CancelPreview(null);
        }
        catch (err) {
            return;
        }
        if (b && p)
            return "You have attempted to leave this page.  If you have made any changes to the fields without clicking the Save button, your changes will be lost.";
    }

});

function mainMenu() {
    $("#sideMenu ul li.selected3").prev().css('border-bottom', 'none');
    $("#sideMenu ul li:not(:first-child)").click(function () {
        $("#sideMenu ul li").css('border-bottom', '1px solid #d3d3d3');
        $('#sideMenu ul li.selected3').removeClass();
        $(this).closest('li').addClass("selected3").prev().css('border-bottom', 'none');
    });
    $("#menu ul li a").click(function () {
        $('#menu ul li.selected2').removeClass();
        $(this).closest('li').addClass("selected2");
    });
    $("#sideMenu ul li:first-child .collapse").click(function () {
        $("#sideMenu2").show();
        $("#sideMenu2").animate({ left: '-=211' }, "slow", function () {
            $(this).css("position", "relative");
        });
        $("#sideMenu ul").animate({ width: 'toggle' }, "slow", function () {
            $("#sideMenu").hide();
            resizeWindow();
        });
        
        $.cookie("sideMenu", "hide", { expires: 1000 });
    });
    $("#sideMenu2").click(function () {
        $(this).hide();
        $(this).css("position", "absolute");
        $(this).css("left", "211px");

        $("#sideMenu").show();
        $("#sideMenu ul").animate({ width: 'toggle' }, "slow");
        
        resizeWindow();
        $.cookie("sideMenu", "show", { expires: 1000 });
    });

    var onlyChild = $('#sideMenu ul').children('li').length <= 1;

    if (onlyChild || $.cookie("sideMenu") == "hide") {
        $("#sideMenu2").css("position", "relative"); 
        $("#sideMenu2").show();
        $("#sideMenu2").css("left", "0");
        $("#sideMenu").hide();
        $("#sideMenu ul").hide();
        //$("#sideMenu ul li:first-child").click();
        
    }

}

function IsFieldsSettings() {
    try {
        if (parent && parent.parent && parent.parent.location.href)
            return queryString2(parent.parent.location.href, "settings") == "fields";
        else
            return false;
    }
    catch (err) {
        return false;
    }
}

function CancelPreview(viewName) {
    if (adminPreviewChanged) {
        adminPreviewChanged = false;
        url = '/Admin/CancelPreview/' + (viewName == null ? getMainViewName() : viewName);
        $.ajax({
            url: url,
            contentType: 'application/json; charset=utf-8',
            data: {},
            async: false,
            dataType: 'json',
            cache: false


        });
    }
}

function initMoreActions(guid) {
    //Drop down menu
    var id = guid + 'ajaxDiv';
    $("#" + id).find(".slide-down:first").unbind('click').bind('click', function (e) {
        $(this).next(".more-actions").slideToggle("fast", "jswing");
    });

    $("#" + id).find(".more-actions:first").unbind('mouseleave').bind('mouseleave', function (e) {
        $(this).slideUp("fast", "jswing");
    });
    $("#" + id).find(".more-actions:first").unbind('click').bind('click', function (e) {
        $(this).slideUp("fast", "jswing");
    });
}

/************************************************************************************/
/*		initButtons (by br)					
/*		Init buttons jquery style
/************************************************************************************/
function initButtons() {
    //by br- prevent bug in jquery button duplicate init
    $('.button:not(.button-init)').addClass('button-init').button();
}

/************************************************************************************/
/*		initImages (by br)					
/*		Init images: Display dialog on click, Display preview on hover.
/************************************************************************************/
function initImages() {

    Durados.Image.Init();
}

/************************************************************************************/
/*		initEditors (by br)					
/*		Init editors: Display editor icon on mouse hover.
/************************************************************************************/
function initEditors() {
    $('.gridview td[hoverEdit].enabledCell').each(function () {
        $(this).append('<a><span class="Edit-icon" title="Edit"></span></a>');
    });
}

function initCheckboxes() {
    $('input:checkbox:not([safari])').checkbox({ cls: 'jquery-safari-checkbox' });
    $('input[safari]:checkbox').checkbox({ cls: 'jquery-safari-checkbox' });
    $('input:radio:not(.radioClass)').checkbox();
}

function initColorPickers() {
    $('input[color=1]').each(function () {
        var colorPicker = $(this);
        if (colorPicker.css('display') != 'none') {
            initColorPicker(colorPicker);
        }
    });
}

function initColorPicker(colorPicker) {

    //Init override checkBox click handler
    var checkBox = colorPicker.siblings('input[type=checkbox]');
    checkBox.unbind('change').bind('change', function () {
        var isChecked = Durados.CheckBox.IsChecked(checkBox);
        var spectrumAction = isChecked ? 'enable' : 'disable';

        if (!isChecked) {
            var emptyValue = '#ffffff';
            colorPicker.spectrum('set', emptyValue);
        }

        colorPicker.spectrum(spectrumAction);
    });

    //Init colorPicker
    colorPicker.spectrum({
        // cancelText: "No way2",
        color: '#ffffff',
        showInput: true,
        showAlpha: true,
        clickoutFiresChange: true,
        showInitial: true,
        showPalette: true,
        localStorageKey: "spectrum.homepage.modubiz",
        palette: [
        ["rgb(0, 0, 0)", "rgb(67, 67, 67)", "rgb(102, 102, 102)", "rgb(153, 153, 153)", "rgb(183, 183, 183)",
        "rgb(204, 204, 204)", "rgb(217, 217, 217)", "rgb(239, 239, 239)", "rgb(243, 243, 243)", "rgb(255, 255, 255)"],
        ["rgb(152, 0, 0)", "rgb(255, 0, 0)", "rgb(255, 153, 0)", "rgb(255, 255, 0)", "rgb(0, 255, 0)",
        "rgb(0, 255, 255)", "rgb(74, 134, 232)", "rgb(0, 0, 255)", "rgb(153, 0, 255)", "rgb(255, 0, 255)"],
        ["rgb(230, 184, 175)", "rgb(244, 204, 204)", "rgb(252, 229, 205)", "rgb(255, 242, 204)", "rgb(217, 234, 211)",
        "rgb(208, 224, 227)", "rgb(201, 218, 248)", "rgb(207, 226, 243)", "rgb(217, 210, 233)", "rgb(234, 209, 220)",
        "rgb(221, 126, 107)", "rgb(234, 153, 153)", "rgb(249, 203, 156)", "rgb(255, 229, 153)", "rgb(182, 215, 168)",
        "rgb(162, 196, 201)", "rgb(164, 194, 244)", "rgb(159, 197, 232)", "rgb(180, 167, 214)", "rgb(213, 166, 189)",
        "rgb(204, 65, 37)", "rgb(224, 102, 102)", "rgb(246, 178, 107)", "rgb(255, 217, 102)", "rgb(147, 196, 125)",
        "rgb(118, 165, 175)", "rgb(109, 158, 235)", "rgb(111, 168, 220)", "rgb(142, 124, 195)", "rgb(194, 123, 160)",
        "rgb(166, 28, 0)", "rgb(204, 0, 0)", "rgb(230, 145, 56)", "rgb(241, 194, 50)", "rgb(106, 168, 79)",
        "rgb(69, 129, 142)", "rgb(60, 120, 216)", "rgb(61, 133, 198)", "rgb(103, 78, 167)", "rgb(166, 77, 121)",
        "rgb(133, 32, 12)", "rgb(153, 0, 0)", "rgb(180, 95, 6)", "rgb(191, 144, 0)", "rgb(56, 118, 29)",
        "rgb(19, 79, 92)", "rgb(17, 85, 204)", "rgb(11, 83, 148)", "rgb(53, 28, 117)", "rgb(116, 27, 71)",
        "rgb(91, 15, 0)", "rgb(102, 0, 0)", "rgb(120, 63, 4)", "rgb(127, 96, 0)", "rgb(39, 78, 19)",
        "rgb(12, 52, 61)", "rgb(28, 69, 135)", "rgb(7, 55, 99)", "rgb(32, 18, 77)", "rgb(76, 17, 48)"]
    ]
        //            preferredFormat: "hex"
        //        chooseText: "Alright",
        //    cancelText: "No way"
    });
}

function initWatermark() {
    $.fn.watermark = function (options) {
        return this.each(function () {
            var input = $(this);
            var defaultText = input.attr('dvalue') || input.val();
            input.bind('focus', function () {
                if (input.val().length === 0 || input.val() === defaultText) {
                    input.val('').removeClass('watermarkThis');
                }
            });

            input.bind('blur change', function () {
                if (input.val().length === 0 || input.val() === defaultText) {
                    input.val(defaultText).addClass('watermarkThis');
                } else {
                    input.removeClass('watermarkThis');
                }
            }).trigger('blur');

        });
    };
    $('.watermark').watermark({
        defaultClass: '',
        waterMarkClass: 'watermarkThis'
    });
}

function initRotateMenu() {

    // Superfish - jQuery menu widget
    $('#mainmenu').superfish();
    $('#mymenu').superfish();

    return;
    //initMoreActions();

    //Open zone menu	
    $(".zone #menuheadbutton, .zone a").click(function (e) {
        $(".submenu-container").slideToggle("fast")
    });
    $(".zone").mouseleave(function (e) {
        $(".submenu-container").slideUp("fast")
    });

    if (!isIE()) {
        // Rotate animation
        var angle = 0;
//        setInterval(function () {
//            angle += 0.5;
//            $(".rotator").rotate(angle);
//        }, 10);


        // Rotate animation	
        $("#menuheadbutton").rotate({
            bind:
         {
             mouseover: function () {
                 $(this).rotate({ animateTo: 30 })
             },
             mouseout: function () {
                 $(this).rotate({ animateTo: 0 })
             }
         }


        });

    }

    //Menu bar animation
    $("a.submenu-item").click(function (e) {
        $('.slider').css({ opacity: '0' });
        $('.slider').animate({ opacity: "1", left: "1024px" }, 700, 'jswing');
        $('.slider').animate({ opacity: "0", left: "50px" }, 300);
    });

    //Replace icon on menu bar
    $(".submenu-item.item1").live('click', function (e) {
        $("#menuheadbutton").attr('class', 'item1');
    });
    $(".submenu-item.item2").live('click', function (e) {
        $("#menuheadbutton").attr('class', 'item2');
    });
    $(".submenu-item.item3").live('click', function (e) {
        $("#menuheadbutton").attr('class', 'item3');
    });
    $(".submenu-item.item4").live('click', function (e) {
        $("#menuheadbutton").attr('class', 'item4');
    });
    $(".submenu-item.item5").live('click', function (e) {
        $("#menuheadbutton").attr('class', 'item5');
    });
    $(".submenu-item.item6").live('click', function (e) {
        $("#menuheadbutton").attr('class', 'item6');
    });
    $(".submenu-item.item7").live('click', function (e) {
        $("#menuheadbutton").attr('class', 'item7');
    });
    $(".submenu-item.item8").live('click', function (e) {
        $("#menuheadbutton").attr('class', 'item8');
    });
    $(".submenu-item.item9").live('click', function (e) {
        $("#menuheadbutton").attr('class', 'item9');
    });
    $(".submenu-item.item10").live('click', function (e) {
        $("#menuheadbutton").attr('class', 'item10');
    });
}

var adjustDataTableHeightDisabled = false;
function adjustDataTableHeight() {
    Durados.FieldEditor.CloseAllEditors();
    Durados.GridHandler.adjustDataTableHeight(0);
    Durados.GridHandler.setHeadersDivWidth();
}


function refreshMainContent() {

    var guid = getMainPageGuid();
    if (guid) {
        //FilterForm.Apply(true, guid);
        AddDialog.Refresh(guid, null);
    }
}

function getMainPageGuid() {
    return $('#mainAppDiv').find('div.fixedViewPort').first().attr('d_fix');
}

function getMainPageDiv() {
    guid = getMainPageGuid();

    if (!guid) { return ''; }

    return $('#' + guid + 'ajaxDiv div.fixedViewPort').first();
}

function getMainPageHeight() {
    return $('#' + getMainPageGuid() + 'ajaxDiv div.fixedViewPort').first().height();
}

$(Durados.View).bind('rowChanged', function (e, data) {
    var pk = data.tr.attr('d_pk');
    var guid = data.tr.attr('guid');
    if (pk != null) {
        Multi.SelectByPK(guid, pk);
    }
});

function setIReportCss() {
    $("body").css("margin-bottom", 0);
    $("body").css("padding-bottom", 0);
}
var iframehide = "no";
function setIframeSize(iframe) {
    if (iframe.contentDocument && iframe.contentDocument.body.scrollHeight) {
        iframe.style.height = $(window).height() - 120;
    }
    else if (iframe.Document && iframe.Document.body.scrollHeight) {
        iframe.height = $(window).height() - 80;
    }
}
/*
//function for shortcuts ctrl-key, currently not in use
$.ctrl = function(element, key, callback, args) {
if (!element) element = document;
$(element).keydown(function(e) {
if (!args) args = []; // IE barks when args is null
if (e.keyCode == key.charCodeAt(0) && e.ctrlKey) {
callback.apply(this, args);
e.preventDefault();
return false;
}
});
};
*/

// rich
var UrlDialog; if (!UrlDialog) UrlDialog = {};

UrlDialog.GetButtons = function (img, pk, fieldName, guid, dialog) {
    var buttons = {};  //initialize the object to hold my buttons
    buttons[translator.ok] = function () { UrlDialog.Update(img, pk, fieldName, guid, dialog); }  //the function that does the save
    buttons[translator.cancel] = function () { $(this).dialog("close"); }  //the function that does the save
    return buttons;
}

UrlDialog.Open = function (img, pk, fieldName, displayName, guid) {
    begin();

    var dialog = $('#UrlDialog');

    if (dialog.parents('div[role="dialog"]').length == 0) {
        var winWidth = $(window).width();
        var winHeight = $(window).height();
        var tabWidth = winWidth * .75 - 55;

        $("#UrlDialog").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            zIndex: 10000,
            position: 'center',
            height: (winHeight * .5),
            width: (winWidth * .5)
        });
    }

    dialog.dialog("option", "title", translator.editRowOn + displayName);
    dialog.dialog("option", "buttons", UrlDialog.GetButtons(img, pk, fieldName, guid, dialog));

    dialog.dialog('open');

    UrlDialog.Load(img, pk, fieldName, guid);

    complete(guid);
}

UrlDialog.GetA = function (img) {
    return $(img).parent().siblings('a:first');
}

UrlDialog.Load = function (img, pk, fieldName, guid) {


    var a = UrlDialog.GetA(img);

    var text = a.find('span').text();
    var href = a.attr('href');
    if (href == text)
        text = '';
    if (href == '' || href == '#') {
        href = 'http://';
    }
    var newWindow = a.attr('target') == '_blank' || a.text() == '';

    var dialog = $('#UrlDialog');

    dialog.find('input[name="urlDisplayName"]').val(text);
    dialog.find('input[name="urlAddress"]').val(href);
    //dialog.find('input[name="urlNewWindow"]').attr('checked', newWindow);
    Durados.CheckBox.SetChecked(dialog.find('input[name="urlNewWindow"]'), newWindow);
}


UrlDialog.Update = function (img, pk, fieldName, guid, dialog) {

    begin();

    var text = dialog.find('input[name="urlDisplayName"]').val();
    var href = dialog.find('input[name="urlAddress"]').val();
    //var newWindow = isChecked(dialog.find('input[name="urlNewWindow"]'));
    var newWindow = Durados.CheckBox.IsChecked(dialog.find('input[name="urlNewWindow"]'));
    var target = newWindow ? "_blank" : "_self";

    var a = UrlDialog.GetA(img);

    if (href == '')
        href = '#';

    if (text == '' && href != '#') {
        text = href;
    }

    a.find('span').text(text);
    a.attr('href', href);
    a.attr('target', target);

    var value = text + "|" + target + "|" + href;
    if (!text || text == href) {
        value = href;
    }

    if (pk == '') {
        a.attr('value', value);
        UrlDialog.HandleSuccess(guid, dialog);
        a.parent().blur(); //For url editor
        if (guid == pageGuid) {
            a.blur();
        }
    }
    else {
        $.post(rootPath + views[guid].Controller + '/EditUrl/' + views[guid].gViewName,
        {
            pk: pk,
            fieldName: fieldName,
            value: value,
            guid: guid
        },
        function (html) {
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                UrlDialog.HandleSuccess(guid, dialog);
                a.parent().blur(); //For url editor

            }
            else {
                ajaxNotSuccessMsg(html, guid);
            }
        });
    }
}

UrlDialog.HandleSuccess = function (guid, dialog) {
    dialog.dialog("close");

    hideProgress();
}

var elementScrollsPosition = { "x": 0, "y": 0 }

function saveElementScrollsPosition(guid) {

    var el;
    if (!guid) { el = getMainPageDiv(); } else { el = $('#' + guid + 'ajaxDiv div.fixedViewPort').first(); }
    if (!el.length) return;

    elementScrollsPosition.y = el.scrollTop();
    elementScrollsPosition.x = el.scrollLeft();

    //    PreviewDisplay.saveElementState(guid);

    //alert(elementScrollsPosition.x + ' ' + elementScrollsPosition.y);
}

function resetElementScrollsPosition(guid) {

    var el;
    if (!guid) { el = getMainPageDiv(); } else { el = $('#' + guid + 'ajaxDiv div.fixedViewPort').first(); }
    if (!el.length) return;

    if (elementScrollsPosition.y)
        el.scrollTop(elementScrollsPosition.y);

    if (elementScrollsPosition.x)
        el.scrollLeft(elementScrollsPosition.x);

    elementScrollsPosition.y = 0;
    elementScrollsPosition.x = 0;

    Durados.GridHandler.scrollHeaders(guid);
    //    PreviewDisplay.loadElementState(guid);
    //Durados.GridHandler.scrollData(guid);


    //alert(elementScrollsPosition.x + ' ' + elementScrollsPosition.y);
}

function isInDialog(element) {
    return $(element).parents("div.ui-dialog:first").length > 0;
}

var Roles; if (!Roles) Roles = {};

$(Durados.View).bind('derivationFieldChanged', function (e, data) {
    if ((data.viewName == 'Workspace' || data.viewName == 'View' || data.viewName == 'Field' || data.viewName == 'Page') && !data.value) {
        var message;
        if (data.viewName == 'Workspace') {
            message = 'By unchecking the Override checkbox, the Workspace Roles will inherit the Systems Roles. The current Roles will be lost. Click OK to proceed.';
        }
        else if (data.viewName == 'View') {
            message = 'By unchecking the Override checkbox, the View Roles will inherit the Workspace Roles. The current Roles will be lost. Click OK to proceed.';
        }
        else if (data.viewName == 'Field') {
            message = 'By unchecking the Override checkbox, the Field Roles will inherit the View Roles. The current Roles will be lost. Click OK to proceed.';
        }
        else if (data.viewName == 'Page') {
            message = 'By unchecking the Override checkbox, the Page Roles will inherit the Workspace Roles. The current Roles will be lost. Click OK to proceed.';
        }
        data.cancel = !confirm(message);
        if (data.cancel) {
            //data.derivationField.attr('checked', true);
            Durados.CheckBox.SetChecked(data.derivationField, true);
            data.derivationField.parent('td:first').focus();
        }
        else {
            var pk = '';
            var tabs;
            var dialog;
            if (data.prefix == editPrefix) {
                dialog = $('#' + data.guid + 'DataRowEdit');
                //tabs = data.derivationField.parents('#' + data.guid + 'EditTabs');
                pk = dialog.attr('pk');
            }
            else if (data.prefix == inlineEditingPrefix) {
                dialog = $('#' + data.guid + data.viewName + "inlineEditing");
                pk = dialog.attr('pk');

            }
            else {
                dialog = $('#' + data.guid + 'DataRowCreate');
                //tabs = data.derivationField.parents('#' + data.guid + 'CreateTabs');
            }

            $.ajax({
                url: gVD + 'Admin/GetInheritRoles',
                contentType: 'application/json; charset=utf-8',
                data: { type: data.viewName, pk: pk },
                async: false,
                dataType: 'json',
                cache: false,
                error: ajaxErrorsHandler,
                success: function (json) {
                    if (json == 'error') {
                        //data.derivationField.attr('checked', true);
                        Durados.CheckBox.SetChecked(data.derivationField, true);
                        data.derivationField.parent('td:first').focus();
                        alert('Failed to change roles. Please notify administrator and try later.');
                    }
                    else {
                        var allowCreateRoles = dialog.find('select[name="AllowCreateRoles"]');
                        var allowDeleteRoles = dialog.find('select[name="AllowDeleteRoles"]');
                        var allowEditRoles = dialog.find('select[name="AllowEditRoles"]');
                        var allowSelectRoles = dialog.find('select[name="AllowSelectRoles"]');
                        var denyCreateRoles = dialog.find('select[name="DenyCreateRoles"]');
                        var denyDeleteRoles = dialog.find('select[name="DenyDeleteRoles"]');
                        var denyEditRoles = dialog.find('select[name="DenyEditRoles"]');
                        var denySelectRoles = dialog.find('select[name="DenySelectRoles"]');
                        if (data.viewName == "Page")
                            clearAndLoadChecklist(allowSelectRoles, json.AllowSelectRoles);
                        else {
                            clearAndLoadChecklist(allowCreateRoles, json.AllowCreateRoles);
                            if (data.viewName != 'Field')
                                clearAndLoadChecklist(allowDeleteRoles, json.AllowDeleteRoles);
                            clearAndLoadChecklist(allowEditRoles, json.AllowEditRoles);
                            clearAndLoadChecklist(allowSelectRoles, json.AllowSelectRoles);
                            clearAndLoadChecklist(denyCreateRoles, json.DenyCreateRoles);
                            if (data.viewName != 'Field')
                                clearAndLoadChecklist(denyDeleteRoles, json.DenyDeleteRoles);
                            clearAndLoadChecklist(denyEditRoles, json.DenyEditRoles);
                            clearAndLoadChecklist(denySelectRoles, json.DenySelectRoles);
                        }
                    }
                }

            });
        }
    }
});


function pageMoved(guid) {
    success(guid);
}


function calculatedFieldChanged(viewName, fieldName, guid, pk) {
    url = rootPath + views[guid].Controller + '/CalculatedFieldChanged/' + viewName;
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { fieldName: fieldName, pk: pk, guid: guid },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            Sharing.Update(json, true);
        }

    });
}

var Sharing; if (!Sharing) Sharing = {};

Sharing.Update = function (json, showEffect) {
    $(json.Views).each(function () {
        var view = $(this);
        $(view.Rows).each(function () {
            var row = $(this);
            var viewName = view.Name;
            var pk = row.PK;
            var tr = $('table.gridview>tr[d_pk="' + pk + '"]');
            if (tr.length == 1) {
                $(row.Fields).each(function () {
                    var field = $(this);
                    var fieldName = field.Name;
                    var value = field.Value;
                    var displayValue = field.DisplayValue;
                    var td = tr.find('td[d_name="' + fieldName + '"]');
                    var guid = td.attr('d_guid');
                    Durados.FieldEditor.setCellValue(td, value, displayValue, guid, fieldName);
                    if (showEffect) {
                        setTimeout(function () {
                            td.addClass('sharingCellChanged');
                            setTimeout(function () {
                                td.removeClass('sharingCellChanged');
                            }, 2000);
                        }, 1);
                    }
                });
            }
        });
    });
}



var DuradosMail; if (!DuradosMail) DuradosMail = {};

DuradosMail.sendMailTo = function (to, cc, subject, body) {
    var iframe = $('<iframe>');
    iframe.attr('src', "about:blank");
    iframe.css('display', "none");
    $('body').append(iframe);
    var href = "mailto:" + to + "?subject=" + subject + "&amp;body=" + encodeURIComponent(body) + "&cc=" + cc;
    //var href = "mailto:" + ((to == null) ? "" : to) + "?subject=" + ((subject == null) ? "" : subject) + "&cc=" + ((cc == null) ? "" : cc) + "&body=" + ((body == null) ? "" : escape(body));
    iframe[0].contentWindow.location.href = href;


    iframe.remove();
}


DuradosMail.send = function (to, cc, subject, body, attachmentFileName, attachmentContents) {
    DuradosMail.sendMailTo(to, cc, subject, body);
}

var Send; if (!Send) Send = {};

Send.DialogName = function () {
    return "dialog_email";
}

Send.Show = function (guid) {
    var gViewName = views[guid].gViewName;
    var pks = Multi.GetSelection(guid);
    if (pks == null)
        return;

    if (pks.length == 0) { hideProgress(); modalErrorMsg('Please select a row'); return; }
    if (pks.length > 10) { hideProgress(); modalErrorMsg('Please select no more than 10 rows'); return; }

    if (!isIE()) {
        Send.ShowCustom(pks, gViewName, guid);
    }
    else {
        $.ajax({
            url: rootPath + views[guid].Controller + '/GetJsonLetter/' + gViewName,
            contentType: 'application/json; charset=utf-8',
            data: { pks: String(pks) },
            async: false,
            cache: false,
            success: function (email) {
                hideProgress();
                //var email = Sys.Serialization.JavaScriptSerializer.deserialize(json);
                DuradosMail.send(email.To, email.Cc, email.Subject, email.Body, email.FileName, email.Content);
            }

        });
    }
}

Send.ShowCustom = function (pkValue, gViewName, guid) {
    try {
        if (!$('#mydialog').is(':data(dialog)')) {
            $('#sendEmailDiv').dialog({
                autoOpen: false,
                width: 750,
                modal: false,
                closeOnEscape: false,
                position: 'center',
                zIndex: 999999,
                title: 'Send',
                buttons: {
                    'Send': function () { Send.Send(pkValue, gViewName, guid); },
                    'Cancel': function () { $(this).dialog("close"); }
                }
            });
        }
        //jquery doesn't remember the position just the size
        var rec = Rectangle.Load('SendProposal');
        if (rec != null) {
            $('#sendEmailDiv').dialog("option", "position", [rec.left, rec.top]);
        }
        Send.Load(pkValue, gViewName, guid);
        $('#sendEmailDiv').dialog('open');

        //Create wysiwyg for all textareas
        Send.CreateWysiwyg();

        complete(guid);
    }
    catch (ex) { }
}


Send.CreateWysiwyg = function () {
    $('#sendEmailDiv textarea').each(function () { $(this).htmlarea("dispose") });
    $('#sendEmailDiv textarea').htmlarea();
}

Send.Load = function (pk, gViewName, guid) {
    $.ajax({
        url: rootPath + views[guid].Controller + '/GetLetter/' + gViewName,
        contentType: 'application/json; charset=utf-8',
        data: { pk: String(pk) },
        async: false,
        cache: false,
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
        },
        success: function (html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                $('#sendEmailDiv').html(html);

            }
        }

    });
}

Send.Send = function (pk, gViewName, guid) {
    begin();

    //var textarea = $('#sendEmailDiv TextArea[name="body"]');
    var textarea = $('#sendEmailDiv').find('Iframe').contents().find('body');
    var subject = $('#sendEmailDiv input[name="subject"]').val();
    var to = $('#sendEmailDiv input[name="to"]').val();
    var cc = $('#sendEmailDiv input[name="cc"]').val();

    var body = textarea.text();


    $.post(rootPath + views[guid].Controller + '/Send/' + gViewName,
    {
        pk: String(pk),
        subject: subject,
        to: to,
        cc: cc,
        body: body
    },
    function (h) {
        var index = h.indexOf("$$error$$", 0)
        if (index < 0 || index > 1000) {
            $('#sendEmailDiv').dialog('close');
            success(guid);
            //modalSuccessMsg('The message was succefully sent.');
            return;
        }
        else {
            modalErrorMsg(h.replace("$$error$$", "")); return;
            complete(guid);
        }
    });
}

//Dialog
Send.SaveDialogOnResize = function (event, ui) {
    var rect = Rectangle.New(ui.position.top, ui.position.left, ui.size.width, ui.size.height);
    Rectangle.Save(rect, Send.DialogName());
}

Send.SaveDialogOnDrag = function (event, ui) {
    var rect = Rectangle.Load(Send.DialogName())
    if (rect != null) {
        rect.top = ui.position.top;
        rect.left = ui.position.left;
        Rectangle.Save(rect, Send.DialogName());
    }
}

Durados.GridHandler = {
    /************************************************************************************/
    /*		setColumnsWidth	(by br)			
    /*		Set columns width by gridDisplayType.	
    /************************************************************************************/
    setColumnsWidth: function (guid, recursiveTime) {
        if (Durados.Indications.fitToWindowWidth(guid)) {
            var ajaxDiv = $('#' + guid + 'ajaxDiv');
            if (!ajaxDiv.length) { return; }

            var viewPort = ajaxDiv.find('div.fixedViewPort').first();
            if (!viewPort.length) { return; }

            var headersSelector = 'tr.gridviewhead';
            var dataColumnsWidth = 0;
            var headers = ajaxDiv.find(headersSelector + ' th:not(.tablecmdhead,.lastTD)');
            var columnsWidth = [];
            var scrollbarWidth = 0;

            //calculate newDataColumnsWidth
            var newDataColumnsWidth = viewPort.width();

            //if has vertical scrollbar- reomove its width from newDataColumnsWidth
            if (viewPort.innerHeight() < viewPort[0].scrollHeight) {
                scrollbarWidth = Durados.GridHandler.getScrollbarWidth();
                newDataColumnsWidth -= scrollbarWidth;
            }

            //Reomove tablecmdhead width from newDataColumnsWidth
            var otherColumnsWidth = 0;
            ajaxDiv.find(headersSelector + ' th.tablecmdhead:visible').each(function () {
                otherColumnsWidth += $(this).outerWidth();
            });
            newDataColumnsWidth -= otherColumnsWidth;

            //calculate dataColumnsWidth
            headers.each(function () {
                var width = $(this).outerWidth();
                dataColumnsWidth += width;
                columnsWidth.push({ element: $(this), width: width });
            });

            //Set columns width
            $(columnsWidth).each(function () {
                var th = this.element;
                var columnWidth = this.width;
                var columnWidthPercents = columnWidth / dataColumnsWidth;
                var newColumnWidth = Math.floor(columnWidthPercents * newDataColumnsWidth);
                var columnIdx = Durados.GridHandler.getCellIndex(this.element[0]);
                Durados.GridHandler.setColumnWidth(guid, columnIdx, newColumnWidth, true);
            });

            //Remove border from last column
            if ($('body').attr('dir') != 'rtl') {
                headers.last(':not(.ScrollPlace)').css('border-right-width', '0px');
            }

            var images = $('#' + guid + 'ajaxDiv table.gridview td.ImageFieldType img');
            images.each(function () {
                Durados.Image.SetSize($(this));
            });

            //To prevent a bug: If already there is a scroll- try again to strech columns
            if (recursiveTime == null) {
                recursiveTime = 1;
            }

            scrollbarWidth = 0;
            if (viewPort.innerHeight() < viewPort[0].scrollHeight) {
                scrollbarWidth = Durados.GridHandler.getScrollbarWidth();
            }
            if ((viewPort.outerWidth() - scrollbarWidth) < viewPort[0].scrollWidth && recursiveTime < 7) {
                Durados.GridHandler.setColumnsWidth(guid, ++recursiveTime);
            }

            //In order to resolve a bug
            Durados.GridHandler.firstColumnsWidthDone = 0;
        }
    },
    /************************************************************************************/
    /*		SetNewColumnWidth	(by br)			
    /*		Set width for new column by calculate avarage.	
    /************************************************************************************/
    setNewColumnWidth: function (guid, newColumnName) {
        //Calculate newColumnWidth
        var columns = $('#' + guid + 'ajaxDiv table.gridview tr.gridviewhead th:not(.tablecmdhead,.lastTD)');
        var totalColumns = columns.length;
        var newColumnWidth = 0;

        if (totalColumns) {
            var totalWidth = 0;

            columns.each(function () {
                totalWidth += $(this).outerWidth();
            });

            newColumnWidth = Math.floor(totalWidth / totalColumns);
        }
        else {
            newColumnWidth = $('#' + guid + 'ajaxDiv div.fixedViewPort').width();
        }

        //Save new column State
        var newColumnState = { "Key": newColumnName, "Value": { "width": newColumnWidth} };
        Durados.ColumnResizer.saveState(guid, false, newColumnState, false);
    },
    adjustDataTable: function (guid) {
        Durados.GridHandler.adjustDataTableHeight(guid);
        Durados.GridHandler.setDataTableScrollAndCellsWidth(guid);
    },
    isAppUseFixedHeadres: true,
    isFixedHeadersTable: function (guid) {
        if (!Durados.GridHandler.isAppUseFixedHeadres) return false;
        return $('#' + guid + 'ajaxDiv div.headWrapper').first().attr('d_fix') == guid;
    },
    firstColumnsWidthDone: 0,
    headersOffset: 0,
    firstDo: 20,
    scollBarsWith: 0,
    getScrollbarWidth: function () {
        if (Durados.GridHandler.scollBarsWith > 0) return Durados.GridHandler.scollBarsWith;
        var inner = document.createElement('p');
        inner.style.width = "100%";
        inner.style.height = "200px";

        var outer = document.createElement('div');
        outer.style.position = "absolute";
        outer.style.top = "0";
        outer.style.left = "0";
        outer.style.visibility = "hidden";
        outer.style.width = "200px";
        outer.style.height = "150px";
        outer.style.overflow = "hidden";
        outer.appendChild(inner);

        document.body.appendChild(outer);
        var w1 = inner.offsetWidth;
        outer.style.overflow = 'scroll';
        var w2 = inner.offsetWidth;
        if (w1 == w2) w2 = outer.clientWidth;

        document.body.removeChild(outer);
        //if (!$.browser.webkit) w1 -= 1;
        return Durados.GridHandler.scollBarsWith = (w1 - w2);
    },
    setIsMainDivHasVScroll: function (guid, hasScroll) {
        var hadScroll = Durados.GridHandler.isMainDivHasVScroll[guid];

        Durados.GridHandler.isMainDivHasVScroll[guid] = hasScroll;
        if (hadScroll != hasScroll) {
            Durados.GridHandler.setFreezeHeaders(guid, hasScroll);
        }
    },
    setFreezeHeaders: function (guid, isFreese) {
        if (isFreese) {
            if (!Durados.GridHandler.isFixedHeadersTable(guid)) {
                Durados.GridHandler.setDataTableScrollAndCellsWidth(guid);
            }
        }
        else {
            if (Durados.GridHandler.isFixedHeadersTable(guid)) {
                Durados.GridHandler.removeFreezeHeaders(guid);
            }
        }
    },
    removeFreezeHeaders: function (guid) {
        if (!guid || Durados.GridHandler.isMainDivHasVScroll[guid] || !Durados.GridHandler.isFixedHeadersTable(guid)) { return; }

        var headWrapper = $('#' + guid + 'ajaxDiv div.headWrapper').first();
        var gridview = $('#' + guid + 'ajaxDiv div.fixedViewPort .gridview').first();

        headWrapper.find('.ScrollPlace').remove();
        gridview.prepend($('<thead>').append(headWrapper.find('tr')));
        headWrapper.remove();
    },
    setDataTableScrollAndCellsWidth: function (guid) {
        if (!Durados.GridHandler.isAppUseFixedHeadres || !guid || !Durados.GridHandler.isMainDivHasVScroll[guid]) { return; }

        //        if (isIE())
        //            return;

        if (!Durados.Indications.isMainGrid(guid))
            return;

        var rowCountInPage = $('#' + guid + 'ajaxDiv').find('table.gridview:first').attr('rowCountInPage');
        if (!rowCountInPage)
            return;

        if (rowCountInPage > 20)
            return;

        var data = $('#' + guid + 'ajaxDiv table.gridview').first();

        if (!data.length) return; //|| data[0].rows.length < 3

        var isFirstTime = Durados.GridHandler.firstColumnsWidthDone == 0;

        if (Durados.GridHandler.isFixedHeadersTable(guid) && isFirstTime) return;

        var viewPort = $('#' + guid + 'ajaxDiv div.fixedViewPort').first();

        var isEditMode = data.attr('d_editMode') == 'on';

        //Set fixed headers
        if (isFirstTime) {
            //YAHOO.tool.Profiler.start("setheaderstable");

            var thead = data.find('thead');

            if (!thead.length) return;

            var theadH = thead.height();

            var vpwidth = viewPort.width() - Durados.GridHandler.scollBarsWith;

            if (isEditMode) { vpwidth--; }

            var head = $('<div class="headWrapper"></div>').attr("d_fix", guid); //.width(vpwidth);
            var multiSelect = data.attr('multiSelect');
            head.html('<table cellspacing="0" class="gridviewhead"  multiSelect="' + multiSelect + '" >' + thead.html() + '</table>');
            head.children('table').first().css('width', '100%');

            thead.remove();

            head.insertBefore(viewPort);

            viewPort.height(viewPort.height() - theadH);

            //Add lastTH as placeholder against data scrollbar
            if (($.browser.webkit || $.browser.mozilla) && $('body').attr('dir') == 'rtl') {
                head.find('tr').append('<td class="ScrollPlace" style="padding:0;margin:0;width:' + Durados.GridHandler.getScrollbarWidth() + 'px"><div style="width:' + Durados.GridHandler.getScrollbarWidth() + 'px">&nbsp;</div></td>');
            } else {
                head.find('tr').append('<td class="ScrollPlace" style="padding:0;margin:0;width:' + Durados.GridHandler.getScrollbarWidth() + 'px"><div style="width:' + Durados.GridHandler.getScrollbarWidth() + 'px">&nbsp;</div></td>');
            }
            //YAHOO.tool.Profiler.stop("setheaderstable");            
        }

        if (Durados.GridHandler.firstColumnsWidthDone > Durados.GridHandler.firstDo) return;

        //Set widths
        var headers = $('#' + guid + 'ajaxDiv table.gridviewhead').first();

        if (headers.length == 0 || !headers[0].rows || !headers[0].rows.length) { return; }

        var lastTRIndex = data[0].rows.length - 1;
        var lastDataTR = data[0].rows[lastTRIndex];
        if (!lastDataTR) {
            hideProgress();
            return;
        }
        var cellsNum = lastDataTR.cells.length - 1;
        var headersTR = headers[0].rows[(headers[0].rows.length - 1)];
        var offset = 1;

        //YAHOO.tool.Profiler.start("findelements");
        for (var c = Durados.GridHandler.firstColumnsWidthDone; c < cellsNum; c++) {
            //YAHOO.tool.Profiler.start("widthopen");

            if (lastDataTR.cells[c].firstChild.className == "ganttDataRowDiv" || $(lastDataTR.cells[c]).hasClass('tablecmd')) continue;

            var h = headersTR.cells[c + Durados.GridHandler.headersOffset];
            var d = lastDataTR.cells[c]; //.firstChild

            var $h = $(h);
            var wh = $h.outerWidth();
            var $d = $(d);
            var wd = $d.outerWidth();

            //YAHOO.tool.Profiler.stop("widthopen");
            if (wh > wd) {
                Durados.GridHandler.setColumnWidth(guid, c, wh, true);
            } else if (wh < wd) {
                //br todo- change only header width
                Durados.GridHandler.setColumnWidth(guid, c, wh, true);
            }

            Durados.GridHandler.firstColumnsWidthDone++;
            if (isFirstTime && Durados.GridHandler.firstColumnsWidthDone == Durados.GridHandler.firstDo) break;
        }
        //YAHOO.tool.Profiler.stop("findelements");

        if (isFirstTime) {
            //Set headers scroll event
            viewPort.unbind('scroll');
            viewPort.bind('scroll', function () { var g = guid; Durados.GridHandler.scrollData(g); triggerEditorBlur(); });

            if (Durados.GridHandler.firstColumnsWidthDone < cellsNum) {
                showProgress();
                setTimeout(function () { var g = guid; Durados.GridHandler.setDataTableScrollAndCellsWidth(g); }, 50);
            } else {
                Durados.GridHandler.firstColumnsWidthDone = 0;
                hideProgress();
            }
        } else {
            Durados.GridHandler.firstColumnsWidthDone = 0;
            hideProgress();
            /* if (Durados.GridHandler.firstDo < cellsNum && Durados.GridHandler.elemx) {
            elementScrollsPosition.x = Durados.GridHandler.elemx;
            elementScrollsPosition.y = Durados.GridHandler.elemy;
            Durados.GridHandler.elemx = 0;
            Durados.GridHandler.elemy = 0;
            resetElementScrollsPosition(guid);
            }Durados.GridHandler.scrollData(guid); */

        }
        hideProgress();
    },
    scrollData: function (guid) {
        if (!Durados.GridHandler.isAppUseFixedHeadres || !guid) { return; }
        var h = $('#' + guid + 'ajaxDiv div.headWrapper').first();
        var d = $('#' + guid + 'ajaxDiv div.fixedViewPort').first();
        if (h.length == 1 && h.attr('d_fix') == guid) {
            //h.width(d.width());
            h.scrollLeft(d.scrollLeft());
        }
    },
    scrollHeaders: function (guid) {
        if (!Durados.GridHandler.isAppUseFixedHeadres || !guid) { return; }
        var h = $('#' + guid + 'ajaxDiv div.headWrapper').first();
        if (h.length == 1 && h.attr('d_fix') == guid) {
            $('#' + guid + 'ajaxDiv div.fixedViewPort').first().scrollLeft(h.scrollLeft());
        }
    },
    setHeadersDivWidth: function () {
        if (!Durados.GridHandler.isAppUseFixedHeadres) { return; }
        var h = $('div.headWrapper');
        if (h.length > 0) {
            h.each(function () {
                var guid = $(this).attr("d_fix");
                if (guid) {
                    var d = $('#' + guid + 'ajaxDiv div.fixedViewPort').first();
                    if (d.attr('d_fix') == guid) {
                        var dw = d.width();
                        if (dw > 100) h.width(dw);
                    }
                }
            });
        }
    },
    scrollParentsData: function (element) {
        if (!Durados.GridHandler.isAppUseFixedHeadres) { return; }

        element.parents('div.fixedViewPort').each(function () {
            var guid = $(this).attr('d_fix');
            if (guid) {
                var h = $('#' + guid + 'ajaxDiv div.headWrapper').first();
                if (h.length == 1 && h.attr('d_fix') == guid) {
                    h.scrollLeft($(this).scrollLeft());
                }
            }
        });
    },
    setHeaderWidth: function (cell) {

        var guid = cell.parents('div.fixedViewPort').first().attr('d_fix');

        if (!guid || !Durados.GridHandler.isFixedHeadersTable(guid)) { return; }

        var headers = $('#' + guid + 'ajaxDiv table.gridviewhead').first();

        if (headers.length == 0 || !headers[0].rows || !headers[0].rows.length) { return; }

        var headersTR = headers[0].rows[(headers[0].rows.length - 1)];


        var c = Durados.GridHandler.getCellIndex(cell[0]);

        var h = headersTR.cells[c + Durados.GridHandler.headersOffset];

        var wh = h.offsetWidth - 21;
        var wd = cell[0].offsetWidth - 21;

        if (wh < wd) {
            h.firstChild.style.width = wd + 'px';
        } else if (wh > wd) {
            var data = $('#' + guid + 'ajaxDiv table.gridview').first();
            var lastTRIndex = data[0].rows.length - 1;
            data[0].rows[lastTRIndex].cells[c].firstChild.style.width = wh + 'px';
        }
    },
    setColumnWidth: function (guid, idx, newWidth, avoidRefreshImagesSizes) {
        if (!guid) return;

        var headers = $('#' + guid + 'ajaxDiv table.gridviewhead').first();
        var data = $('#' + guid + 'ajaxDiv table.gridview').first();

        //by br
        if (headers.length) {
            var $cell;
            var $cellDiv;
            var newDivWidth;

            for (i = 0; i < headers[0].rows.length; i++) {
                var cls = headers[0].rows[i].className;
                if (cls == 'trsubgrid' || cls == 'rowfilter') continue;

                $cell = $(headers[0].rows[i].cells[idx]);
                $cellDiv = $cell.children('div').first();

                newDivWidth = newWidth - num($cell.css('padding-right')) - num($cell.css('padding-left')) - num($cellDiv.css('margin-right')) - num($cellDiv.css('margin-left')) - num($cell.css('border-right-width')) - num($cell.css('border-left-width'));
                if (newDivWidth < 5) { newDivWidth = 5; }
                $cellDiv.width(newDivWidth);
            }
        }

        if (data.length) {

            var isEditMode = data.attr('d_editMode') == 'on';
            var border = 1;
            var $cell;
            var $cellDiv;
            var newDivWidth;

            for (i = 0; i < data[0].rows.length; i++) {
                var cls = data[0].rows[i].className;
                if (cls == 'trsubgrid' || cls == 'rowfilter') continue;

                $cell = $(data[0].rows[i].cells[idx]);
                $cellDiv = $cell.children('div').first();

                newDivWidth = newWidth - num($cell.css('padding-right')) - num($cell.css('padding-left')) - num($cellDiv.css('margin-right')) - num($cellDiv.css('margin-left')) - num($cell.css('border-right-width')) - num($cell.css('border-left-width'));

                if (newDivWidth < 5) { newDivWidth = 5; }
                $cellDiv.width(newDivWidth);
            }

            if (avoidRefreshImagesSizes !== true) {
                var images = $('#' + guid + 'ajaxDiv table.gridview td.ImageFieldType img');
                images.each(function () {
                    Durados.Image.SetSize($(this));
                });
            }
        }

        Durados.GridHandler.scrollData(guid);

    },
    getCellIndex: function (cell) {
        var rtrn = cell.cellIndex || 0;
        if (rtrn == 0) {
            do {
                if (cell.nodeType == 1) rtrn++;
                cell = cell.previousSibling;
            } while (cell);
            --rtrn;
        }
        return rtrn;
    },
    hideColumn: function (th, guid) {
        if (!th || !guid) { return; }
        if (th.length > 0) { th = th[0]; }

        var tbl = $('#' + guid + 'ajaxDiv table.gridview').first();

        var tblHead = $('#' + guid + 'ajaxDiv table.gridviewhead').first();

        var isFixedHead = tblHead.length == 1;

        if (tbl.length != 1 && !isFixedHead) return;

        tbl.find('img.expand[state="expand"]').click();

        tbl = tbl[0];

        var idx = Durados.GridHandler.getCellIndex(th);

        if (isFixedHead) {
            tblHead = tblHead[0];
            for (i = 0; i < tblHead.rows.length; i++) {
                var headersOffset = Durados.GridHandler.headersOffset;
                if ($(tblHead.rows[i]).hasClass("rowfilter")) {
                    headersOffset++;
                }
                if (tblHead.rows[i].cells.length > idx) {
                    $(tblHead.rows[i].cells[idx - headersOffset]).remove();
                }
            }
        }

        for (i = 0; i < tbl.rows.length; i++) {
            //if ($(tbl.rows[i]).hasClass("expanded")) {
            //    tbl.rows[i + 1].style.display = 'none';
            //    $(tbl.rows[i]).removeClass("expanded");
            //}
            var headersOffset = Durados.GridHandler.headersOffset;
            if ($(tbl.rows[i]).hasClass("rowfilter")) {
                headersOffset++;
            }
            if (tbl.rows[i].cells.length > idx - headersOffset) {
                $(tbl.rows[i].cells[idx - headersOffset]).remove();
            }
        }

        Durados.GridHandler.setColumnsWidth(guid);
    },
    moveColumn: function (guid, to) {
        if (!to || !guid || !movedColumnInfo[guid]) { return; }

        var from = movedColumnInfo[guid].elementToMove;
        var placement = movedColumnInfo[guid].placement;

        if (to.length > 0 && from.length > 0) { to = to[0]; from = from[0]; } else { return; }

        var tbl = $('#' + guid + 'ajaxDiv table.gridview').first();

        var tblHead = $('#' + guid + 'ajaxDiv table.gridviewhead').first();

        var isFixedHead = tblHead.length == 1;

        if (tbl.length != 1 && !isFixedHead) return;

        tbl.find('img.expand[state="expand"]').click();

        tbl = tbl[0];

        var idxFrom = Durados.GridHandler.getCellIndex(from);
        var idxTo = Durados.GridHandler.getCellIndex(to);
        if (idxFrom < idxTo) { idxTo--; }

        if (isFixedHead) {
            tblHead = tblHead[0];
            for (i = 0; i < tblHead.rows.length; i++) {
                if (tblHead.rows[i].cells.length > idxTo) {
                    if (placement == 'before') {
                        $(tblHead.rows[i].cells[idxFrom]).detach().insertBefore(tblHead.rows[i].cells[idxTo]);
                    } else {
                        $(tblHead.rows[i].cells[idxFrom]).detach().insertAfter(tblHead.rows[i].cells[idxTo]);
                    }
                }
            }
        }

        idxFrom -= Durados.GridHandler.headersOffset;
        idxTo -= Durados.GridHandler.headersOffset;

        for (i = 0; i < tbl.rows.length; i++) {
            //if ($(tbl.rows[i]).hasClass("expanded")) {
            //    tbl.rows[i + 1].style.display = 'none';
            //    $(tbl.rows[i]).removeClass("expanded");
            //}
            //else if (tbl.rows[i].css("display") == 'none') {continue;} ??

            var isFilterRow = $(tbl.rows[i]).hasClass('rowfilter');

            if (isFilterRow) {
                idxFrom--;
                idxTo--;
            }

            if (tbl.rows[i].cells.length > idxTo) {
                if (placement == 'before') {
                    $(tbl.rows[i].cells[idxFrom]).detach().insertBefore(tbl.rows[i].cells[idxTo]);
                } else {
                    $(tbl.rows[i].cells[idxFrom]).detach().insertAfter(tbl.rows[i].cells[idxTo]);
                }
            }

            if (isFilterRow) {
                idxFrom++;
                idxTo++;
            }
        }

        if (Durados.Indications.fitToWindowWidth(guid)) {
            var headers = $('#' + guid + 'ajaxDiv table.gridview tr.gridviewhead th:not(.tablecmdhead,.lastTD)');

            //Add border for all columns, and remove from last
            if ($('body').attr('dir') != 'rtl') {
                headers.find(':not(.ScrollPlace)').css('border-right-width', '1px');
                headers.last(':not(.ScrollPlace)').css('border-right-width', '0px');
            }
        }


    },
    setHeadersMoveToClass: function (guid, dir) {
        //$.each(movedColumnInfo, function(guid, value) {
        $('#' + guid + 'ajaxDiv').addClass('MovingColumn' + dir);
        //});
    },
    resetHeadersMoveToClass: function () {
        $.each(movedColumnInfo, function (guid, value) {
            var d = $('#' + guid + 'ajaxDiv');
            d.removeClass('MovingColumnLeft');
            d.removeClass('MovingColumnRight');
        });
        movedColumnInfo = {};
    }
    ,
    adjustDataTableHeight: function (guid) {

        var mainGuid = getMainPageGuid();

        if (!guid) { guid = mainGuid; }

        if (!guid) {
            adjustFreeBg(guid);
            return;
        }

        var isMainDiv = guid == mainGuid;


        var viewPort = $('#' + guid + 'ajaxDiv div.fixedViewPort').first();

        if (viewPort.length == 0) { return; }

        var data = $('#' + guid + 'ajaxDiv table.gridview').first(); //viewPort Child?

        if ($.browser.webkit && viewPort.attr("view") == "dashboard") {
            viewPort.find('div.gridboard').width(viewPort.width() - 30)
        }

        var mb = data.css('margin-bottom');
        var mbi = 0;
        if (mb) {
            mbi = parseInt(mb.replace('px', ''));
        }

        var mt = data.css('margin-top');
        var mti = 0;
        if (mt) {
            mti = parseInt(mt.replace('px', ''));
        }

        var dataHeight = data.outerHeight() + mti + mbi;

        if (!isMainDiv && Durados.GridHandler.isViewInSubGrid(viewPort)) {
            var maxHeight = views[guid].MaxSubGridHeight;
            maxHeight = parseInt(maxHeight);
            if (!maxHeight) maxHeight = 400; //maxHeight = 160;

            if (dataHeight > maxHeight) {
                viewPort.css("overflow", "auto").height(maxHeight);
                Durados.GridHandler.setIsMainDivHasVScroll(guid, true);
                //                Durados.GridHandler.isMainDivHasVScroll[guid] = true;
            } else {
                viewPort.css("height", "auto");
                Durados.GridHandler.setIsMainDivHasVScroll(guid, false);
                //                Durados.GridHandler.isMainDivHasVScroll[guid] = false;
            }
            Durados.GridHandler.adjustParentHeight(viewPort);
            adjustFreeBg(guid);
            return;
        }

        var containerHeight;

        var inDialog = viewPort.parents('div[role="dialog"]');

        if (inDialog.length > 0) {
            containerHeight = inDialog.first().height();
            if (dataHeight < 500) {
                viewPort.css("height", "auto");
                Durados.GridHandler.setIsMainDivHasVScroll(guid, false);
                //                Durados.GridHandler.isMainDivHasVScroll[guid] = false;
                adjustFreeBg(guid);
                return;
            }

        } else {
            containerHeight = $(window).height();
            //containerHeight = $('#mainAppDiv').height();
        }

        var newHeight;

        var pager = $('.gridPager-div:first');

        var PagerHeight = 0;
        if (pager.length == 1) {
            PagerHeight = pager.outerHeight() + parseInt(pager.css('margin-top').replace('px', '')) + parseInt(pager.css('margin-bottom').replace('px', '')) + 5;

            if (PagerHeight == 0) {
                PagerHeight = 40;
            }
        }

        var offsetHeight = viewPort.offset().top;

        if (guid == mainGuid) {
            Durados.SplitLayout.adjustSplitContentHeight('#AppTreeDiv', containerHeight);
            Durados.SplitLayout.adjustSplitContentHeight('#AppFilterTreeDiv', containerHeight);

            //changed by br
            if (views[guid] != null && views[guid].DataDisplayType == "Preview") {
                PreviewDisplay.adjustSize(guid, containerHeight);
            }
        }

        var totalHeight = dataHeight + 24 + offsetHeight + PagerHeight;

        var pagerAlwaysAtTheBottom = false;

        if (pagerAlwaysAtTheBottom && totalHeight < containerHeight) {

            //newHeight = dataHeight + 4;
            //if (viewPort[0].scrollWidth > viewPort[0].offsetWidth) {
            //    newHeight +=20;
            //}
            viewPort.css("height", "auto");

            Durados.GridHandler.setIsMainDivHasVScroll(guid, false);
            //            Durados.GridHandler.isMainDivHasVScroll[guid] = false;
            var h1 = adjustFreeBg(guid);

            return;

        } else {
            Durados.GridHandler.setIsMainDivHasVScroll(guid, true);
            //            Durados.GridHandler.isMainDivHasVScroll[guid] = true;
            offsetHeight = viewPort.offset().top;
            var footer = $('#footer').height();
            newHeight = containerHeight - offsetHeight - PagerHeight - footer + 5;

        }
        viewPort.css("overflow", "auto").height(newHeight);
        var h2 = adjustFreeBg(guid);

        Durados.GridHandler.setColumnsWidth(guid);
    }
    , isMainDivHasVScroll: {}
    , adjustParentHeight: function (child) {
        var vp = child.parents('div.fixedViewPort').first();
        if (vp.length == 1) {
            var guid = vp.attr('d_fix');
            if (guid) {
                if (Durados.GridHandler.isMainDivHasVScroll[guid]) return false;
                Durados.GridHandler.adjustDataTableHeight(guid);
            }
        }
    }
    ,
    isViewInSubGrid: function (viewPort) {
        return viewPort.parents('tr[d_role="subGrid"]').first().length == 1;
    },
    insertElementIntoViewport: function (el) {

        var mainDivs = el.parents('div.fixedViewPort');
        if (mainDivs.length > 0) {

            var isScrollChanged = false;

            var elOffset = el.offset();
            var depth = 0;


            mainDivs.each(function () {

                var divWidth = null;
                var divHeight = null;
                var divOffset = null;
                var to;
                var guid = $(this).attr('d_fix');

                if (this.scrollWidth > divWidth) {

                    divOffset = $(this).offset();
                    divHeight = $(this).height();
                    divWidth = $(this).width();

                    if (divOffset.left + divWidth < elOffset.left + 150) { //scroll right
                        to = $(this).scrollLeft() + (elOffset.left - divWidth - divOffset.left) + el.width() + 150;
                        if (to < 0) to = 0;
                        $(this).scrollLeft(to);
                        //Durados.GridHandler.scrollData(guid);
                        $(this).siblings('div.headWrapper').scrollLeft(to);
                        isScrollChanged = true;
                    } else if (elOffset.left < divOffset.left + 30) {  //scroll left
                        to = $(this).scrollLeft() - (divOffset.left - elOffset.left) - 30;
                        $(this).scrollLeft(to);
                        //Durados.GridHandler.scrollData(guid);
                        $(this).siblings('div.headWrapper').scrollLeft(to);
                        isScrollChanged = true;
                    }
                }

                if (this.scrollHeight > divHeight) {

                    if (!divOffset) {
                        divOffset = $(this).offset();
                        divHeight = $(this).height();
                        divWidth = $(this).width();
                    }

                    if (divOffset.top + divHeight < elOffset.top + 60) { //scroll down
                        to = $(this).scrollTop() + (elOffset.top - divHeight - divOffset.top) + 60;
                        if (to < 0) to = 0;
                        $(this).scrollTop(to);
                        //isScrollChanged = true;
                    } else if (divOffset.top > elOffset.top) {  //scroll up
                        to = $(this).scrollTop() - (divOffset.top - elOffset.top) - 30;
                        $(this).scrollTop(to);
                        //isScrollChanged = true;
                    }

                }

                depth++;

            });

        }
        return isScrollChanged;

    },
    showContentAsTooltip: function (el) {
        //el.scrollHeight > el.offsetHeight || el.scrollWidth > el.offsetWidth // clientWidth
        if (el.scrollWidth > el.offsetWidth && !$(el).children('.richTextContainer').length) { // && !el.getAttribute("title")
            //var div = el.firstChild;
            //if (div) {
            el.title = $(el).text();
            //}
        }
    }
}


Durados.ColumnResizer = {

    handler: null,

    initHandler: function (guid) {

        Durados.ColumnResizer.handler = $('<div class="resizehandler" />').appendTo('body');
        Durados.ColumnResizer.handler
        .mousedown(function () {
            //By br: Fix bug of drag right column
            Durados.ColumnResizer.isDragging = true;
        })
        .mouseup(function () {
            Durados.ColumnResizer.isDragging = false;
        });
        Durados.ColumnResizer.handler.draggable({
            axis: "x",
            start: function () {
                $(this).addClass("dragged");
            },
            stop: function (event, ui) {
                Durados.ColumnResizer.setColumnWidth();
                Durados.ColumnResizer.saveState(guid);
                $(this).removeClass("dragged").hide();
                Durados.ColumnResizer.isDragging = false;
            }
        });
    },

    th: null,

    guid: null,

    isDragging: false,

    init: function (guid) {

        var headersSelector = '#' + guid + 'ajaxDiv tr.gridviewhead:first th[SortColumn]';
        //        var preventResizeLastColumn = Durados.Indications.fitToWindowWidth(guid);
        //        if (preventResizeLastColumn) {
        //            headersSelector += ':not(:last)';
        //        }

        $(headersSelector).bind('mouseover', function () {

            if (Durados.ColumnResizer.handler == null) return;

            if (Durados.ColumnResizer.isDragging) return;

            Durados.ColumnResizer.guid = guid;

            Durados.ColumnResizer.th = $(this);

            var offset = Durados.ColumnResizer.th.offset();

            var l = offset.left;

            if ($('body').attr('dir') != 'rtl') {
                l += $(this).outerWidth() - 5;
                if (l + 5 >= $(window).width()) return;
            }

            var h = Durados.ColumnResizer.th.outerHeight();

            Durados.ColumnResizer.handler.css({ top: offset.top, left: l, height: h });

            Durados.ColumnResizer.handler.show();

        });
    },
    setColumnWidth: function () {

        var th = Durados.ColumnResizer.th;

        var el = Durados.ColumnResizer.handler;

        var guid = Durados.ColumnResizer.guid;

        if (!th.length) return;

        var idx = Durados.GridHandler.getCellIndex(th[0]);

        var newWidth = 0;
        var newClosestWidth = 0;
        var needResizeClosestColumn = Durados.Indications.fitToWindowWidth(guid) && th.next().length;

        if ($('body').attr('dir') != 'rtl') {
            newWidth = el.offset().left + 5 - th.offset().left; // - 21 
            if (needResizeClosestColumn) {
                newClosestWidth = th.next().offset().left - 5 - el.offset().left + th.next().outerWidth();
            }
        } else {
            newWidth = th.offset().left - el.offset().left + th.outerWidth();
            if (needResizeClosestColumn) {
                newClosestWidth = el.offset().left - th.next().offset().left; //br todo: check
            }
        }

        Durados.GridHandler.setColumnWidth(guid, idx, newWidth);

        if (needResizeClosestColumn) {
            idx = Durados.GridHandler.getCellIndex(th.next()[0]);
            Durados.GridHandler.setColumnWidth(guid, idx, newClosestWidth);
        }

        Durados.GridHandler.setColumnsWidth(guid);
    },
    saveState: function (guid, isDefault, additionalColumnsState, async) {

        //        var guid = Durados.ColumnResizer.guid;

        var state = [];

        $('#' + guid + 'ajaxDiv tr.gridviewhead:first th[SortColumn]').each(function () {
            var key = $(this).attr('SortColumn');
            //            var firstTd = $('#' + guid + 'ajaxDiv table.gridview tr.data-row td[d_name=' + key + ']');
            // Get width of firstTd in current column, or of header.
            //            var element = firstTd.length ? firstTd : $(this);
            var element = $(this);
            var width = $(element).children('div').first().width();
            state.push({
                "Key": key, "Value": { "width": width }
            });
        });

        if (additionalColumnsState != null) {
            state.push(additionalColumnsState);
        }

        var userData = "&user=";
        if (isDefault) {
            userData += 'default';
        } else {
            userData += 'current';
        }

        var viewState = { "Fields": state }

        var viewName = views[guid].ViewName;

        $.ajax({
            url: rootPath + views[guid].Controller + '/SaveCustomView/' + viewName,
            type: 'POST',
            data: 'viewName=' + encodeURIComponent(viewName) + userData + '&state=' + encodeURIComponent(Sys.Serialization.JavaScriptSerializer.serialize(viewState)),
            async: async == null ? true : async,
            cache: false,
            error: ajaxErrorsHandler,
            success: function (ret) { if (isDefault) { modalSuccessMsg("Saved"); } }

        });
    },
    restoreState: function (guid) {

        var viewName = views[guid].ViewName;

        $.ajax({
            url: rootPath + views[guid].Controller + '/RestoreCustomView/' + viewName,
            type: 'POST',
            data: 'viewName=' + encodeURIComponent(viewName),
            async: true,
            cache: false,
            error: ajaxErrorsHandler,
            success: function (ret) {
                //modalSuccessMsg("Restored"); 
                Refresh(guid);
            }
        });
    }

}




function BookmarkPage(guid) {

    var dialogName = 'BookmarkNameEdit';
    if (!$('#' + dialogName).length) {

        $('body').append('<div id="' + dialogName + '"><table><tr><td><b>Bookmark name:</b></td><td><input type="text" id="bookmarkNameTxt" value=""/></dt></tr></table></div>');
        Bookmarks.getBookmarkNotificationChecklist(dialogName);

        $('#' + dialogName).dialog({
            autoOpen: false,
            position: 'center',
            width: ($(window).width() * 0.4),
            modal: true,
            position: 'center',
            zIndex: 999999,
            title: 'Add Bookmark',
            buttons: {
                "Cancel": function () { $('#BookmarkNameEdit').dialog('close'); },
                "Add": BookmarkPageCallback
            }

        });
        ;
    }
    
    var dialog = $('#' + dialogName);
    dialog.attr("guid", guid);
    $('#bookmarkNameTxt').val(views[guid].gViewDisplayName);
    Bookmarks.initgbChecklist(dialog);
    dialog.dialog('open');

}

function BookmarkPageCallback() {

    begin();

    var guid = $('#BookmarkNameEdit').attr("guid");

    var name = $('#bookmarkNameTxt').val().trim();

    var viewName = views[guid].ViewName;

    var desc = getFriendlyFilter(guid);

    var data = {
        viewName: viewName,
        guid: guid,
        Url: location.href,
        Name: name || views[guid].gViewDisplayName,
        Desc: desc
    }

    $.ajax({
        url: rootPath + views[guid].Controller + '/CreateBookmark/' + viewName,
        contentType: 'application/json; charset=utf-8',
        data: data,
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (id) {
            if (id == '-1') { // Error

            } else {
                $('<li><a href="/Durados/LoadBookmark/?id=' + id + '" title="' + desc + '">' + (name || views[guid].gViewDisplayName) + '</a></li>').insertAfter('#bookmarksMenu li:first');
                Bookmarks.validateAndSaveMessages('add',  { pk: id, dialog: $('#BookmarkNameEdit') });
            }

            $('#BookmarkNameEdit').dialog('close');

        }

    });

    hideProgress();

}

function openBookmarksManager() {

    var jsonBookmarksFilter = 'guid=654321';

    var div = FloatingDialog.Show('/Durados/BookmarksManager/durados_Link', "Manage Bookmarks", jsonBookmarksFilter, true, 654321, true);

    div.attr('adding', 'hide');

    FloatingDialog.TheDialog.bind("dialogclose", function (event, ui) {
        var loc = location.href.split('#')[0];

        if (loc.indexOf('refreshBookmarks=refresh') > -1) {
            location.reload();
        } else {
            loc += loc.indexOf('?') == -1 ? '?' : '&';
            location.href = loc + 'refreshBookmarks=refresh';
        }
        //FloatingDialog.TheDialog.unbind( "dialogclose");
    });

}


Durados.TreeView = {
    moveNodes: {
        selectedKeys: '',
        toolBarLabel: '',
        el: null
    },
    assignLeavesToTree: function (guid, el) {
        if (!Durados.TreeView.moveNodes.selectedKeys) {
            Durados.TreeView.moveNodes.el = el;
            Durados.TreeView.moveNodes.selectedKeys = Multi.GetSelection(guid).join(",");
            if (!Durados.TreeView.moveNodes.selectedKeys) { alert("No selection made!"); return }
            Durados.TreeView.moveNodes.toolBarLabel = Durados.TreeView.moveNodes.el.innerHTML;
            Durados.TreeView.moveNodes.el.innerHTML = "Cancel"; //move
        } else { //Cancel
            Durados.TreeView.moveNodes.el.innerHTML = Durados.TreeView.moveNodes.toolBarLabel;
            Durados.TreeView.moveNodes.selectedKeys = '';
            Durados.TreeView.moveNodes.el = null;
        }
    },
    initTreeView: function (viewName, pk, guid, parentField) {
        var get_nodes_url = rootPath + views[guid].Controller + '/TreeOneLevelNodes/' + viewName;
        var edit_node_url = rootPath + views[guid].Controller + '/EditTreeNode/' + viewName + '?viewName=' + viewName;

        var guid = guid;
        var viewName = viewName;
        var parentField = parentField;

        //$.jstree._themes = rootPath + "/Content/tree/";

        $("#TreeInnerDiv").bind("before.jstree", function (e, data) {
            //$("#alog").append(data.func + "<br />");
        })
	.jstree({ "rtl": $('body').attr('dir') == 'rtl',
	    // List of active plugins
	    "plugins": [
			"themes", "json_data", "ui", "crrm", "dnd", "contextmenu" //,"hotkeys","cookies","search","types"
		],
	    // I usually configure the plugin that handles the data first
	    // This example uses JSON as it is most common
	    "json_data": {
	        // This tree is ajax enabled - as this is most common, and maybe a bit more complex
	        // All the options are almost the same as jQuery's AJAX (read the docs)
	        "ajax": {
	            // the URL to fetch the data
	            "url": get_nodes_url,
	            // the `data` function is executed in the instance's scope
	            // the parameter is the node being loaded 
	            // (may be -1, 0, or undefined when loading the root nodes)
	            "data": function (n) {
	                // the result is fed to the AJAX request `data` option
	                return {
	                    "operation": "get_children",
	                    "id": n.attr ? n.attr("id").replace("node_", "") : ""
	                };
	            }
	        }
	    },
	    // Configuring the search plugin
	    "search": {
	        // As this has been a common question - async search
	        // Same as above - the `ajax` config option is actually jQuery's AJAX object
	        "ajax": {
	            "url": get_nodes_url,
	            // You get the search string as a parameter
	            "data": function (str) {
	                return {
	                    "operation": "search",
	                    "search_str": str
						, "id": n.attr ? n.attr("id").replace("node_", "") : 1
	                };
	            }
	        }
	    },
	    // UI & core - the nodes to initially select and open will be overwritten by the cookie plugin

	    // the UI plugin - it handles selecting/deselecting/hovering nodes
	    "ui": {
	        // this makes the node with ID node_4 selected onload
	        //"initially_select" : [ "node_4" ]
	    },
	    // the core plugin - not many options here
	    "core": {
	        // just open those two nodes up
	        // as this is an AJAX enabled tree, both will be downloaded from the server
	        //"initially_open" : [ "node_2" , "node_3" ] 
	    },
	    "themes": {
	        url: rootPath + "Content/tree/themes/default" + (($('body').attr('dir') == 'rtl') ? "-rtl" : "") + "/style.css",
	        "theme": "default" + (($('body').attr('dir') == 'rtl') ? "-rtl" : ""),
	        "dots": true,
	        "icons": true
	    }
	})
	.bind("select_node.jstree", function (e, data) {
	    if (Durados.TreeView.moveNodes.selectedKeys) {
	        $.post(
			    edit_node_url,
			    {
			        "operation": "assign_to_node",
			        "id": $(data).attr("id").replace("node_", ""),
			        "keys": Durados.TreeView.moveNodes.selectedKeys,
			        "itemsView": views[guid].gViewName,
			        "guid": guid
			    },
			    function (r) {
			        Durados.TreeView.moveNodes.el.innerHTML = Durados.TreeView.moveNodes.toolBarLabel;
			        Durados.TreeView.moveNodes.selectedKeys = '';
			        Durados.TreeView.moveNodes.el = null;

			        if (r.status) {
			            refreshView(guid);
			        }
			        else {
			            ajaxNotSuccessMsg("Operation Failed", guid);
			        }
			        //$.jstree.rollback(data.rlbk);
			    });
	        return false;
	    }
	    var id = $(data.rslt.obj).attr("id").replace("node_", "");
	    refreshView(guid, parentField + '=' + id + '&children=true', "");
	    var json = GetJsonFilter(guid);
	    for (var index = 0, len = json.Fields.length; index < len; ++index) {
	        var field = json.Fields[index].Value;
	        if (field.Name == parentField)
	            field.Value = id;
	    }
	})
	.bind("create.jstree", function (e, data) {
	    $.post(
			edit_node_url,
			{
			    "operation": "create_node",
			    "id": data.rslt.parent.attr("id").replace("node_", ""),
			    "title": data.rslt.name,
			    "guid": guid
			    //"position" : data.rslt.position,
			    //"type" : data.rslt.obj.attr("rel")
			},
			function (r) {
			    if (r.status) {
			        $(data.rslt.obj).attr("id", "node_" + r.id);
			    }
			    else {
			        $.jstree.rollback(data.rlbk);
			    }
			}
		);
	})
	.bind("remove.jstree", function (e, data) {
	    $(data).each(function () {//rslt.obj.
	        var result = null;
	        $.ajax({
	            async: false,
	            type: 'POST',
	            url: edit_node_url,
	            data: {
	                "operation": "remove_node",
	                "id": this.id.replace("node_", ""),
	                "guid": guid
	            },
	            success: function (r) {
	                result = r;
	            }
	        });
	        if (!result || !result.status) {
	            //data.inst.refresh();
	            //$.jstree.rollback(data.rlbk);
	            modalErrorMsg(result);
	        } else {
	            $(this).remove();
	        }

	    });
	})
        //	.bind("remove.jstree", function(e, data) {
        //	    data.rslt.obj.each(function() {
        //	        $.ajax({
        //	            async: false,
        //	            type: 'POST',
        //	            url: edit_node_url,
        //	            data: {
        //	                "operation": "remove_node",
        //	                "id": this.id.replace("node_", ""),
        //	                "guid": guid
        //	            },
        //	            success: function(r) {
        //	                if (!r.status) {
        //	                    data.inst.refresh();
        //	                }
        //	            }
        //	        });
        //	    });
        //	})
	.bind("rename.jstree", function (e, data) {
	    $.post(
			edit_node_url,
			{
			    "operation": "rename_node",
			    "id": data.rslt.obj.attr("id").replace("node_", ""),
			    "title": data.rslt.new_name,
			    "guid": guid
			},
			function (r) {
			    if (!r.status) {
			        $.jstree.rollback(data.rlbk);
			    }
			}
		);
	})
	.bind("move_node.jstree", function (e, data) {
	    data.rslt.o.each(function (i) {
	        $.ajax({
	            async: false,
	            type: 'POST',
	            url: edit_node_url,
	            data: {
	                "operation": "move_node",
	                "id": $(this).attr("id").replace("node_", ""),
	                "ref": data.rslt.cr === -1 ? 1 : data.rslt.np.attr("id").replace("node_", ""),
	                //"position" : data.rslt.cp + i,
	                "title": data.rslt.name,
	                "copy": data.rslt.cy ? 1 : 0,
	                "guid": guid
	            },
	            success: function (r) {
	                if (!r.status) {
	                    $.jstree.rollback(data.rlbk);
	                }
	                else {
	                    $(data.rslt.oc).attr("id", "node_" + r.id);
	                    if (data.rslt.cy && $(data.rslt.oc).children("UL").length) {
	                        data.inst.refresh(data.inst._get_parent(data.rslt.oc));
	                    }
	                }
	                //$("#analyze").click();
	            }
	        });
	    });
	});

    }
}


// data
var Data; if (!Data) Data = {};

Data.GetScalar = function (viewName, pk, fieldName, guid) {
    var scalar = null;
    showProgress();
    url = rootPath + views[guid].Controller + '/GetScalar/' + viewName;
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: viewName, pk: pk, fieldName: fieldName },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            scalar = json;
        }

    });
    hideProgress();

    return scalar;
}

var Diagnostics; if (!Diagnostics) Diagnostics = {};

Diagnostics.GetButtons = function (dialog) {
    var buttons = {};  //initialize the object to hold my buttons

    buttons[translator.cancel] = function () {
        Diagnostics.Cancel();
    }  //the function that does the save

    return buttons;
}


Diagnostics.Diagnose = function (guid) {
    Diagnostics.endDiagnose = false;
    showProgress();

    var dialog = FloatingDialog.Show(null, "Diagnostics", null, true);
    var buttons = Diagnostics.GetButtons(dialog);
    dialog.dialog("option", "buttons", buttons);

    dialog.attr('class', 'diagnoseDialog');
    var status = $("<p></p>");
    dialog.append(status);
    dialog.status = status;

    var views = $("<div></div>");
    dialog.append(views);
    dialog.views = views;
    views.height(dialog.height() - 50);

    status.html('started...');

    var viewNames = '';

    var pks = Multi.GetSelection(guid);

    var url = rootPath + 'Durados/StartDiagnose/';
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { pks: Sys.Serialization.JavaScriptSerializer.serialize(pks) },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            viewNames = json;
        }

    });

    var arr = viewNames.split(',');

    Diagnostics.DiagnoseView(arr, 0, dialog);
    //    $(arr).each(function() {
    //        var viewName = this;
    //        setTimeout(function() {
    //            url = rootPath + 'Durados/Diagnose/' + viewName;
    //            $.ajax({
    //                url: url,
    //                contentType: 'application/json; charset=utf-8',
    //                data: { viewName: viewName },
    //                async: false,
    //                dataType: 'json',
    //                cache: false,
    //                error: ajaxErrorsHandler,
    //                success: function(json) {
    //                    dialog.html(viewName);
    //                }

    //            });
    //        }, 1);
    //    });




}

Diagnostics.DiagnoseView = function (arr, index, dialog) {
    if (index >= arr.length || Diagnostics.endDiagnose) {
        setTimeout(function () {
            dialog.status.html('endded...');
        }, 1);

        setTimeout(function () {
            url = rootPath + 'Durados/EndDiagnose/';
            $.ajax({
                url: url,
                contentType: 'application/json; charset=utf-8',
                data: {},
                async: false,
                dataType: 'json',
                cache: false,
                error: ajaxErrorsHandler,
                success: function (json) {
                    setTimeout(function () {
                        dialog.dialog('close');
                        hideProgress();
                    }, 1000);
                }

            });
        }, 1000);

        return;
    }

    var viewName = arr[index];
    url = rootPath + 'Durados/Diagnose/' + viewName;
    $.ajax({
        url: url,
        contentType: 'application/html; charset=utf-8',
        data: { viewName: viewName },
        async: false,
        dataType: 'html',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (html) {
            ++index;
            dialog.status.html(index + " out of " + arr.length);
            dialog.views.prepend($('<p>' + viewName + '</p>'));
            setTimeout(function () {
                Diagnostics.DiagnoseView(arr, index, dialog);
            }, 1);
        }

    });
}

Diagnostics.Cancel = function () {
    Diagnostics.endDiagnose = true;
}

function Dictionary(url, guid) {
    var pks = Multi.GetSelection(guid);

    var pks = Multi.GetSelection(guid);
    var pk;
    if (pks.length == 1) {
        pk = pks[0];
    }
    else {
        modalErrorMsg("Please select a view.");
        return;
    }

    var viewName = GetViewNameByPK(pk);
    url = url.replace('xxx', viewName);

    window.location = url;

}

function GetViewNameByPK(pk) {
    var viewName = '';

    url = rootPath + 'Durados/GetViewNameByPK/';
    $.ajax({
        url: url,
        contentType: 'application/html; charset=utf-8',
        data: { pk: pk },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            viewName = json;
        }

    });

    return viewName;
}

var Messages; if (!Messages) Messages = {};

Messages.Show = function (viewName, guid) {
    showProgress();
    setTimeout(function () {
        var url = rootPath + 'MessageBoard/FilterByView/' + viewName + '?viewName=' + viewName;
        var dialog = SearchDialog.CreateAndOpen(viewName, "Messages", '', '', url, guid, null, null, null, null);
        hideProgress();
    }, 10);
}

function sync(url, guid) {
    var pks = Multi.GetSelection(guid);

    var pks = Multi.GetSelection(guid);
    if (pks.length == 0) {
        modalErrorMsg("Please select a view.");
        return;
    }
    //    var pk;
    //    if (pks.length == 1) {
    //        pk = pks[0];
    //    }
    //    else {
    //        modalErrorMsg("Please select a view.");
    //        return;
    //    }

    $.ajax({
        url: url,
        contentType: 'application/html; charset=utf-8',
        data: { configViewPks: String(pks) },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            if (json.success)
                modalSuccessMsg(json.message);
            else
                modalErrorMsg(json.message);
        }

    });

}


function showSpryFormats(span) {
    var tt = $('#SpryFormatsTooltip');
    if (!tt.length) return;
    var pos = $(span).offset();
    tt.css("top", pos.top + 18).css("left", pos.left).show();
}

function hideSpryFormats(span) {
    $('#SpryFormatsTooltip').hide();
}

//Durados.DatePickerDone = function(id) {
//    $(id).val($.datepicker.formatDate(translator.gDateFormat, new Date(1930, 0, 1)));
//}

var DuradosNADate = new Date(1930, 0, 1);

//AddDatePickerNAButton
datePickerBeforeShow = function (input) {
    setTimeout(function () {
        if ($(input).datepicker("widget").find("button.ui-datepicker-na").length) return;
        var buttonPane = $(input).datepicker("widget").find(".ui-datepicker-buttonpane");
        var btn = $('<button class="ui-datepicker-current ui-state-default ui-priority-secondary ui-corner-all ui-datepicker-na" type="button">N/A</button>');
        btn.unbind("click").bind("click", function () {
            $(input).datepicker("setDate", DuradosNADate);
            $(input).datepicker("hide");

            //$(input).val($.datepicker.formatDate(translator.gDateFormat, DuradosNADate));
            //$.datepicker._hideDatepicker(input);
            triggerDateChanged('#' + input.id);
            //$.datepicker._clearDate( input );
        });

        btn.appendTo(buttonPane);

    }, 1);

    //var d = $(input).val();
    //if (d == $.datepicker.formatDate(translator.gDateFormat, DuradosNADate)) {
    //    $.datepicker._gotoToday(input);
    //}
}


datePickerOnChangeMonthYear = function (input) {
    setTimeout(function () {
        if ($(input).datepicker("widget").find("button.ui-datepicker-na").length) return;
        var buttonPane = $(input).datepicker("widget").find(".ui-datepicker-buttonpane");
        var btn = $('<button class="ui-datepicker-current ui-state-default ui-priority-secondary ui-corner-all ui-datepicker-na" type="button">N/A</button>');
        btn.unbind("click").bind("click", function () {
            //$(input).val($.datepicker.formatDate(translator.gDateFormat, DuradosNADate));
            $(input).datepicker("setDate", DuradosNADate);
            $(input).datepicker("hide");

            //$.datepicker._hideDatepicker(input);
            triggerDateChanged('#' + input.id);
            //$.datepicker._clearDate( input );
        });

        btn.appendTo(buttonPane);

    }, 1);
}

var EditView; if (!EditView) EditView = {};

EditView.open = function (pk, viewName, title) {
    var inlineEditUrl = '/Admin/InlineEditingEdit/';
    var dialog = InlineEditingDialog.CreateAndOpen2(viewName, title, null, null, inlineEditUrl, null, null, pk, true, function (viewName, type, id, editUrl, guid, dialog, pk) {
        var refreshUrl = '/Admin/RefreshConfig/';
        showProgress();
        refreshConfig(refreshUrl, null, true);
        reloadPage();
        hideProgress();
    });
    dialog.dialog("option", "width", 1000);
    var isAccordion = InitAccordion(null, 1000, pk);
    if (!isAccordion) {
        InitTabs(null, 1000)
    }
    slider.InitSlider(null);
}


var Page; if (!Page) Page = {};

Page.edit = function (pageId) {
    var inlineEditUrl = '/Admin/InlineEditingEdit/';
    var dialog = InlineEditingDialog.CreateAndOpen2("Page", "Page", null, null, inlineEditUrl, null, null, pageId, true, function (viewName, type, id, editUrl, guid, dialog, pk) {
        var refreshUrl = '/Admin/RefreshConfig/';
        showProgress();
        refreshConfig(refreshUrl, null, true);
        reloadPage();
        hideProgress();
    });

    dialog.dialog("option", "width", 1000);
    var isAccordion = InitAccordion(null, 1000, pageId);
    if (!isAccordion) {
        InitTabs(null, 1000);
    }
    slider.InitSlider(null);
    var data = { dialog: dialog, viewName: "Page" };
    initPageDependency(data);
}

var UploadConfig; if (!UploadConfig) UploadConfig = {};

UploadConfig.Open = function (viewName, guid) {
    d_Dialog.CreateAndOpen(viewName, guid, "durados_UploadConfig", "Upload Configuration");

    $(d_Dialog).bind('PerformMethod', function (e, data) {

        var fileName = data.dialog.find('#' + guid + inlineAddingPrefix + "FileName").val();
        if (fileName == "") {
            ajaxNotSuccessMsg("Please upload the configuration zip file");
            return;
        }

        showProgress();

        $.post(rootPath + views[guid].Controller + '/UploadConfig/',
        {
            fileName: fileName
        },
        function (json) {
            hideProgress();
            if (!json.success) {
                ajaxNotSuccessMsg(json.message);
            } else {
                modalSuccessMsg(json.message);
                
                $.ajax({
                    url: '/Admin/Restart',
                    contentType: 'application/html; charset=utf-8',
                    dataType: 'json',
                    // contentType: 'application/json',
                    async: false,
                    success: function (data) {
                        refreshMainContent();
                    },

                    error: function (data, status, jqXHR) {
                        //alert('There was an error.');
                        refreshMainContent();
                    }
                });
                
            }
        });

    });

    hideProgress();
}

var Charts; if (!Charts) Charts = {};

Charts.run = function () {
    Dashboard.run("chart", "Charts");

    $('.chart').each(function () {
        var chartId = $(this).attr('id');

        Charts.load(chartId);
    });
}

Charts.load = function (chartId, isRefreshData) {
    var url = '/Home/GetChartData/';
    var isNotRefreshData = !isRefreshData || isRefreshData == false;
    var queryString = location.search;
    var retData = null;
    $.ajax({
        url: url,
        contentType: 'application/html; charset=utf-8',
        data: { chartId: chartId, queryString: queryString },
        dataType: 'json',
        // contentType: 'application/json',
        async: isNotRefreshData,
        success: function (data) {
            if (isNotRefreshData)
                $('#' + chartId).empty();
            if (data.Type) {
                $('#' + chartId).height(data.Height);
                if (isNotRefreshData) {
                    if (data.table && data.table == true) {
                    }
                    else {
                        $("#" + chartId).show();
                        Charts.build(data, chartId);
                    }
                    if (data.ShowTable) {
                        $("#" + chartId + "Table").load("/Home/GetChartData/", { chartId: chartId, queryString: queryString, isTable: true }).show();
                        if (data.Type == "Table") {
                            $("#" + chartId).hide();
                            $("#" + chartId + "Table").height(data.height);
                        }
                    }
                    else { $("#" + chartId + "Table").hide(); }
                }
                else
                    retData = data;

            }
            else {
                Charts.message(chartId, data);
            }
        },

        error: function (data, status, jqXHR) {
            //alert('There was an error.');
        }
    });
    if (!isNotRefreshData && retData != null)
        return retData;
}

Charts.deleteChart = function (chartId) {
    $.ajax({
        url: "/Admin/DeleteChart?chartId=" + chartId.replace('Chart', ''),
        contentType: 'application/html; charset=utf-8',
        data: { chartId: chartId },
        async: true,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (json) {
            location.reload();
        }
    });
    
}

Charts.edit = function (chartId) {
    var inlineEditUrl = '/Admin/InlineEditingEdit/';
    var heightBefore = $('#' + chartId).height();
    var dialog = InlineEditingDialog.CreateAndOpen2("Chart", "Chart", null, null, inlineEditUrl, null, null, chartId.replace('Chart', ''), true, function (viewName, type, id, editUrl, guid, dialog, pk) {

        var refreshUrl = '/Admin/RefreshConfig/';
        showProgress();
        refreshConfig(refreshUrl, null, true);

        var heightAfter = dialog.find('input[name="Height"]').val();

        if (parseInt(heightAfter) == parseInt(heightBefore)) {
            Charts.load(chartId);
        }
        else {
            location.reload();
        }

        hideProgress();
    });
    initChartDependency(dialog,chartId);
    dialog.dialog("option", "width", 840);
    dialog.dialog("option", "height", 575);

    new Spry.Widget.ValidationTextField("nullinlineEditing_Height", 'integer', { isRequired: true, useCharacterMasking: true, minValue: 160, maxValue: 1000 });
    $(Durados.View).bind('getErrorsList', function (e, data) {
        data.cancel = true;
        data.message = "Height must be between 160 to 1000 pixels";
    });
}

Charts.message = function (chartId, message) {
    $('#' + chartId).empty();
    $('#' + chartId).html('<div align="center" style="verticalAlign: middle">' + message + '</div>')
}

Charts.savePosition = function (sortable, ui) {
    var url = "/Admin/SetChartData/";
    var data = [];
   // var i = 0;
    var o = 0;
    $(".column").each(function () {
        
        $(this).find(".portlet").each(function () {
            var chartId = $(this).attr('chartId');
            if (chartId) {
                var column = $(this).parent('div').attr('columnid');
                data.push({ Id: chartId.replace('Chart', ''), Column: column, Ordinal: o }); //? "Left" : "Right"var data = [];
                o = o + 1;
            }
        });
        //i++;
    });
    $.post(url, { data: Sys.Serialization.JavaScriptSerializer.serialize(data) });
}

var myCharts = [];

Charts.build = function (data, id) {

    Highcharts.theme = {
        colors: ["rgb(27,160,133)", "rgb(243,101,35)", "rgb(93,187,169)", "rgb(248,138,85)", "rgb(13,119,97)", "rgb(243,185,88)", "#eeaaee",
      "#55BF3B", "#DF5353", "#7798BF", "#aaeeee"],
        chart: {
            backgroundColor: {
                linearGradient: [241, 241, 241, 400],
                stops: [
            [0, 'rgb(241, 241, 241)'],
            [1, 'rgb(241, 241, 241)']
         ]
            },
            borderWidth: 0,
            borderRadius: 0,
            plotBackgroundColor: null,
            plotShadow: false,
            plotBorderWidth: 0
        },
        title: {
            style: {
                color: 'Black',
                font: '16px Arial bold'
            }
        },
        subtitle: {
            style: {
                color: 'Black',
                font: '12px Arial'
            }
        },
        xAxis: {
            gridLineWidth: 0,
            lineColor: 'rgb(86, 86, 86)',
            tickColor: 'rgb(86, 86, 86)',
            labels: {
                style: {
                    color: 'rgb(86, 86, 86)',
                    fontWeight: 'bold'
                }
            },
            title: {
                style: {
                    color: 'rgb(86, 86, 86)',
                    font: 'bold 12px Arial'
                }
            }
        },
        yAxis: {
            alternateGridColor: null,
            minorTickInterval: null,
            gridLineColor: 'rgba(86, 86, 86, .2)',
            lineWidth: 0,
            tickWidth: 0,
            labels: {
                style: {
                    color: 'rgb(86, 86, 86)',
                    fontWeight: 'bold'
                }
            },
            title: {
                style: {
                    color: 'rgb(86, 86, 86)',
                    font: 'bold 12px Arial'
                }
            }
        },
        legend: {
            itemStyle: {
                color: 'rgb(86, 86, 86)'
            },
            itemHoverStyle: {
                color: 'rgb(86, 86, 86)'
            },
            itemHiddenStyle: {
                color: 'rgb(86, 86, 86)'
            },
            borderWidth: 0,
            backgroundColor: 'rgb(255, 255, 255)',
            shadow: false,
            align: 'center',
            y:0
           
       
        },
        labels: {
            style: {
                color: 'rgb(86, 86, 86)'
            }
        },
        tooltip: {
            backgroundColor: {
                linearGradient: [255, 255, 255, 50],
                stops: [
            [0, 'rgba(255, 255, 255, .8)'],
            [1, 'rgba(255, 255, 255, .8)']
         ]
            },
            borderWidth: 0,
            style: {
                color: 'rgb(86, 86, 86)'
            }
        },


        plotOptions: {
            line: {
                dataLabels: {
                    color: '#CCC'
                },
                marker: {
                    lineColor: '#333'
                }
            },
            spline: {
                marker: {
                    lineColor: '#333'
                }
            },
            scatter: {
                marker: {
                    lineColor: '#333'
                }
            },
            candlestick: {
                lineColor: 'white'
            }
        },

        toolbar: {
            itemStyle: {
                color: '#CCC'
            }
        },

        navigation: {
            buttonOptions: {
                backgroundColor: {
                    linearGradient: [0, 0, 0, 20],
                    stops: [
               [0.4, '#606060'],
               [0.6, '#333333']
            ]
                },
                borderColor: '#000000',
                symbolStroke: '#C0C0C0',
                hoverSymbolStroke: '#FFFFFF'
            }
        },

        exporting: {
            buttons: {
                exportButton: {
                    symbolFill: '#55BE3B'
                },
                printButton: {
                    symbolFill: '#7797BE'
                }
            }
        },

        // scroll charts
        rangeSelector: {
            buttonTheme: {
                fill: {
                    linearGradient: [0, 0, 0, 20],
                    stops: [
               [0.4, '#888'],
               [0.6, '#555']
            ]
                },
                stroke: '#000000',
                style: {
                    color: '#CCC',
                    fontWeight: 'bold'
                },
                states: {
                    hover: {
                        fill: {
                            linearGradient: [0, 0, 0, 20],
                            stops: [
                     [0.4, '#BBB'],
                     [0.6, '#888']
                  ]
                        },
                        stroke: '#000000',
                        style: {
                            color: 'white'
                        }
                    },
                    select: {
                        fill: {
                            linearGradient: [0, 0, 0, 20],
                            stops: [
                     [0.1, '#000'],
                     [0.3, '#333']
                  ]
                        },
                        stroke: '#000000',
                        style: {
                            color: 'yellow'
                        }
                    }
                }
            },
            inputStyle: {
                backgroundColor: '#333',
                color: 'silver'
            },
            labelStyle: {
                color: 'silver'
            }
        },

        navigator: {
            handles: {
                backgroundColor: '#666',
                borderColor: '#AAA'
            },
            outlineColor: '#CCC',
            maskFill: 'rgba(16, 16, 16, 0.5)',
            series: {
                color: '#7798BF',
                lineColor: '#A6C7ED'
            }
        },

        scrollbar: {
            barBackgroundColor: {
                linearGradient: [0, 0, 0, 20],
                stops: [
               [0.4, '#888'],
               [0.6, '#555']
            ]
            },
            barBorderColor: '#CCC',
            buttonArrowColor: '#CCC',
            buttonBackgroundColor: {
                linearGradient: [0, 0, 0, 20],
                stops: [
               [0.4, '#888'],
               [0.6, '#555']
            ]
            },
            buttonBorderColor: '#CCC',
            rifleColor: '#FFF',
            trackBackgroundColor: {
                linearGradient: [0, 0, 0, 10],
                stops: [
            [0, '#000'],
            [1, '#333']
         ]
            },
            trackBorderColor: '#666'
        },

        // special colors for some of the demo examples
        legendBackgroundColor: 'rgba(48, 48, 48, 0.8)',
        legendBackgroundColorSolid: 'rgb(70, 70, 70)',
        dataLabelsColor: '#444',
        textColor: '#E0E0E0',
        maskColor: 'rgba(255,255,255,0.3)'
    };

    // Apply the theme
    var highchartsOptions = Highcharts.setOptions(Highcharts.theme);

    if (myCharts[id]) {
        myCharts[id].destroy();
        myCharts[id] = null;
    }

    myCharts[id] = Charts.GetChart(data, id);

    $('.highcharts-data-labels div span').css('color', '#999');
}

Charts.GetChart = function (data, id) {
    switch (data.Type.toLowerCase()) {
        case 'gauge':
            return Charts.GetGuageChart(data, id);
        default:
            return Charts.GetBasicChart(data, id);

    }
}
Charts.GetGuageChart = function (data, id) {
    //    var c = Charts.GetBasicChart(data, id);
    //    var cx= {  // the value axis

    //                    
    // //   return jQuery.extend(c,cx); 
    return new Highcharts.Chart({
        chart: {
            renderTo: id,
            type: 'gauge'

        },

        title: {
            text: data.Title
        },
        subtitle: {
            text: data.SubTitle //'Subtitle'
        },

        pane: {
            startAngle: -150,
            endAngle: 150,
            background: [{
                backgroundColor: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
	                    [0, '#FFF'],
	                    [1, '#333']
	                ]
                },
                borderWidth: 0,
                outerRadius: '109%'
            }, {
                backgroundColor: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
	                    [0, '#333'],
	                    [1, '#FFF']
	                ]
                },
                borderWidth: 1,
                outerRadius: '107%'
            }, {
                // default background
            }, {
                backgroundColor: '#DDD',
                borderWidth: 0,
                outerRadius: '105%',
                innerRadius: '103%'
            }]
        },

        // the value axis
        yAxis: {
            min: data.MinValue,
            max: data.MaxValue,

            minorTickInterval: 'auto',
            minorTickWidth: 1,
            minorTickLength: 10,
            minorTickPosition: 'inside',
            minorTickColor: '#666',

            tickPixelInterval: 30,
            tickWidth: 2,
            tickPosition: 'inside',
            tickLength: 10,
            tickColor: '#666',
            labels: {
                step: 2,
                rotation: 'auto'
            },
            //            title: {
            //                text: 'km/h'
            //            },
            plotBands: [{
                from: data.greenFrom,
                to: data.greenTo,
                color: '#55BF3B' // green
            }, {
                from: data.yellowFrom,
                to: data.yellowTo,
                color: '#DDDF0D' // yellow
            }, {
                from: data.redFrom,
                to: data.redTo,
                color: '#DF5353' // red
            }]
        },

        series: data.Data

    },
    // Add some life
	function (chart) {
	    if (!chart.renderer.forExport) {
	        if (data.RefreshInterval <= 3)
	            return;
	        var intervalHendler = "refreshInterval" + id;
	        clearInterval(Charts[intervalHendler]);
	        Charts[intervalHendler] = setInterval(function () {
	            if (chart.series === undefined)
	                return;
	            var point = chart.series[0].points[0];
	            var data = Charts.load(id, true);
	            var newVal = data.Data[0].data[0];
	            point.update(newVal);

	        }, data.RefreshInterval * 1000);
	    }
	});
} 
Charts.GetBasicChart = function (data,id) {
    return new Highcharts.Chart({
        chart: {
            renderTo: id,
            type: data.Type//'bar'
        },
       
        title: {
            text: data.Title//'Title'
        },
        subtitle: {
            text: data.SubTitle //'Subtitle'
        },
        xAxis: {
            categories: data.xAxis,
            //categories: ['Africa', 'America', 'Asia', 'Europe', 'Oceania'],
            title: {
                text: data.XTitle
            }
        },
        yAxis: {
            min: data.Neg ? null : 0,
            title: {
                text: data.YTitle, //'Population (millions)',
                align: 'high'
            }
        },
        tooltip: {
            formatter: function () {
                return '' +
        this.series.name + ': ' + this.y;
            }
        },
        plotOptions: {
            bar: {
                dataLabels: {
                    enabled: true
                }
            }
        },
//        legend: {
//          //  layout: 'vertical',
//          //  align: 'right',
//          //  verticalAlign: 'top',
//           // x: -100,
//            y: 0,
//           // floating: true,
//            borderWidth: 1,
//            backgroundColor: '#FFFFFF',
//            shadow: true
//        },
        credits: {
            enabled: false
        },
        series: data.Data

        
    });

   
}





var Dashboard; if (!Dashboard) Dashboard = {};

/************************************************************************************/
/*		Dashboard.run (by br)						
/*		run sortable on dashboard items, enable expand/collapse on dashboard items
/************************************************************************************/
Dashboard.run = function (header, containerId, guid) {
    var selectorPrefix = containerId == null ? "" : "#" + containerId + " ";
    var disabled = false;
    var disabledSelector = selectorPrefix + "[isEnabled]";
    var disabledElement = $(disabledSelector);

    if (disabledElement.length) {
        disabled = disabledElement.attr("isEnabled") == "False";
    }

    Dashboard.initSortable(header, selectorPrefix, guid, disabled);

    Dashboard.initHeaders(header, selectorPrefix);

    var dialog = $('#' + guid + 'DataRowEdit');
    var deleted = dialog.find('input[name="Excluded"]');

    $('table.portlet-content').each(function () {
        if ($(this).find('tr').length <= 1) {
            var ic = $(this).prev().find('span.ui-icon-minusthick');
            var partofpk = $(this).parent().attr('partofpk') == 'partofpk';
            if (views[guid].ViewName == "Field" && !partofpk) {
                ic.removeClass('ui-icon-minusthick').removeClass('ui-icon').addClass('ui-icon-delete').attr('title', translator.Delete).unbind('click').bind('click', function () {

                    Durados.Dialogs.Confirm("Delete Column Confirmation", "Are you sure that you want to delete this column?", function () {
                        Durados.CheckBox.SetChecked(deleted, true);
                        dialog.attr('changed', 'no');
                        deleted.change();
                    }, null);
                });
            }
            else {
                ic.hide();
            }
        }
    });

    if (views.length > 0)
        if (views[guid].ViewName == "Field" && views[guid].Role == "View Owner") {
            deleted.parents('tr:first').hide();
        }

    if (header == "chart") {
        setTimeout(function () {
            Dashboard.loadPortlets();
        }, 1);
    }
}

/************************************************************************************/
/*		Dashboard.initHeaders (by br)						
/*	    Enable expand/collapse on dashboard items headers
/************************************************************************************/
Dashboard.initHeaders = function (header, selectorPrefix) {
    var headerSpan = "<span class='ui-icon ui-icon-minusthick'></span>";

    if (header == "chart") {
        var editChartCaption = $(selectorPrefix + "#editChartCaption").val();
        if (editChartCaption)
            headerSpan = "<a class='delete-chart'><span class='button-icon' title='Delete Chart'></span><a/><a class='ui-chart edit-chart' href='#'><span class='button-icon'></span>" + editChartCaption + "</a>";
        else
            headerSpan = "<span></span>";
    }

    $(selectorPrefix + ".portlet").addClass("ui-widget ui-widget-content ui-helper-clearfix ui-corner-all")
			    .find(".portlet-header")
				    .addClass("ui-widget-header ui-corner-all")
				    .prepend(headerSpan)
				    .end()
			    .find(".portlet-content");

    if (header != "chart") {
        $(selectorPrefix + ".portlet-header .ui-icon").click(function () {
            $(this).toggleClass("ui-icon-minusthick").toggleClass("ui-icon-plusthick");
            $(this).parents(".portlet:first").find(".portlet-content").toggle();
        });
    }
    else {
        $(selectorPrefix + ".portlet-header .ui-chart").click(function () {
            var chartId = $(this).parents('.portlet:first').find('.chart:first').attr('id'); ;

            Charts.edit(chartId);
        });

        $(selectorPrefix + ".portlet-header .delete-chart").click(function () {
            var chartId = $(this).parents('.portlet:first').find('.chart:first').attr('id'); ;
    
            Durados.Dialogs.Confirm("Delete Chart", "Are you sure the you want to delete this chart?", function () {
                Charts.deleteChart(chartId);
            });
        })
    }
}

/************************************************************************************/
/*		Dashboard.initSortable (by br)						
/*	    Run sortable on dashboard items
/************************************************************************************/
Dashboard.initSortable = function (header, selectorPrefix, guid, disabled) {
    var columnId = 0;

    $(selectorPrefix + ".column").mousedown(function (e) {
        //        var clickedElement, evt = e ? e : event;
        //        if (evt.srcElement) clickedElement = evt.srcElement;
        //        else if (evt.target) clickedElement = evt.target;

        //        if ($(clickedElement).hasClass('boardtitle')) {
        //            $(clickedElement).addClass('boardselected');
        //            //            PreviewDisplay.selectedPk = $(clickedElement).attr('pk');
        //        }
        //        else if ($(clickedElement).hasClass('boardrow2')) {
        //            $(clickedElement).find(boardtitle).addClass('boardselected');
        ////            PreviewDisplay.selectedPk = $(clickedElement).attr('d_pk');
        //        }
        try {
            if (document.activeElement && document.activeElement.blur)
                var ae = document.activeElement;
            if (ae && ae.blur && ae.nodeName != 'BODY') {
                document.activeElement.blur();
            }
        }
        catch (err) { }
    }).sortable({
        connectWith: ".column",
        disabled: disabled,
        distance: 15,
        cancel: '[cancelDrag=cancelDrag]',
        update: function (event, ui) {
            if (header != "chart") {
                Dashboard.savePosition(this, ui, guid);

                try {
                    if (views[guid].ViewName == 'Field' && plugIn()) {
                        showProgress()
                        setTimeout(function () {
                            plugInRefresh();
                        }, 100);
                    }
                    else if (isDockFields(guid)) {
                        try {
                            if (adminPreviewChanged) {
                                var dialog = $('#' + guid + 'DataRowEdit');
                                dialog.parent().find('button.ui-button').each(function () {
                                    if ($(this).text() == translator.save) {
                                        $(this).click();
                                        return;
                                    }
                                });
                            }
                            window.parent.parent.Refresh(window.parent.parent.getMainPageGuid(), true);
                        }
                        catch (err) {
                        }
                    }
                }
                catch (err) {
                }

            }
            else {
                Charts.savePosition(this, ui);
            }
        }
    })
    .disableSelection()
    .each(function () {
        $(this).attr('columnId', ++columnId);
        if (guid != null && views[guid] != null) {
            views[guid].sortDashboard[columnId.toString()] = $(this).sortable("toArray");
        }
    });
}

/************************************************************************************/
/*		Dashboard.savePosition (by br)						
/*		After drag dashboard item- save new position in DB
/************************************************************************************/
Dashboard.savePosition = function (element, ui, guid) {
    if (guid == null) {
        return;
    }

    //Get url
    //trySaveChanges(guid);
    var viewName = views[guid].ViewName;
    var controller = views[guid].Controller;
    var url = rootPath + controller + "/ChangeOrdinal/" + viewName;

    var draggedElementId = ui.item.attr("id");
    var columnId = $(element).attr("columnId");
    var prevOrder = views[guid].sortDashboard[columnId];
    var prevIndex = prevOrder.join(",").indexOf(draggedElementId);
    var currentOrder = $(element).sortable("toArray");
    var currentIndex = currentOrder.join(",").indexOf(draggedElementId);
    var direction = prevIndex < currentIndex ? "down" : "up";

    //Get draggedElementPk, targetElenentPk
    var draggedElementPk = ui.item.attr("pk");
    var targetElenentPk = direction == "up" ? ui.item.next().attr("pk") : ui.item.prev().attr("pk");

    views[guid].sortDashboard[columnId] = currentOrder;
    
    $.post(url, { o_pk: draggedElementPk, d_pk: targetElenentPk, guid: guid }, function () {
        try {
            if (views[guid].ViewName == 'Field' && plugIn()) {
                hideProgress()

            }
        }
        catch (err) {
        }
    });
}

Dashboard.loadPortlets = function () {
    $(".portlet[d_type='grid']").each(function () {
        var portlet = $(this);
        Dashboard.loadPortlet(portlet);
    });
}

Dashboard.loadPortlet = function (portlet) {
    var url = portlet.find(".portlet-link").find("a").attr('href');
    var viewName = portlet.find("input[name='ViewName']").val();
    var guid = portlet.find("input[name='guid']").val();
    var grid = portlet.find(".portlet-grid");

    $.ajax({
        url: url,
        contentType: 'application/html; charset=utf-8',
        data: { viewName: viewName, guid: guid, mainPage: "False", safety: false },
        async: true,
        dataType: 'html',
        cache: false,
        error: ajaxErrorsHandler,
        success: function (html) {
            grid.html(html);
            success(guid);
            resetElementScrollsPosition(guid);
        }
    });
}


Durados.Dependencies = {

    fieldsHandled: [],

    createHandlers: function (field, guid, fields, viewName, prefix) {

        if (!field || !field.length) return;

        var d_event = function () {
            var dialog = $(this).parents('div[role="dialog"]').first();
            if (!dialog.length) return;
            var fieldVal;
            if ($(this).attr('multiple')) {
                fieldVal = $(this).val();
                if (fieldVal) fieldVal = fieldVal.join(",");
            } else if ($(this).attr('type') == 'checkbox') {
                //fieldVal = isChecked($(this));
                fieldVal = Durados.CheckBox.IsChecked($(this));
            } else {
                fieldVal = $(this).val();
            }

            var fieldName = $(this).attr('name');
            Durados.Dependencies.Apply(fieldName, fieldVal, guid, dialog, fields);
            var elm = this;
            viewName = viewName || views[guid].ViewName;

            $(Durados.Dependencies).trigger('OnDependenciesChange', { guid: guid, dialog: dialog, viewName: viewName, fieldVal: fieldVal, fieldName: fieldName, elm: elm, prefix: prefix });

        }

        Durados.Dependencies.fieldsHandled.push(field);

        field.focus(function () {
            // Get the value when input gains focus
            var oldValue = $(this).data('oldVal');
        }).change(d_event);

    },
    Apply: function (fieldName, fieldVal, guid, dialog, jsonFields) {

        if (!jsonFields && !views[guid].jsonViewForEdit) return;

        var allFields = jsonFields || views[guid].jsonViewForEdit.Fields;

        var data;

        var exist = false;
        for (var index = 0, len = allFields.length; index < len; index++) {
            if (allFields[index].Key == fieldName) {
                exist = true;
                data = allFields[index].Value.DependencyData;
                break;
            }
        }

        if (!exist || !data) return;

        if (!fieldVal) fieldVal = '';

        var vals = fieldVal.split(",");

        var rules = data.split('|');

        var hideMode = parseInt(rules[0]) || 0;

        //Enable first
        $(dialog).find('select,input,textarea,li[childrenFieldName],div[childrenFieldName]').each(function () {
            var el = $(this);
            var ds = el.attr("disSource");
            if (ds) {
                if (ds == fieldName + '~') {
                    Durados.Dependencies.toggleField(el, 'enable', hideMode);
                    el.attr("disSource", '');
                } else if (ds.indexOf(fieldName + '~') > -1) {
                    el.attr("disSource", ds.replace(fieldName + '~', ''));
                }
            }
        });

        for (var index = 0, len = allFields.length; index < len; index++) {

            if (data.indexOf(allFields[index].Key) < 0) continue;

            for (var i = 1; i < rules.length; i++) {
                var ruleData = rules[i].split(';'); //.replace(/\s/g, '')

                var values = ruleData[0].split(',');

                for (var j = 0; j < values.length; j++) {
                    if (jQuery.inArray(values[j], vals) > -1) {
                        var fields = ruleData[1].split(',');
                        for (var k = 0; k < fields.length; k++) {
                            $(dialog).find('[name="' + fields[k] + '"],[childrenFieldName="' + fields[k] + '"]').each(function () {
                                var el = $(this);
                                var ds = el.attr("disSource") || '';
                                if (!ds) {
                                    if (!el.attr("disabled")) {
                                        Durados.Dependencies.toggleField(el, 'disable', hideMode);
                                        el.attr("disSource", fieldName + '~');
                                    }
                                } else if (ds.indexOf(fieldName + '~') == -1) {
                                    el.attr("disSource", ds + fieldName + '~');
                                }
                            });
                        }
                    }
                }

            }
        }
    },
    toggleField: function ($el, action, mode) {

        if ($el.is('div')) Durados.Dependencies.toggleSubgrid($el, action, mode);

        if ($el.is('li')) Durados.Dependencies.toggleTab($el, action, mode);

        if (action == 'enable') {

            if (!mode) {
                if ($el.hasClass('dropdownchecklist')) {
                    $el.dropdownchecklist("enable");
                } else {
                    $el.removeAttr("disabled");
                }
                $el.parents('TD.rowViewCell').first().find('img').show();
            } else if (mode == 1) {
                $el.parents('TD.rowViewCell').first().css("visibility", "visible").prev().css("visibility", "visible");
            } else if (mode == 2) {
                $el.parents('TD.rowViewCell').first().css("display", "").prev().css("display", "");
            }
        } else if (action == 'disable') {

            if (!mode) {
                if ($el.hasClass('dropdownchecklist')) {
                    $el.dropdownchecklist("disable");
                } else {
                    $el.attr("disabled", "disabled");
                }
                $el.parents('TD.rowViewCell').first().find('img').hide();
            } else if (mode == 1) {
                $el.parents('TD.rowViewCell').first().css("visibility", "hidden").prev().css("visibility", "hidden");
            } else if (mode == 2) {
                $el.parents('TD.rowViewCell').first().css("display", "none").prev().css("display", "none");
            }
        }
    },
    toggleTab: function (li, action, mode) {

        var url = li.attr('newUrl');

        li.find("a").attr('nocache', 'nocache');

        if (action == 'enable') {
            if (li.attr('childrenDisabled') == 'childrenDisabled') {
                url = url + "&disabled=False";
                var index = parseInt(li.attr('index'));

                li.parents("div:first").tabs("url", index, url);

                li.removeAttr('childrenDisabled')
            }
            else {
                li.find("a").removeAttr('nocache');
            }
        } else if (action == 'disable') {
            if (li.attr('childrenDisabled') != 'childrenDisabled') {
                url = url + "&disabled=True";
                var index = parseInt(li.attr('index'));

                li.parents("div:first").tabs("url", index, url);

                li.attr('childrenDisabled', 'childrenDisabled');
            }
        }
    },
    toggleSubgrid: function (div, action, mode) {
        //var expander = div.find('.inlineAddingImg').first();        
        if (action == 'enable') {
            //expand(expander, true, null, false, guid);
            div.parents('td').first().css('visibility', 'visible').prev('td').css('visibility', 'visible');
        } else if (action == 'disable') {
            //expand(expander, true, null, true, guid);
            div.parents('td').first().css('visibility', 'hidden').prev('td').css('visibility', 'hidden');
        }
    },
    cleanUp: function () {
        while (Durados.Dependencies.fieldsHandled.length) {
            var f = Durados.Dependencies.fieldsHandled.pop();
            f.unbind("change"); //TODO unbind only dependency field change event
            if (f.attr("disSource")) {
                if (f.hasClass('dropdownchecklist')) {
                    $el.dropdownchecklist("enable");
                } else {
                    f.removeAttr("disabled");
                }
                f.attr("disSource", "");
            }
        }
    }
}

var displayFormatNames = { "None": "", "SingleLine": "Single Line", "MultiLines": "Multi Lines", "MultiLines": "Multi Lines",
    "MultiLinesEditor": "Multi Lines Editor", "NumberWithSeparator": "Number With Separator", "Date_mm_dd": "Date (05/28/2013)",
    "Date_dd_mm": "Date (28/05/2013)", "Date_mm_dd_12": "DateTime (05/28/2013 01:10:00 PM)", "Date_dd_mm_12": "DateTime (28/05/2013 01:10:00 PM)",
    "Date_mm_dd_24": "DateTime (05/28/2013 13:10:00)", "Date_dd_mm_24": "DateTime (28/05/2013 13:10:00)", "Date_Custom": "Custom",
    "DropDown": "Drop Down", "AutoCompleteStratWith": "Auto Complete Start With",
    "AutoCompleteMatchAny": "Auto Complete Match Any", "GeneralNumeric": "General", "DocumentReference": "Document Reference",
    "Hyperlink": "Link", "ButtonLink": "Button","Checklist":"Check List","SubGrid":"Sub Grid"
}
var displayFormatRalation = { "ShortText": ["SingleLine", "MultiLines", "MultiLinesEditor", "Html", "Email", "Password", "SSN", "Phone", "DocumentReference"],
    "LongText": ["MultiLines", "MultiLinesEditor", "SingleLine", "Html"],
    "Image": ["Crop", "Fit"],
    "Url": ["Hyperlink", "ButtonLink"],
    "Numeric": ["GeneralNumeric", "Currency", "NumberWithSeparator", "Percentage"],
    "Boolean": [],
    "DateTime": ["Date_mm_dd", "Date_dd_mm", "Date_mm_dd_12", "Date_dd_mm_12", "Date_mm_dd_24", "Date_dd_mm_24", "Date_Custom"], //, "TimeOnly"
    "Email": ["Email"],
    "Html": ["Html"],
    "SingleSelect": ["DropDown", "AutoCompleteStratWith", "AutoCompleteMatchAny"],
    "MultiSelect": [ "Checklist", "SubGrid"],
    "ImageList": ["Tile", "Slider"]
};

var isPlugIn = false;

function IsPlugIn() {
    try {
        return (window.parent && window.parent.parent && window.parent.parent.location.href.split('?')[0].indexOf('PlugIn') != -1) || (window.parent && window.parent.parent && window.parent.parent.parent && window.parent.parent.parent.location.href.split('?')[0].indexOf('PlugIn') != -1);
    }
    catch (err) {
        return false;
    }
}

var isItem = false;

function IsItem() {
    try {
        return isItem != null && isItem == true;
    }
    catch (err) {
        return false;
    }
}

filterDataTypes = function (elm) {
    var values = ["MultiSelect", "ImageList"]; //, "SingleSelect"];
    removeOptions(elm, values);
}

filterDataTypes2 = function (elm) {
    var values = ["ImageList", "Email"]; //, "SingleSelect"];
    removeOptions(elm, values);
}

filterDataFormats = function (elm) {
    var values = ["Password", "MultiLines", "Html", "Email", "DocumentReference", "TimeOnly"];
    removeOptions(elm, values);
}

removeOptions = function (select, values) {
    select.find('option').each(function () {
        var option = $(this);
        $(values).each(function () {
            if (option.val() == this) {
                option.remove();
            }
        });
    });
}

var displayFormatOriginalOptions = null;

filterDisplayFormat2 = function (e, data) {
    if (data.viewName == 'Field') {
        isChange = (e.type == "OnDependenciesChange");
        isAdd = (data.prefix == inlineAddingPrefix || data.prefix == createPrefix);
        var elems = $(data.dialog).find("select[name='DataType']");
        if (!isChange) {
            if (elems.length > 0)
                data.elm = elems[0];
            else
                return;
        }

        if (data.elm == null || data.elm.name != 'DataType' || $(data.elm).val() == null || $(data.elm).val() == '') {
            return;
        }

        if (views[data.guid].Role == "View Owner" && !isAdd) {
            alert(elems.data('pre'))
            Durados.Dialogs.Confirm(translator.ChangeTypeTitle, translator.ChangeTypeMessage, function () { filterDisplayFormat2(e, data); }, null);
        }
        else {
            filterDisplayFormat2(e, data);
        }
    }

}


function getPrevVal(fieldName) {
    var viewJson = PreviewDisplayModeViewJson;
    var pre = null;
    for (var index = 0, len = viewJson.Fields.length; index < len; ++index) {
        if (viewJson.Fields[index].Key == fieldName) {
            pre = viewJson.Fields[index].Value.Value;
            break;
        }
    }
    return pre;
}

var dataTypeGroups = { ShortText: 1, LongText: 1, Image: 1, Url: 1, Html: 2, Numeric: 3, Boolean: 4, DateTime: 5, SingleSelect: 6, MultiSelect: 7, ImageList: 8, Email: 1 };
function allowChangeDataType(pre, cur) {
    return dataTypeGroups[pre] == dataTypeGroups[cur];
}

var syncLink = false;

function addSyncLink(dialog) {
    syncLink = true;
    var link = $('<a href="#" class="settings-sync" title="Sync database changes">Sync Changes</a>');
    dialog.find('[name="DisplayName"]').parent('td').append(link.click(function () {
        var viewName = dialog.find('[name="Name"]').val();
        window.parent.parent.showProgress();
        setTimeout(function(){
        $.ajax({
            url: '/Admin/Sync2/' + viewName,
            contentType: 'application/html; charset=utf-8',
            async: false,
            dataType: 'json',
            cache: false,
            error: ajaxErrorsHandler,
            success: function (json) {
                window.parent.parent.hideProgress();
                if (json.success) {
                    try {
                        window.parent.parent.reloadPage();
                    }
                    catch (err) { }
                }
                else
                    modalErrorMsg(json.message);
            }

        });
        }, 1000);
    }));

}

function syncAll() {

    var ajax = function () {
        showProgress();
        setTimeout(function () {
            $.ajax({
                url: '/Admin/SyncAll/',
                contentType: 'application/html; charset=utf-8',
                async: false,
                dataType: 'json',
                cache: false,
                error: ajaxErrorsHandler,
                success: function (json) {
                    hideProgress();
                    if (json.success) {
                        try {
                            reloadPage();
                        }
                        catch (err) { }
                    }
                    else
                        modalErrorMsg(json.message);
                }

            });
        }, 1000);
    }

    Durados.Dialogs.Confirm('Sync All', 'Sync all the views will take a while. Proceed?', ajax);
}

function syncError(url, viewName) {
    showProgress();
    setTimeout(function () {
        $.ajax({
            url: '/Admin/Sync2/' + viewName,
            contentType: 'application/html; charset=utf-8',
            async: false,
            dataType: 'json',
            cache: false,
            error: ajaxErrorsHandler,
            success: function (json) {
                window.parent.parent.hideProgress();
                if (json.success) {
                    try {
                        window.location = url;
                    }
                    catch (err) { }
                }
                else
                    modalErrorMsg(json.message);
            }

        });
    }, 1000);
    

}

var confirmChangeType = false;
var confirmChangeTypePrevVal = null;

filterDisplayFormat = function (e, data) {

    if (data.viewName == 'Field') {
        isChange = (e.type == "OnDependenciesChange");
        isAdd = (data.prefix == inlineAddingPrefix || data.prefix == createPrefix);
        if (!isChange) {
            var elems = $(data.dialog).find("select[name='DataType']");
            if (elems.length > 0)
                data.elm = elems[0];
            else
                return;
        }

        if (data.elm == null || data.elm.name != 'DataType' || $(data.elm).val() == null || $(data.elm).val() == '') {
            return;
        }

        if (views[data.guid].Role == "View Owner") {
            filterDataTypes($(data.elm));
        }
        else {
            filterDataTypes2($(data.elm));
        }

        if (data.prefix == createPrefix || data.prefix == editPrefix) {
            var relatedViewNameElm = $(data.dialog).find("input[name='RelatedViewName']");
            relatedViewNameElm.attr('disabled', $(data.elm).val() != 'SingleSelect' && $(data.elm).val() != 'MultiSelect' && $(data.elm).val() != 'ImageList');
        }

        var formulaField = $(data.dialog).find("textarea[name='Formula']");
        var formula = formulaField.val();
        var hasFormula = formula != null && formula != '';
        var preVal = isAdd ? null : getPrevVal('DataType');
        var curVal = $(data.elm).val();
        var isAllowChangeDataType = isAdd || allowChangeDataType(preVal, curVal);

        confirmChangeTypePrevVal = preVal;
        confirmChangeType = (isChange && !isAdd && !hasFormula && !IsPlugIn() && views[data.guid].Role != "View Owner" && !isAllowChangeDataType);
        //        if (isChange && !isAdd && !hasFormula && !IsPlugIn() && views[data.guid].Role != "View Owner" && !isAllowChangeDataType)
        //            showMsgInModalDialog("This change will affect the database schema.<br/>The change can not be rolled back.<br/>A database backup is recommended.");
        //        if (isChange && !isAdd && !confirm("Are you sure you want to change the column type?, data mybe lost!.")) {
        //            return;
        //        }
        if (displayFormatRalation == null) {
            loadDisplayFormatRelation();
        }
        var displayFormatElms = $(data.dialog).find("select[name='DisplayFormat']");
        if (displayFormatElms.length > 0) {
            var displayFormatSelect = displayFormatElms[0];
            var oldSelectedVal = $(displayFormatSelect).val() || '';
            if (isChange || isAdd || oldSelectedVal == "None") oldSelectedVal = '';
            $(displayFormatSelect).find('option').remove().end();
            if (displayFormatRalation.hasOwnProperty($(data.elm).val())) {
                $(displayFormatSelect).append('<option value="None"></option>').val('');
                var filterSelectOption = displayFormatRalation[$(data.elm).val()];
                for (var i = 0; i < filterSelectOption.length; i++) {
                    var val = filterSelectOption[i];
                    var str = displayFormatNames[val] || val;
                    $(displayFormatSelect).append('<option value="' + val + '" >' + str + '</option>');
                }
                $(displayFormatSelect).val(oldSelectedVal);

            }
        }

        if (views[data.guid].Role == "View Owner") {
            filterDataFormats(displayFormatElms);
        }

        removeOptions(displayFormatElms, ["TimeOnly"]);



        //        if (IsPlugIn()) {
        //            $(data.elm).attr('disabled', $(data.elm).val() == 'SingleSelect');
        //        }
    }
    else if (data.viewName == 'View') {
        var workspace = $(data.dialog).find("select[name='WorkspaceID']");
        var menu = $(data.dialog).find("select[name='Menu_Parent']");
        Dependency.Load(data.viewName, 'Menu_Parent', workspace.val(), menu, data.guid, null, '/Admin/GetSelectList/');
        workspace.change(function () {

            Dependency.Load(data.viewName, 'Menu_Parent', workspace.val(), menu, data.guid, null, '/Admin/GetSelectList/');
        });
        if (!syncLink && views[data.guid].Role != "View Owner" && data.guid != pageGuid) {
            addSyncLink(data.dialog);
        }

        noMenuUrl(data);
    }
    else if (data.viewName == 'Page') {
        initPageDependency(data);
    }
    else if (data.viewName == 'FtpUpload') {
        initFtpDependency(data);
    }
    else if (data.viewName == 'Menu') {
        if (data.originElementId) {
            var menu = $('#' + data.originElementId);
            if (menu.length == 1) {
                var workspaceView = menu.parents('form:first').find("select[name='WorkspaceID']");
                if (workspaceView.length == 1) {
                    var workspaceViewId = workspaceView.val();
                    var workspaceMenu = $(data.dialog).find("select[name='WorkspaceID']");
                    workspaceMenu.attr('disabled', true);
                    if (workspaceMenu.length == 1) {
                        workspaceMenu.val(workspaceViewId);
                    }

                }
            }
        }
        isAdd = (data.prefix == inlineAddingPrefix || data.prefix == createPrefix);
        if (!isAdd) {
            data.dialog.find('select[name="WorkspaceID"]').change(function () {
                showMsgInModalDialog("All other Views and Pages that associate with this Menu will be assigned to the selected Workspace");
            });
        }
    }
    else if (data.viewName == 'durados_Link') {
        Bookmarks.loadMessagesForLinkView(e, data);
    }

}

function noMenuUrl(data) {
    var viewName = $(data.dialog).find("input[name='Name']").val();
    if (!viewName) return;

    var SystemView = $(data.dialog).find("input[name='SystemView']");

    var table = SystemView.parents('table:first').parents('table:first');
    if (!table) return;

    var label = 'IFrame URL:';
    table.append('<tr><td>' + label + '</td></tr>');

    var tr = $('<tr></tr>');
    table.append(tr);

    var td = $('<td colspan="2"></td>');
    tr.append(td);

    var textarea = $('<textarea class="wtextareashort" disabled="disabled"></textarea>');
    td.append(textarea);

    var controller = $(data.dialog).find("input[name='Controller']").val();
    if (!controller)
        controller = "Home";
    var action = $(data.dialog).find("input[name='IndexAction']").val();
    if (!action)
        action = "Index";
    
    var url = window.location.protocol + '//' + window.location.host + '/' + controller + '/' + action + '/' + viewName + '?menu=off';
    textarea.val(url);
}

$(Durados.View).bind('edit', function (e, data) { filterDisplayFormat(e, data); });
$(Durados.View).bind('add', function (e, data) { filterDisplayFormat(e, data); });
$(Durados.Dependencies).bind('OnDependenciesChange', function (e, data) { filterDisplayFormat(e, data); });

function initPageTab2(data, valSelected, tab) {
    var selectedClass = 'ui-tabs-selected';
    var activeClass = 'ui-state-active';
    var hideClass = 'ui-tabs-hide';

    var lis = $(tab.children('ul')[0]).children('li');
    lis.each(function () {
        $(this).removeClass(selectedClass);
        $(this).removeClass(activeClass);
    });

    tab.tabs("option", "disabled", [0, 1, 2, 3]);
    if (valSelected == 'Content') {
        tab.tabs("enable", 0);
        Durados.Tabs.select(tab, 0);
        //        tab.tabs("select", 0);

    }
    else if (valSelected == 'IFrame') {
        tab.tabs("enable", 1);
        Durados.Tabs.select(tab, 1);
        //        tab.tabs("select", 1);
    }
    else if (valSelected == 'External') {
        tab.tabs("enable", 2);
        Durados.Tabs.select(tab, 2);
        //        tab.tabs("select", 2);
    }
    else if (valSelected == 'ReportingServices') {
        tab.tabs("enable", 3);
        Durados.Tabs.select(tab, 3);
        //        tab.tabs("select", 3);
    }

}

function initFtpTab(data, valSelected, tab) {
    initTabDependencyChange(data, valSelected, tab, [0, 1, 2]);
}

function initTabDependencyChange(data, valSelected, tab, arr) {
    var selectedClass = 'ui-tabs-selected';
    var activeClass = 'ui-state-active';
    var hideClass = 'ui-tabs-hide';

    var lis = $(tab.children('ul')[0]).children('li');
    lis.each(function () {
        $(this).removeClass(selectedClass);
        $(this).removeClass(activeClass);
    });

    tab.tabs("option", "disabled", arr);

    $(arr).each(function () {
        if (valSelected == this) {
            tab.tabs("enable", parseInt(this));
            tab.tabs("select", parseInt(this));

        }
    });

}

function initPageTab(data, pageTypeSelect) {
    var selectedClass = 'ui-tabs-selected';
    var activeClass = 'ui-state-active';
    var hideClass = 'ui-tabs-hide';

    var valSelected = pageTypeSelect.val();
    var tab = data.dialog.find('div.ui-tabs');
    //tab.tabs("option", "collapsible", true);

    var panels = tab.children('div.ui-tabs-panel');
    panels.each(function () {
        $(this).addClass(hideClass);
    });

    var lis = $(tab.children('ul')[0]).children('li');
    lis.each(function () {
        $(this).removeClass(selectedClass);
        $(this).removeClass(activeClass);
    });

    if (valSelected == 'Content') {
        tab.tabs("option", "active", 0);
        var li = $(lis[0]);
        li.addClass(selectedClass);
        li.addClass(activeClass);
        $(panels[0]).removeClass(hideClass);
        tab.tabs("option", "disabled", [1, 2, 3]);
    }
    else if (valSelected == 'IFrame') {
        tab.tabs("option", "active", 1);
        var li = $(lis[1]);
        li.addClass(selectedClass);
        li.addClass(activeClass);
        $(panels[1]).removeClass(hideClass);
        //li.removeClass()
        //tab.data("tabs").select(1);
        tab.tabs("option", "disabled", [0, 2, 3]);
    }
    else if (valSelected == 'External') {
        tab.tabs("option", "active", 2);
        var li = $(lis[2]);
        li.addClass(selectedClass);
        li.addClass(activeClass);
        $(panels[2]).removeClass(hideClass);
        tab.tabs("option", "disabled", [0, 1, 3]);
    }
    else if (valSelected == 'ReportingServices') {
        tab.tabs("option", "active", 3);
        var li = $(lis[3]);
        li.addClass(selectedClass);
        li.addClass(activeClass);
        $(panels[3]).removeClass(hideClass);
        tab.tabs("option", "disabled", [0, 1, 2]);
    }

}

function initFtpDependency(data) {
    var tab = data.dialog.find('div.ui-tabs');
    var typeSelect = data.dialog.find('select[name="StorageType"]');
    var valSelected = typeSelect.val() == "Ftp" ? 0 : typeSelect.val() == "Azure" ? 1 : 2;
    changeUploadLabel(data.dialog, typeSelect.val());
    initFtpTab(data, valSelected, tab);
    typeSelect.change(function () {
        valSelected = typeSelect.val() == "Ftp" ? 0 : typeSelect.val() == "Azure" ? 1 : 2;
        changeUploadLabel(data.dialog, typeSelect.val());
        initFtpTab(data, valSelected, tab);
    });
}

function changeUploadLabel(dialog, valSelected) {
    var text = "Folder / Container / Bucket:";
    
    switch (valSelected) {
        case 'Ftp':
            text = "Folder:";
            break;
        case 'Azure':
            text = "Container:";
            break;
        case 'Aws':
            text = "Bucket:";
            break;
        default:
            text = "Folder / Container / Bucket:";
            break;
    }
    dialog.find('input[name="DirectoryBasePath"]').parent('td:first').prev().text(text);

}

function initPageDependency(data) {
    var workspace = $(data.dialog).find("select[name='WorkspaceID']");
    var menu = $(data.dialog).find("select[name='PageMenu_Parent']");
    Dependency.Load(data.viewName, 'PageMenu_Parent', workspace.val(), menu, data.guid, null, '/Admin/GetSelectList/');
    workspace.change(function () {

        Dependency.Load(data.viewName, 'PageMenu_Parent', workspace.val(), menu, data.guid, null, '/Admin/GetSelectList/');
    });

    var tab = data.dialog.find('div.ui-tabs');
    var pageTypeSelect = data.dialog.find('select[name="PageType"]');
    var i_guid = getMainPageGuid();

    if (!((i_guid) && (views[i_guid].Role == 'Developer')))
        pageTypeSelect.attr('disabled', 'disabled');
    //pageTypeSelect.parents('tr:first').prev().hide().prev().prev().hide();
    //pageTypeSelect.parents('tr:first').prev().prev().children('td').next().next().hide().next().hide();

    var valSelected = pageTypeSelect.val();
    initPageTab2(data, valSelected, tab);
    pageTypeSelect.change(function () {
        valSelected = pageTypeSelect.val();
        initPageTab2(data, valSelected, tab);
    });

    //    var pageTypeSelect = data.dialog.find('select[name="PageType"]');
    //    //setTimeout(function () {
    //    var tab = data.dialog.find('div.ui-tabs');

    ////    tab.tabs({
    ////        activate: function (event, ui) { initPageTab(data, pageTypeSelect); }
    ////    });
    ////    tab.on("tabsactivate", function (event, ui) { 
    //        initPageTab(data, pageTypeSelect);
    ////    });
    //    //}, 1000);
    //    pageTypeSelect.change(function () {
    //        initPageTab(data, pageTypeSelect);
    //    });
}
function initChartDependency(dialog,chartId) {

    var e = { data: { dialog: dialog,chartId:chartId} };
    Charts.changeChartType(e);
    dialog.find('select[name=ChartType]').unbind('change').bind('change', { dialog: dialog, chartId: chartId }, Charts.changeChartType);
}

Charts.changeChartType = function (event) {
    var data = event.data;
    var chartId = data.chartId;
    var chartTypeElm = data.dialog.find('select[name=ChartType]');
    
    switch (chartTypeElm.val()) {
        case "Gauge":
            data.dialog.find('.ChartParameters').parent().parent().hide();
            data.dialog.find('span.SeperatorTitle:contains("Chart Parameters")').closest("tr").hide()

            data.dialog.find('.GaugeParameters').parent().parent().show();
            data.dialog.find('span.SeperatorTitle:contains("Gauge Parameters")').closest("tr").show()
            
            break;

        default:
            
            clearInterval(Charts['refreshInterval' + chartId]);
            data.dialog.find('.ChartParameters').parent().parent().show();
            data.dialog.find('span.SeperatorTitle:contains("Chart Parameters")').closest("tr").show()

            data.dialog.find('.GaugeParameters').parent().parent().hide();
            data.dialog.find('span.SeperatorTitle:contains("Gauge Parameters")').closest("tr").hide()
         

    }

}

Charts.startDashboard = function (id) {
    Charts.run();

    $("select#chartColumns").change(function () {
        showProgress();
        var queryString = location.search.substring(1, location.search.length);
        window.location.href = '/Admin/EditDashboard?id=' + id + '&columns=' + $(this).val() + "&" + queryString;
        // $.post(url, { id: id, columns:  }, function (html) {

        hideProgress();
    });
}   
    
/************************************************************************************/
//      From here till end- changed by br
/************************************************************************************/

/************************************************************************************/
/*		FilterForm.InitTreeFilter (by br)			
/*		Init tree filter: assign to click event of all filter items, assign to close buttons click of current filter selected values		
/************************************************************************************/
FilterForm.InitTreeFilter = function () {
    //Assign to click event of all filter items
    $('.tree_filter li').live('click', function (event) {
        var parentContainer = $(event.target).parents('.tree-filter-values');
        var filterFieldId = parentContainer.attr('data-rel');

        if (filterFieldId != null) {
            var guid = parentContainer.attr('data-guid');
            var value = $(event.target).attr('data-val');

            $('#' + filterFieldId).attr('data-val', value);
            parentContainer.hide();

            FilterForm.Apply(false, guid, null);
        }
    });

    //Assign to close buttons click of current filter selected values
    $('.tree_filter_selectedValues .close').live('click', function () {
        var filterFieldId = $(this).prev().attr('id');
        var guid = $(this).attr('data-guid');

        $(this).prev().attr('data-val', '');
        FilterForm.Apply(false, guid, null);
    });
};

/************************************************************************************/
/*		FilterForm.LessFilterValues	(by br)			
/*		Display less filter values for a specific tree filter field		
/************************************************************************************/
FilterForm.LessFilterValues = function (fieldGuid) {
    var moreItemsId = '#' + fieldGuid + '_more_items';
    var moreButtonId = '#' + fieldGuid + '_more';
    var lessButtonId = '#' + fieldGuid + '_less';

    $(moreItemsId).hide();
    $(moreButtonId).show();
    $(lessButtonId).hide();
}

/************************************************************************************/
/*		FilterForm.MoreFilterValues	(by br)			
/*		Display more filter values for a specific tree filter field				
/************************************************************************************/
FilterForm.MoreFilterValues = function (viewName, fieldName, guid, fieldGuid) {
    var moreItemsId = '#' + fieldGuid + '_more_items';
    var moreButtonId = '#' + fieldGuid + '_more';
    var lessButtonId = '#' + fieldGuid + '_less';

    if ($(moreItemsId).length) {
        $(moreItemsId).show();
        $(moreButtonId).hide();
        $(lessButtonId).show();
    }
    else {
        showProgress();
        $.post(views[guid].allFilterValuesUrl,
        {
            viewName: viewName, fieldName: fieldName, guid: guid
        },
         function (html) {
             $(moreButtonId).parents('.tree-filter-values').html(html);
             hideProgress();
         });

    }
}

Durados.BeforeUnload = function (guid, duringChangeDisplayType) {
    Durados.DisplayType.UnloadByDisplayType(guid, duringChangeDisplayType);
}

Durados.DisplayType =
{
    /************************************************************************************/
    /*		changeDisplayType (by br)						
    /*		change data display type for current view: Table / Dashboard / Preview				
    /************************************************************************************/
    changeDisplayType: function (guid, displayType) {
        if (views[guid] != null) {
            if (views[guid].DataDisplayType == "Preview") {
                var dialog = $('#' + guid + 'DataRowEdit');

                dialog.dialog('close');
            }
            Durados.BeforeUnload(guid, true);
            views[guid].DataDisplayType = displayType;
        }
        refreshView(guid, "dataDisplayType=" + displayType, { needChangeDisplayType: true });
    },
    InitByDisplayType: function (guid) {
        var displayType = views[guid].DataDisplayType;

        switch (displayType) {
            case "Preview":
                var pks1 = Multi.GetSelection(guid);
//                if (pks1 && pks1.length > 1) {
//                }
//                else {
                    PreviewDisplay.Run(guid);

//                }
                
                break;
            default:
                break;
        }
    },
    UnloadByDisplayType: function (guid, duringChangeDisplayType) {
        var displayType = views[guid].DataDisplayType;

        switch (displayType) {
            case "Preview":
//                    var pks1 = Multi.GetSelection(guid);
//                    if (pks1 && pks1.length > 1) {
//                    }
//                    else {
                        PreviewDisplay.Unload(guid, duringChangeDisplayType);
//                
//                    }
                break;
            default:
                break;
        }
    }
}

/************************************************************************************/
/*		PreviewDisplay (by br)						
/*		Provide functionalitty for layout with preview display						
/************************************************************************************/
var PreviewDisplay = {
    scrollX: 0,
    scrollY: 0,
    selectedPk: null,
    //    loadElementStateAfterRefresh: false,
    prevDialogSettings: [],
    views: [],
    isFirstTime: function (guid) {
        var isFirstTime = false;
        if ($.inArray(guid, views) == -1) {
            isFirstTime = true;
            views.push(guid);
        }

        return isFirstTime;
    },
    selectFirstRow: function (containerId, guid) {
        //Clear EditDialog pk
        //        EditDialog.pk = null;

        //Try select first row- if exist
        var firstRow = $("#" + containerId + " .portlet:first");
        if (firstRow.length) {
            //            Multi.BoardClicked(event, firstRow.children('.boardtitle'),guid,true);
            if ($.browser.mozilla) {
                setTimeout(function () {
                    firstRow.click();
                }, 1000);
            }
            else {
                firstRow.click();
            }
        }
        //If there is no first row- close edit dialog
        else {
            var dialog = $('#' + guid + 'DataRowEdit');
            if (dialog.dialog('isOpen')) {
                dialog.dialog("close");
            }
        }
    },
    Run: function (guid) {
        var isFirstTime = PreviewDisplay.isFirstTime(guid);


        if (isFirstTime) {
            PreviewDisplay.savePrevDialogSettings(guid);
        }

        var containerId = guid + "previewContainer";

        if (isFirstTime) {
            if ($.browser.mozilla) {
                $('#' + containerId).parents('.fixedViewPort:first').css('display', 'inline-block');
            }
        }
        //Run dashboard style
        Dashboard.run("Preview", containerId, guid);

        //        if (!isDockFields(guid) && !PreviewDisplay.loadElementStateAfterRefresh) {
        if (isFirstTime) {
            PreviewDisplay.selectFirstRow(containerId, guid);
        }
        else {
            PreviewDisplay.loadElementState(guid);
        }
    },
    Unload: function (guid, duringChangeDisplayType) {
        PreviewDisplay.trySaveChanges(guid);

        if (duringChangeDisplayType) {
            PreviewDisplay.loadPrevDialogSettings(guid);
            views.pop(guid);
        }
        else {
            PreviewDisplay.saveElementState(guid);
        }
        PreviewDisplay.selectedPk = null;
    },
    savePrevDialogSettings: function (guid) {
        var dialog = $('#' + guid + 'DataRowEdit');

        var viewName = views[guid].gViewName;
        var dialodDisplay = Rectangle.Load(viewName);

        var height;
        var width;
        var position;

        if (dialodDisplay != null) {
            height = dialodDisplay.height;
            width = dialodDisplay.width;
            position = [dialodDisplay.left, dialodDisplay.top];
        }
        else {
            height = dialog.dialog("option", "height");
            width = dialog.dialog("option", "width");
            position = dialog.dialog("option", "position");
        }

        PreviewDisplay.prevDialogSettings[guid] = {
            height: height,
            width: width,
            modal: dialog.dialog("option", "modal"),
            closeOnEscape: dialog.dialog("option", "closeOnEscape"),
            resizable: dialog.dialog("option", "resizable"),
            position: position,
            zIndex: Durados.Dialogs.zIndex(dialog)
        };

        dialog.attr('changed', 'no');
    },
    loadPrevDialogSettings: function (guid) {
        var dialog = $('#' + guid + 'DataRowEdit');
        var prevDialogSettings = PreviewDisplay.prevDialogSettings[guid];

        if (prevDialogSettings != null) {
            dialog.dialog("close");
            dialog.dialog("option", "height", prevDialogSettings.height);
            dialog.dialog("option", "width", prevDialogSettings.width);
            dialog.dialog("option", "modal", prevDialogSettings.modal);
            dialog.dialog("option", "closeOnEscape", prevDialogSettings.closeOnEscape);
            dialog.dialog("option", "resizable", prevDialogSettings.resizable);
            dialog.dialog("option", "position", [prevDialogSettings.position.left, prevDialogSettings.position.top]);
            Durados.Dialogs.zIndex(dialog, prevDialogSettings.zIndex);

        }
    },
    /************************************************************************************/
    /*		initDialogSettings (by br)						
    /*		Init some settings for edit dialog: height, width, position, modal, resizable, buttons, title visibility				
    /************************************************************************************/
    initDialogSettings: function (guid, pk) {
        var adjustedElementId = guid + "_editContainer";
        var adjustedElement = $('#' + adjustedElementId);

        if (adjustedElement.length) {
            var dialog = $('#' + guid + 'DataRowEdit');

            var titlebar = dialog.siblings(".ui-dialog-titlebar");
            var maxButtom = titlebar.find("a.ui-dialog-titlebar-max");
            var state = maxButtom.attr('d_state');

            if (state == 'max' || IE7) {
                dialogExt.resize(dialog, maxButtom, guid);
            }
            titlebar.hide();

            var height = adjustedElement.height() - 4;
            var width = adjustedElement.width() - 5;
            var top = adjustedElement.offset().top;
            var left = adjustedElement.offset().left;
            var allowEdit = EditDialog.AllowEdit(guid, pk);

            dialog.dialog("option", "height", height);
            dialog.dialog("option", "width", width);
            dialog.dialog("option", "modal", false);
            dialog.dialog("option", "closeOnEscape", false);
            dialog.dialog("option", "resizable", false);
            dialog.dialog("option", "position", [left, top]);
            Durados.Dialogs.zIndex(dialog, 0);

            var buttons = EditDialog.GetButtons(pk, guid, true, !allowEdit.disabled);
            dialog.dialog("option", "buttons", buttons);

            var s = 83; // key code
            shortcutKey(dialog, buttons[translator.save], s, true);


            dialog.css('padding-top', '0');

        }
    },
    /************************************************************************************/
    /*		trySaveChanges (by br)						
    /*		try save changes of edit dialog				
    /************************************************************************************/
    trySaveChanges: function (guid) {

        //if (isDockFields()) { return; };
        var dialog = $('#' + guid + 'DataRowEdit');
        var changed = dialog.attr('changed') == 'yes';

        //        if (changed && EditDialog.pk != null) {
        if (changed && PreviewDisplay.selectedPk != null) {
            if (confirm("You made changes. Do you want to save changes?")) {
                EditDialog.Update(PreviewDisplay.selectedPk, guid, false, false, false, true);
            }
        }
    },
    /************************************************************************************/
    /*		adjustSize (by br)						
    /*		Adjust size of: previewContainer, preview editContainer, edit dialog into preview editContainer
    /************************************************************************************/
    adjustSize: function (guid, containerHeight) {
        var previewContainer = $('#' + guid + "previewContainer");
        if (previewContainer.length) {
            //br ask
            var previewContainerHeight = containerHeight - previewContainer.offset().top - 42;
            previewContainer.height(previewContainerHeight);
        }

        var editContainer = $('#' + guid + "_editContainer");
        if (editContainer.length) {
            //br todo: ask
            var editContainerHeight = containerHeight - editContainer.offset().top - 44;

            editContainer.height(editContainerHeight);

            var dialog = $('#' + guid + 'DataRowEdit');
            var editContainerWidth = editContainer.width();
            var editContainerTop = editContainer.offset().top;
            var editContainerLeft = editContainer.offset().left;

            //br todo: ask
            dialog.dialog("option", "height", editContainerHeight - 3);
            dialog.dialog("option", "width", editContainerWidth - 5);
            dialog.dialog("option", "position", [editContainerLeft, editContainerTop]);
        }
    },
    /************************************************************************************/
    /*		saveElementState (by br) (temporary deleted)				
    /*		Save preview container state: scrollX, scrollY, selectedPk
    /************************************************************************************/
    saveElementState: function (guid) {
        var previewElement;
        if (guid) {
            previewElement = $('#' + guid + 'previewContainer').first();
        }
        if (!previewElement.length) return;

        PreviewDisplay.scrollY = previewElement.scrollTop();
        PreviewDisplay.scrollX = previewElement.scrollLeft();

        var selectedElement = previewElement.find('.boardselected');
        PreviewDisplay.selectedPk = selectedElement.attr('d_pk');
    },
    /************************************************************************************/
    /*		loadElementState (by br)					
    /*		Load preview container state: scrollX, scrollY, selectedPk
    /************************************************************************************/
    loadElementState: function (guid, isDuringSortOrFilter) {
        if (guid == null) {
            return;
        }

        var previewElement;
        var isPkExist = false;

        previewElement = $('#' + guid + 'previewContainer').first();
        if (!previewElement.length) return;

        if (PreviewDisplay.selectedPk != null) {
            var pkElement = previewElement.find('[d_pk=' + PreviewDisplay.selectedPk + ']');

            if (pkElement.length) {
                //Select previous element
                isPkExist = true;
                pkElement.addClass('boardselected');

                //Check if need refresh editDialog
                var editDialogPk = $("#" + guid + "DataRowEdit").attr("pk");
                if (PreviewDisplay.selectedPk != editDialogPk) {
                    pkElement.parent().click();
                }

                //Scroll to previous position
                if (PreviewDisplay.scrollY)
                    previewElement.scrollTop(PreviewDisplay.scrollY);

                if (PreviewDisplay.scrollX)
                    previewElement.scrollLeft(PreviewDisplay.scrollX);

                pkElement.scrollintoview()
            }
        }

        if (!isPkExist) {
            PreviewDisplay.selectFirstRow(guid + "previewContainer", guid);
        }

        //        PreviewDisplay.scrollY = 0;
        //        PreviewDisplay.scrollX = 0;
        //        PreviewDisplay.selectedPk = null;
    }
};

/************************************************************************************/
/*		Durados.Common	(by br)					
/*		Provide functionalitty for common actions					
/************************************************************************************/
Durados.Common =
{
    /************************************************************************************/
    /*		increaseCssValue (by br)		
    /*		increase a css value for a specific element					
    /************************************************************************************/
    increaseCssValue: function (selector, css, increaseValue) {
        var object = $(selector);
        if (!object.length == 1)
            return;
        var currentValue = parseInt(object.css(css).replace("px", ""));
        var newValue = currentValue + increaseValue;

        object.css(css, newValue)
    }
};

/************************************************************************************/
/*		Durados.Dialogs (by br)						
/*		Provide functionalitty for add end edit dialogs						
/************************************************************************************/
Durados.Dialogs =
{
    Wait: function (element, title, width, height, autoOpen) {

        var dialog = $('<div></div>').appendTo('body')
          .append(element)
          .dialog({
              modal: true, title: title, zIndex: 10000, autoOpen: autoOpen,
              width: width ? width : 'auto', height: height ? height : 'auto', position: ['center', 'center'], resizable: false, open: function () {
                  if (!title)
                      $(this).dialog("widget").find(".ui-dialog-titlebar").hide();

                  $(".ui-widget-overlay").css({
                      opacity: 0.6,
                      filter: "Alpha(Opacity=60)"
                  });
              }

          });
        return dialog;
    },

    Confirm: function (title, message, yesCallback, noCallback, openCallback, yesButtonText, noButtonText, position) {
        if (!title)
            title = "Confirmation";

        if (!message)
            message = "Are you sure?";

        if (!position) position = 'center';
        if (!yesButtonText) yesButtonText = 'Yes';
        if (!noButtonText) noButtonText = 'No';

        var buttons = {};
        buttons[yesButtonText] = function () {
            if (yesCallback) yesCallback(div);
            $(this).dialog("close");
        };

        buttons[noButtonText] = function () {
            if (noCallback) noCallback(div);
            $(this).dialog("close");
        };

        var div = $('<div></div>');
        div.appendTo('body')
          .html('<div class="confirm-message">' + message + '</div>')
          .dialog({
              modal: true, title: title, zIndex: 100000002, autoOpen: true,
              width: 'auto', resizable: false,
              position: position,
              //              buttons: {
              //                  yesButtonText: function () {
              //                      if (yesCallback) yesCallback(div);
              //                      $(this).dialog("close");
              //                  },
              //                  noButtonText: function () {
              //                      if (noCallback) noCallback(div);
              //                      $(this).dialog("close");
              //                  }
              //              },
              buttons: buttons,
              open: function (event, ui) {
                  $(this).parent().find('div.ui-dialog-buttonpane').attr("style", "display:block !important");
                  if (openCallback) openCallback(div);
              },
              close: function (event, ui) {
                  $(this).remove();
              }
          });

        return div;
    },

    Alert: function (title, message, closeCallback) {
        if (!title)
            title = "";

        if (!message)
            message = "";

        $('<div></div>').appendTo('body')
          .html('<div class="confirm-message">' + message + '</div>')
          .dialog({
              modal: true, title: title, zIndex: 2147480000, autoOpen: true,
              width: 'auto', resizable: false,
              buttons: {
                  Ok: function () {
                      $(this).dialog("close");
                      if (closeCallback) closeCallback();
                  }
              },
              close: function (event, ui) {
                  $(this).remove();
              }
          });
    },
    /************************************************************************************/
    /*		general (by br)				
    /*		Display general dialog
    /************************************************************************************/
    general: function (title, html, buttons, closeCallback, dialogClass) {
        var dialog = $('<div></div>')
        .attr('style', 'min-height: 0px !important')
        .appendTo('body')
          .html(html)
          .dialog({
              modal: true, title: title, zIndex: 1000000, autoOpen: true,
              width: 'auto', resizable: false,
              buttons: buttons,
              close: function (event, ui) {
                  if (closeCallback) {
                      closeCallback();
                  }
                  $(this).remove();
              }
          });

        if (dialogClass != null) {
            dialog.addClass(dialogClass);
        }

        return dialog;
    },
    /************************************************************************************/
    /*		save (by br)				
    /*		Display save dialog
    /************************************************************************************/
    save: function (title, html, saveCallback) {
        if (!title)
            title = "Save";
        if (!html)
            html = $('<div/>');

        var buttons = {};
        buttons[translator.saveAndClose] = function () {
            if (saveCallback) saveCallback();
            $(this).dialog("close");
        }
        buttons[translator.close] = function () { $(this).dialog("close"); }

        return Durados.Dialogs.general(title, html, buttons);
    },
    /************************************************************************************/
    /*		BindToChangeEvent (by br)				
    /*		Get dialog inputs by selector, and bind its to change event		
    /************************************************************************************/
    BindToChangeEvent: function (selector, dialog) {
        dialog.find(selector).unbind('change', Durados.Dialogs.DialogChanged).bind('change', { dialog: dialog }, Durados.Dialogs.DialogChanged);
    },
    /************************************************************************************/
    /*		DialogChanged (by br)				
    /*		Event occures when dialog content is changed		
    /************************************************************************************/
    DialogChanged: function (event) {
        var dialog = event.data.dialog;
        if (dialog != null) {
            dialog.attr('changed', 'yes');
        }
    },
    /************************************************************************************/
    /*		zIndex (by br)				
    /*		Get or set dialog zIndex		
    /************************************************************************************/
    zIndex: function (dialog, value) {
        if (value == null) {
            return dialog.parent().css("zIndex");
        }
        else {
            dialog.parent().css("zIndex", value);
        }
    }
}

/************************************************************************************/
/*		Durados.SplitLayout (by br)						
/*		Provide functionalitty for layout with split seperator						
/************************************************************************************/
Durados.SplitLayout =
{
    /************************************************************************************/
    /*		adjustSplitContentHeight (by br)					
    /*		adjust split content height to container height
    /************************************************************************************/
    adjustSplitContentHeight: function (splitContentSelector, containerHeight) {
        var $tree = $(splitContentSelector);
        if ($tree.length) {
            var treeHeight = containerHeight - $tree.offset().top;

            $tree.height(treeHeight);

            if (splitContentSelector == '#AppFilterTreeDiv') {
                if ($tree.children().length > 1) {
                    $($tree.children()[1]).height(treeHeight);
                }
            }
            else {
                $tree.children(":first").height(treeHeight);
            }
        }
    },
    /************************************************************************************/
    /*		initSplitter (by br)					
    /*		init splitter- on click show or hide split content					
    /************************************************************************************/
    initSplitter: function (splitterId, splitContentId, onShow, onHide, alternativeClickHandler) {
        var splitLayout = this;

        $(splitterId).click(function () {
            if (alternativeClickHandler) {
                if (!alternativeClickHandler(this)) {
                    return;
                }
            }
            if ($(splitContentId).width() > 0) {
                splitLayout.hideSplitContent(splitContentId, splitterId, onHide);
            } else {
                splitLayout.showSplitContent(splitContentId, splitterId, onShow);
            }
        });
    },
    /************************************************************************************/
    /*		hideSplitContent (by br)					
    /*		hide split content and adjust elements sizes					
    /************************************************************************************/
    hideSplitContent: function (splitContentId, splitterId, onSuccess) {
        var splitterContent = $(splitContentId);

        if (splitterContent.length) {
            var splitterContentInnerDiv = splitterContent.children(":first");
            var mainAppDivId = '#mainAppDiv';

            $(splitterContentInnerDiv).hide();
            $(splitterContent).width(0);

            if ($('body').attr('dir') != 'rtl') {
                Durados.Common.increaseCssValue(mainAppDivId, 'marginLeft', -200);
                Durados.Common.increaseCssValue(splitterId, 'right', -4);
            } else {
                Durados.Common.increaseCssValue(mainAppDivId, 'marginRight', -200);
                Durados.Common.increaseCssValue(splitterId, 'left', -4);
            }
            Durados.GridHandler.setHeadersDivWidth();
            if (onSuccess) {
                onSuccess();
            }
        }
    },
    /************************************************************************************/
    /*		showSplitContent (by br)					
    /*		show split content and adjust elements sizes					
    /************************************************************************************/
    showSplitContent: function (splitContentId, splitterId, onSuccess) {
        var splitterContent = $(splitContentId);

        if (splitterContent.length) {
            var splitterContentInnerDiv = splitterContent.children(":first");
            var mainAppDivId = '#mainAppDiv';

            $(splitterContent).width(200);
            $(splitterContentInnerDiv).show();

            if ($('body').attr('dir') != 'rtl') {
                Durados.Common.increaseCssValue(mainAppDivId, 'marginLeft', 200);
                Durados.Common.increaseCssValue(splitterId, 'right', 4);
            } else {
                Durados.Common.increaseCssValue(mainAppDivId, 'marginRight', 200);
                Durados.Common.increaseCssValue(splitterId, 'left', 4);
            }
            Durados.GridHandler.setHeadersDivWidth();
            if (onSuccess) {
                onSuccess();
            }
        }
    }
}

/************************************************************************************/
/*		initSplitters (by br)					
/*		init splitters with click handlers					
/************************************************************************************/
function initSplitters() {

    //Init TreeSplitterBar
    Durados.SplitLayout.initSplitter("#TreeSplitterBar", "#AppTreeDiv",
    function () {
        var mainAppDiv = $('#mainAppDiv');
        $("#AppFilterTreeDiv").addClass("filter_tree_with_margin");
        if ($('body').attr('dir') != 'rtl') {
            $("#AppFilterTreeDiv").css("marginLeft", "");
        }
        else {
        }

    },
    function () {
        $("#AppFilterTreeDiv").removeClass("filter_tree_with_margin");
        $("#AppFilterTreeDiv").css("marginLeft", 4);
    });

    //Init FilterTreeSplitterBar
    Durados.SplitLayout.initSplitter("#FilterTreeSplitterBar", "#AppFilterTreeDiv", null, null,
    function (splitter) {
        var guid = $(splitter).attr('data-guid');
        $("#" + guid + "ajaxDiv [name=rowfilter]").first().hide();
        var filterTogleButton = $('#' + guid + 'filterButton');
        //        var filterTogleButton = $('#' + guid + 'ajaxDiv .showHideFilterSpan img:visible');

        if (filterTogleButton.length) {
            filterTogleButton.click();
            return false;
        }
        else {
            return true;
        }
    });
}

/************************************************************************************/
/*		Durados.Tabs (by br)					
/*		Handle tabs functionaliity (In order to support jquery versions)
/************************************************************************************/
Durados.Tabs = {
    /************************************************************************************/
    /*		select	(by br)			
    /*		Select tab by index
    /************************************************************************************/
    select: function (tabElement, index) {
        tabElement.tabs("select", index);
        //        tabElement.tabs("option", "active", index);
    },
    /************************************************************************************/
    /*		url	(by br)			
    /*		Set url for a tab
    /************************************************************************************/
    url: function (tabElement, url, index) {
        tabElement.find("a").attr('href', url);
        //        tabElement.parents("div:first").tabs("url", index, url);
    }
}

function setPlan(elm) {
    $(Durados.View).trigger('setPlan', { elm: elm });

}
slider.UpdateSelectedImage = function (el) {
    var $el = $(el);
    var $hidden = $el.parents("div.slider-container").first().find("input[type=hidden]");
    $el.parents("ul").find(".ui-slider-item").removeClass("slider-active");
    $el.addClass("slider-active");
    $hidden.val($el.attr("value")).change();
}
slider.SetLoadSelectedImage = function ($htmlField) {
    $htmlField.parents(".slider-container").each(function () {
        var sliderContainer = $(this);

        //Set active item
        var sliderItem = sliderContainer.find("div.ui-slider-item[value=" + $htmlField.val() + "]");
        sliderItem.addClass("slider-active");

        //Navigate to active item
        var playRtl = $('body').attr('dir') == 'rtl';
        var index = sliderContainer.find(".panel:not(.cloned)").index(sliderItem.parent()) + 1;

        if (playRtl) {
            var sliderPanelsLength = sliderContainer.find(".panel:not(.cloned)").length;

            index -= 2;
            if (index <= 0) {
                index = sliderPanelsLength + index;
            }
        }
        sliderContainer.find('.ui-slider').anythingSlider(index);
    });
}

/************************************************************************************/
/*		Durados.CheckBox (by miri)						
/*		Provide functionalitty for set and get checkbox value					
/************************************************************************************/
Durados.CheckBox =
{
    IsChecked: function (checkbox) {
        return checkbox.attr('checked') == 'checked' || checkbox.attr('checked') || checkbox.prop('checked');
    },
    SetChecked: function (checkbox, value) {
        if (value == '' || value == null)
            value = false;
        checkbox.attr('checked', value);
    }
}

/************************************************************************************/
/*		Durados.Image (by br)						
/*		Provide functionalitty for image behavior
/************************************************************************************/
Durados.Image = {
    Init: function () {
    },
    /************************************************************************************/
    /*		ShowDialog
    /*		Display current image within a dialog
    /************************************************************************************/
    ShowDialog: function (img) {
        $('<div></div>').appendTo('body')
          .html('<div class="confirm-message"><a target="_blank" href="' + $(img).attr('src') + '"><img style="margin: 0px auto; display: block;" src="' + $(img).attr('src') + '"></img></a></div>')
          .dialog({
              modal: true, title: 'View Image', zIndex: 10000, autoOpen: true,
              width: 'auto', height: 'auto', resizable: false,
              buttons: {
                  Close: function () {
                      $(this).dialog("close");
                  }
              },
              close: function (event, ui) {
                  $(this).remove();
              }
          });
    },
    /************************************************************************************/
    /*		SetSize
    /*		Set image size by cell container size
    /************************************************************************************/
    SetSize: function (img) {
        var src = $(img).attr('src');
        if (src == null || src == '') {
            return;
        }

        var td = $(img).parents('td:first');

        //Fix bud in dashboard view 
        if (!td.hasClass('d_fieldContainer')) {
            return;
        }

        var div = $(img).parents('div:first');
        //td.css('padding', '0px');

        var height = td.height();
        var width = td.width();
        $(img).css('width', 'auto');
        $(img).css('height', 'auto');
        var imgHeight = $(img).height();
        var imgWidth = $(img).width();
        var format = $(img).attr('format');
        div.height(height);
        div.width(width);

        if (format == 'Fit') {
            var imgRatio = imgHeight / imgWidth;
            var cellRatio = height / width;

            if (imgRatio > cellRatio) {
                $(img).height(height);
                $(img).width(height / imgRatio);
            }
            else {
                $(img).width(width);
                $(img).height(width * imgRatio);
            }

            if ($(img).height() < height) {

                var marginTop = (height / 2 - $(img).height() / 2);
                $(img).css('margin-top', marginTop + 'px');
            }
            else {
                $(img).css('margin-top', '0px');
            }
        }
        else {
            $(img).parent().parent().css('width', width + 'px');

            var marginTop = (height / 2 - imgHeight / 2);
            $(img).css('margin-top', marginTop + 'px');
            if (imgWidth > width) {
                var marginLeft = (width / 2 - imgWidth / 2);
                $(img).css('margin-left', marginLeft + 'px');
            }
            else {
                $(img).css('margin-left', '0px');
            }
        }
        div.css('overflow', 'hidden');
        $(img).parent().parent().show();
        $(img).css('display', 'inline-block');
    },
    Init: function () {
        $('img[dialog]').click(function () {
            Durados.Image.ShowDialog($(this));
            return false;
        });
    }
}

/************************************************************************************/
/*		Durados.Preview (by br)						
/*		Provide functionalitty for preview elements
/************************************************************************************/
Durados.Preview = {
    xOffset: 10,
    yOffset: 30,
    tooltipTimeout: 0,
    xMouse: 0,
    yMouse: 0,

    init: function () {

        $('[preview]:not(.preview-init)').addClass('preview-init')
        .hover(function (e) {

            var element = this;

            if ($(element).is('img')) {
                var src = $(element).attr('src')

                if (src == null || src == '') {
                    return;
                }
            }

            Durados.Preview.xMouse = e.pageX;
            Durados.Preview.yMouse = e.pageY;

            element._title = $(element).attr('title');
            $(element).attr('title', '');

            //            //
            //            var parentDiv = $element.parents('td:first').children().first();
            //            element._parentTitle = parentDiv.attr('title');
            //            parentDiv.attr('title', '');

            Durados.Preview.tooltipTimeout = setTimeout(function () {
                Durados.Preview.showPreview(element);
            }, 1000);
        }, Durados.Preview.hidePreview)
        .mousemove(function (e) {
            Durados.Preview.xMouse = e.pageX;
            Durados.Preview.yMouse = e.pageY;
            $("#preview")
			    .css("top", (Durados.Preview.yMouse - Durados.Preview.xOffset) + "px")
			    .css("left", (Durados.Preview.xMouse + Durados.Preview.yOffset) + "px");
        });
    },
    hidePreview: function () {
        clearTimeout(Durados.Preview.tooltipTimeout);
        $(this).attr('title', this._title);
        $("#preview").remove();
    },
    showPreview: function (element, e) {
        var previewElementContent = Durados.Preview.getPreviewElement(element);
        var previewElement = $("<p>").attr('id', 'preview').append(previewElementContent);

        $("body").append(previewElement);
        previewElement.css("top", (Durados.Preview.yMouse - Durados.Preview.xOffset) + "px")
            .css("left", (Durados.Preview.xMouse + Durados.Preview.yOffset) + "px")
            .fadeIn("fast");
    },
    getPreviewElement: function (element) {
        var $element = $(element);
        var previewElement = $("<div>");
        if ($element.is('img')) {
            var imageTitle = (element._title != "") ? "<br/>" + element._title : "";
            previewElement = $("<span>").append($("<img>").attr('src', $element.attr('src')).attr('alt', 'Image preview')).append(imageTitle);
        }
        else if ($element.is('.richTextContainer')) {
            var textAlign = $element.parents('td:first').css('text-align');
            previewElement = $element.clone();
            previewElement.css('text-align', textAlign);
        }

        return previewElement;
    }
}

/************************************************************************************/
/*		Durados.Indications (by br)						
/*		Provide Indications for some Durados issues
/************************************************************************************/
Durados.Indications = {
    fitToWindowWidth: function (guid) {
        var fitToWindowWidth = false;
        if (guid == null || views[guid] == null) { return fitToWindowWidth; }

        var gridDisplayType = views[guid].GridDisplayType;
        var displayType = views[guid].DataDisplayType;

        if (displayType == 'Table' && gridDisplayType == 'FitToWindowWidth') {
            fitToWindowWidth = true;
        }

        return fitToWindowWidth;
    },
    isMainGrid: function (guid) {
        return getMainPageGuid() == guid;
    }
}

function num(value) {
    return parseInt(value, 10) || 0;
};

/************************************************************************************/
/*		duradosImport(by yariv)						
/*		Provide public access to Excel.import
/************************************************************************************/

var zIndexImport = 100000001;
function duradosImport(viewName, afterImportCallback,addPageDialog, pageName) {
    //    var pk = $('div[hasguid="hasguid"]:first').attr('pk');
    //    var viewName = GetViewNameByPK(pk);
    
    var guid = null;

//    if (!views[guid] || views[guid] == null)
//        views[guid] = views.guid;
    var dialogSelector = '#' + guid + viewName + 'durados_Import';

    Excel.Import(viewName, null, true, afterImportCallback, addPageDialog, pageName);
    zIndexImport = zIndexImport + 1000;
    Durados.Dialogs.zIndex($(dialogSelector), zIndexImport);
    $(dialogSelector).parent().find('div.ui-dialog-buttonpane').attr("style", "display:block !important")
    if (addPageDialog == null) {
        $(dialogSelector).dialog({ height: 280 });
        $(dialogSelector).dialog({ width: 440 });
    }
    else {
        $('div[name="ImportAddTemplate"]').hide();
        $('div[name="importTypeDiv"]').hide();
        $("#nullViewdurados_Import").parent().find('button span:contains("Run")').text("Next");
        $('#nullViewdurados_Import').dialog({ width: 740 });
        $('#nullViewdurados_Import').dialog({ height: 280 });
    }


}

function isMobile() {
    try {
        return $('#isMobile').val() == 'yes';
    }
    catch (err) {
        return false;
    }
}
function isMaximizeDialog(guid) {
    var qMax = queryString('max');
    return (qMax == 'yes' || qMax == 'yes#') || views[guid].OpenDialogMax=='True'; //var isMaximize = (qMax == 'yes' || qMax == 'yes#');

}
var Bookmarks;if (!Bookmarks) Bookmarks = {

    loadMessagesForLinkView: function (e, data) {
        if ($(data.dialog).find('select[name="GrobootMessages"]').length < 1)
            return;
        var messagesCheckList = data.dialog.find('select[name="GrobootMessages"]');
        var options = $(data.dialog).find('select[name="GrobootMessages"] > option');
        if (options.length < 1 || (options.length == 1 && options[0].value == '')) {
            var data2 = Bookmarks.getAvailableMessagesFromGB();
            if (data2.success) {
                for (var i in data2.messages) {
                    message = data2.messages[i];
                    $(messagesCheckList).append($("<option></option>")
                                                    .attr("value", message.Id)
                                                    .text(message.Content));
                }
                initDropdownchecklist(messagesCheckList, null, 200);
            }
            else
                modalErrorMsg(data2.description);
        }
        $.post("/Home/GetSelectedGrobootNotificationMessages", { bookmarkId: data.pk }).done(function (data2) {
            if (data2.success) {
                for (var i in data2.selectedIds) {
                    var messageId = data2.selectedIds[i];
                    $(messagesCheckList).find('option[value=' + messageId + ']').attr('selected', true);
                }
                $(messagesCheckList).dropdownchecklist("refresh");
            }
            else {
                modalErrorMsg(data2.description);
            }

        })
        .fail(function (data) {
            modalErrorMsg("failed to get groboot selected messages for this bookmark");
        });

    },

    getAvailableMessagesFromGB: function () {
        var result = { success: false };
        $.ajax({
            type: 'POST',
            url: "/Home/GetAvailableGrobootNotificationMessages",

            success: function (data2) {
                result = data2;
            },
            error: function (data2) {
                modalErrorMsg("failed to get groboot Available messages for this bookmark");
            },

            async: false
        });
        return result;
    },

    validateAndSaveMessages: function (event, data) {

        var selectdMessages = getCheckListValues((data.dialog).find('select[name="GrobootMessages"]'));
        if (selectdMessages.length < 1)
            return true;
        //(data.dialog).find('select[name="GrobootMessages"] >option[selected="selected"]').each(function () {selectdMessages.push( $(this).val()); });

        $.post("/Home/SetBookmarkGrobootNotificationMessages", { bookmarkId: data.pk, messages: selectdMessages }).done(function (data2) {
            if (data2.success) { }
            else {
                modalErrorMsg(data2.description);
            }
        });

    },
    validateGrobootViews: function (pk, dialog) {
        var selectdMessages = getCheckListValues($(dialog).find('select[name="GrobootMessages"]'));
        if (selectdMessages.length < 1)
            return true;
        var validated = false;
        //(data.dialog).find('select[name="GrobootMessages"] >option[selected="selected"]').each(function () {selectdMessages.push( $(this).val()); });
        var url = "/Home/ValidateBookmarkGrobootNotificationView";
        var data = { bookmarkId: pk };
        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            success: function (data2) {
                if (data2.success) {
                    validated = true;
                }
                else {
                    modalErrorMsg(data2.description);
                    validated = false;
                }
            },

            async: false
        });
        return validated;
    },

    getBookmarkNotificationChecklist: function (dialogName) {

        var data = Bookmarks.getAvailableMessagesFromGB();
        if (data.success) {
            if (!data.messages || data.messages.length == 0)
                return '';
            var rowElm = $('<tr></tr>');
            $(rowElm).append('<td></td>').html('pushApp Notification:');
            var chlstTd = $('<td></td>');
            var messagesCheckList = $('<select></select').attr('id', dialogName + '_GBmessagesChecklist').attr('name', 'GrobootMessages').attr('role', 'childrenCheckList');
            $(chlstTd).append(messagesCheckList);
            $(rowElm).append(chlstTd);
            $('div#' + dialogName).find('table').append(rowElm);
            //var messagesCheckList = $('div#' + dialogName).find('select#' + dialogName + '_GBmessagesChecklist');

            initDropdownchecklist(messagesCheckList, null, 200);
            $(messagesCheckList).append($('<option></option>').text('(All)').attr('value', null).attr('selected', false));
            for (var i in data.messages) {
                message = data.messages[i];
                $(messagesCheckList).append($("<option ></option>")
                                                    .attr("value", message.Id)
                                                    .text(message.Content));
            }
            initDropdownchecklist(messagesCheckList, null, 200);
        }
//        else
//            modalErrorMsg(data.description);

    },
    initgbChecklist: function (dialog) {
        var messagesChklst = $(dialog).find('select[name="GrobootMessages"]');
        if (!$(messagesChklst).length)
            return;
       // $(messagesChklst).dropdownchecklist('destroy');
        $(messagesChklst).find('option').each(function () {
            $(this).attr('selected', false);
        });
        $(messagesChklst).dropdownchecklist("refresh");
        //initDropdownchecklist(messagesChklst, null, 200);

    }


}


logTitle = {
    initiate: function () {
        $('#mainMenu > div.logo-title-container > div.title-settings > span.desc-icon').click(function () {
            logTitle.openDialog($(this));
        });
    },

    openDialog: function (settings) {
        var title = $('.title-container').text().trim();
        var src = $('#logo').attr('src');
        var fieldValue = queryString2(src, "pk");
        var fileName = queryString2(src, "fileName");
        var msg = '<div><div class="uploadDiv " id="durados_App_e10_edit_upload_Image" name="Image"><span class="gbutton" id="durados_App_e10_edit_upload_span_Image">Select an Image</span><img style="visibility: visible;" id="durados_App_e10_edit_upload_img_Image" class="upload-logo" src="/MultiTenancy/Download/durados_App?fieldName=Image&amp;filename=&amp;pk=' + fieldValue + '" title="Logo" alt="Logo" uploadpath="/MultiTenancy/Download/durados_App?fieldName=Image&amp;filename=__filename__&amp;pk="><input type="text" id="durados_App_e10_edit_Image" name="Image" upload="Select an Image" value="" viewname="durados_App" pk="" autocomplete="on" d_file="' + fileName + '"></div><input name="title" class="input-title" type="text" value="' + title + '"/></div>';
        var position = null;
        if (settings) position = [settings.position().left, settings.position().top + 75];

        Durados.Dialogs.Confirm('Edit Logo and Title', msg, function (dialog) {
            title = dialog.find('.input-title').val();
            logo = dialog.find('#durados_App_e10_edit_Image').val();
            logTitle.submit(fieldValue, title, logo, fileName);
        }, null,
        function (dialog) {
            var fieldName = "Image";
            var guid = "durados_App_e10_";
            var prefix = editPrefix;
            var viewName = "durados_App";
            UploadFile(fieldName, prefix, guid, viewName);
            showUpload(fieldName, prefix, fileName, $('#' + guid + prefix + 'upload_img_' + fieldName).attr('UploadPath') + fieldValue, guid);
        }, "Update", "Cancel", position)
    },

    submit: function (pk, title, logo, fileName) {
        $.ajax({
            url: "/MultiTenancy/UpdateLogoTitle",
            contentType: 'application/json; charset=utf-8',
            data: { pk: pk, logo: logo, title: title },
            async: false,
            dataType: 'json',
            cache: false,
            error: ajaxErrorsHandler,
            success: function (jsonView) {
                $('.title-container span:first').text(title);
                var src = $('#logo').attr('src');
                $('#logo').attr('src', src.replace(fileName, logo));
            }

        });

    }

}
function IsViewSettings() {
    try {
        if (parent && parent.parent && parent.parent.location.href) {
            var q = queryString2(parent.parent.location.href, "settings");
            return q == "true" || q == "true#";
        }

        else
            return false;
    }
    catch (err) {
        return false;
    }
}

jQuery.fn.extend({
    insertAtCaret: function (myValue) {
        return this.each(function (i) {
            if (document.selection) {
                var dir = jQuery(this).css('direction');
                //For browsers like Internet Explorer
                this.focus();
                var sel = document.selection.createRange();
                sel.text = myValue;
                this.focus();
            }
            else if (this.selectionStart || this.selectionStart == '0') {
                //For browsers like Firefox and Webkit based
                var startPos = this.selectionStart;
                var endPos = this.selectionEnd;
                var scrollTop = this.scrollTop;
                this.value = this.value.substring(0, startPos) + myValue + this.value.substring(endPos, this.value.length);
                this.focus();
                this.selectionStart = startPos + myValue.length;
                this.selectionEnd = startPos + myValue.length;
                this.scrollTop = scrollTop;
            } else {
                this.value += myValue;
                this.focus();
            }
        });
    }
});

function getTopWindowGuid() {
    return window.top.$('#mainAppDiv').find('div.fixedViewPort').first().attr('d_fix');
}
function inIframe() {
    try {
        return window.self !== window.top;
    } catch (e) {
        return true;
    }
}

var Dictionary2; if (!Dictionary2) Dictionary2 = {
 
    addDictionaryLink: function (dialog, guid) {
        if (!guid || guid == '') {
            guid = getTopWindowGuid();
        }
        $(dialog).unbind('dialogbeforeclose', this.close).bind('dialogbeforeclose', { dialog: dialog, guid: guid }, this.close);
        //disabled=
        var dicFields1 = dialog.find('[dicType]:not([disabled]');
        var dicFields = dicFields1.add(dialog.find('[dicType]:[disabled]:hidden'));
        dicFields.each(function () {
            var dicField = this;
            var spanSelector = guid + $(dicField).attr('name') + "_dic_span";
            if ($(dicField).parent().find('span#' + spanSelector).length)
                return;
 
            var dicType = $(dicField).attr('dicType');
            var link = $('<span id="' + spanSelector + '" class="dic-link" title="click here to use dictionary"></span>');
            $(dicField).parent('td').append(link.click(function () { Dictionary2.CreateAndOpen(link, dicField, dicType, dialog, guid); }));
 
        });
    },
    CreateAndOpen: function (dicLink, dicField, dicType, dialog, guid) {
        if ($(dicLink).attr('dic-state') == 'open')
            return;
        var viewNameElmName = $(dicField).attr('dictionaryView');
        if (viewNameElmName) {
            var viewNameElm = dialog.find('[name="' + viewNameElmName + '"]');
            var viewName = viewNameElm.val();
            if (viewNameElm.is('select')) {
                viewName = viewNameElm.find('option[value="' + viewName + '"]').text();
            }
        }
        else
            viewName = $(dicField).attr('viewName');
        //var dicLink = this;
        $.ajax({
            url: '/Block/GetDictionary/' + viewName,
            data: { dicType: dicType },
            contentType: 'application/html; charset=utf-8',
            async: false,
            cache: false,
            error: ajaxErrorsHandler,
            success: function (html) {
                try {
                    var index = html.indexOf("$$error$$", 0)
                    if (index >= 0 && index <= 1000) {
                        modalErrorMsg(html.replace("$$error$$", "")); return;
                    }
                    var dicId = guid + $(dicField).attr('name') + '_dictionary';
                    var dialogWindow = dialog;
                    if (IsViewSettings())
                        window.top.Dictionary2.open(dicId, guid, dialog, html, dicField, dicType, dicLink);
                    else
                        Dictionary2.open(dicId, guid, dialog, html, dicField, dicType, dicLink);
 
                    $(dicLink).attr('dic-state', 'open');
                    //Durados.Dialogs.zIndex($(dicDialog), 100001);
                }
                catch (err) {
                }
 
            }
        });
    },
    open: function (dicId, guid, dialog, html, dicField, dicType, dicLink) {
        var title = dicType == "PlaceHolders" ? "Sytem tokens" : "Columns";
        var dicDialog = $('<div id="' + dicId + '" name="' + guid + '_dictionary"></div>').appendTo(dialog)
                .append(html)
                .dialog({
                    modal: false, title: title, autoOpen: true,
                    width: '400px', resizable: false,
                    minHeight: 0,
                    dialogClass: 'dictionary-dialog',
                    create: function () {
                        $(this).css("maxHeight", 400);
                        //                        $(this).css("overflow-x", 'hidden');
                    },
                    close: function () {
                        var dialog = $(this);
                        dialog.remove();
                        dialog.dialog('destroy');
                        $(dicLink).removeAttr('dic-state');
                    },
                    buttons: {
                        "Close": function () { $(this).dialog('close'); }
                    }
 
                });
        $(dicDialog).find('td span.dic-curr').click(function () {
            var orgElm = dicField;
            var val = $(this).attr('value');
            $(orgElm).insertAtCaret(val.trim());
 
        });
        if (dicType == 'InternalNames' || dicType == 'InternalNamesPlaceHolders') {
            var prevDic = $(dicDialog).find('td span.dic-prev').click(function () {
                var orgElm = dicField;
                var val = $(this).attr('Value');
                $(orgElm).insertAtCaret(val.trim());
            });
           $(prevDic).each(function () { $(this).show(); });
 
 
 
        }
 
    },
    close: function (e) {
        var dicDialogs;
        if (IsViewSettings() && inIframe())
            window.top.Dictionary2.close(e);
        else
            dicDialogs = $('[name="' + e.data.guid + '_dictionary"]');
        if (dicDialogs && dicDialogs.length)
            dicDialogs.each(function () { $(this).dialog('close'); });
    }
}
$(Durados.View).bind('edit', function (e, data) { Dictionary2.addDictionaryLink(data.dialog, data.guid); });
$(Durados.View).bind('add', function (e, data) { Dictionary2.addDictionaryLink(data.dialog, data.guid); });

function editValueInCurrentRow(fieldName, value) {
    var guid = getMainPageGuid();
    var pk = Multi.GetSingleSelection(guid);
    editValueInCurrentView(fieldName, value, pk);
}

function editValueInCurrentView(fieldName, value, pk) {
    var guid = getMainPageGuid();
    var viewName = getMainViewName();
    editValue(viewName, fieldName, value, pk, guid);
}

function editValue(viewName, fieldName, value, pk, guid) {
    $.post('/Home/EditValue/' + viewName,
    {
        fieldName: fieldName, value: value, pk: pk,guid: guid
    },
    function (json) {
        if (json != "success") {

            ajaxNotSuccessMsg(json);

            return;
        }
    });
}
function isOracle(guid){
    return views[guid].SqlProduct == 'Oracle';
}
function HandleProduct(e, data) {
    if (data.viewName == 'Field' && !isOracle(data.guid)) {
        $("input[name='AutoIncrementSequanceName']").closest('table').closest('td').hide();
        $("input[name='AutoIncrement']").closest('table').closest('td').hide()
       
      
    }
}

$(Durados.View).bind('edit', function (e, data) { HandleProduct(e, data); });
$(Durados.View).bind('add', function (e, data) { HandleProduct(e, data); });
$(Durados.Dependencies).bind('OnDependenciesChange', function (e, data) { HandleProduct(e, data); });