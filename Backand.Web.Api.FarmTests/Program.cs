using BackAnd.Web.Api.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backand.Web.Api.FarmTests
{
    class Program
    {
        static void Main(string[] args)
        {
            FarmCache farmCache = new FarmCache();

            farmCache.TestModelUpdate();
        }
    }
}
