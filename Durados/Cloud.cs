using Durados.Security.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados
{
    public class Cloud
    {
        public Cloud(Database database)
        {
            this.Database = database;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public CloudVendor CloudVendor { get; set; }

        public string Region { get; set; }

        public virtual string AccessKeyId { get; set; }

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

        //public AwsCredentials GetAwsCredentials()
        //{
        //    return new AwsCredentials() { AccessKeyID = AccessKeyId, SecretAccessKey = DecryptedSecretAccessKey, Region = AwsRegion.ToString() };
        //}

        public virtual ICloudCredentials[] GetCloudCredentials()
        {
            List<AwsCredentials> list = new List<AwsCredentials>();
            foreach (string region in Regions)
            {
                list.Add(new AwsCredentials() { AccessKeyID = AccessKeyId, SecretAccessKey = DecryptedSecretAccessKey, Region = region });
            }
            return list.ToArray();
        }
    }
    public class AzureCloud : Cloud
    {
        public override string AccessKeyId {
        
        get{ return "aaaaa";}
       // set{AccessKeyId = value;}
        }
    
        public AzureCloud(Database database)
            : base(database)
        {
             
         }
         
        public override ICloudCredentials[] GetCloudCredentials()
        {
            List<ICloudCredentials> list = new List<ICloudCredentials>();
            foreach (string region in Regions)
            {
                list.Add(new AzureCredentials() { AccessKeyID = AccessKeyId, SecretAccessKey = DecryptedSecretAccessKey });
            }
            return list.ToArray();
        }

        
   
       

    }

    public enum CloudVendor
    {
        AWS
        ,Azure
    }
}
