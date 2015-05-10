using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Durados.Web.Mvc.Specifics.Gear.DataAccess
{
    public class User
    {
        public static string GetUsername(int userID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["GearConnectionString"].ConnectionString;
            string sql = "select Email from [gear_User] WITH (NOLOCK) Where ID = " + userID.ToString();

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

                    return Convert.ToString(scalar);
                }
            }
        }
    }
}
