// JavaScript Document
var sequence = null;
var Preloader = {};
var oldTempl= null;
var waitImgSrc = '/Website/img/addviewswait.gif';
var errMsgMaxLength=80;
var LongProductName = 'backand';
var ShortProductName = 'Back&';

$(document).ready(function () {
    preloader(waitImgSrc);

    $status = $(".status");
    var options = {
        autoPlayDelay: 5000,
        hidePreloaderDelay: 500,
        nextButton: ".next",
        prevButton: ".prev",
        pauseButton: ".pause",
        hidePreloaderusingCSS: false,
        animateStartingFrameIn: true,
        transitionThreshold: 500,
        pauseOnHover: true,
        customKeyEvents: { 0: "pause" },

        afterNextFrameAnimatesIn: function () {
            if (sequence.settings.autoPlay) {
                $status.addClass("active").css("opacity", 1);
            }
            $(".prev, .next").css("cursor", "pointer").animate({ "opacity": 1 }, 500);
        },
        beforeCurrentFrameAnimatesOut: function () {
            $status.animate({ "opacity": 0 }, 500, function () {
                $status.removeClass("active");
            });
            $(".prev, .next").css("cursor", "auto").animate({ "opacity": .7 }, 500);
        },
        paused: function () {
            $status.animate({ "opacity": 0 }, 500, function () {
                $status.removeClass("active").addClass("paused");
            });
        },
        unpaused: function () {
            $status.removeClass("active paused");
        }

    };

    sequence = $("#sequence").sequence(options).data("sequence");

    $('.seq1').click(function (e) {
        sequence.goTo(1, 1);
    });

    $('.seq2').click(function (e) {
        sequence.goTo(2, 1);
    });

    $('.seq3').click(function (e) {
        sequence.goTo(3, 1);
    });
    $('#signIn, #resetPassword, #resetPasswordApproval').dialog(
		{
		    modal: true,
		    draggable: false,
		    resizable: false,
		    autoOpen: false,
		    hide: { effect: 'fade', duration: 100 },
		    show: { effect: 'fade', duration: 200 },
		    closeText: '',
		    width: 550,
		    stack: false,
		    position: ["center", 100]
//		    open: function () { setTimeout(function () { $('[name="userName"]').focus(); }, 100); }
		});

    $('#dialog').dialog(
	{
	    modal: true,
	    draggable: false,
	    resizable: false,
	    autoOpen: false,
	    hide: { effect: 'fade', duration: 100 },
	    show: { effect: 'fade', duration: 200 },
	    closeText: '',
	    width: 740,
	    stack: false,
	    position: ["center", 100]
	});

    $('#selectTemplate').dialog(
		{
		    modal: true,
		    draggable: false,
		    resizable: false,
		    autoOpen: false,
		    hide: { effect: 'fade', duration: 100 },
		    show: { effect: 'fade', duration: 200 },
		    closeText: '',
		    width: 'auto',
		    stack: false,
		    position: ["center", 100],
		    close: function () { oldTempl = null; location.reload(); }

		});
    $('#tutorial').dialog(
		{
		    modal: true,
		    draggable: false,
		    resizable: false,
		    autoOpen: false,
		    hide: { effect: 'fade', duration: 1 },
		    show: { effect: 'fade', duration: 200 },
		    closeText: '',
		    width: 1330,
		    stack: false,
		    position: ["center", 100],
		    close: function () { $('#video').html(''); $('.change-sequence').show(); }
		});

    $('#selectTemplateError, #selectTemplateError2, #contactdlg').dialog(
		{
		    modal: true,
		    draggable: false,
		    resizable: false,
		    autoOpen: false,
		    hide: { effect: 'fade', duration: 100 },
		    show: { effect: 'fade', duration: 200 },
		    closeText: '',
		    width: 'auto',
		    stack: false,
		    position: ["center", 100]

		});

    $('.ui-dialog-titlebar-close').click(function () {
        //        $('#dialog, #signIn, #selectTemplate').dialog('close');
        if (sequence) sequence.startAutoPlay(0);
        $(".main-logo").show();
        //Preloader.start();
        return false;
    });

    $('.textfield, .styled').bind('focus', function (e) {
        e.preventDefault();
        $(this).parent('.field-holder').addClass('focus');

    });
    $('.textfield, .styled').bind('blur', function (e) {
        e.preventDefault();
        $(this).parent('.field-holder').removeClass('focus');
    });

    $('.footer-icons').click(function () {
        var bgHeight = "470";
        var bgWidth = "605";
        var url = "https://seal.godaddy.com/verifySeal?sealID=OTEmVHFHFLqvyREwJp9pmMZ6kZi48Js6oezJyzlAywgSSwEpwivb";
        window.open(url, 'SealVerfication', 'menubar=no,toolbar=no,personalbar=no,location=yes,status=no,resizable=yes,fullscreen=no,scrollbars=no,width=' + bgWidth + ',height=' + bgHeight);
        _gaq.push(['_trackEvent', 'Dialogs', 'security', 'open sec main']);
    });


    $('#dialog').find('select[name="databasetype"]').change(function () {
        var db = $(this).val();
        if (db == 0)
            $(this).parent().addClass('blur');
        else
            $(this).parent().removeClass('blur');
        if (db == 9) {
            $('#dialog').find('input[name="dbother"]').parents('fieldset:first').show();
            $('#dialog').find('input[name="dbother"]').attr('requierd', true);
        }
        else {
            $('#dialog').find('input[name="dbother"]').parents('fieldset:first').hide();
            $('#dialog').find('input[name="dbother"]').removeAttr('requierd');
        }

    });

    // List box
    $('select.styled').customStyle();

    // Tips
    $('.info').tipsy({
        delayIn: 0,      // delay before showing tooltip (ms)
        delayOut: 0,     // delay before hiding tooltip (ms)
        fade: true,     // fade tooltips in/out?
        fallback: '',    // fallback text to use when no tooltip text
        gravity: 's',    // gravity
        html: true,     // is tooltip content HTML?
        live: true,     // use live event support?
        offset: 5,       // pixel offset of tooltip from element
        opacity: 1,    // opacity of tooltip
        trigger: 'hover', // how tooltip is triggered - hover | focus | manual
        title: function () { return $(this).next('.toolTip').html(); }
    });

    //send email in contact
    ContactUs.init();
    Demo.init();
    SignIn.init();
    SignUp.init();
    Tutorial.init();
    Forgot.init();

    HandleRequest();

    HandleSupport();

    HandleTootips();

    ClickToCall.init();

    setInterval(barAnimation, 2000);
});


function barAnimation() {
    //run only every 5 times in random
    if (Math.floor(Math.random() * 5) == 1) {
        $('.slider').css({ opacity: '0' });
        $('.slider').animate({ opacity: "0.5", left: "95%" }, '1000', 'jswing');
        $('.slider').animate({ opacity: "0", left: "50px" }, 'slow');
    }
}

function Alert(title, message) {
    if (!title)
        title = "";

    if (!message)
        message = "";

    $('<div></div>').appendTo('body')
          .html('<div class="confirm-message">' + message + '</div>')
          .dialog({
              modal: true, title: title, zIndex: 100000002, autoOpen: true,
              width: 'auto', resizable: false,

              close: function (event, ui) {
                  $(this).remove();
              },
              open: function () {
                  $(this).parent().find('.ui-icon-closethick').text('');
              }

          });
}

var ClickToCall; if (!ClickToCall) ClickToCall = {};

ClickToCall.stripCharsInBag = function (s, bag) {
    var i;
    var returnString = "";
    // Search through string's characters one by one.
    // If character is not in bag, append to returnString.
    for (i = 0; i < s.length; i++) {
        // Check that current character isn't whitespace.
        var c = s.charAt(i);
        if (bag.indexOf(c) == -1) returnString += c;
    }
    return returnString;
}
ClickToCall.isInteger = function isInteger(s) {
    var i;
    for (i = 0; i < s.length; i++) {
        // Check that current character is number.
        var c = s.charAt(i);
        if (((c < "0") || (c > "9"))) return false;
    }
    // All characters are numbers.
    return true;
}

ClickToCall.validatePhoneNumber = function (strPhone) {
    var bracket = 3
    this.digits = "0123456789";
    // non-digit characters which are allowed in phone numbers
    this.phoneNumberDelimiters = "()- ";
    // characters which are allowed in international phone numbers
    // (a leading + is OK)
    this.validWorldPhoneChars = this.phoneNumberDelimiters + "+";
    // Minimum no of digits in an international phone no.
    this.minDigitsInIPhoneNumber = 6;
    if (strPhone.indexOf("+") > 1) return false;
    if (strPhone.indexOf("-") != -1) bracket = bracket + 1;
    if (strPhone.indexOf("(") != -1 && strPhone.indexOf("(") > bracket) return false;
    var brchr = strPhone.indexOf("(");
    if (strPhone.indexOf("(") != -1 && strPhone.charAt(brchr + 2) != ")") return false;
    if (strPhone.indexOf("(") == -1 && strPhone.indexOf(")") != -1) return false;
    s = ClickToCall.stripCharsInBag(strPhone, this.validWorldPhoneChars);
    return (ClickToCall.isInteger(s) && s.length >= this.minDigitsInIPhoneNumber);
}
ClickToCall.OpenDialog = function () {

   
    $("#Click2Call").parent().find('.ui-icon-closethick').text('');

    $("#Click2Call").find("#click2callPhone").blur();
    $('#Click2Call').dialog('open');
}

ClickToCall.CreateDialog = function () {

    $(' #Click2Call').dialog(
		{
		    modal: true,
		    draggable: false,
		    resizable: false,
		    autoOpen: false,
		    hide: { effect: 'fade', duration: 100 },
		    show: { effect: 'fade', duration: 200 },
		    closeText: '',
		    width: '500px',
		    stack: false,
		    position: ["center", 350]

		});
    
}
ClickToCall.init = function () {
    $("#Click2Call").hide();
    ClickToCall.CreateDialog();
    $('#callUs').bind('click', function () {
        var notValidPhoneNumMsg = 'Please enter a valid phone number.';
        var caller = $('input[name="clicktocall"]').val();
        var callerHint = $('input[name="clicktocall"]').attr('hint');

        if (!caller || caller == "" || caller == callerHint || !ClickToCall.validatePhoneNumber(caller)) {//validate international phone|| !validPhone.test(caller)
            Alert('Information message', notValidPhoneNumMsg);
        }
        else {
            var data = { caller: caller };
            var url = '/WebsiteAccount/ClickToCall?caller=' + caller;
            $.ajax({
                url: url,
                contentType: 'application/json; charset=utf-8',
                data: data,
                type: "POST",
                async: true,
                dataType: 'json',
                cache: false,
                error: function () { alert('The server is busy, please try again later.'); },
                success: function (valid) {
                    if (valid.Success == true) {
                        Alert('Information message', 'Your call request was sent succesfully. We will call in 30 seconds.');
                    }
                    else {
                        Alert('Information message', 'The call did not start due to the following reason: ' + valid.Message);
                    }
                }
            });
        }
    });
}


function HandleTootips(){

    $('[data-toggle=popover]').popover(
      //{
      //trigger:'click',
      //animation: false
  //}
  );  
    
    
}
    

function HandleSupport() {
    $('.clickMe').click(function () {
        //alert($(this).siblings("div.supportUnfold").name);
        $(this).siblings("div.supportUnfold:first").slideToggle('slow', function () {
            // Animation complete.
        });
    });

}

function HandleRequest() {
    var dataSource = queryString('ty');
    if (dataSource) {
        _gaq.push(['_trackEvent', 'Dialogs', 'Shortcut ty', 'website open with ty' + dataSource.toString()]);
        defaultDataSource = dataSource;
        $('.reg-button:first').click();
    }
}

function queryString(key) {
    return queryString2(window.location.href, key);
}

function queryString2(url, key) {
    var vars = [], hash;
    var hashes = url.slice(url.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars[key];
}

function StopAutoImages() {
    if (sequence) sequence.stopAutoPlay();
    $(".main-logo").hide();
}

var DivButton; if (!DivButton) DivButton = {};

DivButton.EnabledClass = function () {
    return "login-item";
}

DivButton.DisabledClass = function () {
    return "login-item-disabled";
}

DivButton.Enable = function (div) {
    div.removeAttr('disabled');
    div.removeClass(DivButton.DisabledClass());
    div.addClass(DivButton.EnabledClass());
}

DivButton.Disable = function (div) {
    div.attr('disabled', 'disabled');
    div.removeClass(DivButton.EnabledClass());
    div.addClass(DivButton.DisabledClass());
}

DivButton.IsDisabled = function (div) {
    return div.attr('disabled') == 'disabled';
}

var Tutorial; if (!Tutorial) Tutorial = {};

Tutorial.init = function () {
    $("div.tutorial").click(function () {
        _gaq.push(['_trackEvent', 'Videos', 'Play', 'demo video']);
        $('.change-sequence').hide();
        $('#video').html('<iframe width="1280" height="720" src="https://www.youtube-nocookie.com/embed/Wet_K2g71Rw?rel=0&autoplay=1" frameborder="0"></iframe>');
        $('#tutorial').dialog('open');
        StopAutoImages();
    });

    $("div.main_video").click(function () {
        _gaq.push(['_trackEvent', 'Videos', 'Play', 'main video']);
        $('.change-sequence').hide();
        $('#video').html('<iframe width="1280" height="720" src="https://www.youtube-nocookie.com/embed/Wet_K2g71Rw?rel=0&autoplay=1" frameborder="0"></iframe>');
        $('#tutorial').dialog('open');
        StopAutoImages();
    });

    $("#tutorial").find("a").click(function () {
        $('#video').html('');
        SignUp.reg();
        $('#tutorial').dialog('close');
    });

    $("#tutorial").find(".closeMe").click(function () {
        $('#video').html('');
        $('#tutorial').dialog('close');
    });

}

function howtovideo(video) {

    $('#video').html('<iframe width="1280" height="720" src="' + video + '" frameborder="0"></iframe>');
    $('#tutorial').dialog('open');


}

var SignUp; if (!SignUp) SignUp = {};

SignUp.init = function () {
    $('.reg-button').click(function () {
        _gaq.push(['_trackEvent', 'Dialogs', 'try button', 'click on main green button']);
        isTry = true;
        SignUp.reg();

    });

    $('#next').click(function () {
        var div = $(this);
        if (!DivButton.IsDisabled(div)) {
            SignUp.next(div);
            StopAutoImages();
        }
    });

    $('.sign-up-sign-in a').click(function () {
        $('#dialog').dialog('close');
        $('#signIn').dialog('open');
    });
    
    
    $('#dialog').find('input,select[name="databasetype"]').bind('change',function () {
       Track.lostFocus()
    });

    $('#dialog').find('input[name="dbother"]').parents('fieldset:first').hide();
}

var defaultDataSource = '4'; //MySQL

function getDefaultDataSource(){
    var dataSrc=$('.sign-up-form select[name="databasetype"]').val();
    if(dataSrc==2 || dataSrc==3 || dataSrc==4)
        return dataSrc;
    return defaultDataSource;
}
 

SignUp.reg = function () {
    gotoApps = false;
    StopAutoImages();
    if (!SignUp.isLoggedin()) {
        $('#dialog').dialog('open');
    }
    else {
        if (isTry == true) {
            isTry = false;
            $('#selectTemplate').find('select[name=template]').val(getDefaultDataSource()).change();
            $('#selectTemplate').find('input[required="true"]').each(function () { $(this).val(''); });
        }
        else
            $('#selectTemplate').find('select[name=template]').val('0').change();

        $('#selectTemplate').dialog('open');

    }
    StopAutoImages();
    // find all the input elements with title attributes
    $('input[hint!=""]').hint();
}


SignUp.next = function (div) {
    var elm = $('#dialog');
    //Preloader.start();
    
    if (SignUp.validate(elm, SignUp)) {
            DivButton.Disable(div);
            SignUp.submit(div);
    }
    //Preloader.stop();
}

SignUp.getInfo = function () {
    var form = $('#dialog');
    var phoneElm = form.find('input[name="phonenumber"]');
    var phoneVal = phoneElm.val() == phoneElm.attr('hint') ? "":phoneElm.val();
    return { username: form.find('input[name="username"]').val(), password: form.find('input[name="password"]').val(), phone: phoneVal, fullname: form.find('input[name="fullname"]').val(), dbtype: $('.sign-up-form select[name="databasetype"]').val(), dbother: $('.sign-up-form input[name="dbother"]').val() };
}

SignUp.isLoggedin = function () {

    var url = '/WebsiteAccount/IsLoggedIn';
    var isLoggedin = false;
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        type: "POST",
        async: false,
        dataType: 'json',
        cache: false,
        error: function () { SignUp.message('The server is busy, please try again later.') },
        success: function (response) {
            if (response.isLoggedIn) {
                Demo.setDefaults(response.DemoDefaults, 0);
            }
            isLoggedin = response.isLoggedIn;
        }

    });

    return isLoggedin;

}

SignUp.submit = function (div) {

    var info = SignUp.getInfo();
    var data = { username: info.username, password: info.password, send: 'true', phone: info.phone, fullname: info.fullname, dbtype: info.dbtype, dbother: info.dbother };

    var url = '/WebsiteAccount/SignUp';

    $.ajax({
        url: url,
        //contentType: 'application/json; charset=utf-8',
        data: data,
        type: "Post",
        async: false,
        dataType: 'json',
        cache: false,
        error: function () { SignUp.message('The server is busy, please try again later.') },
        success: function (response) {
            StopAutoImages();
            if (response.Success) {
                if(info.dbtype==10)
                {
                    openContactUsWithMessage("I don't have a database but I would like to use " + ShortProductName + ", please contact me.",info.fullname,info.username,info.phone,true);
                    return;
                }
                if(info.dbtype==9)
                {
                    openContactUsWithMessage("I don't use MySQL or SQL Server (I use " + info.dbother + "), please contact me when you are going to support addtional databases.",info.fullname,info.username,info.phone,true);
                    return;
                }
                Demo.setDefaults(response.DemoDefaults, 0);
                _gaq.push(['_trackEvent', 'Dialogs', 'SignUp', 'SignUp Submit']);
                $('#dialog').dialog('close');
                //$('#selectTemplate').dialog('open')
                g_email = info.username;
                SignUp.reg();
            }
            else {
                DivButton.Enable(div);
                SignUp.message(response.Message);
            }
        }

    });

    return;

}

SignUp.validate = function (elm, classer) {
    SignUp.hideMessage();
    var isValid = true;
    $(elm).find('input[requierd="true"],select[requierd="true"]').each(function () {//,textarea[requierd="true"]'
        if ((!this.value || this.value == $(this).attr('hint')) || this.tagName=="SELECT" && this.value=="0" ) {
            classer.handleFieldError(this, this.name, '');
            $('#dialog').dialog('open')
            isValid = false;
        }
    });

    if (!isValid)
        return isValid;

    var emailElm = $(elm).find('input[name="username"]');
    var email = emailElm.val();

    if (email && !isValidEmail(email)) {
        classer.handleFieldError(emailElm, "Email", 'Invalid Email');
        isValid = false;
    }

    var dbType = $(elm).find('select[name="databasetype"]');
    if (dbType.val() == "0") {

        isValid = false;
    }
    //    var passwordElm = $(elm).find('input[name="password"]');
    //    var password = passwordElm.val();

    //    var confirmPasswordElm = $(elm).find('input[name="confirmPassword"]');
    //    var confirmPassword = confirmPasswordElm.val();

    //    var len = password.length;
    //    if (len > 10 || len < 6) {
    //        classer.handleFieldError(passwordElm, "", '6-10 characters');
    //        isValid = false;
    //        return isValid
    //    }

    //    if (passwordElm && confirmPasswordElm && confirmPassword != password) {
    //        classer.handleFieldError(passwordElm, "", 'No Match');
    //        classer.handleFieldError(confirmPasswordElm, "", 'No Match');
    //        isValid = false;
    //    }

    return isValid;

}
SignUp.handleFieldError = function (elm, name, msg) {

    //var errElm=$(elm).next();
    var errElm=$(elm).parent('div').find('span.error-message');
    if(msg != '')
        errElm.html(msg).css('display', 'block');
    else
        errElm.css('display', 'block');

}

SignUp.hideMessage = function () {
    $('#dialog').find('span.error-message').hide();
}

SignUp.message = function (msg) {
    $('#dialog').find('span.general').show().html(msg);
}

var Demo; if (!Demo) Demo = {};

var demoDefaults = [];
var defaultTemplate = true;
var isTry = false;


Demo.init = function () {
    generalSignInError = $('#DemoForm').find('span.general');

    $('input').blur(function () {
        $(this).next('span.error-message').hide();
        DivButton.Enable($('#submit'));
        generalSignInError.hide();
    });
    $('#DemoForm').find('select[name=template]').change(function () {
        var tmpl = $('select[name=template]').val();
        if (tmpl == '0' || tmpl == '1') {
            if (oldTempl != null)
                Demo.save();
            Demo.setDefaults(null, tmpl);
        }
        else {
            oldTempl = tmpl;
            Demo.restore(tmpl);
        }

        var appName = $('input[name="name"]').val();
        if (appName === '')
            appName = $('input[name="name"]').attr('hint');


        Demo.handleMySQL(tmpl);
        $('input[hint!=""]').hint();

        //set the port of the fw settings
        //        var fwport = $('#fwset');
        //        if (tmpl == '0')
        //            fwport.html('');
        //        else {
        //            var portValue = "1433";
        //            if (tmpl == '4')
        //                portValue = $('input[name="productPort"]').val();

        //            fwport.html(g_msg1.replace("[1235]", portValue));
        //        }
    });

    $('#demo').click(function () {
        var div = $(this);
        if (!DivButton.IsDisabled(div)) {
            Demo.validateAndSubmit(div);
        }
    });

    $('.demo-link').click(function () {
        _gaq.push(['_trackEvent', 'Dialogs', 'demo', 'open demo dialog']);
        if (g_demotoday == 'yes')
            SignUp.reg();
        else
            window.location = "/contact?d=true";
    });

    $('.try').click(function () {
        _gaq.push(['_trackEvent', 'Dialogs', 'try button', 'open try dialog']);
        defaultTemplate = false;
        isTry = true;
        SignUp.reg();

    });

    var usesSsh = $('input[name="ssh"]');
    var sshDiv = $('div.ssh');
    usesSsh.change(function () {
        if ($(this).prop('checked')) {
            sshDiv.show();
            sshDiv.find('input').attr('requierd', true);
        }
        else {
            sshDiv.hide();
            sshDiv.find('input').removeAttr('requierd');
        }
    });

    usesSsh.prop('checked', false);
    sshDiv.hide();

    var remotePort = $('input[name="productPort"]');
    remotePort.val('3306');
    remotePort.change(function () {
        var fwport = $('#fwset');
        fwport.html(g_msg1.replace("[1235]", $(this).val()));
    });

    var sshPort = $('input[name="sshPort"]');
    sshPort.val('22');

    $("#selectTemplateError").find(".login-item").click(function () {
        $("#selectTemplateError").dialog('close');
    });

    $("#selectTemplateError2").find(".login-item").click(function () {
        $("#selectTemplateError2").dialog('close');
    });

    $("#contactdlg").find('.info-container').css('border-right', 'none').css('width', 'auto');


    $(".connection-failure-contact").find("a").click(function () {
        var cause = $('.connection-failure-cause').text();
        var msg = 'I am experiencing difficulties connecting ' + ShortProductName + ' to my database and I got the following error message: ' + cause;
        openContactUsWithMessage(msg, '', g_email, '', false);
    });

    $("#contactdlg").find(".login-item").click(function () {
        if ($("#contactdlg").find('#submit').attr('disabled') == 'disabled') {
            $("#contactdlg").dialog('close');
        }
    });

    $('input[name="productPort"]').change(function () { $('span#cnn-instr-pport').html($('input[name="productPort"]').val()); });
    $('#DemoForm').find('input[name="username"]').change(function () { $('span.cnn-instr-username').html($('#DemoForm').find('input[name="username"]').val()); });
}

function openContactUsWithMessage(msg, fullname, email, phone, close){

        $('textarea[name="Comments"]').val(msg);
        $("#contactdlg").find('input[name="Email"]').val(email);
        $("#contactdlg").find('input[name="Phone"]').val(phone);
        $("#contactdlg").find('input[name="FullName"]').val(fullname);
        //$("#contactdlg").find('#submit').removeAttr('disabled');
        if(close==true)
            $("#contactdlg").on( "dialogclose", function() {location.reload();} );
        else
            $("#contactdlg").on( "dialogclose", function() {} );
        $("#contactdlg").dialog('open');
}

Demo.handleMySQL = function (dataSource) {
    var usesSsh = $('input[name="ssh"]');
    var remotePort = $('input[name="productPort"]');
    var sshDiv = $('div.ssh');
    var dbInput = $('input[name="catalog"]');
    var servertooltip = $('input[name="server"]');
    var serverMySqlcontent = servertooltip.attr('data-content-MySql');
    var serverSqlServercontent = servertooltip.attr('data-content-SqlServer');
    var dbInputMySqlcontent = dbInput.attr('data-content-MySql');
    var dbInputSqlServercontent = dbInput.attr('data-content-SqlServer');
    $('span.cnn-instr-username').html('{Username}');
    if (dataSource == 4) {
        usesSsh.show();
        $('.checkfieldLabel').show();
        remotePort.parents('fieldset').show();
        // dbInput.attr('hint','Database Name or Schema');
        //dbInput.removehint();
        //dbInput.hint();
        servertooltip.attr('data-content', serverMySqlcontent);
        dbInput.attr('data-content', dbInputMySqlcontent);
        if (usesSsh.prop('checked')) {
            sshDiv.show();
            sshDiv.find('input').attr('requierd', 'true');
        }
        else {
            sshDiv.hide();
            sshDiv.find('input').removeAttr('requierd');
        }
        $('div[name="MySql-cnn-instr"]').show();
        $('div[name="SqlServer-cnn-instr"]').hide();



    }
    else {
        usesSsh.hide();
        $('.checkfieldLabel').hide();
        remotePort.parents('fieldset').hide();
        // dbInput.attr('hint','Database Name');
        servertooltip.attr('data-content', serverSqlServercontent);
        dbInput.attr('data-content', dbInputSqlServercontent);
        sshDiv.hide();
        sshDiv.find('input').removeAttr('requierd');
        
        $('div[name="MySql-cnn-instr"]').hide();
        $('div[name="SqlServer-cnn-instr"]').show();

    }
}


var firstTimeRestore = true;

Demo.restore = function (tmpl) {
    if (templServerName == demoDefaults[0].server || templServerName == demoDefaults[1].server) {
        templAppName = "";
        templServerName = "";
        templUserName = "";
        templAppTitle = "";
        templCatalog = "";
        templPassword = "";
    }

    $('#DemoForm').find('input[requierd="true"]').each(function () {
        $(this).removeAttr('disabled');
        if (firstTimeRestore) {
            $('#DemoForm').find('input[name="name"]').val(templAppName).removeAttr('disabled');
            firstTimeRestore = false;
        }
        else {
            $('#DemoForm').find('input[name="name"]').removeAttr('disabled');
        }
        $('#DemoForm').find('input[name="server"]').val(templServerName).removeAttr('disabled');
        $('#DemoForm').find('input[name="username"]').val(templUserName).removeAttr('disabled');
        $('#DemoForm').find('input[name="title"]').val(templAppTitle).removeAttr('disabled');
        $('#DemoForm').find('input[name="catalog"]').val(templCatalog).removeAttr('disabled');
        $('#DemoForm').find('input[name="password"]').val(templPassword).removeAttr('disabled');
    });
}
var templAppName = "";
var templServerName = "";
var templUserName = "";
var templAppTitle = "";
var templCatalog = "";
var templPassword = "";

Demo.save = function () {
    templAppName = $('#DemoForm').find('input[name="name"]').val();
    templServerName = $('#DemoForm').find('input[name="server"]').val();
    templUserName = $('#DemoForm').find('input[name="username"]').val();
    //templAppTitle = $('#DemoForm').find('input[name="title"]').val();
    templAppTitle = $('#DemoForm').find('input[name="name"]').val();
    templCatalog = $('#DemoForm').find('input[name="catalog"]').val();
    templPassword = $('#DemoForm').find('input[name="password"]').val();
}

Demo.setDefaults = function (defaults, i) {

    if (defaults) {
        for (var index = 0, len = defaults.length; index < len; ++index) {
            demoDefaults[index] = { server: defaults[index].server, catalog: defaults[index].catalog, username: defaults[index].username, name: defaults[index].name, title: defaults[index].title }
            //            demoDefaults.server = defaults.server;
            //            demoDefaults.catalog = defaults.catalog;
            //            demoDefaults.username = defaults.username;
            //            demoDefaults.name = defaults.name;
            //            demoDefaults.title = defaults.title;
        }
    }

    if (!i)
        i = 0;

    var form = $('#DemoForm');
    form.find('input[name="name"]').val(demoDefaults[i].name);
    form.find('input[name="title"]').val(demoDefaults[i].title);
    form.find('input[name="server"]').val(demoDefaults[i].server);
    form.find('input[name="catalog"]').val(demoDefaults[i].catalog);
    form.find('input[name="username"]').val(demoDefaults[i].username);
    form.find('input[name="password"]').val("something");

    form.find('input[name="name"]').attr('disabled', 'disabled');
    form.find('input[name="title"]').attr('disabled', 'disabled');
    form.find('input[name="server"]').attr('disabled', 'disabled');
    form.find('input[name="catalog"]').attr('disabled', 'disabled');
    form.find('input[name="username"]').attr('disabled', 'disabled');
    form.find('input[name="password"]').attr('disabled', 'disabled');

}

Demo.validateAndSubmit = function (div) {
    var elm = $('#DemoForm');
    Preloader.stop();

    //clear hints
    $('input[hint!=""]').removehint();

    if (Demo.validate(elm, Demo)) {
        DivButton.Disable(div);
        setTimeout(function () {
            Demo.submit(div);
        }, 100);
        //Preloader.stop();
        return false;
    }
    else
        Preloader.stop();
    return false;
}
Demo.getInfo = function () {
    var form = $('#DemoForm');
    return { template: form.find('select[name="template"]').val(),
        name: form.find('input[name="name"]').val(),
        //title: form.find('input[name="title"]').val(),
        title: form.find('input[name="name"]').val(),
        server: form.find('input[name="server"]').val(),
        catalog: form.find('input[name="catalog"]').val(),
        username: form.find('input[name="username"]').val(),
        password: form.find('input[name="password"]').val(),
        usesSsh: form.find('input[name="ssh"]').prop('checked'),
        sshRemoteHost: form.find('input[name="sshRemoteHost"]').val(),
        productPort: form.find('input[name="productPort"]').val(),
        sshPort: form.find('input[name="sshPort"]').val(),
        sshUsername: form.find('input[name="sshUsername"]').val(),
        sshPassword: form.find('input[name="sshPassword"]').val()
    };
}

function preloader(imgSrc) {
    heavyImage = new Image();
    heavyImage.src = imgSrc;
}

Demo.submit = function (div) {
    Demo.hideMessage();

    var image2 = $('<img>');
    var dialog2 = Dialogs.Wait(image2, null, 'auto', 'auto', true);
    image2.attr('src', '/website/images/wait.gif').load(function () {});
    $("body").css("cursor", "progress");
    //return;

    var info = Demo.getInfo();
    var data = info;

    var title = info.template == "0" ? "Demo Console" : info.name;
    var url = '/Website/CreateApp?template=' + info.template + '&name=' + info.name + '&title=' + title + '&server=' + info.server + '&catalog=' + info.catalog + '&username=' + info.username + '&password=' + info.password + '&usingSsh=' + info.usesSsh + '&sshRemoteHost=' + info.sshRemoteHost + '&sshUser=' + info.sshUsername + '&sshPassword=' + info.sshPassword + '&sshPort=' + info.sshPort + '&productPort=' + info.productPort;
    _gaq.push(['_trackEvent', 'Dialogs', 'Demo', 'Demo Submit Request' + info.template.toString()]);

    setTimeout(function () {
        $.ajax({
            url: url,
            contentType: 'application/json; charset=utf-8',
            type: "POST",
            async: true,
            dataType: 'json',
            cache: false,
            error: function () { Demo.message('The server is busy, please try again later.'), Preloader.stop(); DivButton.Enable(div); $("body").css("cursor", "default"); dialog2.dialog("close"); },
            success: function (response) {
                $("body").css("cursor", "default");
                dialog2.dialog("close");

                if (response.Success) {
                    Preloader.stop();
                    _gaq.push(['_trackEvent', 'Dialogs', 'Demo', 'Demo Submit']);
                    _gaq.push(['_trackEvent', 'Dialogs', 'Demo', 'Demo Submit' + info.template.toString()]);
                    var image = $('<img>');
                    var dialog = Dialogs.Wait(image, null, 1020, 620, true);
                    image.attr('src', waitImgSrc).css('width', 950).css('height', 520).load(function () {
                        // window.location = response.Url;
                    });
                    setTimeout(function () {
                        window.location = response.Url;
                    }, 1000);
                }
                else {
                    DivButton.Enable(div);
                    var t = null;
                    if (response && response.TroubleshootInfo && response.TroubleshootInfo.Id) {
                        t = Demo.GetTroubleshootInfo(response.TroubleshootInfo);
                    }
                    if (t) {
                        Demo.MessageTroubleshooting2(t);
                    }
                    else if (response.CnnString) {
                        Demo.MessageTroubleshooting(response.Message, info, response.CnnString, response.port);
                    }
                    else {
                        Demo.message(response.Message)
                    }
                    Preloader.stop();
                }
            }

        });
    }, 100);

    return;
    //submit();

}
Demo.validate = function (elm, classer) {
    var isValid = true;
    $(elm).find('input[requierd="true"]').each(function () {//,textarea[requierd="true"]'
        if (!this.value) {
            classer.handleFieldError(this, this.name, '');
            $('#selectTemplate').dialog('open')
            isValid = false;
        }
    });

    var appName = $(elm).find('input[name="name"]').val();
    if (appName && !isAlphanumeric(appName)) {
        $('#selectTemplate').dialog('open')
        Demo.message('Invalid Application name, should be alphanumeric (no spaces or other chars).');
        isValid = false;
    }
    else {
        $('#DemoForm').find('span.general').hide();
    }
    return isValid

}

Demo.handleFieldError = function (elm, name, msg) {
    var errElm=$(elm).parent('div').find('span.error-message');
    if(msg && msg != '')
        errElm.html(msg).css('display', 'block');
    else
        errElm.css('display', 'block');
}
Demo.message = function (msg) {
    $('#selectTemplate').dialog('open');
    $('#DemoForm').find('span.general').show().html(msg);
}

Demo.hideMessage = function () {
    $('DemoForm').find('span.error-message').hide();
}

Demo.MessageTroubleshooting2 = function (troubleshootInfo) {
    var tsdlg = $('#selectTemplateError2');

    tsdlg.find('.connection-failure-cause').html(troubleshootInfo.cause);
    tsdlg.find('.connection-failure-fix').html(troubleshootInfo.fix);

    tsdlg.dialog('open');
}

Demo.GetTroubleshootInfo = function (troubleshootInfo) {
    switch (troubleshootInfo.Id) {
        case 0:
            return null;
        case 1001:
            var fix = $("<ul></ul>");
            fix.append($("<li></li>").html("1. Make sure that the server name is correctly spelled"));
            fix.append($("<li></li>").html("2. The server is up and running"));
            fix.append($("<li></li>").html("3. The server allow remote access"));
            return { cause: troubleshootInfo.Cause, fix: fix };
        case 1002:
            var fix = $("<ul></ul>");
            fix.append($("<li></li>").html("1. " + troubleshootInfo.Fix));
            fix.append($("<li></li>").html("2. The server is up and running"));
            fix.append($("<li></li>").html("3. The server allow remote access"));
            return { cause: troubleshootInfo.Cause, fix: fix };
        case 1003:
            var fix = $("<ul></ul>");
            fix.append($("<li></li>").html("1. Please make sure that the database name is correctly spelled."));
            fix.append($("<li></li>").html("2. " + troubleshootInfo.Fix));
            return { cause: troubleshootInfo.Cause, fix: fix };
        case 1004:
            var fix = $("<ul></ul>");
            fix.append($("<li></li>").html("1. Please make sure that the username and password are correctly spelled."));
            fix.append($("<li></li>").html("2. " + troubleshootInfo.Fix));
            return { cause: troubleshootInfo.Cause, fix: fix };
        case 2001:
            var fix = $("<ul></ul>");
            fix.append($("<li></li>").html("1. Make sure that the server name is correctly spelled"));
            fix.append($("<li></li>").html("2. The server is up and running"));
            fix.append($("<li></li>").html("3. The server allow remote access"));
            return { cause: troubleshootInfo.Cause, fix: fix };
        case 2002:
            var fix = $("<ul></ul>");
            fix.append($("<li></li>").html("1. " + troubleshootInfo.Fix));
            fix.append($("<li></li>").html("2. The server is up and running"));
            fix.append($("<li></li>").html("3. The server allow remote access"));
            return { cause: troubleshootInfo.Cause, fix: fix };
        case 3001:
            var fix = $("<ul></ul>");
            fix.append($("<li></li>").html(troubleshootInfo.Fix));
            return { cause: troubleshootInfo.Cause, fix: fix };
        case 3002:
            var fix = $("<ul></ul>");
            fix.append($("<li></li>").html(troubleshootInfo.Fix));
            return { cause: troubleshootInfo.Cause, fix: fix };
        default:
            return null;
    }
}

Demo.MessageTroubleshooting = function (msg, info, cnnstr, port) {
    var tsdlg = $('#selectTemplateError');
    var msg3 = msg;
    if (msg.length > errMsgMaxLength) {

        var msg1 = msg.substring(0, errMsgMaxLength - 1);
        var lastspace = msg1.lastIndexOf(' ');
        msg1 = msg.substring(0, lastspace);
        var msg2 = msg.substring(lastspace, msg.length - 1);
        msg3 = msg1 + ' ' + "<span class='clickMe'>Read more...</span>"
    }
    tsdlg.find("#cnnErrMsg").html(msg3);
    $("#cnnErrMsg").find('.clickMe').click(function () {
        //        $(this).siblings("div.supportUnfold:first").slideToggle('slow', function () {
        //        });
        tsdlg.find("#cnnErrMsg").html(msg);
    });
    if(info.template != 4) // SQL Server it's always 1433
        port = '1433';

    if (!port) port = info.template == 4 ? '3306' : '1433';
    tsdlg.find("#cnnPort").html(port);
    if (cnnstr) {
        var splitCs = cnnstr.split(';');
        if (splitCs.length > 2) {
            cnnstr = splitCs[0] + ";" + splitCs[1] + ";" + '<br>';
            for (var i = 2; i < splitCs.length; i++) {
                cnnstr += splitCs[i] + ";";
            }
        }
        tsdlg.find("#cnnString").html(cnnstr);
    }

    tsdlg.dialog('open');
}

var Track; if (!Track) Track = {};

Track.lostFocus = function(){
    var info= SignUp.getInfo();
    var usernameHint=$('#dialog').find('input[name="username"]').attr('hint');
    var fullnameHint=$('#dialog').find('input[name="fullname"]').attr('hint');
    var phoneHint=$('#dialog').find('input[name="phonenumber"]').attr('hint');
    var data = { username:info.username==usernameHint?'':info.username ,phone:info.phone==phoneHint?'':info.phone,fullname:info.fullname==fullnameHint?'':info.fullname,dbtype:info.dbtype,dbother:info.dbother};
   
    
    var url = '/WebsiteAccount/UpdateUserTracking';
    $.ajax({
        url: url,
        //contentType: 'application/json; charset=utf-8',
        data: data,
        type: "Post",
        async: true,
        dataType: 'json',
        cache: false,
        error: function () { ContactUs.message('The server is busy, please try again later.') },
        success: function (valid) {
        }
    });
    return;
}
    
var Action; if (!Action) Action = {};

Action.submit = function (actionId) {
    
    var data = { email: g_email,comments:'',actionId:actionId };
    
    var url = '/WebsiteAccount/InsertUserAction'; //?from=' + info.username + '&password=' + info.password;
    
    $.ajax({
        url: url,
        //contentType: 'application/json; charset=utf-8',
        data: data,
        type: "Post",
        async: true,
        dataType: 'json',
        cache: false,
        error: function () { ContactUs.message('The server is busy, please try again later.') },
        success: function (valid) {
          
        }

    });

    return;
    //submit();

}


var ContactUs; if (!ContactUs) ContactUs = {};

ContactUs.getInfo = function () {
    return { 
        FullName: $('input[name="FullName"]').val(),
        Email: $('input[name="Email"]').val(),
        //Website: $('input[name="Website"]').val(), 
        Phone: $('input[name="Phone"]').val(),
        RequestSubject: $('select[name="requeste_dpt"] option:selected').html(),
        RequestSubjectId: $('select[name="requeste_dpt"] option:selected').val(),
        Comments: $('textarea[name="Comments"]').val()
    };
}

ContactUs.getMessage = function (info) {
    return '<table><tr><td>Subject:</td><td>' + info.RequestSubject + '</td></tr><tr><td>Full Name:</td><td>' + info.FullName + '</td></tr><tr><td>Email:</td><td>' + info.Email + '</td></tr><tr><td>Phone:</td><td>' + info.Phone + '</td></tr><tr><td>Comments:</td><td>' + info.Comments + '</td></tr></table>';
}

ContactUs.init = function () {
    generalSignInError = $('#signIn').find('span.general');
    $("#ContactUsInfo").find('span.general').html("");
    $('input', 'textarea').blur(function () {
        $(this).next('span.error-message').hide();
        generalSignInError.hide();
        DivButton.Enable($('#submit'));
    });

    $('#submit').click(function () {
        var div = $(this);
        if (!DivButton.IsDisabled(div)) {
            if (ContactUs.validate()) {
                DivButton.Disable(div);
                ContactUs.submit();
            }
        }
        return false;

    });
}
ContactUs.validate = function () {
    var isValid = true;
    $('#ContactUsInfo').find('input[requierd="true"]').each(function () {//,textarea[requierd="true"]'
        if (!this.value) {
            ContactUs.handleFieldError(this.name, "Data Required");
            isValid = false;
        }
    });
    //var msg = "";
//    var url = $('input[name="Website"]').val();
//    if (url && !isValidUrl(url)) {
//        ContactUs.handleFieldError('Website', 'Invalid Url');
//        isValid = false;
//    }

    var email = $('#ContactUsInfo').find('input[name="Email"]').val();
    if (email && !isValidEmail(email)) {
        //if (msg) msg += ', ';
        ContactUs.handleFieldError("Email", 'Invalid Email');
        isValid = false;
    }
//    if (msg)
//        ContactUs.handleError(msg);
    return isValid

}
isAlphanumeric = function (appName) {
    var regexp = /^[a-zA-Z0-9]+$/
    return regexp.test(appName);
}
isValidUrl = function (url) {
    var regexp = /(ftp|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/
    
       // var regexp = /(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/
        return regexp.test(url);
    
}
isValidEmail = function (email) {

        var regexp = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
        return regexp.test(email);
}
ContactUs.handleFieldError=function (name, msg) {
    $('[name="' + name + '"]').next().html(msg).css('display', 'block');
}
ContactUs.handleError = function (message) {
    if (message) {
        ContactUs.message(message);
    }
}
ContactUs.submit = function () {
    var info = ContactUs.getInfo();
    var data = { from: info.Email, to: 'team@' + LongProductName + '.com', cc: '', message: ContactUs.getMessage(info), subject: ShortProductName + ' Contact Us', name: info.FullName, comments: info.Comments, phone: info.Phone, RequestSubjectId: info.RequestSubjectId };
    var url = '/WebsiteAccount/ContactUs'; //?from=' + info.username + '&password=' + info.password;
    
    $.ajax({
        url: url,
        //contentType: 'application/json; charset=utf-8',
        data: data,
        type: "Post",
        async: false,
        dataType: 'json',
        cache: false,
        error: function () { ContactUs.message('The server is busy, please try again later.') },
        success: function (valid) {
            if (valid)
                $("#ContactUsInfo").find('div.success').html('Your message was sent. Thank you for your interest in ' + ShortProductName + '.').css("color", "#31B94D").show();
            else
                ContactUs.message('The server is busy, please try again later.')
        }

    });

    return;
    //submit();

}

ContactUs.message = function (msg) {
   
    $("#ContactUsInfo").find('span.general').html(msg).show();
}

var generalSignInError = null;


var SignIn; if (!SignIn) SignIn = {};

SignIn.init = function () {
    generalSignInError = $('#signIn').find('span.general');

    $('input, select.styled').blur(function () {
        $(this).parent('div').find('span.error-message').hide();
        //$(this).next('span.error-message').hide();
        generalSignInError.hide();
    });

    $('#signIn').find('div.login').click(function () {
        var div = $(this);
        if (!DivButton.IsDisabled(div)) {

            SignIn.validateAndSubmit(div);
        }
    });

    $('#signIn').keyup(function (e) {
        if (e.keyCode == 13) {
            var div = $(this).find('div.login');
            SignIn.validateAndSubmit(div);
        }
    });


    $('.sign-in').click(function () {
        gotoApps = true;
        _gaq.push(['_trackEvent', 'Dialogs', 'login', 'open login dialog']);
        if (SignUp.isLoggedin()) {
            Preloader.start();
            window.location = '/apps';
        }
        else {
            $('#signIn').dialog('open');
            StopAutoImages();
            $(".userName").focus();

        }
        //        setTimeout(function () {
        //            $('[name="userName"]').focus();
        //        }, 1);
    });
}

SignIn.url = function () {
    return g_mainsite;
}

SignIn.getInfo = function () {
    var form = $('#signIn').find('form');
    return { form: form, username: form.find('input[name="userName"]').val(), password: form.find('input[name="password"]').val() };
}

var gotoApps = true;

SignIn.submit = function () {
    var info = SignIn.getInfo();
    var f = info.form;
    //var action = SignIn.url();
    var action = "/Account/LogOn?rememberMe=true&userName=" + info.username + "&password=" + info.password;
    //    f.attr("action", action);

    //    f.submit();

    url = action;

    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        //data: data,
        type: "POST",
        async: true,
        dataType: 'html',
        cache: false,
        error: function () { SignIn.handleError('The server is busy, please try again later.') },
        success: function () {
            if (gotoApps)
                window.location = '/apps';
            else {
                $('#signIn').dialog('close');
                if (!SignUp.isLoggedin()) {
                    $('#dialog').dialog('open');
                }
                else {
                    SignUp.reg();
                    //$('#selectTemplate').dialog('open');
                }
            }
        }

    });
}

SignIn.handleError = function (message) {
    //$('#signIn').dialog('open');
    if (message)
        generalSignInError.text(message);
    generalSignInError.show();
}

SignIn.handleFieldError = function (name) {
    $('[name="' + name + '"]').next().css('display', 'block');
}

SignIn.validateAndSubmit = function (div) {
    //if (gotoApps)
        //Preloader.start();
    var info = SignIn.getInfo();
    if (!info.username) {
        SignIn.handleFieldError('userName');
        //$('#signIn').dialog('open');
        Preloader.stop();
        return;
    }
    if (!info.password) {
        SignIn.handleFieldError('password');
        //$('#signIn').dialog('open');
        Preloader.stop();
        return;
    }

    DivButton.Disable(div);

    setTimeout(function () {
        SignIn.validate(SignIn.submit, SignIn.handleError, div);
    }, 100);
}

SignIn.validate = function (submit, handleError, div) {
    var url = SignIn.url();
    var info = SignIn.getInfo();
    var data = { userName: info.username, password: info.password };

    url = '/Account/ValidateAuthentication?userName=' + info.username + '&password=' + info.password;

    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        //data: data,
        type: "POST",
        async: true,
        dataType: 'json',
        cache: false,
        error: function () { handleError('The server is busy, please try again later.'); Preloader.stop(); },
        success: function (valid) {
            if (valid) {
                submit();
                //$('#signIn').dialog('close')
            }
            else {
                DivButton.Enable(div);
                handleError('Incorrect username or password');
                Preloader.stop();
            }
        }

    });

    return;
    //submit();

}
Preloader.init = function (prependTo, transitions) {
    var sequenceLoadWaitMessage = "Sequence is loading, please wait..."
    var prefix = '';
    var opacity = (transitions) ? 0 : 1;
    if ($("style:contains('.sequence-preloader')").length < 1)
        $("head").append("<style>.sequence-preloader{height: 100%;position: absolute;width: 100%;z-index: 999999;}@" + prefix + "keyframes preload{0%{opacity: 0;}50%{opacity: 1;}100%{opacity: 0;}}.sequence-preloader img{background: #ff9933;border-radius: 6px;display: inline-block;height: 12px;opacity: " + opacity + ";position: relative;top: -50%;width: 12px;" + prefix + "animation: preload 1s infinite; animation: preload 1s infinite;}.preloading{height: 12px;margin: 0 auto;top: 50%;position: relative;width: 48px;}.sequence-preloader img:nth-child(2){" + prefix + "animation-delay: .15s; animation-delay: .15s;}.sequence-preloader img:nth-child(3){" + prefix + "animation-delay: .3s; animation-delay: .3s;}.preloading-complete{opacity: 0;visibility: hidden;" + prefix + "transition-duration: 1s; transition-duration: 1s;}</style>");
    if ($(prependTo).find(".sequence-preloader").length < 1)
        $(prependTo).prepend('<div class="sequence-preloader"><div class="preloading"><img src="website/images/sequence-preloader.png" alt="'+ sequenceLoadWaitMessage +'" />    <img src="website/images/sequence-preloader.png" alt="Sequence is loading, please wait..." />    <img src="website/images/sequence-preloader.png" alt="Sequence is loading, please wait..." /></div></div>');
}

Preloader.start = function () { //if transitions aren't supported, call this fallback to show the default preloading animations
    Preloader.init("#loader_sequence", 1);
    Preloader.oldZIndex = $(".main-loader").css('z-index');
    $(".main-loader").css('z-index', 99999);
    var self = this, i = 0;
    function preload() {
        i = (i === 1) ? 0 : 1;
        $("#loader_sequence").find(".sequence-preloader img:nth-child(1)").animate({ "opacity": i }, 100);
        $("#loader_sequence").find(".sequence-preloader img:nth-child(2)").animate({ "opacity": i }, 350);
        $("#loader_sequence").find(".sequence-preloader img:nth-child(3)").animate({ "opacity": i }, 600);
    }
    Preloader.refreshIntervalId = setInterval(preload, 600);
    $(".main-logo").show();
}
Preloader.stop = function () {
    clearInterval(Preloader.refreshIntervalId);
    //$("#empty-sequence").find("sequence-preloader").remove();
    $("#loader_sequence").find(".sequence-preloader").remove();
    $(".main-loader").css('z-index', Preloader.oldZIndex);
    $(".main-logo").hide();

}

var Forgot; if (!Forgot) Forgot = {};

Forgot.init = function () {

    $('div.reset').click(function () {
        Forgot.send();
    });

    $('div.resetApproval').click(function () {
        $('#resetPasswordApproval').dialog('close');
    });
}

Forgot.resetPassword = function () {

    var loginForm = document.getElementById("signIn").getElementsByTagName("form")[0];
    var username = loginForm.elements["userName"].value;

    $('#resetPassword').find('input[name="userNameForReset"]').val(username);

    $('div.reset').removeAttr('disabled');
    var g = $('#resetPassword').find('span.general');
    g.hide();
    $('#resetPassword').dialog('open');
    $('#signIn').dialog('close');

//    var loginForm = document.getElementById("signIn").getElementsByTagName("form")[0];
//    var username = loginForm.elements["userName"].value;
//    window.location = '/Account/PasswordReset?username=' + username;

}

Forgot.send = function () {
    var username = $('#resetPassword').find('input[name="userNameForReset"]').val();
    var g = $('#resetPassword').find('span.general');
    g.hide();

    if (username == '') {
        Forgot.showFieldError('userNameForReset');
        return;
    }
    else {
        Forgot.hideFieldError('userNameForReset');
        $('div.reset').attr('disabled', 'disabled');

    }

    var url = '/Account/PasswordReset?username=' + username;
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        //data: data,
        type: "POST",
        async: true,
        dataType: 'json',
        cache: false,
        error: function () { handleError('The server is busy, please try again later.'); Preloader.stop(); },
        success: function (valid) {
            if (valid.error != "success") {
                g.text(valid.message);
                g.show();
                $('div.reset').removeAttr('disabled');
            }
            else {
                $('#resetPasswordApproval').dialog('open');
                $('#resetPassword').dialog('close');
    
            }
        }

    });

    return;
}

Forgot.showFieldError = function (name) {
    $('[name="' + name + '"]').next().css('display', 'block');
}

Forgot.hideFieldError = function (name) {
    $('[name="' + name + '"]').next().hide();
}

Dialogs =
{
    Wait: function (element, title, width, height, autoOpen) {

        var dialog = $('<div></div>').appendTo('body')
          .append(element)
          .dialog({
              modal: true, title: title, zIndex: 1000000, autoOpen: autoOpen,
              width: width ? width : 'auto', height: height ? height : 'auto', position: ['center', 'center'], resizable: false, open: function () {
                  if (!title)
                      $(this).dialog("widget").find(".ui-dialog-titlebar").hide();
              }

          });
        return dialog;
    }
}



jQuery.fn.hint = function (blurClass) {
    if (!blurClass) {
        blurClass = 'blur';
    }

    return this.each(function () {
        // get jQuery version of 'this'
        var $input = jQuery(this),

        // capture the rest of the variable to allow for reuse
        title = $input.attr('hint');
        //$form = jQuery(this.form),
        //$win = jQuery(window);

        function remove() {
            if ($input.val() === title && $input.hasClass(blurClass)) {
                $input.val('').removeClass(blurClass);
            }
        }

        // only apply logic if the element has the attribute
        if (title) {
            // on blur, set value to title attr if text is blank
            $input.blur(function () {
                if (this.value === '') {
                    $input.val(title).addClass(blurClass);
                    $input.keyup();
                }
            }).focus(remove).blur(); // now change all inputs to title

            // clear the pre-defined text when form is submitted
            //$form.submit(remove);
            //$win.unload(remove); // handles Firefox's autocomplete
        }
    });
};

jQuery.fn.removehint = function (blurClass) {
    if (!blurClass) {
        blurClass = 'blur';
    }

    return this.each(function () {
        // get jQuery version of 'this'
        var $input = jQuery(this),

        // capture the rest of the variable to allow for reuse
        title = $input.attr('hint');
        //$form = jQuery(this.form),
        //$win = jQuery(window);

        if (title) {
            if ($input.val() === title && $input.hasClass(blurClass)) {
                $input.val('').removeClass(blurClass);
                $input.keyup();
            }
        }
    });
};
