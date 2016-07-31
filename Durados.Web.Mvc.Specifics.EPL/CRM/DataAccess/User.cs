using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.EPL.CRM.DataAccess
{
    public class User
    {
        public static int? GetUserID(string username, string connectionString)
        {
            string sql = "select ID from [User]  WITH (NOLOCK) Where Username = N'" + username + "'";

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
