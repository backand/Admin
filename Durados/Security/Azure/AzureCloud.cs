using Durados.Security.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Durados
{
    public class AzureCloud : Cloud, ICloudProvider, ICloudForCreds
    {
        public AzureCloud(Durados.Database database)
            : base(database)
        {

        }
        public override string GetCloudDescriptor()
        {
            return AppId;

        }

        public string AppId { get; set; }

        public string EncryptedPassword { get; set; }

        private string decryptedPassword = null;

        public string DecryptedPassword
        {
            get
            {
                if (decryptedPassword == null)
                {
                    decryptedPassword = Database.DecryptKey(EncryptedPassword);
                }

                return decryptedPassword;
            }
        }

        public string tenant { get; set; }
        public string SubscriptionId { get; set; }
      
        public override ICloudCredentials[] GetCloudCredentials()
        {
            List<ICloudCredentials> list = new List<ICloudCredentials>();

            list.Add(new AzureCredentials() {  Cloud = this });

            return list.ToArray();
        }

        public override Dictionary<string, object> GetFunctionObject(Dictionary<string, object> functionObject)
        {
            Dictionary<string, object> filteredObject = new Dictionary<string, object>();
            filteredObject.Add("authLevel", functionObject["AuthLevel"]);
            filteredObject.Add("trigger", functionObject["Trigger"]);
            filteredObject.Add("appName", functionObject["AppName"]);
            filteredObject.Add("key", functionObject["Key"]);
            filteredObject.Add("name", functionObject["FunctionName"]);

            return filteredObject;


        }

        public override ICloudCredentials GetCredentialsForRule(Rule rule, string arn)
        {

            System.Runtime.Serialization.Json.DataContractJsonSerializer jss = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(AzureFunction));

            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(rule.LambdaProperties));

            AzureFunction func = (AzureFunction)jss.ReadObject(ms);

            return new AzureCredentials() { FuncObject = func, Cloud = (ICloudForCreds)this };
        }

    }

    [DataContract]
    public class AzureFunction
    {
        [DataMember]
        public String authLevel;

        [DataMember]
        public String trigger;

        [DataMember]
        public String appName;

        [DataMember]
        public String key;

        [DataMember]
        public String name;


        public String range;
    }
   
}
