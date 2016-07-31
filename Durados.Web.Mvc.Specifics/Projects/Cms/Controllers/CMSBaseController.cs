using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc.Specifics.Projects;

namespace Durados.Web.Mvc.App.Controllers
{

    public class CMSBaseController : Durados.Web.Mvc.Controllers.DuradosController
    {

        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);


            //call the site to refresh the content
            Http.AsynWebRequest(RefreshUrl, new ErrorHandling());

        }

        protected string RefreshUrl
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["RefreshContentURL"];
            }
        }

        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);

            //call the site to refresh the content
            Http.AsynWebRequest(RefreshUrl, new ErrorHandling());

        }

    }


    
}
