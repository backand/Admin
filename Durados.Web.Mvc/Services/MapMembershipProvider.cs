using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Durados.Web.Mvc.Services
{
    public class MapMembershipProvider : SqlMembershipProvider
    {
        
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            NameValueCollection _config = GetMembershipProviderSettings(name);
            base.Initialize("MapSqlMembershipProvider", _config);

            // Update the private connection string field in the base class.  
            //string connectionString = "my new connection string value that I get from a custom decryption class not shown here" 
            //string connectionString = Map.securityConnectionString;
            //// Set private property of Membership provider.  
            //System.Reflection.FieldInfo connectionStringField = GetType().BaseType.GetField("_sqlConnectionString", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            //connectionStringField.SetValue(this, connectionString);
        }

        public virtual NameValueCollection GetMembershipProviderSettings(string name)
        {
            //((System.Configuration.ProviderSettings)((new System.Linq.SystemCore_EnumerableDebugView(((System.Web.Configuration.MembershipSection)(System.Configuration.ConfigurationManager.GetSection("system.web/membership"))).Providers)).Items[0])).Parameters.AllKeys

            NameValueCollection _config = ((System.Web.Configuration.MembershipSection)System.Configuration.ConfigurationManager.GetSection("system.web/membership")).Providers[0].Parameters;
                
            //_config.Add("connectionStringName", "SecurityConnectionString");
            //bool b;
            //b = (bool.TryParse(System.Web.Security.Membership.Provider.RequiresQuestionAndAnswer.ToString(), out b)) ? b : false;
            //_config.Add("requiresQuestionAndAnswer", b.ToString());
            //_config.Add("requiresQuestionAndAnswer", b.ToString());
            //_config.Add("passwordFormat", System.Web.Security.Membership.Provider.PasswordFormat.ToString() ?? "Hashed");
            //_config.Add("maxInvalidPasswordAttempts", System.Web.Security.Membership.Provider.MaxInvalidPasswordAttempts.ToString() ?? "5");
            //_config.Add("minRequiredPasswordLength", System.Web.Security.Membership.Provider.MinRequiredPasswordLength.ToString() ?? "6");
            //_config.Add("minRequiredNonalphanumericCharacters", System.Web.Security.Membership.Provider.MinRequiredNonAlphanumericCharacters.ToString() ?? "0");
            //_config.Add("passwordAttemptWindow", System.Web.Security.Membership.Provider.PasswordAttemptWindow.ToString() ?? "10");
            //_config.Add("passwordStrengthRegularExpression", System.Web.Security.Membership.Provider.PasswordStrengthRegularExpression ?? string.Empty);
            //_config.Add("applicationName", name);
            _config["applicationName"] = name;

            return _config;
        }



    }
}
