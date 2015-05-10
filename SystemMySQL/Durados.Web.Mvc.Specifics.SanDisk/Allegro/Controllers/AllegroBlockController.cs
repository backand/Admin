using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroBlockController : Durados.Web.Mvc.Controllers.BlockController
    {
        protected override string GetViewName()
        {
            return "v_POR";
        } 
    }
}
