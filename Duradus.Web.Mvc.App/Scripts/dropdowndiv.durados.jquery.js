
Durados.dropdowndiv = {
    height: 150,
    width: 150,
    input: null,
    div: null,
    date1: null,
    date2: null,
    numeric1: null,
    numeric2: null,
    token: '&&%&',
    hide: function () {
        if (Durados.dropdowndiv.div && Durados.dropdowndiv.div.length) {
            Durados.dropdowndiv.div.hide();
            if (Durados.dropdowndiv.input != null) {
                Durados.dropdowndiv.input.unbind('keydown').unbind('keyup');
            }
            $(document).unbind('click', Durados.dropdowndiv.autoClose);
            Durados.dropdowndiv.div.attr('inputTarget', '');
        }

    },
    autoClose: function (e) {
        if (e.target != null && !$(e.target).closest('div.advancedFilterDialog').length && !$(e.target).closest('.ui-datepicker').length && !$(e.target).closest('.ui-datepicker-calendar').length && $(e.target).attr('tagName') != 'SPAN' && $(e.target).attr('tagName') != 'A' && !$(e.target).hasClass('ui-datepicker-current') && !$(e.target).parents('.ui-datepicker-header').length) {
            if (Durados.dropdowndiv.input && e.target != Durados.dropdowndiv.input[0]) {
                Durados.dropdowndiv.hide();
            }
        }
    },
    /************************************************************************************/
    /*		positionDialog (by br)			
    /*		Position advanced filter dialog related to input
    /************************************************************************************/
    positionDialog: function (input, dialog) {
        var dddLeft;
        var dialogWidth = input.width() - num(dialog.css('padding-right')) - num(dialog.css('padding-left'));

        dialog.width(dialogWidth);

        //Get dialog.innerWidth() in order to get real width of dialog, because dialog has min width css.
        dialogWidth = dialog.innerWidth();

        if ($('body').attr('dir') == 'rtl') {
            dddLeft = input.offset().left + input.width() - dialogWidth;
            if (dddLeft < 0) dddLeft = input.offset().left;
            //            if (dddLeft < 0) dddLeft = 0;
        }
        else {
            dddLeft = input.offset().left;
            if ((dddLeft + dialogWidth) > $(window).width()) dddLeft = input.offset().left + input.width() - dialogWidth;
            //            if ((dddLeft + dialogWidth) > $(window).width()) dddLeft = $(window).width() - dialogWidth;
        }

        dialog.css('left', dddLeft)
        .css('top', input.offset().top + input.height() + 6);
    },
    show: function (el, between) {
        $(document).bind('click', Durados.dropdowndiv.autoClose);

        var input = $(el);

        var typeId = input.attr('filterType');

        if (!typeId) { return; } //typeId = 'Number'; 

        Durados.dropdowndiv.div = $('#' + typeId);

        if (!Durados.dropdowndiv.div.length) return;

        var dialog = Durados.dropdowndiv.div;
        if (dialog.attr('inputTarget') == input.attr('id') && dialog.is(':visible')) {
            //            Durados.dropdowndiv.hide();
            $('div.advancedFilterDialog').hide();
            Durados.dropdowndiv.positionDialog(input, dialog);
            Durados.dropdowndiv.div.show();
            return;
        }

        //        Durados.dropdowndiv.hide();
        $('div.advancedFilterDialog').hide();

        Durados.dropdowndiv.input = input;
        Durados.dropdowndiv.typeId = typeId;

        Durados.dropdowndiv.positionDialog(input, dialog);

        dialog.find('select').val('');

        dialog.find('input[type="text"]').val('');

        initAdvancedFilterHandlers(dialog);
        dialog.find('.date___').width(dialog.width() - 20);

        initAdvancedFilterValues(dialog, between);

        dialog.show();
        dialog.attr('inputTarget', input.attr('id'));
        dialog.find('input:first').focus().select();
    },
    /************************************************************************************/
    /*		displayElementsByCompSign (by br)			
    /*		display inputs in dialog by comp sign
    /************************************************************************************/
    displayElementsByCompSign: function (select) {
        var compSign = select.val();

        if (compSign.indexOf('empty') > -1) {
            select.siblings(':not(.advancedFilterDialog-buttons)').hide();
        }
        else {
            select.siblings().show();
            if (compSign == 'between') {
                select.siblings('.between').show();
            }
            else {
                select.siblings('.between').hide();
            }
        }
    },
    /************************************************************************************/
    /*		handleFilterValue (by br)			
    /*		When user types filter into input not by dialog- handle this case: 
    /*      Set correct filter value and correct tooltip 
    /************************************************************************************/
    handleFilterValue: function (filterElement) {
        var filterDisplayedText = filterElement.val();

        if (filterDisplayedText == null || filterDisplayedText == '') {
            filterElement.attr('d_val', '');
        }
        else {
            var filterType = filterElement.attr('filterType');
            if (!filterType) { return; };

            var selectElement = $('#' + filterType + ' .filter-equal');
            if (!selectElement.length) { return; };

            var filter = filterDisplayedText;
            var tooltip = filterDisplayedText;
            var token = Durados.dropdowndiv.token;
            var foundMatchedComparison = false;

            function sortByValDesc(option1, option2) {
                var option1Val = $(option1).val();
                var option2Val = $(option2).val();

                return ((option1Val > option2Val) ? -1 : ((option1Val < option2Val) ? 1 : 0));
            }

            //Sort select options desc- in order that ">" will appear after ">=", "<" will appear after "<=" (To prevent a bug)
            selectElement.find('option').sort(sortByValDesc).each(function () {
                var option = $(this);
                var text = '';
                if (filterType == 'TextFilter') {
                    text = $.trim(option.text());
                }
                else {
                    var betweenTranslation = translator.between;
                    if (filterDisplayedText.match('^' + betweenTranslation)) {
                        text = $.trim(option.text());
                    }
                    else {
                        text = option.val();
                    }
                }

                if (filterDisplayedText.match('^' + text)) {
                    var compSign = option.val();

                    filter = filter.replace(text, token + compSign + token);
                    //                tooltip
                    if (filterType == 'TextFilter') {
                        tooltip = filterDisplayedText;
                    }
                    else {
                        if (compSign == 'between') {
                            var toTranslation = translator.to;
                            filter = filter.replace(toTranslation, token + 'To' + token);
                            tooltip = filterDisplayedText;
                        }
                        else {
                            tooltip = filterDisplayedText.replace(text, $.trim(option.text()));
                        }
                    }

                    foundMatchedComparison = true;
                    return false;
                }
            });

            if (!foundMatchedComparison) {
                var dialog = $('#' + filterType);
                var defaultSign = getDefaultAdvancedFilterSign(dialog);
                var defaultOption = selectElement.find('option[value="' + defaultSign + '"]');
                var text = '';

                if (filterType == 'TextFilter') {
                    text = $.trim(defaultOption.text());

                    if (!filterDisplayedText.match('^"')) {
                        filterDisplayedText = '"' + filterDisplayedText;
                    }
                    if (!filterDisplayedText.match('"$')) {
                        filterDisplayedText = filterDisplayedText + '"';
                    }
                    tooltip = text + ' ' + filterDisplayedText;
                }
                else {
                    filterDisplayedText = $.trim(filterDisplayedText);
                    text = defaultOption.val();
                    tooltip = $.trim(defaultOption.text()) + ' ' + filterDisplayedText;
                }
                filter = token + defaultSign + token + ' ' + filterDisplayedText;
                filterDisplayedText = text + ' ' + filterDisplayedText;
                filterElement.val(filterDisplayedText);
            }

            //Handle leading zeros
            if (filterType == 'DateFilter') {
                var words = filter.split(token + "To" + token);
                var leftSide = words[0];
                var compSign = '';
                var firstVal = '';

                compSign = Durados.dropdowndiv.getComparerString(leftSide, token, token);
                firstVal = $.trim(leftSide.replace(token + compSign + token, '').replace(/^\s\s*/, '').replace(/\s\s*$/, ''));
                var df = filterElement.attr('df');
                var spryFormat = duradosGetSpryDateFormat(df);
                if (!isValidDate('#' + filterElement.attr('id'), spryFormat)) {
                    var date = Date.parseExact(firstVal, df);
                    //Date is valid in another format
                    if (date != null) {
                        var dateString = date.toString(spryFormat);
                        filter = filter.replace(firstVal, dateString);
                        tooltip = tooltip.replace(firstVal, dateString);
                        filterElement.val(filterElement.val().replace(firstVal, dateString));
                    }
                }
            }

            filterElement.attr('d_val', filter).attr('title', tooltip);
        }
    },
    //TODO- Extract code to this function
    handleLeadingDateZeros: function () {
    },
    /************************************************************************************/
    /*		getComparerString (by br)			
    /*		Get comparer string by search comparer surrounding with prefix and suffix
    /************************************************************************************/
    getComparerString: function (str, prefix, suffix) {
        var beginPos = str.indexOf(prefix, 0);
        var value = "=";

        if (beginPos > -1) {
            var start = beginPos + prefix.length;
            var end = str.indexOf(suffix, start);
            if (end > -1) {
                value = str.substr(start, end - start);
            }
        }

        return value;
    }
}

function okAdvancedFilter() {
    var dialog = Durados.dropdowndiv.div;
    var token = Durados.dropdowndiv.token;
    var select = dialog.find('select').val();
    var selectReplacement = select;
    var selectText = $.trim(dialog.find('select :selected').text());
    var filterType = dialog.attr('id');
    var first = dialog.find('input[name="first"]').val();
    var second = dialog.find('input[name="second"]').val();

    var filter = '';
    var value = '';
    var tooltip = '';

    if (first != '' || select.indexOf('empty') > -1) {
        var space = ' ';
        if (select == 'between' && second != '') {
            filter = first + space + token + 'To' + token + space + second;
            selectReplacement = selectText;
        }
        else if (select.indexOf('empty') > -1) {
            selectReplacement = selectText;
            filter = '';
        }
        else {
            if (filterType == 'TextFilter') {
                first = '"' + first + '"';
                selectReplacement = selectText;
            }
            filter = first;
        }
        filter = token + select + token + space + filter;

        value = filter.replace(token + select + token, selectReplacement);
        tooltip = filter.replace(token + select + token, selectText);

        value = value.replace(token + 'To' + token, translator.to);
        tooltip = tooltip.replace(token + 'To' + token, translator.to);
    }

    Durados.dropdowndiv.input.val(value).attr('title', tooltip).attr('d_val', filter)/*.prop('changed', false)*/;
    Durados.dropdowndiv.hide();

    var guid = Durados.dropdowndiv.input.parents('div[ajaxdiv="ajaxDiv"]:first').attr('guid');
    if (guid == null) {
        guid = getMainPageGuid();
    }
    FilterForm.Apply(false, guid, null);
}

function cancelAdvancedFilter() {
    Durados.dropdowndiv.hide();
}

function initAdvancedFilterValues(dialog, between) {

    var token = Durados.dropdowndiv.token;
    var input = Durados.dropdowndiv.input;

    var firstInput = dialog.find('input[name="first"]');
    var secondInput = dialog.find('input[name="second"]');
    var select = dialog.find('select');

    var first = firstInput.val();
    var second = secondInput.val();

    var compSign;

    if (between) {
        select.siblings('.between').show();
        select.attr('disabled', 'disabled');
    } else {
        select.siblings('.between').hide();
        select.removeAttr('disabled');
    }

    var filter = input.attr('d_val');

    if (filter.length < 3) {
        filter = '';
        input.val('');
    }

    if (filter) {
        var words = filter.split(token + "To" + token);
        var leftSide = words[0];
        var compSign = '';
        var firstVal = '';

        compSign = Durados.dropdowndiv.getComparerString(leftSide, token, token);
        firstVal = $.trim(leftSide.replace(token + compSign + token, '').replace(/^\s\s*/, '').replace(/\s\s*$/, ''));

        if (compSign == 'between') {
            secondInput.val(words[1].replace(/^\s\s*/, ''));
        }
        else {
            secondInput.val('');
        }

        if (firstVal.match('^"')) {
            firstVal = firstVal.substring(1);
        }
        if (firstVal.match('"$')) {
            firstVal = firstVal.substring(0, firstVal.length - 1);
        }
        firstInput.val(firstVal);
    }

    if (compSign)
        select.val(compSign);
    else if (!between) {
        var defaultSign = getDefaultAdvancedFilterSign(dialog);
        select.val(defaultSign);
    }

    Durados.dropdowndiv.displayElementsByCompSign(select);
}

function getDefaultAdvancedFilterSign(dialog) {
    var defaultSign = '=';
    var isTextFilter = dialog.attr('id') == 'TextFilter';

    if (isTextFilter) {
        var insideTextSearch = dialog.attr('insideTextSearch');
        if (insideTextSearch == 'True') {
            defaultSign = '%like%';
        }
        else {
            defaultSign = 'like%';
        }
    }

    return defaultSign;
}

function initAdvancedFilterHandlers(dialog) {

    //if (dialog.attr('ready') == 'yes') return;

    var input = Durados.dropdowndiv.input;
    input.unbind('change').bind('change', function () {
        Durados.dropdowndiv.handleFilterValue(input);
    });

    var firstInput = dialog.find('input[name="first"]');
    var secondInput = dialog.find('input[name="second"]');


    if (dialog.find('#date1').length) {

        var df = Durados.dropdowndiv.input.attr("df");
        var dateType = num(DateFormats.getValidDateType(Durados.dropdowndiv.input.attr("df")));
        //        var dateType = num(DateFormats.getValidDateType(Durados.dropdowndiv.input.attr("dt")));
        var spryFormat = duradosGetSpryDateFormat(df);
        var JQueryFormat = duradosGetJQueryDateFormat(df);

        dialog.find('span.textfieldInvalidFormatMsg').html(spryFormat);

        var options1 = { validation: function () { return isValidDate(firstInput, spryFormat); }, isRequired: false, useCharacterMasking: true, validateOn: ["change"] };
        var options2 = { validation: function () { return isValidDate(secondInput, spryFormat); }, isRequired: false, useCharacterMasking: true, validateOn: ["change"] };
        var vtype = 'custom';

        if (Durados.dropdowndiv.date1 && Durados.dropdowndiv.date2) {
            Durados.dropdowndiv.date1.reset();
            Durados.dropdowndiv.date2.reset();

            Durados.dropdowndiv.date1.type = vtype;
            Durados.dropdowndiv.date2.type = vtype;

            Durados.dropdowndiv.date1.init("date1", options1);
            Durados.dropdowndiv.date2.init("date2", options2);

        } else {

            Durados.dropdowndiv.date1 = new Spry.Widget.ValidationTextField("date1", vtype, options1);

            Durados.dropdowndiv.date2 = new Spry.Widget.ValidationTextField("date2", vtype, options2);

        }
        dialog.find('input.date___').datepicker("destroy");

        dialog.find('input.date___').each(function () {
            switch (dateType) {
                case DateFormats.dateType.dateOnly:
                    $(this).datepicker({ showButtonPanel: true, showOn: 'button', buttonImage: rootPath + "Content/smoothness/images/calendar.jpg", beforeShow: datePickerBeforeShow, onChangeMonthYear: datePickerOnChangeMonthYear, buttonImageOnly: true, dateFormat: JQueryFormat.dateFormat, onSelect: function (dateText, inst) {
                        if (inst.input)
                            setTimeout("triggerDateChanged('#" + $(inst.input).attr('id') + "');", 1);
                    }
                    }); //.removeClass('nadate')
                    break;
                case DateFormats.dateType.dateAndTime:
                    $(this).datetimepicker({ showButtonPanel: true, showOn: 'button', buttonImage: rootPath + "Content/smoothness/images/calendar.jpg", beforeShow: datePickerBeforeShow, onChangeMonthYear: datePickerOnChangeMonthYear, buttonImageOnly: true, dateFormat: JQueryFormat.dateFormat, timeFormat: JQueryFormat.timeFormat, onSelect: function (dateText, inst) { if (inst.input) setTimeout("triggerDateChanged('#" + $(inst.input).attr('id') + "');", 1); } }); //.removeClass('nadate')
                    break;
                case DateFormats.dateType.timeOnly:
                    $(this).timepicker({ showButtonPanel: true, showOn: 'button', buttonImage: rootPath + "Content/smoothness/images/calendar.jpg", beforeShow: datePickerBeforeShow, onChangeMonthYear: datePickerOnChangeMonthYear, buttonImageOnly: true, timeFormat: JQueryFormat.timeFormat, onSelect: function (dateText, inst) { if (inst.input) setTimeout("triggerDateChanged('#" + $(inst.input).attr('id') + "');", 1); } }); //.removeClass('nadate')
                    break;
                default:
                    break;


            }
        });
    }
    else if (dialog.find('#numeric1').length) {

        if (Durados.dropdowndiv.numeric1 && Durados.dropdowndiv.numeric2) {
            return;
        }

        options1 = { isRequired: false, useCharacterMasking: true, validateOn: ["change"] };

        options2 = { isRequired: false, useCharacterMasking: true, validateOn: ["change"] };

        vtype = 'real';


        Durados.dropdowndiv.numeric1 = new Spry.Widget.ValidationTextField("numeric1", vtype, options1);

        Durados.dropdowndiv.numeric2 = new Spry.Widget.ValidationTextField("numeric2", vtype, options2);
    }


    if (dialog.attr('ready') == 'yes') return;

    dialog.find('select').change(function () {
        Durados.dropdowndiv.displayElementsByCompSign($(this));
        firstInput.focus().select();
    });


    dialog.attr('ready', 'yes');

    //Bind close options
    input.bind('keydown', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode == 9) {
            Durados.dropdowndiv.hide();
        }
    });

    input.bind('keyup', function () {
        $(this).attr("title", $(this).val());
    });

    dialog.find('input[name="first"],input[name="second"]').unbind('keydown').bind('keydown', function (e) {
        var charCode;

        if (e && e.which) {
            charCode = e.which;
        } else if (window.event) {
            e = window.event;
            charCode = e.keyCode;
        }

        if (charCode == 13) {
            okAdvancedFilter();
        }
        else if (charCode == 27) {
            Durados.dropdowndiv.hide();
        }
    });
}

