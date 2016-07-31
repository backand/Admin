using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Mvc;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Membership;



namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroBaseController : Durados.Web.Mvc.Controllers.CrmController
    {
        protected override Durados.Web.Mvc.Workflow.Engine CreateWorkflowEngine()
        {
            return new BusinessLogic.Workflow.SanDiskWorkfowEngine();
        }

        public override JsonResult GetScalar(string viewName, string pk, string fieldName)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "GetScalar");
            if (view == null)
                return Json(string.Empty);

            Field field = null;

            if (view.Fields.ContainsKey(fieldName))
            {
                field = view.Fields[fieldName];
            }
            else
            {
                field = view.GetFieldByColumnNames(fieldName);
            }

            if (field == null)
                return Json(string.Empty);

            DataRow row = view.GetDataRow(pk);

            string scalar = field.GetValue(row);

            return Json(scalar);

        }
    }
}
