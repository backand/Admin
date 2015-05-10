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
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class dashboardConfigController : apiController
    {
        
        #region Config

        public IHttpActionResult Get(string id = null)
        {
            try
            {
                const string ViewName = "MyCharts";
                View view = GetView(ViewName);
                if (view == null)
                {

                    return NotFound();
                }

                if (!string.IsNullOrEmpty(id))
                {

                    var item = RestHelper.Get(view, id, true, view_BeforeSelect, view_AfterSelect);
                    if (item == null)
                    {

                        return NotFound();
                    }

                    return Ok(item);
                }
                else
                {
                    int rowCount = 0;


                    var items = RestHelper.Get(view, false, false, 1, 1000, null, null, null, out rowCount, false, view_BeforeSelect, view_AfterSelect);

                    return Ok(items);
                }
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
               
            }
        }
        
        #endregion Config

    }

}
