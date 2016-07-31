using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Durados.Web.Mvc.Specifics.DataAccess
{
    public class Product
    {
        
        public static int? GetCompanyID(int productID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[Durados.Web.Mvc.Specifics.DataAccess.User.connectionKey].ConnectionString;
            string sql = "select CompanyID from [Product]  WITH (NOLOCK) Where ID = " + productID.ToString();

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
