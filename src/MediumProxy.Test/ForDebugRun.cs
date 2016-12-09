using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediumProxy;
using MediumProxy.Model;
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
        public async Task GetFirstImageUriAsync()
        {
            var item = new Item("dummy")
            {
                Description =
                    "<div class=\"medium-feed-item\"><p class=\"medium-feed-image\"><a href=\"https://medium.com/@Medium/personalize-your-medium-experience-with-users-publications-tags-26a41ab1ee0c?source=rss-504c7870fdb6------2\"><img src=\"https://cdn-images-1.medium.com/max/1024/1*Y_AoLGDT2ku9uuksLQDk4w.png\" width=\"1024\"></a></p><p class=\"medium-feed-snippet\">Following users, publications, and tags on Medium</p><p class=\"medium-feed-link\"><img src=\"https://cdn-images-1.medium.com/max/1024/1*Y_AoLGDT2ku9uuksLQDk4w.png\" width=\"1024\">",
            };
            Assert.True(item.Description.Contains((await item.GetFirstImageUriAsync()).AbsoluteUri));

            item.Description = @"dummy";
            Assert.Null(await item.GetFirstImageUriAsync());

            item.Description = @"dummy";
            Assert.True(new Uri("about:blank").AbsoluteUri == (await item.GetFirstImageUriAsync(new Uri("about:blank"))).AbsoluteUri);

            item = null;
            Assert.Null(await item.GetFirstImageUriAsync());

            item = new Item("dummy");
            Assert.Null(await item.GetFirstImageUriAsync());
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
