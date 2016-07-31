using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.DataAccess;
using Durados.Web.Mvc.Controllers;

namespace Durados.Web.Mvc.App.Controllers
{

    public class CRMTasksController : CrmController
    {
        protected override void SetPermanentFilter(Durados.Web.Mvc.View view, Durados.DataAccess.Filter filter)
        {
            if (User.IsInRole("User"))
            {
                if (User == null || User.Identity == null || User.Identity.Name == null)
                {
                    throw new AccessViolationException();
                }

                int? userID = Durados.Web.Mvc.Specifics.Projects.User.GetUserID(User.Identity.Name);

                filter.WhereStatement += " and AssignedUserId = " + userID;
            }
            base.SetPermanentFilter((Durados.Web.Mvc.View)view, filter);
        }

        protected override void DropDownFilter(Durados.Web.Mvc.ParentField parentField, ref string sql)
        {
            Durados.View view = parentField.ParentView;
            if (User.IsInRole("User") && view.Name == "User")
            {
                if (User == null || User.Identity == null || User.Identity.Name == null)
                {
                    throw new AccessViolationException();
                }

                sql += " where Username = N'" + User.Identity.Name + "'";
            }
        }


        protected override string GetTo(string viewName, System.Data.DataRow dataRow)
        {
            View userView = GetView(Durados.Web.Mvc.Specifics.Projects.CRM.CRMViews.User.ToString());

            View view = GetView(viewName);

            string userPK = view.GetValue(Durados.Web.Mvc.Specifics.Projects.CRM.v_TaskAlert.FK_Task_User2_Parent, dataRow).ToString();
            System.Data.DataRow userRow = userView.GetDataRow(userPK);

            return userRow[Durados.Web.Mvc.Specifics.Projects.CRM.User.Email.ToString()].ToString();
        }

        protected override string GetViewDisplayName(View view)
        {
            return "Tasks";
        }
    }
}
