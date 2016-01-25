using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc.Azure;
using Newtonsoft.Json;
using System.Diagnostics;
using Durados.Data;
using Durados.SmartRun;
using Durados.Web.Mvc.Farm;

namespace Durados.Web.Mvc
{
    public class PortManager
    {
        public HashSet<int> OfficialPorts { get; private set; }
        public int StartFromPort { get; private set; }
        public int LastAssigned { get; private set; }
        public int MaxPort { get; private set; }

        public PortManager(int startFromPort, int maxPort)
            : this(new HashSet<int>() { 10008,
10010,
10050,
10051,
10110,
10113,
10114,
10115,
10116,
11371,
12222,
12223,
13075,
13720,
13721,
13724,
13782,
13783,
13785,
13786,
15000,
15000,
15345,
17500,
17500,
18104,
19283,
19315,
19999,
20000,
22347,
22350,
24465,
24554,
25000,
25003,
25005,
25007,
25010,
26000,
26000,
31457,
31620,
33434,
34567,
40000,
43047,
43048,
45824,
47001,
47808,
49151
}, startFromPort, maxPort)
        {
        }

        public PortManager(HashSet<int> officialPorts, int startFromPort, int maxPort)
        {
            this.OfficialPorts = officialPorts;
            this.StartFromPort = startFromPort;
            this.LastAssigned = startFromPort;
            this.MaxPort = maxPort;
        }

        public int Assign()
        {
            if (LastAssigned >= MaxPort)
                LastAssigned = StartFromPort;
            int p = LastAssigned + 1;
            while (IsUsed(p) || IsOfficial(p))
            {
                if (LastAssigned >= MaxPort)
                    throw new NoMorePortsAvailableException();
                p++;
            }

            LastAssigned = p;

            return p;
        }

        private bool IsOfficial(int port)
        {
            return OfficialPorts.Contains(port);
        }


        private bool IsUsed(int port)
        {

            bool isUsed = false;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            System.Net.NetworkInformation.IPGlobalProperties ipGlobalProperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            System.Net.NetworkInformation.TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (System.Net.NetworkInformation.TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    isUsed = true;
                    break;
                }
            }

            return isUsed;
        }
    }
}
