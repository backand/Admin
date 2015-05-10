using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Security.Ssh;

namespace Durados.DataAccess.Ssh
{
    public class Tunnel : ITunnel
    {
        public string RemoteHost { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string PrivateKey { get; set; }
    }

    
}
