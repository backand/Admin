public interface ISqlTextBuilder
{
    string GetQueryForLike();

    string GetQueryTemplateForLike(bool startWith, string match);

    string EscapeDbObject(string Name);

    string EscapeDbObjectStart { get; }

    string EscapeDbObjectEnd { get; }

    string DbTableColumnSeperator { get; }

    string DbEquals { get; }

    string DbAnd { get; }

    string DbOr { get; }

    string AllColumns { get; }

    string WithNolock { get; }

    string GetRowNumber(string orderByColumn);

    string GetRowNumber(string orderByTable, string orderByColumn);

    string GetRowNumber(string orderByTable, string orderByColumn, string sortOrder);

    string GetRowNumberNotEscaped(string orderByColumn, string sortOrder);

    string GetPageOrder(string orderByColumn);

    string GetPage(int page, int rowsCount);

    string GetLastInsertedRow(string tableName);
    //string GetLastInsertedRow2(string tableName);
    string GetLastInsertedRow(Durados.View view);

    string GetDateDiffDays(string start, string end);

    string GetDateAddDays(string days, string date);

    string GetDateOnly(string date);

    string Top(string sql, int top);

    string GetAlterColumn(string columnName);


    string NLS { get; }

    string DbAs { get; }

    string DbParameterPrefix { get; }
    string DbEndOfStatement { get; }

    string GetLastInsertedRow2(Durados.View view);

    string GetConvertDateToVarcharStatement(string escapedColumnName, string dateFormat);

    string mmddyyyy { get; }

    string monddyyyy { get; }

    string InsertWithoutColumns();

    string FromDual();

    string GetPointFieldStatement(string tableName, string columnName);

    string GetEncryptedColumnsStatement(string encryptedName, string databaseNames);

    string GetCloseCertificatesStatement();

    string GetOpenCertificatesStatement();

    string GetDbEncrypytedColumnSql(string p, string columnName);
}