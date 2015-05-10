backand.options.ajax.json = function (url, data, verb, successCallback, erroCallback, forToken) {
    $.ajax({
        url: url,
        async: false,
        type: verb,
        beforeSend: function (xhr) {
            if (!forToken)
                xhr.setRequestHeader('Authorization', backand.security.authentication.token);
        },
        data: data,
        dataType: 'json', 
        cache: false,
        error: function (xhr, textStatus, err) { if (xhr, textStatus, err) erroCallback(xhr, textStatus, err); },
        success: function (data, textStatus, xhr) { if (successCallback) successCallback(data, textStatus, xhr); }
    });
};

backand.options.ajax.file = function (url, data, verb, successCallback, erroCallback) {
    $.ajax({
        url: url,
        type: verb,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', backand.security.authentication.token);
        },
        data: data,
        processData: false,
        contentType: false,
        cache: false,
        error: function (xhr, textStatus, err) { if (xhr, textStatus, err) erroCallback(xhr, textStatus, err); },
        success: function (data, textStatus, xhr) { if (successCallback) successCallback(data, textStatus, xhr); }
    });
};