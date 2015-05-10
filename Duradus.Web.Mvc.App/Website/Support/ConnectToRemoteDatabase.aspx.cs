using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Durados.Web.Mvc.App.Website.Support
{
    public partial class ConnectToDatabase : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (Request.QueryString["Master"] == "Simple")
            {
                MasterPageFile = "~/Website/Support/Support.Master";
               
            }
           

        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}