using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.DataAccess;
using Durados.Web.Mvc.Controllers;

namespace Durados.Web.Mvc.Specifics.EPL.Controllers
{

    public abstract class CRMInfrastructureTasksController : CrmController
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

        protected abstract string UserViewName
        {
            get;
        }
        protected override string GetTo(string viewName, System.Data.DataRow dataRow)
        {
            //View userView = GetView(CRMViews.User.ToString());
            View userView = GetView(UserViewName);

            View view = GetView(viewName);

            //string userPK = view.GetValue(v_TaskAlert.FK_Task_User2_Parent, dataRow).ToString();
            string userPK = view.GetValue(Task_User_Parent, dataRow).ToString();
            System.Data.DataRow userRow = userView.GetDataRow(userPK);

            //return userRow[User.Email.ToString()].ToString();
            return userRow[UserEmailFieldName].ToString();
        }

        protected abstract Enum Task_User_Parent
        {
            get;
        }

        protected abstract string UserEmailFieldName
        {
            get;
        }

        protected override string GetViewDisplayName(View view)
        {
            return "Tasks";
        }
    }
}