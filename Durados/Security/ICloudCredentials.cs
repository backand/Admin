using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Security.Cloud
{
    public interface ICloudCredentials
    {
        ICloudForCreds Cloud { get; set; }

        Dictionary<string, object> GetCredentials();
      
        //string Region { get; set; }

        string GetProvider();

        Dictionary<string, object> GetFunctionObject(string functionArn);
    }
}
