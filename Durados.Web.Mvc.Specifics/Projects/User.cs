using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Data.SqlClient;

namespace Durados.Web.Mvc.Specifics.Projects
{
    public class User 
    {
        public static string connectionKey = string.Empty;

        public static int? GetUserID(string username)
        {
            string key = "userid_" + username;
            if (HttpContext.Current.Session[key] != null)
                return (int)HttpContext.Current.Session[key];

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

                    HttpContext.Current.Session[key] = Convert.ToInt32(scalar);
                    return (int)HttpContext.Current.Session[key];
                }
            }
        }

        public static string GetUsername(int userID)
        {
            string key = "username_" + userID.ToString();
            if (HttpContext.Current.Session[key] != null)
                return HttpContext.Current.Session[key].ToString();

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

                    HttpContext.Current.Session[key] = scalar;
                    return HttpContext.Current.Session[key].ToString();
                }
            }
        }
    }
}
