using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Analytics
{
    public abstract class ReportDataAccess
    {
        private static ReportDataAccess GetReportDataAccess(ReportDataType reportDataType)
        {
            ReportDataAccess reportDataAccess = null;

            switch (reportDataType)
            {
                case ReportDataType.Coola:
                    reportDataAccess = new CoolaReportDataAccess();
                    break;

                default:
                    break;
            }

            return reportDataAccess;
        }

        private static MemoryCache reportsConfig = null;
        private static ReportConfig[] GetReportsConfig()
        {
            throw new NotImplementedException();
        }

        private static ReportConfig GetReportConfig(string reportName)
        {
            if (reportsConfig == null)
            {
                reportsConfig = new MemoryCache("reportsConfig");

                ReportConfig[] reportConfigArray = GetReportsConfig();

                foreach (ReportConfig reportConfig in reportConfigArray)
                {
                    reportsConfig[reportConfig.Name] = reportConfig;
                }
            }

            if (!reportsConfig.Contains(reportName))
            {
                return null;
            }

            return (ReportConfig)reportsConfig[reportName];
        }

        public static ReportDataAccess GetInstance(string reportName)
        {
            ReportConfig reportConfig = GetReportConfig(reportName);

            if (reportConfig == null)
                return null;

            return GetReportDataAccess(reportConfig.ReportDataType);
        }

        public object Get(ReportParameters parameters)
        {
            ReportConfig reportConfig = GetReportConfig(parameters.ReportName);
            string query = GetQuery(reportConfig.Query, parameters);
            object connectionInfo = GetConnectionInfo(reportConfig.ReportDataType);

            return GetData(query, connectionInfo);
        }

        protected abstract object GetData(string query, object connectionInfo);

        protected abstract object GetConnectionInfo(ReportDataType reportDataType);

        protected abstract string GetQuery(string query, ReportParameters parameters);
        
    }
}
