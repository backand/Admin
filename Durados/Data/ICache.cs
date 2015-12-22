using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Data
{
    public interface ICache<T>
    {
        void Add(string key, T value);

        void Remove(string key);

        bool ContainsKey(string key);

        T Get(string key);

        T this[string key] { get; set; }

        IEnumerable<T> Values { get; }
    }

}
