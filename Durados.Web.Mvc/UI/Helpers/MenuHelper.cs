using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Durados.Web.Mvc.Infrastructure;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class MenuHelper
    {
        public static Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }
          
        private static void AddClickMenuItem(View view, IDictionary<ToolbarActionNames, string> actionsMenu, ToolbarActionNames key, string onclick, string iconClass = "")
        {
            string title = view.GetActionTooltipDescription(key);
            string caption = view.GetActionTooltipTitle(key);
            string span = string.IsNullOrEmpty(iconClass) ? string.Empty : "<span class=\"" + iconClass + "\"></span>";
            string menuItem = string.Format(@"<a href=""#"" onclick=""{0}"" title=""{1}"">{2}{3}</a>", onclick, title, span, caption);

            actionsMenu.Add(key, menuItem);
        }

        private static void AddHrefMenuItem(View view, IDictionary<ToolbarActionNames, string> actionsMenu, ToolbarActionNames key, string href, string iconClass = "")
        {
            string title = view.GetActionTooltipDescription(key);
            string caption = view.GetActionTooltipTitle(key);
            string span = string.IsNullOrEmpty(iconClass) ? string.Empty : "<span class=\"" + iconClass + "\"></span>";
            string menuItem = string.Format(@"<a href=""{0}"" title=""{1}"">{2}{3}</a>", href, title, span, caption);

            actionsMenu.Add(key, menuItem);
        }

        public static IDictionary<ToolbarActionNames, string> GetMoreActionsMenu(this View view, string guid, TableViewer tableViewer, UrlHelper UrlHelper)
        {
            IDictionary<ToolbarActionNames, string> actionsMenu = new Dictionary<ToolbarActionNames, string>();

            bool isViewDisabled = view.IsDisabled(guid);
            bool hasHistoryAction = view.SaveHistory && tableViewer.IsEditable(view);
            bool hasExportAction = view.ExportToCsv && !isViewDisabled;
            bool isImportable = view.ImportFromExcel && !Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsDenied(view.DenyEditRoles, view.AllowEditRoles) && !Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsDenied(view.DenyCreateRoles, view.AllowCreateRoles);
            bool hasImportAction = isImportable && !isViewDisabled;
            bool hasPrintAction = view.Print && !isViewDisabled;
            bool hasMessagesAction = view.HasMessages();
            bool hasCopyConfigAction = view is Durados.Config.IConfigView && Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") && view.Name == "View";
            bool hasDatabaseRoles = view is Durados.Config.IConfigView && (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin")) && (view.Name == "Database" || view.Name == "View");
            bool hasDiagnoseAction = view is Durados.Config.IConfigView && Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") && (view.Name == "Database" || view.Name == "View");
            bool hasSyncAllAction = view is Durados.Config.IConfigView && (Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Developer") || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsInRole("Admin")) && (view.Name == "View");

            if (hasHistoryAction)
            {
                string historyGuid = "History_" + ShortGuid.Next() + "_";
                string historyUrl = UrlHelper.Action("HistoryFilter", "History", new { viewName = "durados_v_ChangeHistory" });
                string historyView = view.Name;
                string onclick = "History('" + historyUrl + "', '" + guid + "', '" + historyGuid + "', '" + historyView + "');return false;";
                if (view.Base.Name != null) { historyView = view.Base.Name; }

                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.HISTORY, onclick);
            }

            if (hasExportAction)
            {
                string iconClass = "icon export";
                string href = view.GetExportToCsvUrl() + "?guid=" + guid;
                AddHrefMenuItem(view, actionsMenu, ToolbarActionNames.EXPORT, href, iconClass);
            }

            if (hasImportAction)
            {
                string onclick = "Excel.Import('" + view.Name + "','" + guid + "');return false;";
                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.IMPORT, onclick);
            }

            if (hasPrintAction)
            {
                string onclick = "Print('" + guid + "');return false;";
                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.PRINT, onclick);
            }

            if (hasMessagesAction)
            {
                string onclick = "Messages.Show('" + view.Name + "','" + guid + "');return false;";
                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.MESSAGE_BOARD, onclick);
            }

            if (hasCopyConfigAction)
            {
                string onclick = "showCopyDialog('" + guid + "', this); return false;";
                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.COPY_CONFIG, onclick);

                onclick = "showCloneDialog('" + guid + "'); return false;";
                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.CLONE_CONFIG, onclick);
            }

            if (hasDatabaseRoles)
            {
                string sendConfigUrl = UrlHelper.Action("SendConfig", "Admin");
                string onclick = "sendConfig('" + sendConfigUrl + "');";
                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.SEND_CONFIG, onclick);

                //BR: TODO
                //<a target="_blank" href="<%= downloadConfigUrl%>" title="<%= title%>">
                //<%=Map.Database.Localizer.Translate("Download")%></a>
                string href = UrlHelper.Action("DownloadConfig", "Admin");
                AddHrefMenuItem(view, actionsMenu, ToolbarActionNames.DOWNLOAD_CONFIG, href);

                onclick = "UploadConfig.Open('Database', '" + guid + "')";
                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.UPLOAD_CONFIG, onclick);
            }
            if (hasDiagnoseAction)
            {
                
                string onclick = "Diagnostics.Diagnose('" + guid + "');";
                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.DIAGNOSE, onclick);

                string dictionaryUrl = UrlHelper.Action("Index", "Block", new { parameters = "xxx" });
                onclick = "Dictionary('" + dictionaryUrl + "', '" + guid + "');";
                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.DICTIONARY, onclick);
            }

            if (hasSyncAllAction)
            {
                string onclick = "syncAll()";
                AddClickMenuItem(view, actionsMenu, ToolbarActionNames.SYNC_ALL, onclick);

            }

            return actionsMenu;
        }

        public static bool HideSettings()
        {
            string role = ((Database)Map.Database).GetUserRole().ToLower();
            if (role == "developer" && Map.IsDebug())
            {
                Map.Session["MenuDisplayState"] = true;
                return false;
            }

            object displayState = Map.Session["MenuDisplayState"];
            if (displayState == null)
            {
                if (role == "admin")
                {
                    Map.Session["MenuDisplayState"] = true;
                    return false;
                }
                return false;
            }
            else
            {
                return !(bool)displayState;
            }

        }

        public static Workspace GetCurrentWorkspace()
        {
            return GetCurrentWorkspace(null);
        }

        public static Workspace GetCurrentWorkspace(string viewName)
        {
            Durados.Workspace workspace = null;

            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["menuId"]))
            {
                int menuId = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["menuId"].TrimEnd('#'));

                workspace = Map.Database.GetWorkspaceByMenu(menuId);

                if (workspace != null)
                    return workspace;
            }
            else
            {
                if (!string.IsNullOrEmpty(viewName))
                {
                    foreach (Workspace w in Map.Database.Workspaces.Values)
                    {
                        SpecialMenu menu = w.GetSpecialMenu(viewName);
                        if (menu != null)
                            return w;
                    }
                }
            }

            object o = Map.Session["workspaceId"];
            if (o == null)
            {
                workspace = Map.Database.GetDefaultWorkspace();
                if (workspace == null)
                {
                    if (!string.IsNullOrEmpty(viewName) && Map.Database.Views.ContainsKey(viewName) && Map.Database.Workspaces.ContainsKey(Map.Database.Views[viewName].WorkspaceID))
                    {
                        workspace = Map.Database.Workspaces[Map.Database.Views[viewName].WorkspaceID];
                    }
                    else if (!string.IsNullOrEmpty(viewName) && Map.GetConfigDatabase().Views.ContainsKey(viewName))
                    {
                        workspace = Map.Database.Workspaces[Map.Database.GetAdminWorkspaceId()];
                    }
                    else
                        workspace = Map.Database.GetPublicWorkspace();
                }

                //if (workspace == null)
                //    workspace = Map.Database.Workspaces[Map.Database.GetPublicWorkspaceId()];
            }
            else
            {
                int workspaceId = Convert.ToInt32(o);
                if (Map.Database.Workspaces.ContainsKey(workspaceId))
                    workspace = Map.Database.Workspaces[workspaceId];
                else
                    workspace = Map.Database.Workspaces[Map.Database.GetPublicWorkspaceId()];
            }

            


            return workspace;
        }

        /*
        public static Workspace GetCurrentWorkspace(string viewName)
        {
            Durados.Workspace workspace = null;
            if (!string.IsNullOrEmpty(viewName) && Map.Database.Views.ContainsKey(viewName) && Map.Database.Workspaces.ContainsKey(Map.Database.Views[viewName].WorkspaceID))
            {
                workspace = Map.Database.Workspaces[Map.Database.Views[viewName].WorkspaceID];
            }
            else if (!string.IsNullOrEmpty(viewName) && Map.GetConfigDatabase().Views.ContainsKey(viewName))
            {
                workspace = Map.Database.Workspaces[Map.Database.GetAdminWorkspaceId()];
            }
            else
            {
                object o = Map.Session["workspaceId"];
                if (o == null)
                {
                    workspace = Map.Database.GetDefaultWorkspace();
                    if (workspace == null)
                        workspace = Map.Database.GetWorkspace("Public");
                    //if (workspace == null)
                    //    workspace = Map.Database.Workspaces[Map.Database.GetPublicWorkspaceId()];
                }
                else
                {
                    int workspaceId = Convert.ToInt32(o);
                    if (Map.Database.Workspaces.ContainsKey(workspaceId))
                        workspace = Map.Database.Workspaces[workspaceId];
                    else
                        workspace = Map.Database.Workspaces[Map.Database.GetPublicWorkspaceId()];
                }
            }

            return workspace;
        }
        */
        public static bool IsHidden(this SpecialMenu menu)
        {
            string role = ((Database)Map.Database).GetUserRole().ToLower();
            if (role == "developer" && Map.IsDebug())
                return false;

            if (menu.DisplayState)
            {
                object displayState = Map.Session["MenuDisplayState"];
                if (displayState == null)
                {
                    if (role == "admin")
                    {
                        Map.Session["MenuDisplayState"] = false;
                        return false;
                    }
                    return false;
                }
                else
                {
                    return !(bool)displayState;
                }
            }
            else
            {
                return false;
            }
        }

    }


}
