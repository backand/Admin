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
    public class viewController : RestController
    {
        //
        // GET: /v1/

       

        #region data


        [System.Web.Mvc.ActionName("data")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Get(string name, string pk, bool? deep, bool? withSelectOptions, int? pageNumber, int? pageSize, string filter, string sort, bool descriptive = true)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return new ViewNameWasNotSuppliedApiHttpException();
                }
                View view = GetView(name);
                if (view == null)
                {
                    return new ViewNotFoundApiHttpException(name, Response);
                }

                if (!string.IsNullOrEmpty(pk))
                {

                    var item = RestHelper.Get(view, pk, deep ?? false, view_BeforeSelect, view_AfterSelect, descriptive);

                    if (item == null)
                    {

                        return new ItemNotFoundInViewApiHttpException(pk, name, Response);
                    }

                    Response.StatusCode = (int)HttpStatusCode.OK;

                    return Json(item, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int rowCount = 0;

                    Dictionary<string, object>[] filterArray = null;
                    if (!string.IsNullOrEmpty(filter))
                    {
                        filterArray = JsonConverter.DeserializeArray(filter);
                    }

                    Dictionary<string, object>[] sortArray = null;
                    if (!string.IsNullOrEmpty(sort))
                    {
                        sortArray = JsonConverter.DeserializeArray(sort);
                    }

                    var items = RestHelper.Get(view, withSelectOptions ?? false, false, pageNumber ?? 1, pageSize ?? 20, filterArray, null, sortArray, out rowCount, false, view_BeforeSelect, view_AfterSelect);

                    Response.StatusCode = (int)HttpStatusCode.OK;

                    return Json(items, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                return UnexpectedException(exception, Response);

            }
        }

        const string jsonDataKey = "data";

        [System.Web.Mvc.ActionName("data")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Put)]
        public ActionResult Update(string name, string pk, bool? deep)
        {
            try
            {
                
                if (string.IsNullOrEmpty(pk))
                {
                    //throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.NotFound);
                    return new ItemNotFoundInViewApiHttpException(pk, name, Response);
                }

                //var item = GetItemFromRequest();
                if (string.IsNullOrEmpty(name))
                {
                    return new ViewNameWasNotSuppliedApiHttpException();
                }
                View view = GetView(name);
                if (view == null)
                {
                    return new ViewNotFoundApiHttpException(name, Response);
                }

                var json = GetJsonFromRequest(jsonDataKey);
                
                view.Update(json, pk, deep ?? false, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);


                Response.StatusCode = (int)HttpStatusCode.OK;


                return Json(string.Empty);
            }
            catch (Exception exception)
            {
                //string uri = GetErrorUri(exception);
                //Response.Headers.Add("Location", uri);
                //throw new System.Web.Http.HttpResponseException(HttpStatusCode.SeeOther);
                return UnexpectedException(exception, Response);

            }
        }

        [System.Web.Mvc.ActionName("data")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create(string name, bool? deep)
        {
            try
            {
                var item = GetItemFromRequest();

                string uri = ApiVersion + Slash + GetControllerName() + Slash + GetActionName() + Slash + item["numericField"].ToString();
                Response.Headers.Add("Location", uri);
                
                Response.StatusCode = (int)HttpStatusCode.Created;


                return Json(item);
            }
            catch (Exception exception)
            {
                string uri = GetErrorUri(exception);
                Response.Headers.Add("Location", uri);
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.SeeOther));
            }
        }


       
        [System.Web.Mvc.ActionName("data")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Delete)]
        public JsonResult Delete(string viewName, string pk, bool? deep)
        {
            try
            {
                if (string.IsNullOrEmpty(pk))
                {
                    throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
                }

                Response.StatusCode = (int)HttpStatusCode.OK;

                return Json(string.Empty);
            }
            catch (Exception exception)
            {
                 string uri = GetErrorUri(exception);
                Response.Headers.Add("Location", uri);
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.SeeOther));

             
            }
        }

        #endregion data

        #region config

        [System.Web.Mvc.ActionName("config")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Get(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return new ViewNameWasNotSuppliedApiHttpException();

                }
                View view = GetView(name);
                if (view == null)
                {
                    return new ViewNotFoundApiHttpException(name, Response);
                }

                ConfigAccess ConfigAccess = new ConfigAccess();
                var pk = ConfigAccess.GetViewPK(name, Map.GetConfigDatabase().ConnectionString);
                var item = RestHelper.Get(GetView("View"), pk, true, view_BeforeSelect, view_AfterSelect);

                Response.StatusCode = (int)HttpStatusCode.OK;

                return Json(item, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return new UnexpectedApiHttpException(exception, Response);

            }
           
        }

        [System.Web.Mvc.ActionName("config")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Put)]
        public JsonResult Update(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
                }

                var item = GetItemFromRequest();

                Response.StatusCode = (int)HttpStatusCode.OK;


                return Json(item);
            }
            catch (Exception exception)
            {
                string uri = GetErrorUri(exception);
                Response.Headers.Add("Location", uri);
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.SeeOther));


            }

            
        }

        #endregion metadata

           
    }
    public class ViewNotFoundApiHttpException : NotFoundException
    {
        public ViewNotFoundApiHttpException(string name, HttpResponseBase response) : base(string.Format("The view '{0}' was not found in the views collection.", name), response) { }
    }

    public class ViewNameWasNotSuppliedApiHttpException : ApiHttpException
    {
        public ViewNameWasNotSuppliedApiHttpException() : base(HttpStatusCode.BadRequest, "The view name parameter was not supplied.") { }
    }

    public class ItemNotFoundInViewApiHttpException : NotFoundException
    {
        public ItemNotFoundInViewApiHttpException(string id, string name, HttpResponseBase response) : base(string.Format("Data with id '{0}' was not found in view '{1}'.", id, name), response) { }
    }

    
}
