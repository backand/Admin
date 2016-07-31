using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Web.Mvc;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.UI.Json
{   
    public class Tree : ITree
    {
        protected Durados.Web.Mvc.View view;

        public Tree(Durados.Web.Mvc.View view)
        {
            this.view=view;           
        }

        public IList<TreeNode> GetChildren(string parentKey, string[] selectedKeys)
        {
            return new List<TreeNode>();
        }

        private DataView GetNodesTable(string parentKey)
        {
            if (!view.Fields.ContainsKey(view.TreeRelatedFieldName))
                throw new DuradosException("The View '" + view.DisplayName + "' doeas not contain the Tree Related Field Name '" + view.TreeRelatedFieldName + "'.");

            if (view.Database.IsConfig)
            {

                if (string.IsNullOrEmpty(parentKey))
                {
                    parentKey = "[null]";
                }

                Dictionary<string, object> filter = new Dictionary<string, object>();
                filter.Add(view.TreeRelatedFieldName, parentKey);
                
                int rowCount = 0;
                DataView dataView = view.FillPage(1, 1000000, filter, false, null, out rowCount, null, null);
 
                return dataView;
            }
            else
            {
                return new DataView(GetNodesTableFromDb(parentKey));
            }
        }

        private SqlSchema GetSqlSchema(SqlProduct sqlProduct)
        {
            if (sqlProduct == SqlProduct.MySql)
                return new MySqlSchema();
            else if (sqlProduct == SqlProduct.Postgre)
                return new PostgreSchema();
            else if (sqlProduct == SqlProduct.Oracle)
                return new OracleSchema();
            else
                return new SqlSchema();
        }

        private DataTable GetNodesTableFromDb(string parentKey)
        {
            //string treeFirstLevelSql = "SELECT {0} AS [title], [{1}] AS [key] FROM [{2}] WHERE [{3}] {4}";
            SqlSchema sqlSchema = GetSqlSchema(view.Database.SqlProduct);
            string treeFirstLevelSql = sqlSchema.GetTreeFirstLevelSql();

            if (string.IsNullOrEmpty(parentKey))
            {
                parentKey = "IS NULL";
                //parentKey = "= 0";
            }
            else
            {
                parentKey = "= '" + parentKey + "'";
            }

            if (!string.IsNullOrEmpty(view.PermanentFilter))
            {
                treeFirstLevelSql += " and " + view.GetPermanentFilter() + " ";
            }

            //string query = string.Format(treeFirstLevelSql, view.DisplayField.IsCalculated ? view.DisplayField.Formula : ("[" + view.DisplayField.Name + "]"), view.PrimaryKeyFileds[0].DatabaseNames, view.DataTable.TableName, view.Fields[view.TreeRelatedFieldName].DatabaseNames, parentKey);

            //DataAccess.SqlAccess da = new DataAccess.SqlAccess();
            string query = string.Format(treeFirstLevelSql, view.DisplayField.IsCalculated ? view.DisplayField.Formula : sqlSchema.sqlTextBuilder.EscapeDbObject(view.DisplayField.Name), view.PrimaryKeyFileds[0].DatabaseNames, view.DataTable.TableName, view.Fields[view.TreeRelatedFieldName].DatabaseNames, parentKey);

            DataAccess.IDataTableAccess da = DataAccess.DataAccessHelper.GetDataTableAccess(view);

            DataTable dtNodes = da.ExecuteTable(view.Database.ConnectionString, query, null, CommandType.Text);
            return dtNodes;
        }

        public IList<Node> GetFirstLevelChildren(string parentKey)
        {          
            List<UI.Json.Node> nodes = new List<Durados.Web.Mvc.UI.Json.Node>();

            DataView dtNodes = GetNodesTable(parentKey);

            string title = "title";
            if (view.Database.IsConfig)
            {
                title = view.DisplayColumn;
            }
            string key = "key";
            if (view.Database.IsConfig)
            {
                key = view.PrimaryKeyFileds[0].DatabaseNames;
            }

            foreach(System.Data.DataRowView row in dtNodes)
            {
                UI.Json.Node node = new Durados.Web.Mvc.UI.Json.Node();

                
                
                if (row.Row.IsNull(title))
                {
                    node.data = string.Empty;
                }
                else
                {
                    node.data = row[title].ToString(); //TODO Check nulls
                }

                node.attr = new { id = "node_" + row[key].ToString() }; 
               
                node.state = "closed";

                nodes.Add(node);
            }

            return nodes;            
        }

        //For more then first level in tree
        private string treeAdjacencySql = @"WITH  v (id, parent, level) AS
                                            (
                                            SELECT  id, parent, 1
                                            FROM    {TableName}
                                            WHERE   parent = 0
                                            UNION ALL
                                            SELECT  id, parent, v.level + 1
                                            FROM    v
                                            JOIN    table t
                                            ON      t.parent = v.id
                                            )
                                            SELECT  *
                                            FROM    v"; //WHERE level < 3

        public string AddChild(string parentKey, string name, BeforeCreateEventHandler bfCreate, BeforeCreateInDatabaseEventHandler bfDbCreate, AfterCreateEventHandler afCreate, AfterCreateEventHandler afCreateAndCommit)
        {
            //string sql = "INSERT INTO {0} ([{1}],[{2}]) VALUES ('{3}','{4}')";
            //string query = string.Format(sql, view.DataTable.TableName, view.TreeRelatedFieldName, view.DisplayColumn,parentKey, name);

            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add(view.TreeRelatedFieldName, parentKey);
            values.Add(view.DisplayColumn, name);

            DataAccess.SqlAccess da = new DataAccess.SqlAccess();

            int? id = da.Create(view, values, null, bfCreate, bfDbCreate, afCreate, afCreateAndCommit);

            if (id.HasValue && id.Value > 0)
            {
                return id.Value.ToString();
            }
            else
            {
                return string.Empty; 
            }
           
            
        }

        public void Rename(string key, string name, BeforeEditEventHandler bfEdit, BeforeEditInDatabaseEventHandler bfDbEdit, AfterEditEventHandler afEdit, AfterEditEventHandler afEditAndCommit)
        {
            //string sql = "UPDATE {0} SET [{1}] = '{2}') WHERE [{3}]='{4}'";
            //string query = string.Format(sql,view.DataTable.TableName, view.DisplayColumn, name, view.PrimaryKeyFileds[0].DatabaseNames,key);

            Dictionary<string, object> values = new Dictionary<string, object>();

            //values.Add(view.PrimaryKeyFileds[0].Name, key);
            values.Add(view.DisplayColumn, name);

            DataAccess.SqlAccess da = new DataAccess.SqlAccess();

            da.Edit(view, values, key, bfEdit, bfDbEdit, afEdit, afEditAndCommit);
        }

        public void Move(string key, string newParentKey, BeforeEditEventHandler bfEdit, BeforeEditInDatabaseEventHandler bfDbEdit, AfterEditEventHandler afEdit, AfterEditEventHandler afEditAndCommit)
        {
            //string sql = "UPDATE {0} SET [{1}] = '{2}') WHERE [{3}]='{4}'";
            //string query = string.Format(sql,view.DataTable.TableName, view.TreeRelatedFieldName, newParentKey, view.PrimaryKeyFileds[0].DatabaseNames,key);

            Dictionary<string, object> values = new Dictionary<string, object>();

            //values.Add(view.PrimaryKeyFileds[0].Name, key);
            values.Add(view.TreeRelatedFieldName, newParentKey);

            DataAccess.SqlAccess da = new DataAccess.SqlAccess();


            da.Edit(view, values, key, bfEdit, bfDbEdit, afEdit, afEditAndCommit);
        }

        public void Remove(string key, BeforeDeleteEventHandler bfDelete, AfterDeleteEventHandler afDelete, AfterDeleteEventHandler afDeleteAndCommit)
        {
            //string sql = "DELETE FROM {0} WHERE [{1}]='{2}'";
            //string query = string.Format(sql,view.DataTable.TableName, view.PrimaryKeyFileds[0].DatabaseNames,key);

            DataAccess.SqlAccess da = new DataAccess.SqlAccess();

            da.Delete(view, key, bfDelete, afDelete, afDeleteAndCommit);
        }

    }

    public interface ITree
    {

        IList<TreeNode> GetChildren(string parentKey, string[] selectedKeys);

        IList<Node> GetFirstLevelChildren(string parentKey);

        string AddChild(string parentKey, string name, BeforeCreateEventHandler bfCreate, BeforeCreateInDatabaseEventHandler bfDbCreate, AfterCreateEventHandler afCreate, AfterCreateEventHandler afCreateAndCommit);

        void Move(string key, string newParentKey, BeforeEditEventHandler bfEdit, BeforeEditInDatabaseEventHandler bfDbEdit, AfterEditEventHandler afEdit, AfterEditEventHandler afEditAndCommit);

        void Rename(string key, string name, BeforeEditEventHandler bfEdit, BeforeEditInDatabaseEventHandler bfDbEdit, AfterEditEventHandler afEdit, AfterEditEventHandler afEditAndCommit);

        void Remove(string key, BeforeDeleteEventHandler bfDelete, AfterDeleteEventHandler afDelete, AfterDeleteEventHandler afDeleteAndCommit);
    }

    public interface INode
    {
        string data {get;set;}

        object attr {get;set;}

        string state {get;set;}
    }

    public class Node : INode
    {
        public string data {get;set;}

        public object attr {get;set;}

        public string state {get;set;}
    }

    public class TreeNode : Node
    {
        public List<TreeNode> children{get;set;}
    }
}