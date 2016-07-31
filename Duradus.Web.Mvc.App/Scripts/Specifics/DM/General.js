
$(Durados.View).bind('anotherRowFunction', function(e, data) {
    if (data.pks.length == 1) {
        OpenViewer(data.pks[0], data.guid);
    }
});

$(EditDialog).bind('onafterEditLoad', function(e, data) {
    OpenViewer(data.pk, data.guid);
});


function OpenViewer(pk,guid) {
    window.open("/DMDocument/ImageViewer/Document?pk=" + pk, "Durados_Viewer", "toolbar=no,scrollbars=yes,resizable=yes,width=800,left=1000'", true);
    //window.open("/uploads/4.tif", "Durados_Viewer", "", true);
}