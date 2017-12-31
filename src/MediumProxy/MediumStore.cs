using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediumProxy.Accessor;
using MediumProxy.Model;
using Microsoft.Extensions.Logging;

namespace MediumProxy
{
    public class MediumStore : IDisposable
    {
        private readonly ILogger _logger;
        private static ILoggerFactory _loggerFactory;

        private readonly object _sync = new object();

        private volatile MediumCache _cachedInstance;

        private const int DefaultContentCount = 10;

        private readonly ProxyOptions _proxyOptions;

        public MediumStore(ILoggerFactory loggerFactory, ProxyOptions proxyOptions)
        {
            _loggerFactory = loggerFactory;

            if (_loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<MediumStore>();

            _proxyOptions = proxyOptions ?? new ProxyOptions();
        }

        public MediumCache CachedInstance
        {
            get
            {
                if (_cachedInstance != null) return _cachedInstance;

                lock (_sync)
                {
                    // instanced
                    if (_cachedInstance == null)
                        _cachedInstance = new MediumCache(_loggerFactory, _proxyOptions);

                    // deligation refleshing cache
                    _cachedInstance.RefreshCacheExecute = async (currentContentCount) =>
                    {
                        var refreshContentCount = currentContentCount == 0
                            ? DefaultContentCount
                            : currentContentCount;

                        var results = await CallApiAndAddAsync();
                        return results.OrderByDescending(p => p.PubDate).ToList();
                    };
                }
                return _cachedInstance;
            }
        }

        /// <summary>
        /// Get Timeline from cache and online
        /// </summary>
        /// <param name="willGetCount"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Item>> GetTimelineAsync(int willGetCount = DefaultContentCount)
        {
            return (await CachedInstance.GetAllAsync()).OrderByDescending(p => p.PubDate).Take(willGetCount);
        }

        /// <summary>
        /// Get Current Timeline from online
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Item>> GetCurrentTimelineAsync()
        {
            return (await CachedInstance.GetAllAsync(true)).OrderByDescending(p => p.PubDate);
        }

        /// <summary>
        /// Get and Put Data from Calling Api
        /// </summary>
        private async Task<List<Item>> CallApiAndAddAsync()
        {
            var results = new List<Item>();
            using (var accessor = new MediumApiAccessor(_loggerFactory, _proxyOptions))
            {
                var feed = await accessor.GetRssFeedAsync();

                if (feed != null)
                    results.AddRange(feed.Items);

                _logger.LogInformation($"calling CallApiAndAdd()");
            }
            return results;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~NoteStore() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        void IDisposable.Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
