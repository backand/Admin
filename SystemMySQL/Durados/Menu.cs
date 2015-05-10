using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Menu
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        //public int ID { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int Ordinal { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Root { get; set; }

        public virtual bool Special { get { return false; } }

        public List<View> Views { get; private set; }

        public List<Page> Pages { get; private set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "A group of views that have a common security configuration")]
        public int WorkspaceID { get; set; }


        public Menu()
        {
            UrlLinks = new Dictionary<string, UrlLink>(); 
            Views = new List<View>();
            Pages = new List<Page>();
            //ID = -1;
        }

        public bool HasVisibleViews()
        {
            if (Views.Count > 0)
            {
                foreach (View view in Views)
                {
                    if (!view.HideInMenu && view.IsAllow())
                        return true;
                }

                return false;
            }
            else
            {
                return false;
            }
            
        }

        [Durados.Config.Attributes.ChildrenProperty(TableName = "UrlLink", DictionaryKeyColumnName = "Title", Type = typeof(UrlLink))]
        public Dictionary<string, UrlLink> UrlLinks { get; private set; }

        
    }

}
