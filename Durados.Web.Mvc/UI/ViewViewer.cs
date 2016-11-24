using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Infrastructure;

namespace Durados.Web.Mvc.UI
{
    public class ViewViewer
    {
        const char comma = ',';
        const char seperator = '/';
        
        public ViewViewer()
        {

        }



        public string GetIndexUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.IndexAction + seperator + view.Name;
        }

        public string GetSetLanguageUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.SetLanguageAction + seperator + view.Name;
        }

        public string GetAutoCompleteUrl(View view)
        {
            return General.GetRootPath() + view.AutoCompleteController + seperator + view.AutoCompleteAction + seperator; // +view.Name;
        }

        public string GetChangePageSizeUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + "ChangePageSize" + seperator + view.Name;
        }

        public string GetCreateUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.CreateAction + seperator + view.Name;
        }

        public string GetRefreshUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.RefreshAction + seperator + view.Name;
        }

        public string GetCreateOnlyUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.CreateOnlyAction + seperator + view.Name;
        }

        public string GetEditUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.EditAction + seperator + view.Name;
        }

        public string GetDuplicateUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.DuplicateAction + seperator + view.Name;
        }
        
        public string GetEditRichUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.EditRichAction + seperator + view.Name;
        }

        public string GetRichUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.GetRichAction + seperator + view.Name;
        }

        public string GetEditOnlyUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.EditOnlyAction + seperator + view.Name;
        }

        public string GetJsonViewUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.GetJsonViewAction + seperator + view.Name;
        }

        public string GetSelectListUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.GetSelectListAction;
        }

        public string GetInlineAddingDialogUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.InlineAddingDialogAction + seperator;
        }

        public string GetInlineEditingDialogUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.InlineEditingDialogAction + seperator;
        }

        public string GetInlineAddingCreateUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.InlineAddingCreateAction + seperator;
        }

        public string GetInlineEditingEditUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.InlineEditingEditAction + seperator;
        }

        public string GetInlineDuplicateUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.InlineDuplicateAction + seperator;
        }

        public string GetInlineDuplicateDialogUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.InlineDuplicateDialogAction + seperator;
        }

        public string GetInlineSearchDialogUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.InlineSearchDialogAction + seperator;
        }

        public string GetJsonViewInlineAddingUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.GetJsonViewAction + seperator;
        }

        public string GetJsonViewInlineEditingUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.GetJsonViewAction + seperator;
        }

        

        public string GetDeleteUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.DeleteAction + seperator + view.Name;
        }

        public string GetDeleteSelectionUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.DeleteSelectionAction + seperator + view.Name;
        }

        public string GetEditSelectionUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.EditSelectionAction + seperator + view.Name;
        }

        public string GetFilterUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.FilterAction + seperator + view.Name;
        }

        public string GetUploadUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.UploadAction + seperator + view.Name;
        }

        public string GetExportToCsvUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.ExportToCsvAction + seperator + view.Name;
        }

        public string GetPrintUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.PrintAction + seperator + view.Name;
        }

        public string GetAllFilterValuesUrl(View view)
        {
            return General.GetRootPath() + view.Controller + seperator + view.AllFilterValuesAction + seperator + view.Name;
        }

        public Json.Filter GetJsonFilter(View view, string guid)
        {
            Json.Filter jsonFilter = new Json.Filter();

            //foreach (Field field in view.VisibleFieldsForFilter)
            foreach (Field field in view.Fields.Values)
            {
                string type = (field.FieldType == FieldType.Children) ? "Children" : field.GetHtmlControlType().ToString();
                jsonFilter.Fields.Add(field.Name, new Json.Field() { Name = field.Name, Type = type, Searchable = field.IsSearchable(), Permanent=false });
            }

            Durados.Web.Mvc.UI.Json.Filter pageFilter = view.GetPageFilter(guid);
            string parentFieldName = pageFilter.GetParentFiterFieldName();

            if (!string.IsNullOrEmpty(parentFieldName))
            {
                if (jsonFilter.Fields.ContainsKey(parentFieldName))
                {

                    Field field = view.Fields[parentFieldName];
                    string fieldVal=string.Empty;
                    foreach (string s in field.DatabaseNames.Split(','))
                    {
                         fieldVal+=string.Format("{0},", ViewHelper.GetSessionState(guid + "PageFilterState")[s]);
                    }

                    jsonFilter.Fields[parentFieldName].Value=fieldVal.TrimEnd(',');
                    if (field.FieldType == FieldType.Parent)
                    {
                        ParentField parentField = (ParentField)field;
                        //if (parentField.ParentHtmlControlType == ParentHtmlControlType.Autocomplete)
                        //{
                        string pk = jsonFilter.Fields[parentFieldName].Value.ToString();
                        jsonFilter.Fields[parentFieldName].Default = parentField.GetDisplayValue(pk);
                        //}
                    }
                    else if (field.FieldType == FieldType.Column)
                    {

                    }


                    jsonFilter.Fields[parentFieldName].Permanent = true;
                }
                else
                {
                    if (view.IsDerived)
                    {
                        string derivedFieldName = view.GetDerivedParentField(parentFieldName).Name;
                        if (jsonFilter.Fields.ContainsKey(derivedFieldName))
                        {
                            jsonFilter.Fields[derivedFieldName].Value = ViewHelper.GetSessionState(guid + "PageFilterState")[0];
                            jsonFilter.Fields[derivedFieldName].Permanent = true;
                        }
                    }
                }
            }



            return jsonFilter;
        }

        public Json.View GetJsonView(View view, DataAction dataAction, string guid)
        {
            Durados.Web.Mvc.UI.Json.View jsonView = new Json.View();

            foreach (Field field in view.GetVisibleFieldsForRow(dataAction))
            {
                string type = field.GetHtmlControlType().ToString();
                
                string format = string.Empty;
                if (field.FieldType == FieldType.Column)
                {
                    ColumnFieldType columnFieldType = field.GetColumnFieldType();
                    format = field.Validation.Format;
                }

                string validationType = "Children";
                if (field.FieldType != FieldType.Children)
                {
                    validationType = field.Validation.GetValidationType();
                }

                bool disbled;
                if (dataAction == DataAction.Create)
                {
                    disbled = field.IsDisableForCreate() || field.DisableConfigClonedView(view);
                }
                else
                {
                    disbled = field.IsDisableForEdit(guid) || field.DisableConfigClonedView(view);
                }
                Json.Field jsonField = new Json.Field() { Name = field.Name, Type = type, ValidationType = validationType, Format = format, Required = field.Required, Default = field.ConvertDefaultToString(), Searchable = field.IsSearchable(), Disabled = disbled, DisDup = field.DisableInDuplicate, Refresh = field.Refresh, Min = field.Validation.GetMinForJson(), Max = field.Validation.GetMaxForJson(), DependencyData = field.DependencyData };
                if (type == HtmlControlType.Autocomplete.ToString() && field.FieldType == FieldType.Parent)
                {
                    jsonField.Value = ((ParentField)field).ConvertDefaultToDisplayValue();
                }
                jsonView.Fields.Add(field.Name, jsonField);
            }

            try
            {
                LoadDerivation(view, jsonView);
            }
            catch { }

            jsonView.InlineAddingCreateUrl = view.GetInlineAddingCreateUrl();
            jsonView.InlineEditingCreateUrl = view.GetInlineEditingEditUrl();
            jsonView.ViewName = view.Name;

            return jsonView;
        }

        //private void LoadDerivation(View view, Json.View jsonView)
        //{
        //    if (view.Derivation != null)
        //    {
        //        jsonView.Derivation = new Durados.Web.Mvc.UI.Json.Derivation();
        //        jsonView.Derivation.DerivationField = view.Derivation.DerivationField;
        //        foreach (string derived in view.Derivation.Deriveds.Split('|'))
        //        {
        //            Durados.Web.Mvc.UI.Json.Derived jsonDerived = new Durados.Web.Mvc.UI.Json.Derived();
        //            string[] s = derived.Split(';');
        //            jsonDerived.Value = s[0].Trim('\n');
        //            foreach (string field in s[1].Split(','))
        //            {
        //                string fieldName = field.Trim('\n');
        //                if (view.Fields.ContainsKey(fieldName))
        //                {
        //                    jsonDerived.Fields.Add(fieldName, view.Fields[fieldName].Required);
        //                }
        //            }
        //            jsonView.Derivation.Deriveds.Add(jsonDerived);
        //        }
        //    }
            
        //}

        private void LoadDerivation(View view, Json.View jsonView)
        {
            if (view.Derivation != null)
            {
                jsonView.Derivation = new Durados.Web.Mvc.UI.Json.Derivation();
                jsonView.Derivation.DerivationField = view.Derivation.DerivationField;
                Dictionary<string, Dictionary<string, Field>> deriveds = view.Derivation.GetDeriveds(view);

                foreach (string value in deriveds.Keys)
                {
                    Durados.Web.Mvc.UI.Json.Derived jsonDerived = new Durados.Web.Mvc.UI.Json.Derived();
                    jsonDerived.Value = value;
                    foreach (string fieldName in deriveds[value].Keys)
                    {
                        jsonDerived.Fields.Add(fieldName, deriveds[value][fieldName].Required);
                    }
                    jsonView.Derivation.Deriveds.Add(jsonDerived);
                }

                //foreach (string derived in view.Derivation.Deriveds.Split('|'))
                //{
                //    Durados.Web.Mvc.UI.Json.Derived jsonDerived = new Durados.Web.Mvc.UI.Json.Derived();
                //    string[] s = derived.Split(';');
                //    jsonDerived.Value = s[0].Trim('\n');
                //    foreach (string field in s[1].Split(','))
                //    {
                //        string fieldName = field.Trim('\n');
                //        if (view.Fields.ContainsKey(fieldName))
                //        {
                //            jsonDerived.Fields.Add(fieldName, view.Fields[fieldName].Required);
                //        }
                //    }
                //    jsonView.Derivation.Deriveds.Add(jsonDerived);
                //}
            }

        }


        public Json.View GetJsonDisplayValue(View view, DataRow dataRow)
        {
            Durados.Web.Mvc.UI.Json.View jsonDisplayValue = new Json.View();

            string pk = view.GetPkValue(dataRow);
            string displayValue = view.GetDisplayValue(dataRow);
            jsonDisplayValue.Fields.Add("DisplayValue", new Json.Field() { Value = pk, Default = displayValue });

            return jsonDisplayValue;
        }

        public Json.View GetJsonView(View view, DataAction dataAction, string pk, DataRow dataRow, string guid)
        {
            return GetJsonView(view, dataAction, pk, dataRow, guid, null, null);
        }

        public Json.View GetJsonView(View view, DataAction dataAction, string pk, DataRow dataRow, string guid, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            //DataRow dataRow = view.GetDataRow(pk, null, beforeSelectCallback, afterSelectCallback);

            if (dataRow == null)
                return null;

            Json.View jsonView = new Json.View();

            foreach (Field field in view.GetVisibleFieldsForRow(dataAction))
            {
                
                if (field.FieldType == FieldType.Column)
                {
                    string type = field.GetHtmlControlType().ToString();
                    
                    string format = string.Empty;
                    if (field.FieldType == FieldType.Column)
                    {
                        ColumnFieldType columnFieldType = field.GetColumnFieldType();
                        
                        format = field.Validation.Format;
                    }

                    string validationType = "Children";
                    if (field.FieldType != FieldType.Children)
                    {
                        validationType = field.GetValidationType();
                    } 
                    ColumnField columnField = (ColumnField)field;
                    //string value= columnField.ConvertToString(dataRow);
                    //if (columnField.DataColumn.DataType == typeof(bool) && string.IsNullOrEmpty(value))
                    //    value = false.ToString();
                    object value = dataRow[columnField.DataColumn.ColumnName];
                    

                    if (view is Durados.Config.IConfigView && field.Name == "Formula")
                    {
                        string parentViewname = dataRow.GetParentRow("Fields")["Name"].ToString();

                        Map map = Maps.Instance.GetMap();
            
                        View currentView = (View)map.Database.Views[parentViewname];

                        value = DataAccessHelper.ReplaceFieldDisplayNames(value.ToString(), false, currentView);
                    }

                    if (value is DBNull || value.Equals("null"))
                        value = string.Empty;
                    else if (columnField.DataColumn.DataType == typeof(DateTime) || columnField.DataColumn.DataType == typeof(byte[]))
                    {
                        value = columnField.ConvertToString(dataRow);
                    }

                    bool disbled;
                    if (dataAction == DataAction.Create)
                    {
                        disbled = field.IsDisableForCreate() || field.DisableConfigClonedField(dataRow);
                    }
                    else
                    {
                        disbled = field.IsDisableForEdit(guid) || field.DisableConfigClonedField(dataRow);
                    }

                    if (dataAction == DataAction.Create)
                    {
                        if (!field.IncludeInDuplicate)
                        {
                            if (field.DefaultValue != null)
                                value = field.DefaultValue.ToString();
                            else
                                value = string.Empty;
                        }
                        
                    }

                    if (columnField.Encrypted && columnField.SpecialColumn == SpecialColumn.Password)
                    {
                        value = columnField.View.Database.EncryptedPlaceHolder;
                    }

                    jsonView.Fields.Add(field.Name, new Json.Field() { Name = field.Name, Value = value, Type = field.GetHtmlControlType().ToString(), ValidationType = validationType, Format = format, Required = field.Required, Searchable = field.IsSearchable(), Disabled = disbled, DisDup = field.DisableInDuplicate, Refresh = field.Refresh, Min = field.Validation.GetMinForJson(), Max = field.Validation.GetMaxForJson(), DependencyData = field.DependencyData });
                }
                else if (field.FieldType == FieldType.Parent)
                {
                    //, DependencyChildren = parentField.GetDependencyChildrenNames()
                    ParentField parentField = (ParentField)field;

                    string fk = string.Empty;
                    DataRow parentRow = null;
                    try
                    {
                        parentRow = dataRow.GetParentRow(parentField.DataRelation.RelationName);
                        fk = parentField.ParentView.GetPkValue(parentRow);
                    }
                    catch (Exception)
                    {
                        DataColumn[] columns = parentField.GetDataColumns();
                        if (columns.Length == 1)
                        {
                            fk = dataRow[columns[0].ColumnName].ToString();
                        }
                        else
                        {
                            foreach (DataColumn column in columns)
                                fk += dataRow[column.ColumnName].ToString() + ',';
                            fk.TrimEnd(',');
                        }

                    }

                    DataRow dependencyRow = parentRow;

                    if (parentField.GetHtmlControlType() == HtmlControlType.OutsideDependency)
                    {
                        List<string> dependencyFks = new List<string>();

                        List<Durados.ParentField> dependencyDynasty = parentField.GetDependencyDynasty();
                        Durados.DataAccess.SqlAccess sqlAccess = new SqlAccess();
                                
                        foreach (ParentField dependencyField in dependencyDynasty)
                        {
                            
                            DataRow row = dependencyRow.GetParentRow(dependencyField.DataRelation.RelationName);
                            if (row == null)
                            {
                                string key = dependencyField.View.GetFkValue(dependencyRow, dependencyField.DataRelation.RelationName);
                                row = sqlAccess.GetDataRow(dependencyField.ParentView, key, dependencyRow.Table.DataSet);
                            }
                            dependencyRow = row;
                            string dependencyFk = dependencyField.ParentView.GetPkValue(dependencyRow);
                            dependencyFks.Add(dependencyFk);
                        }

                        dependencyFks.Reverse();
                        dependencyDynasty.Reverse();

                        int i = 0;
                        foreach (ParentField dependencyField in dependencyDynasty)
                        {
                            Json.Field jsonDependencyField = GetParentJsonField(dependencyField, dependencyFks[i], dataAction, guid);

                            jsonView.Fields.Add(jsonDependencyField.Name, jsonDependencyField);

                            i++;
                        }
                    }

                    Json.Field jsonField = GetParentJsonField(parentField, fk, dataAction, guid);

                    if (parentField.DisableConfigClonedField(dataRow))
                    {
                        jsonField.Disabled = true;
                    }

                    jsonView.Fields.Add(field.Name, jsonField);
                }

                else if (field.FieldType == FieldType.Children)
                {
                    if (((ChildrenField)field).ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
                    {
                        string value = string.Empty;
                        if (dataAction == DataAction.Create)
                        {
                            if (field.IncludeInDuplicate)
                            {
                                value = ((ChildrenField)field).GetSelectedChildrenPKDelimited(pk);
                            }
                            else
                            {
                                if (field.DefaultValue != null)
                                    value = field.DefaultValue.ToString();
                            }
                        }
                        else
                        {
                            value = ((ChildrenField)field).GetSelectedChildrenPKDelimited(pk);
                        }

                        Json.Field jsonField = GetChildrenJsonField(((ChildrenField)field), value);

                        jsonView.Fields.Add(field.Name, jsonField);
                    }
                    else
                    {
                        Json.Field jsonField = GetChildrenJsonField(((ChildrenField)field), null);
                        jsonView.Fields.Add(field.Name, jsonField);
                    }
                }
            }

            try
            {
                LoadDerivation(view, jsonView);
            }
            catch { }

            jsonView.InlineAddingCreateUrl = view.GetInlineAddingCreateUrl();
            jsonView.ViewName = view.Name;

            return jsonView;
        }

        private Durados.Web.Mvc.UI.Json.Field GetChildrenJsonField(ChildrenField childrenField, string value)
        {
            Json.Field jsonField = new Json.Field() { Name = childrenField.Name, Value = value, Type = childrenField.GetHtmlControlType().ToString(), DependencyData = childrenField.DependencyData };
            return jsonField;
        }

        private Durados.Web.Mvc.UI.Json.Field GetParentJsonField(ParentField parentField, string fk, DataAction dataAction, string guid)
        {
            bool disabled = parentField.IsDisable(dataAction, guid);
            string value = string.Empty;
            if (dataAction == DataAction.Create)
            {
                if (parentField.IncludeInDuplicate)
                {
                    value = fk;
                }
                else
                {
                    if (parentField.DefaultValue != null)
                        value = parentField.ConvertDefaultToString();
                }
            }
            else
            {
                value = fk;
            }
            Json.Field jsonField = new Json.Field() { Name = parentField.Name, Value = value, Type = parentField.GetHtmlControlType().ToString(), ValidationType = parentField.GetValidationType(), Format = parentField.Validation.Format, Required = parentField.Required, DependencyChildren = parentField.GetDependencyTriggeredFieldNames(), Searchable = parentField.IsSearchable(), Disabled = parentField.IsDisableForCreate(), DisDup = parentField.DisableInDuplicate, Refresh = parentField.Refresh, DependencyData = parentField.DependencyData };
            if (parentField.ParentHtmlControlType == ParentHtmlControlType.Autocomplete || disabled)
                jsonField.Default = parentField.GetLocalizedDisplayValue(value);

            return jsonField;
        }

        public string GetFieldValuesFromDialogIntoTable(View view, DataAction dataAction)
        {

            StringBuilder fieldValuesFromTableIntoDialog = new StringBuilder();

            foreach (Field field in view.GetVisibleFieldsForRow(dataAction))
            {
                if (field is ColumnField && ((ColumnField)field).GetHtmlControlType() == HtmlControlType.Check)
                {
                    fieldValuesFromTableIntoDialog.Append("$('#' + pk + ' [colname=" + field.Name + "]').attr('checked',$('#" + field.Name + "').attr('checked'));$('#' + pk + ' [colname=" + field.Name + "]').css('visibility','visible');");
                }
                else if (field is ColumnField && ((ColumnField)field).GetHtmlControlType() == HtmlControlType.TextArea)
                {
                    fieldValuesFromTableIntoDialog.Append("$('#' + pk + ' [colname=" + field.Name + "]')[0].firstChild.nodeValue = $('#" + field.Name + "').text();");
                    //fieldValuesFromTableIntoDialog += "$('#" + field.Name + "').text($('#' + pk + ' [colname=" + field.Name + "]')[0].firstChild.nodeValue);";
                }
                else if (field is ColumnField)
                {
                    fieldValuesFromTableIntoDialog.Append("$('#' + pk + ' [colname=" + field.Name + "]')[0].firstChild.nodeValue = $('#" + field.Name + "').val();");
                }
                else if (field is ParentField && ((ParentField)field).ParentHtmlControlType == ParentHtmlControlType.DropDown)
                {
                    fieldValuesFromTableIntoDialog.Append("$('#' + pk + ' [colname=" + field.Name + "]')[0].attributes['pk'].value = $('#" + field.Name + "').val();");
                }
                else if (field is ParentField)
                {
                    fieldValuesFromTableIntoDialog.Append("$('#' + pk + ' [colname=" + field.Name + "]')[0].firstChild.nodeValue = $('#" + field.Name + "').val();");
                }
            }

            return fieldValuesFromTableIntoDialog.ToString();
        }

        public string GetAutocompleteClientControlInitiation(View view)
        {
            //$(document).ready(function() {
            //    $("#" + fieldName).autocomplete('<%=Url.Action(view.AutocompleteAction, view.AutocompleteController, new { lookupType="School", substring=false }) %>',
            //    {
            //        dataType: 'json',
            //        parse: function(data) {
            //            var rows = new Array();
            //            for (var i = 0; i < data.length; i++) {
            //                rows[i] = { data: data[i].Tag, value: data[i].Tag.Name, result: data[i].Tag.Name };
            //            }
            //            return rows;
            //        },
            //        formatItem: function(row) {
            //            return row.Name;
            //        },
            //        width: 160,
            //        highlight: false,
            //        multiple: false,
            //        mustMatch : mustMatch
            //    });
            //});

            //             $("#city").autocomplete('/Utilities/Lookup?lookupType=City&substring=False',
            //10 {
            //11 dataType: 'json',
            //12 parse: function(data) {
            //13 var rows = new Array();
            //14 for (var i = 0; i < data.length; i++) {
            //15 rows[i] = { data: data[i].Tag, value: data[i].Tag.Name, result: data[i].Tag.Name };
            //16 }
            //17 return rows;
            //18 },
            //19 formatItem: function(row) {
            //20 return row.Name;
            //21 },
            //22 width: 160,
            //23 highlight: false,
            //24 multiple: false,
            //25 multipleSeparator: ";"
            //26 });
            //27 

            StringBuilder autocompleteClientControlInitiation = new StringBuilder();
            foreach (ParentField parentField in view.Fields.Values.Where(pf => pf.FieldType == FieldType.Parent && ((ParentField)pf).ParentHtmlControlType == ParentHtmlControlType.Autocomplete))
            {
                autocompleteClientControlInitiation.Append("$(document).ready(function() {");
                autocompleteClientControlInitiation.Append("$('#" + parentField.Name + "').autocomplete('" + GetAutoCompleteUrl(view) + "?field=" + parentField.Name + "',");
                autocompleteClientControlInitiation.Append("{");
                autocompleteClientControlInitiation.Append("dataType: 'json',");
                autocompleteClientControlInitiation.Append("parse: AutoCompleteParse,");
                autocompleteClientControlInitiation.Append("formatItem: function(row) {");
                autocompleteClientControlInitiation.Append("return row.Name;");
                autocompleteClientControlInitiation.Append("},");
                autocompleteClientControlInitiation.Append("width: 160,");
                autocompleteClientControlInitiation.Append("highlight: false,");
                autocompleteClientControlInitiation.Append("multiple: false,");
                autocompleteClientControlInitiation.Append("multipleSeparator: \";\"");
                autocompleteClientControlInitiation.Append("});");
                autocompleteClientControlInitiation.Append("});");
            }

            foreach (ParentField parentField in view.Fields.Values.Where(pf => pf.FieldType == FieldType.Parent && ((ParentField)pf).ParentHtmlControlType == ParentHtmlControlType.Autocomplete))
            {
                autocompleteClientControlInitiation.Append("$(document).ready(function() {\n");
                autocompleteClientControlInitiation.Append("$('#filter_" + parentField.Name + "').autocomplete('" + GetAutoCompleteUrl(view) + "?field=" + parentField.Name + "',\n");
                autocompleteClientControlInitiation.Append("{\n");
                autocompleteClientControlInitiation.Append("dataType: 'json',\n");
                autocompleteClientControlInitiation.Append("parse: AutoCompleteParse,\n");
                autocompleteClientControlInitiation.Append("formatItem: function(row) {\n");
                autocompleteClientControlInitiation.Append("return row.Name;\n");
                autocompleteClientControlInitiation.Append("},\n");
                autocompleteClientControlInitiation.Append("width: 160,\n");
                autocompleteClientControlInitiation.Append("highlight: false,\n");
                autocompleteClientControlInitiation.Append("multiple: false,\n");
                autocompleteClientControlInitiation.Append("multipleSeparator: \";\"\n");
                autocompleteClientControlInitiation.Append("}).result(AutoCompleteResult);\n");
                autocompleteClientControlInitiation.Append("});\n");
            }



            return autocompleteClientControlInitiation.ToString();
        }
        /*
        public string GetValidation(View view, DataAction dataAction, string guid)
        {
            StringBuilder validation = new StringBuilder();

            //  var theFirstName = null;
            //  $(document).ready(function() {   
            //      $(function() {
            //          theFirstName = new Spry.Widget.ValidationTextField("theFirstName");
            //      });
            //  });
            ColumnFieldViewer columnFieldViewer = new ColumnFieldViewer();

            foreach (Field field in view.GetVisibleFieldsForRow(dataAction))
            {
                string validationVar = columnFieldViewer.GetContainerElementIDForValidation(field, dataAction, guid);
                validation.Append("var " + validationVar + " = null;");

            }

            validation.Append("$(document).ready(function() {");
            validation.Append("$(function() {");

            //var theCTratio1 = new Spry.Widget.ValidationTextField("theCTratio1", "real", { isRequired: false, validateOn: ["blur"], useCharacterMasking: true, minValue: 1, maxValue: 10000 });
            //var theInstallationDate = new Spry.Widget.ValidationTextField("theInstallationDate", "date", {format: "mm-dd-yyyy", isRequired: true, validateOn: ["blur", "change"] });


            foreach (Field field in view.GetVisibleFieldsForRow(dataAction).Where(f => f.FieldType == FieldType.Parent || f.FieldType == FieldType.Column))
            {
                string validationType = "none";
                string required = (field.Required) ? "true" : "false";
                string useMasking = string.Empty;
                string range = string.Empty;
                string format = string.Empty;

                validationType = columnFieldViewer.GetValidationType(field);
                ColumnFieldType columnFieldType = field.GetColumnFieldType();
                if (field.Validation.Range != null)
                {
                    if (columnFieldType == ColumnFieldType.Integer || columnFieldType == ColumnFieldType.Real)
                    {
                        useMasking = ", useCharacterMasking: true";
                        range = ", useCharacterMasking: true, minValue: " + field.Validation.Range.Min.ToString() + ", maxValue: " + field.Validation.Range.Min.ToString();
                    }
                }
                if (string.IsNullOrEmpty(field.Validation.Format))
                {
                    if (columnFieldType == ColumnFieldType.DateTime)
                    {
                        format = ", format: '" + view.Database.DateFormat + "'";
                    }
                    else
                    {
                        format = string.Empty;
                    }
                }
                else
                {
                    format = ", format: '" + field.Validation.Format + "'";
                }

                HtmlControlType htmlControlType = field.GetHtmlControlType();
                string validationVar = columnFieldViewer.GetContainerElementIDForValidation(field, dataAction, guid);
                if (htmlControlType == HtmlControlType.Text || htmlControlType == HtmlControlType.Autocomplete)
                {
                    validation.Append(validationVar + " = new Spry.Widget.ValidationTextField('" + validationVar + "', '" + validationType + "', { isRequired: " + required + useMasking + format + "}" + ");");
                }
                else if (htmlControlType == HtmlControlType.DropDown || htmlControlType == HtmlControlType.OutsideDependency || htmlControlType == HtmlControlType.Groups)
                {
                    validation.Append(validationVar + " = new Spry.Widget.ValidationSelect('" + validationVar + "', { isRequired: " + required + "}" + ");");
                }
                else if (htmlControlType == HtmlControlType.TextArea)
                {
                    validation.Append(validationVar + " = new Spry.Widget.ValidationTextarea('" + validationVar + "', { isRequired: " + required + "}" + ");");
                }

            }


            validation.Append("});");
            validation.Append("});");

            return validation.ToString();
        }
        */
        

        public void Delete(View view, string fk, string fkField, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback)
        {
            view.Delete(fk, fkField, string.Empty, beforeDeleteCallback, afterDeleteBeforeCommitCallback, afterDeleteAfterCommitCallback);

        }

        public DataRow CreateRow(View view, string insertAbovePK, Json.View jsonView, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (Json.Field jsonField in jsonView.Fields.Values)
            {
                object value;
                if (view.Fields.ContainsKey(jsonField.Name) && view.Fields[jsonField.Name].FieldType == FieldType.Column && ((ColumnField)view.Fields[jsonField.Name]).DataColumn.DataType.Equals(typeof(byte[])))
                {
                    value = ((ColumnField)view.Fields[jsonField.Name]).ConvertFromString(jsonField.Value.ToString());
                }
                else
                {
                    value = LocalizeValue(view, jsonField.Name, jsonField.Value);
                }
                values.Add(jsonField.Name, value);
            }
            DataRow dataRow = view.Create(values, insertAbovePK, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
            return dataRow;
        }

        public void EditRow(View view, Json.View jsonView, string pk, bool ignoreNull, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (Json.Field jsonField in jsonView.Fields.Values)
            {
                if (!(ignoreNull && !jsonField.Refresh && (jsonField.Value == null || jsonField.Value.ToString() == string.Empty || jsonField.Value.Equals(false))))
                {
                    object value;
                    if (view.Fields.ContainsKey(jsonField.Name))
                    {
                        if (view.Fields[jsonField.Name].FieldType == FieldType.Column && ((ColumnField)view.Fields[jsonField.Name]).DataColumn.DataType.Equals(typeof(byte[])))
                        {
                            value = ((ColumnField)view.Fields[jsonField.Name]).ConvertFromString(jsonField.Value.ToString());
                        }
                        else
                        {
                            value = LocalizeValue(view, jsonField.Name, jsonField.Value);
                        }
                        values.Add(jsonField.Name, value);
                    }
                    else
                    {
                        values.Add(jsonField.Name, jsonField.Value);
                    }
                }

            }

            CrudUtility crudUtility = new CrudUtility(view.Database.Map.AppName);
            crudUtility.Edit(view.JsonName, values, pk);
            //view.Edit(values, pk, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);
            
        }

        

        public void CopyPasteView(View view, Json.CopyPaste jsonView, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {

            string pk = "";

            int RowsNumber = jsonView.GetRowsNumber();

            ICopyPaste cpCommand = view.GetCopyPaste();

            try
            {
                int multi = 1;
                int newRownumber = 0;

                int valuesCount = jsonView.Source.FieldsValues.Count;

                if (RowsNumber > valuesCount && RowsNumber % valuesCount == 0)
                {
                    multi = (int)(RowsNumber / valuesCount);
                    RowsNumber = valuesCount;

                }
                else if (RowsNumber != valuesCount)
                {
                    throw new DuradosException("Rows number do not match Source data selected");
                }

                for (int m = 1; m <= multi; m++)
                {
                    for (int r = 0; r < RowsNumber; r++)
                    {
                        int names = jsonView.Source.FieldsNames.Count;

                        if (names != jsonView.Source.FieldsValues[r].Count)
                        {
                            throw new DuradosException("Source values do not match Source Fields Names");
                        }

                        Dictionary<string, object> values = new Dictionary<string, object>();

                        object value;

                        for (int i = 0; i < names; i++)
                        {
                            value = LocalizeValue(view, jsonView.Source.FieldsNames[i], jsonView.Source.FieldsValues[r][i]);
                            values.Add(jsonView.Source.FieldsNames[i], value);
                        }

                        pk = jsonView.Destination.RowsPKs[newRownumber + r];

                        cpCommand.Edit(view, values, pk, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);

                    }

                    newRownumber += RowsNumber;

                }

                cpCommand.PasteCommit(false);

            }
            catch (Exception exeption)
            {

                cpCommand.PasteCommit(true);
                throw new DuradosException(exeption.Message); //"Error while processing data, Paste operation cancelled!");

            }
            finally
            {
                try
                {
                    cpCommand.CloseConnections();
                }
                catch { }
            }


        }

        public void EditField(View view, string fieldName, object fieldValue, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(fieldName, LocalizeValue(view, fieldName, fieldValue));

            view.Edit(values, pk, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);

        }

        

        private string LocalizeValue(View view, string fieldName, object value)
        {
            if (!view.Fields.ContainsKey(fieldName))
                return value.ToString();

            Field field = view.Fields[fieldName];

            if (value == null)
                return string.Empty;
            if (field.FieldType == FieldType.Column)
            {
                return field.ConvertFromString(value.ToString()).ToString();
            }
            else
            {
                return value.ToString();
            }
        }

        public DataView FillPage(View view, int page, int pageSize, Json.Filter jsonFilter, bool search, string sortColumn, SortDirection direction, out int rowCount, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            if (jsonFilter != null)
            {
                foreach (Json.Field jsonField in jsonFilter.Fields.Values)
                {
                    if (jsonField.Value != null && !string.IsNullOrEmpty(jsonField.Value.ToString()))
                    {
                        values.Add(jsonField.Name, jsonField.Value);
                    }
                    //else
                    //{
                    //    values.Add(jsonField.Name, null);
                    //}
                }
            }
            Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                sortColumns.Add(sortColumn, direction);
            }

            return view.FillPage(page, pageSize, values, search, sortColumns, out rowCount, beforeSelectCallback, afterSelectCallback);

        }
    }
}
