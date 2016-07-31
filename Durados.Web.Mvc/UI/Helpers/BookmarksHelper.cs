using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Durados.DataAccess;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class BookmarksHelper
    {
        private static Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        public static List<Bookmark> UserBookmarks
        {
            get
            {
                if (HttpContext.Current.Session["Bookmarks"] == null)//load from db
                {
                    List<Bookmark> bmList = new List<Bookmark>();

                    View LinkView = GetLinkView();

                    int UserId = Convert.ToInt32(Map.Database.GetUserID());

                    int rowCount;

                    Dictionary<string, object> values = new Dictionary<string, object>();//Filter

                    Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();

                    //Bookmarks load:

                    values.Add("UserId", UserId);
                    values.Add("LinkType", 0);

                    sortColumns.Add("Ordinal", SortDirection.Asc);
                    sortColumns.Add("Id", SortDirection.Desc);

                    DataView dvBookmarks = LinkView.FillPage(1, 100, values, false, sortColumns, out rowCount, null, null);

                    FillBookmarksList(bmList, dvBookmarks);

                    //Recents load:
                    values["LinkType"] = 1;

                    sortColumns.Remove("Ordinal");                    
                    
                    dvBookmarks = LinkView.FillPage(1, Map.Database.RecentsCount, values, false, sortColumns, out rowCount, null, null);

                    FillBookmarksList(bmList, dvBookmarks);

                    HttpContext.Current.Session["Bookmarks"] = bmList;

                }

                List<Bookmark> bookmarks = new List<Bookmark>();

                bookmarks.AddRange(Map.GetPublicBoolmarks());
                bookmarks.AddRange((List<Bookmark>)HttpContext.Current.Session["Bookmarks"]);

                return bookmarks;
            }
        }

        public static List<Bookmark> GetPublicBookmarks()
        {
            List<Bookmark> bmList = new List<Bookmark>();

            View LinkView = GetLinkView();

            int UserId = -1;

            int rowCount;

            Dictionary<string, object> values = new Dictionary<string, object>();//Filter

            Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();

            //Bookmarks load:

            values.Add("UserId", UserId);
            values.Add("LinkType", 0);

            sortColumns.Add("Ordinal", SortDirection.Asc);
            sortColumns.Add("Id", SortDirection.Desc);

            DataView dvBookmarks = LinkView.FillPage(1, 100, values, false, sortColumns, out rowCount, null, null);

            FillBookmarksList(bmList, dvBookmarks);

            //Recents load:
            values["LinkType"] = 1;

            sortColumns.Remove("Ordinal");

            dvBookmarks = LinkView.FillPage(1, Map.Database.RecentsCount, values, false, sortColumns, out rowCount, null, null);

            FillBookmarksList(bmList, dvBookmarks);

            return bmList;
        }


        public static void FillBookmarksList(List<Bookmark> bmList, DataView dvBookmarks)
        {
            foreach (System.Data.DataRowView row in dvBookmarks)
            {
                Bookmark bm = new Bookmark();

                bm.Type = IntOrDefault(row.Row["LinkType"]);
                bm.Id = IntOrDefault(row.Row["Id"]);
                bm.Ordinal = IntOrDefault(row.Row["Ordinal"]);
                bm.Name = StringOrDefault(row.Row["Name"]);                
                bm.Filter = StringOrDefault(row.Row["Filter"]);
                bm.Url = StringOrDefault(row.Row["Url"]);
                bm.ViewName = StringOrDefault(row.Row["ViewName"]);
                bm.ControllerName = StringOrDefault(row.Row["ControllerName"]);
                bm.Guid = StringOrDefault(row.Row["Guid"]);  

                bm.SortColumn = StringOrDefault(row.Row["SortColumn"]);
                bm.SortDirection = StringOrDefault(row.Row["SortDirection"]);
                bm.PageNo = IntOrDefault(row.Row["PageNo"]);
                bm.PageSize = IntOrDefault(row.Row["PageSize"]);
                bm.Description = StringOrDefault(row.Row["Description"]);

                bmList.Add(bm);
            }
        }

        
        public static View GetLinkView()
        {
            if (Map.Database.Views.ContainsKey("durados_Link"))
                return (Durados.Web.Mvc.View)Map.Database.Views["durados_Link"];
            return null;            
        }

        public static string StringOrDefault(object Str) {
            if (Str==null) return string.Empty;
            return Str.ToString();
        }

        public static int IntOrDefault(object Int) {
            if (Int == null) return 0;
            return Convert.ToInt32(Int);
        }

        /*
        public static int CreateBookmark() {
            Durados.Web.Mvc.View LinkView = GetLinkView();
            return 0;
        }
        */

        public static List<Bookmark> GetCachedBookmarks(int Type)
        {
            if (Type == 1)
            {
                return UserBookmarks.Where(bm => bm.Type == Type).Take(Map.Database.RecentsCount).ToList();
            }
            return UserBookmarks.Where(bm => bm.Type == Type).ToList();
        }

        public static Bookmark GetBookmark(int Id)
        {
            Bookmark UserBM = UserBookmarks.Where(bm => bm.Id == Id).FirstOrDefault();

            if (UserBM != null) return UserBM;

            List<Bookmark> bmList = new List<Bookmark>();

            View LinkView = GetLinkView();

            int rowCount;

            Dictionary<string, object> values = new Dictionary<string, object>();//Filter

            values.Add("Id", Id);

            DataView dvBookmarks = LinkView.FillPage(1, 1, values, false, null, out rowCount, null, null);

            FillBookmarksList(bmList, dvBookmarks);

            if (bmList.Count > 0)
            {
                return bmList[0];
            }

            return null;
        }

        public static void AddBookmarkToCache(Bookmark bookmark)
        {
            if (HttpContext.Current.Session["Bookmarks"] != null && UserBookmarks.Count > 0)
                UserBookmarks.Insert(0, bookmark);
        }

        public static void RemoveBookmarkFromoCache(int Id)
        {
            UserBookmarks.Remove(GetBookmark(Id));
        }

        public static void ClearCache()
        {
            HttpContext.Current.Session["Bookmarks"] = null;
        }

    }
}
