using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public static class HtmlList
    {
        public static Html GetHtml<T>(this IQueryable<T> htmlList, string name) where T : Html
        {
            var htmls = (from html in htmlList
                                where (html.Name.Equals(name))
                                select html);
            if (htmls == null)
            {
                return null;
            }

            return htmls.FirstOrDefault();
        }

        public static Html GetHtml<T>(this System.Data.Objects.DataClasses.EntityCollection<T> htmlList, string name) where T : Html
        {
            var htmls = (from html in htmlList
                         where (html.Name.Equals(name))
                         select html);
            if (htmls == null)
            {
                return null;
            }

            return htmls.FirstOrDefault();
        }

        public static Html GetHtml<T>(this IEnumerable<T> htmlList, string name) where T : Html
        {
            var htmls = (from html in htmlList
                         where (html.Name.Equals(name))
                         select html);
            if (htmls == null)
            {
                return null;
            }

            return htmls.FirstOrDefault();
        }

        public static Html GetHtml<T>(this System.Data.Objects.ObjectQuery<T> htmlList, string name) where T : Html
        {
            var htmls = (from html in htmlList
                         where (html.Name.Equals(name))
                         select html);
            if (htmls == null)
            {
                return null;
            }

            return htmls.FirstOrDefault();
        }
    }
}
