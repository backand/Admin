using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI
{
    public class DataActionFields
    {
        private Dictionary<string, Durados.Field> fields = null;

        public string Guid { get; set; }
        public Category Category { get; set; }
        public Durados.DataAction DataAction { get; set; }
        public List<Durados.Field> Fields { get; set; }
        public Durados.Web.Mvc.View View { get; set; }

        private void LoadDictionary()
        {
            fields = new Dictionary<string, Durados.Field>();

            foreach (Durados.Field field in Fields)
                fields.Add(field.Name, field);
        }

        public Durados.Field this[string name]
        {
            get
            {
                if (fields == null)
                    LoadDictionary();

                return fields[name];
            }
        }

        //public Durados.Web.Mvc.View View
        //{
        //    get
        //    {
        //        return (Durados.Web.Mvc.View)Fields.First().View;
        //    }
        //}
    }
}
