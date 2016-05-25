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

        

        protected virtual long Calculation(long current, long value)
        {
            return Max(current, value);
        }

        private long Max(long total, long value)
        {
            return Math.Max(total, value);
        }
    }
}
