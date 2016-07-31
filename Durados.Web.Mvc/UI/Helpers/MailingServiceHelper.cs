using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Durados.DataAccess;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class MailingServiceHelper
    {
        public static DataTable GetNewSubscribers(int bookmarkId, View view, View mailngServiceView, int dataActionId, string[] fields, string emailFieldName)
        {
            SqlAccess sqlAccess = new SqlAccess();
            HashSet<string> subscribed = GetSubscribed(mailngServiceView, sqlAccess, dataActionId);
            Dictionary<string, Type> fd = new Dictionary<string, Type>();
            if (fields == null || fields.Length == 0)
            {
                //subscribers.SetColumnsOrder(fields);
                fd = view.Fields.Values.OrderBy(x => x.Order).Where(w => !w.HideInTable).ToDictionary(f => f.DisplayName, f => f.View.DataTable.Columns[f.DatabaseNames].DataType);

            }
            else
                fd = fields.ToDictionary(f => f, f => typeof(string));
            //DataTable subscribers=new DataTable("Subscribers");
            //foreach (KeyValuePair<string,Type> f in fd)
            //{
            //    subscribers.Columns.Add(f.Key, f.Value);
            //}

            Durados.Bookmark bookmark = BookmarksHelper.GetBookmark(bookmarkId);

            int rowCount;
            Filter filter = GetBookmarkFilter(view, bookmark, sqlAccess);
            Durados.SortDirection direction = (SortDirection)Enum.Parse(typeof(SortDirection), bookmark.SortDirection);
            DataTable dt = sqlAccess.FillDataTable(view, 1, GetMaxPageSize(), filter, bookmark.SortColumn, direction, out rowCount, null, null, null, null).Copy();

            //dt.SetColumnsOrder(fd.Keys.ToArray());

            dt.Columns.Add("PK", typeof(string));

            SetRows(view, subscribed, dt, emailFieldName);

            SetColumns(view, fd.Keys.ToArray(), dt);

            //dt.SetColumnsOrder(fd.Keys.ToArray());

            return dt;

        }

        private static void SetColumns(View view, string[] fields, DataTable dt)
        {
            List<DataColumn> removedColumn = new List<DataColumn>();
            dt.PrimaryKey = null;
            foreach (DataColumn dc in dt.Columns)
            {
                Field field = view.GetFieldByColumnNames(dc.ColumnName);
                if (field != null)
                {
                    dc.ColumnName = field.DisplayName;
                }
                if (!fields.Contains(dc.ColumnName) && dc.ColumnName != "PK")

                    removedColumn.Add(dc);

            }
            for (int i = 0; i < removedColumn.Count; i++)
                dt.Columns.Remove(removedColumn[i]);
        }

        private static void SetRows(View view, HashSet<string> subscribed, DataTable dt, string emailFieldName)
        {
            string _emailFieldName = emailFieldName ?? "EMAIL";
            HashSet<string> hTable = new HashSet<string>();
            List<DataRow> removedRow = new List<DataRow>();
            if (!dt.Columns.Contains(_emailFieldName))
                throw new DuradosException("bookmark is missingemail column :" + _emailFieldName);
            foreach (DataRow dr in dt.Rows)
            {
                string email = dr[dt.Columns[_emailFieldName]].ToString();
                dr["PK"] = view.GetPkValue(dr);
                if (string.IsNullOrEmpty(email) || hTable.Contains(email) || subscribed.Contains(email))//- used for mailchimp
                    removedRow.Add(dr);

                if (!string.IsNullOrEmpty(email) && !hTable.Contains(email))
                    hTable.Add(email);

            }
            for (int i = 0; i < removedRow.Count; i++)
                dt.Rows.Remove(removedRow[i]);
        }


        private static HashSet<string> GetSubscribed(View mailngServiceView, SqlAccess sqlAccess, int dataActionId)
        {
            string sql = string.Format("SELECT [Email] FROM [durados_MailingServiceSubscribers] WHERE [IsSubscribed]=1 and DataAction={0}", dataActionId);
            DataTable subscribed = sqlAccess.ExecuteTable(mailngServiceView.ConnectionString, sql, null, CommandType.Text);
            HashSet<string> subscribedHash = new HashSet<string>();
            foreach (string s in subscribed.AsEnumerable().Select(d => d.Field<string>("EMAIL")))
            {
                subscribedHash.Add(s);
            }
            return subscribedHash;
        }

        public static Filter GetBookmarkFilter(View view, Durados.Bookmark bookmark, SqlAccess sqlAccess)
        {
            System.Web.Mvc.FormCollection filterCollection = new System.Web.Mvc.FormCollection(new UI.Json.NameValueCollectionSerializer(bookmark.Filter).NameValueCollection);
            //HttpContext.Current.Session[guid] = c;

            UI.Json.Filter jsonFilter = ViewHelper.GetJsonFilter(filterCollection, bookmark.ViewName);


            Dictionary<string, object> values = new Dictionary<string, object>();

            if (jsonFilter != null)
            {
                foreach (UI.Json.Field jsonField in jsonFilter.Fields.Values)
                {
                    if (jsonField.Value != null && !string.IsNullOrEmpty(jsonField.Value.ToString()))
                    {
                        values.Add(jsonField.Name, jsonField.Value);
                    }

                }
            }
            Filter filter = sqlAccess.GetFilter(view, values);

            return filter;
        }

        private static int GetMaxPageSize()
        {
            return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxMailingServiceSubscribers"] ?? "1000000");
        }


        public static void SetNewSubscribed(int bookmarkId, View view, View mailngServiceView, int dataActionId, DataTable subscribers, Dictionary<string, string> errors, int userId)
        {
            DataTable typeTable = GetTypeTable(bookmarkId, view, mailngServiceView, dataActionId, subscribers, errors, userId);

            SqlAccess sqlAccess = new SqlAccess();

            string sql = "durados_UpdateMailingServiceSubscribers";
            sqlAccess.ExecuteNonQueryBulk(mailngServiceView.ConnectionString, sql, null, typeTable, null);

        }

        private static DataTable GetTypeTable(int bookmarkId, View view, View mailngServiceView, int dataActionId, DataTable subscribers, Dictionary<string, string> errors, int userId)
        {
            DataTable copy = mailngServiceView.DataTable.Copy();

            copy.TableName = "durados_MailingServiceSubscribersType";
            copy.PrimaryKey = null;
            copy.Columns.Remove("Id");
            copy.Clear();

            if (!copy.Columns.Contains("PK"))
            {
                copy.Columns.Add("PK", typeof(string));
            }

            DateTime now = DateTime.Now;

            foreach (DataRow row in subscribers.Rows)
            {
                DataRow newRow = copy.NewRow();
                string email = row.IsNull("Email") ? null : row["Email"].ToString();
                if (copy.Columns.Contains("PK"))
                {
                    newRow["PK"] = row["PK"];
                }
                if (subscribers.Columns.Contains("Email") && copy.Columns.Contains("Email"))
                {
                    newRow["Email"] = email;
                }

                if (copy.Columns.Contains("DataAction"))
                {
                    newRow["DataAction"] = dataActionId;
                }

                if (copy.Columns.Contains("BookmarkId"))
                {
                    newRow["BookmarkId"] = bookmarkId;
                }
                if (copy.Columns.Contains("Date"))
                {
                    newRow["Date"] = now;
                }
                if (copy.Columns.Contains("UserId"))
                {
                    newRow["UserId"] = userId;
                }
                if (copy.Columns.Contains("IsSubscribed") && email != null)
                {
                    if (errors != null && errors.ContainsKey(email))
                    {
                        newRow["IsSubscribed"] = false;
                        if (copy.Columns.Contains("Errors"))
                        {
                            string error = errors[email];
                            if (errors[email].Length >= 250)
                                error = error.Substring(0, 249);
                            newRow["Errors"] = error;
                        }
                    }
                    else
                    {
                        newRow["IsSubscribed"] = true;
                    }
                }
                copy.Rows.Add(newRow);
            }

            return copy;
        }
    }

}
