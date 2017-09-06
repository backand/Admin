using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Durados;

namespace Durados.Web.Mvc
{
    class CloudFactory
    {
        public static  Durados.Cloud GetCloud(System.Data.DataRowView row, int id, Database Database )
        {
            string accessKeyId = row.Row.IsNull("AccessKeyId") ? null : (string)row["AccessKeyId"];
            string awsRegion = row.Row.IsNull("AwsRegion") ? null : (string)row["AwsRegion"];
            CloudVendor cloudVendor = row.Row.IsNull("CloudVendor") ? CloudVendor.AWS : (CloudVendor)Enum.Parse(typeof(CloudVendor), (string)row["CloudVendor"]);
            string encryptedSecretAccessKey = row.Row.IsNull("EncryptedSecretAccessKey") ? null : (string)row["EncryptedSecretAccessKey"];
            string name = row.Row.IsNull("Name") ? null : (string)row["Name"];
            switch (cloudVendor)
            {
               
                case CloudVendor.Azure:
                    return new AzureCloud(Database) { Id = id, AccessKeyId = accessKeyId, Region = awsRegion, CloudVendor = cloudVendor, EncryptedSecretAccessKey = encryptedSecretAccessKey, Name = name };
                    
                default:
                    return new Cloud(Database) { Id = id, AccessKeyId = accessKeyId, Region = awsRegion, CloudVendor = cloudVendor, EncryptedSecretAccessKey = encryptedSecretAccessKey, Name = name };
                    
            }
            
        }

       
    }
}
