using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace Durados.Workflow
{
    public class Engine
    {
        protected Notifier notifier;
        protected Executer executer;
        protected JavaScript javaScript;
        protected NodeJS nodeJS;
        protected WebService webService;
        protected Document document;
        protected ApprovalProcess approvalProcess;
        protected XmlDocument xmlDocument;
        protected Validator validator;
        protected LogicalParser parser;
        protected Step step;

        public static MemoryCache CurrentDatabases = new MemoryCache("CurrentDatabases");
       
        public Engine()
        {
            notifier = GetNotifier();
            executer = GetExecuter();
            javaScript = GetJavaScript();
            nodeJS = GetNodeJS();
            webService = GetWebService();
            step = GetStep();
            document = GetDocument();
            approvalProcess = GetApprovalProcess();
            xmlDocument = GetXmlDocument();
            validator = GetValidator();
            parser = GetLogicalParser();
        }

        protected virtual Notifier GetNotifier()
        {
            return new Notifier();
        }

        protected virtual Executer GetExecuter()
        {
            return new Executer();
        }

        protected virtual JavaScript GetJavaScript()
        {
            return new JavaScript();
        }

        protected virtual NodeJS GetNodeJS()
        {
            return new NodeJS();
        }

        protected virtual WebService GetWebService()
        {
            return new WebService();
        }

        protected virtual Step GetStep()
        {
            return new Step();
        }

        protected virtual Document GetDocument()
        {
            return new Document();
        }

        protected virtual ApprovalProcess GetApprovalProcess()
        {
            return new ApprovalProcess();
        }

        protected virtual XmlDocument GetXmlDocument()
        {
            return new XmlDocument();
        }

        protected virtual Validator GetValidator()
        {
            return new Validator();
        }

        protected virtual LogicalParser GetLogicalParser()
        {
            return new LogicalParser();
        }

        public Dictionary<string, object> Results { private set; get; }

        public Step.Result StepResult { set; get; }

        public virtual void PerformActions(object controller, Durados.View view, Durados.TriggerDataAction dataAction, Dictionary<string, object> values, string pk, DataRow prevRow, string connectionString, int currentUserId, string currentUserRole, IDbCommand command, IDbCommand sysCommand, string ruleName = null)
        {
            SetCurrentDatabase(view);

            IEnumerable<Rule> rules = view.GetRules().Where(r => r.ShouldTrigger(dataAction)).OrderBy(r => r.Name).ToList();
            if (ruleName != null)
                rules = rules.Where(r => r.Name.Equals(ruleName)).ToList();

            List<Rule> failedRules = new List<Rule>();
            List<Exception> exceptions = new List<Exception>();

            Results = new Dictionary<string, object>();

            foreach (Durados.Rule rule in rules)
            {
                bool isChecked = Check(view, rule, dataAction, values, pk, prevRow, rule.UseSqlParser, connectionString, currentUserId, currentUserRole);
                if (isChecked)
                {
                    try
                    {
                        view.Database.Logger.Log(view.Name, "Start", "PerformAction " + rule.Name, "Engine", "", 14, view.Database.Logger.NowWithMilliseconds(), DateTime.Now);
                        object result = PerformAction(controller, rule.WorkflowAction, rule.GetParameters(), view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command, sysCommand, rule.Name, dataAction, rule);
                        view.Database.Logger.Log(view.Name, "End", "PerformAction " + rule.Name, "Engine", "", 14, view.Database.Logger.NowWithMilliseconds(), DateTime.Now);
                        if (result != null)
                        {
                            Results.Add(rule.Name, result);
                            if (result is Step.Result)
                            {
                                if (StepResult != null)
                                    throw new WorkflowEngineException("Must be only one Complete Step rule");

                                StepResult = (Step.Result)result;
                            }
                        }
                    }
                    catch (SqlExecuteException sqlExecuteException)
                    {
                        throw new WorkflowEngineException(sqlExecuteException.Message + "\nat (" + view.JsonName + "/" + rule.Name + ")");
                    }
                    catch (WorkflowEngineException workflowEngineException)
                    {
                        if ((rule.Name == "Create My App User" || rule.Name == "Update My App User" || rule.Name == "Delete My App User") && !view.Database.Views.ContainsKey("users"))
                        {

                        }
                        else
                            throw workflowEngineException;
                    }
                    catch (Exception exception)
                    {
                        failedRules.Add(rule);

                        exceptions.Add(exception);
                    }
                }

            }

            if (exceptions.Count > 0)
            {
                //Backand.Logger.Log(exceptions[0].Message + "\n" + exceptions[0].StackTrace, 501);
                if (IsDebug())
                    throw new WorkflowEngineException(exceptions.ToArray(), failedRules.ToArray());
                else
                    throw exceptions[0];
            }

        }

        private bool IsDebug()
        {
            return System.Web.HttpContext.Current.Request.QueryString["$$debug$$"] == "true";
        }

        private static void SetCurrentDatabase(View view)
        {
            try
            {
                string appName = view.Database.GetCurrentAppName();
                if (appName == null)
                    return;
                if (!CurrentDatabases.Contains(appName))
                {
                    CurrentDatabases[appName] = view.Database;
                }
            }
            catch { }
        }

        public static Database GetCurrentDatabase()
        {
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items[Database.AppName] != null)
            {
                string appName = System.Web.HttpContext.Current.Items[Database.AppName].ToString();

                if (CurrentDatabases.Contains(appName))
                {
                    return (Database)CurrentDatabases[appName];
                }
                return null;
            }
            return null;
        }

        public static Dictionary<string, object> AsToken(Dictionary<string, object> values)
        {
            if (values == null)
                return null;
            Dictionary<string, object> asToken = new Dictionary<string, object>();

            foreach (string key in values.Keys)
            {
                if (key.StartsWith("{{"))
                    asToken.Add(key, values[key]);
                else
                {
                    if (!asToken.ContainsKey(key.AsToken()))
                    {
                        asToken.Add(key.AsToken(), values[key]);
                    }
                }
            }

            return asToken;
        }

        protected virtual string GetPkColumnWhereStatement(View view, string tableName, string columnName)
        {
            return "[" + tableName + "].[" + columnName + "] = @pk_" + columnName.ReplaceNonAlphaNumeric() + " and ";
        }

        private string GetWhereStatement(View view)
        {
            string whereStatement = string.Empty;

            string tableName = view.DataTable.TableName;

            foreach (DataColumn dataColumn in view.DataTable.PrimaryKey)
            {
                whereStatement += GetPkColumnWhereStatement(view, tableName, dataColumn.ColumnName);
            }

            return whereStatement.Substring(0, whereStatement.Length - 4);
        }

        private IEnumerable<System.Data.SqlClient.SqlParameter> GetWhereParemeters(View view, string pk)
        {
            List<System.Data.SqlClient.SqlParameter> parameters = new List<System.Data.SqlClient.SqlParameter>();
            string[] delimitedValues = pk.Split(',');
            for (int i = 0; i < view.DataTable.PrimaryKey.Length; i++)
            {
                System.Data.SqlClient.SqlParameter parameter = new System.Data.SqlClient.SqlParameter("pk_" + view.DataTable.PrimaryKey[i].ColumnName.ReplaceNonAlphaNumeric(), Convert.ChangeType(delimitedValues[i], view.DataTable.PrimaryKey[i].DataType));
                parameters.Add(parameter);
            }

            return parameters;
        }

        public virtual bool Check(View view, Durados.Rule rule, Durados.TriggerDataAction dataAction, Dictionary<string, object> values, string pk, DataRow prevRow, bool useSqlPareser, string connectionString, int currentUserId, string currentUserRole)
        {
            if (rule.WhereCondition.Equals("true"))
            {
                return true;
            }
            if (useSqlPareser && view != view.Database.GetUserView())
                return CheckUsingSql(view, rule, dataAction, values, pk, prevRow, connectionString, currentUserId, currentUserRole);
            else
                if (string.IsNullOrEmpty(rule.WhereCondition))
                    return true;
          
            return parser.Check(rule.WhereCondition.Replace(currentUserId)
                                .Replace(Database.SysUsernamePlaceHolder.AsToken(), GetCurrentUsername(view))
                                .Replace(Database.SysRolePlaceHolder.AsToken(), currentUserRole)
                                .Replace(GetPrevRowAsToken(view,prevRow)).Replace(GetValuesAsToken(values)).Replace(values)
                                .ReplaceConfig(view));
                                //.ReplaceGlobals(view));

        }

        protected virtual Dictionary<string, object> GetValuesAsToken(Dictionary<string, object> values)
        {
            if (values == null)
                return null;
            
            Dictionary<string, object> valuesAsToken = new Dictionary<string, object>();
            foreach (string key in values.Keys)
            {
                if (key.StartsWith(Database.DictionaryPrefix))
                {
                    valuesAsToken.Add(key, values[key]);
                }
                else
                {
                    valuesAsToken.Add(key.AsToken(), values[key]);
                }
            }

            return valuesAsToken;
        }
        protected virtual Dictionary<string, object> GetPrevRowAsToken(View view,DataRow prevRow)
        {
            if (prevRow == null)
                return null;

            Dictionary<string, object> valuesAsToken = new Dictionary<string, object>();
            foreach (Field field in view.Fields.Values)
            {
                
                    valuesAsToken.Add(field.Name.AsToken(false),field.GetValue(prevRow));
               
            }

            return valuesAsToken;
        }

        protected virtual IDbConnection GetConnection(View view)
        {
            return new SqlConnection(view.Database.ConnectionString);
        }

        public virtual IDbCommand GetCommand(string commandText, IDbConnection connection)
        {
            return new SqlCommand(commandText, (SqlConnection)connection);
        }

        protected virtual IDataParameter GetNewParameter(IDbCommand command, string parameterName, object value)
        {
            return new SqlParameter(parameterName, value);
        }

        protected virtual string GetSelectStatement(View view, string viewName, string columnName, string whereCondition)
        {
            return "select [" + viewName + "].[" + columnName + "] from " + viewName + " with(nolock) where " + whereCondition;
        }

        protected virtual bool CheckUsingSql(View view, Durados.Rule rule, Durados.TriggerDataAction dataAction, Dictionary<string, object> values, string pk, DataRow prevRow, string connectionString, int currentUserId, string currentUserRole)
        {
            using (IDbConnection connection = GetConnection(view))
            {
                connection.Open();
                string sql;
                if (dataAction == TriggerDataAction.BeforeCreate || dataAction == TriggerDataAction.AfterCreateBeforeCommit || dataAction == TriggerDataAction.AfterCreate || dataAction == TriggerDataAction.AfterDeleteBeforeCommit || dataAction == TriggerDataAction.AfterDelete)
                    sql = GetSql(rule, view, prevRow, null, currentUserId, currentUserRole, values);
                else
                    sql = GetSql(rule, view, prevRow, pk, currentUserId, currentUserRole, values);
                sql = sql.ReplaceWithDollar(view, values);
                sql = sql.ReplaceWithSharp(view, null, prevRow);

                if ((sql.Contains('$') || sql.Contains(Database.DictionaryPrefix + Database.SysPrevPlaceHolder)) && prevRow != null)
                {
                    sql = sql.ReplaceWithDollar(view, values, prevRow);
                }
                if (sql.Contains('$') || sql.Contains('#') || sql.Contains(Database.DictionaryPrefix))
                    return false;

                IDbCommand command = GetCommand(sql, connection);

                if (!string.IsNullOrEmpty(pk))
                {
                    foreach (SqlParameter parameter in GetWhereParemeters(view, pk))
                    {
                        command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                    }
                }

                object scalar = null;
                try
                {
                    scalar = command.ExecuteScalar();
                }
                catch
                {
                    return false;
                }

                if (scalar == null || scalar == DBNull.Value)
                    return false;
                else
                    return true;
            }
        }

        protected virtual string GetSql(Rule rule, View view, DataRow prevRow, string pk, int currentUserId, string currentUserRole, Dictionary<string,object> values)
        {
            string viewName = rule.DatabaseViewName;
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = view.DataTable.TableName;
            }

            string whereCondition = string.Empty;

            if (!string.IsNullOrEmpty(pk))
            {
                whereCondition = GetWhereStatement(view) + " and ";
            }

            
            whereCondition += "(" + rule.WhereCondition + ")";

            string currentUsername = GetCurrentUsername(view);
            whereCondition = whereCondition.Replace(currentUserId);
            whereCondition = whereCondition.Replace(Durados.Database.RolePlaceHolder, currentUserRole, false);
            whereCondition = whereCondition.Replace(Durados.Database.SysUserPlaceHolder.AsToken(), currentUserId.ToString(), false)
                 .Replace(Durados.Database.UsernamePlaceHolder, currentUsername, false).Replace(Durados.Database.SysUsernamePlaceHolder.AsToken(), currentUsername)
                 .Replace(Durados.Database.RolePlaceHolder, currentUserRole, false).Replace(Durados.Database.SysRolePlaceHolder.AsToken(), currentUserRole);

            //whereCondition = whereCondition.ReplaceGlobals(view);
            whereCondition = whereCondition.ReplaceConfig(view);
            try
            {
                whereCondition = whereCondition.ReplaceAllTokens(view, values, pk, currentUserId.ToString(), currentUsername, currentUserRole, prevRow);
            }
            catch { }
            //if (prevRow != null)
            //{
            //    whereCondition = whereCondition.Replace(prevRow);
            //}

            string columnName = null;
            if (view.DataTable.PrimaryKey.Length > 0)
                columnName = view.DataTable.PrimaryKey[0].ColumnName;
            else
                throw new DuradosException("To run workflow rules on the view [" + view.Name + "] it must have a primary key.");

            string sql = GetSelectStatement(view, viewName, columnName, whereCondition);

            return sql;
        }

        protected virtual string GetCurrentUsername(View view)
        {
            return string.Empty;
        }

        protected virtual object PerformAction(object controller, Durados.WorkflowAction action, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command, IDbCommand sysCommand, string actionName, Durados.TriggerDataAction dataAction, Rule rule)
        {
            switch (action)
            {
                case WorkflowAction.Notify:
                    return Notify(controller, parameters, view, prevRow, values, pk, connectionString, currentUserId, currentUserRole, command);
                //case WorkflowAction.Task:
                //    return Task(controller, parameters, view, values, pk, connectionString);
                case WorkflowAction.Validate:
                    return Validate(controller, parameters, view, values, prevRow, pk, connectionString,currentUserId, currentUserRole);
                case WorkflowAction.Execute:
                    return Execute(controller, parameters, view, prevRow, values, pk, connectionString, currentUserId, currentUserRole, command);
                case WorkflowAction.JavaScript:
                    return ExecuteJs(controller, parameters, view, prevRow, values, pk, connectionString, currentUserId, currentUserRole, command, sysCommand, actionName, dataAction);
                case WorkflowAction.NodeJS:
                    return ExecuteNodeJS(controller, parameters, view, prevRow, values, pk, connectionString, currentUserId, currentUserRole, command, sysCommand, actionName);
                case WorkflowAction.Lambda:
                    return ExecuteLambda(controller, parameters, view, prevRow, values, pk, connectionString, currentUserId, currentUserRole, command, sysCommand, actionName, rule);
                case WorkflowAction.WebService:
                    return CallWebService(controller, parameters, view, prevRow, values, pk, connectionString, currentUserId, currentUserRole, command);
                case WorkflowAction.CompleteStep:
                    return CompleteStep(controller, parameters, view, prevRow, values, pk, connectionString, currentUserId, currentUserRole, command);
                case WorkflowAction.Approval:
                    return Approval(controller, parameters, view, prevRow, values, pk, connectionString, command);
                case WorkflowAction.Document:
                    return Document(controller, parameters, view, prevRow, values, pk, connectionString, command);
                case WorkflowAction.Xml:
                    return ExportToXml(controller, parameters, view, prevRow, values, pk, connectionString, currentUserId, currentUserRole, command);
                case WorkflowAction.Custom:
                    return Custom(controller, parameters, values, pk, connectionString);

                default:
                    return null;
            }
        }

        protected virtual object ExportToXml(object controller, Dictionary<string, Parameter> parameters, View view, DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command)
        {
            xmlDocument.Create(controller, parameters, view, values, prevRow, pk, connectionString, command);
            return null;

        }

        protected virtual object CompleteStep(object controller, Dictionary<string, Parameter> parameters, View view, DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command)
        {
            return step.Complete(controller, parameters, view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command, this);
            
        }

        private object Validate(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString,int currentUserId,string currentUserRole)
        {
            validator.Validate(controller, parameters, view, values, prevRow, parser,pk,currentUserId, currentUserRole);
           return null;
        }

        protected virtual object Notify(object controller, Dictionary<string, Parameter> parameters, View view, DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command)
        {
            notifier.Notify(controller, parameters, view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command);
            return null;
        }

        protected virtual object Execute(object controller, Dictionary<string, Parameter> parameters, View view, DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command)
        {
            executer.Execute(controller, parameters, view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command);
            return null;
        }

        protected virtual object ExecuteJs(object controller, Dictionary<string, Parameter> parameters, View view, DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command, IDbCommand sysCommand, string actionName, Durados.TriggerDataAction dataAction)
        {
            javaScript.Execute(controller, parameters, view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command, sysCommand, actionName, dataAction);
            return null;
        }

        protected virtual object ExecuteNodeJS(object controller, Dictionary<string, Parameter> parameters, View view, DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command, IDbCommand sysCommand, string actionName)
        {
            //nodeJS.Execute(controller, parameters, view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command, sysCommand, actionName);
            return null;
        }

        protected virtual object ExecuteLambda(object controller, Dictionary<string, Parameter> parameters, View view, DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command, IDbCommand sysCommand, string actionName, Rule rule)
        {
            //nodeJS.Execute(controller, parameters, view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command, sysCommand, actionName);
            return null;
        }
        
        protected virtual object CallWebService(object controller, Dictionary<string, Parameter> parameters, View view, DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command)
        {
            webService.Call(controller, parameters, view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command);
            return null;
        }

        protected virtual object Document(object controller, Dictionary<string, Parameter> parameters, View view, DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, IDbCommand command)
        {
            document.Create(controller, parameters, view, values, prevRow, pk, connectionString, command);
            return null;
        }

        protected virtual object Approval(object controller, Dictionary<string, Parameter> parameters, View view, DataRow prevRow, Dictionary<string, object> values, string pk, string connectionString, IDbCommand command)
        {
            approvalProcess.Create(controller, parameters, view, values, prevRow, pk, connectionString, command);
            return null;
        }

        protected virtual object Task(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, string pk, string connectionString)
        {
            return null;
        }

        protected virtual object Custom(object controller, Dictionary<string, Parameter> parameters, Dictionary<string, object> values, string pk, string connectionString)
        {
            return null;
        }
    }

    public class WorkflowEngineException : Durados.DuradosException
    {
        private Exception[] exceptions;
        public WorkflowEngineException(string message)
            : base(message)
        {
        }

        public WorkflowEngineException(Exception[] exceptions, Rule[] rules)
            : base("The following action: " + GetNames(rules) + " failed to perform: " + GetException(exceptions).Message, GetException(exceptions))
        {
            this.exceptions = exceptions;
        }

        public override string StackTrace
        {
            get
            {
                if (exceptions != null && exceptions.Count() > 0)
                {
                    string trace = string.Empty;

                    for (int i = 0; i < exceptions.Count(); i++)
                    {
                        trace += System.Environment.NewLine + "Exception" + i + ":" + System.Environment.NewLine + exceptions[i].StackTrace + System.Environment.NewLine;
                    }

                    return trace;
                }
                else
                {
                    return base.StackTrace;
                }
            }
        }

        private static string GetNames(Rule[] rules)
        {
            string names = string.Empty;
            if (rules != null)
            {
                foreach (Rule rule in rules)
                {
                    names += "\"" + rule.Name + "\", ";
                }
            }

            names = names.Trim();

            names = names.TrimEnd(',');

            return names;
        }

        private static Exception GetException(Exception[] exceptions)
        {

            string message = string.Empty;
            if (exceptions != null)
            {
                if (exceptions != null)
                {
                    if (exceptions.Count() == 1)
                        return exceptions[0];

                    foreach (Exception exception in exceptions)
                    {
                        message += exception.Message + ", ";
                    }
                }

                message = message.Trim();

                message = message.TrimEnd(',');

            }

            return new Exception(message);
        }
    }
}
