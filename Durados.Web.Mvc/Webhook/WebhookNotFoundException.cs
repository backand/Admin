using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Webhook
{
    public class WebhookNotFoundException : DuradosException
    {
        public WebhookNotFoundException(string type)
            : base("The webhook '" + type + "' was not found")
        {

        }
    }
}
