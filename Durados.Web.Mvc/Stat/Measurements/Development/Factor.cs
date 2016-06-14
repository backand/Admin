using Durados.DataAccess;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Stat.Measurements.Development
{
    public class Factor : XmlSize
    {
        public Factor(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }

        public override object Get(DateTime date)
        {
            long? xmlSize = GetXmlSizeFromDb(date);
            if (xmlSize.HasValue)
            {
                return xmlSize.Value;
            }
            else
            {
                xmlSize = (long)base.Get(date);
            }

            if (!xmlSize.HasValue)
                return 1;

            return GetFactor(date, xmlSize.Value);
        }

        private object GetFactor(DateTime date, long xmlSize)
        {
            xmlSize = Convert.ToInt64(xmlSize / 1000);

            string sql = "select top(1) factor from [billing_request_factor] where ([startDate] < @date and ([endDate] is null or [endDate] > @date) and (xmlSize < @xmlSize)) order by factor desc";

            var parameters = new Dictionary<string, object>();
            parameters.Add("date", date);
            parameters.Add("xmlSize", xmlSize);

            SqlAccess sa = new SqlAccess();
            string scalar = sa.ExecuteScalar(Maps.ReportConnectionString, sql, parameters);
            if (string.IsNullOrEmpty(scalar))
                return null;

            return Convert.ToSingle(scalar);
        }
       
        private long? GetXmlSizeFromDb(DateTime date)
        {
            var appRow = GetAppRow();

            var SqlConId = appRow.SystemSqlConnectionId;

            string sql = "select top(1) XmlSize from [modubiz_LogStats2] where [LogDate] = @date and SqlConId = @SqlConId";
            var parameters = new Dictionary<string, object>();
            parameters.Add("date", date);
            parameters.Add("SqlConId", SqlConId);

            SqlAccess sa = new SqlAccess();
            string scalar = sa.ExecuteScalar(Maps.ReportConnectionString, sql, parameters);
            if (string.IsNullOrEmpty(scalar))
                return null;

            return Convert.ToInt64(scalar);
        }
    }
}
