/***********************************************
* backand JavaScript Library
* Authors: backand 
* License: MIT (http://www.opensource.org/licenses/mit-license.php)
* Compiled At: 06/24/2014 
***********************************************/
var SLASH = '/';
backand.admin = {
    routes: {
        admin: 'admin',
        app: 'myApps',
        connection: 'myAppConnection',
    },
    getUrl: function () {
        return backand.options.url + SLASH + backand.admin.routes.admin + SLASH;
    },
    app: {
        getUrl: function () {
            return backand.admin.getUrl() + backand.admin.routes.app + SLASH;
        },
        create: function (name, title, successCallback, errorCallback) {
            var url = backand.admin.app.getUrl();
            var data = JSON.stringify({ Name: name, Title: title });
            backand.options.ajax.json(url, data, backand.options.verbs.post, successCallback, errorCallback);

        },
        update: function (id, title, image, successCallback, errorCallback) {
            var url = backand.admin.app.getUrl() + id;
            var data = JSON.stringify({ Title: title, Image: image });
            backand.options.ajax.json(url, data, backand.options.verbs.put, successCallback, errorCallback);
        },
        delete: function (id, successCallback, errorCallback) {
            var url = backand.admin.app.getUrl() + id;
            backand.options.ajax.json(url, null, backand.options.verbs.delete, successCallback, errorCallback);
        },
        rename: function (id, name, successCallback, errorCallback) {
            var url = backand.admin.app.getUrl() + id;
            var data = JSON.stringify({ Name: name });
            backand.options.ajax.json(url, data, backand.options.verbs.put, successCallback, errorCallback);

        },
        get: function (id, successCallback, errorCallback) {
            var url = backand.admin.app.getUrl() + id;
            backand.options.ajax.json(url, null, backand.options.verbs.get, successCallback, errorCallback);
        },
        /* get a list of rows with optional filter, sort and page */
        getList: function (withSelectOptions, pageNumber, pageSize, filter, sort, search, successCallback, errorCallback) {
            var url = backand.admin.app.getUrl();
            var data = { withSelectOptions: withSelectOptions, pageNumber: pageNumber, pageSize: pageSize, filter: JSON.stringify(filter), sort: JSON.stringify(sort), search: search };
            backand.options.ajax.json(url, data, backand.options.verbs.get, successCallback, errorCallback);
        },

        connection: {
            getUrl: function () {
                return backand.admin.getUrl() + backand.admin.routes.connection + SLASH;
            },
            create: function (appId, product, server, database, username, password, successCallback, errorCallback) {
                var url = backand.admin.app.connection.getUrl() + appId;
                var data = JSON.stringify({ product: product, server: server, database: database, username: username, password: password });
                backand.options.ajax.json(url, data, backand.options.verbs.post, successCallback, errorCallback);
            },
            update: function (appId, product, server, database, username, password, successCallback, errorCallback) {
                var url = backand.admin.app.connection.getUrl() + appId;
                var data = JSON.stringify({ product: product, server: server, database: database, username: username, password: password });
                backand.options.ajax.json(url, data, backand.options.verbs.put, successCallback, errorCallback);

            },
            get: function (appId, successCallback, errorCallback) {
                var url = backand.admin.app.connection.getUrl() + appId;
                backand.options.ajax.json(url, null, backand.options.verbs.get, successCallback, errorCallback);
            },

        }
    }
}



