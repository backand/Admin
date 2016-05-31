using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Analytics
{
    public class ReportConfig
    {
        public string Query {get; set;}
        public string Name {get; set;}
        public ReportDataType ReportDataType {get; set;}
    }

}
