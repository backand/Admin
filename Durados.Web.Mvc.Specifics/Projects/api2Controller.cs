using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Durados;
using System.Data;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Controllers;
using Durados.DataAccess;

namespace Durados.Web.Mvc.App.Controllers
{
//      1.      url (get):  /api2/getview?bookmarkid=5800&pageSize=10000&pageNumber=3&id=s5dd5s65cx56s65c53   

//      2.      JSON results definition:
//              {results:[{name1: value11,name2:value21...},{name1: value12,name2:value22…},…], rowCount:1235896}

    public class api2Controller:CrmController
    {

        public JsonResult getview(int bookmarkid, int? pageSize, int? pageNumber)
        {
            Guid guid = Guid.NewGuid();
            Database.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "getview(" + guid.ToString() + ")", "Start", null, 3, "Parameters:" + bookmarkid.ToString(), DateTime.Now);
            if (!IsValidatUser())
                return Json(new TableForJson { ErrorCode = 101, ErrorDescription = "User is not recognize" });

            int maxPageSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ApiGetViewMaxPageSize"] ?? "10000");
            if (!pageSize.HasValue)
                pageSize = maxPageSize;

            if (pageSize.HasValue && pageSize.Value > maxPageSize)
            {
                return Json(new TableForJson { ErrorCode = 121, ErrorDescription = "Page size exceeded the allowed value of 10000" });
            }
            if (!pageNumber.HasValue)
                pageNumber = 1;

            List<Dictionary<object, object>> rows;
            int rowCount;
            try
            {
                Durados.Bookmark bookmark = BookmarksHelper.GetBookmark(bookmarkid);
                if (bookmark == null)
                    return Json(new TableForJson { ErrorCode = 102, ErrorDescription = "Bookmark was not found" }); ;

                Durados.Web.Mvc.View view = GetView(bookmark.ViewName, "getview");
                if (view == null)
                    return Json(new TableForJson { ErrorCode = 103, ErrorDescription = "View " + bookmark.ViewName + "was not found" });
                if (view.Database.IsConfig)
                    return Json(new TableForJson { ErrorCode = 104, ErrorDescription = "Bookmark view is a config view! notofication services for these views is not implemeneted." });



                DataAccess.Filter filter = MailingServiceHelper.GetBookmarkFilter(view, bookmark, new SqlAccess());
                Dictionary<string, SortDirection> sortColumns = GetSortColumns(bookmark.SortColumn, bookmark.SortDirection);
                DataView dataview;
                try
                {
                    dataview = view.FillPage(pageNumber.Value, pageSize.Value, filter, null, null, sortColumns, out rowCount, null, null);//GetDataViewByBookmark(bookmarkid,pageNumber,pageSize,out rowCount);
                }
                catch (Exception ex)
                {
                    throw new DuradosException("Faild to fill data view from database", ex);
                }
                //DataTable dt;
                //try
                //{
                //    dt = CreateResponseDataTable(view, dataview);
                //}
                //catch (Exception ex)
                //{
                //    throw new DuradosException("Faild to create response ", ex);
                //}

                rows = dataview.GetTableForJson(view, rowCount, pageNumber.Value);

                TableForJson tableJson = new TableForJson();
                tableJson.totalRowCount = rowCount;
                tableJson.results = rows;
                tableJson.rowsInPage = rows.Count;
                tableJson.pageNumber = pageNumber.Value;
                Database.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "getview(" + guid.ToString() + ")", "end", null, 3, null, DateTime.Now);
                return Json(tableJson);
            }
            catch (Exception ex)
            {
                Database.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "getview(" + guid.ToString() + ")", ex, 3, "end", DateTime.Now);
                return Json(new TableForJson { ErrorCode = 105, ErrorDescription = (ex.InnerException == null ? ex.Message : string.Format("Exeception Message {0};Inner Exception Message:{1}", ex.Message, ex.InnerException.Message)) });
            }
        }

        private  List<Dictionary<object, object>> CreateResponseDataTable(Durados.Web.Mvc.View view, DataView dataview,int rowCount, int pageNumber)
        {
            return dataview.GetTableForJson(view, rowCount, pageNumber);
           
        }

        protected virtual Dictionary<string, SortDirection> GetSortColumns(string sortColumn, string sortDirection)
        {

            Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
            sortColumns.Add(sortColumn,(SortDirection)Enum.Parse(typeof(SortDirection), sortDirection));
            return sortColumns;
        }


        protected virtual bool IsValidatUser()
        {

            string role = Map.Database.GetUserRole();
            if (role == "Developer" || role == "Admin")
            {
                return true;
            }
            else
                return false;
        }
        
        

        
    }
    public class TableForJson
    {

        public List<Dictionary<object, object>> results { get; set; }
        public int totalRowCount{get;set;}
        public int rowsInPage { get; set; }
        public int pageNumber { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
