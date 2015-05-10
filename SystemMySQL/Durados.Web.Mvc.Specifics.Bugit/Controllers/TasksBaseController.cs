using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Durados.Web.Mvc.Specifics.Bugit.Controllers
{

    public class TasksBaseController : Durados.Web.Mvc.Controllers.CrmController
    {

        protected override Durados.Web.Mvc.Workflow.Engine CreateWorkflowEngine()
        {
            return new Durados.Web.Mvc.Specifics.Bugit.Workflow.BugitWorkfowEngine();
        }

        protected override void SetPermanentFilter(Durados.Web.Mvc.View view, Durados.DataAccess.Filter filter)
        {
            if ((new string[4] { "Issue", "Project", "User", "vTimeSheet" }).Contains(view.Name))
            {
                if (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("User"))
                {
                    if (User == null || User.Identity == null || User.Identity.Name == null)
                    {
                        throw new AccessViolationException();
                    }

                    int? companyID = DataAccess.User.GetCompanyID(User.Identity.Name);

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
            if ((new string[6] { "Issue", "Project", "User", "TimeSheet", "MonthlyProjectHoursReport", "Company" }).Contains(view.Name))
            {
                if (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("User"))
                {
                    if (User == null || User.Identity == null || User.Identity.Name == null)
                    {
                        throw new AccessViolationException();
                    }

                    int? companyID = DataAccess.User.GetCompanyID(User.Identity.Name);

                    if (!companyID.HasValue)
                    {
                        throw new AccessViolationException();
                    }

                    if(view.Name == "Company")
                        sql += " where ID = " + companyID.Value;
                    else if(sql.IndexOf("CompanyID")<0)
                        if (sql.ToLower().Contains("where"))
                            sql += " and CompanyID = " + companyID.Value;
                        else
                            sql += " where CompanyID = " + companyID.Value;
                }
            }
        }




    }
}
