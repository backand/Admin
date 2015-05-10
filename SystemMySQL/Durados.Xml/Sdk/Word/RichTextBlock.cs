using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Durados.Xml.Sdk.Word
{
    public class RichTextBlock : ABlock
    {
        public override BlockType BlockType
        {
            get { return BlockType.Rich; }
        }

        public override void Update(DocumentFormat.OpenXml.Wordprocessing.Tag tag, object value, IDictionary<string, object> settings, MainDocumentPart mainPart)
        {
            string s = (value == null) ? string.Empty : value.ToString();
            Update(tag, s, mainPart);
        }

        static int altChunkIdCounter = 1;
            
        public void Update(DocumentFormat.OpenXml.Wordprocessing.Tag tag, string value, MainDocumentPart mainPart)
        {
            string mainhtml = "<html><head><style type='text/css'>.catalogGeneralTable{border-collapse: collapse;text-align: left;} .catalogGeneralTable td, th{ padding: 5px; border: 1px solid #999999; } </style></head> <body style='font-family:Trebuchet MS;font-size:.9em;'>" + value + "</body></html>";


            int blockLevelCounter = 1;
            string altChunkId = String.Format("AltChunkId{0}", altChunkIdCounter++);

            AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html, altChunkId);

            using (Stream chunkStream = chunk.GetStream(FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter stringWriter = new StreamWriter(chunkStream, Encoding.UTF8)) //Encoding.UTF8 is important to remove special characters
                {
                    stringWriter.Write(mainhtml);
                }
            }

            AltChunk altChunk = new AltChunk();
            altChunk.Id = altChunkId;

            SdtBlock sdtBlock = tag.Ancestors<SdtBlock>().Single();
            sdtBlock.InsertAt(altChunk, blockLevelCounter++);
        }


        

    }
}
