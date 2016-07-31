using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;

using Durados;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Specifics.Projects.Lm;

namespace Durados.Web.Mvc.App.Controllers
{
    [HandleError]
    public class LmExpenseController : LmBaseController
    {
        protected override Durados.Web.Mvc.View GetView(string viewName, string action)
        {
            if (action == "IndexPage")
                return GetExpenseView(viewName, action);
            else
                return base.GetView(viewName, action);
        }

        protected virtual Durados.Web.Mvc.View GetExpenseView(string viewName, string action)
        {
            
            Durados.Web.Mvc.View view = GetView(viewName);
            System.Collections.Specialized.NameValueCollection queryString = ViewHelper.GetPageFilterState(viewName);
            if (queryString["GrantItemDetailID"] != null)
            {
                int grantItemDetailID = Convert.ToInt32(queryString["GrantItemDetailID"]);
                return GetExpenseView(grantItemDetailID);
            }
            else
            {
                return base.GetView(viewName, action);
            }
        }

        protected virtual Durados.Web.Mvc.View GetExpenseView(int grantItemDetailID)
        {
            
            int expenseTypeID=Durados.Web.Mvc.Specifics.Projects.Lm.DataAccess.ExpenseType.GetExpenseTypeID(grantItemDetailID).Value;
            ExpenseType expenseType = (ExpenseType)Enum.ToObject(typeof(ExpenseType), expenseTypeID);
            string viewName = expenseType.ToString();
            return GetView(viewName);
        }

    }

}
