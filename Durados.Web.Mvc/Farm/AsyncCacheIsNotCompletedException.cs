using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Farm
{
    public class AsyncCacheIsNotCompletedException : DuradosException
    {
        public AsyncCacheIsNotCompletedException() : base("Async cache is not completed") { }
    }
}
