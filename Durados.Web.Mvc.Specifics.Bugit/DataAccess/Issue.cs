using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Durados.Web.Mvc.Specifics.Bugit.DataAccess
{
    public class Issue
    {
        public static string connectionKey = "TasksConnectionString";
        
        public static int? GetProductID(int issueID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;
            string sql = "select ProjectID from [Issue]  WITH (NOLOCK) Where ID = " + issueID.ToString();

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

        public static int? GetReportedBy(int issueID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;
            string sql = "select ReportedBy from [Issue]  WITH (NOLOCK) Where ID = " + issueID.ToString();

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

        public static int? GetAssignedTo(int issueID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;
            string sql = "select AssignedTo from [Issue]  WITH (NOLOCK) Where ID = " + issueID.ToString();

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
