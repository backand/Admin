using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;




namespace Durados.DataAccess.GrooBoot
{

    public class GroBootPushAppNotification
    {
        string apibaseUrl = "https://ws.pushapps.mobi/Backand/";//"http://sandbox.pushapps.mobi/Backand/";//
        string apiGetAvailableMessages = "GetAvailableMessages";
        string apiSetMessagesForBookmark = "SetMessagesForBookmark";
        string apiGetBookmarkMessages = "GetBookmarkMessages";
        public GroBootPushAppNotification(string url)
        {
            apibaseUrl = url ?? apibaseUrl;
        }
        public List<Message> GetAvailableMessages(string accessKey, Diagnostics.ILogger Logger)
        {

            string url = apibaseUrl + apiGetAvailableMessages;
            GroBootRequestContent requestContent = new GroBootRequestContent { SecretToken = accessKey };
            GroBootResponseContent grobootResponse = GetResponse2(url, requestContent);
            if (grobootResponse.Data == null || grobootResponse.Data.Messages == null)
            {
                Logger.Log("Home", "GetAvailableGrobootNotificationMessages", "GetAvailableMessages", null, 1, grobootResponse.Code + ":" + grobootResponse.Message);
                return null;
            }

            return grobootResponse.Data.Messages;

        }

        public List<int> GetBookmarkMessages(string accessKey, int bookmarkId, Diagnostics.ILogger Logger)
        {

            string url = apibaseUrl + apiGetBookmarkMessages;
            GroBootRequestContent requestContent = new GroBootRequestContent { SecretToken = accessKey, BookmarkId = bookmarkId };
            GroBootResponseContent grobootResponse = GetResponse2(url, requestContent);
            if (grobootResponse.Data == null || grobootResponse.Data.Messages == null)
            {
                Logger.Log("Home", "GetSelectedGrobootNotificationMessages", "GetBookmarkMessages", null, 1, grobootResponse.Code + ":" + grobootResponse.Message);
                return null;
            }

            return grobootResponse.Data.Messages.Select(r => r.Id).ToList();
        }

        public void SetMessagesForBookmark(string accessKey, int bookmarkId,int[] messages)
        {

            string url = apibaseUrl + apiSetMessagesForBookmark;
            GroBootRequestContent requestContent = new GroBootRequestContent { SecretToken = accessKey, BookmarkId = bookmarkId, Messages = messages };
            GroBootResponseContent grobootResponse = GetResponse2(url, requestContent);

           // return grobootResponse.Data.Messages.Keys;

        }

        private  GroBootResponseContent GetResponse2(string url, GroBootRequestContent requestContent)
        {
            HttpWebRequest request = GetRequest(url, requestContent);
            GroBootResponseContent grobootResponse = GetResponse(request);
            return grobootResponse;
        }

        private  GroBootResponseContent GetResponse(HttpWebRequest request)
        {
            GroBootResponseContent grobootResponse;
            //if (1 == 1)
            //{
            //    string responseContent = "{  'Code' : 100,  'Message' : 'OK',  'Data' : {  'Messages' : [{ 'Id' : 1, 'Content' : 'Hi guys1...' },{ 'Id' : 2, 'Content' : 'Hi guys2...' },{ 'Id' : 3, 'Content' : 'Hi guys3...' }]  }  }";
            //    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //    grobootResponse = (GroBootResponseContent)jsonSerializer.Deserialize<GroBootResponseContent>(responseContent);  // = new DataContractJsonSerializer(typeof(Response));
            //}
            //else
            //{
                
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                    string responseContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    grobootResponse = (GroBootResponseContent)jsonSerializer.Deserialize<GroBootResponseContent>(responseContent);  // = new DataContractJsonSerializer(typeof(Response));

                }
           // }
            return grobootResponse;
        }

        private  HttpWebRequest GetRequest(string url, GroBootRequestContent requestContent)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = HttpVerb.POST.ToString();
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(new JavaScriptSerializer().Serialize(requestContent));
                streamWriter.Flush();
                streamWriter.Close();

            }
            return request;
        }
    }

    public class GroBootRequestContent
    {
        public string SecretToken { get; set; }
        public int BookmarkId { get; set; }
        public int[] Messages { get; set; }
    }

    public class GroBootResponseContent
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Data Data { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }

    public class Data
    {
        public List<Message> Messages { get; set; }
    }


}
