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
    public class modelController : apiController
    {
        [Route("~/1/model")]
        [HttpPost]
        public IHttpActionResult Post()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);


                Dictionary<string, object> transformResult = Transform(json);

                string sql = string.Join(";", ((System.Collections.ArrayList)transformResult["alter"]).ToArray());

                runSql(sql);

                Dictionary<string, object> syncResult = Sync();

                try
                {
                    OrderViewAndColumns((Dictionary<string, object>)transformResult["order"]);
                }
                catch (Exception exception)
                {
                    map.Logger.Log("model", "OrderViewAndColumns", Map.AppName + ": " + exception.Source, exception, 1, Request.RequestUri.ToString());

                }

                ArrayList backandToObjectResult = GetBackandToObject();

                return Ok(backandToObjectResult);
                
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }


        [Route("~/1/model")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                ArrayList backandToObjectResult = GetBackandToObject();

                return Ok(backandToObjectResult); 
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private ArrayList GetBackandToObject()
        {
            return GetBackandToObject(Request.Headers.Authorization.ToString());
        }



        private ArrayList GetBackandToObject(string token)
        {
            if (CountViews() == 0)
                return new ArrayList();

            string getNodeUrl = GetNodeUrl() + "/json";

            bulk bulk = new Durados.Web.Mvc.UI.Helpers.bulk();

            
            var tasks = new List<Task<string>>();
            object responses = null;
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var responseStatusAndData = bulk.GetWebResponse("POST", getNodeUrl, null, null, new Dictionary<string, object>() { { "Content-Type", "application/json" }, { "Authorization", Request.Headers.Authorization.ToString() } }, 0);
                responses = responseStatusAndData.data;
                return responseStatusAndData.data;
            }));

            Task.WaitAll(tasks.ToArray());

            JavaScriptSerializer jss = new JavaScriptSerializer();
            var result = jss.Deserialize<ArrayList>(responses.ToString());

            return result;
        }

        private void OrderViewAndColumns(Dictionary<string, object> transformResult)
        {
            ArrayList tables = (ArrayList)transformResult["tables"];
            Dictionary<string, object> tableColumns = (Dictionary<string, object>)transformResult["columns"];

            ConfigAccess ca = new ConfigAccess();

            Durados.View viewView = map.GetConfigDatabase().Views["View"];
            Durados.View fieldView = map.GetConfigDatabase().Views["Field"];

            for (int i = 0; i < tables.Count; i++)
            {
                string viewJsonName = tables[i].ToString();
                Durados.View view = Map.Database.GetViewByJsonName(viewJsonName);
                string viewName = view.Name;

                string vpk = ca.GetViewPK(viewJsonName, Map.GetConfigDatabase().ConnectionString);
                viewView.Edit(new Dictionary<string, object>() { { "Order", i * 10 } }, vpk, null, null, null, null);

                ArrayList columns = (ArrayList)tableColumns[viewJsonName];

                for (int j = 0; j < columns.Count; j++)
                {
                    string fieldJsonName = columns[j].ToString();
                    string fieldName = view.GetFieldsByJsonName(fieldJsonName)[0].Name;

                    string fpk = ca.GetFieldPK(viewName, fieldName, Map.GetConfigDatabase().ConnectionString);
                    fieldView.Edit(new Dictionary<string, object>() { { "Order", j * 10 } }, fpk, null, null, null, null);

                }
            }

            RefreshConfigCache();
        }

        private Dictionary<string, object> Sync()
        {
            return (new Sync()).AddNewViewsAndSyncAll(Map);
        }

        private int CountViews()
        {
            return Map.Database.Views.Values.Where(v => !v.SystemView).Count();
        }
        private Dictionary<string, object> Transform(string json)
        {
            string getNodeUrl = GetNodeUrl() + "/transform";

            bulk bulk = new Durados.Web.Mvc.UI.Helpers.bulk();

            JavaScriptSerializer jss = new JavaScriptSerializer();

            var data = jss.Deserialize<Dictionary<string, object>>(json);

            if (!data.ContainsKey("oldSchema"))
            {
                data.Add("oldSchema", "");
            }

            data["oldSchema"] = GetBackandToObject();

            json = jss.Serialize(data);

            var tasks = new List<Task<string>>();
            object responses = null;
            tasks.Add(Task.Factory.StartNew(() =>
            {
                //, { "Authorization", Request.Headers.Authorization.ToString() }
                var responseStatusAndData = bulk.GetWebResponse("POST", getNodeUrl, json, null, new Dictionary<string, object>() { { "Content-Type", "application/json" } }, 0);
                responses = responseStatusAndData.data;
                return responseStatusAndData.data;
            }));

            Task.WaitAll(tasks.ToArray());

            var result = jss.Deserialize<Dictionary<string, object>>(responses.ToString());

            return result;
            
        }

        private string GetNodeUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";
           
        }

        private SqlAccess GetSqlAccess(Durados.SqlProduct sqlProduct)
        {
            SqlAccess sqlAccess = null;

            if (sqlAccess == null)
            {

                switch (sqlProduct)
                {
                    case SqlProduct.MySql:
                        sqlAccess = new MySqlAccess();
                        break;
                    case SqlProduct.Postgre:
                        sqlAccess = new PostgreAccess();
                        break;
                    case SqlProduct.Oracle:
                        sqlAccess = new OracleAccess();
                        break;

                    default:
                        sqlAccess = new SqlAccess();
                        break;
                }
            }

            return sqlAccess;
        }

        private void runSql(string sql)
        {
            string connectionString = Map.connectionString;

            SqlAccess sqlAccess = GetSqlAccess(Map.SqlProduct);

            sqlAccess.ExecuteNonQuery(connectionString, sql, Map.SqlProduct, null, null);
        }
    }

}
