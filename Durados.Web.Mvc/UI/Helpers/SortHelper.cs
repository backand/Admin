using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Durados.Web.Mvc;

namespace System.Web.Mvc
{
    public static class SortHelper
    {
        public static string GetSortColumn(View view)
        {
            if (HttpContext.Current.Session[view.Name + "sortColumn"] == null)
            {
                return null;
            }
            return Convert.ToString(HttpContext.Current.Session[view.Name + "sortColumn"]);
        }

        public static void SetSortColumn(View view, string sortColumn)
        {
            HttpContext.Current.Session[view.Name + "sortColumn"] = sortColumn;
        }

        public static string GetSortDirection(View view)
        {
            if (HttpContext.Current.Session[view.Name + "sortDirection"] == null)
            {
                return "Asc";
            }
            return Convert.ToString(HttpContext.Current.Session[view.Name + "sortDirection"]);
        }

        public static void SetSortDirection(View view, string direction)
        {
            HttpContext.Current.Session[view.Name + "sortDirection"] = direction;
        }
    }
}
