using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Durados.Workflow
{
    public class WebService
    {
        public virtual void Call(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole, IDbCommand command)
        {
            //List<string> expressions = parameters.Keys.ToList();

            //foreach (string expression in expressions)
            //{
            //    try
            //    {
            //        Dictionary<string, object> valuesCopy = new Dictionary<string, object>();

            //        string webService = expression.ReplaceWithDollar(view, values).Replace("[pk]", pk).Replace(Durados.Database.UserPlaceHolder, currentUsetId.ToString(), false).Replace(Durados.Database.RolePlaceHolder, currentUserRole, false);

            //        CallWebService(webService);
            //    }
            //    catch (Exception exception)
            //    {
            //        throw new WorkflowEngineException(parameters[expression].Value ?? exception.Message);
            //    }
            //}

            try
            {
                string url = null;
                if (parameters.ContainsKey("URL"))
                {
                    url = parameters["URL"].Value;
                }

                else if (parameters.ContainsKey("Url"))
                {
                    url = parameters["Url"].Value;
                }

                else if (parameters.ContainsKey("url"))
                {
                    url = parameters["url"].Value;
                }
                else
                {
                    throw new WorkflowEngineException("The parameter URL is missing in the web service workflow rule");
                }

                if (!parameters.ContainsKey("Type") || parameters["Type"].Value.ToLower() == "webservice")
                {
                    //url = url.ReplaceWithDollar(view, values).Replace("[pk]", pk).Replace(Durados.Database.UserPlaceHolder, currentUsetId.ToString(), false).Replace(Durados.Database.RolePlaceHolder, currentUserRole, false);
                    url = url.ReplaceAllTokens(view, values, pk,currentUsetId.ToString(),view.Database.GetCurrentUsername(),currentUserRole, prevRow);
                    url = url.Replace(Engine.AsToken(values), ((Durados.Workflow.INotifier)controller).GetTableViewer(), view);
                }

                CallWebService(url);
            }
            catch (Exception exception)
            {
                if (parameters.ContainsKey("ErrorMessage"))
                    throw new WorkflowEngineException(parameters["ErrorMessage"].Value);
                else
                    throw new WorkflowEngineException(exception.Message);
            }

        }

        protected virtual string CallWebService(string webService)
        {
            return null;
        }
        
    }
}
