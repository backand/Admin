using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    public interface IFarmCaching
    {
        void ClearInternalCache(string appName);

        void ClearMachinesCache(string appName);

        void AsyncCacheStarted(string appName);
        
        void AsyncCacheCompleted(string appName);

        bool IsAsyncCacheCompleted(string appName);

        void AppEnded();
    }

    public class DummyFarmCaching : IFarmCaching
    {
        public void ClearInternalAddresses()
        {
            //throw new NotImplementedException();
        }

        public void ClearInternalCache(string appName)
        {
            //throw new NotImplementedException();
        }

        public void ClearMachinesCache(string appName)
        {
            //throw new NotImplementedException();
        }

        public void AppStarted()
        {
            //throw new NotImplementedException();
        }

        public void ClearMachinesAddresses()
        {
            //throw new NotImplementedException();
        }

        public void AppEnded()
        {
            //throw new NotImplementedException();
        }

        public void AsyncCacheStarted(string appName)
        {

        }

        public void AsyncCacheCompleted(string appName)
        {

        }

        public bool IsAsyncCacheCompleted(string appName)
        {
            return true;
        }

    }

}
