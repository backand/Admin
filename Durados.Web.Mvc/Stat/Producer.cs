using Durados.DataAccess;
using Durados.Web.Mvc.Stat.Measurements;
using Durados.Web.Mvc.Stat.Measurements.Development;
using Durados.Web.Mvc.Stat.Measurements.Sys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Stat
{
    public class Producer
    {
        public object Produce(DateTime date, MeasurementType? measurementType = null, string[] apps = null, string sql = null, bool? persist = false, int bulk = 100, int sleep = 1000)
        {
            apps = GetApps(apps, sql);
            MeasurementType[] measurementTypes = GetMeasurementTypes(measurementType);
            return Produce(date, measurementTypes, apps, persist.Value, bulk, sleep);

        }

        private DateTime GetDate(string date)
        {
            throw new NotImplementedException();
        }

        private MeasurementType[] GetMeasurementTypes(MeasurementType? measurementType)
        {
            if (measurementType.HasValue)
                return new MeasurementType[1] { measurementType.Value };

            return (MeasurementType[])Enum.GetValues(typeof(MeasurementType)).Cast<MeasurementType>();
        }

        private object Produce(DateTime date, MeasurementType[] measurementTypes, string[] apps, bool persist, int bulk, int sleep)
        {
            Dictionary<string, Dictionary<string, object>> appsMeasurements = new Dictionary<string, Dictionary<string, object>>();

            using (SqlConnection connection = new SqlConnection(Maps.ReportConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.Transaction = transaction;

                    int counter = 0;
                    foreach (string appName in apps)
                    {
                        App app = new App(appName);
                        counter++;
                        appsMeasurements.Add(appName, new Dictionary<string, object>());

                        foreach (MeasurementType measurementType in measurementTypes)
                        {
                            object measurementValue = ProduceMeasurement(date, measurementType, app, persist, command);
                            appsMeasurements[appName].Add(measurementType.ToString(), measurementValue);
                        }

                        if (counter == bulk)
                        {
                            System.Threading.Thread.Sleep(sleep);
                        }
                    }
                    transaction.Commit();
                }
            }
            return appsMeasurements;
        }

        private string[] GetApps(string[] apps, string sql)
        {
            if (apps != null)
                return apps;

            try
            {
                return GetApps(sql);
            }
            catch (Exception exception)
            {
                throw new DuradosException("Failed to run the sql " + sql, exception);
            }
        }

        private string[] GetApps(string sql)
        {
            List<string> apps = new List<string>();
            using (SqlConnection connection = new SqlConnection(Maps.ReportConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            apps.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return apps.ToArray();
        }

        private object ProduceMeasurement(DateTime date, MeasurementType measurementType, App app, bool persist, SqlCommand persistCommand)
        {
            Measurement measurement = GetMeasurement(app, measurementType);
            object value = measurement.Get(date);

            if (persist)
            {
                measurement.Persist(date, value, persistCommand);
            }

            return value;
        }

        private Measurement GetMeasurement(App app, MeasurementType measurementType)
        {
            Measurement measurement = null;

            switch (measurementType)
            {
                case MeasurementType.RegisteredUsers:
                    measurement = new RegisteredUsers(app, measurementType);
                    break;
                case MeasurementType.TeamSize:
                    measurement = new TeamSize(app, measurementType);
                    break;
                case MeasurementType.DbSize:
                    measurement = new DbSize(app, measurementType);
                    break;
                case MeasurementType.XmlSize:
                    measurement = new XmlSize(app, measurementType);
                    break;
                case MeasurementType.TotalRows:
                    measurement = new TotalRows(app, measurementType);
                    break;
                case MeasurementType.MaxTableTotalRows:
                    measurement = new MaxTableTotalRows(app, measurementType);
                    break;


                default:
                    break;
            }


            return measurement;
        }
    }
}
