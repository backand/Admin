using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackAnd.Web.Api
{
    public interface IHasMap
    {
        Durados.Web.Mvc.Map Map
        {
            get;
        }
    }
}