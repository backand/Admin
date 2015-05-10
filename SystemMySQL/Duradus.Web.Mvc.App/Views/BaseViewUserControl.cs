using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;

namespace System.Web.Mvc
{
    public class BaseViewUserControl : System.Web.Mvc.ViewUserControl
    {
        public Durados.Web.Mvc.Database Database
        {
            get
            {
                return (Durados.Web.Mvc.Database)ViewData["Database"];
            }
        }

        public string ViewName
        {
            get
            {
                return ViewData["ViewName"].ToString();
            }
        }
    }

    public class BaseViewUserControl<TModel> : ViewUserControl<TModel> where TModel : class
    {
        public Durados.Web.Mvc.Database Database
        {
            get
            {
                return (Durados.Web.Mvc.Database)ViewData["Database"];
            }
        }

        public string ViewName
        {
            get
            {
                return ViewData["ViewName"].ToString();
            }
        }
    }
}
