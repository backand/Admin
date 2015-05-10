using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class ContentParameter
    {
        public string ParameterName
        {
            get
            {
                if (!ParameterReference.IsLoaded)
                    ParameterReference.Load();
                if (Parameter == null)
                    return string.Empty;
                return Parameter.Name;
            }
        }

        public string ParameterValue
        {
            get
            {
                if (!ParameterReference.IsLoaded)
                    ParameterReference.Load();
                if (Parameter == null)
                    return string.Empty;
                return Parameter.Value;
            }
        }
    }
}
