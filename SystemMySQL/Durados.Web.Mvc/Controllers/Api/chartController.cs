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
using Durados.DataAccess;

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
    public class chartController : RestController
    {
        //
        // GET: /v1/

       

        #region data


        [System.Web.Mvc.ActionName("data")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Get(string pk, string parameters)
        {
            ChartHelperForRest helper = null;
            try
            {
                helper = new ChartHelperForRest(Map);
                return Json(helper.GetChartData(pk, parameters));

            }
            catch (ChartIdIsMissingException)
            {
                return new ChartNotFoundApiHttpException(pk, Response);
            }
            catch (ChartHandledException exception)
            {
                int? dashboardId = Database.GetDashboardPK(Convert.ToInt32(pk));
                if (!dashboardId.HasValue)
                    return new ChartNotFoundApiHttpException(pk, Response);

                try
                {
                    return Json(helper.GetChartJsonObjectWithException(dashboardId.Value, Convert.ToInt32(pk), exception));
                }
                catch (Exception e)
                {
                    return UnexpectedException(e, Response);

                }
            }
            catch (Exception exception)
            {
                return UnexpectedException(exception, Response);

            }
        }

        

        #endregion data

        #region config

        [System.Web.Mvc.ActionName("config")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Get(string pk)
        {
            try
            {
                const string ViewName = "Chart";
                View view = GetView(ViewName);
                if (view == null)
                {

                    Exception exception = new DuradosException(string.Format("Could not find {0} view in configuration", ViewName));
                    return new UnexpectedApiHttpException(exception, Response);
                }

                if (!string.IsNullOrEmpty(pk))
                {

                    var item = RestHelper.Get(view, pk, true, view_BeforeSelect, view_AfterSelect);
                    if (item == null)
                    {

                        return new ChartNotFoundApiHttpException(pk, Response);
                    }
                                        
                    Response.StatusCode = (int)HttpStatusCode.OK;

                    return Json(item, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int rowCount = 0;

                   
                    var items = RestHelper.Get(view, false, false, 1, 1000, null, null, null, out rowCount, false, view_BeforeSelect, view_AfterSelect);

                    Response.StatusCode = (int)HttpStatusCode.OK;

                    return Json(items, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                return new UnexpectedApiHttpException(exception, Response);

            }
        }

        #endregion metadata

           
    }
    
    
    

    public class ChartNotFoundApiHttpException : NotFoundException
    {
        public ChartNotFoundApiHttpException(string id, HttpResponseBase response) : base(string.Format("Chart with id '{0}' was not found.", id), response) { }
    }
}
