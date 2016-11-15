using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class OverTheActionParametersSizeLimitException : Durados.DuradosException
    {
        public OverTheActionParametersSizeLimitException(int sizeLimit, int parametersSize)
            : base("The action parameters size is limited to " + sizeLimit + " KB. The current parameters size is " + parametersSize + " KB. Either reduce the parameters size or contact the administrator.")
        {

        }
    }
}
