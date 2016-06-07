using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Webhook
{
    public class WebhookResponseException : DuradosException
    {
        public WebhookResponseException(string message)
            : base(message)
        {

        }
    }
}
