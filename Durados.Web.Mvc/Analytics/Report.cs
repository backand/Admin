using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Analytics
{
    public class Report
    {
        public Report()
        {

        }

        public object Get(ReportParameters parameters)
        {
            ReportDataAccess reportDataAccess = ReportDataAccess.GetInstance(parameters.ReportName);

            return reportDataAccess.Get(parameters);
        }
    }
}
