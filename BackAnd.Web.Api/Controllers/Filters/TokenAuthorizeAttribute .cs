using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Net;
using Durados.Web.Mvc;

namespace BackAnd.Web.Api.Controllers.Filters
{
    public class TokenAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        public TokenAuthorizeAttribute(HeaderToken headerToken)
            : base()
        {
            HeaderToken = headerToken;
        }
        public HeaderToken HeaderToken { get; private set; }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string token = GetToken(actionContext.Request, HeaderToken.ToString());
            if (!string.IsNullOrEmpty(token) && token != "null" && IsGuid(token))
            {

                string appName = GetAppName(token);

                if (!string.IsNullOrEmpty(appName))
                {
                    System.Web.HttpContext.Current.Items.Add(Database.AppName, appName);

                    if (!System.Web.HttpContext.Current.Items.Contains(Database.RequestId))
                        System.Web.HttpContext.Current.Items.Add(Database.RequestId, Guid.NewGuid().ToString());
                    NewRelic.Api.Agent.NewRelic.AddCustomParameter(Database.RequestId, System.Web.HttpContext.Current.Items[Database.RequestId].ToString());
                    return;
                }
            }

            actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        new MissingOrIncorrectSignUpToken());

        }

        private bool IsGuid(string token)
        {
            Guid guid;
            return Guid.TryParse(token, out guid);
        }

        protected virtual string GetAppName(string token)
        {
            string sql = string.Format("SELECT [Name] FROM [durados_app] WITH(NOLOCK)  WHERE [{0}] = @token", HeaderToken.ToString());
            using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(Maps.Instance.DuradosMap.Database.ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, cnn))
                {

                    command.Parameters.AddWithValue("token", token);
                    cnn.Open();
                    object scalar = command.ExecuteScalar();
                    if (scalar == null || scalar == DBNull.Value)
                        return null;
                    return scalar.ToString();
                }
            }
        }

        protected virtual string GetToken(HttpRequestMessage request,  string name)
        {
            var headers = request.Headers;

            if (headers.Contains(name))
            {
                return headers.GetValues(name).FirstOrDefault();
            }

            return null;
        }
    }


    public enum HeaderToken
    {
        SignUpToken
    }
}