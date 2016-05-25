using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Durados.DataAccess;
using System.Data;


namespace Durados.Web.Mvc.Stat.Measurements.Development
{
    public abstract class DevMeasurement : Measurement
    {
        public DevMeasurement(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }

        public override object Get(DateTime date)
        {

            var appRow = GetAppRow();
            int sqlProduct = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SqlProductId;
            object value = null;

            using (IDbConnection connection = GetConnection(appRow))
            {
                connection.Open();
                using (IDbCommand command = GetCommand(appRow))
                {
                    command.Connection = connection;
                    command.CommandText = GetSql((SqlProduct)sqlProduct);
                    value = command.ExecuteScalar();
                }
                connection.Close();
                connection.Dispose();
            }

            return value;
        }
    }
}
