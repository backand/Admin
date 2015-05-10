using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Durados.Data
{
    public interface IDataAccess
    {
        IDbConnection GetConnection(string connectionString=null);
        IDbCommand GetCommand(string connectionStringg = null);
        IDbCommand GetCommand(IDbConnection cnn, string cmdText);

        ISqlTextBuilder GetSqlBuilder();
        string ConnectionString { get; set; }

        
    }

    
}





