using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Services;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Data;

namespace Durados.DataAccess
{
    public class OngageMailAccess : IMailingService
    {
        private bool useFieldOrder = false;
        private string _listId;
        private string[] _apiKey;
        private string _username;
        private string _password;
        private string _accountcode;
        string apiUri = "https://connect.ongage.net/api/contacts";
        string subscribeFieldName = string.Empty;
        public bool UseFieldOrder
        {
            get
            {
                return false;
            }
            set
            {
                useFieldOrder = value;
            }
        }

        public string SubscribeFieldName
        {
            get
            {
                return subscribeFieldName;
            }
            set
            {
                subscribeFieldName = value;
            }
        }

        public Dictionary<string, string> SubscribeBatch(System.Data.DataTable subscribers)
        {
            int listCount = subscribers.Rows.Count;
           
            HttpWebRequest putReq = CreateRequest(HttpVerb.PUT);
           
            SetRequestContent(subscribers, putReq);

            int success = GetResponse(putReq);
           
            if (listCount > success)
            {
                HttpWebRequest postReq = CreateRequest(HttpVerb.POST);
                SetRequestContent(subscribers, postReq);
                GetResponse(postReq);
                
            }

            return null;

        }

        private HttpWebRequest CreateRequest(HttpVerb verb)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(apiUri);
            req.Method = verb.ToString();
            req.ContentType = "text/json";
            SetRequestHeaders(req);

            return req;
        }

        private void SetRequestContent(System.Data.DataTable subscribers, HttpWebRequest req)
        {

            string json = GetJson(subscribers, _listId);

            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

            }
        }

        public Dictionary<string, string> SubscribeBatch(System.Data.DataTable subscribers, string listId, string apiKey)
        {
            _listId = listId;
            _apiKey = apiKey.Split(';');
            _username = _apiKey[0];
            _password = _apiKey[1];
            _accountcode = _apiKey[2];
            return SubscribeBatch(subscribers);
        }

        private void SetRequestHeaders(HttpWebRequest req)
        {

            WebHeaderCollection headrs = new WebHeaderCollection();

            headrs.Add(string.Format("X_USERNAME: {0}", _username));
            headrs.Add(string.Format("X_PASSWORD: {0}", _password));
            headrs.Add("X_ACCOUNT_CODE", _accountcode);
            
            //headrs.Add("X_USERNAME: itay@backand.com");
            //headrs.Add("X_PASSWORD: Itay6914");
            //headrs.Add("X_ACCOUNT_CODE", "modubiz_ltd");
            req.Headers = headrs;
            System.Net.ServicePointManager.Expect100Continue = false;
          
        }
        private int GetResponse(HttpWebRequest req)
        {
            int error = 0;
            string responseString = string.Empty;
            OngageResponse res = new OngageResponse();
            try
            {

                WebResponse response = req.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();

                }
                res = new JavaScriptSerializer().Deserialize<OngageResponse>(responseString);
                return res.payload.success;
            }
            catch (Exception ex)
            {
                string errorCode = System.Text.RegularExpressions.Regex.Match(ex.Message, @"The remote server returned an error: \(([^)]*)\)").Groups[1].Value;
               
                if (errorCode == "412")
                {
                  
                }
                else
                    throw new DuradosException(ex.Message); ;
            }

            return 0;

        }
        public string GetJson(DataTable dt, string listId)
        {
            string json = string.Empty;
            StringBuilder csb = new StringBuilder();
          
            foreach (DataRow dr in dt.Rows)
            {
              
                StringBuilder fsb = new StringBuilder();

                string fields = string.Empty; ;
                foreach (DataColumn col in dr.Table.Columns)
                {
                    if (col.ColumnName.ToLower() != "email")
                    {
                        fsb.Append(",");

                        fsb.AppendFormat("\"{0}\": \"{1}\"", col.ColumnName, dr[col]);
                    }

                }
                csb.Append(",");
                csb.AppendFormat("{{\"email\":\"{0}\",\"list_id\":{1},\"fields\":{{{2}}}}}", dr["EMAIL"], listId, fsb.ToString().Trim(','));

            }
            return "[" + csb.ToString().Trim(',') + "]";


        }



  
    }

   
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class Metadata
    {
        public bool error { get; set; }
    }

    public class SuccessByDomain
    {
        public int others { get; set; }
    }

    public class Payload
    {
        public int rows { get; set; }
        public int success { get; set; }
        public int failed { get; set; }
        public SuccessByDomain success_by_domain { get; set; }
        public List<object> failed_by_domain { get; set; }
    }

    public class OngageResponse
    {
        public Metadata metadata { get; set; }
        public List<object> links { get; set; }
        public Payload payload { get; set; }
    }
}
