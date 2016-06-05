using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Webhook
{
    public class Webhook
    {
        public Webhook()
        {
            
        }
        public void Send(WebhookType webhookType, object body = null, string method = null, Dictionary<string, object> queryStringParameters = null, Dictionary<string, string> headers = null)
        {
            WebhookParameters parameters = Maps.GetWebhookParameters(webhookType);
            if (parameters == null)
                return;
            if (method == null)
                method = parameters.Method;
            RestClient client = new RestClient(parameters.Url);
            RestRequest request = new RestRequest((Method)Enum.Parse(typeof(Method), method));
            if (body == null)
                body = parameters.Body;
            if (body != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddBody(parameters.Body);
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

        }

        public void HandleException(WebhookType webhookType, Exception exception)
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
