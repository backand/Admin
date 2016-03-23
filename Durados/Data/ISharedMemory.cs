using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Data
{
    public interface ISharedMemory
    {
        string Get(string appName, SharedMemoryKey key);

        void Set(string appName, SharedMemoryKey key, string value, int milliseconds);

        void Delete(string appName, SharedMemoryKey key);

        bool Contains(string appName, SharedMemoryKey key);

        void ListRightPush(string key, string value);
    }


    public enum SharedMemoryKey
    {
        DebugMode,
    }
}
