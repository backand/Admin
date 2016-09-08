using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Cron
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public int ID { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Schedule for the Cron.")]
        public CronType CronType { get; set; }
 

        [Durados.Config.Attributes.ColumnProperty(Description = "Schedule for the Cron.")]
        public string Schedule { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Action or Query Id.")]
        public int EntityId { get; set; }
        
        [Durados.Config.Attributes.ColumnProperty(Description = "Url for external http request.")]
        public string ExternalUrl { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Method for the http request.")]
        public string Method { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Query String for the http request.")]
        public string QueryString { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "JSON data for the POST http request.")]
        public string Data { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "JSON headers for the http request.")]
        public string Headers { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "The Description of the Cron.")]
        public string Description { get; set; }

        public bool? Active { get; set; }

        public Cron()
        {
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Override the default web config from email.")]
        public string From { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="A column name that contains the email body template.")]
        public string Template { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="The email subject.")]
        public string Subject { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="A stored procedure name.")]
        public string ProcedureName { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "A Cycle name.")]
        public CycleEnum Cycle { get; set; }



        public Database Database { get; set; }
    }

    public enum CycleEnum:int
    {
        Daily = 1,
        Hourly = 2
    }

    public enum CronType : int
    {
        Action = 1,
        Query = 2,
        External = 3,
    }

    public class CronRequestInfo
    {
        public string url { get; set; }
        public string method { get; set; }
        public string data { get; set; }
        public Dictionary<string, object> headers { get; set; }

    }
}
