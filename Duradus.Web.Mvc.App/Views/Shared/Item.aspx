<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<Durados.Web.Mvc.UI.Item>" %>

<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<asp:Content ID="Content3" ContentPlaceHolderID="headCSS" runat="server">
    <%=((Durados.Web.Mvc.View)Database.Views[Model.ViewName]).GetStyleSheets()%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        document.domain = '<%=Durados.Web.Mvc.Maps.Domain %>';
    </script>
    <%=((Durados.Web.Mvc.View)Database.Views[Model.ViewName]).GetScripts()%>
    <%
        
        if (!string.IsNullOrEmpty(Request.QueryString["plugIn"]))
        {
            string plugIn = Request.QueryString["plugIn"];
    %>
    <script type="text/javascript" src='<%=ResolveUrl("~/Scripts/PlugIn/wix.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/Scripts/PlugIn/item.js")%>'></script>
    <script>

        var jsonModel = window.parent.jsonModel;
    
    </script>
    <style type="text/css">
        .ui-dialog-buttonpane
        {
            display: none !important;
        }
    </style>
    <% }            
    %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% string guid = Model.Guid; %>
    <% Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[Model.ViewName]; %>
    <%=Html.Hidden("a", Durados.Web.Mvc.Infrastructure.General.GetRootPath(), new { id = "GetRootPath" })%>
    <%=Html.Hidden("a", Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(new Durados.Web.Mvc.UI.Json.Translator(Map.Database)), new { id = "translator" })%>
    <%  Html.RenderPartial("~/Views/Shared/Controls/ClientParams.ascx", new Durados.Web.Mvc.UI.ClientParams() { View = view, Guid = guid }); %>
    <script type="text/javascript">
        var rootPath = $('#GetRootPath').val();
        var translator = Sys.Serialization.JavaScriptSerializer.deserialize($('#translator').val());
        var d_autoCommit = '<%= Map.Database.AutoCommit %>' == 'True';

        $(document).ready(function () {

            if (isDock()) {
                Durados.DuringdLoadIframe = true;
            }
            isItem = true;

            var scope = getDocumentScope();
            $('iframe[name=viewProp]', scope).show();

            //            showProgress();
            //initDataTableView('<%=guid %>');
            //            var createDialog = $('#' + '<%=guid %>' + 'DataRowCreate');
            //            var html = createDialog.html();
            //            createDialog.html('');

            InitItemDialog('<%=guid %>');
            var pk = queryString('pk');

            //pk = pk == '' || pk == "undefined" ? null : pk;
            var add = queryString('add');
            if (add == 'yes') {
                var allowEdit = queryString('allowEdit') == "True";
                var insertAbovePK = queryString('insertAbovePK');
                var duplicateRecursive = queryString('duplicateRecursive') == "True";
                AddDialog.Open(pk, '<%=guid %>', true, allowEdit, insertAbovePK, duplicateRecursive);
                if (isMobile()) {
                    //                    setTimeout(function () {
                    var dialog = $('#' + '<%=guid %>' + 'DataRowCreate');

                    var bs = $('.ui-dialog-buttonset:last');
                    var bsc = bs.clone();
                    var referBar = $('<div class="refer-bar"></div>').css('padding-left', 0);
                    referBar.prepend(bsc);
                    //dialog.prepend(referBar);
                    $('body').prepend(referBar);
                    dialog.css('height', ($(window).height() - 80) + 'px');

                    bsc.addClass('mobile-buttonset');
                    bs.hide();
                    bsc.find('button:first').click(function () { bs.find('button:first').click(); });
                    bsc.find('button:last').click(function () { bs.find('button:last').click(); });

                    var b = bsc.find('button:last');
                    var bc = b.clone();
                    //b.parent().append($('<span><%=view.Name %></span>'));
                    b.parent().append(bc);
                    bc.text('Back');
                    //bsc.find('button:last').css('float', 'right');
                    bsc.find('button:last').click(function () { document.location.href = '/Home/Index/' + '<%=view.Name %>'; });
                    $('#' + '<%=guid %>' + 'DataRowCreate').parent().addClass('data-dialog');
                    //alert(dialog.parents('.ui-dialog').length);
                    //                        $(window).resize(function () {
                    //                            setTimeout(function () {
                    //                                $('#' + '<%=guid %>' + 'DataRowCreate').parent().css('top', 55);
                    //                            }, 1000);
                    //                        });

                    //                            //$('#' + '<%=guid %>' + 'DataRowCreate').parent().css('top', 55);
                    //                            $(window).resize();


                    //                    }, 1);
                }

            }
            else {
                EditDialog.Open(pk, '<%=guid %>', true, function () {
                    $('#loadSettings', scope).hide();
                    if (isDock()) {
                        Durados.DuringdLoadIframe = false;
                    }
                });

                if (isMobile()) {
                    setTimeout(function () {
                        //alert($('button:contains("' + translator.save + '")').length + ' ' + translator.save);

                        //                    $('button:containsExact("' + translator.save + '"):first').text('aaa');
                        var dialog = $('#' + '<%=guid %>' + 'DataRowEdit').css('top', 0).parent();
                        var bs = $('.ui-dialog-buttonset:last');
                        var bsc = bs.clone();
                        var referBar = $('<div class="refer-bar"></div>').css('padding-left', 0);
                        referBar.prepend(bsc);
                        //dialog.prepend(referBar);
                        $('body').prepend(referBar);

                        bsc.addClass('mobile-buttonset');
                        bs.hide();
                        bsc.find('button:first').click(function () { bs.find('button:first').click(); });

                        var b = bsc.find('button:last');
                        var bc = b.clone();
                        // b.parent().append($('<span><%=view.Name %></span>'));
                        b.parent().append(bc);
                        $('body').prepend(bsc);
                        bc.text('Back');
                        //bsc.find('button:last').css('float', 'right');
                        bsc.find('button:last').click(function () { document.location.href = '/Home/Index/' + '<%=view.Name %>'; });
                        $('#' + '<%=guid %>' + 'DataRowEdit').parent().addClass('data-dialog');
                        //                        setTimeout(function () {
                        //                            $('#' + '<%=guid %>' + 'DataRowEdit').parent().css('top', 55);
                        //                        }, 1000);

                    }, 1);
                }
            }

            //           WIXWIX if (isDock()) {
            //                $('div[hasguid="hasguid"]').css('background', '#5a5a5a');
            //                setTimeout(function () {
            //                    $('div.ui-dialog-buttonpane').css('background', '#5a5a5a'); $('div.ui-dialog-buttonpane').css('margin', '0');
            //                }, 1);
            //            }

            if (isDock()) {
                $('div[hasguid=hasguid]').css('padding-left', '0')
                $('.ui-dialog.ui-widget.ui-widget-content').css('border-left', 'none')
                $('div.ui-dialog-titlebar').css('background', 'White').css('font-size', '18px').css('color', 'rgb(161,161,161)').css('font-weight', 'normal');
                $('div.ui-dialog-titlebar .ui-dialog-titlebar-max').css('margin-right', '0');


                $('.ui-accordion').addClass('ui-accordion-settings');
                //$('.ui-accordion>h3.ui-accordion-header').css('background', 'rgb(241,241,241)').css('color', 'rgb(76,76,76)').css('font-size', '14px');
                //$('.ui-accordion>h3.ui-accordion-header.ui-state-active').css('background', 'rgb(242,101,34)').css('color', 'White').css('font-weight', 'bold');
                $('.ui-accordion>h3.ui-accordion-header.ui-state-active .ui-icon.ui-icon-triangle-1-s').hide();
                //                $('.ui-accordion>h3.ui-accordion-header.ui-state-active .ui-icon.ui-icon-resizethick.ui-accordion-header-open').css('background', 'url("/content/images/collapse-v.png") 97% 2px no-repeat').show();
                $('.ui-accordion>h3.ui-accordion-header').each(function () {
                    $(this).find('.ui-icon.ui-icon-triangle-1-e:first').hide();
                    //                    $(this).find('.ui-icon.ui-icon-triangle-1-e.ui-accordion-header-open').css('background', 'url("/content/images/expand.png") 97% 2px no-repeat');
                    //                    $(this).find('.ui-icon.ui-icon-resizethick:last').css('background', 'url("/content/images/expand-v.png") 97% 2px no-repeat');
                });

                setTimeout(function () {
                    $('div.ui-dialog-buttonpane').css('margin', '0');
                    $('.ui-dialog-buttonset').find('button.ui-button').css('background', 'rgba(27, 160, 133, 1)').css('color', 'white').css('font-weight', 'bold').css('border', 'none');


                }, 1);
            }

        });

        document.title = '<%= Map.Database.Localizer.Translate(Map.Database.SiteInfo == null ? Map.Database.DisplayName : (string.IsNullOrEmpty(Map.Database.SiteInfo.Product) ? Map.Database.DisplayName : Map.Database.SiteInfo.Product)) + " - " + Map.Database.Localizer.Translate(view.DisplayName) %>';
    
    </script>
    <div style="display: none;" id="<%=guid %>ajaxDiv" ajaxdiv="ajaxDiv" guid="<%=guid %>">
    </div>
    <%  ViewData["guid"] = guid;
                    //if (!Durados.Web.Infrastructure.General.IsMobile())
                    //    Html.RenderPartial("~/Views/Shared/Controls/MainMenu.ascx"); %>
    <div style="display: none;" id="<%=guid %>DataRowCreate" hasguid='hasguid' guid='<%=guid %>'
        itemdatarowview='itemDataRowViewCreate'>
        <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowView.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Create), DataAction = Durados.DataAction.Create, Guid = guid, View = view }); %>
    </div>
    <div style="display: none;" id="<%=guid %>DataRowEdit" hasguid='hasguid' guid='<%=guid %>'
        itemdatarowview='itemDataRowViewEdit'>
        <%  Html.RenderPartial("~/Views/Shared/Controls/DataRowView.ascx", new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Edit), DataAction = Durados.DataAction.Edit, Guid = guid, View = view }); %>
    </div>
    <div style="display: none" id="DeleteMessage" hasguid='hasguid' guid='<%=guid %>'>
        <%  Html.RenderPartial("~/Views/Shared/Controls/DeleteMessage.ascx", String.Empty); %>
    </div>
    <div style="display: none" id="DeleteSelectionMessage">
        <%  Html.RenderPartial("~/Views/Shared/Controls/DeleteMessage.ascx", ViewData["DeleteConfirmationMessage"]); %>
    </div>
    <div style="display: none" id="rich">
        <textarea id='richTextArea' class='wtextarea' hasguid='hasguid' guid='<%=guid %>'></textarea>
    </div>
    <%  Html.RenderPartial("~/Views/Shared/Controls/Url.ascx"); %>
    <div style="display: none;" id="Div1">
    </div>
    <div id="pluginProgress" class="overlay" style="display: none;">
        <div>
        </div>
        <div class="loading">
            <div>
                <%=Map.Database.Localizer.Translate("Performs Changes...")%></div>
            <img src="/Content/Images/wait.gif" alt="loading..." /></div>
    </div>
</asp:Content>
