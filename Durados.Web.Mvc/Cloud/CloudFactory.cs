using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Durados;

namespace Durados.Web.Mvc
{
    public class CloudFactory
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
                    {
                        string encryptedPassword = row.Row.IsNull("Password") ? null : (string)row["Password"];
                        string tenant = row.Row.IsNull("tenant") ? null : (string)row["tenant"];
                        string appId = row.Row.IsNull("appId") ? null : (string)row["appId"];
                        string subscriptionId = row.Row.IsNull("subscriptionId") ? null : (string)row["subscriptionId"];
                        Cloud cloud = new AzureCloud(Database) { Id = id, AppId = appId, SubscriptionId = subscriptionId, EncryptedPassword = encryptedPassword, tenant = tenant, CloudVendor = cloudVendor, Name = name };
                        
                       
                        return cloud;

                    }
                case CloudVendor.GCP:
                    {
                        string projectName = row.Row.IsNull("ProjectName") ? null : (string)row["ProjectName"];
                        string clientEmail = row.Row.IsNull("ClientEmail") ? null : (string)row["ClientEmail"];
                        string encryptedPrivateKey = row.Row.IsNull("EncryptedPrivateKey") ? null : (string)row["EncryptedPrivateKey"];

                        Cloud cloud = new GoogleCloud(Database) { Id = id, EncryptedPrivateKey = encryptedPrivateKey, ClientEmail = clientEmail, ProjectName = projectName, CloudVendor = cloudVendor, Name = name };


                        return cloud;

                    }
                    
                default:
                    return new Cloud(Database) { Id = id, AccessKeyId = accessKeyId, Region = awsRegion, CloudVendor = cloudVendor, EncryptedSecretAccessKey = encryptedSecretAccessKey, Name = name };
                    
            }
            
        }



        public static Cloud GetCloud(EditEventArgs e)
        {
            return null;
            //throw new NotImplementedException();
        }
    }
}
