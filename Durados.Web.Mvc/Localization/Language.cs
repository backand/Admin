using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Localization
{
    public class Language
    {
        public static string GetCurrentUserLanguageCode()
        {
            string languageCode = GetCurrentUserLanguageCodeFromSession();

            if (languageCode == null)
                languageCode = GetCurrentUserLanguageCodeFromCockie();
            else
                return languageCode;

            if (languageCode == null)
                languageCode = GetCurrentUserLanguageCodeFromBrowserCulture();
            else
            {
                SetCurrentUserLanguageCodeIntoSession(languageCode);
                return languageCode;
            }

            if (languageCode == null)
            {
                languageCode = GetDefaultLanguageCode();
                SetCurrentUserLanguageCodeIntoCockie(languageCode);
                SetCurrentUserLanguageCodeIntoSession(languageCode);
                return languageCode;
            }
            else
            {
                SetCurrentUserLanguageCodeIntoCockie(languageCode);
                SetCurrentUserLanguageCodeIntoSession(languageCode);
                return languageCode;
            }
            
        }

        public static void SetCurrentUserLanguageCode(string languageCode)
        {
            SetCurrentUserLanguageCodeIntoCockie(languageCode);
            SetCurrentUserLanguageCodeIntoSession(languageCode);
        }

        private static string GetCurrentUserLanguageCodeFromSession()
        {
            if (System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session["languageCode"] == null)
                return null;
            else
                return System.Web.HttpContext.Current.Session["languageCode"].ToString();
        }

        private static void SetCurrentUserLanguageCodeIntoSession(string languageCode)
        {
            if (System.Web.HttpContext.Current.Session != null)
                System.Web.HttpContext.Current.Session["languageCode"] = languageCode;
        }

        private static void SetCurrentUserLanguageCodeIntoCockie(string languageCode)
        {
        }

        private static string GetCurrentUserLanguageCodeFromCockie()
        {
            return null;
        }

        private static string GetCurrentUserLanguageCodeFromBrowserCulture()
        {
            return null;
        }

        private static string GetDefaultLanguageCode()
        {
            return "en-US";
        }

        public static List<Durados.Localization.Model.Language> Languages
        {
            get
            {
                return Durados.Localization.Localizer.Languages;
            }
        }

        public static string Direction
        {
            get
            {
                try
                {
                    string direction = Durados.Localization.Localizer.GetDirection(GetCurrentUserLanguageCode());
                    if (direction == Durados.Localization.LocalizationConfig.DirectionType.RTL.ToString())
                        return direction;
                    else
                        return Durados.Localization.LocalizationConfig.DirectionType.LTR.ToString();
                }
                catch
                {
                    return Durados.Localization.LocalizationConfig.DirectionType.LTR.ToString();
                }
            }
        }

    }
}
