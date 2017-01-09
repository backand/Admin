using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess
{
    public class MissingRequiredFieldWithoutDefaultValue : DuradosException
    {
        public MissingRequiredFieldWithoutDefaultValue(Field field)
            : base(String.Format("Please provide a value for the field '{0}' that is required and has no default value.", field.JsonName))
        {

        }
    }
}
