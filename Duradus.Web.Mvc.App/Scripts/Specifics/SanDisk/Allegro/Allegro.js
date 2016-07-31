/// POR Product class change
$(Durados.View).bind('edit', function(e, data) {
    var select = data.dialog.find('select[name="ProductClass_POR_Parent"]');
    if (select.length > 0 && select.val() != '') {
        Capacity.Load(data.pk, select.val(), select.closest('form'), true);
    }
    if (data.viewName == 'v_PLM') {
        var forecastACT = $(data.dialog).find('input[name="ForecastACT"]');
        if (forecastACT.length > 0 && forecastACT.val() != '') {
            Plm.ForecastACTChange(forecastACT.closest('form'));
        }

        var CommitACT = $(data.dialog).find('input[name="CommitACT"]');
        if (CommitACT.length > 0 && CommitACT.val() != '') {
            Plm.CommitACTChange(CommitACT.closest('form'));
            Plm.RequiredACTChange(CommitACT.closest('form'));
        }

    }
    if (data.viewName.startsWith("v_PLM")) {
        Plm.SetAvailableStatuses(data.dialog);
    }
    else if (data.viewName == "MemRequest_WaferSupply") {
        select = data.dialog.find('select[name="FK_MemRequest_WaferSupply_WaferSupply_Parent"]');
        LoadDropDown(select, data.guid);
    }
    initBEResponseStatus(data);
});

$(Durados.View).bind('add', function(e, data) {
    var select = data.dialog.find('select[name="ProductClass_POR_Parent"]');
    if (select.length > 0 && select.val() != '' && data.pk != '') {
        Capacity.Load(data.pk, select.val(), select.closest('form'), true);
    }
    else if (views[data.guid].ViewName == "MemRequest_WaferSupply") {
        select = data.dialog.find('select[name="FK_MemRequest_WaferSupply_WaferSupply_Parent"]');
        LoadDropDown(select, data.guid);
    }
    else if (views[data.guid].ViewName == "v_POR") {
        var systemEccSelect = data.dialog.find('select[name="FK_PORSystemECC_v_POR_Parent"]');
        var memorySelect = data.dialog.find('select[name="FK_POR_Nand_Parent"]');
        memorySelect.change(function() {
            LoadDefaultSystemEcc(systemEccSelect, memorySelect, data.guid);
        });
    }

    initBEResponseStatus(data);
});

function initBEResponseStatus(data) {
    var selectBEResponseStatus = data.dialog.find('select[name="FK_BEResponseStatus_v_BETechnical_Parent"]');
    
    var selectBEReleaseName = data.dialog.find('select[name="FK_v_BE_FW_BETechnical_Parent"]');
    if (selectBEReleaseName.length > 0) {
        if (selectBEResponseStatus.length > 0) {
            selectBEReleaseName.change(function() {
                selectBEResponseStatus.val('');
            });
        }
    }

    var selectConfigStatus = data.dialog.find('select[name="FK_BEConfigStatus_v_BETechnical_Parent"]');
    if (selectConfigStatus.length > 0) {
        if (selectBEResponseStatus.length > 0) {
            selectConfigStatus.change(function() {
                selectBEResponseStatus.val('');
            });
        }
    }
    
}

function LoadDefaultSystemEcc(systemEccSelect, memorySelect, guid) {
    var memoryID = memorySelect.val();
    if (memoryID != '') {
        var eccTypeId = GetEccTypeIdFromNand(memoryID, guid);
        if (eccTypeId != null && eccTypeId != '' && systemEccSelect.val() == '') {
            systemEccSelect.val(eccTypeId);
        }
    }
    
}

function GetEccTypeIdFromNand(memoryID, guid) {
    return Data.GetScalar('v_NAND', memoryID, 'ECCTypeId', guid);
}


$(Durados.View).bind('initItemDialog', function(e, data) {
    InitProductClassChange(data.guid);
});


$(Durados.View).bind('initDataTableView', function(e, data) {
    InitProductClassChange(data.guid);
    if (views[data.guid].ViewName == 'v_FabLotPlan') {
        InitImportLink(data.guid);
    }
    if (views[data.guid].ViewName == 'v_PLM') {
        var forecastACT = $('input[name="ForecastACT"]');
        if (forecastACT.length > 0 && forecastACT.val() != '') {
            Plm.ForecastACTChange(forecastACT.closest('form'));
        }
    }

    if (views[data.guid].ViewName == 'v_PLM') {
        var CommitACT = $('input[name="CommitACT"]');
        if (CommitACT.length > 0 && CommitACT.val() != '') {
            Plm.CommitACTChange(CommitACT.closest('form'));
            Plm.RequiredACTChange(CommitACT.closest('form'));
        }
    }
});

$(EditDialog).bind('tabSelected', function(e, data) {
    if (data.action == "edit") {
    
        var forecastACT = $(data.ui.panel).find('input[name="ForecastACT"]');
        if (forecastACT.length > 0 && forecastACT.val() != '') {
            Plm.ForecastACTChange(forecastACT.closest('form'));
        }

        var CommitACT = $(data.ui.panel).find('input[name="CommitACT"]');
        if (CommitACT.length > 0 && CommitACT.val() != '') {
            Plm.CommitACTChange(CommitACT.closest('form'));
            Plm.RequiredACTChange(CommitACT.closest('form'));
        }
    }
});

var Plm; if (!Plm) Plm = {};

Plm.ForecastACTChange = function(form) {
    var forecastACT = form.find('input[name="ForecastACT"]');
    var forecastACTVal = forecastACT.val();

    var RCCommit = form.find('input[name="RCCommit"]');
    var RCCommitVal = RCCommit.val();

    if (RCCommitVal != null && RCCommitVal != '' && forecastACTVal != null && forecastACTVal != '') {
        var forecastACTDate = new Date(forecastACTVal);
        var RCCommitDate = new Date(RCCommitVal);
        RCCommitDate = new Date(RCCommitDate.getYear(), RCCommitDate.getMonth(), RCCommitDate.getDate() - 20);
        var labelTD = RCCommit.parents('td:first').prev();
        if (RCCommitDate > forecastACTDate) {
            labelTD.css('color', 'red');
        }
        else {
            labelTD.css('color', 'black');
        }
    }
}

Plm.SetAvailableStatuses = function(dialog) {
    var beReleaseStatus = dialog.find('input[name="BEReleaseStatus_v_PLM_Parent"]');
    var plmBeStatus = dialog.find('select[name="FK_PLMBEStatus_v_PLM_Parent"]');

    if (plmBeStatus.length == 0)
        return;

    if (beReleaseStatus.length > 0) {
        var option = plmBeStatus.children('option[value="2"]');

        if (beReleaseStatus.val() == '5') {
            option.attr("disabled", "disabled");
        }
        else {
            option.removeAttr("disabled");
        }
    }
    var plmBeStatusVal = plmBeStatus.val();

    Plm.SetAvailableStatuses1(plmBeStatus, plmBeStatusVal);

    plmBeStatus.change(function() {
        setTimeout(function() { Plm.SetAvailableStatuses1(plmBeStatus, plmBeStatusVal) }, 1);
    });
}

Plm.SetAvailableStatuses1 = function(plmBeStatus, plmBeStatusVal) {
    
    var options = plmBeStatus.children('option');

    var Empty = '';
    var Draft = '5';
    var NewRequest = '1';
    var ChangeRequest = '2';
    var POR = '3';
    var NewRequestCR = '7';
    var Canceled = '8';

    var availables = new Array();

    switch (plmBeStatusVal) {
        case Empty:
            availables[Draft] = true;
            break;
        case Draft:
            availables[NewRequest] = true;
            availables[Draft] = true;
            availables[Canceled] = true;
            break;
        case NewRequest:
            availables[NewRequest] = true;
            availables[NewRequestCR] = true;
            availables[POR] = true;
            availables[Canceled] = true;
            break;
        case NewRequestCR:
            availables[NewRequestCR] = true;
            availables[NewRequest] = true;
            availables[POR] = true;
            availables[Canceled] = true;
            break;
        case POR:
            availables[POR] = true;
            availables[ChangeRequest] = true;
            availables[Canceled] = true;
            break;
        case ChangeRequest:
            availables[ChangeRequest] = true;
            availables[POR] = true;
            availables[Canceled] = true;
            break;
        case Canceled:
            availables[Draft] = true;
            availables[Canceled] = true;
            break;

        default:

            break;
    }

    Plm.SetAvailableStatuses2(options, availables);

}

Plm.SetAvailableStatuses2 = function(options, availables) {
    options.each(function() {
        var option = $(this);
        var val = option.val();
        if (availables[val] == null)
            option.attr("disabled", "disabled");
        else
            option.removeAttr("disabled");
   
    });
}

Plm.CommitACTChange = function(form) {
    var forecastACT = form.find('input[name="ForecastACT"]');
    var forecastACTVal = forecastACT.val();

    var CommitACT = form.find('input[name="CommitACT"]');
    var CommitACTVal = CommitACT.val();

    if (CommitACTVal != null && CommitACTVal != '' && forecastACTVal != null && forecastACTVal != '') {
        var forecastACTDate = new Date(forecastACTVal);
        var CommitACTDate = new Date(CommitACTVal);
        var labelTD = forecastACT.parents('td:first').prev();
        if (CommitACTDate < forecastACTDate) {
            labelTD.css('color', 'red');
        }
        else {
            labelTD.css('color', 'black');
        }
    }
}

Plm.RequiredACTChange = function(form) {
    var RequiredACT = form.find('input[name="RequiredACT"]');
    var RequiredACTVal = RequiredACT.val();

    var CommitACT = form.find('input[name="CommitACT"]');
    var CommitACTVal = CommitACT.val();

    if (CommitACTVal != null && CommitACTVal != '' && RequiredACTVal != null && RequiredACTVal != '') {
        var RequiredACTDate = new Date(RequiredACTVal);
        var CommitACTDate = new Date(CommitACTVal);
        var labelTD = CommitACT.parents('td:first').prev();
        if (CommitACTDate > RequiredACTDate) {
            labelTD.css('color', 'red');
        }
        else {
            labelTD.css('color', 'black');
        }
    }
}

//Plm.ActStatusChanges = function(guid) {
//    $('input[name="ForecastACT"]').change(function() {
//        Plm.ForecastACTChange($(this).closest('form'));
//    });

//}

//Plm.CommitACTChanges = function(guid) {
//    $('input[name="CommitACT"]').change(function() {
//        Plm.CommitACTChange($(this).closest('form'));
//        Plm.RequiredACTChange($(this).closest('form'));
//    });

//}



function InitImportLink(guid) {
    var runButton = $('.runReport');
    if (runButton.length == 1) {
        runButton.css('margin-bottom','0px');
        var td = runButton.parents('td:first');
        if (td.length == 1) {
            td.append("<br>");
            var importLink = $('<A title="Import from Excel file" onclick="Excel.Import(\'v_FabLotPlan\',\'' + guid + '\');return false" href="#">Import</A>');
            td.append(importLink);
        
        }
    }
}

function InitProductClassChange(guid) {
    var selects = $("div[guid='" + guid + "']").find('select[name="POR_PORProductClassCapacity_Children"]');

        selects.each(function() {
        var select = $(this);
        if ($(this).attr("filter") == "filter") { return; }
        select.html('');
        initDropdownchecklist(select);

        $("div[guid='" + guid + "']").find('select[name="ProductClass_POR_Parent"]').change(function() {

            if ($(this).attr("filter") == "filter") { return; }
            var val = $(this).val();
            if (val != '') {
                Capacity.Load(0, val, $(this).closest('form'), true);
            }
            else {
                select.html('');
                initDropdownchecklist(select);
            }

        });

        $("div[guid='" + guid + "']").find('select[name="FK_ProductLine_v_POR_Parent"]').change(function() {
            select.html('');
            initDropdownchecklist(select);
        });

    });
}


var Capacity; if (!Capacity) Capacity = {};


Capacity.Load = function(pk, fk, dialog, performChange) {
    showProgress();

    $.ajax({
        url: rootPath + 'AllegroPOR/GetProductClassCapacities',
        contentType: 'application/json; charset=utf-8',
        data: { pk: pk, fk: fk },
        async: false,
        dataType: 'json',
        cache: false,
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            hideProgress();
            alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
        },
        success: function(capacityInfo) {
            if (capacityInfo == null || capacityInfo == '') {
                alert("error");
            }
            else {
                if (performChange) {
                    var select = dialog.find('select[name="POR_PORProductClassCapacity_Children"]');
                    
                    select.html(capacityInfo.Options);
                    initDropdownchecklist(select);
                }
            }
            hideProgress();

        }

    });
}

/// Nand 1st Product Out Change

//$(Durados.View).bind('edit', function(e, data) {
//    var input = data.dialog.find('input[name="ProductionOut"]');
//    Nand.ChangeProductionDates(input.val(), input.closest('form'));
//});

//$(Durados.View).bind('initItemDialog', function(e, data) {
//    InitProductionOutChange(data.guid);
//});


//$(Durados.View).bind('initDataTableView', function(e, data) {
//    InitProductionOutChange(data.guid);
//});

//function InitProductionOutChange(guid) {
//    var inputs = $("div[guid='" + guid + "']").find('input[name="ProductionOut"]').change(function() {
//        var input = $(this);
//        Nand.ChangeProductionDates(input.val(), input.closest('form'));
//    });

//    $("div[guid='" + guid + "']").find('input[name="Month0"]').closest('form').each(function() {
//    $(this).find('input[name="Month0"]').css('font-weight', 'bold');
//    $(this).find('input[name="Month0"]').css('width', '138px');
//    $(this).find('input[name="Month0"]').parent('td:first').prev().css('font-weight', 'bold');
//    });

//    //$("div[guid='" + guid + "']").find('input[name="Month0"]').closest('form').find('input[name="Month0"]').parent('td:first').prev().css('font-weight', 'bold');
//}

//var Nand; if (!Nand) Nand = {};

//Nand.ChangeProductionDates = function(prodDate, dialog) {
//    for (var index = 1; index <= 3; ++index) {
//        var input = dialog.find('input[name="Month_' + (index + '') + '"]');
//        if (input.length == 1) {
//            if (prodDate != '') {
//                var date = toDate(prodDate);
//                date = getNextMonths(date, -index);

//                input.parent('td:first').prev().text(dateFormat(date) + ':');
//                input.removeAttr('disabled');
//            }
//            else {
//                input.parent('td:first').prev().text('Month-' + (index + ':'));
//                input.attr('disabled', 'disabled');
//            }
//        }
//    }
//    for (var index = 0; index <= 6; ++index) {
//        var input = dialog.find('input[name="Month' + (index + '') + '"]');
//        if (input.length == 1) {
//            if (prodDate != '') {
//                var date = toDate(prodDate);
//                date = getNextMonths(date, index);

//                input.parent('td:first').prev().text(dateFormat(date) + ':');
//                input.removeAttr('disabled');
//            }
//            else {
//                input.parent('td:first').prev().text('Month+' + (index + ':'));
//                input.attr('disabled', 'disabled');
//            }
//        }
//    }
//}

//function toDate(dateStr) {
////    var dateParts = dateStr.split('/');
////    if (dateParts.length == 3) {
////        var month = dateParts[0];
////        var day = dateParts[1];
////        var year = dateParts[2];

////        var temp = month + "/" + day + "/" + year;
////        var cfd = Date.parse(temp);

////        var date = new Date(cfd);
////        return date;
////    }
////    return null;

//    var cfd = Date.parse(dateStr);

//    var date = new Date(cfd);
//    return date;

//}

//function getNextMonths(date, m) {
//    return new Date(date.getYear(), date.getMonth() + m, date.getDate());
//}

//function dateFormat(d) {
//    var m_names = new Array("Jan", "Feb", "Mar",
//"Apr", "May", "Jun", "Jul", "Aug", "Sep",
//"Oct", "Nov", "Dec");

//    var curr_date = d.getDate();
//    var curr_month = d.getMonth();
//    var curr_year = d.getFullYear();
//    return (curr_date + "-" + m_names[curr_month] + "-" + curr_year);
//}

//function dateFormat(d) {
//    var m_names = new Array("Jan", "Feb", "Mar",
//"Apr", "May", "Jun", "Jul", "Aug", "Sep",
//"Oct", "Nov", "Dec");

//    //var curr_date = d.getDate();
//    var curr_month = d.getMonth();
//    var curr_year = d.getFullYear();
//    //return (curr_date + "-" + m_names[curr_month] + "-" + curr_year);
//    return (m_names[curr_month] + "-" + curr_year);
//}

/// PMM Parameters
$(EditDialog).bind('onafterEditLoad', function(e, data) {
    if (data.viewName == "v_TechnologyProductClassCapacity") {

        showProgress();

        $.ajax({
            url: rootPath + 'AllegroProductLine/GetProductLineParameters',
            contentType: 'application/json; charset=utf-8',
            data: { pk: data.pk },
            async: false,
            dataType: 'json',
            cache: false,
            error: function(XMLHttpRequest, textStatus, errorThrown) {
                hideProgress();
                alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
            },
            success: function(parameters) {
                if (parameters == null || parameters == '') {
                    alert("error");
                }
                else {
                    disableAllFields(editPrefix, data.guid, data.json);
                    $(parameters.List).each(function() {
                        var elm = data.dialog.find('[name="' + this + '"]');
                        //                        if (elm.attr('class') == "dropdownchecklist") {
                        //                            initDropdownchecklist(elm);
                        //                        }
                        //                        else {
                        //                            elm.attr('disabled', 'disabled');
                        //                        }
                        elm.removeAttr('disabled');
                        if (elm.parent('td:first').prev().length == 1) {
                            elm.parent('td:first').prev().css("color", "red");
                        }
                        else {/// inline adding
                            elm.parent('td:first').parents('td:first').parents('td:first').prev().css("color", "red");
                        }
                    });
                }
                hideProgress();

            }

        });
    }
});

function disableAllFields(prefix, guid, jsonView) {
    for (var index = 0, len = jsonView.Fields.length; index < len; ++index) {
        var field = jsonView.Fields[index].Value;
        var id = guid + prefix + field.Name;
        var htmlField = $('#' + id);

        if (htmlField.length == 1) {
            if ($(htmlField[0]).attr('upload') == 'upload') {
            }
            //            else if (field.Type == "CheckList") {
            //                //htmlField.dropdownchecklist("disable");
            //                htmlField.attr('disabled', 'disabled');
            //            }
            else if ($(htmlField[0]).attr('radioButtons') == 'radioButtons') {
            }
            else if (htmlField.attr('outsideDependency') == 'outsideDependency') {
            }

            else if (htmlField.attr('insideDependency') == 'insideDependency') {
            }

            else if (htmlField[0].type == "textarea") {
            }
            else {
                htmlField.attr('disabled', 'disabled');
                if (htmlField.parent('td:first').prev().length == 1) {
                    htmlField.parent('td:first').prev().css("color", "#C0C0C0");
                }
                else {/// inline adding
                    htmlField.parent('td:first').parents('td:first').parents('td:first').prev().css("color", "#C0C0C0");
                }
                
            }
        }

    }

}

function LoadDropDown(select, guid) {
    var fieldName = select.attr("name");
    $.ajax({
        url: views[guid].gGetSelectListUrl,
        contentType: 'application/json; charset=utf-8',
        data: {
            viewName2: views[guid].ViewName,
            fieldName: fieldName
        },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function(selectList) {
            
            for (var index = 0, len = selectList.Options.length; index < len; ++index) {
                if (selectList.Options[index].Text != '(Empty)') {
                    select.find('option[value="' + selectList.Options[index].Value + '"]').text(selectList.Options[index].Text);
                }
            }
            
        }

    });
}


//// data
//var Data; if (!Data) Data = {};

//Data.GetScalar = function(viewName, pk, fieldName, guid) {
//    var scalar = null;
//    showProgress();
//    url = rootPath + views[guid].Controller + '/GetScalar/' + viewName;
//    $.ajax({
//        url: url,
//        contentType: 'application/json; charset=utf-8',
//        data: { viewName: viewName, pk: pk, fieldName: fieldName },
//        async: false,
//        dataType: 'json',
//        cache: false,
//        error: ajaxErrorsHandler,
//        success: function(json) {
//            scalar = json;
//        }

//    });
//    hideProgress();

//    return scalar;
//}
