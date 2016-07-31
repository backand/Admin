using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI
{
    public class ColumnsExcluder
    {
        Dictionary<string, UI.Json.Field> filterFields;
        Dictionary<string, Field> excludedColumns = new Dictionary<string, Field>();
        View view;

        public ColumnsExcluder(View view, Dictionary<string, UI.Json.Field> filterFields)
        {
            this.filterFields = filterFields;
            this.view = view;
        }

        public Dictionary<string, Field> ExcludedColumns
        {
            get
            {
                return excludedColumns;
            }
        }
    }
}
