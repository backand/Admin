using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic
{
    public class BETchnicalChanges
    {
        public virtual Dictionary<string, Dictionary<string, SortedList<DateTime, PlmChange>>> GetPlmChangesFromLastChangeRequest(View view, DataView dataView)
        {
            return GetPlmChanges(view, dataView, "vc_PlmChangesAfterLastChangeRequest");
        }

        public virtual Dictionary<string, Dictionary<string, SortedList<DateTime, PlmChange>>> GetPlmChangesFromLastNewRequest(View view, DataView dataView)
        {
            //return GetPlmChanges(view, dataView, "vc_PlmChangesAfterLastNewRequest");
            return GetPlmChanges(view, dataView, "vc_PlmChangesAfterLastNewRequestCR");
        }

        public virtual Dictionary<string, string> GetPlmParametersChangesFromLastChangeRequest(View view, DataView dataView)
        {
            return GetPlmParametersChanges(view, dataView, "vc_PlmParameterChangesAfterLastChangeRequest");
        }

        public virtual Dictionary<string, string> GetPlmParametersChangesFromLastNewRequest(View view, DataView dataView)
        {
            //return GetPlmParametersChanges(view, dataView, "vc_PlmParameterChangesAfterLastNewRequest");
            return GetPlmParametersChanges(view, dataView, "vc_PlmParameterChangesAfterLastNewRequestCR");
        }

        public virtual Dictionary<string, string> GetPlmParametersChanges(View view, DataView dataView, string viewName)
        {
            Dictionary<string, string> plmParametersChanges = new Dictionary<string, string>();
            string inStatement = GetInStatement(dataView);
            if (string.IsNullOrEmpty(inStatement))
                return plmParametersChanges;

            string sql = "select PLMId from " + viewName + " where PLMId in (" + inStatement + ")";

            using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string PLMId = reader.GetValue(reader.GetOrdinal("PLMId")).ToString();
                        if (!plmParametersChanges.ContainsKey(PLMId))
                            plmParametersChanges.Add(PLMId, PLMId);
                    }

                    reader.Close();
                    connection.Close();
                }
            }


            return plmParametersChanges;
        }

        public virtual Dictionary<string, Dictionary<string, SortedList<DateTime, PlmChange>>> GetPlmChanges(View view, DataView dataView, string viewName)
        {
            Dictionary<string, Dictionary<string, SortedList<DateTime, PlmChange>>> plmChanges = new Dictionary<string, Dictionary<string, SortedList<DateTime, PlmChange>>>();

            string inStatement = GetInStatement(dataView);
            if (string.IsNullOrEmpty(inStatement))
                return plmChanges;

            string sql = "select * from " + viewName + " where PK in (" + inStatement + ")";

            using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string PK = reader.GetString(reader.GetOrdinal("PK"));
                        string FieldName = reader.GetString(reader.GetOrdinal("FieldName"));
                        string OldValue = reader.GetString(reader.GetOrdinal("OldValue"));
                        string NewValue = reader.GetString(reader.GetOrdinal("NewValue"));
                        DateTime UpdateDate = reader.GetDateTime(reader.GetOrdinal("UpdateDate"));
                        DateTime LastDate = reader.GetDateTime(reader.GetOrdinal("LastDate"));
                        string ColumnNames = reader.GetString(reader.GetOrdinal("ColumnNames"));
                        string FullName = reader.GetString(reader.GetOrdinal("FullName"));

                        PlmChange plmChange = new PlmChange() { PK = PK, FieldName = FieldName, OldValue = OldValue, NewValue = NewValue, UpdateDate = UpdateDate, LastDate = LastDate, ColumnNames = ColumnNames, FullName = FullName };

                        if (!plmChanges.ContainsKey(PK))
                        {
                            plmChanges.Add(PK, new Dictionary<string, SortedList<DateTime, PlmChange>>());
                        }
                        if (!plmChanges[PK].ContainsKey(ColumnNames))
                        {
                            plmChanges[PK].Add(ColumnNames, new SortedList<DateTime, PlmChange>());
                        }
                        if (!plmChanges[PK][ColumnNames].ContainsKey(plmChange.UpdateDate))
                            plmChanges[PK][ColumnNames].Add(plmChange.UpdateDate, plmChange);
                    }

                    reader.Close();
                    connection.Close();
                }
            }

            return plmChanges;
        }

        protected virtual string GetPlmIDColumnName()
        {
            return "PLMId";
        }

        protected virtual string GetInStatement(DataView dataView)
        {
            string inStatement = string.Empty;
            string plmIdColumnName = GetPlmIDColumnName();

            foreach (System.Data.DataRowView row in dataView)
            {
                string pk = row[plmIdColumnName].ToString();
                inStatement += "'" + pk + "',";
            }

            return inStatement.TrimEnd(',');
        }
    }

    public class PlmChange
    {
        public string PK { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime LastDate { get; set; }
        public string ColumnNames { get; set; }
        public string FullName { get; set; }

        public string GetAlt()
        {
            return FullName + " changed " + OldValue + " to " + NewValue + " at " + UpdateDate;
        }
    }

}
