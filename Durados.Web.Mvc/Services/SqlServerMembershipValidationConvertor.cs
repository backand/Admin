using Durados.DataAccess;
using MySql.Data.MySqlClient;
using MySql.Web.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Durados.Web.Mvc.Services
{
    public class SqlServerMembershipValidationConvertor
    {
        internal bool Validate(string userName, string password, MembershipProvider provider)
        {
            MembershipUser user = GetMembershipUser(userName, provider);
            
            if (user == null)
                return false;
                
            return Validate(user, password);
        }


        private MembershipUser GetMembershipUser(string username, MembershipProvider provider)
        {
            return provider.GetUser(username, true);
        }
       
        /// <summary>
        /// Switches the specified ASP.NET membership user to a clear password format, updating the associated fields.
        /// </summary>
        /// <param name="user">The membership user.</param>
        /// <exception cref="System.ArgumentNullException" />
        private bool Validate(MembershipUser user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            
            //if (Membership.Providers[user.ProviderName] is MySQLMembershipProvider)
            //{
            ISqlMainSchema sqlSchema = Maps.MainAppSchema;
            using (IDbConnection connection = sqlSchema.GetNewConnection(ConfigurationManager.ConnectionStrings["SecurityConnectionString"].ConnectionString))
                {
                    // Get the user's (possibly encrypted or hashed) security credentials.
                    string selectQuery = sqlSchema.GetUserSecuritySql();
                    IDbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = selectQuery;
                    IDataParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = "userId";
                    parameter.Value = user.ProviderUserKey;
                    cmd.Parameters.Add(parameter);

                    string hashedPassword = null;
                    string passwordSalt = null;

                    connection.Open();
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        hashedPassword = (string)reader["Password"];
                        passwordSalt = (string)reader["PasswordKey"];
                    }
                    reader.Close();
                    connection.Close();

                    return EncodePassword(password, passwordSalt) == hashedPassword;
                }
            //}

            //return false;
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

}
