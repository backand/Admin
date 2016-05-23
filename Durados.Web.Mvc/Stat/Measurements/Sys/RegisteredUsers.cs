using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.DataAccess;

namespace Durados.Web.Mvc.Stat.Measurements.Sys
{
    public class RegisteredUsers : SysMeasurement
    {
        public RegisteredUsers(MeasurementType measurementType)
            : base(measurementType)
        {

        }
        protected override string GetSql(SqlProduct sqlProduct)
        {
            ISqlTextBuilder builder = GetSqlTextBuilder(sqlProduct);

            return "select count(*) from durados_User " + builder.WithNolock + " where Role <> 'Admin' and " + GetExcludeEmailDomainsWhereStatement();
        }

        
    }
}
