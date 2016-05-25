using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Stat.Measurements.Sys
{
    public class AnonymousUsers : SysMeasurement
    {
        public AnonymousUsers(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }
        protected override string GetSql(SqlProduct sqlProduct)
        {
            return "";
        }
    }
}
