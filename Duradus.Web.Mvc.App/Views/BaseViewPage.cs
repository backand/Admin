using System;
using System.Collections.Generic;
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
    public class BaseViewPage : System.Web.Mvc.ViewPage
    {
        public Durados.Web.Mvc.Database Database
        {
            get
            {
                return (Durados.Web.Mvc.Database)ViewData["Database"];
            }
        }
    }


   public class BaseViewPage<TModel> : ViewPage<TModel> where TModel : class
   {
       public Durados.Web.Mvc.Database Database
       {
           get
           {
               return (Durados.Web.Mvc.Database)ViewData["Database"];
           }
       }
   }

   
}
