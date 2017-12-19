using Durados.DataAccess;
using Durados.Web.Mvc.Stat.Measurements;
using Durados.Web.Mvc.Stat.Measurements.Development;
using Durados.Web.Mvc.Stat.Measurements.S3;
using Durados.Web.Mvc.Stat.Measurements.Sys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
            MeasurementType[] measurementTypes2 = GetMeasurementTypes2(measurementTypes);
            HashSet<MeasurementType> h = measurementTypes2.ToHashSet<MeasurementType>();
            if (h.Contains(MeasurementType.XmlSize) && !h.Contains(MeasurementType.Factor))
            {
                h.Add(MeasurementType.Factor);
            }
            return h.ToArray();
        }
        private MeasurementType[] GetMeasurementTypes2(MeasurementType[] measurementTypes)
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
            Dictionary<string, Dictionary<string, object>> appsWithWornings = new Dictionary<string, Dictionary<string, object>>();

            int counter = 0;
            foreach (string appItem in apps)
            {
                if (!appsMeasurements.ContainsKey(appItem))
                {
                    App app = GetApp(appItem);
                    counter++;
                    appsMeasurements.Add(appItem, new Dictionary<string, object>());

                    var appRow = app.GetAppRow();
                    if (appRow == null)
                    {
                        if (!appsWithErrors.ContainsKey(appItem))
                        {
                            appsWithErrors.Add(appItem, new Dictionary<string, object>());
                        }
                        appsWithErrors[appItem].Add("MissingApp", true);

                        continue;
                    }
                    if (appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System == null)
                    {
                        if (!appsWithErrors.ContainsKey(appItem))
                        {
                            appsWithErrors.Add(appItem, new Dictionary<string, object>());
                        }
                        appsWithErrors[appItem].Add("MissingSystemConnection",true);
                        
                        continue;
                    }

                    LoadAppsMeasurments(date, measurementTypes, persist, appsMeasurements, appsWithErrors, appsWithWornings, appItem, app);

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
            result.Add("warnings", appsWithWornings);
            return result;
        }

        private void LoadAppsMeasurments(DateTime date, MeasurementType[] measurementTypes, bool persist, Dictionary<string, Dictionary<string, object>> appsMeasurements, Dictionary<string, Dictionary<string, object>> appsWithErrors, Dictionary<string, Dictionary<string, object>> appsWithWornings, string appItem, App app)
        {
            ISqlMainSchema sqlSchema = Maps.MainAppSchema;
            appsMeasurements[appItem].Add("AppName", app.AppName);
            foreach (MeasurementType measurementType in measurementTypes)
            {
                try
                {
                    using (IDbConnection connection = sqlSchema.GetNewConnection(Maps.ReportConnectionString))
                    {
                        connection.Open();

                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.Connection = connection;
                            object measurementValue = ProduceMeasurement(date, measurementType, app, persist, command);
                            appsMeasurements[appItem].Add(measurementType.ToString(), measurementValue);
                        }
                    }
                }
                catch (DbException exception)
                {
                    if (!appsWithWornings.ContainsKey(appItem))
                    {
                        appsWithWornings.Add(appItem, new Dictionary<string, object>());
                    }
                    appsMeasurements[appItem].Add(measurementType.ToString(), exception.Message);
                    appsWithWornings[appItem].Add(measurementType.ToString(), exception.Message);
                    Maps.Instance.DuradosMap.Logger.Log("stat", appItem, measurementType.ToString(), exception, 1, appItem);
                }
                catch (Exception exception)
                {
                    if (!appsWithErrors.ContainsKey(appItem))
                    {
                        appsWithErrors.Add(appItem, new Dictionary<string, object>());
                    }
                    appsMeasurements[appItem].Add(measurementType.ToString(), exception.Message);
                    appsWithErrors[appItem].Add(measurementType.ToString(), exception.Message);
                    Maps.Instance.DuradosMap.Logger.Log("stat", appItem, measurementType.ToString(), exception, 1, appItem);
                }
            }
        }

        private static App GetApp(string appItem)
        {
           
            int appId;

            bool result = int.TryParse(appItem, out appId);

            if (result)
            {  
                return new App(appId);
            }
            else
            {
                return new App(appItem);
            }
            
           
            
          
           
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

        private object ProduceMeasurement(DateTime date, MeasurementType measurementType, App app, bool persist, IDbCommand persistCommand)
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
                case MeasurementType.Factor:
                    measurement = new Factor(app, measurementType);
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
