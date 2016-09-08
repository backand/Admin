using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class QueryNotFoundException : DuradosException
    {
        public QueryNotFoundException(int queryId)
            : base("The query was not found id: " + queryId)
        {

        }
    }
}
