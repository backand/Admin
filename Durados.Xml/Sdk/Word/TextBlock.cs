using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Durados.Xml.Sdk.Word
{
    public class TextBlock : ABlock
    {
        public override BlockType BlockType
        {
            get { return BlockType.Text; }
        }

        public override void Update(DocumentFormat.OpenXml.Wordprocessing.Tag tag, object value, IDictionary<string, object> settings, MainDocumentPart mainPart)
        {
            string s = (value == null) ? string.Empty : value.ToString();
            Update(tag, s, mainPart);
        }

        public void Update(DocumentFormat.OpenXml.Wordprocessing.Tag tag, string value, MainDocumentPart mainPart)
        {
            tag.Parent.Parent.Descendants<Text>().Single().Text = value;

        }
    }
}
