using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Specifics.Projects.Gear;

namespace Durados.Web.Mvc.App.Controllers
{
    [HandleError]
    public class GearHomeController : GearBaseController
    {
        public ActionResult Stam()
        {
            return View();
        }

        public virtual void MatchFields(string viewName)
        {
            Durados.Web.Mvc.View view = GetView(viewName);
            Durados.Web.Mvc.View fieldView = GetView(GearViews.durados_Field.ToString());

            Dictionary<string, object> values = new Dictionary<string, object>();

            int rowCount = 0;
            DataView dataView = fieldView.FillPage(1, 1000, values, null, SortDirection.Asc, out rowCount, null, null);
            foreach (DataRow row in dataView.Table.Rows)
            {
                string displayName = row[durados_Field.DisplayName.ToString()].ToString();
                string name = row[durados_Field.Name.ToString()].ToString();
                if (view.Fields.ContainsKey(name))
                    view.Fields[name].DisplayName = displayName;
            }

            Durados.Web.Mvc.Map.SaveConfigForThwFirstTimeInCaseOfChangeInStructure();

        }
    }

}
