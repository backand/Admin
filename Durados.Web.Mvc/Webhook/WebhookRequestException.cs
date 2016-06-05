using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Webhook
{
    public class WebhookRequestException : DuradosException
    {
        public WebhookRequestException(string message, Exception innerException):
            base(message, innerException)
        {

        }
    }
}
