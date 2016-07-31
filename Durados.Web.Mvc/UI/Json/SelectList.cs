using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class SelectList
    {
        [DataMember]
        public Option[] Options { get; set; }

        public static SelectList GetSelectList(Dictionary<string, string> dictionary)
        {
            SelectList selectList = new SelectList();

            List<Option> options = new List<Option>();

            foreach (string key in dictionary.Keys)
            {
                options.Add(new Option() { Value = key, Text = dictionary[key] });
            }

            selectList.Options = options.ToArray();

            return selectList;
        }

        public static SelectList GetSelectList(IEnumerable<SelectListItem> selectElements)
        {
            SelectList selectList = new SelectList();

            List<Option> options = new List<Option>();

            foreach (SelectListItem element in selectElements)
            {
                options.Add(new Option() { Value = element.Value, Text = element.Text, Selected = element.Selected });
            }

            selectList.Options = options.ToArray();

            return selectList;
        }
    }

    [DataContract]
    public class Option
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public bool Selected { get; set; }

        public object Tag { get; set; }

        public override bool Equals(object obj)
        {
            return this.Value.Equals(((Option)obj).Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
