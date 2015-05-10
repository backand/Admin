using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Reporting.WebForms;
using System.Net;

namespace Durados.Web.Mvc.App.Reports
{
    public class ReportServerCredentials : IReportServerCredentials
    {
        private string _username = null;
        private string _password = null;
        private string _domain = null;

        public ReportServerCredentials()
        {
        }

        public ReportServerCredentials(string userName, string password, string domain)
        {
            _username = userName;
            _password = password;
            _domain = domain;
        }

        

        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string userName, out string password, out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;
            return false;
        }

        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                if (string.IsNullOrEmpty(_username))
                    return null;
                else
                    return new NetworkCredential(_username, _password, _domain);
            }
        }



    }
}

    
    

    

    
