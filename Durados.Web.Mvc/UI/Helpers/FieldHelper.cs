using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.DataAccess;
using System.Web;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class FieldHelper
    {
        #region Properties (1)

        public static Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        #endregion Properties

        #region Methods (30)

        // Public Methods (30) 

        public static string GetDataActionPrefix(this Field field, DataAction dataAction)
        {
            return FieldExtentions.GetDataActionPrefix(field, dataAction);
        }

        public static string GetDefaultFilterWithSql(this Field field)
        {
            if (field.DefaultFilter.StartsWith("[") && field.DefaultFilter.EndsWith("]"))
            {
                DataAccess.SqlAccess dataAccess = new Durados.DataAccess.SqlAccess();
                return dataAccess.ExecuteScalar(((Database)field.View.Database).ConnectionString, field.DefaultFilter.TrimStart('[').TrimEnd(']'));
            }
            else
            {
                var type = field.GetColumnFieldType();
                if (type == ColumnFieldType.DateTime || field.IsNumeric)
                {
                    return ColumnFieldViewer.WrapToken(field.DefaultFilter, Filter.TOKEN, Filter.TOKEN);
                }
                else
                {
                    return field.DefaultFilter;
                }
            }

        }

        public static string GetElementForCreate(this Field field, string guid)
        {
            return FieldExtentions.GetElementForCreate(field, guid);
        }

        public static string GetElementForCreate(this Field field, string pk, string value, string guid)
        {
            return FieldExtentions.GetElementForCreate(field, pk, value, guid);
        }

        public static string GetElementForEdit(this Field field, string guid)
        {
            return FieldExtentions.GetElementForEdit(field, guid);
        }

        public static string GetElementForEdit(this Field field, DataRow dataRow, string guid)
        {
            return FieldExtentions.GetElementForEdit(field, dataRow, guid);
        }

        public static string GetElementForEdit(this Field field, string pk, string value, string guid)
        {
            return FieldExtentions.GetElementForEdit(field, pk, value, guid);
        }

        public static string GetElementForFilter(this Field field, object value, string guid)
        {
            return FieldExtentions.GetElementForFilter(field, value, guid);
        }

        public static string GetElementForRowView(this Field field, DataAction dataAction, string guid)
        {
            switch (dataAction)
            {
                case DataAction.Create:
                    return FieldExtentions.GetElementForCreate(field, guid);
                case DataAction.Report:
                    return FieldExtentions.GetElementForReport(field, guid);
                case DataAction.Edit:
                    return FieldExtentions.GetElementForEdit(field, guid);
                case DataAction.InlineAdding:
                    return FieldExtentions.GetElementForInlineAdding(field, guid);
                case DataAction.InlineEditing:
                    return FieldExtentions.GetElementForInlineEditing(field, guid);
                default:
                    throw new NotSupportedException();
            }
        }

        public static string GetElementForRowView(this Field field, DataAction dataAction, object data, string guid)
        {
            switch (dataAction)
            {
                case DataAction.Create:
                    string pk = string.Empty;
                    if (data != null && data is string)
                        pk = (string)data;
                    return FieldExtentions.GetElementForCreate(field, pk, string.Empty, guid);
                case DataAction.Edit:
                    if (data != null && data is DataRow)
                    {
                        DataRow dataRow = (DataRow)data;

                        return FieldExtentions.GetElementForEdit(field, dataRow, guid);
                    }
                    else
                    {
                        return FieldExtentions.GetElementForEdit(field, string.Empty, string.Empty, guid);
                    }
                case DataAction.InlineAdding:
                    return FieldExtentions.GetElementForInlineAdding(field, guid);
                default:
                    throw new NotSupportedException();
            }
        }

        public static string GetElementForRowView(this Field field, DataAction dataAction, string pk, string value, string guid)
        {
            switch (dataAction)
            {
                case DataAction.Create:
                    return FieldExtentions.GetElementForCreate(field, pk, value, guid);
                case DataAction.Edit:
                    return FieldExtentions.GetElementForEdit(field, pk, value, guid);
                case DataAction.InlineAdding:
                    return FieldExtentions.GetElementForInlineAdding(field, guid);
                default:
                    throw new NotSupportedException();
            }
        }

        public static HtmlControlType GetElementForTableView(this Field field, string guid)
        {
            return FieldExtentions.GetHtmlControlType(field);
        }

        public static string GetElementForTableView(this Field field, DataRow dataRow, string guid)
        {
            return FieldExtentions.GetElementForTableView(field, dataRow, guid);
        }

        public static string GetLocalizedDisplayName(this Field field)
        {
            if (field.DoLocalize())
            {
                return Map.Database.Localizer.Translate(field.DisplayName);
            }
            else
            {
                return field.DisplayName;
            }
        }

        public static string GetLocalizedDisplayValue(this ParentField field, string value)
        {
            string displayValue = field.GetDisplayValue(value);

            if (field.DoLocalize())
            {
                return Map.Database.Localizer.Translate(displayValue);
            }
            else
                return displayValue;
        }

        // <summary>
        /// Try get parent field of a field
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static ParentField GetParentField(this Field field)
        {
            ParentField parentField = null;

            if (field.FieldType == FieldType.Children)
            {
                ChildrenField childrenField = field as ChildrenField;
                if (childrenField != null)
                {
                    parentField = childrenField.GetFirstNonEquivalentParentField() as ParentField;
                }
            }
            else
            {
                parentField = field as ParentField;
            }

            return parentField;
        }

        /// <summary>
        /// Try get parent field by viewName and fieldName
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static ParentField GetParentField(string viewName, string fieldName)
        {
            Durados.Web.Mvc.View view = ViewHelper.GetView(viewName);
            ParentField field = null;

            if (view.Fields.ContainsKey(fieldName))
            {
                if (view.Fields[fieldName].FieldType == FieldType.Parent)
                {
                    field = (ParentField)view.Fields[fieldName];
                }
                else if (view.Fields[fieldName].FieldType == FieldType.Children)
                {
                    field = (ParentField)((ChildrenField)view.Fields[fieldName]).GetFirstNonEquivalentParentField();
                }
            }

            return field;
        }

        public static string GetTooltip(this Field field)
        {
            return ViewHelper.GetTooltip(field.DisplayName, field.Description);
        }

        //public static string GetCheckListForFilter(this ParentField field, string guid)
        //{
        //    return (new ParentFieldViewer()).GetCheckListForFilter(field, guid);
        //}
        public static string GetUrlData(string display, string url)
        {
            return display + "|_blank|" + url;
        }

        public static string GetValidationElements(this Field field, DataAction dataAction, string guid)
        {
            return FieldExtentions.GetValidationElements(field, dataAction, guid);
        }

        public static string GetValidationType(this Field field)
        {
            return FieldExtentions.GetValidationType(field);
        }

        public static bool IsDisableForCreate(this Field field)
        {
            return FieldExtentions.IsDisableForCreate(field);
        }

        public static bool IsDisableForEdit(this Field field, string guid)
        {
            return FieldExtentions.IsDisableForEdit(field, guid);
        }

        public static bool IsHidden(this Field field)
        {
            return FieldExtentions.GetHtmlControlType(field) == HtmlControlType.Hidden;
        }

        public static string GetUploadPath(this Durados.Web.Mvc.ColumnField field)
        {
            string uploadPath=string.Empty;
            if (field.IsFtpUpload)
            {
                string fileName = string.Empty;
                uploadPath= field.FtpUpload.GetFtpBasePath(fileName);
            }
            else if (field.IsUpload)
            {
                if (string.IsNullOrEmpty(field.Upload.UploadPhysicalPath))
                {
                    uploadPath = field.Upload.UploadVirtualPath.Replace("/", "\\");

                    if (!field.Upload.UploadVirtualPath.StartsWith(@"~\"))
                    {
                        if (!uploadPath.StartsWith(@"\"))
                        {
                            uploadPath = @"\" + uploadPath;
                        }
                        uploadPath = "~" + uploadPath;
                    }
                    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);
                }
                else
                {
                    uploadPath = field.Upload.UploadPhysicalPath;
                }

                if (!uploadPath.EndsWith(@"\"))
                    uploadPath = uploadPath + @"\";

                
            }
            return uploadPath;
        }

        public static bool IsVisibleForCreate(this Field field)
        {
            return FieldExtentions.IsVisibleForCreate(field);
        }

        public static bool IsVisibleForEdit(this Field field)
        {
            return FieldExtentions.IsVisibleForEdit(field);
        }

        public static bool IsVisibleForFilter(this Field field)
        {
            return FieldExtentions.IsVisibleForFilter(field);
        }

        public static bool IsVisibleForSort(this Field field)
        {
            return FieldExtentions.IsVisibleForSort(field);
        }

        public static bool IsVisibleForRow(this Field field, DataAction dataAction)
        {
            switch (dataAction)
            {
                case DataAction.Create:
                    return FieldExtentions.IsVisibleForCreate(field);
                case DataAction.Report:
                    return FieldExtentions.IsVisibleForReport(field);
                case DataAction.Edit:
                    return FieldExtentions.IsVisibleForEdit(field);
                case DataAction.InlineAdding:
                    return FieldExtentions.IsVisibleForCreate(field);
                case DataAction.InlineEditing:
                    return FieldExtentions.IsVisibleForEdit(field);
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool IsVisibleForTable(this Field field)
        {
            return FieldExtentions.IsVisibleForTable(field);
        }

        /// <summary>
        /// Indication if filter label should be displayed for this field
        /// </summary>
        /// <returns></returns>
        public static bool NeedDisplayFilterLabel(this Field field)
        {
            bool needDisplayFilterLabel = false;

            if (field.View != null && field.View.FilterType == FilterType.Group)
            {
                if ((field.GroupFilterDisplayLabel == GroupFilterDisplayLabel.Inherit && field.View.GroupFilterDisplayLabel)
                || field.GroupFilterDisplayLabel == GroupFilterDisplayLabel.Display)
                {
                    needDisplayFilterLabel = true;
                }
            }

            return needDisplayFilterLabel;
        }


        

        #endregion Methods
    }
}
