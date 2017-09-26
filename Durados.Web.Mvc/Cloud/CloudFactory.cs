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
            CloudType type = row.Row.IsNull("type") ? CloudType.Function : (CloudType)Enum.Parse(typeof(CloudType), (string)row["type"]);
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
                        Cloud cloud = new AzureCloud(Database) { Id = id, AppId = appId, SubscriptionId = subscriptionId, EncryptedPassword = encryptedPassword, tenant = tenant, CloudVendor = cloudVendor, Name = name, Type = type };
                        
                       
                        return cloud;

                    }
                case CloudVendor.GCP:
                    {
                        string projectName = row.Row.IsNull("ProjectName") ? null : (string)row["ProjectName"];
                        string clientEmail = row.Row.IsNull("ClientEmail") ? null : (string)row["ClientEmail"];
                        string encryptedPrivateKey = row.Row.IsNull("EncryptedPrivateKey") ? null : (string)row["EncryptedPrivateKey"];

                        Cloud cloud = new GoogleCloud(Database) { Id = id, EncryptedPrivateKey = encryptedPrivateKey, ClientEmail = clientEmail, ProjectName = projectName, CloudVendor = cloudVendor, Name = name, Type = type  };


                        return cloud;

                    }
                    
                default:
                    return new Cloud(Database) { Id = id, AccessKeyId = accessKeyId, Region = awsRegion, CloudVendor = cloudVendor, EncryptedSecretAccessKey = encryptedSecretAccessKey, Name = name, Type = type };
                    
            }
            
        }



        public static Cloud GetCloud(DataActionEventArgs e, Database Database )
        {
            CloudType type = !e.Values.ContainsKey("type") ? CloudType.Function : (CloudType)Enum.Parse(typeof(CloudType), (string)e.Values["type"]);
            string accessKeyId = !e.Values.ContainsKey("AccessKeyId") ? null : (string)e.Values["AccessKeyId"];
            string awsRegion = !e.Values.ContainsKey("AwsRegion") ? null : (string)e.Values["AwsRegion"];
            CloudVendor cloudVendor = !e.Values.ContainsKey("CloudVendor") ? CloudVendor.AWS : (CloudVendor)Enum.Parse(typeof(CloudVendor), (string)e.Values["CloudVendor"]);
            string encryptedSecretAccessKey = !e.Values.ContainsKey("EncryptedSecretAccessKey") ? null : (string)e.Values["EncryptedSecretAccessKey"];
            string name = !e.Values.ContainsKey("Name") ? null : (string)e.Values["Name"];
            switch (cloudVendor)
            {

                case CloudVendor.Azure:
                    {
                        string encryptedPassword = !e.Values.ContainsKey("password") ? null : (string)e.Values["password"];
                        string tenant = !e.Values.ContainsKey("tenant") ? null : (string)e.Values["tenant"];
                        string appId = !e.Values.ContainsKey("appId") ? null : (string)e.Values["appId"];
                        string subscriptionId = !e.Values.ContainsKey("subscriptionId") ? null : (string)e.Values["subscriptionId"];
                        Cloud cloud = new AzureCloud(Database) { AppId = appId, SubscriptionId = subscriptionId, EncryptedPassword = encryptedPassword, tenant = tenant, CloudVendor = cloudVendor, Name = name, Type = type };


                        return cloud;

                    }
                case CloudVendor.GCP:
                    {
                        string projectName = !e.Values.ContainsKey("ProjectName") ? null : (string)e.Values["ProjectName"];
                        string clientEmail = !e.Values.ContainsKey("ClientEmail") ? null : (string)e.Values["ClientEmail"];
                        string encryptedPrivateKey = !e.Values.ContainsKey("EncryptedPrivateKey") ? null : (string)e.Values["EncryptedPrivateKey"];

                        Cloud cloud = new GoogleCloud(Database) { EncryptedPrivateKey = encryptedPrivateKey, ClientEmail = clientEmail, ProjectName = projectName, CloudVendor = cloudVendor, Name = name, Type = type };


                        return cloud;

                    }

                default:
                    return new Cloud(Database) { AccessKeyId = accessKeyId, Region = awsRegion, CloudVendor = cloudVendor, EncryptedSecretAccessKey = encryptedSecretAccessKey, Name = name, Type = type };

            }
           

            //throw new NotImplementedException();
        }
    }
}
