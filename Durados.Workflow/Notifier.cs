using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Durados.Workflow
{
    public class Notifier
    {
        public delegate void DelegateNotify(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string mainSiteWithoutQueryString, string urlAction, string connectionString, int currentUserId, string currentUsername, string currentUserRole, IDbCommand command);

        public virtual void Notify(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command)
        {
            DelegateNotify delegateNotify =
                new DelegateNotify(NotifyAsync);

            // call the BeginInvoke function!

            string siteWithoutQueryString = ((INotifier)controller).GetSiteWithoutQueryString();
            string mainSiteWithoutQueryString = ((INotifier)controller).GetMainSiteWithoutQueryString();
            string urlAction = ((INotifier)controller).GetUrlAction(view, pk);

            IAsyncResult tag =
                delegateNotify.BeginInvoke(controller, parameters, view, values, prevRow, pk, siteWithoutQueryString, mainSiteWithoutQueryString, urlAction, connectionString, currentUserId, view.Database.GetCurrentUsername(), currentUserRole, command, null, null);

        }

        public virtual void NotifyAsync(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string mainSiteWithoutQueryString, string urlAction, string connectionString, int currentUserId, string currentUsername, string currentUserRole, IDbCommand command)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            EmailParameters emailParameters = GetEmailParameters(controller, parameters, view, values, prevRow, pk, siteWithoutQueryString, mainSiteWithoutQueryString, connectionString, currentUserId.ToString(), currentUsername, currentUserRole);

            View userView = view.Database.GetUserView();
            
            Dictionary<int, bool> recipients = GetRecipients(emailParameters, userView);

            if (string.IsNullOrEmpty(emailParameters.From))
                emailParameters.From = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            
            if (emailParameters.To.Length > 0)
                Durados.Cms.DataAccess.Email.Send(host, view.Database.UseSmtpDefaultCredentials, port, username, password, false, emailParameters.To, emailParameters.Cc, emailParameters.Bcc, emailParameters.Subject, emailParameters.Message, emailParameters.From, null, null, ((INotifier)controller).DontSend, view.Database.Logger);

            if (ShouldSaveInMessageBoard(controller, parameters, view, values, prevRow, pk, siteWithoutQueryString, connectionString))
            {
                SaveInMessageBoard(controller, parameters, view, values, prevRow, pk, siteWithoutQueryString, urlAction, connectionString, emailParameters.Subject, emailParameters.Message, currentUserId, currentUserRole, recipients);
            }
        }

        private Dictionary<int, bool> GetRecipients(EmailParameters emailParameters, View userView)
        {
            HashSet<string> emails = new HashSet<string>();

            foreach (string email in emailParameters.To)
            {
                if (!emails.Contains(email))
                {
                    emails.Add(email);
                }
            }

            Dictionary<int, bool> recipients = new Dictionary<int, bool>();

            if (emails.Count >= 1)
            {
                Durados.Data.IDataAccess dal = userView.Database.GetDataAccess(userView.ConnectionString);
                ISqlTextBuilder sqlBuilder = dal.GetSqlBuilder();
                string emailString = emails.ToArray().Delimited("'","'");

                string sql = GetSelectPKStatement(userView, sqlBuilder, emailString);

                using (IDbConnection connection =dal.GetConnection(userView.ConnectionString))// GetConnection(userView.ConnectionString))
                {
                    connection.Open();

                    using (IDbCommand command = dal.GetCommand(connection, sql))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int i = reader.GetOrdinal("ID");
                                if (!reader.IsDBNull(i))
                                    recipients.Add( reader.GetInt32(i), false);
                                
                            }
                        }
                    }
                }

            }

            return recipients;
        }

        protected virtual string GetSelectPKStatement(View userView, ISqlTextBuilder sqlBuilder, string emailString)
        {
            return "SELECT " + sqlBuilder.EscapeDbObjectStart + userView.DataTable.PrimaryKey[0].ColumnName + sqlBuilder.EscapeDbObjectEnd + " FROM " + sqlBuilder.EscapeDbObjectStart + userView.DataTable.TableName + sqlBuilder.EscapeDbObjectEnd + " where Email in (" + emailString + ")";
        }

        protected virtual bool ShouldSaveInMessageBoard(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string connectionString)
        {
            return !parameters.ContainsKey("ExcludeMessageBoard");
        }

        protected virtual void SaveInMessageBoard(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string urlAction, string connectionString, string subject, string message, int currentUserId, string currentUserRole, Dictionary<int, bool> recipients)
        {
            ((INotifier)controller).SaveInMessageBoard(parameters, view, values, prevRow, pk, siteWithoutQueryString, urlAction, subject, message, currentUserId, currentUserRole, recipients);
        }

        protected virtual EmailParameters GetEmailParameters(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string mainSiteWithoutQueryString, string connectionString, string currentUserId, string currentUsername, string currentUserRole)
        {
            EmailParameters emailParameters = new EmailParameters();

            if (parameters.ContainsKey("from"))
                emailParameters.From = parameters["from"].Value;
            if (parameters.ContainsKey("to"))
                emailParameters.To = parameters["to"].Value.Split(';');
            if (parameters.ContainsKey("cc"))
                emailParameters.Cc = parameters["cc"].Value.Split(';');
            if (parameters.ContainsKey("bcc"))
                emailParameters.Bcc = parameters["bcc"].Value.Split(';');
            if (parameters.ContainsKey("subject"))
                emailParameters.Subject = parameters["subject"].Value;
            if (parameters.ContainsKey("message"))
                emailParameters.Message = parameters["message"].Value;



            return emailParameters;
        }

        

       
       
    }

    public class DictionaryField
    {
        public string DisplayName { get; set; }
        public DataType Type { get; set; }
        public object Value { get; set; }
    }

    public interface INotifier
    {
        bool DontSend {get;}
        string GetSiteWithoutQueryString();
        string GetMainSiteWithoutQueryString();
        string GetUrlAction(View view, string pk);
        string SaveInMessageBoard(Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string urlAction, string subject, string message, int currentUserId, string currentUserRole, Dictionary<int, bool> recipients);
        void SaveMessageAction(View view, string pk, object value, int messageBoardAction, int userId);

        string GetFieldValue(Field field, string pk);
        ITableConverter GetTableViewer();
        void LoadValues(Dictionary<string, object> values, DataRow dataRow, View view, ParentField parentField, View rootView, string dynastyPath, string prefix, string postfix, Dictionary<string, DictionaryField> dicFields, string internalDynastyPath);
        View GetNonConfigView(string viewName);
        string HtmlDecode(string text);
    }
}
