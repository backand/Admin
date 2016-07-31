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

using Durados.Web.Mvc;

namespace System.Web.Mvc.UI.Views
{
    public class BaseViewPage : System.Web.Mvc.ViewPage
    {
        private Map map = null;
        public Map Map
        {
            get
            {
                if (map == null)
                    map = Maps.Instance.GetMap();
                return map;
            }
        }

        public Durados.Web.Mvc.Database Database
        {
            get
            {
                if (ViewData["Database"] == null)
                    return Map.Database;
                else
                    return (Durados.Web.Mvc.Database)ViewData["Database"];
            }
        }

        public Durados.DataAction DataAction
        {
            get
            {
                if (ViewData["DataAction"] == null)
                    return Durados.DataAction.Create;
                else
                    return (Durados.DataAction)ViewData["DataAction"];
            }
        }
    }


    public class BaseViewPage<TModel> : ViewPage<TModel> where TModel : class
    {
        private Map map = null;
        public Map Map
        {
            get
            {
                if (map == null)
                    map = Maps.Instance.GetMap();
                return map;
            }
        }

        public Durados.Web.Mvc.Database Database
        {
            get
            {
                if (ViewData["Database"] == null)
                    return Map.Database;
                else
                    return (Durados.Web.Mvc.Database)ViewData["Database"];
            }
        }
    }

    public class MasterPage : System.Web.Mvc.ViewMasterPage
    {
        private Map map = null;
        public Map Map
        {
            get
            {
                if (map == null)
                    map = Maps.Instance.GetMap();
                return map;
            }
        }

    }
}
