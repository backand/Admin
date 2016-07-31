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
using Durados.Web.Mvc.Specifics.Shade.CRM;
using Durados.Web.Membership;

namespace Durados.Web.Mvc.Specifics.Shade.CRM.Controllers
{

    public class CRMShadeProposalItemController : CRMShadeBaseController
    {
        public JsonResult GetProductInfo(string pk)
        {
            Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic.Product product =
                new Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic.Product();

            return Json(product.GetProductInfo(pk));
        }

        public JsonResult GetVendorInfo(string pk)
        {
            Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic.Vendor vendor =
                new Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic.Vendor();

            return Json(vendor.GetVendorInfo(pk));
        }
        
    }
}


