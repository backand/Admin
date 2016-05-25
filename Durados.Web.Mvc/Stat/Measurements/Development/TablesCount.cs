using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Stat.Measurements.Development
{
    public class TablesCount : DevMeasurement
    {
        public TablesCount(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }

        protected override string GetSql(SqlProduct sqlProduct)
        {
            SqlSchema schema = GetSchema(sqlProduct);

            return schema.CountTablesSelectStatement();
        }

        
    }
}
