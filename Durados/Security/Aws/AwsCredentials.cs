using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Security.Aws
{
    public class AwsCredentials : IAwsCredentials
    {
        public string AccessKeyID { get; set; }

        public string SecretAccessKey { get; set; }

        public string Region { get; set; }

        
    }
}
