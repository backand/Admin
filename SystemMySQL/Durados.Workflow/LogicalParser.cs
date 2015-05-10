using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class LogicalParser
    {
        static DataTable table;

        static LogicalParser()
        {
            table = new DataTable("Table");
            DataColumn column = table.Columns.Add("Column", typeof(string));
            DataRow row = table.NewRow();
            row[column] = "value";
            table.Rows.Add(row);
        }

        public LogicalParser()
        {
        }

        public virtual bool Check(string expression)
        {
            DataView dataView = new DataView(table);

            try
            {
                dataView.RowFilter = expression;
            }
            catch
            {
                return false;
            }
            return dataView.Count > 0;
        }
    }
}
