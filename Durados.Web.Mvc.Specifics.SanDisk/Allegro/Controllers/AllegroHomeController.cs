using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroHomeController : AllegroBaseController
    {
        protected override Durados.Web.Mvc.Workflow.Engine CreateWorkflowEngine()
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.Workflow.SanDiskWorkfowEngine();
        }
    }
}
