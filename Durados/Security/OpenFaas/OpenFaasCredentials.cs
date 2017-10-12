using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Security.Cloud
{
    public class OpenFaasCredentials : ICloudCredentials
    {
        
        public ICloudForCreds Cloud { get; set; }


       
        public string GetProvider()
        {
            return Cloud.CloudVendor.ToString();
        }
      

        public Dictionary<string, object> GetCredentials()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("gateway", (Cloud as FnProjectCloud).gateway);
            data.Add("connectionString", (Cloud as FnProjectCloud).connectionString);
            

            return data;
        }
        public OpenFaasFunction FuncObject { get; set; }//triggerUrl, method,

        public  Dictionary<string, object> GetFunctionObject(string functionArn)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("trigger", FuncObject.trigger);

            return data;
        }
    }
}
