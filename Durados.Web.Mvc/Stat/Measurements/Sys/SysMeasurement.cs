using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Durados.DataAccess;
using System.Data;


namespace Durados.Web.Mvc.Stat.Measurements.Sys
{
    public abstract class SysMeasurement : Measurement
    {
        public SysMeasurement(MeasurementType measurementType)
            : base(measurementType)
        {

        }

        protected virtual string[] ExcludedEmailDomains
        {
            get
            {
                return Maps.ExcludedEmailDomains.Split(',');
            }
        }


        protected virtual IDbConnection GetSystemConnection(MapDataSet.durados_AppRow appRow)
        {
            SqlPersistency persistency = new SqlPersistency();
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();

            persistency.SystemConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SystemMapsConnectionString"].ConnectionString;
            int systemSqlProduct = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System.SqlProductId;

            string connectionString = persistency.GetSystemConnection(appRow, builder).ToString();

            return SqlAccess.GetNewConnection((SqlProduct)systemSqlProduct, connectionString);

        }

        protected virtual IDbCommand GetSystemCommand(MapDataSet.durados_AppRow appRow)
        {
            int systemSqlProduct = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System.SqlProductId;

            switch ((SqlProduct)systemSqlProduct)
            {
                case SqlProduct.MySql:
                    return new MySql.Data.MySqlClient.MySqlCommand();

                case SqlProduct.SqlServer:
                    return new System.Data.SqlClient.SqlCommand();

                default:
                    return null;
            }

        }

        protected virtual ISqlTextBuilder GetSqlTextBuilder(SqlProduct sqlProduct)
        {
            switch (sqlProduct)
            {
                case SqlProduct.MySql:
                    return new MySqlTextBuilder();

                case SqlProduct.SqlServer:
                    return new SqlTextBuilder();

                default:
                    return null;
            }

        }

        protected virtual string GetExcludeEmailDomainsWhereStatement()
        {
            string sql = string.Empty;

            foreach (string domain in ExcludedEmailDomains)
            {
                sql += " Email not like '%" + domain + "%' and ";
            }

            sql += " 1=1 ";

            return sql;
        }

        protected abstract string GetSql(SqlProduct sqlProduct);

        public override object Get(DateTime date, string appName, bool persist)
        {

            var appRow = GetAppRow(appName);
            int systemSqlProduct = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System.SqlProductId;
            object value = null;

            using (IDbConnection connection = GetSystemConnection(appRow))
            {
                connection.Open();
                using (IDbCommand command = GetSystemCommand(appRow))
                {
                    command.Connection = connection;
                    command.CommandText = GetSql((SqlProduct)systemSqlProduct);
                    value = command.ExecuteScalar();
                }
            }

            if (persist)
            {
                int appConnId = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System.Id;
                Persist(date, appConnId, value);
            }

            return value;
        }

        
        private View GetAppView()
        {
            return (View)Maps.Instance.DuradosMap.Database.Views["durados_App"];
        }

        protected virtual MapDataSet.durados_AppRow GetAppRow(string appName)
        {
            View appView = GetAppView();
            Field idField = appView.Fields["Id"];

            int? id = Maps.Instance.AppExists(appName, null);

            if (!id.HasValue)
            {
                Durados.Diagnostics.EventViewer.WriteEvent("Could not find app for: " + appName);
                return null;
            }

            MapDataSet.durados_AppRow appRow = null;
            try
            {
                appRow = (MapDataSet.durados_AppRow)appView.GetDataRow(idField, id.Value.ToString(), false);
            }
            catch (Exception exception)
            {
                Durados.Diagnostics.EventViewer.WriteEvent("failed to GetDataRow for id: " + id.Value, exception);
                try
                {
                    ((DuradosMap)Maps.Instance.DuradosMap).AddSslAndAahKeyColumn();
                    appRow = (MapDataSet.durados_AppRow)appView.GetDataRow(idField, id.Value.ToString(), false);
                }
                catch (Exception exception2)
                {
                    //Durados.Diagnostics.EventViewer.WriteEvent(exception2);
                }
            }

            if (appRow == null)
            {
                return null;
            }

            return appRow;

        }
    }
}
