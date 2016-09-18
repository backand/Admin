using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers.Search
{
    public class FieldNameConfigFieldSearcher : ConfigFieldSearcher
    {
        protected override string GetIdFilterFieldName()
        {
            return "Fields";
        }

        protected override string GetName(System.Data.DataRowView row, string q)
        {
            return row.Row.GetParentRow(GetIdFilterFieldName())["Name"].ToString();
        }
    }
}
