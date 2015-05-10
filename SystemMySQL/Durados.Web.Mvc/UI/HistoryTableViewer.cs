using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Controllers.Filters;


namespace Durados.Web.Mvc.UI
{
    public class HistoryTableViewer : TableViewer
    {
        private Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        Dictionary<string, string> displayValues = null;
        string where = string.Empty;

        public HistoryTableViewer()
            : base()
        {
        }

        public override System.Data.DataView GetDataView(System.Data.DataView dataView, View view, string guid)
        {
            DataTable table = dataView.Table;

            LoadTable(table, view, guid);

            return dataView;
        }

        protected virtual void LoadTable(DataTable table, View view, string guid)
        {
            if (table.Rows.Count == 0)
                return;
            
            Field viewNameField = view.Fields["ViewName"];
            if (viewNameField == null)
                return;
            string realViewName = viewNameField.GetValue(table.Rows[0]);
            if (string.IsNullOrEmpty(realViewName))
                return;

            View realView = null;

            if (Map.Database.Views.ContainsKey(realViewName))
                realView = (View)view.Database.Views[realViewName];
            else if (Map.GetConfigDatabase().Views.ContainsKey(realViewName))
                realView = (View)Map.GetConfigDatabase().Views[realViewName];
            else
                return;
            if (realViewName == "durados_Link")
                return;

            where = GetWhere(table, view);
            int rowCount = 0;
            DataView realDataView = realView.FillPage(1, 100000, new Dictionary<string, object>(), null, null, out rowCount, view_BeforeSelect, view_AfterSelect);

            FillDisplayValues(realDataView, realView);
        }

        private void FillDisplayValues(DataView realDataView, View realView)
        {
            foreach (DataRow row in realDataView.Table.Rows)
            {
                string pk = realView.GetPkValue(row);
                string displayValue = realView.GetDisplayValue(row);

                if (displayValues.ContainsKey(pk))
                {
                    displayValues[pk] = displayValue;
                }
                else
                {
                    displayValues.Add(pk, displayValue);
                }
            }
        }

        private string GetWhere(DataTable table, View view)
        {
            displayValues = new Dictionary<string, string>();
            Field pkField = view.Fields["PK"];

            foreach (DataRow row in table.Rows)
            {
                string pk = pkField.GetValue(row);

                if (!displayValues.ContainsKey(pk))
                {
                    displayValues.Add(pk, string.Empty);
                }
            }

            return displayValues.Keys.ToArray().Delimited();
        }

        protected void view_BeforeSelect(object sender, SelectEventArgs e)
        {
            SetPermanentFilter((Durados.Web.Mvc.View)e.View, (Filter)e.Filter);
        }

        protected void view_AfterSelect(object sender, SelectEventArgs e)
        {
        }

        protected void SetPermanentFilter(Durados.Web.Mvc.View view, Durados.DataAccess.Filter filter)
        {
            if (view.Name == "durados_v_ChangeHistory")
            {
                filter.WhereStatement += " and PK in (" + where + ")";
            }
            
        }

        public override string GetElementForTableView(Field field, DataRow row, string guid)
        {
            if (field.Name == "ViewName")
            {
                return field.View.Database.Views[field.GetValue(row)].DisplayName;
            }
            else if (field.Name == "FieldName")
            {
                string viewName = field.View.Database.Views["durados_v_ChangeHistory"].Fields["ViewName"].GetValue(row);
                string fieldName = field.GetValue(row);
                if (field.View.Database.Views.ContainsKey(viewName))
                {
                    if (!string.IsNullOrEmpty(fieldName) && field.View.Database.Views[viewName].Fields.ContainsKey(fieldName))
                    {
                        return field.View.Database.Views[viewName].Fields[fieldName].DisplayName;
                    }
                    else
                    {
                        return fieldName;
                    }
                }
                else
                {
                    return row["ColumnNames"] + ": " + fieldName;
                }
            }
            else if (field.Name == "PK")
            {
                if (displayValues == null)
                {
                    return row["ViewName"].ToString();
                }
                string value = field.GetValue(row);
                if (value == null)
                    return row["ViewName"].ToString();
                if (displayValues.ContainsKey(value))
                {
                    string s = displayValues[value];
                    if (s != string.Empty)
                        return s;
                    else
                        return row["ViewName"].ToString();
                }
                else
                    return row["ViewName"].ToString();
                
            }
            else
            {
                return field.GetElementForTableView(row, guid);
            }
        }
    }
}
