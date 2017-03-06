using Durados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackAnd.Web.Api.Controllers.Admin
{
    public class ValidateAuthAppException : DuradosException
    {
        public ValidateAuthAppException(string message)
            : base(message)
        {

        }
    }
}
