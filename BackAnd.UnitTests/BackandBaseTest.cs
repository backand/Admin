using Backand.Config;
using Durados.Web.Mvc;
using System;
using System.Data;
using System.Diagnostics;
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

        public ServerConfig Configuration { get { return ConfigStore.GetConfig(); } }


        public Map ValidMap
        {
            get
            {
                var appName = this.ValidAppName;
                var res = Maps.Instance.GetMap(appName);
                return res;
            }
        }


        protected void InitInner()
        {
            try
            {
                Trace.WriteLine("Start Init");
                TestHelper.Init();
            }
            catch (Exception e)
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

        public static DataSet GetSimpleDataSet()
        {
            DataSet ds = new DataSet();
            DataTable table = new DataTable();
            table.Columns.Add("a", typeof(int));
            table.Columns.Add("b", typeof(string));
            table.Columns.Add("c", typeof(bool));
            table.Rows.Add(1, "s", true);
            ds.Tables.Add(table);

            return ds;
        }
    }
}
