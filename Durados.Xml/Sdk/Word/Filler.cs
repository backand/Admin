using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Durados.Xml.Sdk.Word
{
    public class Filler
    {
        private static Bitmap bitmap;
        private static Graphics graphics;

        static Filler()
        {
            bitmap = new Bitmap(2000, 1000);
            graphics = Graphics.FromImage(bitmap);
        }

        public static System.Drawing.Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }

        public void Fill(string newFile, string template, Dictionary<string, object> values)
        {
            Fill(newFile, template, values, null);
        }

        public void Fill(string newFile, string template, Dictionary<string, object> values, Dictionary<string, object> settings)
        {
            if (template != newFile)
                File.Copy(template, newFile, true);

            using (WordprocessingDocument theDoc = WordprocessingDocument.Open(newFile, true))
            {
                MainDocumentPart mainPart = theDoc.MainDocumentPart;
                
                UpdateAllBlocks(mainPart, values, settings);

                mainPart.Document.Save();

                theDoc.Close();
            }
        }

        private void UpdateAllBlocks(MainDocumentPart mainPart, Dictionary<string, object> values, Dictionary<string, object> settings)
        {
            //List<SdtBlock> sdtBlocks = GetSdtBlocks(mainPart);

            //foreach (SdtBlock sdtBlock in sdtBlocks)
            //{
            //    string tagName = sdtBlock.GetTagName();
            //    object value = values[tagName];
            //    sdtBlock.Update(value);
            //}

            List<DocumentFormat.OpenXml.Wordprocessing.Tag> tags = GetAllTags(mainPart);

            foreach (DocumentFormat.OpenXml.Wordprocessing.Tag tag in tags)
            {
                string tagName = tag.Val;

                if (values.ContainsKey(tagName))
                {
                    object value = values[tagName];
                    
                    tag.Update(value, settings, mainPart);
                }
            }
        }


        //private List<SdtBlock> GetSdtBlocks(MainDocumentPart mainPart)
        //{
        //    return mainPart.Document.Body.Descendants<SdtBlock>().ToList();
        //}


        private List<DocumentFormat.OpenXml.Wordprocessing.Tag> GetAllTags(MainDocumentPart mainPart)
        {
            List<DocumentFormat.OpenXml.Wordprocessing.Tag> tags = new List<DocumentFormat.OpenXml.Wordprocessing.Tag>();

            foreach(HeaderPart header in mainPart.HeaderParts.ToList()){
                tags.AddRange(header.Header.Descendants<DocumentFormat.OpenXml.Wordprocessing.Tag>().ToList());
            }
            foreach (FooterPart footer in mainPart.FooterParts.ToList())
            {
                tags.AddRange(footer.Footer.Descendants<DocumentFormat.OpenXml.Wordprocessing.Tag>().ToList());
            }
            tags.AddRange(mainPart.Document.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Tag>().ToList());
            return tags;
        }
        
    }
}
