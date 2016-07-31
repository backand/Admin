using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Durados.DataAccess.AutoGeneration.Dynamic.Schema
{
    public class Table
    {
        public Table()
        {
            Columns = new HashSet<string>();
            myForeignKeys = new ForeignKeys();
            foreignKeysToMe = new ForeignKeys();
        }

        public string Name { get; set; }
        public virtual ObjectType ObjectType
        {
            get
            {
                return ObjectType.Table;
            }
        }
        public HashSet<string> Columns { get; private set; }
        public ForeignKeys myForeignKeys { get; private set; }
        public ForeignKeys foreignKeysToMe { get; private set; }

        public void Load(string connectionString)
        {
            SqlSchema schema = new SqlSchema();

            string sql = schema.GetColumnsSelectStatement(Name);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader.GetString(reader.GetOrdinal("Column_Name"));

                            if (!Columns.Contains(columnName))
                                Columns.Add(columnName);
                        }
                        reader.Close();
                    }
                }

                sql = schema.GetForeignKeyConstraints(Name);
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string foreignKeyName = reader.GetString(reader.GetOrdinal("ForeignKey"));
                            string editableTableName = reader.GetString(reader.GetOrdinal("TableName"));
                            string columnName = reader.GetString(reader.GetOrdinal("ColumnName"));
                            string referenceTableName = reader.GetString(reader.GetOrdinal("ReferenceTableName"));
                            string referenceColumnName = reader.GetString(reader.GetOrdinal("ReferenceColumnName"));


                            if (Columns.Contains(columnName))
                            {
                                ForeignKey foreignKey = null;
                                if (myForeignKeys.ContainsByForeignKeyName(foreignKeyName))
                                {
                                    foreignKey = myForeignKeys.GetByForeignKeyName(foreignKeyName);
                                }
                                else
                                {
                                    foreignKey = new ForeignKey() { Name = foreignKeyName, TableName = Name, ReferenceTableName = referenceTableName };
                                    myForeignKeys.Add(foreignKey);
                                }
                                foreignKey.ColumnsNames.Add(columnName);
                                foreignKey.ReferenceColumnsNames.Add(referenceColumnName);
                            }
                        }
                        reader.Close();
                    }
                }

                sql = schema.GetForeignKeyConstraintsToMe(Name);
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string foreignKeyName = reader.GetString(reader.GetOrdinal("ForeignKey"));
                            string editableTableName = reader.GetString(reader.GetOrdinal("TableName"));
                            string columnName = reader.GetString(reader.GetOrdinal("ColumnName"));
                            string referenceTableName = reader.GetString(reader.GetOrdinal("ReferenceTableName"));
                            string referenceColumnName = reader.GetString(reader.GetOrdinal("ReferenceColumnName"));


                            if (Columns.Contains(columnName))
                            {
                                ForeignKey foreignKey = null;
                                if (myForeignKeys.ContainsByForeignKeyName(foreignKeyName))
                                {
                                    foreignKey = myForeignKeys.GetByForeignKeyName(foreignKeyName);
                                }
                                else
                                {
                                    foreignKey = new ForeignKey() { Name = foreignKeyName, TableName = referenceTableName, ReferenceTableName =  Name};
                                    myForeignKeys.Add(foreignKey);
                                }
                                foreignKey.ColumnsNames.Add(referenceColumnName);
                                foreignKey.ReferenceColumnsNames.Add(columnName);
                            }
                        }
                        reader.Close();
                    }
                }

                connection.Close();

            }

        }



        
    }

    public enum ObjectType
    {
        Table,
        View
    }
}
