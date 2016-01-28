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
            if (args == null || args.Length == 0)
            {
                farmCache.TestModelUpdate();
            }

            Console.WriteLine(args[0]);
                        
            switch (args[0])
            {
                case "model":
                    farmCache.TestModelUpdate();
                    break;

                case "sync":
                    farmCache.TestSync();
                    break;

                default:
                    farmCache.TestModelUpdate();
                    break;
            }
        }
    }
}
