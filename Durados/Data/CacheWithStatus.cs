using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Data
{
    /// <summary>
    /// Wrapper for dataset cache with <see cref="CacheStatus"/>
    /// </summary>
    public class CacheWithStatus : ICache<DataSet>, IStatusCache
    {
        private ICache<DataSet> realCache;

        private CacheStatus cacheChecksums;

        public CacheWithStatus(ICache<DataSet> realCache)
        {
            this.realCache = realCache;
            this.cacheChecksums = new CacheStatus();
        }

        public void Add(string key, DataSet value)
        {
            realCache.Add(key, value);
            cacheChecksums.UpdateAsync(key, value);
        }

        public void Remove(string key)
        {
            realCache.Remove(key);
            cacheChecksums.ClearStatus(key);
        }

        public bool ContainsKey(string key)
        {
            return realCache.ContainsKey(key);
        }

        public DataSet Get(string key)
        {
            return realCache.Get(key);
        }

        public DataSet this[string key]
        {
            get
            {
                return realCache[key];
            }
            set
            {
                realCache.Add(key, value);
            }
        }

        public IEnumerable<DataSet> Values
        {
            get { return realCache.Values; }
        }

        public Dictionary<string, DataSet> ToDictionary()
        {
            return realCache.ToDictionary();
        }

        public Dictionary<string, string> GetCacheStatus()
        {
            return cacheChecksums.GetStatus();
        }
    }
}
