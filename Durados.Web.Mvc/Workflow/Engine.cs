using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Workflow
{
    public class Engine : Durados.Workflow.Engine
    {
        public Engine()
            : base()
        {
        }

        protected override Durados.Workflow.Notifier GetNotifier()
        {
            return new HistoryNotifier();
        }

        public HistoryNotifier Notifier
        {
            get
            {
                return (HistoryNotifier)notifier;
            }
        }

        protected override Durados.Workflow.WebService GetWebService()
        {
            return new WebService();
        }


        #region sql compatibility

        bool connection_ValidateRemoteCertificateCallback(System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }

        protected override System.Data.IDbConnection GetConnection(Durados.View view)
        {
            if (view.SystemView)
                return new System.Data.SqlClient.SqlConnection(view.ConnectionString);

            switch (view.Database.SqlProduct)
            {
                case SqlProduct.MySql:
                    return new MySql.Data.MySqlClient.MySqlConnection(view.ConnectionString);
                case SqlProduct.Postgre:
                    Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(view.ConnectionString);
                    connection.ValidateRemoteCertificateCallback += new Npgsql.ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
                    return connection;
                case SqlProduct.Oracle:
                    return new Oracle.ManagedDataAccess.Client.OracleConnection(view.ConnectionString);
                case SqlProduct.SqlServer:
                case SqlProduct.SqlAzure:
                    return new System.Data.SqlClient.SqlConnection(view.ConnectionString);

                default:
                    return base.GetConnection(view);
            }
        }

        public override System.Data.IDbCommand GetCommand(string commandText, System.Data.IDbConnection connection)
        {
            if (connection is MySql.Data.MySqlClient.MySqlConnection)
                return new MySql.Data.MySqlClient.MySqlCommand(commandText, (MySql.Data.MySqlClient.MySqlConnection)connection);
            else if (connection is Npgsql.NpgsqlConnection)
                return new Npgsql.NpgsqlCommand(commandText, (Npgsql.NpgsqlConnection)connection);
            else if (connection is Oracle.ManagedDataAccess.Client.OracleConnection)
                return new Oracle.ManagedDataAccess.Client.OracleCommand(commandText, (Oracle.ManagedDataAccess.Client.OracleConnection)connection);

            return base.GetCommand(commandText, connection);
        }

        protected override System.Data.IDataParameter GetNewParameter(System.Data.IDbCommand command, string parameterName, object value)
        {
            if (command is MySql.Data.MySqlClient.MySqlCommand)
                return new MySql.Data.MySqlClient.MySqlParameter(parameterName, value);
            else if (command is Npgsql.NpgsqlCommand)
                return new Npgsql.NpgsqlParameter(parameterName, value);
            else if (command is Oracle.ManagedDataAccess.Client.OracleCommand)
                return new Oracle.ManagedDataAccess.Client.OracleParameter(parameterName, value);

            return base.GetNewParameter(command, parameterName, value);
        }

        protected ISqlTextBuilder GetSqlTextBuilder(Durados.View view)
        {
            if (view.SystemView)
                return new SqlTextBuilder();

            switch (view.Database.SqlProduct)
            {
                case SqlProduct.MySql:
                    return new Durados.DataAccess.MySqlTextBuilder();
                case SqlProduct.Postgre:
                    return new Durados.DataAccess.PostgreTextBuilder();
                case SqlProduct.Oracle:
                    return new Durados.DataAccess.OracleTextBuilder();

                default:
                    return new SqlTextBuilder();
            }
        }

        protected override string GetPkColumnWhereStatement(Durados.View view, string tableName, string columnName)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            return sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + " @pk_" + columnName.ReplaceNonAlphaNumeric() + " " + sqlTextBuilder.DbAnd;
        }

        protected override string GetSelectStatement(Durados.View view, string viewName, string columnName, string whereCondition)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            return "select " + sqlTextBuilder.EscapeDbObject(viewName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " from " + sqlTextBuilder.EscapeDbObject(viewName) + sqlTextBuilder.WithNolock + " where " + whereCondition;
        }

        #endregion sql compatibility

        protected override string GetCurrentUsername(Durados.View view)
        {
            return ((Database)view.Database).GetCurrentUsername();
        }

        protected override object ExecuteNodeJS(object controller, Dictionary<string, Parameter> parameters, Durados.View view, System.Data.DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, int currentUserId, string currentUserRole, System.Data.IDbCommand command, System.Data.IDbCommand sysCommand, string actionName)
        {
            nodeJS.Execute(controller, parameters, view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command, sysCommand, actionName, Maps.LambdaArnRoot, Maps.AwsCredentials, false);
            return null;
        }

        protected override object ExecuteLambda(object controller, Dictionary<string, Parameter> parameters, Durados.View view, System.Data.DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, int currentUserId, string currentUserRole, System.Data.IDbCommand command, System.Data.IDbCommand sysCommand, string actionName, Rule rule)
        {
            nodeJS.Execute(controller, parameters, view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command, sysCommand, actionName, rule.LambdaArn, view.GetRuleCredentials(rule), true);
            return null;
        }

        
    }
}
