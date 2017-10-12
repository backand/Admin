using Durados.Security.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Durados
{
    public class OpenFaasCloud : Cloud, ICloudProvider, ICloudForCreds

    {
        public OpenFaasCloud(Durados.Database database)
            : base(database)
        {

        }
        public override string GetCloudDescriptor()
        {
            return projectName;

        }

       
       
        public override ICloudCredentials[] GetCloudCredentials()
        {
            List<ICloudCredentials> list = new List<ICloudCredentials>();

            list.Add(new OpenFaasCredentials() { Cloud = this });

            return list.ToArray();
        }

        public override Dictionary<string, object> GetCredentialsForStorage()
        {

            return new Dictionary<string, object>() { { "projectName", projectName }, { "gateway", gateway }, { "connectionString", connectionString } };

        }
        public override Dictionary<string, object> GetFunctionObject(Dictionary<string, object> functionObject)
        {
            Dictionary<string, object> filteredObject = new Dictionary<string, object>();
            
            filteredObject.Add("trigger", functionObject["Trigger"]);
          
            return filteredObject;


        }

        public override ICloudCredentials GetCredentialsForRule(Rule rule, string arn)
        {

            System.Runtime.Serialization.Json.DataContractJsonSerializer jss = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(FnProjectFunction));

            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(rule.LambdaProperties));

            OpenFaasFunction func = (OpenFaasFunction)jss.ReadObject(ms);

            return new OpenFaasCredentials() { FuncObject = func, Cloud = this };
        }


       

        public string projectName { get; set; }
        public string gateway { get; set; }

        public string connectionString { get; set; }
    }

 [DataContract]
    public class OpenFaasFunction
    {
     [DataMember]
     public String trigger;
    }
   
}
