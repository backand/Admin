using Durados.Security.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Durados
{
    public class GoogleCloud : Cloud, ICloudProvider, ICloudForCreds

    {
        public GoogleCloud(Durados.Database database)
            : base(database)
        {

        }
        public override string GetCloudDescriptor()
        {
            return ProjectName;

        }

        public string EncryptedPrivateKey { get; set; }

        private string decryptedPrivateKey = null;

        public string DecryptedPrivateKey
        {
            get
            {
                if (decryptedPrivateKey == null)
                {
                    try
                    {
                        decryptedPrivateKey = Database.DecryptKey(EncryptedPrivateKey);
                    }
                    catch (DuradosException)
                    {}
                    
                }

                return decryptedPrivateKey;
            }
        }

       
        public override ICloudCredentials[] GetCloudCredentials()
        {
            List<ICloudCredentials> list = new List<ICloudCredentials>();

            list.Add(new GoogleCredentials() {   Cloud = this });

            return list.ToArray();
        }

        public override Dictionary<string, object> GetCredentialsForStorage()
        {

            return new Dictionary<string, object>() { { "privateKey", DecryptedPrivateKey }, { "clientEmail", ClientEmail } };

        }
        public override Dictionary<string, object> GetFunctionObject(Dictionary<string, object> functionObject)
        {
            Dictionary<string, object> filteredObject = new Dictionary<string, object>();
            
            filteredObject.Add("trigger", functionObject["Trigger"]);
          
            return filteredObject;


        }

        public override ICloudCredentials GetCredentialsForRule(Rule rule, string arn)
        {

            System.Runtime.Serialization.Json.DataContractJsonSerializer jss = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GoogleFunction));

            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(rule.LambdaProperties));

            GoogleFunction func = (GoogleFunction)jss.ReadObject(ms);

            return new GoogleCredentials() { FuncObject = func, Cloud = this };
        }


       

        public string ClientEmail { get; set; }

        public string ProjectName { get; set; }
    }

 [DataContract]
    public class GoogleFunction
    {
     [DataMember]
     public String trigger;
    }
   
}
