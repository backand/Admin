using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Durados.DataAccess
{
    public class SqlRequest
    {
        #region Constructors (2)

        public SqlRequest(string sql, IList<SqlParameter> parameters)
        {
            Sql = sql;
            Parameters = parameters;
        }

        public SqlRequest()
        {
        }

        #endregion Constructors

        #region Properties (2)

        public IList<SqlParameter> Parameters { get; set; }

        public string Sql { get; set; }

        #endregion Properties
    }
}
