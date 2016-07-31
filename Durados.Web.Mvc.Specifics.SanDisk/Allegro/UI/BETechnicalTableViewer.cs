using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class BETechnicalTableViewer : Durados.Web.Mvc.UI.TableViewer
    {
        Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers.AllegroBETechnicalController controller;
        public BETechnicalTableViewer(Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers.AllegroBETechnicalController controller)
            : base() 
        {
            this.controller = controller;
        }

        
        public override bool IsEditable(Field field, System.Data.DataRow row, string guid)
        {
            return base.IsEditable(field, row, guid) && IsEditable((View)field.View, row);
        }

        Dictionary<string, bool> editables = new Dictionary<string, bool>();
        public bool IsEditable(View view, System.Data.DataRow row)
        {
            string pk = row["Id"].ToString();
            if (!editables.ContainsKey(pk))
                editables.Add(pk, !controller.IsDisabled(view, pk));

            return editables[pk];
        }

        public override string GetEditCaption(Durados.View view, DataRow row, string guid)
        {
            return ((View)view).IsEditable(guid) && IsEditable(view) && IsEditable((View)view, row) ? "Edit" : "View";
        }
    }
}
