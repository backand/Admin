using System;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace Durados.Web.Mvc.Logging
{
    public enum WriteToEventViewer
    {
        DoNot,
        OnlyIfDbFails,
        OnlyExceptionsAndIfDbFails,
        AllTheTime
    }

    public class Logger : Durados.Diagnostics.ILogger
    {
        //static SqlConnection connection;
        //SqlCommand command;
        //SqlConnection connection;
        string connectionString = null;
        string machineName;
        string superDeveloper = null;

        string eventViewerLogSource = Durados.Database.LongProductName;
        string eventViewerLog = "Application";
        WriteToEventViewer writeToEventViewer = WriteToEventViewer.OnlyExceptionsAndIfDbFails;

        string reportConnectionString = null;
        bool writeToReport = false;


        string logStashServer = null;
        int logStashPort;
        bool writeToLogStash = false;

        private WritingEvents events;
        public WritingEvents Events
        {
            get
            {
                if (!initiated)
                    Initiate();

                return events;
            }
            set
            {
                if (!initiated)
                    Initiate();

                events = value;
            }
        }

        private Dictionary<string, string> Actions;

        private int LogType
        {
            get
            {

                if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("LogType"))
                    return Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["LogType"]);
                else
                    return 0;
            }
        }

        private bool initiated = false;

        public Logger()
        {


        }

        public string GetConfigPath(string configPath, string filename)
        {
            if (configPath.StartsWith("~"))
                return System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Server.MapPath(configPath + filename) : string.Empty;
            else
                return configPath + filename.Replace('/', '\\');
        }

        private void Initiate()
        {
            initiated = true;

            writeToReport = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["writeToReport"] ?? "true");
            reportConnectionString = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["reportConnectionString"] ?? "true");
            if (System.Web.Configuration.WebConfigurationManager.ConnectionStrings["reportConnectionString"] != null)
                reportConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["reportConnectionString"].ConnectionString;
            else
                writeToReport = false;

            writeToLogStash = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["writeToLogStash"] ?? "true");
            //logStashConnectionString = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["LogStashConnectionString"] ?? "true");
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["logStashServer"]) && Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["logStashPort"] ?? "-1") > 0)
            {
                logStashServer = System.Configuration.ConfigurationManager.AppSettings["logStashServer"];
                logStashPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["logStashPort"]);
            }
            else
                writeToLogStash = false;

            machineName = (System.Web.HttpContext.Current != null) ? System.Web.HttpContext.Current.Server.MachineName : System.Environment.MachineName;
            superDeveloper = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["superDeveloper"] ?? "dev@devitout.com").ToLower();


            writeToEventViewer = (WriteToEventViewer)Enum.Parse(typeof(WriteToEventViewer), Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["writeToEventViewer"] ?? WriteToEventViewer.OnlyExceptionsAndIfDbFails.ToString()));
            eventViewerLog = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["eventViewerLog"] ?? "Application");
            eventViewerLogSource = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["eventViewerLogSource"] ?? Durados.Database.LongProductName);

            string logCS = "LogConnectionString";
            bool useAppPath = Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["UseAppPath"]);
            string configPath = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["configPath"] ?? "~/Config/");
            if (useAppPath)
                logCS = System.Web.HttpContext.Current.Request.ApplicationPath + "_" + logCS;

            if (System.Web.Configuration.WebConfigurationManager.ConnectionStrings[logCS] != null && connectionString == null)
                connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings[logCS].ConnectionString;

            //// deployment sql
            var deploymentPath = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["deploymentPath"] ?? "~/Deployment/");

            string logTableFileName = GetConfigPath(deploymentPath, "Sql/logTable.sql");

            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("logTableFileName"))
                logTableFileName = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["logTableFileName"]);
            logTableFileName = GetSchemaCreateFileNameForProduct(logTableFileName);
            //// deployment sql
            string logClearFileName = GetConfigPath(deploymentPath, "Sql/logClear.sql");

            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("logClearFileName"))
                logClearFileName = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["logClearFileName"]);

            //// deployment sql
            string logInsertFileName = GetConfigPath(deploymentPath, "Sql/logInsert.sql");

            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("logInsertFileName"))
                logInsertFileName = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["logInsertFileName"]);
            logInsertFileName = GetSchemaCreateFileNameForProduct(logInsertFileName);

            if (connectionString != null)
            {
                //connection = new SqlConnection(connectionString);
                //connection.Open();
                if (!SchemaExists(connectionString))
                {
                    BuildSchema(connectionString, logTableFileName);
                    //BuildSchema(connectionString, logClearFileName);
                    BuildSchema(connectionString, logInsertFileName);
                }
            }
            //command = new SqlCommand();
            //command.Connection = connection;
            //command.CommandText = "Durados_LogInsert";
            //command.CommandType = CommandType.StoredProcedure;

            Events = GetWritingEvents();

            Actions = new Dictionary<string, string>();

            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("allEventsActions"))
            {
                string[] actionsArray = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["allEventsActions"]).Split(',');
                foreach (string action in actionsArray)
                {
                    Actions.Add(action, action);
                }
            }

        }

        private string GetSchemaCreateFileNameForProduct(string logTableFileName)
        {
            if (connectionString.StartsWith("server="))
                logTableFileName = logTableFileName.Replace(".sql", "-Mysql.sql");
            return logTableFileName;
        }

        public bool IsAllEventsAction(string action)
        {
            if (!initiated)
                Initiate();

            return Actions.ContainsKey(action);
        }

        private WritingEvents GetWritingEvents()
        {
            bool beforeAction = true;
            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("beforeAction"))
                beforeAction = Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["beforeAction"]);

            bool afterAction = false;
            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("afterAction"))
                afterAction = Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["afterAction"]);

            bool beforeResult = false;
            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("beforeResult"))
                beforeResult = Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["beforeResult"]);

            bool afterResult = false;
            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("afterResult"))
                afterResult = Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["afterResult"]);

            return new WritingEvents(beforeAction, afterAction, beforeResult, afterResult);
        }


        public void BuildSchema(string connectionString, string logSchemaGeneratorFileName)
        {
            if (!initiated)
                Initiate();

            FileInfo file = new FileInfo(logSchemaGeneratorFileName);
            string script = file.OpenText().ReadToEnd();
            IDbConnection conn = GetConnection(connectionString);
            script = script.Replace("Logs", conn.Database);
            conn.Open();
            try
            {
                IDbCommand command = GetCommand(connectionString);

                command.Connection = conn;
                //command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = script;
                command.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        private IDbConnection GetConnection(string connectionString)
        {

            if (connectionString.StartsWith("server="))
                return new MySqlConnection(connectionString);
            return new SqlConnection(connectionString);
        }

        private IDbCommand GetCommand(string connectionString)
        {
            if (connectionString.StartsWith("server="))
                return new MySqlCommand();
            return new SqlCommand();

        }
        private IDbCommand GetCommand(string connectionString, string cmdText)
        {
            if (connectionString.StartsWith("server="))
                return new MySqlCommand(cmdText, new MySqlConnection(connectionString));
            return new SqlCommand(cmdText, new SqlConnection(connectionString));

        }
        private IDbCommand GetCommand(IDbConnection cnn, string cmdText)
        {
            if (cnn.ConnectionString.StartsWith("server="))
                return new MySqlCommand(cmdText, (MySqlConnection)cnn);
            return new SqlCommand(cmdText, (SqlConnection)cnn);

        }
        private bool SchemaExists(string connectionString)
        {
            string sql = GetSelectForSchemaTable(connectionString, "Durados_Log");
            IDbConnection conn = GetConnection(connectionString);
            conn.Open();
            try
            {
                IDbCommand command = GetCommand(connectionString);

                command.Connection = conn;
                //command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = sql;
                object scalar = command.ExecuteScalar();
                return scalar != null && scalar != DBNull.Value && Convert.ToInt32(scalar) == 1;
            }
            finally
            {
                conn.Close();
            }
        }

        private string GetSelectForSchemaTable(string connectionString, string table)
        {
            if (connectionString.StartsWith("server="))
                return "SELECT 1  FROM INFORMATION_SCHEMA.TABLES  WHERE   table_name = '" + table + "' AND table_schema=DATABASE();";

            return "SELECT 1 FROM sysobjects  WHERE xtype='u' AND name='" + table + "'";
        }


        public string ConnectionString
        {
            set
            {
                connectionString = value;
                //connection = new SqlConnection(connectionString);
                //try
                //{
                //    connection.Open();
                //}
                //catch (Exception exception)
                //{
                //    WriteToEventLog("Logger", "sql connection", "", exception.Message, exception.StackTrace, 1, "Failed to connect to: " + connection.DataSource + "\\" + connection.Database);
                //    throw exception;
                //}
                Initiate();
            }
        }

        private string Username
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                {
                    return "Guest";
                }
                else if (System.Web.HttpContext.Current.Items.Contains(Database.Username))
                {
                    return System.Web.HttpContext.Current.Items[Database.Username].ToString();
                }
                else if (System.Web.HttpContext.Current.User == null || System.Web.HttpContext.Current.User.Identity == null)
                {
                    return "Guest";
                }
                else
                {
                    return System.Web.HttpContext.Current.User.Identity.Name;
                }
            }
        }

        public string UserIPAddress
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                    return null;
                if (string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
                    return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
        }
        private string GetExeptionMessage(Exception exception)
        {
            string message = string.Empty;
            Exception innerException = exception;

            while (innerException != null)
            {
                message += innerException.Message + "\n\r";
                innerException = innerException.InnerException;
            }


            return message;
        }

        public Durados.Diagnostics.ILog Log(string controller, string action, string method, Exception exception, int logType, string freeText)
        {
            return Log(controller, action, method, exception, logType, freeText, DateTime.Now);
        }
        public Durados.Diagnostics.ILog LogSync(string controller, string action, string method, Exception exception, int logType, string freeText)
        {
            return LogSync(controller, action, method, exception, logType, freeText, DateTime.Now);
        }
        public Durados.Diagnostics.ILog LogSync(string controller, string action, string method, Exception exception, int logType, string freeText, DateTime time)
        {
            string message = string.Empty;
            if (exception != null)
            {
                message = GetExeptionMessage(exception);

            }

            string trace = string.Empty;
            if (exception != null)
            {
                trace = exception.StackTrace;
            }
            return LogSync(controller, action, method, message, trace, logType, freeText, time);
        }
        public Durados.Diagnostics.ILog Log(string controller, string action, string method, Exception exception, int logType, string freeText, DateTime time)
        {
            string message = string.Empty;
            if (exception != null)
            {
                message = GetExeptionMessage(exception);

            }

            string trace = string.Empty;
            if (exception != null)
            {
                trace = exception.StackTrace;
            }
            return Log(controller, action, method, message, trace, logType, freeText, time);
        }

        public bool LogFailed { get; private set; }

        public Durados.Diagnostics.ILog Log(string controller, string action, string method, string message, string trace, int logType, string freeText, DateTime time)
        {
            return Log(controller, action, method, message, trace, logType, freeText, time, null);
        }

        public Durados.Diagnostics.ILog Log(string controller, string action, string method, string message, string trace, int logType, string freeText, DateTime time, Guid? guid)
        {


            if (!initiated)
                Initiate();

            Log log = new Log();
            if (LogType < logType && logType != 500 && logType != 501)
                return null;

            //if (LogFailed)
            //{
            //    if (writeToEventViewer == WriteToEventViewer.AllTheTime || (writeToEventViewer == WriteToEventViewer.OnlyExceptionsAndIfDbFails && logType == 1))
            //    {
            //        WriteToEventLog(controller, action, method, message, trace, logType, freeText);
            //    }
            //    return log;
            //}

            string applicationName = string.Empty;
            string appName = null;
            try
            {
                applicationName = System.Web.HttpContext.Current == null ? string.Empty : System.Web.HttpContext.Current.Request.Headers["Host"];
            }
            catch { }
            appName = GetAppName();

            string username = Username;

            if (string.IsNullOrEmpty(username))
                username = "encrypted username";
            //System.Web.HttpContext.Current.Request.Url.Host !=(System.Configuration.ConfigurationManager.AppSettings["durados_appName"] ?? "www") + (System.Configuration.ConfigurationManager.AppSettings["durados_host"] ?? "durados.com")
            if (username.Equals(superDeveloper) && System.Web.Configuration.WebConfigurationManager.ConnectionStrings["LogConnectionString"].ConnectionString != connectionString)
                return null;

            string clientIP = GetClientIP();
            string clientInfo = GetClientInfo();

            if (guid != null)
            {
                using (IDbConnection sqlConnection = GetConnection(connectionString))
                {
                    using (IDbCommand command = GetCommand(sqlConnection, "Durados_LogInsert"))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        sqlConnection.Open();

                        SetCommandParametrs(command, applicationName, username, controller, action, method, message, trace, logType, freeText, time, log, guid);

                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                try
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(delegate
                    {
                        try
                        {
                            using (IDbConnection sqlConnection = GetConnection(connectionString))
                            {
                                using (IDbCommand command = GetCommand(sqlConnection, "Durados_LogInsert"))
                                {
                                    command.CommandType = CommandType.StoredProcedure;

                                    sqlConnection.Open();

                                    SetCommandParametrs(command, applicationName, username, controller, action, method, message, trace, logType, freeText, time, log, guid);

                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            try
                            {
                                LogFailed = true;
                                //SetCommandParametrs(command, controller, action, method, message, trace, logType, freeText, time, log);
                                LogSync(controller, action, method, message, trace, logType, freeText, time);
                            }
                            catch (Exception logException)
                            {
                                WriteToEventLog(logException.Message, EventLogEntryType.FailureAudit, 1);
                                WriteToEventLog(controller, action, method, message, trace, logType, freeText);
                            }
                        }

                        if (writeToReport)
                        {
                            try
                            {
                                using (IDbConnection sqlConnection = GetConnection(reportConnectionString))
                                {
                                    using (IDbCommand command = GetCommand(sqlConnection, "Durados_LogInsert"))
                                    {
                                        command.CommandType = CommandType.StoredProcedure;

                                        sqlConnection.Open();

                                        SetCommandParametrs(command, applicationName, username, controller, action, method, message, trace, logType, freeText, time, log, appName, clientIP, clientInfo, guid);

                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                            catch { }
                        }
                        if (writeToLogStash)
                        {
                            try
                            {

                                try
                                {
                                    SendLogMessage(applicationName, appName, username, controller, action, method, message, trace, logType, freeText, time, log, guid);
                                }
                                catch { }

                            }
                            catch { }
                        }
                    });
                    ////using (SqlConnection connection = new SqlConnection(connectionString))
                    ////{
                    //SetCommandParametrs(command, controller, action, method, message, trace, logType, freeText, time, log); 

                    //if (command.Connection.State != ConnectionState.Open)
                    //{
                    //    connection.Open();
                    //}
                    //IAsyncResult result = command.BeginExecuteNonQuery();
                    ////if (result.IsCompleted)
                    //    command.EndExecuteNonQuery(result);
                    ////else
                    //  //  throw new InvalidOperationException();
                    ////}
                    //    if (writeToEventViewer == WriteToEventViewer.AllTheTime || (writeToEventViewer == WriteToEventViewer.OnlyExceptionsAndIfDbFails && logType == 1))
                    //    {
                    //        WriteToEventLog(controller, action, method, message, trace, logType, freeText);
                    //    }
                }
                catch (Exception logException)
                {
                    WriteToEventLog(logException.Message, EventLogEntryType.FailureAudit, 1);
                    WriteToEventLog(controller, action, method, message, trace, logType, freeText);
                }
            }
            return log;
        }

        private string GetClientIP()
        {
            try
            {
                if (System.Web.HttpContext.Current == null)
                    return null;

                return GetUserIP();
                //return System.Web.HttpContext.Current.Server.UrlDecode(System.Web.HttpContext.Current.Request.Headers["origin"] ?? string.Empty);
            }
            catch
            {
                return null;
            }
        }

        private string GetClientInfo()
        {
            try
            {
                if (System.Web.HttpContext.Current == null)
                    return null;
                return "origin=" + System.Web.HttpContext.Current.Server.UrlDecode(System.Web.HttpContext.Current.Request.Headers["origin"] ?? string.Empty) + "; " +
                    "host=" + System.Web.HttpContext.Current.Server.UrlDecode(System.Web.HttpContext.Current.Request.Headers["host"] ?? string.Empty) + "; " +
                    "referer=" + System.Web.HttpContext.Current.Server.UrlDecode(System.Web.HttpContext.Current.Request.Headers["referer"] ?? string.Empty) + "; " +
                    "user-agent=" + System.Web.HttpContext.Current.Server.UrlDecode(System.Web.HttpContext.Current.Request.Headers["user-agent"] ?? string.Empty)

                    + "; keys=" + string.Join(",", System.Web.HttpContext.Current.Request.ServerVariables.AllKeys) + " forwarded=" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            catch
            {
                return null;
            }
        }

        private string GetAppName()
        {
            string appName = string.Empty;
            if (System.Web.HttpContext.Current == null)
                return appName;
            try
            {
                //appId = System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Items[Database.AppId] == null ? appId: int.TryParse(System.Web.HttpContext.Current.Items[Database.AppId].ToString(),out appId)?appId:appId;
                if (System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                    appName = System.Web.HttpContext.Current.Items[Database.AppName].ToString();

                if (appName == (System.Configuration.ConfigurationManager.AppSettings["durados_appName"] ?? "bko") && System.Web.HttpContext.Current.Items.Contains(Durados.Database.CurAppName))
                    appName = System.Web.HttpContext.Current.Items[Durados.Database.CurAppName].ToString();
            }
            catch { }
            return appName;
        }




        public void WriteToEventLog(string controller, string action, string method, string message, string trace, int logType, string freeText)
        {
            string sEvent = string.Format("controller: {0}; action: {1}; Method {2}; Message: {3}; trace: {4}; freeText: {5} ", controller, action, method, message, trace, freeText);

            EventLogEntryType eventLogEntryType = logType < 4 ? EventLogEntryType.Warning : EventLogEntryType.Information;

            WriteToEventLog(sEvent, eventLogEntryType, logType);

        }

        public void WriteToEventLog(string sEvent, EventLogEntryType eventLogEntryType, int id)
        {
            try
            {
                if (writeToEventViewer != WriteToEventViewer.DoNot)
                {
                    if (!(EventLog.SourceExists(eventViewerLogSource, ".")))
                    {
                        EventSourceCreationData eventSourceCreationData = new EventSourceCreationData(eventViewerLogSource, eventViewerLog);
                        EventLog.CreateEventSource(eventSourceCreationData);
                    }

                    EventLogPermission eventLogPerm = new EventLogPermission(EventLogPermissionAccess.Administer, ".");
                    eventLogPerm.PermitOnly();


                    EventLog evLog = new EventLog();
                    evLog.Source = eventViewerLogSource;
                    evLog.WriteEntry(sEvent, eventLogEntryType, id);
                    evLog.Close();
                }
            }
            catch
            {

            }
        }


        public Durados.Diagnostics.ILog LogSync(string controller, string action, string method, string message, string trace, int logType, string freeText, DateTime time)
        {
            Log log = new Log();
            try
            {
                string applicationName = System.Web.HttpContext.Current.Request.Headers["Host"];
                using (IDbConnection cnn = GetConnection(connectionString))
                {
                    cnn.Open();
                    using (IDbCommand command = GetCommand("Durados_LogInsert", connectionString))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SetCommandParametrs(command, applicationName, Username, controller, action, method, message, trace, logType, freeText, time, log);
                        command.ExecuteNonQuery();
                    }
                    cnn.Close();
                }
            }
            catch (Exception logException)
            {
                WriteToEventLog(logException.Message, EventLogEntryType.FailureAudit, 1);
                WriteToEventLog(controller, action, method, message, trace, logType, freeText);
            }
            return log;
        }
        private void SetCommandParametrs(IDbCommand cmd, string applicationName, string username, string controller, string action, string method, string message, string trace, int logType, string freeText, DateTime time, Log log, string appName, string clientIP, string clientInfo, Guid? guid = null)
        {
            SetCommandParametrs(cmd, applicationName, username, controller, action, method, message, trace, logType, freeText, time, log, guid);
            IDataParameter appNameParameter = GetNewParameter(cmd, "AppName", SqlDbType.NVarChar);
            appNameParameter.Value = appName;
            cmd.Parameters.Add(appNameParameter);
            IDataParameter clientIPParameter = GetNewParameter(cmd, "ClientIP", SqlDbType.NVarChar);
            clientIPParameter.Value = clientIP;
            cmd.Parameters.Add(clientIPParameter);
            IDataParameter clientInfoParameter = GetNewParameter(cmd, "ClientInfo", SqlDbType.NVarChar);
            clientInfoParameter.Value = clientInfo;
            cmd.Parameters.Add(clientInfoParameter);
        }
        private void SetCommandParametrs(IDbCommand cmd, string applicationName, string username, string controller, string action, string method, string message, string trace, int logType, string freeText, DateTime time, Log log, Guid? guid = null)
        {
            cmd.Parameters.Clear();
            IDataParameter timeParameter = GetNewParameter(cmd, "Time", SqlDbType.DateTime);
            timeParameter.Value = time;
            log.Time = DateTime.Now;
            cmd.Parameters.Add(timeParameter);
            IDataParameter applicationNameParameter = GetNewParameter(cmd, "ApplicationName", SqlDbType.NVarChar);
            applicationNameParameter.Value = applicationName;
            cmd.Parameters.Add(applicationNameParameter);
            IDataParameter controllerParameter = GetNewParameter(cmd, "Controller", SqlDbType.Text);
            controllerParameter.Value = controller;
            log.Controller = controller;
            cmd.Parameters.Add(controllerParameter);
            IDataParameter actionParameter = GetNewParameter(cmd, "Action", SqlDbType.Text);
            actionParameter.Value = action;
            log.Action = action;
            cmd.Parameters.Add(actionParameter);
            IDataParameter methodNameParameter = GetNewParameter(cmd, "MethodName", SqlDbType.Text);
            methodNameParameter.Value = method;
            log.MethodName = method;
            cmd.Parameters.Add(methodNameParameter);
            IDataParameter logTypeParameter = GetNewParameter(cmd, "LogType", SqlDbType.Int);
            logTypeParameter.Value = logType;
            cmd.Parameters.Add(logTypeParameter);
            IDataParameter exceptionMessageParameter = GetNewParameter(cmd, "ExceptionMessage", SqlDbType.Text);
            //if (exception == null)
            //{
            //    exceptionMessageParameter.Value = null;
            //}
            //else
            //{
            //    string message = GetExeptionMessage(exception);
            exceptionMessageParameter.Value = message;
            log.ExceptionMessage = message;
            //}
            cmd.Parameters.Add(exceptionMessageParameter);
            IDataParameter traceParameter = GetNewParameter(cmd, "Trace", SqlDbType.Text);
            //if (exception == null)
            //{
            //    traceParameter.Value = null;
            //}
            //else
            //{
            //    traceParameter.Value = exception.StackTrace;
            //    log.Trace = exception.StackTrace;
            //}
            traceParameter.Value = trace;
            log.Trace = trace;
            cmd.Parameters.Add(traceParameter);
            IDataParameter serverNameParameter = GetNewParameter(cmd, "MachineName", SqlDbType.Text);
            serverNameParameter.Value = machineName;
            log.MachineName = machineName;
            cmd.Parameters.Add(serverNameParameter);
            IDataParameter usernameParameter = GetNewParameter(cmd, "Username", SqlDbType.Text);
            usernameParameter.Value = username;
            cmd.Parameters.Add(usernameParameter);
            IDataParameter freeTextParameter = GetNewParameter(cmd, "FreeText", SqlDbType.Text);
            freeTextParameter.Value = freeText;
            cmd.Parameters.Add(freeTextParameter);
            IDataParameter guidTextParameter = GetNewParameter(cmd, "Guid", SqlDbType.UniqueIdentifier);
            guidTextParameter.Value = guid.HasValue ? guid.Value : Guid.NewGuid();
            cmd.Parameters.Add(guidTextParameter);
        }

        private IDbDataParameter GetNewParameter(System.Data.IDbCommand command, string parameterName, object value)
        {
            if (command is MySqlCommand)
                return new MySqlParameter() { ParameterName = parameterName };
            return new SqlParameter("@" + parameterName, value);
        }

        public string NowWithMilliseconds()
        {
            DateTime now = DateTime.Now;
            return now.Second.ToString().PadLeft(2, '0') + "." + now.Millisecond.ToString().PadLeft(3, '0');
        }


        public void Clear()
        {
            if (!initiated)
                Initiate();

            using (IDbConnection connection = GetConnection(connectionString))
            {
                connection.Open();
                IDbCommand command = GetCommand(connection, "truncate table Durados_Log");
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
            }
        }

        public class WritingEvents
        {
            public bool BeforeAction { get; private set; }
            public bool AfterAction { get; private set; }
            public bool BeforeResult { get; private set; }
            public bool AfterResult { get; private set; }

            public WritingEvents(bool beforeAction, bool afterAction, bool beforeResult, bool afterResult)
            {
                this.BeforeAction = beforeAction;
                this.AfterAction = afterAction;
                this.BeforeResult = beforeResult;
                this.AfterResult = afterResult;
            }
        }


        void SendLogMessage(string applicationName, string appName, string username, string controller, string action, string method, string message, string trace, int logType, string freeText, DateTime time, Logging.Log log, Guid? guid2)
        {
            if (guid2 == null)
                guid2 = Guid.NewGuid();
            SendLogMessage(appName, applicationName, username, machineName, time.ToString(), controller, action, method, logType.ToString(), message, trace, freeText, guid2.ToString());


        }

        void SendLogMessage(
             string ID,
             string ApplicationName,
             string Username,
             string MachineName,
             string Time,
             string Controller,
             string Action,
             string MethodName,
             string LogType,
             string ExceptionMessage,
             string Trace,
             string FreeText,
             string Guid
        )
        {
            LogMessage message = new LogMessage
            {
                ID = ID,
                ApplicationName = ApplicationName,
                Username = Username,
                MachineName = MachineName,
                Time = Time,
                Controller = Controller,
                Action = Action,
                MethodName = MethodName,
                LogType = LogType,
                ExceptionMessage = ExceptionMessage,
                Trace = Trace,
                FreeText = FreeText,
                Guid = Guid
            };
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsonString = javaScriptSerializer.Serialize(message);
            Connect(logStashServer, logStashPort, jsonString);

        }


        void Connect(String server, Int32 port, String message)
        {
            try
            {
                // Create a TcpClient. 
                // Note, for this client to work you need to have a TcpServer  
                // connected to the same address as specified by the server, port 
                // combination.

                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                System.Net.Sockets.NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                WriteToEventLog(e.Message, EventLogEntryType.FailureAudit, 1);
            }
            catch (System.Net.Sockets.SocketException e)
            {
                WriteToEventLog(string.Format("SocketException: {0}", e.Message), EventLogEntryType.Error, 1);
            }


        }

        public static string GetUserIP()
        {


            //var ip = (
            //    System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null
            //          && System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
            //         ? System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]
            //         : System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            var ip = (
                System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null
                      && System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                     ? System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] + ";" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                     : System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if(ip == null)
            {
                return null;
            }

            if (ip.Contains(","))
                ip = ip.Split(',').First().Trim();
            return ip;
        }
    }
}

