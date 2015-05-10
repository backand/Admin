using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Durados.Web.Mvc.Specifics.Bugit.Controllers
{

    public class TasksTimeSheetController : TasksBaseController
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

        protected override void DropDownFilter(Durados.Web.Mvc.ParentField parentField, ref string sql)
        {
            base.DropDownFilter(parentField, ref sql);
            
            Durados.View view = parentField.ParentView;
            if (view.Name == "Issue")
            {
                if (sql.ToLower().Contains("where"))
                    sql += " and [Issue].[TimeEstimate] > 0";
                else
                    sql += "where [Issue].[TimeEstimate] > 0";
            }

        }

        protected override void BeforeCreate(CreateEventArgs e)
        {
            if (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("User"))
            {
                if (User == null || User.Identity == null || User.Identity.Name == null)
                {
                    throw new AccessViolationException();
                }

                int? userID = Durados.Web.Mvc.Specifics.Bugit.DataAccess.User.GetUserID(User.Identity.Name);

                if (!userID.HasValue)
                {
                    throw new AccessViolationException();
                }

                e.Values.Add("FK_TimeSheet_User_Parent", userID.ToString());
            }

            base.BeforeCreate(e);
        }

        protected override void BeforeEdit(EditEventArgs e)
        {
            if (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("User"))
            {
                if (User == null || User.Identity == null || User.Identity.Name == null)
                {
                    throw new AccessViolationException();
                }

                int? userID = Durados.Web.Mvc.Specifics.Bugit.DataAccess.User.GetUserID(User.Identity.Name);

                if (!userID.HasValue)
                {
                    throw new AccessViolationException();
                }

                e.Values.Add("FK_TimeSheet_User_Parent", userID.ToString());
            }

            base.BeforeEdit(e);
        }
    }
}
