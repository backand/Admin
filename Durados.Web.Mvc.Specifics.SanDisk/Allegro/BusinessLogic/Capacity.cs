using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic
{
    public class Capacity
    {
        protected Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        public Json.CapacityInfo GetCapacityInfo(int productClassID, int porID)
        {
            Json.CapacityInfo capacityInfo = new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.Json.CapacityInfo();


            string options = string.Empty;

            Dictionary<int, int> selecteds = new Dictionary<int, int>();

            using (SqlConnection connection = new SqlConnection(Map.Database.ConnectionString))
            {
                string selectStatement = "SELECT ProductCapacityId FROM PORProductClassCapacity where PORId = " + porID;
            
                using (SqlCommand command = new SqlCommand(selectStatement, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("ProductCapacityId"));
                        selecteds.Add(id, id);
                    }


                }
            }

            using (SqlConnection connection = new SqlConnection(Map.Database.ConnectionString))
            {
                    

                string selectStatement = "SELECT ProductCapacity.Id, ProductCapacity.Name FROM ProductCapacity INNER JOIN ProductClassCapacity ON ProductCapacity.Id = ProductClassCapacity.CapacityId WHERE  ProductClassCapacity.ProductClassId = " + productClassID;
            
                using (SqlCommand command = new SqlCommand(selectStatement, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("Id"));
                        string name = reader.GetString(reader.GetOrdinal("Name"));

                        bool selected = selecteds.ContainsKey(id);
                        string option = "<option " + (selected ? "selected" : "") + " value='" + id + "'>" + name + "</option>";

                        options += option;
                    }

                    
                }
            }

            if (options.Length > 0)
            {
                options = "<option value=''>(All)</option>" + options;
            }

            capacityInfo.Options = options;

            return capacityInfo;

        }
    }
}
