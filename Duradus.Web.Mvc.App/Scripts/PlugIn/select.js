var Select; if (!Select) Select = {};

$(document).ready(function () {
    Select.init();
});

Select.init = function () {

    var closeElement = $('input[name="close"]');
    var appsElement = $('[name="apps"]');
    var viewsElement = $('[name="views"]');
    var iframeElement = $('iframe');

    resize(iframeElement);
    $(window).resize(function () {
        resize(iframeElement);
    });

    if (appsElement.length == 1) {
        Select.loadApps(appsElement);
        Select.bindApps(appsElement, viewsElement);
    }

    if (viewsElement.length == 1) {
        Select.bindViews(appsElement, viewsElement, iframeElement);
    }

    Select.bindClose(appsElement, viewsElement, closeElement, iframeElement);
}


Select.bindClose = function (appsElement, viewsElement, closeElement, iframeElement) {
    closeElement.click(function () {
        var appId = appsElement.val();
        var viewName = viewsElement.val();
        if (!(viewName == null || viewName == '' || appId == null || appId == '')) {
            saveNewSelection(appId, viewName, iframeElement);
            window.opener.refreshWidget();
            window.opener.refreshAppsAndViews();
        }
        window.close();
    });
}

Select.loadApps = function (appsElement) {
    loadSelect(appsElement, $(jsonModel.apps));
}

Select.bindApps = function (appsElement, viewsElement) {
    appsElement.change(function () {
        var appId = appsElement.val();
        Select.loadViews(viewsElement, appId);
    });
}

Select.bindViews = function (appsElement, viewsElement, iframeElement) {
    viewsElement.change(function () {
        var appId = appsElement.val();
        var viewName = viewsElement.val();
        if (viewName == null || viewName == '' || appId == null || appId == '')
            return;
        saveNewSelection(appId, viewName, iframeElement);
        
//        window.opener.refreshWidget();
//        window.opener.refreshAppsAndViews();
    });
}

saveNewSelection = function (appId, viewName, iframeElement) {
    $.ajax({
        url: "/" + PlugInPrefix + "PlugIn/SaveNewInstance?" + window.location.href.slice(window.location.href.indexOf('?') + 1),
        contentType: 'application/json; charset=utf-8',
        data: { appId: appId, viewName: viewName },
        async: false,
        dataType: 'json',
        cache: false,
        success: function () {
            //PlugIn.LoadAppsAndViews();
            var url = '/' + PlugInPrefix + 'PlugIn/Widget?' + window.location.href.slice(window.location.href.indexOf('?') + 1);
            iframeElement.attr('src', url);
            window.opener.refreshWidget();
            window.opener.refreshAppsAndViews();
        }
    });
}


Select.loadViews = function (viewsElement, appId) {
    $.ajax({
        url: "/" + PlugInPrefix + "PlugIn/GetViews",
        contentType: 'application/json; charset=utf-8',
        data: { appId: appId },
        async: false,
        dataType: 'json',
        cache: false,
        success: function (json) {
            loadSelect(viewsElement, json);
        }
    });
}


resize = function (iframeElement) {
    iframeElement.width($(window).width() - 50);
    iframeElement.height($(window).height() - 250);
    $('#iframeBlocker').height(iframeElement.height());
}

loadSelect = function (element, arr) {
    var options = '<option value=""></option>';
    for (var i = 0; i < arr.length; i++) {
        var option = arr[i];
        var selected = option.Selected ? 'selected="selected"' : '';
        options += '<option value="' + option.Value + '" ' + selected + ' >' + option.Text + '</option>';
    }
    element.html(options);
}