using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Durados.Web.Mvc.App.Controllers
{

    [Authorize(Roles = "Developer, Admin, User")]
    public class LMSBaseController : Durados.Web.Mvc.Controllers.DuradosController
    {

    }
}
