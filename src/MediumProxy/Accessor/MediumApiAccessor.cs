using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using MediumProxy.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MediumProxy.Accessor
{
    internal class MediumApiAccessor : IDisposable
    {
        private readonly ILogger _logger;

        private readonly HttpClient _httpClient;

        public MediumApiAccessor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MediumApiAccessor>();
            _httpClient = new HttpClient();
        }

        public MediumApiAccessor(ILoggerFactory loggerFactory, HttpMessageHandler httpMessageHandler)
        {
            _logger = loggerFactory.CreateLogger<MediumApiAccessor>();
            _httpClient = new HttpClient(httpMessageHandler);
        }

        public async Task<Channel> GetRssFeedAsync()
        {
            string result;
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{MediumProxyConfigure.MediumApi.EndPointFqdn}{MediumProxyConfigure.MediumApi.UserId}");

                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"fire Exception at {nameof(GetRssFeedAsync)}");
                _logger.LogWarning($"ex=\r\n{ex}");
                result = null;
            }
            return (result != null) ? await new MediumFeedParser().Parse(result) : null;
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
                _httpClient?.Dispose();
            }

            _disposed = true;
        }
    }
}
