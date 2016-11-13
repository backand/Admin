
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Segment.Model;
namespace Durados.Web.Mvc.Logging
{
    public interface IExternalAnalytics
    {
        void Init();
       
        void Log(ExternalAnalyticsAction externalLoggerAction, DateTime? dateTime,string username, Dictionary<string, object> traits, bool identify = false);
    }

    public static class ExternalAnalyticsFactory
    {
       
       
        public static IExternalAnalytics GetLogger(Logger logger, int externalLoggerId)
        {
            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAnalytics"] ?? "false"))
                    return new SegmentAnalytics(logger);
            }
            catch { }
           
            return null;
        }
    }


    public class ExternalAnalyticsException : DuradosException
    {
          public ExternalAnalyticsException(string message)
                : base(message)
            {

            }

          public ExternalAnalyticsException(string message, Exception innerException)
                : base(message)
            {

            }
    }
   
    public enum ExternalAnalyticsAction
    {
        SignedUp
        , SocialSignedUp
    }

    public enum ExternalAnalyticsTraitsKey
    {
        name
        ,email
        ,ipAddress
        , provider,
        signed_up_at
        
    }
}
