using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mobix.Localization
{
    public class Localizer
    {
        private static Durados.Localization.LocalizationConfig localizationConfig;

        static Localizer()
        {
            localizationConfig = new Durados.Localization.LocalizationConfig();
            localizationConfig.AddKeyIfMissing = true;
            localizationConfig.ReturnKeyIfMissing = true;
            localizationConfig.Postfix = string.Empty;
            localizationConfig.Prefix = string.Empty;
            localizationConfig.TranslateKeyIfMissing = true;
            localizationConfig.DefaultLanguage = "en-us";
            if (!Durados.Localization.Localizer.IsInitiated)
            {
                string localizationSchemaGeneratorFileName = System.Web.HttpContext.Current.Server.MapPath("~/Localization/Localization.sql");
                Durados.Localization.Localizer.Initiate(System.Configuration.ConfigurationManager.ConnectionStrings["CmsDBForLocalization"].ConnectionString, localizationSchemaGeneratorFileName);
            }

        }

        public static string Translate(string key)
        {
            if (localizationConfig == null)
                return key;
            else
                return Durados.Localization.Localizer.Translate(Durados.Web.Localization.Language.GetCurrentUserLanguageCode(), key, localizationConfig);
        }

        public static List<Durados.Localization.Model.Language> Languages
        {
            get
            {
                return Durados.Localization.Localizer.Languages;
            }
        }
    }
}
