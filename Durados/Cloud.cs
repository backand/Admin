using Durados.Security.Aws;
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

        public string AwsRegion { get; set; }

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

        public string[] AwsRegions
        {
            get
            {
                if (string.IsNullOrEmpty(AwsRegion))
                    return new string[0];
                return AwsRegion.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public Database Database { get; private set; }

        //public AwsCredentials GetAwsCredentials()
        //{
        //    return new AwsCredentials() { AccessKeyID = AccessKeyId, SecretAccessKey = DecryptedSecretAccessKey, Region = AwsRegion.ToString() };
        //}

        public AwsCredentials[] GetAwsCredentials()
        {
            List<AwsCredentials> list = new List<AwsCredentials>();
            foreach (string region in AwsRegions)
            {
                list.Add(new AwsCredentials() { AccessKeyID = AccessKeyId, SecretAccessKey = DecryptedSecretAccessKey, Region = region });
            }
            return list.ToArray();
        }
    }

    public enum CloudVendor
    {
        AWS
    }
}
