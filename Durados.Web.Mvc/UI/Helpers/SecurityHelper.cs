using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class SecurityHelper
    {
        private static Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        /*
         * Changed by Yossi Lalum - 03/02/11
         * 
         * Added allowRoles parameters here,
         * 
         * and allowRoles in View and Field and defaults values in Database. 
         * 
         * and added config values in project class
         * 
         * [bugit - 438]
         * 
         */
        public static bool IsInRole(string role)
        {
            if (Map.Database.HasRoles)
            {
                return Map.Database.IsInRole(role);
            }
            else
            {
                return System.Web.HttpContext.Current.User.IsInRole(role);
            }

        }

        public static bool IsDenied(string denyRoles, string allowRoles)
        {
            string[] roles;

            if (IsInRole("Developer"))
            {
                return false;
            } 
            else if (string.IsNullOrEmpty(allowRoles))
            {
                return true;
            }
            else if (allowRoles.ToLower() == "everyone")
            {
                if (string.IsNullOrEmpty(denyRoles))
                {
                    return false;
                }
                else
                {
                    roles = denyRoles.Split(',');
                    foreach (string role in roles)
                    {
                        if (IsInRole(role.Trim()))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
            else
            {
                roles = allowRoles.Split(',');
                foreach (string role in roles)
                {
                    if (IsInRole(role.Trim()))
                    {
                        return false;
                    }
                }

                return true;

            }        
        }

        public static string[] GetCurrentUserRoles()
        {
            if (!System.Web.Security.Roles.Enabled)
                return new string[0];
            return System.Web.Security.Roles.GetRolesForUser();
        }

        public static string GetCurrentUserRole()
        {
            if (!System.Web.Security.Roles.Enabled)
                return string.Empty;
            
            string[] roles = System.Web.Security.Roles.GetRolesForUser();
            if (roles.Length > 0)
                return roles[0];
            else
                return string.Empty;
        }

        public static bool IsConfigViewForViewOwner(View view)
        {
            return view.Database.IsConfig && IsAllowForViewOwner(view);
            //string viewName = view.Name;
            //string pk = HttpContext.Current.Request.QueryString["Pk"];
            //if (!string.IsNullOrEmpty(pk) && (viewName == "View" || viewName == "Field"))
            //{
            //    Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
            //    string dataViewName = configAccess.GetViewNameByPK(pk, Map.GetConfigDatabase().ConnectionString);

            //    if (string.IsNullOrEmpty(dataViewName))
            //        throw new DuradosException("viewName are null or empty or not exists.");

            //    Durados.Web.Mvc.View viewDb = (Durados.Web.Mvc.View)Map.Database.Views[dataViewName];
            //    if (viewDb != null && viewDb.IsViewOwner() && view.ViewOwnerRoles.Split(',').Contains(Durados.Web.Mvc.Config.Project.ViewOwenrRole))
            //    {
            //        return true;
            //    }

            //}
            //return false;
        }

        public static bool IsConfigViewForViewOwner(Page page)
        {
            return page.Database.IsConfig && IsAllowForViewOwner(page);
            
        }

        public static bool IsConfigFieldForViewOwner(Field field)
        {
            return IsAllowForViewOwner(field) && !(((View)field.View).SystemView && IsDenied(field.DenyCreateRoles, "everyone"));
            //string viewName = field.View.Name;
            //string pk = HttpContext.Current.Request.QueryString["Pk"];
            //if (!string.IsNullOrEmpty(pk) && (viewName == "View" || viewName == "Field"))
            //{
            //    Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
            //    if (viewName == "Field")
            //    {
            //        string fieldName = configAccess.GetFieldNameByPK(pk, Map.GetConfigDatabase().ConnectionString);
            //        pk = configAccess.GetViewPKByFieldPK(pk, Map.GetConfigDatabase().ConnectionString);

            //        if (string.IsNullOrEmpty(fieldName))
            //        {
            //            throw new DuradosException("fieldName are null or empty.");
            //        }

            //    }

            //    string dataViewName = configAccess.GetViewNameByPK(pk, Map.GetConfigDatabase().ConnectionString);

            //    if (string.IsNullOrEmpty(dataViewName))
            //        throw new DuradosException("viewName are null or empty or not exists.");

            //    Durados.Web.Mvc.View viewDb = (Durados.Web.Mvc.View)Map.Database.Views[dataViewName];
            //    if (viewDb != null && viewDb.IsViewOwner() && field.AllowEditRoles.Split(',').Contains(Durados.Web.Mvc.Config.Project.ViewOwenrRole))
            //    {
            //        return true;
            //    }

            //}

            //return false;

        }
        private static bool IsAllowForViewOwner(object obj)
        {
            if (SecurityHelper.IsInRole("Admin"))
                return false;

            if (HttpContext.Current.User == null || HttpContext.Current.User.Identity == null || string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                return false;
            if (obj is Page)
            {
                return true;
            }
            string viewName = (obj is Field) ? ((Field)obj).View.Name : ((View)obj).Name;
            string pk = HttpContext.Current.Request.QueryString["Pk"];

            //Only can access View, Field, Category
            //if (viewName != "View" && viewName != "Field" & viewName != "Category" && viewName != "Menu")
            //    throw new DuradosException( viewName + " is not allowed by view owner.");

            if (string.IsNullOrEmpty(pk) && HttpContext.Current.Request.UrlReferrer == null)
                return false;
            
            string dataViewName = null;
            if (string.IsNullOrEmpty(pk) && HttpContext.Current.Request.UrlReferrer.Segments.Length == 4)
            {
                dataViewName = HttpContext.Current.Request.UrlReferrer.Segments[3];
            }
            else if (string.IsNullOrEmpty(pk) && viewName == "Field")
            {
                if (HttpContext.Current.Request.UrlReferrer != null && HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["url"] != null)
                {
                    try
                    {
                        string[] s = HttpUtility.UrlDecode(HttpUtility.ParseQueryString(HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["url"]).ToString()).Split('?')[1].Split('&')[0].Split('=');
                        if (s[0] == "Fields")
                            pk = s[1];
                        viewName = "View";
                    }
                    catch { }
                }
            }
            // only relevant in view properties for view owner
            if ((!string.IsNullOrEmpty(pk) || !string.IsNullOrEmpty(dataViewName)) && (viewName == "View" || viewName == "Field"))
            {
                if (!string.IsNullOrEmpty(pk))
                {
                    pk = pk.TrimEnd('#');
                    Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                    if (viewName == "Field")
                    {
                        string fieldName = configAccess.GetFieldNameByPK(pk, Map.GetConfigDatabase().ConnectionString);
                        pk = configAccess.GetViewPKByFieldPK(pk, Map.GetConfigDatabase().ConnectionString);

                        if (string.IsNullOrEmpty(fieldName))
                        {
                            throw new DuradosException("fieldName are null or empty.");
                        }

                    }

                    dataViewName = configAccess.GetViewNameByPK(pk, Map.GetConfigDatabase().ConnectionString);
                }

                if (string.IsNullOrEmpty(dataViewName))
                    throw new DuradosException("viewName are null or empty or not exists.");

                if (Map.Database.Views.ContainsKey(dataViewName))
                {
                    Durados.Web.Mvc.View viewDb = (Durados.Web.Mvc.View)Map.Database.Views[dataViewName];
                    if (viewDb != null && viewDb.IsViewOwner())
                    {
                        if ((obj is Field) && ((Field)obj).AllowEditRoles.Split(',').Contains(Durados.Web.Mvc.Config.Project.ViewOwenrRole))
                        {
                            return true;
                        }
                        else if ((obj is View) && ((View)obj).ViewOwnerRoles.Split(',').Contains(Durados.Web.Mvc.Config.Project.ViewOwenrRole))
                        {
                            return true;
                        }
                        else
                            return false;

                    }
                }
            }

            return true;

        }

        public static bool IsAllowForView(View view,string viewsName, string denyRoles, string allowRoles)
        {
            if (!viewsName.Split(',').Contains(view.Name))
                return true;
            return !IsDenied(denyRoles, allowRoles) || IsConfigViewForViewOwner(view) ||  Maps.IsSuperDeveloper(null) ;
            
        }
    
        // private List<Field> GetViewOwnerAdminFields(Mvc.View view)
        //{
            
        //    return view.Fields.Values.Where(f=> f.AllowEditRoles.Split(',').Contains(Durados.Web.Mvc.Config.Project.ViewOwenrRole)).ToList();
        //}

        public static bool IsLogo(System.Collections.Specialized.NameValueCollection queryString)
        {
            return true;
        }

        public static string  GetUserGuidFromTmpGuid(string tmpGuid)
        {
            // TODO:  add specific behaior for Super user
            Durados.DataAccess.SqlAccess sqlAccess = new DataAccess.SqlAccess();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", tmpGuid);
            parameters.Add("@userGuid", Guid.NewGuid());
            return CallValidUserGuidSP(parameters, "durados_IsValidGuid", "@userGuid");
            
        }
        public static string GetTmpUserGuidFromGuid(string guid,int minutes=15)
        {
            // TODO:  add specific behaior for Super user
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            
            parameters.Add("@userGuid", guid);
            parameters.Add("@timeSpan", minutes);
            parameters.Add("@id", Guid.NewGuid());
            return CallValidUserGuidSP(parameters, "durados_SetValidGuid", "@id");

        }

        private static string CallValidUserGuidSP(Dictionary<string, object> parameters,string spName,string retParamName)
        {
            Durados.DataAccess.SqlAccess sqlAccess = Maps.MainAppSqlAccess;
           List<string> outputsParams= new List<string>();
            outputsParams.Add(retParamName);
            System.Data.IDataParameter[] sqlParameters = sqlAccess.ExecuteProcedure(Maps.Instance.ConnectionString, spName, parameters, outputsParams, null);
            if (sqlParameters.Length > 0)
            {
                foreach (System.Data.IDataParameter parameter in sqlParameters)
                    if (parameter.ParameterName == retParamName)
                        return parameter.Value == null ? null : parameter.Value.ToString();
            }
            return null;
        }
    }

    
}
