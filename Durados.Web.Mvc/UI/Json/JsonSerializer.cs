using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    
    public static class JsonSerializer
    {
        static JsonSerializer() { }

        public static string Serialize<T>(T instance)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(stream, instance);
                stream.Position = 0;

                using (StreamReader rdr = new StreamReader(stream))
                {
                    return rdr.ReadToEnd();
                }
            }
        }

        public static T Deserialize<T>(string json)
        {
            using (MemoryStream stream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                T result = (T)serializer.ReadObject(stream);
                return result;
            }
        }

        public static Dictionary<string, object> Deserialize(string json)
        {
            var jss = new JavaScriptSerializer();
            jss.MaxJsonLength = int.MaxValue;

            var dict = jss.Deserialize<dynamic>(json);

            return (Dictionary<string, object>)dict;
        }
        public static string Deserialize(Dictionary<string, object> dict)
        {
            var jss = new JavaScriptSerializer();
            return jss.Serialize(dict);
        }
    }
}
