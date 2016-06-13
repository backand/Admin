using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.Webhook
{
    public class Webhook
    {
        public Webhook()
        {
            
        }
        public object Send(WebhookType webhookType, object body = null, string method = null, Dictionary<string, object> queryStringParameters = null, Dictionary<string, string> headers = null)
        {
            return Send(webhookType.ToString(), body, method, queryStringParameters, headers);
        }
        public object Send(string webhookType, object body = null, string method = null, Dictionary<string, object> queryStringParameters = null, Dictionary<string, string> headers = null)
        {
            WebhookParameters parameters = Maps.GetWebhookParameters(webhookType);
            if (parameters == null)
                throw new WebhookNotFoundException(webhookType);
            if (method == null)
                method = parameters.Method;
            RestClient client = new RestClient(parameters.Url);
            RestRequest request = new RestRequest((Method)Enum.Parse(typeof(Method), method));
            if (body == null)
                body = parameters.Body;
            if (body != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddBody(body);
            }
            if (queryStringParameters != null)
            {
                foreach (string key in queryStringParameters.Keys)
                {
                    request.Parameters.Add(new RestSharp.Parameter() { Name = key, Value = queryStringParameters[key] });
                }
            }
            if (parameters.QueryStringParameters != null)
            {
                foreach (string key in parameters.QueryStringParameters.Keys)
                {
                    request.Parameters.Add(new RestSharp.Parameter() { Name = key, Value = parameters.QueryStringParameters[key] });
                }
            }
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    request.AddHeader(key, headers[key]);
                }
            }
            if (parameters.Headers != null)
            {
                foreach (string key in parameters.Headers.Keys)
                {
                    request.AddHeader(key, parameters.Headers[key].ToString());
                }
            }
            var response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new WebhookRequestException(response.ErrorMessage ?? response.Content, response.ErrorException);
            }
            try
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var webhookJsonResponse = (Dictionary<string, object>)jss.Deserialize<dynamic>(response.Content);
                return webhookJsonResponse;
            }
            catch
            {
                return response.Content;
            }
        }
        public void HandleException(WebhookType webhookType, Exception exception)
        {
            HandleException(webhookType.ToString(), exception);
        }
        public void HandleException(string webhookType, Exception exception)
        {
            Maps.Instance.DuradosMap.Logger.Log(GetType().Name, new StackFrame(0).GetMethod().Name, webhookType.ToString(), exception, 1, null);
            WebhookParameters parameters = Maps.GetWebhookParameters(webhookType);
            if (parameters == null || parameters.ErrorHandling == null)
                return;

            if (parameters.ErrorHandling.Cancel)
            {
                throw new WebhookResponseException(parameters.ErrorHandling.Message);
            }
        }
    }
}
