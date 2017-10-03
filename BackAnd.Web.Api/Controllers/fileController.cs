using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
using System.IO;
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
    public class fileController : apiController
    {

        [HttpPut]
        [HttpPost]
        [Route("~/1/file")]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize("Admin,Developer")]
        public IHttpActionResult Put()
        {
            try
            {
                string jsonPost = Request.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> jsonPostDict = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(jsonPost);

                if (!jsonPostDict.ContainsKey("filename"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "You must send the filename parameter"));
                String filename = System.Web.HttpContext.Current.Server.UrlDecode((String)jsonPostDict["filename"]);
                if (!jsonPostDict.ContainsKey("filedata"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "You must send the filedata parameter"));
                String filedata = (String)jsonPostDict["filedata"];

                byte[] bytes = Convert.FromBase64String(filedata);

                Backand.IFiles files = Backand.StorageFactoey.GetCloudStorage();
                string url = files.upload(filename, filedata);
                return Ok(new { url = url });
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [HttpDelete]
        [Route("~/1/file")]
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize("Admin,Developer")]
        public IHttpActionResult Delete()
        {
            try
            {
                string jsonPost = Request.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> jsonPostDict = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(jsonPost);

                if (!jsonPostDict.ContainsKey("filename"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "You must send the filename parameter"));
                String filename = System.Web.HttpContext.Current.Server.UrlDecode((String)jsonPostDict["filename"]);
                
                Backand.cloudFiles files = new Backand.cloudFiles();
                files.delete(filename);
                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [HttpPut]
        public IHttpActionResult putObject(String provider)
        {
            try
            {
                string jsonPost = Request.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> jsonPostDict = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(jsonPost);

                if (!jsonPostDict.ContainsKey("key"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "You must send the key parameter"));
                String key = (String)jsonPostDict["key"];
                if (!jsonPostDict.ContainsKey("secret"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "You must send the secret parameter"));
                String secret = (String)jsonPostDict["secret"];
                if (!jsonPostDict.ContainsKey("bucket"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "You must send the bucket parameter"));
                String bucket = (String)jsonPostDict["bucket"];
                //String region = (String)jsonPostDict["region"];
                if (!jsonPostDict.ContainsKey("filename"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "You must send the filename parameter"));
                String filename = System.Web.HttpContext.Current.Server.UrlDecode((String)jsonPostDict["filename"]);
                if (!jsonPostDict.ContainsKey("filedata"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "You must send the filedata parameter"));
                String filedata = (String)jsonPostDict["filedata"];

                byte[] bytes = Convert.FromBase64String(filedata);
                string url = string.Empty;
                using (var dataStream = new MemoryStream(bytes))
                {
                    url = AmazonS3Helper.SaveUploadedFileToAws(key, secret, bucket, filename.Replace(" ", "-"), null, dataStream);
                }
                return Ok(new { url = url });
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }


        [HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
        public HttpResponseMessage Upload(string viewName, string fieldName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing);
            }

            Durados.Web.Mvc.View view = GetView(viewName);
            if (view == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewName));
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, Messages.FieldNameIsMissing);
            }

            Durados.Field[] fields = view.GetFieldsByJsonName(fieldName);
            if (fields.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.FieldNameNotFound, fieldName));
            }

            Durados.Field field = fields[0];

            if (!(field.FieldType == Durados.FieldType.Column && ((ColumnField)field).FtpUpload != null))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.UploadNotFound, fieldName));
            }

            Durados.Web.Mvc.ColumnField columnField = (Durados.Web.Mvc.ColumnField)field;

            //if (!Request.Content.IsMimeMultipartContent())
            //{
            //    this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            //}

            //var provider = GetMultipartProvider();
            //var result = Request.Content.ReadAsMultipartAsync(provider);
            //var result = await Request.Content.ReadAsMultipartAsync();

            // On upload, files are given a generic name like "BodyPart_26d6abe1-3ae1-416a-9429-b35f15e6e5d5"
            // so this is how you can get the original file name
            //var originalFileName = GetDeserializedFileName(result.FileData.First());

            //string strFileName = originalFileName;

            // uploadedFileInfo object will give you some additional stuff like file length,
            // creation time, directory name, a few filesystem methods etc..
            //var uploadedFileInfo = new FileInfo(result.FileData.First().LocalFileName);

            //string strExtension = Path.GetExtension(originalFileName).ToLower().TrimStart('.');

            List<object> files = new List<object>();

            foreach (string key in System.Web.HttpContext.Current.Request.Files.AllKeys)
            {
                var file = System.Web.HttpContext.Current.Request.Files[key];

                string strFileName = Path.GetFileName(file.FileName);

                try
                {

                    string strExtension = Path.GetExtension(strFileName).ToLower().TrimStart('.');

                    if (!FtpUploadValidExtension(strExtension))
                    {
                        //return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, Messages.InvalidFileType);
                        files.Add(new { fileName = strFileName, success = false, error = "Invalid file type" });
                        continue;
                    }

                    if (!string.IsNullOrEmpty(columnField.FtpUpload.FileAllowedTypes))
                    {
                        string[] exts = columnField.FtpUpload.FileAllowedTypes.Split(',');

                        bool valid = false;

                        foreach (string ext in exts)
                        {
                            if (ext.Trim().Equals(strExtension))
                            {
                                valid = true;
                                break;
                            }
                        }
                        if (!valid)
                        {
                            files.Add(new { fileName = strFileName, success = false, error = "Invalid file type" });
                            continue;
                            //return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, string.Format(Messages.InvalidFileType2, columnField.DisplayName, columnField.FtpUpload.FileAllowedTypes));
                        }
                    }

                    //float fileSize = (uploadedFileInfo.Length / 1024) / 1000;
                    float fileSize = (file.ContentLength / 1024) / 1000;

                    if (!FtpUploadValidSize(columnField, fileSize))
                    {
                        files.Add(new { fileName = strFileName, success = false, error = "The file is too big" });
                        continue;

                        //throw new Exception("The file has exceeded the size limit.");
                    }

                    if (!FtpUploadValidFolderSize(columnField, fileSize))
                    {
                        files.Add(new { fileName = strFileName, success = false, error = "Total files exceeded quota" });
                        continue;

                        //throw new Exception("The folder has exceeded the size limit.");
                    }

                    if (columnField.FtpUpload.FileMaxSize > 0)
                    {

                        if (fileSize > columnField.FtpUpload.FileMaxSize)
                        {
                            //throw new Exception("File too big in field [" + field.DisplayName + "].<br><br>Max Allowed size: " + columnField.FtpUpload.FileMaxSize + " MB");
                            files.Add(new { fileName = strFileName, success = false, error = "The file is too big" });
                            continue;

                        }
                    }

                    string strSaveLocation = string.Empty;

                    string src = string.Empty;

                    if (columnField.FtpUpload.StorageType == StorageType.Azure)
                    {
                        src = SaveUploadedFileToAzure(columnField, strFileName, file.ContentType, file.InputStream);
                    }
                    else if (columnField.FtpUpload.StorageType == StorageType.Aws)
                    {
                        src = SaveUploadedFileToAws(columnField, strFileName, file.ContentType, file.InputStream);
                    }
                    else
                    {
                        SaveUploadedFileToFtp(columnField, strFileName, file.ContentType, file.InputStream);

                        if (columnField.FtpUpload.StorageType != StorageType.Azure)
                        {
                            src = columnField.FtpUpload.DirectoryVirtualPath.TrimEnd('/') + "/" + ((string.IsNullOrEmpty(columnField.FtpUpload.DirectoryBasePath)) ? string.Empty : (columnField.FtpUpload.DirectoryBasePath.TrimStart('/').TrimEnd('/') + "/")) + strFileName;
                        }
                        else
                        {
                            src = System.Web.HttpContext.Current.Server.UrlEncode((new Durados.Web.Mvc.UI.ColumnFieldViewer()).GetDownloadUrl(columnField, string.Empty));
                        }
                    }

                    files.Add(new { fileName = strFileName, url = src, success = true });
                }
                catch (Exception exception)
                {
                    files.Add(new { fileName = strFileName, success = false, error = exception.Message });
                    continue;
                }
            }

            // Through the request response you can return an object to the Angular controller
            // You will be able to access this in the .success callback through its data attribute
            // If you want to send something to the .error callback, use the HttpStatusCode.BadRequest instead
            return this.Request.CreateResponse(HttpStatusCode.OK, new { files = files });
        }

        // You could extract these two private methods to a separate utility class since
        // they do not really belong to a controller class but that is up to you
        private MultipartFormDataStreamProvider GetMultipartProvider()
        {
            // IMPORTANT: replace "(tilde)" with the real tilde character
            // (our editor doesn't allow it, so I just wrote "(tilde)" instead)
            var uploadFolder = "~/App_Data/Tmp/FileUploads"; // you could put this to web.config
            var root = HttpContext.Current.Server.MapPath(uploadFolder);
            Directory.CreateDirectory(root);
            return new MultipartFormDataStreamProvider(root);
        }

        private string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return Newtonsoft.Json.JsonConvert.DeserializeObject(fileName).ToString();
        }

        public string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }

        protected virtual bool FtpUploadValidExtension(string extension)
        {
            HashSet<string> h = new HashSet<string>(new string[] { "ade", "adp", "app", "bas", "bat", "chm", "cmd", "cpl", "crt", "csh", "exe", "fxp", "hlp", "hta", "inf", "ins", "isp", "js", "jse", "ksh", "Lnk", "mda", "mdb", "mde", "mdt", "mdt", "mdw", "mdz", "msc", "msi", "msp", "mst", "ops", "pcd", "pif", "prf", "prg", "pst", "reg", "scf", "scr", "sct", "shb", "shs", "url", "vb", "vbe", "vbs", "wsc", "wsf", "wsh" });
            return !h.Contains(extension.ToLower());
        }

        protected virtual bool FtpUploadValidSize(ColumnField field, float fileSize)
        {
            return true;
        }

        protected virtual bool FtpUploadValidFolderSize(ColumnField field, float fileSize)
        {
            return true;
        }

        protected virtual string SaveUploadedFileToAzure(ColumnField field, string strFileName, string contentType, System.IO.Stream stream)
        {
            try
            {
                return AzureHelper.SaveUploadedFileToAzure(field.FtpUpload.AzureAccountName, field.FtpUpload.GetDecryptedAzureAccountKey(Map.GetConfigDatabase()), field.FtpUpload.DirectoryBasePath, strFileName, contentType, stream);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw new Durados.DuradosException(Map.Database.Localizer.Translate("Upload failed: " + exception.Message));
            }

        }

        protected virtual string SaveUploadedFileToAws(ColumnField field, string strFileName, string contentType, System.IO.Stream stream)
        {
            return AmazonS3Helper.SaveUploadedFileToAws(field.FtpUpload.AwsAccessKeyId, field.FtpUpload.GetDecryptedAwsSecretAccessKey(Map.GetConfigDatabase()), field.FtpUpload.DirectoryBasePath, strFileName);
        }

        protected virtual string SaveUploadedFileToFtp(ColumnField field, string strFileName, string contentType, System.IO.Stream stream)
        {
            try
            {
                return FtpHelper.SaveUploadedFileToFtp(field, strFileName);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw new Durados.DuradosException(Map.Database.Localizer.Translate("Operation failed" + ", " + exception.Message));
            }

        }
        
    }

}
