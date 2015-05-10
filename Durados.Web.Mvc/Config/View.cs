using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Config
{
    public class View : Durados.Web.Mvc.View, Durados.Config.IConfigView
    {
        public List<Field> SettingsFields { get; private set; }

        public View(DataTable dataTable, Durados.Web.Mvc.Config.Database database) :
            base(dataTable, database)
        {
            SettingsFields = new List<Field>();
        }

        public View(DataTable dataTable, Durados.Web.Mvc.Config.Database database, string name) :
            base(dataTable, database, name)
        {
            SettingsFields = new List<Field>();
        }

        public override string GetWorkspaceName()
        {
            return "Admin";
        }

        public override bool HasMessages()
        {
            return false;
        }
    }
}
