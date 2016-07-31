<%@ Page Title="" Language="C#" MasterPageFile="~/Website/MainHeroku.master" Inherits="System.Web.Mvc.ViewPage<Durados.Web.Mvc.Controllers.CreateAppParameter>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Main" runat="server">

<div id="idWaitDispaly" class="gridboard " style="position:relative;left:0px;top:0px;height:100%;">
   <script src="https://s3.amazonaws.com/assets.heroku.com/boomerang/boomerang.js"></script>
<script>
    document.addEventListener("DOMContentLoaded", function () {
        Boomerang.init({ app: 'bustrack', addon: 'yontest' }); //hard coded
    });
</script>
</div>
    <script>
        try
        {
            $("#idWaitDispaly").height(document.documentElement.clientHeight * 0.95);
        }
        catch(e)
        {

        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%
        Durados.Web.Mvc.Controllers.CreateAppParameter.CODES code = Model.code;
        string msg = Model.msg;
        if( !String.IsNullOrEmpty(msg) )
        {
            msg = msg.Replace("\r", "");
            msg = msg.Replace("\n", "");
            msg = msg.Replace("\"", "");
            msg = msg.Replace("'", "\'");
            msg = msg.Replace(".", "\\.");
        }
        
        int cid = Model.cid;
        string appName = Model.appName;
        string pluginAppName = Model.pluginAppName;/**Plugin name is the appName without any postfix fix for unique id purposes*/
         %>
    <script>
        function defineConst(sDefName, vValue)
        {
            window[sDefName]=vValue;
        }

        defineConst("OK", "OK") ;
        defineConst("INVALID_SECURITY_DATA", "INVALID_SECURITY_DATA");
        defineConst("INVALID_ARGS", "INVALID_ARGS");
        defineConst("INVALID_CONNECTION_STRING", "INVALID_CONNECTION_STRING");
        defineConst("INVALID_CONNECTION_DATA", "INVALID_CONNECTION_DATA");
        defineConst("NOT_SUPPORT_PROVIDER", "NOT_SUPPORT_PROVIDER");
        defineConst("INVALID_PASSWORD", "INVALID_PASSWORD");
        defineConst("INVALID_USER", "INVALID_USER");

        bLoaded = false;

        var code = "<%=code%>" ;
        var msg = "<%=msg%>";

        function showFailure(code, msg)
        {
            $("body").css("cursor", "default");
            Progress.hide();
            ////////////////////////
            Preloader.stop();

            var div = $("#selectTemplateCreateAppFailure");
            if (msg) {
                $("#idTemplateCreateAppFailureMsg").html(msg);
            }
            else {
                $("#idTemplateCreateAppFailureMsg").html("");
            }
            $("#selectTemplateCreateAppFailure").dialog('open');
         }
        var createApp = function () {

        Demo.hideMessage();

        createDialogReload = false;
        Progress.show();

        var connectionId = '<%=cid%>';
        var appName = '<%=appName%>';
        var pluginAppName = '<%=pluginAppName%>';/**Plugin name is the appName without any postfix fix for unique id purposes*/
            var url = '/HerokuWebsite/CreateApp?connectionId=' + connectionId + '&appName=' + appName + '&pluginAppName=' + pluginAppName;
                $.ajax({
                url: url,
                async: true,
                crossDomain: true,
                dataType: 'json',
                cache: false,
                error: function ()
                {
                    /* Demo.message('The server is busy, please try again later.'), Preloader.stop(); DivButton.Enable(div);*/
                    OnCodeHandle(INVALID_ARGS, 'The server is busy, please try again later.');
                },
                success: function (response) {
                       $("body").css("cursor", "default");
                    Progress.hide();
                    
                    if (response.Success) {
                        Preloader.stop();
                        var image = $('<img>');
                        
                        var dialog = Dialogs.Wait(image, null, 1020, 620, true);
                        image.attr('src', waitImgSrc).css('width', 950).css('height', 520).load(function () {
                        });
                        setTimeout(function () {
                               window.location = response.Url1 ;
                         }, 1000);
                    }
                    else {
                         showFailure(INVALID_ARGS, 'The server is busy, please try again later.');
                    }
                }

            });
            /////////////////////////////////
        }

        function OnCodeHandle(nCode, sMsg) {
            switch(nCode)
            {
                case OK:
                {
                    createApp();
                    return  ;
                }
                case INVALID_SECURITY_DATA:
                {
                    /*showFailure(nCode, sMsg);*/
                    showFailure(nCode, 'The server is busy, please try again later.'/*sMsg*/);
                    break;
                }
                case INVALID_ARGS:
                case INVALID_PASSWORD:
                case INVALID_USER:
                case NOT_SUPPORT_PROVIDER:
                case INVALID_CONNECTION_STRING:
                case INVALID_CONNECTION_DATA:
                {
                        showFailure(nCode, 'The server is busy, please try again later.'/*sMsg*/);
                        break;
                }
                default:
                {
                    showFailure(nCode, 'The server is busy, please try again later.'/*sMsg*/);
                    break;
                }
             
            }
        }
       
        ////////////////////////////////////var Preloader = {};
        var oldTempl = null;
        //var waitImgSrc = '/website/assets/images/addviewswait.gif';
        var errMsgMaxLength = 80;
      
        $(document).ready(function ()
        {
            $("#selectTemplateCreateAppFailure").dialog(
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

            /*$(".contact-support-team").click(function () {
                var msg = 'I am experiencing difficulties connecting ' + '< % =pluginAppName%>' + ' to my Heroku Back& plugin';
              //  openContactUsWithMessage(msg, '', '', '', false);
            });*/

            bLoaded = true;
            $("#idWaitDispaly").height(document.documentElement.clientHeight*0.95);
            preloader(waitImgSrc);
            OnCodeHandle(code, msg);
        });

    </script>
</asp:Content>
