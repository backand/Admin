using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Localization
{
    public class Localizer : Durados.Localization.ILocalizer
    {
        private Durados.Localization.LocalizationConfig localizationConfig = null;

        Durados.Localization.Localizer localizer = null;
        Language language = null;

        public Localizer()
        {
            language = new Language(this);
            localizer = new Durados.Localization.Localizer();
        }

        public string GetDirection(string languageCode)
        {
            return localizer.GetDirection(languageCode);
        }

        public Durados.Localization.Model.LocalizationDataSet LocalizationDataSet
        {
            get
            {
                return localizer.DataSet;
            }
        }

        public bool IsInitiated
        {
            get
            {
                return localizer.IsInitiated;
            }
        }

        public void Refresh()
        {
            localizer.Refresh();
            
        }

        public Language Language
        {
            get
            {
                return language;
            }
        }

        public string Direction
        {
            get
            {
                return language.Direction;
            }
        }

        public void SetCurrentUserLanguageCode(string languageCode)
        {
            language.SetCurrentUserLanguageCode(languageCode);
        }

        public void InitLocalizer(Durados.Localization.LocalizationConfig config, string cs, string localizationSchemaGeneratorFileName)
        {
            /*
            localizationConfig = new Durados.Localization.LocalizationConfig();
            localizationConfig.AddKeyIfMissing = true;
            localizationConfig.ReturnKeyIfMissing = true;
            localizationConfig.Postfix = string.Empty;
            localizationConfig.Prefix = string.Empty;
            localizationConfig.TranslateKeyIfMissing = false;
            localizationConfig.TranslateKeyOrDefaultLanguage = false;
            localizationConfig.DefaultLanguage = "en-us";
            */

            localizer.Initiate(cs, localizationSchemaGeneratorFileName);

            if (config == null) return;

            localizationConfig = config;

            if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["languageCode"] == null)
            {
                string lang = config.DefaultLanguage;
                if (Languages == null || Languages.Where(l => l.Active).Count() == 0)
                    lang = config.DefaultLanguage;
                else if (Languages.Where(l => l.Active).Count() >= 1)
                    lang = Languages.Where(l => l.Active).First().Code;

                language.SetCurrentUserLanguageCode(lang);
            }

            if (!localizer.IsInitiated)
            {
                //string localizationSchemaGeneratorFileName = System.Web.HttpContext.Current.Server.MapPath(localizationConfig.LocalizationSchemaGeneratorFileName);
                bool useAppPath = Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["UseAppPath"]);
                if (useAppPath)
                    localizationConfig.LocalizationConnectionStringKey = Durados.Web.Infrastructure.General.GetRootName() + "_LocalizationConnectionString";

                try
                {
                    localizer.Initiate(System.Configuration.ConfigurationManager.ConnectionStrings[localizationConfig.LocalizationConnectionStringKey].ConnectionString, localizationSchemaGeneratorFileName);
                }
                catch
                {
                    localizationConfig = null;
                }
            }

        }

        public void UnsetLocalizationConfig()
        {
            localizationConfig = null;
        }

        public string GetDefaultLangCode()
        {
            if (localizationConfig == null) return null;


            var lang = Languages.FirstOrDefault(l => l.Active && l.Code == localizationConfig.DefaultLanguage);
            if (lang == null)
            {
                lang = Languages.FirstOrDefault(l => l.Active);
                if (lang == null)
                    return localizationConfig.DefaultLanguage;
                else
                    return lang.Code;
            }
            else
                return lang.Code;
            
        }

        public string Translate(string key)
        {
            if (localizationConfig == null)
                return key;
            else
                return localizer.Translate(language.UserLanguageCode, key, localizationConfig);
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
