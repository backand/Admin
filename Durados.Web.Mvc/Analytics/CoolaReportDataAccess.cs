using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Analytics
{
    public class CoolaReportDataAccess : ReportDataAccess
    {
        protected override object GetConnectionInfo(ReportDataType reportDataType)
        {
            throw new NotImplementedException();
        }

        protected override object GetData(string query, object connectionInfo)
        {
            throw new NotImplementedException();
        }

        protected override string GetQuery(string query, ReportParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
