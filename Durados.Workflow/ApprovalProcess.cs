using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class ApprovalProcess
    {
        public ApprovalProcess()
        {
        }


        public virtual void Create(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, IDbCommand command)
        {
            Create((IApprovalProcess)controller, view, pk, parameters, values, prevRow, command);

        }

        protected virtual void Create(IApprovalProcess controller, View view, string pk, Dictionary<string, Parameter> parameters, Dictionary<string, object> values, DataRow prevRow, IDbCommand command)
        {
            View approvalProcessView = GetApprovalProcessView(controller, view, pk, parameters);
            string storedProcedureName = GetStoredProcedureName(controller, view, pk, parameters);
            int approvalProcessType = GetApprovalProcessType(controller, view, pk, parameters);
            string parentView = GetParentView(controller, view, pk, parameters);
            IDataParameter[] specificParameters = GetSpecificParameters(controller, view, pk, parameters);

            int? approvalProcessId = Create(controller, view, pk, approvalProcessView, storedProcedureName, parentView, approvalProcessType, specificParameters, command);

            if (!approvalProcessId.HasValue)
            {
                throw new WorkflowEngineException("The Approval Process was not created.");

            }

            Dictionary<int, string> messages = Notify(controller, parameters, approvalProcessId.Value, pk, approvalProcessView, values, prevRow);

            string updateMessageProcedureName = GetUpdateMessageProcedureName(controller, view, pk, parameters);
            UpdateMessages(messages, updateMessageProcedureName, command);
        }

        protected virtual void UpdateMessages(Dictionary<int, string> messages, string updateMessageProcedureName, IDbCommand command)
        {
            foreach (int approvalProcessUserId in messages.Keys)
            {
                UpdateMessage(approvalProcessUserId, messages[approvalProcessUserId], updateMessageProcedureName, command);
            }
        }

        private void UpdateMessage(int approvalProcessUserId, string message, string updateMessageProcedureName, IDbCommand command)
        {
            command.Parameters.Clear();

            IDataParameter approvalProcessUserIdParameter = GetParameter("Id", approvalProcessUserId);
            IDataParameter approvalProcessUserMessageParameter = GetParameter("Message", message);
    
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = updateMessageProcedureName;

            command.Parameters.Add(approvalProcessUserIdParameter);
            command.Parameters.Add(approvalProcessUserMessageParameter);
    
            command.ExecuteNonQuery();

            command.CommandType = CommandType.Text;

        }

        protected virtual IDataParameter[] GetSpecificParameters(IApprovalProcess controller, View view, string pk, Dictionary<string, Parameter> parameters)
        {
            List<IDataParameter> dataParameters = new List<IDataParameter>();

            string prefix = "param_";

            foreach (Parameter parameter in parameters.Values)
            {
                if (parameter.Name.StartsWith(prefix))
                {
                    string dataParameterName = parameter.Name.TrimStart(prefix.ToCharArray());
                    string value = parameter.Value;
                    IDataParameter dataParameter = GetParameter(dataParameterName, value);
                    dataParameters.Add(dataParameter);
                }
            }

            return dataParameters.ToArray();
        }

        protected virtual int GetApprovalProcessType(IApprovalProcess controller, View view, string pk, Dictionary<string, Parameter> parameters)
        {
            string parameterName = "ApprovalProcessType";

            if (parameters.ContainsKey(parameterName))
            {
                object parameter = parameters[parameterName].Value;
                if (parameter == null)
                    throw new WorkflowEngineException("The approval process type parameter " + parameterName + " has no value. Please enter a value to the parameter.");

                int approvalProcessType = 0;
                if (int.TryParse(parameter.ToString(), out approvalProcessType))
                    return approvalProcessType;
                else
                    throw new WorkflowEngineException("The approval process type parameter " + parameterName + " has an invalid value. This value must be an integer. Please enter a value to the parameter.");

            }
            else
            {
                throw new WorkflowEngineException("The approval process type parameter " + parameterName + " is missing. Please add this parameter to the rule.");
            }
        }

        protected virtual string GetStoredProcedureName(IApprovalProcess controller, View view, string pk, Dictionary<string, Parameter> parameters)
        {
            string parameterName = "StoredProcedureName";
            
            if (parameters.ContainsKey(parameterName))
            {
                return parameters[parameterName].Value;
            }

            return controller.GetApprovalProcessStoredProcedureName(view);
        }

        protected virtual string GetUpdateMessageProcedureName(IApprovalProcess controller, View view, string pk, Dictionary<string, Parameter> parameters)
        {
            string parameterName = "UpdateMessageProcedureName";

            if (parameters.ContainsKey(parameterName))
            {
                return parameters[parameterName].Value;
            }

            return "durados_UpdateApprovalProcessMessage";
        }

        protected virtual string GetParentView(IApprovalProcess controller, View view, string pk, Dictionary<string, Parameter> parameters)
        {
            string parameterName = "ParentView";

            if (parameters.ContainsKey(parameterName))
            {
                return parameters[parameterName].Value;
            }

            return view.Name;
        }

        protected virtual string GetMessageKey(IApprovalProcess controller, View view, string pk, Dictionary<string, Parameter> parameters)
        {
            string parameterName = "ApprovalProcessMessageKey";

            if (parameters.ContainsKey(parameterName))
            {
                return parameters[parameterName].Value;
            }

            return controller.GetApprovalProcessMessageKey(view, pk);
        }

        protected virtual string GetSubjectKey(IApprovalProcess controller, View view, string pk, Dictionary<string, Parameter> parameters)
        {
            string parameterName = "ApprovalProcessSubjectKey";

            if (parameters.ContainsKey(parameterName))
            {
                return parameters[parameterName].Value;
            }

            return controller.GetApprovalProcessSubjectKey(view, pk);
        }

        protected virtual View GetApprovalProcessView(IApprovalProcess controller, View view, string pk, Dictionary<string, Parameter> parameters)
        {
            string parameterName = "ApprovalProcessViewName";
            string approvalProcessViewName = null;

            if (parameters.ContainsKey(parameterName))
            {
                approvalProcessViewName = parameters[parameterName].Value;

    
                if (!view.Database.Views.ContainsKey(approvalProcessViewName))
                {
                    throw new WorkflowEngineException("The database does not contains the view " + approvalProcessViewName + " as the Approval Process View. Please check the Rule parameter ApprovalProcessViewName.");
                }

                return view.Database.Views[approvalProcessViewName];
            }

            return controller.GetApprovalProcessView(view);
        }

        protected virtual Dictionary<int, string> Notify(IApprovalProcess controller, Dictionary<string, Parameter> parameters, int approvalProcessId, string pk, View view, Dictionary<string, object> values, DataRow prevRow)
        {
            string messageKey = GetMessageKey(controller, view, pk, parameters);
            string subjectKey = GetSubjectKey(controller, view, pk, parameters);
            return Notify(controller, messageKey, subjectKey, approvalProcessId, parameters, view, values, prevRow, pk);
            
        }

        protected virtual Dictionary<int, string> Notify(IApprovalProcess controller, string messageKey, string subjectKey, int id, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk)
        {
            ChildrenField childrenField = GetApprovalProcessUsersChildrenField(controller, view);

            DataView approvalProcessUserDataView = GetApprovalProcessUserDataView(id, childrenField, controller);
            if (approvalProcessUserDataView == null)
            {
                throw new WorkflowEngineException("Could not load the Approval Process Users view .");
            }

            Dictionary<int, string> messages = new Dictionary<int, string>();

            string siteWithoutQueryString = ((INotifier)controller).GetSiteWithoutQueryString();

            int currentUserId = controller.GetCurrentUserId();

            foreach (System.Data.DataRowView row in approvalProcessUserDataView)
            {
                int userId = (int)row.Row["UserId"];
                int approvalProcessUserId = (int)row.Row["Id"];
                string subject = controller.GetSubject((View)childrenField.ChildrenView, subjectKey, approvalProcessUserId);
                string message = controller.GetMessage((View)childrenField.ChildrenView, messageKey, approvalProcessUserId);

                string urlAction = ((INotifier)controller).GetUrlAction(childrenField.ChildrenView, approvalProcessUserId.ToString());
                string to = string.Empty;
                string name = string.Empty;
                controller.GetEmailInfo(userId.ToString(), out to, out name);
                messages.Add(approvalProcessUserId, message);
                SendEmail(controller, to, string.Empty, message, subject);
                string messageBoardId = ((INotifier)controller).SaveInMessageBoard(parameters, childrenField.ChildrenView, values, prevRow, approvalProcessUserId.ToString(), siteWithoutQueryString, urlAction, subject, message, currentUserId, null, new Dictionary<int,bool>());
                ((INotifier)controller).SaveMessageAction(view, messageBoardId, "True", 5, userId); // Required Action

            }

            return messages;
        }

        protected virtual void SendEmail(IApprovalProcess controller, string to, string cc, string message, string subject)
        {
            controller.SendEmail(controller.GetDefaultFrom(), to, cc, message, subject, null);
        }


        protected virtual DataView GetApprovalProcessUserDataView(int id, ChildrenField childrenField, IApprovalProcess controller)
        {
            return controller.GetDataView(childrenField, id.ToString());
        }

        protected virtual ChildrenField GetApprovalProcessUsersChildrenField(IApprovalProcess controller, View view)
        {
            ChildrenField usersChildrenField = controller.GetApprovalProcessUsersChildrenField(view);
            if (usersChildrenField != null)
                return usersChildrenField;
            string approvalProcessUserViewName = GetApprovalProcessUserViewName(controller);

            usersChildrenField = (ChildrenField)view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).ChildrenView.Name == approvalProcessUserViewName).FirstOrDefault();

            if (usersChildrenField == null)
            {
                throw new WorkflowEngineException("Could not found the Approval Process Users field on view " + view.Name + " that connect to the Approval Process Users View by the name " + approvalProcessUserViewName + ".");
            }

            return usersChildrenField;
        }

        private string GetApprovalProcessUserViewName(IApprovalProcess controller)
        {
            return controller.GetApprovalProcessUserViewName();
        }

        protected virtual int? Create(IApprovalProcess controller, View view, string pk, View approvalProcessView, string storedProcedureName, string parentView, int approvalProcessType, IDataParameter[] specificParameters, IDbCommand command)
        {
            command.Parameters.Clear();

            int? approvalProcessId = -1;
            IDataParameter approvalProcessIdParameter = GetParameter("Id", approvalProcessId.Value);

            approvalProcessIdParameter.Direction = ParameterDirection.Output;
            IDataParameter approvalProcessTypeParameter = GetParameter("ApprovalProcessTypeId", approvalProcessType);
            IDataParameter pkParameter = null;
            if (view.DataTable.PrimaryKey.Count() == 1 && view.DataTable.PrimaryKey[0].DataType == typeof(int))
            {
                pkParameter = GetParameter("pk", Convert.ToInt32(pk));
            }
            else
            {
                pkParameter = GetParameter("pk", pk);
            }
            IDataParameter parentViewParameter = GetParameter("ParentView", parentView);
            
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;

            command.Parameters.Add(approvalProcessIdParameter);
            command.Parameters.Add(approvalProcessTypeParameter);
            command.Parameters.Add(pkParameter);
            command.Parameters.Add(parentViewParameter);
            foreach (IDataParameter parameter in specificParameters)
            {
                command.Parameters.Add(parameter);
            }

            command.ExecuteNonQuery();

            command.CommandType = CommandType.Text;

            if (approvalProcessIdParameter.Value != DBNull.Value)
                approvalProcessId = Convert.ToInt32(approvalProcessIdParameter.Value);

            return approvalProcessId;
        }

        protected virtual IDataParameter GetParameter(string name, object value)
        {
            return new System.Data.SqlClient.SqlParameter(name, value);
        }

        
        public virtual int? Complete(object controller, int approvalProcessId, int approvalProcessUserId, string approvalProcessViewName, string statusFieldName, string storedProcedureName, IDbCommand command)
        {
            command.Parameters.Clear();
            int? approvalStatusId = -1;
            
            IDataParameter approvalProcessUserIdParameter = GetParameter("@ApprovalProcessUserId", approvalProcessUserId);
            IDataParameter approvalProcessIdParameter = GetParameter("@ApprovalProcessId", approvalProcessId);
            IDataParameter approvalStatusIdParameter = GetParameter("@ApprovalStatusId", approvalStatusId.Value);
            IDataParameter approvalProcessViewNameParameter = GetParameter("@ApprovalProcessViewName", approvalProcessViewName);
            IDataParameter statusFieldNameParameter = GetParameter("@StatusFieldName", statusFieldName);
            approvalStatusIdParameter.Direction = ParameterDirection.Output;

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;

            command.Parameters.Add(approvalProcessUserIdParameter);
            command.Parameters.Add(approvalProcessIdParameter);
            command.Parameters.Add(approvalStatusIdParameter);
            command.Parameters.Add(approvalProcessViewNameParameter);
            command.Parameters.Add(statusFieldNameParameter);
            
            command.ExecuteNonQuery();

            command.CommandType = CommandType.Text;

            if (approvalStatusIdParameter.Value != DBNull.Value && !approvalStatusIdParameter.Value.Equals(approvalStatusId.Value))
                approvalStatusId = Convert.ToInt32(approvalStatusIdParameter.Value);

            return approvalStatusId;
        }
    }

    public interface IApprovalProcess
    {
        Dictionary<string, object> GetBlocksValues(string id, View view);
        DataRow GetDataRow(Durados.View view, string pk);

        ChildrenField GetApprovalProcessUsersChildrenField(View view);

        string GetApprovalProcessUserViewName();

        DataView GetDataView(ChildrenField childrenField, string pk);

        Notifier GetNotifier();

        void SendEmail(string from, string to, string cc, string message, string subject, string[] attachments);

        string GetDefaultFrom();

        void GetEmailInfo(string userPk, out string email, out string fullname);


        View GetApprovalProcessView(View view);

        string GetApprovalProcessStoredProcedureName(View view);

        string GetMessage(Durados.View view, string messageKey, int id);

        string GetSubject(Durados.View view, string subjectKey, int id);

        string GetApprovalProcessMessageKey(View view, string pk);

        string GetApprovalProcessSubjectKey(View view, string pk);

        int GetCurrentUserId();
    }

    
}
