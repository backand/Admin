using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.DataAccess;

namespace Durados.Web.Mvc.UI
{
    public class MultiTenancyTableViewer : TableViewer
    {
        private int[] userAppsAdminIds;
        public override string GetEditCaption(Durados.View view, DataRow row, string guid)
        {
            if (IsCurrentUserCreator(view, row) || IsCurrentUserAdimn(view,row))
                return base.GetEditCaption(view, row, guid);
            else
                return Maps.Instance.GetMap().Database.Localizer.Translate("View");
        }

        private bool IsCurrentUserAdimn(Durados.View view, DataRow row)
        {

            if (row.IsNull("Id") || string.IsNullOrEmpty(row["Id"].ToString()))
            {
                throw new DuradosException("Application missing Id. Cannot determine if user is admin.");
            }
            int appId;
            if (!int.TryParse(row["Id"].ToString(), out appId))
                throw new DuradosException("Application Id can not be parsed");
            string userId = ((Database)view.Database).GetUserID();
            if (userAppsAdminIds == null)
                LoadUserAdminApps(userId);
            if (userAppsAdminIds!= null && userAppsAdminIds.Contains(appId))
                return true;
            else
                return false;
        }

        private void LoadUserAdminApps(string userId)
        {
            
            SqlAccess sqlAccess = new SqlAccess();

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@userId", userId);
          //  parameters.Add("@appId",appId );
            
            string sql = "select appId from durados_UserApp where  userid=@userId and  ([role]='admin' or [role]='Developer')";

            object scalar = sqlAccess.ExecuteTable(Maps.Instance.ConnectionString, sql, parameters,CommandType.Text);

            if (scalar == null || scalar == DBNull.Value)
                return;
            if (scalar is DataTable)
            {
                DataTable dt = scalar as DataTable;
                userAppsAdminIds =dt.AsEnumerable().Select(s => s.Field<int>("appId")).ToArray<int>();
            }

        }

        private  bool IsCurrentUserCreator(Durados.View view, DataRow row)
        {
            bool isCreator;
            isCreator= !row.IsNull("Creator") && row["Creator"].ToString().Equals(((Database)view.Database).GetUserID());

            return isCreator;
        }

        public override string GetEditTitle(Durados.View view, DataRow row)
        {
            if (row["Creator"].Equals(((Database)view.Database).GetUserID()))
                return base.GetEditTitle(view, row);
            else
                return Maps.Instance.GetMap().Database.Localizer.Translate("View current record");
        }

    }
}
