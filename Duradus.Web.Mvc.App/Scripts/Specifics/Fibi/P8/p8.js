$(Durados.View).bind('add', function(e, data) {
    var actionViewName = 'v_Action';
    if (views[data.guid].ViewName == actionViewName) {
        var classFieldName = 'FK_Class_v_Action_Parent';
        var tableFieldName = 'FK_v_LinkedTable_v_Action_Parent';


        var classSelect = data.dialog.find('select[name="' + classFieldName + '"]');
        var tableSelect = data.dialog.find('select[name="' + tableFieldName + '"]');
    
        
        classSelect.change(function() {
            p8disableSelect(classSelect, tableSelect);
        });

        tableSelect.change(function() {
            p8disableSelect(tableSelect, classSelect);
        });
    }

});

/*
$(Durados.View).bind('initDataTableView', function(e, data) {
    var actionViewName = 'v_Action';
    if (views[data.guid].ViewName == actionViewName) {
        var classFieldName = 'FK_Class_v_Action_Parent';
        var tableFieldName = 'FK_v_LinkedTable_v_Action_Parent';

        var div = $("div[guid='" + data.guid + "']");
        var form = div.find('form[d_prefix="Create"]');
        var classSelect = form.find('select[name="' + classFieldName + '"]');
        var tableSelect = form.find('select[name="' + tableFieldName + '"]');

        classSelect.change(function() {
            p8disableSelect(classSelect, tableSelect);
        });

        tableSelect.change(function() {
            p8disableSelect(tableSelect, classSelect);
        });
    }
});
*/

function p8disableSelect(select1, select2) {
    if (select1.val() != null && select1.find('option').length > 1) {
        if (select1.val() == '') {
            select1.removeAttr('disabled');
            select2.removeAttr('disabled');
        }
        else {
            select2.val('');
            select2.attr('disabled', 'disabled');
        }
    }
}