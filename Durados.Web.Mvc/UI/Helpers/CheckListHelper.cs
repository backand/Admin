using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Durados.DataAccess;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class CheckListHelper
    {
		#region Properties (1) 

        private static Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

		#endregion Properties 

		#region Methods (10) 

		// Public Methods (10) 

        public static string GetListPKDelimitedByValuesForImport(this ChildrenField childrenField, List<string> displayValues, Importer importer)
        {
            View childrenView = (Durados.Web.Mvc.View)childrenField.ChildrenView;
            View view = (Durados.Web.Mvc.View)childrenField.View;
            View parentView = null;
            //ParentField parentField = null;

            string pks = "";
            string temp;
            string msg;

            GetPKValueByDisplayValueStatus status;

            ParentField parentField = null;

            foreach (ParentField field in childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (!field.ParentView.Base.Equals(view.Base))
                {
                    parentField = field;
                    parentView = (Durados.Web.Mvc.View)field.ParentView;
                    break;                    
                }
            }

            string displayName = childrenField.DisplayName;


            foreach (string displayValue in displayValues)
            {
                temp = parentView.GetPKValueByDisplayValue(displayValue.Trim(), out status);

                if (childrenField.Import && status == GetPKValueByDisplayValueStatus.NotFound)
                {
                    temp = importer.CreateParentRecord(parentField, displayValue);

                        if (temp != string.Empty)
                        {
                            status = GetPKValueByDisplayValueStatus.FoundUnique;
                        }
                }

                if (status == GetPKValueByDisplayValueStatus.FoundUnique)
                {
                    pks += temp + ",";
                }
                else
                {
                    if (status == GetPKValueByDisplayValueStatus.FoundMoreThanOne)
                        msg = "(SelectList) The display value [" + displayValue + "] is not a unique identifier of the parent record [" + displayName + "]";
                    else
                    {

                        msg = "(SelectList) Primary key for parent record [" + displayName + "] was not found by value [" + displayValue + "]";

                    }

                    throw new DuradosException(msg);

                }

            }

            return pks.Trim(',');

        }

        public static string[] GetSelectedChildrenPK(this ChildrenField childrenField, string fk)
        {
            childrenField = (ChildrenField)childrenField.Base;
            View childrenView = (Durados.Web.Mvc.View)childrenField.ChildrenView;
            View view = (Durados.Web.Mvc.View)childrenField.View;
            View parentView = null;
            ParentField parentField = null;
            ParentField fkField = null;

            var parentFields = childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent);

            foreach (ParentField field in childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (!field.ParentView.Equals(view))
                {
                    parentField = field;
                    parentView = (Durados.Web.Mvc.View)field.ParentView;
                }
                else
                {
                    fkField = field;
                }
            }

            if (parentView == null)
            {
                if (parentFields.Count() == 2)
                {
                    if (((ParentField)parentFields.FirstOrDefault()).ParentView.Equals(((ParentField)parentFields.LastOrDefault()).ParentView))
                    {
                        ParentField p1 = (ParentField)parentFields.FirstOrDefault();
                        ParentField p2 = (ParentField)parentFields.LastOrDefault();

                        if (!p1.DataRelation.ChildColumns[0].Equals(childrenField.DataRelation.ChildColumns[0]))
                        {
                            parentField = p1;
                            fkField = p2;
                        }
                        else
                        {
                            parentField = p2;
                            fkField = p1;
                        }
                        parentView = (View)parentField.ParentView;
                    }
                }
            }


            //int rowCount = 0;

            //List<string> keys = new List<string>();
            //if (!string.IsNullOrEmpty(fk))
            //{
            //    Dictionary<string, object> filter = new Dictionary<string, object>();
            //    filter.Add(fkField.Name, fk);

            //    DataView childrenDataView = childrenView.FillPage(1, 1000000, filter, false, null, out rowCount, null, null);
            //    if (childrenField.View.Database.DiagnosticsReportInProgress)
            //    {
            //        if (childrenDataView.Count > childrenField.View.Database.DiagnosticsReport.OverLoadLimit)
            //        {
            //            Map.Logger.Log(childrenView.Name, childrenField.Name, childrenField.View.Database.DiagnosticsReport.Name, string.Empty, childrenField.View.Database.DiagnosticsReport.GetStackTrace(), -childrenDataView.Count, string.Empty, childrenField.View.Database.DiagnosticsReport.DateTime);
            //        }
            //    }

            //    foreach (System.Data.DataRowView row in childrenDataView)
            //    {
            //        string key = parentField.GetValue(row.Row);
            //        keys.Add(key);
            //    }
            //}


            //return keys.ToArray();

            if (parentField == null)
            {
                throw new NoLongerChecklistException(childrenField);
            }

            return childrenView.GetKeys(parentField, fkField.Name, fk);
        }

        public static string GetSelectedChildrenPKDelimited(this ChildrenField childrenField, string fk)
        {
            string[] pks = GetSelectedChildrenPK(childrenField, fk);

            string delimited = string.Empty;

            foreach (string pk in pks)
            {
                delimited += pk + ",";
            }

            delimited = delimited.TrimEnd(',');

            return delimited;
        }

        public static IEnumerable<SelectListItem> GetSelectList(this ColumnField columnField)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            
            if (!string.IsNullOrEmpty(columnField.MultiValueAdditionals))
            {
                string[] additionals = columnField.MultiValueAdditionals.Split(',');
                if (additionals.Length % 2 == 0)
                {
                    for (int i = 0; i < additionals.Length / 2; i++)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = additionals[i * 2 + 1];
                        item.Value = additionals[i * 2];
                        item.Selected = false;

                        selectList.Add(item);
                    }
                }
            }

            int rowCount = 0;

            View parentView = columnField.GetParentView();

            if (parentView == null)
                return selectList;
            //View parentView = (View)Map.Database.Views[columnField.MultiValueParentViewName];

            string sortColumn = null;
            if (!string.IsNullOrEmpty(columnField.DropDownDisplayField) && parentView.Fields.ContainsKey(columnField.DropDownDisplayField))
            {
                sortColumn = parentView.Fields[columnField.DropDownDisplayField].Name;
            }
            else
            {
                sortColumn = parentView.GetDisplayColumn();
            }
            Dictionary<string, SortDirection> sort = new Dictionary<string, SortDirection>();

            if (sortColumn != null)
            {
                sort.Add(sortColumn, SortDirection.Asc);
            }

            DataView parentDataView = parentView.FillPage(1, 1000000, null, false, sort, out rowCount, null, null);

            HashSet<string> excludeValues = null;
            if (!string.IsNullOrEmpty(columnField.MultiValueExclude))
            {
                excludeValues = new HashSet<string>(columnField.MultiValueExclude.Split(','));
            }

            foreach (System.Data.DataRowView row in parentDataView)
            {
                string text = null;
                string value = null;

                if (!string.IsNullOrEmpty(columnField.DropDownDisplayField) && parentView.Fields.ContainsKey(columnField.DropDownDisplayField))
                {
                    text = parentView.Fields[columnField.DropDownDisplayField].ConvertToString(row.Row);
                }
                else
                {
                    text = parentView.GetDisplayValue(row.Row);
                }
                if (!string.IsNullOrEmpty(columnField.DropDownValueField) && parentView.Fields.ContainsKey(columnField.DropDownValueField))
                {
                    value = parentView.Fields[columnField.DropDownValueField].GetValue(row.Row).Trim('\'');
                }
                else
                {
                    value = parentView.GetPkValue(row.Row).Trim('\'');
                }
                if (excludeValues == null || !(excludeValues.Contains(value) || excludeValues.Contains(text)))
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = text;
                    item.Value = value;
                    item.Selected = false;

                    selectList.Add(item);
                }
            }


            return selectList;
        }

        public static IEnumerable<SelectListItem> GetSelectList(this ChildrenField childrenField, bool forFilter)
        {
            return GetSelectList(childrenField, null, forFilter);
        }

        //public static View GetOtherParentView(this ChildrenField childrenField, ParentField parentField, ParentField fkField)
        //{
        //    View childrenView = (Durados.Web.Mvc.View)childrenField.ChildrenView;
        //    View view = (Durados.Web.Mvc.View)childrenField.View;
        //    View parentView = null;
            
        //    foreach (ParentField field in childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
        //    {
        //        if (!field.ParentView.Base.Equals(view.Base))
        //        {
        //            parentField = field;
        //            parentView = (Durados.Web.Mvc.View)field.ParentView;
        //        }
        //        else
        //        {
        //            fkField = field;
        //        }
        //    }

        //    return parentView;
        //}
        public static IEnumerable<SelectListItem> GetSelectList(this ChildrenField childrenField, string fk, bool forFilter)
        {
                

            View childrenView = (Durados.Web.Mvc.View)childrenField.ChildrenView;
            View view = (Durados.Web.Mvc.View)childrenField.View;
            //ParentField parentField = null;
            //ParentField fkField = null;
            //View parentView = null;

            //foreach (ParentField field in childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            //{
            //    if (!field.ParentView.Base.Equals(view.Base))
            //    {
            //        parentField = field;
            //        parentView = (Durados.Web.Mvc.View)field.ParentView;
            //    }
            //    else
            //    {
            //        fkField = field;
            //    }
            //}
            Durados.ParentField parentField = null;
            Durados.ParentField fkField = null;
            View parentView = (View)childrenField.GetOtherParentView(out parentField, out fkField);
            
            Dictionary<string, string> selectOptions = parentField.GetSelectOptions(fk, null, forFilter);

            //int rowCount = 0;
            //DataView parentDataView = parentView.FillPage(1, 1000000, null, false, null, out rowCount, null, null);

            //if (childrenField.View.Database.DiagnosticsReportInProgress)
            //{
            //    if (parentDataView.Count > childrenField.View.Database.DiagnosticsReport.OverLoadLimit)
            //    {
            //        Map.Logger.Log(parentView.Name, childrenField.Name, childrenField.View.Database.DiagnosticsReport.Name, string.Empty, childrenField.View.Database.DiagnosticsReport.GetStackTrace(), -parentDataView.Count, string.Empty, childrenField.View.Database.DiagnosticsReport.DateTime);
            //    }
            //}

            //Dictionary<object, object> keys = new Dictionary<object, object>();
            //if (!string.IsNullOrEmpty(fk))
            //{
            //    Dictionary<string, object> filter = new Dictionary<string, object>();
            //    filter.Add(fkField.Name, fk);

            //    DataView childrenDataView = childrenView.FillPage(1, 1000000, filter, false, null, out rowCount, null, null);

            //    if (childrenField.View.Database.DiagnosticsReportInProgress)
            //    {
            //        if (parentDataView.Count > childrenField.View.Database.DiagnosticsReport.OverLoadLimit)
            //        {
            //            Map.Logger.Log(childrenField.View.Name, childrenField.Name, childrenField.View.Database.DiagnosticsReport.Name, string.Empty, childrenField.View.Database.DiagnosticsReport.GetStackTrace(), -parentDataView.Count, string.Empty, childrenField.View.Database.DiagnosticsReport.DateTime);
            //        }
            //    }
                
            //    foreach (System.Data.DataRowView row in childrenDataView)
            //    {
            //        string key = parentField.GetValue(row.Row);
            //        keys.Add(key, key);
            //    }
            //}
            
            List<SelectListItem> selectList = new List<SelectListItem>();

            //foreach (System.Data.DataRowView row in parentDataView)
            //{
            //    string text = parentView.GetDisplayValue(row.Row);
            //    string value = parentView.GetPkValue(row.Row);
            //    bool selected = keys.ContainsKey(value);

            //    SelectListItem item = new SelectListItem();
            //    item.Text = text;
            //    item.Value = value;
            //    item.Selected = selected;

            //    selectList.Add(item);
            //}

            foreach (string key in selectOptions.Keys)
            {
                SelectListItem item = new SelectListItem();
                item.Text = selectOptions[key];
                item.Value = key;
                item.Selected = false;

                selectList.Add(item);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> GetSelectList(this ChildrenField childrenField, string fk, string pk)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            
            View childrenView = (Durados.Web.Mvc.View)childrenField.ChildrenView;


            View view = (Durados.Web.Mvc.View)childrenField.View;

            ParentField fkField = null;

            ParentField parentField = null;

            View parentView = null;


            foreach (ParentField field in childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (!field.ParentView.Base.Equals(view.Base))
                {
                    parentField = field;
                    parentView = (Durados.Web.Mvc.View)field.ParentView;
                }
                else
                {
                    fkField = field;
                }
            }

            if (parentField == null) return selectList;
            
            Dictionary<string, object> filterParent = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(fk) && !string.IsNullOrEmpty(childrenField.DependencyFieldName))
            {
                filterParent.Add(childrenField.DependencyFieldName, fk);
            }

            int rowCount = 0;
            DataView parentDataView = parentView.FillPage(1, 1000000, filterParent, false, null, out rowCount, null, null);

            if (childrenField.View.Database.DiagnosticsReportInProgress)
            {
                if (parentDataView.Count > childrenField.View.Database.DiagnosticsReport.OverLoadLimit)
                {
                    Map.Logger.Log(parentView.Name, childrenField.Name, childrenField.View.Database.DiagnosticsReport.Name, string.Empty, childrenField.View.Database.DiagnosticsReport.GetStackTrace(), -parentDataView.Count, string.Empty, childrenField.View.Database.DiagnosticsReport.DateTime);
                }
            }
            //Selected values = rows in many-to-many view
            Dictionary<object, object> keys = new Dictionary<object, object>();

            Dictionary<string, object> filter = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(pk) && fkField != null)
            {
                filter.Add(fkField.Name, pk);

                DataView childrenDataView = childrenView.FillPage(1, 1000000, filter, false, null, out rowCount, null, null);

                if (childrenField.View.Database.DiagnosticsReportInProgress)
                {
                    if (parentDataView.Count > childrenField.View.Database.DiagnosticsReport.OverLoadLimit)
                    {
                        Map.Logger.Log(childrenView.Name, childrenField.Name, childrenField.View.Database.DiagnosticsReport.Name, string.Empty, childrenField.View.Database.DiagnosticsReport.GetStackTrace(), -parentDataView.Count, string.Empty, childrenField.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                foreach (System.Data.DataRowView row in childrenDataView)
                {
                    string key = parentField.GetValue(row.Row);
                    if (!keys.ContainsKey(key))
                        keys.Add(key, key);
                }
            }

            //Create Options
            foreach (System.Data.DataRowView row in parentDataView)
            {
                string text = parentView.GetDisplayValue(row.Row);
                string value = parentView.GetPkValue(row.Row);
                bool selected = keys.ContainsKey(value);

                SelectListItem item = new SelectListItem();
                item.Text = text;
                item.Value = value;
                item.Selected = selected;

                selectList.Add(item);
            }


            return selectList;
        }

        /// <summary>
        /// Get select options for filter field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fk"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetSelectOptions(this Field field, string fk, int? top, bool forFilter)
        {
            ParentField parentField = field.GetParentField();
            Durados.View view = null;

            if (field != null && field.FieldType == FieldType.Children)
            {
                view = field.View;
            }
            else if (parentField != null)
            {
                view = parentField.View;
            }

            return GetSelectOptions(parentField, view as Durados.Web.Mvc.View, fk, top, forFilter);
        }

        /// <summary>
        /// Get select options for filter field
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetSelectOptions(string viewName, string fieldName, string fk, bool forFilter)
        {
            ParentField field = FieldHelper.GetParentField(viewName, fieldName);

            return GetSelectOptions(field, fk, null, forFilter);
        }

        /// <summary>
        /// Get select options for filter field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fk"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetSelectOptions(this ParentField field, View view, string fk, int? top, bool forFilter)
        {
            Dictionary<string, string> selectOptions = null;
            Dictionary<string, object> values = new Dictionary<string, object>();

            if (field != null)
            {
                if (forFilter)
                {
                    FormCollection collection = null;
                    Json.Filter filter = null;

                    //Init collection
                    collection = new FormCollection(ViewHelper.GetSessionState(view.Name + "Filter"));
                    if (collection != null && collection["jsonFilter"] != null)
                    {
                        try
                        {
                            filter = Json.JsonSerializer.Deserialize<Json.Filter>(collection["jsonFilter"]);
                        }
                        catch (Exception ex)
                        {
                            Map.Logger.Log(string.Empty, string.Empty, "GetSelectOptions", ex, 3, string.Empty);
                        }
                    }

                    //Init json filter
                    if (filter != null)
                    {
                        foreach (Json.Field jsonField in filter.Fields.Values)
                        {
                            if (jsonField.Value != null && !string.IsNullOrEmpty(jsonField.Value.ToString()))
                            {
                                values.Add(jsonField.Name, jsonField.Value);
                            }
                        }
                    }
                }

                //Init useUniqueName
                bool useUniqueName = field.ParentHtmlControlType == ParentHtmlControlType.InsideDependencyUniqueNames;

                //Extract selectOptions
                if (string.IsNullOrEmpty(fk))
                {
                    selectOptions = field.GetSelectOptions(view, forFilter, useUniqueName, top, values);
                }
                else
                {
                    selectOptions = field.GetSelectOptions(view, fk, top, values, forFilter);
                }
            }
            else
            {
                selectOptions = new Dictionary<string, string>();
            }

            return selectOptions;
        }

		#endregion Methods 
    }
}
