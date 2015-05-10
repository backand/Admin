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
using Durados.Web.Mvc.Specifics.Shade.CRM;
using Durados.Web.Membership;

namespace Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic
{
    public class Product
    {
        public Product()
        {
        }

        public Json.ProductInfo GetProductInfo(string pk)
        {
            View productView = (View)Durados.Web.Mvc.Map.Database.Views[ShadeViews.Product.ToString()];

            Json.ProductInfo productInfo = new Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic.Json.ProductInfo();

            DataRow productRow = productView.GetDataRow(pk);

            ColumnField descriptionField = (ColumnField)productView.Fields[Durados.Web.Mvc.Specifics.Shade.CRM.Product.Description.ToString()];
            string description = descriptionField.GetValue(productRow);

            productInfo.Description = description;

            ParentField productPriceCategoryField = (ParentField)productView.Fields[Durados.Web.Mvc.Specifics.Shade.CRM.Product.FK_Product_ProductPriceCategory_Parent.ToString()];

            string productPriceCategoryId = productPriceCategoryField.GetValue(productRow);

            View productPriceView = (Durados.Web.Mvc.View)Map.Database.Views[ShadeViews.ProductPrice.ToString()];

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(ProductPrice.FK_ProductPrice_ProductCategory_Parent.ToString(), productPriceCategoryId);

            Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
            string sortColumn = ProductPrice.Width.ToString();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                sortColumns.Add(sortColumn, SortDirection.Asc);
            }

            int rowCount = 0;
            DataView dataView = productPriceView.FillPage(1, 10000, values, false, sortColumns, out rowCount, null, null);

            List<Json.Price> prices = new List<Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic.Json.Price>();

            foreach (DataRow productPriceRow in dataView.Table.Rows)
            {
                double cost = Convert.ToDouble(productPriceRow[ProductPrice.Cost.ToString()]);
                double pprice = (productPriceRow[ProductPrice.Price.ToString()] == DBNull.Value) ? 0 : Convert.ToDouble(productPriceRow[ProductPrice.Price.ToString()]);
                double width = Convert.ToDouble(productPriceRow[ProductPrice.Width.ToString()]);
                double height = Convert.ToDouble(productPriceRow[ProductPrice.Height.ToString()]);
                bool seamed = Convert.ToBoolean(productPriceRow[ProductPrice.Seamed.ToString()]);
                prices.Add(new Json.Price() { Cost = cost, PPrice = pprice, Height = height, Width = width, Seamed = seamed });
            }

            productInfo.Prices = prices.OrderBy(p => p.Width).OrderBy(p => p.Height).ToArray();

            return productInfo;
        }

    }
}
