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
    public class VisitsVisitController : VisitsBaseController
    {
        protected override void BeforeCreate(CreateEventArgs e)
        {
                
            if (User == null || User.Identity == null || User.Identity.Name == null)
            {
                throw new AccessViolationException();
            }

            if (User.IsInRole("User"))
            {
                string fk = "FK_AA_Visit_AA_Agents_Parent";

                if (e.Values.ContainsKey(fk))
                    e.Values.Remove(fk);

                e.Values.Add(fk, User.Identity.Name);
            }
            base.BeforeCreate(e);
        }

        
    }
}
