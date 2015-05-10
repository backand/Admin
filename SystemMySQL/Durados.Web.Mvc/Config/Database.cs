using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Config
{
    public class Database : Durados.Web.Mvc.Database, Durados.Config.IConfigDatabase
    {
        public Database(DataSet dataSet, Map map) :
            base(dataSet, map)
        {
        }

        protected override void MvcInit()
        {
            base.MvcInit();

            DefaultController = "Admin";
        }



        protected override Durados.View CreateView(DataTable dataTable)
        {
            return new Durados.Web.Mvc.Config.View(dataTable, this);
        }

        public override bool IsConfig
        {
            get
            {
                return true;
            }
        }

        protected override bool GetIsMultiLanguages()
        {
            return Map.Database.IsMultiLanguages;
        }

        protected override Durados.Localization.LocalizationConfig GetLocalization()
        {
            return Map.Database.Localization;
        }

    }
}
