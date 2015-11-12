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
using System.Data;
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
    [RoutePrefix("1/upload/config")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class uploadConfigController : configurationTableController
    {
        [Route("{id}")]
        public new IHttpActionResult Get(string id)
        {
            return GetItem(id, false);


        }
        protected override string GetConfigViewName()
        {
            return "FtpUpload";
        }
        [Route("")]
        [HttpGet]
        public new IHttpActionResult Get(bool? withSelectOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null)
        {
            return base.Get(withSelectOptions, pageNumber, pageSize, filter, sort, search);
        }
         [Route("{id}")]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public override IHttpActionResult Put(string id)
        {
            return base.Put(id);
        }

         [Route("{viewName}/{fieldName}")]
         [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
         public override IHttpActionResult Post()
         {
             IDictionary<string,object> values = Request.GetRouteData().Values;
             if (!values.ContainsKey("viewName"))
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
             }
             if (!values.ContainsKey("fieldName"))
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.FieldNameIsMissing));
             }
             string viewName = values["viewName"].ToString();
             string fieldName = values["fieldName"].ToString();

             View view = GetView(viewName);
             if (view == null)
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewName)));
             }

             Durados.Field[] fields = view.GetFieldsByJsonName(fieldName);
             if (fields == null || fields.Length == 0)
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.FieldNameNotFound, fieldName)));
             }


             Durados.Field field = fields[0];

             if (field.FieldType != Durados.FieldType.Column || field.GetColumnFieldType() != Durados.ColumnFieldType.String)
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.TheFieldMustBeTextual, fieldName)));
             }


             
             return base.Post();
         }

         protected override void HandleDataBeforeCache(string pk, System.Data.DataRow row)
         {
             IDictionary<string, object> values = Request.GetRouteData().Values;
             string viewName = values["viewName"].ToString();
             string fieldName = values["fieldName"].ToString();

             Durados.DataAccess.ConfigAccess configAccess = new ConfigAccess();

             Database configDatabase = map.GetConfigDatabase();

             string fieldPk = configAccess.GetFieldPK(viewName, fieldName, configDatabase.ConnectionString);

             View fieldView = (View)configDatabase.Views["Field"];

             fieldView.Edit(new Dictionary<string, object>() { { "FtpUpload_Parent", pk } }, fieldPk, null, null, null, null);

         }
        
         protected override bool IsDeep()
         {
             return false;
         }


         [Route("{id}")]
         [HttpDelete]
         [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
         public override IHttpActionResult Delete(string id)
         {
             return base.Delete(id);
         }

        const string NAME = "Title";
        protected override IHttpActionResult ValidateInputForPost(View view, Dictionary<string, object> values)
        {
            if (values.ContainsKey(NAME))
            {
                string name = values[NAME].ToString();

                int rowCount = 0;
                DataView dataView = view.FillPage(1, 1000, new Dictionary<string,object>(){{"Title", name}}, false, null, out rowCount, null, null);

                if (rowCount != 0)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.UploadWithNameAlreadyExists, name)));

                }
            }
            else
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Name is missing"));
            }

            
            if (!values.ContainsKey("StorageType"))
            {
                values.Add("StorageType", "Azure");
            }
            if (!values.ContainsKey("FileMaxSize"))
            {
                values.Add("FileMaxSize", "2");
            }
            if (!values.ContainsKey("overrideFileonSave"))
            {
                values.Add("overrideFileonSave", true);
            }
            if (!values.ContainsKey("DirectoryVirtualPath"))
            {
                values.Add("DirectoryVirtualPath", string.Format(Maps.AzureStorageUrl, Maps.AzureStorageAccountName, Maps.AzureAppPrefix + map.Id));
            }
            if (!values.ContainsKey("UploadFileType"))
            {
                values.Add("UploadFileType", "Image");
            }
            if (!values.ContainsKey("FileAllowedTypes"))
            {
                values.Add("FileAllowedTypes", "jpg,jpeg,gif,png,JPEG,JPG,GIF,PNG");
            }
            if (!values.ContainsKey("DirectoryBasePath"))
            {
                values.Add("DirectoryBasePath", Maps.AzureAppPrefix + map.Id);
            }
            if (!values.ContainsKey("Height"))
            {
                values.Add("Height", Maps.DefaultImageHeight);
            }
            if (!values.ContainsKey("accountName"))
            {
                values.Add("accountName", Maps.AzureStorageAccountName);
            }
            if (!values.ContainsKey("AzureAccountName"))
            {
                values.Add("AzureAccountName", Maps.AzureStorageAccountName);
            }
            if (!values.ContainsKey("AzureAccountKey"))
            {
                values.Add("AzureAccountKey", Maps.AzureStorageAccountKey);
            }

            return null;
        }

        protected override IHttpActionResult ValidateInputForUpdate(string id, View view, Dictionary<string, object> values)
        {
            if (values.ContainsKey(NAME))
            {
                string name = values[NAME].ToString();
                int rowCount = 0;
                DataView dataView = view.FillPage(1, 1000, new Dictionary<string, object>() { { "Title", name } }, false, null, out rowCount, null, null);

                if (rowCount > 1)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.QueryWithNameAlreadyExists, name)));
                }
                else if (rowCount == 1)
                {
                    if (!dataView[0].Row["ID"].ToString().Equals(id))
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.QueryWithNameAlreadyExists, name)));
                    }
                }
            }

            
            return null;
        }
    }

}
