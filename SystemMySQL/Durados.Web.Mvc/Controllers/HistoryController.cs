using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.IO;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.UI;
using Durados.Web.Mvc.UI.Json;

namespace Durados.Web.Mvc.Controllers
{
    public class HistoryController : DuradosController
    {
        protected override Durados.Web.Mvc.UI.TableViewer GetNewTableViewer()
        {
            return new UI.HistoryTableViewer();
        }

        //public virtual ActionResult HistoryFilter(string viewName, string jsonFilter, string guid, string search, bool mainPage)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(viewName))
        //        {
        //            return RedirectToAction("Index", new { viewName = Database.FirstView.Name, page = 1, guid = guid });
        //        }
        //        FormCollection collection = new FormCollection();
        //        collection.Add("jsonFilter", jsonFilter);

        //        Map.Session[guid + "Filter"] = collection;
        //        if (mainPage)
        //            Map.Session[viewName + "Filter"] = collection;
        //        Map.Session[guid + "Search"] = search;
        //        if (mainPage)
        //            Map.Session[viewName + "Search"] = search;

        //        Durados.Web.Mvc.View view = GetView(viewName, "Filter");
        //        ViewData["Styler"] = GetNewStyler();
        //        ViewData["TableViewer"] = GetNewTableViewer();

        //        string sortColumn = SortHelper.GetSortColumn(view);
        //        string sortDirection = SortHelper.GetSortDirection(view);

        //        if (string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(view.DefaultSort) || (view.DisplayType != DisplayType.Table && !string.IsNullOrEmpty(view.GroupingFields)))
        //        {
        //            string[] defaultSort = view.GetDefaultSortColumnsAndOrder();
        //            string defaultSortColumnAndOrder = defaultSort[0];
        //            sortColumn = view.GetDefaultSortColumn(defaultSortColumnAndOrder);
        //            sortDirection = view.GetDefaultSortColumnOrder(defaultSortColumnAndOrder);
        //        }

        //        ViewData["SortColumn"] = sortColumn;
        //        ViewData["direction"] = sortDirection;

        //        return RedirectToAction("Index", new { viewName = viewName, guid = guid, firstTime = true });
        //    }
        //    catch (Exception exception)
        //    {
        //        Durados.Web.Mvc.Logging.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
        //        return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", (object)exception.Message);
        //    }
        //    //DataView dataView = GetDataTable(viewName, 1, collection, search, sortColumn, sortDirection, guid);
        //    //dataView.Table.ExtendedProperties.Add("guid", guid);
        //    //return PartialView("~/Views/Shared/Controls/DataTableView.ascx", dataView);
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult HistoryFilter(string viewName, FormCollection collection, string guid)
        {
            try
            {
                if (string.IsNullOrEmpty(viewName))
                {
                    if (Map.Database.DefaultLast && Map.Database.FirstView != null)
                        return RedirectToAction("Index", new { viewName = Map.Database.FirstView.Name, page = 1, guid = guid });
                    else
                        return RedirectToAction("Default");
                }

                HandleFilter(collection);

                ViewHelper.SetSessionState(guid + "Filter", collection);
                ViewHelper.SetSessionState(viewName + "Filter", collection);
                
                Durados.Web.Mvc.View view = GetView(viewName, "Filter");
                //ViewData["Styler"] = GetNewStyler();
                //ViewData["TableViewer"] = GetNewTableViewer();

                string sortColumn = SortHelper.GetSortColumn(view);
                string sortDirection = SortHelper.GetSortDirection(view);

                if (string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(view.DefaultSort) || (view.DisplayType != DisplayType.Table && !string.IsNullOrEmpty(view.GroupingFields)))
                {
                    string[] defaultSort = view.GetDefaultSortColumnsAndOrder();
                    string defaultSortColumnAndOrder = defaultSort[0];
                    sortColumn = view.GetDefaultSortColumn(defaultSortColumnAndOrder);
                    sortDirection = view.GetDefaultSortColumnOrder(defaultSortColumnAndOrder);
                }

                ViewData["SortColumn"] = sortColumn;
                ViewData["direction"] = sortDirection;

                return RedirectToAction("Index", new { viewName = viewName, guid = guid, firstTime = true });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", (object)exception.Message);
            }
            //DataView dataView = GetDataTable(viewName, 1, collection, search, sortColumn, sortDirection, guid);
            //dataView.Table.ExtendedProperties.Add("guid", guid);
            //return PartialView("~/Views/Shared/Controls/DataTableView.ascx", dataView);
        }


        private void HandleFilter(FormCollection collection)
        {
            string jsonFilter = collection["jsonFilter"];
            Durados.Web.Mvc.UI.Json.Filter filter = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<Durados.Web.Mvc.UI.Json.Filter>(jsonFilter);
            if (filter.Fields.ContainsKey("ViewName"))
            {
                string viewName = filter.Fields["ViewName"].Value.ToString();

                if (Map.Database.Views.ContainsKey(viewName))
                {
                    View view = (View)Map.Database.Views[viewName];
                    if (!IsAllow(view) || !ViewOwnerAllow(view))
                        throw new DuradosAccessViolationException(Map.Database.Localizer.Translate("You are trying to view history of an unpermitted Table/View"));
                    if (!string.IsNullOrEmpty(view.BaseName))
                    {
                        if (view.Base != null)
                        {
                            viewName = view.Base.Name;
                            filter.Fields["ViewName"].Value = viewName;
                            jsonFilter = Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize<Durados.Web.Mvc.UI.Json.Filter>(filter);
                            collection["jsonFilter"] = jsonFilter;
                        }
                    }
                }
            }
        }
    }
}
