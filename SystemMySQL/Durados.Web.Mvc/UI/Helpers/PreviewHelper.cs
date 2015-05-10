using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Collections.Specialized;
using Durados.DataAccess;

namespace Durados.Web.Mvc.UI.Helpers
{
    

    public class PreviewHelper
    {
        public const string PageGuid = "xxPageXx_";

        public void EditPreview(string configViewName, string property, string value, string pk, string guid, out Durados.Web.Mvc.View view)
        {

            Durados.Web.Mvc.Map previewMap = Maps.Instance.GetMap();
            Durados.DataAccess.ConfigAccess configAccess = new DataAccess.ConfigAccess();
            view = null;

            if (configViewName == "Page" && guid == PageGuid)
            {
                try
                {
                    view = null;
                    Page page = Maps.Instance.GetMap().Database.Pages[Convert.ToInt32(pk)];
                    System.Reflection.PropertyInfo propertyInfo = page.GetType().GetProperty(property);
                    object value2;
                    if (propertyInfo.PropertyType.BaseType.FullName == "System.Enum")
                    {
                        value2 = Enum.Parse(propertyInfo.PropertyType, value);
                    }
                    else
                    {
                        value2 = Convert.ChangeType(value, propertyInfo.PropertyType);
                    }
                    propertyInfo.SetValue(page, value2, null);
                    Save((View)previewMap.GetConfigDatabase().Views["Page"], property, value2, pk);
                    
                }
                catch (Exception ex)
                {
                    throw new DuradosException("Failed to set value to field property.", ex);
                }

            }
            else if (configViewName == "View")
            {
                string viewName = configAccess.GetViewNameByPK(pk, previewMap.GetConfigDatabase().ConnectionString);
                if (string.IsNullOrEmpty(viewName) )
                {
                   // previewMap.Logger.Log("Admmin", "PreviewEdit", "EditPreview", null, 15, "viewName are null or empty.");
                    throw new DuradosException("viewName are null or empty or not exists.");
                }

                if (!previewMap.Database.Views.ContainsKey(viewName) )
                {
                    //previewMap.Logger.Log("Admmin", "PreviewEdit", "EditPreview", null, 15, "viewName are not contained in Views.");
                    throw new DuradosException("viewName are not contained in Views.");
                }
                
                    view = (Durados.Web.Mvc.View)previewMap.Database.Views[viewName];
                   
                    try
                    {
                        System.Reflection.PropertyInfo propertyInfo = view.GetType().GetProperty(property);
                        object value2;
                        if (propertyInfo.PropertyType.BaseType.FullName == "System.Enum")
                        {
                            value2 = Enum.Parse(propertyInfo.PropertyType, value); 
                        }
                        else
                        {
                            value2 = Convert.ChangeType(value, propertyInfo.PropertyType);
                        }
                        propertyInfo.SetValue(view, value2, null);
                        if (guid == PageGuid)
                        {
                            Save((View)previewMap.GetConfigDatabase().Views["View"], property, value2, pk);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new DuradosException("Failed to set value to field property.", ex);
                    }
                        
            }
            else if (configViewName == "Field")
            {
                string fieldName =configAccess.GetFieldNameByPK(pk,previewMap.GetConfigDatabase().ConnectionString);
                string viewPK=configAccess.GetViewPKByFieldPK(pk,previewMap.GetConfigDatabase().ConnectionString);
                string viewName=configAccess.GetViewNameByPK(viewPK,previewMap.GetConfigDatabase().ConnectionString);
                if(string.IsNullOrEmpty(viewName) || string.IsNullOrEmpty(fieldName))
                {
                    throw new DuradosException("fieldName or viewName are null or empty.");
                    //previewMap.Logger.Log("Admmin","PreviewEdit","EditPreview",null,15,"fieldName or viewName are null or empty.");
                }

                if (!previewMap.Database.Views.ContainsKey(viewName) || !previewMap.Database.Views[viewName].Fields.ContainsKey(fieldName))
                {
                    throw new DuradosException("fieldName or viewName are not contained in file.");
                    //previewMap.Logger.Log("Admmin", "PreviewEdit", "EditPreview", null, 15, "fieldName or viewName are not contained in file.");
                }
                
                Durados.Field field = previewMap.Database.Views[viewName].Fields[fieldName];
              
                try
                {
                    System.Reflection.PropertyInfo propertyInfo = field.GetType().GetProperty(property);
                    object value2;
                    if (propertyInfo.PropertyType.BaseType.FullName == "System.Enum")
                    {
                        value2 = Enum.Parse(propertyInfo.PropertyType, value);
                    }
                    else
                    {
                        value2 = Convert.ChangeType(value, propertyInfo.PropertyType);
                    }
                    propertyInfo.SetValue(field, value2, null);
                }
                catch (Exception ex)
                {
                    throw new DuradosException("Failed to set value to field property.", ex);
                }
                    
            }
        }

        private static void Save(View view, string property, object value, string pk)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(property, value);
            view.Edit(values, pk, null, null, null, null);
            Durados.DataAccess.ConfigAccess.SaveConfigDataset(view.Database.ConnectionString, view.Database.Logger);
                 
        }

        //public static void SetPreviewModeOn(string appName)
        //{

        //    //HttpContext.Session["AdminPreviewMode"] = true;
        //    if (HttpContext.Current.Session["AdminPreviewMap"] == null)
        //        HttpContext.Current.Session["AdminPreviewMap"] = Maps.Instance.CreateMap(appName);
        //}
        //public static void SetPreviewModeOff(string appName)
        //{
        //    if (HttpContext.Current.Session["AdminPreviewMap"] != null) HttpContext.Current.Session["AdminPreviewMap"] = null;
        //    ////HttpContext.Session["AdminPreviewMode"] = true;
        //    //if (HttpContext.Current.Session["AdminPreviewMap"] == null)
        //    //    HttpContext.Current.Session["AdminPreviewMap"] = Maps.Instance.CreateMap(appName);
        //}
        
    }
}
