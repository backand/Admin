using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.Cloning
{
    public class CopyOptions
    {
        private object copyOptions;

        public CopyOptions(bool all = false)
        {
            if (all)
            {
                Schema = true;
                Data = true;
                Configuration = true;
                Host = true;
                Files = true;
                NodeJs = true;
                Cron = true;
            }
        }

        public CopyOptions(object copyOptions)
        {
            if (copyOptions == null)
            {
                Schema = true;
                Data = true;
                Configuration = true;
                Host = true;
                Files = true;
                NodeJs = true;
            }
            else
            {
                if (copyOptions is IDictionary<string, object>)
                {
                    foreach (string key in ((IDictionary<string, object>)copyOptions).Keys)
                    {
                        if (key.ToLower() == "schema" && ((IDictionary<string, object>)copyOptions)[key].Equals(true))
                            Schema = true;
                        if (key.ToLower() == "data" && ((IDictionary<string, object>)copyOptions)[key].Equals(true))
                            Data = true;
                        if (key.ToLower() == "configuration" && ((IDictionary<string, object>)copyOptions)[key].Equals(true))
                            Configuration = true;
                        if (key.ToLower() == "host" && ((IDictionary<string, object>)copyOptions)[key].Equals(true))
                            Host = true;
                        if (key.ToLower() == "files" && ((IDictionary<string, object>)copyOptions)[key].Equals(true))
                            Files = true;
                        if (key.ToLower() == "nodeJs" && ((IDictionary<string, object>)copyOptions)[key].Equals(true))
                            NodeJs = true;
                        if (key.ToLower() == "cron" && ((IDictionary<string, object>)copyOptions)[key].Equals(true))
                            Cron = true;
                    }
                }
            }

        }
        public bool Schema { get; set; }
        public bool Data { get; set; }
        public bool Configuration { get; set; }
        public bool Host { get; set; }
        public bool Files { get; set; }
        public bool NodeJs { get; set; }
        public bool Cron { get; set; }

    }

}
