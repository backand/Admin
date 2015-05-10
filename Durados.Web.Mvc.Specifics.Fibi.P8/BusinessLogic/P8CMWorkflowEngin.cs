using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Fibi.P8.BusinessLogic
{
    public class P8CMEngin : Durados.Web.Mvc.Workflow.Engine
    {
        protected override Durados.Workflow.XmlDocument GetXmlDocument()
        {
            return new P8CMXmlDocument();
        }
    }
}
