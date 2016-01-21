using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Durados.Web.Mvc.UI.Helpers;

using Durados.Web.Mvc;
using Durados.DataAccess;
using Durados.Web.Mvc.Controllers.Api;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security;
using Durados.Web.Mvc.Infrastructure;
using MySql.Data.MySqlClient;
using Durados;
using System.Collections;
using System.Data;
using Backand;
using Durados.Web.Mvc.Farm;
/*
 HTTP Verb	|Entire Collection (e.g. /customers)	                                                        |Specific Item (e.g. /customers/{id})
-----------------------------------------------------------------------------------------------------------------------------------------------
GET	        |200 (OK), list data items. Use pagination, sorting and filtering to navigate big lists.	    |200 (OK), single data item. 404 (Not Found), if ID not found or invalid.
PUT	        |404 (Not Found), unless you want to update/replace every resource in the entire collection.	|200 (OK) or 204 (No Content). 404 (Not Found), if ID not found or invalid.
POST	    |201 (Created), 'Location' header with link to /customers/{id} containing new ID.	            |404 (Not Found).
DELETE	    |404 (Not Found), unless you want to delete the whole collection—not often desirable.	        |200 (OK). 404 (Not Found), if ID not found or invalid.
 
 */

namespace BackAnd.Web.Api.Controllers
{
    [RoutePrefix("1")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize("Admin,Developer")]
    public class debugModeController : apiController
    {

        [Route("~/1/debugMode")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                return Ok(SharedMemorySingeltone.Instance.Contains(Map.AppName, SharedMemoryKey.DebugMode));
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }


        [Route("~/1/debugMode")]
        [HttpPut]
        public IHttpActionResult Put(bool? mode)
        {
            try
            {
                if (!mode.HasValue)
                {
                    return BadRequest("missing mode");
                }
                if (mode.Value)
                {
                    int day = 1000 * 60 * 60 * 24;
                    SharedMemorySingeltone.Instance.Set(Map.AppName, SharedMemoryKey.DebugMode, "true", day);
                }
                else
                {
                    SharedMemorySingeltone.Instance.Delete(Map.AppName, SharedMemoryKey.DebugMode);
                }
                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }
    }
}
