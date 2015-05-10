using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.DataAccess;
using Durados.Web.Mvc.Controllers;

namespace Durados.Web.Mvc.App.Controllers
{

    public class AlegroTtpController : AlegroBaseController
    {
        
        protected override Durados.Web.Mvc.UI.Styler GetNewStyler()
        {
            return new Durados.Web.Mvc.Specifics.Projects.SanDisk.UI.TtpStyler();
        }
    }
}
