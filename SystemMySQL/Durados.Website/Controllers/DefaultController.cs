using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Website.Controllers
{
    public class DefaultController: Durados.Web.Mvc.Controllers.BaseController
    {
        //[Durados.Web.Mvc.Controllers.Attributes.RequiresSSL]
        public ActionResult Home()
        {
            bool hostByUs = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["hostByUs"] ?? "true");
            //for the main app (www), when routing back to the website, first redirect to / (e.g. www.backand.com)
            //and only when coming iwth url= '/' load the index page and return it
            Durados.Web.Mvc.Map map = Durados.Web.Mvc.Maps.Instance.GetMap();
            if (map.IsMainMap && hostByUs)
                if (Request.Url.AbsoluteUri != map.Url && Request.Url.AbsoluteUri != map.Url + "/")
                    return RedirectPermanent(map.Url);
                else
                    return File(Server.MapPath("/ws/index.html"), "text/html");
            else
            {
                return RedirectToAction("Default", "Home");
            }

        }

    }
}
