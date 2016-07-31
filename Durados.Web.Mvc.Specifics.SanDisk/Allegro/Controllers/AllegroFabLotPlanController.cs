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
    public class AllegroFabLotPlanController : AllegroFabLotController
    {

        protected override Durados.Web.Mvc.UI.TableViewer GetNewTableViewer()
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI.FabLotPlanTableViewer();
        }
        //durados_Import - added by yossi

        protected override void ImportInit()
        {
            datesOffset = 4;
        }

        protected override string getViewNameForImport(string name)
        {
            return "FabLotPlan";
        }

        protected override string GetDeleteStoredProcedureName()
        {
            return "Durados_DeleteFabLotPlan";
        }

        protected override string ImportInsert(View view, Dictionary<string, object> values, SqlCommand command, string Comments, int UserId, Importer importer, DataRow row)
        {
            if (!deleted)
            {
                DeleteDateRange(command, Comments, UserId);
                deleted = true;
            }

            command.CommandType = CommandType.StoredProcedure;

            command.CommandText = "Durados_ImportFabLotPlan";

            checkIfAllKeysExist(new string[] { "fab", "technology", "memory", "ecc type" }, values);


            int Fab = int.Parse(values["fab"].ToString());

            int Tech = int.Parse(values["technology"].ToString());

            int ECC_Type = int.Parse(values["ecc type"].ToString());

            int MemoryId = int.Parse(values["memory"].ToString());

            string SNDK_PN = values["sndk pn"].ToString().Trim();


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
                command.Parameters.AddWithValue("@ECC", ECC_Type);
                command.Parameters.AddWithValue("@Memory", MemoryId);

                command.Parameters.AddWithValue("@Month", Month);
                command.Parameters.AddWithValue("@SNDK", SNDK_PN);
                
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
