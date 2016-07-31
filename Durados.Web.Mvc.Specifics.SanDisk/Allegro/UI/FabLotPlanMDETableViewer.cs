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
    public class FabLotPlanMDETableViewer : FabLotTableViewer
    {
        protected string firstPlanningMonthName = "FirstPlanningMonth";
        public override bool IsEditable(Durados.View view)
        {
            return true;
        }

        public override string GetEditCaption(Durados.View view, DataRow row, string guid)
        {
            return string.Empty;
        }


        public override bool IsEditOnDblClick(Durados.View view)
        {
            return false;
        }

        public override bool IsSortable(Durados.Field field, string guid)
        {
            return false;
        }

        public override bool IsEditable(Field field, DataRow row, string guid)
        {
            return !(field.DisableInEdit || Durados.Web.Mvc.UI.Helpers.SecurityHelper.IsDenied(field.DenyEditRoles, field.AllowEditRoles)) && !row.IsNull(firstPlanningMonthName);
        }

        public override bool SelectorCheckbox(Durados.View view, DataRow row)
        {
            return !row.IsNull(firstPlanningMonthName);
        }


        public override string GetElementForTableView(Durados.Field field, DataRow row, string guid)
        {
            if (field.FieldType == FieldType.Children && ((ChildrenField)field).ChildrenHtmlControlType != ChildrenHtmlControlType.CheckList && row.IsNull("EngRevisionId"))
            {
                return string.Empty;
            }
            else
            {
                return base.GetElementForTableView(field, row, guid);
            }
        }

        public override bool ShowRowHover(Durados.View view, DataRow row)
        {
            return !row.IsNull(firstPlanningMonthName);
        }
        public override bool IsVisible(Field field, Dictionary<string, Field> excludedColumn, string guid)
        {
            bool visible = base.IsVisible(field, excludedColumn, guid);
            string[] indexStr = field.Name.Split(new char[1] { GetPeriodPrefix()[0] }, StringSplitOptions.RemoveEmptyEntries);
            int index = 0;

            if (indexStr.Length>0 && Int32.TryParse(indexStr[0], out index))
            {

                DateTime? firstPlanningMonth = GetFirstPlanningMonth();
                if (filter == null)
                {
                    filter = GetFilter((View)field.View, guid);
                }
                DateTime filterDate = filter.Date.Date;
                if (filterDate < firstPlanningMonth.Value)
                {
                    filterDate = firstPlanningMonth.Value;
                }

                int filterIndex = GetFilterIndex(filterDate, firstPlanningMonth);
                
                return visible && (index >= filterIndex && index < filterIndex + 12);
            }
            else
            {
                return visible;
            }
        }

        protected virtual int GetFilterIndex(DateTime filterDate, DateTime? firstPlanningMonth)
        {
            return (filterDate.Year - firstPlanningMonth.Value.Year) * 12 + (filterDate.Month - firstPlanningMonth.Value.Month + 1);
        }

        public override string GetDisplayName(Field field, DataRow row, string guid)
        {
            DateTime? firstPlanningMonth = GetFirstPlanningMonth();

            if (!firstPlanningMonth.HasValue)
                return field.DisplayName;

            int month = 0;
            bool success = int.TryParse(field.DisplayName.Remove(0, 1), out month);

            if (!success)
                return field.DisplayName;

            DateTime date = firstPlanningMonth.Value.Date.AddMonths(month - 1);


            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            CultureInfo provider = new System.Globalization.CultureInfo("en-US");

            return date.ToString("MMM-yyyy", provider);
        }

        DateTime? _firstPlanningMonth = null;

        protected virtual DateTime? GetFirstPlanningMonth()
        {
            if (_firstPlanningMonth != null)
                return _firstPlanningMonth;

            if (DataView == null || DataView.Count == 0)
            {
                _firstPlanningMonth = DateTime.Now.Date;
                return _firstPlanningMonth;
            }

            if (!DataView[0].Row.IsNull(firstPlanningMonthName))
                _firstPlanningMonth = (DateTime)DataView[0].Row[firstPlanningMonthName];
            else if (DataView.Count > 1 && !DataView[1].Row.IsNull(firstPlanningMonthName))
                _firstPlanningMonth = (DateTime)DataView[1].Row[firstPlanningMonthName];
            else
                throw new DuradosException("The First Planning Month is missing. This field is located in Nand and it is required for this report.");


            return _firstPlanningMonth;
        }

        protected override string GetLastUplaodFileViewName()
        {
            return "v_LastFabLotPlanUpload";
        }

        public override System.Data.DataView GetDataView(System.Data.DataView dataView, View view, string guid)
        {
            DataTable table = dataView.Table;

            if (filter == null)
            {
                filter = GetFilter(view, guid);
            }
            if (filter.Fab == -1)
            {
                int[] fabs = GetFabs(table);
                int id = 0;
                foreach (int fab in fabs)
                {
                    id--;
                    filter.Fab = fab;
                    LoadTable(table, view, guid, filter, id);
                }
                filter.Fab = -1;
            }
            else
            {
                LoadTable(table, view, guid, filter, -1);
            }

            dataView.Sort = "FabId, firstPlanningMonth, NANDCSId, EngRevisionId, ProductionRevisionId";
            return dataView;
        }

        private int[] GetFabs(DataTable table)
        {
            Dictionary<int, int> fabs = new Dictionary<int, int>();

            foreach (DataRow row in table.Rows)
            {
                if (!row.IsNull("FabId"))
                {
                    int fab = (int)row["FabId"];
                    if (!fabs.ContainsKey(fab))
                        fabs.Add(fab, fab);
                }
            }
            return fabs.Keys.ToArray();
        }

        FabLotPlanFilter filter = null;

        protected virtual FabLotPlanLoader GetNewFabLotPlanLoader()
        {
            return new FabLotPlanLoader();
        }

        protected virtual void LoadTable(DataTable table, View view, string guid, FabLotPlanFilter filter, int id)
        {
            DateTime? firstPlanningMonth = GetFirstPlanningMonth();

            if (!firstPlanningMonth.HasValue)
            {
                return;
            }

            FabLotPlanLoader fabLotPlanLoader = GetNewFabLotPlanLoader();

            SortedDictionary<DateTime, double> lots = fabLotPlanLoader.GetLots(view, filter);

            DataRow newRow = table.NewRow();

            string periodPrefix = GetPeriodPrefix();

            foreach (DateTime date in lots.Keys)
            {
                
                double lot = lots[date];


                int index = GetIndex(date, firstPlanningMonth);

                if (table.Columns.Contains(periodPrefix + index))
                    newRow[periodPrefix + index] = lot;
            }
            newRow["Id"] = id;
            newRow["TechnologyId"] = filter.Technology;
            newRow["FabId"] = filter.Fab;
            newRow["MemoryId"] = filter.Memory;
            newRow["ECCTypeId"] = filter.Ecc;

            table.Rows.Add(newRow);
        }

        protected virtual string GetPeriodPrefix()
        {
            return "m";
        }

        protected virtual int GetIndex(DateTime date, DateTime? firstPlanningMonth)
        {
            return (date.Year - firstPlanningMonth.Value.Year) * 12 + (date.Month - firstPlanningMonth.Value.Month + 1);
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

        //protected virtual SortedDictionary<DateTime, double> GetLots(View view, FabLotPlanFilter filter)
        //{
            
        //    string sql = "select * from FabLotPlan " + filter.GetWhereStatement();

        //    SortedDictionary<DateTime, double> lots = new SortedDictionary<DateTime, double>();

        //    using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(sql, connection))
        //        {
        //            SqlParameter fromParameter = new SqlParameter("FromDate", new DateTime(filter.Date.Year, filter.Date.Month, 1));
        //            command.Parameters.Add(fromParameter);
        //            SqlParameter toParameter = new SqlParameter("ToDate", new DateTime(filter.Date.Year + 1, filter.Date.Month, 1));
        //            command.Parameters.Add(toParameter);
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    DateTime date = reader.GetDateTime(reader.GetOrdinal("Date"));
        //                    double lot = reader.GetDouble(reader.GetOrdinal("Lot"));
        //                    lots.Add(date, lot);
        //                }
        //                reader.Close();
        //            }
        //        }
        //        connection.Close();
        //    }

        //    return lots;
        //}

        

        protected virtual FabLotPlanFilter GetFilter(View view, string guid)
        {
            FabLotPlanFilter filter = new FabLotPlanMDEFilter(view, guid);
            return filter;
        }

        public override void HandleFilter(Durados.Web.Mvc.UI.Json.Filter jsonFilter)
        {
            jsonFilter.Fields["ProductionOut"].Value = string.Empty;
        }
    }

    public class FabLotPlanMDEFilter : FabLotPlanFilter
    {
        public FabLotPlanMDEFilter(View view, string guid) :
            base(view, guid)
        {
        }
        
        protected override string GetFabFieldName()
        {
            return "Fab_v_FabLotPlanMDE_Parent";
        }

        protected override string GetTechnologyFieldName()
        {
            return "Technology_v_FabLotPlanMDE_Parent";
        }

        protected override string GetMemoryFieldName()
        {
            return "Memory_v_FabLotPlanMDE_Parent";
        }

        protected override string GetEccFieldName()
        {
            return "ECCType_v_FabLotPlanMDE_Parent";
        }

        protected override string GetProductionOutFieldName()
        {
            return "ProductionOut";
        }
    }

    public class FabLotPlanLoader
    {
        public virtual SortedDictionary<DateTime, double> GetLots(View view, FabLotPlanFilter filter)
        {

            string sql = "select * from FabLotPlan " + filter.GetWhereStatement();

            SortedDictionary<DateTime, double> lots = new SortedDictionary<DateTime, double>();

            using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    SqlParameter fromParameter = GetFromParameter(filter);
                    command.Parameters.Add(fromParameter);
                    SqlParameter toParameter = GetToParameter(filter);
                    command.Parameters.Add(toParameter);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime date = reader.GetDateTime(reader.GetOrdinal("Date"));
                            double lot = reader.GetDouble(reader.GetOrdinal("Lot"));

                            date = GetFirstPeriodDate(date);
                            if (lots.ContainsKey(date))
                                lots[date] += lot;
                            else
                                lots.Add(date, lot);
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            return lots;
        }

        protected virtual SqlParameter GetFromParameter(FabLotPlanFilter filter)
        {
            return new SqlParameter("FromDate", new DateTime(filter.Date.Year, filter.Date.Month, 1));
                    
        }

        protected virtual SqlParameter GetToParameter(FabLotPlanFilter filter)
        {
            return new SqlParameter("ToDate", new DateTime(filter.Date.Year + 1, filter.Date.Month, 1));
        }

        protected virtual DateTime GetFirstPeriodDate(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }
    }
}
