using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc
{
    public interface IPersistency
    {
        string ConnectionString { get; set; }
        string SystemConnectionString { get; set; }
        object GetConnection(MapDataSet.durados_AppRow appRow, object builder);
        object GetSqlServerConnection(MapDataSet.durados_AppRow appRow, object builder);
        object GetMySqlConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort);
        object GetPostgreConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort);
        object GetOracleConnection(MapDataSet.durados_AppRow appRow, object builder, int localPort);
        object GetSystemConnection(MapDataSet.durados_AppRow appRow, object builder);
        object GetSecurityConnection(MapDataSet.durados_AppRow appRow, object builder);
        object GetLogConnection(MapDataSet.durados_AppRow appRow, object builder);
    }
}
