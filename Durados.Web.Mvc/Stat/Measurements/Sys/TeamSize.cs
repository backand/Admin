using Durados.DataAccess;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Stat.Measurements.Sys
{
    public class TeamSize : SysMeasurement
    {
        public TeamSize(MeasurementType measurementType)
            : base(measurementType)
        {

        }
        protected override string GetSql(SqlProduct sqlProduct)
        {
            ISqlTextBuilder builder = GetSqlTextBuilder(sqlProduct);

            return "select count(*) from durados_User " + builder.WithNolock + " where Role = 'Admin' and " + GetExcludeEmailDomainsWhereStatement();
        }
        
    }
}
