Array.prototype.remove = function(from, to) {
    var rest = this.slice((to || from) + 1 || this.length);
    this.length = from < 0 ? this.length + from : from;
    return this.push.apply(this, rest);
};


(function($) {

    $.fn.recents = function(options) {

        var r = new Durados.Recents();

        var opts = $.extend({}, $.fn.recents.defaults, options);
        r.SetMaxRecents(opts.maxRecents);
        r.SetCookieOptions(opts.cookieOptions);
        r.SetUnique(opts.unique);
        //r.ResetCookie();
        r.Load();

        var selection = this.each(function() {
            $(this).empty();
            r.Show($(this));
        });

        if (opts.addCurrent) {
            r.AddCurrentLink();
            r.Save();
        }

        return selection;

    }

    $.fn.recents.defaults = { addCurrent: true, maxRecents: 10, cookieOptions: { path: '/', expires: 10 }, unique: true }
})(jQuery);

if (!Durados.Recents) Durados.Recents = function() {
    //this.maxRecents = 10;
    this.array = new Array();
    this.cookieName = 'LastRecents';
    //this.cookieOptions = { path: '/', expires: 10 };
    this.linkSeperator = '!!!';
}

Durados.Recents.prototype.constructor = Durados.Recents;

Durados.Recents.prototype.GetMaxRecents = function() {
    return this.maxRecents;
}

Durados.Recents.prototype.SetMaxRecents = function(value) {
    this.maxRecents = value;
}

Durados.Recents.prototype.GetUnique = function() {
    return this.unique;
}

Durados.Recents.prototype.SetUnique = function(value) {
    this.unique = value;
}

Durados.Recents.prototype.GetCookieOptions = function() {
    return this.cookieOptions;
}

Durados.Recents.prototype.SetCookieOptions = function(value) {
    this.cookieOptions = value;
}

Durados.Recents.prototype.ResetCookie = function() {
    this.array = new Array();
    this.Save();
}

Durados.Recents.prototype.GetArray = function() {
    return this.array;
}

Durados.Recents.prototype.Load = function() {
    var content = $.cookie(this.cookieName);
    if (content != null && content != '') {
        this.array = content.split(',');
    }
}

Durados.Recents.prototype.AddCurrentLink = function() {
    var currentUrl = window.location;
    var currentTitle = document.title;
    var link = currentUrl + this.linkSeperator + currentTitle;
    var titleIndex = this.TitleIndex(currentTitle);
    var containsTitle = titleIndex >= 0;

    if (!this.unique || this.unique && !containsTitle) {
        this.array.unshift(link);
        if (this.array.length > this.maxRecents) {
            this.array.pop();
        }
    }
    else if (this.unique && containsTitle) {
        this.array.remove(titleIndex);
        this.array.unshift(link);
    }
}

Durados.Recents.prototype.TitleIndex = function(currentTitle) {
    for (var index = 0, len = this.array.length; index < len; ++index) {
        var link = this.array[index];
        var linkArray = link.split(this.linkSeperator);
        var title = linkArray[1];

        if (currentTitle == title)
            return index;
    }

    return -1;
}

Durados.Recents.prototype.Save = function() {
    var content = this.array.toString();
    $.cookie(this.cookieName, content, this.cookieOptions);
}

Durados.Recents.prototype.Show = function(container) {
    for (var index = 0, len = this.array.length; index < len; ++index) {
        var link = this.array[index];
        var linkArray = link.split(this.linkSeperator);
        var url = linkArray[0];
        var title = linkArray[1];
        var a = '<li><a href="' + url + '">' + title + '</a></li>';
        container.append(a);
        //container.append('<br>');
    }
}

