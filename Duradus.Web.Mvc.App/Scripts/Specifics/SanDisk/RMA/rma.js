$(Durados.View).bind('add', function(e, data) {
    if (isRma(data.guid)) {
        //        var countrySelect = data.dialog.find('select[name="FK_Country_v_RmaRmaCountryId_Parent"]');
        //        var initiatorSelect = data.dialog.find('select[name="FK_v_User_v_Rma_Parent"]');

        //        var userId = initiatorSelect.val();

        //        var countryId = GetInitiatorCountry(userId, data.guid);

        //        countrySelect.val(countryId);

        //        bindSapNoChange(data);

        var productFieldName = "FK_v_ProductLine_v_Rma_Parent";
        var faLocationFieldName = "FK_v_FaLocation_v_Rma_Parent";

        var pruductSelect = data.dialog.find('select[name="' + productFieldName + '"]');
        var faLocationSelect = data.dialog.find('select[name="' + faLocationFieldName + '"]');
        pruductSelect.change(function() {
            LoadDefaultLocation(pruductSelect, faLocationSelect, data.guid);
        });


    }

    hideReportId(data);

    if (isItem(data.guid)) {
        SetParentPart(data,data.dialog,data.guid);
    }

    if (data.viewName == "v_Failure") {
        setExternalDefaults(data.dialog)
    }
});

var rmaMainViewCachData = { guid : ''};



function SetParentPart(data,dialog, guid) {

    var TopMarkingFieldName = "TopMarking";
    var ParentFieldName = "FK_agile_Part_v_Item_Parent";
    var SKUFieldName = "FK_agile_Part_v_Rma_Parent";
                
    var ProductLineName = "FK_v_ProductLine_v_Rma_Parent";
    var MfgDateName = "MfgDate";
    
    var TopMarkingInput = dialog.find('input[name="' + TopMarkingFieldName + '"]');
    var ParentInput = dialog.find('input[name="' + ParentFieldName + '"]');
    var SKUInput = $('#' + rmaMainViewCachData.guid + 'DataRowEdit').find('[name="' + SKUFieldName + '"]');
    var ProductLineSelect = $('#' + rmaMainViewCachData.guid + 'DataRowEdit').find('select[name = "' + ProductLineName + '"]');
    var ItemPK = $('#' + data.guid + 'DataRowEdit').attr("pk");
    var MfgDateInput = dialog.find('[name="' + MfgDateName + '"]');

    var researchId = 'researchId';

    var researchButton = $('#' + researchId);
    if (researchButton.length == 0) {
        var td = TopMarkingInput.parent('td:first');
        
        researchButton = $('<input id="' + researchId + '" type="button" value="Re-Search" />');
        td.append(researchButton);
        
        if (TopMarkingInput.val() == '') {
            researchButton.attr('disabled', 'disabled');
        }
        else {
            researchButton.removeAttr('disabled');
        }
        researchButton.click(function() {
            LoadParentPart(TopMarkingInput, ParentInput, SKUInput, ItemPK, ProductLineSelect, MfgDateInput, guid);
        });
    }

    TopMarkingInput.change(function() {
        LoadParentPart(TopMarkingInput, ParentInput, SKUInput, ItemPK, ProductLineSelect, MfgDateInput, guid);
        if (TopMarkingInput.val() == '') {
            researchButton.attr('disabled', 'disabled');
        }
        else {
            researchButton.removeAttr('disabled');
        }
    });

    }
   
    
function LoadParentPart(TopMarkingInput, ParentInput,SKUInput,ItemPK,ProductLineSelect,mfgDateInput, guid) {
    var topMarkingVal = TopMarkingInput.val();
    var ProductLineVal = ProductLineSelect.val();
    if (IsValidTopMarking(topMarkingVal)) {
          showProgress();
          setTimeout(function() {
          //var _54_90 =
          GetParentPart(views[guid].ViewName, topMarkingVal, SKUInput.val(), ItemPK, ProductLineVal,ParentInput,mfgDateInput, guid);
//              if (_54_90.success==false) {
//                  modalErrorMsg(_54_90.message);
//                  //SetAutoCompleteValueId(ParentInput, "", "");
//              }
//              else {
//                  //var scalar = ParentPart.ParentPartId;

//              }
              hideProgress();
          }, 1); 
     }
}


function IsValidTopMarking(TopMarkingSelectVal) {
    if (TopMarkingSelectVal.toString().length == 0)
        return false;
    if (TopMarkingSelectVal.toString().length <4) {
        modalErrorMsg("Invalid Top Marking, Please enter valid Top Marking.")
        return false;
    }
    return true;

}

function isItem(guid) {
    return views[guid].ViewName.toLowerCase().startsWith("v_item");
}

       
function GetParentPart(viewName, TopMarkingVal,SKUval,ItemPK,ProductLineVal,ParentInput,MfgDateInput, guid) {
    var ParentPart = {success: false, value:null};
    var mfgDate = { success: false, value: null };
 
    url = rootPath + views[guid].Controller + '/GetParentPart/' + viewName;

    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: viewName, topMarking: TopMarkingVal, SKU: SKUval, ItemPK: ItemPK, ProductLine: ProductLineVal },
        async: false,
        dataType: 'json',
        cache: false,
        error: function(xhr, status, error) {
            modalErrorMsg(error);
        },
        success: function(json) {
            if (json.ParentPartName == undefined || json.mfgDate == undefined) {
                var index = json.indexOf("$$error$$", 0);
                if (index >= 0 && index <= 1000) {
                    modalErrorMsg(json.replace("$$error$$", ""));
                }
                else {
                    modalErrorMsg(" Error Please contact administrator.");
                }

            }
            else {
                var message = '';
                
                if (json.ParentPartName != '') {
                    ParentPart.value = json.ParentPartName;
                    SetAutoCompleteValueId(ParentInput, ParentPart.value, ParentPart.value);
                }
                else {
                    message = "Failed to find Part#.";
                 
                }
                if (json.mfgDate != '') {
                    mfgDate.value = json.mfgDate;
                    var dateParts = mfgDate.value.match(/(\d+)/g);
                    var realDate = new Date(dateParts[0], dateParts[1] - 1, dateParts[2]);
                    MfgDateInput.datepicker("setDate", realDate);
                }
                else {
                    message = message +((message=='')?'':"<br>") +"Failed to retrieve Mfg Date. "

                }
                if (message != '') { modalErrorMsg(message); }
            }

        }
    });
  //  return ParentPart;
}

function setExternalDefaults(dialog) {
    var internalCategoryName = "FK_FailureCategory_v_FailureIntFailureCategoryId_Parent";
    var internalSubCategoryName = "FK_FailureSubCategory_v_FailureIntFailureSubCategoryId_Parent";
    var internalFailureDescriptionName = "IntFailureDescription";
    var internalRootCauseRemarksName = "IntRootCauseRemarks";
    var internalFailureAnalysisResultName = "IntFailureAnalysisResult";
    var externalCategoryName = "FK_FailureCategory_v_Failure_Parent";
    var externalSubCategoryName = "FK_FailureSubCategory_v_Failure_Parent";
    var externalFailureDescriptionName = "FailureDescription";
    var externalRootCauseRemarksName = "RootCauseRemarks";
    var externalFailureAnalysisResultName = "FailureAnalysisResult";
    
    var internalCategory = dialog.find("[name='" + internalCategoryName + "']");
    var internalSubCategory = dialog.find("[name='" + internalSubCategoryName + "']");
    var internalFailureDescription = dialog.find("[name='" + internalFailureDescriptionName + "']");
    var internalRootCauseRemarks = dialog.find("[name='" + internalRootCauseRemarksName + "']");
    var internalFailureAnalysisResult = dialog.find("[name='" + internalFailureAnalysisResultName + "']");
    var externalCategory = dialog.find("[name='" + externalCategoryName + "']");
    var externalSubCategory = dialog.find("[name='" + externalSubCategoryName + "']");
    var externalFailureDescription = dialog.find("[name='" + externalFailureDescriptionName + "']");
    var externalRootCauseRemarks = dialog.find("[name='" + externalRootCauseRemarksName + "']");
    var externalFailureAnalysisResult = dialog.find("[name='" + externalFailureAnalysisResultName + "']");

    internalCategory.change(function() {
        externalCategory.val(internalCategory.val());
        externalCategory.change();
    });

    internalSubCategory.change(function() {
        externalSubCategory.val(internalSubCategory.val());
    });

    internalFailureDescription.change(function() {
        externalFailureDescription.val(internalFailureDescription.val());
    });

    internalRootCauseRemarks.change(function() {
        externalRootCauseRemarks.val(internalRootCauseRemarks.val());
    });

    internalFailureAnalysisResult.change(function() {
        externalFailureAnalysisResult.val(internalFailureAnalysisResult.val());
    });
}

function isRma(guid) {
    return views[guid].ViewName.toLowerCase().startsWith("v_rma");
}

function LoadDefaultLocation(pruductSelect, faLocationSelect, guid) {
    var productID = pruductSelect.val();
    if (productID != '') {
        var faLocationId = GetProductLocation(productID, guid);
        if (faLocationId != null && faLocationId != '' && faLocationSelect.val() == '') {
            faLocationSelect.val(faLocationId);
        }
    }

}

// replace after integration
function GetProductLocation(productID, guid) {
    if (productID + '' == '3') {
        return '1';
    }
    return '2';
}

$(Durados.View).bind('edit', function(e, data) {

    if (isRma(data.guid)) {
        disableDevice(data.dialog);

        bindSapNoChange(data);

        disableSapRequestIfChecked(data.dialog);

        rmaMainViewCachData.guid = data.guid;

    }
    hideReportId(data);

    if (views[data.guid].ViewName == "v_Location") {
        var trackingFieldName = "TrackingNumberToFaLocation";
        var trackingElement = data.dialog.find('[name="' + trackingFieldName + '"]');

        var shippedFieldName = "DateShippedToFaLocation";
        var shippedElement = data.dialog.find('[name="' + shippedFieldName + '"]');
        trackingElement.change(function() {
            if ((shippedElement.val() == null || shippedElement.val() == '') && trackingElement.val() != '') {
                shippedElement.datepicker("setDate", new Date());
            }
        });
    }

    if (data.viewName == "v_Failure") {
        setExternalDefaults(data.dialog)
    }
    if (isItem(data.guid)) {
        SetParentPart(data,data.dialog, data.guid);
    }
});

$(AddDialog).bind('onafterAdd', function(e, data) {
    if (!data.close) {
        AddDialog.Close(data.guid);
        EditDialog.Open(data.pk, data.guid, false);
    }
});

$(AddDialog).bind('newButton', function(e, data) {
    if (isRma(data.guid)) {
        data.hide = true;
    }
});

$(AddDialog).bind('promoteButton', function(e, data) {
    //if (isRma(data.guid)) {
        data.hide = true;
    //}
});

$(EditDialog).bind('promoteButton', function(e, data) {
    data.hide = true;
});

var sapNoId = '';

function bindSapNoChange(data) {
    $(Autocomplete).bind('result', function(event, item) {
        var name = item.element.Element.attr('name');
        if (name == 'FK_v_sap_RMA_v_Rma_Parent') {
            var pk = item.element.PK;
            if (pk == sapNoId)
                return;
            else
                sapNoId = pk;
            var device = data.dialog.find('input[name="FK_agile_Part_v_Rma_Parent"]');
            if (pk != '' && pk != null) {
                showProgress();
                setTimeout(function() {
                    var partCode = GetSapPartCode(pk, data.guid);
                    if (device.length == 1) {
                        SetAutoCompleteValueId(device, partCode, partCode);
                        device.attr('disabled', 'disabled');
                        hideProgress();

                    }
                }, 1);
            }
            else {
                device.removeAttr('disabled');
            }
        }
    });


    $(SearchDialog).bind('afterSearchSelected', function(e, data2) {
        if (data2.fieldName == 'FK_v_sap_RMA_v_Rma_Parent') {
            var device = data.dialog.find('input[name="FK_agile_Part_v_Rma_Parent"]');
            if (data2.pk != null && data2.pk != '') {
                var pk = data2.pk;
                showProgress();
                setTimeout(function() {
                    var partCode = GetSapPartCode(pk, data.guid);
                    if (device.length == 1) {
                        SetAutoCompleteValueId(device, partCode, partCode);
                        device.attr('disabled', 'disabled');
                        hideProgress();

                    }
                }, 1);
            }
            else {
                device.removeAttr('disabled');
            }
        }
    });

    var sapNo = data.dialog.find('input[name="FK_v_sap_RMA_v_Rma_Parent"]');
    
    if (sapNo.length == 1) {
        sapNo.blur(function() {
            if (sapNo.val() == '') {
                var device = data.dialog.find('input[name="FK_agile_Part_v_Rma_Parent"]');
                if (device.length == 1) {
                    device.removeAttr('disabled');
                }
            }
        });
    }
}

function disableDevice(dialog) {
    var device = dialog.find('input[name="FK_agile_Part_v_Rma_Parent"]');
    var sapNo = dialog.find('input[name="FK_v_sap_RMA_v_Rma_Parent"]');
    if (sapNo.length == 1 && device.length == 1) {
        if (GetAutoCompleteValueId(sapNo) != '') {
            device.attr('disabled', 'disabled');
        }
        else {
            device.removeAttr('disabled');
        }
    }
}

function disableSapRequestIfChecked(dialog) {
    var sapRequestFieldName = "IsRequestSAPNum";

    var sapRequest = dialog.find('input[name="' + sapRequestFieldName + '"]');
    if (sapRequest.is(':checked')) {
        sapRequest.attr('disabled', 'disabled');
    }
    else {
        sapRequest.removeAttr('disabled');
    }
}

function GetInitiatorCountry(userID, guid) {
    return Data.GetScalar('v_User', userID, 'CountryId', guid);
}

function GetSapPartCode(sapNo, guid) {
    return Data.GetScalar('v_sap_RMA', sapNo, 'PartCode', guid);
}

$(Durados.View).bind('anotherRowFunction', function(e, data) {
    if (data.pks.length != 1) {
        modalErrorMsg('Please select a row'); return;
    }
    if (views[data.guid].ViewName == 'v_Report') {
        if (isInitiator(views[data.guid].ViewName, data.pks[0], data.guid)) {
            Send.ShowCustom(data.pks[0], views[data.guid].ViewName, data.guid);
        }
        else {
            modalErrorMsg('Only the initiator can send the report to the customer.'); return;
        }
    }
});

function isInitiator(viewName, pk, guid) {
    var scalar = null;
    url = rootPath + views[guid].Controller + '/IsInitiator/' + viewName;
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: viewName, pk: pk },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function(json) {
            scalar = json;
        }

    });

    return scalar;
}

$(document).ready(function() {


    var rec = Rectangle.Load(Send.DialogName());
    if (rec != null) {
        $("#Div1").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            position: [rec.left, rec.top],
            width: rec.width,
            height: rec.height,
            resizeStop: Send.SaveDialogOnResize,
            dragStop: Send.SaveDialogOnDrag
        });
    }
    else {
        $("#Div1").dialog({
            bgiframe: true,
            autoOpen: false,
            modal: true,
            width: 660,
            height: 540,
            resizeStop: Send.SaveDialogOnResize,
            dragStop: Send.SaveDialogOnDrag,
            position: 'center'
        });
    }
});


function hideReportId(data) {
    if (views[data.guid].ViewName == "v_Report") {
        var reportId = data.dialog.find('input[name="ReportId"]');
        var td = reportId.parents('td:first');
        var prev = td.prev();
        td.hide();
        prev.hide();
    }
}

$(Durados.View).bind('initDataTableView', function(e, data) {
    if (views[data.guid].ViewName == "v_RmaContact") {
        var a = $('#' + data.guid + 'ajaxDiv').find('a[title="Add new row"]');
        a.hide();
    }
});

// enable contacts add items
$(AddItemsDialog).bind('beforeAddItems', function(e, data) {
    if (data.viewName = 'v_Contact') {
        if (data.filterVal == '') {
            data.cancel = true;
            ajaxNotSuccessMsg('Please enter a customer.');
            return;
        }

        data.searchUrl = data.searchUrl.replace('&disabled=true', '');
    }
});

$(Durados.View).bind('initDataTableView', function(e, data) {
    if (views[data.guid].ViewName == 'v_RmaContact') {
        var div = $('#' + data.guid + 'ajaxDiv').find('a[title="Add Items"]').text('Add Contacts');
    }
});