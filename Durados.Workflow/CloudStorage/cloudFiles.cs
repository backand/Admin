using System;
using System.Collections.Generic;

namespace Backand
{
    public class cloudFiles : filesBackand,IFiles
    {
        public cloudFiles(Durados.Cloud cloud):base(cloud)
        {
        
        }
        public cloudFiles()
            : base(null)
        {

        }
        protected override Dictionary<string, object> GetFileDetails(string fileName, string bucket,  string path)
        {
            Dictionary<string, object> data = new Dictionary<string,object>();
            data.Add("credentials", Cloud.GetCredentialsForStorage());
            data.Add("storage", Cloud.GetStorageDetails(fileName,  bucket, path));
            data.Add("cloudProvider", Cloud.CloudVendor.ToString());
            return data;
        }

       

        public override void delete(string fileName, string bucket,string path)
        {
            string url = BaseUrl + "/deleteFile";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            
            Dictionary<string, object> data = GetFileDetails(fileName, bucket, path);

            request.setRequestHeader("content-type", "application/json");

            request.send(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data));

            if (request.status != 200)
            {
                throw new Durados.DuradosException(request.responseText);//"Server return status " + request.status + ", " +
            }
        }

        public override void delete(string fileName, string bucket)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Durados.DuradosException(Messages.MissingFileName);

            if (string.IsNullOrEmpty(bucket))
                throw new Durados.DuradosException(Messages.MissingBucket);

            
            
            delete(fileName, bucket, null);
        }

        public override void delete(string fileName)
        {
            throw new Durados.DuradosException(Messages.MissingBucket);

        }

        
    }

   
    
}
