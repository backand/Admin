using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.DataAccess;
using Durados.Web.Mvc.Controllers;

namespace Durados.Web.Mvc.Specifics.Shade.CRM.Controllers
{

    public class CRMShadeTasksController : CRMShadeBaseController
    {
        protected override void SetPermanentFilter(Durados.Web.Mvc.View view, Durados.DataAccess.Filter filter)
        {
            if (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("User"))
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
            if (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("User") && view.Name == "User")
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
            View userView = GetView(Durados.Web.Mvc.Specifics.Shade.CRM.ShadeViews.User.ToString());

            View view = GetView(viewName);

            //string userPK = view.GetValue(Durados.Web.Mvc.Specifics.Shade.CRM.v_TaskAlert.FK_Task_User2_Parent, dataRow).ToString();
            string userPK = view.GetValue(Durados.Web.Mvc.Specifics.Shade.CRM.v_TaskAlert.User_v_TaskAlert_Parent, dataRow).ToString();
            System.Data.DataRow userRow = userView.GetDataRow(userPK);

            return userRow[Durados.Web.Mvc.Specifics.Shade.CRM.User.Email.ToString()].ToString();
        }

        protected override string GetViewDisplayName(View view)
        {
            return "Tasks";
        }

        protected override Durados.Web.Mvc.UI.Styler GetNewStyler(View view, DataView dataView)
        {
            return new Durados.Web.Mvc.Specifics.Shade.CRM.UI.TaskStyler(view, dataView);
        }
    }
}
