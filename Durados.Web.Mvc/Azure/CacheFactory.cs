using Durados.Data;
using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Azure
{
    public static class CacheFactory
    {
        public static ICache<T> CreateCache<T>(string name)
        {
            return new LocalCache<T>(name);
        }
    }
}
