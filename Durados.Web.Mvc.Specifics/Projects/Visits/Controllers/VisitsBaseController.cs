using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Durados.Web.Mvc.App.Controllers
{

    [Authorize(Roles = "Developer, Admin, User")]
    public class VisitsBaseController : Durados.Web.Mvc.Controllers.DuradosController
    {
        protected override void SetPermanentFilter(Durados.Web.Mvc.View view, Durados.DataAccess.Filter filter)
        {
            if ((new string[3] { "A_111", "AA_Agents", "AA_Visit" }).Contains(view.Name))
            {
                if (User.IsInRole("User"))
                {
                    if (User == null || User.Identity == null || User.Identity.Name == null)
                    {
                        throw new AccessViolationException();
                    }

                    if (view.Name == "A_111")
                        filter.WhereStatement += " and sochen_1 = " + User.Identity.Name;

                    if (view.Name == "AA_Agents")
                        filter.WhereStatement += " and Sochen = " + User.Identity.Name;

                    if (view.Name == "AA_Visit")
                        filter.WhereStatement += " and SochenID = " + User.Identity.Name;
                }
            }
            base.SetPermanentFilter((Durados.Web.Mvc.View)view, filter);
        }

        protected override void DropDownFilter(Durados.Web.Mvc.ParentField parentField, ref string sql)
        {
            Durados.View view = parentField.ParentView;
            if ((new string[1] { "A_111" }).Contains(view.Name))
            {
                if (User.IsInRole("User"))
                {
                    if (User == null || User.Identity == null || User.Identity.Name == null)
                    {
                        throw new AccessViolationException();
                    }

                    sql += " where sochen_1 = N'" + User.Identity.Name + "'";
                }
            }
        }




    }
}
