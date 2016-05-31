using Durados.DataAccess;
using Durados.Web.Mvc.Stat.Measurements;
using Durados.Web.Mvc.Stat.Measurements.Development;
using Durados.Web.Mvc.Stat.Measurements.S3;
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
        public object Produce(DateTime date, MeasurementType[] measurementTypes = null, string[] apps = null, string sql = null, bool? persist = false, int bulk = 100, int sleep = 1000)
        {
            apps = GetApps(apps, sql);
            return Produce(date, GetMeasurementTypes(measurementTypes), apps, persist.Value, bulk, sleep);

        }

        private DateTime GetDate(string date)
        {
            throw new NotImplementedException();
        }

        private MeasurementType[] GetMeasurementTypes(MeasurementType[] measurementTypes)
        {
            if (measurementTypes == null || measurementTypes.Length == 0)
                return (MeasurementType[])Enum.GetValues(typeof(MeasurementType)).Cast<MeasurementType>();

            return measurementTypes;
        }

        private object Produce(DateTime date, MeasurementType[] measurementTypes, string[] apps, bool persist, int bulk, int sleep)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<string, Dictionary<string, object>> appsMeasurements = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> appsWithErrors = new Dictionary<string, Dictionary<string, object>>();

            int counter = 0;
            foreach (string appName in apps)
            {
                if (!appsMeasurements.ContainsKey(appName))
                {
                    App app = new App(appName);
                    counter++;
                    appsMeasurements.Add(appName, new Dictionary<string, object>());

                    var appRow = app.GetAppRow();
                    if (appRow == null)
                    {
                        if (!appsWithErrors.ContainsKey(appName))
                        {
                            appsWithErrors.Add(appName, new Dictionary<string, object>());
                        }
                        appsWithErrors[appName].Add(MeasurementType.TeamSize.ToString(), "Missing app");

                        continue;
                    }
                    if (appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System == null)
                    {
                        if (!appsWithErrors.ContainsKey(appName))
                        {
                            appsWithErrors.Add(appName, new Dictionary<string, object>());
                        }
                        appsWithErrors[appName].Add(MeasurementType.TeamSize.ToString(), "Missing system connection");
                            
                        continue;
                    }

                    foreach (MeasurementType measurementType in measurementTypes)
                    {
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(Maps.ReportConnectionString))
                            {
                                connection.Open();

                                using (SqlCommand command = new SqlCommand())
                                {
                                    command.Connection = connection;
                                    object measurementValue = ProduceMeasurement(date, measurementType, app, persist, command);
                                    appsMeasurements[appName].Add(measurementType.ToString(), measurementValue);
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            if (!appsWithErrors.ContainsKey(appName))
                            {
                                appsWithErrors.Add(appName, new Dictionary<string, object>());
                            }
                            appsMeasurements[appName].Add(measurementType.ToString(), exception.Message);
                            appsWithErrors[appName].Add(measurementType.ToString(), exception.Message);
                            Maps.Instance.DuradosMap.Logger.Log("stat", appName, measurementType.ToString(), exception, 1, appName);
                        }
                    }

                    if (counter % bulk == 0)
                    {
                        System.Threading.Thread.Sleep(sleep);
                    }
                }
                else
                {

                }
            }
            result.Add("apps", appsMeasurements);
            result.Add("errors", appsWithErrors);
            return result;
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
                case MeasurementType.S3FilesSize:
                    measurement = new FilesS3Measurement(app, measurementType);
                    break;
                case MeasurementType.S3HostingSize:
                    measurement = new HostingS3Measurement(app, measurementType);
                    break;
                case MeasurementType.S3NodeJsSize:
                    measurement = new NodeJsS3Measurement(app, measurementType);
                    break;


                default:
                    break;
            }


            return measurement;
        }
    }
}
