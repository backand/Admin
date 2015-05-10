using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.Web.Mvc.Infrastructure;

namespace Durados.Web.Mvc.Specifics.Projects
{
    public class ErrorHandling : ISendAsyncErrorHandler
    {
        public void HandleError(Exception exception)
        {
            throw exception;
        }
    }
}
