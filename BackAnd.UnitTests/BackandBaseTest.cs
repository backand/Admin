using Backand.Config;
using Durados.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace BackAnd.UnitTests
{
    public abstract class BackandBaseTest
    {

        public string ValidAppName
        {
            get
            {
                return ConfigStore.GetConfig().appname;
            }
        }


        protected void InitInner()
        {
            try
            {
                Trace.WriteLine("Start Init");
                TestHelper.Init();
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.StackTrace);
                Trace.WriteLine(e);
            }
        }

        protected void CleanupInner()
        {
           
        }
    }


    public static class TestHelper
    {
        public static void Init()
        {
            Maps.Version = "4";
            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null));

            Maps.Instance.RemoveMap(ConfigStore.GetConfig().appname);
        }
    }
}
