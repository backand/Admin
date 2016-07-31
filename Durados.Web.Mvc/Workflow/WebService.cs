using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Workflow
{
    public class WebService : Durados.Workflow.WebService
    {

        protected override string CallWebService(string webServiceUrl)
        {
            return Infrastructure.Http.CallWebService(webServiceUrl);
        }


        
    }
}
