using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;


namespace Durados.Web.Mvc.UI.Helpers
{
    public static class FtpHelper
    {
        public static string SaveUploadedFileToFtp(ColumnField field, string strFileName)
        {
            return SaveUploadedFileToFtp(field, strFileName, System.Web.HttpContext.Current.Request.Files[0].ContentType, System.Web.HttpContext.Current.Request.Files[0].InputStream);
        }
        public static string SaveUploadedFileToFtp(ColumnField field, string strFileName, string contentType, System.IO.Stream stream)
        {

            if (!field.FtpUpload.Override && CheckIfFileExistInFtp(field, strFileName))
            {
                throw new DuradosException(field.View.Database.Localizer.Translate("File with same name already exist"));
            }


            System.Net.FtpWebRequest request = CreateFtpRequest(field, strFileName);

            request.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

            byte[] buffer = new byte[stream.Length];

            //StreamReader sourceStream = new StreamReader(stream); //Only for text files !!!
            //byte[] fileContents = System.Text.Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());//Only for text files !!!

            int count = stream.Read(buffer, 0, buffer.Length);

            request.ContentLength = buffer.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(buffer, 0, buffer.Length);
            }

            System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse();

            response.Close();

            return string.Empty;//response.StatusCode / response.StatusDescription
        }

        private static bool CheckIfFileExistInFtp(ColumnField field, string strFileName)
        {
            System.Net.FtpWebRequest request = CreateFtpRequest(field, strFileName);
            request.Method = System.Net.WebRequestMethods.Ftp.GetDateTimestamp;

            try
            {
                var response = (System.Net.FtpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException ex)
            {
                var response = (System.Net.FtpWebResponse)ex.Response;
                if (response.StatusCode == System.Net.FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
                else
                {
                    throw new DuradosException(response.StatusDescription);
                }
            }

            return true;
        }

        private static System.Net.FtpWebRequest CreateFtpRequest(ColumnField field, string strFileName)
        {
            FtpUpload uploader = field.FtpUpload;

            string target = GetFtpUploadTarget(field, strFileName);

            System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.HttpWebRequest.Create(target);

            //request.Credentials = new System.Net.NetworkCredential(uploader.FtpUserName, uploader.FtpPassword);
            //request.Credentials = new System.Net.NetworkCredential(uploader.FtpUserName, uploader.GetDecryptedPassword(Map.GetConfigDatabase()));
            request.Credentials = GetFtpNetworkCredential(field);

            request.UsePassive = uploader.UsePassive;
            request.UseBinary = true;
            request.KeepAlive = false;

            return request;
        }

        private static string GetFtpUploadTarget(ColumnField field, string strFileName)
        {
            return field.FtpUpload.GetFtpFilepath(strFileName);
        }

        private static System.Net.NetworkCredential GetFtpNetworkCredential(ColumnField field)
        {
            return new System.Net.NetworkCredential(field.FtpUpload.FtpUserName, field.FtpUpload.GetDecryptedPassword(((Database)field.View.Database).Map.GetConfigDatabase()));
        }

    }
}
