using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Analytics
{
    public class EmbeddedReportsConfig
    {
        public EmbeddedReportsConfig(string apiToken, string urlStep1, string urlStep2, string appPropertyName)
        {
            ApiToken = apiToken;
            UrlStep1 = urlStep1;
            UrlStep2 = urlStep2;
            AppPropertyName = appPropertyName;
        }
        public string ApiToken { get; private set; }


        public string UrlStep1 { get; private set; }

        public string UrlStep2 { get; private set; }
        public string AppPropertyName { get; private set; }

        
    }

}

