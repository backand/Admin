using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.Config.Cloud
{
    public interface IStorage
    {
        void Read(DataSet ds, string filename, string appName, bool isMainMap);

        void Write(DataSet ds, string filename, bool async, Map map, string version);
    }
}
