using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Analytics
{
    public class ReportParameters
    {
        public string last { get; set; }
        public DateTime? earlierThan { get; set; }
        public DateTime? laterThan { get; set; }
        public string byThe { get; set; }

        private LastPeriod? lastPeriod = null;
        
        private LastPeriod GetLastPeriod()
        {
            if (!lastPeriod.HasValue)
            {
                if (!string.IsNullOrEmpty(last))
                {

                }
            }
            return lastPeriod.Value;
        } 
        public LastPeriod LastPeriod
        {
            get
            {
                return GetLastPeriod();
            }
        }

        public HttpResponseMessage Validate(HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(last) && (earlierThan.HasValue || laterThan.HasValue))
            {
                return request.CreateResponse(HttpStatusCode.ExpectationFailed, "Please provide either last date period or earlierThan or laterThan or both");
            }
            if ((earlierThan.HasValue && laterThan.HasValue) && (earlierThan.Value > laterThan.Value))
            {
                return request.CreateResponse(HttpStatusCode.ExpectationFailed, "laterThan must be later than earlierThen");
            }
            if (!string.IsNullOrEmpty(last) && !Enum.IsDefined(typeof(LastPeriod), last))
            {
                return request.CreateResponse(HttpStatusCode.ExpectationFailed, "last can be either " + string.Join("','", Enum.GetNames(typeof(LastPeriod))));
            }
            return null;
        }

        public string AppName { get; set; }

        public string ReportName { get; set; }
    }
}
