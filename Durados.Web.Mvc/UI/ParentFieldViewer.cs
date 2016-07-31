using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Infrastructure;

namespace Durados.Web.Mvc.UI
{
    public class ParentFieldViewer : FieldViewer
    {
        protected virtual string GetNavigationUrl(ParentField field, DataRow dataRow)
        {
            string url = GetUtlWithoutQuery(field);

            string query = "?";

            for (int i = 0; i < field.DataRelation.ChildColumns.Count(); i++)
            {
                DataColumn childColumn = field.DataRelation.ChildColumns[i];
                DataColumn parentColumn = field.DataRelation.ParentColumns[i];
                query += parentColumn.ColumnName + "=" + System.Web.HttpContext.Current.Server.UrlEncode(dataRow[childColumn.ColumnName].ToString()) + "&";
            }


            query += "__" + field.Name + "__=" + System.Web.HttpContext.Current.Server.UrlEncode(field.ConvertToString(dataRow));

            return url + query;
        }

        protected virtual string GetUtlWithoutQuery(ParentField field)
        {
            string url = System.Web.HttpContext.Current.Request.ApplicationPath;

            if (!url.EndsWith("/"))
                url += "/";
            View parentView = ((View)field.ParentView);
            url += parentView.Controller + "/";
            url += parentView.IndexAction + "/";
            url += parentView.Name;

            return url;
        }

        public override string GetElementForTableView(Field field, DataRow dataRow, string guid)
        {
            return GetElementForTableView((ParentField)field, dataRow, guid);
        }

        public virtual string GetElementForTableView(ParentField field, DataRow dataRow, string guid)
        {
            if (field.GetHtmlControlType() == HtmlControlType.OutsideDependency && field.ShowDependencyInTable)
            {
                GetGetDependencyDynastyValuesString(GetDependencyDynastyValues(field, dataRow));
                return GetGetDependencyDynastyValuesString(GetDependencyDynastyValues(field, dataRow));
            }
            else if ((!((View)field.ParentView).IsAllow()) || field.NoHyperlink)
                return field.ConvertToString(dataRow);
            else
            {
                string id = guid + inlineEditingPrefix + field.Name;
                string type = "tableView";

                View view = (View)field.ParentView;
                string pkValue = view.GetPkValue(dataRow.GetParentRow(field.DataRelation.RelationName));
                string editClick = string.Empty;

                if (field.EditInTableView)
                {
                    if (view.Popup)
                    {
                        return "<a href='JavaScript:void(0);' colName='" + field.Name + "' pk='" + pkValue + "' onclick=\"InlineEditingDialog.CreateAndOpen('" + view.Name + "','" + view.DisplayName + "','" + type + "',this ,'" + view.GetInlineEditingEditUrl() + "', '" + guid + "', event)\">" + field.ConvertToString(dataRow) + "</a>";
                    }
                    else
                    {
                        string url = GetUrlWithoutQuery(view, "Item");
                        url += "?viewName=" + view.Name + "&pk=____&guid=" + view.Name + "_Item_";

                        editClick += "d_edit(null, " + view.Popup.ToString().ToLower() + ", " + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + view.Name + "', '" + guid + "', this, event)";

                        return "<a href='JavaScript:void(0);' colName='" + field.Name + "' pk='" + pkValue + "' onclick=\"" + editClick + "\">" + field.ConvertToString(dataRow) + "</a>";
                    }
                }
                else
                {
                    if ((!((View)field.ParentView).IsAllow()) || field.NoHyperlink)
                        return field.ConvertToString(dataRow);
                    else
                        return "<a href='JavaScript:void(0);' onclick='ctrlNavigate(this, event);' d_role='parentTableView' d_url='" + GetNavigationUrl(field, dataRow) + "', colName='" + field.Name + "' pk='" + field.ParentView.GetPkValue(dataRow.GetParentRow(field.DataRelation.RelationName)) + "' >" + field.ConvertToString(dataRow) + "</a>";

                }
            }
        }

        protected virtual string GetGetDependencyDynastyValuesString(string[] values)
        {
            string s = string.Empty;

            foreach (string str in values)
            {
                s += str + " ";
            }

            return s;
        }

        public virtual string[] GetDependencyDynastyValues(ParentField parentField, DataRow dataRow)
        {
            List<string> values = new List<string>();

            DataRow parentRow = dataRow.GetParentRow(parentField.DataRelation.RelationName);
            DataRow dependencyRow = parentRow;

            List<Durados.ParentField> dependencyDynasty = parentField.GetDependencyDynasty();
            Durados.DataAccess.SqlAccess sqlAccess = new SqlAccess();

            string text = parentField.ConvertToString(dataRow);
            if ((!((View)parentField.ParentView).IsAllow()) || parentField.NoHyperlink)
                values.Add(text);
            else
                values.Add("<a href='" + GetNavigationUrl(parentField, dataRow) + "' colName='" + parentField.Name + "' pk='" + parentField.ParentView.GetPkValue(dataRow.GetParentRow(parentField.DataRelation.RelationName)) + "' >" + text + "</a>");

            foreach (ParentField dependencyField in dependencyDynasty)
            {
                text = dependencyField.ConvertToString(dependencyRow);
                if ((!((View)dependencyField.ParentView).IsAllow()) || dependencyField.NoHyperlink)
                    values.Add(text);
                else
                    values.Add("<a href='" + GetNavigationUrl(dependencyField, dependencyRow) + "' colName='" + dependencyField.Name + "' pk='" + dependencyField.ParentView.GetPkValue(dependencyRow.GetParentRow(dependencyField.DataRelation.RelationName)) + "' >" + text + "</a>");


                DataRow row = dependencyRow.GetParentRow(dependencyField.DataRelation.RelationName);
                if (row == null)
                {
                    string key = dependencyField.View.GetFkValue(dependencyRow, dependencyField.DataRelation.RelationName);
                    row = sqlAccess.GetDataRow(dependencyField.ParentView, key, dependencyRow.Table.DataSet);
                }
                dependencyRow = row;



            }

            values.Reverse();

            return values.ToArray();
        }

        public override string GetElementForEdit(Field field, string guid)
        {
            return GetElementForEdit(field, string.Empty, string.Empty, guid);
        }

        public override string GetElementForEdit(Field field, DataRow dataRow, string guid)
        {
            string pk = field.View.GetPkValue(dataRow);
            string value = field.GetValue(dataRow);

            return GetElementForEdit((ParentField)field, pk, value, guid);
        }

        public override string GetElementForEdit(Field field, string pk, string value, string guid)
        {
            return GetElementForEdit((ParentField)field, pk, value, guid);
        }

        public virtual string GetElementForEdit(ParentField field, string pk, string value, string guid)
        {
            if (field.ParentHtmlControlType == ParentHtmlControlType.Autocomplete)
                return GetAutoCompleteForEdit(field, pk, value, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Radio)
                return GetRadioForEdit(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.OutsideDependency)
                if (field.DependencyField == null)
                    return GetDropDownForEdit(field, pk, value, guid);
                else
                    return GetOutsideDependencyForEdit(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.InsideDependency || field.ParentHtmlControlType == ParentHtmlControlType.InsideDependencyUniqueNames)
                if (field.DependencyField == null || field.InsideTriggerField == null)
                    return GetDropDownForEdit(field, pk, value, guid);
                else
                    return GetInsideDependencyForEdit(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Groups)
                return GetGroupsForEdit(field, pk, value, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Hidden)
                return GetHiddenForEdit(field, pk, value, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Tile)
                return GetTileImageSelectorForEdit(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Slider)
                return GetSliderImageSelectorForEdit(field, pk, guid);
            else
                return GetDropDownForEdit(field, pk, value, guid);
        }

        public override string GetElementForInlineAdding(Field field, string guid)
        {
            return GetElementForCreate((ParentField)field, guid).Replace(createPrefix, inlineAddingPrefix);
        }

        public override string GetElementForInlineEditing(Field field, string guid)
        {
            return GetElementForEdit((ParentField)field, guid).Replace(editPrefix, inlineEditingPrefix);
        }

        public override string GetElementForCreate(Field field, string guid)
        {
            return GetElementForCreate(field, string.Empty, string.Empty, guid);
        }

        public override string GetElementForCreate(Field field, string pk, string value, string guid)
        {
            return GetElementForCreate((ParentField)field, pk, value, guid);
        }

        public virtual string GetElementForCreate(ParentField field, string pk, string value, string guid)
        {
            if (field.ParentHtmlControlType == ParentHtmlControlType.Autocomplete)
                return GetAutoCompleteForCreate(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Radio)
                return GetRadioForCreate(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.OutsideDependency)
                if (field.DependencyField == null)
                    return GetDropDownForCreate(field, pk, guid);
                else
                    return GetOutsideDependencyForCreate(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.InsideDependency || field.ParentHtmlControlType == ParentHtmlControlType.InsideDependencyUniqueNames)
                if (field.DependencyField == null || field.InsideTriggerField == null)
                    return GetDropDownForCreate(field, pk, guid);
                else
                    return GetInsideDependencyForCreate(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Groups)
                return GetGroupsForCreate(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Hidden)
                return GetHiddenForCreate(field, pk, guid);

            else if (field.ParentHtmlControlType == ParentHtmlControlType.Tile)
                return GetTileImageSelectorForCreate(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Slider)
                return GetSliderImageSelectorForCreate(field, pk, guid);
            else
                return GetDropDownForCreate(field, pk, guid);
        }

        private string GetTileImageSelectorForCreate(ParentField field, string pk, string guid)
        {
            return GetImageTile(field, createPrefix, GetStyle(field), field.ConvertDefaultToString(), field.IsDisableForCreate(), pk, guid, "class='" + field.CssClass + "'", "", true,DataAction.Create);
        }
        private string GetSliderImageSelectorForCreate(ParentField field, string pk, string guid)
        {
            return GetImageSlider(field, createPrefix, GetStyle(field), field.ConvertDefaultToString(), field.IsDisableForCreate(), pk, guid, "class='" + field.CssClass + "'", "", true,DataAction.Create);
        }
        
        private string GetTileImageSelectorForEdit(ParentField field, string pk, string guid)
        {
            return GetImageTile(field, editPrefix, GetStyle(field), field.ConvertDefaultToString(), field.IsDisableForEdit(guid), pk, guid, "class='" + field.CssClass + "'", "", true,DataAction.Edit);
        }
        private string GetSliderImageSelectorForEdit(ParentField field, string pk, string guid)
        {
            return GetImageSlider(field, editPrefix, GetStyle(field), field.ConvertDefaultToString(), field.IsDisableForEdit(guid), pk, guid, "class='" + field.CssClass + "'", "", true,DataAction.Edit);
        }
        

        private string GetImageTile(ParentField field, string prefix, string style, string selectedValue, bool disabled, string pk, string guid, string html, string firstOptionText, bool load,DataAction action)
        {
            System.Text.StringBuilder tile = new StringBuilder();

            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            string insideTriggerAttributes = string.Empty;
            bool hasInsideDefault = false;

           
            tile.Append("<div " + style + " " + GetDisabledHtmlAttribute(disabled) + " id='" + id + "' name='" + field.Name + "' viewName='" + field.View.Name + "' pk='" + pk + "' isGroupFilter='false' " + insideTriggerAttributes + GetInsideDependencyAttributes(field, prefix, guid) + html + " >");

           // tile.Append("<div value=''>" + firstOptionText + "</div>");


            if ((!disabled && load) || hasInsideDefault )
            {

                bool useUniqueName = field.ParentHtmlControlType == ParentHtmlControlType.InsideDependencyUniqueNames;

                //by br 2
                //Dictionary<string, string> selectOptions = field.GetSelectOptions(isFilter, useUniqueName, null, null);
                Dictionary<string, string> selectOptions = field.GetSelectOptions(field.View as Durados.Web.Mvc.View, string.Empty, null, false);
                if (field.View.Database.DiagnosticsReportInProgress)
                {
                    if (selectOptions.Count > field.View.Database.DiagnosticsReport.OverLoadLimit)
                    {
                        Map.Logger.Log(field.View.Name, field.Name, field.View.Database.DiagnosticsReport.Name, string.Empty, field.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptions.Count, string.Empty, field.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                string[] selectedValues = field.SplitValues(selectedValue);
                // selected='selected'
                //if (isFilter)
                //{
                //    dropDown += "<option value='" + Database.EmptyString + "'>" + field.View.Database.EmptyDisplay + "</option>";
                //}

               
                foreach (string key in selectOptions.Keys)
                {
                    string value = selectOptions[key];
                    //if (field.DoLocalize())
                    //{
                    //    value = Map.Database.Localizer.Translate(value);
                    //}
                    if (selectedValues.Contains(key) )
                    {

                        //string disabledAttr = GetDisabledHtmlAttribute();
                        //tile.Append("<div value='" + key + "' class='ImageSelectorTileOption ImageSelectorSelected'><img src='" + value + "' class='ImageSelectorImage'></div>");
                        tile.Append("<div value='" + key + "' class='ImageSelectorTileOption ImageSelectorSelected'>" + value + "</div>");
                        //dropDown += "<option value='" + key + "' " + disabledAttr + ">" + value + "</option>";
                       
                    }
                    else
                    {
                        tile.Append("<div value='" + key + "' class='ImageSelectorTileOption '>" + value + "></div>");
                    }
                }

            }
            if (disabled && !string.IsNullOrEmpty(selectedValue))
            {
                //by br 3
                //Dictionary<string, string> selectOptions = field.GetSelectOptions();
                Dictionary<string, string> selectOptions = field.GetSelectOptions(field.View as Durados.Web.Mvc.View, string.Empty, null, false);

                if (field.View.Database.DiagnosticsReportInProgress)
                {
                    if (selectOptions.Count > field.View.Database.DiagnosticsReport.OverLoadLimit)
                    {
                        Map.Logger.Log(field.View.Name, field.Name, field.View.Database.DiagnosticsReport.Name, string.Empty, field.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptions.Count, string.Empty, field.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                string[] selectedValues = field.SplitValues(selectedValue);

                foreach (string key in selectOptions.Keys)
                {
                    if (selectedValues.Contains(key))
                    {
                        string value = selectOptions[key];
                        if (field.DoLocalize())
                        {
                            value = Map.Database.Localizer.Translate(value);
                        }
                        //tile.Append("<option value='" + key + "' " + ((selectedValue == key) ? "selected='selected'>" : ">").ToString() + value + "</option>");
                        tile.Append("<div value='" + key + "' class='ImageSelectorTileOption " + ((selectedValue == key) ? "ImageSelectorSelected" : string.Empty) + " '>" + value + "</div>");
                    }
                }
            }

            tile.Append("</div>");

          
            if (!load && !hasInsideDefault )
            {
                string onclicked = "";
                string onhovered = "";
                if ((!disabled && string.IsNullOrEmpty(field.InsideTriggerFieldName)))
                {
                    onclicked = " onclick='LoadCheckListFilter($(\"#" + id + "\"), \"" + guid + "\")'";
                    onhovered = " onmouseover='$(this).addClass(\"ui-state-hover\")' onmouseout='$(this).removeClass(\"ui-state-hover\")'";
                }
                string icon = "";

                if (Map.Database.Localizer.Direction != "RTL")
                {
                    icon = "<div style='float: left;' class='ui-icon ui-icon-triangle-1-e'></div>";
                }
                else
                {
                    icon = "<div style='float: right;' class='ui-icon ui-icon-triangle-1-w'></div>";
                }

                if ((!disabled && string.IsNullOrEmpty(field.InsideTriggerFieldName)) )
                {
                    //Changed by br
                    string widthStyle = string.Empty;
                    string displayStyle = "display: block;";

                    tile.Append("<span d_ph='ph' class='ui-dropdownchecklist ui-dropdownchecklist-selector-wrapper ui-widget' style='cursor: default; overflow: hidden; " + displayStyle + widthStyle + "'><span tabindex='0'" + onclicked + " class='ui-dropdownchecklist-selector ui-state-default' style='overflow: hidden; white-space: nowrap; " + displayStyle + widthStyle + "'" + onhovered + ">" + icon + "<span style='display: inline-block; white-space: nowrap; overflow: hidden' class='ui-dropdownchecklist-text'></span></span></span>");
                }
            }

            //if (!isFilter)
            //{
            //    DataAction dataAction;
            //    if (prefix == createPrefix)
            //        dataAction = DataAction.Create;
            //    else
            //        dataAction = DataAction.Edit;

            //    if (field.InlineAdding && ((View)field.ParentView).IsCreatable() && !field.IsDisableForInline(dataAction, guid))
            //    {
            //        AddInlineAdding(field, prefix, tile, "DropDown", id, guid);
            //    }

            //    if (field.InlineEditing && ((View)field.ParentView).IsEditable(guid) && !field.IsDisableForInline(dataAction, guid))
            //    {
            //        AddInlineEditing(field, prefix, tile, "DropDown", id, guid);
            //    }

            //    if (field.InlineDuplicate && ((View)field.ParentView).IsCreatable() && !field.IsDisableForInline(dataAction, guid))
            //    {
            //        AddInlineDuplicate(field, prefix, tile, "DropDown", id, guid);
            //    }

            //    if (field.InlineSearch && ((View)field.ParentView).IsAllow() && !field.IsDisableForInline(dataAction, guid))
            //    {
            //        AddInlineSearch(field, prefix, tile, "DropDown", id, guid);
            //    }
            //}

            return tile.ToString();

        }

        private string GetImageSlider(ParentField field, string prefix, string style, string selectedValue, bool disabled, string pk, string guid, string html, string firstOptionText, bool load,DataAction action)
        {
      
            System.Text.StringBuilder slider = new StringBuilder();
            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            string insideTriggerAttributes = string.Empty;
            bool hasInsideDefault = false;
                     
           
            slider.Append("<div " + style + " " + GetDisabledHtmlAttribute(disabled) + " id='" + id + "' name='" + field.Name + "' viewName='" + field.View.Name + "' pk='" + pk + "' isGroupFilter='false' " + insideTriggerAttributes + GetInsideDependencyAttributes(field, prefix, guid) + html + " >");

            if ((!disabled && load) || hasInsideDefault )
            {

                bool useUniqueName = field.ParentHtmlControlType == ParentHtmlControlType.InsideDependencyUniqueNames;

                //by br 2
                //Dictionary<string, string> selectOptions = field.GetSelectOptions(isFilter, useUniqueName, null, null);
                Dictionary<string, string> selectOptions = field.GetSelectOptions(field.View as Durados.Web.Mvc.View, string.Empty, null, false);
                if (field.View.Database.DiagnosticsReportInProgress)
                {
                    if (selectOptions.Count > field.View.Database.DiagnosticsReport.OverLoadLimit)
                    {
                        Map.Logger.Log(field.View.Name, field.Name, field.View.Database.DiagnosticsReport.Name, string.Empty, field.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptions.Count, string.Empty, field.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                string[] selectedValues = field.SplitValues(selectedValue);

                slider.Append("<div class='slider-container'>");
                slider.Append("<ul class='ui-slider'>");
                //bool selectedValueFound = false;
                foreach (string key in selectOptions.Keys)
                {
                    string value = selectOptions[key];
                   
                    if (selectedValues.Contains(key) )
                    {

                        //string disabledAttr = GetDisabledHtmlAttribute();
                        //slider.Append("<li value='" + key + "' class='"+ liClass+" " + liClassSelected + "'>" + value + "</li>");
                        slider.Append("<li ><div  class='ui-slider-item'   value='" + key + "' onclick='slider.UpdateSelectedImage(this);'>" + value + "</div></li>");
                        //dropDown += "<option value='" + key + "' " + disabledAttr + ">" + value + "</option>";
                        //selectedValueFound = true;
                    }
                    else
                    {
                        //slider.Append("<li value='" + key + "' class='" + liClass + "'>" + value + "</li>");
                        slider.Append("<li ><div  class='ui-slider-item' value='" + key + "' onclick='slider.UpdateSelectedImage(this);'>" + value + "</div></li>");
                    }
                }

            }
            if (disabled && !string.IsNullOrEmpty(selectedValue))
            {
                //by br 3
                //Dictionary<string, string> selectOptions = field.GetSelectOptions();
                Dictionary<string, string> selectOptions = field.GetSelectOptions(field.View as Durados.Web.Mvc.View, string.Empty, null, false);

                if (field.View.Database.DiagnosticsReportInProgress)
                {
                    if (selectOptions.Count > field.View.Database.DiagnosticsReport.OverLoadLimit)
                    {
                        Map.Logger.Log(field.View.Name, field.Name, field.View.Database.DiagnosticsReport.Name, string.Empty, field.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptions.Count, string.Empty, field.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                string[] selectedValues = field.SplitValues(selectedValue);

                foreach (string key in selectOptions.Keys)
                {
                    if (selectedValues.Contains(key))
                    {
                        string value = selectOptions[key];
                        if (field.DoLocalize())
                        {
                            value = Map.Database.Localizer.Translate(value);
                        }
                        //tile.Append("<option value='" + key + "' " + ((selectedValue == key) ? "selected='selected'>" : ">").ToString() + value + "</option>");
                        //slider.Append("<li value='" + key + "' class= '"+liClass +" "+ ((selectedValue == key) ? liClassSelected : string.Empty) + " '>" + value + "</li>");
                        slider.Append("<li ><div  class='ui-slider-item'  value='" + key + "' onclick='slider.UpdateSelectedImage(this);'>" + value + "</div></li>");
                    }
                }
            }
            slider.Append("</ul>");
            string hidden = (action == DataAction.Create) ? GetHiddenForCreate(field, pk, guid) : GetHiddenForEdit(field, pk, selectedValue, guid);
            slider.Append(hidden);
            slider.Append("</div>");
            slider.Append("</div>");

          
            if (!load && !hasInsideDefault )
            {
                string onclicked = "";
                string onhovered = "";
                if ((!disabled && string.IsNullOrEmpty(field.InsideTriggerFieldName)) )
                {
                    onclicked = " onclick='LoadCheckListFilter($(\"#" + id + "\"), \"" + guid + "\")'";
                    onhovered = " onmouseover='$(this).addClass(\"ui-state-hover\")' onmouseout='$(this).removeClass(\"ui-state-hover\")'";
                }
                string icon = "";

                if (Map.Database.Localizer.Direction != "RTL")
                {
                    icon = "<div style='float: left;' class='ui-icon ui-icon-triangle-1-e'></div>";
                }
                else
                {
                    icon = "<div style='float: right;' class='ui-icon ui-icon-triangle-1-w'></div>";
                }

                if ((!disabled && string.IsNullOrEmpty(field.InsideTriggerFieldName)) )
                {
                    //Changed by br
                    string widthStyle =  string.Empty;
                    string displayStyle = "display: block;";

                    slider.Append("<span d_ph='ph' class='ui-dropdownchecklist ui-dropdownchecklist-selector-wrapper ui-widget' style='cursor: default; overflow: hidden; " + displayStyle + widthStyle + "'><span tabindex='0'" + onclicked + " class='ui-dropdownchecklist-selector ui-state-default' style='overflow: hidden; white-space: nowrap; " + displayStyle + widthStyle + "'" + onhovered + ">" + icon + "<span style='display: inline-block; white-space: nowrap; overflow: hidden' class='ui-dropdownchecklist-text'></span></span></span>");
                }
            }

                     return slider.ToString();

        

        }

        public override string GetElementForReport(Field field, string guid)
        {
            return GetElementForReport(field, string.Empty, string.Empty, guid);
        }

        public override string GetElementForReport(Field field, string pk, string value, string guid)
        {
            return GetElementForReport((ParentField)field, pk, value, guid);
        }

        public virtual string GetElementForReport(ParentField field, string pk, string value, string guid)
        {
            if (field.ParentHtmlControlType == ParentHtmlControlType.Autocomplete)
                return GetAutoCompleteForReprot(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Radio)
                return GetRadioForCreate(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.OutsideDependency)
                if (field.DependencyField == null)
                    return GetDropDownForReport(field, pk, guid);
                else
                    return GetOutsideDependencyForCreate(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.InsideDependency || field.ParentHtmlControlType == ParentHtmlControlType.InsideDependencyUniqueNames)
                if (field.DependencyField == null || field.InsideTriggerField == null)
                    return GetDropDownForReport(field, pk, guid);
                else
                    return GetInsideDependencyForCreate(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Groups)
                return GetGroupsForReport(field, pk, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Hidden)
                return GetHiddenForCreate(field, pk, guid);
            else
                return GetDropDownForReport(field, pk, guid);
        }

        public override string GetElementForFilter(Field field, object value, string guid)
        {
            return GetElementForFilter((ParentField)field, value, guid);
        }

        public virtual string GetElementForFilter(ParentField field, object value, string guid)
        {
            string style = GetFilterStyle(field);

            if (field.ParentHtmlControlType == ParentHtmlControlType.Autocomplete)
                if (field.AutocompleteFilter)
                    return GetAutoCompleteForFilter(field, value == null ? string.Empty : value.ToString(), guid);
                else
                    return GetDropDownForFilter(field, value == null ? string.Empty : value.ToString(), guid);
            //else if (field.ParentHtmlControlType == ParentHtmlControlType.OutsideDependency)
            //    return GetOutsideDependencyForFilter(field, string.Empty, guid);
            else if (field.ParentHtmlControlType == ParentHtmlControlType.Groups)
                return GetGroupsForFilter(field, value == null ? string.Empty : value.ToString(), guid);
            else
                return GetDropDownForFilter(field, value == null ? string.Empty : value.ToString(), guid);
        }

        protected virtual string GetDropDownForCreate(ParentField field, string pk, string guid)
        {
            return GetDropDown(field, createPrefix, GetStyle(field), field.ConvertDefaultToString(), field.IsDisableForCreate(), pk, guid, "class='" + field.CssClass + "'", "", true);

        }

        protected virtual string GetDropDownForReport(ParentField field, string pk, string guid)
        {
            return GetDropDown(field, createPrefix, GetStyle(field), field.ConvertDefaultToString(), false, pk, guid, "class='" + field.CssClass + "'", "", true);

        }

        protected virtual string GetGroupsForCreate(ParentField field, string pk, string guid)
        {
            return GetDropDownWithGroups(field, createPrefix, GetStyle(field), field.ConvertDefaultToString(), field.IsDisableForCreate(), pk, guid, "", "");

        }

        protected virtual string GetGroupsForReport(ParentField field, string pk, string guid)
        {
            return GetDropDownWithGroups(field, createPrefix, GetStyle(field), field.ConvertDefaultToString(), false, pk, guid, "", "");

        }

        protected virtual string GetHiddenForCreate(ParentField field, string pk, string guid)
        {
            return GetHidden(field, createPrefix, field.ConvertDefaultToString(), field.IsDisableForCreate(), pk, guid, "");

        }

        protected virtual string GetDropDownForEdit(ParentField field, string pk, string value, string guid)
        {
            return GetDropDown(field, editPrefix, GetStyle(field), value, field.IsDisableForEdit(guid), pk, guid, "class='" + field.CssClass + "'", "", true);
        }

        protected virtual string GetGroupsForEdit(ParentField field, string pk, string value, string guid)
        {
            return GetDropDownWithGroups(field, editPrefix, GetStyle(field), value, field.IsDisableForEdit(guid), pk, guid, "", "");

        }

        protected virtual string GetHiddenForEdit(ParentField field, string pk, string value, string guid)
        {
            return GetHidden(field, editPrefix, value, field.IsDisableForEdit(guid), pk, guid, "");

        }

        protected virtual string GetOutsideDependencyForEdit(ParentField field, string pk, string guid)
        {
            return GetOutsideDependency(field, editPrefix, GetStyle(field), string.Empty, field.IsDisableForEdit(guid), pk, guid);

        }

        protected virtual string GetOutsideDependencyForFilter(ParentField field, string pk, string guid)
        {
            return GetOutsideDependency(field, filterPrefix, string.Empty, string.Empty, field.DisableInFilter, pk, guid);

        }

        protected virtual string GetOutsideDependencyForCreate(ParentField field, string pk, string guid)
        {
            return GetOutsideDependency(field, createPrefix, GetStyle(field), string.Empty, field.IsDisableForCreate(), pk, guid);

        }

        protected virtual string GetInsideDependencyForEdit(ParentField field, string pk, string guid)
        {
            return GetInsideDependency(field, editPrefix, string.Empty, field.IsDisableForEdit(guid), pk, guid, string.Empty, string.Empty);

        }

        protected virtual string GetInsideDependencyForCreate(ParentField field, string pk, string guid)
        {
            return GetInsideDependency(field, createPrefix, string.Empty, field.IsDisableForCreate(), pk, guid, string.Empty, string.Empty);

        }

        protected virtual string GetInsideDependencyForReport(ParentField field, string pk, string guid)
        {
            return GetInsideDependency(field, createPrefix, string.Empty, false, pk, guid, string.Empty, string.Empty);

        }

        protected virtual string GetRadioForEdit(ParentField field, string pk, string guid)
        {
            return GetRadio(field, editPrefix, string.Empty, field.IsVisibleForEdit(), field.IsDisableForEdit(guid), field.RadioColumns, pk, guid);

        }

        protected virtual string GetRadioForCreate(ParentField field, string pk, string guid)
        {
            return GetRadio(field, createPrefix, field.ConvertDefaultToString(), field.IsVisibleForEdit(), field.IsDisableForEdit(guid), field.RadioColumns, pk, guid);

        }

        private string GetDisabledHtmlAttribute()
        {
            return "disabled='disabled'";
        }


        private string GetDisabledHtmlAttribute(bool disabled)
        {
            return ((disabled) ? GetDisabledHtmlAttribute() : string.Empty).ToString();
        }

        protected virtual string GetDropDown(ParentField field, string prefix, string style, string selectedValue, bool disabled, string pk, string guid, string html, string firstOptionText, bool load)
        {
            System.Text.StringBuilder dropDown = new StringBuilder();

            bool isFilter = prefix == filterPrefix;
            bool isGroupFilter = isFilter && field.View != null && field.View.FilterType == FilterType.Group ? true : false;
            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            string insideTriggerAttributes = string.Empty;
            bool hasInsideDefault = false;

            if (!isFilter && !string.IsNullOrEmpty(field.InsideTriggerFieldName))
            {
                if (field.ParentHtmlControlType == ParentHtmlControlType.InsideDependencyDefault && !string.IsNullOrEmpty(field.DependencyFieldName) && field.DependencyField != null)
                {
                    hasInsideDefault = true;
                    insideTriggerAttributes = " hasInsideDefault='hasInsideDefault' triggerName='" + field.InsideTriggerFieldName + "' dependencyFieldName='" + field.DependencyFieldName + "' dependencyViewName='" + field.DependencyField.View.Name + "' ";
                }
                else
                {
                    insideTriggerAttributes = " hasInsideTrigger='hasInsideTrigger' triggerName='" + field.InsideTriggerFieldName + "' ";
                }
            }
            dropDown.Append("<select " + style + " " + GetDisabledHtmlAttribute(disabled) + " id='" + id + "' name='" + field.Name + "' viewName='" + field.View.Name + "' pk='" + pk + "' isGroupFilter='" + isGroupFilter + "' " + insideTriggerAttributes + GetInsideDependencyAttributes(field, prefix, guid) + html + " >");

            dropDown.Append("<option value=''>" + firstOptionText + "</option>");


            if ((!disabled && load) || (hasInsideDefault && !isFilter))
            {

                bool useUniqueName = field.ParentHtmlControlType == ParentHtmlControlType.InsideDependencyUniqueNames;

                //by br 2
                //Dictionary<string, string> selectOptions = field.GetSelectOptions(isFilter, useUniqueName, null, null);
                Dictionary<string, string> selectOptions = field.GetSelectOptions(field.View as Durados.Web.Mvc.View, string.Empty, null, isFilter);
                if (field.View.Database.DiagnosticsReportInProgress)
                {
                    if (selectOptions.Count > field.View.Database.DiagnosticsReport.OverLoadLimit)
                    {
                        Map.Logger.Log(field.View.Name, field.Name, field.View.Database.DiagnosticsReport.Name, string.Empty, field.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptions.Count, string.Empty, field.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                string[] selectedValues = field.SplitValues(selectedValue);
                // selected='selected'
                //if (isFilter)
                //{
                //    dropDown += "<option value='" + Database.EmptyString + "'>" + field.View.Database.EmptyDisplay + "</option>";
                //}

                bool selectedValueFound = false;
                foreach (string key in selectOptions.Keys)
                {
                    string value = selectOptions[key];
                    if (field.DoLocalize())
                    {
                        value = Map.Database.Localizer.Translate(value);
                    }
                    if (selectedValues.Contains(key) || (isFilter && selectedValues.Contains(key.Split(',')[0])))
                    {

                        //string disabledAttr = GetDisabledHtmlAttribute();
                        dropDown.Append("<option value='" + key + "' selected='selected'>" + value + "</option>");
                        //dropDown += "<option value='" + key + "' " + disabledAttr + ">" + value + "</option>";
                        selectedValueFound = true;
                    }
                    else
                    {
                        dropDown.Append("<option value='" + key + "' >" + value + "</option>");
                    }
                }

                if (!string.IsNullOrEmpty(selectedValue) && isFilter && !selectedValueFound)
                {
                    DataRow row = null;
                    if (selectedValues.Length == field.ParentView.DataTable.PrimaryKey.Length)
                        row = field.ParentView.GetDataRow(selectedValue);
                    if (row != null)
                    {
                        string key = field.ParentView.GetPkValue(row);
                        string value = field.ParentView.GetDisplayValue(row);
                        if (field.DoLocalize())
                        {
                            value = Map.Database.Localizer.Translate(value);
                        }
                        //string disabledAttr = GetDisabledHtmlAttribute();
                        dropDown.Append("<option value='" + key + "' " + ((selectedValue == key) ? "selected='selected'>" : ">").ToString() + value + "</option>");

                    }
                    else
                    {
                        //by br 3
                        //selectOptions = field.GetSelectOptions();
                        selectOptions = field.GetSelectOptions(field.View as Durados.Web.Mvc.View, string.Empty, null, isFilter);

                        if (field.View.Database.DiagnosticsReportInProgress)
                        {
                            if (selectOptions.Count > field.View.Database.DiagnosticsReport.OverLoadLimit)
                            {
                                Map.Logger.Log(field.View.Name, field.Name, field.View.Database.DiagnosticsReport.Name, string.Empty, field.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptions.Count, string.Empty, field.View.Database.DiagnosticsReport.DateTime);
                            }
                        }

                        selectedValues = field.SplitValues(selectedValue);

                        foreach (string key in selectOptions.Keys)
                        {
                            if (selectedValues.Contains(key))
                            {
                                string value = selectOptions[key];
                                if (field.DoLocalize())
                                {
                                    value = Map.Database.Localizer.Translate(value);
                                }
                                //string disabledAttr = GetDisabledHtmlAttribute();
                                dropDown.Append("<option value='" + key + "' selected='selected'>" + value + "</option>");
                                //dropDown += "<option value='" + key + "' " + disabledAttr + ">" + value + "</option>";
                                //break;
                            }
                        }
                    }
                }
                //else
                //{
                //    Dictionary<string, string> selectOptions = field.GetSelectOptions();

                //    string[] selectedValues = field.SplitValues(selectedValue);

                //    foreach (string key in selectOptions.Keys)
                //    {
                //        string value = selectOptions[key];
                //        if (field.DoLocalize())
                //        {
                //            value = Map.Database.Localizer.Translate(value);
                //        }
                //        //string disabledAttr = ((selectedValues.Contains(key)) ? GetDisabledHtmlAttribute() : string.Empty).ToString();
                //        dropDown += "<option value='" + key + "' " + ((selectedValue == key) ? "selected='selected'>" : ">").ToString() + value + "</option>";
                //        //dropDown += "<option value='" + key + "' " + disabledAttr + ">" + value + "</option>";
                //    }
                //}
            }
            else if (disabled && !string.IsNullOrEmpty(selectedValue))
            {
                //by br 3
                //Dictionary<string, string> selectOptions = field.GetSelectOptions();
                Dictionary<string, string> selectOptions = field.GetSelectOptions(field.View as Durados.Web.Mvc.View, string.Empty, null, isFilter);

                if (field.View.Database.DiagnosticsReportInProgress)
                {
                    if (selectOptions.Count > field.View.Database.DiagnosticsReport.OverLoadLimit)
                    {
                        Map.Logger.Log(field.View.Name, field.Name, field.View.Database.DiagnosticsReport.Name, string.Empty, field.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptions.Count, string.Empty, field.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                string[] selectedValues = field.SplitValues(selectedValue);

                foreach (string key in selectOptions.Keys)
                {
                    if (selectedValues.Contains(key))
                    {
                        string value = selectOptions[key];
                        if (field.DoLocalize())
                        {
                            value = Map.Database.Localizer.Translate(value);
                        }
                        dropDown.Append("<option value='" + key + "' " + ((selectedValue == key) ? "selected='selected'>" : ">").ToString() + value + "</option>");
                    }
                }
            }

            dropDown.Append("</select>");

            //if ((!load && !hasInsideDefault) || isFilter)
            //{
            //    string onclicked = "";
            //    string onhovered = "";
            //    if ((!disabled && string.IsNullOrEmpty(field.InsideTriggerFieldName)) || isFilter)
            //    {
            //        onclicked = " onclick='LoadCheckListFilter($(\"#" + id + "\"), \"" + guid + "\")'";
            //        onhovered = " onmouseover='$(this).addClass(\"ui-state-hover\")' onmouseout='$(this).removeClass(\"ui-state-hover\")'";
            //    }
            //    string icon = "";

            //    if (Map.Database.Language.Direction != "RTL")
            //    {
            //        icon = "<div style='float: left;' class='ui-icon ui-icon-triangle-1-e'></div>";
            //    }
            //    else
            //    {
            //        icon = "<div style='float: right;' class='ui-icon ui-icon-triangle-1-w'></div>";
            //    }

            //    if ((!disabled && string.IsNullOrEmpty(field.InsideTriggerFieldName)) || isFilter)
            //    {
            //        dropDown.Append("<span d_ph='ph' class='ui-dropdownchecklist ui-dropdownchecklist-selector-wrapper ui-widget' style='display: inline-block; cursor: default; overflow: hidden'><span tabindex='0'" + onclicked + " class='ui-dropdownchecklist-selector ui-state-default' style='display: inline-block; overflow: hidden; white-space: nowrap'" + onhovered + ">" + icon + "<span style='display: inline-block; white-space: nowrap; overflow: hidden;width: " + field.View.Database.MinColumnWidth + "px' class='ui-dropdownchecklist-text'></span></span></span>");
            //    }
            //}

            if (!load && (!hasInsideDefault || isFilter))
            {
                string onclicked = "";
                string onhovered = "";
                if ((!disabled && string.IsNullOrEmpty(field.InsideTriggerFieldName)) || isFilter)
                {
                    onclicked = " onclick='LoadCheckListFilter($(\"#" + id + "\"), \"" + guid + "\")'";
                    onhovered = " onmouseover='$(this).addClass(\"ui-state-hover\")' onmouseout='$(this).removeClass(\"ui-state-hover\")'";
                }
                string icon = "";

                if (Map.Database.Localizer.Direction != "RTL")
                {
                    icon = "<div style='float: left;' class='ui-icon ui-icon-triangle-1-e'></div>";
                }
                else
                {
                    icon = "<div style='float: right;' class='ui-icon ui-icon-triangle-1-w'></div>";
                }

                if ((!disabled && string.IsNullOrEmpty(field.InsideTriggerFieldName)) || isFilter)
                {
                    //Changed by br
                    string widthStyle = isFilter ? GetFilterWidthStyle(field) : string.Empty;
                    string displayStyle = "display: " + (isGroupFilter ? "inline-block" : "block") + ";";

                    dropDown.Append("<span d_ph='ph' class='ui-dropdownchecklist ui-dropdownchecklist-selector-wrapper ui-widget' style='cursor: default; overflow: hidden; " + displayStyle + widthStyle + "'><span tabindex='0'" + onclicked + " class='ui-dropdownchecklist-selector ui-state-default' style='overflow: hidden; white-space: nowrap; " + displayStyle + widthStyle + "'" + onhovered + ">" + icon + "<span style='display: inline-block; white-space: nowrap; overflow: hidden' class='ui-dropdownchecklist-text'></span></span></span>");
                }
            }

            if (!isFilter)
            {
                DataAction dataAction;
                if (prefix == createPrefix)
                    dataAction = DataAction.Create;
                else
                    dataAction = DataAction.Edit;

                if (field.InlineAdding && ((View)field.ParentView).IsCreatable() && !field.IsDisableForInline(dataAction, guid))
                {
                    AddInlineAdding(field, prefix, dropDown, "DropDown", id, guid);
                }

                if (field.InlineEditing && ((View)field.ParentView).IsEditable(guid) && !field.IsDisableForInline(dataAction, guid))
                {
                    AddInlineEditing(field, prefix, dropDown, "DropDown", id, guid);
                }

                if (field.InlineDuplicate && ((View)field.ParentView).IsCreatable() && !field.IsDisableForInline(dataAction, guid))
                {
                    AddInlineDuplicate(field, prefix, dropDown, "DropDown", id, guid);
                }

                if (field.InlineSearch && ((View)field.ParentView).IsAllow() && !field.IsDisableForInline(dataAction, guid))
                {
                    AddInlineSearch(field, prefix, dropDown, "DropDown", id, guid);
                }
            }

            return dropDown.ToString();

        }
        protected virtual string GetInsideDependencyAttributes(ParentField field, string prefix, string guid)
        {
            if (field.InsideTriggeredFields.Count() == 0 || prefix == filterPrefix)
                return string.Empty;

            //if (field.ParentHtmlControlType == ParentHtmlControlType.InsideDependencyDefault)
            //{
            //    return string.Empty;
            //}
            //else
            //{
            string dependentFieldNames = "";
            foreach (string fieldName in field.InsideTriggeredFieldsNames)
            {
                dependentFieldNames += fieldName + ";";
            }

            dependentFieldNames = dependentFieldNames.TrimEnd(';');


            string attr = "  insideDependency='insideDependency' dependentFieldViewName='" + field.View.Name + "' dependentFieldNames='" + dependentFieldNames + "' onchange='insideDependencyChange(this, \"" + guid + "\")' ";

            return attr;
            //}
        }

        protected virtual string GetDropDownWithGroups(ParentField field, string prefix, string style, string selectedValue, bool disabled, string pk, string guid, string html, string firstOptionText)
        {
            StringBuilder dropDown = new StringBuilder();

            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            dropDown.Append("<select " + style + " " + GetDisabledHtmlAttribute(disabled) + " id='" + id + "' name='" + field.Name + "' viewName='" + field.View.Name + "' pk='" + pk + "' " + html + " >");

            dropDown.Append("<option value=''>" + firstOptionText + "</option>");

            Dictionary<string, Dictionary<string, string>> selectOptionsWithGroups = field.GetSelectOptionsWithGroups();

            foreach (string group in selectOptionsWithGroups.Keys)
            {

                Dictionary<string, string> selectOptions = selectOptionsWithGroups[group];

                dropDown.Append("<optgroup label='" + group + "'>");


                string[] selectedValues = field.SplitValues(selectedValue);

                foreach (string key in selectOptions.Keys)
                {
                    string value = selectOptions[key];
                    if (field.DoLocalize())
                    {
                        value = Map.Database.Localizer.Translate(value);
                    }
                    dropDown.Append("<option value='" + key + "' " + ((selectedValues.Contains(key)) ? "selected='selected'>" : ">").ToString() + value + "</option>");
                }

                dropDown.Append("</optgroup>");

            }

            dropDown.Append("</select>");

            DataAction dataAction;
            if (prefix == createPrefix)
                dataAction = DataAction.Create;
            else
                dataAction = DataAction.Edit;

            if (field.InlineAdding && prefix != filterPrefix && ((View)field.ParentView).IsCreatable() && !field.IsDisableForInline(dataAction, guid))
            {
                AddInlineAdding(field, prefix, dropDown, "DropDown", id, guid);
            }

            if (field.InlineEditing && prefix != filterPrefix && ((View)field.ParentView).IsEditable(guid) && !field.IsDisableForInline(dataAction, guid))
            {
                AddInlineEditing(field, prefix, dropDown, "DropDown", id, guid);
            }

            return dropDown.ToString();

        }



        protected virtual string GetHidden(ParentField field, string prefix, string selectedValue, bool disabled, string pk, string guid, string html)
        {
            string hidden = string.Empty;

            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            hidden += "<input type='hidden' id='" + id + "' name='" + field.Name + "' viewName='" + field.View.Name + "' pk='" + pk + "' " + html + " />";

            return hidden;

        }

        public virtual string GetDependencyTriggerDropDown(ParentField dependencyField, ParentField dependentField, string prefix, string style, string selectedValue, bool disabled, string pk, string guid)
        {
            bool recursiveTrigger = dependencyField.DependencyField != null && dependencyField.GetHtmlControlType() == HtmlControlType.OutsideDependency;

            StringBuilder dropDown = new StringBuilder();
            string dependencyRoot = string.Empty;

            if (recursiveTrigger)
            {
                dropDown.Append(GetDependencyTriggerDropDown((ParentField)dependencyField.DependencyField, dependencyField, prefix, style, string.Empty, false, pk, guid));
            }
            else
            {
                dependencyRoot = " dependencyRoot='dependencyRoot' ";
            }

            string id = guid + prefix + dependencyField.Name.ReplaceNonAlphaNumeric();
            dropDown.Append("<select " + style + " outsideDependency='outsideDependency' dependentFieldName='" + dependentField.Name + "' dependentFieldViewName='" + dependentField.View.Name + "' " + GetDisabledHtmlAttribute(disabled) + " id='" + id + "' name='" + id + "' viewName='" + dependencyField.View.Name + "' pk='" + pk + "' " + dependencyRoot + " >");

            dropDown.Append("<option value=''>" + dependencyField.DisplayName + "</option>");

            if (recursiveTrigger)
            {
            }
            else
            {
                //by br 3
                //br todo
                //Dictionary<string, string> selectOptions = dependencyField.GetSelectOptions();
                Dictionary<string, string> selectOptions = dependencyField.GetSelectOptions(dependencyField.View as Durados.Web.Mvc.View, string.Empty, null, false);

                if (dependencyField.View.Database.DiagnosticsReportInProgress)
                {
                    if (selectOptions.Count > dependencyField.View.Database.DiagnosticsReport.OverLoadLimit)
                    {
                        Map.Logger.Log(dependencyField.View.Name, dependencyField.Name, dependencyField.View.Database.DiagnosticsReport.Name, string.Empty, dependencyField.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptions.Count, string.Empty, dependencyField.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                foreach (string key in selectOptions.Keys)
                {
                    string value = selectOptions[key];
                    if (dependencyField.DoLocalize())
                    {
                        value = Map.Database.Localizer.Translate(value);
                    }
                    dropDown.Append("<option value='" + key + "' " + ((selectedValue == key) ? "selected='selected'>" : ">").ToString() + value + "</option>");
                }
            }

            dropDown.Append("</select>");


            return dropDown.ToString();

        }

        protected virtual string GetOutsideDependency(ParentField field, string prefix, string style, string selectedValue, bool disabled, string pk, string guid)
        {
            StringBuilder dropDown = new StringBuilder();

            dropDown.Append(GetDependencyTriggerDropDown((ParentField)field.DependencyField, field, prefix, style, string.Empty, false, pk, guid));

            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            dropDown.Append("&nbsp;-&nbsp;<select " + GetDisabledHtmlAttribute(disabled) + " id='" + id + "' name='" + field.Name + "' >");
            dropDown.Append("<option value=''>" + field.DisplayName + "</option>");

            dropDown.Append("</select>");

            if (field.InlineAdding && prefix != filterPrefix)
            {
                AddInlineAdding(field, prefix, dropDown, "DropDown", id, guid);
            }

            return dropDown.ToString();

        }

        protected virtual string GetInsideDependency(ParentField field, string prefix, string selectedValue, bool disabled, string pk, string guid, string html, string firstOptionText)
        {

            return GetDropDown(field, prefix, GetStyle(field), selectedValue, disabled, pk, guid, html, firstOptionText, false);

        }


        protected virtual string GetRadio(ParentField field, string prefix, string selectedValue, bool visible, bool disabled, int columns, string pk, string guid)
        {
            //by br 3
            //br todo
            //Dictionary<string, string> selectOptions = field.GetSelectOptions();
            Dictionary<string, string> selectOptions = field.GetSelectOptions(field.View as Durados.Web.Mvc.View, string.Empty, null, false);

            string radio = "<table radioButtons='radioButtons' class='radioTable' id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "' style='" + GetVisibilityStyle(visible) + "' viewName='" + field.View.Name + "' pk='" + pk + "'>";

            int col = 0;

            if (columns == 0)
                columns = selectOptions.Count;

            if (columns < 0)
                columns = selectOptions.Count + 1;


            foreach (string key in selectOptions.Keys)
            {
                string value = selectOptions[key];
                if (field.DoLocalize())
                {
                    value = Map.Database.Localizer.Translate(value);
                }

                if (col % columns == 0)
                {
                    if (col > 0)
                    {
                        radio += "</tr>";
                    }
                    radio += "<tr>";
                }

                radio += "<td>";
                radio += GetRadio(field, prefix, key, key == selectedValue, disabled, guid);
                radio += value;
                radio += "</td>";

                col++;
            }

            radio += "</tr>";
            radio += "</table>";
            return radio;

        }

        protected virtual string GetRadio(ParentField field, string prefix, string value, bool @checked, bool disabled, string guid)
        {
            return "<input colName='" + field.Name + "' type='radio'  name='" + guid + prefix + field.Name + "' value='" + value + "' " + (@checked ? "checked='checked'" : "") + GetDisabledHtmlAttribute(disabled) + " />";

        }

        protected virtual void AddInlineAdding(ParentField field, string prefix, StringBuilder element, string type, string id, string guid)
        {
            //string inlineAdding = string.Empty;
            if (field.InlineAdding && prefix != filterPrefix)
            {
                element.Insert(0, "<table border=0 cellspacing=0 cellpadding=0><tr><td>");

                //inlineAdding += element;
                element.Append("</td><td>");


                string title = Map.Database.Localizer.Translate("Add new") + " " + field.GetLocalizedDisplayName();
                string className = "Add-icon";
                string onClick = "CreateDialog.CreateAndOpen('" + field.ParentView.Name + "','" + field.ParentView.DisplayName + "','" + type + "','" + id + "','" + ((Durados.Web.Mvc.View)field.ParentView).GetInlineAddingCreateUrl() + "', false, '" + guid + "')";

                element.Append(GetInlineAddingImg(guid, title, className, onClick));

                element.Append("</td></tr></table>");

            }
            //return inlineAdding;
        }

        protected virtual void AddInlineEditing(ParentField field, string prefix, StringBuilder element, string type, string id, string guid)
        {
            //string inlineEditing = string.Empty;
            if (field.InlineEditing && prefix != filterPrefix)
            {
                element.Insert(0, "<table border=0 cellspacing=0 cellpadding=0><tr><td>");

                //inlineEditing += element;
                element.Append("</td><td>");


                string title = Map.Database.Localizer.Translate("Edit") + " " + field.GetLocalizedDisplayName();
                string className = "Edit-icon";
                string onClick = "InlineEditingDialog.CreateAndOpen('" + field.ParentView.Name + "','" + field.ParentView.DisplayName + "','" + type + "','" + id + "','" + ((Durados.Web.Mvc.View)field.ParentView).GetInlineEditingEditUrl() + "', '" + guid + "')";

                element.Append(GetInlineAddingImg(guid, title, className, onClick));

                element.Append("</td></tr></table>");

            }
            //return inlineEditing;
        }

        protected virtual void AddInlineDuplicate(ParentField field, string prefix, StringBuilder element, string type, string id, string guid)
        {
            //string inlineDuplicate = string.Empty;
            if (field.InlineDuplicate && prefix != filterPrefix)
            {
                element.Insert(0, "<table border=0 cellspacing=0 cellpadding=0><tr><td>");

                //inlineDuplicate += element;
                element.Append("</td><td>");


                string title = Map.Database.Localizer.Translate("Duplicate") + " " + field.GetLocalizedDisplayName();
                string className = "Duplicate-icon";
                string onClick = "CreateDialog.CreateAndOpen('" + field.ParentView.Name + "','" + field.ParentView.DisplayName + "','" + type + "','" + id + "','" + ((Durados.Web.Mvc.View)field.ParentView).GetInlineAddingCreateUrl() + "', true, '" + guid + "')";

                element.Append(GetInlineAddingImg(guid, title, className, onClick));

                element.Append("</td></tr></table>");

            }
            //return inlineDuplicate;
        }

        protected virtual void AddInlineSearch(ParentField field, string prefix, StringBuilder element, string type, string id, string guid)
        {
            //string inlineSearch = string.Empty;
            if (field.InlineSearch && prefix != filterPrefix)
            {
                element.Insert(0, "<table border=0 cellspacing=0 cellpadding=0><tr><td>");

                //inlineSearch += element;
                element.Append("</td><td>");


                View searchView = (string.IsNullOrEmpty(field.InlineSearchView)) ? (View)field.ParentView : (View)Map.Database.Views[field.InlineSearchView];

                string title = Map.Database.Localizer.Translate("Search for") + " " + searchView.GetLocalizedDisplayName();
                string className = "Search-icon";
                string searchFilterParameters = field.SearchFilter ?? string.Empty;
                string onClick = "SearchDialog.CreateAndOpen('" + searchView.Name + "','" + searchView.DisplayName + "','" + type + "','" + id + "','" + searchView.GetIndexUrl() + "?firstTime=true&disabled=true" + "', '" + guid + "', this, '" + searchFilterParameters + "')";

                guid = searchView.Name + "_" + Durados.Web.Mvc.Infrastructure.ShortGuid.Next() + "_";

                //string dependencyFilter = string.Empty;

                //if (field.InsideTriggerField != null)
                //{
                //    dependencyFilter = "&d_filter=" + field.InsideTriggerField.Name + "|$$";
                //}

                element.Append(GetInlineAddingImg(guid, title, className, onClick));

                element.Append("</td></tr></table>");

            }
            //return inlineSearch;
        }

        protected virtual string GetText(ParentField field, string prefix, string columnType, string style, string cssClass, bool disabled, string value, string pk, string guid)
        {
            value = value ?? string.Empty;

            return "<input " + style + " " + GetDisabledHtmlAttribute(disabled) + " type='text' " + (string.IsNullOrEmpty(cssClass) ? "" : ("class='" + cssClass + "'")).ToString() + " id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "'" + columnType + " value='" + System.Web.HttpUtility.HtmlDecode(value) + "' viewName='" + field.View.Name + "' pk='" + pk + "' />";
        }

        //public virtual string GetCheckListForFilter(ParentField field, string guid)
        //{
        //    return GetDropDown(field, "filter_", null, field.DisableInFilter, string.Empty, guid, " style='display:none' multiple='multiple' class='dropdownchecklist' onkeypress='handleEnterFilter(this, event, \"" + guid + "\")';", "(All)", true);
        //}

        protected virtual string GetDropDownForFilter(ParentField field, string selectedKey, string guid)
        {
            string style = GetFilterStyle(field);

            if (field.MultiFilter)
            {
                bool load = !string.IsNullOrEmpty(selectedKey) || !string.IsNullOrEmpty(field.DefaultFilter);
                return GetDropDown(field, "filter_", style, selectedKey, field.DisableInFilter, string.Empty, guid, " style='display:none' multiple='multiple' filter='filter' class='dropdownchecklist' onkeypress='handleEnterFilter(this, event, \"" + guid + "\")'", "(All)", load);
                //return GetText(field, filterPrefix, " filterType='" + UI.Json.AdvancedFilterType.Multi.ToString() + "' columnType='column' onkeypress='handleEnterFilter(this, event, \"" + guid + "\")'; ", "advancedFilter", false, null, string.Empty, guid);
            }
            else
                return GetDropDown(field, "filter_", style, selectedKey, field.DisableInFilter, string.Empty, guid, " onkeypress='handleEnterFilter(this, event, \"" + guid + "\")';", "(All)", true);

        }

        protected virtual string GetGroupsForFilter(ParentField field, string selectedKey, string guid)
        {
            string style = GetFilterStyle(field);

            if (field.MultiFilter)
                return GetDropDownWithGroups(field, "filter_", style, selectedKey, field.DisableInFilter, string.Empty, guid, " multiple='multiple' class='dropdownchecklist' onkeypress='handleEnterFilter(this, event, \"" + guid + "\")';", "(All)");
            else
                return GetDropDownWithGroups(field, "filter_", style, selectedKey, field.DisableInFilter, string.Empty, guid, " onkeypress='handleEnterFilter(this, event, \"" + guid + "\")';", "(All)");

        }

        protected virtual string GetAutoCompleteForCreate(ParentField field, string pk, string guid)
        {
            string value = field.ConvertDefaultToString();

            return GetAutoCompleteForRowView(field, createPrefix, field.IsDisableForCreate(), pk, value, guid);
        }

        protected virtual string GetAutoCompleteForReprot(ParentField field, string pk, string guid)
        {
            string value = field.ConvertDefaultToString();

            return GetAutoCompleteForRowView(field, createPrefix, false, pk, value, guid);
        }

        protected virtual string GetAutoCompleteForEdit(ParentField field, string pk, string value, string guid)
        {
            return GetAutoCompleteForRowView(field, editPrefix, field.IsDisableForEdit(guid), pk, value, guid);
        }

        protected virtual string GetAutoCompleteForRowView(ParentField field, string prefix, bool disabled, string pk, string value, string guid)
        {
            return GetAutoComplete(field, prefix, GetStyle(field), value, "", disabled, pk, string.Empty, guid);
        }

        protected virtual string GetAutoComplete(ParentField field, string prefix, string style, string value, string columnType, bool disabled, string pk, string html, string guid)
        {
            string displayValue = field.GetLocalizedDisplayValue(value) ?? string.Empty;

            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            string cssClass = string.IsNullOrEmpty(field.CssClass) ? "'" : " " + field.CssClass + "'";
            StringBuilder autocomplete = new StringBuilder();
            autocomplete.Append("<input " + style + " class='Autocomplete" + cssClass + " type='text' id='" + id + "' name='" + field.Name + "' value='" + displayValue + "'" + columnType + " valueId='" + value + "' " + GetDisabledHtmlAttribute(disabled) + " viewName='" + field.View.Name + "' pk='" + pk + "' " + html + "  d_type='Autocomplete' />");

            DataAction dataAction;
            if (prefix == createPrefix)
                dataAction = DataAction.Create;
            else
                dataAction = DataAction.Edit;

            if (field.InlineAdding && prefix != filterPrefix && ((View)field.ParentView).IsCreatable() && !field.IsDisableForInline(dataAction, guid))
            {
                AddInlineAdding(field, prefix, autocomplete, "Autocomplete", id, guid);
            }

            if (field.InlineEditing && prefix != filterPrefix && ((View)field.ParentView).IsEditable(guid) && !field.IsDisableForInline(dataAction, guid))
            {
                AddInlineEditing(field, prefix, autocomplete, "Autocomplete", id, guid);
            }

            if (field.InlineDuplicate && prefix != filterPrefix && ((View)field.ParentView).IsCreatable() && !field.IsDisableForInline(dataAction, guid))
            {
                AddInlineDuplicate(field, prefix, autocomplete, "Autocomplete", id, guid);
            }

            if (prefix != filterPrefix && ((View)field.ParentView).IsAllow() && !field.IsDisableForInline(dataAction, guid))
            {
                AddInlineSearch(field, prefix, autocomplete, "Autocomplete", id, guid);
            }

            return autocomplete.ToString();
        }

        protected virtual string GetAutoCompleteForFilter(ParentField field, string value, string guid)
        {
            return GetAutoComplete(field, "filter_", GetFilterStyle(field), value, " columnType='parent' ", field.DisableInFilter, string.Empty, " onkeypress='handleEnterFilter(this, event, \"" + guid + "\")';", guid);
        }

        public override HtmlControlType GetHtmlControlType(Field field)
        {
            return GetHtmlControlType((ParentField)field);
        }

        public virtual HtmlControlType GetHtmlControlType(ParentField field)
        {
            return (HtmlControlType)Enum.Parse(typeof(HtmlControlType), field.ParentHtmlControlType.ToString());
        }

        public override string GetValidationElements(Field field, DataAction dataAction, string guid)
        {
            string validationElements = string.Empty;

            validationElements += GetRequiredValidationElement(field, guid);

            return validationElements;
        }



    }
}
