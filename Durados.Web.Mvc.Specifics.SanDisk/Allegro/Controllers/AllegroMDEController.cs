using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Membership;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroMDEController : AllegroHomeController
    {
        protected override string FormatExceptionMessage(string viewName, string action, Exception exception)
        {
            string waferIndex = "IX_MDE_Wafer";
            string dieIndex = "IX_MDE_Die";

            if (exception.Message.Contains(waferIndex))
            {
                return "Duplicate Wafers are not allowed";
            }
            else if (exception.Message.Contains(dieIndex))
            {
                return "Duplicate Dies are not allowed";
            }
            return base.FormatExceptionMessage(viewName, action, exception);
        }

 
    }
}
