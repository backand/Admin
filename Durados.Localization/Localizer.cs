using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;

namespace Durados.Localization
{
    public class Localizer
    {
        private Model.LocalizationDataSet localizationDataSet;

        public Localizer()
        {
        }

        public void BuildSchema(string connectionString, string localizationSchemaGeneratorFileName)
        {
            if (SchemaExists(connectionString))
                return;

            FileInfo file = new FileInfo(localizationSchemaGeneratorFileName);
            string script = file.OpenText().ReadToEnd();
            SqlConnection conn = new SqlConnection(connectionString);
            script = script.Replace("Durados_Localization", conn.Database);
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand();

                command.Connection = conn;
                //command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = script;
                command.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        private bool SchemaExists(string connectionString)
        {
            string sql = "SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Durados_Translation]') AND name = N'IX_Durados_Translation_Key_Language'";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand();

                command.Connection = conn;
                //command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = sql;
                return command.ExecuteScalar() != null && command.ExecuteScalar() != DBNull.Value;
            }
            finally
            {
                conn.Close();
            }
        }

        private Model.LocalizationDataSet GetLocalizationDataSet(string connectionString)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            localizationDataSet = new Durados.Localization.Model.LocalizationDataSet();
            
            Model.LocalizationDataSetTableAdapters.Durados_LanguageTableAdapter languageTableAdapter =
                new Durados.Localization.Model.LocalizationDataSetTableAdapters.Durados_LanguageTableAdapter();

            languageTableAdapter.Connection = connection;
            languageTableAdapter.Fill(localizationDataSet.Durados_Language);


            Model.LocalizationDataSetTableAdapters.Durados_TranslationKeyTableAdapter translationKeyTableAdapter =
                            new Durados.Localization.Model.LocalizationDataSetTableAdapters.Durados_TranslationKeyTableAdapter();

            translationKeyTableAdapter.Connection = connection;
            try
            {
                translationKeyTableAdapter.Fill(localizationDataSet.Durados_TranslationKey);
            }
            catch (System.Data.ConstraintException exception)
            {
                string constraintName = GetViolatedConstraint(localizationDataSet.Durados_TranslationKey);
                throw new Exception(constraintName, exception);
            }

            Model.LocalizationDataSetTableAdapters.Durados_TranslationTableAdapter translationTableAdapter =
                                        new Durados.Localization.Model.LocalizationDataSetTableAdapters.Durados_TranslationTableAdapter();

            translationTableAdapter.Connection = connection;
            translationTableAdapter.Fill(localizationDataSet.Durados_Translation);


            return localizationDataSet;
        }

        public Model.LocalizationDataSet DataSet
        {
            get
            {
                return localizationDataSet;
            }
        }

        public Model.Translations GetTranslations(string connectionString)
        {
            Model.LocalizationDataSet localizationDataSet = GetLocalizationDataSet(connectionString);
            Model.Translations translations = new Durados.Localization.Model.Translations(localizationDataSet);

            return translations;
        }

        public string GetTranslation(Model.Translations translations, int languageID, int translationKeyID)
        {
            return translations[languageID, translationKeyID];
        }

        public string GetTranslation(Model.Translations translations, int languageID, string key)
        {
            return translations[languageID, key.ToLower()];
        }

        public string GetTranslation(Model.Translations translations, string languageCode, int translationKeyID)
        {
            return translations[languageCode, translationKeyID];
        }

        public string GetTranslation(Model.Translations translations, string languageCode, string key)
        {
            return translations[languageCode, key.ToLower()];
        }

        public int? GetKeyID(string connectionString, string key)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "select ID from Durados_TranslationKey where [key] = @key";
                    SqlCommand command = new SqlCommand(sql, connection);

                    SqlParameter parameter = new SqlParameter("@key", System.Data.SqlDbType.NVarChar);
                    parameter.Value = key;

                    command.Parameters.Add(parameter);

                    object scalar = command.ExecuteScalar();

                    if (scalar == null || scalar == DBNull.Value)
                        return null;
                    else
                        return Convert.ToInt32(scalar);
                }
            }
            catch
            {
                return null;
            }
        }

        private string GetViolatedConstraint(System.Data.DataTable dataTable)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (dataTable.HasErrors)
            {
                foreach (DataRow row in dataTable.GetErrors())
                {
                    sb.AppendLine(row.RowError);
                }
            }

            return "In table " + dataTable.TableName + " " + sb.ToString();
        }

        public int? AddKey(string connectionString, string key)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlParameter parameter = new SqlParameter("@key", key);
                    
                    string sql = "insert into Durados_TranslationKey ([key]) values (@key); select @@IDENTITY";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.Add(parameter);
                    object scalar = command.ExecuteScalar();
                    if (scalar == null || scalar == DBNull.Value)
                        return null;
                    else
                    {
                        return Convert.ToInt32(scalar);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public int? AddTranslation(string connectionString, int keyID, string languageCode, string key, string translation)
        {
            try
            {
                int languageID = GetLanguageID(languageCode).Value;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "insert into Durados_Translation ([TranslationKeyID], [LanguageID], [Translation]) values (@key, @lang, @translation); select @@IDENTITY";
                    SqlCommand command = new SqlCommand(sql, connection);

                    SqlParameter pKey = new SqlParameter("@key", keyID);
                    SqlParameter pLang = new SqlParameter("@lang", languageID);
                    SqlParameter pTranslation = new SqlParameter("@translation", translation);

                    command.Parameters.Add(pKey);
                    command.Parameters.Add(pLang);
                    command.Parameters.Add(pTranslation);

                    object scalar = command.ExecuteScalar();
                    if (scalar == null || scalar == DBNull.Value)
                        return null;
                    else
                    {
                        Durados.Localization.Model.Language lang = translations.GetLangByID(languageID);
                        
                        if (lang != null)
                        {
                            lang.AddTranslation(key, keyID, translation);
                        }

                        return Convert.ToInt32(scalar);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private Model.Translations translations;
        private string connectionString;
        private bool initiated = false;

        //static Localizer()
        //{
        //    localizer = new Localizer();
        //}

        public void Initiate(string connectionString, string localizationSchemaGeneratorFileName)
        {
            if (initiated) { Refresh(); return; }
            this.connectionString = connectionString;
            BuildSchema(connectionString, localizationSchemaGeneratorFileName);
            initiated = true;
            Refresh();
        }

        public virtual bool IsInitiated
        {
            get
            {
                return initiated;
            }
        }

        public virtual void Refresh()
        {
            translations = GetTranslations(connectionString);
        }

        public string Translate(int languageID, int translationKeyID)
        {
            if (translations == null)
                return null;
            return GetTranslation(translations, languageID, translationKeyID);
        }

        public string Translate(int languageID, string key)
        {
            if (translations == null)
                return null;
            return GetTranslation(translations, languageID, key);
        }

        public string Translate(string languageCode, int translationKeyID)
        {
            if (translations == null)
                return null;
            return GetTranslation(translations, languageCode, translationKeyID);
        }

        public string Translate(string languageCode, string key)
        {
            if (translations == null)
                return null;
            return GetTranslation(translations, languageCode, key);
        }


        public string Translate(int languageID, string key, bool addKeyIfMissing, bool returnKeyIfMissing, string prefix, string postfix, bool translateKeyIfMissing, bool translateKeyOrDefaultLanguage, string defaultLanguage)
        {
            return Translate(GetLanguageCode(languageID), key, addKeyIfMissing, returnKeyIfMissing, prefix, postfix, translateKeyIfMissing, translateKeyOrDefaultLanguage, defaultLanguage);
        }

        public string Translate(string languageCode, string key, LocalizationConfig config)
        {
            if (config == null)
                return Translate(languageCode, key, false, true, "{{", "}}", false, false, null);
            return Translate(languageCode, key, config.AddKeyIfMissing, config.ReturnKeyIfMissing, config.Prefix, config.Postfix, config.TranslateKeyIfMissing, config.TranslateKeyOrDefaultLanguage, config.DefaultLanguage);
        }

        public string Translate(string languageCode, string key, bool addKeyIfMissing, bool returnKeyIfMissing, string prefix, string postfix, bool translateKeyIfMissing, bool translateKeyOrDefaultLanguage, string defaultLanguage)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            string translation = Translate(languageCode, key);

            if (translation == string.Empty)
            {
                return key; //prefix + key + postfix;
            }
            else if (translation == null)
            {
                if (translateKeyOrDefaultLanguage && !string.IsNullOrEmpty(defaultLanguage))
                {
                    int? keyID = GetKeyID(connectionString, key);
                    if (!keyID.HasValue)
                        keyID = AddKey(connectionString, key);
                    if (keyID.HasValue)
                    {
                        string defaultLanguageTranslation = GetTranslation(translations, defaultLanguage, key);
                        string source = defaultLanguage;
                        string target = languageCode;
                        if (source != target)
                            translation = Translate(defaultLanguageTranslation, source, target);
                        else
                            translation = defaultLanguageTranslation;

                        if (!string.IsNullOrEmpty(defaultLanguageTranslation))
                        {
                            AddTranslation(connectionString, keyID.Value, languageCode, key, translation);
                        }
                        return translation;
                    }

                }
                else if (translateKeyIfMissing)
                {
                    int? keyID = GetKeyID(connectionString, key);
                    if (!keyID.HasValue)
                        keyID = AddKey(connectionString, key);
                    if (keyID.HasValue)
                    {
                        string source = defaultLanguage;
                        string target = languageCode;
                        if (source != target)
                            translation = Translate(key, source, target);
                        else
                            translation = key;
                        AddTranslation(connectionString, keyID.Value, languageCode, key, translation);
                        return translation;
                    }
                }
                else if (addKeyIfMissing) // Add Key And Empty Translation
                {
                    int? keyID = GetKeyID(connectionString, key);
                    if (!keyID.HasValue)
                        keyID = AddKey(connectionString, key);

                    if (keyID.HasValue)
                    {
                        AddTranslation(connectionString, keyID.Value, languageCode, key, string.Empty);
                        //Refresh();
                    }
                }

                if (string.IsNullOrEmpty(translation))  //if (returnKeyIfMissing)
                {
                    return prefix + key + postfix;
                }
                else
                {
                    return translation;
                }
            }
            else
            {
                return translation;
            }
        }

        private string Translate(string text, string sourceLanguageCode , string targetLanguageCode)
        {
           // return Microsoft.Translator.Translator.Translate(sourceLanguageCode, targetLanguageCode, text);
            return null;
        }

        public string GetDirection(string languageCode)
        {
            if (translations == null)
                return string.Empty;

            return translations.GetDirection(languageCode);
        }

        public string GetLanguageCode(int languageID)
        {
            return translations.GetLanguageCode(languageID);
        }

        public int? GetLanguageID(string languageCode)
        {
            return translations.GetLanguageID(languageCode);
        }

        public Model.LocalizationDataSet LocalizationDataSet
        {
            get
            {
                return DataSet;
            }
        }

        public List<Durados.Localization.Model.Language> Languages
        {
            get
            {
                if (translations == null)
                    return null;
                return translations.Languages;
            }
        }
    }

    public interface ILocalizer
    {
        string Translate(string key);

        string Direction { get; }

        void UnsetLocalizationConfig();

        void SetCurrentUserLanguageCode(string languageCode);

        Durados.Localization.Model.LocalizationDataSet LocalizationDataSet { get; }

        List<Durados.Localization.Model.Language> Languages { get; }

        void InitLocalizer(LocalizationConfig localizationConfig, string cs, string localizationSchemaGeneratorFileName);
       
        bool IsInitiated {get;}
       
        void Refresh();
    }
}
