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


        void Delete(string key);

        void Set(string key, string value, int milliseconds);

        string Get(string key);

        bool Contains(string key);
    }


    public enum SharedMemoryKey
    {
        DebugMode,
    }
}
