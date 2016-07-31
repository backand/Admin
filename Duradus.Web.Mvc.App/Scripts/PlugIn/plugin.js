
$(document).ready(function () {
    PlugIn.init();
});

var PlugIn; if (!PlugIn) PlugIn = {};

PlugIn.init = function () {
    var editDataElement = $('[name="editData"]');
    var myAccountElement = $('[name="myAccount"]');
    var upgradeElement = $('[name="upgrade"]');
    var logoffElement = $('[name="logoff"]');
    var appsAndViewsElement = $('[name="appsAndViews"]');
    var iframeElement = $('iframe');
    var excelImportElement = $('img[name="excelImport"]');

    PlugIn.bindEditData(editDataElement);
    PlugIn.bindMyAccount(myAccountElement);
    PlugIn.bindUpgradeElement(upgradeElement);
    PlugIn.bindLogoff(logoffElement);
    PlugIn.bindAppsAndViews(appsAndViewsElement, iframeElement);
    PlugIn.LoadAppsAndViews(appsAndViewsElement);
    PlugIn.bindExcelImport(excelImportElement);

    appsAndViewsElement.change();

    $(window).unload(function () {
        if (editDataWindow && !editDataWindow.closed) editDataWindow.close()
        if (selectWindow && !selectWindow.closed) selectWindow.close()
    });

    resize(iframeElement);

    //    Wix.currentMember(function (memberDetails) {
    //        //        alert(memberDetails.name + ',' + memberDetails.email + ',' + memberDetails.id + ',' + memberDetails.owner);
    //    });

    Wix.getSiteInfo(function (siteInfo) {
        if (siteInfo) {
            PlugIn.updateSiteInfo(PlugIn.EncodeSiteInfo(siteInfo));
            //alert(siteInfo.siteTitle + ',' + siteInfo.pageTitle + ',' + siteInfo.siteDescription + ',' + siteInfo.siteKeywords + ',' + siteInfo.referrer + ',' + siteInfo.url + ',' + siteInfo.baseUrl);
        }
    });
}

PlugIn.EncodeSiteInfo = function (siteInfo) {
    siteInfo.siteTitle = encodeURIComponent(siteInfo.siteTitle);
    siteInfo.pageTitle = encodeURIComponent(siteInfo.pageTitle);
    siteInfo.siteDescription = encodeURIComponent(siteInfo.siteDescription);
    siteInfo.siteKeywords = encodeURIComponent(siteInfo.siteKeywords);
    siteInfo.referrer = encodeURIComponent(siteInfo.referrer)
    siteInfo.url = encodeURIComponent(siteInfo.url);
    siteInfo.baseUrl = encodeURIComponent(siteInfo.baseUrl);
    return siteInfo;
}

PlugIn.updateSiteInfo = function (siteInfo) {
    $.ajax({
        url: "/" + PlugInPrefix + "PlugIn/UpdateSiteInfo?" + jsonModel.parameters,
        contentType: 'application/json; charset=utf-8',
        data: siteInfo,
        async: true,
        dataType: 'json',
        cache: false,
        success: function () {

        }
    });
}

PlugIn.bindUpgradeElement = function (upgradeElement) {
    upgradeElement.click(function () {
        upgrade();
    });
}

saveChangeSelection = function (sampleAppId) {
    $.ajax({
        url: "/" + PlugInPrefix + "PlugIn/SaveChangeInstance?" + jsonModel.parameters,
        contentType: 'application/json; charset=utf-8',
        data: { sampleAppId: sampleAppId },
        async: false,
        dataType: 'json',
        cache: false,
        success: function () {
            
        }
    });
}

PlugIn.bindLogoff = function (logoffElement) {
    logoffElement.click(function () {
        $.ajax({
            url: "/" + PlugInPrefix + "PlugIn/Logoff?" + jsonModel.parameters,
            contentType: 'application/json; charset=utf-8',
            data: {},
            async: false,
            dataType: 'json',
            cache: false,
            success: function (json) {
                refreshWidget();
                refreshAppsAndViews();
            }
        });
    });
}

function refreshAppsAndViews() {
    //PlugIn.LoadAppsAndViews();
    window.location = window.location;
}

PlugIn.LoadAppsAndViews = function (appsAndViewsElement) {
    if (appsAndViewsElement == null)
        appsAndViewsElement = $('[name="appsAndViews"]');

    $.ajax({
        url: "/" + PlugInPrefix + "PlugIn/GetSampleViews?" + jsonModel.parameters,
        contentType: 'application/json; charset=utf-8',
        data: {  },
        async: false,
        dataType: 'json',
        cache: false,
        success: function (json) {
            loadSelect(appsAndViewsElement, json);
            appsAndViewsElement.children('option[value="-"]').attr('disabled', true);
        }
    });
}

PlugIn.bindEditData = function (editDataElement) {
    editDataElement.click(function () {
        PlugIn.openUpdate();
    });
}

PlugIn.bindMyAccount = function (myAccountElement) {
    myAccountElement.click(function () {
        PlugIn.openSelect();
    });
}

PlugIn.bindExcelImport = function (excelImportElement) {
    excelImportElement.click(function () {
        PlugIn.openImport();
    });
}

function refreshWidget2() {
    refreshWidget();
};


PlugIn.bindAppsAndViews = function (appsAndViewsElement, iframeElement) {
    appsAndViewsElement.change(function () {
        var selection = appsAndViewsElement.val();
        if (selection != "-") {
            saveChangeSelection(selection);
            var url = '/' + PlugInPrefix + 'PlugIn/Design?' + jsonModel.parameters;
            iframeElement.attr('src', url);
            refreshWidget();
        }
    });
}

var editDataWindow = null;
var selectWindow = null;

PlugIn.openUpdate = function () {
    var url = '/' + PlugInPrefix + 'PlugIn/Update?' + jsonModel.parameters;
    editDataWindow = window.open(url, 'EditData', 'width=1450,height=820');
    
}

PlugIn.openSelect = function () {
    //var url = '/' + PlugInPrefix + 'PlugIn/Connect?' + jsonModel.parameters;
    //selectWindow = window.open(url, 'EditData', 'width=1200,height=720');
    selectWindow = window.open('/Account/RegistrationRequest?returnUrl=/WixPlugIn/AfterRegistration&' + jsonModel.parameters, 'BackAnd', 'width=1450,height=820');
    //    selectWindow.unload(function () {
    //        refreshAppsAndViews();
    //    });
}

loadSelect = function (element, arr) {
    var options = '';// '<option value=""></option>';
    for (var i = 0; i < arr.length; i++) {
        var option = arr[i];
        var selected = option.Selected ? 'selected="selected"' : '';
        options += '<option value="' + option.Value + '" ' + selected + ' >' + option.Text + '</option>';
    }
    element.html(options);
}

PlugIn.openImport = function () {
    if ($('#settingIframe')[0].contentWindow && $('#settingIframe')[0].contentWindow.duradosImport) {
        
        var viewName = null;
        $.ajax({
            url: "/" + PlugInPrefix + "PlugIn/GetViewName?" + jsonModel.parameters,
            contentType: 'application/json; charset=utf-8',
            async: false,
            dataType: 'text',
            cache: false,
            success: function (view) {
                viewName = view;
            }
        });

        $('#settingIframe')[0].contentWindow.showProgress();
        $('#settingIframe')[0].contentWindow.duradosImport(viewName, function () { refreshWidget2(); });
        $('#settingIframe')[0].contentWindow.hideProgress();
    }

}

resize = function (iframeElement) {
    iframeElement.height($(window).height() - 250);
    //iframeElement.width($(window).width() - 50);
    // iframeElement.height($(window).height() - 250);
}

function enableImport() {
    $('iframe#settingIframe').load(function () {
       // alert('111');
        var excelImportElement = $('img[name="excelImport"]');
        excelImportElement.addClass('excelImportImageEnabled');
        excelImportElement.attr("src", '../../../Content/Images/Excel-icon.png');
    });

    
}