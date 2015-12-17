using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Azure
{
    public interface ICache<T> 
    {
        void Add(string key, T value);

        void Remove(string key);

        bool ContainsKey(string key);

        T Get(string key);

        T this[string key] { get; }
        
        IEnumerable<T> Values {get;}
    }

    public interface ILargeObjectCachingStamp
    {
        DateTime TimeStamp { get; set; }

        DateTime LastCheck { get; }

        void UpdateLastCheck();
    }
    
 
    
    public enum CacheType
    {
        Local
    
    }

    public static class CacheFactory
    {
        public static ICache<T> CreateCache<T>(string name)
        {
            return new LocalCache<T>(name);
        }
    }
}
