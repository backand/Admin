using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Durados.Xml.Sdk.Word
{
    public abstract class ABlock
    {
        public abstract BlockType BlockType
        {
            get;
        }

        //public virtual string GetTagName(SdtBlock sdtBlock)
        //{
        //    return sdtBlock.SdtProperties.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.Tag>().Val;
        //}

        public abstract void Update(DocumentFormat.OpenXml.Wordprocessing.Tag tag, object value, IDictionary<string, object> settings, MainDocumentPart mainPart);

        public static BlockType GetBlockType(DocumentFormat.OpenXml.Wordprocessing.Tag tag, object value)
        {
            if (value is DataView || (tag.Ancestors<SdtBlock>().Count() > 0 && tag.Ancestors<SdtBlock>().Single().Descendants<Table>().Count() > 0))
                return BlockType.Table;
            else if (tag.Parent.Parent is DocumentFormat.OpenXml.Wordprocessing.SdtBlock)
                return BlockType.Rich;
            else
                return BlockType.Text;
        }
    }

    public enum BlockType
    {
        Text,
        Table,
        Multi,
        Rich
    }
}
