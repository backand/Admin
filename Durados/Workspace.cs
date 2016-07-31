using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Workspace
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int ID { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "If true view takes the workspace roles, otherwise the view takes the database roles")]
        public bool Precedent { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int Ordinal { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="The description of the workspace. This description will be displayed to the users in the Workspace page.")]
        public string Description { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int HomePage { get; set; }

        [Durados.Config.Attributes.ChildrenProperty(TableName = "SpecialMenu", Type = typeof(SpecialMenu), DictionaryKeyColumnName = "Name")]
        public Dictionary<string, SpecialMenu> SpecialMenus { get; set; }


        public SpecialMenu GetHomePageSpecialMenu(Func<SpecialMenu, Boolean> IsAuthorized)
        {
            if (SpecialMenus == null)
                return null;

            return GetSpecialMenu(SpecialMenus, HomePage, IsAuthorized);
        }

        public SpecialMenu GetSpecialMenu(int id)
        {
            return GetSpecialMenu(id, null);
        }
        public SpecialMenu GetSpecialMenu(int id, Func<SpecialMenu, Boolean> IsAuthorized)
        {
            if (SpecialMenus == null)
                return null;
            return GetSpecialMenu(SpecialMenus, id, IsAuthorized);
        }


        public SpecialMenu GetSpecialMenu(string viewName)
        {
            if (SpecialMenus == null)
                return null;
            return GetSpecialMenuByViewName(SpecialMenus, viewName);
        }

        //private SpecialMenu GetHomePageSpecialMenu(Dictionary<string, SpecialMenu> specialMenus, int id)
        //{
        //    if (specialMenus == null || specialMenus.Count == 0)
        //        return null;
        //    else
        //        if (specialMenus.ContainsKey(HomePage))
        //            return specialMenus[id];
        //        else
        //        {
        //            foreach (SpecialMenu specialMenu in specialMenus.Values)
        //            {
        //                return GetHomePageSpecialMenu(specialMenus, id);
        //            }
        //            return null;
        //        }
                    
        //}

        private SpecialMenu GetSpecialMenu(Dictionary<string, SpecialMenu> specialMenus, int id, Func<SpecialMenu, Boolean> IsAuthorized)
        {
            // is IsAuthorized is numm => return always true
            if (IsAuthorized == null)
            {
                IsAuthorized = (menu) => true;
            }
            var s = specialMenus.Values.FirstOrDefault(sm => sm.ID == id && IsAuthorized(sm));
            if (s != null)
                return s;
            else
            {
                foreach (SpecialMenu specialMenu in specialMenus.Values)
                {
                    s = GetSpecialMenu(specialMenu.Menus, id, IsAuthorized);
                    if (s != null)
                        return s;
                }
                return null;
            }

        }

        private SpecialMenu GetSpecialMenuByViewName(Dictionary<string, SpecialMenu> specialMenus, string viewName)
        {
            var s = specialMenus.Values.Where(sm => sm.ViewName == viewName).FirstOrDefault();
            if (s != null)
                return s;
            else
            {
                foreach (SpecialMenu specialMenu in specialMenus.Values)
                {
                    s = GetSpecialMenuByViewName(specialMenu.Menus, viewName);
                    if (s != null)
                        return s;
                }
                return null;
            }

        }


        #region Allow
        [Durados.Config.Attributes.ColumnProperty()]
        public string AllowCreateRoles{ get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string AllowEditRoles{ get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string AllowSelectRoles{ get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string AllowDeleteRoles { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string ViewOwnerRoles { get; set; }
        #endregion

        #region Deny
        [Durados.Config.Attributes.ColumnProperty()]
        public string DenyCreateRoles { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string DenyEditRoles { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string DenyDeleteRoles { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string DenySelectRoles { get; set; }
        #endregion

        public string GetDenySelectRoles(Database database)
        {
            if (Precedent)
                return DenySelectRoles;
            else
                return database.DefaultDenySelectRoles;
        }

        public string GetDenyDenyCreateRoles(Database database)
        {
            if (Precedent)
                return DenyCreateRoles;
            else
                return database.DefaultDenyCreateRoles;
        }

        public string GetDenyEditRoles(Database database)
        {
            if (Precedent)
                return DenyEditRoles;
            else
                return database.DefaultDenyEditRoles;
        }

        public string GetDenyDeleteRoles(Database database)
        {
            if (Precedent)
                return DenyDeleteRoles;
            else
                return database.DefaultDenyDeleteRoles;
        }

        public string GetAllowSelectRoles(Database database)
        {
            if (Precedent)
                return AllowSelectRoles;
            else
                return database.DefaultAllowSelectRoles;
        }

        public string GetAllowCreateRoles(Database database)
        {
            if (Precedent)
                return AllowCreateRoles;
            else
                return database.DefaultAllowCreateRoles;
        }

        public string GetAllowEditRoles(Database database)
        {
            if (Precedent)
                return AllowEditRoles;
            else
                return database.DefaultAllowEditRoles;
        }

        public string GetAllowDeleteRoles(Database database)
        {
            if (Precedent)
                return AllowDeleteRoles;
            else
                return database.DefaultAllowDeleteRoles;
        }

        public string GetViewOwnerRoles(Database database)
        {
            if (Precedent)
                return ViewOwnerRoles;
            else
                return database.DefaultViewOwnerRoles;
        }

        //public List<View> Views { get; private set; }

        public Workspace()
        {
            //Views = new List<View>();
            Precedent = true;
            ID = -1;
            SpecialMenus = new Dictionary<string, SpecialMenu>(StringComparer.InvariantCultureIgnoreCase);
            HomePage = -1;
        }

        internal List<Menu> Menus { get; set; }

        internal List<View> Views { get; set; }

        internal List<View> ViewsWithoutMenu { get; set; }

        internal List<Page> PagesWithoutMenu { get; set; }
    }
}
