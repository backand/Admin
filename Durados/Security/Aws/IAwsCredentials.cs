using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Security.Cloud
{
    public interface ICloudCredentials
    {
        string AccessKeyID { get; set; }

        string SecretAccessKey { get; set; }

        string Region { get; set; }
    }
}
