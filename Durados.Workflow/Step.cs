using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Durados.Workflow
{
    public class Step
    {
        public static readonly string PreviousStep = "Durados_PrevStep";

        public virtual Result Complete(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole, IDbCommand command, Engine engine)
        {
            Result result = new Result();
            Dictionary<string, EnabledStep> enabledSteps = new Dictionary<string, EnabledStep>();
            
            ParentField parentField = (ParentField)view.WorkFlowStepsField;
            View parentView = parentField.ParentView;
            DataTable dataTable = ((ILoader)controller).GetDataTable(parentView);
            result.PK = pk;
            Field ordinalField = null;
            result.Description = ((ILoader)controller).GetTooltip(view.DisplayName + " Steps", parentField.Description);

            if (!string.IsNullOrEmpty(parentView.OrdinalColumnName))
            {
                if (parentView.Fields.ContainsKey(parentView.OrdinalColumnName))
                {
                    ordinalField = parentView.Fields[parentView.OrdinalColumnName];
                }
            }

            string workflowCompletedMessage = string.Empty;
            string workflowNoResultMessage = string.Empty;

            bool messageOnly = false;

            result.Message = view.Database.DefaultStepMessage;
            result.WorkflowCompletedMessage = view.Database.DefaultWorkFlowCompletedMessage;
            if (values.ContainsKey(parentField.Name))
                result.CurrentStepId = values[parentField.Name].ToString();
            else
                result.CurrentStepId = parentField.GetValue(prevRow);

            foreach (string key in parameters.Keys)
            {
                if (key.ToLower() == "message")
                {
                    result.Message = parameters[key].Value;
                }
                else if (key.ToLower() == "workflowcompletedmessage")
                {
                    workflowCompletedMessage = parameters[key].Value;
                }
                else if (key.ToLower() == "workflownoresultmessage")
                {
                    workflowNoResultMessage = parameters[key].Value;
                }
                else if (key.ToLower() == "messageonly")
                {
                    messageOnly = parameters[key].Value.ToLower() != "false" && parameters[key].Value.ToLower() != "0";
                }
                else
                {
                    Rule stepRule = new Rule();
                    stepRule.WhereCondition = key;
                    string[] ids = parameters[key].Value.Split('~');
                    bool enabled = engine.Check(view, stepRule, TriggerDataAction.BeforeCompleteStep, values, pk, prevRow, parameters[key].UseSqlParser, connectionString, currentUsetId, currentUserRole);

                    if (enabled)
                    {
                        foreach (string id2 in ids)
                        {
                            string id = id2;
                            if (id.Equals(Step.PreviousStep))
                            {
                                id = GetPrevStep((ILoader)controller, view, pk, result.CurrentStepId);
                            }
                            if (id != null)
                            {
                                DataRow dataRow = dataTable.Rows.Find(parentView.GetPkValue(id));
                                if (dataRow != null)
                                {
                                    string name = parentView.GetDisplayValue(dataRow);
                                    int ordinal = 0;
                                    if (ordinalField != null)
                                    {
                                        string s = ordinalField.GetValue(dataRow);
                                        if (!string.IsNullOrEmpty(s))
                                            ordinal = Convert.ToInt32(s);
                                    }
                                    string description = string.Empty;
                                    if (dataTable.Columns.Contains("Description"))
                                        description = ((ILoader)controller).GetTooltip(name, dataRow["Description"].ToString());
                                    if (!enabledSteps.ContainsKey(id))
                                    {
                                        enabledSteps.Add(id, new EnabledStep() { Enable = enabled, Id = id, Name = name, Ordinal = ordinal, Description = description });
                                    }
                                }
                            }
                        }
                    }
                }

            }

            foreach (DataRow row in dataTable.Rows)
            {
                string id = parentView.GetPkValue(row);
                if (!enabledSteps.ContainsKey(id))
                {
                    string name = parentView.GetDisplayValue(row);
                    int ordinal = 0;
                    if (ordinalField != null)
                    {
                        string s = ordinalField.GetValue(row);
                        if (!string.IsNullOrEmpty(s))
                            ordinal = Convert.ToInt32(s);
                    }
                    string description = string.Empty;
                    if (dataTable.Columns.Contains("Description"))
                        description = row["Description"].ToString();
                        
                    enabledSteps.Add(id, new EnabledStep() { Enable = false, Id = id, Name = name, Ordinal = ordinal, Description=description });
                }
            }

            result.EnabledSteps = enabledSteps.Values.OrderBy(es => es.Ordinal).ToArray();

            int enabledStepsCount = result.EnabledSteps.Where(es => es.Enable).Count();
            result.MessageOnly = enabledStepsCount == 0 || (enabledStepsCount == 1 && messageOnly);

            if (enabledStepsCount == 0)
            {
                result.WorkflowCompletedMessage = workflowNoResultMessage;
            }
            else
            {
                result.WorkflowCompletedMessage = workflowCompletedMessage;
            }

            return result;
        }

        protected virtual string GetPrevStep(ILoader loader, View view, string pk, string currentStepId)
        {
            return loader.GetPreviousValue(view.Base.Name, view.WorkFlowStepsFieldName, pk, currentStepId);
        }

        public class Result
        {
            public Result()
            {
                IsResult = true;
            }
            public bool IsResult { get; set; }
            public string PK { get; set; }
            public string Message { get; set; }
            public string WorkflowCompletedMessage { get; set; }
            public string CurrentStepId { get; set; }
            public EnabledStep[] EnabledSteps { get; set; }
            public string Description { get; set; }
            public bool MessageOnly { get; set; }
            
        }

        public class EnabledStep
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Enable { get; set; }
            public int Ordinal { get; set; }
        }
    }

    public interface ILoader
    {
        DataTable GetDataTable(Durados.View view);
        string GetTooltip(string title, string description);
        string GetPreviousValue(string viewName, string fieldName, string pk, string currentValue);
        
    }
}
