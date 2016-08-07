using Durados.Web.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Analytics
{
    public class EmbeddedReports
    {
        public object GetUrl(string json)
        {
            EmbeddedReportsConfig embeddedReportsConfig = Maps.GetEmbeddedReportsConfig();
            Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);
            values.Add("apiToken", embeddedReportsConfig.ApiToken);

            string SlicerKey = "slicer";
            string BitwiseOperatorKey = "bitwiseOperator";
            string bitwiseOperator = "AND";
            string ConditionsKey = "conditions";
            string appName = Maps.Instance.GetAppName();

            Dictionary<string, object> slicer;
            if (!values.ContainsKey(SlicerKey))
            {
                slicer = new Dictionary<string,object>();
                values.Add(SlicerKey, slicer);
            }
            else
            {
                slicer = (Dictionary<string, object>)values[SlicerKey];
            }

            if (slicer.ContainsKey(BitwiseOperatorKey))
            {
                slicer[BitwiseOperatorKey] = bitwiseOperator;
            }
            else
            {
                slicer.Add(BitwiseOperatorKey, bitwiseOperator);
            }

            List<object> conditions = new List<object>();

            if (slicer.ContainsKey(ConditionsKey))
            {
                conditions.AddRange((object[])slicer[ConditionsKey]);
                slicer.Remove(ConditionsKey);
            }
            
            conditions.Add(new { property = embeddedReportsConfig.AppPropertyName, @operator = "STRING_EQUALS", values = new object[1] { appName } });

            slicer.Add(ConditionsKey, conditions.ToArray());

            json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(values);

            //Dictionary<string, string> headers = new Dictionary<string, string>() { { "Content-Type", "application/json" } };
            try
            {
                json = Http.PostWebRequest(embeddedReportsConfig.UrlStep1, json, "", "", null, null, "application/json");
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("EmbeddedReports", "PostWebRequest", appName, exception, 1, json);
                throw new DuradosException("Fail to get the url from coola, " + json, exception);
            }
            try
            {
                values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("EmbeddedReports", "Deserialize", appName, exception, 1, json);
                throw new DuradosException("Fail to Deserialize response from coola, " + json, exception);
            }

            string token = null;
            if (values.ContainsKey("token"))
            {
                token = values["token"].ToString();
            }
            else
            {
                Exception exception = new Exception("response does not contain token");
                Maps.Instance.DuradosMap.Logger.Log("EmbeddedReports", "token", appName, exception, 1, json);
                throw new DuradosException("response does not contain token, " + json, exception);
            }
            string url = string.Format(embeddedReportsConfig.UrlStep2, token);
            return new { url = url }; 
        }
    }
}
