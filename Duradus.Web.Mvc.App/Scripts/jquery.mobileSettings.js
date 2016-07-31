/**
* mobileSettings - jQuery plugin 0.9
*
* Copyright (c) 2012-2013 Mudubiz Ltd
*
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*
* $Id: $
*/
(function ($) {

    $.fn.mobileSettings = function (options) {
        // This is the easiest way to have default options.
        var settings = $.extend({
            // These are the defaults.
            topTitle: "Workspaces",
            beforeNavigationCallback: function (li) { }
        }, options);

        var div = this;
        if (!div.is("div")) {
            return this;
        }


        var topUl = div.find('ul:first');


        var innerDiv = $('<div class="mobile-settings-menu-inner"></div>');
        div.append(innerDiv);
        topUl.remove();
        innerDiv.append(topUl);

        var getSelected = function () {
            var selected = topUl.find('ul[selected="selected"]');
            if (selected.length == 0)
                selected = topUl;

            return selected;
        }

        var hideBack = function (title) {
            title.children('a').hide();
        }

        var showBack = function (title, parent) {
            title.children('a').show();
            var text = settings.topTitle;
            if (parent.prev('a').length == 1)
                text = parent.prev('a').text();
            title.children('a').children('span').text(text);
        }

        var setTitle = function (title, text) {
            title.children('span').text(text);
        }

        var show = function (ul) {
            var parent = getParent(ul);
            if (parent != null) {
                hide(parent);
                showBack(title, parent);
                setTitle(title, ul.prev().text());
            }
            else {
                hideBack(title);
                setTitle(title, settings.topTitle);
            }


            ul.parent('li').show();
            ul.parent('li').addClass('hidden');
            ul.prev('a').hide();
            ul.children('li').each(function () {
                var li = $(this);
                li.show();
                li.removeClass('hidden');

                var childUl = li.children('ul');
                if (childUl.length == 1) {
                    hide(childUl);
                    childUl.prev().show();
                    childUl.prev().click(function () {
                        select(childUl);
                    });
                    li.addClass('has-children');
                }
                else {
                    li.removeClass('has-children');

                    li.children('a').mouseup(function () {
                        div.parent().parent().hide();
                        settings.beforeNavigationCallback(li);
                        return false;
                    });
                }
            });
            ul.children('li:first').addClass('first-child');
        }

        var select = function (ul) {
            topUl.removeAttr('selected');
            topUl.find('ul[selected="selected"]').removeAttr('selected');
            ul.attr('selected', 'selected');
            show(ul);
        }

        var getParent = function (ul) {
            var liParent = ul.parent('li');
            if (liParent.length != 1)
                return null;
            var parent = liParent.parent('ul');
            if (parent.length != 1)
                return null;

            return parent;
        }

        var hide = function (ul) {
            ul.children().hide();
        }

        var addTitle = function (div) {
            var span = $('<span>Back</span>');
            var back = $('<a href="#" class="mobile-settings-back"></a>');
            back.append(span);
            var title = $('<div class="mobile-settings-menu-title"><span></span></div>');
            title.prepend(back);
            div.prepend(title);

            back.click(function () {
                select(getParent(getSelected()));
            });

            return title;
        }
        var title = addTitle(div);

        setTitle(title, settings.topTitle);

        select(getSelected());


        return this;
    };



} (jQuery));