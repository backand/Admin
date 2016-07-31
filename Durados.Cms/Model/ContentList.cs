using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public static class ContentList
    {
        public static Content GetContent<T>(this IQueryable<T> contentList, string contentName) where T : Content
        {
            var contents = (from content in contentList
                            where (content.Name.Equals(contentName))
                            select content);
            if (contents == null)
            {
                return null;
            }

            return contents.FirstOrDefault();
        }

        public static Content GetContent<T>(this System.Data.Objects.DataClasses.EntityCollection<T> contentList, string contentName) where T : Content
        {
            if (!contentList.IsLoaded)
                contentList.Load();

            var contents = (from content in contentList
                            where (content.Name.Equals(contentName))
                            select content);
            if (contents == null)
            {
                return null;
            }

            return contents.FirstOrDefault();
        }
        

        
    }
}
