// Abstract Dialog starts here
function Dialog(div, title, viewName, getInputElementsUrl, previousDialog) {
    this.serverErrorPrefix = "$$error$$";
    this.constructor(div, title, viewName, getInputElementsUrl, previousDialog);
}

Dialog.method('constructor', function(div, title, viewName, getInputElementsUrl, previousDialog) {
    this.div = div;
    this.form = $('#'+this.div.attr('id') + 'form')[0];
    this.previousDialog = previousDialog;
    div.dialog({
        bgiframe: true,
        autoOpen: false,
        modal: true,
        position: 'center'
    });
    div.data('title.dialog', title); 
    this.initiateButtons();
    var html = this.getInputElements(viewName, getInputElementsUrl);
    div.html(html);
    div.dialog('option','width',div.scrollWidth);
    
});

Dialog.method('Show', function() {
    this.closePreviousDialog(this.previousDialog); 
    this.div.dialog('open');
        this.loadInputElementsValues();    
    this.initiateWysiwyg();

});

Dialog.method('initiateButtons', function() {
    div.dialog("option", "buttons", {
      "Cancel": function() { this.Close(); }
    });
});

Dialog.method('getInputElements', function(viewName, getInputElementsUrl) {
    var syncHtml = '';

    $.ajax({
        url: getInputElementsUrl + viewName,
        contentType: 'application/json; charset=utf-8',
        data: { viewName: viewName },
        async: false,
        cache: false,
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.getAllResponseHeaders() + " " + textStatus + " " + errorThrown);
        },
        success: function(html) {
            hideProgress();
            var index = html.indexOf(serverErrorPrefix, 0)
            if (index < 0 || index > 1000) {
                syncHtml = html;
            }
            else {
                this.showFailureMessage(html.replace(serverErrorPrefix, ""));
            }
        }

    });


    return syncHtml;
}

Dialog.method('showFailureMessage', function(message) {
    complete();
    alert(message);
}

Dialog.method('closePreviousDialog', function(previousDialog) {
    if (previousDialog != null){
        previousDialog.Close();
    }
}

Dialog.method('loadInputElementsValues', function() {
}

Dialog.method('initiateWysiwyg', function() {
    var id = this.div.attr('id');
    if (id != null){
        $('#' + id + ' textarea').wysiwyg();
    }
}

Dialog.method('Close', function() {
    this.div.dialog('close');
}
// Abstract Dialog ends here

// Add Dialog starts here
function AddDialog(div, title, viewName, getInputElementsUrl, previousDialog) {
    this.constructor(div, title, viewName, getInputElementsUrl, previousDialog);
}

AddDialog.inherits(Dialog);

AddDialog.method('initiateButtons', function() {
    div.dialog("option", "buttons", {
          "Add": function() { this.Add(false); },
          "Add Another": function() { this.Add(true); },
          "Cancel": function() { this.Close(); }
    });
});

AddDialog.method('loadInputElementsValues', function() {
    var jsonFilter = FilterForm.JsonFilter;
    if (jsonFilter==null){
        jsonFilter=GetJsonFilter();
    }
    this.SetDefaultsFromFilter(jsonFilter, createPrefix);
}

AddDialog.method('SetDefaultsFromFilter', function(json, prefix) {
    if (json!=null){
        for (var index = 0, len = json.Fields.length; index < len; ++index) {
            var field = json.Fields[index].Value;
            var htmlField = $('#' + prefix + field.Name);
            if (field.Value != null && field.Value != ''){
                
                if ($(htmlField[0]).attr('upload') == 'upload'){
                    showUpload(field.Name, prefix, field.Value, $('#' + prefix + 'upload_img_' + field.Name).attr('UploadPath') + field.Value);
                }        
                else if(htmlField[0].type == "textarea")
                {
                    htmlField.text(field.Value);
                    $('#' + prefix + field.Name).wysiwyg(); //create the wysiwyg
                }
                else if(htmlField[0].type == "checkbox")
        	        htmlField.attr('checked', field.Value);
    	        else if(field.Type == 'Autocomplete'){
                    htmlField.attr('valueId', field.Value);
                    htmlField.val(field.Default);
                }
		        else
                    htmlField.val(field.Value);
            } 
        }
    }
}
AddDialog.method('Add', function(another) {
        if (AddDialog.isValid() == false) {
            return;
        }
        showProgress();
        $.post(gCreateUrl,
        {
            jsonView : Sys.Serialization.JavaScriptSerializer.serialize(FillJson(GetJsonViewForCreate(), createPrefix))
        },
        function(html) {
            hideProgress();
            var index = html.indexOf("$$error$$", 0);
            if (index < 0 || index > 1000) {
                AddDialog.HandleSuccess(html, another);
            }
            else {
                AddDialog.ShowFailureMessage(html.replace("$$error$$",""));
            }
        });
    }
    
    AddDialog.method('HandleSuccess', function(another) {
        if (another){
                this.form.reset();
        }
        else{
            this.Close();
            this.div.html(html);
            success();
        }
    }
    
    AddDialog.method('isValid', function() {
        
        if (Spry.Widget.Form.validate(this.form) == false) {
            return false;
        }
        return true;
    }
// Add Dialog ends here
