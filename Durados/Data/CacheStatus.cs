using Durados.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Data
{
    public class CacheStatus
    {
        private ICache<string> checksumCache;

        public CacheStatus()
        {
            checksumCache = new LocalCache<string>("CacheStatus");
        }

        public void UpdateAsync(string appName, DataSet ds)
        {
            Task.Factory.StartNew(() =>
            {
                // for dataset to string
                string result = string.Empty;
                using (StringWriter sw = new StringWriter())
                {
                    ds.WriteXml(sw);
                    result = sw.ToString();
                }

                checksumCache[appName] = GetMd5Hash(result);
            });
        }

        public void ClearStatus(string appName)
        {
            checksumCache.Remove(appName);
        }

        public Dictionary<string, string> GetStatus()
        {
            return checksumCache.ToDictionary();
        }

        private string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
    }
}
