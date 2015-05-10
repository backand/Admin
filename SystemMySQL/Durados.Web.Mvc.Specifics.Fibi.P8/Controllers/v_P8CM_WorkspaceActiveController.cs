using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml;
using System.IO;

namespace Durados.Web.Mvc.Specifics.Fibi.P8.Controllers
{
    public class v_P8CM_WorkspaceActiveController : P8CMBaseController
    {

        protected override void BeforeCreateInDatabase(CreateEventArgs e)
        {
            string id = GetP8CMRootSvivotId(e.View.Database.ConnectionString);
            string parentName = "FK_v_P8CMRootSvivot_v_P8CM_Workspace_Parent";

            if (string.IsNullOrEmpty(id))
                throw new DuradosException("נא לבחור סביבת עבודה פעילה");
            if (e.Values.ContainsKey(parentName))
            {
                e.Values[parentName] = id;
            }
            else
            {
                e.Values.Add(parentName, id);
            }
        }

        protected virtual string GetP8CMRootSvivotId(string connectionString)
        {
            object scalar = null;

            string sql = "SELECT Id FROM P8CM_Config with(nolock) WHERE Active = 1";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    scalar = command.ExecuteScalar();

                }
            }

            if (scalar == null || scalar == DBNull.Value)
                return null;
            else
                return scalar.ToString();
        }
        
    }
}
