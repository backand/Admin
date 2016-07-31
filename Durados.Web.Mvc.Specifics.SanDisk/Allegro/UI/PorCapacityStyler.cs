using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class PorCapacityStyler : Durados.Web.Mvc.UI.Styler
    {
        public PorCapacityStyler(View view, DataView dataView)
            : base(view, dataView)
        {
        }

        #region comments
        //DateTime? committed = null;
        //if (!row[v_POR.ComittedACT.ToString()].Equals(DBNull.Value))
        //    committed = (DateTime)row[v_POR.ComittedACT.ToString()];

        //DateTime? forecast = null;
        //if (!row[v_POR.ForecastACT.ToString()].Equals(DBNull.Value))
        //    forecast = (DateTime)row[v_POR.ForecastACT.ToString()];

        //DateTime? required = null;
        //if (!row[v_POR.RequiredACT.ToString()].Equals(DBNull.Value))
        //    required = (DateTime)row[v_POR.RequiredACT.ToString()];

        //DateTime? create = null;
        //if (!row[v_POR.CreateDate.ToString()].Equals(DBNull.Value))
        //    create = (DateTime)row[v_POR.CreateDate.ToString()];

        //if (required.HasValue)
        //{
        //    if (required < DateTime.Today.AddDays(180))
        //    {
        //        if (committed.HasValue)
        //        {
        //            if (committed > required.Value.AddDays(14))
        //            {
        //                return redAlert;
        //            }
        //        }
        //        else
        //        {
        //            if (create.HasValue && create.Value.AddDays(14) < DateTime.Today)
        //            {
        //                return redAlert;
        //            }
        //        }

        //        if (forecast.HasValue)
        //        {
        //            if (forecast > required.Value.AddDays(14))
        //            {
        //                return redAlert;
        //            }
        //        }
        //        else
        //        {
        //            if (create.HasValue && create.Value.AddDays(14) < DateTime.Today)
        //            {
        //                return redAlert;
        //            }
        //        }


        //        if (committed.HasValue)
        //        {
        //            if (committed > required.Value.AddDays(7))
        //            {
        //                return yellowAlert;
        //            }
        //        }
        //        else
        //        {
        //            if (create.HasValue && create.Value.AddDays(7) < DateTime.Today)
        //            {
        //                return yellowAlert;
        //            }
        //        }

        //        if (forecast.HasValue)
        //        {
        //            if (forecast > required.Value.AddDays(7))
        //            {
        //                return yellowAlert;
        //            }
        //        }
        //        else
        //        {
        //            if (create.HasValue && create.Value.AddDays(7) < DateTime.Today)
        //            {
        //                return yellowAlert;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (committed.HasValue)
        //        {
        //            if (committed > required.Value.AddDays(30))
        //            {
        //                return redAlert;
        //            }
        //        }
        //        else
        //        {
        //            if (create.HasValue && create.Value.AddDays(30) < DateTime.Today)
        //            {
        //                return redAlert;
        //            }
        //        }

        //        if (forecast.HasValue)
        //        {
        //            if (forecast > required.Value.AddDays(30))
        //            {
        //                return redAlert;
        //            }
        //        }
        //        else
        //        {
        //            if (create.HasValue && create.Value.AddDays(30) < DateTime.Today)
        //            {
        //                return redAlert;
        //            }
        //        }


        //        if (committed.HasValue)
        //        {
        //            if (committed > required.Value.AddDays(14))
        //            {
        //                return yellowAlert;
        //            }
        //        }
        //        else
        //        {
        //            if (create.HasValue && create.Value.AddDays(14) < DateTime.Today)
        //            {
        //                return yellowAlert;
        //            }
        //        }

        //        if (forecast.HasValue)
        //        {
        //            if (forecast > required.Value.AddDays(14))
        //            {
        //                return yellowAlert;
        //            }
        //        }
        //        else
        //        {
        //            if (create.HasValue && create.Value.AddDays(14) < DateTime.Today)
        //            {
        //                return yellowAlert;
        //            }
        //        }
        //    }
        //}
        #endregion
        public override string GetCellCss(Field field, System.Data.DataRow row)
        {
            string redAlert = " cellRedAlert";
            string yellowAlert = " cellYellowAlert";

            if (field.Name == SanDisk.Allegro.v_PORCapacity.StatusDescription.ToString())
            {
                switch (row[v_PORCapacity.Status.ToString()].ToString())
                {
                    case "R":
                        return base.GetCellCss(field, row) + redAlert;
                    case "Y":
                        return base.GetCellCss(field, row) + yellowAlert;

                    default:
                        return base.GetCellCss(field, row) + " cellGreen";
                }
                

                

            }
            return base.GetCellCss(field, row);
        } 
    }
}
