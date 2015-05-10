using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Localization
{
    public class Language
    {
        Localizer localizer;

        public Language(Localizer localizer)
        {
            this.localizer = localizer;
        }

        public string UserLanguageCode
        {
            get
            {

                if (Languages == null || Languages.Where(l => l.Active).Count() == 0)
                    return GetDefaultLanguageCode();

                if (Languages.Where(l => l.Active).Count() == 1)
                    return Languages.Where(l => l.Active).First().Code;

                string defaultLanguageCode = null;
                if (HttpContext.Current.Session == null || HttpContext.Current.Session["languageCode"] == null)
                {
                    //string languageCode = null;
                    //languageCode = GetCurrentUserLanguageCodeFromCookie();

                    //if (languageCode == null)
                    //    languageCode = GetCurrentUserLanguageCodeFromBrowserCulture();
                    defaultLanguageCode = GetDefaultLanguageCode();
                    SetCurrentUserLanguageCode(GetDefaultLanguageCode());
                }
                if (HttpContext.Current.Session == null)
                {
                    if (defaultLanguageCode == null)
                        defaultLanguageCode = GetDefaultLanguageCode();
                    return defaultLanguageCode;
                }
                else
                {
                    return HttpContext.Current.Session["languageCode"].ToString();
                }
            }
        }

        public string Direction
        {
            get
            {
                return GetLanguageDirection(UserLanguageCode);

                //if (HttpContext.Current.Session == null || HttpContext.Current.Session["languageDir"] == null)
                //{
                //    HttpContext.Current.Session["languageDir"] = GetDefaultLanguageDir();
                //}
                //return HttpContext.Current.Session["languageDir"].ToString();
            }
        }

        private string GetLanguageDirection(string LangCode)
        {
            string direction = localizer.GetDirection(LangCode).ToUpper();
            if (direction == string.Empty)
            {
                return GetDefaultLanguageDir();
            } 
            else if (direction != Durados.Localization.LocalizationConfig.DirectionType.RTL.ToString())
            {
                Durados.Localization.LocalizationConfig.DirectionType.LTR.ToString();
            }
            return direction;
        }

        public void SetCurrentUserLanguageCode(string languageCode)
        {
            SetCurrentUserLanguageCodeIntoSession(languageCode);
            //SetCurrentUserLanguageCodeIntoCookie(languageCode);
        }

        private void SetCurrentUserLanguageCodeIntoSession(string languageCode)
        {
            if (System.Web.HttpContext.Current.Session != null)
            {                
                HttpContext.Current.Session["languageDir"] = GetLanguageDirection(languageCode);

                HttpContext.Current.Session["languageCode"] = languageCode;
            }
        }

        private void SetCurrentUserLanguageCodeIntoCookie(string languageCode)
        {
        }

        private string GetCurrentUserLanguageCodeFromCookie()
        {
            return null;// "pt-br";
        }

        private string GetCurrentUserLanguageCodeFromBrowserCulture()
        {
            return null;
        }

        private string GetDefaultLanguageCode()
        {
            //if (System.Configuration.ConfigurationManager.AppSettings["DefaultLangCode"] != null)
            //    return System.Configuration.ConfigurationManager.AppSettings["DefaultLangCode"];
            if (localizer.GetDefaultLangCode() != null) return localizer.GetDefaultLangCode();
            return "en-us"; //"he-IL";
        }

        private string GetDefaultLanguageDir()
        {
            //if (System.Configuration.ConfigurationManager.AppSettings["DefaultLangDir"] != null)
            //    return System.Configuration.ConfigurationManager.AppSettings["DefaultLangDir"];
            return Durados.Localization.LocalizationConfig.DirectionType.LTR.ToString();
        }


        public List<Durados.Localization.Model.Language> Languages
        {
            get
            {
                return localizer.Languages;
            }
        }



    }
}
