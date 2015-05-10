using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Globalization;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Membership;



namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroFabLotController : AllegroHomeController
    {

        protected override Durados.Web.Mvc.UI.TableViewer GetNewTableViewer()
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI.FabLotTableViewer();
        }


        //durados_Import - added by yossi

        protected List<DateTime> dates;

        protected int datesOffset;

        protected override string GetFieldValuByName()
        {
            return "DisplayName";
        }

        protected override bool AddUnknownFieldsValues()
        {
            return true;
        }

        protected override bool IsSpecificImport()
        {
            return true;
        }        

        protected override void ImportInit()
        {
            datesOffset = 1;
        }

        

        protected virtual DateTime getDateHeader(string header)
        {
            DateTime parsedDate = new DateTime();

            string pattern = "d-M-yyyy";

            int timestamp = 0;

            if (int.TryParse(header, out timestamp))
            {

                if (!DateTime.TryParseExact(ExcelSerialDateToDMY(timestamp), pattern, null, DateTimeStyles.None, out parsedDate))
                {
                    throw new DuradosException("The dates headers are not in valid format!");
                }
            }
            else
            {
                string[] patterns = { "MMM-yyyy", "MMM-yy" };

                if (!DateTime.TryParseExact(header, patterns, null, DateTimeStyles.None, out parsedDate))
                {
                    throw new DuradosException("The dates headers are not in valid format!");
                }
            } 

            return parsedDate;
        }

        protected override void prepareTableForImport(View view, ref DataTable table, ImportModes ImportMode)
        {
            string header;

            dates = new List<DateTime>();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                header = table.Rows[0][i].ToString().Trim();

                if (header == string.Empty) continue;

                //For fab lot excel header...
                if (header.ToLower() == "month") { header = "technology"; }

                if (i > datesOffset)
                {
                    dates.Add(getDateHeader(header));
                    header = "m" + dates.Count.ToString();
                }               

                table.Columns[i].ColumnName = header;
            }
            table.Rows.RemoveAt(0);
            deleted = false;
        }

        protected void checkIfAllKeysExist(string[] keys, Dictionary<string, object> values)
        {
            foreach (string k in keys)
            {
                if (!values.ContainsKey(k))
                {
                    throw new DuradosException("Column name ["+k+"] is required!");
                }
            }

        }

        protected virtual string GetDeleteStoredProcedureName()
        {
            return "Durados_DeleteFabLot";
        }

        protected virtual void DeleteDateRange(SqlCommand command, string Comments, int UserId)
        {
            command.CommandType = CommandType.StoredProcedure;

            command.CommandText = GetDeleteStoredProcedureName();

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@FirstMonth", dates.Min());
            command.Parameters.AddWithValue("@LastMonth", dates.Max());
            command.Parameters.AddWithValue("@UserId", UserId);
            command.Parameters.AddWithValue("@Comments", Comments);

            command.ExecuteNonQuery();
        }

        protected bool deleted = false;
        protected override string ImportInsert(View view, Dictionary<string, object> values, SqlCommand command, string Comments, int UserId, Importer importer, DataRow row)
        {
            if (!deleted)
            {
                DeleteDateRange(command, Comments, UserId);
                deleted = true;
            }
            command.CommandType = CommandType.StoredProcedure;

            command.CommandText = "Durados_ImportFabLot";

            checkIfAllKeysExist(new string[] { "fab", "technology" }, values);

            int Fab = int.Parse(values["fab"].ToString());

            int Tech = int.Parse(values["technology"].ToString());

            DateTime Month;

            double LotSource;

            int Lot;

            bool isNumber;

            int pk;

            for (int i = 0; i < values.Count - datesOffset; i++)
            {
                if (!values.ContainsKey("m" + (i + 1)) || string.IsNullOrEmpty(values["m" + (i + 1)].ToString())) continue;

                Month = dates[i];

                isNumber = double.TryParse(values["m" + (i + 1)].ToString(), out LotSource);

                if (!isNumber)
                {
                    throw new DuradosException("Lot value [" + values["m" + (i + 1)].ToString() + "] need to be a number!");
                }

                LotSource = Math.Round(LotSource, 0);

                Lot = (int)LotSource;
               
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Fab", Fab);
                command.Parameters.AddWithValue("@Tech", Tech);
                command.Parameters.AddWithValue("@Month", Month);
                command.Parameters.AddWithValue("@Lot", Lot);
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@Comments", Comments);

                pk = (int)command.ExecuteScalar();

                //History history = new History();

            }

            return "ok";
        }

    }
}
