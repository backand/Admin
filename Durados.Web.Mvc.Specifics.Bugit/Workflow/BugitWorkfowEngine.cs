using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Bugit.Workflow
{
    public class BugitWorkfowEngine : Durados.Web.Mvc.Workflow.Engine
    {
        public BugitWorkfowEngine()
            : base()
        {
        }

        protected override Durados.Workflow.Notifier GetNotifier()
        {
            return new BugitNotifier();
        }
    }
}
