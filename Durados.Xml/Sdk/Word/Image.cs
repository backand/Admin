using System;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace Durados.Xml.Sdk.Word
{
    public class Image
    {
        public Drawing CreateDrawingElement(MainDocumentPart mainPart, string fileName)
        {
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                Size size;
                try
                {
                    size = GetSize(fileName);
                }
                catch 
                {
                    size = new Size(4500000, 4500000);
                }
            return CreateDrawingElement(mainPart.GetIdOfPart(imagePart), size);
            
        }

        private static Drawing CreateDrawingElement(string relationshipId, Size size)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = size.Width, Cy = size.Height },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                       "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = size.Width, Cy = size.Height }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         ) { Preset = A.ShapeTypeValues.Rectangle }))
                             ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            return element;
            // Append the reference to body, the element should be in a Run.
            //wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }


        private Size GetSize(string fileName)
        {
            var maxWidthCm = 16.51;
            var img = new Bitmap(fileName);
            var widthPx = img.Width;
            var heightPx = img.Height;
            var horzRezDpi = img.HorizontalResolution;
            var vertRezDpi = img.VerticalResolution;
            const int emusPerInch = 914400;
            const int emusPerCm = 360000;
            var widthEmus = (long)(widthPx / horzRezDpi * emusPerInch);
            var heightEmus = (long)(heightPx / vertRezDpi * emusPerInch);
            var maxWidthEmus = (long)(maxWidthCm * emusPerCm);
            if (widthEmus > maxWidthEmus)
            {
                var ratio = (heightEmus * 1.0m) / widthEmus;
                widthEmus = maxWidthEmus;
                heightEmus = (long)(widthEmus * ratio);
            }

            return new Size(Convert.ToInt32(widthEmus), Convert.ToInt32(heightEmus));
        }

    }
}
