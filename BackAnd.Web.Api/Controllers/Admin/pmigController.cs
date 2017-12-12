using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Durados.Web.Mvc.UI.Helpers;

using Durados.Web.Mvc;
using Durados.DataAccess;
using Durados.Web.Mvc.Controllers.Api;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security;
using Durados.Web.Mvc.Infrastructure;
using MySql.Data.MySqlClient;
using Durados;
using System.Collections;
using System.Data;
using Backand;
using System.Web.Security;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
/*
 HTTP Verb	|Entire Collection (e.g. /customers)	                                                        |Specific Item (e.g. /customers/{id})
-----------------------------------------------------------------------------------------------------------------------------------------------
GET	        |200 (OK), list data items. Use pagination, sorting and filtering to navigate big lists.	    |200 (OK), single data item. 404 (Not Found), if ID not found or invalid.
PUT	        |404 (Not Found), unless you want to update/replace every resource in the entire collection.	|200 (OK) or 204 (No Content). 404 (Not Found), if ID not found or invalid.
POST	    |201 (Created), 'Location' header with link to /customers/{id} containing new ID.	            |404 (Not Found).
DELETE	    |404 (Not Found), unless you want to delete the whole collection—not often desirable.	        |200 (OK). 404 (Not Found), if ID not found or invalid.
 
 */

namespace BackAnd.Web.Api.Controllers
{
    [RoutePrefix("1")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize("Admin,Developer")]
    public class pmigController : apiController
    {

        [Route("~/1/mig/p")]
        [HttpGet]
        public IHttpActionResult Get(string username, string password)
        {
            try
            {
                MembershipProviderExtensions.Validate(MembershipProviderExtensions.GetMembershipUser(username), password);
                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        
        
    }


    public static class MembershipProviderExtensions
    {

        public static MembershipUser GetMembershipUser(string username)
        {
            return System.Web.Security.Membership.Provider.GetUser(username, false);
        }

        /// <summary>
        /// Switches the specified ASP.NET membership user to a clear password format, updating the associated fields.
        /// </summary>
        /// <param name="user">The membership user.</param>
        /// <exception cref="System.ArgumentNullException" />
        public static void Validate(this MembershipUser user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (Membership.Providers[user.ProviderName] is SqlMembershipProvider)
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SecurityConnectionString"].ConnectionString))
                {
                    // Get the user's (possibly encrypted or hashed) security credentials.
                    var selectQuery = "SELECT Password, PasswordFormat, PasswordSalt FROM aspnet_Membership WHERE UserId = @userId";
                    var cmd = new SqlCommand(selectQuery, connection);
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "userId", Value = user.ProviderUserKey });

                    string hashedPassword = null;
                    int passwordFormat = 0;
                    string passwordSalt = null;

                    connection.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.HasRows && reader.Read())
                    {
                        hashedPassword = (string)reader["Password"];
                        passwordFormat = (int)reader["PasswordFormat"];
                        passwordSalt = (string)reader["PasswordSalt"];
                    }
                    reader.Close();
                    connection.Close();

                    if (EncodePassword(password, passwordSalt) != hashedPassword)
                    {
                        throw new Exception("wrong");
                    }
                    
                }
            }
        }

        private static string EncodePassword(string pass, string salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = System.Convert.FromBase64String(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            byte[] inArray = null;
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            inArray = algorithm.ComputeHash(dst);
            return System.Convert.ToBase64String(inArray);
        }
    }



    /// <summary>
    /// An ACME corp-specific version of SqlMembershipProvider.
    /// </summary>
    public class AcmeCorpMembershipProvider : System.Web.Security.SqlMembershipProvider
    {
        /// <summary>
        /// Decrypts the specified password string.
        /// </summary>
        /// <param name="encryptedPassword">The encryptedPassword.</param>
        /// <returns>The decrypted password string.</returns>
        public string DecryptPassword(string encryptedPassword)
        {
            var encodedPassword = System.Convert.FromBase64String(encryptedPassword);
            var bytes = base.DecryptPassword(encodedPassword);
            return System.Text.Encoding.Unicode.GetString(bytes, 0x10, bytes.Length - 0x10);
        }
    }
}


