using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.SanDisk
{
    public class RequirementsProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new RequirementsDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "SanDiskReqConnectionString";
                return "SanDiskReqConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "SanDiskReqConfig";
            }
        }

    }

    public class AllegroProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new AllegroDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "SanDiskAllegroConnectionString";
                return "SanDiskAllegroConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "SanDiskAllegroConfig";
            }
        }

    }

    public class Allegro1Project : Project
    {
        public override DataSet GetDataSet()
        {
            return new Allegro1DataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "SanDiskAllegro1ConnectionString";
                return "SanDiskAllegro1ConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "SanDiskAllegro1Config";
            }
        }

    }

}
