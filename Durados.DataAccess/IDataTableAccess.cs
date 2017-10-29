using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess
{
    public interface IDataTableAccess
    {
        bool IsRowAlreadyExistsByDisplayName(View view, string value);

        bool IsRowAlreadyExistsByDisplayName(View view, Dictionary<string, object> values);

        bool? IsRowAlreadyExistsByScalars(View view, Dictionary<string, object> values);

        string GetScalar(ColumnField columnField, string pk);
        
        string GetScalar(ParentField parentField, string pk);

        string GetPKValueByDisplayValue(View view, string displayValue, out GetPKValueByDisplayValueStatus status);

        string GetPKValueByDisplayValue(ParentField field, string displayValue, Dictionary<ParentField, string> dependencyDisplayValues, out GetPKValueByDisplayValueStatus status);
        
        int? Create(View view, Dictionary<string, object> values, bool handleOrder, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback, IDbCommand command, out bool roolback);

        void Delete(View view, string pk, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback, Dictionary<string, object> values = null);

        void Edit(View view, Dictionary<string, object> values, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseEventHandler, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback);

        DataView FillPage(View view, int page, int pageSize, Dictionary<string, object> values, bool? search, bool? useLike, Dictionary<string, SortDirection> sortColumns, out int rowCount, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback);

        DataView FillPage(View view, int page, int pageSize, Filter filter, bool? search, bool? useLike, Dictionary<string, SortDirection> sortColumns, out int rowCount, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback);

        DataRow GetDataRow(View view, string pk);
        DataRow GetDataRow2(View view, string pk, IDbCommand command);

        DataRow GetDataRow(View view, string pk, DataSet dataset);

        DataRow GetDataRow(View view, Field field, string key, bool usePermanentFilter);

        DataRow GetDataRow(View view, string pk, DataSet dataset, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback, bool usePermanentFilter = false);

        DataTable GetDataRows(View view, string[] pks);

        string GetDisplayValue(ParentField parentField, string pk);

        string GetDisplayValue(ChildrenField childrenField, string pk);

        Filter GetFilter(View view, Dictionary<string, object> values, LogicCondition logicCondition, bool insideTextSearch, bool? useLike, Field currentField);

        DataRow GetNewRow(View view, Dictionary<string, object> values, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback);
        string CreateNewRow(View view, Dictionary<string, object> values, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback);
       
        //changed by br 3
        Dictionary<string, string> GetSelectOptions(ParentField parentField, bool forFilter, bool useUniqueName, int? top, Filter filter);

        //changed by br 3
        Dictionary<string, string> GetSelectOptions(ParentField parentField, string fk, int? top, Filter filter, bool forFilter);

        Dictionary<string, string> GetSelectOptions(ParentField parentField, string match, bool startWith, int? top, Filter filter);

        Dictionary<string, string> GetSelectOptions(Field field, string match, bool startWith, int? top, Filter filter);

        Dictionary<string, Dictionary<string, string>> GetSelectOptionsWithGroups(ParentField parentField);

        void Switch(View view, string o_pk, string d_pk, int userID);

        /// <summary>
        /// Change ordinal of records between o_pk and d_pk- in order to move o_pk to d_pk place
        /// </summary>
        /// <param name="view"></param>
        /// <param name="o_pk"></param>
        /// <param name="d_pk"></param>
        void ChangeOrdinal(View view, string o_pk, string d_pk, int userID);


        int RowCount(View view);

        Dictionary<string, Dictionary<string, string>> GetChildren(DataView dataView, ChildrenField field);

        string[] GetKeys(View view, ParentField parentField, string fieldName, string fk);

        List<string> GetCheckListKeys(ChildrenField childrenField, string fk);

        string GetCalculatedFieldStatement(Field field, string fieldName);
        string GetCalculatedFieldStatement(Field field, string fieldName, View view);

        void ExecuteNonQuery(string connectionString, string sql, SqlProduct sqlProduct);

        DataTable ExecuteTable(string connectionString, string sql, Dictionary<string, object> parameters, CommandType commandType);
        string ExecuteScalar(string connectionString, string sql, Dictionary<string, object> parameters);

        SqlSchema GetNewSqlSchema();

        IDbCommand GetNewCommand(View view);

        string GetFirstPK(View view);

        void LoadForeignKeys(string connectionString, SqlProduct sqlProduct, Dictionary<string, Dictionary<string, string>> cache);

        SqlProduct GetSqlProduct();
    }
}
