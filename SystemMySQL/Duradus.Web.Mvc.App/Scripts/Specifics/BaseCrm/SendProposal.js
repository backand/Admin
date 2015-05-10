var Org; if (!Org) Org = {};

Org.Dependency = function(json, guid) {
    var orgID = Org.GetOrgID(json);


    $('#' + guid + 'create_FK_Contact_Organization_Parent').val(orgID);

    var select = $('#' + guid + 'create_V_Contact_v_ProposalLast2Months_Parent');

    Dependency.Load("v_ProposalLast2Months", "V_Contact_v_ProposalLast2Months_Parent", orgID, select, guid);

}

Org.GetOrgID = function(json){
    for (var index = 0, len = json.Fields.length; index < len; ++index) {
        var field = json.Fields[index].Value;
        if (field.Name == "FK_Proposal_Organization1_Parent"){
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

Send.Show = function(pkValue, guid) {
    $('#Div1').data('title.dialog', 'Send Proposal');
    $("#Div1").dialog("option", "buttons", {
        'Send': function() { Send.Send(pkValue, guid); },
        'Cancel': function() { $(this).dialog("close"); }
    });

    //jquery doesn't remember the position just the size
    var rec = Rectangle.Load('SendProposal');
    if (rec != null) {
        $('#Div1').dialog("option", "position", [rec.left, rec.top]);

    }
    $('#Div1').dialog('open');

    Send.Load(pkValue, guid);

    //Create wysiwyg for all textareas
    Send.CreateWysiwyg();


    complete(guid);
}

Send.CreateWysiwyg = function() {
    $('#Div1 textarea').each(function() { $(this).htmlarea("dispose") });
    $('#Div1 textarea').htmlarea();
}

Send.Load = function(pk, guid) {
    $.ajax({
        url: gVD + views[guid].Controller + '/GetLetter/' + views[guid].gViewName,
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

Send.Send = function(pk, guid) {
    begin();

    var textarea = $('#Div1 TextArea[name="body"]');
    var subject = $('#Div1 input[name="subject"]').val();
    var to = $('#Div1 input[name="to"]').val();
    var cc = $('#Div1 input[name="cc"]').val();

    var body = textarea.text();


    $.post(gVD + views[guid].Controller + '/Send/' + views[guid].gViewName,
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
            success(guid);
            alert("The message was succefully sent");
        }
        else {
            alert(h.replace("$$error$$", ""));
            complete(guid);
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