using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.Web.Mvc;

namespace Durados.Web.Mvc.UI
{
    public class Styler
    {
        protected DataView dataView;
        protected View view;

        public Styler(View view, DataView dataView)
        {
            this.view = view;
            this.dataView = dataView;
        }

        public virtual string GetRowCss(View view, DataRow row, int rowIndex)
        {
            if (view.HasRowColor)
            {
                string ClassName = view.Fields[view.RowColorColumnName].GetValue(row);
                if (!string.IsNullOrEmpty(ClassName))
                    return ClassName;
            }

            if (rowIndex % 2 == 0)
                return view.ContainerGraphicProperties + " d_fix_row";
            else
                return "d_alt_row"; 

        }

        public virtual string GetCellCss(Field field, DataRow row, string guid)
        {
            string starter = "=";

            if (IsNaDate(field, row))
            {
                return "nadateTD";
            }

            if (string.IsNullOrEmpty(field.ContainerGraphicField))
            {
                return field.ContainerGraphicProperties;
            }
            else
            {
                if (field.View.Fields.ContainsKey(field.ContainerGraphicField))
                {
                    Field containerGraphicField = field.View.Fields[field.ContainerGraphicField];
                    if (containerGraphicField.FieldType == FieldType.Column || containerGraphicField.Equals(field))
                    {
                        return field.ContainerGraphicProperties + " " + containerGraphicField.GetValue(row);
                    }
                    else
                    {
                        return field.ContainerGraphicProperties + " " + containerGraphicField.Name + containerGraphicField.GetValue(row);
                    }
                }
                else if (field.ContainerGraphicField.Trim().StartsWith(starter))
                {
                    return GetCellCss(field, row, field.ContainerGraphicField, starter[0]);
                }
                else
                {
                    return field.ContainerGraphicProperties;
                }
            }
        }

        private bool IsNaDate(Field field, DataRow row)
        {
            if (field.FieldType == FieldType.Column && ((ColumnField)field).DataColumn.DataType == typeof(DateTime))
            {
                string dateValue= field.GetValue(row);
                if (dateValue != null && dateValue.Equals(((ColumnField)field).ConvertDateToString(Durados.Database.NaDate)))
                    return true;
            }
            return false;
        }

        public virtual string GetCellCss(Field field, DataRow row, string delimitedValues, char starter)
        {
            return GetCellCss(field, row, GetStyleOptions(delimitedValues, '|', starter));
        }

    
        protected virtual Dictionary<string, string> GetStyleOptions(string delimitedValues, char delimiter, char starter)
        {
            Dictionary<string, string> styleOptions = new Dictionary<string, string>();

            delimitedValues = delimitedValues.Trim().TrimStart(starter).Trim();

            string[] values = delimitedValues.Split(delimiter);
            if (values.Length % 2 == 0)
            {
                for (int i = 0; i < values.Length / 2; i++)
                {
                    string value = values[i * 2 + 1];
                    string key = values[i * 2];

                    styleOptions.Add(key, value);
                }
            }

            return styleOptions;
        }

        public virtual string GetCellCss(Field field, DataRow row, Dictionary<string, string> styleOptions)
        {
            string value = field.GetValue(row);
            if (!string.IsNullOrEmpty(value))
            {
                if (styleOptions.ContainsKey(value))
                    return styleOptions[value];
                else
                    return field.ContainerGraphicProperties;
            }
            return field.ContainerGraphicProperties; ;
        }

        public virtual string GetAlt(Field field, DataRow row, string guid)
        {
            return string.Empty;
        }
    }
}
