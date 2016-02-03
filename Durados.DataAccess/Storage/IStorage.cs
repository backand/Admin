using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.DataAccess.Storage
{
    public interface IStorage
    {
        void ReadConfigFromCloud(DataSet ds, string filename);

        void WriteConfigToCloud(DataSet ds, string filename);

        bool IsMainApp(string fileName);


        void WriteToStorage(DataSet ds, string filename);

        void ReadFromStorage(DataSet ds, string filename);

        bool Exists(string filename);

        Durados.Data.ICache<DataSet> ConfigCache { get; }

        Durados.Data.ICache<object> LockerCache { get; }
    }
}
