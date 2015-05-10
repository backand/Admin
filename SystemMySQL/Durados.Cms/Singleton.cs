using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms
{
    public class Singleton
    {
        public static string ConnectionString { get; set; }
        public static Durados.Cms.Model.CmsDB Cms { get; private set; }
        static Singleton()
        {
            
        }

        public static void Refresh()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                return;
            }

            Cms = new Cms.Model.CmsDB(ConnectionString);
   
        }
    }
}
