using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Security.Cloud
{
    public class AzureCredentials : ICloudCredentials
    {
        //public string AccessKeyID { get; set; }

        //public string SecretAccessKey { get; set; }
        //const string PROVIDER = CloudVendor.Azure.ToString();

        public ICloudForCreds Cloud { get; set; }
        public string tenant { get; set; }
        public string AppId { get; set; }
        public string SubscriptionId { get; set; }

        public string Password { get; set; }

        public string Region { get { return "general"; } set { } }

        public string GetProvider()
        {
            return Cloud.CloudVendor.ToString();
        }

        public Dictionary<string, object> GetCredentials()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("appId", AppId);
            data.Add("tenant", tenant);
            data.Add("subscriptionId", SubscriptionId);
            data.Add("password", Password);

            return data;
        }
    }
}
