$(document).ready(function() {
    $(AddDialog).bind('onafterSetDefaultsFromFilter', function(e, data) {
        var viewName = views[data.guid].gViewName;
        if (viewName == 'Issue') {
            var projectID = Issue.SetDefaultCompany(data.json, data.guid);
            var dialog = $('#' + data.guid + 'DataRowCreate');
            var select = dialog.find("select[name='FK_Issue_SubProject_Parent']");
            if (select.length == 1) {
                Dependency.Load(viewName, 'FK_Issue_SubProject_Parent', projectID, select, data.guid);
            }
        }
    });
});

var Issue; if (!Issue) Issue = {};

Issue.SetDefaultCompany = function(json, guid) {
    var projectID = Issue.GetProjectID(json);

    if (projectID == null || projectID == '' || Issue.ContainCompany(json))
        return;

    var dialog = $('#' + guid + 'DataRowCreate');
    var select = dialog.find('select[name="FK_Issue_Company_Parent"]');
    if (select.length == 1) {
        Issue.LoadCompany(projectID, select);
    }
    return projectID;
}

Issue.GetProjectID = function(json) {
    for (var index = 0, len = json.Fields.length; index < len; ++index) {
        var field = json.Fields[index].Value;
        if (field.Name == "FK_Issue_Project_Parent") {
            return field.Value;
        }
    }

    return null;
}

Issue.ContainCompany = function(json) {
    for (var index = 0, len = json.Fields.length; index < len; ++index) {
        var field = json.Fields[index].Value;
        if (field.Name == "FK_Issue_Company_Parent") {
            if (field.Value != null && field.Value != '') {
                return true;
            }
            else {
                return false;
            }
        }
    }

    return false;
}

Issue.LoadCompany = function(projectID, select) {
    showProgress();

    $.ajax({
        url: rootPath + 'TasksIssue/GetCompanyID',
        contentType: 'application/json; charset=utf-8',
        data: { projectID: projectID },
        async: false,
        dataType: 'json',
        cache: false,
        error: ajaxErrorsHandler,
        success: function(companyID) {

            if (companyID != null && companyID != '') {
                select.val(companyID);
            }
            hideProgress();

        }

    });
}
