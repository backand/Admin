<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%
    string url = (ViewData["url"] ?? string.Empty).ToString();
    string backUrl = (ViewData["backUrl"] ?? string.Empty).ToString();

    string style = string.Empty;
    // WIXWIX
    //if (backUrl.Contains("Admin/Item/View"))
    //{
    //    style = @"style=""background: rgb(90, 90, 90)""";
    //}
      
%>
<html xmlns="http://www.w3.org/1999/xhtml" <%=Durados.Web.Mvc.Maps.Instance.GetMap().Database.Localizer.Direction=="RTL" ? " dir='rtl'":"" %>
<%=style %>>
<head>
    <style>
        .overlay > div:first-child
        {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: #000;
            filter: alpha(opacity=60);
            -moz-opacity: 0.6;
            -khtml-opacity: 0.6;
            opacity: 0.6;
            z-index: 10000;
        }
        
        .loading
        {
            position: absolute;
            left: 50%;
            top: 50%;
            text-align: center;
            margin-left: -60px;
            z-index: 20000;
        }
        .loading > div
        {
            color: White;
            margin-bottom: 5px;
            text-shadow: 2px 2px 1px black;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 13px;
        }
        
       
       .pages-close {
            background-image: url("/Content/Images/ui-sprite.png");
            background-position: -59px -150px;
            display: block;
            text-indent: -99999px;
            overflow: hidden;
            background-repeat: no-repeat;
            width: 7px;
            height: 16px;
            text-align: center;
            cursor: pointer;
            color: buttontext;
            padding: 2px 6px 3px;
            border: none;
            float: right;
            font-size: 20px;
            font-family: Arial, Helvetica, sans-serif;
            font: 14px Arial, Helvetica, sans-serif;
        }
        
        .ui-widget-content a {
            color: #000000;
        }
        a {
            text-decoration: none;
        }
        
        .page-settings-header {
            font-size: 20px;
            color: rgb(161,161,161);
            font-family: Arial, Helvetica, sans-serif;
            font: 14px Arial, Helvetica, sans-serif;
        }

        .page-settings-header {
            font-size: 20px;
            color: rgb(161,161,161);
            padding: 10px;
        }

    </style>
    <script type="text/javascript">
        document.domain = '<%=Durados.Web.Mvc.Maps.Host %>';
        var backUrl = '<%=backUrl%>';
    </script>
    <script src='<%=ResolveUrl("~/Scripts/jquery.min.js")%>' type="text/javascript"></script>
    <script type="text/javascript">
        function navigateBack() {
            window.location.href = backUrl;
        }
        function getDocumentScope() {
            var scope = document;
            if (window.parent) {
                scope = window.parent.document;
            }

            return scope;
        }

        function iframeLoaded() {
            try {
                setTimeout(iframeLoaded2(), 1);
            }
            catch (err) {
            }
        }

        function iframeLoaded2() {
            var scope = getDocumentScope();
            // WIXWIX $('iframe').contents().find('body').css('background-color', 'rgb(90, 90, 90)');
            $('iframe').contents().find('body').bind('onafterEditUpdate', function (e, data) {
                //if (parent && parent.refreshWidget2)
                //alert('onafterEditUpdate');

            });

//            $('iframe').contents().find('body').bind('refreshGrid', function (e, guid) {
//                $('iframe').contents().find('.group-sorting').hide();
//            });

            resize($('iframe'));
            $('#loadSettings', scope).hide();
            $('iframe').show();
            $('iframe[name=viewProp]', scope).show();
            $(window).resize();
            
            if ($.browser.mozilla) {
                setTimeout(function () {
                    var iframe1 = $(getDocumentScope()).find('#d_viewProp');
                    iframe1.height(iframe1.height()+1);
                },
                1000);
            }
           // alert($('iframe').$('iframe').contents().html());
            
        }

        $(document).ready(function () {
            //aaa;
            var scope = getDocumentScope();
            //            $('iframe').load(function () {
            //                iframeLoaded();
            //            });


            resize($('iframe'));
            //            $('iframe').show();
            $('input').show();
            if ($.browser.msie) {
                $('iframe[name=viewProp]', scope).show();
                //                $('.loadingSettings', scope).hide();
            }
            $(window).resize(function () {
                resize($('iframe'));
            });
            // WIXWIX $('body').css('background-color', 'rgb(90, 90, 90)');

            $('.columns-done').click(function () {
                debugger;
                var menuOff = '';
                if (parent.location.href.indexOf('menu=off') != -1) {
                    menuOff = '?menu=off';
                }
                if (parent) {
                    //parent.location = parent.location.href.split('?')[0] + menuOff;
                    parent.location = backUrl;
                }
            });

            
        });

        var toResize;
        var dialogResize;

        resize = function (iframeElement) {
            iframeElement.width($(window).width() - 8);
            var t = $('#columns-titlebar');
            var th = 0;
            if (t.length > 0) {
                th = t.height() + 20;
            }
            iframeElement.height($(window).height() - 8 - th);
        }


    </script>
</head>
<body style="margin: 0 0 0 0;" class="template" dir='<%=Durados.Web.Mvc.Maps.Instance.GetMap().Database.Localizer.Direction.ToLower() %>'>
<% if (Durados.Web.Mvc.Maps.Instance.GetMap().Database.GetUserRole() != "View Owner")
   { %>
    <div id="columns-titlebar" class="page-settings-header">
    <span>Columns</span>
    <a href="#" class="columns-done" title="Close">
        <span onclick="navigateBack();" class="pages-close">close</span>
    </a>
    <%} %>
</div><center>
        <iframe style="display:none; margin-top:3px;" src='<%=url %>' frameborder="0" onload="iframeLoaded();"></iframe>
    </center>
    <div id="pluginProgress" class="overlay" style="display: none;">
        <div>
        </div>
        <div class="loading">
            <div>
               <%=Durados.Web.Mvc.Maps.Instance.GetMap().Database.Localizer.Translate("Performs Changes...")%> </div>
            <img src="/Content/Images/wait.gif" alt="loading..." /></div>
    </div>
</body>
</html>
