using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Durados.Xml.Sdk.Word
{
    public static class SdtBockExtension
    {
        public static ABlock GetBlock(this DocumentFormat.OpenXml.Wordprocessing.Tag tag, object value)
        {
            BlockType blockType = ABlock.GetBlockType(tag, value);

            switch (blockType)
            {
                case BlockType.Table:
                    return new TableBlock();

                case BlockType.Rich:
                    return new RichTextBlock();

                default:
                    return new TextBlock();
            }
        }


        public static void Update(this DocumentFormat.OpenXml.Wordprocessing.Tag tag, object value, IDictionary<string, object> settings, MainDocumentPart mainPart)
        {
            ABlock block = GetBlock(tag, value);
            try
            {
                block.Update(tag, value, settings, mainPart);
            }
            catch (Exception exception)
            {
                throw new DuradosException("Could not update the block: " + tag.Val, exception);
            }
        }
    }
}
