using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Durados.Xml.Sdk.Excel
{
    /// <summary>
    /// Handler for working with Excel documents
    /// </summary>
    public class ExcelHandler : IDisposable
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelHandler"/> class.
        /// </summary>
        public ExcelHandler(){}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelHandler"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        public ExcelHandler(string fileName, string sheetName)
        {
            OpenSheet(fileName, ref sheetName);
        }
        #endregion

        #region Declares

        #region Properties

        /// <summary>
        /// Gets or sets the current spreadsheet document.
        /// </summary>
        /// <value>The current document.</value>
        public SpreadsheetDocument CurrentDocument { get; private set; }
        /// <summary>
        /// Gets or sets the current filename.
        /// </summary>
        /// <value>The current filename.</value>
        public string CurrentFilename { get; private set; }
        /// <summary>
        /// Gets or sets the current sheet.
        /// </summary>
        /// <value>The current sheet.</value>
        public WorksheetPart CurrentSheet { get; set; }

        #endregion

        /// <summary>
        /// Static list of columns from A - Z
        /// </summary>
        private static readonly string[] ColumnNames = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        /// <summary>
        /// Generic list with Column names
        /// </summary>
        private static readonly List<string> ColumnNameList = new List<string>();

        #endregion

        #region Public IO functions

        #region Generel

        /// <summary>
        /// Opens the document.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="editable">if set to <c>true</c> [editable].</param>
        /// <returns></returns>
        public bool OpenDocument(string fileName, bool editable)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException("Import file was not found!");
            try
            {
                SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(fileName, editable);
                CurrentDocument = spreadSheet;
                CurrentFilename = fileName;
                return true;
            }
            catch (Exception exception)
            {
                Dispose();
                throw new DuradosXmlException("Import file is not in the correct format! <br /><br /> Please save it in XLSX format and try again.", exception);
            }           
        }
        /// <summary>
        /// Opens the document.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public bool OpenDocument(string fileName)
        {
            return OpenDocument(fileName, true);
        }

        public static void CreateNewDocument(string fileName, string filepath, DataTable dt)
        {

            CreateNewDocument(fileName, filepath, dt, false);
        }

        public static void CreateNewDocument(string fileName, string filepath, DataTable dt, bool useExistingFile)
        {

            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            FileInfo f = new FileInfo(filepath);
            SpreadsheetDocument spreadsheetDocument = null;
            WorkbookPart workbookpart = null;
            WorksheetPart worksheetPart = null;
            if (useExistingFile)
            {
                if (f.Exists)
                    spreadsheetDocument = SpreadsheetDocument.Open(filepath, true);
                else
                    throw new FileNotFoundException("Import Excel file was not found!");

                // Add a blank WorksheetPart.
                workbookpart = spreadsheetDocument.WorkbookPart;

            }
            else
            {
                if (f.Exists)
                    f.Delete();

                spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);

                // Add a WorkbookPart to the document.
                workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

            }

            // Add a WorksheetPart to the WorkbookPart.
            worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

            // Add Sheets to the Workbook.
            worksheetPart.Worksheet = new Worksheet(new SheetData());
            Sheets sheets = workbookpart.Workbook.GetFirstChild<Sheets>();
            if (sheets == null)
                sheets = workbookpart.Workbook.AppendChild<Sheets>(new Sheets());

            //check if sheet exists
            Sheet sheet = sheets.Elements<Sheet>().Where(s => s.Name == fileName).FirstOrDefault(); ;
            if(sheet != null)
                sheets.RemoveChild(sheet);

            sheet = null;
            // Get a unique ID for the new worksheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {   
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            // Append a new worksheet and associate it with the workbook.
            sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = sheetId, Name = fileName };
            sheets.Append(sheet);

            //append data
            string cl = "";
            uint row = 2;
            int index;
            Cell cell;

            SharedStringTablePart shareStringPart;
            if (spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
            {
                shareStringPart = spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                shareStringPart = spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
            }

            foreach (DataRow dr in dt.Rows)
            {
                for (int idx = 0; idx < dt.Columns.Count; idx++)
                {
                    if (idx >= 26)
                        cl = "A" + Convert.ToString(Convert.ToChar(65 + idx - 26));
                    else
                        cl = Convert.ToString(Convert.ToChar(65 + idx));

                    if (row == 2)
                    {
                        index = InsertSharedStringItem(dt.Columns[idx].ColumnName, shareStringPart);
                        cell = InsertCellInWorksheet(cl, row - 1, worksheetPart);
                        cell.CellValue = new CellValue(index.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    }

                    // Insert the text into the SharedStringTablePart.
                    index = InsertSharedStringItem(Convert.ToString(dr[idx]), shareStringPart);
                    cell = InsertCellInWorksheet(cl, row, worksheetPart);
                    cell.CellValue = new CellValue(index.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                }
                row++;
            }

            workbookpart.Workbook.Save();

            spreadsheetDocument.Close();

            
        }

        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {

            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }
            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }
                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();
            return i;
        }


        /// <summary>
        /// Opens the spreadsheet.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <returns></returns>
        public bool OpenSheet(string fileName, ref string sheetName)
        {
            OpenDocument(fileName);

            if (CurrentDocument == null) throw new FileNotFoundException("Import file was not found!");

            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = CurrentDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == s.Name).First().Name;
            }
            //.First<Sheet>()
            return OpenSheet(sheetName);
        }
        /// <summary>
        /// Opens the spreadsheet.
        /// </summary>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <returns></returns>
        public bool OpenSheet(string sheetName)
        {
            if (CurrentDocument == null) throw new FileNotFoundException("Import file was not found!");
            IEnumerable<Sheet> sheets = CurrentDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name.ToString().ToLower() == sheetName.ToLower());
            if (sheets.Count() == 0) throw new FileNotFoundException("Sheet name was not found!");
            string relationshipId = sheets.First().Id.Value;
            CurrentSheet = (WorksheetPart)CurrentDocument.WorkbookPart.GetPartById(relationshipId);
            return true;
        }

        

        #endregion

        #region Save

        /// <summary>
        /// Saves the document.
        /// </summary>
        /// <returns>True if saved</returns>
        public bool SaveDocument()
        {
            CurrentSheet = null;
            if (CurrentDocument != null) CurrentDocument.Dispose();
            CurrentDocument = null;
            CurrentFilename = null;
            return true;
        }

        #endregion

        #region Read

        /// <summary>
        /// Reads the document.
        /// </summary>
        /// <param name="columnCount">Number of columns to read.</param>
        /// <param name="result">The result list.</param>
        /// <returns>True if succes</returns>
        public bool ReadDocument(int columnCount, ref List<string[]> result)
        {
            var dataResult = new DataResult(DataResult.DataResultType.ListType);
            if (!ExecuteReadDocument(columnCount, ref dataResult)) throw new Exception("Import file is not readable!");
            result = dataResult.GetList();
            return true;
        }
        /// <summary>
        /// Reads the document.
        /// </summary>
        /// <param name="columnCount">Number of columns to read.</param>
        /// <param name="result">The result as table.</param>
        /// <returns>True if succes</returns>
        public bool ReadDocument(int columnCount, ref DataTable result)
        {
            var dataResult = new DataResult(DataResult.DataResultType.DataTableType);
            if (!ExecuteReadDocument(columnCount, ref dataResult)) throw new Exception("Import file is not readable!");
            result = dataResult.GetDataTable();
            return true;
        }

        #endregion

        #endregion

        #region Public functions



        public bool SaveErrorsDataTable(DataTable errorTable) {

            try
            {
                List<OpenXmlElement> elements = new List<OpenXmlElement>();

                //Convert DataTable Rows to OpenXml elements:

                // Sheet Columns - Headers
                elements.Add(new Row(
                        from d in errorTable.Columns.OfType<DataColumn>()
                        select (OpenXmlElement)new Cell()
                        {
                            CellValue = new CellValue(d.ColumnName),
                            DataType = CellValues.String
                        }));

                //Data
                foreach (DataRow dr in errorTable.Rows)
                {
                    elements.Add((OpenXmlElement)new Row(from dc in dr.ItemArray
                                               select (OpenXmlElement)new Cell()
                                               {
                                                   CellValue = new CellValue(dc.ToString()),
                                                   DataType = CellValues.String
                                               }));
                }


                WorksheetPart worksheetpart = CurrentDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                worksheetpart.Worksheet = new Worksheet(new SheetData(elements));
                worksheetpart.Worksheet.Save();


                Sheet sheet = new Sheet() {
                    Id = CurrentDocument.WorkbookPart.GetIdOfPart(worksheetpart), 
                    SheetId = (uint)(CurrentDocument.WorkbookPart.Workbook.Sheets.Count() + 1), 
                    Name = errorTable.TableName 
                };

                CurrentDocument.WorkbookPart.Workbook.Sheets.AppendChild(sheet);

                CurrentDocument.WorkbookPart.Workbook.Save();

                return true;
            }
            catch (Exception exception)
            {
                throw new Exception("Can't write error to sheet!",exception);
            }
        }
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="colIndex">Index of the collumn (starting at 0)</param>
        /// <returns></returns>
        public static string GetColumnName(int colIndex)
        {
            if (colIndex < 0) return "#";
            if (ColumnNameList.Count <= colIndex)
            {
                for (int index = ColumnNameList.Count; index < (colIndex+1); index++)
                {
                    string colName;
                    if (index >= ColumnNames.Length)
                    {
                        var subIndex = (int) Math.Floor((double) index/ColumnNames.Length) - 1;
                        int sufIndex = (index - ((subIndex + 1)*ColumnNames.Length));
                        colName = GetColumnName(subIndex) + GetColumnName(sufIndex);
                    }
                    else colName = ColumnNames[index];
                    ColumnNameList.Add(colName);
                }
            }
            return ColumnNameList[colIndex];
        }

        /// <summary>
        /// Creates the XML font.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <returns>Font</returns>
        public Font CreateXmlFont(System.Drawing.Font font)
        {
            var xmlFont = new Font();
            xmlFont.Append(new FontName { Val = font.Name });
            xmlFont.Append(new FontSize { Val = font.SizeInPoints });
            //xmlFont.Append(new FontFamily() {Val = font.FontFamily.Name});
            if (font.Bold) xmlFont.Append(new Bold());
            if (font.Italic) xmlFont.Append(new Italic());
            if (font.Underline) xmlFont.Append(new Underline());
            return xmlFont;
        }

        #endregion

        #region Public commands

        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        /// <param name="align">The align.</param>
        /// <param name="valueType">Type of the value.</param>
        public void InsertValue(string text, string column, int rowindex, HorizontalAlignmentValues? align, CellValues valueType)
        {
            if (CurrentSheet == null) throw new Exception("No sheet selected");
            Cell cell = LocateCell(column, rowindex);
            cell.CellValue = new CellValue(text);
            cell.DataType = new EnumValue<CellValues>(valueType);
            CurrentSheet.Worksheet.Save();            
        }
        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        /// <param name="align">The align.</param>
        public void InsertValue(string text, string column, int rowindex, HorizontalAlignmentValues? align)
        {
            InsertValue(text, column, rowindex, align, CellValues.String);
        }
        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        /// <param name="valueType">Type of the value.</param>
        public void InsertValue(string text, string column, int rowindex, CellValues valueType)
        {
            InsertValue(text, column, rowindex, null, valueType);
        }
        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        public void InsertValue(string text, string column, int rowindex)
        {
            InsertValue(text, column, rowindex, CellValues.String);
        }

        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        /// <param name="align">The align.</param>
        public void InsertValue(int value, string column, int rowindex, HorizontalAlignmentValues? align)
        {
            InsertValue((double)value, column, rowindex, align);
        }
        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        public void InsertValue(int value, string column, int rowindex)
        {
            InsertValue((double)value, column, rowindex);
        }

        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        /// <param name="align">The align.</param>
        public void InsertValue(double value, string column, int rowindex, HorizontalAlignmentValues? align)
        {
            InsertValue(value.ToString(), column, rowindex, align, CellValues.Number);
        }
        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        public void InsertValue(double value, string column, int rowindex)
        {
            InsertValue(value, column, rowindex, HorizontalAlignmentValues.Right);
        }

        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        /// <param name="align">The align.</param>
        public void InsertValue(bool value, string column, int rowindex, HorizontalAlignmentValues? align)
        {
            string svalue = "0";
            if (value) svalue = "1";
            InsertValue(svalue, column, rowindex, align, CellValues.Boolean);
        }
        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        public void InsertValue(bool value, string column, int rowindex)
        {
            InsertValue(value, column, rowindex, HorizontalAlignmentValues.Center);
        }

        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        public void InsertValue(DateTime value, string column, int rowindex)
        {
            InsertValue(value, column, rowindex, HorizontalAlignmentValues.Center);
        }
        /// <summary>
        /// Inserts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        /// <param name="align">The align.</param>
        public void InsertValue(DateTime value, string column, int rowindex, HorizontalAlignmentValues? align)
        {
            if (CurrentSheet == null) throw new Exception("No sheet selected");
            string svalue;
            if (value.Hour == 0 && value.Minute == 0 && value.Millisecond == 0)
            {
                svalue = value.ToString("dd/MM-yyyy");
            }
            else svalue = value.ToString("dd/MM-yyyy HH:mm:ss");
            InsertValue(svalue, column, rowindex, align, CellValues.String);
        }

        /// <summary>
        /// Deletes the text from cell.
        /// </summary>
        /// <param name="colName">Name of the col.</param>
        /// <param name="rowIndex">Index of the row.</param>
        public void DeleteTextFromCell(string colName, uint rowIndex)
        {
            // Open the document for editing.
            if (CurrentSheet == null) return;
            // Get the cell at the specified column and row.
            Cell cell = GetSpreadsheetCell(CurrentSheet.Worksheet, colName, rowIndex);
            if (cell == null) return;

            cell.Remove();
            CurrentSheet.Worksheet.Save();

            if (cell.DataType == null) return;
            // Clean up the shared string table.
            if (cell.DataType.Value == CellValues.SharedString)
            {
                int shareStringId = int.Parse(cell.CellValue.Text);
                RemoveSharedStringItem(shareStringId);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            SaveDocument();
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Reads all rows in document.
        /// </summary>
        /// <param name="columnCount">Number of columns to read.</param>
        /// <param name="result">The result data.</param>
        /// <returns>True is succes</returns>
        private bool ExecuteReadDocument(int columnCount, ref DataResult dresult)
        {
            if (CurrentSheet == null) throw new DuradosXmlException("Sheet not found!");
            var stringTableList = GetSharedStringPart().SharedStringTable.ChildElements.ToList();
            var lastRow = CurrentSheet.Worksheet.Descendants<Row>().LastOrDefault();
            if (lastRow == null) throw new DuradosXmlException("Sheet is empty!");
            var allRows = CurrentSheet.Worksheet.Descendants<Row>().ToList();

            int totalColums = 10000;

            for (var rowIndex = 1; rowIndex <= lastRow.RowIndex; rowIndex++)
            {
                var cellList = new List<string>();
                var row = (from rows in allRows
                                       where rows.RowIndex.Value == rowIndex
                                       select rows).FirstOrDefault();

                if (row == null)
                    throw new EmptyRowException(rowIndex);

                
                var cells = (from c in
                                      row.Descendants<Cell>()
                                  where c.CellValue != null //&&
                                  //c.DataType != null &&
                                  //c.DataType.HasValue
                                  // &&c.DataType.Value == CellValues.SharedString
                                  select c);

                if (cells == null)
                    throw new EmptyRowException(rowIndex);


                var cellValues = cells.ToList();
                
                //var cellValues = (from c in
                //                      (from rows in allRows
                //                       where rows.RowIndex.Value == rowIndex
                //                       select rows).FirstOrDefault().Descendants<Cell>()
                //                  where c.CellValue != null //&&
                //                        //c.DataType != null &&
                //                        //c.DataType.HasValue
                //                        // &&c.DataType.Value == CellValues.SharedString
                //                  select c).ToList();

                for (var cellIndex = 0; cellIndex < cellValues.Count || cellIndex < totalColums; cellIndex++)
                {
                    var colName = GetColumnName(cellIndex);
                    var value = "";
                    
                    var cell = (from c in cellValues
                                where c.CellReference.Value.Equals(colName + rowIndex, StringComparison.CurrentCultureIgnoreCase)
                                select c).FirstOrDefault();

                    if (cell != null)
                    {
                        value = cell.InnerText;

                        if (cell.DataType != null && cell.DataType.HasValue && cell.DataType.Value == CellValues.SharedString)
                        {
                            int sharedStrIndex;
                            
                            if (int.TryParse(value, out sharedStrIndex))
                            {
                                if (sharedStrIndex < stringTableList.Count)
                                    value = stringTableList[sharedStrIndex].InnerText;
                            }

                        }
                    }

                    if (rowIndex == 1 && value == "")
                    {
                        totalColums = cellList.Count;
                        break;
                    }
                    cellList.Add(value);
                }
                dresult.AddRow(cellList.ToArray());    
            }
            return true;
        }

        /// <summary>
        /// Gets the SharedStringPart table object.
        /// </summary>
        /// <returns></returns>
        private SharedStringTablePart GetSharedStringPart()
        {
            return CurrentDocument.WorkbookPart.SharedStringTablePart;
        }

        /// <summary>
        /// Locates the cell.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="rowindex">The rowindex.</param>
        /// <returns></returns>
        private Cell LocateCell(string column, int rowindex)
        {
            if (CurrentSheet == null) throw new Exception("No sheet selected");
            Cell cell = GetSpreadsheetCell(CurrentSheet.Worksheet, column, (uint)rowindex) ??
                        InsertCellInWorksheet(column, (uint)rowindex, CurrentSheet);
            return cell;
        }

        /// <summary>
        /// Gets the spreadsheet cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        private static Cell GetSpreadsheetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Elements<Row>().Where(r => r.RowIndex == rowIndex);
            if (rows.Count() == 0) return null;
            
            IEnumerable<Cell> cells = rows.First().Elements<Cell>().Where(c => string.Compare(c.CellReference.Value, columnName + rowIndex, true) == 0);
            if (cells.Count() == 0) return null;
            
            return cells.First();
        }

        /// <summary>
        /// Inserts the cell in worksheet.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="worksheetPart">The worksheet part.</param>
        /// <returns></returns>
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
            Cell refCell = row.Elements<Cell>().FirstOrDefault(cell => string.Compare(cell.CellReference.Value, cellReference, true) > 0);

            var newCell = new Cell { CellReference = cellReference };
            row.InsertBefore(newCell, refCell);

            worksheet.Save();
            return newCell;
        }

        #endregion

        #region Private commands

        /// <summary>
        /// Removes the shared string item.
        /// </summary>
        /// <param name="shareStringId">The share string id.</param>
        private void RemoveSharedStringItem(int shareStringId)
        {
            var remove = true;
            foreach (var worksheet in
                CurrentDocument.WorkbookPart.GetPartsOfType<WorksheetPart>().Select(part => part.Worksheet))
            {
                remove = worksheet.GetFirstChild<SheetData>().Descendants<Cell>().All(cell => cell.DataType == null || cell.DataType.Value != CellValues.SharedString || cell.CellValue.Text != shareStringId.ToString());
                if (!remove) break;
            }

            // Other cells in the document do not reference the item. Remove the item.
            if (!remove) return;
            var shareStringTablePart = GetSharedStringPart();
            if (shareStringTablePart == null) return;
            var item = shareStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(shareStringId);
            if (item == null) return;
            item.Remove();
            // Refresh all the shared string references.
            foreach (var worksheet in
                CurrentDocument.WorkbookPart.GetPartsOfType<WorksheetPart>().Select(part => part.Worksheet))
            {
                foreach (var cell in worksheet.GetFirstChild<SheetData>().Descendants<Cell>())
                {
                    if (cell.DataType == null || cell.DataType.Value != CellValues.SharedString) continue;
                    var itemIndex = int.Parse(cell.CellValue.Text);
                    if (itemIndex > shareStringId) cell.CellValue.Text = (itemIndex - 1).ToString();                    
                }
                worksheet.Save();
            }
            GetSharedStringPart().SharedStringTable.Save();
        }
        #endregion
    }

    #region Shared class

    public class DataResult
    {
        public enum DataResultType { DataTableType, ListType }

        private readonly DataResultType _dataResultType;
        private readonly DataTable _table;
        private readonly List<string[]> _list;
        public DataResult(DataResultType resultType)
        {
            _dataResultType = resultType;
            switch (resultType)
            {
                case DataResultType.DataTableType:
                    _table = new DataTable();
                    break;
                case DataResultType.ListType:
                    _list = new List<string[]>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("DataResult.DataResultType does not exist");
            }
        }
        public void AddRow(string[] rowData)
        {
            switch (_dataResultType)
            {
                case DataResultType.DataTableType:
                    while (_table.Columns.Count < rowData.Length)
                    {
                        _table.Columns.Add(ExcelHandler.GetColumnName(_table.Columns.Count));
                    }
                    _table.Rows.Add(rowData);
                    break;
                case DataResultType.ListType:
                    _list.Add(rowData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("DataResult.DataResultType does not exist");
            }
        }
        public DataTable GetDataTable()
        {
            return _table;
        }
        public List<string[]> GetList()
        {
            return _list;
        }
    }

    public class EmptyRowException : DuradosXmlException
    {
        public EmptyRowException(int rowIndex)
            : base("The row #" + rowIndex + " is empty. Please delete the empty rows and try again.")
        {
        }
    }

    #endregion
}
