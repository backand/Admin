using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Stat.Measurements.Development
{
    public class TotalRows : DevMeasurement
    {
        public TotalRows(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }

        public override object Get(DateTime date)
        {
            var appRow = GetAppRow();
            int sqlProduct = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SqlProductId;
            long value = 0;
            SqlSchema schema = GetSchema((SqlProduct)sqlProduct);

            using (IDbConnection connection = GetConnection(appRow))
            {
                connection.Open();
                using (IDbCommand command = GetCommand(appRow))
                {
                    command.Connection = connection;
                    command.CommandText = GetTablesSql(schema);
                    List<string> tableNames = new List<string>();
                        
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                        reader.Close();
                    }

                    
                    foreach (string tableName in tableNames)
                    {
                        command.CommandText = GetTotalTablesRowsSql(schema, tableName);
                        object scalar = command.ExecuteScalar();
                        value = Calculation(value, Convert.ToInt64(scalar));
                    }
                }
                connection.Close();
                connection.Dispose();
            }

            return value;
        }

        protected virtual long Calculation(long current, long value)
        {
            return Sum(current, value);
        }

        private long Sum(long total, long value)
        {
            return total + value;
        }

        protected virtual string GetTablesSql(SqlSchema schema)
        {
            
            return schema.GetTableNamesSelectStatement();
        }

        protected virtual string GetTotalTablesRowsSql(SqlSchema schema, string tableName)
        {
            return schema.GetTableRowsCount(tableName);
        }
    }
}
