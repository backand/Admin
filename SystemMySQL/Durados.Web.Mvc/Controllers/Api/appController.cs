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

/*
 HTTP Verb	|Entire Collection (e.g. /customers)	                                                        |Specific Item (e.g. /customers/{id})
-----------------------------------------------------------------------------------------------------------------------------------------------
GET	        |200 (OK), list data items. Use pagination, sorting and filtering to navigate big lists.	    |200 (OK), single data item. 404 (Not Found), if ID not found or invalid.
PUT	        |404 (Not Found), unless you want to update/replace every resource in the entire collection.	|200 (OK) or 204 (No Content). 404 (Not Found), if ID not found or invalid.
POST	    |201 (Created), 'Location' header with link to /customers/{id} containing new ID.	            |404 (Not Found).
DELETE	    |404 (Not Found), unless you want to delete the whole collection—not often desirable.	        |200 (OK). 404 (Not Found), if ID not found or invalid.
 
 */

namespace Durados.Web.Mvc.Controllers.Api
{
    [JsonpFilter]
    public class appController : RestController
    {
        //
        // GET: /v1/

        const string AppView = "Database";

        #region config

        [System.Web.Mvc.ActionName("config")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Get()
        {
            try
            {
                View view = GetView(AppView);

                var item = RestHelper.GetApp(view, Map.Database.GetPublicWorkspace(), Map.Database);

                Response.StatusCode = (int)HttpStatusCode.OK;


                return Json(item);
            }
            catch (Exception exception)
            {
                return UnexpectedException(exception, Response);

            }
        }

        #endregion config
        
    }

}
