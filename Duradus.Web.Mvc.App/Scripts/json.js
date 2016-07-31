// Create a JSON object only if one does not already exist. We create the
// methods in a closure to avoid creating global variables.

if (!this.JSON) {
    this.JSON = {};
}

(function () {

    function f(n) {
        // Format integers to have at least two digits.
        return n < 10 ? '0' + n : n;
    }

    if (typeof Date.prototype.toJSON !== 'function') {

        Date.prototype.toJSON = function (key) {

            return isFinite(this.valueOf()) ?
                   this.getUTCFullYear()   + '-' +
                 f(this.getUTCMonth() + 1) + '-' +
                 f(this.getUTCDate())      + 'T' +
                 f(this.getUTCHours())     + ':' +
                 f(this.getUTCMinutes())   + ':' +
                 f(this.getUTCSeconds())   + 'Z' : null;
        };

        String.prototype.toJSON =
        Number.prototype.toJSON =
        Boolean.prototype.toJSON = function (key) {
            return this.valueOf();
        };
    }

    var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        gap,
        indent,
        meta = {    // table of character substitutions
            '\b': '\\b',
            '\t': '\\t',
            '\n': '\\n',
            '\f': '\\f',
            '\r': '\\r',
            '"' : '\\"',
            '\\': '\\\\'
        },
        rep;


    function quote(string) {

// If the string contains no control characters, no quote characters, and no
// backslash characters, then we can safely slap some quotes around it.
// Otherwise we must also replace the offending characters with safe escape
// sequences.

        escapable.lastIndex = 0;
        return escapable.test(string) ?
            '"' + string.replace(escapable, function (a) {
                var c = meta[a];
                return typeof c === 'string' ? c :
                    '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
            }) + '"' :
            '"' + string + '"';
    }


    function str(key, holder) {

// Produce a string from holder[key].

        var i,          // The loop counter.
            k,          // The member key.
            v,          // The member value.
            length,
            mind = gap,
            partial,
            value = holder[key];

// If the value has a toJSON method, call it to obtain a replacement value.

        if (value && typeof value === 'object' &&
                typeof value.toJSON === 'function') {
            value = value.toJSON(key);
        }

// If we were called with a replacer function, then call the replacer to
// obtain a replacement value.

        if (typeof rep === 'function') {
            value = rep.call(holder, key, value);
        }

// What happens next depends on the value's type.

        switch (typeof value) {
        case 'string':
            return quote(value);

        case 'number':

// JSON numbers must be finite. Encode non-finite numbers as null.

            return isFinite(value) ? String(value) : 'null';

        case 'boolean':
        case 'null':

// If the value is a boolean or null, convert it to a string. Note:
// typeof null does not produce 'null'. The case is included here in
// the remote chance that this gets fixed someday.

            return String(value);

// If the type is 'object', we might be dealing with an object or an array or
// null.

        case 'object':

// Due to a specification blunder in ECMAScript, typeof null is 'object',
// so watch out for that case.

            if (!value) {
                return 'null';
            }

// Make an array to hold the partial results of stringifying this object value.

            gap += indent;
            partial = [];

// Is the value an array?

            if (Object.prototype.toString.apply(value) === '[object Array]') {

// The value is an array. Stringify every element. Use null as a placeholder
// for non-JSON values.

                length = value.length;
                for (i = 0; i < length; i += 1) {
                    partial[i] = str(i, value) || 'null';
                }

// Join all of the elements together, separated with commas, and wrap them in
// brackets.

                v = partial.length === 0 ? '[]' :
                    gap ? '[\n' + gap +
                            partial.join(',\n' + gap) + '\n' +
                                mind + ']' :
                          '[' + partial.join(',') + ']';
                gap = mind;
                return v;
            }

// If the replacer is an array, use it to select the members to be stringified.

            if (rep && typeof rep === 'object') {
                length = rep.length;
                for (i = 0; i < length; i += 1) {
                    k = rep[i];
                    if (typeof k === 'string') {
                        v = str(k, value);
                        if (v) {
                            partial.push(quote(k) + (gap ? ': ' : ':') + v);
                        }
                    }
                }
            } else {

// Otherwise, iterate through all of the keys in the object.

                for (k in value) {
                    if (Object.hasOwnProperty.call(value, k)) {
                        v = str(k, value);
                        if (v) {
                            partial.push(quote(k) + (gap ? ': ' : ':') + v);
                        }
                    }
                }
            }

// Join all of the member texts together, separated with commas,
// and wrap them in braces.

            v = partial.length === 0 ? '{}' :
                gap ? '{\n' + gap + partial.join(',\n' + gap) + '\n' +
                        mind + '}' : '{' + partial.join(',') + '}';
            gap = mind;
            return v;
        }
    }

// If the JSON object does not yet have a stringify method, give it one.

    if (typeof JSON.stringify !== 'function') {
        JSON.stringify = function (value, replacer, space) {

// The stringify method takes a value and an optional replacer, and an optional
// space parameter, and returns a JSON text. The replacer can be a function
// that can replace values, or an array of strings that will select the keys.
// A default replacer method can be provided. Use of the space parameter can
// produce text that is more easily readable.

            var i;
            gap = '';
            indent = '';

// If the space parameter is a number, make an indent string containing that
// many spaces.

            if (typeof space === 'number') {
                for (i = 0; i < space; i += 1) {
                    indent += ' ';
                }

// If the space parameter is a string, it will be used as the indent string.

            } else if (typeof space === 'string') {
                indent = space;
            }

// If there is a replacer, it must be a function or an array.
// Otherwise, throw an error.

            rep = replacer;
            if (replacer && typeof replacer !== 'function' &&
                    (typeof replacer !== 'object' ||
                     typeof replacer.length !== 'number')) {
                throw new Error('JSON.stringify');
            }

// Make a fake root object containing our value under the key of ''.
// Return the result of stringifying the value.

            return str('', {'': value});
        };
    }


// If the JSON object does not yet have a parse method, give it one.

    if (typeof JSON.parse !== 'function') {
        JSON.parse = function (text, reviver) {

// The parse method takes a text and an optional reviver function, and returns
// a JavaScript value if the text is a valid JSON text.

            var j;

            function walk(holder, key) {

// The walk method is used to recursively walk the resulting structure so
// that modifications can be made.

                var k, v, value = holder[key];
                if (value && typeof value === 'object') {
                    for (k in value) {
                        if (Object.hasOwnProperty.call(value, k)) {
                            v = walk(value, k);
                            if (v !== undefined) {
                                value[k] = v;
                            } else {
                                delete value[k];
                            }
                        }
                    }
                }
                return reviver.call(holder, key, value);
            }


// Parsing happens in four stages. In the first stage, we replace certain
// Unicode characters with escape sequences. JavaScript handles many characters
// incorrectly, either silently deleting them, or treating them as line endings.

            text = String(text);
            cx.lastIndex = 0;
            if (cx.test(text)) {
                text = text.replace(cx, function (a) {
                    return '\\u' +
                        ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                });
            }

// In the second stage, we run the text against regular expressions that look
// for non-JSON patterns. We are especially concerned with '()' and 'new'
// because they can cause invocation, and '=' because it can cause mutation.
// But just to be safe, we want to reject all unexpected forms.

// We split the second stage into 4 regexp operations in order to work around
// crippling inefficiencies in IE's and Safari's regexp engines. First we
// replace the JSON backslash pairs with '@' (a non-JSON character). Second, we
// replace all simple value tokens with ']' characters. Third, we delete all
// open brackets that follow a colon or comma or that begin the text. Finally,
// we look to see that the remaining characters are only whitespace or ']' or
// ',' or ':' or '{' or '}'. If that is so, then the text is safe for eval.

            if (/^[\],:{}\s]*$/
.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@')
.replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']')
.replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

// In the third stage we use the eval function to compile the text into a
// JavaScript structure. The '{' operator is subject to a syntactic ambiguity
// in JavaScript: it can begin a block or an object literal. We wrap the text
// in parens to eliminate the ambiguity.

                j = eval('(' + text + ')');

// In the optional fourth stage, we recursively walk the new structure, passing
// each name/value pair to a reviver function for possible transformation.

                return typeof reviver === 'function' ?
                    walk({'': j}, '') : j;
            }

// If the text is not JSON parseable, then a SyntaxError is thrown.

            throw new SyntaxError('JSON.parse');
        };
    }
}());


var isWorking = false;
function StepClickedHandler(e) {

    if (isWorking) return; isWorking = true;
    
    var endpoint = e.data.point;
    var element = endpoint.element;

    var show = element.attr('clicked') != 'clicked';
    
    $.each(endpoint.connections, function(k, con) {
        con.setHover(show);
    });
    
    element.attr('clicked', show ? 'clicked' : '');
    
    isWorking = false;
}

var elementsArray = [];


    
    function createNodesElements() {

        var graphDiv = $('#graphDiv');

        var nodes = getNodesElements();

        if (nodes.length) { return; }

        var nodesData = graphData.Nodes;
        var positionX = 10;
        var positionY = 80;
        var node;

        for (i = 0; i < nodesData.length; i++) {

            node = $('<div id="window' + nodesData[i].Id + '" class="window">' + nodesData[i].Name + '</div>').appendTo(graphDiv);


            if (!graphState[nodesData[i].Id]) {
                graphState[nodesData[i].Id] = { "X": positionX, "Y": positionY };
                positionX += node.width() + 32;
                //positionY += node.height() + 32;                  
            }


            if (nodesData[i].Id == "0") {
                node.addClass("startNode");
            }

            node.css({ top: graphState[nodesData[i].Id].Y, left: graphState[nodesData[i].Id].X });
        }
    }

    var nodesArray = [];

    function createNodesConnections(bottomSource, targetEndpoint) {

        var nodesData = graphData.Nodes;

        var src, hasConnections;

        for (i = 0; i < nodesData.length; i++) {

            src = jsPlumb.addEndpoint('window' + nodesData[i].Id, bottomSource);

            hasConnections = false;                
            
            $.each(nodesData[i].Arrows, function(k, arrow) {

                if ($('#window' + arrow.NodeId).length) {

                    hasConnections = true

                    var lbl = "lbl" + nodesData[i].Id + "" + arrow.NodeId;

                    var conn = jsPlumb.connect({ source: src, target: jsPlumb.addEndpoint("window" + arrow.NodeId, targetEndpoint), overlays: [["PlainArrow", { location: 0.9}], ["Label", { label: arrow.Label || always, location: 0.8, cssClass: "aLabel", id: lbl}]] });


                    conn.ConnLabelId = lbl; //arrow.Label || always;

                    conn.bind("mouseenter", function(conn, originalEvent) {
                        conn.showOverlay(conn.ConnLabelId);
                    });

                    conn.bind("mouseexit", function(conn, originalEvent) {
                        conn.hideOverlay(conn.ConnLabelId);
                    });
                }



            });

            if (hasConnections) { elementsArray.push(src); } else { src.setVisible(false); }

        }


        $.each(elementsArray, function(k, src) {
            src.element.bind("click", { point: src }, StepClickedHandler);
        });             

        
    }

    function getNodesElements() {
        return $('#graphDiv').find('div');
    };


    (function() {

        window.jsDuradosPlumb = {
            init: function() {
                jsPlumb.Defaults.MaxConnections = -1;
                jsPlumb.Defaults.DragOptions = { cursor: 'pointer', zIndex: 2000 };
                jsPlumb.Defaults.EndpointStyles = [{ fillStyle: '#225588' }, { fillStyle: '#558822'}];
                jsPlumb.Defaults.Endpoints = [["Dot", { radius: 7}], ["Dot", { radius: 11}]];
                jsPlumb.setMouseEventsEnabled(true);
                jsPlumb.Defaults.Overlays = [
				["PlainArrow", { location: 0.9}],
				["PreLabel", {
				    location: 0.1,
				    label: always,
				    //function(label) {
				    //    return label.connection.labelText || "";
				    //},
				    cssClass: "aLabel"
                }],
                ["PostLabel", {
				    location: 0.1,
				    label: always,
				    //function(label) {
				    //    return label.connection.labelText || "";
				    //},
				    cssClass: "aLabel"
                }]
			];

                jsPlumb.Defaults.Anchors = ["BottomCenter", "TopCenter"];
                var connectorPaintStyle = {
                    lineWidth: 3,
                    strokeStyle: "#2e2aF8",
                    joinstyle: "round"
                },
			connectorHoverStyle = {
			    lineWidth: 3,
			    strokeStyle: "#28BB28"
			},
                // the definition of source endpoints (the small blue ones)
			sourceEndpoint = {
			    endpoint: "Dot",
			    paintStyle: { fillStyle: "#225588", radius: 7 },
			    isSource: true,
			    connector: ["Flowchart", { stub: 40}],
			    connectorStyle: connectorPaintStyle,
			    hoverPaintStyle: connectorHoverStyle,
			    connectorHoverStyle: connectorHoverStyle
			},
                // a source endpoint that sits at BottomCenter
			bottomSource = jsPlumb.extend({ anchor: "RightMiddle" }, sourceEndpoint),
                // the definition of target endpoints (will appear when the user drags a connection) 
			targetEndpoint = {
			    endpoint: "Dot",
			    paintStyle: { fillStyle: "#558822", radius: 7 },
			    hoverPaintStyle: connectorHoverStyle,
			    maxConnections: -1,
			    dropOptions: { hoverClass: "hover", activeClass: "active" },
			    isTarget: true,
			    anchor: ["LeftMiddle", "RightMiddle", "TopCenter"]
			}

                // add endpoints to all nodes.

                createNodesElements();

                //jsPlumb.addEndpoint(getNodesElements(), [bottomSource]);			

                createNodesConnections(bottomSource, targetEndpoint);


                //jsPlumb.bind("click", function(conn) {
                //    if (confirm("Delete connection from " + conn.sourceId + " to " + conn.targetId + "?"))
                //        jsPlumb.detach(conn);
                //});
                //var sourceEndpoints = jsPlumb.addEndpoint(windows, bottomSource),
                //targetEndpoints = jsPlumb.addEndpoint(windows, targetEndpoint);



            }
        };
    })();

    jsPlumb.bind("ready", function() {

        // chrome fix.
        document.onselectstart = function() { return false; };

        // render mode
        var resetRenderMode = function(desiredMode) {
            var newMode = jsPlumb.setRenderMode(desiredMode);
            $(".rmode").removeClass("selected");
            $(".rmode[mode='" + newMode + "']").addClass("selected");
            var disableList = (newMode === jsPlumb.VML) ? ".rmode[mode='canvas'],.rmode[mode='svg']" : ".rmode[mode='vml']";
            $(disableList).attr("disabled", true);
            jsDuradosPlumb.init();
        };

        $(".rmode").bind("click", function() {
            var desiredMode = $(this).attr("mode");
            if (jsDuradosPlumb.reset) jsDuradosPlumb.reset();
            jsPlumb.reset();
            resetRenderMode(desiredMode);
        });

        resetRenderMode(jsPlumb.CANVAS);

    });


    function saveGraphState() {
        //parent.showProgress();
        var viewName = graphData.ViewName;
        //TODO - use controller name!!!
        //url = parent.rootPath + 'Durados/SaveWorkFlowGraphState';    
        var url = location.href.replace("WorkFlowGraph", "SaveWorkFlowGraphState");

        var nodesData = graphData.Nodes;

        for (i = 0; i < nodesData.length; i++) {
            var pos = $('#window' + nodesData[i].Id).position();
            graphState[nodesData[i].Id] = { "X": parseInt(pos.left,10), "Y": parseInt(pos.top,10) };
        }


        $.ajax({
            url: url,
            type: 'POST',
            data: 'viewName=' + encodeURIComponent(viewName) + '&state=' + encodeURIComponent(JSON.stringify(graphState)),
            async: false,
            cache: false,
            error: function() { alert("Network Error"); }, //parent.ajaxErrorsHandler
            success: function(ret) {
                //parent.hideProgress();
                alert(ret);
            }

        });
}