using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Stat
{
    public abstract class Measurement
    {
        public MeasurementType MeasurementType { get; private set; }
        public Measurement(MeasurementType measurementType)
        {
            MeasurementType = measurementType;
        }
        public abstract object Get(DateTime date, string appName, bool persist);

        protected void Persist(DateTime date, int sqlConnId, object value)
        {
            using (SqlConnection connection = new SqlConnection(Maps.ReportConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "select Id from modubiz_LogStats2 with(nolock) where SqlConId = @SqlConId and LogDate = @LogDate";
                    command.Parameters.AddWithValue("SqlConId", sqlConnId);
                    command.Parameters.AddWithValue("LogDate", date);
                    object scalar = command.ExecuteScalar();
                    if (scalar == null || scalar == DBNull.Value)
                    {
                        command.CommandText = "insert into modubiz_LogStats2 (SqlConId, LogDate) values (@SqlConId, @LogDate); SELECT IDENT_CURRENT(N'[modubiz_LogStats2]') AS ID; ";
                        scalar = command.ExecuteScalar();
                    
                    }

                    command.Parameters.Clear();
                    command.CommandText = "update modubiz_LogStats2 set " + MeasurementType.ToString() + " = @value where Id = @Id";
                    command.Parameters.AddWithValue("value", value);
                    command.Parameters.AddWithValue("Id", scalar);
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

    }
}
