﻿using System;
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
using Durados.Web.Mvc;
using System.Net.Http.Headers;
using Durados.Web.Mvc.Controllers.Api;
using System.Text.RegularExpressions;
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
    [RoutePrefix("admin/myAppKeys")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class keysController : apiController
    {
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
            }

            try
            {
                return Ok(RestHelper.GetKeys(id));


            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        [Route("reset/{id}/{key}")]
        [HttpGet]
        public IHttpActionResult reset(string id, string key)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
            }

            try
            {
                return Ok(RestHelper.ResetKey(id, key));


            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

    }

    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class myAppsController : apiController
    {
        const string AppViewName = "durados_App";

        protected override View GetView(string viewName)
        {
            return (View)Maps.Instance.DuradosMap.Database.Views[AppViewName];
        }


        
        public IHttpActionResult Get(string id = null, bool? deep = null, bool? withSelectOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null)
        {
            if (id != null)
                return Get(id, deep);
            else
                return Get(withSelectOptions, pageNumber, pageSize, filter, sort, search);
        }
        private IHttpActionResult Get(string id = null, bool? deep = null)
        {
            try
            {
                View view = GetView(null);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, AppViewName)));
                }

                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                if (!view.IsAllow())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                int? appId = Maps.Instance.AppExists(id, Convert.ToInt32(Maps.Instance.DuradosMap.Database.GetUserID()));
                if (!appId.HasValue)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.AppNotFound, id)));
                }

                SetRequestItemCurrentAppName(id);

                Map.Logger.Log(GetControllerNameForLog(ControllerContext), "myApp", "before get item", null, 5, null);
 
                var item = RestHelper.Get(view, appId.Value.ToString(), deep ?? false, view_BeforeSelect, view_AfterSelect);

                Map.Logger.Log(GetControllerNameForLog(ControllerContext), "myApp", "after get item", null, 5, null);
 
                if (item == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.AppNotFound, id)));
                }

                if (id != Maps.DuradosAppName && !BlobExists(id, appId.ToString()))
                {
                    if (item["DatabaseStatus"].Equals(1))
                    {
                        item["DatabaseStatus"] = 2;
                        Maps.Instance.DuradosMap.Logger.Log("uue", "myApps", "appName: " + id, null, -754, "141");
                    }
                    return Ok(item);
                }

                View databaseView = null;
                try
                {
                    databaseView = (View)Maps.Instance.GetMap(id).GetConfigDatabase().Views["Database"];
                }
                catch
                {

                }

                if (databaseView == null)
                {
                    item.Add("settings", "");
                }
                else
                {
                    Dictionary<string, object> db = null;

                    try
                    {
                        db = RestHelper.Get(databaseView, "0", true, view_BeforeSelect, view_AfterSelect, true, true);
                    }
                    catch
                    {
                        if (item["DatabaseStatus"].Equals(2))
                        {
                            return Ok(item);
                        }
                        else
                        {
                            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.AppNotFound, id)));
                        }
                    }

                    item.Add("settings", db);
                }

                item.Add("connectionSource", RestHelper.GetConnectionSource(id));


                if (deep.HasValue && deep.Value && item["DatabaseStatus"].Equals(1))
                {
                    RestHelper.AddStat(item, id);
                }
                else
                {
                    if (item["DatabaseStatus"].Equals((int)OnBoardingStatus.Error))
                    {
                        try
                        {
                            Map map = Maps.Instance.GetMap(id);
                            if (map != null)
                            {
                                if (map.Database.TestConnection())
                                {
                                    Maps.Instance.UpdateOnBoardingStatus(OnBoardingStatus.Ready, appId.Value.ToString());
                                }
                                //else
                                //{
                                //    Maps.Instance.Restart(id);
                                //    if (Map.Database.TestConnection())
                                //    {
                                //        Maps.Instance.UpdateOnBoardingStatus(OnBoardingStatus.Ready, appId.Value.ToString());
                                //    }
                                //}
                            }
                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log("Map", "Initiate", "UpdateOnBoardingStatus", exception, 1, "Failed to Update OnBoarding Status");
                        }
                    }
                }

                return Ok(item);


            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private bool BlobExists(string appName, string appId)
        {
            if (Maps.Instance.AppInCach(appName))
                return true;

            bool blobExists = (new Durados.Web.Mvc.Azure.DuradosStorage()).Exists(Maps.GetConfigPath(Maps.DuradosAppPrefix + appId.ToString() + ".xml"));

            if (!blobExists)
                return false;

            return true;
        }

        private IHttpActionResult Get(bool? withSelectOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null)
        {
            try
            {
                View view = GetView(null);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, AppViewName)));
                }
                if (!view.IsAllow())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ViewIsUnauthorized));
                }

                int rowCount = 0;

                Dictionary<string, object>[] filterArray = null;
                if (!string.IsNullOrEmpty(filter) && filter != "filter" && filter != JsonNull)
                {
                    filterArray = JsonConverter.DeserializeArray(filter);
                }

                Dictionary<string, object>[] sortArray = null;
                if (!string.IsNullOrEmpty(sort) && sort != JsonNull)
                {
                    sortArray = JsonConverter.DeserializeArray(sort);
                }

                var items = RestHelper.Get(view, withSelectOptions ?? false, false, pageNumber ?? 1, pageSize ?? 20, filterArray, search, sortArray, out rowCount, false, view_BeforeSelect, view_AfterSelect);

                return Ok(items);

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        public virtual IHttpActionResult Post()
        {
            try
            {
                View view = GetView(null);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, AppViewName)));
                }
                if (!view.IsCreatable() && !view.IsDuplicatable())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                Dictionary<string, object> values = RestHelper.Deserialize(view, json);

                const string Creator = "Creator";
                const string DatabaseStatus = "DatabaseStatus";
                const string Name = "Name";
                const string Title = "Title";

                if (values.ContainsKey(Name))
                {
                    try
                    {
                        values[Name] = GetCleanName(values[Name].ToString());
                    }
                    catch (Exception)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, Messages.AppNameInvalid));
                    }
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, Messages.AppNameCannotBeNull));
                }
                if (!values.ContainsKey(Title))
                {
                    values.Add(Title,values[Name]);
                }
                else if (values[Title] == null || values[Title].Equals(string.Empty))
                {
                    values[Title] = values[Name];
                }
                if (values.ContainsKey(Creator))
                {
                    values.Remove(Creator);
                }
                values.Add(Creator, view.Database.GetUserID());
                values.Add(DatabaseStatus, (int)OnBoardingStatus.NotStarted);

                

                string key = view.Create(values, false, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);

               

                return Ok(new { __metadata = new { id = key } });
            }
            catch (System.Data.SqlClient.SqlException exception)
            {
                const int DuplicateUniqueIndex = 2601;
                if (exception.Number == DuplicateUniqueIndex)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, Messages.AppNameAlreadyExists));
                }
                else
                {
                    throw new BackAndApiUnexpectedResponseException(exception, this);
                }
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        protected string GetCleanName(string name)
        {
            name = name.Trim();

            if (name.ToLower().Equals("www"))
                throw new Durados.DuradosException(Map.Database.Localizer.Translate("This name already exists."));

            Regex regex = new Regex("^[A-Za-z0-9\\-]+$");/**\\- support - inside url*/
            if (!regex.IsMatch(name))
            {
                throw new Durados.DuradosException(Map.Database.Localizer.Translate("The 'Name' is the first part of the URL of the Console.\n\rOnly alphanumeric characters are allowed."));
            }

            if (name.Length > Maps.AppNameMax)
                throw new Durados.DuradosException(Map.Database.Localizer.Translate("The 'Name' is the first part of the URL of the Console.\n\rMaximum 25 characters are allowed."));


            return name.ToLower();
        }

        Dictionary<string, object> databaseSettings = null;
        string appName = null;
        string newAppName = null;
        public virtual IHttpActionResult Put(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                View view = GetView(null);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, AppViewName)));
                }
                if (!view.IsEditable())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                int? appId = Maps.Instance.AppExists(id, Convert.ToInt32(Maps.Instance.DuradosMap.Database.GetUserID()));
                if (!appId.HasValue)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, AppViewName)));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                Dictionary<string, object> values = RestHelper.Deserialize(view, json);
                if (values.ContainsKey("settings") && values["settings"] is Dictionary<string, object>)
                    databaseSettings = (Dictionary<string, object>)values["settings"];

                appName = id;

                if (values.ContainsKey("Name") && values["Name"].ToString() != appName)
                {
                    newAppName = GetCleanName(values["Name"].ToString());
                    if (string.IsNullOrEmpty(newAppName))
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "The app Name cannot be empty"));

                    if (Maps.Instance.AppExists(newAppName).HasValue)
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "An app by this name already exists"));
                }

                view.Update(values, appId.Value.ToString(), false, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        protected override void AfterEditBeforeCommit(Durados.EditEventArgs e)
        {
            base.AfterEditBeforeCommit(e);
            try
            {
                if (databaseSettings != null && appName != null)
                {
                    Map map = Maps.Instance.GetMap(appName);
                    if (System.Web.HttpContext.Current.Items.Contains(Durados.Database.AppName))
                        System.Web.HttpContext.Current.Items[Durados.Database.AppName] = appName;
                    else
                        System.Web.HttpContext.Current.Items.Add(Durados.Database.AppName, appName);

                    View view = (View)map.GetConfigDatabase().Views["Database"];
                    view.Edit(GetAdjustedValues(view, databaseSettings), "0", view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                    if (newAppName != null)
                    {
                        map.AppName = newAppName;
                    }
                    RefreshConfigCache(map);
                    if (!string.IsNullOrEmpty(newAppName) && !appName.Equals(newAppName))
                        Maps.Instance.Rename(appName, newAppName);
                }
            }
            catch (Exception exception)
            {
                e.Cancel = true;
                throw new Durados.DuradosException("Failed to update database configuration", exception);
                
            }
        }


        protected override void AfterEditAfterCommit(Durados.EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);
            if (e.Values != null && e.Values.ContainsKey("Name"))
            {
                MapDataSet.durados_AppRow appRow = (MapDataSet.durados_AppRow)e.PrevRow;
                string oldName = appRow.Name;
                string newName = e.Values["Name"].ToString();
                if (!oldName.Equals(newName))
                {
                    //Maps.Instance.ChangeName(oldName, newName);
                    //CreateDns(newName);
                    Maps.Instance.Restart(oldName);
                }

                //SqlProduct product = Maps.GetSqlProduct(newName);

                //if (product == SqlProduct.MySql)
                //{
                //    string url = Maps.GetAppUrl(newName);
                //    string[] split = url.Split(':');
                //    url = split[0] + ":" + split[1] + ":" + Maps.ProductsPort[product] + "/Admin/Restart?id=" + Map.Database.GetUserGuid();

                //    Infrastructure.Http.CallWebRequest(url);

                //}
                //else
                //{
                    //Maps.Instance.Restart(oldName);
                //}
            }
        }

        protected override void BeforeEdit(Durados.EditEventArgs e)
        {
            if (e.Values.ContainsKey("Name"))
            {
                MapDataSet.durados_AppRow appRow = (MapDataSet.durados_AppRow)e.PrevRow;
                string oldName = appRow.Name;
                string newName = GetCleanName(e.Values["Name"].ToString());
                if (!oldName.Equals(newName))
                {
                    string urlFieldName = "Url";
                    if (e.Values.ContainsKey(urlFieldName))
                        e.Values[urlFieldName] = e.Values[urlFieldName].ToString().Replace("appName=" + oldName, "appName=" + newName);
                    else
                        e.Values.Add(urlFieldName, appRow[urlFieldName].ToString().Replace("appName=" + oldName, "appName=" + newName));
                    e.Values["Name"] = newName;
                }
            }

            base.BeforeEdit(e);
        }


        protected override void AfterDeleteBeforeCommit(Durados.DeleteEventArgs e)
        {
            base.AfterDeleteBeforeCommit(e);
            string name = e.PrevRow["Name"].ToString();

            Maps.Instance.Delete(name);

            try
            {
                int id = Convert.ToInt32(e.PrimaryKey);

                System.Data.SqlClient.SqlConnectionStringBuilder scsb = new System.Data.SqlClient.SqlConnectionStringBuilder(Maps.Instance.ConnectionString);
                string mapServer = scsb.DataSource;
                MapDataSet.durados_SqlConnectionRow systemConnectionRow = ((MapDataSet.durados_AppRow)e.PrevRow).durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System;
                if (systemConnectionRow != null)
                {
                    string systemServer = systemConnectionRow.IsServerNameNull() ? null : systemConnectionRow.ServerName;
                    string systemDatabase = systemConnectionRow.IsCatalogNull() ? null : systemConnectionRow.Catalog;

                    if (string.IsNullOrEmpty(systemServer) || systemServer.Equals(mapServer))
                    {
                        DropDatabase(systemDatabase);
                    }
                }

                MapDataSet.durados_SqlConnectionRow appConnectionRow = ((MapDataSet.durados_AppRow)e.PrevRow).durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection;
                if (appConnectionRow != null)
                {
                    string appServer = appConnectionRow.IsServerNameNull() ? null : appConnectionRow.ServerName;
                    string appDatabase = appConnectionRow.IsCatalogNull() ? null : appConnectionRow.Catalog;

                    if (string.IsNullOrEmpty(appServer) || appServer.Equals(mapServer))
                    {
                        if (Maps.DropAppDatabase)
                        {
                            if (!HasOtherConnectios(appDatabase))
                            {
                                DropDatabase(appDatabase);
                            }
                        }
                    }
                }

                //DeleteConfig(id);
                //DeleteUploads(id);
            }

            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 2, "delete databases and config");
            }

        }

        private bool HasOtherConnectios(string appDatabase)
        {
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Maps.Instance.ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select count(*) from dbo.durados_SqlConnection where [Catalog] = N'" + appDatabase + "'", connection))
                {
                    connection.Open();
                    object scalar = command.ExecuteScalar();

                    if (scalar == null || scalar == DBNull.Value || scalar.Equals(0) || scalar.Equals(1))
                        return false;
                    return true;
                }
            }
        }

        private void DropDatabase(string name)
        {
            System.Data.SqlClient.SqlConnectionStringBuilder scsb = new System.Data.SqlClient.SqlConnectionStringBuilder(Maps.Instance.ConnectionString);
            //scsb.InitialCatalog = null;

            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(scsb.ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("ALTER DATABASE " + name + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE; drop database " + name, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private string GetDeleteAppUrl(string appName)
        {
            string id = GetMasterGuid();

            string qstring = "id=" + id + "&appName=" + appName; 
            
            return RestHelper.GetAppUrl(appName, Maps.OldAdminHttp) + "/Admin/DeleteApp?" + qstring;
        }
        public virtual IHttpActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                View view = GetView(null);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, AppViewName)));
                }
                if (!view.IsDeletable())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewIsUnauthorized));
                }

                int? appId = Maps.Instance.AppExists(id, Convert.ToInt32(Maps.Instance.DuradosMap.Database.GetUserID()));
                if (!appId.HasValue)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, AppViewName)));
                }

                string guid = GetMasterGuid();

                string qstring = "id=" + guid;
                string url = RestHelper.GetAppUrl(id, Maps.OldAdminHttp) + "/Admin/Restart?" + qstring;
                try
                {
                    Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(url, string.Empty, string.Empty, 100000);
                }
                catch
                {

                }
                
                try
                {
                Durados.Web.Mvc.Infrastructure.ProductMaintenance productMaintenece = new Durados.Web.Mvc.Infrastructure.ProductMaintenance();
                productMaintenece.RemoveApp(id);

                }
                catch
                {

                }

                //url = GetDeleteAppUrl(id);
                //string response = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(url,string.Empty,string.Empty, 100000);
                //Dictionary<string, object> ret = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);

                string sql = "delete durados_App where name = '" + id + "'";
                (new SqlAccess()).ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql);

                Maps.Instance.DuradosMap.Logger.Log("myApps", "delete", "", null, 1, "The app " + id + " was deleted");
                Maps.Instance.Restart(id);

                //RefreshOldAdmin(id);

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);


            }
        }
    }

    
}
