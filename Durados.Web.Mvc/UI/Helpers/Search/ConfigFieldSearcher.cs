using Durados.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers.Search
{
    public class ConfigFieldSearcher
    {
        public const int SNIPPET_LENGTH = 100;
        public string Name { get; set; }

        public string ViewName { get; set; }

        public string FieldName { get; set; }
        public EntityType EntityType { get; set; }

        public object Search(ConfigAccess config, string q, int? id, int snippetLength, bool spaceAsWildcard, string highlightTag, int tabChars)
        {
            string whereStatement = GetWhereStatement(q.Replace("'", "''"), id, spaceAsWildcard);

            DataView dataView = config.GetRows(ViewName, whereStatement, Maps.Instance.GetMap().GetConfigDatabase().ConnectionString);

            if (dataView.Count == 0)
                return null;

            return GetResults(dataView, q, snippetLength, highlightTag, tabChars);
        }

        protected virtual object GetResults(DataView dataView, string q, int snippetLength, string highlightTag, int tabChars)
        {
            List<object> list = new List<object>();
            foreach (System.Data.DataRowView row in dataView)
            {
                var result = GetResult(row, q, snippetLength, highlightTag, tabChars);
                list.Add(result);
            }

            return list.ToArray();
        }

        protected virtual object GetResult(System.Data.DataRowView row, string q, int snippetLength, string highlightTag, int tabChars)
        {
            object id = GetId(row);
            if (EntityType == Durados.EntityType.Action)
            {
                return new { id = id, name = GetName(row, q), objectId = GetObjectId((int)id), foundAt = Name, snippets = GetSnippets(row, q, snippetLength, highlightTag, tabChars) };
            }
            else
            {
                return new { id = id, name = GetName(row, q), foundAt = Name, snippets = GetSnippets(row, q, snippetLength, highlightTag, tabChars) };
            }
        }

        private object GetObjectId(int actionId)
        {
            var ruleAndAction = CronHelper.GetRuleAndAction(actionId);

            View view = (View)ruleAndAction["view"];

            return view.ID;
        }

        protected virtual string GetName(System.Data.DataRowView row, string q)
        {
            return row.Row.Table.Columns.Contains("JsonName") && !row.Row.IsNull("JsonName") ? row["JsonName"].ToString() : row["Name"].ToString();
        }

        protected virtual object GetId(System.Data.DataRowView row)
        {
            return row[GetIdFilterFieldName()];
        }

        protected virtual object GetSnippet(System.Data.DataRowView row, string q, int snippetLength, string highlightTag, int tabChars)
        {
            return row[FieldName];
        }

        protected virtual object GetSnippets(System.Data.DataRowView row, string q, int snippetLength, string highlightTag, int tabChars)
        {
            return new Dictionary<string, object>[1] { new Dictionary<string, object>() { { "snippet", GetSnippet(row, q, snippetLength, highlightTag, tabChars) } } };
        }


        private string GetWhereStatement(string q, int? id, bool spaceAsWildcard)
        {
            if (id.HasValue)
            {
                return GetWhereStatementWithId(q, id.Value, spaceAsWildcard);
            }
            else
            {
                return GetWhereStatement(q, spaceAsWildcard);
            }
        }

        protected virtual string GetWhereStatement(string q, bool spaceAsWildcard)
        {
            if (spaceAsWildcard && q.Contains(' '))
            {
                string w = string.Empty;
                foreach (string word in q.Split(' '))
                {
                    w = w + GetWhereStatementOfWord(q) + " or ";
                }
                w = "(" + w.TrimEnd(" or ".ToCharArray()) + ")" + GetExcludeConfig();
                return w;
            }
            else
                return GetWhereStatementOfWord(q) + GetExcludeConfig();
        }

        protected virtual string GetWhereStatementOfWord(string q)
        {
            return FieldName + " like '%" + q + "%'";
        }

        protected virtual string GetExcludeConfig()
        {
            return " and " + FieldName + " not like '%durados%'";
        }

        protected virtual string GetWhereStatementWithId(string q, int id, bool spaceAsWildcard)
        {
            return GetIdFilterFieldName() + " = " + id + " and (" + GetWhereStatement(q, spaceAsWildcard) + ")" + GetExcludeConfig();
        }

        protected virtual string GetIdFilterFieldName()
        {
            return "ID";
        }
    }
}
