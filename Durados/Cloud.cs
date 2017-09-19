using Durados.Security.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Durados
{
    public class Cloud : ICloudProvider, ICloudForCreds
    {
        public Cloud(Database database)
        {
            this.Database = database;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public CloudVendor CloudVendor { get; set; }

        public string Region { get; set; }

        public string AccessKeyId { get; set; }

        public string EncryptedSecretAccessKey { get; set; }

        private string decryptedSecretAccessKey = null;

        public string DecryptedSecretAccessKey
        {
            get
            {
                if (decryptedSecretAccessKey == null)
                {
                    try
                    {
                        decryptedSecretAccessKey = Database.DecryptKey(EncryptedSecretAccessKey);
                    }
                    catch (DuradosException)
                    {}
                    
                }

                return decryptedSecretAccessKey;
            }
        }

        public string[] Regions
        {
            get
            {
                if (string.IsNullOrEmpty(Region))
                    return new string[0];
                return Region.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public Database Database { get; private set; }

        //public ICloudProvider provider = null;

        //public AwsCredentials GetAwsCredentials()
        //{
        //    return new AwsCredentials() { AccessKeyID = AccessKeyId, SecretAccessKey = DecryptedSecretAccessKey, Region = AwsRegion.ToString() };
        //}

        public virtual ICloudCredentials[] GetCloudCredentials()
        {
            //if (provider != null)
            //    return provider.GetCloudCredentials();
            List<AwsCredentials> list = new List<AwsCredentials>();
            foreach (string region in Regions)
            {
                list.Add(new AwsCredentials() { AccessKeyID = AccessKeyId, SecretAccessKey = DecryptedSecretAccessKey, Region = region, Cloud = (ICloudForCreds)this });
            }
            return list.ToArray();
        }

        public virtual string GetCloudDescriptor()
        {

            //if (provider != null)
            //    return provider.GetCloudDescriptor();
            return AccessKeyId;


        }


        public virtual void SetSelectedFunctions(Dictionary<string, Dictionary<string, object>[]>.ValueCollection valueCollection, View functionView)
        { 
            if (isErrorInFunctionObject(valueCollection)) return;

            foreach (var lambdaList in valueCollection)
            {
                foreach (var lambdaFunction in lambdaList)
                {
                    const string FunctionId = "functionId";
                    const string ARN = "FunctionArn";
                    const string FunctionName = "FunctionName";
                    const string SELECTED = "selected";

                    if (!lambdaFunction.ContainsKey(ARN))
                        throw new DuradosException("ORM did not return lambda list with FunctionArn");
                    if (!lambdaFunction.ContainsKey(FunctionName))
                        throw new DuradosException("ORM did not return lambda list with Function name");
                    string arn = lambdaFunction[ARN].ToString();
                    string name = lambdaFunction[FunctionName].ToString();
                    Rule rule = GetRuleByArn(arn,name, functionView);
                    bool selected = (rule != null);
                    lambdaFunction.Add(SELECTED, selected);
                    if (rule != null)
                        lambdaFunction.Add(FunctionId, rule.ID);
                }
            }
        }

        protected bool isErrorInFunctionObject(Dictionary<string, Dictionary<string, object>[]>.ValueCollection valueCollection)
        {
            return (valueCollection == null || valueCollection.Count == 0 || valueCollection.First() == null);
        }

        public virtual Rule GetRuleByArn(string arn, string name,View functionView)
        {
            return functionView.GetRules().Where(r => r.LambdaArn == arn).FirstOrDefault();
        }


        public virtual Dictionary<string, object> GetFunctionObject(Dictionary<string, object> functionObject)
        {
            return null;
        }

        public virtual  ICloudCredentials GetCredentialsForRule(Rule rule, string arn)
        {
            
            string[] regions = this.Region.ToString().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string region = string.Empty;
            if (regions.Length == 1)
                region = regions[0];
            else
            {
                foreach (string r in regions)
                {
                    if (arn.Contains(r))
                    {
                        region = r;
                        break;
                    }
                }
            }
            string secretAccessKey = this.DecryptedSecretAccessKey;
            string accessKeyID = this.AccessKeyId;
            return new Durados.Security.Cloud.AwsCredentials() { Region = region, SecretAccessKey = secretAccessKey, AccessKeyID = accessKeyID };
        }
    }
    public interface ICloudProvider
    {
        string GetCloudDescriptor();
        ICloudCredentials[] GetCloudCredentials();
        //void SetSelectedFunctions(Dictionary<string, Dictionary<string, object>[]>.ValueCollection valueCollection, View functionView);

    }

    public interface ICloudForCreds
    {
        CloudVendor CloudVendor { get; set; }
        void SetSelectedFunctions(Dictionary<string, Dictionary<string, object>[]>.ValueCollection valueCollection, View functionView);

     
    }
    public enum CloudVendor
    {
        AWS
        , Azure
        , GCP
    }
}
