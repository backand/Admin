var DuradosMail; if (!DuradosMail) DuradosMail = {};

DuradosMail.sendMailTo = function(to, cc, subject, body) {
    var iframe = $('<iframe>');
    iframe.attr('src', "about:blank");
    iframe.css('display', "none");
    $('body').append(iframe);
    //var href = "mailto:" + to + "?subject=" + subject + "&amp;body=" + body + "&cc=" + cc;
    var href = "mailto:" + ((to == null) ? "" : to) + "?subject=" + ((subject == null) ? "" : subject) + "&cc=" + ((cc == null) ? "" : cc);
    iframe[0].contentWindow.location.href = href;
    iframe.remove();
}

DuradosMail.sendActiveX = function(to, cc, subject, body, attachmentFileName, attachmentContents) {
    var outlookApp = new ActiveXObject("Outlook.Application");
    var nameSpace = outlookApp.getNameSpace("MAPI");
    var mailItem = outlookApp.CreateItem(0); //olMailItem
    mailItem.BodyFormat = 2; //html
    mailItem.To = (to == null) ? "" : to;
    mailItem.Cc = (cc == null) ? "" : cc;
    mailItem.Subject = (subject == null) ? "" : subject;
    mailItem.HTMLBody = (body == null) ? "" : body;
    if (attachmentContents != null && attachmentContents != "") {
        var filePath = "C:\\temp\\" + attachmentFileName;
        var fso = new ActiveXObject("Scripting.FileSystemObject");
        var newFile = fso.CreateTextFile(filePath, true);
        newFile.Write(attachmentContents);
        newFile.Close();

        //attach the file
        mailItem.Attachments.Add(filePath);
    }
    mailItem.Display(false);
}

DuradosMail.sendActiveX2 = function(to, cc, subject, body, attachmentFileName) {
    var outlookApp = new ActiveXObject("Outlook.Application");
    var nameSpace = outlookApp.getNameSpace("MAPI");
    var mailItem = outlookApp.CreateItem(0); //olMailItem
    mailItem.BodyFormat = 2; //html
    mailItem.To = (to == null) ? "" : to;
    mailItem.Cc = (cc == null) ? "" : cc;
    mailItem.Subject = (subject == null) ? "" : subject;
    mailItem.HTMLBody = (body == null) ? "" : body;
    if (attachmentFileName != null && attachmentFileName != "") {
        try {

            mailItem.Attachments.Add(attachmentFileName);
        }
        catch (e) {
            // catch the exception
        }
    }
    mailItem.Display(false);
}

DuradosMail.testForActiveX = function() {
    tester = null;
    try {
        tester = new ActiveXObject('Outlook.Application');
    }
    catch (e) {
        // catch the exception
    }
    if (tester) {
        // ActiveX is installed
        return true;
    }
    return false;
}

DuradosMail.send = function(to, cc, subject, body, attachmentFileName, attachmentContents) {
    if (DuradosMail.testForActiveX()) {
        //DuradosMail.sendActiveX(to, cc, subject, body, attachmentFileName, attachmentContents);
        DuradosMail.sendActiveX2(to, cc, subject, body, attachmentFileName);
    }
    else {
        DuradosMail.sendMailTo(to, cc, subject, body);
    }
}

//////////////////////////////////



var Org; if (!Org) Org = {};

Org.Dependency = function(json, guid) {
    var orgID = Org.GetOrgID(json);

    if (orgID == null)
        return;

    $('#' + guid + 'create_FK_Contact_Organization_Parent').val(orgID);

    var select = $('#' + guid + 'create_V_Contact_v_ProposalLast2Months_Parent');

    Dependency.Load("v_ProposalLast2Months", "V_Contact_v_ProposalLast2Months_Parent", orgID, select, guid);

}

Org.GetOrgID = function(json) {
    for (var index = 0, len = json.Fields.length; index < len; ++index) {
        var field = json.Fields[index].Value;
        if (field.Name == "FK_Proposal_Organization1_Parent") {
            return field.Value;
        }
    }

    return null;
}

Org.LoadSelectList = function(select, guid) {
    if ($(select).attr('id') == guid + 'create_V_Contact_v_ProposalLast2Months_Parent') {
        if ($(select).children('option').size() > 1) {
            var firstOption = $($(select).children('option')[1]).attr('value');
            $(select).val(firstOption);
        }
    }
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


    $(AddDialog).bind('onafterSetDefaultsFromFilter', function(e, data) {
        Org.Dependency(data.json, data.guid);
    });

    
    $(Dependency).bind('onafterLoadSelectList', function(e, data) {
        Org.LoadSelectList(data.select, data.guid);
    });
});

var Send; if (!Send) Send = {};

Send.DialogName = function() {
    return "dialog_email";
}

Send.Show = function(pkValue, guid, method) {
    var gViewName = views[guid].gViewName;
    
    if (method == 'Custom') {
        Send.ShowCustom(pkValue, gViewName);
    }
    else {
        Send.ShowBuiltin(pkValue, gViewName);
    }
}

Send.ShowBuiltin = function(pk, gViewName) {
    $.ajax({
        url: gVD + 'CRMProposal/GetJsonLetter/' + gViewName,
        contentType: 'application/json; charset=utf-8',
        data: { pk: pk },
        async: false,
        cache: false,
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
        },
        success: function(email) {
            hideProgress();
            //var email = Sys.Serialization.JavaScriptSerializer.deserialize(json);
            DuradosMail.send(email.To, email.Cc, email.Subject, email.Body, email.FileName, email.Content);
        }

    });
}

Send.ShowCustom = function(pkValue, gViewName) {
    $('#Div1').data('title.dialog', 'Send Proposal');
    $("#Div1").dialog("option", "buttons", {
        'Send': function() { Send.Send(pkValue, gViewName); },
        'Cancel': function() { $(this).dialog("close"); }
    });

    //jquery doesn't remember the position just the size
    var rec = Rectangle.Load('SendProposal');
    if (rec != null) {
        $('#Div1').dialog("option", "position", [rec.left, rec.top]);

    }
    $('#Div1').dialog('open');

    Send.Load(pkValue, gViewName);

    //Create wysiwyg for all textareas
    Send.CreateWysiwyg();


    complete();
}

Send.CreateWysiwyg = function() {
    $('#Div1 textarea').each(function() { $(this).htmlarea("dispose") });
    $('#Div1 textarea').htmlarea();
}

Send.Load = function(pk, gViewName) {
    $.ajax({
        url: gVD + 'CRMProposal/GetLetter/' + gViewName,
        contentType: 'application/json; charset=utf-8',
        data: { pk: pk },
        async: false,
        cache: false,
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
        },
        success: function(html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0)
            if (index < 0 || index > 1000) {
                $('#Div1').html(html);
            }
        }

    });
}

Send.Send = function(pk, gViewName) {
    begin();

    var textarea = $('#Div1 TextArea[name="body"]');
    var subject = $('#Div1 input[name="subject"]').val();
    var to = $('#Div1 input[name="to"]').val();
    var cc = $('#Div1 input[name="cc"]').val();

    var body = textarea.text();


    $.post(gVD + 'CRMProposal/Send/' + gViewName,
    {
        pk: pk,
        subject: subject,
        to: to,
        cc: cc,
        body: body
    },
    function(h) {
        var index = h.indexOf("$$error$$", 0)
        if (index < 0 || index > 1000) {
            $('#Div1').dialog('close');
            success();
            alert("The message was succefully sent");
        }
        else {
            alert(h.replace("$$error$$", ""));
            complete();
        }
    });
}

//Dialog
Send.SaveDialogOnResize = function(event, ui) {
    var rect = Rectangle.New(ui.position.top, ui.position.left, ui.size.width, ui.size.height);
    Rectangle.Save(rect, Send.DialogName());
}

Send.SaveDialogOnDrag = function(event, ui) {
    var rect = Rectangle.Load(Send.DialogName())
    if (rect != null) {
        rect.top = ui.position.top;
        rect.left = ui.position.left;
        Rectangle.Save(rect, Send.DialogName());
    }
}

//$(Durados.View).bind('edit', function(e, data) {

//    data.dialog.find('input[name="Date"]').change(function() {
//        Proposal.SetDefaultValid($(this));
//    });

//    //    data.dialog.find('input[name="Date"]').each(function() {
//    //        Proposal.SetDefaultValid($(this));
//    //    });

//});

//$(Durados.View).bind('add', function(e, data) {

//    data.dialog.find('input[name="Date"]').change(function() {
//        Proposal.SetDefaultValid($(this));
//    });

//    data.dialog.find('input[name="Date"]').each(function() {
//        Proposal.SetDefaultValid($(this));
//    });

//});

var Proposal; if (!Proposal) Proposal = {};
Proposal.SetDefaultValid = function(dateElm) {
    var form = dateElm.closest('form');
    if (form.length == 0)
        return;
    var validElm = form.find('input[name="Valid"]');
    var dateStr = dateElm.val();
    var date = toDate(dateStr);

    if (date != null) {
        var validStr = validElm.val();
        var valid = toDate(validStr);

        var newValid;

        var day = date.getDate();

        if (day < 25) {
            newValid = new Date(date.getFullYear(), date.getMonth() + 1, 0);
        }
        else {
            newValid = new Date(date.getFullYear(), date.getMonth() + 2, 0);
        }

        if (valid == null || (newValid.toString() != valid.toString() && confirm('Change valid date?'))) {
            validStr = newValid.getDate() + "/" + (parseInt(newValid.getMonth()) + 1) + "/" + newValid.getFullYear();

            validElm.val(validStr);
        }
    }
}

function toDate(dateStr) {
    var dateParts = dateStr.split('/');
    if (dateParts.length == 3) {
        var day = dateParts[0];
        var month = dateParts[1];
        var year = dateParts[2];

        var temp = month + "/" + day + "/" + year;
        var cfd = Date.parse(temp);

        var date = new Date(cfd);
        return date;
    }
    return null;
}

