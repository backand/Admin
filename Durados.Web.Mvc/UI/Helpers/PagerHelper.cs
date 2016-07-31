using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.Web.Mvc;
using Durados.Web.Mvc.Infrastructure;

namespace System.Web.Mvc
{
    public static class PagerHelper
    {
        public static Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        public static string GoToFirstPage(this AjaxHelper helper, View view, int rowCount, string guid)
        {
            int currentPage = GetCurrentPage(view, guid);
            if (currentPage == 1)
            {
               // return "<img src='" + General.GetRootPath() + "Content/Images/PgFirst" + Map.Database.Localizer.Direction + "Disable.png" + "' >";
               return ImageActionLinkHelper.ImageLink(helper, "pagerFirstPage");
            }
            else
            {
                //return ImageActionLinkHelper.ImageActionLink(helper, "pagerFirstPage", General.GetRootPath() + "Content/Images/PgFirst" + Map.Database.Localizer.Direction + ".png", Map.Database.Localizer.Translate("Page") + " 1", view.IndexAction, view.Controller, new { viewName = view.Name, page = 1, guid = guid }, new AjaxOptions { UpdateTargetId = guid + "ajaxDiv", OnSuccess = "function(){ pageMoved('" + guid + "'); }", OnBegin = "begin", OnComplete = "function(){ complete('" + guid + "'); }" });
                return ImageActionLinkHelper.ImageActionLink(helper, "pagerFirstPage", Map.Database.Localizer.Translate("Page") + " 1", view.IndexAction, view.Controller, new { viewName = view.Name, page = 1, guid = guid }, new AjaxOptions { UpdateTargetId = guid + "ajaxDiv", OnSuccess = "function(){ pageMoved('" + guid + "'); }", OnBegin = "begin", OnComplete = "function(){ complete('" + guid + "'); }" });
            }
        }

        public static string GoToPrevPage(this AjaxHelper helper, View view, int rowCount, string guid)
        {
            int currentPage = GetCurrentPage(view, guid);
            if (currentPage == 1)
            {
                //return "<img src='" + General.GetRootPath() + "Content/Images/PgPrev" + Map.Database.Localizer.Direction + "Disable.png" + "' >";
                return ImageActionLinkHelper.ImageLink(helper, "pagerPrevPage");
            }
            else
            {
                int prevPage = GetPrevPage(view, guid);
                return ImageActionLinkHelper.ImageActionLink(helper, "pagerPrevPage", Map.Database.Localizer.Translate("Page") + " " + prevPage.ToString(), view.IndexAction, view.Controller, new { viewName = view.Name, page = prevPage, guid = guid }, new AjaxOptions { UpdateTargetId = guid + "ajaxDiv", OnSuccess = "function(){ pageMoved('" + guid + "'); }", OnBegin = "begin", OnComplete = "function(){ complete('" + guid + "'); }" });
            }
        }

        public static string GoToNextPage(this AjaxHelper helper, View view, int rowCount, string guid)
        {
            int currentPage = GetCurrentPage(view, guid);
            int pageCount = GetPageCount(view, rowCount, guid);
            if (currentPage == pageCount)
            {
                //return "<img src='" + General.GetRootPath() + "Content/Images/PgNext" + Map.Database.Localizer.Direction + "Disable.png" + "' >";
                return ImageActionLinkHelper.ImageLink(helper, "pagerNextPage");
            }
            else
            {
                int nextPage = GetNextPage(view, rowCount, guid);
                return ImageActionLinkHelper.ImageActionLink(helper, "pagerNextPage", Map.Database.Localizer.Translate("Page") + " " + nextPage.ToString(), view.IndexAction, view.Controller, new { viewName = view.Name, page = nextPage, guid = guid }, new AjaxOptions { UpdateTargetId = guid + "ajaxDiv", OnSuccess = "function(){ pageMoved('" + guid + "'); }", OnBegin = "begin", OnComplete = "function(){ complete('" + guid + "'); }" });
            }
        }

        public static string GoToLastPage(this AjaxHelper helper, View view, int rowCount, string guid)
        {
            int currentPage = GetCurrentPage(view, guid);
            int pageCount = GetPageCount(view, rowCount, guid);
            if (currentPage == pageCount)
            {
                //return "<img src='" + General.GetRootPath() + "Content/Images/PgLast" + Map.Database.Localizer.Direction + "Disable.png" + "' >";
                return ImageActionLinkHelper.ImageLink(helper, "pagerLastPage");
            }
            else
            {
                return ImageActionLinkHelper.ImageActionLink(helper, "pagerLastPage", Map.Database.Localizer.Translate("Page") + " " + pageCount.ToString(), view.IndexAction, view.Controller, new { viewName = view.Name, page = pageCount, guid = guid }, new AjaxOptions { UpdateTargetId = guid + "ajaxDiv", OnSuccess = "function(){ pageMoved('" + guid + "'); }", OnBegin = "begin", OnComplete = "function(){ complete('" + guid + "'); }" });
            }
        }

        public static int GetCurrentPage(View view, string guid)
        {
            if (HttpContext.Current.Session[guid + view.Name + "CurrentPage"] == null)
            {
                return 1;
            }
            return Convert.ToInt32(HttpContext.Current.Session[guid + view.Name + "CurrentPage"]);
        }

        public static void SetCurrentPage(View view, int page, string guid)
        {
            HttpContext.Current.Session[guid + view.Name + "CurrentPage"] = page;
        }

        //public static int GetCurrentPage(View view)
        //{
        //    if (HttpContext.Current.Session[view.Name + "CurrentPage"] == null)
        //    {
        //        return 1;
        //    }
        //    return Convert.ToInt32(HttpContext.Current.Session[view.Name + "CurrentPage"]);
        //}

        //public static void SetCurrentPage(View view, int page)
        //{
        //    HttpContext.Current.Session[view.Name + "CurrentPage"] = page;
        //}

        public static int GetPrevPage(View view, string guid)
        {
            int currentPage = GetCurrentPage(view, guid);
            if (currentPage > 1)
            {
                return currentPage - 1;
            }
            else
            {
                return 1;
            }
        }

        public static int GetPageSize(View view, string guid)
        {
            if (HttpContext.Current.Session[guid + view.Name + "PageSize"] == null)
            {
                return view.PageSize;
            }
            else
            {
                return (int)HttpContext.Current.Session[guid + view.Name + "PageSize"];
            }
        }

        public static void SetPageSize(View view, int pageSize, string guid)
        {
            HttpContext.Current.Session[guid + view.Name + "PageSize"] = pageSize;
            
        }

        public static int GetPageCount(View view, int rowCount, string guid)
        {
            int pageSize = GetPageSize(view, guid);
            if (pageSize > 0)
            {
                int pageCount = rowCount / pageSize;
                if (rowCount % pageSize == 0)
                    return pageCount;
                else
                    return pageCount + 1;
            }
            else
            {
                return 0;
            }
        }

        public static int GetNextPage(View view, int rowCount, string guid)
        {
            int currentPage = GetCurrentPage(view, guid);
            int pageCount = GetPageCount(view, rowCount, guid);
            if (currentPage < pageCount)
            {
                return currentPage + 1;
            }
            else
            {
                return currentPage;
            }
        }

        public static List<SelectListItem> GetPageSizeSelectList(View view, string guid)
        {
            int pageSize = GetPageSize(view, guid);

            List<SelectListItem> selectListItems = new List<SelectListItem>();

            int[] pageSizes = new int[10] {5,10,15,20,30,50,100,200,500,1000 };
            if (pageSizes.Contains(pageSize))
            {
                foreach (int value in pageSizes)
                {
                    selectListItems.Add(new SelectListItem() { Text = value.ToString(), Value = value.ToString(), Selected = pageSize == value });

                }
            }
            else
            {
                throw new InvalidViewPageSize(view, pageSize);
            }
            
            return selectListItems;
        }

        private static string GetPagerImg(string guid, string title, string className, string onClick)
        {
            string pagerImg = "";

            pagerImg = "<a><span class='" + className + "' title='" + title + "' alt='" + title + "' onclick=\"" + onClick + "\" /></a>";

            return pagerImg;
        }
    }

    public class InvalidViewPageSize : Durados.DuradosException
    {
        public InvalidViewPageSize(View view, int pageSize)
            : base("Page size options do not contain " + pageSize.ToString() + " of " + view.Name)
        {
        }
    }
}
