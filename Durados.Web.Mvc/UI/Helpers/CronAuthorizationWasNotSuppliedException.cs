using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class CronAuthorizationWasNotSuppliedException : DuradosException
    {
        public CronAuthorizationWasNotSuppliedException()
            : base("Cron authorization was not supplied")
        {

        }
    }
}
