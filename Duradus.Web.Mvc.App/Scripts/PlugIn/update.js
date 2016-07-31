var Update; if (!Update) Update = {};

$(document).ready(function () {
    Update.init();
});

Update.init = function () {

    var saveElement = $('span[name="update"]');
    var closeElement = $('span[name="close"]');
    var iframeElement = $('iframe');

    resize(iframeElement);
    $(window).resize(function () {
        resize(iframeElement);
    });

    closeElement.parent().css("margin-left", ($(window).width() - 50 - closeElement.parent().width() - saveElement.parent().width()) / 2);

    Update.bindSave(saveElement, false);
    Update.bindSave(closeElement, true);

    window.onbeforeunload = function () {
        try {
            window.opener.refreshWidget();
        }
        catch (err) {
        }
    }
}


Update.bindSave = function (saveElement, close) {
    saveElement.click(function () {
        window.opener.refreshWidget();
        if (close) {
            window.close();
        }
    });
}


resize = function (iframeElement) {
    iframeElement.width($(window).width() - 50);
    iframeElement.height($(window).height()- 10);
}

