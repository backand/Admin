using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.DataAccess;
using System.Data;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class CronHelper
    {
        #region Consts

        private const string WHERE = "CronName= '{0}' and AppId = {1}";
        private const string TABLE_NAME = "durados_Cron";

        #endregion

        public static bool IsCronExists(string name)
        {
            string appId = Maps.Instance.GetMap().Id;
            string connectionString = Maps.Instance.DuradosMap.connectionString;
            string whereStatement = string.Format(WHERE, name, appId);
            Dictionary<string, object> values = new Dictionary<string, object>();
            return SqlGeneralAccess.Select(values, TABLE_NAME, whereStatement, connectionString).Rows.Count > 0;
        }

        public static void Create(string name, string cycle)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            string appId = Maps.Instance.GetMap().Id;
            string connectionString = Maps.Instance.DuradosMap.connectionString;
           
            int cycleNumber = Convert.ToInt32(Enum.Parse(typeof(CycleEnum), cycle));
            values.Add("CronName", name);
            values.Add("Cycle", cycleNumber);
            values.Add("AppId", appId);
            SqlGeneralAccess.Create(values, TABLE_NAME, true, connectionString);
        }

        public static void Edit(string name, string cycle,string prevName)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            string appId = Maps.Instance.GetMap().Id;

            int cycleNumber = Convert.ToInt32(Enum.Parse(typeof(CycleEnum), cycle));
            values.Add("CronName", name);
            values.Add("Cycle", cycleNumber);
            string whereStatement = string.Format(WHERE, prevName, appId);
            string connectionString = Maps.Instance.DuradosMap.connectionString;

            SqlGeneralAccess.Update(values, TABLE_NAME, whereStatement, connectionString);
        }

        public static void Delete(string name)
        {
            string appId = Maps.Instance.GetMap().Id;
            string connectionString = Maps.Instance.DuradosMap.connectionString;
            string whereStatment = string.Format(WHERE, name, appId);
            SqlGeneralAccess.Delete(TABLE_NAME, whereStatment, connectionString);
        }

        public static DataTable GetCycleCrons(int cycle)
        {
            DataTable cronsTable = new DataTable();
            string connectionString = Maps.Instance.DuradosMap.connectionString;
            string whereStatement = string.Format(" Cycle = {0}", cycle);
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("AppId", null);
            cronsTable = SqlGeneralAccess.Select(values, TABLE_NAME, whereStatement, connectionString);
            return cronsTable;
        }

        public static string GetAppName(int appId)
        {
            string appName= string.Empty;
            string connectionString = Maps.Instance.DuradosMap.connectionString;
            string sql = string.Format("select Name from [durados_App] where Id= {0}", appId);

            SqlAccess sqlAccess = new SqlAccess();
            appName = sqlAccess.ExecuteScalar(connectionString, sql);
            return appName;
        }
    }
}
