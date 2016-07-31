using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailChimp;
using MailChimp.Types;
using Durados.Services;
using System.Data;

namespace Durados.DataAccess
{

    public class MailChimpService : IMailingService
    {
        private string _apiKey;
        private string _listId;
        private bool useFieldOrder = false;
        public bool UseFieldOrder { get { return useFieldOrder; } set { useFieldOrder = value; } }
        string subscribeFieldName = "MERGE";
        public string SubscribeFieldName { get { return subscribeFieldName; } set { subscribeFieldName = value; } }
        public MailChimpService(string apiKey, string listId)
        {
            _apiKey = apiKey;
            _listId = listId;
        }

        public MailChimpService()
        {

        }

        public Dictionary<string, string> SubscribeBatch(DataTable subscribers)
        {

            var mc = new MCApi(_apiKey, true);
            var subscribeOptions =
                new List.SubscribeOptions
                {
                    DoubleOptIn = false,
                    SendWelcome = true,
                    UpdateExisting = true,
                };

            var merges = new List<List.Merges>();

            Dictionary<int, string> fields = GetFieldsName(subscribers);
            try
            {
                subscribers.Columns["EMAIL"].AllowDBNull = false;
            }
            catch (Exception ex)
            {
                throw new DuradosException("Email is missing", ex);
            }
            try
            {
                subscribers.PrimaryKey = new DataColumn[] { subscribers.Columns["EMAIL"] };
            }
            catch (Exception ex)
            {
                throw new DuradosException("Email must be unique", ex);
            }


            foreach (DataRow row in subscribers.Rows)
            {
                var merge = new List.Merges();

                for (int i = 0; i < subscribers.Columns.Count; i++)
                {
                    string val = row[i].ToString();
                    if (subscribers.Columns[i].DataType == typeof(bool))
                        val = Convert.ToInt32(row.IsNull(i) ? false : row[i]).ToString();
                    merge.Add(fields[i], val);
                }
                merges.Add(merge);
            }
            if (merges.Count == 0)
                return null;
            List.BatchSubscribe lbs = mc.ListBatchSubscribe(_listId, merges, subscribeOptions);

            return GetErrors(subscribers, lbs);

        }

        private Dictionary<string, string> GetErrors(DataTable subscribers, List.BatchSubscribe lbs)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            if (lbs.ErrorCount == 0)
                return errors;
            //DataColumn hasErrColum = subscribers.Columns.Add("HasError", typeof(bool));
            //DataColumn errMessageColum = subscribers.Columns.Add("ErrorMessage", typeof(string));


            foreach (List.BatchError bErr in lbs.Errors)
            {
                // string filterExp = string.Format(@"Email = '{0}'", bErr.Email);
                DataRow errRows = subscribers.Rows.Find(bErr.Email);
                if (errors.ContainsKey(bErr.Email))
                {
                    errors[bErr.Email] += ";" + bErr.Message;
                }
                else
                    errors.Add(bErr.Email, bErr.Message);

            }
            return errors;

        }

        private Dictionary<int, string> GetFieldsName(DataTable subscribers)
        {
            Dictionary<int, string> fields = new Dictionary<int, string>();
            if (UseFieldOrder)
            {
                for (int i = 0; i < subscribers.Columns.Count; i++)
                    fields.Add(i, SubscribeFieldName + i.ToString());
            }
            else
            {
                for (int i = 0; i < subscribers.Columns.Count; i++)
                    fields.Add(i, subscribers.Columns[i].ColumnName);
            }
            return fields;
        }

        public Dictionary<string, string> SubscribeBatch(DataTable subscribers, string listId, string apiKey)
        {
            _listId = listId;
            _apiKey = apiKey;
            return SubscribeBatch(subscribers);

        }



    }

}
