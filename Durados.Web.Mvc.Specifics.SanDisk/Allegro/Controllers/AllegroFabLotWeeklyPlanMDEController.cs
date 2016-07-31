using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Globalization;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Membership;



namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroFabLotWeeklyPlanMDEController : AllegroFabLotPlanMDEController
    {

        protected override Durados.Web.Mvc.UI.TableViewer GetNewTableViewer()
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI.FabLotPlanMDEWeeklyTableViewer();
        }

        
    }
}
