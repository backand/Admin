using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Durados.Workflow
{
    public class Validator
    {

        public string GetMessage(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, LogicalParser parser, out bool hasErrors,string pk,int currentUserId,string  currentUserRole)
        {
            string message = string.Empty;
            string prefix = string.Empty;
            string postfix = string.Empty;
            List<string> expressions = parameters.Keys.ToList();
            Dictionary<string, string> displayNames = new Dictionary<string, string>();

            if (values != null)
            {
                foreach (Field field in view.Fields.Values)
                {
                    displayNames.Add(field.Name, field.DisplayName);
                }
            }

            if (parameters.ContainsKey("prefix"))
            {
                prefix = parameters["prefix"].Value;
                expressions.Remove("prefix");
            }

            if (parameters.ContainsKey("postfix"))
            {
                postfix = parameters["postfix"].Value;
                expressions.Remove("postfix");
            }

            hasErrors = false;

            if (controller is Durados.Workflow.INotifier)
            {
                foreach (string expression in expressions)
                {
                    string template = expression.ReplaceAllTokens(view, values, pk, currentUserId.ToString(), view.Database.GetCurrentUsername(), currentUserRole, prevRow);
                    template = template.Replace(Engine.AsToken(values), ((Durados.Workflow.INotifier)controller).GetTableViewer(), view);
                    if (!parser.Check(template.ReplaceWithoutPrefix(view, values, prevRow)))
                    {
                        hasErrors = true;
                        message += parameters[expression].Value.Replace(displayNames);
                    }
                }
            }

            message = prefix + message + postfix;

            return message;
        }

        public void Validate(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, LogicalParser parser,string pk,int currentUserId, string currentUserRole)
        {
            //string message = string.Empty;
            //string prefix = string.Empty;
            //string postfix = string.Empty;
            //List<string> expressions = parameters.Keys.ToList();
            //Dictionary<string, string> displayNames = new Dictionary<string,string>();

            //foreach (string fieldName in values.Keys)
            //{
            //    if (view.Fields.ContainsKey(fieldName))
            //    {
            //        displayNames.Add(fieldName, view.Fields[fieldName].DisplayName);
            //    }
            //}

            //bool hasErrors = false;

            //if (parameters.ContainsKey("prefix"))
            //{
            //    prefix = parameters["prefix"];
            //    expressions.Remove("prefix");
            //}

            //if (parameters.ContainsKey("postfix"))
            //{
            //    postfix = parameters["postfix"];
            //    expressions.Remove("postfix");
            //}


            //foreach (string expression in expressions)
            //{
            //    if (!parser.Check(expression.Replace(values)))
            //    {
            //        hasErrors = true;
            //        message += parameters[expression].Replace(displayNames) + "<br>";
            //    }
            //}

            //message = prefix + "<br>" + message + "<br>" + postfix;

            bool hasErrors = false;
            string message = GetMessage(controller, parameters, view, values, prevRow, parser, out hasErrors,pk,currentUserId, currentUserRole);
            if (hasErrors)
            {
                throw new WorkflowEngineException(message);
            }
        }
    }
}
