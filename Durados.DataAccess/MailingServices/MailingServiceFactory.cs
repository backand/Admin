using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Services;

namespace Durados.DataAccess
{
    public  class MailingServiceFactory
    {
        public IMailingService GetMailingService(int mailingServiceId)
        {
            switch (mailingServiceId)
            {
                case 1:
                    return new MailChimpService();
                    
                case 2:
                    return new OngageMailAccess();
                  
                default:
                    return null;
                    
            }
        }
    }
}
