using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Durados
{
    public class MyCharts
    {
        public Database Database { get; set; } 

        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; set; }

        public Dictionary<int, Chart> _charts;
        [Durados.Config.Attributes.ChildrenProperty(TableName = "Chart", Type = typeof(Chart), DictionaryKeyColumnName = "Id")]
        public Dictionary<int, Chart> Charts { 
        
            get{
              foreach (Chart chart in _charts.Values)
                        chart.Database = Database;
                //}
                //isPagesInitialized = true;
                return _charts;
            

            }
            private set { _charts = value; }
        }

        public MyCharts()
        {
            Charts = new Dictionary<int, Chart>();

            //string sql = "SELECT top(20) Controller, COUNT(*) as [Count]  FROM [Durados_Log]  where logtype<=3   group by Controller Order By [Count] desc ";
            //Charts.Add(1, new Chart() { ID = 1, ChartType = ChartType.Pie, Name = "Activity", SubTitle = "Example", SQL = sql, XField = "Controller", YField = "Count", XTitle = "Durados Views", YTitle = "Count Activity", Height = 340, Align = ChartAlignment.Left, Ordinal = 1,Database = Database  });
            //Charts.Add(2, new Chart() { ID = 2, ChartType = ChartType.Line, Name = "Activity", SubTitle = "Example", SQL = sql, XField = "Controller", YField = "Count", XTitle = "Durados Views", YTitle = "Count Activity", Height = 340, Align = ChartAlignment.Left, Ordinal = 2, Database = Database });
            //Charts.Add(3, new Chart() { ID = 3, ChartType = ChartType.Bar, Name = "Activity", SubTitle = "Example", SQL = sql, XField = "Controller", YField = "Count", XTitle = "Durados Views", YTitle = "Count Activity", Height = 340, Align = ChartAlignment.Right, Ordinal = 1, Database = Database });
            //Charts.Add(4, new Chart() { ID = 4, Height = 340, Align = ChartAlignment.Right, Ordinal = 2 });

            //Charts.Add(3, new Chart() { ID = 1,  Database = Database });
            //Charts.Add(4, new Chart() { ID = 2, Database = Database });
            Name = "My Charts";
            //Chart2 = new Chart() { Id = "Chart2" };
            //Chart3 = new Chart() { Id = "Chart3" };
            //Chart4 = new Chart() { Id = "Chart4" };
        }

        public MyCharts(Database database):this()
        {
            // TODO: Complete member initialization
            this.Database = database;
        }

        //[Durados.Config.Attributes.ParentProperty(TableName = "Chart")]
        //public Chart Chart1 { get; set; }

        //[Durados.Config.Attributes.ParentProperty(TableName = "Chart")]
        //public Chart Chart2 { get; set; }

        //[Durados.Config.Attributes.ParentProperty(TableName = "Chart")]
        //public Chart Chart3 { get; set; }

        //[Durados.Config.Attributes.ParentProperty(TableName = "Chart")]
        //public Chart Chart4 { get; set; }
        private int columns;
        [Durados.Config.Attributes.ColumnProperty()]
        public int Columns { get {return columns == 0 ? 2 : columns; } set { columns = value; } }

    }
}
