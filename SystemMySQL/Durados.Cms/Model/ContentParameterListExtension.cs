using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace Durados.Cms.Model
{
    public static class ContentParameterListExtension
    {
        public static string GetParameterValue<T>(this System.Data.Objects.DataClasses.EntityCollection<T> contentParameterList, string name) where T : ContentParameter
        {
            if (!contentParameterList.IsLoaded)
                contentParameterList.Load();

            var contentParameters = (from contentParameter in contentParameterList
                                     where (contentParameter.ParameterName.Equals(name))
                                     select contentParameter);
            if (contentParameters == null)
            {
                return string.Empty;
            }

            return (string)contentParameters.FirstOrDefault().ParameterValue;
        }

    }
}
