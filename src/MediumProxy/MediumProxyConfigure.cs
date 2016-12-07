using System;
using System.Net;
using Microsoft.Extensions.Logging;

namespace MediumProxy
{
    public static class MediumProxyConfigure
    {
        public static ILoggerFactory LoggerFactory;

        public static class MediumApi
        {
            public static string EndPointFqdn { get; private set; }

            public static string UserId { get; private set; }

            static MediumApi()
            {
                EndPointFqdn = "https://medium.com/feed/";
                UserId = "@medium";
            }

            public static void SetEndPointFqdn(string endPointFqdn)
            {
                if (endPointFqdn != null) EndPointFqdn = endPointFqdn;
            }

            public static void SetUsrId(string userId)
            {
                if (userId != null) UserId = userId;
            }
        }

        public static class MediumCachePolicy
        {
            public static int CachedItemsCountHardLimit = 50;

            public static TimeSpan MonitoringThreadInterval = TimeSpan.FromSeconds(12);

            public static TimeSpan CacheLifecycleTimeSpan = TimeSpan.FromSeconds(720);

            public static double CacheLifecycleSpeculativeExecutionRate = 0.8;
        }
    }
}
