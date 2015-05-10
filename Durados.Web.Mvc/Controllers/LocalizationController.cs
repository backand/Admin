using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;


namespace Durados.Web.Mvc.Controllers
{
    public class LocalizationController : Durados.Web.Mvc.Controllers.DuradosController
    {
        protected override Durados.Database GetDatabase()
        {
            return Map.GetLocalizationDatabase();
        }

        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);

            Map.Refresh();
        }

        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);
            Map.Refresh();
        }

        protected override void AfterDeleteAfterCommit(DeleteEventArgs e)
        {
            base.AfterDeleteAfterCommit(e);
            Map.Refresh();
        }
    }
}
