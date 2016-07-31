using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Specifics.Projects.CRMShade;
using Durados.Web.Membership;

namespace Durados.Web.Mvc.App.Controllers
{

    public class CRMShadeJobController : CRMShadeBaseController
    {
        protected override void SetPermanentFilter(Durados.Web.Mvc.View view, Durados.DataAccess.Filter filter)
        {
            if (User.IsInRole("User"))
            {
                if (User == null || User.Identity == null || User.Identity.Name == null)
                {
                    throw new AccessViolationException();
                }

                filter.WhereStatement += " and SalesUserId = " + view.Database.GetUserID();
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

                if (!sql.ToLower().Contains("where"))
                {
                    sql += " where Username = N'" + User.Identity.Name + "'";
                }
            }
        }

        public JsonResult GetContactInfo(string pk)
        {
            Durados.Web.Mvc.Specifics.Projects.CRMShade.BusinessLogic.Contact vendor =
                new Durados.Web.Mvc.Specifics.Projects.CRMShade.BusinessLogic.Contact();

            try
            {
                Durados.Web.Mvc.Specifics.Projects.CRMShade.BusinessLogic.Json.ContactInfo contactInfo = vendor.GetContactInfo(pk);
                if (contactInfo == null)
                    return Json("");
                else
                    return Json(contactInfo);
            }
            catch (Exception exception)
            {
                Durados.Web.Mvc.Logging.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, pk);

                return Json("");
                
            }
        }

       
    }
}


