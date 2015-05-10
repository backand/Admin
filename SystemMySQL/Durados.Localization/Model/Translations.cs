using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Localization.Model
{
    public class Translations
    {
        private Dictionary<int, Language> languagesByID;
        private Dictionary<string, Language> languagesByCode;

        internal protected Translations(LocalizationDataSet localizationDataSet)
        {
            languagesByID = new Dictionary<int, Language>();
            languagesByCode = new Dictionary<string, Language>();
            foreach (LocalizationDataSet.Durados_LanguageRow languageRow in localizationDataSet.Durados_Language)
            {
                Language language = new Language(languageRow);
                AddLanguage(languageRow.ID, languageRow.Code, language);

                //languagesByID.Add(languageRow.ID, language);
                //if (!languagesByCode.ContainsKey(languageRow.Code))
                //    languagesByCode.Add(languageRow.Code, language);
            }
        }

        public void AddLanguage(int langID, string langCode, Language lang)
        {
            languagesByID.Add(langID, lang);
            if (!languagesByCode.ContainsKey(langCode))
                languagesByCode.Add(langCode, lang);
        }

        public Language GetLangByID(int langID)
        {
            if (languagesByID.ContainsKey(langID))
                return languagesByID[langID];

            return null;
        }


        public List<Language> Languages
        {
            get
            {
                return languagesByCode.Values.ToList();
            }
        }

        public string this[int languageID, int translationKeyID]
        {
            get
            {
                if (languagesByID.ContainsKey(languageID))
                    return languagesByID[languageID][translationKeyID];
                else
                    return null;
            }
        }

        public string this[int languageID, string key]
        {
            get
            {
                if (languagesByID.ContainsKey(languageID))
                    return languagesByID[languageID][key];
                else
                    return null;
            }
        }

        public string this[string languageCode, int translationKeyID]
        {
            get
            {
                if (languagesByCode.ContainsKey(languageCode.ToLower()))
                    return languagesByCode[languageCode.ToLower()][translationKeyID];
                else
                    return null;
            }
        }

        public string this[string languageCode, string key]
        {
            get
            {
                if (languagesByCode.ContainsKey(languageCode.ToLower()))
                    return languagesByCode[languageCode.ToLower()][key];
                else
                    return null;
            }
        }

        public string GetDirection(string languageCode)
        {
            if (languagesByCode.ContainsKey(languageCode.ToLower()))
                return languagesByCode[languageCode.ToLower()].Direction;
            else
                return LocalizationConfig.DirectionType.LTR.ToString();
        }

        public int? GetLanguageID(string languageCode)
        {
            if (languagesByCode.ContainsKey(languageCode.ToLower()))
                return languagesByCode[languageCode.ToLower()].ID;
            else
                return null;
        }

        public string GetLanguageCode(int languageID)
        {
            if (languagesByID.ContainsKey(languageID))
                return languagesByID[languageID].Code;
            else
                return null;
        }
    }
}
