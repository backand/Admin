using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class ChartInfo
    {
        public ChartInfo()
        {
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string X { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string Series { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string Ys { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string Types { get; set; }

        public Field GetXField(View view)
        {
            return view.Fields[X];
        }

        public Field[] GetYFields(View view)
        {
            return GetFields(view, Ys);
        }

        public Field[] GetSeriesFields(View view)
        {
            return GetFields(view, Series);
        }

        private Field[] GetFields(View view, string delimitedFieldsNames)
        {
            string[] fieldsNames = delimitedFieldsNames.Split(';');

            List<Field> fields = new List<Field>();

            foreach (string fieldName in fieldsNames)
            {
                fields.Add(view.Fields[fieldName]);
            }

            return fields.ToArray();
        }

        private ChartType[] GetChartTypes(View view)
        {
            string[] types = Types.Split(';');

            List<ChartType> chartTypes = new List<ChartType>();

            foreach (string type in types)
            {
                chartTypes.Add((ChartType)Enum.Parse(typeof(ChartType), type));
            }

            return chartTypes.ToArray();
        }
    }

    public enum ChartType
    {
        Line,
        Column,
        Bar,
        Pie,
        Gauge,
        Area,
        spline,
        scatter,
        bubble,
        Table
        

    }
}
