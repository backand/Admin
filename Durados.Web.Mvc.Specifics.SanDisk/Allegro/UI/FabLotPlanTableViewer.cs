using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;

using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class FabLotPlanTableViewer : FabLotTableViewer
    {

        public override string GetDisplayName(Field field, DataRow row, string guid)
        {
            if (filter == null)
            {
                filter = GetFilter((View)field.View, guid);
            }

            int month = 0;
            bool success = int.TryParse(field.DisplayName.Remove(0, 1), out month);

            if (!success)
                return field.DisplayName;

            DateTime date = filter.Date.Date.AddMonths(month - 1);


            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            CultureInfo provider = new System.Globalization.CultureInfo("en-US");

            return date.ToString("MMM-yyyy", provider);
        }

        protected override string GetLastUplaodFileViewName()
        {
            return "v_LastFabLotPlanUpload";
        }

        public override System.Data.DataView GetDataView(System.Data.DataView dataView, View view, string guid)
        {
            DataTable table = dataView.Table;

            LoadTable(table, view, guid);

            return dataView;
        }

        FabLotPlanFilter filter = null;

        protected virtual void LoadTable(DataTable table, View view, string guid)
        {
            if (filter == null)
            {
                filter = GetFilter(view, guid);
            }

            SortedDictionary<DateTime, double> lots = GetLots(view, filter);

            int index = 0;

            foreach (DateTime date in lots.Keys)
            {
                index++;
                double lot = lots[date];

                DataRow prevRow = null;
                foreach (DataRow row in table.Rows)
                {
                    if (!row.IsNull("ProductionOut"))
                    {
                        DateTime ProductionOut = (DateTime)row["ProductionOut"];
                        DateTime ProductionOutMonth = new DateTime(ProductionOut.Year, ProductionOut.Month, 1);

                        if (date >= ProductionOutMonth)
                        {
                            prevRow = row;
                        }
                        else
                        {
                            if (prevRow != null)
                            {
                                SetLot(index, table, prevRow, lot);
                            }
                        }
                    }
                }
                if (prevRow != null)
                {
                    SetLot(index, table, prevRow, lot);
                }
            }

        }

        protected virtual void SetLot(int index, DataTable table, DataRow row, double lot)
        {
            string lotColumnName = "m" + index;
            if (table.Columns.Contains(lotColumnName))
            {
                table.Columns[lotColumnName].ReadOnly = false;
                row[lotColumnName] = lot;
            }
        }

        protected virtual SortedDictionary<DateTime, double> GetLots(View view, FabLotPlanFilter filter)
        {
            
            string sql = "select * from FabLotPlan " + filter.GetWhereStatement();

            SortedDictionary<DateTime, double> lots = new SortedDictionary<DateTime, double>();

            using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    SqlParameter fromParameter = new SqlParameter("FromDate", new DateTime(filter.Date.Year, filter.Date.Month, 1));
                    command.Parameters.Add(fromParameter);
                    SqlParameter toParameter = new SqlParameter("ToDate", new DateTime(filter.Date.Year + 1, filter.Date.Month, 1));
                    command.Parameters.Add(toParameter);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime date = reader.GetDateTime(reader.GetOrdinal("Date"));
                            double lot = reader.GetDouble(reader.GetOrdinal("Lot"));
                            lots.Add(date, lot);
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            return lots;
        }

        protected virtual FabLotPlanFilter GetFilter(View view, string guid)
        {
            FabLotPlanFilter filter = new FabLotPlanFilter(view, guid);
            return filter;
        }

        public override void HandleFilter(Durados.Web.Mvc.UI.Json.Filter jsonFilter)
        {
            jsonFilter.Fields["ProductionOut"].Value = string.Empty;
        }
    }

    public class FabLotPlanFilter
    {
        public int Fab { get; set; }
        public int Technology { get; private set; }
        public int Memory { get; private set; }
        public int Ecc { get; private set; }
        public DateTime Date { get; private set; }


        protected virtual string GetFabFieldName()
        {
            return "Fab_v_FabLotPlan_Parent";
        }

        protected virtual string GetTechnologyFieldName()
        {
            return "Technology_v_FabLotPlan_Parent";
        }

        protected virtual string GetMemoryFieldName()
        {
            return "Memory_v_FabLotPlan_Parent";
        }

        protected virtual string GetEccFieldName()
        {
            return "ECCType_v_FabLotPlan_Parent";
        }

        protected virtual string GetProductionOutFieldName()
        {
            return "ProductionOut";
        }

        public FabLotPlanFilter(View view, string guid)
        {
            Durados.Web.Mvc.UI.ViewViewer viewViewer = new Durados.Web.Mvc.UI.ViewViewer();

            System.Web.Mvc.FormCollection collection = new System.Web.Mvc.FormCollection(ViewHelper.GetSessionState(guid + "Filter"));
            Durados.Web.Mvc.UI.Json.Filter filter = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<Durados.Web.Mvc.UI.Json.Filter>(collection["jsonFilter"]);

            string fabFieldName = GetFabFieldName();
            if (filter.Fields.ContainsKey(fabFieldName))
            {
                object value = filter.Fields[fabFieldName].Value;
                if (value != null && value.ToString() != string.Empty)
                {
                    Fab = Convert.ToInt32(value);
                }
                else
                {
                    Fab = -1;
                }
            }

            string technologyFieldName = GetTechnologyFieldName();
            if (filter.Fields.ContainsKey(technologyFieldName))
            {
                object value = filter.Fields[technologyFieldName].Value;
                if (value != null)
                {
                    Technology = Convert.ToInt32(value);
                }
            }

            string memoryFieldName = GetMemoryFieldName();
            if (filter.Fields.ContainsKey(memoryFieldName))
            {
                object value = filter.Fields[memoryFieldName].Value;
                if (value != null)
                {
                    Memory = Convert.ToInt32(value);
                }
            }

            string eccFieldName = GetEccFieldName();
            if (filter.Fields.ContainsKey(eccFieldName))
            {
                object value = filter.Fields[eccFieldName].Value;
                if (value != null)
                {
                    Ecc = Convert.ToInt32(value);
                }
            }

            string dateFieldName = GetProductionOutFieldName();
            if (filter.Fields.ContainsKey(dateFieldName))
            {
                object value = filter.Fields[dateFieldName].Value;
                if (value == null)
                {
                    Date = DateTime.Now.Date;
                }
                else
                {
                    if (value.ToString() == string.Empty)
                    {
                        Date = DateTime.Now.Date;
                    }
                    else
                    {
                        Date = Convert.ToDateTime(value);
                    }
                }
            }
        }

        public string GetWhereStatement()
        {
            string where = string.Empty;

            //where += " where " + (Fab == -1 ? "" : ("FabId=" + Fab + " and ")) + "TechnologyId=" + Technology + " and MemoryId=" + Memory + " and ECCTypeId=" + Ecc + " and Date between @FromDate and @ToDate";
            where += " where " + "FabId=" + Fab + " and TechnologyId=" + Technology + " and (MemoryId= (SELECT TOP (1) MemoryId FROM      Nand WITH (nolock) WHERE   (Id = " + Memory + "))) and (ECCTypeId=(SELECT TOP (1) ECCTypeId FROM      Nand AS Nand_1 WITH (nolock) WHERE   (Id = " + Memory + "))) and Date between @FromDate and @ToDate";

            return where;
        }
    }
}
