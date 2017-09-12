using Durados.Security.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    decryptedSecretAccessKey = Database.DecryptKey(EncryptedSecretAccessKey);
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
            //if (provider != null)
            //{
            //    provider.SetSelectedFunctions(valueCollection, functionView);
            //    return;
            //}
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
                    Rule rule = GetRuleByDescriptor(arn,name, functionView);
                    bool selected = (rule != null);
                    lambdaFunction.Add(SELECTED, selected);
                    if (rule != null)
                        lambdaFunction.Add(FunctionId, rule.ID);
                }
            }
        }

        public virtual Rule GetRuleByDescriptor(string arn, string name,View functionView)
        {
            return functionView.GetRules().Where(r => r.LambdaArn == arn).FirstOrDefault();
        }

    }
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

        public string SubscriptionId { get; set; }
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

        


        public override ICloudCredentials[] GetCloudCredentials()
        {
            List<ICloudCredentials> list = new List<ICloudCredentials>();

            list.Add(new AzureCredentials() { tenant = tenant, SubscriptionId = SubscriptionId, AppId = AppId, Password = DecryptedPassword, Cloud = this });

            return list.ToArray();
        }




        public override void SetSelectedFunctions(Dictionary<string, Dictionary<string, object>[]>.ValueCollection valueCollection, View functionView)
        {
            foreach (var lambdaList in valueCollection)
            {
                foreach (var lambdaFunction in lambdaList)
                {
                    const string FunctionId = "functionId";

                    const string FunctionName = "FunctionName";
                    const string AppName = "AppName";

                    const string SELECTED = "selected";
                    if (!lambdaFunction.ContainsKey(FunctionName))
                        throw new DuradosException("ORM did not return Azure function list with Function name");
                    if (!lambdaFunction.ContainsKey(AppName))
                        throw new DuradosException("ORM did not return  Azure function list  with app name");

                    string functionName = lambdaFunction[FunctionName].ToString();

                    string appId = lambdaFunction[AppName].ToString();
                    Rule rule = GetRuleByDescriptor(appId,functionName,  functionView);
                    bool selected = (rule != null );
                    lambdaFunction.Add(SELECTED, selected);
                    if (rule != null)
                        lambdaFunction.Add(FunctionId, rule.ID);
                }
            }
        }

        public override Rule GetRuleByDescriptor( string appId,string name,  View functionView)
        {
            return functionView.GetRules().Where(r => r.LambdaName == name && r.LambdaArn == appId).FirstOrDefault();
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
    }
}
