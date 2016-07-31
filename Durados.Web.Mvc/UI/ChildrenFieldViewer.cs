using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Durados.Web.Mvc.UI.Helpers;

using Durados.Web.Mvc.Infrastructure;

namespace Durados.Web.Mvc.UI
{
    public class ChildrenFieldViewer : FieldViewer
    {
		#region Methods (32) 

		// Public Methods (17) 

        public override string GetElementForCreate(Field field, string guid)
        {
            return GetElementForCreate(field, string.Empty, string.Empty, guid);
        }

        public override string GetElementForCreate(Field field, string pk, string value, string guid)
        {
            return GetElementForCreate((ChildrenField)field, pk, value, guid);
        }

        public virtual string GetElementForCreate(ChildrenField field, string pk, string value, string guid)
        {
            View childrenView = ((View)field.ChildrenView);
            View view = ((View)field.View);
            if (field.ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
                return GetCheckList(field, createPrefix, false, pk, guid, false);
            //return GetDiv(field, createPrefix, guid, GetCheckListUrl(field, guid, GetUrlWithoutQuery(view, view.CheckListAction), createPrefix), false);
            else if (field.ChildrenHtmlControlType == ChildrenHtmlControlType.Grid)
                return GetDiv(field, createPrefix, guid, GetNavigationUrl(field, null, guid), true);
            else
                return string.Empty;
        }

        public override string GetElementForEdit(Field field, string guid)
        {
            return GetElementForEdit(field, string.Empty, string.Empty, guid);
        }

        public override string GetElementForEdit(Field field, DataRow dataRow, string guid)
        {
            string pk = field.View.GetPkValue(dataRow);
            string value = string.Empty;

            return GetElementForEdit((ParentField)field, pk, value, guid);
        }

        public virtual string GetElementForEdit(ChildrenField field, string pk, string value, string guid)
        {
            View childrenView = ((View)field.ChildrenView);
            View view = ((View)field.View);
            if (field.ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
                //return GetDiv(field, editPrefix, guid, GetCheckListUrl(field, guid, GetUrlWithoutQuery(view, view.CheckListAction), editPrefix), false);
                return GetCheckList(field, editPrefix, false, pk, guid, false);
            else if (field.ChildrenHtmlControlType == ChildrenHtmlControlType.Grid)
                return GetDiv(field, editPrefix, guid, GetNavigationUrl(field, null, guid), false);
            else
                return string.Empty;
        }

        public override string GetElementForEdit(Field field, string pk, string value, string guid)
        {
            return GetElementForEdit((ChildrenField)field, pk, value, guid);
        }

        //public override string GetElementForCreate(Field field, string guid)
        //{
        //    throw new NotImplementedException();
        //}
        //public override string GetElementForCreate(Field field, string pk, string value, string guid)
        //{
        //    throw new NotImplementedException();
        //}
        //public override string GetElementForEdit(Field field, string guid)
        //{
        //    throw new NotImplementedException();
        //}
        //public override string GetElementForEdit(Field field, DataRow dataRow, string guid)
        //{
        //    throw new NotImplementedException();
        //}
        //public override string GetElementForEdit(Field field, string pk, string value, string guid)
        //{
        //    throw new NotImplementedException();
        //}
        public override string GetElementForFilter(Field field, object value, string guid)
        {
            if (((ChildrenField)field).ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
                return GetCheckListForFilter((ChildrenField)field, filterPrefix, false, value == null ? string.Empty : value.ToString(), guid);
            else
                return string.Empty;
        }

        public override string GetElementForInlineAdding(Field field, string guid)
        {
            return GetElementForCreate(field, guid);
        }

        public override string GetElementForInlineEditing(Field field, string guid)
        {
            return GetElementForEdit(field, guid);
        }

        public override string GetElementForReport(Durados.Field field, string guid)
        {
            return GetElementForCreate(field, guid);
        }

        public override string GetElementForReport(Field field, string pk, string value, string guid)
        {
            return GetElementForCreate((ChildrenField)field, pk, value, guid);
        }

        public virtual string GetElementForReport(ChildrenField field, string pk, string value, string guid)
        {
            return GetElementForCreate(field, guid);
        }

        public override string GetElementForTableView(Field field, DataRow dataRow, string guid)
        {
            return GetElementForTableView((ChildrenField)field, dataRow, guid);
        }

        public virtual string GetElementForTableView(ChildrenField field, DataRow dataRow, string guid)
        {
            View childrenView = ((View)field.ChildrenView);
            if (!childrenView.IsAllow())
                if (dataRow == null)
                    return string.Empty;
                else
                    return field.ConvertToString(dataRow);
            else
            {
                if (field.FieldType == FieldType.Children && ((ChildrenField)field).ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
                {
                    return string.Empty;
                }
                else
                {
                    if (dataRow == null)
                        return GetNavigationUrl(field, null, guid);

                        //if (field.ChildrenHtmlControlType == ChildrenHtmlControlType.Grid)
                    //    return GetNavigationUrl(field, null);
                    //else if (field.ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
                    //    return GetNavigationUrl(field, null, GetUrlWithoutQuery(field, childrenView.CheckListAction));
                    //else
                    //    return string.Empty;
                    else
                        return GetDivForTable(field, string.Empty, guid, GetNavigationUrl(field, dataRow, guid, true), false, dataRow);

                    //return "<a href='" + GetNavigationUrl(field, dataRow) + "'>" + field.ConvertToString(dataRow) + "</a>";
                }
            }
        }

        public override HtmlControlType GetHtmlControlType(Field field)
        {
            switch (((ChildrenField)field).ChildrenHtmlControlType)
            {
                case ChildrenHtmlControlType.Grid:
                    return HtmlControlType.Grid;
                case ChildrenHtmlControlType.CheckList:
                    return HtmlControlType.CheckList;
                default:
                    return HtmlControlType.Grid;
                    //throw new DuradosException("A Children Field can not handle the type " + ((ChildrenField)field).ChildrenHtmlControlType.ToString());
            }
        }

        public override string GetValidationElements(Field field, DataAction dataAction, string guid)
        {
            return GetRequiredValidationElement(field, guid);
        }
		// Protected Methods (13) 

        protected virtual void AddInlineSearch(ChildrenField field, string prefix, StringBuilder element, string type, string id, string guid)
        {
            //string inlineSearch = string.Empty;
            Durados.ParentField parentField = null;
            Durados.ParentField fkField = null;
            View searchView = (View)field.GetOtherParentView(out parentField, out fkField);
            if (field.InlineSearch && prefix != filterPrefix && searchView.IsAllow())
            {
                element.Insert(0, "<table border=0 cellspacing=0 cellpadding=0><tr><td>");

                //inlineSearch += element;
                element.Append("</td><td>");



                string title = "Search for " + searchView.DisplayName;
                string className = "Search-icon";
                guid = searchView.Name + "_" + Durados.Web.Mvc.Infrastructure.ShortGuid.Next() + "_";
                string onClick = "SearchDialog.CreateAndOpen('" + searchView.Name + "','" + searchView.DisplayName + "','" + type + "','" + id + "','" + searchView.GetIndexUrl() + "?firstTime=true&disabled=true" + "', '" + guid + "')";

                element.Append(GetInlineAddingImg(guid, title, className, onClick));

                element.Append("</td></tr></table>");

            }
            //return inlineSearch;
        }

        protected virtual string GetCheckList(ChildrenField field, string prefix, bool disabled, string pk, string guid, bool forFilter)
        {
            StringBuilder selectList = new StringBuilder();
            string insideTriggerAttributes = string.Empty;

            if (!string.IsNullOrEmpty(field.InsideTriggerFieldName))
            {
                insideTriggerAttributes = " hasInsideTrigger='hasInsideTrigger' triggerName='" + field.InsideTriggerFieldName + "' ";
            }

            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            selectList.Append( "<select " + GetDisabledHtmlAttribute(disabled) + " id='" + id + "' name='" + field.Name + "' viewName='" + field.View.Name + "' pk='" + pk + "' style='display:none' multiple='multiple' d_loaded='d_loaded' role='childrenCheckList'"+insideTriggerAttributes+" class='dropdownchecklist' d_minWidth='" + field.MinWidth.ToString() + "'>");

            selectList .Append( "<option value=''>(All)</option>");

                
            if (string.IsNullOrEmpty(field.InsideTriggerFieldName))
            {
                IEnumerable<SelectListItem> selectListItems = field.GetSelectList(pk, forFilter);                

                foreach (SelectListItem item in selectListItems)
                {
                    selectList .Append( "<option value='" + item.Value + "' " + (item.Selected ? "selected='selected'>" : ">").ToString() + item.Text + "</option>");
                }
            }
            selectList .Append( "</select>");

            if (field.InlineSearch)
            {
                AddInlineSearch(field, prefix, selectList, "CheckList", id, guid);
            }

            return selectList.ToString();

        }

        protected virtual string GetCheckListForCreate(ChildrenField field, string pk, string guid)
        {
            return GetCheckList(field, createPrefix, field.IsDisableForCreate(), pk, guid, false);

        }

        protected virtual string GetCheckListForEdit(ChildrenField field, string pk, string value, string guid)
        {
            return GetCheckList(field, editPrefix, field.IsDisableForEdit(guid), pk, guid, false);

        }

        protected virtual string GetCheckListForFilter(ChildrenField field, string prefix, bool disabled, string selectedValue, string guid)
        {
            string style = GetFilterStyle(field);
            StringBuilder selectList = new StringBuilder();
            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            bool load = !string.IsNullOrEmpty(selectedValue) || !string.IsNullOrEmpty(field.DefaultFilter);
            //Changed by br
            bool isGroupFilter = field.View != null && field.View.FilterType == FilterType.Group ? true : false;

            selectList.Append("<select " + style + " " + GetDisabledHtmlAttribute(disabled) + " id='" + id + "' name='" + field.Name + "' isGroupFilter='" + isGroupFilter + "' viewName='" + field.View.Name + "' role='childrenCheckListFilter' style='display:none' multiple='multiple' filter='filter' class='dropdownchecklist' onkeypress='handleEnterFilter(this, event, \"" + guid + "\")' >");
            selectList.Append("<option value=''>(All)</option>");

            if (load)
            {
                string[] selectedValues = field.GetFirstNonEquivalentParentField().SplitValues(selectedValue);

                bool empty = selectedValues.Contains(Database.EmptyString);
                if (prefix == filterPrefix)
                {
                    selectList.Append("<option value='" + Database.EmptyString + "' " + (empty ? "selected='selected'" : "").ToString() + ">" + field.View.Database.EmptyDisplay + "</option>");
                }

                IEnumerable<SelectListItem> selectListItems = field.GetSelectList(true);


                foreach (SelectListItem item in selectListItems)
                {
                    bool selected = selectedValues.Contains(item.Value);
                    selectList.Append("<option value='" + item.Value + "' " + (selected ? "selected='selected'>" : ">").ToString() + item.Text + "</option>");
                }
            }

            selectList.Append("</select>");

            if (!load)
            {
                //Changed by br
                string widthStyle = GetFilterWidthStyle(field);
                string displayStyle = "display: " + (isGroupFilter ? "inline-block" : "block") + ";"; 
                string onclicked = "";
                
                string onhovered = "";
                if (!disabled && string.IsNullOrEmpty(field.InsideTriggerFieldName))
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
                selectList.Append("<span d_ph='ph' class='ui-dropdownchecklist ui-dropdownchecklist-selector-wrapper ui-widget' style='cursor: default; overflow: hidden; " + displayStyle + widthStyle + "'><span tabindex='0'" + onclicked + " class='ui-dropdownchecklist-selector ui-state-default' style='overflow: hidden; white-space: nowrap; " + displayStyle + widthStyle + "'" + onhovered + ">" + icon + "<span style='display: inline-block; white-space: nowrap; overflow: hidden' class='ui-dropdownchecklist-text'></span></span></span>");
            }
            return selectList.ToString();

        }

        protected virtual string GetCheckListUrl(ChildrenField field, string guid, string url, string prefix)
        {
            string query = "?";

            query += "guid=" + guid + "&fieldName=" + field.Name + "&pk=$$&prefix=" + prefix;

            return url + query;
        }

        protected virtual string GetDiv(ChildrenField field, string prefix, string guid, string url, bool disabled)
        {
            string div = string.Empty;
            //string src = General.GetRootPath() + "Content/Images/Plus.jpg";
            string title = "";
            //string title = "Expand";
            if (disabled)
            {
                //src = General.GetRootPath() + "Content/Images/PlusDisabled.jpg";
                title = string.Format(field.SubgridInstructions, field.DisplayName);
            }

            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();

            string updateParent = " updateParent='" + field.UpdateParent.ToString() + "' ";
            string itemsFilter = string.Empty;
            if (!string.IsNullOrEmpty(field.DependencyFieldName)) 
            {
                itemsFilter = "itemsFilter='"+field.DependencyFieldName+"' ";
            }
            //string expand = "<input id='" + id + "' style='display:none' state='collapsedEmpty' type='image' class='inlineAddingImg expand' title='expand' alt='expand' src='" + src + "' onclick=\"expand(this, false,null,false,'" + guid + "');\" " + GetDisabledHtmlAttribute(disabled) + " title='" + title + "' alt='" + title + "'" + updateParent + "/>";
            string expand = "<span id='" + id + "' state='collapsedEmpty' class='inlineAddingImg expand' " + GetDisabledHtmlAttribute(disabled) + updateParent + ">" + title + "</span>";

            //string label = ((Durados.Web.Mvc.View)field.View).DataRowView == DataRowView.Divs ? string.Empty : field.DisplayName;
            div = "<div><div>" + expand + "</div><div url='" + url + "' class='childrenViewer' " + itemsFilter + " pk=''></div></div>";


            return div;
        }

        protected virtual string GetDivForTable(ChildrenField field, string prefix, string guid, string url, bool disabled, DataRow dataRow)
        {
            string div = string.Empty;
            string src = General.GetRootPath() + "Content/Images/Plus.gif";
            string title = "Expand";
            //if (disabled)
            //{
            //    src = General.GetRootPath() + "Content/Images/PlusDisabled.gif";
            //    title = "Please save first";
            //}

            disabled = !field.IsDerivationEditable(dataRow);
            string disabledStr = disabled ? "true" : "false";
            string updateParent = " updateParent='" + field.UpdateParent.ToString() + "' ";
            string itemsFilter = string.Empty;
            if (!string.IsNullOrEmpty(field.DependencyFieldName)) 
            {
                itemsFilter = "itemsFilter='"+field.DependencyFieldName+"' ";
            }
            string nocache = (field.NoCache) ? " nocache='nocache' " : string.Empty;
            string expand = "<img state='collapsedEmpty' class='inlineAddingImg expand' title='expand' alt='expand' src='" + src + "' onclick=\"expand(this, false, " + field.SubGridPlacement + "," + disabledStr + ",'" + guid + "');\" " + GetDisabledHtmlAttribute(false) + " title='" + title + "' alt='" + title + "' " + updateParent + nocache + " selectedClass='tablecommand' />&nbsp;";

            //string label = field.ConvertToString(dataRow);
            string label = "<a href='" + url + "'>" + field.ConvertToString(dataRow) + "</a>";
            div = "<div><div>" + expand + label + "</div><div url='" + url + "' class='childrenViewer' " + itemsFilter + " pk=''></div></div>";


            return div;
        }

        protected virtual string GetNavigationUrl(ChildrenField field, DataRow dataRow, string guid)
        {
            string url = GetUrlWithoutQuery(field);

            return GetNavigationUrl(field, dataRow, url, guid, false);
        }

        protected virtual string GetNavigationUrl(ChildrenField field, DataRow dataRow, string guid, bool isMainPage)
        {
            string url = GetUrlWithoutQuery(field);

            return GetNavigationUrl(field, dataRow, url, guid, isMainPage);
        }

        protected virtual string GetNavigationUrl(ChildrenField field, DataRow dataRow, string url, string guid, bool isMainPage)
        {
            string query = "?";

            for (int i = 0; i < field.DataRelation.ChildColumns.Count(); i++)
            {
                DataColumn childColumn = field.DataRelation.ChildColumns[i];
                DataColumn parentColumn = field.DataRelation.ParentColumns[i];
                string key = (dataRow == null) ? "$$" : dataRow[parentColumn.ColumnName].ToString();
                query += childColumn.ColumnName + "=" + System.Web.HttpContext.Current.Server.UrlEncode(key) + "&";
            }

            string display = (dataRow == null) ? "xx" : field.View.DisplayField.ConvertToString(dataRow);

            query += "__" + field.Name + "__=" + System.Web.HttpContext.Current.Server.UrlEncode(display);

            return url + query + (((View)field.ChildrenView).Check(TriggerDataAction.Open, dataRow) ? "&disabled=True" : "") + (isMainPage ? "&isMainPage=True" : "") + "&BackTo=" + System.Web.HttpContext.Current.Server.UrlEncode(field.View.Name);
 
        }

        protected virtual string GetUrlWithoutQuery(ChildrenField field)
        {
            View childrenView = ((View)field.ChildrenView);
            return GetUrlWithoutQuery(field, childrenView.IndexAction);
        }

        protected virtual string GetUrlWithoutQuery(ChildrenField field, string action)
        {
            return GetUrlWithoutQuery((View)field.ChildrenView, action);
        }
		// Private Methods (2) 

        private string GetDisabledHtmlAttribute()
        {
            return "disabled='disabled'";
        }

        private string GetDisabledHtmlAttribute(bool disabled)
        {
            return ((disabled) ? GetDisabledHtmlAttribute() : string.Empty).ToString();
        }

		#endregion Methods 
    }
}
