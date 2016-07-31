using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers;
using Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class PLMStyler : BETechnicalStyler
    {
        protected Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        string BESysReqViewName = "v_PLM_SD_uSD_BESysReq".ToLower();
        CapabilitiesConfigComparer capComparer;
        public PLMStyler(View view, DataView dataView)
            : base(view, dataView)
        {
            capComparer = new CapabilitiesConfigComparer(view);
        }

        protected override Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.BETchnicalChanges GetPlmChanges(View view, DataView dataView)
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.PlmChanges();
        }

        protected override string GetPlmIDColumnName()
        {
            return "Id";
        }
 
        public override string GetCellCss(Field field, DataRow row)
        {
            string yellowAlert = " cellYellowAlert";

            if (field.View.Name.ToLower().StartsWith(BESysReqViewName) && field.ContainerGraphicField == "BSSysCapMismatch")
            {
                return GetCapabilityMismatchFieldGraphics(field, row, yellowAlert);
            }

            if (IsCompareField(field))
            {
                return string.IsNullOrEmpty(capComparer.GetCapabilitiesMismatch(field, row)) ? field.ContainerGraphicProperties : yellowAlert;
            }
            else
            {

                return base.GetCellCss(field, row);
            }
        }
        private string GetCapabilityMismatchFieldGraphics(Field field, DataRow row, string yellowAlert)
        {
            object scalar;
            try
            {

                using (SqlConnection connection = new SqlConnection(field.View.Database.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Durados_Allegro_PLM_CompareConfig_To_BESysCap";
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("@PLMId", field.View.GetPkValue(row)));
                        SqlParameter isCompareParameter = new SqlParameter("@IsCompare", DbType.Boolean);
                        isCompareParameter.Direction = ParameterDirection.Output;
                        command.Parameters.Add(isCompareParameter);

                        scalar = command.ExecuteScalar();
                        if (scalar == null || scalar == DBNull.Value)
                            return base.GetCellCss(field, row);
                        else
                            return yellowAlert;
                    }
                }
            }

            catch (SqlException ex)
            {
                Map.Logger.Log("AllegroPLMController-PLMStyler", "", "GetCapabilityMismatchFieldGraphics", ex, 1, "");


                return yellowAlert;
            }
        
        }


        private  bool IsCompareField(Field field)
        {
            bool isCompareField = false;
            
            if (field.View.Name.ToLower().StartsWith(BESysReqViewName) && field.Category != null && field.Category.Name.ToLower() == "parameters")
                if (field.Name.ToLower() != "plmid" && field.Name.ToLower() != "besyscapid" )
                {
                    isCompareField = true; 
                }
            return isCompareField;
        }

        public override string GetAlt(Field field, DataRow row, string guid)
        {
            if (IsCompareField(field))
            {
                string altResult = capComparer.GetCapabilitiesMismatch(field, row);
                if (!string.IsNullOrEmpty(altResult))
                {
                    return "BE SYS Capability value is: " + altResult;
                }
            }
            return base.GetAlt(field, row, guid);
        }
    
        //private string GetCapabilitiesMismatch(Field field, DataRow row)
        //{
        //    if (beSysCapTable == null)
        //    {
        //        beSysCapTable = GetBeSysCapTable(field.View.Database.ConnectionString);
        //    }

        //    string pk = field.View.GetPkValue(row);
        //    int pki;
        //    if (int.TryParse(pk, out pki))
        //    {
        //        DataRow[] beSysCapRows = beSysCapTable.Select("plmid="+pki);
                
        //        if (beSysCapRows != null && beSysCapRows.Length==1 )
        //        {
        //            DataRow beSysCapRow = beSysCapRows[0];
        //            if (beSysCapTable.Columns.Contains("besyscapid") && !string.IsNullOrEmpty(beSysCapRow["besyscapid"].ToString()))
        //            {
        //                string[] dbColumnNames = field.GetColumnsNames();

        //                if (dbColumnNames.Length == 1)
        //                {

        //                    string columnName = dbColumnNames[0];

        //                    if (beSysCapTable.Columns.Contains(columnName))
        //                    {
        //                        if (!beSysCapRow.IsNull(columnName))
        //                        {
        //                            return beSysCapRow[columnName].ToString();
        //                            //string beSysCapMatch = beSysCapRow[columnName].ToString();
        //                            //if (!string.IsNullOrEmpty(beSysCapMatch) && beSysCapMatch == "1")
        //                            //{
        //                            //    string altColumnName = "Alt" + columnName;
        //                            //    if (beSysCapTable.Columns.Contains(altColumnName) && !beSysCapRow.IsNull(altColumnName))
        //                            //        return beSysCapRow[altColumnName].ToString();
        //                            //}
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}

        //private DataTable GetBeSysCapTable(string connectionString)
        //{
        //    string sql = "SELECT * FROM " + altView; ;
        //    DataTable table = new DataTable();
        //    using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
        //    {
        //        connection.Open();
                
        //        using (SqlCommand command = new SqlCommand(sql, connection))
        //        {
        //            SqlDataAdapter adapter = new SqlDataAdapter(command);
        //            //if (table.Rows.Count > 0)
        //            //{
        //                try
        //                {
        //                    adapter.Fill(table);
        //                }
        //                catch
        //                {
        //                    table.Rows.Clear();
        //                    adapter.Fill(table);
        //                }
        //           // }
        //        }
        //    }

        //    return table;
        //}

    }
}
