using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.Web.Mvc.Specifics.Projects.Lm;

namespace Durados.Web.Mvc.App.Controllers
{

    public class LmBaseController : Durados.Web.Mvc.Controllers.DuradosController
    {

        string[] derivedExpenseViews = new string[4] { "Equipment", "General", "Material", "Salary" };

        protected override void BeforeCreate(CreateEventArgs e)
        {
            if (derivedExpenseViews.Contains(e.View.Name))
            {
                int expenseTypeID = Enum.Parse(typeof(ExpenseType), e.View.Name).GetHashCode();
                e.Values.Add("ExpenseType", expenseTypeID);
            }
            base.BeforeCreate(e);
        }

        protected override void BeforeEdit(EditEventArgs e)
        {
            if (derivedExpenseViews.Contains(e.View.Name))
            {
                int expenseTypeID = Enum.Parse(typeof(ExpenseType), e.View.Name).GetHashCode();
                e.Values.Add("ExpenseType", expenseTypeID);
            }
            base.BeforeEdit(e);
        }

        protected override string FormatExceptionMessage(string viewName, string action, Exception exception)
        {
            if (exception is System.Data.SqlClient.SqlException && exception.Message.Contains("Over Fund"))
                return exception.Message;
            return base.FormatExceptionMessage(viewName, action, exception);
        }
    }
}
