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
    [RoutePrefix("1/payment")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class paymentController : wfController
    {
        [Route("create")]
        [HttpPost]
        public virtual IHttpActionResult create()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (!values.ContainsKey("currency"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "currency is missing"));

                }

                string currency = values["currency"].ToString();


                if (!values.ContainsKey("total"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "total is missing"));
                }

                string total = values["total"].ToString();

                OnlinePayments.PayPal paypal = new OnlinePayments.PayPal();

                paypal.Create(currency, total);

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [Route("execute")]
        [HttpPost]
        public virtual IHttpActionResult execute()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (!values.ContainsKey("paymentId"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "paymentId is missing"));

                }

                string paymentId = values["paymentId"].ToString();


                if (!values.ContainsKey("payerId"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "payerId is missing"));
                }

                string payerId = values["payerId"].ToString();

                OnlinePayments.PayPal paypal = new OnlinePayments.PayPal();

                paypal.Execute(paymentId, payerId);

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }
    }

}
