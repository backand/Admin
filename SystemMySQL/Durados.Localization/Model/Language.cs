using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Localization.Model
{
    public class Language
    {
        private Dictionary<int, string> translationsByID;
        private Dictionary<string, string> translationsByKey;
        private LocalizationDataSet.Durados_LanguageRow languageRow;

        internal protected Language(LocalizationDataSet.Durados_LanguageRow languageRow)
        {
            this.languageRow = languageRow;
            translationsByID = new Dictionary<int, string>();
            translationsByKey = new Dictionary<string, string>();
            foreach (LocalizationDataSet.Durados_TranslationRow translationRow in languageRow.GetDurados_TranslationRows())
            {
                string translation = translationRow.Translation;
                AddTranslation(translationRow.Durados_TranslationKeyRow.Key, translationRow.TranslationKeyID, translation);
                //translationsByID.Add(translationRow.TranslationKeyID, translation);
                //translationsByKey.Add(translationRow.Durados_TranslationKeyRow.Key, translation);
            }
        }

        public Language()
        {
            translationsByID = new Dictionary<int, string>();
            translationsByKey = new Dictionary<string, string>();
        }

        public void AddTranslation(string key, int keyID, string translation)
        {
            translationsByID.Add(keyID, translation);
            translationsByKey.Add(key.ToLower(), translation);
        }

        public string this[int translationKeyID]
        {
            get
            {
                if (translationsByID.ContainsKey(translationKeyID))
                    return translationsByID[translationKeyID];
                else
                    return null;
            }
        }

        public string this[string key]
        {
            get
            {
                if (translationsByKey.ContainsKey(key))
                    return translationsByKey[key];
                else
                    return null;
            }
        }

        public int ID
        {
            get
            {
                return languageRow.ID;
            }
        }


        public string Direction
        {
            get
            {
                if (languageRow.Direction == null) 
                    return "LTR";
                else
                    return languageRow.Direction;
            }
        }

        public string Code
        {
            get
            {
                return languageRow.Code;
            }
        }

        public string Name
        {
            get
            {
                return languageRow.Name;
            }
        }

        public string NativeName
        {
            get
            {
                return languageRow.NativeName;
            }
        }

        public bool Active
        {
            get
            {
                return languageRow.Active;
            }
        }
    }
}
