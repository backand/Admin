using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Cron
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; set; }

        
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
    }

    public enum CycleEnum:int
    {
        Daily = 1,
        Hourly = 2
    }
}
