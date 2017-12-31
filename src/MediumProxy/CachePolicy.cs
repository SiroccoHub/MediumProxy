using System;
using System.Collections.Generic;
using System.Text;

namespace MediumProxy
{
    public class CachePolicy
    {
        public int CachedItemsCountHardLimit { get; set; } = 50;

        public TimeSpan MonitoringThreadInterval { get; set; } = TimeSpan.FromSeconds(12);

        public TimeSpan CacheLifecycleTimeSpan { get; set; } = TimeSpan.FromSeconds(720);

        public double CacheLifecycleSpeculativeExecutionRate { get; set; } = 0.8;
    }
}
