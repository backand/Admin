using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace Durados.Web.Mvc
{
    public class Upload : IUpload
    {
        
        [Durados.Config.Attributes.ColumnProperty()]
        public UploadFileType UploadFileType { get; set; }
        
        [Durados.Config.Attributes.ColumnProperty()]
        public UploadStorageType UploadStorageType { get; set; }
        
        [Durados.Config.Attributes.ColumnProperty()]
        public string UploadVirtualPath { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string UploadPhysicalPath { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string TemplatePath { get; set; }
        
        [Durados.Config.Attributes.ColumnProperty()]
        public string Title { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Override { get; set; }

        private int fileMaxSize;
        [Durados.Config.Attributes.ColumnProperty()]
        public int FileMaxSize { get { return fileMaxSize; } set { fileMaxSize = value; } }

        [Durados.Config.Attributes.ColumnProperty()]
        public string FileAllowedTypes { get; set; }


        const string ApplicationPathToken = "[root]";
        public string GetVirtualPath()
        {
            return UploadVirtualPath.ToLower().Replace(ApplicationPathToken, System.Web.HttpContext.Current.Request.ApplicationPath);
            
        }
        public string GetFixedVirtualPath()
        {
            string href = GetVirtualPath().Replace("\\", "/");
            if (!href.EndsWith("/")) href = href + "/";
            if (!href.StartsWith("/")) href = "/" + href;
            return href;
        }


        public Upload()
        {
            UploadFileType = UploadFileType.Image;
            UploadStorageType = UploadStorageType.File;
        }

        #region IUpload Members

        public string GetUploadPath(string fileName)
        {
            string uploadPath = fileName; ;

            if (string.IsNullOrEmpty(UploadPhysicalPath))
            {
                uploadPath = UploadVirtualPath.Replace("/", "\\");

                if (!UploadVirtualPath.StartsWith(@"~\"))
                {
                    if (!uploadPath.StartsWith(@"\"))
                    {
                        uploadPath = @"\" + uploadPath;
                    }
                    uploadPath = "~" + uploadPath;
                }
                uploadPath = System.Web.HttpContext.Current.Server.MapPath(uploadPath);
            }
            else
            {
                uploadPath = UploadPhysicalPath;
            }

            if (!uploadPath.EndsWith(@"\"))
                uploadPath = uploadPath + @"\";
            
            return uploadPath;

        }
        
        public void CreateNewDirectory(string newDirPath)
        {
            newDirPath = GetBaseUploadPath(newDirPath);
            Directory.CreateDirectory(newDirPath);
        }

        public void CreateNewDirectory2(string newPhisicalPath)
        {
            newPhisicalPath = GetBaseUploadPath(newPhisicalPath);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(newPhisicalPath);
            fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.
        }

        public void DeleteOldFile(string newPhisicalPath)
        {
            newPhisicalPath = GetBaseUploadPath(newPhisicalPath);
            if (System.IO.File.Exists(newPhisicalPath))
            {
                System.IO.File.Delete(newPhisicalPath);
            }

        }
    
        public void  MoveUploadedFile(string oldPath, string newPhisicalPath)
        {
            newPhisicalPath = GetBaseUploadPath(newPhisicalPath);
            System.IO.File.Move(oldPath, newPhisicalPath);
        }

        public bool IsFileExists(string oldPath)
        {
            return System.IO.File.Exists(oldPath);
        }



        public string GetBaseUploadPath(string fileName)
        {
            string uploadPath = GetUploadPath(string.Empty);
            System.Web.SessionState.HttpSessionState session=System.Web.HttpContext.Current.Session;

            if (session != null && Maps.Instance.GetMap() is DuradosMap && session["CurrentEditAppId"] != null && session["CurrentEditAppId"].ToString() != string.Empty)
            {
                uploadPath = string.Format("{0}{1}\\", uploadPath, session["CurrentEditAppId"]);

            }
            return uploadPath + fileName;
           


        }
            

        public string DirectoryBasePath
        {
            get { return DirectoryBasePath; }
        }

        public string GetFileTemplatePath(string fileName)
        {
            return GetUploadPath( fileName);
        }

        #endregion
    }

    public enum UploadFileType
    {
        Image,
        Other
    }

    public enum UploadStorageType
    {
        File,
        Database
    }
}
