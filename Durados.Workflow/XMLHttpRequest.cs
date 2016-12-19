using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backand
{
    public class XMLHttpRequest : IXMLHttpRequest
    {

        public XMLHttpRequest()
        {

        }

        WebRequest request = null;

        public int status { get; private set; }
        public string responseText { get; private set; }

        private bool Async = false;
        public void open(string type, string url, bool async)
        {
            Async = async;

            //if (url.Contains("objects/action") && System.Web.HttpContext.Current.Request.RawUrl.Contains("$$debug$$") && !url.Contains("url.Contains"))
            //if (url.Contains("objects") && !url.Contains("url.Contains"))
            if (url.Contains("objects") && System.Web.HttpContext.Current.Request.RawUrl.Contains("$$debug$$") && !url.Contains("url.Contains"))
            {
                if (url.Contains("&parameters="))
                {
                    if (url.Contains("&parameters=%7B%7D"))
                    {
                        url = url.Replace("&parameters=%7B%7D", "&parameters=%7B%22$$debug$$%22:true%7D");
                    }
                    else
                    {
                        url = url.Replace("&parameters=%7B", "&parameters=%7B%22$$debug$$%22:true,");
                    }
                }
                else if (url.Contains("?parameters="))
                {
                    if (url.Contains("?parameters=%7B%7D"))
                    {
                        url = url.Replace("?parameters=%7B%7D", "?parameters=%7B%22$$debug$$%22:true%7D");
                    }
                    else
                    {
                        url = url.Replace("?parameters=%7B", "?parameters=%7B%22$$debug$$%22:true,");
                    }
                }
                else
                {
                    if (url.Contains("?"))
                    {
                        url += "&parameters=%7B%22$$debug$$%22:true%7D";
                    }
                    else
                    {
                        url += "?parameters=%7B%22$$debug$$%22:true%7D";
                    }
                }
                object requestGuid = Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.GuidKey);
                if (requestGuid != null)
                {
                    url += "&" + Durados.Workflow.JavaScript.GuidKey + "=" + requestGuid;
                    if (Durados.Workflow.JavaScript.IsDebug() && !url.Contains("$$debug$$"))
                    {
                        url += "&$$debug$$=true";
                   
                    }
                }
            }
            request = WebRequest.Create(url);
            request.Method = type;
            
        }

        //public void basicAuth(string username, string password)
        //{
        //    string oAuthCredentials = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
        //    request.Headers.Add("Authorization", "Basic " + oAuthCredentials);

        //}

        public void setRequestHeader(string header, string value)
        {
            if (header.ToLower() == "content-type" || header.ToLower() == "accept")
                request.ContentType = value;
            else
                request.Headers.Add(header, value);

        }

        public void send()
        {
            send(null);
        }

        private void Log(int logType, string freeText, Exception exception = null, int? requestTiem = null)
        {
            try
            {
                Durados.Database database = Durados.Workflow.Engine.GetCurrentDatabase();

                if (database != null)
                {
                    database.Logger.Log(request.RequestUri.AbsoluteUri, request.Method, "Durados.Workflow", exception == null ? string.Empty : exception.Message, exception == null ? string.Empty : exception.StackTrace, logType, freeText, DateTime.Now, requestTiem);
                }
            }
            catch { }
        }

        public void send(string data)
        {
            Log(3, "Started");
            
            try
            {
                if (Durados.Workflow.JavaScript.IsCrud(request))
                {
                    try
                    {
                        string result = Durados.Workflow.JavaScript.PerformCrud(request, data);
                        status = 200;
                        responseText = result;
                    }
                    catch (Durados.Data.DataHandlerException e)
                    {
                        responseText = e.Message;
                        status = e.Status;
                    }

                    return;
                }
            }
            catch(Exception crudException) 
            {
                if (crudException.InnerException is StackOverflowException)
                {
                    responseText = crudException.Message;
                    status = (int)HttpStatusCode.BadRequest;
                    return;
                }
            }

            try
            {
                if (Durados.Workflow.JavaScript.IsDebug() || request.RequestUri.AbsoluteUri.Contains("localhost") || request.RequestUri.AbsoluteUri.ToLower().Contains("backand"))
                {
                    string appName = (Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Database.AppName) ?? string.Empty).ToString();
                    if (!string.IsNullOrEmpty(appName))
                    {
                        if (request.Headers["AppName"] == null && request.Headers["appName"] == null && request.Headers["appname"] == null)
                        {
                            request.Headers.Add("AppName", appName);
                        }
                    }
                    if (request.Headers["Authorization"] == null && request.Headers["authorization"] == null)
                    {
                        Durados.Database database = Durados.Workflow.Engine.GetCurrentDatabase();
                        if (database != null)
                        {
                            request.Headers.Add("Authorization", database.GetAuthorization());
                        }
                    }
                }
            }
            catch
            {

            }

            if (!string.IsNullOrEmpty(data) && data != "null")
            {
                if (request.RequestUri.AbsolutePath.Contains("1/file"))
                {
                    ((HttpWebRequest)request).AllowWriteStreamBuffering = false;
                    if (System.Web.HttpContext.Current.Items["file_stream"] != null)
                    {
                        data = data.Replace("file_stream", System.Web.HttpContext.Current.Items["file_stream"].ToString());
                    }
                }

                byte[] bytes;
                if (request.ContentType != null && request.ContentType.ToLower().Contains("multipart/form-data"))
                {
                    if (!request.ContentType.Contains("boundary="))
                    {
                        request.ContentType += "; boundary=" + Boundary;
                    }
                    //((HttpWebRequest)request).Accept = "*/*";
                    //((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.220 Safari/535.1";
                    //ServicePointManager.Expect100Continue = false;
                    //request.Proxy = null;
                    //request.PreAuthenticate = true;

                    bytes = GetMultipartFormData(data);
                }
                else
                {
                    bytes = System.Text.Encoding.UTF8.GetBytes(data);
                }
                request.ContentLength = bytes.Length;
                
                if (request.ContentType == null)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                using (Stream requestStream = request.GetRequestStream())
                {
                    //Writes a sequence of bytes to the current stream 
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();//Close stream
                }

                if (Durados.Workflow.JavaScript.IsDebug())
                {
                    try
                    {
                        string requestBody = System.Text.Encoding.UTF8.GetString(bytes); 

                    }
                    catch { }
                }
            }

            HttpWebResponse response = null;

            if (Async)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    /* run your code here */
                    var asyncRequest = WebRequest.Create(request.RequestUri);
                    asyncRequest.Method = request.Method;
                    asyncRequest.ContentType = request.ContentType;
                    foreach (var header in request.Headers.AllKeys)
                    {
                        if (!(header.ToLower() == "content-type" || header.ToLower() == "accept" || header.ToLower() == "host" || header.ToLower() == "content-length" || header.ToLower() == "expect" || header.ToLower() == "connection"))
                        {
                            try
                            {
                                asyncRequest.Headers.Add(header, request.Headers[header]);
                            }
                            catch { }
                        }
                    }
                    asyncRequest.BeginGetResponse(null, null);
                
                }).Start();
                return;
            }

            try
            {


                response = (HttpWebResponse)request.GetResponse();

                status = (int)response.StatusCode;
                if (status >= 200 && status < 300)
                {
                    //Get response stream into StreamReader
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        string charset = GetCharset(request);
                        Encoding encoding = Encoding.UTF8;
                        if (charset != null)
                        {
                            try
                            {
                               encoding= Encoding.GetEncoding(charset);
                            }
                            catch { }
                        }
                            
                        using (StreamReader reader = new StreamReader(responseStream,encoding))
                            responseText = reader.ReadToEnd();
                        //if(string.IsNullOrEmpty(responseText)) responseText="{}";
                    }
                }
                response.Close();//Close HttpWebResponse
                Log(3, request.RequestUri.OriginalString, null, 0);
            
                //Log(3, "Ended with status " + status);

            }
            catch (WebException we)
            {   //TODO: Add custom exception handling
                if (we.Status == WebExceptionStatus.Timeout)
                    status = (int)HttpStatusCode.RequestTimeout;
                responseText = we.Message;
                var encoding = UTF8Encoding.UTF8;
                if (we.Response != null)
                {
                    using (var reader = new System.IO.StreamReader(we.Response.GetResponseStream(), encoding))
                    {
                        responseText = reader.ReadToEnd();
                    }
                    status = (int)((System.Net.HttpWebResponse)(we.Response)).StatusCode;
                }
                if (we.Response != null && we.Response.Headers.AllKeys.Contains("error"))
                {
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        responseText += "; ";
                    }
                    responseText += we.Response.Headers["error"];
                }
                //Log(1, "Ended with status " + status, we);
                Log(1, request.RequestUri.OriginalString, we);
            
            }
            catch (Exception ex) 
            {
                //Log(1, "Ended with status " + status, ex);
                Log(1, request.RequestUri.OriginalString, ex);
 
                throw new Exception(ex.Message); 
            }
            finally
            {
                if (response != null)
                {
                    try
                    {
                        response.Close();
                    }
                    catch { }
                }
                response = null;
                request = null;
            }
        }

        private string GetCharset(WebRequest request)
        {
            try
            {
                string contentType = request.ContentType;
                string[] arr1 = contentType.Split(";".ToCharArray());
                if (arr1.Length != 2)
                {
                    return null;
                }
                string[] arr2 = arr1.LastOrDefault().Split("=".ToCharArray());
                if (arr2.Length != 2)
                {
                    return null;
                }
                return arr2.LastOrDefault();
            }
            catch
            {
                return null;
            }
        }

        private static readonly Encoding encoding = Encoding.UTF8;
        private static readonly string boundaryPrefix = "----WebKitFormBoundary";
        private static string _boundary = null;
        private static string Boundary
        {
            get
            {
                if (_boundary == null)
                {
                    _boundary = boundaryPrefix + Guid.NewGuid().ToString().Substring(0, 8);
                }
                return _boundary;
            }
        }
        private byte[] GetMultipartFormData(string json)
        {
            var theJavaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> postParameters = (Dictionary<string, object>)theJavaScriptSerializer.Deserialize<Dictionary<string, object>>(json);
            return GetMultipartFormData(postParameters, Boundary);
        }

        private byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }

    public class FileParameter
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameter(byte[] file) : this(file, null) { }
        public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }
    }

    public interface IXMLHttpRequest
    {
        int status { get; }
        string responseText { get; }
        void open(string type, string url, bool async);


        void setRequestHeader(string header, string value);

        void send();



        void send(string data);

    }
}
