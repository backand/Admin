using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Data
{
    public interface IData
    {
        IDataHandler DataHandler { get; }
    }

    public interface IDataHandler
    {
        System.Data.DataRow GetDataRow(Durados.View view, string pk, System.Data.IDbCommand command);

        string Post(string name, string json, IDbCommand command, IDbCommand sysCommand, bool? deep = null, bool? returnObject = null, string parameters = null);

        string Put(string name, string id, string json, IDbCommand command, IDbCommand sysCommand, bool? deep = null, bool? returnObject = null, string parameters = null, bool? overwrite = null);

        string Delete(string name, string id, IDbCommand command, IDbCommand sysCommand, bool? deep = null, string parameters = null);

        string PerformCrud(System.Net.WebRequest request, string json, Dictionary<string, object> executeArgs);
    }

    public class DataHandlerException : Durados.DuradosException
    {
        public int Status { get; private set; }
        public DataHandlerException(int status, string message, Exception innerException = null)
            : base(message, innerException)
        {
            Status = status;
        }
    }
}
