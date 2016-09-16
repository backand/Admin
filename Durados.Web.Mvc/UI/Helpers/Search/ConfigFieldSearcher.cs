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

        public object Search(ConfigAccess config, string q, int? id, int snippetLength)
        {
            string whereStatement = GetWhereStatement(q, id);

            DataView dataView = config.GetRows(ViewName, whereStatement, Maps.Instance.GetMap().GetConfigDatabase().ConnectionString);

            if (dataView.Count == 0)
                return null;

            return GetResults(dataView, q, snippetLength);
        }

        protected virtual object GetResults(DataView dataView, string q, int snippetLength)
        {
            List<object> list = new List<object>();
            foreach (System.Data.DataRowView row in dataView)
            {
                list.Add(new { id = GetId(row), snippet = GetSnippet(row, q, snippetLength) });
            }

            return list.ToArray();
        }

        protected virtual object GetId(System.Data.DataRowView row)
        {
            return row[GetIdFilterFieldName()];
        }

        protected virtual object GetSnippet(System.Data.DataRowView row, string q, int snippetLength)
        {
            return row[FieldName];
        }

        
        private string GetWhereStatement(string q, int? id)
        {
            if (id.HasValue)
            {
                return GetWhereStatementWithId(q, id.Value);
            }
            else
            {
                return GetWhereStatement(q);
            }
        }

        protected virtual string GetWhereStatement(string q)
        {
            return FieldName + " like '%" + q + "%'";
        }

        protected virtual string GetWhereStatementWithId(string q, int id)
        {
            return GetIdFilterFieldName() + " = " + id + " and " + FieldName + " like '%" + q + "%'";
        }

        protected virtual string GetIdFilterFieldName()
        {
            return "ID";
        }
    }
}
