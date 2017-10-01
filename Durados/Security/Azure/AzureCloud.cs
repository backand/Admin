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
                    try
                    {
                        decryptedPassword = Database.DecryptKey(EncryptedPassword);
                    }
                    catch (DuradosException)
                    {}
                }

                return decryptedPassword;
            }
        }

        public string tenant { get; set; }
        public string SubscriptionId { get; set; }
        private string connectionString;
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set { connectionString = value; }
        }
      
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

        public override Dictionary<string, object> GetCredentialsForStorage()
        {

            return new Dictionary<string, object>() { { "connectionString", ConnectionString }};

        }

        public override ICloudCredentials GetCredentialsForRule(Rule rule, string arn)
        {
            if(rule ==null || rule.LambdaProperties == null)
                return new AzureCredentials() { FuncObject = null, Cloud = (ICloudForCreds)this };

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
