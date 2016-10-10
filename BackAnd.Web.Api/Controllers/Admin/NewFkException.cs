using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackAnd.Web.Api.Controllers.Admin
{
    public class NewFkException : Durados.DuradosException
    {
        public NewFkException(string objectName, string fieldName) : base("The object " + objectName + " already has a field " + fieldName)
        {

        }
    }
}
