
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;


namespace BackAnd.Web.Api.Controllers
{
    
    public class systemController : apiController
    {
        public IHttpActionResult Get()
        {
            try
            {
                string json = "{\"newSchema\":[],\"oldSchema\":[],\"severity\":0}";
                string node = "node is running";

                try
                {
                    Dictionary<string, object> transformResult = Transform(json, false);
                }
                catch (Exception exception)
                {
                    node = exception.InnerException == null ? exception.Message : exception.InnerException.Message;
                }
                return Ok(new { version = Durados.Web.Mvc.Infrastructure.General.Version(), node = node });
      
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }
        
    }

    
}
