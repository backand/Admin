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

    public class CRMShadeJobNotesController : CRMShadeBaseController
    {
        protected override void BeforeCreate(CreateEventArgs e)
        {
            e.Values.Add(JobNotes.FK_JobNotes_User_Parent.ToString(), GetUserID());
            base.BeforeCreate(e);
        }
        
    }
}


