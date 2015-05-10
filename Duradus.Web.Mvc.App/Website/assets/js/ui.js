// JavaScript Document
var Preloader = {};
var oldTempl= null;
var waitImgSrc = '/website/assets/images/addviewswait.gif';
var errMsgMaxLength=80;
var ShortProductName = 'Back&';

var ProductNumber = { SQLServer: 2, SQLAzure: 3, MySQL: 4, Postgresql: 8, MongoDB: 6, Oracle: 7 };

$(document).ready(function () {
    preloader(waitImgSrc);
    preloader('/website/assets/images/waiting-animation.gif');
    preloader('/website/assets/images/slider/video.jpg');
    preloader('/website/assets/images/slider/video_over.jpg');

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


        paused: function () {
            $status.animate({ "opacity": 0 }, 500, function () {
                $status.removeClass("active").addClass("paused");
            });
        },
        unpaused: function () {
            $status.removeClass("active paused");
        }

    };

    $('#login-content, #forgot-content, #password-approval-content').dialog(
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
		    open: function () {
		        $('.ui-widget-overlay').css('height', $('body').height() + 'px');
		    }
		    //		    open: function () { setTimeout(function () { $('[name="userName"]').focus(); }, 100); }
		});

    $('#trynow-content').dialog(
	{
	    modal: true,
	    draggable: false,
	    resizable: false,
	    autoOpen: false,
	    hide: { effect: 'fade', duration: 100 },
	    show: { effect: 'fade', duration: 200 },
	    closeText: '',
	    width: '740px',
	    stack: false,
	    position: ["center", 100],
	    open: function () {
	        $('.ui-widget-overlay').css('height', $('body').height() + 'px');
	    }
	});


    $('#create-content').dialog(
		{
		    modal: true,
		    draggable: false,
		    resizable: false,
		    autoOpen: false,
		    hide: { effect: 'fade', duration: 100 },
		    show: { effect: 'fade', duration: 200 },
		    closeText: '',
		    width: '920px',
		    stack: false,
		    position: ["center", 100],
		    close: function () { oldTempl = null; if (createDialogReload) { location.reload(); } else { createDialogReload = true; } },
		    open: function () {
		        $('.ui-widget-overlay').css('height', $('body').height() + 'px');
		    }
		});

    $('#tutorial, #mainhowtovideo').dialog(
		{
		    modal: true,
		    draggable: false,
		    resizable: false,
		    autoOpen: false,
		    hide: { effect: 'fade', duration: 1 },
		    show: { effect: 'fade', duration: 200 },
		    closeText: '',
		    width: 900,
		    stack: false,
		    position: ["center", 100],
		    close: function () { $('#video').html(''); $('.change-sequence').show(); },
		    open: function () {
		        $('.ui-widget-overlay').css('height', $('body').height() + 'px');
		    }
		});

    $('#selectTemplateError, #selectTemplateError2').dialog(
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
		    close: function () { $('#create-content').dialog('open'); }
		});

    $(' #contactdlg').dialog(
		{
		    modal: true,
		    draggable: false,
		    resizable: false,
		    autoOpen: false,
		    hide: { effect: 'fade', duration: 100 },
		    show: { effect: 'fade', duration: 200 },
		    closeText: '',
		    width: '940',
		    stack: false,
		    position: ["center", 100],
		    open: function () {
		        $('.ui-widget-overlay').css('height', $('body').height() + 'px');
		    }
		});


    $('.ui-dialog-titlebar-close').click(function () {
        $("#slider").show();
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


    $('#trynow-content').find('select[name="databasetype"]').change(function () {
        var db = $(this).val();
        if (db == 0)
            $(this).parent().addClass('blur');
        else
            $(this).parent().removeClass('blur');
        if (db == 9) {
            $('#trynow-content').find('input[name="dbother"]').parents('fieldset:first').show();
            $('#trynow-content').find('input[name="dbother"]').attr('requierd', true);
        }
        else {
            $('#trynow-content').find('input[name="dbother"]').parents('fieldset:first').hide();
            $('#trynow-content').find('input[name="dbother"]').removeAttr('requierd');
        }

    });

    // List box
    //$('select.styled').customStyle();

    // Tips
    //    $('.info').tipsy({
    //        delayIn: 0,      // delay before showing tooltip (ms)
    //        delayOut: 0,     // delay before hiding tooltip (ms)
    //        fade: true,     // fade tooltips in/out?
    //        fallback: '',    // fallback text to use when no tooltip text
    //        gravity: 's',    // gravity
    //        html: true,     // is tooltip content HTML?
    //        live: true,     // use live event support?
    //        offset: 5,       // pixel offset of tooltip from element
    //        opacity: 1,    // opacity of tooltip
    //        trigger: 'hover', // how tooltip is triggered - hover | focus | manual
    //        title: function () { return $(this).next('.toolTip').html(); }
    //    });

    //send email in contact
    ContactUs.init();
    Demo.init();
    SignIn.init();
    SignUp.init();
    Tutorial.init();
    Forgot.init();
    Video.init();

    HandleSupport();

    HandleTootips();

    ClickToCall.init();

    HandleRequest();

    InitHoverImages();

});


//function barAnimation() {
//    //run only every 5 times in random
//    if (Math.floor(Math.random() * 5) == 1) {
//        $('.slider').css({ opacity: '0' });
//        $('.slider').animate({ opacity: "0.5", left: "95%" }, '1000', 'jswing');
//        $('.slider').animate({ opacity: "0", left: "50px" }, 'slow');
//    }
//}

function Alert(title, message) {
    if (!title)
        title = "Back& System";

    if (!message)
        message = "";

    var sMsgAlert = '<div class="dialog-header"><div class="sub-title">';
    sMsgAlert += title ;
    sMsgAlert += '</div></div>' ;
    sMsgAlert += '<div class="confirm-message">' + message + '</div>' ;
    
    $('<div></div>').appendTo('body').html(sMsgAlert)
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

    $('#Click2Call').dialog(
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

function InitHoverImages() {
    ImageHover('content1', '/website/assets/images/contents/content-1.jpg', '/website/assets/images/contents/content-1-over.jpg');
    ImageHover('content2', '/website/assets/images/contents/content-2.jpg', '/website/assets/images/contents/content-2-over.jpg');
    ImageHover('content3', '/website/assets/images/contents/content-3.jpg', '/website/assets/images/contents/content-3-over.jpg');
    ImageHover('content4', '/website/assets/images/contents/content-4.jpg', '/website/assets/images/contents/content-4-over.jpg');
    ImageHover('content5', '/website/assets/images/contents/content-5.jpg', '/website/assets/images/contents/content-5-over.jpg');
    ImageHover('content6', '/website/assets/images/contents/content-6.jpg', '/website/assets/images/contents/content-6-over.jpg');
    ImageHover('content7', '/website/assets/images/contents/content-7.jpg', '/website/assets/images/contents/content-7-over.jpg');
    ImageHover('content8', '/website/assets/images/contents/content-8.jpg', '/website/assets/images/contents/content-8-over.jpg');

}

function ImageHover(id, img, hover) {
    $("#" + id).hover(function () {
        $(this).attr("src", hover);
    }, function () {
        $(this).attr("src", img);
    });
}

function HandleTootips(){

    $('[data-toggle=popover]').popover(
    {
        trigger: 'hover',
        animation: true
    }
    );  
    
    
}


function HandleSupport() {
    $('.clickMe').click(function () {
        //alert($(this).siblings("div.supportUnfold").name);
        $(this).siblings(".supportUnfold:first").slideToggle('slow', function () {
            // Animation complete.
        });
    });

}

function HandleRequest() {
    var dataSource = queryString('ty');
    if (dataSource) {
        _gaq.push(['_trackEvent', 'Dialogs', 'Shortcut ty', 'website open with ty' + dataSource.toString()]);
        defaultDataSource = dataSource.substring(0, 1);
        if (defaultDataSource == '0') //if it's a demo of northwind
            SignUp.reg();
        else
            $('.btn-yellow').click();
    }
    var consoleType = queryString('ct');
    if (consoleType)
        defaultConsoleType = consoleType.substring(0, 1);
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
    $("#slider").hide();
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
    $(".a_video").click(function () {
        _gaq.push(['_trackEvent', 'Videos', 'Play', 'main video']);
        $('#video').html('<iframe width="853" height="480" src="https://www.youtube.com/embed/Wk4PVT4MzxA?rel=0&autoplay=1" frameborder="0"></iframe>');
        $('#tutorial .dialog-header .title').html('Back& - Creating Better Back-Office');
        $('#tutorial').dialog('open');
        StopAutoImages();
    });

}

var Video; if (!Video) Video = {};

Video.init = function () {
    $(".howtovideo").each(function () {
        try{
            var mainDiv = $(this);
            var videoId = mainDiv.attr('id');
            var videoData = Video.data(videoId);
            mainDiv.find('.v_video_thumb_img').attr('src', 'http://img.youtube.com/vi/' + videoId + '/mqdefault.jpg');
            mainDiv.find('.v_video_title').html('<p>' + videoData.title + '</p>');
            mainDiv.find('.v_box_wrapper').click(function (e) {
                Video.open(videoId, videoData.title);
            });
            mainDiv.find('.v_desc_video_name').html(videoData.title);
            mainDiv.find('.v_desc_video_desc').html(videoData.description);
            var seconds = String(videoData.seconds % 60);
            mainDiv.find('.v_desc_length').html(String(parseInt(videoData.seconds / 60, 0)) + ':' + ((seconds.length == 1) ? '0' + seconds : seconds));
        }
        catch (err) { }
        });

    //run video based on vid
    if (typeof(g_videoid) != 'undefined' && g_videoid != '')
        setTimeout(function () { Video.open(g_videoid); },1000);

}

Video.open = function (id, title) {

    try{
        if (title == undefined || title == "") {
            title = Video.data(id).title;
        }

        _gaq.push(['_trackEvent', 'Videos', 'Play', 'how to video ' + title]);
        $('#video').html('<iframe width="853" height="480" src="https://www.youtube.com/embed/' + id + '?rel=0&autoplay=1" frameborder="0"></iframe>');
        $('#tutorial .dialog-header .title').html(title);
        $('#tutorial').dialog('open'); //
    }
    catch (err) { }
}

Video.data = function (id) {
    var jsonData;

    $.ajax({
        type: 'GET',
        url: 'https://gdata.youtube.com/feeds/api/videos/' + id + '?format=5&alt=json',
        dataType: 'json',
        success: function (data) {
            jsonData = data 
        },
        data: {},
        async: false
    });

    return {
        title: jsonData.entry.title.$t,
        description: jsonData.entry.content.$t,
        seconds: jsonData.entry.media$group.yt$duration.seconds
    }

}

var SignUp; if (!SignUp) SignUp = {};

SignUp.init = function () {
    $('.btn-yellow').click(function () {
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

    $('.a_login').click(function () {
        $('#trynow-content').dialog('close');
        $('#login-content').dialog('open');
    });
    $('.a_signup').click(function () {
        $('.btn-yellow').click();
    });

//    $('#trynow-content').find('input,select[name="databasetype"]').bind('change', function () {
//        Track.lostFocus()
//    });

    $('#trynow-content').find('input[name="dbother"]').parents('fieldset:first').hide();
}

var defaultDataSource = '4'; //MySQL
var defaultConsoleType = '1' //Console Databse

function getDefaultDataSource(){
//    var dataSrc = $('#trynow-content select[name="databasetype"]').val();
//    if (dataSrc == 2 || dataSrc == 3 || dataSrc == 4)
//        return dataSrc.toString();
    return defaultDataSource;
}
 

SignUp.reg = function () {
    gotoApps = false;
    StopAutoImages();
    if (!SignUp.isLoggedin()) {
        $('#trynow-content').dialog('open');
    }
    else {
        if (isTry == true) {
            isTry = false;
            $('#create-content').find('.rr input[value=' + getDefaultDataSource() + ']').prop('checked', true).change(); 
            $('#create-content').find('input[required="true"]').each(function () { $(this).val(''); });
        }
        else
            $('#create-content').find('.rr input[value=0]').prop('checked', true).change();

        //set defaults
        if(defaultConsoleType==1)
            $('#create-content').find('input[id="consoletype1"]').prop('checked', true).change();
        else
            $('#create-content').find('input[id="consoletype2"]').prop('checked', true).change();
        $('#create-content').find('input[id="startmode1"]').prop('checked', true).change();
        //open the dialog
        $('#create-content').dialog('open');

    }
    //StopAutoImages();
    // find all the input elements with title attributes
    $('input[hint!=""], textarea[hint!=""]').hint();
}


SignUp.next = function (div) {
    var elm = $('#trynow-content');
    //Preloader.start();
    
    if (SignUp.validate(elm, SignUp)) {
        DivButton.Disable(div);

        Progress.show();
        $('#trynow-content').dialog('close');
        setTimeout(function () {
            SignUp.submit(div);
        }, 100);
    }
    //Preloader.stop();
}

SignUp.getInfo = function () {
    var form = $('#trynow-content');
    var phoneElm = form.find('input[name="phonenumber"]');
    var phoneVal = phoneElm.val() == phoneElm.attr('hint') ? "":phoneElm.val();
    return { username: form.find('input[name="username"]').val(), password: form.find('input[name="password"]').val(), phone: phoneVal, fullname: form.find('input[name="fullname"]').val(), dbtype: 100, dbother: form.find('input[name="dbother"]').val() };
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
            Progress.hide();
            $('#trynow-content').dialog('open');
            if (response.Success) {
                if (info.dbtype == 10) {
                    openContactUsWithMessage("I don't have a database but I would like to use " + ShortProductName + ", please contact me.", info.fullname, info.username, info.phone, true);
                    return;
                }
                if (info.dbtype == 9) {
                    openContactUsWithMessage("I don't use MySQL or SQL Server (I use " + info.dbother + "), please contact me when you are going to support addtional databases.", info.fullname, info.username, info.phone, true);
                    return;
                }
                Demo.setDefaults(response.DemoDefaults, 0);
                _gaq.push(['_trackEvent', 'Dialogs', 'SignUp', 'SignUp Submit']);
                $('#trynow-content').dialog('close');
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
            $('#trynow-content').dialog('open')
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
    $('#trynow-content').find('span.error-message').hide();
}

SignUp.message = function (msg) {
    $('#trynow-content').find('span.general').show().html(msg);
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

    $('#DemoForm').find('[name=consoletype]').change(function () {
        var type = $('[name=consoletype]:checked').val();
        if (type == '1') {
            $('#createnewconsole').hide();
            $('#databseconsole').show();
            $('#databaseinstructions').show();
            $('#blankinstructions').hide();
            $('#demo1').css('position','absolute');

        }
        else {
            $('#createnewconsole').show();
            $('#databseconsole').hide();
            $('#databaseinstructions').hide();
            $('#blankinstructions').show();
            $('#demo1').css('position', 'static');
        }

    });

    $('#DemoForm').find('[name=template]').change(function () {
        var tmpl = $('[name=template]:checked').val();
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

        //move the select image
        if (tmpl == ProductNumber.MySQL || tmpl == ProductNumber.Postgresql)
            $('.create-main-top').css('background-position', '160px 10px');
        else if (tmpl == ProductNumber.SQLServer || tmpl == 0 || tmpl == ProductNumber.MongoDB)
            $('.create-main-top').css('background-position', '310px 10px');
        else if (tmpl == ProductNumber.SQLAzure || tmpl == ProductNumber.Oracle)
            $('.create-main-top').css('background-position', '440px 10px');
    });

    $('.submit-create').click(function () {
        var div = $(this);
        if (!DivButton.IsDisabled(div)) {
            Demo.validateAndSubmit(div);
        }
    });

    $('#create-content').keyup(function (e) {
        if (e.keyCode == 13) {
            $('.submit-create').click();
        }
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

    //$("#contactdlg").find('.info-container').css('border-right', 'none').css('width', '940');

    $(".connection-failure-contact").find("a").click(function () {
        var cause = $('.connection-failure-cause').text();
        var msg = 'I am experiencing difficulties connecting ' + ShortProductName + ' to my database and I got the following error message: ' + cause;
        openContactUsWithMessage(msg, '', g_email, '', false);
    });

    $("#contactdlg").find(".login-item").click(function () {
        if ($("#contactdlg").find('button.btn-submit').attr('disabled') == 'disabled') {
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
            $("#contactdlg").on("dialogclose", function () { $('#create-content').dialog('open'); });

        createDialogReload = false;
        $('#create-content').dialog('close');
        $("#contactdlg").dialog('open');
}

Demo.handleMySQL = function (dataSource) {

    var usesSsh = $('input[name="ssh"]');
    var usesSsl = $('input[name="ssl"]');
    var remotePort = $('input[name="productPort"]');
    var sshDiv = $('div.ssh');
    var dbInput = $('input[name="catalog"]');
    var servertooltip = $('input[name="server"]');
    var serverMySqlcontent = servertooltip.attr('data-content-MySql');
    var serverSqlServercontent = servertooltip.attr('data-content-SqlServer');
    var dbInputMySqlcontent = dbInput.attr('data-content-MySql');
    var dbInputSqlServercontent = dbInput.attr('data-content-SqlServer');
    $('span.cnn-instr-username').html('{Username}');
    if (dataSource == ProductNumber.MySQL || dataSource == ProductNumber.Postgresql) {
        usesSsh.parent().show();
        if (dataSource == ProductNumber.Postgresql) {
            usesSsl.next().css('visibility', 'visible');
        }
        else {
            usesSsl.next().css('visibility', 'hidden');
        }
        $('.checkfieldLabel').show();
        remotePort.parents('.mid').show();
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
        if (dataSource == 4) {
            $('div[id="MySql-cnn-instr"]').show();
            $('div[id="postgresql-cnn-instr"]').hide();
        }
        else {
            $('div[id="postgresql-cnn-instr"]').show();
            $('div[id="MySql-cnn-instr"]').hide();
        }
        $('div[id="SqlServer-cnn-instr"]').hide();
        $('div[id="mongodb-cnn-instr"]').hide();
        $('div[id="oracle-cnn-instr"]').hide();
        if (dataSource == ProductNumber.MySQL && remotePort.val() == '5432') {
            remotePort.val('3306');
        }
        else if (dataSource == ProductNumber.Postgresql && remotePort.val() == '3306') {
            remotePort.val('5432');
        }
        $('.create-main-top').show();
        $('#demo1').show();
    }
    else if (dataSource == ProductNumber.MongoDB || dataSource == ProductNumber.Oracle) {
        $('div[id="MySql-cnn-instr"]').hide();
        $('div[id="SqlServer-cnn-instr"]').hide();
        $('div[id="postgresql-cnn-instr"]').hide();
        if (dataSource == ProductNumber.MongoDB) {
            $('div[id="mongodb-cnn-instr"]').show();
            $('div[id="oracle-cnn-instr"]').hide();
        }
        else {
            $('div[id="mongodb-cnn-instr"]').hide();
            $('div[id="oracle-cnn-instr"]').show();
        }
        $('.create-main-top').hide();
        $('#demo1').hide();
    }
    else {
        usesSsh.parent().hide();
        $('.checkfieldLabel').hide();
        remotePort.parents('.mid').hide();
        // dbInput.attr('hint','Database Name');
        servertooltip.attr('data-content', serverSqlServercontent);
        dbInput.attr('data-content', dbInputSqlServercontent);
        sshDiv.hide();
        sshDiv.find('input').removeAttr('requierd');

        $('div[id="MySql-cnn-instr"]').hide();
        $('div[id="SqlServer-cnn-instr"]').show();
        $('div[id="postgresql-cnn-instr"]').hide();
        $('div[id="mongodb-cnn-instr"]').hide();
        $('div[id="oracle-cnn-instr"]').hide();
        $('.create-main-top').show();
        $('#demo1').show();
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
    $('input[hint!=""], textarea[hint!=""]').removehint();

    if (Demo.validate(elm, Demo)) {
        DivButton.Disable(div);
        //setTimeout(function () {
            Demo.submit(div);
        //}, 100);
        //Preloader.stop();
        return false;
    }
    else
        Preloader.stop();
    return false;
}
Demo.getInfo = function () {
    var form = $('#DemoForm');
    return {
        type: form.find('[name=consoletype]:checked').val(),
        template: form.find('[name=template]:checked').val(),
        name: form.find('input[name="name"]').val(),
        //title: form.find('input[name="title"]').val(),
        title: form.find('input[name="name"]').val(),
        server: form.find('input[name="server"]').val(),
        catalog: form.find('input[name="catalog"]').val(),
        username: form.find('input[name="username"]').val(),
        password: form.find('input[name="password"]').val(),
        usesSsh: form.find('input[name="ssh"]').prop('checked'),
        usesSsl: form.find('input[name="ssl"]').prop('checked'),
        sshRemoteHost: form.find('input[name="sshRemoteHost"]').val(),
        productPort: form.find('input[name="productPort"]').val(),
        sshPort: form.find('input[name="sshPort"]').val(),
        sshUsername: form.find('input[name="sshUsername"]').val(),
        sshPassword: form.find('input[name="sshPassword"]').val(),
        sshPrivateKey: form.find('textarea[name="sshPrivateKey"]').val()
    };
}

function preloader(imgSrc) {
    heavyImage = new Image();
    heavyImage.src = imgSrc;
}

var createDialogReload = true;

Demo.submit = function (div) {
    Demo.hideMessage();

    createDialogReload = false;
    $('#create-content').dialog('close');
    Progress.show();

    var info = Demo.getInfo();
    var data = info;

    var title = info.template == "0" ? "Demo Console" : info.name;
    if (info.type == "2")
        info.template = "5"; //5 - create blank app using our database

    var url = '/Website/CreateApp?template=' + info.template + '&name=' + info.name + '&title=' + title + '&server=' + info.server + '&catalog=' + info.catalog + '&username=' + info.username + '&password=' + info.password + '&usingSsh=' + info.usesSsh + '&usingSsl=' + info.usesSsl + '&sshRemoteHost=' + info.sshRemoteHost + '&sshUser=' + info.sshUsername + '&sshPassword=' + info.sshPassword + '&sshPrivateKey=' + info.sshPrivateKey + '&sshPort=' + info.sshPort + '&productPort=' + info.productPort;
    _gaq.push(['_trackEvent', 'Dialogs', 'Demo', 'Demo Submit Request' + info.template.toString()]);
    url = '/Website/CreateApp';

    var data = { template: info.template, name: info.name, title: title, server: info.server, catalog: info.catalog, username: info.username, password: info.password, usingSsh: info.usesSsh, usingSsl: info.usesSsl, sshRemoteHost: info.sshRemoteHost, sshUser: info.sshUsername, sshPassword: info.sshPassword, sshPrivateKey: info.sshPrivateKey, sshPort: info.sshPort, productPort: info.productPort };
    //setTimeout(function () {
    $.ajax({
        url: url,
        data: data,
        type: "Post",
        async: true,
        dataType: 'json',
        cache: false,
//        data: data,
//        contentType: 'application/json; charset=utf-8',
//        type: "POST",
//        async: true,
//        dataType: 'json',
//        cache: false,
        error: function () { Demo.message('The server is busy, please try again later.'), Preloader.stop(); DivButton.Enable(div); },
        success: function (response) {
            $("body").css("cursor", "default");
            Progress.hide();

            if (response.Success) {
                Preloader.stop();
                var image = $('<img>');
                var dialog = Dialogs.Wait(image, null, 1020, 620, true);
                image.attr('src', waitImgSrc).css('width', 950).css('height', 520).load(function () {
                    // window.location = response.Url;
                });
                setTimeout(function () {
                    if ($('#DemoForm').find('[name=consoletype]:checked').val() == "1") {
                        _gaq.push(['_trackEvent', 'Dialogs', 'Demo', 'Demo Submit']);
                        _gaq.push(['_trackEvent', 'Dialogs', 'Demo', 'Demo Submit' + info.template.toString()]);
                        window.location = response.Url;
                    }
                    else {
                        var action = "/Admin/Pages?p=on&v=";
                        _gaq.push(['_trackEvent', 'Dialogs', 'Demo', 'free database Submit']);
                        if ($('#DemoForm').find('[name=startmode]:checked').val() == "1") {
                            action += "1";
                            _gaq.push(['_trackEvent', 'Dialogs', 'Demo', 'free database excel Submit']);
                        }
                        else if ($('#DemoForm').find('[name=startmode]:checked').val() == "2") {
                            action += "0";
                            _gaq.push(['_trackEvent', 'Dialogs', 'Demo', 'free database blank Submit']);
                        }
                        window.location = response.Url + action;
                    }
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
    //}, 100);

    return;

}
Demo.validate = function (elm, classer) {
    var isValid = true;

    if ($(elm).find('[name=consoletype]:checked').val() == "1") {
        $(elm).find('input[requierd="true"],textarea[requierd="true"]').each(function () {//,textarea[requierd="true"]'
            if (!this.value && this.name != 'sshPassword') {
                classer.handleFieldError(this, this.name, '');
                isValid = false;
            }
        });
    }

    var usesSsh = $(elm).find('[name=ssh]').prop('checked');
    var sshPassword = $('input[name=sshPassword]');
    var sshKey = $('textarea[name=sshPrivateKey]').val();
    if (usesSsh && ((!sshPassword.val() && !sshKey) || (sshPassword.val() && sshKey))) {
        classer.handleFieldError(sshPassword[0], sshPassword.attr('name'), '');
        isValid = false;
    }
    else {
        sshPassword.next('span.error-message').hide();
    }


    var appName = $(elm).find('input[name="name"]').val();
    if (appName == '') {
        classer.handleFieldError($(elm).find('input[name="name"]'), 'Name', '');
        isValid = false;
    }
    if (!isValid)
        $('#create-content').dialog('open');

    if (appName && !isAlphanumeric(appName)) {
        $('#create-content').dialog('open');
        Demo.message('Invalid console name, should be alphanumeric (no spaces or special chars).');
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

    Progress.hide();
    $('#create-content').dialog('open');
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
    if (info.template != ProductNumber.MySQL && info.template != ProductNumber.Postgresql && info.template != ProductNumber.Oracle) // SQL Server it's always 1433
        port = '1433';

    if (!port) {
        if (info.template == ProductNumber.MySQL) port = '3306';
        else if (info.template == ProductNumber.Postgresql) port = '5432';
        else if (info.template == ProductNumber.Oracle) port = '1542';
        else port = '1433';
    }
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
    var usernameHint=$('#trynow-content').find('input[name="username"]').attr('hint');
    var fullnameHint=$('#trynow-content').find('input[name="fullname"]').attr('hint');
    var phoneHint=$('#trynow-content').find('input[name="phonenumber"]').attr('hint');
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
    generalSignInError = $('#login-content').find('span.general');
    $("form#ContactForm").parents('div#container').find('span.general').html("");
    $('input', 'textarea').blur(function () {
        $(this).next('span.error-message').hide();
        generalSignInError.hide();
        DivButton.Enable($("form#ContactForm").find('button.btn-submit'));
    });

    $("form#ContactForm").find('button.btn-submit').click(function () {
        var div = $(this);
        if (!DivButton.IsDisabled(div)) {
            if (ContactUs.validate()) {
                DivButton.Disable(div);
                ContactUs.submit();
                ContactUs.Close();
            }
        }
        return false;

    });
}
ContactUs.validate = function () {
    var isValid = true;
    $('form#ContactForm').find('input[requierd="true"]').each(function () {//,textarea[requierd="true"]'
        if (!this.value) {
            ContactUs.handleFieldError(this.name, this.title + " Required");
            isValid = false;
        }
    });

    $('form#ContactForm').find('textarea[requierd="true"]').each(function () {
        if (!this.value) {
            ContactUs.handleFieldError(this.name, this.name + " Required");
            isValid = false;
        }
    });

    var email = $('form#ContactForm').find('input[name="Email"]').val();
    if (email && !isValidEmail(email)) {
        //if (msg) msg += ', ';
        ContactUs.handleFieldError("Email", 'Invalid Email');
        isValid = false;
    }

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

    Progress.show();
    var info = ContactUs.getInfo();
    var data = { from: info.Email, to: 'support@backand.com', cc: '', message: ContactUs.getMessage(info), subject: ShortProductName + ' Contact Us', name: info.FullName, comments: info.Comments, phone: info.Phone, RequestSubjectId: info.RequestSubjectId };
    var url = '/WebsiteAccount/ContactUs'; //?from=' + info.username + '&password=' + info.password;

    $.ajax({
        url: url,
        //contentType: 'application/json; charset=utf-8',
        data: data,
        type: "Post",
        async: false,
        dataType: 'json',
        cache: false,
        error: function () { ContactUs.message('The server is busy, please try again later.'); },
        success: function (valid) {
            if (valid) {
                Progress.hide();
                $("form#ContactForm").find('div.success').html('Your message was sent. Thank you for your interest in ' + ShortProductName + '.').css("color", "#31B94D").show();
                setTimeout(function () { ContactUs.Close(); },1000);
            }
            else
                ContactUs.message('The server is busy, please try again later.');
        }

    });

    return;

}

ContactUs.Close = function () {
    //try to close the dialog alert
    if ($('#contactdlg').dialog)
        $('#contactdlg').dialog('close');
}

ContactUs.message = function (msg) {
    Progress.hide();
    $("form#ContactForm").find('span.general').html(msg).show();
}

var generalSignInError = null;
var Progress; if (!Progress) Progress = {};

Progress.dialog = null;

Progress.show = function () {
    if (!Progress.dialog) {
        Progress.dialog = Dialogs.Wait($('<div class="progress"></div>'), null, null, null, true, function (dialog) {
            dialog.parents('.ui-dialog:first').css({ 'opacity': '0.6', filter: "Alpha(Opacity=60)" });
            $(".ui-widget-overlay").css({ 'opacity': '0.8', filter: "Alpha(Opacity=60)" });
            });
    }
    else {
        Progress.dialog.dialog('open');
    }
}

Progress.hide = function () {
    if (Progress.dialog) 
        Progress.dialog.dialog('close');
}


var SignIn; if (!SignIn) SignIn = {};

SignIn.init = function () {
    generalSignInError = $('#login-content').find('span.general');

    $('input, select.styled').blur(function () {
        $(this).parent('div').find('span.error-message').hide();
        //$(this).next('span.error-message').hide();
        generalSignInError.hide();
    });

    $('#login-content').find('.login').click(function () {
        var div = $(this);
        if (!DivButton.IsDisabled(div)) {
            SignIn.validateAndSubmit(div);
        }
    });

    $('#login-content').keyup(function (e) {
        if (e.keyCode == 13) {
            var div = $(this).find('.login');
            SignIn.validateAndSubmit(div);
        }
    });

}

SignIn.url = function () {
    return g_mainsite;
}

SignIn.getInfo = function () {
    var form = $('#login-content').find('form');
    return { form: form, username: form.find('input[name="userName"]').val(), password: form.find('input[name="password"]').val() };
}

var gotoApps = true;

SignIn.submit = function () {
    var info = SignIn.getInfo();
    var f = info.form;
    var action = "/Account/LogOn?rememberMe=true&userName=" + info.username + "&password=" + info.password;

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
                Progress.hide();
                $('#login-content').dialog('close');
                if (!SignUp.isLoggedin()) {
                    $('#trynow-content').dialog('open');
                }
                else {
                    SignUp.reg();
                    $('#create-content').dialog('open');
                }
            }
        }

    });
}

SignIn.handleError = function (message) {
    Progress.hide();
    $('#login-content').dialog('open');
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
        //$('#login-content').dialog('open');
        Preloader.stop();
        return;
    }
    if (!info.password) {
        SignIn.handleFieldError('password');
        //$('#login-content').dialog('open');
        Preloader.stop();
        return;
    }

    DivButton.Disable(div);

    Progress.show();
    $('#login-content').dialog('close');
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
        error: function () {
                handleError('The server is busy, please try again later.'); Preloader.stop();
                 },
        success: function (valid) {
            if (valid) {
                submit();
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
}

Preloader.start = function () { //if transitions aren't supported, call this fallback to show the default preloading animations
    $("#slider").show();
}
Preloader.stop = function () {
    $("#slider").hide();
}

var Forgot; if (!Forgot) Forgot = {};

Forgot.init = function () {

    $('.reset').click(function () {
        Forgot.send();
    });

    $('#password-approval-content').find('span.close').click(function () {
        $('#password-approval-content').dialog('close');
    });
}

Forgot.resetPassword = function () {

    var username = $('#login-content').find('input[name="userName"]').val()
    //$('#forgot-content').find('div.general').show();
    $('.reset').removeAttr('disabled');
    $('#forgot-content').dialog('open');
    $('#login-content').dialog('close');

    $('#forgot-content').find('input[name="userNameForReset"]').val(username);

}

Forgot.send = function () {
    var username = $('#forgot-content').find('input[name="userNameForReset"]').val();

    if (username == '') {
        Forgot.showFieldError('userNameForReset');
        return;
    }
    else {
        Forgot.hideFieldError('userNameForReset');
        $('.reset').attr('disabled', 'disabled');
        //$('#forgot-content').find('div.general').hide();
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
                $('#forgot-content').find('.error-message').show();
                $('#forgot-content').find('.error-message').text(valid.message);
                $('.reset').removeAttr('disabled');
            }
            else {
                $('#password-approval-content').dialog('open');
                $('#forgot-content').dialog('close');

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
    Wait: function (element, title, width, height, autoOpen, openCallback) {

        var dialog = $('<div></div>').appendTo('body')
          .append(element)
          .dialog({
              modal: true, title: title, zIndex: 1000000, autoOpen: autoOpen,
              width: width ? width : 'auto', height: height ? height : 'auto', position: ['center', 'center'], resizable: false, open: function () {
                  if (!title)
                      $(this).dialog("widget").find(".ui-dialog-titlebar").hide();

                  if (openCallback) openCallback($(this));
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
