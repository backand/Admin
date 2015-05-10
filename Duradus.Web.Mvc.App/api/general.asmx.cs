using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.App.api
{
    /// <summary>
    /// Summary description for general
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class general : System.Web.Services.WebService
    {
        private Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }


        private const string seperator = "/";

        [WebMethod]
        public List<MenuList> GetMenu()
        {
            List<MenuList> list = new List<MenuList>();

            foreach (Durados.Menu menu in Map.Database.Menus.Values.OrderBy(m => m.Ordinal))
            {
                if (menu.Root)
                {
                    AddSubMenu(list, menu, "");
                }
                else if (menu.HasVisibleViews())
                {
                    string prefix = Map.Database.Localizer.Translate(menu.Name);
                    AddSubMenu(list, menu, prefix);
                }
                if (Map.Database.HasViewsWithNoMenu)
                {
                    AddSubMenu(list, Map.Database.ViewsWithNoMenu, "");
                }
            }

            Durados.Web.Mvc.Database configDatabase = Map.GetConfigDatabase();
            if (!SecurityHelper.IsDenied("", configDatabase.AllowConfigConfigRoles) && configDatabase.HasViewsWithNoMenu)
            {
                string prefix = Map.Database.Localizer.Translate("Admin");
                AddSubMenu(list, configDatabase.ViewsWithNoMenu, prefix);
            }


            //foreach (Durados.Web.Mvc.View view in Map.Database.GetVisibleViews())
            //{
            //    string href = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + General.GetRootPath() + view.Controller + seperator + view.IndexAction + seperator + view.Name;
            //    list.Add(new MenuList { Name = view.DisplayName, Url = href });
            //}

            return list;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="menu"></param>
        private void AddSubMenu(List<MenuList> list, Durados.Menu menu, string prefix)
        {
            if (menu != null && menu.Views != null)
            {
                foreach (Durados.Web.Mvc.View view in menu.Views.OrderBy(v => (int)(v == null ? 0 : v.Order)))
                {
                    if (view != null && view.IsVisible())
                    {
                        string href = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + General.GetRootPath() + view.Controller + seperator + view.IndexAction + seperator + view.Name;
                        string name = (prefix != "") ? prefix + " - " + view.GetLocalizedDisplayName() : view.GetLocalizedDisplayName();
                        list.Add(new MenuList { Name = name, Url = href });
                    }
                }
            }
        }
    }

    [Serializable]
    public class MenuList
    {
        public string Name;
        public string Url;

    }
}
