<%--<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>--%>
<%@ Page Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewPage<System.Data.DataView>" %>
 <%

    string CssPath = ResolveUrl("~/Content/");
    string JsPath = ResolveUrl("~/Scripts/");
     //<!-- SYNC VIEW WITH SERVER -->
    string saveERDUrl = Url.Action("ERDSaveState", "Admin");
    bool debug = System.Configuration.ConfigurationManager.AppSettings["Debug"].ToLower() == "true";
   
    %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!--
	WWW SQL Designer, (C) 2005-2011 Ondrej Zara, ondras@zarovi.cz
	Version: 2.6
	See license.txt for licencing information.
-->
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>ERD </title>

 	<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <%if (debug)
      { %>
        <link rel="stylesheet" type="text/css" href='<%= CssPath %>erd/style.css' media="all" />     
        <link rel="stylesheet" type="text/css" href='<%= CssPath %>erd/print.css'  media="print" />
        <script type="text/javascript" src='<%= JsPath %>jquery-1.4.2.min.js'></script>
        <script type="text/javascript" src='<%= JsPath %>dbdesigner.oz.js'></script>
	    <script type="text/javascript" src='<%= JsPath %>dbdesigner.config.js'></script>
	    <script type="text/javascript" src='<%= JsPath %>dbdesigner.js'></script>
    <%}
      else
      { %>
        <link rel="stylesheet" type="text/css" href='<%= CssPath %>erd/style.css' media="all" />     
        <link rel="stylesheet" type="text/css" href='<%= CssPath %>erd/print.css'  media="print" />
        <script type="text/javascript" src='<%= JsPath %>dbdesigner-1.0.0.min.js'></script>

    <%} %>
</head>

<body>

	<div id="area"></div>

	<div id="controls">
		<div id="bar">
            <input type="button" id="saveERD" title="Save" onclick="<%=saveERDUrl %>"  value="Save"/>
            <input type="button" id="aligntables" />
			
            <div id="toggle"></div>
            <div style="display:none">
			<input type="button" id="saveload"  />
           
			<hr/>
            
			<input type="button" id="addtable" />
			<input type="button" id="edittable" />
			<input type="button" id="tablekeys" />
			<input type="button" id="removetable" />
			<input type="button" id="aligntables1" />
			<input type="button" id="cleartables" />
		
			<hr/>
		
			<input type="button" id="addrow" />
			<input type="button" id="editrow" />
			<input type="button" id="uprow" class="small" /><input type="button" id="downrow" class="small"/>
			<input type="button" id="foreigncreate" />
			<input type="button" id="foreignconnect" />
			<input type="button" id="foreigndisconnect" />
			<input type="button" id="removerow" />
		
			<hr/>
		
			<input type="button" id="options" />
			<%--<a href="http://code.google.com/p/wwwsqldesigner/w/list" target="_blank">--%>
            <input type="button" id="docs" value="" /><%--</a>--%>
            </div>
		</div>
	
		<div id="rubberband"></div>

		<div id="minimap"></div>
	
		<div id="background"></div>
	
		<div id="window">
			<div id="windowtitle"><img id="throbber" src="images/throbber.gif" alt="" title=""/></div>
			<div id="windowcontent"></div>
			<input type="button" id="windowok" />
			<input type="button" id="windowcancel" />
		</div>
	</div> <!-- #controls -->
	
	<div id="opts">
		<table>
			<tbody>
				<tr>
					<td>
						* <label id="language" for="optionlocale"></label>
					</td>
					<td>
						<select id="optionlocale"></select>
					</td>
				</tr>
				<tr>
					<td>
						* <label id="db" for="optiondb"></label> 
					</td>
					<td>
						<select id="optiondb"></select>
					</td>
				</tr>
				<tr>
					<td>
						<label id="snap" for="optionsnap"></label> 
					</td>
					<td>
						<input type="text" size="4" id="optionsnap" />
						<span class="small" id="optionsnapnotice"></span>
					</td>
				</tr>
				<tr>
					<td>
						<label id="pattern" for="optionpattern"></label> 
					</td>
					<td>
						<input type="text" size="6" id="optionpattern" />
						<span class="small" id="optionpatternnotice"></span>
					</td>
				</tr>
				<tr>
					<td>
						<label id="hide" for="optionhide"></label> 
					</td>
					<td>
						<input type="checkbox" id="optionhide" />
					</td>
				</tr>
				<tr>
					<td>
						* <label id="vector" for="optionvector"></label> 
					</td>
					<td>
						<input type="checkbox" id="optionvector" />
					</td>
				</tr>
				<tr>
					<td>
						* <label id="showsize" for="optionshowsize"></label> 
					</td>
					<td>
						<input type="checkbox" id="optionshowsize" />
					</td>
				</tr>
				<tr>
					<td>
						* <label id="showtype" for="optionshowtype"></label> 
					</td>
					<td>
						<input type="checkbox" id="optionshowtype" />
					</td>
				</tr>
			</tbody>
		</table>

		<hr />

		* <span class="small" id="optionsnotice"></span>
	</div>
	
	<div id="io">
		<table>
			<tbody>
				<tr>
					<td>
						<fieldset>
							<legend id="client"></legend>
							<input type="button" id="clientsave" /> 
							<input type="button" id="clientload" />
							<hr/>
							<input type="button" id="clientsql" />
						</fieldset>
					</td>
					<td>
						<fieldset>
							<legend id="server"></legend>
							<label for="backend" id="backendlabel"></label> <select id="backend"></select>
							<hr/>
							<input type="button" id="serversave" /> 
							<input type="button" id="quicksave" /> 
							<input type="button" id="serverload" /> 
							<input type="button" id="serverlist" /> 
							<input type="button" id="serverimport" /> 
						</fieldset>
					</td>
				</tr>
				<tr>
					<td colspan="2">
						<fieldset>
							<legend id="output"></legend>
							<textarea id="textarea"></textarea>
						</fieldset>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
	
	<div id="keys">
		<fieldset>
			<legend id="keyslistlabel"></legend> 
			<select id="keyslist"></select>
			<input type="button" id="keyadd" />
			<input type="button" id="keyremove" />
		</fieldset>
		<fieldset>
			<legend id="keyedit"></legend>
			<table>
				<tbody>
					<tr>
						<td>
							<label for="keytype" id="keytypelabel"></label>
							<select id="keytype"></select>
						</td>
						<td></td>
						<td>
							<label for="keyname" id="keynamelabel"></label>
							<input type="text" id="keyname" size="10" />
						</td>
					</tr>
					<tr>
						<td colspan="3"><hr/></td>
					</tr>
					<tr>
						<td>
							<label for="keyfields" id="keyfieldslabel"></label><br/>
							<select id="keyfields" size="5" multiple="multiple"></select>
						</td>
						<td>
							<input type="button" id="keyleft" value="&lt;&lt;" /><br/>
							<input type="button" id="keyright" value="&gt;&gt;" /><br/>
						</td>
						<td>
							<label for="keyavail" id="keyavaillabel"></label><br/>
							<select id="keyavail" size="5" multiple="multiple"></select>
						</td>
					</tr>
				</tbody>
			</table>
		</fieldset>
	</div>
	
	<div id="table">
    
		<table>
			<tbody>
				<tr>
					<td>
						<label id="tablenamelabel" for="tablename"></label>
					</td>
					<td>
						<input id="tablename" type="text" />
					</td>
				</tr>
				<tr>
					<td>
						<label id="tablecommentlabel" for="tablecomment"></label> 
					</td>
					<td>
						<textarea rows="5" cols="40" id="tablecomment"></textarea>
					</td>
				</tr>
			</tbody>
		</table>
	
    </div>
	
	<script type="text/javascript">
   
        var saveStateUrl ='<%=saveERDUrl%>';
        SQL.Designer.prototype.DuradosInit = function () {
            SQL.Row.prototype.dblclick = function (e) { /* dblclicked on row */
                //var fieldName = this.dom.content.getElementsByClassName("erdField")[0].innerHTML;
                parent.showProgress();
                var fieldViewName = jQuery(".erdField", this.dom.content)[0].innerHTML;

                var fieldViewNameArr = fieldViewName.split(",");
                if (fieldViewNameArr.length == 2) {
                    var inlineEditUrl = '/Admin/InlineEditingEdit/';
                    var viewName = fieldViewNameArr[0];
                    var fieldName = fieldViewNameArr[1];
                    var fieldId = parent.GetFieldPK(viewName, fieldName);
                    parent.InlineEditingDialog.CreateAndOpen2("Field", "Field", null, null, inlineEditUrl, parent.erdguid, null, fieldId, true, afterInlineEditing);
                    parent.hideProgress();
                }
                OZ.Event.prevent(e);
                OZ.Event.stop(e);
                //                this.expand();
            }
            SQL.Table.prototype.dblclick = function (e) { /* dblclicked on row */
                parent.showProgress();
                var viewNames = jQuery(".erdField", this.dom.content)[0].innerHTML;
                var viewNamesArr = viewNames.split(",");
                var viewName = viewNamesArr[0];

                var inlineEditUrl = '/Admin/InlineEditingEdit/';

                var viewId = parent.GetViewPK(viewName);
                parent.InlineEditingDialog.CreateAndOpen2("View", "View", null, null, inlineEditUrl, parent.erdguid, null, viewId, true, afterInlineEditing);
                parent.hideProgress();
                //                OZ.Event.prevent(e);
                //                OZ.Event.stop(e);
                //                this.expand();
            }
        };

        window.onbeforeunload = null; 

        function afterInlineEditing(viewName, type, id, editUrl, guid, dialog, pk) {
            parent.commitAndReload();
        }

        SQL.Designer.prototype.requestDB = function () {
            this.init2();
        }
        var d = new SQL.Designer();

//        $("#saveERD").attr('value','Save Position');
//        $("#aligntables").attr('value','Align Tables');
        $("#saveERD").attr('value', '<%= Map.Database.Localizer.Translate("Save Position")%>');
        $("#aligntables").attr('value', '<%= Map.Database.Localizer.Translate("Align Tables")%>');
	</script>
</body>
</html>