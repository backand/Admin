using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backand.Config
{
    public class ConfigStore
    {
        private static List<ServerConfig> Configs
        {
            get
            {
                if (_configInner == null)
                {
                    var str = File.ReadAllText("data.json");
                    _configInner = JsonConvert.DeserializeObject<List<ServerConfig>>(str);
                }

                return _configInner;
            }
        }


        private static List<ServerConfig> _configInner;

        private static readonly string EnvVarName = "BackandTestEnv";
        private static readonly string DefaultKey = "default";

        public static string GetCurrentKey()
        {
            return System.Environment.GetEnvironmentVariable(EnvVarName) ?? DefaultKey;
        }

        public static ServerConfig GetConfig()
        {
            return GetConfig(GetCurrentKey());
        }
        public static ServerConfig GetConfig(string key)
        {
            return Configs.FirstOrDefault(a => a.name == key);
        }
    }
}
