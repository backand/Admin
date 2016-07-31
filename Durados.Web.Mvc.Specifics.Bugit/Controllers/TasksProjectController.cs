using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;

namespace Durados.Web.Mvc.Specifics.Bugit.Controllers
{
    public class TasksProjectController : TasksBaseController
    {
        protected override void BeforeCreate(CreateEventArgs e)
        {
                
            if (User == null || User.Identity == null || User.Identity.Name == null)
            {
                throw new AccessViolationException();
            }

            if (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("User"))
            {
                int? companyID = Durados.Web.Mvc.Specifics.Bugit.DataAccess.User.GetCompanyID(User.Identity.Name);

                if (!companyID.HasValue)
                {
                    throw new AccessViolationException();
                }
                
                e.Values.Add("FK_Project_Company_Parent", companyID.Value.ToString());
            }
            base.BeforeCreate(e);
        }

        
    }
}
