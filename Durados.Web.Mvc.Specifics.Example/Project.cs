using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Example
{
    public class ExampleProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new Durados.Web.Mvc.Specifics.Example.ExampleDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "ExampleConnectionString";
                return "ExampleConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "ExampleConfig";
            }
        }

    }
}
