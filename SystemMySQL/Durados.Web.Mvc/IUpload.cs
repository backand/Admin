using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc
{
    public class UploadFactory
    {

        public static IUpload GetUpload(Durados.Web.Mvc.ColumnField field)
        {

            if (field.IsFtpUpload)
            {
                if (field.FtpUpload.StorageType == StorageType.Ftp)
                    return field.FtpUpload;
                else if (field.FtpUpload.StorageType == StorageType.Azure)
                    return null;
                      //return new FtpAzure();
            }
            else if (field.IsUpload)
                return field.Upload;

            return null;
        }
    }
    public interface IUpload
    {
        string  DirectoryBasePath { get; }

        string GetFileTemplatePath(string fileName);
        string GetUploadPath(string fileName);

        string GetBaseUploadPath(string fileName);
       
        string TemplatePath { get;  set; }

        void CreateNewDirectory(string newDirPath);

        void CreateNewDirectory2(string newPhisicalPath);

        void DeleteOldFile(string newPhisicalPath);

        void MoveUploadedFile(string oldPath, string newPhisicalPath);

        bool IsFileExists(string oldPath);
    }
}
