var v_WorkOrderGuid = '';
var v_WorkOrderDialog = null;

$(Durados.View).bind('editBeforeDerivation', function(e, data) {
    updateWorkOrderStatus(data, editPrefix);
});
$(Durados.View).bind('addBeforeDerivation', function(e, data) {
    updateWorkOrderStatus(data, createPrefix);
});

function updateWorkOrderStatus(data, prefix) {
    if (data.viewName == 'v_WorkOrder' || data.viewName == 'v_WorkOrder_By_Role' || data.viewName == 'v_WorkOrder_Shipment') {
        v_WorkOrderGuid = data.guid;
        v_WorkOrderDialog = data.dialog;
    }
    if (data.viewName == 'v_WorkOrder_Item') {
        var name = 'WorkOrderStatus_v_WorkOrder_Item_Parent';
        var workOrderStatusInItems = $(data.dialog).find('input[name="' + name + '"]');
        if (workOrderStatusInItems.length == 0) {
            workOrderStatusInItems = $(data.dialog).find('select[name="' + name + '"]');
        }

        if (workOrderStatusInItems.length == 1) {
            var workOrderStatus = v_WorkOrderDialog.find('select[name="FK_WorkOrder_WorkOrderStatus_Parent"]');
            var pk = 0;
            if (workOrderStatus.length == 1) {
                pk = workOrderStatus.val();
            }

            workOrderStatusInItems.val(pk);
        }
    }

}