using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Data
{
    public interface IStatusCache
    {
        Dictionary<string, string> GetCacheStatus();
    }
}
