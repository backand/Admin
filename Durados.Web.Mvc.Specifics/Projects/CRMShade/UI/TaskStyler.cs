using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.Web.Mvc.Specifics.Projects.CRMShade;

namespace Durados.Web.Mvc.Specifics.Projects.CRMShade.UI
{
    public class TaskStyler : Durados.Web.Mvc.UI.Styler
    {
        public TaskStyler()
            : base()
        {
        }

        public override string GetCellCss(Field field, System.Data.DataRow row)
        {
            if (field.Name == v_TaskOpen.DueDate.ToString())
            {
                bool complete = row["TaskStatusId"].Equals(3);
                DateTime due = (DateTime)((ColumnField)field).ConvertFromString(field.GetValue(row));

                if (!complete)
                {
                    if (due.Date.Equals(DateTime.UtcNow.Date))
                    {
                        return "alertTask";
                    }
                    else if (due.Date < DateTime.UtcNow.Date)
                    {
                        return "alertTaskOverDue";
                    }
                }

            }
            return base.GetCellCss(field, row);
        } 
    }
}
