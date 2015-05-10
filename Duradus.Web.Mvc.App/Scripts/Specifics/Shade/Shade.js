var prices = null;
var multiply = null;

$(Durados.View).bind('edit', function(e, data) {
    var select = data.dialog.find('select[name="FK_ProposalItem_Product_Parent"]');
    if (select.length > 0 && select.val() != '') {
        ProposalItem.LoadProductInfo(select.val(), select.closest('form'), false);
    }
});

$(Durados.View).bind('initDataTableView', function(e, data) {
    $('select[name="FK_ProposalItem_Product_Parent"]').change(function() {
        var val = $(this).val();
        if (val != '')
            ProposalItem.LoadProductInfo(val, $(this).closest('form'), true);
    });

    $('select[name="FK_ProposalItem_Fruction_Parent"]').change(function() {
        ProposalItem.PriceParamChanged($(this).closest('form'));
    });

    $('select[name="FK_ProposalItem_Fruction1_Parent"]').change(function() {
        ProposalItem.PriceParamChanged($(this).closest('form'));
    });

    $('input[name="Width"]').change(function() {
        ProposalItem.PriceParamChanged($(this).closest('form'));
    });

    $('input[name="Height"]').change(function() {
        ProposalItem.PriceParamChanged($(this).closest('form'));
    });

    $('input[name="Cost"]').change(function() {
        ProposalItem.RateParamChanged($(this).closest('form'));
    });

    $('select[name="FK_ProposalItem_Vndor_Parent"]').change(function() {
        ProposalItem.LoadVendorInfo($(this).val(), $(this).closest('form'));
    });

    $('input[name="Qty"]').change(function() {
        ProposalItem.TotalParamChanged($(this).closest('form'));
    });

    $('input[name="TotalDiscount"]').change(function() {
        ProposalItem.TotalParamChanged($(this).closest('form'));
    });

    $('input[name="Rate"]').change(function() {
        ProposalItem.TotalParamChanged($(this).closest('form'));
    });

});


$(Durados.View).bind('initItemDialog', function(e, data) {

    //    $('select[name="FK_V_Contact_Job_Parent"]').change(function() {
    //        Job.LoadContectInfo($(this).closest('form'), true);
    //    });
    $('input[name="FK_V_Contact_Job_Parent"]').blur(function() {
        var form = $(this).closest('form');
        if (form.length == 1) {
            var pk = GetAutoCompleteValueId(form.find('input[name="FK_V_Contact_Job_Parent"]'));
            if (pk == null || pk == '') {
            }
            else {
                Job.LoadContectInfo(form, false, pk, true);
            }
        }
    });

    //    if ($('select[name="FK_V_Contact_Job_Parent"]').length > 0) {
    //        $('select[name="FK_V_Contact_Job_Parent"]').each(function(index) {
    //            Job.AddLinks($(this).closest('form'));
    //        });
    //    }

    if ($('input[name="FK_V_Contact_Job_Parent"]').length > 0) {
        $('input[name="FK_V_Contact_Job_Parent"]').each(function(index) {
            Job.AddLinks($(this).closest('form'));
        });
    }
});

var ProposalItem; if (!ProposalItem) ProposalItem = {};


ProposalItem.LoadProductInfo = function(pk, form, performChange) {
    showProgress();

    $.ajax({
        url: rootPath + 'CRMShadeProposalItem/GetProductInfo',
        contentType: 'application/json; charset=utf-8',
        data: { pk: pk },
        async: false,
        dataType: 'json',
        cache: false,
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            hideProgress();
            alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
        },
        success: function(productInfo) {
            if (productInfo == null || productInfo == '') {
                alert("error");
            }
            else {
                //var productInfo = Sys.Serialization.JavaScriptSerializer.deserialize(json);
                prices = productInfo.Prices;
                if (performChange) {
                    form.find('textarea[name="Description"]').text(productInfo.Description);
                    ProposalItem.PriceParamChanged(form);
                }
            }
            hideProgress();

        }

    });
}

ProposalItem.LoadVendorInfo = function(pk, form) {
    showProgress();

    $.ajax({
        url: rootPath + 'CRMShadeProposalItem/GetVendorInfo',
        contentType: 'application/json; charset=utf-8',
        data: { pk: pk },
        async: false,
        dataType: 'json',
        cache: false,
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            hideProgress();
            alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
        },
        success: function(vendorInfo) {
            if (vendorInfo == null || vendorInfo == '') {
                alert("error");
            }
            else {
                //var productInfo = Sys.Serialization.JavaScriptSerializer.deserialize(json);
                multiply = vendorInfo.Multiply;
                ProposalItem.RateParamChanged(form);
            }
            hideProgress();

        }

    });
}

ProposalItem.GetPrice = function(width, height) {
    var len = prices.length;

    if (len == 0) {
        return null;
    }

    if (len == 1)
        return prices[0];


    if (prices[0].Width > width) {
        if (prices[0].Height > height) {
            return prices[0];
        }
        else {
            for (var index = 0; index < len - 1; ++index) {
                var currHeight = prices[index].Height;
                var nextHeight = prices[index + 1].Height;

                if (height <= nextHeight) {
                    return prices[index + 1];
                }
            }
            var currHeight = prices[len - 1].Height;
            for (var index = len - 1; index > 0; --index) {
                var prevHeight = prices[index - 1].Height;
                if (prevHeight != currHeight) {
                    return prices[index];
                }
            }

            //            for (var index = 0; index < len; ++index) {
            //                var currHeight = prices[index].Height;
            //                var currWidth = prices[index].Width;
            //                if (index < len) {
            //                    var nextWidth = prices[index + 1].Width;

            //                    if (nextWidth != currWidth) {
            //                        return prices[index];
            //                    }
            //                    else {
            //                        var nextHeight = prices[index + 1].Height;

            //                        if (height < nextHeight) {
            //                            return prices[index];
            //                        }
            //                    }
            //                }
            //                else {
            //                    return prices[index];
            //                }
            //            }
        }
    }
    else {
        if (prices[len - 1].Width < width) {
            if (prices[len - 1].Height < height) {
                return prices[len - 1];
            }
            else {
                for (var index = 0; index < len - 1; ++index) {
                    var currHeight = prices[index].Height;
                    var nextHeight = prices[index + 1].Height;

                    if (height <= nextHeight) {
                        var currHeight = nextHeight;
                        for (var index2 = index; index2 < len; ++index2) {
                            var nextHeight = prices[index2 + 1].Height;
                            if (nextHeight != currHeight) {
                                return prices[index2];
                            }
                        }
                    }
                }
            }
        }
        else {
            //            for (var index = 0; index < len - 1; ++index) {
            //                var currWidth = prices[index].Width;
            //                var nextWidth = prices[index + 1].Width;

            //                if (width < nextWidth) {
            //                    for (var index2 = index; index2 > 0; --index2) {
            //                        var prevWidth = prices[index - 1].Width;
            //                        var prevHeight = prices[index - 1].Height;
            //                        if (prevWidth != currWidth) {
            //                            return prices[index2];
            //                        }
            //                        else {
            //                            if (height < prevHeight) {
            //                                return prices[index2];
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            for (var index = 0; index < len - 1; ++index) {
                var currHeight = prices[index].Height;
                var nextHeight = prices[index + 1].Height;

                if (height <= nextHeight) {
                    for (var index2 = index; index2 < len; ++index2) {
                        var nextWidth = prices[index2 + 1].Width;
                        if (width <= nextWidth) {
                            return prices[index2 + 1];
                        }

                    }
                }
            }

            for (var index = len - 1; index > 0; --index) {
                var prevWidth = prices[index - 1].Width;
                if (width >= prevWidth) {
                    if (width > prevWidth) {
                        return prices[index];
                    }
                    else {
                        return prices[index - 1];
                    }
                }
            }
        }
    }

}


ProposalItem.PriceParamChanged = function(form) {
    var widthInt = form.find('input[name="Width"]').val();
    var heightInt = form.find('input[name="Height"]').val();
    var widthFrc = form.find('select[name="FK_ProposalItem_Fruction_Parent"]').val();
    var heightFrc = form.find('select[name="FK_ProposalItem_Fruction1_Parent"]').val();
    var product = form.find('select[name="FK_ProposalItem_Product_Parent"]').val();

    var price = null;
    if (widthInt != '' && widthFrc != '' && heightInt != '' && heightFrc != '' && product != '' && prices != null) {
        var width = parseInt(widthInt);
        if (parseInt(widthFrc) > 0) {
            width = parseFloat(width + 0.5);
        }
        var height = parseInt(heightInt);
        if (parseInt(heightFrc) > 0) {
            height = parseFloat(height + 0.5);
        }

        var costInput = form.find('input[name="Cost"]');
        var cuurentCost = costInput.val();

        price = ProposalItem.GetPrice(width, height);
    }
    else if (prices.length == 1)
        price = prices[0];

    if (price != null && (cuurentCost == '' || confirm("Change current cost?"))) {
        var costInput = form.find('input[name="Cost"]');
        costInput.val(price.Cost);
        
        var seamedInput = form.find('input[name="Seamed"]');
        seamedInput.attr('checked', price.Seamed);
        ProposalItem.RateParamChanged(form);
    }

}

ProposalItem.RateParamChanged = function(form) {

    if (prices.length == 1) //check in case the product already has rate
    {
        var rate = prices[0].PPrice;
        if (rate > 0) {
            var rateInput = form.find('input[name="Rate"]');
            rateInput.val(rate);
            ProposalItem.TotalParamChanged(form);
            return;
        }
    }

    var cost = form.find('input[name="Cost"]').val();
    var vendor = form.find('select[name="FK_ProposalItem_Vndor_Parent"]').val();

    if (cost != '' && vendor != '' && multiply != null) {
        var rate = parseFloat(cost) * parseFloat(multiply);
        rate = Durados.round(rate, 2).toFixed(2);
        var rateInput = form.find('input[name="Rate"]');
        rateInput.val(rate);
        ProposalItem.TotalParamChanged(form);
    }
}

ProposalItem.TotalParamChanged = function(form) {
    var rate = form.find('input[name="Rate"]').val();
    var qty = form.find('input[name="Qty"]').val();
    var discount = form.find('input[name="TotalDiscount"]').val();

    if (discount == '')
        discount = 0;
        
    if (rate != '' && qty != '') {
        var total = parseFloat(rate) * parseFloat(qty) - discount;
        total = Durados.round(total, 2).toFixed(2);
        var totaInput = form.find('input[name="Total"]');

        totaInput.val(total);
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
    
});

var Send; if (!Send) Send = {};

Send.DialogName = function() {
    return "dialog_email";
}

Send.Show = function(pkValue, guid) {
    $('#Div1').dialog( "option" , "title" , 'Send Proposal');
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
    var bcc = $('#Div1 input[name="bcc"]').val();

    var body = textarea.text();


    $.post(gVD + views[guid].Controller + '/Send/' + views[guid].gViewName,
    {
        pk: pk,
        subject: subject,
        to: to,
        cc: cc,
        bcc: bcc,
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

var Job; if (!Job) Job = {};

Job.AddLinks = function(form) {
    var ClientName = form.find('input[name="ClientName"]');
    var div = ClientName.closest('div');

    if (div != null && form.find('#load').length == 0) {
        var load = $('<a id="load" href="#">Load Contact Info</a>');
        var clear = $('<a id="clear" href="#">Clear Client Info</a>');
        div.prepend(load);
        div.prepend($('<span>&nbsp;&nbsp;&nbsp;&nbsp;</span>'));
        div.prepend(clear);

        load.click(function() {
            var pk = GetAutoCompleteValueId(form.find('input[name="FK_V_Contact_Job_Parent"]'));
            if (pk == null || pk == '') {
                alert("Please select a contact.");
                return;
            }
            Job.LoadContectInfo(form, false, pk, false);
        });

        clear.click(function() {
            Job.ClearClientInfo(form);
        });

    }

}

Job.LoadContectInfo = function(form, confirmation, pk, async) {
    //var pk = form.find('select[name="FK_V_Contact_Job_Parent"]').val();


    $.ajax({
        url: rootPath + 'CRMShadeJob/GetContactInfo',
        contentType: 'application/json; charset=utf-8',
        data: { pk: pk },
        async: async,
        dataType: 'json',
        cache: false,
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            hideProgress();
            alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
        },
        success: function(contactInfo) {
            if (contactInfo == null || contactInfo == '') {
                alert("error");
            }
            else {
                var ClientName = form.find('input[name="ClientName"]');
                var ClientPhone = form.find('input[name="ClientPhone"]');
                var ClientCellular = form.find('input[name="ClientCellular"]');
                var ClientEmail = form.find('input[name="ClientEmail"]');
                var address = form.find('input[name="FK_Job_Address_Parent"]');
                //                var JobStreet = form.find('input[name="JobStreet"]');
                //                var JobCity = form.find('input[name="JobCity"]');
                //                var JobState = form.find('input[name="JobState"]');
                //                var JobZip = form.find('input[name="JobZip"]');
                //                var JobCountry = form.find('input[name="JobCountry"]');

                if (!confirmation || (ClientName.val() == '' && ClientPhone.val() == '' && ClientCellular.val() == '' && ClientEmail.val() == '') || confirm("Change client info according to the selected contact?")) {
                    ClientName.val(contactInfo.ClientName);
                    ClientPhone.val(contactInfo.ClientPhone);
                    ClientCellular.val(contactInfo.ClientCellular);
                    ClientEmail.val(contactInfo.ClientEmail);
                    SetAutoCompleteValueId(address, contactInfo.AddressText, contactInfo.AddressID);
                    //                    JobStreet.val(contactInfo.JobStreet);
                    //                    JobCity.val(contactInfo.JobCity);
                    //                    JobState.val(contactInfo.JobState);
                    //                    JobZip.val(contactInfo.JobZip);
                    //                    JobCountry.val(contactInfo.JobCountry);
                }
            }
            hideProgress();

        }

    });
}

Job.ClearClientInfo = function(form) {
    var ClientName = form.find('input[name="ClientName"]');
    var ClientPhone = form.find('input[name="ClientPhone"]');
    var ClientCellular = form.find('input[name="ClientCellular"]');
    var ClientEmail = form.find('input[name="ClientEmail"]');
    var address = form.find('input[name="FK_Job_Address_Parent"]');
    SetAutoCompleteValueId(address, '', '');
    //    var JobStreet = form.find('input[name="JobStreet"]');
//    var JobCity = form.find('input[name="JobCity"]');
//    var JobState = form.find('input[name="JobState"]');
//    var JobZip = form.find('input[name="JobZip"]');
//    var JobCountry = form.find('input[name="JobCountry"]');

    ClientName.val('');
    ClientPhone.val('');
    ClientCellular.val('');
    ClientEmail.val('');
//    JobStreet.val('');
//    JobCity.val('');
//    JobState.val('');
//    JobZip.val('');
//    JobCountry.val('');
}