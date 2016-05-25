using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Durados.DataAccess;
using System.Data;
using System.Data.SqlClient;


namespace Durados.Web.Mvc.Stat.Measurements.Sys
{
    public abstract class SysMeasurement : Measurement
    {
        public SysMeasurement(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }

        protected virtual string[] ExcludedEmailDomains
        {
            get
            {
                return Maps.ExcludedEmailDomains.Split(',');
            }
        }


        

        protected virtual string GetExcludeEmailDomainsWhereStatement()
        {
            string sql = string.Empty;

            foreach (string domain in ExcludedEmailDomains)
            {
                sql += " Email not like '%" + domain + "%' and ";
            }

            sql += " 1=1 ";

            return sql;
        }

        

        
        

        
    }
}
