using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Durados.Web.Mvc.Specifics.Bugit.Controllers
{

    public class TasksMonthlyProjectHoursReportController : TasksBaseController
    {
        protected override void SetPermanentFilter(Durados.Web.Mvc.View view, Durados.DataAccess.Filter filter)
        {
            if (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("User"))
            {
                if (User == null || User.Identity == null || User.Identity.Name == null)
                {
                    throw new AccessViolationException();
                }

                filter.WhereStatement += " and UserID = " + Durados.Web.Mvc.Specifics.Bugit.DataAccess.User.GetUserID(User.Identity.Name); ;
            }
        }

        
    }
}
