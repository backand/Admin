using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Workflow
{
    public class LambdaLogConverter
    {
        public LambdaLogConverter()
        {

        }

        public void Convert(ArrayList logs, Guid requestId)
        {
            bool ended = false;
            LambdaLog[] lambdaLogs = GetLogs(logs, out ended);
            int i = 0;
            int limit = 30;
            foreach (LambdaLog log in lambdaLogs)
            {
                if (i > limit)
                    break;

                Backand.Logger.Log(log.Value, 500, log.DateTime, requestId, true);
                    
                i++;
            }
        }

        private LambdaLog[] GetLogs(System.Collections.ArrayList logs, out bool ended)
        {
            ended = false;
            bool started = false;
            List<LambdaLog> logs2 = new List<LambdaLog>();
            string value = null;
            DateTime? dateTime = null;
            foreach (string log in logs)
            {
                if (IsLogsStart(log))
                {
                    started = true;
                    continue;
                }
                if (IsLogsEnd(log))
                {
                    ended = true;
                    break;
                }
                if (IsReport(log))
                {
                    continue;
                }

                string valuePart;
                if (IsLogStart(log, out valuePart, out dateTime))
                {
                    if (value != null)
                    {
                        logs2.Add(new LambdaLog() { DateTime = dateTime.Value, Value = value });
                    }
                    value = valuePart;
                }
                else
                {
                    value += valuePart;
                }
            }
            if (started && dateTime.HasValue)
            {
                logs2.Add(new LambdaLog() { DateTime = dateTime.Value, Value = value });
            }

            return logs2.ToArray();
        }

        private bool IsReport(string log)
        {
            return log.StartsWith("REPORT RequestId");
        }

        private bool IsLogsEnd(string log)
        {
            return log.StartsWith("END RequestId:");
        }

        private bool IsLogsStart(string log)
        {
            return log.StartsWith("START ");
        }




        private bool IsLogStart(string log, out string value, out DateTime? dateTime)
        {
            string[] split = log.Split('\t');
            DateTime d = DateTime.MinValue;
            bool isStart = split.Length > 0 && IsDate(split[0], out d);

            if (isStart)
            {
                value = split.LastOrDefault();
                dateTime = d;
            }
            else
            {
                value = log;
                dateTime = null;
            }

            return isStart;
        }

        private bool IsDate(string s, out DateTime dateTime)
        {
            return DateTime.TryParse(s, out dateTime);
        }
    }

    public class LambdaLog
    {
        public string Value;
        public DateTime DateTime;
    }

    public class LambdaLogConversionResult
    {
        public object error;
        public object data;
    }
}
