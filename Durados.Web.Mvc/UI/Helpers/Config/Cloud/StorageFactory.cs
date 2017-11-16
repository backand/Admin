using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.Config.Cloud
{
    public class StorageFactory
    {
        public static IStorage GetStorage(CloudProvider cloudProvider, Map map)
        {
            if (cloudProvider == CloudProvider.Azure)
            {
                return new AzureStorage(map);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
