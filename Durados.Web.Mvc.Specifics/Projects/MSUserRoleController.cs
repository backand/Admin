using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;

namespace Durados.Web.Mvc.App.Controllers
{

    public class MSUserRoleController : Durados.Web.Mvc.Controllers.DuradosController
    {
        protected override void AfterCreateBeforeCommit(CreateEventArgs e)
        {
            string role = e.Values["Name"].ToString();
            System.Web.Security.Roles.CreateRole(role);
            base.AfterCreateBeforeCommit(e);
        }

        protected override void AfterEditBeforeCommit(EditEventArgs e)
        {
            if (e.Values.ContainsKey("Name"))
            {
                string newRole = e.Values["Name"].ToString();
                string currentRole = e.PrevRow["Name"].ToString();

                if (newRole != currentRole)
                {
                    //delete the current role
                    e.Cancel = !System.Web.Security.Roles.DeleteRole(currentRole, true);
                    if (e.Cancel)
                        throw new DuradosException(Map.Database.Localizer.Translate("Failed to rename role"));


                    System.Web.Security.Roles.CreateRole(newRole);

                }
            }
            base.AfterEditBeforeCommit(e);
        }

        protected override void BeforeDelete(DeleteEventArgs e)
        {
            string role = e.PrimaryKey;
            e.Cancel = !System.Web.Security.Roles.DeleteRole(role, true);
            base.BeforeDelete(e);

            if (e.Cancel)
                throw new DuradosException(Map.Database.Localizer.Translate("Failed to delete Role"));
        }

    }
}
