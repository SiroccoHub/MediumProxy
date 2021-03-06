﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediumProxy.Model;
using Microsoft.Extensions.Logging;

namespace MediumProxy
{
    public class MediumCache : IDisposable
    {
        private readonly ILogger _logger;

        private readonly List<Item> _contents;
        public DateTime ModifiedDateTimeUtc { get; private set; }

        private Predicate<DateTime> RefreshCachePredicate { get; }

        private Predicate<DateTime> RefreshCacheSpeculativeExecutionPredicate { get; }

        private volatile bool _executeMonitoringThread = true;

        public Func<int, Task<List<Item>>> RefreshCacheExecute { private get; set; }

        private readonly ProxyOptions _proxyOptions;

        private bool DoRefreshing { get; set; }

        public MediumCache(ILoggerFactory loggerFactory, ProxyOptions proxyOptions)
        {

            _logger = loggerFactory.CreateLogger<MediumCache>();

            _proxyOptions = proxyOptions ?? new ProxyOptions();
            
            _contents = new List<Item>();

            RefreshCachePredicate = currentDateTime =>
            {
                var targetDt = ModifiedDateTimeUtc.Add(_proxyOptions.CachePolicy.CacheLifecycleTimeSpan);

                _logger.LogInformation($"calling delegate RefreshCachePredicate(),{targetDt},{currentDateTime}");
                return (targetDt < currentDateTime);
            };

            RefreshCacheSpeculativeExecutionPredicate = currentDateTime =>
            {
                var targetDt = ModifiedDateTimeUtc.Add(
                    new TimeSpan((long)(
                        _proxyOptions.CachePolicy.CacheLifecycleTimeSpan.Ticks
                        * _proxyOptions.CachePolicy.CacheLifecycleSpeculativeExecutionRate)));

                _logger.LogInformation($"calling delegate RefreshCacheSpeculativeExecutionPredicate(),{targetDt},{currentDateTime}");
                return (targetDt < currentDateTime);
            };

            ModifiedDateTimeUtc = DateTime.MinValue;

            // monitoring thread.
            Task.Factory.StartNew(async () =>
            {
                while (_executeMonitoringThread)
                {
                    await Task.Delay(_proxyOptions.CachePolicy.MonitoringThreadInterval);

                    _logger.LogInformation($"calling ()monitoring. _executeMonitoringThread={ _executeMonitoringThread},interval={_proxyOptions.CachePolicy.MonitoringThreadInterval}");
                    if (RefreshCacheSpeculativeExecutionPredicate(DateTime.UtcNow))
                        await RefreshCacheAsync(true);
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Add to Cache
        /// </summary>
        /// <param name="addList"></param>
        public void AddRange(ICollection<Item> addList)
        {
            _logger.LogInformation("calling AddRange()");

            using (new RwLockScope(RwLockScopes.Upgradeable))
            {
                AddRangeInternal(addList);
            }
        }

        /// <summary>
        /// Add to Cache (internal use)
        /// </summary>
        /// <param name="addList"></param>
        private void AddRangeInternal(ICollection<Item> addList)
        {
            _logger.LogInformation("calling AddRangeInternal()");

            using (new RwLockScope(RwLockScopes.Write))
            {
                _contents.RemoveAll(addList.Contains);
                _contents.AddRange(addList);

                if (_contents.Count > _proxyOptions.CachePolicy.CachedItemsCountHardLimit)
                {
                    _logger.LogWarning($"fire CachedItemsCountHardLimit,{_contents.Count},{_proxyOptions.CachePolicy.CachedItemsCountHardLimit},will delete");
                    _contents.RemoveRange(_proxyOptions.CachePolicy.CachedItemsCountHardLimit, _contents.Count - _proxyOptions.CachePolicy.CachedItemsCountHardLimit);
                }

                ModifiedDateTimeUtc = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Refresh cache called from internal
        /// </summary>
        private async Task RefreshCacheAsync(bool useNewThread = false)
        {
            _logger.LogInformation($"calling RefreshCache({useNewThread})");

            if (RefreshCacheExecute == null)
                return;

            if (DoRefreshing)
            {
                _logger.LogInformation($"doRefreshing. void return.");
                return;
            }

            DoRefreshing = true; // lose lock

            if (!useNewThread)
            {
                AddRange(await RefreshCacheExecute(_contents.Count));
                _logger.LogInformation("called RefreshCache(False)");
            }
            else
            {
                await Task.Factory.StartNew(async () =>
                 {
                     try
                     {
                         AddRange(await RefreshCacheExecute(_contents.Count));
                     }
                     catch (Exception ex)
                     {
                         _logger.LogWarning("fire Exception at RefreshCache() another threads.");
                         _logger.LogWarning($"ex=\r\n{ex}");
                     }
                     finally
                     {
                         _logger.LogInformation("called RefreshCache(True)");
                     }
                 }).ConfigureAwait(false);
            }

            DoRefreshing = false;
        }

        /// <summary>
        /// Get all Contents
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Item>> GetAllAsync(bool forceNow = false)
        {
            _logger.LogInformation("calling GetAllAsync()");

            // if cache has expired, calling Refresh.
            if (forceNow || RefreshCachePredicate(DateTime.UtcNow))
            {
                await RefreshCacheAsync(ModifiedDateTimeUtc != DateTime.MinValue);
            }
            using (new RwLockScope()) //i know it's shallow...
            {
                return _contents.ToList();
            }
        }




        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _executeMonitoringThread = false;
            }

            _disposed = true;
        }
    }
}
