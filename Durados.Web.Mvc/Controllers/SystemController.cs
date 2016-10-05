using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Durados.Web.Mvc.Controllers
{
    
        public class SystemController : BaseController
        {
            public JsonResult Status()
            {
                try
                {
                    bool status = true;
                    string guid = Maps.Instance.DuradosMap.Database.GetGuidByUsername("dev@devitout.com");
                    if (string.IsNullOrEmpty(guid))
                        status = false;

                    var helthCheck = new HealthCheckResponse
                    {
                        version = Durados.Web.Mvc.Infrastructure.General.Version(),
                        time = DateTime.Now,
                        status = status
                    };
                    return Json(helthCheck);
                }
                catch(Exception ex)
                {
                    var helthCheck = new HealthCheckResponse
                    {
                        message = ex.Message,
                        time = DateTime.Now,
                        status = false

                    };
                    return Json(helthCheck);
                }
                 
            }
        }
   
}

public class HealthCheckResponse
{
    public string version { get; set; }

    public string node { get; set; }

    public string nodeVersion { get; set; }

    public string instance { get; set; }

    public DateTime time { get; set; }

    public bool status { get; set; }

    public string message { get; set; }
}
//string validGuid = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetTmpUserGuidFromGuid(guid);
                