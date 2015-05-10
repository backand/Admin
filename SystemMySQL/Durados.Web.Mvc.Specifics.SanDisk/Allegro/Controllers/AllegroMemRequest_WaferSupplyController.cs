using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Json;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Membership;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroMemRequest_WaferSupplyController : AllegroHomeController
    {
        protected override void DropDownFilter(Durados.Web.Mvc.ParentField parentField, ref string sql)
        {
            if (parentField.View.Name != "MemRequest_WaferSupply")
                return;

            if (parentField.ParentView.Name != "v_WaferSupply")
                return;

            int? memRequestID = GetMemRequestID();

            if (memRequestID.HasValue)
            {
                int? mdeID = GetMdeID(memRequestID.Value);

                if (mdeID.HasValue)
                {
                    string filter = GetWaferSupplyFilter(mdeID.Value, sql);

                    if (!string.IsNullOrEmpty(filter))
                    {
                        sql += filter;
                    }

                }
            }
        }

        private int? GetMdeID(int id)
        {
            string viewName = "v_MemRequest";

            if (Map.Database.Views.ContainsKey(viewName))
            {
                View view = (View)Map.Database.Views[viewName];

                string mdeIdFieldName = "FK_v_MDE_v_MemRequest_AllocatedMem_Parent";

                if (view.Fields.ContainsKey(mdeIdFieldName))
                {
                     DataRow row = view.GetDataRow(id.ToString());
                     if (row != null)
                     {
                         Field field = view.Fields[mdeIdFieldName];
                         object value = field.GetValue(row);
                         if (value != null && value.ToString() != string.Empty)
                             return Convert.ToInt32(value);
                     }
                }
            }

            return null;
        }

        private string GetWaferSupplyFilter(int mdeID, string sql)
        {
            if (sql.ToLower().Contains(" where "))
                return string.Format(" and MDEId = {0}", mdeID);
            else
                return string.Format(" where MDEId = {0}", mdeID);
        }

        private int? GetMemRequestID()
        {
            Durados.Web.Mvc.UI.Json.Filter jsonFilter = ViewHelper.ConvertQueryStringToJsonFilter(currentGuid);

            if (jsonFilter == null)
                return null;

            string fkFieldName = "MemoryRequestId";
            if (!jsonFilter.Fields.ContainsKey(fkFieldName))
            {
                return null;
            }
            object value = jsonFilter.Fields[fkFieldName].Value;

            if (value == null || value.ToString() == string.Empty)
                return null;

            return Convert.ToInt32(jsonFilter.Fields[fkFieldName].Value);
        }

 
    }
}
