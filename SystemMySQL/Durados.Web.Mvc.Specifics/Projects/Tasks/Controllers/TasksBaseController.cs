using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Durados.Web.Mvc.App.Controllers
{

    [Authorize(Roles = "Developer, Admin, User")]
    public class TasksBaseController : Durados.Web.Mvc.Controllers.DuradosController
    {
        protected override void SetPermanentFilter(Durados.Web.Mvc.View view, Durados.DataAccess.Filter filter)
        {
            if ((new string[4] { "Issue", "Project", "User", "vTimeSheet" }).Contains(view.Name))
            {
                if (User.IsInRole("User"))
                {
                    if (User == null || User.Identity == null || User.Identity.Name == null)
                    {
                        throw new AccessViolationException();
                    }

                    int? companyID = Durados.Web.Mvc.Specifics.DataAccess.User.GetCompanyID(User.Identity.Name);

                    if (!companyID.HasValue)
                    {
                        throw new AccessViolationException();
                    }
                    filter.WhereStatement += " and CompanyID = " + companyID.Value;
                }
            }
            base.SetPermanentFilter((Durados.Web.Mvc.View)view, filter);
        }

        protected override void DropDownFilter(Durados.Web.Mvc.ParentField parentField, ref string sql)
        {
            Durados.View view = parentField.ParentView;
            if ((new string[5] { "Issue", "Project", "User", "TimeSheet", "MonthlyProjectHoursReport" }).Contains(view.Name))
            {
                if (User.IsInRole("User"))
                {
                    if (User == null || User.Identity == null || User.Identity.Name == null)
                    {
                        throw new AccessViolationException();
                    }

                    int? companyID = Durados.Web.Mvc.Specifics.DataAccess.User.GetCompanyID(User.Identity.Name);

                    if (!companyID.HasValue)
                    {
                        throw new AccessViolationException();
                    }

                    sql += " where CompanyID = " + companyID.Value;
                }
            }
        }




    }
}
