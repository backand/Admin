using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Security.Cloud
{
    public class GoogleCredentials : ICloudCredentials
    {
        
        public ICloudForCreds Cloud { get; set; }


       
        public string GetProvider()
        {
            return Cloud.CloudVendor.ToString();
        }
      

        public Dictionary<string, object> GetCredentials()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("privateKey", (Cloud as GoogleCloud).DecryptedPrivateKey);
            data.Add("projectName", (Cloud as GoogleCloud).ProjectName);
            data.Add("clientEmail", (Cloud as GoogleCloud).ClientEmail);
           

            return data;
        }
        public GoogleFunction FuncObject { get; set; }//triggerUrl, method,

        public  Dictionary<string, object> GetFunctionObject(string functionArn)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("trigger", FuncObject.trigger);

            return data;
        }
    }
}
