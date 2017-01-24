using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Services
{
    public class HashPasswordException : DuradosException
    {
        public HashPasswordException(Exception innerException)
            : base("Failed to hash password", innerException)
        {

        }
    }
}
