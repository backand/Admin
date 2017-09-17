using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Security.Cloud
{
    public class AzureCredentials : ICloudCredentials
    {
        public ICloudForCreds Cloud { get; set; }
        //public string tenant { get { return (Cloud as AzureCloud).tenant; } }
        //public string AppId { get { return (Cloud as AzureCloud).AppId; } }
        //public string SubscriptionId { get { return (Cloud as AzureCloud).SubscriptionId; } }
        //public string Password { get { return (Cloud as AzureCloud).DecryptedPassword; } }
        //public string Region { get { return "general"; } set { } }

        public AzureFunction FuncObject { get; set; }
        public string GetProvider()
        {
            return Cloud.CloudVendor.ToString();
        }

        public Dictionary<string, object> GetCredentials()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("appId", (Cloud as AzureCloud).AppId);
            data.Add("tenant", (Cloud as AzureCloud).tenant);
            data.Add("subscriptionId", (Cloud as AzureCloud).SubscriptionId);
            data.Add("password", (Cloud as AzureCloud).DecryptedPassword);

            return data;
        }
        public  Dictionary<string, object> GetFunctionObject(string functionArn)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("authLevel", FuncObject.authLevel);
            data.Add("trigger", FuncObject.trigger);
            data.Add("key", FuncObject.key);
            data.Add("name", FuncObject.name);
            data.Add("appName", FuncObject.appName);

            return data;
        }
    }
}
