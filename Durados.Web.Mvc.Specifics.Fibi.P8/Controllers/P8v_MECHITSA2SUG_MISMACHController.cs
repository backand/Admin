using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml;
using System.IO;

namespace Durados.Web.Mvc.Specifics.Fibi.P8.Controllers
{
    public class P8v_MECHITSA2SUG_MISMACHController : P8CMBaseController
    {

        protected override void BeforeCreateInDatabase(CreateEventArgs e)
        {
            base.BeforeCreateInDatabase(e);
            e.ColumnNames.Remove("WORKSPACE2");
        }        
        
    }
}
