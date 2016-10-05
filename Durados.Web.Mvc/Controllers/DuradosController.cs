using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Durados.DataAccess;
using Durados.Diagnostics;
using Durados.Web.Mvc.Controllers.Filters;
using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc.UI.Helpers;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Durados.Web.Mvc.Controllers
{
  
    [NoCache]
    [Durados.Web.Mvc.Controllers.Attributes.DynamicAuthorizeAttribute()]
    public class DuradosController : BaseController
    {
        //public Map Map
        //{
        //    get
        //    {
        //        return Maps.Instance.GetMap();
        //    }
        //}

        public Durados.Database Database { get; private set; }
        protected Durados.View DuradosView { get; private set; }
        protected bool removeDuradosEvents = false;
        protected Workflow.Engine wfe = null;
        protected string currentGuid = null;

        protected virtual Workflow.Engine CreateWorkflowEngine()
        {
            return new Workflow.Engine();
        }

        //protected virtual Workflow.Engine GetWorkflowEngine()
        //{
        //    if (Map.WorkflowEngine == null)
        //    {
        //        Map.WorkflowEngine = CreateWorkflowEngine();
        //    }

        //    return Map.WorkflowEngine;
        //}

        public DuradosController()
            : base()
        {
            Init();
        }

        protected virtual void Init()
        {
            Database = GetDatabase();
            ViewData["Database"] = Database;
            wfe = CreateWorkflowEngine();
        }

        protected virtual Durados.Database GetDatabase()
        {
            return Map.Database;
        }

        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    // Output a nice error page
        //    if (filterContext.HttpContext.IsCustomErrorEnabled)
        //    {
        //        filterContext.ExceptionHandled = true;
        //        this.View("Error", filterContext.Exception).ExecuteResult(this.ControllerContext);
        //    }

        //}



        public JsonResult StartDiagnose(string pks)
        {
            string viewNames = string.Empty;
            if (string.IsNullOrEmpty(pks))
            {
                viewNames = Map.Database.Views.Values.Select(v => v.Name).ToArray().Delimited();
            }
            else
            {
                string[] pkArray = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<string[]>(pks);
                if (pkArray.Length == 0)
                {
                    viewNames = Map.Database.Views.Values.Select(v => v.Name).ToArray().Delimited();
                }
                else
                {
                    ConfigAccess configAccess = new ConfigAccess();
                    foreach (string pk in pkArray)
                    {
                        Dictionary<string, string> dinasty = new Dictionary<string, string>();

                        string name = configAccess.GetViewNameByPK(pk, Map.ConfigFileName);

                        FillViewChildrenDinasty(name, dinasty);

                        viewNames = dinasty.Values.ToArray().Delimited();
                    }
                }
            }

            Report report = new Report() { DateTime = DateTime.Now };
            Map.Database.DiagnosticsReport = report;

            return Json(viewNames);

        }

        public JsonResult GetViewNameByPK(string pk)
        {
            ConfigAccess configAccess = new ConfigAccess();
            string name = configAccess.GetViewNameByPK(pk, Map.ConfigFileName);

            return Json(name);
        }

        private void FillViewChildrenDinasty(string viewName, Dictionary<string, string> dinasty)
        {
            View view = GetView(viewName);
            if (dinasty.ContainsKey(viewName))
                return;

            dinasty.Add(viewName, viewName);

            foreach (ChildrenField childrenField in view.Fields.Values.Where(f => f.FieldType == FieldType.Children))
            {
                string childrenViewName = childrenField.ChildrenView.Name;
                FillViewChildrenDinasty(childrenViewName, dinasty);
            }

        }

        public ActionResult Diagnose(string viewName)
        {
            View view = GetView(viewName);

            return RedirectToAction(view.IndexAction, view.Controller, new { viewName = viewName, isMainPage = true, ajax = false });
        }

        public JsonResult EndDiagnose(string viewNames)
        {

            Map.Database.DiagnosticsReport = null;

            return Json("End");

        }

        public virtual ActionResult MenuDisplayState(string viewName, string guid, bool display)
        {
            Map.Session["MenuDisplayState"] = display;
            View view = null;
            if (Map.Database.Views.ContainsKey(viewName))
                view = (View)Map.Database.Views[viewName];
            else if (Map.GetConfigDatabase().Views.ContainsKey(viewName))
                view = (View)Map.GetConfigDatabase().Views[viewName];
            else if (Map.GetLocalizationDatabase() != null && Map.GetLocalizationDatabase().Views.ContainsKey(viewName))
                view = (View)Map.GetLocalizationDatabase().Views[viewName];

            if (view == null)
                view = Map.Database.FirstView;

            if (view == null)
                return RedirectToAction("Default");
            else
                return RedirectToAction(view.IndexAction, view.Controller, new { viewName = viewName, guid = guid, isMainPage = true });
        }

        public virtual ActionResult RdlcReport(string reportName, string reportDisplayName)
        {
            //return Redirect("../Reports/" + reportServicePath);
            return View(new UI.RdlcReport() { ReportName = reportName, ReportDisplayName = reportDisplayName });
        }


        public virtual ActionResult Report(string viewName)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "Report");
            string guid = GetUniqueName(view);
            ViewData["jsonView"] = GetJsonViewSerialized(view, DataAction.Create, view.GetJsonViewNotSerialized(DataAction.Create, guid));

            Durados.Web.Mvc.UI.Item item = new Durados.Web.Mvc.UI.Item() { Guid = guid, ViewName = viewName };
            ViewData["TableViewer"] = GetNewTableViewer();
            ViewData["ViewName"] = viewName;

            return View(item);
        }

        public virtual string GetFieldValue(Field field, string pk)
        {
            return field.GetValue(pk);
        }

        public virtual JsonResult GetScalar(string viewName, string pk, string fieldName)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "GetScalar");
            if (view == null)
                return Json(string.Empty);

            Field field = null;

            if (view.Fields.ContainsKey(fieldName))
            {
                field = view.Fields[fieldName];
            }
            else
            {
                field = view.GetFieldByColumnNames(fieldName);
            }

            if (field == null)
                return Json(string.Empty);

            DataRow row = view.GetDataRow(pk);

            if (row == null)
                return Json(string.Empty);

            string scalar = field.GetValue(row);

            return Json(scalar);

        }

        //protected virtual void HandleFilterVisibility(View view, bool? safety)
        //{
        //    //string viewFilterVisibility = view.Name + "_filterVisibility";
        //    //if (Map.Session[viewFilterVisibility] == null && !safety.HasValue)
        //    //{
        //    //    Map.Session[viewSafety] = view.GridEditableEnabled;
        //    //}
        //    //else
        //    //{
        //    //    if (safety.HasValue)
        //    //    {
        //    //        Map.Session[viewSafety] = safety.Value;
        //    //    }
        //    //}

        //}

        /// <summary>
        /// Save in session filter visibilty
        /// </summary>
        /// <param name="filterVisibilty"></param>
        /// <returns></returns>
        public virtual JsonResult ChangeFilterVisibilty(string viewName, bool filterVisibilty)
        {
            Durados.Web.Mvc.View view = GetView(viewName);

            string viewFilterVisibility = view.Name + "_filterVisibility";

            Map.Session[viewFilterVisibility] = filterVisibilty;

            return Json(null);
        }


        protected virtual void HandleSafety(View view, bool? safety)
        {
            if (view == null)
                throw new DuradosException("View does not exist");

            string viewSafety = view.Name + "_safety";
            if (Map.Session[viewSafety] == null && !safety.HasValue)
            {
                Map.Session[viewSafety] = view.GridEditableEnabled;
            }
            else
            {
                if (safety.HasValue)
                {
                    Map.Session[viewSafety] = safety.Value;
                }
            }

        }

        public ActionResult GetMainMenu()
        {
            return View("~/Views/Shared/Controls/WorkspaceMenu.ascx");
        }

        public void KeepAlive()
        {
        }

        /// <summary>
        /// Check if the system response, the string returned is checked by the CRON process
        /// </summary>
        /// <returns></returns>
        public string CheckResponse()
        {
            return Durados.Database.ShortProductName + " responses fine and is alive";
        }

        public virtual ActionResult FirstTime(string viewName)
        {


            HttpCookie cookie = Request.Cookies["d_lv"];
            if (cookie != null)
            {
                viewName = cookie.Value;
            }
            View view = null;
            if (viewName != null && Map.Database.Views.ContainsKey(viewName))
            {
                view = (View)Map.Database.Views[viewName];
                if (!view.IsAllow())
                {
                    view = Map.Database.FirstView;
                }
            }
            else
            {
                view = Map.Database.FirstView;
            }
            if (view == null)
                return RedirectToAction("Default");

            string username = GetUsername();
            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), null, null, 3, "username=" + username.Replace("'", "\""));

            if (Map.Database.DefaultLast)
                return RedirectToAction(view.IndexAction, view.Controller, new { viewName = view.Name });
            else
                return RedirectToAction("Default");
        }

        public ActionResult EditDefaultContent()
        {
            if (string.IsNullOrEmpty(CmsHelper.GetHtml(Durados.Web.Mvc.Database.DefaultPageContentKey)))
            {
                CmsHelper.SetHtml(Durados.Web.Mvc.Database.DefaultPageContentKey, "<h1>This is the first page</h1><br><h2>Place your content here</h2>");
            }


            return RedirectToAction("Index", new { viewName = "durados_Html", pk = Durados.Web.Mvc.Database.DefaultPageContentKey });
        }

        public ActionResult Page(int pageId)
        {
            Durados.Page page = Map.Database.Pages[pageId];

            if (!IsAllow(page))
            {
                string username = (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null) ? System.Web.HttpContext.Current.User.Identity.Name : string.Empty;
                string message = "The view: " + page.Title + " is not allowed for user: " + username;
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "", null, 4, message);

                return RedirectToAction("LogOn", "Account");
            }

            ViewData["ViewName"] = pageId.ToString();

            if (Request.IsAjaxRequest())
                return PartialView("~/Views/Shared/Controls/Page.ascx", page);

            return View(page);
        }

        public ActionResult Default(int? workspaceId)
        {
            if (((Database)Map.Database).GetUserRow() == null)
            {
                string username = (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null) ? System.Web.HttpContext.Current.User.Identity.Name : string.Empty;
                if (!Maps.IsSuperDeveloper(username))
                {
                    string message = "user does not belong to database";
                    try
                    {
                        message = username == string.Empty ? "System.Web.HttpContext.Current.User.Identity.Name is empty" : "The user: '" + username + "' does not exists in " + (new System.Data.SqlClient.SqlConnectionStringBuilder(Map.Database.ConnectionString)).InitialCatalog;
                    }
                    catch { }
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "", null, 4, message);

                    return RedirectToAction("LogOn", "Account", new { returnUrl = Request.Url.ToString() });
                }
            }

            Workspace workspace = Map.Database.GetDefaultWorkspace();
            if (workspaceId.HasValue && Map.Database.Workspaces.ContainsKey(workspaceId.Value))
            {
                if (!(workspaceId.Equals(Database.GetAdminWorkspaceId()) && Map.Database.GetUserRole() == "View Owner"))
                {
                    workspace = Map.Database.Workspaces[workspaceId.Value];

                }
            }
            else if (!string.IsNullOrEmpty(Map.Database.FirstViewName))
            {
                if (Map.Database.Views.ContainsKey(Map.Database.FirstViewName))
                {
                    View firstView = (View)Map.Database.Views[Map.Database.FirstViewName];
                    workspace = firstView.Workspace;
                    return RedirectToAction("Index", new { viewName = firstView.Name, ajax = false });
                }
            }

            if (workspace == null)
            {
                workspaceId = Map.Database.GetPublicWorkspaceId();
                if (Map.Database.Workspaces.ContainsKey(workspaceId.Value))
                {
                    Map.Session["workspaceId"] = workspaceId.Value;
                    workspace = Map.Database.Workspaces[workspaceId.Value];
                }
                else
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "", null, 2, "Public workspace does not exist");

                    Map.ConfigWorkspace();
                    Map.Commit();
                    workspaceId = Map.Database.GetPublicWorkspaceId();
                    if (Map.Database.Workspaces.ContainsKey(workspaceId.Value))
                    {
                        Map.Session["workspaceId"] = workspaceId.Value;
                        workspace = Map.Database.Workspaces[workspaceId.Value];
                    }
                    else
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "", null, 1, "Public workspace does not exist");
                    }
                }
            }
            else
                Map.Session["workspaceId"] = workspace.ID;

            if (Maps.MultiTenancy)
            {
                string contentKey = Durados.Web.Mvc.Database.DefaultPageContentKey;

                if (workspace == null || string.IsNullOrEmpty(workspace.Description))
                    ViewData[Durados.Web.Mvc.Database.DefaultPageContentKey] = CmsHelper.GetHtml(contentKey);
                else
                    ViewData[Durados.Web.Mvc.Database.DefaultPageContentKey] = workspace.Description;

                //string defaultPath = Map.DefaultPagePath + "Default.aspx";
                //if (System.IO.File.Exists(Server.MapPath(defaultPath)))
                //    return View(defaultPath);
                //else
                SpecialMenu defaultMenu = workspace.GetHomePageSpecialMenu(Map.Database.IsAllowMenu);
                if (defaultMenu == null)
                {
                    if (Request.IsAjaxRequest())
                        return PartialView("~/Views/Shared/Controls/Default.ascx");
                    else
                        return View("~/Views/Shared/Default.aspx");
                }
                else
                {
                    string defaultMenuUrl = defaultMenu.Url;
                    char c = defaultMenuUrl.Contains('?') ? '&' : '?';
                    defaultMenuUrl = defaultMenuUrl + c + "menuId=" + defaultMenu.ID;

                    return Redirect(defaultMenuUrl);
                }
            }
            else
                return View("~/Views/Shared/Default.aspx");
        }

        public void CacheRefresh()
        {
            SqlAccess.Cache.Refresh();
        }


        protected virtual string GetDeleteConfirmationMessage()
        {
            return Map.Database.Localizer.Translate("Are you sure that you want to delete the selected rows?");
        }

        public virtual ActionResult IndexWithButtons(string url, string backUrl)
        {
            ViewData["url"] = url;
            ViewData["backUrl"] = backUrl;

            return View();
        }

        public virtual ActionResult ViewDeleted(string viewName)
        {
            ViewData["viewName"] = viewName;

            return View();
        }

        public virtual ActionResult Index(string viewName, int? page, string SortColumn, string direction, string pks, bool? ajax, string pk, string guid, bool? children, string newPk, bool? firstTime, bool? isMainPage, bool? safety, bool? disabled, string path, bool? needChangeDisplayType)
        {
            if (Database.Logger != null)
            {
                Database.Logger.Log(viewName, "Start", "Index", "Controller", "", 12, Map.Logger.NowWithMilliseconds(), DateTime.Now);
            }

            ViewData["DeleteConfirmationMessage"] = GetDeleteConfirmationMessage();

            string d_filter = Request.QueryString["d_filter"];

            int x = 1;
            if (x == 2)
                throw new Exception("test");

            if (string.IsNullOrEmpty(viewName))
                return RedirectToAction("FirstTime");

            Durados.Web.Mvc.View view = GetView(viewName, "Index");

            if (view == null)
            {
                string menu = "on";
                if (!string.IsNullOrEmpty(Request.QueryString["menu"]) && Request.QueryString["menu"].Equals("off"))
                {
                    menu = "off";
                    return RedirectToAction("ViewDeleted", new { viewName = viewName, menu = menu });
                }
                else
                {
                    return RedirectToAction("ViewDeleted", new { viewName = viewName });
                }
            }

            bool isAjax = false;

            if (view.Database.DiagnosticsReportInProgress && isMainPage.HasValue && isMainPage.Value && ajax.HasValue)
            {
                isAjax = ajax.Value;
            }
            else
            {
                isAjax = Request.IsAjaxRequest() || (ajax.HasValue && ajax.Value);
            }

            if (!isAjax || needChangeDisplayType == true)
            {
                HandleViewMode(view, guid);
            }

            if (view.DisplayType == DisplayType.Report)
            {
                if (!isAjax)
                {
                    HandleSafety(view, safety);
                    return RedirectToAction("Report", new { viewName = viewName });
                }

                //if (string.IsNullOrEmpty(SortColumn) && (view.DisplayType != DisplayType.Table && !string.IsNullOrEmpty(view.GroupingFields)))
                //{
                //    string[] defaultSort = view.GetDefaultSortColumnsAndOrder();
                //    string defaultSortColumnAndOrder = defaultSort[0];
                //    SortColumn = view.GetDefaultSortColumn(defaultSortColumnAndOrder);
                //    direction = view.GetDefaultSortColumnOrder(defaultSortColumnAndOrder);
                //}
            }


            bool mainPage = !((children.HasValue && children.Value) || Request.QueryString.Count > 0) || (Request.QueryString.Count == 1 && Request.QueryString.AllKeys[0] == "path") || (isMainPage.HasValue && isMainPage.Value == true);

            string public2 = Request.QueryString["public"] ?? Request.QueryString["public2"];

            if (string.IsNullOrEmpty(guid))
            {
                if (children.HasValue && children.Value)
                {
                    firstTime = true;
                }
                guid = GetUniqueName(view);
            }
            else
            {
                if (!firstTime.HasValue)
                    firstTime = false;
                else
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["df"]) && !string.IsNullOrEmpty(Request.QueryString["dfval"]))
                    {
                        return RedirectToAction("IndexControl", new { viewName = viewName, page = page, SortColumn = SortColumn, direction = direction, pks = pks, guid = guid, newPk = newPk, firstTime = firstTime.Value, mainPage = false, safety = safety, disabled = disabled, d_filter = d_filter, df = Request.QueryString["df"], dfval = Request.QueryString["dfval"], checkbox = Request.QueryString["checkbox"], public2 = public2 });
                    }

                    return RedirectToAction("IndexControl", new { viewName = viewName, page = page, SortColumn = SortColumn, direction = direction, pks = pks, guid = guid, newPk = newPk, firstTime = firstTime.Value, mainPage = false, safety = safety, disabled = disabled, d_filter = d_filter, checkbox = Request.QueryString["checkbox"], public2 = public2 });
                    //mainPage = true;
                    //ViewHelper.SetPageFilterState(guid, Request.QueryString);
                    //return RedirectToAction("IndexPage", new { viewName = viewName, page = page, SortColumn = SortColumn, direction = direction, pk = pk, guid = guid, mainPage = mainPage });
                }
            }
            ViewData["guid"] = guid;
            currentGuid = guid;

            if (isAjax)
            {
                string subGrid2 = "no";

                if (children.HasValue && children.Value)
                {
                    ViewHelper.SetSessionState(guid + "PageFilterState", Request.QueryString);
                    SetDefaultFilter(view, new FormCollection(), guid, true);

                    subGrid2 = "yes";
                }
                if (Database.Logger != null)
                {
                    Database.Logger.Log(viewName, "End", "Index", "Controller", "", 12, Map.Logger.NowWithMilliseconds(), DateTime.Now);
                }

                return RedirectToAction("IndexControl", new { viewName = viewName, page = page, SortColumn = SortColumn, direction = direction, pks = pks, guid = guid, newPk = newPk, firstTime = firstTime.HasValue ? firstTime.Value : false, mainPage = mainPage, safety = safety, disabled = disabled, d_filter = d_filter, subGrid = Request.QueryString["subGrid"] == "true", public2 = public2, subGrid2 = subGrid2 });
            }
            else
            {
                ViewHelper.SetSessionState(guid + "PageFilterState", Request.QueryString);
                if (Database.Logger != null)
                {
                    Database.Logger.Log(viewName, "End", "Index", "Controller", "", 12, Map.Logger.NowWithMilliseconds(), DateTime.Now);
                }
                string menu = "on";
                if (!string.IsNullOrEmpty(Request.QueryString["menu"]) && Request.QueryString["menu"].Equals("off"))
                {
                    menu = "off";
                    return RedirectToAction("IndexPage", new { viewName = viewName, page = page, SortColumn = SortColumn, direction = direction, pk = pk, guid = guid, mainPage = mainPage, safety = safety, path = path, d_filter = d_filter, menu = menu, checkbox = Request.QueryString["checkbox"], menuId = Request.QueryString["menuId"] });
                }
                else
                {
                    return RedirectToAction("IndexPage", new { viewName = viewName, page = page, SortColumn = SortColumn, direction = direction, pk = pk, guid = guid, mainPage = mainPage, safety = safety, path = path, d_filter = d_filter, checkbox = Request.QueryString["checkbox"], menuId = Request.QueryString["menuId"] });
                }
            }
        }

        protected void HandleViewMode(View view, string guid)
        {
            if (guid == null || view == null) return;

            string viewAsDisplayType = view.Name + "_" + guid + "_dataDisplayType";

            //Init dataDisplayType session by: "view.DataDisplayType" or by: "QueryString["dataDisplayType"]"
            if (string.IsNullOrEmpty(Request.QueryString["dataDisplayType"]))
            {
                if (Map.Session[viewAsDisplayType] == null)
                {
                    Map.Session[viewAsDisplayType] = (int)view.DataDisplayType;
                }
            }
            else
            {
                DataDisplayType newDisplayType = (DataDisplayType)Enum.Parse(typeof(DataDisplayType), Request.QueryString["dataDisplayType"]);

                if (Map.Session[viewAsDisplayType] == null || (DataDisplayType)Map.Session[viewAsDisplayType] != newDisplayType)
                {
                    Map.Session[viewAsDisplayType] = (int)newDisplayType;
                }
            }

            //Validate session value
            DataDisplayType sessionDisplayType = (DataDisplayType)Map.Session[viewAsDisplayType];

            bool isDisplayTypeValid = sessionDisplayType == DataDisplayType.Table && view.EnableTableDisplay
                || sessionDisplayType == DataDisplayType.Dashboard && view.EnableDashboardDisplay
                || sessionDisplayType == DataDisplayType.Preview && view.EnablePreviewDisplay;

            if (!isDisplayTypeValid)
            {
                Map.Session[viewAsDisplayType] = (int)view.DataDisplayType;
            }
        }

        private void HandleFilter(View view, string guid, FormCollection collection, System.Collections.Specialized.NameValueCollection queryString)
        {
            if (!string.IsNullOrEmpty(queryString["df"]) && !string.IsNullOrEmpty(queryString["dfval"]))
            {
                string jsonFilter = collection["jsonFilter"];
                if (jsonFilter == null)
                    jsonFilter = view.GetJsonFilter(guid);

                UI.Json.Filter filter = UI.Json.JsonSerializer.Deserialize<UI.Json.Filter>(jsonFilter);

                string fieldName = null;
                string fieldValue = null;
                if (filter.Fields.ContainsKey(queryString["df"]))
                {
                    fieldName = queryString["df"];
                    fieldValue = queryString["dfval"];
                    filter.Fields[fieldName].Value = fieldValue;
                }

                collection.Add("jsonFilter", UI.Json.JsonSerializer.Serialize(filter));
                ViewHelper.SetSessionState(guid + "Filter", collection);
                if (fieldName != null && view.Fields.ContainsKey(fieldName))
                {
                    Field field = view.Fields[fieldName];
                    if (field.FieldType == FieldType.Parent)
                    {
                        System.Collections.Specialized.NameValueCollection qs = new System.Collections.Specialized.NameValueCollection();
                        string columnNames = field.GetColumnsNames().Delimited();
                        qs.Add(columnNames, fieldValue);

                        ChildrenField childrenField = (ChildrenField)((ParentField)field).GetEquivalentChildrenField();
                        qs.Add("__" + childrenField.Name + "__", fieldValue);
                        ViewHelper.SetSessionState(guid + "PageFilterState", qs);
                    }
                }


                ViewHelper.SetSessionState(guid + "Filter", collection);
                if (fieldName != null && view.Fields.ContainsKey(fieldName))
                {
                    Field field = view.Fields[fieldName];
                    if (field.FieldType == FieldType.Parent)
                    {
                        System.Collections.Specialized.NameValueCollection qs = new System.Collections.Specialized.NameValueCollection();
                        string columnNames = field.GetColumnsNames().Delimited();
                        qs.Add(columnNames, fieldValue);

                        ChildrenField childrenField = (ChildrenField)((ParentField)field).GetEquivalentChildrenField();
                        qs.Add("__" + childrenField.Name + "__", fieldValue);
                        ViewHelper.SetSessionState(guid + "PageFilterState", qs);
                    }
                }


            }
            else if (!string.IsNullOrEmpty(queryString["d_filter"]))
            {
                string d_filter = queryString["d_filter"];
                string jsonFilter = collection["jsonFilter"];
                if (jsonFilter == null)
                    jsonFilter = view.GetJsonFilter(guid);

                UI.Json.Filter filter = UI.Json.JsonSerializer.Deserialize<UI.Json.Filter>(jsonFilter);

                Dictionary<string, string> filterValues = GetFilterValues(d_filter);
                foreach (string fieldName in filterValues.Keys)
                {
                    if (filter.Fields.ContainsKey(fieldName))
                    {
                        string value = filterValues[fieldName];
                        if (!value.StartsWith("$") && !string.IsNullOrEmpty(value))
                        {
                            filter.Fields[fieldName].Value = value;
                        }
                    }
                }
                collection.Add("jsonFilter", UI.Json.JsonSerializer.Serialize(filter));

            }
        }

        private Dictionary<string, string> GetFilterValues(string d_filter)
        {
            Dictionary<string, string> filterValues = new Dictionary<string, string>();

            string[] arr = d_filter.Split(',');

            foreach (string s in arr)
            {
                string[] arr2 = s.Split('|');

                filterValues.Add(arr2[0], arr2[1]);
            }


            return filterValues;
        }


        protected virtual bool IsAllow()
        {
            if (Maps.MultiTenancy)
            {
                return Map.Database.GetUserRow() != null;

            }
            return true;
        }


        protected virtual bool IsAllow(View view)
        {
            if (Maps.MultiTenancy)
            {
                if (view.IsAllow())
                {
                    return Maps.IsSuperDeveloper(null) || Map.Database.GetUserRow() != null;
                }
                else
                {
                    return false;
                }
            }
            return view.IsAllow();
        }

        protected virtual bool IsAllow(Durados.Services.ISecurable securable)
        {
            if (Maps.MultiTenancy)
            {
                if (securable.IsAllow())
                {
                    return Maps.IsSuperDeveloper(null) || Map.Database.GetUserRow() != null;
                }
                else
                {
                    return false;
                }
            }
            return securable.IsAllow();
        }

        public virtual ActionResult IndexPage(string viewName, int? page, string SortColumn, string direction, string pk, string guid, bool? mainPage, bool? safety, string path)
        {
            //try
            //{
            Maps.Instance.DuradosMap.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString());
            if (!mainPage.HasValue)
                mainPage = true;

            if (string.IsNullOrEmpty(viewName))
                return RedirectToAction("FirstTime");

            ViewData["path"] = path;

            ViewData["ViewName"] = viewName;

            ViewData["DeleteConfirmationMessage"] = GetDeleteConfirmationMessage();

            DataView dataView = null;

            Durados.Web.Mvc.View view = GetView(viewName, "IndexPage");

            if (view == null)
            {
                string menu = "on";
                if (!string.IsNullOrEmpty(Request.QueryString["menu"]) && Request.QueryString["menu"].Equals("off"))
                {
                    menu = "off";
                    return RedirectToAction("ViewDeleted", new { viewName = viewName, menu = menu });
                }
                else
                {
                    return RedirectToAction("ViewDeleted", new { viewName = viewName });
                }
            }

            HandleSafety(view, safety);

            if (string.IsNullOrEmpty(guid))
                guid = GetUniqueName(view);

            HandleViewMode(view, guid);

            currentGuid = guid;
            ViewData["guid"] = guid;
            //string guid = GetUniqueName(view);

            if (!IsAllow(view) || !ViewOwnerAllow(view))
            {
                string username = (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null) ? System.Web.HttpContext.Current.User.Identity.Name : string.Empty;
                string message = "The view: " + view.Name + " is not allowed for user: " + username;
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "", null, 4, message);
                return RedirectToAction("LogOn", "Account", new { returnUrl = Request.Url.ToString() });
            }
            if (!SecurityHelper.IsAllowForView(view, "durados_v_ChangeHistory", null, "Admin,Developer"))
            {
                string username = (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null) ? System.Web.HttpContext.Current.User.Identity.Name : string.Empty;
                string message = "The view: " + view.Name + " is not allowed for user: " + username;
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "", null, 4, message);
                return RedirectToAction("Default");
            }
            if (!Database.IsConfig && view.GetRules().Where(r => r.DataAction.Equals(TriggerDataAction.BeforeViewOpen)).Count() > 0)
            {
                using (System.Data.IDbCommand command = view.GetNewCommand())
                {
                    command.Connection.Open();
                    wfe.PerformActions(this, view, TriggerDataAction.BeforeViewOpen, null, null, null, Map.Database.ConnectionString, Convert.ToInt32(Map.Database.GetUserID()), Map.Database.GetUserRole(), command, null);
                    command.Connection.Close();
                }
            }


            ViewData["jsonView"] = GetJsonViewSerialized(view, DataAction.Create, view.GetJsonViewNotSerialized(DataAction.Create, guid));


            FormCollection collection = null;
            string search = null;

            if (System.Web.HttpContext.Current.Request.QueryString["from"] == "bookmark")
            {
                collection = new FormCollection(ViewHelper.GetSessionState(viewName + "Filter"));
                search = ViewHelper.GetSessionString(viewName + "Search");
                ViewData["SortColumn"] = SortHelper.GetSortColumn(view);
                ViewData["direction"] = SortHelper.GetSortDirection(view);

            }
            else
            {

                if (mainPage.Value)
                {
                    collection = new FormCollection(ViewHelper.GetSessionState(viewName + "Filter"));
                    search = ViewHelper.GetSessionString(viewName + "Search");
                }
                else
                {
                    collection = new FormCollection(ViewHelper.GetSessionState(guid + "Filter"));
                    search = ViewHelper.GetSessionString(guid + "Search");
                }

                PagerHelper.SetCurrentPage(view, 1, guid);

                if (!string.IsNullOrEmpty(view.DefaultSort))
                {
                    //string[] defaultSort = view.GetDefaultSortColumnsAndOrder();
                    //string defaultSortColumnAndOrder = defaultSort[0];
                    //SortColumn = view.GetDefaultSortColumn(defaultSortColumnAndOrder);
                    //direction = view.GetDefaultSortColumnOrder(defaultSortColumnAndOrder);
                    //SortHelper.SetSortColumn(view, SortColumn);
                    //SortHelper.SetSortDirection(view, direction);

                    //ViewData["SortColumn"] = SortColumn;
                    //ViewData["direction"] = direction;

                    string[] defaultSort = view.GetDefaultSortColumnsAndOrder();
                    string defaultSortColumnAndOrder = defaultSort[0];
                    ViewData["SortColumn"] = view.GetDefaultSortColumn(defaultSortColumnAndOrder);
                    ViewData["direction"] = view.GetDefaultSortColumnOrder(defaultSortColumnAndOrder);

                }

                HandleFilter(view, guid, collection, Request.QueryString);

                if (!string.IsNullOrEmpty(pk))
                {
                    Durados.Web.Mvc.UI.ViewViewer viewViewer = new Durados.Web.Mvc.UI.ViewViewer();
                    Durados.Web.Mvc.UI.Json.Filter filter = viewViewer.GetJsonFilter(view, guid);
                    string[] pkValues = pk.Split(',');
                    for (int i = 0; i < pkValues.Length; i++)
                    {
                        string fieldName = view.DataTable.PrimaryKey[i].ColumnName;
                        filter.Fields[fieldName].Value = pkValues[i];
                    }
                    string jsonFilter = Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(filter);

                    if (collection == null)
                    {
                        collection = new FormCollection();
                    }
                    else
                    {
                        if (collection["jsonFilter"] != null)
                        {
                            collection.Remove("jsonFilter");
                        }
                    }
                    collection.Add("jsonFilter", jsonFilter);
                    ViewHelper.SetSessionState(guid + "Filter", collection);
                    if (mainPage.Value)
                        ViewHelper.SetSessionState(viewName + "Filter", collection);
                    ViewHelper.SetSessionState(guid + "Filter", collection);
                }
                else
                {
                    //if (collection != null && collection["jsonFilter"] != null)
                    //{
                    //    //collection.Remove("jsonFilter");
                    //}

                    //for (int i = 0; i < pkValues.Length; i++)

                    SetDefaultFilter(view, collection, guid, mainPage.Value);
                }

                if (mainPage.Value && System.Web.HttpContext.Current.Request.QueryString["refreshBookmarks"] == null)
                {
                    Uri refer = System.Web.HttpContext.Current.Request.UrlReferrer;

                    string referer;

                    if (refer == null)
                    {
                        referer = string.Empty;
                    }
                    else
                    {
                        referer = refer.ToString();
                    }

                    try
                    {
                        if (!referer.ToLower().Contains("plugin"))
                            AddBookmark(1, viewName, guid, referer, view.DisplayName, GetFilterText(collection, view)).ToString();
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                    }
                }

            }

            if (System.Web.HttpContext.Current.Request.QueryString["refreshBookmarks"] != null)
            {
                BookmarksHelper.ClearCache();
            }

            dataView = GetDataTable(view, 1, collection, search, SortColumn, direction, guid);

            ViewData["Styler"] = GetNewStyler(view, dataView);
            ViewData["TableViewer"] = GetNewTableViewer();
            //ViewData["ColumnsExcluder"] = GetNewColumnsExcluder();

            dataView.Table.ExtendedProperties.Add("viewName", viewName);
            dataView.Table.ExtendedProperties.Add("guid", guid);
            dataView.Table.ExtendedProperties.Add("mainPage", mainPage.HasValue);

            return View(IndexPageName(), dataView);
            //}
            //catch (Exception exception)
            //{
            //    Durados.Web.Mvc.Logging.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
            //    return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", (object)exception.Message);

            //}
        }

        protected bool ViewOwnerAllow(Mvc.View view)
        {
            if (Map.Database.GetUserRole() != "View Owner")
                return true;

            if (!view.Database.IsConfig)
                return true;

            if (view.Database.IsConfig && (view.Name == "Field" || view.Name == "View"))
                return true;

            return false;
        }

        protected virtual void SetDefaultFilter(View view, FormCollection collection, string guid, bool mainPage)
        {
            var defaultFilterFields = view.Fields.Values.Where(f => f.DefaultFilter != null && f.DefaultFilter != string.Empty);
            if (defaultFilterFields.Count() > 0 && (collection == null || collection.Count == 0))
            {
                Durados.Web.Mvc.UI.ViewViewer viewViewer = new Durados.Web.Mvc.UI.ViewViewer();
                Durados.Web.Mvc.UI.Json.Filter filter = viewViewer.GetJsonFilter(view, guid);

                foreach (Field field in defaultFilterFields)
                {
                    string fieldName = field.Name;
                    filter.Fields[fieldName].Value = field.GetDefaultFilter();

                    //if (field.FieldType == FieldType.Parent)
                    //{
                    //    filter.Fields[fieldName].Value = ((ParentField)field).GetDefaultFilter();
                    //}
                    //else
                    //{
                    //    filter.Fields[fieldName].Value = field.DefaultFilter;
                    //}
                }
                string jsonFilter = Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(filter);

                if (collection == null)
                {
                    collection = new FormCollection();
                }
                else
                {
                    if (collection["jsonFilter"] != null)
                    {
                        collection.Remove("jsonFilter");
                    }
                }
                collection.Add("jsonFilter", jsonFilter);
            }
            if (mainPage)
                ViewHelper.SetSessionState(view.Name + "Filter", collection);
            ViewHelper.SetSessionState(guid + "Filter", collection);
        }

        [ValidateInput(false)]
        public JsonResult SendConfig()
        {
            try
            {
                string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
                int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
                string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
                string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);
                string applicationName = System.Web.HttpContext.Current.Request.Url.Host;
                string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromError"]);
                string to = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["toError"]);

                string message = "Please find the attached configuration file.";
                string[] configFileName = new string[1] { Infrastructure.General.Zip(GetConfigFiles()) };

                Durados.Cms.DataAccess.Email.Send(host, Map.Database.UseSmtpDefaultCredentials, port, username, password, false, new string[1] { to }, null, null, applicationName + " configuration file", message, from, null, null, DontSend, configFileName, Map.Database.Logger);

                return Json("Successfully sent");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(exception.Message);
            }
        }

        protected string[] ChangeAppIdInConfigFiels(string[] fileNames, string oldConsoleId, string to)
        {
            string baseFolder = System.Configuration.ConfigurationManager.AppSettings["ChangeAppIdInConfigFielsFolder"] ?? "C:\\temp";

            List<string> newFileNames = new List<string>();
            foreach (string oldfilename in fileNames)
            {

                FileInfo info = new FileInfo(oldfilename);

                string newFileName = baseFolder + "\\" + info.Name.Replace(oldConsoleId, to);

                System.IO.File.Copy(oldfilename, newFileName, true);

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(newFileName);
                System.Xml.XmlNodeList uploadNodes = doc.SelectNodes("/NewDataSet/Upload/UploadVirtualPath/text()[contains(.,'" + oldConsoleId + "')]");

                foreach (System.Xml.XmlNode node in uploadNodes)
                {
                    node.InnerText = node.InnerText.Replace(oldConsoleId, to);
                }

                uploadNodes = doc.SelectNodes("/NewDataSet/Database/UploadFolder/text()[contains(.,'" + oldConsoleId + "')]");
                foreach (System.Xml.XmlNode node in uploadNodes)
                {
                    node.InnerText = node.InnerText.Replace(oldConsoleId, to);
                }

                doc.Save(newFileName);
                newFileNames.Add(newFileName);
            }
            return newFileNames.ToArray();
        }

        [ValidateInput(false)]
        public FileResult DownloadConfig(int? to)
        {
            try
            {
                if (Maps.Cloud)
                {
                    DataSet ds = new DataSet();
                    Map.ReadConfigFromCloud(ds, Map.ConfigFileName);
                    ds.WriteXml(Map.ConfigFileName, XmlWriteMode.WriteSchema);

                    ds = new DataSet();
                    Map.ReadConfigFromCloud(ds, Map.ConfigFileName + ".xml");
                    ds.WriteXml(Map.ConfigFileName + ".xml", XmlWriteMode.WriteSchema);
                }

                List<string> confogFiles = GetConfigFiles().ToList();
                if (to.HasValue)
                {
                    string[] newFileNames = ChangeAppIdInConfigFiels(GetConfigFiles(), Maps.Instance.GetCurrentAppId(), to.Value.ToString());
                    confogFiles.AddRange(newFileNames);
                }
                string filename = Infrastructure.General.Zip(confogFiles.ToArray());
                string[] fragents = filename.Split('.');

                string extension = string.Empty;

                if (fragents.Length > 0)
                    extension = fragents.Last();

                string virtualPath = Map.ConfigFileName.Replace("xml", extension);
                Response.AppendHeader("content-disposition", "attachment; filename=" + filename); // HttpUtility.UrlPathEncode(filename));

                Response.ContentEncoding = System.Text.Encoding.Unicode;

                if (Maps.Cloud)
                {
                    var bytes = System.IO.File.ReadAllBytes(virtualPath);
                    try
                    {
                        System.IO.File.Delete(virtualPath);
                        foreach (string f in confogFiles)
                        {
                            System.IO.File.Delete(f);
                        }
                    }
                    catch { }
                    return File(bytes, "application/" + extension);
                }

                return File(virtualPath, "application/" + extension);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                //throw new DuradosException("Failed to download configuration file", exception);
                return null;
            }
        }

        public JsonResult UploadConfig(string fileName)
        {
            try
            {
                View uploadConfigView = (View)Map.Database.Views["durados_UploadConfig"];
                ColumnField fileNameField = (ColumnField)uploadConfigView.Fields["FileName"];
                string path = fileNameField.GetUploadPath().TrimEnd("\\".ToArray());
                string[] configFiles = Infrastructure.General.UnZip(path + "\\" + fileName, path, true);

                if (configFiles.Length != 2)
                    return Json(new { success = false, message = "The zip file must contain 2 xml files." });

                string newSchemaFileName = null;
                string newConfigFileName = null;

                if (configFiles[0].ToLower().EndsWith(".xml.xml"))
                {
                    newConfigFileName = path + "\\" + configFiles[1];
                    newSchemaFileName = path + "\\" + configFiles[0];
                    if (configFiles[1].ToLower().EndsWith(".xml.xml"))
                        return Json(new { success = false, message = "The zip file must contain only one file that ends with .xml.xml." });
                }
                else if (configFiles[1].ToLower().EndsWith(".xml.xml"))
                {
                    newConfigFileName = path + "\\" + configFiles[0];
                    newSchemaFileName = path + "\\" + configFiles[1];
                }
                else
                    return Json(new { success = false, message = "The zip file must contain a files that ends with .xml.xml." });


                string id = Maps.Instance.GetCurrentAppId();
                string oldConfigFileName = Maps.GetConfigPath(string.Format("durados_AppSys_{0}.xml", id));
                string oldSchemaFileName = oldConfigFileName + ".xml";

                System.IO.File.Copy(newConfigFileName, oldConfigFileName, true);
                System.IO.File.Copy(newSchemaFileName, oldSchemaFileName, true);

                if (Maps.Cloud)
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(newConfigFileName, XmlReadMode.ReadSchema);
                    Map.WriteConfigToCloud(ds, newConfigFileName);
                    try
                    {
                        System.IO.File.Delete(oldConfigFileName);
                        System.IO.File.Delete(newConfigFileName);
                    }
                    catch { }
                    ds = new DataSet();
                    ds.ReadXml(newSchemaFileName, XmlReadMode.ReadSchema);
                    Map.WriteConfigToCloud(ds, newSchemaFileName, false);
                    try
                    {
                        System.IO.File.Delete(oldSchemaFileName);
                        System.IO.File.Delete(newSchemaFileName);
                    }
                    catch { }

                }
                //Map.Initiate();


                return Json(new { success = true, message = "The system was updated" });
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = "Failed to update system. Please upload the backed-up configuration.<br><br>Details:<br>" + exception.Message });
            }

        }

        protected virtual string[] GetConfigFiles()
        {
            string uiConfig = Map.ConfigFileName;
            string dbConfig = uiConfig + ".xml";

            return new string[2] { uiConfig, dbConfig };
        }

        protected virtual UI.Styler GetNewStyler(View view, DataView dataView)
        {
            return new UI.Styler(view, dataView);
        }

        protected virtual UI.TableViewer GetNewTableViewer()
        {
            return new UI.TableViewer();
        }

        UI.TableViewer tableViewer = null;

        public ITableConverter GetTableViewer()
        {
            if (tableViewer == null)
                tableViewer = GetNewTableViewer();

            return tableViewer;
        }

        protected virtual History GetNewHistory()
        {
            if (MySqlAccess.IsMySqlConnectionString(Map.Database.SystemConnectionString))
                return new MySqlHistory();
            return new History();
        }


        protected virtual UI.ColumnsExcluder GetNewColumnsExcluder(View view, Dictionary<string, UI.Json.Field> filterFields)
        {
            return new UI.ColumnsExcluder(view, filterFields);
        }

        public string HtmlDecode(string text)
        {
            return Server.HtmlDecode(text);
        }

        protected virtual string IndexPageName()
        {
            return "Index";
        }

        public virtual ActionResult CheckList(string viewName, string guid, string fieldName, string pk, string prefix)
        {
            View view = GetView(viewName, "CheckList");
            ChildrenField field = (ChildrenField)view.Fields[fieldName];
            return PartialView("~/Views/Shared/Controls/CheckListView.ascx", new Durados.Web.Mvc.UI.CheckList() { ChildrenField = field, DataAction = Durados.Web.Mvc.UI.FieldViewer.GetDataAction(prefix), Guid = guid, SelectList = field.GetSelectList(pk, false) });
        }

        public virtual ActionResult Item(string viewName, string pk, string guid)
        {
            Maps.Instance.DuradosMap.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "username: " + System.Web.HttpContext.Current.User.Identity.Name + ", id: " + this.Request.QueryString["id"], null, 77, "url: " + System.Web.HttpContext.Current.Request.Url.ToString());
            Durados.Web.Mvc.View view = GetView(viewName, "Item");
            ViewData["DeleteConfirmationMessage"] = GetDeleteConfirmationMessage();
            ViewData["jsonView"] = GetJsonViewSerialized(view, DataAction.Create, view.GetJsonViewNotSerialized(DataAction.Create, guid));
            ViewData["guid"] = guid;
            RegisterDropDownFilterEvents(view);

            return View("Item", new Durados.Web.Mvc.UI.Item() { Guid = guid, ViewName = viewName });
        }

        protected override Durados.Web.Mvc.View GetView(string viewName, string action)
        {
            return GetView(viewName);
        }

        protected override Durados.Web.Mvc.View GetView(string viewName)
        {
            return ViewHelper.GetView(viewName);
        }

        public virtual ActionResult IndexControl(string viewName, int? page, string SortColumn, string direction, string pks, string guid, string newPk, bool? firstTime, bool mainPage, bool? safety, bool? disabled)
        {
            try
            {
                if (Database.Logger != null)
                {
                    Database.Logger.Log(viewName, "Start", "IndexControl", "Controller", "", 12, Map.Logger.NowWithMilliseconds(), DateTime.Now);
                }
                ViewData["DeleteConfirmationMessage"] = GetDeleteConfirmationMessage();

                ViewData["ViewName"] = viewName;

                DataView dataView = null;

                Durados.Web.Mvc.View view = GetView(viewName, "IndexControl");

                HandleSafety(view, safety);

                if (disabled.HasValue)
                {
                    Map.Session["disabled" + guid] = disabled.Value;
                }
                //Map.Session[""] safety.HasValue ? safety.Value : view.GridEditableEnabled;

                ViewData["jsonView"] = GetJsonViewSerialized(view, DataAction.Create, view.GetJsonViewNotSerialized(DataAction.Create, guid));

                ViewData["pk"] = newPk;

                //string guid = GetUniqueName(view);
                if (string.IsNullOrEmpty(guid))
                    throw new Exception("Guid is missing in ajax call to IndexControl");

                //if (!view.IsAllow())
                //    throw new AccessViolationException();
                if (!IsAllow(view) || !ViewOwnerAllow(view))
                    return RedirectToAction("LogOn", "Account");

                FormCollection collection = null;
                string search = null;
                if (mainPage)
                {
                    collection = new FormCollection(ViewHelper.GetSessionState(viewName + "Filter"));
                    search = ViewHelper.GetSessionString(viewName + "Search");
                }
                else
                {
                    collection = new FormCollection(ViewHelper.GetSessionState(guid + "Filter"));
                    search = ViewHelper.GetSessionString(guid + "Search");
                }

                HandleFilter(view, guid, collection, Request.QueryString);

                if (page.HasValue)
                {
                    PagerHelper.SetCurrentPage(view, page.Value, guid);
                }
                else
                {
                    page = PagerHelper.GetCurrentPage(view, guid);
                }

                if (string.IsNullOrEmpty(SortColumn))
                {
                    SortColumn = SortHelper.GetSortColumn(view);
                    direction = SortHelper.GetSortDirection(view);
                    if (string.IsNullOrEmpty(SortColumn) || !view.Fields.ContainsKey(SortColumn))
                    {
                        if (!string.IsNullOrEmpty(view.DefaultSort))
                        {
                            string[] defaultSort = view.GetDefaultSortColumnsAndOrder();
                            string defaultSortColumnAndOrder = defaultSort[0];
                            //SortColumn = view.GetDefaultSortColumn(defaultSortColumnAndOrder);
                            //direction = view.GetDefaultSortColumnOrder(defaultSortColumnAndOrder);
                            ViewData["SortColumn"] = view.GetDefaultSortColumn(defaultSortColumnAndOrder);
                            ViewData["direction"] = view.GetDefaultSortColumnOrder(defaultSortColumnAndOrder);
                        }
                    }
                }
                else
                {
                    SortHelper.SetSortColumn(view, SortColumn);
                    SortHelper.SetSortDirection(view, direction);

                    ViewData["SortColumn"] = SortColumn;
                    ViewData["direction"] = direction;
                }
                //ViewData["SortColumn"] = SortColumn;
                //ViewData["direction"] = direction;

                if (!string.IsNullOrEmpty(pks))
                {
                    string[] pkArray = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<string[]>(pks);
                    ViewData["pkArray"] = pkArray;
                }

                currentGuid = guid;

                dataView = GetDataTable(view, page.Value, collection, search, SortColumn, direction, guid);

                ViewData["Styler"] = GetNewStyler(view, dataView);
                ViewData["TableViewer"] = GetNewTableViewer();
                //ViewData["ColumnsExcluder"] = GetNewColumnsExcluder();

                if (dataView.Table.ExtendedProperties.ContainsKey("guid"))
                    dataView.Table.ExtendedProperties["guid"] = guid;
                else
                    dataView.Table.ExtendedProperties.Add("guid", guid);
                if (dataView.Table.ExtendedProperties.ContainsKey("viewName"))
                    dataView.Table.ExtendedProperties["viewName"] = viewName;
                else
                    dataView.Table.ExtendedProperties.Add("viewName", viewName);
                if (dataView.Table.ExtendedProperties.ContainsKey("viewName"))
                    dataView.Table.ExtendedProperties["mainPage"] = mainPage;
                else
                    dataView.Table.ExtendedProperties.Add("mainPage", mainPage);
                if (Database.Logger != null)
                {
                    Database.Logger.Log(viewName, "End", "IndexControl", "Controller", "", 12, Map.Logger.NowWithMilliseconds(), DateTime.Now);
                }

                if (firstTime.HasValue && firstTime.Value)
                {
                    return PartialView(IndexControlFirstTime(), dataView);
                }
                else
                {
                    return PartialView(IndexControlNextTime(), dataView);
                }
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", (object)exception.Message);

            }
        }

        protected virtual string IndexControlFirstTime()
        {
            return "~/Views/Shared/Controls/GridView.ascx";
        }

        protected virtual string IndexControlNextTime()
        {
            return "~/Views/Shared/Controls/DataTableView.ascx";
        }

        protected virtual string GetUniqueName(View view)
        {
            return GetUniqueName(view.Name);
        }

        protected virtual string GetUniqueName(string viewName)
        {
            return viewName.ReplaceNonAlphaNumeric() + "_" + Durados.Web.Mvc.Infrastructure.ShortGuid.Next() + "_";
            //return view.Name;
        }


        public virtual FileResult ExportToExcel(string viewName, string guid, bool? noData)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "ExportToExcel");
            string SortColumn = SortHelper.GetSortColumn(view);
            string direction = SortHelper.GetSortDirection(view);
            int recordCount = (noData.HasValue && noData.Value) ? 0 : 100000000;
            FormCollection collection = new FormCollection(ViewHelper.GetSessionState(guid + "Filter"));
            //FormCollection collection = (FormCollection)ViewHelper.SetSessionState(viewName + "Filter");
            DataView dataView = GetDataTable(view, 1, recordCount, collection, ViewHelper.GetSessionString(guid + "Search"), SortColumn, direction, guid);

            string name = viewName + System.Guid.NewGuid().ToString() + ".xlsx";

            string filename = viewName + new Random(2).Next(98).ToString() + ".xlsx";

            string physicalPath = Server.MapPath(string.Format("/Uploads/{0}/{1}", Maps.Instance.GetCurrentAppId(), name));

            //string filePath = Server.MapPath("/Uploads/" + Maps.Instance.GetCurrentAppId() + "/" + view.Name + ".xlsx");

            DataAccess.Csv csv = new DataAccess.Csv();

            UI.TableViewer tableViewer = GetNewTableViewer();
            tableViewer.DataView = dataView;
            DataTable content = csv.ExportToDataTable(dataView, view, SecurityHelper.GetCurrentUserRoles(), tableViewer, guid);


            Durados.Xml.Sdk.Excel.ExcelHandler.CreateNewDocument(viewName, physicalPath, content);
            var bytes = System.IO.File.ReadAllBytes(physicalPath);
            System.IO.File.Delete(physicalPath);
            Response.Charset = "utf-8";
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename);
            return File(bytes, filename);

            //Response.Charset = "utf-8";

            //Response.ContentEncoding = System.Text.Encoding.Unicode;

            //Response.AppendHeader("content-disposition", "attachment; filename=" + filename);

            //return File(physicalPath, "application//vnd.ms-excel");
        }



        public virtual string Export(string viewName, string fileName)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "Export");
            int rowCount = 0;
            DataView dataView = view.FillPage(1, 10000000, null, null, null, out rowCount, null, null);

            if (rowCount == 0)
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "The view " + view.DisplayName + " has no rows", null, 3, null);

            return Export(view, dataView, fileName);
        }

        public virtual string Export(Durados.Web.Mvc.View view, DataView dataView, string fileName)
        {
            DataAccess.Csv csv = new DataAccess.Csv();
            string content = csv.Export(dataView, view, SecurityHelper.GetCurrentUserRoles(), GetNewTableViewer(), null);

            string path = HttpContext.Server.MapPath("~/" + view.Database.UploadFolder);

            fileName = fileName.Replace(Durados.Database.CurrentDatePlaceHolder, DateTime.Now.ToString("yyyyMMdd"));

            string csvExtetion = ".csv";
            if (!fileName.EndsWith(csvExtetion))
                fileName += csvExtetion;


            DirectoryInfo d = new DirectoryInfo(path);
            if (!d.Exists)
                d.Create();

            fileName = path + fileName;

            TextWriter w = new StreamWriter(new FileStream(fileName, FileMode.Create));
            try
            {
                w.Write(content);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
            }
            finally
            {
                w.Close();
            }

            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "The view " + view.DisplayName + " was successfully exported to the file \"" + fileName + "\".", null, 3, null);

            return fileName;
        }


        protected virtual DataView GetDataViewForExport(DataView dataView, View view, string guid)
        {
            return dataView;
        }

        public virtual ActionResult ExportToExcel2(string viewName, string guid)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "ExportToExcel2");
            string SortColumn = SortHelper.GetSortColumn(view);
            string direction = SortHelper.GetSortDirection(view);

            FormCollection collection = new FormCollection(ViewHelper.GetSessionState(guid + "Filter"));
            DataView dataView = GetDataTable(view, 1, 100000000, collection, ViewHelper.GetSessionString(guid + "Search"), SortColumn, direction, guid);
            dataView = GetDataViewForExport(dataView, view, guid); // GetTableViewer().GetDataView(dataView, view, guid);
            string content = WorkbookEngine.CreateWorkbook(dataView.Table.DataSet, Server.MapPath("/Content/Excel.xsl"));


            Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1255");
            Response.AppendHeader("content-disposition", "attachment; filename=" + view.GetFileDisplayName() + ".xlsx");

            //Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1255");
            //Response.BinaryWrite(System.Text.Encoding.GetEncoding("windows-1255").GetPreamble());

            return this.Content(content, "application/vnd.ms-excel");
        }


        public virtual ActionResult ExportToCsv(string viewName, string guid, bool? noData)
        {
            //if (Database.DefaultExportImportFormat == ExportFileType.Excel)
            //    return ExportToExcel2(viewName, guid);
            //check if this view already have predefine Excel. If yes loaded and create Data sheet with all the data. Current just return the file
            Durados.Web.Mvc.View view = GetView(viewName, "ExportToCsv");
            string filePath = Server.MapPath("/Uploads/" + Maps.Instance.GetCurrentAppId() + "/" + view.Name + ".xlsx");
            bool fileExists = System.IO.File.Exists(filePath);
            if (true)//!fileExists)
            {

                string SortColumn = SortHelper.GetSortColumn(view);
                string direction = SortHelper.GetSortDirection(view);

                FormCollection collection = new FormCollection(ViewHelper.GetSessionState(guid + "Filter"));
                //FormCollection collection = (FormCollection)ViewHelper.SetSessionState(viewName + "Filter");
                int recordCount = (noData.HasValue && noData.Value) ? 0 : 100000000;
                DataView dataView = GetDataTable(view, 1, recordCount, collection, ViewHelper.GetSessionString(guid + "Search"), SortColumn, direction, guid);
                if (dataView.Count == 0)
                    return ExportToExcel(viewName, guid, noData);
                dataView = GetDataViewForExport(dataView, view, guid); // GetTableViewer().GetDataView(dataView, view, guid);

                DataAccess.Csv csv = new DataAccess.Csv();
                UI.TableViewer tableViewer = GetNewTableViewer();
                tableViewer.DataView = dataView;

                string content = csv.Export(dataView, view, SecurityHelper.GetCurrentUserRoles(), tableViewer, guid);


                Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1255");

                Response.AppendHeader("content-disposition", "attachment; filename=" + view.GetFileDisplayName() + ".csv");

                //Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1255");
                //Response.BinaryWrite(System.Text.Encoding.GetEncoding("windows-1255").GetPreamble());

                return this.Content(content, "text/csv");
            }
            else
            {

                Durados.Xml.Sdk.Excel.ExcelHandler.CreateNewDocument(view.Name, filePath, GetDataTable(view), true);
                Response.AppendHeader("content-disposition", "attachment; filename=" + view.Name + ".xlsx");
                return this.Content(filePath, "application//vnd.ms-excel");
            }
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();

            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();

            return mimeType;
        }

        public virtual ActionResult Download(string viewName, string fieldName, string filename, string pk)
        {
            View view = null;

            if ((viewName == "durados_App" || viewName == null) && pk == null)
                pk = Map.Id;

            if ((viewName == "durados_App" || viewName == null) && fieldName == "Image" && Maps.Cloud)
            {
                string imageFieldName = "Image";
                string defaulLogo = string.Empty;
                try
                {
                    defaulLogo = Maps.Instance.DuradosMap.Database.Views["durados_App"].Fields[imageFieldName].DefaultValue.ToString();
                }
                catch { }
                if (filename.EndsWith(defaulLogo) || filename.ToLower().EndsWith("backand.png") || filename.ToLower().EndsWith("modubiz.png"))
                {
                    return Redirect("/Content/Images/" + defaulLogo);
                }
                string folder = Maps.AzureAppPrefix + pk;
                string url = string.Format(Maps.AzureStorageUrl + "/{2}", Maps.AzureStorageAccountName, folder, filename);
                return Redirect(url);
            }

            bool isMultiTenancy = false;
            if (Database.Views.ContainsKey(viewName))
            {
                view = (View)Database.Views[viewName];
            }
            else if (Maps.Instance.DuradosMap.Database.Views.ContainsKey(viewName))
            {
                isMultiTenancy = true;
                view = (View)Maps.Instance.DuradosMap.Database.Views[viewName];
            }
            else
                throw new DuradosAccessViolationException("View " + viewName + " was not Found.");
            if (!IsAllow(view) && view.Name != "Workspace" && view.Name != "durados_Import")
                throw new DuradosAccessViolationException("The view " + viewName + " is not allowed");

            fieldName = Server.UrlDecode(fieldName);
            filename = Server.UrlDecode(filename);

            string[] fragents = filename.Split('.');

            string extension = string.Empty;

            if (fragents.Length > 0)
                extension = fragents.Last();

            ColumnField field = (ColumnField)view.Fields[fieldName];

            if (filename == "__filename__")
            {
                return File(Infrastructure.General.GetRootPath() + "Content/Images/ViewUpload.gif", "gif");
            }

            if (field.FtpUpload != null)
                return FtpDownLoad(field, filename);

            HandleMultiTenancyAppFileName(ref filename, pk, isMultiTenancy);
            string virtualPath = field.Upload.UploadVirtualPath.Replace('\\', '/').TrimEnd('/') + '/' + filename;

            if (!virtualPath.StartsWith(Map.Database.UploadFolder) || virtualPath.Contains("..") || !Maps.IsAlloweDownload(virtualPath))
            {
                throw new DuradosAccessViolationException("Ilegal path " + virtualPath);
            }


            if (Map is DuradosMap)
            {
                HandleMultiTenancyDownLoadAuthorization(virtualPath);
            }
            string physicalPath = Server.MapPath(virtualPath);

            if (Maps.MultiTenancy && Map is DuradosMap)
            {
                FileInfo fileInfo = new FileInfo(physicalPath);

                if (virtualPath.Replace(fileInfo.Name, "").Trim('/').ToLower() != Map.Database.UploadFolder.Trim('/').ToLower())
                {
                    //throw new DuradosAccessViolationException();
                }
            }


            PrepareDownloadFile(physicalPath, viewName, pk);


            if (!System.IO.File.Exists(physicalPath))
            {
                bool debug = System.Configuration.ConfigurationManager.AppSettings["Debug"].ToLower() == "true";
                if (debug)
                {
                    physicalPath = Server.MapPath("~/Content/Images/logoDefault.png");
                }
                else
                {
                    throw new Durados.FileNotFoundException("The file is missing");
                }
            }
            Response.Charset = "utf-8";

            //System.Text.Encoding dec = System.Text.Encoding.GetEncoding("iso-8859-1");

            //System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");;

            //filename = dec.GetString(enc.GetBytes(filename));
            if (Map.GetLocalizationDatabase() != null)
                filename = Server.UrlEncode(filename).Replace("+", "%20");
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename); // HttpUtility.UrlPathEncode(filename));

            Response.ContentEncoding = System.Text.Encoding.Unicode;

            string mimeType = GetMimeType(filename);
            //Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            try
            {
                HandleIE7(physicalPath);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
            }
            //Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            return File(virtualPath, mimeType);

        }

        protected virtual void HandleMultiTenancyAppFileName(ref string filename, string pk, bool isMultiTenancy)
        {
            if (isMultiTenancy && !string.IsNullOrEmpty(pk) && !filename.StartsWith(pk + "/"))
                filename = string.Format("{0}/{1}", pk, filename);
        }
        protected virtual void HandleMultiTenancyDownLoadAuthorization(string virtualPath)
        {

        }

        private void HandleIE7(string physicalPath)
        {
            if (IsIe7())
            {
                if (IsIe7Handling())
                {
                    Response.AppendHeader("Pragma", "public");
                    Response.AppendHeader("Expires", "0");
                    Response.AppendHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                }
                else if (IsSpecialIe7Handling())
                {
                    string[] specialIe7Handling = GetSpecialIe7Handling();
                    if (specialIe7Handling != null)
                    {
                        foreach (string header in specialIe7Handling)
                        {
                            string[] segments = header.Split(',');
                            if (segments.Length == 2)
                            {
                                Response.AppendHeader(segments[0], segments[1]);
                            }
                        }
                    }

                }
                string[] forbiddenWords;
                if (IsImageFile(physicalPath) && IsCheckFileContent(out forbiddenWords))
                {
                    string fileStr = System.IO.File.ReadAllText(physicalPath);
                    foreach (string word in forbiddenWords)
                    {
                        if (fileStr.IndexOf(Server.UrlDecode(word), StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            throw new DuradosAccessViolationException(string.Format("This file - {0} - contains unallowed word: {1}", physicalPath, word));
                        }
                    }
                }
            }
        }

        protected virtual void HandleMainMapDownLoadAuthorization(string virtualPath)
        {
            string[] virtualPathDirs = VirtualPathUtility.GetDirectory(virtualPath.ToLower()).Trim('/').Split('/');

            if ((virtualPathDirs.Length > 1 && Session["userApps"] != null && !((Dictionary<string, string>)Session["userApps"]).ContainsKey(virtualPathDirs[1])))
            {
                throw new AccessViolationException();
            }
        }

        private bool IsImageFile(string physicalPath)
        {

            string exst = Path.GetExtension(physicalPath).ToLower();
            string fileTypes = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ImageFileTypes"] ?? "png,gif,jpeg,jpg");
            if (!string.IsNullOrEmpty(fileTypes) && fileTypes.Split(',').Length > 0)
            {
                fileTypes = fileTypes.ToLower();
                return fileTypes.Split(',').Contains(exst);

            }
            return false;
        }

        protected bool IsCheckFileContent(out string[] forbiddenWords)
        {
            forbiddenWords = new string[0];
            string words = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FileContentforbiddenWords"] ?? string.Empty);
            if (!string.IsNullOrEmpty(words) && words.Split(',').Length > 0)
            {
                forbiddenWords = words.Split(',');
                return true;
            }

            return false;
        }

        protected bool IsIe7Handling()
        {
            return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["isIe7Handling"] ?? "false");
        }

        protected bool IsSpecialIe7Handling()
        {
            return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["isSpecialIe7Handling"] ?? "false");
        }

        protected string[] GetSpecialIe7Handling()
        {
            return Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["specialIe7Handling"] ?? "").Split(';');
        }

        protected bool IsIe7()
        {
            return Request.Browser.Browser == "IE" && Request.Browser.MajorVersion == 7;
        }

        public virtual JsonResult IsDisabled(string viewName, string pk, string guid)
        {
            View view = GetView(viewName, "IsDisabled");

            if (view == null)
                return Json(true);

            if (string.IsNullOrEmpty(pk))
                return Json(false);

            DataRow row = view.GetDataRow(pk);

            int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
            string currentUserRole = Map.Database.GetUserRole();

            if (row == null)
                return Json(true);

            bool disabled = false;
            IEnumerable<Rule> rules = view.GetRules().Where(r => r.DataAction == TriggerDataAction.Open).ToList();

            Workflow.Engine wfe2 = CreateWorkflowEngine();

            string message = string.Empty;
            foreach (Durados.Rule rule in rules)
            {
                if (wfe2.Check(view, rule, TriggerDataAction.BeforeEdit, null, pk, row, rule.UseSqlParser, Map.Database.ConnectionString, currentUserId, currentUserRole))
                {
                    Durados.Workflow.Validator validator = new Durados.Workflow.Validator();
                    message = validator.GetMessage(this, rule.Parameters, view, null, null, new Durados.Workflow.LogicalParser(), out disabled, pk, currentUserId, currentUserRole);
                    if (disabled)
                        break;
                }
            }

            var json = new { disabled = disabled, message = message };

            return Json(json);

        }

        public virtual JsonResult DeleteFile(string viewName, string fieldName, string filename)
        {
            fieldName = Server.UrlDecode(fieldName);
            filename = Server.UrlDecode(filename);

            string[] fragents = filename.Split('.');

            string extension = string.Empty;

            if (fragents.Length > 0)
                extension = fragents.Last();

            ColumnField field = (ColumnField)Database.Views[viewName].Fields[fieldName];

            if (field.FtpUpload != null)
                return FtpDeleteFile(viewName, fieldName, filename);

            string virtualPath = field.Upload.UploadVirtualPath + filename;

            string physicalPath = Server.MapPath(virtualPath);

            if (System.IO.File.Exists(physicalPath) && !field.Upload.Override)
            {
                try
                {
                    System.IO.File.Delete(physicalPath);
                    return Json("Success");
                }
                catch
                {
                    return Json("Failure");
                }
            }

            return Json("Success");

        }

        protected virtual void PrepareDownloadFile(string physicalPath, string viewName, string pk)
        {

        }

        protected override Durados.Web.Mvc.Infrastructure.CustomError GetCustomError(ILog log, string viewName)
        {
            Durados.Web.Mvc.Infrastructure.CustomError customError = base.GetCustomError(log, viewName);

            if (customError.Log.ExceptionMessage == "The file is missing")
            {
                customError.Title = "The file is missing";
                customError.Message = "please go <a href='#' onclick='history.go(-1);'>back</a> and refresh the the page.<br> if the file is still missng please contact the system adminstrator.";
            }

            return customError;
        }

        protected virtual void SetSessionFilter(string viewName, string guid, string filter, bool? mainPage)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "SetSessionFilter");

            UI.Json.View jsonView = view.GetJsonViewNotSerialized(DataAction.Edit, guid);

            Dictionary<string, string> filterDictionary = GetFilter(filter);
            foreach (string key in filterDictionary.Keys)
            {
                jsonView.Fields[key].Value = filterDictionary[key];
            }

            FormCollection collection = new FormCollection();

            string jsonFilter = GetJsonViewSerialized(view, DataAction.Create, jsonView);

            collection.Add("jsonFilter", jsonFilter);

            ViewHelper.SetSessionState(guid + "Filter", collection);
            if (mainPage.HasValue && mainPage.Value)
                ViewHelper.SetSessionState(viewName + "Filter", collection);


        }

        protected Dictionary<string, string> GetFilter(string filter)
        {
            string[] a = filter.Split(';');
            Dictionary<string, string> d = new Dictionary<string, string>();

            string key = null;
            foreach (string s in a)
            {
                if (key == null)
                {
                    key = s;
                }
                else
                {
                    d.Add(key, s);
                    key = null;
                }
            }

            return d;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult GetAllFilterValues(string viewName, string fieldName, string guid)
        {
            Field field = null;
            View view = GetView(viewName);

            if (view != null && view.Fields.ContainsKey(fieldName))
            {
                field = view.Fields[fieldName];
            }

            ViewData["guid"] = guid;
            ViewData["loadAll"] = true;
            //ViewData["view"] = view;

            return PartialView("~/Views/Shared/Controls/Filter/TreeFilterValues.ascx", field);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Filter(string viewName, FormCollection collection, string guid, string search, bool? mainPage, bool? loadTreeFilter)
        {
            try
            {
                ViewData["ViewName"] = viewName;

                if (string.IsNullOrEmpty(viewName))
                {
                    if (Map.Database.DefaultLast && Map.Database.FirstView != null)
                        return RedirectToAction("Index", new { viewName = Map.Database.FirstView.Name, page = 1, guid = guid });
                    else
                        return RedirectToAction("Default");
                }

                ViewData["subGrid2"] = collection["subGrid2"];

                ViewHelper.SetSessionState(guid + "Filter", collection);
                if (mainPage.HasValue && mainPage.Value)
                    ViewHelper.SetSessionState(viewName + "Filter", collection);
                ViewHelper.SetSessionState(guid + "Search", search);
                if (mainPage.HasValue && mainPage.Value)
                    ViewHelper.SetSessionState(viewName + "Search", search);

                Durados.Web.Mvc.View view = GetView(viewName, "Filter");

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

                DataView dataView = GetDataTable(viewName, 1, collection, search, sortColumn, sortDirection, guid);
                PagerHelper.SetCurrentPage(view, 1, guid);

                ViewData["Styler"] = GetNewStyler(view, dataView);
                ViewData["TableViewer"] = GetNewTableViewer();
                if (dataView.Table.ExtendedProperties.ContainsKey("guid"))
                    dataView.Table.ExtendedProperties["guid"] = guid;
                else
                    dataView.Table.ExtendedProperties.Add("guid", guid);
                if (dataView.Table.ExtendedProperties.ContainsKey("viewName"))
                    dataView.Table.ExtendedProperties["viewName"] = viewName;
                else
                    dataView.Table.ExtendedProperties.Add("viewName", viewName);

                if (mainPage.HasValue && mainPage.Value)
                {
                    Uri refer = System.Web.HttpContext.Current.Request.UrlReferrer;

                    string referer;

                    if (refer == null)
                    {
                        referer = string.Empty;
                    }
                    else
                    {
                        referer = refer.ToString();
                    }

                    try
                    {
                        AddBookmark(1, viewName, guid, referer, view.DisplayName, GetFilterText(collection, view)).ToString();
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                    }
                }

                if (loadTreeFilter == true)
                {
                    return PartialView("~/Views/Shared/Controls/Filter/DataTableViewWithTreeFilter.ascx", dataView);
                }
                else
                {
                    return PartialView("~/Views/Shared/Controls/DataTableView.ascx", dataView);
                }
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", (object)exception.Message);
            }
        }

        private string GetFilterText(System.Collections.Specialized.NameValueCollection collection, Durados.Web.Mvc.View view)
        {
            return GetFilterText(collection, view, FilterTextType.DatabaseName);
        }

        private string GetFilterText(System.Collections.Specialized.NameValueCollection collection, Durados.Web.Mvc.View view, FilterTextType filterTextType)
        {
            return GetFilterTextDatabase(collection, view);

        }

        private string GetFilterTextDatabase(System.Collections.Specialized.NameValueCollection collection, Durados.Web.Mvc.View view)
        {

            UI.Json.Filter jsonFilter = null;

            if (collection != null && collection["jsonFilter"] != null)
            {
                jsonFilter = UI.Json.JsonSerializer.Deserialize<UI.Json.Filter>(collection["jsonFilter"]);
            }


            string text = string.Empty;

            if (jsonFilter != null)
            {
                foreach (UI.Json.Field jsonField in jsonFilter.Fields.Values)
                {
                    if (jsonField.Value != null && !string.IsNullOrEmpty(jsonField.Value.ToString()) && jsonField.Format != null)
                    {
                        if (text != string.Empty) text += " and ";
                        if (view.Fields[jsonField.Name].DataType == DataType.MultiSelect)
                        {
                            text += GetParameterForBookMarkFilterMulti(view, jsonField);
                        }
                        else
                        {
                            text += "[" + view.Fields[jsonField.Name].DatabaseNames + "]" + GetParameterForBookMarkFilter(view, jsonField);//"=" + jsonField.Format.ToString();
                        }
                    }
                }
            }

            return text;
        }

        private string GetFilterTextFieldName(System.Collections.Specialized.NameValueCollection collection, Durados.Web.Mvc.View view)
        {

            UI.Json.Filter jsonFilter = null;

            if (collection != null && collection["jsonFilter"] != null)
            {
                jsonFilter = UI.Json.JsonSerializer.Deserialize<UI.Json.Filter>(collection["jsonFilter"]);
            }


            string text = string.Empty;

            if (jsonFilter != null)
            {
                foreach (UI.Json.Field jsonField in jsonFilter.Fields.Values)
                {
                    if (jsonField.Value != null && !string.IsNullOrEmpty(jsonField.Value.ToString()) && jsonField.Format != null)
                    {
                        if (text != string.Empty) text += " | ";

                        text += view.Fields[jsonField.Name].DisplayName + "=" + jsonField.Format.ToString();

                    }
                }
            }

            return text;
        }

        private string GetParameterForBookMarkFilterMulti(Mvc.View view, UI.Json.Field jsonField)
        {
            //string where = string.Empty;
            string[] children = jsonField.Value.ToString().Split(',');

            ChildrenField field = (ChildrenField)view.Fields[jsonField.Name];
            View childrenView = (View)field.ChildrenView;

            //string exists = " EXISTS (select * from [" + childrenView.DataTable.TableName + "] with (nolock) where ";
            ParentField parentField = (ParentField)field.GetEquivalentParentField();
            //DataColumn[] fkColumns = parentField.DataRelation.ChildColumns;
            //DataColumn[] pkColumns = parentField.DataRelation.ParentColumns;
            //int fkColumnsLength = fkColumns.Length;
            //for (int i = 0; i < fkColumns.Length; i++)
            //{
            //    DataColumn fkColumn = fkColumns[i];
            //    string fkColumnName = fkColumn.ColumnName;
            //    DataColumn pkColumn = pkColumns[i];
            //    string pkColumnName = pkColumn.ColumnName;

            //    exists += "[" + view.DataTable.TableName + "].[" + pkColumnName + "]" + " = [" + fkColumnName + "] and ";

            //}
            string exists = string.Empty;

            if (children.Contains(Durados.Database.EmptyString))
            {
                exists = (new SqlAccess()).GetChildrenExistStatement(field, parentField);
                exists = exists.TrimEnd("and ".ToCharArray());
                exists = " not " + exists;
                exists += ")";
                //filter.WhereStatement += exists + " " + logicCondition.ToString() + " ";
                //filter.WhereStatementWithoutTablePrefix += exists + " " + logicCondition.ToString() + " ";

                if (children.Length > 1)
                {
                    exists += " or ";
                }

            }
            //else
            //{
            if (children.Length > 1 || children[0] != Durados.Database.EmptyString)
            {
                exists = (new SqlAccess()).GetChildrenExistStatement(field, parentField);

                ParentField other = (ParentField)childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent && f.Name != parentField.Name).First();

                exists += "(";


                int k = 0;

                DataColumn column = other.DataRelation.ChildColumns[0];
                string columnName = column.ColumnName;

                foreach (string child in children)
                {
                    if (child != Durados.Database.EmptyString && !string.IsNullOrEmpty(child))
                    {
                        string c = child;
                        if (!IsNumeric(column.DataType))
                        {
                            c = "'" + c + "'";
                        }
                        exists += "[" + columnName + "]" + " = " + c + " or ";

                        k++;
                    }
                }
                exists = exists.TrimEnd(" or ".ToCharArray());

                exists += "))";
            }

            return exists;
        }

        private string GetParameterForBookMarkFilter(Mvc.View view, UI.Json.Field jsonField)
        {
            string where = string.Empty;
            Field field = view.Fields[jsonField.Name];
            string data = jsonField.Value.ToString();
            DataType dataType = field.DataType;
            bool hasEquality = data.StartsWith("<") || data.StartsWith(">") || data.StartsWith("=");
            string equality = "=";
            if (hasEquality)
            {
                string[] dataArr = data.Split(' ');
                if (dataArr.Length > 1)
                {
                    equality = dataArr[0];
                    data = data.TrimStart(equality.ToCharArray()).Trim();
                }
            }
            else if (data.StartsWith("between"))
            {
            }

            switch (dataType)
            {
                case DataType.Boolean:
                    if (data == Database.True)
                        data = "1";
                    else data = "0";
                    break;

                case DataType.DateTime:
                    int c = 101;
                    DateTime now = DateTime.Now;
                    if (DateTime.TryParse("12/13/2013", out now))
                    {
                        c = 103;
                    }
                    else if (DateTime.TryParse("13/12/2013", out now))
                    {
                        c = 101;
                    }

                    data = "CONVERT( DATE,'" + data + "', " + c + " )";
                    break;

                case DataType.Email:
                case DataType.Html:
                case DataType.ShortText:
                case DataType.Image:
                case DataType.LongText:
                case DataType.Url:
                    data = "'%" + data + "%'";
                    equality = " like ";
                    break;

                case DataType.ImageList:
                case DataType.SingleSelect:
                    if (field.FieldType == FieldType.Parent)
                    {
                        equality = " in ";
                        data = "(" + data + ")";
                        if (!IsNumeric(((ParentField)field).ParentView.DataTable.PrimaryKey[0].DataType))
                        {
                            data = "('" + string.Join("','", data.Split(',')) + "')";
                        }
                    }
                    break;

                //case DataType.MultiSelect:

                //    break;


                default:
                    break;
            }

            where += equality + data;



            return where;
        }

        protected bool IsNumeric(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumeric(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;

        }

        /*
        public virtual ContentResult ExportToExcel(string viewName, string guid)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "ExportToExcel");
            string SortColumn = SortHelper.GetSortColumn(view);
            string direction = SortHelper.GetSortDirection(view);

            FormCollection collection = new FormCollection(ViewHelper.GetSessionState(guid + "Filter"));
            //FormCollection collection = (FormCollection)ViewHelper.SetSessionState(viewName + "Filter");
            DataView dataView = GetDataTable(view, 1, 100000000, collection, ViewHelper.GetSessionString(guid + "Search"), SortColumn, direction, guid);

            DataAccess.Csv csv = new DataAccess.Csv();
            string content = csv.Export(dataView, view, SecurityHelper.GetCurrentUserRoles());

            string header = string.Empty;

            System.Data.DataRow HeadersRow = dataView.Table.NewRow();

            string columnHeader = string.Empty;

            int idx = 0;

            foreach (Field field in view.Fields.Values)
            {
                if (!field.HideInTable && !SecurityHelper.IsDenied(field.DenySelectRoles, field.AllowSelectRoles) && field.FieldType != FieldType.Children)
                {
                    columnHeader = field.DisplayName;
                    if (field.Equals(view.Fields.First()) && columnHeader.ToLower() == "id")
                    {
                        columnHeader = " " + columnHeader;
                    }
                    HeadersRow[idx] = columnHeader;
                    idx++;
                }
            }

            

            Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1255");

            Response.AppendHeader("content-disposition", "attachment; filename=" + viewName + ".xslx");

            //Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1255");
            //Response.BinaryWrite(System.Text.Encoding.GetEncoding("windows-1255").GetPreamble());


            return this.Content(content, "text/csv");
        }
        */
        //[AcceptVerbs(HttpVerbs.Post)]
        //public virtual ActionResult Import(string viewName, string sheetName, bool? allowDuplicate, bool? writeErrors)
        //{
        //    Durados.Web.Mvc.View view = GetView(viewName, "Import");

        //    string uploadPath;

        //    Upload upload = Database.Views["durados_Import"].Fields["FileName

        //    if (string.IsNullOrEmpty(upload.UploadPhysicalPath))
        //    {
        //        uploadPath = upload.UploadVirtualPath;
        //        if (!upload.UploadVirtualPath.StartsWith("~"))
        //        {
        //            uploadPath = "~" + uploadPath;
        //        }

        //        uploadPath = Server.MapPath(uploadPath);
        //    }
        //    else
        //    {
        //        uploadPath = upload.UploadPhysicalPath;
        //        if (!uploadPath.EndsWith(@"\"))
        //            uploadPath = uploadPath + @"\";
        //    }

        //    string strFileName = Path.GetFileName(Request.Files[0].FileName);
        //    //string strExtension = Path.GetExtension(Request.Files[0].FileName).ToLower();
        //    //string strSaveLocation = uploadPath + strFileName;
        //    string strSaveLocation = string.Empty;
        //    strFileName = SaveUploadFile(uploadPath, strFileName, upload.Override, out strSaveLocation);

        //    string src = upload.UploadVirtualPath + strFileName;

        //    Import(view, strSaveLocation, sheetName, GetOpenXmlConvertor(), allowDuplicate, writeErrors);
        //    return PartialView("~/Views/Shared/Controls/UploadResponse.ascx", new UI.Json.UploadInfo() { FileName = strFileName, Path = upload.UploadVirtualPath + strFileName });

        //    //return Json(new UI.Json.UploadInfo() { FileName = strFileName, Path = field.Upload.UploadVirtualPath + strFileName });
        //}


        /*
         * Import from excel - base methods
         * 
         * Changed by yossi - 8/2/11
         * 
         */

        protected virtual void prepareTableForImport(View view, ref DataTable table, ImportModes importMode)
        {

            //Set fields (columns) names from first row 
            string header;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                header = table.Rows[0][i].ToString().Trim();

                if (header == string.Empty) continue;

                table.Columns[i].ColumnName = header;
            }
            table.Rows.RemoveAt(0);


            //validate that we have all required fields for insert
            if (importMode != ImportModes.Update && !view.Database.IsConfig)  // insert / update or insert - modes
            {
                foreach (Field field in view.Fields.Values.Where(f => !f.IsExcludedInInsert() && f.Required))
                {
                    if (!table.Columns.Contains(field.DisplayName))
                    {
                        //table.Columns.Add(field.DisplayName);
                        throw new DuradosException("Import file missing required column [" + field.DisplayName + "]");
                    }
                }
            }
        }

        protected virtual View CreateView(DataTable table)
        {
            return null;
        }



        protected virtual bool IsSpecificImport()
        {
            return false;
        }

        protected virtual bool AddUnknownFieldsValues()
        {
            return false;
        }

        protected virtual string GetFieldValuByName()
        {
            return "Name";
        }

        protected virtual void ImportInit()
        {

        }

        protected virtual string ImportInsert(View view, Dictionary<string, object> values, System.Data.IDbCommand command, string Comments, int UserId, Importer importer, DataRow row)
        {
            throw new Exception("Import method not implemented");
            //return "Not implemented";
        }

        protected virtual string getViewNameForImport(string name)
        {
            return name;
        }

        protected virtual Importer GetNewImporter(SqlProduct sqlProduct, string ConnectionString, string sysConnectionString, bool doRollbackOnError)
        {
            switch (sqlProduct)
            {
                case SqlProduct.Oracle:
                    return new OracleImporter(ConnectionString, sysConnectionString, doRollbackOnError);
                case SqlProduct.Postgre:
                    return new PostrgreImporter(ConnectionString, sysConnectionString, doRollbackOnError);
                case SqlProduct.MySql:
                    return new MySqlImporter(ConnectionString, sysConnectionString, doRollbackOnError);
                default:
                    return new Importer(ConnectionString, sysConnectionString, doRollbackOnError);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult Import(string viewName, string fileName, string sheetName, bool? writeErrors, bool? rollBackOnError, int ImportModeIndex, ImportType? importType)
        {

            string uploadPath = ((ColumnField)Map.Database.Views["durados_Import"].Fields["FileName"]).GetUploadPath();

            string strSavedLocation = uploadPath + fileName;

            bool doRollBackOnError = (rollBackOnError.HasValue && rollBackOnError.Value) ? true : false;

            viewName = getViewNameForImport(viewName);

            Durados.Web.Mvc.View view = GetView(viewName, "Import");

            ImportModes ImportMode = (ImportModes)ImportModeIndex;

            if (ImportMode == ImportModes.Update)
            {
                if (!view.AllowEdit) throw new DuradosException("View security configurations do not allow edit!");
            }
            else
            {
                if (!view.AllowCreate) throw new DuradosException("View security configurations do not allow inserting new lines!");
            }

            Importer importer = GetNewImporter(Map.Database.SqlProduct, view.Database.IsConfig ? Map.Database.ConnectionString : view.ConnectionString, Map.Database.SystemConnectionString, doRollBackOnError);


            //Write errors to DataTable
            DataTable errorTable = new DataTable("Errors");
            errorTable.Columns.Add("RowNumber", typeof(int));
            errorTable.Columns.Add("Message", typeof(string));

            bool success = true;
            bool hasErrors = true;
            string message;

            Durados.Xml.Sdk.Excel.ExcelHandler excelHandler = new Durados.Xml.Sdk.Excel.ExcelHandler();

            try
            {
                ImportInit();

                //Read excel data to DataTable
                DataTable table = new DataTable();

                excelHandler.OpenSheet(strSavedLocation, ref sheetName);

                excelHandler.ReadDocument(view.Fields.Count, ref table);

                excelHandler.Dispose();



                //Set DataTable Structure for import + validate fields exist
                prepareTableForImport(view, ref table, ImportMode);

                if (view.Database.IsConfig)
                {
                    table.TableName = sheetName;
                    if (!string.IsNullOrEmpty(Request.Form["pageName"]))
                    {
                        table.TableName = Request.Form["pageName"];
                    }
                    view = CreateView(table);
                }

                if (importType.HasValue)
                {
                    //consider transaction
                    SyncViewSchemaToExcel(view, table, importer.getCommand());
                    if (viewName != "View")
                    {
                        view = (Durados.Web.Mvc.View)Map.Database.Views[viewName];
                        if (importType.Value == ImportType.Replace)
                            importer.CleanUpDbTable(view, GetControllerNameForLog(ControllerContext));
                    }

                }

                int userId = 1;

                int.TryParse(Map.Database.GetUserID(), out userId);

                string importGuid = Guid.NewGuid().ToString();

                importer.setUserId(userId);

                int i = 1;

                string errorMessage;

                foreach (DataRow row in table.Rows)
                {
                    i++;
                    bool error = false;

                    Durados.DataAccess.GetPKValueByDisplayValueStatus status;
                    Dictionary<string, object> values = new Dictionary<string, object>();

                    foreach (DataColumn col in table.Columns)
                    {
                        string displayName = col.ColumnName;

                        Field field;

                        Field[] fields = view.GetFieldsByDisplayName(displayName);
                        if (fields == null || fields.Length == 0)
                            field = null;
                        else if (fields.Length == 1)
                        {
                            field = fields[0];
                        }
                        else
                        {
                            if (ImportMode == ImportModes.Update)
                            {
                                fields = fields.Where(f => !f.ExcludeInUpdate && !f.Excluded).ToArray();
                            }
                            else
                            {
                                fields = fields.Where(f => !f.ExcludeInInsert && !f.Excluded).ToArray();
                            }


                            if (fields.Length == 1)
                            {
                                field = fields[0];
                            }
                            else
                            {
                                if (fields.Length > 1)
                                {
                                    field = null;
                                    errorMessage = "Column name [" + displayName + "] found more than once in the view!";
                                    errorTable.Rows.Add(i, errorMessage);
                                    error = true;
                                    continue;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        if (field == null && AddUnknownFieldsValues())
                        {
                            values.Add(displayName.ToLower(), row[col]);
                            continue;
                        }
                        else if (field == null)
                        {
                            errorMessage = "Column name [" + displayName + "] not found in view!";
                            errorTable.Rows.Add(i, errorMessage);
                            error = true;
                            continue;
                        }
                        else if ((ImportMode == ImportModes.Insert && field.IsExcludedInInsert()) || (ImportMode == ImportModes.Update && field.IsExcludedInUpdate()) || (ImportMode == ImportModes.UpdateOrInsert && field.IsExcludedInInsert() && field.IsExcludedInUpdate()))
                        {
                            continue;
                        }

                        object value = null;
                        if (field.FieldType == FieldType.Column)
                        {
                            value = row[col];

                            if (value == null || value.ToString().Trim() == string.Empty)
                            {
                                value = field.ConvertDefaultToString();
                            }
                            else if (field.GetColumnFieldType() == ColumnFieldType.Boolean)
                            {
                                if (value.ToString().ToLower() == field.View.Database.True.ToLower() || value.ToString().ToLower() == "true")
                                {
                                    value = true;
                                }
                                else if (value.ToString().ToLower() == field.View.Database.False.ToLower() || value.ToString().ToLower() == "false")
                                {
                                    value = false;
                                }
                                else
                                {
                                    errorMessage = "Boolean value for column [" + displayName + "] was not recognized by value [" + value.ToString() + "]";
                                    errorTable.Rows.Add(i, errorMessage);
                                    error = true;

                                    continue;
                                }
                            }
                            else if (field.GetColumnFieldType() == ColumnFieldType.DateTime && value is string)
                            {
                                int dateFromExcel = -1;
                                if (Int32.TryParse(value.ToString(), out dateFromExcel))
                                {
                                    value = ExcelSerialDateToDMY2(dateFromExcel);
                                }
                                else
                                {
                                    try
                                    {
                                        value = field.ConvertFromString(value.ToString()).ToString();
                                    }
                                    catch
                                    {
                                        errorMessage = "Date value for column [" + displayName + "] was not recognized by value [" + value.ToString() + "]";
                                        errorTable.Rows.Add(i, errorMessage);
                                        error = true;

                                        continue;
                                    }
                                }
                            }

                        }
                        else if (field.FieldType == FieldType.Parent)
                        {
                            string displayValue = row[col].ToString();

                            if (displayValue == string.Empty)
                            {
                                value = field.ConvertDefaultToString();
                            }
                            else
                            {
                                ParentField parentField = (ParentField)field;

                                Dictionary<Durados.ParentField, string> dependencyDisplayValues = null;
                                bool insideTriggerFieldOK = true;

                                if (parentField.InsideTriggerField != null)
                                {
                                    dependencyDisplayValues = new Dictionary<Durados.ParentField, string>();
                                    ParentField dependencyField = (ParentField)parentField.InsideTriggerField;
                                    while (dependencyField != null)
                                    {
                                        if (table.Columns.Contains(dependencyField.DisplayName))
                                        {
                                            string dependencyValue = row[dependencyField.DisplayName].ToString();
                                            if (string.IsNullOrEmpty(dependencyValue))
                                            {
                                                errorMessage = "The dependency field [" + dependencyField.DisplayName + "] of [" + parentField.DisplayName + "] is empty.";
                                                errorTable.Rows.Add(i, errorMessage);
                                                error = true;
                                                insideTriggerFieldOK = false;
                                                break;
                                            }
                                            dependencyDisplayValues.Add(dependencyField, dependencyValue);
                                        }
                                        else
                                        {
                                            errorMessage = "The dependency field [" + dependencyField.DisplayName + "] of [" + parentField.DisplayName + "] is missing.";
                                            errorTable.Rows.Add(i, errorMessage);
                                            error = true;
                                            insideTriggerFieldOK = false;
                                            break;
                                        }
                                        dependencyField = (ParentField)dependencyField.InsideTriggerField;
                                    }
                                }
                                if (insideTriggerFieldOK)
                                {
                                    value = parentField.GetPKValueByDisplayValue(displayValue, dependencyDisplayValues, out status);

                                    if (field.Import && status == GetPKValueByDisplayValueStatus.NotFound)
                                    {
                                        value = importer.CreateParentRecord((ParentField)field, displayValue);

                                        if (value.ToString() != string.Empty)
                                        {
                                            status = GetPKValueByDisplayValueStatus.FoundUnique;
                                        }
                                    }

                                    if (status != GetPKValueByDisplayValueStatus.FoundUnique)
                                    {
                                        if (parentField.InsideTriggerField != null && value.ToString() != string.Empty)
                                        {
                                            errorMessage = value.ToString();
                                        }
                                        else
                                        {
                                            if (status == GetPKValueByDisplayValueStatus.FoundMoreThanOne)
                                                errorMessage = "The display value [" + displayValue + "] is not a unique identifier of the parent record [" + displayName + "]";
                                            else
                                                errorMessage = "Primary key for parent record [" + displayName + "] was not found by value [" + displayValue + "]";
                                        }
                                        errorTable.Rows.Add(i, errorMessage);
                                        error = true;

                                        continue;
                                    }
                                }
                            }
                        }
                        else if (field.FieldType == FieldType.Children)
                        {
                            ChildrenField childrenField = (ChildrenField)field;

                            string displayValue = row[col].ToString().Trim();

                            if (displayValue == string.Empty)
                            {
                                value = field.ConvertDefaultToString();// childrenField ???
                            }
                            else
                            {
                                List<string> displayValues = displayValue.Split(',').ToList();

                                try
                                {
                                    value = childrenField.GetListPKDelimitedByValuesForImport(displayValues, importer);
                                }
                                catch (Exception exeption)
                                {
                                    errorMessage = exeption.Message;
                                    errorTable.Rows.Add(i, errorMessage);
                                    error = true;
                                    continue;
                                }

                            }

                        }

                        if (GetFieldValuByName() == "DisplayName")
                        {
                            values.Add(displayName.ToLower(), value);
                        }
                        else
                        {
                            values.Add(field.Name, value);
                        }
                    }

                    if (!error)
                    {
                        //if (!allowDuplicate.HasValue)
                        //    allowDuplicate = false;

                        //if (view.DisplayField.ReadOnly)
                        //    allowDuplicate = true;

                        string pk = "";
                        try
                        {
                            HandleSpecialDefaults(view, values, true);

                            if (IsSpecificImport())
                            {
                                pk = ImportInsert(view, values, importer.getCommand(), fileName + " " + importGuid, userId, importer, row);
                            }
                            else
                            {
                                pk = importer.Create(view, values, row, ImportMode);
                                string userViewName = ((Database)Database).UserViewName;

                                if (view.Name == userViewName)
                                {
                                    string usernameFieldName = ((Database)Database).UsernameFieldName;
                                    string roleDbName = "Role";
                                    string roleFieldName = ((Database)Database).GetUserView().GetFieldByColumnNames(roleDbName).Name;

                                    string currentRole = "User";
                                    if (values.ContainsKey(roleFieldName) && !string.IsNullOrEmpty(values[roleFieldName].ToString()))
                                        currentRole = values[roleFieldName].ToString();
                                    string newUser = values[usernameFieldName].ToString();

                                    HandleMultiTenancyUser(currentRole, newUser);
                                    AfterCreateBeforeCommit(new CreateEventArgs(view, values, pk, importer.getCommand(), importer.getSysCommand()));
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            errorTable.Rows.Add(i, exception.Message);
                        }

                        if (pk == "exist")
                        {
                            errorTable.Rows.Add(i, "Row didn't imported because there is already row with same values in the database");
                        }
                    }
                }

                bool cancelCommit = (errorTable.Rows.Count > 0);

                string CommitMessage = importer.ImportCommit(cancelCommit, view.Name);
                string importcompleteWithErrorMessage = "to view the errors download the file by clicking the download icon near the upload field, the errors are in sheet name - Errors.";
                string errMsg1 = string.Empty;
                ImportTerm(cancelCommit);

                if (!writeErrors.HasValue)
                    writeErrors = false;

                if (errorTable.Rows.Count > 0 && writeErrors.Value)
                {
                    excelHandler = new Durados.Xml.Sdk.Excel.ExcelHandler(strSavedLocation, sheetName);
                    excelHandler.SaveErrorsDataTable(errorTable);

                    excelHandler.Dispose();
                }
                string successMessage = string.Format(Map.Database.Localizer.Translate("Successfully update {0} records to table {1}"), table.Rows.Count, view.DisplayName);
                if (errorTable.Rows.Count > 0 || CommitMessage != "ok")
                {
                    if (CommitMessage == "ok")
                    {
                        message = Map.Database.Localizer.Translate("Import complete with errors");
                    }
                    else
                    {
                        message = CommitMessage;
                    }

                    if (writeErrors.HasValue && writeErrors.Value)
                        errMsg1 = Map.Database.Localizer.Translate("completerWithErrorMessage");
                    errMsg1 = errMsg1 == "completerWithErrorMessage" ? importcompleteWithErrorMessage : errMsg1;
                    message += ", " + errMsg1;
                }
                else
                {
                    hasErrors = false;
                    message = successMessage;
                }
            }
            catch (Exception exception)
            {
                if (excelHandler != null)
                {
                    excelHandler.Dispose();
                }
                success = false;
                message = Map.Database.Localizer.Translate("Import failed to complete with the following error:") + " " + exception.Message;
            }
            finally
            {
                importer.CloseConnections();
            }

            importer.ImportEnd();

            return Json(new { success = success, message = message, viewName = view.Name, hasErrors = hasErrors });
        }


        protected virtual void SyncViewSchemaToExcel(Mvc.View view, DataTable table, IDbCommand command)
        {


        }

        protected virtual void ImportTerm(bool cancelCommit)
        {

        }

        public string ExcelSerialDateToDMY(int nSerialDate)
        {
            // Excel/Lotus 123 have a problem with 29-02-1900. 1900 is not a leap year, but Excel/Lotus 123 think it is...

            int nDay;
            int nMonth;
            int nYear;

            if (nSerialDate == 60)
            {
                nDay = 29;
                nMonth = 2;
                nYear = 1900;

                return nDay + "-" + nMonth + "-" + nYear;
            }

            if (nSerialDate < 60)
            {
                // Because of the 29-02-1900 problem, any serial date under 60 is one off... Compensate.
                nSerialDate++;
            }

            // Modified Julian to DMY calculation with an addition of 2415019
            var l = nSerialDate + 68569 + 2415019;
            var n = (4 * l) / 146097;
            l = l - (146097 * n + 3) / 4;
            var i = (4000 * (l + 1)) / 1461001;
            l = l - (1461 * i) / 4 + 31;
            var j = (80 * l) / 2447;
            nDay = l - (2447 * j) / 80;
            l = j / 11;
            nMonth = j + 2 - (12 * l);
            nYear = 100 * (n - 49) + i + l;

            return nDay + "-" + nMonth + "-" + nYear;
        }

        public DateTime ExcelSerialDateToDMY2(int nSerialDate)
        {

            return DateTime.FromOADate(nSerialDate);
        }

        public virtual string GetUploadForRich(string viewName, string fieldName, string guid)
        {
            UI.ColumnFieldViewer columnFieldViewer = new UI.ColumnFieldViewer();
            View view = GetView(viewName);
            if (view == null)
                return string.Empty;

            if (!view.Fields.ContainsKey(fieldName))
                return string.Empty;

            ColumnField field = (ColumnField)view.Fields[fieldName];

            return columnFieldViewer.GetUploadForCreate(field, string.Empty, guid);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Upload(string viewName, string fieldName, FormCollection collection)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "Upload");

                Durados.Web.Mvc.ColumnField field = (Durados.Web.Mvc.ColumnField)view.Fields[fieldName];

                string strFileName = Path.GetFileName(Request.Files[0].FileName);


                if (field.FtpUpload != null)
                {
                    return FtpUpload(viewName, fieldName, collection);
                }

                if (!string.IsNullOrEmpty(field.Upload.FileAllowedTypes))
                {
                    string strExtension = Path.GetExtension(Request.Files[0].FileName).ToLower().TrimStart('.');
                    string[] exts = field.Upload.FileAllowedTypes.Split(',');

                    bool valid = false;

                    foreach (string ext in exts)
                    {
                        if (ext.Trim().Equals(strExtension))
                        {
                            valid = true;
                            break;
                        }
                    }
                    if (!valid)
                    {
                        throw new Exception("Invalid file type in field [" + field.DisplayName + "].<br><br>Allowed formats: " + field.Upload.FileAllowedTypes);
                    }
                }

                if (field.Upload.FileMaxSize > 0)
                {
                    int fileSize = (int)(Request.Files[0].ContentLength / 1024);

                    if (fileSize > field.Upload.FileMaxSize)
                    {
                        throw new Exception("File too big in field [" + field.DisplayName + "].<br><br>Max Allowed size: " + field.Upload.FileMaxSize + "kb");
                    }
                }




                string uploadPath = field.GetUploadPath();
                HandleMultiTenancyUploadPath(ref uploadPath);

                string strSaveLocation = string.Empty;

                strFileName = SaveUploadFile(uploadPath, strFileName, field.Upload.Override, out strSaveLocation);

                //string src = field.Upload.GetFixedVirtualPath() + strFileName;
                string url = Url.Action(string.Format("{0}/{1}", Map.DownloadActionName, field.View.Name), GetControllerName(), new { fieldName = field.Name, fileName = strFileName, pk = string.Empty });


                if (viewName == "durados_App" && fieldName == "Image" && Maps.Cloud)
                {
                    string folder = Maps.AzureAppPrefix + Maps.Instance.GetCurrentAppId();
                    SaveUploadedFileToAzure(Maps.AzureStorageAccountName, Maps.AzureStorageAccountKey, folder, strFileName);
                }

                return PartialView("~/Views/Shared/Controls/UploadResponse.ascx", new UI.Json.UploadInfo() { FileName = strFileName, Path = Url.Encode(url) });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Content("Upload faild: " + exception.Message);
            }

            //return Json(new UI.Json.UploadInfo() { FileName = strFileName, Path = field.Upload.UploadVirtualPath + strFileName });
        }

        protected virtual void HandleMultiTenancyUploadPath(ref string uploadPath)
        {

        }

        protected virtual string SaveUploadFile(string uploadPath, string strFileName, bool overrideExistingFile, out string strSaveLocation)
        {
            strSaveLocation = uploadPath + strFileName;
            string fileWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(strSaveLocation);

            if (!overrideExistingFile)
            {
                int i = 0;
                while (System.IO.File.Exists(strSaveLocation))
                {
                    i++;

                    strFileName = fileWithoutExtension + "_" + i.ToString() + System.IO.Path.GetExtension(strSaveLocation);
                    strSaveLocation = uploadPath + strFileName;
                }
            }

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(strSaveLocation);
            fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.

            //if (!Directory.Exists(uploadPath))
            //    Directory.CreateDirectory(uploadPath);
            Request.Files[0].SaveAs(strSaveLocation);

            return System.IO.Path.GetFileName(strSaveLocation);
        }

        protected virtual string GetFtpUploadTarget(ColumnField field, string strFileName)
        {
            return field.FtpUpload.GetFtpFilepath(strFileName);
        }

        protected virtual System.Net.NetworkCredential GetFtpNetworkCredential(ColumnField field)
        {
            return new System.Net.NetworkCredential(field.FtpUpload.FtpUserName, field.FtpUpload.GetDecryptedPassword(Map.GetConfigDatabase()));
        }

        protected virtual System.Net.FtpWebRequest CreateFtpRequest(ColumnField field, string strFileName)
        {
            FtpUpload uploader = field.FtpUpload;

            string target = GetFtpUploadTarget(field, strFileName);

            System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.HttpWebRequest.Create(target);

            //request.Credentials = new System.Net.NetworkCredential(uploader.FtpUserName, uploader.FtpPassword);
            //request.Credentials = new System.Net.NetworkCredential(uploader.FtpUserName, uploader.GetDecryptedPassword(Map.GetConfigDatabase()));
            request.Credentials = GetFtpNetworkCredential(field);

            request.UsePassive = uploader.UsePassive;
            request.UseBinary = true;
            request.KeepAlive = false;

            return request;
        }


        //FtpDownLoad
        protected virtual FileResult FtpDownLoad(ColumnField field, string strFileName)
        {
#if DebugLocal
                        return null;
#endif
            Stream ftpStream = null;
            IUpload upload = UploadFactory.GetUpload(field);
            strFileName = upload.GetUploadPath(strFileName);
            try
            {
                System.Net.FtpWebRequest reqFTP = CreateFtpRequest(field, strFileName);

                reqFTP.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;

                var response = (System.Net.FtpWebResponse)reqFTP.GetResponse();

                ftpStream = response.GetResponseStream();//response.StatusCode / response.StatusDescription

                return new FtpDownloadResult("octet-stream") { FileDownloadName = strFileName, responseStream = ftpStream };

                /*                 
                string file_path = Server.MapPath("/Uploads/" + strFileName);
                FileStream outputStream = new FileStream(file_path, FileMode.Create);

                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);

                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close(); 
                 
                return File(file_path, "application/octet-stream", strFileName);
                */
            }
            catch (Exception ex)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Source, ex, 1, null);
                //throw new DuradosException(ex.Message.ToString());
                return null;

            }
        }

        public virtual JsonResult FtpDeleteFile(string viewName, string fieldName, string filename)
        {
            fieldName = Server.UrlDecode(fieldName);
            filename = Server.UrlDecode(filename);

            string[] fragents = filename.Split('.');

            string extension = string.Empty;

            if (fragents.Length > 0)
                extension = fragents.Last();

            ColumnField field = (ColumnField)Database.Views[viewName].Fields[fieldName];

            if (field.FtpUpload.StorageType == StorageType.Azure)
            {
                return DataleUploadedFileFromAzure(field, filename);
            }

            System.Net.FtpWebRequest request = CreateFtpRequest(field, filename);

            request.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;


            try
            {
                var response = (System.Net.FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch
            {
                return Json("Failure");
            }

            return Json("Success");

        }

        protected virtual JsonResult DataleUploadedFileFromAzure(ColumnField field, string filename)
        {
            try
            {
                DataleUploadedFileFromAzure(field.FtpUpload.AzureAccountName, field.FtpUpload.GetDecryptedAzureAccountKey(Map.GetConfigDatabase()), field.FtpUpload.DirectoryBasePath, filename);
                return Json("Success");
            }
            catch (Exception ex)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Source, ex, 5, null);
                return Json("Failure");
            }

        }

        protected virtual void DataleUploadedFileFromAzure(string accountName, string accountKey, string folder, string strFileName)
        {
            if (Maps.AzureStorageAccountKey == accountKey)
                folder = Maps.AzureAppPrefix + Maps.Instance.GetCurrentAppId();

            // Variables for the cloud storage objects.
            CloudStorageAccount cloudStorageAccount;
            CloudBlobClient blobClient;
            CloudBlobContainer blobContainer;
            CloudBlob blob;

            //cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=qacontent;AccountKey=6/OpnuGeLRm8REKhQ/5Led/WoJcHjmFJII4GM5HYJ120o7OxtgC1zCstw0kqyn05N2m1jGadY3aYFypOzD4L5A==");
            cloudStorageAccount = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}", accountName, accountKey));

            // Create the blob client, which provides
            // authenticated access to the Blob service.
            blobClient = cloudStorageAccount.CreateCloudBlobClient();

            // Get the container reference.
            //blobContainer = blobClient.GetContainerReference("general");
            blobContainer = blobClient.GetContainerReference(folder);

            blob = blobContainer.GetBlobReference(strFileName);
            blob.DeleteIfExists();
        }

        protected virtual bool CheckIfFileExistInFtp(ColumnField field, string strFileName)
        {
            System.Net.FtpWebRequest request = CreateFtpRequest(field, strFileName);
            request.Method = System.Net.WebRequestMethods.Ftp.GetDateTimestamp;

            try
            {
                var response = (System.Net.FtpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException ex)
            {
                var response = (System.Net.FtpWebResponse)ex.Response;
                if (response.StatusCode == System.Net.FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
                else
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Source, ex, 1, null);
                    throw new DuradosException(response.StatusDescription);
                }
            }

            return true;
        }

        protected virtual string SaveUploadedFileToAzure(ColumnField field, string strFileName)
        {
            return SaveUploadedFileToAzure(field.FtpUpload.AzureAccountName, field.FtpUpload.GetDecryptedAzureAccountKey(Map.GetConfigDatabase()), field.FtpUpload.DirectoryBasePath, strFileName);
        }

        protected virtual string SaveUploadedFileToAzure(string accountName, string accountKey, string folder, string strFileName)
        {
            string fileUri = string.Empty;
            try
            {
                if (Maps.AzureStorageAccountKey == accountKey)
                    folder = Maps.AzureAppPrefix + Maps.Instance.GetCurrentAppId(); //Azure must have at least 3 chars

                // Variables for the cloud storage objects.
                CloudStorageAccount cloudStorageAccount;
                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;
                BlobContainerPermissions containerPermissions;
                CloudBlob blob;

                //cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=qacontent;AccountKey=6/OpnuGeLRm8REKhQ/5Led/WoJcHjmFJII4GM5HYJ120o7OxtgC1zCstw0kqyn05N2m1jGadY3aYFypOzD4L5A==");
                cloudStorageAccount = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}", accountName, accountKey));

                // Create the blob client, which provides
                // authenticated access to the Blob service.
                blobClient = cloudStorageAccount.CreateCloudBlobClient();

                // Get the container reference.
                //blobContainer = blobClient.GetContainerReference("general");
                blobContainer = blobClient.GetContainerReference(folder);

                // Create the container if it does not exist.
                if (Maps.AzureStorageAccountKey == accountKey)
                {
                    // Set permissions on the container.
                    blobContainer.CreateIfNotExist();
                    containerPermissions = new BlobContainerPermissions();
                    containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                    blobContainer.SetPermissions(containerPermissions);
                }

                // Get a reference to the blob.
                blob = blobContainer.GetBlobReference(strFileName);

                blob.Properties.ContentType = Request.Files[0].ContentType;

                // Upload a file from the local system to the blob.
                //blob.UploadFile(Request.Files[0].FileName);  // File from emulated storage.
                blob.UploadFromStream(Request.Files[0].InputStream);
                fileUri = blob.Uri.ToString();


            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return (Map.Database.Localizer.Translate("Upload failed: " + exception.Message));
            }

            return fileUri;

        }


        protected virtual string SaveUploadedFileToAws(ColumnField field, string strFileName)
        {
            return SaveUploadedFileToAws(field.FtpUpload.AwsAccessKeyId, field.FtpUpload.GetDecryptedAwsSecretAccessKey(Map.GetConfigDatabase()), field.FtpUpload.DirectoryBasePath, strFileName);
        }

        protected virtual string SaveUploadedFileToAws(string accessKeyID, string secretAccessKey, string existingBucketName, string strFileName)
        {
            try
            {
                TransferUtility fileTransferUtility = new
                    TransferUtility(accessKeyID, secretAccessKey);

                //// 1. Upload a file, file name is used as the object key name.
                //fileTransferUtility.Upload(filePath, existingBucketName);
                //Console.WriteLine("Upload 1 completed");

                //// 2. Specify object key name explicitly.
                //fileTransferUtility.Upload(filePath,
                //                          existingBucketName, keyName);
                //Console.WriteLine("Upload 2 completed");

                // 3. Upload data from a type of System.IO.Stream.
                string keyName = strFileName;
                fileTransferUtility.Upload(Request.Files[0].InputStream, existingBucketName, keyName);

                Console.WriteLine("Upload 3 completed");

                //// 4.// Specify advanced settings/options.
                //TransferUtilityUploadRequest fileTransferUtilityRequest =
                //    new TransferUtilityUploadRequest()
                //    .WithBucketName(existingBucketName)
                //    .WithFilePath(filePath)
                //    .WithStorageClass(S3StorageClass.ReducedRedundancy)
                //    .WithMetadata("param1", "Value1")
                //    .WithMetadata("param2", "Value2")
                //    .WithPartSize(6291456) // This is 6 MB.
                //    .WithKey(keyName)
                //    .WithCannedACL(S3CannedACL.PublicRead);
                //fileTransferUtility.Upload(fileTransferUtilityRequest);
                //Console.WriteLine("Upload 4 completed");

                SetACLRequest request = new Amazon.S3.Model.SetACLRequest();
                request.BucketName = existingBucketName;
                request.Key = keyName;
                request.CannedACL = S3CannedACL.PublicRead;
                fileTransferUtility.S3Client.SetACL(request);

                //string preSignedURL = fileTransferUtility.S3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
                //{
                //    BucketName = existingBucketName,
                //    Key = keyName,
                //    Expires = System.DateTime.Now.AddDays(1000)
                //});

                //return preSignedURL;

                string seperator = "/";
                existingBucketName = existingBucketName.Trim(seperator.ToCharArray()) + seperator;
                keyName = keyName.Trim(seperator.ToCharArray());
                return string.Format("http://s3.amazonaws.com/{0}{1}", existingBucketName, keyName);

            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
            }
            return string.Empty;
        }

        protected virtual string SaveUploadedFileToFtp(ColumnField field, string strFileName)
        {

            if (!field.FtpUpload.Override && CheckIfFileExistInFtp(field, strFileName))
            {
                throw new DuradosException(Map.Database.Localizer.Translate("File with same name already exist"));
            }

            try
            {
                System.Net.FtpWebRequest request = CreateFtpRequest(field, strFileName);

                request.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

                Stream stream = Request.Files[0].InputStream;

                byte[] buffer = new byte[stream.Length];

                //StreamReader sourceStream = new StreamReader(stream); //Only for text files !!!
                //byte[] fileContents = System.Text.Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());//Only for text files !!!

                int count = stream.Read(buffer, 0, buffer.Length);

                request.ContentLength = buffer.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse();

                response.Close();
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw new DuradosException(Map.Database.Localizer.Translate("Operation failed" + ", " + exception.Message));
            }

            return string.Empty;//response.StatusCode / response.StatusDescription
        }

        protected virtual bool FtpUploadValidExtension(string extension)
        {
            HashSet<string> h = new HashSet<string>(new string[] { "ade", "adp", "app", "bas", "bat", "chm", "cmd", "cpl", "crt", "csh", "exe", "fxp", "hlp", "hta", "inf", "ins", "isp", "js", "jse", "ksh", "Lnk", "mda", "mdb", "mde", "mdt", "mdt", "mdw", "mdz", "msc", "msi", "msp", "mst", "ops", "pcd", "pif", "prf", "prg", "pst", "reg", "scf", "scr", "sct", "shb", "shs", "url", "vb", "vbe", "vbs", "wsc", "wsf", "wsh" });
            return !h.Contains(extension.ToLower());
        }

        protected virtual bool FtpUploadValidSize(ColumnField field, float fileSize)
        {
            return true;
        }

        protected virtual bool FtpUploadValidFolderSize(ColumnField field, float fileSize)
        {
            return true;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult FtpUpload(string viewName, string fieldName, FormCollection collection)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "FtpUpload");

                Durados.Web.Mvc.ColumnField field = (Durados.Web.Mvc.ColumnField)view.Fields[fieldName];

                string strFileName = Path.GetFileName(Request.Files[0].FileName);

                string strExtension = Path.GetExtension(Request.Files[0].FileName).ToLower().TrimStart('.');
                if (!FtpUploadValidExtension(strExtension))
                {
                    throw new Exception("Invalid file type");
                }

                if (!string.IsNullOrEmpty(field.FtpUpload.FileAllowedTypes))
                {
                    string[] exts = field.FtpUpload.FileAllowedTypes.Split(',');

                    bool valid = false;

                    foreach (string ext in exts)
                    {
                        if (ext.Trim().Equals(strExtension))
                        {
                            valid = true;
                            break;
                        }
                    }
                    if (!valid)
                    {
                        throw new Exception("Invalid file type in field [" + field.DisplayName + "].<br><br>Allowed formats: " + field.FtpUpload.FileAllowedTypes);
                    }
                }

                float fileSize = (Request.Files[0].ContentLength / 1024) / 1000;

                if (!FtpUploadValidSize(field, fileSize))
                {
                    throw new Exception("The file has exceeded the size limit.");
                }

                if (!FtpUploadValidFolderSize(field, fileSize))
                {
                    throw new Exception("The folder has exceeded the size limit.");
                }

                if (field.FtpUpload.FileMaxSize > 0)
                {

                    if (fileSize > field.FtpUpload.FileMaxSize)
                    {
                        throw new Exception("File too big in field [" + field.DisplayName + "].<br><br>Max Allowed size: " + field.FtpUpload.FileMaxSize + " MB");
                    }
                }

                string strSaveLocation = string.Empty;

                string src = string.Empty;

                if (field.FtpUpload.StorageType == StorageType.Azure)
                {
                    src = SaveUploadedFileToAzure(field, strFileName);
                }
                else if (field.FtpUpload.StorageType == StorageType.Aws)
                {
                    src = SaveUploadedFileToAws(field, strFileName);
                }
                else
                {
                    SaveUploadedFileToFtp(field, strFileName);

                    if (field.FtpUpload.StorageType != StorageType.Azure)
                    {
                        src = field.FtpUpload.DirectoryVirtualPath.TrimEnd('/') + "/" + ((string.IsNullOrEmpty(field.FtpUpload.DirectoryBasePath)) ? string.Empty : (field.FtpUpload.DirectoryBasePath.TrimStart('/').TrimEnd('/') + "/")) + strFileName;
                    }
                    else
                    {
                        src = Url.Encode((new UI.ColumnFieldViewer()).GetDownloadUrl(field, string.Empty));
                    }
                }

                return PartialView("~/Views/Shared/Controls/UploadResponse.ascx", new UI.Json.UploadInfo() { FileName = strFileName, Path = src });

            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Content("Upload faild: " + exception.Message);
            }

            //return Json(new UI.Json.UploadInfo() { FileName = strFileName, Path = field.Upload.UploadVirtualPath + strFileName });
        }



        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            if (DuradosView != null && removeDuradosEvents)
            {
                foreach (ParentField parentField in DuradosView.Fields.Values.Where(field => field.FieldType == FieldType.Parent))
                {
                    parentField.BeforeDropDownOptions -= new ParentField.BeforeDropDownOptionsEventHandler(View_BeforeDropDownOptions);

                    ParentField dependencyField = (ParentField)parentField.DependencyField;
                    while (dependencyField != null)
                    {
                        dependencyField.BeforeDropDownOptions -= new ParentField.BeforeDropDownOptionsEventHandler(View_BeforeDropDownOptions);
                        dependencyField = (ParentField)dependencyField.DependencyField;
                    }
                }
                removeDuradosEvents = false;
            }

        }

        protected virtual DataView GetDataTable(string viewName, int page, FormCollection collection, string search, string SortColumn, string direction, string guid)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "GetDataTable");
            return GetDataTable(view, page, PagerHelper.GetPageSize(view, guid), collection, search, SortColumn, direction, guid);
        }

        protected virtual DataView GetDataTable(Durados.Web.Mvc.View view, int page, FormCollection collection, string search, string SortColumn, string direction, string guid)
        {
            return GetDataTable(view, page, PagerHelper.GetPageSize(view, guid), collection, search, SortColumn, direction, guid);
        }

        protected virtual void RegisterDropDownFilterEvents(Durados.Web.Mvc.View view)
        {
            removeDuradosEvents = true;
            foreach (ParentField parentField in view.Fields.Values.Where(field => field.FieldType == FieldType.Parent))
            {
                parentField.BeforeDropDownOptions += new ParentField.BeforeDropDownOptionsEventHandler(View_BeforeDropDownOptions);

                ParentField dependencyField = (ParentField)parentField.DependencyField;
                while (dependencyField != null)
                {
                    dependencyField.BeforeDropDownOptions += new ParentField.BeforeDropDownOptionsEventHandler(View_BeforeDropDownOptions);
                    dependencyField = (ParentField)dependencyField.DependencyField;
                }
            }
        }

        protected Dictionary<string, object> GetSessionFilterValues(string guid)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            FormCollection collection = new FormCollection(ViewHelper.GetSessionState(guid + "Filter"));

            UI.Json.Filter jsonFilter = ViewHelper.GetJsonFilter(collection, guid);
            if (jsonFilter != null)
            {
                foreach (UI.Json.Field jsonField in jsonFilter.Fields.Values)
                {
                    if (jsonField.Value != null && !string.IsNullOrEmpty(jsonField.Value.ToString()))
                    {
                        values.Add(jsonField.Name, jsonField.Value);
                    }
                }
            }

            return values;
        }

        public virtual DataRow GetDataRow(Durados.View view, string pk)
        {
            return view.GetDataRow(pk);
        }

        public virtual DataTable GetDataTable(Durados.View view)
        {
            int rowCount = 0;
            return ((View)view).FillPage(1, 10000, new Dictionary<string, object>(), false, false, null, out rowCount, null, null).Table;
        }

        protected virtual DataView GetDataTable(Durados.Web.Mvc.View view, int page, int pageSize, FormCollection collection, string search, string SortColumn, string direction, string guid)
        {

            DuradosView = view;

            RegisterDropDownFilterEvents(view);


            int rowCount = 0;
            SortDirection sortDirection = SortDirection.Asc;
            if (!string.IsNullOrEmpty(direction))
            {
                object o = Enum.Parse(typeof(SortDirection), direction, true);
                if (o != null)
                    sortDirection = (SortDirection)o;
            }

            Durados.Web.Mvc.UI.Json.Filter filter = null;

            DataView dataView = view.ApplyFilter(page, pageSize, collection, !string.IsNullOrEmpty(search), SortColumn, sortDirection, out rowCount, out filter, guid, view_BeforeSelect, view_AfterSelect, GetNewTableViewer());

            Dictionary<string, UI.Json.Field> filterFields = null;

            if (filter != null && filter.Fields.Count > 0)
            {
                //ViewData["filter"] = filter.Fields;
                filterFields = filter.Fields;
            }
            else
            {
                Durados.Web.Mvc.UI.ViewViewer viewViewer = new Durados.Web.Mvc.UI.ViewViewer();
                //ViewData["filter"] = viewViewer.GetJsonFilter(view, guid).Fields;
                filterFields = viewViewer.GetJsonFilter(view, guid).Fields;
            }

            ViewData["filter"] = filterFields;
            ViewData["ColumnsExcluder"] = GetNewColumnsExcluder(view, filterFields);

            ViewData["search"] = search;

            rowCount = GetRowCount() ?? rowCount;

            ViewData["rowCount"] = rowCount;
            return dataView;

            //return Database.Views[viewName].DataTable;
        }

        protected virtual int? GetRowCount()
        {
            return null;
        }

        void View_BeforeDropDownOptions(object sender, BeforeDropDownOptionsEventArgs e)
        {
            string sql = e.Sql;
            DropDownFilter((Durados.Web.Mvc.ParentField)e.ParentField, ref sql);
            e.Sql = sql;
        }

        protected virtual void DropDownFilter(ParentField parentField, ref string sql)
        {
            sql = HandleRolesFilter(sql, parentField);
        }

        private string HandleRolesFilter(string sql, ParentField parentField)
        {
            if (Maps.MultiTenancy)
            {
                Durados.View view = parentField.ParentView;
                if (System.Web.HttpContext.Current.User == null || System.Web.HttpContext.Current.User.Identity == null || System.Web.HttpContext.Current.User.Identity.Name == null)
                {
                    throw new AccessViolationException();
                }
                //if ((new string[1] { Map.Database.RoleViewName}).Contains(view.Name))
                if (view.Name == Map.Database.RoleViewName && !Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer"))
                {
                    sql += " and Name <> 'Developer'";
                }
            }
            return sql;
        }

        protected virtual void view_BeforeSelect(object sender, SelectEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Sql))
                SetPermanentFilter((Durados.Web.Mvc.View)e.View, (Durados.DataAccess.Filter)e.Filter);
            else
                SetSql(e);
        }

        protected virtual void view_AfterSelect(object sender, SelectEventArgs e)
        {
        }

        protected virtual void SetPermanentFilter(View view, Durados.DataAccess.Filter filter)
        {
        }

        protected virtual void SetSql(SelectEventArgs e)
        {
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult CreateOnly(string viewName, FormCollection collection)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "CreateOnly");
                CreateRow(viewName, collection, null);
                return Json("success");
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("$$error$$" + FormatExceptionMessage(viewName, "CreateOnly", exception));
            }
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult InlineAddingCreate(string viewName, FormCollection collection)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "InlineAddingCreate");
                DataRow dataRow = CreateRow(viewName, collection, null);
                if (IsConfigurationField(viewName))
                {
                    return Json(string.Empty);
                }
                else
                {
                    return Json(view.GetJsonDisplayValue(dataRow));
                }
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("$$error$$" + FormatExceptionMessage(viewName, "InlineAddingCreate", exception));
            }
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult PreviewModeOff()
        {
            return Json("success");
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult InlineEditingEdit(string viewName, FormCollection collection, string pk, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "Edit");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                view.EditRow(collection, pk, false, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);


                DataRow dataRow = view.GetDataRow(pk);
                return Json(view.GetJsonDisplayValue(dataRow));
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("$$error$$" + FormatExceptionMessage(viewName, "InlineAddingCreate", exception));
            }
        }

        protected virtual void AfterCreate(View view, string pk)
        {
        }

        protected virtual void Duplicate(View view, string pk, string duplicatePK)
        {
            Durados.DataAccess.Duplicator duplicator = new Duplicator();

            duplicator.Duplicate(view, duplicatePK, pk);
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult CreateStep(string viewName, FormCollection collection, string guid, string insertAbovePK, bool? duplicateRecursive, string duplicatePK)
        {
            try
            {
                DataRow dataRow = CreateRow(viewName, collection, insertAbovePK);
                View view = GetView(viewName, "Create");

                string pk = view.GetPkValue(dataRow);

                AfterCreate(view, pk);

                return Step(viewName, pk, guid, false);

            }
            catch (MessageException exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                return PartialView("~/Views/Shared/Controls/Message.ascx", FormatExceptionMessage(viewName, "Create", exception));
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Create", exception));
            }
        }

        protected virtual bool IsConfigurationField(string viewName)
        {
            return false;
        }

        protected string newFieldPk = null;

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Create(string viewName, FormCollection collection, string guid, string insertAbovePK, bool? duplicateRecursive, string duplicatePK)
        {
            try
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "Create", "Before Create", string.Empty, 3, collection["jsonView"], DateTime.Now);
                DataRow dataRow = CreateRow(viewName, collection, insertAbovePK);
                View view = GetView(viewName, "Create");

                string pk = view.GetPkValue(dataRow);

                if (string.IsNullOrEmpty(pk))
                {
                    if (IsConfigurationField(viewName))
                    {
                        if (string.IsNullOrEmpty(newFieldPk))
                        {
                            return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
                        }
                        else
                        {
                            return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid, newPk = newFieldPk });
                        }
                    }
                    else
                    {
                        throw new DuradosException("The row was created but its primary key was failed to retreive. Please check that the editable table and its view has the same primary key.");
                    }
                }
                if (duplicateRecursive.HasValue && duplicateRecursive.Value && !string.IsNullOrEmpty(duplicatePK))
                {
                    //Durados.DataAccess.Duplicator duplicator = new Duplicator();

                    //duplicator.Duplicate(view, duplicatePK, pk);
                    Duplicate(view, pk, duplicatePK);
                }

                AfterCreate(view, pk);

                OldNewValue[] oldNewValues = GetOldNewValues(view, dataRow, pk);
                wfe.Notifier.Notify(view, 1, GetUsername(), oldNewValues, pk, dataRow, this, null, GetSiteWithoutQueryString(), GetMainSiteWithoutQueryString());


                //ViewData["pk"] = pk;
                //return Json(new Json.JsonResult(row, true));
                return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid, newPk = pk });
            }
            catch (MessageException exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, collection["jsonView"]);
                return PartialView("~/Views/Shared/Controls/Message.ascx", FormatExceptionMessage(viewName, "Create", exception));
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, collection["jsonView"]);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Create", exception));
            }
        }

        //[ValidateInput(false)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public virtual JsonResult Duplicate(string viewName, string pk, string guid)
        //{
        //    try
        //    {
        //        Durados.DataAccess.Duplicator duplicator = new Duplicator();

        //        View view = GetView(viewName, "Duplicate");
        //        string duplicatedPk = duplicator.Duplicate(view, pk);

        //        Durados.Web.Mvc.Logging.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), null, null, 3, null);
        //        return Json(duplicatedPk);
        //    }
        //    catch (Exception exception)
        //    {
        //        //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
        //        Durados.Web.Mvc.Logging.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
        //        return Json("$$error$$" + FormatExceptionMessage(viewName, "Duplicate", exception));
        //    }
        //}

        protected virtual string FormatExceptionMessage(string viewName, string action, Exception exception)
        {
            if (exception is Durados.Workflow.WorkflowEngineException)
                return exception.Message;

            if (exception is System.Data.SqlClient.SqlException && exception.Message.ToLower().Contains("unique index"))
            {
                string message = exception.Message;
                message = message.Replace("\"", "").Replace("'", "").Replace(".", "").Replace(",", "").Replace("\n\r", " ").Replace("\r\n", " ");
                string indexName = message.Split(' ').Where(s => s.StartsWith("IX_")).FirstOrDefault();

                View view = (View)Map.Database.Views[viewName];



                message = Map.Database.Localizer.Translate("A " + view.DisplayName + " row with {0} already exists");

                string fieldsNames = null;

                try
                {
                    fieldsNames = view.GetIndexFieldsDisplayNames(indexName);
                }
                catch { }

                if (string.IsNullOrEmpty(fieldsNames))
                {
                    fieldsNames = Map.Database.Localizer.Translate("unique values");
                }

                return string.Format(message, fieldsNames);
            }

            else if (exception is DuradosCreateException && exception.InnerException is System.Data.SqlClient.SqlException && exception.InnerException.Message.ToLower().Contains("unique index"))
            {
                DuradosCreateException duradosCreateException = (DuradosCreateException)exception;

                string displayValue = duradosCreateException.View.GetDisplayValue(duradosCreateException.Values);

                if (displayValue == null)
                    return "The row is already exists";

                string message = "The row " + displayValue + " already exists";

                return message;
            }

            else if (exception is DuradosException || exception is Durados.Xml.DuradosXmlException)
                return exception.Message;

            string generalErrorMessage = Map.Database.Localizer.Translate("GeneralErrorMessage");
            generalErrorMessage = generalErrorMessage == "GeneralErrorMessage" ? Maps.Instance.GetMap().Database.GeneralErrorMessage : generalErrorMessage;
            generalErrorMessage = "<div class='general-error-message'>" + generalErrorMessage + "</div>";
            string userRole = Map.Database.GetUserRole();
            if (userRole == "Admin" || userRole == "Developer")
            {
                generalErrorMessage += GetAdditionalInfo(exception.Message + "<br><br>" + exception.StackTrace);
            }

            return generalErrorMessage; //"Temporary busy server. Please try again or contact the system administrator.";
        }

        protected virtual string GetAdditionalInfo(string message)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string additionalInfo = Map.Database.Localizer.Translate("Additional Information:");
            sb.Append("<span class='additional-info-icon'></span>");
            sb.Append("<a href='JavaScript:void(0);' class='additional-info-link' >" + additionalInfo + "</a>");
            sb.Append("<div class='additional-info'>");
            sb.Append(message);
            sb.Append("div");

            return sb.ToString();
        }

        protected virtual Field[] FormatUniqueIndexFields(string viewName, string indexName)
        {
            SqlAccess sqlAccess = new SqlAccess();
            string tableName = null;
            View view = GetView(viewName);
            string[] columns = sqlAccess.GetIndexInfo(indexName, Map.Database.ConnectionString, out tableName);
            List<Field> fields = new List<Field>();

            if (columns != null && tableName != null)
            {
                foreach (string column in columns)
                {
                    Field field = view.GetFieldByColumnNames(column);
                    if (field == null)
                        return null;
                    fields.Add(field);
                }
                return fields.ToArray();
            }
            else
            {
                return null;
            }


        }

        protected virtual DataRow CreateRow(string viewName, FormCollection collection, string insertAbovePK)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "CreateRow");

            if (!view.IsCreatable() && !view.IsDuplicatable())
                throw new AccessViolationException();

            return view.CreateRow(collection, insertAbovePK, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);

        }

        public int AddBookmark(int LinkType, string viewName, string guid, string Url, string Name, string Desc)
        {
            Durados.Web.Mvc.View LinkView = GetView("durados_Link", "CreateLink");

            if (!LinkView.IsCreatable()) return -1;

            bool isPublic = Map.Database.GetUserRole() == "Admin" || Map.Database.GetUserRole() == "Developer";

            int UserId = Convert.ToInt32(Map.Database.GetUserID());

            Durados.Web.Mvc.View view = GetView(viewName, "CreateBookmark");

            Desc = GetFilterText(new FormCollection(ViewHelper.GetSessionState(viewName + "Filter")), view);


            //Durados.Web.Mvc.UI.ViewViewer viewViewer = new Durados.Web.Mvc.UI.ViewViewer();
            //FormCollection filter = new FormCollection(ViewHelper.GetSessionState(viewName + "Filter"));

            string filter = new UI.Json.NameValueCollectionSerializer(ViewHelper.GetSessionState(viewName + "Filter")).ToString();

            //if (Url == string.Empty)
            //{
            //   Durados.Web.Mvc.UI.ViewViewer viewViewer = new Durados.Web.Mvc.UI.ViewViewer();
            //   Url = viewViewer.GetRefreshUrl(view);
            //}

            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add("Filter", filter);
            //values.Add("Search", ViewHelper.GetSessionString(viewName + "Search"));
            values.Add("SortColumn", SortHelper.GetSortColumn(view));
            values.Add("SortDirection", SortHelper.GetSortDirection(view));
            values.Add("PageNo", PagerHelper.GetCurrentPage(view, guid));
            values.Add("PageSize", PagerHelper.GetPageSize(view, guid));
            values.Add("Url", Url);
            values.Add("ViewName", viewName);
            values.Add("ControllerName", GetCurrentControllerName());
            values.Add("Guid", guid);
            values.Add("LinkType", LinkType);
            values.Add("Name", Name);
            values.Add("UserId", UserId);
            values.Add("Ordinal", 1);
            values.Add("Description", Desc);
            values.Add("FolderId", null);

            //UI.Json.View jsonView = viewViewer.GetJsonView(LinkView, DataAction.Create, guid);

            DataRow NewBookmark = LinkView.Create(values, null, view_BeforeCreateBookmark, view_BeforeCreateBookmarkInDatabase, view_AfterCreateBeforeCommitBookmark, view_AfterCreateAfterCommitBookmark);

            string bmUrl = ViewHelper.GetAppBasePath(this.ControllerContext.RequestContext) + "Durados/LoadBookmark/?id=" + NewBookmark["Id"].ToString();

            Dictionary<string, object> EditValues = new Dictionary<string, object>();
            EditValues.Add("Url", bmUrl);
            LinkView.Edit(EditValues, NewBookmark["Id"].ToString(), null, null, null, null);

            int NewBookMarkId = Convert.ToInt32(NewBookmark["Id"]);

            //Add Bookmark To Cache
            Bookmark bmNewBookmark = new Bookmark(NewBookMarkId, LinkType, Name, 1);
            bmNewBookmark.ViewName = BookmarksHelper.StringOrDefault(values["ViewName"]);
            bmNewBookmark.ControllerName = BookmarksHelper.StringOrDefault(values["ControllerName"]);
            bmNewBookmark.Guid = BookmarksHelper.StringOrDefault(values["Guid"]);
            bmNewBookmark.SortColumn = BookmarksHelper.StringOrDefault(values["SortColumn"]);
            bmNewBookmark.SortDirection = BookmarksHelper.StringOrDefault(values["SortDirection"]);
            bmNewBookmark.Url = BookmarksHelper.StringOrDefault(values["Url"]);
            bmNewBookmark.Filter = BookmarksHelper.StringOrDefault(values["Filter"]);
            bmNewBookmark.PageNo = BookmarksHelper.IntOrDefault(values["PageNo"]);
            bmNewBookmark.PageSize = BookmarksHelper.IntOrDefault(values["PageSize"]);
            bmNewBookmark.Id = NewBookMarkId;
            bmNewBookmark.Type = LinkType;
            bmNewBookmark.Ordinal = 1;
            bmNewBookmark.Description = BookmarksHelper.StringOrDefault(Desc);

            BookmarksHelper.AddBookmarkToCache(bmNewBookmark);

            return NewBookMarkId;

            if (isPublic)
            {
                Map.ResetPublicBoolmarks();
            }
            else
            {
                BookmarksHelper.AddBookmarkToCache(bmNewBookmark);
            }
        }

        public string GetCurrentControllerName()
        {
            return this.ControllerContext.Controller.GetType().Name.Replace("Controller", "");
        }

        public JsonResult CreateBookmark(string viewName, string guid, string Url, string Name, string Desc)
        {
            int bmID = AddBookmark(0, viewName, guid, Url, Name, Desc);

            return Json(bmID);
        }

        public JsonResult EditBookmark(int Id, string NewName, string NewDesc)
        {

            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add("Name", NewName);

            //Edit Cached Bookmark
            Bookmark bm = BookmarksHelper.GetBookmark(Id);
            bm.Name = NewName;


            //Description
            if (!string.IsNullOrEmpty(NewDesc))
            {
                values.Add("Description", NewDesc);
                bm.Description = NewDesc;
            }

            //Edit db
            Durados.Web.Mvc.View LinkView = GetView("Link", "EditLink");

            LinkView.Edit(values, Id.ToString(), view_BeforeEditBookmark, view_BeforeEditBookmarkInDatabase, view_AfterEditBeforeCommitBookmark, view_AfterEditAfterCommitBookmark);

            return Json(Id);
        }



        public JsonResult RemoveBookmark(int Id)
        {
            string bmID = Id.ToString();

            //Remove from db
            Durados.Web.Mvc.View LinkView = GetView("Link", "DeleteLink");

            LinkView.Delete(bmID, view_BeforeDeleteBookmark, view_AfterDeleteBeforeCommitBookmark, view_AfterDeleteAfterCommitBookmark);

            //Remove Bookmark From Cache
            BookmarksHelper.RemoveBookmarkFromoCache(Id);

            return Json(bmID);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult LoadBookmark(int Id)
        {

            int UserId = Convert.ToInt32(Map.Database.GetUserID());

            Bookmark bookmark = BookmarksHelper.GetBookmark(Id);

            if (bookmark == null)
            {
                throw new HttpException(404, "Bookmark not found!");
            }

            Durados.Web.Mvc.View view = GetView(bookmark.ViewName, "LoadLink");


            //FormCollection collection = new FormCollection();

            //collection["jsonFilter"] = bookmark.Filter;
            //collection["guid"] = bookmark.Guid;
            //collection["viewName"] = bookmark.ViewName;

            ViewHelper.SetSessionState(bookmark.ViewName + "Filter", bookmark.Filter);

            //Map.Session[bookmark.Guid] = bookmark.Filter;

            if (!string.IsNullOrEmpty(bookmark.SortColumn))
            {
                SortHelper.SetSortColumn(view, bookmark.SortColumn);
            }
            else if (!string.IsNullOrEmpty(view.DefaultSort))
            {
                string[] defaultSort = view.GetDefaultSortColumnsAndOrder();
                string defaultSortColumnAndOrder = defaultSort[0];
                SortHelper.SetSortColumn(view, view.GetDefaultSortColumn(defaultSortColumnAndOrder));
                SortHelper.SetSortDirection(view, view.GetDefaultSortColumnOrder(defaultSortColumnAndOrder));

            }

            if (!string.IsNullOrEmpty(bookmark.SortDirection))
                SortHelper.SetSortDirection(view, bookmark.SortDirection);

            PagerHelper.SetCurrentPage(view, bookmark.PageNo, bookmark.Guid);

            if (bookmark.PageSize > 0)
                PagerHelper.SetPageSize(view, bookmark.PageSize, bookmark.Guid);

            if (!string.IsNullOrEmpty(bookmark.ControllerName))
            {
                return RedirectToAction("IndexPage", bookmark.ControllerName, new { viewName = bookmark.ViewName, page = bookmark.PageNo, SortColumn = bookmark.SortColumn, direction = bookmark.SortDirection, pk = "", guid = bookmark.Guid, mainPage = true, safety = false, path = "", from = "bookmark" });
            }

            return RedirectToAction("IndexPage", new { viewName = bookmark.ViewName, page = bookmark.PageNo, SortColumn = bookmark.SortColumn, direction = bookmark.SortDirection, pk = "", guid = bookmark.Guid, mainPage = true, safety = false, path = "", from = "bookmark" });


            //Session["from"] = "BookmarkLoader";
            //return Redirect(bookmark.Url);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult BookmarksManager(string guid)
        {
            Durados.Web.Mvc.View LinkView = GetView("durados_Link", "ManageLinks");

            bool isPublic = Map.Database.GetUserRole() == "Admin" || Map.Database.GetUserRole() == "Developer";

            int UserId = Convert.ToInt32(Map.Database.GetUserID());

            UI.Json.Filter jsonFilter = new UI.Json.Filter();

            jsonFilter.Fields.Add("LinkType", new UI.Json.Field() { Name = "LinkType", Value = 0, Type = "Text", Searchable = false, Permanent = true });

            jsonFilter.Fields.Add("UserId", new UI.Json.Field() { Name = "UserId", Value = UserId, Type = "Text", Searchable = false, Permanent = true });

            //string jsonFilter = LinkView.GetJsonFilter(guid);

            FormCollection collection = new FormCollection();
            collection["jsonFilter"] = UI.Json.JsonSerializer.Serialize<UI.Json.Filter>(jsonFilter);
            collection["guid"] = guid;
            collection["viewName"] = "durados_Link";

            ViewHelper.SetSessionState(guid + "Filter", collection);
            ViewHelper.SetSessionState("durados_Link" + "Filter", collection);

            ViewData["SortColumn"] = "Ordinal";
            ViewData["direction"] = "Asc";

            return RedirectToAction("Index", new { viewName = "durados_Link", guid = guid, firstTime = true });
        }

        public ActionResult InlineAddingDialog(string viewName, string guid)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "InlineAddingDialog");
            Durados.Web.Mvc.UI.DataActionFields inlineAddingDialog = new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Create), DataAction = Durados.DataAction.InlineAdding, Guid = guid, View = view };
            ViewData["clientValidation"] = true;
            return PartialView("~/Views/Shared/Controls/DataRowViewTabs.ascx", inlineAddingDialog);
        }

        public ActionResult InlineEditingDialog(string viewName, string guid)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "InlineEditingDialog");
            Durados.Web.Mvc.UI.DataActionFields inlineEditingDialog = new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(Durados.DataAction.Edit), DataAction = Durados.DataAction.InlineEditing, Guid = guid, View = view };
            ViewData["clientValidation"] = true;
            return PartialView("~/Views/Shared/Controls/DataRowView.ascx", inlineEditingDialog);
        }

        protected void view_BeforeCreate(object sender, CreateEventArgs e)
        {
            try
            {
                BeforeCreate(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected void view_BeforeCreateInDatabase(object sender, CreateEventArgs e)
        {
            try
            {
                BeforeCreateInDatabase(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void BeforeCreateInDatabase(CreateEventArgs e)
        {
        }

        protected void view_BeforeCreateBookmark(object sender, CreateEventArgs e)
        {
            try
            {
                int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
                string currentUserRole = Map.Database.GetUserRole();

                if (e.View.SaveHistory)
                {
                    e.History = GetNewHistory();
                    e.UserId = currentUserId;
                }
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected void view_BeforeCreateBookmarkInDatabase(object sender, CreateEventArgs e)
        {
        }

        protected void view_BeforeCreateForImport(object sender, CreateEventArgs e)
        {
            try
            {
                BeforeCreate(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void BeforeCreate(CreateEventArgs e)
        {

            LoadCreationSignature(e.View, e.Values);
            LoadModificationSignature(e.View, e.Values);
            HandleSpecialDefaults((Durados.Web.Mvc.View)e.View, e.Values);

            int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
            string currentUserRole = Map.Database.GetUserRole();

            if (e.View.SaveHistory)
            {
                e.History = GetNewHistory();
                e.UserId = currentUserId;
            }

            CreateWorkflowEngine().PerformActions(this, e.View, TriggerDataAction.BeforeCreate, e.Values, e.PrimaryKey, null, Map.Database.ConnectionString, currentUserId, currentUserRole, e.Command, e.SysCommand);
        }




        private void HandleSpecialDefaults(Durados.Web.Mvc.View view, Dictionary<string, object> values)
        {
            HandleSpecialDefaults(view, values, false);
        }

        private void HandleSpecialDefaults(Durados.Web.Mvc.View view, Dictionary<string, object> values, bool import)
        {
            HandleCurrentUserDefault(view, values, import);

            if (import)
            {
                HandleCreationDate(view, values);
            }
            //LoadCreationSignature(view, values);
            //LoadModificationSignature(view, values);
        }

        private void HandleCurrentUserDefault(View view, Dictionary<string, object> values)
        {
            HandleCurrentUserDefault(view, values, false);

            string userViewName = ((Database)Database).UserViewName;
            string usernameFieldName = ((Database)Database).UsernameFieldName;

            if (view.Name == userViewName)
            {
                string currentRole = "User";
                if (values.ContainsKey(usernameFieldName) && values[usernameFieldName] != null)
                {
                    string newUser = values[usernameFieldName].ToString();
                    HandleMultiTenancyUser(currentRole, newUser);
                }
            }
        }

        private void HandleCurrentUserDefault(View view, Dictionary<string, object> values, bool import)
        {
            var fields = view.Fields.Values.Where(f => f.FieldType == FieldType.Parent && f.DefaultValue != null && f.DefaultValue.ToString().ToLower() == Durados.Web.Mvc.Database.UserPlaceHolder.ToLower());
            foreach (ParentField field in fields)
            {
                if (values.ContainsKey(field.Name))
                {
                    if ((values[field.Name] == null || values[field.Name].ToString() == string.Empty || values[field.Name].ToString().ToLower() == Durados.Web.Mvc.Database.UserPlaceHolder.ToLower()))
                    {
                        values[field.Name] = GetUserID();
                    }
                }
                else
                {
                    //if (field.IsExcludedInInsert() || import)
                    //{
                    //    values.Add(field.Name, GetUserID());
                    //}
                    values.Add(field.Name, GetUserID());

                }
            }

        }

        private void HandleCreationDate(Durados.View view, Dictionary<string, object> values)
        {

            if (view.CreateDate != null)
            {
                Field createDate = view.CreateDate;

                if (!values.ContainsKey(createDate.Name))
                {
                    values.Add(createDate.Name, DateTime.Now);
                }
                else
                {
                    values[createDate.Name] = DateTime.Now;
                }
            }

        }



        private void LoadCreationSignature(Durados.View view, Dictionary<string, object> values)
        {
            Field createDate = view.CreateDate;
            Field createdBy = view.CreatedBy;

            LoadSignature(view, values, createDate, createdBy);

        }

        private void LoadSignature(Durados.View view, Dictionary<string, object> values, Field dateField, Field userField)
        {
            if (dateField != null)
            {
                dateField.ExcludeInInsert = false;
                dateField.ExcludeInUpdate = false;
                if (!values.ContainsKey(dateField.Name))
                {
                    values.Add(dateField.Name, DateTime.Now);
                }
                else
                {
                    values[dateField.Name] = DateTime.Now;
                }
            }

            if (userField != null)
            {
                userField.ExcludeInInsert = false;
                userField.ExcludeInUpdate = false;
                if (!values.ContainsKey(userField.Name))
                {
                    values.Add(userField.Name, Map.Database.GetUserID());
                }
                else
                {
                    values[userField.Name] = Map.Database.GetUserID();
                }
            }
        }


        virtual protected void view_AfterCreateBeforeCommit(object sender, CreateEventArgs e)
        {
            try
            {
                AfterCreateBeforeCommit(e);

                HandleUploadsSpecialPaths(e.View, DataAction.Create, e.Values, e.PrimaryKey, (DataActionEventArgs)e);

                if (Maps.MultiTenancy)
                {
                    HandleMultiTenancyUser(e);
                }
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                if (!Database.IdenticalSystemConnection && e.SysCommand != null && e.SysCommand.Transaction != null)
                    e.SysCommand.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void HandleMultiTenancyUser(DeleteEventArgs e)
        {
            string usernameFieldName = ((Database)Database).UsernameFieldName;
            string userViewName = ((Database)Database).UserViewName;

            if (e.View.Name == userViewName && e.PrevRow != null)
            {
                string user = e.PrevRow[usernameFieldName].ToString();
                string appName = Maps.GetCurrentAppName();
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("user", user);
                parameters.Add("appName", appName);
                SqlAccess sql = new SqlAccess();
                sql.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, "durados_DeleteUserApp @User, @appName", parameters, SendNewAppAsignment);


            }
        }

        protected virtual void HandleMultiTenancyUser(EditEventArgs e)
        {
            string usernameFieldName = ((Database)Database).UsernameFieldName;
            string userViewName = ((Database)Database).UserViewName;
            string roleDbName = "Role";

            if (e.View.Name == userViewName)
            {
                string roleFieldName = ((Database)Database).GetUserView().GetFieldByColumnNames(roleDbName).Name;
                if (e.Values.ContainsKey(roleFieldName) && e.PrevRow.Table.Columns.Contains(roleDbName))
                {
                    string currentRole = e.Values[roleFieldName].ToString();
                    string prevRole = e.PrevRow[roleDbName].ToString();

                    if (!currentRole.Equals(prevRole))
                    {
                        string newRole = "User";
                        if (currentRole == "Developer" || currentRole == "Admin")
                            newRole = "Admin";
                        string user = e.PrevRow[usernameFieldName].ToString();
                        string appName = Maps.GetCurrentAppName();
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters.Add("user", user);
                        parameters.Add("appName", appName);
                        parameters.Add("newRole", newRole);
                        SqlAccess sql = new SqlAccess();
                        sql.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, "durados_UserAppRoleUpdate @User, @appName, @newRole", parameters, SendNewAppAsignment);

                    }
                }
            }
        }

        protected virtual void HandleMultiTenancyUser(CreateEventArgs e)
        {
            string userViewName = ((Database)Database).UserViewName;
            string usernameFieldName = ((Database)Database).UsernameFieldName;

            if (e.View.Name == userViewName)
            {
                string roleDbName = "Role";
                string roleFieldName = ((Database)Database).GetUserView().GetFieldByColumnNames(roleDbName).Name;

                string currentRole = "User";
                if (e.Values.ContainsKey(roleFieldName) && !string.IsNullOrEmpty(e.Values[roleFieldName].ToString()))
                    currentRole = e.Values[roleFieldName].ToString();
                string newUser = e.Values[usernameFieldName].ToString();

                HandleMultiTenancyUser(currentRole, newUser);
            }
        }

        protected virtual void HandleMultiTenancyUser(string currentRole, string newUser)
        {
            //string userViewName = ((Database)Database).UserViewName;
            //string usernameFieldName = ((Database)Database).UsernameFieldName;
            //string roleDbName = "Role";

            //if (e.View.Name == userViewName)
            //{
            //    string roleFieldName = ((Database)Database).GetUserView().GetFieldByColumnNames(roleDbName).Name;
            //    string currentRole = "User";
            //    if (e.Values.ContainsKey(roleFieldName) && !string.IsNullOrEmpty(e.Values[roleFieldName].ToString()))
            //        currentRole = e.Values[roleFieldName].ToString();
            //    string newUser = e.Values[usernameFieldName].ToString();
            string appName = Maps.GetCurrentAppName();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("newUser", newUser);
            parameters.Add("appName", appName);
            parameters.Add("role", currentRole);
            SqlAccess sql = new SqlAccess();
            sql.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, "durados_NewAppAsignment @newUser, @appName, @role", parameters, SendNewAppAsignment);
            //Maps.Instance.DuradosMap.Database.Views["durados_UserApp"].Create(values, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);
            //}
        }

        protected virtual string SendNewAppAsignment(Dictionary<string, object> parameters)
        {
            return "success";
        }

        protected void view_AfterCreateBeforeCommitBookmark(object sender, CreateEventArgs e)
        {
            try
            {
                //AfterCreateBeforeCommit(e);

                HandleUploadsSpecialPaths(e.View, DataAction.Create, e.Values, e.PrimaryKey, (DataActionEventArgs)e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                if (!Database.IdenticalSystemConnection && e.SysCommand != null && e.SysCommand.Transaction != null)
                    e.SysCommand.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected void view_AfterCreateBeforeCommitForImport(object sender, CreateEventArgs e)
        {
            try
            {
                AfterCreateBeforeCommit(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                if (!Database.IdenticalSystemConnection && e.SysCommand != null && e.SysCommand.Transaction != null)
                    e.SysCommand.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void AfterCreateBeforeCommit(CreateEventArgs e)
        {
            //Workflow.Engine wfe = CreateWorkflowEngine();

            wfe.PerformActions(this, e.View, TriggerDataAction.AfterCreateBeforeCommit, e.Values, e.PrimaryKey, null, Map.Database.ConnectionString, Convert.ToInt32(((Database)e.View.Database).GetUserID()), ((Database)e.View.Database).GetUserRole(), e.Command, e.SysCommand);


        }

        public virtual void LoadValues(Dictionary<string, object> values, DataRow dataRow, Durados.View view, Durados.ParentField parentField, Durados.View rootView, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {

        }

        protected virtual void HandleUploadsSpecialPaths(Durados.View view, DataAction dataAction, Dictionary<string, object> values, string pk, DataActionEventArgs e)
        {
            bool valuesLoaded = true;

            if (dataAction == DataAction.Edit) valuesLoaded = false;

            foreach (Field field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column && (((ColumnField)f).Upload != null && !string.IsNullOrEmpty(((ColumnField)f).Upload.TemplatePath) || ((ColumnField)f).FtpUpload != null && !string.IsNullOrEmpty(((ColumnField)f).FtpUpload.TemplatePath)) && !f.IsExcluded(dataAction) && f.IsDerivationEditable(values)))
            {

                if (!valuesLoaded)
                {
                    LoadValues(view, values, e);
                    valuesLoaded = true;
                }

                if (!values.ContainsKey(field.Name) || values[field.Name] == null) continue;

                string fileName = values[field.Name].ToString().Trim();

                if (fileName == string.Empty) continue;

                if (dataAction == DataAction.Edit)
                {
                    if (fileName == field.ConvertToString(((EditEventArgs)e).PrevRow)) continue;
                }

                Durados.Web.Mvc.ColumnField cField = (ColumnField)field;

                IUpload upload = UploadFactory.GetUpload(cField);

                string oldPath;

                if (upload != null) oldPath = upload.GetBaseUploadPath(fileName);//cField.GetUploadPath();

                else throw new DuradosException("Upload is not recognized.");
                // else uploadPath = cField.FtpUpload.GetFtpBasePath(fileName);

                //string oldPath;
                //string oldPath = uploadBasePath + fileName;
                //string oldPath = upload.GetBaseUploadPath(fileName);

                if (!upload.IsFileExists(oldPath)) continue;


                string template = upload.TemplatePath;

                string newPath = string.Empty;

                string newDirPath = CreateTemplatePath(values, pk, fileName, ref template, ref newPath);

                upload.CreateNewDirectory(newDirPath);

                string newPhisicalPath = newPath.Replace("/", "\\");

                upload.DeleteOldFile(newPhisicalPath);

                upload.CreateNewDirectory2(newPhisicalPath);

                MoveUploadedFile(upload, oldPath, newPhisicalPath);

                Dictionary<string, object> newFileNameValues = new Dictionary<string, object>();

                newFileNameValues.Add(cField.Name, newPath);

                SqlAccess sa = new SqlAccess();

                sa.Edit(view, newFileNameValues, pk, null, null, null, null, (System.Data.SqlClient.SqlCommand)e.Command, (System.Data.SqlClient.SqlCommand)e.SysCommand, null, null);
            }
        }

        //private static void CreateNewDirectory2(string newPhisicalPath)
        //{
        //    System.IO.FileInfo fileInfo = new System.IO.FileInfo(newPhisicalPath);
        //    fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.
        //}

        //private static void DeleteOldFile(string newPhisicalPath)
        //{
        //    if (System.IO.File.Exists(newPhisicalPath))
        //    {
        //        System.IO.File.Delete(newPhisicalPath);
        //    }
        //}

        //private static void CreateNewDirectory(string newDirPath)
        //{
        //    Directory.CreateDirectory(newDirPath);
        //}

        public static string CreateTemplatePath(Dictionary<string, object> values, string pk, string fileName, ref string template, ref string newPath)
        {
            if (template.Contains("["))
            {
                if (template.Contains("[PK]"))
                {
                    template = template.Replace("[PK]", pk);
                }

                foreach (string k in values.Keys)
                {
                    if (template.Contains("[" + k + "]"))
                    {
                        template = template.Replace("[" + k + "]", values[k].ToString());
                    }
                }
                template = template.Replace(" ", "-");

                template = MakeValidPath(template);
            }

            //template = template.Replace("/", "\\");

            string newDirPath = template.Replace("/", "\\");

            if (!newDirPath.EndsWith(@"\") && newDirPath.Contains(@"\"))
            {
                newDirPath = newDirPath.Substring(0, newDirPath.LastIndexOf(@"\"));
            }


            if (template.EndsWith("."))
            {
                newPath = template.TrimEnd('.') + System.IO.Path.GetExtension(fileName);
            }
            else
            {
                newPath = (template + "/" + MakeValidFileName(fileName)).Replace("\\", "/").Replace("//", "/");
            }
            return newDirPath;
        }

        public static void LoadValues(Durados.View view, Dictionary<string, object> values, DataActionEventArgs e)
        {

            foreach (Field valueField in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
            {
                if (!values.ContainsKey(valueField.Name))
                {
                    string value = valueField.ConvertToString(((EditEventArgs)e).PrevRow);

                    values.Add(valueField.Name, value);
                }
            }
        }

        protected virtual void MoveUploadedFile(IUpload upload, string oldPath, string newPhisicalPath)
        {
            upload.MoveUploadedFile(oldPath, newPhisicalPath);
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format(@"[{0}]+", invalidChars);
            return System.Text.RegularExpressions.Regex.Replace(name, invalidReStr, "_");
        }
        private static string MakeValidPath(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidPathChars()));
            string invalidReStr = string.Format(@"[{0}]+", invalidChars);
            return System.Text.RegularExpressions.Regex.Replace(name, invalidReStr, "_");
        }

        protected void view_AfterCreateAfterCommitBookmark(object sender, CreateEventArgs e)
        {
            //AfterCreateAfterCommit(e);
        }

        protected void view_AfterCreateAfterCommit(object sender, CreateEventArgs e)
        {
            AfterCreateAfterCommit(e);
        }

        protected void view_AfterCreateAfterCommitForImport(object sender, CreateEventArgs e)
        {
            AfterCreateAfterCommit(e);
        }

        protected virtual void AfterCreateAfterCommit(CreateEventArgs e)
        {
            //Workflow.Engine wfe = CreateWorkflowEngine();
            wfe.PerformActions(this, e.View, TriggerDataAction.AfterCreate, e.Values, e.PrimaryKey, null, Map.Database.ConnectionString, Convert.ToInt32(((Database)e.View.Database).GetUserID()), ((Database)e.View.Database).GetUserRole(), e.Command, e.SysCommand);

            HandleApi(e.View);
        }

        private void HandleApi(Durados.View view)
        {
            if (view == Database.GetUserView() || view == ((Database)Database).GetRoleView())
            {
                Map.RefreshApis(Map);
            }
        }

        private OldNewValue[] GetOldNewValues(View view, DataRow row, string pk)
        {
            History history = new History();

            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (Field field in view.Fields.Values)
            {
                values.Add(field.Name, string.Empty);
            }

            OldNewValue[] oldNewValues = history.GetChanges(view, row, values, pk);

            List<OldNewValue> oldNewValuesList = new List<OldNewValue>();

            foreach (OldNewValue oldNewValue in oldNewValues)
            {
                oldNewValuesList.Add(new OldNewValue() { FieldName = oldNewValue.FieldName, NewKey = oldNewValue.OldKey, NewValue = oldNewValue.OldValue, OldKey = oldNewValue.NewKey, OldValue = oldNewValue.NewValue });

            }

            return oldNewValuesList.ToArray();
        }



        public virtual ActionResult EditForm(string viewName, string pk)
        {
            try
            {
                ViewData["PK"] = pk;

                if (Request.IsAjaxRequest())
                {
                    Durados.Web.Mvc.View view = GetView(viewName, "EditForm");
                    Durados.Web.Mvc.UI.DataActionFields fields = new Durados.Web.Mvc.UI.DataActionFields() { Fields = view.GetVisibleFieldsForRow(DataAction.Edit), DataAction = DataAction.Edit, View = view };
                    return PartialView("~/Views/Shared/Controls/Custom/Forms/DataRowView.ascx", fields);
                }
                else
                {
                    Durados.Web.Mvc.View view = GetView(viewName, "EditForm");

                    ViewData["pageName"] = view.DisplayName;

                    return View("Edit", (object)viewName);
                }
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
        }

        public virtual void EditOnlyAdditionalSpecificProcess(Durados.Web.Mvc.View view, string pk)
        {

        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult EditOnly(string viewName, FormCollection collection, string pk, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "EditOnly");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                view.EditRow(collection, pk, false, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                EditOnlyAdditionalSpecificProcess(view, pk);

                return Json("success");
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("$$error$$" + FormatExceptionMessage(viewName, "EditOnly", exception));
            }
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult EditValue(string viewName, string fieldName, string value, string pk, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "EditValue");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                if (!view.Fields.ContainsKey(fieldName))
                    throw new DuradosException("The field " + fieldName + " does not exist in view " + viewName);

                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add(fieldName, value);
                view.Edit(values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                return Json("success");
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("$$error$$" + FormatExceptionMessage(viewName, "EditValue", exception));
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult SaveCustomView(string viewName, string state, string user)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "CustomView");

                if (view == null || state == null)
                {
                    return Content("Error");
                }

                int UserId = -1; //Default

                if (user != "default")
                {
                    UserId = Convert.ToInt32(GetUserID());
                }

                string sp = "durados_UpdateUserCustomView";

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("UserId", UserId);
                parameters.Add("ViewName", viewName);
                parameters.Add("CustomView", state);

                SqlAccess da = new SqlAccess();

                da.ExecuteNoneQueryStoredProcedure(Map.Database.SysDbConnectionString, sp, parameters);

                //when admin save also save the default
                if (UserId != -1 && view.IsAdmin())
                    SaveCustomView(viewName, state, "default");

                Map.RefreshApis(Map);

                return Content("Success");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Content("Operation Faild: " + exception.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult RestoreCustomView(string viewName)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "CustomView");

                string state = UI.Helpers.ViewHelper.GetCustomView(viewName, true);

                string sp = "durados_UpdateUserCustomView";

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("UserId", Convert.ToInt32(GetUserID()));
                parameters.Add("ViewName", viewName);
                parameters.Add("CustomView", state);

                SqlAccess da = new SqlAccess();

                da.ExecuteNoneQueryStoredProcedure(Map.Database.ConnectionString, sp, parameters);

                Map.RefreshApis(Map);

                return Content("Success");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Content("Operation Faild: " + exception.Message);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult SaveWorkFlowGraphState(string viewName, string state)
        {
            try
            {
                if (!Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") && !Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin"))
                {
                    throw new DuradosException("User do not allow edit graph state!");
                }

                Durados.Web.Mvc.View view = GetView(viewName, "Step");

                if (view == null || state == null)
                {
                    return Content("Error");
                }

                string sp = "durados_UpdateWorkflowGraphState";

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("ViewName", viewName);
                parameters.Add("State", state);

                SqlAccess da = new SqlAccess();

                da.ExecuteNoneQueryStoredProcedure(view.Database.ConnectionString, sp, parameters);

                return Content("Success");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Content("Operation Faild: " + exception.Message);
            }
        }

        public virtual ActionResult WorkFlowGraph(string viewName)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "Step");

                if (view == null)
                {
                    throw new DuradosException("No data in view");
                }

                string sql = "SELECT TOP 1 GraphState FROM [dbo].[durados_WF_Info] WHERE [ViewName] = @ViewName";

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("ViewName", viewName);

                SqlAccess da = new SqlAccess();

                ViewData["GraphState"] = da.ExecuteScalar(view.Database.SystemConnectionString, sql, parameters);

                return View(new Durados.Workflow.Graph(view, this, Durados.WorkflowAction.CompleteStep));
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                ViewData["GraphState"] = "";
                return View(new Durados.Workflow.Graph());
            }
        }

        public virtual ActionResult Step(string viewName, string pk, string guid, bool? noPrevDialog)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "Step");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                if (!noPrevDialog.HasValue)
                    noPrevDialog = false;

                if (wfe.StepResult == null)
                {
                    if (noPrevDialog.Value)
                    {
                        DataRow prevRow = view.GetDataRow(pk);

                        Dictionary<string, object> values = new Dictionary<string, object>();
                        object stepValue = view.WorkFlowStepsField.GetValue(prevRow);
                        values.Add(view.WorkFlowStepsFieldName, stepValue);

                        wfe.PerformActions(this, view, TriggerDataAction.AfterCreateOrEdit, values, pk, prevRow, view.Database.ConnectionString, Convert.ToInt32(((Database)view.Database).GetUserID()), ((Database)view.Database).GetUserRole(), null, null);
                    }
                    else
                    {
                        return Json(new Durados.Workflow.Step.Result() { Message = "No result made for this current step" });
                    }
                }

                if (wfe.StepResult == null)
                    throw new Durados.Workflow.WorkflowEngineException("Please check workflow rules, complete step must be configured to AfterCreateOrEdit");

                var enabledSteps = wfe.StepResult.EnabledSteps.Where(es => es.Enable);
                if (enabledSteps.Count() == 1 && wfe.StepResult.MessageOnly)
                {
                    Durados.Workflow.Step.EnabledStep enabledStep = enabledSteps.First();
                    Dictionary<string, object> values = new Dictionary<string, object>();
                    values.Add(view.WorkFlowStepsFieldName, enabledStep.Id);
                    //view.Edit(values, pk, null, null, null);
                    view.Edit(values, pk, BeforeCompleteStep, BeforeCompleteStepInDatabase, AfterCompleteStepBeforeCommit, AfterCompleteStepAfterCommit);
                    enabledStep.Enable = false;
                    return Json(wfe.StepResult);
                }
                else
                {
                    return Json(wfe.StepResult);
                }
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));
            }
        }


        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult EditStep(string viewName, FormCollection collection, string pk, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "EditOnly");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                view.EditRow(collection, pk, false, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                return Step(viewName, pk, guid, false);

                //return RedirectToAction("Step", view.Controller, new { viewName = viewName, pk = pk, guid = guid, wfe = wfe });

                //if (wfe.StepResult == null)
                //{
                //    return Json(new Durados.Workflow.Step.Result() { Message = "No result made for this current step" });
                //}

                //var enabledSteps = wfe.StepResult.EnabledSteps.Where(es=>es.Enable);
                //if (enabledSteps.Count() == 1)
                //{
                //    Durados.Workflow.Step.EnabledStep enabledStep = enabledSteps.First();
                //    Dictionary<string, object> values = new Dictionary<string, object>();
                //    values.Add(view.WorkFlowStepsFieldName, enabledStep.Id);
                //    view.Edit(values, pk, null, null, null);
                //    enabledStep.Enable = false;
                //    return Json(wfe.StepResult);
                //}
                //else
                //{
                //    return Json(wfe.StepResult);
                //}
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));
            }
        }

        private void BeforeCompleteStep(object sender, EditEventArgs e)
        {
            e.History = GetNewHistory();
            int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
            e.UserId = currentUserId;
            if (StepWasChangedByDifferentUser(e))
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();

                throw new Durados.Workflow.WorkflowEngineException(Map.Database.Localizer.Translate("The step was already changed by another user."));
            }
            try
            {
                BeforeCompleteStep(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        private void BeforeCompleteStepInDatabase(object sender, EditEventArgs e)
        {
            try
            {
                BeforeCompleteStepInDatabase(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        public string GetUrlAction(Durados.View view, string pk)
        {
            return Url.Action(((View)view).IndexAction, ((View)view).Controller, new { viewName = view.Name, pk = pk });

        }

        protected virtual void BeforeCompleteStep(EditEventArgs e)
        {
            LoadModificationSignature(e.View, e.Values);

            wfe.PerformActions(this, e.View, TriggerDataAction.BeforeCompleteStep, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, Convert.ToInt32(((Database)e.View.Database).GetUserID()), ((Database)e.View.Database).GetUserRole(), e.Command, e.SysCommand);

        }

        protected virtual void BeforeCompleteStepInDatabase(EditEventArgs e)
        {

        }

        private void AfterCompleteStepBeforeCommit(object sender, EditEventArgs e)
        {
            try
            {
                AfterCompleteStepBeforeCommit(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                if (!Database.IdenticalSystemConnection && e.SysCommand != null && e.SysCommand.Transaction != null)
                    e.SysCommand.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void AfterCompleteStepBeforeCommit(EditEventArgs e)
        {
            wfe.PerformActions(this, e.View, TriggerDataAction.AfterCompleteStepBeforeCommit, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, Convert.ToInt32(((Database)e.View.Database).GetUserID()), ((Database)e.View.Database).GetUserRole(), e.Command, e.SysCommand);

        }

        private void AfterCompleteStepAfterCommit(object sender, EditEventArgs e)
        {
            AfterCompleteStepAfterCommit(e);
        }

        protected virtual void AfterCompleteStepAfterCommit(EditEventArgs e)
        {
            wfe.PerformActions(this, e.View, TriggerDataAction.AfterCompleteStepAfterCommit, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, Convert.ToInt32(((Database)e.View.Database).GetUserID()), ((Database)e.View.Database).GetUserRole(), e.Command, e.SysCommand);

        }


        private bool StepWasChangedByDifferentUser(EditEventArgs e)
        {
            if (!e.Values.ContainsKey("prevStepId"))
                return false; // new row

            string prevStepId = e.Values["prevStepId"].ToString();
            DataRow row = e.View.GetDataRow(e.PrimaryKey);
            if (row == null)
                return true; // the row was deleted

            return e.View.WorkFlowStepsField.GetValue(row) != prevStepId;
        }



        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult CompleteStep(string viewName, string stepId, string prevStepId, string pk, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "CompleteStep");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                //view.Edit(values, pk, view_BeforeEdit, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add(view.WorkFlowStepsFieldName, stepId);
                values.Add("prevStepId", prevStepId);
                view.Edit(values, pk, BeforeCompleteStep, BeforeCompleteStepInDatabase, AfterCompleteStepBeforeCommit, AfterCompleteStepAfterCommit);

                return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });

            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("$$error$$" + FormatExceptionMessage(viewName, "EditOnly", exception));
            }
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult CopyPaste(string viewName, FormCollection collection, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "CopyPaste");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                view.CopyPaste(collection, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                //Durados.Web.Mvc.Logging.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), null, null, 3, null);
                return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
                //return Json(new Json.JsonResult("Success", true));
            }
            catch (MessageException exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                return PartialView("~/Views/Shared/Controls/Message.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Refresh(string viewName, string guid)
        {
            try
            {
                return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
            }
            catch (MessageException exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                return PartialView("~/Views/Shared/Controls/Message.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
        }



        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Edit(string viewName, FormCollection collection, string pk, string guid)
        {
            try
            {
                if (Database.Logger != null)
                {
                    Database.Logger.Log(viewName, "Start", "Edit", "Controller", "", 11, Map.Logger.NowWithMilliseconds(), DateTime.Now);
                }
                Durados.Web.Mvc.View view = GetView(viewName, "Edit");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                if (pk.EndsWith("#") && view.DataTable.PrimaryKey != null && view.DataTable.PrimaryKey.Length == 1 && view.DataTable.PrimaryKey[0].DataType != typeof(string))
                {
                    pk = pk.TrimEnd('#');
                }

                view.EditRow(collection, pk, false, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                //Durados.Web.Mvc.Logging.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), null, null, 3, null);
                if (Database.Logger != null)
                {
                    Database.Logger.Log(view.Name, "End", "Edit", "Controller", "", 11, Map.Logger.NowWithMilliseconds(), DateTime.Now);
                }

                return RedirectToAction(view.IndexAction, view.Controller, new { viewName = viewName, ajax = true, guid = guid });
                //return Json(new Json.JsonResult("Success", true));
            }
            catch (MessageException exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                return PartialView("~/Views/Shared/Controls/Message.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
        }

        protected virtual void CommitSwitch(Durados.Web.Mvc.View view)
        {

        }

        /// <summary>
        /// Change ordinal of records between o_pk and d_pk- in order to move o_pk to d_pk place
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="o_pk"></param>
        /// <param name="d_pk"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public virtual JsonResult ChangeOrdinal(string viewName, string o_pk, string d_pk, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "ChangeOrdinal");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                view.ChangeOrdinal(o_pk, d_pk, Convert.ToInt32(GetUserID()));

                return Json("success");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(FormatExceptionMessage(viewName, "ChangeOrdinal", exception));
            }
        }

        [ValidateInput(false)]
        public virtual JsonResult Switch(string viewName, string o_pk, string d_pk, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "Switch");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                view.Switch(o_pk, d_pk, Convert.ToInt32(GetUserID()));

                CommitSwitch(view);

                //Durados.Web.Mvc.Logging.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), null, null, 3, null);
                return Json("success");
                //return Json(new Json.JsonResult("Success", true));
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(FormatExceptionMessage(viewName, "Switch", exception));

            }
        }

        protected virtual void view_BeforeEdit(object sender, EditEventArgs e)
        {
            try
            {
                BeforeEdit(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void view_BeforeEditInDatabase(object sender, EditEventArgs e)
        {
            try
            {
                BeforeEditInDatabase(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void BeforeEditInDatabase(EditEventArgs e)
        {
        }

        protected virtual IDbCommand GetCommand(SqlProduct sqlProduct)
        {
            if (sqlProduct == SqlProduct.Oracle)
                return new Oracle.ManagedDataAccess.Client.OracleCommand();
            else if (sqlProduct == SqlProduct.Postgre)
                return new Npgsql.NpgsqlCommand();
            else if (sqlProduct == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlCommand();
            else
                return new System.Data.SqlClient.SqlCommand();
        }

        protected virtual IDbConnection GetConnection(SqlProduct sqlProduct, string connectionString)
        {
            if (sqlProduct == SqlProduct.Oracle)
            {
                return new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString);
            }
            else if (sqlProduct == SqlProduct.Postgre)
            {
                Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(connectionString);
                connection.ValidateRemoteCertificateCallback += new Npgsql.ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
                return connection;
            }
            else if (sqlProduct == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            else
                return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        bool connection_ValidateRemoteCertificateCallback(System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }

        protected virtual void BeforeEdit(EditEventArgs e)
        {
            HandleEncryptedHiddenFields(e);

            LoadModificationSignature(e.View, e.Values);
            //e.LoadPrevRow = e.View.Rules.Count > 0 || e.View.SaveHistory;

            if (IsApprovalProcessUserView(e.View))
            {
                HandleApprovalProcess(e);
            }

            int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
            string currentUserRole = Map.Database.GetUserRole();

            if (e.View.SaveHistory)
            {
                e.History = GetNewHistory();
                e.UserId = currentUserId;
                if (e.View.Database is Durados.Web.Mvc.Config.Database)
                {
                    e.Command = GetCommand(Map.Database.SqlProduct);
                    e.Command.Connection = GetConnection(Map.Database.SqlProduct, Map.Database.ConnectionString);

                    if (Database.IdenticalSystemConnection)
                    {
                        e.SysCommand = e.Command;
                    }
                    else
                    {
                        e.SysCommand = GetCommand(Map.Database.SystemSqlProduct);// new System.Data.SqlClient.SqlCommand();
                        e.SysCommand.Connection = GetConnection(Map.Database.SystemSqlProduct, Map.Database.SystemConnectionString);// new System.Data.SqlClient.SqlConnection();
                    }
                }
            }
            if (e.View.GetRules().Count() > 0)
            {
                CreateWorkflowEngine().PerformActions(this, e.View, TriggerDataAction.BeforeEdit, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, currentUserId, currentUserRole, e.Command, e.SysCommand);
            }
        }

        protected virtual void HandleEncryptedHiddenFields(EditEventArgs e)
        {
            List<string> valuesToRemove = new List<string>();
            foreach (string name in e.Values.Keys)
            {
                if (e.View.Fields.ContainsKey(name) && e.View.Fields[name].FieldType == FieldType.Column)
                {
                    ColumnField columnField = (ColumnField)e.View.Fields[name];
                    if (columnField.Encrypted && columnField.SpecialColumn == SpecialColumn.Password)
                    {
                        if (e.Values[name].ToString().Equals(Map.Database.EncryptedPlaceHolder))
                        {
                            valuesToRemove.Add(name);
                        }
                    }
                }
            }

            foreach (string name in valuesToRemove)
                e.Values.Remove(name);
        }

        protected virtual void HandleApprovalProcess(EditEventArgs e)
        {
            Field signedUserField = ((View)e.View).GetFieldByColumnNames("SignedUserId");
            Field signedDateField = ((View)e.View).GetFieldByColumnNames("SignedDate");

            Dictionary<string, object> values = e.Values;

            if (values.ContainsKey(signedUserField.Name))
            {
                values[signedUserField.Name] = GetUserID();
            }
            else
            {
                values.Add(signedUserField.Name, GetUserID());
            }



            if (values.ContainsKey(signedDateField.Name))
            {
                values[signedDateField.Name] = DateTime.Now;
            }
            else
            {
                values.Add(signedDateField.Name, DateTime.Now);
            }
        }

        protected virtual void view_BeforeEditBookmarkInDatabase(object sender, EditEventArgs e)
        {
        }

        protected virtual void view_BeforeEditBookmark(object sender, EditEventArgs e)
        {
            try
            {
                int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
                if (e.View.SaveHistory)
                {
                    e.History = GetNewHistory();
                    e.UserId = currentUserId;
                    if (e.View.Database is Durados.Web.Mvc.Config.Database)
                    {
                        e.Command = GetCommand(Map.Database.SqlProduct);
                        e.Command.Connection = GetConnection(Map.Database.SqlProduct, Map.Database.ConnectionString);

                        if (Database.IdenticalSystemConnection)
                        {
                            e.SysCommand = e.Command;
                        }
                        else
                        {
                            e.SysCommand = GetCommand(Map.Database.SystemSqlProduct);// new System.Data.SqlClient.SqlCommand();
                            e.SysCommand.Connection = GetConnection(Map.Database.SystemSqlProduct, Map.Database.SystemConnectionString);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                {
                    e.Command.Transaction.Rollback();
                    if (!Database.IdenticalSystemConnection && e.SysCommand != null && e.SysCommand.Transaction != null)
                    {
                        e.SysCommand.Transaction.Rollback();
                    }
                }
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        private void LoadModificationSignature(Durados.View view, Dictionary<string, object> values)
        {
            Field modifiedDate = view.ModifiedDate;
            Field modifiedBy = view.ModifiedBy;

            LoadSignature(view, values, modifiedDate, modifiedBy);


        }

        protected virtual void view_AfterEditBeforeCommit(object sender, EditEventArgs e)
        {
            try
            {
                AfterEditBeforeCommit(e);

                if (Maps.MultiTenancy)
                {
                    HandleMultiTenancyUser(e);
                }
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                if (!Database.IdenticalSystemConnection && e.SysCommand != null && e.SysCommand.Transaction != null)
                    e.SysCommand.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void view_AfterEditBeforeCommitBookmark(object sender, EditEventArgs e)
        {
            try
            {
                //AfterEditBeforeCommit(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                if (!Database.IdenticalSystemConnection && e.SysCommand != null && e.SysCommand.Transaction != null)
                    e.SysCommand.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void AfterEditBeforeCommit(EditEventArgs e)
        {

            //Workflow.Engine wfe = CreateWorkflowEngine();
            wfe.PerformActions(this, e.View, TriggerDataAction.AfterEditBeforeCommit, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, Convert.ToInt32(((Database)e.View.Database).GetUserID()), ((Database)e.View.Database).GetUserRole(), e.Command, e.SysCommand);

            HandleUploadsSpecialPaths(e.View, DataAction.Edit, e.Values, e.PrimaryKey, (DataActionEventArgs)e);

            HandleFieldsFromOtherViews(e);

            CompleteApprovalProcess(e);


        }

        public ActionResult Charts()
        {
            if (!IsAllow() && !Maps.IsSuperDeveloper(null))
                throw new DuradosAccessViolationException();

            if (Request.IsAjaxRequest())
                return PartialView("~/Views/Shared/Controls/Charts.ascx", Map.Database.MyCharts);

            //string url = ChartHelper.GetDashboardUrlWithQueryString(1);
            return RedirectToAction("ChartsDashboard", "Home", Request.QueryString.ToRouteValues(new { id = 1 }));
            //return RedirectToAction(url, "Home");
            //return RedirectToAction("ChartsDashboard", new { id = 1});
        }

        public ActionResult ChartsDashboard(int? id)
        {
            if (!IsAllow() && !Maps.IsSuperDeveloper(null))
                throw new DuradosAccessViolationException();

            if (Request.IsAjaxRequest())
                return PartialView("~/Views/Shared/Controls/Charts.ascx", Map.Database.MyCharts);
            id = id.HasValue ? id : 0;
            if (!Database.Dashboards.ContainsKey(id.Value))
            {
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", "Dashboard not found");

                //throw new HttpException(404, "Dashboard not found!");
                //throw new DuradosException("Dashboard Id was not found");
            }

            ViewData["DashboardId"] = id.Value;
            return View();
        }
        #region Chart

        public ActionResult GetChartData(string chartId, string queryString, bool? isTable)
        {
            ChartHelper helper = new ChartHelper(Map);
            string errorMessage;
            string createMessage;
            string userId;
            string userRole;
            helper.SetErrorMessageValues(out errorMessage, out createMessage, out userId, out userRole);


            if (string.IsNullOrEmpty(chartId))
                return Json(string.Format(createMessage, chartId));

            int id = Convert.ToInt32(chartId.Replace("Chart", ""));

            int? dashboardId = Database.GetDashboardPK(id);

            if (!dashboardId.HasValue)
                return Json(string.Format(createMessage, chartId));

            if (!Map.Database.Dashboards[dashboardId.Value].Charts.ContainsKey(id))
                return Json(string.Format(createMessage, chartId));

            Chart chart = Map.Database.Dashboards[dashboardId.Value].Charts[id];


            if (string.IsNullOrEmpty(chart.SQL))
                return Json(string.Format(createMessage, chartId));

            System.Data.IDbConnection connection = null;
            System.Data.IDataReader reader = null;
            try
            {
                Durados.Web.Mvc.UI.Helpers.tblObject content = helper.GetChartJsonObject(chart);
                // Connect
                System.Data.IDbCommand cmd = helper.BuildConnectionAndCommand(userId, userRole, chart, queryString, ref connection);
                // Read
                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (Exception exception)
                {
                    Map.Database.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "GetChartData", exception, 2, null);
                    return Json(string.Format(errorMessage, "SQL", exception.Message, chartId));
                }

                string xFieldName = chart.XField;
                Dictionary<int, string> yFieldNames = new Dictionary<int, string>();
                //if(!string.IsNullOrEmpty(chart.YField) && chart.ChartType!=ChartType.Gauge)
                //    yFieldNames.AddRange(chart.YField.Split(','));
                content.Neg = false;

                List<Durados.Web.Mvc.UI.Helpers.ChartSeries> series = new List<Durados.Web.Mvc.UI.Helpers.ChartSeries>();
                if (reader.FieldCount >= 2 || chart.ChartType == ChartType.Gauge && reader.FieldCount == 1)
                {
                    if (string.IsNullOrEmpty(xFieldName) || chart.ChartType == ChartType.Gauge)
                        xFieldName = reader.GetName(0);
                    helper.FillChartData(content, reader, xFieldName, yFieldNames, series, chart);
                }
                else
                {

                    string message = chart.ChartType != ChartType.Gauge ? string.Format(errorMessage, "SQL", "The select statement must contain at least two columns.", chartId) : string.Format(errorMessage, "SQL", "The select statement must contain one column.", chartId);
                    Map.Database.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "GetChartData", null, 2, message);
                    return Json(message);
                }

                helper.SetChartProperties(chart, content, xFieldName, yFieldNames, series);

                if ((isTable.HasValue && isTable.Value == true))
                {
                    return PartialView("~/Views/Shared/Controls/ChartTable.ascx", content);//content.xAxis

                }
                if (chart.ChartType == ChartType.Table)
                {

                    return Json(new { table = true, html = "", Type = "Table", ShowTable = true, height = content.Height });

                }
                //System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                //return jss.Serialize(content);           
                return Json(content);
            }
            catch (Exception exception)
            {
                Map.Database.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "GetChartData", exception, 1, null);
                return Json(string.Format(errorMessage, "", exception.Message, chartId));
            }
            finally
            {
                try
                {
                    reader.Close();
                }
                catch { }
                try
                {
                    connection.Close();
                }
                catch { }
            }
        }



        #endregion


        protected virtual void HandleFieldsFromOtherViews(EditEventArgs e)
        {
            SqlAccess sa = new SqlAccess();

            int userId = Convert.ToInt32(GetUserID());

            foreach (Field field in e.View.Fields.Values.Where(f => f.IsFromOtherView()))
            {
                string pk = null;
                object v = null;

                if (e.Values.ContainsKey(field.OriginalParentRelatedFieldName))
                {
                    v = e.Values[field.OriginalParentRelatedFieldName];
                }
                else if (e.View.Fields.ContainsKey(field.OriginalParentRelatedFieldName))
                {
                    string fk = ((ParentField)e.View.Fields[field.OriginalParentRelatedFieldName]).DatabaseNames;

                    if (e.PrevRow != null && e.PrevRow.Table.Columns.Contains(fk))
                        v = e.PrevRow[fk];
                }
                else
                {
                    throw new DuradosException("Configuraion error: Parent Related Fieldname not exist in view");
                }

                if (v != null)
                {
                    pk = v.ToString();
                }

                if (string.IsNullOrEmpty(pk))
                {
                    throw new DuradosException("Foreign key to original view is invalid");
                }

                Dictionary<string, object> values = new Dictionary<string, object>();

                values.Add(field.OriginalFieldName, e.Values[field.Name]);

                Durados.View originalView = ((ParentField)e.View.Fields[field.OriginalParentRelatedFieldName]).ParentView;

                sa.Edit(originalView, values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit, (System.Data.SqlClient.SqlCommand)e.Command, (System.Data.SqlClient.SqlCommand)e.SysCommand, e.History, userId);

            }

        }

        protected virtual void CompleteApprovalProcess(EditEventArgs e)
        {
            if (IsApprovalProcessUserView(e.View))
            {
                CompleteApprovalProcess(e.View, e.PrevRow, GetApprovalProcessId(e), Convert.ToInt32(e.PrimaryKey), e.Command);
            }
        }

        private int GetApprovalProcessUserId(EditEventArgs e)
        {
            Field field = ((View)e.View).GetFieldByColumnNames("ApprovalProcessUserId");
            if (field == null)
            {
                throw new Durados.Workflow.WorkflowEngineException("Could not found the Approval Process User Id value in view " + e.View.DisplayName);
            }
            if (!e.Values.ContainsKey(field.Name))
            {
                throw new Durados.Workflow.WorkflowEngineException("Could not found the Approval Process User Id value in edit action of view " + e.View.DisplayName);
            }
            return Convert.ToInt32(e.Values[field.Name]);
        }

        private int GetApprovalProcessId(EditEventArgs e)
        {
            Field field = ((View)e.View).GetFieldByColumnNames("ApprovalProcessId");
            if (field == null)
            {
                throw new Durados.Workflow.WorkflowEngineException("Could not found the Approval Process Id value in view " + e.View.DisplayName);
            }
            if (!e.Values.ContainsKey(field.Name))
            {
                if (e.PrevRow == null)
                    throw new Durados.Workflow.WorkflowEngineException("Could not load the Approval Process data of the view " + e.View.DisplayName);

                string value = field.GetValue(e.PrevRow);
                if (string.IsNullOrEmpty(value))
                    throw new Durados.Workflow.WorkflowEngineException("Could not found the Approval Process Id value in edit action of view " + e.View.DisplayName);

                return Convert.ToInt32(value);
            }
            return Convert.ToInt32(e.Values[field.Name]);
        }

        private bool IsApprovalProcessUserView(Durados.View view)
        {
            return view.Name.Equals("v_durados_ApprovalProcessUser");
        }

        protected virtual void CompleteApprovalProcess(Durados.View approvalProcessUserView, DataRow approvalProcessUserRow, int approvalProcessId, int approvalProcessUserId, IDbCommand command)
        {
            Durados.Workflow.ApprovalProcess approvalProcess = GetApprovalProcess();

            Field field = approvalProcessUserView.GetFieldByColumnNames("ApprovalProcessId");
            ParentField approvalProcessParentField = (ParentField)field;

            string statusFieldName = "";
            Field statusField = approvalProcessParentField.ParentView.GetFieldByColumnNames("ApprovalStatusId");
            if (statusField != null)
                statusFieldName = statusField.Name;

            int? status = approvalProcess.Complete(this, approvalProcessId, approvalProcessUserId, approvalProcessParentField.ParentView.Name, statusFieldName, GetCompleteApprovalProcessStoredProcedureName(), command);

            if (!status.HasValue || status.Value == 1) // pending
                return;


            if (field == null)
                return;

            DataRow approvalProcessRow = approvalProcessUserRow.GetParentRow(approvalProcessParentField.DataRelation.RelationName);

            if (approvalProcessRow == null)
                return;

            Field parentViewNameField = approvalProcessParentField.ParentView.GetFieldByColumnNames("ParentView");

            string parentViewName = parentViewNameField.GetValue(approvalProcessRow);

            View view = (View)GetView(parentViewName).Base;

            field = approvalProcessParentField.ParentView.Fields.Values.Where(f => f.FieldType == FieldType.Parent && ((ParentField)f).ParentView.Name.Equals(parentViewName)).FirstOrDefault();

            if (field == null)
                return;

            ParentField parentViewField = (ParentField)field;

            string pk = parentViewField.GetValue(approvalProcessRow);
            if (pk == null)
                return;

            DataRow parentRow = approvalProcessRow.GetParentRow(parentViewField.DataRelation.RelationName);

            if (parentRow == null)
                parentRow = view.GetDataRow(pk);

            if (parentRow == null)
                return;

            Dictionary<string, object> values = new Dictionary<string, object>();
            if (view.WorkFlowStepsField != null)
            {
                object stepValue = view.WorkFlowStepsField.GetValue(parentRow);
                values.Add(view.WorkFlowStepsFieldName, stepValue);
            }

            wfe.PerformActions(this, view, TriggerDataAction.AfterCreateOrEdit, values, pk, parentRow, view.Database.ConnectionString, Convert.ToInt32(((Database)view.Database).GetUserID()), ((Database)view.Database).GetUserRole(), null, null);
            if (wfe.StepResult == null)
                return;
            //throw new Durados.Workflow.WorkflowEngineException("Please check workflow rules, complete step must be configured to AfterCreateOrEdit");

            var enabledSteps = wfe.StepResult.EnabledSteps.Where(es => es.Enable);
            if (enabledSteps.Count() == 1)
            {
                Durados.Workflow.Step.EnabledStep enabledStep = enabledSteps.First();
                values = new Dictionary<string, object>();
                values.Add(view.WorkFlowStepsFieldName, enabledStep.Id);
                //view.Edit(values, pk, null, null, null);
                view.Edit(values, pk, BeforeCompleteStep, BeforeCompleteStepInDatabase, AfterCompleteStepBeforeCommit, AfterCompleteStepAfterCommit);
            }

        }



        protected virtual string GetCompleteApprovalProcessStoredProcedureName()
        {
            return "durados_CompleteApprovalProcess";
        }

        protected virtual Durados.Workflow.ApprovalProcess GetApprovalProcess()
        {
            return new Durados.Workflow.ApprovalProcess();
        }

        protected virtual void view_AfterEditAfterCommit(object sender, EditEventArgs e)
        {
            AfterEditAfterCommit(e);
        }

        protected virtual void view_AfterEditAfterCommitBookmark(object sender, EditEventArgs e)
        {
            //AfterEditAfterCommit(e);
        }

        private string GetUsername()
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && System.Web.HttpContext.Current.User.Identity.Name != null)
                return System.Web.HttpContext.Current.User.Identity.Name;
            else
                return string.Empty;

        }
        protected virtual void AfterEditAfterCommit(EditEventArgs e)
        {
            //Workflow.Engine wfe = CreateWorkflowEngine();
            wfe.PerformActions(this, e.View, TriggerDataAction.AfterEdit, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, Convert.ToInt32(((Database)e.View.Database).GetUserID()), ((Database)e.View.Database).GetUserRole(), e.Command, e.SysCommand);

            wfe.Notifier.Notify((View)e.View, 1, GetUsername(), e.OldNewValues, e.PrimaryKey, e.PrevRow, this, e.Values, GetSiteWithoutQueryString(), GetMainSiteWithoutQueryString());

            const string Active = "Active";
            if (e.View.Name == "Durados_Language")
            {
                bool prevActive = !e.PrevRow.IsNull(Active) && Convert.ToBoolean(e.PrevRow[Active]);
                bool currActive = e.Values.ContainsKey(Active) && Convert.ToBoolean(e.Values[Active]);

                if (!prevActive && currActive)
                {
                    string code = e.PrevRow["Code"].ToString();
                    string scriptFile = Maps.GetDeploymentPath("Sql/Localization/" + code + "pack.sql");

                    SqlAccess sqlAcces = new SqlAccess();
                    sqlAcces.RunScriptFile(scriptFile, Map.GetLocalizationDatabase().ConnectionString);
                    Map.Database.Localizer.SetCurrentUserLanguageCode(code);
                }
            }

            HandleApi(e.View);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Delete(string viewName, string pk, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "Delete");


                if (!view.IsDeletable())
                    throw new AccessViolationException();


                view.Delete(pk, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit);

                //Durados.Web.Mvc.Logging.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), null, null, 3, null);

                return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", exception.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult DeleteSelection(string viewName, string pks, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "DeleteSelection");

                //view.BeforeDelete += new View.BeforeDeleteEventHandler(view_BeforeDelete);
                //view.AfterDeleteBeforeCommit += new View.AfterDeleteEventHandler(view_AfterDeleteBeforeCommit);
                //view.AfterDeleteAfterCommit += new View.AfterDeleteEventHandler(view_AfterDeleteAfterCommit);

                //if (!view.IsDeletable())
                //    throw new AccessViolationException();

                string[] pkArray = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<string[]>(pks);

                foreach (string pk in pkArray)
                    view.Delete(pk, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit);

                return RedirectToAction("Index", new { viewName = viewName, pks = pks, guid = guid });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Delete", exception));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult EditSelection(string viewName, string pks, FormCollection collection, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "EditSelection");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                string[] pkArray = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<string[]>(pks);

                foreach (string pk in pkArray)
                {
                    if (wfe != null)
                        wfe.StepResult = null;
                    view.EditRow(collection, pk, true, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                }

                return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
                //return Json(new Json.JsonResult("Success", true));
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
        }

        protected UI.FieldValue GetFilterField(View view, string guid)
        {
            Durados.Web.Mvc.UI.Json.Filter jsonFilter = ViewHelper.ConvertQueryStringToJsonFilter(guid);

            if (jsonFilter == null || jsonFilter.Fields.Count == 0)
                return null;

            foreach (Durados.Web.Mvc.UI.Json.Field jsonField in jsonFilter.Fields.Values)
            {
                Field field = view.GetFieldByColumnNames(jsonField.Name);
                if (field != null)
                    return new UI.FieldValue() { Field = field, Value = jsonField.Value.ToString() };
            }

            return null;
        }

        public string[] GetFilterFieldValue(Durados.View view)
        {
            if (ViewData["guid"] == null)
                return null;

            UI.FieldValue filterField = GetFilterField((View)view, ViewData["guid"].ToString());

            if (filterField == null)
                return null;

            return new string[2] { filterField.Field.Name, filterField.Value };
        }


        public virtual ActionResult AddItemsBulk(string viewName, string parentFieldName, string fk, FormCollection collection, string guid, string searchGuid)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "AddItemsBulk");
            SqlAccess sqlAccess = new SqlAccess();
            if (sqlAccess == null)
                throw new DuradosException("Failed to create SqlAccess in Add Items Bulk");
            int asyncAddItemsRowsAmount = GetAsyncAddItemsRowsAmount();

            if (!view.IsCreatable())
                throw new AccessViolationException();

            ParentField field = (ParentField)view.AddItemsField;
            if (field == null)
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", Map.Database.Localizer.Translate("Illegal operation"));
            Durados.Web.Mvc.ParentField addItemsField = (Durados.Web.Mvc.ParentField)view.AddItemsField;

            Durados.Web.Mvc.View parentView = (Durados.Web.Mvc.View)addItemsField.ParentView;
            FormCollection filterCollection = new FormCollection(ViewHelper.GetSessionState(searchGuid + "Filter"));
            UI.Json.Filter jsonFilter = ViewHelper.GetJsonFilter(filterCollection, searchGuid);


            Dictionary<string, object> values = new Dictionary<string, object>();

            if (jsonFilter != null)
            {
                foreach (UI.Json.Field jsonField in jsonFilter.Fields.Values)
                {
                    if (jsonField.Value != null && !string.IsNullOrEmpty(jsonField.Value.ToString()))
                    {
                        values.Add(jsonField.Name, jsonField.Value);
                    }

                }
            }
            Durados.DataAccess.Filter filter = sqlAccess.GetFilter(parentView, values);
            int totalRows = filter.Parameters.Count() > 0 ? sqlAccess.RowFilterCount(parentView, filter) : sqlAccess.RowCount(parentView);
            int timeout = GetAddItemsAsyncTimeout();
            string email = Map.Database.GetEmailByUsername(GetUsername());
            Dictionary<string, object> defaultValues = new Dictionary<string, object>();
            var hasDefaultValues = view.Fields.Values.Where(f => f.IsExcluded(DataAction.Create) == false && f.DefaultValue != null);

            foreach (Field field2 in hasDefaultValues)
            {
                if (!defaultValues.ContainsKey(field2.DatabaseNames))
                {
                    defaultValues.Add(field2.DatabaseNames, ReplaceToken(field2.DefaultValue.ToString(), view));
                }
            }
            if (parentFieldName != null && fk != null)
            {
                ParentField parentField = (ParentField)view.Fields[parentFieldName];
                {
                    if (values.ContainsKey(parentField.DatabaseNames))
                        defaultValues[parentField.DatabaseNames] = fk;
                    else
                        defaultValues.Add(parentField.DatabaseNames, fk);
                }


            }
            try
            {
                sqlAccess.InsertSelectBulk(view, addItemsField, filter, defaultValues, asyncAddItemsRowsAmount, totalRows, timeout, email, Map, HandleAddItemsAsyncCallback_success, HandleAddItemsAsyncCallback_failure, RunningAsyncOperationCallback, IsAsyncOperationRuningCallback);
            }
            catch (DuradosAsyncRunningException)
            {
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", Map.Database.Localizer.Translate("A previous long request is undergoing, Please try again later."));
            }
            if (totalRows > asyncAddItemsRowsAmount)
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", Map.Database.Localizer.Translate("Your request is performed. You will receive an email when it is complete."));
            else
                return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });

        }

        protected void RunningAsyncOperationCallback()
        {
            Map.AsyncOperationRuning = true;
        }

        protected bool IsAsyncOperationRuningCallback()
        {
            return Map.AsyncOperationRuning;
        }

        private object ReplaceToken(string value, View view)
        {

            if (value.Contains(Durados.Web.Mvc.Database.UserPlaceHolder) || value.ToLower().Contains(Durados.Web.Mvc.Database.UserPlaceHolder.ToLower()))
                value = value.Replace(Durados.Web.Mvc.Database.UserPlaceHolder, Map.Database.GetUserID() ?? Durados.Web.Mvc.Database.NullInt, false);
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && !string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name) && (value.Contains(Durados.Web.Mvc.Database.UsernamePlaceHolder) || value.ToLower().Contains(Durados.Web.Mvc.Database.UsernamePlaceHolder.ToLower())))
                value = value.Replace(Durados.Web.Mvc.Database.UsernamePlaceHolder, System.Web.HttpContext.Current.User.Identity.Name ?? Durados.Web.Mvc.Database.NullInt, false);
            if (value.Contains(Durados.Web.Mvc.Database.RolePlaceHolder) || value.ToLower().Contains(Durados.Web.Mvc.Database.RolePlaceHolder.ToLower()))
                value = value.Replace(Durados.Web.Mvc.Database.RolePlaceHolder, Map.Database.GetUserRole(), false);
            if (value.Contains(Durados.Web.Mvc.Database.CurrentDatePlaceHolder) || value.ToLower().Contains(Durados.Web.Mvc.Database.CurrentDatePlaceHolder.ToLower()))
                value = value.Replace(Durados.Web.Mvc.Database.CurrentDatePlaceHolder, DateTime.Now.ToString(Database.DateFormat), false);
            return value;
        }

        protected void HandleAddItemsAsyncCallback_success(IAsyncResult ar)
        {
            System.Data.SqlClient.SqlCommand command = null;

            try
            {
                Dictionary<string, object> args = (Dictionary<string, object>)ar.AsyncState;
                Map map = (Map)args["map"];
                map.Logger.Log(GetControllerNameForLog(ControllerContext), "AddItemsBulk", "HandleAddItemsAsyncCallback", null, 3, "start Async Add items callback");
                Durados.View view = (Durados.View)args["view"];
                command = (System.Data.SqlClient.SqlCommand)args["command"];
                string message = map.Database.Localizer.Translate("Your request to add items to {0} is finished, Please go to {1} to review the results");

                string AddItemsErrEmailMessage = map.Database.Localizer.Translate("AddItemsErrEmailMessage");
                AddItemsErrEmailMessage = AddItemsErrEmailMessage != "AddItemsErrEmailMessage" ? AddItemsErrEmailMessage : "Your request to add items to {0} has terminated prior to completion, Please contact your administrator.\r\n Go to {1} to review the results";

                if (view != null)
                {
                    message = string.Format(message, view.DisplayName, Map.Url + "/Home/Index/" + view.Name);//args["time"].ToString(),
                }
                string to = args["email"] != null ? args["email"].ToString() : Map.GetUserEmail(GetUserRow()[GetUserRow().Table.PrimaryKey[0].ToString()].ToString());
                try
                {
                    int rowCount = command.EndExecuteNonQuery(ar);
                }
                catch (Exception)
                {
                    AddItemsErrEmailMessage = string.Format(AddItemsErrEmailMessage, view.DisplayName, Map.Url + "/Home/Index/" + view.Name);
                    SendEmail(GetDefaultFrom(), to, GetDefaultFrom(), AddItemsErrEmailMessage, map.Database.Localizer.Translate(Durados.Database.ShortProductName + " Add Items to  view has finished!"));
                }

                SendEmail(GetDefaultFrom(), to, GetDefaultFrom(), message, map.Database.Localizer.Translate(Durados.Database.ShortProductName + " Add Items to  view has finished!"));

                map.Logger.Log(GetControllerNameForLog(ControllerContext), "AddItemsBulk", "HandleAddItemsAsyncCallback", null, 3, "end  Async Add items callback and sent mail");



            }
            catch (Exception exception)
            {
                Maps.Instance.GetMap().Logger.Log(GetControllerNameForLog(ControllerContext), "AddItemsBulk", "HandleAddItemsAsyncCallback", exception, 3, null);
            }
            finally
            {
                if (command != null && command.Connection != null)
                    command.Connection.Close();
                Map.AsyncOperationRuning = false;
            }

        }


        protected void HandleAddItemsAsyncCallback_failure(Dictionary<string, object> args)
        {
            if (args.ContainsKey("exception"))
                try
                {
                    string message = "Your request to add items to {0} from {1} has failed. Please review the error\r\n{2}\r\n{3}.";
                    Durados.View view = (Durados.View)args["view"];
                    if (view != null)
                    {
                        message = string.Format(message, view.DisplayName, args["time"].ToString(), ((Exception)args["exception"]).Message, ((Exception)args["exception"]).StackTrace);

                    }

                    string to = Map.GetUserEmail(GetUserRow()[GetUserRow().Table.PrimaryKey[0].ToString()].ToString());
                    SendEmail(GetDefaultFrom(), to, GetDefaultFrom(), message, Durados.Database.ShortProductName + " ModuBiz Add Items to  view has finished!");
                }
                catch (Exception exception)
                {
                    throw new DuradosException(exception.Message, exception);
                }

        }

        protected virtual int GetAsyncAddItemsRowsAmount()
        {
            return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AsyncAddItemsRowsAmount"] ?? "100000");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult AddItems(string viewName, string pks, string parentViewName, string fk, FormCollection collection, string guid, bool? isSelectAll, string searchGuid)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "EditSelection");
            Durados.Web.Mvc.View parentView = null;
            Durados.Web.Mvc.ParentField parentField = null;

            if (!string.IsNullOrEmpty(parentViewName))
            {
                if (string.IsNullOrEmpty(fk))
                {
                    return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", "The related value of the parent [" + parentViewName + "] is missing.");
                }
                parentView = GetView(parentViewName);

                foreach (ParentField p in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
                {
                    if (p.ParentView.Base.Name.Equals(parentView.Base.Name))
                    {
                        parentField = p;
                        break;
                    }
                }
            }

            if (parentField == null && string.IsNullOrEmpty(fk))
            {
                UI.FieldValue fieldValue = GetFilterField(view, guid);
                if (fieldValue != null)
                {
                    parentField = (ParentField)fieldValue.Field;
                    fk = fieldValue.Value;
                }
            }

            //System.Data.SqlClient.SqlTransaction transaction = null;
            System.Data.IDbTransaction transaction = null;


            //using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(view.Database.ConnectionString))
            //{
            //    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand())
            //    {
            using (System.Data.IDbConnection connection = GetConnection(view.Database.SqlProduct, view.Database.ConnectionString))
            {
                using (System.Data.IDbCommand command = GetCommand(view.Database.SqlProduct))
                {
                    connection.Open();

                    System.Data.SqlClient.SqlCommand sysCommand = null;

                    bool identicalSystemConnection = Database.IdenticalSystemConnection;
                    try
                    {
                        transaction = connection.BeginTransaction();
                        command.Connection = connection;
                        command.Transaction = transaction;
                        bool roolback = false;

                        if (identicalSystemConnection)
                            sysCommand = (System.Data.SqlClient.SqlCommand)command;
                        else
                        {
                            System.Data.SqlClient.SqlConnection sysConnection = new System.Data.SqlClient.SqlConnection(view.Database.SystemConnectionString);
                            sysConnection.Open();
                            System.Data.SqlClient.SqlTransaction sysTransaction = sysConnection.BeginTransaction();
                            sysCommand = new System.Data.SqlClient.SqlCommand();
                            sysCommand.Connection = sysConnection;
                            sysCommand.Transaction = sysTransaction;

                        }

                        if (!view.IsCreatable())
                            throw new AccessViolationException();

                        ParentField field = (ParentField)view.AddItemsField;
                        if (field == null)
                            return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", "Illegal operation");

                        string[] pkArray = pks.Split(',');

                        var notExcluded = view.Fields.Values.Where(f => f.IsExcluded(DataAction.Create) == false);

                        History history = GetNewHistory();

                        int userId = Convert.ToInt32(GetUserID());

                        foreach (string pk in pkArray)
                        {
                            Dictionary<string, object> values = new Dictionary<string, object>();
                            values.Add(field.Name, pk);
                            if (parentField != null)
                            {
                                if (values.ContainsKey(parentField.Name))
                                    values[parentField.Name] = fk;
                                else
                                    values.Add(parentField.Name, fk);
                            }

                            foreach (Field field2 in notExcluded)
                            {
                                if (!values.ContainsKey(field2.Name))
                                {
                                    string val = field2.DefaultValue != null ? field2.DefaultValue.ToString() : string.Empty;
                                    values.Add(field2.Name, val);
                                }
                            }
                            int? id = view.Create(values, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit, command, out roolback);
                            history.SaveCreate(sysCommand, view, id.Value.ToString(), userId, view.Database.SwVersion, view.GetWorkspaceName());

                            if (roolback)
                            {
                                string displayValue = field.ParentView.GetDisplayValue(field.ParentView.GetDataRow(pk));
                                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", "Failed to add " + displayValue + ". The operation was canceled.");
                            }
                        }

                        transaction.Commit();
                        if (!identicalSystemConnection)
                            sysCommand.Transaction.Commit();

                        return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
                        //return Json(new Json.JsonResult("Success", true));

                    }
                    catch (Exception exception)
                    {
                        //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                        if (transaction != null)
                            transaction.Rollback();
                        if (!identicalSystemConnection)
                            sysCommand.Transaction.Rollback();
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                        return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));

                    }
                    finally
                    {
                        if (!identicalSystemConnection && sysCommand != null)
                            sysCommand.Connection.Close();
                    }
                }
            }

        }

        protected void view_BeforeDelete(object sender, DeleteEventArgs e)
        {
            try
            {
                BeforeDelete(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        void view_BeforeDeleteBookmark(object sender, DeleteEventArgs e)
        {
            try
            {
                int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
                string currentUserRole = Map.Database.GetUserRole();

                if (e.View.SaveHistory)
                {
                    e.History = GetNewHistory();
                    e.UserId = currentUserId;
                }
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected void view_AfterDeleteBeforeCommitBookmark(object sender, DeleteEventArgs e)
        {
            try
            {
                //AfterDeleteBeforeCommit(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected void view_AfterDeleteBeforeCommit(object sender, DeleteEventArgs e)
        {
            try
            {
                AfterDeleteBeforeCommit(e);

                if (Maps.MultiTenancy)
                {
                    HandleMultiTenancyUser(e);
                }
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected void view_AfterDeleteAfterCommit(object sender, DeleteEventArgs e)
        {
            AfterDeleteAfterCommit(e);
        }

        void view_AfterDeleteAfterCommitBookmark(object sender, DeleteEventArgs e)
        {
            //AfterDeleteAfterCommit(e);
        }

        protected virtual void BeforeDelete(DeleteEventArgs e)
        {
            int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
            string currentUserRole = Map.Database.GetUserRole();

            if (e.View.SaveHistory)
            {
                e.History = GetNewHistory();
                e.UserId = currentUserId;
            }
            CreateWorkflowEngine().PerformActions(this, e.View, TriggerDataAction.BeforeDelete, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, currentUserId, currentUserRole, e.Command, e.SysCommand);
        }

        protected virtual void AfterDeleteBeforeCommit(DeleteEventArgs e)
        {
            //Workflow.Engine wfe = CreateWorkflowEngine();

            wfe.PerformActions(this, e.View, TriggerDataAction.AfterDeleteBeforeCommit, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, Convert.ToInt32(Map.Database.GetUserID()), ((Database)e.View.Database).GetUserRole(), e.Command, e.SysCommand);

        }

        protected virtual void AfterDeleteAfterCommit(DeleteEventArgs e)
        {

            wfe.PerformActions(this, e.View, TriggerDataAction.AfterDelete, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, Convert.ToInt32(Map.Database.GetUserID()), ((Database)e.View.Database).GetUserRole(), e.Command, e.SysCommand);
            wfe.Notifier.Notify((View)e.View, 3, GetUsername(), null, e.PrimaryKey, e.PrevRow, this, null, GetSiteWithoutQueryString(), GetMainSiteWithoutQueryString());

        }

        public virtual ActionResult AutoComplete(string q, int limit, string viewName, string field)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "AutoComplete");
            return Json(view.GetAutoCompleteValues(field, limit, q));
        }

        public JsonResult GetDependencyCheckList(string viewName2, string fieldName, string fk, string pk, FormCollection collection)
        {
            Durados.Web.Mvc.View view = GetView(viewName2, "GetDependencyCheckList");

            if (view.Fields.ContainsKey(fieldName) && view.Fields[fieldName].FieldType == FieldType.Children)
            {
                ChildrenField childField = (ChildrenField)view.Fields[fieldName];

                IEnumerable<SelectListItem> selectMultiOptions = childField.GetSelectList(fk, pk);
                Durados.Web.Mvc.UI.Json.SelectList selectMultiList = Durados.Web.Mvc.UI.Json.SelectList.GetSelectList(selectMultiOptions);

                return Json(selectMultiList);
            }

            return Json("");

        }

        /// <summary>
        /// Get filter uniqe values by viewName and fieldName
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="fieldName"></param>
        /// <param name="fk"></param>
        /// <returns></returns>
        public virtual JsonResult GetFilterValues(string viewName, string fieldName, string fk)
        {
            Dictionary<string, string> filterValues = CheckListHelper.GetSelectOptions(viewName, fieldName, fk, true);

            return Json(filterValues);
        }

        public virtual JsonResult GetSelectList(string viewName2, string fieldName, string fk, FormCollection collection)
        {
            Durados.Web.Mvc.View view = GetView(viewName2, "GetSelectList");

            ParentField field = FieldHelper.GetParentField(viewName2, fieldName);

            Dictionary<string, string> selectOptions;
            if (field != null)
            {
                field.BeforeDropDownOptions += new ParentField.BeforeDropDownOptionsEventHandler(View_BeforeDropDownOptions);
                //br todo:ask
                selectOptions = CheckListHelper.GetSelectOptions(field, fk, null, true);
                field.BeforeDropDownOptions -= new ParentField.BeforeDropDownOptionsEventHandler(View_BeforeDropDownOptions);
            }
            else
            {
                selectOptions = new Dictionary<string, string>();
            }


            Durados.Web.Mvc.UI.Json.SelectList selectList = Durados.Web.Mvc.UI.Json.SelectList.GetSelectList(selectOptions);

            return Json(selectList);

        }

        public JsonResult GetSelectList2(string viewName2, string fieldName)
        {
            Durados.Web.Mvc.View view = GetView(viewName2, "GetSelectList");

            ParentField field = null;

            if (view.Fields.ContainsKey(fieldName))
            {
                if (view.Fields[fieldName].FieldType == FieldType.Parent)
                {
                    field = (ParentField)view.Fields[fieldName];
                    field.BeforeDropDownOptions += new ParentField.BeforeDropDownOptionsEventHandler(View_BeforeDropDownOptions);
                }
                else if (view.Fields[fieldName].FieldType == FieldType.Children)
                {
                    field = (ParentField)((ChildrenField)view.Fields[fieldName]).GetFirstNonEquivalentParentField();
                    field.BeforeDropDownOptions += new ParentField.BeforeDropDownOptionsEventHandler(View_BeforeDropDownOptions);
                }
            }
            Dictionary<string, string> selectOptions;
            if (field != null)
            {
                bool useUniqueName = field.ParentHtmlControlType == ParentHtmlControlType.InsideDependencyUniqueNames;

                //changed by br 3
                //br todo:ask
                selectOptions = CheckListHelper.GetSelectOptions(field, string.Empty, null, true);
                //selectOptions = field.GetSelectOptions(null, false, useUniqueName, null, null);

                field.BeforeDropDownOptions -= new ParentField.BeforeDropDownOptionsEventHandler(View_BeforeDropDownOptions);
            }
            else
            {
                selectOptions = new Dictionary<string, string>();
            }


            Durados.Web.Mvc.UI.Json.SelectList selectList = Durados.Web.Mvc.UI.Json.SelectList.GetSelectList(selectOptions);

            return Json(selectList);

        }

        protected string CleanValueSuffix(View view, string value)
        {
            if (view.DataTable.PrimaryKey.Length > 0)
            {
                DataColumn column = view.DataTable.PrimaryKey[view.DataTable.PrimaryKey.Length - 1];
                return CleanValueSuffix(column, value);
            }
            return value;
        }

        protected string CleanValueSuffix(DataColumn column, string value)
        {
            if (value != null && value.ToString().EndsWith("#"))
            {
                if (!column.DataType.Equals(typeof(string)))
                {
                    value = value.ToString().TrimEnd('#');
                }
            }

            return value;
        }

        public virtual JsonResult GetJsonView(string viewName, DataAction dataAction, string pk, string guid)
        {
            Durados.Web.Mvc.View view = GetView(viewName);

            pk = CleanValueSuffix(view, pk);

            UI.Json.View jsonView;
            if (string.IsNullOrEmpty(pk))
                jsonView = view.GetJsonViewNotSerialized(dataAction, guid);
            else
            {

                jsonView = view.GetJsonViewNotSerialized(dataAction, pk, GetDataRow(view, pk), guid);
            }

            return Json(GetJsonViewSerialized(view, dataAction, jsonView));
        }

        protected virtual DataRow GetDataRow(View view, string pk)
        {
            return view.GetDataRow(pk, null, view_BeforeSelect, view_AfterSelect);
        }

        protected virtual string GetJsonViewSerialized(View view, DataAction dataAction, UI.Json.View jsonView)
        {
            return view.GetJsonViewSerialized(jsonView);
        }

        public virtual JsonResult UpdateParent(string viewName, string pk, string guid)
        {
            Durados.Web.Mvc.View view = GetView(viewName);

            DataRow row = view.GetDataRow(pk);
            UI.TableViewer tableViewer = GetNewTableViewer();

            List<object> fields = new List<object>();

            foreach (Durados.Field field in view.VisibleFieldsForTableFirstRow)
            {
                var fieldValue = new { Name = field.Name, Value = Server.HtmlEncode(tableViewer.GetFieldValue(field, row)), DisplayValue = tableViewer.GetElementForTableView(field, row, guid) };
                fields.Add(fieldValue);
            }

            return Json(fields);
        }



        public ContentResult GetRich(string viewName, string pk, string fieldName)
        {

            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Database.Views[viewName];
            DataRow dataRow = view.GetDataRow(pk);
            Field field = view.Fields[fieldName];
            string html = field.ConvertToString(dataRow);
            html = html ?? string.Empty;

            return Content(html);

        }

        //[ValidateInput(false)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public virtual ActionResult EditRich(string viewName, string fieldName, string pk, string html, string guid)
        //{
        //    try
        //    {
        //        Durados.Web.Mvc.View view = GetView(viewName, "EditRich");

        //        if (!view.IsEditable(guid))
        //            throw new AccessViolationException();

        //        view.EditField(fieldName, html, pk, view_BeforeEdit, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
        //        return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
        //        //return Json(new Json.JsonResult("Success", true));
        //    }
        //    catch (Exception exception)
        //    {
        //        //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
        //        return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));

        //    }
        //}

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult EditRich(string viewName, string fieldName, string pk, string html, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "EditRich");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                view.EditField(fieldName, html, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                return Json((new UI.ColumnFieldViewer()).GetRichDialog((ColumnField)view.Fields[fieldName], pk, html, guid));
                //return Json(new Json.JsonResult("Success", true));
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("$$error$$" + FormatExceptionMessage(viewName, "EditRich", exception));
            }
        }


        public virtual ActionResult EditUrl(string viewName, string fieldName, string pk, string value, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "EditRich");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();

                view.EditField(fieldName, value, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                return PartialView("~/Views/Shared/Controls/Message.ascx", (object)"succeass");
                //return Json(new Json.JsonResult("Success", true));
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
        }

        //[ValidateInput(false)]
        //public virtual ActionResult GetCheckListForFilter(string viewName, string fieldName, string guid)
        //{
        //    try
        //    {
        //        Durados.Web.Mvc.View view = GetView(viewName, "GetCheckListForFilter");

        //        ParentField field = (ParentField)view.Fields[fieldName];
        //        ViewData["guid"] = guid;
        //        return PartialView("~/Views/Shared/Controls/CheckListForFilter.ascx", field);
        //    }
        //    catch (Exception exception)
        //    {
        //        Durados.Web.Mvc.Logging.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
        //        return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "GetCheckListForFilter", exception));

        //    }
        //}

        public virtual ActionResult ChangePageSize(string viewName, int pageSize, string guid)
        {
            string public2 = Request.QueryString["public"] ?? Request.QueryString["public2"];

            Durados.Web.Mvc.View view = GetView(viewName, "ChangePageSize");
            PagerHelper.SetPageSize(view, pageSize, guid);
            return RedirectToAction("Index", new { viewName = viewName, page = 1, guid = guid, public2 = public2 });
        }

        public virtual ActionResult ImageViewer(string viewName, string pk)
        {
            return View(GetImageUrl(viewName, pk));
        }

        protected virtual Durados.Web.Mvc.UI.ImageViewerInfo GetImageUrl(string viewName, string pk)
        {
            return new Durados.Web.Mvc.UI.ImageViewerInfo() { Url = string.Empty, Title = string.Empty };
        }

        public virtual void SetLanguage2(string languageCode)
        {
            Map.Database.Localizer.SetCurrentUserLanguageCode(languageCode);
        }

        public virtual ActionResult SetLanguage(string viewName, string languageCode)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "SetLanguage");

            Map.Database.Localizer.SetCurrentUserLanguageCode(languageCode);

            return RedirectToAction(view.IndexAction, view.Controller, new { viewName = viewName });
        }

        public virtual ActionResult GetTextArea(string text)
        {
            return PartialView("~/Views/Shared/Controls/TextArea.ascx", text);
        }

        protected virtual DataRow GetUserRow()
        {
            View userView = GetUserView();
            Field usernameField = GetUsernameField(userView);
            return GetUserRowByUsername(HttpContext.User.Identity.Name);
        }

        protected virtual string GetUserID()
        {
            return Map.Database.GetUserID();
        }

        protected virtual DataRow GetUserRowByUsername(string username)
        {
            View userView = GetUserView();
            Field usernameField = GetUsernameField(userView);
            return userView.GetDataRow(usernameField, username);
        }

        protected virtual DataRow GetUserRow(string pk)
        {
            View userView = GetUserView();
            return userView.GetDataRow(pk);
        }

        protected virtual View GetUserView()
        {
            return (View)Database.GetUserView();
        }

        protected virtual Field GetUsernameField(View userView)
        {
            return ((Database)Database).GetUsernameField();
        }

        public virtual JsonResult Copy(string viewName, string templateView)
        {

            try
            {
                View view = GetView(viewName);
                View template = GetView(templateView);

                string message = Map.Copy(template, view);

                Map.SaveConfigForTheFirstTimeInCaseOfChangeInStructure();

                return Json(message);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(exception.Message);
            }
        }

        public virtual JsonResult Clone(string viewName, string clonedViewName)
        {

            try
            {
                if (Database.Views.ContainsKey(clonedViewName))
                {
                    return Json("View already exists. Choose different name");
                }

                View view = GetView(viewName);

                List<string> excludeRelations = new List<string>();

                excludeRelations.Add("Rules");

                (new ConfigAccess()).Clone(Map.GetConfigDatabase().Views["View"], viewName, clonedViewName, excludeRelations);

                Map.Refresh();

                return Json("success");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(exception.Message);
            }
        }

        protected virtual string GetPasswordFieldName()
        {
            return "Password";
        }

        protected virtual string GetUsernameFieldName()
        {
            return ((Durados.Web.Mvc.Database)Database).UsernameFieldName;
        }

        protected virtual string GetDuradosParameterView()
        {
            return "Durados_Parameters";
        }

        protected virtual string GetDuradosParameter(string pk)
        {

            string viewName = GetDuradosParameterView();
            Durados.Web.Mvc.View templateView = GetView(viewName);
            DataRow dRow = templateView.GetDataRow(pk);

            string value = templateView.GetDisplayValue("Text", dRow);

            return value;

        }

        #region Cron

        public virtual void CronCycle(string cycle)
        {
            int cycleNumber = Convert.ToInt32(Enum.Parse(typeof(CycleEnum), cycle));
            string url = "http://" + System.Web.HttpContext.Current.Request.Headers["Host"];
            DataTable cronsTable = CronHelper.GetCycleCrons(cycleNumber);
            foreach (DataRow cronRow in cronsTable.Rows)
            {
                url += Url.Action("AsyncCron", "Home", new { appId = cronRow["AppId"].ToString(), cycle = cycleNumber });
                ISendAsyncErrorHandler SendAsyncErrorHandler = null;
                Infrastructure.Http.AsynWebRequest(url, SendAsyncErrorHandler);
                //Infrastructure.Http.CallWebService(url);
                //Call httprequest  to AsyncCron
                //(cronRow["AppId"].ToString());

            }
        }

        public virtual void AsyncCron(int appId, int cycle)
        {
            CycleEnum cycleEnum = (CycleEnum)cycle;
            Map Map = Maps.Instance.GetMap(CronHelper.GetAppName(appId));
            Map.Logger.Log("AsyncCron", "Send", "start", null, 10, "");
            IEnumerable<Cron> crons = Map.Database.Crons.Where(cron => cycleEnum.Equals(((Cron)cron.Value).Cycle)).Select(c => c.Value);
            foreach (Cron cron in crons)
            {
                Cron(cron.Name);
            }
        }

        public virtual void Cron(string cronName)
        {
            if (!Map.Database.Crons.ContainsKey(cronName))
                return;
            Cron cron = Map.Database.Crons[cronName];

            SqlAccess sqlAccess = new SqlAccess();

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("CronName", cron.Name);
            DataTable table = sqlAccess.ExecuteTable(Map.Database.ConnectionString, cron.ProcedureName, parameters, CommandType.StoredProcedure);

            string userId = Map.Database.GetUserID();
            string template = GetTemplate(cron);
            if (!string.IsNullOrEmpty(userId))
            {
                foreach (DataRow row in table.Rows)
                {
                    SendCron(cron, row, Convert.ToInt32(userId), template);
                }
            }
        }


        #endregion

        protected virtual string GetTemplate(string viewName, string pk)
        {
            Durados.Web.Mvc.View templateView = GetView(viewName);
            DataRow templateRow = templateView.GetDataRow(pk);
            if (templateRow == null)
                return pk;

            string template = templateView.GetDisplayValue(GetEmailTemplateFieldName(), templateRow);

            return template;
        }

        protected virtual string GetEmailTemplateFieldName()
        {
            return "Text";
        }

        public virtual string GetTemplateViewName()
        {
            return "durados_Html";
        }

        public string GetTooltip(Durados.View view)
        {
            return ((View)view).GetTooltip();
        }

        public string GetTooltip(Durados.Field field)
        {
            return field.GetTooltip();
        }

        public string GetTooltip(string title, string description)
        {
            return ViewHelper.GetTooltip(title, description);
        }


        public virtual string GetDefaultFrom()
        {
            return Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
        }

        public virtual int GetAddItemsAsyncTimeout()
        {
            return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AddItemsAsyncTimeOut"] ?? "0");
        }

        protected virtual void SendCron(Cron cron, DataRow dataRow, int userId, string template)
        {
            string toColName = "to";
            string ccColName = "cc";
            string pkColName = "pk";

            if (!dataRow.Table.Columns.Contains(toColName) && dataRow.IsNull(toColName))
                return;


            string body = template.Replace(dataRow);

            string subject = cron.Subject.Replace(dataRow);

            string to = dataRow[toColName].ToString();
            string cc = string.Empty;
            string pk = dataRow[pkColName].ToString();
            if (dataRow.Table.Columns.Contains(ccColName))
                cc = dataRow[ccColName].ToString();
            string from = GetDefaultFrom();
            if (!string.IsNullOrEmpty(cron.From))
                from = cron.From;

            try
            {
                SendEmail(from, to, cc, body, subject);
                History.Save(cron.Name, pk, userId, 1, Map.GetConfigDatabase().SysDbConnectionString, Map.Version, "Admin");

            }
            catch (Exception exception)
            {
                Map.Logger.Log("Cron " + cron.Name, this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
            }

        }

        public virtual string GetSiteWithoutQueryString()
        {
            //return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;
            return Request.Url.Scheme + "://" + Request.Url.Authority;

        }

        public virtual string GetMainSiteWithoutQueryString()
        {
            //return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;
            return Maps.Instance.DuradosMap.Url;

        }

        protected virtual string GetTemplate(Cron cron)
        {
            string template = System.Web.HttpContext.Current.Server.HtmlDecode(GetTemplate(GetTemplateViewName(), cron.Template));


            template = template.Replace("[AppPath]", GetSiteWithoutQueryString());

            template = template.Replace('[', '$');

            //int index = 0;
            //while (index != -1)
            //{
            //    index = template.IndexOf('$', index);
            //    if (index != -1)
            //    {
            //        int indexBreak = template.IndexOf(']', index);
            //        int nextPoint = template.IndexOf('.', index);
            //        int indexPoint = -1;
            //        while (nextPoint < indexBreak && nextPoint != -1)
            //        {
            //            indexPoint = nextPoint;
            //            nextPoint = template.IndexOf('.', indexPoint);

            //        }
            //        if (indexPoint != -1)
            //        {
            //            template = template.Remove(index + 1, indexPoint - index);
            //        }
            //        template = template.Replace("]", "");

            //    }
            //}
            template = template.Replace("]", "");

            return template;
        }



        public virtual void SendEmail(string from, string to, string cc, string message, string subject)
        {
            SendEmail(from, to, cc, message, subject, null);
        }

        public virtual void SendEmail(string from, string to, string cc, string message, string subject, string[] attachments)
        {
            SendEmail(from, to.Split(';'), cc.Split(';'), message, subject, attachments);
        }

        public virtual void SendEmail(string from, string[] to, string[] cc, string message, string subject, string[] attachments)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            if (attachments == null)
                attachments = new string[0];

            Durados.Cms.DataAccess.Email.Send(host, Map.Database.UseSmtpDefaultCredentials, port, username, password, false, to, cc, null, subject, message, from, null, null, DontSend, attachments, Map.Database.Logger);
        }

        protected virtual UI.Json.ITree GetNewTree(View view)
        {
            return new UI.Json.Tree(view);
        }

        public virtual JsonResult TreeOneLevelNodes(string viewName, string id, string guid)
        {
            View view = GetView(viewName, "Tree");

            UI.Json.ITree tree = GetNewTree(view);

            //string[] selectedKeys = null;
            //if (initiation.HasValue && initiation.Value)
            //{
            //    selectedKeys= (string[])Map.Session["selectedKeys"];
            //}

            if (string.IsNullOrEmpty(id))
                if (!string.IsNullOrEmpty(view.TreeRoot))
                    id = view.TreeRoot;

            return Json(tree.GetFirstLevelChildren(id));
        }

        public virtual JsonResult EditTreeNode(string viewName, string guid)
        {
            //string viewName = Request.QueryString["viewName"];

            View view = GetView(viewName, "Tree");

            //TODO security
            if (!view.IsEditable(guid))
                throw new AccessViolationException();

            UI.Json.ITree tree = GetNewTree(view);

            if (view == null || Request.Form["id"] == null)
            {
                throw new DuradosException("Invalid parameters");
            }
            string treeId = Request.Form["id"];


            switch (Request.Form["operation"])
            {
                case "create_node":
                    string nodeId = tree.AddChild(treeId, Request.Form["title"], view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);
                    if (string.IsNullOrEmpty(nodeId))
                        return Json(new { status = 0 });

                    return Json(new { status = 1, id = nodeId });

                case "rename_node":
                    tree.Rename(treeId, Request.Form["title"], view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                    break;

                case "remove_node":
                    try
                    {
                        tree.Remove(treeId, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit);
                        return Json(new { status = 1, id = treeId });
                    }
                    catch (Exception exception)
                    {
                        return Json(GetDeleteTreeNode(exception, treeId));

                    }

                case "move_node":

                    if (Request.Form["ref"] == null)
                    {
                        throw new DuradosException("Invalid parameters");
                    }
                    string newId = Request.Form["ref"];
                    tree.Move(treeId, newId, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                    break;

                case "assign_to_node":

                    View itemsView = GetView(Request.Form["itemsView"], "Tree");
                    if (itemsView == null || string.IsNullOrEmpty(Request.Form["keys"]))
                    {
                        throw new DuradosException("Invalid parameters");
                    }

                    if (!itemsView.IsEditable(guid))
                        throw new AccessViolationException();

                    string[] pkArray = Request.Form["keys"].Split(',');


                    (itemsView.Base).AssignToParent(treeId, pkArray, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                    return Json(new { status = 1 });
            }

            return Json(new { status = 1, id = treeId });
        }

        protected virtual string GetDeleteTreeNode(Exception exception, string id)
        {
            return Map.Database.Localizer.Translate("The folders you are trying to delete contain items. Please assign those items to other folders and try again.");
        }
        /*
        public virtual JsonResult TreeRemoveNode(string viewName,  string pk)
        {
            View view = GetView(viewName, "Tree");

            UI.Json.ITree tree = GetNewTree(view);

            tree.Remove(pk, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit);

            return Json("Success");
        }

        public virtual JsonResult TreeRenameNode(string viewName,  string pk, string name)
        {
            View view = GetView(viewName, "Tree");

            UI.Json.ITree tree = GetNewTree(view);

            tree.Rename(pk, name, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

            return Json("Success");
        }

        
        public virtual JsonResult TreeMoveNode(string viewName,  string pk, string newParent)
        {
            View view = GetView(viewName, "Tree");

            UI.Json.ITree tree = GetNewTree(view);

            tree.Move(pk, newParent, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
            
            return Json("Success");
        }

        public virtual void NodeSelection(string viewName, string guid,  string treePk, bool isSelected)
        {
           
           if ( Map.Session["selectedKeys"] == null)
           {
               if (isSelected)
               {
                 string[] selectedKeys = new string[1]{treePk};
                 Map.Session["selectedKeys"] = selectedKeys;              
               }
           }
           else
           {
               HashSet<string> hash = null;
               string[] selectedKeys =(string[])Map.Session["selectedKeys"];
               try
               {
                hash = new HashSet<string>(selectedKeys);
               }
               catch{

                   hash = new HashSet<string>();
                   foreach(string key in selectedKeys){
                       if (!hash.Contains(key))
                           hash.Add(key);
                   }
               }
               if (isSelected){
                   if (!hash.Contains(treePk))
                   {
                       hash.Add(treePk);
                   }
               }
               else{
                   if (hash.Contains(treePk)){
                       hash.Remove(treePk);
                   }
               }
               Map.Session["selectedKeys"] = hash.ToArray();
           }
        }*/

        public virtual ActionResult Dashboard()
        {
            ViewData["TableViewer"] = GetNewTableViewer();
            return View();
        }

        public ActionResult IFrameDomain()
        {
            return View();
        }


        //public string GetConnection(string serverName, string catalog, bool? integratedSecurity, string username, string password, string duradosuserId, SqlProduct sqlProduct, int localPort, bool usesSsh, bool usesSsl)
        //{

        //    string connectionString = null;
        //    System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
        //    builder.ConnectionString = Map.connectionString;

        //    bool hasServer = !string.IsNullOrEmpty(serverName);
        //    bool hasCatalog = !string.IsNullOrEmpty(catalog);


        //    if (!hasCatalog)
        //        throw new DuradosException("Catalog Name is missing");


        //    if (integratedSecurity.HasValue && integratedSecurity.Value)
        //    {
        //        if (!hasServer)
        //        {
        //            serverName = builder.DataSource;

        //        }
        //        connectionString = "Data Source={0};Initial Catalog={1};Integrated Security=True;";
        //        return string.Format(connectionString, serverName, catalog);
        //    }
        //    else
        //    {

        //        connectionString = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;";
        //        if (sqlProduct == SqlProduct.MySql)
        //        {
        //            if (usesSsh)
        //                connectionString = "server=localhost;database={1};User Id={2};password={3};port={4};convert zero datetime=True";
        //            else
        //                connectionString = "server={0};database={1};User Id={2};password={3};port={4};convert zero datetime=True";
        //        }

        //        bool hasUsername = !string.IsNullOrEmpty(username);
        //        bool hasPassword = !string.IsNullOrEmpty(password);

        //        //if (!hasServer)
        //        //{
        //        //    if (Maps.AllowLocalConnection) 
        //        //        serverName = builder.DataSource;
        //        //    else
        //        //        throw new DuradosException("Server Name is missing");
        //        //}

        //        //if (!hasUsername)
        //        //{
        //        //    if (Maps.AllowLocalConnection) 
        //        //        username = builder.UserID;
        //        //    else
        //        //        throw new DuradosException("Username Name is missing");
        //        //}

        //        //if (!hasPassword)
        //        //{
        //        //    if (Maps.AllowLocalConnection) 
        //        //        password = builder.Password;
        //        //    else
        //        //        throw new DuradosException("Password Name is missing");
        //        //}

        //        //return string.Format(connectionString, serverName, catalog, username, password);  
        //        if (!hasServer)
        //        {
        //            if (Maps.AllowLocalConnection)
        //                serverName = builder.DataSource;
        //            else
        //                throw new DuradosException("Server Name is missing");
        //        }

        //        if (!hasUsername)
        //        {
        //            if (Maps.AllowLocalConnection)
        //                username = builder.UserID;
        //            else
        //                throw new DuradosException("Username Name is missing");
        //        }

        //        if (!hasPassword)
        //        {
        //            if (Maps.AllowLocalConnection)
        //                password = builder.Password;
        //            else
        //                throw new DuradosException("Password Name is missing");
        //        }

        //        return string.Format(connectionString, serverName, catalog, username, password, localPort);

        //    }

        //}

        public string GetConnection(string serverName, string catalog, bool? integratedSecurity, string username, string password, string duradosuserId, SqlProduct sqlProduct, int localPort, bool usesSsh, bool usesSsl)
        {

            string connectionString = null;
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = Map.connectionString;

            bool hasServer = !string.IsNullOrEmpty(serverName);
            bool hasCatalog = !string.IsNullOrEmpty(catalog);


            if (!hasCatalog)
                throw new DuradosException("Catalog Name is missing");


            if (integratedSecurity.HasValue && integratedSecurity.Value)
            {
                if (!hasServer)
                {
                    serverName = builder.DataSource;

                }
                connectionString = "Data Source={0};Initial Catalog={1};Integrated Security=True;";
                return string.Format(connectionString, serverName, catalog);
            }
            else
            {

                connectionString = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;";
                if (sqlProduct == SqlProduct.MySql)
                {
                    if (usesSsh)
                        connectionString = "server=localhost;database={1};User Id={2};password={3};port={4};convert zero datetime=True";
                    else
                        connectionString = "server={0};database={1};User Id={2};password={3};port={4};convert zero datetime=True";
                }
                if (sqlProduct == SqlProduct.Postgre)
                {
                    if (usesSsl)
                        if (usesSsh)
                            connectionString = "server=localhost;database={1};User Id={2};password={3};port={4};SSL=true;SslMode=Require;";
                        else
                            connectionString = "server={0};database={1};User Id={2};password={3};port={4};SSL=true;SslMode=Require;";
                    //connectionString = "HOST={0};DATABASE={1};USER ID={2};PASSWORD={3}";//test1.cb8bfk90dnws.us-west-2.rds.amazonaws.com;DATABASE=demo;USER ID=root;PASSWORD=Modubiz2012
                    else
                        if (usesSsh)
                        connectionString = "server=localhost;database={1};User Id={2};password={3};port={4};Encoding=UNICODE;";
                    else
                        connectionString = "server={0};database={1};User Id={2};password={3};port={4};Encoding=UNICODE;";
                    // connectionString = "HOST={0};DATABASE={1};USER ID={2};PASSWORD={3}";//
                }
                if (sqlProduct == SqlProduct.Oracle)
                {
                    //connectionString =  "POOLING=False;USER ID=yariv;PASSWORD=Back2014;DATA SOURCE=backandtrail1.cb8bfk90dnws.us-west-2.rds.amazonaws.com:1521/ORCL";

                    //Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=MyHost)(PORT=MyPort)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=MyOracleSID)));
                    //User Id = myUsername; Password = myPassword;

                    connectionString = OracleAccess.GetConnectionStringSchema();
                    //connectionString = "Data Source={0};Persist Security Info=True;User ID={2};Password={3};Unicode=False";
                    //connectionString = "POOLING=False;USER ID=yariv;PASSWORD=Back2014;DATA SOURCE=oracletrail1.cb8bfk90dnws.us-west-2.rds.amazonaws.com:1521/ORCL";
                    //connectionString = "USER ID=yariv;PASSWORD=Back2014;DATA SOURCE=oracletrail1.cb8bfk90dnws.us-west-2.rds.amazonaws.com:1521/ORCL";
                    // connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id = {2}; Password = {3};";


                }//POOLING=False;USER ID=yariv;PASSWORD=Back2014;DATA SOURCE=oracletrail1.cb8bfk90dnws.us-west-2.rds.amazonaws.com:1521/ORCL

                //        connectionString = "USER ID=itay;CONNECTION TIMEOUT=600;PASSWORD=itay123;DATA SOURCE=199.203.211.166:1521/XE;POOLING=False";


                bool hasUsername = !string.IsNullOrEmpty(username);
                bool hasPassword = !string.IsNullOrEmpty(password);

                //if (!hasServer)
                //{
                //    if (Maps.AllowLocalConnection) 
                //        serverName = builder.DataSource;
                //    else
                //        throw new DuradosException("Server Name is missing");
                //}

                //if (!hasUsername)
                //{
                //    if (Maps.AllowLocalConnection) 
                //        username = builder.UserID;
                //    else
                //        throw new DuradosException("Username Name is missing");
                //}

                //if (!hasPassword)
                //{
                //    if (Maps.AllowLocalConnection) 
                //        password = builder.Password;
                //    else
                //        throw new DuradosException("Password Name is missing");
                //}

                //return string.Format(connectionString, serverName, catalog, username, password);  
                if (!hasServer)
                {
                    if (Maps.AllowLocalConnection)
                        serverName = builder.DataSource;
                    else
                        throw new DuradosException("Server Name is missing");
                }

                if (!hasUsername)
                {
                    if (Maps.AllowLocalConnection)
                        username = builder.UserID;
                    else
                        throw new DuradosException("Username Name is missing");
                }

                if (!hasPassword)
                {
                    if (Maps.AllowLocalConnection)
                        password = builder.Password;
                    else
                        throw new DuradosException("Password Name is missing");
                }

                return string.Format(connectionString, serverName, catalog, username, password, localPort);

            }
        }

        protected virtual IDbConnection GetNewConnection(SqlProduct sqlProduct, string connectionString)
        {
            return Durados.DataAccess.DataAccessObject.GetNewConnection(sqlProduct, connectionString);
        }

        string ServernameFieldName = "ServerName";
        string CatalogFieldName = "Catalog";
        string UsernameFieldName = "Username";
        string PasswordFieldName = "Password";
        string IntegratedSecurityFieldName = "IntegratedSecurity";
        string DuradosUserFieldName = "DuradosUser";
        string ProductPortFieldName = "ProductPort";

        string SshRemoteHost = "SshRemoteHost";
        string SshPort = "SshPort";
        string SshUser = "SshUser";
        string SshPassword = "SshPassword";
        string SshUses = "SshUses";
        string ProductPort = "ProductPort";

        protected virtual void ValidateConnectionString(bool integratedSecurity, string serverName, string catalog, string username, string password, bool usesSsh, bool usesSsl, string duradosUserId, SqlProduct sqlProduct, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort)
        {
            OpenSshSessionIfNecessary(usesSsh, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort, sqlProduct);

            int port = productPort;
            if (usesSsh)
                port = localPort;
            string connectionString = GetConnection(serverName, catalog, integratedSecurity, username, password, duradosUserId, sqlProduct, port, usesSsh, usesSsl);
            IDbConnection connection = GetNewConnection(sqlProduct, connectionString);

            try
            {
                connection.Open();

            }
            catch (InvalidOperationException ex)
            {
                throw new DuradosException("Connection to Database Faild. Please check connection fields.", ex);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new DuradosException(ex.Message, ex);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new DuradosException(ex.Message, ex);
            }
            catch (ExceedLengthException ex)
            {
                throw new DuradosException(ex.Message, ex);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                CloseSshSessionIfNecessary();
            }
        }



        private void CloseSshSessionIfNecessary()
        {
            if (session != null)
                session.Close();
        }

        Durados.Security.Ssh.ISession session = null;
        int localPort = 0;
        private void OpenSshSessionIfNecessary(bool usingSsh, string sshRemoteHost, string sshUser, string sshPassword, string privateKey, int sshPort, int productPort, SqlProduct product)
        {
            if (product == SqlProduct.MySql && usingSsh)
            {
                Durados.Security.Ssh.ITunnel tunnel = new Durados.DataAccess.Ssh.Tunnel();

                tunnel.RemoteHost = sshRemoteHost;
                tunnel.User = sshUser;
                tunnel.Password = sshPassword;
                tunnel.PrivateKey = privateKey;
                tunnel.Port = sshPort;
                int remotePort = productPort;
                localPort = Maps.Instance.AssignLocalPort();

                session = new Durados.DataAccess.Ssh.ChilkatSession(tunnel, remotePort, localPort);
                session.Open(15);
            }
        }
        protected virtual DataView GetDataViewByBookmark(int bookmarkId)
        {
            int rowCount;
            return GetDataViewByBookmark(bookmarkId, 1, 10000, out rowCount);
        }
        protected virtual DataView GetDataViewByBookmark(int bookmarkId, int pageNumber, int pageSize, out int rowCount)
        {
            SqlAccess sqlAccess = new SqlAccess();
            Durados.Bookmark bookmark = BookmarksHelper.GetBookmark(bookmarkId);
            View view = GetView(bookmark.ViewName);

            //int rowCount;
            Durados.DataAccess.Filter filter = MailingServiceHelper.GetBookmarkFilter(view, bookmark, sqlAccess);
            Durados.SortDirection direction = (SortDirection)Enum.Parse(typeof(SortDirection), bookmark.SortDirection);
            return new DataView(sqlAccess.FillDataTable(view, pageNumber, pageSize, filter, bookmark.SortColumn, direction, out rowCount, null, null, null, null).Copy());
        }



        public string SubscribeBatch(int bookmarkId, int dataActionId, string listId, string apiKey, bool? ordered, string fieldName, string fields, bool? updateHistory)
        {
            try
            {
                string[] serviceFields = null; ;
                if (!string.IsNullOrEmpty(fields))
                    serviceFields = fields.Split(',');
                //else
                //    throw new DuradosException("Missing servic fields");
                Durados.Bookmark bookmark = BookmarksHelper.GetBookmark(bookmarkId);
                if (bookmark == null)
                    return Map.Database.Localizer.Translate("Bookmark was not found");

                Durados.Web.Mvc.View view = GetView(bookmark.ViewName, "SubscribeBatch");
                if (view.Database.IsConfig)
                    return Map.Database.Localizer.Translate("Bookmark view is a config view! mailing services for these views is not implemeneted.");
                Durados.Web.Mvc.View mailngServiceView = GetView("durados_MailingServiceSubscribers", "SubscribeBatch");
                DataTable subscribers = MailingServiceHelper.GetNewSubscribers(bookmarkId, view, mailngServiceView, dataActionId, serviceFields, fieldName);
                Dictionary<string, string> errors = null;
                Map.Logger.Log(GetControllerNameForLog(ControllerContext), "SubscribeBatch", string.Format("Start running action:{0} with {1} emails count", dataActionId, subscribers.Rows.Count), null, 3, null);

                MailingServiceFactory msfactory = new MailingServiceFactory();

                Durados.Services.IMailingService mailingService = msfactory.GetMailingService(dataActionId);

                errors = mailingService.SubscribeBatch(subscribers, listId, apiKey);
                //if (dataActionId == 1)
                //{
                //    Durados.Services.IMailingService mailingService = new OngageMailAccess(); //Durados.Services.MailServiceFactory.GetMailService();

                //}
                bool UpdateHistory = !(updateHistory.HasValue && updateHistory.Value == false);
                if (UpdateHistory)
                {
                    int userId = Convert.ToInt32(GetUserID());
                    MailingServiceHelper.SetNewSubscribed(bookmarkId, view, mailngServiceView, dataActionId, subscribers, errors, userId);
                }
                int errorCount = (errors == null) ? 0 : errors.Count;
                string msg = string.Format("{0} {1}, {2} {3}", subscribers.Rows.Count, Map.Database.Localizer.Translate("items were subscribed"), errorCount, Map.Database.Localizer.Translate("items had errors."));

                Map.Logger.Log(GetControllerNameForLog(ControllerContext), "SubscribeBatch", msg, null, 3, null);

                return msg;
            }
            catch (Exception exception)
            {
                return string.Format("The following error occured:<br>{0}.<br><br> The call stuck:<br><br><br>   {1} ", exception.Message.Replace("{", "{{").Replace("}", "}}"), exception.StackTrace.Replace("{", "{{").Replace("}", "}}"));
            }

        }

        public JsonResult GetAvailableGrobootNotificationMessages()
        {
            string role = Map.Database.GetUserRole();
            if (!(role == "Developer" || role == "Admin"))
            {
                return Json(new { success = false, description = "User Not Allowed" });
            }

            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["GroBootBaseurl"];
            Durados.DataAccess.GrooBoot.GroBootPushAppNotification groboot = new DataAccess.GrooBoot.GroBootPushAppNotification(baseUrl);
            try
            {
                string accessKey = Database.GrobootNotificationAccessKey.Decrypt(Database);
                return Json(new { success = true, messages = groboot.GetAvailableMessages(accessKey, Map.Database.Logger) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, description = ex.Message });
            }

        }

        public JsonResult GetSelectedGrobootNotificationMessages(int bookmarkId)
        {
            string role = Map.Database.GetUserRole();
            if (!(role == "Developer" || role == "Admin"))
            {
                return Json(new { success = false, description = "User Not Allowed" });
            }
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["GroBootBaseurl"];
            Durados.DataAccess.GrooBoot.GroBootPushAppNotification groboot = new DataAccess.GrooBoot.GroBootPushAppNotification(baseUrl);

            try
            {
                string accessKey = Database.GrobootNotificationAccessKey.Decrypt(Database);
                return Json(new { success = true, selectedIds = groboot.GetBookmarkMessages(accessKey, bookmarkId, Map.Database.Logger) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, description = ex.Message });
            }
        }

        public JsonResult SetBookmarkGrobootNotificationMessages(int bookmarkId, FormCollection messages)
        {
            string role = Map.Database.GetUserRole();
            if (!(role == "Developer" || role == "Admin"))
            {
                return Json(new { success = false, description = "User Not Allowed" });
            }
            Bookmark bookmark = BookmarksHelper.GetBookmark(bookmarkId);
            if (bookmark == null)
                return Json(new { success = false, description = "Bookmark does not exists" });
            Durados.Web.Mvc.View view = GetView(bookmark.ViewName, "SetBookmarkGrobootNotificationMessages");
            if (view == null)
                return Json(new { success = false, description = "Bookmark view does not exists" });
            //          "DeviceId" OR  ( "PushToken" AND "DeviceType" ) 
            IEnumerable<string> displayNames = view.Fields.Values.Select(r => r.DisplayName);
            if (!(displayNames.Contains("DeviceId") || (displayNames.Contains("PushToken") && displayNames.Contains("DeviceType"))))
                return Json(new { success = false, description = "In order to update GroBoot pushApps notification this bookmark must contain 'DeviceId' or  both PushToken' and 'DeviceType' fields" });
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["GroBootBaseurl"];
            Durados.DataAccess.GrooBoot.GroBootPushAppNotification groboot = new DataAccess.GrooBoot.GroBootPushAppNotification(baseUrl);
            try
            {
                int[] messageIds = messages["messages"].Split(',').Select(r => Convert.ToInt32(r)).ToArray();//values[""].Split(',').ToList<string>().Select(r=>Convrt.ToInt32(r))
                string accessKey = Database.GrobootNotificationAccessKey.Decrypt(Database);
                groboot.SetMessagesForBookmark(accessKey, bookmarkId, messageIds);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, description = ex.Message });
            }
        }
        public JsonResult ValidateBookmarkGrobootNotificationView(int bookmarkId)
        {
            string role = Map.Database.GetUserRole();
            if (!(role == "Developer" || role == "Admin"))
            {
                return Json(new { success = false, description = "User Not Allowed" });
            }
            Bookmark bookmark = BookmarksHelper.GetBookmark(bookmarkId);
            if (bookmark == null)
                return Json(new { success = false, description = "Bookmark does not exists" });
            Durados.Web.Mvc.View view = GetView(bookmark.ViewName, "SetBookmarkGrobootNotificationMessages");
            if (view == null)
                return Json(new { success = false, description = "Bookmark view does not exists" });
            //          "DeviceId" OR  ( "PushToken" AND "DeviceType" ) 
            IEnumerable<string> displayNames = view.Fields.Values.Select(r => r.DisplayName);
            if (!(displayNames.Contains("DeviceId") || (displayNames.Contains("PushToken") && displayNames.Contains("DeviceType"))))
                return Json(new { success = false, description = "In order to update GroBoot pushApps notification this bookmark must contain 'DeviceId' or  both PushToken' and 'DeviceType' fields" });
            else
                return Json(new { success = true });
        }
        public JsonResult RunAction(string url)
        {
            string masterGuid = Map.Guid == null? null : Map.Guid.ToString();
            string userGuid = Map.Database.GetUserGuid();

            var client = new  RestSharp.RestClient(Maps.ApiUrls[0]);
            client.Authenticator = new  RestSharp.HttpBasicAuthenticator(masterGuid, userGuid);

            var request = new RestSharp.RestRequest();
            request.Resource = url;
            try
            {
                var response = client.Execute(request);
                if(response.ErrorException != null)
                    return Json(new { success = false, error = response.ErrorMessage, exception = response.ErrorException, response = response.Content });
                return Json(new { success = true, response = response.Content });

            }
            catch(Exception ex)
            {
                string username = GetUsername();
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(),null,ex,3, "username=" + username.Replace("'", "\""),DateTime.Now);
                return Json(new { success = false, error = ex.Message  });

            }
            
            
            
        }
    }


    
    public static class NameValueCollectionExtention
    {
        public static System.Web.Routing.RouteValueDictionary ToRouteValues(this System.Collections.Specialized.NameValueCollection col, Object obj)
        {
            var values = new System.Web.Routing.RouteValueDictionary(obj);
            if (col != null)
            {
                foreach (string key in col)
                {
                    //values passed in object override those already in collection
                    if (!string.IsNullOrEmpty(key))
                        if (!values.ContainsKey(key) && key.IndexOf("column", StringComparison.CurrentCultureIgnoreCase) < 0 && key.IndexOf("dashboardId", StringComparison.CurrentCultureIgnoreCase) < 0 && key.IndexOf("id", StringComparison.CurrentCultureIgnoreCase) < 0)
                            values[key] = col[key];
                }
            }
            return values;
        }
    }
    internal enum FilterTextType
    {
        FieldName,
        FieldDisplayName,
        DatabaseName
    }

    public enum ImportType
    {
        Merg,
        Replace

    }
}
