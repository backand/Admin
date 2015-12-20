using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    public interface IFarmCaching
    {
        void ClearInternalCache(string appName);

        void ClearMachinesCache(string appName, bool async = false);

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

        public void ClearMachinesCache(string appName, bool async = false)
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
    }

}
