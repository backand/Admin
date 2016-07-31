using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Data;
using Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class PLMTableViewer:Durados.Web.Mvc.UI.TableViewer
    {
        
        public override string GetElementForTableView(Field field, System.Data.DataRow row, string guid)
        {
            if (field.View.Name ==AllegroPLMController.reqSduSdCloneName && field.DatabaseNames== "BSSysCapMismatch")
            {
                object scalar;
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
                            return string.Empty;
                        else
                            return scalar.ToString();
                    }
                }

            }

            
            return base.GetElementForTableView(field, row, guid);
        }
    }
}
