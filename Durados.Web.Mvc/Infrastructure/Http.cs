using System;
using System.Xml;
using System.Collections;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;

namespace Durados.Web.Mvc.Infrastructure
{

    public interface ISendAsyncErrorHandler
    {
        void HandleError(Exception exception);
    }

    // The RequestState class passes data across async calls.
    public class RequestState
    {
        const int BufferSize = 1024;
        public StringBuilder RequestData;
        public byte[] BufferRead;
        public WebRequest Request;
        public Stream ResponseStream;
        // Create Decoder for appropriate enconding type.
        public Decoder StreamDecode = Encoding.UTF8.GetDecoder();
        public ISendAsyncErrorHandler SendAsyncErrorHandler;
        public RequestState()
        {
            BufferRead = new byte[BufferSize];
            RequestData = new StringBuilder(String.Empty);
            Request = null;
            ResponseStream = null;
        }
    }

    public class ErrorEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public ISendAsyncErrorHandler SendAsyncErrorHandler { get; private set; }
        public ErrorEventArgs(string message, ISendAsyncErrorHandler sendAsyncErrorHandler)
            : base()
        {
            Message = message;
            SendAsyncErrorHandler = sendAsyncErrorHandler;
        }
    }

    public class Http
    {
        public delegate void ErrorEventDelegate(object sender, ErrorEventArgs a);
        public static event ErrorEventDelegate ErrorEvent;
        
        const int BUFFER_SIZE = 1024;

        public static void AsynWebRequest(string url, ISendAsyncErrorHandler sendAsyncErrorHandler)
        {
            AsyncWebRequest(url, sendAsyncErrorHandler, new AsyncCallback(RespCallback));
        }
        
        public static void AsyncWebRequest(string url, ISendAsyncErrorHandler sendAsyncErrorHandler, AsyncCallback asyncCallback, Dictionary<string, string> headers = null)
        {
            // Get the URI from the command line.
            Uri httpSite = new Uri(url);

            // Create the request object.
            WebRequest wreq = WebRequest.Create(httpSite);

            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    wreq.Headers.Add(key, headers[key]);
                }
            }

            wreq.Timeout = 60 * 60 * 1000;
            // Create the state object.
            RequestState rs = new RequestState();

            // Put the request into the state object so it can be passed around.
            rs.Request = wreq;
            rs.SendAsyncErrorHandler = sendAsyncErrorHandler;

            // Issue the async request.
            IAsyncResult r = (IAsyncResult)wreq.BeginGetResponse(
               asyncCallback, rs);

            // Wait until the ManualResetEvent is set so that the application 
            // does not exit until after the callback is called.
            //allDone.WaitOne();
        }

        public static void AsyncPostWebRequest(string url, string postData, Dictionary<string, string> headers, ISendAsyncErrorHandler sendAsyncErrorHandler, AsyncCallback asyncCallback)
        {
            // Get the URI from the command line.
            Uri httpSite = new Uri(url);

            // Create the request object.
            WebRequest wreq = WebRequest.Create(httpSite);

            StreamWriter requestWriter;
            if (wreq != null)
            {
                wreq.Method = "POST";
                //wreq.ServicePoint.Expect100Continue = false;
                wreq.Timeout = 60 * 60 * 1000;
                foreach (string key in headers.Keys)
                {
                    wreq.Headers.Add(key, headers[key]);
                }

                wreq.ContentType = "application/json";
                //POST the data.
                using (requestWriter = new StreamWriter(wreq.GetRequestStream()))
                {
                    requestWriter.Write(postData);
                }
            }

            wreq.Timeout = 60 * 60 * 1000;
            // Create the state object.
            RequestState rs = new RequestState();

            // Put the request into the state object so it can be passed around.
            rs.Request = wreq;
            rs.SendAsyncErrorHandler = sendAsyncErrorHandler;

            // Issue the async request.
            IAsyncResult r = (IAsyncResult)wreq.BeginGetResponse(
               asyncCallback, rs);

            // Wait until the ManualResetEvent is set so that the application 
            // does not exit until after the callback is called.
            //allDone.WaitOne();
        }

        private static void RespCallback(IAsyncResult ar)
        {
            RequestState rs = null;
            WebRequest req = null;
            try
            {
                // Get the RequestState object from the async result.
                rs = (RequestState)ar.AsyncState;


                
                // Get the WebRequest from RequestState.
                req = rs.Request;

                // Call EndGetResponse, which produces the WebResponse object
                //  that came from the request issued above.
                WebResponse resp = req.EndGetResponse(ar);

                //  Start reading data from the response stream.
                Stream ResponseStream = resp.GetResponseStream();

                // Store the response stream in RequestState to read 
                // the stream asynchronously.
                rs.ResponseStream = ResponseStream;

                //  Pass rs.BufferRead to BeginRead. Read data into rs.BufferRead
                IAsyncResult iarRead = ResponseStream.BeginRead(rs.BufferRead, 0,
                   BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);
            }
            catch (WebException e)
            {
                string strContent = e.Status + " ";

                if(req != null)
                {
                    strContent += req.RequestUri.ToString();
                }

                if(rs != null)
                {
                  OnErrorEvent(new ErrorEventArgs(strContent, rs.SendAsyncErrorHandler));

                  if (ar != null && ar.AsyncState != null && ar.AsyncState is RequestState && ((RequestState)ar.AsyncState).SendAsyncErrorHandler != null)
                  {
                      ((RequestState)ar.AsyncState).SendAsyncErrorHandler.HandleError(e);
                  }
                }
                else
                {
                    OnErrorEvent(new ErrorEventArgs(strContent, null));
                }

            }
        }

        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            // Get the RequestState object from AsyncResult.
            RequestState rs = (RequestState)asyncResult.AsyncState;

            // Retrieve the ResponseStream that was set in RespCallback. 
            Stream responseStream = rs.ResponseStream;

            // Read rs.BufferRead to verify that it contains data. 
            int read = responseStream.EndRead(asyncResult);
            if (read > 0)
            {
                // Prepare a Char array buffer for converting to Unicode.
                Char[] charBuffer = new Char[BUFFER_SIZE];

                // Convert byte stream to Char array and then to String.
                // len contains the number of characters converted to Unicode.
                int len =
                   rs.StreamDecode.GetChars(rs.BufferRead, 0, read, charBuffer, 0);

                String str = new String(charBuffer, 0, len);

                // Append the recently read data to the RequestData stringbuilder
                // object contained in RequestState.
                rs.RequestData.Append(
                   Encoding.ASCII.GetString(rs.BufferRead, 0, read));

                // Continue reading data until 
                // responseStream.EndRead returns –1.
                IAsyncResult ar = responseStream.BeginRead(
                   rs.BufferRead, 0, BUFFER_SIZE,
                   new AsyncCallback(ReadCallBack), rs);
            }
            else
            {
                if (rs.RequestData.Length > 0)
                {
                    //  Read data.
                    string strContent;
                    strContent = rs.RequestData.ToString();

                    //need to raise error
                    if (strContent.StartsWith("ERR:"))
                    {
                        OnErrorEvent(new ErrorEventArgs(strContent, rs.SendAsyncErrorHandler));
                    }

                }
                // Close down the response stream.
                responseStream.Close();
                // Set the ManualResetEvent so the main thread can exit.
                //allDone.Set();
            }

            return;
        }

        public static void OnErrorEvent(ErrorEventArgs e)
        {
            if (ErrorEvent != null)
            {
                ErrorEvent(null, e);
            }
        }

        public static string LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        }

        public static string WebRequestingJson(string url, string postData, Dictionary<string, string> headers)
        {
            string ret = string.Empty;

            StreamWriter requestWriter;

            var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.Method = "POST";
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 60 * 60 * 1000;
                foreach (string key in headers.Keys)
                {
                    webRequest.Headers.Add(key, headers[key]);
                }

                webRequest.ContentType = "application/json";
                //POST the data.
                using (requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(postData);
                }
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            ret = reader.ReadToEnd();

            return ret;
        }

        public static string PostWebRequest(string url, string postData, string header = "", string Accept = "", Dictionary<string, string> headers = null, int? timeout = null, string contentType = null)
        {

            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            if (!string.IsNullOrEmpty(contentType))
            {
                webRequest.ContentType = contentType;
            
            }
            if (Accept != "")
                ((HttpWebRequest)webRequest).Accept = Accept;
            if (header != "")
                webRequest.Headers.Add(header);
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    webRequest.Headers.Add(key, headers[key]);
                }

            }
            Stream reqStream = webRequest.GetRequestStream();
            byte[] postArray = Encoding.ASCII.GetBytes(postData);
            reqStream.Write(postArray, 0, postArray.Length);
            if (timeout.HasValue)
                webRequest.Timeout = timeout.Value;
            reqStream.Close();
            StreamReader sr = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string result = sr.ReadToEnd();

            return result;
        }

        public static void PostWebRequest2(string url, string postData, string header = "", string Accept = "", Dictionary<string, string> headers = null, int? timeout = null)
        {

            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            if (Accept != "")
                ((HttpWebRequest)webRequest).Accept = Accept;
            if (header != "")
                webRequest.Headers.Add(header);
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    webRequest.Headers.Add(key, headers[key]);
                }

            }
            Stream reqStream = webRequest.GetRequestStream();
            byte[] postArray = Encoding.ASCII.GetBytes(postData);
            reqStream.Write(postArray, 0, postArray.Length);
            if (timeout.HasValue)
                webRequest.Timeout = timeout.Value;
            reqStream.Close();
            try
            {
                webRequest.GetResponse();
            }
            catch { }
        }
        
        public static string GetWebRequest(string url, string header = "", string UserAgent = "", int? timeout = null)
        {

            WebRequest request = WebRequest.Create(url);
            if (timeout.HasValue)
                request.Timeout = timeout.Value;

            request.Method = "GET";
            if (header != "")
                request.Headers.Add(header);
            if (UserAgent != "")
                ((HttpWebRequest)request).UserAgent = UserAgent;
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    return sr.ReadToEnd();
                }
            }
        }

        public static void CallWebRequest(string url)
        {
            try
            {
                // Create a 'WebRequest' object with the specified url. 
                WebRequest myWebRequest = WebRequest.Create(url);

                // Send the 'WebRequest' and wait for response.
                WebResponse myWebResponse = myWebRequest.GetResponse();

                // Release resources of response object.
                myWebResponse.Close();
            }
            catch (Exception ex)
            {
                Map map = Maps.Instance.GetMap();
                if (map != null)
                    map.Logger.Log("Http", "CallWebRequest", "CallWebRequest", ex, 3, url);

            }

        }

        public static string CallWebService(string webServiceUrl, string header = "")
        {
            HttpWebRequest HttpWReq;
            HttpWebResponse HttpWResp;
            HttpWReq = (HttpWebRequest)WebRequest.Create(webServiceUrl);
            HttpWReq.Method = "GET";
            if (header != "")
                HttpWReq.Headers.Add(header);
            HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();
            if (HttpWResp.StatusCode == HttpStatusCode.OK)
            {
                //Consume webservice with basic XML reading, assumes it returns (one) string
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ProhibitDtd = false;
                XmlReader reader = XmlReader.Create(HttpWResp.GetResponseStream(), settings);

                try
                {
                    while (reader.Read())
                    {
                        reader.MoveToFirstAttribute();
                        if (reader.NodeType == XmlNodeType.Text)
                        {
                            return reader.Value;
                        }
                    }
                }
                catch (Exception exception)
                {
                    return exception.Message;
                }
                return String.Empty;
            }
            else
            {
                throw new Exception("Error on remote IP to Country service: " + HttpWResp.StatusCode.ToString());
            }
        }
    }
}
