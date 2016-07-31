using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic
{
    class CapabilitiesConfigComparer
    {
        
        DataTable beSysCapTable = null;
        Durados.View View;

        public CapabilitiesConfigComparer(Durados.View view)
        {
            this.View = view;
        }
        public virtual string GetCapabilitiesMismatch(Field field, DataRow row)
        {
            if (beSysCapTable == null)
            {
                beSysCapTable = GetBeSysCapTable(field.View.Database.ConnectionString);
            }

            string pk = field.View.GetPkValue(row);
            int pki;
            if (int.TryParse(pk, out pki))
            {
                DataRow[] beSysCapRows = beSysCapTable.Select(GetViewPKFieldName()+"=" + pki);

                if (beSysCapRows != null && beSysCapRows.Length == 1)
                {
                    DataRow beSysCapRow = beSysCapRows[0];
                    if (beSysCapTable.Columns.Contains("besyscapid") && string.IsNullOrEmpty(beSysCapRow["besyscapid"].ToString()))
                    {
                        return null;
                    }
                    else
                    {
                        string[] dbColumnNames = field.GetColumnsNames();

                        if (dbColumnNames.Length == 1)
                        {

                            string columnName = dbColumnNames[0];

                            if (beSysCapTable.Columns.Contains(columnName))
                            {
                                if (!beSysCapRow.IsNull(columnName))
                                {
                                    return beSysCapRow[columnName].ToString();
                                    //string beSysCapMatch = beSysCapRow[columnName].ToString();
                                    //if (!string.IsNullOrEmpty(beSysCapMatch) && beSysCapMatch == "1")
                                    //{
                                    //    string altColumnName = "Alt" + columnName;
                                    //    if (beSysCapTable.Columns.Contains(altColumnName) && !beSysCapRow.IsNull(altColumnName))
                                    //        return beSysCapRow[altColumnName].ToString();
                                    //}
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public virtual string GetViewPKFieldName()
        {
            return "plmid";
        }

        public virtual DataTable GetBeSysCapTable(string connectionString)
        {
            string sql = "SELECT * FROM " + GetAltView(); ;
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(View.Database.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    //if (table.Rows.Count > 0)
                    //{
                    try
                    {
                        adapter.Fill(table);
                    }
                    catch
                    {
                        table.Rows.Clear();
                        adapter.Fill(table);
                    }
                    // }
                }
            }

            return table;
        }

        public virtual string GetAltView()
        {
            return "v_PLMBesSysCapParameterCompare";
        }

    }
    class CapabilitiesParamModeComparer : CapabilitiesConfigComparer
    {
        public CapabilitiesParamModeComparer(Durados.View view):base(view)
        {
             
        }
        public override string GetAltView()
        {
            return "v_PLMBesSysCapParameterModeCompare";
        }
        public override string GetViewPKFieldName()
        {
            return "plmParamId";
        }
    }
}
