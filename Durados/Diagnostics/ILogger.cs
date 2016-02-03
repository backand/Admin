using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Diagnostics
{
    public interface ILogger
    {
        void Log(string controller, string action, string method, Exception exception, int logType, string freeText);
        void Log(string controller, string action, string method, Exception exception, int logType, string freeText, DateTime time);
        void Log(string controller, string action, string method, string message, string trace, int logType, string freeText, DateTime time);
        void Log(string controller, string action, string method, string message, string trace, int logType, string freeText, DateTime time, Guid? guid);

        void WriteToEventLog(string sEvent, EventLogEntryType eventLogEntryType, int id);
        void WriteToEventLog(string controller, string action, string method, string message, string trace, int logType, string freeText);

        string ConnectionString { set; }

        void BuildSchema(string connectionString, string logSchemaGeneratorFileName);

        string NowWithMilliseconds();
    }

    public interface ILog
    {
        string MachineName { get; set; }
        string Controller { get; set; }
        string Action { get; set; }
        string MethodName { get; set; }
        string ExceptionMessage { get; set; }
        string Trace { get; set; }
        DateTime Time { get; set; }
    }

    public class EventViewer
    {
        public static void WriteEvent(Exception e)
        {
            WriteEvent(e.Message + System.Environment.NewLine + e.StackTrace);
        }
        public static void WriteEvent(string message, Exception e)
        {
            WriteEvent(message + System.Environment.NewLine + e.Message + System.Environment.NewLine + e.StackTrace);
        }

        public static void WriteEvent(string message, EventLogEntryType eventLogEntryType = EventLogEntryType.Error, int id = 2030)
        {
            string eventViewerLogSource = Durados.Database.LongProductName;
            string eventViewerLog = "Application";

            if (!(EventLog.SourceExists(eventViewerLogSource, ".")))
            {
                EventSourceCreationData eventSourceCreationData = new EventSourceCreationData(eventViewerLogSource, eventViewerLog);
                EventLog.CreateEventSource(eventSourceCreationData);
            }

            EventLogPermission eventLogPerm = new EventLogPermission(EventLogPermissionAccess.Administer, ".");
            eventLogPerm.PermitOnly();


            EventLog evLog = new EventLog();
            evLog.Source = eventViewerLogSource;
            evLog.WriteEntry(message, eventLogEntryType, id);
            evLog.Close();
        }
    }
}
