var Backand = importNamespace('Backand');

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
            return JSON.parse(value);
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
    var exports = {};
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
    var xhr = function (type, url, data, async, headers) {
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
                return JSON.parse(request.responseText);
            }
            else {
                var err = new Error(request.responseText);
                err.name = request.status;
                throw err;
            }
        }
    };
    exports['get'] = function (src, async, headers) {
        return xhr('GET', src, null, async, headers);
    };
    exports['put'] = function (url, data, async, headers) {
        return xhr('PUT', url, data, async, headers);
    };
    exports['post'] = function (url, data, async, headers) {
        return xhr('POST', url, data, async, headers);
    };
    exports['delete'] = function (url, async, headers) {
        return xhr('DELETE', url, null, async, headers);
    };
    return exports;
});
var $http = atomic;


