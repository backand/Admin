using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic
{
    public class PlmChanges : BETchnicalChanges
    {
        protected override string GetPlmIDColumnName()
        {
            return "Id";
        }

        
    }

    public class PLMBEStatus
    {
        private static int GetStatusId(string name, int defaultId)
        {
            int id = defaultId;
            string setting = System.Web.Configuration.WebConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(setting))
            {
                return id;
            }
            else
            {
                Int32.TryParse(setting, out id);
                return id;
            }
        }

        public static int POR
        {
            get
            {
                return GetStatusId("PLMBEStatus-POR", 3);

            }
        }

        public static int NewRequest
        {
            get
            {
                return GetStatusId("PLMBEStatus-NewRequest", 1);

            }
        }

        public static int NewRequestCR
        {
            get
            {
                return GetStatusId("PLMBEStatus-NewRequestCR", 7);

            }
        }

        public static int ChangeRequest
        {
            get
            {
                return GetStatusId("PLMBEStatus-ChangeRequest", 2);


            }
        }
        public static int Canceled
        {
            get
            {
                return GetStatusId("PLMBEStatus-Canceled", 8);


            }
        }
    }

   
}
