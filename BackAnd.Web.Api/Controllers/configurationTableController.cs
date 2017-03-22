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
using System.Reflection;
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
    public class configurationTableController : wfController
    {
         //[Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            return GetItem(id,true);
        }

        protected IHttpActionResult GetItem(string id,bool deep)
        {
            try
            {
                if (!IsAdmin())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                var item = RestHelper.Get(GetView(GetConfigViewName()), id, deep, view_BeforeSelect, view_AfterSelect);

                if (item == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, GetView(GetConfigViewName()).DisplayName)));
                }

                return Ok(item);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }
        protected virtual string GetConfigViewName()
        {
            return null;
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get(bool? withSelectOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null)
        {

            return GetList(withSelectOptions, pageNumber, pageSize, filter, sort, search);
        }

        protected virtual IHttpActionResult GetList(bool? withSelectOptions, int? pageNumber, int? pageSize, string filter, string sort, string search)
        {
            try
            {
                if (!IsAdmin())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                View view = (View)Map.GetConfigDatabase().Views[GetConfigViewName()];

                int rowCount = 0;

                Dictionary<string, object>[] filterArray = null;
                if (!string.IsNullOrEmpty(filter))
                {
                    filterArray = JsonConverter.DeserializeArray(filter);
                }

                if (filterArray != null)
                {
                    AdjustFilterItem(filterArray);
                }

                Dictionary<string, object>[] sortArray = null;
                if (!string.IsNullOrEmpty(sort))
                {
                    sortArray = JsonConverter.DeserializeArray(sort);
                }

                if (sortArray != null)
                {
                    AdjustSortItem(sortArray);
                }

                var items = RestHelper.Get(view, withSelectOptions ?? false, false, pageNumber ?? 1, pageSize ?? 20, filterArray, search, sortArray, out rowCount, false, view_BeforeSelect, view_AfterSelect);

                AdjustItems(items, filterArray, sortArray);
               
                return Ok(items);

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        protected virtual void AdjustItems(object items, Dictionary<string, object>[] filterArray, Dictionary<string, object>[] sortArray)
        {
            
        }

        protected virtual void AdjustSortItem(Dictionary<string, object>[] sortArray)
        {
          
        }

        protected virtual void AdjustFilterItem(Dictionary<string, object>[] filterArray)
        {
          
        }

       
         [Route("{id}")]
         [HttpPut]
        public virtual IHttpActionResult Put(string id)
        {
            try
            {

                View view = GetView(GetConfigViewName());

                if (!IsAdmin())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                Dictionary<string, object> values = view.Deserialize(json);

                values = GetAdjustedValues(view, values);

                var result = ValidateInputForUpdate(id, view, values);
                if (result != null)
                    return result;
                view.Edit(values, id, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                HandelChildViewsForUpdate(id, values);

                RefreshConfigCache();

                var item = RestHelper.Get(view, id, IsDeep(), view_BeforeSelect, view_AfterSelect, false, true);


                if (item == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.RuleNotFound)));
                }

                return Ok(item);

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

         protected virtual bool IsDeep()
         {
             return true;
         }

        protected virtual void HandelChildViewsForUpdate(string id, Dictionary<string, object> values)
        {
           
        }

        protected virtual IHttpActionResult ValidateInputForUpdate(string id, View view, Dictionary<string, object> values)
        {
            return null;
        }

       
        protected virtual bool IsAllow(View view)
        {
            if (view.SystemView && view.Database.Map.IsMainMap)
            {
                return false;
            }
            return view.IsAllow();
        }

     
        [Route("")]
        public virtual IHttpActionResult Post()
        {
            try
            {
                View view = GetView(GetConfigViewName());

                if (!IsAdmin())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                Dictionary<string, object> values = view.Deserialize(json);

                values = GetAdjustedValues(view, values);

                var result= ValidateInputForPost(view, values);
                if (result != null)
                    return result;
                System.Data.DataRow row = view.Create(values, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);
                
                var pk = row["ID"].ToString();

                HandleDataBeforeCache(pk, row);
                
                HandelChiledViewsForPost(values, pk);

                RefreshConfigCache();

                var item = RestHelper.Get(view, pk, IsDeep(), view_BeforeSelect, view_AfterSelect, false, true);

                return Ok(item);

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        protected virtual void HandleDataBeforeCache(string pk, System.Data.DataRow row)
        {
            
        }

        protected virtual void HandelChiledViewsForPost(Dictionary<string, object> values, string pk)
        {

        }

        protected virtual IHttpActionResult ValidateInputForPost(View view, Dictionary<string, object> values)
        {
            return null;
        }

        
        protected override void AfterCreateAfterCommit(Durados.CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);
            if (Map.Database.AutoCommit)
            {
                RefreshConfigCache();
            }
        }

        protected override void AfterDeleteAfterCommit(Durados.DeleteEventArgs e)
        {
            base.AfterDeleteAfterCommit(e);
            if (Map.Database.AutoCommit)
            {
                RefreshConfigCache();
            }
        }

        protected override void AfterEditAfterCommit(Durados.EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);
            if (Map.Database.AutoCommit)
            {
                RefreshConfigCache();
            }
        }
        
        [Route("{id}")]
        public virtual IHttpActionResult Delete(string id)
        {
            try
            {
                View view = GetView(GetConfigViewName());

                if (!IsAdmin())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                var item = RestHelper.Get(view, id, true, view_BeforeSelect, view_AfterSelect, false, true);


                if (item == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.RuleNotFound)));
                }



                view.Delete(id, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit);
                RefreshConfigCache();

                return Ok(new { __metadata = new { id = id } });

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

         

    }

}
