using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.CronScheduler
{
    public abstract class Scheduler
    {

        public abstract string Standardize(string schedule);
    }
}
