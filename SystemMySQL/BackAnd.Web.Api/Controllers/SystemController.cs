
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
                return Ok(new { version = Durados.Web.Mvc.Infrastructure.General.Version() });
      
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }
        
    }

    
}
