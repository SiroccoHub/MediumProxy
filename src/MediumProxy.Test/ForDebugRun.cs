using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediumProxy;
using Xunit;

namespace MediumProxy.Test
{
    public class ForDebugRun
    {
        [Fact]
        public async Task GetCurrentTimelineAsync()
        {
            using (var x = new MediumStore(ApplicationLogging.LoggerFactory))
            {
                var content = (await x.GetCurrentTimelineAsync()).ToList();
                Assert.True(content.Any());
                Debug.WriteLine(content.First());
            }
        }

        [Fact]
        public async Task GetCached()
        {
            var sw = new Stopwatch();

            using (var x = new MediumStore(ApplicationLogging.LoggerFactory))
            {
                sw.Restart();

                var contents = (await x.GetTimelineAsync(5)).ToList();
                Assert.True(contents.Any());

                Debug.WriteLine("Elapsed: {0} ms", sw.ElapsedMilliseconds);
                Debug.WriteLine("content.Count()={0}", contents.Count());
                foreach (var content in contents)
                {
                    Debug.WriteLine(content);
                }
                sw.Restart();

                contents = (await x.GetTimelineAsync(5)).ToList();
                Assert.True(contents.Any());
                Assert.True(sw.ElapsedMilliseconds < 10);

                Debug.WriteLine("Elapsed: {0} ms", sw.ElapsedMilliseconds);
                Debug.WriteLine("content.Count()={0}", contents.Count());
                foreach (var content in contents)
                {
                    Debug.WriteLine(content);
                }

                sw.Restart();

                contents = (await x.GetTimelineAsync(5)).ToList();
                Assert.True(contents.Any());
                Assert.True(sw.ElapsedMilliseconds < 10);

                Debug.WriteLine("Elapsed: {0} ms", sw.ElapsedMilliseconds);
                Debug.WriteLine("content.Count()={0}", contents.Count());
                foreach (var content in contents)
                {
                    Debug.WriteLine(content);
                }
                sw.Restart();

                contents = (await x.GetTimelineAsync(10)).ToList();
                Assert.True(contents.Any());

                Debug.WriteLine("Elapsed: {0} ms", sw.ElapsedMilliseconds);
                Debug.WriteLine("content.Count()={0}", contents.Count());
                foreach (var content in contents)
                {
                    Debug.WriteLine(content);
                }

                sw.Restart();

                contents = (await x.GetTimelineAsync(10)).ToList();
                Assert.True(contents.Any());
                Assert.True(sw.ElapsedMilliseconds < 10);

                Debug.WriteLine("Elapsed: {0} ms", sw.ElapsedMilliseconds);
                Debug.WriteLine("content.Count()={0}", contents.Count());
                foreach (var content in contents)
                {
                    Debug.WriteLine(content);
                }

                sw.Restart();

                contents = (await x.GetTimelineAsync(10)).ToList();
                Assert.True(contents.Any());
                Assert.True(sw.ElapsedMilliseconds < 10);

                Debug.WriteLine("Elapsed: {0} ms", sw.ElapsedMilliseconds);
                Debug.WriteLine("content.Count()={0}", contents.Count());
                foreach (var content in contents)
                {
                    Debug.WriteLine(content);
                }
            }

            using (var x = new MediumStore(ApplicationLogging.LoggerFactory))
            {
                sw.Restart();

                var contents = (await x.GetTimelineAsync(5)).ToList();
                Assert.True(contents.Any());
                Assert.True(sw.ElapsedMilliseconds < 10);

                Debug.WriteLine("Elapsed: {0} ms", sw.ElapsedMilliseconds);
                Debug.WriteLine("content.Count()={0}", contents.Count());
                foreach (var content in contents)
                {
                    Debug.WriteLine(content);
                }
                sw.Restart();

                contents = (await x.GetTimelineAsync(5)).ToList();
                Assert.True(contents.Any());
                Assert.True(sw.ElapsedMilliseconds < 10);

                Debug.WriteLine("Elapsed: {0} ms", sw.ElapsedMilliseconds);
                Debug.WriteLine("content.Count()={0}", contents.Count());
                foreach (var content in contents)
                {
                    Debug.WriteLine(content);
                }
            }
        }
    }
}
