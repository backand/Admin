using Durados.Security.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Durados
{
    public class FnProjectCloud : Cloud, ICloudProvider, ICloudForCreds

    {
        public FnProjectCloud(Durados.Database database)
            : base(database)
        {

        }
        public override string GetCloudDescriptor()
        {
            return Name;

        }

       
       
        public override ICloudCredentials[] GetCloudCredentials()
        {
            List<ICloudCredentials> list = new List<ICloudCredentials>();

            list.Add(new FnProjectCredentials() {   Cloud = this });

            return list.ToArray();
        }

        public override Dictionary<string, object> GetCredentialsForStorage()
        {

            return new Dictionary<string, object>() { { "gateway", gateway }, { "connectionString", connectionString } };

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

            FnProjectFunction func = (FnProjectFunction)jss.ReadObject(ms);

            return new FnProjectCredentials() { FuncObject = func, Cloud = this };
        }


       

        public string gateway { get; set; }

        public string connectionString { get; set; }
    }

 [DataContract]
    public class FnProjectFunction
    {
     [DataMember]
     public String trigger;
    }
   
}
