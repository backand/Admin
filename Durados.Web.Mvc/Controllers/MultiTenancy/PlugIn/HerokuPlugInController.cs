using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using System.Text;
using System.Security.Cryptography;
using System.IO;

using Durados.Web.Infrastructure;
using Durados.Web.Localization;
using Durados.Services;

using Durados.Web.Mvc.UI.Json;

using System.Data.SqlClient;
using Durados.Web.Mvc.Controllers;
using System.Configuration;

using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.Controllers
{
    public class ConnectionParameter
    {
        public string serverName { get; set; }
        public string catalog { get; set; }
        public string dbUsername { get; set; }
        public string dbPassword { get; set; }
        public int port { get; set; }
        public SqlProduct productId { get; set; }
        public bool integratedSecurity { get; set; }
        public string connectionString { get; set; }
        public bool ssl { get; set; }
        public ConnectionParameter()
        {
            ssl = false;
            serverName = "";
            catalog = "";
            port = 0;
            integratedSecurity = false;
            connectionString = "";
        }
    } ;

    /** public enum SqlProduct
    {
        SqlServer = 1,
        SqlAzure = 2,
        MySql = 3,
        Postgre = 4
    }
     * 
     * <db type>://<username>:<password>@<database server host>:<port>/<database name>
     */
    public static class ConnectionBuilder
    {
        public static ConnectionParameter GetConnectionParam(string connectionString)
        {
            ConnectionParameter oConnectionParameter = new ConnectionParameter();
            try
            {
                /* <db type>://<username>:<password>@<database server host>:<port>/<database name>
                 * "{\"db\":\"SqlAzure://itay:Back2013@.\\\\SQL2012:0/durados_AppSys_1214\",\"resource_id\":\"307\",\"app_name\":\"backand-qatest\",\"token\":\"backand\",\"expires\":1398783079238}";*/

                /***Product Name and convertion into Product Id (DB)*/
                String[] oArgs = connectionString.Split(':');
                string sProd = oArgs[0].ToLower();
                if (sProd == "oracle")
                {
                    oConnectionParameter.productId = SqlProduct.Oracle;
                }
                if (sProd == "postgres")
                {
                    oConnectionParameter.productId = SqlProduct.Postgre;
                    oConnectionParameter.ssl = true;
                }
                else if (sProd == "mysql")
                {
                    oConnectionParameter.productId = SqlProduct.MySql;
                }
                else if (sProd == "sqlazure")
                {
                    oConnectionParameter.productId = SqlProduct.SqlAzure;
                }
                else if (sProd == "sqlserver")
                {
                    oConnectionParameter.productId = SqlProduct.SqlServer;
                }
             

                /***User Name, <username>
                 oArgs[1] = //itay
                 */
                oConnectionParameter.dbUsername = oArgs[1].Substring(2, oArgs[1].Length - 2);

                /** Password and Server name, <password>@<database server host>
                oArgs[2] = Back2013@.\\\\SQL2012
                */
                String[] oPassAndServer = oArgs[2].Split('@');

                oConnectionParameter.dbPassword = oPassAndServer[0];/**<password>*/
                string serverName = oPassAndServer[1];

                if (oArgs.Length == 4)/**Connection string contains port*/
                {
                    oConnectionParameter.serverName = serverName;/**<database server host>*/
                    /** Port and Catalog, <port>/<database name>
                    oArgs[3] = 0/durados_AppSys_1214
                    */
                    String[] oPortAndCatalog = oArgs[3].Split('/');
                    oConnectionParameter.port = Convert.ToInt32(oPortAndCatalog[0]);
                    oConnectionParameter.catalog = oPortAndCatalog[1];
                }
                else/**If connection string doesn't contain port*/
                {
                    /** serverName = <database server host>/<database name>*/

                    String[] oServerAndCatalog = serverName.Split('/');
                    oConnectionParameter.port = 0;
                    oConnectionParameter.serverName = oServerAndCatalog[0];
                    oConnectionParameter.catalog = oServerAndCatalog[1];
                }
            }
            catch (Exception)
            {
                new ArgumentNullException("Invalid Argument");
            }

            return oConnectionParameter;
        }
    } ;

    public class HerokuPlugInController : PlugInController
    {
        public class PluginArgs
        {
            public string db { get; set; }
            public string resource_id { get; set; }
            public string app_name { get; set; }
            public string expires { get; set; }
            public string token { get; set; }
        } ;

        public class HerokuParameterNames : ParameterNames
        {
            public string resource_id { get { return "resource_id"; } }
            public string heroku_id { get { return "heroku_id"; } }
            public string plan { get { return "plan"; } }
            public string user_id { get { return "user_id"; } }
            public string token { get { return "token"; } }
            public string is_new_user { get { return "is_new_user"; } }

            public override List<string> GetNames()
            {
                return new List<string>() { resource_id, heroku_id, plan, user_id, token, is_new_user };
            }

        }

        private static SqlAccess sqlAccess = null;
        private static SqlAccess SqlAccess
        {
            get
            {
                if (sqlAccess == null)
                    sqlAccess = new SqlAccess();

                return sqlAccess;
            }
        }

        private static AccountMembershipService membershipService = null;
        private static AccountMembershipService MembershipService
        {
            get
            {
                if (membershipService == null)
                    membershipService = new AccountMembershipService();

                return membershipService;
            }
        }

        protected override ParameterNames GetParameterNames()
        {
            return new HerokuParameterNames();
        }

        public override PlugInType PlugInType
        {
            get
            {
                return PlugInType.Heroku;
            }
        }

        /*protected override void ValidateConnectionString(string connectionString, SqlProduct sqlProduct)
        {
           ValidateConnectionString(connectionString, sqlProduct) ;
        }*/

        protected virtual ConnectionParameter ValidateConnection(string dbConnectionString, string duradosUserId, bool usesSsh = false, bool usesSsl = false, string sshRemoteHost = null, string sshUser = null, string sshPassword = null, string sshPrivateKey = null, int sshPort = 0, bool integratedSecurity = false)
        {
            /*** dbConnectionString example postgres://qvsrwdpkmcundl:Xq9Rv48RqvPjDJVlSJAXwqUZBB@ec2-54-204-31-13.compute-1.amazonaws.com:5432/d35mshek2k9rg1
            string serverName = "ec2-54-204-31-13.compute-1.amazonaws.com";
            string catalog = "d35mshek2k9rg1";
            string userName = "qvsrwdpkmcundl";
            string password = "Xq9Rv48RqvPjDJVlSJAXwqUZBB";
                */
            ConnectionParameter oConnectionParameter = new ConnectionParameter();

            oConnectionParameter = ConnectionBuilder.GetConnectionParam(dbConnectionString);

            string serverName = oConnectionParameter.serverName;
            string catalog = oConnectionParameter.catalog;
            string dbUserName = oConnectionParameter.dbUsername;
            string dbPassword = oConnectionParameter.dbPassword;
            int port = oConnectionParameter.port;
            SqlProduct productId = oConnectionParameter.productId;
            if (productId == SqlProduct.Postgre )
            {
                usesSsl = true ;
            }
            ValidateConnectionString(integratedSecurity, serverName, catalog, dbUserName, dbPassword, usesSsh, usesSsl, duradosUserId, productId, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, port);

            return oConnectionParameter;
        }

        protected override string GetPlugInUserId()
        {
            return Request["heroku_id"].ToString();
        }

        protected override int GetPlanId()
        {
            return WixPlugInHelper.GetPlanId(Request["plan"]);
        }


        protected override string GetSiteIdFromParameters()
        {
            WixInstance wixInstance = WixPlugInHelper.GetInstance(Request.QueryString["instance"]);

            return wixInstance.instanceId.ToString();

        }

        protected override string GetSiteUrlFromParameters()
        {
            try
            {
                return Server.UrlDecode(Request.QueryString["baseUrl"]);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        protected override string GetSiteInfoFromParameters()
        {
            try
            {
                return string.Format("siteTitle: {0}, siteDescription: {1}, siteKeywords: {2}, referrer: {3}, baseUrl: {4}, baseUrl: {5}", Server.UrlDecode(Request.QueryString["siteTitle"]), Server.UrlDecode(Request.QueryString["siteDescription"]), Server.UrlDecode(Request.QueryString["siteKeywords"]), Server.UrlDecode(Request.QueryString["referrer"]), Server.UrlDecode(Request.QueryString["baseUrl"]), Server.UrlDecode(Request.QueryString["url"]));
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public JsonResult Provision(string resource_id, string heroku_id, string plan)
        {
            HerokuParameters herokuParameters = new HerokuParameters();

            //check if heroku_id exists
            resource_id = GetHerokuResourceId(heroku_id);
            herokuParameters.is_new_user = false;
            if (string.IsNullOrEmpty(resource_id))
            {

                //Create new entry for heroku_id
                string instanceId = "";

                int newUser = CreateNewUser(heroku_id,false,false,true);

                Maps.Instance.DuradosMap.Database.SaveSelectedInstance(instanceId, null, PlugInType, 1, "", newUser, GetPlugInUserId());

                resource_id = GetHerokuResourceId(heroku_id);
                herokuParameters.is_new_user = true;
            }

            herokuParameters.heroku_id = heroku_id;
            herokuParameters.resource_id = resource_id;
            herokuParameters.plan = plan;

            return Json(herokuParameters);
        }
        private ActionResult SuccessResponse(int connectionId, string userName, string appName, string pluginAppName = null, bool app_exist = false)
        {
            if( app_exist == true )
            {
                
                //redirect
                var builder = new UriBuilder(Request.Url);

                string url = builder.Scheme + "://" +appName + "." + Maps.Host + ":" + builder.Port;
                return Redirect(url);
            }
            else
            {
                return View("~/Views/PlugIn/Heroku/CreateApp.aspx", new CreateAppParameter(connectionId, userName, appName, pluginAppName, app_exist));
            }

        }
        private ActionResult FailureResponse(CreateAppParameter.CODES code, string msg, int connectionId = 0, string userName = null, string appName = null, string pluginAppName = null)
        {
            return View("~/Views/PlugIn/Heroku/CreateApp.aspx", new CreateAppParameter(code, msg, connectionId, userName, appName, pluginAppName));

        }

        private int CreateNewUser(string userName, bool createInUserAppTable, bool createInCurrentAppUsersTable, bool CreateInMembership)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string role = "User";
            string password = new AccountMembershipService().GetRandomPassword(10);
            string firstName = "heroku";
            string lastName = "heroku";
            Guid guid = Guid.NewGuid();
            string sql = "INSERT INTO [durados_User] ([Username],[FirstName],[LastName],[Email],[Role],[Guid]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid); SELECT IDENT_CURRENT(N'[durados_User]') AS ID ";

            parameters.Add("@Email", userName);
            parameters.Add("@Username", userName);
            parameters.Add("@FirstName", firstName);
            parameters.Add("@LastName", lastName);
            parameters.Add("@Role", role);
            parameters.Add("@Guid", guid);

            object scalar = SqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.Database.ConnectionString, sql, parameters);
            int newUserId = Convert.ToInt32(scalar);

            if (createInUserAppTable)
            {
                parameters = new Dictionary<string, object>();
                parameters.Add("newUser", userName);
                parameters.Add("appName", Map.AppName);
                parameters.Add("role", role);
                sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, "durados_NewAppAsignment @newUser, @appName, @role", parameters, null);
            }

            if (createInCurrentAppUsersTable)
            {
                int userId = Map.Database.GetUserID(userName);

                if (userId == -1)
                {
                    throw new DuradosException("Problem with get user detalis");
                }

                PlugInHelper.AddUserToApp(Convert.ToInt32(Map.Id), userId, role);
            }

            if (CreateInMembership)
            {

                System.Web.Security.MembershipCreateStatus createStatus = (new Durados.Web.Mvc.Controllers.AccountMembershipService()).CreateUser(userName, password, userName);
                if (createStatus == System.Web.Security.MembershipCreateStatus.Success)
                {
                    System.Web.Security.Roles.AddUserToRole(userName, role);

                }
            }
            return newUserId;

        }

        private string GetHerokuResourceId(string herokuId)
        {

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters = new Dictionary<string, object>();
            parameters.Add("PlugInUserId", herokuId);

            SqlAccess sqlAccess = new SqlAccess();
            return sqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.Database.ConnectionString, "Select top 1 pii.Id From [dbo].[durados_PlugInInstance] pii with (NOLOCK) WHERE [PlugInUserId]=@PlugInUserId", parameters);
        }

        protected override string GenerateAppName(string app_name, int uid)
        {
            return app_name + "h1" ;
        }

        public ActionResult app(string state)
        {
            string json = "" ;

            try
            {
                 //this is a sample
                /**string encryptedBase64String=A9XqFaC4VY7qQeXRwBGthnXaf2bFtIaAMhaTpROibMKdGhM8NFTRIvqWjyN3S9mCPjRprQwjJ7P%2F1a6TNpQPPidE7kPx%2BGhvAEhEYV6dZN02qASI1HVvVdCooHzsFaqVixIqZSGGU8SBQu%2F2GVrPGJiMN9CyvmJBSe3YFtWzNFs0g0OjkG8K8aDmw3VqgcWalFr7CYDyU4H81rgY6DpIwdLNewAPJ5rBvchRMAOvXbgCe4DAGbFNmYEpy9I4TH8oKRMtaDoHOlvPkYKTWFGWmg%3D%3D*/

                json = Base64CryptoHelper.DecryptString(Server.UrlDecode(state).Replace(" ", "+"));/*Fix for decoding problem that causes + to be replaced with space " "*/
                if (String.IsNullOrEmpty(json))
                {
                    if (!String.IsNullOrEmpty(state))
                    {
                        json = state;
                    }
                }
            }
            catch(Exception e)
            {
                return FailureResponse(CreateAppParameter.CODES.INVALID_SECURITY_DATA, "Security failure", 0, null, null);
            }


            if (String.IsNullOrEmpty(json))
            {
                return FailureResponse(CreateAppParameter.CODES.INVALID_SECURITY_DATA, "Security failure", 0, null, null);
            }

            PluginArgs oArgs = new PluginArgs();
            try
            {
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                oArgs = (PluginArgs)jsonSerializer.Deserialize<PluginArgs>(json);  
              
                if (string.Compare(oArgs.token, "backand", false) != 0)
                {
                    return FailureResponse(CreateAppParameter.CODES.INVALID_SECURITY_DATA, "Security failure", 0, null, null);
                    //Security failure due to invalid arguments, redirect to Heroku
                }
            }
            catch(Exception e)
            {
                    //Security failure due to invalid arguments, redirect to Heroku
                return FailureResponse(CreateAppParameter.CODES.INVALID_CONNECTION_DATA, "invalid ConnectionId", 0, null, null);
            }

            //pass security token then sign the user GetUsernameByUserId(uid);
            //oArgs.resource_id = "404";
            string userName = Maps.Instance.DuradosMap.Database.GetUsernameById(oArgs.resource_id);
            PlugInHelper.SignIn(userName);
            int uid = Convert.ToInt32(oArgs.resource_id);

            //oArgs.app_name += "testqa";

            int? connectionId = null;

            string appName = oArgs.app_name;
            string pluginAppName = appName;
            bool bAppExist = Maps.Instance.AppExists(appName, uid).HasValue;
            if (bAppExist == true)
            {
                /**This user has this App/Console*/
                /***Validate Connection String !!! what about updatig connection string scenario?*/
                //Redirect to app (console)
                return SuccessResponse(0, userName, appName, pluginAppName, true);
            }
            else
            {
                //DuradosController d = new DuradosController();
                
                bool connectionValidation = false;
                bAppExist = Maps.Instance.AppExists(appName).HasValue;
                ConnectionParameter oConnectionParameter = new ConnectionParameter();
                /**If true app exists but not for this user (resource_id)*/
                
                if (bAppExist == true)
                {
                    /**Create new and unique app name*/
                    appName = GenerateAppName(appName, uid);
                }

                try
                {
                    oConnectionParameter = ValidateConnection(oArgs.db, oArgs.resource_id);
                    connectionValidation = true;
                }
                catch (Exception exception)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                    //Redirect dur to invalid connection string
                    return FailureResponse(CreateAppParameter.CODES.INVALID_CONNECTION_DATA, exception.Message, 0, userName, appName);
                }

                if (connectionValidation == true)
                {
                    try
                    {
                        string serverName = oConnectionParameter.serverName;
                        string catalog = oConnectionParameter.catalog;
                        string dbUserName = oConnectionParameter.dbUsername;
                        string dbPassword = oConnectionParameter.dbPassword;
                        int port = oConnectionParameter.port;
                        SqlProduct productId = oConnectionParameter.productId;

                        connectionId = SaveConnection(serverName, catalog, dbUserName, dbPassword, oArgs.resource_id, productId, oConnectionParameter.ssl);
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "fail to save connection string");
                        //Redirect due to create connection id for retrived connection string
                        //return View("~/Views/PlugIn/Heroku/ConnectionHandler.aspx", new ConnectionHandlerParameter() { url = "" });
                        return FailureResponse(CreateAppParameter.CODES.INVALID_CONNECTION_DATA, exception.Message, 0, userName, appName);
                    }
                }

                if (!connectionId.HasValue || connectionId == null)
                {
                    /**If not valide redirect to failure page*/
                    return FailureResponse(CreateAppParameter.CODES.INVALID_CONNECTION_DATA, "invalid ConnectionId", 0, userName, appName);
                }
             }

            /**Redirect and Create new App send new connection id*/
            return SuccessResponse(connectionId.Value, userName, oArgs.app_name, pluginAppName);
        }

        private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an Aes object 
            // with the specified key and IV. 
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

        /// <summary>
        /// Create SSO for Heroku
        /// </summary>
        /// <param name="resource_id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        //public JsonResult SSO(string resource_id, string token)
        //{
        //    HerokuParameters herokuParameters = new HerokuParameters();

        //    //Check that the token is OK
        //    string salt = System.Configuration.ConfigurationManager.AppSettings["herokukey"]; //"ac2fbb90758092e7b091eca2a6833d65";
        //    string timestamp = ((DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks) / 10000000).ToString();
        //    string myToken = sha1(resource_id + ":" + salt + ":" + timestamp);
        //    if(myToken != token)
        //        throw new DuradosException("Token is incorrect");

        //    //Auth the user and update parameters
        //    herokuParameters.resource_id = resource_id;
        //    herokuParameters.heroku_id = GetHerokuId(resource_id);
        //    herokuParameters.token = myToken;
        //    herokuParameters.is_new_user = false;

        //    //(new Durados.Web.Mvc.Controllers.FormsAuthenticationService()).SignIn(herokuParameters.heroku_id, true);
        //    PlugInHelper.SignIn(herokuParameters.heroku_id);

        //    return Json(herokuParameters);
        //}

        /// <summary>
        /// Implement sha1
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        //private string sha1(string text)
        //{

        //    System.Security.Cryptography.SHA1 hash = System.Security.Cryptography.SHA1CryptoServiceProvider.Create();
        //    byte[] plainTextBytes = Encoding.UTF8.GetBytes(text);
        //    byte[] hashBytes = hash.ComputeHash(plainTextBytes);
        //    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        //}

        /// <summary>
        /// Get herokuId which is an email username from the resource id
        /// </summary>
        /// <param name="resource_id"></param>
        /// <returns></returns>
        private string GetHerokuId(string resource_id)
        {

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters = new Dictionary<string, object>();
            parameters.Add("Id", resource_id);

            SqlAccess sqlAccess = new SqlAccess();
            return sqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.Database.ConnectionString, "Select top 1 pii.[PlugInUserId] From [dbo].[durados_PlugInInstance] pii with (NOLOCK) WHERE pii.id=@Id", parameters);
        }

    }

    public class HerokuParameters
    {
        public string resource_id { get; set; }
        public string heroku_id { get; set; }
        public string plan { get; set; }
        public string user_id { get; set; }
        public string token { get; set; }
        public bool is_new_user { get; set; }
    }

    public class CreateAppParameter
    {
        public CreateAppParameter(int connectionId = 0, string userName = null, string appName = null, string pluginAppName = null, bool app_exist = false)
        {
           this.code = CODES.OK;
            // TODO: Complete member initialization
           cid = connectionId;
           this.userName = userName;
           this.appName = appName;
           this.pluginAppName = pluginAppName;
           this.app_exist = app_exist;
           this.msg = "";
            
        }
        public CreateAppParameter(CreateAppParameter.CODES code, string msg, int connectionId = 0, string userName = null, string appName = null, string pluginAppName = null, bool app_exist = false)
        {
            // TODO: Complete member initialization
            this.code = code;
            this.msg = msg;
            this.appName = appName;
            this.pluginAppName = pluginAppName;
            this.userName = userName;
            this.app_exist = app_exist;
        }
         public CODES code { get; set; }
        public string msg { get; set; }

        public string userName { get; set; }

        public int cid { get; set; }
        public string appName { get; set; }
        public string pluginAppName { get; set; }/**Plugin name is the appName without any postfix fix for unique id purposes*/
        public bool app_exist { get; set; }

        public enum CODES {
            OK,
            INVALID_SECURITY_DATA,
            INVALID_ARGS,
            INVALID_CONNECTION_STRING,
            INVALID_CONNECTION_DATA,
            NOT_SUPPORT_PROVIDER,
            INVALID_PASSWORD,
            INVALID_USER
        } ;
    }

    public class ConnectionHandlerParameter
    {
        public int code { get; set; }
    }
    

}


