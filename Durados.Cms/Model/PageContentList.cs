using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public static class PageContentList
    {
        public static List<T> GetPageContentList<T>(this IQueryable<T> pageContentList, string placeHolder) where T : PageContent
        {
            var pageContents = (from pageContent in pageContentList
                                where (pageContent.PlaceHolderName.Equals(placeHolder))
                                orderby pageContent.Ordinal
                                select pageContent);
            if (pageContents == null)
            {
                return null;
            }

            return pageContents.ToList();
        }

        public static List<T> GetPageContentList<T>(this System.Data.Objects.DataClasses.EntityCollection<T> pageContentList, string placeHolder) where T : PageContent
        {
            if (!pageContentList.IsLoaded)
                pageContentList.Load();

            var pageContents = (from pageContent in pageContentList
                                where (pageContent.PlaceHolderName.Equals(placeHolder))
                                orderby pageContent.Ordinal
                                select pageContent);
            if (pageContents == null)
            {
                return null;
            }

            return pageContents.ToList();
        }

        public static List<T> GetPageContentList<T>(this System.Data.Objects.DataClasses.EntityCollection<T> pageContentList) where T : PageContent
        {
            if (!pageContentList.IsLoaded)
                pageContentList.Load();

            return pageContentList.ToList();
        }
        
        public static List<T> GetPageContentList<T>(this IEnumerable<T> pageContentList, string placeHolder) where T : PageContent
        {
            var pageContents = (from pageContent in pageContentList
                                where (pageContent.PlaceHolderName.Equals(placeHolder))
                                orderby pageContent.Ordinal
                                select pageContent);
            if (pageContents == null)
            {
                return null;
            }

            return pageContents.ToList();
        }

        
    }
}
