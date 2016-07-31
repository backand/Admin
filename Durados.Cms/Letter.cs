using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms
{
    public class Letter
    {
        public Letter()
        {
        }

        public virtual string GetMessage(string htmlName, object[] args)
        {
            Durados.Cms.Model.Html html = Durados.Cms.Singleton.Cms.HtmlSet.Where(h => h.Name == htmlName).FirstOrDefault();
            return GetMessage(html, args);
        }

        public virtual string GetMessage(Durados.Cms.Model.Html html, object[] args)
        {
            return string.Format(html.Text, args);
        }

        
    }
}
