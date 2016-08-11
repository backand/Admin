using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Script.Serialization;
using System.Web;

namespace Durados.Web.Mvc.UI.Helpers
{
     public class ChartHelper
    {
        public Map Map { get; set; }
        public Database Database { get { return Map.Database; } }
        public ChartHelper(Map map)
        {
            Map = map;
        }
       
        //public virtual string GetChartConnectionString(Chart chart)
        //{
        //    foreach (View view in Map.Database.Views.Values.Where(v => v.SystemView))
        //    {
        //        string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.Name : view.EditableTableName;
        //        if (chart.SQL.Contains(tableName))
        //            return Map.Database.SystemConnectionString;
        //    }
        //    return Map.Database.ConnectionString;
        //}

        protected virtual IDbCommand GetChartCommand(string sql, IDbConnection connection)
        {
            if (connection is Npgsql.NpgsqlConnection)
                return new Npgsql.NpgsqlCommand(sql, (Npgsql.NpgsqlConnection)connection);
            else if (connection is MySql.Data.MySqlClient.MySqlConnection)
                return new MySql.Data.MySqlClient.MySqlCommand(sql, (MySql.Data.MySqlClient.MySqlConnection)connection);
            else
                return new System.Data.SqlClient.SqlCommand(sql, (System.Data.SqlClient.SqlConnection)connection);
        }

        protected virtual IDataParameter GetChartParameter(string name, object value, IDbConnection connection)
        {
            if (connection is Oracle.ManagedDataAccess.Client.OracleConnection)
                return GetParameter(SqlProduct.Oracle, name, value);
            else if (connection is Npgsql.NpgsqlConnection)
                return GetParameter(SqlProduct.Postgre, name, value);
            else if (connection is MySql.Data.MySqlClient.MySqlConnection)
                return GetParameter(SqlProduct.MySql, name, value);
            else
                return GetParameter(SqlProduct.SqlServer, name, value);
        }

        protected virtual IDbConnection GetChartConnection(Chart chart)
        {
            foreach (View view in Map.Database.Views.Values.Where(v => v.SystemView))
            {
                string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.Name : view.EditableTableName;
                if (chart.SQL.Contains(tableName))
                {
                    return GetConnection(Map.Database.SystemSqlProduct, Map.Database.SystemConnectionString);
                   
                }
                    
            }
            return GetConnection(Map.Database.SqlProduct, Map.Database.ConnectionString);
        }

        protected virtual IDataParameter GetParameter(SqlProduct sqlProduct, string name, object value)
        {
            if (sqlProduct == SqlProduct.Oracle)
                return new Oracle.ManagedDataAccess.Client.OracleParameter(name, value);
            else if (sqlProduct == SqlProduct.Postgre)
                return new Npgsql.NpgsqlParameter(name, value);
            else if (sqlProduct == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlParameter(name, value);
            else
                return new System.Data.SqlClient.SqlParameter(name, value);
        }

        protected virtual IDbConnection GetConnection(SqlProduct sqlProduct, string connectionString)
        {
            if (sqlProduct == SqlProduct.Oracle)
            {
                return new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString);
            }
            else if (sqlProduct == SqlProduct.Postgre)
            {
                Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(connectionString);
                connection.ValidateRemoteCertificateCallback += new Npgsql.ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
                return connection;
            }
            else if (sqlProduct == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            
            else
                return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        bool connection_ValidateRemoteCertificateCallback(System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }

        public virtual tblObject GetChartJsonObject(Chart chart)
        {
            switch (chart.ChartType)
            {

                case ChartType.Pie:
                    return new tblPieObject();
                case ChartType.Gauge:
                    return new tblGaugeObject(chart);
                default:
                    return new tblSeriesObject();

            }

        }

        public void SetChartProperties(Chart chart, tblObject content, string xFieldName, Dictionary<int, string> yFieldNames, List<ChartSeries> series)
        {
            content.series = series;
            content.Title = chart.Name;
            content.SubTitle = chart.SubTitle;
            content.Type = chart.ChartType.ToString().ToLower();
            content.XTitle = string.IsNullOrEmpty(chart.XTitle) ? xFieldName : chart.XTitle;
            content.YTitle = string.IsNullOrEmpty(chart.YTitle) ? string.Join(",", yFieldNames.Values.ToArray()) : chart.YTitle;
            content.Height = (chart.Height == 0) ? "340" : chart.Height.ToString();
            content.ShowTable = chart.ShowTable;
            
        }

        public void FillChartData(tblObject content, System.Data.IDataReader reader, string xFieldName, Dictionary<int, string> yFieldNames, List<ChartSeries> series, Chart chart)
        {
            SetSeriesSchema(reader, yFieldNames, xFieldName, series, chart);

            while (reader.Read())
            {
                int i = 0;
                foreach (KeyValuePair<int, string> yField in yFieldNames)
                {
                    string yFieldName = yField.Value.ToString();
                    object ob = string.IsNullOrEmpty(yFieldName) ? reader[yField.Key] : reader[yFieldName];
                    object o = ConvertChartYValue(ob);
                    //object o = (ob is DBNull) ? null : ConvertChartYValue(ob);
                    series[i].data.Add(o);
                    Type type = reader.GetFieldType(reader.GetOrdinal(yFieldName));

                    if (!(o is DBNull) && IsNumeric(type))
                    {
                        try
                        {
                            if (!content.Neg && Convert.ToDecimal(o) < 0)
                            {
                                content.Neg = true;
                            }
                        }
                        catch { }
                    }
                    i++;
                }

                content.xAxis.Add(ConvertChartXValue(reader[xFieldName]));
            }

        }

        public System.Data.IDbCommand BuildConnectionAndCommand(string userId, string userRole, Chart chart, string queryString, ref System.Data.IDbConnection connection)
        {
            //string conStr = GetChartConnectionString(chart);
            //connection = new System.Data.SqlClient.SqlConnection(conStr);

            connection = GetChartConnection(chart);

            connection.Open();

            Dictionary<string, object> parameters;
            string query = GetSelectForChart(userId, userRole, chart, queryString, out parameters);

            System.Data.IDbCommand cmd = GetChartCommand(query, connection);

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    cmd.Parameters.Add(GetChartParameter(key, parameters[key], connection));
                }
            }

            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        private string GetSelectForChart(string userId, string userRole, Chart chart, string queryString, out Dictionary<string, object> parameters)
        {
            string query = chart.SQL;
            parameters = new Dictionary<string, object>();
            //replace tokens
            if (query.Contains(Durados.Database.UserPlaceHolder) || query.ToLower().Contains(Durados.Database.UserPlaceHolder.ToLower()))
                query = query.Replace(Durados.Database.UserPlaceHolder, userId, false);
            if (query.Contains(Durados.Database.RolePlaceHolder) || query.ToLower().Contains(Durados.Database.RolePlaceHolder.ToLower()))
                query = query.Replace(Durados.Database.RolePlaceHolder, userRole, false);
            if (!string.IsNullOrEmpty(queryString))
            {
                Dictionary<string, string> dic = QueryStringToDictionary(queryString);

                foreach (KeyValuePair<string, string> token in dic)
                {
                    //Old version tokens in  [ ]
                    string key = string.Format("[{0}]", token.Key);
                    if (query.IndexOf(key, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        string paramName = string.Format("@{0}", token.Key.ReplaceNonAlphaNumeric().StripBreaks());
                        parameters.Add(paramName, token.Value);
                        query = query.Replace(key, paramName);
                    }
                    //New version tokens in {{ }}
                    string key2 =  token.Key.AsToken();
                    if (query.IndexOf(key2, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        string paramName = string.Format("@{0}", token.Key.ReplaceNonAlphaNumeric().TrimStart(Database.DictionaryPrefix.ToCharArray()).TrimEnd(Database.DictionaryPostfix.ToCharArray()));
                        parameters.Add(paramName, token.Value);
                        query = query.Replace(key2, paramName);
                    }

                }

            }
            System.Text.RegularExpressions.MatchCollection matchs = System.Text.RegularExpressions.Regex.Matches(query, @"\{\{[a-z0-9A-Z^}}]*\}\}");
            foreach (System.Text.RegularExpressions.Match match in matchs)
            {
               
                    string value = match.Value;
                    query = query.Replace(value, "NULL");
                
            }
            return query;
        }

        public static string GetDashboardUrlWithQueryString(int id)
        {

            Dictionary<string, string> queryString = new Dictionary<string, string>();
            foreach (string key in HttpContext.Current.Request.QueryString.Keys)
            {
                if (!string.IsNullOrEmpty(key) && !queryString.ContainsKey(key) && key.IndexOf("column", StringComparison.CurrentCultureIgnoreCase) < 0 && key.IndexOf("dashboardId", StringComparison.CurrentCultureIgnoreCase) < 0 && key.IndexOf("id", StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    queryString.Add(key, HttpContext.Current.Request.QueryString[key]);
                }
            }
            return "ChartsDashboard?id=" + id.ToString() + "&" + string.Join("&", queryString.Select(r => r.Key + "=" + r.Value).ToArray());

        }
        public static string GetAddChartQueryString()
        {

            Dictionary<string, string> queryString = new Dictionary<string, string>();
            foreach (string key in HttpContext.Current.Request.QueryString.Keys)
            {
                if (!string.IsNullOrEmpty(key) && !queryString.ContainsKey(key) && key.IndexOf("column", StringComparison.CurrentCultureIgnoreCase) < 0 && key.IndexOf("dashboardId", StringComparison.CurrentCultureIgnoreCase) < 0 && key.IndexOf("id", StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    queryString.Add(key, HttpContext.Current.Request.QueryString[key]);
                }
            }
            return "&" + string.Join("&", queryString.Select(r => r.Key + "=" + r.Value).ToArray());

        }
       
        private Dictionary<string, string> QueryStringToDictionary(string queryString)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            System.Collections.Specialized.NameValueCollection queryCollection = HttpUtility.ParseQueryString(queryString);
            foreach (var k in queryCollection.AllKeys)
            {
                if (k != null)
                {
                    dict.Add(k, queryCollection[k]);
                }
            }
            dict.Remove("id");
            // dict.Remove("");
            return dict;
        }


        public void SetErrorMessageValues(out string errorMessage, out string createMessage, out string userId, out string userRole)
        {
            errorMessage = "Chart place holder";
            createMessage = "Chart place holder";
            userId = Map.Database.GetUserID();
            userRole = Map.Database.GetUserRole();
            if (userRole == "Admin" || userRole == "Developer")
            {
                errorMessage = "The following {0} error occurred:<br>{1}<br><br>Please Edit or click <a class='ui-chart-edit-here' href='#' onclick='Charts.edit(\"{2}\");'>here</a> to correct.";
                createMessage = "Please Edit or click <a class='ui-chart-edit-here' href='#' onclick='Charts.edit(\"{0}\");'>here</a> to create a chart.";
            }
        }

        private static void SetSeriesSchema(System.Data.IDataReader reader, Dictionary<int, string> yFieldNames, string xFieldName, List<ChartSeries> series, Chart chart)
        {
            if (chart.ChartType == ChartType.Gauge)
            {
                if (reader.FieldCount == 0)
                    throw new DuradosException("Select statement must contain 1 column");
                string fieldName = reader.GetName(0);
                yFieldNames.Add(0, fieldName);
                series.Add(new ChartSeries() { name = fieldName, data = new List<object>() });
                return;
            }





            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = reader.GetName(i);

                if (!fieldName.Equals(xFieldName, StringComparison.InvariantCultureIgnoreCase) || string.IsNullOrEmpty(xFieldName) && i > 0)
                {
                    yFieldNames.Add(i, fieldName);
                    series.Add(new ChartSeries() { name = fieldName, data = new List<object>() });
                }

            }
           
        }

        protected virtual object ConvertChartXValue(object val)
        {
            return ConvertChartValue(val);
        }

        protected virtual object ConvertChartYValue(object val)
        {
            return ConvertChartValue(val);
        }

        protected virtual object ConvertChartValue(object val)
        {
            if (val is DateTime)
            {
                return ((DateTime)val).ToString(Database.DateFormat);
            }

            return val;
        }
       
         protected bool IsNumeric(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumeric(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;

        }

         public virtual Durados.Web.Mvc.UI.Helpers.tblObject GetChartData(string chartId, string queryString)
         {
             string errorMessage;
             string createMessage;
             string userId;
             string userRole;
             SetErrorMessageValues(out errorMessage, out createMessage, out userId, out userRole);

             if (string.IsNullOrEmpty(chartId))
                 throw new ChartIdIsMissingException();

             int id = Convert.ToInt32(chartId.Replace("Chart", ""));

             int? dashboardId = Database.GetDashboardPK(id);

             if (!dashboardId.HasValue)
                 throw new DashboardIdIsMissingException();

             if (!Map.Database.Dashboards[dashboardId.Value].Charts.ContainsKey(id))
                 throw new ChartIdNotFoundException(chartId);

             Chart chart = Map.Database.Dashboards[dashboardId.Value].Charts[id];


             if (string.IsNullOrEmpty(chart.SQL))
                 throw new SqlStatementIsMissingException();

             System.Data.IDbConnection connection = null;
             System.Data.IDataReader reader = null;
             try
             {
                 Durados.Web.Mvc.UI.Helpers.tblObject content = GetChartJsonObject(chart);
                 // Connect
                 System.Data.IDbCommand cmd = BuildConnectionAndCommand(userId, userRole, chart, queryString, ref connection);
                 // Read
                 try
                 {
                     reader = cmd.ExecuteReader();
                 }
                 catch (Exception exception)
                 {
                     throw new SqlExecutionFailureException(exception.Message);
                 }

                 string xFieldName = chart.XField;
                 Dictionary<int, string> yFieldNames = new Dictionary<int, string>();
                 //if(!string.IsNullOrEmpty(chart.YField) && chart.ChartType!=ChartType.Gauge)
                 //    yFieldNames.AddRange(chart.YField.Split(','));
                 content.Neg = false;

                 List<Durados.Web.Mvc.UI.Helpers.ChartSeries> series = new List<Durados.Web.Mvc.UI.Helpers.ChartSeries>();
                 if (reader.FieldCount >= 2 || chart.ChartType == ChartType.Gauge && reader.FieldCount == 1)
                 {
                     if (string.IsNullOrEmpty(xFieldName) || chart.ChartType == ChartType.Gauge)
                         xFieldName = reader.GetName(0);
                     FillChartData(content, reader, xFieldName, yFieldNames, series, chart);
                 }
                 else
                 {

                     string message = chart.ChartType != ChartType.Gauge ? string.Format(errorMessage, "SQL", "The select statement must contain at least two columns.", chartId) : string.Format(errorMessage, "SQL", "The select statement must contain one column.", chartId);
                     throw new SqlReturnsLessThanTwoColumnsException(chartId);
                 }

                 SetChartProperties(chart, content, xFieldName, yFieldNames, series);

                 //System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                 //return jss.Serialize(content);           
                 return content;
             }
             catch (Exception exception)
             {
                 throw new ChartException(exception.Message);
             }
             finally
             {
                 try
                 {
                     reader.Close();
                 }
                 catch { }
                 try
                 {
                     connection.Close();
                 }
                 catch { }
             }
         }

    }

     public class ChartException : DuradosException
     {
         public ChartException(string message)
             : base(message)
         {
         }

     }

     public class ChartHandledException : ChartException
     {
         public ChartHandledException(string message)
             : base(message)
         {
         }

     }

     public class ChartIdIsMissingException : ChartException
     {
         public ChartIdIsMissingException()
             : base("Chart id was not supplied")
         {
         }

     }

     public class ChartIdNotFoundException : ChartException
     {
         public ChartIdNotFoundException(string chartId)
             : base(string.Format("Chart not found with id: {0}", chartId))
         {
         }

     }

     public class DashboardIdIsMissingException : ChartException
     {
         public DashboardIdIsMissingException()
             : base("Dashboard id was not supplied")
         {
         }

     }

     public class DashboardIdNotFoundException : ChartException
     {
         public DashboardIdNotFoundException(string dashboardId)
             : base(string.Format("Dashboard not found with id: {0}", dashboardId))
         {
         }

     }

     public class SqlStatementIsMissingException : ChartHandledException
     {
         public SqlStatementIsMissingException()
             : base("SQL Statement is missing")
         {
         }

     }

     public class SqlExecutionFailureException : ChartHandledException
     {
         public SqlExecutionFailureException(string message)
             : base("The following sql exception occued:" + message)
         {
         }

     }

     public class SqlReturnsLessThanTwoColumnsException : ChartHandledException
     {
         public SqlReturnsLessThanTwoColumnsException(string chartId)
             : base(string.Format("The select statement must contain at least two columns for chart id: " + chartId))
         {
         }

     }
     public class tblObject
     {
        
         public System.Collections.ArrayList xAxis;
         public System.Collections.ArrayList yPieAxis;
         public virtual object Data { get { return series; } }
         public List<ChartSeries> series;
        
         public string Title { get; set; }

         public string SubTitle { get; set; }

         public string Type { get; set; }

         public string XTitle { get; set; }

         public string YTitle { get; set; }

         public string Height { get; set; }

         public bool ShowTable { get; set; }

         public bool Neg { get; set; }

         public tblObject()
         {
             
             xAxis = new System.Collections.ArrayList();
             series = new List<ChartSeries>();

         }
         public int? NullableInt(string str)
         {
             int i;
             if (int.TryParse(str, out i))
                 return i;
             return null;
         }



         
     }

     public class tblSeriesObject : tblObject
     {
        
         public override object Data { get { return series; } }

     }
     public class tblPieObject : tblObject
     {
         public List<Matrix> matrix
         {
             get
             {
                 Matrix matrix = new Matrix();
               
                 for (int i = 0; i < series[0].data.Count; i++)
                 {
                     matrix.data.Add(new List<object>() { xAxis[i], series[0].data[i] });
                 }

                 return new List<Matrix>() { matrix };
             }
         }

         public override object Data { get { return matrix; } }

     }
     public class Matrix
     {
         public string type { get { return ChartType.Pie.ToString().ToLower(); } }
         public List<List<object>> data;
         public Matrix()
         {
             data = new List<List<object>>();
         }

     }
     public class ChartSeries
     {
         public object name { get; set; }
         public List<object> data { get; set; }
     }

     public class tblGaugeObject:tblObject
     {
         [ScriptIgnore]
         public Chart chart { get; set; }
         public int greenFrom { get { return GetColorBandFrom(chart.GaugeGreen); } }
         public int greenTo { get { return GetColorBandTo(chart.GaugeGreen); } }
         public int yellowFrom { get { return GetColorBandFrom(chart.GaugeYellow); } }
         public int yellowTo { get { return GetColorBandTo(chart.GaugeYellow); } }
         public int redFrom { get { return GetColorBandFrom(chart.GaugeRed); } }
         public int redTo { get { return GetColorBandTo(chart.GaugeRed); } }
         public int RefreshInterval { get { return chart.RefreshInterval; } }
         public int MinValue { get { return chart.GaugeMinValue == 0 ? greenFrom : chart.GaugeMinValue; } }
         public int MaxValue { get { return chart.GaugeMaxValue == 0 ? redTo : chart.GaugeMaxValue; } }
        
         public tblGaugeObject(Chart chart)
             : base()
         {
             this.chart = chart;
         }

       

         private int GetColorBandFrom(string colorBands)
         {
            
             
             int to;
             int from;
             GetColorBands(colorBands, out  from, out    to);
             return from;
         }
         private int GetColorBandTo(string colorBands)
         {
            
             int to;
             int from;
             GetColorBands(colorBands, out  from, out    to);
             return to;
         }
         private void GetColorBands(string colorBands, out int from, out   int to)
         {

             from = 0; 
             to = 0;
            // int from =0, to = 0;
             if (string.IsNullOrEmpty(colorBands ))
                 return;
             string[] bands = colorBands.Split(',');
             int? tmp = null;
             to = bands.Length > 0 && int.TryParse(bands[0].Trim(), out to) ? to : 0;
             if (bands.Length > 1)
             {
                 tmp = NullableInt(bands[1].Trim());
             }

             if (tmp.HasValue)
             {
                 from = to;
                 to = tmp.Value;
             }


         }

        
     }

}
