using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;

namespace Durados.Web.Mvc.App.Controllers
{
    [Authorize(Roles = "Developer, Admin, User")]
    public class TasksIssueController : TasksBaseController
    {
        protected override void BeforeCreate(CreateEventArgs e)
        {
            if (User == null || User.Identity == null || User.Identity.Name == null)
            {
                throw new AccessViolationException();
            }

            int? userID = Durados.Web.Mvc.Specifics.DataAccess.User.GetUserID(User.Identity.Name);

            if (!userID.HasValue)
            {
                throw new AccessViolationException();
            }

            int productID = Convert.ToInt32(e.Values["FK_Issue_Project_Parent"]);

            int? companyID = Durados.Web.Mvc.Specifics.Projects.Tasks.DataAccess.Project.GetCompanyID(productID);
            if (!companyID.HasValue)
            {
                throw new AccessViolationException();
            }

            e.Values.Add("FK_Issue_User_Report_Parent", userID.ToString());
            e.Values.Add("ReportDate", DateTime.Now.ToString());
            e.Values.Add("ModifiedDate", DateTime.Now.ToString());
            e.Values.Add("FK_Issue_Status_Parent", "1" /*open*/);
            e.Values.Add("FK_Issue_Company_Parent", companyID.Value.ToString());
            
            base.BeforeCreate(e);
        }

        protected override void BeforeEdit(EditEventArgs e)
        {
            if (User == null || User.Identity == null || User.Identity.Name == null)
            {
                throw new AccessViolationException();
            }

            int? userID = Durados.Web.Mvc.Specifics.DataAccess.User.GetUserID(User.Identity.Name);

            if (!userID.HasValue)
            {
                throw new AccessViolationException();
            }

            int productID = Convert.ToInt32(e.Values["FK_Issue_Project_Parent"]);

            int? companyID = Durados.Web.Mvc.Specifics.Projects.Tasks.DataAccess.Project.GetCompanyID(productID);
            if (!companyID.HasValue)
            {
                throw new AccessViolationException();
            }

            e.Values.Add("FK_Issue_User_Parent", userID.ToString());
            e.Values.Add("ModifiedDate", DateTime.Now.ToString());
            e.Values.Add("FK_Issue_Company_Parent", companyID.Value.ToString());

            base.BeforeEdit(e);
        }

    }
}
