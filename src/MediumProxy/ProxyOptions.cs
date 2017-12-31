using System;
using System.Net;
using Microsoft.Extensions.Logging;

namespace MediumProxy
{
    public class ProxyOptions
    {
        public string EndPointFqdn { get; set; } = "https://medium.com/feed/";

        public string UserId { get; set; } = "@medium";

        public CachePolicy CachePolicy { get; set; } = new CachePolicy();
    }
}
