using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Durados.Web.Mvc.App.Helpers
{
    public class AssemblyHelper
    {
        private const string AspNetNamespace = "ASP";

        public static Assembly getWebApplicationAssembly()
        {
            //Guard.AgainstNullArgument(context);
            HttpContext context = System.Web.HttpContext.Current;
            IHttpHandler handler = context.CurrentHandler;
            if (handler == null) return null;

            Type type = handler.GetType();
            while (type != null && type != typeof(object) && type.Namespace == AspNetNamespace)
                type = type.BaseType;

            return type.Assembly;
        }
        public static  Assembly GetWebEntryAssembly()
        {
            if (System.Web.HttpContext.Current == null ||
                System.Web.HttpContext.Current.ApplicationInstance == null)
            {
                return null;
            }

            var type = System.Web.HttpContext.Current.ApplicationInstance.GetType();
            while (type != null && type.Namespace == "ASP")
            {
                type = type.BaseType;
            }

            return type == null ? null : type.Assembly;
        }
    }
}
