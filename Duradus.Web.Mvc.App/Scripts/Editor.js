/////////////////// FieldEditor /////////////////////

Durados.FieldEditor = function () { }

Durados.FieldEditor.prototype.constructor = Durados.FieldEditor;

Durados.FieldEditor.prototype.init = function () { }

var prevField = null;

Durados.FieldEditor.datePickerOff = true;

Durados.FieldEditor.prototype.show = function (cell, guid, name, pk, url) {
    adjustDataTableHeightDisabled = true;
    var elementContainer = this.getElementContainer(guid, name, cell);

    var fieldEditor = this;
    var currentID = 'f' + validationGuid;

    if (prevField != null) {

        if (prevField.params.element.hasClass("date")) {
            Durados.FieldEditor.datePickerOff = true;
        }
        prevField.elementChanged(prevField.params.element, prevField.params.cell, prevField.params.guid, prevField.params.name, prevField.params.pk, prevField.params.url, prevField.params.fieldJson, prevField.params.validation, prevField.params.elementContainer);
    }


    var element = this.getElement(elementContainer);
    if (element.length == 0)
        return;
    var type = this.getElementType(element);
    if (!(type == "Text" || type == "DropDown" || type == "Check" || type == "InsideDependency" || type == "Autocomplete" || type == "CheckList" || type == "Url" || type == "Upload" || type == "TextArea"))
        return;
    var location = this.getLocation(cell);
    var size = this.getSize(cell);
    var viewJson = this.getViewJson(guid);
    var fieldJson = this.getFieldJson(name, viewJson);
    this.showElement(elementContainer, size, location, cell, guid);

    if (type != 'TextArea') {
        var validation = getFieldValidation(fieldJson.Fields[0].Value, elementContainer.find('span:first').attr('id'), element);
    }

    this.params = { element: element, cell: cell, guid: guid, name: name, pk: pk, url: url, fieldJson: fieldJson, validation: validation, elementContainer: elementContainer };

    prevField = fieldEditor;


    this.bindElementBlur(element, cell, guid, name, pk, url, fieldJson, validation, elementContainer, validationGuid);
}

var elementGuid = 0;
var validationGuid = 0;

Durados.FieldEditor.prototype.getRichTextElement = function (cell) {
    var richText = cell.find('.richTextContainer');

    var richTextContainer = $("<div/>")
    .attr('type', 'textArea')
    .attr('click', '1')
    .attr('d_val', cell.attr('d_val'))
    .click(function () {
        var guid = cell.attr('d_guid');
        var fieldName = cell.attr('d_name');
        var value = cell.attr('d_val');
        var rich = richText.attr('rich') == '1';

        var callback = function (html) {
            richTextContainer.attr('d_val', html);

            if (!rich && html.length > 0) {
                var length = richText.attr('d_len');
                if (length > 0 && html.length > length) {
                    html = html.substr(0, length) + '...';
                }
            }

            if (richText.length == 0) {
                var img = cell.find('img[fullUrl="yes"]:first');
                if (img.length == 1) {
                    img.attr('src', html);
                    img.parent('a:first').attr('href', html);
                }
            }
            else {
                richText.children().first().html(html);
            }
            //richText.children().first().html(html);
            richTextContainer.blur();
        }

        RichDialog.Open(fieldName, false, rich, guid, value, callback);
    });

    return richTextContainer;
}


Durados.FieldEditor.prototype.getUploadElement = function (cell) {
    var image = cell.find('img');
    var a = cell.find('a:first');
    var fullUrl = image.attr('fullUrl') == 'yes';

    var imageContainer = $("<div/>")
    .attr('type', 'upload')
    .attr('click', '1')
    .attr('d_val', cell.attr('d_val'))
    .click(function () {
        var guid = cell.attr('d_guid');
        var fieldName = cell.attr('d_name');
        var viewName = views[guid].ViewName;
        var file = cell.attr('d_val');
        var src = image.attr('src');
        var dialog = null;

        var callback = function (url, fileName) {
            //                                var b = 'cross';
            //                                var url = 'http://qacontent.blob.core.windows.net/485/' + b + '.png';
            //                                var fileName = b + '.png';
            if (fileName != null && fileName != '' && (url != fileName || fileName.indexOf('http:') != -1) || fileName.indexOf('https:')) {
                image.attr('src', url);
            }
            else {
                image.remove();
            }
            if (fullUrl) {
                imageContainer.attr('d_val', url);
                if (image.length == 0)
                    a.text(url);
            }
            else {
                imageContainer.attr('d_val', fileName);
                if (url == fileName && fileName.indexOf('http:') == -1 && fileName.indexOf('https:') == -1)
                    a.text(fileName);
            }
            imageContainer.blur();
        };
        dialog = RichDialog.OpenUploadDialog(viewName, fieldName, guid, callback, file, src);
        /*if ($.browser.mozilla) {
            if (dialog.find('.ff-warning').length == 0) {
                dialog.prepend($("<div class='ff-warning'><div>Not supported in Firefox.</div><div>Import your Excel file using Chrome or Internet Explorer.</div></div>"));
            }
        }*/
        
        updateDoanloadButton(guid, dialog, file);
    });

    return imageContainer;
}

function updateDoanloadButton(guid, dialog, file) {
    var downloadIcon = dialog.find('a.uploadIcon');
    

    if (downloadIcon.length > 0) {
        downloadIcon.show();
        var href = downloadIcon.attr('d_href');
        downloadIcon.attr('href', href.replace('__filename__', encodeURI(file)));
        
    }
}

Durados.FieldEditor.prototype.getElementContainer = function (guid, name, cell) {
    var dialog = this.getEditDialog(guid);
    var element = dialog.find("[name='" + name + "']");
    var parent = element.parents('td:first');
    var id = 'v' + validationGuid++;
    var span = $('<span id=' + id + '></span>');
    if (element.hasClass("date")) {
        var elements = parent.children();
        span.append('<input type="text" class="date" df="' + element.attr("df") + '"  dt="' + element.attr("dt") + '" />');
        if (elements.length > 2) { span.append($(elements[2]).clone()); }
    }
    else if (element.hasClass("uploadDiv")) {
        element = this.getUploadElement(cell);
        span.append(element);
    }
    else if (element.is('textarea')) {
        element = this.getRichTextElement(cell);
        span.append(element);
    }
    else if (element.is(':checkbox')) {
        element.css('visibility', '')
        span.append(element.clone());
    }
    else {
        span.html(parent.html());
    }

    var id = 'e' + elementGuid++;
    span.find(':first').attr('id', id);
    var form = $('<form id="f' + validationGuid + '" class="inGridForm"></form>');
    form.append(span);

    return form;
}

Durados.FieldEditor.prototype.getElement = function (elementContainer) {
    return elementContainer.find('span:first').children(':first-child');
}

Durados.FieldEditor.prototype.getEditDialog = function (guid) {
    var dialog = $('#' + guid + 'DataRowEdit');
    return dialog;
}

Durados.FieldEditor.prototype.setValue = function (element, value, text) {
    var elementType = this.getElementType(element);
    if (elementType == "Text" || elementType == "DropDown" || elementType == "InsideDependency") {
        element.val(value);
    }
    else if (elementType == "CheckList") {
        element.find("option").each(function () {
            $(this).removeAttr('selected');
        });

        $(value.split(',')).each(function () {
            element.find("option[value='" + this + "']").attr('selected', 'selected');
        });
    }
    else if (elementType == "Check") {
        //        element.attr('checked', value);
        Durados.CheckBox.SetChecked(element, value);
    }
    else if (elementType == 'Autocomplete') {
        SetAutoCompleteValueId(element, text, value)
    }
    else if (elementType == 'Url') {
        var a = element.find(".url");
        this.setUrl(a, value);
    }
}

Durados.FieldEditor.prototype.setUrl = function (a, value) {
    var displayText = '';
    var href = '';
    var target = '_blank';

    if (value != null) {
        var values = value.split('|');

        if (values.length == 3) {
            displayText = values[0];
            href = values[2];
            target = values[1];
        }
        else if (values.length == 1) {
            href = values[0];
            displayText = values[0];
            
        }
    }

    if (displayText == '' && href != '#') {
        displayText = href;
    }

    var format = a.attr('format');
    if (format == 'ButtonLink') {
        a.find('button span').text(displayText);
    }
    else {
        a.find('span').text(displayText);
    }
    a.attr('href', href);
    a.attr('target', target);
    a.attr('value', value);
}

Durados.FieldEditor.prototype.getElementType = function (element) {
    if (element.attr('type') == 'upload') {
        return 'Upload';
    }
    else if (element.attr('role') == "childrenCheckList") {
        return 'CheckList';
    }
    else if (element.attr('radioButtons') == 'radioButtons') {
        return 'Radio';
    }
    else if (element.attr('outsideDependency') == 'outsideDependency') {
        return 'OutsideDependency';
    }
    else if (element.attr('hasInsideTrigger') == 'hasInsideTrigger') {
        return 'InsideDependency';
    }
    if (element.attr('type') == 'textArea') {
        return 'TextArea';
    }
    else if (element[0].type == "checkbox") {
        return 'Check';
    }
    else if (element.hasClass('Autocomplete') && element.hasClass('ac_input')) {
        return 'Autocomplete';
    }
    else if (element[0].type == 'select-one') {
        return 'DropDown';
    }
    else if (element[0].type == "hidden") {
        return 'Hidden';
    }
    else if (element.find(".url").length == 1) {
        return 'Url';
    }
    else if (element.attr('color') == '1') {
        return 'Color';
    }
    else {
        return 'Text';
    }
}


Durados.FieldEditor.prototype.getLocation = function (cell) {
    return cell.offset();
}

Durados.FieldEditor.prototype.getSize = function (cell) {
    return { width: cell.width(), height: cell.height() };
}

Durados.FieldEditor.prototype.handleKeys = function (element, cell, guid, name, pk, url, fieldJson, validation, elementContainer, event) {
    var editor = this;
    if (event.keyCode == $.ui.keyCode.ESCAPE) { /// escape - cancel editing
        Durados.FieldEditor.datePickerOff = true;
        editor.endEdit(elementContainer);
    }
    editor.enter = event.keyCode == $.ui.keyCode.ENTER; /// return - perform editing and move to cell bellow

    if (editor.enter) {
        event.preventDefault();
        event.stopImmediatePropagation();
        event.stopPropagation();
        editor.shift = event.shiftKey;

        if (element.hasClass("date")) { //&& element.datepicker("widget").is(":visible")
            element.datepicker("destroy");
            Durados.FieldEditor.datePickerOff = true;
        }
        editor.elementChanged(element, cell, guid, name, pk, url, fieldJson, validation, elementContainer);
    }
    editor.tab = event.keyCode == $.ui.keyCode.TAB; /// tab  - perform editing and move to next cell
    if (editor.tab) {
        event.preventDefault();
        event.stopImmediatePropagation();
        event.stopPropagation();
        editor.shift = event.shiftKey;
        if (element.hasClass("date")) {
            element.datepicker("destroy");
            Durados.FieldEditor.datePickerOff = true;
        }
        editor.elementChanged(element, cell, guid, name, pk, url, fieldJson, validation, elementContainer);
    }
}

Durados.FieldEditor.prototype.bindElementBlur = function (element, cell, guid, name, pk, url, fieldJson, validation, elementContainer) {
    var elementType = this.getElementType(element);
    var editor = this;
    if (elementType == "Text" || elementType == "DropDown" || elementType == "Check" || elementType == "InsideDependency" || elementType == "Autocomplete" || elementType == "CheckList" || elementType == 'Url' || elementType == 'Upload' || elementType == 'TextArea') {
        element.keydown(function (event) {
            editor.handleKeys(element, cell, guid, name, pk, url, fieldJson, validation, elementContainer, event);
        });

        //$(document).unbind('click', Durados.FieldEditor.CloseAllEditors);
        //$(document).bind('click', Durados.FieldEditor.CloseAllEditors);

        // in checklist there is no element in focus
        if (elementType == "CheckList") {
            $(document).bind('keydown', function (event) {
                editor.handleKeys(element, cell, guid, name, pk, url, fieldJson, validation, elementContainer, event);
                if (!event.shiftKey)
                    $(document).unbind('keydown');

            });
        }
        else {
            $(document).unbind('keydown');
        }

        if (elementType == "Autocomplete") {
            if (element.attr('d_type') == 'AutocompleteColumn') {
                element.blur(function () {
                    if (!$(this).val()) {
                        editor.endEdit(elementContainer);
                    }
                });
            }
            $(Autocomplete).bind('result', function (event, item) {
                editor.elementChanged(element, cell, guid, name, pk, url, fieldJson, validation, elementContainer);
            });
        }
        else {
            element.blur(function (e) {
                editor.elementChanged(element, cell, guid, name, pk, url, fieldJson, validation, elementContainer);
            });
        }


        if (elementType == "Check" && $.browser.webkit) {
            element.mousedown(function (e) {
                var v = element.attr("checked");
                var nv = v ? "" : "checked";
                element.attr("checked", nv);
            });
        }
    }
}

Durados.FieldEditor.prototype.getInsideDependencyFK = function (element, cell) {
    var tr = cell.parent();
    var triggerName = element.attr('triggerName');
    if (triggerName == null || triggerName == '') {
        return null;
    }
    var triggerCell = tr.find('td[d_name="' + triggerName + '"]');
    var fk = triggerCell.attr('d_val');
    return fk;
}

Durados.FieldEditor.prototype.loadInsideDependency = function (fk, element, cell) {
    var guid = cell.attr('d_guid');
    var fieldName = cell.attr('d_name');
    var viewName = views[guid].ViewName;

    Dependency.Load(viewName, fieldName, fk, element, guid);
}


Durados.FieldEditor.prototype.showElement = function (elementContainer, size, location, cell, guid) {

    var element = this.getElement(elementContainer);
    var click = element.attr('click') == '1';

    if (click) {
        element.click();
        return;
    }
    cell.children().first().hide();
    cell.css("padding", "0px 5px");
    cell.removeAttr('onmousedown');
    cell.append(elementContainer);

    var width = size.width + 5;

    var top = location.top + (cell.innerHeight() - element.height() - 2) / 2;
    adjustDataTableHeightDisabled = true;

    var a = elementContainer.find('input:checkbox:first');
    a.checkbox({ cls: 'jquery-safari-checkbox' });

    var type = this.getElementType(element);
    if (type == "Text" || type == "DropDown" || type == "Check" || type == "CheckList" || type == "InsideDependency" || type == "Autocomplete") {

        if (type == "Text") {
            //            if ($.browser.msie) { width -= 17; top -= 1; }
            if ($.browser.msie) { width -= 4; top -= 1; }
        } else if (type == "DropDown" || type == "CheckList") {
            if ($.browser.msie) { width -= 1; top -= 1; }

        } else if (type == "Check") {
            elementContainer.css({ "padding": "2px" });
            if ($('body').attr('dir') == 'rtl') {
                location.left += width - 15;
            }
        } else if (type == "Autocomplete") {
            Autocomplete.InitElement(element, guid);
        } else if (type == "InsideDependency") {
            var fk = this.getInsideDependencyFK(element, cell);
            if (fk == null || fk == '') {
                elementContainer.remove();
                return;
            }
            this.loadInsideDependency(fk, element, cell);
        }
        if (type != "Check") {
            element.width(width);
        }
        else {
            location.left = cell.find('span').offset().left - 8;
        }
        //element.height(size.height);

        //by br
        //        elementContainer.css({ position: "absolute",
        //            top: top, left: location.left + 5
        //        });

    }

    var text = null;

    if (type == "Url") {
        elementContainer.css("margin", "5px");
    }
    else if (type == "Upload") {
        //        elementContainer.css("margin", "5px 0px");
        cell.css("padding", "0px");
    }
    else if (type == "Text" || type == "Autocomplete") {
        elementContainer.find('input').css('text-align', cell.find('div:first').css('text-align'));
        if (type == 'Autocomplete') {
            text = this.getCellDisplayValue(cell);
        }
    }

    var value = this.getCellValue(cell, type);
    this.setValue(element, value, text);


    if (type == "CheckList") {
        initDropdownchecklist(element, guid, width);
        //var cl_wrapper = $(cl_wrapper).drop();
        //cell.children('div').css('visibility','hidden');
        adjustDataTableHeightDisabled = false;
    } else if (type == "Check" && $.browser.webkit && false) {
        adjustDataTableHeightDisabled = false;
    } else {
        setTimeout(function () {
            element.focus();
            setTimeout(function () {
                element.select(); adjustDataTableHeightDisabled = false;
            }, 20);
        }, 10);
    }


    if (element.hasClass("date")) {

        //elementContainer.find('img').remove();
        //element.removeClass('nadate');
        triggerDateChanged(element);

        var dateType = num(DateFormats.getValidDateType(element.attr("df")));
        var df = duradosGetJQueryDateFormat(element.attr("df"), dateType);

        switch (dateType) {
            case DateFormats.dateType.dateOnly:
                element.datepicker({ showButtonPanel: true, showOn: 'focus', beforeShow: datePickerBeforeShow, onChangeMonthYear: datePickerOnChangeMonthYear, dateFormat: df.dateFormat, onClose: function (dateText, inst) {
                    Durados.FieldEditor.datePickerOff = true; $(inst.input).blur();
                }
                });
                break;
            case DateFormats.dateType.dateAndTime:
                element.datetimepicker({ showButtonPanel: true, showOn: 'focus', beforeShow: datePickerBeforeShow, onChangeMonthYear: datePickerOnChangeMonthYear, dateFormat: df.dateFormat, timeFormat: df.timeFormat, onClose: function (dateText, inst) {
                    Durados.FieldEditor.datePickerOff = true; $(inst.input).blur();
                }
                });
                break;
            case DateFormats.dateType.timeOnly:
                element.timepicker({ showButtonPanel: true, showOn: 'focus', beforeShow: datePickerBeforeShow, onChangeMonthYear: datePickerOnChangeMonthYear, timeFormat: df.timeFormat, onClose: function (dateText, inst) {
                    Durados.FieldEditor.datePickerOff = true; $(inst.input).blur();
                }
                });
                break;
            default:
                break;
        }

        Durados.FieldEditor.datePickerOff = false;
    }
}

Durados.FieldEditor.prototype.endEdit = function (elementContainer) {

    if (!Durados.FieldEditor.datePickerOff) return;
    if (elementContainer.find(".date").length) { //&& element.datepicker("widget").is(":visible")
        elementContainer.find(".date").datepicker("destroy");
    }

    var cell = elementContainer.parent('td');
    var element = this.getElement(elementContainer);

    if (element.attr('type') != 'upload') {
        cell.css('padding', '0px 10px');
    }

    cell.attr('onmousedown', 'Durados.FieldEditor.Show(this, event);');

    elementContainer.hide();
    elementContainer.remove();
    Durados.FieldEditor.cancel = false;

    cell.children().first().show();
}

Durados.FieldEditor.prototype.elementChanged = function (element, cell, guid, name, pk, url, fieldJson, validation, elementContainer) {

    prevField = null;

    var message = '';

    if (!Durados.FieldEditor.datePickerOff && element.hasClass("date") && element.datepicker("widget").is(":visible")) {
        return;
    }

    var elementType = this.getElementType(element);


    try {
        if (validation && !validation.validate()) {

            //if (elementType == "Autocomplete" && false) {

            //    this.endEdit(elementContainer);

            //    return;

            //} else {
            setTimeout(function () {
                element.focus();
            }, 1);

            Durados.FieldEditor.cancel = true;
            return false;
            //}
        }
    }
    catch (err) {
    }

    var value = this.getElementValue(element, elementType);
    fieldJson.Fields[0].Value.Value = value;

    var oldValue = this.getCellValue(cell, elementType);
    var oldDisplayValue = this.getCellDisplayValue(cell);
    var newDisplayValue = this.getElementDisplayValue(element);

    if (oldValue != value)
        this.setCellValue(cell, value, newDisplayValue, oldValue, oldDisplayValue, elementType);

    if (element.hasClass("date")) {
        Durados.FieldEditor.datePickerOff = true;
        triggerDateChanged(element, cell);
    }

    this.endEdit(elementContainer);


    var newCell = null;
    //var direction = '';

    if (this.tab) {
        if (this.shift) {
            newCell = this.getPrevCell(cell); //direction = 'L';
        }
        else {
            newCell = this.getNextCell(cell); //direction = 'R';
        }
    }
    else if (this.enter) {
        if (this.shift) {
            newCell = this.getAboveCell(cell); //direction = 'U';
        }
        else {
            newCell = this.getBellowCell(cell); //direction = 'D';
        }
    }

    if (newCell) {
        if (Durados.GridHandler.insertElementIntoViewport(newCell)) {
            //Durados.GridHandler.scrollParentsData(newCell);
        }
        setTimeout(function () { var ncell = newCell; ncell.mousedown(); }, 40);
    }


    if (oldValue == value)
        return;

    var editor = this;

    $.post(url,
    {
        pk: pk,
        jsonView: Sys.Serialization.JavaScriptSerializer.serialize(fieldJson), guid: guid
    },
    function (json) {
        if (json != "success") {

            editor.setCellValue(cell, oldValue, oldDisplayValue, value, newDisplayValue, elementType);
            ajaxNotSuccessMsg(json);

            return;
        }

        var d_refresh = cell.attr('d_refresh') == 'd_refresh';

        if (d_refresh) {
            setTimeout(function () {
                var scalar = Data.GetScalar(views[guid].ViewName, pk, name, guid);
                editor.setCellValue(cell, value, scalar, oldValue, oldDisplayValue, elementType);
            }, 1);
        }

        var d_hasCss = cell.attr('d_hasCss') == 'd_hasCss';

        if (d_hasCss) {
            var oldClass = cell.attr('class');
            var newClass = oldClass.replace(' ' + oldValue + ' ', ' ' + value + ' ');

            cell.removeClass(oldClass);
            cell.addClass(newClass);
        }
        //cell.attr('class', cell.attr('class').replace(' ' + oldValue + ' ', ' ' + value + ' '));
        //setTimeout(function() {
        //cell.toggleClass(cell.attr('class').replace(' ' + oldValue + ' ', ' ' + value + ' '));
        //}, 1);

        if (views[guid].updateParent) {
            if (cell.attr('updateParent') == 'updateParent') {
                updateParent(guid);
            }
        }

    });
}

Durados.FieldEditor.prototype.getElementValue = function (element, type) {
    if (type == "Text" || type == "DropDown" || type == "InsideDependency") {
        return element.val();
    }
    else if (type == "CheckList") {
        var val = element.val();
        if (val == null)
            return '';
        else {
            var str = val.toString();
            if (str.substr(0, 1) == ",") {
                str = str.substring(1);
            }
            return str;
        }
    }
    else if (type == "Check") {
        return Durados.CheckBox.IsChecked(element);
        //        return Durados.Checkbox.IsChecked(element);//by br
        //        return element.val() == 'on';
    }
    else if (type == "Autocomplete") {
        if (element.attr('d_type') == 'AutocompleteColumn') {
            return element.val();
        }
        else {
            return GetAutoCompleteValueId(element);
        }
    }
    else if (type == 'Url') {
        var a = element.find(".url");
        return a.attr('value');
    }
    else if (type == 'Upload' || type == 'TextArea') {
        return element.attr('d_val');
    }

    return null;
}


Durados.FieldEditor.prototype.isTextArea = function (cell) {
    return cell.attr('textArea') == 'textArea';
}

Durados.FieldEditor.prototype.getNextCell = function (cell) {
    var editor = this;
    var next = cell.next();
    var tr = cell.parent();
    var endOfTr = false;
    while (!endOfTr) {
        if (next.length > 0) {
            if (next.attr("d_role") == "cell") {
                //            if (next.attr("d_role") == "cell" && !editor.isTextArea(next)) {
                //$(Durados.View).trigger('rowChanged', { row: next, originalCell: cell, tr: tr });
                return next;
            }
            else {
                next = next.next();
            }
        }
        else {
            endOfTr = true;
        }
    }

    if (endOfTr) {
        var nextTr = tr.next();

        if (nextTr.length == 0) return null;

        next = nextTr.find('td[d_role="cell"]:first');

        if (next.length > 0) {
            $(Durados.View).trigger('rowChanged', { row: next, originalCell: cell, tr: nextTr });
            return next;
        }
    }

    return next;
}

Durados.FieldEditor.prototype.getPrevCell = function (cell) {
    var editor = this;
    var prev = cell.prev();
    var tr = cell.parent();
    var endOfTr = false;
    while (!endOfTr) {
        if (prev.length > 0) {
            if (prev.attr("d_role") == "cell") {
                //            if (prev.attr("d_role") == "cell" && !editor.isTextArea(prev)) {
                //$(Durados.View).trigger('rowChanged', { row: prev, originalCell: cell, tr: tr });
                return prev;
            }
            else {
                prev = prev.prev();
            }
        }
        else {
            endOfTr = true;
        }
    }

    if (endOfTr) {
        var prevTr = tr.prev();
        if (prevTr.length == 0 || prevTr.attr('d_row') == null) return null;

        prev = prevTr.find('td[d_role="cell"]:last');

        if (prev.length > 0) {
            $(Durados.View).trigger('rowChanged', { row: prev, originalCell: cell, tr: prevTr });
            return prev;
        }
    }

    return prev;
}

Durados.FieldEditor.prototype.getBellowCell = function (cell) {
    var editor = this;
    var tr = cell.parent();
    var index = tr.children('td').index(cell);

    var nextTr = tr;
    var endOfTr = false;
    while (!endOfTr) {

        var nextTr = nextTr.next();
        if (nextTr.length == 0) {
            nextTr = tr.parent().children('tr[d_row="d_row"]:first');
        }

        var bellow = nextTr.children('td:eq(' + index + ')');

        if (bellow.attr("d_role") == "cell") {
            //        if (bellow.attr("d_role") == "cell" && !editor.isTextArea(bellow)) {
            $(Durados.View).trigger('rowChanged', { row: bellow, originalCell: cell, tr: tr });
            if (tr.attr('id') == nextTr.attr('id')) {
                return null;
            }
            else {
                return bellow;
            }
        }
    }
    return null;
}

Durados.FieldEditor.prototype.getAboveCell = function (cell) {
    var editor = this;
    var tr = cell.parent();
    var index = tr.children('td').index(cell);

    var prevTr = tr;
    var endOfTr = false;
    while (!endOfTr) {

        var prevTr = prevTr.prev();
        if (prevTr.length == 0) {
            prevTr = tr.parent().children('tr[d_row="d_row"]:last');
        }

        var above = prevTr.children('td:eq(' + index + ')');

        if (above.attr("d_role") == "cell") {
            //        if (above.attr("d_role") == "cell" && !editor.isTextArea(above)) {
            $(Durados.View).trigger('rowChanged', { row: above, originalCell: cell, tr: tr });
            if (tr.attr('id') == prevTr.attr('id')) {
                return null;
            }
            else {
                return above;
            }
        }
    }
    return null;
}


Durados.FieldEditor.prototype.isValid = function (element, message) {
    return true;
}

Durados.FieldEditor.prototype.getCellValue = function (cell, type) {
    if (type == "Text" || type == "DropDown" || type == "InsideDependency" || type == "Autocomplete" || type == "CheckList" || type == "Url" || type == 'Upload' || type == 'TextArea') {
        return cell.attr('d_val');
    }
    else if (type == "Check") {
        return cell.attr('d_val') == "Yes";
    }

    return null;
}

Durados.FieldEditor.prototype.getCellDisplayValue = function (cell) {
    var cellDiv = cell.find('div:first');
    var a = cellDiv.find('a');

    if (a.length == 0) {
        var span = cellDiv.find('span');
        if (span.length == 0) {
            if (!cellDiv.length) { cellDiv = cell; }
            return cellDiv.text();
        }
        else {
            return span.text();
        }
    }
    else {
        return a.text(); // parent, children, other links....
    }
}

Durados.FieldEditor.prototype.setCellValue = function (cell, value, displayValue, prevValue, prevDisplayValue, type) {
    cell.attr('d_val', value); //alert(value)

    var cellDiv = cell.find('div:first');

    if (!cellDiv.length) cellDiv = cell;

    if (type == 'Text') {
        cellDiv.text(displayValue);
    }
    else if (type == 'DropDown' || type == "InsideDependency" || type == "Autocomplete" || type == "CheckList") {
        var a = cellDiv.find('a');
        if (a.length == 0) {
            var span = cellDiv.find('span');
            if (span.length == 0) {
                cellDiv.text(displayValue);
            }
            else {
                var limit = cell.attr('limit');
                if (limit != null && limit != '') {
                    limit = parseInt(limit);
                    var values = displayValue.split(',');
                    if (limit > 0) {
                        span.attr('alt', displayValue);
                        span.attr('title', displayValue);
                    }
                    if (limit > 0 && values.length > limit) {
                        var shortDisplayValue = '';
                        for (var index = 0; index < limit; ++index) {
                            shortDisplayValue = shortDisplayValue + values[index] + ',';
                        }
                        shortDisplayValue = shortDisplayValue.slice(0, -1) + "...";
                        span.text(shortDisplayValue);
                    }
                    else {
                        span.text(displayValue);
                    }
                }
                else {
                    span.text(displayValue);
                }
            }
        }
        else {
            a.text(displayValue);
            if (a.attr('d_role') == 'parentTableView') {
                var url = a.attr('d_url');
                url = url.replace("__=" + prevDisplayValue, "__=" + displayValue);
                url = url.replace("=" + prevValue, "=" + value);
                a.attr("d_url", url);
            }
            else {
                a.attr('pk', value);
            }

        }
    }
    else if (type == 'Check') {
        cell.attr('d_val', value ? "Yes" : "No");
        var text = value ? String('&#8730;') : "&nbsp;";
        var className = value ? "checked" : "not-checked";
        cellDiv.html("<span class='" + className + "'>" + text + "</span>");
    }
    else if (type == 'Url') {
        var a = cell.find(".url");
        this.setUrl(a, value);
    }
    else {
        //parent, children, other links....
    }
    if (displayValue != prevDisplayValue) {
        Durados.GridHandler.setHeaderWidth(cell);
    }
}

Durados.FieldEditor.prototype.getElementDisplayValue = function (element) {
    var elementType = this.getElementType(element);
    if (elementType == "Text" || elementType == "Autocomplete") {
        return element.val();
    }
    else if (elementType == "DropDown" || elementType == "InsideDependency") {
        return element.find('option:selected').text();
    }
    else if (elementType == "CheckList") {
        var text = '';
        element.find('option:selected').each(function () {
            if ($(this).text() != "(All)") {
                text = text + $(this).text() + ",";
            }
        });
        if (text != '') {
            text = text.slice(0, -1)
        }
        return text;
    }
    else {
        return ''; // other elements here....
    }
}

Durados.FieldEditor.prototype.getViewJson = function (guid) {
    return GetJsonViewForEdit(guid);
}

Durados.FieldEditor.prototype.getFieldJson = function (name, viewJson) {
    // clone
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

/// inheritence
Durados.CellFieldEditor = function () { };
Durados.CellFieldEditor.prototype = new Durados.FieldEditor;
Durados.CellFieldEditor.prototype.constructor = Durados.CellFieldEditor;
// override
Durados.CellFieldEditor.prototype.init = function () {
    // base
    Durados.FieldEditor.prototype.init.call(this);
}
// override
Durados.CellFieldEditor.prototype.show = function (cell, guid, name, pk, url) {
    // base
    Durados.FieldEditor.prototype.show.call(this, cell, guid, name, pk, url);

}


Durados.FieldEditor.CloseAllEditors = function () {
    //var fieldEditor = new Durados.CellFieldEditor();
    var editor = Durados.FieldEditor.prototype;
    $("form.inGridForm").each(function () {
        //fieldEditor.endEdit($(this));
        editor.endEdit($(this));
        //        $(this).hide().remove();
    });
    Durados.FieldEditor.cancel = false;
}

Durados.FieldEditor.MouseDown = function (e, div) {
    e = e || event;
    $(document).mousedown();
    if (isRightClick(e)) {// && e.target.nodeName != 'INPUT'
        Durados.FieldEditor.CloseAllEditors();
    }
    return true;
}

function isRightClick(e) {
    if (window.event) {
        if (window.event.button == 2) return true; // ie        
    } else if (e && e.which && e.which == 3) { // rest
        return true;
    }
    return false;
}

function isLeftClick(e) {
    var IE7 = (navigator.appVersion.indexOf("MSIE 7.") == -1) ? false : true;
    var IE8 = (navigator.appVersion.indexOf("MSIE 7.") == -1) ? false : true;
    var IEOldVersion = IE7 || IE8;

    return (!IEOldVersion && e.button == 0) || (IEOldVersion && e.button == 1)
}

Durados.FieldEditor.setFocus = function () {
    $('table.toolbar:first').focus();
}

// factory
Durados.FieldEditor.Show = function (cell, e) {
    e = e || event;

    var fieldEditor = new Durados.CellFieldEditor();

    if (Durados.FieldEditor.cancel) {
        Durados.FieldEditor.setFocus();
        return;
    }

    if ($(cell).attr('d_role') != 'cell') {
        Durados.FieldEditor.setFocus();
        return;
    }

    //Durados.FieldEditor.CloseAllEditors();

    var ctrl = false;
    var shift = false;

    if (e && e.ctrlKey) {
        ctrl = true;
    } else if (e && e.shiftKey) {
        shift = true;
    }

    //var type = Durados.FieldEditor.GetType(cell);

    var guid = $(cell).attr('d_guid');
    var name = $(cell).attr('d_name');
    var pk = $(cell).attr('d_pk');
    var derivation = $(cell).attr('d_derivation');
    var disabled = derivation == 'disabled';
    var url = views[guid].EditOnlyUrl;

    //Enable edit only if safetyOff=true
    var safetyOffElement = $('#' + guid + "Safety");
    if (safetyOffElement.length) {
        var safetyOff = Durados.CheckBox.IsChecked($('#' + guid + "Safety"));
        if (!safetyOff)
            return;
    }

    //setTimeout(function() {
    cellsSelection.cellClicked(cell, e, ctrl, shift);
    //}, 10);

    if (e && (ctrl || shift))
        return;

    if (isRightClick(e))
        return;

    //    if (type == 'Cell') {
    //        fieldEditor = new Durados.CellFieldEditor();
    //    }
    if (fieldEditor && !disabled) {
        fieldEditor.show($(cell), guid, name, pk, url);
    }


    return fieldEditor;
}

Durados.FieldEditor.setCellValue = function (td, newValue, newDisplayValue, guid, name) {
    var cell = td;

    var fieldEditor = new Durados.CellFieldEditor();


    var elementContainer = fieldEditor.getElementContainer(guid, name, cell);

    var element = fieldEditor.getElement(elementContainer);
    if (element.length == 0)
        return;
    var type = fieldEditor.getElementType(element);
    var oldValue = fieldEditor.getCellValue(cell, type);
    var oldDisplayValue = fieldEditor.getCellDisplayValue(cell);

    fieldEditor.setCellValue(cell, newValue, newDisplayValue, oldValue, oldDisplayValue, type);
}

Durados.FieldEditor.GetType = function (cell) {
    return 'Cell';
}

////////////// CellsSelection ////////////////
Durados.CellsSelection = function () {
    if (this.clear) this.clear();
}

Durados.CellsSelection.prototype.constructor = Durados.CellSelection;

Durados.CellsSelection.prototype.clear = function () {
    this.clearWithoutShow();
    this.show();
}

Durados.CellsSelection.prototype.clearWithoutShow = function () {
    this.rows = new Array();
    this.firstCell = null;
    this.firstRow = null;
}

Durados.CellsSelection.prototype.cellClicked = function (td, e, ctrl, shift) {

    if (shift) {
        Durados.FieldEditor.CloseAllEditors();
        this.addRect(td);
    }
    else if (ctrl) {
        Durados.FieldEditor.CloseAllEditors();
        this.addCell(td, true);
    }
    else {
        if (isRightClick(e) && $(td).hasClass('selectedCell')) { return; }

        this.select(td);
    }
}

Durados.CellsSelection.prototype.select = function (td) {
    this.clearWithoutShow();
    var cell = new Durados.Cell(td);
    var row = new Durados.Row(cell.GetTR());
    row.GetCells().push(cell);
    this.firstCell = cell;
    this.firstRow = row;
    this.rows.push(row);
    this.show();
}

Durados.CellsSelection.prototype.selectCurrentRow = function () {
    var guid = this.GetGuid();
    if (guid == null)
        return;
    var pks = Multi.GetSelection(guid);
    var tr;
    if (pks.length == 1) {
        tr = Multi.GetElementByPK(guid, pks[0]);
    }
    else {
        return;
    }

    this.addRow($(tr).parents("tr[d_row='d_row']:first"));
}

Durados.CellsSelection.prototype.addRow = function (tr) {

    var row = new Durados.Row(tr);
    this.rows.push(row);

    var tds = tr.children("td");
    for (var cellIndex = 0; cellIndex < tds.length; cellIndex++) {
        var cell = row.GetCell(cellIndex);
        if (cell != null) {
            row.GetCells().push(cell);
        }
    }

    this.show();
}

Durados.CellsSelection.prototype.addCell = function (td, singleRow) {
    if (this.rows.length != 1 && singleRow) {
        this.select(td);
        return;
    }
    var row = this.rows[0];

    var cell = new Durados.Cell(td);

    if (!(cell.GetPK() == row.GetPK() && cell.GetGuid() == row.GetGuid())) {
        this.select(td);
        return;
    }

    row.GetCells().push(cell);

    this.show();
}

Durados.CellsSelection.prototype.addRect = function (td) {
    var row1 = this.firstRow;

    if (row1 == null || this.rows.length == 0) {
        this.select(td);
        return;
    }

    var cell1 = this.firstCell;

    if (cell1 == null || row1.GetCells().length == 0) {
        this.select(td);
        return;
    }

    var cell2 = new Durados.Cell(td);

    var row2 = new Durados.Row(cell2.GetTR());

    if (row1.GetGuid() != row2.GetGuid()) {
        this.select(td);
        return;
    }

    var rowIndex1 = row1.GetIndex();
    var rowIndex2 = row2.GetIndex();

    var cellIndex1 = cell1.GetIndex();
    var cellIndex2 = cell2.GetIndex();

    if (rowIndex1 == rowIndex2 && cellIndex1 == cellIndex2) {
        this.select(td);
        return;
    }

    this.rows = new Array();

    if (rowIndex1 > rowIndex2) {
        var temp = rowIndex1;
        rowIndex1 = rowIndex2;
        rowIndex2 = temp;
    }
    if (cellIndex1 > cellIndex2) {
        var temp = cellIndex1;
        cellIndex1 = cellIndex2;
        cellIndex2 = temp;
    }

    for (var rowIndex = rowIndex1; rowIndex <= rowIndex2; rowIndex++) {
        var row = this.GetRow(rowIndex);
        this.rows.push(row);
        for (var cellIndex = cellIndex1; cellIndex <= cellIndex2; cellIndex++) {
            var cell = row.GetCell(cellIndex);
            if (cell != null) {
                row.GetCells().push(cell);
            }
        }
    }

    this.show();
}

Durados.CellsSelection.prototype.GetTable = function () {
    if (this.firstRow == null)
        return null;
    return this.firstRow.GetTable();
}

Durados.CellsSelection.prototype.GetTR = function (index) {
    var table = this.GetTable();
    if (table == null)
        return null;

    return table.children('tr:eq(' + index + ')');
}

Durados.CellsSelection.prototype.GetRows = function () {
    return this.rows;
}


Durados.CellsSelection.prototype.GetRow = function (index) {

    var tr = this.GetTR(index);
    var row = new Durados.Row(tr);
    if (row.pk == null)
        return null;
    return row;
}

Durados.CellsSelection.prototype.GetGuid = function () {
    if (this.rows.length > 0)
        return this.rows[0].guid;
    else
        return null;
}

Durados.CellsSelection.prototype.copy = function (selection) {
    if (selection == null || selection.rows == null)
        return false;

    var selLen = selection.rows.length;

    if (this.rows == null)
        return false;

    var len = this.rows.length;

    if (selLen == 0 || len == 0) {
        return false;
    }

    var m = len % selLen;

    if (m != 0)
        return false;

    // permutations
    var p = len / selLen;

    for (var i = 0; i < p; i++) {
        for (var r = 0; r < selLen; r++) {
            var rowC = selection.rows[r];
            var rowP = this.rows[p * i + r];

            for (var c = 0; c < rowC.cells.length; c++) {
                var cellC = rowC[c];
                var cellP = rowP[c];


            }
        }
    }
}

Durados.CellsSelection.prototype.show = function () {
    var selectedCellClass = "selectedCell";
    $("." + selectedCellClass).removeClass(selectedCellClass);
    var guid = this.GetGuid();

    if (this.rows.length > 0) {
        $("#" + guid + 'ajaxDiv').find('input.Multi:checked').each(function () {
            if ($(this).parents('tr').first().attr('guid') == guid)
            //$(this).attr('checked', '');
                Durados.CheckBox.SetChecked($(this), '');
        });
    }

    $(this.rows).each(function () {
        var row = this;
        if (views[row.guid].MultiSelect) {
            row.GetTR().find("input.Multi:first").attr('checked', true);
        }

        $(row.GetCells()).each(function () {
            var cell = this;
            cell.GetTD().addClass(selectedCellClass);
        });
    });
    this.showBorder("");

    if (guid != null && views[guid].MultiSelect) {
        setTimeout(function () {
            Multi.MarkSelection(guid);
        }, 1);
    }
    $(Durados.CellsSelection).trigger('onchanged', { cellsSelection: this });

}

Durados.CellsSelection.prototype.showCopy = function () {
    //    this.hideCopy();
    //    var copyCellClass = "copyCell";
    //    $(this.rows).each(function() {
    //        var row = this;
    //        $(row.GetCells()).each(function() {
    //            var cell = this;
    //            cell.GetTD().addClass(copyCellClass);
    //        });
    //    });
    $("td.selectedCell").removeClass("selectedCell");
    this.showBorder("Copy");
}

Durados.CellsSelection.prototype.showBorder = function (postfix) {
    this.hideBorder(postfix);
    var l = this.rows.length;
    var isRTL = $('body').attr('dir') == 'rtl';
    if (isRTL) {
        var leftClass = 'borderCellRight' + postfix;
        var rightClass = 'borderCellLeft' + postfix;
    } else {
        var leftClass = 'borderCellLeft' + postfix;
        var rightClass = 'borderCellRight' + postfix;
    }

    if (l > 1) {
        for (var r = 0; r < l; r++) {
            var row = this.rows[r];
            var top = r == 0;
            var bottom = r == this.rows.length - 1;
            for (var c = 0; c < row.cells.length; c++) {

                var left = c == 0;
                var right = c == row.cells.length - 1;


                var cell = row.cells[c];
                var td = cell.GetTD();
                var cellIndex = cell.GetIndex();

                if (top) {
                    td.addClass("borderCellTop" + postfix);
                    var tr = row.tr.prev();
                    if (tr.length == 1)
                        tr.children('td:eq(' + cellIndex + ')').addClass("borderCellBottom" + postfix);
                }
                if (bottom) {
                    td.addClass("borderCellBottom" + postfix);
                    var tr = row.tr.next();
                    if (tr.length == 1)
                        tr.children('td:eq(' + cellIndex + ')').addClass("borderCellTop" + postfix);
                }
                if (left) {
                    td.addClass(leftClass);
                    var prev = td.prev();
                    if (prev.length == 1)
                        prev.addClass(rightClass);
                }
                if (right) {
                    td.addClass(rightClass);
                    var next = td.next();
                    if (next.length == 1)
                        next.addClass(leftClass);
                }
            }
        }
    }
    else {
        //var borderCellClass = "borderCell" + postfix;
        $(this.rows).each(function () {
            var row = this;
            var cellsCount = row.cells.sort(compareCells).length;

            if (cellsCount <= 1)
                return;

            for (var c = 0; c < cellsCount; c++) {
                var cell = row.cells[c];

                var td = cell.GetTD();
                var cellIndex = cell.GetIndex();

                var right = c == row.cells.length - 1;
                var left = c == 0;

                //right
                if (right) {
                    //td.addClass(borderCellClass);
                    td.addClass(rightClass);
                    var next = td.next();
                    if (next.length == 1)
                        next.addClass(leftClass);
                }

                //left
                if (left) {
                    td.addClass(leftClass);
                    var prev = td.prev();
                    if (prev.length == 1)
                        prev.addClass(rightClass);
                }
                //bottom
                td.addClass("borderCellBottom" + postfix);
                var trNext = row.tr.next();
                if (trNext.length == 1)
                    trNext.children('td:eq(' + cellIndex + ')').addClass("borderCellTop");
                //top
                td.addClass("borderCellTop" + postfix);
                var trPrev = row.tr.prev();
                if (trPrev.length == 1)
                    trPrev.children('td:eq(' + cellIndex + ')').addClass("borderCellBottom");
            }
        });
    }
}

Durados.CellsSelection.prototype.hideBorder = function (postfix) {
    $("td.borderCell" + postfix).removeClass("borderCell" + postfix);
    $("td.borderCellTop" + postfix).removeClass("borderCellTop" + postfix);
    $("td.borderCellBottom" + postfix).removeClass("borderCellBottom" + postfix);
    $("td.borderCellLeft" + postfix).removeClass("borderCellLeft" + postfix);
    $("td.borderCellRight" + postfix).removeClass("borderCellRight" + postfix);
}

Durados.CellsSelection.prototype.hideCopy = function () {
    this.hideBorder("Copy");
}

Durados.CellsSelection.prototype.clone = function () {
    return jQuery.extend(true, {}, this);
}

Durados.Cell = function (td) {
    this.td = $(td);
    this.name = this.td.attr("d_name");
    this.val = this.td.attr("d_val");
    this.pk = this.td.attr("d_pk");
    this.guid = this.td.attr("d_guid");
}

Durados.Cell.prototype.constructor = Durados.Cell;

Durados.Cell.prototype.GetName = function () {
    return this.name;
}

Durados.Cell.prototype.GetValue = function () {
    return this.val;
}

Durados.Cell.prototype.GetPK = function () {
    return this.pk;
}

Durados.Cell.prototype.GetGuid = function () {
    return this.guid;
}

Durados.Cell.prototype.GetIndex = function () {
    return this.GetTR().children('td').index(this.GetTD());
}

Durados.Cell.prototype.GetTR = function () {
    return this.GetTD().parent();
}

Durados.Cell.prototype.GetTD = function () {
    return this.td;
}



Durados.Row = function (tr) {
    this.cells = new Array();
    this.tr = tr;
    this.pk = this.tr.attr("d_pk");
    this.guid = this.tr.attr("guid");
}

Durados.Row.prototype.constructor = Durados.Row;

Durados.Row.prototype.GetPK = function () {
    return this.pk;
}

Durados.Row.prototype.GetGuid = function () {
    return this.guid;
}

Durados.Row.prototype.GetTR = function () {
    return this.tr;
}



Durados.Row.prototype.GetTable = function () {
    return this.GetTR().parent();
}

Durados.Row.prototype.GetIndex = function () {
    return this.GetTable().children('tr').index(this.GetTR());
}

Durados.Row.prototype.GetCells = function () {
    return this.cells;
}

Durados.Row.prototype.GetTD = function (index) {
    var tr = this.GetTR();
    return tr.children('td:eq(' + index + ')');
}

Durados.Row.prototype.GetCell = function (index) {

    var td = this.GetTD(index);
    if (td.attr("d_role") != "cell")
        return null;
    var cell = new Durados.Cell(td);
    return cell;
}

function compareCells(c1, c2) {
    return c1.GetIndex() - c2.GetIndex();
}

////////////// CopyPaste //////////////

var d_copy = null;

$(document).ready(function () {
    $(document).keyup(function (e) {
        var code = e.which;  //(e.keyCode ? e.keyCode : e.which);

        if (e.ctrlKey) {
            if (code == '67') { // ctrl + c
                if (copyPaste != null) {
                    copyPaste.copy();
                }
            }
            else if (code == '86') { // ctrl + v
                if (copyPaste != null) {
                    copyPaste.paste();
                }
            }
        } else if (code == '27') { // 27 - escape - cancel editing $.ui.keyCode.ESCAPE
            if (cellsSelection != null) {
                cellsSelection.clear();
            }
            if (d_copy != null) {
                d_copy.hideCopy();
                d_copy = null;
            }
        }
    });
});

Durados.CopyPaste = function (guid) {
    this.copyButtons = $("a.copyButton");
    this.pasteButtons = $("a.pasteButton");
    this.cellsSelction = null;
    var copyPaste = this;

    var safetyOff = Durados.CheckBox.IsChecked($('#' + guid + "Safety"));
    if (safetyOff) {
        this.enableCopy(true);
        this.copyButtons.each(function () {
            $(this).unbind("click");
            $(this).one("click", function () {
                copyPaste.copy();
                return false;
            });
        });
    }
    else {
        this.enableCopy(false);
    }

    this.pasteButtons.each(function () {
        copyPaste.enablePaste(false);
        $(this).unbind("click");
        $(this).one("click", function () {
            copyPaste.paste();
            return false;
        });
    });

    $(Durados.CellsSelection).bind('onchanged', function () {
        if (d_copy != null) {
            copyPaste.enablePaste(true);
        }
    });

    this.validationMessages = [];
    this.validationMessages.push("The area you selected to copy does not match the paste area. The operation was canceled.");
    this.validationMessages.push("You are about to paste on an area of #cols columns by #rows rows. Please confirm.");
    this.validationMessages.push("You are about to paste on an area of #cols columns by #rows rows. Please confirm.");
    this.validationMessages.push("You are about to paste on an area of #cols columns by #rows rows. Please confirm.");
    this.validationMessages.push("You are about to paste on an area of #cols columns by #rows rows. Please confirm.");
    this.validationMessages.push("Please confirm a paste of a single cell.");
    this.validationMessages.push("You are about to paste on an area of #cols columns by #rows rows. Please confirm.");
}

Durados.CopyPaste.prototype.constructor = Durados.CopyPaste;

Durados.CopyPaste.prototype.enableAction = function (buttons, able) {
    var copyPaste = this;
    buttons.each(function () {
        if ($(this).attr("type") == "button") {
            copyPaste.enableButton($(this), able);
        }
        else {
            copyPaste.enableElement($(this), able);
        }
    });

}

Durados.CopyPaste.prototype.enableCopy = function (able) {
    this.enableAction(this.copyButtons, able);
}

Durados.CopyPaste.prototype.enablePaste = function (able) {
    this.enableAction(this.pasteButtons, able);
}

//Durados.CopyPaste.prototype.enablePaste = function(able) {
//    var copyPaste = this;
//    this.pasteButtons.each(function() {
//        if ($(this).attr("type") == "button") {
//            copyPaste.enablePasteButton($(this), able);
//        }
//        else {
//            copyPaste.enablePasteElement($(this), able);
//        }
//    });

//}

Durados.CopyPaste.prototype.enableButton = function (button, able) {
    button.attr('disabled', able);
}

Durados.CopyPaste.prototype.enableElement = function (element, able) {
    if (able) {
        element.removeClass("disablePaste");
        element.unbind("click");
        element.one("click", function () {
            copyPaste.paste();
            return false;
        });
    }
    else {
        element.addClass("disablePaste");
        element.unbind("click");
        element.one("click", function () {
            return false;
        });
    }
}

Durados.CopyPaste.prototype.copy = function () {
    if (cellsSelection.GetRows().length == 0) {
        //cellsSelection.selectCurrentRow();
        return;
    }
    d_copy = cellsSelection.clone();
    this.enablePaste(false);
    d_copy.showCopy();

    /*  //Copy to Clipboard
    var csv = '';
    $(d_copy.rows).each(function() {        
    $(this.cells).each(function() {
    csv += this.val + "\t";
    });
    if (csv && csv.length > 1) {
    csv = csv.substring(0,csv.length-1);
    }
    csv += "\n";      
    });
    if (window.clipboardData) {
    window.clipboardData.clearData();
    window.clipboardData.setData('Text',csv);
    }
    */
}

Durados.CopyPaste.prototype.paste = function () {
    if (d_copy == null) {
        return;
    }

    var paste = cellsSelection;

    if (!paste.rows || !paste.rows[0]) { return; }

    var status = this.validation(d_copy, paste);

    var message = this.validationMessages[status];
    var pasteRowsCount = paste.rows.length;
    var copyRowsCount = d_copy.rows.length;
    var pasteCellsCount = paste.rows[0].cells.length;
    var copyCellsCount = d_copy.rows[0].cells.length;
    if (status == 2) {
        pasteRowsCount = copyRowsCount;
        pasteCellsCount = copyCellsCount;
    }
    if (status == 6) {
        pasteCellsCount = copyCellsCount;
    }

    message = message.replace("#cols", pasteCellsCount);
    message = message.replace("#rows", pasteRowsCount);

    var guid = paste.GetGuid();
    if (guid == null)
        return;

    if (status == 0) {
        alert(message);
        return;
    }
    else if (!confirm(message)) {
        return;
    }

    var CopyPasteJson = this.getJson(d_copy, paste, status);

    var viewName = views[guid].ViewName;

    //alert(viewName + " => " + guid); return;

    saveElementScrollsPosition('');

    showProgress();

    /// call ajax to paste on server
    $.post(rootPath + views[guid].Controller + '/CopyPaste/' + viewName,
        {
            jsonView: Sys.Serialization.JavaScriptSerializer.serialize(CopyPasteJson), guid: guid
        },
        function (html) {
            var indexError = html.indexOf("$$error$$", 0);
            var indexMessage = html.indexOf("$$message$$", 0);
            var hasError = indexError > 0 && indexError < 1000;
            var hasMessage = indexMessage > 0 && indexMessage < 1000;
            if (hasError) {
                ajaxNotSuccessMsg(html, guid);
            }
            else if (hasMessage) {
                ajaxNotSuccessMsg(html, guid);
                refreshView();
            }
            else {
                EditDialog.HandleSuccess(html, guid);
                d_copy.hideCopy();
                //d_copy.showCopy();
                //paste.copy(d_copy);
            }
            resetElementScrollsPosition('');
            //$(Durados.CopyPaste).trigger('onafterCopyPaste', { guid: guid, json: CopyPasteJson, viewName: viewName });
        });

}

Durados.CopyPaste.prototype.validation = function (copy, paste) {
    ////// VALIDATION STATUS
    // 0 - mo match
    // 1 - matching area
    // 2 - upper left corner
    // 3 - using entire rows
    // 4 - matching columns and matching rows permutation
    // 5 - Cell to Cell
    // 6 - left columns permutation

    if (copy == null || copy.rows == null)
        return 0;
    if (paste == null || paste.rows == null)
        return 0;

    var copyRowsCount = copy.rows.length;
    var pasteRowsCount = paste.rows.length;

    if (copyRowsCount == 0 || pasteRowsCount == 0)
        return 0;

    var copyCellsCount = copy.rows[0].cells.length;
    var pasteCellsCount = paste.rows[0].cells.length;

    if (copyCellsCount == 0 || pasteCellsCount == 0)
        return 0;

    if (copy.rows[0].cells[0].name != paste.rows[0].cells[0].name)
        return 0;


    if (pasteCellsCount == 1 && pasteRowsCount == 1) {
        if (copyCellsCount == 1 && copyRowsCount == 1) {
            return 5;
        }
        else {
            var index = paste.rows[0].GetIndex();
            var lastIndex = index + copyRowsCount - 1;
            var lastRow = paste.GetRow(lastIndex);
            if (lastRow == null)
                return 0;
            else {
                if (copyRowsCount > 1) {
                    var cellIndex = copy.rows[0].cells[copyCellsCount - 1].GetIndex();
                    var td = lastRow.GetCell(cellIndex).td;
                    paste.addRect(td);
                }
                else {
                    $(copy.rows[0].cells).each(function () {
                        var cell = this;
                        if (copy.rows[0].cells[0] != cell) {
                            var cellIndex = cell.GetIndex();
                            paste.addCell(lastRow.GetCell(cellIndex).td, true);
                        }
                    });
                }
                return 2;
            }
        }
    }

    if (pasteCellsCount != copyCellsCount) {
        if (pasteCellsCount != 1) {
            return 0;
        }
        else {
            if (copy.rows[0].cells[0].name != paste.rows[0].cells[0].name)
                return 0;

            //            $(paste.rows).each(function() {
            //                var row = this;
            //                $(copy.rows[0].cells).each(function() {
            //                    var cell = this;
            //                    if (copy.rows[0].cells[0] != cell) {
            //                        var cellIndex = cell.GetIndex();
            //                        paste.addCell(row.GetCell(cellIndex).td, false);
            //                    }
            //                });
            //            });
            return 6;
        }
    }

    for (var i = 0; i < copy.rows[0].cells.length; i++) {
        if (copy.rows[0].cells[i].name != paste.rows[0].cells[i].name)
            return 0;
    }

    if (copyRowsCount == pasteRowsCount)
        return 1;

    if (pasteRowsCount % copyRowsCount == 0)
        return 4;
    else
        return 0;
}

Durados.CopyPaste.prototype.getJson = function (copy, paste, status) {

    var jsonObj = { "Source": { "FieldsNames": [], "FieldsValues": [] }, "Destination": { "RowsPKs": []} }

    if (copy.rows.length == 0)
        return jsonObj;
    $(copy.rows[0].cells).each(function () {
        jsonObj.Source.FieldsNames.push(this.name);
    });

    $(copy.rows).each(function () {
        var values = [];
        $(this.cells).each(function () {
            values.push(this.val);
        });
        jsonObj.Source.FieldsValues.push(values);
    });


    $(paste.rows).each(function () {
        jsonObj.Destination.RowsPKs.push(this.pk);
    });

    return jsonObj;
}

