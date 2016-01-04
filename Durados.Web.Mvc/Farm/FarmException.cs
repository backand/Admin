using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Farm
{
    public class FarmException : DuradosException
    {
        public FarmException(string message) : base(message)
        {

        }
    }

    public class PublishSyncTimeoutException : FarmException
    {
        public PublishSyncTimeoutException()
            : base("Load balance publish timeout")
        {

        }
    }
     

}
