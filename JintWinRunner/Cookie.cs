using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backand
{
    public class Cookie : ICookie
    {
        public object get(string key)
        {
            if (!File.Exists(key))
                return null;
            return File.ReadAllText(key);
        }
        public void put(string key, object value)
        {
            File.WriteAllText(key, value.ToString());
        }
        public void remove(string key)
        {
            if (File.Exists(key))
                File.Delete(key);
        }
    }

    public interface ICookie
    {
        object get(string key);
        void put(string key, object value);
        void remove(string key);
    }
}
