using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Durados.Workflow
{
    public class Executer
    {
        protected virtual IDbCommand GetCommand(Database database)
        {
            foreach (View view in database.Views.Values)
            {
                if (!view.SystemView)
                {
                    return view.GetCommand();
                }
            }
            return null;
        }
        #region consts

        private const string IN_PARAMETER = "in";
        private const string OUT_PARAMETER = "out";

        #endregion
        public virtual void Execute(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole, IDbCommand command)
        {
            IDbCommand spCommand = null;
            if (view.SystemView)
            {
                //spCommand = view.GetCommand();
                //spCommand.Connection.Open();
                spCommand = GetCommand(view.Database);
                if (spCommand == null)
                    throw new DuradosException("could not create command for sp execution");
                spCommand.Connection.Open();
            }
            else
            {
                spCommand = command;
            }

            List<string> expressions = parameters.Keys.ToList();
            string[] inParameters;
            string[] splitParameter;
            SqlParameter param;

            //use spCommand type =text
            if (!expressions.Exists(e => e == IN_PARAMETER || e == OUT_PARAMETER))
            {
                foreach (string expression in expressions)
                {
                    try
                    {
                        spCommand.Parameters.Clear();

                        Dictionary<string, object> valuesCopy = new Dictionary<string, object>();

                        foreach (string key in values.Keys)
                        {
                            valuesCopy.Add(key, values[key]);
                        }

                        //foreach (Field field in view.Fields.Values)
                        //{
                        //    if (!valuesCopy.ContainsKey(field.Name))
                        //    {
                        //        object val = field.GetValue(prevRow);
                        //        valuesCopy.Add(field.Name, val);
                        //    }
                        //}
                        try
                        {
                            EscapeApostrophe(valuesCopy);
                        }
                        catch { }
                        spCommand.CommandText = ReplaceAllTokens(view, valuesCopy, prevRow, pk, currentUsetId, currentUserRole, expression);
                        spCommand.CommandText = spCommand.CommandText.Replace(Engine.AsToken(valuesCopy), ((Durados.Workflow.INotifier)controller).GetTableViewer(), view);
                
                        string[] filter = ((IExecuter)controller).GetFilterFieldValue(view);

                        if (filter != null && filter.Length == 2)
                        {
                            spCommand.CommandText = spCommand.CommandText.Replace("[filter_" + filter[0] + "]", filter[1]);
                        }

                        if (spCommand.CommandText.Contains("[filter_"))
                        {
                            return;
                        }

                        if (spCommand.CommandText.Contains('$') && prevRow != null)
                        {
                            spCommand.CommandText = spCommand.CommandText.ReplaceWithDollar(view, values, prevRow);
                        }

                        bool commandWasClosed = spCommand.Connection.State == ConnectionState.Closed;
                        if (commandWasClosed)
                        {
                            spCommand.Connection.ConnectionString = connectionString;
                            spCommand.Connection.Open();
                        }
                        try
                        {
                            spCommand.ExecuteNonQuery();
                        }
                        finally
                        {
                            if (commandWasClosed)
                                spCommand.Connection.Close();
                            if (view.SystemView)
                            {
                                if (spCommand.Connection.State == ConnectionState.Open)
                                    spCommand.Connection.Close();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new SqlExecuteException((string.IsNullOrEmpty(parameters[expression].Value) ? "Failed to run: " + spCommand.CommandText + "; \nError: " + exception.Message : parameters[expression].Value));
                    }
                }
            }
            else
            {
                spCommand.Parameters.Clear();
                spCommand.CommandType = CommandType.StoredProcedure;
                spCommand.CommandText = parameters.FirstOrDefault().Value.Name;
                //add in parameters
                if (expressions.Contains(IN_PARAMETER))
                {
                    inParameters = parameters[IN_PARAMETER].Value.Split(',');
                    foreach (string parameter in inParameters)
                    {
                        splitParameter = parameter.Split(':');
                        var key = splitParameter[1].Replace("'", "").Replace("$", "");
                        param = new SqlParameter(splitParameter[0], GetDbTypeofDataType((DataType)view.Fields[key].FieldType));
                        string value = splitParameter[1].ReplaceWithDollar(view, values);
                        if (value != string.Empty)
                            param.Value = value;
                        else
                            param.Value = DBNull.Value;
                        spCommand.Parameters.Add(param);

                    }
                }
                //add out parameters
                if (expressions.Contains(OUT_PARAMETER))
                {
                    inParameters = parameters[OUT_PARAMETER].Value.Split(',');

                    foreach (string parameter in inParameters)
                    {
                        splitParameter = parameter.Split(':');
                        var key = splitParameter[1].Replace("'", "").Replace("$", "");
                        Field field = view.Fields[key];
                        string type = GetDbTypeofDataType((DataType)field.FieldType);
                        param = new SqlParameter(splitParameter[0], type)
                        {
                            Direction = ParameterDirection.InputOutput
                        };
                        if (type == "nvarchar(500)" || type == "nvarchar(max)")
                        {
                            param.Size = 500;
                        }
                        spCommand.Parameters.Add(param);
                    }
                }
                bool commandWasClosed = spCommand.Connection.State == ConnectionState.Closed;
                if (commandWasClosed)
                {
                    spCommand.Connection.ConnectionString = connectionString;
                    spCommand.Connection.Open();
                }
                try
                {
                    spCommand.ExecuteNonQuery();
                }
                finally
                {
                    if (commandWasClosed)
                        spCommand.Connection.Close();
                    if (view.SystemView)
                    {
                        if (spCommand.Connection.State == ConnectionState.Open)
                            spCommand.Connection.Close();
                    }
                }
                //fill data with out parameters
                inParameters = parameters[OUT_PARAMETER].Value.Split(',');
                foreach (string parameter in inParameters)
                {
                    splitParameter = parameter.Split(':');
                    var key = splitParameter[1].Replace("'", "").Replace("$", "");
                    values[key] = ((SqlParameter)(spCommand.Parameters[splitParameter[0]])).Value;
                }

            }
        }

        private void EscapeApostrophe(Dictionary<string, object> values)
        {
            if (values == null || values.Count == 0)
                return;
            List<string> keys = new List<string>();

            foreach (string key in values.Keys)
            {
                if (values[key] is string && values[key].ToString().Contains('\''))
                    keys.Add(key);
            }

            foreach (string key in keys)
            {
                values[key] = values[key].ToString().EscapeApostrophe();
            }
        }

        

        private static string ReplaceAllTokens(View view, Dictionary<string, object> values, DataRow prevRow, string pk, int currentUsetId, string currentUserRole, string expression)
        {
            return expression.ReplaceAllTokens(view, values, pk, currentUsetId.ToString(), view.Database.GetCurrentUsername(), currentUserRole, prevRow);
               
        }

        public string GetDbTypeofDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    return "bit";

                case DataType.DateTime:
                    return "datetime";

                case DataType.LongText:
                    return "nvarchar(max)";

                case DataType.Numeric:
                    return "float";

                case DataType.Image:
                case DataType.Url:
                case DataType.Email:
                case DataType.Html:
                case DataType.ShortText:
                    return "nvarchar(500)";

            }
            return string.Empty;
        }
    }

    public interface IExecuter
    {
        string[] GetFilterFieldValue(Durados.View view);
    }
}