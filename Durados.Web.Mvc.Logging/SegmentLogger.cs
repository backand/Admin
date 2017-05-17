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

        private Dict GetCampaign(Dictionary<string,object> data)
        {
            
            var campaign = new Segment.Model.Dict();

            foreach (string key in data.Keys)
            {
                campaign.Add(key, data[key]);
            }

            return campaign;
        }

        public static string GetUserAgent()
        {
            return System.Web.HttpContext.Current.Request.UserAgent;
        }

        public static Dictionary<string,object> GetCampaign()
        {
            const string utm_content = "utm_content";
            const string utm_campaign = "utm_campaign";
            const string utm_medium = "utm_medium";
            const string utm_source = "utm_source";
            const string utm_term = "utm_term";

            var campaign = new Dictionary<string, object>();

            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                var query = System.Web.HttpUtility.ParseQueryString(System.Web.HttpContext.Current.Request.UrlReferrer.Query);
                if (query[utm_content] != null) campaign.Add("content", query[utm_content]);
                if (query[utm_campaign] != null) campaign.Add("name", query[utm_campaign]);
                if (query[utm_medium] != null) campaign.Add("medium", query[utm_medium]);
                if (query[utm_source] != null) campaign.Add("source", query[utm_source]);
                if (query[utm_term] != null) campaign.Add("keyword", query[utm_term]);
            }
            return campaign;
        }

        public void _Log(ExternalAnalyticsAction actioName, DateTime? timestamp, string userId, Dictionary<string, object> traits, bool identify = false, Dictionary<string, object> page = null, Dictionary<string, object> campaign = null, string userAgent = null)
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
                        { "page", GetPage(page) },
                        { "campaign", GetCampaign(campaign) },
                        { "ip", origIp },
                        {"userAgent", userAgent}}).SetTimestamp(timestamp));


                    if (actioName == ExternalAnalyticsAction.SignedUp || actioName == ExternalAnalyticsAction.SocialSignedUp)
                    {
                        Analytics.Client.Track(userId, ExternalAnalyticsAction.SignedUp.ToString(), new Properties() {
                                                { ExternalAnalyticsTraitsKey.name.ToString(), name }
                                            }, new Options().SetContext(new Context() {
                        { "page", GetPage(page) },
                        { "campaign", GetCampaign(campaign) },
                        { "ip", origIp },
                        {"userAgent", userAgent}}).SetTimestamp(timestamp));

                        string provider = !traits.ContainsKey(ExternalAnalyticsTraitsKey.provider.ToString()) ? "self" : traits[ExternalAnalyticsTraitsKey.provider.ToString()].ToString();
                        Analytics.Client.Track(userId, ExternalAnalyticsAction.SocialSignedUp.ToString(), new Properties() {
                            { ExternalAnalyticsTraitsKey.provider.ToString(), provider }
                        }, new Options().SetContext(new Context() {
                        { "page", GetPage(page) },
                        { "campaign", GetCampaign(campaign) },
                        { "ip", origIp },
                        {"userAgent", userAgent}}).SetTimestamp(timestamp));
                    }
                }




                Analytics.Client.Flush();
            }
            catch (Exception exception)
            {
                logger.Log("externalLog-Segment", string.Empty, "_Log", exception, 1, string.Empty);// System.Diagnostics.EventLogEntryType.FailureAudit, 1);
            }

        }

        public static Dictionary<string, object> GetPage()
        {
            var page = new Dictionary<string, object>();
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                page.Add("path", System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery);
                page.Add("search", System.Web.HttpContext.Current.Request.UrlReferrer.Query);
                page.Add("url", System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
            }
            return page;
        }

        private Dict GetPage(Dictionary<string, object> data)
        {
            var page = new Segment.Model.Dict();
            if (data.ContainsKey("path")) page.Add("path", (string)data["path"]);
            if (data.ContainsKey("search")) page.Add("search", (string)data["search"]);
            if (data.ContainsKey("url")) page.Add("url", (string)data["url"]);

            return page;
        }

        public void Log(ExternalAnalyticsAction actioName, DateTime? timestamp, string username, Dictionary<string, object> traits, bool identify = false, Dictionary<string, object> page = null, Dictionary<string, object> campaign = null, string userAgent = null)
        {
            try
            {
                if (!traits.ContainsKey(ExternalAnalyticsTraitsKey.ipAddress.ToString()))
                    traits.Add(ExternalAnalyticsTraitsKey.ipAddress.ToString(), Logger.UserIPAddress);

                if (async)
                    System.Threading.Tasks.Task.Run(() => _Log(actioName, timestamp, username, traits, identify, page, campaign, userAgent));

                else
                    _Log(actioName, timestamp, username, traits, identify, page, campaign, userAgent);
            }
            catch (Exception exception)
            {
                logger.Log("externalLog-Segment", string.Empty, "Log", exception, 1, string.Empty);
            }
        }

    }
}
