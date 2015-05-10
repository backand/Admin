using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
 

namespace Durados.Xml.Sdk.Excel
{
    public class Convertor
    {

        public void ExportDataTable(
            DataTable table,
            string exportFile,
            string sheetName,
            int? rowStart)
        {            
            //populate the data into the spreadsheet
            using (SpreadsheetDocument spreadsheet =
                SpreadsheetDocument.Open(exportFile, true))
            {
                WorkbookPart workbook = spreadsheet.WorkbookPart;
                //create a reference to Sheet1

                WorksheetPart worksheet = workbook.WorksheetParts.Last();

                SheetData data;
                if (string.IsNullOrEmpty(sheetName))
                {
                    data = worksheet.Worksheet.GetFirstChild<SheetData>();
                }
                else
                {
                    data = (SheetData)worksheet.Worksheet.Where(s => s.LocalName == sheetName).First();
                }
                
                //add column names to the first row
                Row header = new Row();
                if (!rowStart.HasValue)
                    rowStart = 1;
                header.RowIndex = (UInt32)rowStart;

                foreach (DataColumn column in table.Columns)
                {
                    Cell headerCell = createTextCell(
                        table.Columns.IndexOf(column) + 1,
                        rowStart.Value,
                        column.ColumnName);

                    header.AppendChild(headerCell);
                }
                data.AppendChild(header);

                //loop through each data row
                DataRow contentRow;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    contentRow = table.Rows[i];
                    data.AppendChild(createContentRow(contentRow, rowStart.Value + i + 2));
                }
            }
        }

        private Cell createTextCell(
    int columnIndex,
    int rowIndex,
    object cellValue)
        {
            Cell cell = new Cell();

            cell.DataType = CellValues.InlineString;
            cell.CellReference = getColumnName(columnIndex) + rowIndex;

            InlineString inlineString = new InlineString();
            Text t = new Text();

            t.Text = cellValue.ToString();
            inlineString.AppendChild(t);
            cell.AppendChild(inlineString);

            return cell;
        }

        private Row createContentRow(
    DataRow dataRow,
    int rowIndex)
        {
            Row row = new Row

            {
                RowIndex = (UInt32)rowIndex
            };

            for (int i = 0; i < dataRow.Table.Columns.Count; i++)
            {
                Cell dataCell = createTextCell(i + 1, rowIndex, dataRow[i]);
                row.AppendChild(dataCell);
            }
            return row;
        }

        private string getColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = String.Empty;
            int modifier;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName =
                    Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = (int)((dividend - modifier) / 26);
            }

            return columnName;
        }

        public DataTable ConvertToDataTable(string fileName)
        {
            return ConvertToDataTable(fileName, null);
        }

        public DataTable ConvertToDataTable(string fileName, string sheetName)
        {
            if (string.IsNullOrEmpty(sheetName))
                sheetName = GetFirstSheetName(fileName);

            DataTable table = ConvertToDataTableWithoutData(fileName, sheetName);

            //if (!ConfirmSheetNameOrGetFirstIfNull(fileName, ref sheetName))
            //{
            //    throw new DuradosException("Sheet name was not found in the Excel file!");
            //}

            ExcelHandler excelHandler = new ExcelHandler(fileName, sheetName);

            if (excelHandler.ReadDocument(table.Columns.Count, ref table))
            {
                excelHandler.Dispose();

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    table.Columns[i].ColumnName = table.Rows[0][i].ToString();
                }
                table.Rows.RemoveAt(0); //...so i'm taking it out here.

                return table;
            }

            throw new FileNotFoundException("Failed to convert Excel to data table");
        }


        public string GetFirstSheetName(string fileName)
        {
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                if (spreadSheetDocument == null) return null;
                Sheet sheet = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().First();
                if (sheet == null) return null;
                return sheet.Name;
            }
        }

        public DataTable ConvertToDataTableWithoutData(string fileName)
        {
            return ConvertToDataTableWithoutData(fileName, null);
        }

        public bool ConfirmSheetNameOrGetFirstIfNull(string fileName, ref string sheetName)
        {
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
            {

                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                //SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                SheetData sheetData;
                if (string.IsNullOrEmpty(sheetName))
                {
                    sheetData = workSheet.GetFirstChild<SheetData>();
                    sheetName = sheetData.LocalName;
                }
                else
                {
                    string localName = sheetName;
                    sheetData = (SheetData)workSheet.Where(s => s.LocalName == localName).First();
                    if (sheetData == null)
                        return false;
                }
            }

            return true;
        }

        public DataTable ConvertToDataTableWithoutData(string fileName, string sheetName)
        {
            DataTable dt = new DataTable();

            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
            {

                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                //SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                SheetData sheetData;
                if (string.IsNullOrEmpty(sheetName))
                {
                    sheetData = workSheet.GetFirstChild<SheetData>();
                }
                else
                {
                    sheetData = (SheetData)workSheet.Where(s => s.LocalName == sheetName).First();
                }

                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                foreach (Cell cell in rows.ElementAt(0))
                {
                    dt.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                }

                //foreach (Row row in rows) //this will also include your header row...
                //{
                //    DataRow tempRow = dt.NewRow();

                //    for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                //    {
                //        tempRow[i] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
                //    }

                //    dt.Rows.Add(tempRow);
                //}

            }
            //dt.Rows.RemoveAt(0); //...so i'm taking it out here.

            return dt;
        }


        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }
    }
}
