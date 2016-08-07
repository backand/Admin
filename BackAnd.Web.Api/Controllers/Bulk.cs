using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Web.Mvc;
using Durados.DataAccess;
using Durados;
using System.Web.Script.Serialization;

namespace BackAnd.Web.Api.Controllers
{
    class Bulk
    {
        internal object Run(Dictionary<string, object>[] requests, Map map, Durados.Data.IDataHandler DataHandler)
        {
            IDbConnection connection = null;
            IDbConnection sysConnection = null;

            try
            {
                connection = SqlAccess.GetNewConnection(map.Database.SqlProduct);
                IDbCommand command = GetSqlAccess(map.Database.SqlProduct).GetNewCommand();

                connection.ConnectionString = map.Database.ConnectionString;
                connection.Open();
                command.Connection = connection;


                sysConnection = SqlAccess.GetNewConnection(map.Database.SystemSqlProduct, map.Database.SystemConnectionString);
                IDbCommand sysCommand = GetSqlAccess(map.Database.SystemSqlProduct).GetNewCommand();
                sysConnection.Open();
                sysCommand.Connection = sysConnection;

                var responses = new List<object>();
                JavaScriptSerializer jss = new JavaScriptSerializer();
            
                foreach (Dictionary<string, object> request in requests)
                {
                    try
                    {
                        string method = GetMethod(request);
                        string objectName = GetObjectName(request);
                        string json = GetJson(request, jss);
                        bool? deep = GetDeep(request);
                        bool? returnObject = GetReturnObject(request);
                        string parameters = GetParameters(request, jss);
                        string id = null;

                        switch (method)
                        {


                            case "POST":
                                try
                                {
                                    string s = DataHandler.Post(objectName, json, command, sysCommand, deep, returnObject, parameters);
                                    object response = jss.DeserializeObject(s);
                                    responses.Add(response);
                                }
                                catch (Exception exception)
                                {
                                    responses.Add(new { error = exception.Message });
                                }
                                break;

                            case "PUT":
                                bool? overwrite = GetOverwrite(request);
                                id = GetId(request);
                                    
                                try
                                {
                                    string s = DataHandler.Put(objectName, id, json, command, sysCommand, deep, returnObject, parameters, overwrite);
                                    object response = jss.DeserializeObject(s);
                                    responses.Add(response);
                                }
                                catch (Exception exception)
                                {
                                    responses.Add(new { error = exception.Message });
                                }
                                break;

                            case "DELETE":

                                id = GetId(request);
                                try
                                {
                                    string s = DataHandler.Delete(objectName, id, command, sysCommand, deep, parameters);
                                    object response = jss.DeserializeObject(s);
                                    responses.Add(response);
                                }
                                catch (Exception exception)
                                {
                                    responses.Add(new { error = exception.Message });
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        responses.Add(new { error = ex.Message });
                    }

                }

                return responses.ToArray();
            }
            finally
            {
                if (connection != null)
                {
                    try
                    {
                        connection.Close();
                    }
                    catch { }
                }
                if (sysConnection != null)
                {
                    try
                    {
                        sysConnection.Close();
                    }
                    catch { }
                }
            }
        }

        private bool? GetOverwrite(Dictionary<string, object> request)
        {
            return null;
        }

        private string GetId(Dictionary<string, object> request)
        {
            string url = (string)request["url"];
            return new Uri(url).Segments.LastOrDefault();
        }

        private string GetMethod(Dictionary<string, object> request)
        {
            string method = "GET";
            if (request.ContainsKey("method"))
            {
                method = (string)request["method"];
            }
            return method;
        }

        private string GetParameters(Dictionary<string, object> request, JavaScriptSerializer jss)
        {
            Dictionary<string, object> parameters = null;
            if (request.ContainsKey("parameters"))
            {
                parameters = (Dictionary<string, object>)request["parameters"];
            }
            else
            {
                return null;
            }
            return jss.Serialize(parameters);
        }

        private bool? GetReturnObject(Dictionary<string, object> request)
        {
            return null;
        }

        private bool? GetDeep(Dictionary<string, object> request)
        {
            return null;
        }

        private string GetJson(Dictionary<string, object> request, JavaScriptSerializer jss)
        {
            string data = null;
            if (request.ContainsKey("data"))
            {
                data = jss.Serialize((Dictionary<string, object>)request["data"]);
            }

            return data;
        }

        private string GetObjectName(Dictionary<string, object> request)
        {
            string url = (string)request["url"];
            string[] ar = url.Split('/');
            for (int i = 0; i < ar.Length; i++)
            {
                if (ar[i].ToLower() == "objects")
                {
                    return ar[i + 1];
                }
            }

            return null;
        }

        private SqlAccess GetSqlAccess(SqlProduct sqlProduct)
        {

            switch (sqlProduct)
            {
                case SqlProduct.MySql:
                    return new MySqlAccess();
                    break;
                case SqlProduct.Postgre:
                    return new PostgreAccess();
                    break;
                case SqlProduct.Oracle:
                    return new OracleAccess();
                    break;

                default:
                    return new SqlAccess();
                    break;
            }


        }
    }
}
