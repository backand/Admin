using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Windows.Utilities.AzureUploader
{
    public class FileInfo
    {
        public string Name { get; private set; }
        public double LastUpdate { get; private set; }
        public FileType FileType { get; private set; }
        public bool Exists { get; private set; }
        public string FileName { get; private set; }

        public FileInfo(string fileName)
        {
            FileName = fileName;

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            Name = fileInfo.Name;
            Exists = fileInfo.Exists;
            if (Exists)
            {
                LastUpdate = Math.Round(DateTime.Now.Subtract(fileInfo.LastWriteTime).TotalHours, 2);
            }
            FileType = fileName.ToLower().EndsWith(".xml.xml") ? FileType.Small : FileType.Big;
        }
    }

    public enum FileType
    {
        Big,
        Small
    }
}
