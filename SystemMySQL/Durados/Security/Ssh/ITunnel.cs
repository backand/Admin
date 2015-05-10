using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Security.Ssh
{
    public interface ITunnel
    {
        string RemoteHost { get; set; }
        int Port { get; set; }
        string User { get; set; }
        string Password { get; set; }
        string PrivateKey { get; set; }
    }
}
