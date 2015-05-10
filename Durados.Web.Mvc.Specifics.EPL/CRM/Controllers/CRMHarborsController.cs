using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.Web.Mvc.Specifics.EPL.CRM;

namespace Durados.Web.Mvc.Specifics.EPL.Controllers
{

    public class CRMHarborsController : CRMBaseController
    {
 
        protected override void DropDownFilter(Durados.Web.Mvc.ParentField parentField, ref string sql)
        {
            Durados.View view = parentField.ParentView;
            if (view.Name == CRMViews.CountryType.ToString())
            {

                sql += " where Active = 1";
            }
        }

    }
}
