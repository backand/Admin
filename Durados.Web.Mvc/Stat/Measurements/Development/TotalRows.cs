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

        protected override string GetSql(SqlProduct sqlProduct)
        {
            SqlSchema schema = GetSchema(sqlProduct);
            return schema.GetTotalRowsCount();
        }
    }
}
