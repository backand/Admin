using Durados.Web.Mvc.Controllers.Api;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.Analytics
{
    public class Cql
    {
        public const string AnalyticsLog = "AnalyticsLog";
        public Cql()
        {

        }

        public object Get(string name, int page, int pageSize, string filter, string sort)
        {
            return GetBackandResponse(GetInner(name, page, pageSize, GetFilter(filter), GetSort(sort)));
        }

        private object GetBackandResponse(Dictionary<string, object> cqlResponse)
        {
            if (!cqlResponse.ContainsKey("table"))
            {
                throw new CqlException("Cql response is missing the table property");
            }

            Dictionary<string, object> response = new Dictionary<string, object>();
            List<Dictionary<string,object>> list = new List<Dictionary<string,object>>();

            Dictionary<string, object> table = (Dictionary<string, object>)cqlResponse["table"];

            object[] cols = (object[])table["cols"];
            object[] rows = (object[])table["rows"];


            foreach (Dictionary<string, object> cqlRow in rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                object[] cqlRowValues = (object[])cqlRow["c"];
                for (int i = 0; i < cols.Length; i++)
                {
                    Dictionary<string, object> col = (Dictionary<string, object>)cols[i];
                    string colName = col["label"].ToString();
                    object value = ConvertToBackand(((Dictionary<string, object>)cqlRowValues[i])["v"]);
                    row.Add(colName, value);
                }

                list.Add(row);
            }
            
            response.Add("data", list.ToArray());

            return response;
        }

        private object ConvertToBackand(object o)
        {
            if (!(o is string))
            {
                return o;
            }

            if (!o.ToString().StartsWith("Date("))
            {
                return o;
            }

            string dateString = o.ToString().TrimStart("Date(".ToCharArray()).TrimEnd(")".ToCharArray());
            string[] dateParts = dateString.Split(",".ToCharArray());

            int year = Convert.ToInt32(dateParts[0]);
            int month = Convert.ToInt32(dateParts[1]) + 1;
            int day = Convert.ToInt32(dateParts[2]);
            int hour = Convert.ToInt32(dateParts[3]);
            int minute = Convert.ToInt32(dateParts[4]);
            int second = Convert.ToInt32(dateParts[5]);
            return new DateTime(year, month, day, hour, minute, second);
        }

        private string GetSort(string sort)
        {
            if (string.IsNullOrEmpty(sort) || sort.ToLower() == "null")
            {
                return " 1 desc ";
            }
            Dictionary<string, object>[] sortArray = null;

            try
            {
                sortArray = JsonConverter.DeserializeArray(sort);
            }
            catch (Exception exception)
            {
                throw new CqlException("Failed to Deserialize sort JSON: " + sort, exception);
            }
            string s = string.Empty;

            foreach (Dictionary<string, object> field in sortArray)
            {
                s += field["fieldName"] + " " + field["order"] + ",";
            }

            s = s.TrimEnd(',');

            return s;
        }

        private string GetFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return "1=1";
            return "(" + filter + ")";
        }

        private Dictionary<string, object> GetInner(string name, int page, int pageSize, string filter, string sort)
        {
            CqlConfig cqlConfig = GetCqlConfig();
            RestClient client = new RestClient(cqlConfig.ApiUrl + GetCql(cqlConfig, name, page, pageSize, filter, sort));
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", cqlConfig.AuthorizationHeader);
            var response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new CqlException(response.ErrorMessage ?? response.Content, response.ErrorException);
            }
            try
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var result = (Dictionary<string, object>)jss.Deserialize<dynamic>(response.Content);
                return result;
            }
            catch
            {
                throw new CqlException(response.Content);
            }
        }

        private string GetCql(CqlConfig cqlConfig, string name, int page, int pageSize, string filter, string sort)
        {
            return System.Web.HttpContext.Current.Server.UrlEncode(string.Format(cqlConfig.Cqls[name], Maps.Instance.GetMap().AppName, filter, sort, pageSize));
        }

        private CqlConfig GetCqlConfig()
        {
            return Maps.CqlConfig;
        }

    }

}
