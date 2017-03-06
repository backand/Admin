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
    public class RestController : CrmController
    {
        //
        // GET: /v1/

        #region uri
        protected string GetControllerName()
        {
            return ControllerContext.RouteData.Values["controller"].ToString();
        }

        protected string GetActionName()
        {
            return ControllerContext.RouteData.Values["action"].ToString();
        }

        public const char Slash = '/';
        protected virtual string ApiVersion
        {
            get
            {
                return "/1";
            }
        }

        protected string GetErrorUri(Exception exception)
        {
            return "/Error";
        }

        #endregion uri

        #region repository
        protected Dictionary<string, object> GetItem()
        {
            Dictionary<string, object> item = new Dictionary<string, object>();
            item.Add("numericField", 1);
            item.Add("textField", "text1");
            item.Add("dateField", DateTime.Now);
            item.Add("boolField", true);
            Dictionary<string, object> parentItem = new Dictionary<string, object>();
            parentItem.Add("numericField", 2);
            parentItem.Add("textField", "text2");
            item.Add("objectField", parentItem);

            return item;
        }

        protected Dictionary<string, object>[] GetItemCollection(int pageNumber)
        {
            Dictionary<string, object>[] itemCollection = new Dictionary<string, object>[pageNumber];

            for (int i = 0; i < pageNumber; i++)
            {
                itemCollection[i] = GetItem();
                itemCollection[i]["numericField"] = i;
            }

            return itemCollection;
        }

        protected Dictionary<string, object> GetItemFromRequest(string key = null)
        {
            string json = null;
            if (key == null)
            {
                json = Server.UrlDecode(Request.Form.ToString());
            }
            else
            {
                json = Server.UrlDecode(Request.Form[key]);
 
            }
            return JsonConverter.Deserialize(json);
        }

        protected string GetJsonFromRequest(string key = null)
        {
            string json = null;
            if (key == null)
            {
                json = Server.UrlDecode(Request.Form.ToString());
            }
            else
            {
                json = Server.UrlDecode(Request.Form[key]);

            }
            return json;
        }
        #endregion repository

        public ActionResult Test()
        {
            return View();
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding)
        {
            return base.Json(data, contentType, contentEncoding);
        }

        protected virtual UnexpectedApiHttpException UnexpectedException(Exception exception, HttpResponseBase Response)
        {
            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null); 
            return new UnexpectedApiHttpException(exception, Response);
        }

    }

    public class JsonConverter
    {
        public static Dictionary<string, object> Deserialize(string json)
        {
            var jss = new JavaScriptSerializer();
            jss.MaxJsonLength = Int32.MaxValue; 

            var dict = jss.Deserialize<dynamic>(json);

            return (Dictionary<string, object>)dict;
        }

        public static Dictionary<string, object>[] DeserializeArray(string json)
        {
            var jss = new JavaScriptSerializer();
            jss.MaxJsonLength = Int32.MaxValue;
            var dict = jss.Deserialize<Dictionary<string, object>[]>(json);

            return (Dictionary<string, object>[])dict;
        }

        public static string[] DeserializeStringArray(string json)
        {
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<string[]>(json);

            return (string[])dict;
        }
    }

    public class ApiHttpException : HttpStatusCodeResult
    {

        public ApiHttpException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message) { }
    }

    public class NotFoundException : HttpStatusCodeResult
    {

        public NotFoundException(string message, HttpResponseBase response) : base(HttpStatusCode.NotFound, message) 
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            
        }
    }

    public class UnexpectedApiHttpException : ApiHttpException
    {
        public UnexpectedApiHttpException(Exception exception, HttpResponseBase response)
            : base(HttpStatusCode.SeeOther, exception.Message)
        {
            string uri = GetErrorUri();
            response.StatusCode = (int)HttpStatusCode.SeeOther;
            response.Headers.Add("Location", uri);

        }

        protected string GetErrorUri()
        {
            return "/Error";
        }
    }

    /// <summary>
    /// Renders result as JSON and also wraps the JSON in a call
    /// to the callback function specified in "JsonpResult.Callback".
    /// </summary>
    public class JsonpResult : JsonResult
    {
        /// <summary>
        /// Gets or sets the javascript callback function that is
        /// to be invoked in the resulting script output.
        /// </summary>
        /// <value>The callback function name.</value>
        public string Callback { get; set; }

        /// <summary>
        /// Enables processing of the result of an action method by a
        /// custom type that inherits from
        /// <see cref="T:System.Web.Mvc.ActionResult"/>.
        /// </summary>
        /// <param name="context">The context within which the
        /// result is executed.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            if (!String.IsNullOrEmpty(ContentType))
                response.ContentType = ContentType;
            else
                response.ContentType = "application/javascript";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Callback == null || Callback.Length == 0)
            {
                Callback = context.HttpContext.
                  Request.QueryString["callback"];
            }

            if (Data != null)
            {
                // The JavaScriptSerializer type was marked as obsolete
                // prior to .NET Framework 3.5 SP1 
#pragma warning disable 0618
                JavaScriptSerializer serializer =
                     new JavaScriptSerializer();
                string ser = serializer.Serialize(Data);
                response.Write(Callback + "(" + ser + ");");
#pragma warning restore 0618
            }
        }
    }

    public class JsonpFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(
                ActionExecutedContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            //
            // see if this request included a "callback" querystring
            // parameter
            //
            string callback = filterContext.HttpContext.
                      Request.QueryString["callback"];
            if (callback != null && callback.Length > 0)
            {
                //
                // ensure that the result is a "JsonResult"
                //
                JsonResult result = filterContext.Result as JsonResult;
                if (result != null)
                {
                    //throw new InvalidOperationException(
                    //    "JsonpFilterAttribute must be applied only " +
                    //    "on controllers and actions that return a " +
                    //    "JsonResult object.");


                    filterContext.Result = new JsonpResult
                    {
                        ContentEncoding = result.ContentEncoding,
                        ContentType = result.ContentType,
                        Data = result.Data,
                        Callback = callback
                    };
                }
            }
        }
    }
}
