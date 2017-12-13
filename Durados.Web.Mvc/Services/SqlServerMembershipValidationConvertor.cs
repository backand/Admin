using MySql.Data.MySqlClient;
using MySql.Web.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Durados.Web.Mvc.Services
{
    public class SqlServerMembershipValidationConvertor
    {
        public bool Validate(string username, string password)
        {
            return Validate(GetMembershipUser(username), password);
        }

        private MembershipUser GetMembershipUser(string username)
        {
            return System.Web.Security.Membership.Provider.GetUser(username, false);
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

            if (Membership.Providers[user.ProviderName] is MySQLMembershipProvider)
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SecurityConnectionString"].ConnectionString))
                {
                    // Get the user's (possibly encrypted or hashed) security credentials.
                    var selectQuery = "SELECT Password, PasswordFormat, PasswordKey FROM my_aspnet_membership WHERE UserId = @userId";
                    var cmd = new MySqlCommand(selectQuery, connection);
                    cmd.Parameters.Add(new MySqlParameter { ParameterName = "userId", Value = user.ProviderUserKey });

                    string hashedPassword = null;
                    string passwordSalt = null;

                    connection.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.HasRows && reader.Read())
                    {
                        hashedPassword = (string)reader["Password"];
                        passwordSalt = (string)reader["PasswordKey"];
                    }
                    reader.Close();
                    connection.Close();

                    return EncodePassword(password, passwordSalt) == hashedPassword;
                }
            }

            return false;
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
