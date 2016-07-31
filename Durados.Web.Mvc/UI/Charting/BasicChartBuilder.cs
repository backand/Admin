using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.UI.Charting
{
    public class BasicChartBuilder
    {
        public virtual Json.Chart GetChart(View view, int top, FormCollection collection, string search, string SortColumn, SortDirection sortDirection, out int rowCount, out Durados.Web.Mvc.UI.Json.Filter filter, string guid, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            rowCount = 0;
            filter = null;
            DataView dataView = view.ApplyFilter(0, top, collection, !string.IsNullOrEmpty(search), SortColumn, sortDirection, out rowCount, out filter, guid, beforeSelectCallback, afterSelectCallback);

            return GetChart(view, dataView);
        }

        public virtual Json.Chart GetChart(View view, DataView dataView)
        {
            Field xField = view.ChartInfo.GetXField(view);
            Field[] yFields = view.ChartInfo.GetYFields(view);

            Json.Chart chart = GetNewChart(xField);
            
            chart.xTitle = xField.DisplayName;
            chart.yTitle = view.DisplayName;

            foreach (Field yField in yFields)
            {
                Json.Chart.Series series = chart.AddSeries(yField.DisplayName);
            }

            if (!xField.IsNumeric)
            {
                foreach (System.Data.DataRowView row in dataView)
                {
                    string x = xField.ConvertToString(row.Row);
                    ((Json.Chart1D)chart).xAxis.Add(x);
                }
            }

            foreach (System.Data.DataRowView row in dataView)
            {
                for (int i = 0; i < yFields.Length; i++ )
                {
                    Field yField = yFields[i];
                    Json.Chart.Series series = chart.Serieses[i];

                    double y = Convert.ToDouble(yField.GetValue(row.Row));
                    if (xField.IsNumeric)
                    {
                        double x = Convert.ToDouble(xField.GetValue(row.Row));
                        ((Json.Chart2D.Series2D)series).AddPoint(x, y);
                    }
                    else
                    {
                        ((Json.Chart1D.Series1D)series).AddPoint(y);
                    }
                }
            }

            return chart;
        }

        protected virtual Json.Chart GetNewChart(Field xField)
        {
            if (xField.IsNumeric)
                return new Durados.Web.Mvc.UI.Json.Chart2D();
            else
                return new Durados.Web.Mvc.UI.Json.Chart1D();
        }
    }
}
