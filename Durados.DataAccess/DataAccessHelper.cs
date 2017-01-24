using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Durados;

namespace Durados.DataAccess
{
    public static class DataAccessHelper
    {
        public static void AssignToParent(this View view, string newParent, string[] pks, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseEventHandler, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            if (!view.AllowEdit)
                throw new AccessViolationException();

            ICopyPaste assignCommand = GetCopyPaste(view);

            try
            {
                foreach (string pk in pks)
                {
                    Dictionary<string, object> values = new Dictionary<string, object>();

                    values.Add(view.TreeRelatedFieldName, newParent);

                    assignCommand.Edit(view, values, pk, beforeEditCallback, beforeEditInDatabaseEventHandler, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);
                }

            }
            catch (Exception exeption)
            {

                assignCommand.PasteCommit(true);
                throw new DuradosException(exeption.Message);

            }
            finally
            {
                try
                {
                    assignCommand.CloseConnections();
                }
                catch { }
            }

            assignCommand.PasteCommit(false);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                try
                {
                    fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                }
                catch { }
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static string Create(this View view, Dictionary<string, object> values)
        {
            return (GetDataTableAccess(view)).CreateNewRow(view, values, null, null, null, null, null);
        }

        public static DataRow Create(this View view, Dictionary<string, object> values, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            if (!view.AllowCreate && !view.AllowDuplicate)
                throw new AccessViolationException();

            if (!view.SystemView)
            {
                string[] fields = ValidateTextLength(view, values);
                if (fields.Length > 0)
                {
                    throw new ExceedLengthException(fields);
                }
            }
            return GetDataTableAccess(view).GetNewRow(view, values, insertAbovePK, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
        }

        public static int? Create(this View view, Dictionary<string, object> values, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback, IDbCommand command, out bool roolback)
        {
            if (!view.AllowCreate && !view.AllowDuplicate)
                throw new AccessViolationException();
            return GetDataTableAccess(view).Create(view, values, false, insertAbovePK, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback, (System.Data.SqlClient.SqlCommand)command, out roolback);
        }

        public static void Delete(this View view, string pk, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback)
        {
            if (!view.AllowDelete)
                throw new AccessViolationException();
            GetDataTableAccess(view).Delete(view, pk, beforeDeleteCallback, afterDeleteBeforeCommitCallback, afterDeleteAfterCommitCallback);
        }

        public static void Delete(this View view, string fk, string fkField, string stam, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback)
        {
            if (!view.AllowDelete)
                throw new AccessViolationException();
            ((SqlAccess)GetDataTableAccess(view)).Delete(view, fk, fkField, beforeDeleteCallback, afterDeleteBeforeCommitCallback, afterDeleteAfterCommitCallback);
        }

        public static void Edit(this View view, Dictionary<string, object> values, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseEventHandler, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            if (!view.AllowEdit)
                throw new AccessViolationException();
            GetDataTableAccess(view).Edit(view, values, pk, beforeEditCallback, beforeEditInDatabaseEventHandler, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);
        }

        public static DataView FillPage(this View view, int page, int pageSize, Dictionary<string, object> values, bool? search, Dictionary<string, SortDirection> sortColumns, out int rowCount, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            return GetDataTableAccess(view).FillPage(view, page, pageSize, values, search, null, sortColumns, out rowCount, beforeSelectCallback, afterSelectCallback);
        }

        public static DataView FillPage(this View view, int page, int pageSize, Dictionary<string, object> values, bool? search, bool? useLike, Dictionary<string, SortDirection> sortColumns, out int rowCount, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            return GetDataTableAccess(view).FillPage(view, page, pageSize, values, search, useLike, sortColumns, out rowCount, beforeSelectCallback, afterSelectCallback);
        }
        public static DataView FillPage(this View view, int page, int pageSize, Filter filter, bool? search, bool? useLike, Dictionary<string, SortDirection> sortColumns, out int rowCount, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            return GetDataTableAccess(view).FillPage(view, page, pageSize, filter, search, useLike, sortColumns, out rowCount, beforeSelectCallback, afterSelectCallback);
        }
        public static List<string> GetCheckListKeys(this ChildrenField childrenField, DataRow row)
        {
            string fk = childrenField.View.GetPkValue(row, true);
            return GetCheckListKeys(childrenField, fk);
        }

        public static List<string> GetCheckListKeys(this ChildrenField childrenField, string fk)
        {
            return (new SqlAccess()).GetCheckListKeys(childrenField, fk);

        }

        public static ICopyPaste GetCopyPaste(this View view)
        {
            if (view is Durados.Config.IConfigView)
                return new CopyPasteConfig(view);
            else
                return new CopyPaste(view);
        }

        public static DataRow GetDataRow(this View view, string pk)
        {
            return GetDataTableAccess(view).GetDataRow(view, pk);
        }

        public static DataRow GetDataRow2(this View view, string pk, IDbCommand command)
        {
            return GetDataTableAccess(view).GetDataRow2(view, pk, command);
        }

        public static DataRow GetDataRow(this View view, string pk, DataSet dataset)
        {
            return GetDataTableAccess(view).GetDataRow(view, pk, dataset);
        }

        public static DataRow GetDataRow(this View view, Field field, string key)
        {
            return GetDataRow(view, field, key, true);
        }

        public static DataRow GetDataRow(this View view, Field field, string key, bool usePermanentFilter)
        {
            return GetDataTableAccess(view).GetDataRow(view, field, key, usePermanentFilter);
        }

        public static DataRow GetDataRow(this View view, string pk, DataSet dataset, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback, bool usePermanentFilter = false)
        {
            return GetDataTableAccess(view).GetDataRow(view, pk, dataset, beforeSelectCallback, afterSelectCallback, usePermanentFilter);
        }

        public static DataTable GetDataRows(this View view, string[] pks)
        {
            return GetDataTableAccess(view).GetDataRows(view, pks);
        }

        public static DataView GetDataView(this ChildrenField childrenField, DataRow dataRow)
        {
            return GetDataView(childrenField, childrenField.View.GetPkValue(dataRow));
        }

        public static DataView GetDataView(this ChildrenField childrenField, string key)
        {
            ParentField parentField = childrenField.GetEquivalentParentField();
            View view = parentField.View;

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(parentField.Name, key);

            int rowCount = 0;

            Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
            if (!string.IsNullOrEmpty(view.OrdinalColumnName) && view.DataTable.Columns.Contains(view.OrdinalColumnName))
            {
                sortColumns.Add(view.OrdinalColumnName, SortDirection.Asc);
            }

            DataView dataView = view.FillPage(1, 1000, values, false, sortColumns, out rowCount, null, null);

            return dataView;
        }

        public static string GetDisplayValue(this ParentField parentField, string pk)
        {
            return GetDataTableAccess(parentField.View).GetDisplayValue(parentField, pk);
        }

        public static string GetDisplayValue(this ChildrenField childrenField, string pk)
        {
            return GetDataTableAccess(childrenField.View).GetDisplayValue(childrenField, pk);
        }


        public static string GetDisplayValue(this View view, Enum fieldEnum, DataRow dataRow)
        {
            return GetDisplayValue(view, fieldEnum.ToString(), dataRow);
        }

        public static string GetDisplayValue(this View view, string fieldName, DataRow dataRow)
        {
            Field field = view.Fields[fieldName];

            return field.ConvertToString(dataRow);

        }

        public static string GetDisplayValue(this View view, string pk, Dictionary<string, object> values, DataRow prevRow)
        {
            if (!values.ContainsKey(view.DisplayField.Name))
            {
                return view.GetDisplayValue(prevRow);
            }
            else if (view.DisplayField.FieldType == FieldType.Parent)
            {
                if (view.DisplayField.GetValue(prevRow) == values[view.DisplayField.Name].ToString())
                {
                    return view.GetDisplayValue(prevRow);
                }
                else
                {
                    DataRow dataRow = GetDataRow(view, pk);
                    return view.GetDisplayValue(dataRow);
                }
            }
            else
            {
                return view.GetDisplayValue(values);
            }


        }

        //public static string GetDisplayValue(this View view, DataRow dataRow)
        //{

        //    string displayValue = string.Empty;

        //    if (dataRow != null)
        //    {
        //        Field displayField = view.DisplayField;
        //        while (displayField.FieldType == FieldType.Parent)
        //        {
        //            DataRow originalRow = dataRow;
        //            dataRow = dataRow.GetParentRow(((ParentField)displayField).DataRelation.RelationName);
        //            if (dataRow == null)
        //            {
        //                dataRow = ((ParentField)displayField).ParentView.GetDataRow(((ParentField)displayField).GetValue(originalRow));
        //                if (dataRow == null)
        //                    return string.Empty;
        //            }
        //            displayField = ((ParentField)displayField).ParentView.DisplayField;
        //        }

        //        displayValue = dataRow[displayField.View.DisplayColumn].ToString();

        //    }

        //    return displayValue;
        //}

        public static IDbCommand GetNewCommand(this View view)
        {
            return GetDataTableAccess(view).GetNewCommand(view);
        }

        public static string GetIndexFieldsDisplayNames(this View view, string indexName)
        {
            return (new Durados.DataAccess.AutoGeneration.Dynamic.Mapper()).GetIndexFieldsDisplayNames(indexName, view);
        }

        public static string[] GetKeys(this View view, ParentField parentField, string fieldName, string fk)
        {
            return GetDataTableAccess(view).GetKeys(view, parentField, fieldName, fk);
        }

        public static string GetPKValueByDisplayValue(this View view, string displayValue, out GetPKValueByDisplayValueStatus status)
        {
            return GetDataTableAccess(view).GetPKValueByDisplayValue(view, displayValue, out status);
        }

        public static string GetPKValueByDisplayValue(this ParentField field, string displayValue, Dictionary<ParentField, string> dependencyDisplayValues, out GetPKValueByDisplayValueStatus status)
        {
            return GetDataTableAccess(field.View).GetPKValueByDisplayValue(field, displayValue, dependencyDisplayValues, out status);
        }

        public static Dictionary<string, string> GetSelectOptions(this Field field, string match, bool startWith, int top, Filter filter)
        {
            if (field.View is Durados.Config.IConfigView)
                return (new ConfigAccess()).GetSelectOptions((ColumnField)field, match, startWith, top);
            else
                return GetDataTableAccess(field.View).GetSelectOptions(field, match, startWith, top, null);
        }

        //by br 3
        //public static Dictionary<string, string> GetSelectOptions(this ParentField parentField)
        //{
        //    return GetSelectOptions(parentField, false, false, null, null);
        //}
        //changed by br 3
        public static Dictionary<string, string> GetSelectOptions(this ParentField parentField, View view, bool forFilter, bool useUniqueName, int? topCount, Dictionary<string, object> filterValues)
        {
            Filter filter = GetDataTableAccess(parentField.View).GetFilter(parentField.View, filterValues, LogicCondition.And, false, false, parentField);

            return GetDataTableAccess(parentField.View).GetSelectOptions(parentField, forFilter, useUniqueName, null, filter);
        }

        //changed by br 3
        public static Dictionary<string, string> GetSelectOptions(this ParentField parentField, View view, string fk, int? top, Dictionary<string, object> filterValues, bool forFilter)
        {
            Filter filter = GetDataTableAccess(parentField.View).GetFilter(parentField.View, filterValues, LogicCondition.And, false, false, parentField);

            return GetDataTableAccess(parentField.View).GetSelectOptions(parentField, fk, top, filter, forFilter);
        }

        public static Dictionary<string, Dictionary<string, string>> GetSelectOptionsWithGroups(this ParentField parentField)
        {
            return GetDataTableAccess(parentField.View).GetSelectOptionsWithGroups(parentField);
        }

        public static string GetValue(this Field field, string pk)
        {
            View view = field.View;
            if (view is Durados.Config.IConfigView)
                throw new NotImplementedException();


            if (field.FieldType == FieldType.Column)
                return GetDataTableAccess(view).GetScalar((ColumnField)field, pk);
            else if (field.FieldType == FieldType.Column)
                return GetDataTableAccess(view).GetScalar((ParentField)field, pk);
            else
                throw new NotImplementedException();

        }

        public static object GetValue(this View view, Enum fieldEnum, DataRow dataRow)
        {
            return GetValue(view, fieldEnum.ToString(), dataRow);
        }

        public static object GetValue(this View view, string fieldName, DataRow dataRow)
        {
            Field field = view.Fields[fieldName];


            return field.GetValue(dataRow);

        }

        public static bool IsRowAlreadyExistsByDisplayName(this View view, string value)
        {
            return GetDataTableAccess(view).IsRowAlreadyExistsByDisplayName(view, value);
        }

        public static bool IsRowAlreadyExistsByDisplayName(this View view, Dictionary<string, object> values)
        {
            return GetDataTableAccess(view).IsRowAlreadyExistsByDisplayName(view, values);
        }

        public static bool? IsRowAlreadyExistsByScalars(this View view, Dictionary<string, object> values)
        {
            return GetDataTableAccess(view).IsRowAlreadyExistsByScalars(view, values);
        }

        //Replace display names with field names or oposite + security validation
        public static string ReplaceFieldDisplayNames(string statement, bool DisplayToName, View view)
        {
            if (statement == null) return null;

            string pattern = @"\[(.*?)\]";

            var matches = Regex.Matches(statement, pattern, RegexOptions.IgnoreCase);

            foreach (Match m in matches)
            {
                if (!m.Success) continue;

                if (DisplayToName)
                {
                    //security validation
                    if (Regex.IsMatch(statement, "\b|drop|exec|delete|truncate|insert|update|alter|create|\b", RegexOptions.IgnoreCase | RegexOptions.Compiled))
                    {
                        throw new DuradosException("There are disallowed words in formula");
                    }

                    string fieldDisplayName = m.Groups[1].ToString();

                    foreach (Field f in view.Fields.Values.Where(x => x.DisplayName == fieldDisplayName))
                    {
                        statement = statement.Replace("[" + fieldDisplayName + "]", "[" + f.DatabaseNames + "]");
                        break;
                    }
                }
                else //Names To Display
                {
                    string fieldName = m.Groups[1].ToString();

                    foreach (Field f in view.Fields.Values.Where(x => x.Name == fieldName))
                    {
                        statement = statement.Replace("[" + f.DatabaseNames + "]", "[" + f.DisplayName + "]");
                        break;
                    }
                }
            }

            return statement;
        }

        /// <summary>
        /// Change ordinal of records between o_pk and d_pk- in order to move o_pk to d_pk place
        /// </summary>
        /// <param name="view"></param>
        /// <param name="o_pk"></param>
        /// <param name="d_pk"></param>
        public static void ChangeOrdinal(this View view, string o_pk, string d_pk, int userID)
        {
            if (!view.AllowEdit)
                throw new AccessViolationException();

            GetDataTableAccess(view).ChangeOrdinal(view, o_pk, d_pk, userID);
        }


        public static void Switch(this View view, string o_pk, string d_pk, int userID)
        {
            if (!view.AllowEdit)
                throw new AccessViolationException();
            GetDataTableAccess(view).Switch(view, o_pk, d_pk, userID);
        }

        public static IDataTableAccess GetDataTableAccess(Durados.View view)
        {
            if (view is Durados.Config.IConfigView)
                return new ConfigAccess();
            else
                if (OracleAccess.IsOracleConnectionString(view.ConnectionString))
                    return new OracleAccess();
                if (PostgreAccess.IsPostgreConnectionString(view.ConnectionString))
                    return new PostgreAccess();
                else if (MySqlAccess.IsMySqlConnectionString(view.ConnectionString))
                    return new MySqlAccess();
                else
                    return new SqlAccess();
        }

        public static IDataTableAccess GetDataTableAccess(string connectionString)
        {
             if (OracleAccess.IsOracleConnectionString(connectionString))
                return new OracleAccess();
            else if (PostgreAccess.IsPostgreConnectionString(connectionString))
                return new PostgreAccess();
            else if (MySqlAccess.IsMySqlConnectionString(connectionString))
                return new MySqlAccess();
            else
                return new SqlAccess();
        }

        public static int GetRowCount(this View view)
        {
            return GetDataTableAccess(view).RowCount(view);
        }


        private static string[] ValidateTextLength(View view, Dictionary<string, object> values)
        {
            List<string> names = new List<string>();

            foreach (string name in values.Keys)
            {
                if (view.Fields.ContainsKey(name))
                {
                    Field field = view.Fields[name];
                    if (field.FieldType == FieldType.Column)
                    {
                        ColumnField columnField = (ColumnField)field;

                        if (columnField.DataColumn.DataType == typeof(string) && values[name] != null && columnField.Max > 0 && values[name].ToString().Length > columnField.Max)
                        {
                            names.Add(name);
                        }
                    }
                }
            }

            return names.ToArray();
        }

        public static ISqlTextBuilder GetSqlTextBuilder(this Durados.Database database)
        {
            switch (database.SqlProduct)
            {
                case SqlProduct.MySql:
                    return new Durados.DataAccess.MySqlTextBuilder();
                case SqlProduct.Postgre:
                    return new Durados.DataAccess.PostgreTextBuilder();
                case SqlProduct.Oracle:
                    return new Durados.DataAccess.OracleTextBuilder();

                default:
                    return new SqlTextBuilder();
            }
        }

    }

    public class ExceedLengthException : DuradosException
    {
		
        public ExceedLengthException(string[] fieldsNames)
            : base(fieldsNames.Length == 1 ? fieldsNames[0] + " exceeds its length" : (fieldsNames.Delimited() + " exceed their length"))
        {
        }

    }

}
