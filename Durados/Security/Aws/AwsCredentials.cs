using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Security.Cloud
{
    public class AwsCredentials : ICloudCredentials
    {
        public ICloudForCreds Cloud { get; set; }
        //const string PROVIDER = CloudVendor.AWS.ToString();
        public string AccessKeyID { get; set; }

        public string SecretAccessKey { get; set; }

        public string Region { get; set; }


        public Dictionary<string, object> GetCredentials()
        {
            Dictionary<string, object> data =  new Dictionary<string, object>();

            data.Add("awsRegion", Region);
            data.Add("accessKeyId", AccessKeyID);
            data.Add("secretAccessKey", SecretAccessKey);
           
            return data;
        }
        public string GetProvider()
        {
            if(Cloud == null)
                return CloudVendor.AWS.ToString();
            return Cloud.CloudVendor.ToString();
        }
    }
}
