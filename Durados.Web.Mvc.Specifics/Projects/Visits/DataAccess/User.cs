using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Durados.Web.Mvc.Specifics.Projects.Visits.DataAccess
{
    public class User
    {
        public static string connectionKey = string.Empty;

        

        public static int? GetUserID(string username)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;
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

        public static string GetUsername(int userID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;
            string sql = "select Sochen from AA_Agents WITH (NOLOCK) Where ID = " + userID.ToString();

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
