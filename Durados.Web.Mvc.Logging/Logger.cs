﻿using System;
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
                return System.Web.HttpContext.Current !=null ?System.Web.HttpContext.Current.Server.MapPath(configPath + filename):string.Empty;
            else
                return configPath + filename.Replace('/', '\\');
        }

        private void Initiate()
        {
            initiated = true;

            machineName = (System.Web.HttpContext.Current !=null)?System.Web.HttpContext.Current.Server.MachineName:System.Environment.MachineName;
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
            string logTableFileName = GetConfigPath("~/Deployment/", "Sql/logTable.sql");

            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("logTableFileName"))
                logTableFileName = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["logTableFileName"]);
            logTableFileName = GetSchemaCreateFileNameForProduct(logTableFileName);
            //// deployment sql
            string logClearFileName = GetConfigPath("~/Deployment/", "Sql/logClear.sql");

            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("logClearFileName"))
                logClearFileName = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["logClearFileName"]);

            //// deployment sql
            string logInsertFileName = GetConfigPath("~/Deployment/", "Sql/logInsert.sql");

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

        private  IDbConnection GetConnection(string connectionString)
        {
           
            if (connectionString.StartsWith("server="))
                return new MySqlConnection(connectionString);
            return new SqlConnection(connectionString);
        }

        private  IDbCommand GetCommand(string connectionString)
        {
            if (connectionString.StartsWith("server="))
                return new MySqlCommand();
            return new SqlCommand();
             
        }
        private  IDbCommand GetCommand(string connectionString,string cmdText)
        {
            if (connectionString.StartsWith("server="))
                return new MySqlCommand(cmdText, new MySqlConnection(connectionString));
            return new SqlCommand(cmdText,new SqlConnection(connectionString));

        }
        private  IDbCommand GetCommand(IDbConnection cnn, string cmdText)
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
                IDbCommand command =  GetCommand(connectionString);

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

        private  string GetSelectForSchemaTable(string connectionString, string  table)
        {
            if (connectionString.StartsWith("server="))
               return "SELECT 1  FROM INFORMATION_SCHEMA.TABLES  WHERE   table_name = '" + table + "' AND table_schema=DATABASE();";
            
            return "SELECT 1 FROM sysobjects  WHERE xtype='u' AND name='"+table+"'";
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
            try
            {
                applicationName =System.Web.HttpContext.Current == null?string.Empty: System.Web.HttpContext.Current.Request.Headers["Host"];
            }
            catch{}
            string username = Username;

            if (string.IsNullOrEmpty(username))
                username = "encrypted username";
            //System.Web.HttpContext.Current.Request.Url.Host !=(System.Configuration.ConfigurationManager.AppSettings["durados_appName"] ?? "www") + (System.Configuration.ConfigurationManager.AppSettings["durados_host"] ?? "durados.com")
            if (username.Equals(superDeveloper) && System.Web.Configuration.WebConfigurationManager.ConnectionStrings["LogConnectionString"].ConnectionString != connectionString)
                return null;

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

        public void WriteToEventLog(string controller, string action, string method, string message, string trace, int logType, string freeText)
        {
            string sEvent = string.Format("controller: {0}; action: {1}; Method {2}; Message: {3}; trace: {4}; freeText: {5} ", controller, action, method, message, trace,  freeText);

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
                    using (IDbCommand command = GetCommand("Durados_LogInsert",connectionString))
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

        private void SetCommandParametrs(IDbCommand cmd, string applicationName, string username, string controller, string action, string method, string message, string trace, int logType, string freeText, DateTime time, Log log, Guid? guid = null)
        {
            cmd.Parameters.Clear();
            IDataParameter timeParameter = GetNewParameter(cmd,"Time", SqlDbType.DateTime);
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
            return new SqlParameter("@"+parameterName, value);
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
    }
}

