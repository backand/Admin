using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Cms.Services
{
    public class EmailProvider
    {
        public SMTPProvider GetSMTPProvider()
        {
            return (SMTPProvider)Enum.Parse(typeof(SMTPProvider), System.Configuration.ConfigurationManager.AppSettings["SMTPProvider"]);
        }

        public  SMTPServiceDetails GetSMTPServiceDetails(SMTPProvider smtpProvider = SMTPProvider.external)
        {
            SMTPServiceDetails smtpDetails;
            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromError"]);
            string to = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["toError"]);

            if (smtpProvider == SMTPProvider.local)
                return smtpDetails = new SMTPServiceDetails
                {
                    host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SmtpLocalHost"]),
                    port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SmtpLocalPort"]),
                    username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SmtpLocalUsername"]),
                    password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SmtpLocalPassword"]),
                    from = from,
                    to=to,
                    useDefaultCredentials =true
                };

            return smtpDetails = new SMTPServiceDetails
            {
                host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]),
                port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]),
                username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]),
                password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]),
                from = from,
                to = to,
                useDefaultCredentials = false
            };
        }
    }
    public class SMTPServiceDetails
    {
        public string host { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string from { get; set; }
        public string to { get; set; }


        public bool useDefaultCredentials { get; set; }
    }

    public enum SMTPProvider { local, external }
}
