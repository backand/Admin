using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Azure
{
    public interface ICache
    {
        void Add(string key, object value);

        void Remove(string key);

        bool ContainsKey(string key);

        object Get(string key);

        void SyncCache(string key);
        
    }

    public interface ILargeObjectCachingStamp
    {
        DateTime TimeStamp { get; set; }

        DateTime LastCheck { get; }

        void UpdateLastCheck();
    }
    
    

    public class LocalCache : ICache
    {
        public Dictionary<string, object> Dictionary { get; private set; }

        public LocalCache()
        {
            Dictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void Add(string key, object value)
        {
            if (ContainsKey(key))
            {
                try
                {
                    Remove(key);
                }
                catch { }
            }
            Dictionary.Add(key, value);
        }

        public void Remove(string key)
        {
            Dictionary.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            return Dictionary.ContainsKey(key);
        }

        public object Get(string key)
        {
            if (!Dictionary.ContainsKey(key))
                return null;
            return Dictionary[key];
        }

        public void SyncCache(string key)
        {
            
        }
    }

    public class LargeObjectsAzureCache : ICache
    {
        public ICache LocalCache { get; private set; }
        public ICache AzureCache { get; private set; }
        
        public LargeObjectsAzureCache()
        {
            LocalCache = new LocalCache();
            AzureCache = new AzureCache();
        }

        public void Add(string key, object value)
        {
            Add(key, (ILargeObjectCachingStamp)value);
        }
        public void Add(string key, ILargeObjectCachingStamp value)
        {
            DateTime timeStamp = DateTime.Now;
            value.TimeStamp = timeStamp;
            LocalCache.Add(key, value);
            AzureCache.Add(key, timeStamp);
        }

        public void Remove(string key)
        {
            LocalCache.Remove(key);
            AzureCache.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            if (!LocalCache.ContainsKey(key))
                return false;

            if (System.Web.HttpContext.Current.Items.Contains(key))
                return true;

            ILargeObjectCachingStamp localValue = LocalCache.Get(key) as ILargeObjectCachingStamp;

            object azureValue = AzureCache.Get(key);
            DateTime? timeStamp = null;
            if (azureValue != null)
                timeStamp = (DateTime)azureValue;

            System.Web.HttpContext.Current.Items.Add(key, true);

            return localValue != null && timeStamp.HasValue && timeStamp.Value <= localValue.TimeStamp;

            //if (!LocalCache.ContainsKey(key))
            //    return false;
            //ILargeObjectCachingStamp localValue = LocalCache.Get(key) as ILargeObjectCachingStamp;

            //if (Maps.AzureCacheUpdateInterval > DateTime.Now.Subtract(localValue.LastCheck))
            //{
            //    return LocalCache.ContainsKey(key);
            //}

            //object azureValue = AzureCache.Get(key);
            //DateTime? timeStamp = null;
            //if (azureValue != null)
            //    timeStamp = (DateTime)azureValue;

            //localValue.UpdateLastCheck();

            //return localValue != null && timeStamp.HasValue && timeStamp.Value <= localValue.TimeStamp;
        }

        public object Get(string key)
        {
            return ContainsKey(key) ? LocalCache.Get(key) : null;
        }

        public void SyncCache(string key)
        {
            DateTime timeStamp = DateTime.Now;
            ((ILargeObjectCachingStamp)LocalCache.Get(key)).TimeStamp = timeStamp;
            AzureCache.Add(key, timeStamp);
        }
    }
    
    public class AzureCache : ICache
    {
        public DataCache DataCache { get; private set; }
        public AzureCache()
        {
            DataCacheFactory cacheFactory = new DataCacheFactory();
            DataCache = cacheFactory.GetDefaultCache();
        }

        public void Add(string key, object value)
        {
            if (ContainsKey(key))
            {
                Remove(key);
            }
            DataCache.Add(key, value);
        }

        public void Remove(string key)
        {
            DataCache.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            return DataCache.Get(key) != null;
        }

        public object Get(string key)
        {
            return DataCache[key];
        }

        public void SyncCache(string key)
        {

        }
    }
    
    public enum CacheType
    {
        Local,
        Azure,
        AzureLargeObjects,
    }

    public class Cache : ICache
    {
        public CacheType CacheType { get; private set; }
        public ICache SpecificCache { get; private set; }
        public Cache()
            : this(CacheType.Local)
        {
        }

        public Cache(CacheType cacheType)
        {
            this.CacheType = cacheType;
            SpecificCache = GetCache(CacheType);
        }
        public void Add(string key, object value)
        {
            SpecificCache.Add(key, value);
        }
        public void Remove(string key)
        {
            SpecificCache.Remove(key);
        }
        public bool ContainsKey(string key)
        {
            return SpecificCache.ContainsKey(key);
        }
        public object Get(string key)
        {
            return SpecificCache.Get(key);
        }

        public void SyncCache(string key)
        {
            SpecificCache.SyncCache(key);
        }

        protected virtual ICache GetCache(CacheType cacheType) 
        {
            ICache cache = null;
            switch (cacheType)
            {
                case CacheType.Azure:
                    cache = new AzureCache();
                    break;
                case CacheType.Local:
                    cache = new LocalCache();
                    break;
                case CacheType.AzureLargeObjects:
                    cache = new LargeObjectsAzureCache();
                    break;

                default:
                    break;
            }

            return cache;
        }
    }
}
