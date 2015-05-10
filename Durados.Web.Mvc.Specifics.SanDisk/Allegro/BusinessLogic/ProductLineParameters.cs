using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.Web.Mvc;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic
{
    public class ProductLineParameters
    {
        protected Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        public Json.ProductLineParametersInfo GetProductLineParametersInfo(int id)
        {
            Json.ProductLineParametersInfo productLineParametersInfo = new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.Json.ProductLineParametersInfo();


            using (SqlConnection connection = new SqlConnection(Map.Database.ConnectionString))
            {
                string selectStatement = "SELECT DISTINCT Parameter.Name FROM ProductLineParameter INNER JOIN Parameter ON ProductLineParameter.ParameterId = Parameter.Id INNER JOIN TechnologyProductClassCapacity INNER JOIN ProductClassCapacity ON TechnologyProductClassCapacity.ProductClassCapacityId = ProductClassCapacity.Id INNER JOIN ProductClass ON ProductClassCapacity.ProductClassId = ProductClass.Id ON ProductLineParameter.ProductLineId = ProductClass.ProductLineId WHERE  TechnologyProductClassCapacity.Id = " + id;
            
                using (SqlCommand command = new SqlCommand(selectStatement, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string name = reader.GetString(reader.GetOrdinal("Name"));
                        productLineParametersInfo.List.Add(name);
                    }


                }
            }

            
            return productLineParametersInfo;

        }

        public Json.ProductLineParametersInfo GetProductLineParametersInfoByProductLine(int id)
        {
            Json.ProductLineParametersInfo productLineParametersInfo = new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.Json.ProductLineParametersInfo();


            using (SqlConnection connection = new SqlConnection(Map.Database.ConnectionString))
            {
                string selectStatement = "SELECT DISTINCT Parameter.Name FROM     ProductLineParameter INNER JOIN Parameter ON ProductLineParameter.ParameterId = Parameter.Id INNER JOIN ProductClass ON ProductLineParameter.ProductLineId = ProductClass.ProductLineId WHERE  ProductLineParameter.ProductLineId = " + id;
                using (SqlCommand command = new SqlCommand(selectStatement, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string name = reader.GetString(reader.GetOrdinal("Name"));
                        productLineParametersInfo.List.Add(name);
                    }


                }
            }


            return productLineParametersInfo;

        }
    }
}
