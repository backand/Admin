using Durados.Web.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class FarmCachingSingeltone
    {
        public static IFarmCaching Instance
        {
            get
            {
                return instance;
            }
        }

        private static IFarmCaching instance { get; set; }

        static FarmCachingSingeltone()
        {
            instance = new DummyFarmCaching();
        }

        private FarmCachingSingeltone()
        {

        }
    }

    public class FarmCachingNormal : IFarmCaching
    {

        public void ClearInternalAddresses()
        {
            internalAddresses = null;
        }

        public void ClearInternalCache(string appName)
        {
            if (Maps.Instance.AppInCach(appName))
                RestHelper.Refresh(appName);
        }
        public void ClearMachinesCache(string appName, bool async = false)
        {
            if (async)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(delegate
                {
                    RunBulk(appName);
                });
            }
            else
            {
                RunBulk(appName);
            }

        }

        private string GetAuthorization()
        {
            return (System.Configuration.ConfigurationManager.AppSettings["farmAuth"] ?? "69F77115-495F-4C83-A9EC-0AA46714482E").ToString();

        }

        public void AppStarted()
        {
            try
            {
                AddMeToList();
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("FarmCaching", "AppStarted", "AddMeToList", exception, 1, "");
            }

            try
            {
                ClearMachinesAddresses();
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("FarmCaching", "AppStarted", "ClearMachinesAddresses", exception, 1, "");
            }

            try
            {
                LogStart();
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("FarmCaching", "AppStarted", "LogStart", exception, 1, "");
            }
        }
        public void ClearMachinesAddresses()
        {
            RunBulk();
        }

        public void AppEnded()
        {
            try
            {
                RemoveMeFromList();
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("FarmCaching", "AppEndeded", "RemoveMeFromList", exception, 1, "");
            }

            try
            {
                ClearMachinesAddresses();
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("FarmCaching", "AppEndeded", "ClearMachinesAddresses", exception, 1, "");
            }
            try
            {
                LogEnd();
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("FarmCaching", "AppStarted", "LogEnd", exception, 1, "");
            }
        }

        #region PrivateMethod
        private void LoadInternalAddresses()
        {
            HashSet<string> h = new HashSet<string>();

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(Maps.Instance.DuradosMap.connectionString))
            {
                connection.Open();

                string sql = "select address from backand_farm";
                using (System.Data.SqlClient.SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string address = reader.GetString(0);
                            if (!string.IsNullOrEmpty(address) && !address.Equals(GetMyAddress()) && !h.Contains(address))
                            {
                                h.Add(address);
                            }
                        }
                    }
                }

                connection.Close();
            }

            internalAddresses = h.ToArray();
        }

        string[] internalAddresses = null;

        string myAddress = null;

        private string GetMyAddress()
        {
            if (Maps.Debug)
            {
                myAddress = System.Configuration.ConfigurationManager.AppSettings["farmMyIntanceAddress"];
            }
            if (myAddress == null)
            {
                myAddress = Http.LocalIPAddress();
            }

            return myAddress;
        }

        private string[] GetInternalAddresses()
        {
            if (internalAddresses == null || internalAddresses.Length == 0)
            {
                LoadInternalAddresses();
            }

            return internalAddresses;
        }


        private string GetSchema()
        {
            return (System.Configuration.ConfigurationManager.AppSettings["farmSchema"] ?? "https").ToString();
        }

        private string GetPort()
        {
            return (System.Configuration.ConfigurationManager.AppSettings["farmPort"] ?? "").ToString();
        }

        private Dictionary<string, object> GetRequest(string address, string appName)
        {
            Dictionary<string, object> request = new Dictionary<string, object>();

            string port = GetPort();
            if (!string.IsNullOrEmpty(port))
            {
                port = ":" + port;
            }

            string url = GetSchema() + "://" + address + port + "/farm/reload/" + appName ?? string.Empty;
            request.Add("url", url);

            return request;
        }



        private void RunBulk(string appName = null, string[] addresses = null)
        {
            if (addresses == null || addresses.Length == 0)
            {
                addresses = GetInternalAddresses();
            }

            bulk bulk = new bulk();
            List<Dictionary<string, object>> requests = new List<Dictionary<string, object>>();

            foreach (string address in addresses)
            {
                requests.Add(GetRequest(address, appName));
            }

            bulk.Run(requests.ToArray(), GetAuthorization(), appName);

        }

        private void AddMeToList()
        {
            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(Maps.Instance.DuradosMap.connectionString))
            {
                connection.Open();

                string sql = "if not exists (select * from backand_farm where [address] = @address) begin  insert into backand_farm ([address]) values (@address) end";
                using (System.Data.SqlClient.SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("address", GetMyAddress());
                    command.ExecuteNonQuery();

                }

                connection.Close();
            }
        }

        private void RemoveMeFromList()
        {
            RemoveFromList(GetMyAddress());
        }

        private void RemoveFromList(string address)
        {
            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(Maps.Instance.DuradosMap.connectionString))
            {
                connection.Open();

                string sql = "delete from backand_farm where [address] = @address";
                using (System.Data.SqlClient.SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("address", GetMyAddress());
                    command.ExecuteNonQuery();

                }

                connection.Close();
            }
        }



        private void LogStart()
        {
            Log("started");
        }

        private void Log(string ev)
        {
            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(Maps.Instance.DuradosMap.connectionString))
            {
                connection.Open();

                string sql = "insert into backand_farmLog ([address], [event]) values (@address, @event)";
                using (System.Data.SqlClient.SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("address", GetMyAddress());
                    command.Parameters.AddWithValue("event", ev);
                    command.ExecuteNonQuery();

                }

                connection.Close();
            }
        }



        private void LogEnd()
        {
            Log("ended");
        }

        #endregion
    }
}
