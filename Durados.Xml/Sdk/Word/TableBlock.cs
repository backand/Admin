using System;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Durados.Xml.Sdk.Word
{
    public class TableBlock : ABlock
    {
        public override BlockType BlockType
        {
            get { return BlockType.Table; }
        }

        public override void Update(DocumentFormat.OpenXml.Wordprocessing.Tag tag, object value, IDictionary<string, object> settings, MainDocumentPart mainPart)
        {
            bool? completePage = false;
            if (settings!= null && settings.ContainsKey("completePage"))
                completePage = (bool) settings["completePage"];

            int? completeHeight = 28;
            if (settings != null && settings.ContainsKey("linesInPage"))
                completeHeight = (int)settings["linesInPage"];

            Update(tag, (DataView)value, completePage, completeHeight, mainPart);
        }

        protected virtual int GetRowLines(Table table, TableRow row)
        {
            //decimal lineHeight = Convert.ToDecimal(row.GetFirstChild<TableRowProperties>().GetFirstChild<TableRowHeight>().Val.ToString());

            int maxLines = 1;
            int cellIndex = 0;
            TableGrid grid = table.GetFirstChild<TableGrid>();
            foreach (TableCell cell in row.Descendants<TableCell>().ToList())
            {
                int lines = GetLinesInCell(cell, cellIndex, grid);

                if (lines > maxLines)
                    maxLines = lines;

                cellIndex++;
            }

            return maxLines;
        }

        private int GetLinesInCell(TableCell cell, int cellIndex, TableGrid grid)
        {
            string text = cell.InnerText.Replace("\n"," ");
            //Paragraph p = cell.Descendants<Paragraph>().First();
            //Style style = p.Descendants<Style>().First();
            //DocumentFormat.OpenXml.Wordprocessing.Font f = style.Descendants<DocumentFormat.OpenXml.Wordprocessing.Font>().First();
            //string fontFamily = f.FontFamily.ToString();
            //float fontSize = Convert.ToSingle(cell.Descendants<Style>().First().Descendants<DocumentFormat.OpenXml.Wordprocessing.Font>().First().ToString());

            string fontFamily = "Calibri";
            float fontSize = 11;

            System.Drawing.Font font = new System.Drawing.Font(fontFamily, fontSize);
            float textWidth = Filler.Graphics.MeasureString(text, font).Width * 72 / Filler.Graphics.DpiX;

            float cellWidth = Convert.ToSingle(grid.Descendants<GridColumn>().ToList()[cellIndex].Width.Value) / 20;
            
            return Convert.ToInt32(Math.Ceiling(textWidth / cellWidth));

        }

        protected OpenXmlElement GetElement(DataView dataView, DataRow row, int col, MainDocumentPart mainPart)
        {
            if (dataView.Table.Columns[col].ExtendedProperties.ContainsKey("ImagePath"))
            {
                return GetImageElement(dataView, row, col, mainPart, dataView.Table.Columns[col].ExtendedProperties["ImagePath"].ToString());
            }
            else
            {
                return GetTextElement(dataView, row, col);
            }
        }

        protected OpenXmlElement GetTextElement(DataView dataView, DataRow row, int col)
        {
            string value = string.Empty;
            value = row[col].ToString();
            return new Paragraph(new Run(new Text(value)));
        }

        Image image = null;
        Image Image
        {
            get
            {
                if (image == null)
                    image = new Image();

                return image;
            }
        }

        protected OpenXmlElement GetImageElement(DataView dataView, DataRow row, int col, MainDocumentPart mainPart, string imagePath)
        {
            string value = string.Empty;
            if (row.IsNull(col) || row[col].ToString() == string.Empty)
            {
                return new Paragraph(new Run(string.Empty));
            }
            value = imagePath + row[col].ToString();
            if (!System.IO.File.Exists(value))
                return new Paragraph(new Run(string.Empty));
            return new Paragraph(new Run(Image.CreateDrawingElement(mainPart, value)));
        }


        public void Update(DocumentFormat.OpenXml.Wordprocessing.Tag tag, DataView dataView, bool? completePage, int? linesInPage, MainDocumentPart mainPart)
        {
            
            // This should return only one table.
            SdtBlock sdtBlock = tag.Ancestors<SdtBlock>().Single();

            Table theTable = sdtBlock.Descendants<Table>().Single();
            
            var tableRows = theTable.Elements<TableRow>().ToList();

            
            TableRow theRow = tableRows.Last();
            TableRow headerRow = tableRows.First();
            
            Orientation orientation = theRow.InnerText.ToLower().StartsWith("v") ? Orientation.Vertical : Orientation.Horizontal;

            for (int i = tableRows.Count() - 1; i >= 1; i--)
            {
                TableRow row = tableRows[i];

                theTable.RemoveChild(row);
            }


            var rowCells = theRow.Descendants<TableCell>().ToList();

            for (int j = 0; j < rowCells.Count; j++)
            {
                //TableCell cell = rowCells[j];
                theRow.Descendants<TableCell>().ElementAt(j).RemoveAllChildren<Paragraph>();

            }

            var headerCells = headerRow.Descendants<TableCell>().ToList();


            int totaLines = 0;

            int rowCount = 0;

            if (orientation == Orientation.Horizontal)
            {
                foreach (DataRowView row in dataView)
                {
                    TableRow rowCopy = (TableRow)theRow.CloneNode(true);

                    for (int j = 0; j < headerCells.Count; j++)
                    {
                        //TableCell cell = headerCells[j];
                        //string value = string.Empty;
                        //value = row.Row[j].ToString();
                        //rowCopy.Descendants<TableCell>().ElementAt(j).Append(new Paragraph
                        //(new Run(new Text(value))));
                        rowCopy.Descendants<TableCell>().ElementAt(j).Append(GetElement(dataView, row.Row, j, mainPart));
                    }

                    theTable.AppendChild(rowCopy);
                    rowCount++;

                    if (completePage.HasValue && completePage.Value && linesInPage.HasValue)
                        totaLines += GetRowLines(theTable, rowCopy);

                }
            }
            else
            {

                foreach (DataRowView row in dataView)
                {
                    for (int j = 0; j < headerCells.Count; j++)
                    {
                        TableRow rowCopy = (TableRow)theRow.CloneNode(true);

                        rowCopy.Descendants<TableCell>().ElementAt(0).Append(GetElement(dataView, row.Row, j, mainPart));
                        rowCopy.Descendants<TableCell>().ElementAt(1).Append(new Paragraph(new Run(new Text(""))));
                        
                        theTable.AppendChild(rowCopy);
                        rowCount++;

                        if (completePage.HasValue && completePage.Value && linesInPage.HasValue)
                            totaLines += GetRowLines(theTable, rowCopy);
                    }

                    

                }
            }


            //if (orientation == Orientation.Horizontal)
            //{
                if (completePage.HasValue && completePage.Value && linesInPage.HasValue)
                {
                    for (int i = totaLines % linesInPage.Value; i < linesInPage.Value; i++)
                    {
                        TableRow rowCopy = (TableRow)theRow.CloneNode(true);

                        for (int j = 0; j < headerCells.Count; j++)
                        {
                            string value = string.Empty;
                            TableCell cell = headerCells[j];
                            rowCopy.Descendants<TableCell>().ElementAt(j).Append(new Paragraph
                            (new Run(new Text(value))));

                        }

                        theTable.AppendChild(rowCopy);
                    }
                }
            //}
        }
    }
}
