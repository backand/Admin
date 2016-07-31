using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Data.SqlClient;

using Durados.DataAccess;
using Durados.Web.Mvc;
namespace Durados.Web.Mvc.App.Controllers
{
    [HandleError]
    public class DnsAliasController : Durados.Web.Mvc.Controllers.DuradosController
    {
        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);

            string prevAlias = e.PrevRow["Alias"].ToString();
            string currAlias = e.Values["Alias"].ToString();
            string name = GetAppName(e.PrimaryKey);

            if (Maps.DnsAliases.ContainsKey(prevAlias))
            {
                Maps.DnsAliases.Remove(prevAlias);
            }
            if (Maps.DnsAliases.ContainsKey(currAlias))
            {
                Maps.DnsAliases.Remove(currAlias);
            }

            Maps.DnsAliases.Add(currAlias, name);
        }

        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);

            string currAlias = e.Values["Alias"].ToString();
            string name = GetAppName(e.PrimaryKey);

            if (Maps.DnsAliases.ContainsKey(currAlias))
            {
                Maps.DnsAliases.Remove(currAlias);
            }

            Maps.DnsAliases.Add(currAlias, name);
        }

        protected override void AfterDeleteAfterCommit(DeleteEventArgs e)
        {
            base.AfterDeleteAfterCommit(e);

            string prevAlias = e.PrevRow["Alias"].ToString();
            
            if (Maps.DnsAliases.ContainsKey(prevAlias))
            {
                Maps.DnsAliases.Remove(prevAlias);
            }
        }

        private string GetAppName(string pk)
        {
            SqlAccess sqlAccess = new SqlAccess();

            return sqlAccess.ExecuteScalar(Map.connectionString, "SELECT dbo.durados_App.Name FROM dbo.durados_App INNER JOIN dbo.durados_DnsAlias ON dbo.durados_App.Id = dbo.durados_DnsAlias.AppId WHERE dbo.durados_DnsAlias.Id = " + pk).ToLower();
        }
    }
}

