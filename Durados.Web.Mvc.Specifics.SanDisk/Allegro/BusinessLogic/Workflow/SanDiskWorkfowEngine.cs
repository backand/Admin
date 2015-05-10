using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.Workflow
{
    public class SanDiskWorkfowEngine : Durados.Web.Mvc.Workflow.Engine
    {
        public SanDiskWorkfowEngine()
            : base()
        {
        }

        protected override Durados.Workflow.Notifier GetNotifier()
        {
            return new SanDiskNotifier();
        }
    }
}
