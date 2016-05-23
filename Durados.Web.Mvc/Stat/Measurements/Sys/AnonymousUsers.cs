using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Stat.Measurements.Sys
{
    public class AnonymousUsers : SysMeasurement
    {
        public AnonymousUsers(MeasurementType measurementType)
            : base(measurementType)
        {

        }
        protected override string GetSql(SqlProduct sqlProduct)
        {
            return "";
        }
    }
}
