using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Durados.Web.Mvc.Specifics.Bugit.DataAccess
{
    public class User
    {
        public static string connectionKey = "BugDBConnectionString";
        
        public static int? GetCompanyID(string username)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;
            string sql = "select CompanyID from [User]  WITH (NOLOCK) Where Username = N'" + username + "'";

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
            string sql = "select Username from [User] WITH (NOLOCK) Where ID = " + userID.ToString();

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

        public static string GetUserEmail(int userID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;
            string sql = "select Email from [User] WITH (NOLOCK) Where ID = " + userID.ToString();

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
