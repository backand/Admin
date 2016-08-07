using Durados.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Stat.Measurements.Development
{
    public class DbSize : DevMeasurement
    {
        public DbSize(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }

        protected override string GetSql(SqlProduct sqlProduct)
        {
            string sql = null;

            switch (sqlProduct)
            {
                case SqlProduct.MySql:
                    sql = "SELECT Sum(data_length + index_length) FROM   information_schema.tables GROUP  BY table_schema; ";
                    break;
                case SqlProduct.Postgre:
                    sql = "SELECT pg_database_size(current_DATABASE());";
                    break;
                case SqlProduct.Oracle:
                    sql = "";
                    break;

                default:
                    sql = "SELECT row_size_mb = CAST(SUM(CASE WHEN type_desc = 'ROWS' THEN size END) * 8. * 1024 AS bigint) FROM sys.master_files WITH(NOWAIT) WHERE database_id = DB_ID() GROUP BY database_id";
                    break;
            }

            return sql;
        }
    }
}
