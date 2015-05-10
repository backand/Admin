using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Localization
{
    public class Localizer
    {
        public static string Translate(string key)
        {
            Durados.Localization.LocalizationConfig localizationConfig = Map.Database.Localization;
            if (localizationConfig == null)
                return key;
            else
                return Durados.Localization.Localizer.Translate(Durados.Web.Mvc.Localization.Language.GetCurrentUserLanguageCode(), key, localizationConfig);
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
