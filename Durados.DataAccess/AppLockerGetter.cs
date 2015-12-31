using Durados.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess
{
    public class AppLockerGetter : ILockGetter
    {
        ICache<object> cache;

        public AppLockerGetter(ICache<object> cache)
        {
            this.cache = cache;
        }

        private object locker2 = new object();

        public object GetLock(string key)
        {
            if (!cache.ContainsKey(key))
            {
                lock (locker2)
                {
                    if (!cache.ContainsKey(key))
                    {
                        var locker = new object();
                        cache.Add(key, locker);
                        return locker;
                    }
                }
            }

            return cache[key];
        }
    }
}
