using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class FabLotTableViewer : Durados.Web.Mvc.UI.TableViewer
    {
        public override string GetDisplayName(Field field, DataRow row, string guid)
        {
            int month = 0;
            bool success = int.TryParse(field.DisplayName.Remove(0, 1), out month);

            if (!success)
                return field.DisplayName;

            DateTime date = DateTime.Now.Date.AddMonths(month-1);


            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            CultureInfo provider = new System.Globalization.CultureInfo("en-US");

            return date.ToString("MMM-yyyy", provider);
        }

        public override string GetDisplayName(View view, string guid)
        {
            string lastUploadFile = GetLastUploadFile(((Database)view.Database).ConnectionString);
            if (string.IsNullOrEmpty(lastUploadFile))
            {
                return base.GetDisplayName(view, guid);
            }
            else
            {
                return base.GetDisplayName(view, guid) + " (" + lastUploadFile + ")";
            }
        }

        private string GetLastUploadFile(string connectionString)
        {
            string sql = "select * from " + GetLastUplaodFileViewName();

            string lastUploadFile = string.Empty;

            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    object scalar = command.ExecuteScalar();
                    if (scalar == null || scalar == DBNull.Value)
                    {

                    }
                    else
                    {
                        string comment = scalar.ToString();
                        int index = comment.IndexOf(".xlsx ");
                        if (index > 0)
                        {
                            lastUploadFile = comment.Substring(0, index);
                        }
                    }
                }
                connection.Close();
            }

            return lastUploadFile;
        }

        protected virtual string GetLastUplaodFileViewName()
        {
            return "v_LastFabLotUpload";
        }
    }
}
