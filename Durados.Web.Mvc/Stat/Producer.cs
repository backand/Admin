using Durados.DataAccess;
using Durados.Web.Mvc.Stat.Measurements;
using Durados.Web.Mvc.Stat.Measurements.Sys;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Stat
{
    public class Producer
    {
        public object Produce(DateTime date, MeasurementType? measurementType = null, string[] apps = null, bool? persist = false)
        {
            apps = GetApps(apps);
            MeasurementType[] measurementTypes = GetMeasurementTypes(measurementType);
            return Produce(date, measurementTypes, apps, persist.Value);

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

        private object Produce(DateTime date, MeasurementType[] measurementTypes, string[] apps, bool persist)
        {
            Dictionary<string, Dictionary<string, object>> appsMeasurements = new Dictionary<string, Dictionary<string, object>>();
            
            foreach (string app in apps)
            {
                appsMeasurements.Add(app, new Dictionary<string, object>());

                foreach (MeasurementType measurementType in measurementTypes)
                {
                    object measurementValue = ProduceMeasurement(date, measurementType, app, persist);
                    appsMeasurements[app].Add(measurementType.ToString(), measurementValue);
                }
            }

            return appsMeasurements;
        }

        private string[] GetApps(string[] apps)
        {
            if (apps != null)
                return apps;

            return GetApps();
        }

        private string[] GetApps()
        {
            List<string> apps = new List<string>();
            using (SqlConnection connection = new SqlConnection(Maps.Instance.DuradosMap.Database.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(GetAppsSql(), connection))
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

        private string GetAppsSql()
        {
            return "select Name from v_StatApps";
        }

        private object ProduceMeasurement(DateTime date, MeasurementType measurementType, string appName, bool persist)
        {
            Measurement measurement = GetMeasurement(measurementType);
            return measurement.Get(date, appName, persist);
        }

        private Measurement GetMeasurement(MeasurementType measurementType)
        {
            Measurement measurement = null;

            switch (measurementType)
            {
                case MeasurementType.RegisteredUsers:
                    measurement = new RegisteredUsers(measurementType);
                    break;
                case MeasurementType.TeamSize:
                    measurement = new TeamSize(measurementType);
                    break;


                default:
                    break;
            }


            return measurement;
        }
    }
}
