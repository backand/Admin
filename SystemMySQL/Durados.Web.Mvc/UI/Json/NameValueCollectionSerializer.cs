using System;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
     public class NameValueCollectionSerializer
    {
        const char seperator = '^';
        #region Private members
        private NameValueCollection nameValueCollection;
        #endregion

        #region Constructors
        public NameValueCollectionSerializer(string s)
        {
            nameValueCollection = FromString(s);
        }

        public NameValueCollectionSerializer
            (NameValueCollection nameValueCollection)
        {
            this.nameValueCollection = nameValueCollection;
        }
        #endregion

        #region Properties
        public NameValueCollection NameValueCollection
        {
            get { return this.nameValueCollection; }
            set { this.nameValueCollection = value; }
        }
        #endregion

        public override string ToString()
        {
            string s = string.Empty;

            foreach (string key in nameValueCollection.Keys)
            {
                s += key + seperator;
            }

            foreach (string key in nameValueCollection.Keys)
            {
                s += nameValueCollection[key] + seperator;
            }

            s = s.TrimEnd(seperator);

            return s;
        }

        private NameValueCollection FromString(string s)
        {
            NameValueCollection c = new NameValueCollection();

            string[] ss = s.Split(seperator);

            int count = ss.Length / 2;

            for (int i = 0; i < count; i++)
            {
                c.Add(ss[i], ss[i+count]);
            }

            return c;
        }
    }
}
