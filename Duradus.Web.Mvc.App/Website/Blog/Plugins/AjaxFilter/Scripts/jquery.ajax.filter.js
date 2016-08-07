/**
* ajaxFilter plug-in for JQuery Version 0.0.1 
* You can arrange a filter that has many options that will fit to the window size
* https://github.com/relly/jquery.ajax.filter/
* Copyright © Relly Rivlin <relly@backand.com>
* License: MIT.
*
* options:
*   itemSelector:
*       type: string 
*       default: '.item'
*       description: the jquery selector for the filter's items
*   inputSelector:
*       type: string 
*       default: '.input'
*       description: the jquery selector for the item's input element
*   labelSelector:
*       type: string 
*       default: '.label'
*       description: the jquery selector for the item's label element
*   textFilterOptions:
*       type: array of object {text: string, operator: string, prefixWildcard: boolean, postfixWildcard: boolean, hideValues: boolean}
*       default : Equals, Does Not Equals, Begins With, Ends With, Contains, Does not Contains, Empty, Is not Empty
*       description: array of object that define the options for textual filter
*   numericFilterOptions:
*       type: array of object {text: string, operator: string, hideValues: boolean}
*       default : Equals, Does Not Equals, Greater Than, Greater Than or Equal to, Less Than, Less Than or Equal to, Between, Empty, Is not Empty
*       description: array of object that define the options for numeric filter
*   dateFilterOptions:
*       type: array of object {text: string, operator: string, hideValues: boolean}
*       default : Equals, Does Not Equals, After, After or Equal to, Before, Before or Equal to, Between, Empty, Is not Empty
*       description: array of object that define the options for date filter
*   clearInputLinkText:
*       type: string
*       default: "&times;"
*       description: this is use to clear a filter input
*   isValid:
*       type: function(inputType, select, operator, hideValues, first, second, dialog)
*       return: bool
*       description: enable to override the isValid function
*
* events:
*   afterResizeCallback(container):
*       description: triggered after the filter container is resized
*   inputEnteredCallback(input, inputType, select, first, second)
*       description: triggered after the user clicks ok on the dialog
*   inputClearedCallback(input)
*       description: triggered after the user clicks on right x that clears the input
*   dialogOpenedCallback(dialog, input, inputType)
*       description: triggered after the dialog is opened
*
* methods:
*   getJson:
*       return: string
*       description: serialized the filter input to json, array of { _name: string, _operator: string, _parameter: string }
*   clear:
*       description: reset the input elements
*/
(function ($) {


    $.fn.ajaxFilter = function (options) {
        // This is the default options.
        var settings = $.extend({
            // These are the defaults.
            itemSelector: '.item',
            inputSelector: '.input',
            labelSelector: '.label',
            textFilterOptions: [{ text: 'Equals', operator: '=' }, { text: 'Does Not Equals', operator: '<>' }, { text: 'Begins With', operator: 'like', postfixWildcard: true }, { text: 'Ends With', operator: 'like', prefixWildcard: true }, { text: 'Contains', operator: 'like', prefixWildcard: true, postfixWildcard: true }, { text: 'Does not Contains', operator: 'not like', prefixWildcard: true, postfixWildcard: true }, { text: 'Empty', operator: 'is null', hideValues: true }, { text: 'Is not Empty', operator: 'is not null', hideValues: true}],
            numericFilterOptions: [{ text: 'Equals', operator: '=' }, { text: 'Does Not Equals', operator: '<>' }, { text: 'Greater Than', operator: '>' }, { text: 'Greater Than or Equal to', operator: '>=' }, { text: 'Less Than', operator: '<' }, { text: 'Less Than or Equal to', operator: '<=' }, { text: 'Between', operator: 'Between' }, { text: 'Empty', operator: 'is null', hideValues: true }, { text: 'Is not Empty', operator: 'is not null', hideValues: true}],
            dateFilterOptions: [{ text: 'Equals', operator: '=' }, { text: 'Does Not Equals', operator: '<>' }, { text: 'After', operator: '>' }, { text: 'After or Equal to', operator: '>=' }, { text: 'Before', operator: '<' }, { text: 'Before or Equal to', operator: '<=' }, { text: 'Between', operator: 'Between' }, { text: 'Empty', operator: 'is null', hideValues: true }, { text: 'Is not Empty', operator: 'is not null', hideValues: true}],
            clearInputLinkText: "&times;",
            afterResizeCallback: null, // triggered after the filter container is resized
            inputEnteredCallback: null, // triggered after the user clicks ok on the dialog
            dialogOpenedCallback: null, // triggered after the dialog is opened
            isValid: null // enable to override the isValid function
        }, options);

        // the original container that will later be arranged in a table
        var container = $(this);
        // a clone to the original
        var groupFilterContainerClone = container.clone();

        // consts
        var SPACE = ' ';
        var BETWEEN = 'Between';
        var COMMA = ',';
        var AND = ' and ';
        var WILDCARD = '%';

        // description: rebuild the filter table each time the widow is resized
        var resize = function (container) {
            var width = container.width();
            var table = resizeToTable(container, 10, width);

            if (table && table.find('tr').length > 1)
                table.attr('width', '100%');

            initiateInput(container);

            if (settings.afterResizeCallback)
                settings.afterResizeCallback(container);
        }

        // return: table element
        // description: build a table for a list of items with a given column number
        var getTable = function (cols, items) {
            var table = $('<table></table>');
            var tr;
            for (var itemIndex = 0; itemIndex < items.length; itemIndex++) {

                if (itemIndex % cols == 0) {
                    tr = $('<tr></tr>');
                    table.append(tr);
                }

                var item = $(items[itemIndex]);
                var label = item.find(settings.labelSelector).clone();
                var labelTd = $('<td></td>')
                tr.append(labelTd);
                labelTd.append(label);
                var filterTd = $('<td></td>')
                tr.append(filterTd);
                var filter = item.find(settings.inputSelector).clone();
                filterTd.append(filter);

            }

            return table;
        }

        // return: table element
        // description: an algorithm that fits maximum items in a row inside the container
        var resizeToTable = function (container, colCount, width) {
            var items = groupFilterContainerClone.find(settings.itemSelector);

            for (var cols = colCount; cols > 0; cols = cols - 1) {
                var table = getTable(cols, items);
                container.html(table);
                var tableWidth = table.width();
                // if the width of the items row is wider than the container then remove an item
                if (tableWidth < width) {
                    return table;
                }

            }

            return null;
        }

        var bindResize = function () {
            $(window).resize(function () {
                resize(container);
            });
        }

        // return: json
        // description: a name value array of the filter, serialized to json
        var getJson = function (container) {
            var elements = container.find(settings.inputSelector);

            var filter = [];

            elements.each(function () {
                var element = $(this);
                if (!element.attr('name'))
                    element = element.find('[name]:first');
                var operator = element.attr('operator');
                var parameter = element.attr('parameter');
                if (operator) {
                    if (parameter) {
                        filter.push({ _name: element.attr('name'), _operator: operator, _parameter: parameter });
                    }
                    else {
                        filter.push({ _name: element.attr('name'), _operator: operator });
                    }
                }
                else if (getInputType(element) == "dropdown" && element.val()) {
                    filter.push({ _name: element.attr('name'), _operator: '=', _parameter: element.val() });
                }
                else if (getInputType(element) == "checklist" && element.val()) {
                    filter.push({ _name: element.attr('name'), _operator: 'in', _parameter: element.val() });
                }
            });

            return JSON.stringify(filter, null, 2);
        }

        // description: reset the input elements
        var clear = function (container) {
            var inputs = container.find(settings.inputSelector);

            var filter = [];

            inputs.each(function () {
                var input = $(this);
                if (!input.attr('name'))
                    input = input.find('[name]:first');
                input.val('');
                input.removeAttr('operator');
                input.removeAttr('parameter');
                input.removeAttr('selection');
                input.removeAttr('originalParameter');
                input.change();
            });
        }

        // description: get the values from the dialog and puts them into the original input element
        var setInputValue = function (input, select, first, second) {
            var option = select.find(':selected');
            var operator = option.attr('operator');
            var parameter = first.val() + (operator == BETWEEN ? AND + second.val() : '');
            input.val(option.text() + SPACE + parameter);
            input.attr('operator', operator);
            input.attr('originalParameter', parameter);
            input.attr('selection', select.val());
            input.attr('parameter', (option.attr('prefixWildcard') == 'prefixWildcard' ? WILDCARD : '') + first.val() + (option.attr('postfixWildcard') == 'postfixWildcard' ? WILDCARD : '') + (operator == BETWEEN ? AND + second.val() : ''));
            input.change();
        }

        // description: get the values from the original input element and puts them into the dialog 
        var getInputValue = function (dialog, input, select, first, second) {

            first.focus();
            var selection = input.attr('selection');
            if (!selection) return;

            select.val(selection);

            var parameter = input.attr('originalParameter');
            if (!parameter) {
                first.hide();
                select.focus();
                return;
            }

            first.show();

            var operator = select.find(':selected').attr('operator');
            if (operator == BETWEEN) {
                split = parameter.split(AND);
                if (split.length != 2)
                    return;
                first.val(split[0]);
                second.val(split[1]);
                dialog.find('div.between').show();
            }
            else {
                first.val(parameter);
                dialog.find('div.between').hide();
            }

        }

        var openedDialog = null;

        // return: string
        // description: return the type of filter, either text, date, numeric or dropdown
        var getInputType = function (input) {
            if (input.is('.text')) {
                return "text";
            }
            else if (input.is('.date')) {
                return "date";
            }
            else if (input.is('.numeric')) {
                return "numeric";
            }
            else if (input.is('.dropdown')) {
                return "dropdown";
            }
            return null;
        }

        // return: boolean
        // description: if the input is valid, for example if the input type is numeric then it should be a valid number, then return true
        var isValid = function (inputType, select, operator, hideValues, first, second, dialog) {
            dialog.find('.invalid-msg').remove();

            if (hideValues)
                return true;
            var isBetween = operator == BETWEEN;

            switch (inputType) {
                case 'date':
                    if (!first.val() || (isBetween && !second.val())) {
                        dialog.append($('<span class="invalid-msg">Required</span>'));
                        return false;
                    }
                    break;
                case 'numeric':
                    if (!$.isNumeric(first.val()) || (isBetween && !$.isNumeric(second.val()))) {
                        dialog.append($('<span class="invalid-msg">Numeric Value</span>'));
                        return false;
                    }
                    break;
                case 'text':
                    if (!first.val()) {
                        dialog.append($('<span class="invalid-msg">Required</span>'));
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        // description: clear input validation error messages
        var clearValidationMessages = function (dialog) {
            dialog.find('.invalid-msg').remove();
        }

        // description: creates and opens the dialog when the input gets the focus
        var openDialog = function (input) {
            var content = $('<div class="dialog"></div>');
            var select = $('<select></select>');
            content.append(select);
            var inputType = getInputType(input);
            if (inputType == "text") {
                var value = 0;
                $(settings.textFilterOptions).each(function () {
                    var option = $('<option value="' + value + '">' + this.text + '</option>');
                    value++;
                    option.attr('operator', this.operator);
                    if (this.postfixWildcard) {
                        option.attr('postfixWildcard', 'postfixWildcard');
                    }
                    if (this.prefixWildcard) {
                        option.attr('prefixWildcard', 'prefixWildcard');
                    }
                    if (this.hideValues) {
                        option.attr('hideValues', 'hideValues');
                    }
                    select.append(option);
                });
                content.append($("<input class='filter-text first' type='text' >"));
            }
            else if (inputType == "date") {
                var value = 0;
                $(settings.dateFilterOptions).each(function () {
                    var option = $('<option value="' + value + '">' + this.text + '</option>');
                    value++;
                    option.attr('operator', this.operator);
                    if (this.hideValues) {
                        option.attr('hideValues', 'hideValues');
                    }
                    select.append(option);
                });
                content.append($("<input class='filter-date first' type='text' >"));
                content.append($("<div class='between' style='display: none;'></div>").html("<span>To</span><br><input class='filter-date second' type='text'>"));
            }
            else if (inputType == "numeric") {
                var value = 0;
                $(settings.numericFilterOptions).each(function () {
                    var option = $('<option value="' + value + '">' + this.text + '</option>');
                    value++;
                    option.attr('operator', this.operator);
                    if (this.hideValues) {
                        option.attr('hideValues', 'hideValues');
                    }
                    select.append(option);
                });
                content.append($("<input class='filter-numeric first' type='text' >"));
                content.append($("<div class='between' style='display: none;'></div>").html("<span>To</span><br><input class='filter-numeric second' type='text'>"));
            }

            var position = input.offset();
            var dialog = $('<div></div>').appendTo('body')
          .append(content)
          .dialog({
              modal: false, autoOpen: true,
              width: Math.max(input.width(), 200), height: 'auto', position: [position.left, position.top - $(window).scrollTop() + input.height() + 8], resizable: false,
              buttons: {
                  OK: function () {
                      var select = $(this).find('select');
                      var first = $(this).find('input.first');
                      var second = $(this).find('input.second');
                      var operator = select.find(':selected').attr('operator');
                      var hideValues = select.find(':selected').attr('hideValues');

                      if (settings.isValid) {
                          if (!settings.isValid(inputType, select, operator, hideValues, first, second, $(this)))
                              return;
                      }
                      else if (!isValid(inputType, select, operator, hideValues, first, second, $(this)))
                          return;

                      setInputValue(input, select, first, second);
                      if (settings.inputEnteredCallback) settings.inputEnteredCallback(input, inputType, select, first, second);
                      $(this).dialog("close");
                  },
                  Cancel: function () {
                      $(this).dialog("close");
                  }
              },
              open: function (event, ui) {
                  if (settings.dialogOpenedCallback)
                      settings.dialogOpenedCallback($(this), input, inputType);

                  $(this).find('select').change(function () {
                      clearValidationMessages(dialog);

                  });
                  $(this).find('input').bind('keyup blur', function () {
                      clearValidationMessages(dialog);
                  });

                  var select = $(this).find('select');
                  var first = $(this).find('input.first');
                  var second = $(this).find('input.second');

                  var d = $(this);
                  var enter = function (e) {
                      if (e.which == '13') {
                          e.preventDefault();
                          d.parent().find('.ui-button:contains("OK")').click()
                      }
                  }
                  first.keypress(enter);
                  second.keypress(enter);

                  getInputValue($(this), input, select, first, second);

                  var height = $(this).innerHeight();

                  $(this).dialog("widget").find(".ui-dialog-titlebar").hide();
                  $(this).dialog("widget").find("button").addClass('filter-button');

                  $(document).bind('click', function (e) {
                      if (e.target != input[0] && !$(e.target).is('.ui-dialog, a') && !$(e.target).closest('.ui-dialog').length && !$(e.target).parents('.ui-datepicker-header').length && !$(e.target).parents('.ui-datepicker').length) {
                          dialog.dialog("close");
                      }
                  });
                  $(this).find('select').change(function () {
                      var operator = $(this).find(':selected').attr('operator');

                      if (operator == BETWEEN) {
                          dialog.find('div.between').show();
                          dialog.css('height', height + dialog.find('div.between').height());
                      }
                      else {
                          dialog.find('div.between').hide().find('.second').val('');
                          dialog.css('height', height)
                      }
                      if ($(this).find(':selected').attr('hideValues') == 'hideValues') {
                          dialog.find('.first').hide().val('');
                      }
                      else {
                          dialog.find('.first').show().focus();
                      }
                  });
              },
              close: function (event, ui) {
                  $(this).remove();
                  $(document).unbind('click');
              }
          });

            return dialog;

        }

        // initiate the input attributes
        var initiateInput = function (container) {
            if (openedDialog)
                openedDialog.dialog('close');

            var inputs = container.find(".numeric, .date, .text");

            inputs.each(function () {
                var input = $(this);
                input.focus(function (e) {
                    if (openedDialog)
                        openedDialog.dialog('close');
                    openedDialog = openDialog(input);
                });

                input.clearSearch({ callback: function () {
                    input.removeAttr('operator');
                    input.removeAttr('originalParameter');
                    input.removeAttr('selection');
                    input.removeAttr('parameter');
                    if (settings.inputClearedCallback) {
                        settings.inputClearedCallback(input);
                    }
                },
                    focusAfterClear: false,
                    linkText: settings.clearInputLinkText ? settings.clearInputLinkText : "&times;"
                });
            });

            var selects = container.find(".dropdown");
            selects.each(function () {
                var select = $(this);
                select.change(function () {
                    if (settings.inputEnteredCallback) {
                        settings.inputEnteredCallback(select, 'dropdown');
                    }
                });
            });
        }

        if (options == 'getJson')
            return getJson(container);
        if (options == 'clear')
            clear(container);
        else {
            //do default action
            bindResize();
            resize(container);


        }


    };
} (jQuery));

