using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using System.Linq;
using Durados;
using System.IO;
using System.Security.Cryptography;


namespace System
{
    public static class FileHelper
    {
        public static bool ValidateConfig(string file1, string file2)
        {
            if (!EndsProperly(file1, "</NewDataSet>"))
                return false;
            if (!Contains(file1, "<workspace>"))
                return false;

            return true;
        }

        public static bool Compare(string file1, string file2)
        {
            //return CheckSum(file1).Equals(CheckSum(file2));

            long l1 = new FileInfo(file1).Length;
            long l2 = new FileInfo(file2).Length;
            if (!l1.Equals(l2))
                return false;

            string[] lines1 = System.IO.File.ReadAllLines(file1);
            string[] lines2 = System.IO.File.ReadAllLines(file2);

            if (!lines1.Length.Equals(lines2.Length))
                return false;

            for (int i = 0; i < lines1.Length; i++)
            {
                string line1 = lines1[i];
                string line2 = lines2[i];
                if (!line1.Equals(line2))
                {
                    return false;
                }
            }

            return true;
        }

        
        public static string CheckSum(string file)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream));
                }
            }
        }

        public static bool Contains(string file, string text)
        {
            string[] lines = System.IO.File.ReadAllLines(file);

            for (int i = lines.Length - 1; i >= 0; i--)
            {
                string line = lines[i];
                if (line.ToLower().Contains(text.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool EndsProperly(string file, string ending)
        {
            string[] lines = System.IO.File.ReadAllLines(file);

            if (lines.Length == 0)
                return false;

            if (!lines[lines.Length - 1].ToLower().Equals(ending.ToLower()))
                return false;

            for (int i = lines.Length - 2; i >= 0; i--)
            {
                string line = lines[i];
                if (line.ToLower().Contains(ending.ToLower()))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
