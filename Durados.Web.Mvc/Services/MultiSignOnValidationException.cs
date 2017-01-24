using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Services
{
    public class MultiSignOnValidationException : Exception
    {
        public MultiSignOnValidationException(Exception innerException) : base("Failed to compare password", innerException)
        {

        }
    }
}
