using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess
{
    public interface ILockGetter
    {
        object GetLock(string key);
    }
}
