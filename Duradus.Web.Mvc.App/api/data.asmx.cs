using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.IO;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Controllers.Filters;

namespace Durados.Web.Mvc.App.api
{
    /// <summary>
    /// Summary description for data
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class data : System.Web.Services.WebService
    {

        [WebMethod]
        public string AddRow(string tableName, List<d_Field> fields)
        {
            return ViewHelper.AddRow(tableName, fields);
        }

        [WebMethod]
        public List<d_Field> GetFieldsCollection(string tableName)
        {
            return ViewHelper.GetFieldsCollection(tableName);
        }
    }

    
}
