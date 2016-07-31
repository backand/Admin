$(EditDialog).bind('onafterEditUpdate', function (e, data) {
    //if (parent && parent.refreshWidget2)
    //parent.refreshWidget2();

});

$(Durados.View).bind('edit', function (e, data) {
    data.dialog.parent('div:first').find('.ui-dialog-titlebar:first').find('a').remove();

    webMasterAccess.init(data.guid, data.dialog);
});


$(InlineEditingDialog).bind('beforeUpdatePreview', function (e, data) {
    //    $('button.ui-button').each(function () {
    //        if ($(this).text() == translator.save) {
    //            $(this).click();
    //            return;
    //        }
    //    });
});

$(Durados.View).bind('setPlan', function (e, data) {
    parent.upgrade();
});

/************************************************************************************/
/*		webMasterAccess (by br)				
/*		Provide functionalitty for webMasterAccess issue
/************************************************************************************/
var webMasterAccess = {
    /************************************************************************************/
    /*		getWebMasterAccessDetails (by br)				
    /*		Get details of current WebMasterAccess
    /************************************************************************************/
    getWebMasterAccessDetails: function (guid, viewPk, callback) {
        $.ajax({
            url: "/" + PlugInPrefix + "PlugIn/GetWebMasterAccessDetails/?" + jsonModel.parameters,
            contentType: 'application/json; charset=utf-8',
            data: { viewPk: viewPk },
            async: false,
            dataType: 'json',
            cache: false,
            success: function (data) {
                if (callback) {
                    callback(data);
                }
            }
        });
    },
    /************************************************************************************/
    /*		setWebMasterAccess (by br)				
    /*		Set WebMasterAccess- Assign new WebMasterAccess or change password
    /************************************************************************************/
    setWebMasterAccess: function (password, urlElement, guid, viewPk) {
        $.ajax({
            url: "/" + PlugInPrefix + "PlugIn/SetWebMasterAccess?" + jsonModel.parameters,
            contentType: 'application/json; charset=utf-8',
            data: { password: password, viewPk: viewPk },
            async: false,
            dataType: 'json',
            cache: false,
            success: function (data) {
                if (data != null) {
                    urlElement.text(data);
                    urlElement.attr('href', data);
                    urlElement.parent().parent().show();
                }
            }
        });
    },
    /************************************************************************************/
    /*		setProtectionValue (by br)				
    /*		Set value in protection element by indication if hasWebMasterAccess 
    /************************************************************************************/
    setProtectionValue: function (input, hyperlink, hasWebMasterAccess) {
        var hyperlinkText;
        var checkBoxValue;

        if (hasWebMasterAccess) {
            checkBoxValue = true;
            hyperlinkText = 'Settings';
        }
        else {
            checkBoxValue = false;
            hyperlinkText = 'Activate';
        }
        Durados.CheckBox.SetChecked(input, checkBoxValue);
        hyperlink.text(hyperlinkText);
    },
    /************************************************************************************/
    /*		openDialog (by br)				
    /*		Open protection dialog with current web master access details
    /************************************************************************************/
    openDialog: function (targetElement, guid, viewPk) {

        webMasterAccess.getWebMasterAccessDetails(guid, viewPk, function (webMasterAccessDetails) {

            var webMasterUrl = webMasterAccessDetails == null ? '' : webMasterAccessDetails.Url;
            var password = webMasterAccessDetails == null ? '' : webMasterAccessDetails.Password;

            var url = $('<a/>').attr('target', '_blank');
            var urlDiv = $('<div/>').addClass('webMasterUrl').append('Web Master Url: ').append($('<div/>').append(url));

            if (webMasterUrl != null && webMasterUrl.length > 0) {
                url.text(webMasterUrl);
                url.attr('href', webMasterUrl);
            }
            else {
                urlDiv.css('display', 'none');
            }

            var inputPassword = $('<input type="text"/>').val(password);
            var inputPasswordDiv = $('<div/>').append('Password: ').append(inputPassword);

            var html = $('<div/>').append(inputPasswordDiv).append(urlDiv);
            var title = translator.WebMasterAccess
            var dialog;

            var buttons = {};
            buttons[translator.save] = function () {
                var password = inputPassword.val();
                webMasterAccess.setWebMasterAccess(password, url, guid, viewPk);

            }
            buttons[translator.close] = function () {
                $(this).dialog("close");
            }

            var closeCallback = function () {
                var input = targetElement.find('.passwordProtection-input input');
                var hyperlink = targetElement.find('.passwordProtection-hyperlink a');
                var hasProtection = hyperlink.is(':visible');

                webMasterAccess.setProtectionValue(input, hyperlink, hasProtection);

            }

            dialog = Durados.Dialogs.general(title, html, buttons, closeCallback, 'passwordProtectionDialog');
        });
    },
    /************************************************************************************/
    /*		setEditEnabled (by br)				
    /*		Set WebMasterAccess element display by its state (enable or disable)
    /************************************************************************************/
    setEditEnabled: function (editEnabled, upgradePlanContent, input, hyperlink, newElement, guid, viewPk) {
        if (editEnabled) {
            input.change(
                    function () {
                        if ($(this).is(':checked')) {
                            webMasterAccess.openDialog(newElement, guid, viewPk);
                        }
                    }
                );

            hyperlink.click(function () {
                webMasterAccess.openDialog(newElement, guid, viewPk);
            })
        }
        else {
            hyperlink.parent().hide();
            input.prop('disabled', true);
            newElement.append(upgradePlanContent);
            //  newElement.append($('<div/>').addClass('passwordProtection-hyperlink').append(upgradePlanContent));
        }
    },
    /************************************************************************************/
    /*		addWebMasterAccessElement (by br)				
    /*		Add to view settings accordion element of web master access
    /************************************************************************************/
    addWebMasterAccessElement: function (webMasterAccessDetails, guid, dialog, viewPk) {
        var firstCategoryLastRow = dialog.find('.ui-accordion-content:first table:first tbody:first >tr:last');

        if (!firstCategoryLastRow.find('.passwordProtection').length) {
            var editEnabled = webMasterAccessDetails == null ? false : webMasterAccessDetails.PlanId != 3;
            var webMasterUrl = webMasterAccessDetails == null ? '' : webMasterAccessDetails.Url;
            var password = webMasterAccessDetails == null ? '' : webMasterAccessDetails.Password;
            var upgradePlanContent = webMasterAccessDetails == null ? '' : webMasterAccessDetails.UpgradePlanContent;

            var newRow = firstCategoryLastRow.clone();
            var newTd = newRow.find('table td').attr('id', '').empty();
            var newElement;
            var input = $('<input type="checkbox" safari="1"/>');
            var hyperlink = $('<a></a>');

            newElement = $('<div/>').addClass('passwordProtection')
                .append($('<div/>').addClass('passwordProtection-input').append(input).append($('<span/>').addClass('passwordProtection-label').text(translator.WebMasterAccess)))
                .append($('<div/>').addClass('passwordProtection-hyperlink').append(hyperlink));

            newRow.find('td:first span:first').remove();
            newTd.append(newElement);
            firstCategoryLastRow.after(newRow);
            input.checkbox({ cls: 'jquery-safari-checkbox' });

            webMasterAccess.setProtectionValue(input, hyperlink, webMasterUrl != null && webMasterUrl.length > 0);
            webMasterAccess.setEditEnabled(editEnabled, upgradePlanContent, input, hyperlink, newElement, guid, viewPk);
        }
    },
    /************************************************************************************/
    /*		init (by br)				
    /*		init web master access display and functionality
    /************************************************************************************/
    init: function (guid, dialog) {
        var isViewItem = guid != null && views[guid] != null && views[guid].ViewName == 'View' && views[guid].Role == "View Owner";

        if (isViewItem) {
            var viewPk = dialog.attr('pk');
            webMasterAccess.getWebMasterAccessDetails(guid, viewPk, function (webMasterAccessDetails) {
                webMasterAccess.addWebMasterAccessElement(webMasterAccessDetails, guid, dialog, viewPk);
            });
        }
    }

}
    