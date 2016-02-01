using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ionic.Zip;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace Durados.Web.Mvc.Infrastructure
{
    public class General
    {

        public static string GetRootPath()
        {


            if (HttpContext.Current.Request.ApplicationPath == "/")
                return '/'.ToString();
            else
                return HttpContext.Current.Request.ApplicationPath + '/';

        }

        public static string StripTags(string HTML)
        {
            // Removes tags from passed HTML            
            //System.Text.RegularExpressions.Regex objRegEx = new System.Text.RegularExpressions.Regex("<[^>]*>");

            //return objRegEx.Replace(HTML, "");

            HTML = HTML.Replace("<br>", ((char)10).ToString());
            HTML = HTML.Replace("<br/>", ((char)10).ToString());
            HTML = HTML.Replace("<br />", ((char)10).ToString());
            HTML = HTML.Replace("<BR>", ((char)10).ToString());
            HTML = HTML.Replace("<BR/>", ((char)10).ToString());
            HTML = HTML.Replace("<BR />", ((char)10).ToString());
            HTML = HTML.Replace("</p>", "</p>" + ((char)10).ToString());
            HTML = HTML.Replace("</P>", "</P>" + ((char)10).ToString());

            string stripped = Stripper.StripTagsAndAttributes(HTML, new string[0] { });

            //stripped = System.Web.HttpContext.Current.Server.HtmlDecode(stripped);

            return stripped;
        }

        public static class Stripper
        {
            private static string ReplaceFirst(string haystack, string needle, string replacement)
            {
                int pos = haystack.IndexOf(needle);
                if (pos < 0) return haystack;
                return haystack.Substring(0, pos) + replacement + haystack.Substring(pos + needle.Length);
            }

            private static string ReplaceAll(string haystack, string needle, string replacement)
            {
                int pos;
                // Avoid a possible infinite loop
                if (needle == replacement) return haystack;
                while ((pos = haystack.IndexOf(needle)) > 0)
                    haystack = haystack.Substring(0, pos) + replacement + haystack.Substring(pos + needle.Length);
                return haystack;
            }

            public static string StripTags(string Input, string[] AllowedTags)
            {
                Regex StripHTMLExp = new Regex(@"(<\/?[^>]+>)");
                string Output = Input;

                foreach (Match Tag in StripHTMLExp.Matches(Input))
                {
                    string HTMLTag = Tag.Value.ToLower();
                    bool IsAllowed = false;

                    foreach (string AllowedTag in AllowedTags)
                    {
                        int offset = -1;

                        // Determine if it is an allowed tag
                        // "<tag>" , "<tag " and "</tag"
                        if (offset != 0) offset = HTMLTag.IndexOf('<' + AllowedTag + '>');
                        if (offset != 0) offset = HTMLTag.IndexOf('<' + AllowedTag + ' ');
                        if (offset != 0) offset = HTMLTag.IndexOf("</" + AllowedTag);

                        // If it matched any of the above the tag is allowed
                        if (offset == 0)
                        {
                            IsAllowed = true;
                            break;
                        }
                    }

                    // Remove tags that are not allowed
                    if (!IsAllowed) Output = ReplaceFirst(Output, Tag.Value, "");
                }

                return Output;
            }

            public static string StripTagsAndAttributes(string Input, string[] AllowedTags)
            {
                /* Remove all unwanted tags first */
                string Output = StripTags(Input, AllowedTags);

                /* Lambda functions */
                MatchEvaluator HrefMatch = m => m.Groups[1].Value + "href..;,;.." + m.Groups[2].Value;
                MatchEvaluator ClassMatch = m => m.Groups[1].Value + "class..;,;.." + m.Groups[2].Value;
                MatchEvaluator UnsafeMatch = m => m.Groups[1].Value + m.Groups[4].Value;

                /* Allow the "href" attribute */
                Output = new Regex("(<a.*)href=(.*>)").Replace(Output, HrefMatch);

                /* Allow the "class" attribute */
                Output = new Regex("(<a.*)class=(.*>)").Replace(Output, ClassMatch);

                /* Remove unsafe attributes in any of the remaining tags */
                Output = new Regex(@"(<.*) .*=(\'|\""|\w)[\w|.|(|)]*(\'|\""|\w)(.*>)").Replace(Output, UnsafeMatch);

                /* Return the allowed tags to their proper form */
                Output = ReplaceAll(Output, "..;,;..", "=");

                return Output;
            }

        }

        public static string[] UnZip(string zipFilename, string path, bool overwrite)
        {
            List<string> fileNames = new List<string>();
            using (ZipFile zip = new ZipFile(zipFilename))
            {
                foreach (var zippedFile in zip)
                {
                    fileNames.Add(zippedFile.FileName);
                }
                zip.ExtractAll(path, overwrite ? ExtractExistingFileAction.OverwriteSilently : ExtractExistingFileAction.DoNotOverwrite);
            }

            return fileNames.ToArray();
        }

        public static Dictionary<string, MemoryStream> UnZip(MemoryStream memStream)
        {
            //using (ZipArchive archive = ZipFile.Read(b))
            //{
            //    foreach (ZipArchiveEntry entry in archive.Entries)
            //    {
            //        if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            //        {
            //            entry.(Path.Combine(extractPath, entry.FullName));
            //        }
            //    }
            //} 
            Dictionary<string, MemoryStream> configFiles = new Dictionary<string, MemoryStream>();
            
            using (ZipArchive archive = new ZipArchive(memStream))
            {
                foreach(ZipArchiveEntry entry in archive.Entries)
                {

                    var ms = new MemoryStream();
                  
                       entry.Open().CopyTo(ms);
                       ms.Seek(0, SeekOrigin.Begin);
                       configFiles.Add(entry.FullName, ms);
                   
                }
            }
            
            return configFiles;
        }
        public static string Zip(string[] filenames)
        {
            return Zip(filenames, null);
        }

        public static string Zip(string[] filenames, string zipFilename)
        {
            if (filenames == null || filenames.Length == 0)
                return null;

            FileInfo file = new FileInfo(filenames[0]);
            if (string.IsNullOrEmpty(zipFilename))
            {

                zipFilename = filenames[0].TrimEnd(file.Extension.ToCharArray()) + ".zip";
            }

            if (File.Exists(zipFilename))
            {
                File.Delete(zipFilename);
            }

            using (ZipFile zip = new ZipFile())
            {
                foreach (string filename in filenames)
                {

                    if (!File.Exists(filename))
                    {
                        throw new FileNotFoundException("Configuration file was not found");
                    }


                    zip.AddFile(filename, "");


                    zip.Save(zipFilename);
                }
                zip.Dispose();
            }

            return zipFilename;
        }

        public static Stream Zip(Dictionary<string, Stream> memStreams)
        {
            MemoryStream zipStream = new MemoryStream();//)
            using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var memStream in memStreams)
                {

                    var entry = zip.CreateEntry(memStream.Key);

                    using (StreamWriter sw = new StreamWriter(entry.Open()))
                    {
                        memStream.Value.Seek(0,SeekOrigin.Begin);
                        if (memStream.Value is MemoryStream)
                        {
                            char[] buffer = System.Text.Encoding.UTF8.GetString((memStream.Value as MemoryStream).ToArray()).ToCharArray();
                            sw.Write(buffer, 0, buffer.Length);
                        }
                        else
                            throw new DuradosException("Fail  zipping configuration streams - stream is not memory stream");

                    }

                }
            }
         
            return zipStream;

        }
        //public static string Zip(string filename)
        //{
        //    return Zip(filename, null);
        //}

        //public static string Zip(string filename, string zipFilename)
        //{
        //    if (string.IsNullOrEmpty(zipFilename))
        //    {
        //        FileInfo file = new FileInfo(filename);

        //        zipFilename = filename.TrimEnd(file.Extension.ToCharArray()) + ".zip";
        //    }

        //    if (File.Exists(zipFilename))
        //    {
        //        File.Delete(zipFilename);
        //    }

        //    if (!File.Exists(filename))
        //    {
        //        throw new FileNotFoundException("Configuration file was not found");
        //    }
        //    FileStream sourceFile = File.OpenRead(filename);
        //    FileStream destFile = File.Create(zipFilename);

        //    GZipStream compStream = new GZipStream(destFile, CompressionMode.Compress);

        //    try
        //    {
        //        int theByte = sourceFile.ReadByte();
        //        while (theByte != -1)
        //        {
        //            compStream.WriteByte((byte)theByte);
        //            theByte = sourceFile.ReadByte();
        //        }
        //        return zipFilename;
        //    }
        //    finally
        //    {
        //        compStream.Dispose();
        //    } 
        //}

        public static string GetActionName()
        {
            //string url = HttpContext.Current.Request.RawUrl;
            HttpContextBase httpContextBase = new HttpContextWrapper(HttpContext.Current);
            System.Web.Routing.RouteData route = System.Web.Routing.RouteTable.Routes.GetRouteData(httpContextBase);
            if (route == null)
                return string.Empty;
            System.Web.Mvc.UrlHelper urlHelper = new System.Web.Mvc.UrlHelper(new System.Web.Routing.RequestContext(httpContextBase, route));
            //System.Web.Mvc.UrlHelper urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);

            var routeValueDictionary = urlHelper.RequestContext.RouteData.Values;

            if (!routeValueDictionary.ContainsKey("action"))
                return string.Empty;

            string actionName = routeValueDictionary["action"].ToString();
            httpContextBase = null;
            return actionName;
        }

        public static string Version()
        {
            return System.Reflection.Assembly.GetCallingAssembly()
                          .GetName()
                          .Version
                          .ToString();


        }
        public static string Version(System.Reflection.Assembly assembly)
        {
            return assembly.GetName()
                          .Version
                          .ToString();


        }

    }

    public static class GenericCopier<T>
    {
        public static T DeepCopy(object objectToCopy)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToCopy);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }

    public class ColorUtility
    {
        public static string GetGradient(string stringColor, bool isIE)
        {
            return GetGradient(stringColor, 0.5f, isIE);
        }

        public static string GetGradient(string stringColor, float slope, bool isIE)
        {
            try
            {
                Color color = GetColorFromString(stringColor);
                Color gradient = GetGradient(color, slope);
                string alpha = GetAlphaFromString(stringColor);
                return GetStringFromColor(gradient, alpha, isIE);

            }
            catch
            {
                return stringColor;
            }

        }

        private static bool HasAlphaInString(string stringColor)
        {
            return GetRgba(stringColor).Length == 4;
        }

        private static string GetAlphaFromString(string stringColor)
        {
            if (HasAlphaInString(stringColor))
                return GetRgba(stringColor)[3];

            return string.Empty;
        }

        public static Color GetGradient(Color color, float slope)
        {
            ColorConverter convertor = new ColorConverter();



            int argb = color.ToArgb();

            Color gradient = Color.FromArgb(CScale(color.R, slope), CScale(color.G, slope), CScale(color.B, slope));
            return gradient;
        }

        public static Color GetColorFromString(string colorString)
        {
            string[] rgb = GetRgba(colorString);
            int r = Convert.ToInt32(rgb[0]);
            int g = Convert.ToInt32(rgb[1]);
            int b = Convert.ToInt32(rgb[2]);
            return Color.FromArgb(r, g, b);

        }

        public static string[] GetRgba(string colorString)
        {
            return colorString.Replace("rgba", String.Empty).Replace("rgb", string.Empty).Replace("(", String.Empty).Replace(")", string.Empty).Split(',').ToArray();
        }

        public static string GetStringFromColor(Color color, string alpha, bool isIE)
        {
            //System.Drawing.ColorTranslator.
            if (string.IsNullOrEmpty(alpha))
                return string.Format("rgb({0},{1},{2})", color.R, color.G, color.B);
            else
                if (isIE)
                    return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
                else
                    return string.Format("rgba({0},{1},{2},{3})", color.R, color.G, color.B, alpha);
        }

        public static int CScale(int c, float g)
        {
            return Math.Min(Math.Max(Convert.ToInt32(255 - (255 - c) * g), 0), 255);
        }

    }
}
