using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    class InvalidCronHeadersJsonException : DuradosException
    {
        public InvalidCronHeadersJsonException(Exception innerException)
            : base("The Cron Header must be a valid JSON", innerException)
        {

        }
    }
}
