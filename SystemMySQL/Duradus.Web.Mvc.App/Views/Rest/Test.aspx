<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Test</title>
    <%--<link href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css" rel="stylesheet">--%>
    
    <link href="../../Content/bootstrap.min.css" rel="stylesheet" />

    <link href="../../Content/jquery.jsonview.css" rel="stylesheet" />

    <link href="../../Content/EasyUI/themes/default/easyui.css" rel="stylesheet" />
    <link href="../../Content/EasyUI/themes/icon.css" rel="stylesheet" />

    <script src="http://code.jquery.com/jquery-latest.js"></script>
    
    <script src="../../Scripts/ngback/bootstrap.min.js"></script>
    
    
    <script src="../../Scripts/jquery.jsonview.js"></script>
    <script src="../../Scripts/jquery.easyui.min.js"></script>
    <script src="../../Scripts/backand.js"></script>
    <script src="../../Scripts/jquery.backand.js"></script>
    <script src="http://localhost/WebApplication1/api/banner"></script>

    <script>

        var data2 = null;
        var numericOrDateOperators = [{ 'operator': 'equals' }, { 'operator': 'notEquals' }, { 'operator': 'greaterThan' }, { 'operator': 'greaterThanOrEqualsTo' }, { 'operator': 'lessThan' }, { 'operator': 'lessThanOrEqualsTo' }, { 'operator': 'empty' }, { 'operator': 'notEmpty' }];
        var textOperators = [{ 'operator': 'equals' }, { 'operator': 'notEquals' }, { 'operator': 'startsWith' }, { 'operator': 'endsWith' }, { 'operator': 'contains' }, { 'operator': 'notContains' }, { 'operator': 'empty' }, { 'operator': 'notEmpty' }];
        var relationOperators = [{ 'operator': 'in' }];


        var initJqBackand = function () {
            var url = 'http://localhost/WebApplication1';
            backand.options.url = url;

            var urlInput = $('input.url');
            //backand.security.authentication.token = 'Bearer LF6e7oMW84TWBuwvMP5ZnAIFXLfLxPlTnXqnSh5GSzD1YjbRU3frcom6i7xkKMrfnBdpXN2bYl4sBZ05gjDsp_9LAk-YnKHtJpC9ASUCd4cRS9AEEBIL9eDLUFCioNxJaUMuYHqsDxwm4lzntdvoOcVQsNWr6bIRYD6gA1w4OqZsLAsklhgCpYHRjF2xp5qU-5HO3VVzLl2QtXSGSSSWHRcKAh4AOpiYSEaV-Doynj5FypVCVdtNUDSeazqD0Q8Vpd41CX14QOcuMXH2pS5qSQ';
        };

        function login() {
            $('.login-container').show();
            
            //$('.login-container').dialog({
            //    modal: true, title: 'login', zIndex: 2147480000, autoOpen: true,
            //    width: 'auto', resizable: false,
            //    buttons: {
            //        login: function () {
            //            $(this).dialog("close");
            //            var username = $('.username').val();
            //            var password = $('.password').val();
            //            var appname = $('.appname').val();

            //            backand.security.authentication.getToken(username, password, appname);
            //        }
            //    },
            //    close: function (event, ui) {
            //        //$(this).remove();
            //    }
            //});
        }

        $(document).ready(function () {

            $('.login').click(function () {
                var username = $('.username').val();
                var password = $('.password').val();
                var appname = $('.appname').val();

                backand.security.authentication.login(username, password, appname, function (data) { alert(JSON.stringify(data)); $('.login-container').hide(); }, function (xhr, textStatus, err) { alert(JSON.stringify(xhr)); });
                

            });

            $('.logoButton').click(function () {
                backand.api.app.getConfig(function (data) {
                    var logoUrl = data.company.logo;
                    $('.logo').attr('src', decodeURIComponent(logoUrl));
                });
            });

            var versionSelect = $('.api-url-segment-version select');
            var repositorySelect = $('.api-url-segment-repository select');
            var configOrDataSelect = $('.api-url-segment-config select');
            var viewSelect = $('.api-url-segment-view select');
            var dashboardSelect = $('.api-url-segment-dashboard select');
            var chartSelect = $('.api-url-segment-chart select');
            var idInput = $('.api-url-segment-id input');

            var updateUrl = function () {
                var urlInput = $('input.url');
                var version = versionSelect.val();
                var repository = $('.api-url-segment-repository select').val();
                var configOrData = $('.api-url-segment-config select').val();
                var pageType = $('.api-url-segment-page-type:visible select').val();
                var id = idInput.val();
                if (repository != "view")
                    id = null;

                var url = backand.options.url;//[location.protocol, '//', location.host, location.pathname].join('').replace('/Rest/Test', '');
                urlInput.val(url + '/' + version + '/' + repository + '/' + configOrData + '/' + (pageType ? pageType : '') + (id ? '/' + id : '')); //+ '?id=' + userGuid);
            };


            var clean = function () {
                $('.json-output').val('');
                $('.status').val('');
                $('.location').val('');
                $('.error').val('');
                $('.error-text').attr('src', '');
                $('.json-viewer2').jsonView('');

            };

            $('button.clean').click(function () {
                clean();
            });

            initJqBackand();
            updateUrl();

            $('label.use-backand-js input').prop('checked', true);

            $('button.submit').click(function () {
                var url = $('.url').val();
                var verb = $('.verb').val();
                var json = $('.json-input').val();
                var jsonData = json ? JSON.parse(json) : null;
                var data = {};

                if (verb == backand.options.verbs.get) {
                    var filter = $('.filter').val();

                    var filterData = filter ? JSON.parse(filter) : null;
                    data.filter = JSON.stringify(filterData);

                    var sort = $('.sort').val();

                    var sortData = sort ? JSON.parse(sort) : null;
                    data.sort = JSON.stringify(sortData);

                    var deep = $('label.deep input').prop('checked');
                    data.deep = deep;

                    var withSelectOptions = $('label.with-select-options input').prop('checked');
                    data.withSelectOptions = withSelectOptions;

                    var withFilterOptions = $('label.with-filter-options input').prop('checked');
                    data.withFilterOptions = withFilterOptions;

                    try{
                        var pageSize = parseInt($('.page-size').val());
                        data.pageSize = pageSize;

                    }
                    catch (err) { }

                    try {
                        var pageNumber = parseInt($('.page-number').val());
                        data.pageNumber = pageNumber;

                    }
                    catch (err) { }
                }
                else if (verb == "PUT" || verb == "POST") {
                    var json = $('.json-input').val();

                    var itemData = json ? JSON.parse(json) : null;
                    data = JSON.stringify(itemData);
                    
                }

                clean();
                
                var useJqBackand = $('label.use-backand-js input').prop('checked');
                if (!useJqBackand) {
                    api(url, data, verb,
                        function (data, textStatus, xhr) {
                            $('.json-output').val(JSON.stringify(data));
                            $('.json-viewer2').jsonView(data);

                            $('.status').val(xhr.status);
                            $('.location').val(xhr.getResponseHeader('location'));
                        },
                        function (xhr, textStatus, err) {
                            $('.error-text').attr('src', xhr.responseText);
                            $('.status').val(xhr.status);
                            $('.location').val(xhr.getResponseHeader('location'));
                        }
                    )
                }
                else {
                    var repository = $('.api-url-segment-repository select').val();
                    var configOrData = $('.api-url-segment-config select').val();
                    var success = function (data, textStatus, xhr) {
                        $('.json-output').val(JSON.stringify(data));
                        $('.json-viewer2').jsonView(data);

                        $('.status').val(xhr.status);
                        $('.location').val(xhr.getResponseHeader('location'));
                    };
                    var error = function (xhr, textStatus, err) {
                        $('.error-text').attr('src', xhr.responseText);
                        $('.status').val(xhr.status);
                        $('.location').val(xhr.getResponseHeader('location'));
                        switch (xhr.status) {
                            case 401:
                                login();
                                break;

                            default:
                                break;
                        }
                    };

                    if (repository == 'app' && configOrData == 'config') {
                        backand.api.app.getConfig(success, error);
                    }
                    else if (repository == 'view') {
                            var viewName = $('.api-url-segment-view select').val();

                            if (configOrData == 'config') {
                                backand.api.view.config.getItem(viewName, success, error);
                            }
                            else if (configOrData == 'data') {
                            var id = $('.api-url-segment-id input').val();
                            if (id) {
                                if (verb == backand.options.verbs.get) {
                                    backand.api.view.data.getItem(viewName, id, data.deep, success, error);
                                }
                                else if (verb == backand.options.verbs.put) {
                                    backand.api.view.data.updateItem(viewName, id, data, success, error);
                                }
                                else if (verb == backand.options.verbs.delete) {
                                    backand.api.view.data.deleteItem(viewName, id, success, error);
                                }
                            }
                            else {
                                if (verb == backand.options.verbs.post) {
                                    backand.api.view.data.createItem(viewName, data, success, error);
                                }
                                else {
                                    var filter = $('.filter').val();

                                    var filterData = filter ? JSON.parse(filter) : null;
                                    var sort = $('.sort').val();

                                    var sortData = sort ? JSON.parse(sort) : null;

                                    var search = $('.search').val();


                                    backand.api.view.data.getList(viewName, data.withSelectOptions, data.withFilterOptions, data.pageNumber, data.pageSize, filterData, sortData, search, success, error);
                                }
                            }
                        }
                        
                    }
                    else if (repository == 'dashboard') {
                        var id = $('.api-url-segment-dashboard select').val();

                        if (configOrData == 'config') {
                            backand.api.dashboard.config.getItem(id, success, error);
                        }
                        else if (configOrData == 'data') {
                            backand.api.dashboard.data.getItem(id, success, error);
                        }
                    }
                    else if (repository == 'chart') {
                        var id = $('.api-url-segment-chart select').val();

                        if (configOrData == 'config') {
                            backand.api.chart.config.getItem(id, success, error);
                        }
                        else if (configOrData == 'data') {
                            backand.api.chart.data.getItem(id, success, error);
                        }
                    }
                }


                var jsonViewer = $('.json-viewer').click(function () {
                    var url = "http://jsonviewer.stack.hu"; ///#http://rellymssql002.backand.loc:51065/app/config?id=d87edf91-a559-48c1-afc7-d1bb62ced6ae";
                    var win = window.open(url, '_blank');
                    //win.focus();
                });

                
                
            });

            
            repositorySelect.change(function () {
                var val = $(this).val();
                switch (val) {
                    case 'app':
                        $('.api-url-segment-page-type').css('display', 'none');
                        $('.api-url-segment-id').css('display', 'none');
                        updateUrl();
                        break;

                    case 'view':
                        $('.api-url-segment-page-type').css('display', 'none');
                        $('.api-url-segment-view').css('display', 'inline-block');
                        var configOrData = $('.api-url-segment-config select').val();
                        if (configOrData == 'data')
                            $('.api-url-segment-id').css('display', 'inline-block');
                        updateUrl();
                        var view = viewSelect.val();
                        if (view)
                            viewChanged(view);

                        break;
                    case 'dashboard':
                        $('.api-url-segment-page-type').css('display', 'none');
                        $('.api-url-segment-dashboard').css('display', 'inline-block');
                        $('.api-url-segment-id').css('display', 'none');
                        updateUrl();
                       
                        break;
                    case 'chart':
                        $('.api-url-segment-page-type').css('display', 'none');
                        $('.api-url-segment-chart').css('display', 'inline-block');
                        $('.api-url-segment-id').css('display', 'none');
                        updateUrl();

                        break;
                    default:
                        break;
                }
            });

            configOrDataSelect.change(function () {
                var val = $(this).val();
                switch (val) {
                    case 'config':
                        $('.api-url-segment-id').css('display', 'none');
                        updateUrl();
                        break;

                    case 'data':
                        var repository = repositorySelect.val();
                        if (repository == 'view')
                            $('.api-url-segment-id').css('display', 'inline-block');
                        updateUrl();
                        break;
                    default:
                        break;
                }
            });

            viewSelect.change(function () {
                var view = $(this).val();
                viewChanged(view);
            });

            dashboardSelect.change(function () {
                updateUrl();

            });

            chartSelect.change(function () {
                updateUrl();

            });

            var viewChanged = function (view) {
                cleanAndLoadFields($('#filterBuilder'), view);
                cleanAndLoadFields($('#sortBuilder'), view);

            }

            var cleanAndLoadFields = function (grid, view) {
                grid.datagrid('loadData', { "total": 0, "rows": [] });
                var opts = grid.datagrid('getColumnOption', 'fieldName');
                opts.editor = {
                    type: 'combobox',
                    options: {
                        valueField: 'fieldName',
                        textField: 'fieldName',
                        data: getFields(view)
                        //onChange: function (newValue, oldValue) {
                        //    if (newValue) {
                        //        var col = $('#filterBuilder').datagrid('getColumnOption', 'operator');
                        //        col.editor = {
                        //            type: 'combobox',
                        //            options: {
                        //                valueField: 'operator',
                        //                textField: 'operator',
                        //                data: getOperatorJson(newValue)
                        //            }
                        //        }
                        //        $(col.editor).combobox('setValues', getOperatorJson(newValue));
                        //    }
                        //}
                    }
                }
            }

            idInput.change(function () {
                var val = $(this).val();
                updateUrl();

            });


            //$('#dg').datagrid('loadData', dataGridData);
            ////var opts = $('#dg').datagrid('getColumnOption', 'productid');
            ////opts.editor = {
            ////    type: 'combobox',
            ////    options: productjson
            ////}
        

            //$('#filterBuilder').datagrid({
            //    onClickCell: function (rowIndex, field, value) {
            //        var operatorjson = getOperatorJson(rowIndex);
            //        var col = $(this).datagrid('getColumnOption', 'operator');
            //        col.editor = {
            //            type: 'combobox',
            //            options: {
            //                valueField: 'operator',
            //                textField: 'operator',
            //                data: operatorjson
            //            }
            //        }
                    
            //    }
            //});
        });

        function api(url, data, verb, successCallback, erroCallback) {
            $.ajax({
                url: url,
                //contentType: 'application/json; charset=utf-8',
                async: false,
                type: verb,
                data: data ,
                dataType: 'jsonp',
                cache: false,
                error: function (xhr, textStatus, err) { erroCallback(xhr, textStatus, err); },
                success: function (data, textStatus, xhr) { successCallback(data, textStatus, xhr); }
            });
        }

        function getOperatorJson(fieldName) {
            var operators = [];
            $(data2.fields).each(function () {
                if (this.internalName == fieldName) {
                    switch (getFilterType(this)) {
                        case 'numericOrDate':
                            operators = numericOrDateOperators;
                            break;
                        case 'text':
                            operators = textOperators;
                            break;
                        case 'relation':
                            operators = relationOperators;
                            break;

                        default:
                            break;
                    }
                }
            });

            return operators;
        }

        function getFilterType(field) {
            if (field.type == "Numeric" || field.type == "Date")
                return 'numericOrDate';
            else if (field.type == "SingleSelect" || field.type == "MultiSelect")
                return 'relation';
            else
                return 'text';
        }

        function getFields(view) {
            var data3 = [];
            api('/1/view/config/' + view, null, "GET", function (data) {
                data2 = data;
            });

            $(data2.fields).each(function () {
                data3.push({ fieldName: this.internalName });
            });

            return data3;
        }

        </script>

    <script type="text/javascript">
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#filterBuilder').datagrid('validateRow', editIndex)) {
                var ed = $('#filterBuilder').datagrid('getEditor', { index: editIndex, field: 'fieldName' });
                if (!ed) return true;
                var fieldName = $(ed.target).combobox('getText');
                $('#filterBuilder').datagrid('getRows')[editIndex]['fieldName'] = fieldName;
                $('#filterBuilder').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#filterBuilder').datagrid('selectRow', index)
							.datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#filterBuilder').datagrid('selectRow', editIndex);
                }
            }
        }
        function append() {
            if (endEditing()) {
                $('#filterBuilder').datagrid('appendRow', {  });
                editIndex = $('#filterBuilder').datagrid('getRows').length - 1;
                $('#filterBuilder').datagrid('selectRow', editIndex)
						.datagrid('beginEdit', editIndex);
            }
        }
        function removeit() {
            if (editIndex == undefined) { return }
            $('#filterBuilder').datagrid('cancelEdit', editIndex)
					.datagrid('deleteRow', editIndex);
            editIndex = undefined;
        }
        function accept() {
            if (endEditing()) {
                $('#filterBuilder').datagrid('acceptChanges');
                var myData = $('#filterBuilder').datagrid('getData');
                $('.filter').val(JSON.stringify(myData.rows));
            }
        }
        function reject() {
            $('#filterBuilder').datagrid('rejectChanges');
            editIndex = undefined;
        }
        function getChanges() {
            var rows = $('#filterBuilder').datagrid('getChanges');
            alert(rows.length + ' rows are changed!');
        }
	</script>

    <script type="text/javascript">
        var editSortIndex = undefined;
        function endSortEditing() {
            if (editSortIndex == undefined) { return true }
            if ($('#sortBuilder').datagrid('validateRow', editSortIndex)) {
                var ed = $('#sortBuilder').datagrid('getEditor', { index: editSortIndex, field: 'fieldName' });
                if (!ed) return true;
                var fieldName = $(ed.target).combobox('getText');
                $('#sortBuilder').datagrid('getRows')[editSortIndex]['fieldName'] = fieldName;
                $('#sortBuilder').datagrid('endEdit', editSortIndex);
                editSortIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editSortIndex != index) {
                if (endSortEditing()) {
                    $('#sortBuilder').datagrid('selectRow', index)
							.datagrid('beginEdit', index);
                    editSortIndex = index;
                } else {
                    $('#sortBuilder').datagrid('selectRow', editSortIndex);
                }
            }
        }
        function appendSort() {
            if (endSortEditing()) {
                $('#sortBuilder').datagrid('appendRow', {});
                editSortIndex = $('#sortBuilder').datagrid('getRows').length - 1;
                $('#sortBuilder').datagrid('selectRow', editSortIndex)
						.datagrid('beginEdit', editSortIndex);
            }
        }
        function removeitSort() {
            if (editSortIndex == undefined) { return }
            $('#sortBuilder').datagrid('cancelEdit', editSortIndex)
					.datagrid('deleteRow', editSortIndex);
            editSortIndex = undefined;
        }
        function acceptSort() {
            if (endSortEditing()) {
                $('#sortBuilder').datagrid('acceptChanges');
                var myData = $('#sortBuilder').datagrid('getData');
                $('.sort').val(JSON.stringify(myData.rows));
            }
        }
        function rejectSort() {
            $('#sortBuilder').datagrid('rejectChanges');
            editSortIndex = undefined;
        }
        function getChangesSort() {
            var rows = $('#sortBuilder').datagrid('getChanges');
            alert(rows.length + ' rows are changed!');
        }
	</script>

    <style>
        div.api {padding:10px 10px 10px 10px;}

        .api-input {
            display:inline-block;
            float:left;
            width:600px;
        }
        .api-output {
            margin-left:620px;
        }
       .api fieldset  { display: table; }
        .api p     { display: table-row;  }
        .api label { display: table-cell; padding-top: 10px;}
        .api input { display: table-cell; width:40%;height:20px;}
        .api input[type=checkbox] { width:inherit;}
        .api select { display: table-cell; width:40%;height:20px;width:auto;}
        .api textarea { display: table-cell; width:80%;}

         fieldset   button.submit {
                margin-top:40px;
            }

       fieldset  .upper-tb button.submit {
                margin-top:0;
            }
        .api  iframe {
            width:80%;
            height:auto;
        }
        .api-url{padding: 0; }
        .api-url-segment {
            display: inline-block;
        }
        .api div.api-url-segment{
            padding: 0;
            padding-left: 5px;
        }
        .url{width:80%;}
        .api-url-segment-page-type {display:none;}
        .api-url-segment-id {display:none;}
        label.check-label{display:block;}
        label input{width:initial;margin: 0;
    vertical-align: bottom;
    position: relative;
    top: 1.999px;}

        .api-url fieldset {
            border: 1px solid #e5e5e5;
            padding: 5px;
        }
            .api-url fieldset legend {
                border:initial;
                margin: 0;
                padding: 0;
                width: inherit;
            }

            .json-viewer2{display:inherit;}
        .login-container {
        }
        input.url{width:100%;}
    </style>
</head>
<body>
    <!--
    <style>#backand-banner .container {width: 100%;}#backand-banner .navbar-nav > li > a {padding-top:0;padding-bottom:0;color: #4c4c4c;}#backand-banner .navbar-brand{padding-top:0;}#backand-banner .navbar-nav > li > a {padding-top:5px !important; padding-bottom:5px !important;}#backand-banner.navbar {min-height:20px !important;background: #f3b858;border-bottom: 1px solid #1ba085;}#backand-banner .navbar-brand {padding-top: 5px;padding-bottom: 0;color: #4c4c4c;}</style>
    <nav id="backand-banner" class="navbar navbar-default" role="navigation"><div class="container"><div class="navbar-header"><button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1"><span class="sr-only">Toggle navigation</span><span class="icon-bar"></span><span class="icon-bar"></span><span class="icon-bar"></span></button><a class="navbar-brand" href="https://www.backand.com/">Back&</a></div><div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1"><ul class="nav navbar-nav"><li><a href="[adminUrl]/Admin/Pages?menuId=">Pages</a></li></ul><ul class="nav navbar-nav navbar-right"><li class="dropdown"><a href="#" data-toggle="dropdown" class="dropdown-toggle">aaaa@aaaa.com<b class="caret"></b></a><ul class="dropdown-menu"><li><a href="[adminUrl]/Account/ChangePassword">Change Password</a></li><li><a href="https://www.backand.com/apps">My Consoles</a></li><li><a href="https://www.backand.com/support">Support</a></li></ul></li></ul><ul class="nav navbar-nav navbar-right"><li class="dropdown"><a href="#" data-toggle="dropdown" class="dropdown-toggle">Admin <b class="caret"></b></a><ul class="dropdown-menu"><li><a href="[adminUrl]/Admin/Index/Database">Default Settings</a></li><li><a href="[adminUrl]/Admin/Index/View">Tables &amp; Views</a></li><li><a href="[adminUrl]/Admin/Index/Workspace">Workspaces</a></li><li><a href="[adminUrl]/Home/IndexPage/v_durados_User">Users</a></li><li><a href="[adminUrl]/Admin/Index/Rule">Business Rules</a></li><li><a href="[adminUrl]/Home/Index/Durados_Log">Trace</a></li></ul></li></ul></div></div></nav>
        -->
    <div class="api">
        <div class="api-input">
            <fieldset>
                <legend>Input</legend>
                <div class="upper-tb">
                <button class="clean">clean</button>
                <button class="submit">submit</button>
                </div>

                <div class="api-url api-url-segments">
                    <fieldset>
                        <legend>namespaces:</legend>
                 
                    <label class="check-label use-backand-js"><input type="checkbox"/>use backand.js</label>
                    <div class="api api-url-segment api-url-segment-version">
                        <label>version:</label>
                        <select>
                            <option value="1">1</option>
                        </select>
                        /
                    </div>
                    <div class="api api-url-segment api-url-segment-repository">
                    <label>content part:</label>
                    <select>
                        <option value="app">app</option>
                        <option value="view">view</option>
                        <option value="dashboard">dashboard</option>
                        <option value="chart">chart</option>
                        <option value="content">content</option>
                    </select>
                    /
                    </div>
                    <div class="api api-url-segment api-url-segment-config">
                    <label>config/data:</label>
                    <select>
                        <option value="config">config</option>
                        <option value="data">data</option>
                    </select>
                    /
                    </div>
                    <div class="api api-url-segment api-url-segment-view api-url-segment-page-type">
                    <label>view:</label>
                    <select>
                        <%
                            foreach (Durados.View view in Durados.Web.Mvc.Maps.Instance.GetMap().Database.Views.Values.Where(v=>!v.SystemView))
                            {
                        %>
                            <option value="<%=view.Name %>"><%=view.Name %></option>
                        <%} %>
                    </select>
                    /
                    </div>
                     <div class="api api-url-segment api-url-segment-dashboard api-url-segment-page-type">
                    <label>id:</label>
                    <select>
                        <%
                            
                            foreach (int dashboardId in Durados.Web.Mvc.Maps.Instance.GetMap().Database.Dashboards.Keys)
                            {
                        %>
                            <option value="<%=dashboardId %>"><%= dashboardId + " - (" + Durados.Web.Mvc.Maps.Instance.GetMap().Database.Dashboards[dashboardId].Name + ")" %></option>
                        <%} %>
                    </select>
                    /
                    </div>
                    <div class="api api-url-segment api-url-segment-chart api-url-segment-page-type">
                    <label>id:</label>
                    <select>
                        <%
                            
                            foreach (Durados.MyCharts dashboard in Durados.Web.Mvc.Maps.Instance.GetMap().Database.Dashboards.Values)
                            {
                                foreach (Durados.Chart chart in dashboard.Charts.Values)
                                { 
                        %>
                            <option value="<%=chart.ID %>"><%=chart.ID + " - (" + chart.Name + ")" %></option>
                        <%  }
                          } %>
                    </select>
                    /
                    </div>
                    <div class="api api-url-segment api-url-segment-id">
                    <label>id:</label>
                    <input type="text"/>
                    
                    </div>
                        </fieldset>
                
                </div>

                <div class="api api-url api-url-query">
                <fieldset>
                    <legend>query:</legend>
                
                    <label class="check-label deep"><input type="checkbox"/>deep</label>
                    <label class="check-label with-select-options"><input type="checkbox"/>with select options</label>
                    <label class="check-label with-filter-options"><input type="checkbox"/>with filter options</label>
                    <label>page number:</label>
                    <input type="number" class="page-number"/>
                    <label>page size:</label>
                    <input type="number" class="page-size"/>
                    <label>sort:</label>
                    <div style="display:inherit;float:inherit;">

                        <table id="sortBuilder" class="easyui-datagrid" title="Create Sort Parameters" style="width:400px;height:auto"
			                    data-options="
				                    iconCls: 'icon-edit',
				                    singleSelect: true,
				                    toolbar: '#tbSort',
				                    method: 'get',
				                    onClickRow: onClickRow
			                    ">
		                    <thead>
			                    <tr>
				                    <th data-options="field:'fieldName',width:100,
						                    formatter:function(value,row){
							                    return row.fieldName;
						                    },
						                    editor:{
							                    type:'combobox',
							                    options:{
								                    valueField:'fieldName',
								                    textField:'fieldName',
								                    <%--url:'products.json',--%>
								                    required:true
							                    }
						                    }">Column</th>
				                    <th data-options="field:'order',width:100,
						                    formatter:function(value,row){
							                    return row.order;
						                    },
						                    editor:{
							                    type:'combobox',
							                    options:{
								                    valueField:'order',
								                    textField:'order',
								                    <%--url:'products.json',--%>
                                                    data:[{ 'order': 'asc' }, { 'order': 'desc' }],
								                    required:true
							                    }
						                    }">Order</th>
			                    </tr>
		                    </thead>
	                    </table>

	                    <div id="tbSort" style="height:auto">
		                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="appendSort()">Append</a>
		                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="removeitSort()">Remove</a>
		                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="acceptSort()">Accept</a>
		                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-undo',plain:true" onclick="rejectSort()">Reject</a>
		                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" onclick="getChangesSort()">GetChanges</a>
	                    </div>

                </div>
                    <input type="text" class="sort"/>
                    <label>filter:</label>

                    <div style="display:inherit;float:inherit;">

                        <table id="filterBuilder" class="easyui-datagrid" title="Create Filter Parameters" style="width:400px;height:auto"
			                    data-options="
				                    iconCls: 'icon-edit',
				                    singleSelect: true,
				                    toolbar: '#tbFilter',
				                    method: 'get',
				                    onClickRow: onClickRow
			                    ">
		                    <thead>
			                    <tr>
				                    <th data-options="field:'fieldName',width:100,
						                    formatter:function(value,row){
							                    return row.fieldName;
						                    },
						                    editor:{
							                    type:'combobox',
							                    options:{
								                    valueField:'fieldName',
								                    textField:'fieldName',
								                    <%--url:'products.json',--%>
								                    required:true
							                    }
						                    }">Column</th>
				                    <th data-options="field:'operator',width:100,
						                    formatter:function(value,row){
							                    return row.operator;
						                    },
						                    editor:{
							                    type:'combobox',
							                    options:{
								                    valueField:'operator',
								                    textField:'operator',
								                    <%--url:'products.json',--%>
                                                    data:[{ 'operator': 'equals' }, { 'operator': 'notEquals' }, { 'operator': 'greaterThan' }, { 'operator': 'greaterThanOrEqualsTo' }, { 'operator': 'lessThan' }, { 'operator': 'lessThanOrEqualsTo' }, { 'operator': 'empty' }, { 'operator': 'notEmpty' }, { 'operator': 'startsWith' }, { 'operator': 'endsWith' }, { 'operator': 'contains' }, { 'operator': 'notContains' },{ 'operator': 'in' }],
								                    required:true
							                    }
						                    }">Operator</th>
				                    <th data-options="field:'value',width:150,editor:'text'">Value</th>
			                    </tr>
		                    </thead>
	                    </table>

	                    <div id="tbFilter" style="height:auto">
		                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="append()">Append</a>
		                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="removeit()">Remove</a>
		                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="accept()">Accept</a>
		                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-undo',plain:true" onclick="reject()">Reject</a>
		                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" onclick="getChanges()">GetChanges</a>
	                    </div>

                </div>

                    <input type="text" class="filter"/>
                    
                    <label>search:</label>
                    <input type="text" class="search"/>
                    
                    <br /><br />
                    
                    <button class="logoButton">logo</button>
                    <img class="logo"/>

                </fieldset>
                </div>

                <label>url:</label>
                <input type="text" class="url" value="/1/app/config"/>
                
                <label>data (json):</label>
                <textarea class="json json-input"></textarea>
                
                <label>verb:</label>
                <select class="verb">
                    <option value="GET">GET</option>
                    <option value="PUT">PUT</option>
                    <option value="POST">POST</option>
                    <option value="DELETE">DELETE</option>
                </select>

                
                <br />
                <button class="clean">clean</button>
                <button class="submit">submit</button>
                
            </fieldset>
        </div>
        <div class="api-output">
            <fieldset>
                <legend>Output</legend>
            
                <label>status:</label>
                <input type="text" class="status" />

                <label>json output:</label>
                <textarea class="json json-output"></textarea>

                <button class="json-viewer">json viewer</button>

                <div class="json-viewer2">

                </div>

                <label>location:</label>
                <input type="text" class="location" />

                <label>error:</label>
                <iframe class="json error-text"></iframe>
                
            </fieldset>
        </div>
        
    </div>
    <div class="login-container">
        <label>username:</label>
        <input class="username" type="text" value="<%=Durados.Web.Mvc.Maps.Instance.GetMap().Database.GetCurrentUsername() %>"/>
        <label>password:</label>
        <input class="password" type="password" autocomplete="on" value="" name="aaaaa"/>
        <label>appname:</label>
        <input class="appname" type="text" value="<%= Durados.Web.Mvc.Maps.Instance.GetMap().AppName%>"/>
        <button class="login">login</button>
     </div>
</body>
</html>
