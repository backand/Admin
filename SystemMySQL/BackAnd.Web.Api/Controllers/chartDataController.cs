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
    public class chartDataController : apiController
    {
        
        #region Data

        public IHttpActionResult Get(string id)
        {
            ChartHelperForRest helper = null;
            try
            {
                helper = new ChartHelperForRest(Map);
                return Ok(helper.GetChartData(id, System.Web.HttpContext.Current.Server.UrlDecode(System.Web.HttpContext.Current.Request.QueryString.ToString())));

            }
            catch (ChartIdIsMissingException chartIdIsMissingException)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, chartIdIsMissingException.Message));
            }
            catch (ChartHandledException chartHandledException)
            {
                int? dashboardId = Database.GetDashboardPK(Convert.ToInt32(id));
                if (!dashboardId.HasValue)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, chartHandledException.Message));
                
                try
                {
                    return Ok(helper.GetChartJsonObjectWithException(dashboardId.Value, Convert.ToInt32(id), chartHandledException));
                }
                catch (Exception exception)
                {
                    throw new BackAndApiUnexpectedResponseException(exception, this);
                

                }
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
                

            }
        }

        #endregion Data

    }
    
}
