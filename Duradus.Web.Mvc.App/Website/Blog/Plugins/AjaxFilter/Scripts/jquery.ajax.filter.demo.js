$(document).ready(function () {
    // initiate
    $('.filter').ajaxFilter({ clearInputLinkText: '<img style="vertical-align: bottom;" src="Images/clear_cross.png" />', dialogOpenedCallback: function (dialog, input, inputType) {
            dialog.find('.filter-date').datepicker();
        }
    });
    // get json
    $('input.filter-button[value="Apply"]').click(function () {
        alert("The filter name value json array:\n\n" + $('.filter').ajaxFilter('getJson'));
    });
    // clear
    $('input.filter-button[value="Clear"]').click(function () {
        $('.filter').ajaxFilter('clear');
    });
});
