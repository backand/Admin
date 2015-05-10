using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration
{
    public class History : Generator
    {

        public History(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }
        //protected override void BuildSchema(string connectionString)
        //{
        //    if (SchemaExists(connectionString))
        //        return;

        //    base.BuildSchema(connectionString);
        //    SqlConnection conn = new SqlConnection(connectionString);
        //    conn.Open();
        //    try
        //    {
        //        SqlCommand command = new SqlCommand();

        //        command.Connection = conn;
        //        //command.CommandType = System.Data.CommandType.StoredProcedure;
        //        command.CommandText = "CREATE VIEW [dbo].[durados_v_ChangeHistory] AS SELECT        ROW_NUMBER() OVER (ORDER BY dbo.durados_ChangeHistory.Id) AS AutoId, dbo.durados_ChangeHistory.ViewName, dbo.durados_ChangeHistory.PK,dbo.durados_ChangeHistory.ActionId, dbo.durados_ChangeHistory.UpdateDate, dbo.durados_ChangeHistory.UpdateUserId, dbo.durados_ChangeHistoryField.FieldName, dbo.durados_ChangeHistoryField.ColumnNames, dbo.durados_ChangeHistoryField.OldValue, dbo.durados_ChangeHistoryField.NewValue, dbo.durados_ChangeHistoryField.Id, dbo.durados_ChangeHistoryField.ChangeHistoryId, dbo.durados_ChangeHistory.Comments FROM dbo.durados_ChangeHistory WITH (NOLOCK) LEFT OUTER JOIN dbo.durados_ChangeHistoryField WITH (NOLOCK) ON dbo.durados_ChangeHistory.id = dbo.durados_ChangeHistoryField.ChangeHistoryId";
        //        command.ExecuteNonQuery();
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        protected override string RootObjectName
        {
            get { return "durados_ChangeHistory"; }
        }


    }

    public class MySqlHistory : History
    {
        public MySqlHistory(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }
        protected override SqlSchema GetNewSqlSchema()
        {
            return   new MySqlSchema();
        }


    }
}
