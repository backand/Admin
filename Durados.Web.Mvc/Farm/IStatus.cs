using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Farm
{
    public interface IStatus
    {
        void Clear(string appName);

        void Set(string appName);

        bool Contains(string appName);
    }
}
