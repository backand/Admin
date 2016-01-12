using BackAnd.Web.Api.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Backand.Web.Api.FarmTests
{
    class Program
    {
        static void Main(string[] args)
        {

            ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => {
                        Console.WriteLine("Bla");
                        return true;
                    };

            FarmCache farmCache = new FarmCache();

            farmCache.TestModelUpdate();
        }
    }
}
