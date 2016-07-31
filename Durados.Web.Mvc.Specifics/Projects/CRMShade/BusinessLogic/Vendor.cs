using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Specifics.Projects.CRMShade;
using Durados.Web.Membership;

namespace Durados.Web.Mvc.Specifics.Projects.CRMShade.BusinessLogic
{
    public class Vendor
    {
        public Vendor()
        {
        }

        public Json.VendorInfo GetVendorInfo(string pk)
        {
            View vendorView = (View)Durados.Web.Mvc.Map.Database.Views[ShadeViews.V_Vendor.ToString()];

            Json.VendorInfo vendorInfo = new Durados.Web.Mvc.Specifics.Projects.CRMShade.BusinessLogic.Json.VendorInfo();

            DataRow vendorRow = vendorView.GetDataRow(pk);

            //ColumnField multiplyField = (ColumnField)vendorView.Fields[Durados.Web.Mvc.Specifics.Projects.CRMShade.Organization.Multiply.ToString()];
            ColumnField multiplyField = (ColumnField)vendorView.Fields[V_Vendor.Multiply.ToString()];
            string s = multiplyField.GetValue(vendorRow);
            double multiply = 1;
            if (!string.IsNullOrEmpty(s))
            {
                multiply = Convert.ToDouble(s);
            }

            vendorInfo.Multiply = multiply;


            return vendorInfo;
        }

    }
}
