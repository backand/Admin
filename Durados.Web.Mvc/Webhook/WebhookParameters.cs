using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Webhook
{
    class WebhookParameters
    {
        public string Url { get; set; }

        public string Method { get; set; }

        public object Body { get; set; }

        public Dictionary<string, object> QueryStringParameters { get; set; }

        public Dictionary<string, object> Headers { get; set; }

        public WebhookErrorHandling ErrorHandling { get; set; }

        public bool Async { get; set; }

        public string LimitApps { get; set; }
    }
}
