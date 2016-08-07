using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Analytics
{
    public class CqlConfig
    {
        public string ApiUrl { get; set; }

        public Dictionary<string, string> Cqls { get; set; }

        public string AuthorizationHeader { get; set; }
    }
}
