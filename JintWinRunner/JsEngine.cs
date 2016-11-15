using Jint;
using Jint.Runtime.Debug;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Backand
{
    public class JsEngine
    {
        public delegate string btoaHandler(string text);
    
        private readonly Engine _engine = new Engine(cfg => cfg.AllowClr(typeof(Backand.XMLHttpRequest).Assembly));

        public JsEngine()
        {
            _engine
                .SetValue("btoa", new btoaHandler(Backand.Convert.btoa));
        }

        private string xhr = null;
        private string defaultJsInfrastructureFileName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory).Replace(@"bin\Debug", string.Empty) + "jsInfrastructure.js";
        public string GetXhrWrapper()
        {
            return GetXhrWrapper(defaultJsInfrastructureFileName);
        }
        public string GetXhrWrapper(string jsInfrastructureFileName)
        {
            if (xhr == null)
            {
                if (File.Exists(jsInfrastructureFileName))
                {
                    xhr = File.ReadAllText(jsInfrastructureFileName);
                }
                else
                {
                    throw new FileNotFoundException("The js infrastructure file was not found", jsInfrastructureFileName);
                }
            }
            return xhr;
        }
        public string Execute(string code, bool overwrite = false)
        {
            _engine.SetValue("date1", DateTime.Now);

            if (!overwrite)
            {
                code = GetXhrWrapper() + "var f = function () {" + code + "}; var s = f(); ";
            }

            DebugInfo debugInfo = new DebugInfo(114);

            var s = _engine
                .Execute(code, debugInfo)
                .GetValue("s");

            object o = s.ToObject();

            
            JavaScriptSerializer jss = new JavaScriptSerializer();

            string callStack = jss.Serialize(debugInfo);

            return jss.Serialize(o);
        }

        internal void ReloadXhrWrapper()
        {
            xhr = null;
        }

       
    }

    public class Convert
    {
        public static string btoa(string text)
        {
            return System.Convert.ToBase64String(Encoding.Default.GetBytes(text));
        }
    }
}
