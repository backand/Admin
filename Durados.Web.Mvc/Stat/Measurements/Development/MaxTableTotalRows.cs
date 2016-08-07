using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Stat.Measurements.Development
{
    public class MaxTableTotalRows : TotalRows
    {
        public MaxTableTotalRows(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }



        protected override string GetSql(SqlProduct sqlProduct)
        {
            SqlSchema schema = GetSchema(sqlProduct);
            return schema.GetMaxTableRowsCount();
        }
    }
}
