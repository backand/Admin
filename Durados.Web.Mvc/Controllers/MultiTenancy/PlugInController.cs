using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using System.Net;
using Durados.Web.Mvc.UI.Json;

namespace Durados.Web.Mvc.Controllers
{
    public class PlugInController : CrmController
    {
        public ActionResult ViewStyle()
        {


            var jsonModel = new ViewStyle() { views = Map.Database.GetViewNameDisplayList(), styles = Map.Database.GetStyleNameDisplayList() };
            return View(jsonModel);
        }
        
    }
}



