/**
 * Ajax upload
 * Project page - http://valums.com/ajax-upload/
 * Copyright (c) 2008 Andris Valums, http://valums.com
 * Licensed under the MIT license (http://valums.com/mit-license/)
 * Version 3.6 (26.06.2009)
 */

/**
 * Changes from the previous version:
 * 1. Fixed minor bug where click outside the button
 * would open the file browse window
 * 
 * For the full changelog please visit: 
 * http://valums.com/ajax-upload-changelog/
 */

(function () {

    var d = document, w = window;

    /**
    * Get element by id
    */
    function get(element) {
        if (typeof element == "string")
            element = d.getElementById(element);
        return element;
    }

    var d = document, w = window;

    /*** Start Section - input with id Mozilla fix*/
    /**Liad, 13/5/14 Fix for Mozilla consider to use it for all browsers, currently there is a scenario of creationg input adding it to body but not accessing or removing the proper 1, the fix is to add id and get and remove it by id*/
    if ($.browser.mozilla) {

        /*** Remove File Upload Input*/
        function removeInput(oAjaxUpload) {
            try {
                var oItem = $(oAjaxUpload._parentDialog);
                var oInput = oItem.find("#idInputFile");
                if (oInput.length > 0) {
                    oItem.remove("#idInputFile");
                }

                return true;
            }
            catch (e) {
                return false;
            }
        }
        /*** Get File upload Input by id*/
        function getInput(oAjaxUpload) {
            try {
                var oInput = $(oAjaxUpload._parentDialog).find("#idInputFile")
                if (oInput.length > 0) {
                    oAjaxUpload._input = oInput[0];
                }
                return oInput;
            }
            catch (e) {
                return null;
            }
        }
    }

    /** End Section - input with id Mozilla fix*/

    /*** Cross browser Prevent bubbling*/
    function PreventBubbling(event) {
        try {
            if (event.stopPropagation) { // W3C/addEventListener()
                event.stopPropagation();
            }
            else if (event.preventDefault) { // W3C/addEventListener()
                event.preventDefault();
            }
            else if (event.stopImmediatePropagation) {
                event.stopImmediatePropagation();
            }
            else { // Older IE.
                event.cancelBubble = true;
            }
        }
        catch (e) {
        }

        return true;
    }

    /**
    * Attaches event to a dom element
    */
    function addEvent(el, type, fn) {
        if (w.addEventListener) {
            el.addEventListener(type, fn, false);
        } else if (w.attachEvent) {
            var f = function () {
                fn.call(el, w.event);
            };
            el.attachEvent('on' + type, f)
        }
    }


    /**
    * Creates and returns element from html chunk
    */
    var toElement = function () {
        var div = d.createElement('div');
        return function (html) {
            div.innerHTML = html;
            var el = div.childNodes[0];
            div.removeChild(el);
            return el;
        }
    } ();

    function hasClass(ele, cls) {
        return ele.className.match(new RegExp('(\\s|^)' + cls + '(\\s|$)'));
    }
    function addClass(ele, cls) {
        if (!hasClass(ele, cls)) ele.className += " " + cls;
    }
    function removeClass(ele, cls) {
        var reg = new RegExp('(\\s|^)' + cls + '(\\s|$)');
        ele.className = ele.className.replace(reg, ' ');
    }

    // getOffset function copied from jQuery lib (http://jquery.com/)
    if (document.documentElement["getBoundingClientRect"]) {
        // Get Offset using getBoundingClientRect
        // http://ejohn.org/blog/getboundingclientrect-is-awesome/
        var getOffset = function (el) {
            var box = el.getBoundingClientRect(),
		doc = el.ownerDocument,
		body = doc.body,
		docElem = doc.documentElement,

            // for ie 
		clientTop = docElem.clientTop || body.clientTop || 0,
		clientLeft = docElem.clientLeft || body.clientLeft || 0,

            // In Internet Explorer 7 getBoundingClientRect property is treated as physical,
            // while others are logical. Make all logical, like in IE8.


		zoom = 1;
            if (body.getBoundingClientRect) {
                var bound = body.getBoundingClientRect();
                zoom = (bound.right - bound.left) / body.clientWidth;
            }
            if (zoom > 1) {
                clientTop = 0;
                clientLeft = 0;
            }
            var top = box.top / zoom + (window.pageYOffset || docElem && docElem.scrollTop / zoom || body.scrollTop / zoom) - clientTop,
		left = box.left / zoom + (window.pageXOffset || docElem && docElem.scrollLeft / zoom || body.scrollLeft / zoom) - clientLeft;

            return {
                top: top,
                left: left
            };
        }

    } else {
        // Get offset adding all offsets 
        var getOffset = function (el) {
            if (w.jQuery) {
                return jQuery(el).offset();
            }

            var top = 0, left = 0;
            do {
                top += el.offsetTop || 0;
                left += el.offsetLeft || 0;
            }
            while (el = el.offsetParent);

            return {
                left: left,
                top: top
            };
        }
    }

    function getBox(el) {
        var right, bottom, offset;

        if ($('body').attr('dir') != 'rtl') {
            offset = getOffset(el);
            right = offset.left + el.offsetWidth - 100;
            bottom = offset.top + el.offsetHeight;
        } else {
            //offset = $(el).offset();
            offset = getOffset(el);
            right = offset.left + el.offsetWidth;
            bottom = offset.top + el.offsetHeight;
        }

        return {
            left: offset.left,
            right: right,
            top: offset.top,
            bottom: bottom
        };
    }

    /**
    * Crossbrowser mouse coordinates
    */
    function getMouseCoords(e) {
        // pageX/Y is not supported in IE
        // http://www.quirksmode.org/dom/w3c_cssom.html			
        if (!e.pageX && e.clientX) {
            // In Internet Explorer 7 some properties (mouse coordinates) are treated as physical,
            // while others are logical (offset).
            var zoom = 1;
            var body = document.body;

            if (body.getBoundingClientRect) {
                var bound = body.getBoundingClientRect();
                zoom = (bound.right - bound.left) / body.clientWidth;
            }

            return {
                x: e.clientX / zoom + d.body.scrollLeft + d.documentElement.scrollLeft,
                y: e.clientY / zoom + d.body.scrollTop + d.documentElement.scrollTop
            };
        }

        return {
            x: e.pageX,
            y: e.pageY
        };

    }
    /**
    * Function generates unique id
    */
    var getUID = function () {
        var id = 0;
        return function () {
            return 'ValumsAjaxUpload' + id++;
        }
    } ();

    function fileFromPath(file) {
        return file.replace(/.*(\/|\\)/, "");
    }

    function getExt(file) {
        return (/[.]/.exec(file)) ? /[^.]+$/.exec(file.toLowerCase()) : '';
    }

    // Please use AjaxUpload , Ajax_upload will be removed in the next version
    Ajax_upload = AjaxUpload = function (button, options) {
        if (button.jquery) {
            // jquery object was passed
            button = button[0];
        } else if (typeof button == "string" && /^#.*/.test(button)) {
            button = button.slice(1);
        }
        button = get(button);

        this._input = null;
        this._button = button;
        this._disabled = false;
        this._submitting = false;
        // Variable changes to true if the button was clicked
        // 3 seconds ago (requred to fix Safari on Mac error)
        this._justClicked = false;
        this._parentDialog = d.body;

        if (window.jQuery && jQuery.ui && jQuery.ui.dialog) {
            var parentDialog = jQuery(this._button).parents('.ui-dialog');
            if (parentDialog.length) {
                this._parentDialog = parentDialog[0];
            }
        }

        this._settings = {
            // Location of the server-side upload script
            action: 'upload.php',
            // File upload name
            name: 'userfile',
            // Additional data to send
            data: {},
            // Submit file as soon as it's selected
            autoSubmit: true,
            // The type of data that you're expecting back from the server.
            // Html and xml are detected automatically.
            // Only useful when you are using json data as a response.
            // Set to "json" in that case. 
            responseType: false,
            // When user selects a file, useful with autoSubmit disabled			
            onChange: function (file, extension) { },
            // Callback to fire before file is uploaded
            // You can return false to cancel upload
            onSubmit: function (file, extension) { },
            // Fired when file upload is completed
            // WARNING! DO NOT USE "FALSE" STRING AS A RESPONSE!
            onComplete: function (file, response) { }
        };

        // Merge the users options with our defaults
        for (var i in options) {
            this._settings[i] = options[i];
        }

        this._createInput();
        this._rerouteClicks();
    }

    // assigning methods to our class
    AjaxUpload.prototype = {
        setData: function (data) {
            this._settings.data = data;
        },
        disable: function () {
            this._disabled = true;
        },
        enable: function () {
            this._disabled = false;
        },
        // removes ajaxupload
        destroy: function () {
            if (this._input) {
                if (this._input.parentNode) {
                    this._input.parentNode.removeChild(this._input);
                }
                this._input = null;
            }
        },
        /**
        * Creates invisible file input above the button 
        */
        _createInput: function () {

            // clean previous files
            //            if ($.browser.mozilla && $(this._button).parents('.ui-dialog:first').find('input[type=file][name=' + this._settings.name + ']').length == 1)
            //                return;
            //            
            var self = this;

            if ($.browser.mozilla)
            {
                /*** Remove File Upload Input*/
                removeInput(self);
            }

            var input = d.createElement("input");
            if ($.browser.mozilla) {
                input.setAttribute('id', 'idInputFile');
            }

            input.setAttribute('type', 'file');
            //input.setAttribute('type', 'button');
            //input.setAttribute('onclick', 'alert(1)');
            //input.setAttribute('onmouseup', 'this.fireEvent("onclick")');
            input.setAttribute('name', this._settings.name);

            var styles = null;
            if ($.browser.mozilla) {
                styles = {
                    'position': 'absolute'
			, 'margin': '-5px 0 0 -175px'
			, 'padding': 0
			, 'width': '100px'
			, 'height': '30px'
			, 'fontSize': '14px'
                    , 'opacity': 0
			, 'cursor': 'pointer'
                    , 'display': 'none'
                    //, 'zIndex': 999999999 //Max zIndex supported by Opera 9.0-9.2x 
                    // Strange, I expected 2147483647					
                };
            }
            else {
                styles = {
                    'position': 'absolute'
			, 'margin': '-5px 0 0 -175px'
			, 'padding': 0
			, 'width': '100px'
			, 'height': '30px'
			, 'fontSize': '14px'
			, 'opacity': 0
			, 'cursor': 'pointer'
            , 'display': 'none'
			, 'zIndex': 999999999 //Max zIndex supported by Opera 9.0-9.2x 
                    // Strange, I expected 2147483647					
                };
            }

            if ($('body').attr('dir') == 'rtl') {
                styles['margin'] = '0';
                //styles['padding'] = '0 0 0 60px';
                //styles['opacity'] = '90';
            }

            for (var i in styles) {
                input.style[i] = styles[i];
            }

            // Make sure that element opacity exists
            // (IE uses filter instead)
            if (!(input.style.opacity === "0")) {
                input.style.filter = "alpha(opacity=0)";
            }

            this._parentDialog.appendChild(input);

            addEvent(input, 'change', function () {
                // get filename from input
                var file = fileFromPath(this.value);
                if (self._settings.onChange.call(self, file, getExt(file)) == false) {
                    return;
                }
                // Submit form when value is changed
                if (self._settings.autoSubmit) {
                    self.submit();
                }
            });

            // Fixing problem with Safari
            // The problem is that if you leave input before the file select dialog opens
            // it does not upload the file.
            // As dialog opens slowly (it is a sheet dialog which takes some time to open)
            // there is some time while you can leave the button.
            // So we should not change display to none immediately
            addEvent(input, 'click', function (evt) {
                self.justClicked = true;
                setTimeout(function () {
                    // we will wait 3 seconds for dialog to open
                    self.justClicked = false;
                }, 2500);

                if ($.browser.mozilla) {
                    /**Fix for Mozilla preventing event bubbling and allowing file upload to work correctly and open dialog (+input id and removing input by id)*/
                    return PreventBubbling(evt);
                }
            });

            this._input = input;
        },
        _rerouteClicks: function () {
            var self = this;

            // IE displays 'access denied' error when using this method
            // other browsers just ignore click()
            //addEvent(this._button, 'click', function(e){
            //   self._input.click();
            // });

            var box, dialogOffset = { top: 0, left: 0 }, over = false;

            addEvent(self._button, 'mouseover', function (e) {
                if (!self._input || over) return;

                over = true;
                box = getBox(self._button);

                if (self._parentDialog != d.body) {
                    dialogOffset = getOffset(self._parentDialog);
                }
            });


            // We can't use mouseout on the button,
            // because invisible input is over it
            addEvent(document, 'mousemove', function (e) {
                var input = self._input;
                if (!input || !over) return;

                if (self._disabled) {
                    removeClass(self._button, 'hover');
                    input.style.display = 'none';
                    return;
                }

                var c = getMouseCoords(e);

                var bw = box.right - box.left;

                if ((c.x + 3 >= box.left) && (c.x - 3 <= box.right) && (c.y + 6 >= box.top) && (c.y - 6 <= box.bottom)) {

                    if ($('body').attr('dir') == 'rtl') {
                        input.style.left = (c.x - dialogOffset.left - 15) + 'px';
                    } else {
                        input.style.left = c.x - dialogOffset.left + 120 + 'px';
                    }

                    input.style.top = c.y - dialogOffset.top - 6 + 'px';
                    //input.style.left = c.x - dialogOffset.left + 120 + 'px';
                    input.style.display = 'block';
                    addClass(self._button, 'hover');

                } else {
                    // mouse left the button
                    over = false;

                    var check = setInterval(function () {
                        // if input was just clicked do not hide it
                        // to prevent safari bug

                        if (self.justClicked) {
                            return;
                        }

                        if (!over) {
                            input.style.display = 'none';
                        }

                        clearInterval(check);

                    }, 25);


                    removeClass(self._button, 'hover');
                }
            });

        },
        /**
        * Creates iframe with unique name
        */
        _createIframe: function () {
            // unique name
            // We cannot use getTime, because it sometimes return
            // same value in safari :(
            var id = getUID();

            // Remove ie6 "This page contains both secure and nonsecure items" prompt 
            // http://tinyurl.com/77w9wh
            var iframe = toElement('<iframe src="javascript:false;" name="' + id + '" />');
            //		var src = "javascript:'<html><head><script>document.domain=" + "\"" + document.domain + "\"" + "</script></head></html>'";
            //		var iframe = toElement('<iframe src="' + src + '" name="' + id + '" />');
            iframe.id = id;
            iframe.style.display = 'none';
            d.body.appendChild(iframe);
            return iframe;
        },
        /**
        * Upload file without refreshing the page
        */
        submit: function () {
            var self = this, settings = this._settings;

            if (this._input.value === '') {
                // there is no file
                return;
            }

            // get filename from input
            var file = fileFromPath(this._input.value);

            // execute user event
            if (!(settings.onSubmit.call(this, file, getExt(file)) == false)) {
                // Create new iframe for this submission
                var iframe = this._createIframe();

                // Do not submit if user function returns false										
                var form = this._createForm(iframe);
                form.appendChild(this._input);

                form.submit();

                d.body.removeChild(form);
                form = null;
                this._input = null;

                // create new input
                this._createInput();

                var toDeleteFlag = false;

                addEvent(iframe, 'load', function (e) {

                    if (// For Safari
					iframe.src == "javascript:'%3Chtml%3E%3C/html%3E';" ||
                    // For FF, IE
                    //iframe.src == "javascript:'<html><head><script>document.domain=" + "\"" + document.domain + "\"" + "</script></head></html>';") {						
					iframe.src == "javascript:'<html></html>';") {

                        // First time around, do not delete.
                        if (toDeleteFlag) {
                            // Fix busy state in FF3
                            setTimeout(function () {
                                d.body.removeChild(iframe);
                            }, 0);
                        }
                        return;
                    }

                    var doc = iframe.contentDocument ? iframe.contentDocument : frames[iframe.id].document;

                    // fixing Opera 9.26
                    if (doc.readyState && doc.readyState != 'complete') {
                        // Opera fires load event multiple times
                        // Even when the DOM is not ready yet
                        // this fix should not affect other browsers
                        return;
                    }

                    // fixing Opera 9.64
                    if (doc.body && doc.body.innerHTML == "false") {
                        // In Opera 9.64 event was fired second time
                        // when body.innerHTML changed from false 
                        // to server response approx. after 1 sec
                        return;
                    }

                    var sBody = $(doc.body).text();

                    if (!sBody) return;

                    var response;

                    if (doc.XMLDocument) {
                        // response is a xml document IE property
                        response = doc.XMLDocument;
                    } else if (doc.body) {
                        // response is html document or plain text
                        response = $.trim(sBody);
                        if (settings.responseType && settings.responseType.toLowerCase() == 'json') {
                            // If the document was sent as 'application/javascript' or
                            // 'text/javascript', then the browser wraps the text in a <pre>
                            // tag and performs html encoding on the contents.  In this case,
                            // we need to pull the original text content from the text node's
                            // nodeValue property to retrieve the unmangled content.
                            // Note that IE6 only understands text/html
                            if (doc.body.firstChild && doc.body.firstChild.nodeName.toUpperCase() == 'PRE') {
                                response = doc.body.firstChild.firstChild.nodeValue;
                            }
                            if (response) {
                                response = window["eval"]("(" + response + ")");
                            } else {
                                response = {};
                            }
                        }
                    } else {
                        // response is a xml document
                        var response = doc;
                    }

                    settings.onComplete.call(self, file, response);

                    // Reload blank page, so that reloading main page
                    // does not re-submit the post. Also, remember to
                    // delete the frame
                    toDeleteFlag = true;

                    // Fix IE mixed content issue
                    iframe.src = "javascript:'<html></html>';";
                });

            } else {
                // clear input to allow user to select same file
                // Doesn't work in IE6
                // this._input.value = '';

                if (!$.browser.mozilla)
                {
                    /**Problematic code since input was added by  this._parentDialog.appendChild(input); this._parentDialog is body as default value but changed to be dialog, 
                    so any dialog manipluation can influence. another problem is regarding this._input to used as a ref for removing the proper file upload input, but it could be null and then input tag won't be removed.
                    I left this code as it is already works for Chrome but it is sensetive problematic code that could easily stop working with any code future changes*/
                    d.body.removeChild(this._input);
                }

                this._input = null;
                // create new input
                this._createInput();
            }
        },
        /**
        * Creates form, that will be submitted to iframe
        */
        _createForm: function (iframe) {
            var settings = this._settings;

            // method, enctype must be specified here
            // because changing this attr on the fly is not allowed in IE 6/7		
            var form = toElement('<form method="post" enctype="multipart/form-data"></form>');
            form.style.display = 'none';
            form.action = settings.action;
            form.target = iframe.name;
            d.body.appendChild(form);

            // Create hidden input element for each data key
            for (var prop in settings.data) {
                var el = d.createElement("input");
                el.type = 'hidden';
                el.name = prop;
                el.value = settings.data[prop];
                form.appendChild(el);
            }
            return form;
        }
    };
})();