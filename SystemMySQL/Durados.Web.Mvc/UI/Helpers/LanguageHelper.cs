using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.Web.Mvc;

namespace System.Web.Mvc
{
    public static class LanguageHelper
    {
        public static List<SelectListItem> GetLanguages(this HtmlHelper html)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            Durados.Web.Mvc.Database database = Maps.Instance.GetMap().Database;
            Durados.Web.Localization.Localizer localizer = (Durados.Web.Localization.Localizer)database.Localizer;
            
            if (localizer.Languages != null)
            {
                foreach (Durados.Localization.Model.Language language in localizer.Languages.Where(l=>l.Active || database.Localization.DefaultLanguage == l.Code))
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = language.Code;
                    item.Text = language.NativeName;
                    item.Selected = localizer.Language.UserLanguageCode == language.Code;

                    selectList.Add(item);
                }
            }
            return selectList;
        }


        public static string FormatLocalized(this string str, params object[] args)
        {
            Durados.Localization.ILocalizer localizer = Maps.Instance.GetMap().Database.Localizer;
            if (str == null) return string.Empty;
            str = localizer.Translate(str);
            try
            {
                return String.Format(str, args);
            }
            catch (FormatException) { } //TODO Log exception
            return str;
        }

        public static int GetLanguagesCount(this HtmlHelper html)
        {
            Durados.Web.Localization.Localizer localizer = (Durados.Web.Localization.Localizer)Maps.Instance.GetMap().Database.Localizer;
            if (localizer.Languages != null)
                return localizer.Languages.Count;
            else
                return 0;
        }
        
    }
}
