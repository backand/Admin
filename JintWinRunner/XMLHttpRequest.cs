using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
        public void open(string type, string url, bool async)
        {
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
            if (header.ToLower() == "content-type")
                request.ContentType = value;
            else
                request.Headers.Add(header, value);
                
        }

        public void send()
        {
            send(null);
        }

        public void send(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(data);
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
            }

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                status = (int)response.StatusCode;
                if (status >= 200 && status< 300)
                {
                    //Get response stream into StreamReader
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                            responseText = reader.ReadToEnd();
                    }
                }
                response.Close();//Close HttpWebResponse
            }
            catch (WebException we)
            {   //TODO: Add custom exception handling
                responseText = we.Message;
                status = (int)((System.Net.HttpWebResponse)(we.Response)).StatusCode;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
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
