var Backand = importNamespace('Backand');
var console = new Backand.console(); var socket = new Backand.socket(); var files = new Backand.files(); var request = new Backand.request(); var ParseAuth = new Backand.ParseAuth(); var security = new Backand.security();

(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        define(factory);
    }
    else if (typeof ports === 'object') {
        module.exports = factory;
    }
    else {
        root.cookie = factory(root);
    }
})
(this, function (root) {
    'use strict';
    var exports = {};

    var bCookie = new Backand.Cookie();

    exports['get'] = function (key) {
        var value = bCookie.get(key)
        if (value != null)
            try { return JSON.parse(value); } catch (err) { return value;}
        else
            return null;
    };
    exports['put'] = function (key, value) {
        if (value != null)
            bCookie.put(key, JSON.stringify(value));
        else
            bCookie.put(key, null);

    };
    exports['remove'] = function (key) {
        bCookie.remove(key);
    };
    return exports;
});

(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        define(factory);
    }
    else if (typeof ports === 'object') {
        module.exports = factory;
    }
    else {
        root.atomic = factory(root);
    }
})
(this, function (root) {
    'use strict';
    var parse = function (req) {
        var result;
        try {
            result = JSON.parse(req.responseText);
        }
        catch (e) {
            result = req.responseText;
        }
        return [result, req];
    };
    var xhr1 = function (type, url, data, async, headers) {
        var methods = {
            success: function () { },
            error: function () { }
        };
        var request = new Backand.XMLHttpRequest();
        request.open(type, url, async);
        request.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
        if (headers) {
            for (var property in headers) {
                if (headers.hasOwnProperty(property)) {
                    request.setRequestHeader(property, headers[property]);
                }
            }
        }
        /*request.onreadystatechange = function () {      if (request.readyState === 4) {        if (request.status >= 200 && request.status < 300) {          methods.success.apply(methods, parse(request));        } else {          methods.error.apply(methods, parse(request));        }      }    }; */
        if (data) request.send(data);
        else request.send();

        var callbacks = { success: function (callback) { methods.success = callback; return callbacks; }, error: function (callback) { methods.error = callback; return callbacks; } };
        if (async) {
            return callbacks;
        }
        else {
            if (request.status >= 200 && request.status < 300) {
                if (request.responseText) {
                    try{
                        return JSON.parse(request.responseText);
                    }
                    catch (err) {
                        return request.responseText;
                    }
                }
                return request.responseText;
            }
            else {
                var err = new Error(request.responseText);
                err.name = request.status;
                throw err;
            }
        }
    };

    var xhr = function (type, url, data, async, headers) {
        var errorContainMessage = function (err) {
            var messages = ['Could not create SSL/TLS secure channel'];
            for (var i = 0; i < messages.length; i++) {
                if ((err.message && err.message.indexOf(messages[i]) >= 0) || (err.indexOf && err.indexOf(messages[i]) >= 0)) {
                    return true;
                }
            }
            return false;
        }

        try {
            return xhr1(type, url, data, async, headers);
        }
        catch (err1) {
            var er = err1;
                
            var tries = 3;
            var currentTry = 0;
            while (errorContainMessage(er) && currentTry < tries) {
                try {
                    currentTry++;
                    return xhr1(type, url, data, async, headers);
                }
                catch (err3) {
                    er = err3;
                }
            }
            throw err1;
        }
    
    }
    var isString = function (s) {
        return (typeof s == 'string' || s instanceof String) || (Object.prototype.toString.call(s) == '[object String]');
    }
    var exports = function (parameters) {
        var method = "GET"; var async = (parameters && parameters.async) ? true : false;
        if (parameters && parameters.method) method = parameters.method;
        var url = null;
        if (parameters && parameters.url) url = parameters.url;
        if (url == null)
            throw new Error("url is missing");
        var headers = null;
        if (parameters && parameters.headers) headers = parameters.headers;
        var data = null;
        if (parameters && parameters.data) data = parameters.data;

        var params = null;
        if (parameters && parameters.params) params = parameters.params; 

        if (params) {
            var prefix = url.indexOf('?') === -1 ? '?' : '&';
            var qs = '';
            for (var property in params) {
                if (params.hasOwnProperty(property)) {
                    var val = params[property];
                    if (typeof val === 'object')
                        val = JSON.stringify(val);
                    qs += prefix + encodeURIComponent(property) + '=' + encodeURIComponent(val);
                    prefix = '&';
                }
            }
            url = url + qs;
        }
        return xhr(method, url, data ? (isString(data) ? data : JSON.stringify(data)) : null, async, headers);
    };

    exports['get'] = function (url, headers) {
        return xhr('GET', url, null, false, headers);
    };
    exports['put'] = function (url, data, headers) {
        return xhr('PUT', url, data ? (isString(data) ? data : JSON.stringify(data)) : null, false, headers);
    };
    exports['patch'] = function (url, data, headers) {
        return xhr('PATCH', url, data ? (isString(data) ? data : JSON.stringify(data)) : null, false, headers);
    };
    exports['post'] = function (url, data, headers) {
        return xhr('POST', url, data ? (isString(data) ? data : JSON.stringify(data)) : null, false, headers);
    };
    exports['delete'] = function (url, headers) {
        return xhr('DELETE', url, null, false, headers);
    };
    return exports;
});
var $http = atomic;

(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        define(factory);
    }
    else if (typeof ports === 'object') {
        module.exports = factory;
    }
    else {
        root.backand = factory(root);
    }
})



(this, function (root) {
    'use strict';

    var getOwnPropertySymbols = Object.getOwnPropertySymbols;
    var hasOwnProperty = Object.prototype.hasOwnProperty;
    var propIsEnumerable = Object.prototype.propertyIsEnumerable;

    var toObject = function(val) {
        if (val === null || val === undefined) {
            throw new TypeError('Object.assign cannot be called with null or undefined');
        }

        return Object(val);
    }

    var isObject = function(obj) {
        return obj === Object(obj);
    }

    var stringifyIfObject = function (val) {
        if (isObject(val)) {
            return JSON.stringify(val);
        }
        else {
            return val;
        }
    }

    var exports = {
	    fn: {
	        post: function(name, data, parameters, headers){
	            return $http({ method: "POST", url: CONSTS.apiUrl + "/1/function/general/" + name, data: data, params: {parameters:parameters}, headers:headers});
	        }, 
	        get: function(name, parameters, headers){
	            return $http({ method: "GET", url: CONSTS.apiUrl + "/1/function/general/" + name, params: {parameters:parameters}, headers:headers});
	        }
	    },
	    objectAssign: function (target, source) {
	        var from;
	        var to = toObject(target);
	        var symbols;

	        for (var s = 1; s < arguments.length; s++) {
	            from = Object(arguments[s]);

	            for (var key in from) {
	                if (hasOwnProperty.call(from, key)) {
	                    to[key] = stringifyIfObject(from[key]);
	                }
	            }

	            if (getOwnPropertySymbols) {
	                symbols = getOwnPropertySymbols(from);
	                for (var i = 0; i < symbols.length; i++) {
	                    if (propIsEnumerable.call(from, symbols[i])) {
	                        to[symbols[i]] = stringifyIfObject(from[symbols[i]]);
	                    }
	                }
	            }
	        }

	        return to;
	    }
	}

    return exports;
});
