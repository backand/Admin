using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.Web.Mvc;
using Durados.Web.Mvc.Infrastructure;
using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace System.Web.Mvc
{
    public static class CmsHelper
    {
        private static Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        public static void SetHtml(string key, string value)
        {
            Durados.Web.Mvc.View htmlView = GetHtmlView();
            DataRow htmlRow = htmlView.GetDataRow(key);

            if (htmlRow == null)
            {
                if (htmlView.AllowCreate)
                {
                    Dictionary<string, object> values = new Dictionary<string, object>();
                    values.Add("Name", key);
                    values.Add("Text", value);
                    htmlView.Create(values);
                }
                else
                {
                    htmlView.AllowCreate = true;
                    try
                    {
                        Dictionary<string, object> values = new Dictionary<string, object>();
                        values.Add("Name", key);
                        values.Add("Text", value);
                        htmlView.Create(values);
                    }
                    catch { };
                    htmlView.AllowCreate = false;
                }
            }
            else
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("Text", value);

                htmlView.Edit(values, key, null, null, null, null);
            }

            
        }

        public static string GetContent(this HtmlHelper helper, string key)
        {
            return GetContent(key);
        }

        public static string GetContent(string key)
        {
            string content = GetHtml(key);

            if (string.IsNullOrEmpty(content))
            {
                Durados.Web.Mvc.View htmlView = GetMainHtmlView();
                DataRow htmlRow = htmlView.GetDataRow(key);

                if (htmlRow == null)
                    return string.Empty;

                content = HttpContext.Current.Server.HtmlDecode(htmlView.GetDisplayValue(GetHtmlFieldName(), htmlRow));
            }

            return content;
        }

        public static string GetHtml(string key)
        {
            Durados.Web.Mvc.View htmlView = GetHtmlView();
            DataRow htmlRow = htmlView.GetDataRow(key);

            if (htmlRow == null)
                return string.Empty;

            return HttpContext.Current.Server.HtmlDecode(htmlView.GetDisplayValue(GetHtmlFieldName(), htmlRow));
        }

        public static string GetHtml(this HtmlHelper helper, string key)
        {
            return GetHtml(key);
        }

        private static string GetHtmlViewName()
        {
            return "durados_Html";
        }

        private static Durados.Web.Mvc.View GetHtmlView()
        {
            return (Durados.Web.Mvc.View)Map.Database.Views[GetHtmlViewName()];
        }

        private static Durados.Web.Mvc.View GetMainHtmlView()
        {
            return (Durados.Web.Mvc.View)Maps.Instance.DuradosMap.Database.Views[GetHtmlViewName()];
        }


        private static string GetHtmlFieldName()
        {
            return "Text";
        }

        
    }
}
