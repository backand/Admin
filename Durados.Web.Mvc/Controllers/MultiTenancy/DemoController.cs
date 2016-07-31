using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using System.Net;

namespace Durados.Web.Mvc.Controllers
{
    public class DemoController : CrmController
    {
        protected override string GetFtpUploadTarget(ColumnField field, string strFileName)
        {
            if (Maps.DemoFtpTempHost.Equals(field.FtpUpload.FtpHost))
            {
                return "ftp://" + Maps.DemoFtpHost + ":" + Maps.DemoFtpPort + "/" + Maps.GetCurrentAppName() + field.FtpUpload.GetFtpBasePath(strFileName);
            }
            else
            {
                return base.GetFtpUploadTarget(field, strFileName);
            }
        }

        protected override bool FtpUploadValidSize(ColumnField field, float fileSize)
        {
            if (Maps.DemoFtpTempHost.Equals(field.FtpUpload.FtpHost))
            {
                return fileSize * 1024 < Maps.DemoFtpFileSizeLimitKb;
            }
            else
            {
                return base.FtpUploadValidSize(field, fileSize);
            }
        }

        protected override bool FtpUploadValidFolderSize(ColumnField field, float fileSize)
        {
            if (Maps.DemoFtpTempHost.Equals(field.FtpUpload.FtpHost))
            {
                string target = "ftp://" + Maps.DemoFtpHost + ":" + Maps.DemoFtpPort + "/" + Maps.GetCurrentAppName() + "/";
                System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.HttpWebRequest.Create(target);
                request.Credentials = GetFtpNetworkCredential(field);

                request.UsePassive = field.FtpUpload.UsePassive;
                request.UseBinary = true;
                request.KeepAlive = false;


                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                long size = GetDirectorySize(request);
                //FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
                //Console.WriteLine(ftpResponse.ContentLength.ToString());
                //StreamReader streamReader = new StreamReader(ftpResponse.GetResponseStream());
                //string fileSizeString = streamReader.ReadToEnd();

                return size < Maps.DemoFtpFolderSizeLimitKb * 1024;
            }
            return true;
        }

        protected override System.Net.NetworkCredential GetFtpNetworkCredential(ColumnField field)
        {
            if (Maps.DemoFtpTempHost.Equals(field.FtpUpload.FtpHost))
            {
                return new System.Net.NetworkCredential(Maps.DemoFtpUser, Maps.DemoFtpPassword);
            }
            else
            {
                return base.GetFtpNetworkCredential(field);
            }
        }

        private static string GetResponseString(FtpWebRequest request)
        {
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream datastream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(datastream, System.Text.Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        private static long GetDirectorySize(FtpWebRequest request)
        {
            string result = GetResponseString(request);
            string[] lines = result.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Regex parser = new Regex(@"(?<dir>[\-dl])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)");

            long totalSize = 0;
            if (lines.Length > 0)
            {
                foreach (string line in lines)
                {
                    string[] words = line.Split(new char[1]{' '}, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string word in words)
                    {
                        long size = 0;
                        if (long.TryParse(word, out size))
                        {
                            totalSize += size;
                            continue;
                        }
                    }

                    //Match match = parser.Match(line);
                    //if (match != null)
                    //{
                    //    long size = 0;
                    //    if (long.TryParse(match.Groups["size"].Value, out size))
                    //    {
                    //        totalSize += size;
                    //    }
                    //}
                    //else
                    //{
                    //    //Console.WriteLine(line);
                    //}
                }

                return totalSize;
            }
            else
            {
                return 0;
            }
        }
    }
}



