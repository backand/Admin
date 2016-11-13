using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Segment;
using Segment.Model;
using Newtonsoft.Json;



namespace Durados.Web.Mvc.Logging
{
    public class SegmentAnalytics : IExternalAnalytics
    {
        string writeKey = null;
        bool async = true;
        Logger logger = new Logger();
        public SegmentAnalytics()
        {

            // initialize the project #{project.owner.login}/#{project.slug}...
            Init();

        }

        public SegmentAnalytics(Logger logger)
        {
            // TODO: Complete member initialization
            this.logger = logger;
            Init();

        }
        public void Init()
        {
            try
            {
                writeKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SegmentWriteKey"] ?? "W21J7U7JO9IXajS1w292q1atMOGklmPi");
                Analytics.Dispose();
                // Segment.Logger.Handlers += LoggingHandler;

                Analytics.Initialize(writeKey, new Config().SetAsync(false));
            }
            catch (Exception ex) { }

        }
        public void _Log(ExternalAnalyticsAction actioName, DateTime? timestamp, string userId, Dictionary<string, object> traits, bool identify = false)
        {
            try
            {

                if (Analytics.Client == null)
                    Analytics.Initialize(writeKey, new Config().SetAsync(false));
                timestamp = timestamp ?? DateTime.Now;
                if (Analytics.Client == null)
                    throw new ExternalAnalyticsException("Analytics.Client faild to initail");

                if (string.IsNullOrEmpty(userId) && !traits.ContainsKey(ExternalAnalyticsTraitsKey.email.ToString()))
                {
                    throw new ExternalAnalyticsException("Analytics missing username or email");
                }


                userId = !string.IsNullOrEmpty(userId) ? userId : traits[ExternalAnalyticsTraitsKey.email.ToString()].ToString();
                string name = !traits.ContainsKey(ExternalAnalyticsTraitsKey.name.ToString()) ? userId : traits[ExternalAnalyticsTraitsKey.name.ToString()].ToString();
                if ((identify || actioName == ExternalAnalyticsAction.SignedUp || actioName == ExternalAnalyticsAction.SocialSignedUp))
                {

                    string origIp = Convert.ToString(traits[ExternalAnalyticsTraitsKey.ipAddress.ToString()] ?? null);

                    Analytics.Client.Identify(userId, new Traits() {
                        { ExternalAnalyticsTraitsKey.email.ToString(), userId },
                        { ExternalAnalyticsTraitsKey.name.ToString(), name },
                        { ExternalAnalyticsTraitsKey.signed_up_at.ToString(), DateTime.Now }
                        }, new Options().SetContext(new Context() {
                        { "ip", origIp }}).SetTimestamp(timestamp));


                    if (actioName == ExternalAnalyticsAction.SignedUp || actioName == ExternalAnalyticsAction.SocialSignedUp)
                    {
                        Analytics.Client.Track(userId, ExternalAnalyticsAction.SignedUp.ToString(), new Properties() {
                                                { ExternalAnalyticsTraitsKey.name.ToString(), name }
                                            });

                        string provider = !traits.ContainsKey(ExternalAnalyticsTraitsKey.provider.ToString()) ? "self" : traits[ExternalAnalyticsTraitsKey.provider.ToString()].ToString();
                        Analytics.Client.Track(userId, ExternalAnalyticsAction.SocialSignedUp.ToString(), new Properties() {
                            { ExternalAnalyticsTraitsKey.provider.ToString(), provider }
                        });
                    }
                }




                Analytics.Client.Flush();
            }
            catch (Exception exception)
            {
                logger.Log("externalLog-Segment", string.Empty, "_Log", exception, 1, string.Empty);// System.Diagnostics.EventLogEntryType.FailureAudit, 1);
            }

        }

        public void Log(ExternalAnalyticsAction actioName, DateTime? timestamp, string username, Dictionary<string, object> traits, bool identify = false)
        {
            try
            {
                if (!traits.ContainsKey(ExternalAnalyticsTraitsKey.ipAddress.ToString()))
                    traits.Add(ExternalAnalyticsTraitsKey.ipAddress.ToString(), logger.UserIPAddress);

                if (async)
                    System.Threading.Tasks.Task.Run(() => _Log(actioName, timestamp, username, traits, identify));

                else
                    _Log(actioName, timestamp, username, traits, identify);
            }
            catch (Exception exception)
            {
                logger.Log("externalLog-Segment", string.Empty, "Log", exception, 1, string.Empty);
            }
        }

    }
}
