﻿using Durados.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Data
{
    public class LocalCache<T> : ICache<T>
    {
        private ObjectCache cache;

        public LocalCache(string name)
        {
            cache = new MemoryCache(name);
        }

        public void Add(string key, T value)
        {
            cache[key] = value;
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            return cache.Contains(key);
        }

        public T Get(string key)
        {
            return (T)cache.Get(key);
        }

        public T this[string key]
        {
            get
            {
                return (T)cache[key];
            }
            set
            {
                cache[key] = value;
            }
        }

        public IEnumerable<T> Values
        {
            get
            {
                return cache.ToList().Select(a => a.Value).Cast<T>();
            }
        }


        public Dictionary<string, T> ToDictionary()
        {
            return cache.ToDictionary(a => a.Key, a => ((T)a.Value));
        }
    }
}
