using Backand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Aws
{
    public class S3
    {
        private string nodeUrl;

        public S3(string nodeUrl)
        {
            this.nodeUrl = nodeUrl;
        }

        public void ChangeFolderName(string bucket, string oldFolder, string newFolder)
        {
            try
            {
                string url = nodeUrl + "/folder/rename";
                XMLHttpRequest request = new XMLHttpRequest();
                request.open("POST", url, false);
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("bucket", bucket);
                data.Add("oldFolder", oldFolder);
                data.Add("newFolder", newFolder);


                request.setRequestHeader("content-type", "application/json");

                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                request.send(jss.Serialize(data));

                if (request.status != 200)
                {
                    Maps.Instance.DuradosMap.Logger.Log("hosting", "folder", request.responseText, null, 1, "status: " + request.status);
                    throw new DuradosException(request.responseText);
                    
                }

                Dictionary<string, object>[] response = null;
                try
                {
                    response = jss.Deserialize<Dictionary<string, object>[]>(request.responseText);
                }
                catch (Exception exception)
                {
                    Maps.Instance.DuradosMap.Logger.Log("hosting", "folder", exception.Source, exception, 1, request.responseText);
                    throw new DuradosException("Could not parse upload response: " + request.responseText + ". With the following error: " + exception.Message, exception);
                }

            }
            catch (Exception exception)
            {
                throw new DuradosException("Failed to rename folder", exception);

            }
        }
    }
}
