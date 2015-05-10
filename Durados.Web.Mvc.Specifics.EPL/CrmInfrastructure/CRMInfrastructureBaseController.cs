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

namespace Durados.Web.Mvc.Specifics.EPL.Controllers
{

    [Authorize(Roles = "Developer, Admin, User")]
    public abstract class CRMInfrastructureBaseController : Durados.Web.Mvc.Controllers.CrmController
    {
        protected override void SetPermanentFilter(View view, Durados.DataAccess.Filter filter)
        {
            int? userID = Durados.Web.Mvc.Specifics.Projects.User.GetUserID(User.Identity.Name);

            foreach (string fieldName in GetPermanentFilterFieldsNames())
            {
                if (view.Fields.ContainsKey(fieldName))
                    view.Fields[fieldName].DefaultValue = userID;
            }

            base.SetPermanentFilter(view, filter);
        }

        protected abstract string[] GetPermanentFilterFieldsNames();
        
    }
}
