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
using System.Data;
using BackAnd.Web.Api.Controllers.Admin;
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
        
        [Route("~/1/model/Validate")]
        [HttpPost]
        public IHttpActionResult Validate()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                JavaScriptSerializer jss = new JavaScriptSerializer();

                var data = jss.Deserialize<Dictionary<string, object>>(json);

                const string OldSchema = "oldSchema";
                
                if (!data.ContainsKey(OldSchema))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Missing oldSchema property."));
                }

                if (!IsOldModelEqualsToCurrent((ArrayList)data[OldSchema], GetBackandToObject()))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "The model has changed. Please reload the page and make your changes again."));

                }

                Dictionary<string, object> transformResult = Transform(jss, json, data, false);

                const string Alter = "alter";

                if (transformResult.ContainsKey(Alter))
                {
                    if (transformResult[Alter] is ArrayList)
                    {
                        ArrayList arrayList = (ArrayList)transformResult[Alter];
                        for (int i = 0; i< arrayList.Count; i++)
                        {
                            string sql = arrayList[i].ToString();
                            ValidateSql(sql);
                            if (arrayList[i] != null)
                                arrayList[i] = AdjustSql(sql);
                        }
                    }
                }
                return Ok(transformResult);
            }
            catch (WebException exception)
            {
                return ResponseMessage(Request.CreateResponse((HttpStatusCode)(int)exception.Status, exception.Message));

            }
            catch (Exception exception)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private bool IsOldModelEqualsToCurrent(ArrayList oldModel, ArrayList newModel)
        {
            return new ModelComparer().IsEquals(oldModel, newModel);
        }

        

        private void ValidateSql(string sql)
        {
            Fk fk = GetFk(sql);
            if (fk == null)
                return;

            ValidateVia(fk);
        }

        private void ValidateVia(Fk fk)
        {
            if (!Map.Database.Views.ContainsKey(fk.ParentTable))
                return;

            Durados.View parentView = Map.Database.Views[fk.ParentTable];

            if (!Map.Database.Views.ContainsKey(fk.ChildTable))
                return;

            Durados.View childView = Map.Database.Views[fk.ChildTable];

            if (parentView.DataTable.Columns.Contains(fk.ParentColumn) || parentView.Fields.ContainsKey(fk.ParentColumn))
            {
                throw new NewFkException(fk.ParentTable, fk.ParentColumn);
            }

            if (childView.DataTable.Columns.Contains(fk.ChildColumn) || childView.Fields.ContainsKey(fk.ChildColumn))
            {
                throw new NewFkException(fk.ChildTable, fk.ChildColumn);
            }
        }



        private Fk GetFk(string sql)
        {
            const string Constraint = "constraint";
            const string Foreign = "foreign";
            const string Key = "key";
            const char Space = ' ';
            const char SqlArgBoundery = '`';
            const char _ = '_';
            const char LeftBracket = '(';
            const char RightBracket = ')';
            const string Table = "table";
            const string References = "references";
            

            if (!(sql.Contains(Constraint) && sql.Contains(Foreign)))
                return null;

            string[] words = sql.Split(Space);

            Fk fk = new Fk();

            for (int i = 0; i < words.Length - 1; i++)
            {
                string word = words[i];
                string nextWord = words[i + 1];

                if (word.Equals(Table))
                {
                    fk.ChildTable = nextWord.Trim(SqlArgBoundery);
                }

                if (word.Equals(Constraint))
                {
                    fk.ParentColumn = nextWord.Split(_).Last();
                }

                if (word.Equals(Key))
                {
                    fk.ChildColumn = nextWord.Trim(LeftBracket).Trim(RightBracket).Trim(SqlArgBoundery);
                }

                if (word.Equals(References))
                {
                    fk.ParentTable = nextWord.Trim(SqlArgBoundery);
                }
            }

            return fk;
        }



        public class Fk
        {
            public string ParentTable { get; set; }
            public string ParentColumn { get; set; }
            public string ChildTable { get; set; }
            public string ChildColumn { get; set; }
        }

        private string AdjustSql(string sql)
        {
            sql = sql.Replace("int unsigned", "int(11)");
            sql = sql.Replace(" boolean,", " bit,");
            sql = sql.Replace(" boolean null", " bit null");
            sql = sql.Replace(" boolean)", " bit)");
            sql = sql.Replace(" boolean;", " bit;");
            sql = new Squeezer().Squeeze(sql);
            return sql;
        }

        [Route("~/1/model")]
        [HttpPost]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public IHttpActionResult Post(bool? firstTime = false)
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                string sql = string.Empty;

                Dictionary<string, object> transformResult = null;
                if (!IsDropAllTables(json))
                {
                    transformResult = Transform(json);

                    if (!transformResult.ContainsKey("alter"))
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, Messages.InvalidSchema + ": " + GetWarnings(transformResult)));

                    }

                    sql = string.Join(";", ((System.Collections.ArrayList)transformResult["alter"]).ToArray());
                    sql = AdjustSql(sql);
                }
                else
                {
                    sql = GetDtopAllTablesSQL();
                }

                if (!string.IsNullOrEmpty(sql))
                {

                    LogSQL(Map.AppName, json, sql);

                    try
                    {
                        runSql(sql);
                    }
                    catch (Exception exception)
                    {
                        UpdateLogModel(exception);
                        Sync();
                        throw exception;
                    }
                }

                Dictionary<string, object> syncResult = Sync();

                if (firstTime.HasValue && firstTime.Value)
                {
                    HandleFirstTime();
                }

                try
                {
                    OrderViewAndColumns((Dictionary<string, object>)transformResult["order"]);
                }
                catch (Exception exception)
                {
                    map.Logger.Log("model", "Post", Map.AppName + ": " + exception.Source, exception, 1, Request.RequestUri.ToString());

                }

                ArrayList backandToObjectResult = GetBackandToObject();

                return Ok(backandToObjectResult);

            }
            catch (WebException exception)
            {
                map.Logger.Log("model", "Post", Map.AppName + ": " + exception.Source, exception, 1, Request.RequestUri.ToString());
                return ResponseMessage(Request.CreateResponse((HttpStatusCode)(int)exception.Status, exception.Message));

            }
            catch (Exception exception)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
                map.Logger.Log("model", "Post", Map.AppName + ": " + exception.Source, exception, 1, Request.RequestUri.ToString());
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private bool IsDropAllTables(string json)
        {
            return json == "{\"newSchema\":[],\"severity\":0}";
        }

        private string GetDtopAllTablesSQL()
        {
            string sql = string.Empty;
            sql += "SET FOREIGN_KEY_CHECKS = 0;";

            foreach (Durados.View view in Map.Database.Views.Values.Where(v => !v.SystemView))
            {
                sql += "drop table if exists `" + view.DataTable.TableName + "`;";
            }

            sql += "SET FOREIGN_KEY_CHECKS = 1;";


            return sql;
        }

        private void HandleFirstTime()
        {
            try
            {
                AccountService.DefaultUsersTable.HandleFirstTime(Map);
                RefreshConfigCache(Map);
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("Model", "Post", "HandleFirstTime", exception, 1, null);
            }
        }
     

        private void LogSQL(string appName, string model, string sql)
        {
            try
            {
                int logType = -611;
                Guid guid = Guid.NewGuid();
                DateTime timestamp = DateTime.Now;
                int max = 4000;
                int modelParts = model.Length / max;
                int sqlParts = sql.Length / max;

                if (modelParts == 0 && sqlParts == 0)
                {
                    LogSQL(appName, model, sql, 0, timestamp, logType);
                    return;
                }

                int maxParts;
                if (modelParts >= sqlParts)
                {
                    maxParts = modelParts;
                }
                else
                {
                    maxParts = sqlParts;
                }

                string[] modelPartsArray = ChunksUpto(model, max).ToArray();
                string[] sqlPartsArray = ChunksUpto(sql, max).ToArray();

                for (int i = 0; i < maxParts; i++)
                {
                    string modelChunk = string.Empty;
                    string sqlChunk = string.Empty;

                    if (modelPartsArray.Length < i)
                    {
                        modelChunk = modelPartsArray[i];
                    }
                    if (sqlPartsArray.Length < i)
                    {
                        sqlChunk = sqlPartsArray[i];
                    }
                    LogSQL(guid.ToString(), modelChunk, sqlChunk, i, timestamp, logType);
                    
                }
            }
            catch { }
            //Math.Max()

        }

        [Route("~/1/model/last")]
        [HttpGet]
        public IHttpActionResult Last()
        {
            SqlAccess sa = new SqlAccess();

            Durados.Web.Mvc.View logView = (Durados.Web.Mvc.View)map.Database.Views["Durados_Log"];

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(logView.GetFieldByColumnNames("LogType").Name, "-611");
            int rowCount = 0;
            DataView dataView = logView.FillPage(1, 1, values, false, new Dictionary<string, SortDirection>() { { "ID", SortDirection.Desc } }, out rowCount, null, null);

            if (dataView.Count == 0)
                return NotFound();

            string guid = (string)dataView[0]["Action"];
            DateTime timestamp = (DateTime)dataView[0]["Time"];
            values = new Dictionary<string, object>();
            values.Add(logView.GetFieldByColumnNames("LogType").Name, "-611");
            values.Add(logView.GetFieldByColumnNames("Action").Name, guid);
            rowCount = 0;
            dataView = logView.FillPage(1, 1, values, false, new Dictionary<string, SortDirection>() { { "MethodName", SortDirection.Asc } }, out rowCount, null, null);

            string model = string.Empty;
            string sql = string.Empty;

            foreach (System.Data.DataRowView row in dataView)
            {
                if (!row.Row.IsNull("Trace"))
                {
                    model += row["Trace"].ToString();
                }

                if (!row.Row.IsNull("FreeText"))
                {
                    sql += row["FreeText"].ToString();
                }
            }

            return Ok(new { model = model, sql = sql, timestamp = timestamp });
        }

        private IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private void LogSQL(string guid, string model, string sql, int counter, DateTime timestamp, int logType)
        {
            map.Logger.Log("model", guid, counter.ToString(), "sql", model, logType, sql, timestamp);
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
            catch (DuplicateFieldException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, exception.Message));

            }
            catch (WebException exception)
            {
                return ResponseMessage(Request.CreateResponse((HttpStatusCode)(int)exception.Status, exception.Message));
 
            }
            catch (Exception exception)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
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
                viewView.Edit(new Dictionary<string, object>() { { "Order", 10000 + i * 10 } }, vpk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                ArrayList columns = (ArrayList)tableColumns[viewJsonName];

                for (int j = 0; j < columns.Count; j++)
                {
                    string columnName = columns[j].ToString();
                    Field field = view.GetFieldByColumnNames(columnName);
                    if (field != null)
                    {
                        string fieldName = field.Name;

                        string fpk = ca.GetFieldPK(viewName, fieldName, Map.GetConfigDatabase().ConnectionString);
                        fieldView.Edit(new Dictionary<string, object>() { { "Order", 500 + j * 10 } }, fpk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                    }
                }
            }

            RefreshConfigCache();
        }

        private Dictionary<string, object> Sync()
        {
            Dictionary<string, object> response = (new Sync()).AddNewViewsAndSyncAll(Map);

            EmitMessage("ModelChanged", new {changedBy = Map.Database.GetCurrentUsername()});

            return response;
        }

        private void EmitMessage(string eventName, object data)
        {
            Backand.socket socket = new Backand.socket();
            string appName = (System.Web.HttpContext.Current.Items[Durados.Database.AppName] ?? string.Empty).ToString();
               System.Threading.ThreadPool.QueueUserWorkItem(delegate
                {
                    try
                    {
                            socket.emitRole(eventName, data, "Admin", appName);
                        
                    }
                    catch (Exception exception)
                    {
                        Database.Logger.Log("model", "socket", "emit", exception, 1, eventName);
                    }
                });
            
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
    public class Squeezer
    {
        string _bkname_ = "_bkname_";

        string addConstraint = "add constraint";
        string foreignKey = "foreign key";
            
        public string Squeeze(string sql)
        {
            if (!sql.Contains(addConstraint))
                return sql;

            int limit = 64;
            string[] foreignKeys = GetFk(sql);
            string[] longForeignKeys = GetLongFk(foreignKeys, limit);
            Dictionary<string, string> shortForeignKeys = ShortenForeignKeys(longForeignKeys);
            foreach (string longForeignKey in shortForeignKeys.Keys)
            {
                sql = sql.Replace(longForeignKey, shortForeignKeys[longForeignKey]);
            }

            return sql;
        }

        string[] GetFk(string sql)
        {
            List<string> foreignKeys = new List<string>();

            string[] s1 = sql.Split(new string[] { addConstraint }, StringSplitOptions.None);
            foreach (string s in s1)
            {
                string[] s2 = s.Split(new string[] { foreignKey }, StringSplitOptions.None);
                string fk = s2.FirstOrDefault();
                if (fk.Contains(_bkname_))
                {
                    foreignKeys.Add(fk.Trim());
                }
            }

            return foreignKeys.ToArray();
        }

        string[] GetLongFk(string[] foreignKeys, int len)
        {
            List<string> longForeignKeys = new List<string>();

            foreach (string fk in foreignKeys)
            {
                if (fk != null && fk.Length >= len)
                {
                    longForeignKeys.Add(fk);
                }
            }

            return longForeignKeys.ToArray();
        }

        Dictionary<string, string> ShortenForeignKeys(string[] longForeignKeys)
        {
            Dictionary<string, string> shortForeignKeys = new Dictionary<string, string>();

            foreach (string longForeignKey in longForeignKeys)
            {
                shortForeignKeys.Add(longForeignKey, ShortenForeignKey(longForeignKey));
            }


            return shortForeignKeys;
        }

        string ShortenForeignKey(string longForeignKey)
        {
            return "b" + Guid.NewGuid().ToString().Split('-').First() + _bkname_ + longForeignKey.Split(new string[] { _bkname_ }, StringSplitOptions.None).LastOrDefault();
        }


    }
}
