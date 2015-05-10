using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Membership;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroProductLineController : AllegroHomeController
    {
        public void AddParameters()
        {
            View plmView = GetView("TechnologyProductClassCapacity");
            View parameterView = GetView("Parameter");

            foreach (Field field in plmView.Fields.Values.Where(f => f.Category != null && f.Category.Name == "Parameters"))
            {
                Dictionary<string,object> values = new Dictionary<string,object>();
                values.Add("DisplayName", field.DisplayName);
                values.Add("Name", field.Name);

                try
                {
                    parameterView.Create(values, null, null, null, null, null);
                }
                catch { }
            }
        }

        public JsonResult GetProductLineParameters(string pk)
        {
            Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.ProductLineParameters productLineParameters =
                new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.ProductLineParameters();

            int id = -1;
            
            if (int.TryParse(pk, out id))
                return Json(productLineParameters.GetProductLineParametersInfo(id));
            else
                return Json(string.Empty);

        }

 
    }
}
