using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class SpecialMenu : Menu
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public bool DisplayState { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool HideFromMenu { get; set; }

        public SpecialMenu()
        {
            Links = new List<Link>();
            Menus = new Dictionary<string, SpecialMenu>(StringComparer.InvariantCultureIgnoreCase);
        }

        public override bool Special
        {
            get
            {
                return true;
            }
        }



        public bool IsEmpty(Database database)
        {
            foreach (SpecialMenu menu in Menus.Values)
            {
                if (!menu.IsEmpty(database))
                    return false;
            }

            if (Links == null)
                return true;
            int countReport = Links.Where(l => !string.IsNullOrEmpty(l.ReportServicePath)).Count();
            int countUrl = Links.Where(l => !string.IsNullOrEmpty(l.Url)).Count();
            int countViews = Links.Where(l => string.IsNullOrEmpty(l.Url) && !(string.IsNullOrEmpty(l.ViewName)) && database.Views.ContainsKey(l.ViewName) && database.Views[l.ViewName].IsAllow()).Count();

            return countUrl + countViews + countReport == 0;
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public int ID { get; set; }


        private LinkType linkType = LinkType.View;

        [Durados.Config.Attributes.ColumnProperty()]
        public LinkType LinkType
        {
            get
            {
                // remove after a while
                if (linkType == LinkType.View && !string.IsNullOrEmpty(Url) && Url.StartsWith("/Home/Page?pageId="))
                {
                    return Durados.LinkType.Page;
                }
                
                // end remove after a while
                return linkType;
            }
            set
            {
                linkType = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Url { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "The target of the page. Empty by default. If empty the page will be open in the current window.")]
        public string Target { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ReportServicePath { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ViewName { get; set; }

        [Durados.Config.Attributes.ChildrenProperty(TableName = "Link", Type = typeof(Link))]
        public List<Link> Links { get; private set; }

        [Durados.Config.Attributes.ChildrenProperty(TableName = "SpecialMenu", Type = typeof(SpecialMenu), DictionaryKeyColumnName = "Name")]
        public Dictionary<string, SpecialMenu> Menus { get; private set; }

        public SpecialMenu Parent { get; set; }

        const string pathSeperator = " - ";
        private string path = null;
        public string GetPath()
        {
            if (string.IsNullOrEmpty(path))
            {
                SpecialMenu parent = this; ;
                while (parent != null)
                {
                    path = parent.Name + pathSeperator + path;
                    parent = parent.Parent;
                }

                if (path == pathSeperator)
                    path = string.Empty;
            }

            return path;
        }

        public string GetConfigViewName()
        {
            switch (LinkType)
            {
                case Durados.LinkType.View:
                    return "View";

                case Durados.LinkType.Page:
                    return "Page";

                default:
                    return "MyCharts";
            }
        }

        public string GetMenuUrl()
        {
            if (string.IsNullOrEmpty(Url))
                return "#";

            string menuUrl = Url + (Url.Contains('?') ? "&" : "?") + "menuId=" + ID;
            
            return menuUrl;
        }

        public int Generation
        {
            get
            {
                if (Parent == null)
                    return 0;

                return Parent.Generation + 1;
            }
        }

        public void SetSpecialMenusParent(SpecialMenu parent)
        {
            foreach (SpecialMenu specialMenu in Menus.Values)
            {
                specialMenu.Parent = parent;
                specialMenu.SetSpecialMenusParent(this);
            }
        }

        public SpecialMenu GetMenuById(int id)
        {
            foreach (SpecialMenu menu in Menus.Values)
            {
                if (menu.ID == id)
                    return menu;
                else
                {
                    menu.GetMenuById(id);
                }
            }

            return null;
        }
    }

    public class Nestable
    {
        public string id { get; set; }
        public Nestable[] children { get; set; }
    }
}
