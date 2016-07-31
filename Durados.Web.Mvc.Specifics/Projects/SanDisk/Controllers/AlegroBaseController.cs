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
using Durados.Web.Mvc.Specifics.Projects.CRMShade;
using Durados.Web.Membership;

namespace Durados.Web.Mvc.App.Controllers
{

    public class AlegroBaseController : Durados.Web.Mvc.Controllers.DuradosController
    {
        protected override void SetPermanentFilter(View view, Durados.DataAccess.Filter filter)
        {
            //int? userID = Durados.Web.Mvc.Specifics.Projects.User.GetUserID(User.Identity.Name);

            //if (view.Fields.ContainsKey("V_Contact_User_Parent"))
            //    view.Fields["V_Contact_User_Parent"].DefaultValue = userID;

            //if (view.Fields.ContainsKey("Task_User_Parent"))
            //    view.Fields["Task_User_Parent"].DefaultValue = userID;

            //if (view.Fields.ContainsKey("FK_Task_User_Parent"))
            //    view.Fields["FK_Task_User_Parent"].DefaultValue = userID;

            //if (view.Fields.ContainsKey("FK_Task_User1_Parent"))
            //    view.Fields["FK_Task_User1_Parent"].DefaultValue = userID;

            base.SetPermanentFilter(view, filter);
        }


        
        
    }
}
