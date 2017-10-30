using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Durados.Web.Mvc;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Newtonsoft.Json;

namespace BackAnd.AspNetCore.Api.Controllers
{
    [Produces("application/json")]
    [Route("1/objects")]
    public class ViewDataController : ApiController
    {
        string where = null;
        bool isNosqlFilter = false;

        private bool IsNosqlFilter(string filter)
        {
            return !string.IsNullOrEmpty(filter) && filter.Contains("\"q\":");
        }

        private Dictionary<string, object> GetWhereFromCache(View view, string json)
        {
            return NoSqlFilterCache.Get(Map.AppName, view, json);
        }

        private string GetWhere(View view, string json, bool useCache = false)
        {
            if (useCache)
            {
                try
                {
                    var cacheResponse = GetWhereFromCache(view, json);

                    if (cacheResponse != null)
                    {
                        ReplaceVariables(cacheResponse);
                        return cacheResponse["where"].ToString();
                    }
                }
                catch { }
            }

            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            if (!(data.Count == 1 && data.ContainsKey("q")))
            {
                throw new Durados.DuradosException("filter must start with \"q\"");
            }

            if (!data.ContainsKey("object"))
            {
                var data2 = new Dictionary<string, object>();
                data2.Add("object", view.JsonName);
                data2.Add("q", data["q"]);
                data = data2;
            }


            if (!data.ContainsKey("json"))
            {
                var data2 = new Dictionary<string, object>();
                data2.Add("json", data);
                data = data2;
            }

            data.Add("isFilter", true);
            
            json = JsonConvert.SerializeObject(data);

            var response = transformJson(json, useCache);

            if (!response.ContainsKey("where"))
            {
                throw new Durados.DuradosException("Failed to transform nosql filter");
            }

            if (useCache)
            {
                SetWhereToCache(view, json, response);
            }

            if (useCache)
            {
                ReplaceVariables(response);
            }


            return response["where"].ToString();

        }


        // GET: api/viewData
        [HttpGet("{name}")]
        public ObjectResult Get(string name, bool? withSelectOptions = null, bool? withFilterOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null, bool? deep = null, bool descriptive = true, bool? relatedObjects = false, string exclude = null, string fields = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return NotFound(Messages.ViewNameIsMissing);
                }
                View view = GetView(name);
                if (view == null)
                {
                    return NotFound(string.Format(Messages.ViewNameNotFound, name));
                }
                if (!IsAllow(view))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, Messages.ViewIsUnauthorized);
                }

                int rowCount = 0;

                Dictionary<string, object>[] filterArray = null;
                isNosqlFilter = IsNosqlFilter(filter);
                if (isNosqlFilter)
                {
                    where = GetWhere(view, filter);
                }

                else if (!string.IsNullOrEmpty(filter) && filter != "filter" && filter != "false" && filter != "null" && filter != "undefined" && filter != "[{}]")
                {
                    if (filter == "(Collection)")
                    {
                        filter = "[" + System.Web.HttpContext.Current.Request.Params["filter"] + "]";
                    }
                    if (filter.StartsWith("{"))
                    {
                        filter = "[" + filter + "]";
                    }
                    try
                    {
                        filterArray = JsonConverter.DeserializeArray(filter);
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "get", exception.Source, exception, 1, "Deserialize filter " + filter + ", original: " + System.Web.HttpContext.Current.Request.Params["filter"]);
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFilter));
                    }
                }

                Dictionary<string, object>[] sortArray = null;

                if (!string.IsNullOrEmpty(sort) && sort != "sort" && sort != "false" && sort != "null" && sort != "undefined" && sort != "[{}]")
                {
                    if (sort == "(Collection)")
                    {
                        sort = "[" + System.Web.HttpContext.Current.Request.Params["sort"] + "]";
                    }
                    if (sort.StartsWith("{"))
                    {
                        sort = "[" + sort + "]";
                    }
                    try
                    {
                        sortArray = JsonConverter.DeserializeArray(sort);
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "get", exception.Source, exception, 1, "Deserialize sort " + sort + ", original: " + System.Web.HttpContext.Current.Request.Params["sort"]);
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifySort));
                    }
                }

                if (search == "null" || search == "undefined")
                    search = null;

                string[] fieldsArray = null;
                if (!string.IsNullOrEmpty(fields) && fields != "fields" && fields != "all" && fields != "false" && fields != "null" && fields != "undefined" && fields != "[]")
                {
                    if (!(fields.Contains('\'') || fields.Contains('\"')))
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFields));
                    }
                    try
                    {
                        fieldsArray = JsonConverter.DeserializeStringArray(fields);
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "get", exception.Source, exception, 1, "Deserialize fields " + fields + ", original: " + System.Web.HttpContext.Current.Request.Params["fields"]);
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFields));
                    }
                }


                bool hideMetadata = exclude != null && exclude.ToLower().Contains("metadata");
                bool hideTotalRows = exclude != null && exclude.ToLower().Contains("totalrows");
                var items = RestHelper.Get(view, withSelectOptions ?? false, withFilterOptions ?? false, pageNumber ?? 1, pageSize ?? view.Database.DefaultPageSize, filterArray, search, sortArray, out rowCount, deep ?? false, view_BeforeSelect, view_AfterSelect, false, descriptive, false, relatedObjects ?? false, where, hideMetadata, hideTotalRows, fieldsArray);

                return Ok(items);

            }
            catch (FilterException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, exception.Message));
            }
            catch (SortException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, exception.Message));
            }
            catch (System.Data.Common.DbException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Check the security Pre-defined Filter for the following sql errors: " + exception.Message));

            }
            catch (AggregateException exception)
            {
                if (exception.InnerException != null)
                    throw new BackAndApiUnexpectedResponseException(exception.InnerException, this);
                else
                    throw new BackAndApiUnexpectedResponseException(exception, this);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        // GET: api/viewData/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/viewData
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/viewData/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
