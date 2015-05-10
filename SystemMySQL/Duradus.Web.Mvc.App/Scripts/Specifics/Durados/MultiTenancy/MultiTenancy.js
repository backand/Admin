var defaultProd = "4";

$(Durados.View).bind('refreshGrid', function (e, data) {
    var basicButton = addCreateBasicAppButton(data);
    defaultProd = getDefaultProd();

    $(AddDialog).bind('autoOpenAdd', function (e, data) {
        data.cancel = true;
        basicButton.click();
    });

});

getDefaultProd = function () {
    return defaultProd;
//    try {
//        var firstRow = $('.boardrow:first');


//        if (firstRow.length != 1) {
//            return defaultProd;
//        }

//        if (firstRow.html().toLowerCase().indexOf('northwind') != -1) {
//            return defaultProd;
//        }

//        var curUrl = document.location.href;
//        var curPort = curUrl.split(':')[2].split('/')[0];

//        var lastAppUrl = firstRow.find('a[name="Url"]');
//        if (lastAppUrl.length != 1)
//            return defaultProd;

//        var appPort = lastAppUrl.attr('href').split(':')[2].split('?')[0];

//        if (appPort == curPort)
//            return "2";
//        else
//            return defaultProd;
//    }
//    catch (err) {
//        return defaultProd;
//    }
}

var basicClicked = false;
var addButton = null;
var basicButton = null;
var d_add = null;

function addCreateBasicAppButton(data) {
    var toolbar = $('.table-menu:first');
    if (toolbar.length != 1) return;
    var group = toolbar.find('.group-l:first');
    if (group.length != 1) return;

    addButton = group.find('a:first');
    
    basicButton = addButton.clone();
    var basicSpan = basicButton.find('span');

    basicButton.text(translator.CreateBasicApp);
    basicButton.append(basicSpan);

    basicButton.attr('onclick', 'basicClicked = true;'  + addButton.attr('onclick'));
    addButton.attr('onclick', 'basicClicked = false;' + addButton.attr('onclick'));

    group.prepend(basicButton);

    return basicButton;
}

$(CreateDialog).bind('onafterInlineAdding', function (e, data) {
    return;
    var viewName = "durados_App";
    var name = data.element.attr('name');


    var dbName = "FK_durados_App_durados_SqlConnection_Parent";
    var systemName = "FK_durados_App_durados_SqlConnection_System_Parent";
    var securityName = "FK_durados_App_durados_SqlConnection_Security_Parent";

    var dialog = data.element.parents('div:first');

    var dbSelect = dialog.find('select[name="' + dbName + '"]');
    var systemSelect = dialog.find('select[name="' + systemName + '"]');
    var securitySelect = dialog.find('select[name="' + securityName + '"]');

    if (name == dbName) {
        LoadSelect(viewName, systemName, systemSelect, data.guid);
        LoadSelect(viewName, securityName, securitySelect, data.guid);
    }

    if (name == systemName) {
        LoadSelect(viewName, dbName, dbSelect, data.guid);
        LoadSelect(viewName, securityName, securitySelect, data.guid);
    }

    if (name == securityName) {
        LoadSelect(viewName, dbName, dbSelect, data.guid);
        LoadSelect(viewName, securityName, securitySelect, data.guid);
    }
});

//$(CreateDialog).bind('dialogclose',
// function (event) {
//     basicOpen = false;
// });
function updateConnectionLists(guid) {
    var dbName = "FK_durados_App_durados_SqlConnection_Parent";
    var systemName = "FK_durados_App_durados_SqlConnection_System_Parent";
    var securityName = "FK_durados_App_durados_SqlConnection_Security_Parent";

    var viewName = "durados_App";

    var dbSelects = $('select[name="' + dbName + '"]');
    var systemSelects = $('select[name="' + systemName + '"]');
    var securitySelects = $('select[name="' + securityName + '"]');

    systemSelects.each(function () {
        var systemSelect = $(this);
        LoadSelect(viewName, systemName, systemSelect, guid);
    });
    securitySelects.each(function () {
        var securitySelect = $(this);
        LoadSelect(viewName, securityName, securitySelect, guid);
    });
    dbSelects.each(function () {
        var dbSelect = $(this);
        LoadSelect(viewName, dbName, dbSelect, guid);
    });

}

var basicOpen = false;
var templateOption = null;


function clearBasicValidation(data) {
    var validationElms = data.dialog.find(".sqlrequierd");

    $.each(validationElms, function () {
        if ($(this).next().attr('class') == "textfieldRequiredState")
            $(this).next().hide();
    });
    var dataSource = data.dialog.find('select[name="DataSource"]');
    if ($(dataSource).next().attr('class') == "textfieldRequiredState")
        $(dataSource).next().hide();


}
$(Durados.View).bind('add', function (e, data) {
    if (data.viewName != 'durados_App')
        return;

    if (g_hostByUs) {
        if (basicClicked) {
            window.location.href = '/connect'
        }
    }

    var basic = data.dialog.find('input[name="Basic"]');
    var title = data.dialog.find('input[name="Title"]');
    var template = data.dialog.find('select[name="FK_durados_App_durados_Template_Parent"]');
    var dataSourceType = data.dialog.find('select[name="FK_durados_App_durados_DataSourceType_Parent"]');
    var tab = data.dialog.find('div.ui-tabs');
    var lis = tab.find('li');

    //    var buttons = data.dialog.dialog("option", "buttons"); // getter
    //    if (buttons["Create Basic Console"] === undefined) {
    //        var extendbuttons = { "Create Basic Console": function () { ToggleCreateApp(data); } };
    //        $.extend(extendbuttons, buttons); //translator
    //        data.dialog.dialog('option', 'buttons', extendbuttons);
    //    }
    var saveButton = data.dialog.parent().find('button:containsExact("' + translator.save + '")');
    var saveAndCloseButton = data.dialog.parent().find('button:containsExact("' + translator.saveAndClose + '")');
    var newButton = data.dialog.parent().find('button:containsExact("' + translator.New + '")');
    //    var createBasicButton = data.dialog.parent().find('button:containsExact("Create Basic Console")');
    saveButton.hide();
    newButton.hide();
    var title = data.dialog.parent().find('span.ui-dialog-title');
    saveAndCloseButton.find('span').text("Create Console");

    basic.parent('td:first').parent('tr:first').css('display', 'none');
    var connFields = data.dialog.find("input.showOnlyInBasic");
    var connBasicFields = data.dialog.find("input.basicbasic");
    clearBasicValidation(data);

    var dataSource = data.dialog.find('select[name="DataSource"]');

    dataSource.unbind('change').bind('change', function () {
        productDependency($(this).val(), data.dialog);
    });

    var connUseSSH = data.dialog.find('input[name="UseSSH"]');
    connUseSSH.unbind('change').bind('change', function () {
        sshDependency(data.dialog);
    });

    //if (dataSource.val() == '' || dataSource.val() == null) {
    dataSource.val(defaultProd);
    productDependency(defaultProd, data.dialog);
    //}

    var connUseSSH = data.dialog.find('input[name="UseSSH"]');
    var databaseConn = data.dialog.find('select[name="FK_durados_App_durados_SqlConnection_Parent"]');
    var databaseConnRowSelector = 'td#the' + data.guid + 'create_durados_App_FK_durados_App_durados_SqlConnection_Parent';
    var appTitle = data.dialog.find('input[name="Title"]');

    if (basicClicked) {
        basicClicked = false;
        basicOpen = true;
        Durados.CheckBox.SetChecked(basic, true);
        basic.parent('td:first').parent('tr:first').css('display', 'none');
        title.parent('td:first').parent('tr:first').css('display', 'none');
        template.parent('td:first').parent('tr:first').css('display', 'none');
        dataSourceType.parent('td:first').parent('tr:first').css('display', 'none');
        appTitle.parent('td:first').parent('tr:first').css('display', 'none');
        templateOption = dataSourceType.find('option[value="4"]').remove();
        $(lis[1]).hide();
        $(lis[2]).hide();
        //        saveAndCloseButton.hide();
        //        createBasicButton.show()
        databaseConn.parents(databaseConnRowSelector).parent('tr:first').css('display', 'none');

        $.each(connBasicFields, function () {
            $(this).parent('td:first').parent('tr:first').css('display', '');

        });

        dataSource.parent('td:first').parent('tr:first').css('display', '');

        connUseSSH.parent('td:first').parent('tr:first').css('display', '');


        //title.text(basicButton.text());
    }
    else {
        basicClicked = false;
        basicOpen = false;
        Durados.CheckBox.SetChecked(basic, false);
        title.parent('td:first').parent('tr:first').css('display', '');
        basic.parent('td:first').parent('tr:first').css('display', '');
        template.parent('td:first').parent('tr:first').css('display', '');
        appTitle.parent('td:first').parent('tr:first').css('display', '');
        dataSourceType.parent('td:first').parent('tr:first').css('display', '');
        if (dataSourceType.find('option[value="4"]').length == 0) {
            dataSourceType.append(templateOption);
        }
        $(lis[1]).show();
        $(lis[2]).show();
        //        saveAndCloseButton.show();
        //        createBasicButton.hide();
        databaseConn.parents(databaseConnRowSelector).parent('tr:first').css('display', '');
        $.each(connFields, function () { $(this).parent('td:first').parent('tr:first').css('display', 'none'); });
        dataSource.parent('td:first').parent('tr:first').css('display', 'none');
        connUseSSH.parent('td:first').parent('tr:first').css('display', 'none');

        //title.text(addButton.text());

    }

    addHost(data.dialog);
    updateTitle(data.dialog);
    //ShowSourceId(data.dialog);

});


sshDependency = function (form) {
    var fieldsName = { template: "DataSource", name: "Name", title: "Title", server: "Server", catalog: "Catalog", username: "Username", password: "Password", usesSsh: "UseSSH", usesSsl: "UseSSL", sshRemoteHost: "SSHRemoteHost", productPort: "RemotePort", sshPort: "SSHPort", sshUsername: "SSHUsername", sshPassword: "SSHPassword", sshPrivateKey: "SshPrivateKey" }

    var usesSsh = form.find('input[name="' + fieldsName.usesSsh + '"]');
    var usesSsl = form.find('input[name="' + fieldsName.usesSsl + '"]');
    var sshRemoteHost = form.find('input[name="' + fieldsName.sshRemoteHost + '"]');
    var productPort = form.find('input[name="' + fieldsName.productPort + '"]');
    var sshPort = form.find('input[name="' + fieldsName.sshPort + '"]');
    var sshUsername = form.find('input[name="' + fieldsName.sshUsername + '"]');
    var sshPassword = form.find('input[name="' + fieldsName.sshPassword + '"]');
    var sshPrivateKey = form.find('input[name="' + fieldsName.sshPrivateKey + '"]');

    var b = usesSsh.prop('checked');

    if (b) {
        sshRemoteHost.parent('td:first').parent('tr:first').css('display', '');
        sshPort.parent('td:first').parent('tr:first').css('display', '');
        sshUsername.parent('td:first').parent('tr:first').css('display', '');
        sshPassword.parent('td:first').parent('tr:first').css('display', '');
        sshPrivateKey.parent('td:first').parent('tr:first').css('display', '');
        usesSsl.parent('td:first').parent('tr:first').css('display', '');

        sshRemoteHost.removeClass('sqlrequierd').addClass('sqlrequierd');
        sshPort.removeClass('sqlrequierd').addClass('sqlrequierd');
        sshUsername.removeClass('sqlrequierd').addClass('sqlrequierd');
        //sshPassword.removeClass('sqlrequierd').addClass('sqlrequierd');
    }
    else {
        sshRemoteHost.parent('td:first').parent('tr:first').css('display', 'none');
        sshPort.parent('td:first').parent('tr:first').css('display', 'none');
        sshUsername.parent('td:first').parent('tr:first').css('display', 'none');
        sshPassword.parent('td:first').parent('tr:first').css('display', 'none');
        sshPrivateKey.parent('td:first').parent('tr:first').css('display', 'none');
        usesSsl.parent('td:first').parent('tr:first').css('display', 'none');

        sshRemoteHost.removeClass('sqlrequierd');
        sshPort.removeClass('sqlrequierd');
        sshUsername.removeClass('sqlrequierd');
        //sshPassword.removeClass('sqlrequierd');
    }
}

productDependency = function (prod, form) {
    var fieldsName = { template: "DataSource", name: "Name", title: "Title", server: "Server", catalog: "Catalog", username: "Username", password: "Password", usesSsh: "UseSSH", usesSsl: "UseSSL", sshRemoteHost: "SSHRemoteHost", productPort: "RemotePort", sshPort: "SSHPort", sshUsername: "SSHUsername", sshPassword: "SSHPassword", sshPrivateKey: "SshPrivateKey" }

    var usesSsh = form.find('input[name="' + fieldsName.usesSsh + '"]');
    var usesSsl = form.find('input[name="' + fieldsName.usesSsl + '"]');
    var sshRemoteHost = form.find('input[name="' + fieldsName.sshRemoteHost + '"]');
    var productPort = form.find('input[name="' + fieldsName.productPort + '"]');
    var sshPort = form.find('input[name="' + fieldsName.sshPort + '"]');
    var sshUsername = form.find('input[name="' + fieldsName.sshUsername + '"]');
    var sshPassword = form.find('input[name="' + fieldsName.sshPassword + '"]');
    var sshPrivateKey = form.find('input[name="' + fieldsName.sshPrivateKey + '"]');

    switch (prod) {
        case "4": // MySQL
            usesSsh.parent('td:first').parent('tr:first').css('display', '');
            productPort.parent('td:first').parent('tr:first').css('display', '');

            usesSsh.removeClass('sqlrequierd').addClass('sqlrequierd');
            productPort.removeClass('sqlrequierd').addClass('sqlrequierd');

            sshDependency(form);

            break;

        default: // SQL Server
            usesSsh.parent('td:first').parent('tr:first').css('display', 'none');
            usesSsl.parent('td:first').parent('tr:first').css('display', 'none');
            sshRemoteHost.parent('td:first').parent('tr:first').css('display', 'none');
            productPort.parent('td:first').parent('tr:first').css('display', 'none');
            sshPort.parent('td:first').parent('tr:first').css('display', 'none');
            sshUsername.parent('td:first').parent('tr:first').css('display', 'none');
            sshPassword.parent('td:first').parent('tr:first').css('display', 'none');
            sshPrivateKey.parent('td:first').parent('tr:first').css('display', 'none');

            usesSsh.removeClass('sqlrequierd');
            sshRemoteHost.removeClass('sqlrequierd');
            productPort.removeClass('sqlrequierd');
            sshPort.removeClass('sqlrequierd');
            sshUsername.removeClass('sqlrequierd');
            sshPassword.removeClass('sqlrequierd');

            break;
    }
}

$(Durados.View).bind('edit', function (e, data) {
    addHost(data.dialog);
    HideSourceId(data.dialog);
    HideSqlConnectionInlines(data.dialog,data.guid);
});

function addHost(dialog) {
    var name = dialog.find('input[name="Name"]');
    if (name.next().attr('class') != 'd_host' && name.next().next().attr('class') != 'd_host')
        $('<span class="d_host">.' + d_host + '</span>').insertAfter(name);

}

function updateTitle(dialog) {
    var name = dialog.find('input[name="Name"]');
    name.blur(function () {
        var title = dialog.find('input[name="Title"]');
        if (title.val() == '') {
            title.val(name.val());
        }
    });
}

$(EditDialog).bind('onafterEditUpdate', function (e, data) {
    updateConnectionLists(data.guid);
});

var waitImgSrc = '/Website/img/addviewswait.gif';

$(document).ready(function () {
    preloader(waitImgSrc);
    var rowcount = $('table.gridview').attr('rowcount');
    var redirectToApp = queryString("redirectToApp");
    if (redirectToApp == "1") {
        if (rowcount == 1) {
            location.href = $('a[name=Url]:first').attr('href');
        }
    }
});

function preloader(imgSrc) {
    heavyImage = new Image();
    heavyImage.src = imgSrc;
}

$(AddDialog).bind('onafterAdd', function (e, data) {
    updateConnectionLists(data.guid);

    if (basicOpen) {
        basicOpen = false;
        showWaitImage(data);
//        setTimeout(function () {
//            navigateToNewApp(data);
//        }, 1000);
    }
});

function showWaitImage(data) {
    var image = $('<img>');
    var dialog = Durados.Dialogs.Wait(image, null, 1000, 600, true);
    image.attr('src', waitImgSrc)
    .load(function () {
        navigateToNewApp(data);
    });
}

function navigateToNewApp(data) {
    hideAjaxErrorMsg = true;
    if (basicOpen) {
        basicOpen = false;
        try {
            document.location.href = data.Url;
        }
        catch (err) { }
    }
//    else {
//        var pk = data.pk;
//        var url = Data.GetScalar(views[data.guid].ViewName, data.pk, 'Url', data.guid);
//        try {
//            document.location = url.split('|')[2];
//        }
//        catch (err) { }
//    }
}

function ShowSourceId(dialog) {
    var select = dialog.find('select[name="FK_durados_App_durados_DataSourceType_Parent"]');
    var tr = select.parents("tr:first").show();
}

function HideSourceId(dialog) {
    var select = dialog.find('select[name="FK_durados_App_durados_DataSourceType_Parent"]');
    var tr = select.parents("tr:first").hide();
}

function HideSqlConnectionInlines(dialog,guid) {

    // var select = dialog.find('select[name="FK_durados_App_durados_DataSourceType_Parent"]');
    var isRowViewOnly=dialog.parent().find("#ui-dialog-title-"+guid+"DataRowEdit").html().indexOf("View")==0
    if (isRowViewOnly) {
        var tr = dialog.find('#the' + guid + 'edit_durados_App_FK_durados_App_durados_SqlConnection_Parent');
        tr.find('span.Add-icon,span.Edit-icon,span.Duplicate-icon').hide();
        
    }
}
var AddBasicApp; if (!AddBasicApp) AddBasicApp = {};

AddBasicApp.getInfo = function(form, fieldsName) {
    var info = null;


    info = { template: form.find('select[name="' + fieldsName.template + '"]').val(),
        name: form.find('input[name="' + fieldsName.name + '"]').val(),
        title: form.find('input[name="' + fieldsName.title + '"]').val(),
        server: form.find('input[name="' + fieldsName.server + '"]').val(),
        catalog: form.find('input[name="' + fieldsName.catalog + '"]').val(),
        username: form.find('input[name="' + fieldsName.username + '"]').val(),
        password: form.find('input[name="' + fieldsName.password + '"]').val(),
        usesSsh: form.find('input[name="' + fieldsName.usesSsh + '"]').prop('checked'),
        sshRemoteHost: form.find('input[name="' + fieldsName.sshRemoteHost + '"]').val(),
        productPort: form.find('input[name="' + fieldsName.productPort + '"]').val(),
        sshPort: form.find('input[name="' + fieldsName.sshPort + '"]').val(),
        sshUsername: form.find('input[name="' + fieldsName.sshUsername + '"]').val(),
        sshPassword: form.find('input[name="' + fieldsName.sshPassword + '"]').val()
    }
    return info;

}

$(AddDialog).bind('onbeforeAdd', function (e, data) {
    if (basicOpen) {
        data.cancel = true;
        var fieldsName = { template: "DataSource", name: "Name", title: "Title", server: "Server", catalog: "Catalog", username: "Username", password: "Password", usesSsh: "UseSSH", sshRemoteHost: "SSHRemoteHost", productPort: "RemotePort", sshPort: "SSHPort", sshUsername: "SSHUsername", sshPassword: "SSHPassword" }
        submitBasicApp(data, fieldsName);
    }
});

function ToggleCreateApp(data) {

    if (basicOpen) {
        var fieldsName = { template: "DataSource", name: "Name", title: "Title", server: "Server", catalog: "Catalog", username: "Username", password: "Password", usesSsh: "UseSSH", sshRemoteHost: "SSHRemoteHost", productPort: "RemotePort", sshPort: "SSHPort", sshUsername: "SSHUsername", sshPassword: "SSHPassword" }
        submitBasicApp(data, fieldsName);
    }
    else {
        data.dialog.dialog('option', 'buttons')["Save and Close"]();
    }
}
AddBasicApp.validate = function (form, classer) {
    var isValid = true;

    form.find('.sqlrequierd').each(function () {//,textarea[requierd="true"]'
        if (!this.value) {
            classer.handleFieldError(this, this.name);
            form.parents('[role=dialog]').dialog('open');
            isValid = false;
        }
    });
    var dataSource = form.find('select[name="DataSource"]')[0];
    if (!dataSource.value) {
        classer.handleFieldError(dataSource, dataSource.name);
    }

    var appName = form.find('input[name="Name"]').val();
    if (appName && !isAlphanumeric(appName)) {

        classer.message('Invalid Application name, should be alphanumeric (no spaces or other chars).');

        isValid = false;
    }
    else {
        form.find('span.general').hide();
    }
    return isValid
    //alert(i);

}
AddBasicApp.handleFieldError = function (elm, name) {
    if ($(elm).next().attr('class') != 'textfieldRequiredState')
        $('<span d_label="' + $(elm)[0].name + '" class="textfieldRequiredState" style="display: none;color: #CC3333;border: 1px solid #CC3333;">Required Field</span>').insertAfter($(elm));
    $(elm).next().show();
}
AddBasicApp.message = function (msg, elm) {
    var errMsgElm = elm;
    if (elm == null) {
        errMsgElm = $("div#modal_errors");
    }
    $(errMsgElm).html(msg).show();
    $(errMsgElm).parents("[role=dialog]").dialog('open');
    $(errMsgElm).parents("[role=dialog]").show();

}
    
        



AddBasicApp.hideMessage = function (form) {
    form.find('div#modal_errors').hide();
    errMsgElm = $("div#modal_errors");
    $(errMsgElm).parents("[role=dialog]").dialog('close');
}
isAlphanumeric = function (appName) {
    var regexp = /^[a-zA-Z0-9]+$/
    return regexp.test(appName);
}

function submitBasicApp(data, fieldsName) {
    var dialog = data.dialog;
    clearBasicValidation(data);
    if (!AddBasicApp.validate(dialog.find('form:first'), AddBasicApp)) {
        return false;
    }
    AddBasicApp.hideMessage(dialog.find('form:first'));
    var info = AddBasicApp.getInfo(dialog.find('form:first'), fieldsName);
    var data = info; 

    var url = '/WebSite/CreateApp?template=' + info.template + '&name=' + info.name + '&title=' + /*info.title*/info.name + '&server=' + info.server + '&catalog=' + info.catalog + '&username=' + info.username + '&password=' + info.password + '&usingSsh=' + info.usesSsh + '&sshRemoteHost=' + info.sshRemoteHost + '&sshUser=' + info.sshUsername + '&sshPassword=' + info.sshPassword + '&sshPort=' + info.sshPort + '&productPort=' + info.productPort;
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        type: "POST",
        async: false,
        dataType: 'json',
        cache: false,
        error: function () { alert("error"); },
        success: function (response) {
            if (response.Success) {
                showWaitImage(response);
            }
            else {
                //                DivButton.Enable(div);
                AddBasicApp.message(response.Message)
                //preloader.stop();
            }
        }
    });

    return;


}

