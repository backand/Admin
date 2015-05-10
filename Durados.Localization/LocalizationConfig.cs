using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Localization
{
    public class LocalizationConfig
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public bool AddKeyIfMissing { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public bool ReturnKeyIfMissing { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string Prefix { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string Postfix { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Title { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool TranslateKeyIfMissing { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool TranslateKeyOrDefaultLanguage { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DefaultLanguage { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string LocalizationSchemaGeneratorFileName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string LocalizationConnectionStringKey { get; set; }
        
        public LocalizationConfig()
        {
            Title = "Default";
            //// deployment sql
            //string configPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["configPath"] ?? "~/Config/");
            //LocalizationSchemaGeneratorFileName = "~/Deployment/Sql/Localization.sql";
            //if (configPath.StartsWith("~"))
            //    LocalizationSchemaGeneratorFileName = configPath + "Sql/Localization.sql";
            //else
            //    LocalizationSchemaGeneratorFileName = configPath + @"Sql\Localization.sql";
            LocalizationConnectionStringKey = "LocalizationConnectionString";
        }

        public enum DirectionType
        {
            LTR,
            RTL
        }
    }
}
