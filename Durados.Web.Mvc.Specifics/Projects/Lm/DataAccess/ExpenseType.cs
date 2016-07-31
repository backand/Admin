using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Durados.Web.Mvc.Specifics.Projects.Lm.DataAccess
{
    public class ExpenseType
    {

        public static int? GetExpenseTypeID(int grantItemDetailID)
        {
            Lm.LmProject project = new Durados.Web.Mvc.Specifics.Projects.Lm.LmProject();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[project.ConnectionStringKey].ConnectionString;
            string sql = "SELECT GrantItemType.ExpenseType " +
                        "FROM    GrantItemDetail WITH (NOLOCK) INNER JOIN " +
                            "GrantItemType WITH (NOLOCK) ON GrantItemDetail.GrantItemTypeID = GrantItemType.ID " +
                        "WHERE GrantItemDetail.ID = " + grantItemDetailID.ToString();
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    object scalar = command.ExecuteScalar();

                    if (scalar is DBNull || scalar == null)
                    {
                        return null;
                    }

                    return Convert.ToInt32(scalar);
                }
            }
        }

        
    }
}
